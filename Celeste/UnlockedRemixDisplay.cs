using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000200 RID: 512
	public class UnlockedRemixDisplay : Entity
	{
		// Token: 0x060010BF RID: 4287 RVA: 0x0004E748 File Offset: 0x0004C948
		public UnlockedRemixDisplay()
		{
			base.Tag = (Tags.HUD | Tags.Global | Tags.PauseUpdate | Tags.TransitionUpdate);
			this.bg = GFX.Gui["strawberryCountBG"];
			this.icon = GFX.Gui["collectables/cassette"];
			this.text = Dialog.Clean("ui_remix_unlocked", null);
			this.Visible = false;
			base.Y = 96f;
		}

		// Token: 0x060010C0 RID: 4288 RVA: 0x0004E7DC File Offset: 0x0004C9DC
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.hasCassetteAlready = SaveData.Instance.Areas[AreaData.Get(base.Scene).ID].Cassette;
			this.unlockedRemix = (scene as Level).Session.Cassette;
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x0004E830 File Offset: 0x0004CA30
		public override void Update()
		{
			base.Update();
			if (!this.unlockedRemix && (base.Scene as Level).Session.Cassette)
			{
				this.unlockedRemix = true;
				base.Add(new Coroutine(this.DisplayRoutine(), true));
			}
			if (this.Visible)
			{
				float num = 96f;
				if (Settings.Instance.SpeedrunClock == SpeedrunType.Chapter)
				{
					num += 58f;
				}
				else if (Settings.Instance.SpeedrunClock == SpeedrunType.File)
				{
					num += 78f;
				}
				if (this.strawberries.Visible)
				{
					num += 96f;
				}
				base.Y = Calc.Approach(base.Y, num, Engine.DeltaTime * 800f);
			}
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x0004E8E7 File Offset: 0x0004CAE7
		private IEnumerator DisplayRoutine()
		{
			this.strawberries = base.Scene.Entities.FindFirst<TotalStrawberriesDisplay>();
			this.Visible = true;
			while ((this.drawLerp += Engine.DeltaTime * 1.2f) < 1f)
			{
				yield return null;
			}
			base.Add(this.wiggler = Wiggler.Create(0.8f, 4f, delegate(float f)
			{
				this.rotation = f * 0.1f;
			}, true, false));
			this.drawLerp = 1f;
			yield return 4f;
			while ((this.drawLerp -= Engine.DeltaTime * 2f) > 0f)
			{
				yield return null;
			}
			this.Visible = false;
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x060010C3 RID: 4291 RVA: 0x0004E8F8 File Offset: 0x0004CAF8
		public override void Render()
		{
			float num;
			if (this.hasCassetteAlready)
			{
				num = 188f;
			}
			else
			{
				num = ActiveFont.Measure(this.text).X + 128f + 80f;
			}
			Vector2 vector = Vector2.Lerp(new Vector2(-num, base.Y), new Vector2(0f, base.Y), Ease.CubeOut(this.drawLerp));
			this.bg.DrawJustified(vector + new Vector2(num, 0f), new Vector2(1f, 0.5f));
			Draw.Rect(vector.X, vector.Y - (float)(this.bg.Height / 2), num - (float)this.bg.Width + 1f, (float)this.bg.Height, Color.Black);
			float num2 = 128f / (float)this.icon.Width;
			this.icon.DrawJustified(vector + new Vector2(20f + (float)this.icon.Width * num2 * 0.5f, 0f), new Vector2(0.5f, 0.5f), Color.White, num2, this.rotation);
			if (!this.hasCassetteAlready)
			{
				ActiveFont.DrawOutline(this.text, vector + new Vector2(168f, 0f), new Vector2(0f, 0.6f), Vector2.One, Color.White, 2f, Color.Black);
			}
		}

		// Token: 0x04000C37 RID: 3127
		private const float DisplayDuration = 4f;

		// Token: 0x04000C38 RID: 3128
		private const float LerpInSpeed = 1.2f;

		// Token: 0x04000C39 RID: 3129
		private const float LerpOutSpeed = 2f;

		// Token: 0x04000C3A RID: 3130
		private const float IconSize = 128f;

		// Token: 0x04000C3B RID: 3131
		private const float Spacing = 20f;

		// Token: 0x04000C3C RID: 3132
		private string text;

		// Token: 0x04000C3D RID: 3133
		private float drawLerp;

		// Token: 0x04000C3E RID: 3134
		private MTexture bg;

		// Token: 0x04000C3F RID: 3135
		private MTexture icon;

		// Token: 0x04000C40 RID: 3136
		private float rotation;

		// Token: 0x04000C41 RID: 3137
		private bool unlockedRemix;

		// Token: 0x04000C42 RID: 3138
		private TotalStrawberriesDisplay strawberries;

		// Token: 0x04000C43 RID: 3139
		private Wiggler wiggler;

		// Token: 0x04000C44 RID: 3140
		private bool hasCassetteAlready;
	}
}
