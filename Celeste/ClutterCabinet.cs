using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000290 RID: 656
	[Tracked(false)]
	public class ClutterCabinet : Entity
	{
		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06001458 RID: 5208 RVA: 0x0006EA63 File Offset: 0x0006CC63
		// (set) Token: 0x06001459 RID: 5209 RVA: 0x0006EA6B File Offset: 0x0006CC6B
		public bool Opened { get; private set; }

		// Token: 0x0600145A RID: 5210 RVA: 0x0006EA74 File Offset: 0x0006CC74
		public ClutterCabinet(Vector2 position) : base(position)
		{
			base.Add(this.sprite = GFX.SpriteBank.Create("clutterCabinet"));
			this.sprite.Position = new Vector2(8f);
			this.sprite.Play("idle", false, false);
			base.Depth = -10001;
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x0006EAD8 File Offset: 0x0006CCD8
		public ClutterCabinet(EntityData data, Vector2 offset) : this(data.Position + offset)
		{
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x0006EAEC File Offset: 0x0006CCEC
		public void Open()
		{
			this.sprite.Play("open", false, false);
			this.Opened = true;
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x0006EB07 File Offset: 0x0006CD07
		public void Close()
		{
			this.sprite.Play("close", false, false);
			this.Opened = false;
		}

		// Token: 0x0400100C RID: 4108
		private Sprite sprite;
	}
}
