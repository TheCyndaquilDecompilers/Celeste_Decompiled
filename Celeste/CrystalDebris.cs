using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001B3 RID: 435
	[Pooled]
	public class CrystalDebris : Actor
	{
		// Token: 0x06000F2A RID: 3882 RVA: 0x0003CD2C File Offset: 0x0003AF2C
		public CrystalDebris() : base(Vector2.Zero)
		{
			base.Depth = -9990;
			base.Collider = new Hitbox(2f, 2f, -1f, -1f);
			this.collideH = new Collision(this.OnCollideH);
			this.collideV = new Collision(this.OnCollideV);
			this.image = new Image(GFX.Game["particles/shard"]);
			this.image.CenterOrigin();
			base.Add(this.image);
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x0003CDC4 File Offset: 0x0003AFC4
		private void Init(Vector2 position, Color color, bool boss)
		{
			this.Position = position;
			GraphicsComponent graphicsComponent = this.image;
			this.color = color;
			graphicsComponent.Color = color;
			this.image.Scale = Vector2.One;
			this.percent = 0f;
			this.duration = (boss ? Calc.Random.Range(0.25f, 1f) : Calc.Random.Range(1f, 2f));
			this.speed = Calc.AngleToVector(Calc.Random.NextAngle(), (float)(boss ? Calc.Random.Range(200, 240) : Calc.Random.Range(60, 160)));
			this.bossShatter = boss;
		}

		// Token: 0x06000F2C RID: 3884 RVA: 0x0003CE84 File Offset: 0x0003B084
		public override void Update()
		{
			base.Update();
			if (this.percent > 1f)
			{
				base.RemoveSelf();
				return;
			}
			this.percent += Engine.DeltaTime / this.duration;
			if (!this.bossShatter)
			{
				this.speed.X = Calc.Approach(this.speed.X, 0f, Engine.DeltaTime * 20f);
				this.speed.Y = this.speed.Y + 200f * Engine.DeltaTime;
			}
			else
			{
				float num = this.speed.Length();
				num = Calc.Approach(num, 0f, 300f * Engine.DeltaTime);
				this.speed = this.speed.SafeNormalize() * num;
			}
			if (this.speed.Length() > 0f)
			{
				this.image.Rotation = this.speed.Angle();
			}
			this.image.Scale = Vector2.One * Calc.ClampedMap(this.percent, 0.8f, 1f, 1f, 0f);
			Image image = this.image;
			image.Scale.X = image.Scale.X * Calc.ClampedMap(this.speed.Length(), 0f, 400f, 1f, 2f);
			Image image2 = this.image;
			image2.Scale.Y = image2.Scale.Y * Calc.ClampedMap(this.speed.Length(), 0f, 400f, 1f, 0.2f);
			base.MoveH(this.speed.X * Engine.DeltaTime, this.collideH, null);
			base.MoveV(this.speed.Y * Engine.DeltaTime, this.collideV, null);
			if (base.Scene.OnInterval(0.05f))
			{
				(base.Scene as Level).ParticlesFG.Emit(CrystalDebris.P_Dust, this.Position);
			}
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x0003D08C File Offset: 0x0003B28C
		public override void Render()
		{
			Color color = this.image.Color;
			this.image.Color = Color.Black;
			this.image.Position = new Vector2(-1f, 0f);
			this.image.Render();
			this.image.Position = new Vector2(0f, -1f);
			this.image.Render();
			this.image.Position = new Vector2(1f, 0f);
			this.image.Render();
			this.image.Position = new Vector2(0f, 1f);
			this.image.Render();
			this.image.Position = Vector2.Zero;
			this.image.Color = color;
			base.Render();
		}

		// Token: 0x06000F2E RID: 3886 RVA: 0x0003D16B File Offset: 0x0003B36B
		private void OnCollideH(CollisionData hit)
		{
			this.speed.X = this.speed.X * -0.8f;
		}

		// Token: 0x06000F2F RID: 3887 RVA: 0x0003D184 File Offset: 0x0003B384
		private void OnCollideV(CollisionData hit)
		{
			if (this.bossShatter)
			{
				base.RemoveSelf();
				return;
			}
			if (Math.Sign(this.speed.X) != 0)
			{
				this.speed.X = this.speed.X + (float)(Math.Sign(this.speed.X) * 5);
			}
			else
			{
				this.speed.X = this.speed.X + (float)(Calc.Random.Choose(-1, 1) * 5);
			}
			this.speed.Y = this.speed.Y * -1.2f;
		}

		// Token: 0x06000F30 RID: 3888 RVA: 0x0003D208 File Offset: 0x0003B408
		public static void Burst(Vector2 position, Color color, bool boss, int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				CrystalDebris crystalDebris = Engine.Pooler.Create<CrystalDebris>();
				Vector2 position2 = position + new Vector2((float)Calc.Random.Range(-4, 4), (float)Calc.Random.Range(-4, 4));
				crystalDebris.Init(position2, color, boss);
				Engine.Scene.Add(crystalDebris);
			}
		}

		// Token: 0x04000A88 RID: 2696
		public static ParticleType P_Dust;

		// Token: 0x04000A89 RID: 2697
		private Image image;

		// Token: 0x04000A8A RID: 2698
		private float percent;

		// Token: 0x04000A8B RID: 2699
		private float duration;

		// Token: 0x04000A8C RID: 2700
		private Vector2 speed;

		// Token: 0x04000A8D RID: 2701
		private Collision collideH;

		// Token: 0x04000A8E RID: 2702
		private Collision collideV;

		// Token: 0x04000A8F RID: 2703
		private Color color;

		// Token: 0x04000A90 RID: 2704
		private bool bossShatter;
	}
}
