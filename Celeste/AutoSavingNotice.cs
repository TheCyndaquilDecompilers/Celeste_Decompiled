using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000231 RID: 561
	public class AutoSavingNotice : Renderer
	{
		// Token: 0x060011DA RID: 4570 RVA: 0x00059720 File Offset: 0x00057920
		public AutoSavingNotice()
		{
			this.icon.Visible = false;
			this.wiggler = Wiggler.Create(0.4f, 4f, delegate(float f)
			{
				this.icon.Rotation = f * 0.1f;
			}, false, false);
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x0005978C File Offset: 0x0005798C
		public override void Update(Scene scene)
		{
			base.Update(scene);
			if (this.startTimer > 0f)
			{
				this.startTimer -= Engine.DeltaTime;
				if (this.startTimer <= 0f)
				{
					this.icon.Play("start", false, false);
					this.icon.Visible = true;
				}
			}
			if (scene.OnInterval(1f))
			{
				this.wiggler.Start();
			}
			bool flag = this.ForceClose || (!this.Display && this.timer >= 1f);
			this.ease = Calc.Approach(this.ease, (float)((!flag) ? 1 : 0), Engine.DeltaTime);
			this.timer += Engine.DeltaTime / 3f;
			this.StillVisible = (this.Display || this.ease > 0f);
			this.wiggler.Update();
			this.icon.Update();
			if (flag && !string.IsNullOrEmpty(this.icon.CurrentAnimationID) && this.icon.CurrentAnimationID.Equals("idle"))
			{
				this.icon.Play("end", false, false);
			}
		}

		// Token: 0x060011DC RID: 4572 RVA: 0x000598D4 File Offset: 0x00057AD4
		public override void Render(Scene scene)
		{
			float num = Ease.CubeInOut(this.ease);
			Color color = AutoSavingNotice.TextColor * num;
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, Engine.ScreenMatrix);
			ActiveFont.Draw(Dialog.Clean("autosaving_title_PC", null), new Vector2(960f, 480f - 30f * num), new Vector2(0.5f, 1f), Vector2.One, color);
			if (this.icon.Visible)
			{
				this.icon.RenderPosition = new Vector2(1920f, 1080f) / 2f;
				this.icon.Render();
			}
			ActiveFont.Draw(Dialog.Clean("autosaving_desc_PC", null), new Vector2(960f, 600f + 30f * num), new Vector2(0.5f, 0f), Vector2.One, color);
			Draw.SpriteBatch.End();
		}

		// Token: 0x04000D75 RID: 3445
		private const string title = "autosaving_title_PC";

		// Token: 0x04000D76 RID: 3446
		private const string desc = "autosaving_desc_PC";

		// Token: 0x04000D77 RID: 3447
		private const float duration = 3f;

		// Token: 0x04000D78 RID: 3448
		public static readonly Color TextColor = Color.White;

		// Token: 0x04000D79 RID: 3449
		public bool Display = true;

		// Token: 0x04000D7A RID: 3450
		public bool StillVisible;

		// Token: 0x04000D7B RID: 3451
		public bool ForceClose;

		// Token: 0x04000D7C RID: 3452
		private float ease;

		// Token: 0x04000D7D RID: 3453
		private float timer;

		// Token: 0x04000D7E RID: 3454
		private Sprite icon = GFX.GuiSpriteBank.Create("save");

		// Token: 0x04000D7F RID: 3455
		private float startTimer = 0.5f;

		// Token: 0x04000D80 RID: 3456
		private Wiggler wiggler;
	}
}
