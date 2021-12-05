using System;
using System.Collections.Generic;

namespace Monocle
{
	// Token: 0x020000FB RID: 251
	public class Alarm : Component
	{
		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060006BD RID: 1725 RVA: 0x0000B590 File Offset: 0x00009790
		// (set) Token: 0x060006BE RID: 1726 RVA: 0x0000B598 File Offset: 0x00009798
		public Alarm.AlarmMode Mode { get; private set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060006BF RID: 1727 RVA: 0x0000B5A1 File Offset: 0x000097A1
		// (set) Token: 0x060006C0 RID: 1728 RVA: 0x0000B5A9 File Offset: 0x000097A9
		public float Duration { get; private set; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060006C1 RID: 1729 RVA: 0x0000B5B2 File Offset: 0x000097B2
		// (set) Token: 0x060006C2 RID: 1730 RVA: 0x0000B5BA File Offset: 0x000097BA
		public float TimeLeft { get; private set; }

		// Token: 0x060006C3 RID: 1731 RVA: 0x0000B5C4 File Offset: 0x000097C4
		public static Alarm Create(Alarm.AlarmMode mode, Action onComplete, float duration = 1f, bool start = false)
		{
			Alarm alarm;
			if (Alarm.cached.Count == 0)
			{
				alarm = new Alarm();
			}
			else
			{
				alarm = Alarm.cached.Pop();
			}
			alarm.Init(mode, onComplete, duration, start);
			return alarm;
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x0000B5FC File Offset: 0x000097FC
		public static Alarm Set(Entity entity, float duration, Action onComplete, Alarm.AlarmMode alarmMode = Alarm.AlarmMode.Oneshot)
		{
			Alarm alarm = Alarm.Create(alarmMode, onComplete, duration, true);
			entity.Add(alarm);
			return alarm;
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x0000B61B File Offset: 0x0000981B
		private Alarm() : base(false, false)
		{
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x0000B625 File Offset: 0x00009825
		private void Init(Alarm.AlarmMode mode, Action onComplete, float duration = 1f, bool start = false)
		{
			this.Mode = mode;
			this.Duration = duration;
			this.OnComplete = onComplete;
			this.Active = false;
			this.TimeLeft = 0f;
			if (start)
			{
				this.Start();
			}
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x0000B658 File Offset: 0x00009858
		public override void Update()
		{
			this.TimeLeft -= Engine.DeltaTime;
			if (this.TimeLeft <= 0f)
			{
				this.TimeLeft = 0f;
				if (this.OnComplete != null)
				{
					this.OnComplete();
				}
				if (this.Mode == Alarm.AlarmMode.Looping)
				{
					this.Start();
					return;
				}
				if (this.Mode == Alarm.AlarmMode.Oneshot)
				{
					base.RemoveSelf();
					return;
				}
				if (this.TimeLeft <= 0f)
				{
					this.Active = false;
				}
			}
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x0000B6D6 File Offset: 0x000098D6
		public override void Removed(Entity entity)
		{
			base.Removed(entity);
			Alarm.cached.Push(this);
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x0000B6EA File Offset: 0x000098EA
		public void Start()
		{
			this.Active = true;
			this.TimeLeft = this.Duration;
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x0000B6FF File Offset: 0x000098FF
		public void Start(float duration)
		{
			this.Duration = duration;
			this.Start();
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x0000B70E File Offset: 0x0000990E
		public void Stop()
		{
			this.Active = false;
		}

		// Token: 0x040004FD RID: 1277
		public Action OnComplete;

		// Token: 0x04000501 RID: 1281
		private static Stack<Alarm> cached = new Stack<Alarm>();

		// Token: 0x020003A0 RID: 928
		public enum AlarmMode
		{
			// Token: 0x04001EFF RID: 7935
			Persist,
			// Token: 0x04001F00 RID: 7936
			Oneshot,
			// Token: 0x04001F01 RID: 7937
			Looping
		}
	}
}
