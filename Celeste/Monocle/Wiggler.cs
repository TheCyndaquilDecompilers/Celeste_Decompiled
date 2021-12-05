using System;
using System.Collections.Generic;

namespace Monocle
{
	// Token: 0x02000104 RID: 260
	public class Wiggler : Component
	{
		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000723 RID: 1827 RVA: 0x0000CAFA File Offset: 0x0000ACFA
		// (set) Token: 0x06000724 RID: 1828 RVA: 0x0000CB02 File Offset: 0x0000AD02
		public float Counter { get; private set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000725 RID: 1829 RVA: 0x0000CB0B File Offset: 0x0000AD0B
		// (set) Token: 0x06000726 RID: 1830 RVA: 0x0000CB13 File Offset: 0x0000AD13
		public float Value { get; private set; }

		// Token: 0x06000727 RID: 1831 RVA: 0x0000CB1C File Offset: 0x0000AD1C
		public static Wiggler Create(float duration, float frequency, Action<float> onChange = null, bool start = false, bool removeSelfOnFinish = false)
		{
			Wiggler wiggler;
			if (Wiggler.cache.Count > 0)
			{
				wiggler = Wiggler.cache.Pop();
			}
			else
			{
				wiggler = new Wiggler();
			}
			wiggler.Init(duration, frequency, onChange, start, removeSelfOnFinish);
			return wiggler;
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x0000B61B File Offset: 0x0000981B
		private Wiggler() : base(false, false)
		{
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x0000CB58 File Offset: 0x0000AD58
		private void Init(float duration, float frequency, Action<float> onChange, bool start, bool removeSelfOnFinish)
		{
			this.Counter = (this.sineCounter = 0f);
			this.UseRawDeltaTime = false;
			this.increment = 1f / duration;
			this.sineAdd = 6.2831855f * frequency;
			this.onChange = onChange;
			this.removeSelfOnFinish = removeSelfOnFinish;
			if (start)
			{
				this.Start();
				return;
			}
			this.Active = false;
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x0000CBBB File Offset: 0x0000ADBB
		public override void Removed(Entity entity)
		{
			base.Removed(entity);
			Wiggler.cache.Push(this);
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x0000CBD0 File Offset: 0x0000ADD0
		public void Start()
		{
			this.Counter = 1f;
			if (this.StartZero)
			{
				this.sineCounter = 1.5707964f;
				this.Value = 0f;
				if (this.onChange != null)
				{
					this.onChange(0f);
				}
			}
			else
			{
				this.sineCounter = 0f;
				this.Value = 1f;
				if (this.onChange != null)
				{
					this.onChange(1f);
				}
			}
			this.Active = true;
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x0000CC55 File Offset: 0x0000AE55
		public void Start(float duration, float frequency)
		{
			this.increment = 1f / duration;
			this.sineAdd = 6.2831855f * frequency;
			this.Start();
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x0000B70E File Offset: 0x0000990E
		public void Stop()
		{
			this.Active = false;
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x0000CC77 File Offset: 0x0000AE77
		public void StopAndClear()
		{
			this.Stop();
			this.Value = 0f;
		}

		// Token: 0x0600072F RID: 1839 RVA: 0x0000CC8C File Offset: 0x0000AE8C
		public override void Update()
		{
			if (this.UseRawDeltaTime)
			{
				this.sineCounter += this.sineAdd * Engine.RawDeltaTime;
				this.Counter -= this.increment * Engine.RawDeltaTime;
			}
			else
			{
				this.sineCounter += this.sineAdd * Engine.DeltaTime;
				this.Counter -= this.increment * Engine.DeltaTime;
			}
			if (this.Counter <= 0f)
			{
				this.Counter = 0f;
				this.Active = false;
				if (this.removeSelfOnFinish)
				{
					base.RemoveSelf();
				}
			}
			this.Value = (float)Math.Cos((double)this.sineCounter) * this.Counter;
			if (this.onChange != null)
			{
				this.onChange(this.Value);
			}
		}

		// Token: 0x0400053A RID: 1338
		private static Stack<Wiggler> cache = new Stack<Wiggler>();

		// Token: 0x0400053D RID: 1341
		public bool StartZero;

		// Token: 0x0400053E RID: 1342
		public bool UseRawDeltaTime;

		// Token: 0x0400053F RID: 1343
		private float sineCounter;

		// Token: 0x04000540 RID: 1344
		private float increment;

		// Token: 0x04000541 RID: 1345
		private float sineAdd;

		// Token: 0x04000542 RID: 1346
		private Action<float> onChange;

		// Token: 0x04000543 RID: 1347
		private bool removeSelfOnFinish;
	}
}
