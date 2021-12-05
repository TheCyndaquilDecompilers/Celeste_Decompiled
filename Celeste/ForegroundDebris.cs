using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002CD RID: 717
	public class ForegroundDebris : Entity
	{
		// Token: 0x06001634 RID: 5684 RVA: 0x000824DC File Offset: 0x000806DC
		public ForegroundDebris(Vector2 position) : base(position)
		{
			this.start = this.Position;
			base.Depth = -999900;
			string key = "scenery/fgdebris/" + Calc.Random.Choose("rock_a", "rock_b");
			List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(key);
			atlasSubtextures.Reverse();
			foreach (MTexture texture in atlasSubtextures)
			{
				Image img = new Image(texture);
				img.CenterOrigin();
				base.Add(img);
				SineWave sine = new SineWave(0.4f, 0f);
				sine.Randomize();
				sine.OnUpdate = delegate(float f)
				{
					img.Y = sine.Value * 2f;
				};
				base.Add(sine);
			}
			this.parallax = 0.05f + Calc.Random.NextFloat(0.08f);
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x000825FC File Offset: 0x000807FC
		public ForegroundDebris(EntityData data, Vector2 offset) : this(data.Position + offset)
		{
		}

		// Token: 0x06001636 RID: 5686 RVA: 0x00082610 File Offset: 0x00080810
		public override void Render()
		{
			Vector2 value = base.SceneAs<Level>().Camera.Position + new Vector2(320f, 180f) / 2f - this.start;
			Vector2 position = this.Position;
			this.Position -= value * this.parallax;
			base.Render();
			this.Position = position;
		}

		// Token: 0x04001294 RID: 4756
		private Vector2 start;

		// Token: 0x04001295 RID: 4757
		private float parallax;
	}
}
