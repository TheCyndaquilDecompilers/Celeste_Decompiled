using System;
using FMOD.Studio;
using Monocle;

namespace Celeste
{
	// Token: 0x02000143 RID: 323
	[Tracked(false)]
	public class CassetteBlockManager : Entity
	{
		// Token: 0x06000BC4 RID: 3012 RVA: 0x00023B1C File Offset: 0x00021D1C
		public CassetteBlockManager()
		{
			base.Tag = Tags.Global;
			base.Add(new TransitionListener
			{
				OnOutBegin = delegate()
				{
					if (!base.SceneAs<Level>().HasCassetteBlocks)
					{
						base.RemoveSelf();
						return;
					}
					this.maxBeat = base.SceneAs<Level>().CassetteBlockBeats;
					this.tempoMult = base.SceneAs<Level>().CassetteBlockTempo;
				}
			});
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x00023B60 File Offset: 0x00021D60
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			this.isLevelMusic = (AreaData.Areas[base.SceneAs<Level>().Session.Area.ID].CassetteSong == null);
			if (this.isLevelMusic)
			{
				this.leadBeats = 0;
				this.beatIndexOffset = 5;
			}
			else
			{
				this.beatIndexOffset = 0;
				this.leadBeats = 16;
				this.snapshot = Audio.CreateSnapshot("snapshot:/music_mains_mute", true);
			}
			this.maxBeat = base.SceneAs<Level>().CassetteBlockBeats;
			this.tempoMult = base.SceneAs<Level>().CassetteBlockTempo;
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x00023BFB File Offset: 0x00021DFB
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			if (!this.isLevelMusic)
			{
				Audio.Stop(this.snapshot, true);
				Audio.Stop(this.sfx, true);
			}
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x00023C24 File Offset: 0x00021E24
		public override void SceneEnd(Scene scene)
		{
			base.SceneEnd(scene);
			if (!this.isLevelMusic)
			{
				Audio.Stop(this.snapshot, true);
				Audio.Stop(this.sfx, true);
			}
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x00023C50 File Offset: 0x00021E50
		public override void Update()
		{
			base.Update();
			if (this.isLevelMusic)
			{
				this.sfx = Audio.CurrentMusicEventInstance;
			}
			if (this.sfx == null && !this.isLevelMusic)
			{
				string cassetteSong = AreaData.Areas[base.SceneAs<Level>().Session.Area.ID].CassetteSong;
				this.sfx = Audio.CreateInstance(cassetteSong, null);
				Audio.Play("event:/game/general/cassette_block_switch_2");
				return;
			}
			this.AdvanceMusic(Engine.DeltaTime * this.tempoMult);
		}

		// Token: 0x06000BC9 RID: 3017 RVA: 0x00023CE4 File Offset: 0x00021EE4
		public void AdvanceMusic(float time)
		{
			float num = this.beatTimer;
			this.beatTimer += time;
			if (this.beatTimer >= 0.16666667f)
			{
				this.beatTimer -= 0.16666667f;
				this.beatIndex++;
				this.beatIndex %= 256;
				if (this.beatIndex % 8 == 0)
				{
					this.currentIndex++;
					this.currentIndex %= this.maxBeat;
					this.SetActiveIndex(this.currentIndex);
					if (!this.isLevelMusic)
					{
						Audio.Play("event:/game/general/cassette_block_switch_2");
					}
					Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
				}
				else if ((this.beatIndex + 1) % 8 == 0)
				{
					this.SetWillActivate((this.currentIndex + 1) % this.maxBeat);
				}
				else if ((this.beatIndex + 4) % 8 == 0 && !this.isLevelMusic)
				{
					Audio.Play("event:/game/general/cassette_block_switch_1");
				}
				if (this.leadBeats > 0)
				{
					this.leadBeats--;
					if (this.leadBeats == 0)
					{
						this.beatIndex = 0;
						if (!this.isLevelMusic)
						{
							this.sfx.start();
						}
					}
				}
				if (this.leadBeats <= 0)
				{
					this.sfx.setParameterValue("sixteenth_note", (float)this.GetSixteenthNote());
				}
			}
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x00023E38 File Offset: 0x00022038
		public int GetSixteenthNote()
		{
			return (this.beatIndex + this.beatIndexOffset) % 256 + 1;
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x00023E50 File Offset: 0x00022050
		public void StopBlocks()
		{
			foreach (Entity entity in base.Scene.Tracker.GetEntities<CassetteBlock>())
			{
				((CassetteBlock)entity).Finish();
			}
			if (!this.isLevelMusic)
			{
				Audio.Stop(this.sfx, true);
			}
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x00023EC4 File Offset: 0x000220C4
		public void Finish()
		{
			if (!this.isLevelMusic)
			{
				Audio.Stop(this.snapshot, true);
			}
			base.RemoveSelf();
		}

		// Token: 0x06000BCD RID: 3021 RVA: 0x00023EE0 File Offset: 0x000220E0
		public void OnLevelStart()
		{
			this.maxBeat = base.SceneAs<Level>().CassetteBlockBeats;
			this.tempoMult = base.SceneAs<Level>().CassetteBlockTempo;
			if (this.beatIndex % 8 >= 5)
			{
				this.currentIndex = this.maxBeat - 2;
			}
			else
			{
				this.currentIndex = this.maxBeat - 1;
			}
			this.SilentUpdateBlocks();
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x00023F40 File Offset: 0x00022140
		private void SilentUpdateBlocks()
		{
			foreach (Entity entity in base.Scene.Tracker.GetEntities<CassetteBlock>())
			{
				CassetteBlock cassetteBlock = (CassetteBlock)entity;
				if (cassetteBlock.ID.Level == base.SceneAs<Level>().Session.Level)
				{
					cassetteBlock.SetActivatedSilently(cassetteBlock.Index == this.currentIndex);
				}
			}
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x00023FD4 File Offset: 0x000221D4
		public void SetActiveIndex(int index)
		{
			foreach (Entity entity in base.Scene.Tracker.GetEntities<CassetteBlock>())
			{
				CassetteBlock cassetteBlock = (CassetteBlock)entity;
				cassetteBlock.Activated = (cassetteBlock.Index == index);
			}
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x0002403C File Offset: 0x0002223C
		public void SetWillActivate(int index)
		{
			foreach (Entity entity in base.Scene.Tracker.GetEntities<CassetteBlock>())
			{
				CassetteBlock cassetteBlock = (CassetteBlock)entity;
				if (cassetteBlock.Index == index || cassetteBlock.Activated)
				{
					cassetteBlock.WillToggle();
				}
			}
		}

		// Token: 0x04000720 RID: 1824
		private int currentIndex;

		// Token: 0x04000721 RID: 1825
		private float beatTimer;

		// Token: 0x04000722 RID: 1826
		private int beatIndex;

		// Token: 0x04000723 RID: 1827
		private float tempoMult;

		// Token: 0x04000724 RID: 1828
		private int leadBeats;

		// Token: 0x04000725 RID: 1829
		private int maxBeat;

		// Token: 0x04000726 RID: 1830
		private bool isLevelMusic;

		// Token: 0x04000727 RID: 1831
		private int beatIndexOffset;

		// Token: 0x04000728 RID: 1832
		private EventInstance sfx;

		// Token: 0x04000729 RID: 1833
		private EventInstance snapshot;
	}
}
