using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200018F RID: 399
	[Tracked(false)]
	public class MirrorSurface : Component
	{
		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000DDF RID: 3551 RVA: 0x000319B7 File Offset: 0x0002FBB7
		// (set) Token: 0x06000DE0 RID: 3552 RVA: 0x000319C0 File Offset: 0x0002FBC0
		public Vector2 ReflectionOffset
		{
			get
			{
				return this.reflectionOffset;
			}
			set
			{
				this.reflectionOffset = value;
				this.ReflectionColor = new Color(0.5f + Calc.Clamp(this.reflectionOffset.X / 32f, -1f, 1f) * 0.5f, 0.5f + Calc.Clamp(this.reflectionOffset.Y / 32f, -1f, 1f) * 0.5f, 0f, 1f);
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000DE1 RID: 3553 RVA: 0x00031A41 File Offset: 0x0002FC41
		// (set) Token: 0x06000DE2 RID: 3554 RVA: 0x00031A49 File Offset: 0x0002FC49
		public Color ReflectionColor { get; private set; }

		// Token: 0x06000DE3 RID: 3555 RVA: 0x00031A52 File Offset: 0x0002FC52
		public MirrorSurface(Action onRender = null) : base(false, true)
		{
			this.OnRender = onRender;
		}

		// Token: 0x0400092D RID: 2349
		public Action OnRender;

		// Token: 0x0400092E RID: 2350
		private Vector2 reflectionOffset;
	}
}
