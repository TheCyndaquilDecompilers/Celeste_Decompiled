using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002A1 RID: 673
	public class MainMenuClimb : MenuButton
	{
		// Token: 0x060014DB RID: 5339 RVA: 0x00074A38 File Offset: 0x00072C38
		public MainMenuClimb(Oui oui, Vector2 targetPosition, Vector2 tweenFrom, Action onConfirm) : base(oui, targetPosition, tweenFrom, onConfirm)
		{
			this.label = Dialog.Clean("menu_begin", null);
			this.icon = GFX.Gui["menu/start"];
			this.labelScale = 1f;
			float num = ActiveFont.Measure(this.label).X * 1.5f;
			if (num > 256f)
			{
				this.labelScale = 256f / num;
			}
			base.Add(this.bounceWiggler = Wiggler.Create(0.25f, 4f, null, false, false));
			base.Add(this.rotateWiggler = Wiggler.Create(0.3f, 6f, null, false, false));
			base.Add(this.bigBounceWiggler = Wiggler.Create(0.4f, 2f, null, false, false));
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x00074B10 File Offset: 0x00072D10
		public override void OnSelect()
		{
			this.confirmed = false;
			this.bounceWiggler.Start();
		}

		// Token: 0x060014DD RID: 5341 RVA: 0x00074B24 File Offset: 0x00072D24
		public override void Confirm()
		{
			base.Confirm();
			this.confirmed = true;
			this.bounceWiggler.Start();
			this.bigBounceWiggler.Start();
			this.rotateWiggler.Start();
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
		}

		// Token: 0x060014DE RID: 5342 RVA: 0x00074B5C File Offset: 0x00072D5C
		public override void Render()
		{
			Vector2 value = new Vector2(0f, this.bounceWiggler.Value * 8f);
			Vector2 vector = Vector2.UnitY * (float)this.icon.Height + new Vector2(0f, -Math.Abs(this.bigBounceWiggler.Value * 40f));
			if (!this.confirmed)
			{
				vector += value;
			}
			this.icon.DrawOutlineJustified(this.Position + vector, new Vector2(0.5f, 1f), Color.White, 1f, this.rotateWiggler.Value * 10f * 0.017453292f);
			ActiveFont.DrawOutline(this.label, this.Position + value + new Vector2(0f, (float)(48 + this.icon.Height)), new Vector2(0.5f, 0.5f), Vector2.One * 1.5f * this.labelScale, base.SelectionColor, 2f, Color.Black);
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060014DF RID: 5343 RVA: 0x00074C88 File Offset: 0x00072E88
		public override float ButtonHeight
		{
			get
			{
				return (float)this.icon.Height + ActiveFont.LineHeight + 48f;
			}
		}

		// Token: 0x040010AF RID: 4271
		private const float MaxLabelWidth = 256f;

		// Token: 0x040010B0 RID: 4272
		private const float BaseLabelScale = 1.5f;

		// Token: 0x040010B1 RID: 4273
		private string label;

		// Token: 0x040010B2 RID: 4274
		private MTexture icon;

		// Token: 0x040010B3 RID: 4275
		private float labelScale;

		// Token: 0x040010B4 RID: 4276
		private Wiggler bounceWiggler;

		// Token: 0x040010B5 RID: 4277
		private Wiggler rotateWiggler;

		// Token: 0x040010B6 RID: 4278
		private Wiggler bigBounceWiggler;

		// Token: 0x040010B7 RID: 4279
		private bool confirmed;
	}
}
