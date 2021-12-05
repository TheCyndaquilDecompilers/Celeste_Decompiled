using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	// Token: 0x020000DE RID: 222
	public class System : HandleBase
	{
		// Token: 0x060003C4 RID: 964 RVA: 0x000052E8 File Offset: 0x000034E8
		public static RESULT create(out System studiosystem)
		{
			studiosystem = null;
			IntPtr raw;
			RESULT result = System.FMOD_Studio_System_Create(out raw, 69652U);
			if (result != RESULT.OK)
			{
				return result;
			}
			studiosystem = new System(raw);
			return result;
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x00005315 File Offset: 0x00003515
		public RESULT setAdvancedSettings(ADVANCEDSETTINGS settings)
		{
			settings.cbsize = Marshal.SizeOf(typeof(ADVANCEDSETTINGS));
			return System.FMOD_Studio_System_SetAdvancedSettings(this.rawPtr, ref settings);
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0000533A File Offset: 0x0000353A
		public RESULT getAdvancedSettings(out ADVANCEDSETTINGS settings)
		{
			settings.cbsize = Marshal.SizeOf(typeof(ADVANCEDSETTINGS));
			return System.FMOD_Studio_System_GetAdvancedSettings(this.rawPtr, out settings);
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0000535D File Offset: 0x0000355D
		public RESULT initialize(int maxchannels, INITFLAGS studioFlags, INITFLAGS flags, IntPtr extradriverdata)
		{
			return System.FMOD_Studio_System_Initialize(this.rawPtr, maxchannels, studioFlags, flags, extradriverdata);
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x0000536F File Offset: 0x0000356F
		public RESULT release()
		{
			return System.FMOD_Studio_System_Release(this.rawPtr);
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x0000537C File Offset: 0x0000357C
		public RESULT update()
		{
			return System.FMOD_Studio_System_Update(this.rawPtr);
		}

		// Token: 0x060003CA RID: 970 RVA: 0x0000538C File Offset: 0x0000358C
		public RESULT getLowLevelSystem(out System system)
		{
			system = null;
			IntPtr raw = 0;
			RESULT result = System.FMOD_Studio_System_GetLowLevelSystem(this.rawPtr, out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			system = new System(raw);
			return result;
		}

		// Token: 0x060003CB RID: 971 RVA: 0x000053C0 File Offset: 0x000035C0
		public RESULT getEvent(string path, out EventDescription _event)
		{
			_event = null;
			IntPtr raw = 0;
			RESULT result = System.FMOD_Studio_System_GetEvent(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			_event = new EventDescription(raw);
			return result;
		}

		// Token: 0x060003CC RID: 972 RVA: 0x0000540C File Offset: 0x0000360C
		public RESULT getBus(string path, out Bus bus)
		{
			bus = null;
			IntPtr raw = 0;
			RESULT result = System.FMOD_Studio_System_GetBus(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			bus = new Bus(raw);
			return result;
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00005458 File Offset: 0x00003658
		public RESULT getVCA(string path, out VCA vca)
		{
			vca = null;
			IntPtr raw = 0;
			RESULT result = System.FMOD_Studio_System_GetVCA(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			vca = new VCA(raw);
			return result;
		}

		// Token: 0x060003CE RID: 974 RVA: 0x000054A4 File Offset: 0x000036A4
		public RESULT getBank(string path, out Bank bank)
		{
			bank = null;
			IntPtr raw = 0;
			RESULT result = System.FMOD_Studio_System_GetBank(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			bank = new Bank(raw);
			return result;
		}

		// Token: 0x060003CF RID: 975 RVA: 0x000054F0 File Offset: 0x000036F0
		public RESULT getEventByID(Guid guid, out EventDescription _event)
		{
			_event = null;
			IntPtr raw = 0;
			RESULT result = System.FMOD_Studio_System_GetEventByID(this.rawPtr, ref guid, out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			_event = new EventDescription(raw);
			return result;
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00005528 File Offset: 0x00003728
		public RESULT getBusByID(Guid guid, out Bus bus)
		{
			bus = null;
			IntPtr raw = 0;
			RESULT result = System.FMOD_Studio_System_GetBusByID(this.rawPtr, ref guid, out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			bus = new Bus(raw);
			return result;
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00005560 File Offset: 0x00003760
		public RESULT getVCAByID(Guid guid, out VCA vca)
		{
			vca = null;
			IntPtr raw = 0;
			RESULT result = System.FMOD_Studio_System_GetVCAByID(this.rawPtr, ref guid, out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			vca = new VCA(raw);
			return result;
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00005598 File Offset: 0x00003798
		public RESULT getBankByID(Guid guid, out Bank bank)
		{
			bank = null;
			IntPtr raw = 0;
			RESULT result = System.FMOD_Studio_System_GetBankByID(this.rawPtr, ref guid, out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			bank = new Bank(raw);
			return result;
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x000055D0 File Offset: 0x000037D0
		public RESULT getSoundInfo(string key, out SOUND_INFO info)
		{
			SOUND_INFO_INTERNAL sound_INFO_INTERNAL;
			RESULT result = System.FMOD_Studio_System_GetSoundInfo(this.rawPtr, Encoding.UTF8.GetBytes(key + "\0"), out sound_INFO_INTERNAL);
			if (result != RESULT.OK)
			{
				info = new SOUND_INFO();
				return result;
			}
			sound_INFO_INTERNAL.assign(out info);
			return result;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x00005615 File Offset: 0x00003815
		public RESULT lookupID(string path, out Guid guid)
		{
			return System.FMOD_Studio_System_LookupID(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), out guid);
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00005638 File Offset: 0x00003838
		public RESULT lookupPath(Guid guid, out string path)
		{
			path = null;
			byte[] array = new byte[256];
			int num = 0;
			RESULT result = System.FMOD_Studio_System_LookupPath(this.rawPtr, ref guid, array, array.Length, out num);
			if (result == RESULT.ERR_TRUNCATED)
			{
				array = new byte[num];
				result = System.FMOD_Studio_System_LookupPath(this.rawPtr, ref guid, array, array.Length, out num);
			}
			if (result == RESULT.OK)
			{
				path = Encoding.UTF8.GetString(array, 0, num - 1);
			}
			return result;
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x0000569E File Offset: 0x0000389E
		public RESULT getNumListeners(out int numlisteners)
		{
			return System.FMOD_Studio_System_GetNumListeners(this.rawPtr, out numlisteners);
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x000056AC File Offset: 0x000038AC
		public RESULT setNumListeners(int numlisteners)
		{
			return System.FMOD_Studio_System_SetNumListeners(this.rawPtr, numlisteners);
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x000056BA File Offset: 0x000038BA
		public RESULT getListenerAttributes(int listener, out _3D_ATTRIBUTES attributes)
		{
			return System.FMOD_Studio_System_GetListenerAttributes(this.rawPtr, listener, out attributes);
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x000056C9 File Offset: 0x000038C9
		public RESULT setListenerAttributes(int listener, _3D_ATTRIBUTES attributes)
		{
			return System.FMOD_Studio_System_SetListenerAttributes(this.rawPtr, listener, ref attributes);
		}

		// Token: 0x060003DA RID: 986 RVA: 0x000056D9 File Offset: 0x000038D9
		public RESULT getListenerWeight(int listener, out float weight)
		{
			return System.FMOD_Studio_System_GetListenerWeight(this.rawPtr, listener, out weight);
		}

		// Token: 0x060003DB RID: 987 RVA: 0x000056E8 File Offset: 0x000038E8
		public RESULT setListenerWeight(int listener, float weight)
		{
			return System.FMOD_Studio_System_SetListenerWeight(this.rawPtr, listener, weight);
		}

		// Token: 0x060003DC RID: 988 RVA: 0x000056F8 File Offset: 0x000038F8
		public RESULT loadBankFile(string name, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			bank = null;
			IntPtr raw = 0;
			RESULT result = System.FMOD_Studio_System_LoadBankFile(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), flags, out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			bank = new Bank(raw);
			return result;
		}

		// Token: 0x060003DD RID: 989 RVA: 0x00005744 File Offset: 0x00003944
		public RESULT loadBankMemory(byte[] buffer, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			bank = null;
			IntPtr raw = 0;
			RESULT result = System.FMOD_Studio_System_LoadBankMemory(this.rawPtr, buffer, buffer.Length, LOAD_MEMORY_MODE.LOAD_MEMORY, flags, out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			bank = new Bank(raw);
			return result;
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00005780 File Offset: 0x00003980
		public RESULT loadBankCustom(BANK_INFO info, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			bank = null;
			info.size = Marshal.SizeOf(info);
			IntPtr raw = 0;
			RESULT result = System.FMOD_Studio_System_LoadBankCustom(this.rawPtr, ref info, flags, out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			bank = new Bank(raw);
			return result;
		}

		// Token: 0x060003DF RID: 991 RVA: 0x000057C9 File Offset: 0x000039C9
		public RESULT unloadAll()
		{
			return System.FMOD_Studio_System_UnloadAll(this.rawPtr);
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x000057D6 File Offset: 0x000039D6
		public RESULT flushCommands()
		{
			return System.FMOD_Studio_System_FlushCommands(this.rawPtr);
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x000057E3 File Offset: 0x000039E3
		public RESULT flushSampleLoading()
		{
			return System.FMOD_Studio_System_FlushSampleLoading(this.rawPtr);
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x000057F0 File Offset: 0x000039F0
		public RESULT startCommandCapture(string path, COMMANDCAPTURE_FLAGS flags)
		{
			return System.FMOD_Studio_System_StartCommandCapture(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), flags);
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x00005813 File Offset: 0x00003A13
		public RESULT stopCommandCapture()
		{
			return System.FMOD_Studio_System_StopCommandCapture(this.rawPtr);
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x00005820 File Offset: 0x00003A20
		public RESULT loadCommandReplay(string path, COMMANDREPLAY_FLAGS flags, out CommandReplay replay)
		{
			replay = null;
			IntPtr raw = 0;
			RESULT result = System.FMOD_Studio_System_LoadCommandReplay(this.rawPtr, Encoding.UTF8.GetBytes(path + "\0"), flags, out raw);
			if (result == RESULT.OK)
			{
				replay = new CommandReplay(raw);
			}
			return result;
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x00005866 File Offset: 0x00003A66
		public RESULT getBankCount(out int count)
		{
			return System.FMOD_Studio_System_GetBankCount(this.rawPtr, out count);
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x00005874 File Offset: 0x00003A74
		public RESULT getBankList(out Bank[] array)
		{
			array = null;
			int num;
			RESULT result = System.FMOD_Studio_System_GetBankCount(this.rawPtr, out num);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num == 0)
			{
				array = new Bank[0];
				return result;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			result = System.FMOD_Studio_System_GetBankList(this.rawPtr, array2, num, out num2);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new Bank[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = new Bank(array2[i]);
			}
			return RESULT.OK;
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x000058ED File Offset: 0x00003AED
		public RESULT getCPUUsage(out CPU_USAGE usage)
		{
			return System.FMOD_Studio_System_GetCPUUsage(this.rawPtr, out usage);
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x000058FB File Offset: 0x00003AFB
		public RESULT getBufferUsage(out BUFFER_USAGE usage)
		{
			return System.FMOD_Studio_System_GetBufferUsage(this.rawPtr, out usage);
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x00005909 File Offset: 0x00003B09
		public RESULT resetBufferUsage()
		{
			return System.FMOD_Studio_System_ResetBufferUsage(this.rawPtr);
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x00005916 File Offset: 0x00003B16
		public RESULT setCallback(SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask = SYSTEM_CALLBACK_TYPE.ALL)
		{
			return System.FMOD_Studio_System_SetCallback(this.rawPtr, callback, callbackmask);
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00005925 File Offset: 0x00003B25
		public RESULT getUserData(out IntPtr userdata)
		{
			return System.FMOD_Studio_System_GetUserData(this.rawPtr, out userdata);
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x00005933 File Offset: 0x00003B33
		public RESULT setUserData(IntPtr userdata)
		{
			return System.FMOD_Studio_System_SetUserData(this.rawPtr, userdata);
		}

		// Token: 0x060003ED RID: 1005
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Create(out IntPtr studiosystem, uint headerversion);

		// Token: 0x060003EE RID: 1006
		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_System_IsValid(IntPtr studiosystem);

		// Token: 0x060003EF RID: 1007
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetAdvancedSettings(IntPtr studiosystem, ref ADVANCEDSETTINGS settings);

		// Token: 0x060003F0 RID: 1008
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetAdvancedSettings(IntPtr studiosystem, out ADVANCEDSETTINGS settings);

		// Token: 0x060003F1 RID: 1009
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Initialize(IntPtr studiosystem, int maxchannels, INITFLAGS studioFlags, INITFLAGS flags, IntPtr extradriverdata);

		// Token: 0x060003F2 RID: 1010
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Release(IntPtr studiosystem);

		// Token: 0x060003F3 RID: 1011
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Update(IntPtr studiosystem);

		// Token: 0x060003F4 RID: 1012
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetLowLevelSystem(IntPtr studiosystem, out IntPtr system);

		// Token: 0x060003F5 RID: 1013
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetEvent(IntPtr studiosystem, byte[] path, out IntPtr description);

		// Token: 0x060003F6 RID: 1014
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBus(IntPtr studiosystem, byte[] path, out IntPtr bus);

		// Token: 0x060003F7 RID: 1015
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetVCA(IntPtr studiosystem, byte[] path, out IntPtr vca);

		// Token: 0x060003F8 RID: 1016
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBank(IntPtr studiosystem, byte[] path, out IntPtr bank);

		// Token: 0x060003F9 RID: 1017
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetEventByID(IntPtr studiosystem, ref Guid guid, out IntPtr description);

		// Token: 0x060003FA RID: 1018
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBusByID(IntPtr studiosystem, ref Guid guid, out IntPtr bus);

		// Token: 0x060003FB RID: 1019
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetVCAByID(IntPtr studiosystem, ref Guid guid, out IntPtr vca);

		// Token: 0x060003FC RID: 1020
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBankByID(IntPtr studiosystem, ref Guid guid, out IntPtr bank);

		// Token: 0x060003FD RID: 1021
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetSoundInfo(IntPtr studiosystem, byte[] key, out SOUND_INFO_INTERNAL info);

		// Token: 0x060003FE RID: 1022
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LookupID(IntPtr studiosystem, byte[] path, out Guid guid);

		// Token: 0x060003FF RID: 1023
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LookupPath(IntPtr studiosystem, ref Guid guid, [Out] byte[] path, int size, out int retrieved);

		// Token: 0x06000400 RID: 1024
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetNumListeners(IntPtr studiosystem, out int numlisteners);

		// Token: 0x06000401 RID: 1025
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetNumListeners(IntPtr studiosystem, int numlisteners);

		// Token: 0x06000402 RID: 1026
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetListenerAttributes(IntPtr studiosystem, int listener, out _3D_ATTRIBUTES attributes);

		// Token: 0x06000403 RID: 1027
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetListenerAttributes(IntPtr studiosystem, int listener, ref _3D_ATTRIBUTES attributes);

		// Token: 0x06000404 RID: 1028
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetListenerWeight(IntPtr studiosystem, int listener, out float weight);

		// Token: 0x06000405 RID: 1029
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetListenerWeight(IntPtr studiosystem, int listener, float weight);

		// Token: 0x06000406 RID: 1030
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadBankFile(IntPtr studiosystem, byte[] filename, LOAD_BANK_FLAGS flags, out IntPtr bank);

		// Token: 0x06000407 RID: 1031
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadBankMemory(IntPtr studiosystem, byte[] buffer, int length, LOAD_MEMORY_MODE mode, LOAD_BANK_FLAGS flags, out IntPtr bank);

		// Token: 0x06000408 RID: 1032
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadBankCustom(IntPtr studiosystem, ref BANK_INFO info, LOAD_BANK_FLAGS flags, out IntPtr bank);

		// Token: 0x06000409 RID: 1033
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_UnloadAll(IntPtr studiosystem);

		// Token: 0x0600040A RID: 1034
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_FlushCommands(IntPtr studiosystem);

		// Token: 0x0600040B RID: 1035
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_FlushSampleLoading(IntPtr studiosystem);

		// Token: 0x0600040C RID: 1036
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_StartCommandCapture(IntPtr studiosystem, byte[] path, COMMANDCAPTURE_FLAGS flags);

		// Token: 0x0600040D RID: 1037
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_StopCommandCapture(IntPtr studiosystem);

		// Token: 0x0600040E RID: 1038
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadCommandReplay(IntPtr studiosystem, byte[] path, COMMANDREPLAY_FLAGS flags, out IntPtr commandReplay);

		// Token: 0x0600040F RID: 1039
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBankCount(IntPtr studiosystem, out int count);

		// Token: 0x06000410 RID: 1040
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBankList(IntPtr studiosystem, IntPtr[] array, int capacity, out int count);

		// Token: 0x06000411 RID: 1041
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetCPUUsage(IntPtr studiosystem, out CPU_USAGE usage);

		// Token: 0x06000412 RID: 1042
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBufferUsage(IntPtr studiosystem, out BUFFER_USAGE usage);

		// Token: 0x06000413 RID: 1043
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_ResetBufferUsage(IntPtr studiosystem);

		// Token: 0x06000414 RID: 1044
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetCallback(IntPtr studiosystem, SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask);

		// Token: 0x06000415 RID: 1045
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetUserData(IntPtr studiosystem, out IntPtr userdata);

		// Token: 0x06000416 RID: 1046
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetUserData(IntPtr studiosystem, IntPtr userdata);

		// Token: 0x06000417 RID: 1047 RVA: 0x00005941 File Offset: 0x00003B41
		public System(IntPtr raw) : base(raw)
		{
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x0000594A File Offset: 0x00003B4A
		protected override bool isValidInternal()
		{
			return System.FMOD_Studio_System_IsValid(this.rawPtr);
		}
	}
}
