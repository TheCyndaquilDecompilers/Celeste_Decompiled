using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002F7 RID: 759
	[Tracked(true)]
	public class Billboard : Component
	{
		// Token: 0x06001771 RID: 6001 RVA: 0x0008F82C File Offset: 0x0008DA2C
		public Billboard(MTexture texture, Vector3 position, Vector2? size = null, Color? color = null, Vector2? scale = null) : base(true, true)
		{
			this.Texture = texture;
			this.Position = position;
			this.Size = ((size != null) ? size.Value : Vector2.One);
			this.Color = ((color != null) ? color.Value : Color.White);
			this.Scale = ((scale != null) ? scale.Value : Vector2.One);
		}

		// Token: 0x04001420 RID: 5152
		public MTexture Texture;

		// Token: 0x04001421 RID: 5153
		public Vector3 Position;

		// Token: 0x04001422 RID: 5154
		public Color Color = Color.White;

		// Token: 0x04001423 RID: 5155
		public Vector2 Size = Vector2.One;

		// Token: 0x04001424 RID: 5156
		public Vector2 Scale = Vector2.One;

		// Token: 0x04001425 RID: 5157
		public Action BeforeRender;
	}
}
