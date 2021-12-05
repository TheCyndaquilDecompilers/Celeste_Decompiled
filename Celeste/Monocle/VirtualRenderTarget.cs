using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x0200013C RID: 316
	public class VirtualRenderTarget : VirtualAsset
	{
		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000B76 RID: 2934 RVA: 0x00020AD1 File Offset: 0x0001ECD1
		// (set) Token: 0x06000B77 RID: 2935 RVA: 0x00020AD9 File Offset: 0x0001ECD9
		public bool Depth { get; private set; }

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000B78 RID: 2936 RVA: 0x00020AE2 File Offset: 0x0001ECE2
		// (set) Token: 0x06000B79 RID: 2937 RVA: 0x00020AEA File Offset: 0x0001ECEA
		public bool Preserve { get; private set; }

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000B7A RID: 2938 RVA: 0x00020AF3 File Offset: 0x0001ECF3
		public bool IsDisposed
		{
			get
			{
				return this.Target == null || this.Target.IsDisposed || this.Target.GraphicsDevice.IsDisposed;
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000B7B RID: 2939 RVA: 0x00020B1C File Offset: 0x0001ED1C
		public Rectangle Bounds
		{
			get
			{
				return this.Target.Bounds;
			}
		}

		// Token: 0x06000B7C RID: 2940 RVA: 0x00020B29 File Offset: 0x0001ED29
		internal VirtualRenderTarget(string name, int width, int height, int multiSampleCount, bool depth, bool preserve)
		{
			base.Name = name;
			base.Width = width;
			base.Height = height;
			this.MultiSampleCount = multiSampleCount;
			this.Depth = depth;
			this.Preserve = preserve;
			this.Reload();
		}

		// Token: 0x06000B7D RID: 2941 RVA: 0x00020B64 File Offset: 0x0001ED64
		internal override void Unload()
		{
			if (this.Target != null && !this.Target.IsDisposed)
			{
				this.Target.Dispose();
			}
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x00020B88 File Offset: 0x0001ED88
		internal override void Reload()
		{
			this.Unload();
			this.Target = new RenderTarget2D(Engine.Instance.GraphicsDevice, base.Width, base.Height, false, SurfaceFormat.Color, this.Depth ? DepthFormat.Depth24Stencil8 : DepthFormat.None, this.MultiSampleCount, this.Preserve ? RenderTargetUsage.PreserveContents : RenderTargetUsage.DiscardContents);
		}

		// Token: 0x06000B7F RID: 2943 RVA: 0x00020BDC File Offset: 0x0001EDDC
		public override void Dispose()
		{
			this.Unload();
			this.Target = null;
			VirtualContent.Remove(this);
		}

		// Token: 0x06000B80 RID: 2944 RVA: 0x00020BF1 File Offset: 0x0001EDF1
		public static implicit operator RenderTarget2D(VirtualRenderTarget target)
		{
			return target.Target;
		}

		// Token: 0x040006D0 RID: 1744
		public RenderTarget2D Target;

		// Token: 0x040006D3 RID: 1747
		public int MultiSampleCount;
	}
}
