using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x02000274 RID: 628
	public class NPC04_Theo : NPC
	{
		// Token: 0x06001381 RID: 4993 RVA: 0x0006A0C0 File Offset: 0x000682C0
		public NPC04_Theo(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("theo"));
			this.IdleAnim = "idle";
			this.MoveAnim = "walk";
			this.Visible = false;
			this.Maxspeed = 48f;
			base.SetupTheoSpriteSounds();
		}

		// Token: 0x06001382 RID: 4994 RVA: 0x0006A120 File Offset: 0x00068320
		public override void Update()
		{
			base.Update();
			if (!this.started)
			{
				Gondola gondola = base.Scene.Entities.FindFirst<Gondola>();
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (gondola != null && entity != null && entity.X > gondola.Left - 16f)
				{
					this.started = true;
					base.Scene.Add(new CS04_Gondola(this, gondola, entity));
				}
			}
		}

		// Token: 0x04000F55 RID: 3925
		private bool started;
	}
}
