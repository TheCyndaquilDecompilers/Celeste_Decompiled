using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000379 RID: 889
	[Tracked(true)]
	public class JumpThru : Platform
	{
		// Token: 0x06001C3D RID: 7229 RVA: 0x000BF56D File Offset: 0x000BD76D
		public JumpThru(Vector2 position, int width, bool safe) : base(position, safe)
		{
			base.Collider = new Hitbox((float)width, 5f, 0f, 0f);
		}

		// Token: 0x06001C3E RID: 7230 RVA: 0x000BF594 File Offset: 0x000BD794
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
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
		}

		// Token: 0x06001C3F RID: 7231 RVA: 0x000BF62C File Offset: 0x000BD82C
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

		// Token: 0x06001C40 RID: 7232 RVA: 0x000BF698 File Offset: 0x000BD898
		public bool HasPlayerRider()
		{
			using (List<Entity>.Enumerator enumerator = base.Scene.Tracker.GetEntities<Player>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((Player)enumerator.Current).IsRiding(this))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001C41 RID: 7233 RVA: 0x000BF704 File Offset: 0x000BD904
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

		// Token: 0x06001C42 RID: 7234 RVA: 0x000BF770 File Offset: 0x000BD970
		public override void MoveHExact(int move)
		{
			if (this.Collidable)
			{
				foreach (Entity entity in base.Scene.Tracker.GetEntities<Actor>())
				{
					Actor actor = (Actor)entity;
					if (actor.IsRiding(this))
					{
						if (actor.TreatNaive)
						{
							actor.NaiveMove(Vector2.UnitX * (float)move);
						}
						else
						{
							actor.MoveHExact(move, null, null);
						}
					}
				}
			}
			base.X += (float)move;
			base.MoveStaticMovers(Vector2.UnitX * (float)move);
		}

		// Token: 0x06001C43 RID: 7235 RVA: 0x000BF824 File Offset: 0x000BDA24
		public override void MoveVExact(int move)
		{
			if (this.Collidable)
			{
				if (move < 0)
				{
					using (List<Entity>.Enumerator enumerator = base.Scene.Tracker.GetEntities<Actor>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Entity entity = enumerator.Current;
							Actor actor = (Actor)entity;
							if (actor.IsRiding(this))
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
							else if (!actor.TreatNaive && base.CollideCheck(actor, this.Position + Vector2.UnitY * (float)move) && !base.CollideCheck(actor))
							{
								this.Collidable = false;
								actor.MoveVExact((int)(base.Top + (float)move - actor.Bottom), null, null);
								actor.LiftSpeed = this.LiftSpeed;
								this.Collidable = true;
							}
						}
						goto IL_190;
					}
				}
				foreach (Entity entity2 in base.Scene.Tracker.GetEntities<Actor>())
				{
					Actor actor2 = (Actor)entity2;
					if (actor2.IsRiding(this))
					{
						this.Collidable = false;
						if (actor2.TreatNaive)
						{
							actor2.NaiveMove(Vector2.UnitY * (float)move);
						}
						else
						{
							actor2.MoveVExact(move, null, null);
						}
						actor2.LiftSpeed = this.LiftSpeed;
						this.Collidable = true;
					}
				}
			}
			IL_190:
			base.Y += (float)move;
			base.MoveStaticMovers(Vector2.UnitY * (float)move);
		}
	}
}
