using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000160 RID: 352
	public class CS10_MissTheBird : CutsceneEntity
	{
		// Token: 0x06000C8A RID: 3210 RVA: 0x0002A579 File Offset: 0x00028779
		public CS10_MissTheBird(Player player, FlingBirdIntro flingBird) : base(true, false)
		{
			this.player = player;
			this.flingBird = flingBird;
			base.Add(new LevelEndingHook(delegate()
			{
				Audio.Stop(this.crashMusicSfx, true);
			}));
		}

		// Token: 0x06000C8B RID: 3211 RVA: 0x0002A5A8 File Offset: 0x000287A8
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000C8C RID: 3212 RVA: 0x0002A5BD File Offset: 0x000287BD
		private IEnumerator Cutscene(Level level)
		{
			Audio.SetMusicParam("bird_grab", 1f);
			this.crashMusicSfx = Audio.Play("event:/new_content/music/lvl10/cinematic/bird_crash_first");
			yield return this.flingBird.DoGrabbingRoutine(this.player);
			this.bird = new BirdNPC(this.flingBird.Position, BirdNPC.Modes.None);
			level.Add(this.bird);
			this.flingBird.RemoveSelf();
			yield return null;
			level.ResetZoom();
			level.Shake(0.5f);
			this.player.Position = this.player.Position.Floor();
			this.player.DummyGravity = true;
			this.player.DummyAutoAnimate = false;
			this.player.DummyFriction = false;
			this.player.ForceCameraUpdate = true;
			this.player.Speed = new Vector2(200f, 200f);
			this.bird.Position += Vector2.UnitX * 16f;
			this.bird.Add(new Coroutine(this.bird.Startle(null, 0.5f, new Vector2?(new Vector2(3f, 0.25f))), true));
			base.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate
			{
				this.bird.Sprite.Play("hoverStressed", false, false);
				base.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate
				{
					base.Add(new Coroutine(this.bird.FlyAway(0.2f), true));
					this.bird.Position += new Vector2(0f, -4f);
				}, 0.8f, true));
			}, 0.1f, true));
			while (!this.player.OnGround(1))
			{
				this.player.MoveVExact(1, null, null);
			}
			Engine.TimeRate = 0.5f;
			this.player.Sprite.Play("roll", false, false);
			while (this.player.Speed.X != 0f)
			{
				this.player.Speed.X = Calc.Approach(this.player.Speed.X, 0f, 120f * Engine.DeltaTime);
				if (base.Scene.OnInterval(0.1f))
				{
					Dust.BurstFG(this.player.Position, -1.5707964f, 2, 4f, null);
				}
				yield return null;
			}
			while (Engine.TimeRate < 1f)
			{
				Engine.TimeRate = Calc.Approach(Engine.TimeRate, 1f, 4f * Engine.DeltaTime);
				yield return null;
			}
			this.player.Speed.X = 0f;
			this.player.DummyFriction = true;
			yield return 0.25f;
			base.Add(this.zoomRoutine = new Coroutine(level.ZoomTo(new Vector2(160f, 110f), 1.5f, 6f), true));
			yield return 1.5f;
			this.player.Sprite.Play("rollGetUp", false, false);
			yield return 0.5f;
			this.player.ForceCameraUpdate = false;
			yield return Textbox.Say("CH9_MISS_THE_BIRD", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.StandUpFaceLeft),
				new Func<IEnumerator>(this.TakeStepLeft),
				new Func<IEnumerator>(this.TakeStepRight),
				new Func<IEnumerator>(this.FlickerBlackhole),
				new Func<IEnumerator>(this.OpenBlackhole)
			});
			this.StartMusic();
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000C8D RID: 3213 RVA: 0x0002A5D3 File Offset: 0x000287D3
		private IEnumerator StandUpFaceLeft()
		{
			while (!this.zoomRoutine.Finished)
			{
				yield return null;
			}
			yield return 0.2f;
			Audio.Play("event:/char/madeline/stand", this.player.Position);
			this.player.DummyAutoAnimate = true;
			this.player.Sprite.Play("idle", false, false);
			yield return 0.2f;
			this.player.Facing = Facings.Left;
			yield return 0.5f;
			yield break;
		}

		// Token: 0x06000C8E RID: 3214 RVA: 0x0002A5E2 File Offset: 0x000287E2
		private IEnumerator TakeStepLeft()
		{
			yield return this.player.DummyWalkTo(this.player.X - 16f, false, 1f, false);
			yield break;
		}

		// Token: 0x06000C8F RID: 3215 RVA: 0x0002A5F1 File Offset: 0x000287F1
		private IEnumerator TakeStepRight()
		{
			yield return this.player.DummyWalkTo(this.player.X + 32f, false, 1f, false);
			yield break;
		}

		// Token: 0x06000C90 RID: 3216 RVA: 0x0002A600 File Offset: 0x00028800
		private IEnumerator FlickerBlackhole()
		{
			yield return 0.5f;
			Audio.Play("event:/new_content/game/10_farewell/glitch_medium");
			yield return MoonGlitchBackgroundTrigger.GlitchRoutine(0.5f, false);
			yield return this.player.DummyWalkTo(this.player.X - 8f, true, 1f, false);
			yield return 0.4f;
			yield break;
		}

		// Token: 0x06000C91 RID: 3217 RVA: 0x0002A60F File Offset: 0x0002880F
		private IEnumerator OpenBlackhole()
		{
			yield return 0.2f;
			this.Level.ResetZoom();
			this.Level.Flash(Color.White, false);
			this.Level.Shake(0.4f);
			this.Level.Add(new LightningStrike(new Vector2(this.player.X, (float)this.Level.Bounds.Top), 80, 240f, 0f));
			this.Level.Add(new LightningStrike(new Vector2(this.player.X - 100f, (float)this.Level.Bounds.Top), 90, 240f, 0.5f));
			Audio.Play("event:/new_content/game/10_farewell/lightning_strike");
			this.TriggerEnvironmentalEvents();
			this.StartMusic();
			yield return 1.2f;
			yield break;
		}

		// Token: 0x06000C92 RID: 3218 RVA: 0x0002A620 File Offset: 0x00028820
		private void StartMusic()
		{
			this.Level.Session.Audio.Music.Event = "event:/new_content/music/lvl10/part03";
			this.Level.Session.Audio.Ambience.Event = "event:/new_content/env/10_voidspiral";
			this.Level.Session.Audio.Apply(false);
		}

		// Token: 0x06000C93 RID: 3219 RVA: 0x0002A684 File Offset: 0x00028884
		private void TriggerEnvironmentalEvents()
		{
			CutsceneNode cutsceneNode = CutsceneNode.Find("player_skip");
			if (cutsceneNode != null)
			{
				RumbleTrigger.ManuallyTrigger(cutsceneNode.X, 0f);
			}
			MoonGlitchBackgroundTrigger moonGlitchBackgroundTrigger = base.Scene.Entities.FindFirst<MoonGlitchBackgroundTrigger>();
			if (moonGlitchBackgroundTrigger != null)
			{
				moonGlitchBackgroundTrigger.Invoke();
			}
		}

		// Token: 0x06000C94 RID: 3220 RVA: 0x0002A6CC File Offset: 0x000288CC
		public override void OnEnd(Level level)
		{
			Audio.Stop(this.crashMusicSfx, true);
			Engine.TimeRate = 1f;
			level.Session.SetFlag("MissTheBird", true);
			if (this.WasSkipped)
			{
				this.player.Sprite.Play("idle", false, false);
				CutsceneNode cutsceneNode = CutsceneNode.Find("player_skip");
				if (cutsceneNode != null)
				{
					this.player.Position = cutsceneNode.Position.Floor();
					level.Camera.Position = this.player.CameraTarget;
				}
				if (this.flingBird != null)
				{
					if (this.flingBird.CrashSfxEmitter != null)
					{
						this.flingBird.CrashSfxEmitter.RemoveSelf();
					}
					this.flingBird.RemoveSelf();
				}
				if (this.bird != null)
				{
					this.bird.RemoveSelf();
				}
				this.TriggerEnvironmentalEvents();
				this.StartMusic();
			}
			this.player.Speed = Vector2.Zero;
			this.player.DummyAutoAnimate = true;
			this.player.DummyFriction = true;
			this.player.DummyGravity = true;
			this.player.ForceCameraUpdate = false;
			this.player.StateMachine.State = 0;
		}

		// Token: 0x04000802 RID: 2050
		public const string Flag = "MissTheBird";

		// Token: 0x04000803 RID: 2051
		private Player player;

		// Token: 0x04000804 RID: 2052
		private FlingBirdIntro flingBird;

		// Token: 0x04000805 RID: 2053
		private BirdNPC bird;

		// Token: 0x04000806 RID: 2054
		private Coroutine zoomRoutine;

		// Token: 0x04000807 RID: 2055
		private EventInstance crashMusicSfx;
	}
}
