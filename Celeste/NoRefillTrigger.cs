using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x020001E9 RID: 489
	public class NoRefillTrigger : Trigger
	{
		// Token: 0x0600103B RID: 4155 RVA: 0x0004670A File Offset: 0x0004490A
		public NoRefillTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.State = data.Bool("state", false);
		}

		// Token: 0x0600103C RID: 4156 RVA: 0x00046726 File Offset: 0x00044926
		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			base.SceneAs<Level>().Session.Inventory.NoRefills = this.State;
		}

		// Token: 0x04000B9A RID: 2970
		public bool State;
	}
}
