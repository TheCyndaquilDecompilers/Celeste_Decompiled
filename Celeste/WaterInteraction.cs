using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200015E RID: 350
	[Tracked(false)]
	public class WaterInteraction : Component
	{
		// Token: 0x06000C78 RID: 3192 RVA: 0x00029D80 File Offset: 0x00027F80
		public WaterInteraction(Func<bool> isDashing) : base(false, false)
		{
			this.IsDashing = isDashing;
		}

		// Token: 0x06000C79 RID: 3193 RVA: 0x00029D94 File Offset: 0x00027F94
		public override void Update()
		{
			if (this.DrippingTimer > 0f)
			{
				this.DrippingTimer -= Engine.DeltaTime;
				if (base.Scene.OnInterval(0.1f))
				{
					float x = base.Entity.Left - 2f + Calc.Random.NextFloat(base.Entity.Width + 4f);
					float y = base.Entity.Top + this.DrippingOffset + Calc.Random.NextFloat(base.Entity.Height - this.DrippingOffset);
					(base.Scene as Level).ParticlesFG.Emit(WaterInteraction.P_Drip, new Vector2(x, y));
				}
			}
		}

		// Token: 0x040007E3 RID: 2019
		public static ParticleType P_Drip;

		// Token: 0x040007E4 RID: 2020
		public Func<bool> IsDashing;

		// Token: 0x040007E5 RID: 2021
		public float DrippingTimer;

		// Token: 0x040007E6 RID: 2022
		public float DrippingOffset;
	}
}
