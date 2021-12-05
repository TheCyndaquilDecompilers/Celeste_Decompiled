using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000354 RID: 852
	public class Spring : Entity
	{
		// Token: 0x06001AB6 RID: 6838 RVA: 0x000AD0C4 File Offset: 0x000AB2C4
		public Spring(Vector2 position, Spring.Orientations orientation, bool playerCanUse) : base(position)
		{
			this.Orientation = orientation;
			this.playerCanUse = playerCanUse;
			base.Add(new PlayerCollider(new Action<Player>(this.OnCollide), null, null));
			base.Add(new HoldableCollider(new Action<Holdable>(this.OnHoldable), null));
			PufferCollider pufferCollider = new PufferCollider(new Action<Puffer>(this.OnPuffer), null);
			base.Add(pufferCollider);
			base.Add(this.sprite = new Sprite(GFX.Game, "objects/spring/"));
			this.sprite.Add("idle", "", 0f, new int[1]);
			this.sprite.Add("bounce", "", 0.07f, "idle", new int[]
			{
				0,
				1,
				2,
				2,
				2,
				2,
				2,
				2,
				2,
				2,
				2,
				3,
				4,
				5
			});
			this.sprite.Add("disabled", "white", 0.07f);
			this.sprite.Play("idle", false, false);
			this.sprite.Origin.X = this.sprite.Width / 2f;
			this.sprite.Origin.Y = this.sprite.Height;
			base.Depth = -8501;
			this.staticMover = new StaticMover();
			this.staticMover.OnAttach = delegate(Platform p)
			{
				base.Depth = p.Depth + 1;
			};
			if (orientation == Spring.Orientations.Floor)
			{
				this.staticMover.SolidChecker = ((Solid s) => base.CollideCheck(s, this.Position + Vector2.UnitY));
				this.staticMover.JumpThruChecker = ((JumpThru jt) => base.CollideCheck(jt, this.Position + Vector2.UnitY));
				base.Add(this.staticMover);
			}
			else if (orientation == Spring.Orientations.WallLeft)
			{
				this.staticMover.SolidChecker = ((Solid s) => base.CollideCheck(s, this.Position - Vector2.UnitX));
				this.staticMover.JumpThruChecker = ((JumpThru jt) => base.CollideCheck(jt, this.Position - Vector2.UnitX));
				base.Add(this.staticMover);
			}
			else if (orientation == Spring.Orientations.WallRight)
			{
				this.staticMover.SolidChecker = ((Solid s) => base.CollideCheck(s, this.Position + Vector2.UnitX));
				this.staticMover.JumpThruChecker = ((JumpThru jt) => base.CollideCheck(jt, this.Position + Vector2.UnitX));
				base.Add(this.staticMover);
			}
			base.Add(this.wiggler = Wiggler.Create(1f, 4f, delegate(float v)
			{
				this.sprite.Scale.Y = 1f + v * 0.2f;
			}, false, false));
			if (orientation == Spring.Orientations.Floor)
			{
				base.Collider = new Hitbox(16f, 6f, -8f, -6f);
				pufferCollider.Collider = new Hitbox(16f, 10f, -8f, -10f);
			}
			else if (orientation == Spring.Orientations.WallLeft)
			{
				base.Collider = new Hitbox(6f, 16f, 0f, -8f);
				pufferCollider.Collider = new Hitbox(12f, 16f, 0f, -8f);
				this.sprite.Rotation = 1.5707964f;
			}
			else
			{
				if (orientation != Spring.Orientations.WallRight)
				{
					throw new Exception("Orientation not supported!");
				}
				base.Collider = new Hitbox(6f, 16f, -6f, -8f);
				pufferCollider.Collider = new Hitbox(12f, 16f, -12f, -8f);
				this.sprite.Rotation = -1.5707964f;
			}
			this.staticMover.OnEnable = new Action(this.OnEnable);
			this.staticMover.OnDisable = new Action(this.OnDisable);
		}

		// Token: 0x06001AB7 RID: 6839 RVA: 0x000AD450 File Offset: 0x000AB650
		public Spring(EntityData data, Vector2 offset, Spring.Orientations orientation) : this(data.Position + offset, orientation, data.Bool("playerCanUse", true))
		{
		}

		// Token: 0x06001AB8 RID: 6840 RVA: 0x000AD474 File Offset: 0x000AB674
		private void OnEnable()
		{
			this.Visible = (this.Collidable = true);
			this.sprite.Color = Color.White;
			this.sprite.Play("idle", false, false);
		}

		// Token: 0x06001AB9 RID: 6841 RVA: 0x000AD4B3 File Offset: 0x000AB6B3
		private void OnDisable()
		{
			this.Collidable = false;
			if (this.VisibleWhenDisabled)
			{
				this.sprite.Play("disabled", false, false);
				this.sprite.Color = this.DisabledColor;
				return;
			}
			this.Visible = false;
		}

		// Token: 0x06001ABA RID: 6842 RVA: 0x000AD4F0 File Offset: 0x000AB6F0
		private void OnCollide(Player player)
		{
			if (player.StateMachine.State == 9 || !this.playerCanUse)
			{
				return;
			}
			if (this.Orientation == Spring.Orientations.Floor)
			{
				if (player.Speed.Y >= 0f)
				{
					this.BounceAnimate();
					player.SuperBounce(base.Top);
					return;
				}
			}
			else if (this.Orientation == Spring.Orientations.WallLeft)
			{
				if (player.SideBounce(1, base.Right, base.CenterY))
				{
					this.BounceAnimate();
					return;
				}
			}
			else
			{
				if (this.Orientation != Spring.Orientations.WallRight)
				{
					throw new Exception("Orientation not supported!");
				}
				if (player.SideBounce(-1, base.Left, base.CenterY))
				{
					this.BounceAnimate();
					return;
				}
			}
		}

		// Token: 0x06001ABB RID: 6843 RVA: 0x000AD597 File Offset: 0x000AB797
		private void BounceAnimate()
		{
			Audio.Play("event:/game/general/spring", base.BottomCenter);
			this.staticMover.TriggerPlatform();
			this.sprite.Play("bounce", true, false);
			this.wiggler.Start();
		}

		// Token: 0x06001ABC RID: 6844 RVA: 0x000AD5D2 File Offset: 0x000AB7D2
		private void OnHoldable(Holdable h)
		{
			if (h.HitSpring(this))
			{
				this.BounceAnimate();
			}
		}

		// Token: 0x06001ABD RID: 6845 RVA: 0x000AD5E3 File Offset: 0x000AB7E3
		private void OnPuffer(Puffer p)
		{
			if (p.HitSpring(this))
			{
				this.BounceAnimate();
			}
		}

		// Token: 0x06001ABE RID: 6846 RVA: 0x000AD5F4 File Offset: 0x000AB7F4
		private void OnSeeker(Seeker seeker)
		{
			if (seeker.Speed.Y >= -120f)
			{
				this.BounceAnimate();
				seeker.HitSpring();
			}
		}

		// Token: 0x06001ABF RID: 6847 RVA: 0x000AD614 File Offset: 0x000AB814
		public override void Render()
		{
			if (this.Collidable)
			{
				this.sprite.DrawOutline(1);
			}
			base.Render();
		}

		// Token: 0x04001761 RID: 5985
		private Sprite sprite;

		// Token: 0x04001762 RID: 5986
		private Wiggler wiggler;

		// Token: 0x04001763 RID: 5987
		private StaticMover staticMover;

		// Token: 0x04001764 RID: 5988
		public Spring.Orientations Orientation;

		// Token: 0x04001765 RID: 5989
		private bool playerCanUse;

		// Token: 0x04001766 RID: 5990
		public Color DisabledColor = Color.White;

		// Token: 0x04001767 RID: 5991
		public bool VisibleWhenDisabled;

		// Token: 0x0200070D RID: 1805
		public enum Orientations
		{
			// Token: 0x04002D81 RID: 11649
			Floor,
			// Token: 0x04002D82 RID: 11650
			WallLeft,
			// Token: 0x04002D83 RID: 11651
			WallRight
		}
	}
}
