using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x020001EA RID: 490
	public class OshiroTrigger : Trigger
	{
		// Token: 0x0600103D RID: 4157 RVA: 0x0004674A File Offset: 0x0004494A
		public OshiroTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.State = data.Bool("state", true);
		}

		// Token: 0x0600103E RID: 4158 RVA: 0x00046768 File Offset: 0x00044968
		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			if (this.State)
			{
				Level level = base.SceneAs<Level>();
				Vector2 position = new Vector2((float)(level.Bounds.Left - 32), (float)(level.Bounds.Top + level.Bounds.Height / 2));
				base.Scene.Add(new AngryOshiro(position, false));
				base.RemoveSelf();
				return;
			}
			AngryOshiro entity = base.Scene.Tracker.GetEntity<AngryOshiro>();
			if (entity != null)
			{
				entity.Leave();
			}
			base.RemoveSelf();
		}

		// Token: 0x04000B9B RID: 2971
		public bool State;
	}
}
