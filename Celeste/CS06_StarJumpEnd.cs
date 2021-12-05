using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000172 RID: 370
	public class CS06_StarJumpEnd : CutsceneEntity
	{
		// Token: 0x06000D1F RID: 3359 RVA: 0x0002C740 File Offset: 0x0002A940
		public CS06_StarJumpEnd(NPC theo, Player player, Vector2 playerStart, Vector2 cameraStart) : base(true, false)
		{
			base.Depth = 10100;
			this.theo = theo;
			this.player = player;
			this.playerStart = playerStart;
			this.cameraStart = cameraStart;
			base.Add(this.anxietySine = new SineWave(0.3f, 0f));
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x0002C7AD File Offset: 0x0002A9AD
		public override void Added(Scene scene)
		{
			this.Level = (scene as Level);
			this.bonfire = scene.Entities.FindFirst<Bonfire>();
			this.plateau = scene.Entities.FindFirst<Plateau>();
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x0002C7E0 File Offset: 0x0002A9E0
		public override void Update()
		{
			base.Update();
			if (this.waiting && this.player.Y <= (float)(this.Level.Bounds.Top + 160))
			{
				this.waiting = false;
				base.Start();
			}
			if (this.shaking)
			{
				this.Level.Shake(0.2f);
			}
			if (this.Level != null && this.Level.OnInterval(0.1f))
			{
				this.anxietyJitter = Calc.Random.Range(-0.1f, 0.1f);
			}
			Distort.Anxiety = this.anxietyFade * Math.Max(0f, 0f + this.anxietyJitter + this.anxietySine.Value * 0.6f);
			this.maddySine = Calc.Approach(this.maddySine, this.maddySineTarget, 12f * Engine.DeltaTime);
			if (this.maddySine > 0f)
			{
				this.player.Y = this.maddySineAnchorY + (float)Math.Sin((double)(this.Level.TimeActive * 2f)) * 3f * this.maddySine;
			}
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x0002C915 File Offset: 0x0002AB15
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x0002C92A File Offset: 0x0002AB2A
		private IEnumerator Cutscene(Level level)
		{
			StarJumpController starJumpController = level.Entities.FindFirst<StarJumpController>();
			if (starJumpController != null)
			{
				starJumpController.RemoveSelf();
			}
			foreach (StarJumpBlock starJumpBlock in level.Entities.FindAll<StarJumpBlock>())
			{
				starJumpBlock.Collidable = false;
			}
			int center = level.Bounds.X + 160;
			Vector2 cutsceneCenter = new Vector2((float)center, (float)(level.Bounds.Top + 150));
			NorthernLights bg = level.Background.Get<NorthernLights>();
			level.CameraOffset.Y = -30f;
			base.Add(new Coroutine(CutsceneEntity.CameraTo(cutsceneCenter + new Vector2(-160f, -70f), 1.5f, Ease.CubeOut, 0f), true));
			base.Add(new Coroutine(CutsceneEntity.CameraTo(cutsceneCenter + new Vector2(-160f, -120f), 2f, Ease.CubeInOut, 1.5f), true));
			Tween.Set(this, Tween.TweenMode.Oneshot, 3f, Ease.CubeInOut, delegate(Tween t)
			{
				bg.OffsetY = t.Eased * 32f;
			}, null);
			if (this.player.StateMachine.State == 19)
			{
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			}
			this.player.Dashes = 0;
			this.player.StateMachine.State = 11;
			this.player.DummyGravity = false;
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("fallSlow", false, false);
			this.player.Dashes = 1;
			this.player.Speed = new Vector2(0f, -80f);
			this.player.Facing = Facings.Right;
			this.player.ForceCameraUpdate = false;
			while (this.player.Speed.Length() > 0f || this.player.Position != cutsceneCenter)
			{
				this.player.Speed = Calc.Approach(this.player.Speed, Vector2.Zero, 200f * Engine.DeltaTime);
				this.player.Position = Calc.Approach(this.player.Position, cutsceneCenter, 64f * Engine.DeltaTime);
				yield return null;
			}
			this.player.Sprite.Play("spin", false, false);
			yield return 3.5f;
			this.player.Facing = Facings.Right;
			level.Add(this.badeline = new BadelineDummy(this.player.Position));
			level.Displacement.AddBurst(this.player.Position, 0.5f, 8f, 48f, 0.5f, null, null);
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			this.player.CreateSplitParticles();
			Audio.Play("event:/char/badeline/maddy_split");
			this.badeline.Sprite.Scale.X = -1f;
			Vector2 start = this.player.Position;
			Vector2 target = cutsceneCenter + new Vector2(-30f, 0f);
			this.maddySineAnchorY = cutsceneCenter.Y;
			for (float p = 0f; p <= 1f; p += 2f * Engine.DeltaTime)
			{
				yield return null;
				if (p > 1f)
				{
					p = 1f;
				}
				this.player.Position = Vector2.Lerp(start, target, Ease.CubeOut(p));
				this.badeline.Position = new Vector2((float)center + ((float)center - this.player.X), this.player.Y);
			}
			start = default(Vector2);
			target = default(Vector2);
			this.charactersSpinning = true;
			base.Add(new Coroutine(this.SpinCharacters(), true));
			this.SetMusicLayer(2);
			yield return 1f;
			yield return Textbox.Say("ch6_dreaming", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.TentaclesAppear),
				new Func<IEnumerator>(this.TentaclesGrab),
				new Func<IEnumerator>(this.FeatherMinigame),
				new Func<IEnumerator>(this.EndFeatherMinigame),
				new Func<IEnumerator>(this.StartCirclingPlayer)
			});
			Audio.Play("event:/game/06_reflection/badeline_pull_whooshdown");
			base.Add(new Coroutine(this.BadelineFlyDown(), true));
			yield return 0.7f;
			foreach (FlyFeather flyFeather in level.Entities.FindAll<FlyFeather>())
			{
				flyFeather.RemoveSelf();
			}
			foreach (StarJumpBlock starJumpBlock2 in level.Entities.FindAll<StarJumpBlock>())
			{
				starJumpBlock2.RemoveSelf();
			}
			foreach (JumpThru jumpThru in level.Entities.FindAll<JumpThru>())
			{
				jumpThru.RemoveSelf();
			}
			level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
			level.CameraOffset.Y = 0f;
			this.player.Sprite.Play("tentacle_pull", false, false);
			this.player.Speed.Y = 160f;
			FallEffects.Show(true);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 3f)
			{
				Player player = this.player;
				player.Speed.Y = player.Speed.Y + Engine.DeltaTime * 100f;
				if (this.player.X < (float)(level.Bounds.X + 32))
				{
					this.player.X = (float)(level.Bounds.X + 32);
				}
				if (this.player.X > (float)(level.Bounds.Right - 32))
				{
					this.player.X = (float)(level.Bounds.Right - 32);
				}
				if (p > 0.7f)
				{
					Level level2 = level;
					level2.CameraOffset.Y = level2.CameraOffset.Y - 100f * Engine.DeltaTime;
				}
				foreach (ReflectionTentacles reflectionTentacles in this.tentacles)
				{
					reflectionTentacles.Nodes[0] = new Vector2((float)level.Bounds.Center.X, this.player.Y + 300f);
					reflectionTentacles.Nodes[1] = new Vector2((float)level.Bounds.Center.X, this.player.Y + 600f);
				}
				FallEffects.SpeedMultiplier += Engine.DeltaTime * 0.75f;
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
				yield return null;
			}
			Audio.Play("event:/game/06_reflection/badeline_pull_impact");
			FallEffects.Show(false);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			level.Flash(Color.White, false);
			level.Session.Dreaming = false;
			level.CameraOffset.Y = 0f;
			level.Camera.Position = this.cameraStart;
			this.SetBloom(0f);
			this.bonfire.SetMode(Bonfire.Mode.Smoking);
			this.plateau.Depth = this.player.Depth + 10;
			this.plateau.Remove(this.plateau.Occluder);
			this.player.Position = this.playerStart + new Vector2(0f, 8f);
			this.player.Speed = Vector2.Zero;
			this.player.Sprite.Play("tentacle_dangling", false, false);
			this.player.Facing = Facings.Left;
			NPC npc = this.theo;
			npc.Position.X = npc.Position.X - 24f;
			this.theo.Sprite.Play("alert", false, false);
			foreach (ReflectionTentacles reflectionTentacles2 in this.tentacles)
			{
				reflectionTentacles2.Index = 0;
				reflectionTentacles2.Nodes[0] = new Vector2((float)level.Bounds.Center.X, this.player.Y + 32f);
				reflectionTentacles2.Nodes[1] = new Vector2((float)level.Bounds.Center.X, this.player.Y + 400f);
				reflectionTentacles2.SnapTentacles();
			}
			this.shaking = true;
			base.Add(this.shakingLoopSfx = new SoundSource());
			this.shakingLoopSfx.Play("event:/game/06_reflection/badeline_pull_rumble_loop", null, 0f);
			yield return Textbox.Say("ch6_theo_watchout", new Func<IEnumerator>[0]);
			Audio.Play("event:/game/06_reflection/badeline_pull_cliffbreak");
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Long);
			this.shakingLoopSfx.Stop(true);
			this.shaking = false;
			int num = 0;
			while ((float)num < this.plateau.Width)
			{
				level.Add(Engine.Pooler.Create<Debris>().Init(this.plateau.Position + new Vector2((float)num + Calc.Random.NextFloat(8f), Calc.Random.NextFloat(8f)), '3', true).BlastFrom(this.plateau.Center + new Vector2(0f, 8f)));
				level.Add(Engine.Pooler.Create<Debris>().Init(this.plateau.Position + new Vector2((float)num + Calc.Random.NextFloat(8f), Calc.Random.NextFloat(8f)), '3', true).BlastFrom(this.plateau.Center + new Vector2(0f, 8f)));
				num += 8;
			}
			this.plateau.RemoveSelf();
			this.bonfire.RemoveSelf();
			level.Shake(0.3f);
			this.player.Speed.Y = 160f;
			this.player.Sprite.Play("tentacle_pull", false, false);
			this.player.ForceCameraUpdate = false;
			FadeWipe wipe = new FadeWipe(level, false, delegate()
			{
				this.EndCutscene(level, true);
			});
			wipe.Duration = 3f;
			target = level.Camera.Position;
			start = level.Camera.Position + new Vector2(0f, 400f);
			while (wipe.Percent < 1f)
			{
				level.Camera.Position = Vector2.Lerp(target, start, Ease.CubeIn(wipe.Percent));
				Player player2 = this.player;
				player2.Speed.Y = player2.Speed.Y + 400f * Engine.DeltaTime;
				foreach (ReflectionTentacles reflectionTentacles3 in this.tentacles)
				{
					reflectionTentacles3.Nodes[0] = new Vector2((float)level.Bounds.Center.X, this.player.Y + 300f);
					reflectionTentacles3.Nodes[1] = new Vector2((float)level.Bounds.Center.X, this.player.Y + 600f);
				}
				yield return null;
			}
			wipe = null;
			target = default(Vector2);
			start = default(Vector2);
			yield break;
		}

		// Token: 0x06000D24 RID: 3364 RVA: 0x0002C940 File Offset: 0x0002AB40
		private void SetMusicLayer(int index)
		{
			for (int i = 1; i <= 3; i++)
			{
				this.Level.Session.Audio.Music.Layer(i, index == i);
			}
			this.Level.Session.Audio.Apply(false);
		}

		// Token: 0x06000D25 RID: 3365 RVA: 0x0002C98F File Offset: 0x0002AB8F
		private IEnumerator TentaclesAppear()
		{
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			if (this.tentacleIndex == 0)
			{
				Audio.Play("event:/game/06_reflection/badeline_freakout_1");
			}
			else if (this.tentacleIndex == 1)
			{
				Audio.Play("event:/game/06_reflection/badeline_freakout_2");
			}
			else if (this.tentacleIndex == 2)
			{
				Audio.Play("event:/game/06_reflection/badeline_freakout_3");
			}
			else
			{
				Audio.Play("event:/game/06_reflection/badeline_freakout_4");
			}
			if (!this.hidingNorthingLights)
			{
				base.Add(new Coroutine(this.NothernLightsDown(), true));
				this.hidingNorthingLights = true;
			}
			this.Level.Shake(0.3f);
			this.anxietyFade += 0.1f;
			if (this.tentacleIndex == 0)
			{
				this.SetMusicLayer(3);
			}
			int num = 400;
			int num2 = 140;
			List<Vector2> list = new List<Vector2>();
			list.Add(new Vector2(this.Level.Camera.X + 160f, this.Level.Camera.Y + (float)num));
			list.Add(new Vector2(this.Level.Camera.X + 160f, this.Level.Camera.Y + (float)num + 200f));
			ReflectionTentacles reflectionTentacles = new ReflectionTentacles();
			reflectionTentacles.Create(0f, 0, this.tentacles.Count, list);
			reflectionTentacles.Nodes[0] = new Vector2(reflectionTentacles.Nodes[0].X, this.Level.Camera.Y + (float)num2);
			this.Level.Add(reflectionTentacles);
			this.tentacles.Add(reflectionTentacles);
			this.charactersSpinning = false;
			this.tentacleIndex++;
			this.badeline.Sprite.Play("angry", false, false);
			this.maddySineTarget = 1f;
			yield return null;
			yield break;
		}

		// Token: 0x06000D26 RID: 3366 RVA: 0x0002C99E File Offset: 0x0002AB9E
		private IEnumerator TentaclesGrab()
		{
			this.maddySineTarget = 0f;
			Audio.Play("event:/game/06_reflection/badeline_freakout_5");
			this.player.Sprite.Play("tentacle_grab", false, false);
			yield return 0.1f;
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
			this.Level.Shake(0.3f);
			this.rumbler = new BreathingRumbler();
			this.Level.Add(this.rumbler);
			yield break;
		}

		// Token: 0x06000D27 RID: 3367 RVA: 0x0002C9AD File Offset: 0x0002ABAD
		private IEnumerator StartCirclingPlayer()
		{
			base.Add(new Coroutine(this.BadelineCirclePlayer(), true));
			Vector2 from = this.player.Position;
			Vector2 to = new Vector2((float)this.Level.Bounds.Center.X, this.player.Y);
			Tween.Set(this, Tween.TweenMode.Oneshot, 0.5f, Ease.CubeOut, delegate(Tween t)
			{
				this.player.Position = Vector2.Lerp(from, to, t.Eased);
			}, null);
			yield return null;
			yield break;
		}

		// Token: 0x06000D28 RID: 3368 RVA: 0x0002C9BC File Offset: 0x0002ABBC
		private IEnumerator EndCirclingPlayer()
		{
			this.baddyCircling = false;
			yield return null;
			yield break;
		}

		// Token: 0x06000D29 RID: 3369 RVA: 0x0002C9CB File Offset: 0x0002ABCB
		private IEnumerator BadelineCirclePlayer()
		{
			float offset = 0f;
			float dist = (this.badeline.Position - this.player.Position).Length();
			this.baddyCircling = true;
			while (this.baddyCircling)
			{
				offset -= Engine.DeltaTime * 4f;
				dist = Calc.Approach(dist, 24f, Engine.DeltaTime * 32f);
				this.badeline.Position = this.player.Position + Calc.AngleToVector(offset, dist);
				int num = Math.Sign(this.player.X - this.badeline.X);
				if (num != 0)
				{
					this.badeline.Sprite.Scale.X = (float)num;
				}
				if (this.Level.OnInterval(0.1f))
				{
					TrailManager.Add(this.badeline, Player.NormalHairColor, 1f, false, false);
				}
				yield return null;
			}
			this.badeline.Sprite.Scale.X = -1f;
			yield return this.badeline.FloatTo(this.player.Position + new Vector2(40f, -16f), new int?(-1), false, false, false);
			yield break;
		}

		// Token: 0x06000D2A RID: 3370 RVA: 0x0002C9DA File Offset: 0x0002ABDA
		private IEnumerator FeatherMinigame()
		{
			this.breathing = new BreathingMinigame(false, this.rumbler);
			this.Level.Add(this.breathing);
			while (!this.breathing.Pausing)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000D2B RID: 3371 RVA: 0x0002C9E9 File Offset: 0x0002ABE9
		private IEnumerator EndFeatherMinigame()
		{
			this.baddyCircling = false;
			this.breathing.Pausing = false;
			while (!this.breathing.Completed)
			{
				yield return null;
			}
			this.breathing = null;
			yield break;
		}

		// Token: 0x06000D2C RID: 3372 RVA: 0x0002C9F8 File Offset: 0x0002ABF8
		private IEnumerator BadelineFlyDown()
		{
			this.badeline.Sprite.Play("fallFast", false, false);
			this.badeline.FloatSpeed = 600f;
			this.badeline.FloatAccel = 1200f;
			yield return this.badeline.FloatTo(new Vector2(this.badeline.X, this.Level.Camera.Y + 200f), null, true, true, false);
			this.badeline.RemoveSelf();
			yield break;
		}

		// Token: 0x06000D2D RID: 3373 RVA: 0x0002CA07 File Offset: 0x0002AC07
		private IEnumerator NothernLightsDown()
		{
			NorthernLights bg = this.Level.Background.Get<NorthernLights>();
			if (bg != null)
			{
				while (bg.NorthernLightsAlpha > 0f)
				{
					bg.NorthernLightsAlpha -= Engine.DeltaTime * 0.5f;
					yield return null;
				}
			}
			yield break;
		}

		// Token: 0x06000D2E RID: 3374 RVA: 0x0002CA16 File Offset: 0x0002AC16
		private IEnumerator SpinCharacters()
		{
			Vector2 maddyStart = this.player.Position;
			Vector2 baddyStart = this.badeline.Position;
			Vector2 center = (maddyStart + baddyStart) / 2f;
			float dist = Math.Abs(maddyStart.X - center.X);
			float timer = 1.5707964f;
			this.player.Sprite.Play("spin", false, false);
			this.badeline.Sprite.Play("spin", false, false);
			this.badeline.Sprite.Scale.X = 1f;
			while (this.charactersSpinning)
			{
				int num = (int)(timer / 6.2831855f * 14f + 10f);
				this.player.Sprite.SetAnimationFrame(num);
				this.badeline.Sprite.SetAnimationFrame(num + 7);
				float num2 = (float)Math.Sin((double)timer);
				float num3 = (float)Math.Cos((double)timer);
				this.player.Position = center - new Vector2(num2 * dist, num3 * 8f);
				this.badeline.Position = center + new Vector2(num2 * dist, num3 * 8f);
				timer += Engine.DeltaTime * 2f;
				yield return null;
			}
			this.player.Facing = Facings.Right;
			this.player.Sprite.Play("fallSlow", false, false);
			this.badeline.Sprite.Scale.X = -1f;
			this.badeline.Sprite.Play("angry", false, false);
			this.badeline.AutoAnimator.Enabled = false;
			Vector2 maddyFrom = this.player.Position;
			Vector2 baddyFrom = this.badeline.Position;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 3f)
			{
				this.player.Position = Vector2.Lerp(maddyFrom, maddyStart, Ease.CubeOut(p));
				this.badeline.Position = Vector2.Lerp(baddyFrom, baddyStart, Ease.CubeOut(p));
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000D2F RID: 3375 RVA: 0x0002CA28 File Offset: 0x0002AC28
		public override void OnEnd(Level level)
		{
			if (this.rumbler != null)
			{
				this.rumbler.RemoveSelf();
				this.rumbler = null;
			}
			if (this.breathing != null)
			{
				this.breathing.RemoveSelf();
			}
			this.SetBloom(0f);
			level.Session.Audio.Music.Event = null;
			level.Session.Audio.Apply(false);
			level.Remove(this.player);
			level.UnloadLevel();
			level.EndCutscene();
			level.Session.SetFlag("plateau_2", true);
			level.SnapColorGrade(AreaData.Get(level).ColorGrade);
			level.Session.Dreaming = false;
			level.Session.FirstLevel = false;
			if (this.WasSkipped)
			{
				level.OnEndOfFrame += delegate()
				{
					level.Session.Level = "00";
					level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float)level.Bounds.Left, (float)level.Bounds.Bottom)));
					level.LoadLevel(Player.IntroTypes.None, false);
					FallEffects.Show(false);
					level.Session.Audio.Music.Event = "event:/music/lvl6/main";
					level.Session.Audio.Apply(false);
				};
				return;
			}
			Engine.Scene = new OverworldReflectionsFall(level, delegate()
			{
				Audio.SetAmbience(null, true);
				level.Session.Level = "04";
				level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float)level.Bounds.Center.X, (float)level.Bounds.Top)));
				level.LoadLevel(Player.IntroTypes.Fall, false);
				level.Add(new BackgroundFadeIn(Color.Black, 2f, 30f));
				level.Entities.UpdateLists();
				foreach (Entity entity in level.Tracker.GetEntities<CrystalStaticSpinner>())
				{
					((CrystalStaticSpinner)entity).ForceInstantiate();
				}
			});
		}

		// Token: 0x06000D30 RID: 3376 RVA: 0x0002CB61 File Offset: 0x0002AD61
		private void SetBloom(float add)
		{
			this.Level.Session.BloomBaseAdd = add;
			this.Level.Bloom.Base = AreaData.Get(this.Level).BloomBase + add;
		}

		// Token: 0x0400085A RID: 2138
		public const string Flag = "plateau_2";

		// Token: 0x0400085B RID: 2139
		private bool waiting = true;

		// Token: 0x0400085C RID: 2140
		private bool shaking;

		// Token: 0x0400085D RID: 2141
		private NPC theo;

		// Token: 0x0400085E RID: 2142
		private Player player;

		// Token: 0x0400085F RID: 2143
		private Bonfire bonfire;

		// Token: 0x04000860 RID: 2144
		private BadelineDummy badeline;

		// Token: 0x04000861 RID: 2145
		private Plateau plateau;

		// Token: 0x04000862 RID: 2146
		private BreathingMinigame breathing;

		// Token: 0x04000863 RID: 2147
		private List<ReflectionTentacles> tentacles = new List<ReflectionTentacles>();

		// Token: 0x04000864 RID: 2148
		private Vector2 playerStart;

		// Token: 0x04000865 RID: 2149
		private Vector2 cameraStart;

		// Token: 0x04000866 RID: 2150
		private float anxietyFade;

		// Token: 0x04000867 RID: 2151
		private SineWave anxietySine;

		// Token: 0x04000868 RID: 2152
		private float anxietyJitter;

		// Token: 0x04000869 RID: 2153
		private bool hidingNorthingLights;

		// Token: 0x0400086A RID: 2154
		private bool charactersSpinning;

		// Token: 0x0400086B RID: 2155
		private float maddySine;

		// Token: 0x0400086C RID: 2156
		private float maddySineTarget;

		// Token: 0x0400086D RID: 2157
		private float maddySineAnchorY;

		// Token: 0x0400086E RID: 2158
		private SoundSource shakingLoopSfx;

		// Token: 0x0400086F RID: 2159
		private bool baddyCircling;

		// Token: 0x04000870 RID: 2160
		private BreathingRumbler rumbler;

		// Token: 0x04000871 RID: 2161
		private int tentacleIndex;
	}
}
