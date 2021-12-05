using System;

namespace Monocle
{
	// Token: 0x02000101 RID: 257
	public class SineWave : Component
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060006EA RID: 1770 RVA: 0x0000BF09 File Offset: 0x0000A109
		// (set) Token: 0x060006EB RID: 1771 RVA: 0x0000BF11 File Offset: 0x0000A111
		public float Value { get; private set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060006EC RID: 1772 RVA: 0x0000BF1A File Offset: 0x0000A11A
		// (set) Token: 0x060006ED RID: 1773 RVA: 0x0000BF22 File Offset: 0x0000A122
		public float ValueOverTwo { get; private set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060006EE RID: 1774 RVA: 0x0000BF2B File Offset: 0x0000A12B
		// (set) Token: 0x060006EF RID: 1775 RVA: 0x0000BF33 File Offset: 0x0000A133
		public float TwoValue { get; private set; }

		// Token: 0x060006F0 RID: 1776 RVA: 0x0000BF3C File Offset: 0x0000A13C
		public SineWave() : base(true, false)
		{
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x0000BF5C File Offset: 0x0000A15C
		public SineWave(float frequency, float offset = 0f) : this()
		{
			this.Frequency = frequency;
			this.Counter = offset;
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x0000BF74 File Offset: 0x0000A174
		public override void Update()
		{
			this.Counter += 6.2831855f * this.Frequency * this.Rate * (this.UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime);
			if (this.OnUpdate != null)
			{
				this.OnUpdate(this.Value);
			}
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x0000BFCF File Offset: 0x0000A1CF
		public float ValueOffset(float offset)
		{
			return (float)Math.Sin((double)(this.counter + offset));
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x0000BFE0 File Offset: 0x0000A1E0
		public SineWave Randomize()
		{
			this.Counter = Calc.Random.NextFloat() * 6.2831855f * 2f;
			return this;
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x0000BFFF File Offset: 0x0000A1FF
		public void Reset()
		{
			this.Counter = 0f;
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x0000C00C File Offset: 0x0000A20C
		public void StartUp()
		{
			this.Counter = 1.5707964f;
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x0000C019 File Offset: 0x0000A219
		public void StartDown()
		{
			this.Counter = 4.712389f;
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060006F8 RID: 1784 RVA: 0x0000C026 File Offset: 0x0000A226
		// (set) Token: 0x060006F9 RID: 1785 RVA: 0x0000C030 File Offset: 0x0000A230
		public float Counter
		{
			get
			{
				return this.counter;
			}
			set
			{
				this.counter = (value + 25.132742f) % 25.132742f;
				this.Value = (float)Math.Sin((double)this.counter);
				this.ValueOverTwo = (float)Math.Sin((double)(this.counter / 2f));
				this.TwoValue = (float)Math.Sin((double)(this.counter * 2f));
			}
		}

		// Token: 0x0400051A RID: 1306
		public float Frequency = 1f;

		// Token: 0x0400051B RID: 1307
		public float Rate = 1f;

		// Token: 0x0400051F RID: 1311
		public Action<float> OnUpdate;

		// Token: 0x04000520 RID: 1312
		public bool UseRawDeltaTime;

		// Token: 0x04000521 RID: 1313
		private float counter;
	}
}
