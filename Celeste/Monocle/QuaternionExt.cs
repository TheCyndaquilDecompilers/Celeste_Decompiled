using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x02000122 RID: 290
	public static class QuaternionExt
	{
		// Token: 0x06000A47 RID: 2631 RVA: 0x00019844 File Offset: 0x00017A44
		public static Quaternion Conjugated(this Quaternion q)
		{
			Quaternion result = q;
			result.Conjugate();
			return result;
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x0001985B File Offset: 0x00017A5B
		public static Quaternion LookAt(this Quaternion q, Vector3 from, Vector3 to, Vector3 up)
		{
			return Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(from, to, up));
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x0001986A File Offset: 0x00017A6A
		public static Quaternion LookAt(this Quaternion q, Vector3 direction, Vector3 up)
		{
			return Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(Vector3.Zero, direction, up));
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x0001987D File Offset: 0x00017A7D
		public static Vector3 Forward(this Quaternion q)
		{
			return Vector3.Transform(Vector3.Forward, q.Conjugated());
		}

		// Token: 0x06000A4B RID: 2635 RVA: 0x0001988F File Offset: 0x00017A8F
		public static Vector3 Left(this Quaternion q)
		{
			return Vector3.Transform(Vector3.Left, q.Conjugated());
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x000198A1 File Offset: 0x00017AA1
		public static Vector3 Up(this Quaternion q)
		{
			return Vector3.Transform(Vector3.Up, q.Conjugated());
		}
	}
}
