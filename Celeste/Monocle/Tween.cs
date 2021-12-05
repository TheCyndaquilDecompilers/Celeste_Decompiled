using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x02000103 RID: 259
	public class Tween : Component
	{
		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000708 RID: 1800 RVA: 0x0000C68B File Offset: 0x0000A88B
		// (set) Token: 0x06000709 RID: 1801 RVA: 0x0000C693 File Offset: 0x0000A893
		public Tween.TweenMode Mode { get; private set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600070A RID: 1802 RVA: 0x0000C69C File Offset: 0x0000A89C
		// (set) Token: 0x0600070B RID: 1803 RVA: 0x0000C6A4 File Offset: 0x0000A8A4
		public float Duration { get; private set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600070C RID: 1804 RVA: 0x0000C6AD File Offset: 0x0000A8AD
		// (set) Token: 0x0600070D RID: 1805 RVA: 0x0000C6B5 File Offset: 0x0000A8B5
		public float TimeLeft { get; private set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600070E RID: 1806 RVA: 0x0000C6BE File Offset: 0x0000A8BE
		// (set) Token: 0x0600070F RID: 1807 RVA: 0x0000C6C6 File Offset: 0x0000A8C6
		public float Percent { get; private set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000710 RID: 1808 RVA: 0x0000C6CF File Offset: 0x0000A8CF
		// (set) Token: 0x06000711 RID: 1809 RVA: 0x0000C6D7 File Offset: 0x0000A8D7
		public float Eased { get; private set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000712 RID: 1810 RVA: 0x0000C6E0 File Offset: 0x0000A8E0
		// (set) Token: 0x06000713 RID: 1811 RVA: 0x0000C6E8 File Offset: 0x0000A8E8
		public bool Reverse { get; private set; }

		// Token: 0x06000714 RID: 1812 RVA: 0x0000C6F4 File Offset: 0x0000A8F4
		public static Tween Create(Tween.TweenMode mode, Ease.Easer easer = null, float duration = 1f, bool start = false)
		{
			Tween tween = null;
			foreach (Tween tween2 in Tween.cached)
			{
				if (Engine.FrameCounter > tween2.cachedFrame + 3UL)
				{
					tween = tween2;
					Tween.cached.Remove(tween2);
					break;
				}
			}
			if (tween == null)
			{
				tween = new Tween();
			}
			tween.OnUpdate = (tween.OnComplete = (tween.OnStart = null));
			tween.Init(mode, easer, duration, start);
			return tween;
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x0000C790 File Offset: 0x0000A990
		public static Tween Set(Entity entity, Tween.TweenMode tweenMode, float duration, Ease.Easer easer, Action<Tween> onUpdate, Action<Tween> onComplete = null)
		{
			Tween tween = Tween.Create(tweenMode, easer, duration, true);
			Tween tween2 = tween;
			tween2.OnUpdate = (Action<Tween>)Delegate.Combine(tween2.OnUpdate, onUpdate);
			Tween tween3 = tween;
			tween3.OnComplete = (Action<Tween>)Delegate.Combine(tween3.OnComplete, onComplete);
			entity.Add(tween);
			return tween;
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x0000C7E0 File Offset: 0x0000A9E0
		public static Tween Position(Entity entity, Vector2 targetPosition, float duration, Ease.Easer easer, Tween.TweenMode tweenMode = Tween.TweenMode.Oneshot)
		{
			Vector2 startPosition = entity.Position;
			Tween tween = Tween.Create(tweenMode, easer, duration, true);
			tween.OnUpdate = delegate(Tween t)
			{
				entity.Position = Vector2.Lerp(startPosition, targetPosition, t.Eased);
			};
			entity.Add(tween);
			return tween;
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x0000B61B File Offset: 0x0000981B
		private Tween() : base(false, false)
		{
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x0000C83C File Offset: 0x0000AA3C
		private void Init(Tween.TweenMode mode, Ease.Easer easer, float duration, bool start)
		{
			if (duration <= 0f)
			{
				duration = 1E-06f;
			}
			this.UseRawDeltaTime = false;
			this.Mode = mode;
			this.Easer = easer;
			this.Duration = duration;
			this.TimeLeft = 0f;
			this.Percent = 0f;
			this.Active = false;
			if (start)
			{
				this.Start();
			}
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x0000C89B File Offset: 0x0000AA9B
		public override void Removed(Entity entity)
		{
			base.Removed(entity);
			Tween.cached.Add(this);
			this.cachedFrame = Engine.FrameCounter;
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x0000C8BC File Offset: 0x0000AABC
		public override void Update()
		{
			this.TimeLeft -= (this.UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime);
			this.Percent = Math.Max(0f, this.TimeLeft) / this.Duration;
			if (!this.Reverse)
			{
				this.Percent = 1f - this.Percent;
			}
			if (this.Easer != null)
			{
				this.Eased = this.Easer(this.Percent);
			}
			else
			{
				this.Eased = this.Percent;
			}
			if (this.OnUpdate != null)
			{
				this.OnUpdate(this);
			}
			if (this.TimeLeft <= 0f)
			{
				this.TimeLeft = 0f;
				if (this.OnComplete != null)
				{
					this.OnComplete(this);
				}
				switch (this.Mode)
				{
				case Tween.TweenMode.Persist:
					this.Active = false;
					return;
				case Tween.TweenMode.Oneshot:
					this.Active = false;
					base.RemoveSelf();
					return;
				case Tween.TweenMode.Looping:
					this.Start(this.Reverse);
					return;
				case Tween.TweenMode.YoyoOneshot:
					if (this.Reverse == this.startedReversed)
					{
						this.Start(!this.Reverse);
						this.startedReversed = !this.Reverse;
						return;
					}
					this.Active = false;
					base.RemoveSelf();
					return;
				case Tween.TweenMode.YoyoLooping:
					this.Start(!this.Reverse);
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x0000CA21 File Offset: 0x0000AC21
		public void Start()
		{
			this.Start(false);
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x0000CA2C File Offset: 0x0000AC2C
		public void Start(bool reverse)
		{
			this.Reverse = reverse;
			this.startedReversed = reverse;
			this.TimeLeft = this.Duration;
			this.Eased = (this.Percent = (float)(this.Reverse ? 1 : 0));
			this.Active = true;
			if (this.OnStart != null)
			{
				this.OnStart(this);
			}
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x0000CA8C File Offset: 0x0000AC8C
		public void Start(float duration, bool reverse = false)
		{
			this.Duration = duration;
			this.Start(reverse);
		}

		// Token: 0x0600071E RID: 1822 RVA: 0x0000B70E File Offset: 0x0000990E
		public void Stop()
		{
			this.Active = false;
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x0000CA9C File Offset: 0x0000AC9C
		public void Reset()
		{
			this.TimeLeft = this.Duration;
			this.Eased = (this.Percent = (float)(this.Reverse ? 1 : 0));
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x0000CAD1 File Offset: 0x0000ACD1
		public IEnumerator Wait()
		{
			while (this.Active)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000721 RID: 1825 RVA: 0x0000CAE0 File Offset: 0x0000ACE0
		public float Inverted
		{
			get
			{
				return 1f - this.Eased;
			}
		}

		// Token: 0x0400052C RID: 1324
		public Ease.Easer Easer;

		// Token: 0x0400052D RID: 1325
		public Action<Tween> OnUpdate;

		// Token: 0x0400052E RID: 1326
		public Action<Tween> OnComplete;

		// Token: 0x0400052F RID: 1327
		public Action<Tween> OnStart;

		// Token: 0x04000530 RID: 1328
		public bool UseRawDeltaTime;

		// Token: 0x04000537 RID: 1335
		private bool startedReversed;

		// Token: 0x04000538 RID: 1336
		private ulong cachedFrame;

		// Token: 0x04000539 RID: 1337
		private static List<Tween> cached = new List<Tween>();

		// Token: 0x020003A3 RID: 931
		public enum TweenMode
		{
			// Token: 0x04001F09 RID: 7945
			Persist,
			// Token: 0x04001F0A RID: 7946
			Oneshot,
			// Token: 0x04001F0B RID: 7947
			Looping,
			// Token: 0x04001F0C RID: 7948
			YoyoOneshot,
			// Token: 0x04001F0D RID: 7949
			YoyoLooping
		}
	}
}
