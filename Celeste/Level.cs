using System;
using System.Collections;
using System.Collections.Generic;
using Celeste.Editor;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x02000373 RID: 883
	public class Level : Scene, IOverlayHandler
	{
		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06001BBE RID: 7102 RVA: 0x000B7400 File Offset: 0x000B5600
		public Vector2 LevelOffset
		{
			get
			{
				return new Vector2((float)this.Bounds.Left, (float)this.Bounds.Top);
			}
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06001BBF RID: 7103 RVA: 0x000B7430 File Offset: 0x000B5630
		public Point LevelSolidOffset
		{
			get
			{
				return new Point(this.Bounds.Left / 8 - this.TileBounds.X, this.Bounds.Top / 8 - this.TileBounds.Y);
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06001BC0 RID: 7104 RVA: 0x000B747A File Offset: 0x000B567A
		public Rectangle TileBounds
		{
			get
			{
				return this.Session.MapData.TileBounds;
			}
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06001BC1 RID: 7105 RVA: 0x000B748C File Offset: 0x000B568C
		public bool Transitioning
		{
			get
			{
				return this.transition != null;
			}
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06001BC2 RID: 7106 RVA: 0x000B7497 File Offset: 0x000B5697
		// (set) Token: 0x06001BC3 RID: 7107 RVA: 0x000B749F File Offset: 0x000B569F
		public Vector2 ShakeVector { get; private set; }

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06001BC4 RID: 7108 RVA: 0x000B74A8 File Offset: 0x000B56A8
		public float VisualWind
		{
			get
			{
				return this.Wind.X + this.WindSine;
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06001BC5 RID: 7109 RVA: 0x000B74BC File Offset: 0x000B56BC
		public bool FrozenOrPaused
		{
			get
			{
				return this.Frozen || this.Paused;
			}
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06001BC6 RID: 7110 RVA: 0x000B74D0 File Offset: 0x000B56D0
		public bool CanPause
		{
			get
			{
				Player entity = base.Tracker.GetEntity<Player>();
				return entity != null && !entity.Dead && !this.wasPaused && !this.Paused && !this.PauseLock && !this.SkippingCutscene && !this.Transitioning && this.Wipe == null && !UserIO.Saving && (entity.LastBooster == null || !entity.LastBooster.Ch9HubTransition || !entity.LastBooster.BoostingPlayer);
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x06001BC7 RID: 7111 RVA: 0x000B7552 File Offset: 0x000B5752
		// (set) Token: 0x06001BC8 RID: 7112 RVA: 0x000B755A File Offset: 0x000B575A
		public Overlay Overlay { get; set; }

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06001BC9 RID: 7113 RVA: 0x000B7564 File Offset: 0x000B5764
		public bool ShowHud
		{
			get
			{
				return !this.Completed && (this.Paused || (base.Tracker.GetEntity<Textbox>() == null && base.Tracker.GetEntity<MiniTextbox>() == null && !this.Frozen && !this.InCutscene));
			}
		}

		// Token: 0x06001BCA RID: 7114 RVA: 0x000B75B2 File Offset: 0x000B57B2
		public override void Begin()
		{
			ScreenWipe.WipeColor = Color.Black;
			GameplayBuffers.Create();
			Distort.WaterAlpha = 1f;
			Distort.WaterSineDirection = 1f;
			Audio.MusicUnderwater = false;
			Audio.EndSnapshot(Level.DialogSnapshot);
			base.Begin();
		}

		// Token: 0x06001BCB RID: 7115 RVA: 0x000B75F0 File Offset: 0x000B57F0
		public override void End()
		{
			base.End();
			this.Foreground.Ended(this);
			this.Background.Ended(this);
			this.EndPauseEffects();
			Audio.BusStopAll("bus:/gameplay_sfx", false);
			Audio.MusicUnderwater = false;
			Audio.SetAmbience(null, true);
			Audio.SetAltMusic(null);
			Audio.EndSnapshot(Level.DialogSnapshot);
			Audio.ReleaseSnapshot(Level.AssistSpeedSnapshot);
			Level.AssistSpeedSnapshot = null;
			Level.AssistSpeedSnapshotValue = -1;
			GameplayBuffers.Unload();
			ClutterBlockGenerator.Dispose();
			Engine.TimeRateB = 1f;
		}

		// Token: 0x06001BCC RID: 7116 RVA: 0x000B7674 File Offset: 0x000B5874
		public void LoadLevel(Player.IntroTypes playerIntro, bool isFromLoader = false)
		{
			this.TimerHidden = false;
			this.TimerStopped = false;
			this.LastIntroType = playerIntro;
			this.Background.Fade = 0f;
			this.CanRetry = true;
			this.ScreenPadding = 0f;
			this.Displacement.Enabled = true;
			this.PauseLock = false;
			this.Frozen = false;
			this.CameraLockMode = Level.CameraLockModes.None;
			this.RetryPlayerCorpse = null;
			this.FormationBackdrop.Display = false;
			this.SaveQuitDisabled = false;
			this.lastColorGrade = this.Session.ColorGrade;
			this.colorGradeEase = 0f;
			this.colorGradeEaseSpeed = 1f;
			this.HasCassetteBlocks = false;
			this.CassetteBlockTempo = 1f;
			this.CassetteBlockBeats = 2;
			this.Raining = false;
			bool flag = false;
			bool flag2 = false;
			if (this.HiccupRandom == null)
			{
				this.HiccupRandom = new Random((int)(this.Session.Area.ID * 77 + this.Session.Area.Mode * (AreaMode)999));
			}
			LightningRenderer lightningRenderer = base.Entities.FindFirst<LightningRenderer>();
			if (lightningRenderer != null)
			{
				lightningRenderer.Reset();
			}
			Calc.PushRandom(this.Session.LevelData.LoadSeed);
			MapData mapData = this.Session.MapData;
			LevelData levelData = this.Session.LevelData;
			Vector2 vector = new Vector2((float)levelData.Bounds.Left, (float)levelData.Bounds.Top);
			bool flag3 = playerIntro != Player.IntroTypes.Fall || levelData.Name == "0";
			this.DarkRoom = (levelData.Dark && !this.Session.GetFlag("ignore_darkness_" + levelData.Name));
			this.Zoom = 1f;
			if (this.Session.Audio == null)
			{
				this.Session.Audio = AreaData.Get(this.Session).Mode[(int)this.Session.Area.Mode].AudioState.Clone();
			}
			if (!levelData.DelayAltMusic)
			{
				Audio.SetAltMusic(SFX.EventnameByHandle(levelData.AltMusic));
			}
			if (levelData.Music.Length > 0)
			{
				this.Session.Audio.Music.Event = SFX.EventnameByHandle(levelData.Music);
			}
			if (!AreaData.GetMode(this.Session.Area).IgnoreLevelAudioLayerData)
			{
				for (int i = 0; i < 4; i++)
				{
					this.Session.Audio.Music.Layer(i + 1, levelData.MusicLayers[i]);
				}
			}
			if (levelData.MusicProgress >= 0)
			{
				this.Session.Audio.Music.Progress = levelData.MusicProgress;
			}
			this.Session.Audio.Music.Layer(6, levelData.MusicWhispers);
			if (levelData.Ambience.Length > 0)
			{
				this.Session.Audio.Ambience.Event = SFX.EventnameByHandle(levelData.Ambience);
			}
			if (levelData.AmbienceProgress >= 0)
			{
				this.Session.Audio.Ambience.Progress = levelData.AmbienceProgress;
			}
			this.Session.Audio.Apply(isFromLoader);
			this.CoreMode = this.Session.CoreMode;
			this.NewLevel = !this.Session.LevelFlags.Contains(levelData.Name);
			if (flag3)
			{
				if (!this.Session.LevelFlags.Contains(levelData.Name))
				{
					this.Session.FurthestSeenLevel = levelData.Name;
				}
				this.Session.LevelFlags.Add(levelData.Name);
				this.Session.UpdateLevelStartDashes();
			}
			Vector2? startPosition = null;
			this.CameraOffset = new Vector2(48f, 32f) * levelData.CameraOffset;
			WindController windController = base.Entities.FindFirst<WindController>();
			if (windController != null)
			{
				windController.RemoveSelf();
			}
			base.Add(this.windController = new WindController(levelData.WindPattern));
			if (playerIntro != Player.IntroTypes.Transition)
			{
				this.windController.SetStartPattern();
			}
			if (levelData.Underwater)
			{
				base.Add(new Water(vector, false, false, (float)levelData.Bounds.Width, (float)levelData.Bounds.Height));
			}
			this.InSpace = levelData.Space;
			if (this.InSpace)
			{
				base.Add(new SpaceController());
			}
			if (levelData.Name == "-1" && this.Session.Area.ID == 0 && !SaveData.Instance.CheatMode)
			{
				base.Add(new UnlockEverythingThingy());
			}
			int num = 0;
			List<EntityID> list = new List<EntityID>();
			Player entity = base.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				foreach (Follower follower in entity.Leader.Followers)
				{
					list.Add(follower.ParentEntityID);
				}
			}
			foreach (EntityData entityData in levelData.Entities)
			{
				int id = entityData.ID;
				EntityID entityID = new EntityID(levelData.Name, id);
				if (!this.Session.DoNotLoad.Contains(entityID) && !list.Contains(entityID))
				{
					string name = entityData.Name;
					uint num2 = <PrivateImplementationDetails>.ComputeStringHash(name);
					if (num2 <= 2148869264U)
					{
						if (num2 <= 1025015505U)
						{
							if (num2 <= 575172026U)
							{
								if (num2 <= 222806219U)
								{
									if (num2 <= 161273198U)
									{
										if (num2 <= 52587787U)
										{
											if (num2 != 9226351U)
											{
												if (num2 != 52587787U)
												{
													continue;
												}
												if (!(name == "theoCrystalPedestal"))
												{
													continue;
												}
												base.Add(new TheoCrystalPedestal(entityData, vector));
												continue;
											}
											else
											{
												if (!(name == "floatySpaceBlock"))
												{
													continue;
												}
												base.Add(new FloatySpaceBlock(entityData, vector));
												continue;
											}
										}
										else if (num2 != 80761655U)
										{
											if (num2 != 152801799U)
											{
												if (num2 != 161273198U)
												{
													continue;
												}
												if (!(name == "cassetteBlock"))
												{
													continue;
												}
												CassetteBlock cassetteBlock = new CassetteBlock(entityData, vector, entityID);
												base.Add(cassetteBlock);
												this.HasCassetteBlocks = true;
												if (this.CassetteBlockTempo == 1f)
												{
													this.CassetteBlockTempo = cassetteBlock.Tempo;
												}
												this.CassetteBlockBeats = Math.Max(cassetteBlock.Index + 1, this.CassetteBlockBeats);
												if (flag)
												{
													continue;
												}
												flag = true;
												if (base.Tracker.GetEntity<CassetteBlockManager>() == null && this.ShouldCreateCassetteManager)
												{
													base.Add(new CassetteBlockManager());
													continue;
												}
												continue;
											}
											else
											{
												if (!(name == "lockBlock"))
												{
													continue;
												}
												base.Add(new LockBlock(entityData, vector, entityID));
												continue;
											}
										}
										else
										{
											if (!(name == "dreamHeartGem"))
											{
												continue;
											}
											if (!this.Session.HeartGem)
											{
												base.Add(new DreamHeartGem(entityData, vector));
												continue;
											}
											continue;
										}
									}
									else if (num2 <= 204722089U)
									{
										if (num2 != 195559697U)
										{
											if (num2 != 204722089U)
											{
												continue;
											}
											if (!(name == "cutsceneNode"))
											{
												continue;
											}
											base.Add(new CutsceneNode(entityData, vector));
											continue;
										}
										else
										{
											if (!(name == "soundSource"))
											{
												continue;
											}
											base.Add(new SoundSourceEntity(entityData, vector));
											continue;
										}
									}
									else if (num2 != 213157723U)
									{
										if (num2 != 215997667U)
										{
											if (num2 != 222806219U)
											{
												continue;
											}
											if (!(name == "wallBooster"))
											{
												continue;
											}
											base.Add(new WallBooster(entityData, vector));
											continue;
										}
										else
										{
											if (!(name == "payphone"))
											{
												continue;
											}
											base.Add(new Payphone(vector + entityData.Position));
											continue;
										}
									}
									else
									{
										if (!(name == "blackGem"))
										{
											continue;
										}
										if (!this.Session.HeartGem || this.Session.Area.Mode != AreaMode.Normal)
										{
											base.Add(new HeartGem(entityData, vector));
											continue;
										}
										continue;
									}
								}
								else if (num2 <= 413676777U)
								{
									if (num2 <= 267408341U)
									{
										if (num2 != 235448849U)
										{
											if (num2 != 267408341U)
											{
												continue;
											}
											if (!(name == "bigWaterfall"))
											{
												continue;
											}
											base.Add(new BigWaterfall(entityData, vector));
											continue;
										}
										else
										{
											if (!(name == "iceBlock"))
											{
												continue;
											}
											base.Add(new IceBlock(entityData, vector));
											continue;
										}
									}
									else if (num2 != 318028258U)
									{
										if (num2 != 372659800U)
										{
											if (num2 != 413676777U)
											{
												continue;
											}
											if (!(name == "gondola"))
											{
												continue;
											}
											base.Add(new Gondola(entityData, vector));
											continue;
										}
										else
										{
											if (!(name == "movingPlatform"))
											{
												continue;
											}
											base.Add(new MovingPlatform(entityData, vector));
											continue;
										}
									}
									else
									{
										if (!(name == "sandwichLava"))
										{
											continue;
										}
										base.Add(new SandwichLava(entityData, vector));
										continue;
									}
								}
								else if (num2 <= 564362684U)
								{
									if (num2 != 449227134U)
									{
										if (num2 != 564362684U)
										{
											continue;
										}
										if (!(name == "jumpThru"))
										{
											continue;
										}
										base.Add(new JumpthruPlatform(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "cloud"))
										{
											continue;
										}
										base.Add(new Cloud(entityData, vector));
										continue;
									}
								}
								else if (num2 != 566021791U)
								{
									if (num2 != 569751337U)
									{
										if (num2 != 575172026U)
										{
											continue;
										}
										if (!(name == "bounceBlock"))
										{
											continue;
										}
										base.Add(new BounceBlock(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "door"))
										{
											continue;
										}
										base.Add(new Door(entityData, vector));
										continue;
									}
								}
								else
								{
									if (!(name == "spikesLeft"))
									{
										continue;
									}
									base.Add(new Spikes(entityData, vector, Spikes.Directions.Left));
									continue;
								}
							}
							else if (num2 <= 726910904U)
							{
								if (num2 <= 646060618U)
								{
									if (num2 <= 592517570U)
									{
										if (num2 != 578985502U)
										{
											if (num2 != 592517570U)
											{
												continue;
											}
											if (!(name == "exitBlock"))
											{
												continue;
											}
											base.Add(new ExitBlock(entityData, vector));
											continue;
										}
										else
										{
											if (!(name == "seekerStatue"))
											{
												continue;
											}
											base.Add(new SeekerStatue(entityData, vector));
											continue;
										}
									}
									else if (num2 != 596355729U)
									{
										if (num2 != 610939280U)
										{
											if (num2 != 646060618U)
											{
												continue;
											}
											if (!(name == "seeker"))
											{
												continue;
											}
											base.Add(new Seeker(entityData, vector));
											continue;
										}
										else
										{
											if (!(name == "npc"))
											{
												continue;
											}
											string a = entityData.Attr("npc", "").ToLower();
											Vector2 position = entityData.Position + vector;
											if (a == "granny_00_house")
											{
												base.Add(new NPC00_Granny(position));
												continue;
											}
											if (a == "theo_01_campfire")
											{
												base.Add(new NPC01_Theo(position));
												continue;
											}
											if (a == "theo_02_campfire")
											{
												base.Add(new NPC02_Theo(position));
												continue;
											}
											if (a == "theo_03_escaping")
											{
												if (!this.Session.GetFlag("resort_theo"))
												{
													base.Add(new NPC03_Theo_Escaping(position));
													continue;
												}
												continue;
											}
											else
											{
												if (a == "theo_03_vents")
												{
													base.Add(new NPC03_Theo_Vents(position));
													continue;
												}
												if (a == "oshiro_03_lobby")
												{
													base.Add(new NPC03_Oshiro_Lobby(position));
													continue;
												}
												if (a == "oshiro_03_hallway")
												{
													base.Add(new NPC03_Oshiro_Hallway1(position));
													continue;
												}
												if (a == "oshiro_03_hallway2")
												{
													base.Add(new NPC03_Oshiro_Hallway2(position));
													continue;
												}
												if (a == "oshiro_03_bigroom")
												{
													base.Add(new NPC03_Oshiro_Cluttter(entityData, vector));
													continue;
												}
												if (a == "oshiro_03_breakdown")
												{
													base.Add(new NPC03_Oshiro_Breakdown(position));
													continue;
												}
												if (a == "oshiro_03_suite")
												{
													base.Add(new NPC03_Oshiro_Suite(position));
													continue;
												}
												if (a == "oshiro_03_rooftop")
												{
													base.Add(new NPC03_Oshiro_Rooftop(position));
													continue;
												}
												if (a == "granny_04_cliffside")
												{
													base.Add(new NPC04_Granny(position));
													continue;
												}
												if (a == "theo_04_cliffside")
												{
													base.Add(new NPC04_Theo(position));
													continue;
												}
												if (a == "theo_05_entrance")
												{
													base.Add(new NPC05_Theo_Entrance(position));
													continue;
												}
												if (a == "theo_05_inmirror")
												{
													base.Add(new NPC05_Theo_Mirror(position));
													continue;
												}
												if (a == "evil_05")
												{
													base.Add(new NPC05_Badeline(entityData, vector));
													continue;
												}
												if (a == "theo_06_plateau")
												{
													base.Add(new NPC06_Theo_Plateau(entityData, vector));
													continue;
												}
												if (a == "granny_06_intro")
												{
													base.Add(new NPC06_Granny(entityData, vector));
													continue;
												}
												if (a == "badeline_06_crying")
												{
													base.Add(new NPC06_Badeline_Crying(entityData, vector));
													continue;
												}
												if (a == "granny_06_ending")
												{
													base.Add(new NPC06_Granny_Ending(entityData, vector));
													continue;
												}
												if (a == "theo_06_ending")
												{
													base.Add(new NPC06_Theo_Ending(entityData, vector));
													continue;
												}
												if (a == "granny_07x")
												{
													base.Add(new NPC07X_Granny_Ending(entityData, vector, false));
													continue;
												}
												if (a == "theo_08_inside")
												{
													base.Add(new NPC08_Theo(entityData, vector));
													continue;
												}
												if (a == "granny_08_inside")
												{
													base.Add(new NPC08_Granny(entityData, vector));
													continue;
												}
												if (a == "granny_09_outside")
												{
													base.Add(new NPC09_Granny_Outside(entityData, vector));
													continue;
												}
												if (a == "granny_09_inside")
												{
													base.Add(new NPC09_Granny_Inside(entityData, vector));
													continue;
												}
												if (a == "gravestone_10")
												{
													base.Add(new NPC10_Gravestone(entityData, vector));
													continue;
												}
												if (a == "granny_10_never")
												{
													base.Add(new NPC07X_Granny_Ending(entityData, vector, true));
													continue;
												}
												continue;
											}
										}
									}
									else
									{
										if (!(name == "refill"))
										{
											continue;
										}
										base.Add(new Refill(entityData, vector));
										continue;
									}
								}
								else if (num2 <= 674211988U)
								{
									if (num2 != 671830573U)
									{
										if (num2 != 674211988U)
										{
											continue;
										}
										if (!(name == "memorialTextController"))
										{
											continue;
										}
										if (this.Session.Dashes == 0 && this.Session.StartedFromBeginning)
										{
											base.Add(new Strawberry(entityData, vector, entityID));
											continue;
										}
										continue;
									}
									else
									{
										if (!(name == "kevins_pc"))
										{
											continue;
										}
										base.Add(new KevinsPC(entityData, vector));
										continue;
									}
								}
								else if (num2 != 684976558U)
								{
									if (num2 != 705377853U)
									{
										if (num2 != 726910904U)
										{
											continue;
										}
										if (!(name == "flingBirdIntro"))
										{
											continue;
										}
										base.Add(new FlingBirdIntro(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "picoconsole"))
										{
											continue;
										}
										base.Add(new PicoConsole(entityData, vector));
										continue;
									}
								}
								else
								{
									if (!(name == "eyebomb"))
									{
										continue;
									}
									base.Add(new Puffer(entityData, vector));
									continue;
								}
							}
							else if (num2 <= 861328228U)
							{
								if (num2 <= 781922691U)
								{
									if (num2 != 751365554U)
									{
										if (num2 != 781922691U)
										{
											continue;
										}
										if (!(name == "invisibleBarrier"))
										{
											continue;
										}
										base.Add(new InvisibleBarrier(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "redBlocks"))
										{
											continue;
										}
										ClutterBlockGenerator.Init(this);
										ClutterBlockGenerator.Add((int)(entityData.Position.X / 8f), (int)(entityData.Position.Y / 8f), entityData.Width / 8, entityData.Height / 8, ClutterBlock.Colors.Red);
										continue;
									}
								}
								else if (num2 != 787325485U)
								{
									if (num2 != 792679097U)
									{
										if (num2 != 861328228U)
										{
											continue;
										}
										if (!(name == "glassBlock"))
										{
											continue;
										}
										base.Add(new GlassBlock(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "infiniteStar"))
										{
											continue;
										}
										base.Add(new FlyFeather(entityData, vector));
										continue;
									}
								}
								else
								{
									if (!(name == "crushBlock"))
									{
										continue;
									}
									base.Add(new CrushBlock(entityData, vector));
									continue;
								}
							}
							else if (num2 <= 895575064U)
							{
								if (num2 != 894534591U)
								{
									if (num2 != 895575064U)
									{
										continue;
									}
									if (!(name == "chaserBarrier"))
									{
										continue;
									}
									base.Add(new ChaserBarrier(entityData, vector));
									continue;
								}
								else
								{
									if (!(name == "cobweb"))
									{
										continue;
									}
									base.Add(new Cobweb(entityData, vector));
									continue;
								}
							}
							else if (num2 != 945501150U)
							{
								if (num2 != 946298880U)
								{
									if (num2 != 1025015505U)
									{
										continue;
									}
									if (!(name == "clothesline"))
									{
										continue;
									}
									base.Add(new Clothesline(entityData, vector));
									continue;
								}
								else
								{
									if (!(name == "fakeWall"))
									{
										continue;
									}
									base.Add(new FakeWall(entityID, entityData, vector, FakeWall.Modes.Wall));
									continue;
								}
							}
							else
							{
								if (!(name == "triggerSpikesRight"))
								{
									continue;
								}
								base.Add(new TriggerSpikes(entityData, vector, TriggerSpikes.Directions.Right));
								continue;
							}
						}
						else if (num2 <= 1560637382U)
						{
							if (num2 <= 1366274809U)
							{
								if (num2 <= 1237752336U)
								{
									if (num2 <= 1047946079U)
									{
										if (num2 != 1038666314U)
										{
											if (num2 != 1047946079U)
											{
												continue;
											}
											if (!(name == "templeMirror"))
											{
												continue;
											}
											base.Add(new TempleMirror(entityData, vector));
											continue;
										}
										else
										{
											if (!(name == "bridgeFixed"))
											{
												continue;
											}
											base.Add(new BridgeFixed(entityData, vector));
											continue;
										}
									}
									else if (num2 != 1125582856U)
									{
										if (num2 != 1211431605U)
										{
											if (num2 != 1237752336U)
											{
												continue;
											}
											if (!(name == "water"))
											{
												continue;
											}
											base.Add(new Water(entityData, vector));
											continue;
										}
										else
										{
											if (!(name == "moonCreature"))
											{
												continue;
											}
											base.Add(new MoonCreature(entityData, vector));
											continue;
										}
									}
									else
									{
										if (!(name == "reflectionHeartStatue"))
										{
											continue;
										}
										base.Add(new ReflectionHeartStatue(entityData, vector));
										continue;
									}
								}
								else if (num2 <= 1284124754U)
								{
									if (num2 != 1238105105U)
									{
										if (num2 != 1284124754U)
										{
											continue;
										}
										if (!(name == "dashBlock"))
										{
											continue;
										}
										base.Add(new DashBlock(entityData, vector, entityID));
										continue;
									}
									else
									{
										if (!(name == "cliffside_flag"))
										{
											continue;
										}
										base.Add(new CliffsideWindFlag(entityData, vector));
										continue;
									}
								}
								else if (num2 != 1302160983U)
								{
									if (num2 != 1318385285U)
									{
										if (num2 != 1366274809U)
										{
											continue;
										}
										if (!(name == "waterfall"))
										{
											continue;
										}
										base.Add(new WaterFall(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "triggerSpikesLeft"))
										{
											continue;
										}
										base.Add(new TriggerSpikes(entityData, vector, TriggerSpikes.Directions.Left));
										continue;
									}
								}
								else
								{
									if (!(name == "summitcloud"))
									{
										continue;
									}
									base.Add(new SummitCloud(entityData, vector));
									continue;
								}
							}
							else if (num2 <= 1429671225U)
							{
								if (num2 <= 1388897414U)
								{
									if (num2 != 1375342212U)
									{
										if (num2 != 1388897414U)
										{
											continue;
										}
										if (!(name == "blockField"))
										{
											continue;
										}
										base.Add(new BlockField(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "touchSwitch"))
										{
											continue;
										}
										base.Add(new TouchSwitch(entityData, vector));
										continue;
									}
								}
								else if (num2 != 1401920885U)
								{
									if (num2 != 1407523398U)
									{
										if (num2 != 1429671225U)
										{
											continue;
										}
										if (!(name == "summitgem"))
										{
											continue;
										}
										base.Add(new SummitGem(entityData, vector, entityID));
										continue;
									}
									else
									{
										if (!(name == "wallSpringRight"))
										{
											continue;
										}
										base.Add(new Spring(entityData, vector, Spring.Orientations.WallRight));
										continue;
									}
								}
								else
								{
									if (!(name == "dreamBlock"))
									{
										continue;
									}
									base.Add(new DreamBlock(entityData, vector));
									continue;
								}
							}
							else if (num2 <= 1513500148U)
							{
								if (num2 != 1496617359U)
								{
									if (num2 != 1513500148U)
									{
										continue;
									}
									if (!(name == "fireBarrier"))
									{
										continue;
									}
									base.Add(new FireBarrier(entityData, vector));
									continue;
								}
								else
								{
									if (!(name == "resortmirror"))
									{
										continue;
									}
									base.Add(new ResortMirror(entityData, vector));
									continue;
								}
							}
							else if (num2 != 1519562801U)
							{
								if (num2 != 1552748532U)
								{
									if (num2 != 1560637382U)
									{
										continue;
									}
									if (!(name == "spinner"))
									{
										continue;
									}
									if (this.Session.Area.ID == 3 || (this.Session.Area.ID == 7 && this.Session.Level.StartsWith("d-")))
									{
										base.Add(new DustStaticSpinner(entityData, vector));
										continue;
									}
									CrystalColor color = CrystalColor.Blue;
									if (this.Session.Area.ID == 5)
									{
										color = CrystalColor.Red;
									}
									else if (this.Session.Area.ID == 6)
									{
										color = CrystalColor.Purple;
									}
									else if (this.Session.Area.ID == 10)
									{
										color = CrystalColor.Rainbow;
									}
									base.Add(new CrystalStaticSpinner(entityData, vector, color));
									continue;
								}
								else
								{
									if (!(name == "bonfire"))
									{
										continue;
									}
									base.Add(new Bonfire(entityData, vector));
									continue;
								}
							}
							else
							{
								if (!(name == "negaBlock"))
								{
									continue;
								}
								base.Add(new NegaBlock(entityData, vector));
								continue;
							}
						}
						else if (num2 <= 1856792572U)
						{
							if (num2 <= 1810951995U)
							{
								if (num2 <= 1676751724U)
								{
									if (num2 != 1605062321U)
									{
										if (num2 != 1676751724U)
										{
											continue;
										}
										if (!(name == "spring"))
										{
											continue;
										}
										base.Add(new Spring(entityData, vector, Spring.Orientations.Floor));
										continue;
									}
									else
									{
										if (!(name == "birdPath"))
										{
											continue;
										}
										base.Add(new BirdPath(entityID, entityData, vector));
										continue;
									}
								}
								else if (num2 != 1692621893U)
								{
									if (num2 != 1746258028U)
									{
										if (num2 != 1810951995U)
										{
											continue;
										}
										if (!(name == "lamp"))
										{
											continue;
										}
										base.Add(new Lamp(vector + entityData.Position, entityData.Bool("broken", false)));
										continue;
									}
									else
									{
										if (!(name == "key"))
										{
											continue;
										}
										base.Add(new Key(entityData, vector, entityID));
										continue;
									}
								}
								else
								{
									if (!(name == "introCar"))
									{
										continue;
									}
									base.Add(new IntroCar(entityData, vector));
									continue;
								}
							}
							else if (num2 <= 1848791901U)
							{
								if (num2 != 1826206115U)
								{
									if (num2 != 1848791901U)
									{
										continue;
									}
									if (!(name == "coreMessage"))
									{
										continue;
									}
									base.Add(new CoreMessage(entityData, vector));
									continue;
								}
								else
								{
									if (!(name == "rotateSpinner"))
									{
										continue;
									}
									if (this.Session.Area.ID == 10)
									{
										base.Add(new StarRotateSpinner(entityData, vector));
										continue;
									}
									if (this.Session.Area.ID == 3 || (this.Session.Area.ID == 7 && this.Session.Level.StartsWith("d-")))
									{
										base.Add(new DustRotateSpinner(entityData, vector));
										continue;
									}
									base.Add(new BladeRotateSpinner(entityData, vector));
									continue;
								}
							}
							else if (num2 != 1854405085U)
							{
								if (num2 != 1855539126U)
								{
									if (num2 != 1856792572U)
									{
										continue;
									}
									if (!(name == "slider"))
									{
										continue;
									}
									base.Add(new Slider(entityData, vector));
									continue;
								}
								else
								{
									if (!(name == "switchGate"))
									{
										continue;
									}
									base.Add(new SwitchGate(entityData, vector));
									continue;
								}
							}
							else
							{
								if (!(name == "coverupWall"))
								{
									continue;
								}
								base.Add(new CoverupWall(entityData, vector));
								continue;
							}
						}
						else if (num2 <= 2003626169U)
						{
							if (num2 <= 1878541890U)
							{
								if (num2 != 1858286930U)
								{
									if (num2 != 1878541890U)
									{
										continue;
									}
									if (!(name == "lightningBlock"))
									{
										continue;
									}
									base.Add(new LightningBreakerBox(entityData, vector));
									continue;
								}
								else
								{
									if (!(name == "greenBlocks"))
									{
										continue;
									}
									ClutterBlockGenerator.Init(this);
									ClutterBlockGenerator.Add((int)(entityData.Position.X / 8f), (int)(entityData.Position.Y / 8f), entityData.Width / 8, entityData.Height / 8, ClutterBlock.Colors.Green);
									continue;
								}
							}
							else if (num2 != 1890995534U)
							{
								if (num2 != 1990189131U)
								{
									if (num2 != 2003626169U)
									{
										continue;
									}
									if (!(name == "cassette"))
									{
										continue;
									}
									if (!this.Session.Cassette)
									{
										base.Add(new Cassette(entityData, vector));
										continue;
									}
									continue;
								}
								else if (!(name == "swapBlock"))
								{
									continue;
								}
							}
							else
							{
								if (!(name == "flutterbird"))
								{
									continue;
								}
								base.Add(new FlutterBird(entityData, vector));
								continue;
							}
						}
						else if (num2 <= 2077000741U)
						{
							if (num2 != 2059041933U)
							{
								if (num2 != 2077000741U)
								{
									continue;
								}
								if (!(name == "birdForsakenCityGem"))
								{
									continue;
								}
								base.Add(new ForsakenCitySatellite(entityData, vector));
								continue;
							}
							else
							{
								if (!(name == "wallSpringLeft"))
								{
									continue;
								}
								base.Add(new Spring(entityData, vector, Spring.Orientations.WallLeft));
								continue;
							}
						}
						else if (num2 != 2092346725U)
						{
							if (num2 != 2130575545U)
							{
								if (num2 != 2148869264U)
								{
									continue;
								}
								if (!(name == "tentacles"))
								{
									continue;
								}
								base.Add(new ReflectionTentacles(entityData, vector));
								continue;
							}
							else
							{
								if (!(name == "resortRoofEnding"))
								{
									continue;
								}
								base.Add(new ResortRoofEnding(entityData, vector));
								continue;
							}
						}
						else
						{
							if (!(name == "friendlyGhost"))
							{
								continue;
							}
							base.Add(new AngryOshiro(entityData, vector));
							continue;
						}
					}
					else if (num2 <= 3044120231U)
					{
						if (num2 <= 2613483558U)
						{
							if (num2 <= 2435441605U)
							{
								if (num2 <= 2343958089U)
								{
									if (num2 <= 2200315251U)
									{
										if (num2 != 2164623049U)
										{
											if (num2 != 2200315251U)
											{
												continue;
											}
											if (!(name == "memorial"))
											{
												continue;
											}
											base.Add(new Memorial(entityData, vector));
											continue;
										}
										else
										{
											if (!(name == "triggerSpikesUp"))
											{
												continue;
											}
											base.Add(new TriggerSpikes(entityData, vector, TriggerSpikes.Directions.Up));
											continue;
										}
									}
									else if (num2 != 2218399068U)
									{
										if (num2 != 2222412862U)
										{
											if (num2 != 2343958089U)
											{
												continue;
											}
											if (!(name == "zipMover"))
											{
												continue;
											}
											base.Add(new ZipMover(entityData, vector));
											continue;
										}
										else
										{
											if (!(name == "trapdoor"))
											{
												continue;
											}
											base.Add(new Trapdoor(entityData, vector));
											continue;
										}
									}
									else
									{
										if (!(name == "strawberry"))
										{
											continue;
										}
										base.Add(new Strawberry(entityData, vector, entityID));
										continue;
									}
								}
								else if (num2 <= 2370377959U)
								{
									if (num2 != 2363884029U)
									{
										if (num2 != 2370377959U)
										{
											continue;
										}
										if (!(name == "introCrusher"))
										{
											continue;
										}
										base.Add(new IntroCrusher(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "darkChaser"))
										{
											continue;
										}
										base.Add(new BadelineOldsite(entityData, vector, num));
										num++;
										continue;
									}
								}
								else if (num2 != 2375284596U)
								{
									if (num2 != 2425491370U)
									{
										if (num2 != 2435441605U)
										{
											continue;
										}
										if (!(name == "coreModeToggle"))
										{
											continue;
										}
										base.Add(new CoreModeToggle(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "powerSourceNumber"))
										{
											continue;
										}
										base.Add(new PowerSourceNumber(entityData.Position + vector, entityData.Int("number", 1), this.GotCollectables(entityData)));
										continue;
									}
								}
								else
								{
									if (!(name == "templeCrackedBlock"))
									{
										continue;
									}
									base.Add(new TempleCrackedBlock(entityID, entityData, vector));
									continue;
								}
							}
							else if (num2 <= 2510061083U)
							{
								if (num2 <= 2443734766U)
								{
									if (num2 != 2437279610U)
									{
										if (num2 != 2443734766U)
										{
											continue;
										}
										if (!(name == "templeBigEyeball"))
										{
											continue;
										}
										base.Add(new TempleBigEyeball(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "finalBossFallingBlock"))
										{
											continue;
										}
										base.Add(FallingBlock.CreateFinalBossBlock(entityData, vector));
										continue;
									}
								}
								else if (num2 != 2459492797U)
								{
									if (num2 != 2499067864U)
									{
										if (num2 != 2510061083U)
										{
											continue;
										}
										if (!(name == "conditionBlock"))
										{
											continue;
										}
										Level.ConditionBlockModes conditionBlockModes = entityData.Enum<Level.ConditionBlockModes>("condition", Level.ConditionBlockModes.Key);
										EntityID none = EntityID.None;
										string[] array = entityData.Attr("conditionID", "").Split(new char[]
										{
											':'
										});
										none.Level = array[0];
										none.ID = Convert.ToInt32(array[1]);
										bool flag4;
										if (conditionBlockModes == Level.ConditionBlockModes.Button)
										{
											flag4 = this.Session.GetFlag(DashSwitch.GetFlagName(none));
										}
										else if (conditionBlockModes == Level.ConditionBlockModes.Key)
										{
											flag4 = this.Session.DoNotLoad.Contains(none);
										}
										else
										{
											if (conditionBlockModes != Level.ConditionBlockModes.Strawberry)
											{
												throw new Exception("Condition type not supported!");
											}
											flag4 = this.Session.Strawberries.Contains(none);
										}
										if (flag4)
										{
											base.Add(new ExitBlock(entityData, vector));
											continue;
										}
										continue;
									}
									else
									{
										if (!(name == "fakeHeart"))
										{
											continue;
										}
										base.Add(new FakeHeart(entityData, vector));
										continue;
									}
								}
								else
								{
									if (!(name == "sinkingPlatform"))
									{
										continue;
									}
									base.Add(new SinkingPlatform(entityData, vector));
									continue;
								}
							}
							else if (num2 <= 2540775602U)
							{
								if (num2 != 2532045385U)
								{
									if (num2 != 2540775602U)
									{
										continue;
									}
									if (!(name == "bird"))
									{
										continue;
									}
									base.Add(new BirdNPC(entityData, vector));
									continue;
								}
								else
								{
									if (!(name == "plateau"))
									{
										continue;
									}
									base.Add(new Plateau(entityData, vector));
									continue;
								}
							}
							else if (num2 != 2560795989U)
							{
								if (num2 != 2592350980U)
								{
									if (num2 != 2613483558U)
									{
										continue;
									}
									if (!(name == "switchBlock"))
									{
										continue;
									}
								}
								else
								{
									if (!(name == "colorSwitch"))
									{
										continue;
									}
									base.Add(new ClutterSwitch(entityData, vector));
									continue;
								}
							}
							else
							{
								if (!(name == "finalBossMovingBlock"))
								{
									continue;
								}
								base.Add(new FinalBossMovingBlock(entityData, vector));
								continue;
							}
						}
						else if (num2 <= 2802483833U)
						{
							if (num2 <= 2706859956U)
							{
								if (num2 <= 2640383021U)
								{
									if (num2 != 2626889435U)
									{
										if (num2 != 2640383021U)
										{
											continue;
										}
										if (!(name == "risingLava"))
										{
											continue;
										}
										base.Add(new RisingLava(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "rotatingPlatforms"))
										{
											continue;
										}
										Vector2 value = entityData.Position + vector;
										Vector2 vector2 = entityData.Nodes[0] + vector;
										int width = entityData.Width;
										int num3 = entityData.Int("platforms", 0);
										bool clockwise = entityData.Bool("clockwise", false);
										float length = (value - vector2).Length();
										float num4 = (value - vector2).Angle();
										float num5 = 6.2831855f / (float)num3;
										for (int j = 0; j < num3; j++)
										{
											float angleRadians = num4 + num5 * (float)j;
											angleRadians = Calc.WrapAngle(angleRadians);
											Vector2 position2 = vector2 + Calc.AngleToVector(angleRadians, length);
											base.Add(new RotatingPlatform(position2, width, vector2, clockwise));
										}
										continue;
									}
								}
								else if (num2 != 2669543514U)
								{
									if (num2 != 2696566046U)
									{
										if (num2 != 2706859956U)
										{
											continue;
										}
										if (!(name == "summitcheckpoint"))
										{
											continue;
										}
										base.Add(new SummitCheckpoint(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "clutterCabinet"))
										{
											continue;
										}
										base.Add(new ClutterCabinet(entityData, vector));
										continue;
									}
								}
								else
								{
									if (!(name == "hahaha"))
									{
										continue;
									}
									base.Add(new Hahaha(entityData, vector));
									continue;
								}
							}
							else if (num2 <= 2724447635U)
							{
								if (num2 != 2719756024U)
								{
									if (num2 != 2724447635U)
									{
										continue;
									}
									if (!(name == "crumbleWallOnRumble"))
									{
										continue;
									}
									base.Add(new CrumbleWallOnRumble(entityData, vector, entityID));
									continue;
								}
								else
								{
									if (!(name == "summitGemManager"))
									{
										continue;
									}
									base.Add(new SummitGemManager(entityData, vector));
									continue;
								}
							}
							else if (num2 != 2760486510U)
							{
								if (num2 != 2790929943U)
								{
									if (num2 != 2802483833U)
									{
										continue;
									}
									if (!(name == "fallingBlock"))
									{
										continue;
									}
									base.Add(new FallingBlock(entityData, vector));
									continue;
								}
								else
								{
									if (!(name == "yellowBlocks"))
									{
										continue;
									}
									ClutterBlockGenerator.Init(this);
									ClutterBlockGenerator.Add((int)(entityData.Position.X / 8f), (int)(entityData.Position.Y / 8f), entityData.Width / 8, entityData.Height / 8, ClutterBlock.Colors.Yellow);
									continue;
								}
							}
							else
							{
								if (!(name == "heartGemDoor"))
								{
									continue;
								}
								base.Add(new HeartGemDoor(entityData, vector));
								continue;
							}
						}
						else if (num2 <= 2922681633U)
						{
							if (num2 <= 2845320905U)
							{
								if (num2 != 2841206838U)
								{
									if (num2 != 2845320905U)
									{
										continue;
									}
									if (!(name == "SummitBackgroundManager"))
									{
										continue;
									}
									base.Add(new AscendManager(entityData, vector));
									continue;
								}
								else
								{
									if (!(name == "killbox"))
									{
										continue;
									}
									base.Add(new Killbox(entityData, vector));
									continue;
								}
							}
							else if (num2 != 2876813229U)
							{
								if (num2 != 2880622461U)
								{
									if (num2 != 2922681633U)
									{
										continue;
									}
									if (!(name == "torch"))
									{
										continue;
									}
									base.Add(new Torch(entityData, vector, entityID));
									continue;
								}
								else
								{
									if (!(name == "goldenBlock"))
									{
										continue;
									}
									base.Add(new GoldenBlock(entityData, vector));
									continue;
								}
							}
							else
							{
								if (!(name == "fakeBlock"))
								{
									continue;
								}
								base.Add(new FakeWall(entityID, entityData, vector, FakeWall.Modes.Block));
								continue;
							}
						}
						else if (num2 <= 2950802439U)
						{
							if (num2 != 2940633403U)
							{
								if (num2 != 2950802439U)
								{
									continue;
								}
								if (!(name == "SoundTest3d"))
								{
									continue;
								}
								base.Add(new _3dSoundTest(entityData, vector));
								continue;
							}
							else
							{
								if (!(name == "templeGate"))
								{
									continue;
								}
								base.Add(new TempleGate(entityData, vector, levelData.Name));
								continue;
							}
						}
						else if (num2 != 2952295164U)
						{
							if (num2 != 3037452417U)
							{
								if (num2 != 3044120231U)
								{
									continue;
								}
								if (!(name == "oshirodoor"))
								{
									continue;
								}
								base.Add(new MrOshiroDoor(entityData, vector));
								continue;
							}
							else
							{
								if (!(name == "seekerBarrier"))
								{
									continue;
								}
								base.Add(new SeekerBarrier(entityData, vector));
								continue;
							}
						}
						else
						{
							if (!(name == "floatingDebris"))
							{
								continue;
							}
							base.Add(new FloatingDebris(entityData, vector));
							continue;
						}
					}
					else
					{
						if (num2 > 3886993895U)
						{
							if (num2 <= 3997399324U)
							{
								if (num2 <= 3935989976U)
								{
									if (num2 <= 3898938152U)
									{
										if (num2 != 3893556229U)
										{
											if (num2 != 3898938152U)
											{
												continue;
											}
											if (!(name == "lightbeam"))
											{
												continue;
											}
											base.Add(new LightBeam(entityData, vector));
											continue;
										}
										else
										{
											if (!(name == "lightning"))
											{
												continue;
											}
											bool flag5 = entityData.Bool("perLevel", false) || !this.Session.GetFlag("disable_lightning");
											if (flag5)
											{
												base.Add(new Lightning(entityData, vector));
												flag2 = true;
												continue;
											}
											continue;
										}
									}
									else if (num2 != 3900438013U)
									{
										if (num2 != 3927636875U)
										{
											if (num2 != 3935989976U)
											{
												continue;
											}
											if (!(name == "fireBall"))
											{
												continue;
											}
											base.Add(new FireBall(entityData, vector));
											continue;
										}
										else if (!(name == "dashSwitchH"))
										{
											continue;
										}
									}
									else
									{
										if (!(name == "wavedashmachine"))
										{
											continue;
										}
										base.Add(new WaveDashTutorialMachine(entityData, vector));
										continue;
									}
								}
								else if (num2 <= 3937928720U)
								{
									if (num2 != 3937539705U)
									{
										if (num2 != 3937928720U)
										{
											continue;
										}
										if (!(name == "bridge"))
										{
											continue;
										}
										base.Add(new Bridge(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "templeEye"))
										{
											continue;
										}
										base.Add(new TempleEye(entityData, vector));
										continue;
									}
								}
								else if (num2 != 3990819637U)
								{
									if (num2 != 3992636533U)
									{
										if (num2 != 3997399324U)
										{
											continue;
										}
										if (!(name == "badelineBoost"))
										{
											continue;
										}
										base.Add(new BadelineBoost(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "theoCrystal"))
										{
											continue;
										}
										base.Add(new TheoCrystal(entityData, vector));
										continue;
									}
								}
								else
								{
									if (!(name == "hanginglamp"))
									{
										continue;
									}
									base.Add(new HangingLamp(entityData, vector + entityData.Position));
									continue;
								}
							}
							else if (num2 <= 4084982687U)
							{
								if (num2 <= 4039348104U)
								{
									if (num2 != 4008221787U)
									{
										if (num2 != 4039348104U)
										{
											continue;
										}
										if (!(name == "towerviewer"))
										{
											continue;
										}
										base.Add(new Lookout(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "cliffflag"))
										{
											continue;
										}
										base.Add(new CliffFlags(entityData, vector));
										continue;
									}
								}
								else if (num2 != 4054789074U)
								{
									if (num2 != 4082753944U)
									{
										if (num2 != 4084982687U)
										{
											continue;
										}
										if (!(name == "playerSeeker"))
										{
											continue;
										}
										base.Add(new PlayerSeeker(entityData, vector));
										continue;
									}
									else
									{
										if (!(name == "wire"))
										{
											continue;
										}
										base.Add(new Wire(entityData, vector));
										continue;
									}
								}
								else
								{
									if (!(name == "clutterDoor"))
									{
										continue;
									}
									base.Add(new ClutterDoor(entityData, vector, this.Session));
									continue;
								}
							}
							else if (num2 <= 4162523541U)
							{
								if (num2 != 4151454224U)
								{
									if (num2 != 4158956927U)
									{
										if (num2 != 4162523541U)
										{
											continue;
										}
										if (!(name == "dashSwitchV"))
										{
											continue;
										}
									}
									else
									{
										if (!(name == "checkpoint"))
										{
											continue;
										}
										if (flag3)
										{
											Checkpoint checkpoint = new Checkpoint(entityData, vector);
											base.Add(checkpoint);
											startPosition = new Vector2?(entityData.Position + vector + checkpoint.SpawnOffset);
											continue;
										}
										continue;
									}
								}
								else
								{
									if (!(name == "crumbleBlock"))
									{
										continue;
									}
									base.Add(new CrumblePlatform(entityData, vector));
									continue;
								}
							}
							else if (num2 != 4180794884U)
							{
								if (num2 != 4241930501U)
								{
									if (num2 != 4286327515U)
									{
										continue;
									}
									if (!(name == "booster"))
									{
										continue;
									}
									base.Add(new Booster(entityData, vector));
									continue;
								}
								else
								{
									if (!(name == "templeMirrorPortal"))
									{
										continue;
									}
									base.Add(new TempleMirrorPortal(entityData, vector));
									continue;
								}
							}
							else
							{
								if (!(name == "resortLantern"))
								{
									continue;
								}
								base.Add(new ResortLantern(entityData, vector));
								continue;
							}
							base.Add(DashSwitch.Create(entityData, vector, entityID));
							continue;
						}
						if (num2 <= 3465903364U)
						{
							if (num2 <= 3296307935U)
							{
								if (num2 <= 3151942350U)
								{
									if (num2 != 3080934558U)
									{
										if (num2 != 3151942350U)
										{
											continue;
										}
										if (!(name == "starClimbController"))
										{
											continue;
										}
										base.Add(new StarJumpController());
										continue;
									}
									else
									{
										if (!(name == "starJumpBlock"))
										{
											continue;
										}
										base.Add(new StarJumpBlock(entityData, vector));
										continue;
									}
								}
								else if (num2 != 3159511168U)
								{
									if (num2 != 3228136161U)
									{
										if (num2 != 3296307935U)
										{
											continue;
										}
										if (!(name == "dreammirror"))
										{
											continue;
										}
										base.Add(new DreamMirror(vector + entityData.Position));
										continue;
									}
									else
									{
										if (!(name == "trackSpinner"))
										{
											continue;
										}
										if (this.Session.Area.ID == 10)
										{
											base.Add(new StarTrackSpinner(entityData, vector));
											continue;
										}
										if (this.Session.Area.ID == 3 || (this.Session.Area.ID == 7 && this.Session.Level.StartsWith("d-")))
										{
											base.Add(new DustTrackSpinner(entityData, vector));
											continue;
										}
										base.Add(new BladeTrackSpinner(entityData, vector));
										continue;
									}
								}
								else
								{
									if (!(name == "glider"))
									{
										continue;
									}
									base.Add(new Glider(entityData, vector));
									continue;
								}
							}
							else if (num2 <= 3348026958U)
							{
								if (num2 != 3299453551U)
								{
									if (num2 != 3348026958U)
									{
										continue;
									}
									if (!(name == "finalBoss"))
									{
										continue;
									}
									base.Add(new FinalBoss(entityData, vector));
									continue;
								}
								else
								{
									if (!(name == "spikesUp"))
									{
										continue;
									}
									base.Add(new Spikes(entityData, vector, Spikes.Directions.Up));
									continue;
								}
							}
							else if (num2 != 3361728423U)
							{
								if (num2 != 3456914806U)
								{
									if (num2 != 3465903364U)
									{
										continue;
									}
									if (!(name == "goldenBerry"))
									{
										continue;
									}
									bool cheatMode = SaveData.Instance.CheatMode;
									bool flag6 = this.Session.FurthestSeenLevel == this.Session.Level || this.Session.Deaths == 0;
									bool flag7 = SaveData.Instance.UnlockedModes >= 3 || SaveData.Instance.DebugMode;
									bool completed = SaveData.Instance.Areas[this.Session.Area.ID].Modes[(int)this.Session.Area.Mode].Completed;
									if ((cheatMode || (flag7 && completed)) && flag6)
									{
										base.Add(new Strawberry(entityData, vector, entityID));
										continue;
									}
									continue;
								}
								else
								{
									if (!(name == "playbackTutorial"))
									{
										continue;
									}
									base.Add(new PlayerPlayback(entityData, vector));
									continue;
								}
							}
							else
							{
								if (!(name == "playbackBillboard"))
								{
									continue;
								}
								base.Add(new PlaybackBillboard(entityData, vector));
								continue;
							}
						}
						else if (num2 <= 3572560554U)
						{
							if (num2 <= 3505615487U)
							{
								if (num2 != 3487266669U)
								{
									if (num2 != 3505615487U)
									{
										continue;
									}
									if (!(name == "foregroundDebris"))
									{
										continue;
									}
									base.Add(new ForegroundDebris(entityData, vector));
									continue;
								}
								else
								{
									if (!(name == "moveBlock"))
									{
										continue;
									}
									base.Add(new MoveBlock(entityData, vector));
									continue;
								}
							}
							else if (num2 != 3535036080U)
							{
								if (num2 != 3560845264U)
								{
									if (num2 != 3572560554U)
									{
										continue;
									}
									if (!(name == "spikesDown"))
									{
										continue;
									}
									base.Add(new Spikes(entityData, vector, Spikes.Directions.Down));
									continue;
								}
								else
								{
									if (!(name == "spikesRight"))
									{
										continue;
									}
									base.Add(new Spikes(entityData, vector, Spikes.Directions.Right));
									continue;
								}
							}
							else
							{
								if (!(name == "triggerSpikesDown"))
								{
									continue;
								}
								base.Add(new TriggerSpikes(entityData, vector, TriggerSpikes.Directions.Down));
								continue;
							}
						}
						else if (num2 <= 3801947695U)
						{
							if (num2 != 3760641466U)
							{
								if (num2 != 3801947695U)
								{
									continue;
								}
								if (!(name == "light"))
								{
									continue;
								}
								base.Add(new PropLight(entityData, vector));
								continue;
							}
							else
							{
								if (!(name == "bigSpinner"))
								{
									continue;
								}
								base.Add(new Bumper(entityData, vector));
								continue;
							}
						}
						else if (num2 != 3819775742U)
						{
							if (num2 != 3866100643U)
							{
								if (num2 != 3886993895U)
								{
									continue;
								}
								if (!(name == "ridgeGate"))
								{
									continue;
								}
								if (this.GotCollectables(entityData))
								{
									base.Add(new RidgeGate(entityData, vector));
									continue;
								}
								continue;
							}
							else
							{
								if (!(name == "whiteblock"))
								{
									continue;
								}
								base.Add(new WhiteBlock(entityData, vector));
								continue;
							}
						}
						else
						{
							if (!(name == "flingBird"))
							{
								continue;
							}
							base.Add(new FlingBird(entityData, vector));
							continue;
						}
					}
					base.Add(new SwapBlock(entityData, vector));
				}
			}
			ClutterBlockGenerator.Generate();
			foreach (EntityData entityData2 in levelData.Triggers)
			{
				int entityID2 = entityData2.ID + 10000000;
				EntityID entityID3 = new EntityID(levelData.Name, entityID2);
				if (!this.Session.DoNotLoad.Contains(entityID3))
				{
					string name = entityData2.Name;
					uint num2 = <PrivateImplementationDetails>.ComputeStringHash(name);
					if (num2 <= 2175028017U)
					{
						if (num2 <= 1308852169U)
						{
							if (num2 <= 382937527U)
							{
								if (num2 != 282179160U)
								{
									if (num2 != 329132276U)
									{
										if (num2 == 382937527U)
										{
											if (name == "lightFadeTrigger")
											{
												base.Add(new LightFadeTrigger(entityData2, vector));
											}
										}
									}
									else if (name == "lookoutBlocker")
									{
										base.Add(new LookoutBlocker(entityData2, vector));
									}
								}
								else if (name == "musicTrigger")
								{
									base.Add(new MusicTrigger(entityData2, vector));
								}
							}
							else if (num2 <= 507524009U)
							{
								if (num2 != 505159009U)
								{
									if (num2 == 507524009U)
									{
										if (name == "windTrigger")
										{
											base.Add(new WindTrigger(entityData2, vector));
										}
									}
								}
								else if (name == "cameraTargetTrigger")
								{
									string text = entityData2.Attr("deleteFlag", "");
									if (string.IsNullOrEmpty(text) || !this.Session.GetFlag(text))
									{
										base.Add(new CameraTargetTrigger(entityData2, vector));
									}
								}
							}
							else if (num2 != 1049115386U)
							{
								if (num2 == 1308852169U)
								{
									if (name == "altMusicTrigger")
									{
										base.Add(new AltMusicTrigger(entityData2, vector));
									}
								}
							}
							else if (name == "ambienceParamTrigger")
							{
								base.Add(new AmbienceParamTrigger(entityData2, vector));
							}
						}
						else if (num2 <= 1495096055U)
						{
							if (num2 != 1428627673U)
							{
								if (num2 != 1464301262U)
								{
									if (num2 == 1495096055U)
									{
										if (name == "birdPathTrigger")
										{
											base.Add(new BirdPathTrigger(entityData2, vector));
										}
									}
								}
								else if (name == "goldenBerryCollectTrigger")
								{
									base.Add(new GoldBerryCollectTrigger(entityData2, vector));
								}
							}
							else if (name == "changeRespawnTrigger")
							{
								base.Add(new ChangeRespawnTrigger(entityData2, vector));
							}
						}
						else if (num2 <= 1949955238U)
						{
							if (num2 != 1753659785U)
							{
								if (num2 == 1949955238U)
								{
									if (name == "noRefillTrigger")
									{
										base.Add(new NoRefillTrigger(entityData2, vector));
									}
								}
							}
							else if (name == "checkpointBlockerTrigger")
							{
								base.Add(new CheckpointBlockerTrigger(entityData2, vector));
							}
						}
						else if (num2 != 2130858406U)
						{
							if (num2 == 2175028017U)
							{
								if (name == "interactTrigger")
								{
									base.Add(new InteractTrigger(entityData2, vector));
								}
							}
						}
						else if (name == "minitextboxTrigger")
						{
							base.Add(new MiniTextboxTrigger(entityData2, vector, entityID3));
						}
					}
					else if (num2 <= 2694917423U)
					{
						if (num2 <= 2256371320U)
						{
							if (num2 != 2212054090U)
							{
								if (num2 != 2219223802U)
								{
									if (num2 == 2256371320U)
									{
										if (name == "rumbleTrigger")
										{
											base.Add(new RumbleTrigger(entityData2, vector, entityID3));
										}
									}
								}
								else if (name == "bloomFadeTrigger")
								{
									base.Add(new BloomFadeTrigger(entityData2, vector));
								}
							}
							else if (name == "spawnFacingTrigger")
							{
								base.Add(new SpawnFacingTrigger(entityData2, vector));
							}
						}
						else if (num2 <= 2546582034U)
						{
							if (num2 != 2354822499U)
							{
								if (num2 == 2546582034U)
								{
									if (name == "musicFadeTrigger")
									{
										base.Add(new MusicFadeTrigger(entityData2, vector));
									}
								}
							}
							else if (name == "detachFollowersTrigger")
							{
								base.Add(new DetachStrawberryTrigger(entityData2, vector));
							}
						}
						else if (num2 != 2550153456U)
						{
							if (num2 == 2694917423U)
							{
								if (name == "cameraAdvanceTargetTrigger")
								{
									base.Add(new CameraAdvanceTargetTrigger(entityData2, vector));
								}
							}
						}
						else if (name == "stopBoostTrigger")
						{
							base.Add(new StopBoostTrigger(entityData2, vector));
						}
					}
					else if (num2 <= 3801863475U)
					{
						if (num2 <= 3070454271U)
						{
							if (num2 != 3019691746U)
							{
								if (num2 == 3070454271U)
								{
									if (name == "cameraOffsetTrigger")
									{
										base.Add(new CameraOffsetTrigger(entityData2, vector));
									}
								}
							}
							else if (name == "respawnTargetTrigger")
							{
								base.Add(new RespawnTargetTrigger(entityData2, vector));
							}
						}
						else if (num2 != 3118307023U)
						{
							if (num2 == 3801863475U)
							{
								if (name == "creditsTrigger")
								{
									base.Add(new CreditsTrigger(entityData2, vector));
								}
							}
						}
						else if (name == "blackholeStrength")
						{
							base.Add(new BlackholeStrengthTrigger(entityData2, vector));
						}
					}
					else if (num2 <= 4093214961U)
					{
						if (num2 != 3860539099U)
						{
							if (num2 == 4093214961U)
							{
								if (name == "windAttackTrigger")
								{
									base.Add(new WindAttackTrigger(entityData2, vector));
								}
							}
						}
						else if (name == "moonGlitchBackgroundTrigger")
						{
							base.Add(new MoonGlitchBackgroundTrigger(entityData2, vector));
						}
					}
					else if (num2 != 4277659601U)
					{
						if (num2 == 4288062401U)
						{
							if (name == "eventTrigger")
							{
								base.Add(new EventTrigger(entityData2, vector));
							}
						}
					}
					else if (name == "oshiroTrigger")
					{
						base.Add(new OshiroTrigger(entityData2, vector));
					}
				}
			}
			foreach (DecalData decalData in levelData.FgDecals)
			{
				base.Add(new Decal(decalData.Texture, vector + decalData.Position, decalData.Scale, -10500));
			}
			foreach (DecalData decalData2 in levelData.BgDecals)
			{
				base.Add(new Decal(decalData2.Texture, vector + decalData2.Position, decalData2.Scale, 9000));
			}
			if (playerIntro != Player.IntroTypes.Transition)
			{
				if (this.Session.JustStarted && !this.Session.StartedFromBeginning && startPosition != null && this.StartPosition == null)
				{
					this.StartPosition = startPosition;
				}
				if (this.Session.RespawnPoint == null)
				{
					if (this.StartPosition != null)
					{
						this.Session.RespawnPoint = new Vector2?(this.GetSpawnPoint(this.StartPosition.Value));
					}
					else
					{
						this.Session.RespawnPoint = new Vector2?(this.DefaultSpawnPoint);
					}
				}
				PlayerSpriteMode spriteMode = this.Session.Inventory.Backpack ? PlayerSpriteMode.Madeline : PlayerSpriteMode.MadelineNoBackpack;
				Player player = new Player(this.Session.RespawnPoint.Value, spriteMode);
				player.IntroType = playerIntro;
				base.Add(player);
				base.Entities.UpdateLists();
				Level.CameraLockModes cameraLockMode = this.CameraLockMode;
				this.CameraLockMode = Level.CameraLockModes.None;
				this.Camera.Position = this.GetFullCameraTargetAt(player, player.Position);
				this.CameraLockMode = cameraLockMode;
				this.CameraUpwardMaxY = this.Camera.Y + 180f;
				foreach (EntityID id2 in this.Session.Keys)
				{
					base.Add(new Key(player, id2));
				}
				SpotlightWipe.FocusPoint = this.Session.RespawnPoint.Value - this.Camera.Position;
				if (playerIntro != Player.IntroTypes.Respawn && playerIntro != Player.IntroTypes.Fall)
				{
					new SpotlightWipe(this, true, null);
				}
				else
				{
					this.DoScreenWipe(true, null, false);
				}
				if (isFromLoader)
				{
					base.RendererList.UpdateLists();
				}
				if (this.DarkRoom)
				{
					this.Lighting.Alpha = this.Session.DarkRoomAlpha;
				}
				else
				{
					this.Lighting.Alpha = this.BaseLightingAlpha + this.Session.LightingAlphaAdd;
				}
				this.Bloom.Base = AreaData.Get(this.Session).BloomBase + this.Session.BloomBaseAdd;
			}
			else
			{
				base.Entities.UpdateLists();
			}
			if (this.HasCassetteBlocks && this.ShouldCreateCassetteManager)
			{
				CassetteBlockManager entity2 = base.Tracker.GetEntity<CassetteBlockManager>();
				if (entity2 != null)
				{
					entity2.OnLevelStart();
				}
			}
			if (!string.IsNullOrEmpty(levelData.ObjTiles))
			{
				Tileset tileset = new Tileset(GFX.Game["tilesets/scenery"], 8, 8);
				int[,] array2 = Calc.ReadCSVIntGrid(levelData.ObjTiles, this.Bounds.Width / 8, this.Bounds.Height / 8);
				for (int k = 0; k < array2.GetLength(0); k++)
				{
					for (int l = 0; l < array2.GetLength(1); l++)
					{
						if (array2[k, l] != -1)
						{
							TileInterceptor.TileCheck(this, tileset[array2[k, l]], new Vector2((float)(k * 8), (float)(l * 8)) + this.LevelOffset);
						}
					}
				}
			}
			LightningRenderer entity3 = base.Tracker.GetEntity<LightningRenderer>();
			if (entity3 != null)
			{
				if (flag2)
				{
					entity3.StartAmbience();
				}
				else
				{
					entity3.StopAmbience();
				}
			}
			Calc.PopRandom();
		}

		// Token: 0x06001BCD RID: 7117 RVA: 0x000BB738 File Offset: 0x000B9938
		public void UnloadLevel()
		{
			List<Entity> entitiesExcludingTagMask = base.GetEntitiesExcludingTagMask(Tags.Global);
			foreach (Entity item in base.Tracker.GetEntities<Textbox>())
			{
				entitiesExcludingTagMask.Add(item);
			}
			this.UnloadEntities(entitiesExcludingTagMask);
			base.Entities.UpdateLists();
		}

		// Token: 0x06001BCE RID: 7118 RVA: 0x000BB7B4 File Offset: 0x000B99B4
		public void Reload()
		{
			if (!this.Completed)
			{
				if (this.Session.FirstLevel && this.Session.Strawberries.Count <= 0 && !this.Session.Cassette && !this.Session.HeartGem && !this.Session.HitCheckpoint)
				{
					this.Session.Time = 0L;
					this.Session.Deaths = 0;
					this.TimerStarted = false;
				}
				this.Session.Dashes = this.Session.DashesAtLevelStart;
				Glitch.Value = 0f;
				Engine.TimeRate = 1f;
				Distort.Anxiety = 0f;
				Distort.GameRate = 1f;
				Audio.SetMusicParam("fade", 1f);
				this.ParticlesBG.Clear();
				this.Particles.Clear();
				this.ParticlesFG.Clear();
				TrailManager.Clear();
				this.UnloadLevel();
				GC.Collect();
				GC.WaitForPendingFinalizers();
				this.LoadLevel(Player.IntroTypes.Respawn, false);
				this.strawberriesDisplay.DrawLerp = 0f;
				WindController windController = base.Entities.FindFirst<WindController>();
				if (windController != null)
				{
					windController.SnapWind();
					return;
				}
				this.Wind = Vector2.Zero;
			}
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06001BCF RID: 7119 RVA: 0x000BB8EF File Offset: 0x000B9AEF
		private bool ShouldCreateCassetteManager
		{
			get
			{
				return this.Session.Area.Mode != AreaMode.Normal || !this.Session.Cassette;
			}
		}

		// Token: 0x06001BD0 RID: 7120 RVA: 0x000BB914 File Offset: 0x000B9B14
		private bool GotCollectables(EntityData e)
		{
			bool flag = true;
			bool flag2 = true;
			List<EntityID> list = new List<EntityID>();
			if (e.Attr("strawberries", "").Length > 0)
			{
				foreach (string text in e.Attr("strawberries", "").Split(new char[]
				{
					','
				}))
				{
					EntityID none = EntityID.None;
					string[] array2 = text.Split(new char[]
					{
						':'
					});
					none.Level = array2[0];
					none.ID = Convert.ToInt32(array2[1]);
					list.Add(none);
				}
			}
			foreach (EntityID item in list)
			{
				if (!this.Session.Strawberries.Contains(item))
				{
					flag = false;
					break;
				}
			}
			List<EntityID> list2 = new List<EntityID>();
			if (e.Attr("keys", "").Length > 0)
			{
				foreach (string text2 in e.Attr("keys", "").Split(new char[]
				{
					','
				}))
				{
					EntityID none2 = EntityID.None;
					string[] array3 = text2.Split(new char[]
					{
						':'
					});
					none2.Level = array3[0];
					none2.ID = Convert.ToInt32(array3[1]);
					list2.Add(none2);
				}
			}
			foreach (EntityID item2 in list2)
			{
				if (!this.Session.DoNotLoad.Contains(item2))
				{
					flag2 = false;
					break;
				}
			}
			return flag2 && flag;
		}

		// Token: 0x06001BD1 RID: 7121 RVA: 0x000BBAF4 File Offset: 0x000B9CF4
		public void TransitionTo(LevelData next, Vector2 direction)
		{
			this.Session.CoreMode = this.CoreMode;
			this.transition = new Coroutine(this.TransitionRoutine(next, direction), true);
		}

		// Token: 0x06001BD2 RID: 7122 RVA: 0x000BBB1B File Offset: 0x000B9D1B
		private IEnumerator TransitionRoutine(LevelData next, Vector2 direction)
		{
			Player player = base.Tracker.GetEntity<Player>();
			List<Entity> toRemove = base.GetEntitiesExcludingTagMask(Tags.Persistent | Tags.Global);
			List<Component> transitionOut = base.Tracker.GetComponentsCopy<TransitionListener>();
			player.CleanUpTriggers();
			foreach (Component component in base.Tracker.GetComponents<SoundSource>())
			{
				SoundSource soundSource = (SoundSource)component;
				if (soundSource.DisposeOnTransition)
				{
					soundSource.Stop(true);
				}
			}
			this.PreviousBounds = new Rectangle?(this.Bounds);
			this.Session.Level = next.Name;
			this.Session.FirstLevel = false;
			this.Session.DeathsInCurrentLevel = 0;
			this.LoadLevel(Player.IntroTypes.Transition, false);
			Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "has_conveyors", (float)((base.Tracker.GetEntities<WallBooster>().Count > 0) ? 1 : 0));
			List<Component> transitionIn = base.Tracker.GetComponentsCopy<TransitionListener>();
			transitionIn.RemoveAll((Component c) => transitionOut.Contains(c));
			GC.Collect();
			float cameraAt = 0f;
			Vector2 cameraFrom = this.Camera.Position;
			Vector2 dirPad = direction * 4f;
			if (direction == Vector2.UnitY)
			{
				dirPad = direction * 12f;
			}
			Vector2 playerTo = player.Position;
			while (direction.X != 0f)
			{
				if (playerTo.Y < (float)this.Bounds.Bottom)
				{
					break;
				}
				playerTo.Y -= 1f;
			}
			while (!this.IsInBounds(playerTo, dirPad))
			{
				playerTo += direction;
			}
			Vector2 cameraTo = this.GetFullCameraTargetAt(player, playerTo);
			Vector2 position = player.Position;
			player.Position = playerTo;
			foreach (Entity entity in player.CollideAll<WindTrigger>())
			{
				if (!toRemove.Contains(entity))
				{
					this.windController.SetPattern((entity as WindTrigger).Pattern);
					break;
				}
			}
			this.windController.SetStartPattern();
			player.Position = position;
			foreach (Component component2 in transitionOut)
			{
				TransitionListener transitionListener = (TransitionListener)component2;
				if (transitionListener.OnOutBegin != null)
				{
					transitionListener.OnOutBegin();
				}
			}
			foreach (Component component3 in transitionIn)
			{
				TransitionListener transitionListener2 = (TransitionListener)component3;
				if (transitionListener2.OnInBegin != null)
				{
					transitionListener2.OnInBegin();
				}
			}
			float lightingStart = this.Lighting.Alpha;
			float lightingEnd = this.DarkRoom ? this.Session.DarkRoomAlpha : (this.BaseLightingAlpha + this.Session.LightingAlphaAdd);
			bool lightingWait = lightingStart >= this.Session.DarkRoomAlpha || lightingEnd >= this.Session.DarkRoomAlpha;
			if (lightingEnd > lightingStart && lightingWait)
			{
				Audio.Play("event:/game/05_mirror_temple/room_lightlevel_down");
				while (this.Lighting.Alpha != lightingEnd)
				{
					yield return null;
					this.Lighting.Alpha = Calc.Approach(this.Lighting.Alpha, lightingEnd, 2f * Engine.DeltaTime);
				}
			}
			bool cameraFinished = false;
			while (!player.TransitionTo(playerTo, direction) || cameraAt < 1f)
			{
				yield return null;
				if (!cameraFinished)
				{
					cameraAt = Calc.Approach(cameraAt, 1f, Engine.DeltaTime / this.NextTransitionDuration);
					if (cameraAt > 0.9f)
					{
						this.Camera.Position = cameraTo;
					}
					else
					{
						this.Camera.Position = Vector2.Lerp(cameraFrom, cameraTo, Ease.CubeOut(cameraAt));
					}
					if (!lightingWait && lightingStart < lightingEnd)
					{
						this.Lighting.Alpha = lightingStart + (lightingEnd - lightingStart) * cameraAt;
					}
					foreach (Component component4 in transitionOut)
					{
						TransitionListener transitionListener3 = (TransitionListener)component4;
						if (transitionListener3.OnOut != null)
						{
							transitionListener3.OnOut(cameraAt);
						}
					}
					foreach (Component component5 in transitionIn)
					{
						TransitionListener transitionListener4 = (TransitionListener)component5;
						if (transitionListener4.OnIn != null)
						{
							transitionListener4.OnIn(cameraAt);
						}
					}
					if (cameraAt >= 1f)
					{
						cameraFinished = true;
					}
				}
			}
			if (lightingEnd < lightingStart && lightingWait)
			{
				Audio.Play("event:/game/05_mirror_temple/room_lightlevel_up");
				while (this.Lighting.Alpha != lightingEnd)
				{
					yield return null;
					this.Lighting.Alpha = Calc.Approach(this.Lighting.Alpha, lightingEnd, 2f * Engine.DeltaTime);
				}
			}
			this.UnloadEntities(toRemove);
			base.Entities.UpdateLists();
			Rectangle bounds = this.Bounds;
			bounds.Inflate(16, 16);
			this.Particles.ClearRect(bounds, false);
			this.ParticlesBG.ClearRect(bounds, false);
			this.ParticlesFG.ClearRect(bounds, false);
			RespawnTargetTrigger respawnTargetTrigger = player.CollideFirst<RespawnTargetTrigger>();
			Vector2 to;
			if (respawnTargetTrigger == null)
			{
				to = player.Position;
			}
			else
			{
				to = respawnTargetTrigger.Target;
			}
			this.Session.RespawnPoint = new Vector2?(this.Session.LevelData.Spawns.ClosestTo(to));
			player.OnTransition();
			foreach (Component component6 in transitionIn)
			{
				TransitionListener transitionListener5 = (TransitionListener)component6;
				if (transitionListener5.OnInEnd != null)
				{
					transitionListener5.OnInEnd();
				}
			}
			if (this.Session.LevelData.DelayAltMusic)
			{
				Audio.SetAltMusic(SFX.EventnameByHandle(this.Session.LevelData.AltMusic));
			}
			cameraFrom = default(Vector2);
			playerTo = default(Vector2);
			cameraTo = default(Vector2);
			this.NextTransitionDuration = 0.65f;
			this.transition = null;
			yield break;
		}

		// Token: 0x06001BD3 RID: 7123 RVA: 0x000BBB38 File Offset: 0x000B9D38
		public void UnloadEntities(List<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				base.Remove(entity);
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06001BD4 RID: 7124 RVA: 0x000BBB88 File Offset: 0x000B9D88
		public Vector2 DefaultSpawnPoint
		{
			get
			{
				return this.GetSpawnPoint(new Vector2((float)this.Bounds.Left, (float)this.Bounds.Bottom));
			}
		}

		// Token: 0x06001BD5 RID: 7125 RVA: 0x000BBBBE File Offset: 0x000B9DBE
		public Vector2 GetSpawnPoint(Vector2 from)
		{
			return this.Session.GetSpawnPoint(from);
		}

		// Token: 0x06001BD6 RID: 7126 RVA: 0x000BBBCC File Offset: 0x000B9DCC
		public Vector2 GetFullCameraTargetAt(Player player, Vector2 at)
		{
			Vector2 position = player.Position;
			player.Position = at;
			foreach (Entity entity in base.Tracker.GetEntities<Trigger>())
			{
				if (entity is CameraTargetTrigger && player.CollideCheck(entity))
				{
					(entity as CameraTargetTrigger).OnStay(player);
				}
				else if (entity is CameraOffsetTrigger && player.CollideCheck(entity))
				{
					(entity as CameraOffsetTrigger).OnEnter(player);
				}
			}
			Vector2 cameraTarget = player.CameraTarget;
			player.Position = position;
			return cameraTarget;
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06001BD7 RID: 7127 RVA: 0x000BBC74 File Offset: 0x000B9E74
		public Rectangle Bounds
		{
			get
			{
				return this.Session.LevelData.Bounds;
			}
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06001BD8 RID: 7128 RVA: 0x000BBC86 File Offset: 0x000B9E86
		// (set) Token: 0x06001BD9 RID: 7129 RVA: 0x000BBC8E File Offset: 0x000B9E8E
		public Rectangle? PreviousBounds { get; private set; }

		// Token: 0x06001BDA RID: 7130 RVA: 0x000BBC98 File Offset: 0x000B9E98
		public void TeleportTo(Player player, string nextLevel, Player.IntroTypes introType, Vector2? nearestSpawn = null)
		{
			Leader.StoreStrawberries(player.Leader);
			Vector2 position = player.Position;
			base.Remove(player);
			this.UnloadLevel();
			this.Session.Level = nextLevel;
			this.Session.RespawnPoint = new Vector2?(this.GetSpawnPoint(new Vector2((float)this.Bounds.Left, (float)this.Bounds.Top) + ((nearestSpawn != null) ? nearestSpawn.Value : Vector2.Zero)));
			if (introType == Player.IntroTypes.Transition)
			{
				player.Position = this.Session.RespawnPoint.Value;
				player.Hair.MoveHairBy(player.Position - position);
				player.MuffleLanding = true;
				base.Add(player);
				this.LoadLevel(Player.IntroTypes.Transition, false);
				base.Entities.UpdateLists();
			}
			else
			{
				this.LoadLevel(introType, false);
				base.Entities.UpdateLists();
				player = base.Tracker.GetEntity<Player>();
			}
			this.Camera.Position = player.CameraTarget;
			this.Update();
			Leader.RestoreStrawberries(player.Leader);
		}

		// Token: 0x06001BDB RID: 7131 RVA: 0x000BBDBA File Offset: 0x000B9FBA
		public void AutoSave()
		{
			if (this.saving == null)
			{
				this.saving = new Coroutine(this.SavingRoutine(), true);
			}
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x000BBDD6 File Offset: 0x000B9FD6
		public bool IsAutoSaving()
		{
			return this.saving != null;
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x000BBDE1 File Offset: 0x000B9FE1
		private IEnumerator SavingRoutine()
		{
			UserIO.SaveHandler(true, false);
			while (UserIO.Saving)
			{
				yield return null;
			}
			this.saving = null;
			yield break;
		}

		// Token: 0x06001BDE RID: 7134 RVA: 0x000BBDF0 File Offset: 0x000B9FF0
		public void UpdateTime()
		{
			if (!this.InCredits && this.Session.Area.ID != 8 && !this.TimerStopped)
			{
				long ticks = TimeSpan.FromSeconds((double)Engine.RawDeltaTime).Ticks;
				SaveData.Instance.AddTime(this.Session.Area, ticks);
				if (!this.TimerStarted && !this.InCutscene)
				{
					Player entity = base.Tracker.GetEntity<Player>();
					if (entity != null && !entity.TimePaused)
					{
						this.TimerStarted = true;
					}
				}
				if (!this.Completed && this.TimerStarted)
				{
					this.Session.Time += ticks;
				}
			}
		}

		// Token: 0x06001BDF RID: 7135 RVA: 0x000BBEA4 File Offset: 0x000BA0A4
		public override void Update()
		{
			if (this.unpauseTimer > 0f)
			{
				this.unpauseTimer -= Engine.RawDeltaTime;
				this.UpdateTime();
				return;
			}
			if (this.Overlay != null)
			{
				this.Overlay.Update();
				base.Entities.UpdateLists();
				return;
			}
			int num = 10;
			if (!this.InCutscene && base.Tracker.GetEntity<Player>() != null && this.Wipe == null && !this.Frozen)
			{
				num = SaveData.Instance.Assists.GameSpeed;
			}
			Engine.TimeRateB = (float)num / 10f;
			if (num != 10)
			{
				if (Level.AssistSpeedSnapshot == null || Level.AssistSpeedSnapshotValue != num)
				{
					Audio.ReleaseSnapshot(Level.AssistSpeedSnapshot);
					Level.AssistSpeedSnapshot = null;
					Level.AssistSpeedSnapshotValue = num;
					if (Level.AssistSpeedSnapshotValue < 10)
					{
						Level.AssistSpeedSnapshot = Audio.CreateSnapshot("snapshot:/assist_game_speed/assist_speed_" + Level.AssistSpeedSnapshotValue * 10, true);
					}
					else if (Level.AssistSpeedSnapshotValue <= 16)
					{
						Level.AssistSpeedSnapshot = Audio.CreateSnapshot("snapshot:/variant_speed/variant_speed_" + Level.AssistSpeedSnapshotValue * 10, true);
					}
				}
			}
			else if (Level.AssistSpeedSnapshot != null)
			{
				Audio.ReleaseSnapshot(Level.AssistSpeedSnapshot);
				Level.AssistSpeedSnapshot = null;
				Level.AssistSpeedSnapshotValue = -1;
			}
			if (this.wasPaused && !this.Paused)
			{
				this.EndPauseEffects();
			}
			if (this.CanPause && Input.QuickRestart.Pressed)
			{
				Input.QuickRestart.ConsumeBuffer();
				this.Pause(0, false, true);
			}
			else if (this.CanPause && (Input.Pause.Pressed || Input.ESC.Pressed))
			{
				Input.Pause.ConsumeBuffer();
				Input.ESC.ConsumeBuffer();
				this.Pause(0, false, false);
			}
			if (this.wasPaused && !this.Paused)
			{
				this.wasPaused = false;
			}
			if (this.Paused)
			{
				this.wasPausedTimer = 0f;
			}
			else
			{
				this.wasPausedTimer += Engine.DeltaTime;
			}
			this.UpdateTime();
			if (this.saving != null)
			{
				this.saving.Update();
			}
			if (!this.Paused)
			{
				this.glitchTimer += Engine.DeltaTime;
				this.glitchSeed = Calc.Random.NextFloat();
			}
			if (this.SkippingCutscene)
			{
				if (this.skipCoroutine != null)
				{
					this.skipCoroutine.Update();
				}
				base.RendererList.Update();
			}
			else if (this.FrozenOrPaused)
			{
				bool disabled = MInput.Disabled;
				MInput.Disabled = false;
				if (!this.Paused)
				{
					foreach (Entity entity in base[Tags.FrozenUpdate])
					{
						if (entity.Active)
						{
							entity.Update();
						}
					}
				}
				foreach (Entity entity2 in base[Tags.PauseUpdate])
				{
					if (entity2.Active)
					{
						entity2.Update();
					}
				}
				MInput.Disabled = disabled;
				if (this.Wipe != null)
				{
					this.Wipe.Update(this);
				}
				if (this.HiresSnow != null)
				{
					this.HiresSnow.Update(this);
				}
				base.Entities.UpdateLists();
			}
			else
			{
				if (!this.Transitioning)
				{
					if (this.RetryPlayerCorpse == null)
					{
						base.Update();
						goto IL_40D;
					}
					this.RetryPlayerCorpse.Update();
					base.RendererList.Update();
					using (List<Entity>.Enumerator enumerator = base[Tags.PauseUpdate].GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Entity entity3 = enumerator.Current;
							if (entity3.Active)
							{
								entity3.Update();
							}
						}
						goto IL_40D;
					}
				}
				foreach (Entity entity4 in base[Tags.TransitionUpdate])
				{
					entity4.Update();
				}
				this.transition.Update();
				base.RendererList.Update();
			}
			IL_40D:
			this.HudRenderer.BackgroundFade = Calc.Approach(this.HudRenderer.BackgroundFade, this.Paused ? 1f : 0f, 8f * Engine.RawDeltaTime);
			if (!this.FrozenOrPaused)
			{
				this.WindSineTimer += Engine.DeltaTime;
				this.WindSine = (float)(Math.Sin((double)this.WindSineTimer) + 1.0) / 2f;
			}
			foreach (Component component in base.Tracker.GetComponents<PostUpdateHook>())
			{
				PostUpdateHook postUpdateHook = (PostUpdateHook)component;
				if (postUpdateHook.Entity.Active)
				{
					postUpdateHook.OnPostUpdate();
				}
			}
			if (this.updateHair)
			{
				foreach (Component component2 in base.Tracker.GetComponents<PlayerHair>())
				{
					if (component2.Active && component2.Entity.Active)
					{
						(component2 as PlayerHair).AfterUpdate();
					}
				}
				if (this.FrozenOrPaused)
				{
					this.updateHair = false;
				}
			}
			else if (!this.FrozenOrPaused)
			{
				this.updateHair = true;
			}
			if (this.shakeTimer > 0f)
			{
				if (base.OnRawInterval(0.04f))
				{
					int num2 = (int)Math.Ceiling((double)(this.shakeTimer * 10f));
					if (this.shakeDirection == Vector2.Zero)
					{
						this.ShakeVector = new Vector2((float)(-(float)num2 + Calc.Random.Next(num2 * 2 + 1)), (float)(-(float)num2 + Calc.Random.Next(num2 * 2 + 1)));
					}
					else
					{
						if (this.lastDirectionalShake == 0)
						{
							this.lastDirectionalShake = 1;
						}
						else
						{
							this.lastDirectionalShake *= -1;
						}
						this.ShakeVector = -this.shakeDirection * (float)this.lastDirectionalShake * (float)num2;
					}
					if (Settings.Instance.ScreenShake == ScreenshakeAmount.Half)
					{
						float x = (float)Math.Sign(this.ShakeVector.X);
						float y = (float)Math.Sign(this.ShakeVector.Y);
						this.ShakeVector = new Vector2(x, y);
					}
				}
				float num3 = (Settings.Instance.ScreenShake == ScreenshakeAmount.Half) ? 1.5f : 1f;
				this.shakeTimer -= Engine.RawDeltaTime * num3;
			}
			else
			{
				this.ShakeVector = Vector2.Zero;
			}
			if (this.doFlash)
			{
				this.flash = Calc.Approach(this.flash, 1f, Engine.DeltaTime * 10f);
				if (this.flash >= 1f)
				{
					this.doFlash = false;
				}
			}
			else if (this.flash > 0f)
			{
				this.flash = Calc.Approach(this.flash, 0f, Engine.DeltaTime * 3f);
			}
			if (this.lastColorGrade != this.Session.ColorGrade)
			{
				if (this.colorGradeEase >= 1f)
				{
					this.colorGradeEase = 0f;
					this.lastColorGrade = this.Session.ColorGrade;
				}
				else
				{
					this.colorGradeEase = Calc.Approach(this.colorGradeEase, 1f, Engine.DeltaTime * this.colorGradeEaseSpeed);
				}
			}
			if (Celeste.PlayMode == Celeste.PlayModes.Debug)
			{
				if (MInput.Keyboard.Pressed(Keys.Tab) && Engine.Scene.Tracker.GetEntity<KeyboardConfigUI>() == null && Engine.Scene.Tracker.GetEntity<ButtonConfigUI>() == null)
				{
					Engine.Scene = new MapEditor(this.Session.Area, true);
				}
				if (MInput.Keyboard.Pressed(Keys.F1))
				{
					Celeste.ReloadAssets(true, false, false, new AreaKey?(this.Session.Area));
					Engine.Scene = new LevelLoader(this.Session, null);
					return;
				}
				if (MInput.Keyboard.Pressed(Keys.F2))
				{
					Celeste.ReloadAssets(true, true, false, new AreaKey?(this.Session.Area));
					Engine.Scene = new LevelLoader(this.Session, null);
					return;
				}
				if (MInput.Keyboard.Pressed(Keys.F3))
				{
					Celeste.ReloadAssets(true, true, true, new AreaKey?(this.Session.Area));
					Engine.Scene = new LevelLoader(this.Session, null);
				}
			}
		}

		// Token: 0x06001BE0 RID: 7136 RVA: 0x000BC780 File Offset: 0x000BA980
		public override void BeforeRender()
		{
			this.cameraPreShake = this.Camera.Position;
			this.Camera.Position += this.ShakeVector;
			this.Camera.Position = this.Camera.Position.Floor();
			foreach (Component component in base.Tracker.GetComponents<BeforeRenderHook>())
			{
				BeforeRenderHook beforeRenderHook = (BeforeRenderHook)component;
				if (beforeRenderHook.Visible)
				{
					beforeRenderHook.Callback();
				}
			}
			SpeedRing.DrawToBuffer(this);
			base.BeforeRender();
		}

		// Token: 0x06001BE1 RID: 7137 RVA: 0x000BC840 File Offset: 0x000BAA40
		public override void Render()
		{
			Engine.Instance.GraphicsDevice.SetRenderTarget(GameplayBuffers.Gameplay);
			Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
			this.GameplayRenderer.Render(this);
			this.Lighting.Render(this);
			Engine.Instance.GraphicsDevice.SetRenderTarget(GameplayBuffers.Level);
			Engine.Instance.GraphicsDevice.Clear(this.BackgroundColor);
			this.Background.Render(this);
			Distort.Render(GameplayBuffers.Gameplay, GameplayBuffers.Displacement, this.Displacement.HasDisplacement(this));
			this.Bloom.Apply(GameplayBuffers.Level, this);
			this.Foreground.Render(this);
			Glitch.Apply(GameplayBuffers.Level, this.glitchTimer * 2f, this.glitchSeed, 6.2831855f);
			if (Engine.DashAssistFreeze)
			{
				PlayerDashAssist entity = base.Tracker.GetEntity<PlayerDashAssist>();
				if (entity != null)
				{
					Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, this.Camera.Matrix);
					entity.Render();
					Draw.SpriteBatch.End();
				}
			}
			if (this.flash > 0f)
			{
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
				Draw.Rect(-1f, -1f, 322f, 182f, this.flashColor * this.flash);
				Draw.SpriteBatch.End();
				if (this.flashDrawPlayer)
				{
					Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, this.Camera.Matrix);
					Player entity2 = base.Tracker.GetEntity<Player>();
					if (entity2 != null && entity2.Visible)
					{
						entity2.Render();
					}
					Draw.SpriteBatch.End();
				}
			}
			Engine.Instance.GraphicsDevice.SetRenderTarget(null);
			Engine.Instance.GraphicsDevice.Clear(Color.Black);
			Engine.Instance.GraphicsDevice.Viewport = Engine.Viewport;
			Matrix matrix = Matrix.CreateScale(6f) * Engine.ScreenMatrix;
			Vector2 vector = new Vector2(320f, 180f);
			Vector2 vector2 = vector / this.ZoomTarget;
			Vector2 vector3 = (this.ZoomTarget != 1f) ? ((this.ZoomFocusPoint - vector2 / 2f) / (vector - vector2) * vector) : Vector2.Zero;
			MTexture orDefault = GFX.ColorGrades.GetOrDefault(this.lastColorGrade, GFX.ColorGrades["none"]);
			MTexture orDefault2 = GFX.ColorGrades.GetOrDefault(this.Session.ColorGrade, GFX.ColorGrades["none"]);
			if (this.colorGradeEase > 0f && orDefault != orDefault2)
			{
				ColorGrade.Set(orDefault, orDefault2, this.colorGradeEase);
			}
			else
			{
				ColorGrade.Set(orDefault2);
			}
			float scale = this.Zoom * ((320f - this.ScreenPadding * 2f) / 320f);
			Vector2 vector4 = new Vector2(this.ScreenPadding, this.ScreenPadding * 0.5625f);
			if (SaveData.Instance.Assists.MirrorMode)
			{
				vector4.X = -vector4.X;
				vector3.X = 160f - (vector3.X - 160f);
			}
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, ColorGrade.Effect, matrix);
			Draw.SpriteBatch.Draw(GameplayBuffers.Level, vector3 + vector4, new Rectangle?(GameplayBuffers.Level.Bounds), Color.White, 0f, vector3, scale, SaveData.Instance.Assists.MirrorMode ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
			Draw.SpriteBatch.End();
			if (this.Pathfinder != null && this.Pathfinder.DebugRenderEnabled)
			{
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, this.Camera.Matrix * matrix);
				this.Pathfinder.Render();
				Draw.SpriteBatch.End();
			}
			if (((!this.Paused || !this.PauseMainMenuOpen) && this.wasPausedTimer >= 1f) || !Input.MenuJournal.Check || !this.AllowHudHide)
			{
				this.HudRenderer.Render(this);
			}
			if (this.Wipe != null)
			{
				this.Wipe.Render(this);
			}
			if (this.HiresSnow != null)
			{
				this.HiresSnow.Render(this);
			}
		}

		// Token: 0x06001BE2 RID: 7138 RVA: 0x000BCD13 File Offset: 0x000BAF13
		public override void AfterRender()
		{
			base.AfterRender();
			this.Camera.Position = this.cameraPreShake;
		}

		// Token: 0x06001BE3 RID: 7139 RVA: 0x000BCD2C File Offset: 0x000BAF2C
		private void StartPauseEffects()
		{
			if (Audio.CurrentMusic == "event:/music/lvl0/bridge")
			{
				Audio.PauseMusic = true;
			}
			Audio.PauseGameplaySfx = true;
			Audio.Play("event:/ui/game/pause");
			if (Level.PauseSnapshot == null)
			{
				Level.PauseSnapshot = Audio.CreateSnapshot("snapshot:/pause_menu", true);
			}
		}

		// Token: 0x06001BE4 RID: 7140 RVA: 0x000BCD7E File Offset: 0x000BAF7E
		private void EndPauseEffects()
		{
			Audio.PauseMusic = false;
			Audio.PauseGameplaySfx = false;
			Audio.ReleaseSnapshot(Level.PauseSnapshot);
			Level.PauseSnapshot = null;
		}

		// Token: 0x06001BE5 RID: 7141 RVA: 0x000BCD9C File Offset: 0x000BAF9C
		public void Pause(int startIndex = 0, bool minimal = false, bool quickReset = false)
		{
			Level.<>c__DisplayClass149_0 CS$<>8__locals1 = new Level.<>c__DisplayClass149_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.minimal = minimal;
			this.wasPaused = true;
			CS$<>8__locals1.player = base.Tracker.GetEntity<Player>();
			if (!this.Paused)
			{
				this.StartPauseEffects();
			}
			this.Paused = true;
			if (quickReset)
			{
				Audio.Play("event:/ui/main/message_confirm");
				this.PauseMainMenuOpen = false;
				this.GiveUp(0, true, CS$<>8__locals1.minimal, false);
				return;
			}
			this.PauseMainMenuOpen = true;
			CS$<>8__locals1.menu = new TextMenu();
			if (!CS$<>8__locals1.minimal)
			{
				CS$<>8__locals1.menu.Add(new TextMenu.Header(Dialog.Clean("menu_pause_title", null)));
			}
			CS$<>8__locals1.menu.Add(new TextMenu.Button(Dialog.Clean("menu_pause_resume", null)).Pressed(delegate
			{
				CS$<>8__locals1.menu.OnCancel();
			}));
			if (this.InCutscene && !this.SkippingCutscene)
			{
				CS$<>8__locals1.menu.Add(new TextMenu.Button(Dialog.Clean("menu_pause_skip_cutscene", null)).Pressed(delegate
				{
					CS$<>8__locals1.<>4__this.SkipCutscene();
					CS$<>8__locals1.<>4__this.Paused = false;
					CS$<>8__locals1.<>4__this.PauseMainMenuOpen = false;
					CS$<>8__locals1.menu.RemoveSelf();
				}));
			}
			if (!CS$<>8__locals1.minimal && !this.InCutscene && !this.SkippingCutscene)
			{
				TextMenu.Item item3;
				CS$<>8__locals1.menu.Add(item3 = new TextMenu.Button(Dialog.Clean("menu_pause_retry", null)).Pressed(delegate
				{
					if (CS$<>8__locals1.player != null && !CS$<>8__locals1.player.Dead)
					{
						Engine.TimeRate = 1f;
						Distort.GameRate = 1f;
						Distort.Anxiety = 0f;
						CS$<>8__locals1.<>4__this.InCutscene = (CS$<>8__locals1.<>4__this.SkippingCutscene = false);
						CS$<>8__locals1.<>4__this.RetryPlayerCorpse = CS$<>8__locals1.player.Die(Vector2.Zero, true, true);
						foreach (Component component in CS$<>8__locals1.<>4__this.Tracker.GetComponents<LevelEndingHook>())
						{
							LevelEndingHook levelEndingHook = (LevelEndingHook)component;
							if (levelEndingHook.OnEnd != null)
							{
								levelEndingHook.OnEnd();
							}
						}
					}
					CS$<>8__locals1.<>4__this.Paused = false;
					CS$<>8__locals1.<>4__this.PauseMainMenuOpen = false;
					CS$<>8__locals1.<>4__this.EndPauseEffects();
					CS$<>8__locals1.menu.RemoveSelf();
				}));
				item3.Disabled = (!this.CanRetry || (CS$<>8__locals1.player != null && !CS$<>8__locals1.player.CanRetry) || this.Frozen || this.Completed);
			}
			TextMenu.Item item;
			if (!CS$<>8__locals1.minimal && SaveData.Instance.AssistMode)
			{
				TextMenu.Item item = null;
				CS$<>8__locals1.menu.Add(item = new TextMenu.Button(Dialog.Clean("menu_pause_assist", null)).Pressed(delegate
				{
					CS$<>8__locals1.menu.RemoveSelf();
					CS$<>8__locals1.<>4__this.PauseMainMenuOpen = false;
					CS$<>8__locals1.<>4__this.AssistMode(CS$<>8__locals1.menu.IndexOf(item), CS$<>8__locals1.minimal);
				}));
			}
			if (!CS$<>8__locals1.minimal && SaveData.Instance.VariantMode)
			{
				TextMenu.Item item = null;
				CS$<>8__locals1.menu.Add(item = new TextMenu.Button(Dialog.Clean("menu_pause_variant", null)).Pressed(delegate
				{
					CS$<>8__locals1.menu.RemoveSelf();
					CS$<>8__locals1.<>4__this.PauseMainMenuOpen = false;
					CS$<>8__locals1.<>4__this.VariantMode(CS$<>8__locals1.menu.IndexOf(item), CS$<>8__locals1.minimal);
				}));
			}
			item = null;
			CS$<>8__locals1.menu.Add(item = new TextMenu.Button(Dialog.Clean("menu_pause_options", null)).Pressed(delegate
			{
				CS$<>8__locals1.menu.RemoveSelf();
				CS$<>8__locals1.<>4__this.PauseMainMenuOpen = false;
				CS$<>8__locals1.<>4__this.Options(CS$<>8__locals1.menu.IndexOf(item), CS$<>8__locals1.minimal);
			}));
			if (!CS$<>8__locals1.minimal && Celeste.PlayMode != Celeste.PlayModes.Event)
			{
				TextMenu.Item item2;
				CS$<>8__locals1.menu.Add(item2 = new TextMenu.Button(Dialog.Clean("menu_pause_savequit", null)).Pressed(delegate
				{
					CS$<>8__locals1.menu.Focused = false;
					Engine.TimeRate = 1f;
					Audio.SetMusic(null, true, true);
					Audio.BusStopAll("bus:/gameplay_sfx", true);
					CS$<>8__locals1.<>4__this.Session.InArea = true;
					CS$<>8__locals1.<>4__this.Session.Deaths++;
					CS$<>8__locals1.<>4__this.Session.DeathsInCurrentLevel++;
					SaveData.Instance.AddDeath(CS$<>8__locals1.<>4__this.Session.Area);
					Level <>4__this = CS$<>8__locals1.<>4__this;
					bool wipeIn = false;
					Action onComplete;
					if ((onComplete = CS$<>8__locals1.<>9__9) == null)
					{
						onComplete = (CS$<>8__locals1.<>9__9 = delegate()
						{
							Engine.Scene = new LevelExit(LevelExit.Mode.SaveAndQuit, CS$<>8__locals1.<>4__this.Session, CS$<>8__locals1.<>4__this.HiresSnow);
						});
					}
					<>4__this.DoScreenWipe(wipeIn, onComplete, true);
					foreach (Component component in CS$<>8__locals1.<>4__this.Tracker.GetComponents<LevelEndingHook>())
					{
						LevelEndingHook levelEndingHook = (LevelEndingHook)component;
						if (levelEndingHook.OnEnd != null)
						{
							levelEndingHook.OnEnd();
						}
					}
				}));
				if (this.SaveQuitDisabled || (CS$<>8__locals1.player != null && CS$<>8__locals1.player.StateMachine.State == 18))
				{
					item2.Disabled = true;
				}
			}
			if (!CS$<>8__locals1.minimal)
			{
				CS$<>8__locals1.menu.Add(new TextMenu.SubHeader("", true));
				TextMenu.Item item = null;
				CS$<>8__locals1.menu.Add(item = new TextMenu.Button(Dialog.Clean("menu_pause_restartarea", null)).Pressed(delegate
				{
					CS$<>8__locals1.<>4__this.PauseMainMenuOpen = false;
					CS$<>8__locals1.menu.RemoveSelf();
					CS$<>8__locals1.<>4__this.GiveUp(CS$<>8__locals1.menu.IndexOf(item), true, CS$<>8__locals1.minimal, true);
				}));
				(item as TextMenu.Button).ConfirmSfx = "event:/ui/main/message_confirm";
				if (SaveData.Instance.Areas[0].Modes[0].Completed || SaveData.Instance.DebugMode || SaveData.Instance.CheatMode)
				{
					TextMenu.Item item = null;
					CS$<>8__locals1.menu.Add(item = new TextMenu.Button(Dialog.Clean("menu_pause_return", null)).Pressed(delegate
					{
						CS$<>8__locals1.<>4__this.PauseMainMenuOpen = false;
						CS$<>8__locals1.menu.RemoveSelf();
						CS$<>8__locals1.<>4__this.GiveUp(CS$<>8__locals1.menu.IndexOf(item), false, CS$<>8__locals1.minimal, false);
					}));
					(item as TextMenu.Button).ConfirmSfx = "event:/ui/main/message_confirm";
				}
				if (Celeste.PlayMode == Celeste.PlayModes.Event)
				{
					CS$<>8__locals1.menu.Add(new TextMenu.Button(Dialog.Clean("menu_pause_restartdemo", null)).Pressed(delegate
					{
						CS$<>8__locals1.<>4__this.EndPauseEffects();
						Audio.SetMusic(null, true, true);
						CS$<>8__locals1.menu.Focused = false;
						CS$<>8__locals1.<>4__this.DoScreenWipe(false, delegate
						{
							LevelEnter.Go(new Session(new AreaKey(0, AreaMode.Normal), null, null), false);
						}, false);
					}));
				}
			}
			CS$<>8__locals1.menu.OnESC = (CS$<>8__locals1.menu.OnCancel = (CS$<>8__locals1.menu.OnPause = delegate()
			{
				CS$<>8__locals1.<>4__this.PauseMainMenuOpen = false;
				CS$<>8__locals1.menu.RemoveSelf();
				CS$<>8__locals1.<>4__this.Paused = false;
				Audio.Play("event:/ui/game/unpause");
				CS$<>8__locals1.<>4__this.unpauseTimer = 0.15f;
			}));
			if (startIndex > 0)
			{
				CS$<>8__locals1.menu.Selection = startIndex;
			}
			base.Add(CS$<>8__locals1.menu);
		}

		// Token: 0x06001BE6 RID: 7142 RVA: 0x000BD270 File Offset: 0x000BB470
		private void GiveUp(int returnIndex, bool restartArea, bool minimal, bool showHint)
		{
			this.Paused = true;
			QuickResetHint quickHint = null;
			ReturnMapHint returnHint = null;
			if (!restartArea)
			{
				base.Add(returnHint = new ReturnMapHint());
			}
			TextMenu menu = new TextMenu();
			menu.AutoScroll = false;
			menu.Position = new Vector2((float)Engine.Width / 2f, (float)Engine.Height / 2f - 100f);
			menu.Add(new TextMenu.Header(Dialog.Clean(restartArea ? "menu_restart_title" : "menu_return_title", null)));
			Action <>9__4;
			Action <>9__5;
			menu.Add(new TextMenu.Button(Dialog.Clean(restartArea ? "menu_restart_continue" : "menu_return_continue", null)).Pressed(delegate
			{
				Engine.TimeRate = 1f;
				menu.Focused = false;
				this.Session.InArea = false;
				Audio.SetMusic(null, true, true);
				Audio.BusStopAll("bus:/gameplay_sfx", true);
				if (restartArea)
				{
					Level <>4__this = this;
					bool wipeIn = false;
					Action onComplete;
					if ((onComplete = <>9__4) == null)
					{
						onComplete = (<>9__4 = delegate()
						{
							Engine.Scene = new LevelExit(LevelExit.Mode.Restart, this.Session, null);
						});
					}
					<>4__this.DoScreenWipe(wipeIn, onComplete, false);
				}
				else
				{
					Level <>4__this2 = this;
					bool wipeIn2 = false;
					Action onComplete2;
					if ((onComplete2 = <>9__5) == null)
					{
						onComplete2 = (<>9__5 = delegate()
						{
							Engine.Scene = new LevelExit(LevelExit.Mode.GiveUp, this.Session, this.HiresSnow);
						});
					}
					<>4__this2.DoScreenWipe(wipeIn2, onComplete2, true);
				}
				foreach (Component component in this.Tracker.GetComponents<LevelEndingHook>())
				{
					LevelEndingHook levelEndingHook = (LevelEndingHook)component;
					if (levelEndingHook.OnEnd != null)
					{
						levelEndingHook.OnEnd();
					}
				}
			}));
			menu.Add(new TextMenu.Button(Dialog.Clean(restartArea ? "menu_restart_cancel" : "menu_return_cancel", null)).Pressed(delegate
			{
				menu.OnCancel();
			}));
			menu.OnPause = (menu.OnESC = delegate()
			{
				menu.RemoveSelf();
				if (quickHint != null)
				{
					quickHint.RemoveSelf();
				}
				if (returnHint != null)
				{
					returnHint.RemoveSelf();
				}
				this.Paused = false;
				this.unpauseTimer = 0.15f;
				Audio.Play("event:/ui/game/unpause");
			});
			menu.OnCancel = delegate()
			{
				Audio.Play("event:/ui/main/button_back");
				menu.RemoveSelf();
				if (quickHint != null)
				{
					quickHint.RemoveSelf();
				}
				if (returnHint != null)
				{
					returnHint.RemoveSelf();
				}
				this.Pause(returnIndex, minimal, false);
			};
			base.Add(menu);
		}

		// Token: 0x06001BE7 RID: 7143 RVA: 0x000BD400 File Offset: 0x000BB600
		private void Options(int returnIndex, bool minimal)
		{
			this.Paused = true;
			bool oldAllowHudHide = this.AllowHudHide;
			this.AllowHudHide = false;
			TextMenu options = MenuOptions.Create(true, Level.PauseSnapshot);
			Action <>9__2;
			options.OnESC = (options.OnCancel = delegate()
			{
				Audio.Play("event:/ui/main/button_back");
				this.AllowHudHide = oldAllowHudHide;
				TextMenu options = options;
				IEnumerator routine = this.SaveFromOptions();
				Action onClose;
				if ((onClose = <>9__2) == null)
				{
					onClose = (<>9__2 = delegate()
					{
						this.Pause(returnIndex, minimal, false);
					});
				}
				options.CloseAndRun(routine, onClose);
			});
			Action <>9__3;
			options.OnPause = delegate()
			{
				Audio.Play("event:/ui/main/button_back");
				TextMenu options = options;
				IEnumerator routine = this.SaveFromOptions();
				Action onClose;
				if ((onClose = <>9__3) == null)
				{
					onClose = (<>9__3 = delegate()
					{
						this.AllowHudHide = oldAllowHudHide;
						this.Paused = false;
						this.unpauseTimer = 0.15f;
					});
				}
				options.CloseAndRun(routine, onClose);
			};
			base.Add(options);
		}

		// Token: 0x06001BE8 RID: 7144 RVA: 0x000BD49B File Offset: 0x000BB69B
		private IEnumerator SaveFromOptions()
		{
			UserIO.SaveHandler(false, true);
			while (UserIO.Saving)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001BE9 RID: 7145 RVA: 0x000BD4A4 File Offset: 0x000BB6A4
		private void AssistMode(int returnIndex, bool minimal)
		{
			this.Paused = true;
			TextMenu menu = new TextMenu();
			menu.Add(new TextMenu.Header(Dialog.Clean("MENU_ASSIST_TITLE", null)));
			menu.Add(new TextMenu.Slider(Dialog.Clean("MENU_ASSIST_GAMESPEED", null), (int i) => i * 10 + "%", 5, 10, SaveData.Instance.Assists.GameSpeed).Change(delegate(int i)
			{
				SaveData.Instance.Assists.GameSpeed = i;
				Engine.TimeRateB = (float)SaveData.Instance.Assists.GameSpeed / 10f;
			}));
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_INFINITE_STAMINA", null), SaveData.Instance.Assists.InfiniteStamina).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.InfiniteStamina = on;
			}));
			TextMenu.Option<int> option;
			menu.Add(option = new TextMenu.Slider(Dialog.Clean("MENU_ASSIST_AIR_DASHES", null), delegate(int i)
			{
				if (i == 0)
				{
					return Dialog.Clean("MENU_ASSIST_AIR_DASHES_NORMAL", null);
				}
				if (i == 1)
				{
					return Dialog.Clean("MENU_ASSIST_AIR_DASHES_TWO", null);
				}
				return Dialog.Clean("MENU_ASSIST_AIR_DASHES_INFINITE", null);
			}, 0, 2, (int)SaveData.Instance.Assists.DashMode).Change(delegate(int on)
			{
				SaveData.Instance.Assists.DashMode = (Assists.DashModes)on;
				Player entity = this.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					entity.Dashes = Math.Min(entity.Dashes, entity.MaxDashes);
				}
			}));
			if (this.Session.Area.ID == 0)
			{
				option.Disabled = true;
			}
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_DASH_ASSIST", null), SaveData.Instance.Assists.DashAssist).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.DashAssist = on;
			}));
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_INVINCIBLE", null), SaveData.Instance.Assists.Invincible).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.Invincible = on;
			}));
			menu.OnESC = (menu.OnCancel = delegate()
			{
				Audio.Play("event:/ui/main/button_back");
				this.Pause(returnIndex, minimal, false);
				menu.Close();
			});
			menu.OnPause = delegate()
			{
				Audio.Play("event:/ui/main/button_back");
				this.Paused = false;
				this.unpauseTimer = 0.15f;
				menu.Close();
			};
			base.Add(menu);
		}

		// Token: 0x06001BEA RID: 7146 RVA: 0x000BD718 File Offset: 0x000BB918
		private void VariantMode(int returnIndex, bool minimal)
		{
			this.Paused = true;
			TextMenu menu = new TextMenu();
			menu.Add(new TextMenu.Header(Dialog.Clean("MENU_VARIANT_TITLE", null)));
			menu.Add(new TextMenu.SubHeader(Dialog.Clean("MENU_VARIANT_SUBTITLE", null), true));
			TextMenu.Slider speed;
			menu.Add(speed = new TextMenu.Slider(Dialog.Clean("MENU_ASSIST_GAMESPEED", null), (int i) => i * 10 + "%", 5, 16, SaveData.Instance.Assists.GameSpeed));
			speed.Change(delegate(int i)
			{
				if (i > 10)
				{
					if (speed.Values[speed.PreviousIndex].Item2 > i)
					{
						i--;
					}
					else
					{
						i++;
					}
				}
				speed.Index = i - 5;
				SaveData.Instance.Assists.GameSpeed = i;
				Engine.TimeRateB = (float)SaveData.Instance.Assists.GameSpeed / 10f;
			});
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_MIRROR", null), SaveData.Instance.Assists.MirrorMode).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.MirrorMode = on;
				VirtualIntegerAxis moveX = Input.MoveX;
				VirtualJoystick aim = Input.Aim;
				Input.Feather.InvertedX = on;
				aim.InvertedX = on;
				moveX.Inverted = on;
			}));
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_360DASHING", null), SaveData.Instance.Assists.ThreeSixtyDashing).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.ThreeSixtyDashing = on;
			}));
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_INVISMOTION", null), SaveData.Instance.Assists.InvisibleMotion).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.InvisibleMotion = on;
			}));
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_NOGRABBING", null), SaveData.Instance.Assists.NoGrabbing).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.NoGrabbing = on;
			}));
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_LOWFRICTION", null), SaveData.Instance.Assists.LowFriction).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.LowFriction = on;
			}));
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_SUPERDASHING", null), SaveData.Instance.Assists.SuperDashing).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.SuperDashing = on;
			}));
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_HICCUPS", null), SaveData.Instance.Assists.Hiccups).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.Hiccups = on;
			}));
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_VARIANT_PLAYASBADELINE", null), SaveData.Instance.Assists.PlayAsBadeline).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.PlayAsBadeline = on;
				Player entity = this.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					PlayerSpriteMode mode = SaveData.Instance.Assists.PlayAsBadeline ? PlayerSpriteMode.MadelineAsBadeline : entity.DefaultSpriteMode;
					if (entity.Active)
					{
						entity.ResetSpriteNextFrame(mode);
						return;
					}
					entity.ResetSprite(mode);
				}
			}));
			menu.Add(new TextMenu.SubHeader(Dialog.Clean("MENU_ASSIST_SUBTITLE", null), true));
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_INFINITE_STAMINA", null), SaveData.Instance.Assists.InfiniteStamina).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.InfiniteStamina = on;
			}));
			TextMenu.Option<int> option;
			menu.Add(option = new TextMenu.Slider(Dialog.Clean("MENU_ASSIST_AIR_DASHES", null), delegate(int i)
			{
				if (i == 0)
				{
					return Dialog.Clean("MENU_ASSIST_AIR_DASHES_NORMAL", null);
				}
				if (i == 1)
				{
					return Dialog.Clean("MENU_ASSIST_AIR_DASHES_TWO", null);
				}
				return Dialog.Clean("MENU_ASSIST_AIR_DASHES_INFINITE", null);
			}, 0, 2, (int)SaveData.Instance.Assists.DashMode).Change(delegate(int on)
			{
				SaveData.Instance.Assists.DashMode = (Assists.DashModes)on;
				Player entity = this.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					entity.Dashes = Math.Min(entity.Dashes, entity.MaxDashes);
				}
			}));
			if (this.Session.Area.ID == 0)
			{
				option.Disabled = true;
			}
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_DASH_ASSIST", null), SaveData.Instance.Assists.DashAssist).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.DashAssist = on;
			}));
			menu.Add(new TextMenu.OnOff(Dialog.Clean("MENU_ASSIST_INVINCIBLE", null), SaveData.Instance.Assists.Invincible).Change(delegate(bool on)
			{
				SaveData.Instance.Assists.Invincible = on;
			}));
			menu.OnESC = (menu.OnCancel = delegate()
			{
				Audio.Play("event:/ui/main/button_back");
				this.Pause(returnIndex, minimal, false);
				menu.Close();
			});
			menu.OnPause = delegate()
			{
				Audio.Play("event:/ui/main/button_back");
				this.Paused = false;
				this.unpauseTimer = 0.15f;
				menu.Close();
			};
			base.Add(menu);
		}

		// Token: 0x06001BEB RID: 7147 RVA: 0x000BDC28 File Offset: 0x000BBE28
		public void SnapColorGrade(string next)
		{
			if (this.Session.ColorGrade != next)
			{
				this.lastColorGrade = next;
				this.colorGradeEase = 0f;
				this.colorGradeEaseSpeed = 1f;
				this.Session.ColorGrade = next;
			}
		}

		// Token: 0x06001BEC RID: 7148 RVA: 0x000BDC66 File Offset: 0x000BBE66
		public void NextColorGrade(string next, float speed = 1f)
		{
			if (this.Session.ColorGrade != next)
			{
				this.colorGradeEase = 0f;
				this.colorGradeEaseSpeed = speed;
				this.Session.ColorGrade = next;
			}
		}

		// Token: 0x06001BED RID: 7149 RVA: 0x000BDC99 File Offset: 0x000BBE99
		public void Shake(float time = 0.3f)
		{
			if (Settings.Instance.ScreenShake != ScreenshakeAmount.Off)
			{
				this.shakeDirection = Vector2.Zero;
				this.shakeTimer = Math.Max(this.shakeTimer, time);
			}
		}

		// Token: 0x06001BEE RID: 7150 RVA: 0x000BDCC4 File Offset: 0x000BBEC4
		public void StopShake()
		{
			this.shakeTimer = 0f;
		}

		// Token: 0x06001BEF RID: 7151 RVA: 0x000BDCD1 File Offset: 0x000BBED1
		public void DirectionalShake(Vector2 dir, float time = 0.3f)
		{
			if (Settings.Instance.ScreenShake != ScreenshakeAmount.Off)
			{
				this.shakeDirection = dir.SafeNormalize();
				this.lastDirectionalShake = 0;
				this.shakeTimer = Math.Max(this.shakeTimer, time);
			}
		}

		// Token: 0x06001BF0 RID: 7152 RVA: 0x000BDD04 File Offset: 0x000BBF04
		public void Flash(Color color, bool drawPlayerOver = false)
		{
			if (!Settings.Instance.DisableFlashes)
			{
				this.doFlash = true;
				this.flashDrawPlayer = drawPlayerOver;
				this.flash = 1f;
				this.flashColor = color;
			}
		}

		// Token: 0x06001BF1 RID: 7153 RVA: 0x000BDD34 File Offset: 0x000BBF34
		public void ZoomSnap(Vector2 screenSpaceFocusPoint, float zoom)
		{
			this.ZoomFocusPoint = screenSpaceFocusPoint;
			this.Zoom = zoom;
			this.ZoomTarget = zoom;
		}

		// Token: 0x06001BF2 RID: 7154 RVA: 0x000BDD58 File Offset: 0x000BBF58
		public IEnumerator ZoomTo(Vector2 screenSpaceFocusPoint, float zoom, float duration)
		{
			this.ZoomFocusPoint = screenSpaceFocusPoint;
			this.ZoomTarget = zoom;
			float from = this.Zoom;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
			{
				this.Zoom = MathHelper.Lerp(from, this.ZoomTarget, Ease.SineInOut(MathHelper.Clamp(p, 0f, 1f)));
				yield return null;
			}
			this.Zoom = this.ZoomTarget;
			yield break;
		}

		// Token: 0x06001BF3 RID: 7155 RVA: 0x000BDD7C File Offset: 0x000BBF7C
		public IEnumerator ZoomAcross(Vector2 screenSpaceFocusPoint, float zoom, float duration)
		{
			float fromZoom = this.Zoom;
			Vector2 fromFocus = this.ZoomFocusPoint;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
			{
				float amount = Ease.SineInOut(MathHelper.Clamp(p, 0f, 1f));
				this.Zoom = (this.ZoomTarget = MathHelper.Lerp(fromZoom, zoom, amount));
				this.ZoomFocusPoint = Vector2.Lerp(fromFocus, screenSpaceFocusPoint, amount);
				yield return null;
			}
			this.Zoom = this.ZoomTarget;
			this.ZoomFocusPoint = screenSpaceFocusPoint;
			yield break;
		}

		// Token: 0x06001BF4 RID: 7156 RVA: 0x000BDDA0 File Offset: 0x000BBFA0
		public IEnumerator ZoomBack(float duration)
		{
			float from = this.Zoom;
			float to = 1f;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
			{
				this.Zoom = MathHelper.Lerp(from, to, Ease.SineInOut(MathHelper.Clamp(p, 0f, 1f)));
				yield return null;
			}
			this.ResetZoom();
			yield break;
		}

		// Token: 0x06001BF5 RID: 7157 RVA: 0x000BDDB6 File Offset: 0x000BBFB6
		public void ResetZoom()
		{
			this.Zoom = 1f;
			this.ZoomTarget = 1f;
			this.ZoomFocusPoint = new Vector2(320f, 180f) / 2f;
		}

		// Token: 0x06001BF6 RID: 7158 RVA: 0x000BDDF0 File Offset: 0x000BBFF0
		public void DoScreenWipe(bool wipeIn, Action onComplete = null, bool hiresSnow = false)
		{
			AreaData.Get(this.Session).DoScreenWipe(this, wipeIn, onComplete);
			if (hiresSnow)
			{
				base.Add(this.HiresSnow = new HiresSnow(0.45f));
				this.HiresSnow.Alpha = 0f;
				this.HiresSnow.AttachAlphaTo = this.Wipe;
			}
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06001BF7 RID: 7159 RVA: 0x000BDE4D File Offset: 0x000BC04D
		// (set) Token: 0x06001BF8 RID: 7160 RVA: 0x000BDE58 File Offset: 0x000BC058
		public Session.CoreModes CoreMode
		{
			get
			{
				return this.coreMode;
			}
			set
			{
				if (this.coreMode != value)
				{
					this.coreMode = value;
					this.Session.SetFlag("cold", this.coreMode == Session.CoreModes.Cold);
					Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "room_state", (float)((this.coreMode == Session.CoreModes.Hot) ? 0 : 1));
					if (Audio.CurrentMusic == "event:/music/lvl9/main")
					{
						this.Session.Audio.Music.Layer(1, this.coreMode == Session.CoreModes.Hot);
						this.Session.Audio.Music.Layer(2, this.coreMode == Session.CoreModes.Cold);
						this.Session.Audio.Apply(false);
					}
					foreach (Component component in base.Tracker.GetComponents<CoreModeListener>())
					{
						CoreModeListener coreModeListener = (CoreModeListener)component;
						if (coreModeListener.OnChange != null)
						{
							coreModeListener.OnChange(value);
						}
					}
				}
			}
		}

		// Token: 0x06001BF9 RID: 7161 RVA: 0x000BDF70 File Offset: 0x000BC170
		public bool InsideCamera(Vector2 position, float expand = 0f)
		{
			return position.X >= this.Camera.Left - expand && position.X < this.Camera.Right + expand && position.Y >= this.Camera.Top - expand && position.Y < this.Camera.Bottom + expand;
		}

		// Token: 0x06001BFA RID: 7162 RVA: 0x000BDFD4 File Offset: 0x000BC1D4
		public void EnforceBounds(Player player)
		{
			Rectangle bounds = this.Bounds;
			Rectangle rectangle = new Rectangle((int)this.Camera.Left, (int)this.Camera.Top, 320, 180);
			if (this.transition != null)
			{
				return;
			}
			if (this.CameraLockMode == Level.CameraLockModes.FinalBoss && player.Left < (float)rectangle.Left)
			{
				player.Left = (float)rectangle.Left;
				player.OnBoundsH();
			}
			else if (player.Left < (float)bounds.Left)
			{
				if (player.Top >= (float)bounds.Top && player.Bottom < (float)bounds.Bottom && this.Session.MapData.CanTransitionTo(this, player.Center + Vector2.UnitX * -8f))
				{
					player.BeforeSideTransition();
					this.NextLevel(player.Center + Vector2.UnitX * -8f, -Vector2.UnitX);
					return;
				}
				player.Left = (float)bounds.Left;
				player.OnBoundsH();
			}
			TheoCrystal entity = base.Tracker.GetEntity<TheoCrystal>();
			if (this.CameraLockMode == Level.CameraLockModes.FinalBoss && player.Right > (float)rectangle.Right && rectangle.Right < bounds.Right - 4)
			{
				player.Right = (float)rectangle.Right;
				player.OnBoundsH();
			}
			else if (entity != null && (player.Holding == null || !player.Holding.IsHeld) && player.Right > (float)(bounds.Right - 1))
			{
				player.Right = (float)(bounds.Right - 1);
			}
			else if (player.Right > (float)bounds.Right)
			{
				if (player.Top >= (float)bounds.Top && player.Bottom < (float)bounds.Bottom && this.Session.MapData.CanTransitionTo(this, player.Center + Vector2.UnitX * 8f))
				{
					player.BeforeSideTransition();
					this.NextLevel(player.Center + Vector2.UnitX * 8f, Vector2.UnitX);
					return;
				}
				player.Right = (float)bounds.Right;
				player.OnBoundsH();
			}
			if (this.CameraLockMode != Level.CameraLockModes.None && player.Top < (float)rectangle.Top)
			{
				player.Top = (float)rectangle.Top;
				player.OnBoundsV();
			}
			else if (player.CenterY < (float)bounds.Top)
			{
				if (this.Session.MapData.CanTransitionTo(this, player.Center - Vector2.UnitY * 12f))
				{
					player.BeforeUpTransition();
					this.NextLevel(player.Center - Vector2.UnitY * 12f, -Vector2.UnitY);
					return;
				}
				if (player.Top < (float)(bounds.Top - 24))
				{
					player.Top = (float)(bounds.Top - 24);
					player.OnBoundsV();
				}
			}
			if (this.CameraLockMode == Level.CameraLockModes.None || rectangle.Bottom >= bounds.Bottom - 4 || player.Top <= (float)rectangle.Bottom)
			{
				if (player.Bottom > (float)bounds.Bottom && this.Session.MapData.CanTransitionTo(this, player.Center + Vector2.UnitY * 12f) && !this.Session.LevelData.DisableDownTransition)
				{
					if (!player.CollideCheck<Solid>(player.Position + Vector2.UnitY * 4f))
					{
						player.BeforeDownTransition();
						this.NextLevel(player.Center + Vector2.UnitY * 12f, Vector2.UnitY);
						return;
					}
				}
				else
				{
					if (player.Top > (float)bounds.Bottom && SaveData.Instance.Assists.Invincible)
					{
						player.Play("event:/game/general/assist_screenbottom", null, 0f);
						player.Bounce((float)bounds.Bottom);
						return;
					}
					if (player.Top > (float)(bounds.Bottom + 4))
					{
						player.Die(Vector2.Zero, false, true);
					}
				}
				return;
			}
			if (SaveData.Instance.Assists.Invincible)
			{
				player.Play("event:/game/general/assist_screenbottom", null, 0f);
				player.Bounce((float)rectangle.Bottom);
				return;
			}
			player.Die(Vector2.Zero, false, true);
		}

		// Token: 0x06001BFB RID: 7163 RVA: 0x000BE45C File Offset: 0x000BC65C
		public bool IsInBounds(Entity entity)
		{
			Rectangle bounds = this.Bounds;
			return entity.Right > (float)bounds.Left && entity.Bottom > (float)bounds.Top && entity.Left < (float)bounds.Right && entity.Top < (float)bounds.Bottom;
		}

		// Token: 0x06001BFC RID: 7164 RVA: 0x000BE4B4 File Offset: 0x000BC6B4
		public bool IsInBounds(Vector2 position)
		{
			Rectangle bounds = this.Bounds;
			return position.X >= (float)bounds.Left && position.Y >= (float)bounds.Top && position.X < (float)bounds.Right && position.Y < (float)bounds.Bottom;
		}

		// Token: 0x06001BFD RID: 7165 RVA: 0x000BE50C File Offset: 0x000BC70C
		public bool IsInBounds(Vector2 position, float pad)
		{
			Rectangle bounds = this.Bounds;
			return position.X >= (float)bounds.Left - pad && position.Y >= (float)bounds.Top - pad && position.X < (float)bounds.Right + pad && position.Y < (float)bounds.Bottom + pad;
		}

		// Token: 0x06001BFE RID: 7166 RVA: 0x000BE56C File Offset: 0x000BC76C
		public bool IsInBounds(Vector2 position, Vector2 dirPad)
		{
			float num = Math.Max(dirPad.X, 0f);
			float num2 = Math.Max(-dirPad.X, 0f);
			float num3 = Math.Max(dirPad.Y, 0f);
			float num4 = Math.Max(-dirPad.Y, 0f);
			Rectangle bounds = this.Bounds;
			return position.X >= (float)bounds.Left + num && position.Y >= (float)bounds.Top + num3 && position.X < (float)bounds.Right - num2 && position.Y < (float)bounds.Bottom - num4;
		}

		// Token: 0x06001BFF RID: 7167 RVA: 0x000BE614 File Offset: 0x000BC814
		public bool IsInCamera(Vector2 position, float pad)
		{
			Rectangle rectangle = new Rectangle((int)this.Camera.X, (int)this.Camera.Y, 320, 180);
			return position.X >= (float)rectangle.Left - pad && position.Y >= (float)rectangle.Top - pad && position.X < (float)rectangle.Right + pad && position.Y < (float)rectangle.Bottom + pad;
		}

		// Token: 0x06001C00 RID: 7168 RVA: 0x000BE694 File Offset: 0x000BC894
		public void StartCutscene(Action<Level> onSkip, bool fadeInOnSkip = true, bool endingChapterAfterCutscene = false, bool resetZoomOnSkip = true)
		{
			this.endingChapterAfterCutscene = endingChapterAfterCutscene;
			this.InCutscene = true;
			this.onCutsceneSkip = onSkip;
			this.onCutsceneSkipFadeIn = fadeInOnSkip;
			this.onCutsceneSkipResetZoom = resetZoomOnSkip;
		}

		// Token: 0x06001C01 RID: 7169 RVA: 0x000BE6BA File Offset: 0x000BC8BA
		public void CancelCutscene()
		{
			this.InCutscene = false;
			this.SkippingCutscene = false;
		}

		// Token: 0x06001C02 RID: 7170 RVA: 0x000BE6CC File Offset: 0x000BC8CC
		public void SkipCutscene()
		{
			this.SkippingCutscene = true;
			Engine.TimeRate = 1f;
			Distort.Anxiety = 0f;
			Distort.GameRate = 1f;
			if (this.endingChapterAfterCutscene)
			{
				Audio.BusStopAll("bus:/gameplay_sfx", true);
			}
			List<Entity> list = new List<Entity>();
			foreach (Entity item in base.Tracker.GetEntities<Textbox>())
			{
				list.Add(item);
			}
			foreach (Entity entity in list)
			{
				entity.RemoveSelf();
			}
			this.skipCoroutine = new Coroutine(this.SkipCutsceneRoutine(), true);
		}

		// Token: 0x06001C03 RID: 7171 RVA: 0x000BE7B0 File Offset: 0x000BC9B0
		private IEnumerator SkipCutsceneRoutine()
		{
			yield return new FadeWipe(this, false, null)
			{
				Duration = 0.25f
			}.Wait();
			this.onCutsceneSkip(this);
			this.strawberriesDisplay.DrawLerp = 0f;
			if (this.onCutsceneSkipResetZoom)
			{
				this.ResetZoom();
			}
			GameplayStats gameplayStats = base.Entities.FindFirst<GameplayStats>();
			if (gameplayStats != null)
			{
				gameplayStats.DrawLerp = 0f;
			}
			if (this.onCutsceneSkipFadeIn)
			{
				FadeWipe fadeWipe = new FadeWipe(this, true, null);
				fadeWipe.Duration = 0.25f;
				base.RendererList.UpdateLists();
				yield return fadeWipe.Wait();
			}
			this.SkippingCutscene = false;
			this.EndCutscene();
			yield break;
		}

		// Token: 0x06001C04 RID: 7172 RVA: 0x000BE7BF File Offset: 0x000BC9BF
		public void EndCutscene()
		{
			if (!this.SkippingCutscene)
			{
				this.InCutscene = false;
			}
		}

		// Token: 0x06001C05 RID: 7173 RVA: 0x000BE7D0 File Offset: 0x000BC9D0
		private void NextLevel(Vector2 at, Vector2 dir)
		{
			base.OnEndOfFrame += delegate()
			{
				Engine.TimeRate = 1f;
				Distort.Anxiety = 0f;
				Distort.GameRate = 1f;
				this.TransitionTo(this.Session.MapData.GetAt(at), dir);
			};
		}

		// Token: 0x06001C06 RID: 7174 RVA: 0x000BE80C File Offset: 0x000BCA0C
		public void RegisterAreaComplete()
		{
			if (!this.Completed)
			{
				Player entity = base.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					List<Strawberry> list = new List<Strawberry>();
					foreach (Follower follower in entity.Leader.Followers)
					{
						if (follower.Entity is Strawberry)
						{
							list.Add(follower.Entity as Strawberry);
						}
					}
					foreach (Strawberry strawberry in list)
					{
						strawberry.OnCollect();
					}
				}
				this.Completed = true;
				SaveData.Instance.RegisterCompletion(this.Session);
			}
		}

		// Token: 0x06001C07 RID: 7175 RVA: 0x000BE8F0 File Offset: 0x000BCAF0
		public ScreenWipe CompleteArea(bool spotlightWipe = true, bool skipScreenWipe = false, bool skipCompleteScreen = false)
		{
			this.RegisterAreaComplete();
			this.PauseLock = true;
			Action action;
			if (AreaData.Get(this.Session).Interlude || skipCompleteScreen)
			{
				action = delegate()
				{
					Engine.Scene = new LevelExit(LevelExit.Mode.CompletedInterlude, this.Session, this.HiresSnow);
				};
			}
			else
			{
				action = delegate()
				{
					Engine.Scene = new LevelExit(LevelExit.Mode.Completed, this.Session, null);
				};
			}
			if (this.SkippingCutscene || skipScreenWipe)
			{
				Audio.BusStopAll("bus:/gameplay_sfx", true);
				action();
				return null;
			}
			if (spotlightWipe)
			{
				Player entity = base.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					SpotlightWipe.FocusPoint = entity.Position - this.Camera.Position - new Vector2(0f, 8f);
				}
				return new SpotlightWipe(this, false, action);
			}
			return new FadeWipe(this, false, action);
		}

		// Token: 0x040018DF RID: 6367
		public bool Completed;

		// Token: 0x040018E0 RID: 6368
		public bool NewLevel;

		// Token: 0x040018E1 RID: 6369
		public bool TimerStarted;

		// Token: 0x040018E2 RID: 6370
		public bool TimerStopped;

		// Token: 0x040018E3 RID: 6371
		public bool TimerHidden;

		// Token: 0x040018E4 RID: 6372
		public Session Session;

		// Token: 0x040018E5 RID: 6373
		public Vector2? StartPosition;

		// Token: 0x040018E6 RID: 6374
		public bool DarkRoom;

		// Token: 0x040018E7 RID: 6375
		public Player.IntroTypes LastIntroType;

		// Token: 0x040018E8 RID: 6376
		public bool InCredits;

		// Token: 0x040018E9 RID: 6377
		public bool AllowHudHide = true;

		// Token: 0x040018EA RID: 6378
		public VirtualMap<char> SolidsData;

		// Token: 0x040018EB RID: 6379
		public VirtualMap<char> BgData;

		// Token: 0x040018EC RID: 6380
		public float NextTransitionDuration = 0.65f;

		// Token: 0x040018ED RID: 6381
		public const float DefaultTransitionDuration = 0.65f;

		// Token: 0x040018EE RID: 6382
		public ScreenWipe Wipe;

		// Token: 0x040018EF RID: 6383
		private Coroutine transition;

		// Token: 0x040018F0 RID: 6384
		private Coroutine saving;

		// Token: 0x040018F1 RID: 6385
		public FormationBackdrop FormationBackdrop;

		// Token: 0x040018F2 RID: 6386
		public SolidTiles SolidTiles;

		// Token: 0x040018F3 RID: 6387
		public BackgroundTiles BgTiles;

		// Token: 0x040018F4 RID: 6388
		public Color BackgroundColor = Color.Black;

		// Token: 0x040018F5 RID: 6389
		public BackdropRenderer Background;

		// Token: 0x040018F6 RID: 6390
		public BackdropRenderer Foreground;

		// Token: 0x040018F7 RID: 6391
		public GameplayRenderer GameplayRenderer;

		// Token: 0x040018F8 RID: 6392
		public HudRenderer HudRenderer;

		// Token: 0x040018F9 RID: 6393
		public LightingRenderer Lighting;

		// Token: 0x040018FA RID: 6394
		public DisplacementRenderer Displacement;

		// Token: 0x040018FB RID: 6395
		public BloomRenderer Bloom;

		// Token: 0x040018FC RID: 6396
		public TileGrid FgTilesLightMask;

		// Token: 0x040018FD RID: 6397
		public ParticleSystem Particles;

		// Token: 0x040018FE RID: 6398
		public ParticleSystem ParticlesBG;

		// Token: 0x040018FF RID: 6399
		public ParticleSystem ParticlesFG;

		// Token: 0x04001900 RID: 6400
		public HiresSnow HiresSnow;

		// Token: 0x04001901 RID: 6401
		public TotalStrawberriesDisplay strawberriesDisplay;

		// Token: 0x04001902 RID: 6402
		private WindController windController;

		// Token: 0x04001903 RID: 6403
		public const float CameraOffsetXInterval = 48f;

		// Token: 0x04001904 RID: 6404
		public const float CameraOffsetYInterval = 32f;

		// Token: 0x04001905 RID: 6405
		public Camera Camera;

		// Token: 0x04001906 RID: 6406
		public Level.CameraLockModes CameraLockMode;

		// Token: 0x04001907 RID: 6407
		public Vector2 CameraOffset;

		// Token: 0x04001908 RID: 6408
		public float CameraUpwardMaxY;

		// Token: 0x0400190A RID: 6410
		private Vector2 shakeDirection;

		// Token: 0x0400190B RID: 6411
		private int lastDirectionalShake;

		// Token: 0x0400190C RID: 6412
		private float shakeTimer;

		// Token: 0x0400190D RID: 6413
		private Vector2 cameraPreShake;

		// Token: 0x0400190E RID: 6414
		public float ScreenPadding;

		// Token: 0x0400190F RID: 6415
		private float flash;

		// Token: 0x04001910 RID: 6416
		private Color flashColor = Color.White;

		// Token: 0x04001911 RID: 6417
		private bool doFlash;

		// Token: 0x04001912 RID: 6418
		private bool flashDrawPlayer;

		// Token: 0x04001913 RID: 6419
		private float glitchTimer;

		// Token: 0x04001914 RID: 6420
		private float glitchSeed;

		// Token: 0x04001915 RID: 6421
		public float Zoom = 1f;

		// Token: 0x04001916 RID: 6422
		public float ZoomTarget = 1f;

		// Token: 0x04001917 RID: 6423
		public Vector2 ZoomFocusPoint;

		// Token: 0x04001918 RID: 6424
		private string lastColorGrade;

		// Token: 0x04001919 RID: 6425
		private float colorGradeEase;

		// Token: 0x0400191A RID: 6426
		private float colorGradeEaseSpeed = 1f;

		// Token: 0x0400191B RID: 6427
		public Vector2 Wind;

		// Token: 0x0400191C RID: 6428
		public float WindSine;

		// Token: 0x0400191D RID: 6429
		public float WindSineTimer;

		// Token: 0x0400191E RID: 6430
		public bool Frozen;

		// Token: 0x0400191F RID: 6431
		public bool PauseLock;

		// Token: 0x04001920 RID: 6432
		public bool CanRetry = true;

		// Token: 0x04001921 RID: 6433
		public bool PauseMainMenuOpen;

		// Token: 0x04001922 RID: 6434
		private bool wasPaused;

		// Token: 0x04001923 RID: 6435
		private float wasPausedTimer;

		// Token: 0x04001924 RID: 6436
		private float unpauseTimer;

		// Token: 0x04001925 RID: 6437
		public bool SaveQuitDisabled;

		// Token: 0x04001927 RID: 6439
		public bool InCutscene;

		// Token: 0x04001928 RID: 6440
		public bool SkippingCutscene;

		// Token: 0x04001929 RID: 6441
		private Coroutine skipCoroutine;

		// Token: 0x0400192A RID: 6442
		private Action<Level> onCutsceneSkip;

		// Token: 0x0400192B RID: 6443
		private bool onCutsceneSkipFadeIn;

		// Token: 0x0400192C RID: 6444
		private bool onCutsceneSkipResetZoom;

		// Token: 0x0400192D RID: 6445
		private bool endingChapterAfterCutscene;

		// Token: 0x0400192E RID: 6446
		public static EventInstance DialogSnapshot;

		// Token: 0x0400192F RID: 6447
		private static EventInstance PauseSnapshot;

		// Token: 0x04001930 RID: 6448
		private static EventInstance AssistSpeedSnapshot;

		// Token: 0x04001931 RID: 6449
		private static int AssistSpeedSnapshotValue = -1;

		// Token: 0x04001932 RID: 6450
		public Pathfinder Pathfinder;

		// Token: 0x04001933 RID: 6451
		public PlayerDeadBody RetryPlayerCorpse;

		// Token: 0x04001934 RID: 6452
		public float BaseLightingAlpha;

		// Token: 0x04001935 RID: 6453
		private bool updateHair = true;

		// Token: 0x04001936 RID: 6454
		public bool InSpace;

		// Token: 0x04001937 RID: 6455
		public bool HasCassetteBlocks;

		// Token: 0x04001938 RID: 6456
		public float CassetteBlockTempo;

		// Token: 0x04001939 RID: 6457
		public int CassetteBlockBeats;

		// Token: 0x0400193A RID: 6458
		public Random HiccupRandom;

		// Token: 0x0400193B RID: 6459
		public bool Raining;

		// Token: 0x0400193D RID: 6461
		private Session.CoreModes coreMode;

		// Token: 0x02000723 RID: 1827
		public enum CameraLockModes
		{
			// Token: 0x04002DE6 RID: 11750
			None,
			// Token: 0x04002DE7 RID: 11751
			BoostSequence,
			// Token: 0x04002DE8 RID: 11752
			FinalBoss,
			// Token: 0x04002DE9 RID: 11753
			FinalBossNoY,
			// Token: 0x04002DEA RID: 11754
			Lava
		}

		// Token: 0x02000724 RID: 1828
		private enum ConditionBlockModes
		{
			// Token: 0x04002DEC RID: 11756
			Key,
			// Token: 0x04002DED RID: 11757
			Button,
			// Token: 0x04002DEE RID: 11758
			Strawberry
		}
	}
}
