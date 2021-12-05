using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001E4 RID: 484
	[Tracked(false)]
	public class CreditsTrigger : Trigger
	{
		// Token: 0x0600102B RID: 4139 RVA: 0x00045F0C File Offset: 0x0004410C
		public CreditsTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.Event = data.Attr("event", "");
		}

		// Token: 0x0600102C RID: 4140 RVA: 0x00045F2C File Offset: 0x0004412C
		public override void OnEnter(Player player)
		{
			this.Triggered = true;
			if (CS07_Credits.Instance != null)
			{
				CS07_Credits.Instance.Event = this.Event;
			}
		}

		// Token: 0x04000B8E RID: 2958
		public string Event;
	}
}
