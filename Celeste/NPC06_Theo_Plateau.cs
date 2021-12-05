using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001A6 RID: 422
	public class NPC06_Theo_Plateau : NPC
	{
		// Token: 0x06000EB1 RID: 3761 RVA: 0x000375F8 File Offset: 0x000357F8
		public NPC06_Theo_Plateau(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("theo"));
			this.IdleAnim = "idle";
			this.MoveAnim = "walk";
			this.Maxspeed = 48f;
			this.MoveY = false;
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x00037660 File Offset: 0x00035860
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			base.Scene.Add(new CS06_Campfire(this, entity));
			base.Add(this.Light = new VertexLight(new Vector2(0f, -6f), Color.White, 1f, 16, 48));
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x000376C8 File Offset: 0x000358C8
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

		// Token: 0x040009E7 RID: 2535
		private float speedY;
	}
}
