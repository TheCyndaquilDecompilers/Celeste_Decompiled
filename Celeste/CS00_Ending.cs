using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200027C RID: 636
	public class CS00_Ending : CutsceneEntity
	{
		// Token: 0x060013B3 RID: 5043 RVA: 0x0006B09A File Offset: 0x0006929A
		public CS00_Ending(Player player, BirdNPC bird, Bridge bridge) : base(false, true)
		{
			this.player = player;
			this.bird = bird;
			this.bridge = bridge;
		}

		// Token: 0x060013B4 RID: 5044 RVA: 0x0006B0B9 File Offset: 0x000692B9
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x060013B5 RID: 5045 RVA: 0x0006B0CE File Offset: 0x000692CE
		private IEnumerator Cutscene(Level level)
		{
			while (Engine.TimeRate > 0f)
			{
				yield return null;
				if (Engine.TimeRate < 0.5f && this.bridge != null)
				{
					this.bridge.StopCollapseLoop();
				}
				level.StopShake();
				MInput.GamePads[Input.Gamepad].StopRumble();
				Engine.TimeRate -= Engine.RawDeltaTime * 2f;
			}
			Engine.TimeRate = 0f;
			this.player.StateMachine.State = 11;
			this.player.Facing = Facings.Right;
			yield return this.WaitFor(1f);
			EventInstance instance = Audio.Play("event:/game/general/bird_in", this.bird.Position);
			this.bird.Facing = Facings.Left;
			this.bird.Sprite.Play("fall", false, false);
			float percent = 0f;
			Vector2 from = this.bird.Position;
			Vector2 to = this.bird.StartPosition;
			while (percent < 1f)
			{
				this.bird.Position = from + (to - from) * Ease.QuadOut(percent);
				Audio.Position(instance, this.bird.Position);
				if (percent > 0.5f)
				{
					this.bird.Sprite.Play("fly", false, false);
				}
				percent += Engine.RawDeltaTime * 0.5f;
				yield return null;
			}
			this.bird.Position = to;
			instance = null;
			from = default(Vector2);
			to = default(Vector2);
			Audio.Play("event:/game/general/bird_land_dirt", this.bird.Position);
			Dust.Burst(this.bird.Position, -1.5707964f, 12, null);
			this.bird.Sprite.Play("idle", false, false);
			yield return this.WaitFor(0.5f);
			this.bird.Sprite.Play("peck", false, false);
			yield return this.WaitFor(1.1f);
			yield return this.bird.ShowTutorial(new BirdTutorialGui(this.bird, new Vector2(0f, -16f), Dialog.Clean("tutorial_dash", null), new object[]
			{
				new Vector2(1f, -1f),
				"+",
				BirdTutorialGui.ButtonPrompt.Dash
			}), true);
			for (;;)
			{
				Vector2 aimVector = Input.GetAimVector(Facings.Right);
				if (aimVector.X > 0f && aimVector.Y < 0f && Input.Dash.Pressed)
				{
					break;
				}
				yield return null;
			}
			this.player.StateMachine.State = 16;
			this.player.Dashes = 0;
			level.Session.Inventory.Dashes = 1;
			Engine.TimeRate = 1f;
			this.keyOffed = true;
			Audio.CurrentMusicEventInstance.triggerCue();
			this.bird.Add(new Coroutine(this.bird.HideTutorial(), true));
			yield return 0.25f;
			this.bird.Add(new Coroutine(this.bird.StartleAndFlyAway(), true));
			while (!this.player.Dead && !this.player.OnGround(1))
			{
				yield return null;
			}
			yield return 2f;
			Audio.SetMusic("event:/music/lvl0/title_ping", true, true);
			yield return 2f;
			this.endingText = new PrologueEndingText(false);
			base.Scene.Add(this.endingText);
			Snow bgSnow = level.Background.Get<Snow>();
			Snow fgSnow = level.Foreground.Get<Snow>();
			level.Add(level.HiresSnow = new HiresSnow(0.45f));
			level.HiresSnow.Alpha = 0f;
			float ease = 0f;
			while (ease < 1f)
			{
				ease += Engine.DeltaTime * 0.25f;
				float num = Ease.CubeInOut(ease);
				if (fgSnow != null)
				{
					fgSnow.Alpha -= Engine.DeltaTime * 0.5f;
				}
				if (bgSnow != null)
				{
					bgSnow.Alpha -= Engine.DeltaTime * 0.5f;
				}
				level.HiresSnow.Alpha = Calc.Approach(level.HiresSnow.Alpha, 1f, Engine.DeltaTime * 0.5f);
				this.endingText.Position = new Vector2(960f, 540f - 1080f * (1f - num));
				level.Camera.Y = (float)level.Bounds.Top - 3900f * num;
				yield return null;
			}
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x060013B6 RID: 5046 RVA: 0x0006B0E4 File Offset: 0x000692E4
		private IEnumerator WaitFor(float time)
		{
			for (float t = 0f; t < time; t += Engine.RawDeltaTime)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x060013B7 RID: 5047 RVA: 0x0006B0F4 File Offset: 0x000692F4
		public override void OnEnd(Level level)
		{
			if (this.WasSkipped)
			{
				if (this.bird != null)
				{
					this.bird.Visible = false;
				}
				if (this.player != null)
				{
					this.player.Position = new Vector2(2120f, 40f);
					this.player.StateMachine.State = 11;
					this.player.DummyAutoAnimate = false;
					this.player.Sprite.Play("tired", false, false);
					this.player.Speed = Vector2.Zero;
				}
				if (!this.keyOffed)
				{
					Audio.CurrentMusicEventInstance.triggerCue();
				}
				if (level.HiresSnow == null)
				{
					level.Add(level.HiresSnow = new HiresSnow(0.45f));
				}
				level.HiresSnow.Alpha = 1f;
				Snow snow = level.Background.Get<Snow>();
				if (snow != null)
				{
					snow.Alpha = 0f;
				}
				Snow snow2 = level.Foreground.Get<Snow>();
				if (snow2 != null)
				{
					snow2.Alpha = 0f;
				}
				if (this.endingText != null)
				{
					level.Remove(this.endingText);
				}
				level.Add(this.endingText = new PrologueEndingText(true));
				this.endingText.Position = new Vector2(960f, 540f);
				level.Camera.Y = (float)(level.Bounds.Top - 3900);
			}
			Engine.TimeRate = 1f;
			level.PauseLock = true;
			level.Entities.FindFirst<SpeedrunTimerDisplay>().CompleteTimer = 10f;
			level.Add(new CS00_Ending.EndingCutsceneDelay());
		}

		// Token: 0x04000F7E RID: 3966
		private Player player;

		// Token: 0x04000F7F RID: 3967
		private BirdNPC bird;

		// Token: 0x04000F80 RID: 3968
		private Bridge bridge;

		// Token: 0x04000F81 RID: 3969
		private bool keyOffed;

		// Token: 0x04000F82 RID: 3970
		private PrologueEndingText endingText;

		// Token: 0x020005C4 RID: 1476
		private class EndingCutsceneDelay : Entity
		{
			// Token: 0x0600286F RID: 10351 RVA: 0x00106847 File Offset: 0x00104A47
			public EndingCutsceneDelay()
			{
				base.Add(new Coroutine(this.Routine(), true));
			}

			// Token: 0x06002870 RID: 10352 RVA: 0x00106861 File Offset: 0x00104A61
			private IEnumerator Routine()
			{
				yield return 3f;
				(base.Scene as Level).CompleteArea(false, false, false);
				yield break;
			}
		}
	}
}
