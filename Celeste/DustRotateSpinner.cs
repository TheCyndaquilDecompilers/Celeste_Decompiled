using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200032B RID: 811
	public class DustRotateSpinner : RotateSpinner
	{
		// Token: 0x06001970 RID: 6512 RVA: 0x000A3984 File Offset: 0x000A1B84
		public DustRotateSpinner(EntityData data, Vector2 offset) : base(data, offset)
		{
			base.Add(this.dusty = new DustGraphic(true, false, false));
		}

		// Token: 0x06001971 RID: 6513 RVA: 0x000A39B0 File Offset: 0x000A1BB0
		public override void Update()
		{
			base.Update();
			if (this.Moving)
			{
				this.dusty.EyeDirection = (this.dusty.EyeTargetDirection = Calc.AngleToVector(base.Angle + 1.5707964f * (float)(base.Clockwise ? 1 : -1), 1f));
				if (base.Scene.OnInterval(0.02f))
				{
					base.SceneAs<Level>().ParticlesBG.Emit(DustStaticSpinner.P_Move, 1, this.Position, Vector2.One * 4f);
				}
			}
		}

		// Token: 0x06001972 RID: 6514 RVA: 0x000A3A45 File Offset: 0x000A1C45
		public override void OnPlayer(Player player)
		{
			base.OnPlayer(player);
			this.dusty.OnHitPlayer();
		}

		// Token: 0x04001631 RID: 5681
		private DustGraphic dusty;
	}
}
