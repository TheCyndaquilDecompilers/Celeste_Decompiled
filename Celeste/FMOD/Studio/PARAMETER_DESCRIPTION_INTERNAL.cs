using System;

namespace FMOD.Studio
{
	// Token: 0x020000C4 RID: 196
	internal struct PARAMETER_DESCRIPTION_INTERNAL
	{
		// Token: 0x060003A1 RID: 929 RVA: 0x00004EE8 File Offset: 0x000030E8
		public void assign(out PARAMETER_DESCRIPTION publicDesc)
		{
			publicDesc.name = MarshallingHelper.stringFromNativeUtf8(this.name);
			publicDesc.index = this.index;
			publicDesc.minimum = this.minimum;
			publicDesc.maximum = this.maximum;
			publicDesc.defaultvalue = this.defaultvalue;
			publicDesc.type = this.type;
		}

		// Token: 0x04000416 RID: 1046
		public IntPtr name;

		// Token: 0x04000417 RID: 1047
		public int index;

		// Token: 0x04000418 RID: 1048
		public float minimum;

		// Token: 0x04000419 RID: 1049
		public float maximum;

		// Token: 0x0400041A RID: 1050
		public float defaultvalue;

		// Token: 0x0400041B RID: 1051
		public PARAMETER_TYPE type;
	}
}
