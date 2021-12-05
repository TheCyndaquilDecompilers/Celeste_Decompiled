using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x020001FC RID: 508
	[Tracked(false)]
	public class ButtonConfigUI : TextMenu
	{
		// Token: 0x060010A8 RID: 4264 RVA: 0x0004D5D4 File Offset: 0x0004B7D4
		public ButtonConfigUI()
		{
			base.Add(new TextMenu.Header(Dialog.Clean("BTN_CONFIG_TITLE", null)));
			base.Add(new InputMappingInfo(true));
			base.Add(new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_GAMEPLAY", null), true));
			this.AddMap("LEFT", Settings.Instance.Left);
			this.AddMap("RIGHT", Settings.Instance.Right);
			this.AddMap("UP", Settings.Instance.Up);
			this.AddMap("DOWN", Settings.Instance.Down);
			this.AddMap("JUMP", Settings.Instance.Jump);
			this.AddMap("DASH", Settings.Instance.Dash);
			this.AddMap("GRAB", Settings.Instance.Grab);
			this.AddMap("TALK", Settings.Instance.Talk);
			base.Add(new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_MENUS", null), true));
			base.Add(new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_MENU_NOTICE", null), false));
			this.AddMap("LEFT", Settings.Instance.MenuLeft);
			this.AddMap("RIGHT", Settings.Instance.MenuRight);
			this.AddMap("UP", Settings.Instance.MenuUp);
			this.AddMap("DOWN", Settings.Instance.MenuDown);
			this.AddMap("CONFIRM", Settings.Instance.Confirm);
			this.AddMap("CANCEL", Settings.Instance.Cancel);
			this.AddMap("JOURNAL", Settings.Instance.Journal);
			this.AddMap("PAUSE", Settings.Instance.Pause);
			base.Add(new TextMenu.SubHeader("", true));
			base.Add(new TextMenu.Button(Dialog.Clean("KEY_CONFIG_RESET", null))
			{
				IncludeWidthInMeasurement = false,
				AlwaysCenter = true,
				OnPressed = delegate()
				{
					this.resetHeld = true;
					this.resetTime = 0f;
					this.resetDelay = 0f;
				},
				ConfirmSfx = "event:/ui/main/button_lowkey"
			});
			base.Add(new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_ADVANCED", null), true));
			this.AddMap("QUICKRESTART", Settings.Instance.QuickRestart);
			this.AddMap("DEMO", Settings.Instance.DemoDash);
			base.Add(new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_MOVE_ONLY", null), true));
			this.AddMap("LEFT", Settings.Instance.LeftMoveOnly);
			this.AddMap("RIGHT", Settings.Instance.RightMoveOnly);
			this.AddMap("UP", Settings.Instance.UpMoveOnly);
			this.AddMap("DOWN", Settings.Instance.DownMoveOnly);
			base.Add(new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_DASH_ONLY", null), true));
			this.AddMap("LEFT", Settings.Instance.LeftDashOnly);
			this.AddMap("RIGHT", Settings.Instance.RightDashOnly);
			this.AddMap("UP", Settings.Instance.UpDashOnly);
			this.AddMap("DOWN", Settings.Instance.DownDashOnly);
			this.OnESC = (this.OnCancel = delegate()
			{
				MenuOptions.UpdateCrouchDashModeVisibility();
				this.Focused = false;
				this.closing = true;
			});
			this.MinWidth = 600f;
			this.Position.Y = base.ScrollTargetY;
			this.Alpha = 0f;
		}

		// Token: 0x060010A9 RID: 4265 RVA: 0x0004D9B8 File Offset: 0x0004BBB8
		private void AddMap(string label, Binding binding)
		{
			string txt = Dialog.Clean("KEY_CONFIG_" + label, null);
			base.Add(new TextMenu.Setting(txt, binding, true).Pressed(delegate
			{
				this.remappingText = txt;
				this.Remap(binding);
			}).AltPressed(delegate
			{
				this.Clear(binding);
			}));
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x0004DA2B File Offset: 0x0004BC2B
		private void Remap(Binding binding)
		{
			if (Input.GuiInputController(Input.PrefixMode.Latest))
			{
				this.remapping = true;
				this.remappingBinding = binding;
				this.timeout = 5f;
				this.Focused = false;
			}
		}

		// Token: 0x060010AB RID: 4267 RVA: 0x0004DA58 File Offset: 0x0004BC58
		private void AddRemap(Buttons btn)
		{
			while (this.remappingBinding.Controller.Count >= Input.MaxBindings)
			{
				this.remappingBinding.Controller.RemoveAt(0);
			}
			this.remapping = false;
			this.inputDelay = 0.25f;
			if (!this.remappingBinding.Add(new Buttons[]
			{
				btn
			}))
			{
				Audio.Play("event:/ui/main/button_invalid");
			}
			Input.Initialize();
		}

		// Token: 0x060010AC RID: 4268 RVA: 0x0004DAC8 File Offset: 0x0004BCC8
		private void Clear(Binding binding)
		{
			if (!binding.ClearGamepad())
			{
				Audio.Play("event:/ui/main/button_invalid");
			}
		}

		// Token: 0x060010AD RID: 4269 RVA: 0x0004DAE0 File Offset: 0x0004BCE0
		public override void Update()
		{
			if (this.resetHeld)
			{
				this.resetDelay += Engine.DeltaTime;
				this.resetTime += Engine.DeltaTime;
				if (this.resetTime > 1.5f)
				{
					this.resetDelay = 0f;
					this.resetHeld = false;
					Settings.Instance.SetDefaultButtonControls(true);
					Input.Initialize();
					Audio.Play("event:/ui/main/button_select");
				}
				if (!Input.MenuConfirm.Check && this.resetDelay > 0.3f)
				{
					Audio.Play("event:/ui/main/button_invalid");
					this.resetHeld = false;
				}
				if (this.resetHeld)
				{
					return;
				}
			}
			base.Update();
			this.Focused = (!this.closing && this.inputDelay <= 0f && !this.waitingForController && !this.remapping);
			if (!this.closing)
			{
				if (!MInput.GamePads[Input.Gamepad].Attached)
				{
					this.waitingForController = true;
				}
				else if (this.waitingForController)
				{
					this.waitingForController = false;
				}
				if (Input.MenuCancel.Pressed && !this.remapping)
				{
					this.OnCancel();
				}
			}
			if (this.inputDelay > 0f && !this.remapping)
			{
				this.inputDelay -= Engine.RawDeltaTime;
			}
			this.remappingEase = Calc.Approach(this.remappingEase, (float)(this.remapping ? 1 : 0), Engine.RawDeltaTime * 4f);
			if (this.remappingEase >= 0.25f && this.remapping)
			{
				if (Input.ESC.Pressed || this.timeout <= 0f || !Input.GuiInputController(Input.PrefixMode.Latest))
				{
					this.remapping = false;
					this.Focused = true;
				}
				else
				{
					MInput.GamePadData gamePadData = MInput.GamePads[Input.Gamepad];
					float num = 0.25f;
					if (gamePadData.LeftStickLeftPressed(num))
					{
						this.AddRemap(Buttons.LeftThumbstickLeft);
					}
					else if (gamePadData.LeftStickRightPressed(num))
					{
						this.AddRemap(Buttons.LeftThumbstickRight);
					}
					else if (gamePadData.LeftStickUpPressed(num))
					{
						this.AddRemap(Buttons.LeftThumbstickUp);
					}
					else if (gamePadData.LeftStickDownPressed(num))
					{
						this.AddRemap(Buttons.LeftThumbstickDown);
					}
					else if (gamePadData.RightStickLeftPressed(num))
					{
						this.AddRemap(Buttons.RightThumbstickLeft);
					}
					else if (gamePadData.RightStickRightPressed(num))
					{
						this.AddRemap(Buttons.RightThumbstickRight);
					}
					else if (gamePadData.RightStickDownPressed(num))
					{
						this.AddRemap(Buttons.RightThumbstickDown);
					}
					else if (gamePadData.RightStickUpPressed(num))
					{
						this.AddRemap(Buttons.RightThumbstickUp);
					}
					else if (gamePadData.LeftTriggerPressed(num))
					{
						this.AddRemap(Buttons.LeftTrigger);
					}
					else if (gamePadData.RightTriggerPressed(num))
					{
						this.AddRemap(Buttons.RightTrigger);
					}
					else if (gamePadData.Pressed(Buttons.DPadLeft))
					{
						this.AddRemap(Buttons.DPadLeft);
					}
					else if (gamePadData.Pressed(Buttons.DPadRight))
					{
						this.AddRemap(Buttons.DPadRight);
					}
					else if (gamePadData.Pressed(Buttons.DPadUp))
					{
						this.AddRemap(Buttons.DPadUp);
					}
					else if (gamePadData.Pressed(Buttons.DPadDown))
					{
						this.AddRemap(Buttons.DPadDown);
					}
					else if (gamePadData.Pressed(Buttons.A))
					{
						this.AddRemap(Buttons.A);
					}
					else if (gamePadData.Pressed(Buttons.B))
					{
						this.AddRemap(Buttons.B);
					}
					else if (gamePadData.Pressed(Buttons.X))
					{
						this.AddRemap(Buttons.X);
					}
					else if (gamePadData.Pressed(Buttons.Y))
					{
						this.AddRemap(Buttons.Y);
					}
					else if (gamePadData.Pressed(Buttons.Start))
					{
						this.AddRemap(Buttons.Start);
					}
					else if (gamePadData.Pressed(Buttons.Back))
					{
						this.AddRemap(Buttons.Back);
					}
					else if (gamePadData.Pressed(Buttons.LeftShoulder))
					{
						this.AddRemap(Buttons.LeftShoulder);
					}
					else if (gamePadData.Pressed(Buttons.RightShoulder))
					{
						this.AddRemap(Buttons.RightShoulder);
					}
					else if (gamePadData.Pressed(Buttons.LeftStick))
					{
						this.AddRemap(Buttons.LeftStick);
					}
					else if (gamePadData.Pressed(Buttons.RightStick))
					{
						this.AddRemap(Buttons.RightStick);
					}
				}
				this.timeout -= Engine.RawDeltaTime;
			}
			this.closingDelay -= Engine.RawDeltaTime;
			this.Alpha = Calc.Approach(this.Alpha, (float)((this.closing && this.closingDelay <= 0f) ? 0 : 1), Engine.RawDeltaTime * 8f);
			if (this.closing && this.Alpha <= 0f)
			{
				base.Close();
			}
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x0004DF80 File Offset: 0x0004C180
		public override void Render()
		{
			Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * Ease.CubeOut(this.Alpha));
			Vector2 vector = new Vector2(1920f, 1080f) * 0.5f;
			if (MInput.GamePads[Input.Gamepad].Attached)
			{
				base.Render();
				if (this.remappingEase > 0f)
				{
					Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.95f * Ease.CubeInOut(this.remappingEase));
					ActiveFont.Draw(Dialog.Get("BTN_CONFIG_CHANGING", null), vector + new Vector2(0f, -8f), new Vector2(0.5f, 1f), Vector2.One * 0.7f, Color.LightGray * Ease.CubeIn(this.remappingEase));
					ActiveFont.Draw(this.remappingText, vector + new Vector2(0f, 8f), new Vector2(0.5f, 0f), Vector2.One * 2f, Color.White * Ease.CubeIn(this.remappingEase));
				}
			}
			else
			{
				ActiveFont.Draw(Dialog.Clean("BTN_CONFIG_NOCONTROLLER", null), vector, new Vector2(0.5f, 0.5f), Vector2.One, Color.White * Ease.CubeOut(this.Alpha));
			}
			if (this.resetHeld)
			{
				float scale = Ease.CubeInOut(Calc.Min(new float[]
				{
					1f,
					this.resetDelay / 0.2f
				}));
				float num = Ease.SineOut(Calc.Min(new float[]
				{
					1f,
					this.resetTime / 1.5f
				}));
				Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.95f * scale);
				float num2 = 480f;
				float x = (1920f - num2) / 2f;
				Draw.Rect(x, 530f, num2, 20f, Color.White * 0.25f * scale);
				Draw.Rect(x, 530f, num2 * num, 20f, Color.White * scale);
			}
		}

		// Token: 0x04000C22 RID: 3106
		private bool remapping;

		// Token: 0x04000C23 RID: 3107
		private float remappingEase;

		// Token: 0x04000C24 RID: 3108
		private Binding remappingBinding;

		// Token: 0x04000C25 RID: 3109
		private string remappingText;

		// Token: 0x04000C26 RID: 3110
		private float inputDelay;

		// Token: 0x04000C27 RID: 3111
		private float timeout;

		// Token: 0x04000C28 RID: 3112
		private bool closing;

		// Token: 0x04000C29 RID: 3113
		private float closingDelay;

		// Token: 0x04000C2A RID: 3114
		private bool waitingForController;

		// Token: 0x04000C2B RID: 3115
		private bool resetHeld;

		// Token: 0x04000C2C RID: 3116
		private float resetTime;

		// Token: 0x04000C2D RID: 3117
		private float resetDelay;

		// Token: 0x04000C2E RID: 3118
		private List<Buttons> all = new List<Buttons>
		{
			Buttons.A,
			Buttons.B,
			Buttons.X,
			Buttons.Y,
			Buttons.LeftShoulder,
			Buttons.RightShoulder,
			Buttons.LeftTrigger,
			Buttons.RightTrigger
		};

		// Token: 0x04000C2F RID: 3119
		public static readonly string StadiaControllerDisclaimer = "No endorsement or affiliation is intended between Stadia and the manufacturers\nof non-Stadia controllers or consoles. STADIA, the Stadia beacon, Google, and related\nmarks and logos are trademarks of Google LLC. All other trademarks are the\nproperty of their respective owners.";
	}
}
