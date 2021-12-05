using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001F2 RID: 498
	public class Planets : Backdrop
	{
		// Token: 0x06001062 RID: 4194 RVA: 0x00049194 File Offset: 0x00047394
		public Planets(int count, string size)
		{
			List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("bgs/10/" + size);
			this.planets = new Planets.Planet[count];
			for (int i = 0; i < this.planets.Length; i++)
			{
				this.planets[i].Texture = Calc.Random.Choose(atlasSubtextures);
				this.planets[i].Position = new Vector2
				{
					X = Calc.Random.NextFloat(640f),
					Y = Calc.Random.NextFloat(360f)
				};
			}
		}

		// Token: 0x06001063 RID: 4195 RVA: 0x00049240 File Offset: 0x00047440
		public override void Render(Scene scene)
		{
			Vector2 position = (scene as Level).Camera.Position;
			Color color = this.Color * this.FadeAlphaMultiplier;
			for (int i = 0; i < this.planets.Length; i++)
			{
				Vector2 position2 = new Vector2
				{
					X = -32f + this.Mod(this.planets[i].Position.X - position.X * this.Scroll.X, 640f),
					Y = -32f + this.Mod(this.planets[i].Position.Y - position.Y * this.Scroll.Y, 360f)
				};
				this.planets[i].Texture.DrawCentered(position2, color);
			}
		}

		// Token: 0x06001064 RID: 4196 RVA: 0x00026894 File Offset: 0x00024A94
		private float Mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000BE5 RID: 3045
		private Planets.Planet[] planets;

		// Token: 0x04000BE6 RID: 3046
		public const int MapWidth = 640;

		// Token: 0x04000BE7 RID: 3047
		public const int MapHeight = 360;

		// Token: 0x020004F1 RID: 1265
		private struct Planet
		{
			// Token: 0x0400244A RID: 9290
			public MTexture Texture;

			// Token: 0x0400244B RID: 9291
			public Vector2 Position;
		}
	}
}
