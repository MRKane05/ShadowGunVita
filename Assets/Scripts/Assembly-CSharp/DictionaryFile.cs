using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DictionaryFile
{
	private Dictionary<string, object> dict = new Dictionary<string, object>();

	public void SetInt(string key, int val)
	{
		Add(key, val);
	}

	public void SetString(string key, string val)
	{
		Add(key, val);
	}

	public void SetFloat(string key, float val)
	{
		Add(key, val);
	}

	private void Add(string key, object val)
	{
		dict.Add(key, val);
	}

	public int GetInt(string key, int defVal)
	{
		if (dict.ContainsKey(key))
		{
			return (int)dict[key];
		}
		return defVal;
	}

	public string GetString(string key, string defVal)
	{
		if (dict.ContainsKey(key))
		{
			return (string)dict[key];
		}
		return defVal;
	}

	public float GetFloat(string key, float defVal)
	{
		if (dict.ContainsKey(key))
		{
			return (float)dict[key];
		}
		return defVal;
	}

	public bool HasKey(string key)
	{
		return dict.ContainsKey(key);
	}

	public void Save(string filename)
	{
		ArrayList arrayList = new ArrayList();
		ArrayList arrayList2 = new ArrayList();
		ArrayList arrayList3 = new ArrayList();
		foreach (KeyValuePair<string, object> item in dict)
		{
			if (item.Value is string)
			{
				KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(item.Key, item.Value as string);
				arrayList.Add(keyValuePair);
			}
			else if (item.Value is int)
			{
				KeyValuePair<string, int> keyValuePair2 = new KeyValuePair<string, int>(item.Key, (int)item.Value);
				arrayList2.Add(keyValuePair2);
			}
			else if (item.Value is float)
			{
				KeyValuePair<string, float> keyValuePair3 = new KeyValuePair<string, float>(item.Key, (float)item.Value);
				arrayList3.Add(keyValuePair3);
			}
		}
		using (FileStream output = File.OpenWrite(filename))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(output))
			{
				binaryWriter.Write(arrayList.Count);
				foreach (KeyValuePair<string, string> item2 in arrayList)
				{
					binaryWriter.Write(item2.Key);
					binaryWriter.Write(item2.Value);
				}
				binaryWriter.Write(arrayList2.Count);
				foreach (KeyValuePair<string, int> item3 in arrayList2)
				{
					binaryWriter.Write(item3.Key);
					binaryWriter.Write(item3.Value);
				}
				binaryWriter.Write(arrayList3.Count);
				foreach (KeyValuePair<string, float> item4 in arrayList3)
				{
					binaryWriter.Write(item4.Key);
					binaryWriter.Write(item4.Value);
				}
			}
		}
	}

	public void Load(string filename)
	{
		dict = new Dictionary<string, object>();
		using (FileStream input = File.OpenRead(filename))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				int num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					string key = binaryReader.ReadString();
					string value = binaryReader.ReadString();
					dict[key] = value;
				}
				int num2 = binaryReader.ReadInt32();
				for (int j = 0; j < num2; j++)
				{
					string key2 = binaryReader.ReadString();
					int num3 = binaryReader.ReadInt32();
					dict[key2] = num3;
				}
				int num4 = binaryReader.ReadInt32();
				for (int k = 0; k < num4; k++)
				{
					string key3 = binaryReader.ReadString();
					float num5 = binaryReader.ReadSingle();
					dict[key3] = num5;
				}
			}
		}
	}
}
