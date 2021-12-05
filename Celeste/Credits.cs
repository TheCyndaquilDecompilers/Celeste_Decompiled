using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200021B RID: 539
	public class Credits
	{
		// Token: 0x06001170 RID: 4464 RVA: 0x00055D88 File Offset: 0x00053F88
		private static List<Credits.CreditNode> CreateCredits(bool title, bool polaroids)
		{
			List<Credits.CreditNode> list = new List<Credits.CreditNode>();
			if (title)
			{
				list.Add(new Credits.Image("title", 320f));
			}
			list.AddRange(new List<Credits.CreditNode>
			{
				new Credits.Role("Maddy Thorson", new string[]
				{
					"Director",
					"Designer",
					"Writer",
					"Gameplay Coder"
				}),
				new Credits.Role("Noel Berry", new string[]
				{
					"Co-Creator",
					"Programmer",
					"Artist"
				}),
				new Credits.Role("Amora B.", new string[]
				{
					"Concept Artist",
					"High Res Artist"
				}),
				new Credits.Role("Pedro Medeiros", new string[]
				{
					"Pixel Artist",
					"UI Artist"
				}),
				new Credits.Role("Lena Raine", new string[]
				{
					"Composer"
				}),
				new Credits.Team("Power Up Audio", new string[]
				{
					"Kevin Regamey",
					"Jeff Tangsoc",
					"Joey Godard",
					"Cole Verderber"
				}, new string[]
				{
					"Sound Designers"
				})
			});
			if (polaroids)
			{
				list.Add(new Credits.Image(GFX.Portraits, "credits/a", 64f, -0.05f, false));
			}
			list.AddRange(new List<Credits.CreditNode>
			{
				new Credits.Role("Gabby DaRienzo", new string[]
				{
					"3D Artist"
				}),
				new Credits.Role("Sven Bergström", new string[]
				{
					"3D Lighting Artist"
				})
			});
			list.AddRange(new List<Credits.CreditNode>
			{
				new Credits.Thanks("Writing Assistance", new string[]
				{
					"Noel Berry",
					"Amora B.",
					"Greg Lobanov",
					"Lena Raine",
					"Nick Suttner"
				}),
				new Credits.Thanks("Script Editor", new string[]
				{
					"Nick Suttner"
				}),
				new Credits.Thanks("Narrative Consulting", new string[]
				{
					"Silverstring Media",
					"Claris Cyarron",
					"with Lucas JW Johnson",
					"and Tanya Kan"
				})
			});
			list.Add(new Credits.Thanks("Remixers", Credits.Remixers));
			list.Add(new Credits.MultiCredit("Musical Performances", new Credits.MultiCredit.Section[]
			{
				new Credits.MultiCredit.Section("Violin, Viola", new string[]
				{
					"Michaela Nachtigall"
				}),
				new Credits.MultiCredit.Section("Cello", new string[]
				{
					"SungHa Hong"
				}),
				new Credits.MultiCredit.Section("Drums", new string[]
				{
					"Doug Perry"
				})
			}));
			if (polaroids)
			{
				list.Add(new Credits.Image(GFX.Portraits, "credits/b", 64f, 0.05f, false));
			}
			list.Add(new Credits.Thanks("Operations Manager", new string[]
			{
				"Heidy Motta"
			}));
			list.Add(new Credits.Thanks("Porting", new string[]
			{
				"Sickhead Games, LLC",
				"Ethan Lee"
			}));
			list.Add(new Credits.MultiCredit("Localization", new Credits.MultiCredit.Section[]
			{
				new Credits.MultiCredit.Section("French, Italian, Korean, Russian,\nSimplified Chinese, Spanish,\nBrazilian Portuguese, German", new string[]
				{
					"EDS Wordland, Ltd."
				}),
				new Credits.MultiCredit.Section("Japanese", new string[]
				{
					"8-4, Ltd",
					"Keiko Fukuichi",
					"Graeme Howard",
					"John Ricciardi"
				}),
				new Credits.MultiCredit.Section("German, Additional Brazilian Portuguese", new string[]
				{
					"Shloc, Ltd.",
					"Oli Chance",
					"Isabel Sterner",
					"Nadine Leonhardt"
				}),
				new Credits.MultiCredit.Section("Additional Brazilian Portuguese", new string[]
				{
					"Amora B."
				})
			}));
			list.Add(new Credits.Thanks("Contributors & Playtesters", new string[]
			{
				"Nels Anderson",
				"Liam & Graeme Berry",
				"Tamara Bruketta",
				"Allan Defensor",
				"Grayson Evans",
				"Jada Gibbs",
				"Em Halberstadt",
				"Justin Jaffray",
				"Chevy Ray Johnston",
				"Will Lacerda",
				"Myriame Lachapelle",
				"Greg Lobanov",
				"Rafinha Martinelli",
				"Shane Neville",
				"Kyle Pulver",
				"Murphy Pyan",
				"Garret Randell",
				"Kevin Regamey",
				"Atlas Regaudie",
				"Stefano Strapazzon",
				"Nick Suttner",
				"Ryan Thorson",
				"Greg Wohlwend",
				"Justin Yngelmo",
				"baldjared",
				"zep",
				"DevilSquirrel",
				"Covert_Muffin",
				"buhbai",
				"Chaikitty",
				"Msushi",
				"TGH"
			}));
			list.Add(new Credits.Thanks("Community", new string[]
			{
				"Speedrunners & Tool Assisted Speedrunners",
				"Everest Modding Community",
				"The Celeste Discord"
			}));
			list.Add(new Credits.Thanks("Special Thanks", new string[]
			{
				"Fe Angelo",
				"Bruce Berry & Marilyn Firth",
				"Josephine Baird",
				"Liliane Carpinski",
				"Yvonne Hanson",
				"Katherine Elaine Jones",
				"Clint 'halfcoordinated' Lexa",
				"Greg Lobanov",
				"Gabi 'Platy' Madureira",
				"Rodrigo Monteiro",
				"Fernando Piovesan",
				"Paulo Szyszko Pita",
				"Zoe Si",
				"Julie, Richard, & Ryan Thorson",
				"Davey Wreden"
			}));
			if (polaroids)
			{
				list.Add(new Credits.Image(GFX.Portraits, "credits/c", 64f, -0.05f, false));
			}
			list.Add(new Credits.Thanks("Production Kitties", new string[]
			{
				"Jiji, Mr Satan, Peridot",
				"Azzy, Phil, Furiosa",
				"Fred, Bastion, Meredith",
				"Bobbin, Finn"
			}));
			list.AddRange(new List<Credits.CreditNode>
			{
				new Credits.Image(GFX.Misc, "fmod", 0f, 0f, false),
				new Credits.Image(GFX.Misc, "monogame", 0f, 0f, false),
				new Credits.ImageRow(new Credits.Image[]
				{
					new Credits.Image(GFX.Misc, "fna", 0f, 0f, false),
					new Credits.Image(GFX.Misc, "xna", 0f, 0f, false)
				})
			});
			list.Add(new Credits.Break(540f));
			if (polaroids)
			{
				list.Add(new Credits.Image(GFX.Portraits, "credits/d", 0f, 0.05f, true));
			}
			list.Add(new Credits.Ending(Dialog.Clean("CREDITS_THANKYOU", null), !polaroids));
			return list;
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x000564A4 File Offset: 0x000546A4
		public Credits(float alignment = 0.5f, float scale = 1f, bool haveTitle = true, bool havePolaroids = false)
		{
			this.alignment = alignment;
			this.scale = scale;
			this.credits = Credits.CreateCredits(haveTitle, havePolaroids);
			Credits.Font = Dialog.Languages["english"].Font;
			Credits.FontSize = Dialog.Languages["english"].FontFaceSize;
			Credits.LineHeight = (float)Credits.Font.Get(Credits.FontSize).LineHeight;
			this.height = 0f;
			foreach (Credits.CreditNode creditNode in this.credits)
			{
				this.height += creditNode.Height(scale) + 64f * scale;
			}
			this.height += 476f;
			if (havePolaroids)
			{
				this.height -= 280f;
			}
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x000565D4 File Offset: 0x000547D4
		public void Update()
		{
			if (this.Enabled)
			{
				this.scroll += this.scrollSpeed * Engine.DeltaTime * this.scale;
				if (this.scrollDelay <= 0f)
				{
					this.scrollSpeed = Calc.Approach(this.scrollSpeed, 100f * this.AutoScrollSpeedMultiplier, 1800f * Engine.DeltaTime);
				}
				else
				{
					this.scrollDelay -= Engine.DeltaTime;
				}
				if (this.AllowInput)
				{
					if (Input.MenuDown.Check)
					{
						this.scrollDelay = 1f;
						this.scrollSpeed = Calc.Approach(this.scrollSpeed, 600f, 1800f * Engine.DeltaTime);
					}
					else if (Input.MenuUp.Check)
					{
						this.scrollDelay = 1f;
						this.scrollSpeed = Calc.Approach(this.scrollSpeed, -600f, 1800f * Engine.DeltaTime);
					}
					else if (this.scrollDelay > 0f)
					{
						this.scrollSpeed = Calc.Approach(this.scrollSpeed, 0f, 1800f * Engine.DeltaTime);
					}
				}
				if (this.scroll < 0f || this.scroll > this.height)
				{
					this.scrollSpeed = 0f;
				}
				this.scroll = Calc.Clamp(this.scroll, 0f, this.height);
				if (this.scroll >= this.height)
				{
					this.BottomTimer += Engine.DeltaTime;
				}
				else
				{
					this.BottomTimer = 0f;
				}
			}
			this.scrollbarAlpha = Calc.Approach(this.scrollbarAlpha, (this.Enabled && this.scrollDelay > 0f) ? 1f : 0f, Engine.DeltaTime * 2f);
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x000567B0 File Offset: 0x000549B0
		public void Render(Vector2 position)
		{
			Vector2 vector = position + new Vector2(0f, 1080f - this.scroll).Floor();
			foreach (Credits.CreditNode creditNode in this.credits)
			{
				float num = creditNode.Height(this.scale);
				if (vector.Y > -num && vector.Y < 1080f)
				{
					creditNode.Render(vector, this.alignment, this.scale);
				}
				vector.Y += num + 64f * this.scale;
			}
			if (this.scrollbarAlpha > 0f)
			{
				int num2 = 64;
				int num3 = 1080 - num2 * 2;
				float num4 = (float)num3 * ((float)num3 / this.height);
				float num5 = this.scroll / this.height * ((float)num3 - num4);
				Draw.Rect(1844f, (float)num2, 12f, (float)num3, Color.White * 0.2f * this.scrollbarAlpha);
				Draw.Rect(1844f, (float)num2 + num5, 12f, num4, Color.White * 0.5f * this.scrollbarAlpha);
			}
		}

		// Token: 0x04000D01 RID: 3329
		public static string[] Remixers = new string[]
		{
			"Maxo",
			"Ben Prunty",
			"Christa Lee",
			"in love with a ghost",
			"2 Mello",
			"Jukio Kallio",
			"Kuraine",
			"image:matthewseiji"
		};

		// Token: 0x04000D02 RID: 3330
		public static Color BorderColor = Color.Black;

		// Token: 0x04000D03 RID: 3331
		public const float CreditSpacing = 64f;

		// Token: 0x04000D04 RID: 3332
		public const float AutoScrollSpeed = 100f;

		// Token: 0x04000D05 RID: 3333
		public const float InputScrollSpeed = 600f;

		// Token: 0x04000D06 RID: 3334
		public const float ScrollResumeDelay = 1f;

		// Token: 0x04000D07 RID: 3335
		public const float ScrollAcceleration = 1800f;

		// Token: 0x04000D08 RID: 3336
		private List<Credits.CreditNode> credits;

		// Token: 0x04000D09 RID: 3337
		public float AutoScrollSpeedMultiplier = 1f;

		// Token: 0x04000D0A RID: 3338
		private float scrollSpeed = 100f;

		// Token: 0x04000D0B RID: 3339
		private float scroll;

		// Token: 0x04000D0C RID: 3340
		private float height;

		// Token: 0x04000D0D RID: 3341
		private float scrollDelay;

		// Token: 0x04000D0E RID: 3342
		private float scrollbarAlpha;

		// Token: 0x04000D0F RID: 3343
		private float alignment;

		// Token: 0x04000D10 RID: 3344
		private float scale;

		// Token: 0x04000D11 RID: 3345
		public float BottomTimer;

		// Token: 0x04000D12 RID: 3346
		public bool Enabled = true;

		// Token: 0x04000D13 RID: 3347
		public bool AllowInput = true;

		// Token: 0x04000D14 RID: 3348
		public static PixelFont Font;

		// Token: 0x04000D15 RID: 3349
		public static float FontSize;

		// Token: 0x04000D16 RID: 3350
		public static float LineHeight;

		// Token: 0x0200052A RID: 1322
		private abstract class CreditNode
		{
			// Token: 0x06002552 RID: 9554
			public abstract void Render(Vector2 position, float alignment = 0.5f, float scale = 1f);

			// Token: 0x06002553 RID: 9555
			public abstract float Height(float scale = 1f);
		}

		// Token: 0x0200052B RID: 1323
		private class Role : Credits.CreditNode
		{
			// Token: 0x06002555 RID: 9557 RVA: 0x000F8243 File Offset: 0x000F6443
			public Role(string name, params string[] roles)
			{
				this.Name = name;
				this.Roles = string.Join(", ", roles);
			}

			// Token: 0x06002556 RID: 9558 RVA: 0x000F8264 File Offset: 0x000F6464
			public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
			{
				Credits.Font.DrawOutline(Credits.FontSize, this.Name, position.Floor(), new Vector2(alignment, 0f), Vector2.One * 1.8f * scale, Credits.Role.NameColor, 2f, Credits.BorderColor);
				position.Y += (Credits.LineHeight * 1.8f + 8f) * scale;
				Credits.Font.DrawOutline(Credits.FontSize, this.Roles, position.Floor(), new Vector2(alignment, 0f), Vector2.One * 1f * scale, Credits.Role.RolesColor, 2f, Credits.BorderColor);
			}

			// Token: 0x06002557 RID: 9559 RVA: 0x000F8323 File Offset: 0x000F6523
			public override float Height(float scale = 1f)
			{
				return (Credits.LineHeight * 2.8f + 8f + 64f) * scale;
			}

			// Token: 0x04002556 RID: 9558
			public const float NameScale = 1.8f;

			// Token: 0x04002557 RID: 9559
			public const float RolesScale = 1f;

			// Token: 0x04002558 RID: 9560
			public const float Spacing = 8f;

			// Token: 0x04002559 RID: 9561
			public const float BottomSpacing = 64f;

			// Token: 0x0400255A RID: 9562
			public static readonly Color NameColor = Color.White;

			// Token: 0x0400255B RID: 9563
			public static readonly Color RolesColor = Color.White * 0.8f;

			// Token: 0x0400255C RID: 9564
			public string Name;

			// Token: 0x0400255D RID: 9565
			public string Roles;
		}

		// Token: 0x0200052C RID: 1324
		private class Team : Credits.CreditNode
		{
			// Token: 0x06002559 RID: 9561 RVA: 0x000F835E File Offset: 0x000F655E
			public Team(string name, string[] members, params string[] roles)
			{
				this.Name = name;
				this.Members = members;
				this.Roles = string.Join(", ", roles);
			}

			// Token: 0x0600255A RID: 9562 RVA: 0x000F8388 File Offset: 0x000F6588
			public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
			{
				Credits.Font.DrawOutline(Credits.FontSize, this.Name, position.Floor(), new Vector2(alignment, 0f), Vector2.One * 1.8f * scale, Credits.Role.NameColor, 2f, Credits.BorderColor);
				position.Y += (Credits.LineHeight * 1.8f + 8f) * scale;
				for (int i = 0; i < this.Members.Length; i++)
				{
					Credits.Font.DrawOutline(Credits.FontSize, this.Members[i], position.Floor(), new Vector2(alignment, 0f), Vector2.One * 1.4f * scale, Credits.Team.TeamColor, 2f, Credits.BorderColor);
					position.Y += Credits.LineHeight * 1.4f * scale;
				}
				Credits.Font.DrawOutline(Credits.FontSize, this.Roles, position.Floor(), new Vector2(alignment, 0f), Vector2.One * 1f * scale, Credits.Role.RolesColor, 2f, Credits.BorderColor);
			}

			// Token: 0x0600255B RID: 9563 RVA: 0x000F84BE File Offset: 0x000F66BE
			public override float Height(float scale = 1f)
			{
				return (Credits.LineHeight * (1.8f + (float)this.Members.Length * 1.4f + 1f) + 8f + 64f) * scale;
			}

			// Token: 0x0400255E RID: 9566
			public const float TeamScale = 1.4f;

			// Token: 0x0400255F RID: 9567
			public static readonly Color TeamColor = Color.White;

			// Token: 0x04002560 RID: 9568
			public string Name;

			// Token: 0x04002561 RID: 9569
			public string[] Members;

			// Token: 0x04002562 RID: 9570
			public string Roles;
		}

		// Token: 0x0200052D RID: 1325
		private class Thanks : Credits.CreditNode
		{
			// Token: 0x0600255D RID: 9565 RVA: 0x000F84FB File Offset: 0x000F66FB
			public Thanks(string title, params string[] to) : this(0, title, to)
			{
			}

			// Token: 0x0600255E RID: 9566 RVA: 0x000F8508 File Offset: 0x000F6708
			public Thanks(int topPadding, string title, params string[] to)
			{
				this.TopPadding = topPadding;
				this.Title = title;
				this.Credits = to;
				this.linkedImages = new string[this.Credits.Length];
				for (int i = 0; i < this.linkedImages.Length; i++)
				{
					this.linkedImages[i] = null;
					if (this.Credits[i].StartsWith("image:"))
					{
						this.linkedImages[i] = this.Credits[i].Substring(6);
					}
				}
			}

			// Token: 0x0600255F RID: 9567 RVA: 0x000F85AC File Offset: 0x000F67AC
			public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
			{
				position.Y += (float)this.TopPadding * scale;
				Celeste.Credits.Font.DrawOutline(Celeste.Credits.FontSize, this.Title, position.Floor(), new Vector2(alignment, 0f), Vector2.One * 1.4f * scale, this.TitleColor, 2f, Celeste.Credits.BorderColor);
				position.Y += (Celeste.Credits.LineHeight * 1.4f + 8f) * scale;
				for (int i = 0; i < this.Credits.Length; i++)
				{
					if (this.linkedImages[i] != null)
					{
						MTexture mtexture = GFX.Gui[this.linkedImages[i]];
						mtexture.DrawJustified(position.Floor() + new Vector2(0f, -2f), new Vector2(alignment, 0f), Celeste.Credits.BorderColor, 1.15f * scale, 0f);
						mtexture.DrawJustified(position.Floor() + new Vector2(0f, 2f), new Vector2(alignment, 0f), Celeste.Credits.BorderColor, 1.15f * scale, 0f);
						mtexture.DrawJustified(position.Floor() + new Vector2(-2f, 0f), new Vector2(alignment, 0f), Celeste.Credits.BorderColor, 1.15f * scale, 0f);
						mtexture.DrawJustified(position.Floor() + new Vector2(2f, 0f), new Vector2(alignment, 0f), Celeste.Credits.BorderColor, 1.15f * scale, 0f);
						mtexture.DrawJustified(position.Floor(), new Vector2(alignment, 0f), this.CreditsColor, 1.15f * scale, 0f);
						position.Y += (float)mtexture.Height * 1.15f * scale;
					}
					else
					{
						Celeste.Credits.Font.DrawOutline(Celeste.Credits.FontSize, this.Credits[i], position.Floor(), new Vector2(alignment, 0f), Vector2.One * 1.15f * scale, this.CreditsColor, 2f, Celeste.Credits.BorderColor);
						position.Y += Celeste.Credits.LineHeight * 1.15f * scale;
					}
				}
			}

			// Token: 0x06002560 RID: 9568 RVA: 0x000F8809 File Offset: 0x000F6A09
			public override float Height(float scale = 1f)
			{
				return (Celeste.Credits.LineHeight * (1.4f + (float)this.Credits.Length * 1.15f) + ((this.Credits.Length != 0) ? 8f : 0f) + (float)this.TopPadding) * scale;
			}

			// Token: 0x04002563 RID: 9571
			public const float TitleScale = 1.4f;

			// Token: 0x04002564 RID: 9572
			public const float CreditsScale = 1.15f;

			// Token: 0x04002565 RID: 9573
			public const float Spacing = 8f;

			// Token: 0x04002566 RID: 9574
			public readonly Color TitleColor = Color.White;

			// Token: 0x04002567 RID: 9575
			public readonly Color CreditsColor = Color.White * 0.8f;

			// Token: 0x04002568 RID: 9576
			public int TopPadding;

			// Token: 0x04002569 RID: 9577
			public string Title;

			// Token: 0x0400256A RID: 9578
			public string[] Credits;

			// Token: 0x0400256B RID: 9579
			private string[] linkedImages;
		}

		// Token: 0x0200052E RID: 1326
		private class MultiCredit : Credits.CreditNode
		{
			// Token: 0x06002561 RID: 9569 RVA: 0x000F8846 File Offset: 0x000F6A46
			public MultiCredit(string title, params Credits.MultiCredit.Section[] to) : this(0, title, to)
			{
			}

			// Token: 0x06002562 RID: 9570 RVA: 0x000F8854 File Offset: 0x000F6A54
			public MultiCredit(int topPadding, string title, Credits.MultiCredit.Section[] to)
			{
				this.TopPadding = topPadding;
				this.Title = title;
				this.Sections = to;
			}

			// Token: 0x06002563 RID: 9571 RVA: 0x000F88AC File Offset: 0x000F6AAC
			public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
			{
				position.Y += (float)this.TopPadding * scale;
				Credits.Font.DrawOutline(Credits.FontSize, this.Title, position.Floor(), new Vector2(alignment, 0f), Vector2.One * 1.4f * scale, this.TitleColor, 2f, Credits.BorderColor);
				position.Y += (Credits.LineHeight * 1.4f + 8f) * scale;
				for (int i = 0; i < this.Sections.Length; i++)
				{
					Credits.MultiCredit.Section section = this.Sections[i];
					string subtitle = section.Subtitle;
					Credits.Font.DrawOutline(Credits.FontSize, section.Subtitle, position.Floor(), new Vector2(alignment, 0f), Vector2.One * 0.7f * scale, this.SubtitleColor, 2f, Credits.BorderColor);
					position.Y += (float)section.SubtitleLines * Credits.LineHeight * 0.7f * scale;
					for (int j = 0; j < section.Credits.Length; j++)
					{
						Credits.Font.DrawOutline(Credits.FontSize, section.Credits[j], position.Floor(), new Vector2(alignment, 0f), Vector2.One * 1.15f * scale, this.CreditsColor, 2f, Credits.BorderColor);
						position.Y += Credits.LineHeight * 1.15f * scale;
					}
					position.Y += 32f * scale;
				}
			}

			// Token: 0x06002564 RID: 9572 RVA: 0x000F8A54 File Offset: 0x000F6C54
			public override float Height(float scale = 1f)
			{
				float num = 0f;
				num += (float)this.TopPadding;
				num += Credits.LineHeight * 1.4f + 8f;
				for (int i = 0; i < this.Sections.Length; i++)
				{
					num += (float)this.Sections[i].SubtitleLines * Credits.LineHeight * 0.7f;
					num += Credits.LineHeight * 1.15f * (float)this.Sections[i].Credits.Length;
				}
				num += 32f * (float)(this.Sections.Length - 1);
				return num * scale;
			}

			// Token: 0x0400256C RID: 9580
			public const float TitleScale = 1.4f;

			// Token: 0x0400256D RID: 9581
			public const float SubtitleScale = 0.7f;

			// Token: 0x0400256E RID: 9582
			public const float CreditsScale = 1.15f;

			// Token: 0x0400256F RID: 9583
			public const float Spacing = 8f;

			// Token: 0x04002570 RID: 9584
			public const float SectionSpacing = 32f;

			// Token: 0x04002571 RID: 9585
			public readonly Color TitleColor = Color.White;

			// Token: 0x04002572 RID: 9586
			public readonly Color SubtitleColor = Calc.HexToColor("a8a694");

			// Token: 0x04002573 RID: 9587
			public readonly Color CreditsColor = Color.White * 0.8f;

			// Token: 0x04002574 RID: 9588
			public int TopPadding;

			// Token: 0x04002575 RID: 9589
			public string Title;

			// Token: 0x04002576 RID: 9590
			public Credits.MultiCredit.Section[] Sections;

			// Token: 0x02000790 RID: 1936
			public class Section
			{
				// Token: 0x06002FF9 RID: 12281 RVA: 0x0012CEE8 File Offset: 0x0012B0E8
				public Section(string subtitle, params string[] credits)
				{
					this.Subtitle = subtitle.ToUpper();
					this.SubtitleLines = subtitle.Split(new char[]
					{
						'\n'
					}).Length;
					this.Credits = credits;
				}

				// Token: 0x04002F9E RID: 12190
				public string Subtitle;

				// Token: 0x04002F9F RID: 12191
				public int SubtitleLines;

				// Token: 0x04002FA0 RID: 12192
				public string[] Credits;
			}
		}

		// Token: 0x0200052F RID: 1327
		private class Ending : Credits.CreditNode
		{
			// Token: 0x06002565 RID: 9573 RVA: 0x000F8AEB File Offset: 0x000F6CEB
			public Ending(string text, bool spacing)
			{
				this.Text = text;
				this.Spacing = spacing;
			}

			// Token: 0x06002566 RID: 9574 RVA: 0x000F8B04 File Offset: 0x000F6D04
			public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
			{
				if (this.Spacing)
				{
					position.Y += 540f;
				}
				else
				{
					position.Y += ActiveFont.LineHeight * 1.5f * scale * 0.5f;
				}
				ActiveFont.DrawOutline(this.Text, new Vector2(960f, position.Y), new Vector2(0.5f, 0.5f), Vector2.One * 1.5f * scale, Color.White, 2f, Credits.BorderColor);
			}

			// Token: 0x06002567 RID: 9575 RVA: 0x000F8B97 File Offset: 0x000F6D97
			public override float Height(float scale = 1f)
			{
				if (this.Spacing)
				{
					return 540f;
				}
				return ActiveFont.LineHeight * 1.5f * scale;
			}

			// Token: 0x04002577 RID: 9591
			public string Text;

			// Token: 0x04002578 RID: 9592
			public bool Spacing;
		}

		// Token: 0x02000530 RID: 1328
		private class Image : Credits.CreditNode
		{
			// Token: 0x06002568 RID: 9576 RVA: 0x000F8BB4 File Offset: 0x000F6DB4
			public Image(string path, float bottomPadding = 0f) : this(GFX.Gui, path, bottomPadding, 0f, false)
			{
			}

			// Token: 0x06002569 RID: 9577 RVA: 0x000F8BC9 File Offset: 0x000F6DC9
			public Image(Atlas atlas, string path, float bottomPadding = 0f, float rotation = 0f, bool screenCenter = false)
			{
				this.Atlas = atlas;
				this.ImagePath = path;
				this.BottomPadding = bottomPadding;
				this.Rotation = rotation;
				this.ScreenCenter = screenCenter;
			}

			// Token: 0x0600256A RID: 9578 RVA: 0x000F8BF8 File Offset: 0x000F6DF8
			public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
			{
				MTexture mtexture = this.Atlas[this.ImagePath];
				Vector2 position2 = position + new Vector2((float)mtexture.Width * (0.5f - alignment), (float)mtexture.Height * 0.5f) * scale;
				if (this.ScreenCenter)
				{
					position2.X = 960f;
				}
				mtexture.DrawCentered(position2, Color.White, scale, this.Rotation);
			}

			// Token: 0x0600256B RID: 9579 RVA: 0x000F8C6C File Offset: 0x000F6E6C
			public override float Height(float scale = 1f)
			{
				return ((float)this.Atlas[this.ImagePath].Height + this.BottomPadding) * scale;
			}

			// Token: 0x04002579 RID: 9593
			public Atlas Atlas;

			// Token: 0x0400257A RID: 9594
			public string ImagePath;

			// Token: 0x0400257B RID: 9595
			public float BottomPadding;

			// Token: 0x0400257C RID: 9596
			public float Rotation;

			// Token: 0x0400257D RID: 9597
			public bool ScreenCenter;
		}

		// Token: 0x02000531 RID: 1329
		private class ImageRow : Credits.CreditNode
		{
			// Token: 0x0600256C RID: 9580 RVA: 0x000F8C8E File Offset: 0x000F6E8E
			public ImageRow(params Credits.Image[] images)
			{
				this.images = images;
			}

			// Token: 0x0600256D RID: 9581 RVA: 0x000F8CA0 File Offset: 0x000F6EA0
			public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
			{
				float num = this.Height(scale);
				float num2 = 0f;
				foreach (Credits.Image image in this.images)
				{
					num2 += (float)(image.Atlas[image.ImagePath].Width + 32) * scale;
				}
				num2 -= 32f * scale;
				Vector2 value = position - new Vector2(alignment * num2, 0f);
				foreach (Credits.Image image2 in this.images)
				{
					image2.Render(value + new Vector2(0f, (num - image2.Height(scale)) / 2f), 0f, scale);
					value.X += (float)(image2.Atlas[image2.ImagePath].Width + 32) * scale;
				}
			}

			// Token: 0x0600256E RID: 9582 RVA: 0x000F8D90 File Offset: 0x000F6F90
			public override float Height(float scale = 1f)
			{
				float num = 0f;
				foreach (Credits.Image image in this.images)
				{
					if (image.Height(scale) > num)
					{
						num = image.Height(scale);
					}
				}
				return num;
			}

			// Token: 0x0400257E RID: 9598
			private Credits.Image[] images;
		}

		// Token: 0x02000532 RID: 1330
		private class Break : Credits.CreditNode
		{
			// Token: 0x0600256F RID: 9583 RVA: 0x000F8DCF File Offset: 0x000F6FCF
			public Break(float size = 64f)
			{
				this.Size = size;
			}

			// Token: 0x06002570 RID: 9584 RVA: 0x000091E2 File Offset: 0x000073E2
			public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
			{
			}

			// Token: 0x06002571 RID: 9585 RVA: 0x000F8DDE File Offset: 0x000F6FDE
			public override float Height(float scale = 1f)
			{
				return this.Size * scale;
			}

			// Token: 0x0400257F RID: 9599
			public float Size;
		}
	}
}
