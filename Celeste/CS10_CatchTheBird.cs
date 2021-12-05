using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000169 RID: 361
	public class CS10_CatchTheBird : CutsceneEntity
	{
		// Token: 0x06000CDC RID: 3292 RVA: 0x0002B83C File Offset: 0x00029A3C
		public CS10_CatchTheBird(Player player, FlingBirdIntro flingBird) : base(true, false)
		{
			this.player = player;
			this.flingBird = flingBird;
			this.birdWaitPosition = flingBird.BirdEndPosition;
		}

		// Token: 0x06000CDD RID: 3293 RVA: 0x0002B860 File Offset: 0x00029A60
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000CDE RID: 3294 RVA: 0x0002B875 File Offset: 0x00029A75
		private IEnumerator Cutscene(Level level)
		{
			Audio.SetMusic("event:/new_content/music/lvl10/cinematic/bird_crash_second", true, true);
			BadelineBoost boost = base.Scene.Entities.FindFirst<BadelineBoost>();
			if (boost != null)
			{
				boost.Active = (boost.Visible = (boost.Collidable = false));
			}
			yield return this.flingBird.DoGrabbingRoutine(this.player);
			this.flingBird.Sprite.Play("hurt", false, false);
			this.flingBird.X += 8f;
			while (!this.player.OnGround(1))
			{
				this.player.MoveVExact(1, null, null);
			}
			while (this.player.CollideCheck<Solid>())
			{
				Player player = this.player;
				float y = player.Y;
				player.Y = y - 1f;
			}
			Engine.TimeRate = 0.65f;
			float ground = this.player.Position.Y;
			this.player.Dashes = 1;
			this.player.Sprite.Play("roll", false, false);
			this.player.Speed.X = 200f;
			this.player.DummyFriction = false;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime)
			{
				this.player.Speed.X = Calc.Approach(this.player.Speed.X, 0f, 160f * Engine.DeltaTime);
				if (this.player.Speed.X != 0f && base.Scene.OnInterval(0.1f))
				{
					Dust.BurstFG(this.player.Position, -1.5707964f, 2, 4f, null);
				}
				FlingBirdIntro flingBirdIntro = this.flingBird;
				flingBirdIntro.Position.X = flingBirdIntro.Position.X + Engine.DeltaTime * 80f * Ease.CubeOut(1f - p);
				this.flingBird.Position.Y = ground;
				yield return null;
			}
			this.player.Speed.X = 0f;
			this.player.DummyFriction = true;
			this.player.DummyGravity = true;
			yield return 0.25f;
			while (Engine.TimeRate < 1f)
			{
				Engine.TimeRate = Calc.Approach(Engine.TimeRate, 1f, 4f * Engine.DeltaTime);
				yield return null;
			}
			this.player.ForceCameraUpdate = false;
			yield return 0.6f;
			this.player.Sprite.Play("rollGetUp", false, false);
			yield return 0.8f;
			level.Session.Audio.Music.Event = "event:/new_content/music/lvl10/reconciliation";
			level.Session.Audio.Apply(false);
			yield return Textbox.Say("CH9_CATCH_THE_BIRD", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.BirdLooksHurt),
				new Func<IEnumerator>(this.BirdSquakOnGround),
				new Func<IEnumerator>(this.ApproachBird),
				new Func<IEnumerator>(this.ApproachBirdAgain),
				new Func<IEnumerator>(this.BadelineAppears),
				new Func<IEnumerator>(this.WaitABeat),
				new Func<IEnumerator>(this.MadelineSits),
				new Func<IEnumerator>(this.BadelineHugs),
				new Func<IEnumerator>(this.StandUp),
				new Func<IEnumerator>(this.ShiftCameraToBird)
			});
			yield return level.ZoomBack(0.5f);
			if (this.badeline != null)
			{
				this.badeline.Vanish();
			}
			yield return 0.5f;
			if (boost != null)
			{
				this.Level.Displacement.AddBurst(boost.Center, 0.5f, 8f, 32f, 0.5f, null, null);
				Audio.Play("event:/new_content/char/badeline/booster_first_appear", boost.Center);
				boost.Active = (boost.Visible = (boost.Collidable = true));
				yield return 0.2f;
			}
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000CDF RID: 3295 RVA: 0x0002B88B File Offset: 0x00029A8B
		private IEnumerator BirdTwitches(string sfx = null)
		{
			this.flingBird.Sprite.Scale.Y = 1.6f;
			if (!string.IsNullOrWhiteSpace(sfx))
			{
				Audio.Play(sfx, this.flingBird.Position);
			}
			while (this.flingBird.Sprite.Scale.Y > 1f)
			{
				this.flingBird.Sprite.Scale.Y = Calc.Approach(this.flingBird.Sprite.Scale.Y, 1f, 2f * Engine.DeltaTime);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000CE0 RID: 3296 RVA: 0x0002B8A1 File Offset: 0x00029AA1
		private IEnumerator BirdLooksHurt()
		{
			yield return 0.8f;
			yield return this.BirdTwitches("event:/new_content/game/10_farewell/bird_crashscene_twitch_1");
			yield return 0.4f;
			yield return this.BirdTwitches("event:/new_content/game/10_farewell/bird_crashscene_twitch_2");
			yield return 0.5f;
			yield break;
		}

		// Token: 0x06000CE1 RID: 3297 RVA: 0x0002B8B0 File Offset: 0x00029AB0
		private IEnumerator BirdSquakOnGround()
		{
			yield return 0.6f;
			yield return this.BirdTwitches("event:/new_content/game/10_farewell/bird_crashscene_twitch_3");
			yield return 0.8f;
			Audio.Play("event:/new_content/game/10_farewell/bird_crashscene_recover", this.flingBird.Position);
			this.flingBird.RemoveSelf();
			base.Scene.Add(this.bird = new BirdNPC(this.flingBird.Position, BirdNPC.Modes.None));
			this.bird.Facing = Facings.Right;
			this.bird.Sprite.Play("recover", false, false);
			yield return 0.6f;
			this.bird.Facing = Facings.Left;
			this.bird.Sprite.Play("idle", false, false);
			this.bird.X += 3f;
			yield return 0.4f;
			yield return this.bird.Caw();
			yield break;
		}

		// Token: 0x06000CE2 RID: 3298 RVA: 0x0002B8BF File Offset: 0x00029ABF
		private IEnumerator ApproachBird()
		{
			this.player.DummyAutoAnimate = true;
			yield return 0.25f;
			yield return this.bird.Caw();
			base.Add(new Coroutine(this.player.DummyWalkTo(this.player.X + 20f, false, 1f, false), true));
			yield return 0.1f;
			Audio.Play("event:/game/general/bird_startle", this.bird.Position);
			yield return this.bird.Startle("event:/new_content/game/10_farewell/bird_crashscene_relocate", 0.8f, null);
			yield return this.bird.FlyTo(new Vector2(this.player.X + 80f, this.player.Y), 3f, false);
			yield break;
		}

		// Token: 0x06000CE3 RID: 3299 RVA: 0x0002B8CE File Offset: 0x00029ACE
		private IEnumerator ApproachBirdAgain()
		{
			Audio.Play("event:/new_content/game/10_farewell/bird_crashscene_leave", this.bird.Position);
			base.Add(new Coroutine(this.bird.FlyTo(this.birdWaitPosition, 2f, false), true));
			yield return this.player.DummyWalkTo(this.player.X + 20f, false, 1f, false);
			this.snapshot = Audio.CreateSnapshot("snapshot:/game_10_bird_wings_silenced", true);
			yield return 0.8f;
			this.bird.RemoveSelf();
			base.Scene.Add(this.bird = new BirdNPC(this.birdWaitPosition, BirdNPC.Modes.WaitForLightningOff));
			this.bird.Facing = Facings.Right;
			this.bird.FlyAwayUp = false;
			this.bird.WaitForLightningPostDelay = 1f;
			yield break;
		}

		// Token: 0x06000CE4 RID: 3300 RVA: 0x0002B8DD File Offset: 0x00029ADD
		private IEnumerator BadelineAppears()
		{
			yield return this.player.DummyWalkToExact((int)this.player.X + 20, false, 0.5f, false);
			this.Level.Add(this.badeline = new BadelineDummy(this.player.Position + new Vector2(24f, -8f)));
			this.Level.Displacement.AddBurst(this.badeline.Center, 0.5f, 8f, 32f, 0.5f, null, null);
			Audio.Play("event:/char/badeline/maddy_split", this.player.Position);
			this.badeline.Sprite.Scale.X = -1f;
			yield return 0.2f;
			yield break;
		}

		// Token: 0x06000CE5 RID: 3301 RVA: 0x0002B8EC File Offset: 0x00029AEC
		private IEnumerator WaitABeat()
		{
			yield return this.player.DummyWalkToExact((int)this.player.X - 4, true, 0.5f, false);
			yield return 0.8f;
			yield break;
		}

		// Token: 0x06000CE6 RID: 3302 RVA: 0x0002B8FB File Offset: 0x00029AFB
		private IEnumerator MadelineSits()
		{
			yield return 0.5f;
			yield return this.player.DummyWalkToExact((int)this.player.X - 16, false, 0.25f, false);
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("sitDown", false, false);
			yield return 1.5f;
			yield break;
		}

		// Token: 0x06000CE7 RID: 3303 RVA: 0x0002B90A File Offset: 0x00029B0A
		private IEnumerator BadelineHugs()
		{
			yield return 1f;
			yield return this.badeline.FloatTo(this.badeline.Position + new Vector2(0f, 8f), null, true, false, true);
			this.badeline.Floatness = 0f;
			this.badeline.AutoAnimator.Enabled = false;
			this.badeline.Sprite.Play("idle", false, false);
			Audio.Play("event:/char/badeline/landing", this.badeline.Position);
			yield return 0.5f;
			yield return this.badeline.WalkTo(this.player.X - 9f, 40f);
			this.badeline.Sprite.Scale.X = 1f;
			yield return 0.2f;
			Audio.Play("event:/char/badeline/duck", this.badeline.Position);
			this.badeline.Depth = this.player.Depth + 5;
			this.badeline.Sprite.Play("hug", false, false);
			yield return 1f;
			yield break;
		}

		// Token: 0x06000CE8 RID: 3304 RVA: 0x0002B919 File Offset: 0x00029B19
		private IEnumerator StandUp()
		{
			Audio.Play("event:/char/badeline/stand", this.badeline.Position);
			yield return this.badeline.WalkTo(this.badeline.X - 8f, 64f);
			this.badeline.Sprite.Scale.X = 1f;
			yield return 0.2f;
			this.player.DummyAutoAnimate = true;
			this.Level.NextColorGrade("none", 0.25f);
			yield return 0.25f;
			yield break;
		}

		// Token: 0x06000CE9 RID: 3305 RVA: 0x0002B928 File Offset: 0x00029B28
		private IEnumerator ShiftCameraToBird()
		{
			Audio.ReleaseSnapshot(this.snapshot);
			this.snapshot = null;
			Audio.Play("event:/new_content/char/badeline/birdcrash_scene_float", this.badeline.Position);
			base.Add(new Coroutine(this.badeline.FloatTo(this.player.Position + new Vector2(-16f, -16f), new int?(1), true, false, false), true));
			Level level = base.Scene as Level;
			this.player.Facing = Facings.Right;
			yield return level.ZoomAcross(level.ZoomFocusPoint + new Vector2(70f, 0f), 1.5f, 1f);
			yield return 0.4;
			yield break;
		}

		// Token: 0x06000CEA RID: 3306 RVA: 0x0002B938 File Offset: 0x00029B38
		public override void OnEnd(Level level)
		{
			Audio.ReleaseSnapshot(this.snapshot);
			this.snapshot = null;
			if (this.WasSkipped)
			{
				CutsceneNode cutsceneNode = CutsceneNode.Find("player_skip");
				if (cutsceneNode != null)
				{
					this.player.Sprite.Play("idle", false, false);
					this.player.Position = cutsceneNode.Position.Floor();
					level.Camera.Position = this.player.CameraTarget;
				}
				foreach (Lightning lightning in base.Scene.Entities.FindAll<Lightning>())
				{
					lightning.ToggleCheck();
				}
				LightningRenderer entity = base.Scene.Tracker.GetEntity<LightningRenderer>();
				if (entity != null)
				{
					entity.ToggleEdges(true);
				}
				level.Session.Audio.Music.Event = "event:/new_content/music/lvl10/reconciliation";
				level.Session.Audio.Apply(false);
			}
			this.player.Speed = Vector2.Zero;
			this.player.DummyGravity = true;
			this.player.DummyFriction = true;
			this.player.DummyAutoAnimate = true;
			this.player.ForceCameraUpdate = false;
			this.player.StateMachine.State = 0;
			BadelineBoost badelineBoost = base.Scene.Entities.FindFirst<BadelineBoost>();
			if (badelineBoost != null)
			{
				badelineBoost.Active = (badelineBoost.Visible = (badelineBoost.Collidable = true));
			}
			if (this.badeline != null)
			{
				this.badeline.RemoveSelf();
			}
			if (this.flingBird != null)
			{
				if (this.flingBird.CrashSfxEmitter != null)
				{
					this.flingBird.CrashSfxEmitter.RemoveSelf();
				}
				this.flingBird.RemoveSelf();
			}
			if (this.WasSkipped)
			{
				if (this.bird != null)
				{
					this.bird.RemoveSelf();
				}
				base.Scene.Add(this.bird = new BirdNPC(this.birdWaitPosition, BirdNPC.Modes.WaitForLightningOff));
				this.bird.Facing = Facings.Right;
				this.bird.FlyAwayUp = false;
				this.bird.WaitForLightningPostDelay = 1f;
				level.SnapColorGrade("none");
			}
			level.ResetZoom();
		}

		// Token: 0x06000CEB RID: 3307 RVA: 0x0002BB84 File Offset: 0x00029D84
		public override void Removed(Scene scene)
		{
			Audio.ReleaseSnapshot(this.snapshot);
			this.snapshot = null;
			base.Removed(scene);
		}

		// Token: 0x06000CEC RID: 3308 RVA: 0x0002BB9F File Offset: 0x00029D9F
		public override void SceneEnd(Scene scene)
		{
			Audio.ReleaseSnapshot(this.snapshot);
			this.snapshot = null;
			base.SceneEnd(scene);
		}

		// Token: 0x06000CED RID: 3309 RVA: 0x0002BBBC File Offset: 0x00029DBC
		public static void HandlePostCutsceneSpawn(FlingBirdIntro flingBird, Level level)
		{
			BadelineBoost badelineBoost = level.Entities.FindFirst<BadelineBoost>();
			if (badelineBoost != null)
			{
				badelineBoost.Active = (badelineBoost.Visible = (badelineBoost.Collidable = true));
			}
			if (flingBird != null)
			{
				flingBird.RemoveSelf();
			}
			BirdNPC birdNPC;
			level.Add(birdNPC = new BirdNPC(flingBird.BirdEndPosition, BirdNPC.Modes.WaitForLightningOff));
			birdNPC.Facing = Facings.Right;
			birdNPC.FlyAwayUp = false;
		}

		// Token: 0x04000836 RID: 2102
		private Player player;

		// Token: 0x04000837 RID: 2103
		private FlingBirdIntro flingBird;

		// Token: 0x04000838 RID: 2104
		private BadelineDummy badeline;

		// Token: 0x04000839 RID: 2105
		private BirdNPC bird;

		// Token: 0x0400083A RID: 2106
		private Vector2 birdWaitPosition;

		// Token: 0x0400083B RID: 2107
		private EventInstance snapshot;
	}
}
