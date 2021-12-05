using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000225 RID: 549
	public class NPC05_Theo_Mirror : NPC
	{
		// Token: 0x060011A2 RID: 4514 RVA: 0x000576A0 File Offset: 0x000558A0
		public NPC05_Theo_Mirror(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("theo"));
			this.IdleAnim = "idle";
			this.MoveAnim = "walk";
			this.Visible = false;
			base.Add(new MirrorReflection
			{
				IgnoreEntityVisible = true
			});
			this.Sprite.Scale.X = 1f;
			this.Maxspeed = 48f;
		}

		// Token: 0x060011A3 RID: 4515 RVA: 0x00057723 File Offset: 0x00055923
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (base.Session.GetFlag("theoInMirror"))
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x00057744 File Offset: 0x00055944
		public override void Update()
		{
			base.Update();
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (!this.started && entity != null && entity.X > base.X - 64f)
			{
				this.started = true;
				base.Scene.Add(new CS05_TheoInMirror(this, entity));
			}
		}

		// Token: 0x04000D3D RID: 3389
		private bool started;
	}
}
