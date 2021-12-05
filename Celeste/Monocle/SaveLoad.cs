using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Monocle
{
	// Token: 0x02000133 RID: 307
	public static class SaveLoad
	{
		// Token: 0x06000B11 RID: 2833 RVA: 0x0001E694 File Offset: 0x0001C894
		public static void SerializeToFile<T>(T obj, string filepath, SaveLoad.SerializeModes mode)
		{
			using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
			{
				if (mode == SaveLoad.SerializeModes.Binary)
				{
					new BinaryFormatter().Serialize(fileStream, obj);
				}
				else if (mode == SaveLoad.SerializeModes.XML)
				{
					new XmlSerializer(typeof(T)).Serialize(fileStream, obj);
				}
			}
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x0001E6FC File Offset: 0x0001C8FC
		public static bool SafeSerializeToFile<T>(T obj, string filepath, SaveLoad.SerializeModes mode)
		{
			bool result;
			try
			{
				SaveLoad.SerializeToFile<T>(obj, filepath, mode);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x0001E72C File Offset: 0x0001C92C
		public static T DeserializeFromFile<T>(string filepath, SaveLoad.SerializeModes mode)
		{
			T result;
			using (FileStream fileStream = File.OpenRead(filepath))
			{
				if (mode == SaveLoad.SerializeModes.Binary)
				{
					result = (T)((object)new BinaryFormatter().Deserialize(fileStream));
				}
				else
				{
					result = (T)((object)new XmlSerializer(typeof(T)).Deserialize(fileStream));
				}
			}
			return result;
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x0001E790 File Offset: 0x0001C990
		public static T SafeDeserializeFromFile<T>(string filepath, SaveLoad.SerializeModes mode, bool debugUnsafe = false)
		{
			if (File.Exists(filepath))
			{
				if (debugUnsafe)
				{
					return SaveLoad.DeserializeFromFile<T>(filepath, mode);
				}
				try
				{
					return SaveLoad.DeserializeFromFile<T>(filepath, mode);
				}
				catch
				{
					return default(T);
				}
			}
			return default(T);
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x0001E7E4 File Offset: 0x0001C9E4
		public static T SafeDeserializeFromFile<T>(string filepath, SaveLoad.SerializeModes mode, out bool loadError, bool debugUnsafe = false)
		{
			if (File.Exists(filepath))
			{
				if (debugUnsafe)
				{
					loadError = false;
					return SaveLoad.DeserializeFromFile<T>(filepath, mode);
				}
				try
				{
					loadError = false;
					return SaveLoad.DeserializeFromFile<T>(filepath, mode);
				}
				catch
				{
					loadError = true;
					return default(T);
				}
			}
			loadError = false;
			return default(T);
		}

		// Token: 0x020003BF RID: 959
		public enum SerializeModes
		{
			// Token: 0x04001F62 RID: 8034
			Binary,
			// Token: 0x04001F63 RID: 8035
			XML
		}
	}
}
