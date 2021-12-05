using System;
using System.Collections;
using Monocle;

namespace Celeste
{
	// Token: 0x02000301 RID: 769
	public abstract class Oui : Entity
	{
		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06001804 RID: 6148 RVA: 0x00095939 File Offset: 0x00093B39
		public Overworld Overworld
		{
			get
			{
				return base.SceneAs<Overworld>();
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06001805 RID: 6149 RVA: 0x00095941 File Offset: 0x00093B41
		public bool Selected
		{
			get
			{
				return this.Overworld != null && this.Overworld.Current == this;
			}
		}

		// Token: 0x06001806 RID: 6150 RVA: 0x0009595B File Offset: 0x00093B5B
		public Oui()
		{
			base.AddTag(Tags.HUD);
		}

		// Token: 0x06001807 RID: 6151 RVA: 0x00008E00 File Offset: 0x00007000
		public virtual bool IsStart(Overworld overworld, Overworld.StartMode start)
		{
			return false;
		}

		// Token: 0x06001808 RID: 6152
		public abstract IEnumerator Enter(Oui from);

		// Token: 0x06001809 RID: 6153
		public abstract IEnumerator Leave(Oui next);

		// Token: 0x040014C6 RID: 5318
		public bool Focused;
	}
}
