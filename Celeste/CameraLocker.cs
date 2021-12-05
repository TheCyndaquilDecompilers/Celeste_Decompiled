using System;
using Monocle;

namespace Celeste
{
	// Token: 0x0200018C RID: 396
	[Tracked(false)]
	public class CameraLocker : Component
	{
		// Token: 0x06000DDB RID: 3547 RVA: 0x00031959 File Offset: 0x0002FB59
		public CameraLocker(Level.CameraLockModes lockMode, float maxXOffset, float maxYOffset) : base(lockMode == Level.CameraLockModes.BoostSequence, false)
		{
			this.LockMode = lockMode;
			this.MaxXOffset = maxXOffset;
			this.MaxYOffset = maxYOffset;
		}

		// Token: 0x06000DDC RID: 3548 RVA: 0x0003197B File Offset: 0x0002FB7B
		public override void EntityAdded(Scene scene)
		{
			base.EntityAdded(scene);
			base.SceneAs<Level>().CameraLockMode = this.LockMode;
		}

		// Token: 0x04000927 RID: 2343
		public const float UpwardMaxYOffset = 180f;

		// Token: 0x04000928 RID: 2344
		public Level.CameraLockModes LockMode;

		// Token: 0x04000929 RID: 2345
		public float MaxXOffset;

		// Token: 0x0400092A RID: 2346
		public float MaxYOffset;
	}
}
