using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Celeste.Editor;
using Celeste.Pico8;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200030B RID: 779
	public static class Commands
	{
		// Token: 0x06001843 RID: 6211 RVA: 0x0009860C File Offset: 0x0009680C
		[Command("global_stats", "logs global steam stats")]
		private static void CmdGlobalStats()
		{
			foreach (object obj in Enum.GetValues(typeof(Stat)))
			{
				Stat stat = (Stat)obj;
				Engine.Commands.Log(stat.ToString() + ": " + Stats.Global(stat));
			}
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x00098694 File Offset: 0x00096894
		[Command("export_dialog", "export dialog files to binary format")]
		private static void CmdExportDialog()
		{
			foreach (string text in Directory.EnumerateFiles(Path.Combine("Content", "Dialog"), "*.txt"))
			{
				if (text.EndsWith(".txt"))
				{
					Language language = Language.FromTxt(text);
					language.Export(text + ".export");
					language.Dispose();
				}
			}
		}

		// Token: 0x06001845 RID: 6213 RVA: 0x00098718 File Offset: 0x00096918
		[Command("give_golden", "gives you a golden strawb")]
		private static void CmdGiveGolden()
		{
			Level level = Engine.Scene as Level;
			if (level != null)
			{
				Player entity = level.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					EntityData entityData = new EntityData();
					entityData.Position = entity.Position + new Vector2(0f, -16f);
					entityData.ID = Calc.Random.Next();
					entityData.Name = "goldenBerry";
					EntityID gid = new EntityID(level.Session.Level, entityData.ID);
					Strawberry entity2 = new Strawberry(entityData, Vector2.Zero, gid);
					level.Add(entity2);
				}
			}
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x000987B4 File Offset: 0x000969B4
		[Command("unlock_doors", "unlock all lockblocks")]
		private static void CmdUnlockDoors()
		{
			foreach (LockBlock lockBlock in (Engine.Scene as Level).Entities.FindAll<LockBlock>())
			{
				lockBlock.RemoveSelf();
			}
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x00098814 File Offset: 0x00096A14
		[Command("ltng", "disable lightning")]
		private static void CmdLightning(bool disabled = true)
		{
			(Engine.Scene as Level).Session.SetFlag("disable_lightning", disabled);
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x00098830 File Offset: 0x00096A30
		[Command("bounce", "bounces the player!")]
		private static void CmdBounce()
		{
			Player entity = Engine.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				entity.Bounce(entity.Bottom);
			}
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x0009885C File Offset: 0x00096A5C
		[Command("sound_instances", "gets active sound count")]
		private static void CmdSounds()
		{
			int num = 0;
			foreach (KeyValuePair<string, EventDescription> keyValuePair in Audio.cachedEventDescriptions)
			{
				int num2;
				keyValuePair.Value.getInstanceCount(out num2);
				if (num2 > 0)
				{
					string arg;
					keyValuePair.Value.getPath(out arg);
					Engine.Commands.Log(arg + ": " + num2);
					Console.WriteLine(arg + ": " + num2);
				}
				num += num2;
			}
			Engine.Commands.Log("total: " + num);
			Console.WriteLine("total: " + num);
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x00098934 File Offset: 0x00096B34
		[Command("lighting", "checks lightiing values")]
		private static void CmdLighting()
		{
			Level level = Engine.Scene as Level;
			if (level != null)
			{
				Engine.Commands.Log(string.Concat(new object[]
				{
					"base(",
					level.BaseLightingAlpha,
					"), session add(",
					level.Session.LightingAlphaAdd,
					"), current (",
					level.Lighting.Alpha,
					")"
				}));
			}
		}

		// Token: 0x0600184B RID: 6219 RVA: 0x000989B8 File Offset: 0x00096BB8
		[Command("detailed_levels", "counts detailed levels")]
		private static void CmdDetailedLevels(int area = -1, int mode = 0)
		{
			if (area == -1)
			{
				int num = 0;
				int num2 = 0;
				foreach (AreaData areaData in AreaData.Areas)
				{
					for (int i = 0; i < areaData.Mode.Length; i++)
					{
						ModeProperties modeProperties = areaData.Mode[i];
						if (modeProperties != null)
						{
							foreach (LevelData levelData in modeProperties.MapData.Levels)
							{
								if (!levelData.Dummy)
								{
									num++;
									if (levelData.BgDecals.Count + levelData.FgDecals.Count >= 2)
									{
										num2++;
									}
								}
							}
						}
					}
				}
				Engine.Commands.Log(num2 + " / " + num);
				return;
			}
			int num3 = 0;
			int num4 = 0;
			List<string> list = new List<string>();
			foreach (LevelData levelData2 in AreaData.GetMode(area, (AreaMode)mode).MapData.Levels)
			{
				if (!levelData2.Dummy)
				{
					num3++;
					if (levelData2.BgDecals.Count + levelData2.FgDecals.Count >= 2)
					{
						num4++;
					}
					else
					{
						list.Add(levelData2.Name);
					}
				}
			}
			Engine.Commands.Log(string.Join(", ", list), Color.Red);
			Engine.Commands.Log(num4 + " / " + num3);
		}

		// Token: 0x0600184C RID: 6220 RVA: 0x00098BA4 File Offset: 0x00096DA4
		[Command("hearts", "gives a certain number of hearts (default all)")]
		private static void CmdHearts(int amount = 24)
		{
			int num = 0;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < SaveData.Instance.Areas.Count; j++)
				{
					AreaModeStats areaModeStats = SaveData.Instance.Areas[j].Modes[i];
					if (areaModeStats != null)
					{
						if (num < amount)
						{
							num++;
							areaModeStats.HeartGem = true;
						}
						else
						{
							areaModeStats.HeartGem = false;
						}
					}
				}
			}
			Calc.Log(new object[]
			{
				SaveData.Instance.TotalHeartGems
			});
		}

		// Token: 0x0600184D RID: 6221 RVA: 0x00098C2C File Offset: 0x00096E2C
		[Command("logsession", "log session to output")]
		private static void CmdLogSession()
		{
			Session session = (Engine.Scene as Level).Session;
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Session));
			StringWriter stringWriter = new StringWriter();
			xmlSerializer.Serialize(stringWriter, session);
			Console.WriteLine(stringWriter.ToString());
		}

		// Token: 0x0600184E RID: 6222 RVA: 0x00098C70 File Offset: 0x00096E70
		[Command("postcard", "views a postcard")]
		private static void CmdPostcard(string id, int area = 1)
		{
			Engine.Scene = new PreviewPostcard(new Postcard(Dialog.Get("POSTCARD_" + id, null), area));
		}

		// Token: 0x0600184F RID: 6223 RVA: 0x00098C93 File Offset: 0x00096E93
		[Command("postcard_cside", "views a postcard")]
		private static void CmdPostcardCside()
		{
			Engine.Scene = new PreviewPostcard(new Postcard(Dialog.Get("POSTCARD_CSIDES", null), "event:/ui/main/postcard_csides_in", "event:/ui/main/postcard_csides_out"));
		}

		// Token: 0x06001850 RID: 6224 RVA: 0x00098CB9 File Offset: 0x00096EB9
		[Command("postcard_variants", "views a postcard")]
		private static void CmdPostcardVariants()
		{
			Engine.Scene = new PreviewPostcard(new Postcard(Dialog.Get("POSTCARD_VARIANTS", null), "event:/new_content/ui/postcard_variants_in", "event:/new_content/ui/postcard_variants_out"));
		}

		// Token: 0x06001851 RID: 6225 RVA: 0x00098CE0 File Offset: 0x00096EE0
		[Command("check_all_languages", "compares all langauges to english")]
		private static void CmdCheckLangauges(bool compareContent = false)
		{
			Engine.Commands.Log("---------------------");
			bool flag = true;
			foreach (KeyValuePair<string, Language> keyValuePair in Dialog.Languages)
			{
				flag &= Commands.CmdCheckLangauge(keyValuePair.Key, compareContent);
			}
			Engine.Commands.Log("---------------------");
			Engine.Commands.Log("REUSLT: " + flag.ToString(), flag ? Color.LawnGreen : Color.Red);
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x00098D88 File Offset: 0x00096F88
		[Command("check_language", "compares all langauges to english")]
		private static bool CmdCheckLangauge(string id, bool compareContent = false)
		{
			bool flag = true;
			Language language = Dialog.Languages[id];
			Language language2 = Dialog.Languages["english"];
			bool flag2 = language.FontFace != language2.FontFace && (Settings.Instance == null || language.FontFace != Dialog.Languages[Settings.Instance.Language].FontFace);
			if (flag2)
			{
				Fonts.Load(language.FontFace);
			}
			bool flag3 = Dialog.CheckLanguageFontCharacters(id);
			bool flag4 = Dialog.CompareLanguages("english", id, compareContent);
			if (flag2)
			{
				Fonts.Unload(language.FontFace);
			}
			Engine.Commands.Log(string.Concat(new string[]
			{
				id,
				" [FONT: ",
				flag3.ToString(),
				", MATCH: ",
				flag4.ToString(),
				"]"
			}), (flag3 && flag4) ? Color.White : Color.Red);
			return flag && flag3 && flag4;
		}

		// Token: 0x06001853 RID: 6227 RVA: 0x00098E87 File Offset: 0x00097087
		[Command("characters", "gets all the characters of each text file (writes to console")]
		private static void CmdTextCharacters()
		{
			Dialog.CheckCharacters();
		}

		// Token: 0x06001854 RID: 6228 RVA: 0x00098E90 File Offset: 0x00097090
		[Command("berries_order", "checks strawbs order")]
		private static void CmdBerriesOrder()
		{
			foreach (AreaData areaData in AreaData.Areas)
			{
				for (int i = 0; i < areaData.Mode.Length; i++)
				{
					if (areaData.Mode[i] != null)
					{
						HashSet<string> hashSet = new HashSet<string>();
						EntityData[,] array = new EntityData[10, 25];
						foreach (EntityData entityData in areaData.Mode[i].MapData.Strawberries)
						{
							int num = entityData.Int("checkpointID", 0);
							int num2 = entityData.Int("order", 0);
							string item = num + ":" + num2;
							if (hashSet.Contains(item))
							{
								Engine.Commands.Log(string.Concat(new object[]
								{
									"Conflicting Berry: Area[",
									areaData.ID,
									"] Mode[",
									i,
									"] Checkpoint[",
									num,
									"] Order[",
									num2,
									"]"
								}), Color.Red);
							}
							else
							{
								hashSet.Add(item);
							}
							array[num, num2] = entityData;
						}
						for (int j = 0; j < array.GetLength(0); j++)
						{
							for (int k = 1; k < array.GetLength(1); k++)
							{
								if (array[j, k] != null && array[j, k - 1] == null)
								{
									Engine.Commands.Log(string.Concat(new object[]
									{
										"Missing Berry Order #",
										k - 1,
										": Area[",
										areaData.ID,
										"] Mode[",
										i,
										"] Checkpoint[",
										j,
										"]"
									}), Color.Red);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001855 RID: 6229 RVA: 0x00099110 File Offset: 0x00097310
		[Command("ow_reflection_fall", "tests reflection overworld fall cutscene")]
		private static void CmdOWReflectionFall()
		{
			Engine.Scene = new OverworldReflectionsFall(null, delegate()
			{
				Engine.Scene = new OverworldLoader(Overworld.StartMode.Titlescreen, null);
			});
		}

		// Token: 0x06001856 RID: 6230 RVA: 0x0009913C File Offset: 0x0009733C
		[Command("core", "set the core mode of the level")]
		private static void CmdCore(int mode = 0, bool session = false)
		{
			(Engine.Scene as Level).CoreMode = (Session.CoreModes)mode;
			if (session)
			{
				(Engine.Scene as Level).Session.CoreMode = (Session.CoreModes)mode;
			}
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x00099168 File Offset: 0x00097368
		[Command("audio", "checks audio state of session")]
		private static void CmdAudio()
		{
			if (Engine.Scene is Level)
			{
				AudioState audio = (Engine.Scene as Level).Session.Audio;
				Engine.Commands.Log("MUSIC: " + audio.Music.Event, Color.Green);
				foreach (MEP mep in audio.Music.Parameters)
				{
					Engine.Commands.Log(string.Concat(new object[]
					{
						"    ",
						mep.Key,
						" = ",
						mep.Value
					}));
				}
				Engine.Commands.Log("AMBIENCE: " + audio.Ambience.Event, Color.Green);
				foreach (MEP mep2 in audio.Ambience.Parameters)
				{
					Engine.Commands.Log(string.Concat(new object[]
					{
						"    ",
						mep2.Key,
						" = ",
						mep2.Value
					}));
				}
			}
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x000992E0 File Offset: 0x000974E0
		[Command("heartgem", "give heart gem")]
		private static void CmdHeartGem(int area, int mode, bool gem = true)
		{
			SaveData.Instance.Areas[area].Modes[mode].HeartGem = gem;
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x00099300 File Offset: 0x00097500
		[Command("summitgem", "gives summit gem")]
		private static void CmdSummitGem(string gem)
		{
			if (gem == "all")
			{
				for (int i = 0; i < 6; i++)
				{
					(Engine.Scene as Level).Session.SummitGems[i] = true;
				}
				return;
			}
			(Engine.Scene as Level).Session.SummitGems[int.Parse(gem)] = true;
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x0009935C File Offset: 0x0009755C
		[Command("screenpadding", "sets level screenpadding")]
		private static void CmdScreenPadding(int value)
		{
			Level level = Engine.Scene as Level;
			if (level != null)
			{
				level.ScreenPadding = (float)value;
			}
		}

		// Token: 0x0600185B RID: 6235 RVA: 0x0009937F File Offset: 0x0009757F
		[Command("textures", "counts textures in memory")]
		private static void CmdTextures()
		{
			Engine.Commands.Log(VirtualContent.Count);
			VirtualContent.BySize();
		}

		// Token: 0x0600185C RID: 6236 RVA: 0x0009939C File Offset: 0x0009759C
		[Command("givekey", "creates a key on the player")]
		private static void CmdGiveKey()
		{
			Player entity = Engine.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				Level level = Engine.Scene as Level;
				Key key = new Key(entity, new EntityID("unknown", 1073741823 + Calc.Random.Next(10000)));
				level.Add(key);
				level.Session.Keys.Add(key.ID);
			}
		}

		// Token: 0x0600185D RID: 6237 RVA: 0x0009940C File Offset: 0x0009760C
		[Command("ref_fall", "test the reflection fall sequence")]
		private static void CmdRefFall()
		{
			SaveData.InitializeDebugMode(true);
			Session session = new Session(new AreaKey(6, AreaMode.Normal), null, null);
			session.Level = "04";
			Engine.Scene = new LevelLoader(session, new Vector2?(session.GetSpawnPoint(new Vector2((float)session.LevelData.Bounds.Center.X, (float)session.LevelData.Bounds.Top))))
			{
				PlayerIntroTypeOverride = new Player.IntroTypes?(Player.IntroTypes.Fall),
				Level = 
				{
					new BackgroundFadeIn(Color.Black, 2f, 30f)
				}
			};
		}

		// Token: 0x0600185E RID: 6238 RVA: 0x000994A8 File Offset: 0x000976A8
		[Command("lines", "Counts Dialog Lines")]
		private static void CmdLines(string language)
		{
			if (string.IsNullOrEmpty(language))
			{
				language = Dialog.Language.Id;
			}
			if (Dialog.Languages.ContainsKey(language))
			{
				Engine.Commands.Log(string.Concat(new object[]
				{
					language,
					": ",
					Dialog.Languages[language].Lines,
					" lines, ",
					Dialog.Languages[language].Words,
					" words"
				}));
				return;
			}
			Engine.Commands.Log("language '" + language + "' doesn't exist");
		}

		// Token: 0x0600185F RID: 6239 RVA: 0x00099552 File Offset: 0x00097752
		[Command("leaf", "play the leaf minigame")]
		private static void CmdLeaf()
		{
			Engine.Scene = new TestBreathingGame();
		}

		// Token: 0x06001860 RID: 6240 RVA: 0x0009955E File Offset: 0x0009775E
		[Command("wipes", "plays screen wipes for kert")]
		private static void CmdWipes()
		{
			Engine.Scene = new TestWipes();
		}

		// Token: 0x06001861 RID: 6241 RVA: 0x0009956A File Offset: 0x0009776A
		[Command("pico", "plays pico-8 game, optional room skip (x/y)")]
		private static void CmdPico(int roomX = 0, int roomY = 0)
		{
			Engine.Scene = new Emulator(null, roomX, roomY);
		}

		// Token: 0x06001862 RID: 6242 RVA: 0x00099579 File Offset: 0x00097779
		[Command("colorgrading", "sets color grading enabled (true/false)")]
		private static void CmdColorGrading(bool enabled)
		{
			ColorGrade.Enabled = enabled;
		}

		// Token: 0x06001863 RID: 6243 RVA: 0x00099581 File Offset: 0x00097781
		[Command("portraits", "portrait debugger")]
		private static void CmdPortraits()
		{
			Engine.Scene = new PreviewPortrait(64f);
		}

		// Token: 0x06001864 RID: 6244 RVA: 0x00099592 File Offset: 0x00097792
		[Command("dialog", "dialog debugger")]
		private static void CmdDialog()
		{
			Engine.Scene = new PreviewDialog(null, 64f, 0f, null);
		}

		// Token: 0x06001865 RID: 6245 RVA: 0x000995AA File Offset: 0x000977AA
		[Command("titlescreen", "go to the titlescreen")]
		private static void CmdTitlescreen()
		{
			Engine.Scene = new OverworldLoader(Overworld.StartMode.Titlescreen, null);
		}

		// Token: 0x06001866 RID: 6246 RVA: 0x000995B8 File Offset: 0x000977B8
		[Command("time", "set the time speed")]
		private static void CmdTime(float rate = 1f)
		{
			Engine.TimeRate = rate;
		}

		// Token: 0x06001867 RID: 6247 RVA: 0x000995C0 File Offset: 0x000977C0
		[Command("load", "test a level")]
		private static void CmdLoad(int id = 0, string level = null)
		{
			SaveData.InitializeDebugMode(true);
			SaveData.Instance.LastArea = new AreaKey(id, AreaMode.Normal);
			Session session = new Session(new AreaKey(id, AreaMode.Normal), null, null);
			if (level != null && session.MapData.Get(level) != null)
			{
				session.Level = level;
				session.FirstLevel = false;
			}
			Engine.Scene = new LevelLoader(session, null);
		}

		// Token: 0x06001868 RID: 6248 RVA: 0x00099628 File Offset: 0x00097828
		[Command("hard", "test a hard level")]
		private static void CmdHard(int id = 0, string level = null)
		{
			SaveData.InitializeDebugMode(true);
			SaveData.Instance.LastArea = new AreaKey(id, AreaMode.BSide);
			Session session = new Session(new AreaKey(id, AreaMode.BSide), null, null);
			if (level != null)
			{
				session.Level = level;
				session.FirstLevel = false;
			}
			Engine.Scene = new LevelLoader(session, null);
		}

		// Token: 0x06001869 RID: 6249 RVA: 0x00099680 File Offset: 0x00097880
		[Command("music_progress", "set music progress value")]
		private static void CmdMusProgress(int progress)
		{
			Audio.SetMusicParam("progress", (float)progress);
		}

		// Token: 0x0600186A RID: 6250 RVA: 0x00099690 File Offset: 0x00097890
		[Command("rmx2", "test a RMX2 level")]
		private static void CmdRMX2(int id = 0, string level = null)
		{
			SaveData.InitializeDebugMode(true);
			SaveData.Instance.LastArea = new AreaKey(id, AreaMode.CSide);
			Session session = new Session(new AreaKey(id, AreaMode.CSide), null, null);
			if (level != null)
			{
				session.Level = level;
				session.FirstLevel = false;
			}
			Engine.Scene = new LevelLoader(session, null);
		}

		// Token: 0x0600186B RID: 6251 RVA: 0x000996E8 File Offset: 0x000978E8
		[Command("complete", "test the complete screen for an area")]
		private static void CmdComplete(int index = 1, int mode = 0, int deaths = 0, int strawberries = 0, bool gem = false)
		{
			if (SaveData.Instance == null)
			{
				SaveData.InitializeDebugMode(true);
				SaveData.Instance.CurrentSession = new Session(AreaKey.Default, null, null);
			}
			AreaKey area = new AreaKey(index, (AreaMode)mode);
			int num = 0;
			Session session = new Session(area, null, null);
			while (session.Strawberries.Count < strawberries)
			{
				num++;
				session.Strawberries.Add(new EntityID("null", num));
			}
			session.Deaths = deaths;
			session.Cassette = gem;
			session.Time = 100000L + (long)Calc.Random.Next();
			Engine.Scene = new LevelExit(LevelExit.Mode.Completed, session, null);
		}

		// Token: 0x0600186C RID: 6252 RVA: 0x00099788 File Offset: 0x00097988
		[Command("ow_complete", "test the completion sequence on the overworld after a level")]
		private static void CmdOWComplete(int index = 1, int mode = 0, int deaths = 0, int strawberries = -1, bool cassette = true, bool heartGem = true, float beatBestTimeBy = 1.7921f, float beatBestFullClearTimeBy = 1.7921f)
		{
			if (SaveData.Instance == null)
			{
				SaveData.InitializeDebugMode(true);
				SaveData.Instance.CurrentSession = new Session(AreaKey.Default, null, null);
			}
			AreaKey areaKey = new AreaKey(index, (AreaMode)mode);
			Session session = new Session(areaKey, null, null);
			AreaStats areaStats = SaveData.Instance.Areas[index];
			AreaModeStats areaModeStats = areaStats.Modes[mode];
			double totalSeconds = TimeSpan.FromTicks(areaModeStats.BestTime).TotalSeconds;
			double totalSeconds2 = TimeSpan.FromTicks(areaModeStats.BestFullClearTime).TotalSeconds;
			SaveData.Instance.RegisterCompletion(session);
			SaveData.Instance.CurrentSession = session;
			SaveData.Instance.CurrentSession.OldStats = new AreaStats(index);
			SaveData.Instance.LastArea = areaKey;
			if (mode == 0)
			{
				if (strawberries == -1)
				{
					areaStats.Modes[0].TotalStrawberries = AreaData.Areas[index].Mode[0].TotalStrawberries;
				}
				else
				{
					areaStats.Modes[0].TotalStrawberries = Math.Max(areaStats.TotalStrawberries, strawberries);
				}
				if (cassette)
				{
					areaStats.Cassette = true;
				}
			}
			areaModeStats.Deaths = Math.Max(deaths, areaModeStats.Deaths);
			if (heartGem)
			{
				areaModeStats.HeartGem = true;
			}
			if (totalSeconds <= 0.0)
			{
				areaModeStats.BestTime = TimeSpan.FromMinutes(5.0).Ticks;
			}
			else if (beatBestTimeBy > 0f)
			{
				areaModeStats.BestTime = TimeSpan.FromSeconds(totalSeconds - (double)beatBestTimeBy).Ticks;
			}
			if (beatBestFullClearTimeBy > 0f)
			{
				if (totalSeconds2 <= 0.0)
				{
					areaModeStats.BestFullClearTime = TimeSpan.FromMinutes(5.0).Ticks;
				}
				else
				{
					areaModeStats.BestFullClearTime = TimeSpan.FromSeconds(totalSeconds2 - (double)beatBestFullClearTimeBy).Ticks;
				}
			}
			Engine.Scene = new OverworldLoader(Overworld.StartMode.AreaComplete, null);
		}

		// Token: 0x0600186D RID: 6253 RVA: 0x00099960 File Offset: 0x00097B60
		[Command("mapedit", "edit a map")]
		private static void CmdMapEdit(int index = -1, int mode = 0)
		{
			AreaKey area;
			if (index == -1)
			{
				if (Engine.Scene is Level)
				{
					area = (Engine.Scene as Level).Session.Area;
				}
				else
				{
					area = AreaKey.Default;
				}
			}
			else
			{
				area = new AreaKey(index, (AreaMode)mode);
			}
			Engine.Scene = new MapEditor(area, true);
			Engine.Commands.Open = false;
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x000999BC File Offset: 0x00097BBC
		[Command("dflag", "Set a savedata flag")]
		private static void CmdDFlag(string flag, bool setTo = true)
		{
			if (setTo)
			{
				SaveData.Instance.SetFlag(flag);
				return;
			}
			SaveData.Instance.Flags.Remove(flag);
		}

		// Token: 0x0600186F RID: 6255 RVA: 0x000999E0 File Offset: 0x00097BE0
		[Command("meet", "Sets flags as though you met Theo")]
		private static void CmdMeet(bool met = true, bool knowsName = true)
		{
			if (met)
			{
				SaveData.Instance.SetFlag("MetTheo");
			}
			else
			{
				SaveData.Instance.Flags.Remove("MetTheo");
			}
			if (knowsName)
			{
				SaveData.Instance.SetFlag("TheoKnowsName");
				return;
			}
			SaveData.Instance.Flags.Remove("TheoKnowsName");
		}

		// Token: 0x06001870 RID: 6256 RVA: 0x00099A3E File Offset: 0x00097C3E
		[Command("flag", "set a session flag")]
		private static void CmdFlag(string flag, bool setTo = true)
		{
			SaveData.Instance.CurrentSession.SetFlag(flag, setTo);
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x00099A51 File Offset: 0x00097C51
		[Command("level_flag", "set a session load flag")]
		private static void CmdLevelFlag(string flag)
		{
			SaveData.Instance.CurrentSession.LevelFlags.Add(flag);
		}

		// Token: 0x06001872 RID: 6258 RVA: 0x00099A69 File Offset: 0x00097C69
		[Command("e", "edit a map")]
		private static void CmdE(int index = -1, int mode = 0)
		{
			Commands.CmdMapEdit(index, mode);
		}

		// Token: 0x06001873 RID: 6259 RVA: 0x00099A72 File Offset: 0x00097C72
		[Command("overworld", "go to the overworld")]
		private static void CmdOverworld()
		{
			if (SaveData.Instance == null)
			{
				SaveData.InitializeDebugMode(true);
				SaveData.Instance.CurrentSession = new Session(AreaKey.Default, null, null);
			}
			Engine.Scene = new OverworldLoader(Overworld.StartMode.Titlescreen, null);
		}

		// Token: 0x06001874 RID: 6260 RVA: 0x00099AA3 File Offset: 0x00097CA3
		[Command("music", "play a music track")]
		private static void CmdMusic(string song)
		{
			Audio.SetMusic(SFX.EventnameByHandle(song), true, true);
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x00099AB3 File Offset: 0x00097CB3
		[Command("sd_clearflags", "clears all flags from the save file")]
		private static void CmdClearSave()
		{
			SaveData.Instance.Flags.Clear();
		}

		// Token: 0x06001876 RID: 6262 RVA: 0x00099AC4 File Offset: 0x00097CC4
		[Command("music_vol", "set the music volume")]
		private static void CmdMusicVol(int num)
		{
			Settings.Instance.MusicVolume = num;
			Settings.Instance.ApplyVolumes();
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x00099ADB File Offset: 0x00097CDB
		[Command("sfx_vol", "set the sfx volume")]
		private static void CmdSFXVol(int num)
		{
			Settings.Instance.SFXVolume = num;
			Settings.Instance.ApplyVolumes();
		}

		// Token: 0x06001878 RID: 6264 RVA: 0x00099AF2 File Offset: 0x00097CF2
		[Command("p_dreamdash", "enable dream dashing")]
		private static void CmdDreamDash(bool set = true)
		{
			(Engine.Scene as Level).Session.Inventory.DreamDash = set;
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x00099B0E File Offset: 0x00097D0E
		[Command("p_twodashes", "enable two dashes")]
		private static void CmdTwoDashes(bool set = true)
		{
			(Engine.Scene as Level).Session.Inventory.Dashes = (set ? 2 : 1);
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x00099B30 File Offset: 0x00097D30
		[Command("berries", "check how many strawberries are in the given chapter, or the entire game")]
		private static void CmdStrawberries(int chapterID = -1)
		{
			Color lime = Color.Lime;
			Color red = Color.Red;
			Color yellow = Color.Yellow;
			Color gray = Color.Gray;
			if (chapterID == -1)
			{
				int num = 0;
				int[] array = new int[AreaData.Areas.Count];
				for (int i = 0; i < AreaData.Areas.Count; i++)
				{
					new MapData(new AreaKey(i, AreaMode.Normal)).GetStrawberries(out array[i]);
					num += array[i];
				}
				Engine.Commands.Log("Grand Total Strawberries: " + num, yellow);
				for (int j = 0; j < array.Length; j++)
				{
					Color color;
					if (array[j] != AreaData.Areas[j].Mode[0].TotalStrawberries)
					{
						color = red;
					}
					else if (array[j] == 0)
					{
						color = gray;
					}
					else
					{
						color = lime;
					}
					Engine.Commands.Log(string.Concat(new object[]
					{
						"Chapter ",
						j,
						": ",
						array[j]
					}), color);
				}
				return;
			}
			AreaData areaData = AreaData.Areas[chapterID];
			int totalStrawberries = areaData.Mode[0].TotalStrawberries;
			int[] array2 = new int[areaData.Mode[0].Checkpoints.Length + 1];
			array2[0] = areaData.Mode[0].StartStrawberries;
			for (int k = 1; k < array2.Length; k++)
			{
				array2[k] = areaData.Mode[0].Checkpoints[k - 1].Strawberries;
			}
			int num2;
			int[] strawberries = new MapData(new AreaKey(chapterID, AreaMode.Normal)).GetStrawberries(out num2);
			Engine.Commands.Log("Chapter " + chapterID + " Strawberries");
			Engine.Commands.Log("Total: " + num2, (totalStrawberries == num2) ? lime : red);
			for (int l = 0; l < array2.Length; l++)
			{
				Color color2;
				if (strawberries[l] != array2[l])
				{
					color2 = red;
				}
				else if (strawberries[l] == 0)
				{
					color2 = gray;
				}
				else
				{
					color2 = lime;
				}
				Engine.Commands.Log(string.Concat(new object[]
				{
					"CP",
					l,
					": ",
					strawberries[l]
				}), color2);
			}
		}

		// Token: 0x0600187B RID: 6267 RVA: 0x00099D92 File Offset: 0x00097F92
		[Command("say", "initiate a dialog message")]
		private static void CmdSay(string id)
		{
			Engine.Scene.Add(new Textbox(id, new Func<IEnumerator>[0]));
		}

		// Token: 0x0600187C RID: 6268 RVA: 0x00099DAC File Offset: 0x00097FAC
		[Command("level_count", "print out total level count!")]
		private static void CmdTotalLevels(int areaID = -1, int mode = 0)
		{
			if (areaID >= 0)
			{
				Engine.Commands.Log(Commands.GetLevelsInArea(new AreaKey(areaID, (AreaMode)mode)));
				return;
			}
			int num = 0;
			foreach (AreaData areaData in AreaData.Areas)
			{
				for (int i = 0; i < areaData.Mode.Length; i++)
				{
					num += Commands.GetLevelsInArea(new AreaKey(areaData.ID, (AreaMode)i));
				}
			}
			Engine.Commands.Log(num);
		}

		// Token: 0x0600187D RID: 6269 RVA: 0x00099E50 File Offset: 0x00098050
		[Command("input_gui", "override input gui")]
		private static void CmdInputGui(string prefix)
		{
			Input.OverrideInputPrefix = prefix;
		}

		// Token: 0x0600187E RID: 6270 RVA: 0x00099E58 File Offset: 0x00098058
		private static int GetLevelsInArea(AreaKey key)
		{
			ModeProperties modeProperties = AreaData.Get(key).Mode[(int)key.Mode];
			if (modeProperties != null)
			{
				return modeProperties.MapData.LevelCount;
			}
			return 0;
		}

		// Token: 0x0600187F RID: 6271 RVA: 0x00099E88 File Offset: 0x00098088
		[Command("assist", "toggle assist mode for current savefile")]
		private static void CmdAssist()
		{
			SaveData.Instance.AssistMode = !SaveData.Instance.AssistMode;
			SaveData.Instance.VariantMode = false;
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x00099EAC File Offset: 0x000980AC
		[Command("variants", "toggle varaint mode for current savefile")]
		private static void CmdVariants()
		{
			SaveData.Instance.VariantMode = !SaveData.Instance.VariantMode;
			SaveData.Instance.AssistMode = false;
		}

		// Token: 0x06001881 RID: 6273 RVA: 0x00099ED0 File Offset: 0x000980D0
		[Command("cheat", "toggle cheat mode for the current savefile")]
		private static void CmdCheat()
		{
			SaveData.Instance.CheatMode = !SaveData.Instance.CheatMode;
		}

		// Token: 0x06001882 RID: 6274 RVA: 0x00099EEC File Offset: 0x000980EC
		[Command("capture", "capture the last ~200 frames of player movement to a file")]
		private static void CmdCapture(string filename)
		{
			Player entity = Engine.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				PlaybackData.Export(entity.ChaserStates, filename + ".bin");
			}
		}

		// Token: 0x06001883 RID: 6275 RVA: 0x00099F22 File Offset: 0x00098122
		[Command("playback", "play back the file name")]
		private static void CmdPlayback(string filename)
		{
			filename += ".bin";
			if (File.Exists(filename))
			{
				Engine.Scene = new PreviewRecording(filename);
				return;
			}
			Engine.Commands.Log("FILE NOT FOUND");
		}

		// Token: 0x06001884 RID: 6276 RVA: 0x00099F54 File Offset: 0x00098154
		[Command("fonts", "check loaded fonts")]
		private static void CmdFonts()
		{
			Fonts.Log();
		}

		// Token: 0x06001885 RID: 6277 RVA: 0x00099F5C File Offset: 0x0009815C
		[Command("rename", "renames a level")]
		private static void CmdRename(string current, string newName)
		{
			MapEditor mapEditor = Engine.Scene as MapEditor;
			if (mapEditor == null)
			{
				Engine.Commands.Log("Must be in the Map Editor");
				return;
			}
			mapEditor.Rename(current, newName);
		}

		// Token: 0x06001886 RID: 6278 RVA: 0x00099F90 File Offset: 0x00098190
		[Command("blackhole_strength", "value 0 - 3")]
		private static void CmdBlackHoleStrength(int strength)
		{
			strength = Calc.Clamp(strength, 0, 3);
			Level level = Engine.Scene as Level;
			if (level != null)
			{
				BlackholeBG blackholeBG = level.Background.Get<BlackholeBG>();
				if (blackholeBG != null)
				{
					blackholeBG.NextStrength(level, (BlackholeBG.Strengths)strength);
				}
			}
		}
	}
}
