using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200026A RID: 618
	public class NPC03_Oshiro_Breakdown : NPC
	{
		// Token: 0x0600134E RID: 4942 RVA: 0x00069010 File Offset: 0x00067210
		public NPC03_Oshiro_Breakdown(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = new OshiroSprite(1));
			base.Add(this.Light = new VertexLight(-Vector2.UnitY * 16f, Color.White, 1f, 32, 64));
			this.MoveAnim = "move";
			this.IdleAnim = "idle";
		}

		// Token: 0x0600134F RID: 4943 RVA: 0x00069085 File Offset: 0x00067285
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (base.Session.GetFlag("oshiro_breakdown"))
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06001350 RID: 4944 RVA: 0x000690A8 File Offset: 0x000672A8
		public override void Update()
		{
			base.Update();
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (!this.talked && entity != null && ((entity.X <= (float)(this.Level.Bounds.Left + 370) && entity.OnSafeGround && entity.Y < (float)this.Level.Bounds.Center.Y) || entity.X <= (float)(this.Level.Bounds.Left + 320)))
			{
				base.Scene.Add(new CS03_OshiroBreakdown(entity, this));
				this.talked = true;
			}
		}

		// Token: 0x04000F3D RID: 3901
		private bool talked;
	}
}
