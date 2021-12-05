using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002A9 RID: 681
	public class AreaData
	{
		// Token: 0x0600150B RID: 5387 RVA: 0x00076BD4 File Offset: 0x00074DD4
		public static ModeProperties GetMode(AreaKey area)
		{
			return AreaData.GetMode(area.ID, area.Mode);
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x00076BE7 File Offset: 0x00074DE7
		public static ModeProperties GetMode(int id, AreaMode mode = AreaMode.Normal)
		{
			return AreaData.Areas[id].Mode[(int)mode];
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x00076BFC File Offset: 0x00074DFC
		public static void Load()
		{
			AreaData.Areas = new List<AreaData>();
			List<AreaData> areas = AreaData.Areas;
			AreaData areaData = new AreaData();
			areaData.Name = "area_0";
			areaData.Icon = "areas/intro";
			areaData.Interlude = true;
			areaData.CompleteScreenName = null;
			AreaData areaData2 = areaData;
			ModeProperties[] array = new ModeProperties[3];
			array[0] = new ModeProperties
			{
				PoemID = null,
				Path = "0-Intro",
				Checkpoints = null,
				Inventory = PlayerInventory.Prologue,
				AudioState = new AudioState("event:/music/lvl0/intro", "event:/env/amb/00_prologue")
			};
			areaData2.Mode = array;
			areaData.TitleBaseColor = Calc.HexToColor("383838");
			areaData.TitleAccentColor = Calc.HexToColor("50AFAE");
			areaData.TitleTextColor = Color.White;
			areaData.IntroType = Player.IntroTypes.WalkInRight;
			areaData.Dreaming = false;
			areaData.ColorGrade = null;
			areaData.Wipe = delegate(Scene scene, bool wipeIn, Action onComplete)
			{
				new CurtainWipe(scene, wipeIn, onComplete);
			};
			areaData.DarknessAlpha = 0.05f;
			areaData.BloomBase = 0f;
			areaData.BloomStrength = 1f;
			areaData.OnLevelBegin = null;
			areaData.Jumpthru = "wood";
			areas.Add(areaData);
			List<AreaData> areas2 = AreaData.Areas;
			areaData = new AreaData();
			areaData.Name = "area_1";
			areaData.Icon = "areas/city";
			areaData.Interlude = false;
			areaData.CanFullClear = true;
			areaData.CompleteScreenName = "ForsakenCity";
			areaData.CassetteCheckpointIndex = 2;
			areaData.Mode = new ModeProperties[]
			{
				new ModeProperties
				{
					PoemID = "fc",
					Path = "1-ForsakenCity",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("6", "checkpoint_1_0", null, false, null),
						new CheckpointData("9b", "checkpoint_1_1", null, false, null)
					},
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/lvl1/main", "event:/env/amb/01_main")
				},
				new ModeProperties
				{
					PoemID = "fcr",
					Path = "1H-ForsakenCity",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("04", "checkpoint_1h_0", null, false, null),
						new CheckpointData("08", "checkpoint_1h_1", null, false, null)
					},
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/remix/01_forsaken_city", "event:/env/amb/01_main")
				},
				new ModeProperties
				{
					Path = "1X-ForsakenCity",
					Checkpoints = null,
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/remix/01_forsaken_city", "event:/env/amb/01_main")
				}
			};
			areaData.TitleBaseColor = Calc.HexToColor("6c7c81");
			areaData.TitleAccentColor = Calc.HexToColor("2f344b");
			areaData.TitleTextColor = Color.White;
			areaData.IntroType = Player.IntroTypes.Jump;
			areaData.Dreaming = false;
			areaData.ColorGrade = null;
			areaData.Wipe = delegate(Scene scene, bool wipeIn, Action onComplete)
			{
				new AngledWipe(scene, wipeIn, onComplete);
			};
			areaData.DarknessAlpha = 0.05f;
			areaData.BloomBase = 0f;
			areaData.BloomStrength = 1f;
			areaData.OnLevelBegin = null;
			areaData.Jumpthru = "wood";
			areaData.CassseteNoteColor = Calc.HexToColor("33a9ee");
			areaData.CassetteSong = "event:/music/cassette/01_forsaken_city";
			areas2.Add(areaData);
			List<AreaData> areas3 = AreaData.Areas;
			areaData = new AreaData();
			areaData.Name = "area_2";
			areaData.Icon = "areas/oldsite";
			areaData.Interlude = false;
			areaData.CanFullClear = true;
			areaData.CompleteScreenName = "OldSite";
			areaData.CassetteCheckpointIndex = 0;
			areaData.Mode = new ModeProperties[]
			{
				new ModeProperties
				{
					PoemID = "os",
					Path = "2-OldSite",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("3", "checkpoint_2_0", new PlayerInventory?(PlayerInventory.Default), true, null),
						new CheckpointData("end_3", "checkpoint_2_1", null, false, null)
					},
					Inventory = PlayerInventory.OldSite,
					AudioState = new AudioState("event:/music/lvl2/beginning", "event:/env/amb/02_dream")
				},
				new ModeProperties
				{
					PoemID = "osr",
					Path = "2H-OldSite",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("03", "checkpoint_2h_0", null, true, null),
						new CheckpointData("08b", "checkpoint_2h_1", null, true, null)
					},
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/remix/02_old_site", "event:/env/amb/02_dream")
				},
				new ModeProperties
				{
					Path = "2X-OldSite",
					Checkpoints = null,
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/remix/02_old_site", "event:/env/amb/02_dream")
				}
			};
			areaData.TitleBaseColor = Calc.HexToColor("247F35");
			areaData.TitleAccentColor = Calc.HexToColor("E4EF69");
			areaData.TitleTextColor = Color.White;
			areaData.IntroType = Player.IntroTypes.WakeUp;
			areaData.Dreaming = true;
			areaData.ColorGrade = "oldsite";
			areaData.Wipe = delegate(Scene scene, bool wipeIn, Action onComplete)
			{
				new DreamWipe(scene, wipeIn, onComplete);
			};
			areaData.DarknessAlpha = 0.15f;
			areaData.BloomBase = 0.5f;
			areaData.BloomStrength = 1f;
			areaData.OnLevelBegin = delegate(Level level)
			{
				if (level.Session.Area.Mode == AreaMode.Normal)
				{
					level.Add(new OldSiteChaseMusicHandler());
				}
			};
			areaData.Jumpthru = "wood";
			areaData.CassseteNoteColor = Calc.HexToColor("33eea2");
			areaData.CassetteSong = "event:/music/cassette/02_old_site";
			areas3.Add(areaData);
			List<AreaData> areas4 = AreaData.Areas;
			areaData = new AreaData();
			areaData.Name = "area_3";
			areaData.Icon = "areas/resort";
			areaData.Interlude = false;
			areaData.CanFullClear = true;
			areaData.CompleteScreenName = "CelestialResort";
			areaData.CassetteCheckpointIndex = 2;
			areaData.Mode = new ModeProperties[]
			{
				new ModeProperties
				{
					PoemID = "cr",
					Path = "3-CelestialResort",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("08-a", "checkpoint_3_0", null, false, null)
						{
							AudioState = new AudioState(new AudioTrackState("event:/music/lvl3/explore").SetProgress(3), new AudioTrackState("event:/env/amb/03_interior"))
						},
						new CheckpointData("09-d", "checkpoint_3_1", null, false, null)
						{
							AudioState = new AudioState(new AudioTrackState("event:/music/lvl3/clean").SetProgress(3), new AudioTrackState("event:/env/amb/03_interior"))
						},
						new CheckpointData("00-d", "checkpoint_3_2", null, false, null)
					},
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/lvl3/intro", "event:/env/amb/03_exterior")
				},
				new ModeProperties
				{
					PoemID = "crr",
					Path = "3H-CelestialResort",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("06", "checkpoint_3h_0", null, false, null),
						new CheckpointData("11", "checkpoint_3h_1", null, false, null),
						new CheckpointData("16", "checkpoint_3h_2", null, false, null)
					},
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/remix/03_resort", "event:/env/amb/03_exterior")
				},
				new ModeProperties
				{
					Path = "3X-CelestialResort",
					Checkpoints = null,
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/remix/03_resort", "event:/env/amb/03_exterior")
				}
			};
			areaData.TitleBaseColor = Calc.HexToColor("b93c27");
			areaData.TitleAccentColor = Calc.HexToColor("ffdd42");
			areaData.TitleTextColor = Color.White;
			areaData.IntroType = Player.IntroTypes.WalkInRight;
			areaData.Dreaming = false;
			areaData.ColorGrade = null;
			areaData.Wipe = delegate(Scene scene, bool wipeIn, Action onComplete)
			{
				new KeyDoorWipe(scene, wipeIn, onComplete);
			};
			areaData.DarknessAlpha = 0.15f;
			areaData.BloomBase = 0f;
			areaData.BloomStrength = 1f;
			areaData.OnLevelBegin = null;
			areaData.Jumpthru = "wood";
			areaData.CassseteNoteColor = Calc.HexToColor("eed933");
			areaData.CassetteSong = "event:/music/cassette/03_resort";
			areas4.Add(areaData);
			List<AreaData> areas5 = AreaData.Areas;
			areaData = new AreaData();
			areaData.Name = "area_4";
			areaData.Icon = "areas/cliffside";
			areaData.Interlude = false;
			areaData.CanFullClear = true;
			areaData.CompleteScreenName = "Cliffside";
			areaData.CassetteCheckpointIndex = 0;
			areaData.TitleBaseColor = Calc.HexToColor("FF7F83");
			areaData.TitleAccentColor = Calc.HexToColor("6D54B7");
			areaData.TitleTextColor = Color.White;
			areaData.Mode = new ModeProperties[]
			{
				new ModeProperties
				{
					PoemID = "cs",
					Path = "4-GoldenRidge",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("b-00", "checkpoint_4_0", null, false, null),
						new CheckpointData("c-00", "checkpoint_4_1", null, false, null),
						new CheckpointData("d-00", "checkpoint_4_2", null, false, null)
					},
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/lvl4/main", "event:/env/amb/04_main")
				},
				new ModeProperties
				{
					PoemID = "csr",
					Path = "4H-GoldenRidge",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("b-00", "checkpoint_4h_0", null, false, null),
						new CheckpointData("c-00", "checkpoint_4h_1", null, false, null),
						new CheckpointData("d-00", "checkpoint_4h_2", null, false, null)
					},
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/remix/04_cliffside", "event:/env/amb/04_main")
				},
				new ModeProperties
				{
					Path = "4X-GoldenRidge",
					Checkpoints = null,
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/remix/04_cliffside", "event:/env/amb/04_main")
				}
			};
			areaData.IntroType = Player.IntroTypes.WalkInRight;
			areaData.Dreaming = false;
			areaData.ColorGrade = null;
			areaData.Wipe = delegate(Scene scene, bool wipeIn, Action onComplete)
			{
				new WindWipe(scene, wipeIn, onComplete);
			};
			areaData.DarknessAlpha = 0.1f;
			areaData.BloomBase = 0.25f;
			areaData.BloomStrength = 1f;
			areaData.OnLevelBegin = null;
			areaData.Jumpthru = "cliffside";
			areaData.Spike = "cliffside";
			areaData.CrumbleBlock = "cliffside";
			areaData.WoodPlatform = "cliffside";
			areaData.CassseteNoteColor = Calc.HexToColor("eb4bd9");
			areaData.CassetteSong = "event:/music/cassette/04_cliffside";
			areas5.Add(areaData);
			List<AreaData> areas6 = AreaData.Areas;
			areaData = new AreaData();
			areaData.Name = "area_5";
			areaData.Icon = "areas/temple";
			areaData.Interlude = false;
			areaData.CanFullClear = true;
			areaData.CompleteScreenName = "MirrorTemple";
			areaData.CassetteCheckpointIndex = 1;
			areaData.Mode = new ModeProperties[]
			{
				new ModeProperties
				{
					PoemID = "t",
					Path = "5-MirrorTemple",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("b-00", "checkpoint_5_0", null, false, null),
						new CheckpointData("c-00", "checkpoint_5_1", null, true, new AudioState("event:/music/lvl5/middle_temple", "event:/env/amb/05_interior_dark")),
						new CheckpointData("d-00", "checkpoint_5_2", null, true, new AudioState("event:/music/lvl5/middle_temple", "event:/env/amb/05_interior_dark")),
						new CheckpointData("e-00", "checkpoint_5_3", null, true, new AudioState("event:/music/lvl5/mirror", "event:/env/amb/05_interior_dark"))
					},
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/lvl5/normal", "event:/env/amb/05_interior_main")
				},
				new ModeProperties
				{
					PoemID = "tr",
					Path = "5H-MirrorTemple",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("b-00", "checkpoint_5h_0", null, false, null),
						new CheckpointData("c-00", "checkpoint_5h_1", null, false, null),
						new CheckpointData("d-00", "checkpoint_5h_2", null, false, null)
					},
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/remix/05_mirror_temple", "event:/env/amb/05_interior_main")
				},
				new ModeProperties
				{
					Path = "5X-MirrorTemple",
					Checkpoints = null,
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/remix/05_mirror_temple", "event:/env/amb/05_interior_main")
				}
			};
			areaData.TitleBaseColor = Calc.HexToColor("8314bc");
			areaData.TitleAccentColor = Calc.HexToColor("df72f9");
			areaData.TitleTextColor = Color.White;
			areaData.IntroType = Player.IntroTypes.WakeUp;
			areaData.Dreaming = false;
			areaData.ColorGrade = null;
			areaData.Wipe = delegate(Scene scene, bool wipeIn, Action onComplete)
			{
				new DropWipe(scene, wipeIn, onComplete);
			};
			areaData.DarknessAlpha = 0.15f;
			areaData.BloomBase = 0f;
			areaData.BloomStrength = 1f;
			areaData.OnLevelBegin = delegate(Level level)
			{
				level.Add(new SeekerEffectsController());
				if (level.Session.Area.Mode == AreaMode.Normal)
				{
					level.Add(new TempleEndingMusicHandler());
				}
			};
			areaData.Jumpthru = "temple";
			areaData.CassseteNoteColor = Calc.HexToColor("5a56e6");
			areaData.CobwebColor = new Color[]
			{
				Calc.HexToColor("9f2166")
			};
			areaData.CassetteSong = "event:/music/cassette/05_mirror_temple";
			areas6.Add(areaData);
			List<AreaData> areas7 = AreaData.Areas;
			areaData = new AreaData();
			areaData.Name = "area_6";
			areaData.Icon = "areas/reflection";
			areaData.Interlude = false;
			areaData.CanFullClear = true;
			areaData.CompleteScreenName = "Fall";
			areaData.CassetteCheckpointIndex = 2;
			areaData.Mode = new ModeProperties[]
			{
				new ModeProperties
				{
					PoemID = "tf",
					Path = "6-Reflection",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("00", "checkpoint_6_0", null, false, null),
						new CheckpointData("04", "checkpoint_6_1", null, false, null),
						new CheckpointData("b-00", "checkpoint_6_2", null, false, null),
						new CheckpointData("boss-00", "checkpoint_6_3", null, false, null),
						new CheckpointData("after-00", "checkpoint_6_4", new PlayerInventory?(PlayerInventory.CH6End), false, null)
						{
							Flags = new HashSet<string>
							{
								"badeline_connection"
							},
							AudioState = new AudioState(new AudioTrackState("event:/music/lvl6/badeline_acoustic").Param("levelup", 2f), new AudioTrackState("event:/env/amb/06_main"))
						}
					},
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/lvl6/main", "event:/env/amb/06_main")
				},
				new ModeProperties
				{
					PoemID = "tfr",
					Path = "6H-Reflection",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("b-00", "checkpoint_6h_0", null, false, null),
						new CheckpointData("c-00", "checkpoint_6h_1", null, false, null),
						new CheckpointData("d-00", "checkpoint_6h_2", null, false, null)
					},
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/remix/06_reflection", "event:/env/amb/06_main")
				},
				new ModeProperties
				{
					Path = "6X-Reflection",
					Checkpoints = null,
					Inventory = PlayerInventory.Default,
					AudioState = new AudioState("event:/music/remix/06_reflection", "event:/env/amb/06_main")
				}
			};
			areaData.TitleBaseColor = Calc.HexToColor("359FE0");
			areaData.TitleAccentColor = Calc.HexToColor("3C5CBC");
			areaData.TitleTextColor = Color.White;
			areaData.IntroType = Player.IntroTypes.None;
			areaData.Dreaming = false;
			areaData.ColorGrade = "reflection";
			areaData.Wipe = delegate(Scene scene, bool wipeIn, Action onComplete)
			{
				new FallWipe(scene, wipeIn, onComplete);
			};
			areaData.DarknessAlpha = 0.2f;
			areaData.BloomBase = 0.2f;
			areaData.BloomStrength = 1f;
			areaData.OnLevelBegin = null;
			areaData.Jumpthru = "reflection";
			areaData.Spike = "reflection";
			areaData.CassseteNoteColor = Calc.HexToColor("56e6dd");
			areaData.CassetteSong = "event:/music/cassette/06_reflection";
			areas7.Add(areaData);
			List<AreaData> areas8 = AreaData.Areas;
			areaData = new AreaData();
			areaData.Name = "area_7";
			areaData.Icon = "areas/summit";
			areaData.Interlude = false;
			areaData.CanFullClear = true;
			areaData.CompleteScreenName = "Summit";
			areaData.CassetteCheckpointIndex = 3;
			areaData.Mode = new ModeProperties[]
			{
				new ModeProperties
				{
					PoemID = "ts",
					Path = "7-Summit",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("b-00", "checkpoint_7_0", null, false, new AudioState(new AudioTrackState("event:/music/lvl7/main").SetProgress(1), null)),
						new CheckpointData("c-00", "checkpoint_7_1", null, false, new AudioState(new AudioTrackState("event:/music/lvl7/main").SetProgress(2), null)),
						new CheckpointData("d-00", "checkpoint_7_2", null, false, new AudioState(new AudioTrackState("event:/music/lvl7/main").SetProgress(3), null)),
						new CheckpointData("e-00b", "checkpoint_7_3", null, false, new AudioState(new AudioTrackState("event:/music/lvl7/main").SetProgress(4), null)),
						new CheckpointData("f-00", "checkpoint_7_4", null, false, new AudioState(new AudioTrackState("event:/music/lvl7/main").SetProgress(5), null)),
						new CheckpointData("g-00", "checkpoint_7_5", null, false, new AudioState("event:/music/lvl7/final_ascent", null))
					},
					Inventory = PlayerInventory.TheSummit,
					AudioState = new AudioState("event:/music/lvl7/main", null)
				},
				new ModeProperties
				{
					PoemID = "tsr",
					Path = "7H-Summit",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("b-00", "checkpoint_7H_0", null, false, null),
						new CheckpointData("c-01", "checkpoint_7H_1", null, false, null),
						new CheckpointData("d-00", "checkpoint_7H_2", null, false, null),
						new CheckpointData("e-00", "checkpoint_7H_3", null, false, null),
						new CheckpointData("f-00", "checkpoint_7H_4", null, false, null),
						new CheckpointData("g-00", "checkpoint_7H_5", null, false, null)
					},
					Inventory = PlayerInventory.TheSummit,
					AudioState = new AudioState("event:/music/remix/07_summit", null)
				},
				new ModeProperties
				{
					Path = "7X-Summit",
					Checkpoints = null,
					Inventory = PlayerInventory.TheSummit,
					AudioState = new AudioState("event:/music/remix/07_summit", null)
				}
			};
			areaData.TitleBaseColor = Calc.HexToColor("FFD819");
			areaData.TitleAccentColor = Calc.HexToColor("197DB7");
			areaData.TitleTextColor = Color.Black;
			areaData.IntroType = Player.IntroTypes.None;
			areaData.Dreaming = false;
			areaData.ColorGrade = null;
			areaData.Wipe = delegate(Scene scene, bool wipeIn, Action onComplete)
			{
				new MountainWipe(scene, wipeIn, onComplete);
			};
			areaData.DarknessAlpha = 0.05f;
			areaData.BloomBase = 0.2f;
			areaData.BloomStrength = 1f;
			areaData.OnLevelBegin = null;
			areaData.Jumpthru = "temple";
			areaData.Spike = "outline";
			areaData.CassseteNoteColor = Calc.HexToColor("e69156");
			areaData.CassetteSong = "event:/music/cassette/07_summit";
			areas8.Add(areaData);
			List<AreaData> areas9 = AreaData.Areas;
			areaData = new AreaData();
			areaData.Name = "area_8";
			areaData.Icon = "areas/intro";
			areaData.Interlude = true;
			areaData.CompleteScreenName = null;
			areaData.CassetteCheckpointIndex = 1;
			AreaData areaData3 = areaData;
			ModeProperties[] array2 = new ModeProperties[3];
			array2[0] = new ModeProperties
			{
				PoemID = null,
				Path = "8-Epilogue",
				Checkpoints = null,
				Inventory = PlayerInventory.TheSummit,
				AudioState = new AudioState("event:/music/lvl8/main", "event:/env/amb/00_prologue")
			};
			areaData3.Mode = array2;
			areaData.TitleBaseColor = Calc.HexToColor("383838");
			areaData.TitleAccentColor = Calc.HexToColor("50AFAE");
			areaData.TitleTextColor = Color.White;
			areaData.IntroType = Player.IntroTypes.WalkInLeft;
			areaData.Dreaming = false;
			areaData.ColorGrade = null;
			areaData.Wipe = delegate(Scene scene, bool wipeIn, Action onComplete)
			{
				new CurtainWipe(scene, wipeIn, onComplete);
			};
			areaData.DarknessAlpha = 0.05f;
			areaData.BloomBase = 0f;
			areaData.BloomStrength = 1f;
			areaData.OnLevelBegin = null;
			areaData.Jumpthru = "wood";
			areas9.Add(areaData);
			List<AreaData> areas10 = AreaData.Areas;
			areaData = new AreaData();
			areaData.Name = "area_9";
			areaData.Icon = "areas/core";
			areaData.Interlude = false;
			areaData.CanFullClear = true;
			areaData.CompleteScreenName = "Core";
			areaData.CassetteCheckpointIndex = 3;
			areaData.Mode = new ModeProperties[]
			{
				new ModeProperties
				{
					PoemID = "mc",
					Path = "9-Core",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("a-00", "checkpoint_8_0", null, false, null),
						new CheckpointData("c-00", "checkpoint_8_1", null, false, null)
						{
							CoreMode = new Session.CoreModes?(Session.CoreModes.Cold)
						},
						new CheckpointData("d-00", "checkpoint_8_2", null, false, null)
					},
					Inventory = PlayerInventory.Core,
					AudioState = new AudioState("event:/music/lvl9/main", "event:/env/amb/09_main"),
					IgnoreLevelAudioLayerData = true
				},
				new ModeProperties
				{
					PoemID = "mcr",
					Path = "9H-Core",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("a-00", "checkpoint_8H_0", null, false, null),
						new CheckpointData("b-00", "checkpoint_8H_1", null, false, null),
						new CheckpointData("c-01", "checkpoint_8H_2", null, false, null)
					},
					Inventory = PlayerInventory.Core,
					AudioState = new AudioState("event:/music/remix/09_core", "event:/env/amb/09_main")
				},
				new ModeProperties
				{
					Path = "9X-Core",
					Checkpoints = null,
					Inventory = PlayerInventory.Core,
					AudioState = new AudioState("event:/music/remix/09_core", "event:/env/amb/09_main")
				}
			};
			areaData.TitleBaseColor = Calc.HexToColor("761008");
			areaData.TitleAccentColor = Calc.HexToColor("E0201D");
			areaData.TitleTextColor = Color.White;
			areaData.IntroType = Player.IntroTypes.WalkInRight;
			areaData.Dreaming = false;
			areaData.ColorGrade = null;
			areaData.Wipe = delegate(Scene scene, bool wipeIn, Action onComplete)
			{
				new HeartWipe(scene, wipeIn, onComplete);
			};
			areaData.DarknessAlpha = 0.05f;
			areaData.BloomBase = 0f;
			areaData.BloomStrength = 1f;
			areaData.OnLevelBegin = null;
			areaData.Jumpthru = "core";
			areaData.CassseteNoteColor = Calc.HexToColor("e6566a");
			areaData.CassetteSong = "event:/music/cassette/09_core";
			areaData.CoreMode = Session.CoreModes.Hot;
			areas10.Add(areaData);
			List<AreaData> areas11 = AreaData.Areas;
			areaData = new AreaData();
			areaData.Name = "area_10";
			areaData.Icon = "areas/farewell";
			areaData.Interlude = false;
			areaData.CanFullClear = false;
			areaData.IsFinal = true;
			areaData.CompleteScreenName = "Core";
			areaData.CassetteCheckpointIndex = -1;
			areaData.Mode = new ModeProperties[]
			{
				new ModeProperties
				{
					PoemID = "fw",
					Path = "LostLevels",
					Checkpoints = new CheckpointData[]
					{
						new CheckpointData("a-00", "checkpoint_9_0", null, false, new AudioState(new AudioTrackState("event:/new_content/music/lvl10/part01").SetProgress(1), null)),
						new CheckpointData("c-00", "checkpoint_9_1", null, false, new AudioState(new AudioTrackState("event:/new_content/music/lvl10/part01").SetProgress(1), null)),
						new CheckpointData("e-00z", "checkpoint_9_2", null, false, new AudioState(new AudioTrackState("event:/new_content/music/lvl10/part02"), null)),
						new CheckpointData("f-door", "checkpoint_9_3", null, false, new AudioState(new AudioTrackState("event:/new_content/music/lvl10/intermission_heartgroove"), null)),
						new CheckpointData("h-00b", "checkpoint_9_4", null, false, new AudioState(new AudioTrackState("event:/new_content/music/lvl10/part03"), null)),
						new CheckpointData("i-00", "checkpoint_9_5", null, false, new AudioState(new AudioTrackState("event:/new_content/music/lvl10/cassette_rooms").Param("sixteenth_note", 7f), null))
						{
							ColorGrade = "feelingdown"
						},
						new CheckpointData("j-00", "checkpoint_9_6", null, false, new AudioState(new AudioTrackState("event:/new_content/music/lvl10/cassette_rooms").Param("sixteenth_note", 7f).SetProgress(3), null))
						{
							ColorGrade = "feelingdown"
						},
						new CheckpointData("j-16", "checkpoint_9_7", null, false, new AudioState(new AudioTrackState("event:/new_content/music/lvl10/final_run").SetProgress(3), null))
					},
					Inventory = PlayerInventory.Farewell,
					AudioState = new AudioState(new AudioTrackState("event:/new_content/music/lvl10/part01").SetProgress(1), new AudioTrackState("event:/env/amb/00_prologue"))
				}
			};
			areaData.TitleBaseColor = Calc.HexToColor("240d7c");
			areaData.TitleAccentColor = Calc.HexToColor("FF6AA9");
			areaData.TitleTextColor = Color.White;
			areaData.IntroType = Player.IntroTypes.ThinkForABit;
			areaData.Dreaming = false;
			areaData.ColorGrade = null;
			areaData.Wipe = delegate(Scene scene, bool wipeIn, Action onComplete)
			{
				new StarfieldWipe(scene, wipeIn, onComplete);
			};
			areaData.DarknessAlpha = 0.05f;
			areaData.BloomBase = 0.5f;
			areaData.BloomStrength = 1f;
			areaData.OnLevelBegin = null;
			areaData.Jumpthru = "wood";
			areaData.CassseteNoteColor = Calc.HexToColor("e6566a");
			areaData.CassetteSong = null;
			areaData.CobwebColor = new Color[]
			{
				Calc.HexToColor("42c192"),
				Calc.HexToColor("af36a8"),
				Calc.HexToColor("3474a6")
			};
			areas11.Add(areaData);
			int num = Enum.GetNames(typeof(AreaMode)).Length;
			for (int i = 0; i < AreaData.Areas.Count; i++)
			{
				AreaData.Areas[i].ID = i;
				AreaData.Areas[i].Mode[0].MapData = new MapData(new AreaKey(i, AreaMode.Normal));
				if (!AreaData.Areas[i].Interlude)
				{
					for (int j = 1; j < num; j++)
					{
						if (AreaData.Areas[i].HasMode((AreaMode)j))
						{
							AreaData.Areas[i].Mode[j].MapData = new MapData(new AreaKey(i, (AreaMode)j));
						}
					}
				}
			}
			AreaData.ReloadMountainViews();
		}

		// Token: 0x0600150E RID: 5390 RVA: 0x00078904 File Offset: 0x00076B04
		public static void ReloadMountainViews()
		{
			foreach (object obj in Calc.LoadXML(Path.Combine(Engine.ContentDirectory, "Overworld", "AreaViews.xml"))["Views"])
			{
				XmlElement xmlElement = (XmlElement)obj;
				int num = xmlElement.AttrInt("id");
				if (num >= 0 && num < AreaData.Areas.Count)
				{
					Vector3 pos = xmlElement["Idle"].AttrVector3("position");
					Vector3 target = xmlElement["Idle"].AttrVector3("target");
					AreaData.Areas[num].MountainIdle = new MountainCamera(pos, target);
					pos = xmlElement["Select"].AttrVector3("position");
					target = xmlElement["Select"].AttrVector3("target");
					AreaData.Areas[num].MountainSelect = new MountainCamera(pos, target);
					pos = xmlElement["Zoom"].AttrVector3("position");
					target = xmlElement["Zoom"].AttrVector3("target");
					AreaData.Areas[num].MountainZoom = new MountainCamera(pos, target);
					if (xmlElement["Cursor"] != null)
					{
						AreaData.Areas[num].MountainCursor = xmlElement["Cursor"].AttrVector3("position");
					}
					AreaData.Areas[num].MountainState = xmlElement.AttrInt("state", 0);
				}
			}
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x00078AC8 File Offset: 0x00076CC8
		public static bool IsPoemRemix(string id)
		{
			foreach (AreaData areaData in AreaData.Areas)
			{
				if (areaData.Mode.Length > 1 && areaData.Mode[1] != null && !string.IsNullOrEmpty(areaData.Mode[1].PoemID) && areaData.Mode[1].PoemID.Equals(id, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001510 RID: 5392 RVA: 0x00078B5C File Offset: 0x00076D5C
		public static int GetCheckpointID(AreaKey area, string level)
		{
			CheckpointData[] checkpoints = AreaData.Areas[area.ID].Mode[(int)area.Mode].Checkpoints;
			if (checkpoints != null)
			{
				for (int i = 0; i < checkpoints.Length; i++)
				{
					if (checkpoints[i].Level.Equals(level))
					{
						return i;
					}
				}
			}
			return -1;
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x00078BB0 File Offset: 0x00076DB0
		public static CheckpointData GetCheckpoint(AreaKey area, string level)
		{
			CheckpointData[] checkpoints = AreaData.Areas[area.ID].Mode[(int)area.Mode].Checkpoints;
			if (level != null && checkpoints != null)
			{
				foreach (CheckpointData checkpointData in checkpoints)
				{
					if (checkpointData.Level.Equals(level))
					{
						return checkpointData;
					}
				}
			}
			return null;
		}

		// Token: 0x06001512 RID: 5394 RVA: 0x00078C0C File Offset: 0x00076E0C
		public static string GetCheckpointName(AreaKey area, string level)
		{
			if (string.IsNullOrEmpty(level))
			{
				return "START";
			}
			CheckpointData checkpoint = AreaData.GetCheckpoint(area, level);
			if (checkpoint != null)
			{
				return Dialog.Clean(checkpoint.Name, null);
			}
			return null;
		}

		// Token: 0x06001513 RID: 5395 RVA: 0x00078C40 File Offset: 0x00076E40
		public static PlayerInventory GetCheckpointInventory(AreaKey area, string level)
		{
			CheckpointData checkpoint = AreaData.GetCheckpoint(area, level);
			if (checkpoint != null && checkpoint.Inventory != null)
			{
				return checkpoint.Inventory.Value;
			}
			return AreaData.Areas[area.ID].Mode[(int)area.Mode].Inventory;
		}

		// Token: 0x06001514 RID: 5396 RVA: 0x00078C94 File Offset: 0x00076E94
		public static bool GetCheckpointDreaming(AreaKey area, string level)
		{
			CheckpointData checkpoint = AreaData.GetCheckpoint(area, level);
			if (checkpoint != null)
			{
				return checkpoint.Dreaming;
			}
			return AreaData.Areas[area.ID].Dreaming;
		}

		// Token: 0x06001515 RID: 5397 RVA: 0x00078CC8 File Offset: 0x00076EC8
		public static Session.CoreModes GetCheckpointCoreMode(AreaKey area, string level)
		{
			CheckpointData checkpoint = AreaData.GetCheckpoint(area, level);
			if (checkpoint != null && checkpoint.CoreMode != null)
			{
				return checkpoint.CoreMode.Value;
			}
			return AreaData.Areas[area.ID].CoreMode;
		}

		// Token: 0x06001516 RID: 5398 RVA: 0x00078D10 File Offset: 0x00076F10
		public static AudioState GetCheckpointAudioState(AreaKey area, string level)
		{
			CheckpointData checkpoint = AreaData.GetCheckpoint(area, level);
			if (checkpoint != null)
			{
				return checkpoint.AudioState;
			}
			return null;
		}

		// Token: 0x06001517 RID: 5399 RVA: 0x00078D30 File Offset: 0x00076F30
		public static string GetCheckpointColorGrading(AreaKey area, string level)
		{
			CheckpointData checkpoint = AreaData.GetCheckpoint(area, level);
			if (checkpoint != null)
			{
				return checkpoint.ColorGrade;
			}
			return null;
		}

		// Token: 0x06001518 RID: 5400 RVA: 0x00078D50 File Offset: 0x00076F50
		public static void Unload()
		{
			AreaData.Areas = null;
		}

		// Token: 0x06001519 RID: 5401 RVA: 0x00078D58 File Offset: 0x00076F58
		public static AreaData Get(Scene scene)
		{
			if (scene != null && scene is Level)
			{
				return AreaData.Areas[(scene as Level).Session.Area.ID];
			}
			return null;
		}

		// Token: 0x0600151A RID: 5402 RVA: 0x00078D86 File Offset: 0x00076F86
		public static AreaData Get(Session session)
		{
			if (session != null)
			{
				return AreaData.Areas[session.Area.ID];
			}
			return null;
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x00078DA2 File Offset: 0x00076FA2
		public static AreaData Get(AreaKey area)
		{
			return AreaData.Areas[area.ID];
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x00078DB4 File Offset: 0x00076FB4
		public static AreaData Get(int id)
		{
			return AreaData.Areas[id];
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x0600151D RID: 5405 RVA: 0x00078DC1 File Offset: 0x00076FC1
		public XmlElement CompleteScreenXml
		{
			get
			{
				return GFX.CompleteScreensXml["Screens"][this.CompleteScreenName];
			}
		}

		// Token: 0x0600151E RID: 5406 RVA: 0x00078DDD File Offset: 0x00076FDD
		public void DoScreenWipe(Scene scene, bool wipeIn, Action onComplete = null)
		{
			if (this.Wipe == null)
			{
				new WindWipe(scene, wipeIn, onComplete);
				return;
			}
			this.Wipe(scene, wipeIn, onComplete);
		}

		// Token: 0x0600151F RID: 5407 RVA: 0x00078DFF File Offset: 0x00076FFF
		public bool HasMode(AreaMode mode)
		{
			return (AreaMode)this.Mode.Length > mode && this.Mode[(int)mode] != null && this.Mode[(int)mode].Path != null;
		}

		// Token: 0x040010F1 RID: 4337
		public static List<AreaData> Areas;

		// Token: 0x040010F2 RID: 4338
		public string Name;

		// Token: 0x040010F3 RID: 4339
		public string Icon;

		// Token: 0x040010F4 RID: 4340
		public int ID;

		// Token: 0x040010F5 RID: 4341
		public bool Interlude;

		// Token: 0x040010F6 RID: 4342
		public bool CanFullClear;

		// Token: 0x040010F7 RID: 4343
		public bool IsFinal;

		// Token: 0x040010F8 RID: 4344
		public string CompleteScreenName;

		// Token: 0x040010F9 RID: 4345
		public ModeProperties[] Mode;

		// Token: 0x040010FA RID: 4346
		public int CassetteCheckpointIndex = -1;

		// Token: 0x040010FB RID: 4347
		public Color TitleBaseColor = Color.White;

		// Token: 0x040010FC RID: 4348
		public Color TitleAccentColor = Color.Gray;

		// Token: 0x040010FD RID: 4349
		public Color TitleTextColor = Color.White;

		// Token: 0x040010FE RID: 4350
		public Player.IntroTypes IntroType;

		// Token: 0x040010FF RID: 4351
		public bool Dreaming;

		// Token: 0x04001100 RID: 4352
		public string ColorGrade;

		// Token: 0x04001101 RID: 4353
		public Action<Scene, bool, Action> Wipe;

		// Token: 0x04001102 RID: 4354
		public float DarknessAlpha = 0.05f;

		// Token: 0x04001103 RID: 4355
		public float BloomBase;

		// Token: 0x04001104 RID: 4356
		public float BloomStrength = 1f;

		// Token: 0x04001105 RID: 4357
		public Action<Level> OnLevelBegin;

		// Token: 0x04001106 RID: 4358
		public string Jumpthru = "wood";

		// Token: 0x04001107 RID: 4359
		public string Spike = "default";

		// Token: 0x04001108 RID: 4360
		public string CrumbleBlock = "default";

		// Token: 0x04001109 RID: 4361
		public string WoodPlatform = "default";

		// Token: 0x0400110A RID: 4362
		public Color CassseteNoteColor = Color.White;

		// Token: 0x0400110B RID: 4363
		public Color[] CobwebColor = new Color[]
		{
			Calc.HexToColor("696a6a")
		};

		// Token: 0x0400110C RID: 4364
		public string CassetteSong = "event:/music/cassette/01_forsaken_city";

		// Token: 0x0400110D RID: 4365
		public Session.CoreModes CoreMode;

		// Token: 0x0400110E RID: 4366
		public int MountainState;

		// Token: 0x0400110F RID: 4367
		public MountainCamera MountainIdle;

		// Token: 0x04001110 RID: 4368
		public MountainCamera MountainSelect;

		// Token: 0x04001111 RID: 4369
		public MountainCamera MountainZoom;

		// Token: 0x04001112 RID: 4370
		public Vector3 MountainCursor;

		// Token: 0x04001113 RID: 4371
		public float MountainCursorScale;
	}
}
