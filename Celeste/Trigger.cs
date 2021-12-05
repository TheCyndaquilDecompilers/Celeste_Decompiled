using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200034E RID: 846
	[Tracked(true)]
	public abstract class Trigger : Entity
	{
		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06001A89 RID: 6793 RVA: 0x000AB63A File Offset: 0x000A983A
		// (set) Token: 0x06001A8A RID: 6794 RVA: 0x000AB642 File Offset: 0x000A9842
		public bool PlayerIsInside { get; private set; }

		// Token: 0x06001A8B RID: 6795 RVA: 0x000AB64B File Offset: 0x000A984B
		public Trigger(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Collider = new Hitbox((float)data.Width, (float)data.Height, 0f, 0f);
			this.Visible = false;
		}

		// Token: 0x06001A8C RID: 6796 RVA: 0x000AB689 File Offset: 0x000A9889
		public virtual void OnEnter(Player player)
		{
			this.PlayerIsInside = true;
		}

		// Token: 0x06001A8D RID: 6797 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void OnStay(Player player)
		{
		}

		// Token: 0x06001A8E RID: 6798 RVA: 0x000AB692 File Offset: 0x000A9892
		public virtual void OnLeave(Player player)
		{
			this.PlayerIsInside = false;
		}

		// Token: 0x06001A8F RID: 6799 RVA: 0x000AB69C File Offset: 0x000A989C
		protected float GetPositionLerp(Player player, Trigger.PositionModes mode)
		{
			switch (mode)
			{
			default:
				return 1f;
			case Trigger.PositionModes.HorizontalCenter:
				return Math.Min(Calc.ClampedMap(player.CenterX, base.Left, base.CenterX, 0f, 1f), Calc.ClampedMap(player.CenterX, base.Right, base.CenterX, 0f, 1f));
			case Trigger.PositionModes.VerticalCenter:
				return Math.Min(Calc.ClampedMap(player.CenterY, base.Top, base.CenterY, 0f, 1f), Calc.ClampedMap(player.CenterY, base.Bottom, base.CenterY, 0f, 1f));
			case Trigger.PositionModes.TopToBottom:
				return Calc.ClampedMap(player.CenterY, base.Top, base.Bottom, 0f, 1f);
			case Trigger.PositionModes.BottomToTop:
				return Calc.ClampedMap(player.CenterY, base.Bottom, base.Top, 0f, 1f);
			case Trigger.PositionModes.LeftToRight:
				return Calc.ClampedMap(player.CenterX, base.Left, base.Right, 0f, 1f);
			case Trigger.PositionModes.RightToLeft:
				return Calc.ClampedMap(player.CenterX, base.Right, base.Left, 0f, 1f);
			}
		}

		// Token: 0x0400172C RID: 5932
		public bool Triggered;

		// Token: 0x0200070A RID: 1802
		public enum PositionModes
		{
			// Token: 0x04002D70 RID: 11632
			NoEffect,
			// Token: 0x04002D71 RID: 11633
			HorizontalCenter,
			// Token: 0x04002D72 RID: 11634
			VerticalCenter,
			// Token: 0x04002D73 RID: 11635
			TopToBottom,
			// Token: 0x04002D74 RID: 11636
			BottomToTop,
			// Token: 0x04002D75 RID: 11637
			LeftToRight,
			// Token: 0x04002D76 RID: 11638
			RightToLeft
		}
	}
}
