using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002C2 RID: 706
	public class DustTrackSpinner : TrackSpinner
	{
		// Token: 0x060015E6 RID: 5606 RVA: 0x0007EF0C File Offset: 0x0007D10C
		public DustTrackSpinner(EntityData data, Vector2 offset) : base(data, offset)
		{
			base.Add(this.dusty = new DustGraphic(true, false, false));
			this.dusty.EyeDirection = (this.dusty.EyeTargetDirection = (base.End - base.Start).SafeNormalize());
			this.dusty.OnEstablish = new Action(this.Establish);
			base.Depth = -50;
		}

		// Token: 0x060015E7 RID: 5607 RVA: 0x0007EF88 File Offset: 0x0007D188
		private void Establish()
		{
			Vector2 vector = (base.End - base.Start).SafeNormalize();
			Vector2 vector2 = new Vector2(-vector.Y, vector.X);
			bool flag = base.Scene.CollideCheck<Solid>(new Rectangle((int)(base.X + vector2.X * 4f) - 2, (int)(base.Y + vector2.Y * 4f) - 2, 4, 4));
			if (!flag)
			{
				vector2 = -vector2;
				flag = base.Scene.CollideCheck<Solid>(new Rectangle((int)(base.X + vector2.X * 4f) - 2, (int)(base.Y + vector2.Y * 4f) - 2, 4, 4));
			}
			if (flag)
			{
				float num = (base.End - base.Start).Length();
				int num2 = 8;
				while ((float)num2 < num && flag)
				{
					flag = (flag && base.Scene.CollideCheck<Solid>(new Rectangle((int)(base.X + vector2.X * 4f + vector.X * (float)num2) - 2, (int)(base.Y + vector2.Y * 4f + vector.Y * (float)num2) - 2, 4, 4)));
					num2 += 8;
				}
				if (flag)
				{
					List<DustGraphic.Node> list = null;
					if (vector2.X < 0f)
					{
						list = this.dusty.LeftNodes;
					}
					else if (vector2.X > 0f)
					{
						list = this.dusty.RightNodes;
					}
					else if (vector2.Y < 0f)
					{
						list = this.dusty.TopNodes;
					}
					else if (vector2.Y > 0f)
					{
						list = this.dusty.BottomNodes;
					}
					if (list != null)
					{
						foreach (DustGraphic.Node node in list)
						{
							node.Enabled = false;
						}
					}
					this.outwards = -vector2;
					this.dusty.Position -= vector2;
					this.dusty.EyeDirection = (this.dusty.EyeTargetDirection = Calc.AngleToVector(Calc.AngleLerp(this.outwards.Angle(), this.Up ? (this.Angle + 3.1415927f) : this.Angle, 0.3f), 1f));
				}
			}
		}

		// Token: 0x060015E8 RID: 5608 RVA: 0x0007F210 File Offset: 0x0007D410
		public override void Update()
		{
			base.Update();
			if (this.Moving && this.PauseTimer < 0f && base.Scene.OnInterval(0.02f))
			{
				base.SceneAs<Level>().ParticlesBG.Emit(DustStaticSpinner.P_Move, 1, this.Position, Vector2.One * 4f);
			}
		}

		// Token: 0x060015E9 RID: 5609 RVA: 0x0007F275 File Offset: 0x0007D475
		public override void OnPlayer(Player player)
		{
			base.OnPlayer(player);
			this.dusty.OnHitPlayer();
		}

		// Token: 0x060015EA RID: 5610 RVA: 0x0007F28C File Offset: 0x0007D48C
		public override void OnTrackEnd()
		{
			if (this.outwards != Vector2.Zero)
			{
				this.dusty.EyeTargetDirection = Calc.AngleToVector(Calc.AngleLerp(this.outwards.Angle(), this.Up ? (this.Angle + 3.1415927f) : this.Angle, 0.3f), 1f);
				return;
			}
			this.dusty.EyeTargetDirection = Calc.AngleToVector(this.Up ? (this.Angle + 3.1415927f) : this.Angle, 1f);
			this.dusty.EyeFlip = -this.dusty.EyeFlip;
		}

		// Token: 0x04001227 RID: 4647
		private DustGraphic dusty;

		// Token: 0x04001228 RID: 4648
		private Vector2 outwards;
	}
}
