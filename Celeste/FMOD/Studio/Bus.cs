using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	// Token: 0x020000E2 RID: 226
	public class Bus : HandleBase
	{
		// Token: 0x060004A4 RID: 1188 RVA: 0x00005FB9 File Offset: 0x000041B9
		public RESULT getID(out Guid id)
		{
			return Bus.FMOD_Studio_Bus_GetID(this.rawPtr, out id);
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x00005FC8 File Offset: 0x000041C8
		public RESULT getPath(out string path)
		{
			path = null;
			byte[] array = new byte[256];
			int num = 0;
			RESULT result = Bus.FMOD_Studio_Bus_GetPath(this.rawPtr, array, array.Length, out num);
			if (result == RESULT.ERR_TRUNCATED)
			{
				array = new byte[num];
				result = Bus.FMOD_Studio_Bus_GetPath(this.rawPtr, array, array.Length, out num);
			}
			if (result == RESULT.OK)
			{
				path = Encoding.UTF8.GetString(array, 0, num - 1);
			}
			return result;
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x0000602A File Offset: 0x0000422A
		public RESULT getVolume(out float volume, out float finalvolume)
		{
			return Bus.FMOD_Studio_Bus_GetVolume(this.rawPtr, out volume, out finalvolume);
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x00006039 File Offset: 0x00004239
		public RESULT setVolume(float volume)
		{
			return Bus.FMOD_Studio_Bus_SetVolume(this.rawPtr, volume);
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x00006047 File Offset: 0x00004247
		public RESULT getPaused(out bool paused)
		{
			return Bus.FMOD_Studio_Bus_GetPaused(this.rawPtr, out paused);
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x00006055 File Offset: 0x00004255
		public RESULT setPaused(bool paused)
		{
			return Bus.FMOD_Studio_Bus_SetPaused(this.rawPtr, paused);
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00006063 File Offset: 0x00004263
		public RESULT getMute(out bool mute)
		{
			return Bus.FMOD_Studio_Bus_GetMute(this.rawPtr, out mute);
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00006071 File Offset: 0x00004271
		public RESULT setMute(bool mute)
		{
			return Bus.FMOD_Studio_Bus_SetMute(this.rawPtr, mute);
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x0000607F File Offset: 0x0000427F
		public RESULT stopAllEvents(STOP_MODE mode)
		{
			return Bus.FMOD_Studio_Bus_StopAllEvents(this.rawPtr, mode);
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x0000608D File Offset: 0x0000428D
		public RESULT lockChannelGroup()
		{
			return Bus.FMOD_Studio_Bus_LockChannelGroup(this.rawPtr);
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x0000609A File Offset: 0x0000429A
		public RESULT unlockChannelGroup()
		{
			return Bus.FMOD_Studio_Bus_UnlockChannelGroup(this.rawPtr);
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x000060A8 File Offset: 0x000042A8
		public RESULT getChannelGroup(out ChannelGroup group)
		{
			group = null;
			IntPtr raw = 0;
			RESULT result = Bus.FMOD_Studio_Bus_GetChannelGroup(this.rawPtr, out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			group = new ChannelGroup(raw);
			return result;
		}

		// Token: 0x060004B0 RID: 1200
		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_Bus_IsValid(IntPtr bus);

		// Token: 0x060004B1 RID: 1201
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetID(IntPtr bus, out Guid id);

		// Token: 0x060004B2 RID: 1202
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetPath(IntPtr bus, [Out] byte[] path, int size, out int retrieved);

		// Token: 0x060004B3 RID: 1203
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetVolume(IntPtr bus, out float volume, out float finalvolume);

		// Token: 0x060004B4 RID: 1204
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_SetVolume(IntPtr bus, float volume);

		// Token: 0x060004B5 RID: 1205
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetPaused(IntPtr bus, out bool paused);

		// Token: 0x060004B6 RID: 1206
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_SetPaused(IntPtr bus, bool paused);

		// Token: 0x060004B7 RID: 1207
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetMute(IntPtr bus, out bool mute);

		// Token: 0x060004B8 RID: 1208
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_SetMute(IntPtr bus, bool mute);

		// Token: 0x060004B9 RID: 1209
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_StopAllEvents(IntPtr bus, STOP_MODE mode);

		// Token: 0x060004BA RID: 1210
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_LockChannelGroup(IntPtr bus);

		// Token: 0x060004BB RID: 1211
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_UnlockChannelGroup(IntPtr bus);

		// Token: 0x060004BC RID: 1212
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetChannelGroup(IntPtr bus, out IntPtr group);

		// Token: 0x060004BD RID: 1213 RVA: 0x00005941 File Offset: 0x00003B41
		public Bus(IntPtr raw) : base(raw)
		{
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x000060DC File Offset: 0x000042DC
		protected override bool isValidInternal()
		{
			return Bus.FMOD_Studio_Bus_IsValid(this.rawPtr);
		}
	}
}
