using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000147 RID: 327
	public class CS10_FinalRoom : CutsceneEntity
	{
		// Token: 0x06000BFB RID: 3067 RVA: 0x000251F0 File Offset: 0x000233F0
		public CS10_FinalRoom(Player player, bool first) : base(true, false)
		{
			base.Depth = -8500;
			this.player = player;
			this.first = first;
		}

		// Token: 0x06000BFC RID: 3068 RVA: 0x00025213 File Offset: 0x00023413
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000BFD RID: 3069 RVA: 0x00025228 File Offset: 0x00023428
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			if (this.first)
			{
				yield return this.player.DummyWalkToExact((int)(this.player.X + 16f), false, 1f, false);
				yield return 0.5f;
			}
			else
			{
				this.player.DummyAutoAnimate = false;
				this.player.Sprite.Play("sitDown", false, false);
				this.player.Sprite.SetAnimationFrame(this.player.Sprite.CurrentAnimationTotalFrames - 1);
				yield return 1.25f;
			}
			yield return this.BadelineAppears();
			if (this.first)
			{
				yield return Textbox.Say("CH9_LAST_ROOM", new Func<IEnumerator>[0]);
			}
			else
			{
				yield return Textbox.Say("CH9_LAST_ROOM_ALT", new Func<IEnumerator>[0]);
			}
			yield return this.BadelineVanishes();
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000BFE RID: 3070 RVA: 0x0002523E File Offset: 0x0002343E
		private IEnumerator BadelineAppears()
		{
			this.Level.Add(this.badeline = new BadelineDummy(this.player.Position + new Vector2(18f, -8f)));
			this.Level.Displacement.AddBurst(this.badeline.Center, 0.5f, 8f, 32f, 0.5f, null, null);
			Audio.Play("event:/char/badeline/maddy_split", this.badeline.Position);
			this.badeline.Sprite.Scale.X = -1f;
			yield return null;
			yield break;
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x0002524D File Offset: 0x0002344D
		private IEnumerator BadelineVanishes()
		{
			yield return 0.2f;
			this.badeline.Vanish();
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			this.badeline = null;
			yield return 0.5f;
			this.player.Facing = Facings.Right;
			yield break;
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x0002525C File Offset: 0x0002345C
		public override void OnEnd(Level level)
		{
			this.Level.Session.Inventory.Dashes = 1;
			this.player.StateMachine.State = 0;
			if (!this.first && !this.WasSkipped)
			{
				Audio.Play("event:/char/madeline/stand", this.player.Position);
			}
			if (this.badeline != null)
			{
				this.badeline.RemoveSelf();
			}
		}

		// Token: 0x0400074C RID: 1868
		private Player player;

		// Token: 0x0400074D RID: 1869
		private BadelineDummy badeline;

		// Token: 0x0400074E RID: 1870
		private bool first;
	}
}
