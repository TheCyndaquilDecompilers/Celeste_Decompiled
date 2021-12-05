using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000287 RID: 647
	public class CS03_OshiroHallway1 : CutsceneEntity
	{
		// Token: 0x06001403 RID: 5123 RVA: 0x0006C235 File Offset: 0x0006A435
		public CS03_OshiroHallway1(Player player, NPC oshiro) : base(true, false)
		{
			this.player = player;
			this.oshiro = oshiro;
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x0006C24D File Offset: 0x0006A44D
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x0006C262 File Offset: 0x0006A462
		private IEnumerator Cutscene(Level level)
		{
			level.Session.Audio.Music.Layer(1, false);
			level.Session.Audio.Music.Layer(2, true);
			level.Session.Audio.Apply(false);
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			yield return Textbox.Say("CH3_OSHIRO_HALLWAY_A", new Func<IEnumerator>[0]);
			this.oshiro.MoveToAndRemove(new Vector2((float)(base.SceneAs<Level>().Bounds.Right + 64), this.oshiro.Y));
			this.oshiro.Add(new SoundSource("event:/char/oshiro/move_02_03a_exit"));
			yield return 1f;
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x0006C278 File Offset: 0x0006A478
		public override void OnEnd(Level level)
		{
			level.Session.Audio.Music.Layer(1, true);
			level.Session.Audio.Music.Layer(2, false);
			level.Session.Audio.Apply(false);
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			level.Session.SetFlag("oshiro_resort_talked_2", true);
			if (this.WasSkipped)
			{
				level.Remove(this.oshiro);
			}
		}

		// Token: 0x04000FB8 RID: 4024
		public const string Flag = "oshiro_resort_talked_2";

		// Token: 0x04000FB9 RID: 4025
		private Player player;

		// Token: 0x04000FBA RID: 4026
		private NPC oshiro;
	}
}
