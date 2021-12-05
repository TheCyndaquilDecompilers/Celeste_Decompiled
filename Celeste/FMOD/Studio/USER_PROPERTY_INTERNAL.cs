using System;

namespace FMOD.Studio
{
	// Token: 0x020000CB RID: 203
	internal struct USER_PROPERTY_INTERNAL
	{
		// Token: 0x060003A6 RID: 934 RVA: 0x0000510C File Offset: 0x0000330C
		public USER_PROPERTY createPublic()
		{
			USER_PROPERTY result = default(USER_PROPERTY);
			result.name = MarshallingHelper.stringFromNativeUtf8(this.name);
			result.type = this.type;
			switch (this.type)
			{
			case USER_PROPERTY_TYPE.INTEGER:
				result.intvalue = this.value.intvalue;
				break;
			case USER_PROPERTY_TYPE.BOOLEAN:
				result.boolvalue = this.value.boolvalue;
				break;
			case USER_PROPERTY_TYPE.FLOAT:
				result.floatvalue = this.value.floatvalue;
				break;
			case USER_PROPERTY_TYPE.STRING:
				result.stringvalue = MarshallingHelper.stringFromNativeUtf8(this.value.stringvalue);
				break;
			}
			return result;
		}

		// Token: 0x04000434 RID: 1076
		private IntPtr name;

		// Token: 0x04000435 RID: 1077
		private USER_PROPERTY_TYPE type;

		// Token: 0x04000436 RID: 1078
		private Union_IntBoolFloatString value;
	}
}
