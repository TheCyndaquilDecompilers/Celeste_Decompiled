using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002E3 RID: 739
	public class DreamStars : Backdrop
	{
		// Token: 0x060016CA RID: 5834 RVA: 0x0008722C File Offset: 0x0008542C
		public DreamStars()
		{
			for (int i = 0; i < this.stars.Length; i++)
			{
				this.stars[i].Position = new Vector2(Calc.Random.NextFloat(320f), Calc.Random.NextFloat(180f));
				this.stars[i].Speed = 24f + Calc.Random.NextFloat(24f);
				this.stars[i].Size = 2f + Calc.Random.NextFloat(6f);
			}
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x0008730C File Offset: 0x0008550C
		public override void Update(Scene scene)
		{
			base.Update(scene);
			Vector2 position = (scene as Level).Camera.Position;
			Vector2 value = position - this.lastCamera;
			for (int i = 0; i < this.stars.Length; i++)
			{
				DreamStars.Stars[] array = this.stars;
				int num = i;
				array[num].Position = array[num].Position + (this.angle * this.stars[i].Speed * Engine.DeltaTime - value * 0.5f);
			}
			this.lastCamera = position;
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x000873B0 File Offset: 0x000855B0
		public override void Render(Scene scene)
		{
			for (int i = 0; i < this.stars.Length; i++)
			{
				Draw.HollowRect(new Vector2(this.mod(this.stars[i].Position.X, 320f), this.mod(this.stars[i].Position.Y, 180f)), this.stars[i].Size, this.stars[i].Size, Color.Teal);
			}
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x00026894 File Offset: 0x00024A94
		private float mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04001353 RID: 4947
		private DreamStars.Stars[] stars = new DreamStars.Stars[50];

		// Token: 0x04001354 RID: 4948
		private Vector2 angle = Vector2.Normalize(new Vector2(-2f, -7f));

		// Token: 0x04001355 RID: 4949
		private Vector2 lastCamera = Vector2.Zero;

		// Token: 0x02000688 RID: 1672
		private struct Stars
		{
			// Token: 0x04002B1B RID: 11035
			public Vector2 Position;

			// Token: 0x04002B1C RID: 11036
			public float Speed;

			// Token: 0x04002B1D RID: 11037
			public float Size;
		}
	}
}
