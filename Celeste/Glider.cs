using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001AC RID: 428
	public class Glider : Actor
	{
		// Token: 0x06000EF2 RID: 3826 RVA: 0x00039F34 File Offset: 0x00038134
		public Glider(Vector2 position, bool bubble, bool tutorial) : base(position)
		{
			this.bubble = bubble;
			this.tutorial = tutorial;
			this.startPos = this.Position;
			base.Collider = new Hitbox(8f, 10f, -4f, -10f);
			this.onCollideH = new Collision(this.OnCollideH);
			this.onCollideV = new Collision(this.OnCollideV);
			base.Add(this.sprite = GFX.SpriteBank.Create("glider"));
			base.Add(this.wiggler = Wiggler.Create(0.25f, 4f, null, false, false));
			base.Depth = -5;
			base.Add(this.Hold = new Holdable(0.3f));
			this.Hold.PickupCollider = new Hitbox(20f, 22f, -10f, -16f);
			this.Hold.SlowFall = true;
			this.Hold.SlowRun = false;
			this.Hold.OnPickup = new Action(this.OnPickup);
			this.Hold.OnRelease = new Action<Vector2>(this.OnRelease);
			this.Hold.SpeedGetter = (() => this.Speed);
			this.Hold.OnHitSpring = new Func<Spring, bool>(this.HitSpring);
			this.platformSine = new SineWave(0.3f, 0f);
			base.Add(this.platformSine);
			this.fallingSfx = new SoundSource();
			base.Add(this.fallingSfx);
			base.Add(new WindMover(new Action<Vector2>(this.WindMode)));
		}

		// Token: 0x06000EF3 RID: 3827 RVA: 0x0003A0EC File Offset: 0x000382EC
		public Glider(EntityData e, Vector2 offset) : this(e.Position + offset, e.Bool("bubble", false), e.Bool("tutorial", false))
		{
		}

		// Token: 0x06000EF4 RID: 3828 RVA: 0x0003A118 File Offset: 0x00038318
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
			if (this.tutorial)
			{
				this.tutorialGui = new BirdTutorialGui(this, new Vector2(0f, -24f), Dialog.Clean("tutorial_carry", null), new object[]
				{
					Dialog.Clean("tutorial_hold", null),
					BirdTutorialGui.ButtonPrompt.Grab
				});
				this.tutorialGui.Open = true;
				base.Scene.Add(this.tutorialGui);
			}
		}

		// Token: 0x06000EF5 RID: 3829 RVA: 0x0003A1A0 File Offset: 0x000383A0
		public override void Update()
		{
			if (base.Scene.OnInterval(0.05f))
			{
				this.level.Particles.Emit(Glider.P_Glow, 1, base.Center + Vector2.UnitY * -9f, new Vector2(10f, 4f));
			}
			float target;
			if (this.Hold.IsHeld)
			{
				if (this.Hold.Holder.OnGround(1))
				{
					target = Calc.ClampedMap(this.Hold.Holder.Speed.X, -300f, 300f, 0.6981317f, -0.6981317f);
				}
				else
				{
					target = Calc.ClampedMap(this.Hold.Holder.Speed.X, -300f, 300f, 1.0471976f, -1.0471976f);
				}
			}
			else
			{
				target = 0f;
			}
			this.sprite.Rotation = Calc.Approach(this.sprite.Rotation, target, 3.1415927f * Engine.DeltaTime);
			if (this.Hold.IsHeld && !this.Hold.Holder.OnGround(1) && (this.sprite.CurrentAnimationID == "fall" || this.sprite.CurrentAnimationID == "fallLoop"))
			{
				if (!this.fallingSfx.Playing)
				{
					Audio.Play("event:/new_content/game/10_farewell/glider_engage", this.Position);
					this.fallingSfx.Play("event:/new_content/game/10_farewell/glider_movement", null, 0f);
				}
				Vector2 speed = this.Hold.Holder.Speed;
				Vector2 vector = new Vector2(speed.X * 0.5f, (speed.Y < 0f) ? (speed.Y * 2f) : speed.Y);
				float value = Calc.Map(vector.Length(), 0f, 120f, 0f, 0.7f);
				this.fallingSfx.Param("glider_speed", value);
			}
			else
			{
				this.fallingSfx.Stop(true);
			}
			base.Update();
			if (!this.destroyed)
			{
				using (List<Entity>.Enumerator enumerator = base.Scene.Tracker.GetEntities<SeekerBarrier>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Entity entity = enumerator.Current;
						SeekerBarrier seekerBarrier = (SeekerBarrier)entity;
						seekerBarrier.Collidable = true;
						bool flag = base.CollideCheck(seekerBarrier);
						seekerBarrier.Collidable = false;
						if (flag)
						{
							this.destroyed = true;
							this.Collidable = false;
							if (this.Hold.IsHeld)
							{
								Vector2 speed2 = this.Hold.Holder.Speed;
								this.Hold.Holder.Drop();
								this.Speed = speed2 * 0.333f;
								Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
							}
							base.Add(new Coroutine(this.DestroyAnimationRoutine(), true));
							return;
						}
					}
					goto IL_313;
				}
				goto IL_2F1;
				IL_313:
				if (this.Hold.IsHeld)
				{
					this.prevLiftSpeed = Vector2.Zero;
				}
				else if (!this.bubble)
				{
					if (this.highFrictionTimer > 0f)
					{
						this.highFrictionTimer -= Engine.DeltaTime;
					}
					if (base.OnGround(1))
					{
						float target2;
						if (!base.OnGround(this.Position + Vector2.UnitX * 3f, 1))
						{
							target2 = 20f;
						}
						else if (!base.OnGround(this.Position - Vector2.UnitX * 3f, 1))
						{
							target2 = -20f;
						}
						else
						{
							target2 = 0f;
						}
						this.Speed.X = Calc.Approach(this.Speed.X, target2, 800f * Engine.DeltaTime);
						Vector2 liftSpeed = base.LiftSpeed;
						if (liftSpeed == Vector2.Zero && this.prevLiftSpeed != Vector2.Zero)
						{
							this.Speed = this.prevLiftSpeed;
							this.prevLiftSpeed = Vector2.Zero;
							this.Speed.Y = Math.Min(this.Speed.Y * 0.6f, 0f);
							if (this.Speed.X != 0f && this.Speed.Y == 0f)
							{
								this.Speed.Y = -60f;
							}
							if (this.Speed.Y < 0f)
							{
								this.noGravityTimer = 0.15f;
							}
						}
						else
						{
							this.prevLiftSpeed = liftSpeed;
							if (liftSpeed.Y < 0f && this.Speed.Y < 0f)
							{
								this.Speed.Y = 0f;
							}
						}
					}
					else if (this.Hold.ShouldHaveGravity)
					{
						float num = 200f;
						if (this.Speed.Y >= -30f)
						{
							num *= 0.5f;
						}
						float num2;
						if (this.Speed.Y < 0f)
						{
							num2 = 40f;
						}
						else if (this.highFrictionTimer <= 0f)
						{
							num2 = 40f;
						}
						else
						{
							num2 = 10f;
						}
						this.Speed.X = Calc.Approach(this.Speed.X, 0f, num2 * Engine.DeltaTime);
						if (this.noGravityTimer > 0f)
						{
							this.noGravityTimer -= Engine.DeltaTime;
						}
						else if (this.level.Wind.Y < 0f)
						{
							this.Speed.Y = Calc.Approach(this.Speed.Y, 0f, num * Engine.DeltaTime);
						}
						else
						{
							this.Speed.Y = Calc.Approach(this.Speed.Y, 30f, num * Engine.DeltaTime);
						}
					}
					base.MoveH(this.Speed.X * Engine.DeltaTime, this.onCollideH, null);
					base.MoveV(this.Speed.Y * Engine.DeltaTime, this.onCollideV, null);
					if (base.Left < (float)this.level.Bounds.Left)
					{
						base.Left = (float)this.level.Bounds.Left;
						this.OnCollideH(new CollisionData
						{
							Direction = -Vector2.UnitX
						});
					}
					else if (base.Right > (float)this.level.Bounds.Right)
					{
						base.Right = (float)this.level.Bounds.Right;
						this.OnCollideH(new CollisionData
						{
							Direction = Vector2.UnitX
						});
					}
					if (base.Top < (float)this.level.Bounds.Top)
					{
						base.Top = (float)this.level.Bounds.Top;
						this.OnCollideV(new CollisionData
						{
							Direction = -Vector2.UnitY
						});
					}
					else if (base.Top > (float)(this.level.Bounds.Bottom + 16))
					{
						base.RemoveSelf();
						return;
					}
					this.Hold.CheckAgainstColliders();
				}
				else
				{
					this.Position = this.startPos + Vector2.UnitY * this.platformSine.Value * 1f;
				}
				Vector2 one = Vector2.One;
				if (!this.Hold.IsHeld)
				{
					if (this.level.Wind.Y < 0f)
					{
						this.PlayOpen();
					}
					else
					{
						this.sprite.Play("idle", false, false);
					}
				}
				else if (this.Hold.Holder.Speed.Y > 20f || this.level.Wind.Y < 0f)
				{
					if (this.level.OnInterval(0.04f))
					{
						if (this.level.Wind.Y < 0f)
						{
							this.level.ParticlesBG.Emit(Glider.P_GlideUp, 1, this.Position - Vector2.UnitY * 20f, new Vector2(6f, 4f));
						}
						else
						{
							this.level.ParticlesBG.Emit(Glider.P_Glide, 1, this.Position - Vector2.UnitY * 10f, new Vector2(6f, 4f));
						}
					}
					this.PlayOpen();
					if (Input.GliderMoveY.Value > 0)
					{
						one.X = 0.7f;
						one.Y = 1.4f;
					}
					else if (Input.GliderMoveY.Value < 0)
					{
						one.X = 1.2f;
						one.Y = 0.8f;
					}
					Input.Rumble(RumbleStrength.Climb, RumbleLength.Short);
				}
				else
				{
					this.sprite.Play("held", false, false);
				}
				this.sprite.Scale.Y = Calc.Approach(this.sprite.Scale.Y, one.Y, Engine.DeltaTime * 2f);
				this.sprite.Scale.X = Calc.Approach(this.sprite.Scale.X, (float)Math.Sign(this.sprite.Scale.X) * one.X, Engine.DeltaTime * 2f);
				if (this.tutorialGui != null)
				{
					this.tutorialGui.Open = (this.tutorial && !this.Hold.IsHeld && (base.OnGround(4) || this.bubble));
				}
				return;
			}
			IL_2F1:
			this.Position += this.Speed * Engine.DeltaTime;
		}

		// Token: 0x06000EF6 RID: 3830 RVA: 0x0003ABD8 File Offset: 0x00038DD8
		private void PlayOpen()
		{
			if (this.sprite.CurrentAnimationID != "fall" && this.sprite.CurrentAnimationID != "fallLoop")
			{
				this.sprite.Play("fall", false, false);
				this.sprite.Scale = new Vector2(1.5f, 0.6f);
				this.level.Particles.Emit(Glider.P_Expand, 16, base.Center + (Vector2.UnitY * -12f).Rotate(this.sprite.Rotation), new Vector2(8f, 3f), -1.5707964f + this.sprite.Rotation);
				if (this.Hold.IsHeld)
				{
					Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
				}
			}
		}

		// Token: 0x06000EF7 RID: 3831 RVA: 0x0003ACBC File Offset: 0x00038EBC
		public override void Render()
		{
			if (!this.destroyed)
			{
				this.sprite.DrawSimpleOutline();
			}
			base.Render();
			if (this.bubble)
			{
				for (int i = 0; i < 24; i++)
				{
					Draw.Point(this.Position + this.PlatformAdd(i), this.PlatformColor(i));
				}
			}
		}

		// Token: 0x06000EF8 RID: 3832 RVA: 0x0003AD18 File Offset: 0x00038F18
		private void WindMode(Vector2 wind)
		{
			if (!this.Hold.IsHeld)
			{
				if (wind.X != 0f)
				{
					base.MoveH(wind.X * 0.5f, null, null);
				}
				if (wind.Y != 0f)
				{
					base.MoveV(wind.Y, null, null);
				}
			}
		}

		// Token: 0x06000EF9 RID: 3833 RVA: 0x0003AD70 File Offset: 0x00038F70
		private Vector2 PlatformAdd(int num)
		{
			return new Vector2((float)(-12 + num), (float)(-5 + (int)Math.Round(Math.Sin((double)(base.Scene.TimeActive + (float)num * 0.2f)) * 1.7999999523162842)));
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x0003ADAA File Offset: 0x00038FAA
		private Color PlatformColor(int num)
		{
			if (num <= 1 || num >= 22)
			{
				return Color.White * 0.4f;
			}
			return Color.White * 0.8f;
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x0003ADD4 File Offset: 0x00038FD4
		private void OnCollideH(CollisionData data)
		{
			if (data.Hit is DashSwitch)
			{
				(data.Hit as DashSwitch).OnDashCollide(null, Vector2.UnitX * (float)Math.Sign(this.Speed.X));
			}
			if (this.Speed.X < 0f)
			{
				Audio.Play("event:/new_content/game/10_farewell/glider_wallbounce_left", this.Position);
			}
			else
			{
				Audio.Play("event:/new_content/game/10_farewell/glider_wallbounce_right", this.Position);
			}
			this.Speed.X = this.Speed.X * -1f;
			this.sprite.Scale = new Vector2(0.8f, 1.2f);
		}

		// Token: 0x06000EFC RID: 3836 RVA: 0x0003AE84 File Offset: 0x00039084
		private void OnCollideV(CollisionData data)
		{
			if (Math.Abs(this.Speed.Y) > 8f)
			{
				this.sprite.Scale = new Vector2(1.2f, 0.8f);
				Audio.Play("event:/new_content/game/10_farewell/glider_land", this.Position);
			}
			if (this.Speed.Y < 0f)
			{
				this.Speed.Y = this.Speed.Y * -0.5f;
				return;
			}
			this.Speed.Y = 0f;
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x0003AF0C File Offset: 0x0003910C
		private void OnPickup()
		{
			if (this.bubble)
			{
				for (int i = 0; i < 24; i++)
				{
					this.level.Particles.Emit(Glider.P_Platform, this.Position + this.PlatformAdd(i), this.PlatformColor(i));
				}
			}
			this.AllowPushing = false;
			this.Speed = Vector2.Zero;
			base.AddTag(Tags.Persistent);
			this.highFrictionTimer = 0.5f;
			this.bubble = false;
			this.wiggler.Start();
			this.tutorial = false;
		}

		// Token: 0x06000EFE RID: 3838 RVA: 0x0003AFA4 File Offset: 0x000391A4
		private void OnRelease(Vector2 force)
		{
			if (force.X == 0f)
			{
				Audio.Play("event:/new_content/char/madeline/glider_drop", this.Position);
			}
			this.AllowPushing = true;
			base.RemoveTag(Tags.Persistent);
			force.Y *= 0.5f;
			if (force.X != 0f && force.Y == 0f)
			{
				force.Y = -0.4f;
			}
			this.Speed = force * 100f;
			this.wiggler.Start();
		}

		// Token: 0x06000EFF RID: 3839 RVA: 0x0003B038 File Offset: 0x00039238
		protected override void OnSquish(CollisionData data)
		{
			if (!base.TrySquishWiggle(data, 3, 3))
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06000F00 RID: 3840 RVA: 0x0003B04C File Offset: 0x0003924C
		public bool HitSpring(Spring spring)
		{
			if (!this.Hold.IsHeld)
			{
				if (spring.Orientation == Spring.Orientations.Floor && this.Speed.Y >= 0f)
				{
					this.Speed.X = this.Speed.X * 0.5f;
					this.Speed.Y = -160f;
					this.noGravityTimer = 0.15f;
					this.wiggler.Start();
					return true;
				}
				if (spring.Orientation == Spring.Orientations.WallLeft && this.Speed.X <= 0f)
				{
					base.MoveTowardsY(spring.CenterY + 5f, 4f, null);
					this.Speed.X = 160f;
					this.Speed.Y = -80f;
					this.noGravityTimer = 0.1f;
					this.wiggler.Start();
					return true;
				}
				if (spring.Orientation == Spring.Orientations.WallRight && this.Speed.X >= 0f)
				{
					base.MoveTowardsY(spring.CenterY + 5f, 4f, null);
					this.Speed.X = -160f;
					this.Speed.Y = -80f;
					this.noGravityTimer = 0.1f;
					this.wiggler.Start();
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000F01 RID: 3841 RVA: 0x0003B196 File Offset: 0x00039396
		private IEnumerator DestroyAnimationRoutine()
		{
			Audio.Play("event:/new_content/game/10_farewell/glider_emancipate", this.Position);
			this.sprite.Play("death", false, false);
			yield return 1f;
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x04000A3F RID: 2623
		public static ParticleType P_Glide;

		// Token: 0x04000A40 RID: 2624
		public static ParticleType P_GlideUp;

		// Token: 0x04000A41 RID: 2625
		public static ParticleType P_Platform;

		// Token: 0x04000A42 RID: 2626
		public static ParticleType P_Glow;

		// Token: 0x04000A43 RID: 2627
		public static ParticleType P_Expand;

		// Token: 0x04000A44 RID: 2628
		private const float HighFrictionTime = 0.5f;

		// Token: 0x04000A45 RID: 2629
		public Vector2 Speed;

		// Token: 0x04000A46 RID: 2630
		public Holdable Hold;

		// Token: 0x04000A47 RID: 2631
		private Level level;

		// Token: 0x04000A48 RID: 2632
		private Collision onCollideH;

		// Token: 0x04000A49 RID: 2633
		private Collision onCollideV;

		// Token: 0x04000A4A RID: 2634
		private Vector2 prevLiftSpeed;

		// Token: 0x04000A4B RID: 2635
		private Vector2 startPos;

		// Token: 0x04000A4C RID: 2636
		private float noGravityTimer;

		// Token: 0x04000A4D RID: 2637
		private float highFrictionTimer;

		// Token: 0x04000A4E RID: 2638
		private bool bubble;

		// Token: 0x04000A4F RID: 2639
		private bool tutorial;

		// Token: 0x04000A50 RID: 2640
		private bool destroyed;

		// Token: 0x04000A51 RID: 2641
		private Sprite sprite;

		// Token: 0x04000A52 RID: 2642
		private Wiggler wiggler;

		// Token: 0x04000A53 RID: 2643
		private SineWave platformSine;

		// Token: 0x04000A54 RID: 2644
		private SoundSource fallingSfx;

		// Token: 0x04000A55 RID: 2645
		private BirdTutorialGui tutorialGui;
	}
}
