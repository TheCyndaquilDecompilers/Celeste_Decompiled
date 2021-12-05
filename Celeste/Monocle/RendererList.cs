using System;
using System.Collections.Generic;

namespace Monocle
{
	// Token: 0x02000115 RID: 277
	public class RendererList
	{
		// Token: 0x060008B6 RID: 2230 RVA: 0x0001410B File Offset: 0x0001230B
		internal RendererList(Scene scene)
		{
			this.scene = scene;
			this.Renderers = new List<Renderer>();
			this.adding = new List<Renderer>();
			this.removing = new List<Renderer>();
		}

		// Token: 0x060008B7 RID: 2231 RVA: 0x0001413C File Offset: 0x0001233C
		internal void UpdateLists()
		{
			if (this.adding.Count > 0)
			{
				foreach (Renderer item in this.adding)
				{
					this.Renderers.Add(item);
				}
			}
			this.adding.Clear();
			if (this.removing.Count > 0)
			{
				foreach (Renderer item2 in this.removing)
				{
					this.Renderers.Remove(item2);
				}
			}
			this.removing.Clear();
		}

		// Token: 0x060008B8 RID: 2232 RVA: 0x00014210 File Offset: 0x00012410
		internal void Update()
		{
			foreach (Renderer renderer in this.Renderers)
			{
				renderer.Update(this.scene);
			}
		}

		// Token: 0x060008B9 RID: 2233 RVA: 0x00014268 File Offset: 0x00012468
		internal void BeforeRender()
		{
			for (int i = 0; i < this.Renderers.Count; i++)
			{
				if (this.Renderers[i].Visible)
				{
					Draw.Renderer = this.Renderers[i];
					this.Renderers[i].BeforeRender(this.scene);
				}
			}
		}

		// Token: 0x060008BA RID: 2234 RVA: 0x000142C8 File Offset: 0x000124C8
		internal void Render()
		{
			for (int i = 0; i < this.Renderers.Count; i++)
			{
				if (this.Renderers[i].Visible)
				{
					Draw.Renderer = this.Renderers[i];
					this.Renderers[i].Render(this.scene);
				}
			}
		}

		// Token: 0x060008BB RID: 2235 RVA: 0x00014328 File Offset: 0x00012528
		internal void AfterRender()
		{
			for (int i = 0; i < this.Renderers.Count; i++)
			{
				if (this.Renderers[i].Visible)
				{
					Draw.Renderer = this.Renderers[i];
					this.Renderers[i].AfterRender(this.scene);
				}
			}
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x00014386 File Offset: 0x00012586
		public void MoveToFront(Renderer renderer)
		{
			this.Renderers.Remove(renderer);
			this.Renderers.Add(renderer);
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x000143A1 File Offset: 0x000125A1
		public void Add(Renderer renderer)
		{
			this.adding.Add(renderer);
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x000143AF File Offset: 0x000125AF
		public void Remove(Renderer renderer)
		{
			this.removing.Add(renderer);
		}

		// Token: 0x040005CD RID: 1485
		public List<Renderer> Renderers;

		// Token: 0x040005CE RID: 1486
		private List<Renderer> adding;

		// Token: 0x040005CF RID: 1487
		private List<Renderer> removing;

		// Token: 0x040005D0 RID: 1488
		private Scene scene;
	}
}
