using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200014D RID: 333
	[Tracked(false)]
	public class GoldenBlock : Solid
	{
		// Token: 0x06000C23 RID: 3107 RVA: 0x00026AE8 File Offset: 0x00024CE8
		public GoldenBlock(Vector2 position, float width, float height) : base(position, width, height, false)
		{
			this.startY = base.Y;
			this.berry = new Image(GFX.Game["collectables/goldberry/idle00"]);
			this.berry.CenterOrigin();
			this.berry.Position = new Vector2(width / 2f, height / 2f);
			MTexture mtexture = GFX.Game["objects/goldblock"];
			this.nineSlice = new MTexture[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					this.nineSlice[i, j] = mtexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
				}
			}
			base.Depth = -10000;
			base.Add(new LightOcclude(1f));
			base.Add(new MirrorSurface(null));
			this.SurfaceSoundIndex = 32;
		}

		// Token: 0x06000C24 RID: 3108 RVA: 0x00026BD3 File Offset: 0x00024DD3
		public GoldenBlock(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height)
		{
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x00026BF8 File Offset: 0x00024DF8
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			this.Visible = false;
			this.Collidable = false;
			this.renderLerp = 1f;
			bool flag = false;
			foreach (Strawberry strawberry in scene.Entities.FindAll<Strawberry>())
			{
				if (strawberry.Golden && strawberry.Follower.Leader != null)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06000C26 RID: 3110 RVA: 0x00026C90 File Offset: 0x00024E90
		public override void Update()
		{
			base.Update();
			if (!this.Visible)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.X > base.X - 80f)
				{
					this.Visible = true;
					this.Collidable = true;
					this.renderLerp = 1f;
				}
			}
			if (this.Visible)
			{
				this.renderLerp = Calc.Approach(this.renderLerp, 0f, Engine.DeltaTime * 3f);
			}
			if (base.HasPlayerRider())
			{
				this.sinkTimer = 0.1f;
			}
			else if (this.sinkTimer > 0f)
			{
				this.sinkTimer -= Engine.DeltaTime;
			}
			if (this.sinkTimer > 0f)
			{
				this.yLerp = Calc.Approach(this.yLerp, 1f, 1f * Engine.DeltaTime);
			}
			else
			{
				this.yLerp = Calc.Approach(this.yLerp, 0f, 1f * Engine.DeltaTime);
			}
			float y = MathHelper.Lerp(this.startY, this.startY + 12f, Ease.SineInOut(this.yLerp));
			base.MoveToY(y);
		}

		// Token: 0x06000C27 RID: 3111 RVA: 0x00026DCC File Offset: 0x00024FCC
		private void DrawBlock(Vector2 offset, Color color)
		{
			float num = base.Collider.Width / 8f - 1f;
			float num2 = base.Collider.Height / 8f - 1f;
			int num3 = 0;
			while ((float)num3 <= num)
			{
				int num4 = 0;
				while ((float)num4 <= num2)
				{
					int num5 = ((float)num3 < num) ? Math.Min(num3, 1) : 2;
					int num6 = ((float)num4 < num2) ? Math.Min(num4, 1) : 2;
					this.nineSlice[num5, num6].Draw(this.Position + offset + base.Shake + new Vector2((float)(num3 * 8), (float)(num4 * 8)), Vector2.Zero, color);
					num4++;
				}
				num3++;
			}
		}

		// Token: 0x06000C28 RID: 3112 RVA: 0x00026E88 File Offset: 0x00025088
		public override void Render()
		{
			Level level = base.Scene as Level;
			Vector2 value = new Vector2(0f, ((float)level.Bounds.Bottom - this.startY + 32f) * Ease.CubeIn(this.renderLerp));
			Vector2 position = this.Position;
			this.Position += value;
			this.DrawBlock(new Vector2(-1f, 0f), Color.Black);
			this.DrawBlock(new Vector2(1f, 0f), Color.Black);
			this.DrawBlock(new Vector2(0f, -1f), Color.Black);
			this.DrawBlock(new Vector2(0f, 1f), Color.Black);
			this.DrawBlock(Vector2.Zero, Color.White);
			this.berry.Color = Color.White;
			this.berry.RenderPosition = base.Center;
			this.berry.Render();
			this.Position = position;
		}

		// Token: 0x04000778 RID: 1912
		private MTexture[,] nineSlice;

		// Token: 0x04000779 RID: 1913
		private Image berry;

		// Token: 0x0400077A RID: 1914
		private float startY;

		// Token: 0x0400077B RID: 1915
		private float yLerp;

		// Token: 0x0400077C RID: 1916
		private float sinkTimer;

		// Token: 0x0400077D RID: 1917
		private float renderLerp;
	}
}
