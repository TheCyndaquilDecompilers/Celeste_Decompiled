using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200036D RID: 877
	public static class Dust
	{
		// Token: 0x06001B83 RID: 7043 RVA: 0x000B4070 File Offset: 0x000B2270
		public static void Burst(Vector2 position, float direction, int count = 1, ParticleType particleType = null)
		{
			if (particleType == null)
			{
				particleType = ParticleTypes.Dust;
			}
			Vector2 vector = Calc.AngleToVector(direction - 1.5707964f, 4f);
			vector.X = Math.Abs(vector.X);
			vector.Y = Math.Abs(vector.Y);
			Level level = Engine.Scene as Level;
			for (int i = 0; i < count; i++)
			{
				level.Particles.Emit(particleType, position + Calc.Random.Range(-vector, vector), direction);
			}
		}

		// Token: 0x06001B84 RID: 7044 RVA: 0x000B40F8 File Offset: 0x000B22F8
		public static void BurstFG(Vector2 position, float direction, int count = 1, float range = 4f, ParticleType particleType = null)
		{
			if (particleType == null)
			{
				particleType = ParticleTypes.Dust;
			}
			Vector2 vector = Calc.AngleToVector(direction - 1.5707964f, range);
			vector.X = Math.Abs(vector.X);
			vector.Y = Math.Abs(vector.Y);
			Level level = Engine.Scene as Level;
			for (int i = 0; i < count; i++)
			{
				level.ParticlesFG.Emit(particleType, position + Calc.Random.Range(-vector, vector), direction);
			}
		}
	}
}
