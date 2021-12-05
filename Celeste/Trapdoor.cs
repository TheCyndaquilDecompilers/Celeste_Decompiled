using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002CE RID: 718
	public class Trapdoor : Entity
	{
		// Token: 0x06001637 RID: 5687 RVA: 0x00082688 File Offset: 0x00080888
		public Trapdoor(EntityData data, Vector2 offset)
		{
			this.Position = data.Position + offset;
			base.Depth = 8999;
			base.Add(this.sprite = GFX.SpriteBank.Create("trapdoor"));
			this.sprite.Play("idle", false, false);
			this.sprite.Y = 6f;
			base.Collider = new Hitbox(24f, 4f, 0f, 6f);
			base.Add(this.playerCollider = new PlayerCollider(new Action<Player>(this.Open), null, null));
			base.Add(this.occluder = new LightOcclude(new Rectangle(0, 6, 24, 2), 1f));
		}

		// Token: 0x06001638 RID: 5688 RVA: 0x0008275C File Offset: 0x0008095C
		private void Open(Player player)
		{
			this.Collidable = false;
			this.occluder.Visible = false;
			if (player.Speed.Y >= 0f)
			{
				Audio.Play("event:/game/03_resort/trapdoor_fromtop", this.Position);
				this.sprite.Play("open", false, false);
				return;
			}
			Audio.Play("event:/game/03_resort/trapdoor_frombottom", this.Position);
			base.Add(new Coroutine(this.OpenFromBottom(), true));
		}

		// Token: 0x06001639 RID: 5689 RVA: 0x000827D5 File Offset: 0x000809D5
		private IEnumerator OpenFromBottom()
		{
			this.sprite.Scale.Y = -1f;
			yield return this.sprite.PlayRoutine("open_partial", false);
			yield return 0.1f;
			this.sprite.Rate = -1f;
			yield return this.sprite.PlayRoutine("open_partial", true);
			this.sprite.Scale.Y = 1f;
			this.sprite.Rate = 1f;
			this.sprite.Play("open", true, false);
			yield break;
		}

		// Token: 0x04001296 RID: 4758
		private Sprite sprite;

		// Token: 0x04001297 RID: 4759
		private PlayerCollider playerCollider;

		// Token: 0x04001298 RID: 4760
		private LightOcclude occluder;
	}
}
