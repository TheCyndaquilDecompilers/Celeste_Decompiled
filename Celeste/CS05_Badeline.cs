using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000190 RID: 400
	public class CS05_Badeline : CutsceneEntity
	{
		// Token: 0x06000DE4 RID: 3556 RVA: 0x00031A63 File Offset: 0x0002FC63
		public static string GetFlag(int index)
		{
			return "badeline_" + index;
		}

		// Token: 0x06000DE5 RID: 3557 RVA: 0x00031A75 File Offset: 0x0002FC75
		public CS05_Badeline(Player player, NPC05_Badeline npc, BadelineDummy badeline, int index) : base(true, false)
		{
			this.player = player;
			this.npc = npc;
			this.badeline = badeline;
			this.index = index;
		}

		// Token: 0x06000DE6 RID: 3558 RVA: 0x00031A9C File Offset: 0x0002FC9C
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000DE7 RID: 3559 RVA: 0x00031AB1 File Offset: 0x0002FCB1
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			yield return 0.25f;
			if (this.index == 3)
			{
				this.player.DummyAutoAnimate = false;
				this.player.Sprite.Play("tired", false, false);
				yield return 0.2f;
			}
			while (this.player.Scene != null && !this.player.OnGround(1))
			{
				yield return null;
			}
			Vector2 screenSpaceFocusPoint = (this.badeline.Center + this.player.Center) * 0.5f - this.Level.Camera.Position + new Vector2(0f, -12f);
			yield return this.Level.ZoomTo(screenSpaceFocusPoint, 2f, 0.5f);
			yield return Textbox.Say("ch5_shadow_maddy_" + this.index, new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.BadelineLeaves)
			});
			if (!this.moved)
			{
				this.npc.MoveToNode(this.index, true);
			}
			yield return this.Level.ZoomBack(0.5f);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000DE8 RID: 3560 RVA: 0x00031AC8 File Offset: 0x0002FCC8
		public override void OnEnd(Level level)
		{
			this.npc.SnapToNode(this.index);
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			level.Session.SetFlag(CS05_Badeline.GetFlag(this.index), true);
		}

		// Token: 0x06000DE9 RID: 3561 RVA: 0x00031B1F File Offset: 0x0002FD1F
		private IEnumerator BadelineLeaves()
		{
			yield return 0.1f;
			this.moved = true;
			this.npc.MoveToNode(this.index, true);
			yield return 0.5f;
			this.player.Sprite.Play("tiredStill", false, false);
			yield return 0.5f;
			this.player.Sprite.Play("idle", false, false);
			yield return 0.6f;
			yield break;
		}

		// Token: 0x04000930 RID: 2352
		private Player player;

		// Token: 0x04000931 RID: 2353
		private NPC05_Badeline npc;

		// Token: 0x04000932 RID: 2354
		private BadelineDummy badeline;

		// Token: 0x04000933 RID: 2355
		private int index;

		// Token: 0x04000934 RID: 2356
		private bool moved;
	}
}
