using System;

namespace FMOD
{
	// Token: 0x0200003A RID: 58
	public class PRESET
	{
		// Token: 0x06000057 RID: 87 RVA: 0x0000281C File Offset: 0x00000A1C
		public static REVERB_PROPERTIES OFF()
		{
			return new REVERB_PROPERTIES(1000f, 7f, 11f, 5000f, 100f, 100f, 100f, 250f, 0f, 20f, 96f, -80f);
		}

		// Token: 0x06000058 RID: 88 RVA: 0x0000286C File Offset: 0x00000A6C
		public static REVERB_PROPERTIES GENERIC()
		{
			return new REVERB_PROPERTIES(1500f, 7f, 11f, 5000f, 83f, 100f, 100f, 250f, 0f, 14500f, 96f, -8f);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000028BC File Offset: 0x00000ABC
		public static REVERB_PROPERTIES PADDEDCELL()
		{
			return new REVERB_PROPERTIES(170f, 1f, 2f, 5000f, 10f, 100f, 100f, 250f, 0f, 160f, 84f, -7.8f);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x0000290C File Offset: 0x00000B0C
		public static REVERB_PROPERTIES ROOM()
		{
			return new REVERB_PROPERTIES(400f, 2f, 3f, 5000f, 83f, 100f, 100f, 250f, 0f, 6050f, 88f, -9.4f);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000295C File Offset: 0x00000B5C
		public static REVERB_PROPERTIES BATHROOM()
		{
			return new REVERB_PROPERTIES(1500f, 7f, 11f, 5000f, 54f, 100f, 60f, 250f, 0f, 2900f, 83f, 0.5f);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000029AC File Offset: 0x00000BAC
		public static REVERB_PROPERTIES LIVINGROOM()
		{
			return new REVERB_PROPERTIES(500f, 3f, 4f, 5000f, 10f, 100f, 100f, 250f, 0f, 160f, 58f, -19f);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000029FC File Offset: 0x00000BFC
		public static REVERB_PROPERTIES STONEROOM()
		{
			return new REVERB_PROPERTIES(2300f, 12f, 17f, 5000f, 64f, 100f, 100f, 250f, 0f, 7800f, 71f, -8.5f);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00002A4C File Offset: 0x00000C4C
		public static REVERB_PROPERTIES AUDITORIUM()
		{
			return new REVERB_PROPERTIES(4300f, 20f, 30f, 5000f, 59f, 100f, 100f, 250f, 0f, 5850f, 64f, -11.7f);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00002A9C File Offset: 0x00000C9C
		public static REVERB_PROPERTIES CONCERTHALL()
		{
			return new REVERB_PROPERTIES(3900f, 20f, 29f, 5000f, 70f, 100f, 100f, 250f, 0f, 5650f, 80f, -9.8f);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00002AEC File Offset: 0x00000CEC
		public static REVERB_PROPERTIES CAVE()
		{
			return new REVERB_PROPERTIES(2900f, 15f, 22f, 5000f, 100f, 100f, 100f, 250f, 0f, 20000f, 59f, -11.3f);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002B3C File Offset: 0x00000D3C
		public static REVERB_PROPERTIES ARENA()
		{
			return new REVERB_PROPERTIES(7200f, 20f, 30f, 5000f, 33f, 100f, 100f, 250f, 0f, 4500f, 80f, -9.6f);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00002B8C File Offset: 0x00000D8C
		public static REVERB_PROPERTIES HANGAR()
		{
			return new REVERB_PROPERTIES(10000f, 20f, 30f, 5000f, 23f, 100f, 100f, 250f, 0f, 3400f, 72f, -7.4f);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00002BDC File Offset: 0x00000DDC
		public static REVERB_PROPERTIES CARPETTEDHALLWAY()
		{
			return new REVERB_PROPERTIES(300f, 2f, 30f, 5000f, 10f, 100f, 100f, 250f, 0f, 500f, 56f, -24f);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00002C2C File Offset: 0x00000E2C
		public static REVERB_PROPERTIES HALLWAY()
		{
			return new REVERB_PROPERTIES(1500f, 7f, 11f, 5000f, 59f, 100f, 100f, 250f, 0f, 7800f, 87f, -5.5f);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00002C7C File Offset: 0x00000E7C
		public static REVERB_PROPERTIES STONECORRIDOR()
		{
			return new REVERB_PROPERTIES(270f, 13f, 20f, 5000f, 79f, 100f, 100f, 250f, 0f, 9000f, 86f, -6f);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00002CCC File Offset: 0x00000ECC
		public static REVERB_PROPERTIES ALLEY()
		{
			return new REVERB_PROPERTIES(1500f, 7f, 11f, 5000f, 86f, 100f, 100f, 250f, 0f, 8300f, 80f, -9.8f);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00002D1C File Offset: 0x00000F1C
		public static REVERB_PROPERTIES FOREST()
		{
			return new REVERB_PROPERTIES(1500f, 162f, 88f, 5000f, 54f, 79f, 100f, 250f, 0f, 760f, 94f, -12.3f);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00002D6C File Offset: 0x00000F6C
		public static REVERB_PROPERTIES CITY()
		{
			return new REVERB_PROPERTIES(1500f, 7f, 11f, 5000f, 67f, 50f, 100f, 250f, 0f, 4050f, 66f, -26f);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00002DBC File Offset: 0x00000FBC
		public static REVERB_PROPERTIES MOUNTAINS()
		{
			return new REVERB_PROPERTIES(1500f, 300f, 100f, 5000f, 21f, 27f, 100f, 250f, 0f, 1220f, 82f, -24f);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00002E0C File Offset: 0x0000100C
		public static REVERB_PROPERTIES QUARRY()
		{
			return new REVERB_PROPERTIES(1500f, 61f, 25f, 5000f, 83f, 100f, 100f, 250f, 0f, 3400f, 100f, -5f);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00002E5C File Offset: 0x0000105C
		public static REVERB_PROPERTIES PLAIN()
		{
			return new REVERB_PROPERTIES(1500f, 179f, 100f, 5000f, 50f, 21f, 100f, 250f, 0f, 1670f, 65f, -28f);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00002EAC File Offset: 0x000010AC
		public static REVERB_PROPERTIES PARKINGLOT()
		{
			return new REVERB_PROPERTIES(1700f, 8f, 12f, 5000f, 100f, 100f, 100f, 250f, 0f, 20000f, 56f, -19.5f);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00002EFC File Offset: 0x000010FC
		public static REVERB_PROPERTIES SEWERPIPE()
		{
			return new REVERB_PROPERTIES(2800f, 14f, 21f, 5000f, 14f, 80f, 60f, 250f, 0f, 3400f, 66f, 1.2f);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00002F4C File Offset: 0x0000114C
		public static REVERB_PROPERTIES UNDERWATER()
		{
			return new REVERB_PROPERTIES(1500f, 7f, 11f, 5000f, 10f, 100f, 100f, 250f, 0f, 500f, 92f, 7f);
		}
	}
}
