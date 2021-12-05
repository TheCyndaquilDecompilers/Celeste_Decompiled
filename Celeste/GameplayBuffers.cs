using System;
using System.Collections.Generic;
using Monocle;

namespace Celeste
{
	// Token: 0x02000308 RID: 776
	public static class GameplayBuffers
	{
		// Token: 0x06001833 RID: 6195 RVA: 0x00097A08 File Offset: 0x00095C08
		public static void Create()
		{
			GameplayBuffers.Unload();
			GameplayBuffers.Gameplay = GameplayBuffers.Create(320, 180);
			GameplayBuffers.Level = GameplayBuffers.Create(320, 180);
			GameplayBuffers.ResortDust = GameplayBuffers.Create(320, 180);
			GameplayBuffers.Light = GameplayBuffers.Create(320, 180);
			GameplayBuffers.Displacement = GameplayBuffers.Create(320, 180);
			GameplayBuffers.LightBuffer = GameplayBuffers.Create(1024, 1024);
			GameplayBuffers.MirrorSources = GameplayBuffers.Create(384, 244);
			GameplayBuffers.MirrorMasks = GameplayBuffers.Create(384, 244);
			GameplayBuffers.SpeedRings = GameplayBuffers.Create(512, 512);
			GameplayBuffers.Lightning = GameplayBuffers.Create(160, 160);
			GameplayBuffers.TempA = GameplayBuffers.Create(320, 180);
			GameplayBuffers.TempB = GameplayBuffers.Create(320, 180);
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x00097B0C File Offset: 0x00095D0C
		private static VirtualRenderTarget Create(int width, int height)
		{
			VirtualRenderTarget virtualRenderTarget = VirtualContent.CreateRenderTarget("gameplay-buffer-" + GameplayBuffers.all.Count, width, height, false, true, 0);
			GameplayBuffers.all.Add(virtualRenderTarget);
			return virtualRenderTarget;
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x00097B4C File Offset: 0x00095D4C
		public static void Unload()
		{
			foreach (VirtualRenderTarget virtualRenderTarget in GameplayBuffers.all)
			{
				virtualRenderTarget.Dispose();
			}
			GameplayBuffers.all.Clear();
		}

		// Token: 0x0400150C RID: 5388
		public static VirtualRenderTarget Gameplay;

		// Token: 0x0400150D RID: 5389
		public static VirtualRenderTarget Level;

		// Token: 0x0400150E RID: 5390
		public static VirtualRenderTarget ResortDust;

		// Token: 0x0400150F RID: 5391
		public static VirtualRenderTarget LightBuffer;

		// Token: 0x04001510 RID: 5392
		public static VirtualRenderTarget Light;

		// Token: 0x04001511 RID: 5393
		public static VirtualRenderTarget Displacement;

		// Token: 0x04001512 RID: 5394
		public static VirtualRenderTarget MirrorSources;

		// Token: 0x04001513 RID: 5395
		public static VirtualRenderTarget MirrorMasks;

		// Token: 0x04001514 RID: 5396
		public static VirtualRenderTarget SpeedRings;

		// Token: 0x04001515 RID: 5397
		public static VirtualRenderTarget Lightning;

		// Token: 0x04001516 RID: 5398
		public static VirtualRenderTarget TempA;

		// Token: 0x04001517 RID: 5399
		public static VirtualRenderTarget TempB;

		// Token: 0x04001518 RID: 5400
		private static List<VirtualRenderTarget> all = new List<VirtualRenderTarget>();
	}
}
