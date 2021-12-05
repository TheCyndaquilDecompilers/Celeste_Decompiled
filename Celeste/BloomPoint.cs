using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000336 RID: 822
	[Tracked(false)]
	public class BloomPoint : Component
	{
		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x060019BD RID: 6589 RVA: 0x000A5EBF File Offset: 0x000A40BF
		// (set) Token: 0x060019BE RID: 6590 RVA: 0x000A5ECC File Offset: 0x000A40CC
		public float X
		{
			get
			{
				return this.Position.X;
			}
			set
			{
				this.Position.X = value;
			}
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x060019BF RID: 6591 RVA: 0x000A5EDA File Offset: 0x000A40DA
		// (set) Token: 0x060019C0 RID: 6592 RVA: 0x000A5EE7 File Offset: 0x000A40E7
		public float Y
		{
			get
			{
				return this.Position.Y;
			}
			set
			{
				this.Position.Y = value;
			}
		}

		// Token: 0x060019C1 RID: 6593 RVA: 0x000A5EF5 File Offset: 0x000A40F5
		public BloomPoint(float alpha, float radius) : base(false, true)
		{
			this.Alpha = alpha;
			this.Radius = radius;
		}

		// Token: 0x060019C2 RID: 6594 RVA: 0x000A5F2E File Offset: 0x000A412E
		public BloomPoint(Vector2 position, float alpha, float radius) : base(false, true)
		{
			this.Position = position;
			this.Alpha = alpha;
			this.Radius = radius;
		}

		// Token: 0x0400167D RID: 5757
		public Vector2 Position = Vector2.Zero;

		// Token: 0x0400167E RID: 5758
		public float Alpha = 1f;

		// Token: 0x0400167F RID: 5759
		public float Radius = 8f;
	}
}
