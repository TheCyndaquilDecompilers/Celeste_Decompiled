using System;

namespace Monocle
{
	// Token: 0x02000111 RID: 273
	public class VirtualIntegerAxis : VirtualInput
	{
		// Token: 0x06000873 RID: 2163 RVA: 0x000127D7 File Offset: 0x000109D7
		public VirtualIntegerAxis()
		{
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x00012A08 File Offset: 0x00010C08
		public VirtualIntegerAxis(Binding negative, Binding positive, int gamepadIndex, float threshold, VirtualInput.OverlapBehaviors overlapBehavior = VirtualInput.OverlapBehaviors.TakeNewer)
		{
			this.Positive = positive;
			this.Negative = negative;
			this.Threshold = threshold;
			this.GamepadIndex = gamepadIndex;
			this.OverlapBehavior = overlapBehavior;
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x00012A35 File Offset: 0x00010C35
		public VirtualIntegerAxis(Binding negative, Binding negativeAlt, Binding positive, Binding positiveAlt, int gamepadIndex, float threshold, VirtualInput.OverlapBehaviors overlapBehavior = VirtualInput.OverlapBehaviors.TakeNewer)
		{
			this.Positive = positive;
			this.Negative = negative;
			this.PositiveAlt = positiveAlt;
			this.NegativeAlt = negativeAlt;
			this.Threshold = threshold;
			this.GamepadIndex = gamepadIndex;
			this.OverlapBehavior = overlapBehavior;
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x00012A74 File Offset: 0x00010C74
		public override void Update()
		{
			this.PreviousValue = this.Value;
			if (!MInput.Disabled)
			{
				bool flag = this.Positive.Axis(this.GamepadIndex, this.Threshold) > 0f || (this.PositiveAlt != null && this.PositiveAlt.Axis(this.GamepadIndex, this.Threshold) > 0f);
				bool flag2 = this.Negative.Axis(this.GamepadIndex, this.Threshold) > 0f || (this.NegativeAlt != null && this.NegativeAlt.Axis(this.GamepadIndex, this.Threshold) > 0f);
				if (flag && flag2)
				{
					switch (this.OverlapBehavior)
					{
					case VirtualInput.OverlapBehaviors.CancelOut:
						this.Value = 0;
						break;
					case VirtualInput.OverlapBehaviors.TakeOlder:
						this.Value = this.PreviousValue;
						break;
					case VirtualInput.OverlapBehaviors.TakeNewer:
						if (!this.turned)
						{
							this.Value *= -1;
							this.turned = true;
						}
						break;
					}
				}
				else if (flag)
				{
					this.turned = false;
					this.Value = 1;
				}
				else if (flag2)
				{
					this.turned = false;
					this.Value = -1;
				}
				else
				{
					this.turned = false;
					this.Value = 0;
				}
				if (this.Inverted)
				{
					this.Value = -this.Value;
				}
			}
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x00012BCC File Offset: 0x00010DCC
		public static implicit operator float(VirtualIntegerAxis axis)
		{
			return (float)axis.Value;
		}

		// Token: 0x0400059D RID: 1437
		public Binding Positive;

		// Token: 0x0400059E RID: 1438
		public Binding Negative;

		// Token: 0x0400059F RID: 1439
		public Binding PositiveAlt;

		// Token: 0x040005A0 RID: 1440
		public Binding NegativeAlt;

		// Token: 0x040005A1 RID: 1441
		public float Threshold;

		// Token: 0x040005A2 RID: 1442
		public int GamepadIndex;

		// Token: 0x040005A3 RID: 1443
		public VirtualInput.OverlapBehaviors OverlapBehavior;

		// Token: 0x040005A4 RID: 1444
		public bool Inverted;

		// Token: 0x040005A5 RID: 1445
		public int Value;

		// Token: 0x040005A6 RID: 1446
		public int PreviousValue;

		// Token: 0x040005A7 RID: 1447
		private bool turned;
	}
}
