using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200017E RID: 382
	[Tracked(false)]
	public class PlaybackBillboard : Entity
	{
		// Token: 0x06000D85 RID: 3461 RVA: 0x0002F2B4 File Offset: 0x0002D4B4
		public PlaybackBillboard(EntityData e, Vector2 offset)
		{
			this.Position = e.Position + offset;
			base.Collider = new Hitbox((float)e.Width, (float)e.Height, 0f, 0f);
			base.Depth = 9010;
			base.Add(new CustomBloom(new Action(this.RenderBloom)));
		}

		// Token: 0x06000D86 RID: 3462 RVA: 0x0002F31E File Offset: 0x0002D51E
		public override void Added(Scene scene)
		{
			base.Added(scene);
			scene.Add(new PlaybackBillboard.FG(this));
		}

		// Token: 0x06000D87 RID: 3463 RVA: 0x0002F334 File Offset: 0x0002D534
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			MTexture mtexture = GFX.Game["scenery/tvSlices"];
			this.tiles = new MTexture[mtexture.Width / 8, mtexture.Height / 8];
			for (int i = 0; i < mtexture.Width / 8; i++)
			{
				for (int j = 0; j < mtexture.Height / 8; j++)
				{
					this.tiles[i, j] = mtexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
				}
			}
			int num = (int)(base.Width / 8f);
			int num2 = (int)(base.Height / 8f);
			for (int k = -1; k <= num; k++)
			{
				this.AutoTile(k, -1);
				this.AutoTile(k, num2);
			}
			for (int l = 0; l < num2; l++)
			{
				this.AutoTile(-1, l);
				this.AutoTile(num, l);
			}
		}

		// Token: 0x06000D88 RID: 3464 RVA: 0x0002F41C File Offset: 0x0002D61C
		private void AutoTile(int x, int y)
		{
			if (this.Empty(x, y))
			{
				bool flag = !this.Empty(x - 1, y);
				bool flag2 = !this.Empty(x + 1, y);
				bool flag3 = !this.Empty(x, y - 1);
				bool flag4 = !this.Empty(x, y + 1);
				bool flag5 = !this.Empty(x - 1, y - 1);
				bool flag6 = !this.Empty(x + 1, y - 1);
				bool flag7 = !this.Empty(x - 1, y + 1);
				bool flag8 = !this.Empty(x + 1, y + 1);
				if (!flag2 && !flag4 && flag8)
				{
					this.Tile(x, y, this.tiles[0, 0]);
					return;
				}
				if (!flag && !flag4 && flag7)
				{
					this.Tile(x, y, this.tiles[2, 0]);
					return;
				}
				if (!flag3 && !flag2 && flag6)
				{
					this.Tile(x, y, this.tiles[0, 2]);
					return;
				}
				if (!flag3 && !flag && flag5)
				{
					this.Tile(x, y, this.tiles[2, 2]);
					return;
				}
				if (flag2 && flag4)
				{
					this.Tile(x, y, this.tiles[3, 0]);
					return;
				}
				if (flag && flag4)
				{
					this.Tile(x, y, this.tiles[4, 0]);
					return;
				}
				if (flag2 && flag3)
				{
					this.Tile(x, y, this.tiles[3, 2]);
					return;
				}
				if (flag && flag3)
				{
					this.Tile(x, y, this.tiles[4, 2]);
					return;
				}
				if (flag4)
				{
					this.Tile(x, y, this.tiles[1, 0]);
					return;
				}
				if (flag2)
				{
					this.Tile(x, y, this.tiles[0, 1]);
					return;
				}
				if (flag)
				{
					this.Tile(x, y, this.tiles[2, 1]);
					return;
				}
				if (flag3)
				{
					this.Tile(x, y, this.tiles[1, 2]);
				}
			}
		}

		// Token: 0x06000D89 RID: 3465 RVA: 0x0002F618 File Offset: 0x0002D818
		private void Tile(int x, int y, MTexture tile)
		{
			base.Add(new Image(tile)
			{
				Position = new Vector2((float)x, (float)y) * 8f
			});
		}

		// Token: 0x06000D8A RID: 3466 RVA: 0x0002F64C File Offset: 0x0002D84C
		private bool Empty(int x, int y)
		{
			return !base.Scene.CollideCheck<PlaybackBillboard>(new Rectangle((int)base.X + x * 8, (int)base.Y + y * 8, 8, 8));
		}

		// Token: 0x06000D8B RID: 3467 RVA: 0x0002F679 File Offset: 0x0002D879
		public override void Update()
		{
			base.Update();
			if (base.Scene.OnInterval(0.1f))
			{
				this.Seed += 1U;
			}
		}

		// Token: 0x06000D8C RID: 3468 RVA: 0x0002F6A1 File Offset: 0x0002D8A1
		private void RenderBloom()
		{
			Draw.Rect(base.Collider, Color.White * 0.4f);
		}

		// Token: 0x06000D8D RID: 3469 RVA: 0x0002F6C0 File Offset: 0x0002D8C0
		public override void Render()
		{
			base.Render();
			uint seed = this.Seed;
			Draw.Rect(base.Collider, PlaybackBillboard.BackgroundColor);
			PlaybackBillboard.DrawNoise(base.Collider.Bounds, ref seed, Color.White * 0.1f);
		}

		// Token: 0x06000D8E RID: 3470 RVA: 0x0002F70C File Offset: 0x0002D90C
		public static void DrawNoise(Rectangle bounds, ref uint seed, Color color)
		{
			MTexture mtexture = GFX.Game["util/noise"];
			Vector2 vector = new Vector2(PlaybackBillboard.PseudoRandRange(ref seed, 0f, (float)(mtexture.Width / 2)), PlaybackBillboard.PseudoRandRange(ref seed, 0f, (float)(mtexture.Height / 2)));
			Vector2 vector2 = new Vector2((float)mtexture.Width, (float)mtexture.Height) / 2f;
			for (float num = 0f; num < (float)bounds.Width; num += vector2.X)
			{
				float num2 = Math.Min((float)bounds.Width - num, vector2.X);
				for (float num3 = 0f; num3 < (float)bounds.Height; num3 += vector2.Y)
				{
					float num4 = Math.Min((float)bounds.Height - num3, vector2.Y);
					int x = (int)((float)mtexture.ClipRect.X + vector.X);
					int y = (int)((float)mtexture.ClipRect.Y + vector.Y);
					Rectangle value = new Rectangle(x, y, (int)num2, (int)num4);
					Draw.SpriteBatch.Draw(mtexture.Texture.Texture, new Vector2((float)bounds.X + num, (float)bounds.Y + num3), new Rectangle?(value), color);
				}
			}
		}

		// Token: 0x06000D8F RID: 3471 RVA: 0x0002F85B File Offset: 0x0002DA5B
		private static uint PseudoRand(ref uint seed)
		{
			seed ^= seed << 13;
			seed ^= seed >> 17;
			return seed;
		}

		// Token: 0x06000D90 RID: 3472 RVA: 0x0002F873 File Offset: 0x0002DA73
		private static float PseudoRandRange(ref uint seed, float min, float max)
		{
			return min + PlaybackBillboard.PseudoRand(ref seed) % 1000U / 1000f * (max - min);
		}

		// Token: 0x040008C4 RID: 2244
		public const int BGDepth = 9010;

		// Token: 0x040008C5 RID: 2245
		public static readonly Color BackgroundColor = Color.Lerp(Color.DarkSlateBlue, Color.Black, 0.6f);

		// Token: 0x040008C6 RID: 2246
		public uint Seed;

		// Token: 0x040008C7 RID: 2247
		private MTexture[,] tiles;

		// Token: 0x0200045A RID: 1114
		private class FG : Entity
		{
			// Token: 0x060021EA RID: 8682 RVA: 0x000E72EC File Offset: 0x000E54EC
			public FG(PlaybackBillboard parent)
			{
				this.Parent = parent;
				base.Depth = this.Parent.Depth - 5;
			}

			// Token: 0x060021EB RID: 8683 RVA: 0x000E7310 File Offset: 0x000E5510
			public override void Render()
			{
				uint seed = this.Parent.Seed;
				PlaybackBillboard.DrawNoise(this.Parent.Collider.Bounds, ref seed, Color.White * 0.1f);
				int num = (int)this.Parent.Y;
				while ((float)num < this.Parent.Bottom)
				{
					float scale = 0.05f + (1f + (float)Math.Sin((double)((float)num / 16f + base.Scene.TimeActive * 2f))) / 2f * 0.2f;
					Draw.Line(this.Parent.X, (float)num, this.Parent.X + this.Parent.Width, (float)num, Color.Teal * scale);
					num += 2;
				}
			}

			// Token: 0x040021DA RID: 8666
			public PlaybackBillboard Parent;
		}
	}
}
