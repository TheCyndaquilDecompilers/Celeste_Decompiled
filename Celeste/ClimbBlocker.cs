using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000154 RID: 340
	[Tracked(false)]
	public class ClimbBlocker : Component
	{
		// Token: 0x06000C54 RID: 3156 RVA: 0x000288E7 File Offset: 0x00026AE7
		public ClimbBlocker(bool edge) : base(false, false)
		{
			this.Edge = edge;
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x00028900 File Offset: 0x00026B00
		public static bool Check(Scene scene, Entity entity, Vector2 at)
		{
			Vector2 position = entity.Position;
			entity.Position = at;
			bool result = ClimbBlocker.Check(scene, entity);
			entity.Position = position;
			return result;
		}

		// Token: 0x06000C56 RID: 3158 RVA: 0x0002892C File Offset: 0x00026B2C
		public static bool Check(Scene scene, Entity entity)
		{
			foreach (Component component in scene.Tracker.GetComponents<ClimbBlocker>())
			{
				ClimbBlocker climbBlocker = (ClimbBlocker)component;
				if (climbBlocker.Blocking && entity.CollideCheck(climbBlocker.Entity))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000C57 RID: 3159 RVA: 0x000289A0 File Offset: 0x00026BA0
		public static bool EdgeCheck(Scene scene, Entity entity, int dir)
		{
			foreach (Component component in scene.Tracker.GetComponents<ClimbBlocker>())
			{
				ClimbBlocker climbBlocker = (ClimbBlocker)component;
				if (climbBlocker.Blocking && climbBlocker.Edge && entity.CollideCheck(climbBlocker.Entity, entity.Position + Vector2.UnitX * (float)dir))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040007B8 RID: 1976
		public bool Blocking = true;

		// Token: 0x040007B9 RID: 1977
		public bool Edge;
	}
}
