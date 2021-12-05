using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000305 RID: 773
	public class FancyText
	{
		// Token: 0x0600181F RID: 6175 RVA: 0x00096396 File Offset: 0x00094596
		public static FancyText.Text Parse(string text, int maxLineWidth, int linesPerPage, float startFade = 1f, Color? defaultColor = null, Language language = null)
		{
			return new FancyText(text, maxLineWidth, linesPerPage, startFade, (defaultColor != null) ? defaultColor.Value : FancyText.DefaultColor, language).Parse();
		}

		// Token: 0x06001820 RID: 6176 RVA: 0x000963C0 File Offset: 0x000945C0
		private FancyText(string text, int maxLineWidth, int linesPerPage, float startFade, Color defaultColor, Language language)
		{
			this.text = text;
			this.maxLineWidth = maxLineWidth;
			this.linesPerPage = ((linesPerPage < 0) ? int.MaxValue : linesPerPage);
			this.startFade = startFade;
			this.currentColor = defaultColor;
			this.defaultColor = defaultColor;
			if (language == null)
			{
				language = Dialog.Language;
			}
			this.language = language;
			this.group.Nodes = new List<FancyText.Node>();
			this.group.Font = (this.font = Fonts.Get(language.FontFace));
			this.group.BaseSize = language.FontFaceSize;
			this.size = this.font.Get(this.group.BaseSize);
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x000964A0 File Offset: 0x000946A0
		private FancyText.Text Parse()
		{
			string[] array = Regex.Split(this.text, this.language.SplitRegex);
			string[] array2 = new string[array.Length];
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					array2[num++] = array[i];
				}
			}
			Stack<Color> stack = new Stack<Color>();
			FancyText.Portrait[] array3 = new FancyText.Portrait[2];
			for (int j = 0; j < num; j++)
			{
				if (array2[j] == "{")
				{
					j++;
					string text = array2[j++];
					List<string> list = new List<string>();
					while (j < array2.Length && array2[j] != "}")
					{
						if (!string.IsNullOrWhiteSpace(array2[j]))
						{
							list.Add(array2[j]);
						}
						j++;
					}
					float duration = 0f;
					if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out duration))
					{
						this.group.Nodes.Add(new FancyText.Wait
						{
							Duration = duration
						});
					}
					else if (text[0] == '#')
					{
						string text2 = "";
						if (text.Length > 1)
						{
							text2 = text.Substring(1);
						}
						else if (list.Count > 0)
						{
							text2 = list[0];
						}
						if (string.IsNullOrEmpty(text2))
						{
							if (stack.Count > 0)
							{
								this.currentColor = stack.Pop();
							}
							else
							{
								this.currentColor = this.defaultColor;
							}
						}
						else
						{
							stack.Push(this.currentColor);
							if (text2 == "red")
							{
								this.currentColor = Color.Red;
							}
							else if (text2 == "green")
							{
								this.currentColor = Color.Green;
							}
							else if (text2 == "blue")
							{
								this.currentColor = Color.Blue;
							}
							else
							{
								this.currentColor = Calc.HexToColor(text2);
							}
						}
					}
					else if (text == "break")
					{
						this.CalcLineWidth();
						this.currentPage++;
						this.group.Pages++;
						this.currentLine = 0;
						this.currentPosition = 0f;
						this.group.Nodes.Add(new FancyText.NewPage());
					}
					else if (text == "n")
					{
						this.AddNewLine();
					}
					else if (text == ">>")
					{
						float num2;
						if (list.Count > 0 && float.TryParse(list[0], NumberStyles.Float, CultureInfo.InvariantCulture, out num2))
						{
							this.currentDelay = 0.01f / num2;
						}
						else
						{
							this.currentDelay = 0.01f;
						}
					}
					else if (text.Equals("/>>"))
					{
						this.currentDelay = 0.01f;
					}
					else if (text.Equals("anchor"))
					{
						FancyText.Anchors position;
						if (Enum.TryParse<FancyText.Anchors>(list[0], true, out position))
						{
							this.group.Nodes.Add(new FancyText.Anchor
							{
								Position = position
							});
						}
					}
					else if (text.Equals("portrait") || text.Equals("left") || text.Equals("right"))
					{
						if (text.Equals("portrait") && list.Count > 0 && list[0].Equals("none"))
						{
							FancyText.Portrait portrait = new FancyText.Portrait();
							this.group.Nodes.Add(portrait);
						}
						else
						{
							FancyText.Portrait portrait;
							if (text.Equals("left"))
							{
								portrait = array3[0];
							}
							else if (text.Equals("right"))
							{
								portrait = array3[1];
							}
							else
							{
								portrait = new FancyText.Portrait();
								foreach (string text3 in list)
								{
									if (text3.Equals("upsidedown"))
									{
										portrait.UpsideDown = true;
									}
									else if (text3.Equals("flip"))
									{
										portrait.Flipped = true;
									}
									else if (text3.Equals("left"))
									{
										portrait.Side = -1;
									}
									else if (text3.Equals("right"))
									{
										portrait.Side = 1;
									}
									else if (text3.Equals("pop"))
									{
										portrait.Pop = true;
									}
									else if (portrait.Sprite == null)
									{
										portrait.Sprite = text3;
									}
									else
									{
										portrait.Animation = text3;
									}
								}
							}
							if (GFX.PortraitsSpriteBank.Has(portrait.SpriteId))
							{
								List<SpriteDataSource> sources = GFX.PortraitsSpriteBank.SpriteData[portrait.SpriteId].Sources;
								for (int k = sources.Count - 1; k >= 0; k--)
								{
									XmlElement xml = sources[k].XML;
									if (xml != null)
									{
										if (portrait.SfxEvent == null)
										{
											portrait.SfxEvent = "event:/char/dialogue/" + xml.Attr("sfx", "");
										}
										if (xml.HasAttr("glitchy"))
										{
											portrait.Glitchy = xml.AttrBool("glitchy", false);
										}
										if (xml.HasChild("sfxs") && portrait.SfxExpression == 1)
										{
											foreach (object obj in xml["sfxs"])
											{
												XmlElement xmlElement = obj as XmlElement;
												if (xmlElement != null && xmlElement.Name.Equals(portrait.Animation, StringComparison.InvariantCultureIgnoreCase))
												{
													portrait.SfxExpression = xmlElement.AttrInt("index");
													break;
												}
											}
										}
									}
								}
							}
							this.group.Nodes.Add(portrait);
							array3[(portrait.Side > 0) ? 1 : 0] = portrait;
						}
					}
					else if (text.Equals("trigger") || text.Equals("silent_trigger"))
					{
						string text4 = "";
						for (int l = 1; l < list.Count; l++)
						{
							text4 = text4 + list[l] + " ";
						}
						int num3;
						if (int.TryParse(list[0], out num3) && num3 >= 0)
						{
							this.group.Nodes.Add(new FancyText.Trigger
							{
								Index = num3,
								Silent = text.StartsWith("silent"),
								Label = text4
							});
						}
					}
					else if (text.Equals("*"))
					{
						this.currentShake = true;
					}
					else if (text.Equals("/*"))
					{
						this.currentShake = false;
					}
					else if (text.Equals("~"))
					{
						this.currentWave = true;
					}
					else if (text.Equals("/~"))
					{
						this.currentWave = false;
					}
					else if (text.Equals("!"))
					{
						this.currentImpact = true;
					}
					else if (text.Equals("/!"))
					{
						this.currentImpact = false;
					}
					else if (text.Equals("%"))
					{
						this.currentMessedUp = true;
					}
					else if (text.Equals("/%"))
					{
						this.currentMessedUp = false;
					}
					else if (text.Equals("big"))
					{
						this.currentScale = 1.5f;
					}
					else if (text.Equals("/big"))
					{
						this.currentScale = 1f;
					}
					else if (text.Equals("s"))
					{
						int num4 = 1;
						if (list.Count > 0)
						{
							int.TryParse(list[0], out num4);
						}
						this.currentPosition += (float)(5 * num4);
					}
					else if (text.Equals("savedata"))
					{
						if (SaveData.Instance == null)
						{
							if (list[0].Equals("name", StringComparison.OrdinalIgnoreCase))
							{
								this.AddWord("Madeline");
							}
							else
							{
								this.AddWord("[SD:" + list[0] + "]");
							}
						}
						else if (list[0].Equals("name", StringComparison.OrdinalIgnoreCase))
						{
							if (!this.language.CanDisplay(SaveData.Instance.Name))
							{
								this.AddWord(Dialog.Clean("FILE_DEFAULT", this.language));
							}
							else
							{
								this.AddWord(SaveData.Instance.Name);
							}
						}
						else
						{
							FieldInfo field = typeof(SaveData).GetField(list[0]);
							this.AddWord(field.GetValue(SaveData.Instance).ToString());
						}
					}
				}
				else
				{
					this.AddWord(array2[j]);
				}
			}
			this.CalcLineWidth();
			return this.group;
		}

		// Token: 0x06001822 RID: 6178 RVA: 0x00096DCC File Offset: 0x00094FCC
		private void CalcLineWidth()
		{
			FancyText.Char @char = null;
			int num = this.group.Nodes.Count - 1;
			while (num >= 0 && @char == null)
			{
				if (this.group.Nodes[num] is FancyText.Char)
				{
					@char = (this.group.Nodes[num] as FancyText.Char);
				}
				else if (this.group.Nodes[num] is FancyText.NewLine || this.group.Nodes[num] is FancyText.NewPage)
				{
					return;
				}
				num--;
			}
			if (@char != null)
			{
				float lineWidth = @char.Position + (float)this.size.Get(@char.Character).XAdvance * @char.Scale;
				@char.LineWidth = lineWidth;
				while (num >= 0 && !(this.group.Nodes[num] is FancyText.NewLine) && !(this.group.Nodes[num] is FancyText.NewPage))
				{
					if (this.group.Nodes[num] is FancyText.Char)
					{
						(this.group.Nodes[num] as FancyText.Char).LineWidth = lineWidth;
					}
					num--;
				}
			}
		}

		// Token: 0x06001823 RID: 6179 RVA: 0x00096F00 File Offset: 0x00095100
		private void AddNewLine()
		{
			this.CalcLineWidth();
			this.currentLine++;
			this.currentPosition = 0f;
			this.group.Lines++;
			if (this.currentLine > this.linesPerPage)
			{
				this.group.Pages++;
				this.currentPage++;
				this.currentLine = 0;
				this.group.Nodes.Add(new FancyText.NewPage());
				return;
			}
			this.group.Nodes.Add(new FancyText.NewLine());
		}

		// Token: 0x06001824 RID: 6180 RVA: 0x00096FA0 File Offset: 0x000951A0
		private void AddWord(string word)
		{
			float num = this.size.Measure(word).X * this.currentScale;
			if (this.currentPosition + num > (float)this.maxLineWidth)
			{
				this.AddNewLine();
			}
			for (int i = 0; i < word.Length; i++)
			{
				if ((this.currentPosition != 0f || word[i] != ' ') && word[i] != '\\')
				{
					PixelFontCharacter pixelFontCharacter = this.size.Get((int)word[i]);
					if (pixelFontCharacter != null)
					{
						float num2 = 0f;
						if (i == word.Length - 1 && (i == 0 || word[i - 1] != '\\'))
						{
							if (this.Contains(this.language.CommaCharacters, word[i]))
							{
								num2 = 0.15f;
							}
							else if (this.Contains(this.language.PeriodCharacters, word[i]))
							{
								num2 = 0.3f;
							}
						}
						List<FancyText.Node> nodes = this.group.Nodes;
						FancyText.Char @char = new FancyText.Char();
						int num3 = this.currentCharIndex;
						this.currentCharIndex = num3 + 1;
						@char.Index = num3;
						@char.Character = (int)word[i];
						@char.Position = this.currentPosition;
						@char.Line = this.currentLine;
						@char.Page = this.currentPage;
						@char.Delay = (this.currentImpact ? 0.0034999999f : (this.currentDelay + num2));
						@char.Color = this.currentColor;
						@char.Scale = this.currentScale;
						@char.Rotation = (this.currentMessedUp ? ((float)Calc.Random.Choose(-1, 1) * Calc.Random.Choose(0.17453292f, 0.34906584f)) : 0f);
						@char.YOffset = (this.currentMessedUp ? ((float)Calc.Random.Choose(-3, -6, 3, 6)) : 0f);
						@char.Fade = this.startFade;
						@char.Shake = this.currentShake;
						@char.Impact = this.currentImpact;
						@char.Wave = this.currentWave;
						@char.IsPunctuation = (this.Contains(this.language.CommaCharacters, word[i]) || this.Contains(this.language.PeriodCharacters, word[i]));
						nodes.Add(@char);
						this.currentPosition += (float)pixelFontCharacter.XAdvance * this.currentScale;
						int num4;
						if (i < word.Length - 1 && pixelFontCharacter.Kerning.TryGetValue((int)word[i], out num4))
						{
							this.currentPosition += (float)num4 * this.currentScale;
						}
					}
				}
			}
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x0009724C File Offset: 0x0009544C
		private bool Contains(string str, char character)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] == character)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040014DF RID: 5343
		public static Color DefaultColor = Color.LightGray;

		// Token: 0x040014E0 RID: 5344
		public const float CharacterDelay = 0.01f;

		// Token: 0x040014E1 RID: 5345
		public const float PeriodDelay = 0.3f;

		// Token: 0x040014E2 RID: 5346
		public const float CommaDelay = 0.15f;

		// Token: 0x040014E3 RID: 5347
		public const float ShakeDistance = 2f;

		// Token: 0x040014E4 RID: 5348
		private Language language;

		// Token: 0x040014E5 RID: 5349
		private string text;

		// Token: 0x040014E6 RID: 5350
		private FancyText.Text group = new FancyText.Text();

		// Token: 0x040014E7 RID: 5351
		private int maxLineWidth;

		// Token: 0x040014E8 RID: 5352
		private int linesPerPage;

		// Token: 0x040014E9 RID: 5353
		private PixelFont font;

		// Token: 0x040014EA RID: 5354
		private PixelFontSize size;

		// Token: 0x040014EB RID: 5355
		private Color defaultColor;

		// Token: 0x040014EC RID: 5356
		private float startFade;

		// Token: 0x040014ED RID: 5357
		private int currentLine;

		// Token: 0x040014EE RID: 5358
		private int currentPage;

		// Token: 0x040014EF RID: 5359
		private float currentPosition;

		// Token: 0x040014F0 RID: 5360
		private Color currentColor;

		// Token: 0x040014F1 RID: 5361
		private float currentScale = 1f;

		// Token: 0x040014F2 RID: 5362
		private float currentDelay = 0.01f;

		// Token: 0x040014F3 RID: 5363
		private bool currentShake;

		// Token: 0x040014F4 RID: 5364
		private bool currentWave;

		// Token: 0x040014F5 RID: 5365
		private bool currentImpact;

		// Token: 0x040014F6 RID: 5366
		private bool currentMessedUp;

		// Token: 0x040014F7 RID: 5367
		private int currentCharIndex;

		// Token: 0x020006CA RID: 1738
		public class Node
		{
		}

		// Token: 0x020006CB RID: 1739
		public class Char : FancyText.Node
		{
			// Token: 0x06002D04 RID: 11524 RVA: 0x0011DBE8 File Offset: 0x0011BDE8
			public void Draw(PixelFont font, float baseSize, Vector2 position, Vector2 scale, float alpha)
			{
				float scaleFactor = (this.Impact ? (2f - this.Fade) : 1f) * this.Scale;
				Vector2 value = Vector2.Zero;
				Vector2 vector = scale * scaleFactor;
				PixelFontSize pixelFontSize = font.Get(baseSize * Math.Max(vector.X, vector.Y));
				PixelFontCharacter pixelFontCharacter = pixelFontSize.Get(this.Character);
				vector *= baseSize / pixelFontSize.Size;
				position.X += this.Position * scale.X;
				value += (this.Shake ? (new Vector2((float)(-1 + Calc.Random.Next(3)), (float)(-1 + Calc.Random.Next(3))) * 2f) : Vector2.Zero);
				value += (this.Wave ? new Vector2(0f, (float)Math.Sin((double)((float)this.Index * 0.25f + Engine.Scene.RawTimeActive * 8f)) * 4f) : Vector2.Zero);
				value.X += (float)pixelFontCharacter.XOffset;
				value.Y += (float)pixelFontCharacter.YOffset + (-8f * (1f - this.Fade) + this.YOffset * this.Fade);
				pixelFontCharacter.Texture.Draw(position + value * vector, Vector2.Zero, this.Color * this.Fade * alpha, vector, this.Rotation);
			}

			// Token: 0x04002C33 RID: 11315
			public int Index;

			// Token: 0x04002C34 RID: 11316
			public int Character;

			// Token: 0x04002C35 RID: 11317
			public float Position;

			// Token: 0x04002C36 RID: 11318
			public int Line;

			// Token: 0x04002C37 RID: 11319
			public int Page;

			// Token: 0x04002C38 RID: 11320
			public float Delay;

			// Token: 0x04002C39 RID: 11321
			public float LineWidth;

			// Token: 0x04002C3A RID: 11322
			public Color Color;

			// Token: 0x04002C3B RID: 11323
			public float Scale;

			// Token: 0x04002C3C RID: 11324
			public float Rotation;

			// Token: 0x04002C3D RID: 11325
			public float YOffset;

			// Token: 0x04002C3E RID: 11326
			public float Fade;

			// Token: 0x04002C3F RID: 11327
			public bool Shake;

			// Token: 0x04002C40 RID: 11328
			public bool Wave;

			// Token: 0x04002C41 RID: 11329
			public bool Impact;

			// Token: 0x04002C42 RID: 11330
			public bool IsPunctuation;
		}

		// Token: 0x020006CC RID: 1740
		public class Portrait : FancyText.Node
		{
			// Token: 0x17000669 RID: 1641
			// (get) Token: 0x06002D06 RID: 11526 RVA: 0x0011DD8F File Offset: 0x0011BF8F
			public string SpriteId
			{
				get
				{
					return "portrait_" + this.Sprite;
				}
			}

			// Token: 0x1700066A RID: 1642
			// (get) Token: 0x06002D07 RID: 11527 RVA: 0x0011DDA1 File Offset: 0x0011BFA1
			public string BeginAnimation
			{
				get
				{
					return "begin_" + this.Animation;
				}
			}

			// Token: 0x1700066B RID: 1643
			// (get) Token: 0x06002D08 RID: 11528 RVA: 0x0011DDB3 File Offset: 0x0011BFB3
			public string IdleAnimation
			{
				get
				{
					return "idle_" + this.Animation;
				}
			}

			// Token: 0x1700066C RID: 1644
			// (get) Token: 0x06002D09 RID: 11529 RVA: 0x0011DDC5 File Offset: 0x0011BFC5
			public string TalkAnimation
			{
				get
				{
					return "talk_" + this.Animation;
				}
			}

			// Token: 0x04002C43 RID: 11331
			public int Side;

			// Token: 0x04002C44 RID: 11332
			public string Sprite;

			// Token: 0x04002C45 RID: 11333
			public string Animation;

			// Token: 0x04002C46 RID: 11334
			public bool UpsideDown;

			// Token: 0x04002C47 RID: 11335
			public bool Flipped;

			// Token: 0x04002C48 RID: 11336
			public bool Pop;

			// Token: 0x04002C49 RID: 11337
			public bool Glitchy;

			// Token: 0x04002C4A RID: 11338
			public string SfxEvent;

			// Token: 0x04002C4B RID: 11339
			public int SfxExpression = 1;
		}

		// Token: 0x020006CD RID: 1741
		public class Wait : FancyText.Node
		{
			// Token: 0x04002C4C RID: 11340
			public float Duration;
		}

		// Token: 0x020006CE RID: 1742
		public class Trigger : FancyText.Node
		{
			// Token: 0x04002C4D RID: 11341
			public int Index;

			// Token: 0x04002C4E RID: 11342
			public bool Silent;

			// Token: 0x04002C4F RID: 11343
			public string Label;
		}

		// Token: 0x020006CF RID: 1743
		public class NewLine : FancyText.Node
		{
		}

		// Token: 0x020006D0 RID: 1744
		public class NewPage : FancyText.Node
		{
		}

		// Token: 0x020006D1 RID: 1745
		public enum Anchors
		{
			// Token: 0x04002C51 RID: 11345
			Top,
			// Token: 0x04002C52 RID: 11346
			Middle,
			// Token: 0x04002C53 RID: 11347
			Bottom
		}

		// Token: 0x020006D2 RID: 1746
		public class Anchor : FancyText.Node
		{
			// Token: 0x04002C54 RID: 11348
			public FancyText.Anchors Position;
		}

		// Token: 0x020006D3 RID: 1747
		public class Text
		{
			// Token: 0x1700066D RID: 1645
			// (get) Token: 0x06002D10 RID: 11536 RVA: 0x0011DDE6 File Offset: 0x0011BFE6
			public int Count
			{
				get
				{
					return this.Nodes.Count;
				}
			}

			// Token: 0x1700066E RID: 1646
			public FancyText.Node this[int index]
			{
				get
				{
					return this.Nodes[index];
				}
			}

			// Token: 0x06002D12 RID: 11538 RVA: 0x0011DE04 File Offset: 0x0011C004
			public int GetCharactersOnPage(int start)
			{
				int num = 0;
				for (int i = start; i < this.Count; i++)
				{
					if (this.Nodes[i] is FancyText.Char)
					{
						num++;
					}
					else if (this.Nodes[i] is FancyText.NewPage)
					{
						break;
					}
				}
				return num;
			}

			// Token: 0x06002D13 RID: 11539 RVA: 0x0011DE54 File Offset: 0x0011C054
			public int GetNextPageStart(int start)
			{
				for (int i = start; i < this.Count; i++)
				{
					if (this.Nodes[i] is FancyText.NewPage)
					{
						return i + 1;
					}
				}
				return this.Nodes.Count;
			}

			// Token: 0x06002D14 RID: 11540 RVA: 0x0011DE94 File Offset: 0x0011C094
			public float WidestLine()
			{
				int num = 0;
				for (int i = 0; i < this.Nodes.Count; i++)
				{
					if (this.Nodes[i] is FancyText.Char)
					{
						num = Math.Max(num, (int)(this.Nodes[i] as FancyText.Char).LineWidth);
					}
				}
				return (float)num;
			}

			// Token: 0x06002D15 RID: 11541 RVA: 0x0011DEEC File Offset: 0x0011C0EC
			public void Draw(Vector2 position, Vector2 justify, Vector2 scale, float alpha, int start = 0, int end = 2147483647)
			{
				int num = Math.Min(this.Nodes.Count, end);
				int num2 = 0;
				float num3 = 0f;
				float num4 = 0f;
				PixelFontSize pixelFontSize = this.Font.Get(this.BaseSize);
				for (int i = start; i < num; i++)
				{
					if (this.Nodes[i] is FancyText.NewLine)
					{
						if (num3 == 0f)
						{
							num3 = 1f;
						}
						num4 += num3;
						num3 = 0f;
					}
					else if (this.Nodes[i] is FancyText.Char)
					{
						num2 = Math.Max(num2, (int)(this.Nodes[i] as FancyText.Char).LineWidth);
						num3 = Math.Max(num3, (this.Nodes[i] as FancyText.Char).Scale);
					}
					else if (this.Nodes[i] is FancyText.NewPage)
					{
						break;
					}
				}
				num4 += num3;
				position -= justify * new Vector2((float)num2, num4 * (float)pixelFontSize.LineHeight) * scale;
				num3 = 0f;
				int num5 = start;
				while (num5 < num && !(this.Nodes[num5] is FancyText.NewPage))
				{
					if (this.Nodes[num5] is FancyText.NewLine)
					{
						if (num3 == 0f)
						{
							num3 = 1f;
						}
						position.Y += (float)pixelFontSize.LineHeight * num3 * scale.Y;
						num3 = 0f;
					}
					if (this.Nodes[num5] is FancyText.Char)
					{
						FancyText.Char @char = this.Nodes[num5] as FancyText.Char;
						@char.Draw(this.Font, this.BaseSize, position, scale, alpha);
						num3 = Math.Max(num3, @char.Scale);
					}
					num5++;
				}
			}

			// Token: 0x06002D16 RID: 11542 RVA: 0x0011E0C8 File Offset: 0x0011C2C8
			public void DrawJustifyPerLine(Vector2 position, Vector2 justify, Vector2 scale, float alpha, int start = 0, int end = 2147483647)
			{
				int num = Math.Min(this.Nodes.Count, end);
				float num2 = 0f;
				float num3 = 0f;
				PixelFontSize pixelFontSize = this.Font.Get(this.BaseSize);
				for (int i = start; i < num; i++)
				{
					if (this.Nodes[i] is FancyText.NewLine)
					{
						if (num2 == 0f)
						{
							num2 = 1f;
						}
						num3 += num2;
						num2 = 0f;
					}
					else if (this.Nodes[i] is FancyText.Char)
					{
						num2 = Math.Max(num2, (this.Nodes[i] as FancyText.Char).Scale);
					}
					else if (this.Nodes[i] is FancyText.NewPage)
					{
						break;
					}
				}
				num3 += num2;
				num2 = 0f;
				int num4 = start;
				while (num4 < num && !(this.Nodes[num4] is FancyText.NewPage))
				{
					if (this.Nodes[num4] is FancyText.NewLine)
					{
						if (num2 == 0f)
						{
							num2 = 1f;
						}
						position.Y += num2 * (float)pixelFontSize.LineHeight * scale.Y;
						num2 = 0f;
					}
					if (this.Nodes[num4] is FancyText.Char)
					{
						FancyText.Char @char = this.Nodes[num4] as FancyText.Char;
						Vector2 value = -justify * new Vector2(@char.LineWidth, num3 * (float)pixelFontSize.LineHeight) * scale;
						@char.Draw(this.Font, this.BaseSize, position + value, scale, alpha);
						num2 = Math.Max(num2, @char.Scale);
					}
					num4++;
				}
			}

			// Token: 0x04002C55 RID: 11349
			public List<FancyText.Node> Nodes;

			// Token: 0x04002C56 RID: 11350
			public int Lines;

			// Token: 0x04002C57 RID: 11351
			public int Pages;

			// Token: 0x04002C58 RID: 11352
			public PixelFont Font;

			// Token: 0x04002C59 RID: 11353
			public float BaseSize;
		}
	}
}
