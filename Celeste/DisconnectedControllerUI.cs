using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000140 RID: 320
	public class DisconnectedControllerUI
	{
		// Token: 0x06000BA0 RID: 2976 RVA: 0x00021B44 File Offset: 0x0001FD44
		public DisconnectedControllerUI()
		{
			Celeste.DisconnectUI = this;
			Engine.OverloadGameLoop = new Action(this.Update);
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x00021B63 File Offset: 0x0001FD63
		private void OnClose()
		{
			Celeste.DisconnectUI = null;
			Engine.OverloadGameLoop = null;
		}

		// Token: 0x06000BA2 RID: 2978 RVA: 0x00021B74 File Offset: 0x0001FD74
		public void Update()
		{
			bool disabled = MInput.Disabled;
			MInput.Disabled = false;
			this.fade = Calc.Approach(this.fade, (float)(this.closing ? 0 : 1), Engine.DeltaTime * 8f);
			if (!this.closing)
			{
				int gamepad = -1;
				if (Input.AnyGamepadConfirmPressed(out gamepad))
				{
					Input.Gamepad = gamepad;
					this.closing = true;
				}
			}
			else if (this.fade <= 0f)
			{
				this.OnClose();
			}
			MInput.Disabled = disabled;
		}

		// Token: 0x06000BA3 RID: 2979 RVA: 0x00021BF0 File Offset: 0x0001FDF0
		public void Render()
		{
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Engine.ScreenMatrix);
			Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.fade * 0.8f);
			ActiveFont.DrawOutline(Dialog.Clean("XB1_RECONNECT_CONTROLLER", null), Celeste.TargetCenter, new Vector2(0.5f, 0.5f), Vector2.One, Color.White * this.fade, 2f, Color.Black * this.fade * this.fade);
			Input.GuiButton(Input.MenuConfirm, Input.PrefixMode.Latest, "controls/keyboard/oemquestion").DrawCentered(Celeste.TargetCenter + new Vector2(0f, 128f), Color.White * this.fade);
			Draw.SpriteBatch.End();
		}

		// Token: 0x06000BA4 RID: 2980 RVA: 0x00021CF8 File Offset: 0x0001FEF8
		private static bool IsGamepadConnected()
		{
			int gamepad = Input.Gamepad;
			return MInput.GamePads[gamepad].Attached;
		}

		// Token: 0x06000BA5 RID: 2981 RVA: 0x00021D18 File Offset: 0x0001FF18
		private static bool RequiresGamepad()
		{
			if (Engine.Scene == null || Engine.Scene is GameLoader || Engine.Scene is OverworldLoader)
			{
				return false;
			}
			Overworld overworld = Engine.Scene as Overworld;
			return overworld == null || !(overworld.Current is OuiTitleScreen);
		}

		// Token: 0x06000BA6 RID: 2982 RVA: 0x00021D64 File Offset: 0x0001FF64
		public static void CheckGamepadDisconnect()
		{
			if (Celeste.DisconnectUI != null)
			{
				return;
			}
			if (!DisconnectedControllerUI.RequiresGamepad())
			{
				return;
			}
			if (DisconnectedControllerUI.IsGamepadConnected())
			{
				return;
			}
			new DisconnectedControllerUI();
		}

		// Token: 0x040006F0 RID: 1776
		private float fade;

		// Token: 0x040006F1 RID: 1777
		private bool closing;
	}
}
