using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001AB RID: 427
	public class FakeHeart : Entity
	{
		// Token: 0x06000EE9 RID: 3817 RVA: 0x00039A24 File Offset: 0x00037C24
		public FakeHeart(Vector2 position) : base(position)
		{
			base.Add(this.crystalCollider = new HoldableCollider(new Action<Holdable>(this.OnHoldable), null));
			base.Add(new MirrorReflection());
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x00039A64 File Offset: 0x00037C64
		public FakeHeart(EntityData data, Vector2 offset) : this(data.Position + offset)
		{
		}

		// Token: 0x06000EEB RID: 3819 RVA: 0x00039A78 File Offset: 0x00037C78
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			AreaMode areaMode = Calc.Random.Choose(AreaMode.Normal, AreaMode.BSide, AreaMode.CSide);
			base.Add(this.sprite = GFX.SpriteBank.Create("heartgem" + (int)areaMode));
			this.sprite.Play("spin", false, false);
			this.sprite.OnLoop = delegate(string anim)
			{
				if (this.Visible && anim == "spin")
				{
					Audio.Play("event:/game/general/crystalheart_pulse", this.Position);
					this.ScaleWiggler.Start();
					(base.Scene as Level).Displacement.AddBurst(this.Position, 0.35f, 8f, 48f, 0.25f, null, null);
				}
			};
			base.Collider = new Hitbox(16f, 16f, -8f, -8f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(this.ScaleWiggler = Wiggler.Create(0.5f, 4f, delegate(float f)
			{
				this.sprite.Scale = Vector2.One * (1f + f * 0.25f);
			}, false, false));
			base.Add(this.bloom = new BloomPoint(0.75f, 16f));
			Color color;
			if (areaMode == AreaMode.Normal)
			{
				color = Color.Aqua;
				this.shineParticle = HeartGem.P_BlueShine;
			}
			else if (areaMode == AreaMode.BSide)
			{
				color = Color.Red;
				this.shineParticle = HeartGem.P_RedShine;
			}
			else
			{
				color = Color.Gold;
				this.shineParticle = HeartGem.P_GoldShine;
			}
			color = Color.Lerp(color, Color.White, 0.5f);
			base.Add(this.light = new VertexLight(color, 1f, 32, 64));
			this.moveWiggler = Wiggler.Create(0.8f, 2f, null, false, false);
			this.moveWiggler.StartZero = true;
			base.Add(this.moveWiggler);
		}

		// Token: 0x06000EEC RID: 3820 RVA: 0x00039C14 File Offset: 0x00037E14
		public override void Update()
		{
			this.bounceSfxDelay -= Engine.DeltaTime;
			this.timer += Engine.DeltaTime;
			this.sprite.Position = Vector2.UnitY * (float)Math.Sin((double)(this.timer * 2f)) * 2f + this.moveWiggleDir * this.moveWiggler.Value * -8f;
			if (this.respawnTimer > 0f)
			{
				this.respawnTimer -= Engine.DeltaTime;
				if (this.respawnTimer <= 0f)
				{
					this.Collidable = (this.Visible = true);
					this.ScaleWiggler.Start();
				}
			}
			base.Update();
			if (this.Visible && base.Scene.OnInterval(0.1f))
			{
				base.SceneAs<Level>().Particles.Emit(this.shineParticle, 1, base.Center, Vector2.One * 8f);
			}
		}

		// Token: 0x06000EED RID: 3821 RVA: 0x00039D30 File Offset: 0x00037F30
		public void OnHoldable(Holdable h)
		{
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (this.Visible && h.Dangerous(this.crystalCollider))
			{
				this.Collect(entity, h.GetSpeed().Angle());
			}
		}

		// Token: 0x06000EEE RID: 3822 RVA: 0x00039D78 File Offset: 0x00037F78
		public void OnPlayer(Player player)
		{
			if (this.Visible && !(base.Scene as Level).Frozen)
			{
				if (player.DashAttacking)
				{
					Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
					this.Collect(player, player.Speed.Angle());
					return;
				}
				if (this.bounceSfxDelay <= 0f)
				{
					Audio.Play("event:/game/general/crystalheart_bounce", this.Position);
					this.bounceSfxDelay = 0.1f;
				}
				player.PointBounce(base.Center);
				this.moveWiggler.Start();
				this.ScaleWiggler.Start();
				this.moveWiggleDir = (base.Center - player.Center).SafeNormalize(Vector2.UnitY);
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			}
		}

		// Token: 0x06000EEF RID: 3823 RVA: 0x00039E3C File Offset: 0x0003803C
		private void Collect(Player player, float angle)
		{
			if (this.Collidable)
			{
				this.Collidable = (this.Visible = false);
				this.respawnTimer = 3f;
				Celeste.Freeze(0.05f);
				base.SceneAs<Level>().Shake(0.3f);
				SlashFx.Burst(this.Position, angle);
				if (player != null)
				{
					player.RefillDash();
				}
			}
		}

		// Token: 0x04000A33 RID: 2611
		private const float RespawnTime = 3f;

		// Token: 0x04000A34 RID: 2612
		private Sprite sprite;

		// Token: 0x04000A35 RID: 2613
		private ParticleType shineParticle;

		// Token: 0x04000A36 RID: 2614
		public Wiggler ScaleWiggler;

		// Token: 0x04000A37 RID: 2615
		private Wiggler moveWiggler;

		// Token: 0x04000A38 RID: 2616
		private Vector2 moveWiggleDir;

		// Token: 0x04000A39 RID: 2617
		private BloomPoint bloom;

		// Token: 0x04000A3A RID: 2618
		private VertexLight light;

		// Token: 0x04000A3B RID: 2619
		private HoldableCollider crystalCollider;

		// Token: 0x04000A3C RID: 2620
		private float timer;

		// Token: 0x04000A3D RID: 2621
		private float bounceSfxDelay;

		// Token: 0x04000A3E RID: 2622
		private float respawnTimer;
	}
}
