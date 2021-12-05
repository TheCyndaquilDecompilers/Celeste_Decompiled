using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000235 RID: 565
	public class Postcard : Entity
	{
		// Token: 0x060011F3 RID: 4595 RVA: 0x0005AB1B File Offset: 0x00058D1B
		public Postcard(string msg, int area) : this(msg, "event:/ui/main/postcard_ch" + area + "_in", "event:/ui/main/postcard_ch" + area + "_out")
		{
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x0005AB50 File Offset: 0x00058D50
		public Postcard(string msg, string sfxEventIn, string sfxEventOut)
		{
			this.Visible = false;
			base.Tag = Tags.HUD;
			this.sfxEventIn = sfxEventIn;
			this.sfxEventOut = sfxEventOut;
			this.postcard = GFX.Gui["postcard"];
			this.text = FancyText.Parse(msg, (int)((float)(this.postcard.Width - 120) / 0.7f), -1, 1f, new Color?(Color.Black * 0.6f), null);
		}

		// Token: 0x060011F5 RID: 4597 RVA: 0x0005ABF0 File Offset: 0x00058DF0
		public IEnumerator DisplayRoutine()
		{
			yield return this.EaseIn();
			yield return 0.75f;
			while (!Input.MenuConfirm.Pressed)
			{
				yield return null;
			}
			Audio.Play("event:/ui/main/button_lowkey");
			yield return this.EaseOut();
			yield return 1.2f;
			yield break;
		}

		// Token: 0x060011F6 RID: 4598 RVA: 0x0005ABFF File Offset: 0x00058DFF
		public IEnumerator EaseIn()
		{
			Audio.Play(this.sfxEventIn);
			Vector2 vector = new Vector2((float)Engine.Width, (float)Engine.Height) / 2f;
			Vector2 from = vector + new Vector2(0f, 200f);
			Vector2 to = vector;
			float rFrom = -0.1f;
			float rTo = 0.05f;
			this.Visible = true;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 0.8f)
			{
				this.Position = from + (to - from) * Ease.CubeOut(p);
				this.alpha = Ease.CubeOut(p);
				this.rotation = rFrom + (rTo - rFrom) * Ease.CubeOut(p);
				yield return null;
			}
			base.Add(this.easeButtonIn = new Coroutine(this.EaseButtinIn(), true));
			yield break;
		}

		// Token: 0x060011F7 RID: 4599 RVA: 0x0005AC0E File Offset: 0x00058E0E
		private IEnumerator EaseButtinIn()
		{
			yield return 0.75f;
			while ((this.buttonEase += Engine.DeltaTime * 2f) < 1f)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x060011F8 RID: 4600 RVA: 0x0005AC1D File Offset: 0x00058E1D
		public IEnumerator EaseOut()
		{
			Audio.Play(this.sfxEventOut);
			if (this.easeButtonIn != null)
			{
				this.easeButtonIn.RemoveSelf();
			}
			Vector2 from = this.Position;
			Vector2 to = new Vector2((float)Engine.Width, (float)Engine.Height) / 2f + new Vector2(0f, -200f);
			float rFrom = this.rotation;
			float rTo = this.rotation + 0.1f;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime)
			{
				this.Position = from + (to - from) * Ease.CubeIn(p);
				this.alpha = 1f - Ease.CubeIn(p);
				this.rotation = rFrom + (rTo - rFrom) * Ease.CubeIn(p);
				this.buttonEase = Calc.Approach(this.buttonEase, 0f, Engine.DeltaTime * 8f);
				yield return null;
			}
			this.alpha = 0f;
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x060011F9 RID: 4601 RVA: 0x0005AC2C File Offset: 0x00058E2C
		public void BeforeRender()
		{
			if (this.target == null)
			{
				this.target = VirtualContent.CreateRenderTarget("postcard", this.postcard.Width, this.postcard.Height, false, true, 0);
			}
			Engine.Graphics.GraphicsDevice.SetRenderTarget(this.target);
			Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
			Draw.SpriteBatch.Begin();
			string text = Dialog.Clean("FILE_DEFAULT", null);
			if (SaveData.Instance != null && Dialog.Language.CanDisplay(SaveData.Instance.Name))
			{
				text = SaveData.Instance.Name;
			}
			this.postcard.Draw(Vector2.Zero);
			ActiveFont.Draw(text, new Vector2(115f, 30f), Vector2.Zero, Vector2.One * 0.9f, Color.Black * 0.7f);
			this.text.DrawJustifyPerLine(new Vector2((float)this.postcard.Width, (float)this.postcard.Height) / 2f + new Vector2(0f, 40f), new Vector2(0.5f, 0.5f), Vector2.One * 0.7f, 1f, 0, int.MaxValue);
			Draw.SpriteBatch.End();
		}

		// Token: 0x060011FA RID: 4602 RVA: 0x0005AD98 File Offset: 0x00058F98
		public override void Render()
		{
			if (this.target != null)
			{
				Draw.SpriteBatch.Draw(this.target, this.Position, new Rectangle?(this.target.Bounds), Color.White * this.alpha, this.rotation, new Vector2((float)this.target.Width, (float)this.target.Height) / 2f, this.scale, SpriteEffects.None, 0f);
			}
			if (this.buttonEase > 0f)
			{
				Input.GuiButton(Input.MenuConfirm, Input.PrefixMode.Latest, "controls/keyboard/oemquestion").DrawCentered(new Vector2((float)(Engine.Width - 120), (float)(Engine.Height - 100) - 20f * Ease.CubeOut(this.buttonEase)), Color.White * Ease.CubeOut(this.buttonEase));
			}
		}

		// Token: 0x060011FB RID: 4603 RVA: 0x0005AE8B File Offset: 0x0005908B
		public override void Removed(Scene scene)
		{
			this.Dispose();
			base.Removed(scene);
		}

		// Token: 0x060011FC RID: 4604 RVA: 0x0005AE9A File Offset: 0x0005909A
		public override void SceneEnd(Scene scene)
		{
			this.Dispose();
			base.SceneEnd(scene);
		}

		// Token: 0x060011FD RID: 4605 RVA: 0x0005AEA9 File Offset: 0x000590A9
		private void Dispose()
		{
			if (this.target != null)
			{
				this.target.Dispose();
			}
			this.target = null;
		}

		// Token: 0x04000DA7 RID: 3495
		private const float TextScale = 0.7f;

		// Token: 0x04000DA8 RID: 3496
		private MTexture postcard;

		// Token: 0x04000DA9 RID: 3497
		private VirtualRenderTarget target;

		// Token: 0x04000DAA RID: 3498
		private FancyText.Text text;

		// Token: 0x04000DAB RID: 3499
		private float alpha = 1f;

		// Token: 0x04000DAC RID: 3500
		private float scale = 1f;

		// Token: 0x04000DAD RID: 3501
		private float rotation;

		// Token: 0x04000DAE RID: 3502
		private float buttonEase;

		// Token: 0x04000DAF RID: 3503
		private string sfxEventIn;

		// Token: 0x04000DB0 RID: 3504
		private string sfxEventOut;

		// Token: 0x04000DB1 RID: 3505
		private Coroutine easeButtonIn;
	}
}
