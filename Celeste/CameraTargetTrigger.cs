using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000329 RID: 809
	[Tracked(false)]
	public class CameraTargetTrigger : Trigger
	{
		// Token: 0x0600196C RID: 6508 RVA: 0x000A3708 File Offset: 0x000A1908
		public CameraTargetTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.Target = data.Nodes[0] + offset - new Vector2(320f, 180f) * 0.5f;
			this.LerpStrength = data.Float("lerpStrength", 0f);
			this.PositionMode = data.Enum<Trigger.PositionModes>("positionMode", Trigger.PositionModes.NoEffect);
			this.XOnly = data.Bool("xOnly", false);
			this.YOnly = data.Bool("yOnly", false);
			this.DeleteFlag = data.Attr("deleteFlag", "");
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x000A37B8 File Offset: 0x000A19B8
		public override void OnStay(Player player)
		{
			if (string.IsNullOrEmpty(this.DeleteFlag) || !base.SceneAs<Level>().Session.GetFlag(this.DeleteFlag))
			{
				player.CameraAnchor = this.Target;
				player.CameraAnchorLerp = Vector2.One * MathHelper.Clamp(this.LerpStrength * base.GetPositionLerp(player, this.PositionMode), 0f, 1f);
				player.CameraAnchorIgnoreX = this.YOnly;
				player.CameraAnchorIgnoreY = this.XOnly;
			}
		}

		// Token: 0x0600196E RID: 6510 RVA: 0x000A3844 File Offset: 0x000A1A44
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

		// Token: 0x0400162A RID: 5674
		public Vector2 Target;

		// Token: 0x0400162B RID: 5675
		public float LerpStrength;

		// Token: 0x0400162C RID: 5676
		public Trigger.PositionModes PositionMode;

		// Token: 0x0400162D RID: 5677
		public bool XOnly;

		// Token: 0x0400162E RID: 5678
		public bool YOnly;

		// Token: 0x0400162F RID: 5679
		public string DeleteFlag;
	}
}
