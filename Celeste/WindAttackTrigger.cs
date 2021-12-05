using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x020002DE RID: 734
	public class WindAttackTrigger : Trigger
	{
		// Token: 0x060016AE RID: 5806 RVA: 0x00042A43 File Offset: 0x00040C43
		public WindAttackTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
		}

		// Token: 0x060016AF RID: 5807 RVA: 0x000867BF File Offset: 0x000849BF
		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			if (base.Scene.Entities.FindFirst<Snowball>() == null)
			{
				base.Scene.Add(new Snowball());
			}
			base.RemoveSelf();
		}
	}
}
