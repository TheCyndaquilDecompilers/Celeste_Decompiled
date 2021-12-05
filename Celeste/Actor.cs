using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000377 RID: 887
	[Tracked(true)]
	public class Actor : Entity
	{
		// Token: 0x06001C11 RID: 7185 RVA: 0x000BEA4B File Offset: 0x000BCC4B
		public Actor(Vector2 position) : base(position)
		{
			this.SquishCallback = new Collision(this.OnSquish);
		}

		// Token: 0x06001C12 RID: 7186 RVA: 0x0003B038 File Offset: 0x00039238
		protected virtual void OnSquish(CollisionData data)
		{
			if (!this.TrySquishWiggle(data, 3, 3))
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06001C13 RID: 7187 RVA: 0x000BEA7C File Offset: 0x000BCC7C
		protected bool TrySquishWiggle(CollisionData data, int wiggleX = 3, int wiggleY = 3)
		{
			data.Pusher.Collidable = true;
			for (int i = 0; i <= wiggleX; i++)
			{
				for (int j = 0; j <= wiggleY; j++)
				{
					if (i != 0 || j != 0)
					{
						for (int k = 1; k >= -1; k -= 2)
						{
							for (int l = 1; l >= -1; l -= 2)
							{
								Vector2 value = new Vector2((float)(i * k), (float)(j * l));
								if (!base.CollideCheck<Solid>(this.Position + value))
								{
									this.Position += value;
									data.Pusher.Collidable = false;
									return true;
								}
							}
						}
					}
				}
			}
			for (int m = 0; m <= wiggleX; m++)
			{
				for (int n = 0; n <= wiggleY; n++)
				{
					if (m != 0 || n != 0)
					{
						for (int num = 1; num >= -1; num -= 2)
						{
							for (int num2 = 1; num2 >= -1; num2 -= 2)
							{
								Vector2 value2 = new Vector2((float)(m * num), (float)(n * num2));
								if (!base.CollideCheck<Solid>(data.TargetPosition + value2))
								{
									this.Position = data.TargetPosition + value2;
									data.Pusher.Collidable = false;
									return true;
								}
							}
						}
					}
				}
			}
			data.Pusher.Collidable = false;
			return false;
		}

		// Token: 0x06001C14 RID: 7188 RVA: 0x000BEBB4 File Offset: 0x000BCDB4
		public virtual bool IsRiding(JumpThru jumpThru)
		{
			return !this.IgnoreJumpThrus && base.CollideCheckOutside(jumpThru, this.Position + Vector2.UnitY);
		}

		// Token: 0x06001C15 RID: 7189 RVA: 0x000AD640 File Offset: 0x000AB840
		public virtual bool IsRiding(Solid solid)
		{
			return base.CollideCheck(solid, this.Position + Vector2.UnitY);
		}

		// Token: 0x06001C16 RID: 7190 RVA: 0x000BEBD8 File Offset: 0x000BCDD8
		public bool OnGround(int downCheck = 1)
		{
			return base.CollideCheck<Solid>(this.Position + Vector2.UnitY * (float)downCheck) || (!this.IgnoreJumpThrus && base.CollideCheckOutside<JumpThru>(this.Position + Vector2.UnitY * (float)downCheck));
		}

		// Token: 0x06001C17 RID: 7191 RVA: 0x000BEC30 File Offset: 0x000BCE30
		public bool OnGround(Vector2 at, int downCheck = 1)
		{
			Vector2 position = this.Position;
			this.Position = at;
			bool result = this.OnGround(downCheck);
			this.Position = position;
			return result;
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06001C18 RID: 7192 RVA: 0x000BEC59 File Offset: 0x000BCE59
		public Vector2 ExactPosition
		{
			get
			{
				return this.Position + this.movementCounter;
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06001C19 RID: 7193 RVA: 0x000BEC6C File Offset: 0x000BCE6C
		public Vector2 PositionRemainder
		{
			get
			{
				return this.movementCounter;
			}
		}

		// Token: 0x06001C1A RID: 7194 RVA: 0x000BEC74 File Offset: 0x000BCE74
		public void ZeroRemainderX()
		{
			this.movementCounter.X = 0f;
		}

		// Token: 0x06001C1B RID: 7195 RVA: 0x000BEC86 File Offset: 0x000BCE86
		public void ZeroRemainderY()
		{
			this.movementCounter.Y = 0f;
		}

		// Token: 0x06001C1C RID: 7196 RVA: 0x000BEC98 File Offset: 0x000BCE98
		public override void Update()
		{
			base.Update();
			this.LiftSpeed = Vector2.Zero;
			if (this.liftSpeedTimer > 0f)
			{
				this.liftSpeedTimer -= Engine.DeltaTime;
				if (this.liftSpeedTimer <= 0f)
				{
					this.lastLiftSpeed = Vector2.Zero;
				}
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06001C1E RID: 7198 RVA: 0x000BED23 File Offset: 0x000BCF23
		// (set) Token: 0x06001C1D RID: 7197 RVA: 0x000BECED File Offset: 0x000BCEED
		public Vector2 LiftSpeed
		{
			get
			{
				if (this.currentLiftSpeed == Vector2.Zero)
				{
					return this.lastLiftSpeed;
				}
				return this.currentLiftSpeed;
			}
			set
			{
				this.currentLiftSpeed = value;
				if (value != Vector2.Zero && this.LiftSpeedGraceTime > 0f)
				{
					this.lastLiftSpeed = value;
					this.liftSpeedTimer = this.LiftSpeedGraceTime;
				}
			}
		}

		// Token: 0x06001C1F RID: 7199 RVA: 0x000BED44 File Offset: 0x000BCF44
		public void ResetLiftSpeed()
		{
			this.currentLiftSpeed = (this.lastLiftSpeed = Vector2.Zero);
			this.liftSpeedTimer = 0f;
		}

		// Token: 0x06001C20 RID: 7200 RVA: 0x000BED70 File Offset: 0x000BCF70
		public bool MoveH(float moveH, Collision onCollide = null, Solid pusher = null)
		{
			this.movementCounter.X = this.movementCounter.X + moveH;
			int num = (int)Math.Round((double)this.movementCounter.X, MidpointRounding.ToEven);
			if (num != 0)
			{
				this.movementCounter.X = this.movementCounter.X - (float)num;
				return this.MoveHExact(num, onCollide, pusher);
			}
			return false;
		}

		// Token: 0x06001C21 RID: 7201 RVA: 0x000BEDC0 File Offset: 0x000BCFC0
		public bool MoveV(float moveV, Collision onCollide = null, Solid pusher = null)
		{
			this.movementCounter.Y = this.movementCounter.Y + moveV;
			int num = (int)Math.Round((double)this.movementCounter.Y, MidpointRounding.ToEven);
			if (num != 0)
			{
				this.movementCounter.Y = this.movementCounter.Y - (float)num;
				return this.MoveVExact(num, onCollide, pusher);
			}
			return false;
		}

		// Token: 0x06001C22 RID: 7202 RVA: 0x000BEE10 File Offset: 0x000BD010
		public bool MoveHExact(int moveH, Collision onCollide = null, Solid pusher = null)
		{
			Vector2 targetPosition = this.Position + Vector2.UnitX * (float)moveH;
			int num = Math.Sign(moveH);
			int num2 = 0;
			while (moveH != 0)
			{
				Solid solid = base.CollideFirst<Solid>(this.Position + Vector2.UnitX * (float)num);
				if (solid != null)
				{
					this.movementCounter.X = 0f;
					if (onCollide != null)
					{
						onCollide(new CollisionData
						{
							Direction = Vector2.UnitX * (float)num,
							Moved = Vector2.UnitX * (float)num2,
							TargetPosition = targetPosition,
							Hit = solid,
							Pusher = pusher
						});
					}
					return true;
				}
				num2 += num;
				moveH -= num;
				base.X += (float)num;
			}
			return false;
		}

		// Token: 0x06001C23 RID: 7203 RVA: 0x000BEEE8 File Offset: 0x000BD0E8
		public bool MoveVExact(int moveV, Collision onCollide = null, Solid pusher = null)
		{
			Vector2 targetPosition = this.Position + Vector2.UnitY * (float)moveV;
			int num = Math.Sign(moveV);
			int num2 = 0;
			while (moveV != 0)
			{
				Platform platform = base.CollideFirst<Solid>(this.Position + Vector2.UnitY * (float)num);
				if (platform != null)
				{
					this.movementCounter.Y = 0f;
					if (onCollide != null)
					{
						onCollide(new CollisionData
						{
							Direction = Vector2.UnitY * (float)num,
							Moved = Vector2.UnitY * (float)num2,
							TargetPosition = targetPosition,
							Hit = platform,
							Pusher = pusher
						});
					}
					return true;
				}
				if (moveV > 0 && !this.IgnoreJumpThrus)
				{
					platform = base.CollideFirstOutside<JumpThru>(this.Position + Vector2.UnitY * (float)num);
					if (platform != null)
					{
						this.movementCounter.Y = 0f;
						if (onCollide != null)
						{
							onCollide(new CollisionData
							{
								Direction = Vector2.UnitY * (float)num,
								Moved = Vector2.UnitY * (float)num2,
								TargetPosition = targetPosition,
								Hit = platform,
								Pusher = pusher
							});
						}
						return true;
					}
				}
				num2 += num;
				moveV -= num;
				base.Y += (float)num;
			}
			return false;
		}

		// Token: 0x06001C24 RID: 7204 RVA: 0x000BF054 File Offset: 0x000BD254
		public void MoveTowardsX(float targetX, float maxAmount, Collision onCollide = null)
		{
			float toX = Calc.Approach(this.ExactPosition.X, targetX, maxAmount);
			this.MoveToX(toX, onCollide);
		}

		// Token: 0x06001C25 RID: 7205 RVA: 0x000BF07C File Offset: 0x000BD27C
		public void MoveTowardsY(float targetY, float maxAmount, Collision onCollide = null)
		{
			float toY = Calc.Approach(this.ExactPosition.Y, targetY, maxAmount);
			this.MoveToY(toY, onCollide);
		}

		// Token: 0x06001C26 RID: 7206 RVA: 0x000BF0A4 File Offset: 0x000BD2A4
		public void MoveToX(float toX, Collision onCollide = null)
		{
			this.MoveH(toX - this.ExactPosition.X, onCollide, null);
		}

		// Token: 0x06001C27 RID: 7207 RVA: 0x000BF0BC File Offset: 0x000BD2BC
		public void MoveToY(float toY, Collision onCollide = null)
		{
			this.MoveV(toY - this.ExactPosition.Y, onCollide, null);
		}

		// Token: 0x06001C28 RID: 7208 RVA: 0x000BF0D4 File Offset: 0x000BD2D4
		public void NaiveMove(Vector2 amount)
		{
			this.movementCounter += amount;
			int num = (int)Math.Round((double)this.movementCounter.X);
			int num2 = (int)Math.Round((double)this.movementCounter.Y);
			this.Position += new Vector2((float)num, (float)num2);
			this.movementCounter -= new Vector2((float)num, (float)num2);
		}

		// Token: 0x04001947 RID: 6471
		public Collision SquishCallback;

		// Token: 0x04001948 RID: 6472
		public bool TreatNaive;

		// Token: 0x04001949 RID: 6473
		private Vector2 movementCounter;

		// Token: 0x0400194A RID: 6474
		public bool IgnoreJumpThrus;

		// Token: 0x0400194B RID: 6475
		public bool AllowPushing = true;

		// Token: 0x0400194C RID: 6476
		public float LiftSpeedGraceTime = 0.16f;

		// Token: 0x0400194D RID: 6477
		private Vector2 currentLiftSpeed;

		// Token: 0x0400194E RID: 6478
		private Vector2 lastLiftSpeed;

		// Token: 0x0400194F RID: 6479
		private float liftSpeedTimer;
	}
}
