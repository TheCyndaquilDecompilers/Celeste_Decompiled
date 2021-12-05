using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200037D RID: 893
	[Tracked(true)]
	public class Solid : Platform
	{
		// Token: 0x06001C73 RID: 7283 RVA: 0x000C0698 File Offset: 0x000BE898
		public Solid(Vector2 position, float width, float height, bool safe) : base(position, safe)
		{
			base.Collider = new Hitbox(width, height, 0f, 0f);
		}

		// Token: 0x06001C74 RID: 7284 RVA: 0x000C06D8 File Offset: 0x000BE8D8
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (this.AllowStaticMovers)
			{
				bool collidable = this.Collidable;
				this.Collidable = true;
				foreach (Component component in scene.Tracker.GetComponents<StaticMover>())
				{
					StaticMover staticMover = (StaticMover)component;
					if (staticMover.IsRiding(this) && staticMover.Platform == null)
					{
						this.staticMovers.Add(staticMover);
						staticMover.Platform = this;
						if (staticMover.OnAttach != null)
						{
							staticMover.OnAttach(this);
						}
					}
				}
				this.Collidable = collidable;
			}
		}

		// Token: 0x06001C75 RID: 7285 RVA: 0x000C0790 File Offset: 0x000BE990
		public override void Update()
		{
			base.Update();
			base.MoveH(this.Speed.X * Engine.DeltaTime);
			base.MoveV(this.Speed.Y * Engine.DeltaTime);
			if (this.EnableAssistModeChecks && SaveData.Instance != null && SaveData.Instance.Assists.Invincible && base.Components.Get<SolidOnInvinciblePlayer>() == null && this.Collidable)
			{
				Player player = base.CollideFirst<Player>();
				Level level = base.Scene as Level;
				if (player == null && base.Bottom > (float)level.Bounds.Bottom)
				{
					player = base.CollideFirst<Player>(this.Position + Vector2.UnitY);
				}
				if (player != null && player.StateMachine.State != 9 && player.StateMachine.State != 21)
				{
					base.Add(new SolidOnInvinciblePlayer());
					return;
				}
				TheoCrystal theoCrystal = base.CollideFirst<TheoCrystal>();
				if (theoCrystal != null && !theoCrystal.Hold.IsHeld)
				{
					base.Add(new SolidOnInvinciblePlayer());
				}
			}
		}

		// Token: 0x06001C76 RID: 7286 RVA: 0x000C08A8 File Offset: 0x000BEAA8
		public bool HasRider()
		{
			using (List<Entity>.Enumerator enumerator = base.Scene.Tracker.GetEntities<Actor>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((Actor)enumerator.Current).IsRiding(this))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001C77 RID: 7287 RVA: 0x000C0914 File Offset: 0x000BEB14
		public Player GetPlayerRider()
		{
			foreach (Entity entity in base.Scene.Tracker.GetEntities<Player>())
			{
				Player player = (Player)entity;
				if (player.IsRiding(this))
				{
					return player;
				}
			}
			return null;
		}

		// Token: 0x06001C78 RID: 7288 RVA: 0x000C0980 File Offset: 0x000BEB80
		public bool HasPlayerRider()
		{
			return this.GetPlayerRider() != null;
		}

		// Token: 0x06001C79 RID: 7289 RVA: 0x000C098B File Offset: 0x000BEB8B
		public bool HasPlayerOnTop()
		{
			return this.GetPlayerOnTop() != null;
		}

		// Token: 0x06001C7A RID: 7290 RVA: 0x000C0996 File Offset: 0x000BEB96
		public Player GetPlayerOnTop()
		{
			return base.CollideFirst<Player>(this.Position - Vector2.UnitY);
		}

		// Token: 0x06001C7B RID: 7291 RVA: 0x000C09AE File Offset: 0x000BEBAE
		public bool HasPlayerClimbing()
		{
			return this.GetPlayerClimbing() != null;
		}

		// Token: 0x06001C7C RID: 7292 RVA: 0x000C09BC File Offset: 0x000BEBBC
		public Player GetPlayerClimbing()
		{
			foreach (Entity entity in base.Scene.Tracker.GetEntities<Player>())
			{
				Player player = (Player)entity;
				if (player.StateMachine.State == 1)
				{
					if (player.Facing == Facings.Left && base.CollideCheck(player, this.Position + Vector2.UnitX))
					{
						return player;
					}
					if (player.Facing == Facings.Right && base.CollideCheck(player, this.Position - Vector2.UnitX))
					{
						return player;
					}
				}
			}
			return null;
		}

		// Token: 0x06001C7D RID: 7293 RVA: 0x000C0A74 File Offset: 0x000BEC74
		public void GetRiders()
		{
			foreach (Entity entity in base.Scene.Tracker.GetEntities<Actor>())
			{
				Actor actor = (Actor)entity;
				if (actor.IsRiding(this))
				{
					Solid.riders.Add(actor);
				}
			}
		}

		// Token: 0x06001C7E RID: 7294 RVA: 0x000C0AE4 File Offset: 0x000BECE4
		public override void MoveHExact(int move)
		{
			this.GetRiders();
			float right = base.Right;
			float left = base.Left;
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null && Input.MoveX.Value == Math.Sign(move) && Math.Sign(entity.Speed.X) == Math.Sign(move) && !Solid.riders.Contains(entity) && base.CollideCheck(entity, this.Position + Vector2.UnitX * (float)move - Vector2.UnitY))
			{
				entity.MoveV(1f, null, null);
			}
			base.X += (float)move;
			base.MoveStaticMovers(Vector2.UnitX * (float)move);
			if (this.Collidable)
			{
				foreach (Entity entity2 in base.Scene.Tracker.GetEntities<Actor>())
				{
					Actor actor = (Actor)entity2;
					if (actor.AllowPushing)
					{
						bool collidable = actor.Collidable;
						actor.Collidable = true;
						if (!actor.TreatNaive && base.CollideCheck(actor, this.Position))
						{
							int moveH;
							if (move > 0)
							{
								moveH = move - (int)(actor.Left - right);
							}
							else
							{
								moveH = move - (int)(actor.Right - left);
							}
							this.Collidable = false;
							actor.MoveHExact(moveH, actor.SquishCallback, this);
							actor.LiftSpeed = this.LiftSpeed;
							this.Collidable = true;
						}
						else if (Solid.riders.Contains(actor))
						{
							this.Collidable = false;
							if (actor.TreatNaive)
							{
								actor.NaiveMove(Vector2.UnitX * (float)move);
							}
							else
							{
								actor.MoveHExact(move, null, null);
							}
							actor.LiftSpeed = this.LiftSpeed;
							this.Collidable = true;
						}
						actor.Collidable = collidable;
					}
				}
			}
			Solid.riders.Clear();
		}

		// Token: 0x06001C7F RID: 7295 RVA: 0x000C0D04 File Offset: 0x000BEF04
		public override void MoveVExact(int move)
		{
			this.GetRiders();
			float bottom = base.Bottom;
			float top = base.Top;
			base.Y += (float)move;
			base.MoveStaticMovers(Vector2.UnitY * (float)move);
			if (this.Collidable)
			{
				foreach (Entity entity in base.Scene.Tracker.GetEntities<Actor>())
				{
					Actor actor = (Actor)entity;
					if (actor.AllowPushing)
					{
						bool collidable = actor.Collidable;
						actor.Collidable = true;
						if (!actor.TreatNaive && base.CollideCheck(actor, this.Position))
						{
							int moveV;
							if (move > 0)
							{
								moveV = move - (int)(actor.Top - bottom);
							}
							else
							{
								moveV = move - (int)(actor.Bottom - top);
							}
							this.Collidable = false;
							actor.MoveVExact(moveV, actor.SquishCallback, this);
							actor.LiftSpeed = this.LiftSpeed;
							this.Collidable = true;
						}
						else if (Solid.riders.Contains(actor))
						{
							this.Collidable = false;
							if (actor.TreatNaive)
							{
								actor.NaiveMove(Vector2.UnitY * (float)move);
							}
							else
							{
								actor.MoveVExact(move, null, null);
							}
							actor.LiftSpeed = this.LiftSpeed;
							this.Collidable = true;
						}
						actor.Collidable = collidable;
					}
				}
			}
			Solid.riders.Clear();
		}

		// Token: 0x04001974 RID: 6516
		public Vector2 Speed;

		// Token: 0x04001975 RID: 6517
		public bool AllowStaticMovers = true;

		// Token: 0x04001976 RID: 6518
		public bool EnableAssistModeChecks = true;

		// Token: 0x04001977 RID: 6519
		public bool DisableLightsInside = true;

		// Token: 0x04001978 RID: 6520
		public bool StopPlayerRunIntoAnimation = true;

		// Token: 0x04001979 RID: 6521
		public bool SquishEvenInAssistMode;

		// Token: 0x0400197A RID: 6522
		private static HashSet<Actor> riders = new HashSet<Actor>();
	}
}
