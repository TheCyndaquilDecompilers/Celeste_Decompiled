using System;
using Monocle;

namespace Celeste
{
	// Token: 0x020001BF RID: 447
	public class BreathingRumbler : Entity
	{
		// Token: 0x06000F5B RID: 3931 RVA: 0x0003E655 File Offset: 0x0003C855
		public BreathingRumbler()
		{
			this.currentRumble = this.Strength;
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x0003E674 File Offset: 0x0003C874
		public override void Update()
		{
			base.Update();
			this.currentRumble = Calc.Approach(this.currentRumble, this.Strength, 2f * Engine.DeltaTime);
			if (this.currentRumble > 0f)
			{
				Input.RumbleSpecific(this.currentRumble * 0.25f, 0.05f);
			}
		}

		// Token: 0x04000AC0 RID: 2752
		private const float MaxRumble = 0.25f;

		// Token: 0x04000AC1 RID: 2753
		public float Strength = 0.2f;

		// Token: 0x04000AC2 RID: 2754
		private float currentRumble;
	}
}
