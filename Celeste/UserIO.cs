using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using Monocle;

namespace Celeste
{
	// Token: 0x0200038E RID: 910
	public static class UserIO
	{
		// Token: 0x06001D7D RID: 7549 RVA: 0x000CD13F File Offset: 0x000CB33F
		private static string GetHandle(string name)
		{
			return Path.Combine("Saves", name + ".celeste");
		}

		// Token: 0x06001D7E RID: 7550 RVA: 0x000CD156 File Offset: 0x000CB356
		private static string GetBackupHandle(string name)
		{
			return Path.Combine("Backups", name + ".celeste");
		}

		// Token: 0x06001D7F RID: 7551 RVA: 0x000CD16D File Offset: 0x000CB36D
		public static bool Open(UserIO.Mode mode)
		{
			return true;
		}

		// Token: 0x06001D80 RID: 7552 RVA: 0x000CD170 File Offset: 0x000CB370
		public static bool Save<T>(string path, byte[] data) where T : class
		{
			string handle = UserIO.GetHandle(path);
			bool flag = false;
			try
			{
				string backupHandle = UserIO.GetBackupHandle(path);
				DirectoryInfo directory = new FileInfo(handle).Directory;
				if (!directory.Exists)
				{
					directory.Create();
				}
				directory = new FileInfo(backupHandle).Directory;
				if (!directory.Exists)
				{
					directory.Create();
				}
				using (FileStream fileStream = File.Open(backupHandle, FileMode.Create, FileAccess.Write))
				{
					fileStream.Write(data, 0, data.Length);
				}
				if (UserIO.Load<T>(path, true) != null)
				{
					File.Copy(backupHandle, handle, true);
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR: " + ex.ToString());
				ErrorLog.Write(ex);
			}
			if (!flag)
			{
				Console.WriteLine("Save Failed");
			}
			return flag;
		}

		// Token: 0x06001D81 RID: 7553 RVA: 0x000CD24C File Offset: 0x000CB44C
		public static T Load<T>(string path, bool backup = false) where T : class
		{
			string path2 = (!backup) ? UserIO.GetHandle(path) : UserIO.GetBackupHandle(path);
			T result = default(T);
			try
			{
				if (File.Exists(path2))
				{
					using (FileStream fileStream = File.OpenRead(path2))
					{
						result = UserIO.Deserialize<T>(fileStream);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR: " + ex.ToString());
				ErrorLog.Write(ex);
			}
			return result;
		}

		// Token: 0x06001D82 RID: 7554 RVA: 0x000CD2D4 File Offset: 0x000CB4D4
		private static T Deserialize<T>(Stream stream) where T : class
		{
			return (T)((object)new XmlSerializer(typeof(T)).Deserialize(stream));
		}

		// Token: 0x06001D83 RID: 7555 RVA: 0x000CD2F0 File Offset: 0x000CB4F0
		public static bool Exists(string path)
		{
			return File.Exists(UserIO.GetHandle(path));
		}

		// Token: 0x06001D84 RID: 7556 RVA: 0x000CD300 File Offset: 0x000CB500
		public static bool Delete(string path)
		{
			string handle = UserIO.GetHandle(path);
			if (File.Exists(handle))
			{
				File.Delete(handle);
				return true;
			}
			return false;
		}

		// Token: 0x06001D85 RID: 7557 RVA: 0x000091E2 File Offset: 0x000073E2
		public static void Close()
		{
		}

		// Token: 0x06001D86 RID: 7558 RVA: 0x000CD328 File Offset: 0x000CB528
		public static byte[] Serialize<T>(T instance)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new XmlSerializer(typeof(T)).Serialize(memoryStream, instance);
				result = memoryStream.ToArray();
			}
			return result;
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06001D87 RID: 7559 RVA: 0x000CD37C File Offset: 0x000CB57C
		// (set) Token: 0x06001D88 RID: 7560 RVA: 0x000CD383 File Offset: 0x000CB583
		public static bool Saving { get; private set; }

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06001D89 RID: 7561 RVA: 0x000CD38B File Offset: 0x000CB58B
		// (set) Token: 0x06001D8A RID: 7562 RVA: 0x000CD392 File Offset: 0x000CB592
		public static bool SavingResult { get; private set; }

		// Token: 0x06001D8B RID: 7563 RVA: 0x000CD39A File Offset: 0x000CB59A
		public static void SaveHandler(bool file, bool settings)
		{
			if (!UserIO.Saving)
			{
				UserIO.Saving = true;
				Celeste.SaveRoutine = new Coroutine(UserIO.SaveRoutine(file, settings), true);
			}
		}

		// Token: 0x06001D8C RID: 7564 RVA: 0x000CD3BB File Offset: 0x000CB5BB
		private static IEnumerator SaveRoutine(bool file, bool settings)
		{
			UserIO.savingFile = file;
			UserIO.savingSettings = settings;
			FileErrorOverlay menu;
			do
			{
				if (UserIO.savingFile)
				{
					SaveData.Instance.BeforeSave();
					UserIO.savingFileData = UserIO.Serialize<SaveData>(SaveData.Instance);
				}
				if (UserIO.savingSettings)
				{
					UserIO.savingSettingsData = UserIO.Serialize<Settings>(Settings.Instance);
				}
				UserIO.savingInternal = true;
				UserIO.SavingResult = false;
				RunThread.Start(new Action(UserIO.SaveThread), "USER_IO", false);
				SaveLoadIcon.Show(Engine.Scene);
				while (UserIO.savingInternal)
				{
					yield return null;
				}
				SaveLoadIcon.Hide();
				if (UserIO.SavingResult)
				{
					goto IL_110;
				}
				menu = new FileErrorOverlay(FileErrorOverlay.Error.Save);
				while (menu.Open)
				{
					yield return null;
				}
			}
			while (menu.TryAgain);
			menu = null;
			IL_110:
			UserIO.Saving = false;
			Celeste.SaveRoutine = null;
			yield break;
		}

		// Token: 0x06001D8D RID: 7565 RVA: 0x000CD3D4 File Offset: 0x000CB5D4
		private static void SaveThread()
		{
			UserIO.SavingResult = false;
			if (UserIO.Open(UserIO.Mode.Write))
			{
				UserIO.SavingResult = true;
				if (UserIO.savingFile)
				{
					UserIO.SavingResult &= UserIO.Save<SaveData>(SaveData.GetFilename(), UserIO.savingFileData);
				}
				if (UserIO.savingSettings)
				{
					UserIO.SavingResult &= UserIO.Save<Settings>("settings", UserIO.savingSettingsData);
				}
				UserIO.Close();
			}
			UserIO.savingInternal = false;
		}

		// Token: 0x04001E50 RID: 7760
		public const string SaveDataTitle = "Celeste Save Data";

		// Token: 0x04001E51 RID: 7761
		private const string SavePath = "Saves";

		// Token: 0x04001E52 RID: 7762
		private const string BackupPath = "Backups";

		// Token: 0x04001E53 RID: 7763
		private const string Extension = ".celeste";

		// Token: 0x04001E56 RID: 7766
		private static bool savingInternal;

		// Token: 0x04001E57 RID: 7767
		private static bool savingFile;

		// Token: 0x04001E58 RID: 7768
		private static bool savingSettings;

		// Token: 0x04001E59 RID: 7769
		private static byte[] savingFileData;

		// Token: 0x04001E5A RID: 7770
		private static byte[] savingSettingsData;

		// Token: 0x02000756 RID: 1878
		public enum Mode
		{
			// Token: 0x04002EDF RID: 11999
			Read,
			// Token: 0x04002EE0 RID: 12000
			Write
		}
	}
}
