using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x02000043 RID: 67
	public class ChannelControl : HandleBase
	{
		// Token: 0x06000194 RID: 404 RVA: 0x00003D84 File Offset: 0x00001F84
		public RESULT getSystemObject(out System system)
		{
			system = null;
			IntPtr raw;
			RESULT result = ChannelControl.FMOD_ChannelGroup_GetSystemObject(this.rawPtr, out raw);
			system = new System(raw);
			return result;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00003DA9 File Offset: 0x00001FA9
		public RESULT stop()
		{
			return ChannelControl.FMOD_ChannelGroup_Stop(this.rawPtr);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00003DB6 File Offset: 0x00001FB6
		public RESULT setPaused(bool paused)
		{
			return ChannelControl.FMOD_ChannelGroup_SetPaused(this.rawPtr, paused);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00003DC4 File Offset: 0x00001FC4
		public RESULT getPaused(out bool paused)
		{
			return ChannelControl.FMOD_ChannelGroup_GetPaused(this.rawPtr, out paused);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00003DD2 File Offset: 0x00001FD2
		public RESULT setVolume(float volume)
		{
			return ChannelControl.FMOD_ChannelGroup_SetVolume(this.rawPtr, volume);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00003DE0 File Offset: 0x00001FE0
		public RESULT getVolume(out float volume)
		{
			return ChannelControl.FMOD_ChannelGroup_GetVolume(this.rawPtr, out volume);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00003DEE File Offset: 0x00001FEE
		public RESULT setVolumeRamp(bool ramp)
		{
			return ChannelControl.FMOD_ChannelGroup_SetVolumeRamp(this.rawPtr, ramp);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00003DFC File Offset: 0x00001FFC
		public RESULT getVolumeRamp(out bool ramp)
		{
			return ChannelControl.FMOD_ChannelGroup_GetVolumeRamp(this.rawPtr, out ramp);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00003E0A File Offset: 0x0000200A
		public RESULT getAudibility(out float audibility)
		{
			return ChannelControl.FMOD_ChannelGroup_GetAudibility(this.rawPtr, out audibility);
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00003E18 File Offset: 0x00002018
		public RESULT setPitch(float pitch)
		{
			return ChannelControl.FMOD_ChannelGroup_SetPitch(this.rawPtr, pitch);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x00003E26 File Offset: 0x00002026
		public RESULT getPitch(out float pitch)
		{
			return ChannelControl.FMOD_ChannelGroup_GetPitch(this.rawPtr, out pitch);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00003E34 File Offset: 0x00002034
		public RESULT setMute(bool mute)
		{
			return ChannelControl.FMOD_ChannelGroup_SetMute(this.rawPtr, mute);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00003E42 File Offset: 0x00002042
		public RESULT getMute(out bool mute)
		{
			return ChannelControl.FMOD_ChannelGroup_GetMute(this.rawPtr, out mute);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00003E50 File Offset: 0x00002050
		public RESULT setReverbProperties(int instance, float wet)
		{
			return ChannelControl.FMOD_ChannelGroup_SetReverbProperties(this.rawPtr, instance, wet);
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00003E5F File Offset: 0x0000205F
		public RESULT getReverbProperties(int instance, out float wet)
		{
			return ChannelControl.FMOD_ChannelGroup_GetReverbProperties(this.rawPtr, instance, out wet);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00003E6E File Offset: 0x0000206E
		public RESULT setLowPassGain(float gain)
		{
			return ChannelControl.FMOD_ChannelGroup_SetLowPassGain(this.rawPtr, gain);
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00003E7C File Offset: 0x0000207C
		public RESULT getLowPassGain(out float gain)
		{
			return ChannelControl.FMOD_ChannelGroup_GetLowPassGain(this.rawPtr, out gain);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x00003E8A File Offset: 0x0000208A
		public RESULT setMode(MODE mode)
		{
			return ChannelControl.FMOD_ChannelGroup_SetMode(this.rawPtr, mode);
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00003E98 File Offset: 0x00002098
		public RESULT getMode(out MODE mode)
		{
			return ChannelControl.FMOD_ChannelGroup_GetMode(this.rawPtr, out mode);
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00003EA6 File Offset: 0x000020A6
		public RESULT setCallback(CHANNEL_CALLBACK callback)
		{
			return ChannelControl.FMOD_ChannelGroup_SetCallback(this.rawPtr, callback);
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00003EB4 File Offset: 0x000020B4
		public RESULT isPlaying(out bool isplaying)
		{
			return ChannelControl.FMOD_ChannelGroup_IsPlaying(this.rawPtr, out isplaying);
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00003EC2 File Offset: 0x000020C2
		public RESULT setPan(float pan)
		{
			return ChannelControl.FMOD_ChannelGroup_SetPan(this.rawPtr, pan);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00003ED0 File Offset: 0x000020D0
		public RESULT setMixLevelsOutput(float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright)
		{
			return ChannelControl.FMOD_ChannelGroup_SetMixLevelsOutput(this.rawPtr, frontleft, frontright, center, lfe, surroundleft, surroundright, backleft, backright);
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00003EF5 File Offset: 0x000020F5
		public RESULT setMixLevelsInput(float[] levels, int numlevels)
		{
			return ChannelControl.FMOD_ChannelGroup_SetMixLevelsInput(this.rawPtr, levels, numlevels);
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00003F04 File Offset: 0x00002104
		public RESULT setMixMatrix(float[] matrix, int outchannels, int inchannels, int inchannel_hop = 0)
		{
			return ChannelControl.FMOD_ChannelGroup_SetMixMatrix(this.rawPtr, matrix, outchannels, inchannels, inchannel_hop);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00003F16 File Offset: 0x00002116
		public RESULT getMixMatrix(float[] matrix, out int outchannels, out int inchannels, int inchannel_hop = 0)
		{
			return ChannelControl.FMOD_ChannelGroup_GetMixMatrix(this.rawPtr, matrix, out outchannels, out inchannels, inchannel_hop);
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00003F28 File Offset: 0x00002128
		public RESULT getDSPClock(out ulong dspclock, out ulong parentclock)
		{
			return ChannelControl.FMOD_ChannelGroup_GetDSPClock(this.rawPtr, out dspclock, out parentclock);
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00003F37 File Offset: 0x00002137
		public RESULT setDelay(ulong dspclock_start, ulong dspclock_end, bool stopchannels = true)
		{
			return ChannelControl.FMOD_ChannelGroup_SetDelay(this.rawPtr, dspclock_start, dspclock_end, stopchannels);
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00003F47 File Offset: 0x00002147
		public RESULT getDelay(out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels)
		{
			return ChannelControl.FMOD_ChannelGroup_GetDelay(this.rawPtr, out dspclock_start, out dspclock_end, out stopchannels);
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00003F57 File Offset: 0x00002157
		public RESULT addFadePoint(ulong dspclock, float volume)
		{
			return ChannelControl.FMOD_ChannelGroup_AddFadePoint(this.rawPtr, dspclock, volume);
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00003F66 File Offset: 0x00002166
		public RESULT setFadePointRamp(ulong dspclock, float volume)
		{
			return ChannelControl.FMOD_ChannelGroup_SetFadePointRamp(this.rawPtr, dspclock, volume);
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x00003F75 File Offset: 0x00002175
		public RESULT removeFadePoints(ulong dspclock_start, ulong dspclock_end)
		{
			return ChannelControl.FMOD_ChannelGroup_RemoveFadePoints(this.rawPtr, dspclock_start, dspclock_end);
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00003F84 File Offset: 0x00002184
		public RESULT getFadePoints(ref uint numpoints, ulong[] point_dspclock, float[] point_volume)
		{
			return ChannelControl.FMOD_ChannelGroup_GetFadePoints(this.rawPtr, ref numpoints, point_dspclock, point_volume);
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00003F94 File Offset: 0x00002194
		public RESULT getDSP(int index, out DSP dsp)
		{
			dsp = null;
			IntPtr raw;
			RESULT result = ChannelControl.FMOD_ChannelGroup_GetDSP(this.rawPtr, index, out raw);
			dsp = new DSP(raw);
			return result;
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x00003FBA File Offset: 0x000021BA
		public RESULT addDSP(int index, DSP dsp)
		{
			return ChannelControl.FMOD_ChannelGroup_AddDSP(this.rawPtr, index, dsp.getRaw());
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x00003FCE File Offset: 0x000021CE
		public RESULT removeDSP(DSP dsp)
		{
			return ChannelControl.FMOD_ChannelGroup_RemoveDSP(this.rawPtr, dsp.getRaw());
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00003FE1 File Offset: 0x000021E1
		public RESULT getNumDSPs(out int numdsps)
		{
			return ChannelControl.FMOD_ChannelGroup_GetNumDSPs(this.rawPtr, out numdsps);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00003FEF File Offset: 0x000021EF
		public RESULT setDSPIndex(DSP dsp, int index)
		{
			return ChannelControl.FMOD_ChannelGroup_SetDSPIndex(this.rawPtr, dsp.getRaw(), index);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00004003 File Offset: 0x00002203
		public RESULT getDSPIndex(DSP dsp, out int index)
		{
			return ChannelControl.FMOD_ChannelGroup_GetDSPIndex(this.rawPtr, dsp.getRaw(), out index);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00004017 File Offset: 0x00002217
		public RESULT set3DAttributes(ref VECTOR pos, ref VECTOR vel, ref VECTOR alt_pan_pos)
		{
			return ChannelControl.FMOD_ChannelGroup_Set3DAttributes(this.rawPtr, ref pos, ref vel, ref alt_pan_pos);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00004027 File Offset: 0x00002227
		public RESULT get3DAttributes(out VECTOR pos, out VECTOR vel, out VECTOR alt_pan_pos)
		{
			return ChannelControl.FMOD_ChannelGroup_Get3DAttributes(this.rawPtr, out pos, out vel, out alt_pan_pos);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00004037 File Offset: 0x00002237
		public RESULT set3DMinMaxDistance(float mindistance, float maxdistance)
		{
			return ChannelControl.FMOD_ChannelGroup_Set3DMinMaxDistance(this.rawPtr, mindistance, maxdistance);
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00004046 File Offset: 0x00002246
		public RESULT get3DMinMaxDistance(out float mindistance, out float maxdistance)
		{
			return ChannelControl.FMOD_ChannelGroup_Get3DMinMaxDistance(this.rawPtr, out mindistance, out maxdistance);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00004055 File Offset: 0x00002255
		public RESULT set3DConeSettings(float insideconeangle, float outsideconeangle, float outsidevolume)
		{
			return ChannelControl.FMOD_ChannelGroup_Set3DConeSettings(this.rawPtr, insideconeangle, outsideconeangle, outsidevolume);
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00004065 File Offset: 0x00002265
		public RESULT get3DConeSettings(out float insideconeangle, out float outsideconeangle, out float outsidevolume)
		{
			return ChannelControl.FMOD_ChannelGroup_Get3DConeSettings(this.rawPtr, out insideconeangle, out outsideconeangle, out outsidevolume);
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x00004075 File Offset: 0x00002275
		public RESULT set3DConeOrientation(ref VECTOR orientation)
		{
			return ChannelControl.FMOD_ChannelGroup_Set3DConeOrientation(this.rawPtr, ref orientation);
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x00004083 File Offset: 0x00002283
		public RESULT get3DConeOrientation(out VECTOR orientation)
		{
			return ChannelControl.FMOD_ChannelGroup_Get3DConeOrientation(this.rawPtr, out orientation);
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x00004091 File Offset: 0x00002291
		public RESULT set3DCustomRolloff(ref VECTOR points, int numpoints)
		{
			return ChannelControl.FMOD_ChannelGroup_Set3DCustomRolloff(this.rawPtr, ref points, numpoints);
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x000040A0 File Offset: 0x000022A0
		public RESULT get3DCustomRolloff(out IntPtr points, out int numpoints)
		{
			return ChannelControl.FMOD_ChannelGroup_Get3DCustomRolloff(this.rawPtr, out points, out numpoints);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x000040AF File Offset: 0x000022AF
		public RESULT set3DOcclusion(float directocclusion, float reverbocclusion)
		{
			return ChannelControl.FMOD_ChannelGroup_Set3DOcclusion(this.rawPtr, directocclusion, reverbocclusion);
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x000040BE File Offset: 0x000022BE
		public RESULT get3DOcclusion(out float directocclusion, out float reverbocclusion)
		{
			return ChannelControl.FMOD_ChannelGroup_Get3DOcclusion(this.rawPtr, out directocclusion, out reverbocclusion);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x000040CD File Offset: 0x000022CD
		public RESULT set3DSpread(float angle)
		{
			return ChannelControl.FMOD_ChannelGroup_Set3DSpread(this.rawPtr, angle);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x000040DB File Offset: 0x000022DB
		public RESULT get3DSpread(out float angle)
		{
			return ChannelControl.FMOD_ChannelGroup_Get3DSpread(this.rawPtr, out angle);
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x000040E9 File Offset: 0x000022E9
		public RESULT set3DLevel(float level)
		{
			return ChannelControl.FMOD_ChannelGroup_Set3DLevel(this.rawPtr, level);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x000040F7 File Offset: 0x000022F7
		public RESULT get3DLevel(out float level)
		{
			return ChannelControl.FMOD_ChannelGroup_Get3DLevel(this.rawPtr, out level);
		}

		// Token: 0x060001CB RID: 459 RVA: 0x00004105 File Offset: 0x00002305
		public RESULT set3DDopplerLevel(float level)
		{
			return ChannelControl.FMOD_ChannelGroup_Set3DDopplerLevel(this.rawPtr, level);
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00004113 File Offset: 0x00002313
		public RESULT get3DDopplerLevel(out float level)
		{
			return ChannelControl.FMOD_ChannelGroup_Get3DDopplerLevel(this.rawPtr, out level);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00004121 File Offset: 0x00002321
		public RESULT set3DDistanceFilter(bool custom, float customLevel, float centerFreq)
		{
			return ChannelControl.FMOD_ChannelGroup_Set3DDistanceFilter(this.rawPtr, custom, customLevel, centerFreq);
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00004131 File Offset: 0x00002331
		public RESULT get3DDistanceFilter(out bool custom, out float customLevel, out float centerFreq)
		{
			return ChannelControl.FMOD_ChannelGroup_Get3DDistanceFilter(this.rawPtr, out custom, out customLevel, out centerFreq);
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00004141 File Offset: 0x00002341
		public RESULT setUserData(IntPtr userdata)
		{
			return ChannelControl.FMOD_ChannelGroup_SetUserData(this.rawPtr, userdata);
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000414F File Offset: 0x0000234F
		public RESULT getUserData(out IntPtr userdata)
		{
			return ChannelControl.FMOD_ChannelGroup_GetUserData(this.rawPtr, out userdata);
		}

		// Token: 0x060001D1 RID: 465
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Stop(IntPtr channelgroup);

		// Token: 0x060001D2 RID: 466
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetPaused(IntPtr channelgroup, bool paused);

		// Token: 0x060001D3 RID: 467
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetPaused(IntPtr channelgroup, out bool paused);

		// Token: 0x060001D4 RID: 468
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetVolume(IntPtr channelgroup, out float volume);

		// Token: 0x060001D5 RID: 469
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetVolumeRamp(IntPtr channelgroup, bool ramp);

		// Token: 0x060001D6 RID: 470
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetVolumeRamp(IntPtr channelgroup, out bool ramp);

		// Token: 0x060001D7 RID: 471
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetAudibility(IntPtr channelgroup, out float audibility);

		// Token: 0x060001D8 RID: 472
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetPitch(IntPtr channelgroup, float pitch);

		// Token: 0x060001D9 RID: 473
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetPitch(IntPtr channelgroup, out float pitch);

		// Token: 0x060001DA RID: 474
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetMute(IntPtr channelgroup, bool mute);

		// Token: 0x060001DB RID: 475
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetMute(IntPtr channelgroup, out bool mute);

		// Token: 0x060001DC RID: 476
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetReverbProperties(IntPtr channelgroup, int instance, float wet);

		// Token: 0x060001DD RID: 477
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetReverbProperties(IntPtr channelgroup, int instance, out float wet);

		// Token: 0x060001DE RID: 478
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetLowPassGain(IntPtr channelgroup, float gain);

		// Token: 0x060001DF RID: 479
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetLowPassGain(IntPtr channelgroup, out float gain);

		// Token: 0x060001E0 RID: 480
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetMode(IntPtr channelgroup, MODE mode);

		// Token: 0x060001E1 RID: 481
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetMode(IntPtr channelgroup, out MODE mode);

		// Token: 0x060001E2 RID: 482
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetCallback(IntPtr channelgroup, CHANNEL_CALLBACK callback);

		// Token: 0x060001E3 RID: 483
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_IsPlaying(IntPtr channelgroup, out bool isplaying);

		// Token: 0x060001E4 RID: 484
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetPan(IntPtr channelgroup, float pan);

		// Token: 0x060001E5 RID: 485
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetMixLevelsOutput(IntPtr channelgroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);

		// Token: 0x060001E6 RID: 486
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetMixLevelsInput(IntPtr channelgroup, float[] levels, int numlevels);

		// Token: 0x060001E7 RID: 487
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetMixMatrix(IntPtr channelgroup, float[] matrix, int outchannels, int inchannels, int inchannel_hop);

		// Token: 0x060001E8 RID: 488
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetMixMatrix(IntPtr channelgroup, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);

		// Token: 0x060001E9 RID: 489
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetDSPClock(IntPtr channelgroup, out ulong dspclock, out ulong parentclock);

		// Token: 0x060001EA RID: 490
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetDelay(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end, bool stopchannels);

		// Token: 0x060001EB RID: 491
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetDelay(IntPtr channelgroup, out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels);

		// Token: 0x060001EC RID: 492
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_AddFadePoint(IntPtr channelgroup, ulong dspclock, float volume);

		// Token: 0x060001ED RID: 493
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetFadePointRamp(IntPtr channelgroup, ulong dspclock, float volume);

		// Token: 0x060001EE RID: 494
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_RemoveFadePoints(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end);

		// Token: 0x060001EF RID: 495
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetFadePoints(IntPtr channelgroup, ref uint numpoints, ulong[] point_dspclock, float[] point_volume);

		// Token: 0x060001F0 RID: 496
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Set3DAttributes(IntPtr channelgroup, ref VECTOR pos, ref VECTOR vel, ref VECTOR alt_pan_pos);

		// Token: 0x060001F1 RID: 497
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Get3DAttributes(IntPtr channelgroup, out VECTOR pos, out VECTOR vel, out VECTOR alt_pan_pos);

		// Token: 0x060001F2 RID: 498
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Set3DMinMaxDistance(IntPtr channelgroup, float mindistance, float maxdistance);

		// Token: 0x060001F3 RID: 499
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Get3DMinMaxDistance(IntPtr channelgroup, out float mindistance, out float maxdistance);

		// Token: 0x060001F4 RID: 500
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Set3DConeSettings(IntPtr channelgroup, float insideconeangle, float outsideconeangle, float outsidevolume);

		// Token: 0x060001F5 RID: 501
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Get3DConeSettings(IntPtr channelgroup, out float insideconeangle, out float outsideconeangle, out float outsidevolume);

		// Token: 0x060001F6 RID: 502
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Set3DConeOrientation(IntPtr channelgroup, ref VECTOR orientation);

		// Token: 0x060001F7 RID: 503
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Get3DConeOrientation(IntPtr channelgroup, out VECTOR orientation);

		// Token: 0x060001F8 RID: 504
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Set3DCustomRolloff(IntPtr channelgroup, ref VECTOR points, int numpoints);

		// Token: 0x060001F9 RID: 505
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Get3DCustomRolloff(IntPtr channelgroup, out IntPtr points, out int numpoints);

		// Token: 0x060001FA RID: 506
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Set3DOcclusion(IntPtr channelgroup, float directocclusion, float reverbocclusion);

		// Token: 0x060001FB RID: 507
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Get3DOcclusion(IntPtr channelgroup, out float directocclusion, out float reverbocclusion);

		// Token: 0x060001FC RID: 508
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Set3DSpread(IntPtr channelgroup, float angle);

		// Token: 0x060001FD RID: 509
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Get3DSpread(IntPtr channelgroup, out float angle);

		// Token: 0x060001FE RID: 510
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Set3DLevel(IntPtr channelgroup, float level);

		// Token: 0x060001FF RID: 511
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Get3DLevel(IntPtr channelgroup, out float level);

		// Token: 0x06000200 RID: 512
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Set3DDopplerLevel(IntPtr channelgroup, float level);

		// Token: 0x06000201 RID: 513
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Get3DDopplerLevel(IntPtr channelgroup, out float level);

		// Token: 0x06000202 RID: 514
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Set3DDistanceFilter(IntPtr channelgroup, bool custom, float customLevel, float centerFreq);

		// Token: 0x06000203 RID: 515
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Get3DDistanceFilter(IntPtr channelgroup, out bool custom, out float customLevel, out float centerFreq);

		// Token: 0x06000204 RID: 516
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetSystemObject(IntPtr channelgroup, out IntPtr system);

		// Token: 0x06000205 RID: 517
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetVolume(IntPtr channelgroup, float volume);

		// Token: 0x06000206 RID: 518
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetDSP(IntPtr channelgroup, int index, out IntPtr dsp);

		// Token: 0x06000207 RID: 519
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_AddDSP(IntPtr channelgroup, int index, IntPtr dsp);

		// Token: 0x06000208 RID: 520
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_RemoveDSP(IntPtr channelgroup, IntPtr dsp);

		// Token: 0x06000209 RID: 521
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetNumDSPs(IntPtr channelgroup, out int numdsps);

		// Token: 0x0600020A RID: 522
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetDSPIndex(IntPtr channelgroup, IntPtr dsp, int index);

		// Token: 0x0600020B RID: 523
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetDSPIndex(IntPtr channelgroup, IntPtr dsp, out int index);

		// Token: 0x0600020C RID: 524
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_SetUserData(IntPtr channelgroup, IntPtr userdata);

		// Token: 0x0600020D RID: 525
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetUserData(IntPtr channelgroup, out IntPtr userdata);

		// Token: 0x0600020E RID: 526 RVA: 0x00003A27 File Offset: 0x00001C27
		protected ChannelControl(IntPtr raw) : base(raw)
		{
		}
	}
}
