using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000266 RID: 614
	public class NPC00_Granny : NPC
	{
		// Token: 0x0600132B RID: 4907 RVA: 0x00068090 File Offset: 0x00066290
		public NPC00_Granny(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("granny"));
			this.Sprite.Play("idle", false, false);
			base.Add(this.LaughSfx = new GrannyLaughSfx(this.Sprite));
		}

		// Token: 0x0600132C RID: 4908 RVA: 0x000680F0 File Offset: 0x000662F0
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if ((scene as Level).Session.GetFlag("granny"))
			{
				this.Sprite.Play("laugh", false, false);
			}
			scene.Add(this.Hahaha = new Hahaha(this.Position + new Vector2(8f, -4f), "", false, null));
			this.Hahaha.Enabled = false;
		}

		// Token: 0x0600132D RID: 4909 RVA: 0x00068178 File Offset: 0x00066378
		public override void Update()
		{
			Player entity = this.Level.Tracker.GetEntity<Player>();
			if (entity != null && !base.Session.GetFlag("granny") && !this.talking)
			{
				int num = this.Level.Bounds.Left + 96;
				if (entity.OnGround(1) && entity.X >= (float)num && entity.X <= base.X + 16f && Math.Abs(entity.Y - base.Y) < 4f && entity.Facing == (Facings)Math.Sign(base.X - entity.X))
				{
					this.talking = true;
					base.Scene.Add(new CS00_Granny(this, entity));
				}
			}
			this.Hahaha.Enabled = (this.Sprite.CurrentAnimationID == "laugh");
			base.Update();
		}

		// Token: 0x04000F19 RID: 3865
		public Hahaha Hahaha;

		// Token: 0x04000F1A RID: 3866
		public GrannyLaughSfx LaughSfx;

		// Token: 0x04000F1B RID: 3867
		private bool talking;
	}
}
