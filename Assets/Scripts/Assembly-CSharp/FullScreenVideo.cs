using System.Collections;
using System.IO;
using UnityEngine;
#if !UNITY_PSP2
// UnityEngine.Video (VideoPlayer) has no PS Vita support, so the module isn't
// even referenced when building for PSP2 - keep the using PSP2-only excluded.
using UnityEngine.Video;
#else
using UnityEngine.PSVita;
#endif

// Replacement for Handheld.PlayFullScreenMovie, which only exists on iOS/Android
// and is a silent no-op everywhere else. Plays a StreamingAssets video fullscreen
// and does not return until it finishes or the player skips it, matching the
// blocking behaviour of the original call.
//
// PS VITA NOTE: Unity's VideoPlayer is NOT Sony's supported path on Vita. Playback
// goes through the built-in UnityEngine.PSVita.PSVitaVideoPlayer, based on the
// usage shown in https://github.com/ParkingLotGames/Unity-PS-Vita-Video-Player.
// Mode has no dedicated fullscreen value (its FullScreen example is an empty
// stub) - only Mode.RenderToTexture is confirmed, so playback renders into a
// screen-sized RenderTexture that gets blitted fullscreen via OnGUI.
public static class FullScreenVideo
{
	public static IEnumerator Play(string fileName)
	{
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS
		Handheld.PlayFullScreenMovie(fileName, Color.black,
			FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
		yield break;
#elif UNITY_PSP2
		// RenderTexture must be allocated on the GPU before handing its native
		// pointer to PSVitaVideoPlayer.Init, otherwise it renders into nothing.
		RenderTexture vitaTexture = new RenderTexture(Screen.width, Screen.height, 0);
		vitaTexture.Create();
		GameObject vitaGo = new GameObject("_FullScreenVideoVita");
		Object.DontDestroyOnLoad(vitaGo);
		PSVitaMovieEventListener listener = vitaGo.AddComponent<PSVitaMovieEventListener>();
		listener.Texture = vitaTexture;

		// "Raw/" is the iOS StreamingAssets folder. On Vita, StreamingAssets ship in
		// Media/StreamingAssets, which Application.streamingAssetsPath points at.
		string path = Application.streamingAssetsPath + "/" + fileName;
		Debug.Log("[VIDEO] playing " + path + " (file found: " + File.Exists(path) + ")");

		PSVitaVideoPlayer.Init(vitaTexture);
		PSVitaVideoPlayer.Play(path, PSVitaVideoPlayer.Looping.None, PSVitaVideoPlayer.Mode.RenderToTexture);
		float startTime = Time.realtimeSinceStartup;
		string endReason = "movie finished";
		while (listener.IsPlaying)
		{
			PSVitaVideoPlayer.Update();
			float elapsed = Time.realtimeSinceStartup - startTime;
			// Ignore input for the first half second so the menu press that started the
			// game can't skip the movie straight away.
			if (elapsed > 0.5f && Skipped())
			{
				endReason = "skipped by player";
				break;
			}
			// No event at all from the player means it never opened the file: give up
			// rather than sit on a black screen.
			if (!listener.AnyEvent && elapsed > NoEventTimeout)
			{
				endReason = "no response from video player after " + NoEventTimeout + " s";
				break;
			}
			// Safety cap in case the stop event never arrives.
			if (elapsed > MaxPlayTime)
			{
				endReason = "safety cap of " + MaxPlayTime + " s reached";
				break;
			}
			yield return null;
		}
		Debug.Log(string.Concat("[VIDEO] ended: ", endReason, " after ", (Time.realtimeSinceStartup - startTime).ToString("F1"), " s"));
		PSVitaVideoPlayer.Stop();
		Object.Destroy(vitaGo);
		vitaTexture.Release();
		Object.Destroy(vitaTexture);
#else
		string path = Path.Combine(Application.streamingAssetsPath, fileName);
		GameObject go = new GameObject("_FullScreenVideo");
		Object.DontDestroyOnLoad(go);

		Camera cam = go.AddComponent<Camera>();
		cam.clearFlags = CameraClearFlags.SolidColor;
		cam.backgroundColor = Color.black;
		cam.cullingMask = 0;
		cam.depth = 1000f;

		AudioSource audio = go.AddComponent<AudioSource>();
		VideoPlayer vp = go.AddComponent<VideoPlayer>();
		vp.playOnAwake = false;
		vp.source = VideoSource.Url;
		vp.url = path;
		vp.renderMode = VideoRenderMode.CameraNearPlane;
		vp.targetCamera = cam;
		vp.aspectRatio = VideoAspectRatio.FitInside;
		vp.audioOutputMode = VideoAudioOutputMode.AudioSource;
		vp.EnableAudioTrack(0, true);
		vp.SetTargetAudioSource(0, audio);
		vp.Prepare();

		float timeout = Time.realtimeSinceStartup + 10f;
		while (!vp.isPrepared)
		{
			if (Time.realtimeSinceStartup > timeout)
			{
				Debug.LogWarning("FullScreenVideo: timed out preparing " + path);
				Object.Destroy(go);
				yield break;
			}
			yield return null;
		}

		vp.Play();
		yield return null;
		while (vp.isPlaying && !Skipped())
		{
			yield return null;
		}
		Object.Destroy(go);
#endif
	}

#if UNITY_PSP2
	private const float NoEventTimeout = 10f;

	// introHD.mp4 runs 3:47; the cap only matters if the stop event never arrives.
	private const float MaxPlayTime = 260f;
#endif

	private static bool Skipped()
	{
		return Input.anyKeyDown || Input.touchCount > 0;
	}

#if UNITY_PSP2
	// PSVitaVideoPlayer reports playback state via SendMessage("OnMovieEvent", ...)
	// to the GameObject that hosts this component, per the ParkingLotGames example.
	private class PSVitaMovieEventListener : MonoBehaviour
	{
		public bool IsPlaying = true;
		public bool AnyEvent;
		public RenderTexture Texture;

		private void OnMovieEvent(int eventID)
		{
			AnyEvent = true;
			Debug.Log("[VIDEO] event " + (PSVitaVideoPlayer.MovieEvent)eventID + " (" + eventID + ")");
			if ((PSVitaVideoPlayer.MovieEvent)eventID == PSVitaVideoPlayer.MovieEvent.STOP)
			{
				IsPlaying = false;
			}
		}

		private void OnGUI()
		{
			Rect fullScreen = new Rect(0f, 0f, Screen.width, Screen.height);
			GUI.DrawTexture(fullScreen, Texture2D.blackTexture);
			// The Vita video player writes frames top-down, but Unity draws textures
			// bottom-up, so draw it with the V coordinate flipped. The texture is
			// screen-sized, so no aspect fitting is needed.
			GUI.DrawTextureWithTexCoords(fullScreen, Texture, new Rect(0f, 1f, 1f, -1f));
		}
	}
#endif
}
