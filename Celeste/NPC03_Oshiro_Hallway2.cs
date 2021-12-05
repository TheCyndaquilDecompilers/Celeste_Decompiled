using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200026C RID: 620
	public class NPC03_Oshiro_Hallway2 : NPC
	{
		// Token: 0x06001354 RID: 4948 RVA: 0x00069258 File Offset: 0x00067458
		public NPC03_Oshiro_Hallway2(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = new OshiroSprite(-1));
			base.Add(this.Light = new VertexLight(-Vector2.UnitY * 16f, Color.White, 1f, 32, 64));
			this.MoveAnim = "move";
			this.IdleAnim = "idle";
		}

		// Token: 0x06001355 RID: 4949 RVA: 0x000692CD File Offset: 0x000674CD
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (base.Session.GetFlag("oshiro_resort_talked_3"))
			{
				base.RemoveSelf();
				return;
			}
			base.Session.LightingAlphaAdd = 0.15f;
		}

		// Token: 0x06001356 RID: 4950 RVA: 0x00069300 File Offset: 0x00067500
		public override void Update()
		{
			base.Update();
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (!this.talked && entity != null && entity.X > base.X - 60f)
			{
				base.Scene.Add(new CS03_OshiroHallway2(entity, this));
				this.talked = true;
			}
		}

		// Token: 0x04000F3F RID: 3903
		private bool talked;
	}
}
