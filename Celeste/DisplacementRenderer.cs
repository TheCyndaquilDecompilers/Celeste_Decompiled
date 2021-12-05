using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000367 RID: 871
	public class DisplacementRenderer : Renderer
	{
		// Token: 0x06001B5E RID: 7006 RVA: 0x000B30F5 File Offset: 0x000B12F5
		public bool HasDisplacement(Scene scene)
		{
			return this.points.Count > 0 || scene.Tracker.GetComponent<DisplacementRenderHook>() != null || (scene as Level).Foreground.Get<HeatWave>() != null;
		}

		// Token: 0x06001B5F RID: 7007 RVA: 0x000B3127 File Offset: 0x000B1327
		public DisplacementRenderer.Burst Add(DisplacementRenderer.Burst point)
		{
			this.points.Add(point);
			return point;
		}

		// Token: 0x06001B60 RID: 7008 RVA: 0x000B3136 File Offset: 0x000B1336
		public DisplacementRenderer.Burst Remove(DisplacementRenderer.Burst point)
		{
			this.points.Remove(point);
			return point;
		}

		// Token: 0x06001B61 RID: 7009 RVA: 0x000B3148 File Offset: 0x000B1348
		public DisplacementRenderer.Burst AddBurst(Vector2 position, float duration, float radiusFrom, float radiusTo, float alpha = 1f, Ease.Easer alphaEaser = null, Ease.Easer radiusEaser = null)
		{
			MTexture mtexture = GFX.Game["util/displacementcircle"];
			return this.Add(new DisplacementRenderer.Burst(mtexture, position, mtexture.Center, duration)
			{
				ScaleFrom = radiusFrom / (float)(mtexture.Width / 2),
				ScaleTo = radiusTo / (float)(mtexture.Width / 2),
				AlphaFrom = alpha,
				AlphaTo = 0f,
				AlphaEaser = alphaEaser
			});
		}

		// Token: 0x06001B62 RID: 7010 RVA: 0x000B31BC File Offset: 0x000B13BC
		public override void Update(Scene scene)
		{
			this.timer += Engine.DeltaTime;
			for (int i = this.points.Count - 1; i >= 0; i--)
			{
				if (this.points[i].Percent >= 1f)
				{
					this.points.RemoveAt(i);
				}
				else
				{
					this.points[i].Update();
				}
			}
		}

		// Token: 0x06001B63 RID: 7011 RVA: 0x000B322A File Offset: 0x000B142A
		public void Clear()
		{
			this.points.Clear();
		}

		// Token: 0x06001B64 RID: 7012 RVA: 0x000B3238 File Offset: 0x000B1438
		public override void BeforeRender(Scene scene)
		{
			Distort.WaterSine = this.timer * 16f;
			Distort.WaterCameraY = (float)((int)Math.Floor((double)(scene as Level).Camera.Y));
			Camera camera = (scene as Level).Camera;
			Color color = new Color(0.5f, 0.5f, 0f, 1f);
			Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.Displacement.Target);
			Engine.Graphics.GraphicsDevice.Clear(color);
			if (this.Enabled)
			{
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, camera.Matrix);
				HeatWave heatWave = (scene as Level).Foreground.Get<HeatWave>();
				if (heatWave != null)
				{
					heatWave.RenderDisplacement(scene as Level);
				}
				foreach (Component component in scene.Tracker.GetComponents<DisplacementRenderHook>())
				{
					DisplacementRenderHook displacementRenderHook = (DisplacementRenderHook)component;
					if (displacementRenderHook.Visible && displacementRenderHook.RenderDisplacement != null)
					{
						displacementRenderHook.RenderDisplacement();
					}
				}
				foreach (DisplacementRenderer.Burst burst in this.points)
				{
					burst.Render();
				}
				foreach (Entity entity in scene.Tracker.GetEntities<FakeWall>())
				{
					Draw.Rect(entity.X, entity.Y, entity.Width, entity.Height, color);
				}
				Draw.SpriteBatch.End();
			}
		}

		// Token: 0x04001873 RID: 6259
		public bool Enabled = true;

		// Token: 0x04001874 RID: 6260
		private float timer;

		// Token: 0x04001875 RID: 6261
		private List<DisplacementRenderer.Burst> points = new List<DisplacementRenderer.Burst>();

		// Token: 0x0200071B RID: 1819
		public class Burst
		{
			// Token: 0x06002E36 RID: 11830 RVA: 0x00123CCC File Offset: 0x00121ECC
			public Burst(MTexture texture, Vector2 position, Vector2 origin, float duration)
			{
				this.Texture = texture;
				this.Position = position;
				this.Origin = origin;
				this.Duration = duration;
			}

			// Token: 0x06002E37 RID: 11831 RVA: 0x00123D07 File Offset: 0x00121F07
			public void Update()
			{
				this.Percent += Engine.DeltaTime / this.Duration;
			}

			// Token: 0x06002E38 RID: 11832 RVA: 0x00123D24 File Offset: 0x00121F24
			public void Render()
			{
				Vector2 vector = this.Position;
				if (this.Follow != null)
				{
					vector += this.Follow.Position;
				}
				float scale;
				if (this.AlphaEaser != null)
				{
					scale = this.AlphaFrom + (this.AlphaTo - this.AlphaFrom) * this.AlphaEaser(this.Percent);
				}
				else
				{
					scale = this.AlphaFrom + (this.AlphaTo - this.AlphaFrom) * this.Percent;
				}
				float num;
				if (this.ScaleEaser != null)
				{
					num = this.ScaleFrom + (this.ScaleTo - this.ScaleFrom) * this.ScaleEaser(this.Percent);
				}
				else
				{
					num = this.ScaleFrom + (this.ScaleTo - this.ScaleFrom) * this.Percent;
				}
				Vector2 origin = this.Origin;
				Rectangle clip = new Rectangle(0, 0, this.Texture.Width, this.Texture.Height);
				if (this.WorldClipCollider != null)
				{
					this.WorldClipRect = new Rectangle?(this.WorldClipCollider.Bounds);
				}
				if (this.WorldClipRect != null)
				{
					Rectangle value = this.WorldClipRect.Value;
					value.X -= 1 + this.WorldClipPadding;
					value.Y -= 1 + this.WorldClipPadding;
					value.Width += 1 + this.WorldClipPadding * 2;
					value.Height += 1 + this.WorldClipPadding * 2;
					float num2 = vector.X - origin.X * num;
					if (num2 < (float)value.Left)
					{
						int num3 = (int)(((float)value.Left - num2) / num);
						origin.X -= (float)num3;
						clip.X = num3;
						clip.Width -= num3;
					}
					float num4 = vector.Y - origin.Y * num;
					if (num4 < (float)value.Top)
					{
						int num5 = (int)(((float)value.Top - num4) / num);
						origin.Y -= (float)num5;
						clip.Y = num5;
						clip.Height -= num5;
					}
					float num6 = vector.X + ((float)this.Texture.Width - origin.X) * num;
					if (num6 > (float)value.Right)
					{
						int num7 = (int)((num6 - (float)value.Right) / num);
						clip.Width -= num7;
					}
					float num8 = vector.Y + ((float)this.Texture.Height - origin.Y) * num;
					if (num8 > (float)value.Bottom)
					{
						int num9 = (int)((num8 - (float)value.Bottom) / num);
						clip.Height -= num9;
					}
				}
				this.Texture.Draw(vector, origin, Color.White * scale, Vector2.One * num, 0f, clip);
			}

			// Token: 0x04002DB5 RID: 11701
			public MTexture Texture;

			// Token: 0x04002DB6 RID: 11702
			public Entity Follow;

			// Token: 0x04002DB7 RID: 11703
			public Vector2 Position;

			// Token: 0x04002DB8 RID: 11704
			public Vector2 Origin;

			// Token: 0x04002DB9 RID: 11705
			public float Duration;

			// Token: 0x04002DBA RID: 11706
			public float Percent;

			// Token: 0x04002DBB RID: 11707
			public float ScaleFrom;

			// Token: 0x04002DBC RID: 11708
			public float ScaleTo = 1f;

			// Token: 0x04002DBD RID: 11709
			public Ease.Easer ScaleEaser;

			// Token: 0x04002DBE RID: 11710
			public float AlphaFrom = 1f;

			// Token: 0x04002DBF RID: 11711
			public float AlphaTo;

			// Token: 0x04002DC0 RID: 11712
			public Ease.Easer AlphaEaser;

			// Token: 0x04002DC1 RID: 11713
			public Rectangle? WorldClipRect;

			// Token: 0x04002DC2 RID: 11714
			public Collider WorldClipCollider;

			// Token: 0x04002DC3 RID: 11715
			public int WorldClipPadding;
		}
	}
}
