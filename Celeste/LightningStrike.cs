using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001F9 RID: 505
	public class LightningStrike : Entity
	{
		// Token: 0x06001096 RID: 4246 RVA: 0x0004C4E4 File Offset: 0x0004A6E4
		public LightningStrike(Vector2 position, int seed, float height, float delay = 0f)
		{
			this.Position = position;
			base.Depth = 10010;
			this.rand = new Random(seed);
			this.strikeHeight = height;
			base.Add(new Coroutine(this.Routine(delay), true));
		}

		// Token: 0x06001097 RID: 4247 RVA: 0x0004C530 File Offset: 0x0004A730
		private IEnumerator Routine(float delay)
		{
			if (delay > 0f)
			{
				yield return delay;
			}
			this.scale = 1f;
			this.GenerateStikeNodes(-1, 10f, null);
			int num;
			for (int i = 0; i < 5; i = num + 1)
			{
				this.on = true;
				yield return (1f - (float)i / 5f) * 0.1f;
				this.scale -= 0.2f;
				this.on = false;
				this.strike.Wiggle(this.rand);
				yield return 0.01f;
				num = i;
			}
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x06001098 RID: 4248 RVA: 0x0004C548 File Offset: 0x0004A748
		private void GenerateStikeNodes(int direction, float size, LightningStrike.Node parent = null)
		{
			if (parent == null)
			{
				parent = (this.strike = new LightningStrike.Node(0f, 0f, size));
			}
			if (parent.Position.Y >= this.strikeHeight)
			{
				return;
			}
			float num = (float)(direction * this.rand.Range(-8, 20));
			float num2 = (float)this.rand.Range(8, 16);
			float size2 = (0.25f + (1f - (parent.Position.Y + num2) / this.strikeHeight) * 0.75f) * size;
			LightningStrike.Node node = new LightningStrike.Node(parent.Position + new Vector2(num, num2), size2);
			parent.Children.Add(node);
			this.GenerateStikeNodes(direction, size, node);
			if (this.rand.Chance(0.1f))
			{
				LightningStrike.Node node2 = new LightningStrike.Node(parent.Position + new Vector2(-num, num2 * 1.5f), size2);
				parent.Children.Add(node2);
				this.GenerateStikeNodes(-direction, size, node2);
			}
		}

		// Token: 0x06001099 RID: 4249 RVA: 0x0004C64E File Offset: 0x0004A84E
		public override void Render()
		{
			if (!this.on)
			{
				return;
			}
			this.strike.Render(this.Position, this.scale);
		}

		// Token: 0x04000C14 RID: 3092
		private bool on;

		// Token: 0x04000C15 RID: 3093
		private float scale;

		// Token: 0x04000C16 RID: 3094
		private Random rand;

		// Token: 0x04000C17 RID: 3095
		private float strikeHeight;

		// Token: 0x04000C18 RID: 3096
		private LightningStrike.Node strike;

		// Token: 0x020004FA RID: 1274
		private class Node
		{
			// Token: 0x060024B0 RID: 9392 RVA: 0x000F4DD7 File Offset: 0x000F2FD7
			public Node(float x, float y, float size) : this(new Vector2(x, y), size)
			{
			}

			// Token: 0x060024B1 RID: 9393 RVA: 0x000F4DE7 File Offset: 0x000F2FE7
			public Node(Vector2 position, float size)
			{
				this.Position = position;
				this.Children = new List<LightningStrike.Node>();
				this.Size = size;
			}

			// Token: 0x060024B2 RID: 9394 RVA: 0x000F4E08 File Offset: 0x000F3008
			public void Wiggle(Random rand)
			{
				this.Position.X = this.Position.X + (float)rand.Range(-2, 2);
				if (this.Position.Y != 0f)
				{
					this.Position.Y = this.Position.Y + (float)rand.Range(-1, 1);
				}
				foreach (LightningStrike.Node node in this.Children)
				{
					node.Wiggle(rand);
				}
			}

			// Token: 0x060024B3 RID: 9395 RVA: 0x000F4E9C File Offset: 0x000F309C
			public void Render(Vector2 offset, float scale)
			{
				float num = this.Size * scale;
				foreach (LightningStrike.Node node in this.Children)
				{
					Vector2 value = (node.Position - this.Position).SafeNormalize();
					Draw.Line(offset + this.Position, offset + node.Position + value * num * 0.5f, Color.White, num);
					node.Render(offset, scale);
				}
			}

			// Token: 0x04002480 RID: 9344
			public Vector2 Position;

			// Token: 0x04002481 RID: 9345
			public float Size;

			// Token: 0x04002482 RID: 9346
			public List<LightningStrike.Node> Children;
		}
	}
}
