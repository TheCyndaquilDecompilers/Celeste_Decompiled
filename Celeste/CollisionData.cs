using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x02000376 RID: 886
	public struct CollisionData
	{
		// Token: 0x04001941 RID: 6465
		public Vector2 Direction;

		// Token: 0x04001942 RID: 6466
		public Vector2 Moved;

		// Token: 0x04001943 RID: 6467
		public Vector2 TargetPosition;

		// Token: 0x04001944 RID: 6468
		public Platform Hit;

		// Token: 0x04001945 RID: 6469
		public Solid Pusher;

		// Token: 0x04001946 RID: 6470
		public static readonly CollisionData Empty;
	}
}
