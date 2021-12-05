using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001E1 RID: 481
	public class AmbienceParamTrigger : Trigger
	{
		// Token: 0x06001025 RID: 4133 RVA: 0x00045BF8 File Offset: 0x00043DF8
		public AmbienceParamTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.Parameter = data.Attr("parameter", "");
			this.From = data.Float("from", 0f);
			this.To = data.Float("to", 0f);
			this.PositionMode = data.Enum<Trigger.PositionModes>("direction", Trigger.PositionModes.NoEffect);
		}

		// Token: 0x06001026 RID: 4134 RVA: 0x00045C64 File Offset: 0x00043E64
		public override void OnStay(Player player)
		{
			float value = Calc.ClampedMap(base.GetPositionLerp(player, this.PositionMode), 0f, 1f, this.From, this.To);
			Level level = base.Scene as Level;
			level.Session.Audio.Ambience.Param(this.Parameter, value);
			level.Session.Audio.Apply(false);
		}

		// Token: 0x04000B84 RID: 2948
		public string Parameter;

		// Token: 0x04000B85 RID: 2949
		public float From;

		// Token: 0x04000B86 RID: 2950
		public float To;

		// Token: 0x04000B87 RID: 2951
		public Trigger.PositionModes PositionMode;
	}
}
