using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000326 RID: 806
	public class RotatingPlatform : JumpThru
	{
		// Token: 0x06001963 RID: 6499 RVA: 0x000A3080 File Offset: 0x000A1280
		public RotatingPlatform(Vector2 position, int width, Vector2 center, bool clockwise) : base(position, width, false)
		{
			base.Collider.Position.X = (float)(-(float)width / 2);
			base.Collider.Position.Y = -base.Height / 2f;
			this.center = center;
			this.clockwise = clockwise;
			this.length = (position - center).Length();
			this.currentAngle = (position - center).Angle();
			this.SurfaceSoundIndex = 5;
			base.Add(new LightOcclude(0.2f));
		}

		// Token: 0x06001964 RID: 6500 RVA: 0x000A3118 File Offset: 0x000A1318
		public override void Update()
		{
			base.Update();
			if (this.clockwise)
			{
				this.currentAngle -= 1.0471976f * Engine.DeltaTime;
			}
			else
			{
				this.currentAngle += 1.0471976f * Engine.DeltaTime;
			}
			this.currentAngle = Calc.WrapAngle(this.currentAngle);
			base.MoveTo(this.center + Calc.AngleToVector(this.currentAngle, this.length));
		}

		// Token: 0x06001965 RID: 6501 RVA: 0x000A3198 File Offset: 0x000A1398
		public override void Render()
		{
			base.Render();
			Draw.Rect(base.Collider, Color.White);
		}

		// Token: 0x0400161C RID: 5660
		private const float RotateSpeed = 1.0471976f;

		// Token: 0x0400161D RID: 5661
		private Vector2 center;

		// Token: 0x0400161E RID: 5662
		private bool clockwise;

		// Token: 0x0400161F RID: 5663
		private float length;

		// Token: 0x04001620 RID: 5664
		private float currentAngle;
	}
}
