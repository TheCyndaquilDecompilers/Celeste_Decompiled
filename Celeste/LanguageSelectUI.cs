using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001FD RID: 509
	public class LanguageSelectUI : TextMenu
	{
		// Token: 0x060010B2 RID: 4274 RVA: 0x0004E264 File Offset: 0x0004C464
		public LanguageSelectUI()
		{
			base.Tag = (Tags.HUD | Tags.PauseUpdate);
			this.Alpha = 0f;
			using (List<Language>.Enumerator enumerator = Dialog.OrderedLanguages.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Language language = enumerator.Current;
					base.Add(new LanguageSelectUI.LanguageOption(language).Pressed(delegate
					{
						this.open = false;
						this.SetNextLanguage(language);
					}));
				}
			}
			this.OnESC = (this.OnPause = (this.OnCancel = delegate()
			{
				this.open = false;
				this.Focused = false;
			}));
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x0004E33C File Offset: 0x0004C53C
		private void SetNextLanguage(Language next)
		{
			if (Settings.Instance.Language != next.Id)
			{
				Language language = Dialog.Languages[Settings.Instance.Language];
				Language language2 = Dialog.Languages["english"];
				if (language.FontFace != language2.FontFace)
				{
					Fonts.Unload(language.FontFace);
				}
				Fonts.Load(next.FontFace);
				Settings.Instance.Language = next.Id;
				Settings.Instance.ApplyLanguage();
			}
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x0004E3CC File Offset: 0x0004C5CC
		public override void Update()
		{
			if (this.Alpha > 0f)
			{
				base.Update();
			}
			if (!this.open && this.Alpha <= 0f)
			{
				base.Close();
			}
			this.Alpha = Calc.Approach(this.Alpha, (float)(this.open ? 1 : 0), Engine.DeltaTime * 8f);
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x0004E430 File Offset: 0x0004C630
		public override void Render()
		{
			if (this.Alpha > 0f)
			{
				Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * Ease.CubeOut(this.Alpha));
				base.Render();
			}
		}

		// Token: 0x04000C30 RID: 3120
		private bool open = true;

		// Token: 0x020004FE RID: 1278
		private class LanguageOption : TextMenu.Item
		{
			// Token: 0x060024C1 RID: 9409 RVA: 0x000F531E File Offset: 0x000F351E
			public LanguageOption(Language language)
			{
				this.Selectable = true;
				this.Language = language;
			}

			// Token: 0x1700042C RID: 1068
			// (get) Token: 0x060024C2 RID: 9410 RVA: 0x000F5334 File Offset: 0x000F3534
			public bool Selected
			{
				get
				{
					return this.Container.Current == this;
				}
			}

			// Token: 0x060024C3 RID: 9411 RVA: 0x000F5344 File Offset: 0x000F3544
			public override void Added()
			{
				this.Container.InnerContent = TextMenu.InnerContentMode.OneColumn;
				if (Dialog.Language == this.Language)
				{
					this.Container.Current = this;
				}
			}

			// Token: 0x060024C4 RID: 9412 RVA: 0x000F536B File Offset: 0x000F356B
			public override float LeftWidth()
			{
				return (float)this.Language.Icon.Width;
			}

			// Token: 0x060024C5 RID: 9413 RVA: 0x000F537E File Offset: 0x000F357E
			public override float Height()
			{
				return (float)this.Language.Icon.Height;
			}

			// Token: 0x060024C6 RID: 9414 RVA: 0x000F5391 File Offset: 0x000F3591
			public override void Update()
			{
				this.selectedEase = Calc.Approach(this.selectedEase, this.Selected ? 1f : 0f, Engine.DeltaTime * 5f);
			}

			// Token: 0x060024C7 RID: 9415 RVA: 0x000F53C4 File Offset: 0x000F35C4
			public override void Render(Vector2 position, bool highlighted)
			{
				Color color = this.Disabled ? Color.DarkSlateGray : ((highlighted ? this.Container.HighlightColor : Color.White) * this.Container.Alpha);
				position += (1f - Ease.CubeOut(this.Container.Alpha)) * Vector2.UnitY * 32f;
				if (this.Selected)
				{
					GFX.Gui["dotarrow_outline"].DrawCentered(position, color);
				}
				position += Vector2.UnitX * Ease.CubeInOut(this.selectedEase) * 32f;
				this.Language.Icon.DrawJustified(position, new Vector2(0f, 0.5f), Color.White * this.Container.Alpha, 1f);
			}

			// Token: 0x04002495 RID: 9365
			public Language Language;

			// Token: 0x04002496 RID: 9366
			private float selectedEase;
		}
	}
}
