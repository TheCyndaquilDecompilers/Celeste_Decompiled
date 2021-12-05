using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001B0 RID: 432
	public class AbsorbOrb : Entity
	{
		// Token: 0x06000F18 RID: 3864 RVA: 0x0003BDA8 File Offset: 0x00039FA8
		public AbsorbOrb(Vector2 position, Entity into = null, Vector2? absorbTarget = null)
		{
			this.AbsorbInto = into;
			this.AbsorbTarget = absorbTarget;
			this.Position = position;
			base.Tag = Tags.FrozenUpdate;
			base.Depth = -2000000;
			this.consumeDelay = 0.7f + Calc.Random.NextFloat() * 0.3f;
			this.burstSpeed = 80f + Calc.Random.NextFloat() * 40f;
			this.burstDirection = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
			base.Add(this.sprite = new Image(GFX.Game["collectables/heartGem/orb"]));
			this.sprite.CenterOrigin();
			base.Add(this.bloom = new BloomPoint(1f, 16f));
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x0003BE9C File Offset: 0x0003A09C
		public override void Update()
		{
			base.Update();
			Vector2 vector = Vector2.Zero;
			bool flag = false;
			if (this.AbsorbInto != null)
			{
				vector = this.AbsorbInto.Center;
				flag = (this.AbsorbInto.Scene == null || (this.AbsorbInto is Player && (this.AbsorbInto as Player).Dead));
			}
			else if (this.AbsorbTarget != null)
			{
				vector = this.AbsorbTarget.Value;
			}
			else
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					vector = entity.Center;
				}
				flag = (entity == null || entity.Scene == null || entity.Dead);
			}
			if (flag)
			{
				this.Position += this.burstDirection * this.burstSpeed * Engine.RawDeltaTime;
				this.burstSpeed = Calc.Approach(this.burstSpeed, 800f, Engine.RawDeltaTime * 200f);
				this.sprite.Rotation = this.burstDirection.Angle();
				this.sprite.Scale = new Vector2(Math.Min(2f, 0.5f + this.burstSpeed * 0.02f), Math.Max(0.05f, 0.5f - this.burstSpeed * 0.004f));
				this.sprite.Color = Color.White * (this.alpha = Calc.Approach(this.alpha, 0f, Engine.DeltaTime));
				return;
			}
			if (this.consumeDelay > 0f)
			{
				this.Position += this.burstDirection * this.burstSpeed * Engine.RawDeltaTime;
				this.burstSpeed = Calc.Approach(this.burstSpeed, 0f, Engine.RawDeltaTime * 120f);
				this.sprite.Rotation = this.burstDirection.Angle();
				this.sprite.Scale = new Vector2(Math.Min(2f, 0.5f + this.burstSpeed * 0.02f), Math.Max(0.05f, 0.5f - this.burstSpeed * 0.004f));
				this.consumeDelay -= Engine.RawDeltaTime;
				if (this.consumeDelay <= 0f)
				{
					Vector2 position = this.Position;
					Vector2 vector2 = vector;
					Vector2 value = (position + vector2) / 2f;
					Vector2 vector3 = (vector2 - position).SafeNormalize().Perpendicular() * (position - vector2).Length() * (0.05f + Calc.Random.NextFloat() * 0.45f);
					float value2 = vector2.X - position.X;
					float value3 = vector2.Y - position.Y;
					if ((Math.Abs(value2) > Math.Abs(value3) && Math.Sign(vector3.X) != Math.Sign(value2)) || (Math.Abs(value3) > Math.Abs(value3) && Math.Sign(vector3.Y) != Math.Sign(value3)))
					{
						vector3 *= -1f;
					}
					this.curve = new SimpleCurve(position, vector2, value + vector3);
					this.duration = 0.3f + Calc.Random.NextFloat(0.25f);
					this.burstScale = this.sprite.Scale;
					return;
				}
			}
			else
			{
				this.curve.End = vector;
				if (this.percent >= 1f)
				{
					base.RemoveSelf();
				}
				this.percent = Calc.Approach(this.percent, 1f, Engine.RawDeltaTime / this.duration);
				float num = Ease.CubeIn(this.percent);
				this.Position = this.curve.GetPoint(num);
				float num2 = Calc.YoYo(num) * this.curve.GetLengthParametric(10);
				this.sprite.Scale = new Vector2(Math.Min(2f, 0.5f + num2 * 0.02f), Math.Max(0.05f, 0.5f - num2 * 0.004f));
				this.sprite.Color = Color.White * (1f - num);
				this.sprite.Rotation = Calc.Angle(this.Position, this.curve.GetPoint(Ease.CubeIn(this.percent + 0.01f)));
			}
		}

		// Token: 0x04000A6E RID: 2670
		public Entity AbsorbInto;

		// Token: 0x04000A6F RID: 2671
		public Vector2? AbsorbTarget;

		// Token: 0x04000A70 RID: 2672
		private SimpleCurve curve;

		// Token: 0x04000A71 RID: 2673
		private float duration;

		// Token: 0x04000A72 RID: 2674
		private float percent;

		// Token: 0x04000A73 RID: 2675
		private float consumeDelay;

		// Token: 0x04000A74 RID: 2676
		private float burstSpeed;

		// Token: 0x04000A75 RID: 2677
		private Vector2 burstDirection;

		// Token: 0x04000A76 RID: 2678
		private Vector2 burstScale;

		// Token: 0x04000A77 RID: 2679
		private float alpha = 1f;

		// Token: 0x04000A78 RID: 2680
		private Image sprite;

		// Token: 0x04000A79 RID: 2681
		private BloomPoint bloom;
	}
}
