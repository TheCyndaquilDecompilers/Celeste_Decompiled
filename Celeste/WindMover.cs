using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002BF RID: 703
	[Tracked(false)]
	public class WindMover : Component
	{
		// Token: 0x060015A7 RID: 5543 RVA: 0x0007CD2B File Offset: 0x0007AF2B
		public WindMover(Action<Vector2> move) : base(false, false)
		{
			this.Move = move;
		}

		// Token: 0x040011C8 RID: 4552
		public Action<Vector2> Move;
	}
}
