using System;
using FMOD.Studio;
using Monocle;

namespace Celeste
{
	// Token: 0x02000314 RID: 788
	public static class MenuOptions
	{
		// Token: 0x060018E8 RID: 6376 RVA: 0x0009C1A4 File Offset: 0x0009A3A4
		public static TextMenu Create(bool inGame = false, EventInstance snapshot = null)
		{
			MenuOptions.inGame = inGame;
			MenuOptions.snapshot = snapshot;
			MenuOptions.menu = new TextMenu();
			MenuOptions.menu.Add(new TextMenu.Header(Dialog.Clean("options_title", null)));
			MenuOptions.menu.OnClose = delegate()
			{
				MenuOptions.crouchDashMode = null;
			};
			if (!inGame && Dialog.Languages.Count > 1)
			{
				MenuOptions.menu.Add(new TextMenu.SubHeader("", true));
				TextMenu.LanguageButton languageButton = new TextMenu.LanguageButton(Dialog.Clean("options_language", null), Dialog.Language);
				languageButton.Pressed(new Action(MenuOptions.SelectLanguage));
				MenuOptions.menu.Add(languageButton);
			}
			MenuOptions.menu.Add(new TextMenu.SubHeader(Dialog.Clean("options_controls", null), true));
			MenuOptions.CreateRumble(MenuOptions.menu);
			MenuOptions.CreateGrabMode(MenuOptions.menu);
			MenuOptions.crouchDashMode = MenuOptions.CreateCrouchDashMode(MenuOptions.menu);
			MenuOptions.menu.Add(new TextMenu.Button(Dialog.Clean("options_keyconfig", null)).Pressed(new Action(MenuOptions.OpenKeyboardConfig)));
			MenuOptions.menu.Add(new TextMenu.Button(Dialog.Clean("options_btnconfig", null)).Pressed(new Action(MenuOptions.OpenButtonConfig)));
			MenuOptions.menu.Add(new TextMenu.SubHeader(Dialog.Clean("options_video", null), true));
			MenuOptions.menu.Add(new TextMenu.OnOff(Dialog.Clean("options_fullscreen", null), Settings.Instance.Fullscreen).Change(new Action<bool>(MenuOptions.SetFullscreen)));
			MenuOptions.menu.Add(MenuOptions.window = new TextMenu.Slider(Dialog.Clean("options_window", null), (int i) => i + "x", 3, 10, Settings.Instance.WindowScale).Change(new Action<int>(MenuOptions.SetWindow)));
			MenuOptions.menu.Add(new TextMenu.OnOff(Dialog.Clean("options_vsync", null), Settings.Instance.VSync).Change(new Action<bool>(MenuOptions.SetVSync)));
			MenuOptions.menu.Add(new TextMenu.OnOff(Dialog.Clean("OPTIONS_DISABLE_FLASH", null), Settings.Instance.DisableFlashes).Change(delegate(bool b)
			{
				Settings.Instance.DisableFlashes = b;
			}));
			MenuOptions.menu.Add(new TextMenu.Slider(Dialog.Clean("OPTIONS_DISABLE_SHAKE", null), delegate(int i)
			{
				if (i == 2)
				{
					return Dialog.Clean("OPTIONS_RUMBLE_ON", null);
				}
				if (i == 1)
				{
					return Dialog.Clean("OPTIONS_RUMBLE_HALF", null);
				}
				return Dialog.Clean("OPTIONS_RUMBLE_OFF", null);
			}, 0, 2, (int)Settings.Instance.ScreenShake).Change(delegate(int i)
			{
				Settings.Instance.ScreenShake = (ScreenshakeAmount)i;
			}));
			MenuOptions.menu.Add(MenuOptions.viewport = new TextMenu.Button(Dialog.Clean("OPTIONS_VIEWPORT_PC", null)).Pressed(new Action(MenuOptions.OpenViewportAdjustment)));
			MenuOptions.menu.Add(new TextMenu.SubHeader(Dialog.Clean("options_audio", null), true));
			MenuOptions.menu.Add(new TextMenu.Slider(Dialog.Clean("options_music", null), (int i) => i.ToString(), 0, 10, Settings.Instance.MusicVolume).Change(new Action<int>(MenuOptions.SetMusic)).Enter(new Action(MenuOptions.EnterSound)).Leave(new Action(MenuOptions.LeaveSound)));
			MenuOptions.menu.Add(new TextMenu.Slider(Dialog.Clean("options_sounds", null), (int i) => i.ToString(), 0, 10, Settings.Instance.SFXVolume).Change(new Action<int>(MenuOptions.SetSfx)).Enter(new Action(MenuOptions.EnterSound)).Leave(new Action(MenuOptions.LeaveSound)));
			MenuOptions.menu.Add(new TextMenu.SubHeader(Dialog.Clean("options_gameplay", null), true));
			MenuOptions.menu.Add(new TextMenu.Slider(Dialog.Clean("options_speedrun", null), delegate(int i)
			{
				if (i == 0)
				{
					return Dialog.Get("OPTIONS_OFF", null);
				}
				if (i == 1)
				{
					return Dialog.Get("OPTIONS_SPEEDRUN_CHAPTER", null);
				}
				return Dialog.Get("OPTIONS_SPEEDRUN_FILE", null);
			}, 0, 2, (int)Settings.Instance.SpeedrunClock).Change(new Action<int>(MenuOptions.SetSpeedrunClock)));
			MenuOptions.viewport.Visible = Settings.Instance.Fullscreen;
			if (MenuOptions.window != null)
			{
				MenuOptions.window.Visible = !Settings.Instance.Fullscreen;
			}
			if (MenuOptions.menu.Height > MenuOptions.menu.ScrollableMinSize)
			{
				MenuOptions.menu.Position.Y = MenuOptions.menu.ScrollTargetY;
			}
			return MenuOptions.menu;
		}

		// Token: 0x060018E9 RID: 6377 RVA: 0x0009C6B4 File Offset: 0x0009A8B4
		private static void CreateRumble(TextMenu menu)
		{
			menu.Add(new TextMenu.Slider(Dialog.Clean("options_rumble_PC", null), delegate(int i)
			{
				if (i == 2)
				{
					return Dialog.Clean("OPTIONS_RUMBLE_ON", null);
				}
				if (i == 1)
				{
					return Dialog.Clean("OPTIONS_RUMBLE_HALF", null);
				}
				return Dialog.Clean("OPTIONS_RUMBLE_OFF", null);
			}, 0, 2, (int)Settings.Instance.Rumble).Change(delegate(int i)
			{
				Settings.Instance.Rumble = (RumbleAmount)i;
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			}));
		}

		// Token: 0x060018EA RID: 6378 RVA: 0x0009C728 File Offset: 0x0009A928
		private static void CreateGrabMode(TextMenu menu)
		{
			menu.Add(new TextMenu.Slider(Dialog.Clean("OPTIONS_GRAB_MODE", null), delegate(int i)
			{
				if (i == 0)
				{
					return Dialog.Clean("OPTIONS_BUTTON_HOLD", null);
				}
				if (i == 1)
				{
					return Dialog.Clean("OPTIONS_BUTTON_INVERT", null);
				}
				return Dialog.Clean("OPTIONS_BUTTON_TOGGLE", null);
			}, 0, 2, (int)Settings.Instance.GrabMode).Change(delegate(int i)
			{
				Settings.Instance.GrabMode = (GrabModes)i;
				Input.ResetGrab();
			}));
		}

		// Token: 0x060018EB RID: 6379 RVA: 0x0009C79C File Offset: 0x0009A99C
		private static TextMenu.Item CreateCrouchDashMode(TextMenu menu)
		{
			TextMenu.Option<int> option = new TextMenu.Slider(Dialog.Clean("OPTIONS_CROUCH_DASH_MODE", null), delegate(int i)
			{
				if (i == 0)
				{
					return Dialog.Clean("OPTIONS_BUTTON_PRESS", null);
				}
				return Dialog.Clean("OPTIONS_BUTTON_HOLD", null);
			}, 0, 1, (int)Settings.Instance.CrouchDashMode).Change(delegate(int i)
			{
				Settings.Instance.CrouchDashMode = (CrouchDashModes)i;
			});
			option.Visible = Input.CrouchDash.Binding.HasInput;
			menu.Add(option);
			return option;
		}

		// Token: 0x060018EC RID: 6380 RVA: 0x0009C827 File Offset: 0x0009AA27
		private static void SetFullscreen(bool on)
		{
			Settings.Instance.Fullscreen = on;
			Settings.Instance.ApplyScreen();
			if (MenuOptions.window != null)
			{
				MenuOptions.window.Visible = !on;
			}
			if (MenuOptions.viewport != null)
			{
				MenuOptions.viewport.Visible = on;
			}
		}

		// Token: 0x060018ED RID: 6381 RVA: 0x0009C865 File Offset: 0x0009AA65
		private static void SetVSync(bool on)
		{
			Settings.Instance.VSync = on;
			Engine.Graphics.SynchronizeWithVerticalRetrace = Settings.Instance.VSync;
			Engine.Graphics.ApplyChanges();
		}

		// Token: 0x060018EE RID: 6382 RVA: 0x0009C890 File Offset: 0x0009AA90
		private static void SetWindow(int scale)
		{
			Settings.Instance.WindowScale = scale;
			Settings.Instance.ApplyScreen();
		}

		// Token: 0x060018EF RID: 6383 RVA: 0x0009C8A7 File Offset: 0x0009AAA7
		private static void SetMusic(int volume)
		{
			Settings.Instance.MusicVolume = volume;
			Settings.Instance.ApplyMusicVolume();
		}

		// Token: 0x060018F0 RID: 6384 RVA: 0x0009C8BE File Offset: 0x0009AABE
		private static void SetSfx(int volume)
		{
			Settings.Instance.SFXVolume = volume;
			Settings.Instance.ApplySFXVolume();
		}

		// Token: 0x060018F1 RID: 6385 RVA: 0x0009C8D5 File Offset: 0x0009AAD5
		private static void SetSpeedrunClock(int val)
		{
			Settings.Instance.SpeedrunClock = (SpeedrunType)val;
		}

		// Token: 0x060018F2 RID: 6386 RVA: 0x0009C8E4 File Offset: 0x0009AAE4
		private static void OpenViewportAdjustment()
		{
			if (Engine.Scene is Overworld)
			{
				(Engine.Scene as Overworld).ShowInputUI = false;
			}
			MenuOptions.menu.Visible = false;
			MenuOptions.menu.Focused = false;
			ViewportAdjustmentUI viewportAdjustmentUI = new ViewportAdjustmentUI();
			viewportAdjustmentUI.OnClose = delegate()
			{
				MenuOptions.menu.Visible = true;
				MenuOptions.menu.Focused = true;
				if (Engine.Scene is Overworld)
				{
					(Engine.Scene as Overworld).ShowInputUI = true;
				}
			};
			Engine.Scene.Add(viewportAdjustmentUI);
			Engine.Scene.OnEndOfFrame += delegate()
			{
				Engine.Scene.Entities.UpdateLists();
			};
		}

		// Token: 0x060018F3 RID: 6387 RVA: 0x0009C984 File Offset: 0x0009AB84
		private static void SelectLanguage()
		{
			MenuOptions.menu.Focused = false;
			LanguageSelectUI languageSelectUI = new LanguageSelectUI();
			languageSelectUI.OnClose = delegate()
			{
				MenuOptions.menu.Focused = true;
			};
			Engine.Scene.Add(languageSelectUI);
			Engine.Scene.OnEndOfFrame += delegate()
			{
				Engine.Scene.Entities.UpdateLists();
			};
		}

		// Token: 0x060018F4 RID: 6388 RVA: 0x0009C9FC File Offset: 0x0009ABFC
		private static void OpenKeyboardConfig()
		{
			MenuOptions.menu.Focused = false;
			KeyboardConfigUI keyboardConfigUI = new KeyboardConfigUI();
			keyboardConfigUI.OnClose = delegate()
			{
				MenuOptions.menu.Focused = true;
			};
			Engine.Scene.Add(keyboardConfigUI);
			Engine.Scene.OnEndOfFrame += delegate()
			{
				Engine.Scene.Entities.UpdateLists();
			};
		}

		// Token: 0x060018F5 RID: 6389 RVA: 0x0009CA74 File Offset: 0x0009AC74
		private static void OpenButtonConfig()
		{
			MenuOptions.menu.Focused = false;
			if (Engine.Scene is Overworld)
			{
				(Engine.Scene as Overworld).ShowConfirmUI = false;
			}
			ButtonConfigUI buttonConfigUI = new ButtonConfigUI();
			buttonConfigUI.OnClose = delegate()
			{
				MenuOptions.menu.Focused = true;
				if (Engine.Scene is Overworld)
				{
					(Engine.Scene as Overworld).ShowConfirmUI = true;
				}
			};
			Engine.Scene.Add(buttonConfigUI);
			Engine.Scene.OnEndOfFrame += delegate()
			{
				Engine.Scene.Entities.UpdateLists();
			};
		}

		// Token: 0x060018F6 RID: 6390 RVA: 0x0009CB07 File Offset: 0x0009AD07
		private static void EnterSound()
		{
			if (MenuOptions.inGame && MenuOptions.snapshot != null)
			{
				Audio.EndSnapshot(MenuOptions.snapshot);
			}
		}

		// Token: 0x060018F7 RID: 6391 RVA: 0x0009CB27 File Offset: 0x0009AD27
		private static void LeaveSound()
		{
			if (MenuOptions.inGame && MenuOptions.snapshot != null)
			{
				Audio.ResumeSnapshot(MenuOptions.snapshot);
			}
		}

		// Token: 0x060018F8 RID: 6392 RVA: 0x0009CB47 File Offset: 0x0009AD47
		public static void UpdateCrouchDashModeVisibility()
		{
			if (MenuOptions.crouchDashMode != null)
			{
				MenuOptions.crouchDashMode.Visible = Input.CrouchDash.Binding.HasInput;
			}
		}

		// Token: 0x04001555 RID: 5461
		private static TextMenu menu;

		// Token: 0x04001556 RID: 5462
		private static bool inGame;

		// Token: 0x04001557 RID: 5463
		private static TextMenu.Item crouchDashMode;

		// Token: 0x04001558 RID: 5464
		private static TextMenu.Item window;

		// Token: 0x04001559 RID: 5465
		private static TextMenu.Item viewport;

		// Token: 0x0400155A RID: 5466
		private static EventInstance snapshot;
	}
}
