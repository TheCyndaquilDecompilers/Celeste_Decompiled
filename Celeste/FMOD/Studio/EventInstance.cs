using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	// Token: 0x020000E0 RID: 224
	public class EventInstance : HandleBase
	{
		// Token: 0x06000452 RID: 1106 RVA: 0x00005C8C File Offset: 0x00003E8C
		public RESULT getDescription(out EventDescription description)
		{
			description = null;
			IntPtr raw;
			RESULT result = EventInstance.FMOD_Studio_EventInstance_GetDescription(this.rawPtr, out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			description = new EventDescription(raw);
			return result;
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x00005CB8 File Offset: 0x00003EB8
		public RESULT getVolume(out float volume, out float finalvolume)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetVolume(this.rawPtr, out volume, out finalvolume);
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x00005CC7 File Offset: 0x00003EC7
		public RESULT setVolume(float volume)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetVolume(this.rawPtr, volume);
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x00005CD5 File Offset: 0x00003ED5
		public RESULT getPitch(out float pitch, out float finalpitch)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetPitch(this.rawPtr, out pitch, out finalpitch);
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x00005CE4 File Offset: 0x00003EE4
		public RESULT setPitch(float pitch)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetPitch(this.rawPtr, pitch);
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x00005CF2 File Offset: 0x00003EF2
		public RESULT get3DAttributes(out _3D_ATTRIBUTES attributes)
		{
			return EventInstance.FMOD_Studio_EventInstance_Get3DAttributes(this.rawPtr, out attributes);
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x00005D00 File Offset: 0x00003F00
		public RESULT set3DAttributes(_3D_ATTRIBUTES attributes)
		{
			return EventInstance.FMOD_Studio_EventInstance_Set3DAttributes(this.rawPtr, ref attributes);
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x00005D0F File Offset: 0x00003F0F
		public RESULT getListenerMask(out uint mask)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetListenerMask(this.rawPtr, out mask);
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x00005D1D File Offset: 0x00003F1D
		public RESULT setListenerMask(uint mask)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetListenerMask(this.rawPtr, mask);
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x00005D2B File Offset: 0x00003F2B
		public RESULT getProperty(EVENT_PROPERTY index, out float value)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetProperty(this.rawPtr, index, out value);
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x00005D3A File Offset: 0x00003F3A
		public RESULT setProperty(EVENT_PROPERTY index, float value)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetProperty(this.rawPtr, index, value);
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x00005D49 File Offset: 0x00003F49
		public RESULT getReverbLevel(int index, out float level)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetReverbLevel(this.rawPtr, index, out level);
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x00005D58 File Offset: 0x00003F58
		public RESULT setReverbLevel(int index, float level)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetReverbLevel(this.rawPtr, index, level);
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x00005D67 File Offset: 0x00003F67
		public RESULT getPaused(out bool paused)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetPaused(this.rawPtr, out paused);
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x00005D75 File Offset: 0x00003F75
		public RESULT setPaused(bool paused)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetPaused(this.rawPtr, paused);
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x00005D83 File Offset: 0x00003F83
		public RESULT start()
		{
			return EventInstance.FMOD_Studio_EventInstance_Start(this.rawPtr);
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x00005D90 File Offset: 0x00003F90
		public RESULT stop(STOP_MODE mode)
		{
			return EventInstance.FMOD_Studio_EventInstance_Stop(this.rawPtr, mode);
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x00005D9E File Offset: 0x00003F9E
		public RESULT getTimelinePosition(out int position)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetTimelinePosition(this.rawPtr, out position);
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x00005DAC File Offset: 0x00003FAC
		public RESULT setTimelinePosition(int position)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetTimelinePosition(this.rawPtr, position);
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x00005DBA File Offset: 0x00003FBA
		public RESULT getPlaybackState(out PLAYBACK_STATE state)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetPlaybackState(this.rawPtr, out state);
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x00005DC8 File Offset: 0x00003FC8
		public RESULT getChannelGroup(out ChannelGroup group)
		{
			group = null;
			IntPtr raw = 0;
			RESULT result = EventInstance.FMOD_Studio_EventInstance_GetChannelGroup(this.rawPtr, out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			group = new ChannelGroup(raw);
			return result;
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x00005DFC File Offset: 0x00003FFC
		public RESULT release()
		{
			return EventInstance.FMOD_Studio_EventInstance_Release(this.rawPtr);
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x00005E09 File Offset: 0x00004009
		public RESULT isVirtual(out bool virtualstate)
		{
			return EventInstance.FMOD_Studio_EventInstance_IsVirtual(this.rawPtr, out virtualstate);
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x00005E18 File Offset: 0x00004018
		public RESULT getParameter(string name, out ParameterInstance instance)
		{
			instance = null;
			IntPtr raw = 0;
			RESULT result = EventInstance.FMOD_Studio_EventInstance_GetParameter(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			instance = new ParameterInstance(raw);
			return result;
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x00005E61 File Offset: 0x00004061
		public RESULT getParameterCount(out int count)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetParameterCount(this.rawPtr, out count);
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x00005E70 File Offset: 0x00004070
		public RESULT getParameterByIndex(int index, out ParameterInstance instance)
		{
			instance = null;
			IntPtr raw = 0;
			RESULT result = EventInstance.FMOD_Studio_EventInstance_GetParameterByIndex(this.rawPtr, index, out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			instance = new ParameterInstance(raw);
			return result;
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x00005EA5 File Offset: 0x000040A5
		public RESULT getParameterValue(string name, out float value, out float finalvalue)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetParameterValue(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), out value, out finalvalue);
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x00005EC9 File Offset: 0x000040C9
		public RESULT setParameterValue(string name, float value)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetParameterValue(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), value);
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x00005EEC File Offset: 0x000040EC
		public RESULT getParameterValueByIndex(int index, out float value, out float finalvalue)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetParameterValueByIndex(this.rawPtr, index, out value, out finalvalue);
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x00005EFC File Offset: 0x000040FC
		public RESULT setParameterValueByIndex(int index, float value)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetParameterValueByIndex(this.rawPtr, index, value);
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x00005F0B File Offset: 0x0000410B
		public RESULT setParameterValuesByIndices(int[] indices, float[] values, int count)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetParameterValuesByIndices(this.rawPtr, indices, values, count);
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x00005F1B File Offset: 0x0000411B
		public RESULT triggerCue()
		{
			return EventInstance.FMOD_Studio_EventInstance_TriggerCue(this.rawPtr);
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x00005F28 File Offset: 0x00004128
		public RESULT setCallback(EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask = EVENT_CALLBACK_TYPE.ALL)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetCallback(this.rawPtr, callback, callbackmask);
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x00005F37 File Offset: 0x00004137
		public RESULT getUserData(out IntPtr userdata)
		{
			return EventInstance.FMOD_Studio_EventInstance_GetUserData(this.rawPtr, out userdata);
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x00005F45 File Offset: 0x00004145
		public RESULT setUserData(IntPtr userdata)
		{
			return EventInstance.FMOD_Studio_EventInstance_SetUserData(this.rawPtr, userdata);
		}

		// Token: 0x06000475 RID: 1141
		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_EventInstance_IsValid(IntPtr _event);

		// Token: 0x06000476 RID: 1142
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetDescription(IntPtr _event, out IntPtr description);

		// Token: 0x06000477 RID: 1143
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetVolume(IntPtr _event, out float volume, out float finalvolume);

		// Token: 0x06000478 RID: 1144
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetVolume(IntPtr _event, float volume);

		// Token: 0x06000479 RID: 1145
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetPitch(IntPtr _event, out float pitch, out float finalpitch);

		// Token: 0x0600047A RID: 1146
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetPitch(IntPtr _event, float pitch);

		// Token: 0x0600047B RID: 1147
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Get3DAttributes(IntPtr _event, out _3D_ATTRIBUTES attributes);

		// Token: 0x0600047C RID: 1148
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Set3DAttributes(IntPtr _event, ref _3D_ATTRIBUTES attributes);

		// Token: 0x0600047D RID: 1149
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetListenerMask(IntPtr _event, out uint mask);

		// Token: 0x0600047E RID: 1150
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetListenerMask(IntPtr _event, uint mask);

		// Token: 0x0600047F RID: 1151
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetProperty(IntPtr _event, EVENT_PROPERTY index, out float value);

		// Token: 0x06000480 RID: 1152
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetProperty(IntPtr _event, EVENT_PROPERTY index, float value);

		// Token: 0x06000481 RID: 1153
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetReverbLevel(IntPtr _event, int index, out float level);

		// Token: 0x06000482 RID: 1154
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetReverbLevel(IntPtr _event, int index, float level);

		// Token: 0x06000483 RID: 1155
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetPaused(IntPtr _event, out bool paused);

		// Token: 0x06000484 RID: 1156
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetPaused(IntPtr _event, bool paused);

		// Token: 0x06000485 RID: 1157
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Start(IntPtr _event);

		// Token: 0x06000486 RID: 1158
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Stop(IntPtr _event, STOP_MODE mode);

		// Token: 0x06000487 RID: 1159
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetTimelinePosition(IntPtr _event, out int position);

		// Token: 0x06000488 RID: 1160
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetTimelinePosition(IntPtr _event, int position);

		// Token: 0x06000489 RID: 1161
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetPlaybackState(IntPtr _event, out PLAYBACK_STATE state);

		// Token: 0x0600048A RID: 1162
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetChannelGroup(IntPtr _event, out IntPtr group);

		// Token: 0x0600048B RID: 1163
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Release(IntPtr _event);

		// Token: 0x0600048C RID: 1164
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_IsVirtual(IntPtr _event, out bool virtualstate);

		// Token: 0x0600048D RID: 1165
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameter(IntPtr _event, byte[] name, out IntPtr parameter);

		// Token: 0x0600048E RID: 1166
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameterByIndex(IntPtr _event, int index, out IntPtr parameter);

		// Token: 0x0600048F RID: 1167
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameterCount(IntPtr _event, out int count);

		// Token: 0x06000490 RID: 1168
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameterValue(IntPtr _event, byte[] name, out float value, out float finalvalue);

		// Token: 0x06000491 RID: 1169
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetParameterValue(IntPtr _event, byte[] name, float value);

		// Token: 0x06000492 RID: 1170
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameterValueByIndex(IntPtr _event, int index, out float value, out float finalvalue);

		// Token: 0x06000493 RID: 1171
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetParameterValueByIndex(IntPtr _event, int index, float value);

		// Token: 0x06000494 RID: 1172
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetParameterValuesByIndices(IntPtr _event, int[] indices, float[] values, int count);

		// Token: 0x06000495 RID: 1173
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_TriggerCue(IntPtr _event);

		// Token: 0x06000496 RID: 1174
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetCallback(IntPtr _event, EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask);

		// Token: 0x06000497 RID: 1175
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetUserData(IntPtr _event, out IntPtr userdata);

		// Token: 0x06000498 RID: 1176
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetUserData(IntPtr _event, IntPtr userdata);

		// Token: 0x06000499 RID: 1177 RVA: 0x00005941 File Offset: 0x00003B41
		public EventInstance(IntPtr raw) : base(raw)
		{
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x00005F53 File Offset: 0x00004153
		protected override bool isValidInternal()
		{
			return EventInstance.FMOD_Studio_EventInstance_IsValid(this.rawPtr);
		}
	}
}
