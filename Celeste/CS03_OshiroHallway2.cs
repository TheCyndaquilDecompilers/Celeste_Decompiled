using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000286 RID: 646
	public class CS03_OshiroHallway2 : CutsceneEntity
	{
		// Token: 0x060013FF RID: 5119 RVA: 0x0006C15D File Offset: 0x0006A35D
		public CS03_OshiroHallway2(Player player, NPC oshiro) : base(true, false)
		{
			this.player = player;
			this.oshiro = oshiro;
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x0006C175 File Offset: 0x0006A375
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x0006C18A File Offset: 0x0006A38A
		private IEnumerator Cutscene(Level level)
		{
			level.Session.Audio.Music.Layer(1, false);
			level.Session.Audio.Music.Layer(2, true);
			level.Session.Audio.Apply(false);
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			yield return Textbox.Say("CH3_OSHIRO_HALLWAY_B", new Func<IEnumerator>[0]);
			this.oshiro.MoveToAndRemove(new Vector2((float)(level.Bounds.Right + 64), this.oshiro.Y));
			this.oshiro.Add(new SoundSource("event:/char/oshiro/move_03_08a_exit"));
			yield return 1f;
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06001402 RID: 5122 RVA: 0x0006C1A0 File Offset: 0x0006A3A0
		public override void OnEnd(Level level)
		{
			level.Session.Audio.Music.Layer(1, true);
			level.Session.Audio.Music.Layer(2, false);
			level.Session.Audio.Apply(false);
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			level.Session.SetFlag("oshiro_resort_talked_3", true);
			if (this.WasSkipped)
			{
				level.Remove(this.oshiro);
			}
		}

		// Token: 0x04000FB5 RID: 4021
		public const string Flag = "oshiro_resort_talked_3";

		// Token: 0x04000FB6 RID: 4022
		private Player player;

		// Token: 0x04000FB7 RID: 4023
		private NPC oshiro;
	}
}
