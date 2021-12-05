using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001E2 RID: 482
	[Tracked(false)]
	public class CameraAdvanceTargetTrigger : Trigger
	{
		// Token: 0x06001027 RID: 4135 RVA: 0x00045CD4 File Offset: 0x00043ED4
		public CameraAdvanceTargetTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.Target = data.Nodes[0] + offset - new Vector2(320f, 180f) * 0.5f;
			this.LerpStrength.X = data.Float("lerpStrengthX", 0f);
			this.LerpStrength.Y = data.Float("lerpStrengthY", 0f);
			this.PositionModeX = data.Enum<Trigger.PositionModes>("positionModeX", Trigger.PositionModes.NoEffect);
			this.PositionModeY = data.Enum<Trigger.PositionModes>("positionModeY", Trigger.PositionModes.NoEffect);
			this.XOnly = data.Bool("xOnly", false);
			this.YOnly = data.Bool("yOnly", false);
		}

		// Token: 0x06001028 RID: 4136 RVA: 0x00045DA0 File Offset: 0x00043FA0
		public override void OnStay(Player player)
		{
			player.CameraAnchor = this.Target;
			player.CameraAnchorLerp.X = MathHelper.Clamp(this.LerpStrength.X * base.GetPositionLerp(player, this.PositionModeX), 0f, 1f);
			player.CameraAnchorLerp.Y = MathHelper.Clamp(this.LerpStrength.Y * base.GetPositionLerp(player, this.PositionModeY), 0f, 1f);
			player.CameraAnchorIgnoreX = this.YOnly;
			player.CameraAnchorIgnoreY = this.XOnly;
		}

		// Token: 0x06001029 RID: 4137 RVA: 0x00045E38 File Offset: 0x00044038
		public override void OnLeave(Player player)
		{
			base.OnLeave(player);
			bool flag = false;
			using (List<Entity>.Enumerator enumerator = base.Scene.Tracker.GetEntities<CameraTargetTrigger>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((CameraTargetTrigger)enumerator.Current).PlayerIsInside)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				using (List<Entity>.Enumerator enumerator = base.Scene.Tracker.GetEntities<CameraAdvanceTargetTrigger>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (((CameraAdvanceTargetTrigger)enumerator.Current).PlayerIsInside)
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				player.CameraAnchorLerp = Vector2.Zero;
			}
		}

		// Token: 0x04000B88 RID: 2952
		public Vector2 Target;

		// Token: 0x04000B89 RID: 2953
		public Vector2 LerpStrength;

		// Token: 0x04000B8A RID: 2954
		public Trigger.PositionModes PositionModeX;

		// Token: 0x04000B8B RID: 2955
		public Trigger.PositionModes PositionModeY;

		// Token: 0x04000B8C RID: 2956
		public bool XOnly;

		// Token: 0x04000B8D RID: 2957
		public bool YOnly;
	}
}
