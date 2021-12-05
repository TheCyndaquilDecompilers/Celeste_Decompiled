using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000185 RID: 389
	public class NPC10_Gravestone : NPC
	{
		// Token: 0x06000DBB RID: 3515 RVA: 0x000308FC File Offset: 0x0002EAFC
		public NPC10_Gravestone(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			this.boostTarget = (data.FirstNodeNullable(new Vector2?(offset)) ?? Vector2.Zero);
			base.Add(this.talk = new TalkComponent(new Rectangle(-24, -8, 32, 8), new Vector2(-0.5f, -20f), new Action<Player>(this.Interact), null));
			this.talk.PlayerMustBeFacing = false;
		}

		// Token: 0x06000DBC RID: 3516 RVA: 0x00030990 File Offset: 0x0002EB90
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (this.Level.Session.GetFlag("gravestone"))
			{
				this.Level.Add(new BadelineBoost(new Vector2[]
				{
					this.boostTarget
				}, false, false, false, false, false));
				this.talk.RemoveSelf();
			}
		}

		// Token: 0x06000DBD RID: 3517 RVA: 0x000309EE File Offset: 0x0002EBEE
		private void Interact(Player player)
		{
			this.Level.Session.SetFlag("gravestone", true);
			base.Scene.Add(new CS10_Gravestone(player, this, this.boostTarget));
			this.talk.Enabled = false;
		}

		// Token: 0x040008F7 RID: 2295
		private const string Flag = "gravestone";

		// Token: 0x040008F8 RID: 2296
		private Player player;

		// Token: 0x040008F9 RID: 2297
		private Vector2 boostTarget;

		// Token: 0x040008FA RID: 2298
		private TalkComponent talk;
	}
}
