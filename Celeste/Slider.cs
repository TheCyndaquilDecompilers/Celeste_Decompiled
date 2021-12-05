using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000223 RID: 547
	public class Slider : Entity
	{
		// Token: 0x06001193 RID: 4499 RVA: 0x00056FCC File Offset: 0x000551CC
		public Slider(Vector2 position, bool clockwise, Slider.Surfaces surface) : base(position)
		{
			base.Collider = new Circle(10f, 0f, 0f);
			base.Add(new StaticMover());
			switch (surface)
			{
			default:
				this.dir = Vector2.UnitX;
				this.surface = Vector2.UnitY;
				break;
			case Slider.Surfaces.Ceiling:
				this.dir = -Vector2.UnitX;
				this.surface = -Vector2.UnitY;
				break;
			case Slider.Surfaces.LeftWall:
				this.dir = -Vector2.UnitY;
				this.surface = -Vector2.UnitX;
				break;
			case Slider.Surfaces.RightWall:
				this.dir = Vector2.UnitY;
				this.surface = Vector2.UnitX;
				break;
			}
			if (!clockwise)
			{
				this.dir *= -1f;
			}
			this.moving = true;
			this.foundSurfaceAfterCorner = (this.gotOutOfWall = true);
			this.speed = 80f;
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x000570E1 File Offset: 0x000552E1
		public Slider(EntityData e, Vector2 offset) : this(e.Position + offset, e.Bool("clockwise", true), e.Enum<Slider.Surfaces>("surface", Slider.Surfaces.Floor))
		{
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x00057110 File Offset: 0x00055310
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			int num = 0;
			while (!base.Scene.CollideCheck<Solid>(this.Position))
			{
				this.Position += this.surface;
				if (num >= 100)
				{
					throw new Exception("Couldn't find surface");
				}
			}
		}

		// Token: 0x06001196 RID: 4502 RVA: 0x00057162 File Offset: 0x00055362
		private void OnPlayer(Player Player)
		{
			Player.Die((Player.Center - base.Center).SafeNormalize(-Vector2.UnitY), false, true);
			this.moving = false;
		}

		// Token: 0x06001197 RID: 4503 RVA: 0x00057194 File Offset: 0x00055394
		public override void Update()
		{
			base.Update();
			if (this.moving)
			{
				this.speed = Calc.Approach(this.speed, 80f, 400f * Engine.DeltaTime);
				this.Position += this.dir * this.speed * Engine.DeltaTime;
				if (!this.OnSurfaceCheck())
				{
					if (this.foundSurfaceAfterCorner)
					{
						this.Position = this.Position.Round();
						int num = 0;
						while (!this.OnSurfaceCheck())
						{
							this.Position -= this.dir;
							num++;
							if (num >= 100)
							{
								throw new Exception("Couldn't get back onto corner!");
							}
						}
						this.foundSurfaceAfterCorner = false;
						Vector2 value = this.dir;
						this.dir = this.surface;
						this.surface = -value;
						return;
					}
				}
				else
				{
					this.foundSurfaceAfterCorner = true;
					if (this.InWallCheck())
					{
						if (this.gotOutOfWall)
						{
							this.Position = this.Position.Round();
							int num2 = 0;
							while (this.InWallCheck())
							{
								this.Position -= this.dir;
								num2++;
								if (num2 >= 100)
								{
									throw new Exception("Couldn't get out of wall!");
								}
							}
							this.Position += this.dir - this.surface;
							this.gotOutOfWall = false;
							Vector2 value2 = this.surface;
							this.surface = this.dir;
							this.dir = -value2;
							return;
						}
					}
					else
					{
						this.gotOutOfWall = true;
					}
				}
			}
		}

		// Token: 0x06001198 RID: 4504 RVA: 0x00057336 File Offset: 0x00055536
		private bool OnSurfaceCheck()
		{
			return base.Scene.CollideCheck<Solid>(this.Position.Round() + this.surface);
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x00057359 File Offset: 0x00055559
		private bool InWallCheck()
		{
			return base.Scene.CollideCheck<Solid>(this.Position.Round() - this.surface);
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x0005737C File Offset: 0x0005557C
		public override void Render()
		{
			Draw.Circle(this.Position, 12f, Color.Red, 8);
		}

		// Token: 0x04000D2E RID: 3374
		private const float MaxSpeed = 80f;

		// Token: 0x04000D2F RID: 3375
		private const float Accel = 400f;

		// Token: 0x04000D30 RID: 3376
		private Vector2 dir;

		// Token: 0x04000D31 RID: 3377
		private Vector2 surface;

		// Token: 0x04000D32 RID: 3378
		private bool foundSurfaceAfterCorner;

		// Token: 0x04000D33 RID: 3379
		private bool gotOutOfWall;

		// Token: 0x04000D34 RID: 3380
		private float speed;

		// Token: 0x04000D35 RID: 3381
		private bool moving;

		// Token: 0x02000540 RID: 1344
		public enum Surfaces
		{
			// Token: 0x040025AD RID: 9645
			Floor,
			// Token: 0x040025AE RID: 9646
			Ceiling,
			// Token: 0x040025AF RID: 9647
			LeftWall,
			// Token: 0x040025B0 RID: 9648
			RightWall
		}
	}
}
