using System;
using System.IO;
using System.Reflection;
using System.Runtime;
using Celeste;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Monocle
{
	// Token: 0x02000105 RID: 261
	public class Engine : Game
	{
		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000731 RID: 1841 RVA: 0x0000CD73 File Offset: 0x0000AF73
		// (set) Token: 0x06000732 RID: 1842 RVA: 0x0000CD7A File Offset: 0x0000AF7A
		public static Engine Instance { get; private set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000733 RID: 1843 RVA: 0x0000CD82 File Offset: 0x0000AF82
		// (set) Token: 0x06000734 RID: 1844 RVA: 0x0000CD89 File Offset: 0x0000AF89
		public static GraphicsDeviceManager Graphics { get; private set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000735 RID: 1845 RVA: 0x0000CD91 File Offset: 0x0000AF91
		// (set) Token: 0x06000736 RID: 1846 RVA: 0x0000CD98 File Offset: 0x0000AF98
		public static Commands Commands { get; private set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000737 RID: 1847 RVA: 0x0000CDA0 File Offset: 0x0000AFA0
		// (set) Token: 0x06000738 RID: 1848 RVA: 0x0000CDA7 File Offset: 0x0000AFA7
		public static Pooler Pooler { get; private set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000739 RID: 1849 RVA: 0x0000CDAF File Offset: 0x0000AFAF
		// (set) Token: 0x0600073A RID: 1850 RVA: 0x0000CDB6 File Offset: 0x0000AFB6
		public static int Width { get; private set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600073B RID: 1851 RVA: 0x0000CDBE File Offset: 0x0000AFBE
		// (set) Token: 0x0600073C RID: 1852 RVA: 0x0000CDC5 File Offset: 0x0000AFC5
		public static int Height { get; private set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600073D RID: 1853 RVA: 0x0000CDCD File Offset: 0x0000AFCD
		// (set) Token: 0x0600073E RID: 1854 RVA: 0x0000CDD4 File Offset: 0x0000AFD4
		public static int ViewWidth { get; private set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600073F RID: 1855 RVA: 0x0000CDDC File Offset: 0x0000AFDC
		// (set) Token: 0x06000740 RID: 1856 RVA: 0x0000CDE3 File Offset: 0x0000AFE3
		public static int ViewHeight { get; private set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000741 RID: 1857 RVA: 0x0000CDEB File Offset: 0x0000AFEB
		// (set) Token: 0x06000742 RID: 1858 RVA: 0x0000CDF2 File Offset: 0x0000AFF2
		public static int ViewPadding
		{
			get
			{
				return Engine.viewPadding;
			}
			set
			{
				Engine.viewPadding = value;
				Engine.Instance.UpdateView();
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000743 RID: 1859 RVA: 0x0000CE04 File Offset: 0x0000B004
		// (set) Token: 0x06000744 RID: 1860 RVA: 0x0000CE0B File Offset: 0x0000B00B
		public static float DeltaTime { get; private set; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000745 RID: 1861 RVA: 0x0000CE13 File Offset: 0x0000B013
		// (set) Token: 0x06000746 RID: 1862 RVA: 0x0000CE1A File Offset: 0x0000B01A
		public static float RawDeltaTime { get; private set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000747 RID: 1863 RVA: 0x0000CE22 File Offset: 0x0000B022
		// (set) Token: 0x06000748 RID: 1864 RVA: 0x0000CE29 File Offset: 0x0000B029
		public static ulong FrameCounter { get; private set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000749 RID: 1865 RVA: 0x0000CE31 File Offset: 0x0000B031
		public static string ContentDirectory
		{
			get
			{
				return Path.Combine(Engine.AssemblyDirectory, Engine.Instance.Content.RootDirectory);
			}
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x0000CE4C File Offset: 0x0000B04C
		public Engine(int width, int height, int windowWidth, int windowHeight, string windowTitle, bool fullscreen, bool vsync)
		{
			Engine.Instance = this;
			base.Window.Title = windowTitle;
			this.Title = windowTitle;
			Engine.Width = width;
			Engine.Height = height;
			Engine.ClearColor = Color.Black;
			base.InactiveSleepTime = new TimeSpan(0L);
			Engine.Graphics = new GraphicsDeviceManager(this);
			Engine.Graphics.DeviceReset += this.OnGraphicsReset;
			Engine.Graphics.DeviceCreated += this.OnGraphicsCreate;
			Engine.Graphics.SynchronizeWithVerticalRetrace = vsync;
			Engine.Graphics.PreferMultiSampling = false;
			Engine.Graphics.GraphicsProfile = GraphicsProfile.HiDef;
			Engine.Graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
			Engine.Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
			base.Window.AllowUserResizing = true;
			base.Window.ClientSizeChanged += this.OnClientSizeChanged;
			if (fullscreen)
			{
				Engine.Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				Engine.Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
				Engine.Graphics.IsFullScreen = true;
			}
			else
			{
				Engine.Graphics.PreferredBackBufferWidth = windowWidth;
				Engine.Graphics.PreferredBackBufferHeight = windowHeight;
				Engine.Graphics.IsFullScreen = false;
			}
			base.Content.RootDirectory = "Content";
			base.IsMouseVisible = false;
			Engine.ExitOnEscapeKeypress = true;
			GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x0000CFC8 File Offset: 0x0000B1C8
		protected virtual void OnClientSizeChanged(object sender, EventArgs e)
		{
			if (base.Window.ClientBounds.Width > 0 && base.Window.ClientBounds.Height > 0 && !Engine.resizing)
			{
				Engine.resizing = true;
				Engine.Graphics.PreferredBackBufferWidth = base.Window.ClientBounds.Width;
				Engine.Graphics.PreferredBackBufferHeight = base.Window.ClientBounds.Height;
				this.UpdateView();
				Engine.resizing = false;
			}
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x0000D048 File Offset: 0x0000B248
		protected virtual void OnGraphicsReset(object sender, EventArgs e)
		{
			this.UpdateView();
			if (this.scene != null)
			{
				this.scene.HandleGraphicsReset();
			}
			if (this.nextScene != null && this.nextScene != this.scene)
			{
				this.nextScene.HandleGraphicsReset();
			}
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x0000D084 File Offset: 0x0000B284
		protected virtual void OnGraphicsCreate(object sender, EventArgs e)
		{
			this.UpdateView();
			if (this.scene != null)
			{
				this.scene.HandleGraphicsCreate();
			}
			if (this.nextScene != null && this.nextScene != this.scene)
			{
				this.nextScene.HandleGraphicsCreate();
			}
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x0000D0C0 File Offset: 0x0000B2C0
		protected override void OnActivated(object sender, EventArgs args)
		{
			base.OnActivated(sender, args);
			if (this.scene != null)
			{
				this.scene.GainFocus();
			}
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x0000D0DD File Offset: 0x0000B2DD
		protected override void OnDeactivated(object sender, EventArgs args)
		{
			base.OnDeactivated(sender, args);
			if (this.scene != null)
			{
				this.scene.LoseFocus();
			}
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x0000D0FA File Offset: 0x0000B2FA
		protected override void Initialize()
		{
			base.Initialize();
			MInput.Initialize();
			Tracker.Initialize();
			Engine.Pooler = new Pooler();
			Engine.Commands = new Commands();
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x0000D120 File Offset: 0x0000B320
		protected override void LoadContent()
		{
			base.LoadContent();
			VirtualContent.Reload();
			Monocle.Draw.Initialize(base.GraphicsDevice);
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x0000D138 File Offset: 0x0000B338
		protected override void UnloadContent()
		{
			base.UnloadContent();
			VirtualContent.Unload();
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x0000D148 File Offset: 0x0000B348
		protected override void Update(GameTime gameTime)
		{
			Engine.RawDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			Engine.DeltaTime = Engine.RawDeltaTime * Engine.TimeRate * Engine.TimeRateB;
			Engine.FrameCounter += 1UL;
			MInput.Update();
			if (Engine.ExitOnEscapeKeypress && MInput.Keyboard.Pressed(Keys.Escape))
			{
				base.Exit();
				return;
			}
			if (Engine.OverloadGameLoop != null)
			{
				Engine.OverloadGameLoop();
				base.Update(gameTime);
				return;
			}
			if (Engine.DashAssistFreeze)
			{
				if (Input.Dash.Check || !Engine.DashAssistFreezePress)
				{
					if (Input.Dash.Check)
					{
						Engine.DashAssistFreezePress = true;
					}
					if (this.scene != null)
					{
						PlayerDashAssist entity = this.scene.Tracker.GetEntity<PlayerDashAssist>();
						if (entity != null)
						{
							entity.Update();
						}
						if (this.scene is Level)
						{
							(this.scene as Level).UpdateTime();
						}
						this.scene.Entities.UpdateLists();
					}
				}
				else
				{
					Engine.DashAssistFreeze = false;
				}
			}
			if (!Engine.DashAssistFreeze)
			{
				if (Engine.FreezeTimer > 0f)
				{
					Engine.FreezeTimer = Math.Max(Engine.FreezeTimer - Engine.RawDeltaTime, 0f);
				}
				else if (this.scene != null)
				{
					this.scene.BeforeUpdate();
					this.scene.Update();
					this.scene.AfterUpdate();
				}
			}
			if (Engine.Commands.Open)
			{
				Engine.Commands.UpdateOpen();
			}
			else if (Engine.Commands.Enabled)
			{
				Engine.Commands.UpdateClosed();
			}
			if (this.scene != this.nextScene)
			{
				Scene from = this.scene;
				if (this.scene != null)
				{
					this.scene.End();
				}
				this.scene = this.nextScene;
				this.OnSceneTransition(from, this.nextScene);
				if (this.scene != null)
				{
					this.scene.Begin();
				}
			}
			base.Update(gameTime);
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x0000D32C File Offset: 0x0000B52C
		protected override void Draw(GameTime gameTime)
		{
			this.RenderCore();
			base.Draw(gameTime);
			if (Engine.Commands.Open)
			{
				Engine.Commands.Render();
			}
			this.fpsCounter++;
			this.counterElapsed += gameTime.ElapsedGameTime;
			if (this.counterElapsed >= TimeSpan.FromSeconds(1.0))
			{
				Engine.FPS = this.fpsCounter;
				this.fpsCounter = 0;
				this.counterElapsed -= TimeSpan.FromSeconds(1.0);
			}
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x0000D3D0 File Offset: 0x0000B5D0
		protected virtual void RenderCore()
		{
			if (this.scene != null)
			{
				this.scene.BeforeRender();
			}
			base.GraphicsDevice.SetRenderTarget(null);
			base.GraphicsDevice.Viewport = Engine.Viewport;
			base.GraphicsDevice.Clear(Engine.ClearColor);
			if (this.scene != null)
			{
				this.scene.Render();
				this.scene.AfterRender();
			}
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x0000D43A File Offset: 0x0000B63A
		protected override void OnExiting(object sender, EventArgs args)
		{
			base.OnExiting(sender, args);
			MInput.Shutdown();
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x0000D44C File Offset: 0x0000B64C
		public void RunWithLogging()
		{
			try
			{
				base.Run();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				ErrorLog.Write(ex);
				ErrorLog.Open();
			}
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x0000D488 File Offset: 0x0000B688
		protected virtual void OnSceneTransition(Scene from, Scene to)
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			Engine.TimeRate = 1f;
			Engine.DashAssistFreeze = false;
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000759 RID: 1881 RVA: 0x0000D4A4 File Offset: 0x0000B6A4
		// (set) Token: 0x0600075A RID: 1882 RVA: 0x0000D4B0 File Offset: 0x0000B6B0
		public static Scene Scene
		{
			get
			{
				return Engine.Instance.scene;
			}
			set
			{
				Engine.Instance.nextScene = value;
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x0600075B RID: 1883 RVA: 0x0000D4BD File Offset: 0x0000B6BD
		// (set) Token: 0x0600075C RID: 1884 RVA: 0x0000D4C4 File Offset: 0x0000B6C4
		public static Viewport Viewport { get; private set; }

		// Token: 0x0600075D RID: 1885 RVA: 0x0000D4CC File Offset: 0x0000B6CC
		public static void SetWindowed(int width, int height)
		{
			if (width > 0 && height > 0)
			{
				Engine.resizing = true;
				Engine.Graphics.PreferredBackBufferWidth = width;
				Engine.Graphics.PreferredBackBufferHeight = height;
				Engine.Graphics.IsFullScreen = false;
				Engine.Graphics.ApplyChanges();
				Console.WriteLine(string.Concat(new object[]
				{
					"WINDOW-",
					width,
					"x",
					height
				}));
				Engine.resizing = false;
			}
		}

		// Token: 0x0600075E RID: 1886 RVA: 0x0000D54C File Offset: 0x0000B74C
		public static void SetFullscreen()
		{
			Engine.resizing = true;
			Engine.Graphics.PreferredBackBufferWidth = Engine.Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
			Engine.Graphics.PreferredBackBufferHeight = Engine.Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
			Engine.Graphics.IsFullScreen = true;
			Engine.Graphics.ApplyChanges();
			Console.WriteLine("FULLSCREEN");
			Engine.resizing = false;
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x0000D5CC File Offset: 0x0000B7CC
		private void UpdateView()
		{
			float num = (float)base.GraphicsDevice.PresentationParameters.BackBufferWidth;
			float num2 = (float)base.GraphicsDevice.PresentationParameters.BackBufferHeight;
			if (num / (float)Engine.Width > num2 / (float)Engine.Height)
			{
				Engine.ViewWidth = (int)(num2 / (float)Engine.Height * (float)Engine.Width);
				Engine.ViewHeight = (int)num2;
			}
			else
			{
				Engine.ViewWidth = (int)num;
				Engine.ViewHeight = (int)(num / (float)Engine.Width * (float)Engine.Height);
			}
			float num3 = (float)Engine.ViewHeight / (float)Engine.ViewWidth;
			Engine.ViewWidth -= Engine.ViewPadding * 2;
			Engine.ViewHeight -= (int)(num3 * (float)Engine.ViewPadding * 2f);
			Engine.ScreenMatrix = Matrix.CreateScale((float)Engine.ViewWidth / (float)Engine.Width);
			Engine.Viewport = new Viewport
			{
				X = (int)(num / 2f - (float)(Engine.ViewWidth / 2)),
				Y = (int)(num2 / 2f - (float)(Engine.ViewHeight / 2)),
				Width = Engine.ViewWidth,
				Height = Engine.ViewHeight,
				MinDepth = 0f,
				MaxDepth = 1f
			};
		}

		// Token: 0x04000544 RID: 1348
		public string Title;

		// Token: 0x04000545 RID: 1349
		public Version Version;

		// Token: 0x0400054A RID: 1354
		public static Action OverloadGameLoop;

		// Token: 0x0400054F RID: 1359
		private static int viewPadding = 0;

		// Token: 0x04000550 RID: 1360
		private static bool resizing;

		// Token: 0x04000554 RID: 1364
		public static float TimeRate = 1f;

		// Token: 0x04000555 RID: 1365
		public static float TimeRateB = 1f;

		// Token: 0x04000556 RID: 1366
		public static float FreezeTimer;

		// Token: 0x04000557 RID: 1367
		public static bool DashAssistFreeze;

		// Token: 0x04000558 RID: 1368
		public static bool DashAssistFreezePress;

		// Token: 0x04000559 RID: 1369
		public static int FPS;

		// Token: 0x0400055A RID: 1370
		private TimeSpan counterElapsed = TimeSpan.Zero;

		// Token: 0x0400055B RID: 1371
		private int fpsCounter;

		// Token: 0x0400055C RID: 1372
		private static string AssemblyDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

		// Token: 0x0400055D RID: 1373
		public static Color ClearColor;

		// Token: 0x0400055E RID: 1374
		public static bool ExitOnEscapeKeypress;

		// Token: 0x0400055F RID: 1375
		private Scene scene;

		// Token: 0x04000560 RID: 1376
		private Scene nextScene;

		// Token: 0x04000562 RID: 1378
		public static Matrix ScreenMatrix;
	}
}
