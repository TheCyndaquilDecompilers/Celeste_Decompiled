using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x020001A3 RID: 419
	public class NPC06_Granny_Ending : NPC
	{
		// Token: 0x06000EA9 RID: 3753 RVA: 0x000371A4 File Offset: 0x000353A4
		public NPC06_Granny_Ending(EntityData data, Vector2 position) : base(data.Position + position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("granny"));
			this.Sprite.Scale.X = -1f;
			this.Sprite.Play("idle", false, false);
			this.IdleAnim = "idle";
			this.MoveAnim = "walk";
			this.Maxspeed = 30f;
			this.Visible = false;
			base.Add(this.Light = new VertexLight(new Vector2(0f, -8f), Color.White, 1f, 16, 32));
			base.SetupGrannySpriteSounds();
		}

		// Token: 0x06000EAA RID: 3754 RVA: 0x00037268 File Offset: 0x00035468
		public override void Update()
		{
			base.Update();
			if (!this.talked)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.OnGround(1))
				{
					this.talked = true;
					base.Scene.Add(new CS06_Ending(entity, this));
				}
			}
		}

		// Token: 0x040009E3 RID: 2531
		private bool talked;
	}
}
