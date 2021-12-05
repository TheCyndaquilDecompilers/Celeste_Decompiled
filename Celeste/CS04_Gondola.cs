using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000264 RID: 612
	public class CS04_Gondola : CutsceneEntity
	{
		// Token: 0x0600130C RID: 4876 RVA: 0x00067790 File Offset: 0x00065990
		public CS04_Gondola(NPC theo, Gondola gondola, Player player) : base(false, true)
		{
			this.theo = theo;
			this.gondola = gondola;
			this.player = player;
		}

		// Token: 0x0600130D RID: 4877 RVA: 0x000677BC File Offset: 0x000659BC
		public override void OnBegin(Level level)
		{
			level.RegisterAreaComplete();
			foreach (Backdrop backdrop in level.Foreground.Backdrops)
			{
				if (backdrop is WindSnowFG)
				{
					this.windSnowFg = (backdrop as WindSnowFG);
				}
			}
			base.Add(this.moveLoopSfx = new SoundSource());
			base.Add(this.haltLoopSfx = new SoundSource());
			base.Add(new Coroutine(this.Cutscene(), true));
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x00067864 File Offset: 0x00065A64
		private IEnumerator Cutscene()
		{
			this.player.StateMachine.State = 11;
			yield return this.player.DummyWalkToExact((int)this.gondola.X + 16, false, 1f, false);
			while (!this.player.OnGround(1))
			{
				yield return null;
			}
			Audio.SetMusic("event:/music/lvl1/theo", true, true);
			yield return Textbox.Say("CH4_GONDOLA", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.EnterTheo),
				new Func<IEnumerator>(this.CheckOnTheo),
				new Func<IEnumerator>(this.GetUpTheo),
				new Func<IEnumerator>(this.LookAtLever),
				new Func<IEnumerator>(this.PullLever),
				new Func<IEnumerator>(this.WaitABit),
				new Func<IEnumerator>(this.WaitForCenter),
				new Func<IEnumerator>(this.SelfieThenStallsOut),
				new Func<IEnumerator>(this.MovePlayerLeft),
				new Func<IEnumerator>(this.SnapLeverOff),
				new Func<IEnumerator>(this.DarknessAppears),
				new Func<IEnumerator>(this.DarknessConsumes),
				new Func<IEnumerator>(this.CantBreath),
				new Func<IEnumerator>(this.StartBreathing),
				new Func<IEnumerator>(this.Ascend),
				new Func<IEnumerator>(this.WaitABit),
				new Func<IEnumerator>(this.TheoTakesOutPhone),
				new Func<IEnumerator>(this.FaceTheo)
			});
			yield return this.ShowPhoto();
			base.EndCutscene(this.Level, true);
			yield break;
		}

		// Token: 0x0600130F RID: 4879 RVA: 0x00067874 File Offset: 0x00065A74
		public override void OnEnd(Level level)
		{
			if (this.rumbler != null)
			{
				this.rumbler.RemoveSelf();
				this.rumbler = null;
			}
			level.CompleteArea(true, false, false);
			if (!this.WasSkipped)
			{
				SpotlightWipe.Modifier = 120f;
				SpotlightWipe.FocusPoint = new Vector2(320f, 180f) / 2f;
			}
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x000678D5 File Offset: 0x00065AD5
		private IEnumerator EnterTheo()
		{
			this.player.Facing = Facings.Left;
			yield return 0.2f;
			yield return this.PanCamera(new Vector2((float)this.Level.Bounds.Left, this.theo.Y - 90f), 1f, null);
			this.theo.Visible = true;
			float theoStartX = this.theo.X;
			yield return this.theo.MoveTo(new Vector2(theoStartX + 35f, this.theo.Y), false, null, false);
			yield return 0.6f;
			yield return this.theo.MoveTo(new Vector2(theoStartX + 60f, this.theo.Y), false, null, false);
			Audio.Play("event:/game/04_cliffside/gondola_theo_fall", this.theo.Position);
			this.theo.Sprite.Play("idleEdge", false, false);
			yield return 1f;
			this.theo.Sprite.Play("falling", false, false);
			this.theo.X += 4f;
			this.theo.Depth = -10010;
			float speed = 80f;
			while (this.theo.Y < this.player.Y)
			{
				this.theo.Y += speed * Engine.DeltaTime;
				speed += 120f * Engine.DeltaTime;
				yield return null;
			}
			this.Level.DirectionalShake(new Vector2(0f, 1f), 0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			this.theo.Y = this.player.Y;
			this.theo.Sprite.Play("hitGround", false, false);
			this.theo.Sprite.Rate = 0f;
			this.theo.Depth = 1000;
			this.theo.Sprite.Scale = new Vector2(1.3f, 0.8f);
			yield return 0.5f;
			Vector2 start = this.theo.Sprite.Scale;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, null, 2f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				this.theo.Sprite.Scale.X = MathHelper.Lerp(start.X, 1f, t.Eased);
				this.theo.Sprite.Scale.Y = MathHelper.Lerp(start.Y, 1f, t.Eased);
			};
			base.Add(tween);
			yield return this.PanCamera(new Vector2((float)this.Level.Bounds.Left, this.theo.Y - 120f), 1f, null);
			yield return 0.6f;
			yield break;
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x000678E4 File Offset: 0x00065AE4
		private IEnumerator CheckOnTheo()
		{
			yield return this.player.DummyWalkTo(this.gondola.X - 18f, false, 1f, false);
			yield break;
		}

		// Token: 0x06001312 RID: 4882 RVA: 0x000678F3 File Offset: 0x00065AF3
		private IEnumerator GetUpTheo()
		{
			yield return 1.4f;
			Audio.Play("event:/game/04_cliffside/gondola_theo_recover", this.theo.Position);
			this.theo.Sprite.Rate = 1f;
			this.theo.Sprite.Play("recoverGround", false, false);
			yield return 1.6f;
			yield return this.theo.MoveTo(new Vector2(this.gondola.X - 50f, this.player.Y), false, null, false);
			yield return 0.2f;
			yield break;
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x00067902 File Offset: 0x00065B02
		private IEnumerator LookAtLever()
		{
			yield return this.theo.MoveTo(new Vector2(this.gondola.X + 7f, this.theo.Y), false, null, false);
			this.player.Facing = Facings.Right;
			this.theo.Sprite.Scale.X = -1f;
			yield break;
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x00067911 File Offset: 0x00065B11
		private IEnumerator PullLever()
		{
			base.Add(new Coroutine(this.player.DummyWalkToExact((int)this.gondola.X - 7, false, 1f, false), true));
			this.theo.Sprite.Scale.X = -1f;
			yield return 0.2f;
			Audio.Play("event:/game/04_cliffside/gondola_theo_lever_start", this.theo.Position);
			this.theo.Sprite.Play("pullVent", false, false);
			yield return 1f;
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			this.gondola.Lever.Play("pulled", false, false);
			this.theo.Sprite.Play("fallVent", false, false);
			yield return 0.6f;
			this.Level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
			yield return 0.5f;
			yield return this.PanCamera(this.gondola.Position + new Vector2(-160f, -120f), 1f, null);
			yield return 0.5f;
			this.Level.Background.Backdrops.Add(this.loopingCloud = new Parallax(GFX.Game["bgs/04/bgCloudLoop"]));
			this.Level.Background.Backdrops.Add(this.bottomCloud = new Parallax(GFX.Game["bgs/04/bgCloud"]));
			this.loopingCloud.LoopX = (this.bottomCloud.LoopX = true);
			this.loopingCloud.LoopY = (this.bottomCloud.LoopY = false);
			this.loopingCloud.Position.Y = this.Level.Camera.Top - (float)this.loopingCloud.Texture.Height - (float)this.bottomCloud.Texture.Height;
			this.bottomCloud.Position.Y = this.Level.Camera.Top - (float)this.bottomCloud.Texture.Height;
			this.LoopCloudsAt = this.bottomCloud.Position.Y;
			this.AutoSnapCharacters = true;
			this.theoXOffset = this.theo.X - this.gondola.X;
			this.playerXOffset = this.player.X - this.gondola.X;
			this.player.StateMachine.State = 17;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, null, 16f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				if (Audio.CurrentMusic == "event:/music/lvl1/theo")
				{
					Audio.SetMusicParam("fade", 1f - t.Eased);
				}
			};
			base.Add(tween);
			SoundSource soundSource = new SoundSource();
			soundSource.Position = this.gondola.LeftCliffside.Position;
			soundSource.Play("event:/game/04_cliffside/gondola_cliffmechanism_start", null, 0f);
			base.Add(soundSource);
			this.moveLoopSfx.Play("event:/game/04_cliffside/gondola_movement_loop", null, 0f);
			this.Level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.FullSecond);
			this.gondolaSpeed = 32f;
			this.gondola.RotationSpeed = 1f;
			this.gondolaState = CS04_Gondola.GondolaStates.MovingToCenter;
			yield return 1f;
			yield return this.MoveTheoOnGondola(12f, false);
			yield return 0.2f;
			this.theo.Sprite.Scale.X = -1f;
			yield break;
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x00067920 File Offset: 0x00065B20
		private IEnumerator WaitABit()
		{
			yield return 1f;
			yield break;
		}

		// Token: 0x06001316 RID: 4886 RVA: 0x00067928 File Offset: 0x00065B28
		private IEnumerator WaitForCenter()
		{
			while (this.gondolaState != CS04_Gondola.GondolaStates.InCenter)
			{
				yield return null;
			}
			this.theo.Sprite.Scale.X = 1f;
			yield return 1f;
			yield return this.MovePlayerOnGondola(-20f);
			yield return 0.5f;
			yield break;
		}

		// Token: 0x06001317 RID: 4887 RVA: 0x00067937 File Offset: 0x00065B37
		private IEnumerator SelfieThenStallsOut()
		{
			Audio.SetMusic("event:/music/lvl4/minigame", true, true);
			base.Add(new Coroutine(this.Level.ZoomTo(new Vector2(160f, 110f), 2f, 0.5f), true));
			yield return 0.3f;
			this.theo.Sprite.Scale.X = 1f;
			yield return 0.2f;
			base.Add(new Coroutine(this.MovePlayerOnGondola(this.theoXOffset - 8f), true));
			yield return 0.4f;
			Audio.Play("event:/game/04_cliffside/gondola_theoselfie_halt", this.theo.Position);
			this.theo.Sprite.Play("holdOutPhone", false, false);
			yield return 1.5f;
			this.theoXOffset += 4f;
			this.playerXOffset += 4f;
			this.gondola.RotationSpeed = -1f;
			this.gondolaState = CS04_Gondola.GondolaStates.Stopped;
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
			this.theo.Sprite.Play("takeSelfieImmediate", false, false);
			base.Add(new Coroutine(this.PanCamera(this.gondola.Position + (this.gondola.Destination - this.gondola.Position).SafeNormalize() * 32f + new Vector2(-160f, -120f), 0.3f, Ease.CubeOut), true));
			yield return 0.5f;
			this.Level.Flash(Color.White, false);
			this.Level.Add(this.evil = new BadelineDummy(Vector2.Zero));
			this.evil.Appear(this.Level, false);
			this.evil.Floatness = 0f;
			this.evil.Depth = -1000000;
			this.moveLoopSfx.Stop(true);
			this.haltLoopSfx.Play("event:/game/04_cliffside/gondola_halted_loop", null, 0f);
			this.gondolaState = CS04_Gondola.GondolaStates.Shaking;
			yield return this.PanCamera(this.gondola.Position + new Vector2(-160f, -120f), 1f, null);
			yield return 1f;
			yield break;
		}

		// Token: 0x06001318 RID: 4888 RVA: 0x00067946 File Offset: 0x00065B46
		private IEnumerator MovePlayerLeft()
		{
			yield return this.MovePlayerOnGondola(-20f);
			this.theo.Sprite.Scale.X = -1f;
			yield return 0.5f;
			yield return this.MovePlayerOnGondola(20f);
			yield return 0.5f;
			yield return this.MovePlayerOnGondola(-10f);
			yield return 0.5f;
			this.player.Facing = Facings.Right;
			yield break;
		}

		// Token: 0x06001319 RID: 4889 RVA: 0x00067955 File Offset: 0x00065B55
		private IEnumerator SnapLeverOff()
		{
			yield return this.MoveTheoOnGondola(7f, true);
			Audio.Play("event:/game/04_cliffside/gondola_theo_lever_fail", this.theo.Position);
			this.theo.Sprite.Play("pullVent", false, false);
			yield return 1f;
			this.theo.Sprite.Play("fallVent", false, false);
			yield return 1f;
			this.gondola.BreakLever();
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			this.Level.Shake(0.3f);
			yield return 2.5f;
			yield break;
		}

		// Token: 0x0600131A RID: 4890 RVA: 0x00067964 File Offset: 0x00065B64
		private IEnumerator DarknessAppears()
		{
			Audio.SetMusicParam("calm", 0f);
			yield return 0.25f;
			this.player.Sprite.Play("tired", false, false);
			yield return 0.25f;
			this.evil.Vanish();
			this.evil = null;
			yield return 0.3f;
			this.Level.NextColorGrade("panicattack", 1f);
			this.Level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			this.BurstTentacles(3, 90f, 200f);
			Audio.Play("event:/game/04_cliffside/gondola_scaryhair_01", this.gondola.Position);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 2f)
			{
				yield return null;
				this.Level.Background.Fade = p;
				this.anxiety = p;
				if (this.windSnowFg != null)
				{
					this.windSnowFg.Alpha = 1f - p;
				}
			}
			yield return 0.25f;
			yield break;
		}

		// Token: 0x0600131B RID: 4891 RVA: 0x00067973 File Offset: 0x00065B73
		private IEnumerator DarknessConsumes()
		{
			this.Level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			Audio.Play("event:/game/04_cliffside/gondola_scaryhair_02", this.gondola.Position);
			this.BurstTentacles(2, 60f, 200f);
			yield return this.MoveTheoOnGondola(0f, true);
			this.theo.Sprite.Play("comfortStart", false, false);
			yield break;
		}

		// Token: 0x0600131C RID: 4892 RVA: 0x00067982 File Offset: 0x00065B82
		private IEnumerator CantBreath()
		{
			this.Level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			Audio.Play("event:/game/04_cliffside/gondola_scaryhair_03", this.gondola.Position);
			this.BurstTentacles(1, 30f, 200f);
			this.BurstTentacles(0, 0f, 100f);
			this.rumbler = new BreathingRumbler();
			base.Scene.Add(this.rumbler);
			yield return null;
			yield break;
		}

		// Token: 0x0600131D RID: 4893 RVA: 0x00067991 File Offset: 0x00065B91
		private IEnumerator StartBreathing()
		{
			BreathingMinigame breathing = new BreathingMinigame(true, this.rumbler);
			base.Scene.Add(breathing);
			while (!breathing.Completed)
			{
				yield return null;
			}
			foreach (ReflectionTentacles reflectionTentacles in this.tentacles)
			{
				reflectionTentacles.RemoveSelf();
			}
			this.anxiety = 0f;
			this.Level.Background.Fade = 0f;
			this.Level.SnapColorGrade(null);
			this.gondola.CancelPullSides();
			this.Level.ResetZoom();
			yield return 0.5f;
			Audio.Play("event:/game/04_cliffside/gondola_restart", this.gondola.Position);
			yield return 1f;
			this.moveLoopSfx.Play("event:/game/04_cliffside/gondola_movement_loop", null, 0f);
			this.haltLoopSfx.Stop(true);
			this.Level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
			this.gondolaState = CS04_Gondola.GondolaStates.InCenter;
			this.gondola.RotationSpeed = 0.5f;
			yield return 1.2f;
			yield break;
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x000679A0 File Offset: 0x00065BA0
		private IEnumerator Ascend()
		{
			this.gondolaState = CS04_Gondola.GondolaStates.MovingToEnd;
			while (this.gondolaState != CS04_Gondola.GondolaStates.Stopped)
			{
				yield return null;
			}
			this.Level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
			this.moveLoopSfx.Stop(true);
			Audio.Play("event:/game/04_cliffside/gondola_finish", this.gondola.Position);
			this.gondola.RotationSpeed = 0.5f;
			yield return 0.1f;
			while (this.gondola.Rotation > 0f)
			{
				yield return null;
			}
			this.gondola.Rotation = (this.gondola.RotationSpeed = 0f);
			this.Level.Shake(0.3f);
			this.AutoSnapCharacters = false;
			this.player.StateMachine.State = 11;
			this.player.Position = this.player.Position.Floor();
			while (this.player.CollideCheck<Solid>())
			{
				Player player = this.player;
				float y = player.Y;
				player.Y = y - 1f;
			}
			this.theo.Position.Y = this.player.Position.Y;
			this.theo.Sprite.Play("comfortRecover", false, false);
			this.theo.Sprite.Scale.X = 1f;
			yield return this.player.DummyWalkTo(this.gondola.X + 80f, false, 1f, false);
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("tired", false, false);
			yield return this.theo.MoveTo(new Vector2(this.gondola.X + 64f, this.theo.Y), false, null, false);
			yield return 0.5f;
			yield break;
		}

		// Token: 0x0600131F RID: 4895 RVA: 0x000679AF File Offset: 0x00065BAF
		private IEnumerator TheoTakesOutPhone()
		{
			this.player.Facing = Facings.Right;
			yield return 0.25f;
			this.theo.Sprite.Play("usePhone", false, false);
			yield return 2f;
			yield break;
		}

		// Token: 0x06001320 RID: 4896 RVA: 0x000679BE File Offset: 0x00065BBE
		private IEnumerator FaceTheo()
		{
			this.player.DummyAutoAnimate = true;
			yield return 0.2f;
			this.player.Facing = Facings.Left;
			yield return 0.2f;
			yield break;
		}

		// Token: 0x06001321 RID: 4897 RVA: 0x000679CD File Offset: 0x00065BCD
		private IEnumerator ShowPhoto()
		{
			this.theo.Sprite.Scale.X = -1f;
			yield return 0.25f;
			yield return this.player.DummyWalkTo(this.theo.X + 5f, false, 1f, false);
			yield return 1f;
			Selfie selfie = new Selfie(base.SceneAs<Level>());
			base.Scene.Add(selfie);
			yield return selfie.OpenRoutine("selfieGondola");
			yield return selfie.WaitForInput();
			yield break;
		}

		// Token: 0x06001322 RID: 4898 RVA: 0x000679DC File Offset: 0x00065BDC
		public override void Update()
		{
			base.Update();
			if (this.anxietyRumble > 0f)
			{
				Input.RumbleSpecific(this.anxietyRumble, 0.1f);
			}
			if (base.Scene.OnInterval(0.05f))
			{
				this.anxietyStutter = Calc.Random.NextFloat(0.1f);
			}
			Distort.AnxietyOrigin = new Vector2(0.5f, 0.5f);
			Distort.Anxiety = this.anxiety * 0.2f + this.anxietyStutter * this.anxiety;
			if (this.moveLoopSfx != null && this.gondola != null)
			{
				this.moveLoopSfx.Position = this.gondola.Position;
			}
			if (this.haltLoopSfx != null && this.gondola != null)
			{
				this.haltLoopSfx.Position = this.gondola.Position;
			}
			if (this.gondolaState == CS04_Gondola.GondolaStates.MovingToCenter)
			{
				this.MoveGondolaTowards(0.5f);
				if (this.gondolaPercent >= 0.5f)
				{
					this.gondolaState = CS04_Gondola.GondolaStates.InCenter;
				}
			}
			else if (this.gondolaState == CS04_Gondola.GondolaStates.InCenter)
			{
				Vector2 vector = (this.gondola.Destination - this.gondola.Position).SafeNormalize() * this.gondolaSpeed;
				Parallax parallax = this.loopingCloud;
				parallax.CameraOffset.X = parallax.CameraOffset.X + vector.X * Engine.DeltaTime;
				Parallax parallax2 = this.loopingCloud;
				parallax2.CameraOffset.Y = parallax2.CameraOffset.Y + vector.Y * Engine.DeltaTime;
				this.windSnowFg.CameraOffset = this.loopingCloud.CameraOffset;
				this.loopingCloud.LoopY = true;
			}
			else if (this.gondolaState != CS04_Gondola.GondolaStates.Stopped)
			{
				if (this.gondolaState == CS04_Gondola.GondolaStates.Shaking)
				{
					this.Level.Wind.X = -400f;
					if (this.shakeTimer <= 0f && (this.gondola.Rotation == 0f || this.gondola.Rotation < -0.25f))
					{
						this.shakeTimer = 1f;
						this.gondola.RotationSpeed = 0.5f;
					}
					this.shakeTimer -= Engine.DeltaTime;
				}
				else if (this.gondolaState == CS04_Gondola.GondolaStates.MovingToEnd)
				{
					this.MoveGondolaTowards(1f);
					if (this.gondolaPercent >= 1f)
					{
						this.gondolaState = CS04_Gondola.GondolaStates.Stopped;
					}
				}
			}
			if (this.loopingCloud != null && !this.loopingCloud.LoopY && this.Level.Camera.Bottom < this.LoopCloudsAt)
			{
				this.loopingCloud.LoopY = true;
			}
			if (this.AutoSnapCharacters)
			{
				this.theo.Position = this.gondola.GetRotatedFloorPositionAt(this.theoXOffset, 52f);
				this.player.Position = this.gondola.GetRotatedFloorPositionAt(this.playerXOffset, 52f);
				if (this.evil != null)
				{
					this.evil.Position = this.gondola.GetRotatedFloorPositionAt(-24f, 20f);
				}
			}
		}

		// Token: 0x06001323 RID: 4899 RVA: 0x00067CE0 File Offset: 0x00065EE0
		private void MoveGondolaTowards(float percent)
		{
			float num = (this.gondola.Start - this.gondola.Destination).Length();
			this.gondolaSpeed = Calc.Approach(this.gondolaSpeed, 64f, 120f * Engine.DeltaTime);
			this.gondolaPercent = Calc.Approach(this.gondolaPercent, percent, this.gondolaSpeed / num * Engine.DeltaTime);
			this.gondola.Position = (this.gondola.Start + (this.gondola.Destination - this.gondola.Start) * this.gondolaPercent).Floor();
			this.Level.Camera.Position = this.gondola.Position + new Vector2(-160f, -120f);
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x00067DC7 File Offset: 0x00065FC7
		private IEnumerator PanCamera(Vector2 to, float duration, Ease.Easer ease = null)
		{
			if (ease == null)
			{
				ease = Ease.CubeInOut;
			}
			Vector2 from = this.Level.Camera.Position;
			for (float t = 0f; t < 1f; t += Engine.DeltaTime / duration)
			{
				yield return null;
				this.Level.Camera.Position = from + (to - from) * ease(Math.Min(t, 1f));
			}
			yield break;
		}

		// Token: 0x06001325 RID: 4901 RVA: 0x00067DEB File Offset: 0x00065FEB
		private IEnumerator MovePlayerOnGondola(float x)
		{
			this.player.Sprite.Play("walk", false, false);
			this.player.Facing = (Facings)Math.Sign(x - this.playerXOffset);
			while (this.playerXOffset != x)
			{
				this.playerXOffset = Calc.Approach(this.playerXOffset, x, 48f * Engine.DeltaTime);
				yield return null;
			}
			this.player.Sprite.Play("idle", false, false);
			yield break;
		}

		// Token: 0x06001326 RID: 4902 RVA: 0x00067E01 File Offset: 0x00066001
		private IEnumerator MoveTheoOnGondola(float x, bool changeFacing = true)
		{
			this.theo.Sprite.Play("walk", false, false);
			if (changeFacing)
			{
				this.theo.Sprite.Scale.X = (float)Math.Sign(x - this.theoXOffset);
			}
			while (this.theoXOffset != x)
			{
				this.theoXOffset = Calc.Approach(this.theoXOffset, x, 48f * Engine.DeltaTime);
				yield return null;
			}
			this.theo.Sprite.Play("idle", false, false);
			yield break;
		}

		// Token: 0x06001327 RID: 4903 RVA: 0x00067E20 File Offset: 0x00066020
		private void BurstTentacles(int layer, float dist, float from = 200f)
		{
			Vector2 value = this.Level.Camera.Position + new Vector2(160f, 90f);
			ReflectionTentacles reflectionTentacles = new ReflectionTentacles();
			reflectionTentacles.Create(0f, 0, layer, new List<Vector2>
			{
				value + new Vector2(-from, 0f),
				value + new Vector2(-800f, 0f)
			});
			reflectionTentacles.SnapTentacles();
			reflectionTentacles.Nodes[0] = value + new Vector2(-dist, 0f);
			ReflectionTentacles reflectionTentacles2 = new ReflectionTentacles();
			reflectionTentacles2.Create(0f, 0, layer, new List<Vector2>
			{
				value + new Vector2(from, 0f),
				value + new Vector2(800f, 0f)
			});
			reflectionTentacles2.SnapTentacles();
			reflectionTentacles2.Nodes[0] = value + new Vector2(dist, 0f);
			this.tentacles.Add(reflectionTentacles);
			this.tentacles.Add(reflectionTentacles2);
			this.Level.Add(reflectionTentacles);
			this.Level.Add(reflectionTentacles2);
		}

		// Token: 0x04000EFF RID: 3839
		private NPC theo;

		// Token: 0x04000F00 RID: 3840
		private Gondola gondola;

		// Token: 0x04000F01 RID: 3841
		private Player player;

		// Token: 0x04000F02 RID: 3842
		private BadelineDummy evil;

		// Token: 0x04000F03 RID: 3843
		private Parallax loopingCloud;

		// Token: 0x04000F04 RID: 3844
		private Parallax bottomCloud;

		// Token: 0x04000F05 RID: 3845
		private WindSnowFG windSnowFg;

		// Token: 0x04000F06 RID: 3846
		private float LoopCloudsAt;

		// Token: 0x04000F07 RID: 3847
		private List<ReflectionTentacles> tentacles = new List<ReflectionTentacles>();

		// Token: 0x04000F08 RID: 3848
		private SoundSource moveLoopSfx;

		// Token: 0x04000F09 RID: 3849
		private SoundSource haltLoopSfx;

		// Token: 0x04000F0A RID: 3850
		private float gondolaPercent;

		// Token: 0x04000F0B RID: 3851
		private bool AutoSnapCharacters;

		// Token: 0x04000F0C RID: 3852
		private float theoXOffset;

		// Token: 0x04000F0D RID: 3853
		private float playerXOffset;

		// Token: 0x04000F0E RID: 3854
		private float gondolaSpeed;

		// Token: 0x04000F0F RID: 3855
		private float shakeTimer;

		// Token: 0x04000F10 RID: 3856
		private const float gondolaMaxSpeed = 64f;

		// Token: 0x04000F11 RID: 3857
		private float anxiety;

		// Token: 0x04000F12 RID: 3858
		private float anxietyStutter;

		// Token: 0x04000F13 RID: 3859
		private float anxietyRumble;

		// Token: 0x04000F14 RID: 3860
		private BreathingRumbler rumbler;

		// Token: 0x04000F15 RID: 3861
		private CS04_Gondola.GondolaStates gondolaState;

		// Token: 0x02000585 RID: 1413
		private enum GondolaStates
		{
			// Token: 0x040026F4 RID: 9972
			Stopped,
			// Token: 0x040026F5 RID: 9973
			MovingToCenter,
			// Token: 0x040026F6 RID: 9974
			InCenter,
			// Token: 0x040026F7 RID: 9975
			Shaking,
			// Token: 0x040026F8 RID: 9976
			MovingToEnd
		}
	}
}
