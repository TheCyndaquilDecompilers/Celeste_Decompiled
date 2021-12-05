using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001B7 RID: 439
	public class SummitCloud : Entity
	{
		// Token: 0x06000F41 RID: 3905 RVA: 0x0003DD50 File Offset: 0x0003BF50
		public SummitCloud(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Depth = -10550;
			this.diff = Calc.Random.Range(0.1f, 0.2f);
			List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("scenery/summitclouds/cloud");
			this.image = new Image(Calc.Random.Choose(atlasSubtextures));
			this.image.CenterOrigin();
			this.image.Scale.X = (float)Calc.Random.Choose(-1, 1);
			base.Add(this.image);
			SineWave sineWave = new SineWave(Calc.Random.Range(0.05f, 0.15f), 0f);
			sineWave.Randomize();
			sineWave.OnUpdate = delegate(float f)
			{
				this.image.Y = f * 8f;
			};
			base.Add(sineWave);
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000F42 RID: 3906 RVA: 0x0003DE30 File Offset: 0x0003C030
		private Vector2 RenderPosition
		{
			get
			{
				Vector2 value = (base.Scene as Level).Camera.Position + new Vector2(160f, 90f);
				Vector2 value2 = this.Position + new Vector2(128f, 64f) / 2f - value;
				return this.Position + value2 * (0.1f + this.diff);
			}
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x0003DEB0 File Offset: 0x0003C0B0
		public override void Render()
		{
			Vector2 position = this.Position;
			this.Position = this.RenderPosition;
			base.Render();
			this.Position = position;
		}

		// Token: 0x04000AAC RID: 2732
		private Image image;

		// Token: 0x04000AAD RID: 2733
		private float diff;
	}
}
