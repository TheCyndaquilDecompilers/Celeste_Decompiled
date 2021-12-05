using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200029F RID: 671
	public class MainMenuSmallButton : MenuButton
	{
		// Token: 0x060014C8 RID: 5320 RVA: 0x00074500 File Offset: 0x00072700
		public MainMenuSmallButton(string labelName, string iconName, Oui oui, Vector2 targetPosition, Vector2 tweenFrom, Action onConfirm) : base(oui, targetPosition, tweenFrom, onConfirm)
		{
			this.label = Dialog.Clean(labelName, null);
			this.icon = GFX.Gui[iconName];
			this.labelScale = 1f;
			float x = ActiveFont.Measure(this.label).X;
			if (x > 400f)
			{
				this.labelScale = 400f / x;
			}
			base.Add(this.wiggler = Wiggler.Create(0.25f, 4f, null, false, false));
		}

		// Token: 0x060014C9 RID: 5321 RVA: 0x0007458A File Offset: 0x0007278A
		public override void Update()
		{
			base.Update();
			this.ease = Calc.Approach(this.ease, (float)(base.Selected ? 1 : 0), 6f * Engine.DeltaTime);
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x000745BC File Offset: 0x000727BC
		public override void Render()
		{
			base.Render();
			float scale = 64f / (float)this.icon.Width;
			Vector2 value = new Vector2(Ease.CubeInOut(this.ease) * 32f, ActiveFont.LineHeight / 2f + this.wiggler.Value * 8f);
			this.icon.DrawOutlineJustified(this.Position + value, new Vector2(0f, 0.5f), Color.White, scale);
			ActiveFont.DrawOutline(this.label, this.Position + value + new Vector2(84f, 0f), new Vector2(0f, 0.5f), Vector2.One * this.labelScale, base.SelectionColor, 2f, Color.Black);
		}

		// Token: 0x060014CB RID: 5323 RVA: 0x000746A2 File Offset: 0x000728A2
		public override void OnSelect()
		{
			this.wiggler.Start();
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x060014CC RID: 5324 RVA: 0x000746AF File Offset: 0x000728AF
		public override float ButtonHeight
		{
			get
			{
				return ActiveFont.LineHeight * 1.25f;
			}
		}

		// Token: 0x0400109C RID: 4252
		private const float IconWidth = 64f;

		// Token: 0x0400109D RID: 4253
		private const float IconSpacing = 20f;

		// Token: 0x0400109E RID: 4254
		private const float MaxLabelWidth = 400f;

		// Token: 0x0400109F RID: 4255
		private MTexture icon;

		// Token: 0x040010A0 RID: 4256
		private string label;

		// Token: 0x040010A1 RID: 4257
		private float labelScale;

		// Token: 0x040010A2 RID: 4258
		private Wiggler wiggler;

		// Token: 0x040010A3 RID: 4259
		private float ease;
	}
}
