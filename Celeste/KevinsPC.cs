using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001C3 RID: 451
	public class KevinsPC : Actor
	{
		// Token: 0x06000F73 RID: 3955 RVA: 0x0003F604 File Offset: 0x0003D804
		public KevinsPC(Vector2 position) : base(position)
		{
			base.Add(this.image = new Image(GFX.Game["objects/kevinspc/pc"]));
			this.image.JustifyOrigin(0.5f, 1f);
			base.Depth = 8999;
			this.spectogram = GFX.Game["objects/kevinspc/spectogram"];
			this.subtex = this.spectogram.GetSubtexture(0, 0, 32, 18, this.subtex);
			base.Add(this.sfx = new SoundSource("event:/new_content/env/local/kevinpc"));
			this.sfx.Position = new Vector2(0f, -16f);
			this.timer = 0f;
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x0003F6CC File Offset: 0x0003D8CC
		public KevinsPC(EntityData data, Vector2 offset) : this(data.Position + offset)
		{
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x0003F6E0 File Offset: 0x0003D8E0
		public override bool IsRiding(Solid solid)
		{
			return base.Scene.CollideCheck(new Rectangle((int)base.X - 4, (int)base.Y, 8, 2), solid);
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x0003F708 File Offset: 0x0003D908
		public override void Update()
		{
			base.Update();
			this.timer += Engine.DeltaTime;
			int num = this.spectogram.Width - 32;
			int x = (int)(this.timer * ((float)num / 22f) % (float)num);
			this.subtex = this.spectogram.GetSubtexture(x, 0, 32, 18, this.subtex);
		}

		// Token: 0x06000F77 RID: 3959 RVA: 0x0003F770 File Offset: 0x0003D970
		public override void Render()
		{
			base.Render();
			if (this.subtex != null)
			{
				this.subtex.Draw(this.Position + new Vector2(-16f, -39f));
				Draw.Rect(base.X - 16f, base.Y - 39f, 32f, 18f, Color.Black * 0.25f);
			}
		}

		// Token: 0x04000AD7 RID: 2775
		private Image image;

		// Token: 0x04000AD8 RID: 2776
		private MTexture spectogram;

		// Token: 0x04000AD9 RID: 2777
		private MTexture subtex;

		// Token: 0x04000ADA RID: 2778
		private SoundSource sfx;

		// Token: 0x04000ADB RID: 2779
		private float timer;
	}
}
