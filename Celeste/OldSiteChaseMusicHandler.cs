using System;
using Monocle;

namespace Celeste
{
	// Token: 0x020002CF RID: 719
	public class OldSiteChaseMusicHandler : Entity
	{
		// Token: 0x0600163A RID: 5690 RVA: 0x000827E4 File Offset: 0x000809E4
		public OldSiteChaseMusicHandler()
		{
			base.Tag = (Tags.TransitionUpdate | Tags.Global);
		}

		// Token: 0x0600163B RID: 5691 RVA: 0x00082808 File Offset: 0x00080A08
		public override void Update()
		{
			base.Update();
			int num = 1150;
			int num2 = 2832;
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null && Audio.CurrentMusic == "event:/music/lvl2/chase")
			{
				float value = (entity.X - (float)num) / (float)(num2 - num);
				Audio.SetMusicParam("escape", value);
			}
		}
	}
}
