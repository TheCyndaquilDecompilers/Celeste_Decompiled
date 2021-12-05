using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000232 RID: 562
	public class CompleteRenderer : HiresRenderer, IDisposable
	{
		// Token: 0x17000140 RID: 320
		// (get) Token: 0x060011DF RID: 4575 RVA: 0x000599F9 File Offset: 0x00057BF9
		// (set) Token: 0x060011E0 RID: 4576 RVA: 0x00059A01 File Offset: 0x00057C01
		public bool HasUI { get; private set; }

		// Token: 0x060011E1 RID: 4577 RVA: 0x00059A0C File Offset: 0x00057C0C
		public CompleteRenderer(XmlElement xml, Atlas atlas, float delay, Action onDoneSlide = null)
		{
			this.atlas = atlas;
			this.xml = xml;
			if (xml != null)
			{
				if (xml["start"] != null)
				{
					this.StartScroll = xml["start"].Position();
				}
				if (xml["center"] != null)
				{
					this.CenterScroll = xml["center"].Position();
				}
				if (xml["offset"] != null)
				{
					this.Offset = xml["offset"].Position();
				}
				foreach (object obj in xml["layers"])
				{
					if (obj is XmlElement)
					{
						XmlElement xmlElement = obj as XmlElement;
						if (xmlElement.Name == "layer")
						{
							this.Layers.Add(new CompleteRenderer.ImageLayer(this.Offset, atlas, xmlElement));
						}
						else if (xmlElement.Name == "ui")
						{
							this.HasUI = true;
							this.Layers.Add(new CompleteRenderer.UILayer(this, xmlElement));
						}
					}
				}
			}
			this.Scroll = this.StartScroll;
			this.routine = new Coroutine(this.SlideRoutine(delay, onDoneSlide), true);
		}

		// Token: 0x060011E2 RID: 4578 RVA: 0x00059B8C File Offset: 0x00057D8C
		public void Dispose()
		{
			if (this.atlas != null)
			{
				this.atlas.Dispose();
			}
		}

		// Token: 0x060011E3 RID: 4579 RVA: 0x00059BA1 File Offset: 0x00057DA1
		private IEnumerator SlideRoutine(float delay, Action onDoneSlide)
		{
			yield return delay;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / this.SlideDuration)
			{
				yield return null;
				this.Scroll = Vector2.Lerp(this.StartScroll, this.CenterScroll, Ease.SineOut(p));
				this.fadeAlpha = Calc.LerpClamp(1f, 0f, p * 2f);
			}
			this.Scroll = this.CenterScroll;
			this.fadeAlpha = 0f;
			yield return 0.2f;
			if (onDoneSlide != null)
			{
				onDoneSlide();
			}
			for (;;)
			{
				this.controlMult = Calc.Approach(this.controlMult, 1f, 5f * Engine.DeltaTime);
				yield return null;
			}
			yield break;
		}

		// Token: 0x060011E4 RID: 4580 RVA: 0x00059BC0 File Offset: 0x00057DC0
		public override void Update(Scene scene)
		{
			Vector2 vector = Input.Aim.Value;
			vector += Input.MountainAim.Value;
			if (vector.Length() > 1f)
			{
				vector.Normalize();
			}
			vector *= 200f;
			this.controlScroll = Calc.Approach(this.controlScroll, vector, 600f * Engine.DeltaTime);
			foreach (CompleteRenderer.Layer layer in this.Layers)
			{
				layer.Update(scene);
			}
			this.routine.Update();
		}

		// Token: 0x060011E5 RID: 4581 RVA: 0x00059C78 File Offset: 0x00057E78
		public override void RenderContent(Scene scene)
		{
			HiresRenderer.BeginRender(BlendState.AlphaBlend, SamplerState.LinearClamp);
			foreach (CompleteRenderer.Layer layer in this.Layers)
			{
				layer.Render(-this.Scroll - this.controlScroll * this.controlMult);
			}
			if (this.RenderPostUI != null)
			{
				this.RenderPostUI();
			}
			if (this.fadeAlpha > 0f)
			{
				Draw.Rect(-10f, -10f, (float)(Engine.Width + 20), (float)(Engine.Height + 20), Color.Black * this.fadeAlpha);
			}
			HiresRenderer.EndRender();
		}

		// Token: 0x04000D81 RID: 3457
		private const float ScrollRange = 200f;

		// Token: 0x04000D82 RID: 3458
		private const float ScrollSpeed = 600f;

		// Token: 0x04000D83 RID: 3459
		private Atlas atlas;

		// Token: 0x04000D84 RID: 3460
		private XmlElement xml;

		// Token: 0x04000D85 RID: 3461
		private float fadeAlpha = 1f;

		// Token: 0x04000D86 RID: 3462
		private Coroutine routine;

		// Token: 0x04000D87 RID: 3463
		private Vector2 controlScroll;

		// Token: 0x04000D88 RID: 3464
		private float controlMult;

		// Token: 0x04000D89 RID: 3465
		public float SlideDuration = 1.5f;

		// Token: 0x04000D8A RID: 3466
		public List<CompleteRenderer.Layer> Layers = new List<CompleteRenderer.Layer>();

		// Token: 0x04000D8B RID: 3467
		public Vector2 Scroll;

		// Token: 0x04000D8C RID: 3468
		public Vector2 StartScroll;

		// Token: 0x04000D8D RID: 3469
		public Vector2 CenterScroll;

		// Token: 0x04000D8E RID: 3470
		public Vector2 Offset;

		// Token: 0x04000D8F RID: 3471
		public float Scale;

		// Token: 0x04000D90 RID: 3472
		public Action<Vector2> RenderUI;

		// Token: 0x04000D91 RID: 3473
		public Action RenderPostUI;

		// Token: 0x0200054F RID: 1359
		public abstract class Layer
		{
			// Token: 0x060025FC RID: 9724 RVA: 0x000FB0E8 File Offset: 0x000F92E8
			public Layer(XmlElement xml)
			{
				this.Position = xml.Position(Vector2.Zero);
				if (xml.HasAttr("scroll"))
				{
					this.ScrollFactor.X = (this.ScrollFactor.Y = xml.AttrFloat("scroll"));
					return;
				}
				this.ScrollFactor.X = xml.AttrFloat("scrollX", 0f);
				this.ScrollFactor.Y = xml.AttrFloat("scrollY", 0f);
			}

			// Token: 0x060025FD RID: 9725 RVA: 0x000091E2 File Offset: 0x000073E2
			public virtual void Update(Scene scene)
			{
			}

			// Token: 0x060025FE RID: 9726
			public abstract void Render(Vector2 scroll);

			// Token: 0x060025FF RID: 9727 RVA: 0x000FB174 File Offset: 0x000F9374
			public Vector2 GetScrollPosition(Vector2 scroll)
			{
				Vector2 position = this.Position;
				if (this.ScrollFactor != Vector2.Zero)
				{
					position.X = MathHelper.Lerp(this.Position.X, this.Position.X + scroll.X, this.ScrollFactor.X);
					position.Y = MathHelper.Lerp(this.Position.Y, this.Position.Y + scroll.Y, this.ScrollFactor.Y);
				}
				return position;
			}

			// Token: 0x040025ED RID: 9709
			public Vector2 Position;

			// Token: 0x040025EE RID: 9710
			public Vector2 ScrollFactor;
		}

		// Token: 0x02000550 RID: 1360
		public class UILayer : CompleteRenderer.Layer
		{
			// Token: 0x06002600 RID: 9728 RVA: 0x000FB203 File Offset: 0x000F9403
			public UILayer(CompleteRenderer renderer, XmlElement xml) : base(xml)
			{
				this.renderer = renderer;
			}

			// Token: 0x06002601 RID: 9729 RVA: 0x000FB213 File Offset: 0x000F9413
			public override void Render(Vector2 scroll)
			{
				if (this.renderer.RenderUI != null)
				{
					this.renderer.RenderUI(scroll);
				}
			}

			// Token: 0x040025EF RID: 9711
			private CompleteRenderer renderer;
		}

		// Token: 0x02000551 RID: 1361
		public class ImageLayer : CompleteRenderer.Layer
		{
			// Token: 0x06002602 RID: 9730 RVA: 0x000FB234 File Offset: 0x000F9434
			public ImageLayer(Vector2 offset, Atlas atlas, XmlElement xml) : base(xml)
			{
				this.Position += offset;
				foreach (string id in xml.Attr("img").Split(new char[]
				{
					','
				}))
				{
					if (atlas.Has(id))
					{
						this.Images.Add(atlas[id]);
					}
					else
					{
						this.Images.Add(null);
					}
				}
				this.FrameRate = xml.AttrFloat("fps", 6f);
				this.Alpha = xml.AttrFloat("alpha", 1f);
				this.Speed = new Vector2(xml.AttrFloat("speedx", 0f), xml.AttrFloat("speedy", 0f));
				this.Scale = xml.AttrFloat("scale", 1f);
			}

			// Token: 0x06002603 RID: 9731 RVA: 0x000FB329 File Offset: 0x000F9529
			public override void Update(Scene scene)
			{
				this.Frame += Engine.DeltaTime * this.FrameRate;
				this.Offset += this.Speed * Engine.DeltaTime;
			}

			// Token: 0x06002604 RID: 9732 RVA: 0x000FB368 File Offset: 0x000F9568
			public override void Render(Vector2 scroll)
			{
				Vector2 vector = base.GetScrollPosition(scroll).Floor();
				MTexture mtexture = this.Images[(int)(this.Frame % (float)this.Images.Count)];
				if (mtexture != null)
				{
					bool flag = SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode;
					if (flag)
					{
						vector.X = 1920f - vector.X - mtexture.DrawOffset.X * this.Scale - (float)mtexture.Texture.Texture.Width * this.Scale;
						vector.Y += mtexture.DrawOffset.Y * this.Scale;
					}
					else
					{
						vector += mtexture.DrawOffset * this.Scale;
					}
					Rectangle clipRect = mtexture.ClipRect;
					bool flag2 = this.Offset.X != 0f || this.Offset.Y != 0f;
					if (flag2)
					{
						clipRect = new Rectangle((int)(-this.Offset.X / this.Scale) + 1, (int)(-this.Offset.Y / this.Scale) + 1, mtexture.ClipRect.Width - 2, mtexture.ClipRect.Height - 2);
						HiresRenderer.EndRender();
						HiresRenderer.BeginRender(BlendState.AlphaBlend, SamplerState.LinearWrap);
					}
					Draw.SpriteBatch.Draw(mtexture.Texture.Texture, vector, new Rectangle?(clipRect), Color.White * this.Alpha, 0f, Vector2.Zero, this.Scale, flag ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
					if (flag2)
					{
						HiresRenderer.EndRender();
						HiresRenderer.BeginRender(BlendState.AlphaBlend, SamplerState.LinearClamp);
					}
				}
			}

			// Token: 0x040025F0 RID: 9712
			public List<MTexture> Images = new List<MTexture>();

			// Token: 0x040025F1 RID: 9713
			public float Frame;

			// Token: 0x040025F2 RID: 9714
			public float FrameRate;

			// Token: 0x040025F3 RID: 9715
			public float Alpha;

			// Token: 0x040025F4 RID: 9716
			public Vector2 Offset;

			// Token: 0x040025F5 RID: 9717
			public Vector2 Speed;

			// Token: 0x040025F6 RID: 9718
			public float Scale;
		}
	}
}
