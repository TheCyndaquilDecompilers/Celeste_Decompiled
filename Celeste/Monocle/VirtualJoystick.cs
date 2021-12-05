using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x02000112 RID: 274
	public class VirtualJoystick : VirtualInput
	{
		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000878 RID: 2168 RVA: 0x00012BD5 File Offset: 0x00010DD5
		// (set) Token: 0x06000879 RID: 2169 RVA: 0x00012BDD File Offset: 0x00010DDD
		public Vector2 Value { get; private set; }

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x0600087A RID: 2170 RVA: 0x00012BE6 File Offset: 0x00010DE6
		// (set) Token: 0x0600087B RID: 2171 RVA: 0x00012BEE File Offset: 0x00010DEE
		public Vector2 PreviousValue { get; private set; }

		// Token: 0x0600087C RID: 2172 RVA: 0x00012BF7 File Offset: 0x00010DF7
		public VirtualJoystick(Binding up, Binding down, Binding left, Binding right, int gamepadIndex, float threshold, VirtualInput.OverlapBehaviors overlapBehavior = VirtualInput.OverlapBehaviors.TakeNewer)
		{
			this.Up = up;
			this.Down = down;
			this.Left = left;
			this.Right = right;
			this.GamepadIndex = gamepadIndex;
			this.Threshold = threshold;
			this.OverlapBehavior = overlapBehavior;
		}

		// Token: 0x0600087D RID: 2173 RVA: 0x00012C34 File Offset: 0x00010E34
		public VirtualJoystick(Binding up, Binding upAlt, Binding down, Binding downAlt, Binding left, Binding leftAlt, Binding right, Binding rightAlt, int gamepadIndex, float threshold, VirtualInput.OverlapBehaviors overlapBehavior = VirtualInput.OverlapBehaviors.TakeNewer)
		{
			this.Up = up;
			this.Down = down;
			this.Left = left;
			this.Right = right;
			this.UpAlt = upAlt;
			this.DownAlt = downAlt;
			this.LeftAlt = leftAlt;
			this.RightAlt = rightAlt;
			this.GamepadIndex = gamepadIndex;
			this.Threshold = threshold;
			this.OverlapBehavior = overlapBehavior;
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x00012C9C File Offset: 0x00010E9C
		public override void Update()
		{
			this.previousValue = this.value;
			if (!MInput.Disabled)
			{
				Vector2 zero = this.value;
				float num = this.Right.Axis(this.GamepadIndex, 0f);
				float num2 = this.Left.Axis(this.GamepadIndex, 0f);
				float num3 = this.Down.Axis(this.GamepadIndex, 0f);
				float num4 = this.Up.Axis(this.GamepadIndex, 0f);
				if (num == 0f && this.RightAlt != null)
				{
					num = this.RightAlt.Axis(this.GamepadIndex, 0f);
				}
				if (num2 == 0f && this.LeftAlt != null)
				{
					num2 = this.LeftAlt.Axis(this.GamepadIndex, 0f);
				}
				if (num3 == 0f && this.DownAlt != null)
				{
					num3 = this.DownAlt.Axis(this.GamepadIndex, 0f);
				}
				if (num4 == 0f && this.UpAlt != null)
				{
					num4 = this.UpAlt.Axis(this.GamepadIndex, 0f);
				}
				if (num > num2)
				{
					num2 = 0f;
				}
				else if (num2 > num)
				{
					num = 0f;
				}
				if (num3 > num4)
				{
					num4 = 0f;
				}
				else if (num4 > num3)
				{
					num3 = 0f;
				}
				if (num != 0f && num2 != 0f)
				{
					switch (this.OverlapBehavior)
					{
					case VirtualInput.OverlapBehaviors.CancelOut:
						zero.X = 0f;
						break;
					case VirtualInput.OverlapBehaviors.TakeOlder:
						if (zero.X > 0f)
						{
							zero.X = num;
						}
						else if (zero.X < 0f)
						{
							zero.X = num2;
						}
						break;
					case VirtualInput.OverlapBehaviors.TakeNewer:
						if (!this.hTurned)
						{
							if (zero.X > 0f)
							{
								zero.X = -num2;
							}
							else if (zero.X < 0f)
							{
								zero.X = num;
							}
							this.hTurned = true;
						}
						else if (zero.X > 0f)
						{
							zero.X = num;
						}
						else if (zero.X < 0f)
						{
							zero.X = -num2;
						}
						break;
					}
				}
				else if (num != 0f)
				{
					this.hTurned = false;
					zero.X = num;
				}
				else if (num2 != 0f)
				{
					this.hTurned = false;
					zero.X = -num2;
				}
				else
				{
					this.hTurned = false;
					zero.X = 0f;
				}
				if (num3 != 0f && num4 != 0f)
				{
					switch (this.OverlapBehavior)
					{
					case VirtualInput.OverlapBehaviors.CancelOut:
						zero.Y = 0f;
						break;
					case VirtualInput.OverlapBehaviors.TakeOlder:
						if (zero.Y > 0f)
						{
							zero.Y = num3;
						}
						else if (zero.Y < 0f)
						{
							zero.Y = -num4;
						}
						break;
					case VirtualInput.OverlapBehaviors.TakeNewer:
						if (!this.vTurned)
						{
							if (zero.Y > 0f)
							{
								zero.Y = -num4;
							}
							else if (zero.Y < 0f)
							{
								zero.Y = num3;
							}
							this.vTurned = true;
						}
						else if (zero.Y > 0f)
						{
							zero.Y = num3;
						}
						else if (zero.Y < 0f)
						{
							zero.Y = -num4;
						}
						break;
					}
				}
				else if (num3 != 0f)
				{
					this.vTurned = false;
					zero.Y = num3;
				}
				else if (num4 != 0f)
				{
					this.vTurned = false;
					zero.Y = -num4;
				}
				else
				{
					this.vTurned = false;
					zero.Y = 0f;
				}
				if (zero.Length() < this.Threshold)
				{
					zero = Vector2.Zero;
				}
				this.value = zero;
			}
			this.Value = new Vector2(this.InvertedX ? (this.value.X * -1f) : this.value.X, this.InvertedY ? (this.value.Y * -1f) : this.value.Y);
			this.PreviousValue = new Vector2(this.InvertedX ? (this.previousValue.X * -1f) : this.previousValue.X, this.InvertedY ? (this.previousValue.Y * -1f) : this.previousValue.Y);
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x00013120 File Offset: 0x00011320
		public static implicit operator Vector2(VirtualJoystick joystick)
		{
			return joystick.Value;
		}

		// Token: 0x040005A8 RID: 1448
		public Binding Up;

		// Token: 0x040005A9 RID: 1449
		public Binding Down;

		// Token: 0x040005AA RID: 1450
		public Binding Left;

		// Token: 0x040005AB RID: 1451
		public Binding Right;

		// Token: 0x040005AC RID: 1452
		public Binding UpAlt;

		// Token: 0x040005AD RID: 1453
		public Binding DownAlt;

		// Token: 0x040005AE RID: 1454
		public Binding LeftAlt;

		// Token: 0x040005AF RID: 1455
		public Binding RightAlt;

		// Token: 0x040005B0 RID: 1456
		public float Threshold;

		// Token: 0x040005B1 RID: 1457
		public int GamepadIndex;

		// Token: 0x040005B2 RID: 1458
		public VirtualInput.OverlapBehaviors OverlapBehavior;

		// Token: 0x040005B3 RID: 1459
		public bool InvertedX;

		// Token: 0x040005B4 RID: 1460
		public bool InvertedY;

		// Token: 0x040005B7 RID: 1463
		private Vector2 value;

		// Token: 0x040005B8 RID: 1464
		private Vector2 previousValue;

		// Token: 0x040005B9 RID: 1465
		private bool hTurned;

		// Token: 0x040005BA RID: 1466
		private bool vTurned;
	}
}
