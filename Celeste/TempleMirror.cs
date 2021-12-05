using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000227 RID: 551
	public class TempleMirror : Entity
	{
		// Token: 0x060011A7 RID: 4519 RVA: 0x00057860 File Offset: 0x00055A60
		public TempleMirror(EntityData e, Vector2 offset) : base(e.Position + offset)
		{
			this.size = new Vector2((float)e.Width, (float)e.Height);
			base.Depth = 9500;
			base.Collider = new Hitbox((float)e.Width, (float)e.Height, 0f, 0f);
			base.Add(this.surface = new MirrorSurface(null));
			this.surface.ReflectionOffset = new Vector2(e.Float("reflectX", 0f), e.Float("reflectY", 0f));
			this.surface.OnRender = delegate()
			{
				Draw.Rect(base.X + 2f, base.Y + 2f, this.size.X - 4f, this.size.Y - 4f, this.surface.ReflectionColor);
			};
			MTexture mtexture = GFX.Game["scenery/templemirror"];
			for (int i = 0; i < mtexture.Width / 8; i++)
			{
				for (int j = 0; j < mtexture.Height / 8; j++)
				{
					this.frame[i, j] = mtexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
				}
			}
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x00057995 File Offset: 0x00055B95
		public override void Added(Scene scene)
		{
			base.Added(scene);
			scene.Add(new TempleMirror.Frame(this));
		}

		// Token: 0x060011A9 RID: 4521 RVA: 0x000579AC File Offset: 0x00055BAC
		public override void Render()
		{
			Draw.Rect(base.X + 3f, base.Y + 3f, this.size.X - 6f, this.size.Y - 6f, this.color);
		}

		// Token: 0x04000D3E RID: 3390
		private readonly Color color = Calc.HexToColor("05070e");

		// Token: 0x04000D3F RID: 3391
		private readonly Vector2 size;

		// Token: 0x04000D40 RID: 3392
		private MTexture[,] frame = new MTexture[3, 3];

		// Token: 0x04000D41 RID: 3393
		private MirrorSurface surface;

		// Token: 0x02000544 RID: 1348
		private class Frame : Entity
		{
			// Token: 0x060025CE RID: 9678 RVA: 0x000FA306 File Offset: 0x000F8506
			public Frame(TempleMirror mirror)
			{
				this.mirror = mirror;
				base.Depth = 8995;
			}

			// Token: 0x060025CF RID: 9679 RVA: 0x000FA320 File Offset: 0x000F8520
			public override void Render()
			{
				this.Position = this.mirror.Position;
				MTexture[,] frame = this.mirror.frame;
				Vector2 size = this.mirror.size;
				frame[0, 0].Draw(this.Position + new Vector2(0f, 0f));
				frame[2, 0].Draw(this.Position + new Vector2(size.X - 8f, 0f));
				frame[0, 2].Draw(this.Position + new Vector2(0f, size.Y - 8f));
				frame[2, 2].Draw(this.Position + new Vector2(size.X - 8f, size.Y - 8f));
				int num = 1;
				while ((float)num < size.X / 8f - 1f)
				{
					frame[1, 0].Draw(this.Position + new Vector2((float)(num * 8), 0f));
					frame[1, 2].Draw(this.Position + new Vector2((float)(num * 8), size.Y - 8f));
					num++;
				}
				int num2 = 1;
				while ((float)num2 < size.Y / 8f - 1f)
				{
					frame[0, 1].Draw(this.Position + new Vector2(0f, (float)(num2 * 8)));
					frame[2, 1].Draw(this.Position + new Vector2(size.X - 8f, (float)(num2 * 8)));
					num2++;
				}
			}

			// Token: 0x040025C1 RID: 9665
			private TempleMirror mirror;
		}
	}
}
