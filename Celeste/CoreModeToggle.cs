using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001A7 RID: 423
	public class CoreModeToggle : Entity
	{
		// Token: 0x06000EB4 RID: 3764 RVA: 0x0003773C File Offset: 0x0003593C
		public CoreModeToggle(Vector2 position, bool onlyFire, bool onlyIce, bool persistent) : base(position)
		{
			this.onlyFire = onlyFire;
			this.onlyIce = onlyIce;
			this.persistent = persistent;
			base.Collider = new Hitbox(16f, 24f, -8f, -12f);
			base.Add(new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(this.sprite = GFX.SpriteBank.Create("coreFlipSwitch"));
			base.Depth = 2000;
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x000377DE File Offset: 0x000359DE
		public CoreModeToggle(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("onlyFire", false), data.Bool("onlyIce", false), data.Bool("persistent", false))
		{
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x00037816 File Offset: 0x00035A16
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.iceMode = (base.SceneAs<Level>().CoreMode == Session.CoreModes.Cold);
			this.SetSprite(false);
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x0003783A File Offset: 0x00035A3A
		private void OnChangeMode(Session.CoreModes mode)
		{
			this.iceMode = (mode == Session.CoreModes.Cold);
			this.SetSprite(true);
		}

		// Token: 0x06000EB8 RID: 3768 RVA: 0x00037850 File Offset: 0x00035A50
		private void SetSprite(bool animate)
		{
			if (animate)
			{
				if (this.playSounds)
				{
					Audio.Play(this.iceMode ? "event:/game/09_core/switch_to_cold" : "event:/game/09_core/switch_to_hot", this.Position);
				}
				if (this.Usable)
				{
					this.sprite.Play(this.iceMode ? "ice" : "hot", false, false);
				}
				else
				{
					if (this.playSounds)
					{
						Audio.Play("event:/game/09_core/switch_dies", this.Position);
					}
					this.sprite.Play(this.iceMode ? "iceOff" : "hotOff", false, false);
				}
			}
			else if (this.Usable)
			{
				this.sprite.Play(this.iceMode ? "iceLoop" : "hotLoop", false, false);
			}
			else
			{
				this.sprite.Play(this.iceMode ? "iceOffLoop" : "hotOffLoop", false, false);
			}
			this.playSounds = false;
		}

		// Token: 0x06000EB9 RID: 3769 RVA: 0x00037948 File Offset: 0x00035B48
		private void OnPlayer(Player player)
		{
			if (this.Usable && this.cooldownTimer <= 0f)
			{
				this.playSounds = true;
				Level level = base.SceneAs<Level>();
				if (level.CoreMode == Session.CoreModes.Cold)
				{
					level.CoreMode = Session.CoreModes.Hot;
				}
				else
				{
					level.CoreMode = Session.CoreModes.Cold;
				}
				if (this.persistent)
				{
					level.Session.CoreMode = level.CoreMode;
				}
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
				level.Flash(Color.White * 0.15f, true);
				Celeste.Freeze(0.05f);
				this.cooldownTimer = 1f;
			}
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x000379DC File Offset: 0x00035BDC
		public override void Update()
		{
			base.Update();
			if (this.cooldownTimer > 0f)
			{
				this.cooldownTimer -= Engine.DeltaTime;
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000EBB RID: 3771 RVA: 0x00037A03 File Offset: 0x00035C03
		private bool Usable
		{
			get
			{
				return (!this.onlyFire || this.iceMode) && (!this.onlyIce || !this.iceMode);
			}
		}

		// Token: 0x040009E8 RID: 2536
		private const float Cooldown = 1f;

		// Token: 0x040009E9 RID: 2537
		private bool iceMode;

		// Token: 0x040009EA RID: 2538
		private float cooldownTimer;

		// Token: 0x040009EB RID: 2539
		private bool onlyFire;

		// Token: 0x040009EC RID: 2540
		private bool onlyIce;

		// Token: 0x040009ED RID: 2541
		private bool persistent;

		// Token: 0x040009EE RID: 2542
		private bool playSounds;

		// Token: 0x040009EF RID: 2543
		private Sprite sprite;
	}
}
