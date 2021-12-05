using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000198 RID: 408
	public class RotateSpinner : Entity
	{
		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000E2A RID: 3626 RVA: 0x00032E10 File Offset: 0x00031010
		public float Angle
		{
			get
			{
				return MathHelper.Lerp(4.712389f, -1.5707964f, this.Easer(this.rotationPercent));
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000E2B RID: 3627 RVA: 0x00032E2D File Offset: 0x0003102D
		// (set) Token: 0x06000E2C RID: 3628 RVA: 0x00032E35 File Offset: 0x00031035
		public bool Clockwise { get; private set; }

		// Token: 0x06000E2D RID: 3629 RVA: 0x00032E40 File Offset: 0x00031040
		public RotateSpinner(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Depth = -50;
			this.center = data.Nodes[0] + offset;
			this.Clockwise = data.Bool("clockwise", false);
			base.Collider = new Circle(6f, 0f, 0f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(new StaticMover
			{
				SolidChecker = ((Solid s) => s.CollidePoint(this.center)),
				JumpThruChecker = ((JumpThru jt) => jt.CollidePoint(this.center)),
				OnMove = delegate(Vector2 v)
				{
					this.center += v;
					this.Position += v;
				},
				OnDestroy = delegate()
				{
					this.fallOutOfScreen = true;
				}
			});
			float num = Calc.Angle(this.center, this.Position);
			num = Calc.WrapAngle(num);
			this.rotationPercent = this.EaserInverse(Calc.Percent(num, -1.5707964f, 4.712389f));
			this.length = (this.Position - this.center).Length();
			this.Position = this.center + Calc.AngleToVector(this.Angle, this.length);
		}

		// Token: 0x06000E2E RID: 3630 RVA: 0x00032F97 File Offset: 0x00031197
		private float Easer(float v)
		{
			return v;
		}

		// Token: 0x06000E2F RID: 3631 RVA: 0x00032F97 File Offset: 0x00031197
		private float EaserInverse(float v)
		{
			return v;
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x00032F9C File Offset: 0x0003119C
		public override void Update()
		{
			base.Update();
			if (this.Moving)
			{
				if (this.Clockwise)
				{
					this.rotationPercent -= Engine.DeltaTime / 1.8f;
					this.rotationPercent += 1f;
				}
				else
				{
					this.rotationPercent += Engine.DeltaTime / 1.8f;
				}
				this.rotationPercent %= 1f;
				this.Position = this.center + Calc.AngleToVector(this.Angle, this.length);
			}
			if (this.fallOutOfScreen)
			{
				this.center.Y = this.center.Y + 160f * Engine.DeltaTime;
				if (base.Y > (float)((base.Scene as Level).Bounds.Bottom + 32))
				{
					base.RemoveSelf();
				}
			}
		}

		// Token: 0x06000E31 RID: 3633 RVA: 0x00033086 File Offset: 0x00031286
		public virtual void OnPlayer(Player player)
		{
			if (player.Die((player.Position - this.Position).SafeNormalize(), false, true) != null)
			{
				this.Moving = false;
			}
		}

		// Token: 0x04000964 RID: 2404
		private const float RotationTime = 1.8f;

		// Token: 0x04000965 RID: 2405
		public bool Moving = true;

		// Token: 0x04000967 RID: 2407
		private Vector2 center;

		// Token: 0x04000968 RID: 2408
		private float rotationPercent;

		// Token: 0x04000969 RID: 2409
		private float length;

		// Token: 0x0400096A RID: 2410
		private bool fallOutOfScreen;
	}
}
