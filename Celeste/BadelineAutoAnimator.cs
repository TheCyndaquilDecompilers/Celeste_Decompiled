using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000153 RID: 339
	public class BadelineAutoAnimator : Component
	{
		// Token: 0x06000C4E RID: 3150 RVA: 0x000286A2 File Offset: 0x000268A2
		public BadelineAutoAnimator() : base(true, false)
		{
		}

		// Token: 0x06000C4F RID: 3151 RVA: 0x000286B4 File Offset: 0x000268B4
		public override void Added(Entity entity)
		{
			base.Added(entity);
			entity.Add(this.pop = Wiggler.Create(0.5f, 4f, delegate(float f)
			{
				Sprite sprite = base.Entity.Get<Sprite>();
				if (sprite != null)
				{
					sprite.Scale = new Vector2((float)Math.Sign(sprite.Scale.X), 1f) * (1f + 0.25f * f);
				}
			}, false, false));
		}

		// Token: 0x06000C50 RID: 3152 RVA: 0x000286F4 File Offset: 0x000268F4
		public override void Removed(Entity entity)
		{
			entity.Remove(this.pop);
			base.Removed(entity);
		}

		// Token: 0x06000C51 RID: 3153 RVA: 0x00028709 File Offset: 0x00026909
		public void SetReturnToAnimation(string anim)
		{
			this.lastAnimation = anim;
		}

		// Token: 0x06000C52 RID: 3154 RVA: 0x00028714 File Offset: 0x00026914
		public override void Update()
		{
			Sprite sprite = base.Entity.Get<Sprite>();
			if (base.Scene != null && sprite != null)
			{
				bool flag = false;
				Textbox entity = base.Scene.Tracker.GetEntity<Textbox>();
				if (this.Enabled && entity != null && entity.PortraitName.IsIgnoreCase(new string[]
				{
					"badeline"
				}))
				{
					if (entity.PortraitAnimation.IsIgnoreCase(new string[]
					{
						"scoff"
					}))
					{
						if (!this.wasSyncingSprite)
						{
							this.lastAnimation = sprite.CurrentAnimationID;
						}
						sprite.Play("laugh", false, false);
						flag = (this.wasSyncingSprite = true);
					}
					else if (entity.PortraitAnimation.IsIgnoreCase(new string[]
					{
						"yell",
						"freakA",
						"freakB",
						"freakC"
					}))
					{
						if (!this.wasSyncingSprite)
						{
							this.pop.Start();
							this.lastAnimation = sprite.CurrentAnimationID;
						}
						sprite.Play("angry", false, false);
						flag = (this.wasSyncingSprite = true);
					}
				}
				if (this.wasSyncingSprite && !flag)
				{
					this.wasSyncingSprite = false;
					if (string.IsNullOrEmpty(this.lastAnimation) || this.lastAnimation == "spin")
					{
						this.lastAnimation = "fallSlow";
					}
					if (sprite.CurrentAnimationID == "angry")
					{
						this.pop.Start();
					}
					sprite.Play(this.lastAnimation, false, false);
				}
			}
		}

		// Token: 0x040007B4 RID: 1972
		public bool Enabled = true;

		// Token: 0x040007B5 RID: 1973
		private string lastAnimation;

		// Token: 0x040007B6 RID: 1974
		private bool wasSyncingSprite;

		// Token: 0x040007B7 RID: 1975
		private Wiggler pop;
	}
}
