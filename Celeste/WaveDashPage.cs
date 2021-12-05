using System;
using System.Collections;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x020001DC RID: 476
	public abstract class WaveDashPage
	{
		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000FF1 RID: 4081 RVA: 0x00044075 File Offset: 0x00042275
		public int Width
		{
			get
			{
				return this.Presentation.ScreenWidth;
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000FF2 RID: 4082 RVA: 0x00044082 File Offset: 0x00042282
		public int Height
		{
			get
			{
				return this.Presentation.ScreenHeight;
			}
		}

		// Token: 0x06000FF3 RID: 4083
		public abstract IEnumerator Routine();

		// Token: 0x06000FF4 RID: 4084 RVA: 0x0004408F File Offset: 0x0004228F
		public virtual void Added(WaveDashPresentation presentation)
		{
			this.Presentation = presentation;
		}

		// Token: 0x06000FF5 RID: 4085 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void Update()
		{
		}

		// Token: 0x06000FF6 RID: 4086 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void Render()
		{
		}

		// Token: 0x06000FF7 RID: 4087 RVA: 0x00044098 File Offset: 0x00042298
		protected IEnumerator PressButton()
		{
			this.WaitingForInput = true;
			while (!Input.MenuConfirm.Pressed)
			{
				yield return null;
			}
			this.WaitingForInput = false;
			Audio.Play("event:/new_content/game/10_farewell/ppt_mouseclick");
			yield break;
		}

		// Token: 0x04000B48 RID: 2888
		public WaveDashPresentation Presentation;

		// Token: 0x04000B49 RID: 2889
		public Color ClearColor;

		// Token: 0x04000B4A RID: 2890
		public WaveDashPage.Transitions Transition;

		// Token: 0x04000B4B RID: 2891
		public bool AutoProgress;

		// Token: 0x04000B4C RID: 2892
		public bool WaitingForInput;

		// Token: 0x020004E3 RID: 1251
		public enum Transitions
		{
			// Token: 0x04002411 RID: 9233
			ScaleIn,
			// Token: 0x04002412 RID: 9234
			FadeIn,
			// Token: 0x04002413 RID: 9235
			Rotate3D,
			// Token: 0x04002414 RID: 9236
			Blocky,
			// Token: 0x04002415 RID: 9237
			Spiral
		}
	}
}
