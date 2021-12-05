using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002FF RID: 767
	public class OuiOptions : Oui
	{
		// Token: 0x060017F5 RID: 6133 RVA: 0x000952C8 File Offset: 0x000934C8
		public override void Added(Scene scene)
		{
			base.Added(scene);
		}

		// Token: 0x060017F6 RID: 6134 RVA: 0x000952D4 File Offset: 0x000934D4
		private void ReloadMenu()
		{
			Vector2 position = Vector2.Zero;
			int num = -1;
			if (this.menu != null)
			{
				position = this.menu.Position;
				num = this.menu.Selection;
				base.Scene.Remove(this.menu);
			}
			this.menu = MenuOptions.Create(false, null);
			if (num >= 0)
			{
				this.menu.Selection = num;
				this.menu.Position = position;
			}
			base.Scene.Add(this.menu);
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x00095354 File Offset: 0x00093554
		public override IEnumerator Enter(Oui from)
		{
			this.ReloadMenu();
			this.menu.Visible = (this.Visible = true);
			this.menu.Focused = false;
			this.currentLanguage = (this.startLanguage = Settings.Instance.Language);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
			{
				this.menu.X = 2880f + -1920f * Ease.CubeOut(p);
				this.alpha = Ease.CubeOut(p);
				yield return null;
			}
			this.menu.Focused = true;
			yield break;
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x00095363 File Offset: 0x00093563
		public override IEnumerator Leave(Oui next)
		{
			Audio.Play("event:/ui/main/whoosh_large_out");
			this.menu.Focused = false;
			UserIO.SaveHandler(false, true);
			while (UserIO.Saving)
			{
				yield return null;
			}
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
			{
				this.menu.X = 960f + 1920f * Ease.CubeIn(p);
				this.alpha = 1f - Ease.CubeIn(p);
				yield return null;
			}
			if (this.startLanguage != Settings.Instance.Language)
			{
				base.Overworld.ReloadMenus(Overworld.StartMode.ReturnFromOptions);
				yield return null;
			}
			this.menu.Visible = (this.Visible = false);
			this.menu.RemoveSelf();
			this.menu = null;
			yield break;
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x00095374 File Offset: 0x00093574
		public override void Update()
		{
			if (this.menu != null && this.menu.Focused && base.Selected && Input.MenuCancel.Pressed)
			{
				Audio.Play("event:/ui/main/button_back");
				base.Overworld.Goto<OuiMainMenu>();
			}
			if (base.Selected && this.currentLanguage != Settings.Instance.Language)
			{
				this.currentLanguage = Settings.Instance.Language;
				this.ReloadMenu();
			}
			base.Update();
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x000953FC File Offset: 0x000935FC
		public override void Render()
		{
			if (this.alpha > 0f)
			{
				Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.alpha * 0.4f);
			}
			base.Render();
		}

		// Token: 0x040014B4 RID: 5300
		private TextMenu menu;

		// Token: 0x040014B5 RID: 5301
		private const float onScreenX = 960f;

		// Token: 0x040014B6 RID: 5302
		private const float offScreenX = 2880f;

		// Token: 0x040014B7 RID: 5303
		private string startLanguage;

		// Token: 0x040014B8 RID: 5304
		private string currentLanguage;

		// Token: 0x040014B9 RID: 5305
		private float alpha;
	}
}
