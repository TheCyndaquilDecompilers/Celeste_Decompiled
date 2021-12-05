using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000270 RID: 624
	public class NPC03_Theo_Escaping : NPC
	{
		// Token: 0x06001362 RID: 4962 RVA: 0x00069830 File Offset: 0x00067A30
		public NPC03_Theo_Escaping(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("theo"));
			this.Sprite.Play("idle", false, false);
			this.Sprite.X = -4f;
			base.SetupTheoSpriteSounds();
		}

		// Token: 0x06001363 RID: 4963 RVA: 0x0006988C File Offset: 0x00067A8C
		public override void Added(Scene scene)
		{
			base.Added(scene);
			base.Add(this.light = new VertexLight(base.Center - this.Position, Color.White, 1f, 32, 64));
			while (!base.CollideCheck<Solid>(this.Position + new Vector2(1f, 0f)))
			{
				float x = base.X;
				base.X = x + 1f;
			}
			this.grate = new NPC03_Theo_Escaping.Grate(this.Position + new Vector2(base.Width / 2f, -8f));
			base.Scene.Add(this.grate);
			this.Sprite.Play("goToVent", false, false);
		}

		// Token: 0x06001364 RID: 4964 RVA: 0x0006995C File Offset: 0x00067B5C
		public override void Update()
		{
			base.Update();
			Player player = base.Scene.Entities.FindFirst<Player>();
			if (player != null && !this.talked && player.X > base.X - 100f)
			{
				this.Talk(player);
			}
			if (this.Sprite.CurrentAnimationID == "pullVent" && this.Sprite.CurrentAnimationFrame > 0)
			{
				this.grate.Sprite.X = 0f;
				return;
			}
			this.grate.Sprite.X = 1f;
		}

		// Token: 0x06001365 RID: 4965 RVA: 0x000699F6 File Offset: 0x00067BF6
		private void Talk(Player player)
		{
			this.talked = true;
			base.Scene.Add(new CS03_TheoEscape(this, player));
		}

		// Token: 0x06001366 RID: 4966 RVA: 0x00069A11 File Offset: 0x00067C11
		public void CrawlUntilOut()
		{
			this.Sprite.Scale.X = 1f;
			this.Sprite.Play("crawl", false, false);
			base.Add(new Coroutine(this.CrawlUntilOutRoutine(), true));
		}

		// Token: 0x06001367 RID: 4967 RVA: 0x00069A4C File Offset: 0x00067C4C
		private IEnumerator CrawlUntilOutRoutine()
		{
			base.AddTag(Tags.Global);
			int target = (base.Scene as Level).Bounds.Right + 280;
			while (base.X != (float)target)
			{
				base.X = Calc.Approach(base.X, (float)target, 20f * Engine.DeltaTime);
				yield return null;
			}
			base.Scene.Remove(this);
			yield break;
		}

		// Token: 0x04000F44 RID: 3908
		private bool talked;

		// Token: 0x04000F45 RID: 3909
		private VertexLight light;

		// Token: 0x04000F46 RID: 3910
		public NPC03_Theo_Escaping.Grate grate;

		// Token: 0x020005AB RID: 1451
		public class Grate : Entity
		{
			// Token: 0x060027E8 RID: 10216 RVA: 0x001046C0 File Offset: 0x001028C0
			public Grate(Vector2 position) : base(position)
			{
				base.Add(this.Sprite = new Image(GFX.Game["scenery/grate"]));
				this.Sprite.JustifyOrigin(0.5f, 0f);
				this.Sprite.Rotation = 1.5707964f;
			}

			// Token: 0x060027E9 RID: 10217 RVA: 0x00104728 File Offset: 0x00102928
			public void Fall()
			{
				Audio.Play("event:/char/theo/resort_vent_tumble", this.Position);
				this.falling = true;
				this.speed = new Vector2(-120f, -120f);
				base.Collider = new Hitbox(2f, 2f, -2f, -1f);
			}

			// Token: 0x060027EA RID: 10218 RVA: 0x00104784 File Offset: 0x00102984
			public override void Update()
			{
				if (this.falling)
				{
					this.speed.X = Calc.Approach(this.speed.X, 0f, Engine.DeltaTime * 120f);
					this.speed.Y = this.speed.Y + 400f * Engine.DeltaTime;
					this.Position += this.speed * Engine.DeltaTime;
					if (base.CollideCheck<Solid>(this.Position + new Vector2(0f, 2f)) && this.speed.Y > 0f)
					{
						this.speed.Y = -this.speed.Y * 0.25f;
					}
					this.alpha -= Engine.DeltaTime;
					this.Sprite.Rotation += Engine.DeltaTime * this.speed.Length() * 0.05f;
					this.Sprite.Color = Color.White * this.alpha;
					if (this.alpha <= 0f)
					{
						base.RemoveSelf();
					}
				}
				base.Update();
			}

			// Token: 0x04002788 RID: 10120
			public Image Sprite;

			// Token: 0x04002789 RID: 10121
			private Vector2 speed;

			// Token: 0x0400278A RID: 10122
			private bool falling;

			// Token: 0x0400278B RID: 10123
			private float alpha = 1f;
		}
	}
}
