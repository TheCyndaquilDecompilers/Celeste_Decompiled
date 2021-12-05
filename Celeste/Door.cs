using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002C8 RID: 712
	[Tracked(false)]
	public class Door : Actor
	{
		// Token: 0x0600160C RID: 5644 RVA: 0x000806AC File Offset: 0x0007E8AC
		public Door(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Depth = 8998;
			string text = data.Attr("type", "wood");
			if (text == "wood")
			{
				base.Add(this.sprite = GFX.SpriteBank.Create("door"));
				this.openSfx = "event:/game/03_resort/door_wood_open";
				this.closeSfx = "event:/game/03_resort/door_wood_close";
			}
			else
			{
				base.Add(this.sprite = GFX.SpriteBank.Create(text + "door"));
				this.openSfx = "event:/game/03_resort/door_metal_open";
				this.closeSfx = "event:/game/03_resort/door_metal_close";
			}
			this.sprite.Play("idle", false, false);
			base.Collider = new Hitbox(12f, 22f, -6f, -23f);
			base.Add(this.occlude = new LightOcclude(new Rectangle(-1, -24, 2, 24), 1f));
			base.Add(new PlayerCollider(new Action<Player>(this.HitPlayer), null, null));
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x000807D2 File Offset: 0x0007E9D2
		public override bool IsRiding(Solid solid)
		{
			return base.Scene.CollideCheck(new Rectangle((int)base.X - 2, (int)base.Y - 2, 4, 4), solid);
		}

		// Token: 0x0600160E RID: 5646 RVA: 0x000091E2 File Offset: 0x000073E2
		protected override void OnSquish(CollisionData data)
		{
		}

		// Token: 0x0600160F RID: 5647 RVA: 0x000807F9 File Offset: 0x0007E9F9
		private void HitPlayer(Player player)
		{
			if (!this.disabled)
			{
				this.Open(player.X);
			}
		}

		// Token: 0x06001610 RID: 5648 RVA: 0x00080810 File Offset: 0x0007EA10
		public void Open(float x)
		{
			if (this.sprite.CurrentAnimationID == "idle")
			{
				Audio.Play(this.openSfx, this.Position);
				this.sprite.Play("open", false, false);
				if (base.X != x)
				{
					this.sprite.Scale.X = (float)Math.Sign(x - base.X);
					return;
				}
			}
			else if (this.sprite.CurrentAnimationID == "close")
			{
				this.sprite.Play("close", true, false);
			}
		}

		// Token: 0x06001611 RID: 5649 RVA: 0x000808AC File Offset: 0x0007EAAC
		public override void Update()
		{
			string currentAnimationID = this.sprite.CurrentAnimationID;
			base.Update();
			this.occlude.Visible = (this.sprite.CurrentAnimationID == "idle");
			if (!this.disabled && base.CollideCheck<Solid>())
			{
				this.disabled = true;
			}
			if (currentAnimationID == "close" && this.sprite.CurrentAnimationID == "idle")
			{
				Audio.Play(this.closeSfx, this.Position);
			}
		}

		// Token: 0x04001253 RID: 4691
		private Sprite sprite;

		// Token: 0x04001254 RID: 4692
		private string openSfx;

		// Token: 0x04001255 RID: 4693
		private string closeSfx;

		// Token: 0x04001256 RID: 4694
		private LightOcclude occlude;

		// Token: 0x04001257 RID: 4695
		private bool disabled;
	}
}
