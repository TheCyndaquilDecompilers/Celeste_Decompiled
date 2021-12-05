using System;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x02000320 RID: 800
	[Serializable]
	public class Settings
	{
		// Token: 0x170001CF RID: 463
		// (get) Token: 0x0600193A RID: 6458 RVA: 0x000A1580 File Offset: 0x0009F780
		// (set) Token: 0x0600193B RID: 6459 RVA: 0x000091E2 File Offset: 0x000073E2
		[XmlAnyElement("LaunchInDebugModeComment")]
		public XmlComment DebugModeComment
		{
			get
			{
				return new XmlDocument().CreateComment("\n\t\tLaunchWithFMODLiveUpdate:\n\t\t\tThis Enables FMOD Studio Live Update so you can interact with the sounds in real time.\n\t\t\tNote this will also require access to the private network.\n\t\t\n\t\tLaunchInDebugMode:\n\t\t\tDebug Mode can destroy save files, crash the game, and do other unwanted behaviour.\n\t\t\tIt is not documented. Use at own risk.\n\t");
			}
			set
			{
			}
		}

		// Token: 0x0600193C RID: 6460 RVA: 0x000A1594 File Offset: 0x0009F794
		public Settings()
		{
			if (Celeste.PlayMode != Celeste.PlayModes.Debug)
			{
				this.Fullscreen = true;
			}
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x000A1718 File Offset: 0x0009F918
		public void AfterLoad()
		{
			Binding.SetExclusive(new Binding[]
			{
				this.MenuLeft,
				this.MenuRight,
				this.MenuUp,
				this.MenuDown,
				this.Confirm,
				this.Cancel,
				this.Journal,
				this.Pause
			});
			this.MusicVolume = Calc.Clamp(this.MusicVolume, 0, 10);
			this.SFXVolume = Calc.Clamp(this.SFXVolume, 0, 10);
			this.WindowScale = Math.Min(this.WindowScale, this.MaxScale);
			this.WindowScale = Calc.Clamp(this.WindowScale, 3, 10);
			this.SetDefaultKeyboardControls(false);
			this.SetDefaultButtonControls(false);
			if (this.LaunchInDebugMode)
			{
				Celeste.PlayMode = Celeste.PlayModes.Debug;
			}
			Settings.LastVersion = (Settings.Existed ? Settings.Instance.Version : Celeste.Instance.Version.ToString());
			Settings.Instance.Version = Celeste.Instance.Version.ToString();
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x000A1828 File Offset: 0x0009FA28
		public void SetDefaultKeyboardControls(bool reset)
		{
			if (reset || this.Left.Keyboard.Count <= 0)
			{
				this.Left.Keyboard.Clear();
				this.Left.Add(new Keys[]
				{
					Keys.Left
				});
			}
			if (reset || this.Right.Keyboard.Count <= 0)
			{
				this.Right.Keyboard.Clear();
				this.Right.Add(new Keys[]
				{
					Keys.Right
				});
			}
			if (reset || this.Down.Keyboard.Count <= 0)
			{
				this.Down.Keyboard.Clear();
				this.Down.Add(new Keys[]
				{
					Keys.Down
				});
			}
			if (reset || this.Up.Keyboard.Count <= 0)
			{
				this.Up.Keyboard.Clear();
				this.Up.Add(new Keys[]
				{
					Keys.Up
				});
			}
			if (reset || this.MenuLeft.Keyboard.Count <= 0)
			{
				this.MenuLeft.Keyboard.Clear();
				this.MenuLeft.Add(new Keys[]
				{
					Keys.Left
				});
			}
			if (reset || this.MenuRight.Keyboard.Count <= 0)
			{
				this.MenuRight.Keyboard.Clear();
				this.MenuRight.Add(new Keys[]
				{
					Keys.Right
				});
			}
			if (reset || this.MenuDown.Keyboard.Count <= 0)
			{
				this.MenuDown.Keyboard.Clear();
				this.MenuDown.Add(new Keys[]
				{
					Keys.Down
				});
			}
			if (reset || this.MenuUp.Keyboard.Count <= 0)
			{
				this.MenuUp.Keyboard.Clear();
				this.MenuUp.Add(new Keys[]
				{
					Keys.Up
				});
			}
			if (reset || this.Grab.Keyboard.Count <= 0)
			{
				this.Grab.Keyboard.Clear();
				Binding grab = this.Grab;
				Keys[] array = new Keys[3];
				RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.66336979A2F1495881DE511DADF5E095C9571DCC).FieldHandle);
				grab.Add(array);
			}
			if (reset || this.Jump.Keyboard.Count <= 0)
			{
				this.Jump.Keyboard.Clear();
				this.Jump.Add(new Keys[]
				{
					Keys.C
				});
			}
			if (reset || this.Dash.Keyboard.Count <= 0)
			{
				this.Dash.Keyboard.Clear();
				this.Dash.Add(new Keys[]
				{
					Keys.X
				});
			}
			if (reset || this.Talk.Keyboard.Count <= 0)
			{
				this.Talk.Keyboard.Clear();
				this.Talk.Add(new Keys[]
				{
					Keys.X
				});
			}
			if (reset || this.Pause.Keyboard.Count <= 0)
			{
				this.Pause.Keyboard.Clear();
				this.Pause.Add(new Keys[]
				{
					Keys.Enter
				});
			}
			if (reset || this.Confirm.Keyboard.Count <= 0)
			{
				this.Confirm.Keyboard.Clear();
				this.Confirm.Add(new Keys[]
				{
					Keys.C
				});
			}
			if (reset || this.Cancel.Keyboard.Count <= 0)
			{
				this.Cancel.Keyboard.Clear();
				this.Cancel.Add(new Keys[]
				{
					Keys.X,
					Keys.Back
				});
			}
			if (reset || this.Journal.Keyboard.Count <= 0)
			{
				this.Journal.Keyboard.Clear();
				this.Journal.Add(new Keys[]
				{
					Keys.Tab
				});
			}
			if (reset || this.QuickRestart.Keyboard.Count <= 0)
			{
				this.QuickRestart.Keyboard.Clear();
				this.QuickRestart.Add(new Keys[]
				{
					Keys.R
				});
			}
			if (reset)
			{
				this.DemoDash.Keyboard.Clear();
				this.LeftMoveOnly.Keyboard.Clear();
				this.RightMoveOnly.Keyboard.Clear();
				this.UpMoveOnly.Keyboard.Clear();
				this.DownMoveOnly.Keyboard.Clear();
				this.LeftDashOnly.Keyboard.Clear();
				this.RightDashOnly.Keyboard.Clear();
				this.UpDashOnly.Keyboard.Clear();
				this.DownDashOnly.Keyboard.Clear();
			}
		}

		// Token: 0x0600193F RID: 6463 RVA: 0x000A1CE4 File Offset: 0x0009FEE4
		public void SetDefaultButtonControls(bool reset)
		{
			if (reset || this.Left.Controller.Count <= 0)
			{
				this.Left.Controller.Clear();
				this.Left.Add(new Buttons[]
				{
					Buttons.LeftThumbstickLeft,
					Buttons.DPadLeft
				});
			}
			if (reset || this.Right.Controller.Count <= 0)
			{
				this.Right.Controller.Clear();
				this.Right.Add(new Buttons[]
				{
					Buttons.LeftThumbstickRight,
					Buttons.DPadRight
				});
			}
			if (reset || this.Down.Controller.Count <= 0)
			{
				this.Down.Controller.Clear();
				this.Down.Add(new Buttons[]
				{
					Buttons.LeftThumbstickDown,
					Buttons.DPadDown
				});
			}
			if (reset || this.Up.Controller.Count <= 0)
			{
				this.Up.Controller.Clear();
				this.Up.Add(new Buttons[]
				{
					Buttons.LeftThumbstickUp,
					Buttons.DPadUp
				});
			}
			if (reset || this.MenuLeft.Controller.Count <= 0)
			{
				this.MenuLeft.Controller.Clear();
				this.MenuLeft.Add(new Buttons[]
				{
					Buttons.LeftThumbstickLeft,
					Buttons.DPadLeft
				});
			}
			if (reset || this.MenuRight.Controller.Count <= 0)
			{
				this.MenuRight.Controller.Clear();
				this.MenuRight.Add(new Buttons[]
				{
					Buttons.LeftThumbstickRight,
					Buttons.DPadRight
				});
			}
			if (reset || this.MenuDown.Controller.Count <= 0)
			{
				this.MenuDown.Controller.Clear();
				this.MenuDown.Add(new Buttons[]
				{
					Buttons.LeftThumbstickDown,
					Buttons.DPadDown
				});
			}
			if (reset || this.MenuUp.Controller.Count <= 0)
			{
				this.MenuUp.Controller.Clear();
				this.MenuUp.Add(new Buttons[]
				{
					Buttons.LeftThumbstickUp,
					Buttons.DPadUp
				});
			}
			if (reset || this.Grab.Controller.Count <= 0)
			{
				this.Grab.Controller.Clear();
				Binding grab = this.Grab;
				Buttons[] array = new Buttons[4];
				RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.43C95568B0DD539C3C647B8B9A44D6F5969D5C4C).FieldHandle);
				grab.Add(array);
			}
			if (reset || this.Jump.Controller.Count <= 0)
			{
				this.Jump.Controller.Clear();
				this.Jump.Add(new Buttons[]
				{
					Buttons.A,
					Buttons.Y
				});
			}
			if (reset || this.Dash.Controller.Count <= 0)
			{
				this.Dash.Controller.Clear();
				this.Dash.Add(new Buttons[]
				{
					Buttons.X,
					Buttons.B
				});
			}
			if (reset || this.Talk.Controller.Count <= 0)
			{
				this.Talk.Controller.Clear();
				this.Talk.Add(new Buttons[]
				{
					Buttons.B
				});
			}
			if (reset || this.Pause.Controller.Count <= 0)
			{
				this.Pause.Controller.Clear();
				this.Pause.Add(new Buttons[]
				{
					Buttons.Start
				});
			}
			if (reset || this.Confirm.Controller.Count <= 0)
			{
				this.Confirm.Controller.Clear();
				this.Confirm.Add(new Buttons[]
				{
					Buttons.A
				});
			}
			if (reset || this.Cancel.Controller.Count <= 0)
			{
				this.Cancel.Controller.Clear();
				this.Cancel.Add(new Buttons[]
				{
					Buttons.B,
					Buttons.X
				});
			}
			if (reset || this.Journal.Controller.Count <= 0)
			{
				this.Journal.Controller.Clear();
				this.Journal.Add(new Buttons[]
				{
					Buttons.LeftTrigger
				});
			}
			if (reset || this.QuickRestart.Controller.Count <= 0)
			{
				this.QuickRestart.Controller.Clear();
			}
			if (reset)
			{
				this.DemoDash.Controller.Clear();
				this.LeftMoveOnly.Controller.Clear();
				this.RightMoveOnly.Controller.Clear();
				this.UpMoveOnly.Controller.Clear();
				this.DownMoveOnly.Controller.Clear();
				this.LeftDashOnly.Controller.Clear();
				this.RightDashOnly.Controller.Clear();
				this.UpDashOnly.Controller.Clear();
				this.DownDashOnly.Controller.Clear();
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06001940 RID: 6464 RVA: 0x000A21E8 File Offset: 0x000A03E8
		public int MaxScale
		{
			get
			{
				return Math.Min(Engine.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Width / 320, Engine.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Height / 180);
			}
		}

		// Token: 0x06001941 RID: 6465 RVA: 0x000A2238 File Offset: 0x000A0438
		public void ApplyVolumes()
		{
			this.ApplySFXVolume();
			this.ApplyMusicVolume();
		}

		// Token: 0x06001942 RID: 6466 RVA: 0x000A2246 File Offset: 0x000A0446
		public void ApplySFXVolume()
		{
			Audio.SfxVolume = (float)this.SFXVolume / 10f;
		}

		// Token: 0x06001943 RID: 6467 RVA: 0x000A225A File Offset: 0x000A045A
		public void ApplyMusicVolume()
		{
			Audio.MusicVolume = (float)this.MusicVolume / 10f;
		}

		// Token: 0x06001944 RID: 6468 RVA: 0x000A226E File Offset: 0x000A046E
		public void ApplyScreen()
		{
			if (this.Fullscreen)
			{
				Engine.ViewPadding = this.ViewportPadding;
				Engine.SetFullscreen();
				return;
			}
			Engine.ViewPadding = 0;
			Engine.SetWindowed(320 * this.WindowScale, 180 * this.WindowScale);
		}

		// Token: 0x06001945 RID: 6469 RVA: 0x000A22AC File Offset: 0x000A04AC
		public void ApplyLanguage()
		{
			if (!Dialog.Languages.ContainsKey(this.Language))
			{
				this.Language = "english";
			}
			Dialog.Language = Dialog.Languages[this.Language];
			Fonts.Load(Dialog.Languages[this.Language].FontFace);
		}

		// Token: 0x06001946 RID: 6470 RVA: 0x000A2306 File Offset: 0x000A0506
		public static void Initialize()
		{
			if (UserIO.Open(UserIO.Mode.Read))
			{
				Settings.Instance = UserIO.Load<Settings>("settings", false);
				UserIO.Close();
			}
			Settings.Existed = (Settings.Instance != null);
			if (Settings.Instance == null)
			{
				Settings.Instance = new Settings();
			}
		}

		// Token: 0x06001947 RID: 6471 RVA: 0x000A2344 File Offset: 0x000A0544
		public static void Reload()
		{
			Settings.Initialize();
			Settings.Instance.AfterLoad();
			Settings.Instance.ApplyVolumes();
			Settings.Instance.ApplyScreen();
			Settings.Instance.ApplyLanguage();
			if (Engine.Scene is Overworld)
			{
				OuiMainMenu ui = (Engine.Scene as Overworld).GetUI<OuiMainMenu>();
				if (ui != null)
				{
					ui.CreateButtons();
				}
			}
		}

		// Token: 0x040015C8 RID: 5576
		public static Settings Instance;

		// Token: 0x040015C9 RID: 5577
		public static bool Existed;

		// Token: 0x040015CA RID: 5578
		public static string LastVersion;

		// Token: 0x040015CB RID: 5579
		public const string EnglishLanguage = "english";

		// Token: 0x040015CC RID: 5580
		public string Version;

		// Token: 0x040015CD RID: 5581
		public string DefaultFileName = "";

		// Token: 0x040015CE RID: 5582
		public bool Fullscreen;

		// Token: 0x040015CF RID: 5583
		public int WindowScale = 6;

		// Token: 0x040015D0 RID: 5584
		public int ViewportPadding;

		// Token: 0x040015D1 RID: 5585
		public bool VSync = true;

		// Token: 0x040015D2 RID: 5586
		public bool DisableFlashes;

		// Token: 0x040015D3 RID: 5587
		public ScreenshakeAmount ScreenShake = ScreenshakeAmount.Half;

		// Token: 0x040015D4 RID: 5588
		public RumbleAmount Rumble = RumbleAmount.On;

		// Token: 0x040015D5 RID: 5589
		public GrabModes GrabMode;

		// Token: 0x040015D6 RID: 5590
		public CrouchDashModes CrouchDashMode;

		// Token: 0x040015D7 RID: 5591
		public int MusicVolume = 10;

		// Token: 0x040015D8 RID: 5592
		public int SFXVolume = 10;

		// Token: 0x040015D9 RID: 5593
		public SpeedrunType SpeedrunClock;

		// Token: 0x040015DA RID: 5594
		public int LastSaveFile;

		// Token: 0x040015DB RID: 5595
		public string Language = "english";

		// Token: 0x040015DC RID: 5596
		public bool Pico8OnMainMenu;

		// Token: 0x040015DD RID: 5597
		public bool SetViewportOnce;

		// Token: 0x040015DE RID: 5598
		public bool VariantsUnlocked;

		// Token: 0x040015DF RID: 5599
		public Binding Left = new Binding();

		// Token: 0x040015E0 RID: 5600
		public Binding Right = new Binding();

		// Token: 0x040015E1 RID: 5601
		public Binding Down = new Binding();

		// Token: 0x040015E2 RID: 5602
		public Binding Up = new Binding();

		// Token: 0x040015E3 RID: 5603
		public Binding MenuLeft = new Binding();

		// Token: 0x040015E4 RID: 5604
		public Binding MenuRight = new Binding();

		// Token: 0x040015E5 RID: 5605
		public Binding MenuDown = new Binding();

		// Token: 0x040015E6 RID: 5606
		public Binding MenuUp = new Binding();

		// Token: 0x040015E7 RID: 5607
		public Binding Grab = new Binding();

		// Token: 0x040015E8 RID: 5608
		public Binding Jump = new Binding();

		// Token: 0x040015E9 RID: 5609
		public Binding Dash = new Binding();

		// Token: 0x040015EA RID: 5610
		public Binding Talk = new Binding();

		// Token: 0x040015EB RID: 5611
		public Binding Pause = new Binding();

		// Token: 0x040015EC RID: 5612
		public Binding Confirm = new Binding();

		// Token: 0x040015ED RID: 5613
		public Binding Cancel = new Binding();

		// Token: 0x040015EE RID: 5614
		public Binding Journal = new Binding();

		// Token: 0x040015EF RID: 5615
		public Binding QuickRestart = new Binding();

		// Token: 0x040015F0 RID: 5616
		public Binding DemoDash = new Binding();

		// Token: 0x040015F1 RID: 5617
		public Binding RightMoveOnly = new Binding();

		// Token: 0x040015F2 RID: 5618
		public Binding LeftMoveOnly = new Binding();

		// Token: 0x040015F3 RID: 5619
		public Binding UpMoveOnly = new Binding();

		// Token: 0x040015F4 RID: 5620
		public Binding DownMoveOnly = new Binding();

		// Token: 0x040015F5 RID: 5621
		public Binding RightDashOnly = new Binding();

		// Token: 0x040015F6 RID: 5622
		public Binding LeftDashOnly = new Binding();

		// Token: 0x040015F7 RID: 5623
		public Binding UpDashOnly = new Binding();

		// Token: 0x040015F8 RID: 5624
		public Binding DownDashOnly = new Binding();

		// Token: 0x040015F9 RID: 5625
		public bool LaunchWithFMODLiveUpdate;

		// Token: 0x040015FA RID: 5626
		public bool LaunchInDebugMode;

		// Token: 0x040015FB RID: 5627
		public const string Filename = "settings";
	}
}
