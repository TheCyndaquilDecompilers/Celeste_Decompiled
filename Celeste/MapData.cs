using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200036E RID: 878
	public class MapData
	{
		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06001B85 RID: 7045 RVA: 0x000B417E File Offset: 0x000B237E
		public string Filename
		{
			get
			{
				return this.Data.Mode[(int)this.Area.Mode].Path;
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06001B86 RID: 7046 RVA: 0x000B419C File Offset: 0x000B239C
		public string Filepath
		{
			get
			{
				return Path.Combine(Engine.ContentDirectory, "Maps", this.Filename + ".bin");
			}
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06001B87 RID: 7047 RVA: 0x000B41C0 File Offset: 0x000B23C0
		public Rectangle TileBounds
		{
			get
			{
				return new Rectangle(this.Bounds.X / 8, this.Bounds.Y / 8, (int)Math.Ceiling((double)((float)this.Bounds.Width / 8f)), (int)Math.Ceiling((double)((float)this.Bounds.Height / 8f)));
			}
		}

		// Token: 0x06001B88 RID: 7048 RVA: 0x000B4220 File Offset: 0x000B2420
		public MapData(AreaKey area)
		{
			this.Area = area;
			this.Data = AreaData.Areas[this.Area.ID];
			this.ModeData = this.Data.Mode[(int)this.Area.Mode];
			this.Load();
		}

		// Token: 0x06001B89 RID: 7049 RVA: 0x000B42AF File Offset: 0x000B24AF
		public LevelData GetTransitionTarget(Level level, Vector2 position)
		{
			return this.GetAt(position);
		}

		// Token: 0x06001B8A RID: 7050 RVA: 0x000B42B8 File Offset: 0x000B24B8
		public bool CanTransitionTo(Level level, Vector2 position)
		{
			LevelData transitionTarget = this.GetTransitionTarget(level, position);
			return transitionTarget != null && !transitionTarget.Dummy;
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06001B8B RID: 7051 RVA: 0x000B42DC File Offset: 0x000B24DC
		public int LoadSeed
		{
			get
			{
				int num = 0;
				foreach (char c in this.Data.Name)
				{
					num += (int)c;
				}
				return num;
			}
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06001B8C RID: 7052 RVA: 0x000B4318 File Offset: 0x000B2518
		public int LevelCount
		{
			get
			{
				int num = 0;
				using (List<LevelData>.Enumerator enumerator = this.Levels.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.Dummy)
						{
							num++;
						}
					}
				}
				return num;
			}
		}

		// Token: 0x06001B8D RID: 7053 RVA: 0x000B4374 File Offset: 0x000B2574
		public void Reload()
		{
			this.Load();
		}

		// Token: 0x06001B8E RID: 7054 RVA: 0x000B437C File Offset: 0x000B257C
		private void Load()
		{
			if (!File.Exists(this.Filepath))
			{
				return;
			}
			this.Strawberries = new List<EntityData>();
			BinaryPacker.Element element = BinaryPacker.FromBinary(this.Filepath);
			if (!element.Package.Equals(this.ModeData.Path))
			{
				throw new Exception("Corrupted Level Data");
			}
			foreach (BinaryPacker.Element element2 in element.Children)
			{
				if (element2.Name == "levels")
				{
					this.Levels = new List<LevelData>();
					using (List<BinaryPacker.Element>.Enumerator enumerator2 = element2.Children.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							BinaryPacker.Element data = enumerator2.Current;
							LevelData levelData = new LevelData(data);
							this.DetectedStrawberries += levelData.Strawberries;
							if (levelData.HasGem)
							{
								this.DetectedRemixNotes = true;
							}
							if (levelData.HasHeartGem)
							{
								this.DetectedHeartGem = true;
							}
							this.Levels.Add(levelData);
						}
						continue;
					}
				}
				if (element2.Name == "Filler")
				{
					this.Filler = new List<Rectangle>();
					if (element2.Children == null)
					{
						continue;
					}
					using (List<BinaryPacker.Element>.Enumerator enumerator2 = element2.Children.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							BinaryPacker.Element element3 = enumerator2.Current;
							this.Filler.Add(new Rectangle((int)element3.Attributes["x"], (int)element3.Attributes["y"], (int)element3.Attributes["w"], (int)element3.Attributes["h"]));
						}
						continue;
					}
				}
				if (element2.Name == "Style")
				{
					if (element2.HasAttr("color"))
					{
						this.BackgroundColor = Calc.HexToColor(element2.Attr("color", ""));
					}
					if (element2.Children != null)
					{
						foreach (BinaryPacker.Element element4 in element2.Children)
						{
							if (element4.Name == "Backgrounds")
							{
								this.Background = element4;
							}
							else if (element4.Name == "Foregrounds")
							{
								this.Foreground = element4;
							}
						}
					}
				}
			}
			foreach (LevelData levelData2 in this.Levels)
			{
				foreach (EntityData entityData in levelData2.Entities)
				{
					if (entityData.Name == "strawberry")
					{
						this.Strawberries.Add(entityData);
					}
					else if (entityData.Name == "goldenBerry")
					{
						this.Goldenberries.Add(entityData);
					}
				}
			}
			int num = int.MaxValue;
			int num2 = int.MaxValue;
			int num3 = int.MinValue;
			int num4 = int.MinValue;
			foreach (LevelData levelData3 in this.Levels)
			{
				if (levelData3.Bounds.Left < num)
				{
					num = levelData3.Bounds.Left;
				}
				if (levelData3.Bounds.Top < num2)
				{
					num2 = levelData3.Bounds.Top;
				}
				if (levelData3.Bounds.Right > num3)
				{
					num3 = levelData3.Bounds.Right;
				}
				if (levelData3.Bounds.Bottom > num4)
				{
					num4 = levelData3.Bounds.Bottom;
				}
			}
			foreach (Rectangle rectangle in this.Filler)
			{
				if (rectangle.Left < num)
				{
					num = rectangle.Left;
				}
				if (rectangle.Top < num2)
				{
					num2 = rectangle.Top;
				}
				if (rectangle.Right > num3)
				{
					num3 = rectangle.Right;
				}
				if (rectangle.Bottom > num4)
				{
					num4 = rectangle.Bottom;
				}
			}
			int num5 = 64;
			this.Bounds = new Rectangle(num - num5, num2 - num5, num3 - num + num5 * 2, num4 - num2 + num5 * 2);
			this.ModeData.TotalStrawberries = 0;
			this.ModeData.StartStrawberries = 0;
			this.ModeData.StrawberriesByCheckpoint = new EntityData[10, 25];
			int num6 = 0;
			while (this.ModeData.Checkpoints != null && num6 < this.ModeData.Checkpoints.Length)
			{
				if (this.ModeData.Checkpoints[num6] != null)
				{
					this.ModeData.Checkpoints[num6].Strawberries = 0;
				}
				num6++;
			}
			foreach (EntityData entityData2 in this.Strawberries)
			{
				if (!entityData2.Bool("moon", false))
				{
					int num7 = entityData2.Int("checkpointID", 0);
					int num8 = entityData2.Int("order", 0);
					if (this.ModeData.StrawberriesByCheckpoint[num7, num8] == null)
					{
						this.ModeData.StrawberriesByCheckpoint[num7, num8] = entityData2;
					}
					if (num7 == 0)
					{
						this.ModeData.StartStrawberries++;
					}
					else if (this.ModeData.Checkpoints != null)
					{
						this.ModeData.Checkpoints[num7 - 1].Strawberries++;
					}
					this.ModeData.TotalStrawberries++;
				}
			}
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x000B4A64 File Offset: 0x000B2C64
		public int[] GetStrawberries(out int total)
		{
			total = 0;
			int[] array = new int[10];
			foreach (LevelData levelData in this.Levels)
			{
				foreach (EntityData entityData in levelData.Entities)
				{
					if (entityData.Name == "strawberry")
					{
						total++;
						array[entityData.Int("checkpointID", 0)]++;
					}
				}
			}
			return array;
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x000B4B24 File Offset: 0x000B2D24
		public LevelData StartLevel()
		{
			return this.GetAt(Vector2.Zero);
		}

		// Token: 0x06001B91 RID: 7057 RVA: 0x000B4B34 File Offset: 0x000B2D34
		public LevelData GetAt(Vector2 at)
		{
			foreach (LevelData levelData in this.Levels)
			{
				if (levelData.Check(at))
				{
					return levelData;
				}
			}
			return null;
		}

		// Token: 0x06001B92 RID: 7058 RVA: 0x000B4B90 File Offset: 0x000B2D90
		public LevelData Get(string levelName)
		{
			foreach (LevelData levelData in this.Levels)
			{
				if (levelData.Name.Equals(levelName))
				{
					return levelData;
				}
			}
			return null;
		}

		// Token: 0x06001B93 RID: 7059 RVA: 0x000B4BF4 File Offset: 0x000B2DF4
		public List<Backdrop> CreateBackdrops(BinaryPacker.Element data)
		{
			List<Backdrop> list = new List<Backdrop>();
			if (data != null && data.Children != null)
			{
				foreach (BinaryPacker.Element element in data.Children)
				{
					if (element.Name.Equals("apply", StringComparison.OrdinalIgnoreCase))
					{
						if (element.Children == null)
						{
							continue;
						}
						using (List<BinaryPacker.Element>.Enumerator enumerator2 = element.Children.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								BinaryPacker.Element child = enumerator2.Current;
								list.Add(this.ParseBackdrop(child, element));
							}
							continue;
						}
					}
					list.Add(this.ParseBackdrop(element, null));
				}
			}
			return list;
		}

		// Token: 0x06001B94 RID: 7060 RVA: 0x000B4CCC File Offset: 0x000B2ECC
		private Backdrop ParseBackdrop(BinaryPacker.Element child, BinaryPacker.Element above)
		{
			Backdrop backdrop;
			if (child.Name.Equals("parallax", StringComparison.OrdinalIgnoreCase))
			{
				string id = child.Attr("texture", "");
				string a = child.Attr("atlas", "game");
				MTexture texture;
				if (a == "game" && GFX.Game.Has(id))
				{
					texture = GFX.Game[id];
				}
				else if (a == "gui" && GFX.Gui.Has(id))
				{
					texture = GFX.Gui[id];
				}
				else
				{
					texture = GFX.Misc[id];
				}
				Parallax parallax = new Parallax(texture);
				backdrop = parallax;
				string text = "";
				if (child.HasAttr("blendmode"))
				{
					text = child.Attr("blendmode", "alphablend").ToLower();
				}
				else if (above != null && above.HasAttr("blendmode"))
				{
					text = above.Attr("blendmode", "alphablend").ToLower();
				}
				if (text.Equals("additive"))
				{
					parallax.BlendState = BlendState.Additive;
				}
				parallax.DoFadeIn = bool.Parse(child.Attr("fadeIn", "false"));
			}
			else if (child.Name.Equals("snowfg", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new Snow(true);
			}
			else if (child.Name.Equals("snowbg", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new Snow(false);
			}
			else if (child.Name.Equals("windsnow", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new WindSnowFG();
			}
			else if (child.Name.Equals("dreamstars", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new DreamStars();
			}
			else if (child.Name.Equals("stars", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new StarsBG();
			}
			else if (child.Name.Equals("mirrorfg", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new MirrorFG();
			}
			else if (child.Name.Equals("reflectionfg", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new ReflectionFG();
			}
			else if (child.Name.Equals("godrays", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new Godrays();
			}
			else if (child.Name.Equals("tentacles", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new Tentacles((Tentacles.Side)Enum.Parse(typeof(Tentacles.Side), child.Attr("side", "Right")), Calc.HexToColor(child.Attr("color", "")), child.AttrFloat("offset", 0f));
			}
			else if (child.Name.Equals("northernlights", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new NorthernLights();
			}
			else if (child.Name.Equals("bossStarField", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new FinalBossStarfield();
			}
			else if (child.Name.Equals("petals", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new Petals();
			}
			else if (child.Name.Equals("heatwave", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new HeatWave();
			}
			else if (child.Name.Equals("corestarsfg", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new CoreStarsFG();
			}
			else if (child.Name.Equals("starfield", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new Starfield(Calc.HexToColor(child.Attr("color", "")), child.AttrFloat("speed", 1f));
			}
			else if (child.Name.Equals("planets", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new Planets((int)child.AttrFloat("count", 32f), child.Attr("size", "small"));
			}
			else if (child.Name.Equals("rain", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new RainFG();
			}
			else if (child.Name.Equals("stardust", StringComparison.OrdinalIgnoreCase))
			{
				backdrop = new StardustFG();
			}
			else
			{
				if (!child.Name.Equals("blackhole", StringComparison.OrdinalIgnoreCase))
				{
					throw new Exception("Background type " + child.Name + " does not exist");
				}
				backdrop = new BlackholeBG();
			}
			if (child.HasAttr("tag"))
			{
				backdrop.Tags.Add(child.Attr("tag", ""));
			}
			if (above != null && above.HasAttr("tag"))
			{
				backdrop.Tags.Add(above.Attr("tag", ""));
			}
			if (child.HasAttr("x"))
			{
				backdrop.Position.X = child.AttrFloat("x", 0f);
			}
			else if (above != null && above.HasAttr("x"))
			{
				backdrop.Position.X = above.AttrFloat("x", 0f);
			}
			if (child.HasAttr("y"))
			{
				backdrop.Position.Y = child.AttrFloat("y", 0f);
			}
			else if (above != null && above.HasAttr("y"))
			{
				backdrop.Position.Y = above.AttrFloat("y", 0f);
			}
			if (child.HasAttr("scrollx"))
			{
				backdrop.Scroll.X = child.AttrFloat("scrollx", 0f);
			}
			else if (above != null && above.HasAttr("scrollx"))
			{
				backdrop.Scroll.X = above.AttrFloat("scrollx", 0f);
			}
			if (child.HasAttr("scrolly"))
			{
				backdrop.Scroll.Y = child.AttrFloat("scrolly", 0f);
			}
			else if (above != null && above.HasAttr("scrolly"))
			{
				backdrop.Scroll.Y = above.AttrFloat("scrolly", 0f);
			}
			if (child.HasAttr("speedx"))
			{
				backdrop.Speed.X = child.AttrFloat("speedx", 0f);
			}
			else if (above != null && above.HasAttr("speedx"))
			{
				backdrop.Speed.X = above.AttrFloat("speedx", 0f);
			}
			if (child.HasAttr("speedy"))
			{
				backdrop.Speed.Y = child.AttrFloat("speedy", 0f);
			}
			else if (above != null && above.HasAttr("speedy"))
			{
				backdrop.Speed.Y = above.AttrFloat("speedy", 0f);
			}
			backdrop.Color = Color.White;
			if (child.HasAttr("color"))
			{
				backdrop.Color = Calc.HexToColor(child.Attr("color", ""));
			}
			else if (above != null && above.HasAttr("color"))
			{
				backdrop.Color = Calc.HexToColor(above.Attr("color", ""));
			}
			if (child.HasAttr("alpha"))
			{
				backdrop.Color *= child.AttrFloat("alpha", 0f);
			}
			else if (above != null && above.HasAttr("alpha"))
			{
				backdrop.Color *= above.AttrFloat("alpha", 0f);
			}
			if (child.HasAttr("flipx"))
			{
				backdrop.FlipX = child.AttrBool("flipx", false);
			}
			else if (above != null && above.HasAttr("flipx"))
			{
				backdrop.FlipX = above.AttrBool("flipx", false);
			}
			if (child.HasAttr("flipy"))
			{
				backdrop.FlipY = child.AttrBool("flipy", false);
			}
			else if (above != null && above.HasAttr("flipy"))
			{
				backdrop.FlipY = above.AttrBool("flipy", false);
			}
			if (child.HasAttr("loopx"))
			{
				backdrop.LoopX = child.AttrBool("loopx", false);
			}
			else if (above != null && above.HasAttr("loopx"))
			{
				backdrop.LoopX = above.AttrBool("loopx", false);
			}
			if (child.HasAttr("loopy"))
			{
				backdrop.LoopY = child.AttrBool("loopy", false);
			}
			else if (above != null && above.HasAttr("loopy"))
			{
				backdrop.LoopY = above.AttrBool("loopy", false);
			}
			if (child.HasAttr("wind"))
			{
				backdrop.WindMultiplier = child.AttrFloat("wind", 0f);
			}
			else if (above != null && above.HasAttr("wind"))
			{
				backdrop.WindMultiplier = above.AttrFloat("wind", 0f);
			}
			string text2 = null;
			if (child.HasAttr("exclude"))
			{
				text2 = child.Attr("exclude", "");
			}
			else if (above != null && above.HasAttr("exclude"))
			{
				text2 = above.Attr("exclude", "");
			}
			if (text2 != null)
			{
				backdrop.ExcludeFrom = this.ParseLevelsList(text2);
			}
			string text3 = null;
			if (child.HasAttr("only"))
			{
				text3 = child.Attr("only", "");
			}
			else if (above != null && above.HasAttr("only"))
			{
				text3 = above.Attr("only", "");
			}
			if (text3 != null)
			{
				backdrop.OnlyIn = this.ParseLevelsList(text3);
			}
			string text4 = null;
			if (child.HasAttr("flag"))
			{
				text4 = child.Attr("flag", "");
			}
			else if (above != null && above.HasAttr("flag"))
			{
				text4 = above.Attr("flag", "");
			}
			if (text4 != null)
			{
				backdrop.OnlyIfFlag = text4;
			}
			string text5 = null;
			if (child.HasAttr("notflag"))
			{
				text5 = child.Attr("notflag", "");
			}
			else if (above != null && above.HasAttr("notflag"))
			{
				text5 = above.Attr("notflag", "");
			}
			if (text5 != null)
			{
				backdrop.OnlyIfNotFlag = text5;
			}
			string text6 = null;
			if (child.HasAttr("always"))
			{
				text6 = child.Attr("always", "");
			}
			else if (above != null && above.HasAttr("always"))
			{
				text6 = above.Attr("always", "");
			}
			if (text6 != null)
			{
				backdrop.AlsoIfFlag = text6;
			}
			bool? dreaming = null;
			if (child.HasAttr("dreaming"))
			{
				dreaming = new bool?(child.AttrBool("dreaming", false));
			}
			else if (above != null && above.HasAttr("dreaming"))
			{
				dreaming = new bool?(above.AttrBool("dreaming", false));
			}
			if (dreaming != null)
			{
				backdrop.Dreaming = dreaming;
			}
			if (child.HasAttr("instantIn"))
			{
				backdrop.InstantIn = child.AttrBool("instantIn", false);
			}
			else if (above != null && above.HasAttr("instantIn"))
			{
				backdrop.InstantIn = above.AttrBool("instantIn", false);
			}
			if (child.HasAttr("instantOut"))
			{
				backdrop.InstantOut = child.AttrBool("instantOut", false);
			}
			else if (above != null && above.HasAttr("instantOut"))
			{
				backdrop.InstantOut = above.AttrBool("instantOut", false);
			}
			string text7 = null;
			if (child.HasAttr("fadex"))
			{
				text7 = child.Attr("fadex", "");
			}
			else if (above != null && above.HasAttr("fadex"))
			{
				text7 = above.Attr("fadex", "");
			}
			if (text7 != null)
			{
				backdrop.FadeX = new Backdrop.Fader();
				string[] array = text7.Split(new char[]
				{
					':'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length == 2)
					{
						string[] array3 = array2[0].Split(new char[]
						{
							'-'
						});
						string[] array4 = array2[1].Split(new char[]
						{
							'-'
						});
						float fadeFrom = float.Parse(array4[0], CultureInfo.InvariantCulture);
						float fadeTo = float.Parse(array4[1], CultureInfo.InvariantCulture);
						int num = 1;
						int num2 = 1;
						if (array3[0][0] == 'n')
						{
							num = -1;
							array3[0] = array3[0].Substring(1);
						}
						if (array3[1][0] == 'n')
						{
							num2 = -1;
							array3[1] = array3[1].Substring(1);
						}
						backdrop.FadeX.Add((float)(num * int.Parse(array3[0])), (float)(num2 * int.Parse(array3[1])), fadeFrom, fadeTo);
					}
				}
			}
			string text8 = null;
			if (child.HasAttr("fadey"))
			{
				text8 = child.Attr("fadey", "");
			}
			else if (above != null && above.HasAttr("fadey"))
			{
				text8 = above.Attr("fadey", "");
			}
			if (text8 != null)
			{
				backdrop.FadeY = new Backdrop.Fader();
				string[] array = text8.Split(new char[]
				{
					':'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array5 = array[i].Split(new char[]
					{
						','
					});
					if (array5.Length == 2)
					{
						string[] array6 = array5[0].Split(new char[]
						{
							'-'
						});
						string[] array7 = array5[1].Split(new char[]
						{
							'-'
						});
						float fadeFrom2 = float.Parse(array7[0], CultureInfo.InvariantCulture);
						float fadeTo2 = float.Parse(array7[1], CultureInfo.InvariantCulture);
						int num3 = 1;
						int num4 = 1;
						if (array6[0][0] == 'n')
						{
							num3 = -1;
							array6[0] = array6[0].Substring(1);
						}
						if (array6[1][0] == 'n')
						{
							num4 = -1;
							array6[1] = array6[1].Substring(1);
						}
						backdrop.FadeY.Add((float)(num3 * int.Parse(array6[0])), (float)(num4 * int.Parse(array6[1])), fadeFrom2, fadeTo2);
					}
				}
			}
			return backdrop;
		}

		// Token: 0x06001B95 RID: 7061 RVA: 0x000B5A94 File Offset: 0x000B3C94
		private HashSet<string> ParseLevelsList(string list)
		{
			HashSet<string> hashSet = new HashSet<string>();
			string[] array = list.Split(new char[]
			{
				','
			});
			int i = 0;
			while (i < array.Length)
			{
				string text = array[i];
				if (text.Contains('*'))
				{
					string pattern = "^" + Regex.Escape(text).Replace("\\*", ".*") + "$";
					using (List<LevelData>.Enumerator enumerator = this.Levels.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							LevelData levelData = enumerator.Current;
							if (Regex.IsMatch(levelData.Name, pattern))
							{
								hashSet.Add(levelData.Name);
							}
						}
						goto IL_AA;
					}
					goto IL_A2;
				}
				goto IL_A2;
				IL_AA:
				i++;
				continue;
				IL_A2:
				hashSet.Add(text);
				goto IL_AA;
			}
			return hashSet;
		}

		// Token: 0x04001894 RID: 6292
		public AreaKey Area;

		// Token: 0x04001895 RID: 6293
		public AreaData Data;

		// Token: 0x04001896 RID: 6294
		public ModeProperties ModeData;

		// Token: 0x04001897 RID: 6295
		public int DetectedStrawberries;

		// Token: 0x04001898 RID: 6296
		public bool DetectedRemixNotes;

		// Token: 0x04001899 RID: 6297
		public bool DetectedHeartGem;

		// Token: 0x0400189A RID: 6298
		public List<LevelData> Levels = new List<LevelData>();

		// Token: 0x0400189B RID: 6299
		public List<Rectangle> Filler = new List<Rectangle>();

		// Token: 0x0400189C RID: 6300
		public List<EntityData> Strawberries = new List<EntityData>();

		// Token: 0x0400189D RID: 6301
		public List<EntityData> Goldenberries = new List<EntityData>();

		// Token: 0x0400189E RID: 6302
		public Color BackgroundColor = Color.Black;

		// Token: 0x0400189F RID: 6303
		public BinaryPacker.Element Foreground;

		// Token: 0x040018A0 RID: 6304
		public BinaryPacker.Element Background;

		// Token: 0x040018A1 RID: 6305
		public Rectangle Bounds;
	}
}
