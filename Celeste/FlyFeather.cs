using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001AA RID: 426
	[Tracked(false)]
	public class FlyFeather : Entity
	{
		// Token: 0x06000EDF RID: 3807 RVA: 0x000394E4 File Offset: 0x000376E4
		public FlyFeather(Vector2 position, bool shielded, bool singleUse) : base(position)
		{
			this.shielded = shielded;
			this.singleUse = singleUse;
			base.Collider = new Hitbox(20f, 20f, -10f, -10f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(this.sprite = GFX.SpriteBank.Create("flyFeather"));
			base.Add(this.wiggler = Wiggler.Create(1f, 4f, delegate(float v)
			{
				this.sprite.Scale = Vector2.One * (1f + v * 0.2f);
			}, false, false));
			base.Add(this.bloom = new BloomPoint(0.5f, 20f));
			base.Add(this.light = new VertexLight(Color.White, 1f, 16, 48));
			base.Add(this.sine = new SineWave(0.6f, 0f).Randomize());
			base.Add(this.outline = new Image(GFX.Game["objects/flyFeather/outline"]));
			this.outline.CenterOrigin();
			this.outline.Visible = false;
			this.shieldRadiusWiggle = Wiggler.Create(0.5f, 4f, null, false, false);
			base.Add(this.shieldRadiusWiggle);
			this.moveWiggle = Wiggler.Create(0.8f, 2f, null, false, false);
			this.moveWiggle.StartZero = true;
			base.Add(this.moveWiggle);
			this.UpdateY();
		}

		// Token: 0x06000EE0 RID: 3808 RVA: 0x00039684 File Offset: 0x00037884
		public FlyFeather(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("shielded", false), data.Bool("singleUse", false))
		{
		}

		// Token: 0x06000EE1 RID: 3809 RVA: 0x000396B0 File Offset: 0x000378B0
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
		}

		// Token: 0x06000EE2 RID: 3810 RVA: 0x000396C8 File Offset: 0x000378C8
		public override void Update()
		{
			base.Update();
			if (this.respawnTimer > 0f)
			{
				this.respawnTimer -= Engine.DeltaTime;
				if (this.respawnTimer <= 0f)
				{
					this.Respawn();
				}
			}
			this.UpdateY();
			this.light.Alpha = Calc.Approach(this.light.Alpha, this.sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
			this.bloom.Alpha = this.light.Alpha * 0.8f;
		}

		// Token: 0x06000EE3 RID: 3811 RVA: 0x00039770 File Offset: 0x00037970
		public override void Render()
		{
			base.Render();
			if (this.shielded && this.sprite.Visible)
			{
				Draw.Circle(this.Position + this.sprite.Position, 10f - this.shieldRadiusWiggle.Value * 2f, Color.White, 3);
			}
		}

		// Token: 0x06000EE4 RID: 3812 RVA: 0x000397D0 File Offset: 0x000379D0
		private void Respawn()
		{
			if (!this.Collidable)
			{
				this.outline.Visible = false;
				this.Collidable = true;
				this.sprite.Visible = true;
				this.wiggler.Start();
				Audio.Play("event:/game/06_reflection/feather_reappear", this.Position);
				this.level.ParticlesFG.Emit(FlyFeather.P_Respawn, 16, this.Position, Vector2.One * 2f);
			}
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x0003984C File Offset: 0x00037A4C
		private void UpdateY()
		{
			this.sprite.X = 0f;
			this.sprite.Y = (this.bloom.Y = this.sine.Value * 2f);
			this.sprite.Position += this.moveWiggleDir * this.moveWiggle.Value * -8f;
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x000398CC File Offset: 0x00037ACC
		private void OnPlayer(Player player)
		{
			Vector2 speed = player.Speed;
			if (this.shielded && !player.DashAttacking)
			{
				player.PointBounce(base.Center);
				this.moveWiggle.Start();
				this.shieldRadiusWiggle.Start();
				this.moveWiggleDir = (base.Center - player.Center).SafeNormalize(Vector2.UnitY);
				Audio.Play("event:/game/06_reflection/feather_bubble_bounce", this.Position);
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
				return;
			}
			bool flag = player.StateMachine.State == 19;
			if (player.StartStarFly())
			{
				if (!flag)
				{
					Audio.Play(this.shielded ? "event:/game/06_reflection/feather_bubble_get" : "event:/game/06_reflection/feather_get", this.Position);
				}
				else
				{
					Audio.Play(this.shielded ? "event:/game/06_reflection/feather_bubble_renew" : "event:/game/06_reflection/feather_renew", this.Position);
				}
				this.Collidable = false;
				base.Add(new Coroutine(this.CollectRoutine(player, speed), true));
				if (!this.singleUse)
				{
					this.outline.Visible = true;
					this.respawnTimer = 3f;
				}
			}
		}

		// Token: 0x06000EE7 RID: 3815 RVA: 0x000399E3 File Offset: 0x00037BE3
		private IEnumerator CollectRoutine(Player player, Vector2 playerSpeed)
		{
			this.level.Shake(0.3f);
			this.sprite.Visible = false;
			yield return 0.05f;
			float direction;
			if (playerSpeed != Vector2.Zero)
			{
				direction = playerSpeed.Angle();
			}
			else
			{
				direction = (this.Position - player.Center).Angle();
			}
			this.level.ParticlesFG.Emit(FlyFeather.P_Collect, 10, this.Position, Vector2.One * 6f);
			SlashFx.Burst(this.Position, direction);
			yield break;
		}

		// Token: 0x04000A21 RID: 2593
		public static ParticleType P_Collect;

		// Token: 0x04000A22 RID: 2594
		public static ParticleType P_Boost;

		// Token: 0x04000A23 RID: 2595
		public static ParticleType P_Flying;

		// Token: 0x04000A24 RID: 2596
		public static ParticleType P_Respawn;

		// Token: 0x04000A25 RID: 2597
		private const float RespawnTime = 3f;

		// Token: 0x04000A26 RID: 2598
		private Sprite sprite;

		// Token: 0x04000A27 RID: 2599
		private Image outline;

		// Token: 0x04000A28 RID: 2600
		private Wiggler wiggler;

		// Token: 0x04000A29 RID: 2601
		private BloomPoint bloom;

		// Token: 0x04000A2A RID: 2602
		private VertexLight light;

		// Token: 0x04000A2B RID: 2603
		private Level level;

		// Token: 0x04000A2C RID: 2604
		private SineWave sine;

		// Token: 0x04000A2D RID: 2605
		private bool shielded;

		// Token: 0x04000A2E RID: 2606
		private bool singleUse;

		// Token: 0x04000A2F RID: 2607
		private Wiggler shieldRadiusWiggle;

		// Token: 0x04000A30 RID: 2608
		private Wiggler moveWiggle;

		// Token: 0x04000A31 RID: 2609
		private Vector2 moveWiggleDir;

		// Token: 0x04000A32 RID: 2610
		private float respawnTimer;
	}
}
