using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Celeste
{
	// Token: 0x0200025C RID: 604
	[Serializable]
	public class AudioTrackState
	{
		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060012C7 RID: 4807 RVA: 0x00065B3C File Offset: 0x00063D3C
		// (set) Token: 0x060012C8 RID: 4808 RVA: 0x00065B44 File Offset: 0x00063D44
		[XmlAttribute]
		public string Event
		{
			get
			{
				return this.ev;
			}
			set
			{
				if (this.ev != value)
				{
					this.ev = value;
					this.Parameters.Clear();
				}
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060012C9 RID: 4809 RVA: 0x00065B66 File Offset: 0x00063D66
		// (set) Token: 0x060012CA RID: 4810 RVA: 0x00065B74 File Offset: 0x00063D74
		[XmlIgnore]
		public int Progress
		{
			get
			{
				return (int)this.GetParam("progress");
			}
			set
			{
				this.Param("progress", (float)value);
			}
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x00065B84 File Offset: 0x00063D84
		public AudioTrackState()
		{
		}

		// Token: 0x060012CC RID: 4812 RVA: 0x00065B97 File Offset: 0x00063D97
		public AudioTrackState(string ev)
		{
			this.Event = ev;
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x00065BB1 File Offset: 0x00063DB1
		public AudioTrackState Layer(int index, float value)
		{
			return this.Param(AudioState.LayerParameters[index], value);
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x00065BC1 File Offset: 0x00063DC1
		public AudioTrackState Layer(int index, bool value)
		{
			return this.Param(AudioState.LayerParameters[index], value);
		}

		// Token: 0x060012CF RID: 4815 RVA: 0x00065BD1 File Offset: 0x00063DD1
		public AudioTrackState SetProgress(int value)
		{
			this.Progress = value;
			return this;
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x00065BDC File Offset: 0x00063DDC
		public AudioTrackState Param(string key, float value)
		{
			foreach (MEP mep in this.Parameters)
			{
				if (mep.Key != null && mep.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))
				{
					mep.Value = value;
					return this;
				}
			}
			this.Parameters.Add(new MEP(key, value));
			return this;
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x00065C60 File Offset: 0x00063E60
		public AudioTrackState Param(string key, bool value)
		{
			return this.Param(key, (float)(value ? 1 : 0));
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x00065C74 File Offset: 0x00063E74
		public float GetParam(string key)
		{
			foreach (MEP mep in this.Parameters)
			{
				if (mep.Key != null && mep.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))
				{
					return mep.Value;
				}
			}
			return 0f;
		}

		// Token: 0x060012D3 RID: 4819 RVA: 0x00065CE8 File Offset: 0x00063EE8
		public AudioTrackState Clone()
		{
			AudioTrackState audioTrackState = new AudioTrackState();
			audioTrackState.Event = this.Event;
			foreach (MEP mep in this.Parameters)
			{
				audioTrackState.Parameters.Add(new MEP(mep.Key, mep.Value));
			}
			return audioTrackState;
		}

		// Token: 0x04000EC5 RID: 3781
		[XmlIgnore]
		private string ev;

		// Token: 0x04000EC6 RID: 3782
		public List<MEP> Parameters = new List<MEP>();
	}
}
