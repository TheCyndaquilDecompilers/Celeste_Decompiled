using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000240 RID: 576
	public class StarfieldWipe : ScreenWipe
	{
		// Token: 0x06001249 RID: 4681 RVA: 0x0005F988 File Offset: 0x0005DB88
		public StarfieldWipe(Scene scene, bool wipeIn, Action onComplete = null) : base(scene, wipeIn, onComplete)
		{
			for (int i = 0; i < 5; i++)
			{
				this.starShape[i] = Calc.AngleToVector((float)i / 5f * 6.2831855f, 1f);
			}
			for (int j = 0; j < this.stars.Length; j++)
			{
				this.stars[j] = new StarfieldWipe.Star((float)Math.Pow((double)((float)j / (float)this.stars.Length), 5.0));
			}
			for (int k = 0; k < this.verts.Length; k++)
			{
				this.verts[k].Color = (this.WipeIn ? Color.Black : Color.White);
			}
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x0005FA70 File Offset: 0x0005DC70
		public override void Update(Scene scene)
		{
			base.Update(scene);
			for (int i = 0; i < this.stars.Length; i++)
			{
				this.stars[i].Update();
			}
		}

		// Token: 0x0600124B RID: 4683 RVA: 0x0005FAA8 File Offset: 0x0005DCA8
		public override void BeforeRender(Scene scene)
		{
			this.hasDrawn = true;
			Engine.Graphics.GraphicsDevice.SetRenderTarget(Celeste.WipeTarget);
			Engine.Graphics.GraphicsDevice.Clear(this.WipeIn ? Color.White : Color.Black);
			if (this.Percent > 0.8f)
			{
				float num = Calc.Map(this.Percent, 0.8f, 1f, 0f, 1f) * 1082f;
				Draw.SpriteBatch.Begin();
				Draw.Rect(-1f, (1080f - num) * 0.5f, 1922f, num, (!this.WipeIn) ? Color.White : Color.Black);
				Draw.SpriteBatch.End();
			}
			int num2 = 0;
			for (int i = 0; i < this.stars.Length; i++)
			{
				float xPosition = -500f + this.stars[i].X % 2920f;
				float yPosition = (float)((double)this.stars[i].Y + Math.Sin((double)this.stars[i].Sine) * (double)this.stars[i].SineDistance);
				float scale = (0.1f + this.stars[i].Scale * 0.9f) * 1080f * 0.8f * Ease.CubeIn(this.Percent);
				this.DrawStar(ref num2, Matrix.CreateRotationZ(this.stars[i].Rotation) * Matrix.CreateScale(scale) * Matrix.CreateTranslation(xPosition, yPosition, 0f));
			}
			GFX.DrawVertices<VertexPositionColor>(Matrix.Identity, this.verts, this.verts.Length, null, null);
		}

		// Token: 0x0600124C RID: 4684 RVA: 0x0005FC7C File Offset: 0x0005DE7C
		private void DrawStar(ref int index, Matrix matrix)
		{
			int num = index;
			for (int i = 1; i < this.starShape.Length - 1; i++)
			{
				VertexPositionColor[] array = this.verts;
				int num2 = index;
				index = num2 + 1;
				array[num2].Position = new Vector3(this.starShape[0], 0f);
				VertexPositionColor[] array2 = this.verts;
				num2 = index;
				index = num2 + 1;
				array2[num2].Position = new Vector3(this.starShape[i], 0f);
				VertexPositionColor[] array3 = this.verts;
				num2 = index;
				index = num2 + 1;
				array3[num2].Position = new Vector3(this.starShape[i + 1], 0f);
			}
			for (int j = 0; j < 5; j++)
			{
				Vector2 vector = this.starShape[j];
				Vector2 vector2 = this.starShape[(j + 1) % 5];
				Vector2 value = (vector + vector2) * 0.5f + (vector - vector2).SafeNormalize().TurnRight();
				VertexPositionColor[] array4 = this.verts;
				int num2 = index;
				index = num2 + 1;
				array4[num2].Position = new Vector3(vector, 0f);
				VertexPositionColor[] array5 = this.verts;
				num2 = index;
				index = num2 + 1;
				array5[num2].Position = new Vector3(value, 0f);
				VertexPositionColor[] array6 = this.verts;
				num2 = index;
				index = num2 + 1;
				array6[num2].Position = new Vector3(vector2, 0f);
			}
			for (int k = num; k < num + 24; k++)
			{
				this.verts[k].Position = Vector3.Transform(this.verts[k].Position, matrix);
			}
		}

		// Token: 0x0600124D RID: 4685 RVA: 0x0005FE44 File Offset: 0x0005E044
		public override void Render(Scene scene)
		{
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, StarfieldWipe.SubtractBlendmode, SamplerState.LinearClamp, null, null, null, Engine.ScreenMatrix);
			if ((this.WipeIn && this.Percent <= 0.01f) || (!this.WipeIn && this.Percent >= 0.99f))
			{
				Draw.Rect(-1f, -1f, 1922f, 1082f, Color.White);
			}
			else if (this.hasDrawn)
			{
				Draw.SpriteBatch.Draw(Celeste.WipeTarget, new Vector2(-1f, -1f), Color.White);
			}
			Draw.SpriteBatch.End();
		}

		// Token: 0x04000DF8 RID: 3576
		public static readonly BlendState SubtractBlendmode = new BlendState
		{
			ColorSourceBlend = Blend.One,
			ColorDestinationBlend = Blend.One,
			ColorBlendFunction = BlendFunction.ReverseSubtract,
			AlphaSourceBlend = Blend.One,
			AlphaDestinationBlend = Blend.One,
			AlphaBlendFunction = BlendFunction.Add
		};

		// Token: 0x04000DF9 RID: 3577
		private StarfieldWipe.Star[] stars = new StarfieldWipe.Star[64];

		// Token: 0x04000DFA RID: 3578
		private VertexPositionColor[] verts = new VertexPositionColor[1536];

		// Token: 0x04000DFB RID: 3579
		private Vector2[] starShape = new Vector2[5];

		// Token: 0x04000DFC RID: 3580
		private bool hasDrawn;

		// Token: 0x02000566 RID: 1382
		private struct Star
		{
			// Token: 0x06002671 RID: 9841 RVA: 0x000FCFF0 File Offset: 0x000FB1F0
			public Star(float scale)
			{
				this.Scale = scale;
				float num = 1f - scale;
				this.X = (float)Calc.Random.Range(0, 2920);
				this.Y = 1080f * (0.5f + (float)Calc.Random.Choose(-1, 1) * num * Calc.Random.Range(0.25f, 0.5f));
				this.Sine = Calc.Random.NextFloat(6.2831855f);
				this.SineDistance = scale * 1080f * 0.05f;
				this.Speed = (0.5f + (1f - this.Scale) * 0.5f) * 1920f * 0.05f;
				this.Rotation = Calc.Random.NextFloat(6.2831855f);
			}

			// Token: 0x06002672 RID: 9842 RVA: 0x000FD0C0 File Offset: 0x000FB2C0
			public void Update()
			{
				this.X += this.Speed * Engine.DeltaTime;
				this.Sine += (1f - this.Scale) * 8f * Engine.DeltaTime;
				this.Rotation += (1f - this.Scale) * Engine.DeltaTime;
			}

			// Token: 0x04002653 RID: 9811
			public float X;

			// Token: 0x04002654 RID: 9812
			public float Y;

			// Token: 0x04002655 RID: 9813
			public float Sine;

			// Token: 0x04002656 RID: 9814
			public float SineDistance;

			// Token: 0x04002657 RID: 9815
			public float Speed;

			// Token: 0x04002658 RID: 9816
			public float Scale;

			// Token: 0x04002659 RID: 9817
			public float Rotation;
		}
	}
}
