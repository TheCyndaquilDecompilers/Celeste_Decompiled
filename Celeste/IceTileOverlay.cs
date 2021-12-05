using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000391 RID: 913
	public class IceTileOverlay : Entity
	{
		// Token: 0x06001D9C RID: 7580 RVA: 0x000CE1D8 File Offset: 0x000CC3D8
		public IceTileOverlay()
		{
			base.Depth = -10010;
			base.Tag = Tags.Global;
			this.Visible = false;
			this.surfaces = GFX.Game.GetAtlasSubtextures("scenery/iceSurface");
		}

		// Token: 0x06001D9D RID: 7581 RVA: 0x000CE218 File Offset: 0x000CC418
		public override void Update()
		{
			base.Update();
			this.alpha = Calc.Approach(this.alpha, (float)(((base.Scene as Level).CoreMode == Session.CoreModes.Cold) ? 1 : 0), Engine.DeltaTime * 4f);
			this.Visible = (this.alpha > 0f);
		}

		// Token: 0x06001D9E RID: 7582 RVA: 0x000CE274 File Offset: 0x000CC474
		public override void Render()
		{
			Level level = base.Scene as Level;
			Camera camera = level.Camera;
			Color color = Color.White * this.alpha;
			int num = (int)(Math.Floor((double)((camera.Left - level.SolidTiles.X) / 8f)) - 1.0);
			int num2 = (int)(Math.Floor((double)((camera.Top - level.SolidTiles.Y) / 8f)) - 1.0);
			int num3 = (int)(Math.Ceiling((double)((camera.Right - level.SolidTiles.X) / 8f)) + 1.0);
			int num4 = (int)(Math.Ceiling((double)((camera.Bottom - level.SolidTiles.Y) / 8f)) + 1.0);
			for (int i = num; i < num3; i++)
			{
				for (int j = num2; j < num4; j++)
				{
					if (level.SolidsData.SafeCheck(i, j) != '0' && level.SolidsData.SafeCheck(i, j - 1) == '0')
					{
						Vector2 position = level.SolidTiles.Position + new Vector2((float)i, (float)j) * 8f;
						int index = (i * 5 + j * 17) % this.surfaces.Count;
						this.surfaces[index].Draw(position, Vector2.Zero, color);
					}
				}
			}
		}

		// Token: 0x04001E65 RID: 7781
		private List<MTexture> surfaces;

		// Token: 0x04001E66 RID: 7782
		private float alpha;
	}
}
