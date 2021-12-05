using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200017C RID: 380
	[Pooled]
	public class TempleBigEyeballShockwave : Entity
	{
		// Token: 0x06000D79 RID: 3449 RVA: 0x0002EE20 File Offset: 0x0002D020
		public TempleBigEyeballShockwave()
		{
			base.Depth = -1000000;
			base.Collider = new Hitbox(48f, 200f, -30f, -100f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			MTexture mtexture = GFX.Game["util/displacementcirclehollow"];
			this.distortionTexture = mtexture.GetSubtexture(0, 0, mtexture.Width / 2, mtexture.Height, null);
			base.Add(new DisplacementRenderHook(new Action(this.RenderDisplacement)));
		}

		// Token: 0x06000D7A RID: 3450 RVA: 0x0002EEBA File Offset: 0x0002D0BA
		public TempleBigEyeballShockwave Init(Vector2 position)
		{
			this.Position = position;
			this.Collidable = true;
			this.distortionAlpha = 0f;
			this.hasHitPlayer = false;
			return this;
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x0002EEE0 File Offset: 0x0002D0E0
		public override void Update()
		{
			base.Update();
			base.X -= 300f * Engine.DeltaTime;
			this.distortionAlpha = Calc.Approach(this.distortionAlpha, 1f, Engine.DeltaTime * 4f);
			if (base.X < (float)(base.SceneAs<Level>().Bounds.Left - 20))
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x0002EF51 File Offset: 0x0002D151
		private void RenderDisplacement()
		{
			this.distortionTexture.DrawCentered(this.Position, Color.White * 0.8f * this.distortionAlpha, new Vector2(0.9f, 1.5f));
		}

		// Token: 0x06000D7D RID: 3453 RVA: 0x0002EF90 File Offset: 0x0002D190
		private void OnPlayer(Player player)
		{
			if (player.StateMachine.State != 2)
			{
				player.Speed.X = -100f;
				if (player.Speed.Y > 30f)
				{
					player.Speed.Y = 30f;
				}
				if (!this.hasHitPlayer)
				{
					Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
					Audio.Play("event:/game/05_mirror_temple/eye_pulse", player.Position);
					this.hasHitPlayer = true;
				}
			}
		}

		// Token: 0x040008B9 RID: 2233
		private MTexture distortionTexture;

		// Token: 0x040008BA RID: 2234
		private float distortionAlpha;

		// Token: 0x040008BB RID: 2235
		private bool hasHitPlayer;
	}
}
