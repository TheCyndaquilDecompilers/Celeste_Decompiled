using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	// Token: 0x02000041 RID: 65
	public class System : HandleBase
	{
		// Token: 0x06000084 RID: 132 RVA: 0x00003083 File Offset: 0x00001283
		public RESULT release()
		{
			RESULT result = System.FMOD_System_Release(this.rawPtr);
			if (result == RESULT.OK)
			{
				this.rawPtr = IntPtr.Zero;
			}
			return result;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000309E File Offset: 0x0000129E
		public RESULT setOutput(OUTPUTTYPE output)
		{
			return System.FMOD_System_SetOutput(this.rawPtr, output);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000030AC File Offset: 0x000012AC
		public RESULT getOutput(out OUTPUTTYPE output)
		{
			return System.FMOD_System_GetOutput(this.rawPtr, out output);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000030BA File Offset: 0x000012BA
		public RESULT getNumDrivers(out int numdrivers)
		{
			return System.FMOD_System_GetNumDrivers(this.rawPtr, out numdrivers);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000030C8 File Offset: 0x000012C8
		public RESULT getDriverInfo(int id, StringBuilder name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(name.Capacity);
			RESULT result = System.FMOD_System_GetDriverInfo(this.rawPtr, id, intPtr, namelen, out guid, out systemrate, out speakermode, out speakermodechannels);
			StringMarshalHelper.NativeToBuilder(name, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00003104 File Offset: 0x00001304
		public RESULT setDriver(int driver)
		{
			return System.FMOD_System_SetDriver(this.rawPtr, driver);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003112 File Offset: 0x00001312
		public RESULT getDriver(out int driver)
		{
			return System.FMOD_System_GetDriver(this.rawPtr, out driver);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00003120 File Offset: 0x00001320
		public RESULT setSoftwareChannels(int numsoftwarechannels)
		{
			return System.FMOD_System_SetSoftwareChannels(this.rawPtr, numsoftwarechannels);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000312E File Offset: 0x0000132E
		public RESULT getSoftwareChannels(out int numsoftwarechannels)
		{
			return System.FMOD_System_GetSoftwareChannels(this.rawPtr, out numsoftwarechannels);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x0000313C File Offset: 0x0000133C
		public RESULT setSoftwareFormat(int samplerate, SPEAKERMODE speakermode, int numrawspeakers)
		{
			return System.FMOD_System_SetSoftwareFormat(this.rawPtr, samplerate, speakermode, numrawspeakers);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000314C File Offset: 0x0000134C
		public RESULT getSoftwareFormat(out int samplerate, out SPEAKERMODE speakermode, out int numrawspeakers)
		{
			return System.FMOD_System_GetSoftwareFormat(this.rawPtr, out samplerate, out speakermode, out numrawspeakers);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000315C File Offset: 0x0000135C
		public RESULT setDSPBufferSize(uint bufferlength, int numbuffers)
		{
			return System.FMOD_System_SetDSPBufferSize(this.rawPtr, bufferlength, numbuffers);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000316B File Offset: 0x0000136B
		public RESULT getDSPBufferSize(out uint bufferlength, out int numbuffers)
		{
			return System.FMOD_System_GetDSPBufferSize(this.rawPtr, out bufferlength, out numbuffers);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000317A File Offset: 0x0000137A
		public RESULT setFileSystem(FILE_OPENCALLBACK useropen, FILE_CLOSECALLBACK userclose, FILE_READCALLBACK userread, FILE_SEEKCALLBACK userseek, FILE_ASYNCREADCALLBACK userasyncread, FILE_ASYNCCANCELCALLBACK userasynccancel, int blockalign)
		{
			return System.FMOD_System_SetFileSystem(this.rawPtr, useropen, userclose, userread, userseek, userasyncread, userasynccancel, blockalign);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00003192 File Offset: 0x00001392
		public RESULT attachFileSystem(FILE_OPENCALLBACK useropen, FILE_CLOSECALLBACK userclose, FILE_READCALLBACK userread, FILE_SEEKCALLBACK userseek)
		{
			return System.FMOD_System_AttachFileSystem(this.rawPtr, useropen, userclose, userread, userseek);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000031A4 File Offset: 0x000013A4
		public RESULT setAdvancedSettings(ref ADVANCEDSETTINGS settings)
		{
			settings.cbSize = Marshal.SizeOf(settings);
			return System.FMOD_System_SetAdvancedSettings(this.rawPtr, ref settings);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000031C8 File Offset: 0x000013C8
		public RESULT getAdvancedSettings(ref ADVANCEDSETTINGS settings)
		{
			settings.cbSize = Marshal.SizeOf(settings);
			return System.FMOD_System_GetAdvancedSettings(this.rawPtr, ref settings);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000031EC File Offset: 0x000013EC
		public RESULT setCallback(SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask = SYSTEM_CALLBACK_TYPE.ALL)
		{
			return System.FMOD_System_SetCallback(this.rawPtr, callback, callbackmask);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000031FB File Offset: 0x000013FB
		public RESULT setPluginPath(string path)
		{
			return System.FMOD_System_SetPluginPath(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"));
		}

		// Token: 0x06000097 RID: 151 RVA: 0x0000321D File Offset: 0x0000141D
		public RESULT loadPlugin(string filename, out uint handle, uint priority = 0U)
		{
			return System.FMOD_System_LoadPlugin(this.rawPtr, Encoding.UTF8.GetBytes(filename + "\0"), out handle, priority);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00003241 File Offset: 0x00001441
		public RESULT unloadPlugin(uint handle)
		{
			return System.FMOD_System_UnloadPlugin(this.rawPtr, handle);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000324F File Offset: 0x0000144F
		public RESULT getNumNestedPlugins(uint handle, out int count)
		{
			return System.FMOD_System_GetNumNestedPlugins(this.rawPtr, handle, out count);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000325E File Offset: 0x0000145E
		public RESULT getNestedPlugin(uint handle, int index, out uint nestedhandle)
		{
			return System.FMOD_System_GetNestedPlugin(this.rawPtr, handle, index, out nestedhandle);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x0000326E File Offset: 0x0000146E
		public RESULT getNumPlugins(PLUGINTYPE plugintype, out int numplugins)
		{
			return System.FMOD_System_GetNumPlugins(this.rawPtr, plugintype, out numplugins);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000327D File Offset: 0x0000147D
		public RESULT getPluginHandle(PLUGINTYPE plugintype, int index, out uint handle)
		{
			return System.FMOD_System_GetPluginHandle(this.rawPtr, plugintype, index, out handle);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00003290 File Offset: 0x00001490
		public RESULT getPluginInfo(uint handle, out PLUGINTYPE plugintype, StringBuilder name, int namelen, out uint version)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(name.Capacity);
			RESULT result = System.FMOD_System_GetPluginInfo(this.rawPtr, handle, out plugintype, intPtr, namelen, out version);
			StringMarshalHelper.NativeToBuilder(name, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000032C8 File Offset: 0x000014C8
		public RESULT setOutputByPlugin(uint handle)
		{
			return System.FMOD_System_SetOutputByPlugin(this.rawPtr, handle);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000032D6 File Offset: 0x000014D6
		public RESULT getOutputByPlugin(out uint handle)
		{
			return System.FMOD_System_GetOutputByPlugin(this.rawPtr, out handle);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000032E4 File Offset: 0x000014E4
		public RESULT createDSPByPlugin(uint handle, out DSP dsp)
		{
			dsp = null;
			IntPtr raw;
			RESULT result = System.FMOD_System_CreateDSPByPlugin(this.rawPtr, handle, out raw);
			dsp = new DSP(raw);
			return result;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x0000330A File Offset: 0x0000150A
		public RESULT getDSPInfoByPlugin(uint handle, out IntPtr description)
		{
			return System.FMOD_System_GetDSPInfoByPlugin(this.rawPtr, handle, out description);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00003319 File Offset: 0x00001519
		public RESULT registerDSP(ref DSP_DESCRIPTION description, out uint handle)
		{
			return System.FMOD_System_RegisterDSP(this.rawPtr, ref description, out handle);
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00003328 File Offset: 0x00001528
		public RESULT init(int maxchannels, INITFLAGS flags, IntPtr extradriverdata)
		{
			return System.FMOD_System_Init(this.rawPtr, maxchannels, flags, extradriverdata);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00003338 File Offset: 0x00001538
		public RESULT close()
		{
			return System.FMOD_System_Close(this.rawPtr);
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00003345 File Offset: 0x00001545
		public RESULT update()
		{
			return System.FMOD_System_Update(this.rawPtr);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00003352 File Offset: 0x00001552
		public RESULT setSpeakerPosition(SPEAKER speaker, float x, float y, bool active)
		{
			return System.FMOD_System_SetSpeakerPosition(this.rawPtr, speaker, x, y, active);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00003364 File Offset: 0x00001564
		public RESULT getSpeakerPosition(SPEAKER speaker, out float x, out float y, out bool active)
		{
			return System.FMOD_System_GetSpeakerPosition(this.rawPtr, speaker, out x, out y, out active);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00003376 File Offset: 0x00001576
		public RESULT setStreamBufferSize(uint filebuffersize, TIMEUNIT filebuffersizetype)
		{
			return System.FMOD_System_SetStreamBufferSize(this.rawPtr, filebuffersize, filebuffersizetype);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00003385 File Offset: 0x00001585
		public RESULT getStreamBufferSize(out uint filebuffersize, out TIMEUNIT filebuffersizetype)
		{
			return System.FMOD_System_GetStreamBufferSize(this.rawPtr, out filebuffersize, out filebuffersizetype);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00003394 File Offset: 0x00001594
		public RESULT set3DSettings(float dopplerscale, float distancefactor, float rolloffscale)
		{
			return System.FMOD_System_Set3DSettings(this.rawPtr, dopplerscale, distancefactor, rolloffscale);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000033A4 File Offset: 0x000015A4
		public RESULT get3DSettings(out float dopplerscale, out float distancefactor, out float rolloffscale)
		{
			return System.FMOD_System_Get3DSettings(this.rawPtr, out dopplerscale, out distancefactor, out rolloffscale);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000033B4 File Offset: 0x000015B4
		public RESULT set3DNumListeners(int numlisteners)
		{
			return System.FMOD_System_Set3DNumListeners(this.rawPtr, numlisteners);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000033C2 File Offset: 0x000015C2
		public RESULT get3DNumListeners(out int numlisteners)
		{
			return System.FMOD_System_Get3DNumListeners(this.rawPtr, out numlisteners);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000033D0 File Offset: 0x000015D0
		public RESULT set3DListenerAttributes(int listener, ref VECTOR pos, ref VECTOR vel, ref VECTOR forward, ref VECTOR up)
		{
			return System.FMOD_System_Set3DListenerAttributes(this.rawPtr, listener, ref pos, ref vel, ref forward, ref up);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000033E4 File Offset: 0x000015E4
		public RESULT get3DListenerAttributes(int listener, out VECTOR pos, out VECTOR vel, out VECTOR forward, out VECTOR up)
		{
			return System.FMOD_System_Get3DListenerAttributes(this.rawPtr, listener, out pos, out vel, out forward, out up);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000033F8 File Offset: 0x000015F8
		public RESULT set3DRolloffCallback(CB_3D_ROLLOFFCALLBACK callback)
		{
			return System.FMOD_System_Set3DRolloffCallback(this.rawPtr, callback);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00003406 File Offset: 0x00001606
		public RESULT mixerSuspend()
		{
			return System.FMOD_System_MixerSuspend(this.rawPtr);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00003413 File Offset: 0x00001613
		public RESULT mixerResume()
		{
			return System.FMOD_System_MixerResume(this.rawPtr);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00003420 File Offset: 0x00001620
		public RESULT getDefaultMixMatrix(SPEAKERMODE sourcespeakermode, SPEAKERMODE targetspeakermode, float[] matrix, int matrixhop)
		{
			return System.FMOD_System_GetDefaultMixMatrix(this.rawPtr, sourcespeakermode, targetspeakermode, matrix, matrixhop);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00003432 File Offset: 0x00001632
		public RESULT getSpeakerModeChannels(SPEAKERMODE mode, out int channels)
		{
			return System.FMOD_System_GetSpeakerModeChannels(this.rawPtr, mode, out channels);
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00003441 File Offset: 0x00001641
		public RESULT getVersion(out uint version)
		{
			return System.FMOD_System_GetVersion(this.rawPtr, out version);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000344F File Offset: 0x0000164F
		public RESULT getOutputHandle(out IntPtr handle)
		{
			return System.FMOD_System_GetOutputHandle(this.rawPtr, out handle);
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000345D File Offset: 0x0000165D
		public RESULT getChannelsPlaying(out int channels, out int realchannels)
		{
			return System.FMOD_System_GetChannelsPlaying(this.rawPtr, out channels, out realchannels);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x0000346C File Offset: 0x0000166C
		public RESULT getCPUUsage(out float dsp, out float stream, out float geometry, out float update, out float total)
		{
			return System.FMOD_System_GetCPUUsage(this.rawPtr, out dsp, out stream, out geometry, out update, out total);
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00003480 File Offset: 0x00001680
		public RESULT getFileUsage(out long sampleBytesRead, out long streamBytesRead, out long otherBytesRead)
		{
			return System.FMOD_System_GetFileUsage(this.rawPtr, out sampleBytesRead, out streamBytesRead, out otherBytesRead);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00003490 File Offset: 0x00001690
		public RESULT getSoundRAM(out int currentalloced, out int maxalloced, out int total)
		{
			return System.FMOD_System_GetSoundRAM(this.rawPtr, out currentalloced, out maxalloced, out total);
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000034A0 File Offset: 0x000016A0
		public RESULT createSound(string name, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			sound = null;
			byte[] bytes = Encoding.UTF8.GetBytes(name + "\0");
			exinfo.cbsize = Marshal.SizeOf(exinfo);
			IntPtr raw;
			RESULT result = System.FMOD_System_CreateSound(this.rawPtr, bytes, mode, ref exinfo, out raw);
			sound = new Sound(raw);
			return result;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000034F8 File Offset: 0x000016F8
		public RESULT createSound(byte[] data, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			sound = null;
			exinfo.cbsize = Marshal.SizeOf(exinfo);
			IntPtr raw;
			RESULT result = System.FMOD_System_CreateSound(this.rawPtr, data, mode, ref exinfo, out raw);
			sound = new Sound(raw);
			return result;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00003538 File Offset: 0x00001738
		public RESULT createSound(string name, MODE mode, out Sound sound)
		{
			CREATESOUNDEXINFO createsoundexinfo = default(CREATESOUNDEXINFO);
			createsoundexinfo.cbsize = Marshal.SizeOf(createsoundexinfo);
			return this.createSound(name, mode, ref createsoundexinfo, out sound);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000356C File Offset: 0x0000176C
		public RESULT createStream(string name, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			sound = null;
			byte[] bytes = Encoding.UTF8.GetBytes(name + "\0");
			exinfo.cbsize = Marshal.SizeOf(exinfo);
			IntPtr raw;
			RESULT result = System.FMOD_System_CreateStream(this.rawPtr, bytes, mode, ref exinfo, out raw);
			sound = new Sound(raw);
			return result;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000035C4 File Offset: 0x000017C4
		public RESULT createStream(byte[] data, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			sound = null;
			exinfo.cbsize = Marshal.SizeOf(exinfo);
			IntPtr raw;
			RESULT result = System.FMOD_System_CreateStream(this.rawPtr, data, mode, ref exinfo, out raw);
			sound = new Sound(raw);
			return result;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00003604 File Offset: 0x00001804
		public RESULT createStream(string name, MODE mode, out Sound sound)
		{
			CREATESOUNDEXINFO createsoundexinfo = default(CREATESOUNDEXINFO);
			createsoundexinfo.cbsize = Marshal.SizeOf(createsoundexinfo);
			return this.createStream(name, mode, ref createsoundexinfo, out sound);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00003638 File Offset: 0x00001838
		public RESULT createDSP(ref DSP_DESCRIPTION description, out DSP dsp)
		{
			dsp = null;
			IntPtr raw;
			RESULT result = System.FMOD_System_CreateDSP(this.rawPtr, ref description, out raw);
			dsp = new DSP(raw);
			return result;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00003660 File Offset: 0x00001860
		public RESULT createDSPByType(DSP_TYPE type, out DSP dsp)
		{
			dsp = null;
			IntPtr raw;
			RESULT result = System.FMOD_System_CreateDSPByType(this.rawPtr, type, out raw);
			dsp = new DSP(raw);
			return result;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00003688 File Offset: 0x00001888
		public RESULT createChannelGroup(string name, out ChannelGroup channelgroup)
		{
			channelgroup = null;
			byte[] bytes = Encoding.UTF8.GetBytes(name + "\0");
			IntPtr raw;
			RESULT result = System.FMOD_System_CreateChannelGroup(this.rawPtr, bytes, out raw);
			channelgroup = new ChannelGroup(raw);
			return result;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000036C4 File Offset: 0x000018C4
		public RESULT createSoundGroup(string name, out SoundGroup soundgroup)
		{
			soundgroup = null;
			byte[] bytes = Encoding.UTF8.GetBytes(name + "\0");
			IntPtr raw;
			RESULT result = System.FMOD_System_CreateSoundGroup(this.rawPtr, bytes, out raw);
			soundgroup = new SoundGroup(raw);
			return result;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00003700 File Offset: 0x00001900
		public RESULT createReverb3D(out Reverb3D reverb)
		{
			IntPtr raw;
			RESULT result = System.FMOD_System_CreateReverb3D(this.rawPtr, out raw);
			reverb = new Reverb3D(raw);
			return result;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00003724 File Offset: 0x00001924
		public RESULT playSound(Sound sound, ChannelGroup channelGroup, bool paused, out Channel channel)
		{
			channel = null;
			IntPtr channelGroup2 = (channelGroup != null) ? channelGroup.getRaw() : IntPtr.Zero;
			IntPtr raw;
			RESULT result = System.FMOD_System_PlaySound(this.rawPtr, sound.getRaw(), channelGroup2, paused, out raw);
			channel = new Channel(raw);
			return result;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x0000376C File Offset: 0x0000196C
		public RESULT playDSP(DSP dsp, ChannelGroup channelGroup, bool paused, out Channel channel)
		{
			channel = null;
			IntPtr channelGroup2 = (channelGroup != null) ? channelGroup.getRaw() : IntPtr.Zero;
			IntPtr raw;
			RESULT result = System.FMOD_System_PlayDSP(this.rawPtr, dsp.getRaw(), channelGroup2, paused, out raw);
			channel = new Channel(raw);
			return result;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000037B4 File Offset: 0x000019B4
		public RESULT getChannel(int channelid, out Channel channel)
		{
			channel = null;
			IntPtr raw;
			RESULT result = System.FMOD_System_GetChannel(this.rawPtr, channelid, out raw);
			channel = new Channel(raw);
			return result;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000037DC File Offset: 0x000019DC
		public RESULT getMasterChannelGroup(out ChannelGroup channelgroup)
		{
			channelgroup = null;
			IntPtr raw;
			RESULT result = System.FMOD_System_GetMasterChannelGroup(this.rawPtr, out raw);
			channelgroup = new ChannelGroup(raw);
			return result;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00003804 File Offset: 0x00001A04
		public RESULT getMasterSoundGroup(out SoundGroup soundgroup)
		{
			soundgroup = null;
			IntPtr raw;
			RESULT result = System.FMOD_System_GetMasterSoundGroup(this.rawPtr, out raw);
			soundgroup = new SoundGroup(raw);
			return result;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00003829 File Offset: 0x00001A29
		public RESULT attachChannelGroupToPort(uint portType, ulong portIndex, ChannelGroup channelgroup, bool passThru = false)
		{
			return System.FMOD_System_AttachChannelGroupToPort(this.rawPtr, portType, portIndex, channelgroup.getRaw(), passThru);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00003840 File Offset: 0x00001A40
		public RESULT detachChannelGroupFromPort(ChannelGroup channelgroup)
		{
			return System.FMOD_System_DetachChannelGroupFromPort(this.rawPtr, channelgroup.getRaw());
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00003853 File Offset: 0x00001A53
		public RESULT setReverbProperties(int instance, ref REVERB_PROPERTIES prop)
		{
			return System.FMOD_System_SetReverbProperties(this.rawPtr, instance, ref prop);
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00003862 File Offset: 0x00001A62
		public RESULT getReverbProperties(int instance, out REVERB_PROPERTIES prop)
		{
			return System.FMOD_System_GetReverbProperties(this.rawPtr, instance, out prop);
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00003871 File Offset: 0x00001A71
		public RESULT lockDSP()
		{
			return System.FMOD_System_LockDSP(this.rawPtr);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000387E File Offset: 0x00001A7E
		public RESULT unlockDSP()
		{
			return System.FMOD_System_UnlockDSP(this.rawPtr);
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000388B File Offset: 0x00001A8B
		public RESULT getRecordNumDrivers(out int numdrivers, out int numconnected)
		{
			return System.FMOD_System_GetRecordNumDrivers(this.rawPtr, out numdrivers, out numconnected);
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000389C File Offset: 0x00001A9C
		public RESULT getRecordDriverInfo(int id, StringBuilder name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels, out DRIVER_STATE state)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(name.Capacity);
			RESULT result = System.FMOD_System_GetRecordDriverInfo(this.rawPtr, id, intPtr, namelen, out guid, out systemrate, out speakermode, out speakermodechannels, out state);
			StringMarshalHelper.NativeToBuilder(name, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x000038DA File Offset: 0x00001ADA
		public RESULT getRecordPosition(int id, out uint position)
		{
			return System.FMOD_System_GetRecordPosition(this.rawPtr, id, out position);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x000038E9 File Offset: 0x00001AE9
		public RESULT recordStart(int id, Sound sound, bool loop)
		{
			return System.FMOD_System_RecordStart(this.rawPtr, id, sound.getRaw(), loop);
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x000038FE File Offset: 0x00001AFE
		public RESULT recordStop(int id)
		{
			return System.FMOD_System_RecordStop(this.rawPtr, id);
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000390C File Offset: 0x00001B0C
		public RESULT isRecording(int id, out bool recording)
		{
			return System.FMOD_System_IsRecording(this.rawPtr, id, out recording);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000391C File Offset: 0x00001B1C
		public RESULT createGeometry(int maxpolygons, int maxvertices, out Geometry geometry)
		{
			geometry = null;
			IntPtr raw;
			RESULT result = System.FMOD_System_CreateGeometry(this.rawPtr, maxpolygons, maxvertices, out raw);
			geometry = new Geometry(raw);
			return result;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00003943 File Offset: 0x00001B43
		public RESULT setGeometrySettings(float maxworldsize)
		{
			return System.FMOD_System_SetGeometrySettings(this.rawPtr, maxworldsize);
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00003951 File Offset: 0x00001B51
		public RESULT getGeometrySettings(out float maxworldsize)
		{
			return System.FMOD_System_GetGeometrySettings(this.rawPtr, out maxworldsize);
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00003960 File Offset: 0x00001B60
		public RESULT loadGeometry(IntPtr data, int datasize, out Geometry geometry)
		{
			geometry = null;
			IntPtr raw;
			RESULT result = System.FMOD_System_LoadGeometry(this.rawPtr, data, datasize, out raw);
			geometry = new Geometry(raw);
			return result;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00003987 File Offset: 0x00001B87
		public RESULT getGeometryOcclusion(ref VECTOR listener, ref VECTOR source, out float direct, out float reverb)
		{
			return System.FMOD_System_GetGeometryOcclusion(this.rawPtr, ref listener, ref source, out direct, out reverb);
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00003999 File Offset: 0x00001B99
		public RESULT setNetworkProxy(string proxy)
		{
			return System.FMOD_System_SetNetworkProxy(this.rawPtr, Encoding.UTF8.GetBytes(proxy + "\0"));
		}

		// Token: 0x060000DD RID: 221 RVA: 0x000039BC File Offset: 0x00001BBC
		public RESULT getNetworkProxy(StringBuilder proxy, int proxylen)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(proxy.Capacity);
			RESULT result = System.FMOD_System_GetNetworkProxy(this.rawPtr, intPtr, proxylen);
			StringMarshalHelper.NativeToBuilder(proxy, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000039EF File Offset: 0x00001BEF
		public RESULT setNetworkTimeout(int timeout)
		{
			return System.FMOD_System_SetNetworkTimeout(this.rawPtr, timeout);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x000039FD File Offset: 0x00001BFD
		public RESULT getNetworkTimeout(out int timeout)
		{
			return System.FMOD_System_GetNetworkTimeout(this.rawPtr, out timeout);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00003A0B File Offset: 0x00001C0B
		public RESULT setUserData(IntPtr userdata)
		{
			return System.FMOD_System_SetUserData(this.rawPtr, userdata);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00003A19 File Offset: 0x00001C19
		public RESULT getUserData(out IntPtr userdata)
		{
			return System.FMOD_System_GetUserData(this.rawPtr, out userdata);
		}

		// Token: 0x060000E2 RID: 226
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_Release(IntPtr system);

		// Token: 0x060000E3 RID: 227
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetOutput(IntPtr system, OUTPUTTYPE output);

		// Token: 0x060000E4 RID: 228
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetOutput(IntPtr system, out OUTPUTTYPE output);

		// Token: 0x060000E5 RID: 229
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetNumDrivers(IntPtr system, out int numdrivers);

		// Token: 0x060000E6 RID: 230
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetDriverInfo(IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels);

		// Token: 0x060000E7 RID: 231
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetDriver(IntPtr system, int driver);

		// Token: 0x060000E8 RID: 232
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetDriver(IntPtr system, out int driver);

		// Token: 0x060000E9 RID: 233
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetSoftwareChannels(IntPtr system, int numsoftwarechannels);

		// Token: 0x060000EA RID: 234
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetSoftwareChannels(IntPtr system, out int numsoftwarechannels);

		// Token: 0x060000EB RID: 235
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetSoftwareFormat(IntPtr system, int samplerate, SPEAKERMODE speakermode, int numrawspeakers);

		// Token: 0x060000EC RID: 236
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetSoftwareFormat(IntPtr system, out int samplerate, out SPEAKERMODE speakermode, out int numrawspeakers);

		// Token: 0x060000ED RID: 237
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetDSPBufferSize(IntPtr system, uint bufferlength, int numbuffers);

		// Token: 0x060000EE RID: 238
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetDSPBufferSize(IntPtr system, out uint bufferlength, out int numbuffers);

		// Token: 0x060000EF RID: 239
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetFileSystem(IntPtr system, FILE_OPENCALLBACK useropen, FILE_CLOSECALLBACK userclose, FILE_READCALLBACK userread, FILE_SEEKCALLBACK userseek, FILE_ASYNCREADCALLBACK userasyncread, FILE_ASYNCCANCELCALLBACK userasynccancel, int blockalign);

		// Token: 0x060000F0 RID: 240
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_AttachFileSystem(IntPtr system, FILE_OPENCALLBACK useropen, FILE_CLOSECALLBACK userclose, FILE_READCALLBACK userread, FILE_SEEKCALLBACK userseek);

		// Token: 0x060000F1 RID: 241
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetPluginPath(IntPtr system, byte[] path);

		// Token: 0x060000F2 RID: 242
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_LoadPlugin(IntPtr system, byte[] filename, out uint handle, uint priority);

		// Token: 0x060000F3 RID: 243
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_UnloadPlugin(IntPtr system, uint handle);

		// Token: 0x060000F4 RID: 244
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetNumNestedPlugins(IntPtr system, uint handle, out int count);

		// Token: 0x060000F5 RID: 245
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetNestedPlugin(IntPtr system, uint handle, int index, out uint nestedhandle);

		// Token: 0x060000F6 RID: 246
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetNumPlugins(IntPtr system, PLUGINTYPE plugintype, out int numplugins);

		// Token: 0x060000F7 RID: 247
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetPluginHandle(IntPtr system, PLUGINTYPE plugintype, int index, out uint handle);

		// Token: 0x060000F8 RID: 248
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetPluginInfo(IntPtr system, uint handle, out PLUGINTYPE plugintype, IntPtr name, int namelen, out uint version);

		// Token: 0x060000F9 RID: 249
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_CreateDSPByPlugin(IntPtr system, uint handle, out IntPtr dsp);

		// Token: 0x060000FA RID: 250
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetOutputByPlugin(IntPtr system, uint handle);

		// Token: 0x060000FB RID: 251
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetOutputByPlugin(IntPtr system, out uint handle);

		// Token: 0x060000FC RID: 252
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetDSPInfoByPlugin(IntPtr system, uint handle, out IntPtr description);

		// Token: 0x060000FD RID: 253
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_RegisterDSP(IntPtr system, ref DSP_DESCRIPTION description, out uint handle);

		// Token: 0x060000FE RID: 254
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_Init(IntPtr system, int maxchannels, INITFLAGS flags, IntPtr extradriverdata);

		// Token: 0x060000FF RID: 255
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_Close(IntPtr system);

		// Token: 0x06000100 RID: 256
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_Update(IntPtr system);

		// Token: 0x06000101 RID: 257
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetAdvancedSettings(IntPtr system, ref ADVANCEDSETTINGS settings);

		// Token: 0x06000102 RID: 258
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetAdvancedSettings(IntPtr system, ref ADVANCEDSETTINGS settings);

		// Token: 0x06000103 RID: 259
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_Set3DRolloffCallback(IntPtr system, CB_3D_ROLLOFFCALLBACK callback);

		// Token: 0x06000104 RID: 260
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_MixerSuspend(IntPtr system);

		// Token: 0x06000105 RID: 261
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_MixerResume(IntPtr system);

		// Token: 0x06000106 RID: 262
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetDefaultMixMatrix(IntPtr system, SPEAKERMODE sourcespeakermode, SPEAKERMODE targetspeakermode, float[] matrix, int matrixhop);

		// Token: 0x06000107 RID: 263
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetSpeakerModeChannels(IntPtr system, SPEAKERMODE mode, out int channels);

		// Token: 0x06000108 RID: 264
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetCallback(IntPtr system, SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask);

		// Token: 0x06000109 RID: 265
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetSpeakerPosition(IntPtr system, SPEAKER speaker, float x, float y, bool active);

		// Token: 0x0600010A RID: 266
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetSpeakerPosition(IntPtr system, SPEAKER speaker, out float x, out float y, out bool active);

		// Token: 0x0600010B RID: 267
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_Set3DSettings(IntPtr system, float dopplerscale, float distancefactor, float rolloffscale);

		// Token: 0x0600010C RID: 268
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_Get3DSettings(IntPtr system, out float dopplerscale, out float distancefactor, out float rolloffscale);

		// Token: 0x0600010D RID: 269
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_Set3DNumListeners(IntPtr system, int numlisteners);

		// Token: 0x0600010E RID: 270
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_Get3DNumListeners(IntPtr system, out int numlisteners);

		// Token: 0x0600010F RID: 271
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_Set3DListenerAttributes(IntPtr system, int listener, ref VECTOR pos, ref VECTOR vel, ref VECTOR forward, ref VECTOR up);

		// Token: 0x06000110 RID: 272
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_Get3DListenerAttributes(IntPtr system, int listener, out VECTOR pos, out VECTOR vel, out VECTOR forward, out VECTOR up);

		// Token: 0x06000111 RID: 273
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetStreamBufferSize(IntPtr system, uint filebuffersize, TIMEUNIT filebuffersizetype);

		// Token: 0x06000112 RID: 274
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetStreamBufferSize(IntPtr system, out uint filebuffersize, out TIMEUNIT filebuffersizetype);

		// Token: 0x06000113 RID: 275
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetVersion(IntPtr system, out uint version);

		// Token: 0x06000114 RID: 276
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetOutputHandle(IntPtr system, out IntPtr handle);

		// Token: 0x06000115 RID: 277
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetChannelsPlaying(IntPtr system, out int channels, out int realchannels);

		// Token: 0x06000116 RID: 278
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetCPUUsage(IntPtr system, out float dsp, out float stream, out float geometry, out float update, out float total);

		// Token: 0x06000117 RID: 279
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetFileUsage(IntPtr system, out long sampleBytesRead, out long streamBytesRead, out long otherBytesRead);

		// Token: 0x06000118 RID: 280
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetSoundRAM(IntPtr system, out int currentalloced, out int maxalloced, out int total);

		// Token: 0x06000119 RID: 281
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_CreateSound(IntPtr system, byte[] name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);

		// Token: 0x0600011A RID: 282
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_CreateStream(IntPtr system, byte[] name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);

		// Token: 0x0600011B RID: 283
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_CreateDSP(IntPtr system, ref DSP_DESCRIPTION description, out IntPtr dsp);

		// Token: 0x0600011C RID: 284
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_CreateDSPByType(IntPtr system, DSP_TYPE type, out IntPtr dsp);

		// Token: 0x0600011D RID: 285
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_CreateChannelGroup(IntPtr system, byte[] name, out IntPtr channelgroup);

		// Token: 0x0600011E RID: 286
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_CreateSoundGroup(IntPtr system, byte[] name, out IntPtr soundgroup);

		// Token: 0x0600011F RID: 287
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_CreateReverb3D(IntPtr system, out IntPtr reverb);

		// Token: 0x06000120 RID: 288
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_PlaySound(IntPtr system, IntPtr sound, IntPtr channelGroup, bool paused, out IntPtr channel);

		// Token: 0x06000121 RID: 289
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_PlayDSP(IntPtr system, IntPtr dsp, IntPtr channelGroup, bool paused, out IntPtr channel);

		// Token: 0x06000122 RID: 290
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetChannel(IntPtr system, int channelid, out IntPtr channel);

		// Token: 0x06000123 RID: 291
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetMasterChannelGroup(IntPtr system, out IntPtr channelgroup);

		// Token: 0x06000124 RID: 292
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetMasterSoundGroup(IntPtr system, out IntPtr soundgroup);

		// Token: 0x06000125 RID: 293
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_AttachChannelGroupToPort(IntPtr system, uint portType, ulong portIndex, IntPtr channelgroup, bool passThru);

		// Token: 0x06000126 RID: 294
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_DetachChannelGroupFromPort(IntPtr system, IntPtr channelgroup);

		// Token: 0x06000127 RID: 295
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetReverbProperties(IntPtr system, int instance, ref REVERB_PROPERTIES prop);

		// Token: 0x06000128 RID: 296
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetReverbProperties(IntPtr system, int instance, out REVERB_PROPERTIES prop);

		// Token: 0x06000129 RID: 297
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_LockDSP(IntPtr system);

		// Token: 0x0600012A RID: 298
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_UnlockDSP(IntPtr system);

		// Token: 0x0600012B RID: 299
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetRecordNumDrivers(IntPtr system, out int numdrivers, out int numconnected);

		// Token: 0x0600012C RID: 300
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetRecordDriverInfo(IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels, out DRIVER_STATE state);

		// Token: 0x0600012D RID: 301
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetRecordPosition(IntPtr system, int id, out uint position);

		// Token: 0x0600012E RID: 302
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_RecordStart(IntPtr system, int id, IntPtr sound, bool loop);

		// Token: 0x0600012F RID: 303
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_RecordStop(IntPtr system, int id);

		// Token: 0x06000130 RID: 304
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_IsRecording(IntPtr system, int id, out bool recording);

		// Token: 0x06000131 RID: 305
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_CreateGeometry(IntPtr system, int maxpolygons, int maxvertices, out IntPtr geometry);

		// Token: 0x06000132 RID: 306
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetGeometrySettings(IntPtr system, float maxworldsize);

		// Token: 0x06000133 RID: 307
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetGeometrySettings(IntPtr system, out float maxworldsize);

		// Token: 0x06000134 RID: 308
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_LoadGeometry(IntPtr system, IntPtr data, int datasize, out IntPtr geometry);

		// Token: 0x06000135 RID: 309
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetGeometryOcclusion(IntPtr system, ref VECTOR listener, ref VECTOR source, out float direct, out float reverb);

		// Token: 0x06000136 RID: 310
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetNetworkProxy(IntPtr system, byte[] proxy);

		// Token: 0x06000137 RID: 311
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetNetworkProxy(IntPtr system, IntPtr proxy, int proxylen);

		// Token: 0x06000138 RID: 312
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetNetworkTimeout(IntPtr system, int timeout);

		// Token: 0x06000139 RID: 313
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetNetworkTimeout(IntPtr system, out int timeout);

		// Token: 0x0600013A RID: 314
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_SetUserData(IntPtr system, IntPtr userdata);

		// Token: 0x0600013B RID: 315
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_GetUserData(IntPtr system, out IntPtr userdata);

		// Token: 0x0600013C RID: 316 RVA: 0x00003A27 File Offset: 0x00001C27
		public System(IntPtr raw) : base(raw)
		{
		}
	}
}
