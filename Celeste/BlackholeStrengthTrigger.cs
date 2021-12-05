using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x020001D0 RID: 464
	public class BlackholeStrengthTrigger : Trigger
	{
		// Token: 0x06000FBB RID: 4027 RVA: 0x00042AB4 File Offset: 0x00040CB4
		public BlackholeStrengthTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.strength = data.Enum<BlackholeBG.Strengths>("strength", BlackholeBG.Strengths.Mild);
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x00042AD0 File Offset: 0x00040CD0
		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			BlackholeBG blackholeBG = (base.Scene as Level).Background.Get<BlackholeBG>();
			if (blackholeBG != null)
			{
				blackholeBG.NextStrength(base.Scene as Level, this.strength);
			}
		}

		// Token: 0x04000B22 RID: 2850
		private BlackholeBG.Strengths strength;
	}
}
