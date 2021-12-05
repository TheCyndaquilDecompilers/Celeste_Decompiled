using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200022A RID: 554
	public class GondolaDarkness : Entity
	{
		// Token: 0x060011B6 RID: 4534 RVA: 0x000584CC File Offset: 0x000566CC
		public GondolaDarkness()
		{
			base.Add(this.sprite = GFX.SpriteBank.Create("gondolaDarkness"));
			this.sprite.Play("appear", false, false);
			base.Add(this.hands = GFX.SpriteBank.Create("gondolaHands"));
			this.hands.Visible = false;
			this.Visible = false;
			base.Depth = -999900;
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x0005854B File Offset: 0x0005674B
		public IEnumerator Appear(WindSnowFG windSnowFG = null)
		{
			this.windSnowFG = windSnowFG;
			this.Visible = true;
			base.Scene.Add(this.blackness = new GondolaDarkness.Blackness());
			for (float t = 0f; t < 1f; t += Engine.DeltaTime / 2f)
			{
				yield return null;
				this.blackness.Fade = t;
				this.anxiety = t;
				if (windSnowFG != null)
				{
					windSnowFG.Alpha = 1f - t;
				}
			}
			yield return null;
			yield break;
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x00058561 File Offset: 0x00056761
		public IEnumerator Expand()
		{
			this.hands.Visible = true;
			this.hands.Play("appear", false, false);
			yield return 1f;
			yield break;
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x00058570 File Offset: 0x00056770
		public IEnumerator Reach(Gondola gondola)
		{
			this.hands.Play("grab", false, false);
			yield return 0.4f;
			this.hands.Play("pull", false, false);
			gondola.PullSides();
			yield break;
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x00058588 File Offset: 0x00056788
		public override void Update()
		{
			base.Update();
			if (base.Scene.OnInterval(0.05f))
			{
				this.anxietyStutter = Calc.Random.NextFloat(0.1f);
			}
			Distort.AnxietyOrigin = new Vector2(0.5f, 0.5f);
			Distort.Anxiety = this.anxiety * 0.2f + this.anxietyStutter * this.anxiety;
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x000585F5 File Offset: 0x000567F5
		public override void Render()
		{
			this.Position = (base.Scene as Level).Camera.Position + (base.Scene as Level).ZoomFocusPoint;
			base.Render();
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x00058630 File Offset: 0x00056830
		public override void Removed(Scene scene)
		{
			this.anxiety = 0f;
			Distort.Anxiety = 0f;
			if (this.blackness != null)
			{
				this.blackness.RemoveSelf();
			}
			if (this.windSnowFG != null)
			{
				this.windSnowFG.Alpha = 1f;
			}
			base.Removed(scene);
		}

		// Token: 0x04000D5D RID: 3421
		private Sprite sprite;

		// Token: 0x04000D5E RID: 3422
		private Sprite hands;

		// Token: 0x04000D5F RID: 3423
		private GondolaDarkness.Blackness blackness;

		// Token: 0x04000D60 RID: 3424
		private float anxiety;

		// Token: 0x04000D61 RID: 3425
		private float anxietyStutter;

		// Token: 0x04000D62 RID: 3426
		private WindSnowFG windSnowFG;

		// Token: 0x02000548 RID: 1352
		private class Blackness : Entity
		{
			// Token: 0x060025E4 RID: 9700 RVA: 0x000FAE10 File Offset: 0x000F9010
			public Blackness()
			{
				base.Depth = 9001;
			}

			// Token: 0x060025E5 RID: 9701 RVA: 0x000FAE24 File Offset: 0x000F9024
			public override void Render()
			{
				base.Render();
				Camera camera = (base.Scene as Level).Camera;
				Draw.Rect(camera.Left - 1f, camera.Top - 1f, 322f, 182f, Color.Black * this.Fade);
			}

			// Token: 0x040025D7 RID: 9687
			public float Fade;
		}
	}
}
