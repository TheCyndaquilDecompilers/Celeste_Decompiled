using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000251 RID: 593
	public class MountainState
	{
		// Token: 0x06001294 RID: 4756 RVA: 0x00062945 File Offset: 0x00060B45
		public MountainState(VirtualTexture terrainTexture, VirtualTexture buildingsTexture, VirtualTexture skyboxTexture, Color fogColor)
		{
			this.TerrainTexture = terrainTexture;
			this.BuildingsTexture = buildingsTexture;
			this.Skybox = new Skybox(skyboxTexture, 25f);
			this.FogColor = fogColor;
		}

		// Token: 0x04000E71 RID: 3697
		public Skybox Skybox;

		// Token: 0x04000E72 RID: 3698
		public VirtualTexture TerrainTexture;

		// Token: 0x04000E73 RID: 3699
		public VirtualTexture BuildingsTexture;

		// Token: 0x04000E74 RID: 3700
		public Color FogColor;
	}
}
