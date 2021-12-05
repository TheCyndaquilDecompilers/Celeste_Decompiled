using System;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002D9 RID: 729
	[Tracked(false)]
	public class SwapBlock : Solid
	{
		// Token: 0x06001687 RID: 5767 RVA: 0x00084FAC File Offset: 0x000831AC
		public SwapBlock(Vector2 position, float width, float height, Vector2 node, SwapBlock.Themes theme) : base(position, width, height, false)
		{
			this.Theme = theme;
			this.start = this.Position;
			this.end = node;
			this.maxForwardSpeed = 360f / Vector2.Distance(this.start, this.end);
			this.maxBackwardSpeed = this.maxForwardSpeed * 0.4f;
			this.Direction.X = (float)Math.Sign(this.end.X - this.start.X);
			this.Direction.Y = (float)Math.Sign(this.end.Y - this.start.Y);
			base.Add(new DashListener
			{
				OnDash = new Action<Vector2>(this.OnDash)
			});
			int num = (int)MathHelper.Min(base.X, node.X);
			int num2 = (int)MathHelper.Min(base.Y, node.Y);
			int num3 = (int)MathHelper.Max(base.X + base.Width, node.X + base.Width);
			int num4 = (int)MathHelper.Max(base.Y + base.Height, node.Y + base.Height);
			this.moveRect = new Rectangle(num, num2, num3 - num, num4 - num2);
			MTexture mtexture;
			MTexture mtexture2;
			MTexture mtexture3;
			if (this.Theme == SwapBlock.Themes.Moon)
			{
				mtexture = GFX.Game["objects/swapblock/moon/block"];
				mtexture2 = GFX.Game["objects/swapblock/moon/blockRed"];
				mtexture3 = GFX.Game["objects/swapblock/moon/target"];
			}
			else
			{
				mtexture = GFX.Game["objects/swapblock/block"];
				mtexture2 = GFX.Game["objects/swapblock/blockRed"];
				mtexture3 = GFX.Game["objects/swapblock/target"];
			}
			this.nineSliceGreen = new MTexture[3, 3];
			this.nineSliceRed = new MTexture[3, 3];
			this.nineSliceTarget = new MTexture[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					this.nineSliceGreen[i, j] = mtexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
					this.nineSliceRed[i, j] = mtexture2.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
					this.nineSliceTarget[i, j] = mtexture3.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
				}
			}
			if (this.Theme == SwapBlock.Themes.Normal)
			{
				base.Add(this.middleGreen = GFX.SpriteBank.Create("swapBlockLight"));
				base.Add(this.middleRed = GFX.SpriteBank.Create("swapBlockLightRed"));
			}
			else if (this.Theme == SwapBlock.Themes.Moon)
			{
				base.Add(this.middleGreen = GFX.SpriteBank.Create("swapBlockLightMoon"));
				base.Add(this.middleRed = GFX.SpriteBank.Create("swapBlockLightRedMoon"));
			}
			base.Add(new LightOcclude(0.2f));
			base.Depth = -9999;
		}

		// Token: 0x06001688 RID: 5768 RVA: 0x000852E4 File Offset: 0x000834E4
		public SwapBlock(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height, data.Nodes[0] + offset, data.Enum<SwapBlock.Themes>("theme", SwapBlock.Themes.Normal))
		{
		}

		// Token: 0x06001689 RID: 5769 RVA: 0x00085324 File Offset: 0x00083524
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			scene.Add(this.path = new SwapBlock.PathRenderer(this));
		}

		// Token: 0x0600168A RID: 5770 RVA: 0x0008534D File Offset: 0x0008354D
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			Audio.Stop(this.moveSfx, true);
			Audio.Stop(this.returnSfx, true);
		}

		// Token: 0x0600168B RID: 5771 RVA: 0x0008536E File Offset: 0x0008356E
		public override void SceneEnd(Scene scene)
		{
			base.SceneEnd(scene);
			Audio.Stop(this.moveSfx, true);
			Audio.Stop(this.returnSfx, true);
		}

		// Token: 0x0600168C RID: 5772 RVA: 0x00085390 File Offset: 0x00083590
		private void OnDash(Vector2 direction)
		{
			this.Swapping = (this.lerp < 1f);
			this.target = 1;
			this.returnTimer = 0.8f;
			this.burst = (base.Scene as Level).Displacement.AddBurst(base.Center, 0.2f, 0f, 16f, 1f, null, null);
			if (this.lerp >= 0.2f)
			{
				this.speed = this.maxForwardSpeed;
			}
			else
			{
				this.speed = MathHelper.Lerp(this.maxForwardSpeed * 0.333f, this.maxForwardSpeed, this.lerp / 0.2f);
			}
			Audio.Stop(this.returnSfx, true);
			Audio.Stop(this.moveSfx, true);
			if (!this.Swapping)
			{
				Audio.Play("event:/game/05_mirror_temple/swapblock_move_end", base.Center);
				return;
			}
			this.moveSfx = Audio.Play("event:/game/05_mirror_temple/swapblock_move", base.Center);
		}

		// Token: 0x0600168D RID: 5773 RVA: 0x00085488 File Offset: 0x00083688
		public override void Update()
		{
			base.Update();
			if (this.returnTimer > 0f)
			{
				this.returnTimer -= Engine.DeltaTime;
				if (this.returnTimer <= 0f)
				{
					this.target = 0;
					this.speed = 0f;
					this.returnSfx = Audio.Play("event:/game/05_mirror_temple/swapblock_return", base.Center);
				}
			}
			if (this.burst != null)
			{
				this.burst.Position = base.Center;
			}
			this.redAlpha = Calc.Approach(this.redAlpha, (float)((this.target == 1) ? 0 : 1), Engine.DeltaTime * 32f);
			if (this.target == 0 && this.lerp == 0f)
			{
				this.middleRed.SetAnimationFrame(0);
				this.middleGreen.SetAnimationFrame(0);
			}
			if (this.target == 1)
			{
				this.speed = Calc.Approach(this.speed, this.maxForwardSpeed, this.maxForwardSpeed / 0.2f * Engine.DeltaTime);
			}
			else
			{
				this.speed = Calc.Approach(this.speed, this.maxBackwardSpeed, this.maxBackwardSpeed / 1.5f * Engine.DeltaTime);
			}
			float num = this.lerp;
			this.lerp = Calc.Approach(this.lerp, (float)this.target, this.speed * Engine.DeltaTime);
			if (this.lerp != num)
			{
				Vector2 vector = (this.end - this.start) * this.speed;
				Vector2 position = this.Position;
				if (this.target == 1)
				{
					vector = (this.end - this.start) * this.maxForwardSpeed;
				}
				if (this.lerp < num)
				{
					vector *= -1f;
				}
				if (this.target == 1 && base.Scene.OnInterval(0.02f))
				{
					this.MoveParticles(this.end - this.start);
				}
				base.MoveTo(Vector2.Lerp(this.start, this.end, this.lerp), vector);
				if (position != this.Position)
				{
					Audio.Position(this.moveSfx, base.Center);
					Audio.Position(this.returnSfx, base.Center);
					if (this.Position == this.start && this.target == 0)
					{
						Audio.SetParameter(this.returnSfx, "end", 1f);
						Audio.Play("event:/game/05_mirror_temple/swapblock_return_end", base.Center);
					}
					else if (this.Position == this.end && this.target == 1)
					{
						Audio.Play("event:/game/05_mirror_temple/swapblock_move_end", base.Center);
					}
				}
			}
			if (this.Swapping && this.lerp >= 1f)
			{
				this.Swapping = false;
			}
			this.StopPlayerRunIntoAnimation = (this.lerp <= 0f || this.lerp >= 1f);
		}

		// Token: 0x0600168E RID: 5774 RVA: 0x00085788 File Offset: 0x00083988
		private void MoveParticles(Vector2 normal)
		{
			Vector2 position;
			Vector2 vector;
			float direction;
			float num;
			if (normal.X > 0f)
			{
				position = base.CenterLeft;
				vector = Vector2.UnitY * (base.Height - 6f);
				direction = 3.1415927f;
				num = Math.Max(2f, base.Height / 14f);
			}
			else if (normal.X < 0f)
			{
				position = base.CenterRight;
				vector = Vector2.UnitY * (base.Height - 6f);
				direction = 0f;
				num = Math.Max(2f, base.Height / 14f);
			}
			else if (normal.Y > 0f)
			{
				position = base.TopCenter;
				vector = Vector2.UnitX * (base.Width - 6f);
				direction = -1.5707964f;
				num = Math.Max(2f, base.Width / 14f);
			}
			else
			{
				position = base.BottomCenter;
				vector = Vector2.UnitX * (base.Width - 6f);
				direction = 1.5707964f;
				num = Math.Max(2f, base.Width / 14f);
			}
			this.particlesRemainder += num;
			int num2 = (int)this.particlesRemainder;
			this.particlesRemainder -= (float)num2;
			vector *= 0.5f;
			base.SceneAs<Level>().Particles.Emit(SwapBlock.P_Move, num2, position, vector, direction);
		}

		// Token: 0x0600168F RID: 5775 RVA: 0x00085904 File Offset: 0x00083B04
		public override void Render()
		{
			Vector2 vector = this.Position + base.Shake;
			if (this.lerp != (float)this.target && this.speed > 0f)
			{
				Vector2 value = (this.end - this.start).SafeNormalize();
				if (this.target == 1)
				{
					value *= -1f;
				}
				float num = this.speed / this.maxForwardSpeed;
				float num2 = 16f * num;
				int num3 = 2;
				while ((float)num3 < num2)
				{
					this.DrawBlockStyle(vector + value * (float)num3, base.Width, base.Height, this.nineSliceGreen, this.middleGreen, Color.White * (1f - (float)num3 / num2));
					num3 += 2;
				}
			}
			if (this.redAlpha < 1f)
			{
				this.DrawBlockStyle(vector, base.Width, base.Height, this.nineSliceGreen, this.middleGreen, Color.White);
			}
			if (this.redAlpha > 0f)
			{
				this.DrawBlockStyle(vector, base.Width, base.Height, this.nineSliceRed, this.middleRed, Color.White * this.redAlpha);
			}
		}

		// Token: 0x06001690 RID: 5776 RVA: 0x00085A48 File Offset: 0x00083C48
		private void DrawBlockStyle(Vector2 pos, float width, float height, MTexture[,] ninSlice, Sprite middle, Color color)
		{
			int num = (int)(width / 8f);
			int num2 = (int)(height / 8f);
			ninSlice[0, 0].Draw(pos + new Vector2(0f, 0f), Vector2.Zero, color);
			ninSlice[2, 0].Draw(pos + new Vector2(width - 8f, 0f), Vector2.Zero, color);
			ninSlice[0, 2].Draw(pos + new Vector2(0f, height - 8f), Vector2.Zero, color);
			ninSlice[2, 2].Draw(pos + new Vector2(width - 8f, height - 8f), Vector2.Zero, color);
			for (int i = 1; i < num - 1; i++)
			{
				ninSlice[1, 0].Draw(pos + new Vector2((float)(i * 8), 0f), Vector2.Zero, color);
				ninSlice[1, 2].Draw(pos + new Vector2((float)(i * 8), height - 8f), Vector2.Zero, color);
			}
			for (int j = 1; j < num2 - 1; j++)
			{
				ninSlice[0, 1].Draw(pos + new Vector2(0f, (float)(j * 8)), Vector2.Zero, color);
				ninSlice[2, 1].Draw(pos + new Vector2(width - 8f, (float)(j * 8)), Vector2.Zero, color);
			}
			for (int k = 1; k < num - 1; k++)
			{
				for (int l = 1; l < num2 - 1; l++)
				{
					ninSlice[1, 1].Draw(pos + new Vector2((float)k, (float)l) * 8f, Vector2.Zero, color);
				}
			}
			if (middle != null)
			{
				middle.Color = color;
				middle.RenderPosition = pos + new Vector2(width / 2f, height / 2f);
				middle.Render();
			}
		}

		// Token: 0x040012F6 RID: 4854
		public static ParticleType P_Move;

		// Token: 0x040012F7 RID: 4855
		private const float ReturnTime = 0.8f;

		// Token: 0x040012F8 RID: 4856
		public Vector2 Direction;

		// Token: 0x040012F9 RID: 4857
		public bool Swapping;

		// Token: 0x040012FA RID: 4858
		public SwapBlock.Themes Theme;

		// Token: 0x040012FB RID: 4859
		private Vector2 start;

		// Token: 0x040012FC RID: 4860
		private Vector2 end;

		// Token: 0x040012FD RID: 4861
		private float lerp;

		// Token: 0x040012FE RID: 4862
		private int target;

		// Token: 0x040012FF RID: 4863
		private Rectangle moveRect;

		// Token: 0x04001300 RID: 4864
		private float speed;

		// Token: 0x04001301 RID: 4865
		private float maxForwardSpeed;

		// Token: 0x04001302 RID: 4866
		private float maxBackwardSpeed;

		// Token: 0x04001303 RID: 4867
		private float returnTimer;

		// Token: 0x04001304 RID: 4868
		private float redAlpha = 1f;

		// Token: 0x04001305 RID: 4869
		private MTexture[,] nineSliceGreen;

		// Token: 0x04001306 RID: 4870
		private MTexture[,] nineSliceRed;

		// Token: 0x04001307 RID: 4871
		private MTexture[,] nineSliceTarget;

		// Token: 0x04001308 RID: 4872
		private Sprite middleGreen;

		// Token: 0x04001309 RID: 4873
		private Sprite middleRed;

		// Token: 0x0400130A RID: 4874
		private SwapBlock.PathRenderer path;

		// Token: 0x0400130B RID: 4875
		private EventInstance moveSfx;

		// Token: 0x0400130C RID: 4876
		private EventInstance returnSfx;

		// Token: 0x0400130D RID: 4877
		private DisplacementRenderer.Burst burst;

		// Token: 0x0400130E RID: 4878
		private float particlesRemainder;

		// Token: 0x02000676 RID: 1654
		public enum Themes
		{
			// Token: 0x04002ACF RID: 10959
			Normal,
			// Token: 0x04002AD0 RID: 10960
			Moon
		}

		// Token: 0x02000677 RID: 1655
		private class PathRenderer : Entity
		{
			// Token: 0x06002BA4 RID: 11172 RVA: 0x00117C90 File Offset: 0x00115E90
			public PathRenderer(SwapBlock block) : base(block.Position)
			{
				this.block = block;
				base.Depth = 8999;
				this.pathTexture = GFX.Game["objects/swapblock/path" + ((block.start.X == block.end.X) ? "V" : "H")];
				this.timer = Calc.Random.NextFloat();
			}

			// Token: 0x06002BA5 RID: 11173 RVA: 0x00117D14 File Offset: 0x00115F14
			public override void Update()
			{
				base.Update();
				this.timer += Engine.DeltaTime * 4f;
			}

			// Token: 0x06002BA6 RID: 11174 RVA: 0x00117D34 File Offset: 0x00115F34
			public override void Render()
			{
				if (this.block.Theme != SwapBlock.Themes.Moon)
				{
					for (int i = this.block.moveRect.Left; i < this.block.moveRect.Right; i += this.pathTexture.Width)
					{
						for (int j = this.block.moveRect.Top; j < this.block.moveRect.Bottom; j += this.pathTexture.Height)
						{
							this.pathTexture.GetSubtexture(0, 0, Math.Min(this.pathTexture.Width, this.block.moveRect.Right - i), Math.Min(this.pathTexture.Height, this.block.moveRect.Bottom - j), this.clipTexture);
							this.clipTexture.DrawCentered(new Vector2((float)(i + this.clipTexture.Width / 2), (float)(j + this.clipTexture.Height / 2)), Color.White);
						}
					}
				}
				float scale = 0.5f * (0.5f + ((float)Math.Sin((double)this.timer) + 1f) * 0.25f);
				this.block.DrawBlockStyle(new Vector2((float)this.block.moveRect.X, (float)this.block.moveRect.Y), (float)this.block.moveRect.Width, (float)this.block.moveRect.Height, this.block.nineSliceTarget, null, Color.White * scale);
			}

			// Token: 0x04002AD1 RID: 10961
			private SwapBlock block;

			// Token: 0x04002AD2 RID: 10962
			private MTexture pathTexture;

			// Token: 0x04002AD3 RID: 10963
			private MTexture clipTexture = new MTexture();

			// Token: 0x04002AD4 RID: 10964
			private float timer;
		}
	}
}
