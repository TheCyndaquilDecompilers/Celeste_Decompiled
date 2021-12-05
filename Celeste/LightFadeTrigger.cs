using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x02000277 RID: 631
	public class LightFadeTrigger : Trigger
	{
		// Token: 0x06001394 RID: 5012 RVA: 0x0006A920 File Offset: 0x00068B20
		public LightFadeTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			base.AddTag(Tags.TransitionUpdate);
			this.LightAddFrom = data.Float("lightAddFrom", 0f);
			this.LightAddTo = data.Float("lightAddTo", 0f);
			this.PositionMode = data.Enum<Trigger.PositionModes>("positionMode", Trigger.PositionModes.NoEffect);
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x0006A984 File Offset: 0x00068B84
		public override void OnStay(Player player)
		{
			Level level = base.Scene as Level;
			Session session = level.Session;
			float num = this.LightAddFrom + (this.LightAddTo - this.LightAddFrom) * MathHelper.Clamp(base.GetPositionLerp(player, this.PositionMode), 0f, 1f);
			session.LightingAlphaAdd = num;
			level.Lighting.Alpha = level.BaseLightingAlpha + num;
		}

		// Token: 0x04000F67 RID: 3943
		public float LightAddFrom;

		// Token: 0x04000F68 RID: 3944
		public float LightAddTo;

		// Token: 0x04000F69 RID: 3945
		public Trigger.PositionModes PositionMode;
	}
}
