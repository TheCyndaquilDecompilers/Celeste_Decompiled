using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002E5 RID: 741
	public class StarsBG : Backdrop
	{
		// Token: 0x060016DA RID: 5850 RVA: 0x00087EF8 File Offset: 0x000860F8
		public StarsBG()
		{
			this.textures = new List<List<MTexture>>();
			this.textures.Add(GFX.Game.GetAtlasSubtextures("bgs/02/stars/a"));
			this.textures.Add(GFX.Game.GetAtlasSubtextures("bgs/02/stars/b"));
			this.textures.Add(GFX.Game.GetAtlasSubtextures("bgs/02/stars/c"));
			this.center = new Vector2((float)this.textures[0][0].Width, (float)this.textures[0][0].Height) / 2f;
			this.stars = new StarsBG.Star[100];
			for (int i = 0; i < this.stars.Length; i++)
			{
				this.stars[i] = new StarsBG.Star
				{
					Position = new Vector2(Calc.Random.NextFloat(320f), Calc.Random.NextFloat(180f)),
					Timer = Calc.Random.NextFloat(6.2831855f),
					Rate = 2f + Calc.Random.NextFloat(2f),
					TextureSet = Calc.Random.Next(this.textures.Count)
				};
			}
			this.colors = new Color[8];
			for (int j = 0; j < this.colors.Length; j++)
			{
				this.colors[j] = Color.Teal * 0.7f * (1f - (float)j / (float)this.colors.Length);
			}
		}

		// Token: 0x060016DB RID: 5851 RVA: 0x000880B0 File Offset: 0x000862B0
		public override void Update(Scene scene)
		{
			base.Update(scene);
			if (this.Visible)
			{
				Level level = scene as Level;
				for (int i = 0; i < this.stars.Length; i++)
				{
					StarsBG.Star[] array = this.stars;
					int num = i;
					array[num].Timer = array[num].Timer + Engine.DeltaTime * this.stars[i].Rate;
				}
				if (level.Session.Dreaming)
				{
					this.falling += Engine.DeltaTime * 12f;
				}
			}
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x00088138 File Offset: 0x00086338
		public override void Render(Scene scene)
		{
			Draw.Rect(0f, 0f, 320f, 180f, Color.Black);
			Level level = scene as Level;
			Color color = Color.White;
			int num = 100;
			if (level.Session.Dreaming)
			{
				color = Color.Teal * 0.7f;
			}
			else
			{
				num /= 2;
			}
			for (int i = 0; i < num; i++)
			{
				List<MTexture> list = this.textures[this.stars[i].TextureSet];
				int num2 = (int)((Math.Sin((double)this.stars[i].Timer) + 1.0) / 2.0 * (double)list.Count);
				num2 %= list.Count;
				Vector2 position = this.stars[i].Position;
				MTexture mtexture = list[num2];
				if (level.Session.Dreaming)
				{
					position.Y -= level.Camera.Y;
					position.Y += this.falling * this.stars[i].Rate;
					position.Y %= 180f;
					if (position.Y < 0f)
					{
						position.Y += 180f;
					}
					for (int j = 0; j < this.colors.Length; j++)
					{
						mtexture.Draw(position - Vector2.UnitY * (float)j, this.center, this.colors[j]);
					}
				}
				mtexture.Draw(position, this.center, color);
			}
		}

		// Token: 0x04001359 RID: 4953
		private const int StarCount = 100;

		// Token: 0x0400135A RID: 4954
		private StarsBG.Star[] stars;

		// Token: 0x0400135B RID: 4955
		private Color[] colors;

		// Token: 0x0400135C RID: 4956
		private List<List<MTexture>> textures;

		// Token: 0x0400135D RID: 4957
		private float falling;

		// Token: 0x0400135E RID: 4958
		private Vector2 center;

		// Token: 0x0200068F RID: 1679
		private struct Star
		{
			// Token: 0x04002B2F RID: 11055
			public Vector2 Position;

			// Token: 0x04002B30 RID: 11056
			public int TextureSet;

			// Token: 0x04002B31 RID: 11057
			public float Timer;

			// Token: 0x04002B32 RID: 11058
			public float Rate;
		}
	}
}
