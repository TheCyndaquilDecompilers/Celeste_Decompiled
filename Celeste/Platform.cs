using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200037C RID: 892
	[Tracked(true)]
	public abstract class Platform : Entity
	{
		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06001C48 RID: 7240 RVA: 0x000BFA00 File Offset: 0x000BDC00
		public Vector2 Shake
		{
			get
			{
				return this.shakeAmount;
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x06001C49 RID: 7241 RVA: 0x000BFA08 File Offset: 0x000BDC08
		public Hitbox Hitbox
		{
			get
			{
				return base.Collider as Hitbox;
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06001C4A RID: 7242 RVA: 0x000BFA15 File Offset: 0x000BDC15
		public Vector2 ExactPosition
		{
			get
			{
				return this.Position + this.movementCounter;
			}
		}

		// Token: 0x06001C4B RID: 7243 RVA: 0x000BFA28 File Offset: 0x000BDC28
		public Platform(Vector2 position, bool safe) : base(position)
		{
			this.Safe = safe;
			base.Depth = -9000;
		}

		// Token: 0x06001C4C RID: 7244 RVA: 0x000BFA5C File Offset: 0x000BDC5C
		public void ClearRemainder()
		{
			this.movementCounter = Vector2.Zero;
		}

		// Token: 0x06001C4D RID: 7245 RVA: 0x000BFA6C File Offset: 0x000BDC6C
		public override void Update()
		{
			base.Update();
			this.LiftSpeed = Vector2.Zero;
			if (this.shaking)
			{
				if (base.Scene.OnInterval(0.04f))
				{
					Vector2 value = this.shakeAmount;
					this.shakeAmount = Calc.Random.ShakeVector();
					this.OnShake(this.shakeAmount - value);
				}
				if (this.shakeTimer > 0f)
				{
					this.shakeTimer -= Engine.DeltaTime;
					if (this.shakeTimer <= 0f)
					{
						this.shaking = false;
						this.StopShaking();
					}
				}
			}
		}

		// Token: 0x06001C4E RID: 7246 RVA: 0x000BFB06 File Offset: 0x000BDD06
		public void StartShaking(float time = 0f)
		{
			this.shaking = true;
			this.shakeTimer = time;
		}

		// Token: 0x06001C4F RID: 7247 RVA: 0x000BFB16 File Offset: 0x000BDD16
		public void StopShaking()
		{
			this.shaking = false;
			if (this.shakeAmount != Vector2.Zero)
			{
				this.OnShake(-this.shakeAmount);
				this.shakeAmount = Vector2.Zero;
			}
		}

		// Token: 0x06001C50 RID: 7248 RVA: 0x000BFB4D File Offset: 0x000BDD4D
		public virtual void OnShake(Vector2 amount)
		{
			this.ShakeStaticMovers(amount);
		}

		// Token: 0x06001C51 RID: 7249 RVA: 0x000BFB58 File Offset: 0x000BDD58
		public void ShakeStaticMovers(Vector2 amount)
		{
			foreach (StaticMover staticMover in this.staticMovers)
			{
				staticMover.Shake(amount);
			}
		}

		// Token: 0x06001C52 RID: 7250 RVA: 0x000BFBAC File Offset: 0x000BDDAC
		public void MoveStaticMovers(Vector2 amount)
		{
			foreach (StaticMover staticMover in this.staticMovers)
			{
				staticMover.Move(amount);
			}
		}

		// Token: 0x06001C53 RID: 7251 RVA: 0x000BFC00 File Offset: 0x000BDE00
		public void DestroyStaticMovers()
		{
			foreach (StaticMover staticMover in this.staticMovers)
			{
				staticMover.Destroy();
			}
			this.staticMovers.Clear();
		}

		// Token: 0x06001C54 RID: 7252 RVA: 0x000BFC5C File Offset: 0x000BDE5C
		public void DisableStaticMovers()
		{
			foreach (StaticMover staticMover in this.staticMovers)
			{
				staticMover.Disable();
			}
		}

		// Token: 0x06001C55 RID: 7253 RVA: 0x000BFCAC File Offset: 0x000BDEAC
		public void EnableStaticMovers()
		{
			foreach (StaticMover staticMover in this.staticMovers)
			{
				staticMover.Enable();
			}
		}

		// Token: 0x06001C56 RID: 7254 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void OnStaticMoverTrigger(StaticMover sm)
		{
		}

		// Token: 0x06001C57 RID: 7255 RVA: 0x000BFCFC File Offset: 0x000BDEFC
		public virtual int GetLandSoundIndex(Entity entity)
		{
			return this.SurfaceSoundIndex;
		}

		// Token: 0x06001C58 RID: 7256 RVA: 0x000BFCFC File Offset: 0x000BDEFC
		public virtual int GetWallSoundIndex(Player player, int side)
		{
			return this.SurfaceSoundIndex;
		}

		// Token: 0x06001C59 RID: 7257 RVA: 0x000BFCFC File Offset: 0x000BDEFC
		public virtual int GetStepSoundIndex(Entity entity)
		{
			return this.SurfaceSoundIndex;
		}

		// Token: 0x06001C5A RID: 7258 RVA: 0x000BFD04 File Offset: 0x000BDF04
		public void MoveH(float moveH)
		{
			if (Engine.DeltaTime == 0f)
			{
				this.LiftSpeed.X = 0f;
			}
			else
			{
				this.LiftSpeed.X = moveH / Engine.DeltaTime;
			}
			this.movementCounter.X = this.movementCounter.X + moveH;
			int num = (int)Math.Round((double)this.movementCounter.X);
			if (num != 0)
			{
				this.movementCounter.X = this.movementCounter.X - (float)num;
				this.MoveHExact(num);
			}
		}

		// Token: 0x06001C5B RID: 7259 RVA: 0x000BFD80 File Offset: 0x000BDF80
		public void MoveH(float moveH, float liftSpeedH)
		{
			this.LiftSpeed.X = liftSpeedH;
			this.movementCounter.X = this.movementCounter.X + moveH;
			int num = (int)Math.Round((double)this.movementCounter.X);
			if (num != 0)
			{
				this.movementCounter.X = this.movementCounter.X - (float)num;
				this.MoveHExact(num);
			}
		}

		// Token: 0x06001C5C RID: 7260 RVA: 0x000BFDD8 File Offset: 0x000BDFD8
		public void MoveV(float moveV)
		{
			if (Engine.DeltaTime == 0f)
			{
				this.LiftSpeed.Y = 0f;
			}
			else
			{
				this.LiftSpeed.Y = moveV / Engine.DeltaTime;
			}
			this.movementCounter.Y = this.movementCounter.Y + moveV;
			int num = (int)Math.Round((double)this.movementCounter.Y);
			if (num != 0)
			{
				this.movementCounter.Y = this.movementCounter.Y - (float)num;
				this.MoveVExact(num);
			}
		}

		// Token: 0x06001C5D RID: 7261 RVA: 0x000BFE54 File Offset: 0x000BE054
		public void MoveV(float moveV, float liftSpeedV)
		{
			this.LiftSpeed.Y = liftSpeedV;
			this.movementCounter.Y = this.movementCounter.Y + moveV;
			int num = (int)Math.Round((double)this.movementCounter.Y);
			if (num != 0)
			{
				this.movementCounter.Y = this.movementCounter.Y - (float)num;
				this.MoveVExact(num);
			}
		}

		// Token: 0x06001C5E RID: 7262 RVA: 0x000BFEAB File Offset: 0x000BE0AB
		public void MoveToX(float x)
		{
			this.MoveH(x - this.ExactPosition.X);
		}

		// Token: 0x06001C5F RID: 7263 RVA: 0x000BFEC0 File Offset: 0x000BE0C0
		public void MoveToX(float x, float liftSpeedX)
		{
			this.MoveH(x - this.ExactPosition.X, liftSpeedX);
		}

		// Token: 0x06001C60 RID: 7264 RVA: 0x000BFED6 File Offset: 0x000BE0D6
		public void MoveToY(float y)
		{
			this.MoveV(y - this.ExactPosition.Y);
		}

		// Token: 0x06001C61 RID: 7265 RVA: 0x000BFEEB File Offset: 0x000BE0EB
		public void MoveToY(float y, float liftSpeedY)
		{
			this.MoveV(y - this.ExactPosition.Y, liftSpeedY);
		}

		// Token: 0x06001C62 RID: 7266 RVA: 0x000BFF01 File Offset: 0x000BE101
		public void MoveTo(Vector2 position)
		{
			this.MoveToX(position.X);
			this.MoveToY(position.Y);
		}

		// Token: 0x06001C63 RID: 7267 RVA: 0x000BFF1B File Offset: 0x000BE11B
		public void MoveTo(Vector2 position, Vector2 liftSpeed)
		{
			this.MoveToX(position.X, liftSpeed.X);
			this.MoveToY(position.Y, liftSpeed.Y);
		}

		// Token: 0x06001C64 RID: 7268 RVA: 0x000BFF44 File Offset: 0x000BE144
		public void MoveTowardsX(float x, float amount)
		{
			float x2 = Calc.Approach(this.ExactPosition.X, x, amount);
			this.MoveToX(x2);
		}

		// Token: 0x06001C65 RID: 7269 RVA: 0x000BFF6C File Offset: 0x000BE16C
		public void MoveTowardsY(float y, float amount)
		{
			float y2 = Calc.Approach(this.ExactPosition.Y, y, amount);
			this.MoveToY(y2);
		}

		// Token: 0x06001C66 RID: 7270
		public abstract void MoveHExact(int move);

		// Token: 0x06001C67 RID: 7271
		public abstract void MoveVExact(int move);

		// Token: 0x06001C68 RID: 7272 RVA: 0x000BFF93 File Offset: 0x000BE193
		public void MoveToNaive(Vector2 position)
		{
			this.MoveToXNaive(position.X);
			this.MoveToYNaive(position.Y);
		}

		// Token: 0x06001C69 RID: 7273 RVA: 0x000BFFAD File Offset: 0x000BE1AD
		public void MoveToXNaive(float x)
		{
			this.MoveHNaive(x - this.ExactPosition.X);
		}

		// Token: 0x06001C6A RID: 7274 RVA: 0x000BFFC2 File Offset: 0x000BE1C2
		public void MoveToYNaive(float y)
		{
			this.MoveVNaive(y - this.ExactPosition.Y);
		}

		// Token: 0x06001C6B RID: 7275 RVA: 0x000BFFD8 File Offset: 0x000BE1D8
		public void MoveHNaive(float moveH)
		{
			if (Engine.DeltaTime == 0f)
			{
				this.LiftSpeed.X = 0f;
			}
			else
			{
				this.LiftSpeed.X = moveH / Engine.DeltaTime;
			}
			this.movementCounter.X = this.movementCounter.X + moveH;
			int num = (int)Math.Round((double)this.movementCounter.X);
			if (num != 0)
			{
				this.movementCounter.X = this.movementCounter.X - (float)num;
				base.X += (float)num;
				this.MoveStaticMovers(Vector2.UnitX * (float)num);
			}
		}

		// Token: 0x06001C6C RID: 7276 RVA: 0x000C0070 File Offset: 0x000BE270
		public void MoveVNaive(float moveV)
		{
			if (Engine.DeltaTime == 0f)
			{
				this.LiftSpeed.Y = 0f;
			}
			else
			{
				this.LiftSpeed.Y = moveV / Engine.DeltaTime;
			}
			this.movementCounter.Y = this.movementCounter.Y + moveV;
			int num = (int)Math.Round((double)this.movementCounter.Y);
			if (num != 0)
			{
				this.movementCounter.Y = this.movementCounter.Y - (float)num;
				base.Y += (float)num;
				this.MoveStaticMovers(Vector2.UnitY * (float)num);
			}
		}

		// Token: 0x06001C6D RID: 7277 RVA: 0x000C0108 File Offset: 0x000BE308
		public bool MoveHCollideSolids(float moveH, bool thruDashBlocks, Action<Vector2, Vector2, Platform> onCollide = null)
		{
			if (Engine.DeltaTime == 0f)
			{
				this.LiftSpeed.X = 0f;
			}
			else
			{
				this.LiftSpeed.X = moveH / Engine.DeltaTime;
			}
			this.movementCounter.X = this.movementCounter.X + moveH;
			int num = (int)Math.Round((double)this.movementCounter.X);
			if (num != 0)
			{
				this.movementCounter.X = this.movementCounter.X - (float)num;
				return this.MoveHExactCollideSolids(num, thruDashBlocks, onCollide);
			}
			return false;
		}

		// Token: 0x06001C6E RID: 7278 RVA: 0x000C0188 File Offset: 0x000BE388
		public bool MoveVCollideSolids(float moveV, bool thruDashBlocks, Action<Vector2, Vector2, Platform> onCollide = null)
		{
			if (Engine.DeltaTime == 0f)
			{
				this.LiftSpeed.Y = 0f;
			}
			else
			{
				this.LiftSpeed.Y = moveV / Engine.DeltaTime;
			}
			this.movementCounter.Y = this.movementCounter.Y + moveV;
			int num = (int)Math.Round((double)this.movementCounter.Y);
			if (num != 0)
			{
				this.movementCounter.Y = this.movementCounter.Y - (float)num;
				return this.MoveVExactCollideSolids(num, thruDashBlocks, onCollide);
			}
			return false;
		}

		// Token: 0x06001C6F RID: 7279 RVA: 0x000C0208 File Offset: 0x000BE408
		public bool MoveHCollideSolidsAndBounds(Level level, float moveH, bool thruDashBlocks, Action<Vector2, Vector2, Platform> onCollide = null)
		{
			if (Engine.DeltaTime == 0f)
			{
				this.LiftSpeed.X = 0f;
			}
			else
			{
				this.LiftSpeed.X = moveH / Engine.DeltaTime;
			}
			this.movementCounter.X = this.movementCounter.X + moveH;
			int num = (int)Math.Round((double)this.movementCounter.X);
			if (num != 0)
			{
				this.movementCounter.X = this.movementCounter.X - (float)num;
				bool flag;
				if (base.Left + (float)num < (float)level.Bounds.Left)
				{
					flag = true;
					num = level.Bounds.Left - (int)base.Left;
				}
				else if (base.Right + (float)num > (float)level.Bounds.Right)
				{
					flag = true;
					num = level.Bounds.Right - (int)base.Right;
				}
				else
				{
					flag = false;
				}
				return this.MoveHExactCollideSolids(num, thruDashBlocks, onCollide) || flag;
			}
			return false;
		}

		// Token: 0x06001C70 RID: 7280 RVA: 0x000C02FC File Offset: 0x000BE4FC
		public bool MoveVCollideSolidsAndBounds(Level level, float moveV, bool thruDashBlocks, Action<Vector2, Vector2, Platform> onCollide = null, bool checkBottom = true)
		{
			if (Engine.DeltaTime == 0f)
			{
				this.LiftSpeed.Y = 0f;
			}
			else
			{
				this.LiftSpeed.Y = moveV / Engine.DeltaTime;
			}
			this.movementCounter.Y = this.movementCounter.Y + moveV;
			int num = (int)Math.Round((double)this.movementCounter.Y);
			if (num != 0)
			{
				this.movementCounter.Y = this.movementCounter.Y - (float)num;
				int num2 = level.Bounds.Bottom + 32;
				bool flag;
				if (base.Top + (float)num < (float)level.Bounds.Top)
				{
					flag = true;
					num = level.Bounds.Top - (int)base.Top;
				}
				else if (checkBottom && base.Bottom + (float)num > (float)num2)
				{
					flag = true;
					num = num2 - (int)base.Bottom;
				}
				else
				{
					flag = false;
				}
				return this.MoveVExactCollideSolids(num, thruDashBlocks, onCollide) || flag;
			}
			return false;
		}

		// Token: 0x06001C71 RID: 7281 RVA: 0x000C03EC File Offset: 0x000BE5EC
		public bool MoveHExactCollideSolids(int moveH, bool thruDashBlocks, Action<Vector2, Vector2, Platform> onCollide = null)
		{
			float x = base.X;
			int num = Math.Sign(moveH);
			int num2 = 0;
			Solid solid = null;
			while (moveH != 0)
			{
				if (thruDashBlocks)
				{
					foreach (Entity entity in base.Scene.Tracker.GetEntities<DashBlock>())
					{
						DashBlock dashBlock = (DashBlock)entity;
						if (base.CollideCheck(dashBlock, this.Position + Vector2.UnitX * (float)num))
						{
							dashBlock.Break(base.Center, Vector2.UnitX * (float)num, true, true);
							base.SceneAs<Level>().Shake(0.2f);
							Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
						}
					}
				}
				solid = base.CollideFirst<Solid>(this.Position + Vector2.UnitX * (float)num);
				if (solid != null)
				{
					break;
				}
				num2 += num;
				moveH -= num;
				base.X += (float)num;
			}
			base.X = x;
			this.MoveHExact(num2);
			if (solid != null && onCollide != null)
			{
				onCollide(Vector2.UnitX * (float)num, Vector2.UnitX * (float)num2, solid);
			}
			return solid != null;
		}

		// Token: 0x06001C72 RID: 7282 RVA: 0x000C0530 File Offset: 0x000BE730
		public bool MoveVExactCollideSolids(int moveV, bool thruDashBlocks, Action<Vector2, Vector2, Platform> onCollide = null)
		{
			float y = base.Y;
			int num = Math.Sign(moveV);
			int num2 = 0;
			Platform platform = null;
			while (moveV != 0)
			{
				if (thruDashBlocks)
				{
					foreach (Entity entity in base.Scene.Tracker.GetEntities<DashBlock>())
					{
						DashBlock dashBlock = (DashBlock)entity;
						if (base.CollideCheck(dashBlock, this.Position + Vector2.UnitY * (float)num))
						{
							dashBlock.Break(base.Center, Vector2.UnitY * (float)num, true, true);
							base.SceneAs<Level>().Shake(0.2f);
							Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
						}
					}
				}
				platform = base.CollideFirst<Solid>(this.Position + Vector2.UnitY * (float)num);
				if (platform != null)
				{
					break;
				}
				if (moveV > 0)
				{
					platform = base.CollideFirstOutside<JumpThru>(this.Position + Vector2.UnitY * (float)num);
					if (platform != null)
					{
						break;
					}
				}
				num2 += num;
				moveV -= num;
				base.Y += (float)num;
			}
			base.Y = y;
			this.MoveVExact(num2);
			if (platform != null && onCollide != null)
			{
				onCollide(Vector2.UnitY * (float)num, Vector2.UnitY * (float)num2, platform);
			}
			return platform != null;
		}

		// Token: 0x04001968 RID: 6504
		private Vector2 movementCounter;

		// Token: 0x04001969 RID: 6505
		private Vector2 shakeAmount;

		// Token: 0x0400196A RID: 6506
		private bool shaking;

		// Token: 0x0400196B RID: 6507
		private float shakeTimer;

		// Token: 0x0400196C RID: 6508
		protected List<StaticMover> staticMovers = new List<StaticMover>();

		// Token: 0x0400196D RID: 6509
		public Vector2 LiftSpeed;

		// Token: 0x0400196E RID: 6510
		public bool Safe;

		// Token: 0x0400196F RID: 6511
		public bool BlockWaterfalls = true;

		// Token: 0x04001970 RID: 6512
		public int SurfaceSoundIndex = 8;

		// Token: 0x04001971 RID: 6513
		public int SurfaceSoundPriority;

		// Token: 0x04001972 RID: 6514
		public DashCollision OnDashCollide;

		// Token: 0x04001973 RID: 6515
		public Action<Vector2> OnCollide;
	}
}
