using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200027D RID: 637
	public class CS01_Ending : CutsceneEntity
	{
		// Token: 0x060013B8 RID: 5048 RVA: 0x0006B294 File Offset: 0x00069494
		public CS01_Ending(Player player) : base(false, true)
		{
			this.player = player;
		}

		// Token: 0x060013B9 RID: 5049 RVA: 0x0006B2A5 File Offset: 0x000694A5
		public override void OnBegin(Level level)
		{
			level.RegisterAreaComplete();
			this.bonfire = base.Scene.Tracker.GetEntity<Bonfire>();
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x0006B2D6 File Offset: 0x000694D6
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.Dashes = 1;
			level.Session.Audio.Music.Layer(3, false);
			level.Session.Audio.Apply(false);
			yield return 0.5f;
			yield return this.player.DummyWalkTo(this.bonfire.X + 40f, false, 1f, false);
			yield return 1.5f;
			this.player.Facing = Facings.Left;
			yield return 0.5f;
			yield return Textbox.Say("CH1_END", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.EndCityTrigger)
			});
			yield return 0.3f;
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x060013BB RID: 5051 RVA: 0x0006B2EC File Offset: 0x000694EC
		private IEnumerator EndCityTrigger()
		{
			yield return 0.2f;
			yield return this.player.DummyWalkTo(this.bonfire.X - 12f, false, 1f, false);
			yield return 0.2f;
			this.player.Facing = Facings.Right;
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("duck", false, false);
			yield return 0.5f;
			if (this.bonfire != null)
			{
				this.bonfire.SetMode(Bonfire.Mode.Lit);
			}
			yield return 1f;
			this.player.Sprite.Play("idle", false, false);
			yield return 0.4f;
			this.player.DummyAutoAnimate = true;
			yield return this.player.DummyWalkTo(this.bonfire.X - 24f, false, 1f, false);
			yield return 0.4f;
			this.player.DummyAutoAnimate = false;
			this.player.Facing = Facings.Right;
			this.player.Sprite.Play("sleep", false, false);
			Audio.Play("event:/char/madeline/campfire_sit", this.player.Position);
			yield return 4f;
			BirdNPC bird = new BirdNPC(this.player.Position + new Vector2(88f, -200f), BirdNPC.Modes.None);
			base.Scene.Add(bird);
			EventInstance instance = Audio.Play("event:/game/general/bird_in", bird.Position);
			bird.Facing = Facings.Left;
			bird.Sprite.Play("fall", false, false);
			Vector2 from = bird.Position;
			Vector2 to = this.player.Position + new Vector2(1f, -12f);
			float percent = 0f;
			while (percent < 1f)
			{
				bird.Position = from + (to - from) * Ease.QuadOut(percent);
				Audio.Position(instance, bird.Position);
				if (percent > 0.5f)
				{
					bird.Sprite.Play("fly", false, false);
				}
				percent += Engine.DeltaTime * 0.5f;
				yield return null;
			}
			bird.Position = to;
			bird.Sprite.Play("idle", false, false);
			yield return 0.5f;
			bird.Sprite.Play("croak", false, false);
			yield return 0.6f;
			Audio.Play("event:/game/general/bird_squawk", bird.Position);
			yield return 0.9f;
			bird.Sprite.Play("sleep", false, false);
			yield return null;
			bird = null;
			instance = null;
			from = default(Vector2);
			to = default(Vector2);
			yield return 2f;
			yield break;
		}

		// Token: 0x060013BC RID: 5052 RVA: 0x0006B2FB File Offset: 0x000694FB
		public override void OnEnd(Level level)
		{
			level.CompleteArea(true, false, false);
		}

		// Token: 0x04000F83 RID: 3971
		private Player player;

		// Token: 0x04000F84 RID: 3972
		private Bonfire bonfire;
	}
}
