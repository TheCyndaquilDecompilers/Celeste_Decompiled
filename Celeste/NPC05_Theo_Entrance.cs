using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000226 RID: 550
	public class NPC05_Theo_Entrance : NPC
	{
		// Token: 0x060011A5 RID: 4517 RVA: 0x000577A0 File Offset: 0x000559A0
		public NPC05_Theo_Entrance(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("theo"));
			this.IdleAnim = "idle";
			this.MoveAnim = "walk";
			this.Maxspeed = 48f;
			base.Add(this.Light = new VertexLight(-Vector2.UnitY * 12f, Color.White, 1f, 32, 64));
			base.SetupTheoSpriteSounds();
		}

		// Token: 0x060011A6 RID: 4518 RVA: 0x0005782F File Offset: 0x00055A2F
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (base.Session.GetFlag("entrance"))
			{
				base.RemoveSelf();
				return;
			}
			scene.Add(new CS05_Entrance(this));
		}
	}
}
