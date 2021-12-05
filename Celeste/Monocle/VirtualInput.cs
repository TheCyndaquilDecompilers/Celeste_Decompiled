using System;

namespace Monocle
{
	// Token: 0x0200010F RID: 271
	public abstract class VirtualInput
	{
		// Token: 0x0600086E RID: 2158 RVA: 0x000129E7 File Offset: 0x00010BE7
		public VirtualInput()
		{
			MInput.VirtualInputs.Add(this);
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x000129FA File Offset: 0x00010BFA
		public void Deregister()
		{
			MInput.VirtualInputs.Remove(this);
		}

		// Token: 0x06000870 RID: 2160
		public abstract void Update();

		// Token: 0x020003AA RID: 938
		public enum OverlapBehaviors
		{
			// Token: 0x04001F28 RID: 7976
			CancelOut,
			// Token: 0x04001F29 RID: 7977
			TakeOlder,
			// Token: 0x04001F2A RID: 7978
			TakeNewer
		}

		// Token: 0x020003AB RID: 939
		public enum ThresholdModes
		{
			// Token: 0x04001F2C RID: 7980
			LargerThan,
			// Token: 0x04001F2D RID: 7981
			LessThan,
			// Token: 0x04001F2E RID: 7982
			EqualTo
		}
	}
}
