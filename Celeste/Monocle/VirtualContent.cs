using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x0200013D RID: 317
	public static class VirtualContent
	{
		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000B81 RID: 2945 RVA: 0x00020BF9 File Offset: 0x0001EDF9
		public static int Count
		{
			get
			{
				return VirtualContent.assets.Count;
			}
		}

		// Token: 0x06000B82 RID: 2946 RVA: 0x00020C08 File Offset: 0x0001EE08
		public static VirtualTexture CreateTexture(string path)
		{
			VirtualTexture virtualTexture = new VirtualTexture(path);
			VirtualContent.assets.Add(virtualTexture);
			return virtualTexture;
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x00020C28 File Offset: 0x0001EE28
		public static VirtualTexture CreateTexture(string name, int width, int height, Color color)
		{
			VirtualTexture virtualTexture = new VirtualTexture(name, width, height, color);
			VirtualContent.assets.Add(virtualTexture);
			return virtualTexture;
		}

		// Token: 0x06000B84 RID: 2948 RVA: 0x00020C4C File Offset: 0x0001EE4C
		public static VirtualRenderTarget CreateRenderTarget(string name, int width, int height, bool depth = false, bool preserve = true, int multiSampleCount = 0)
		{
			VirtualRenderTarget virtualRenderTarget = new VirtualRenderTarget(name, width, height, multiSampleCount, depth, preserve);
			VirtualContent.assets.Add(virtualRenderTarget);
			return virtualRenderTarget;
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x00020C74 File Offset: 0x0001EE74
		public static void BySize()
		{
			Dictionary<int, Dictionary<int, int>> dictionary = new Dictionary<int, Dictionary<int, int>>();
			foreach (VirtualAsset virtualAsset in VirtualContent.assets)
			{
				if (!dictionary.ContainsKey(virtualAsset.Width))
				{
					dictionary.Add(virtualAsset.Width, new Dictionary<int, int>());
				}
				if (!dictionary[virtualAsset.Width].ContainsKey(virtualAsset.Height))
				{
					dictionary[virtualAsset.Width].Add(virtualAsset.Height, 0);
				}
				Dictionary<int, int> dictionary2 = dictionary[virtualAsset.Width];
				int height = virtualAsset.Height;
				int num = dictionary2[height];
				dictionary2[height] = num + 1;
			}
			foreach (KeyValuePair<int, Dictionary<int, int>> keyValuePair in dictionary)
			{
				foreach (KeyValuePair<int, int> keyValuePair2 in keyValuePair.Value)
				{
					Console.WriteLine(string.Concat(new object[]
					{
						keyValuePair.Key,
						"x",
						keyValuePair2.Key,
						": ",
						keyValuePair2.Value
					}));
				}
			}
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x00020E08 File Offset: 0x0001F008
		public static void ByName()
		{
			foreach (VirtualAsset virtualAsset in VirtualContent.assets)
			{
				Console.WriteLine(string.Concat(new object[]
				{
					virtualAsset.Name,
					"[",
					virtualAsset.Width,
					"x",
					virtualAsset.Height,
					"]"
				}));
			}
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x00020EA0 File Offset: 0x0001F0A0
		internal static void Remove(VirtualAsset asset)
		{
			VirtualContent.assets.Remove(asset);
		}

		// Token: 0x06000B88 RID: 2952 RVA: 0x00020EB0 File Offset: 0x0001F0B0
		internal static void Reload()
		{
			if (VirtualContent.reloading)
			{
				foreach (VirtualAsset virtualAsset in VirtualContent.assets)
				{
					virtualAsset.Reload();
				}
			}
			VirtualContent.reloading = false;
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x00020F0C File Offset: 0x0001F10C
		internal static void Unload()
		{
			foreach (VirtualAsset virtualAsset in VirtualContent.assets)
			{
				virtualAsset.Unload();
			}
			VirtualContent.reloading = true;
		}

		// Token: 0x040006D4 RID: 1748
		private static List<VirtualAsset> assets = new List<VirtualAsset>();

		// Token: 0x040006D5 RID: 1749
		private static bool reloading;
	}
}
