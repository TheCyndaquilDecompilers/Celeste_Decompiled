using System;

namespace Monocle
{
	// Token: 0x0200013A RID: 314
	public abstract class VirtualAsset
	{
		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000B63 RID: 2915 RVA: 0x000203EF File Offset: 0x0001E5EF
		// (set) Token: 0x06000B64 RID: 2916 RVA: 0x000203F7 File Offset: 0x0001E5F7
		public string Name { get; internal set; }

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000B65 RID: 2917 RVA: 0x00020400 File Offset: 0x0001E600
		// (set) Token: 0x06000B66 RID: 2918 RVA: 0x00020408 File Offset: 0x0001E608
		public int Width { get; internal set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000B67 RID: 2919 RVA: 0x00020411 File Offset: 0x0001E611
		// (set) Token: 0x06000B68 RID: 2920 RVA: 0x00020419 File Offset: 0x0001E619
		public int Height { get; internal set; }

		// Token: 0x06000B69 RID: 2921 RVA: 0x000091E2 File Offset: 0x000073E2
		internal virtual void Unload()
		{
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x000091E2 File Offset: 0x000073E2
		internal virtual void Reload()
		{
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void Dispose()
		{
		}
	}
}
