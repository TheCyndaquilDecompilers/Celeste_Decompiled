using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	// Token: 0x020000DF RID: 223
	public class EventDescription : HandleBase
	{
		// Token: 0x06000419 RID: 1049 RVA: 0x00005957 File Offset: 0x00003B57
		public RESULT getID(out Guid id)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetID(this.rawPtr, out id);
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x00005968 File Offset: 0x00003B68
		public RESULT getPath(out string path)
		{
			path = null;
			byte[] array = new byte[256];
			int num = 0;
			RESULT result = EventDescription.FMOD_Studio_EventDescription_GetPath(this.rawPtr, array, array.Length, out num);
			if (result == RESULT.ERR_TRUNCATED)
			{
				array = new byte[num];
				result = EventDescription.FMOD_Studio_EventDescription_GetPath(this.rawPtr, array, array.Length, out num);
			}
			if (result == RESULT.OK)
			{
				path = Encoding.UTF8.GetString(array, 0, num - 1);
			}
			return result;
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x000059CA File Offset: 0x00003BCA
		public RESULT getParameterCount(out int count)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetParameterCount(this.rawPtr, out count);
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x000059D8 File Offset: 0x00003BD8
		public RESULT getParameterByIndex(int index, out PARAMETER_DESCRIPTION parameter)
		{
			parameter = default(PARAMETER_DESCRIPTION);
			PARAMETER_DESCRIPTION_INTERNAL parameter_DESCRIPTION_INTERNAL;
			RESULT result = EventDescription.FMOD_Studio_EventDescription_GetParameterByIndex(this.rawPtr, index, out parameter_DESCRIPTION_INTERNAL);
			if (result != RESULT.OK)
			{
				return result;
			}
			parameter_DESCRIPTION_INTERNAL.assign(out parameter);
			return result;
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x00005A0C File Offset: 0x00003C0C
		public RESULT getParameter(string name, out PARAMETER_DESCRIPTION parameter)
		{
			parameter = default(PARAMETER_DESCRIPTION);
			PARAMETER_DESCRIPTION_INTERNAL parameter_DESCRIPTION_INTERNAL;
			RESULT result = EventDescription.FMOD_Studio_EventDescription_GetParameter(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), out parameter_DESCRIPTION_INTERNAL);
			if (result != RESULT.OK)
			{
				return result;
			}
			parameter_DESCRIPTION_INTERNAL.assign(out parameter);
			return result;
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x00005A51 File Offset: 0x00003C51
		public RESULT getUserPropertyCount(out int count)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetUserPropertyCount(this.rawPtr, out count);
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x00005A60 File Offset: 0x00003C60
		public RESULT getUserPropertyByIndex(int index, out USER_PROPERTY property)
		{
			USER_PROPERTY_INTERNAL user_PROPERTY_INTERNAL;
			RESULT result = EventDescription.FMOD_Studio_EventDescription_GetUserPropertyByIndex(this.rawPtr, index, out user_PROPERTY_INTERNAL);
			if (result != RESULT.OK)
			{
				property = default(USER_PROPERTY);
				return result;
			}
			property = user_PROPERTY_INTERNAL.createPublic();
			return RESULT.OK;
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x00005A98 File Offset: 0x00003C98
		public RESULT getUserProperty(string name, out USER_PROPERTY property)
		{
			USER_PROPERTY_INTERNAL user_PROPERTY_INTERNAL;
			RESULT result = EventDescription.FMOD_Studio_EventDescription_GetUserProperty(this.rawPtr, Encoding.UTF8.GetBytes(name + "\0"), out user_PROPERTY_INTERNAL);
			if (result != RESULT.OK)
			{
				property = default(USER_PROPERTY);
				return result;
			}
			property = user_PROPERTY_INTERNAL.createPublic();
			return RESULT.OK;
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00005AE2 File Offset: 0x00003CE2
		public RESULT getLength(out int length)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetLength(this.rawPtr, out length);
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x00005AF0 File Offset: 0x00003CF0
		public RESULT getMinimumDistance(out float distance)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetMinimumDistance(this.rawPtr, out distance);
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x00005AFE File Offset: 0x00003CFE
		public RESULT getMaximumDistance(out float distance)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetMaximumDistance(this.rawPtr, out distance);
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x00005B0C File Offset: 0x00003D0C
		public RESULT getSoundSize(out float size)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetSoundSize(this.rawPtr, out size);
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x00005B1A File Offset: 0x00003D1A
		public RESULT isSnapshot(out bool snapshot)
		{
			return EventDescription.FMOD_Studio_EventDescription_IsSnapshot(this.rawPtr, out snapshot);
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x00005B28 File Offset: 0x00003D28
		public RESULT isOneshot(out bool oneshot)
		{
			return EventDescription.FMOD_Studio_EventDescription_IsOneshot(this.rawPtr, out oneshot);
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x00005B36 File Offset: 0x00003D36
		public RESULT isStream(out bool isStream)
		{
			return EventDescription.FMOD_Studio_EventDescription_IsStream(this.rawPtr, out isStream);
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00005B44 File Offset: 0x00003D44
		public RESULT is3D(out bool is3D)
		{
			return EventDescription.FMOD_Studio_EventDescription_Is3D(this.rawPtr, out is3D);
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x00005B52 File Offset: 0x00003D52
		public RESULT hasCue(out bool cue)
		{
			return EventDescription.FMOD_Studio_EventDescription_HasCue(this.rawPtr, out cue);
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x00005B60 File Offset: 0x00003D60
		public RESULT createInstance(out EventInstance instance)
		{
			instance = null;
			IntPtr raw = 0;
			RESULT result = EventDescription.FMOD_Studio_EventDescription_CreateInstance(this.rawPtr, out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			instance = new EventInstance(raw);
			return result;
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x00005B94 File Offset: 0x00003D94
		public RESULT getInstanceCount(out int count)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetInstanceCount(this.rawPtr, out count);
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00005BA4 File Offset: 0x00003DA4
		public RESULT getInstanceList(out EventInstance[] array)
		{
			array = null;
			int num;
			RESULT result = EventDescription.FMOD_Studio_EventDescription_GetInstanceCount(this.rawPtr, out num);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num == 0)
			{
				array = new EventInstance[0];
				return result;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			result = EventDescription.FMOD_Studio_EventDescription_GetInstanceList(this.rawPtr, array2, num, out num2);
			if (result != RESULT.OK)
			{
				return result;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new EventInstance[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = new EventInstance(array2[i]);
			}
			return RESULT.OK;
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x00005C1D File Offset: 0x00003E1D
		public RESULT loadSampleData()
		{
			return EventDescription.FMOD_Studio_EventDescription_LoadSampleData(this.rawPtr);
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x00005C2A File Offset: 0x00003E2A
		public RESULT unloadSampleData()
		{
			return EventDescription.FMOD_Studio_EventDescription_UnloadSampleData(this.rawPtr);
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x00005C37 File Offset: 0x00003E37
		public RESULT getSampleLoadingState(out LOADING_STATE state)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetSampleLoadingState(this.rawPtr, out state);
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x00005C45 File Offset: 0x00003E45
		public RESULT releaseAllInstances()
		{
			return EventDescription.FMOD_Studio_EventDescription_ReleaseAllInstances(this.rawPtr);
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x00005C52 File Offset: 0x00003E52
		public RESULT setCallback(EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask = EVENT_CALLBACK_TYPE.ALL)
		{
			return EventDescription.FMOD_Studio_EventDescription_SetCallback(this.rawPtr, callback, callbackmask);
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x00005C61 File Offset: 0x00003E61
		public RESULT getUserData(out IntPtr userdata)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetUserData(this.rawPtr, out userdata);
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x00005C6F File Offset: 0x00003E6F
		public RESULT setUserData(IntPtr userdata)
		{
			return EventDescription.FMOD_Studio_EventDescription_SetUserData(this.rawPtr, userdata);
		}

		// Token: 0x06000434 RID: 1076
		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_EventDescription_IsValid(IntPtr eventdescription);

		// Token: 0x06000435 RID: 1077
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetID(IntPtr eventdescription, out Guid id);

		// Token: 0x06000436 RID: 1078
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetPath(IntPtr eventdescription, [Out] byte[] path, int size, out int retrieved);

		// Token: 0x06000437 RID: 1079
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameterCount(IntPtr eventdescription, out int count);

		// Token: 0x06000438 RID: 1080
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameterByIndex(IntPtr eventdescription, int index, out PARAMETER_DESCRIPTION_INTERNAL parameter);

		// Token: 0x06000439 RID: 1081
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameter(IntPtr eventdescription, byte[] name, out PARAMETER_DESCRIPTION_INTERNAL parameter);

		// Token: 0x0600043A RID: 1082
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyCount(IntPtr eventdescription, out int count);

		// Token: 0x0600043B RID: 1083
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyByIndex(IntPtr eventdescription, int index, out USER_PROPERTY_INTERNAL property);

		// Token: 0x0600043C RID: 1084
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserProperty(IntPtr eventdescription, byte[] name, out USER_PROPERTY_INTERNAL property);

		// Token: 0x0600043D RID: 1085
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetLength(IntPtr eventdescription, out int length);

		// Token: 0x0600043E RID: 1086
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetMinimumDistance(IntPtr eventdescription, out float distance);

		// Token: 0x0600043F RID: 1087
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetMaximumDistance(IntPtr eventdescription, out float distance);

		// Token: 0x06000440 RID: 1088
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetSoundSize(IntPtr eventdescription, out float size);

		// Token: 0x06000441 RID: 1089
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_IsSnapshot(IntPtr eventdescription, out bool snapshot);

		// Token: 0x06000442 RID: 1090
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_IsOneshot(IntPtr eventdescription, out bool oneshot);

		// Token: 0x06000443 RID: 1091
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_IsStream(IntPtr eventdescription, out bool isStream);

		// Token: 0x06000444 RID: 1092
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_Is3D(IntPtr eventdescription, out bool is3D);

		// Token: 0x06000445 RID: 1093
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_HasCue(IntPtr eventdescription, out bool cue);

		// Token: 0x06000446 RID: 1094
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_CreateInstance(IntPtr eventdescription, out IntPtr instance);

		// Token: 0x06000447 RID: 1095
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetInstanceCount(IntPtr eventdescription, out int count);

		// Token: 0x06000448 RID: 1096
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetInstanceList(IntPtr eventdescription, IntPtr[] array, int capacity, out int count);

		// Token: 0x06000449 RID: 1097
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_LoadSampleData(IntPtr eventdescription);

		// Token: 0x0600044A RID: 1098
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_UnloadSampleData(IntPtr eventdescription);

		// Token: 0x0600044B RID: 1099
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetSampleLoadingState(IntPtr eventdescription, out LOADING_STATE state);

		// Token: 0x0600044C RID: 1100
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_ReleaseAllInstances(IntPtr eventdescription);

		// Token: 0x0600044D RID: 1101
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_SetCallback(IntPtr eventdescription, EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask);

		// Token: 0x0600044E RID: 1102
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserData(IntPtr eventdescription, out IntPtr userdata);

		// Token: 0x0600044F RID: 1103
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_SetUserData(IntPtr eventdescription, IntPtr userdata);

		// Token: 0x06000450 RID: 1104 RVA: 0x00005941 File Offset: 0x00003B41
		public EventDescription(IntPtr raw) : base(raw)
		{
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x00005C7D File Offset: 0x00003E7D
		protected override bool isValidInternal()
		{
			return EventDescription.FMOD_Studio_EventDescription_IsValid(this.rawPtr);
		}
	}
}
