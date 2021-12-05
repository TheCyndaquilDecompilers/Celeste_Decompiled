using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000351 RID: 849
	public class Bridge : Entity
	{
		// Token: 0x06001A9E RID: 6814 RVA: 0x000ABE7C File Offset: 0x000AA07C
		public Bridge(Vector2 position, int width, float gapStartX, float gapEndX) : base(position)
		{
			this.width = width;
			this.gapStartX = gapStartX;
			this.gapEndX = gapEndX;
			this.tileSizes.Add(new Rectangle(0, 0, 16, 52));
			this.tileSizes.Add(new Rectangle(16, 0, 8, 52));
			this.tileSizes.Add(new Rectangle(24, 0, 8, 52));
			this.tileSizes.Add(new Rectangle(32, 0, 8, 52));
			this.tileSizes.Add(new Rectangle(40, 0, 8, 52));
			this.tileSizes.Add(new Rectangle(48, 0, 8, 52));
			this.tileSizes.Add(new Rectangle(56, 0, 8, 52));
			this.tileSizes.Add(new Rectangle(64, 0, 8, 52));
			this.tileSizes.Add(new Rectangle(72, 0, 8, 52));
			this.tileSizes.Add(new Rectangle(80, 0, 16, 52));
			this.tileSizes.Add(new Rectangle(96, 0, 8, 52));
			base.Add(this.collapseSfx = new SoundSource());
		}

		// Token: 0x06001A9F RID: 6815 RVA: 0x000ABFB8 File Offset: 0x000AA1B8
		public Bridge(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Nodes[0].X, data.Nodes[1].X)
		{
		}

		// Token: 0x06001AA0 RID: 6816 RVA: 0x000ABFF4 File Offset: 0x000AA1F4
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = (scene as Level);
			this.tiles = new List<BridgeTile>();
			this.gapStartX += (float)this.level.Bounds.Left;
			this.gapEndX += (float)this.level.Bounds.Left;
			Calc.PushRandom(1);
			Vector2 position = this.Position;
			int num = 0;
			while (position.X < base.X + (float)this.width)
			{
				Rectangle rectangle;
				if (num >= 2 && num <= 7)
				{
					rectangle = this.tileSizes[2 + Calc.Random.Next(6)];
				}
				else
				{
					rectangle = this.tileSizes[num];
				}
				if (position.X < this.gapStartX || position.X >= this.gapEndX)
				{
					BridgeTile bridgeTile = new BridgeTile(position, rectangle);
					this.tiles.Add(bridgeTile);
					this.level.Add(bridgeTile);
				}
				position.X += (float)rectangle.Width;
				num = (num + 1) % this.tileSizes.Count;
			}
			Calc.PopRandom();
		}

		// Token: 0x06001AA1 RID: 6817 RVA: 0x000AC124 File Offset: 0x000AA324
		public override void Update()
		{
			base.Update();
			Player entity = this.level.Tracker.GetEntity<Player>();
			if (entity == null || entity.Dead)
			{
				this.collapseSfx.Stop(true);
			}
			if (!this.canCollapse)
			{
				if (entity != null && entity.X >= base.X + 112f)
				{
					Audio.SetMusic("event:/music/lvl0/bridge", true, true);
					this.collapseSfx.Play("event:/game/00_prologue/bridge_rumble_loop", null, 0f);
					this.canCollapse = true;
					this.canEndCollapseA = true;
					this.canEndCollapseB = true;
					for (int i = 0; i < 11; i++)
					{
						this.tiles[0].Fall(Calc.Random.Range(0.1f, 0.5f));
						this.tiles.RemoveAt(0);
					}
					return;
				}
			}
			else if (this.tiles.Count > 0)
			{
				if (entity != null)
				{
					if (this.canEndCollapseA && entity.X > base.X + (float)this.width - 216f)
					{
						this.canEndCollapseA = false;
						for (int j = 0; j < 5; j++)
						{
							this.tiles[this.tiles.Count - 8].Fall(Calc.Random.Range(0.1f, 0.5f));
							this.tiles.RemoveAt(this.tiles.Count - 8);
						}
						return;
					}
					if (this.canEndCollapseB && entity.X > base.X + (float)this.width - 104f)
					{
						this.canEndCollapseB = false;
						int num = 0;
						while (num < 7 && this.tiles.Count > 0)
						{
							this.tiles[this.tiles.Count - 1].Fall(Calc.Random.Range(0.1f, 0.3f));
							this.tiles.RemoveAt(this.tiles.Count - 1);
							num++;
						}
						return;
					}
					if (this.collapseTimer <= 0f)
					{
						this.tiles[0].Fall(0.2f);
						this.tiles.RemoveAt(0);
						this.collapseTimer = 0.2f;
						return;
					}
					this.collapseTimer -= Engine.DeltaTime;
					if (this.tiles.Count >= 5 && entity.X >= this.tiles[4].X)
					{
						int index = 0;
						this.tiles[index].Fall(0.2f);
						this.tiles.RemoveAt(index);
						return;
					}
				}
			}
			else if (!this.ending)
			{
				this.ending = true;
				this.StopCollapseLoop();
			}
		}

		// Token: 0x06001AA2 RID: 6818 RVA: 0x000AC3DE File Offset: 0x000AA5DE
		public void StopCollapseLoop()
		{
			this.collapseSfx.Stop(true);
		}

		// Token: 0x04001745 RID: 5957
		private List<BridgeTile> tiles;

		// Token: 0x04001746 RID: 5958
		private Level level;

		// Token: 0x04001747 RID: 5959
		private bool canCollapse;

		// Token: 0x04001748 RID: 5960
		private bool canEndCollapseA;

		// Token: 0x04001749 RID: 5961
		private bool canEndCollapseB;

		// Token: 0x0400174A RID: 5962
		private float collapseTimer;

		// Token: 0x0400174B RID: 5963
		private int width;

		// Token: 0x0400174C RID: 5964
		private List<Rectangle> tileSizes = new List<Rectangle>();

		// Token: 0x0400174D RID: 5965
		private bool ending;

		// Token: 0x0400174E RID: 5966
		private float gapStartX;

		// Token: 0x0400174F RID: 5967
		private float gapEndX;

		// Token: 0x04001750 RID: 5968
		private SoundSource collapseSfx;
	}
}
