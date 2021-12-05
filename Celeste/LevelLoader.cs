using System;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002A4 RID: 676
	public class LevelLoader : Scene
	{
		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060014F3 RID: 5363 RVA: 0x00075468 File Offset: 0x00073668
		// (set) Token: 0x060014F4 RID: 5364 RVA: 0x00075470 File Offset: 0x00073670
		public Level Level { get; private set; }

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060014F5 RID: 5365 RVA: 0x00075479 File Offset: 0x00073679
		// (set) Token: 0x060014F6 RID: 5366 RVA: 0x00075481 File Offset: 0x00073681
		public bool Loaded { get; private set; }

		// Token: 0x060014F7 RID: 5367 RVA: 0x0007548C File Offset: 0x0007368C
		public LevelLoader(Session session, Vector2? startPosition = null)
		{
			this.session = session;
			if (startPosition == null)
			{
				this.startPosition = session.RespawnPoint;
			}
			else
			{
				this.startPosition = startPosition;
			}
			this.Level = new Level();
			RunThread.Start(new Action(this.LoadingThread), "LEVEL_LOADER", false);
		}

		// Token: 0x060014F8 RID: 5368 RVA: 0x000754E8 File Offset: 0x000736E8
		private void LoadingThread()
		{
			MapData mapData = this.session.MapData;
			AreaData areaData = AreaData.Get(this.session);
			if (this.session.Area.ID == 0)
			{
				SaveData.Instance.Assists.DashMode = Assists.DashModes.Normal;
			}
			this.Level.Add(this.Level.GameplayRenderer = new GameplayRenderer());
			this.Level.Add(this.Level.Lighting = new LightingRenderer());
			this.Level.Add(this.Level.Bloom = new BloomRenderer());
			this.Level.Add(this.Level.Displacement = new DisplacementRenderer());
			this.Level.Add(this.Level.Background = new BackdropRenderer());
			this.Level.Add(this.Level.Foreground = new BackdropRenderer());
			this.Level.Add(new DustEdges());
			this.Level.Add(new WaterSurface());
			this.Level.Add(new MirrorSurfaces());
			this.Level.Add(new GlassBlockBg());
			this.Level.Add(new LightningRenderer());
			this.Level.Add(new SeekerBarrierRenderer());
			this.Level.Add(this.Level.HudRenderer = new HudRenderer());
			if (this.session.Area.ID == 9)
			{
				this.Level.Add(new IceTileOverlay());
			}
			this.Level.BaseLightingAlpha = (this.Level.Lighting.Alpha = areaData.DarknessAlpha);
			this.Level.Bloom.Base = areaData.BloomBase;
			this.Level.Bloom.Strength = areaData.BloomStrength;
			this.Level.BackgroundColor = mapData.BackgroundColor;
			this.Level.Background.Backdrops = mapData.CreateBackdrops(mapData.Background);
			foreach (Backdrop backdrop in this.Level.Background.Backdrops)
			{
				backdrop.Renderer = this.Level.Background;
			}
			this.Level.Foreground.Backdrops = mapData.CreateBackdrops(mapData.Foreground);
			foreach (Backdrop backdrop2 in this.Level.Foreground.Backdrops)
			{
				backdrop2.Renderer = this.Level.Foreground;
			}
			this.Level.RendererList.UpdateLists();
			this.Level.Add(this.Level.FormationBackdrop = new FormationBackdrop());
			this.Level.Camera = this.Level.GameplayRenderer.Camera;
			Audio.SetCamera(this.Level.Camera);
			this.Level.Session = this.session;
			SaveData.Instance.StartSession(this.Level.Session);
			this.Level.Particles = new ParticleSystem(-8000, 400);
			this.Level.Particles.Tag = Tags.Global;
			this.Level.Add(this.Level.Particles);
			this.Level.ParticlesBG = new ParticleSystem(8000, 400);
			this.Level.ParticlesBG.Tag = Tags.Global;
			this.Level.Add(this.Level.ParticlesBG);
			this.Level.ParticlesFG = new ParticleSystem(-50000, 800);
			this.Level.ParticlesFG.Tag = Tags.Global;
			this.Level.ParticlesFG.Add(new MirrorReflection());
			this.Level.Add(this.Level.ParticlesFG);
			this.Level.Add(this.Level.strawberriesDisplay = new TotalStrawberriesDisplay());
			this.Level.Add(new SpeedrunTimerDisplay());
			this.Level.Add(new GameplayStats());
			this.Level.Add(new GrabbyIcon());
			Rectangle tileBounds = mapData.TileBounds;
			GFX.FGAutotiler.LevelBounds.Clear();
			VirtualMap<char> virtualMap = new VirtualMap<char>(tileBounds.Width, tileBounds.Height, '0');
			VirtualMap<char> virtualMap2 = new VirtualMap<char>(tileBounds.Width, tileBounds.Height, '0');
			VirtualMap<bool> virtualMap3 = new VirtualMap<bool>(tileBounds.Width, tileBounds.Height, false);
			Regex regex = new Regex("\\r\\n|\\n\\r|\\n|\\r");
			foreach (LevelData levelData in mapData.Levels)
			{
				int left = levelData.TileBounds.Left;
				int top = levelData.TileBounds.Top;
				string[] array = regex.Split(levelData.Bg);
				for (int i = top; i < top + array.Length; i++)
				{
					for (int j = left; j < left + array[i - top].Length; j++)
					{
						virtualMap[j - tileBounds.X, i - tileBounds.Y] = array[i - top][j - left];
					}
				}
				string[] array2 = regex.Split(levelData.Solids);
				for (int k = top; k < top + array2.Length; k++)
				{
					for (int l = left; l < left + array2[k - top].Length; l++)
					{
						virtualMap2[l - tileBounds.X, k - tileBounds.Y] = array2[k - top][l - left];
					}
				}
				for (int m = levelData.TileBounds.Left; m < levelData.TileBounds.Right; m++)
				{
					for (int n = levelData.TileBounds.Top; n < levelData.TileBounds.Bottom; n++)
					{
						virtualMap3[m - tileBounds.Left, n - tileBounds.Top] = true;
					}
				}
				GFX.FGAutotiler.LevelBounds.Add(new Rectangle(levelData.TileBounds.X - tileBounds.X, levelData.TileBounds.Y - tileBounds.Y, levelData.TileBounds.Width, levelData.TileBounds.Height));
			}
			foreach (Rectangle rectangle in mapData.Filler)
			{
				for (int num = rectangle.Left; num < rectangle.Right; num++)
				{
					for (int num2 = rectangle.Top; num2 < rectangle.Bottom; num2++)
					{
						char c = '0';
						if (rectangle.Top - tileBounds.Y > 0)
						{
							char c2 = virtualMap2[num - tileBounds.X, rectangle.Top - tileBounds.Y - 1];
							if (c2 != '0')
							{
								c = c2;
							}
						}
						if (c == '0' && rectangle.Left - tileBounds.X > 0)
						{
							char c3 = virtualMap2[rectangle.Left - tileBounds.X - 1, num2 - tileBounds.Y];
							if (c3 != '0')
							{
								c = c3;
							}
						}
						if (c == '0' && rectangle.Right - tileBounds.X < tileBounds.Width - 1)
						{
							char c4 = virtualMap2[rectangle.Right - tileBounds.X, num2 - tileBounds.Y];
							if (c4 != '0')
							{
								c = c4;
							}
						}
						if (c == '0' && rectangle.Bottom - tileBounds.Y < tileBounds.Height - 1)
						{
							char c5 = virtualMap2[num - tileBounds.X, rectangle.Bottom - tileBounds.Y];
							if (c5 != '0')
							{
								c = c5;
							}
						}
						if (c == '0')
						{
							c = '1';
						}
						virtualMap2[num - tileBounds.X, num2 - tileBounds.Y] = c;
						virtualMap3[num - tileBounds.X, num2 - tileBounds.Y] = true;
					}
				}
			}
			foreach (LevelData levelData2 in mapData.Levels)
			{
				for (int num3 = levelData2.TileBounds.Left; num3 < levelData2.TileBounds.Right; num3++)
				{
					int num4 = levelData2.TileBounds.Top;
					char value = virtualMap[num3 - tileBounds.X, num4 - tileBounds.Y];
					int num5 = 1;
					while (num5 < 4 && !virtualMap3[num3 - tileBounds.X, num4 - tileBounds.Y - num5])
					{
						virtualMap[num3 - tileBounds.X, num4 - tileBounds.Y - num5] = value;
						num5++;
					}
					num4 = levelData2.TileBounds.Bottom - 1;
					char value2 = virtualMap[num3 - tileBounds.X, num4 - tileBounds.Y];
					int num6 = 1;
					while (num6 < 4 && !virtualMap3[num3 - tileBounds.X, num4 - tileBounds.Y + num6])
					{
						virtualMap[num3 - tileBounds.X, num4 - tileBounds.Y + num6] = value2;
						num6++;
					}
				}
				for (int num7 = levelData2.TileBounds.Top - 4; num7 < levelData2.TileBounds.Bottom + 4; num7++)
				{
					int num8 = levelData2.TileBounds.Left;
					char value3 = virtualMap[num8 - tileBounds.X, num7 - tileBounds.Y];
					int num9 = 1;
					while (num9 < 4 && !virtualMap3[num8 - tileBounds.X - num9, num7 - tileBounds.Y])
					{
						virtualMap[num8 - tileBounds.X - num9, num7 - tileBounds.Y] = value3;
						num9++;
					}
					num8 = levelData2.TileBounds.Right - 1;
					char value4 = virtualMap[num8 - tileBounds.X, num7 - tileBounds.Y];
					int num10 = 1;
					while (num10 < 4 && !virtualMap3[num8 - tileBounds.X + num10, num7 - tileBounds.Y])
					{
						virtualMap[num8 - tileBounds.X + num10, num7 - tileBounds.Y] = value4;
						num10++;
					}
				}
			}
			foreach (LevelData levelData3 in mapData.Levels)
			{
				for (int num11 = levelData3.TileBounds.Left; num11 < levelData3.TileBounds.Right; num11++)
				{
					int num12 = levelData3.TileBounds.Top;
					if (virtualMap2[num11 - tileBounds.X, num12 - tileBounds.Y] == '0')
					{
						for (int num13 = 1; num13 < 8; num13++)
						{
							virtualMap3[num11 - tileBounds.X, num12 - tileBounds.Y - num13] = true;
						}
					}
					num12 = levelData3.TileBounds.Bottom - 1;
					if (virtualMap2[num11 - tileBounds.X, num12 - tileBounds.Y] == '0')
					{
						for (int num14 = 1; num14 < 8; num14++)
						{
							virtualMap3[num11 - tileBounds.X, num12 - tileBounds.Y + num14] = true;
						}
					}
				}
			}
			foreach (LevelData levelData4 in mapData.Levels)
			{
				for (int num15 = levelData4.TileBounds.Left; num15 < levelData4.TileBounds.Right; num15++)
				{
					int num16 = levelData4.TileBounds.Top;
					char value5 = virtualMap2[num15 - tileBounds.X, num16 - tileBounds.Y];
					int num17 = 1;
					while (num17 < 4 && !virtualMap3[num15 - tileBounds.X, num16 - tileBounds.Y - num17])
					{
						virtualMap2[num15 - tileBounds.X, num16 - tileBounds.Y - num17] = value5;
						num17++;
					}
					num16 = levelData4.TileBounds.Bottom - 1;
					char value6 = virtualMap2[num15 - tileBounds.X, num16 - tileBounds.Y];
					int num18 = 1;
					while (num18 < 4 && !virtualMap3[num15 - tileBounds.X, num16 - tileBounds.Y + num18])
					{
						virtualMap2[num15 - tileBounds.X, num16 - tileBounds.Y + num18] = value6;
						num18++;
					}
				}
				for (int num19 = levelData4.TileBounds.Top - 4; num19 < levelData4.TileBounds.Bottom + 4; num19++)
				{
					int num20 = levelData4.TileBounds.Left;
					char value7 = virtualMap2[num20 - tileBounds.X, num19 - tileBounds.Y];
					int num21 = 1;
					while (num21 < 4 && !virtualMap3[num20 - tileBounds.X - num21, num19 - tileBounds.Y])
					{
						virtualMap2[num20 - tileBounds.X - num21, num19 - tileBounds.Y] = value7;
						num21++;
					}
					num20 = levelData4.TileBounds.Right - 1;
					char value8 = virtualMap2[num20 - tileBounds.X, num19 - tileBounds.Y];
					int num22 = 1;
					while (num22 < 4 && !virtualMap3[num20 - tileBounds.X + num22, num19 - tileBounds.Y])
					{
						virtualMap2[num20 - tileBounds.X + num22, num19 - tileBounds.Y] = value8;
						num22++;
					}
				}
			}
			Vector2 position = new Vector2((float)tileBounds.X, (float)tileBounds.Y) * 8f;
			Calc.PushRandom(mapData.LoadSeed);
			BackgroundTiles backgroundTiles;
			this.Level.Add(this.Level.BgTiles = (backgroundTiles = new BackgroundTiles(position, virtualMap)));
			SolidTiles solidTiles;
			this.Level.Add(this.Level.SolidTiles = (solidTiles = new SolidTiles(position, virtualMap2)));
			this.Level.BgData = virtualMap;
			this.Level.SolidsData = virtualMap2;
			Calc.PopRandom();
			new Entity(position).Add(this.Level.FgTilesLightMask = new TileGrid(8, 8, tileBounds.Width, tileBounds.Height));
			this.Level.FgTilesLightMask.Color = Color.Black;
			foreach (LevelData levelData5 in mapData.Levels)
			{
				int left2 = levelData5.TileBounds.Left;
				int top2 = levelData5.TileBounds.Top;
				int width = levelData5.TileBounds.Width;
				int height = levelData5.TileBounds.Height;
				if (!string.IsNullOrEmpty(levelData5.BgTiles))
				{
					int[,] tiles = Calc.ReadCSVIntGrid(levelData5.BgTiles, width, height);
					backgroundTiles.Tiles.Overlay(GFX.SceneryTiles, tiles, left2 - tileBounds.X, top2 - tileBounds.Y);
				}
				if (!string.IsNullOrEmpty(levelData5.FgTiles))
				{
					int[,] tiles2 = Calc.ReadCSVIntGrid(levelData5.FgTiles, width, height);
					solidTiles.Tiles.Overlay(GFX.SceneryTiles, tiles2, left2 - tileBounds.X, top2 - tileBounds.Y);
					this.Level.FgTilesLightMask.Overlay(GFX.SceneryTiles, tiles2, left2 - tileBounds.X, top2 - tileBounds.Y);
				}
			}
			if (areaData.OnLevelBegin != null)
			{
				areaData.OnLevelBegin(this.Level);
			}
			this.Level.StartPosition = this.startPosition;
			this.Level.Pathfinder = new Pathfinder(this.Level);
			this.Loaded = true;
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x0007670C File Offset: 0x0007490C
		private void StartLevel()
		{
			this.started = true;
			Session session = this.Level.Session;
			Player.IntroTypes playerIntro;
			if (this.PlayerIntroTypeOverride != null)
			{
				playerIntro = this.PlayerIntroTypeOverride.Value;
			}
			else if (session.FirstLevel && session.StartedFromBeginning && session.JustStarted)
			{
				if (session.Area.Mode == AreaMode.CSide)
				{
					playerIntro = Player.IntroTypes.WalkInRight;
				}
				else
				{
					playerIntro = AreaData.Get(this.Level).IntroType;
				}
			}
			else
			{
				playerIntro = Player.IntroTypes.Respawn;
			}
			this.Level.LoadLevel(playerIntro, true);
			this.Level.Session.JustStarted = false;
			if (Engine.Scene == this)
			{
				Engine.Scene = this.Level;
			}
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x000767B7 File Offset: 0x000749B7
		public override void Update()
		{
			base.Update();
			if (this.Loaded && !this.started)
			{
				this.StartLevel();
			}
		}

		// Token: 0x040010CE RID: 4302
		private Session session;

		// Token: 0x040010CF RID: 4303
		private Vector2? startPosition;

		// Token: 0x040010D0 RID: 4304
		private bool started;

		// Token: 0x040010D1 RID: 4305
		public Player.IntroTypes? PlayerIntroTypeOverride;
	}
}
