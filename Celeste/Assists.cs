using System;
using Monocle;

namespace Celeste
{
	// Token: 0x02000321 RID: 801
	[Serializable]
	public struct Assists
	{
		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06001948 RID: 6472 RVA: 0x000A23A4 File Offset: 0x000A05A4
		public static Assists Default
		{
			get
			{
				return new Assists
				{
					GameSpeed = 10
				};
			}
		}

		// Token: 0x06001949 RID: 6473 RVA: 0x000A23C4 File Offset: 0x000A05C4
		public void EnfornceAssistMode()
		{
			this.GameSpeed = Calc.Clamp(this.GameSpeed, 5, 10);
			this.MirrorMode = false;
			this.ThreeSixtyDashing = false;
			this.InvisibleMotion = false;
			this.NoGrabbing = false;
			this.LowFriction = false;
			this.SuperDashing = false;
			this.Hiccups = false;
			this.PlayAsBadeline = false;
		}

		// Token: 0x040015FC RID: 5628
		public int GameSpeed;

		// Token: 0x040015FD RID: 5629
		public bool Invincible;

		// Token: 0x040015FE RID: 5630
		public Assists.DashModes DashMode;

		// Token: 0x040015FF RID: 5631
		public bool DashAssist;

		// Token: 0x04001600 RID: 5632
		public bool InfiniteStamina;

		// Token: 0x04001601 RID: 5633
		public bool MirrorMode;

		// Token: 0x04001602 RID: 5634
		public bool ThreeSixtyDashing;

		// Token: 0x04001603 RID: 5635
		public bool InvisibleMotion;

		// Token: 0x04001604 RID: 5636
		public bool NoGrabbing;

		// Token: 0x04001605 RID: 5637
		public bool LowFriction;

		// Token: 0x04001606 RID: 5638
		public bool SuperDashing;

		// Token: 0x04001607 RID: 5639
		public bool Hiccups;

		// Token: 0x04001608 RID: 5640
		public bool PlayAsBadeline;

		// Token: 0x020006DD RID: 1757
		public enum DashModes
		{
			// Token: 0x04002C8B RID: 11403
			Normal,
			// Token: 0x04002C8C RID: 11404
			Two,
			// Token: 0x04002C8D RID: 11405
			Infinite
		}
	}
}
