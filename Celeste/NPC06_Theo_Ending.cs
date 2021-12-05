using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001A5 RID: 421
	public class NPC06_Theo_Ending : NPC
	{
		// Token: 0x06000EAF RID: 3759 RVA: 0x000374E0 File Offset: 0x000356E0
		public NPC06_Theo_Ending(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("theo"));
			this.IdleAnim = "idle";
			this.MoveAnim = "run";
			this.Maxspeed = 72f;
			this.MoveY = false;
			this.Visible = false;
			base.Add(this.Light = new VertexLight(new Vector2(0f, -8f), Color.White, 1f, 16, 32));
			base.SetupTheoSpriteSounds();
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x00037584 File Offset: 0x00035784
		public override void Update()
		{
			base.Update();
			if (!base.CollideCheck<Solid>(this.Position + new Vector2(0f, 1f)))
			{
				this.speedY += 400f * Engine.DeltaTime;
				this.Position.Y = this.Position.Y + this.speedY * Engine.DeltaTime;
				return;
			}
			this.speedY = 0f;
		}

		// Token: 0x040009E6 RID: 2534
		private float speedY;
	}
}
