using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001C9 RID: 457
	[Tracked(false)]
	public class StarJumpBlock : Solid
	{
		// Token: 0x06000F99 RID: 3993 RVA: 0x00041510 File Offset: 0x0003F710
		public StarJumpBlock(Vector2 position, float width, float height, bool sinks) : base(position, width, height, false)
		{
			base.Depth = -10000;
			this.sinks = sinks;
			base.Add(new LightOcclude(1f));
			this.startY = base.Y;
			this.SurfaceSoundIndex = 32;
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x0004155E File Offset: 0x0003F75E
		public StarJumpBlock(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height, data.Bool("sinks", false))
		{
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x0004158C File Offset: 0x0003F78C
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			this.level = base.SceneAs<Level>();
			List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/starjumpBlock/leftrailing");
			List<MTexture> atlasSubtextures2 = GFX.Game.GetAtlasSubtextures("objects/starjumpBlock/railing");
			List<MTexture> atlasSubtextures3 = GFX.Game.GetAtlasSubtextures("objects/starjumpBlock/rightrailing");
			List<MTexture> atlasSubtextures4 = GFX.Game.GetAtlasSubtextures("objects/starjumpBlock/edgeH");
			int num = 8;
			while ((float)num < base.Width - 8f)
			{
				if (this.Open((float)num, -8f))
				{
					Image image = new Image(Calc.Random.Choose(atlasSubtextures4));
					image.CenterOrigin();
					image.Position = new Vector2((float)(num + 4), 4f);
					base.Add(image);
					base.Add(new Image(atlasSubtextures2[this.mod((int)(base.X + (float)num) / 8, atlasSubtextures2.Count)])
					{
						Position = new Vector2((float)num, -8f)
					});
				}
				if (this.Open((float)num, base.Height))
				{
					Image image2 = new Image(Calc.Random.Choose(atlasSubtextures4));
					image2.CenterOrigin();
					image2.Scale.Y = -1f;
					image2.Position = new Vector2((float)(num + 4), base.Height - 4f);
					base.Add(image2);
				}
				num += 8;
			}
			List<MTexture> atlasSubtextures5 = GFX.Game.GetAtlasSubtextures("objects/starjumpBlock/edgeV");
			int num2 = 8;
			while ((float)num2 < base.Height - 8f)
			{
				if (this.Open(-8f, (float)num2))
				{
					Image image3 = new Image(Calc.Random.Choose(atlasSubtextures5));
					image3.CenterOrigin();
					image3.Scale.X = -1f;
					image3.Position = new Vector2(4f, (float)(num2 + 4));
					base.Add(image3);
				}
				if (this.Open(base.Width, (float)num2))
				{
					Image image4 = new Image(Calc.Random.Choose(atlasSubtextures5));
					image4.CenterOrigin();
					image4.Position = new Vector2(base.Width - 4f, (float)(num2 + 4));
					base.Add(image4);
				}
				num2 += 8;
			}
			List<MTexture> atlasSubtextures6 = GFX.Game.GetAtlasSubtextures("objects/starjumpBlock/corner");
			Image image5 = null;
			if (this.Open(-8f, 0f) && this.Open(0f, -8f))
			{
				image5 = new Image(Calc.Random.Choose(atlasSubtextures6));
				image5.Scale.X = -1f;
				base.Add(new Image(atlasSubtextures[this.mod((int)base.X / 8, atlasSubtextures.Count)])
				{
					Position = new Vector2(0f, -8f)
				});
			}
			else if (this.Open(-8f, 0f))
			{
				image5 = new Image(Calc.Random.Choose(atlasSubtextures5));
				image5.Scale.X = -1f;
			}
			else if (this.Open(0f, -8f))
			{
				image5 = new Image(Calc.Random.Choose(atlasSubtextures4));
				base.Add(new Image(atlasSubtextures2[this.mod((int)base.X / 8, atlasSubtextures2.Count)])
				{
					Position = new Vector2(0f, -8f)
				});
			}
			image5.CenterOrigin();
			image5.Position = new Vector2(4f, 4f);
			base.Add(image5);
			Image image6 = null;
			if (this.Open(base.Width, 0f) && this.Open(base.Width - 8f, -8f))
			{
				image6 = new Image(Calc.Random.Choose(atlasSubtextures6));
				base.Add(new Image(atlasSubtextures3[this.mod((int)(base.X + base.Width) / 8 - 1, atlasSubtextures3.Count)])
				{
					Position = new Vector2(base.Width - 8f, -8f)
				});
			}
			else if (this.Open(base.Width, 0f))
			{
				image6 = new Image(Calc.Random.Choose(atlasSubtextures5));
			}
			else if (this.Open(base.Width - 8f, -8f))
			{
				image6 = new Image(Calc.Random.Choose(atlasSubtextures4));
				base.Add(new Image(atlasSubtextures2[this.mod((int)(base.X + base.Width) / 8 - 1, atlasSubtextures2.Count)])
				{
					Position = new Vector2(base.Width - 8f, -8f)
				});
			}
			image6.CenterOrigin();
			image6.Position = new Vector2(base.Width - 4f, 4f);
			base.Add(image6);
			Image image7 = null;
			if (this.Open(-8f, base.Height - 8f) && this.Open(0f, base.Height))
			{
				image7 = new Image(Calc.Random.Choose(atlasSubtextures6));
				image7.Scale.X = -1f;
			}
			else if (this.Open(-8f, base.Height - 8f))
			{
				image7 = new Image(Calc.Random.Choose(atlasSubtextures5));
				image7.Scale.X = -1f;
			}
			else if (this.Open(0f, base.Height))
			{
				image7 = new Image(Calc.Random.Choose(atlasSubtextures4));
			}
			image7.Scale.Y = -1f;
			image7.CenterOrigin();
			image7.Position = new Vector2(4f, base.Height - 4f);
			base.Add(image7);
			Image image8 = null;
			if (this.Open(base.Width, base.Height - 8f) && this.Open(base.Width - 8f, base.Height))
			{
				image8 = new Image(Calc.Random.Choose(atlasSubtextures6));
			}
			else if (this.Open(base.Width, base.Height - 8f))
			{
				image8 = new Image(Calc.Random.Choose(atlasSubtextures5));
			}
			else if (this.Open(base.Width - 8f, base.Height))
			{
				image8 = new Image(Calc.Random.Choose(atlasSubtextures4));
			}
			image8.Scale.Y = -1f;
			image8.CenterOrigin();
			image8.Position = new Vector2(base.Width - 4f, base.Height - 4f);
			base.Add(image8);
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x00026894 File Offset: 0x00024A94
		private int mod(int x, int m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x06000F9D RID: 3997 RVA: 0x00041C87 File Offset: 0x0003FE87
		private bool Open(float x, float y)
		{
			return !base.Scene.CollideCheck<StarJumpBlock>(new Vector2(base.X + x + 4f, base.Y + y + 4f));
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x00041CB8 File Offset: 0x0003FEB8
		public override void Update()
		{
			base.Update();
			if (this.sinks)
			{
				if (base.HasPlayerRider())
				{
					this.sinkTimer = 0.1f;
				}
				else if (this.sinkTimer > 0f)
				{
					this.sinkTimer -= Engine.DeltaTime;
				}
				if (this.sinkTimer > 0f)
				{
					this.yLerp = Calc.Approach(this.yLerp, 1f, 1f * Engine.DeltaTime);
				}
				else
				{
					this.yLerp = Calc.Approach(this.yLerp, 0f, 1f * Engine.DeltaTime);
				}
				float y = MathHelper.Lerp(this.startY, this.startY + 12f, Ease.SineInOut(this.yLerp));
				base.MoveToY(y);
			}
		}

		// Token: 0x06000F9F RID: 3999 RVA: 0x00041D8C File Offset: 0x0003FF8C
		public override void Render()
		{
			StarJumpController entity = base.Scene.Tracker.GetEntity<StarJumpController>();
			if (entity != null)
			{
				Vector2 vector = this.level.Camera.Position.Floor();
				VirtualRenderTarget blockFill = entity.BlockFill;
				Draw.SpriteBatch.Draw(blockFill, this.Position, new Rectangle?(new Rectangle((int)(base.X - vector.X), (int)(base.Y - vector.Y), (int)base.Width, (int)base.Height)), Color.White);
			}
			base.Render();
		}

		// Token: 0x04000B02 RID: 2818
		private Level level;

		// Token: 0x04000B03 RID: 2819
		private bool sinks;

		// Token: 0x04000B04 RID: 2820
		private float startY;

		// Token: 0x04000B05 RID: 2821
		private float yLerp;

		// Token: 0x04000B06 RID: 2822
		private float sinkTimer;
	}
}
