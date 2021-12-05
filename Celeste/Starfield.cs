using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001F5 RID: 501
	public class Starfield : Backdrop
	{
		// Token: 0x0600106F RID: 4207 RVA: 0x00049A84 File Offset: 0x00047C84
		public Starfield(Color color, float speed = 1f)
		{
			this.Color = color;
			this.FlowSpeed = speed;
			float num = Calc.Random.NextFloat(180f);
			int i = 0;
			while (i < 15)
			{
				this.YNodes.Add(num);
				i++;
				num += (float)Calc.Random.Choose(-1, 1) * (16f + Calc.Random.NextFloat(24f));
			}
			for (int j = 0; j < 4; j++)
			{
				this.YNodes[this.YNodes.Count - 1 - j] = Calc.LerpClamp(this.YNodes[this.YNodes.Count - 1 - j], this.YNodes[0], 1f - (float)j / 4f);
			}
			List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("particles/starfield/");
			for (int k = 0; k < this.Stars.Length; k++)
			{
				float num2 = Calc.Random.NextFloat(1f);
				this.Stars[k].NodeIndex = Calc.Random.Next(this.YNodes.Count - 1);
				this.Stars[k].NodePercent = Calc.Random.NextFloat(1f);
				this.Stars[k].Distance = 4f + num2 * 20f;
				this.Stars[k].Sine = Calc.Random.NextFloat(6.2831855f);
				this.Stars[k].Position = this.GetTargetOfStar(ref this.Stars[k]);
				this.Stars[k].Color = Color.Lerp(this.Color, Color.Transparent, num2 * 0.5f);
				int index = (int)Calc.Clamp(Ease.CubeIn(1f - num2) * (float)atlasSubtextures.Count, 0f, (float)(atlasSubtextures.Count - 1));
				this.Stars[k].Texture = atlasSubtextures[index];
			}
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x00049CD0 File Offset: 0x00047ED0
		public override void Update(Scene scene)
		{
			base.Update(scene);
			for (int i = 0; i < this.Stars.Length; i++)
			{
				this.UpdateStar(ref this.Stars[i]);
			}
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x00049D0C File Offset: 0x00047F0C
		private void UpdateStar(ref Starfield.Star star)
		{
			star.Sine += Engine.DeltaTime * this.FlowSpeed;
			star.NodePercent += Engine.DeltaTime * 0.25f * this.FlowSpeed;
			if (star.NodePercent >= 1f)
			{
				star.NodePercent -= 1f;
				star.NodeIndex++;
				if (star.NodeIndex >= this.YNodes.Count - 1)
				{
					star.NodeIndex = 0;
					star.Position.X = star.Position.X - 448f;
				}
			}
			star.Position += (this.GetTargetOfStar(ref star) - star.Position) / 50f;
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x00049DD4 File Offset: 0x00047FD4
		private Vector2 GetTargetOfStar(ref Starfield.Star star)
		{
			Vector2 vector = new Vector2((float)(star.NodeIndex * 32), this.YNodes[star.NodeIndex]);
			Vector2 value = new Vector2((float)((star.NodeIndex + 1) * 32), this.YNodes[star.NodeIndex + 1]);
			Vector2 value2 = vector + (value - vector) * star.NodePercent;
			Vector2 vector2 = (value - vector).SafeNormalize();
			Vector2 value3 = new Vector2(-vector2.Y, vector2.X);
			return value2 + value3 * star.Distance * (float)Math.Sin((double)star.Sine);
		}

		// Token: 0x06001073 RID: 4211 RVA: 0x00049E88 File Offset: 0x00048088
		public override void Render(Scene scene)
		{
			Vector2 position = (scene as Level).Camera.Position;
			for (int i = 0; i < this.Stars.Length; i++)
			{
				Vector2 position2 = new Vector2
				{
					X = -64f + this.Mod(this.Stars[i].Position.X - position.X * this.Scroll.X, 448f),
					Y = -16f + this.Mod(this.Stars[i].Position.Y - position.Y * this.Scroll.Y, 212f)
				};
				this.Stars[i].Texture.DrawCentered(position2, this.Stars[i].Color * this.FadeAlphaMultiplier);
			}
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x00026894 File Offset: 0x00024A94
		private float Mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000BF0 RID: 3056
		public const int StepSize = 32;

		// Token: 0x04000BF1 RID: 3057
		public const int Steps = 15;

		// Token: 0x04000BF2 RID: 3058
		public const float MinDist = 4f;

		// Token: 0x04000BF3 RID: 3059
		public const float MaxDist = 24f;

		// Token: 0x04000BF4 RID: 3060
		public float FlowSpeed;

		// Token: 0x04000BF5 RID: 3061
		public List<float> YNodes = new List<float>();

		// Token: 0x04000BF6 RID: 3062
		public Starfield.Star[] Stars = new Starfield.Star[128];

		// Token: 0x020004F4 RID: 1268
		public struct Star
		{
			// Token: 0x04002457 RID: 9303
			public MTexture Texture;

			// Token: 0x04002458 RID: 9304
			public Vector2 Position;

			// Token: 0x04002459 RID: 9305
			public Color Color;

			// Token: 0x0400245A RID: 9306
			public int NodeIndex;

			// Token: 0x0400245B RID: 9307
			public float NodePercent;

			// Token: 0x0400245C RID: 9308
			public float Distance;

			// Token: 0x0400245D RID: 9309
			public float Sine;
		}
	}
}
