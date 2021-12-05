using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001B9 RID: 441
	public class TemplePortalTorch : Entity
	{
		// Token: 0x06000F4D RID: 3917 RVA: 0x0003E278 File Offset: 0x0003C478
		public TemplePortalTorch(Vector2 pos) : base(pos)
		{
			base.Add(this.sprite = new Sprite(GFX.Game, "objects/temple/portal/portaltorch"));
			this.sprite.AddLoop("idle", "", 0f, new int[1]);
			this.sprite.AddLoop("lit", "", 0.08f, new int[]
			{
				1,
				2,
				3,
				4,
				5,
				6
			});
			this.sprite.Play("idle", false, false);
			this.sprite.Origin = new Vector2(32f, 64f);
			base.Depth = 8999;
		}

		// Token: 0x06000F4E RID: 3918 RVA: 0x0003E32C File Offset: 0x0003C52C
		public void Light(int count = 0)
		{
			this.sprite.Play("lit", false, false);
			base.Add(this.bloom = new BloomPoint(1f, 16f));
			base.Add(this.light = new VertexLight(Color.LightSeaGreen, 0f, 32, 128));
			Audio.Play((count == 0) ? "event:/game/05_mirror_temple/mainmirror_torch_lit_1" : "event:/game/05_mirror_temple/mainmirror_torch_lit_2", this.Position);
			base.Add(this.loopSfx = new SoundSource());
			this.loopSfx.Play("event:/game/05_mirror_temple/mainmirror_torch_loop", null, 0f);
		}

		// Token: 0x06000F4F RID: 3919 RVA: 0x0003E3D4 File Offset: 0x0003C5D4
		public override void Update()
		{
			base.Update();
			if (this.bloom != null && this.bloom.Alpha > 0.5f)
			{
				this.bloom.Alpha -= Engine.DeltaTime;
			}
			if (this.light != null && this.light.Alpha < 1f)
			{
				this.light.Alpha = Calc.Approach(this.light.Alpha, 1f, Engine.DeltaTime);
			}
		}

		// Token: 0x04000AB6 RID: 2742
		private Sprite sprite;

		// Token: 0x04000AB7 RID: 2743
		private VertexLight light;

		// Token: 0x04000AB8 RID: 2744
		private BloomPoint bloom;

		// Token: 0x04000AB9 RID: 2745
		private SoundSource loopSfx;
	}
}
