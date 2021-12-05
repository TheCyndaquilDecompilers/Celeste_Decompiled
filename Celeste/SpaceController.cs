using System;
using Monocle;

namespace Celeste
{
	// Token: 0x020001AD RID: 429
	public class SpaceController : Entity
	{
		// Token: 0x06000F04 RID: 3844 RVA: 0x0003B1AD File Offset: 0x000393AD
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
		}

		// Token: 0x06000F05 RID: 3845 RVA: 0x0003B1C4 File Offset: 0x000393C4
		public override void Update()
		{
			base.Update();
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				if (entity.Top > this.level.Camera.Bottom + 12f)
				{
					entity.Bottom = this.level.Camera.Top - 4f;
					return;
				}
				if (entity.Bottom < this.level.Camera.Top - 4f)
				{
					entity.Top = this.level.Camera.Bottom + 12f;
				}
			}
		}

		// Token: 0x04000A56 RID: 2646
		private Level level;
	}
}
