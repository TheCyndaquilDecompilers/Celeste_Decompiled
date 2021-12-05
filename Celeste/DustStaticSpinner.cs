using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002C1 RID: 705
	[Tracked(false)]
	public class DustStaticSpinner : Entity
	{
		// Token: 0x060015DE RID: 5598 RVA: 0x0007ED20 File Offset: 0x0007CF20
		public DustStaticSpinner(Vector2 position, bool attachToSolid, bool ignoreSolids = false) : base(position)
		{
			base.Collider = new ColliderList(new Collider[]
			{
				new Circle(6f, 0f, 0f),
				new Hitbox(16f, 4f, -8f, -3f)
			});
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(new HoldableCollider(new Action<Holdable>(this.OnHoldable), null));
			base.Add(new LedgeBlocker(null));
			base.Add(this.Sprite = new DustGraphic(ignoreSolids, true, true));
			base.Depth = -50;
			if (attachToSolid)
			{
				base.Add(new StaticMover
				{
					OnShake = new Action<Vector2>(this.OnShake),
					SolidChecker = new Func<Solid, bool>(this.IsRiding)
				});
			}
		}

		// Token: 0x060015DF RID: 5599 RVA: 0x0007EE18 File Offset: 0x0007D018
		public DustStaticSpinner(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("attachToSolid", false), false)
		{
		}

		// Token: 0x060015E0 RID: 5600 RVA: 0x0007EE39 File Offset: 0x0007D039
		public void ForceInstantiate()
		{
			this.Sprite.AddDustNodesIfInCamera();
		}

		// Token: 0x060015E1 RID: 5601 RVA: 0x0007EE48 File Offset: 0x0007D048
		public override void Update()
		{
			base.Update();
			if (base.Scene.OnInterval(0.05f, this.offset) && this.Sprite.Estableshed)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					this.Collidable = (Math.Abs(entity.X - base.X) < 128f && Math.Abs(entity.Y - base.Y) < 128f);
				}
			}
		}

		// Token: 0x060015E2 RID: 5602 RVA: 0x0007EECF File Offset: 0x0007D0CF
		private void OnShake(Vector2 pos)
		{
			this.Sprite.Position = pos;
		}

		// Token: 0x060015E3 RID: 5603 RVA: 0x00033BE8 File Offset: 0x00031DE8
		private bool IsRiding(Solid solid)
		{
			return base.CollideCheck(solid);
		}

		// Token: 0x060015E4 RID: 5604 RVA: 0x0007EEDD File Offset: 0x0007D0DD
		private void OnPlayer(Player player)
		{
			player.Die((player.Position - this.Position).SafeNormalize(), false, true);
			this.Sprite.OnHitPlayer();
		}

		// Token: 0x060015E5 RID: 5605 RVA: 0x00033C12 File Offset: 0x00031E12
		private void OnHoldable(Holdable h)
		{
			h.HitSpinner(this);
		}

		// Token: 0x04001223 RID: 4643
		public static ParticleType P_Move;

		// Token: 0x04001224 RID: 4644
		public const float ParticleInterval = 0.02f;

		// Token: 0x04001225 RID: 4645
		public DustGraphic Sprite;

		// Token: 0x04001226 RID: 4646
		private float offset = Calc.Random.NextFloat();
	}
}
