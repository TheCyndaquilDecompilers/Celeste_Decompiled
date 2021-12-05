using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000260 RID: 608
	public class DreamHeartGem : Entity
	{
		// Token: 0x060012E8 RID: 4840 RVA: 0x000666E0 File Offset: 0x000648E0
		public DreamHeartGem(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Add(this.sprite = GFX.SpriteBank.Create("heartgem0"));
			this.sprite.Color = Color.White * 0.25f;
			this.sprite.Play("spin", false, false);
			base.Add(new BloomPoint(0.5f, 16f));
			base.Add(new VertexLight(Color.Aqua, 1f, 32, 64));
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x00066778 File Offset: 0x00064978
		public override void Render()
		{
			int num = 0;
			while ((float)num < this.sprite.Height)
			{
				this.sprite.DrawSubrect(new Vector2((float)Math.Sin((double)(base.Scene.TimeActive * 2f + (float)num * 0.4f)) * 2f, (float)num), new Rectangle(0, num, (int)this.sprite.Width, 1));
				num++;
			}
		}

		// Token: 0x04000ED7 RID: 3799
		private Sprite sprite;
	}
}
