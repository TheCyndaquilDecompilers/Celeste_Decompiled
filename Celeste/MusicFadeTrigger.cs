using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200034D RID: 845
	public class MusicFadeTrigger : Trigger
	{
		// Token: 0x06001A87 RID: 6791 RVA: 0x000AB534 File Offset: 0x000A9734
		public MusicFadeTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.LeftToRight = (data.Attr("direction", "leftToRight") == "leftToRight");
			this.FadeA = data.Float("fadeA", 0f);
			this.FadeB = data.Float("fadeB", 1f);
			this.Parameter = data.Attr("parameter", "");
		}

		// Token: 0x06001A88 RID: 6792 RVA: 0x000AB5AC File Offset: 0x000A97AC
		public override void OnStay(Player player)
		{
			float value;
			if (this.LeftToRight)
			{
				value = Calc.ClampedMap(player.Center.X, base.Left, base.Right, this.FadeA, this.FadeB);
			}
			else
			{
				value = Calc.ClampedMap(player.Center.Y, base.Top, base.Bottom, this.FadeA, this.FadeB);
			}
			if (string.IsNullOrEmpty(this.Parameter))
			{
				Audio.SetMusicParam("fade", value);
				return;
			}
			Audio.SetMusicParam(this.Parameter, value);
		}

		// Token: 0x04001728 RID: 5928
		public bool LeftToRight;

		// Token: 0x04001729 RID: 5929
		public float FadeA;

		// Token: 0x0400172A RID: 5930
		public float FadeB;

		// Token: 0x0400172B RID: 5931
		public string Parameter;
	}
}
