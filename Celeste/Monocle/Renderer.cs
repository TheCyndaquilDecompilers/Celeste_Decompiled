using System;

namespace Monocle
{
	// Token: 0x0200011B RID: 283
	public abstract class Renderer
	{
		// Token: 0x060008E6 RID: 2278 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void Update(Scene scene)
		{
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void BeforeRender(Scene scene)
		{
		}

		// Token: 0x060008E8 RID: 2280 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void Render(Scene scene)
		{
		}

		// Token: 0x060008E9 RID: 2281 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void AfterRender(Scene scene)
		{
		}

		// Token: 0x04000601 RID: 1537
		public bool Visible = true;
	}
}
