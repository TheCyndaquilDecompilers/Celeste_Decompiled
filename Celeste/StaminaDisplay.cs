using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000322 RID: 802
	public class StaminaDisplay : Component
	{
		// Token: 0x0600194A RID: 6474 RVA: 0x00029C68 File Offset: 0x00027E68
		public StaminaDisplay() : base(true, false)
		{
		}

		// Token: 0x0600194B RID: 6475 RVA: 0x000A241D File Offset: 0x000A061D
		public override void Added(Entity entity)
		{
			base.Added(entity);
			this.level = base.SceneAs<Level>();
			this.player = base.EntityAs<Player>();
			this.drawStamina = this.player.Stamina;
		}

		// Token: 0x0600194C RID: 6476 RVA: 0x000A2450 File Offset: 0x000A0650
		public override void Update()
		{
			base.Update();
			this.drawStamina = Calc.Approach(this.drawStamina, this.player.Stamina, 300f * Engine.DeltaTime);
			if (this.drawStamina < 110f && this.drawStamina > 0f)
			{
				this.displayTimer = 0.75f;
				return;
			}
			if (this.displayTimer > 0f)
			{
				this.displayTimer -= Engine.DeltaTime;
			}
		}

		// Token: 0x0600194D RID: 6477 RVA: 0x000A24D0 File Offset: 0x000A06D0
		public void RenderHUD()
		{
			if (this.displayTimer > 0f)
			{
				Vector2 vector = this.level.Camera.CameraToScreen(this.player.Position + new Vector2(0f, -18f)) * 6f;
				Color color;
				if (this.drawStamina < 20f)
				{
					color = Color.Red;
				}
				else
				{
					color = Color.Lime;
				}
				Draw.Rect(vector.X - 48f - 1f, vector.Y - 6f - 1f, 98f, 14f, Color.Black);
				Draw.Rect(vector.X - 48f, vector.Y - 6f, 96f * (this.drawStamina / 110f), 12f, color);
			}
		}

		// Token: 0x04001609 RID: 5641
		private Player player;

		// Token: 0x0400160A RID: 5642
		private float drawStamina;

		// Token: 0x0400160B RID: 5643
		private float displayTimer;

		// Token: 0x0400160C RID: 5644
		private Level level;
	}
}
