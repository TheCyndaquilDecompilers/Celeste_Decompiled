using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x02000118 RID: 280
	public class ParticleSystem : Entity
	{
		// Token: 0x060008C9 RID: 2249 RVA: 0x00014978 File Offset: 0x00012B78
		public ParticleSystem(int depth, int maxParticles)
		{
			this.particles = new Particle[maxParticles];
			base.Depth = depth;
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x00014994 File Offset: 0x00012B94
		public void Clear()
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.particles[i].Active = false;
			}
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x000149C8 File Offset: 0x00012BC8
		public void ClearRect(Rectangle rect, bool inside)
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				Vector2 position = this.particles[i].Position;
				if ((position.X > (float)rect.Left && position.Y > (float)rect.Top && position.X < (float)rect.Right && position.Y < (float)rect.Bottom) == inside)
				{
					this.particles[i].Active = false;
				}
			}
		}

		// Token: 0x060008CC RID: 2252 RVA: 0x00014A54 File Offset: 0x00012C54
		public override void Update()
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				if (this.particles[i].Active)
				{
					this.particles[i].Update(null);
				}
			}
		}

		// Token: 0x060008CD RID: 2253 RVA: 0x00014AA4 File Offset: 0x00012CA4
		public override void Render()
		{
			foreach (Particle particle in this.particles)
			{
				if (particle.Active)
				{
					particle.Render();
				}
			}
		}

		// Token: 0x060008CE RID: 2254 RVA: 0x00014AE0 File Offset: 0x00012CE0
		public void Render(float alpha)
		{
			foreach (Particle particle in this.particles)
			{
				if (particle.Active)
				{
					particle.Render(alpha);
				}
			}
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x00014B1C File Offset: 0x00012D1C
		public void Simulate(float duration, float interval, Action<ParticleSystem> emitter)
		{
			float num = 0.016f;
			for (float num2 = 0f; num2 < duration; num2 += num)
			{
				if ((int)((num2 - num) / interval) < (int)(num2 / interval))
				{
					emitter(this);
				}
				for (int i = 0; i < this.particles.Length; i++)
				{
					if (this.particles[i].Active)
					{
						this.particles[i].Update(new float?(num));
					}
				}
			}
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x00014B8F File Offset: 0x00012D8F
		public void Add(Particle particle)
		{
			this.particles[this.nextSlot] = particle;
			this.nextSlot = (this.nextSlot + 1) % this.particles.Length;
		}

		// Token: 0x060008D1 RID: 2257 RVA: 0x00014BBA File Offset: 0x00012DBA
		public void Emit(ParticleType type, Vector2 position)
		{
			type.Create(ref this.particles[this.nextSlot], position);
			this.nextSlot = (this.nextSlot + 1) % this.particles.Length;
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x00014BEC File Offset: 0x00012DEC
		public void Emit(ParticleType type, Vector2 position, float direction)
		{
			type.Create(ref this.particles[this.nextSlot], position, direction);
			this.nextSlot = (this.nextSlot + 1) % this.particles.Length;
		}

		// Token: 0x060008D3 RID: 2259 RVA: 0x00014C1F File Offset: 0x00012E1F
		public void Emit(ParticleType type, Vector2 position, Color color)
		{
			type.Create(ref this.particles[this.nextSlot], position, color);
			this.nextSlot = (this.nextSlot + 1) % this.particles.Length;
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x00014C52 File Offset: 0x00012E52
		public void Emit(ParticleType type, Vector2 position, Color color, float direction)
		{
			type.Create(ref this.particles[this.nextSlot], position, color, direction);
			this.nextSlot = (this.nextSlot + 1) % this.particles.Length;
		}

		// Token: 0x060008D5 RID: 2261 RVA: 0x00014C88 File Offset: 0x00012E88
		public void Emit(ParticleType type, int amount, Vector2 position, Vector2 positionRange)
		{
			for (int i = 0; i < amount; i++)
			{
				this.Emit(type, Calc.Random.Range(position - positionRange, position + positionRange));
			}
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x00014CC4 File Offset: 0x00012EC4
		public void Emit(ParticleType type, int amount, Vector2 position, Vector2 positionRange, float direction)
		{
			for (int i = 0; i < amount; i++)
			{
				this.Emit(type, Calc.Random.Range(position - positionRange, position + positionRange), direction);
			}
		}

		// Token: 0x060008D7 RID: 2263 RVA: 0x00014D00 File Offset: 0x00012F00
		public void Emit(ParticleType type, int amount, Vector2 position, Vector2 positionRange, Color color)
		{
			for (int i = 0; i < amount; i++)
			{
				this.Emit(type, Calc.Random.Range(position - positionRange, position + positionRange), color);
			}
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x00014D3C File Offset: 0x00012F3C
		public void Emit(ParticleType type, int amount, Vector2 position, Vector2 positionRange, Color color, float direction)
		{
			for (int i = 0; i < amount; i++)
			{
				this.Emit(type, Calc.Random.Range(position - positionRange, position + positionRange), color, direction);
			}
		}

		// Token: 0x060008D9 RID: 2265 RVA: 0x00014D7C File Offset: 0x00012F7C
		public void Emit(ParticleType type, Entity track, int amount, Vector2 position, Vector2 positionRange, float direction)
		{
			for (int i = 0; i < amount; i++)
			{
				type.Create(ref this.particles[this.nextSlot], track, Calc.Random.Range(position - positionRange, position + positionRange), direction, type.Color);
				this.nextSlot = (this.nextSlot + 1) % this.particles.Length;
			}
		}

		// Token: 0x040005E3 RID: 1507
		private Particle[] particles;

		// Token: 0x040005E4 RID: 1508
		private int nextSlot;
	}
}
