using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Celeste.Pico8;
using Microsoft.Xna.Framework;
using Monocle;
using Steamworks;

namespace Celeste
{
	// Token: 0x0200037F RID: 895
	public class Celeste : Engine
	{
		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06001D49 RID: 7497 RVA: 0x0004EC30 File Offset: 0x0004CE30
		public static Vector2 TargetCenter
		{
			get
			{
				return new Vector2(1920f, 1080f) / 2f;
			}
		}

		// Token: 0x06001D4A RID: 7498 RVA: 0x000CC06C File Offset: 0x000CA26C
		public Celeste() : base(1920, 1080, 960, 540, "Celeste", Settings.Instance.Fullscreen, Settings.Instance.VSync)
		{
			this.Version = new Version(1, 4, 0, 0);
			Celeste.Instance = this;
			Engine.ExitOnEscapeKeypress = false;
			base.IsFixedTimeStep = true;
			Stats.MakeRequest();
			StatsForStadia.MakeRequest();
			Console.WriteLine("CELESTE : " + this.Version);
		}

		// Token: 0x06001D4B RID: 7499 RVA: 0x000CC100 File Offset: 0x000CA300
		protected override void Initialize()
		{
			base.Initialize();
			Settings.Instance.AfterLoad();
			if (Settings.Instance.Fullscreen)
			{
				Engine.ViewPadding = Settings.Instance.ViewportPadding;
			}
			Settings.Instance.ApplyScreen();
			SFX.Initialize();
			Tags.Initialize();
			Input.Initialize();
			Engine.Commands.Enabled = (Celeste.PlayMode == Celeste.PlayModes.Debug);
			Engine.Scene = new GameLoader();
		}

		// Token: 0x06001D4C RID: 7500 RVA: 0x000CC170 File Offset: 0x000CA370
		protected override void LoadContent()
		{
			base.LoadContent();
			Console.WriteLine("BEGIN LOAD");
			Celeste.LoadTimer = Stopwatch.StartNew();
			PlaybackData.Load();
			if (this.firstLoad)
			{
				this.firstLoad = false;
				Celeste.HudTarget = VirtualContent.CreateRenderTarget("hud-target", 1922, 1082, false, true, 0);
				Celeste.WipeTarget = VirtualContent.CreateRenderTarget("wipe-target", 1922, 1082, false, true, 0);
				OVR.Load();
				GFX.Load();
				MTN.Load();
			}
			if (GFX.Game != null)
			{
				Monocle.Draw.Particle = GFX.Game["util/particle"];
				Monocle.Draw.Pixel = new MTexture(GFX.Game["util/pixel"], 1, 1, 1, 1);
			}
			GFX.LoadEffects();
		}

		// Token: 0x06001D4D RID: 7501 RVA: 0x000CC22F File Offset: 0x000CA42F
		protected override void Update(GameTime gameTime)
		{
			SteamAPI.RunCallbacks();
			if (Celeste.SaveRoutine != null)
			{
				Celeste.SaveRoutine.Update();
			}
			this.AutoSplitterInfo.Update();
			Audio.Update();
			base.Update(gameTime);
			Input.UpdateGrab();
		}

		// Token: 0x06001D4E RID: 7502 RVA: 0x000CC264 File Offset: 0x000CA464
		protected override void OnSceneTransition(Scene last, Scene next)
		{
			if (!(last is OverworldLoader) || !(next is Overworld))
			{
				base.OnSceneTransition(last, next);
			}
			Engine.TimeRate = 1f;
			Audio.PauseGameplaySfx = false;
			Audio.SetMusicParam("fade", 1f);
			Distort.Anxiety = 0f;
			Distort.GameRate = 1f;
			Glitch.Value = 0f;
		}

		// Token: 0x06001D4F RID: 7503 RVA: 0x000CC2C6 File Offset: 0x000CA4C6
		protected override void RenderCore()
		{
			base.RenderCore();
			if (Celeste.DisconnectUI != null)
			{
				Celeste.DisconnectUI.Render();
			}
		}

		// Token: 0x06001D50 RID: 7504 RVA: 0x000CC2E0 File Offset: 0x000CA4E0
		public static void Freeze(float time)
		{
			if (Engine.FreezeTimer < time)
			{
				Engine.FreezeTimer = time;
				if (Engine.Scene != null)
				{
					CassetteBlockManager entity = Engine.Scene.Tracker.GetEntity<CassetteBlockManager>();
					if (entity != null)
					{
						entity.AdvanceMusic(time);
					}
				}
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06001D51 RID: 7505 RVA: 0x000CC31C File Offset: 0x000CA51C
		public static bool IsMainThread
		{
			get
			{
				return Thread.CurrentThread.ManagedThreadId == Celeste._mainThreadId;
			}
		}

		// Token: 0x06001D52 RID: 7506 RVA: 0x000CC330 File Offset: 0x000CA530
		private static void Main(string[] args)
		{
			Celeste celeste;
			try
			{
				Celeste._mainThreadId = Thread.CurrentThread.ManagedThreadId;
				Settings.Initialize();
				if (SteamAPI.RestartAppIfNecessary(Celeste.SteamID))
				{
					return;
				}
				if (!SteamAPI.Init())
				{
					ErrorLog.Write("Steam not found!");
					ErrorLog.Open();
					return;
				}
				if (!Settings.Existed)
				{
					Settings.Instance.Language = SteamApps.GetCurrentGameLanguage();
				}
				bool existed = Settings.Existed;
				for (int i = 0; i < args.Length - 1; i++)
				{
					if (args[i] == "--language" || args[i] == "-l")
					{
						Settings.Instance.Language = args[++i];
					}
					else if (args[i] == "--default-language" || args[i] == "-dl")
					{
						if (!Settings.Existed)
						{
							Settings.Instance.Language = args[++i];
						}
					}
					else if (args[i] == "--gui" || args[i] == "-g")
					{
						Input.OverrideInputPrefix = args[++i];
					}
				}
				celeste = new Celeste();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				ErrorLog.Write(ex);
				try
				{
					ErrorLog.Open();
				}
				catch
				{
					Console.WriteLine("Failed to open the log!");
				}
				return;
			}
			celeste.RunWithLogging();
			RunThread.WaitAll();
			celeste.Dispose();
			Audio.Unload();
		}

		// Token: 0x06001D53 RID: 7507 RVA: 0x000CC4BC File Offset: 0x000CA6BC
		public static void ReloadAssets(bool levels, bool graphics, bool hires, AreaKey? area = null)
		{
			if (levels)
			{
				Celeste.ReloadLevels(area);
			}
			if (graphics)
			{
				Celeste.ReloadGraphics(hires);
			}
		}

		// Token: 0x06001D54 RID: 7508 RVA: 0x000091E2 File Offset: 0x000073E2
		public static void ReloadLevels(AreaKey? area = null)
		{
		}

		// Token: 0x06001D55 RID: 7509 RVA: 0x000091E2 File Offset: 0x000073E2
		public static void ReloadGraphics(bool hires)
		{
		}

		// Token: 0x06001D56 RID: 7510 RVA: 0x000091E2 File Offset: 0x000073E2
		public static void ReloadPortraits()
		{
		}

		// Token: 0x06001D57 RID: 7511 RVA: 0x000091E2 File Offset: 0x000073E2
		public static void ReloadDialog()
		{
		}

		// Token: 0x06001D58 RID: 7512 RVA: 0x000CC4D0 File Offset: 0x000CA6D0
		private static void CallProcess(string path, string args = "", bool createWindow = false)
		{
			Process process = new Process();
			process.StartInfo = new ProcessStartInfo
			{
				FileName = path,
				WorkingDirectory = Path.GetDirectoryName(path),
				RedirectStandardOutput = false,
				CreateNoWindow = !createWindow,
				UseShellExecute = false,
				Arguments = args
			};
			process.Start();
			process.WaitForExit();
		}

		// Token: 0x06001D59 RID: 7513 RVA: 0x000CC52C File Offset: 0x000CA72C
		public static bool PauseAnywhere()
		{
			if (Engine.Scene is Level)
			{
				Level level = Engine.Scene as Level;
				if (level.CanPause)
				{
					level.Pause(0, false, false);
					return true;
				}
			}
			else if (Engine.Scene is Emulator)
			{
				Emulator emulator = Engine.Scene as Emulator;
				if (emulator.CanPause)
				{
					emulator.CreatePauseMenu();
					return true;
				}
			}
			else if (Engine.Scene is IntroVignette)
			{
				IntroVignette introVignette = Engine.Scene as IntroVignette;
				if (introVignette.CanPause)
				{
					introVignette.OpenMenu();
					return true;
				}
			}
			else if (Engine.Scene is CoreVignette)
			{
				CoreVignette coreVignette = Engine.Scene as CoreVignette;
				if (coreVignette.CanPause)
				{
					coreVignette.OpenMenu();
					return true;
				}
			}
			return false;
		}

		// Token: 0x04001ADB RID: 6875
		public const int GameWidth = 320;

		// Token: 0x04001ADC RID: 6876
		public const int GameHeight = 180;

		// Token: 0x04001ADD RID: 6877
		public const int TargetWidth = 1920;

		// Token: 0x04001ADE RID: 6878
		public const int TargetHeight = 1080;

		// Token: 0x04001ADF RID: 6879
		public static Celeste.PlayModes PlayMode = Celeste.PlayModes.Normal;

		// Token: 0x04001AE0 RID: 6880
		public const string EventName = "";

		// Token: 0x04001AE1 RID: 6881
		public const bool Beta = false;

		// Token: 0x04001AE2 RID: 6882
		public const string PLATFORM = "PC";

		// Token: 0x04001AE3 RID: 6883
		public new static Celeste Instance;

		// Token: 0x04001AE4 RID: 6884
		public static VirtualRenderTarget HudTarget;

		// Token: 0x04001AE5 RID: 6885
		public static VirtualRenderTarget WipeTarget;

		// Token: 0x04001AE6 RID: 6886
		public static DisconnectedControllerUI DisconnectUI;

		// Token: 0x04001AE7 RID: 6887
		private bool firstLoad = true;

		// Token: 0x04001AE8 RID: 6888
		public AutoSplitterInfo AutoSplitterInfo = new AutoSplitterInfo();

		// Token: 0x04001AE9 RID: 6889
		public static Coroutine SaveRoutine;

		// Token: 0x04001AEA RID: 6890
		public static Stopwatch LoadTimer;

		// Token: 0x04001AEB RID: 6891
		public static readonly AppId_t SteamID = new AppId_t(504230U);

		// Token: 0x04001AEC RID: 6892
		private static int _mainThreadId;

		// Token: 0x02000754 RID: 1876
		public enum PlayModes
		{
			// Token: 0x04002ED9 RID: 11993
			Normal,
			// Token: 0x04002EDA RID: 11994
			Debug,
			// Token: 0x04002EDB RID: 11995
			Event,
			// Token: 0x04002EDC RID: 11996
			Demo
		}
	}
}
