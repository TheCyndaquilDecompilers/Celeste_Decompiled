using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001A9 RID: 425
	public class Puffer : Actor
	{
		// Token: 0x06000ECB RID: 3787 RVA: 0x0003805C File Offset: 0x0003625C
		public Puffer(Vector2 position, bool faceRight) : base(position)
		{
			base.Collider = new Hitbox(12f, 10f, -6f, -5f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), new Hitbox(14f, 12f, -7f, -7f), null));
			base.Add(this.sprite = GFX.SpriteBank.Create("pufferFish"));
			this.sprite.Play("idle", false, false);
			if (!faceRight)
			{
				this.facing.X = -1f;
			}
			this.idleSine = new SineWave(0.5f, 0f);
			this.idleSine.Randomize();
			base.Add(this.idleSine);
			this.anchorPosition = this.Position;
			this.Position += new Vector2(this.idleSine.Value * 3f, this.idleSine.ValueOverTwo * 2f);
			this.state = Puffer.States.Idle;
			this.startPosition = (this.lastSinePosition = (this.lastSpeedPosition = this.Position));
			this.pushRadius = new Circle(40f, 0f, 0f);
			this.detectRadius = new Circle(32f, 0f, 0f);
			this.breakWallsRadius = new Circle(16f, 0f, 0f);
			this.onCollideV = new Collision(this.OnCollideV);
			this.onCollideH = new Collision(this.OnCollideH);
			this.scale = Vector2.One;
			this.bounceWiggler = Wiggler.Create(0.6f, 2.5f, delegate(float v)
			{
				this.sprite.Rotation = v * 20f * 0.017453292f;
			}, false, false);
			base.Add(this.bounceWiggler);
			this.inflateWiggler = Wiggler.Create(0.6f, 2f, null, false, false);
			base.Add(this.inflateWiggler);
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x00038274 File Offset: 0x00036474
		public Puffer(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("right", false))
		{
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x00008E00 File Offset: 0x00007000
		public override bool IsRiding(JumpThru jumpThru)
		{
			return false;
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x00008E00 File Offset: 0x00007000
		public override bool IsRiding(Solid solid)
		{
			return false;
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x00038294 File Offset: 0x00036494
		protected override void OnSquish(CollisionData data)
		{
			this.Explode();
			this.GotoGone();
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x000382A2 File Offset: 0x000364A2
		private void OnCollideH(CollisionData data)
		{
			this.hitSpeed.X = this.hitSpeed.X * -0.8f;
		}

		// Token: 0x06000ED1 RID: 3793 RVA: 0x000382B8 File Offset: 0x000364B8
		private void OnCollideV(CollisionData data)
		{
			if (data.Direction.Y > 0f)
			{
				for (int i = -1; i <= 1; i += 2)
				{
					for (int j = 1; j <= 2; j++)
					{
						Vector2 vector = this.Position + Vector2.UnitX * (float)j * (float)i;
						if (!base.CollideCheck<Solid>(vector) && !base.OnGround(vector, 1))
						{
							this.Position = vector;
							return;
						}
					}
				}
				this.hitSpeed.Y = this.hitSpeed.Y * -0.2f;
			}
		}

		// Token: 0x06000ED2 RID: 3794 RVA: 0x00038340 File Offset: 0x00036540
		private void GotoIdle()
		{
			if (this.state == Puffer.States.Gone)
			{
				this.Position = this.startPosition;
				this.cantExplodeTimer = 0.5f;
				this.sprite.Play("recover", false, false);
				Audio.Play("event:/new_content/game/10_farewell/puffer_reform", this.Position);
			}
			this.lastSinePosition = (this.lastSpeedPosition = (this.anchorPosition = this.Position));
			this.hitSpeed = Vector2.Zero;
			this.idleSine.Reset();
			this.state = Puffer.States.Idle;
		}

		// Token: 0x06000ED3 RID: 3795 RVA: 0x000383CC File Offset: 0x000365CC
		private void GotoHit(Vector2 from)
		{
			this.scale = new Vector2(1.2f, 0.8f);
			this.hitSpeed = Vector2.UnitY * 200f;
			this.state = Puffer.States.Hit;
			this.bounceWiggler.Start();
			this.Alert(true, false);
			Audio.Play("event:/new_content/game/10_farewell/puffer_boop", this.Position);
		}

		// Token: 0x06000ED4 RID: 3796 RVA: 0x0003842E File Offset: 0x0003662E
		private void GotoHitSpeed(Vector2 speed)
		{
			this.hitSpeed = speed;
			this.state = Puffer.States.Hit;
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x00038440 File Offset: 0x00036640
		private void GotoGone()
		{
			Vector2 vector = this.Position + (this.startPosition - this.Position) * 0.5f;
			if ((this.startPosition - this.Position).LengthSquared() > 100f)
			{
				if (Math.Abs(this.Position.Y - this.startPosition.Y) > Math.Abs(this.Position.X - this.startPosition.X))
				{
					if (this.Position.X > this.startPosition.X)
					{
						vector += Vector2.UnitX * -24f;
					}
					else
					{
						vector += Vector2.UnitX * 24f;
					}
				}
				else if (this.Position.Y > this.startPosition.Y)
				{
					vector += Vector2.UnitY * -24f;
				}
				else
				{
					vector += Vector2.UnitY * 24f;
				}
			}
			this.returnCurve = new SimpleCurve(this.Position, this.startPosition, vector);
			this.Collidable = false;
			this.goneTimer = 2.5f;
			this.state = Puffer.States.Gone;
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x00038590 File Offset: 0x00036790
		private void Explode()
		{
			Collider collider = base.Collider;
			base.Collider = this.pushRadius;
			Audio.Play("event:/new_content/game/10_farewell/puffer_splode", this.Position);
			this.sprite.Play("explode", false, false);
			Player player = base.CollideFirst<Player>();
			if (player != null && !base.Scene.CollideCheck<Solid>(this.Position, player.Center))
			{
				player.ExplodeLaunch(this.Position, false, true);
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
			foreach (Entity entity3 in base.Scene.Tracker.GetEntities<FloatingDebris>())
			{
				FloatingDebris floatingDebris = (FloatingDebris)entity3;
				if (base.CollideCheck(floatingDebris))
				{
					floatingDebris.OnExplode(this.Position);
				}
			}
			base.Collider = collider;
			Level level = base.SceneAs<Level>();
			level.Shake(0.3f);
			level.Displacement.AddBurst(this.Position, 0.4f, 12f, 36f, 0.5f, null, null);
			level.Displacement.AddBurst(this.Position, 0.4f, 24f, 48f, 0.5f, null, null);
			level.Displacement.AddBurst(this.Position, 0.4f, 36f, 60f, 0.5f, null, null);
			for (float num = 0f; num < 6.2831855f; num += 0.17453292f)
			{
				Vector2 position = base.Center + Calc.AngleToVector(num + Calc.Random.Range(-0.034906585f, 0.034906585f), (float)Calc.Random.Range(12, 18));
				level.Particles.Emit(Seeker.P_Regen, position, num);
			}
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x00038854 File Offset: 0x00036A54
		public override void Render()
		{
			this.sprite.Scale = this.scale * (1f + this.inflateWiggler.Value * 0.4f);
			this.sprite.Scale *= this.facing;
			bool flag = false;
			if (this.sprite.CurrentAnimationID != "hidden" && this.sprite.CurrentAnimationID != "explode" && this.sprite.CurrentAnimationID != "recover")
			{
				flag = true;
			}
			else if (this.sprite.CurrentAnimationID == "explode" && this.sprite.CurrentAnimationFrame <= 1)
			{
				flag = true;
			}
			else if (this.sprite.CurrentAnimationID == "recover" && this.sprite.CurrentAnimationFrame >= 4)
			{
				flag = true;
			}
			if (flag)
			{
				this.sprite.DrawSimpleOutline();
			}
			float num = this.playerAliveFade * Calc.ClampedMap((this.Position - this.lastPlayerPos).Length(), 128f, 96f, 0f, 1f);
			if (num > 0f && this.state != Puffer.States.Gone)
			{
				bool flag2 = false;
				Vector2 vector = this.lastPlayerPos;
				if (vector.Y < base.Y)
				{
					vector.Y = base.Y - (vector.Y - base.Y) * 0.5f;
					vector.X += vector.X - base.X;
					flag2 = true;
				}
				float radiansB = (vector - this.Position).Angle();
				for (int i = 0; i < 28; i++)
				{
					float num2 = (float)Math.Sin((double)(base.Scene.TimeActive * 0.5f)) * 0.02f;
					float num3 = Calc.Map((float)i / 28f + num2, 0f, 1f, -0.10471976f, 3.2463126f);
					num3 += this.bounceWiggler.Value * 20f * 0.017453292f;
					Vector2 value = Calc.AngleToVector(num3, 1f);
					Vector2 vector2 = this.Position + value * 32f;
					float num4 = Calc.ClampedMap(Calc.AbsAngleDiff(num3, radiansB), 1.5707964f, 0.17453292f, 0f, 1f);
					num4 = Ease.CubeOut(num4) * 0.8f * num;
					if (num4 > 0f)
					{
						if (i == 0 || i == 27)
						{
							Draw.Line(vector2, vector2 - value * 10f, Color.White * num4);
						}
						else
						{
							Vector2 vector3 = value * (float)Math.Sin((double)(base.Scene.TimeActive * 2f + (float)i * 0.6f));
							if (i % 2 == 0)
							{
								vector3 *= -1f;
							}
							vector2 += vector3;
							if (!flag2 && Calc.AbsAngleDiff(num3, radiansB) <= 0.17453292f)
							{
								Draw.Line(vector2, vector2 - value * 3f, Color.White * num4);
							}
							else
							{
								Draw.Point(vector2, Color.White * num4);
							}
						}
					}
				}
			}
			base.Render();
			if (this.sprite.CurrentAnimationID == "alerted")
			{
				Vector2 vector4 = this.Position + new Vector2(3f, (float)((this.facing.X < 0f) ? -5 : -4)) * this.sprite.Scale;
				Vector2 to = this.lastPlayerPos + new Vector2(0f, -4f);
				Vector2 vector5 = Calc.AngleToVector(Calc.Angle(vector4, to) + this.eyeSpin * 6.2831855f * 2f, 1f);
				Vector2 vector6 = vector4 + new Vector2((float)Math.Round((double)vector5.X), (float)Math.Round((double)Calc.ClampedMap(vector5.Y, -1f, 1f, -1f, 2f)));
				Draw.Rect(vector6.X, vector6.Y, 1f, 1f, Color.Black);
			}
			this.sprite.Scale /= this.facing;
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x00038CE4 File Offset: 0x00036EE4
		public override void Update()
		{
			base.Update();
			this.eyeSpin = Calc.Approach(this.eyeSpin, 0f, Engine.DeltaTime * 1.5f);
			this.scale = Calc.Approach(this.scale, Vector2.One, 1f * Engine.DeltaTime);
			if (this.cannotHitTimer > 0f)
			{
				this.cannotHitTimer -= Engine.DeltaTime;
			}
			if (this.state != Puffer.States.Gone && this.cantExplodeTimer > 0f)
			{
				this.cantExplodeTimer -= Engine.DeltaTime;
			}
			if (this.alertTimer > 0f)
			{
				this.alertTimer -= Engine.DeltaTime;
			}
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity == null)
			{
				this.playerAliveFade = Calc.Approach(this.playerAliveFade, 0f, 1f * Engine.DeltaTime);
			}
			else
			{
				this.playerAliveFade = Calc.Approach(this.playerAliveFade, 1f, 1f * Engine.DeltaTime);
				this.lastPlayerPos = entity.Center;
			}
			switch (this.state)
			{
			case Puffer.States.Idle:
			{
				if (this.Position != this.lastSinePosition)
				{
					this.anchorPosition += this.Position - this.lastSinePosition;
				}
				Vector2 vector = this.anchorPosition + new Vector2(this.idleSine.Value * 3f, this.idleSine.ValueOverTwo * 2f);
				base.MoveToX(vector.X, null);
				base.MoveToY(vector.Y, null);
				this.lastSinePosition = this.Position;
				if (this.ProximityExplodeCheck())
				{
					this.Explode();
					this.GotoGone();
					return;
				}
				if (this.AlertedCheck())
				{
					this.Alert(false, true);
				}
				else if (this.sprite.CurrentAnimationID == "alerted" && this.alertTimer <= 0f)
				{
					Audio.Play("event:/new_content/game/10_farewell/puffer_shrink", this.Position);
					this.sprite.Play("unalert", false, false);
				}
				using (List<Component>.Enumerator enumerator = base.Scene.Tracker.GetComponents<PufferCollider>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Component component = enumerator.Current;
						((PufferCollider)component).Check(this);
					}
					return;
				}
				break;
			}
			case Puffer.States.Hit:
				break;
			case Puffer.States.Gone:
			{
				float num = this.goneTimer;
				this.goneTimer -= Engine.DeltaTime;
				if (this.goneTimer <= 0.5f)
				{
					if (num > 0.5f && this.returnCurve.GetLengthParametric(8) > 8f)
					{
						Audio.Play("event:/new_content/game/10_farewell/puffer_return", this.Position);
					}
					this.Position = this.returnCurve.GetPoint(Ease.CubeInOut(Calc.ClampedMap(this.goneTimer, 0.5f, 0f, 0f, 1f)));
				}
				if (this.goneTimer <= 0f)
				{
					this.Visible = (this.Collidable = true);
					this.GotoIdle();
					return;
				}
				return;
			}
			default:
				return;
			}
			this.lastSpeedPosition = this.Position;
			base.MoveH(this.hitSpeed.X * Engine.DeltaTime, this.onCollideH, null);
			base.MoveV(this.hitSpeed.Y * Engine.DeltaTime, new Collision(this.OnCollideV), null);
			this.anchorPosition = this.Position;
			this.hitSpeed.X = Calc.Approach(this.hitSpeed.X, 0f, 150f * Engine.DeltaTime);
			this.hitSpeed = Calc.Approach(this.hitSpeed, Vector2.Zero, 320f * Engine.DeltaTime);
			if (this.ProximityExplodeCheck())
			{
				this.Explode();
				this.GotoGone();
				return;
			}
			if (base.Top >= (float)(base.SceneAs<Level>().Bounds.Bottom + 5))
			{
				this.sprite.Play("hidden", false, false);
				this.GotoGone();
				return;
			}
			foreach (Component component2 in base.Scene.Tracker.GetComponents<PufferCollider>())
			{
				((PufferCollider)component2).Check(this);
			}
			if (this.hitSpeed == Vector2.Zero)
			{
				base.ZeroRemainderX();
				base.ZeroRemainderY();
				this.GotoIdle();
				return;
			}
		}

		// Token: 0x06000ED9 RID: 3801 RVA: 0x00039190 File Offset: 0x00037390
		public bool HitSpring(Spring spring)
		{
			switch (spring.Orientation)
			{
			default:
				if (this.hitSpeed.Y >= 0f)
				{
					this.GotoHitSpeed(224f * -Vector2.UnitY);
					base.MoveTowardsX(spring.CenterX, 4f, null);
					this.bounceWiggler.Start();
					this.Alert(true, false);
					return true;
				}
				return false;
			case Spring.Orientations.WallLeft:
				if (this.hitSpeed.X <= 60f)
				{
					this.facing.X = 1f;
					this.GotoHitSpeed(280f * Vector2.UnitX);
					base.MoveTowardsY(spring.CenterY, 4f, null);
					this.bounceWiggler.Start();
					this.Alert(true, false);
					return true;
				}
				return false;
			case Spring.Orientations.WallRight:
				if (this.hitSpeed.X >= -60f)
				{
					this.facing.X = -1f;
					this.GotoHitSpeed(280f * -Vector2.UnitX);
					base.MoveTowardsY(spring.CenterY, 4f, null);
					this.bounceWiggler.Start();
					this.Alert(true, false);
					return true;
				}
				return false;
			}
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x000392D0 File Offset: 0x000374D0
		private bool ProximityExplodeCheck()
		{
			if (this.cantExplodeTimer > 0f)
			{
				return false;
			}
			bool result = false;
			Collider collider = base.Collider;
			base.Collider = this.detectRadius;
			Player player;
			if ((player = base.CollideFirst<Player>()) != null && player.CenterY >= base.Y + collider.Bottom - 4f && !base.Scene.CollideCheck<Solid>(this.Position, player.Center))
			{
				result = true;
			}
			base.Collider = collider;
			return result;
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x0003934C File Offset: 0x0003754C
		private bool AlertedCheck()
		{
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			return entity != null && (entity.Center - base.Center).Length() < 60f;
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x00039390 File Offset: 0x00037590
		private void Alert(bool restart, bool playSfx)
		{
			if (this.sprite.CurrentAnimationID == "idle")
			{
				if (playSfx)
				{
					Audio.Play("event:/new_content/game/10_farewell/puffer_expand", this.Position);
				}
				this.sprite.Play("alert", false, false);
				this.inflateWiggler.Start();
			}
			else if (restart && playSfx)
			{
				Audio.Play("event:/new_content/game/10_farewell/puffer_expand", this.Position);
			}
			this.alertTimer = 2f;
		}

		// Token: 0x06000EDD RID: 3805 RVA: 0x0003940C File Offset: 0x0003760C
		private void OnPlayer(Player player)
		{
			if (this.state != Puffer.States.Gone && this.cantExplodeTimer <= 0f)
			{
				if (this.cannotHitTimer <= 0f)
				{
					if (player.Bottom > this.lastSpeedPosition.Y + 3f)
					{
						this.Explode();
						this.GotoGone();
					}
					else
					{
						player.Bounce(base.Top);
						this.GotoHit(player.Center);
						base.MoveToX(this.anchorPosition.X, null);
						this.idleSine.Reset();
						this.anchorPosition = (this.lastSinePosition = this.Position);
						this.eyeSpin = 1f;
					}
				}
				this.cannotHitTimer = 0.1f;
			}
		}

		// Token: 0x04000A00 RID: 2560
		private const float RespawnTime = 2.5f;

		// Token: 0x04000A01 RID: 2561
		private const float RespawnMoveTime = 0.5f;

		// Token: 0x04000A02 RID: 2562
		private const float BounceSpeed = 200f;

		// Token: 0x04000A03 RID: 2563
		private const float ExplodeRadius = 40f;

		// Token: 0x04000A04 RID: 2564
		private const float DetectRadius = 32f;

		// Token: 0x04000A05 RID: 2565
		private const float StunnedAccel = 320f;

		// Token: 0x04000A06 RID: 2566
		private const float AlertedRadius = 60f;

		// Token: 0x04000A07 RID: 2567
		private const float CantExplodeTime = 0.5f;

		// Token: 0x04000A08 RID: 2568
		private Sprite sprite;

		// Token: 0x04000A09 RID: 2569
		private Puffer.States state;

		// Token: 0x04000A0A RID: 2570
		private Vector2 startPosition;

		// Token: 0x04000A0B RID: 2571
		private Vector2 anchorPosition;

		// Token: 0x04000A0C RID: 2572
		private Vector2 lastSpeedPosition;

		// Token: 0x04000A0D RID: 2573
		private Vector2 lastSinePosition;

		// Token: 0x04000A0E RID: 2574
		private Circle pushRadius;

		// Token: 0x04000A0F RID: 2575
		private Circle breakWallsRadius;

		// Token: 0x04000A10 RID: 2576
		private Circle detectRadius;

		// Token: 0x04000A11 RID: 2577
		private SineWave idleSine;

		// Token: 0x04000A12 RID: 2578
		private Vector2 hitSpeed;

		// Token: 0x04000A13 RID: 2579
		private float goneTimer;

		// Token: 0x04000A14 RID: 2580
		private float cannotHitTimer;

		// Token: 0x04000A15 RID: 2581
		private Collision onCollideV;

		// Token: 0x04000A16 RID: 2582
		private Collision onCollideH;

		// Token: 0x04000A17 RID: 2583
		private float alertTimer;

		// Token: 0x04000A18 RID: 2584
		private Wiggler bounceWiggler;

		// Token: 0x04000A19 RID: 2585
		private Wiggler inflateWiggler;

		// Token: 0x04000A1A RID: 2586
		private Vector2 scale;

		// Token: 0x04000A1B RID: 2587
		private SimpleCurve returnCurve;

		// Token: 0x04000A1C RID: 2588
		private float cantExplodeTimer;

		// Token: 0x04000A1D RID: 2589
		private Vector2 lastPlayerPos;

		// Token: 0x04000A1E RID: 2590
		private float playerAliveFade;

		// Token: 0x04000A1F RID: 2591
		private Vector2 facing = Vector2.One;

		// Token: 0x04000A20 RID: 2592
		private float eyeSpin;

		// Token: 0x020004BC RID: 1212
		private enum States
		{
			// Token: 0x0400235E RID: 9054
			Idle,
			// Token: 0x0400235F RID: 9055
			Hit,
			// Token: 0x04002360 RID: 9056
			Gone
		}
	}
}
