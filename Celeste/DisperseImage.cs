using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001F6 RID: 502
	public class DisperseImage : Entity
	{
		// Token: 0x06001075 RID: 4213 RVA: 0x00049F80 File Offset: 0x00048180
		public DisperseImage(Vector2 position, Vector2 direction, Vector2 origin, Vector2 scale, MTexture texture)
		{
			this.Position = position;
			this.scale = new Vector2(Math.Abs(scale.X), Math.Abs(scale.Y));
			float num = direction.Angle();
			for (int i = 0; i < texture.Width; i++)
			{
				for (int j = 0; j < texture.Height; j++)
				{
					this.particles.Add(new DisperseImage.Particle
					{
						Position = position + scale * (new Vector2((float)i, (float)j) - origin),
						Direction = Calc.AngleToVector(num + Calc.Random.Range(-0.2f, 0.2f), 1f),
						Sin = Calc.Random.NextFloat(6.2831855f),
						Speed = Calc.Random.Range(0f, 4f),
						Alpha = 1f,
						Percent = 0f,
						Duration = Calc.Random.Range(1f, 3f),
						Image = new MTexture(texture, i, j, 1, 1)
					});
				}
			}
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x0004A0C8 File Offset: 0x000482C8
		public override void Update()
		{
			bool flag = false;
			foreach (DisperseImage.Particle particle in this.particles)
			{
				particle.Percent += Engine.DeltaTime / particle.Duration;
				particle.Position += particle.Direction * particle.Speed * Engine.DeltaTime;
				particle.Position += (float)Math.Sin((double)particle.Sin) * particle.Direction.Perpendicular() * particle.Percent * 4f * Engine.DeltaTime;
				particle.Speed += Engine.DeltaTime * (4f + particle.Percent * 80f);
				particle.Sin += Engine.DeltaTime * 4f;
				if (particle.Percent < 1f)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06001077 RID: 4215 RVA: 0x0004A204 File Offset: 0x00048404
		public override void Render()
		{
			foreach (DisperseImage.Particle particle in this.particles)
			{
				particle.Image.Draw(particle.Position, Vector2.Zero, Color.White * (1f - particle.Percent), this.scale);
			}
		}

		// Token: 0x04000BF7 RID: 3063
		private List<DisperseImage.Particle> particles = new List<DisperseImage.Particle>();

		// Token: 0x04000BF8 RID: 3064
		private Vector2 scale;

		// Token: 0x020004F5 RID: 1269
		private class Particle
		{
			// Token: 0x0400245E RID: 9310
			public Vector2 Position;

			// Token: 0x0400245F RID: 9311
			public Vector2 Direction;

			// Token: 0x04002460 RID: 9312
			public float Speed;

			// Token: 0x04002461 RID: 9313
			public float Sin;

			// Token: 0x04002462 RID: 9314
			public float Alpha;

			// Token: 0x04002463 RID: 9315
			public float Percent;

			// Token: 0x04002464 RID: 9316
			public float Duration;

			// Token: 0x04002465 RID: 9317
			public MTexture Image;
		}
	}
}
