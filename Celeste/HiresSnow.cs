using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000233 RID: 563
	public class HiresSnow : Renderer
	{
		// Token: 0x060011E6 RID: 4582 RVA: 0x00059D50 File Offset: 0x00057F50
		public HiresSnow(float overlayAlpha = 0.45f)
		{
			this.overlayAlpha = overlayAlpha;
			this.overlay = OVR.Atlas["overlay"];
			this.snow = OVR.Atlas["snow"].GetSubtexture(1, 1, 254, 254, null);
			this.particles = new HiresSnow.Particle[50];
			this.Reset();
		}

		// Token: 0x060011E7 RID: 4583 RVA: 0x00059DE4 File Offset: 0x00057FE4
		public void Reset()
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.particles[i].Reset(this.Direction);
				this.particles[i].Position.X = Calc.Random.NextFloat((float)Engine.Width);
				this.particles[i].Position.Y = Calc.Random.NextFloat((float)Engine.Height);
			}
		}

		// Token: 0x060011E8 RID: 4584 RVA: 0x00059E68 File Offset: 0x00058068
		public override void Update(Scene scene)
		{
			base.Update(scene);
			if (this.AttachAlphaTo != null)
			{
				this.Alpha = this.AttachAlphaTo.Percent;
			}
			for (int i = 0; i < this.particles.Length; i++)
			{
				HiresSnow.Particle[] array = this.particles;
				int num = i;
				array[num].Position = array[num].Position + this.Direction * this.particles[i].Speed * Engine.DeltaTime;
				HiresSnow.Particle[] array2 = this.particles;
				int num2 = i;
				array2[num2].Position.Y = array2[num2].Position.Y + (float)Math.Sin((double)this.particles[i].Sin) * 100f * Engine.DeltaTime;
				HiresSnow.Particle[] array3 = this.particles;
				int num3 = i;
				array3[num3].Sin = array3[num3].Sin + Engine.DeltaTime;
				if (this.particles[i].Position.X < -128f || this.particles[i].Position.X > (float)(Engine.Width + 128) || this.particles[i].Position.Y < -128f || this.particles[i].Position.Y > (float)(Engine.Height + 128))
				{
					this.particles[i].Reset(this.Direction);
				}
			}
			this.timer += Engine.DeltaTime;
		}

		// Token: 0x060011E9 RID: 4585 RVA: 0x00059FF8 File Offset: 0x000581F8
		public override void Render(Scene scene)
		{
			float num = Calc.Clamp(this.Direction.Length(), 0f, 20f);
			float num2 = 0f;
			Vector2 one = Vector2.One;
			bool flag = num > 1f;
			if (flag)
			{
				num2 = this.Direction.Angle();
				one = new Vector2(num, 0.2f + (1f - num / 20f) * 0.8f);
			}
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap, null, null, null, Engine.ScreenMatrix);
			float num3 = this.Alpha * this.ParticleAlpha;
			for (int i = 0; i < this.particles.Length; i++)
			{
				Color color = this.particles[i].Color;
				float rotation = this.particles[i].Rotation;
				if (num3 < 1f)
				{
					color *= num3;
				}
				this.snow.DrawCentered(this.particles[i].Position, color, one * this.particles[i].Scale, flag ? num2 : rotation);
			}
			float num4 = this.timer * 32f % (float)this.overlay.Width;
			float num5 = this.timer * 20f % (float)this.overlay.Height;
			Color color2 = Color.White * (this.Alpha * this.overlayAlpha);
			Draw.SpriteBatch.Draw(this.overlay.Texture.Texture, Vector2.Zero, new Rectangle?(new Rectangle(-(int)num4, -(int)num5, 1920, 1080)), color2);
			Draw.SpriteBatch.End();
		}

		// Token: 0x04000D93 RID: 3475
		public float Alpha = 1f;

		// Token: 0x04000D94 RID: 3476
		public float ParticleAlpha = 1f;

		// Token: 0x04000D95 RID: 3477
		public Vector2 Direction = new Vector2(-1f, 0f);

		// Token: 0x04000D96 RID: 3478
		public ScreenWipe AttachAlphaTo;

		// Token: 0x04000D97 RID: 3479
		private HiresSnow.Particle[] particles;

		// Token: 0x04000D98 RID: 3480
		private MTexture overlay;

		// Token: 0x04000D99 RID: 3481
		private MTexture snow;

		// Token: 0x04000D9A RID: 3482
		private float timer;

		// Token: 0x04000D9B RID: 3483
		private float overlayAlpha;

		// Token: 0x02000553 RID: 1363
		private struct Particle
		{
			// Token: 0x0600260B RID: 9739 RVA: 0x000FB6C0 File Offset: 0x000F98C0
			public void Reset(Vector2 direction)
			{
				float num = Calc.Random.NextFloat();
				num *= num * num * num;
				this.Scale = Calc.Map(num, 0f, 1f, 0.05f, 0.8f);
				this.Speed = this.Scale * (float)Calc.Random.Range(2500, 5000);
				if (direction.X < 0f)
				{
					this.Position = new Vector2((float)(Engine.Width + 128), Calc.Random.NextFloat((float)Engine.Height));
				}
				else if (direction.X > 0f)
				{
					this.Position = new Vector2(-128f, Calc.Random.NextFloat((float)Engine.Height));
				}
				else if (direction.Y > 0f)
				{
					this.Position = new Vector2(Calc.Random.NextFloat((float)Engine.Width), -128f);
				}
				else if (direction.Y < 0f)
				{
					this.Position = new Vector2(Calc.Random.NextFloat((float)Engine.Width), (float)(Engine.Height + 128));
				}
				this.Sin = Calc.Random.NextFloat(6.2831855f);
				this.Rotation = Calc.Random.NextFloat(6.2831855f);
				this.Color = Color.Lerp(Color.White, Color.Transparent, num * 0.8f);
			}

			// Token: 0x040025FD RID: 9725
			public float Scale;

			// Token: 0x040025FE RID: 9726
			public Vector2 Position;

			// Token: 0x040025FF RID: 9727
			public float Speed;

			// Token: 0x04002600 RID: 9728
			public float Sin;

			// Token: 0x04002601 RID: 9729
			public float Rotation;

			// Token: 0x04002602 RID: 9730
			public Color Color;
		}
	}
}
