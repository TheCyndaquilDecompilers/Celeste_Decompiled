using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000181 RID: 385
	[Tracked(false)]
	public class CutsceneNode : Entity
	{
		// Token: 0x06000D9E RID: 3486 RVA: 0x0002FE68 File Offset: 0x0002E068
		public CutsceneNode(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			this.Name = data.Attr("nodeName", "");
		}

		// Token: 0x06000D9F RID: 3487 RVA: 0x0002FE94 File Offset: 0x0002E094
		public static CutsceneNode Find(string name)
		{
			foreach (Entity entity in Engine.Scene.Tracker.GetEntities<CutsceneNode>())
			{
				CutsceneNode cutsceneNode = (CutsceneNode)entity;
				if (cutsceneNode.Name != null && cutsceneNode.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return cutsceneNode;
				}
			}
			return null;
		}

		// Token: 0x040008D9 RID: 2265
		public string Name;
	}
}
