using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020001DE RID: 478
	public class WaveDashPresentation : Entity
	{
		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000FFF RID: 4095 RVA: 0x00044557 File Offset: 0x00042757
		// (set) Token: 0x06001000 RID: 4096 RVA: 0x0004455F File Offset: 0x0004275F
		public bool Viewing { get; private set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06001001 RID: 4097 RVA: 0x00044568 File Offset: 0x00042768
		// (set) Token: 0x06001002 RID: 4098 RVA: 0x00044570 File Offset: 0x00042770
		public Atlas Gfx { get; private set; }

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06001003 RID: 4099 RVA: 0x00044579 File Offset: 0x00042779
		public bool ShowInput
		{
			get
			{
				return this.waitingForPageTurn || (this.CurrPage != null && this.CurrPage.WaitingForInput);
			}
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06001004 RID: 4100 RVA: 0x0004459A File Offset: 0x0004279A
		private WaveDashPage PrevPage
		{
			get
			{
				if (this.pageIndex <= 0)
				{
					return null;
				}
				return this.pages[this.pageIndex - 1];
			}
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06001005 RID: 4101 RVA: 0x000445BA File Offset: 0x000427BA
		private WaveDashPage CurrPage
		{
			get
			{
				if (this.pageIndex >= this.pages.Count)
				{
					return null;
				}
				return this.pages[this.pageIndex];
			}
		}

		// Token: 0x06001006 RID: 4102 RVA: 0x000445E4 File Offset: 0x000427E4
		public WaveDashPresentation(EventInstance usingSfx = null)
		{
			base.Tag = Tags.HUD;
			this.Viewing = true;
			this.loading = true;
			base.Add(new Coroutine(this.Routine(), true));
			this.usingSfx = usingSfx;
			RunThread.Start(new Action(this.LoadingThread), "Wave Dash Presentation Loading", true);
		}

		// Token: 0x06001007 RID: 4103 RVA: 0x00044691 File Offset: 0x00042891
		private void LoadingThread()
		{
			this.Gfx = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "WaveDashing"), Atlas.AtlasDataFormat.Packer);
			this.loading = false;
		}

		// Token: 0x06001008 RID: 4104 RVA: 0x000446BA File Offset: 0x000428BA
		private IEnumerator Routine()
		{
			while (this.loading)
			{
				yield return null;
			}
			this.pages.Add(new WaveDashPage00());
			this.pages.Add(new WaveDashPage01());
			this.pages.Add(new WaveDashPage02());
			this.pages.Add(new WaveDashPage03());
			this.pages.Add(new WaveDashPage04());
			this.pages.Add(new WaveDashPage05());
			this.pages.Add(new WaveDashPage06());
			foreach (WaveDashPage waveDashPage in this.pages)
			{
				waveDashPage.Added(this);
			}
			base.Add(new BeforeRenderHook(new Action(this.BeforeRender)));
			while (this.ease < 1f)
			{
				this.ease = Calc.Approach(this.ease, 1f, Engine.DeltaTime * 2f);
				yield return null;
			}
			while (this.pageIndex < this.pages.Count)
			{
				this.pageUpdating = true;
				yield return this.CurrPage.Routine();
				if (!this.CurrPage.AutoProgress)
				{
					this.waitingForPageTurn = true;
					while (!Input.MenuConfirm.Pressed)
					{
						yield return null;
					}
					this.waitingForPageTurn = false;
					Audio.Play("event:/new_content/game/10_farewell/ppt_mouseclick");
				}
				this.pageUpdating = false;
				this.pageIndex++;
				if (this.pageIndex < this.pages.Count)
				{
					float num = 0.5f;
					if (this.CurrPage.Transition == WaveDashPage.Transitions.Rotate3D)
					{
						num = 1.5f;
					}
					else if (this.CurrPage.Transition == WaveDashPage.Transitions.Blocky)
					{
						num = 1f;
					}
					this.pageTurning = true;
					this.pageEase = 0f;
					base.Add(new Coroutine(this.TurnPage(num), true));
					yield return num * 0.8f;
				}
			}
			if (this.usingSfx != null)
			{
				Audio.SetParameter(this.usingSfx, "end", 1f);
				this.usingSfx.release();
			}
			Audio.Play("event:/new_content/game/10_farewell/cafe_computer_off");
			while (this.ease > 0f)
			{
				this.ease = Calc.Approach(this.ease, 0f, Engine.DeltaTime * 2f);
				yield return null;
			}
			this.Viewing = false;
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x06001009 RID: 4105 RVA: 0x000446C9 File Offset: 0x000428C9
		private IEnumerator TurnPage(float duration)
		{
			if (this.CurrPage.Transition != WaveDashPage.Transitions.ScaleIn && this.CurrPage.Transition != WaveDashPage.Transitions.FadeIn)
			{
				if (this.CurrPage.Transition == WaveDashPage.Transitions.Rotate3D)
				{
					Audio.Play("event:/new_content/game/10_farewell/ppt_cube_transition");
				}
				else if (this.CurrPage.Transition == WaveDashPage.Transitions.Blocky)
				{
					Audio.Play("event:/new_content/game/10_farewell/ppt_dissolve_transition");
				}
				else if (this.CurrPage.Transition == WaveDashPage.Transitions.Spiral)
				{
					Audio.Play("event:/new_content/game/10_farewell/ppt_spinning_transition");
				}
			}
			while (this.pageEase < 1f)
			{
				this.pageEase += Engine.DeltaTime / duration;
				yield return null;
			}
			this.pageTurning = false;
			yield break;
		}

		// Token: 0x0600100A RID: 4106 RVA: 0x000446E0 File Offset: 0x000428E0
		private void BeforeRender()
		{
			if (this.loading)
			{
				return;
			}
			if (this.screenBuffer == null || this.screenBuffer.IsDisposed)
			{
				this.screenBuffer = VirtualContent.CreateRenderTarget("WaveDash-Buffer", this.ScreenWidth, this.ScreenHeight, true, true, 0);
			}
			if (this.prevPageBuffer == null || this.prevPageBuffer.IsDisposed)
			{
				this.prevPageBuffer = VirtualContent.CreateRenderTarget("WaveDash-Screen1", this.ScreenWidth, this.ScreenHeight, false, true, 0);
			}
			if (this.currPageBuffer == null || this.currPageBuffer.IsDisposed)
			{
				this.currPageBuffer = VirtualContent.CreateRenderTarget("WaveDash-Screen2", this.ScreenWidth, this.ScreenHeight, false, true, 0);
			}
			if (this.pageTurning && this.PrevPage != null)
			{
				Engine.Graphics.GraphicsDevice.SetRenderTarget(this.prevPageBuffer);
				Engine.Graphics.GraphicsDevice.Clear(this.PrevPage.ClearColor);
				Draw.SpriteBatch.Begin();
				this.PrevPage.Render();
				Draw.SpriteBatch.End();
			}
			if (this.CurrPage != null)
			{
				Engine.Graphics.GraphicsDevice.SetRenderTarget(this.currPageBuffer);
				Engine.Graphics.GraphicsDevice.Clear(this.CurrPage.ClearColor);
				Draw.SpriteBatch.Begin();
				this.CurrPage.Render();
				Draw.SpriteBatch.End();
			}
			Engine.Graphics.GraphicsDevice.SetRenderTarget(this.screenBuffer);
			Engine.Graphics.GraphicsDevice.Clear(Color.Black);
			if (this.pageTurning)
			{
				if (this.CurrPage.Transition == WaveDashPage.Transitions.ScaleIn)
				{
					Draw.SpriteBatch.Begin();
					Draw.SpriteBatch.Draw(this.prevPageBuffer, Vector2.Zero, Color.White);
					Vector2 scale = Vector2.One * this.pageEase;
					Draw.SpriteBatch.Draw(this.currPageBuffer, this.ScaleInPoint, new Rectangle?(this.currPageBuffer.Bounds), Color.White, 0f, this.ScaleInPoint, scale, SpriteEffects.None, 0f);
					Draw.SpriteBatch.End();
					return;
				}
				if (this.CurrPage.Transition == WaveDashPage.Transitions.FadeIn)
				{
					Draw.SpriteBatch.Begin();
					Draw.SpriteBatch.Draw(this.prevPageBuffer, Vector2.Zero, Color.White);
					Draw.SpriteBatch.Draw(this.currPageBuffer, Vector2.Zero, Color.White * this.pageEase);
					Draw.SpriteBatch.End();
					return;
				}
				if (this.CurrPage.Transition == WaveDashPage.Transitions.Rotate3D)
				{
					float num = -1.5707964f * this.pageEase;
					this.RenderQuad(this.prevPageBuffer, this.pageEase, num);
					this.RenderQuad(this.currPageBuffer, this.pageEase, 1.5707964f + num);
					return;
				}
				if (this.CurrPage.Transition == WaveDashPage.Transitions.Blocky)
				{
					Draw.SpriteBatch.Begin();
					Draw.SpriteBatch.Draw(this.prevPageBuffer, Vector2.Zero, Color.White);
					uint num2 = 1U;
					int num3 = this.ScreenWidth / 60;
					for (int i = 0; i < this.ScreenWidth; i += num3)
					{
						for (int j = 0; j < this.ScreenHeight; j += num3)
						{
							if (WaveDashPresentation.PseudoRandRange(ref num2, 0f, 1f) <= this.pageEase)
							{
								Draw.SpriteBatch.Draw(this.currPageBuffer, new Rectangle(i, j, num3, num3), new Rectangle?(new Rectangle(i, j, num3, num3)), Color.White);
							}
						}
					}
					Draw.SpriteBatch.End();
					return;
				}
				if (this.CurrPage.Transition == WaveDashPage.Transitions.Spiral)
				{
					Draw.SpriteBatch.Begin();
					Draw.SpriteBatch.Draw(this.prevPageBuffer, Vector2.Zero, Color.White);
					Vector2 scale2 = Vector2.One * this.pageEase;
					float rotation = (1f - this.pageEase) * 12f;
					Draw.SpriteBatch.Draw(this.currPageBuffer, Celeste.TargetCenter, new Rectangle?(this.currPageBuffer.Bounds), Color.White, rotation, Celeste.TargetCenter, scale2, SpriteEffects.None, 0f);
					Draw.SpriteBatch.End();
					return;
				}
			}
			else
			{
				Draw.SpriteBatch.Begin();
				Draw.SpriteBatch.Draw(this.currPageBuffer, Vector2.Zero, Color.White);
				Draw.SpriteBatch.End();
			}
		}

		// Token: 0x0600100B RID: 4107 RVA: 0x00044B84 File Offset: 0x00042D84
		private void RenderQuad(Texture texture, float ease, float rotation)
		{
			float num = (float)this.screenBuffer.Width / (float)this.screenBuffer.Height;
			float num2 = num;
			float num3 = 1f;
			Vector3 position = new Vector3(-num2, num3, 0f);
			Vector3 position2 = new Vector3(num2, num3, 0f);
			Vector3 position3 = new Vector3(num2, -num3, 0f);
			Vector3 position4 = new Vector3(-num2, -num3, 0f);
			this.verts[0].Position = position;
			this.verts[0].TextureCoordinate = new Vector2(0f, 0f);
			this.verts[0].Color = Color.White;
			this.verts[1].Position = position2;
			this.verts[1].TextureCoordinate = new Vector2(1f, 0f);
			this.verts[1].Color = Color.White;
			this.verts[2].Position = position3;
			this.verts[2].TextureCoordinate = new Vector2(1f, 1f);
			this.verts[2].Color = Color.White;
			this.verts[3].Position = position;
			this.verts[3].TextureCoordinate = new Vector2(0f, 0f);
			this.verts[3].Color = Color.White;
			this.verts[4].Position = position3;
			this.verts[4].TextureCoordinate = new Vector2(1f, 1f);
			this.verts[4].Color = Color.White;
			this.verts[5].Position = position4;
			this.verts[5].TextureCoordinate = new Vector2(0f, 1f);
			this.verts[5].Color = Color.White;
			float num4 = 4.15f + Calc.YoYo(ease) * 1.7f;
			Matrix value = Matrix.CreateTranslation(0f, 0f, num) * Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(0f, 0f, -num4) * Matrix.CreatePerspectiveFieldOfView(0.7853982f, num, 1f, 10f);
			Engine.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			Engine.Instance.GraphicsDevice.BlendState = BlendState.AlphaBlend;
			Engine.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			Engine.Instance.GraphicsDevice.Textures[0] = texture;
			GFX.FxTexture.Parameters["World"].SetValue(value);
			foreach (EffectPass effectPass in GFX.FxTexture.CurrentTechnique.Passes)
			{
				effectPass.Apply();
				Engine.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, this.verts, 0, this.verts.Length / 3);
			}
		}

		// Token: 0x0600100C RID: 4108 RVA: 0x00044EE0 File Offset: 0x000430E0
		public override void Update()
		{
			base.Update();
			if (this.ShowInput)
			{
				this.waitingForInputTime += Engine.DeltaTime;
			}
			else
			{
				this.waitingForInputTime = 0f;
			}
			if (!this.loading && this.CurrPage != null && this.pageUpdating)
			{
				this.CurrPage.Update();
			}
		}

		// Token: 0x0600100D RID: 4109 RVA: 0x00044F40 File Offset: 0x00043140
		public override void Render()
		{
			if (!this.loading && this.screenBuffer != null && !this.screenBuffer.IsDisposed)
			{
				float num = (float)this.ScreenWidth * Ease.CubeOut(Calc.ClampedMap(this.ease, 0f, 0.5f, 0f, 1f));
				float num2 = (float)this.ScreenHeight * Ease.CubeInOut(Calc.ClampedMap(this.ease, 0.5f, 1f, 0.2f, 1f));
				Rectangle rectangle = new Rectangle((int)((1920f - num) / 2f), (int)((1080f - num2) / 2f), (int)num, (int)num2);
				Draw.SpriteBatch.Draw(this.screenBuffer, rectangle, Color.White);
				if (this.ShowInput && this.waitingForInputTime > 0.2f)
				{
					GFX.Gui["textboxbutton"].DrawCentered(new Vector2(1856f, (float)(1016 + ((base.Scene.TimeActive % 1f < 0.25f) ? 6 : 0))), Color.Black);
				}
				if ((base.Scene as Level).Paused)
				{
					Draw.Rect(rectangle, Color.Black * 0.7f);
				}
			}
		}

		// Token: 0x0600100E RID: 4110 RVA: 0x0004509B File Offset: 0x0004329B
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			this.Dispose();
		}

		// Token: 0x0600100F RID: 4111 RVA: 0x000450AA File Offset: 0x000432AA
		public override void SceneEnd(Scene scene)
		{
			base.SceneEnd(scene);
			this.Dispose();
		}

		// Token: 0x06001010 RID: 4112 RVA: 0x000450BC File Offset: 0x000432BC
		private void Dispose()
		{
			while (this.loading)
			{
				Thread.Sleep(1);
			}
			if (this.screenBuffer != null)
			{
				this.screenBuffer.Dispose();
			}
			this.screenBuffer = null;
			if (this.prevPageBuffer != null)
			{
				this.prevPageBuffer.Dispose();
			}
			this.prevPageBuffer = null;
			if (this.currPageBuffer != null)
			{
				this.currPageBuffer.Dispose();
			}
			this.currPageBuffer = null;
			this.Gfx.Dispose();
			this.Gfx = null;
		}

		// Token: 0x06001011 RID: 4113 RVA: 0x0004513C File Offset: 0x0004333C
		private static uint PseudoRand(ref uint seed)
		{
			uint num = seed;
			num ^= num << 13;
			num ^= num >> 17;
			num ^= num << 5;
			seed = num;
			return num;
		}

		// Token: 0x06001012 RID: 4114 RVA: 0x00045164 File Offset: 0x00043364
		public static float PseudoRandRange(ref uint seed, float min, float max)
		{
			return min + WaveDashPresentation.PseudoRand(ref seed) % 1000U / 1000f * (max - min);
		}

		// Token: 0x04000B5C RID: 2908
		public Vector2 ScaleInPoint = new Vector2(1920f, 1080f) / 2f;

		// Token: 0x04000B5D RID: 2909
		public readonly int ScreenWidth = 1920;

		// Token: 0x04000B5E RID: 2910
		public readonly int ScreenHeight = 1080;

		// Token: 0x04000B5F RID: 2911
		private float ease;

		// Token: 0x04000B60 RID: 2912
		private bool loading;

		// Token: 0x04000B61 RID: 2913
		private float waitingForInputTime;

		// Token: 0x04000B62 RID: 2914
		private VirtualRenderTarget screenBuffer;

		// Token: 0x04000B63 RID: 2915
		private VirtualRenderTarget prevPageBuffer;

		// Token: 0x04000B64 RID: 2916
		private VirtualRenderTarget currPageBuffer;

		// Token: 0x04000B65 RID: 2917
		private int pageIndex;

		// Token: 0x04000B66 RID: 2918
		private List<WaveDashPage> pages = new List<WaveDashPage>();

		// Token: 0x04000B67 RID: 2919
		private float pageEase;

		// Token: 0x04000B68 RID: 2920
		private bool pageTurning;

		// Token: 0x04000B69 RID: 2921
		private bool pageUpdating;

		// Token: 0x04000B6A RID: 2922
		private bool waitingForPageTurn;

		// Token: 0x04000B6B RID: 2923
		private VertexPositionColorTexture[] verts = new VertexPositionColorTexture[6];

		// Token: 0x04000B6C RID: 2924
		private EventInstance usingSfx;
	}
}
