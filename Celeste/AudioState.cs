using System;

namespace Celeste
{
	// Token: 0x0200025D RID: 605
	[Serializable]
	public class AudioState
	{
		// Token: 0x060012D4 RID: 4820 RVA: 0x00065D64 File Offset: 0x00063F64
		public AudioState()
		{
		}

		// Token: 0x060012D5 RID: 4821 RVA: 0x00065D82 File Offset: 0x00063F82
		public AudioState(AudioTrackState music, AudioTrackState ambience)
		{
			if (music != null)
			{
				this.Music = music.Clone();
			}
			if (ambience != null)
			{
				this.Ambience = ambience.Clone();
			}
		}

		// Token: 0x060012D6 RID: 4822 RVA: 0x00065DBE File Offset: 0x00063FBE
		public AudioState(string music, string ambience)
		{
			this.Music.Event = music;
			this.Ambience.Event = ambience;
		}

		// Token: 0x060012D7 RID: 4823 RVA: 0x00065DF4 File Offset: 0x00063FF4
		public void Apply(bool forceSixteenthNoteHack = false)
		{
			bool flag = Audio.SetMusic(this.Music.Event, false, true);
			if (Audio.CurrentMusicEventInstance != null)
			{
				foreach (MEP mep in this.Music.Parameters)
				{
					if (!(mep.Key == "sixteenth_note") || forceSixteenthNoteHack)
					{
						Audio.SetParameter(Audio.CurrentMusicEventInstance, mep.Key, mep.Value);
					}
				}
				if (flag)
				{
					Audio.CurrentMusicEventInstance.start();
				}
			}
			flag = Audio.SetAmbience(this.Ambience.Event, false);
			if (Audio.CurrentAmbienceEventInstance != null)
			{
				foreach (MEP mep2 in this.Ambience.Parameters)
				{
					Audio.SetParameter(Audio.CurrentAmbienceEventInstance, mep2.Key, mep2.Value);
				}
				if (flag)
				{
					Audio.CurrentAmbienceEventInstance.start();
				}
			}
		}

		// Token: 0x060012D8 RID: 4824 RVA: 0x00065F24 File Offset: 0x00064124
		public void Stop(bool allowFadeOut = true)
		{
			Audio.SetMusic(null, false, allowFadeOut);
			Audio.SetAmbience(null, true);
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x00065F37 File Offset: 0x00064137
		public AudioState Clone()
		{
			return new AudioState
			{
				Music = this.Music.Clone(),
				Ambience = this.Ambience.Clone()
			};
		}

		// Token: 0x04000EC7 RID: 3783
		public static string[] LayerParameters = new string[]
		{
			"layer0",
			"layer1",
			"layer2",
			"layer3",
			"layer4",
			"layer5",
			"layer6",
			"layer7",
			"layer8",
			"layer9"
		};

		// Token: 0x04000EC8 RID: 3784
		public AudioTrackState Music = new AudioTrackState();

		// Token: 0x04000EC9 RID: 3785
		public AudioTrackState Ambience = new AudioTrackState();
	}
}
