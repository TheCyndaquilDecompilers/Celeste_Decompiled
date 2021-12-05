using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	// Token: 0x020000E1 RID: 225
	public class ParameterInstance : HandleBase
	{
		// Token: 0x0600049B RID: 1179 RVA: 0x00005F60 File Offset: 0x00004160
		public RESULT getDescription(out PARAMETER_DESCRIPTION description)
		{
			description = default(PARAMETER_DESCRIPTION);
			PARAMETER_DESCRIPTION_INTERNAL parameter_DESCRIPTION_INTERNAL;
			RESULT result = ParameterInstance.FMOD_Studio_ParameterInstance_GetDescription(this.rawPtr, out parameter_DESCRIPTION_INTERNAL);
			if (result != RESULT.OK)
			{
				return result;
			}
			parameter_DESCRIPTION_INTERNAL.assign(out description);
			return result;
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x00005F90 File Offset: 0x00004190
		public RESULT getValue(out float value)
		{
			return ParameterInstance.FMOD_Studio_ParameterInstance_GetValue(this.rawPtr, out value);
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x00005F9E File Offset: 0x0000419E
		public RESULT setValue(float value)
		{
			return ParameterInstance.FMOD_Studio_ParameterInstance_SetValue(this.rawPtr, value);
		}

		// Token: 0x0600049E RID: 1182
		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_ParameterInstance_IsValid(IntPtr parameter);

		// Token: 0x0600049F RID: 1183
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParameterInstance_GetDescription(IntPtr parameter, out PARAMETER_DESCRIPTION_INTERNAL description);

		// Token: 0x060004A0 RID: 1184
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParameterInstance_GetValue(IntPtr parameter, out float value);

		// Token: 0x060004A1 RID: 1185
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParameterInstance_SetValue(IntPtr parameter, float value);

		// Token: 0x060004A2 RID: 1186 RVA: 0x00005941 File Offset: 0x00003B41
		public ParameterInstance(IntPtr raw) : base(raw)
		{
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x00005FAC File Offset: 0x000041AC
		protected override bool isValidInternal()
		{
			return ParameterInstance.FMOD_Studio_ParameterInstance_IsValid(this.rawPtr);
		}
	}
}
