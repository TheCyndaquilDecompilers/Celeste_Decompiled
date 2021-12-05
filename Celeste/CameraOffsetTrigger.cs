using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x02000328 RID: 808
	public class CameraOffsetTrigger : Trigger
	{
		// Token: 0x0600196A RID: 6506 RVA: 0x000A368C File Offset: 0x000A188C
		public CameraOffsetTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.CameraOffset = new Vector2(data.Float("cameraX", 0f), data.Float("cameraY", 0f));
			this.CameraOffset.X = this.CameraOffset.X * 48f;
			this.CameraOffset.Y = this.CameraOffset.Y * 32f;
		}

		// Token: 0x0600196B RID: 6507 RVA: 0x000A36F4 File Offset: 0x000A18F4
		public override void OnEnter(Player player)
		{
			base.SceneAs<Level>().CameraOffset = this.CameraOffset;
		}

		// Token: 0x04001629 RID: 5673
		public Vector2 CameraOffset;
	}
}
