using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000205 RID: 517
	public class PlayerSeeker : Actor
	{
		// Token: 0x060010E3 RID: 4323 RVA: 0x0004FD94 File Offset: 0x0004DF94
		public PlayerSeeker(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Add(this.sprite = GFX.SpriteBank.Create("seeker"));
			this.sprite.Play("statue", false, false);
			this.sprite.OnLastFrame = delegate(string a)
			{
				if (a == "flipMouth" || a == "flipEyes")
				{
					this.facing = -this.facing;
				}
			};
			base.Collider = new Hitbox(10f, 10f, -5f, -5f);
			base.Add(new MirrorReflection());
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(new VertexLight(Color.White, 1f, 32, 64));
			this.facing = Facings.Right;
			base.Add(this.shaker = new Shaker(false, null));
			base.Add(new Coroutine(this.IntroSequence(), true));
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x0004FE85 File Offset: 0x0004E085
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Level level = scene as Level;
			level.Session.ColorGrade = "templevoid";
			level.ScreenPadding = 32f;
			level.CanRetry = false;
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x0004FEB5 File Offset: 0x0004E0B5
		private IEnumerator IntroSequence()
		{
			Level level = base.Scene as Level;
			yield return null;
			Glitch.Value = 0.05f;
			Player entity = level.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				entity.StartTempleMirrorVoidSleep();
			}
			yield return 3f;
			Vector2 from = level.Camera.Position;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 2f, true);
			tween.OnUpdate = delegate(Tween f)
			{
				Vector2 cameraTarget = this.CameraTarget;
				level.Camera.Position = from + (cameraTarget - from) * f.Eased;
			};
			base.Add(tween);
			yield return 2f;
			this.shaker.ShakeFor(0.5f, false);
			this.BreakOutParticles();
			Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
			yield return 1f;
			this.shaker.ShakeFor(0.5f, false);
			this.BreakOutParticles();
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Long);
			yield return 1f;
			this.BreakOutParticles();
			Audio.Play("event:/game/05_mirror_temple/seeker_statue_break", this.Position);
			this.shaker.ShakeFor(1f, false);
			this.sprite.Play("hatch", false, false);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.FullSecond);
			this.enabled = true;
			yield return 0.8f;
			this.BreakOutParticles();
			yield return 0.7f;
			yield break;
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x0004FEC4 File Offset: 0x0004E0C4
		private void BreakOutParticles()
		{
			Level level = base.SceneAs<Level>();
			for (float num = 0f; num < 6.2831855f; num += 0.17453292f)
			{
				Vector2 position = base.Center + Calc.AngleToVector(num + Calc.Random.Range(-0.034906585f, 0.034906585f), (float)Calc.Random.Range(12, 20));
				level.Particles.Emit(Seeker.P_BreakOut, position, num);
			}
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x0004FF38 File Offset: 0x0004E138
		private void OnPlayer(Player player)
		{
			if (!player.Dead)
			{
				Leader.StoreStrawberries(player.Leader);
				PlayerDeadBody playerDeadBody = player.Die((player.Position - this.Position).SafeNormalize(), true, false);
				playerDeadBody.DeathAction = new Action(this.End);
				playerDeadBody.ActionDelay = 0.3f;
				Engine.TimeRate = 0.25f;
			}
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x0004FF9C File Offset: 0x0004E19C
		private void End()
		{
			Level level = base.Scene as Level;
			level.OnEndOfFrame += delegate()
			{
				Glitch.Value = 0f;
				Distort.Anxiety = 0f;
				Engine.TimeRate = 1f;
				level.Session.ColorGrade = null;
				level.UnloadLevel();
				level.CanRetry = true;
				level.Session.Level = "c-00";
				level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float)level.Bounds.Left, (float)level.Bounds.Top)));
				level.LoadLevel(Player.IntroTypes.WakeUp, false);
				Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
			};
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x0004FFD8 File Offset: 0x0004E1D8
		public override void Update()
		{
			foreach (Entity entity in base.Scene.Tracker.GetEntities<SeekerBarrier>())
			{
				entity.Collidable = true;
			}
			Level level = base.Scene as Level;
			base.Update();
			this.sprite.Scale.X = Calc.Approach(this.sprite.Scale.X, 1f, 2f * Engine.DeltaTime);
			this.sprite.Scale.Y = Calc.Approach(this.sprite.Scale.Y, 1f, 2f * Engine.DeltaTime);
			if (this.enabled && this.sprite.CurrentAnimationID != "hatch")
			{
				if (this.dashTimer > 0f)
				{
					this.speed = Calc.Approach(this.speed, Vector2.Zero, 800f * Engine.DeltaTime);
					this.dashTimer -= Engine.DeltaTime;
					if (this.dashTimer <= 0f)
					{
						this.sprite.Play("spotted", false, false);
					}
					if (this.trailTimerA > 0f)
					{
						this.trailTimerA -= Engine.DeltaTime;
						if (this.trailTimerA <= 0f)
						{
							this.CreateTrail();
						}
					}
					if (this.trailTimerB > 0f)
					{
						this.trailTimerB -= Engine.DeltaTime;
						if (this.trailTimerB <= 0f)
						{
							this.CreateTrail();
						}
					}
					if (base.Scene.OnInterval(0.04f))
					{
						Vector2 vector = this.speed.SafeNormalize();
						base.SceneAs<Level>().Particles.Emit(Seeker.P_Attack, 2, this.Position + vector * 4f, Vector2.One * 4f, vector.Angle());
					}
				}
				else
				{
					Vector2 vector2 = Input.Aim.Value.SafeNormalize();
					this.speed += vector2 * 600f * Engine.DeltaTime;
					float num = this.speed.Length();
					if (num > 120f)
					{
						num = Calc.Approach(num, 120f, Engine.DeltaTime * 700f);
						this.speed = this.speed.SafeNormalize(num);
					}
					if (vector2.Y == 0f)
					{
						this.speed.Y = Calc.Approach(this.speed.Y, 0f, 400f * Engine.DeltaTime);
					}
					if (vector2.X == 0f)
					{
						this.speed.X = Calc.Approach(this.speed.X, 0f, 400f * Engine.DeltaTime);
					}
					if (vector2.Length() > 0f && this.sprite.CurrentAnimationID == "idle")
					{
						level.Displacement.AddBurst(this.Position, 0.5f, 8f, 32f, 1f, null, null);
						this.sprite.Play("spotted", false, false);
						Audio.Play("event:/game/05_mirror_temple/seeker_playercontrolstart");
					}
					int num2 = Math.Sign((int)this.facing);
					int num3 = Math.Sign(this.speed.X);
					if (num3 != 0 && num2 != num3 && Math.Sign(Input.Aim.Value.X) == Math.Sign(this.speed.X) && Math.Abs(this.speed.X) > 20f && this.sprite.CurrentAnimationID != "flipMouth" && this.sprite.CurrentAnimationID != "flipEyes")
					{
						this.sprite.Play("flipMouth", false, false);
					}
					if (Input.Dash.Pressed)
					{
						this.Dash(Input.Aim.Value.EightWayNormal());
					}
				}
				base.MoveH(this.speed.X * Engine.DeltaTime, new Collision(this.OnCollide), null);
				base.MoveV(this.speed.Y * Engine.DeltaTime, new Collision(this.OnCollide), null);
				this.Position = this.Position.Clamp((float)level.Bounds.X, (float)level.Bounds.Y, (float)level.Bounds.Right, (float)level.Bounds.Bottom);
				Player entity2 = base.Scene.Tracker.GetEntity<Player>();
				if (entity2 != null)
				{
					float num4 = (this.Position - entity2.Position).Length();
					if (num4 < 200f && entity2.Sprite.CurrentAnimationID == "asleep")
					{
						entity2.Sprite.Rate = 2f;
						entity2.Sprite.Play("wakeUp", false, false);
					}
					else if (num4 < 100f && entity2.Sprite.CurrentAnimationID != "wakeUp")
					{
						entity2.Sprite.Rate = 1f;
						entity2.Sprite.Play("runFast", false, false);
						entity2.Facing = ((base.X > entity2.X) ? Facings.Left : Facings.Right);
					}
					if (num4 < 50f && this.dashTimer <= 0f)
					{
						this.Dash((entity2.Center - base.Center).SafeNormalize());
					}
					Engine.TimeRate = Calc.ClampedMap(num4, 60f, 220f, 0.5f, 1f);
					Camera camera = level.Camera;
					Vector2 cameraTarget = this.CameraTarget;
					camera.Position += (cameraTarget - camera.Position) * (1f - (float)Math.Pow(0.009999999776482582, (double)Engine.DeltaTime));
					Distort.Anxiety = Calc.ClampedMap(num4, 0f, 200f, 0.25f, 0f) + Calc.Random.NextFloat(0.05f);
					Distort.AnxietyOrigin = (new Vector2(entity2.X, level.Camera.Top) - level.Camera.Position) / new Vector2(320f, 180f);
				}
				else
				{
					Engine.TimeRate = Calc.Approach(Engine.TimeRate, 1f, 1f * Engine.DeltaTime);
				}
			}
			foreach (Entity entity3 in base.Scene.Tracker.GetEntities<SeekerBarrier>())
			{
				entity3.Collidable = false;
			}
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x00050704 File Offset: 0x0004E904
		private void CreateTrail()
		{
			Vector2 scale = this.sprite.Scale;
			Sprite sprite = this.sprite;
			sprite.Scale.X = sprite.Scale.X * (float)this.facing;
			TrailManager.Add(this, Seeker.TrailColor, 1f, false, false);
			this.sprite.Scale = scale;
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x00050758 File Offset: 0x0004E958
		private void OnCollide(CollisionData data)
		{
			if (this.dashTimer <= 0f)
			{
				if (data.Direction.X != 0f)
				{
					this.speed.X = 0f;
				}
				if (data.Direction.Y != 0f)
				{
					this.speed.Y = 0f;
					return;
				}
			}
			else
			{
				float direction;
				Vector2 position;
				Vector2 positionRange;
				if (data.Direction.X > 0f)
				{
					direction = 3.1415927f;
					position = new Vector2(base.Right, base.Y);
					positionRange = Vector2.UnitY * 4f;
				}
				else if (data.Direction.X < 0f)
				{
					direction = 0f;
					position = new Vector2(base.Left, base.Y);
					positionRange = Vector2.UnitY * 4f;
				}
				else if (data.Direction.Y > 0f)
				{
					direction = -1.5707964f;
					position = new Vector2(base.X, base.Bottom);
					positionRange = Vector2.UnitX * 4f;
				}
				else
				{
					direction = 1.5707964f;
					position = new Vector2(base.X, base.Top);
					positionRange = Vector2.UnitX * 4f;
				}
				base.SceneAs<Level>().Particles.Emit(Seeker.P_HitWall, 12, position, positionRange, direction);
				if (data.Hit is SeekerBarrier)
				{
					(data.Hit as SeekerBarrier).OnReflectSeeker();
					Audio.Play("event:/game/05_mirror_temple/seeker_hit_lightwall", this.Position);
				}
				else
				{
					Audio.Play("event:/game/05_mirror_temple/seeker_hit_normal", this.Position);
				}
				if (data.Direction.X != 0f)
				{
					this.speed.X = this.speed.X * -0.8f;
					this.sprite.Scale = new Vector2(0.6f, 1.4f);
				}
				else if (data.Direction.Y != 0f)
				{
					this.speed.Y = this.speed.Y * -0.8f;
					this.sprite.Scale = new Vector2(1.4f, 0.6f);
				}
				if (data.Hit is TempleCrackedBlock)
				{
					Celeste.Freeze(0.15f);
					Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
					(data.Hit as TempleCrackedBlock).Break(this.Position);
				}
			}
		}

		// Token: 0x060010EC RID: 4332 RVA: 0x000509B0 File Offset: 0x0004EBB0
		private void Dash(Vector2 dir)
		{
			if (this.dashTimer <= 0f)
			{
				this.CreateTrail();
				this.trailTimerA = 0.1f;
				this.trailTimerB = 0.25f;
			}
			this.dashTimer = 0.3f;
			this.dashDirection = dir;
			if (this.dashDirection == Vector2.Zero)
			{
				this.dashDirection.X = (float)Math.Sign((int)this.facing);
			}
			if (this.dashDirection.X != 0f)
			{
				this.facing = (Facings)Math.Sign(this.dashDirection.X);
			}
			this.speed = this.dashDirection * 400f;
			this.sprite.Play("attacking", false, false);
			base.SceneAs<Level>().DirectionalShake(this.dashDirection, 0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			Audio.Play("event:/game/05_mirror_temple/seeker_dash", this.Position);
			if (this.dashDirection.X == 0f)
			{
				this.sprite.Scale = new Vector2(0.6f, 1.4f);
				return;
			}
			this.sprite.Scale = new Vector2(1.4f, 0.6f);
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x060010ED RID: 4333 RVA: 0x00050AE8 File Offset: 0x0004ECE8
		public Vector2 CameraTarget
		{
			get
			{
				Rectangle bounds = (base.Scene as Level).Bounds;
				return (this.Position + new Vector2(-160f, -90f)).Clamp((float)bounds.Left, (float)bounds.Top, (float)(bounds.Right - 320), (float)(bounds.Bottom - 180));
			}
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x00050B54 File Offset: 0x0004ED54
		public override void Render()
		{
			if (SaveData.Instance.Assists.InvisibleMotion && this.enabled && this.speed.LengthSquared() > 100f)
			{
				return;
			}
			Vector2 position = this.Position;
			this.Position += this.shaker.Value;
			Vector2 scale = this.sprite.Scale;
			Sprite sprite = this.sprite;
			sprite.Scale.X = sprite.Scale.X * (float)this.facing;
			base.Render();
			this.Position = position;
			this.sprite.Scale = scale;
		}

		// Token: 0x04000C87 RID: 3207
		private Facings facing;

		// Token: 0x04000C88 RID: 3208
		private Sprite sprite;

		// Token: 0x04000C89 RID: 3209
		private Vector2 speed;

		// Token: 0x04000C8A RID: 3210
		private bool enabled;

		// Token: 0x04000C8B RID: 3211
		private float dashTimer;

		// Token: 0x04000C8C RID: 3212
		private Vector2 dashDirection;

		// Token: 0x04000C8D RID: 3213
		private float trailTimerA;

		// Token: 0x04000C8E RID: 3214
		private float trailTimerB;

		// Token: 0x04000C8F RID: 3215
		private Shaker shaker;
	}
}
