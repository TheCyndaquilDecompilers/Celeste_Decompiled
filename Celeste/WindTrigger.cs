using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002DF RID: 735
	[Tracked(false)]
	public class WindTrigger : Trigger
	{
		// Token: 0x060016B0 RID: 5808 RVA: 0x000867F0 File Offset: 0x000849F0
		public WindTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.Pattern = data.Enum<WindController.Patterns>("pattern", WindController.Patterns.None);
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x0008680C File Offset: 0x00084A0C
		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			WindController windController = base.Scene.Entities.FindFirst<WindController>();
			if (windController == null)
			{
				windController = new WindController(this.Pattern);
				base.Scene.Add(windController);
				return;
			}
			windController.SetPattern(this.Pattern);
		}

		// Token: 0x04001334 RID: 4916
		public WindController.Patterns Pattern;
	}
}
