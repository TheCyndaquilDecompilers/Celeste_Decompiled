using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x02000187 RID: 391
	public class NPC08_Theo : NPC
	{
		// Token: 0x06000DC5 RID: 3525 RVA: 0x00030C54 File Offset: 0x0002EE54
		public NPC08_Theo(EntityData data, Vector2 position) : base(data.Position + position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("theo"));
			this.Sprite.Scale.X = -1f;
			this.Sprite.Play("idle", false, false);
			this.IdleAnim = "idle";
			this.MoveAnim = "walk";
			this.Maxspeed = 30f;
		}
	}
}
