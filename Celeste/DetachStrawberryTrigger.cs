using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001D1 RID: 465
	public class DetachStrawberryTrigger : Trigger
	{
		// Token: 0x06000FBD RID: 4029 RVA: 0x00042B14 File Offset: 0x00040D14
		public DetachStrawberryTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			Vector2[] array = data.NodesOffset(offset);
			if (array.Length != 0)
			{
				this.Target = array[0];
			}
			this.Global = data.Bool("global", true);
		}

		// Token: 0x06000FBE RID: 4030 RVA: 0x00042B54 File Offset: 0x00040D54
		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			for (int i = player.Leader.Followers.Count - 1; i >= 0; i--)
			{
				if (player.Leader.Followers[i].Entity is Strawberry)
				{
					base.Add(new Coroutine(this.DetatchFollower(player.Leader.Followers[i]), true));
				}
			}
		}

		// Token: 0x06000FBF RID: 4031 RVA: 0x00042BC5 File Offset: 0x00040DC5
		private IEnumerator DetatchFollower(Follower follower)
		{
			Leader leader = follower.Leader;
			Entity entity = follower.Entity;
			float num = (entity.Position - this.Target).Length();
			float time = num / 200f;
			Strawberry strawberry = entity as Strawberry;
			if (strawberry != null)
			{
				strawberry.ReturnHomeWhenLost = false;
			}
			leader.LoseFollower(follower);
			entity.Active = false;
			entity.Collidable = false;
			if (this.Global)
			{
				entity.AddTag(Tags.Global);
				follower.OnGainLeader = (Action)Delegate.Combine(follower.OnGainLeader, new Action(delegate()
				{
					entity.RemoveTag(Tags.Global);
				}));
			}
			else
			{
				entity.AddTag(Tags.Persistent);
			}
			Audio.Play("event:/new_content/game/10_farewell/strawberry_gold_detach", entity.Position);
			Vector2 position = entity.Position;
			SimpleCurve curve = new SimpleCurve(position, this.Target, position + (this.Target - position) * 0.5f + new Vector2(0f, -64f));
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / time)
			{
				entity.Position = curve.GetPoint(Ease.CubeInOut(p));
				yield return null;
			}
			entity.Active = true;
			entity.Collidable = true;
			yield break;
		}

		// Token: 0x04000B23 RID: 2851
		public Vector2 Target;

		// Token: 0x04000B24 RID: 2852
		public bool Global;
	}
}
