using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	// Token: 0x020000E5 RID: 229
	public class CommandReplay : HandleBase
	{
		// Token: 0x060004EF RID: 1263 RVA: 0x00006490 File Offset: 0x00004690
		public RESULT getSystem(out System system)
		{
			system = null;
			IntPtr raw = 0;
			RESULT result = CommandReplay.FMOD_Studio_CommandReplay_GetSystem(this.rawPtr, out raw);
			if (result == RESULT.OK)
			{
				system = new System(raw);
			}
			return result;
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x000064C0 File Offset: 0x000046C0
		public RESULT getLength(out float totalTime)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetLength(this.rawPtr, out totalTime);
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x000064CE File Offset: 0x000046CE
		public RESULT getCommandCount(out int count)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetCommandCount(this.rawPtr, out count);
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x000064DC File Offset: 0x000046DC
		public RESULT getCommandInfo(int commandIndex, out COMMAND_INFO info)
		{
			COMMAND_INFO_INTERNAL command_INFO_INTERNAL = default(COMMAND_INFO_INTERNAL);
			RESULT result = CommandReplay.FMOD_Studio_CommandReplay_GetCommandInfo(this.rawPtr, commandIndex, out command_INFO_INTERNAL);
			if (result != RESULT.OK)
			{
				info = default(COMMAND_INFO);
				return result;
			}
			info = command_INFO_INTERNAL.createPublic();
			return result;
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x0000651C File Offset: 0x0000471C
		public RESULT getCommandString(int commandIndex, out string description)
		{
			description = null;
			byte[] array = new byte[8];
			RESULT result;
			for (;;)
			{
				result = CommandReplay.FMOD_Studio_CommandReplay_GetCommandString(this.rawPtr, commandIndex, array, array.Length);
				if (result != RESULT.ERR_TRUNCATED)
				{
					break;
				}
				array = new byte[2 * array.Length];
			}
			if (result == RESULT.OK)
			{
				int num = 0;
				while (array[num] != 0)
				{
					num++;
				}
				description = Encoding.UTF8.GetString(array, 0, num);
			}
			return result;
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x00006576 File Offset: 0x00004776
		public RESULT getCommandAtTime(float time, out int commandIndex)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetCommandAtTime(this.rawPtr, time, out commandIndex);
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x00006585 File Offset: 0x00004785
		public RESULT setBankPath(string bankPath)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SetBankPath(this.rawPtr, Encoding.UTF8.GetBytes(bankPath + "\0"));
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x000065A7 File Offset: 0x000047A7
		public RESULT start()
		{
			return CommandReplay.FMOD_Studio_CommandReplay_Start(this.rawPtr);
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x000065B4 File Offset: 0x000047B4
		public RESULT stop()
		{
			return CommandReplay.FMOD_Studio_CommandReplay_Stop(this.rawPtr);
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x000065C1 File Offset: 0x000047C1
		public RESULT seekToTime(float time)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SeekToTime(this.rawPtr, time);
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x000065CF File Offset: 0x000047CF
		public RESULT seekToCommand(int commandIndex)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SeekToCommand(this.rawPtr, commandIndex);
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x000065DD File Offset: 0x000047DD
		public RESULT getPaused(out bool paused)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetPaused(this.rawPtr, out paused);
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x000065EB File Offset: 0x000047EB
		public RESULT setPaused(bool paused)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SetPaused(this.rawPtr, paused);
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x000065F9 File Offset: 0x000047F9
		public RESULT getPlaybackState(out PLAYBACK_STATE state)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetPlaybackState(this.rawPtr, out state);
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x00006607 File Offset: 0x00004807
		public RESULT getCurrentCommand(out int commandIndex, out float currentTime)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetCurrentCommand(this.rawPtr, out commandIndex, out currentTime);
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x00006616 File Offset: 0x00004816
		public RESULT release()
		{
			return CommandReplay.FMOD_Studio_CommandReplay_Release(this.rawPtr);
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x00006623 File Offset: 0x00004823
		public RESULT setFrameCallback(COMMANDREPLAY_FRAME_CALLBACK callback)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SetFrameCallback(this.rawPtr, callback);
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x00006631 File Offset: 0x00004831
		public RESULT setLoadBankCallback(COMMANDREPLAY_LOAD_BANK_CALLBACK callback)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SetLoadBankCallback(this.rawPtr, callback);
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x0000663F File Offset: 0x0000483F
		public RESULT setCreateInstanceCallback(COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SetCreateInstanceCallback(this.rawPtr, callback);
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x0000664D File Offset: 0x0000484D
		public RESULT getUserData(out IntPtr userdata)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetUserData(this.rawPtr, out userdata);
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x0000665B File Offset: 0x0000485B
		public RESULT setUserData(IntPtr userdata)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SetUserData(this.rawPtr, userdata);
		}

		// Token: 0x06000504 RID: 1284
		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_CommandReplay_IsValid(IntPtr replay);

		// Token: 0x06000505 RID: 1285
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetSystem(IntPtr replay, out IntPtr system);

		// Token: 0x06000506 RID: 1286
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetLength(IntPtr replay, out float totalTime);

		// Token: 0x06000507 RID: 1287
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCommandCount(IntPtr replay, out int count);

		// Token: 0x06000508 RID: 1288
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCommandInfo(IntPtr replay, int commandIndex, out COMMAND_INFO_INTERNAL info);

		// Token: 0x06000509 RID: 1289
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCommandString(IntPtr replay, int commandIndex, [Out] byte[] description, int capacity);

		// Token: 0x0600050A RID: 1290
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCommandAtTime(IntPtr replay, float time, out int commandIndex);

		// Token: 0x0600050B RID: 1291
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetBankPath(IntPtr replay, byte[] bankPath);

		// Token: 0x0600050C RID: 1292
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_Start(IntPtr replay);

		// Token: 0x0600050D RID: 1293
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_Stop(IntPtr replay);

		// Token: 0x0600050E RID: 1294
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SeekToTime(IntPtr replay, float time);

		// Token: 0x0600050F RID: 1295
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SeekToCommand(IntPtr replay, int commandIndex);

		// Token: 0x06000510 RID: 1296
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetPaused(IntPtr replay, out bool paused);

		// Token: 0x06000511 RID: 1297
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetPaused(IntPtr replay, bool paused);

		// Token: 0x06000512 RID: 1298
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetPlaybackState(IntPtr replay, out PLAYBACK_STATE state);

		// Token: 0x06000513 RID: 1299
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCurrentCommand(IntPtr replay, out int commandIndex, out float currentTime);

		// Token: 0x06000514 RID: 1300
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_Release(IntPtr replay);

		// Token: 0x06000515 RID: 1301
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetFrameCallback(IntPtr replay, COMMANDREPLAY_FRAME_CALLBACK callback);

		// Token: 0x06000516 RID: 1302
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetLoadBankCallback(IntPtr replay, COMMANDREPLAY_LOAD_BANK_CALLBACK callback);

		// Token: 0x06000517 RID: 1303
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetCreateInstanceCallback(IntPtr replay, COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback);

		// Token: 0x06000518 RID: 1304
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetUserData(IntPtr replay, out IntPtr userdata);

		// Token: 0x06000519 RID: 1305
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetUserData(IntPtr replay, IntPtr userdata);

		// Token: 0x0600051A RID: 1306 RVA: 0x00005941 File Offset: 0x00003B41
		public CommandReplay(IntPtr raw) : base(raw)
		{
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x00006669 File Offset: 0x00004869
		protected override bool isValidInternal()
		{
			return CommandReplay.FMOD_Studio_CommandReplay_IsValid(this.rawPtr);
		}
	}
}
