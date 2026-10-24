using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEditorInternal.Profiling;
using UnityEngine;

// Dumps a saved .data profiler capture to CSV so it can be analysed outside the editor.
// Tools > Profiler > Load capture and export CSV
public static class ProfilerCsvExport
{
	const string kCapturePath = @"D:\unity\shadowgun\profiler.data";
	const string kOutDir = @"D:\unity\shadowgun\profiler_csv";

	class Agg
	{
		public double totalMs;
		public double selfMs;
		public double gcBytes;
		public double calls;
		public double maxSelfMs;
		public int frames;
		public int minDepth = int.MaxValue;
	}

	[MenuItem("Tools/Profiler/Load capture and export CSV")]
	static void LoadAndExport()
	{
		if (!File.Exists(kCapturePath))
		{
			Debug.LogError("ProfilerCsvExport: no capture at " + kCapturePath);
			return;
		}
		ProfilerDriver.ClearAllFrames();
		if (!ProfilerDriver.LoadProfile(kCapturePath, false))
		{
			Debug.LogError("ProfilerCsvExport: LoadProfile failed for " + kCapturePath);
			return;
		}
		ExportLoaded();
	}

	[MenuItem("Tools/Profiler/Export currently loaded frames to CSV")]
	static void ExportLoaded()
	{
		int first = ProfilerDriver.firstFrameIndex;
		int last = ProfilerDriver.lastFrameIndex;
		if (first < 0 || last < first)
		{
			Debug.LogError("ProfilerCsvExport: no frames loaded (first=" + first + " last=" + last + ")");
			return;
		}

		Directory.CreateDirectory(kOutDir);

		Dictionary<string, Agg> agg = new Dictionary<string, Agg>();
		StringBuilder frames = new StringBuilder();
		frames.Append("frame,frame_ms,sum_self_ms,gc_alloc_bytes,draw_calls\n");

		int frameCount = 0;
		int worstFrame = first;
		double worstMs = -1.0;

		for (int f = first; f <= last; f++)
		{
			ProfilerProperty p = new ProfilerProperty();
			p.SetRoot(f, ProfilerColumn.TotalTime, ProfilerViewType.Hierarchy);
			p.onlyShowGPUSamples = false;

			double frameMs = ParseNum(p.frameTime);
			double sumSelf = 0.0;
			double frameGc = 0.0;
			double frameDraw = 0.0;
			bool any = false;

			while (p.Next(true))
			{
				string path = p.propertyPath;
				if (string.IsNullOrEmpty(path))
				{
					continue;
				}

				double total = p.GetColumnAsSingle(ProfilerColumn.TotalTime);
				double self = p.GetColumnAsSingle(ProfilerColumn.SelfTime);
				double gc = p.GetColumnAsSingle(ProfilerColumn.GCMemory);
				double calls = p.GetColumnAsSingle(ProfilerColumn.Calls);
				double draws = p.GetColumnAsSingle(ProfilerColumn.DrawCalls);
				int depth = p.depth;

				Agg a;
				if (!agg.TryGetValue(path, out a))
				{
					a = new Agg();
					agg[path] = a;
				}
				a.totalMs += total;
				a.selfMs += self;
				a.gcBytes += gc;
				a.calls += calls;
				a.frames++;
				if (self > a.maxSelfMs)
				{
					a.maxSelfMs = self;
				}
				if (depth < a.minDepth)
				{
					a.minDepth = depth;
				}

				sumSelf += self;
				frameGc += gc;
				if (depth == 1)
				{
					frameDraw += draws;
				}
				any = true;
			}
			p.Cleanup();

			if (!any)
			{
				continue;
			}

			frameCount++;
			frames.Append(f.ToString(CultureInfo.InvariantCulture)).Append(",");
			frames.Append(frameMs.ToString("F3", CultureInfo.InvariantCulture)).Append(",");
			frames.Append(sumSelf.ToString("F3", CultureInfo.InvariantCulture)).Append(",");
			frames.Append(frameGc.ToString("F0", CultureInfo.InvariantCulture)).Append(",");
			frames.Append(frameDraw.ToString("F0", CultureInfo.InvariantCulture)).Append("\n");

			double rank = frameMs > 0.0 ? frameMs : sumSelf;
			if (rank > worstMs)
			{
				worstMs = rank;
				worstFrame = f;
			}
		}

		if (frameCount == 0)
		{
			Debug.LogError("ProfilerCsvExport: walked " + (last - first + 1) + " frames but found no samples.");
			return;
		}

		List<KeyValuePair<string, Agg>> rows = new List<KeyValuePair<string, Agg>>(agg);
		rows.Sort(delegate(KeyValuePair<string, Agg> x, KeyValuePair<string, Agg> y)
		{
			return y.Value.selfMs.CompareTo(x.Value.selfMs);
		});

		StringBuilder sb = new StringBuilder();
		sb.Append("self_ms_per_frame,total_ms_per_frame,max_self_ms,calls_per_frame,gc_bytes_per_frame,frames_seen,depth,path\n");
		for (int i = 0; i < rows.Count; i++)
		{
			Agg a = rows[i].Value;
			sb.Append((a.selfMs / frameCount).ToString("F4", CultureInfo.InvariantCulture)).Append(",");
			sb.Append((a.totalMs / frameCount).ToString("F4", CultureInfo.InvariantCulture)).Append(",");
			sb.Append(a.maxSelfMs.ToString("F4", CultureInfo.InvariantCulture)).Append(",");
			sb.Append((a.calls / frameCount).ToString("F2", CultureInfo.InvariantCulture)).Append(",");
			sb.Append((a.gcBytes / frameCount).ToString("F0", CultureInfo.InvariantCulture)).Append(",");
			sb.Append(a.frames.ToString(CultureInfo.InvariantCulture)).Append(",");
			sb.Append(a.minDepth.ToString(CultureInfo.InvariantCulture)).Append(",");
			sb.Append(Csv(rows[i].Key)).Append("\n");
		}
		File.WriteAllText(Path.Combine(kOutDir, "aggregate.csv"), sb.ToString());
		File.WriteAllText(Path.Combine(kOutDir, "frames.csv"), frames.ToString());
		WriteFrameHierarchy(worstFrame, Path.Combine(kOutDir, "worst_frame.csv"));

		StringBuilder ov = new StringBuilder();
		ov.Append("frames=").Append(frameCount).Append(" range=").Append(first).Append("..").Append(last).Append("\n");
		ov.Append("worst frame=").Append(worstFrame).Append(" at ").Append(worstMs.ToString("F2", CultureInfo.InvariantCulture)).Append(" ms\n\n");
		ov.Append(ProfilerDriver.GetOverviewText(ProfilerArea.CPU, worstFrame)).Append("\n");
		ov.Append(ProfilerDriver.GetOverviewText(ProfilerArea.Rendering, worstFrame)).Append("\n");
		ov.Append(ProfilerDriver.GetOverviewText(ProfilerArea.Memory, worstFrame)).Append("\n");
		File.WriteAllText(Path.Combine(kOutDir, "overview.txt"), ov.ToString());

		Debug.Log("ProfilerCsvExport: wrote " + rows.Count + " sample paths over " + frameCount + " frames to " + kOutDir);
	}

	static void WriteFrameHierarchy(int frame, string file)
	{
		ProfilerProperty p = new ProfilerProperty();
		p.SetRoot(frame, ProfilerColumn.TotalTime, ProfilerViewType.Hierarchy);
		p.onlyShowGPUSamples = false;
		StringBuilder sb = new StringBuilder();
		sb.Append("depth,total_ms,self_ms,calls,gc_bytes,path\n");
		while (p.Next(true))
		{
			sb.Append(p.depth.ToString(CultureInfo.InvariantCulture)).Append(",");
			sb.Append(p.GetColumnAsSingle(ProfilerColumn.TotalTime).ToString("F4", CultureInfo.InvariantCulture)).Append(",");
			sb.Append(p.GetColumnAsSingle(ProfilerColumn.SelfTime).ToString("F4", CultureInfo.InvariantCulture)).Append(",");
			sb.Append(p.GetColumnAsSingle(ProfilerColumn.Calls).ToString("F0", CultureInfo.InvariantCulture)).Append(",");
			sb.Append(p.GetColumnAsSingle(ProfilerColumn.GCMemory).ToString("F0", CultureInfo.InvariantCulture)).Append(",");
			sb.Append(Csv(p.propertyPath)).Append("\n");
		}
		p.Cleanup();
		File.WriteAllText(file, sb.ToString());
	}

	static string Csv(string s)
	{
		if (s == null)
		{
			return "";
		}
		if (s.IndexOf(',') < 0 && s.IndexOf('"') < 0 && s.IndexOf('\n') < 0)
		{
			return s;
		}
		return "\"" + s.Replace("\"", "\"\"") + "\"";
	}

	// frameTime comes back as a display string such as "16.74".
	static double ParseNum(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return 0.0;
		}
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < s.Length; i++)
		{
			char c = s[i];
			if ((c >= '0' && c <= '9') || c == '.' || (c == '-' && sb.Length == 0))
			{
				sb.Append(c);
			}
			else if (sb.Length > 0)
			{
				break;
			}
		}
		double v;
		if (double.TryParse(sb.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out v))
		{
			return v;
		}
		return 0.0;
	}
}
