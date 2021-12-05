using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001C4 RID: 452
	public class LevelUpEffect : Entity
	{
		// Token: 0x06000F78 RID: 3960 RVA: 0x0003F7E8 File Offset: 0x0003D9E8
		public LevelUpEffect(Vector2 position) : base(position)
		{
			base.Depth = -1000000;
			Audio.Play("event:/game/06_reflection/hug_levelup_text_in", this.Position);
			base.Add(this.sprite = GFX.SpriteBank.Create("player_level_up"));
			this.sprite.OnLastFrame = delegate(string anim)
			{
				base.RemoveSelf();
			};
			this.sprite.OnFrameChange = delegate(string anim)
			{
				if (this.sprite.CurrentAnimationFrame == 20)
				{
					Audio.Play("event:/game/06_reflection/hug_levelup_text_out");
				}
			};
			this.sprite.Play("levelUp", false, false);
		}

		// Token: 0x04000ADC RID: 2780
		private Sprite sprite;
	}
}
