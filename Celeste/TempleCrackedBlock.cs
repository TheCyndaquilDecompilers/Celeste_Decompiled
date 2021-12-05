using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200025F RID: 607
	[Tracked(false)]
	public class TempleCrackedBlock : Solid
	{
		// Token: 0x060012E2 RID: 4834 RVA: 0x00066380 File Offset: 0x00064580
		public TempleCrackedBlock(EntityID eid, Vector2 position, float width, float height, bool persistent) : base(position, width, height, true)
		{
			this.eid = eid;
			this.persistent = persistent;
			this.Collidable = (this.Visible = false);
			int num = (int)(width / 8f);
			int num2 = (int)(height / 8f);
			List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/temple/breakBlock");
			this.tiles = new MTexture[num, num2, atlasSubtextures.Count];
			this.frames = atlasSubtextures.Count;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					int num3;
					if (i < num / 2 && i < 2)
					{
						num3 = i;
					}
					else if (i >= num / 2 && i >= num - 2)
					{
						num3 = 5 - (num - i - 1);
					}
					else
					{
						num3 = 2 + i % 2;
					}
					int num4;
					if (j < num2 / 2 && j < 2)
					{
						num4 = j;
					}
					else if (j >= num2 / 2 && j >= num2 - 2)
					{
						num4 = 5 - (num2 - j - 1);
					}
					else
					{
						num4 = 2 + j % 2;
					}
					for (int k = 0; k < atlasSubtextures.Count; k++)
					{
						this.tiles[i, j, k] = atlasSubtextures[k].GetSubtexture(num3 * 8, num4 * 8, 8, 8, null);
					}
				}
			}
			base.Add(new LightOcclude(0.5f));
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x000664DB File Offset: 0x000646DB
		public TempleCrackedBlock(EntityID eid, EntityData data, Vector2 offset) : this(eid, data.Position + offset, (float)data.Width, (float)data.Height, data.Bool("persistent", false))
		{
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x0006650C File Offset: 0x0006470C
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (base.CollideCheck<Player>())
			{
				if (this.persistent)
				{
					base.SceneAs<Level>().Session.DoNotLoad.Add(this.eid);
				}
				base.RemoveSelf();
				return;
			}
			this.Collidable = (this.Visible = true);
		}

		// Token: 0x060012E5 RID: 4837 RVA: 0x00066563 File Offset: 0x00064763
		public override void Update()
		{
			base.Update();
			if (this.broken)
			{
				this.frame += Engine.DeltaTime * 15f;
				if (this.frame >= (float)this.frames)
				{
					base.RemoveSelf();
				}
			}
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x000665A0 File Offset: 0x000647A0
		public override void Render()
		{
			int num = (int)this.frame;
			if (num < this.frames)
			{
				int num2 = 0;
				while ((float)num2 < base.Width / 8f)
				{
					int num3 = 0;
					while ((float)num3 < base.Height / 8f)
					{
						this.tiles[num2, num3, num].Draw(this.Position + new Vector2((float)num2, (float)num3) * 8f);
						num3++;
					}
					num2++;
				}
			}
		}

		// Token: 0x060012E7 RID: 4839 RVA: 0x00066620 File Offset: 0x00064820
		public void Break(Vector2 from)
		{
			if (this.persistent)
			{
				base.SceneAs<Level>().Session.DoNotLoad.Add(this.eid);
			}
			Audio.Play("event:/game/05_mirror_temple/crackedwall_vanish", base.Center);
			this.broken = true;
			this.Collidable = false;
			int num = 0;
			while ((float)num < base.Width / 8f)
			{
				int num2 = 0;
				while ((float)num2 < base.Height / 8f)
				{
					base.Scene.Add(Engine.Pooler.Create<Debris>().Init(this.Position + new Vector2((float)(num * 8 + 4), (float)(num2 * 8 + 4)), '1', true).BlastFrom(from));
					num2++;
				}
				num++;
			}
		}

		// Token: 0x04000ED1 RID: 3793
		private EntityID eid;

		// Token: 0x04000ED2 RID: 3794
		private bool persistent;

		// Token: 0x04000ED3 RID: 3795
		private MTexture[,,] tiles;

		// Token: 0x04000ED4 RID: 3796
		private float frame;

		// Token: 0x04000ED5 RID: 3797
		private bool broken;

		// Token: 0x04000ED6 RID: 3798
		private int frames;
	}
}
