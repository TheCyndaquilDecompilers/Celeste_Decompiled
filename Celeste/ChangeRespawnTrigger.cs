using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200022C RID: 556
	public class ChangeRespawnTrigger : Trigger
	{
		// Token: 0x060011C2 RID: 4546 RVA: 0x00058994 File Offset: 0x00056B94
		public ChangeRespawnTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			base.Collider = new Hitbox((float)data.Width, (float)data.Height, 0f, 0f);
			if (data.Nodes != null && data.Nodes.Length != 0)
			{
				this.Target = data.Nodes[0] + offset;
			}
			else
			{
				this.Target = base.Center;
			}
			this.Visible = (this.Active = false);
		}

		// Token: 0x060011C3 RID: 4547 RVA: 0x00058A13 File Offset: 0x00056C13
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.Target = base.SceneAs<Level>().GetSpawnPoint(this.Target);
		}

		// Token: 0x060011C4 RID: 4548 RVA: 0x00058A34 File Offset: 0x00056C34
		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			Session session = (base.Scene as Level).Session;
			if (this.SolidCheck() && (session.RespawnPoint == null || session.RespawnPoint.Value != this.Target))
			{
				session.HitCheckpoint = true;
				session.RespawnPoint = new Vector2?(this.Target);
				session.UpdateLevelStartDashes();
			}
		}

		// Token: 0x060011C5 RID: 4549 RVA: 0x00058AA4 File Offset: 0x00056CA4
		private bool SolidCheck()
		{
			Vector2 point = this.Target + Vector2.UnitY * -4f;
			return !base.Scene.CollideCheck<Solid>(point) || base.Scene.CollideCheck<FloatySpaceBlock>(point);
		}

		// Token: 0x04000D69 RID: 3433
		public Vector2 Target;
	}
}
