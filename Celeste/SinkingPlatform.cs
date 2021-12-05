using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000333 RID: 819
	public class SinkingPlatform : JumpThru
	{
		// Token: 0x060019B2 RID: 6578 RVA: 0x000A5944 File Offset: 0x000A3B44
		public SinkingPlatform(Vector2 position, int width) : base(position, width, false)
		{
			this.startY = base.Y;
			base.Depth = 1;
			this.SurfaceSoundIndex = 15;
			base.Add(this.shaker = new Shaker(false, null));
			base.Add(new LightOcclude(0.2f));
			base.Add(this.downSfx = new SoundSource());
			base.Add(this.upSfx = new SoundSource());
		}

		// Token: 0x060019B3 RID: 6579 RVA: 0x000A59C3 File Offset: 0x000A3BC3
		public SinkingPlatform(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width)
		{
		}

		// Token: 0x060019B4 RID: 6580 RVA: 0x000A59E0 File Offset: 0x000A3BE0
		public override void Added(Scene scene)
		{
			base.Added(scene);
			MTexture mtexture = GFX.Game["objects/woodPlatform/" + AreaData.Get(scene).WoodPlatform];
			this.textures = new MTexture[mtexture.Width / 8];
			for (int i = 0; i < this.textures.Length; i++)
			{
				this.textures[i] = mtexture.GetSubtexture(i * 8, 0, 8, 8, null);
			}
			scene.Add(new SinkingPlatformLine(this.Position + new Vector2(base.Width / 2f, base.Height / 2f)));
		}

		// Token: 0x060019B5 RID: 6581 RVA: 0x000A5A84 File Offset: 0x000A3C84
		public override void Render()
		{
			Vector2 value = this.shaker.Value;
			this.textures[0].Draw(this.Position + value);
			int num = 8;
			while ((float)num < base.Width - 8f)
			{
				this.textures[1].Draw(this.Position + value + new Vector2((float)num, 0f));
				num += 8;
			}
			this.textures[3].Draw(this.Position + value + new Vector2(base.Width - 8f, 0f));
			this.textures[2].Draw(this.Position + value + new Vector2(base.Width / 2f - 4f, 0f));
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x000A5B68 File Offset: 0x000A3D68
		public override void Update()
		{
			base.Update();
			Player playerRider = base.GetPlayerRider();
			if (playerRider != null)
			{
				if (this.riseTimer <= 0f)
				{
					if (base.ExactPosition.Y <= this.startY)
					{
						Audio.Play("event:/game/03_resort/platform_vert_start", this.Position);
					}
					this.shaker.ShakeFor(0.15f, false);
				}
				this.riseTimer = 0.1f;
				this.speed = Calc.Approach(this.speed, playerRider.Ducking ? 60f : 30f, 400f * Engine.DeltaTime);
			}
			else if (this.riseTimer > 0f)
			{
				this.riseTimer -= Engine.DeltaTime;
				this.speed = Calc.Approach(this.speed, 45f, 600f * Engine.DeltaTime);
			}
			else
			{
				this.speed = Calc.Approach(this.speed, -50f, 400f * Engine.DeltaTime);
			}
			if (this.speed > 0f)
			{
				if (!this.downSfx.Playing)
				{
					this.downSfx.Play("event:/game/03_resort/platform_vert_down_loop", null, 0f);
				}
				this.downSfx.Param("ducking", (float)((playerRider != null && playerRider.Ducking) ? 1 : 0));
				if (this.upSfx.Playing)
				{
					this.upSfx.Stop(true);
				}
				base.MoveV(this.speed * Engine.DeltaTime);
				return;
			}
			if (this.speed < 0f && base.ExactPosition.Y > this.startY)
			{
				if (!this.upSfx.Playing)
				{
					this.upSfx.Play("event:/game/03_resort/platform_vert_up_loop", null, 0f);
				}
				if (this.downSfx.Playing)
				{
					this.downSfx.Stop(true);
				}
				base.MoveTowardsY(this.startY, -this.speed * Engine.DeltaTime);
				if (base.ExactPosition.Y <= this.startY)
				{
					this.upSfx.Stop(true);
					Audio.Play("event:/game/03_resort/platform_vert_end", this.Position);
					this.shaker.ShakeFor(0.1f, false);
				}
			}
		}

		// Token: 0x04001673 RID: 5747
		private float speed;

		// Token: 0x04001674 RID: 5748
		private float startY;

		// Token: 0x04001675 RID: 5749
		private float riseTimer;

		// Token: 0x04001676 RID: 5750
		private MTexture[] textures;

		// Token: 0x04001677 RID: 5751
		private Shaker shaker;

		// Token: 0x04001678 RID: 5752
		private SoundSource downSfx;

		// Token: 0x04001679 RID: 5753
		private SoundSource upSfx;
	}
}
