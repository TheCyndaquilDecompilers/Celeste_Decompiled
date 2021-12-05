using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x020000F4 RID: 244
	public class Sprite : Image
	{
		// Token: 0x06000643 RID: 1603 RVA: 0x0000983F File Offset: 0x00007A3F
		public Sprite(Atlas atlas, string path) : base(null, true)
		{
			this.atlas = atlas;
			this.Path = path;
			this.animations = new Dictionary<string, Sprite.Animation>(StringComparer.OrdinalIgnoreCase);
			this.CurrentAnimationID = "";
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x00009880 File Offset: 0x00007A80
		public void Reset(Atlas atlas, string path)
		{
			this.atlas = atlas;
			this.Path = path;
			this.animations = new Dictionary<string, Sprite.Animation>(StringComparer.OrdinalIgnoreCase);
			this.currentAnimation = null;
			this.CurrentAnimationID = "";
			this.OnFinish = null;
			this.OnLoop = null;
			this.OnFrameChange = null;
			this.OnChange = null;
			this.Animating = false;
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x000098E0 File Offset: 0x00007AE0
		public MTexture GetFrame(string animation, int frame)
		{
			return this.animations[animation].Frames[frame];
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000646 RID: 1606 RVA: 0x000098F5 File Offset: 0x00007AF5
		public Vector2 Center
		{
			get
			{
				return new Vector2(this.Width / 2f, this.Height / 2f);
			}
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x00009914 File Offset: 0x00007B14
		public override void Update()
		{
			if (this.Animating)
			{
				if (this.UseRawDeltaTime)
				{
					this.animationTimer += Engine.RawDeltaTime * this.Rate;
				}
				else
				{
					this.animationTimer += Engine.DeltaTime * this.Rate;
				}
				if (Math.Abs(this.animationTimer) >= this.currentAnimation.Delay)
				{
					this.CurrentAnimationFrame += Math.Sign(this.animationTimer);
					this.animationTimer -= (float)Math.Sign(this.animationTimer) * this.currentAnimation.Delay;
					if (this.CurrentAnimationFrame < 0 || this.CurrentAnimationFrame >= this.currentAnimation.Frames.Length)
					{
						string currentAnimationID = this.CurrentAnimationID;
						if (this.OnLastFrame != null)
						{
							this.OnLastFrame(this.CurrentAnimationID);
						}
						if (currentAnimationID == this.CurrentAnimationID)
						{
							if (this.currentAnimation.Goto != null)
							{
								this.CurrentAnimationID = this.currentAnimation.Goto.Choose();
								if (this.OnChange != null)
								{
									this.OnChange(this.LastAnimationID, this.CurrentAnimationID);
								}
								this.LastAnimationID = this.CurrentAnimationID;
								this.currentAnimation = this.animations[this.LastAnimationID];
								if (this.CurrentAnimationFrame < 0)
								{
									this.CurrentAnimationFrame = this.currentAnimation.Frames.Length - 1;
								}
								else
								{
									this.CurrentAnimationFrame = 0;
								}
								this.SetFrame(this.currentAnimation.Frames[this.CurrentAnimationFrame]);
								if (this.OnLoop != null)
								{
									this.OnLoop(this.CurrentAnimationID);
									return;
								}
							}
							else
							{
								if (this.CurrentAnimationFrame < 0)
								{
									this.CurrentAnimationFrame = 0;
								}
								else
								{
									this.CurrentAnimationFrame = this.currentAnimation.Frames.Length - 1;
								}
								this.Animating = false;
								string currentAnimationID2 = this.CurrentAnimationID;
								this.CurrentAnimationID = "";
								this.currentAnimation = null;
								this.animationTimer = 0f;
								if (this.OnFinish != null)
								{
									this.OnFinish(currentAnimationID2);
									return;
								}
							}
						}
					}
					else
					{
						this.SetFrame(this.currentAnimation.Frames[this.CurrentAnimationFrame]);
					}
				}
			}
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x00009B54 File Offset: 0x00007D54
		private void SetFrame(MTexture texture)
		{
			if (texture == this.Texture)
			{
				return;
			}
			this.Texture = texture;
			if (this.width == 0)
			{
				this.width = texture.Width;
			}
			if (this.height == 0)
			{
				this.height = texture.Height;
			}
			if (this.Justify != null)
			{
				this.Origin = new Vector2((float)this.Texture.Width * this.Justify.Value.X, (float)this.Texture.Height * this.Justify.Value.Y);
			}
			if (this.OnFrameChange != null)
			{
				this.OnFrameChange(this.CurrentAnimationID);
			}
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00009C05 File Offset: 0x00007E05
		public void SetAnimationFrame(int frame)
		{
			this.animationTimer = 0f;
			this.CurrentAnimationFrame = frame % this.currentAnimation.Frames.Length;
			this.SetFrame(this.currentAnimation.Frames[this.CurrentAnimationFrame]);
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00009C3F File Offset: 0x00007E3F
		public void AddLoop(string id, string path, float delay)
		{
			this.animations[id] = new Sprite.Animation
			{
				Delay = delay,
				Frames = this.GetFrames(path, null),
				Goto = new Chooser<string>(id, 1f)
			};
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x00009C78 File Offset: 0x00007E78
		public void AddLoop(string id, string path, float delay, params int[] frames)
		{
			this.animations[id] = new Sprite.Animation
			{
				Delay = delay,
				Frames = this.GetFrames(path, frames),
				Goto = new Chooser<string>(id, 1f)
			};
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x00009CB2 File Offset: 0x00007EB2
		public void AddLoop(string id, float delay, params MTexture[] frames)
		{
			this.animations[id] = new Sprite.Animation
			{
				Delay = delay,
				Frames = frames,
				Goto = new Chooser<string>(id, 1f)
			};
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x00009CE4 File Offset: 0x00007EE4
		public void Add(string id, string path)
		{
			this.animations[id] = new Sprite.Animation
			{
				Delay = 0f,
				Frames = this.GetFrames(path, null),
				Goto = null
			};
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00009D17 File Offset: 0x00007F17
		public void Add(string id, string path, float delay)
		{
			this.animations[id] = new Sprite.Animation
			{
				Delay = delay,
				Frames = this.GetFrames(path, null),
				Goto = null
			};
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x00009D46 File Offset: 0x00007F46
		public void Add(string id, string path, float delay, params int[] frames)
		{
			this.animations[id] = new Sprite.Animation
			{
				Delay = delay,
				Frames = this.GetFrames(path, frames),
				Goto = null
			};
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x00009D76 File Offset: 0x00007F76
		public void Add(string id, string path, float delay, string into)
		{
			this.animations[id] = new Sprite.Animation
			{
				Delay = delay,
				Frames = this.GetFrames(path, null),
				Goto = Chooser<string>.FromString<string>(into)
			};
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00009DAB File Offset: 0x00007FAB
		public void Add(string id, string path, float delay, Chooser<string> into)
		{
			this.animations[id] = new Sprite.Animation
			{
				Delay = delay,
				Frames = this.GetFrames(path, null),
				Goto = into
			};
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00009DDB File Offset: 0x00007FDB
		public void Add(string id, string path, float delay, string into, params int[] frames)
		{
			this.animations[id] = new Sprite.Animation
			{
				Delay = delay,
				Frames = this.GetFrames(path, frames),
				Goto = Chooser<string>.FromString<string>(into)
			};
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x00009E11 File Offset: 0x00008011
		public void Add(string id, float delay, string into, params MTexture[] frames)
		{
			this.animations[id] = new Sprite.Animation
			{
				Delay = delay,
				Frames = frames,
				Goto = Chooser<string>.FromString<string>(into)
			};
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x00009E3F File Offset: 0x0000803F
		public void Add(string id, string path, float delay, Chooser<string> into, params int[] frames)
		{
			this.animations[id] = new Sprite.Animation
			{
				Delay = delay,
				Frames = this.GetFrames(path, frames),
				Goto = into
			};
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00009E70 File Offset: 0x00008070
		private MTexture[] GetFrames(string path, int[] frames = null)
		{
			MTexture[] array;
			if (frames == null || frames.Length == 0)
			{
				array = this.atlas.GetAtlasSubtextures(this.Path + path).ToArray();
			}
			else
			{
				string text = this.Path + path;
				MTexture[] array2 = new MTexture[frames.Length];
				for (int i = 0; i < frames.Length; i++)
				{
					MTexture atlasSubtexturesAt = this.atlas.GetAtlasSubtexturesAt(text, frames[i]);
					if (atlasSubtexturesAt == null)
					{
						throw new Exception(string.Concat(new object[]
						{
							"Can't find sprite ",
							text,
							" with index ",
							frames[i]
						}));
					}
					array2[i] = atlasSubtexturesAt;
				}
				array = array2;
			}
			this.width = Math.Max(array[0].Width, this.width);
			this.height = Math.Max(array[0].Height, this.height);
			return array;
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00009F46 File Offset: 0x00008146
		public void ClearAnimations()
		{
			this.animations.Clear();
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x00009F54 File Offset: 0x00008154
		public void Play(string id, bool restart = false, bool randomizeFrame = false)
		{
			if (this.CurrentAnimationID != id || restart)
			{
				if (this.OnChange != null)
				{
					this.OnChange(this.LastAnimationID, id);
				}
				this.CurrentAnimationID = id;
				this.LastAnimationID = id;
				this.currentAnimation = this.animations[id];
				this.Animating = (this.currentAnimation.Delay > 0f);
				if (randomizeFrame)
				{
					this.animationTimer = Calc.Random.NextFloat(this.currentAnimation.Delay);
					this.CurrentAnimationFrame = Calc.Random.Next(this.currentAnimation.Frames.Length);
				}
				else
				{
					this.animationTimer = 0f;
					this.CurrentAnimationFrame = 0;
				}
				this.SetFrame(this.currentAnimation.Frames[this.CurrentAnimationFrame]);
			}
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x0000A030 File Offset: 0x00008230
		public void PlayOffset(string id, float offset, bool restart = false)
		{
			if (this.CurrentAnimationID != id || restart)
			{
				if (this.OnChange != null)
				{
					this.OnChange(this.LastAnimationID, id);
				}
				this.CurrentAnimationID = id;
				this.LastAnimationID = id;
				this.currentAnimation = this.animations[id];
				if (this.currentAnimation.Delay > 0f)
				{
					this.Animating = true;
					float num = this.currentAnimation.Delay * (float)this.currentAnimation.Frames.Length * offset;
					this.CurrentAnimationFrame = 0;
					while (num >= this.currentAnimation.Delay)
					{
						int currentAnimationFrame = this.CurrentAnimationFrame;
						this.CurrentAnimationFrame = currentAnimationFrame + 1;
						num -= this.currentAnimation.Delay;
					}
					this.CurrentAnimationFrame %= this.currentAnimation.Frames.Length;
					this.animationTimer = num;
					this.SetFrame(this.currentAnimation.Frames[this.CurrentAnimationFrame]);
					return;
				}
				this.animationTimer = 0f;
				this.Animating = false;
				this.CurrentAnimationFrame = 0;
				this.SetFrame(this.currentAnimation.Frames[0]);
			}
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x0000A160 File Offset: 0x00008360
		public IEnumerator PlayRoutine(string id, bool restart = false)
		{
			this.Play(id, restart, false);
			return this.PlayUtil();
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x0000A171 File Offset: 0x00008371
		public IEnumerator ReverseRoutine(string id, bool restart = false)
		{
			this.Reverse(id, restart);
			return this.PlayUtil();
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x0000A181 File Offset: 0x00008381
		private IEnumerator PlayUtil()
		{
			while (this.Animating)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x0000A190 File Offset: 0x00008390
		public void Reverse(string id, bool restart = false)
		{
			this.Play(id, restart, false);
			if (this.Rate > 0f)
			{
				this.Rate *= -1f;
			}
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x0000A1BA File Offset: 0x000083BA
		public bool Has(string id)
		{
			return id != null && this.animations.ContainsKey(id);
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x0000A1CD File Offset: 0x000083CD
		public void Stop()
		{
			this.Animating = false;
			this.currentAnimation = null;
			this.CurrentAnimationID = "";
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600065F RID: 1631 RVA: 0x0000A1E8 File Offset: 0x000083E8
		// (set) Token: 0x06000660 RID: 1632 RVA: 0x0000A1F0 File Offset: 0x000083F0
		public bool Animating { get; private set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000661 RID: 1633 RVA: 0x0000A1F9 File Offset: 0x000083F9
		// (set) Token: 0x06000662 RID: 1634 RVA: 0x0000A201 File Offset: 0x00008401
		public string CurrentAnimationID { get; private set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000663 RID: 1635 RVA: 0x0000A20A File Offset: 0x0000840A
		// (set) Token: 0x06000664 RID: 1636 RVA: 0x0000A212 File Offset: 0x00008412
		public string LastAnimationID { get; private set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000665 RID: 1637 RVA: 0x0000A21B File Offset: 0x0000841B
		// (set) Token: 0x06000666 RID: 1638 RVA: 0x0000A223 File Offset: 0x00008423
		public int CurrentAnimationFrame { get; private set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000667 RID: 1639 RVA: 0x0000A22C File Offset: 0x0000842C
		public int CurrentAnimationTotalFrames
		{
			get
			{
				if (this.currentAnimation != null)
				{
					return this.currentAnimation.Frames.Length;
				}
				return 0;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000668 RID: 1640 RVA: 0x0000A245 File Offset: 0x00008445
		public override float Width
		{
			get
			{
				return (float)this.width;
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000669 RID: 1641 RVA: 0x0000A24E File Offset: 0x0000844E
		public override float Height
		{
			get
			{
				return (float)this.height;
			}
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x0000A257 File Offset: 0x00008457
		internal Sprite() : base(null, true)
		{
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x0000A26C File Offset: 0x0000846C
		internal Sprite CreateClone()
		{
			return this.CloneInto(new Sprite());
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x0000A27C File Offset: 0x0000847C
		internal Sprite CloneInto(Sprite clone)
		{
			clone.Texture = this.Texture;
			clone.Position = this.Position;
			clone.Justify = this.Justify;
			clone.Origin = this.Origin;
			clone.animations = new Dictionary<string, Sprite.Animation>(this.animations, StringComparer.OrdinalIgnoreCase);
			clone.currentAnimation = this.currentAnimation;
			clone.animationTimer = this.animationTimer;
			clone.width = this.width;
			clone.height = this.height;
			clone.Animating = this.Animating;
			clone.CurrentAnimationID = this.CurrentAnimationID;
			clone.LastAnimationID = this.LastAnimationID;
			clone.CurrentAnimationFrame = this.CurrentAnimationFrame;
			return clone;
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x0000A330 File Offset: 0x00008530
		public void DrawSubrect(Vector2 offset, Rectangle rectangle)
		{
			if (this.Texture != null)
			{
				Rectangle relativeRect = this.Texture.GetRelativeRect(rectangle);
				Vector2 value = new Vector2(-Math.Min((float)rectangle.X - this.Texture.DrawOffset.X, 0f), -Math.Min((float)rectangle.Y - this.Texture.DrawOffset.Y, 0f));
				Draw.SpriteBatch.Draw(this.Texture.Texture.Texture, base.RenderPosition + offset, new Rectangle?(relativeRect), this.Color, this.Rotation, this.Origin - value, this.Scale, this.Effects, 0f);
			}
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x0000A3F8 File Offset: 0x000085F8
		public void LogAnimations()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, Sprite.Animation> keyValuePair in this.animations)
			{
				Sprite.Animation value = keyValuePair.Value;
				stringBuilder.Append(keyValuePair.Key);
				stringBuilder.Append("\n{\n\t");
				StringBuilder stringBuilder2 = stringBuilder;
				string separator = "\n\t";
				object[] frames = value.Frames;
				stringBuilder2.Append(string.Join(separator, frames));
				stringBuilder.Append("\n}\n");
			}
			Calc.Log(new object[]
			{
				stringBuilder.ToString()
			});
		}

		// Token: 0x040004BF RID: 1215
		public float Rate = 1f;

		// Token: 0x040004C0 RID: 1216
		public bool UseRawDeltaTime;

		// Token: 0x040004C1 RID: 1217
		public Vector2? Justify;

		// Token: 0x040004C2 RID: 1218
		public Action<string> OnFinish;

		// Token: 0x040004C3 RID: 1219
		public Action<string> OnLoop;

		// Token: 0x040004C4 RID: 1220
		public Action<string> OnFrameChange;

		// Token: 0x040004C5 RID: 1221
		public Action<string> OnLastFrame;

		// Token: 0x040004C6 RID: 1222
		public Action<string, string> OnChange;

		// Token: 0x040004C7 RID: 1223
		private Atlas atlas;

		// Token: 0x040004C8 RID: 1224
		public string Path;

		// Token: 0x040004C9 RID: 1225
		private Dictionary<string, Sprite.Animation> animations;

		// Token: 0x040004CA RID: 1226
		private Sprite.Animation currentAnimation;

		// Token: 0x040004CB RID: 1227
		private float animationTimer;

		// Token: 0x040004CC RID: 1228
		private int width;

		// Token: 0x040004CD RID: 1229
		private int height;

		// Token: 0x02000399 RID: 921
		private class Animation
		{
			// Token: 0x04001EE8 RID: 7912
			public float Delay;

			// Token: 0x04001EE9 RID: 7913
			public MTexture[] Frames;

			// Token: 0x04001EEA RID: 7914
			public Chooser<string> Goto;
		}
	}
}
