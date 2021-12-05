using System;
using Monocle;

namespace Celeste
{
	// Token: 0x02000262 RID: 610
	public class OshiroSprite : Sprite
	{
		// Token: 0x060012FC RID: 4860 RVA: 0x00067319 File Offset: 0x00065519
		public OshiroSprite(int facing)
		{
			this.Scale.X = (float)facing;
			GFX.SpriteBank.CreateOn(this, "oshiro");
		}

		// Token: 0x060012FD RID: 4861 RVA: 0x00067350 File Offset: 0x00065550
		public override void Added(Entity entity)
		{
			base.Added(entity);
			entity.Add(this.wiggler = Wiggler.Create(0.3f, 2f, delegate(float f)
			{
				this.Scale.X = (float)Math.Sign(this.Scale.X) * (1f + f * 0.2f);
				this.Scale.Y = 1f - f * 0.2f;
			}, false, false));
		}

		// Token: 0x060012FE RID: 4862 RVA: 0x00067390 File Offset: 0x00065590
		public override void Update()
		{
			base.Update();
			if (this.AllowSpriteChanges)
			{
				Textbox entity = base.Scene.Tracker.GetEntity<Textbox>();
				if (entity != null)
				{
					if (entity.PortraitName.Equals("oshiro", StringComparison.OrdinalIgnoreCase) && entity.PortraitAnimation.StartsWith("side", StringComparison.OrdinalIgnoreCase))
					{
						if (base.CurrentAnimationID.Equals("idle"))
						{
							this.Pop("side", true);
						}
					}
					else if (base.CurrentAnimationID.Equals("side"))
					{
						this.Pop("idle", true);
					}
				}
			}
			if (this.AllowTurnInvisible && this.Visible)
			{
				Level level = base.Scene as Level;
				this.Visible = (base.RenderPosition.X > (float)(level.Bounds.Left - 8) && base.RenderPosition.Y > (float)(level.Bounds.Top - 8) && base.RenderPosition.X < (float)(level.Bounds.Right + 8) && base.RenderPosition.Y < (float)(level.Bounds.Bottom + 16));
			}
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x000674C7 File Offset: 0x000656C7
		public void Wiggle()
		{
			this.wiggler.Start();
		}

		// Token: 0x06001300 RID: 4864 RVA: 0x000674D4 File Offset: 0x000656D4
		public void Pop(string name, bool flip)
		{
			if (!base.CurrentAnimationID.Equals(name))
			{
				base.Play(name, false, false);
				if (flip)
				{
					this.Scale.X = -this.Scale.X;
					if (this.Scale.X < 0f)
					{
						Audio.Play("event:/char/oshiro/chat_turn_left", base.Entity.Position);
					}
					else
					{
						Audio.Play("event:/char/oshiro/chat_turn_right", base.Entity.Position);
					}
				}
				this.wiggler.Start();
			}
		}

		// Token: 0x04000EF4 RID: 3828
		public bool AllowSpriteChanges = true;

		// Token: 0x04000EF5 RID: 3829
		public bool AllowTurnInvisible = true;

		// Token: 0x04000EF6 RID: 3830
		private Wiggler wiggler;
	}
}
