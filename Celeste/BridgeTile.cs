using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000352 RID: 850
	public class BridgeTile : JumpThru
	{
		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06001AA3 RID: 6819 RVA: 0x000AC3ED File Offset: 0x000AA5ED
		// (set) Token: 0x06001AA4 RID: 6820 RVA: 0x000AC3F5 File Offset: 0x000AA5F5
		public bool Fallen { get; private set; }

		// Token: 0x06001AA5 RID: 6821 RVA: 0x000AC400 File Offset: 0x000AA600
		public BridgeTile(Vector2 position, Rectangle tileSize) : base(position, tileSize.Width, false)
		{
			this.images = new List<Image>();
			if (tileSize.Width == 16)
			{
				int num = 24;
				int i = 0;
				while (i < tileSize.Height)
				{
					Image image;
					base.Add(image = new Image(GFX.Game["scenery/bridge"].GetSubtexture(tileSize.X, i, tileSize.Width, num, null)));
					image.Origin = new Vector2(image.Width / 2f, 0f);
					image.X = image.Width / 2f;
					image.Y = (float)(i - 8);
					this.images.Add(image);
					i += num;
					num = 12;
				}
				return;
			}
			Image image2;
			base.Add(image2 = new Image(GFX.Game["scenery/bridge"].GetSubtexture(tileSize)));
			image2.Origin = new Vector2(image2.Width / 2f, 0f);
			image2.X = image2.Width / 2f;
			image2.Y = -8f;
			this.images.Add(image2);
		}

		// Token: 0x06001AA6 RID: 6822 RVA: 0x000AC52C File Offset: 0x000AA72C
		public override void Update()
		{
			base.Update();
			bool flag = this.images[0].Width == 16f;
			if (this.Fallen)
			{
				if (this.shakeTimer > 0f)
				{
					this.shakeTimer -= Engine.DeltaTime;
					if (base.Scene.OnInterval(0.02f))
					{
						this.shakeOffset = Calc.Random.ShakeVector();
					}
					if (this.shakeTimer <= 0f)
					{
						this.Collidable = false;
						base.SceneAs<Level>().Shake(0.1f);
						Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
						if (flag)
						{
							Audio.Play("event:/game/00_prologue/bridge_support_break", this.Position);
							foreach (Image image in this.images)
							{
								if (image.RenderPosition.Y > base.Y + 4f)
								{
									Dust.Burst(image.RenderPosition, -1.5707964f, 8, null);
								}
							}
						}
					}
					this.images[0].Position = new Vector2(this.images[0].Width / 2f, -8f) + this.shakeOffset;
					return;
				}
				this.colorLerp = Calc.Approach(this.colorLerp, 1f, 10f * Engine.DeltaTime);
				this.images[0].Color = Color.Lerp(Color.White, Color.Gray, this.colorLerp);
				this.shakeOffset = Vector2.Zero;
				if (flag)
				{
					int num = 0;
					foreach (Image image2 in this.images)
					{
						image2.Rotation -= (float)((num % 2 == 0) ? -1 : 1) * Engine.DeltaTime * (float)num * 2f;
						image2.Y += (float)num * Engine.DeltaTime * 16f;
						num++;
					}
					this.speedY = Calc.Approach(this.speedY, 120f, 600f * Engine.DeltaTime);
				}
				else
				{
					this.speedY = Calc.Approach(this.speedY, 200f, 900f * Engine.DeltaTime);
				}
				base.MoveV(this.speedY * Engine.DeltaTime);
				if (base.Top > 220f)
				{
					base.RemoveSelf();
				}
			}
		}

		// Token: 0x06001AA7 RID: 6823 RVA: 0x000AC7D4 File Offset: 0x000AA9D4
		public void Fall(float timer = 0.2f)
		{
			if (!this.Fallen)
			{
				this.Fallen = true;
				this.shakeTimer = timer;
			}
		}

		// Token: 0x04001751 RID: 5969
		private List<Image> images;

		// Token: 0x04001752 RID: 5970
		private Vector2 shakeOffset;

		// Token: 0x04001753 RID: 5971
		private float shakeTimer;

		// Token: 0x04001754 RID: 5972
		private float speedY;

		// Token: 0x04001755 RID: 5973
		private float colorLerp;
	}
}
