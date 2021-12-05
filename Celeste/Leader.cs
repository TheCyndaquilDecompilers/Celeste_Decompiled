using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000339 RID: 825
	public class Leader : Component
	{
		// Token: 0x060019D4 RID: 6612 RVA: 0x000A6285 File Offset: 0x000A4485
		public Leader() : base(true, false)
		{
		}

		// Token: 0x060019D5 RID: 6613 RVA: 0x000A62A5 File Offset: 0x000A44A5
		public Leader(Vector2 position) : base(true, false)
		{
			this.Position = position;
		}

		// Token: 0x060019D6 RID: 6614 RVA: 0x000A62CC File Offset: 0x000A44CC
		public void GainFollower(Follower follower)
		{
			this.Followers.Add(follower);
			follower.OnGainLeaderUtil(this);
		}

		// Token: 0x060019D7 RID: 6615 RVA: 0x000A62E1 File Offset: 0x000A44E1
		public void LoseFollower(Follower follower)
		{
			this.Followers.Remove(follower);
			follower.OnLoseLeaderUtil();
		}

		// Token: 0x060019D8 RID: 6616 RVA: 0x000A62F8 File Offset: 0x000A44F8
		public void LoseFollowers()
		{
			foreach (Follower follower in this.Followers)
			{
				follower.OnLoseLeaderUtil();
			}
			this.Followers.Clear();
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x000A6354 File Offset: 0x000A4554
		public override void Update()
		{
			Vector2 vector = base.Entity.Position + this.Position;
			if (base.Scene.OnInterval(0.02f) && (this.PastPoints.Count == 0 || (vector - this.PastPoints[0]).Length() >= 3f))
			{
				this.PastPoints.Insert(0, vector);
				if (this.PastPoints.Count > 350)
				{
					this.PastPoints.RemoveAt(this.PastPoints.Count - 1);
				}
			}
			int num = 5;
			foreach (Follower follower in this.Followers)
			{
				if (num >= this.PastPoints.Count)
				{
					break;
				}
				Vector2 value = this.PastPoints[num];
				if (follower.DelayTimer <= 0f && follower.MoveTowardsLeader)
				{
					follower.Entity.Position = follower.Entity.Position + (value - follower.Entity.Position) * (1f - (float)Math.Pow(0.009999999776482582, (double)Engine.DeltaTime));
				}
				num += 5;
			}
		}

		// Token: 0x060019DA RID: 6618 RVA: 0x000A64C4 File Offset: 0x000A46C4
		public bool HasFollower<T>()
		{
			using (List<Follower>.Enumerator enumerator = this.Followers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Entity is T)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060019DB RID: 6619 RVA: 0x000A6524 File Offset: 0x000A4724
		public void TransferFollowers()
		{
			for (int i = 0; i < this.Followers.Count; i++)
			{
				Follower follower = this.Followers[i];
				if (!follower.Entity.TagCheck(Tags.Persistent))
				{
					this.LoseFollower(follower);
					i--;
				}
			}
		}

		// Token: 0x060019DC RID: 6620 RVA: 0x000A6578 File Offset: 0x000A4778
		public static void StoreStrawberries(Leader leader)
		{
			Leader.storedBerries = new List<Strawberry>();
			Leader.storedOffsets = new List<Vector2>();
			foreach (Follower follower in leader.Followers)
			{
				if (follower.Entity is Strawberry)
				{
					Leader.storedBerries.Add(follower.Entity as Strawberry);
					Leader.storedOffsets.Add(follower.Entity.Position - leader.Entity.Position);
				}
			}
			foreach (Strawberry strawberry in Leader.storedBerries)
			{
				leader.Followers.Remove(strawberry.Follower);
				strawberry.Follower.Leader = null;
				strawberry.AddTag(Tags.Global);
			}
		}

		// Token: 0x060019DD RID: 6621 RVA: 0x000A6688 File Offset: 0x000A4888
		public static void RestoreStrawberries(Leader leader)
		{
			leader.PastPoints.Clear();
			for (int i = 0; i < Leader.storedBerries.Count; i++)
			{
				Strawberry strawberry = Leader.storedBerries[i];
				leader.GainFollower(strawberry.Follower);
				strawberry.Position = leader.Entity.Position + Leader.storedOffsets[i];
				strawberry.RemoveTag(Tags.Global);
			}
		}

		// Token: 0x0400168C RID: 5772
		public const int MaxPastPoints = 350;

		// Token: 0x0400168D RID: 5773
		public List<Follower> Followers = new List<Follower>();

		// Token: 0x0400168E RID: 5774
		public List<Vector2> PastPoints = new List<Vector2>();

		// Token: 0x0400168F RID: 5775
		public Vector2 Position;

		// Token: 0x04001690 RID: 5776
		private static List<Strawberry> storedBerries;

		// Token: 0x04001691 RID: 5777
		private static List<Vector2> storedOffsets;
	}
}
