using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000196 RID: 406
	public class CS06_BossEnd : CutsceneEntity
	{
		// Token: 0x06000E1C RID: 3612 RVA: 0x000328A3 File Offset: 0x00030AA3
		public CS06_BossEnd(Player player, NPC06_Badeline_Crying badeline) : base(true, false)
		{
			base.Tag = Tags.HUD;
			this.player = player;
			this.badeline = badeline;
		}

		// Token: 0x06000E1D RID: 3613 RVA: 0x000328CB File Offset: 0x00030ACB
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000E1E RID: 3614 RVA: 0x000328E0 File Offset: 0x00030AE0
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			while (!this.player.OnGround(1))
			{
				yield return null;
			}
			this.player.Facing = Facings.Right;
			yield return 1f;
			Level level2 = base.SceneAs<Level>();
			level2.Session.Audio.Music.Event = "event:/music/lvl6/badeline_acoustic";
			level2.Session.Audio.Apply(false);
			yield return Textbox.Say("ch6_boss_ending", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.StartMusic),
				new Func<IEnumerator>(this.PlayerHug),
				new Func<IEnumerator>(this.BadelineCalmDown)
			});
			yield return 0.5f;
			while ((this.fade += Engine.DeltaTime) < 1f)
			{
				yield return null;
			}
			this.picture = GFX.Portraits["hug1"];
			this.sfx = Audio.Play("event:/game/06_reflection/hug_image_1");
			yield return this.PictureFade(1f, 1f);
			yield return this.WaitForPress();
			this.sfx = Audio.Play("event:/game/06_reflection/hug_image_2");
			yield return this.PictureFade(0f, 0.5f);
			this.picture = GFX.Portraits["hug2"];
			yield return this.PictureFade(1f, 1f);
			yield return this.WaitForPress();
			this.sfx = Audio.Play("event:/game/06_reflection/hug_image_3");
			while ((this.pictureGlow += Engine.DeltaTime / 2f) < 1f)
			{
				yield return null;
			}
			yield return 0.2f;
			yield return this.PictureFade(0f, 0.5f);
			while ((this.fade -= Engine.DeltaTime * 12f) > 0f)
			{
				yield return null;
			}
			level.Session.Audio.Music.Param("levelup", 1f);
			level.Session.Audio.Apply(false);
			base.Add(new Coroutine(this.badeline.TurnWhite(1f), true));
			yield return 0.5f;
			this.player.Sprite.Play("idle", false, false);
			yield return 0.25f;
			yield return this.player.DummyWalkToExact((int)this.player.X - 8, true, 1f, false);
			base.Add(new Coroutine(this.CenterCameraOnPlayer(), true));
			yield return this.badeline.Disperse();
			(base.Scene as Level).Session.SetFlag("badeline_connection", true);
			level.Flash(Color.White, false);
			level.Session.Inventory.Dashes = 2;
			this.badeline.RemoveSelf();
			yield return 0.1f;
			level.Add(new LevelUpEffect(this.player.Position));
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			yield return 2f;
			yield return level.ZoomBack(0.5f);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000E1F RID: 3615 RVA: 0x000328F6 File Offset: 0x00030AF6
		private IEnumerator StartMusic()
		{
			Level level = base.SceneAs<Level>();
			level.Session.Audio.Music.Event = "event:/music/lvl6/badeline_acoustic";
			level.Session.Audio.Apply(false);
			yield return 0.5f;
			yield break;
		}

		// Token: 0x06000E20 RID: 3616 RVA: 0x00032905 File Offset: 0x00030B05
		private IEnumerator PlayerHug()
		{
			base.Add(new Coroutine(this.Level.ZoomTo(this.badeline.Center + new Vector2(0f, -24f) - this.Level.Camera.Position, 2f, 0.5f), true));
			yield return 0.6f;
			yield return this.player.DummyWalkToExact((int)this.badeline.X - 10, false, 1f, false);
			this.player.Facing = Facings.Right;
			yield return 0.25f;
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("hug", false, false);
			yield return 0.5f;
			yield break;
		}

		// Token: 0x06000E21 RID: 3617 RVA: 0x00032914 File Offset: 0x00030B14
		private IEnumerator BadelineCalmDown()
		{
			Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "postboss", 0f);
			this.badeline.LoopingSfx.Param("end", 1f);
			yield return 0.5f;
			this.badeline.Sprite.Play("scaredTransition", false, false);
			Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
			FinalBossStarfield bossBg = this.Level.Background.Get<FinalBossStarfield>();
			if (bossBg != null)
			{
				while (bossBg.Alpha > 0f)
				{
					bossBg.Alpha -= Engine.DeltaTime;
					yield return null;
				}
			}
			yield return 1.5f;
			yield break;
		}

		// Token: 0x06000E22 RID: 3618 RVA: 0x00032923 File Offset: 0x00030B23
		private IEnumerator CenterCameraOnPlayer()
		{
			yield return 0.5f;
			Vector2 from = this.Level.ZoomFocusPoint;
			Vector2 to = new Vector2((float)(this.Level.Bounds.Left + 580), (float)(this.Level.Bounds.Top + 124)) - this.Level.Camera.Position;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime)
			{
				this.Level.ZoomFocusPoint = from + (to - from) * Ease.SineInOut(p);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000E23 RID: 3619 RVA: 0x00032932 File Offset: 0x00030B32
		private IEnumerator PictureFade(float to, float duration = 1f)
		{
			while ((this.pictureFade = Calc.Approach(this.pictureFade, to, Engine.DeltaTime / duration)) != to)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000E24 RID: 3620 RVA: 0x0003294F File Offset: 0x00030B4F
		private IEnumerator WaitForPress()
		{
			this.waitForKeyPress = true;
			while (!Input.MenuConfirm.Pressed)
			{
				yield return null;
			}
			this.waitForKeyPress = false;
			yield break;
		}

		// Token: 0x06000E25 RID: 3621 RVA: 0x00032960 File Offset: 0x00030B60
		public override void OnEnd(Level level)
		{
			if (this.WasSkipped && this.sfx != null)
			{
				Audio.Stop(this.sfx, true);
			}
			Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "postboss", 0f);
			level.ResetZoom();
			level.Session.Inventory.Dashes = 2;
			level.Session.Audio.Music.Event = "event:/music/lvl6/badeline_acoustic";
			if (this.WasSkipped)
			{
				level.Session.Audio.Music.Param("levelup", 2f);
			}
			level.Session.Audio.Apply(false);
			if (this.WasSkipped)
			{
				level.Add(new LevelUpEffect(this.player.Position));
			}
			this.player.DummyAutoAnimate = true;
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			FinalBossStarfield finalBossStarfield = this.Level.Background.Get<FinalBossStarfield>();
			if (finalBossStarfield != null)
			{
				finalBossStarfield.Alpha = 0f;
			}
			this.badeline.RemoveSelf();
			level.Session.SetFlag("badeline_connection", true);
		}

		// Token: 0x06000E26 RID: 3622 RVA: 0x00032A94 File Offset: 0x00030C94
		public override void Update()
		{
			this.timer += Engine.DeltaTime;
			base.Update();
		}

		// Token: 0x06000E27 RID: 3623 RVA: 0x00032AB0 File Offset: 0x00030CB0
		public override void Render()
		{
			if (this.fade > 0f)
			{
				Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * Ease.CubeOut(this.fade) * 0.8f);
				if (this.picture != null && this.pictureFade > 0f)
				{
					float num = Ease.CubeOut(this.pictureFade);
					Vector2 position = new Vector2(960f, 540f);
					float scale = 1f + (1f - num) * 0.025f;
					this.picture.DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade), scale, 0f);
					if (this.pictureGlow > 0f)
					{
						GFX.Portraits["hug-light2a"].DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade * this.pictureGlow), scale);
						GFX.Portraits["hug-light2b"].DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade * this.pictureGlow), scale);
						GFX.Portraits["hug-light2c"].DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade * this.pictureGlow), scale);
						HiresRenderer.EndRender();
						HiresRenderer.BeginRender(BlendState.Additive, null);
						GFX.Portraits["hug-light2a"].DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade * this.pictureGlow), scale);
						GFX.Portraits["hug-light2b"].DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade * this.pictureGlow), scale);
						GFX.Portraits["hug-light2c"].DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade * this.pictureGlow), scale);
						HiresRenderer.EndRender();
						HiresRenderer.BeginRender(null, null);
					}
					if (this.waitForKeyPress)
					{
						GFX.Gui["textboxbutton"].DrawCentered(new Vector2(1520f, (float)(880 + ((this.timer % 1f < 0.25f) ? 6 : 0))));
					}
				}
			}
		}

		// Token: 0x04000959 RID: 2393
		public const string Flag = "badeline_connection";

		// Token: 0x0400095A RID: 2394
		private Player player;

		// Token: 0x0400095B RID: 2395
		private NPC06_Badeline_Crying badeline;

		// Token: 0x0400095C RID: 2396
		private float fade;

		// Token: 0x0400095D RID: 2397
		private float pictureFade;

		// Token: 0x0400095E RID: 2398
		private float pictureGlow;

		// Token: 0x0400095F RID: 2399
		private MTexture picture;

		// Token: 0x04000960 RID: 2400
		private bool waitForKeyPress;

		// Token: 0x04000961 RID: 2401
		private float timer;

		// Token: 0x04000962 RID: 2402
		private EventInstance sfx;
	}
}
