using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	// Token: 0x02000045 RID: 69
	public class ChannelGroup : ChannelControl
	{
		// Token: 0x06000232 RID: 562 RVA: 0x00004278 File Offset: 0x00002478
		public RESULT release()
		{
			RESULT result = ChannelGroup.FMOD_ChannelGroup_Release(base.getRaw());
			if (result == RESULT.OK)
			{
				this.rawPtr = IntPtr.Zero;
			}
			return result;
		}

		// Token: 0x06000233 RID: 563 RVA: 0x00004294 File Offset: 0x00002494
		public RESULT addGroup(ChannelGroup group, bool propagatedspclock, out DSPConnection connection)
		{
			connection = null;
			IntPtr raw;
			RESULT result = ChannelGroup.FMOD_ChannelGroup_AddGroup(base.getRaw(), group.getRaw(), propagatedspclock, out raw);
			connection = new DSPConnection(raw);
			return result;
		}

		// Token: 0x06000234 RID: 564 RVA: 0x000042C0 File Offset: 0x000024C0
		public RESULT getNumGroups(out int numgroups)
		{
			return ChannelGroup.FMOD_ChannelGroup_GetNumGroups(base.getRaw(), out numgroups);
		}

		// Token: 0x06000235 RID: 565 RVA: 0x000042D0 File Offset: 0x000024D0
		public RESULT getGroup(int index, out ChannelGroup group)
		{
			group = null;
			IntPtr raw;
			RESULT result = ChannelGroup.FMOD_ChannelGroup_GetGroup(base.getRaw(), index, out raw);
			group = new ChannelGroup(raw);
			return result;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x000042F8 File Offset: 0x000024F8
		public RESULT getParentGroup(out ChannelGroup group)
		{
			group = null;
			IntPtr raw;
			RESULT result = ChannelGroup.FMOD_ChannelGroup_GetParentGroup(base.getRaw(), out raw);
			group = new ChannelGroup(raw);
			return result;
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00004320 File Offset: 0x00002520
		public RESULT getName(StringBuilder name, int namelen)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(name.Capacity);
			RESULT result = ChannelGroup.FMOD_ChannelGroup_GetName(base.getRaw(), intPtr, namelen);
			StringMarshalHelper.NativeToBuilder(name, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		// Token: 0x06000238 RID: 568 RVA: 0x00004353 File Offset: 0x00002553
		public RESULT getNumChannels(out int numchannels)
		{
			return ChannelGroup.FMOD_ChannelGroup_GetNumChannels(base.getRaw(), out numchannels);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x00004364 File Offset: 0x00002564
		public RESULT getChannel(int index, out Channel channel)
		{
			channel = null;
			IntPtr raw;
			RESULT result = ChannelGroup.FMOD_ChannelGroup_GetChannel(base.getRaw(), index, out raw);
			channel = new Channel(raw);
			return result;
		}

		// Token: 0x0600023A RID: 570
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_Release(IntPtr channelgroup);

		// Token: 0x0600023B RID: 571
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_AddGroup(IntPtr channelgroup, IntPtr group, bool propagatedspclock, out IntPtr connection);

		// Token: 0x0600023C RID: 572
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetNumGroups(IntPtr channelgroup, out int numgroups);

		// Token: 0x0600023D RID: 573
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetGroup(IntPtr channelgroup, int index, out IntPtr group);

		// Token: 0x0600023E RID: 574
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetParentGroup(IntPtr channelgroup, out IntPtr group);

		// Token: 0x0600023F RID: 575
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetName(IntPtr channelgroup, IntPtr name, int namelen);

		// Token: 0x06000240 RID: 576
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetNumChannels(IntPtr channelgroup, out int numchannels);

		// Token: 0x06000241 RID: 577
		[DllImport("fmod")]
		private static extern RESULT FMOD_ChannelGroup_GetChannel(IntPtr channelgroup, int index, out IntPtr channel);

		// Token: 0x06000242 RID: 578 RVA: 0x0000426F File Offset: 0x0000246F
		public ChannelGroup(IntPtr raw) : base(raw)
		{
		}
	}
}
