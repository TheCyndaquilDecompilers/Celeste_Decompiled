using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000292 RID: 658
	[Tracked(false)]
	public class Water : Entity
	{
		// Token: 0x06001464 RID: 5220 RVA: 0x0006ED70 File Offset: 0x0006CF70
		public Water(EntityData data, Vector2 offset) : this(data.Position + offset, true, data.Bool("hasBottom", false), (float)data.Width, (float)data.Height)
		{
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x0006EDA0 File Offset: 0x0006CFA0
		public Water(Vector2 position, bool topSurface, bool bottomSurface, float width, float height)
		{
			this.Position = position;
			base.Tag = Tags.TransitionUpdate;
			base.Depth = -9999;
			base.Collider = new Hitbox(width, height, 0f, 0f);
			this.grid = new bool[(int)(width / 8f), (int)(height / 8f)];
			this.fill = new Rectangle(0, 0, (int)width, (int)height);
			int num = 8;
			if (topSurface)
			{
				this.TopSurface = new Water.Surface(this.Position + new Vector2(width / 2f, (float)num), new Vector2(0f, -1f), width, height);
				this.Surfaces.Add(this.TopSurface);
				this.fill.Y = this.fill.Y + num;
				this.fill.Height = this.fill.Height - num;
			}
			if (bottomSurface)
			{
				this.BottomSurface = new Water.Surface(this.Position + new Vector2(width / 2f, height - (float)num), new Vector2(0f, 1f), width, height);
				this.Surfaces.Add(this.BottomSurface);
				this.fill.Height = this.fill.Height - num;
			}
			base.Add(new DisplacementRenderHook(new Action(this.RenderDisplacement)));
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x0006EF18 File Offset: 0x0006D118
		public override void Added(Scene scene)
		{
			base.Added(scene);
			int i = 0;
			int length = this.grid.GetLength(0);
			while (i < length)
			{
				int j = 0;
				int length2 = this.grid.GetLength(1);
				while (j < length2)
				{
					this.grid[i, j] = !base.Scene.CollideCheck<Solid>(new Rectangle((int)base.X + i * 8, (int)base.Y + j * 8, 8, 8));
					j++;
				}
				i++;
			}
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x0006EF98 File Offset: 0x0006D198
		public override void Update()
		{
			base.Update();
			foreach (Water.Surface surface in this.Surfaces)
			{
				surface.Update();
			}
			foreach (Component component in base.Scene.Tracker.GetComponents<WaterInteraction>())
			{
				WaterInteraction waterInteraction = (WaterInteraction)component;
				Entity entity = waterInteraction.Entity;
				bool flag = this.contains.Contains(waterInteraction);
				bool flag2 = base.CollideCheck(entity);
				if (flag != flag2)
				{
					if (entity.Center.Y <= base.Center.Y && this.TopSurface != null)
					{
						this.TopSurface.DoRipple(entity.Center, 1f);
					}
					else if (entity.Center.Y > base.Center.Y && this.BottomSurface != null)
					{
						this.BottomSurface.DoRipple(entity.Center, 1f);
					}
					bool flag3 = waterInteraction.IsDashing();
					int num = (entity.Center.Y < base.Center.Y && !base.Scene.CollideCheck<Solid>(new Rectangle((int)entity.Center.X - 4, (int)entity.Center.Y, 8, 16))) ? 1 : 0;
					if (flag)
					{
						if (flag3)
						{
							Audio.Play("event:/char/madeline/water_dash_out", entity.Center, "deep", (float)num);
						}
						else
						{
							Audio.Play("event:/char/madeline/water_out", entity.Center, "deep", (float)num);
						}
						waterInteraction.DrippingTimer = 2f;
					}
					else
					{
						if (flag3 && num == 1)
						{
							Audio.Play("event:/char/madeline/water_dash_in", entity.Center, "deep", (float)num);
						}
						else
						{
							Audio.Play("event:/char/madeline/water_in", entity.Center, "deep", (float)num);
						}
						waterInteraction.DrippingTimer = 0f;
					}
					if (flag)
					{
						this.contains.Remove(waterInteraction);
					}
					else
					{
						this.contains.Add(waterInteraction);
					}
				}
				if (this.BottomSurface != null && entity is Player)
				{
					if (flag2 && entity.Y > base.Bottom - 8f)
					{
						if (this.playerBottomTension == null)
						{
							this.playerBottomTension = this.BottomSurface.SetTension(entity.Position, 0f);
						}
						this.playerBottomTension.Position = this.BottomSurface.GetPointAlong(entity.Position);
						this.playerBottomTension.Strength = Calc.ClampedMap(entity.Y, base.Bottom - 8f, base.Bottom + 4f, 0f, 1f);
					}
					else if (this.playerBottomTension != null)
					{
						this.BottomSurface.RemoveTension(this.playerBottomTension);
						this.playerBottomTension = null;
					}
				}
			}
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x0006F2C4 File Offset: 0x0006D4C4
		public void RenderDisplacement()
		{
			Color color = new Color(0.5f, 0.5f, 0.25f, 1f);
			int i = 0;
			int length = this.grid.GetLength(0);
			int length2 = this.grid.GetLength(1);
			while (i < length)
			{
				if (length2 > 0 && this.grid[i, 0])
				{
					Draw.Rect(base.X + (float)(i * 8), base.Y + 3f, 8f, 5f, color);
				}
				for (int j = 1; j < length2; j++)
				{
					if (this.grid[i, j])
					{
						int num = 1;
						while (j + num < length2 && this.grid[i, j + num])
						{
							num++;
						}
						Draw.Rect(base.X + (float)(i * 8), base.Y + (float)(j * 8), 8f, (float)(num * 8), color);
						j += num - 1;
					}
				}
				i++;
			}
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x0006F3C8 File Offset: 0x0006D5C8
		public override void Render()
		{
			Draw.Rect(base.X + (float)this.fill.X, base.Y + (float)this.fill.Y, (float)this.fill.Width, (float)this.fill.Height, Water.FillColor);
			GameplayRenderer.End();
			foreach (Water.Surface surface in this.Surfaces)
			{
				surface.Render((base.Scene as Level).Camera);
			}
			GameplayRenderer.Begin();
		}

		// Token: 0x04001014 RID: 4116
		public static ParticleType P_Splash;

		// Token: 0x04001015 RID: 4117
		public static readonly Color FillColor = Color.LightSkyBlue * 0.3f;

		// Token: 0x04001016 RID: 4118
		public static readonly Color SurfaceColor = Color.LightSkyBlue * 0.8f;

		// Token: 0x04001017 RID: 4119
		public static readonly Color RayTopColor = Color.LightSkyBlue * 0.6f;

		// Token: 0x04001018 RID: 4120
		public static readonly Vector2 RayAngle = new Vector2(-4f, 8f).SafeNormalize();

		// Token: 0x04001019 RID: 4121
		public Water.Surface TopSurface;

		// Token: 0x0400101A RID: 4122
		public Water.Surface BottomSurface;

		// Token: 0x0400101B RID: 4123
		public List<Water.Surface> Surfaces = new List<Water.Surface>();

		// Token: 0x0400101C RID: 4124
		private Rectangle fill;

		// Token: 0x0400101D RID: 4125
		private bool[,] grid;

		// Token: 0x0400101E RID: 4126
		private Water.Tension playerBottomTension;

		// Token: 0x0400101F RID: 4127
		private HashSet<WaterInteraction> contains = new HashSet<WaterInteraction>();

		// Token: 0x0200060E RID: 1550
		public class Ripple
		{
			// Token: 0x0400290A RID: 10506
			public float Position;

			// Token: 0x0400290B RID: 10507
			public float Speed;

			// Token: 0x0400290C RID: 10508
			public float Height;

			// Token: 0x0400290D RID: 10509
			public float Percent;

			// Token: 0x0400290E RID: 10510
			public float Duration;
		}

		// Token: 0x0200060F RID: 1551
		public class Tension
		{
			// Token: 0x0400290F RID: 10511
			public float Position;

			// Token: 0x04002910 RID: 10512
			public float Strength;
		}

		// Token: 0x02000610 RID: 1552
		public class Ray
		{
			// Token: 0x06002A07 RID: 10759 RVA: 0x0010E1E0 File Offset: 0x0010C3E0
			public Ray(float maxWidth)
			{
				this.MaxWidth = maxWidth;
				this.Reset(Calc.Random.NextFloat());
			}

			// Token: 0x06002A08 RID: 10760 RVA: 0x0010E200 File Offset: 0x0010C400
			public void Reset(float percent)
			{
				this.Position = Calc.Random.NextFloat() * this.MaxWidth;
				this.Percent = percent;
				this.Duration = Calc.Random.Range(2f, 8f);
				this.Width = (float)Calc.Random.Range(2, 16);
				this.Length = Calc.Random.Range(8f, 128f);
			}

			// Token: 0x04002911 RID: 10513
			public float Position;

			// Token: 0x04002912 RID: 10514
			public float Percent;

			// Token: 0x04002913 RID: 10515
			public float Duration;

			// Token: 0x04002914 RID: 10516
			public float Width;

			// Token: 0x04002915 RID: 10517
			public float Length;

			// Token: 0x04002916 RID: 10518
			private float MaxWidth;
		}

		// Token: 0x02000611 RID: 1553
		public class Surface
		{
			// Token: 0x06002A09 RID: 10761 RVA: 0x0010E274 File Offset: 0x0010C474
			public Surface(Vector2 position, Vector2 outwards, float width, float bodyHeight)
			{
				this.Position = position;
				this.Outwards = outwards;
				this.Width = (int)width;
				this.BodyHeight = (int)bodyHeight;
				int num = (int)(width / 4f);
				int num2 = (int)(width * 0.2f);
				this.Rays = new List<Water.Ray>();
				for (int i = 0; i < num2; i++)
				{
					this.Rays.Add(new Water.Ray(width));
				}
				this.fillStartIndex = 0;
				this.rayStartIndex = num * 6;
				this.surfaceStartIndex = (num + num2) * 6;
				this.mesh = new VertexPositionColor[(num * 2 + num2) * 6];
				for (int j = this.fillStartIndex; j < this.fillStartIndex + num * 6; j++)
				{
					this.mesh[j].Color = Water.FillColor;
				}
				for (int k = this.rayStartIndex; k < this.rayStartIndex + num2 * 6; k++)
				{
					this.mesh[k].Color = Color.Transparent;
				}
				for (int l = this.surfaceStartIndex; l < this.surfaceStartIndex + num * 6; l++)
				{
					this.mesh[l].Color = Water.SurfaceColor;
				}
			}

			// Token: 0x06002A0A RID: 10762 RVA: 0x0010E3C8 File Offset: 0x0010C5C8
			public float GetPointAlong(Vector2 position)
			{
				Vector2 value = this.Outwards.Perpendicular();
				Vector2 vector = this.Position + value * (float)(-(float)this.Width / 2);
				Vector2 lineB = this.Position + value * (float)(this.Width / 2);
				Vector2 value2 = Calc.ClosestPointOnLine(vector, lineB, position);
				return (vector - value2).Length();
			}

			// Token: 0x06002A0B RID: 10763 RVA: 0x0010E430 File Offset: 0x0010C630
			public Water.Tension SetTension(Vector2 position, float strength)
			{
				Water.Tension tension = new Water.Tension
				{
					Position = this.GetPointAlong(position),
					Strength = strength
				};
				this.Tensions.Add(tension);
				return tension;
			}

			// Token: 0x06002A0C RID: 10764 RVA: 0x0010E464 File Offset: 0x0010C664
			public void RemoveTension(Water.Tension tension)
			{
				this.Tensions.Remove(tension);
			}

			// Token: 0x06002A0D RID: 10765 RVA: 0x0010E474 File Offset: 0x0010C674
			public void DoRipple(Vector2 position, float multiplier)
			{
				float num = 80f;
				float num2 = 3f;
				float pointAlong = this.GetPointAlong(position);
				int num3 = 2;
				if (this.Width < 200)
				{
					num2 *= Calc.ClampedMap((float)this.Width, 0f, 200f, 0.25f, 1f);
					multiplier *= Calc.ClampedMap((float)this.Width, 0f, 200f, 0.5f, 1f);
				}
				this.Ripples.Add(new Water.Ripple
				{
					Position = pointAlong,
					Speed = -num,
					Height = (float)num3 * multiplier,
					Percent = 0f,
					Duration = num2
				});
				this.Ripples.Add(new Water.Ripple
				{
					Position = pointAlong,
					Speed = num,
					Height = (float)num3 * multiplier,
					Percent = 0f,
					Duration = num2
				});
			}

			// Token: 0x06002A0E RID: 10766 RVA: 0x0010E560 File Offset: 0x0010C760
			public void Update()
			{
				this.timer += Engine.DeltaTime;
				Vector2 value = this.Outwards.Perpendicular();
				for (int i = this.Ripples.Count - 1; i >= 0; i--)
				{
					Water.Ripple ripple = this.Ripples[i];
					if (ripple.Percent > 1f)
					{
						this.Ripples.RemoveAt(i);
					}
					else
					{
						ripple.Position += ripple.Speed * Engine.DeltaTime;
						if (ripple.Position < 0f || ripple.Position > (float)this.Width)
						{
							ripple.Speed = -ripple.Speed;
							ripple.Position = Calc.Clamp(ripple.Position, 0f, (float)this.Width);
						}
						ripple.Percent += Engine.DeltaTime / ripple.Duration;
					}
				}
				int j = 0;
				int num = this.fillStartIndex;
				int num2 = this.surfaceStartIndex;
				while (j < this.Width)
				{
					int num3 = j;
					float surfaceHeight = this.GetSurfaceHeight((float)num3);
					int num4 = Math.Min(j + 4, this.Width);
					float surfaceHeight2 = this.GetSurfaceHeight((float)num4);
					this.mesh[num].Position = new Vector3(this.Position + value * (float)(-(float)this.Width / 2 + num3) + this.Outwards * surfaceHeight, 0f);
					this.mesh[num + 1].Position = new Vector3(this.Position + value * (float)(-(float)this.Width / 2 + num4) + this.Outwards * surfaceHeight2, 0f);
					this.mesh[num + 2].Position = new Vector3(this.Position + value * (float)(-(float)this.Width / 2 + num3), 0f);
					this.mesh[num + 3].Position = new Vector3(this.Position + value * (float)(-(float)this.Width / 2 + num4) + this.Outwards * surfaceHeight2, 0f);
					this.mesh[num + 4].Position = new Vector3(this.Position + value * (float)(-(float)this.Width / 2 + num4), 0f);
					this.mesh[num + 5].Position = new Vector3(this.Position + value * (float)(-(float)this.Width / 2 + num3), 0f);
					this.mesh[num2].Position = new Vector3(this.Position + value * (float)(-(float)this.Width / 2 + num3) + this.Outwards * (surfaceHeight + 1f), 0f);
					this.mesh[num2 + 1].Position = new Vector3(this.Position + value * (float)(-(float)this.Width / 2 + num4) + this.Outwards * (surfaceHeight2 + 1f), 0f);
					this.mesh[num2 + 2].Position = new Vector3(this.Position + value * (float)(-(float)this.Width / 2 + num3) + this.Outwards * surfaceHeight, 0f);
					this.mesh[num2 + 3].Position = new Vector3(this.Position + value * (float)(-(float)this.Width / 2 + num4) + this.Outwards * (surfaceHeight2 + 1f), 0f);
					this.mesh[num2 + 4].Position = new Vector3(this.Position + value * (float)(-(float)this.Width / 2 + num4) + this.Outwards * surfaceHeight2, 0f);
					this.mesh[num2 + 5].Position = new Vector3(this.Position + value * (float)(-(float)this.Width / 2 + num3) + this.Outwards * surfaceHeight, 0f);
					j += 4;
					num += 6;
					num2 += 6;
				}
				Vector2 value2 = this.Position + value * ((float)(-(float)this.Width) / 2f);
				int num5 = this.rayStartIndex;
				foreach (Water.Ray ray in this.Rays)
				{
					if (ray.Percent > 1f)
					{
						ray.Reset(0f);
					}
					ray.Percent += Engine.DeltaTime / ray.Duration;
					float scale = 1f;
					if (ray.Percent < 0.1f)
					{
						scale = Calc.ClampedMap(ray.Percent, 0f, 0.1f, 0f, 1f);
					}
					else if (ray.Percent > 0.9f)
					{
						scale = Calc.ClampedMap(ray.Percent, 0.9f, 1f, 1f, 0f);
					}
					float num6 = Math.Max(0f, ray.Position - ray.Width / 2f);
					float num7 = Math.Min((float)this.Width, ray.Position + ray.Width / 2f);
					float scaleFactor = Math.Min((float)this.BodyHeight, 0.7f * ray.Length);
					float num8 = 0.3f * ray.Length;
					Vector2 value3 = value2 + value * num6 + this.Outwards * this.GetSurfaceHeight(num6);
					Vector2 value4 = value2 + value * num7 + this.Outwards * this.GetSurfaceHeight(num7);
					Vector2 value5 = value2 + value * (num7 - num8) - this.Outwards * scaleFactor;
					Vector2 value6 = value2 + value * (num6 - num8) - this.Outwards * scaleFactor;
					this.mesh[num5].Position = new Vector3(value3, 0f);
					this.mesh[num5].Color = Water.RayTopColor * scale;
					this.mesh[num5 + 1].Position = new Vector3(value4, 0f);
					this.mesh[num5 + 1].Color = Water.RayTopColor * scale;
					this.mesh[num5 + 2].Position = new Vector3(value6, 0f);
					this.mesh[num5 + 3].Position = new Vector3(value4, 0f);
					this.mesh[num5 + 3].Color = Water.RayTopColor * scale;
					this.mesh[num5 + 4].Position = new Vector3(value5, 0f);
					this.mesh[num5 + 5].Position = new Vector3(value6, 0f);
					num5 += 6;
				}
			}

			// Token: 0x06002A0F RID: 10767 RVA: 0x0010ED70 File Offset: 0x0010CF70
			public float GetSurfaceHeight(Vector2 position)
			{
				return this.GetSurfaceHeight(this.GetPointAlong(position));
			}

			// Token: 0x06002A10 RID: 10768 RVA: 0x0010ED80 File Offset: 0x0010CF80
			public float GetSurfaceHeight(float position)
			{
				if (position < 0f || position > (float)this.Width)
				{
					return 0f;
				}
				float num = 0f;
				foreach (Water.Ripple ripple in this.Ripples)
				{
					float num2 = Math.Abs(ripple.Position - position);
					float num3;
					if (num2 < 12f)
					{
						num3 = Calc.ClampedMap(num2, 0f, 16f, 1f, -0.75f);
					}
					else
					{
						num3 = Calc.ClampedMap(num2, 16f, 32f, -0.75f, 0f);
					}
					num += num3 * ripple.Height * Ease.CubeIn(1f - ripple.Percent);
				}
				num = Calc.Clamp(num, -4f, 4f);
				foreach (Water.Tension tension in this.Tensions)
				{
					float t = Calc.ClampedMap(Math.Abs(tension.Position - position), 0f, 24f, 1f, 0f);
					num += Ease.CubeOut(t) * tension.Strength * 12f;
				}
				float val = position / (float)this.Width;
				num *= Calc.ClampedMap(val, 0f, 0.1f, 0.5f, 1f);
				num *= Calc.ClampedMap(val, 0.9f, 1f, 1f, 0.5f);
				num += (float)Math.Sin((double)(this.timer + position * 0.1f));
				num += 6f;
				return num;
			}

			// Token: 0x06002A11 RID: 10769 RVA: 0x0010EF68 File Offset: 0x0010D168
			public void Render(Camera camera)
			{
				GFX.DrawVertices<VertexPositionColor>(camera.Matrix, this.mesh, this.mesh.Length, null, null);
			}

			// Token: 0x04002917 RID: 10519
			public const int Resolution = 4;

			// Token: 0x04002918 RID: 10520
			public const float RaysPerPixel = 0.2f;

			// Token: 0x04002919 RID: 10521
			public const float BaseHeight = 6f;

			// Token: 0x0400291A RID: 10522
			public readonly Vector2 Outwards;

			// Token: 0x0400291B RID: 10523
			public readonly int Width;

			// Token: 0x0400291C RID: 10524
			public readonly int BodyHeight;

			// Token: 0x0400291D RID: 10525
			public Vector2 Position;

			// Token: 0x0400291E RID: 10526
			public List<Water.Ripple> Ripples = new List<Water.Ripple>();

			// Token: 0x0400291F RID: 10527
			public List<Water.Ray> Rays = new List<Water.Ray>();

			// Token: 0x04002920 RID: 10528
			public List<Water.Tension> Tensions = new List<Water.Tension>();

			// Token: 0x04002921 RID: 10529
			private float timer;

			// Token: 0x04002922 RID: 10530
			private VertexPositionColor[] mesh;

			// Token: 0x04002923 RID: 10531
			private int fillStartIndex;

			// Token: 0x04002924 RID: 10532
			private int rayStartIndex;

			// Token: 0x04002925 RID: 10533
			private int surfaceStartIndex;
		}
	}
}
