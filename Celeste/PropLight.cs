using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002CA RID: 714
	public class PropLight : Entity
	{
		// Token: 0x06001625 RID: 5669 RVA: 0x00081B7B File Offset: 0x0007FD7B
		public PropLight(Vector2 position, Color color, float alpha) : base(position)
		{
			base.Add(new VertexLight(color, alpha, 128, 256));
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x00081B9C File Offset: 0x0007FD9C
		public PropLight(EntityData data, Vector2 offset) : this(data.Position + offset, data.HexColor("color", default(Color)), data.Float("alpha", 0f))
		{
		}
	}
}
