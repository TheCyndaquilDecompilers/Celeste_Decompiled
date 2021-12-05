using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000178 RID: 376
	[Tracked(false)]
	public class Killbox : Entity
	{
		// Token: 0x06000D53 RID: 3411 RVA: 0x0002D9A0 File Offset: 0x0002BBA0
		public Killbox(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Collider = new Hitbox((float)data.Width, 32f, 0f, 0f);
			this.Collidable = false;
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
		}

		// Token: 0x06000D54 RID: 3412 RVA: 0x0002DA00 File Offset: 0x0002BC00
		private void OnPlayer(Player player)
		{
			if (SaveData.Instance.Assists.Invincible)
			{
				player.Play("event:/game/general/assist_screenbottom", null, 0f);
				player.Bounce(base.Top);
				return;
			}
			player.Die(Vector2.Zero, false, true);
		}

		// Token: 0x06000D55 RID: 3413 RVA: 0x0002DA40 File Offset: 0x0002BC40
		public override void Update()
		{
			if (!this.Collidable)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.Bottom < base.Top - 32f)
				{
					this.Collidable = true;
				}
			}
			else
			{
				Player entity2 = base.Scene.Tracker.GetEntity<Player>();
				if (entity2 != null && entity2.Top > base.Bottom + 32f)
				{
					this.Collidable = false;
				}
			}
			base.Update();
		}
	}
}
