using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002A0 RID: 672
	[Tracked(true)]
	public abstract class MenuButton : Entity
	{
		// Token: 0x060014CD RID: 5325 RVA: 0x000746BC File Offset: 0x000728BC
		public static MenuButton GetSelection(Scene scene)
		{
			foreach (Entity entity in scene.Tracker.GetEntities<MenuButton>())
			{
				MenuButton menuButton = (MenuButton)entity;
				if (menuButton.Selected)
				{
					return menuButton;
				}
			}
			return null;
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x00074724 File Offset: 0x00072924
		public static void ClearSelection(Scene scene)
		{
			MenuButton selection = MenuButton.GetSelection(scene);
			if (selection != null)
			{
				selection.Selected = false;
			}
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x00074742 File Offset: 0x00072942
		public MenuButton(Oui oui, Vector2 targetPosition, Vector2 tweenFrom, Action onConfirm) : base(tweenFrom)
		{
			this.TargetPosition = targetPosition;
			this.TweenFrom = tweenFrom;
			this.OnConfirm = onConfirm;
			this.oui = oui;
		}

		// Token: 0x060014D0 RID: 5328 RVA: 0x00074768 File Offset: 0x00072968
		public override void Update()
		{
			base.Update();
			if (!this.canAcceptInput)
			{
				this.canAcceptInput = true;
				return;
			}
			if (this.oui.Selected && this.oui.Focused && this.selected)
			{
				if (Input.MenuConfirm.Pressed)
				{
					this.Confirm();
					return;
				}
				if (Input.MenuLeft.Pressed && this.LeftButton != null)
				{
					Audio.Play("event:/ui/main/rollover_up");
					this.LeftButton.Selected = true;
					return;
				}
				if (Input.MenuRight.Pressed && this.RightButton != null)
				{
					Audio.Play("event:/ui/main/rollover_down");
					this.RightButton.Selected = true;
					return;
				}
				if (Input.MenuUp.Pressed && this.UpButton != null)
				{
					Audio.Play("event:/ui/main/rollover_up");
					this.UpButton.Selected = true;
					return;
				}
				if (Input.MenuDown.Pressed && this.DownButton != null)
				{
					Audio.Play("event:/ui/main/rollover_down");
					this.DownButton.Selected = true;
				}
			}
		}

		// Token: 0x060014D1 RID: 5329 RVA: 0x00074878 File Offset: 0x00072A78
		public void TweenIn(float time)
		{
			if (this.tween != null && this.tween.Entity == this)
			{
				this.tween.RemoveSelf();
			}
			Vector2 from = this.Position;
			base.Add(this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, time, true));
			this.tween.OnUpdate = delegate(Tween t)
			{
				this.Position = Vector2.Lerp(from, this.TargetPosition, t.Eased);
			};
		}

		// Token: 0x060014D2 RID: 5330 RVA: 0x000748F4 File Offset: 0x00072AF4
		public void TweenOut(float time)
		{
			if (this.tween != null && this.tween.Entity == this)
			{
				this.tween.RemoveSelf();
			}
			Vector2 from = this.Position;
			base.Add(this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, time, true));
			this.tween.OnUpdate = delegate(Tween t)
			{
				this.Position = Vector2.Lerp(from, this.TweenFrom, t.Eased);
			};
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x060014D3 RID: 5331 RVA: 0x0007496E File Offset: 0x00072B6E
		public Color SelectionColor
		{
			get
			{
				if (!this.selected)
				{
					return Color.White;
				}
				if (!Settings.Instance.DisableFlashes && !base.Scene.BetweenInterval(0.1f))
				{
					return TextMenu.HighlightColorB;
				}
				return TextMenu.HighlightColorA;
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060014D4 RID: 5332 RVA: 0x000749A7 File Offset: 0x00072BA7
		// (set) Token: 0x060014D5 RID: 5333 RVA: 0x000749B0 File Offset: 0x00072BB0
		public bool Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				if (base.Scene == null)
				{
					throw new Exception("Cannot set Selected while MenuButton is not in a Scene.");
				}
				if (!this.selected && value)
				{
					MenuButton selection = MenuButton.GetSelection(base.Scene);
					if (selection != null)
					{
						selection.Selected = false;
					}
					this.selected = true;
					this.canAcceptInput = false;
					this.OnSelect();
					return;
				}
				if (this.selected && !value)
				{
					this.selected = false;
					this.OnDeselect();
				}
			}
		}

		// Token: 0x060014D6 RID: 5334 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void OnSelect()
		{
		}

		// Token: 0x060014D7 RID: 5335 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void OnDeselect()
		{
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x00074A20 File Offset: 0x00072C20
		public virtual void Confirm()
		{
			this.OnConfirm();
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x00074A2D File Offset: 0x00072C2D
		public virtual void StartSelected()
		{
			this.selected = true;
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060014DA RID: 5338
		public abstract float ButtonHeight { get; }

		// Token: 0x040010A4 RID: 4260
		public Vector2 TargetPosition;

		// Token: 0x040010A5 RID: 4261
		public Vector2 TweenFrom;

		// Token: 0x040010A6 RID: 4262
		public MenuButton LeftButton;

		// Token: 0x040010A7 RID: 4263
		public MenuButton RightButton;

		// Token: 0x040010A8 RID: 4264
		public MenuButton UpButton;

		// Token: 0x040010A9 RID: 4265
		public MenuButton DownButton;

		// Token: 0x040010AA RID: 4266
		public Action OnConfirm;

		// Token: 0x040010AB RID: 4267
		private bool canAcceptInput;

		// Token: 0x040010AC RID: 4268
		private Oui oui;

		// Token: 0x040010AD RID: 4269
		private bool selected;

		// Token: 0x040010AE RID: 4270
		private Tween tween;
	}
}
