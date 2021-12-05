using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200022D RID: 557
	public static class DustStyles
	{
		// Token: 0x060011C6 RID: 4550 RVA: 0x00058AE8 File Offset: 0x00056CE8
		public static DustStyles.DustStyle Get(Session session)
		{
			if (!DustStyles.Styles.ContainsKey(session.Area.ID))
			{
				return DustStyles.Styles[3];
			}
			return DustStyles.Styles[session.Area.ID];
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x00058B22 File Offset: 0x00056D22
		public static DustStyles.DustStyle Get(Scene scene)
		{
			return DustStyles.Get((scene as Level).Session);
		}

		// Token: 0x04000D6A RID: 3434
		public static Dictionary<int, DustStyles.DustStyle> Styles = new Dictionary<int, DustStyles.DustStyle>
		{
			{
				3,
				new DustStyles.DustStyle
				{
					EdgeColors = new Vector3[]
					{
						Calc.HexToColor("f25a10").ToVector3(),
						Calc.HexToColor("ff0000").ToVector3(),
						Calc.HexToColor("f21067").ToVector3()
					},
					EyeColor = Color.Red,
					EyeTextures = "danger/dustcreature/eyes"
				}
			},
			{
				5,
				new DustStyles.DustStyle
				{
					EdgeColors = new Vector3[]
					{
						Calc.HexToColor("245ebb").ToVector3(),
						Calc.HexToColor("17a0ff").ToVector3(),
						Calc.HexToColor("17a0ff").ToVector3()
					},
					EyeColor = Calc.HexToColor("245ebb"),
					EyeTextures = "danger/dustcreature/templeeyes"
				}
			}
		};

		// Token: 0x0200054C RID: 1356
		public struct DustStyle
		{
			// Token: 0x040025E4 RID: 9700
			public Vector3[] EdgeColors;

			// Token: 0x040025E5 RID: 9701
			public Color EyeColor;

			// Token: 0x040025E6 RID: 9702
			public string EyeTextures;
		}
	}
}
