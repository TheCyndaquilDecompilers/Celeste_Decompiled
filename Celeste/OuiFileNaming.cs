using System;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x020002FC RID: 764
	public class OuiFileNaming : Oui
	{
		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060017C5 RID: 6085 RVA: 0x000934F4 File Offset: 0x000916F4
		// (set) Token: 0x060017C6 RID: 6086 RVA: 0x00093501 File Offset: 0x00091701
		public string Name
		{
			get
			{
				return this.FileSlot.Name;
			}
			set
			{
				this.FileSlot.Name = value;
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x060017C7 RID: 6087 RVA: 0x0009350F File Offset: 0x0009170F
		public int MaxNameLength
		{
			get
			{
				if (!this.Japanese)
				{
					return 12;
				}
				return 8;
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060017C8 RID: 6088 RVA: 0x0009351D File Offset: 0x0009171D
		public bool Japanese
		{
			get
			{
				return Settings.Instance.Language == "japanese";
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x060017C9 RID: 6089 RVA: 0x00093533 File Offset: 0x00091733
		private Vector2 boxtopleft
		{
			get
			{
				return this.Position + new Vector2((1920f - this.boxWidth) / 2f, 360f + (680f - this.boxHeight) / 2f);
			}
		}

		// Token: 0x060017CA RID: 6090 RVA: 0x00093570 File Offset: 0x00091770
		public OuiFileNaming()
		{
			this.wiggler = Wiggler.Create(0.25f, 4f, null, false, false);
			this.Position = new Vector2(0f, 1080f);
			this.Visible = false;
		}

		// Token: 0x060017CB RID: 6091 RVA: 0x000935FB File Offset: 0x000917FB
		public override IEnumerator Enter(Oui from)
		{
			if (this.Name == Dialog.Clean("FILE_DEFAULT", null) || (Settings.Instance != null && this.Name == Settings.Instance.DefaultFileName))
			{
				this.Name = "";
			}
			base.Overworld.ShowInputUI = false;
			this.selectingOptions = false;
			this.optionsIndex = 0;
			this.index = 0;
			this.line = 0;
			this.ReloadLetters(Dialog.Clean("name_letters", null));
			this.optionsScale = 0.75f;
			this.cancel = Dialog.Clean("name_back", null);
			this.space = Dialog.Clean("name_space", null);
			this.backspace = Dialog.Clean("name_backspace", null);
			this.accept = Dialog.Clean("name_accept", null);
			this.cancelWidth = ActiveFont.Measure(this.cancel).X * this.optionsScale;
			this.spaceWidth = ActiveFont.Measure(this.space).X * this.optionsScale;
			this.backspaceWidth = ActiveFont.Measure(this.backspace).X * this.optionsScale;
			this.beginWidth = ActiveFont.Measure(this.accept).X * this.optionsScale * 1.25f;
			this.optionsWidth = this.cancelWidth + this.spaceWidth + this.backspaceWidth + this.beginWidth + this.widestLetter * 3f;
			this.Visible = true;
			Vector2 posFrom = this.Position;
			Vector2 posTo = Vector2.Zero;
			for (float t = 0f; t < 1f; t += Engine.DeltaTime * 3f)
			{
				this.ease = Ease.CubeIn(t);
				this.Position = posFrom + (posTo - posFrom) * Ease.CubeInOut(t);
				yield return null;
			}
			this.ease = 1f;
			posFrom = default(Vector2);
			posTo = default(Vector2);
			yield return 0.05f;
			this.Focused = true;
			yield return 0.05f;
			this.wiggler.Start();
			yield break;
		}

		// Token: 0x060017CC RID: 6092 RVA: 0x0009360C File Offset: 0x0009180C
		private void ReloadLetters(string chars)
		{
			this.letters = chars.Split(new char[]
			{
				'\n'
			});
			this.widestLetter = 0f;
			for (int i = 0; i < chars.Length; i++)
			{
				float x = ActiveFont.Measure(chars[i]).X;
				if (x > this.widestLetter)
				{
					this.widestLetter = x;
				}
			}
			if (this.Japanese)
			{
				this.widestLetter *= 1.5f;
			}
			this.widestLineCount = 0;
			foreach (string text in this.letters)
			{
				if (text.Length > this.widestLineCount)
				{
					this.widestLineCount = text.Length;
				}
			}
			this.widestLine = (float)this.widestLineCount * this.widestLetter;
			this.lineHeight = ActiveFont.LineHeight;
			this.lineSpacing = ActiveFont.LineHeight * 0.1f;
			this.boxPadding = this.widestLetter;
			this.boxWidth = Math.Max(this.widestLine, this.optionsWidth) + this.boxPadding * 2f;
			this.boxHeight = (float)(this.letters.Length + 1) * this.lineHeight + (float)this.letters.Length * this.lineSpacing + this.boxPadding * 3f;
		}

		// Token: 0x060017CD RID: 6093 RVA: 0x0009375D File Offset: 0x0009195D
		public override IEnumerator Leave(Oui next)
		{
			base.Overworld.ShowInputUI = true;
			this.Focused = false;
			Vector2 posFrom = this.Position;
			Vector2 posTo = new Vector2(0f, 1080f);
			for (float t = 0f; t < 1f; t += Engine.DeltaTime * 2f)
			{
				this.ease = 1f - Ease.CubeIn(t);
				this.Position = posFrom + (posTo - posFrom) * Ease.CubeInOut(t);
				yield return null;
			}
			this.Visible = false;
			yield break;
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x0009376C File Offset: 0x0009196C
		public override void Update()
		{
			base.Update();
			if (base.Selected && this.Focused)
			{
				if (!string.IsNullOrWhiteSpace(this.Name) && MInput.Keyboard.Check(Keys.LeftControl) && MInput.Keyboard.Pressed(Keys.S))
				{
					this.ResetDefaultName();
				}
				if (Input.MenuJournal.Pressed && this.Japanese)
				{
					this.SwapType();
				}
				if (Input.MenuRight.Pressed && (this.optionsIndex < 3 || !this.selectingOptions) && (this.Name.Length > 0 || !this.selectingOptions))
				{
					if (this.selectingOptions)
					{
						this.optionsIndex = Math.Min(this.optionsIndex + 1, 3);
					}
					else
					{
						do
						{
							this.index = (this.index + 1) % this.letters[this.line].Length;
						}
						while (this.letters[this.line][this.index] == ' ');
					}
					this.wiggler.Start();
					Audio.Play("event:/ui/main/rename_entry_rollover");
				}
				else if (Input.MenuLeft.Pressed && (this.optionsIndex > 0 || !this.selectingOptions))
				{
					if (this.selectingOptions)
					{
						this.optionsIndex = Math.Max(this.optionsIndex - 1, 0);
					}
					else
					{
						do
						{
							this.index = (this.index + this.letters[this.line].Length - 1) % this.letters[this.line].Length;
						}
						while (this.letters[this.line][this.index] == ' ');
					}
					this.wiggler.Start();
					Audio.Play("event:/ui/main/rename_entry_rollover");
				}
				else
				{
					if (Input.MenuDown.Pressed && !this.selectingOptions)
					{
						for (int i = this.line + 1; i < this.letters.Length; i++)
						{
							if (this.index < this.letters[i].Length && this.letters[i][this.index] != ' ')
							{
								this.line = i;
								IL_235:
								if (this.selectingOptions)
								{
									float num = (float)this.index * this.widestLetter;
									float num2 = this.boxWidth - this.boxPadding * 2f;
									if (this.Name.Length == 0 || num < this.cancelWidth + (num2 - this.cancelWidth - this.beginWidth - this.backspaceWidth - this.spaceWidth - this.widestLetter * 3f) / 2f)
									{
										this.optionsIndex = 0;
									}
									else if (num < num2 - this.beginWidth - this.backspaceWidth - this.widestLetter * 2f)
									{
										this.optionsIndex = 1;
									}
									else if (num < num2 - this.beginWidth - this.widestLetter)
									{
										this.optionsIndex = 2;
									}
									else
									{
										this.optionsIndex = 3;
									}
								}
								this.wiggler.Start();
								Audio.Play("event:/ui/main/rename_entry_rollover");
								goto IL_7FC;
							}
						}
						this.selectingOptions = true;
						goto IL_235;
					}
					if ((Input.MenuUp.Pressed || (this.selectingOptions && this.Name.Length <= 0 && this.optionsIndex > 0)) && (this.line > 0 || this.selectingOptions))
					{
						if (this.selectingOptions)
						{
							this.line = this.letters.Length;
							this.selectingOptions = false;
							float num3 = this.boxWidth - this.boxPadding * 2f;
							if (this.optionsIndex == 0)
							{
								this.index = (int)(this.cancelWidth / 2f / this.widestLetter);
							}
							else if (this.optionsIndex == 1)
							{
								this.index = (int)((num3 - this.beginWidth - this.backspaceWidth - this.spaceWidth / 2f - this.widestLetter * 2f) / this.widestLetter);
							}
							else if (this.optionsIndex == 2)
							{
								this.index = (int)((num3 - this.beginWidth - this.backspaceWidth / 2f - this.widestLetter) / this.widestLetter);
							}
							else if (this.optionsIndex == 3)
							{
								this.index = (int)((num3 - this.beginWidth / 2f) / this.widestLetter);
							}
						}
						this.line--;
						while (this.line > 0)
						{
							if (this.index < this.letters[this.line].Length && this.letters[this.line][this.index] != ' ')
							{
								break;
							}
							this.line--;
						}
						while (this.index >= this.letters[this.line].Length || this.letters[this.line][this.index] == ' ')
						{
							this.index--;
						}
						this.wiggler.Start();
						Audio.Play("event:/ui/main/rename_entry_rollover");
					}
					else if (Input.MenuConfirm.Pressed)
					{
						if (this.selectingOptions)
						{
							if (this.optionsIndex == 0)
							{
								this.Cancel();
							}
							else if (this.optionsIndex == 1 && this.Name.Length > 0)
							{
								this.Space();
							}
							else if (this.optionsIndex == 2)
							{
								this.Backspace();
							}
							else if (this.optionsIndex == 3)
							{
								this.Finish();
							}
						}
						else if (this.Japanese && this.letters[this.line][this.index] == '゛' && this.Name.Length > 0 && OuiFileNaming.dakuten_able.Contains((int)this.Name.Last<char>()))
						{
							int num4 = (int)this.Name[this.Name.Length - 1];
							num4++;
							this.Name = this.Name.Substring(0, this.Name.Length - 1);
							this.Name += ((char)num4).ToString();
							this.wiggler.Start();
							Audio.Play("event:/ui/main/rename_entry_char");
						}
						else if (this.Japanese && this.letters[this.line][this.index] == '゜' && this.Name.Length > 0 && (OuiFileNaming.handakuten_able.Contains((int)this.Name.Last<char>()) || OuiFileNaming.handakuten_able.Contains((int)(this.Name.Last<char>() + '\u0001'))))
						{
							int num5 = (int)this.Name[this.Name.Length - 1];
							if (OuiFileNaming.handakuten_able.Contains(num5))
							{
								num5++;
							}
							else
							{
								num5 += 2;
							}
							this.Name = this.Name.Substring(0, this.Name.Length - 1);
							this.Name += ((char)num5).ToString();
							this.wiggler.Start();
							Audio.Play("event:/ui/main/rename_entry_char");
						}
						else if (this.Name.Length < this.MaxNameLength)
						{
							this.Name += this.letters[this.line][this.index].ToString();
							this.wiggler.Start();
							Audio.Play("event:/ui/main/rename_entry_char");
						}
						else
						{
							Audio.Play("event:/ui/main/button_invalid");
						}
					}
					else if (Input.MenuCancel.Pressed)
					{
						if (this.Name.Length > 0)
						{
							this.Backspace();
						}
						else
						{
							this.Cancel();
						}
					}
					else if (Input.Pause.Pressed)
					{
						this.Finish();
					}
				}
			}
			IL_7FC:
			this.pressedTimer -= Engine.DeltaTime;
			this.timer += Engine.DeltaTime;
			this.wiggler.Update();
		}

		// Token: 0x060017CF RID: 6095 RVA: 0x00093FA4 File Offset: 0x000921A4
		private void ResetDefaultName()
		{
			if (this.StartingName == Settings.Instance.DefaultFileName || this.StartingName == Dialog.Clean("FILE_DEFAULT", null))
			{
				this.StartingName = this.Name;
			}
			Settings.Instance.DefaultFileName = this.Name;
			Audio.Play("event:/new_content/ui/rename_entry_accept_locked");
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x00094008 File Offset: 0x00092208
		private void Space()
		{
			if (this.Name.Length < this.MaxNameLength && this.Name.Length > 0)
			{
				this.Name += " ";
				this.wiggler.Start();
				Audio.Play("event:/ui/main/rename_entry_char");
				return;
			}
			Audio.Play("event:/ui/main/button_invalid");
		}

		// Token: 0x060017D1 RID: 6097 RVA: 0x00094070 File Offset: 0x00092270
		private void Backspace()
		{
			if (this.Name.Length > 0)
			{
				this.Name = this.Name.Substring(0, this.Name.Length - 1);
				Audio.Play("event:/ui/main/rename_entry_backspace");
				return;
			}
			Audio.Play("event:/ui/main/button_invalid");
		}

		// Token: 0x060017D2 RID: 6098 RVA: 0x000940C4 File Offset: 0x000922C4
		private void Finish()
		{
			if (this.Name.Length >= 1)
			{
				if (MInput.GamePads.Length != 0 && MInput.GamePads[0] != null && (MInput.GamePads[0].Check(Buttons.LeftTrigger) || MInput.GamePads[0].Check(Buttons.LeftShoulder)) && (MInput.GamePads[0].Check(Buttons.RightTrigger) || MInput.GamePads[0].Check(Buttons.RightShoulder)))
				{
					this.ResetDefaultName();
				}
				this.Focused = false;
				base.Overworld.Goto<OuiFileSelect>();
				Audio.Play("event:/ui/main/rename_entry_accept");
				return;
			}
			Audio.Play("event:/ui/main/button_invalid");
		}

		// Token: 0x060017D3 RID: 6099 RVA: 0x0009416F File Offset: 0x0009236F
		private void SwapType()
		{
			this.hiragana = !this.hiragana;
			if (this.hiragana)
			{
				this.ReloadLetters(Dialog.Clean("name_letters", null));
				return;
			}
			this.ReloadLetters(Dialog.Clean("name_letters_katakana", null));
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x000941AB File Offset: 0x000923AB
		private void Cancel()
		{
			this.FileSlot.Name = this.StartingName;
			this.Focused = false;
			base.Overworld.Goto<OuiFileSelect>();
			Audio.Play("event:/ui/main/button_back");
		}

		// Token: 0x060017D5 RID: 6101 RVA: 0x000941DC File Offset: 0x000923DC
		public override void Render()
		{
			Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.8f * this.ease);
			Vector2 vector = this.boxtopleft + new Vector2(this.boxPadding, this.boxPadding);
			int num = 0;
			foreach (string text in this.letters)
			{
				for (int j = 0; j < text.Length; j++)
				{
					bool flag = num == this.line && j == this.index && !this.selectingOptions;
					Vector2 scale = Vector2.One * (flag ? 1.2f : 1f);
					Vector2 vector2 = vector + new Vector2(this.widestLetter, this.lineHeight) / 2f;
					if (flag)
					{
						vector2 += new Vector2(0f, this.wiggler.Value) * 8f;
					}
					this.DrawOptionText(text[j].ToString(), vector2, new Vector2(0.5f, 0.5f), scale, flag, false);
					vector.X += this.widestLetter;
				}
				vector.X = this.boxtopleft.X + this.boxPadding;
				vector.Y += this.lineHeight + this.lineSpacing;
				num++;
			}
			float num2 = this.wiggler.Value * 8f;
			vector.Y = this.boxtopleft.Y + this.boxHeight - this.lineHeight - this.boxPadding;
			Draw.Rect(vector.X, vector.Y - this.boxPadding * 0.5f, this.boxWidth - this.boxPadding * 2f, 4f, Color.White);
			this.DrawOptionText(this.cancel, vector + new Vector2(0f, this.lineHeight + ((this.selectingOptions && this.optionsIndex == 0) ? num2 : 0f)), new Vector2(0f, 1f), Vector2.One * this.optionsScale, this.selectingOptions && this.optionsIndex == 0, false);
			vector.X = this.boxtopleft.X + this.boxWidth - this.backspaceWidth - this.widestLetter - this.spaceWidth - this.widestLetter - this.beginWidth - this.boxPadding;
			this.DrawOptionText(this.space, vector + new Vector2(0f, this.lineHeight + ((this.selectingOptions && this.optionsIndex == 1) ? num2 : 0f)), new Vector2(0f, 1f), Vector2.One * this.optionsScale, this.selectingOptions && this.optionsIndex == 1, this.Name.Length == 0 || !this.Focused);
			vector.X += this.spaceWidth + this.widestLetter;
			this.DrawOptionText(this.backspace, vector + new Vector2(0f, this.lineHeight + ((this.selectingOptions && this.optionsIndex == 2) ? num2 : 0f)), new Vector2(0f, 1f), Vector2.One * this.optionsScale, this.selectingOptions && this.optionsIndex == 2, this.Name.Length <= 0 || !this.Focused);
			vector.X += this.backspaceWidth + this.widestLetter;
			this.DrawOptionText(this.accept, vector + new Vector2(0f, this.lineHeight + ((this.selectingOptions && this.optionsIndex == 3) ? num2 : 0f)), new Vector2(0f, 1f), Vector2.One * this.optionsScale * 1.25f, this.selectingOptions && this.optionsIndex == 3, this.Name.Length < 1 || !this.Focused);
			if (this.Japanese)
			{
				float num3 = 1f;
				string text2 = Dialog.Clean(this.hiragana ? "NAME_LETTERS_SWAP_KATAKANA" : "NAME_LETTERS_SWAP_HIRAGANA", null);
				MTexture mtexture = Input.GuiButton(Input.MenuJournal, Input.PrefixMode.Latest, "controls/keyboard/oemquestion");
				ActiveFont.Measure(text2);
				float num4 = (float)mtexture.Width * num3;
				Vector2 vector3 = new Vector2(70f, 1144f - 154f * this.ease);
				mtexture.DrawJustified(vector3, new Vector2(0f, 0.5f), Color.White, num3, 0f);
				ActiveFont.DrawOutline(text2, vector3 + new Vector2(16f + num4, 0f), new Vector2(0f, 0.5f), Vector2.One * num3, Color.White, 2f, Color.Black);
			}
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x00094754 File Offset: 0x00092954
		private void DrawOptionText(string text, Vector2 at, Vector2 justify, Vector2 scale, bool selected, bool disabled = false)
		{
			bool flag = selected && this.pressedTimer > 0f;
			Color color = disabled ? this.disableColor : this.GetTextColor(selected);
			Color edgeColor = disabled ? Color.Lerp(this.disableColor, Color.Black, 0.7f) : Color.Gray;
			if (flag)
			{
				ActiveFont.Draw(text, at + Vector2.UnitY, justify, scale, color);
				return;
			}
			ActiveFont.DrawEdgeOutline(text, at, justify, scale, color, 4f, edgeColor, 0f, default(Color));
		}

		// Token: 0x060017D7 RID: 6103 RVA: 0x000947E2 File Offset: 0x000929E2
		private Color GetTextColor(bool selected)
		{
			if (!selected)
			{
				return this.unselectColor;
			}
			if (Settings.Instance.DisableFlashes)
			{
				return this.selectColorA;
			}
			if (!Calc.BetweenInterval(this.timer, 0.1f))
			{
				return this.selectColorB;
			}
			return this.selectColorA;
		}

		// Token: 0x0400147B RID: 5243
		public string StartingName;

		// Token: 0x0400147C RID: 5244
		public OuiFileSelectSlot FileSlot;

		// Token: 0x0400147D RID: 5245
		public const int MinNameLength = 1;

		// Token: 0x0400147E RID: 5246
		public const int MaxNameLengthNormal = 12;

		// Token: 0x0400147F RID: 5247
		public const int MaxNameLengthJP = 8;

		// Token: 0x04001480 RID: 5248
		private string[] letters;

		// Token: 0x04001481 RID: 5249
		private int index;

		// Token: 0x04001482 RID: 5250
		private int line;

		// Token: 0x04001483 RID: 5251
		private float widestLetter;

		// Token: 0x04001484 RID: 5252
		private float widestLine;

		// Token: 0x04001485 RID: 5253
		private int widestLineCount;

		// Token: 0x04001486 RID: 5254
		private bool selectingOptions = true;

		// Token: 0x04001487 RID: 5255
		private int optionsIndex;

		// Token: 0x04001488 RID: 5256
		private bool hiragana = true;

		// Token: 0x04001489 RID: 5257
		private float lineHeight;

		// Token: 0x0400148A RID: 5258
		private float lineSpacing;

		// Token: 0x0400148B RID: 5259
		private float boxPadding;

		// Token: 0x0400148C RID: 5260
		private float optionsScale;

		// Token: 0x0400148D RID: 5261
		private string cancel;

		// Token: 0x0400148E RID: 5262
		private string space;

		// Token: 0x0400148F RID: 5263
		private string backspace;

		// Token: 0x04001490 RID: 5264
		private string accept;

		// Token: 0x04001491 RID: 5265
		private float cancelWidth;

		// Token: 0x04001492 RID: 5266
		private float spaceWidth;

		// Token: 0x04001493 RID: 5267
		private float backspaceWidth;

		// Token: 0x04001494 RID: 5268
		private float beginWidth;

		// Token: 0x04001495 RID: 5269
		private float optionsWidth;

		// Token: 0x04001496 RID: 5270
		private float boxWidth;

		// Token: 0x04001497 RID: 5271
		private float boxHeight;

		// Token: 0x04001498 RID: 5272
		private float pressedTimer;

		// Token: 0x04001499 RID: 5273
		private float timer;

		// Token: 0x0400149A RID: 5274
		private float ease;

		// Token: 0x0400149B RID: 5275
		private Wiggler wiggler;

		// Token: 0x0400149C RID: 5276
		private static int[] dakuten_able = new int[]
		{
			12363,
			12365,
			12367,
			12369,
			12371,
			12373,
			12375,
			12377,
			12379,
			12381,
			12383,
			12385,
			12388,
			12390,
			12392,
			12399,
			12402,
			12405,
			12408,
			12411,
			12459,
			12461,
			12463,
			12465,
			12467,
			12469,
			12471,
			12473,
			12475,
			12477,
			12479,
			12481,
			12484,
			12486,
			12488,
			12495,
			12498,
			12501,
			12504,
			12507
		};

		// Token: 0x0400149D RID: 5277
		private static int[] handakuten_able = new int[]
		{
			12400,
			12403,
			12406,
			12409,
			12412,
			12496,
			12499,
			12502,
			12505,
			12508
		};

		// Token: 0x0400149E RID: 5278
		private Color unselectColor = Color.LightGray;

		// Token: 0x0400149F RID: 5279
		private Color selectColorA = Calc.HexToColor("84FF54");

		// Token: 0x040014A0 RID: 5280
		private Color selectColorB = Calc.HexToColor("FCFF59");

		// Token: 0x040014A1 RID: 5281
		private Color disableColor = Color.DarkSlateBlue;
	}
}
