using System;
using Microsoft.Xna.Framework.Input;

namespace Monocle
{
	// Token: 0x0200010E RID: 270
	public class VirtualButton : VirtualInput
	{
		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000861 RID: 2145 RVA: 0x000127A1 File Offset: 0x000109A1
		// (set) Token: 0x06000862 RID: 2146 RVA: 0x000127A9 File Offset: 0x000109A9
		public bool Repeating { get; private set; }

		// Token: 0x06000863 RID: 2147 RVA: 0x000127B2 File Offset: 0x000109B2
		public VirtualButton(Binding binding, int gamepadIndex, float bufferTime, float triggerThreshold)
		{
			this.Binding = binding;
			this.GamepadIndex = gamepadIndex;
			this.BufferTime = bufferTime;
			this.Threshold = triggerThreshold;
		}

		// Token: 0x06000864 RID: 2148 RVA: 0x000127D7 File Offset: 0x000109D7
		public VirtualButton()
		{
		}

		// Token: 0x06000865 RID: 2149 RVA: 0x000127DF File Offset: 0x000109DF
		public void SetRepeat(float repeatTime)
		{
			this.SetRepeat(repeatTime, repeatTime);
		}

		// Token: 0x06000866 RID: 2150 RVA: 0x000127E9 File Offset: 0x000109E9
		public void SetRepeat(float firstRepeatTime, float multiRepeatTime)
		{
			this.firstRepeatTime = firstRepeatTime;
			this.multiRepeatTime = multiRepeatTime;
			this.canRepeat = (this.firstRepeatTime > 0f);
			if (!this.canRepeat)
			{
				this.Repeating = false;
			}
		}

		// Token: 0x06000867 RID: 2151 RVA: 0x0001281C File Offset: 0x00010A1C
		public override void Update()
		{
			this.consumed = false;
			this.bufferCounter -= Engine.DeltaTime;
			bool flag = false;
			if (this.Binding.Pressed(this.GamepadIndex, this.Threshold))
			{
				this.bufferCounter = this.BufferTime;
				flag = true;
			}
			else if (this.Binding.Check(this.GamepadIndex, this.Threshold))
			{
				flag = true;
			}
			if (!flag)
			{
				this.Repeating = false;
				this.repeatCounter = 0f;
				this.bufferCounter = 0f;
				return;
			}
			if (this.canRepeat)
			{
				this.Repeating = false;
				if (this.repeatCounter == 0f)
				{
					this.repeatCounter = this.firstRepeatTime;
					return;
				}
				this.repeatCounter -= Engine.DeltaTime;
				if (this.repeatCounter <= 0f)
				{
					this.Repeating = true;
					this.repeatCounter = this.multiRepeatTime;
				}
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000868 RID: 2152 RVA: 0x00012904 File Offset: 0x00010B04
		public bool Check
		{
			get
			{
				return !MInput.Disabled && this.Binding.Check(this.GamepadIndex, this.Threshold);
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000869 RID: 2153 RVA: 0x00012928 File Offset: 0x00010B28
		public bool Pressed
		{
			get
			{
				return (this.DebugOverridePressed != null && MInput.Keyboard.Check(this.DebugOverridePressed.Value)) || (!MInput.Disabled && !this.consumed && (this.bufferCounter > 0f || this.Repeating || this.Binding.Pressed(this.GamepadIndex, this.Threshold)));
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x0600086A RID: 2154 RVA: 0x0001299C File Offset: 0x00010B9C
		public bool Released
		{
			get
			{
				return !MInput.Disabled && this.Binding.Released(this.GamepadIndex, this.Threshold);
			}
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x000129BE File Offset: 0x00010BBE
		public void ConsumeBuffer()
		{
			this.bufferCounter = 0f;
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x000129CB File Offset: 0x00010BCB
		public void ConsumePress()
		{
			this.bufferCounter = 0f;
			this.consumed = true;
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x000129DF File Offset: 0x00010BDF
		public static implicit operator bool(VirtualButton button)
		{
			return button.Check;
		}

		// Token: 0x04000591 RID: 1425
		public Binding Binding;

		// Token: 0x04000592 RID: 1426
		public float Threshold;

		// Token: 0x04000593 RID: 1427
		public float BufferTime;

		// Token: 0x04000594 RID: 1428
		public int GamepadIndex;

		// Token: 0x04000596 RID: 1430
		public Keys? DebugOverridePressed;

		// Token: 0x04000597 RID: 1431
		private float firstRepeatTime;

		// Token: 0x04000598 RID: 1432
		private float multiRepeatTime;

		// Token: 0x04000599 RID: 1433
		private float bufferCounter;

		// Token: 0x0400059A RID: 1434
		private float repeatCounter;

		// Token: 0x0400059B RID: 1435
		private bool canRepeat;

		// Token: 0x0400059C RID: 1436
		private bool consumed;
	}
}
