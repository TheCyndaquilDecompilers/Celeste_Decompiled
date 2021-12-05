using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000303 RID: 771
	public class TestWipes : Scene
	{
		// Token: 0x06001815 RID: 6165 RVA: 0x0009619C File Offset: 0x0009439C
		public TestWipes()
		{
			this.coroutine = new Coroutine(this.routine(), true);
		}

		// Token: 0x06001816 RID: 6166 RVA: 0x000961C1 File Offset: 0x000943C1
		private IEnumerator routine()
		{
			float dur = 1f;
			yield return 1f;
			for (;;)
			{
				ScreenWipe.WipeColor = Color.Black;
				new CurtainWipe(this, false, null).Duration = dur;
				yield return dur;
				this.lastColor = ScreenWipe.WipeColor;
				ScreenWipe.WipeColor = Calc.HexToColor("ff0034");
				new AngledWipe(this, false, null).Duration = dur;
				yield return dur;
				this.lastColor = ScreenWipe.WipeColor;
				ScreenWipe.WipeColor = Calc.HexToColor("0b0960");
				new DreamWipe(this, false, null).Duration = dur;
				yield return dur;
				this.lastColor = ScreenWipe.WipeColor;
				ScreenWipe.WipeColor = Calc.HexToColor("39bf00");
				new KeyDoorWipe(this, false, null).Duration = dur;
				yield return dur;
				this.lastColor = ScreenWipe.WipeColor;
				ScreenWipe.WipeColor = Calc.HexToColor("4376b3");
				new WindWipe(this, false, null).Duration = dur;
				yield return dur;
				this.lastColor = ScreenWipe.WipeColor;
				ScreenWipe.WipeColor = Calc.HexToColor("ffae00");
				new DropWipe(this, false, null).Duration = dur;
				yield return dur;
				this.lastColor = ScreenWipe.WipeColor;
				ScreenWipe.WipeColor = Calc.HexToColor("cc54ff");
				new FallWipe(this, false, null).Duration = dur;
				yield return dur;
				this.lastColor = ScreenWipe.WipeColor;
				ScreenWipe.WipeColor = Calc.HexToColor("ff007a");
				new MountainWipe(this, false, null).Duration = dur;
				yield return dur;
				this.lastColor = ScreenWipe.WipeColor;
				ScreenWipe.WipeColor = Color.White;
				new HeartWipe(this, false, null).Duration = dur;
				yield return dur;
				this.lastColor = ScreenWipe.WipeColor;
			}
			yield break;
		}

		// Token: 0x06001817 RID: 6167 RVA: 0x000961D0 File Offset: 0x000943D0
		public override void Update()
		{
			base.Update();
			this.coroutine.Update();
		}

		// Token: 0x06001818 RID: 6168 RVA: 0x000961E3 File Offset: 0x000943E3
		public override void Render()
		{
			Draw.SpriteBatch.Begin();
			Draw.Rect(-1f, -1f, 1920f, 1080f, this.lastColor);
			Draw.SpriteBatch.End();
			base.Render();
		}

		// Token: 0x040014D7 RID: 5335
		private Coroutine coroutine;

		// Token: 0x040014D8 RID: 5336
		private Color lastColor = Color.White;
	}
}
