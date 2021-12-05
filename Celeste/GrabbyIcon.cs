using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001D4 RID: 468
	public class GrabbyIcon : Entity
	{
		// Token: 0x06000FC9 RID: 4041 RVA: 0x00043084 File Offset: 0x00041284
		public GrabbyIcon()
		{
			base.Depth = -1000001;
			base.Tag = (Tags.Global | Tags.PauseUpdate | Tags.TransitionUpdate);
			base.Add(this.wiggler = Wiggler.Create(0.1f, 0.3f, null, false, false));
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x000430EC File Offset: 0x000412EC
		public override void Update()
		{
			base.Update();
			bool flag = false;
			if (!base.SceneAs<Level>().InCutscene)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && !entity.Dead && Settings.Instance.GrabMode == GrabModes.Toggle && Input.GrabCheck)
				{
					flag = true;
				}
			}
			if (flag != this.enabled)
			{
				this.enabled = flag;
				this.wiggler.Start();
			}
		}

		// Token: 0x06000FCB RID: 4043 RVA: 0x0004315C File Offset: 0x0004135C
		public override void Render()
		{
			if (this.enabled)
			{
				Vector2 scale = Vector2.One * (1f + this.wiggler.Value * 0.2f);
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					GFX.Game["util/glove"].DrawJustified(new Vector2(entity.X, entity.Y - 16f), new Vector2(0.5f, 1f), Color.White, scale);
				}
			}
		}

		// Token: 0x04000B2F RID: 2863
		private bool enabled;

		// Token: 0x04000B30 RID: 2864
		private Wiggler wiggler;
	}
}
