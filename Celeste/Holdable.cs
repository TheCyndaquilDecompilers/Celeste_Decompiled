using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000378 RID: 888
	[Tracked(false)]
	public class Holdable : Component
	{
		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06001C29 RID: 7209 RVA: 0x000BF14D File Offset: 0x000BD34D
		// (set) Token: 0x06001C2A RID: 7210 RVA: 0x000BF155 File Offset: 0x000BD355
		public Player Holder { get; private set; }

		// Token: 0x06001C2B RID: 7211 RVA: 0x000BF15E File Offset: 0x000BD35E
		public Holdable(float cannotHoldDelay = 0.1f) : base(true, false)
		{
			this.cannotHoldDelay = cannotHoldDelay;
		}

		// Token: 0x06001C2C RID: 7212 RVA: 0x000BF178 File Offset: 0x000BD378
		public bool Check(Player player)
		{
			Collider collider = base.Entity.Collider;
			if (this.PickupCollider != null)
			{
				base.Entity.Collider = this.PickupCollider;
			}
			bool result = player.CollideCheck(base.Entity);
			base.Entity.Collider = collider;
			return result;
		}

		// Token: 0x06001C2D RID: 7213 RVA: 0x000BF1C2 File Offset: 0x000BD3C2
		public override void Added(Entity entity)
		{
			base.Added(entity);
			this.startPos = base.Entity.Position;
		}

		// Token: 0x06001C2E RID: 7214 RVA: 0x000BF1DC File Offset: 0x000BD3DC
		public override void EntityRemoved(Scene scene)
		{
			base.EntityRemoved(scene);
			if (this.Holder != null && this.Holder != null)
			{
				this.Holder.Holding = null;
			}
		}

		// Token: 0x06001C2F RID: 7215 RVA: 0x000BF204 File Offset: 0x000BD404
		public bool Pickup(Player player)
		{
			if (this.cannotHoldTimer > 0f || base.Scene == null || base.Entity.Scene == null)
			{
				return false;
			}
			this.idleDepth = base.Entity.Depth;
			base.Entity.Depth = player.Depth - 1;
			base.Entity.Visible = true;
			this.Holder = player;
			if (this.OnPickup != null)
			{
				this.OnPickup();
			}
			return true;
		}

		// Token: 0x06001C30 RID: 7216 RVA: 0x000BF280 File Offset: 0x000BD480
		public void Carry(Vector2 position)
		{
			if (this.OnCarry != null)
			{
				this.OnCarry(position);
				return;
			}
			base.Entity.Position = position;
		}

		// Token: 0x06001C31 RID: 7217 RVA: 0x000BF2A4 File Offset: 0x000BD4A4
		public void Release(Vector2 force)
		{
			if (base.Entity.CollideCheck<Solid>())
			{
				if (force.X != 0f)
				{
					bool flag = false;
					int num = Math.Sign(force.X);
					int num2 = 0;
					while (!flag && num2++ < 10)
					{
						if (!base.Entity.CollideCheck<Solid>(base.Entity.Position + (float)(num * num2) * Vector2.UnitX))
						{
							flag = true;
						}
					}
					if (flag)
					{
						base.Entity.X += (float)(num * num2);
					}
				}
				while (base.Entity.CollideCheck<Solid>())
				{
					base.Entity.Position += Vector2.UnitY;
				}
			}
			base.Entity.Depth = this.idleDepth;
			this.Holder = null;
			this.gravityTimer = 0.1f;
			this.cannotHoldTimer = this.cannotHoldDelay;
			if (this.OnRelease != null)
			{
				this.OnRelease(force);
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06001C32 RID: 7218 RVA: 0x000BF3A2 File Offset: 0x000BD5A2
		public bool IsHeld
		{
			get
			{
				return this.Holder != null;
			}
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06001C33 RID: 7219 RVA: 0x000BF3AD File Offset: 0x000BD5AD
		public bool ShouldHaveGravity
		{
			get
			{
				return this.gravityTimer <= 0f;
			}
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x000BF3C0 File Offset: 0x000BD5C0
		public override void Update()
		{
			base.Update();
			if (this.cannotHoldTimer > 0f)
			{
				this.cannotHoldTimer -= Engine.DeltaTime;
			}
			if (this.gravityTimer > 0f)
			{
				this.gravityTimer -= Engine.DeltaTime;
			}
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x000BF414 File Offset: 0x000BD614
		public void CheckAgainstColliders()
		{
			foreach (Component component in base.Scene.Tracker.GetComponents<HoldableCollider>())
			{
				HoldableCollider holdableCollider = (HoldableCollider)component;
				if (holdableCollider.Check(this))
				{
					holdableCollider.OnCollide(this);
				}
			}
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x000BF484 File Offset: 0x000BD684
		public override void DebugRender(Camera camera)
		{
			base.DebugRender(camera);
			if (this.PickupCollider != null)
			{
				Collider collider = base.Entity.Collider;
				base.Entity.Collider = this.PickupCollider;
				base.Entity.Collider.Render(camera, Color.Pink);
				base.Entity.Collider = collider;
			}
		}

		// Token: 0x06001C37 RID: 7223 RVA: 0x000BF4DF File Offset: 0x000BD6DF
		public bool Dangerous(HoldableCollider hc)
		{
			return this.DangerousCheck != null && this.DangerousCheck(hc);
		}

		// Token: 0x06001C38 RID: 7224 RVA: 0x000BF4F7 File Offset: 0x000BD6F7
		public void HitSeeker(Seeker seeker)
		{
			if (this.OnHitSeeker != null)
			{
				this.OnHitSeeker(seeker);
			}
		}

		// Token: 0x06001C39 RID: 7225 RVA: 0x000BF50D File Offset: 0x000BD70D
		public void Swat(HoldableCollider hc, int dir)
		{
			if (this.OnSwat != null)
			{
				this.OnSwat(hc, dir);
			}
		}

		// Token: 0x06001C3A RID: 7226 RVA: 0x000BF524 File Offset: 0x000BD724
		public bool HitSpring(Spring spring)
		{
			return this.OnHitSpring != null && this.OnHitSpring(spring);
		}

		// Token: 0x06001C3B RID: 7227 RVA: 0x000BF53C File Offset: 0x000BD73C
		public void HitSpinner(Entity spinner)
		{
			if (this.OnHitSpinner != null)
			{
				this.OnHitSpinner(spinner);
			}
		}

		// Token: 0x06001C3C RID: 7228 RVA: 0x000BF552 File Offset: 0x000BD752
		public Vector2 GetSpeed()
		{
			if (this.SpeedGetter != null)
			{
				return this.SpeedGetter();
			}
			return Vector2.Zero;
		}

		// Token: 0x04001951 RID: 6481
		public Collider PickupCollider;

		// Token: 0x04001952 RID: 6482
		public Action OnPickup;

		// Token: 0x04001953 RID: 6483
		public Action<Vector2> OnCarry;

		// Token: 0x04001954 RID: 6484
		public Action<Vector2> OnRelease;

		// Token: 0x04001955 RID: 6485
		public Func<HoldableCollider, bool> DangerousCheck;

		// Token: 0x04001956 RID: 6486
		public Action<Seeker> OnHitSeeker;

		// Token: 0x04001957 RID: 6487
		public Action<HoldableCollider, int> OnSwat;

		// Token: 0x04001958 RID: 6488
		public Func<Spring, bool> OnHitSpring;

		// Token: 0x04001959 RID: 6489
		public Action<Entity> OnHitSpinner;

		// Token: 0x0400195A RID: 6490
		public Func<Vector2> SpeedGetter;

		// Token: 0x0400195B RID: 6491
		public bool SlowRun = true;

		// Token: 0x0400195C RID: 6492
		public bool SlowFall;

		// Token: 0x0400195D RID: 6493
		private float cannotHoldDelay;

		// Token: 0x0400195E RID: 6494
		private Vector2 startPos;

		// Token: 0x0400195F RID: 6495
		private float gravityTimer;

		// Token: 0x04001960 RID: 6496
		private float cannotHoldTimer;

		// Token: 0x04001961 RID: 6497
		private int idleDepth;
	}
}
