using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020001F1 RID: 497
	public class BlackholeBG : Backdrop
	{
		// Token: 0x06001056 RID: 4182 RVA: 0x00047BA8 File Offset: 0x00045DA8
		public BlackholeBG()
		{
			this.bgTexture = GFX.Game["objects/temple/portal/portal"];
			List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("bgs/10/blackhole/particle");
			int num = 0;
			for (int i = 0; i < 50; i++)
			{
				MTexture mtexture = this.streams[i].Texture = Calc.Random.Choose(atlasSubtextures);
				this.streams[i].Percent = Calc.Random.NextFloat();
				this.streams[i].Speed = Calc.Random.Range(0.2f, 0.4f);
				this.streams[i].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
				this.streams[i].Color = Calc.Random.Next(this.colorsMild.Length);
				this.streamVerts[num].TextureCoordinate = new Vector2(mtexture.LeftUV, mtexture.TopUV);
				this.streamVerts[num + 1].TextureCoordinate = new Vector2(mtexture.RightUV, mtexture.TopUV);
				this.streamVerts[num + 2].TextureCoordinate = new Vector2(mtexture.RightUV, mtexture.BottomUV);
				this.streamVerts[num + 3].TextureCoordinate = new Vector2(mtexture.LeftUV, mtexture.TopUV);
				this.streamVerts[num + 4].TextureCoordinate = new Vector2(mtexture.RightUV, mtexture.BottomUV);
				this.streamVerts[num + 5].TextureCoordinate = new Vector2(mtexture.LeftUV, mtexture.BottomUV);
				num += 6;
			}
			int num2 = 0;
			for (int j = 0; j < 10; j++)
			{
				MTexture mtexture2 = this.streams[j].Texture = Calc.Random.Choose(atlasSubtextures);
				this.spirals[j].Percent = Calc.Random.NextFloat();
				this.spirals[j].Offset = Calc.Random.NextFloat(6.2831855f);
				this.spirals[j].Color = Calc.Random.Next(this.colorsMild.Length);
				for (int k = 0; k < 12; k++)
				{
					float x = MathHelper.Lerp(mtexture2.LeftUV, mtexture2.RightUV, (float)k / 12f);
					float x2 = MathHelper.Lerp(mtexture2.LeftUV, mtexture2.RightUV, (float)(k + 1) / 12f);
					this.spiralVerts[num2].TextureCoordinate = new Vector2(x, mtexture2.TopUV);
					this.spiralVerts[num2 + 1].TextureCoordinate = new Vector2(x2, mtexture2.TopUV);
					this.spiralVerts[num2 + 2].TextureCoordinate = new Vector2(x2, mtexture2.BottomUV);
					this.spiralVerts[num2 + 3].TextureCoordinate = new Vector2(x, mtexture2.TopUV);
					this.spiralVerts[num2 + 4].TextureCoordinate = new Vector2(x2, mtexture2.BottomUV);
					this.spiralVerts[num2 + 5].TextureCoordinate = new Vector2(x, mtexture2.BottomUV);
					num2 += 6;
				}
			}
			for (int l = 0; l < 220; l++)
			{
				this.particles[l].Percent = Calc.Random.NextFloat();
				this.particles[l].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
				this.particles[l].Color = Calc.Random.Next(this.colorsMild.Length);
			}
			this.center = new Vector2(320f, 180f) / 2f;
			this.offset = Vector2.Zero;
			this.colorsLerp = new Color[this.colorsMild.Length];
			this.colorsLerpBlack = new Color[this.colorsMild.Length, 20];
			this.colorsLerpTransparent = new Color[this.colorsMild.Length, 20];
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x0004816C File Offset: 0x0004636C
		public void SnapStrength(Level level, BlackholeBG.Strengths strength)
		{
			this.strength = strength;
			this.StrengthMultiplier = 1f + (float)strength;
			level.Session.SetCounter("blackhole_strength", (int)strength);
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x00048194 File Offset: 0x00046394
		public void NextStrength(Level level, BlackholeBG.Strengths strength)
		{
			this.strength = strength;
			level.Session.SetCounter("blackhole_strength", (int)strength);
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06001059 RID: 4185 RVA: 0x000481AE File Offset: 0x000463AE
		public int StreamCount
		{
			get
			{
				return (int)MathHelper.Lerp(30f, 50f, (this.StrengthMultiplier - 1f) / 3f);
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x0600105A RID: 4186 RVA: 0x000481D2 File Offset: 0x000463D2
		public int ParticleCount
		{
			get
			{
				return (int)MathHelper.Lerp(150f, 220f, (this.StrengthMultiplier - 1f) / 3f);
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x0600105B RID: 4187 RVA: 0x000481F6 File Offset: 0x000463F6
		public int SpiralCount
		{
			get
			{
				return (int)MathHelper.Lerp(0f, 10f, (this.StrengthMultiplier - 1f) / 3f);
			}
		}

		// Token: 0x0600105C RID: 4188 RVA: 0x0004821C File Offset: 0x0004641C
		public override void Update(Scene scene)
		{
			base.Update(scene);
			if (!this.checkedFlag)
			{
				int counter = (scene as Level).Session.GetCounter("blackhole_strength");
				if (counter >= 0)
				{
					this.SnapStrength(scene as Level, (BlackholeBG.Strengths)counter);
				}
				this.checkedFlag = true;
			}
			if (this.Visible)
			{
				this.StrengthMultiplier = Calc.Approach(this.StrengthMultiplier, 1f + (float)this.strength, Engine.DeltaTime * 0.1f);
				if (scene.OnInterval(0.05f))
				{
					for (int i = 0; i < this.colorsMild.Length; i++)
					{
						this.colorsLerp[i] = Color.Lerp(this.colorsMild[i], this.colorsWild[i], (this.StrengthMultiplier - 1f) / 3f);
						for (int j = 0; j < 20; j++)
						{
							this.colorsLerpBlack[i, j] = Color.Lerp(this.colorsLerp[i], Color.Black, (float)j / 19f) * this.FadeAlphaMultiplier;
							this.colorsLerpTransparent[i, j] = Color.Lerp(this.colorsLerp[i], Color.Transparent, (float)j / 19f) * this.FadeAlphaMultiplier;
						}
					}
				}
				float num = 1f + (this.StrengthMultiplier - 1f) * 0.7f;
				int streamCount = this.StreamCount;
				int num2 = 0;
				for (int k = 0; k < streamCount; k++)
				{
					BlackholeBG.StreamParticle[] array = this.streams;
					int num3 = k;
					array[num3].Percent = array[num3].Percent + this.streams[k].Speed * Engine.DeltaTime * num * this.Direction;
					if (this.streams[k].Percent >= 1f && this.Direction > 0f)
					{
						this.streams[k].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
						BlackholeBG.StreamParticle[] array2 = this.streams;
						int num4 = k;
						array2[num4].Percent = array2[num4].Percent - 1f;
					}
					else if (this.streams[k].Percent < 0f && this.Direction < 0f)
					{
						this.streams[k].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
						BlackholeBG.StreamParticle[] array3 = this.streams;
						int num5 = k;
						array3[num5].Percent = array3[num5].Percent + 1f;
					}
					float percent = this.streams[k].Percent;
					float num6 = Ease.CubeIn(Calc.ClampedMap(percent, 0f, 0.8f, 0f, 1f));
					float num7 = Ease.CubeIn(Calc.ClampedMap(percent, 0.2f, 1f, 0f, 1f));
					Vector2 normal = this.streams[k].Normal;
					Vector2 value = normal.Perpendicular();
					Vector2 value2 = normal * 16f + normal * (1f - num6) * 200f;
					float scaleFactor = (1f - num6) * 8f;
					Color color = this.colorsLerpBlack[this.streams[k].Color, (int)(num6 * 0.6f * 19f)];
					Vector2 value3 = normal * 16f + normal * (1f - num7) * 280f;
					float scaleFactor2 = (1f - num7) * 8f;
					Color color2 = this.colorsLerpBlack[this.streams[k].Color, (int)(num7 * 0.6f * 19f)];
					Vector2 vector = value2 - value * scaleFactor;
					Vector2 vector2 = value2 + value * scaleFactor;
					Vector2 vector3 = value3 + value * scaleFactor2;
					Vector2 vector4 = value3 - value * scaleFactor2;
					this.AssignVertColors(this.streamVerts, num2, ref color, ref color, ref color2, ref color2);
					this.AssignVertPosition(this.streamVerts, num2, ref vector, ref vector2, ref vector3, ref vector4);
					num2 += 6;
				}
				float num8 = this.StrengthMultiplier * 0.25f;
				int particleCount = this.ParticleCount;
				for (int l = 0; l < particleCount; l++)
				{
					BlackholeBG.Particle[] array4 = this.particles;
					int num9 = l;
					array4[num9].Percent = array4[num9].Percent + Engine.DeltaTime * num8 * this.Direction;
					if (this.particles[l].Percent >= 1f && this.Direction > 0f)
					{
						this.particles[l].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
						BlackholeBG.Particle[] array5 = this.particles;
						int num10 = l;
						array5[num10].Percent = array5[num10].Percent - 1f;
					}
					else if (this.particles[l].Percent < 0f && this.Direction < 0f)
					{
						this.particles[l].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
						BlackholeBG.Particle[] array6 = this.particles;
						int num11 = l;
						array6[num11].Percent = array6[num11].Percent + 1f;
					}
				}
				float num12 = 0.2f + (this.StrengthMultiplier - 1f) * 0.1f;
				int spiralCount = this.SpiralCount;
				Color value4 = Color.Lerp(Color.Lerp(this.bgColorOuterMild, this.bgColorOuterWild, (this.StrengthMultiplier - 1f) / 3f), Color.White, 0.1f) * 0.8f;
				int num13 = 0;
				for (int m = 0; m < spiralCount; m++)
				{
					BlackholeBG.SpiralDebris[] array7 = this.spirals;
					int num14 = m;
					array7[num14].Percent = array7[num14].Percent + this.streams[m].Speed * Engine.DeltaTime * num12 * this.Direction;
					if (this.spirals[m].Percent >= 1f && this.Direction > 0f)
					{
						this.spirals[m].Offset = Calc.Random.NextFloat(6.2831855f);
						BlackholeBG.SpiralDebris[] array8 = this.spirals;
						int num15 = m;
						array8[num15].Percent = array8[num15].Percent - 1f;
					}
					else if (this.spirals[m].Percent < 0f && this.Direction < 0f)
					{
						this.spirals[m].Offset = Calc.Random.NextFloat(6.2831855f);
						BlackholeBG.SpiralDebris[] array9 = this.spirals;
						int num16 = m;
						array9[num16].Percent = array9[num16].Percent + 1f;
					}
					float percent2 = this.spirals[m].Percent;
					float num17 = this.spirals[m].Offset;
					float value5 = Calc.ClampedMap(percent2, 0f, 0.8f, 0f, 1f);
					float value6 = Calc.ClampedMap(percent2, 0f, 1f, 0f, 1f);
					for (int n = 0; n < 12; n++)
					{
						float num18 = 1f - MathHelper.Lerp(value5, value6, (float)n / 12f);
						float num19 = 1f - MathHelper.Lerp(value5, value6, (float)(n + 1) / 12f);
						Vector2 value7 = Calc.AngleToVector(num18 * (20f + (float)n * 0.2f) + num17, 1f);
						Vector2 value8 = value7 * num18 * 200f;
						float scaleFactor3 = num18 * (4f + this.StrengthMultiplier * 4f);
						Vector2 value9 = Calc.AngleToVector(num19 * (20f + (float)(n + 1) * 0.2f) + num17, 1f);
						Vector2 value10 = value9 * num19 * 200f;
						float scaleFactor4 = num19 * (4f + this.StrengthMultiplier * 4f);
						Color color3 = Color.Lerp(value4, Color.Black, (1f - num18) * 0.5f);
						Color color4 = Color.Lerp(value4, Color.Black, (1f - num19) * 0.5f);
						Vector2 vector5 = value8 + value7 * scaleFactor3;
						Vector2 vector6 = value10 + value9 * scaleFactor4;
						Vector2 vector7 = value10 - value9 * scaleFactor4;
						Vector2 vector8 = value8 - value7 * scaleFactor3;
						this.AssignVertColors(this.spiralVerts, num13, ref color3, ref color4, ref color4, ref color3);
						this.AssignVertPosition(this.spiralVerts, num13, ref vector5, ref vector6, ref vector7, ref vector8);
						num13 += 6;
					}
				}
				Vector2 wind = (scene as Level).Wind;
				Vector2 value11 = new Vector2(320f, 180f) / 2f + wind * 0.15f + this.CenterOffset;
				this.center += (value11 - this.center) * (1f - (float)Math.Pow(0.009999999776482582, (double)Engine.DeltaTime));
				Vector2 value12 = -wind * 0.25f + this.OffsetOffset;
				this.offset += (value12 - this.offset) * (1f - (float)Math.Pow(0.009999999776482582, (double)Engine.DeltaTime));
				if (scene.OnInterval(0.025f))
				{
					this.shake = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 2f * (this.StrengthMultiplier - 1f));
				}
				this.spinTime += (2f + this.StrengthMultiplier) * Engine.DeltaTime;
			}
		}

		// Token: 0x0600105D RID: 4189 RVA: 0x00048C5C File Offset: 0x00046E5C
		private void AssignVertColors(VertexPositionColorTexture[] verts, int v, ref Color a, ref Color b, ref Color c, ref Color d)
		{
			verts[v].Color = a;
			verts[v + 1].Color = b;
			verts[v + 2].Color = c;
			verts[v + 3].Color = a;
			verts[v + 4].Color = c;
			verts[v + 5].Color = d;
		}

		// Token: 0x0600105E RID: 4190 RVA: 0x00048CE4 File Offset: 0x00046EE4
		private void AssignVertPosition(VertexPositionColorTexture[] verts, int v, ref Vector2 a, ref Vector2 b, ref Vector2 c, ref Vector2 d)
		{
			verts[v].Position = new Vector3(a, 0f);
			verts[v + 1].Position = new Vector3(b, 0f);
			verts[v + 2].Position = new Vector3(c, 0f);
			verts[v + 3].Position = new Vector3(a, 0f);
			verts[v + 4].Position = new Vector3(c, 0f);
			verts[v + 5].Position = new Vector3(d, 0f);
		}

		// Token: 0x0600105F RID: 4191 RVA: 0x00048DA8 File Offset: 0x00046FA8
		public override void BeforeRender(Scene scene)
		{
			if (this.buffer == null || this.buffer.IsDisposed)
			{
				this.buffer = VirtualContent.CreateRenderTarget("Black Hole", 320, 180, false, true, 0);
			}
			Engine.Graphics.GraphicsDevice.SetRenderTarget(this.buffer);
			Engine.Graphics.GraphicsDevice.Clear(this.bgColorInner);
			Draw.SpriteBatch.Begin();
			Color value = Color.Lerp(this.bgColorOuterMild, this.bgColorOuterWild, (this.StrengthMultiplier - 1f) / 3f);
			for (int i = 0; i < 20; i++)
			{
				float num = (1f - this.spinTime % 1f) * 0.05f + (float)i / 20f;
				Color color = Color.Lerp(this.bgColorInner, value, Ease.SineOut(num));
				float scale = Calc.ClampedMap(num, 0f, 1f, 0.1f, 4f);
				float rotation = 6.2831855f * num;
				this.bgTexture.DrawCentered(this.center + this.offset * num + this.shake * (1f - num), color, scale, rotation);
			}
			Draw.SpriteBatch.End();
			if (this.SpiralCount > 0)
			{
				Engine.Instance.GraphicsDevice.Textures[0] = GFX.Game.Sources[0].Texture;
				GFX.DrawVertices<VertexPositionColorTexture>(Matrix.CreateTranslation(this.center.X, this.center.Y, 0f), this.spiralVerts, this.SpiralCount * 12 * 6, GFX.FxTexture, null);
			}
			if (this.StreamCount > 0)
			{
				Engine.Instance.GraphicsDevice.Textures[0] = GFX.Game.Sources[0].Texture;
				GFX.DrawVertices<VertexPositionColorTexture>(Matrix.CreateTranslation(this.center.X, this.center.Y, 0f), this.streamVerts, this.StreamCount * 6, GFX.FxTexture, null);
			}
			Draw.SpriteBatch.Begin();
			int particleCount = this.ParticleCount;
			for (int j = 0; j < particleCount; j++)
			{
				float num2 = Ease.CubeIn(Calc.Clamp(this.particles[j].Percent, 0f, 1f));
				Vector2 value2 = this.center + this.particles[j].Normal * Calc.ClampedMap(num2, 1f, 0f, 8f, 220f);
				Color color2 = this.colorsLerpTransparent[this.particles[j].Color, (int)(num2 * 19f)];
				float num3 = 1f + (1f - num2) * 1.5f;
				Draw.Rect(value2 - new Vector2(num3, num3) / 2f, num3, num3, color2);
			}
			Draw.SpriteBatch.End();
		}

		// Token: 0x06001060 RID: 4192 RVA: 0x000490D7 File Offset: 0x000472D7
		public override void Ended(Scene scene)
		{
			if (this.buffer != null)
			{
				this.buffer.Dispose();
				this.buffer = null;
			}
		}

		// Token: 0x06001061 RID: 4193 RVA: 0x000490F4 File Offset: 0x000472F4
		public override void Render(Scene scene)
		{
			if (this.buffer != null && !this.buffer.IsDisposed)
			{
				Vector2 vector = new Vector2((float)this.buffer.Width, (float)this.buffer.Height) / 2f;
				Draw.SpriteBatch.Draw(this.buffer, vector, new Rectangle?(this.buffer.Bounds), Color.White * this.FadeAlphaMultiplier * this.Alpha, 0f, vector, this.Scale, SpriteEffects.None, 0f);
			}
		}

		// Token: 0x04000BC0 RID: 3008
		private const string STRENGTH_FLAG = "blackhole_strength";

		// Token: 0x04000BC1 RID: 3009
		private const int BG_STEPS = 20;

		// Token: 0x04000BC2 RID: 3010
		private const int STREAM_MIN_COUNT = 30;

		// Token: 0x04000BC3 RID: 3011
		private const int STREAM_MAX_COUNT = 50;

		// Token: 0x04000BC4 RID: 3012
		private const int PARTICLE_MIN_COUNT = 150;

		// Token: 0x04000BC5 RID: 3013
		private const int PARTICLE_MAX_COUNT = 220;

		// Token: 0x04000BC6 RID: 3014
		private const int SPIRAL_MIN_COUNT = 0;

		// Token: 0x04000BC7 RID: 3015
		private const int SPIRAL_MAX_COUNT = 10;

		// Token: 0x04000BC8 RID: 3016
		private const int SPIRAL_SEGMENTS = 12;

		// Token: 0x04000BC9 RID: 3017
		private Color[] colorsMild = new Color[]
		{
			Calc.HexToColor("6e3199") * 0.8f,
			Calc.HexToColor("851f91") * 0.8f,
			Calc.HexToColor("3026b0") * 0.8f
		};

		// Token: 0x04000BCA RID: 3018
		private Color[] colorsWild = new Color[]
		{
			Calc.HexToColor("ca4ca7"),
			Calc.HexToColor("b14cca"),
			Calc.HexToColor("ca4ca7")
		};

		// Token: 0x04000BCB RID: 3019
		private Color[] colorsLerp;

		// Token: 0x04000BCC RID: 3020
		private Color[,] colorsLerpBlack;

		// Token: 0x04000BCD RID: 3021
		private Color[,] colorsLerpTransparent;

		// Token: 0x04000BCE RID: 3022
		private const int colorSteps = 20;

		// Token: 0x04000BCF RID: 3023
		public float Alpha = 1f;

		// Token: 0x04000BD0 RID: 3024
		public float Scale = 1f;

		// Token: 0x04000BD1 RID: 3025
		public float Direction = 1f;

		// Token: 0x04000BD2 RID: 3026
		public float StrengthMultiplier = 1f;

		// Token: 0x04000BD3 RID: 3027
		public Vector2 CenterOffset;

		// Token: 0x04000BD4 RID: 3028
		public Vector2 OffsetOffset;

		// Token: 0x04000BD5 RID: 3029
		private BlackholeBG.Strengths strength;

		// Token: 0x04000BD6 RID: 3030
		private readonly Color bgColorInner = Calc.HexToColor("000000");

		// Token: 0x04000BD7 RID: 3031
		private readonly Color bgColorOuterMild = Calc.HexToColor("512a8b");

		// Token: 0x04000BD8 RID: 3032
		private readonly Color bgColorOuterWild = Calc.HexToColor("bd2192");

		// Token: 0x04000BD9 RID: 3033
		private readonly MTexture bgTexture;

		// Token: 0x04000BDA RID: 3034
		private BlackholeBG.StreamParticle[] streams = new BlackholeBG.StreamParticle[50];

		// Token: 0x04000BDB RID: 3035
		private VertexPositionColorTexture[] streamVerts = new VertexPositionColorTexture[300];

		// Token: 0x04000BDC RID: 3036
		private BlackholeBG.Particle[] particles = new BlackholeBG.Particle[220];

		// Token: 0x04000BDD RID: 3037
		private BlackholeBG.SpiralDebris[] spirals = new BlackholeBG.SpiralDebris[10];

		// Token: 0x04000BDE RID: 3038
		private VertexPositionColorTexture[] spiralVerts = new VertexPositionColorTexture[720];

		// Token: 0x04000BDF RID: 3039
		private VirtualRenderTarget buffer;

		// Token: 0x04000BE0 RID: 3040
		private Vector2 center;

		// Token: 0x04000BE1 RID: 3041
		private Vector2 offset;

		// Token: 0x04000BE2 RID: 3042
		private Vector2 shake;

		// Token: 0x04000BE3 RID: 3043
		private float spinTime;

		// Token: 0x04000BE4 RID: 3044
		private bool checkedFlag;

		// Token: 0x020004ED RID: 1261
		public enum Strengths
		{
			// Token: 0x0400243B RID: 9275
			Mild,
			// Token: 0x0400243C RID: 9276
			Medium,
			// Token: 0x0400243D RID: 9277
			High,
			// Token: 0x0400243E RID: 9278
			Wild
		}

		// Token: 0x020004EE RID: 1262
		private struct StreamParticle
		{
			// Token: 0x0400243F RID: 9279
			public int Color;

			// Token: 0x04002440 RID: 9280
			public MTexture Texture;

			// Token: 0x04002441 RID: 9281
			public float Percent;

			// Token: 0x04002442 RID: 9282
			public float Speed;

			// Token: 0x04002443 RID: 9283
			public Vector2 Normal;
		}

		// Token: 0x020004EF RID: 1263
		private struct Particle
		{
			// Token: 0x04002444 RID: 9284
			public int Color;

			// Token: 0x04002445 RID: 9285
			public Vector2 Normal;

			// Token: 0x04002446 RID: 9286
			public float Percent;
		}

		// Token: 0x020004F0 RID: 1264
		private struct SpiralDebris
		{
			// Token: 0x04002447 RID: 9287
			public int Color;

			// Token: 0x04002448 RID: 9288
			public float Percent;

			// Token: 0x04002449 RID: 9289
			public float Offset;
		}
	}
}
