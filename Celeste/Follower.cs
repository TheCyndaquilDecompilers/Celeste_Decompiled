using System;
using Monocle;

namespace Celeste
{
	// Token: 0x02000338 RID: 824
	public class Follower : Component
	{
		// Token: 0x170001DB RID: 475
		// (get) Token: 0x060019CD RID: 6605 RVA: 0x000A6144 File Offset: 0x000A4344
		public bool HasLeader
		{
			get
			{
				return this.Leader != null;
			}
		}

		// Token: 0x060019CE RID: 6606 RVA: 0x000A614F File Offset: 0x000A434F
		public Follower(Action onGainLeader = null, Action onLoseLeader = null) : base(true, false)
		{
			this.OnGainLeader = onGainLeader;
			this.OnLoseLeader = onLoseLeader;
		}

		// Token: 0x060019CF RID: 6607 RVA: 0x000A6180 File Offset: 0x000A4380
		public Follower(EntityID entityID, Action onGainLeader = null, Action onLoseLeader = null) : base(true, false)
		{
			this.ParentEntityID = entityID;
			this.OnGainLeader = onGainLeader;
			this.OnLoseLeader = onLoseLeader;
		}

		// Token: 0x060019D0 RID: 6608 RVA: 0x000A61B8 File Offset: 0x000A43B8
		public override void Update()
		{
			base.Update();
			if (this.DelayTimer > 0f)
			{
				this.DelayTimer -= Engine.DeltaTime;
			}
		}

		// Token: 0x060019D1 RID: 6609 RVA: 0x000A61DF File Offset: 0x000A43DF
		public void OnLoseLeaderUtil()
		{
			if (this.PersistentFollow)
			{
				base.Entity.RemoveTag(Tags.Persistent);
			}
			this.Leader = null;
			if (this.OnLoseLeader != null)
			{
				this.OnLoseLeader();
			}
		}

		// Token: 0x060019D2 RID: 6610 RVA: 0x000A6218 File Offset: 0x000A4418
		public void OnGainLeaderUtil(Leader leader)
		{
			if (this.PersistentFollow)
			{
				base.Entity.AddTag(Tags.Persistent);
			}
			this.Leader = leader;
			this.DelayTimer = this.FollowDelay;
			if (this.OnGainLeader != null)
			{
				this.OnGainLeader();
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x060019D3 RID: 6611 RVA: 0x000A6268 File Offset: 0x000A4468
		public int FollowIndex
		{
			get
			{
				if (this.Leader == null)
				{
					return -1;
				}
				return this.Leader.Followers.IndexOf(this);
			}
		}

		// Token: 0x04001684 RID: 5764
		public EntityID ParentEntityID;

		// Token: 0x04001685 RID: 5765
		public Leader Leader;

		// Token: 0x04001686 RID: 5766
		public Action OnGainLeader;

		// Token: 0x04001687 RID: 5767
		public Action OnLoseLeader;

		// Token: 0x04001688 RID: 5768
		public bool PersistentFollow = true;

		// Token: 0x04001689 RID: 5769
		public float FollowDelay = 0.5f;

		// Token: 0x0400168A RID: 5770
		public float DelayTimer;

		// Token: 0x0400168B RID: 5771
		public bool MoveTowardsLeader = true;
	}
}
