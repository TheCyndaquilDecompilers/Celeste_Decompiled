using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	// Token: 0x02000042 RID: 66
	public class Sound : HandleBase
	{
		// Token: 0x0600013D RID: 317 RVA: 0x00003A30 File Offset: 0x00001C30
		public RESULT release()
		{
			RESULT result = Sound.FMOD_Sound_Release(this.rawPtr);
			if (result == RESULT.OK)
			{
				this.rawPtr = IntPtr.Zero;
			}
			return result;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00003A4C File Offset: 0x00001C4C
		public RESULT getSystemObject(out System system)
		{
			system = null;
			IntPtr raw;
			RESULT result = Sound.FMOD_Sound_GetSystemObject(this.rawPtr, out raw);
			system = new System(raw);
			return result;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00003A71 File Offset: 0x00001C71
		public RESULT @lock(uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2)
		{
			return Sound.FMOD_Sound_Lock(this.rawPtr, offset, length, out ptr1, out ptr2, out len1, out len2);
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00003A87 File Offset: 0x00001C87
		public RESULT unlock(IntPtr ptr1, IntPtr ptr2, uint len1, uint len2)
		{
			return Sound.FMOD_Sound_Unlock(this.rawPtr, ptr1, ptr2, len1, len2);
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00003A99 File Offset: 0x00001C99
		public RESULT setDefaults(float frequency, int priority)
		{
			return Sound.FMOD_Sound_SetDefaults(this.rawPtr, frequency, priority);
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00003AA8 File Offset: 0x00001CA8
		public RESULT getDefaults(out float frequency, out int priority)
		{
			return Sound.FMOD_Sound_GetDefaults(this.rawPtr, out frequency, out priority);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00003AB7 File Offset: 0x00001CB7
		public RESULT set3DMinMaxDistance(float min, float max)
		{
			return Sound.FMOD_Sound_Set3DMinMaxDistance(this.rawPtr, min, max);
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00003AC6 File Offset: 0x00001CC6
		public RESULT get3DMinMaxDistance(out float min, out float max)
		{
			return Sound.FMOD_Sound_Get3DMinMaxDistance(this.rawPtr, out min, out max);
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00003AD5 File Offset: 0x00001CD5
		public RESULT set3DConeSettings(float insideconeangle, float outsideconeangle, float outsidevolume)
		{
			return Sound.FMOD_Sound_Set3DConeSettings(this.rawPtr, insideconeangle, outsideconeangle, outsidevolume);
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00003AE5 File Offset: 0x00001CE5
		public RESULT get3DConeSettings(out float insideconeangle, out float outsideconeangle, out float outsidevolume)
		{
			return Sound.FMOD_Sound_Get3DConeSettings(this.rawPtr, out insideconeangle, out outsideconeangle, out outsidevolume);
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00003AF5 File Offset: 0x00001CF5
		public RESULT set3DCustomRolloff(ref VECTOR points, int numpoints)
		{
			return Sound.FMOD_Sound_Set3DCustomRolloff(this.rawPtr, ref points, numpoints);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00003B04 File Offset: 0x00001D04
		public RESULT get3DCustomRolloff(out IntPtr points, out int numpoints)
		{
			return Sound.FMOD_Sound_Get3DCustomRolloff(this.rawPtr, out points, out numpoints);
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00003B14 File Offset: 0x00001D14
		public RESULT getSubSound(int index, out Sound subsound)
		{
			subsound = null;
			IntPtr raw;
			RESULT result = Sound.FMOD_Sound_GetSubSound(this.rawPtr, index, out raw);
			subsound = new Sound(raw);
			return result;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00003B3C File Offset: 0x00001D3C
		public RESULT getSubSoundParent(out Sound parentsound)
		{
			parentsound = null;
			IntPtr raw;
			RESULT result = Sound.FMOD_Sound_GetSubSoundParent(this.rawPtr, out raw);
			parentsound = new Sound(raw);
			return result;
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00003B64 File Offset: 0x00001D64
		public RESULT getName(StringBuilder name, int namelen)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(name.Capacity);
			RESULT result = Sound.FMOD_Sound_GetName(this.rawPtr, intPtr, namelen);
			StringMarshalHelper.NativeToBuilder(name, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00003B97 File Offset: 0x00001D97
		public RESULT getLength(out uint length, TIMEUNIT lengthtype)
		{
			return Sound.FMOD_Sound_GetLength(this.rawPtr, out length, lengthtype);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00003BA6 File Offset: 0x00001DA6
		public RESULT getFormat(out SOUND_TYPE type, out SOUND_FORMAT format, out int channels, out int bits)
		{
			return Sound.FMOD_Sound_GetFormat(this.rawPtr, out type, out format, out channels, out bits);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00003BB8 File Offset: 0x00001DB8
		public RESULT getNumSubSounds(out int numsubsounds)
		{
			return Sound.FMOD_Sound_GetNumSubSounds(this.rawPtr, out numsubsounds);
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00003BC6 File Offset: 0x00001DC6
		public RESULT getNumTags(out int numtags, out int numtagsupdated)
		{
			return Sound.FMOD_Sound_GetNumTags(this.rawPtr, out numtags, out numtagsupdated);
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00003BD5 File Offset: 0x00001DD5
		public RESULT getTag(string name, int index, out TAG tag)
		{
			return Sound.FMOD_Sound_GetTag(this.rawPtr, name, index, out tag);
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00003BE5 File Offset: 0x00001DE5
		public RESULT getOpenState(out OPENSTATE openstate, out uint percentbuffered, out bool starving, out bool diskbusy)
		{
			return Sound.FMOD_Sound_GetOpenState(this.rawPtr, out openstate, out percentbuffered, out starving, out diskbusy);
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00003BF7 File Offset: 0x00001DF7
		public RESULT readData(IntPtr buffer, uint length, out uint read)
		{
			return Sound.FMOD_Sound_ReadData(this.rawPtr, buffer, length, out read);
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00003C07 File Offset: 0x00001E07
		public RESULT seekData(uint pcm)
		{
			return Sound.FMOD_Sound_SeekData(this.rawPtr, pcm);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00003C15 File Offset: 0x00001E15
		public RESULT setSoundGroup(SoundGroup soundgroup)
		{
			return Sound.FMOD_Sound_SetSoundGroup(this.rawPtr, soundgroup.getRaw());
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00003C28 File Offset: 0x00001E28
		public RESULT getSoundGroup(out SoundGroup soundgroup)
		{
			soundgroup = null;
			IntPtr raw;
			RESULT result = Sound.FMOD_Sound_GetSoundGroup(this.rawPtr, out raw);
			soundgroup = new SoundGroup(raw);
			return result;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00003C4D File Offset: 0x00001E4D
		public RESULT getNumSyncPoints(out int numsyncpoints)
		{
			return Sound.FMOD_Sound_GetNumSyncPoints(this.rawPtr, out numsyncpoints);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00003C5B File Offset: 0x00001E5B
		public RESULT getSyncPoint(int index, out IntPtr point)
		{
			return Sound.FMOD_Sound_GetSyncPoint(this.rawPtr, index, out point);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00003C6C File Offset: 0x00001E6C
		public RESULT getSyncPointInfo(IntPtr point, StringBuilder name, int namelen, out uint offset, TIMEUNIT offsettype)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(name.Capacity);
			RESULT result = Sound.FMOD_Sound_GetSyncPointInfo(this.rawPtr, point, intPtr, namelen, out offset, offsettype);
			StringMarshalHelper.NativeToBuilder(name, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00003CA4 File Offset: 0x00001EA4
		public RESULT addSyncPoint(uint offset, TIMEUNIT offsettype, string name, out IntPtr point)
		{
			return Sound.FMOD_Sound_AddSyncPoint(this.rawPtr, offset, offsettype, name, out point);
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00003CB6 File Offset: 0x00001EB6
		public RESULT deleteSyncPoint(IntPtr point)
		{
			return Sound.FMOD_Sound_DeleteSyncPoint(this.rawPtr, point);
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00003CC4 File Offset: 0x00001EC4
		public RESULT setMode(MODE mode)
		{
			return Sound.FMOD_Sound_SetMode(this.rawPtr, mode);
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00003CD2 File Offset: 0x00001ED2
		public RESULT getMode(out MODE mode)
		{
			return Sound.FMOD_Sound_GetMode(this.rawPtr, out mode);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00003CE0 File Offset: 0x00001EE0
		public RESULT setLoopCount(int loopcount)
		{
			return Sound.FMOD_Sound_SetLoopCount(this.rawPtr, loopcount);
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00003CEE File Offset: 0x00001EEE
		public RESULT getLoopCount(out int loopcount)
		{
			return Sound.FMOD_Sound_GetLoopCount(this.rawPtr, out loopcount);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00003CFC File Offset: 0x00001EFC
		public RESULT setLoopPoints(uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype)
		{
			return Sound.FMOD_Sound_SetLoopPoints(this.rawPtr, loopstart, loopstarttype, loopend, loopendtype);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00003D0E File Offset: 0x00001F0E
		public RESULT getLoopPoints(out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype)
		{
			return Sound.FMOD_Sound_GetLoopPoints(this.rawPtr, out loopstart, loopstarttype, out loopend, loopendtype);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00003D20 File Offset: 0x00001F20
		public RESULT getMusicNumChannels(out int numchannels)
		{
			return Sound.FMOD_Sound_GetMusicNumChannels(this.rawPtr, out numchannels);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00003D2E File Offset: 0x00001F2E
		public RESULT setMusicChannelVolume(int channel, float volume)
		{
			return Sound.FMOD_Sound_SetMusicChannelVolume(this.rawPtr, channel, volume);
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00003D3D File Offset: 0x00001F3D
		public RESULT getMusicChannelVolume(int channel, out float volume)
		{
			return Sound.FMOD_Sound_GetMusicChannelVolume(this.rawPtr, channel, out volume);
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00003D4C File Offset: 0x00001F4C
		public RESULT setMusicSpeed(float speed)
		{
			return Sound.FMOD_Sound_SetMusicSpeed(this.rawPtr, speed);
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00003D5A File Offset: 0x00001F5A
		public RESULT getMusicSpeed(out float speed)
		{
			return Sound.FMOD_Sound_GetMusicSpeed(this.rawPtr, out speed);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00003D68 File Offset: 0x00001F68
		public RESULT setUserData(IntPtr userdata)
		{
			return Sound.FMOD_Sound_SetUserData(this.rawPtr, userdata);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00003D76 File Offset: 0x00001F76
		public RESULT getUserData(out IntPtr userdata)
		{
			return Sound.FMOD_Sound_GetUserData(this.rawPtr, out userdata);
		}

		// Token: 0x06000168 RID: 360
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_Release(IntPtr sound);

		// Token: 0x06000169 RID: 361
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetSystemObject(IntPtr sound, out IntPtr system);

		// Token: 0x0600016A RID: 362
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_Lock(IntPtr sound, uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2);

		// Token: 0x0600016B RID: 363
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_Unlock(IntPtr sound, IntPtr ptr1, IntPtr ptr2, uint len1, uint len2);

		// Token: 0x0600016C RID: 364
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_SetDefaults(IntPtr sound, float frequency, int priority);

		// Token: 0x0600016D RID: 365
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetDefaults(IntPtr sound, out float frequency, out int priority);

		// Token: 0x0600016E RID: 366
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_Set3DMinMaxDistance(IntPtr sound, float min, float max);

		// Token: 0x0600016F RID: 367
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_Get3DMinMaxDistance(IntPtr sound, out float min, out float max);

		// Token: 0x06000170 RID: 368
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_Set3DConeSettings(IntPtr sound, float insideconeangle, float outsideconeangle, float outsidevolume);

		// Token: 0x06000171 RID: 369
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_Get3DConeSettings(IntPtr sound, out float insideconeangle, out float outsideconeangle, out float outsidevolume);

		// Token: 0x06000172 RID: 370
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_Set3DCustomRolloff(IntPtr sound, ref VECTOR points, int numpoints);

		// Token: 0x06000173 RID: 371
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_Get3DCustomRolloff(IntPtr sound, out IntPtr points, out int numpoints);

		// Token: 0x06000174 RID: 372
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetSubSound(IntPtr sound, int index, out IntPtr subsound);

		// Token: 0x06000175 RID: 373
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetSubSoundParent(IntPtr sound, out IntPtr parentsound);

		// Token: 0x06000176 RID: 374
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetName(IntPtr sound, IntPtr name, int namelen);

		// Token: 0x06000177 RID: 375
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetLength(IntPtr sound, out uint length, TIMEUNIT lengthtype);

		// Token: 0x06000178 RID: 376
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetFormat(IntPtr sound, out SOUND_TYPE type, out SOUND_FORMAT format, out int channels, out int bits);

		// Token: 0x06000179 RID: 377
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetNumSubSounds(IntPtr sound, out int numsubsounds);

		// Token: 0x0600017A RID: 378
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetNumTags(IntPtr sound, out int numtags, out int numtagsupdated);

		// Token: 0x0600017B RID: 379
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetTag(IntPtr sound, string name, int index, out TAG tag);

		// Token: 0x0600017C RID: 380
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetOpenState(IntPtr sound, out OPENSTATE openstate, out uint percentbuffered, out bool starving, out bool diskbusy);

		// Token: 0x0600017D RID: 381
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_ReadData(IntPtr sound, IntPtr buffer, uint length, out uint read);

		// Token: 0x0600017E RID: 382
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_SeekData(IntPtr sound, uint pcm);

		// Token: 0x0600017F RID: 383
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_SetSoundGroup(IntPtr sound, IntPtr soundgroup);

		// Token: 0x06000180 RID: 384
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetSoundGroup(IntPtr sound, out IntPtr soundgroup);

		// Token: 0x06000181 RID: 385
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetNumSyncPoints(IntPtr sound, out int numsyncpoints);

		// Token: 0x06000182 RID: 386
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetSyncPoint(IntPtr sound, int index, out IntPtr point);

		// Token: 0x06000183 RID: 387
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetSyncPointInfo(IntPtr sound, IntPtr point, IntPtr name, int namelen, out uint offset, TIMEUNIT offsettype);

		// Token: 0x06000184 RID: 388
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_AddSyncPoint(IntPtr sound, uint offset, TIMEUNIT offsettype, string name, out IntPtr point);

		// Token: 0x06000185 RID: 389
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_DeleteSyncPoint(IntPtr sound, IntPtr point);

		// Token: 0x06000186 RID: 390
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_SetMode(IntPtr sound, MODE mode);

		// Token: 0x06000187 RID: 391
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetMode(IntPtr sound, out MODE mode);

		// Token: 0x06000188 RID: 392
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_SetLoopCount(IntPtr sound, int loopcount);

		// Token: 0x06000189 RID: 393
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetLoopCount(IntPtr sound, out int loopcount);

		// Token: 0x0600018A RID: 394
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_SetLoopPoints(IntPtr sound, uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype);

		// Token: 0x0600018B RID: 395
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetLoopPoints(IntPtr sound, out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype);

		// Token: 0x0600018C RID: 396
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetMusicNumChannels(IntPtr sound, out int numchannels);

		// Token: 0x0600018D RID: 397
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_SetMusicChannelVolume(IntPtr sound, int channel, float volume);

		// Token: 0x0600018E RID: 398
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetMusicChannelVolume(IntPtr sound, int channel, out float volume);

		// Token: 0x0600018F RID: 399
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_SetMusicSpeed(IntPtr sound, float speed);

		// Token: 0x06000190 RID: 400
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetMusicSpeed(IntPtr sound, out float speed);

		// Token: 0x06000191 RID: 401
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_SetUserData(IntPtr sound, IntPtr userdata);

		// Token: 0x06000192 RID: 402
		[DllImport("fmod")]
		private static extern RESULT FMOD_Sound_GetUserData(IntPtr sound, out IntPtr userdata);

		// Token: 0x06000193 RID: 403 RVA: 0x00003A27 File Offset: 0x00001C27
		public Sound(IntPtr raw) : base(raw)
		{
		}
	}
}
