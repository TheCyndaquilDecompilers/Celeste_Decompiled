using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000158 RID: 344
	public class LavaRect : Component
	{
		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000C5F RID: 3167 RVA: 0x00028DBE File Offset: 0x00026FBE
		// (set) Token: 0x06000C60 RID: 3168 RVA: 0x00028DC6 File Offset: 0x00026FC6
		public int SurfaceStep { get; private set; }

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000C61 RID: 3169 RVA: 0x00028DCF File Offset: 0x00026FCF
		// (set) Token: 0x06000C62 RID: 3170 RVA: 0x00028DD7 File Offset: 0x00026FD7
		public float Width { get; private set; }

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000C63 RID: 3171 RVA: 0x00028DE0 File Offset: 0x00026FE0
		// (set) Token: 0x06000C64 RID: 3172 RVA: 0x00028DE8 File Offset: 0x00026FE8
		public float Height { get; private set; }

		// Token: 0x06000C65 RID: 3173 RVA: 0x00028DF4 File Offset: 0x00026FF4
		public LavaRect(float width, float height, int step) : base(true, true)
		{
			this.Resize(width, height, step);
		}

		// Token: 0x06000C66 RID: 3174 RVA: 0x00028E80 File Offset: 0x00027080
		public void Resize(float width, float height, int step)
		{
			this.Width = width;
			this.Height = height;
			this.SurfaceStep = step;
			this.dirty = true;
			int num = (int)(width / (float)this.SurfaceStep * 2f + height / (float)this.SurfaceStep * 2f + 4f);
			this.verts = new VertexPositionColor[num * 3 * 6 + 6];
			this.bubbles = new LavaRect.Bubble[(int)(width * height * 0.005f)];
			this.surfaceBubbles = new LavaRect.SurfaceBubble[(int)Math.Max(4f, (float)this.bubbles.Length * 0.25f)];
			for (int i = 0; i < this.bubbles.Length; i++)
			{
				this.bubbles[i].Position = new Vector2(1f + Calc.Random.NextFloat(this.Width - 2f), Calc.Random.NextFloat(this.Height));
				this.bubbles[i].Speed = (float)Calc.Random.Range(4, 12);
				this.bubbles[i].Alpha = Calc.Random.Range(0.4f, 0.8f);
			}
			for (int j = 0; j < this.surfaceBubbles.Length; j++)
			{
				this.surfaceBubbles[j].X = -1f;
			}
			this.surfaceBubbleAnimations = new List<List<MTexture>>();
			this.surfaceBubbleAnimations.Add(GFX.Game.GetAtlasSubtextures("danger/lava/bubble_a"));
		}

		// Token: 0x06000C67 RID: 3175 RVA: 0x0002900C File Offset: 0x0002720C
		public override void Update()
		{
			this.timer += this.UpdateMultiplier * Engine.DeltaTime;
			if (this.UpdateMultiplier != 0f)
			{
				this.dirty = true;
			}
			for (int i = 0; i < this.bubbles.Length; i++)
			{
				LavaRect.Bubble[] array = this.bubbles;
				int num = i;
				array[num].Position.Y = array[num].Position.Y - this.UpdateMultiplier * this.bubbles[i].Speed * Engine.DeltaTime;
				if (this.bubbles[i].Position.Y < 2f - this.Wave((int)(this.bubbles[i].Position.X / (float)this.SurfaceStep), this.Width))
				{
					this.bubbles[i].Position.Y = this.Height - 1f;
					if (Calc.Random.Chance(0.75f))
					{
						this.surfaceBubbles[this.surfaceBubbleIndex].X = this.bubbles[i].Position.X;
						this.surfaceBubbles[this.surfaceBubbleIndex].Frame = 0f;
						this.surfaceBubbles[this.surfaceBubbleIndex].Animation = (byte)Calc.Random.Next(this.surfaceBubbleAnimations.Count);
						this.surfaceBubbleIndex = (this.surfaceBubbleIndex + 1) % this.surfaceBubbles.Length;
					}
				}
			}
			for (int j = 0; j < this.surfaceBubbles.Length; j++)
			{
				if (this.surfaceBubbles[j].X >= 0f)
				{
					LavaRect.SurfaceBubble[] array2 = this.surfaceBubbles;
					int num2 = j;
					array2[num2].Frame = array2[num2].Frame + Engine.DeltaTime * 6f;
					if (this.surfaceBubbles[j].Frame >= (float)this.surfaceBubbleAnimations[(int)this.surfaceBubbles[j].Animation].Count)
					{
						this.surfaceBubbles[j].X = -1f;
					}
				}
			}
			base.Update();
		}

		// Token: 0x06000C68 RID: 3176 RVA: 0x00029245 File Offset: 0x00027445
		private float Sin(float value)
		{
			return (1f + (float)Math.Sin((double)value)) / 2f;
		}

		// Token: 0x06000C69 RID: 3177 RVA: 0x0002925C File Offset: 0x0002745C
		private float Wave(int step, float length)
		{
			int num = step * this.SurfaceStep;
			float num2 = (this.OnlyMode != LavaRect.OnlyModes.None) ? 1f : (Calc.ClampedMap((float)num, 0f, length * 0.1f, 0f, 1f) * Calc.ClampedMap((float)num, length * 0.9f, length, 1f, 0f));
			float num3 = this.Sin((float)num * 0.25f + this.timer * 4f) * this.SmallWaveAmplitude;
			num3 += this.Sin((float)num * 0.05f + this.timer * 0.5f) * this.BigWaveAmplitude;
			if (step % 2 == 0)
			{
				num3 += this.Spikey;
			}
			if (this.OnlyMode != LavaRect.OnlyModes.None)
			{
				num3 += (1f - Calc.YoYo((float)num / length)) * this.CurveAmplitude;
			}
			return num3 * num2;
		}

		// Token: 0x06000C6A RID: 3178 RVA: 0x00029334 File Offset: 0x00027534
		private void Quad(ref int vert, Vector2 va, Vector2 vb, Vector2 vc, Vector2 vd, Color color)
		{
			this.Quad(ref vert, va, color, vb, color, vc, color, vd, color);
		}

		// Token: 0x06000C6B RID: 3179 RVA: 0x00029358 File Offset: 0x00027558
		private void Quad(ref int vert, Vector2 va, Color ca, Vector2 vb, Color cb, Vector2 vc, Color cc, Vector2 vd, Color cd)
		{
			this.verts[vert].Position.X = va.X;
			this.verts[vert].Position.Y = va.Y;
			VertexPositionColor[] array = this.verts;
			int num = vert;
			vert = num + 1;
			array[num].Color = ca;
			this.verts[vert].Position.X = vb.X;
			this.verts[vert].Position.Y = vb.Y;
			VertexPositionColor[] array2 = this.verts;
			num = vert;
			vert = num + 1;
			array2[num].Color = cb;
			this.verts[vert].Position.X = vc.X;
			this.verts[vert].Position.Y = vc.Y;
			VertexPositionColor[] array3 = this.verts;
			num = vert;
			vert = num + 1;
			array3[num].Color = cc;
			this.verts[vert].Position.X = va.X;
			this.verts[vert].Position.Y = va.Y;
			VertexPositionColor[] array4 = this.verts;
			num = vert;
			vert = num + 1;
			array4[num].Color = ca;
			this.verts[vert].Position.X = vc.X;
			this.verts[vert].Position.Y = vc.Y;
			VertexPositionColor[] array5 = this.verts;
			num = vert;
			vert = num + 1;
			array5[num].Color = cc;
			this.verts[vert].Position.X = vd.X;
			this.verts[vert].Position.Y = vd.Y;
			VertexPositionColor[] array6 = this.verts;
			num = vert;
			vert = num + 1;
			array6[num].Color = cd;
		}

		// Token: 0x06000C6C RID: 3180 RVA: 0x0002956C File Offset: 0x0002776C
		private void Edge(ref int vert, Vector2 a, Vector2 b, float fade, float insetFade)
		{
			float num = (a - b).Length();
			float num2 = (this.OnlyMode == LavaRect.OnlyModes.None) ? (insetFade / num) : 0f;
			float num3 = num / (float)this.SurfaceStep;
			Vector2 vector = (b - a).SafeNormalize().Perpendicular();
			int num4 = 1;
			while ((float)num4 <= num3)
			{
				Vector2 value = Vector2.Lerp(a, b, (float)(num4 - 1) / num3);
				float num5 = this.Wave(num4 - 1, num);
				Vector2 vector2 = value - vector * num5;
				Vector2 value2 = Vector2.Lerp(a, b, (float)num4 / num3);
				float num6 = this.Wave(num4, num);
				Vector2 vector3 = value2 - vector * num6;
				Vector2 value3 = Vector2.Lerp(a, b, Calc.ClampedMap((float)(num4 - 1) / num3, 0f, 1f, num2, 1f - num2));
				Vector2 value4 = Vector2.Lerp(a, b, Calc.ClampedMap((float)num4 / num3, 0f, 1f, num2, 1f - num2));
				this.Quad(ref vert, vector2 + vector, this.EdgeColor, vector3 + vector, this.EdgeColor, value4 + vector * (fade - num6), this.CenterColor, value3 + vector * (fade - num5), this.CenterColor);
				this.Quad(ref vert, value3 + vector * (fade - num5), value4 + vector * (fade - num6), value4 + vector * fade, value3 + vector * fade, this.CenterColor);
				this.Quad(ref vert, vector2, vector3, vector3 + vector * 1f, vector2 + vector * 1f, this.SurfaceColor);
				num4++;
			}
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x00029744 File Offset: 0x00027944
		public override void Render()
		{
			GameplayRenderer.End();
			if (this.dirty)
			{
				Vector2 zero = Vector2.Zero;
				Vector2 vector = zero;
				Vector2 vector2 = new Vector2(zero.X + this.Width, zero.Y);
				Vector2 vector3 = new Vector2(zero.X, zero.Y + this.Height);
				Vector2 vector4 = zero + new Vector2(this.Width, this.Height);
				Vector2 vector5 = new Vector2(Math.Min(this.Fade, this.Width / 2f), Math.Min(this.Fade, this.Height / 2f));
				this.vertCount = 0;
				if (this.OnlyMode == LavaRect.OnlyModes.None)
				{
					this.Edge(ref this.vertCount, vector, vector2, vector5.Y, vector5.X);
					this.Edge(ref this.vertCount, vector2, vector4, vector5.X, vector5.Y);
					this.Edge(ref this.vertCount, vector4, vector3, vector5.Y, vector5.X);
					this.Edge(ref this.vertCount, vector3, vector, vector5.X, vector5.Y);
					this.Quad(ref this.vertCount, vector + vector5, vector2 + new Vector2(-vector5.X, vector5.Y), vector4 - vector5, vector3 + new Vector2(vector5.X, -vector5.Y), this.CenterColor);
				}
				else if (this.OnlyMode == LavaRect.OnlyModes.OnlyTop)
				{
					this.Edge(ref this.vertCount, vector, vector2, vector5.Y, 0f);
					this.Quad(ref this.vertCount, vector + new Vector2(0f, vector5.Y), vector2 + new Vector2(0f, vector5.Y), vector4, vector3, this.CenterColor);
				}
				else if (this.OnlyMode == LavaRect.OnlyModes.OnlyBottom)
				{
					this.Edge(ref this.vertCount, vector4, vector3, vector5.Y, 0f);
					this.Quad(ref this.vertCount, vector, vector2, vector4 + new Vector2(0f, -vector5.Y), vector3 + new Vector2(0f, -vector5.Y), this.CenterColor);
				}
				this.dirty = false;
			}
			Camera camera = (base.Scene as Level).Camera;
			GFX.DrawVertices<VertexPositionColor>(Matrix.CreateTranslation(new Vector3(base.Entity.Position + this.Position, 0f)) * camera.Matrix, this.verts, this.vertCount, null, null);
			GameplayRenderer.Begin();
			Vector2 value = base.Entity.Position + this.Position;
			MTexture mtexture = GFX.Game["particles/bubble"];
			for (int i = 0; i < this.bubbles.Length; i++)
			{
				mtexture.DrawCentered(value + this.bubbles[i].Position, this.SurfaceColor * this.bubbles[i].Alpha);
			}
			for (int j = 0; j < this.surfaceBubbles.Length; j++)
			{
				if (this.surfaceBubbles[j].X >= 0f)
				{
					MTexture mtexture2 = this.surfaceBubbleAnimations[(int)this.surfaceBubbles[j].Animation][(int)this.surfaceBubbles[j].Frame];
					int num = (int)(this.surfaceBubbles[j].X / (float)this.SurfaceStep);
					float y = 1f - this.Wave(num, this.Width);
					mtexture2.DrawJustified(value + new Vector2((float)(num * this.SurfaceStep), y), new Vector2(0.5f, 1f), this.SurfaceColor);
				}
			}
		}

		// Token: 0x040007C4 RID: 1988
		public Vector2 Position;

		// Token: 0x040007C8 RID: 1992
		public float Fade = 16f;

		// Token: 0x040007C9 RID: 1993
		public float Spikey;

		// Token: 0x040007CA RID: 1994
		public LavaRect.OnlyModes OnlyMode;

		// Token: 0x040007CB RID: 1995
		public float SmallWaveAmplitude = 1f;

		// Token: 0x040007CC RID: 1996
		public float BigWaveAmplitude = 4f;

		// Token: 0x040007CD RID: 1997
		public float CurveAmplitude = 12f;

		// Token: 0x040007CE RID: 1998
		public float UpdateMultiplier = 1f;

		// Token: 0x040007CF RID: 1999
		public Color SurfaceColor = Color.White;

		// Token: 0x040007D0 RID: 2000
		public Color EdgeColor = Color.LightGray;

		// Token: 0x040007D1 RID: 2001
		public Color CenterColor = Color.DarkGray;

		// Token: 0x040007D2 RID: 2002
		private float timer = Calc.Random.NextFloat(100f);

		// Token: 0x040007D3 RID: 2003
		private VertexPositionColor[] verts;

		// Token: 0x040007D4 RID: 2004
		private bool dirty;

		// Token: 0x040007D5 RID: 2005
		private int vertCount;

		// Token: 0x040007D6 RID: 2006
		private LavaRect.Bubble[] bubbles;

		// Token: 0x040007D7 RID: 2007
		private LavaRect.SurfaceBubble[] surfaceBubbles;

		// Token: 0x040007D8 RID: 2008
		private int surfaceBubbleIndex;

		// Token: 0x040007D9 RID: 2009
		private List<List<MTexture>> surfaceBubbleAnimations;

		// Token: 0x020003E1 RID: 993
		public enum OnlyModes
		{
			// Token: 0x04001FEC RID: 8172
			None,
			// Token: 0x04001FED RID: 8173
			OnlyTop,
			// Token: 0x04001FEE RID: 8174
			OnlyBottom
		}

		// Token: 0x020003E2 RID: 994
		private struct Bubble
		{
			// Token: 0x04001FEF RID: 8175
			public Vector2 Position;

			// Token: 0x04001FF0 RID: 8176
			public float Speed;

			// Token: 0x04001FF1 RID: 8177
			public float Alpha;
		}

		// Token: 0x020003E3 RID: 995
		private struct SurfaceBubble
		{
			// Token: 0x04001FF2 RID: 8178
			public float X;

			// Token: 0x04001FF3 RID: 8179
			public float Frame;

			// Token: 0x04001FF4 RID: 8180
			public byte Animation;
		}
	}
}
