using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002C0 RID: 704
	[Tracked(false)]
	public class Seeker : Actor
	{
		// Token: 0x060015A8 RID: 5544 RVA: 0x0007CD3C File Offset: 0x0007AF3C
		public Seeker(Vector2 position, Vector2[] patrolPoints) : base(position)
		{
			base.Depth = -200;
			this.patrolPoints = patrolPoints;
			this.lastPosition = position;
			base.Collider = (this.physicsHitbox = new Hitbox(6f, 6f, -3f, -3f));
			this.breakWallsHitbox = new Hitbox(6f, 14f, -3f, -7f);
			this.attackHitbox = new Hitbox(12f, 8f, -6f, -2f);
			this.bounceHitbox = new Hitbox(16f, 6f, -8f, -8f);
			this.pushRadius = new Circle(40f, 0f, 0f);
			this.breakWallsRadius = new Circle(16f, 0f, 0f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnAttackPlayer), this.attackHitbox, null));
			base.Add(new PlayerCollider(new Action<Player>(this.OnBouncePlayer), this.bounceHitbox, null));
			base.Add(this.shaker = new Shaker(false, null));
			base.Add(this.State = new StateMachine(10));
			this.State.SetCallbacks(0, new Func<int>(this.IdleUpdate), new Func<IEnumerator>(this.IdleCoroutine), null, null);
			this.State.SetCallbacks(1, new Func<int>(this.PatrolUpdate), null, new Action(this.PatrolBegin), null);
			this.State.SetCallbacks(2, new Func<int>(this.SpottedUpdate), new Func<IEnumerator>(this.SpottedCoroutine), new Action(this.SpottedBegin), null);
			this.State.SetCallbacks(3, new Func<int>(this.AttackUpdate), new Func<IEnumerator>(this.AttackCoroutine), new Action(this.AttackBegin), null);
			this.State.SetCallbacks(4, new Func<int>(this.StunnedUpdate), new Func<IEnumerator>(this.StunnedCoroutine), null, null);
			this.State.SetCallbacks(5, new Func<int>(this.SkiddingUpdate), new Func<IEnumerator>(this.SkiddingCoroutine), new Action(this.SkiddingBegin), new Action(this.SkiddingEnd));
			this.State.SetCallbacks(6, new Func<int>(this.RegenerateUpdate), new Func<IEnumerator>(this.RegenerateCoroutine), new Action(this.RegenerateBegin), new Action(this.RegenerateEnd));
			this.State.SetCallbacks(7, null, new Func<IEnumerator>(this.ReturnedCoroutine), null, null);
			this.onCollideH = new Collision(this.OnCollideH);
			this.onCollideV = new Collision(this.OnCollideV);
			base.Add(this.idleSineX = new SineWave(0.5f, 0f));
			base.Add(this.idleSineY = new SineWave(0.7f, 0f));
			base.Add(this.Light = new VertexLight(Color.White, 1f, 32, 64));
			base.Add(this.theo = new HoldableCollider(new Action<Holdable>(this.OnHoldable), this.attackHitbox));
			base.Add(new MirrorReflection());
			this.path = new List<Vector2>();
			this.IgnoreJumpThrus = true;
			base.Add(this.sprite = GFX.SpriteBank.Create("seeker"));
			this.sprite.OnLastFrame = delegate(string f)
			{
				if (this.flipAnimations.Contains(f) && this.spriteFacing != this.facing)
				{
					this.spriteFacing = this.facing;
					if (this.nextSprite != null)
					{
						this.sprite.Play(this.nextSprite, false, false);
						this.nextSprite = null;
					}
				}
			};
			this.sprite.OnChange = delegate(string last, string next)
			{
				this.nextSprite = null;
				this.sprite.OnLastFrame(last);
			};
			this.SquishCallback = delegate(CollisionData d)
			{
				if (!this.dead && !base.TrySquishWiggle(d, 3, 3))
				{
					Entity entity = new Entity(this.Position);
					DeathEffect deathEffect = new DeathEffect(Color.HotPink, new Vector2?(base.Center - this.Position));
					deathEffect.OnEnd = delegate()
					{
						entity.RemoveSelf();
					};
					entity.Add(deathEffect);
					entity.Depth = -1000000;
					base.Scene.Add(entity);
					Audio.Play("event:/game/05_mirror_temple/seeker_death", this.Position);
					base.RemoveSelf();
					this.dead = true;
				}
			};
			this.scaleWiggler = Wiggler.Create(0.8f, 2f, null, false, false);
			base.Add(this.scaleWiggler);
			base.Add(this.boopedSfx = new SoundSource());
			base.Add(this.aggroSfx = new SoundSource());
			base.Add(this.reviveSfx = new SoundSource());
		}

		// Token: 0x060015A9 RID: 5545 RVA: 0x0007D1BD File Offset: 0x0007B3BD
		public Seeker(EntityData data, Vector2 offset) : this(data.Position + offset, data.NodesOffset(offset))
		{
		}

		// Token: 0x060015AA RID: 5546 RVA: 0x0007D1D8 File Offset: 0x0007B3D8
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.random = new Random(base.SceneAs<Level>().Session.LevelData.LoadSeed);
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x0007D204 File Offset: 0x0007B404
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity == null || base.X == entity.X)
			{
				this.SnapFacing(1f);
				return;
			}
			this.SnapFacing((float)Math.Sign(entity.X - base.X));
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x00008E00 File Offset: 0x00007000
		public override bool IsRiding(JumpThru jumpThru)
		{
			return false;
		}

		// Token: 0x060015AD RID: 5549 RVA: 0x00008E00 File Offset: 0x00007000
		public override bool IsRiding(Solid solid)
		{
			return false;
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x060015AE RID: 5550 RVA: 0x0007D25F File Offset: 0x0007B45F
		public bool Attacking
		{
			get
			{
				return this.State.State == 3 && !this.attackWindUp;
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x060015AF RID: 5551 RVA: 0x0007D27A File Offset: 0x0007B47A
		public bool Spotted
		{
			get
			{
				return this.State.State == 3 || this.State.State == 2;
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x060015B0 RID: 5552 RVA: 0x0007D29A File Offset: 0x0007B49A
		public bool Regenerating
		{
			get
			{
				return this.State.State == 6;
			}
		}

		// Token: 0x060015B1 RID: 5553 RVA: 0x0007D2AC File Offset: 0x0007B4AC
		private void OnAttackPlayer(Player player)
		{
			if (this.State.State != 4)
			{
				player.Die((player.Center - this.Position).SafeNormalize(), false, true);
				return;
			}
			Collider collider = base.Collider;
			base.Collider = this.bounceHitbox;
			player.PointBounce(base.Center);
			this.Speed = (base.Center - player.Center).SafeNormalize(100f);
			this.scaleWiggler.Start();
			base.Collider = collider;
		}

		// Token: 0x060015B2 RID: 5554 RVA: 0x0007D33C File Offset: 0x0007B53C
		private void OnBouncePlayer(Player player)
		{
			Collider collider = base.Collider;
			base.Collider = this.attackHitbox;
			if (base.CollideCheck(player))
			{
				this.OnAttackPlayer(player);
			}
			else
			{
				player.Bounce(base.Top);
				this.GotBouncedOn(player);
			}
			base.Collider = collider;
		}

		// Token: 0x060015B3 RID: 5555 RVA: 0x0007D388 File Offset: 0x0007B588
		private void GotBouncedOn(Entity entity)
		{
			Celeste.Freeze(0.15f);
			this.Speed = (base.Center - entity.Center).SafeNormalize(200f);
			this.State.State = 6;
			this.sprite.Scale = new Vector2(1.4f, 0.6f);
			base.SceneAs<Level>().Particles.Emit(Seeker.P_Stomp, 8, base.Center - Vector2.UnitY * 5f, new Vector2(6f, 3f));
		}

		// Token: 0x060015B4 RID: 5556 RVA: 0x0007D425 File Offset: 0x0007B625
		public void HitSpring()
		{
			this.Speed.Y = -150f;
		}

		// Token: 0x060015B5 RID: 5557 RVA: 0x0007D438 File Offset: 0x0007B638
		private bool CanSeePlayer(Player player)
		{
			if (player == null)
			{
				return false;
			}
			if (this.State.State != 2 && !base.SceneAs<Level>().InsideCamera(base.Center, 0f) && Vector2.DistanceSquared(base.Center, player.Center) > 25600f)
			{
				return false;
			}
			Vector2 value = (player.Center - base.Center).Perpendicular().SafeNormalize(2f);
			return !base.Scene.CollideCheck<Solid>(base.Center + value, player.Center + value) && !base.Scene.CollideCheck<Solid>(base.Center - value, player.Center - value);
		}

		// Token: 0x060015B6 RID: 5558 RVA: 0x0007D4F8 File Offset: 0x0007B6F8
		public override void Update()
		{
			this.Light.Alpha = Calc.Approach(this.Light.Alpha, 1f, Engine.DeltaTime * 2f);
			foreach (Entity entity in base.Scene.Tracker.GetEntities<SeekerBarrier>())
			{
				entity.Collidable = true;
			}
			this.sprite.Scale.X = Calc.Approach(this.sprite.Scale.X, 1f, 2f * Engine.DeltaTime);
			this.sprite.Scale.Y = Calc.Approach(this.sprite.Scale.Y, 1f, 2f * Engine.DeltaTime);
			if (this.State.State == 6)
			{
				this.canSeePlayer = false;
			}
			else
			{
				Player entity2 = base.Scene.Tracker.GetEntity<Player>();
				this.canSeePlayer = this.CanSeePlayer(entity2);
				if (this.canSeePlayer)
				{
					this.spotted = true;
					this.lastSpottedAt = entity2.Center;
				}
			}
			if (this.lastPathTo != this.lastSpottedAt)
			{
				this.lastPathTo = this.lastSpottedAt;
				this.pathIndex = 0;
				this.lastPathFound = base.SceneAs<Level>().Pathfinder.Find(ref this.path, base.Center, this.FollowTarget, true, false);
			}
			base.Update();
			this.lastPosition = this.Position;
			base.MoveH(this.Speed.X * Engine.DeltaTime, this.onCollideH, null);
			base.MoveV(this.Speed.Y * Engine.DeltaTime, this.onCollideV, null);
			Level level = base.SceneAs<Level>();
			if (base.Left < (float)level.Bounds.Left && this.Speed.X < 0f)
			{
				base.Left = (float)level.Bounds.Left;
				this.onCollideH(CollisionData.Empty);
			}
			else if (base.Right > (float)level.Bounds.Right && this.Speed.X > 0f)
			{
				base.Right = (float)level.Bounds.Right;
				this.onCollideH(CollisionData.Empty);
			}
			if (base.Top < (float)(level.Bounds.Top + -8) && this.Speed.Y < 0f)
			{
				base.Top = (float)(level.Bounds.Top + -8);
				this.onCollideV(CollisionData.Empty);
			}
			else if (base.Bottom > (float)level.Bounds.Bottom && this.Speed.Y > 0f)
			{
				base.Bottom = (float)level.Bounds.Bottom;
				this.onCollideV(CollisionData.Empty);
			}
			foreach (Component component in base.Scene.Tracker.GetComponents<SeekerCollider>())
			{
				((SeekerCollider)component).Check(this);
			}
			if (this.State.State == 3 && this.Speed.X > 0f)
			{
				this.bounceHitbox.Width = 16f;
				this.bounceHitbox.Position.X = -10f;
			}
			else if (this.State.State == 3 && this.Speed.Y < 0f)
			{
				this.bounceHitbox.Width = 16f;
				this.bounceHitbox.Position.X = -6f;
			}
			else
			{
				this.bounceHitbox.Width = 12f;
				this.bounceHitbox.Position.X = -6f;
			}
			foreach (Entity entity3 in base.Scene.Tracker.GetEntities<SeekerBarrier>())
			{
				entity3.Collidable = false;
			}
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x0007D970 File Offset: 0x0007BB70
		private void TurnFacing(float dir, string gotoSprite = null)
		{
			if (dir != 0f)
			{
				this.facing = Math.Sign(dir);
			}
			if (this.spriteFacing != this.facing)
			{
				if (this.State.State == 5)
				{
					this.sprite.Play("skid", false, false);
				}
				else if (this.State.State == 3 || this.State.State == 2)
				{
					this.sprite.Play("flipMouth", false, false);
				}
				else
				{
					this.sprite.Play("flipEyes", false, false);
				}
				this.nextSprite = gotoSprite;
				return;
			}
			if (gotoSprite != null)
			{
				this.sprite.Play(gotoSprite, false, false);
			}
		}

		// Token: 0x060015B8 RID: 5560 RVA: 0x0007DA1C File Offset: 0x0007BC1C
		private void SnapFacing(float dir)
		{
			if (dir != 0f)
			{
				this.spriteFacing = (this.facing = Math.Sign(dir));
			}
		}

		// Token: 0x060015B9 RID: 5561 RVA: 0x0007DA48 File Offset: 0x0007BC48
		private void OnHoldable(Holdable holdable)
		{
			if (this.State.State != 6 && holdable.Dangerous(this.theo))
			{
				holdable.HitSeeker(this);
				this.State.State = 4;
				this.Speed = (base.Center - holdable.Entity.Center).SafeNormalize(120f);
				this.scaleWiggler.Start();
				return;
			}
			if ((this.State.State == 3 || this.State.State == 5) && holdable.IsHeld)
			{
				holdable.Swat(this.theo, Math.Sign(this.Speed.X));
				this.State.State = 4;
				this.Speed = (base.Center - holdable.Entity.Center).SafeNormalize(120f);
				this.scaleWiggler.Start();
			}
		}

		// Token: 0x060015BA RID: 5562 RVA: 0x0007DB34 File Offset: 0x0007BD34
		public override void Render()
		{
			Vector2 position = this.Position;
			this.Position += this.shaker.Value;
			Vector2 scale = this.sprite.Scale;
			this.sprite.Scale *= 1f - 0.3f * this.scaleWiggler.Value;
			Sprite sprite = this.sprite;
			sprite.Scale.X = sprite.Scale.X * (float)this.spriteFacing;
			base.Render();
			this.Position = position;
			this.sprite.Scale = scale;
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x0007DBD4 File Offset: 0x0007BDD4
		public override void DebugRender(Camera camera)
		{
			Collider collider = base.Collider;
			base.Collider = this.attackHitbox;
			this.attackHitbox.Render(camera, Color.Red);
			base.Collider = this.bounceHitbox;
			this.bounceHitbox.Render(camera, Color.Aqua);
			base.Collider = collider;
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x0007DC2C File Offset: 0x0007BE2C
		private void SlammedIntoWall(CollisionData data)
		{
			float direction;
			float x;
			if (data.Direction.X > 0f)
			{
				direction = 3.1415927f;
				x = base.Right;
			}
			else
			{
				direction = 0f;
				x = base.Left;
			}
			base.SceneAs<Level>().Particles.Emit(Seeker.P_HitWall, 12, new Vector2(x, base.Y), Vector2.UnitY * 4f, direction);
			if (data.Hit is DashSwitch)
			{
				(data.Hit as DashSwitch).OnDashCollide(null, Vector2.UnitX * (float)Math.Sign(this.Speed.X));
			}
			base.Collider = this.breakWallsHitbox;
			foreach (Entity entity in base.Scene.Tracker.GetEntities<TempleCrackedBlock>())
			{
				TempleCrackedBlock templeCrackedBlock = (TempleCrackedBlock)entity;
				if (base.CollideCheck(templeCrackedBlock, this.Position + Vector2.UnitX * (float)Math.Sign(this.Speed.X)))
				{
					templeCrackedBlock.Break(base.Center);
				}
			}
			base.Collider = this.physicsHitbox;
			base.SceneAs<Level>().DirectionalShake(Vector2.UnitX * (float)Math.Sign(this.Speed.X), 0.3f);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			this.Speed.X = (float)Math.Sign(this.Speed.X) * -100f;
			this.Speed.Y = this.Speed.Y * 0.4f;
			this.sprite.Scale.X = 0.6f;
			this.sprite.Scale.Y = 1.4f;
			this.shaker.ShakeFor(0.5f, false);
			this.scaleWiggler.Start();
			this.State.State = 4;
			if (data.Hit is SeekerBarrier)
			{
				(data.Hit as SeekerBarrier).OnReflectSeeker();
				Audio.Play("event:/game/05_mirror_temple/seeker_hit_lightwall", this.Position);
				return;
			}
			Audio.Play("event:/game/05_mirror_temple/seeker_hit_normal", this.Position);
		}

		// Token: 0x060015BD RID: 5565 RVA: 0x0007DE78 File Offset: 0x0007C078
		private void OnCollideH(CollisionData data)
		{
			if (this.State.State == 3 && data.Hit != null)
			{
				int num = Math.Sign(this.Speed.X);
				if (!base.CollideCheck<Solid>(this.Position + new Vector2((float)num, 4f)) && !base.MoveVExact(4, null, null))
				{
					return;
				}
				if (!base.CollideCheck<Solid>(this.Position + new Vector2((float)num, -4f)) && !base.MoveVExact(-4, null, null))
				{
					return;
				}
			}
			if ((this.State.State == 3 || this.State.State == 5) && Math.Abs(this.Speed.X) >= 100f)
			{
				this.SlammedIntoWall(data);
				return;
			}
			this.Speed.X = this.Speed.X * -0.2f;
		}

		// Token: 0x060015BE RID: 5566 RVA: 0x0007DF52 File Offset: 0x0007C152
		private void OnCollideV(CollisionData data)
		{
			if (this.State.State == 3)
			{
				this.Speed.Y = this.Speed.Y * -0.6f;
				return;
			}
			this.Speed.Y = this.Speed.Y * -0.2f;
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x060015BF RID: 5567 RVA: 0x0007DF8B File Offset: 0x0007C18B
		private Vector2 FollowTarget
		{
			get
			{
				return this.lastSpottedAt - Vector2.UnitY * 2f;
			}
		}

		// Token: 0x060015C0 RID: 5568 RVA: 0x0007DFA8 File Offset: 0x0007C1A8
		private void CreateTrail()
		{
			Vector2 scale = this.sprite.Scale;
			this.sprite.Scale *= 1f - 0.3f * this.scaleWiggler.Value;
			Sprite sprite = this.sprite;
			sprite.Scale.X = sprite.Scale.X * (float)this.spriteFacing;
			TrailManager.Add(this, Seeker.TrailColor, 0.5f, false, false);
			this.sprite.Scale = scale;
		}

		// Token: 0x060015C1 RID: 5569 RVA: 0x0007E028 File Offset: 0x0007C228
		private int IdleUpdate()
		{
			if (this.canSeePlayer)
			{
				return 2;
			}
			Vector2 vector = Vector2.Zero;
			if (this.spotted && Vector2.DistanceSquared(base.Center, this.FollowTarget) > 64f)
			{
				float speedMagnitude = this.GetSpeedMagnitude(50f);
				if (this.lastPathFound)
				{
					vector = this.GetPathSpeed(speedMagnitude);
				}
				else
				{
					vector = (this.FollowTarget - base.Center).SafeNormalize(speedMagnitude);
				}
			}
			if (vector == Vector2.Zero)
			{
				vector.X = this.idleSineX.Value * 6f;
				vector.Y = this.idleSineY.Value * 6f;
			}
			this.Speed = Calc.Approach(this.Speed, vector, 200f * Engine.DeltaTime);
			if (this.Speed.LengthSquared() > 400f)
			{
				this.TurnFacing(this.Speed.X, null);
			}
			if (this.spriteFacing == this.facing)
			{
				this.sprite.Play("idle", false, false);
			}
			return 0;
		}

		// Token: 0x060015C2 RID: 5570 RVA: 0x0007E13A File Offset: 0x0007C33A
		private IEnumerator IdleCoroutine()
		{
			if (this.patrolPoints != null && this.patrolPoints.Length != 0 && this.spotted)
			{
				while (Vector2.DistanceSquared(base.Center, this.FollowTarget) > 64f)
				{
					yield return null;
				}
				yield return 0.3f;
				this.State.State = 1;
			}
			yield break;
		}

		// Token: 0x060015C3 RID: 5571 RVA: 0x0007E14C File Offset: 0x0007C34C
		private Vector2 GetPathSpeed(float magnitude)
		{
			if (this.pathIndex >= this.path.Count)
			{
				return Vector2.Zero;
			}
			if (Vector2.DistanceSquared(base.Center, this.path[this.pathIndex]) < 36f)
			{
				this.pathIndex++;
				return this.GetPathSpeed(magnitude);
			}
			return (this.path[this.pathIndex] - base.Center).SafeNormalize(magnitude);
		}

		// Token: 0x060015C4 RID: 5572 RVA: 0x0007E1D0 File Offset: 0x0007C3D0
		private float GetSpeedMagnitude(float baseMagnitude)
		{
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity == null)
			{
				return baseMagnitude;
			}
			if (Vector2.DistanceSquared(base.Center, entity.Center) > 12544f)
			{
				return baseMagnitude * 3f;
			}
			return baseMagnitude * 1.5f;
		}

		// Token: 0x060015C5 RID: 5573 RVA: 0x0007E21A File Offset: 0x0007C41A
		private void PatrolBegin()
		{
			this.State.State = this.ChoosePatrolTarget();
			this.patrolWaitTimer = 0f;
		}

		// Token: 0x060015C6 RID: 5574 RVA: 0x0007E238 File Offset: 0x0007C438
		private int PatrolUpdate()
		{
			if (this.canSeePlayer)
			{
				return 2;
			}
			if (this.patrolWaitTimer > 0f)
			{
				this.patrolWaitTimer -= Engine.DeltaTime;
				if (this.patrolWaitTimer <= 0f)
				{
					return this.ChoosePatrolTarget();
				}
			}
			else if (Vector2.DistanceSquared(base.Center, this.lastSpottedAt) < 144f)
			{
				this.patrolWaitTimer = 0.4f;
			}
			float speedMagnitude = this.GetSpeedMagnitude(25f);
			Vector2 target;
			if (this.lastPathFound)
			{
				target = this.GetPathSpeed(speedMagnitude);
			}
			else
			{
				target = (this.FollowTarget - base.Center).SafeNormalize(speedMagnitude);
			}
			this.Speed = Calc.Approach(this.Speed, target, 600f * Engine.DeltaTime);
			if (this.Speed.LengthSquared() > 100f)
			{
				this.TurnFacing(this.Speed.X, null);
			}
			if (this.spriteFacing == this.facing)
			{
				this.sprite.Play("search", false, false);
			}
			return 1;
		}

		// Token: 0x060015C7 RID: 5575 RVA: 0x0007E340 File Offset: 0x0007C540
		private int ChoosePatrolTarget()
		{
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity == null)
			{
				return 0;
			}
			for (int i = 0; i < 3; i++)
			{
				Seeker.patrolChoices[i].Distance = 0f;
			}
			int num = 0;
			foreach (Vector2 vector in this.patrolPoints)
			{
				if (Vector2.DistanceSquared(base.Center, vector) >= 576f)
				{
					float num2 = Vector2.DistanceSquared(vector, entity.Center);
					for (int k = 0; k < 3; k++)
					{
						if (num2 < Seeker.patrolChoices[k].Distance || Seeker.patrolChoices[k].Distance <= 0f)
						{
							num++;
							for (int l = 2; l > k; l--)
							{
								Seeker.patrolChoices[l].Distance = Seeker.patrolChoices[l - 1].Distance;
								Seeker.patrolChoices[l].Point = Seeker.patrolChoices[l - 1].Point;
							}
							Seeker.patrolChoices[k].Distance = num2;
							Seeker.patrolChoices[k].Point = vector;
							break;
						}
					}
				}
			}
			if (num <= 0)
			{
				return 0;
			}
			this.lastSpottedAt = Seeker.patrolChoices[this.random.Next(Math.Min(3, num))].Point;
			this.lastPathTo = this.lastSpottedAt;
			this.pathIndex = 0;
			this.lastPathFound = base.SceneAs<Level>().Pathfinder.Find(ref this.path, base.Center, this.FollowTarget, true, false);
			return 1;
		}

		// Token: 0x060015C8 RID: 5576 RVA: 0x0007E50C File Offset: 0x0007C70C
		private void SpottedBegin()
		{
			this.aggroSfx.Play("event:/game/05_mirror_temple/seeker_aggro", null, 0f);
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				this.TurnFacing(entity.X - base.X, "spot");
			}
			this.spottedLosePlayerTimer = 0.6f;
			this.spottedTurnDelay = 1f;
		}

		// Token: 0x060015C9 RID: 5577 RVA: 0x0007E574 File Offset: 0x0007C774
		private int SpottedUpdate()
		{
			if (!this.canSeePlayer)
			{
				this.spottedLosePlayerTimer -= Engine.DeltaTime;
				if (this.spottedLosePlayerTimer < 0f)
				{
					return 0;
				}
			}
			else
			{
				this.spottedLosePlayerTimer = 0.6f;
			}
			float speedMagnitude = this.GetSpeedMagnitude(60f);
			Vector2 vector;
			if (this.lastPathFound)
			{
				vector = this.GetPathSpeed(speedMagnitude);
			}
			else
			{
				vector = (this.FollowTarget - base.Center).SafeNormalize(speedMagnitude);
			}
			if (Vector2.DistanceSquared(base.Center, this.FollowTarget) < 2500f && base.Y < this.FollowTarget.Y)
			{
				float num = vector.Angle();
				if (base.Y < this.FollowTarget.Y - 2f)
				{
					num = Calc.AngleLerp(num, 1.5707964f, 0.5f);
				}
				else if (base.Y > this.FollowTarget.Y + 2f)
				{
					num = Calc.AngleLerp(num, -1.5707964f, 0.5f);
				}
				vector = Calc.AngleToVector(num, 60f);
				Vector2 value = Vector2.UnitX * (float)Math.Sign(base.X - this.lastSpottedAt.X) * 48f;
				if (Math.Abs(base.X - this.lastSpottedAt.X) < 36f && !base.CollideCheck<Solid>(this.Position + value) && !base.CollideCheck<Solid>(this.lastSpottedAt + value))
				{
					vector.X = (float)(Math.Sign(base.X - this.lastSpottedAt.X) * 60);
				}
			}
			this.Speed = Calc.Approach(this.Speed, vector, 600f * Engine.DeltaTime);
			this.spottedTurnDelay -= Engine.DeltaTime;
			if (this.spottedTurnDelay <= 0f)
			{
				this.TurnFacing(this.Speed.X, "spotted");
			}
			return 2;
		}

		// Token: 0x060015CA RID: 5578 RVA: 0x0007E76D File Offset: 0x0007C96D
		private IEnumerator SpottedCoroutine()
		{
			yield return 0.2f;
			while (!this.CanAttack())
			{
				yield return null;
			}
			this.State.State = 3;
			yield break;
		}

		// Token: 0x060015CB RID: 5579 RVA: 0x0007E77C File Offset: 0x0007C97C
		private bool CanAttack()
		{
			if (Math.Abs(base.Y - this.lastSpottedAt.Y) > 24f)
			{
				return false;
			}
			if (Math.Abs(base.X - this.lastSpottedAt.X) < 16f)
			{
				return false;
			}
			Vector2 value = (this.FollowTarget - base.Center).SafeNormalize();
			return Vector2.Dot(-Vector2.UnitY, value) <= 0.5f && Vector2.Dot(Vector2.UnitY, value) <= 0.5f && !base.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float)Math.Sign(this.lastSpottedAt.X - base.X) * 24f);
		}

		// Token: 0x060015CC RID: 5580 RVA: 0x0007E850 File Offset: 0x0007CA50
		private void AttackBegin()
		{
			Audio.Play("event:/game/05_mirror_temple/seeker_dash", this.Position);
			this.attackWindUp = true;
			this.attackSpeed = -60f;
			this.Speed = (this.FollowTarget - base.Center).SafeNormalize(-60f);
		}

		// Token: 0x060015CD RID: 5581 RVA: 0x0007E8A4 File Offset: 0x0007CAA4
		private int AttackUpdate()
		{
			if (!this.attackWindUp)
			{
				Vector2 vector = (this.FollowTarget - base.Center).SafeNormalize();
				if (Vector2.Dot(this.Speed.SafeNormalize(), vector) < 0.4f)
				{
					return 5;
				}
				this.attackSpeed = Calc.Approach(this.attackSpeed, 260f, 300f * Engine.DeltaTime);
				this.Speed = this.Speed.RotateTowards(vector.Angle(), 0.61086524f * Engine.DeltaTime).SafeNormalize(this.attackSpeed);
				if (base.Scene.OnInterval(0.04f))
				{
					Vector2 vector2 = (-this.Speed).SafeNormalize();
					base.SceneAs<Level>().Particles.Emit(Seeker.P_Attack, 2, this.Position + vector2 * 4f, Vector2.One * 4f, vector2.Angle());
				}
				if (base.Scene.OnInterval(0.06f))
				{
					this.CreateTrail();
				}
			}
			return 3;
		}

		// Token: 0x060015CE RID: 5582 RVA: 0x0007E9B8 File Offset: 0x0007CBB8
		private IEnumerator AttackCoroutine()
		{
			this.TurnFacing(this.lastSpottedAt.X - base.X, "windUp");
			yield return 0.3f;
			this.attackWindUp = false;
			this.attackSpeed = 180f;
			this.Speed = (this.lastSpottedAt - Vector2.UnitY * 2f - base.Center).SafeNormalize(180f);
			this.SnapFacing(this.Speed.X);
			yield break;
		}

		// Token: 0x060015CF RID: 5583 RVA: 0x0007E9C7 File Offset: 0x0007CBC7
		private int StunnedUpdate()
		{
			this.Speed = Calc.Approach(this.Speed, Vector2.Zero, 150f * Engine.DeltaTime);
			return 4;
		}

		// Token: 0x060015D0 RID: 5584 RVA: 0x0007E9EB File Offset: 0x0007CBEB
		private IEnumerator StunnedCoroutine()
		{
			yield return 0.8f;
			this.State.State = 0;
			yield break;
		}

		// Token: 0x060015D1 RID: 5585 RVA: 0x0007E9FA File Offset: 0x0007CBFA
		private void SkiddingBegin()
		{
			Audio.Play("event:/game/05_mirror_temple/seeker_dash_turn", this.Position);
			this.strongSkid = false;
			this.TurnFacing((float)(-(float)this.facing), null);
		}

		// Token: 0x060015D2 RID: 5586 RVA: 0x0007EA24 File Offset: 0x0007CC24
		private int SkiddingUpdate()
		{
			this.Speed = Calc.Approach(this.Speed, Vector2.Zero, (this.strongSkid ? 400f : 200f) * Engine.DeltaTime);
			if (this.Speed.LengthSquared() >= 400f)
			{
				return 5;
			}
			if (this.canSeePlayer)
			{
				return 2;
			}
			return 0;
		}

		// Token: 0x060015D3 RID: 5587 RVA: 0x0007EA80 File Offset: 0x0007CC80
		private IEnumerator SkiddingCoroutine()
		{
			yield return 0.08f;
			this.strongSkid = true;
			yield break;
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x0007EA8F File Offset: 0x0007CC8F
		private void SkiddingEnd()
		{
			this.spriteFacing = this.facing;
		}

		// Token: 0x060015D5 RID: 5589 RVA: 0x0007EAA0 File Offset: 0x0007CCA0
		private void RegenerateBegin()
		{
			Audio.Play("event:/game/general/thing_booped", this.Position);
			this.boopedSfx.Play("event:/game/05_mirror_temple/seeker_booped", null, 0f);
			this.sprite.Play("takeHit", false, false);
			this.Collidable = false;
			this.State.Locked = true;
			this.Light.StartRadius = 16f;
			this.Light.EndRadius = 32f;
		}

		// Token: 0x060015D6 RID: 5590 RVA: 0x0007EB1A File Offset: 0x0007CD1A
		private void RegenerateEnd()
		{
			this.reviveSfx.Play("event:/game/05_mirror_temple/seeker_revive", null, 0f);
			this.Collidable = true;
			this.Light.StartRadius = 32f;
			this.Light.EndRadius = 64f;
		}

		// Token: 0x060015D7 RID: 5591 RVA: 0x0007EB5C File Offset: 0x0007CD5C
		private int RegenerateUpdate()
		{
			this.Speed.X = Calc.Approach(this.Speed.X, 0f, 150f * Engine.DeltaTime);
			this.Speed = Calc.Approach(this.Speed, Vector2.Zero, 150f * Engine.DeltaTime);
			return 6;
		}

		// Token: 0x060015D8 RID: 5592 RVA: 0x0007EBB6 File Offset: 0x0007CDB6
		private IEnumerator RegenerateCoroutine()
		{
			yield return 1f;
			this.shaker.On = true;
			yield return 0.2f;
			this.sprite.Play("pulse", false, false);
			yield return 0.5f;
			this.sprite.Play("recover", false, false);
			Seeker.RecoverBlast.Spawn(this.Position);
			yield return 0.15f;
			base.Collider = this.pushRadius;
			Player player = base.CollideFirst<Player>();
			if (player != null && !base.Scene.CollideCheck<Solid>(this.Position, player.Center))
			{
				player.ExplodeLaunch(this.Position, true, false);
			}
			TheoCrystal theoCrystal = base.CollideFirst<TheoCrystal>();
			if (theoCrystal != null && !base.Scene.CollideCheck<Solid>(this.Position, theoCrystal.Center))
			{
				theoCrystal.ExplodeLaunch(this.Position);
			}
			foreach (Entity entity in base.Scene.Tracker.GetEntities<TempleCrackedBlock>())
			{
				TempleCrackedBlock templeCrackedBlock = (TempleCrackedBlock)entity;
				if (base.CollideCheck(templeCrackedBlock))
				{
					templeCrackedBlock.Break(this.Position);
				}
			}
			foreach (Entity entity2 in base.Scene.Tracker.GetEntities<TouchSwitch>())
			{
				TouchSwitch touchSwitch = (TouchSwitch)entity2;
				if (base.CollideCheck(touchSwitch))
				{
					touchSwitch.TurnOn();
				}
			}
			base.Collider = this.physicsHitbox;
			Level level = base.SceneAs<Level>();
			level.Displacement.AddBurst(this.Position, 0.4f, 12f, 36f, 0.5f, null, null);
			level.Displacement.AddBurst(this.Position, 0.4f, 24f, 48f, 0.5f, null, null);
			level.Displacement.AddBurst(this.Position, 0.4f, 36f, 60f, 0.5f, null, null);
			for (float num = 0f; num < 6.2831855f; num += 0.17453292f)
			{
				Vector2 position = base.Center + Calc.AngleToVector(num + Calc.Random.Range(-0.034906585f, 0.034906585f), (float)Calc.Random.Range(12, 18));
				level.Particles.Emit(Seeker.P_Regen, position, num);
			}
			this.shaker.On = false;
			this.State.Locked = false;
			this.State.State = 7;
			yield break;
		}

		// Token: 0x060015D9 RID: 5593 RVA: 0x0007EBC5 File Offset: 0x0007CDC5
		private IEnumerator ReturnedCoroutine()
		{
			yield return 0.3f;
			this.State.State = 0;
			yield break;
		}

		// Token: 0x040011C9 RID: 4553
		public static ParticleType P_Attack;

		// Token: 0x040011CA RID: 4554
		public static ParticleType P_HitWall;

		// Token: 0x040011CB RID: 4555
		public static ParticleType P_Stomp;

		// Token: 0x040011CC RID: 4556
		public static ParticleType P_Regen;

		// Token: 0x040011CD RID: 4557
		public static ParticleType P_BreakOut;

		// Token: 0x040011CE RID: 4558
		public static readonly Color TrailColor = Calc.HexToColor("99e550");

		// Token: 0x040011CF RID: 4559
		private const int StIdle = 0;

		// Token: 0x040011D0 RID: 4560
		private const int StPatrol = 1;

		// Token: 0x040011D1 RID: 4561
		private const int StSpotted = 2;

		// Token: 0x040011D2 RID: 4562
		private const int StAttack = 3;

		// Token: 0x040011D3 RID: 4563
		private const int StStunned = 4;

		// Token: 0x040011D4 RID: 4564
		private const int StSkidding = 5;

		// Token: 0x040011D5 RID: 4565
		private const int StRegenerate = 6;

		// Token: 0x040011D6 RID: 4566
		private const int StReturned = 7;

		// Token: 0x040011D7 RID: 4567
		private const int size = 12;

		// Token: 0x040011D8 RID: 4568
		private const int bounceWidth = 16;

		// Token: 0x040011D9 RID: 4569
		private const int bounceHeight = 4;

		// Token: 0x040011DA RID: 4570
		private const float Accel = 600f;

		// Token: 0x040011DB RID: 4571
		private const float WallCollideStunThreshold = 100f;

		// Token: 0x040011DC RID: 4572
		private const float StunXSpeed = 100f;

		// Token: 0x040011DD RID: 4573
		private const float BounceSpeed = 200f;

		// Token: 0x040011DE RID: 4574
		private const float SightDistSq = 25600f;

		// Token: 0x040011DF RID: 4575
		private const float ExplodeRadius = 40f;

		// Token: 0x040011E0 RID: 4576
		private Hitbox physicsHitbox;

		// Token: 0x040011E1 RID: 4577
		private Hitbox breakWallsHitbox;

		// Token: 0x040011E2 RID: 4578
		private Hitbox attackHitbox;

		// Token: 0x040011E3 RID: 4579
		private Hitbox bounceHitbox;

		// Token: 0x040011E4 RID: 4580
		private Circle pushRadius;

		// Token: 0x040011E5 RID: 4581
		private Circle breakWallsRadius;

		// Token: 0x040011E6 RID: 4582
		private StateMachine State;

		// Token: 0x040011E7 RID: 4583
		private Vector2 lastSpottedAt;

		// Token: 0x040011E8 RID: 4584
		private Vector2 lastPathTo;

		// Token: 0x040011E9 RID: 4585
		private bool spotted;

		// Token: 0x040011EA RID: 4586
		private bool canSeePlayer;

		// Token: 0x040011EB RID: 4587
		private Collision onCollideH;

		// Token: 0x040011EC RID: 4588
		private Collision onCollideV;

		// Token: 0x040011ED RID: 4589
		private Random random;

		// Token: 0x040011EE RID: 4590
		private Vector2 lastPosition;

		// Token: 0x040011EF RID: 4591
		private Shaker shaker;

		// Token: 0x040011F0 RID: 4592
		private Wiggler scaleWiggler;

		// Token: 0x040011F1 RID: 4593
		private bool lastPathFound;

		// Token: 0x040011F2 RID: 4594
		private List<Vector2> path;

		// Token: 0x040011F3 RID: 4595
		private int pathIndex;

		// Token: 0x040011F4 RID: 4596
		private Vector2[] patrolPoints;

		// Token: 0x040011F5 RID: 4597
		private SineWave idleSineX;

		// Token: 0x040011F6 RID: 4598
		private SineWave idleSineY;

		// Token: 0x040011F7 RID: 4599
		public VertexLight Light;

		// Token: 0x040011F8 RID: 4600
		private bool dead;

		// Token: 0x040011F9 RID: 4601
		private SoundSource boopedSfx;

		// Token: 0x040011FA RID: 4602
		private SoundSource aggroSfx;

		// Token: 0x040011FB RID: 4603
		private SoundSource reviveSfx;

		// Token: 0x040011FC RID: 4604
		private Sprite sprite;

		// Token: 0x040011FD RID: 4605
		private int facing = 1;

		// Token: 0x040011FE RID: 4606
		private int spriteFacing = 1;

		// Token: 0x040011FF RID: 4607
		private string nextSprite;

		// Token: 0x04001200 RID: 4608
		private HoldableCollider theo;

		// Token: 0x04001201 RID: 4609
		private HashSet<string> flipAnimations = new HashSet<string>
		{
			"flipMouth",
			"flipEyes",
			"skid"
		};

		// Token: 0x04001202 RID: 4610
		public Vector2 Speed;

		// Token: 0x04001203 RID: 4611
		private const float FarDistSq = 12544f;

		// Token: 0x04001204 RID: 4612
		private const float IdleAccel = 200f;

		// Token: 0x04001205 RID: 4613
		private const float IdleSpeed = 50f;

		// Token: 0x04001206 RID: 4614
		private const float PatrolSpeed = 25f;

		// Token: 0x04001207 RID: 4615
		private const int PatrolChoices = 3;

		// Token: 0x04001208 RID: 4616
		private const float PatrolWaitTime = 0.4f;

		// Token: 0x04001209 RID: 4617
		private static Seeker.PatrolPoint[] patrolChoices = new Seeker.PatrolPoint[3];

		// Token: 0x0400120A RID: 4618
		private float patrolWaitTimer;

		// Token: 0x0400120B RID: 4619
		private const float SpottedTargetSpeed = 60f;

		// Token: 0x0400120C RID: 4620
		private const float SpottedFarSpeed = 90f;

		// Token: 0x0400120D RID: 4621
		private const float SpottedMaxYDist = 24f;

		// Token: 0x0400120E RID: 4622
		private const float AttackMinXDist = 16f;

		// Token: 0x0400120F RID: 4623
		private const float SpottedLosePlayerTime = 0.6f;

		// Token: 0x04001210 RID: 4624
		private const float SpottedMinAttackTime = 0.2f;

		// Token: 0x04001211 RID: 4625
		private float spottedLosePlayerTimer;

		// Token: 0x04001212 RID: 4626
		private float spottedTurnDelay;

		// Token: 0x04001213 RID: 4627
		private const float AttackWindUpSpeed = -60f;

		// Token: 0x04001214 RID: 4628
		private const float AttackWindUpTime = 0.3f;

		// Token: 0x04001215 RID: 4629
		private const float AttackStartSpeed = 180f;

		// Token: 0x04001216 RID: 4630
		private const float AttackTargetSpeed = 260f;

		// Token: 0x04001217 RID: 4631
		private const float AttackAccel = 300f;

		// Token: 0x04001218 RID: 4632
		private const float DirectionDotThreshold = 0.4f;

		// Token: 0x04001219 RID: 4633
		private const int AttackTargetUpShift = 2;

		// Token: 0x0400121A RID: 4634
		private const float AttackMaxRotateRadians = 0.61086524f;

		// Token: 0x0400121B RID: 4635
		private float attackSpeed;

		// Token: 0x0400121C RID: 4636
		private bool attackWindUp;

		// Token: 0x0400121D RID: 4637
		private const float StunnedAccel = 150f;

		// Token: 0x0400121E RID: 4638
		private const float StunTime = 0.8f;

		// Token: 0x0400121F RID: 4639
		private const float SkiddingAccel = 200f;

		// Token: 0x04001220 RID: 4640
		private const float StrongSkiddingAccel = 400f;

		// Token: 0x04001221 RID: 4641
		private const float StrongSkiddingTime = 0.08f;

		// Token: 0x04001222 RID: 4642
		private bool strongSkid;

		// Token: 0x0200064B RID: 1611
		private struct PatrolPoint
		{
			// Token: 0x04002A02 RID: 10754
			public Vector2 Point;

			// Token: 0x04002A03 RID: 10755
			public float Distance;
		}

		// Token: 0x0200064C RID: 1612
		[Pooled]
		private class RecoverBlast : Entity
		{
			// Token: 0x06002AF3 RID: 10995 RVA: 0x0011296C File Offset: 0x00110B6C
			public override void Added(Scene scene)
			{
				base.Added(scene);
				base.Depth = -199;
				if (this.sprite == null)
				{
					base.Add(this.sprite = GFX.SpriteBank.Create("seekerShockWave"));
					this.sprite.OnLastFrame = delegate(string a)
					{
						base.RemoveSelf();
					};
				}
				this.sprite.Play("shockwave", true, false);
			}

			// Token: 0x06002AF4 RID: 10996 RVA: 0x001129DC File Offset: 0x00110BDC
			public static void Spawn(Vector2 position)
			{
				Seeker.RecoverBlast recoverBlast = Engine.Pooler.Create<Seeker.RecoverBlast>();
				recoverBlast.Position = position;
				Engine.Scene.Add(recoverBlast);
			}

			// Token: 0x04002A04 RID: 10756
			private Sprite sprite;
		}
	}
}
