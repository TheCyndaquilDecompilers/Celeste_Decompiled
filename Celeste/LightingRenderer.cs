using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200023F RID: 575
	public class LightingRenderer : Renderer
	{
		// Token: 0x06001237 RID: 4663 RVA: 0x0005D93C File Offset: 0x0005BB3C
		public LightingRenderer()
		{
			this.lights = new VertexLight[64];
			for (int i = 0; i < 20; i++)
			{
				this.angles[i] = new Vector3(Calc.AngleToVector((float)i / 20f * 6.2831855f, 1f), 0f);
			}
		}

		// Token: 0x06001238 RID: 4664 RVA: 0x0005D9F5 File Offset: 0x0005BBF5
		public VertexLight SetSpotlight(VertexLight light)
		{
			this.spotlight = light;
			this.inSpotlight = true;
			return light;
		}

		// Token: 0x06001239 RID: 4665 RVA: 0x0005DA06 File Offset: 0x0005BC06
		public void UnsetSpotlight()
		{
			this.inSpotlight = false;
		}

		// Token: 0x0600123A RID: 4666 RVA: 0x0005DA0F File Offset: 0x0005BC0F
		public override void Update(Scene scene)
		{
			this.nonSpotlightAlphaMultiplier = Calc.Approach(this.nonSpotlightAlphaMultiplier, this.inSpotlight ? 0f : 1f, Engine.DeltaTime * 2f);
			base.Update(scene);
		}

		// Token: 0x0600123B RID: 4667 RVA: 0x0005DA48 File Offset: 0x0005BC48
		public override void BeforeRender(Scene scene)
		{
			Level level = scene as Level;
			Camera camera = level.Camera;
			for (int i = 0; i < 64; i++)
			{
				if (this.lights[i] != null && this.lights[i].Entity.Scene != scene)
				{
					this.lights[i].Index = -1;
					this.lights[i] = null;
				}
			}
			foreach (Component component in scene.Tracker.GetComponents<VertexLight>())
			{
				VertexLight vertexLight = (VertexLight)component;
				if (vertexLight.Entity != null && vertexLight.Entity.Visible && vertexLight.Visible && vertexLight.Alpha > 0f && vertexLight.Color.A > 0 && vertexLight.Center.X + vertexLight.EndRadius > camera.X && vertexLight.Center.Y + vertexLight.EndRadius > camera.Y && vertexLight.Center.X - vertexLight.EndRadius < camera.X + 320f && vertexLight.Center.Y - vertexLight.EndRadius < camera.Y + 180f)
				{
					if (vertexLight.Index < 0)
					{
						vertexLight.Dirty = true;
						for (int j = 0; j < 64; j++)
						{
							if (this.lights[j] == null)
							{
								this.lights[j] = vertexLight;
								vertexLight.Index = j;
								break;
							}
						}
					}
					if (vertexLight.LastPosition != vertexLight.Position || vertexLight.LastEntityPosition != vertexLight.Entity.Position || vertexLight.Dirty)
					{
						vertexLight.LastPosition = vertexLight.Position;
						vertexLight.InSolid = false;
						using (List<Solid>.Enumerator enumerator2 = scene.CollideAll<Solid>(vertexLight.Center).GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.DisableLightsInside)
								{
									vertexLight.InSolid = true;
									break;
								}
							}
						}
						if (!vertexLight.InSolid)
						{
							vertexLight.LastNonSolidPosition = vertexLight.Center;
						}
						if (vertexLight.InSolid && !vertexLight.Started)
						{
							vertexLight.InSolidAlphaMultiplier = 0f;
						}
					}
					if (vertexLight.Entity.Position != vertexLight.LastEntityPosition)
					{
						vertexLight.Dirty = true;
						vertexLight.LastEntityPosition = vertexLight.Entity.Position;
					}
					vertexLight.Started = true;
				}
				else if (vertexLight.Index >= 0)
				{
					this.lights[vertexLight.Index] = null;
					vertexLight.Index = -1;
					vertexLight.Started = false;
				}
			}
			Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.LightBuffer);
			Engine.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			Matrix matrix = Matrix.CreateScale(0.0009765625f) * Matrix.CreateScale(2f, -2f, 1f) * Matrix.CreateTranslation(-1f, 1f, 0f);
			this.ClearDirtyLights(matrix);
			this.DrawLightGradients(matrix);
			this.DrawLightOccluders(matrix, level);
			Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.Light);
			Engine.Graphics.GraphicsDevice.Clear(this.BaseColor);
			Engine.Graphics.GraphicsDevice.Textures[0] = GameplayBuffers.LightBuffer;
			this.StartDrawingPrimitives();
			for (int k = 0; k < 64; k++)
			{
				VertexLight vertexLight2 = this.lights[k];
				if (vertexLight2 != null)
				{
					vertexLight2.Dirty = false;
					float num = vertexLight2.Alpha * vertexLight2.InSolidAlphaMultiplier;
					if (this.nonSpotlightAlphaMultiplier < 1f && vertexLight2 != this.spotlight)
					{
						num *= this.nonSpotlightAlphaMultiplier;
					}
					if (num > 0f && vertexLight2.Color.A > 0 && vertexLight2.EndRadius >= 2f)
					{
						int num2 = 128;
						while (vertexLight2.EndRadius <= (float)(num2 / 2))
						{
							num2 /= 2;
						}
						this.DrawLight(k, vertexLight2.InSolid ? vertexLight2.LastNonSolidPosition : vertexLight2.Center, vertexLight2.Color * num, (float)num2);
					}
				}
			}
			if (this.vertexCount > 0)
			{
				GFX.DrawIndexedVertices<LightingRenderer.VertexPositionColorMaskTexture>(camera.Matrix, this.resultVerts, this.vertexCount, this.indices, this.indexCount / 3, GFX.FxLighting, BlendState.Additive);
			}
			GaussianBlur.Blur(GameplayBuffers.Light, GameplayBuffers.TempA, GameplayBuffers.Light, 0f, true, GaussianBlur.Samples.Nine, 1f, GaussianBlur.Direction.Both, 1f);
		}

		// Token: 0x0600123C RID: 4668 RVA: 0x0005DF74 File Offset: 0x0005C174
		private void ClearDirtyLights(Matrix matrix)
		{
			this.StartDrawingPrimitives();
			for (int i = 0; i < 64; i++)
			{
				VertexLight vertexLight = this.lights[i];
				if (vertexLight != null && vertexLight.Dirty)
				{
					this.SetClear(i);
				}
			}
			if (this.vertexCount > 0)
			{
				Engine.Instance.GraphicsDevice.BlendState = LightingRenderer.OccludeBlendState;
				GFX.FxPrimitive.Parameters["World"].SetValue(matrix);
				foreach (EffectPass effectPass in GFX.FxPrimitive.CurrentTechnique.Passes)
				{
					effectPass.Apply();
					Engine.Instance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, this.verts, 0, this.vertexCount, this.indices, 0, this.indexCount / 3);
				}
			}
		}

		// Token: 0x0600123D RID: 4669 RVA: 0x0005E064 File Offset: 0x0005C264
		private void DrawLightGradients(Matrix matrix)
		{
			this.StartDrawingPrimitives();
			int num = 0;
			for (int i = 0; i < 64; i++)
			{
				VertexLight vertexLight = this.lights[i];
				if (vertexLight != null && vertexLight.Dirty)
				{
					num++;
					this.SetGradient(i, Calc.Clamp(vertexLight.StartRadius, 0f, 120f), Calc.Clamp(vertexLight.EndRadius, 0f, 120f));
				}
			}
			if (this.vertexCount > 0)
			{
				Engine.Instance.GraphicsDevice.BlendState = LightingRenderer.GradientBlendState;
				GFX.FxPrimitive.Parameters["World"].SetValue(matrix);
				foreach (EffectPass effectPass in GFX.FxPrimitive.CurrentTechnique.Passes)
				{
					effectPass.Apply();
					Engine.Instance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, this.verts, 0, this.vertexCount, this.indices, 0, this.indexCount / 3);
				}
			}
		}

		// Token: 0x0600123E RID: 4670 RVA: 0x0005E184 File Offset: 0x0005C384
		private void DrawLightOccluders(Matrix matrix, Level level)
		{
			this.StartDrawingPrimitives();
			Rectangle tileBounds = level.Session.MapData.TileBounds;
			List<Component> components = level.Tracker.GetComponents<LightOcclude>();
			List<Component> components2 = level.Tracker.GetComponents<EffectCutout>();
			foreach (Component component in components)
			{
				LightOcclude lightOcclude = (LightOcclude)component;
				if (lightOcclude.Visible && lightOcclude.Entity.Visible)
				{
					lightOcclude.RenderBounds = new Rectangle(lightOcclude.Left, lightOcclude.Top, lightOcclude.Width, lightOcclude.Height);
				}
			}
			for (int i = 0; i < 64; i++)
			{
				VertexLight vertexLight = this.lights[i];
				if (vertexLight != null && vertexLight.Dirty)
				{
					Vector2 vector = vertexLight.InSolid ? vertexLight.LastNonSolidPosition : vertexLight.Center;
					Rectangle rectangle = new Rectangle((int)(vector.X - vertexLight.EndRadius), (int)(vector.Y - vertexLight.EndRadius), (int)vertexLight.EndRadius * 2, (int)vertexLight.EndRadius * 2);
					Vector3 center = this.GetCenter(i);
					Color mask = this.GetMask(i, 0f, 1f);
					foreach (Component component2 in components)
					{
						LightOcclude lightOcclude2 = (LightOcclude)component2;
						if (lightOcclude2.Visible && lightOcclude2.Entity.Visible && lightOcclude2.Alpha > 0f)
						{
							Rectangle rect = lightOcclude2.RenderBounds;
							if (rect.Intersects(rectangle))
							{
								rect = rect.ClampTo(rectangle);
								Color mask2 = this.GetMask(i, 1f - lightOcclude2.Alpha, 1f);
								if (rect.Bottom > rectangle.Top && rect.Bottom < rectangle.Center.Y)
								{
									this.SetOccluder(center, mask2, vector, new Vector2((float)rect.Left, (float)rect.Bottom), new Vector2((float)rect.Right, (float)rect.Bottom));
								}
								if (rect.Top < rectangle.Bottom && rect.Top > rectangle.Center.Y)
								{
									this.SetOccluder(center, mask2, vector, new Vector2((float)rect.Left, (float)rect.Top), new Vector2((float)rect.Right, (float)rect.Top));
								}
								if (rect.Right > rectangle.Left && rect.Right < rectangle.Center.X)
								{
									this.SetOccluder(center, mask2, vector, new Vector2((float)rect.Right, (float)rect.Top), new Vector2((float)rect.Right, (float)rect.Bottom));
								}
								if (rect.Left < rectangle.Right && rect.Left > rectangle.Center.X)
								{
									this.SetOccluder(center, mask2, vector, new Vector2((float)rect.Left, (float)rect.Top), new Vector2((float)rect.Left, (float)rect.Bottom));
								}
							}
						}
					}
					int num = rectangle.Left / 8 - tileBounds.Left;
					int num2 = rectangle.Top / 8 - tileBounds.Top;
					int num3 = rectangle.Height / 8;
					int num4 = rectangle.Width / 8;
					int num5 = num + num4;
					int num6 = num2 + num3;
					for (int j = num2; j < num2 + num3 / 2; j++)
					{
						for (int k = num; k < num5; k++)
						{
							if (level.SolidsData.SafeCheck(k, j) != '0' && level.SolidsData.SafeCheck(k, j + 1) == '0')
							{
								int num7 = k;
								do
								{
									k++;
								}
								while (k < num5 && level.SolidsData.SafeCheck(k, j) != '0' && level.SolidsData.SafeCheck(k, j + 1) == '0');
								this.SetOccluder(center, mask, vector, new Vector2((float)(tileBounds.X + num7), (float)(tileBounds.Y + j + 1)) * 8f, new Vector2((float)(tileBounds.X + k), (float)(tileBounds.Y + j + 1)) * 8f);
							}
						}
					}
					for (int l = num; l < num + num4 / 2; l++)
					{
						for (int m = num2; m < num6; m++)
						{
							if (level.SolidsData.SafeCheck(l, m) != '0' && level.SolidsData.SafeCheck(l + 1, m) == '0')
							{
								int num8 = m;
								do
								{
									m++;
								}
								while (m < num6 && level.SolidsData.SafeCheck(l, m) != '0' && level.SolidsData.SafeCheck(l + 1, m) == '0');
								this.SetOccluder(center, mask, vector, new Vector2((float)(tileBounds.X + l + 1), (float)(tileBounds.Y + num8)) * 8f, new Vector2((float)(tileBounds.X + l + 1), (float)(tileBounds.Y + m)) * 8f);
							}
						}
					}
					for (int n = num2 + num3 / 2; n < num6; n++)
					{
						for (int num9 = num; num9 < num5; num9++)
						{
							if (level.SolidsData.SafeCheck(num9, n) != '0' && level.SolidsData.SafeCheck(num9, n - 1) == '0')
							{
								int num10 = num9;
								do
								{
									num9++;
								}
								while (num9 < num5 && level.SolidsData.SafeCheck(num9, n) != '0' && level.SolidsData.SafeCheck(num9, n - 1) == '0');
								this.SetOccluder(center, mask, vector, new Vector2((float)(tileBounds.X + num10), (float)(tileBounds.Y + n)) * 8f, new Vector2((float)(tileBounds.X + num9), (float)(tileBounds.Y + n)) * 8f);
							}
						}
					}
					for (int num11 = num + num4 / 2; num11 < num5; num11++)
					{
						for (int num12 = num2; num12 < num6; num12++)
						{
							if (level.SolidsData.SafeCheck(num11, num12) != '0' && level.SolidsData.SafeCheck(num11 - 1, num12) == '0')
							{
								int num13 = num12;
								do
								{
									num12++;
								}
								while (num12 < num6 && level.SolidsData.SafeCheck(num11, num12) != '0' && level.SolidsData.SafeCheck(num11 - 1, num12) == '0');
								this.SetOccluder(center, mask, vector, new Vector2((float)(tileBounds.X + num11), (float)(tileBounds.Y + num13)) * 8f, new Vector2((float)(tileBounds.X + num11), (float)(tileBounds.Y + num12)) * 8f);
							}
						}
					}
					foreach (Component component3 in components2)
					{
						EffectCutout effectCutout = (EffectCutout)component3;
						if (effectCutout.Visible && effectCutout.Entity.Visible && effectCutout.Alpha > 0f)
						{
							Rectangle rectangle2 = effectCutout.Bounds;
							if (rectangle2.Intersects(rectangle))
							{
								rectangle2 = rectangle2.ClampTo(rectangle);
								Color mask3 = this.GetMask(i, 1f - effectCutout.Alpha, 1f);
								this.SetCutout(center, mask3, vector, (float)rectangle2.X, (float)rectangle2.Y, (float)rectangle2.Width, (float)rectangle2.Height);
							}
						}
					}
					for (int num14 = num; num14 < num5; num14++)
					{
						for (int num15 = num2; num15 < num6; num15++)
						{
							if (level.FgTilesLightMask.Tiles.SafeCheck(num14, num15) != null)
							{
								this.SetCutout(center, mask, vector, (float)((tileBounds.X + num14) * 8), (float)((tileBounds.Y + num15) * 8), 8f, 8f);
							}
						}
					}
				}
			}
			if (this.vertexCount > 0)
			{
				Engine.Instance.GraphicsDevice.BlendState = LightingRenderer.OccludeBlendState;
				GFX.FxPrimitive.Parameters["World"].SetValue(matrix);
				foreach (EffectPass effectPass in GFX.FxPrimitive.CurrentTechnique.Passes)
				{
					effectPass.Apply();
					Engine.Instance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, this.verts, 0, this.vertexCount, this.indices, 0, this.indexCount / 3);
				}
			}
		}

		// Token: 0x0600123F RID: 4671 RVA: 0x0005EB20 File Offset: 0x0005CD20
		private Color GetMask(int index, float maskOn, float maskOff)
		{
			int num = index / 16;
			return new Color((num == 0) ? maskOn : maskOff, (num == 1) ? maskOn : maskOff, (num == 2) ? maskOn : maskOff, (num == 3) ? maskOn : maskOff);
		}

		// Token: 0x06001240 RID: 4672 RVA: 0x0005EB58 File Offset: 0x0005CD58
		private Vector3 GetCenter(int index)
		{
			int num = index % 16;
			return new Vector3(128f * ((float)(num % 4) + 0.5f) * 2f, 128f * ((float)(num / 4) + 0.5f) * 2f, 0f);
		}

		// Token: 0x06001241 RID: 4673 RVA: 0x0005EBA0 File Offset: 0x0005CDA0
		private void StartDrawingPrimitives()
		{
			this.vertexCount = 0;
			this.indexCount = 0;
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x0005EBB0 File Offset: 0x0005CDB0
		private void SetClear(int index)
		{
			Vector3 center = this.GetCenter(index);
			Color mask = this.GetMask(index, 0f, 1f);
			int[] array = this.indices;
			int num = this.indexCount;
			this.indexCount = num + 1;
			array[num] = this.vertexCount;
			int[] array2 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array2[num] = this.vertexCount + 1;
			int[] array3 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array3[num] = this.vertexCount + 2;
			int[] array4 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array4[num] = this.vertexCount;
			int[] array5 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array5[num] = this.vertexCount + 2;
			int[] array6 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array6[num] = this.vertexCount + 3;
			this.verts[this.vertexCount].Position = center + new Vector3(-128f, -128f, 0f);
			VertexPositionColor[] array7 = this.verts;
			num = this.vertexCount;
			this.vertexCount = num + 1;
			array7[num].Color = mask;
			this.verts[this.vertexCount].Position = center + new Vector3(128f, -128f, 0f);
			VertexPositionColor[] array8 = this.verts;
			num = this.vertexCount;
			this.vertexCount = num + 1;
			array8[num].Color = mask;
			this.verts[this.vertexCount].Position = center + new Vector3(128f, 128f, 0f);
			VertexPositionColor[] array9 = this.verts;
			num = this.vertexCount;
			this.vertexCount = num + 1;
			array9[num].Color = mask;
			this.verts[this.vertexCount].Position = center + new Vector3(-128f, 128f, 0f);
			VertexPositionColor[] array10 = this.verts;
			num = this.vertexCount;
			this.vertexCount = num + 1;
			array10[num].Color = mask;
		}

		// Token: 0x06001243 RID: 4675 RVA: 0x0005EDDC File Offset: 0x0005CFDC
		private void SetGradient(int index, float startFade, float endFade)
		{
			Vector3 center = this.GetCenter(index);
			Color mask = this.GetMask(index, 1f, 0f);
			int num = this.vertexCount;
			this.verts[this.vertexCount].Position = center;
			this.verts[this.vertexCount].Color = mask;
			this.vertexCount++;
			for (int i = 0; i < 20; i++)
			{
				this.verts[this.vertexCount].Position = center + this.angles[i] * startFade;
				this.verts[this.vertexCount].Color = mask;
				this.vertexCount++;
				this.verts[this.vertexCount].Position = center + this.angles[i] * endFade;
				this.verts[this.vertexCount].Color = Color.Transparent;
				this.vertexCount++;
				int num2 = i;
				int num3 = (i + 1) % 20;
				int[] array = this.indices;
				int num4 = this.indexCount;
				this.indexCount = num4 + 1;
				array[num4] = num;
				int[] array2 = this.indices;
				num4 = this.indexCount;
				this.indexCount = num4 + 1;
				array2[num4] = num + 1 + num2 * 2;
				int[] array3 = this.indices;
				num4 = this.indexCount;
				this.indexCount = num4 + 1;
				array3[num4] = num + 1 + num3 * 2;
				int[] array4 = this.indices;
				num4 = this.indexCount;
				this.indexCount = num4 + 1;
				array4[num4] = num + 1 + num2 * 2;
				int[] array5 = this.indices;
				num4 = this.indexCount;
				this.indexCount = num4 + 1;
				array5[num4] = num + 2 + num2 * 2;
				int[] array6 = this.indices;
				num4 = this.indexCount;
				this.indexCount = num4 + 1;
				array6[num4] = num + 2 + num3 * 2;
				int[] array7 = this.indices;
				num4 = this.indexCount;
				this.indexCount = num4 + 1;
				array7[num4] = num + 1 + num2 * 2;
				int[] array8 = this.indices;
				num4 = this.indexCount;
				this.indexCount = num4 + 1;
				array8[num4] = num + 2 + num3 * 2;
				int[] array9 = this.indices;
				num4 = this.indexCount;
				this.indexCount = num4 + 1;
				array9[num4] = num + 1 + num3 * 2;
			}
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x0005F044 File Offset: 0x0005D244
		private void SetOccluder(Vector3 center, Color mask, Vector2 light, Vector2 edgeA, Vector2 edgeB)
		{
			Vector2 vector = (edgeA - light).Floor();
			Vector2 vector2 = (edgeB - light).Floor();
			float num = vector.Angle();
			float num2 = vector2.Angle();
			int num3 = this.vertexCount;
			this.verts[this.vertexCount].Position = center + new Vector3(vector, 0f);
			VertexPositionColor[] array = this.verts;
			int num4 = this.vertexCount;
			this.vertexCount = num4 + 1;
			array[num4].Color = mask;
			this.verts[this.vertexCount].Position = center + new Vector3(vector2, 0f);
			VertexPositionColor[] array2 = this.verts;
			num4 = this.vertexCount;
			this.vertexCount = num4 + 1;
			array2[num4].Color = mask;
			while (num != num2)
			{
				this.verts[this.vertexCount].Position = center + new Vector3(Calc.AngleToVector(num, 128f), 0f);
				this.verts[this.vertexCount].Color = mask;
				int[] array3 = this.indices;
				num4 = this.indexCount;
				this.indexCount = num4 + 1;
				array3[num4] = num3;
				int[] array4 = this.indices;
				num4 = this.indexCount;
				this.indexCount = num4 + 1;
				array4[num4] = this.vertexCount;
				int[] array5 = this.indices;
				num4 = this.indexCount;
				this.indexCount = num4 + 1;
				array5[num4] = this.vertexCount + 1;
				this.vertexCount++;
				num = Calc.AngleApproach(num, num2, 0.7853982f);
			}
			this.verts[this.vertexCount].Position = center + new Vector3(Calc.AngleToVector(num, 128f), 0f);
			this.verts[this.vertexCount].Color = mask;
			int[] array6 = this.indices;
			num4 = this.indexCount;
			this.indexCount = num4 + 1;
			array6[num4] = num3;
			int[] array7 = this.indices;
			num4 = this.indexCount;
			this.indexCount = num4 + 1;
			array7[num4] = this.vertexCount;
			int[] array8 = this.indices;
			num4 = this.indexCount;
			this.indexCount = num4 + 1;
			array8[num4] = num3 + 1;
			this.vertexCount++;
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x0005F2A0 File Offset: 0x0005D4A0
		private void SetCutout(Vector3 center, Color mask, Vector2 light, float x, float y, float width, float height)
		{
			int[] array = this.indices;
			int num = this.indexCount;
			this.indexCount = num + 1;
			array[num] = this.vertexCount;
			int[] array2 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array2[num] = this.vertexCount + 1;
			int[] array3 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array3[num] = this.vertexCount + 2;
			int[] array4 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array4[num] = this.vertexCount;
			int[] array5 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array5[num] = this.vertexCount + 2;
			int[] array6 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array6[num] = this.vertexCount + 3;
			this.verts[this.vertexCount].Position = center + new Vector3(x - light.X, y - light.Y, 0f);
			VertexPositionColor[] array7 = this.verts;
			num = this.vertexCount;
			this.vertexCount = num + 1;
			array7[num].Color = mask;
			this.verts[this.vertexCount].Position = center + new Vector3(x + width - light.X, y - light.Y, 0f);
			VertexPositionColor[] array8 = this.verts;
			num = this.vertexCount;
			this.vertexCount = num + 1;
			array8[num].Color = mask;
			this.verts[this.vertexCount].Position = center + new Vector3(x + width - light.X, y + height - light.Y, 0f);
			VertexPositionColor[] array9 = this.verts;
			num = this.vertexCount;
			this.vertexCount = num + 1;
			array9[num].Color = mask;
			this.verts[this.vertexCount].Position = center + new Vector3(x - light.X, y + height - light.Y, 0f);
			VertexPositionColor[] array10 = this.verts;
			num = this.vertexCount;
			this.vertexCount = num + 1;
			array10[num].Color = mask;
		}

		// Token: 0x06001246 RID: 4678 RVA: 0x0005F4E0 File Offset: 0x0005D6E0
		private void DrawLight(int index, Vector2 position, Color color, float radius)
		{
			Vector3 center = this.GetCenter(index);
			Color mask = this.GetMask(index, 1f, 0f);
			int[] array = this.indices;
			int num = this.indexCount;
			this.indexCount = num + 1;
			array[num] = this.vertexCount;
			int[] array2 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array2[num] = this.vertexCount + 1;
			int[] array3 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array3[num] = this.vertexCount + 2;
			int[] array4 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array4[num] = this.vertexCount;
			int[] array5 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array5[num] = this.vertexCount + 2;
			int[] array6 = this.indices;
			num = this.indexCount;
			this.indexCount = num + 1;
			array6[num] = this.vertexCount + 3;
			this.resultVerts[this.vertexCount].Position = new Vector3(position + new Vector2(-radius, -radius), 0f);
			this.resultVerts[this.vertexCount].Color = color;
			this.resultVerts[this.vertexCount].Mask = mask;
			LightingRenderer.VertexPositionColorMaskTexture[] array7 = this.resultVerts;
			num = this.vertexCount;
			this.vertexCount = num + 1;
			array7[num].Texcoord = new Vector2(center.X - radius, center.Y - radius) / 1024f;
			this.resultVerts[this.vertexCount].Position = new Vector3(position + new Vector2(radius, -radius), 0f);
			this.resultVerts[this.vertexCount].Color = color;
			this.resultVerts[this.vertexCount].Mask = mask;
			LightingRenderer.VertexPositionColorMaskTexture[] array8 = this.resultVerts;
			num = this.vertexCount;
			this.vertexCount = num + 1;
			array8[num].Texcoord = new Vector2(center.X + radius, center.Y - radius) / 1024f;
			this.resultVerts[this.vertexCount].Position = new Vector3(position + new Vector2(radius, radius), 0f);
			this.resultVerts[this.vertexCount].Color = color;
			this.resultVerts[this.vertexCount].Mask = mask;
			LightingRenderer.VertexPositionColorMaskTexture[] array9 = this.resultVerts;
			num = this.vertexCount;
			this.vertexCount = num + 1;
			array9[num].Texcoord = new Vector2(center.X + radius, center.Y + radius) / 1024f;
			this.resultVerts[this.vertexCount].Position = new Vector3(position + new Vector2(-radius, radius), 0f);
			this.resultVerts[this.vertexCount].Color = color;
			this.resultVerts[this.vertexCount].Mask = mask;
			LightingRenderer.VertexPositionColorMaskTexture[] array10 = this.resultVerts;
			num = this.vertexCount;
			this.vertexCount = num + 1;
			array10[num].Texcoord = new Vector2(center.X - radius, center.Y + radius) / 1024f;
		}

		// Token: 0x06001247 RID: 4679 RVA: 0x0005F844 File Offset: 0x0005DA44
		public override void Render(Scene scene)
		{
			GFX.FxDither.CurrentTechnique = GFX.FxDither.Techniques["InvertDither"];
			GFX.FxDither.Parameters["size"].SetValue(new Vector2((float)GameplayBuffers.Light.Width, (float)GameplayBuffers.Light.Height));
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, GFX.DestinationTransparencySubtract, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, GFX.FxDither, Matrix.Identity);
			Draw.SpriteBatch.Draw(GameplayBuffers.Light, Vector2.Zero, Color.White * MathHelper.Clamp(this.Alpha, 0f, 1f));
			Draw.SpriteBatch.End();
		}

		// Token: 0x04000DE1 RID: 3553
		public static BlendState GradientBlendState = new BlendState
		{
			AlphaBlendFunction = BlendFunction.Max,
			ColorBlendFunction = BlendFunction.Max,
			ColorSourceBlend = Blend.One,
			ColorDestinationBlend = Blend.One,
			AlphaSourceBlend = Blend.One,
			AlphaDestinationBlend = Blend.One
		};

		// Token: 0x04000DE2 RID: 3554
		public static BlendState OccludeBlendState = new BlendState
		{
			AlphaBlendFunction = BlendFunction.Min,
			ColorBlendFunction = BlendFunction.Min,
			ColorSourceBlend = Blend.One,
			ColorDestinationBlend = Blend.One,
			AlphaSourceBlend = Blend.One,
			AlphaDestinationBlend = Blend.One
		};

		// Token: 0x04000DE3 RID: 3555
		public const int TextureSize = 1024;

		// Token: 0x04000DE4 RID: 3556
		public const int TextureSplit = 4;

		// Token: 0x04000DE5 RID: 3557
		public const int Channels = 4;

		// Token: 0x04000DE6 RID: 3558
		public const int Padding = 8;

		// Token: 0x04000DE7 RID: 3559
		public const int CircleSegments = 20;

		// Token: 0x04000DE8 RID: 3560
		private const int Cells = 16;

		// Token: 0x04000DE9 RID: 3561
		private const int MaxLights = 64;

		// Token: 0x04000DEA RID: 3562
		private const int Radius = 128;

		// Token: 0x04000DEB RID: 3563
		private const int LightRadius = 120;

		// Token: 0x04000DEC RID: 3564
		public Color BaseColor = Color.Black;

		// Token: 0x04000DED RID: 3565
		public float Alpha = 0.1f;

		// Token: 0x04000DEE RID: 3566
		private VertexPositionColor[] verts = new VertexPositionColor[11520];

		// Token: 0x04000DEF RID: 3567
		private LightingRenderer.VertexPositionColorMaskTexture[] resultVerts = new LightingRenderer.VertexPositionColorMaskTexture[384];

		// Token: 0x04000DF0 RID: 3568
		private int[] indices = new int[11520];

		// Token: 0x04000DF1 RID: 3569
		private int vertexCount;

		// Token: 0x04000DF2 RID: 3570
		private int indexCount;

		// Token: 0x04000DF3 RID: 3571
		private VertexLight[] lights;

		// Token: 0x04000DF4 RID: 3572
		private VertexLight spotlight;

		// Token: 0x04000DF5 RID: 3573
		private bool inSpotlight;

		// Token: 0x04000DF6 RID: 3574
		private float nonSpotlightAlphaMultiplier = 1f;

		// Token: 0x04000DF7 RID: 3575
		private Vector3[] angles = new Vector3[20];

		// Token: 0x02000565 RID: 1381
		private struct VertexPositionColorMaskTexture : IVertexType
		{
			// Token: 0x1700048A RID: 1162
			// (get) Token: 0x0600266F RID: 9839 RVA: 0x000FCF89 File Offset: 0x000FB189
			VertexDeclaration IVertexType.VertexDeclaration
			{
				get
				{
					return LightingRenderer.VertexPositionColorMaskTexture.VertexDeclaration;
				}
			}

			// Token: 0x0400264E RID: 9806
			public Vector3 Position;

			// Token: 0x0400264F RID: 9807
			public Color Color;

			// Token: 0x04002650 RID: 9808
			public Color Mask;

			// Token: 0x04002651 RID: 9809
			public Vector2 Texcoord;

			// Token: 0x04002652 RID: 9810
			public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[]
			{
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
				new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 1),
				new VertexElement(20, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
			});
		}
	}
}
