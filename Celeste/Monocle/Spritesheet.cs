using System;
using System.Collections.Generic;

namespace Monocle
{
	// Token: 0x020000F5 RID: 245
	public class Spritesheet<T> : Image
	{
		// Token: 0x0600066F RID: 1647 RVA: 0x0000A4A8 File Offset: 0x000086A8
		public Spritesheet(MTexture texture, int frameWidth, int frameHeight, int frameSep = 0) : base(texture, true)
		{
			this.SetFrames(texture, frameWidth, frameHeight, frameSep);
			this.animations = new Dictionary<T, Spritesheet<T>.Animation>();
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x0000A4D4 File Offset: 0x000086D4
		public void SetFrames(MTexture texture, int frameWidth, int frameHeight, int frameSep = 0)
		{
			List<MTexture> list = new List<MTexture>();
			int i = 0;
			int j = 0;
			while (j <= texture.Height - frameHeight)
			{
				while (i <= texture.Width - frameWidth)
				{
					list.Add(texture.GetSubtexture(i, j, frameWidth, frameHeight, null));
					i += frameWidth + frameSep;
				}
				j += frameHeight + frameSep;
				i = 0;
			}
			this.Frames = list.ToArray();
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x0000A530 File Offset: 0x00008730
		public override void Update()
		{
			if (this.Animating && this.currentAnimation.Delay > 0f)
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
						if (this.currentAnimation.Loop)
						{
							this.CurrentAnimationFrame -= Math.Sign(this.CurrentAnimationFrame) * this.currentAnimation.Frames.Length;
							this.CurrentFrame = this.currentAnimation.Frames[this.CurrentAnimationFrame];
							if (this.OnAnimate != null)
							{
								this.OnAnimate(this.CurrentAnimationID);
							}
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
							this.animationTimer = 0f;
							if (this.OnFinish != null)
							{
								this.OnFinish(this.CurrentAnimationID);
								return;
							}
						}
					}
					else
					{
						this.CurrentFrame = this.currentAnimation.Frames[this.CurrentAnimationFrame];
						if (this.OnAnimate != null)
						{
							this.OnAnimate(this.CurrentAnimationID);
						}
					}
				}
			}
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x0000A717 File Offset: 0x00008917
		public override void Render()
		{
			this.Texture = this.Frames[this.CurrentFrame];
			base.Render();
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x0000A734 File Offset: 0x00008934
		public void Add(T id, bool loop, float delay, params int[] frames)
		{
			this.animations[id] = new Spritesheet<T>.Animation
			{
				Delay = delay,
				Frames = frames,
				Loop = loop
			};
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x0000A76F File Offset: 0x0000896F
		public void Add(T id, float delay, params int[] frames)
		{
			this.Add(id, true, delay, frames);
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x0000A77B File Offset: 0x0000897B
		public void Add(T id, int frame)
		{
			this.Add(id, false, 0f, new int[]
			{
				frame
			});
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x0000A794 File Offset: 0x00008994
		public void ClearAnimations()
		{
			this.animations.Clear();
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x0000A7A4 File Offset: 0x000089A4
		public bool IsPlaying(T id)
		{
			if (!this.played)
			{
				return false;
			}
			if (this.CurrentAnimationID == null)
			{
				return id == null;
			}
			T currentAnimationID = this.CurrentAnimationID;
			return currentAnimationID.Equals(id);
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x0000A7EC File Offset: 0x000089EC
		public void Play(T id, bool restart = false)
		{
			if (!this.IsPlaying(id) || restart)
			{
				this.CurrentAnimationID = id;
				this.currentAnimation = this.animations[id];
				this.animationTimer = 0f;
				this.CurrentAnimationFrame = 0;
				this.played = true;
				this.Animating = (this.currentAnimation.Frames.Length > 1);
				this.CurrentFrame = this.currentAnimation.Frames[0];
			}
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x0000A862 File Offset: 0x00008A62
		public void Reverse(T id, bool restart = false)
		{
			this.Play(id, restart);
			if (this.Rate > 0f)
			{
				this.Rate *= -1f;
			}
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x0000A88B File Offset: 0x00008A8B
		public void Stop()
		{
			this.Animating = false;
			this.played = false;
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600067B RID: 1659 RVA: 0x0000A89B File Offset: 0x00008A9B
		// (set) Token: 0x0600067C RID: 1660 RVA: 0x0000A8A3 File Offset: 0x00008AA3
		public MTexture[] Frames { get; private set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600067D RID: 1661 RVA: 0x0000A8AC File Offset: 0x00008AAC
		// (set) Token: 0x0600067E RID: 1662 RVA: 0x0000A8B4 File Offset: 0x00008AB4
		public bool Animating { get; private set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600067F RID: 1663 RVA: 0x0000A8BD File Offset: 0x00008ABD
		// (set) Token: 0x06000680 RID: 1664 RVA: 0x0000A8C5 File Offset: 0x00008AC5
		public T CurrentAnimationID { get; private set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000681 RID: 1665 RVA: 0x0000A8CE File Offset: 0x00008ACE
		// (set) Token: 0x06000682 RID: 1666 RVA: 0x0000A8D6 File Offset: 0x00008AD6
		public int CurrentAnimationFrame { get; private set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000683 RID: 1667 RVA: 0x0000A8DF File Offset: 0x00008ADF
		public override float Width
		{
			get
			{
				if (this.Frames.Length != 0)
				{
					return (float)this.Frames[0].Width;
				}
				return 0f;
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000684 RID: 1668 RVA: 0x0000A8FE File Offset: 0x00008AFE
		public override float Height
		{
			get
			{
				if (this.Frames.Length != 0)
				{
					return (float)this.Frames[0].Height;
				}
				return 0f;
			}
		}

		// Token: 0x040004D2 RID: 1234
		public int CurrentFrame;

		// Token: 0x040004D3 RID: 1235
		public float Rate = 1f;

		// Token: 0x040004D4 RID: 1236
		public bool UseRawDeltaTime;

		// Token: 0x040004D5 RID: 1237
		public Action<T> OnFinish;

		// Token: 0x040004D6 RID: 1238
		public Action<T> OnLoop;

		// Token: 0x040004D7 RID: 1239
		public Action<T> OnAnimate;

		// Token: 0x040004D8 RID: 1240
		private Dictionary<T, Spritesheet<T>.Animation> animations;

		// Token: 0x040004D9 RID: 1241
		private Spritesheet<T>.Animation currentAnimation;

		// Token: 0x040004DA RID: 1242
		private float animationTimer;

		// Token: 0x040004DB RID: 1243
		private bool played;

		// Token: 0x0200039B RID: 923
		private struct Animation
		{
			// Token: 0x04001EEE RID: 7918
			public float Delay;

			// Token: 0x04001EEF RID: 7919
			public int[] Frames;

			// Token: 0x04001EF0 RID: 7920
			public bool Loop;
		}
	}
}
