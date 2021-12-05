using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000145 RID: 325
	public class CS10_Farewell : CutsceneEntity
	{
		// Token: 0x06000BE1 RID: 3041 RVA: 0x00024C2C File Offset: 0x00022E2C
		public CS10_Farewell(Player player) : base(false, false)
		{
			this.player = player;
			base.Depth = -1000000;
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x00024C48 File Offset: 0x00022E48
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Level level = scene as Level;
			level.TimerStopped = true;
			level.TimerHidden = true;
			level.SaveQuitDisabled = true;
			level.SnapColorGrade("none");
			this.snapshot = Audio.CreateSnapshot("snapshot:/game_10_granny_clouds_dialogue", true);
		}

		// Token: 0x06000BE3 RID: 3043 RVA: 0x00024C87 File Offset: 0x00022E87
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x00024C9C File Offset: 0x00022E9C
		private IEnumerator Cutscene(Level level)
		{
			this.player.Dashes = 1;
			this.player.StateMachine.State = 11;
			this.player.Sprite.Play("idle", false, false);
			this.player.Visible = false;
			Audio.SetMusic("event:/new_content/music/lvl10/granny_farewell", true, true);
			FadeWipe fadeWipe = new FadeWipe(this.Level, true, null);
			fadeWipe.Duration = 2f;
			ScreenWipe.WipeColor = Color.White;
			yield return fadeWipe.Duration;
			yield return 1.5f;
			base.Add(new Coroutine(this.Level.ZoomTo(new Vector2(160f, 125f), 2f, 5f), true));
			yield return 0.2f;
			Audio.Play("event:/new_content/char/madeline/screenentry_gran");
			yield return 0.3f;
			Vector2 position = this.player.Position;
			this.player.Position = new Vector2(this.player.X, (float)(level.Bounds.Bottom + 8));
			this.player.Speed.Y = -160f;
			this.player.Visible = true;
			this.player.DummyGravity = false;
			this.player.MuffleLanding = true;
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			while (!this.player.OnGround(1) || this.player.Speed.Y < 0f)
			{
				float y = this.player.Speed.Y;
				Player player = this.player;
				player.Speed.Y = player.Speed.Y + Engine.DeltaTime * 900f * 0.2f;
				if (y < 0f && this.player.Speed.Y >= 0f)
				{
					this.player.Speed.Y = 0f;
					yield return 0.2f;
				}
				yield return null;
			}
			Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
			Audio.Play("event:/new_content/char/madeline/screenentry_gran_landing", this.player.Position);
			this.granny = new NPC(this.player.Position + new Vector2(164f, 0f));
			this.granny.IdleAnim = "idle";
			this.granny.MoveAnim = "walk";
			this.granny.Maxspeed = 15f;
			this.granny.Add(this.granny.Sprite = GFX.SpriteBank.Create("granny"));
			GrannyLaughSfx grannyLaughSfx = new GrannyLaughSfx(this.granny.Sprite);
			grannyLaughSfx.FirstPlay = false;
			this.granny.Add(grannyLaughSfx);
			this.granny.Sprite.OnFrameChange = delegate(string anim)
			{
				int currentAnimationFrame = this.granny.Sprite.CurrentAnimationFrame;
				if (anim == "walk" && currentAnimationFrame == 2)
				{
					float volume = Calc.ClampedMap((this.player.Position - this.granny.Position).Length(), 64f, 128f, 1f, 0f);
					Audio.Play("event:/new_content/char/granny/cane_tap_ending", this.granny.Position).setVolume(volume);
				}
			};
			base.Scene.Add(this.granny);
			this.grannyWalk = new Coroutine(this.granny.MoveTo(this.player.Position + new Vector2(32f, 0f), false, null, false), true);
			base.Add(this.grannyWalk);
			yield return 2f;
			this.player.Facing = Facings.Left;
			yield return 1.6f;
			this.player.Facing = Facings.Right;
			yield return 0.8f;
			yield return this.player.DummyWalkToExact((int)this.player.X + 4, false, 0.4f, false);
			yield return 0.8f;
			yield return Textbox.Say("CH9_FAREWELL", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.Laugh),
				new Func<IEnumerator>(this.StopLaughing),
				new Func<IEnumerator>(this.StepForward),
				new Func<IEnumerator>(this.GrannyDisappear),
				new Func<IEnumerator>(this.FadeToWhite),
				new Func<IEnumerator>(this.WaitForGranny)
			});
			yield return 2f;
			while (this.fade < 1f)
			{
				yield return null;
			}
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000BE5 RID: 3045 RVA: 0x00024CB2 File Offset: 0x00022EB2
		private IEnumerator WaitForGranny()
		{
			while (this.grannyWalk != null && !this.grannyWalk.Finished)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x00024CC1 File Offset: 0x00022EC1
		private IEnumerator Laugh()
		{
			this.granny.Sprite.Play("laugh", false, false);
			yield break;
		}

		// Token: 0x06000BE7 RID: 3047 RVA: 0x00024CD0 File Offset: 0x00022ED0
		private IEnumerator StopLaughing()
		{
			this.granny.Sprite.Play("idle", false, false);
			yield break;
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x00024CDF File Offset: 0x00022EDF
		private IEnumerator StepForward()
		{
			yield return this.player.DummyWalkToExact((int)this.player.X + 8, false, 0.4f, false);
			yield break;
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x00024CEE File Offset: 0x00022EEE
		private IEnumerator GrannyDisappear()
		{
			Audio.SetMusicParam("end", 1f);
			base.Add(new Coroutine(this.player.DummyWalkToExact((int)this.player.X + 8, false, 0.4f, false), true));
			yield return 0.1f;
			this.dissipate = Audio.Play("event:/new_content/char/granny/dissipate", this.granny.Position);
			MTexture frame = this.granny.Sprite.GetFrame(this.granny.Sprite.CurrentAnimationID, this.granny.Sprite.CurrentAnimationFrame);
			this.Level.Add(new DisperseImage(this.granny.Position, new Vector2(1f, -0.1f), this.granny.Sprite.Origin, this.granny.Sprite.Scale, frame));
			yield return null;
			this.granny.Visible = false;
			yield return 3.5f;
			yield break;
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x00024CFD File Offset: 0x00022EFD
		private IEnumerator FadeToWhite()
		{
			base.Add(new Coroutine(this.DoFadeToWhite(), true));
			yield break;
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x00024D0C File Offset: 0x00022F0C
		private IEnumerator DoFadeToWhite()
		{
			base.Add(new Coroutine(this.Level.ZoomBack(8f), true));
			while (this.fade < 1f)
			{
				this.fade = Calc.Approach(this.fade, 1f, Engine.DeltaTime / 8f);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x00024D1B File Offset: 0x00022F1B
		public override void OnEnd(Level level)
		{
			this.Dispose();
			if (this.WasSkipped)
			{
				Audio.Stop(this.dissipate, true);
			}
			this.Level.OnEndOfFrame += delegate()
			{
				Achievements.Register(Achievement.FAREWELL);
				this.Level.TeleportTo(this.player, "end-cinematic", Player.IntroTypes.Transition, null);
			};
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x00024D4E File Offset: 0x00022F4E
		public override void SceneEnd(Scene scene)
		{
			base.SceneEnd(scene);
			this.Dispose();
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x00024D5D File Offset: 0x00022F5D
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			this.Dispose();
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x00024D6C File Offset: 0x00022F6C
		private void Dispose()
		{
			Audio.ReleaseSnapshot(this.snapshot);
			this.snapshot = null;
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x00024D80 File Offset: 0x00022F80
		public override void Render()
		{
			if (this.fade > 0f)
			{
				Draw.Rect(this.Level.Camera.X - 1f, this.Level.Camera.Y - 1f, 322f, 182f, Color.White * this.fade);
			}
		}

		// Token: 0x0400073A RID: 1850
		private Player player;

		// Token: 0x0400073B RID: 1851
		private NPC granny;

		// Token: 0x0400073C RID: 1852
		private float fade;

		// Token: 0x0400073D RID: 1853
		private Coroutine grannyWalk;

		// Token: 0x0400073E RID: 1854
		private EventInstance snapshot;

		// Token: 0x0400073F RID: 1855
		private EventInstance dissipate;
	}
}
