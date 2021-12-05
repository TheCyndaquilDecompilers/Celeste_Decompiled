using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000146 RID: 326
	public class CS10_FinalLaunch : CutsceneEntity
	{
		// Token: 0x06000BF3 RID: 3059 RVA: 0x00024EA4 File Offset: 0x000230A4
		public CS10_FinalLaunch(Player player, BadelineBoost boost, bool sayDialog = true) : base(false, false)
		{
			this.player = player;
			this.boost = boost;
			this.sayDialog = sayDialog;
			base.Depth = 10010;
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x00024ED0 File Offset: 0x000230D0
		public override void OnBegin(Level level)
		{
			Audio.SetMusic(null, true, true);
			ScreenWipe.WipeColor = Color.White;
			foreach (Follower follower in this.player.Leader.Followers)
			{
				Strawberry strawberry = follower.Entity as Strawberry;
				if (strawberry != null && strawberry.Golden)
				{
					this.hasGolden = true;
					break;
				}
			}
			base.Add(new Coroutine(this.Cutscene(), true));
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x00024F6C File Offset: 0x0002316C
		private IEnumerator Cutscene()
		{
			Engine.TimeRate = 1f;
			this.boost.Active = false;
			yield return null;
			if (this.sayDialog)
			{
				yield return Textbox.Say("CH9_LAST_BOOST", new Func<IEnumerator>[0]);
			}
			else
			{
				yield return 0.152f;
			}
			this.cameraOffset = new Vector2(0f, -20f);
			this.boost.Active = true;
			this.player.EnforceLevelBounds = false;
			yield return null;
			BlackholeBG blackholeBG = this.Level.Background.Get<BlackholeBG>();
			blackholeBG.Direction = -2.5f;
			blackholeBG.SnapStrength(this.Level, BlackholeBG.Strengths.High);
			blackholeBG.CenterOffset.Y = 100f;
			blackholeBG.OffsetOffset.Y = -50f;
			base.Add(this.wave = new Coroutine(this.WaveCamera(), true));
			base.Add(new Coroutine(this.BirdRoutine(0.8f), true));
			this.Level.Add(this.streaks = new AscendManager.Streaks(null));
			float p;
			for (p = 0f; p < 1f; p += Engine.DeltaTime / 12f)
			{
				this.fadeToWhite = p;
				this.streaks.Alpha = p;
				foreach (Parallax parallax in this.Level.Foreground.GetEach<Parallax>("blackhole"))
				{
					parallax.FadeAlphaMultiplier = 1f - p;
				}
				yield return null;
			}
			while (this.bird != null)
			{
				yield return null;
			}
			FadeWipe wipe = new FadeWipe(this.Level, false, null);
			wipe.Duration = 4f;
			ScreenWipe.WipeColor = Color.White;
			if (!this.hasGolden)
			{
				Audio.SetMusic("event:/new_content/music/lvl10/granny_farewell", true, true);
			}
			p = this.cameraOffset.Y;
			int to = 180;
			for (float p2 = 0f; p2 < 1f; p2 += Engine.DeltaTime / 2f)
			{
				this.cameraOffset.Y = p + ((float)to - p) * Ease.BigBackIn(p2);
				yield return null;
			}
			while (wipe.Percent < 1f)
			{
				yield return null;
			}
			base.EndCutscene(this.Level, true);
			yield break;
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x00024F7C File Offset: 0x0002317C
		public override void OnEnd(Level level)
		{
			if (this.WasSkipped && this.boost != null && this.boost.Ch9FinalBoostSfx != null)
			{
				this.boost.Ch9FinalBoostSfx.stop(STOP_MODE.ALLOWFADEOUT);
				this.boost.Ch9FinalBoostSfx.release();
			}
			string nextLevelName = "end-granny";
			Player.IntroTypes nextLevelIntro = Player.IntroTypes.Transition;
			if (this.hasGolden)
			{
				nextLevelName = "end-golden";
				nextLevelIntro = Player.IntroTypes.Jump;
			}
			this.player.Active = true;
			this.player.Speed = Vector2.Zero;
			this.player.EnforceLevelBounds = true;
			this.player.StateMachine.State = 0;
			this.player.DummyFriction = true;
			this.player.DummyGravity = true;
			this.player.DummyAutoAnimate = true;
			this.player.ForceCameraUpdate = false;
			Engine.TimeRate = 1f;
			this.Level.OnEndOfFrame += delegate()
			{
				this.Level.TeleportTo(this.player, nextLevelName, nextLevelIntro, null);
				if (this.hasGolden)
				{
					if (this.Level.Wipe != null)
					{
						this.Level.Wipe.Cancel();
					}
					this.Level.SnapColorGrade("golden");
					new FadeWipe(level, true, null).Duration = 2f;
					ScreenWipe.WipeColor = Color.White;
				}
			};
		}

		// Token: 0x06000BF7 RID: 3063 RVA: 0x00025099 File Offset: 0x00023299
		private IEnumerator WaveCamera()
		{
			float timer = 0f;
			for (;;)
			{
				this.cameraWaveOffset.X = (float)Math.Sin((double)timer) * 16f;
				this.cameraWaveOffset.Y = (float)Math.Sin((double)(timer * 0.5f)) * 16f + (float)Math.Sin((double)(timer * 0.25f)) * 8f;
				timer += Engine.DeltaTime * 2f;
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x000250A8 File Offset: 0x000232A8
		private IEnumerator BirdRoutine(float delay)
		{
			yield return delay;
			this.Level.Add(this.bird = new BirdNPC(Vector2.Zero, BirdNPC.Modes.None));
			this.bird.Sprite.Play("flyupIdle", false, false);
			Vector2 vector = new Vector2(320f, 180f) / 2f;
			Vector2 topCenter = new Vector2(vector.X, 0f);
			Vector2 value = new Vector2(vector.X, 180f);
			Vector2 from = value + new Vector2(40f, 40f);
			Vector2 to = vector + new Vector2(-32f, -24f);
			for (float t = 0f; t < 1f; t += Engine.DeltaTime / 4f)
			{
				this.birdScreenPosition = from + (to - from) * Ease.BackOut(t);
				yield return null;
			}
			this.bird.Sprite.Play("flyupRoll", false, false);
			for (float t = 0f; t < 1f; t += Engine.DeltaTime / 2f)
			{
				this.birdScreenPosition = to + new Vector2(64f, 0f) * Ease.CubeInOut(t);
				yield return null;
			}
			from = default(Vector2);
			to = default(Vector2);
			to = this.birdScreenPosition;
			from = topCenter + new Vector2(-40f, -100f);
			bool playedAnim = false;
			for (float t = 0f; t < 1f; t += Engine.DeltaTime / 4f)
			{
				if (t >= 0.35f && !playedAnim)
				{
					this.bird.Sprite.Play("flyupRoll", false, false);
					playedAnim = true;
				}
				this.birdScreenPosition = to + (from - to) * Ease.BigBackIn(t);
				this.birdScreenPosition.X = this.birdScreenPosition.X + t * 32f;
				yield return null;
			}
			this.bird.RemoveSelf();
			this.bird = null;
			to = default(Vector2);
			from = default(Vector2);
			yield break;
		}

		// Token: 0x06000BF9 RID: 3065 RVA: 0x000250C0 File Offset: 0x000232C0
		public override void Update()
		{
			base.Update();
			this.timer += Engine.DeltaTime;
			if (this.bird != null)
			{
				this.bird.Position = this.Level.Camera.Position + this.birdScreenPosition;
				BirdNPC birdNPC = this.bird;
				birdNPC.Position.X = birdNPC.Position.X + (float)Math.Sin((double)this.timer) * 4f;
				BirdNPC birdNPC2 = this.bird;
				birdNPC2.Position.Y = birdNPC2.Position.Y + ((float)Math.Sin((double)(this.timer * 0.1f)) * 4f + (float)Math.Sin((double)(this.timer * 0.25f)) * 4f);
			}
			this.Level.CameraOffset = this.cameraOffset + this.cameraWaveOffset;
		}

		// Token: 0x06000BFA RID: 3066 RVA: 0x000251A0 File Offset: 0x000233A0
		public override void Render()
		{
			Camera camera = this.Level.Camera;
			Draw.Rect(camera.X - 1f, camera.Y - 1f, 322f, 322f, Color.White * this.fadeToWhite);
		}

		// Token: 0x04000740 RID: 1856
		private Player player;

		// Token: 0x04000741 RID: 1857
		private BadelineBoost boost;

		// Token: 0x04000742 RID: 1858
		private BirdNPC bird;

		// Token: 0x04000743 RID: 1859
		private float fadeToWhite;

		// Token: 0x04000744 RID: 1860
		private Vector2 birdScreenPosition;

		// Token: 0x04000745 RID: 1861
		private AscendManager.Streaks streaks;

		// Token: 0x04000746 RID: 1862
		private Vector2 cameraWaveOffset;

		// Token: 0x04000747 RID: 1863
		private Vector2 cameraOffset;

		// Token: 0x04000748 RID: 1864
		private float timer;

		// Token: 0x04000749 RID: 1865
		private Coroutine wave;

		// Token: 0x0400074A RID: 1866
		private bool hasGolden;

		// Token: 0x0400074B RID: 1867
		private bool sayDialog;
	}
}
