using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x020000E8 RID: 232
	public class ParticleEmitter : Component
	{
		// Token: 0x0600052E RID: 1326 RVA: 0x00006DBC File Offset: 0x00004FBC
		public ParticleEmitter(ParticleSystem system, ParticleType type, Vector2 position, Vector2 range, int amount, float interval) : base(true, false)
		{
			this.System = system;
			this.Type = type;
			this.Position = position;
			this.Range = range;
			this.Amount = amount;
			this.Interval = interval;
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x00006DF3 File Offset: 0x00004FF3
		public ParticleEmitter(ParticleSystem system, ParticleType type, Vector2 position, Vector2 range, float direction, int amount, float interval) : this(system, type, position, range, amount, interval)
		{
			this.Direction = new float?(direction);
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x00006E11 File Offset: 0x00005011
		public ParticleEmitter(ParticleSystem system, ParticleType type, Entity track, Vector2 position, Vector2 range, float direction, int amount, float interval) : this(system, type, position, range, amount, interval)
		{
			this.Direction = new float?(direction);
			this.Track = track;
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x00006E37 File Offset: 0x00005037
		public void SimulateCycle()
		{
			this.Simulate(this.Type.LifeMax);
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x00006E4C File Offset: 0x0000504C
		public void Simulate(float duration)
		{
			float num = duration / this.Interval;
			int num2 = 0;
			while ((float)num2 < num)
			{
				for (int i = 0; i < this.Amount; i++)
				{
					Particle particle = default(Particle);
					Vector2 position = base.Entity.Position + this.Position + Calc.Random.Range(-this.Range, this.Range);
					if (this.Direction != null)
					{
						particle = this.Type.Create(ref particle, position, this.Direction.Value);
					}
					else
					{
						particle = this.Type.Create(ref particle, position);
					}
					particle.Track = this.Track;
					float duration2 = duration - this.Interval * (float)num2;
					if (particle.SimulateFor(duration2))
					{
						this.System.Add(particle);
					}
				}
				num2++;
			}
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x00006F38 File Offset: 0x00005138
		public void Emit()
		{
			if (this.Direction != null)
			{
				this.System.Emit(this.Type, this.Amount, base.Entity.Position + this.Position, this.Range, this.Direction.Value);
				return;
			}
			this.System.Emit(this.Type, this.Amount, base.Entity.Position + this.Position, this.Range);
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x00006FC4 File Offset: 0x000051C4
		public override void Update()
		{
			this.timer -= Engine.DeltaTime;
			if (this.timer <= 0f)
			{
				this.timer = this.Interval;
				this.Emit();
			}
		}

		// Token: 0x0400048E RID: 1166
		public ParticleSystem System;

		// Token: 0x0400048F RID: 1167
		public ParticleType Type;

		// Token: 0x04000490 RID: 1168
		public Entity Track;

		// Token: 0x04000491 RID: 1169
		public float Interval;

		// Token: 0x04000492 RID: 1170
		public Vector2 Position;

		// Token: 0x04000493 RID: 1171
		public Vector2 Range;

		// Token: 0x04000494 RID: 1172
		public int Amount;

		// Token: 0x04000495 RID: 1173
		public float? Direction;

		// Token: 0x04000496 RID: 1174
		private float timer;
	}
}
