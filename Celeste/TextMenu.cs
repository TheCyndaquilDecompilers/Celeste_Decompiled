using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x02000237 RID: 567
	public class TextMenu : Entity
	{
		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06001205 RID: 4613 RVA: 0x0005B027 File Offset: 0x00059227
		// (set) Token: 0x06001206 RID: 4614 RVA: 0x0005B053 File Offset: 0x00059253
		public TextMenu.Item Current
		{
			get
			{
				if (this.items.Count <= 0 || this.Selection < 0)
				{
					return null;
				}
				return this.items[this.Selection];
			}
			set
			{
				this.Selection = this.items.IndexOf(value);
			}
		}

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06001207 RID: 4615 RVA: 0x0005B067 File Offset: 0x00059267
		// (set) Token: 0x06001208 RID: 4616 RVA: 0x0005B06F File Offset: 0x0005926F
		public new float Width { get; private set; }

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06001209 RID: 4617 RVA: 0x0005B078 File Offset: 0x00059278
		// (set) Token: 0x0600120A RID: 4618 RVA: 0x0005B080 File Offset: 0x00059280
		public new float Height { get; private set; }

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x0600120B RID: 4619 RVA: 0x0005B089 File Offset: 0x00059289
		// (set) Token: 0x0600120C RID: 4620 RVA: 0x0005B091 File Offset: 0x00059291
		public float LeftColumnWidth { get; private set; }

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x0600120D RID: 4621 RVA: 0x0005B09A File Offset: 0x0005929A
		// (set) Token: 0x0600120E RID: 4622 RVA: 0x0005B0A2 File Offset: 0x000592A2
		public float RightColumnWidth { get; private set; }

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x0600120F RID: 4623 RVA: 0x0005B0AB File Offset: 0x000592AB
		public float ScrollableMinSize
		{
			get
			{
				return (float)(Engine.Height - 300);
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06001210 RID: 4624 RVA: 0x0005B0BC File Offset: 0x000592BC
		public int FirstPossibleSelection
		{
			get
			{
				for (int i = 0; i < this.items.Count; i++)
				{
					if (this.items[i] != null && this.items[i].Hoverable)
					{
						return i;
					}
				}
				return 0;
			}
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06001211 RID: 4625 RVA: 0x0005B104 File Offset: 0x00059304
		public int LastPossibleSelection
		{
			get
			{
				for (int i = this.items.Count - 1; i >= 0; i--)
				{
					if (this.items[i] != null && this.items[i].Hoverable)
					{
						return i;
					}
				}
				return 0;
			}
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x0005B150 File Offset: 0x00059350
		public TextMenu()
		{
			base.Tag = (Tags.PauseUpdate | Tags.HUD);
			this.Position = new Vector2((float)Engine.Width, (float)Engine.Height) / 2f;
			this.Justify = new Vector2(0.5f, 0.5f);
		}

		// Token: 0x06001213 RID: 4627 RVA: 0x0005B1F8 File Offset: 0x000593F8
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (this.AutoScroll)
			{
				if (this.Height > this.ScrollableMinSize)
				{
					this.Position.Y = this.ScrollTargetY;
					return;
				}
				this.Position.Y = 540f;
			}
		}

		// Token: 0x06001214 RID: 4628 RVA: 0x0005B244 File Offset: 0x00059444
		public TextMenu Add(TextMenu.Item item)
		{
			this.items.Add(item);
			item.Container = this;
			base.Add(item.ValueWiggler = Wiggler.Create(0.25f, 3f, null, false, false));
			base.Add(item.SelectWiggler = Wiggler.Create(0.25f, 3f, null, false, false));
			item.ValueWiggler.UseRawDeltaTime = (item.SelectWiggler.UseRawDeltaTime = true);
			if (this.Selection == -1)
			{
				this.FirstSelection();
			}
			this.RecalculateSize();
			item.Added();
			return this;
		}

		// Token: 0x06001215 RID: 4629 RVA: 0x0005B2DC File Offset: 0x000594DC
		public void Clear()
		{
			this.items = new List<TextMenu.Item>();
		}

		// Token: 0x06001216 RID: 4630 RVA: 0x0005B2E9 File Offset: 0x000594E9
		public int IndexOf(TextMenu.Item item)
		{
			return this.items.IndexOf(item);
		}

		// Token: 0x06001217 RID: 4631 RVA: 0x0005B2F7 File Offset: 0x000594F7
		public void FirstSelection()
		{
			this.Selection = -1;
			this.MoveSelection(1, false);
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x0005B308 File Offset: 0x00059508
		public void MoveSelection(int direction, bool wiggle = false)
		{
			int selection = this.Selection;
			direction = Math.Sign(direction);
			int num = 0;
			using (List<TextMenu.Item>.Enumerator enumerator = this.items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Hoverable)
					{
						num++;
					}
				}
			}
			bool flag = num > 2;
			for (;;)
			{
				this.Selection += direction;
				if (flag)
				{
					if (this.Selection < 0)
					{
						this.Selection = this.items.Count - 1;
					}
					else if (this.Selection >= this.items.Count)
					{
						this.Selection = 0;
					}
				}
				else if (this.Selection < 0 || this.Selection > this.items.Count - 1)
				{
					break;
				}
				if (this.Current.Hoverable)
				{
					goto IL_E9;
				}
			}
			this.Selection = Calc.Clamp(this.Selection, 0, this.items.Count - 1);
			IL_E9:
			if (!this.Current.Hoverable)
			{
				this.Selection = selection;
			}
			if (this.Selection != selection && this.Current != null)
			{
				if (selection >= 0 && this.items[selection] != null && this.items[selection].OnLeave != null)
				{
					this.items[selection].OnLeave();
				}
				if (this.Current.OnEnter != null)
				{
					this.Current.OnEnter();
				}
				if (wiggle)
				{
					Audio.Play((direction > 0) ? "event:/ui/main/rollover_down" : "event:/ui/main/rollover_up");
					this.Current.SelectWiggler.Start();
				}
			}
		}

		// Token: 0x06001219 RID: 4633 RVA: 0x0005B4BC File Offset: 0x000596BC
		public void RecalculateSize()
		{
			this.LeftColumnWidth = (this.RightColumnWidth = (this.Height = 0f));
			foreach (TextMenu.Item item in this.items)
			{
				if (item.IncludeWidthInMeasurement)
				{
					this.LeftColumnWidth = Math.Max(this.LeftColumnWidth, item.LeftWidth());
				}
			}
			foreach (TextMenu.Item item2 in this.items)
			{
				if (item2.IncludeWidthInMeasurement)
				{
					this.RightColumnWidth = Math.Max(this.RightColumnWidth, item2.RightWidth());
				}
			}
			foreach (TextMenu.Item item3 in this.items)
			{
				if (item3.Visible)
				{
					this.Height += item3.Height() + this.ItemSpacing;
				}
			}
			this.Height -= this.ItemSpacing;
			this.Width = Math.Max(this.MinWidth, this.LeftColumnWidth + this.RightColumnWidth);
		}

		// Token: 0x0600121A RID: 4634 RVA: 0x0005B634 File Offset: 0x00059834
		public float GetYOffsetOf(TextMenu.Item item)
		{
			if (item == null)
			{
				return 0f;
			}
			float num = 0f;
			foreach (TextMenu.Item item2 in this.items)
			{
				if (item.Visible)
				{
					num += item2.Height() + this.ItemSpacing;
				}
				if (item2 == item)
				{
					break;
				}
			}
			return num - item.Height() * 0.5f - this.ItemSpacing;
		}

		// Token: 0x0600121B RID: 4635 RVA: 0x0005B6C4 File Offset: 0x000598C4
		public void Close()
		{
			if (this.Current != null && this.Current.OnLeave != null)
			{
				this.Current.OnLeave();
			}
			if (this.OnClose != null)
			{
				this.OnClose();
			}
			base.RemoveSelf();
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x0005B704 File Offset: 0x00059904
		public void CloseAndRun(IEnumerator routine, Action onClose)
		{
			this.Focused = false;
			this.Visible = false;
			base.Add(new Coroutine(this.CloseAndRunRoutine(routine, onClose), true));
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x0005B728 File Offset: 0x00059928
		private IEnumerator CloseAndRunRoutine(IEnumerator routine, Action onClose)
		{
			yield return routine;
			if (onClose != null)
			{
				onClose();
			}
			this.Close();
			yield break;
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x0005B748 File Offset: 0x00059948
		public override void Update()
		{
			base.Update();
			if (this.OnUpdate != null)
			{
				this.OnUpdate();
			}
			if (this.Focused)
			{
				if (Input.MenuDown.Pressed)
				{
					if (!Input.MenuDown.Repeating || this.Selection != this.LastPossibleSelection)
					{
						this.MoveSelection(1, true);
					}
				}
				else if (Input.MenuUp.Pressed && (!Input.MenuUp.Repeating || this.Selection != this.FirstPossibleSelection))
				{
					this.MoveSelection(-1, true);
				}
				if (this.Current != null)
				{
					if (Input.MenuLeft.Pressed)
					{
						this.Current.LeftPressed();
					}
					if (Input.MenuRight.Pressed)
					{
						this.Current.RightPressed();
					}
					if (Input.MenuConfirm.Pressed)
					{
						this.Current.ConfirmPressed();
						if (this.Current.OnPressed != null)
						{
							this.Current.OnPressed();
						}
					}
					if (Input.MenuJournal.Pressed && this.Current.OnAltPressed != null)
					{
						this.Current.OnAltPressed();
					}
				}
				if (!Input.MenuConfirm.Pressed)
				{
					if (Input.MenuCancel.Pressed && this.OnCancel != null)
					{
						this.OnCancel();
					}
					else if (Input.ESC.Pressed && this.OnESC != null)
					{
						Input.ESC.ConsumeBuffer();
						this.OnESC();
					}
					else if (Input.Pause.Pressed && this.OnPause != null)
					{
						Input.Pause.ConsumeBuffer();
						this.OnPause();
					}
				}
			}
			foreach (TextMenu.Item item in this.items)
			{
				if (item.OnUpdate != null)
				{
					item.OnUpdate();
				}
				item.Update();
			}
			if (Settings.Instance.DisableFlashes)
			{
				this.HighlightColor = TextMenu.HighlightColorA;
			}
			else if (Engine.Scene.OnRawInterval(0.1f))
			{
				if (this.HighlightColor == TextMenu.HighlightColorA)
				{
					this.HighlightColor = TextMenu.HighlightColorB;
				}
				else
				{
					this.HighlightColor = TextMenu.HighlightColorA;
				}
			}
			if (this.AutoScroll)
			{
				if (this.Height > this.ScrollableMinSize)
				{
					this.Position.Y = this.Position.Y + (this.ScrollTargetY - this.Position.Y) * (1f - (float)Math.Pow(0.009999999776482582, (double)Engine.RawDeltaTime));
					return;
				}
				this.Position.Y = 540f;
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x0600121F RID: 4639 RVA: 0x0005BA00 File Offset: 0x00059C00
		public float ScrollTargetY
		{
			get
			{
				float min = (float)(Engine.Height - 150) - this.Height * this.Justify.Y;
				float max = 150f + this.Height * this.Justify.Y;
				return Calc.Clamp((float)(Engine.Height / 2) + this.Height * this.Justify.Y - this.GetYOffsetOf(this.Current), min, max);
			}
		}

		// Token: 0x06001220 RID: 4640 RVA: 0x0005BA78 File Offset: 0x00059C78
		public override void Render()
		{
			this.RecalculateSize();
			Vector2 vector = this.Position - this.Justify * new Vector2(this.Width, this.Height);
			Vector2 value = vector;
			bool flag = false;
			foreach (TextMenu.Item item in this.items)
			{
				if (item.Visible)
				{
					float num = item.Height();
					if (!item.AboveAll)
					{
						item.Render(value + new Vector2(0f, num * 0.5f + item.SelectWiggler.Value * 8f), this.Focused && this.Current == item);
					}
					else
					{
						flag = true;
					}
					value.Y += num + this.ItemSpacing;
				}
			}
			if (flag)
			{
				value = vector;
				foreach (TextMenu.Item item2 in this.items)
				{
					if (item2.Visible)
					{
						float num2 = item2.Height();
						if (item2.AboveAll)
						{
							item2.Render(value + new Vector2(0f, num2 * 0.5f + item2.SelectWiggler.Value * 8f), this.Focused && this.Current == item2);
						}
						value.Y += num2 + this.ItemSpacing;
					}
				}
			}
		}

		// Token: 0x04000DB6 RID: 3510
		public bool Focused = true;

		// Token: 0x04000DB7 RID: 3511
		public TextMenu.InnerContentMode InnerContent;

		// Token: 0x04000DB8 RID: 3512
		private List<TextMenu.Item> items = new List<TextMenu.Item>();

		// Token: 0x04000DB9 RID: 3513
		public int Selection = -1;

		// Token: 0x04000DBA RID: 3514
		public Vector2 Justify;

		// Token: 0x04000DBB RID: 3515
		public float ItemSpacing = 4f;

		// Token: 0x04000DC0 RID: 3520
		public float MinWidth;

		// Token: 0x04000DC1 RID: 3521
		public float Alpha = 1f;

		// Token: 0x04000DC2 RID: 3522
		public Color HighlightColor = Color.White;

		// Token: 0x04000DC3 RID: 3523
		public static readonly Color HighlightColorA = Calc.HexToColor("84FF54");

		// Token: 0x04000DC4 RID: 3524
		public static readonly Color HighlightColorB = Calc.HexToColor("FCFF59");

		// Token: 0x04000DC5 RID: 3525
		public Action OnESC;

		// Token: 0x04000DC6 RID: 3526
		public Action OnCancel;

		// Token: 0x04000DC7 RID: 3527
		public Action OnUpdate;

		// Token: 0x04000DC8 RID: 3528
		public Action OnPause;

		// Token: 0x04000DC9 RID: 3529
		public Action OnClose;

		// Token: 0x04000DCA RID: 3530
		public bool AutoScroll = true;

		// Token: 0x0200055A RID: 1370
		public enum InnerContentMode
		{
			// Token: 0x04002621 RID: 9761
			OneColumn,
			// Token: 0x04002622 RID: 9762
			TwoColumn
		}

		// Token: 0x0200055B RID: 1371
		public abstract class Item
		{
			// Token: 0x17000486 RID: 1158
			// (get) Token: 0x0600262D RID: 9773 RVA: 0x000FBEC3 File Offset: 0x000FA0C3
			public bool Hoverable
			{
				get
				{
					return this.Selectable && this.Visible && !this.Disabled;
				}
			}

			// Token: 0x0600262E RID: 9774 RVA: 0x000FBEE0 File Offset: 0x000FA0E0
			public TextMenu.Item Enter(Action onEnter)
			{
				this.OnEnter = onEnter;
				return this;
			}

			// Token: 0x0600262F RID: 9775 RVA: 0x000FBEEA File Offset: 0x000FA0EA
			public TextMenu.Item Leave(Action onLeave)
			{
				this.OnLeave = onLeave;
				return this;
			}

			// Token: 0x06002630 RID: 9776 RVA: 0x000FBEF4 File Offset: 0x000FA0F4
			public TextMenu.Item Pressed(Action onPressed)
			{
				this.OnPressed = onPressed;
				return this;
			}

			// Token: 0x06002631 RID: 9777 RVA: 0x000FBEFE File Offset: 0x000FA0FE
			public TextMenu.Item AltPressed(Action onPressed)
			{
				this.OnAltPressed = onPressed;
				return this;
			}

			// Token: 0x17000487 RID: 1159
			// (get) Token: 0x06002632 RID: 9778 RVA: 0x000FBF08 File Offset: 0x000FA108
			public float Width
			{
				get
				{
					return this.LeftWidth() + this.RightWidth();
				}
			}

			// Token: 0x06002633 RID: 9779 RVA: 0x000091E2 File Offset: 0x000073E2
			public virtual void ConfirmPressed()
			{
			}

			// Token: 0x06002634 RID: 9780 RVA: 0x000091E2 File Offset: 0x000073E2
			public virtual void LeftPressed()
			{
			}

			// Token: 0x06002635 RID: 9781 RVA: 0x000091E2 File Offset: 0x000073E2
			public virtual void RightPressed()
			{
			}

			// Token: 0x06002636 RID: 9782 RVA: 0x000091E2 File Offset: 0x000073E2
			public virtual void Added()
			{
			}

			// Token: 0x06002637 RID: 9783 RVA: 0x000091E2 File Offset: 0x000073E2
			public virtual void Update()
			{
			}

			// Token: 0x06002638 RID: 9784 RVA: 0x000FBF17 File Offset: 0x000FA117
			public virtual float LeftWidth()
			{
				return 0f;
			}

			// Token: 0x06002639 RID: 9785 RVA: 0x000FBF17 File Offset: 0x000FA117
			public virtual float RightWidth()
			{
				return 0f;
			}

			// Token: 0x0600263A RID: 9786 RVA: 0x000FBF17 File Offset: 0x000FA117
			public virtual float Height()
			{
				return 0f;
			}

			// Token: 0x0600263B RID: 9787 RVA: 0x000091E2 File Offset: 0x000073E2
			public virtual void Render(Vector2 position, bool highlighted)
			{
			}

			// Token: 0x04002623 RID: 9763
			public bool Selectable;

			// Token: 0x04002624 RID: 9764
			public bool Visible = true;

			// Token: 0x04002625 RID: 9765
			public bool Disabled;

			// Token: 0x04002626 RID: 9766
			public bool IncludeWidthInMeasurement = true;

			// Token: 0x04002627 RID: 9767
			public bool AboveAll;

			// Token: 0x04002628 RID: 9768
			public TextMenu Container;

			// Token: 0x04002629 RID: 9769
			public Wiggler SelectWiggler;

			// Token: 0x0400262A RID: 9770
			public Wiggler ValueWiggler;

			// Token: 0x0400262B RID: 9771
			public Action OnEnter;

			// Token: 0x0400262C RID: 9772
			public Action OnLeave;

			// Token: 0x0400262D RID: 9773
			public Action OnPressed;

			// Token: 0x0400262E RID: 9774
			public Action OnAltPressed;

			// Token: 0x0400262F RID: 9775
			public Action OnUpdate;
		}

		// Token: 0x0200055C RID: 1372
		public class Header : TextMenu.Item
		{
			// Token: 0x0600263D RID: 9789 RVA: 0x000FBF34 File Offset: 0x000FA134
			public Header(string title)
			{
				this.Title = title;
				this.Selectable = false;
				this.IncludeWidthInMeasurement = false;
			}

			// Token: 0x0600263E RID: 9790 RVA: 0x000FBF51 File Offset: 0x000FA151
			public override float LeftWidth()
			{
				return ActiveFont.Measure(this.Title).X * 2f;
			}

			// Token: 0x0600263F RID: 9791 RVA: 0x0004D0E7 File Offset: 0x0004B2E7
			public override float Height()
			{
				return ActiveFont.LineHeight * 2f;
			}

			// Token: 0x06002640 RID: 9792 RVA: 0x000FBF6C File Offset: 0x000FA16C
			public override void Render(Vector2 position, bool highlighted)
			{
				float alpha = this.Container.Alpha;
				Color strokeColor = Color.Black * (alpha * alpha * alpha);
				ActiveFont.DrawEdgeOutline(this.Title, position + new Vector2(this.Container.Width * 0.5f, 0f), new Vector2(0.5f, 0.5f), Vector2.One * 2f, Color.Gray * alpha, 4f, Color.DarkSlateBlue * alpha, 2f, strokeColor);
			}

			// Token: 0x04002630 RID: 9776
			public const float Scale = 2f;

			// Token: 0x04002631 RID: 9777
			public string Title;
		}

		// Token: 0x0200055D RID: 1373
		public class SubHeader : TextMenu.Item
		{
			// Token: 0x06002641 RID: 9793 RVA: 0x000FC000 File Offset: 0x000FA200
			public SubHeader(string title, bool topPadding = true)
			{
				this.Title = title;
				this.Selectable = false;
				this.TopPadding = topPadding;
			}

			// Token: 0x06002642 RID: 9794 RVA: 0x000FC024 File Offset: 0x000FA224
			public override float LeftWidth()
			{
				return ActiveFont.Measure(this.Title).X * 0.6f;
			}

			// Token: 0x06002643 RID: 9795 RVA: 0x000FC03C File Offset: 0x000FA23C
			public override float Height()
			{
				return ((this.Title.Length > 0) ? (ActiveFont.LineHeight * 0.6f) : 0f) + (float)(this.TopPadding ? 48 : 0);
			}

			// Token: 0x06002644 RID: 9796 RVA: 0x000FC070 File Offset: 0x000FA270
			public override void Render(Vector2 position, bool highlighted)
			{
				if (this.Title.Length > 0)
				{
					float alpha = this.Container.Alpha;
					Color strokeColor = Color.Black * (alpha * alpha * alpha);
					int num = this.TopPadding ? 32 : 0;
					Vector2 position2 = position + ((this.Container.InnerContent == TextMenu.InnerContentMode.TwoColumn) ? new Vector2(0f, (float)num) : new Vector2(this.Container.Width * 0.5f, (float)num));
					Vector2 justify = new Vector2((this.Container.InnerContent == TextMenu.InnerContentMode.TwoColumn) ? 0f : 0.5f, 0.5f);
					ActiveFont.DrawOutline(this.Title, position2, justify, Vector2.One * 0.6f, Color.Gray * alpha, 2f, strokeColor);
				}
			}

			// Token: 0x04002632 RID: 9778
			public const float Scale = 0.6f;

			// Token: 0x04002633 RID: 9779
			public string Title;

			// Token: 0x04002634 RID: 9780
			public bool TopPadding = true;
		}

		// Token: 0x0200055E RID: 1374
		public class Option<T> : TextMenu.Item
		{
			// Token: 0x06002645 RID: 9797 RVA: 0x000FC147 File Offset: 0x000FA347
			public Option(string label)
			{
				this.Label = label;
				this.Selectable = true;
			}

			// Token: 0x06002646 RID: 9798 RVA: 0x000FC168 File Offset: 0x000FA368
			public TextMenu.Option<T> Add(string label, T value, bool selected = false)
			{
				this.Values.Add(new Tuple<string, T>(label, value));
				if (selected)
				{
					this.PreviousIndex = (this.Index = this.Values.Count - 1);
				}
				return this;
			}

			// Token: 0x06002647 RID: 9799 RVA: 0x000FC1A7 File Offset: 0x000FA3A7
			public TextMenu.Option<T> Change(Action<T> action)
			{
				this.OnValueChange = action;
				return this;
			}

			// Token: 0x06002648 RID: 9800 RVA: 0x000FC1B1 File Offset: 0x000FA3B1
			public override void Added()
			{
				this.Container.InnerContent = TextMenu.InnerContentMode.TwoColumn;
			}

			// Token: 0x06002649 RID: 9801 RVA: 0x000FC1C0 File Offset: 0x000FA3C0
			public override void LeftPressed()
			{
				if (this.Index > 0)
				{
					Audio.Play("event:/ui/main/button_toggle_off");
					this.PreviousIndex = this.Index;
					this.Index--;
					this.lastDir = -1;
					this.ValueWiggler.Start();
					if (this.OnValueChange != null)
					{
						this.OnValueChange(this.Values[this.Index].Item2);
					}
				}
			}

			// Token: 0x0600264A RID: 9802 RVA: 0x000FC238 File Offset: 0x000FA438
			public override void RightPressed()
			{
				if (this.Index < this.Values.Count - 1)
				{
					Audio.Play("event:/ui/main/button_toggle_on");
					this.PreviousIndex = this.Index;
					this.Index++;
					this.lastDir = 1;
					this.ValueWiggler.Start();
					if (this.OnValueChange != null)
					{
						this.OnValueChange(this.Values[this.Index].Item2);
					}
				}
			}

			// Token: 0x0600264B RID: 9803 RVA: 0x000FC2BC File Offset: 0x000FA4BC
			public override void ConfirmPressed()
			{
				if (this.Values.Count == 2)
				{
					if (this.Index == 0)
					{
						Audio.Play("event:/ui/main/button_toggle_on");
					}
					else
					{
						Audio.Play("event:/ui/main/button_toggle_off");
					}
					this.PreviousIndex = this.Index;
					this.Index = 1 - this.Index;
					this.lastDir = ((this.Index == 1) ? 1 : -1);
					this.ValueWiggler.Start();
					if (this.OnValueChange != null)
					{
						this.OnValueChange(this.Values[this.Index].Item2);
					}
				}
			}

			// Token: 0x0600264C RID: 9804 RVA: 0x000FC35B File Offset: 0x000FA55B
			public override void Update()
			{
				this.sine += Engine.RawDeltaTime;
			}

			// Token: 0x0600264D RID: 9805 RVA: 0x000FC36F File Offset: 0x000FA56F
			public override float LeftWidth()
			{
				return ActiveFont.Measure(this.Label).X + 32f;
			}

			// Token: 0x0600264E RID: 9806 RVA: 0x000FC388 File Offset: 0x000FA588
			public override float RightWidth()
			{
				float num = 0f;
				foreach (Tuple<string, T> tuple in this.Values)
				{
					num = Math.Max(num, ActiveFont.Measure(tuple.Item1).X);
				}
				return num + 120f;
			}

			// Token: 0x0600264F RID: 9807 RVA: 0x000FC3F8 File Offset: 0x000FA5F8
			public override float Height()
			{
				return ActiveFont.LineHeight;
			}

			// Token: 0x06002650 RID: 9808 RVA: 0x000FC400 File Offset: 0x000FA600
			public override void Render(Vector2 position, bool highlighted)
			{
				float alpha = this.Container.Alpha;
				Color strokeColor = Color.Black * (alpha * alpha * alpha);
				Color color = this.Disabled ? Color.DarkSlateGray : ((highlighted ? this.Container.HighlightColor : Color.White) * alpha);
				ActiveFont.DrawOutline(this.Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);
				if (this.Values.Count > 0)
				{
					float num = this.RightWidth();
					ActiveFont.DrawOutline(this.Values[this.Index].Item1, position + new Vector2(this.Container.Width - num * 0.5f + (float)this.lastDir * this.ValueWiggler.Value * 8f, 0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, color, 2f, strokeColor);
					Vector2 vector = Vector2.UnitX * (highlighted ? ((float)Math.Sin((double)(this.sine * 4f)) * 4f) : 0f);
					bool flag = this.Index > 0;
					Color color2 = flag ? color : (Color.DarkSlateGray * alpha);
					Vector2 position2 = position + new Vector2(this.Container.Width - num + 40f + ((this.lastDir < 0) ? (-this.ValueWiggler.Value * 8f) : 0f), 0f) - (flag ? vector : Vector2.Zero);
					ActiveFont.DrawOutline("<", position2, new Vector2(0.5f, 0.5f), Vector2.One, color2, 2f, strokeColor);
					bool flag2 = this.Index < this.Values.Count - 1;
					Color color3 = flag2 ? color : (Color.DarkSlateGray * alpha);
					Vector2 position3 = position + new Vector2(this.Container.Width - 40f + ((this.lastDir > 0) ? (this.ValueWiggler.Value * 8f) : 0f), 0f) + (flag2 ? vector : Vector2.Zero);
					ActiveFont.DrawOutline(">", position3, new Vector2(0.5f, 0.5f), Vector2.One, color3, 2f, strokeColor);
				}
			}

			// Token: 0x04002635 RID: 9781
			public string Label;

			// Token: 0x04002636 RID: 9782
			public int Index;

			// Token: 0x04002637 RID: 9783
			public Action<T> OnValueChange;

			// Token: 0x04002638 RID: 9784
			public int PreviousIndex;

			// Token: 0x04002639 RID: 9785
			public List<Tuple<string, T>> Values = new List<Tuple<string, T>>();

			// Token: 0x0400263A RID: 9786
			private float sine;

			// Token: 0x0400263B RID: 9787
			private int lastDir;
		}

		// Token: 0x0200055F RID: 1375
		public class Slider : TextMenu.Option<int>
		{
			// Token: 0x06002651 RID: 9809 RVA: 0x000FC68C File Offset: 0x000FA88C
			public Slider(string label, Func<int, string> values, int min, int max, int value = -1) : base(label)
			{
				for (int i = min; i <= max; i++)
				{
					base.Add(values(i), i, value == i);
				}
			}
		}

		// Token: 0x02000560 RID: 1376
		public class OnOff : TextMenu.Option<bool>
		{
			// Token: 0x06002652 RID: 9810 RVA: 0x000FC6C1 File Offset: 0x000FA8C1
			public OnOff(string label, bool on) : base(label)
			{
				base.Add(Dialog.Clean("options_off", null), false, !on);
				base.Add(Dialog.Clean("options_on", null), true, on);
			}
		}

		// Token: 0x02000561 RID: 1377
		public class Setting : TextMenu.Item
		{
			// Token: 0x06002653 RID: 9811 RVA: 0x000FC6F5 File Offset: 0x000FA8F5
			public Setting(string label, string value = "")
			{
				this.Label = label;
				this.Values.Add(value);
				this.Selectable = true;
			}

			// Token: 0x06002654 RID: 9812 RVA: 0x000FC72D File Offset: 0x000FA92D
			public Setting(string label, Binding binding, bool controllerMode) : this(label, "")
			{
				this.Binding = binding;
				this.BindingController = controllerMode;
				this.bindingHash = 0;
			}

			// Token: 0x06002655 RID: 9813 RVA: 0x000FC750 File Offset: 0x000FA950
			public void Set(List<Keys> keys)
			{
				this.Values.Clear();
				int i = 0;
				int num = Math.Min(Input.MaxBindings, keys.Count);
				while (i < num)
				{
					if (keys[i] != Keys.None)
					{
						MTexture mtexture = Input.GuiKey(keys[i], null);
						if (mtexture != null)
						{
							this.Values.Add(mtexture);
						}
						else
						{
							string text = keys[i].ToString();
							string text2 = "";
							for (int j = 0; j < text.Length; j++)
							{
								if (j > 0 && char.IsUpper(text[j]))
								{
									text2 += " ";
								}
								text2 += text[j].ToString();
							}
							this.Values.Add(text2);
						}
					}
					i++;
				}
			}

			// Token: 0x06002656 RID: 9814 RVA: 0x000FC834 File Offset: 0x000FAA34
			public void Set(List<Buttons> buttons)
			{
				this.Values.Clear();
				int i = 0;
				int num = Math.Min(Input.MaxBindings, buttons.Count);
				while (i < num)
				{
					MTexture mtexture = Input.GuiSingleButton(buttons[i], Input.PrefixMode.Latest, null);
					if (mtexture != null)
					{
						this.Values.Add(mtexture);
					}
					else
					{
						string text = buttons[i].ToString();
						string text2 = "";
						for (int j = 0; j < text.Length; j++)
						{
							if (j > 0 && char.IsUpper(text[j]))
							{
								text2 += " ";
							}
							text2 += text[j].ToString();
						}
						this.Values.Add(text2);
					}
					i++;
				}
			}

			// Token: 0x06002657 RID: 9815 RVA: 0x000FC1B1 File Offset: 0x000FA3B1
			public override void Added()
			{
				this.Container.InnerContent = TextMenu.InnerContentMode.TwoColumn;
			}

			// Token: 0x06002658 RID: 9816 RVA: 0x000FC90B File Offset: 0x000FAB0B
			public override void ConfirmPressed()
			{
				Audio.Play(this.ConfirmSfx);
				base.ConfirmPressed();
			}

			// Token: 0x06002659 RID: 9817 RVA: 0x000FC91F File Offset: 0x000FAB1F
			public override float LeftWidth()
			{
				return ActiveFont.Measure(this.Label).X;
			}

			// Token: 0x0600265A RID: 9818 RVA: 0x000FC934 File Offset: 0x000FAB34
			public override float RightWidth()
			{
				float num = 0f;
				foreach (object obj in this.Values)
				{
					if (obj is MTexture)
					{
						num += (float)(obj as MTexture).Width;
					}
					else if (obj is string)
					{
						num += ActiveFont.Measure(obj as string).X * 0.7f + 16f;
					}
				}
				return num;
			}

			// Token: 0x0600265B RID: 9819 RVA: 0x000FC9C8 File Offset: 0x000FABC8
			public override float Height()
			{
				return ActiveFont.LineHeight * 1.2f;
			}

			// Token: 0x0600265C RID: 9820 RVA: 0x000FC9D8 File Offset: 0x000FABD8
			public override void Update()
			{
				if (this.Binding != null)
				{
					int num = 17;
					if (this.BindingController)
					{
						using (List<Buttons>.Enumerator enumerator = this.Binding.Controller.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Buttons buttons = enumerator.Current;
								num = num * 31 + buttons.GetHashCode();
							}
							goto IL_A5;
						}
					}
					foreach (Keys keys in this.Binding.Keyboard)
					{
						num = num * 31 + keys.GetHashCode();
					}
					IL_A5:
					if (num != this.bindingHash)
					{
						this.bindingHash = num;
						if (this.BindingController)
						{
							this.Set(this.Binding.Controller);
							return;
						}
						this.Set(this.Binding.Keyboard);
					}
				}
			}

			// Token: 0x0600265D RID: 9821 RVA: 0x000FCAE4 File Offset: 0x000FACE4
			public override void Render(Vector2 position, bool highlighted)
			{
				float alpha = this.Container.Alpha;
				Color strokeColor = Color.Black * (alpha * alpha * alpha);
				Color color = this.Disabled ? Color.DarkSlateGray : ((highlighted ? this.Container.HighlightColor : Color.White) * alpha);
				ActiveFont.DrawOutline(this.Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);
				float num = this.RightWidth();
				foreach (object obj in this.Values)
				{
					if (obj is MTexture)
					{
						MTexture mtexture = obj as MTexture;
						mtexture.DrawJustified(position + new Vector2(this.Container.Width - num, 0f), new Vector2(0f, 0.5f), Color.White * alpha);
						num -= (float)mtexture.Width;
					}
					else if (obj is string)
					{
						string text = obj as string;
						float num2 = ActiveFont.Measure(obj as string).X * 0.7f + 16f;
						ActiveFont.DrawOutline(text, position + new Vector2(this.Container.Width - num + num2 * 0.5f, 0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.7f, Color.LightGray * alpha, 2f, strokeColor);
						num -= num2;
					}
				}
			}

			// Token: 0x0400263C RID: 9788
			public string ConfirmSfx = "event:/ui/main/button_select";

			// Token: 0x0400263D RID: 9789
			public string Label;

			// Token: 0x0400263E RID: 9790
			public List<object> Values = new List<object>();

			// Token: 0x0400263F RID: 9791
			public Binding Binding;

			// Token: 0x04002640 RID: 9792
			public bool BindingController;

			// Token: 0x04002641 RID: 9793
			private int bindingHash;
		}

		// Token: 0x02000562 RID: 1378
		public class Button : TextMenu.Item
		{
			// Token: 0x0600265E RID: 9822 RVA: 0x000FCCAC File Offset: 0x000FAEAC
			public Button(string label)
			{
				this.Label = label;
				this.Selectable = true;
			}

			// Token: 0x0600265F RID: 9823 RVA: 0x000FCCCD File Offset: 0x000FAECD
			public override void ConfirmPressed()
			{
				if (!string.IsNullOrEmpty(this.ConfirmSfx))
				{
					Audio.Play(this.ConfirmSfx);
				}
				base.ConfirmPressed();
			}

			// Token: 0x06002660 RID: 9824 RVA: 0x000FCCEE File Offset: 0x000FAEEE
			public override float LeftWidth()
			{
				return ActiveFont.Measure(this.Label).X;
			}

			// Token: 0x06002661 RID: 9825 RVA: 0x000FC3F8 File Offset: 0x000FA5F8
			public override float Height()
			{
				return ActiveFont.LineHeight;
			}

			// Token: 0x06002662 RID: 9826 RVA: 0x000FCD00 File Offset: 0x000FAF00
			public override void Render(Vector2 position, bool highlighted)
			{
				float alpha = this.Container.Alpha;
				Color color = this.Disabled ? Color.DarkSlateGray : ((highlighted ? this.Container.HighlightColor : Color.White) * alpha);
				Color strokeColor = Color.Black * (alpha * alpha * alpha);
				bool flag = this.Container.InnerContent == TextMenu.InnerContentMode.TwoColumn && !this.AlwaysCenter;
				Vector2 position2 = position + (flag ? Vector2.Zero : new Vector2(this.Container.Width * 0.5f, 0f));
				Vector2 justify = (flag && !this.AlwaysCenter) ? new Vector2(0f, 0.5f) : new Vector2(0.5f, 0.5f);
				ActiveFont.DrawOutline(this.Label, position2, justify, Vector2.One, color, 2f, strokeColor);
			}

			// Token: 0x04002642 RID: 9794
			public string ConfirmSfx = "event:/ui/main/button_select";

			// Token: 0x04002643 RID: 9795
			public string Label;

			// Token: 0x04002644 RID: 9796
			public bool AlwaysCenter;
		}

		// Token: 0x02000563 RID: 1379
		public class LanguageButton : TextMenu.Item
		{
			// Token: 0x06002663 RID: 9827 RVA: 0x000FCDE4 File Offset: 0x000FAFE4
			public LanguageButton(string label, Language language)
			{
				this.Label = label;
				this.Language = language;
				this.Selectable = true;
			}

			// Token: 0x06002664 RID: 9828 RVA: 0x000FCE0C File Offset: 0x000FB00C
			public override void ConfirmPressed()
			{
				Audio.Play(this.ConfirmSfx);
				base.ConfirmPressed();
			}

			// Token: 0x06002665 RID: 9829 RVA: 0x000FCE20 File Offset: 0x000FB020
			public override float LeftWidth()
			{
				return ActiveFont.Measure(this.Label).X;
			}

			// Token: 0x06002666 RID: 9830 RVA: 0x000FCE32 File Offset: 0x000FB032
			public override float RightWidth()
			{
				return (float)this.Language.Icon.Width;
			}

			// Token: 0x06002667 RID: 9831 RVA: 0x000FC3F8 File Offset: 0x000FA5F8
			public override float Height()
			{
				return ActiveFont.LineHeight;
			}

			// Token: 0x06002668 RID: 9832 RVA: 0x000FCE48 File Offset: 0x000FB048
			public override void Render(Vector2 position, bool highlighted)
			{
				float alpha = this.Container.Alpha;
				Color color = this.Disabled ? Color.DarkSlateGray : ((highlighted ? this.Container.HighlightColor : Color.White) * alpha);
				Color strokeColor = Color.Black * (alpha * alpha * alpha);
				ActiveFont.DrawOutline(this.Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);
				this.Language.Icon.DrawJustified(position + new Vector2(this.Container.Width - this.RightWidth(), 0f), new Vector2(0f, 0.5f), Color.White, 1f);
			}

			// Token: 0x04002645 RID: 9797
			public string ConfirmSfx = "event:/ui/main/button_select";

			// Token: 0x04002646 RID: 9798
			public string Label;

			// Token: 0x04002647 RID: 9799
			public Language Language;

			// Token: 0x04002648 RID: 9800
			public bool AlwaysCenter;
		}
	}
}
