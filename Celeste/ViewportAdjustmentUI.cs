using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000238 RID: 568
	public class ViewportAdjustmentUI : Entity
	{
		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06001222 RID: 4642 RVA: 0x0005BC54 File Offset: 0x00059E54
		// (set) Token: 0x06001223 RID: 4643 RVA: 0x0005BC5C File Offset: 0x00059E5C
		public float Alpha { get; private set; }

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06001224 RID: 4644 RVA: 0x0005BC65 File Offset: 0x00059E65
		// (set) Token: 0x06001225 RID: 4645 RVA: 0x0005BC6D File Offset: 0x00059E6D
		public bool Open { get; private set; }

		// Token: 0x06001226 RID: 4646 RVA: 0x0005BC78 File Offset: 0x00059E78
		public ViewportAdjustmentUI()
		{
			this.Open = true;
			base.Tag = (Tags.HUD | Tags.PauseUpdate);
		}

		// Token: 0x06001227 RID: 4647 RVA: 0x0005BCDB File Offset: 0x00059EDB
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (scene is Overworld)
			{
				(scene as Overworld).Mountain.Model.LockBufferResizing = true;
			}
		}

		// Token: 0x06001228 RID: 4648 RVA: 0x0005BD02 File Offset: 0x00059F02
		public override void Removed(Scene scene)
		{
			if (scene is Overworld)
			{
				(scene as Overworld).Mountain.Model.LockBufferResizing = false;
			}
			base.Removed(scene);
		}

		// Token: 0x06001229 RID: 4649 RVA: 0x0005BD2C File Offset: 0x00059F2C
		public override void Update()
		{
			base.Update();
			if (!this.closing)
			{
				this.inputDelay += Engine.RawDeltaTime;
				if (this.inputDelay > 0.25f)
				{
					if (Input.MenuCancel.Pressed || Input.ESC.Pressed)
					{
						this.closing = (this.canceling = true);
					}
					else if (Input.MenuConfirm.Pressed)
					{
						this.closing = true;
					}
				}
			}
			else if (this.Alpha <= 0f)
			{
				if (this.canceling)
				{
					Engine.ViewPadding = (int)this.originalPadding;
				}
				else
				{
					Settings.Instance.ViewportPadding = (int)this.viewPadding;
				}
				Settings.Instance.SetViewportOnce = true;
				this.Open = false;
				base.RemoveSelf();
				if (this.OnClose != null)
				{
					this.OnClose();
				}
				return;
			}
			this.Alpha = Calc.Approach(this.Alpha, (float)(this.closing ? 0 : 1), Engine.RawDeltaTime * 4f);
			this.viewPadding -= Input.Aim.Value.X * 48f * Engine.RawDeltaTime;
			this.viewPadding = Calc.Clamp(this.viewPadding, 0f, 128f);
			this.leftAlpha = Calc.Approach(this.leftAlpha, (this.viewPadding < 128f) ? 1f : 0.25f, Engine.DeltaTime * 4f);
			this.rightAlpha = Calc.Approach(this.rightAlpha, (this.viewPadding > 0f) ? 1f : 0.25f, Engine.DeltaTime * 4f);
			Engine.ViewPadding = (int)this.viewPadding;
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x0005BEF0 File Offset: 0x0005A0F0
		public override void Render()
		{
			float num = Ease.SineInOut(this.Alpha);
			Color color = Color.Black * 0.75f * num;
			Color color2 = Color.White * num;
			if (!(base.Scene is Level))
			{
				Draw.Rect(-1f, -1f, (float)(Engine.Width + 2), (float)(Engine.Height + 2), color);
			}
			Draw.Rect(0f, 0f, (float)Engine.Width, 16f, color2);
			Draw.Rect(0f, 16f, 16f, (float)(Engine.Height - 32), color2);
			Draw.Rect((float)(Engine.Width - 16), 16f, 16f, (float)(Engine.Height - 32), color2);
			Draw.Rect(0f, (float)(Engine.Height - 16), (float)Engine.Width, 16f, color2);
			Draw.LineAngle(new Vector2(8f, 8f), 0.7853982f, 128f, color2, 16f);
			Draw.LineAngle(new Vector2((float)(Engine.Width - 8), 8f), 2.3561945f, 128f, color2, 16f);
			Draw.LineAngle(new Vector2(8f, (float)(Engine.Height - 8)), -0.7853982f, 128f, color2, 16f);
			Draw.LineAngle(new Vector2((float)(Engine.Width - 8), (float)(Engine.Height - 8)), -2.3561945f, 128f, color2, 16f);
			string text = Dialog.Clean("OPTIONS_VIEWPORT_PC", null);
			ActiveFont.Measure(text);
			float num2 = (float)Math.Sin((double)(base.Scene.RawTimeActive * 2f)) * 16f;
			Vector2 value = new Vector2((float)Engine.Width, (float)Engine.Height) * 0.5f;
			ActiveFont.Draw(text, value + new Vector2(0f, -60f), new Vector2(0.5f, 0.5f), Vector2.One * 1.2f, color2);
			float num3 = ButtonUI.Width(Dialog.Clean("ui_confirm", null), Input.MenuConfirm) * 0.8f;
			ButtonUI.Render(value + new Vector2(0f, 60f), Dialog.Clean("ui_confirm", null), Input.MenuConfirm, 0.8f, 0.5f, 0f, num);
			Vector2 value2 = value + new Vector2(num3 * 0.6f + 80f + num2, 60f);
			GFX.Gui["adjustarrowright"].DrawCentered(value2 + new Vector2(8f, 4f), color2 * this.rightAlpha, Vector2.One);
			Vector2 value3 = value + new Vector2(-(num3 * 0.6f + 80f + num2), 60f);
			GFX.Gui["adjustarrowleft"].DrawCentered(value3 + new Vector2(-8f, 4f), color2 * this.leftAlpha, Vector2.One);
		}

		// Token: 0x04000DCB RID: 3531
		private const float minPadding = 0f;

		// Token: 0x04000DCC RID: 3532
		private const float maxPadding = 128f;

		// Token: 0x04000DCD RID: 3533
		private readonly float originalPadding = (float)Engine.ViewPadding;

		// Token: 0x04000DCE RID: 3534
		private float viewPadding = (float)Engine.ViewPadding;

		// Token: 0x04000DCF RID: 3535
		private float inputDelay;

		// Token: 0x04000DD0 RID: 3536
		private bool closing;

		// Token: 0x04000DD1 RID: 3537
		private bool canceling;

		// Token: 0x04000DD2 RID: 3538
		private float leftAlpha = 1f;

		// Token: 0x04000DD3 RID: 3539
		private float rightAlpha = 1f;

		// Token: 0x04000DD6 RID: 3542
		public Action OnClose;
	}
}
