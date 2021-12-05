using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002AF RID: 687
	public class Booster : Entity
	{
		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06001532 RID: 5426 RVA: 0x00079478 File Offset: 0x00077678
		// (set) Token: 0x06001533 RID: 5427 RVA: 0x00079480 File Offset: 0x00077680
		public bool BoostingPlayer { get; private set; }

		// Token: 0x06001534 RID: 5428 RVA: 0x0007948C File Offset: 0x0007768C
		public Booster(Vector2 position, bool red) : base(position)
		{
			base.Depth = -8500;
			base.Collider = new Circle(10f, 0f, 2f);
			this.red = red;
			base.Add(this.sprite = GFX.SpriteBank.Create(red ? "boosterRed" : "booster"));
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(this.light = new VertexLight(Color.White, 1f, 16, 32));
			base.Add(this.bloom = new BloomPoint(0.1f, 16f));
			base.Add(this.wiggler = Wiggler.Create(0.5f, 4f, delegate(float f)
			{
				this.sprite.Scale = Vector2.One * (1f + f * 0.25f);
			}, false, false));
			base.Add(this.dashRoutine = new Coroutine(false));
			base.Add(this.dashListener = new DashListener());
			base.Add(new MirrorReflection());
			base.Add(this.loopingSfx = new SoundSource());
			this.dashListener.OnDash = new Action<Vector2>(this.OnPlayerDashed);
			this.particleType = (red ? Booster.P_BurstRed : Booster.P_Burst);
		}

		// Token: 0x06001535 RID: 5429 RVA: 0x000795F3 File Offset: 0x000777F3
		public Booster(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("red", false))
		{
			this.Ch9HubBooster = data.Bool("ch9_hub_booster", false);
		}

		// Token: 0x06001536 RID: 5430 RVA: 0x00079628 File Offset: 0x00077828
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Image image = new Image(GFX.Game["objects/booster/outline"]);
			image.CenterOrigin();
			image.Color = Color.White * 0.75f;
			this.outline = new Entity(this.Position);
			this.outline.Depth = 8999;
			this.outline.Visible = false;
			this.outline.Add(image);
			this.outline.Add(new MirrorReflection());
			scene.Add(this.outline);
		}

		// Token: 0x06001537 RID: 5431 RVA: 0x000796C4 File Offset: 0x000778C4
		public void Appear()
		{
			Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_reappear" : "event:/game/04_cliffside/greenbooster_reappear", this.Position);
			this.sprite.Play("appear", false, false);
			this.wiggler.Start();
			this.Visible = true;
			this.AppearParticles();
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x0007971C File Offset: 0x0007791C
		private void AppearParticles()
		{
			ParticleSystem particlesBG = base.SceneAs<Level>().ParticlesBG;
			for (int i = 0; i < 360; i += 30)
			{
				particlesBG.Emit(this.red ? Booster.P_RedAppear : Booster.P_Appear, 1, base.Center, Vector2.One * 2f, (float)i * 0.017453292f);
			}
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x00079780 File Offset: 0x00077980
		private void OnPlayer(Player player)
		{
			if (this.respawnTimer <= 0f && this.cannotUseTimer <= 0f && !this.BoostingPlayer)
			{
				this.cannotUseTimer = 0.45f;
				if (this.red)
				{
					player.RedBoost(this);
				}
				else
				{
					player.Boost(this);
				}
				Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_enter" : "event:/game/04_cliffside/greenbooster_enter", this.Position);
				this.wiggler.Start();
				this.sprite.Play("inside", false, false);
				this.sprite.FlipX = (player.Facing == Facings.Left);
			}
		}

		// Token: 0x0600153A RID: 5434 RVA: 0x00079828 File Offset: 0x00077A28
		public void PlayerBoosted(Player player, Vector2 direction)
		{
			Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_dash" : "event:/game/04_cliffside/greenbooster_dash", this.Position);
			if (this.red)
			{
				this.loopingSfx.Play("event:/game/05_mirror_temple/redbooster_move", null, 0f);
				this.loopingSfx.DisposeOnTransition = false;
			}
			if (this.Ch9HubBooster && direction.Y < 0f)
			{
				bool flag = true;
				List<LockBlock> list = base.Scene.Entities.FindAll<LockBlock>();
				if (list.Count > 0)
				{
					using (List<LockBlock>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (!enumerator.Current.UnlockingRegistered)
							{
								flag = false;
								break;
							}
						}
					}
				}
				if (flag)
				{
					this.Ch9HubTransition = true;
					base.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate
					{
						base.Add(new SoundSource("event:/new_content/timeline_bubble_to_remembered")
						{
							DisposeOnTransition = false
						});
					}, 2f, true));
				}
			}
			this.BoostingPlayer = true;
			base.Tag = (Tags.Persistent | Tags.TransitionUpdate);
			this.sprite.Play("spin", false, false);
			this.sprite.FlipX = (player.Facing == Facings.Left);
			this.outline.Visible = true;
			this.wiggler.Start();
			this.dashRoutine.Replace(this.BoostRoutine(player, direction));
		}

		// Token: 0x0600153B RID: 5435 RVA: 0x00079994 File Offset: 0x00077B94
		private IEnumerator BoostRoutine(Player player, Vector2 dir)
		{
			float angle = (-dir).Angle();
			while ((player.StateMachine.State == 2 || player.StateMachine.State == 5) && this.BoostingPlayer)
			{
				this.sprite.RenderPosition = player.Center + Booster.playerOffset;
				this.loopingSfx.Position = this.sprite.Position;
				if (base.Scene.OnInterval(0.02f))
				{
					(base.Scene as Level).ParticlesBG.Emit(this.particleType, 2, player.Center - dir * 3f + new Vector2(0f, -2f), new Vector2(3f, 3f), angle);
				}
				yield return null;
			}
			this.PlayerReleased();
			if (player.StateMachine.State == 4)
			{
				this.sprite.Visible = false;
			}
			while (base.SceneAs<Level>().Transitioning)
			{
				yield return null;
			}
			base.Tag = 0;
			yield break;
		}

		// Token: 0x0600153C RID: 5436 RVA: 0x000799B1 File Offset: 0x00077BB1
		public void OnPlayerDashed(Vector2 direction)
		{
			if (this.BoostingPlayer)
			{
				this.BoostingPlayer = false;
			}
		}

		// Token: 0x0600153D RID: 5437 RVA: 0x000799C4 File Offset: 0x00077BC4
		public void PlayerReleased()
		{
			Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_end" : "event:/game/04_cliffside/greenbooster_end", this.sprite.RenderPosition);
			this.sprite.Play("pop", false, false);
			this.cannotUseTimer = 0f;
			this.respawnTimer = 1f;
			this.BoostingPlayer = false;
			this.wiggler.Stop();
			this.loopingSfx.Stop(true);
		}

		// Token: 0x0600153E RID: 5438 RVA: 0x00079A3D File Offset: 0x00077C3D
		public void PlayerDied()
		{
			if (this.BoostingPlayer)
			{
				this.PlayerReleased();
				this.dashRoutine.Active = false;
				base.Tag = 0;
			}
		}

		// Token: 0x0600153F RID: 5439 RVA: 0x00079A60 File Offset: 0x00077C60
		public void Respawn()
		{
			Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_reappear" : "event:/game/04_cliffside/greenbooster_reappear", this.Position);
			this.sprite.Position = Vector2.Zero;
			this.sprite.Play("loop", true, false);
			this.wiggler.Start();
			this.sprite.Visible = true;
			this.outline.Visible = false;
			this.AppearParticles();
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x00079AD8 File Offset: 0x00077CD8
		public override void Update()
		{
			base.Update();
			if (this.cannotUseTimer > 0f)
			{
				this.cannotUseTimer -= Engine.DeltaTime;
			}
			if (this.respawnTimer > 0f)
			{
				this.respawnTimer -= Engine.DeltaTime;
				if (this.respawnTimer <= 0f)
				{
					this.Respawn();
				}
			}
			if (!this.dashRoutine.Active && this.respawnTimer <= 0f)
			{
				Vector2 target = Vector2.Zero;
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && base.CollideCheck(entity))
				{
					target = entity.Center + Booster.playerOffset - this.Position;
				}
				this.sprite.Position = Calc.Approach(this.sprite.Position, target, 80f * Engine.DeltaTime);
			}
			if (this.sprite.CurrentAnimationID == "inside" && !this.BoostingPlayer && !base.CollideCheck<Player>())
			{
				this.sprite.Play("loop", false, false);
			}
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x00079BF8 File Offset: 0x00077DF8
		public override void Render()
		{
			Vector2 position = this.sprite.Position;
			this.sprite.Position = position.Floor();
			if (this.sprite.CurrentAnimationID != "pop" && this.sprite.Visible)
			{
				this.sprite.DrawOutline(1);
			}
			base.Render();
			this.sprite.Position = position;
		}

		// Token: 0x06001542 RID: 5442 RVA: 0x00079C64 File Offset: 0x00077E64
		public override void Removed(Scene scene)
		{
			if (this.Ch9HubTransition)
			{
				Level level = scene as Level;
				foreach (Backdrop backdrop in level.Background.GetEach<Backdrop>("bright"))
				{
					backdrop.ForceVisible = false;
					backdrop.FadeAlphaMultiplier = 1f;
				}
				level.Bloom.Base = AreaData.Get(level).BloomBase + 0.25f;
				level.Session.BloomBaseAdd = 0.25f;
			}
			base.Removed(scene);
		}

		// Token: 0x0400112E RID: 4398
		private const float RespawnTime = 1f;

		// Token: 0x0400112F RID: 4399
		public static ParticleType P_Burst;

		// Token: 0x04001130 RID: 4400
		public static ParticleType P_BurstRed;

		// Token: 0x04001131 RID: 4401
		public static ParticleType P_Appear;

		// Token: 0x04001132 RID: 4402
		public static ParticleType P_RedAppear;

		// Token: 0x04001133 RID: 4403
		public static readonly Vector2 playerOffset = new Vector2(0f, -2f);

		// Token: 0x04001134 RID: 4404
		private Sprite sprite;

		// Token: 0x04001135 RID: 4405
		private Entity outline;

		// Token: 0x04001136 RID: 4406
		private Wiggler wiggler;

		// Token: 0x04001137 RID: 4407
		private BloomPoint bloom;

		// Token: 0x04001138 RID: 4408
		private VertexLight light;

		// Token: 0x04001139 RID: 4409
		private Coroutine dashRoutine;

		// Token: 0x0400113A RID: 4410
		private DashListener dashListener;

		// Token: 0x0400113B RID: 4411
		private ParticleType particleType;

		// Token: 0x0400113D RID: 4413
		private float respawnTimer;

		// Token: 0x0400113E RID: 4414
		private float cannotUseTimer;

		// Token: 0x0400113F RID: 4415
		private bool red;

		// Token: 0x04001140 RID: 4416
		private SoundSource loopingSfx;

		// Token: 0x04001141 RID: 4417
		public bool Ch9HubBooster;

		// Token: 0x04001142 RID: 4418
		public bool Ch9HubTransition;
	}
}
