using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x02000276 RID: 630
	public class BloomFadeTrigger : Trigger
	{
		// Token: 0x06001392 RID: 5010 RVA: 0x0006A85C File Offset: 0x00068A5C
		public BloomFadeTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.BloomAddFrom = data.Float("bloomAddFrom", 0f);
			this.BloomAddTo = data.Float("bloomAddTo", 0f);
			this.PositionMode = data.Enum<Trigger.PositionModes>("positionMode", Trigger.PositionModes.NoEffect);
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x0006A8B0 File Offset: 0x00068AB0
		public override void OnStay(Player player)
		{
			Level level = base.Scene as Level;
			Session session = level.Session;
			float num = this.BloomAddFrom + (this.BloomAddTo - this.BloomAddFrom) * MathHelper.Clamp(base.GetPositionLerp(player, this.PositionMode), 0f, 1f);
			session.BloomBaseAdd = num;
			level.Bloom.Base = AreaData.Get(level).BloomBase + num;
		}

		// Token: 0x04000F64 RID: 3940
		public float BloomAddFrom;

		// Token: 0x04000F65 RID: 3941
		public float BloomAddTo;

		// Token: 0x04000F66 RID: 3942
		public Trigger.PositionModes PositionMode;
	}
}
