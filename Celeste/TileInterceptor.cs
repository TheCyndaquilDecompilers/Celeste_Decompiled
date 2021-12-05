using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002BD RID: 701
	[Tracked(false)]
	public class TileInterceptor : Component
	{
		// Token: 0x060015A3 RID: 5539 RVA: 0x0007CC12 File Offset: 0x0007AE12
		public TileInterceptor(Action<MTexture, Vector2, Point> intercepter, bool highPriority) : base(false, false)
		{
			this.Intercepter = intercepter;
			this.HighPriority = highPriority;
		}

		// Token: 0x060015A4 RID: 5540 RVA: 0x0007CC2C File Offset: 0x0007AE2C
		public TileInterceptor(TileGrid applyToGrid, bool highPriority) : base(false, false)
		{
			this.Intercepter = delegate(MTexture t, Vector2 v, Point p)
			{
				applyToGrid.Tiles[p.X, p.Y] = t;
			};
			this.HighPriority = highPriority;
		}

		// Token: 0x060015A5 RID: 5541 RVA: 0x0007CC68 File Offset: 0x0007AE68
		public static bool TileCheck(Scene scene, MTexture tile, Vector2 at)
		{
			at += Vector2.One * 4f;
			TileInterceptor tileInterceptor = null;
			List<Component> components = scene.Tracker.GetComponents<TileInterceptor>();
			for (int i = components.Count - 1; i >= 0; i--)
			{
				TileInterceptor tileInterceptor2 = (TileInterceptor)components[i];
				if ((tileInterceptor == null || tileInterceptor2.HighPriority) && tileInterceptor2.Entity.CollidePoint(at))
				{
					tileInterceptor = tileInterceptor2;
					if (tileInterceptor2.HighPriority)
					{
						break;
					}
				}
			}
			if (tileInterceptor != null)
			{
				Point arg = new Point((int)((at.X - tileInterceptor.Entity.X) / 8f), (int)((at.Y - tileInterceptor.Entity.Y) / 8f));
				tileInterceptor.Intercepter(tile, at, arg);
				return true;
			}
			return false;
		}

		// Token: 0x040011C1 RID: 4545
		public Action<MTexture, Vector2, Point> Intercepter;

		// Token: 0x040011C2 RID: 4546
		public bool HighPriority;
	}
}
