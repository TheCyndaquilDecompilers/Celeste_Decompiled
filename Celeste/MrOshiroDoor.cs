using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002C7 RID: 711
	public class MrOshiroDoor : Solid
	{
		// Token: 0x06001606 RID: 5638 RVA: 0x0008050C File Offset: 0x0007E70C
		public MrOshiroDoor(EntityData data, Vector2 offset) : base(data.Position + offset, 32f, 32f, false)
		{
			base.Add(this.sprite = GFX.SpriteBank.Create("ghost_door"));
			this.sprite.Position = new Vector2(base.Width, base.Height) / 2f;
			this.sprite.Play("idle", false, false);
			this.OnDashCollide = new DashCollision(this.OnDashed);
			base.Add(this.wiggler = Wiggler.Create(0.6f, 3f, delegate(float f)
			{
				this.sprite.Scale = Vector2.One * (1f - f * 0.2f);
			}, false, false));
			this.SurfaceSoundIndex = 20;
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x000805D4 File Offset: 0x0007E7D4
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.Visible = (this.Collidable = !base.SceneAs<Level>().Session.GetFlag("oshiro_resort_talked_1"));
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x0008060F File Offset: 0x0007E80F
		public void Open()
		{
			if (this.Collidable)
			{
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
				Audio.Play("event:/game/03_resort/forcefield_vanish", this.Position);
				this.sprite.Play("open", false, false);
				this.Collidable = false;
			}
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x0008064C File Offset: 0x0007E84C
		public void InstantOpen()
		{
			this.Collidable = (this.Visible = false);
		}

		// Token: 0x0600160A RID: 5642 RVA: 0x00080669 File Offset: 0x0007E869
		private DashCollisionResults OnDashed(Player player, Vector2 direction)
		{
			Audio.Play("event:/game/03_resort/forcefield_bump", this.Position);
			this.wiggler.Start();
			return DashCollisionResults.Bounce;
		}

		// Token: 0x04001251 RID: 4689
		private Sprite sprite;

		// Token: 0x04001252 RID: 4690
		private Wiggler wiggler;
	}
}
