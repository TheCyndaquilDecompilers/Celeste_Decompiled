using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200026B RID: 619
	public class NPC03_Oshiro_Hallway1 : NPC
	{
		// Token: 0x06001351 RID: 4945 RVA: 0x00069164 File Offset: 0x00067364
		public NPC03_Oshiro_Hallway1(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = new OshiroSprite(-1));
			base.Add(this.Light = new VertexLight(-Vector2.UnitY * 16f, Color.White, 1f, 32, 64));
			this.MoveAnim = "move";
			this.IdleAnim = "idle";
		}

		// Token: 0x06001352 RID: 4946 RVA: 0x000691D9 File Offset: 0x000673D9
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (base.Session.GetFlag("oshiro_resort_talked_2"))
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06001353 RID: 4947 RVA: 0x000691FC File Offset: 0x000673FC
		public override void Update()
		{
			base.Update();
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (!this.talked && entity != null && entity.X > base.X - 60f)
			{
				base.Scene.Add(new CS03_OshiroHallway1(entity, this));
				this.talked = true;
			}
		}

		// Token: 0x04000F3E RID: 3902
		private bool talked;
	}
}
