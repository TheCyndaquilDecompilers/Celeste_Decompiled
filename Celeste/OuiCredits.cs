using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002A2 RID: 674
	public class OuiCredits : Oui
	{
		// Token: 0x060014E0 RID: 5344 RVA: 0x00074CA2 File Offset: 0x00072EA2
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.Position = this.offScreen;
			this.Visible = false;
		}

		// Token: 0x060014E1 RID: 5345 RVA: 0x00074CBE File Offset: 0x00072EBE
		public override IEnumerator Enter(Oui from)
		{
			Audio.SetMusic("event:/music/menu/credits", true, true);
			base.Overworld.ShowConfirmUI = false;
			Credits.BorderColor = Color.Black;
			this.credits = new Credits(0.5f, 1f, true, false);
			this.credits.Enabled = false;
			this.Visible = true;
			this.vignetteAlpha = 0f;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
			{
				this.Position = this.offScreen + (this.onScreen - this.offScreen) * Ease.CubeOut(p);
				yield return null;
			}
			yield break;
		}

		// Token: 0x060014E2 RID: 5346 RVA: 0x00074CCD File Offset: 0x00072ECD
		public override IEnumerator Leave(Oui next)
		{
			Audio.Play("event:/ui/main/whoosh_large_out");
			base.Overworld.SetNormalMusic();
			base.Overworld.ShowConfirmUI = true;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
			{
				this.Position = this.onScreen + (this.offScreen - this.onScreen) * Ease.CubeIn(p);
				yield return null;
			}
			this.Visible = false;
			yield break;
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x00074CDC File Offset: 0x00072EDC
		public override void Update()
		{
			if (this.Focused && (Input.MenuCancel.Pressed || this.credits.BottomTimer > 3f))
			{
				base.Overworld.Goto<OuiMainMenu>();
			}
			if (this.credits != null)
			{
				this.credits.Update();
				this.credits.Enabled = (this.Focused && base.Selected);
			}
			this.vignetteAlpha = Calc.Approach(this.vignetteAlpha, (float)(base.Selected ? 1 : 0), Engine.DeltaTime * (base.Selected ? 1f : 4f));
			base.Update();
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x00074D88 File Offset: 0x00072F88
		public override void Render()
		{
			if (this.vignetteAlpha > 0f)
			{
				Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.vignetteAlpha * 0.4f);
				OVR.Atlas["vignette"].Draw(Vector2.Zero, Vector2.Zero, Color.White * Ease.CubeInOut(this.vignetteAlpha), 1f);
			}
			if (this.credits != null)
			{
				this.credits.Render(this.Position);
			}
		}

		// Token: 0x040010B8 RID: 4280
		private readonly Vector2 onScreen = new Vector2(960f, 0f);

		// Token: 0x040010B9 RID: 4281
		private readonly Vector2 offScreen = new Vector2(3840f, 0f);

		// Token: 0x040010BA RID: 4282
		private Credits credits;

		// Token: 0x040010BB RID: 4283
		private float vignetteAlpha;
	}
}
