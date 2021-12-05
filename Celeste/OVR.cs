using System;
using System.IO;
using Monocle;

namespace Celeste
{
	// Token: 0x02000241 RID: 577
	public static class OVR
	{
		// Token: 0x1700014D RID: 333
		// (get) Token: 0x0600124F RID: 4687 RVA: 0x0005FF28 File Offset: 0x0005E128
		// (set) Token: 0x06001250 RID: 4688 RVA: 0x0005FF2F File Offset: 0x0005E12F
		public static bool Loaded { get; private set; }

		// Token: 0x06001251 RID: 4689 RVA: 0x0005FF37 File Offset: 0x0005E137
		public static void Load()
		{
			OVR.Atlas = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Overworld"), Atlas.AtlasDataFormat.PackerNoAtlas);
			OVR.Loaded = true;
		}

		// Token: 0x06001252 RID: 4690 RVA: 0x0005FF5E File Offset: 0x0005E15E
		public static void Unload()
		{
			OVR.Atlas.Dispose();
			OVR.Atlas = null;
			OVR.Loaded = false;
		}

		// Token: 0x04000DFE RID: 3582
		public static Atlas Atlas;
	}
}
