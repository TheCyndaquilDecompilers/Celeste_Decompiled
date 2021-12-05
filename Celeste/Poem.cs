using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000302 RID: 770
	public class Poem : Entity
	{
		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x0600180A RID: 6154 RVA: 0x00095973 File Offset: 0x00093B73
		// (set) Token: 0x0600180B RID: 6155 RVA: 0x0009597B File Offset: 0x00093B7B
		public Color Color { get; private set; }

		// Token: 0x0600180C RID: 6156 RVA: 0x00095984 File Offset: 0x00093B84
		public Poem(string text, int heartIndex, float heartAlpha)
		{
			if (text != null)
			{
				this.text = ActiveFont.FontSize.AutoNewline(text, 1024);
			}
			this.Color = Poem.colors[heartIndex];
			this.Heart = GFX.GuiSpriteBank.Create("heartgem" + heartIndex);
			this.Heart.Play("spin", false, false);
			this.Heart.Position = new Vector2(1920f, 1080f) * 0.5f;
			this.Heart.Color = Color.White * heartAlpha;
			int num = Math.Min(1920, Engine.ViewWidth);
			int num2 = Math.Min(1080, Engine.ViewHeight);
			this.poem = VirtualContent.CreateRenderTarget("poem-a", num, num2, false, true, 0);
			this.smoke = VirtualContent.CreateRenderTarget("poem-b", num / 2, num2 / 2, false, true, 0);
			this.temp = VirtualContent.CreateRenderTarget("poem-c", num / 2, num2 / 2, false, true, 0);
			base.Tag = (Tags.HUD | Tags.FrozenUpdate);
			base.Add(new BeforeRenderHook(new Action(this.BeforeRender)));
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.particles[i].Reset(Calc.Random.NextFloat());
			}
		}

		// Token: 0x0600180D RID: 6157 RVA: 0x00095B22 File Offset: 0x00093D22
		public void Dispose()
		{
			if (!this.disposed)
			{
				this.poem.Dispose();
				this.smoke.Dispose();
				this.temp.Dispose();
				base.RemoveSelf();
				this.disposed = true;
			}
		}

		// Token: 0x0600180E RID: 6158 RVA: 0x00095B5C File Offset: 0x00093D5C
		private void DrawPoem(Vector2 offset, Color color)
		{
			MTexture mtexture = GFX.Gui["poemside"];
			float num = ActiveFont.Measure(this.text).X * 1.5f;
			Vector2 vector = new Vector2(960f, 540f) + offset;
			mtexture.DrawCentered(vector - Vector2.UnitX * (num / 2f + 64f), color);
			ActiveFont.Draw(this.text, vector, new Vector2(0.5f, 0.5f), Vector2.One * 1.5f, color);
			mtexture.DrawCentered(vector + Vector2.UnitX * (num / 2f + 64f), color);
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x00095C18 File Offset: 0x00093E18
		public override void Update()
		{
			this.timer += Engine.DeltaTime;
			for (int i = 0; i < this.particles.Length; i++)
			{
				Poem.Particle[] array = this.particles;
				int num = i;
				array[num].Percent = array[num].Percent + Engine.DeltaTime / this.particles[i].Duration * this.ParticleSpeed;
				if (this.particles[i].Percent > 1f)
				{
					this.particles[i].Reset(0f);
				}
			}
			this.Heart.Update();
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x00095CB8 File Offset: 0x00093EB8
		public void BeforeRender()
		{
			if (this.disposed)
			{
				return;
			}
			Engine.Graphics.GraphicsDevice.SetRenderTarget(this.poem);
			Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
			Matrix transformMatrix = Matrix.CreateScale((float)this.poem.Width / 1920f);
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, transformMatrix);
			this.Heart.Position = this.Offset + new Vector2(1920f, 1080f) * 0.5f;
			this.Heart.Scale = Vector2.One * (1f + this.Shake * 0.1f);
			MTexture mtexture = OVR.Atlas["snow"];
			for (int i = 0; i < this.particles.Length; i++)
			{
				Poem.Particle particle = this.particles[i];
				float num = Ease.SineIn(particle.Percent);
				Vector2 position = this.Heart.Position + particle.Direction * (1f - num) * 1920f;
				float x = 1f + num * 2f;
				float y = 0.25f * (0.25f + (1f - num) * 0.75f);
				float scale = 1f - num;
				mtexture.DrawCentered(position, this.Color * scale, new Vector2(x, y), (-particle.Direction).Angle());
			}
			this.Heart.Position += new Vector2(Calc.Random.Range(-1f, 1f), Calc.Random.Range(-1f, 1f)) * 16f * this.Shake;
			this.Heart.Render();
			if (!string.IsNullOrEmpty(this.text))
			{
				this.DrawPoem(this.Offset + new Vector2(-2f, 0f), Color.Black * this.TextAlpha);
				this.DrawPoem(this.Offset + new Vector2(2f, 0f), Color.Black * this.TextAlpha);
				this.DrawPoem(this.Offset + new Vector2(0f, -2f), Color.Black * this.TextAlpha);
				this.DrawPoem(this.Offset + new Vector2(0f, 2f), Color.Black * this.TextAlpha);
				this.DrawPoem(this.Offset + Vector2.Zero, this.Color * this.TextAlpha);
			}
			Draw.SpriteBatch.End();
			Engine.Graphics.GraphicsDevice.SetRenderTarget(this.smoke);
			Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
			MagicGlow.Render(this.poem, this.timer, -1f, Matrix.CreateScale(0.5f));
			GaussianBlur.Blur(this.smoke, this.temp, this.smoke, 0f, true, GaussianBlur.Samples.Nine, 1f, GaussianBlur.Direction.Both, 1f);
		}

		// Token: 0x06001811 RID: 6161 RVA: 0x00096044 File Offset: 0x00094244
		public override void Render()
		{
			if (this.disposed || base.Scene.Paused)
			{
				return;
			}
			float num = 1920f / (float)this.poem.Width;
			Draw.SpriteBatch.Draw(this.smoke, Vector2.Zero, new Rectangle?(this.smoke.Bounds), Color.White * 0.3f * this.Alpha, 0f, Vector2.Zero, num * 2f, SpriteEffects.None, 0f);
			Draw.SpriteBatch.Draw(this.poem, Vector2.Zero, new Rectangle?(this.poem.Bounds), Color.White * this.Alpha, 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
		}

		// Token: 0x06001812 RID: 6162 RVA: 0x00096120 File Offset: 0x00094320
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			this.Dispose();
		}

		// Token: 0x06001813 RID: 6163 RVA: 0x0009612F File Offset: 0x0009432F
		public override void SceneEnd(Scene scene)
		{
			base.SceneEnd(scene);
			this.Dispose();
		}

		// Token: 0x040014C7 RID: 5319
		private const float textScale = 1.5f;

		// Token: 0x040014C8 RID: 5320
		private static readonly Color[] colors = new Color[]
		{
			Calc.HexToColor("8cc7fa"),
			Calc.HexToColor("ff668a"),
			Calc.HexToColor("fffc24"),
			Calc.HexToColor("ffffff")
		};

		// Token: 0x040014C9 RID: 5321
		public float Alpha = 1f;

		// Token: 0x040014CA RID: 5322
		public float TextAlpha = 1f;

		// Token: 0x040014CB RID: 5323
		public Vector2 Offset;

		// Token: 0x040014CD RID: 5325
		public Sprite Heart;

		// Token: 0x040014CE RID: 5326
		public float ParticleSpeed = 1f;

		// Token: 0x040014CF RID: 5327
		public float Shake;

		// Token: 0x040014D0 RID: 5328
		private float timer;

		// Token: 0x040014D1 RID: 5329
		private string text;

		// Token: 0x040014D2 RID: 5330
		private bool disposed;

		// Token: 0x040014D3 RID: 5331
		private VirtualRenderTarget poem;

		// Token: 0x040014D4 RID: 5332
		private VirtualRenderTarget smoke;

		// Token: 0x040014D5 RID: 5333
		private VirtualRenderTarget temp;

		// Token: 0x040014D6 RID: 5334
		private Poem.Particle[] particles = new Poem.Particle[80];

		// Token: 0x020006C8 RID: 1736
		private struct Particle
		{
			// Token: 0x06002CFC RID: 11516 RVA: 0x0011D844 File Offset: 0x0011BA44
			public void Reset(float percent)
			{
				this.Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f);
				this.Percent = percent;
				this.Duration = 0.5f + Calc.Random.NextFloat() * 0.5f;
			}

			// Token: 0x04002C2C RID: 11308
			public Vector2 Direction;

			// Token: 0x04002C2D RID: 11309
			public float Percent;

			// Token: 0x04002C2E RID: 11310
			public float Duration;
		}
	}
}
