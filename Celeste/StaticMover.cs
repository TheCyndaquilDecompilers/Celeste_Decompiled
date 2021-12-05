using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200033B RID: 827
	[Tracked(false)]
	public class StaticMover : Component
	{
		// Token: 0x060019EA RID: 6634 RVA: 0x0000B61B File Offset: 0x0000981B
		public StaticMover() : base(false, false)
		{
		}

		// Token: 0x060019EB RID: 6635 RVA: 0x000A69E0 File Offset: 0x000A4BE0
		public void Destroy()
		{
			if (this.OnDestroy != null)
			{
				this.OnDestroy();
				return;
			}
			base.Entity.RemoveSelf();
		}

		// Token: 0x060019EC RID: 6636 RVA: 0x000A6A01 File Offset: 0x000A4C01
		public void Shake(Vector2 amount)
		{
			if (this.OnShake != null)
			{
				this.OnShake(amount);
			}
		}

		// Token: 0x060019ED RID: 6637 RVA: 0x000A6A17 File Offset: 0x000A4C17
		public void Move(Vector2 amount)
		{
			if (this.OnMove != null)
			{
				this.OnMove(amount);
				return;
			}
			base.Entity.Position += amount;
		}

		// Token: 0x060019EE RID: 6638 RVA: 0x000A6A45 File Offset: 0x000A4C45
		public bool IsRiding(Solid solid)
		{
			return this.SolidChecker != null && this.SolidChecker(solid);
		}

		// Token: 0x060019EF RID: 6639 RVA: 0x000A6A5D File Offset: 0x000A4C5D
		public bool IsRiding(JumpThru jumpThru)
		{
			return this.JumpThruChecker != null && this.JumpThruChecker(jumpThru);
		}

		// Token: 0x060019F0 RID: 6640 RVA: 0x000A6A75 File Offset: 0x000A4C75
		public void TriggerPlatform()
		{
			if (this.Platform != null)
			{
				this.Platform.OnStaticMoverTrigger(this);
			}
		}

		// Token: 0x060019F1 RID: 6641 RVA: 0x000A6A8C File Offset: 0x000A4C8C
		public void Disable()
		{
			if (this.OnDisable != null)
			{
				this.OnDisable();
				return;
			}
			base.Entity.Active = (base.Entity.Visible = (base.Entity.Collidable = false));
		}

		// Token: 0x060019F2 RID: 6642 RVA: 0x000A6AD8 File Offset: 0x000A4CD8
		public void Enable()
		{
			if (this.OnEnable != null)
			{
				this.OnEnable();
				return;
			}
			base.Entity.Active = (base.Entity.Visible = (base.Entity.Collidable = true));
		}

		// Token: 0x04001698 RID: 5784
		public Func<Solid, bool> SolidChecker;

		// Token: 0x04001699 RID: 5785
		public Func<JumpThru, bool> JumpThruChecker;

		// Token: 0x0400169A RID: 5786
		public Action<Vector2> OnMove;

		// Token: 0x0400169B RID: 5787
		public Action<Vector2> OnShake;

		// Token: 0x0400169C RID: 5788
		public Action<Platform> OnAttach;

		// Token: 0x0400169D RID: 5789
		public Action OnDestroy;

		// Token: 0x0400169E RID: 5790
		public Action OnDisable;

		// Token: 0x0400169F RID: 5791
		public Action OnEnable;

		// Token: 0x040016A0 RID: 5792
		public Platform Platform;
	}
}
