using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200024A RID: 586
	public class OuiAssistMode : Oui
	{
		// Token: 0x06001270 RID: 4720 RVA: 0x000613C4 File Offset: 0x0005F5C4
		public OuiAssistMode()
		{
			this.Visible = false;
			base.Add(this.wiggler = Wiggler.Create(0.4f, 4f, null, false, false));
		}

		// Token: 0x06001271 RID: 4721 RVA: 0x00061421 File Offset: 0x0005F621
		public override IEnumerator Enter(Oui from)
		{
			this.Focused = false;
			this.Visible = true;
			this.pageIndex = 0;
			this.questionIndex = 1;
			this.questionEase = 0f;
			this.dot = 0f;
			this.questionText = FancyText.Parse(Dialog.Get("ASSIST_ASK", null), 1600, -1, 1f, new Color?(Color.White), null);
			if (!this.FileSlot.AssistModeEnabled)
			{
				int num = 0;
				while (Dialog.Has("ASSIST_MODE_" + num, null))
				{
					OuiAssistMode.Page page = new OuiAssistMode.Page();
					page.Text = FancyText.Parse(Dialog.Get("ASSIST_MODE_" + num, null), 2000, -1, 1f, new Color?(Color.White * 0.9f), null);
					page.Ease = 0f;
					this.pages.Add(page);
					num++;
				}
				this.pages[0].Ease = 1f;
				this.mainSfx = Audio.Play("event:/ui/main/assist_info_whistle");
			}
			else
			{
				this.questionEase = 1f;
			}
			while (this.fade < 1f)
			{
				this.fade += Engine.DeltaTime * 4f;
				yield return null;
			}
			this.Focused = true;
			base.Add(new Coroutine(this.InputRoutine(), true));
			yield break;
		}

		// Token: 0x06001272 RID: 4722 RVA: 0x00061430 File Offset: 0x0005F630
		public override IEnumerator Leave(Oui next)
		{
			this.Focused = false;
			while (this.fade > 0f)
			{
				this.fade -= Engine.DeltaTime * 4f;
				yield return null;
			}
			if (this.mainSfx != null)
			{
				this.mainSfx.release();
			}
			this.pages.Clear();
			this.Visible = false;
			yield break;
		}

		// Token: 0x06001273 RID: 4723 RVA: 0x0006143F File Offset: 0x0005F63F
		private IEnumerator InputRoutine()
		{
			while (!Input.MenuCancel.Pressed)
			{
				int was = this.pageIndex;
				if ((Input.MenuConfirm.Pressed || Input.MenuRight.Pressed) && this.pageIndex < this.pages.Count)
				{
					this.pageIndex++;
					Audio.Play("event:/ui/main/rollover_down");
					Audio.SetParameter(this.mainSfx, "assist_progress", (float)this.pageIndex);
				}
				else if (Input.MenuLeft.Pressed && this.pageIndex > 0)
				{
					Audio.Play("event:/ui/main/rollover_up");
					this.pageIndex--;
				}
				if (was != this.pageIndex)
				{
					if (was < this.pages.Count)
					{
						this.pages[was].Direction = (float)Math.Sign(was - this.pageIndex);
						while ((this.pages[was].Ease = Calc.Approach(this.pages[was].Ease, 0f, Engine.DeltaTime * 8f)) != 0f)
						{
							yield return null;
						}
					}
					else
					{
						while ((this.questionEase = Calc.Approach(this.questionEase, 0f, Engine.DeltaTime * 8f)) != 0f)
						{
							yield return null;
						}
					}
					if (this.pageIndex < this.pages.Count)
					{
						this.pages[this.pageIndex].Direction = (float)Math.Sign(this.pageIndex - was);
						while ((this.pages[this.pageIndex].Ease = Calc.Approach(this.pages[this.pageIndex].Ease, 1f, Engine.DeltaTime * 8f)) != 1f)
						{
							yield return null;
						}
					}
					else
					{
						while ((this.questionEase = Calc.Approach(this.questionEase, 1f, Engine.DeltaTime * 8f)) != 1f)
						{
							yield return null;
						}
					}
				}
				if (this.pageIndex >= this.pages.Count)
				{
					if (Input.MenuConfirm.Pressed)
					{
						this.FileSlot.AssistModeEnabled = (this.questionIndex == 0);
						if (this.FileSlot.AssistModeEnabled)
						{
							this.FileSlot.VariantModeEnabled = false;
						}
						this.FileSlot.CreateButtons();
						this.Focused = false;
						base.Overworld.Goto<OuiFileSelect>();
						Audio.Play((this.questionIndex == 0) ? "event:/ui/main/assist_button_yes" : "event:/ui/main/assist_button_no");
						Audio.SetParameter(this.mainSfx, "assist_progress", (float)((this.questionIndex == 0) ? 4 : 5));
						IL_42B:
						yield break;
					}
					if (Input.MenuUp.Pressed && this.questionIndex > 0)
					{
						Audio.Play("event:/ui/main/rollover_up");
						this.questionIndex--;
						this.wiggler.Start();
					}
					else if (Input.MenuDown.Pressed && this.questionIndex < 1)
					{
						Audio.Play("event:/ui/main/rollover_down");
						this.questionIndex++;
						this.wiggler.Start();
					}
				}
				yield return null;
			}
			this.Focused = false;
			base.Overworld.Goto<OuiFileSelect>();
			Audio.Play("event:/ui/main/button_back");
			Audio.SetParameter(this.mainSfx, "assist_progress", 6f);
			goto IL_42B;
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x00061450 File Offset: 0x0005F650
		public override void Update()
		{
			this.dot = Calc.Approach(this.dot, (float)this.pageIndex, Engine.DeltaTime * 8f);
			this.leftArrowEase = Calc.Approach(this.leftArrowEase, (float)((this.pageIndex > 0) ? 1 : 0), Engine.DeltaTime * 4f);
			this.rightArrowEase = Calc.Approach(this.rightArrowEase, (float)((this.pageIndex < this.pages.Count) ? 1 : 0), Engine.DeltaTime * 4f);
			base.Update();
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x000614E4 File Offset: 0x0005F6E4
		public override void Render()
		{
			if (!this.Visible)
			{
				return;
			}
			Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.fade * 0.9f);
			for (int i = 0; i < this.pages.Count; i++)
			{
				OuiAssistMode.Page page = this.pages[i];
				float num = Ease.CubeOut(page.Ease);
				if (num > 0f)
				{
					Vector2 position = new Vector2(960f, 620f);
					position.X += page.Direction * (1f - num) * 256f;
					page.Text.DrawJustifyPerLine(position, new Vector2(0.5f, 0f), Vector2.One * 0.8f, num * this.fade, 0, int.MaxValue);
				}
			}
			if (this.questionEase > 0f)
			{
				float num2 = Ease.CubeOut(this.questionEase);
				float num3 = this.wiggler.Value * 8f;
				Vector2 vector = new Vector2(960f + (1f - num2) * 256f, 620f);
				float lineHeight = ActiveFont.LineHeight;
				this.questionText.DrawJustifyPerLine(vector, new Vector2(0.5f, 0f), Vector2.One, num2 * this.fade, 0, int.MaxValue);
				ActiveFont.DrawOutline(Dialog.Clean("ASSIST_YES", null), vector + new Vector2(((this.questionIndex == 0) ? num3 : 0f) * 1.2f * num2, lineHeight * 1.4f + 10f), new Vector2(0.5f, 0f), Vector2.One * 0.8f, this.SelectionColor(this.questionIndex == 0), 2f, Color.Black * num2 * this.fade);
				ActiveFont.DrawOutline(Dialog.Clean("ASSIST_NO", null), vector + new Vector2(((this.questionIndex == 1) ? num3 : 0f) * 1.2f * num2, lineHeight * 2.2f + 20f), new Vector2(0.5f, 0f), Vector2.One * 0.8f, this.SelectionColor(this.questionIndex == 1), 2f, Color.Black * num2 * this.fade);
			}
			if (this.pages.Count > 0)
			{
				int num4 = this.pages.Count + 1;
				MTexture mtexture = GFX.Gui["dot"];
				int num5 = mtexture.Width * num4;
				Vector2 value = new Vector2(960f, 960f - 40f * Ease.CubeOut(this.fade));
				for (int j = 0; j < num4; j++)
				{
					mtexture.DrawCentered(value + new Vector2((float)(-(float)num5 / 2) + (float)mtexture.Width * ((float)j + 0.5f), 0f), Color.White * 0.25f);
				}
				float x = 1f + Calc.YoYo(this.dot % 1f) * 4f;
				mtexture.DrawCentered(value + new Vector2((float)(-(float)num5 / 2) + (float)mtexture.Width * (this.dot + 0.5f), 0f), this.iconColor, new Vector2(x, 1f));
				GFX.Gui["dotarrow"].DrawCentered(value + new Vector2((float)(-(float)num5 / 2 - 50), 32f * (1f - Ease.CubeOut(this.leftArrowEase))), this.iconColor * this.leftArrowEase, new Vector2(-1f, 1f));
				GFX.Gui["dotarrow"].DrawCentered(value + new Vector2((float)(num5 / 2 + 50), 32f * (1f - Ease.CubeOut(this.rightArrowEase))), this.iconColor * this.rightArrowEase);
			}
			GFX.Gui["assistmode"].DrawJustified(new Vector2(960f, 540f + 64f * Ease.CubeOut(this.fade)), new Vector2(0.5f, 1f), this.iconColor * this.fade);
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x000619B0 File Offset: 0x0005FBB0
		private Color SelectionColor(bool selected)
		{
			if (selected)
			{
				return ((Settings.Instance.DisableFlashes || base.Scene.BetweenInterval(0.1f)) ? TextMenu.HighlightColorA : TextMenu.HighlightColorB) * this.fade;
			}
			return Color.White * this.fade;
		}

		// Token: 0x04000E2A RID: 3626
		public OuiFileSelectSlot FileSlot;

		// Token: 0x04000E2B RID: 3627
		private float fade;

		// Token: 0x04000E2C RID: 3628
		private List<OuiAssistMode.Page> pages = new List<OuiAssistMode.Page>();

		// Token: 0x04000E2D RID: 3629
		private int pageIndex;

		// Token: 0x04000E2E RID: 3630
		private int questionIndex = 1;

		// Token: 0x04000E2F RID: 3631
		private float questionEase;

		// Token: 0x04000E30 RID: 3632
		private Wiggler wiggler;

		// Token: 0x04000E31 RID: 3633
		private float dot;

		// Token: 0x04000E32 RID: 3634
		private FancyText.Text questionText;

		// Token: 0x04000E33 RID: 3635
		private Color iconColor = Calc.HexToColor("44adf7");

		// Token: 0x04000E34 RID: 3636
		private float leftArrowEase;

		// Token: 0x04000E35 RID: 3637
		private float rightArrowEase;

		// Token: 0x04000E36 RID: 3638
		private EventInstance mainSfx;

		// Token: 0x04000E37 RID: 3639
		private const float textScale = 0.8f;

		// Token: 0x0200056C RID: 1388
		private class Page
		{
			// Token: 0x04002676 RID: 9846
			public FancyText.Text Text;

			// Token: 0x04002677 RID: 9847
			public float Ease;

			// Token: 0x04002678 RID: 9848
			public float Direction;
		}
	}
}
