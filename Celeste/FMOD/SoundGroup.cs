using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	// Token: 0x02000046 RID: 70
	public class SoundGroup : HandleBase
	{
		// Token: 0x06000243 RID: 579 RVA: 0x0000438A File Offset: 0x0000258A
		public RESULT release()
		{
			RESULT result = SoundGroup.FMOD_SoundGroup_Release(base.getRaw());
			if (result == RESULT.OK)
			{
				this.rawPtr = IntPtr.Zero;
			}
			return result;
		}

		// Token: 0x06000244 RID: 580 RVA: 0x000043A8 File Offset: 0x000025A8
		public RESULT getSystemObject(out System system)
		{
			system = null;
			IntPtr raw;
			RESULT result = SoundGroup.FMOD_SoundGroup_GetSystemObject(this.rawPtr, out raw);
			system = new System(raw);
			return result;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x000043CD File Offset: 0x000025CD
		public RESULT setMaxAudible(int maxaudible)
		{
			return SoundGroup.FMOD_SoundGroup_SetMaxAudible(this.rawPtr, maxaudible);
		}

		// Token: 0x06000246 RID: 582 RVA: 0x000043DB File Offset: 0x000025DB
		public RESULT getMaxAudible(out int maxaudible)
		{
			return SoundGroup.FMOD_SoundGroup_GetMaxAudible(this.rawPtr, out maxaudible);
		}

		// Token: 0x06000247 RID: 583 RVA: 0x000043E9 File Offset: 0x000025E9
		public RESULT setMaxAudibleBehavior(SOUNDGROUP_BEHAVIOR behavior)
		{
			return SoundGroup.FMOD_SoundGroup_SetMaxAudibleBehavior(this.rawPtr, behavior);
		}

		// Token: 0x06000248 RID: 584 RVA: 0x000043F7 File Offset: 0x000025F7
		public RESULT getMaxAudibleBehavior(out SOUNDGROUP_BEHAVIOR behavior)
		{
			return SoundGroup.FMOD_SoundGroup_GetMaxAudibleBehavior(this.rawPtr, out behavior);
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00004405 File Offset: 0x00002605
		public RESULT setMuteFadeSpeed(float speed)
		{
			return SoundGroup.FMOD_SoundGroup_SetMuteFadeSpeed(this.rawPtr, speed);
		}

		// Token: 0x0600024A RID: 586 RVA: 0x00004413 File Offset: 0x00002613
		public RESULT getMuteFadeSpeed(out float speed)
		{
			return SoundGroup.FMOD_SoundGroup_GetMuteFadeSpeed(this.rawPtr, out speed);
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00004421 File Offset: 0x00002621
		public RESULT setVolume(float volume)
		{
			return SoundGroup.FMOD_SoundGroup_SetVolume(this.rawPtr, volume);
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000442F File Offset: 0x0000262F
		public RESULT getVolume(out float volume)
		{
			return SoundGroup.FMOD_SoundGroup_GetVolume(this.rawPtr, out volume);
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000443D File Offset: 0x0000263D
		public RESULT stop()
		{
			return SoundGroup.FMOD_SoundGroup_Stop(this.rawPtr);
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000444C File Offset: 0x0000264C
		public RESULT getName(StringBuilder name, int namelen)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(name.Capacity);
			RESULT result = SoundGroup.FMOD_SoundGroup_GetName(this.rawPtr, intPtr, namelen);
			StringMarshalHelper.NativeToBuilder(name, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000447F File Offset: 0x0000267F
		public RESULT getNumSounds(out int numsounds)
		{
			return SoundGroup.FMOD_SoundGroup_GetNumSounds(this.rawPtr, out numsounds);
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00004490 File Offset: 0x00002690
		public RESULT getSound(int index, out Sound sound)
		{
			sound = null;
			IntPtr raw;
			RESULT result = SoundGroup.FMOD_SoundGroup_GetSound(this.rawPtr, index, out raw);
			sound = new Sound(raw);
			return result;
		}

		// Token: 0x06000251 RID: 593 RVA: 0x000044B6 File Offset: 0x000026B6
		public RESULT getNumPlaying(out int numplaying)
		{
			return SoundGroup.FMOD_SoundGroup_GetNumPlaying(this.rawPtr, out numplaying);
		}

		// Token: 0x06000252 RID: 594 RVA: 0x000044C4 File Offset: 0x000026C4
		public RESULT setUserData(IntPtr userdata)
		{
			return SoundGroup.FMOD_SoundGroup_SetUserData(this.rawPtr, userdata);
		}

		// Token: 0x06000253 RID: 595 RVA: 0x000044D2 File Offset: 0x000026D2
		public RESULT getUserData(out IntPtr userdata)
		{
			return SoundGroup.FMOD_SoundGroup_GetUserData(this.rawPtr, out userdata);
		}

		// Token: 0x06000254 RID: 596
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_Release(IntPtr soundgroup);

		// Token: 0x06000255 RID: 597
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_GetSystemObject(IntPtr soundgroup, out IntPtr system);

		// Token: 0x06000256 RID: 598
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_SetMaxAudible(IntPtr soundgroup, int maxaudible);

		// Token: 0x06000257 RID: 599
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_GetMaxAudible(IntPtr soundgroup, out int maxaudible);

		// Token: 0x06000258 RID: 600
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_SetMaxAudibleBehavior(IntPtr soundgroup, SOUNDGROUP_BEHAVIOR behavior);

		// Token: 0x06000259 RID: 601
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_GetMaxAudibleBehavior(IntPtr soundgroup, out SOUNDGROUP_BEHAVIOR behavior);

		// Token: 0x0600025A RID: 602
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_SetMuteFadeSpeed(IntPtr soundgroup, float speed);

		// Token: 0x0600025B RID: 603
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_GetMuteFadeSpeed(IntPtr soundgroup, out float speed);

		// Token: 0x0600025C RID: 604
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_SetVolume(IntPtr soundgroup, float volume);

		// Token: 0x0600025D RID: 605
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_GetVolume(IntPtr soundgroup, out float volume);

		// Token: 0x0600025E RID: 606
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_Stop(IntPtr soundgroup);

		// Token: 0x0600025F RID: 607
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_GetName(IntPtr soundgroup, IntPtr name, int namelen);

		// Token: 0x06000260 RID: 608
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_GetNumSounds(IntPtr soundgroup, out int numsounds);

		// Token: 0x06000261 RID: 609
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_GetSound(IntPtr soundgroup, int index, out IntPtr sound);

		// Token: 0x06000262 RID: 610
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_GetNumPlaying(IntPtr soundgroup, out int numplaying);

		// Token: 0x06000263 RID: 611
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_SetUserData(IntPtr soundgroup, IntPtr userdata);

		// Token: 0x06000264 RID: 612
		[DllImport("fmod")]
		private static extern RESULT FMOD_SoundGroup_GetUserData(IntPtr soundgroup, out IntPtr userdata);

		// Token: 0x06000265 RID: 613 RVA: 0x00003A27 File Offset: 0x00001C27
		public SoundGroup(IntPtr raw) : base(raw)
		{
		}
	}
}
