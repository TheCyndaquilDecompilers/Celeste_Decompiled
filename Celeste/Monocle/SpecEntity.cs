using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x02000135 RID: 309
	public class SpecEntity<T> : Entity where T : Scene
	{
		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000B1E RID: 2846 RVA: 0x0001E9F3 File Offset: 0x0001CBF3
		// (set) Token: 0x06000B1F RID: 2847 RVA: 0x0001E9FB File Offset: 0x0001CBFB
		public T SpecScene { get; private set; }

		// Token: 0x06000B20 RID: 2848 RVA: 0x0001EA04 File Offset: 0x0001CC04
		public SpecEntity(Vector2 position) : base(position)
		{
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x0001EA0D File Offset: 0x0001CC0D
		public SpecEntity()
		{
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x0001EA15 File Offset: 0x0001CC15
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (base.Scene is T)
			{
				this.SpecScene = (base.Scene as T);
			}
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x0001EA44 File Offset: 0x0001CC44
		public override void Removed(Scene scene)
		{
			this.SpecScene = default(T);
			base.Removed(scene);
		}
	}
}
