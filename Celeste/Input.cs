using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x0200035C RID: 860
	public static class Input
	{
		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06001B17 RID: 6935 RVA: 0x000B0AF9 File Offset: 0x000AECF9
		// (set) Token: 0x06001B18 RID: 6936 RVA: 0x000B0B00 File Offset: 0x000AED00
		public static int Gamepad
		{
			get
			{
				return Input.gamepad;
			}
			set
			{
				int num = Calc.Clamp(value, 0, MInput.GamePads.Length - 1);
				if (Input.gamepad != num)
				{
					Input.gamepad = num;
					Input.Initialize();
				}
			}
		}

		// Token: 0x06001B19 RID: 6937 RVA: 0x000B0B34 File Offset: 0x000AED34
		public static void Initialize()
		{
			bool flag = false;
			if (Input.MoveX != null)
			{
				flag = Input.MoveX.Inverted;
			}
			Input.Deregister();
			Input.MoveX = new VirtualIntegerAxis(Settings.Instance.Left, Settings.Instance.LeftMoveOnly, Settings.Instance.Right, Settings.Instance.RightMoveOnly, Input.Gamepad, 0.3f, VirtualInput.OverlapBehaviors.TakeNewer);
			Input.MoveX.Inverted = flag;
			Input.MoveY = new VirtualIntegerAxis(Settings.Instance.Up, Settings.Instance.UpMoveOnly, Settings.Instance.Down, Settings.Instance.DownMoveOnly, Input.Gamepad, 0.7f, VirtualInput.OverlapBehaviors.TakeNewer);
			Input.GliderMoveY = new VirtualIntegerAxis(Settings.Instance.Up, Settings.Instance.UpMoveOnly, Settings.Instance.Down, Settings.Instance.DownMoveOnly, Input.Gamepad, 0.3f, VirtualInput.OverlapBehaviors.TakeNewer);
			Input.Aim = new VirtualJoystick(Settings.Instance.Up, Settings.Instance.UpDashOnly, Settings.Instance.Down, Settings.Instance.DownDashOnly, Settings.Instance.Left, Settings.Instance.LeftDashOnly, Settings.Instance.Right, Settings.Instance.RightDashOnly, Input.Gamepad, 0.25f, VirtualInput.OverlapBehaviors.TakeNewer);
			Input.Aim.InvertedX = flag;
			Input.Feather = new VirtualJoystick(Settings.Instance.Up, Settings.Instance.UpMoveOnly, Settings.Instance.Down, Settings.Instance.DownMoveOnly, Settings.Instance.Left, Settings.Instance.LeftMoveOnly, Settings.Instance.Right, Settings.Instance.RightMoveOnly, Input.Gamepad, 0.25f, VirtualInput.OverlapBehaviors.TakeNewer);
			Input.Feather.InvertedX = flag;
			Input.Jump = new VirtualButton(Settings.Instance.Jump, Input.Gamepad, 0.08f, 0.2f);
			Input.Dash = new VirtualButton(Settings.Instance.Dash, Input.Gamepad, 0.08f, 0.2f);
			Input.Talk = new VirtualButton(Settings.Instance.Talk, Input.Gamepad, 0.08f, 0.2f);
			Input.Grab = new VirtualButton(Settings.Instance.Grab, Input.Gamepad, 0f, 0.2f);
			Input.CrouchDash = new VirtualButton(Settings.Instance.DemoDash, Input.Gamepad, 0.08f, 0.2f);
			Binding binding = new Binding();
			binding.Add(new Keys[]
			{
				Keys.A
			});
			binding.Add(new Buttons[]
			{
				Buttons.RightThumbstickLeft
			});
			Binding binding2 = new Binding();
			binding2.Add(new Keys[]
			{
				Keys.D
			});
			binding2.Add(new Buttons[]
			{
				Buttons.RightThumbstickRight
			});
			Binding binding3 = new Binding();
			binding3.Add(new Keys[]
			{
				Keys.W
			});
			binding3.Add(new Buttons[]
			{
				Buttons.RightThumbstickUp
			});
			Binding binding4 = new Binding();
			binding4.Add(new Keys[]
			{
				Keys.S
			});
			binding4.Add(new Buttons[]
			{
				Buttons.RightThumbstickDown
			});
			Input.MountainAim = new VirtualJoystick(binding3, binding4, binding, binding2, Input.Gamepad, 0.1f, VirtualInput.OverlapBehaviors.TakeNewer);
			Binding binding5 = new Binding();
			binding5.Add(new Keys[]
			{
				Keys.Escape
			});
			Input.ESC = new VirtualButton(binding5, Input.Gamepad, 0.1f, 0.2f);
			Input.Pause = new VirtualButton(Settings.Instance.Pause, Input.Gamepad, 0.1f, 0.2f);
			Input.QuickRestart = new VirtualButton(Settings.Instance.QuickRestart, Input.Gamepad, 0.1f, 0.2f);
			Input.MenuLeft = new VirtualButton(Settings.Instance.MenuLeft, Input.Gamepad, 0f, 0.4f);
			Input.MenuLeft.SetRepeat(0.4f, 0.1f);
			Input.MenuRight = new VirtualButton(Settings.Instance.MenuRight, Input.Gamepad, 0f, 0.4f);
			Input.MenuRight.SetRepeat(0.4f, 0.1f);
			Input.MenuUp = new VirtualButton(Settings.Instance.MenuUp, Input.Gamepad, 0f, 0.4f);
			Input.MenuUp.SetRepeat(0.4f, 0.1f);
			Input.MenuDown = new VirtualButton(Settings.Instance.MenuDown, Input.Gamepad, 0f, 0.4f);
			Input.MenuDown.SetRepeat(0.4f, 0.1f);
			Input.MenuJournal = new VirtualButton(Settings.Instance.Journal, Input.Gamepad, 0f, 0.2f);
			Input.MenuConfirm = new VirtualButton(Settings.Instance.Confirm, Input.Gamepad, 0f, 0.2f);
			Input.MenuCancel = new VirtualButton(Settings.Instance.Cancel, Input.Gamepad, 0f, 0.2f);
		}

		// Token: 0x06001B1A RID: 6938 RVA: 0x000B103C File Offset: 0x000AF23C
		public static void Deregister()
		{
			if (Input.ESC != null)
			{
				Input.ESC.Deregister();
			}
			if (Input.Pause != null)
			{
				Input.Pause.Deregister();
			}
			if (Input.MenuLeft != null)
			{
				Input.MenuLeft.Deregister();
			}
			if (Input.MenuRight != null)
			{
				Input.MenuRight.Deregister();
			}
			if (Input.MenuUp != null)
			{
				Input.MenuUp.Deregister();
			}
			if (Input.MenuDown != null)
			{
				Input.MenuDown.Deregister();
			}
			if (Input.MenuConfirm != null)
			{
				Input.MenuConfirm.Deregister();
			}
			if (Input.MenuCancel != null)
			{
				Input.MenuCancel.Deregister();
			}
			if (Input.MenuJournal != null)
			{
				Input.MenuJournal.Deregister();
			}
			if (Input.QuickRestart != null)
			{
				Input.QuickRestart.Deregister();
			}
			if (Input.MoveX != null)
			{
				Input.MoveX.Deregister();
			}
			if (Input.MoveY != null)
			{
				Input.MoveY.Deregister();
			}
			if (Input.GliderMoveY != null)
			{
				Input.GliderMoveY.Deregister();
			}
			if (Input.Aim != null)
			{
				Input.Aim.Deregister();
			}
			if (Input.MountainAim != null)
			{
				Input.MountainAim.Deregister();
			}
			if (Input.Jump != null)
			{
				Input.Jump.Deregister();
			}
			if (Input.Dash != null)
			{
				Input.Dash.Deregister();
			}
			if (Input.Grab != null)
			{
				Input.Grab.Deregister();
			}
			if (Input.Talk != null)
			{
				Input.Talk.Deregister();
			}
			if (Input.CrouchDash != null)
			{
				Input.CrouchDash.Deregister();
			}
		}

		// Token: 0x06001B1B RID: 6939 RVA: 0x000B11A0 File Offset: 0x000AF3A0
		public static bool AnyGamepadConfirmPressed(out int gamepadIndex)
		{
			bool result = false;
			gamepadIndex = -1;
			int gamepadIndex2 = Input.MenuConfirm.GamepadIndex;
			for (int i = 0; i < MInput.GamePads.Length; i++)
			{
				Input.MenuConfirm.GamepadIndex = i;
				if (Input.MenuConfirm.Pressed)
				{
					result = true;
					gamepadIndex = i;
					break;
				}
			}
			Input.MenuConfirm.GamepadIndex = gamepadIndex2;
			return result;
		}

		// Token: 0x06001B1C RID: 6940 RVA: 0x000B11FC File Offset: 0x000AF3FC
		public static void Rumble(RumbleStrength strength, RumbleLength length)
		{
			float num = 1f;
			if (Settings.Instance.Rumble == RumbleAmount.Half)
			{
				num = 0.5f;
			}
			if (Settings.Instance.Rumble != RumbleAmount.Off && MInput.GamePads.Length != 0 && !MInput.Disabled)
			{
				MInput.GamePads[Input.Gamepad].Rumble(Input.rumbleStrengths[(int)strength] * num, Input.rumbleLengths[(int)length]);
			}
		}

		// Token: 0x06001B1D RID: 6941 RVA: 0x000B1260 File Offset: 0x000AF460
		public static void RumbleSpecific(float strength, float time)
		{
			float num = 1f;
			if (Settings.Instance.Rumble == RumbleAmount.Half)
			{
				num = 0.5f;
			}
			if (Settings.Instance.Rumble != RumbleAmount.Off && MInput.GamePads.Length != 0 && !MInput.Disabled)
			{
				MInput.GamePads[Input.Gamepad].Rumble(strength * num, time);
			}
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06001B1E RID: 6942 RVA: 0x000B12B8 File Offset: 0x000AF4B8
		public static bool GrabCheck
		{
			get
			{
				switch (Settings.Instance.GrabMode)
				{
				default:
					return Input.Grab.Check;
				case GrabModes.Invert:
					return !Input.Grab.Check;
				case GrabModes.Toggle:
					return Input.grabToggle;
				}
			}
		}

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06001B1F RID: 6943 RVA: 0x000B1300 File Offset: 0x000AF500
		public static bool DashPressed
		{
			get
			{
				CrouchDashModes crouchDashMode = Settings.Instance.CrouchDashMode;
				if (crouchDashMode == CrouchDashModes.Press || crouchDashMode != CrouchDashModes.Hold)
				{
					return Input.Dash.Pressed;
				}
				return Input.Dash.Pressed && !Input.CrouchDash.Check;
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06001B20 RID: 6944 RVA: 0x000B1348 File Offset: 0x000AF548
		public static bool CrouchDashPressed
		{
			get
			{
				CrouchDashModes crouchDashMode = Settings.Instance.CrouchDashMode;
				if (crouchDashMode == CrouchDashModes.Press || crouchDashMode != CrouchDashModes.Hold)
				{
					return Input.CrouchDash.Pressed;
				}
				return Input.Dash.Pressed && Input.CrouchDash.Check;
			}
		}

		// Token: 0x06001B21 RID: 6945 RVA: 0x000B138A File Offset: 0x000AF58A
		public static void UpdateGrab()
		{
			if (Settings.Instance.GrabMode == GrabModes.Toggle && Input.Grab.Pressed)
			{
				Input.grabToggle = !Input.grabToggle;
			}
		}

		// Token: 0x06001B22 RID: 6946 RVA: 0x000B13B2 File Offset: 0x000AF5B2
		public static void ResetGrab()
		{
			Input.grabToggle = false;
		}

		// Token: 0x06001B23 RID: 6947 RVA: 0x000B13BC File Offset: 0x000AF5BC
		public static Vector2 GetAimVector(Facings defaultFacing = Facings.Right)
		{
			Vector2 value = Input.Aim.Value;
			if (value == Vector2.Zero)
			{
				if (SaveData.Instance != null && SaveData.Instance.Assists.DashAssist)
				{
					return Input.LastAim;
				}
				Input.LastAim = Vector2.UnitX * (float)defaultFacing;
			}
			else if (SaveData.Instance != null && SaveData.Instance.Assists.ThreeSixtyDashing)
			{
				Input.LastAim = value.SafeNormalize();
			}
			else
			{
				float num = value.Angle();
				int num2 = (num < 0f) ? 1 : 0;
				float num3 = 0.3926991f - (float)num2 * 0.08726646f;
				if (Calc.AbsAngleDiff(num, 0f) < num3)
				{
					Input.LastAim = new Vector2(1f, 0f);
				}
				else if (Calc.AbsAngleDiff(num, 3.1415927f) < num3)
				{
					Input.LastAim = new Vector2(-1f, 0f);
				}
				else if (Calc.AbsAngleDiff(num, -1.5707964f) < num3)
				{
					Input.LastAim = new Vector2(0f, -1f);
				}
				else if (Calc.AbsAngleDiff(num, 1.5707964f) < num3)
				{
					Input.LastAim = new Vector2(0f, 1f);
				}
				else
				{
					Input.LastAim = new Vector2((float)Math.Sign(value.X), (float)Math.Sign(value.Y)).SafeNormalize();
				}
			}
			return Input.LastAim;
		}

		// Token: 0x06001B24 RID: 6948 RVA: 0x000B1520 File Offset: 0x000AF720
		public static string GuiInputPrefix(Input.PrefixMode mode = Input.PrefixMode.Latest)
		{
			if (!string.IsNullOrEmpty(Input.OverrideInputPrefix))
			{
				return Input.OverrideInputPrefix;
			}
			bool flag;
			if (mode == Input.PrefixMode.Latest)
			{
				flag = MInput.ControllerHasFocus;
			}
			else
			{
				flag = MInput.GamePads[Input.Gamepad].Attached;
			}
			if (flag)
			{
				return "xb1";
			}
			return "keyboard";
		}

		// Token: 0x06001B25 RID: 6949 RVA: 0x000B156B File Offset: 0x000AF76B
		public static bool GuiInputController(Input.PrefixMode mode = Input.PrefixMode.Latest)
		{
			return !Input.GuiInputPrefix(mode).Equals("keyboard");
		}

		// Token: 0x06001B26 RID: 6950 RVA: 0x000B1580 File Offset: 0x000AF780
		public static MTexture GuiButton(VirtualButton button, Input.PrefixMode mode = Input.PrefixMode.Latest, string fallback = "controls/keyboard/oemquestion")
		{
			string prefix = Input.GuiInputPrefix(mode);
			bool flag = Input.GuiInputController(mode);
			string input = "";
			if (flag)
			{
				using (List<Buttons>.Enumerator enumerator = button.Binding.Controller.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						Buttons key = enumerator.Current;
						if (!Input.buttonNameLookup.TryGetValue(key, out input))
						{
							Input.buttonNameLookup.Add(key, input = key.ToString());
						}
					}
					goto IL_AA;
				}
			}
			Keys key2 = Input.FirstKey(button);
			if (!Input.keyNameLookup.TryGetValue(key2, out input))
			{
				Input.keyNameLookup.Add(key2, input = key2.ToString());
			}
			IL_AA:
			MTexture mtexture = Input.GuiTexture(prefix, input);
			if (mtexture == null && fallback != null)
			{
				return GFX.Gui[fallback];
			}
			return mtexture;
		}

		// Token: 0x06001B27 RID: 6951 RVA: 0x000B1664 File Offset: 0x000AF864
		public static MTexture GuiSingleButton(Buttons button, Input.PrefixMode mode = Input.PrefixMode.Latest, string fallback = "controls/keyboard/oemquestion")
		{
			string prefix;
			if (Input.GuiInputController(mode))
			{
				prefix = Input.GuiInputPrefix(mode);
			}
			else
			{
				prefix = "xb1";
			}
			string input = "";
			if (!Input.buttonNameLookup.TryGetValue(button, out input))
			{
				Input.buttonNameLookup.Add(button, input = button.ToString());
			}
			MTexture mtexture = Input.GuiTexture(prefix, input);
			if (mtexture == null && fallback != null)
			{
				return GFX.Gui[fallback];
			}
			return mtexture;
		}

		// Token: 0x06001B28 RID: 6952 RVA: 0x000B16D4 File Offset: 0x000AF8D4
		public static MTexture GuiKey(Keys key, string fallback = "controls/keyboard/oemquestion")
		{
			string input;
			if (!Input.keyNameLookup.TryGetValue(key, out input))
			{
				Input.keyNameLookup.Add(key, input = key.ToString());
			}
			MTexture mtexture = Input.GuiTexture("keyboard", input);
			if (mtexture == null && fallback != null)
			{
				return GFX.Gui[fallback];
			}
			return mtexture;
		}

		// Token: 0x06001B29 RID: 6953 RVA: 0x000B172C File Offset: 0x000AF92C
		public static Buttons FirstButton(VirtualButton button)
		{
			using (List<Buttons>.Enumerator enumerator = button.Binding.Controller.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return Buttons.A;
		}

		// Token: 0x06001B2A RID: 6954 RVA: 0x000B1788 File Offset: 0x000AF988
		public static Keys FirstKey(VirtualButton button)
		{
			foreach (Keys keys in button.Binding.Keyboard)
			{
				if (keys != Keys.None)
				{
					return keys;
				}
			}
			return Keys.None;
		}

		// Token: 0x06001B2B RID: 6955 RVA: 0x000B17E4 File Offset: 0x000AF9E4
		public static MTexture GuiDirection(Vector2 direction)
		{
			int num = Math.Sign(direction.X);
			int num2 = Math.Sign(direction.Y);
			string input = num + "x" + num2;
			return Input.GuiTexture("directions", input);
		}

		// Token: 0x06001B2C RID: 6956 RVA: 0x000B182C File Offset: 0x000AFA2C
		private static MTexture GuiTexture(string prefix, string input)
		{
			Dictionary<string, string> dictionary;
			if (!Input.guiPathLookup.TryGetValue(prefix, out dictionary))
			{
				Input.guiPathLookup.Add(prefix, dictionary = new Dictionary<string, string>());
			}
			string id;
			if (!dictionary.TryGetValue(input, out id))
			{
				dictionary.Add(input, id = "controls/" + prefix + "/" + input);
			}
			if (GFX.Gui.Has(id))
			{
				return GFX.Gui[id];
			}
			if (prefix != "fallback")
			{
				return Input.GuiTexture("fallback", input);
			}
			return null;
		}

		// Token: 0x06001B2D RID: 6957 RVA: 0x000091E2 File Offset: 0x000073E2
		public static void SetLightbarColor(Color color)
		{
		}

		// Token: 0x040017E4 RID: 6116
		private static int gamepad = 0;

		// Token: 0x040017E5 RID: 6117
		public static readonly int MaxBindings = 8;

		// Token: 0x040017E6 RID: 6118
		public static VirtualButton ESC;

		// Token: 0x040017E7 RID: 6119
		public static VirtualButton Pause;

		// Token: 0x040017E8 RID: 6120
		public static VirtualButton MenuLeft;

		// Token: 0x040017E9 RID: 6121
		public static VirtualButton MenuRight;

		// Token: 0x040017EA RID: 6122
		public static VirtualButton MenuUp;

		// Token: 0x040017EB RID: 6123
		public static VirtualButton MenuDown;

		// Token: 0x040017EC RID: 6124
		public static VirtualButton MenuConfirm;

		// Token: 0x040017ED RID: 6125
		public static VirtualButton MenuCancel;

		// Token: 0x040017EE RID: 6126
		public static VirtualButton MenuJournal;

		// Token: 0x040017EF RID: 6127
		public static VirtualButton QuickRestart;

		// Token: 0x040017F0 RID: 6128
		public static VirtualIntegerAxis MoveX;

		// Token: 0x040017F1 RID: 6129
		public static VirtualIntegerAxis MoveY;

		// Token: 0x040017F2 RID: 6130
		public static VirtualIntegerAxis GliderMoveY;

		// Token: 0x040017F3 RID: 6131
		public static VirtualJoystick Aim;

		// Token: 0x040017F4 RID: 6132
		public static VirtualJoystick Feather;

		// Token: 0x040017F5 RID: 6133
		public static VirtualJoystick MountainAim;

		// Token: 0x040017F6 RID: 6134
		public static VirtualButton Jump;

		// Token: 0x040017F7 RID: 6135
		public static VirtualButton Dash;

		// Token: 0x040017F8 RID: 6136
		public static VirtualButton Grab;

		// Token: 0x040017F9 RID: 6137
		public static VirtualButton Talk;

		// Token: 0x040017FA RID: 6138
		public static VirtualButton CrouchDash;

		// Token: 0x040017FB RID: 6139
		private static bool grabToggle;

		// Token: 0x040017FC RID: 6140
		public static Vector2 LastAim;

		// Token: 0x040017FD RID: 6141
		public static string OverrideInputPrefix = null;

		// Token: 0x040017FE RID: 6142
		private static Dictionary<Keys, string> keyNameLookup = new Dictionary<Keys, string>();

		// Token: 0x040017FF RID: 6143
		private static Dictionary<Buttons, string> buttonNameLookup = new Dictionary<Buttons, string>();

		// Token: 0x04001800 RID: 6144
		private static Dictionary<string, Dictionary<string, string>> guiPathLookup = new Dictionary<string, Dictionary<string, string>>();

		// Token: 0x04001801 RID: 6145
		private static float[] rumbleStrengths = new float[]
		{
			0.15f,
			0.4f,
			1f,
			0.05f
		};

		// Token: 0x04001802 RID: 6146
		private static float[] rumbleLengths = new float[]
		{
			0.1f,
			0.25f,
			0.5f,
			1f,
			2f
		};

		// Token: 0x02000717 RID: 1815
		public enum PrefixMode
		{
			// Token: 0x04002DA8 RID: 11688
			Latest,
			// Token: 0x04002DA9 RID: 11689
			Attached
		}
	}
}
