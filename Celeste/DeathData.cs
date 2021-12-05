using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000316 RID: 790
	public class DeathData
	{
		// Token: 0x060018FA RID: 6394 RVA: 0x0009FFB0 File Offset: 0x0009E1B0
		public DeathData(Vector2 position)
		{
			this.Position = position;
			this.Amount = 1;
		}

		// Token: 0x060018FB RID: 6395 RVA: 0x0009FFC6 File Offset: 0x0009E1C6
		public DeathData(DeathData old, Vector2 add)
		{
			this.Position = Vector2.Lerp(old.Position, add, 1f / (float)(old.Amount + 1));
			this.Amount = old.Amount + 1;
		}

		// Token: 0x060018FC RID: 6396 RVA: 0x0009FFFD File Offset: 0x0009E1FD
		public bool CombinesWith(Vector2 position)
		{
			return Vector2.DistanceSquared(this.Position, position) <= 100f;
		}

		// Token: 0x060018FD RID: 6397 RVA: 0x000A0018 File Offset: 0x0009E218
		public void Render()
		{
			float scale = Math.Min(0.7f, 0.3f + 0.1f * (float)this.Amount);
			int num = Math.Min(6, this.Amount + 1);
			Draw.Rect(this.Position.X - (float)num, this.Position.Y - (float)num, (float)(num * 2), (float)(num * 2), Color.Red * scale);
		}

		// Token: 0x04001560 RID: 5472
		public Vector2 Position;

		// Token: 0x04001561 RID: 5473
		public int Amount;
	}
}
