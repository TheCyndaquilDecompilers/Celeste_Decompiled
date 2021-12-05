using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x02000117 RID: 279
	public struct Particle
	{
		// Token: 0x060008C5 RID: 2245 RVA: 0x00014508 File Offset: 0x00012708
		public bool SimulateFor(float duration)
		{
			if (duration > this.Life)
			{
				this.Life = 0f;
				this.Active = false;
				return false;
			}
			float num = Engine.TimeRate * ((float)Engine.Instance.TargetElapsedTime.Milliseconds / 1000f);
			if (num > 0f)
			{
				for (float num2 = 0f; num2 < duration; num2 += num)
				{
					this.Update(new float?(num));
				}
			}
			return true;
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x00014578 File Offset: 0x00012778
		public void Update(float? delta = null)
		{
			float num;
			if (delta != null)
			{
				num = delta.Value;
			}
			else
			{
				num = (this.Type.UseActualDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime);
			}
			float num2 = this.Life / this.StartLife;
			this.Life -= num;
			if (this.Life <= 0f)
			{
				this.Active = false;
				return;
			}
			if (this.Type.RotationMode == ParticleType.RotationModes.SameAsDirection)
			{
				if (this.Speed != Vector2.Zero)
				{
					this.Rotation = this.Speed.Angle();
				}
			}
			else
			{
				this.Rotation += this.Spin * num;
			}
			float num3;
			if (this.Type.FadeMode == ParticleType.FadeModes.Linear)
			{
				num3 = num2;
			}
			else if (this.Type.FadeMode == ParticleType.FadeModes.Late)
			{
				num3 = Math.Min(1f, num2 / 0.25f);
			}
			else if (this.Type.FadeMode == ParticleType.FadeModes.InAndOut)
			{
				if (num2 > 0.75f)
				{
					num3 = 1f - (num2 - 0.75f) / 0.25f;
				}
				else if (num2 < 0.25f)
				{
					num3 = num2 / 0.25f;
				}
				else
				{
					num3 = 1f;
				}
			}
			else
			{
				num3 = 1f;
			}
			if (num3 == 0f)
			{
				this.Color = Color.Transparent;
			}
			else
			{
				if (this.Type.ColorMode == ParticleType.ColorModes.Static)
				{
					this.Color = this.StartColor;
				}
				else if (this.Type.ColorMode == ParticleType.ColorModes.Fade)
				{
					this.Color = Color.Lerp(this.Type.Color2, this.StartColor, num2);
				}
				else if (this.Type.ColorMode == ParticleType.ColorModes.Blink)
				{
					this.Color = (Calc.BetweenInterval(this.Life, 0.1f) ? this.StartColor : this.Type.Color2);
				}
				else if (this.Type.ColorMode == ParticleType.ColorModes.Choose)
				{
					this.Color = this.StartColor;
				}
				if (num3 < 1f)
				{
					this.Color *= num3;
				}
			}
			this.Position += this.Speed * num;
			this.Speed += this.Type.Acceleration * num;
			this.Speed = Calc.Approach(this.Speed, Vector2.Zero, this.Type.Friction * num);
			if (this.Type.SpeedMultiplier != 1f)
			{
				this.Speed *= (float)Math.Pow((double)this.Type.SpeedMultiplier, (double)num);
			}
			if (this.Type.ScaleOut)
			{
				this.Size = this.StartSize * Ease.CubeOut(num2);
			}
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x00014844 File Offset: 0x00012A44
		public void Render()
		{
			Vector2 vector = new Vector2((float)((int)this.Position.X), (float)((int)this.Position.Y));
			if (this.Track != null)
			{
				vector += this.Track.Position;
			}
			Draw.SpriteBatch.Draw(this.Source.Texture.Texture, vector, new Rectangle?(this.Source.ClipRect), this.Color, this.Rotation, this.Source.Center, this.Size, SpriteEffects.None, 0f);
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x000148DC File Offset: 0x00012ADC
		public void Render(float alpha)
		{
			Vector2 vector = new Vector2((float)((int)this.Position.X), (float)((int)this.Position.Y));
			if (this.Track != null)
			{
				vector += this.Track.Position;
			}
			Draw.SpriteBatch.Draw(this.Source.Texture.Texture, vector, new Rectangle?(this.Source.ClipRect), this.Color * alpha, this.Rotation, this.Source.Center, this.Size, SpriteEffects.None, 0f);
		}

		// Token: 0x040005D4 RID: 1492
		public Entity Track;

		// Token: 0x040005D5 RID: 1493
		public ParticleType Type;

		// Token: 0x040005D6 RID: 1494
		public MTexture Source;

		// Token: 0x040005D7 RID: 1495
		public bool Active;

		// Token: 0x040005D8 RID: 1496
		public Color Color;

		// Token: 0x040005D9 RID: 1497
		public Color StartColor;

		// Token: 0x040005DA RID: 1498
		public Vector2 Position;

		// Token: 0x040005DB RID: 1499
		public Vector2 Speed;

		// Token: 0x040005DC RID: 1500
		public float Size;

		// Token: 0x040005DD RID: 1501
		public float StartSize;

		// Token: 0x040005DE RID: 1502
		public float Life;

		// Token: 0x040005DF RID: 1503
		public float StartLife;

		// Token: 0x040005E0 RID: 1504
		public float ColorSwitch;

		// Token: 0x040005E1 RID: 1505
		public float Rotation;

		// Token: 0x040005E2 RID: 1506
		public float Spin;
	}
}
