using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200015D RID: 349
	[Tracked(false)]
	public class SolidOnInvinciblePlayer : Component
	{
		// Token: 0x06000C74 RID: 3188 RVA: 0x00029C68 File Offset: 0x00027E68
		public SolidOnInvinciblePlayer() : base(true, false)
		{
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x00029C74 File Offset: 0x00027E74
		public override void Added(Entity entity)
		{
			base.Added(entity);
			Audio.Play("event:/game/general/assist_nonsolid_in", entity.Center);
			this.wasCollidable = entity.Collidable;
			this.wasVisible = entity.Visible;
			entity.Collidable = false;
			entity.Visible = false;
			if (entity.Scene != null)
			{
				entity.Scene.Add(this.outline = new SolidOnInvinciblePlayer.Outline(this));
			}
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x00029CE4 File Offset: 0x00027EE4
		public override void Update()
		{
			base.Update();
			base.Entity.Collidable = true;
			if (!base.Entity.CollideCheck<Player>() && !base.Entity.CollideCheck<TheoCrystal>())
			{
				base.RemoveSelf();
				return;
			}
			base.Entity.Collidable = false;
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x00029D30 File Offset: 0x00027F30
		public override void Removed(Entity entity)
		{
			Audio.Play("event:/game/general/assist_nonsolid_out", entity.Center);
			entity.Collidable = this.wasCollidable;
			entity.Visible = this.wasVisible;
			if (this.outline != null)
			{
				this.outline.RemoveSelf();
			}
			base.Removed(entity);
		}

		// Token: 0x040007E0 RID: 2016
		private bool wasCollidable;

		// Token: 0x040007E1 RID: 2017
		private bool wasVisible;

		// Token: 0x040007E2 RID: 2018
		private SolidOnInvinciblePlayer.Outline outline;

		// Token: 0x020003E4 RID: 996
		private class Outline : Entity
		{
			// Token: 0x06001F59 RID: 8025 RVA: 0x000D9121 File Offset: 0x000D7321
			public Outline(SolidOnInvinciblePlayer parent)
			{
				this.Parent = parent;
				base.Depth = -10;
			}

			// Token: 0x06001F5A RID: 8026 RVA: 0x000D9138 File Offset: 0x000D7338
			public override void Render()
			{
				if (this.Parent != null && this.Parent.Entity != null)
				{
					Entity entity = this.Parent.Entity;
					int num = (int)entity.Left;
					int num2 = (int)entity.Right;
					int num3 = (int)entity.Top;
					int num4 = (int)entity.Bottom;
					Draw.Rect((float)(num + 4), (float)(num3 + 4), entity.Width - 8f, entity.Height - 8f, Color.White * 0.25f);
					for (float num5 = (float)num; num5 < (float)(num2 - 3); num5 += 3f)
					{
						Draw.Line(num5, (float)num3, num5 + 2f, (float)num3, Color.White);
						Draw.Line(num5, (float)(num4 - 1), num5 + 2f, (float)(num4 - 1), Color.White);
					}
					for (float num6 = (float)num3; num6 < (float)(num4 - 3); num6 += 3f)
					{
						Draw.Line((float)(num + 1), num6, (float)(num + 1), num6 + 2f, Color.White);
						Draw.Line((float)num2, num6, (float)num2, num6 + 2f, Color.White);
					}
					Draw.Rect((float)(num + 1), (float)num3, 1f, 2f, Color.White);
					Draw.Rect((float)(num2 - 2), (float)num3, 2f, 2f, Color.White);
					Draw.Rect((float)num, (float)(num4 - 2), 2f, 2f, Color.White);
					Draw.Rect((float)(num2 - 2), (float)(num4 - 2), 2f, 2f, Color.White);
				}
			}

			// Token: 0x04001FF5 RID: 8181
			public SolidOnInvinciblePlayer Parent;
		}
	}
}
