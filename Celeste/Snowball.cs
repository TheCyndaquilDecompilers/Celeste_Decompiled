using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002C3 RID: 707
	public class Snowball : Entity
	{
		// Token: 0x060015EB RID: 5611 RVA: 0x0007F33C File Offset: 0x0007D53C
		public Snowball()
		{
			base.Depth = -12500;
			base.Collider = new Hitbox(12f, 9f, -5f, -2f);
			this.bounceCollider = new Hitbox(16f, 6f, -6f, -8f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayerBounce), this.bounceCollider, null));
			base.Add(this.sine = new SineWave(0.5f, 0f));
			base.Add(this.sprite = GFX.SpriteBank.Create("snowball"));
			this.sprite.Play("spin", false, false);
			base.Add(this.spawnSfx = new SoundSource());
		}

		// Token: 0x060015EC RID: 5612 RVA: 0x0007F431 File Offset: 0x0007D631
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
			this.ResetPosition();
		}

		// Token: 0x060015ED RID: 5613 RVA: 0x0007F44C File Offset: 0x0007D64C
		private void ResetPosition()
		{
			Player entity = this.level.Tracker.GetEntity<Player>();
			if (entity != null && entity.Right < (float)(this.level.Bounds.Right - 64))
			{
				this.spawnSfx.Play("event:/game/04_cliffside/snowball_spawn", null, 0f);
				this.Collidable = (this.Visible = true);
				this.resetTimer = 0f;
				base.X = this.level.Camera.Right + 10f;
				this.atY = (base.Y = entity.CenterY);
				this.sine.Reset();
				this.sprite.Play("spin", false, false);
				return;
			}
			this.resetTimer = 0.05f;
		}

		// Token: 0x060015EE RID: 5614 RVA: 0x0007F51E File Offset: 0x0007D71E
		private void Destroy()
		{
			this.Collidable = false;
			this.sprite.Play("break", false, false);
		}

		// Token: 0x060015EF RID: 5615 RVA: 0x0007F539 File Offset: 0x0007D739
		private void OnPlayer(Player player)
		{
			player.Die(new Vector2(-1f, 0f), false, true);
			this.Destroy();
			Audio.Play("event:/game/04_cliffside/snowball_impact", this.Position);
		}

		// Token: 0x060015F0 RID: 5616 RVA: 0x0007F56A File Offset: 0x0007D76A
		private void OnPlayerBounce(Player player)
		{
			if (!base.CollideCheck(player))
			{
				Celeste.Freeze(0.1f);
				player.Bounce(base.Top - 2f);
				this.Destroy();
				Audio.Play("event:/game/general/thing_booped", this.Position);
			}
		}

		// Token: 0x060015F1 RID: 5617 RVA: 0x0007F5A8 File Offset: 0x0007D7A8
		public override void Update()
		{
			base.Update();
			base.X -= 200f * Engine.DeltaTime;
			base.Y = this.atY + 4f * this.sine.Value;
			if (base.X < this.level.Camera.Left - 60f)
			{
				this.resetTimer += Engine.DeltaTime;
				if (this.resetTimer >= 0.8f)
				{
					this.ResetPosition();
				}
			}
		}

		// Token: 0x060015F2 RID: 5618 RVA: 0x0007F634 File Offset: 0x0007D834
		public override void Render()
		{
			this.sprite.DrawOutline(1);
			base.Render();
		}

		// Token: 0x04001229 RID: 4649
		private const float ResetTime = 0.8f;

		// Token: 0x0400122A RID: 4650
		private Sprite sprite;

		// Token: 0x0400122B RID: 4651
		private float resetTimer;

		// Token: 0x0400122C RID: 4652
		private Level level;

		// Token: 0x0400122D RID: 4653
		private SineWave sine;

		// Token: 0x0400122E RID: 4654
		private float atY;

		// Token: 0x0400122F RID: 4655
		private SoundSource spawnSfx;

		// Token: 0x04001230 RID: 4656
		private Collider bounceCollider;
	}
}
