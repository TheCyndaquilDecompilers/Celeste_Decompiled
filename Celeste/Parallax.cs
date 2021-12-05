using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200036B RID: 875
	public class Parallax : Backdrop
	{
		// Token: 0x06001B7E RID: 7038 RVA: 0x000B3CB8 File Offset: 0x000B1EB8
		public Parallax(MTexture texture)
		{
			this.Name = texture.AtlasPath;
			this.Texture = texture;
		}

		// Token: 0x06001B7F RID: 7039 RVA: 0x000B3D0C File Offset: 0x000B1F0C
		public override void Update(Scene scene)
		{
			base.Update(scene);
			this.Position += this.Speed * Engine.DeltaTime;
			this.Position += this.WindMultiplier * (scene as Level).Wind * Engine.DeltaTime;
			if (this.DoFadeIn)
			{
				this.fadeIn = Calc.Approach(this.fadeIn, (float)(this.Visible ? 1 : 0), Engine.DeltaTime);
				return;
			}
			this.fadeIn = (float)(this.Visible ? 1 : 0);
		}

		// Token: 0x06001B80 RID: 7040 RVA: 0x000B3DB4 File Offset: 0x000B1FB4
		public override void Render(Scene scene)
		{
			Vector2 vector = ((scene as Level).Camera.Position + this.CameraOffset).Floor();
			Vector2 vector2 = (this.Position - vector * this.Scroll).Floor();
			float num = this.fadeIn * this.Alpha * this.FadeAlphaMultiplier;
			if (this.FadeX != null)
			{
				num *= this.FadeX.Value(vector.X + 160f);
			}
			if (this.FadeY != null)
			{
				num *= this.FadeY.Value(vector.Y + 90f);
			}
			Color color = this.Color;
			if (num < 1f)
			{
				color *= num;
			}
			if (color.A > 1)
			{
				if (this.LoopX)
				{
					while (vector2.X < 0f)
					{
						vector2.X += (float)this.Texture.Width;
					}
					while (vector2.X > 0f)
					{
						vector2.X -= (float)this.Texture.Width;
					}
				}
				if (this.LoopY)
				{
					while (vector2.Y < 0f)
					{
						vector2.Y += (float)this.Texture.Height;
					}
					while (vector2.Y > 0f)
					{
						vector2.Y -= (float)this.Texture.Height;
					}
				}
				SpriteEffects flip = SpriteEffects.None;
				if (this.FlipX && this.FlipY)
				{
					flip = (SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically);
				}
				else if (this.FlipX)
				{
					flip = SpriteEffects.FlipHorizontally;
				}
				else if (this.FlipY)
				{
					flip = SpriteEffects.FlipVertically;
				}
				for (float num2 = vector2.X; num2 < 320f; num2 += (float)this.Texture.Width)
				{
					for (float num3 = vector2.Y; num3 < 180f; num3 += (float)this.Texture.Height)
					{
						this.Texture.Draw(new Vector2(num2, num3), Vector2.Zero, color, 1f, 0f, flip);
						if (!this.LoopY)
						{
							break;
						}
					}
					if (!this.LoopX)
					{
						break;
					}
				}
			}
		}

		// Token: 0x0400188E RID: 6286
		public Vector2 CameraOffset = Vector2.Zero;

		// Token: 0x0400188F RID: 6287
		public BlendState BlendState = BlendState.AlphaBlend;

		// Token: 0x04001890 RID: 6288
		public MTexture Texture;

		// Token: 0x04001891 RID: 6289
		public bool DoFadeIn;

		// Token: 0x04001892 RID: 6290
		public float Alpha = 1f;

		// Token: 0x04001893 RID: 6291
		private float fadeIn = 1f;
	}
}
