using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x02000234 RID: 564
	[Tracked(false)]
	public class KeyboardConfigUI : TextMenu
	{
		// Token: 0x060011EA RID: 4586 RVA: 0x0005A1C0 File Offset: 0x000583C0
		public KeyboardConfigUI()
		{
			base.Add(new TextMenu.Header(Dialog.Clean("KEY_CONFIG_TITLE", null)));
			base.Add(new InputMappingInfo(false));
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
				ConfirmSfx = "event:/ui/main/button_lowkey",
				OnPressed = delegate()
				{
					this.resetHeld = true;
					this.resetTime = 0f;
					this.resetDelay = 0f;
				}
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

		// Token: 0x060011EB RID: 4587 RVA: 0x0005A544 File Offset: 0x00058744
		private void AddMap(string label, Binding binding)
		{
			string txt = Dialog.Clean("KEY_CONFIG_" + label, null);
			base.Add(new TextMenu.Setting(txt, binding, false).Pressed(delegate
			{
				this.remappingText = txt;
				this.Remap(binding);
			}).AltPressed(delegate
			{
				this.Clear(binding);
			}));
		}

		// Token: 0x060011EC RID: 4588 RVA: 0x0005A5B7 File Offset: 0x000587B7
		private void Remap(Binding binding)
		{
			this.remapping = true;
			this.remappingBinding = binding;
			this.timeout = 5f;
			this.Focused = false;
		}

		// Token: 0x060011ED RID: 4589 RVA: 0x0005A5DC File Offset: 0x000587DC
		private void AddRemap(Keys key)
		{
			while (this.remappingBinding.Keyboard.Count >= Input.MaxBindings)
			{
				this.remappingBinding.Keyboard.RemoveAt(0);
			}
			this.remapping = false;
			this.inputDelay = 0.25f;
			if (!this.remappingBinding.Add(new Keys[]
			{
				key
			}))
			{
				Audio.Play("event:/ui/main/button_invalid");
			}
			Input.Initialize();
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x0005A64C File Offset: 0x0005884C
		private void Clear(Binding binding)
		{
			if (!binding.ClearKeyboard())
			{
				Audio.Play("event:/ui/main/button_invalid");
			}
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x0005A664 File Offset: 0x00058864
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
					Settings.Instance.SetDefaultKeyboardControls(true);
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
			this.Focused = (!this.closing && this.inputDelay <= 0f && !this.remapping);
			if (!this.closing && Input.MenuCancel.Pressed && !this.remapping)
			{
				this.OnCancel();
			}
			if (this.inputDelay > 0f && !this.remapping)
			{
				this.inputDelay -= Engine.RawDeltaTime;
			}
			this.remappingEase = Calc.Approach(this.remappingEase, (float)(this.remapping ? 1 : 0), Engine.RawDeltaTime * 4f);
			if (this.remappingEase >= 0.25f && this.remapping)
			{
				if (Input.ESC.Pressed || this.timeout <= 0f)
				{
					this.remapping = false;
					this.Focused = true;
				}
				else
				{
					Keys[] pressedKeys = MInput.Keyboard.CurrentState.GetPressedKeys();
					if (pressedKeys != null && pressedKeys.Length != 0 && MInput.Keyboard.Pressed(pressedKeys[pressedKeys.Length - 1]))
					{
						this.AddRemap(pressedKeys[pressedKeys.Length - 1]);
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

		// Token: 0x060011F0 RID: 4592 RVA: 0x0005A89C File Offset: 0x00058A9C
		public override void Render()
		{
			Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * Ease.CubeOut(this.Alpha));
			Vector2 value = new Vector2(1920f, 1080f) * 0.5f;
			base.Render();
			if (this.remappingEase > 0f)
			{
				Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.95f * Ease.CubeInOut(this.remappingEase));
				ActiveFont.Draw(Dialog.Get("KEY_CONFIG_CHANGING", null), value + new Vector2(0f, -8f), new Vector2(0.5f, 1f), Vector2.One * 0.7f, Color.LightGray * Ease.CubeIn(this.remappingEase));
				ActiveFont.Draw(this.remappingText, value + new Vector2(0f, 8f), new Vector2(0.5f, 0f), Vector2.One * 2f, Color.White * Ease.CubeIn(this.remappingEase));
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

		// Token: 0x04000D9C RID: 3484
		private bool remapping;

		// Token: 0x04000D9D RID: 3485
		private float remappingEase;

		// Token: 0x04000D9E RID: 3486
		private Binding remappingBinding;

		// Token: 0x04000D9F RID: 3487
		private string remappingText;

		// Token: 0x04000DA0 RID: 3488
		private float inputDelay;

		// Token: 0x04000DA1 RID: 3489
		private float timeout;

		// Token: 0x04000DA2 RID: 3490
		private bool closing;

		// Token: 0x04000DA3 RID: 3491
		private float closingDelay;

		// Token: 0x04000DA4 RID: 3492
		private bool resetHeld;

		// Token: 0x04000DA5 RID: 3493
		private float resetTime;

		// Token: 0x04000DA6 RID: 3494
		private float resetDelay;
	}
}
