using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x02000044 RID: 68
	public class Channel : ChannelControl
	{
		// Token: 0x0600020F RID: 527 RVA: 0x0000415D File Offset: 0x0000235D
		public RESULT setFrequency(float frequency)
		{
			return Channel.FMOD_Channel_SetFrequency(base.getRaw(), frequency);
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000416B File Offset: 0x0000236B
		public RESULT getFrequency(out float frequency)
		{
			return Channel.FMOD_Channel_GetFrequency(base.getRaw(), out frequency);
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00004179 File Offset: 0x00002379
		public RESULT setPriority(int priority)
		{
			return Channel.FMOD_Channel_SetPriority(base.getRaw(), priority);
		}

		// Token: 0x06000212 RID: 530 RVA: 0x00004187 File Offset: 0x00002387
		public RESULT getPriority(out int priority)
		{
			return Channel.FMOD_Channel_GetPriority(base.getRaw(), out priority);
		}

		// Token: 0x06000213 RID: 531 RVA: 0x00004195 File Offset: 0x00002395
		public RESULT setPosition(uint position, TIMEUNIT postype)
		{
			return Channel.FMOD_Channel_SetPosition(base.getRaw(), position, postype);
		}

		// Token: 0x06000214 RID: 532 RVA: 0x000041A4 File Offset: 0x000023A4
		public RESULT getPosition(out uint position, TIMEUNIT postype)
		{
			return Channel.FMOD_Channel_GetPosition(base.getRaw(), out position, postype);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x000041B3 File Offset: 0x000023B3
		public RESULT setChannelGroup(ChannelGroup channelgroup)
		{
			return Channel.FMOD_Channel_SetChannelGroup(base.getRaw(), channelgroup.getRaw());
		}

		// Token: 0x06000216 RID: 534 RVA: 0x000041C8 File Offset: 0x000023C8
		public RESULT getChannelGroup(out ChannelGroup channelgroup)
		{
			channelgroup = null;
			IntPtr raw;
			RESULT result = Channel.FMOD_Channel_GetChannelGroup(base.getRaw(), out raw);
			channelgroup = new ChannelGroup(raw);
			return result;
		}

		// Token: 0x06000217 RID: 535 RVA: 0x000041ED File Offset: 0x000023ED
		public RESULT setLoopCount(int loopcount)
		{
			return Channel.FMOD_Channel_SetLoopCount(base.getRaw(), loopcount);
		}

		// Token: 0x06000218 RID: 536 RVA: 0x000041FB File Offset: 0x000023FB
		public RESULT getLoopCount(out int loopcount)
		{
			return Channel.FMOD_Channel_GetLoopCount(base.getRaw(), out loopcount);
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00004209 File Offset: 0x00002409
		public RESULT setLoopPoints(uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype)
		{
			return Channel.FMOD_Channel_SetLoopPoints(base.getRaw(), loopstart, loopstarttype, loopend, loopendtype);
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000421B File Offset: 0x0000241B
		public RESULT getLoopPoints(out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype)
		{
			return Channel.FMOD_Channel_GetLoopPoints(base.getRaw(), out loopstart, loopstarttype, out loopend, loopendtype);
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000422D File Offset: 0x0000242D
		public RESULT isVirtual(out bool isvirtual)
		{
			return Channel.FMOD_Channel_IsVirtual(base.getRaw(), out isvirtual);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000423C File Offset: 0x0000243C
		public RESULT getCurrentSound(out Sound sound)
		{
			sound = null;
			IntPtr raw;
			RESULT result = Channel.FMOD_Channel_GetCurrentSound(base.getRaw(), out raw);
			sound = new Sound(raw);
			return result;
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00004261 File Offset: 0x00002461
		public RESULT getIndex(out int index)
		{
			return Channel.FMOD_Channel_GetIndex(base.getRaw(), out index);
		}

		// Token: 0x0600021E RID: 542
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_SetFrequency(IntPtr channel, float frequency);

		// Token: 0x0600021F RID: 543
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_GetFrequency(IntPtr channel, out float frequency);

		// Token: 0x06000220 RID: 544
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_SetPriority(IntPtr channel, int priority);

		// Token: 0x06000221 RID: 545
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_GetPriority(IntPtr channel, out int priority);

		// Token: 0x06000222 RID: 546
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_SetChannelGroup(IntPtr channel, IntPtr channelgroup);

		// Token: 0x06000223 RID: 547
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_GetChannelGroup(IntPtr channel, out IntPtr channelgroup);

		// Token: 0x06000224 RID: 548
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_IsVirtual(IntPtr channel, out bool isvirtual);

		// Token: 0x06000225 RID: 549
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_GetCurrentSound(IntPtr channel, out IntPtr sound);

		// Token: 0x06000226 RID: 550
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_GetIndex(IntPtr channel, out int index);

		// Token: 0x06000227 RID: 551
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_SetPosition(IntPtr channel, uint position, TIMEUNIT postype);

		// Token: 0x06000228 RID: 552
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_GetPosition(IntPtr channel, out uint position, TIMEUNIT postype);

		// Token: 0x06000229 RID: 553
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_SetMode(IntPtr channel, MODE mode);

		// Token: 0x0600022A RID: 554
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_GetMode(IntPtr channel, out MODE mode);

		// Token: 0x0600022B RID: 555
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_SetLoopCount(IntPtr channel, int loopcount);

		// Token: 0x0600022C RID: 556
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_GetLoopCount(IntPtr channel, out int loopcount);

		// Token: 0x0600022D RID: 557
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_SetLoopPoints(IntPtr channel, uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype);

		// Token: 0x0600022E RID: 558
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_GetLoopPoints(IntPtr channel, out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype);

		// Token: 0x0600022F RID: 559
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_SetUserData(IntPtr channel, IntPtr userdata);

		// Token: 0x06000230 RID: 560
		[DllImport("fmod")]
		private static extern RESULT FMOD_Channel_GetUserData(IntPtr channel, out IntPtr userdata);

		// Token: 0x06000231 RID: 561 RVA: 0x0000426F File Offset: 0x0000246F
		public Channel(IntPtr raw) : base(raw)
		{
		}
	}
}
