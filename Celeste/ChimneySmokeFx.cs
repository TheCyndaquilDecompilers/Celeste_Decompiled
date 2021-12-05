using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200036C RID: 876
	public class ChimneySmokeFx
	{
		// Token: 0x06001B81 RID: 7041 RVA: 0x000B3FD4 File Offset: 0x000B21D4
		public static void Burst(Vector2 position, float direction, int count, ParticleSystem system = null)
		{
			Vector2 vector = Calc.AngleToVector(direction - 1.5707964f, 2f);
			vector.X = Math.Abs(vector.X);
			vector.Y = Math.Abs(vector.Y);
			if (system == null)
			{
				system = (Engine.Scene as Level).ParticlesFG;
			}
			for (int i = 0; i < count; i++)
			{
				system.Emit(Calc.Random.Choose(new ParticleType[]
				{
					ParticleTypes.Chimney
				}), position + Calc.Random.Range(-vector, vector), direction);
			}
		}
	}
}
