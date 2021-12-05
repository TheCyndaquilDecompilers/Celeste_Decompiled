using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000166 RID: 358
	public class CS05_TheoPhone : CutsceneEntity
	{
		// Token: 0x06000CB1 RID: 3249 RVA: 0x0002AB53 File Offset: 0x00028D53
		public CS05_TheoPhone(Player player, float targetX) : base(true, false)
		{
			this.player = player;
			this.targetX = targetX;
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x0002AB6B File Offset: 0x00028D6B
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Routine(), true));
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x0002AB7F File Offset: 0x00028D7F
		private IEnumerator Routine()
		{
			this.player.StateMachine.State = 11;
			if (this.player.X != this.targetX)
			{
				this.player.Facing = (Facings)Math.Sign(this.targetX - this.player.X);
			}
			yield return 0.5f;
			yield return this.Level.ZoomTo(new Vector2(80f, 60f), 2f, 0.5f);
			yield return Textbox.Say("CH5_PHONE", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.WalkToPhone),
				new Func<IEnumerator>(this.StandBackUp)
			});
			yield return this.Level.ZoomBack(0.5f);
			base.EndCutscene(this.Level, true);
			yield break;
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x0002AB8E File Offset: 0x00028D8E
		private IEnumerator WalkToPhone()
		{
			yield return 0.25f;
			yield return this.player.DummyWalkToExact((int)this.targetX, false, 1f, false);
			this.player.Facing = Facings.Left;
			yield return 0.5f;
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("duck", false, false);
			yield return 0.5f;
			yield break;
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x0002AB9D File Offset: 0x00028D9D
		private IEnumerator StandBackUp()
		{
			this.RemovePhone();
			yield return 0.6f;
			this.player.Sprite.Play("idle", false, false);
			yield return 0.2f;
			yield break;
		}

		// Token: 0x06000CB6 RID: 3254 RVA: 0x0002ABAC File Offset: 0x00028DAC
		public override void OnEnd(Level level)
		{
			this.RemovePhone();
			this.player.StateMachine.State = 0;
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x0002ABC8 File Offset: 0x00028DC8
		private void RemovePhone()
		{
			TheoPhone theoPhone = base.Scene.Entities.FindFirst<TheoPhone>();
			if (theoPhone != null)
			{
				theoPhone.RemoveSelf();
			}
		}

		// Token: 0x04000813 RID: 2067
		private Player player;

		// Token: 0x04000814 RID: 2068
		private float targetX;
	}
}
