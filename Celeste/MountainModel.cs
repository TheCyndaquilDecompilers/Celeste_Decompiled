using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000254 RID: 596
	public class MountainModel : IDisposable
	{
		// Token: 0x0600129C RID: 4764 RVA: 0x000632B0 File Offset: 0x000614B0
		public MountainModel()
		{
			this.mountainStates[0] = new MountainState(MTN.MountainTerrainTextures[0], MTN.MountainBuildingTextures[0], MTN.MountainSkyboxTextures[0], Calc.HexToColor("010817"));
			this.mountainStates[1] = new MountainState(MTN.MountainTerrainTextures[1], MTN.MountainBuildingTextures[1], MTN.MountainSkyboxTextures[1], Calc.HexToColor("13203E"));
			this.mountainStates[2] = new MountainState(MTN.MountainTerrainTextures[2], MTN.MountainBuildingTextures[2], MTN.MountainSkyboxTextures[2], Calc.HexToColor("281A35"));
			this.mountainStates[3] = new MountainState(MTN.MountainTerrainTextures[0], MTN.MountainBuildingTextures[0], MTN.MountainSkyboxTextures[0], Calc.HexToColor("010817"));
			this.fog = new Ring(6f, -1f, 20f, 0f, 24, Color.White, MTN.MountainFogTexture, 1f);
			this.fog2 = new Ring(6f, -4f, 10f, 0f, 24, Color.White, MTN.MountainFogTexture, 1f);
			this.starsky = new Ring(18f, -18f, 20f, 0f, 24, Color.White, Color.Transparent, MTN.MountainStarSky, 1f);
			this.starfog = new Ring(10f, -18f, 19.5f, 0f, 24, Calc.HexToColor("020915"), Color.Transparent, MTN.MountainFogTexture, 1f);
			this.stardots0 = new Ring(16f, -18f, 19f, 0f, 24, Color.White, Color.Transparent, MTN.MountainStars, 4f);
			this.starstream0 = new Ring(5f, -8f, 18.5f, 0.2f, 80, Color.Black, MTN.MountainStarStream, 1f);
			this.starstream1 = new Ring(4f, -6f, 18f, 1f, 80, Calc.HexToColor("9228e2") * 0.5f, MTN.MountainStarStream, 1f);
			this.starstream2 = new Ring(3f, -4f, 17.9f, 1.4f, 80, Calc.HexToColor("30ffff") * 0.5f, MTN.MountainStarStream, 1f);
			this.ResetRenderTargets();
			this.ResetBillboardBuffers();
		}

		// Token: 0x0600129D RID: 4765 RVA: 0x000635A4 File Offset: 0x000617A4
		public void SnapState(int state)
		{
			this.currState = (this.nextState = (this.targetState = state % this.mountainStates.Length));
			this.easeState = 1f;
			if (state == 3)
			{
				this.StarEase = 1f;
			}
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x000635ED File Offset: 0x000617ED
		public void EaseState(int state)
		{
			this.targetState = state % this.mountainStates.Length;
			this.lastCameraRotation = this.Camera.Rotation;
		}

		// Token: 0x0600129F RID: 4767 RVA: 0x00063610 File Offset: 0x00061810
		public void Update()
		{
			if (this.currState != this.nextState)
			{
				this.easeState = Calc.Approach(this.easeState, 1f, (float)((this.nextState == this.targetState) ? 1 : 4) * Engine.DeltaTime);
				if (this.easeState >= 1f)
				{
					this.currState = this.nextState;
				}
			}
			else if (this.nextState != this.targetState)
			{
				this.nextState = this.targetState;
				this.easeState = 0f;
			}
			this.StarEase = Calc.Approach(this.StarEase, (this.nextState == 3) ? 1f : 0f, ((this.nextState == 3) ? 1.5f : 1f) * Engine.DeltaTime);
			this.SnowForceFloat = Calc.ClampedMap(this.StarEase, 0.95f, 1f, 0f, 1f);
			this.ignoreCameraRotation = ((this.nextState == 3 && this.currState != 3 && this.StarEase < 0.5f) || (this.nextState != 3 && this.currState == 3 && this.StarEase > 0.5f));
			if (this.nextState == 3)
			{
				this.SnowStretch = Calc.ClampedMap(this.StarEase, 0f, 0.25f, 0f, 1f) * 50f;
				this.SnowSpeedAddition = this.SnowStretch * 4f;
			}
			else
			{
				this.SnowStretch = Calc.ClampedMap(this.StarEase, 0.25f, 1f, 0f, 1f) * 50f;
				this.SnowSpeedAddition = -this.SnowStretch * 4f;
			}
			this.starfog.Rotate(-Engine.DeltaTime * 0.01f);
			this.fog.Rotate(-Engine.DeltaTime * 0.01f);
			this.fog.TopColor = (this.fog.BotColor = Color.Lerp(this.mountainStates[this.currState].FogColor, this.mountainStates[this.nextState].FogColor, this.easeState));
			this.fog2.Rotate(-Engine.DeltaTime * 0.01f);
			this.fog2.TopColor = (this.fog2.BotColor = Color.White * 0.3f * this.NearFogAlpha);
			this.starstream1.Rotate(Engine.DeltaTime * 0.01f);
			this.starstream2.Rotate(Engine.DeltaTime * 0.02f);
			this.birdTimer += Engine.DeltaTime;
		}

		// Token: 0x060012A0 RID: 4768 RVA: 0x000638D0 File Offset: 0x00061AD0
		public void ResetRenderTargets()
		{
			int num = Math.Min(1920, Engine.ViewWidth);
			int num2 = Math.Min(1080, Engine.ViewHeight);
			if (this.buffer == null || this.buffer.IsDisposed || (this.buffer.Width != num && !this.LockBufferResizing))
			{
				this.DisposeTargets();
				this.buffer = VirtualContent.CreateRenderTarget("mountain-a", num, num2, true, false, 0);
				this.blurA = VirtualContent.CreateRenderTarget("mountain-blur-a", num / 2, num2 / 2, false, true, 0);
				this.blurB = VirtualContent.CreateRenderTarget("mountain-blur-b", num / 2, num2 / 2, false, true, 0);
			}
		}

		// Token: 0x060012A1 RID: 4769 RVA: 0x00063978 File Offset: 0x00061B78
		public void ResetBillboardBuffers()
		{
			if (this.billboardVertices == null || this.billboardIndices.IsDisposed || this.billboardIndices.GraphicsDevice.IsDisposed || this.billboardVertices.IsDisposed || this.billboardVertices.GraphicsDevice.IsDisposed || this.billboardInfo.Length > this.billboardVertices.VertexCount)
			{
				this.DisposeBillboardBuffers();
				this.billboardVertices = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexPositionColorTexture), this.billboardInfo.Length, BufferUsage.None);
				this.billboardIndices = new IndexBuffer(Engine.Graphics.GraphicsDevice, typeof(short), this.billboardInfo.Length / 4 * 6, BufferUsage.None);
				short[] array = new short[this.billboardIndices.IndexCount];
				int i = 0;
				int num = 0;
				while (i < array.Length)
				{
					array[i] = (short)num;
					array[i + 1] = (short)(num + 1);
					array[i + 2] = (short)(num + 2);
					array[i + 3] = (short)num;
					array[i + 4] = (short)(num + 2);
					array[i + 5] = (short)(num + 3);
					i += 6;
					num += 4;
				}
				this.billboardIndices.SetData<short>(array);
			}
		}

		// Token: 0x060012A2 RID: 4770 RVA: 0x00063A9E File Offset: 0x00061C9E
		public void Dispose()
		{
			this.DisposeTargets();
			this.DisposeBillboardBuffers();
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x00063AAC File Offset: 0x00061CAC
		public void DisposeTargets()
		{
			if (this.buffer != null && !this.buffer.IsDisposed)
			{
				this.buffer.Dispose();
				this.blurA.Dispose();
				this.blurB.Dispose();
			}
		}

		// Token: 0x060012A4 RID: 4772 RVA: 0x00063AE4 File Offset: 0x00061CE4
		public void DisposeBillboardBuffers()
		{
			if (this.billboardVertices != null && !this.billboardVertices.IsDisposed)
			{
				this.billboardVertices.Dispose();
			}
			if (this.billboardIndices != null && !this.billboardIndices.IsDisposed)
			{
				this.billboardIndices.Dispose();
			}
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x00063B34 File Offset: 0x00061D34
		public void BeforeRender(Scene scene)
		{
			this.ResetRenderTargets();
			Quaternion rotation = this.Camera.Rotation;
			if (this.ignoreCameraRotation)
			{
				rotation = this.lastCameraRotation;
			}
			Matrix matrix = Matrix.CreatePerspectiveFieldOfView(0.7853982f, (float)Engine.Width / (float)Engine.Height, 0.25f, 50f);
			Matrix matrix2 = Matrix.CreateTranslation(-this.Camera.Position) * Matrix.CreateFromQuaternion(rotation);
			Matrix matrix3 = matrix2 * matrix;
			this.Forward = Vector3.Transform(Vector3.Forward, this.Camera.Rotation.Conjugated());
			Engine.Graphics.GraphicsDevice.SetRenderTarget(this.buffer);
			if (this.StarEase < 1f)
			{
				Matrix matrix4 = Matrix.CreateTranslation(0f, 5f - this.Camera.Position.Y * 1.1f, 0f) * Matrix.CreateFromQuaternion(rotation) * matrix;
				if (this.currState == this.nextState)
				{
					this.mountainStates[this.currState].Skybox.Draw(matrix4, Color.White);
				}
				else
				{
					this.mountainStates[this.currState].Skybox.Draw(matrix4, Color.White);
					this.mountainStates[this.nextState].Skybox.Draw(matrix4, Color.White * this.easeState);
				}
				if (this.currState != this.nextState)
				{
					GFX.FxMountain.Parameters["ease"].SetValue(this.easeState);
					GFX.FxMountain.CurrentTechnique = GFX.FxMountain.Techniques["Easing"];
				}
				else
				{
					GFX.FxMountain.CurrentTechnique = GFX.FxMountain.Techniques["Single"];
				}
				Engine.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
				Engine.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
				Engine.Graphics.GraphicsDevice.RasterizerState = MountainModel.MountainRasterizer;
				GFX.FxMountain.Parameters["WorldViewProj"].SetValue(matrix3);
				GFX.FxMountain.Parameters["fog"].SetValue(this.fog.TopColor.ToVector3());
				Engine.Graphics.GraphicsDevice.Textures[0] = this.mountainStates[this.currState].TerrainTexture.Texture;
				Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
				if (this.currState != this.nextState)
				{
					Engine.Graphics.GraphicsDevice.Textures[1] = this.mountainStates[this.nextState].TerrainTexture.Texture;
					Engine.Graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
				}
				MTN.MountainTerrain.Draw(GFX.FxMountain);
				GFX.FxMountain.Parameters["WorldViewProj"].SetValue(Matrix.CreateTranslation(this.CoreWallPosition) * matrix3);
				MTN.MountainCoreWall.Draw(GFX.FxMountain);
				GFX.FxMountain.Parameters["WorldViewProj"].SetValue(matrix3);
				Engine.Graphics.GraphicsDevice.Textures[0] = this.mountainStates[this.currState].BuildingsTexture.Texture;
				Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
				if (this.currState != this.nextState)
				{
					Engine.Graphics.GraphicsDevice.Textures[1] = this.mountainStates[this.nextState].BuildingsTexture.Texture;
					Engine.Graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
				}
				MTN.MountainBuildings.Draw(GFX.FxMountain);
				this.fog.Draw(matrix3, null, 1f);
			}
			if (this.StarEase > 0f)
			{
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null);
				Draw.Rect(0f, 0f, (float)this.buffer.Width, (float)this.buffer.Height, Color.Black * Ease.CubeInOut(Calc.ClampedMap(this.StarEase, 0f, 0.6f, 0f, 1f)));
				Draw.SpriteBatch.End();
				Matrix matrix5 = Matrix.CreateTranslation(this.starCenter - this.Camera.Position) * Matrix.CreateFromQuaternion(rotation) * matrix;
				float alpha = Calc.ClampedMap(this.StarEase, 0.8f, 1f, 0f, 1f);
				this.starsky.Draw(matrix5, MountainModel.CullCCRasterizer, alpha);
				this.starfog.Draw(matrix5, MountainModel.CullCCRasterizer, alpha);
				this.stardots0.Draw(matrix5, MountainModel.CullCCRasterizer, alpha);
				this.starstream0.Draw(matrix5, MountainModel.CullCCRasterizer, alpha);
				this.starstream1.Draw(matrix5, MountainModel.CullCCRasterizer, alpha);
				this.starstream2.Draw(matrix5, MountainModel.CullCCRasterizer, alpha);
				Engine.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
				Engine.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
				Engine.Graphics.GraphicsDevice.RasterizerState = MountainModel.CullCRasterizer;
				Engine.Graphics.GraphicsDevice.Textures[0] = MTN.MountainMoonTexture.Texture;
				Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
				GFX.FxMountain.CurrentTechnique = GFX.FxMountain.Techniques["Single"];
				GFX.FxMountain.Parameters["WorldViewProj"].SetValue(matrix3);
				GFX.FxMountain.Parameters["fog"].SetValue(this.fog.TopColor.ToVector3());
				MTN.MountainMoon.Draw(GFX.FxMountain);
				float num = this.birdTimer * 0.2f;
				Matrix matrix6 = Matrix.CreateScale(0.25f) * Matrix.CreateRotationZ((float)Math.Cos((double)(num * 2f)) * 0.5f) * Matrix.CreateRotationX(0.4f + (float)Math.Sin((double)num) * 0.05f) * Matrix.CreateRotationY(-num - 1.5707964f) * Matrix.CreateTranslation((float)Math.Cos((double)num) * 2.2f, 31f + (float)Math.Sin((double)(num * 2f)) * 0.8f, (float)Math.Sin((double)num) * 2.2f);
				GFX.FxMountain.Parameters["WorldViewProj"].SetValue(matrix6 * matrix3);
				GFX.FxMountain.Parameters["fog"].SetValue(this.fog.TopColor.ToVector3());
				MTN.MountainBird.Draw(GFX.FxMountain);
			}
			this.DrawBillboards(matrix3, scene.Tracker.GetComponents<Billboard>());
			if (this.StarEase < 1f)
			{
				this.fog2.Draw(matrix3, MountainModel.CullCRasterizer, 1f);
			}
			if (this.DrawDebugPoints && this.DebugPoints.Count > 0)
			{
				GFX.FxDebug.World = Matrix.Identity;
				GFX.FxDebug.View = matrix2;
				GFX.FxDebug.Projection = matrix;
				GFX.FxDebug.TextureEnabled = false;
				GFX.FxDebug.VertexColorEnabled = true;
				VertexPositionColor[] array = this.DebugPoints.ToArray();
				foreach (EffectPass effectPass in GFX.FxDebug.CurrentTechnique.Passes)
				{
					effectPass.Apply();
					Engine.Graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, array, 0, array.Length / 3);
				}
			}
			GaussianBlur.Blur(this.buffer, this.blurA, this.blurB, 0.75f, true, GaussianBlur.Samples.Five, 1f, GaussianBlur.Direction.Both, 1f);
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x000643B8 File Offset: 0x000625B8
		private void DrawBillboards(Matrix matrix, List<Component> billboards)
		{
			int num = 0;
			int num2 = this.billboardInfo.Length / 4;
			Vector3 value = Vector3.Transform(Vector3.Left, this.Camera.Rotation.LookAt(Vector3.Zero, this.Forward, Vector3.Up).Conjugated());
			Vector3 value2 = Vector3.Transform(Vector3.Up, this.Camera.Rotation.LookAt(Vector3.Zero, this.Forward, Vector3.Up).Conjugated());
			foreach (Component component in billboards)
			{
				Billboard billboard = (Billboard)component;
				if (billboard.Entity.Visible && billboard.Visible)
				{
					if (billboard.BeforeRender != null)
					{
						billboard.BeforeRender();
					}
					if (billboard.Color.A >= 0 && billboard.Size.X != 0f && billboard.Size.Y != 0f && billboard.Scale.X != 0f && billboard.Scale.Y != 0f && billboard.Texture != null)
					{
						if (num < num2)
						{
							Vector3 position = billboard.Position;
							Vector3 vector = value * billboard.Size.X * billboard.Scale.X;
							Vector3 vector2 = value2 * billboard.Size.Y * billboard.Scale.Y;
							Vector3 value3 = -vector;
							Vector3 value4 = -vector2;
							int num3 = num * 4;
							int num4 = num * 4 + 1;
							int num5 = num * 4 + 2;
							int num6 = num * 4 + 3;
							this.billboardInfo[num3].Color = billboard.Color;
							this.billboardInfo[num3].TextureCoordinate.X = billboard.Texture.LeftUV;
							this.billboardInfo[num3].TextureCoordinate.Y = billboard.Texture.BottomUV;
							this.billboardInfo[num3].Position = position + vector + value4;
							this.billboardInfo[num4].Color = billboard.Color;
							this.billboardInfo[num4].TextureCoordinate.X = billboard.Texture.LeftUV;
							this.billboardInfo[num4].TextureCoordinate.Y = billboard.Texture.TopUV;
							this.billboardInfo[num4].Position = position + vector + vector2;
							this.billboardInfo[num5].Color = billboard.Color;
							this.billboardInfo[num5].TextureCoordinate.X = billboard.Texture.RightUV;
							this.billboardInfo[num5].TextureCoordinate.Y = billboard.Texture.TopUV;
							this.billboardInfo[num5].Position = position + value3 + vector2;
							this.billboardInfo[num6].Color = billboard.Color;
							this.billboardInfo[num6].TextureCoordinate.X = billboard.Texture.RightUV;
							this.billboardInfo[num6].TextureCoordinate.Y = billboard.Texture.BottomUV;
							this.billboardInfo[num6].Position = position + value3 + value4;
							this.billboardTextures[num] = billboard.Texture.Texture.Texture;
						}
						num++;
					}
				}
			}
			this.ResetBillboardBuffers();
			if (num > 0)
			{
				this.billboardVertices.SetData<VertexPositionColorTexture>(this.billboardInfo);
				Engine.Graphics.GraphicsDevice.SetVertexBuffer(this.billboardVertices);
				Engine.Graphics.GraphicsDevice.Indices = this.billboardIndices;
				Engine.Graphics.GraphicsDevice.RasterizerState = MountainModel.CullNoneRasterizer;
				Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
				Engine.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
				Engine.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
				GFX.FxTexture.Parameters["World"].SetValue(matrix);
				int num7 = Math.Min(num, this.billboardInfo.Length / 4);
				Texture2D texture2D = this.billboardTextures[0];
				int num8 = 0;
				for (int i = 1; i < num7; i++)
				{
					if (this.billboardTextures[i] != texture2D)
					{
						this.DrawBillboardBatch(texture2D, num8, i - num8);
						texture2D = this.billboardTextures[i];
						num8 = i;
					}
				}
				this.DrawBillboardBatch(texture2D, num8, num7 - num8);
				if (num * 4 > this.billboardInfo.Length)
				{
					this.billboardInfo = new VertexPositionColorTexture[this.billboardInfo.Length * 2];
					this.billboardTextures = new Texture2D[this.billboardInfo.Length / 4];
				}
			}
		}

		// Token: 0x060012A7 RID: 4775 RVA: 0x00064930 File Offset: 0x00062B30
		private void DrawBillboardBatch(Texture2D texture, int offset, int sprites)
		{
			Engine.Graphics.GraphicsDevice.Textures[0] = texture;
			foreach (EffectPass effectPass in GFX.FxTexture.CurrentTechnique.Passes)
			{
				effectPass.Apply();
				Engine.Graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, offset * 4, 0, sprites * 4, 0, sprites * 2);
			}
		}

		// Token: 0x060012A8 RID: 4776 RVA: 0x000649BC File Offset: 0x00062BBC
		public void Render()
		{
			float num = (float)Engine.ViewWidth / (float)this.buffer.Width;
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null);
			Draw.SpriteBatch.Draw(this.buffer, Vector2.Zero, new Rectangle?(this.buffer.Bounds), Color.White * 1f, 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
			Draw.SpriteBatch.Draw(this.blurB, Vector2.Zero, new Rectangle?(this.blurB.Bounds), Color.White, 0f, Vector2.Zero, num * 2f, SpriteEffects.None, 0f);
			Draw.SpriteBatch.End();
		}

		// Token: 0x04000E7B RID: 3707
		public MountainCamera Camera;

		// Token: 0x04000E7C RID: 3708
		public Vector3 Forward;

		// Token: 0x04000E7D RID: 3709
		public float SkyboxOffset;

		// Token: 0x04000E7E RID: 3710
		public bool LockBufferResizing;

		// Token: 0x04000E7F RID: 3711
		private VirtualRenderTarget buffer;

		// Token: 0x04000E80 RID: 3712
		private VirtualRenderTarget blurA;

		// Token: 0x04000E81 RID: 3713
		private VirtualRenderTarget blurB;

		// Token: 0x04000E82 RID: 3714
		public static RasterizerState MountainRasterizer = new RasterizerState
		{
			CullMode = CullMode.CullClockwiseFace,
			MultiSampleAntiAlias = true
		};

		// Token: 0x04000E83 RID: 3715
		public static RasterizerState CullNoneRasterizer = new RasterizerState
		{
			CullMode = CullMode.None,
			MultiSampleAntiAlias = false
		};

		// Token: 0x04000E84 RID: 3716
		public static RasterizerState CullCCRasterizer = new RasterizerState
		{
			CullMode = CullMode.CullCounterClockwiseFace,
			MultiSampleAntiAlias = false
		};

		// Token: 0x04000E85 RID: 3717
		public static RasterizerState CullCRasterizer = new RasterizerState
		{
			CullMode = CullMode.CullClockwiseFace,
			MultiSampleAntiAlias = false
		};

		// Token: 0x04000E86 RID: 3718
		private int currState;

		// Token: 0x04000E87 RID: 3719
		private int nextState;

		// Token: 0x04000E88 RID: 3720
		private int targetState;

		// Token: 0x04000E89 RID: 3721
		private float easeState = 1f;

		// Token: 0x04000E8A RID: 3722
		private MountainState[] mountainStates = new MountainState[4];

		// Token: 0x04000E8B RID: 3723
		public Vector3 CoreWallPosition = Vector3.Zero;

		// Token: 0x04000E8C RID: 3724
		private VertexBuffer billboardVertices;

		// Token: 0x04000E8D RID: 3725
		private IndexBuffer billboardIndices;

		// Token: 0x04000E8E RID: 3726
		private VertexPositionColorTexture[] billboardInfo = new VertexPositionColorTexture[2048];

		// Token: 0x04000E8F RID: 3727
		private Texture2D[] billboardTextures = new Texture2D[512];

		// Token: 0x04000E90 RID: 3728
		private Ring fog;

		// Token: 0x04000E91 RID: 3729
		private Ring fog2;

		// Token: 0x04000E92 RID: 3730
		public float NearFogAlpha;

		// Token: 0x04000E93 RID: 3731
		public float StarEase;

		// Token: 0x04000E94 RID: 3732
		public float SnowStretch;

		// Token: 0x04000E95 RID: 3733
		public float SnowSpeedAddition = 1f;

		// Token: 0x04000E96 RID: 3734
		public float SnowForceFloat;

		// Token: 0x04000E97 RID: 3735
		private Ring starsky;

		// Token: 0x04000E98 RID: 3736
		private Ring starfog;

		// Token: 0x04000E99 RID: 3737
		private Ring stardots0;

		// Token: 0x04000E9A RID: 3738
		private Ring starstream0;

		// Token: 0x04000E9B RID: 3739
		private Ring starstream1;

		// Token: 0x04000E9C RID: 3740
		private Ring starstream2;

		// Token: 0x04000E9D RID: 3741
		private bool ignoreCameraRotation;

		// Token: 0x04000E9E RID: 3742
		private Quaternion lastCameraRotation;

		// Token: 0x04000E9F RID: 3743
		private Vector3 starCenter = new Vector3(0f, 32f, 0f);

		// Token: 0x04000EA0 RID: 3744
		private float birdTimer;

		// Token: 0x04000EA1 RID: 3745
		public List<VertexPositionColor> DebugPoints = new List<VertexPositionColor>();

		// Token: 0x04000EA2 RID: 3746
		public bool DrawDebugPoints;
	}
}
