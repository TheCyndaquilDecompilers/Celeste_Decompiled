using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000250 RID: 592
	public struct MountainCamera
	{
		// Token: 0x06001292 RID: 4754 RVA: 0x000628CC File Offset: 0x00060ACC
		public MountainCamera(Vector3 pos, Vector3 target)
		{
			this.Position = pos;
			this.Target = target;
			this.Rotation = default(Quaternion).LookAt(this.Position, this.Target, Vector3.Up);
		}

		// Token: 0x06001293 RID: 4755 RVA: 0x0006290C File Offset: 0x00060B0C
		public void LookAt(Vector3 pos)
		{
			this.Target = pos;
			this.Rotation = default(Quaternion).LookAt(this.Position, this.Target, Vector3.Up);
		}

		// Token: 0x04000E6E RID: 3694
		public Vector3 Position;

		// Token: 0x04000E6F RID: 3695
		public Vector3 Target;

		// Token: 0x04000E70 RID: 3696
		public Quaternion Rotation;
	}
}
