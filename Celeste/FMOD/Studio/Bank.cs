using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	// Token: 0x020000E4 RID: 228
	public class Bank : HandleBase
	{
		// Token: 0x060004CA RID: 1226 RVA: 0x00006184 File Offset: 0x00004384
		public RESULT getID(out Guid id)
		{
			return Bank.FMOD_Studio_Bank_GetID(this.rawPtr, out id);
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x00006194 File Offset: 0x00004394
		public RESULT getPath(out string path)
		{
			path = null;
			byte[] array = new byte[256];
			int num = 0;
			RESULT result = Bank.FMOD_Studio_Bank_GetPath(this.rawPtr, array, array.Length, out num);
			if (result == RESULT.ERR_TRUNCATED)
			{
				array = new byte[num];
				result = Bank.FMOD_Studio_Bank_GetPath(this.rawPtr, array, array.Length, out num);
			}
			if (result == RESULT.OK)
			{
				path = Encoding.UTF8.GetString(array, 0, num - 1);
			}
			return result;
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x000061F8 File Offset: 0x000043F8
		public RESULT unload()
		{
			RESULT result = Bank.FMOD_Studio_Bank_Unload(this.rawPtr);
			if (result != RESULT.OK)
			{
				return result;
			}
			this.rawPtr = IntPtr.Zero;
			return RESULT.OK;
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x00006222 File Offset: 0x00004422
		public RESULT loadSampleData()
		{
			return Bank.FMOD_Studio_Bank_LoadSampleData(this.rawPtr);
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x0000622F File Offset: 0x0000442F
		public RESULT unloadSampleData()
		{
			return Bank.FMOD_Studio_Bank_UnloadSampleData(this.rawPtr);
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0000623C File Offset: 0x0000443C
		public RESULT getLoadingState(out LOADING_STATE state)
		{
			return Bank.FMOD_Studio_Bank_GetLoadingState(this.rawPtr, out state);
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0000624A File Offset: 0x0000444A
		public RESULT getSampleLoadingState(out LOADING_STATE state)
		{
			return Bank.FMOD_Studio_Bank_GetSampleLoadingState(this.rawPtr, out state);
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x00006258 File Offset: 0x00004458
		public RESULT getStringCount(out int count)
		{
			return Bank.FMOD_Studio_Bank_GetStringCount(this.rawPtr, out count);
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x00006268 File Offset: 0x00004468
		public RESULT getStringInfo(int index, out Guid id, out string path)
		{
			path = null;
			byte[] array = new byte[256];
			int num = 0;
			RESULT result = Bank.FMOD_Studio_Bank_GetStringInfo(this.rawPtr, index, out id, array, array.Length, out num);
			if (result == RESULT.ERR_TRUNCATED)
			{
				array = new byte[num];
				result = Bank.FMOD_Studio_Bank_GetStringInfo(this.rawPtr, index, out id, array, array.Length, out num);
			}
			if (result == RESULT.OK)
			{
				path = Encoding.UTF8.GetString(array, 0, num - 1);
			}
			return RESULT.OK;
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x000062CE File Offset: 0x000044CE
		public RESULT getEventCount(out int count)
		{
			return Bank.FMOD_Studio_Bank_GetEventCount(this.rawPtr, out count);
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x000062DC File Offset: 0x000044DC
		public RESULT getEventList(out EventDescription[] array)
		{
			array = null;
			int num;
			RESULT result = Bank.FMOD_Studio_Bank_GetEventCount(this.rawPtr, out num);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num == 0)
			{
				array = new EventDescription[0];
				return result;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			result = Bank.FMOD_Studio_Bank_GetEventList(this.rawPtr, array2, num, out num2);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new EventDescription[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = new EventDescription(array2[i]);
			}
			return RESULT.OK;
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x00006355 File Offset: 0x00004555
		public RESULT getBusCount(out int count)
		{
			return Bank.FMOD_Studio_Bank_GetBusCount(this.rawPtr, out count);
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x00006364 File Offset: 0x00004564
		public RESULT getBusList(out Bus[] array)
		{
			array = null;
			int num;
			RESULT result = Bank.FMOD_Studio_Bank_GetBusCount(this.rawPtr, out num);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num == 0)
			{
				array = new Bus[0];
				return result;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			result = Bank.FMOD_Studio_Bank_GetBusList(this.rawPtr, array2, num, out num2);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new Bus[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = new Bus(array2[i]);
			}
			return RESULT.OK;
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x000063DD File Offset: 0x000045DD
		public RESULT getVCACount(out int count)
		{
			return Bank.FMOD_Studio_Bank_GetVCACount(this.rawPtr, out count);
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x000063EC File Offset: 0x000045EC
		public RESULT getVCAList(out VCA[] array)
		{
			array = null;
			int num;
			RESULT result = Bank.FMOD_Studio_Bank_GetVCACount(this.rawPtr, out num);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num == 0)
			{
				array = new VCA[0];
				return result;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			result = Bank.FMOD_Studio_Bank_GetVCAList(this.rawPtr, array2, num, out num2);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new VCA[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = new VCA(array2[i]);
			}
			return RESULT.OK;
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x00006465 File Offset: 0x00004665
		public RESULT getUserData(out IntPtr userdata)
		{
			return Bank.FMOD_Studio_Bank_GetUserData(this.rawPtr, out userdata);
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x00006473 File Offset: 0x00004673
		public RESULT setUserData(IntPtr userdata)
		{
			return Bank.FMOD_Studio_Bank_SetUserData(this.rawPtr, userdata);
		}

		// Token: 0x060004DB RID: 1243
		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_Bank_IsValid(IntPtr bank);

		// Token: 0x060004DC RID: 1244
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetID(IntPtr bank, out Guid id);

		// Token: 0x060004DD RID: 1245
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetPath(IntPtr bank, [Out] byte[] path, int size, out int retrieved);

		// Token: 0x060004DE RID: 1246
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_Unload(IntPtr bank);

		// Token: 0x060004DF RID: 1247
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_LoadSampleData(IntPtr bank);

		// Token: 0x060004E0 RID: 1248
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_UnloadSampleData(IntPtr bank);

		// Token: 0x060004E1 RID: 1249
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetLoadingState(IntPtr bank, out LOADING_STATE state);

		// Token: 0x060004E2 RID: 1250
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetSampleLoadingState(IntPtr bank, out LOADING_STATE state);

		// Token: 0x060004E3 RID: 1251
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetStringCount(IntPtr bank, out int count);

		// Token: 0x060004E4 RID: 1252
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetStringInfo(IntPtr bank, int index, out Guid id, [Out] byte[] path, int size, out int retrieved);

		// Token: 0x060004E5 RID: 1253
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetEventCount(IntPtr bank, out int count);

		// Token: 0x060004E6 RID: 1254
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetEventList(IntPtr bank, IntPtr[] array, int capacity, out int count);

		// Token: 0x060004E7 RID: 1255
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetBusCount(IntPtr bank, out int count);

		// Token: 0x060004E8 RID: 1256
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetBusList(IntPtr bank, IntPtr[] array, int capacity, out int count);

		// Token: 0x060004E9 RID: 1257
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetVCACount(IntPtr bank, out int count);

		// Token: 0x060004EA RID: 1258
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetVCAList(IntPtr bank, IntPtr[] array, int capacity, out int count);

		// Token: 0x060004EB RID: 1259
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetUserData(IntPtr studiosystem, out IntPtr userdata);

		// Token: 0x060004EC RID: 1260
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_SetUserData(IntPtr studiosystem, IntPtr userdata);

		// Token: 0x060004ED RID: 1261 RVA: 0x00005941 File Offset: 0x00003B41
		public Bank(IntPtr raw) : base(raw)
		{
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x00006481 File Offset: 0x00004681
		protected override bool isValidInternal()
		{
			return Bank.FMOD_Studio_Bank_IsValid(this.rawPtr);
		}
	}
}
