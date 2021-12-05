using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200026F RID: 623
	public class NPC03_Oshiro_Rooftop : NPC
	{
		// Token: 0x06001360 RID: 4960 RVA: 0x00069770 File Offset: 0x00067970
		public NPC03_Oshiro_Rooftop(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = new OshiroSprite(1));
			(this.Sprite as OshiroSprite).AllowTurnInvisible = false;
			this.MoveAnim = "move";
			this.IdleAnim = "idle";
			base.Add(this.Light = new VertexLight(-Vector2.UnitY * 16f, Color.White, 1f, 32, 64));
		}

		// Token: 0x06001361 RID: 4961 RVA: 0x000697F6 File Offset: 0x000679F6
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (base.Session.GetFlag("oshiro_resort_roof"))
			{
				base.RemoveSelf();
				return;
			}
			this.Visible = false;
			base.Scene.Add(new CS03_OshiroRooftop(this));
		}
	}
}
