using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x02000390 RID: 912
	public class PreviewDialog : Scene
	{
		// Token: 0x06001D92 RID: 7570 RVA: 0x000CD5A0 File Offset: 0x000CB7A0
		public PreviewDialog(Language language = null, float listScroll = 64f, float textboxScroll = 0f, string dialog = null)
		{
			this.listScroll.Y = listScroll;
			this.textboxScroll.Y = textboxScroll;
			if (language != null)
			{
				this.SetLanguage(language);
			}
			if (dialog != null)
			{
				this.SetCurrent(dialog);
			}
			base.Add(new PreviewDialog.Renderer(this));
		}

		// Token: 0x06001D93 RID: 7571 RVA: 0x000CD62D File Offset: 0x000CB82D
		public override void End()
		{
			base.End();
			this.UnsetLanguage();
		}

		// Token: 0x06001D94 RID: 7572 RVA: 0x000CD63C File Offset: 0x000CB83C
		public override void Update()
		{
			if (!Engine.Instance.IsActive)
			{
				this.delay = 0.1f;
				return;
			}
			if (this.delay > 0f)
			{
				this.delay -= Engine.DeltaTime;
				return;
			}
			if (this.current != null)
			{
				float num = 1f;
				foreach (object obj in this.elements)
				{
					Textbox textbox = obj as Textbox;
					if (textbox != null)
					{
						textbox.RenderOffset = this.textboxScroll + Vector2.UnitY * num;
						num += 300f;
						if (textbox.Scene != null)
						{
							textbox.Update();
						}
					}
					else
					{
						num += (float)(this.language.FontSize.LineHeight + 50);
					}
				}
				this.textboxScroll.Y = this.textboxScroll.Y + (float)MInput.Mouse.WheelDelta * Engine.DeltaTime * ActiveFont.LineHeight;
				this.textboxScroll.Y = this.textboxScroll.Y - Input.Aim.Value.Y * Engine.DeltaTime * ActiveFont.LineHeight * 20f;
				this.textboxScroll.Y = Calc.Clamp(this.textboxScroll.Y, 716f - num, 64f);
				if (MInput.Keyboard.Pressed(Keys.Escape) || Input.MenuConfirm.Pressed)
				{
					this.ClearTextboxes();
				}
				else if (MInput.Keyboard.Pressed(Keys.Space))
				{
					string item = this.current;
					this.ClearTextboxes();
					int num2 = this.list.IndexOf(item) + 1;
					if (num2 < this.list.Count)
					{
						this.SetCurrent(this.list[num2]);
					}
				}
			}
			else
			{
				this.listScroll.Y = this.listScroll.Y + (float)MInput.Mouse.WheelDelta * Engine.DeltaTime * ActiveFont.LineHeight;
				this.listScroll.Y = this.listScroll.Y - Input.Aim.Value.Y * Engine.DeltaTime * ActiveFont.LineHeight * 20f;
				this.listScroll.Y = Calc.Clamp(this.listScroll.Y, 1016f - (float)this.list.Count * ActiveFont.LineHeight * 0.6f, 64f);
				if (this.language != null)
				{
					if (MInput.Mouse.PressedLeftButton)
					{
						for (int i = 0; i < this.list.Count; i++)
						{
							if (this.MouseOverOption(i))
							{
								this.SetCurrent(this.list[i]);
								break;
							}
						}
					}
					if (MInput.Keyboard.Pressed(Keys.Escape) || Input.MenuConfirm.Pressed)
					{
						this.listScroll = new Vector2(64f, 64f);
						this.UnsetLanguage();
					}
				}
				else if (MInput.Mouse.PressedLeftButton)
				{
					int num3 = 0;
					foreach (KeyValuePair<string, Language> keyValuePair in Dialog.Languages)
					{
						if (this.MouseOverOption(num3))
						{
							this.SetLanguage(keyValuePair.Value);
							this.listScroll = new Vector2(64f, 64f);
							break;
						}
						num3++;
					}
				}
			}
			if (MInput.Keyboard.Pressed(Keys.F2))
			{
				Celeste.ReloadPortraits();
				Engine.Scene = new PreviewDialog(this.language, this.listScroll.Y, this.textboxScroll.Y, this.current);
			}
			if (MInput.Keyboard.Pressed(Keys.F1) && this.language != null)
			{
				Celeste.ReloadDialog();
				Engine.Scene = new PreviewDialog(Dialog.Languages[this.language.Id], this.listScroll.Y, this.textboxScroll.Y, this.current);
			}
		}

		// Token: 0x06001D95 RID: 7573 RVA: 0x000CDA50 File Offset: 0x000CBC50
		private void ClearTextboxes()
		{
			foreach (object obj in this.elements)
			{
				if (obj is Textbox)
				{
					base.Remove(obj as Textbox);
				}
			}
			this.current = null;
			this.textboxScroll = Vector2.Zero;
		}

		// Token: 0x06001D96 RID: 7574 RVA: 0x000CDAC4 File Offset: 0x000CBCC4
		private void SetCurrent(string id)
		{
			this.current = id;
			this.elements.Clear();
			Textbox textbox = null;
			int num = 0;
			for (;;)
			{
				Textbox textbox2 = new Textbox(id, this.language, new Func<IEnumerator>[0]);
				if (!textbox2.SkipToPage(num))
				{
					break;
				}
				if (textbox != null)
				{
					int num2 = textbox.Start + 1;
					while (num2 <= textbox2.Start && num2 < textbox.Nodes.Count)
					{
						FancyText.Trigger trigger = textbox.Nodes[num2] as FancyText.Trigger;
						if (trigger != null)
						{
							this.elements.Add(string.Concat(new object[]
							{
								trigger.Silent ? "Silent " : "",
								"Trigger [",
								trigger.Index,
								"] ",
								trigger.Label
							}));
						}
						num2++;
					}
				}
				base.Add(textbox2);
				this.elements.Add(textbox2);
				textbox2.RenderOffset = this.textboxScroll + Vector2.UnitY * (float)(1 + num * 300);
				textbox = textbox2;
				num++;
			}
		}

		// Token: 0x06001D97 RID: 7575 RVA: 0x000CDBE8 File Offset: 0x000CBDE8
		private void SetLanguage(Language lan)
		{
			Fonts.Load(lan.FontFace);
			this.language = lan;
			this.list.Clear();
			bool flag = false;
			foreach (KeyValuePair<string, string> keyValuePair in this.language.Dialog)
			{
				if (!flag && keyValuePair.Key.StartsWith("CH0", StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
				}
				if (flag && !keyValuePair.Key.StartsWith("poem_", StringComparison.OrdinalIgnoreCase) && !keyValuePair.Key.StartsWith("journal_", StringComparison.OrdinalIgnoreCase))
				{
					this.list.Add(keyValuePair.Key);
				}
			}
		}

		// Token: 0x06001D98 RID: 7576 RVA: 0x000CDCB0 File Offset: 0x000CBEB0
		private void UnsetLanguage()
		{
			if (this.language != null && this.language.Id != Settings.Instance.Language && this.language.FontFace != Dialog.Languages["english"].FontFace)
			{
				Fonts.Unload(this.language.FontFace);
			}
			this.language = null;
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06001D99 RID: 7577 RVA: 0x0008F4D8 File Offset: 0x0008D6D8
		public Vector2 Mouse
		{
			get
			{
				return Vector2.Transform(new Vector2((float)MInput.Mouse.CurrentState.X, (float)MInput.Mouse.CurrentState.Y), Matrix.Invert(Engine.ScreenMatrix));
			}
		}

		// Token: 0x06001D9A RID: 7578 RVA: 0x000CDD20 File Offset: 0x000CBF20
		private void RenderContent()
		{
			Draw.Rect(0f, 0f, 960f, 1080f, Color.DarkSlateGray * 0.25f);
			if (this.current != null)
			{
				int num = 1;
				int num2 = 0;
				foreach (object obj in this.elements)
				{
					Textbox textbox = obj as Textbox;
					if (textbox != null)
					{
						if (textbox.Opened && this.language.Font.Sizes.Count > 0)
						{
							textbox.Render();
							this.language.Font.DrawOutline(this.language.FontFaceSize, "#" + num.ToString(), textbox.RenderOffset + new Vector2(32f, 64f), Vector2.Zero, Vector2.One * 0.5f, Color.White, 2f, Color.Black);
							num++;
							num2 += 300;
						}
					}
					else
					{
						this.language.Font.DrawOutline(this.language.FontFaceSize, obj.ToString(), this.textboxScroll + new Vector2(128f, (float)(num2 + 50 + this.language.FontSize.LineHeight)), new Vector2(0f, 0.5f), Vector2.One * 0.5f, Color.White, 2f, Color.Black);
						num2 += this.language.FontSize.LineHeight + 50;
					}
				}
				ActiveFont.DrawOutline(this.current, new Vector2(1888f, 32f), new Vector2(1f, 0f), Vector2.One * 0.5f, Color.Red, 2f, Color.Black);
			}
			else
			{
				if (this.language != null)
				{
					int num3 = 0;
					using (List<string>.Enumerator enumerator2 = this.list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							string text = enumerator2.Current;
							if (this.language.Font.Sizes.Count > 0)
							{
								this.language.Font.Draw(this.language.FontFaceSize, text, this.listScroll + new Vector2(0f, (float)num3 * ActiveFont.LineHeight * 0.6f), Vector2.Zero, Vector2.One * 0.6f, this.MouseOverOption(num3) ? Color.White : Color.Gray);
							}
							num3++;
						}
						goto IL_368;
					}
				}
				int num4 = 0;
				foreach (KeyValuePair<string, Language> keyValuePair in Dialog.Languages)
				{
					ActiveFont.Draw(keyValuePair.Value.Id, this.listScroll + new Vector2(0f, (float)num4 * ActiveFont.LineHeight * 0.6f), Vector2.Zero, Vector2.One * 0.6f, this.MouseOverOption(num4) ? Color.White : Color.Gray);
					num4++;
				}
			}
			IL_368:
			Draw.Rect(this.Mouse.X - 12f, this.Mouse.Y - 4f, 24f, 8f, Color.Red);
			Draw.Rect(this.Mouse.X - 4f, this.Mouse.Y - 12f, 8f, 24f, Color.Red);
		}

		// Token: 0x06001D9B RID: 7579 RVA: 0x000CE150 File Offset: 0x000CC350
		private bool MouseOverOption(int i)
		{
			return this.Mouse.X > this.listScroll.X && this.Mouse.Y > this.listScroll.Y + (float)i * ActiveFont.LineHeight * 0.6f && MInput.Mouse.X < 960f && this.Mouse.Y < this.listScroll.Y + (float)(i + 1) * ActiveFont.LineHeight * 0.6f;
		}

		// Token: 0x04001E5D RID: 7773
		private Language language;

		// Token: 0x04001E5E RID: 7774
		private List<string> list = new List<string>();

		// Token: 0x04001E5F RID: 7775
		private Vector2 listScroll = new Vector2(64f, 64f);

		// Token: 0x04001E60 RID: 7776
		private const float scale = 0.6f;

		// Token: 0x04001E61 RID: 7777
		private string current;

		// Token: 0x04001E62 RID: 7778
		private List<object> elements = new List<object>();

		// Token: 0x04001E63 RID: 7779
		private Vector2 textboxScroll = new Vector2(0f, 0f);

		// Token: 0x04001E64 RID: 7780
		private float delay;

		// Token: 0x02000758 RID: 1880
		private class Renderer : HiresRenderer
		{
			// Token: 0x06002F49 RID: 12105 RVA: 0x00128BFA File Offset: 0x00126DFA
			public Renderer(PreviewDialog previewer)
			{
				this.previewer = previewer;
			}

			// Token: 0x06002F4A RID: 12106 RVA: 0x00128C09 File Offset: 0x00126E09
			public override void RenderContent(Scene scene)
			{
				HiresRenderer.BeginRender(null, null);
				this.previewer.RenderContent();
				HiresRenderer.EndRender();
			}

			// Token: 0x04002EE6 RID: 12006
			public PreviewDialog previewer;
		}
	}
}
