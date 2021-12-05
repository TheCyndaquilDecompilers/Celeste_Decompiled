using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200020C RID: 524
	public class AnimatedTiles : Component
	{
		// Token: 0x06001111 RID: 4369 RVA: 0x00051D07 File Offset: 0x0004FF07
		public AnimatedTiles(int columns, int rows, AnimatedTilesBank bank) : base(true, true)
		{
			this.tiles = new VirtualMap<List<AnimatedTiles.Tile>>(columns, rows, null);
			this.Bank = bank;
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x00051D3C File Offset: 0x0004FF3C
		public void Set(int x, int y, string name, float scaleX = 1f, float scaleY = 1f)
		{
			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			AnimatedTilesBank.Animation animation = this.Bank.AnimationsByName[name];
			List<AnimatedTiles.Tile> list = this.tiles[x, y];
			if (list == null)
			{
				list = (this.tiles[x, y] = new List<AnimatedTiles.Tile>());
			}
			list.Add(new AnimatedTiles.Tile
			{
				AnimationID = animation.ID,
				Frame = (float)Calc.Random.Next(animation.Frames.Length),
				Scale = new Vector2(scaleX, scaleY)
			});
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x00051DCC File Offset: 0x0004FFCC
		public Rectangle GetClippedRenderTiles(int extend)
		{
			Vector2 vector = base.Entity.Position + this.Position;
			int num;
			int num2;
			int num3;
			int num4;
			if (this.ClipCamera == null)
			{
				num = -extend;
				num2 = -extend;
				num3 = this.tiles.Columns + extend;
				num4 = this.tiles.Rows + extend;
			}
			else
			{
				Camera clipCamera = this.ClipCamera;
				num = (int)Math.Max(0.0, Math.Floor((double)((clipCamera.Left - vector.X) / 8f)) - (double)extend);
				num2 = (int)Math.Max(0.0, Math.Floor((double)((clipCamera.Top - vector.Y) / 8f)) - (double)extend);
				num3 = (int)Math.Min((double)this.tiles.Columns, Math.Ceiling((double)((clipCamera.Right - vector.X) / 8f)) + (double)extend);
				num4 = (int)Math.Min((double)this.tiles.Rows, Math.Ceiling((double)((clipCamera.Bottom - vector.Y) / 8f)) + (double)extend);
			}
			num = Math.Max(num, 0);
			num2 = Math.Max(num2, 0);
			num3 = Math.Min(num3, this.tiles.Columns);
			num4 = Math.Min(num4, this.tiles.Rows);
			return new Rectangle(num, num2, num3 - num, num4 - num2);
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x00051F28 File Offset: 0x00050128
		public override void Update()
		{
			Rectangle clippedRenderTiles = this.GetClippedRenderTiles(1);
			for (int i = clippedRenderTiles.Left; i < clippedRenderTiles.Right; i++)
			{
				for (int j = clippedRenderTiles.Top; j < clippedRenderTiles.Bottom; j++)
				{
					List<AnimatedTiles.Tile> list = this.tiles[i, j];
					if (list != null)
					{
						for (int k = 0; k < list.Count; k++)
						{
							AnimatedTilesBank.Animation animation = this.Bank.Animations[list[k].AnimationID];
							list[k].Frame += Engine.DeltaTime / animation.Delay;
						}
					}
				}
			}
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x00051FD9 File Offset: 0x000501D9
		public override void Render()
		{
			this.RenderAt(base.Entity.Position + this.Position);
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x00051FF8 File Offset: 0x000501F8
		public void RenderAt(Vector2 position)
		{
			Rectangle clippedRenderTiles = this.GetClippedRenderTiles(1);
			Color color = this.Color * this.Alpha;
			for (int i = clippedRenderTiles.Left; i < clippedRenderTiles.Right; i++)
			{
				for (int j = clippedRenderTiles.Top; j < clippedRenderTiles.Bottom; j++)
				{
					List<AnimatedTiles.Tile> list = this.tiles[i, j];
					if (list != null)
					{
						for (int k = 0; k < list.Count; k++)
						{
							AnimatedTiles.Tile tile = list[k];
							AnimatedTilesBank.Animation animation = this.Bank.Animations[tile.AnimationID];
							animation.Frames[(int)tile.Frame % animation.Frames.Length].Draw(position + animation.Offset + new Vector2((float)i + 0.5f, (float)j + 0.5f) * 8f, animation.Origin, color, tile.Scale);
						}
					}
				}
			}
		}

		// Token: 0x04000CB7 RID: 3255
		public Camera ClipCamera;

		// Token: 0x04000CB8 RID: 3256
		public Vector2 Position;

		// Token: 0x04000CB9 RID: 3257
		public Color Color = Color.White;

		// Token: 0x04000CBA RID: 3258
		public float Alpha = 1f;

		// Token: 0x04000CBB RID: 3259
		public AnimatedTilesBank Bank;

		// Token: 0x04000CBC RID: 3260
		private VirtualMap<List<AnimatedTiles.Tile>> tiles;

		// Token: 0x02000517 RID: 1303
		private class Tile
		{
			// Token: 0x040024F4 RID: 9460
			public int AnimationID;

			// Token: 0x040024F5 RID: 9461
			public float Frame;

			// Token: 0x040024F6 RID: 9462
			public Vector2 Scale;
		}
	}
}
