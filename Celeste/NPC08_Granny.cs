using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x02000188 RID: 392
	public class NPC08_Granny : NPC
	{
		// Token: 0x06000DC6 RID: 3526 RVA: 0x00030CDC File Offset: 0x0002EEDC
		public NPC08_Granny(EntityData data, Vector2 position) : base(data.Position + position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("granny"));
			this.Sprite.Scale.X = -1f;
			this.Sprite.Play("idle", false, false);
			this.IdleAnim = "idle";
			this.MoveAnim = "walk";
			this.Maxspeed = 30f;
			base.Depth = -10;
		}
	}
}
