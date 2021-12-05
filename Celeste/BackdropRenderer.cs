using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000368 RID: 872
	public class BackdropRenderer : Renderer
	{
		// Token: 0x06001B66 RID: 7014 RVA: 0x000B3448 File Offset: 0x000B1648
		public override void BeforeRender(Scene scene)
		{
			foreach (Backdrop backdrop in this.Backdrops)
			{
				backdrop.BeforeRender(scene);
			}
		}

		// Token: 0x06001B67 RID: 7015 RVA: 0x000B349C File Offset: 0x000B169C
		public override void Update(Scene scene)
		{
			foreach (Backdrop backdrop in this.Backdrops)
			{
				backdrop.Update(scene);
			}
		}

		// Token: 0x06001B68 RID: 7016 RVA: 0x000B34F0 File Offset: 0x000B16F0
		public void Ended(Scene scene)
		{
			foreach (Backdrop backdrop in this.Backdrops)
			{
				backdrop.Ended(scene);
			}
		}

		// Token: 0x06001B69 RID: 7017 RVA: 0x000B3544 File Offset: 0x000B1744
		public T Get<T>() where T : class
		{
			foreach (Backdrop backdrop in this.Backdrops)
			{
				if (backdrop is T)
				{
					return backdrop as T;
				}
			}
			return default(T);
		}

		// Token: 0x06001B6A RID: 7018 RVA: 0x000B35B4 File Offset: 0x000B17B4
		public IEnumerable<T> GetEach<T>() where T : class
		{
			foreach (Backdrop backdrop in this.Backdrops)
			{
				if (backdrop is T)
				{
					yield return backdrop as T;
				}
			}
			List<Backdrop>.Enumerator enumerator = default(List<Backdrop>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06001B6B RID: 7019 RVA: 0x000B35C4 File Offset: 0x000B17C4
		public IEnumerable<T> GetEach<T>(string tag) where T : class
		{
			foreach (Backdrop backdrop in this.Backdrops)
			{
				if (backdrop is T && backdrop.Tags.Contains(tag))
				{
					yield return backdrop as T;
				}
			}
			List<Backdrop>.Enumerator enumerator = default(List<Backdrop>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06001B6C RID: 7020 RVA: 0x000B35DB File Offset: 0x000B17DB
		public void StartSpritebatch(BlendState blendState)
		{
			if (!this.usingSpritebatch)
			{
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, blendState, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, this.Matrix);
			}
			this.usingSpritebatch = true;
		}

		// Token: 0x06001B6D RID: 7021 RVA: 0x000B360E File Offset: 0x000B180E
		public void EndSpritebatch()
		{
			if (this.usingSpritebatch)
			{
				Draw.SpriteBatch.End();
			}
			this.usingSpritebatch = false;
		}

		// Token: 0x06001B6E RID: 7022 RVA: 0x000B362C File Offset: 0x000B182C
		public override void Render(Scene scene)
		{
			BlendState blendState = BlendState.AlphaBlend;
			foreach (Backdrop backdrop in this.Backdrops)
			{
				if (backdrop.Visible)
				{
					if (backdrop is Parallax && (backdrop as Parallax).BlendState != blendState)
					{
						this.EndSpritebatch();
						blendState = (backdrop as Parallax).BlendState;
					}
					if (backdrop.UseSpritebatch && !this.usingSpritebatch)
					{
						this.StartSpritebatch(blendState);
					}
					if (!backdrop.UseSpritebatch && this.usingSpritebatch)
					{
						this.EndSpritebatch();
					}
					backdrop.Render(scene);
				}
			}
			if (this.Fade > 0f)
			{
				Draw.Rect(-10f, -10f, 340f, 200f, this.FadeColor * this.Fade);
			}
			this.EndSpritebatch();
		}

		// Token: 0x06001B6F RID: 7023 RVA: 0x000B3720 File Offset: 0x000B1920
		public void Remove<T>() where T : Backdrop
		{
			for (int i = this.Backdrops.Count - 1; i >= 0; i--)
			{
				if (this.Backdrops[i].GetType() == typeof(T))
				{
					this.Backdrops.RemoveAt(i);
				}
			}
		}

		// Token: 0x04001876 RID: 6262
		public Matrix Matrix = Matrix.Identity;

		// Token: 0x04001877 RID: 6263
		public List<Backdrop> Backdrops = new List<Backdrop>();

		// Token: 0x04001878 RID: 6264
		public float Fade;

		// Token: 0x04001879 RID: 6265
		public Color FadeColor = Color.Black;

		// Token: 0x0400187A RID: 6266
		private bool usingSpritebatch;
	}
}
