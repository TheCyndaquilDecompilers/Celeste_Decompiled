using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000167 RID: 359
	public class CS07_Credits : CutsceneEntity
	{
		// Token: 0x06000CB8 RID: 3256 RVA: 0x0002ABF0 File Offset: 0x00028DF0
		public CS07_Credits() : base(true, false)
		{
			MInput.Disabled = true;
			CS07_Credits.Instance = this;
			base.Tag = (Tags.Global | Tags.HUD);
			this.wasDashAssistOn = SaveData.Instance.Assists.DashAssist;
			SaveData.Instance.Assists.DashAssist = false;
		}

		// Token: 0x06000CB9 RID: 3257 RVA: 0x0002AC94 File Offset: 0x00028E94
		public override void OnBegin(Level level)
		{
			Audio.BusMuted("bus:/gameplay_sfx", new bool?(true));
			this.gotoEpilogue = level.Session.OldStats.Modes[0].Completed;
			this.gotoEpilogue = true;
			base.Add(new Coroutine(this.Routine(), true));
			base.Add(new PostUpdateHook(new Action(this.PostUpdate)));
		}

		// Token: 0x06000CBA RID: 3258 RVA: 0x0002ACFF File Offset: 0x00028EFF
		public override void Added(Scene scene)
		{
			base.Added(scene);
			(base.Scene as Level).InCredits = true;
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x0002AD19 File Offset: 0x00028F19
		private IEnumerator Routine()
		{
			this.Level.Background.Backdrops.Add(this.fillbg = new CS07_Credits.Fill());
			this.Level.Completed = true;
			SpeedrunTimerDisplay speedrunTimerDisplay = this.Level.Entities.FindFirst<SpeedrunTimerDisplay>();
			if (speedrunTimerDisplay != null)
			{
				speedrunTimerDisplay.RemoveSelf();
			}
			TotalStrawberriesDisplay totalStrawberriesDisplay = this.Level.Entities.FindFirst<TotalStrawberriesDisplay>();
			if (totalStrawberriesDisplay != null)
			{
				totalStrawberriesDisplay.RemoveSelf();
			}
			GameplayStats gameplayStats = this.Level.Entities.FindFirst<GameplayStats>();
			if (gameplayStats != null)
			{
				gameplayStats.RemoveSelf();
			}
			yield return null;
			this.Level.Wipe.Cancel();
			yield return 0.5f;
			float alignment = 1f;
			if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
			{
				alignment = 0f;
			}
			this.credits = new Credits(alignment, 0.6f, false, true);
			this.credits.AllowInput = false;
			yield return 3f;
			this.SetBgFade(0f);
			base.Add(new Coroutine(this.FadeTo(0f), true));
			yield return this.SetupLevel();
			yield return this.WaitForPlayer();
			yield return this.FadeTo(1f);
			yield return 1f;
			this.SetBgFade(0.1f);
			yield return this.NextLevel("credits-dashes");
			yield return this.SetupLevel();
			base.Add(new Coroutine(this.FadeTo(0f), true));
			yield return this.WaitForPlayer();
			yield return this.FadeTo(1f);
			yield return 1f;
			this.SetBgFade(0.2f);
			yield return this.NextLevel("credits-walking");
			yield return this.SetupLevel();
			base.Add(new Coroutine(this.FadeTo(0f), true));
			yield return 5.8f;
			this.badelineAutoFloat = false;
			yield return 0.5f;
			this.badeline.Sprite.Scale.X = 1f;
			yield return 0.5f;
			this.autoWalk = false;
			this.player.Speed = Vector2.Zero;
			this.player.Facing = Facings.Right;
			yield return 1.5f;
			this.badeline.Sprite.Scale.X = -1f;
			yield return 1f;
			this.badeline.Sprite.Scale.X = -1f;
			this.badelineAutoWalk = true;
			this.badelineWalkApproachFrom = this.badeline.Position;
			base.Add(new Coroutine(this.BadelineApproachWalking(), true));
			yield return 0.7f;
			this.autoWalk = true;
			this.player.Facing = Facings.Left;
			yield return this.WaitForPlayer();
			yield return this.FadeTo(1f);
			yield return 1f;
			this.SetBgFade(0.3f);
			yield return this.NextLevel("credits-tree");
			yield return this.SetupLevel();
			Petals petals = new Petals();
			this.Level.Foreground.Backdrops.Add(petals);
			this.autoUpdateCamera = false;
			Vector2 target = this.Level.Camera.Position + new Vector2(-220f, 32f);
			this.Level.Camera.Position += new Vector2(-100f, 0f);
			this.badelineWalkApproach = 1f;
			this.badelineAutoFloat = false;
			this.badelineAutoWalk = true;
			this.badeline.Floatness = 0f;
			base.Add(new Coroutine(this.FadeTo(0f), true));
			base.Add(new Coroutine(CutsceneEntity.CameraTo(target, 12f, Ease.Linear, 0f), true));
			yield return 3.5f;
			this.badeline.Sprite.Play("idle", false, false);
			this.badelineAutoWalk = false;
			yield return 0.25f;
			this.autoWalk = false;
			this.player.Sprite.Play("idle", false, false);
			this.player.Speed = Vector2.Zero;
			this.player.DummyAutoAnimate = false;
			this.player.Facing = Facings.Right;
			yield return 0.5f;
			this.player.Sprite.Play("sitDown", false, false);
			yield return 4f;
			this.badeline.Sprite.Play("laugh", false, false);
			yield return 1.75f;
			yield return this.FadeTo(1f);
			this.Level.Foreground.Backdrops.Remove(petals);
			petals = null;
			yield return 1f;
			this.SetBgFade(0.4f);
			yield return this.NextLevel("credits-clouds");
			yield return this.SetupLevel();
			this.autoWalk = false;
			this.player.Speed = Vector2.Zero;
			this.autoUpdateCamera = false;
			this.player.ForceCameraUpdate = false;
			this.badeline.Visible = false;
			Player other = null;
			foreach (Entity entity in base.Scene.Tracker.GetEntities<CreditsTrigger>())
			{
				CreditsTrigger creditsTrigger = (CreditsTrigger)entity;
				if (creditsTrigger.Event == "BadelineOffset")
				{
					other = new Player(creditsTrigger.Position, PlayerSpriteMode.Badeline);
					other.OverrideHairColor = new Color?(BadelineOldsite.HairColor);
					yield return null;
					other.StateMachine.State = 11;
					other.Facing = Facings.Left;
					base.Scene.Add(other);
				}
			}
			List<Entity>.Enumerator enumerator = default(List<Entity>.Enumerator);
			base.Add(new Coroutine(this.FadeTo(0f), true));
			this.Level.Camera.Position += new Vector2(0f, -100f);
			Vector2 target2 = this.Level.Camera.Position + new Vector2(0f, 160f);
			base.Add(new Coroutine(CutsceneEntity.CameraTo(target2, 12f, Ease.Linear, 0f), true));
			float playerHighJump = 0f;
			float baddyHighJump = 0f;
			for (float p = 0f; p < 10f; p += Engine.DeltaTime)
			{
				if (((p > 3f && p < 6f) || p > 9f) && this.player.Speed.Y < 0f && this.player.OnGround(4))
				{
					playerHighJump = 0.25f;
				}
				if (p > 5f && p < 8f && other.Speed.Y < 0f && other.OnGround(4))
				{
					baddyHighJump = 0.25f;
				}
				if (playerHighJump > 0f)
				{
					playerHighJump -= Engine.DeltaTime;
					this.player.Speed.Y = -200f;
				}
				if (baddyHighJump > 0f)
				{
					baddyHighJump -= Engine.DeltaTime;
					other.Speed.Y = -200f;
				}
				yield return null;
			}
			yield return this.FadeTo(1f);
			other = null;
			yield return 1f;
			CS07_Credits.<>c__DisplayClass24_0 CS$<>8__locals1 = new CS07_Credits.<>c__DisplayClass24_0();
			CS$<>8__locals1.<>4__this = this;
			this.SetBgFade(0.5f);
			yield return this.NextLevel("credits-resort");
			yield return this.SetupLevel();
			base.Add(new Coroutine(this.FadeTo(0f), true));
			this.badelineWalkApproach = 1f;
			this.badelineAutoFloat = false;
			this.badelineAutoWalk = true;
			this.badeline.Floatness = 0f;
			Vector2 value = Vector2.Zero;
			foreach (CreditsTrigger creditsTrigger2 in base.Scene.Entities.FindAll<CreditsTrigger>())
			{
				if (creditsTrigger2.Event == "Oshiro")
				{
					value = creditsTrigger2.Position;
				}
			}
			CS$<>8__locals1.oshiro = new NPC(value + new Vector2(0f, 4f));
			CS$<>8__locals1.oshiro.Add(CS$<>8__locals1.oshiro.Sprite = new OshiroSprite(1));
			CS$<>8__locals1.oshiro.MoveAnim = "sweeping";
			CS$<>8__locals1.oshiro.IdleAnim = "sweeping";
			CS$<>8__locals1.oshiro.Sprite.Play("sweeping", false, false);
			CS$<>8__locals1.oshiro.Maxspeed = 10f;
			CS$<>8__locals1.oshiro.Depth = -60;
			base.Scene.Add(CS$<>8__locals1.oshiro);
			base.Add(new Coroutine(this.DustyRoutine(CS$<>8__locals1.oshiro), true));
			yield return 4.8f;
			Vector2 oshiroTarget = CS$<>8__locals1.oshiro.Position + new Vector2(116f, 0f);
			Coroutine oshiroRoutine = new Coroutine(CS$<>8__locals1.oshiro.MoveTo(oshiroTarget, false, null, false), true);
			base.Add(oshiroRoutine);
			yield return 2f;
			this.autoUpdateCamera = false;
			yield return CutsceneEntity.CameraTo(new Vector2((float)(this.Level.Bounds.Left + 64), (float)this.Level.Bounds.Top), 2f, null, 0f);
			yield return 5f;
			BirdNPC bird = new BirdNPC(CS$<>8__locals1.oshiro.Position + new Vector2(280f, -160f), BirdNPC.Modes.None);
			bird.Depth = 10010;
			bird.Light.Visible = false;
			base.Scene.Add(bird);
			bird.Facing = Facings.Left;
			bird.Sprite.Play("fall", false, false);
			Vector2 from = bird.Position;
			Vector2 to = oshiroTarget + new Vector2(50f, -12f);
			baddyHighJump = 0f;
			while (baddyHighJump < 1f)
			{
				bird.Position = from + (to - from) * Ease.QuadOut(baddyHighJump);
				if (baddyHighJump > 0.5f)
				{
					bird.Sprite.Play("fly", false, false);
					bird.Depth = -1000000;
					bird.Light.Visible = true;
				}
				baddyHighJump += Engine.DeltaTime * 0.5f;
				yield return null;
			}
			bird.Position = to;
			oshiroRoutine.RemoveSelf();
			CS$<>8__locals1.oshiro.Sprite.Play("putBroomAway", false, false);
			CS$<>8__locals1.oshiro.Sprite.OnFrameChange = delegate(string anim)
			{
				if (CS$<>8__locals1.oshiro.Sprite.CurrentAnimationFrame == 10)
				{
					Entity entity3 = new Entity(CS$<>8__locals1.oshiro.Position);
					entity3.Depth = CS$<>8__locals1.oshiro.Depth + 1;
					CS$<>8__locals1.<>4__this.Scene.Add(entity3);
					entity3.Add(new Image(GFX.Game["characters/oshiro/broom"])
					{
						Origin = CS$<>8__locals1.oshiro.Sprite.Origin
					});
					CS$<>8__locals1.oshiro.Sprite.OnFrameChange = null;
				}
			};
			bird.Sprite.Play("idle", false, false);
			yield return 0.5f;
			bird.Sprite.Play("croak", false, false);
			yield return 0.6f;
			from = default(Vector2);
			to = default(Vector2);
			CS$<>8__locals1.oshiro.Maxspeed = 40f;
			CS$<>8__locals1.oshiro.MoveAnim = "move";
			CS$<>8__locals1.oshiro.IdleAnim = "idle";
			yield return CS$<>8__locals1.oshiro.MoveTo(oshiroTarget + new Vector2(14f, 0f), false, null, false);
			yield return 2f;
			base.Add(new Coroutine(bird.StartleAndFlyAway(), true));
			yield return 0.75f;
			bird.Light.Visible = false;
			bird.Depth = 10010;
			CS$<>8__locals1.oshiro.Sprite.Scale.X = -1f;
			yield return this.FadeTo(1f);
			CS$<>8__locals1 = null;
			oshiroTarget = default(Vector2);
			oshiroRoutine = null;
			bird = null;
			yield return 1f;
			this.SetBgFade(0.6f);
			yield return this.NextLevel("credits-wallslide");
			yield return this.SetupLevel();
			this.badelineAutoFloat = false;
			this.badeline.Floatness = 0f;
			this.badeline.Sprite.Play("idle", false, false);
			this.badeline.Sprite.Scale.X = 1f;
			foreach (Entity entity2 in base.Scene.Tracker.GetEntities<CreditsTrigger>())
			{
				CreditsTrigger creditsTrigger3 = (CreditsTrigger)entity2;
				if (creditsTrigger3.Event == "BadelineOffset")
				{
					this.badeline.Position = creditsTrigger3.Position + new Vector2(8f, 16f);
				}
			}
			base.Add(new Coroutine(this.FadeTo(0f), true));
			base.Add(new Coroutine(this.WaitForPlayer(), true));
			while (this.player.X > this.badeline.X - 16f)
			{
				yield return null;
			}
			this.badeline.Sprite.Scale.X = -1f;
			yield return 0.1f;
			this.badelineAutoWalk = true;
			this.badelineWalkApproachFrom = this.badeline.Position;
			this.badelineWalkApproach = 0f;
			this.badeline.Sprite.Play("walk", false, false);
			while (this.badelineWalkApproach != 1f)
			{
				this.badelineWalkApproach = Calc.Approach(this.badelineWalkApproach, 1f, Engine.DeltaTime * 4f);
				yield return null;
			}
			while (this.player.X > (float)(this.Level.Bounds.X + 160))
			{
				yield return null;
			}
			yield return this.FadeTo(1f);
			yield return 1f;
			this.SetBgFade(0.7f);
			yield return this.NextLevel("credits-payphone");
			yield return this.SetupLevel();
			this.player.Speed = Vector2.Zero;
			this.player.Facing = Facings.Left;
			this.autoWalk = false;
			this.badeline.Sprite.Play("idle", false, false);
			this.badeline.Floatness = 0f;
			this.badeline.Y = this.player.Y;
			this.badeline.Sprite.Scale.X = 1f;
			this.badelineAutoFloat = false;
			this.autoUpdateCamera = false;
			this.Level.Camera.X += 100f;
			Vector2 target3 = this.Level.Camera.Position + new Vector2(-200f, 0f);
			base.Add(new Coroutine(CutsceneEntity.CameraTo(target3, 14f, Ease.Linear, 0f), true));
			base.Add(new Coroutine(this.FadeTo(0f), true));
			yield return 1.5f;
			this.badeline.Sprite.Scale.X = -1f;
			yield return 0.5f;
			base.Add(new Coroutine(this.badeline.FloatTo(this.badeline.Position + new Vector2(16f, -12f), new int?(-1), false, false, false), true));
			yield return 0.5f;
			this.player.Facing = Facings.Right;
			yield return 1.5f;
			oshiroTarget = this.badeline.Position;
			to = this.player.Center;
			base.Add(new Coroutine(this.BadelineAround(oshiroTarget, to, this.badeline), true));
			yield return 0.5f;
			base.Add(new Coroutine(this.BadelineAround(oshiroTarget, to, null), true));
			yield return 0.5f;
			base.Add(new Coroutine(this.BadelineAround(oshiroTarget, to, null), true));
			yield return 3f;
			this.badeline.Sprite.Play("laugh", false, false);
			yield return 0.5f;
			this.player.Facing = Facings.Left;
			yield return 0.5f;
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("sitDown", false, false);
			yield return 3f;
			yield return this.FadeTo(1f);
			oshiroTarget = default(Vector2);
			to = default(Vector2);
			yield return 1f;
			this.SetBgFade(0.8f);
			yield return this.NextLevel("credits-city");
			yield return this.SetupLevel();
			BirdNPC birdNPC = base.Scene.Entities.FindFirst<BirdNPC>();
			if (birdNPC != null)
			{
				birdNPC.Facing = Facings.Right;
			}
			this.badelineWalkApproach = 1f;
			this.badelineAutoFloat = false;
			this.badelineAutoWalk = true;
			this.badeline.Floatness = 0f;
			base.Add(new Coroutine(this.FadeTo(0f), true));
			yield return this.WaitForPlayer();
			yield return this.FadeTo(1f);
			yield return 1f;
			this.SetBgFade(0f);
			yield return this.NextLevel("credits-prologue");
			yield return this.SetupLevel();
			this.badelineWalkApproach = 1f;
			this.badelineAutoFloat = false;
			this.badelineAutoWalk = true;
			this.badeline.Floatness = 0f;
			base.Add(new Coroutine(this.FadeTo(0f), true));
			yield return this.WaitForPlayer();
			yield return this.FadeTo(1f);
			while (this.credits.BottomTimer < 2f)
			{
				yield return null;
			}
			if (!this.gotoEpilogue)
			{
				this.snow = new HiresSnow(0.45f);
				this.snow.Alpha = 0f;
				this.snow.AttachAlphaTo = new FadeWipe(this.Level, false, delegate()
				{
					base.EndCutscene(this.Level, true);
				});
				this.Level.Add(this.Level.HiresSnow = this.snow);
			}
			else
			{
				new FadeWipe(this.Level, false, delegate()
				{
					base.EndCutscene(this.Level, true);
				});
			}
			yield break;
			yield break;
		}

		// Token: 0x06000CBC RID: 3260 RVA: 0x0002AD28 File Offset: 0x00028F28
		private IEnumerator SetupLevel()
		{
			this.Level.SnapColorGrade("credits");
			this.player = null;
			while ((this.player = base.Scene.Tracker.GetEntity<Player>()) == null)
			{
				yield return null;
			}
			this.Level.Add(this.badeline = new BadelineDummy(this.player.Position + new Vector2(16f, -16f)));
			this.badeline.Floatness = 4f;
			this.badelineAutoFloat = true;
			this.badelineAutoWalk = false;
			this.badelineWalkApproach = 0f;
			this.Level.Session.Inventory.Dashes = 1;
			this.player.Dashes = 1;
			this.player.StateMachine.State = 11;
			this.player.DummyFriction = false;
			this.player.DummyMaxspeed = false;
			this.player.Facing = Facings.Left;
			this.autoWalk = true;
			this.autoUpdateCamera = true;
			this.Level.CameraOffset.X = 70f;
			this.Level.CameraOffset.Y = -24f;
			this.Level.Camera.Position = this.player.CameraTarget;
			yield break;
		}

		// Token: 0x06000CBD RID: 3261 RVA: 0x0002AD37 File Offset: 0x00028F37
		private IEnumerator WaitForPlayer()
		{
			while (this.player.X > (float)(this.Level.Bounds.X + 160))
			{
				if (this.Event != null)
				{
					yield return this.DoEvent(this.Event);
				}
				this.Event = null;
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000CBE RID: 3262 RVA: 0x0002AD46 File Offset: 0x00028F46
		private IEnumerator NextLevel(string name)
		{
			if (this.player != null)
			{
				this.player.RemoveSelf();
			}
			this.player = null;
			this.Level.OnEndOfFrame += delegate()
			{
				this.Level.UnloadLevel();
				this.Level.Session.Level = name;
				this.Level.Session.RespawnPoint = new Vector2?(this.Level.GetSpawnPoint(new Vector2((float)this.Level.Bounds.Left, (float)this.Level.Bounds.Top)));
				this.Level.LoadLevel(Player.IntroTypes.None, false);
				this.Level.Wipe.Cancel();
			};
			yield return null;
			yield return null;
			yield break;
		}

		// Token: 0x06000CBF RID: 3263 RVA: 0x0002AD5C File Offset: 0x00028F5C
		private IEnumerator FadeTo(float value)
		{
			while ((this.fade = Calc.Approach(this.fade, value, Engine.DeltaTime * 0.5f)) != value)
			{
				yield return null;
			}
			this.fade = value;
			yield break;
		}

		// Token: 0x06000CC0 RID: 3264 RVA: 0x0002AD72 File Offset: 0x00028F72
		private IEnumerator BadelineApproachWalking()
		{
			while (this.badelineWalkApproach < 1f)
			{
				this.badeline.Floatness = Calc.Approach(this.badeline.Floatness, 0f, Engine.DeltaTime * 8f);
				this.badelineWalkApproach = Calc.Approach(this.badelineWalkApproach, 1f, Engine.DeltaTime * 0.6f);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000CC1 RID: 3265 RVA: 0x0002AD81 File Offset: 0x00028F81
		private IEnumerator DustyRoutine(Entity oshiro)
		{
			List<Entity> dusty = new List<Entity>();
			float timer = 0f;
			Vector2 offset = oshiro.Position + new Vector2(220f, -24f);
			Vector2 start = offset;
			for (int i = 0; i < 3; i++)
			{
				Entity entity = new Entity(offset + new Vector2((float)(i * 24), 0f));
				entity.Depth = -50;
				entity.Add(new DustGraphic(true, false, true));
				Image image = new Image(GFX.Game["decals/3-resort/brokenbox_" + ((char)(97 + i)).ToString()]);
				image.JustifyOrigin(0.5f, 1f);
				image.Position = new Vector2(0f, -4f);
				entity.Add(image);
				base.Scene.Add(entity);
				dusty.Add(entity);
			}
			yield return 3.8f;
			for (;;)
			{
				for (int j = 0; j < dusty.Count; j++)
				{
					Entity entity2 = dusty[j];
					entity2.X = offset.X + (float)(j * 24);
					entity2.Y = offset.Y + (float)Math.Sin((double)(timer * 4f + (float)j * 0.8f)) * 4f;
				}
				if (offset.X < (float)(this.Level.Bounds.Left + 120))
				{
					offset.Y = Calc.Approach(offset.Y, start.Y + 16f, Engine.DeltaTime * 16f);
				}
				offset.X -= 26f * Engine.DeltaTime;
				timer += Engine.DeltaTime;
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000CC2 RID: 3266 RVA: 0x0002AD97 File Offset: 0x00028F97
		private IEnumerator BadelineAround(Vector2 start, Vector2 around, BadelineDummy badeline = null)
		{
			bool removeAtEnd = badeline == null;
			if (badeline == null)
			{
				base.Scene.Add(badeline = new BadelineDummy(start));
			}
			badeline.Sprite.Play("fallSlow", false, false);
			float angle = Calc.Angle(around, start);
			float dist = (around - start).Length();
			float duration = 3f;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
			{
				float num = p * 2f;
				badeline.Position = around + Calc.AngleToVector(angle - num * 6.2831855f, dist + Calc.YoYo(p) * 16f + (float)Math.Sin((double)(p * 6.2831855f * 4f)) * 5f);
				badeline.Sprite.Scale.X = (float)Math.Sign(around.X - badeline.X);
				if (!removeAtEnd)
				{
					this.player.Facing = (Facings)Math.Sign(badeline.X - this.player.X);
				}
				if (base.Scene.OnInterval(0.1f))
				{
					TrailManager.Add(badeline, Player.NormalHairColor, 1f, false, false);
				}
				yield return null;
			}
			if (removeAtEnd)
			{
				badeline.Vanish();
			}
			else
			{
				badeline.Sprite.Play("laugh", false, false);
			}
			yield break;
		}

		// Token: 0x06000CC3 RID: 3267 RVA: 0x0002ADBB File Offset: 0x00028FBB
		private IEnumerator DoEvent(string e)
		{
			if (e == "WaitJumpDash")
			{
				yield return this.EventWaitJumpDash();
			}
			else if (e == "WaitJumpDoubleDash")
			{
				yield return this.EventWaitJumpDoubleDash();
			}
			else if (e == "ClimbDown")
			{
				yield return this.EventClimbDown();
			}
			else if (e == "Wait")
			{
				yield return this.EventWait();
			}
			yield break;
		}

		// Token: 0x06000CC4 RID: 3268 RVA: 0x0002ADD1 File Offset: 0x00028FD1
		private IEnumerator EventWaitJumpDash()
		{
			this.autoWalk = false;
			this.player.DummyFriction = true;
			yield return 0.1f;
			this.PlayerJump(-1);
			yield return 0.2f;
			this.player.OverrideDashDirection = new Vector2?(new Vector2(-1f, -1f));
			this.player.StateMachine.State = this.player.StartDash();
			yield return 0.6f;
			this.player.OverrideDashDirection = null;
			this.player.StateMachine.State = 11;
			this.autoWalk = true;
			yield break;
		}

		// Token: 0x06000CC5 RID: 3269 RVA: 0x0002ADE0 File Offset: 0x00028FE0
		private IEnumerator EventWaitJumpDoubleDash()
		{
			this.autoWalk = false;
			this.player.DummyFriction = true;
			yield return 0.1f;
			this.player.Facing = Facings.Right;
			yield return 0.25f;
			yield return this.BadelineCombine();
			this.player.Dashes = 2;
			yield return 0.5f;
			this.player.Facing = Facings.Left;
			yield return 0.7f;
			this.PlayerJump(-1);
			yield return 0.4f;
			this.player.OverrideDashDirection = new Vector2?(new Vector2(-1f, -1f));
			this.player.StateMachine.State = this.player.StartDash();
			yield return 0.6f;
			this.player.OverrideDashDirection = new Vector2?(new Vector2(-1f, 0f));
			this.player.StateMachine.State = this.player.StartDash();
			yield return 0.6f;
			this.player.OverrideDashDirection = null;
			this.player.StateMachine.State = 11;
			this.autoWalk = true;
			while (!this.player.OnGround(1))
			{
				yield return null;
			}
			this.autoWalk = false;
			this.player.DummyFriction = true;
			this.player.Dashes = 2;
			yield return 0.5f;
			this.player.Facing = Facings.Right;
			yield return 1f;
			this.Level.Displacement.AddBurst(this.player.Position, 0.4f, 8f, 32f, 0.5f, null, null);
			this.badeline.Position = this.player.Position;
			this.badeline.Visible = true;
			this.badelineAutoFloat = true;
			this.player.Dashes = 1;
			yield return 0.8f;
			this.player.Facing = Facings.Left;
			this.autoWalk = true;
			this.player.DummyFriction = false;
			yield break;
		}

		// Token: 0x06000CC6 RID: 3270 RVA: 0x0002ADEF File Offset: 0x00028FEF
		private IEnumerator EventClimbDown()
		{
			this.autoWalk = false;
			this.player.DummyFriction = true;
			yield return 0.1f;
			this.PlayerJump(-1);
			yield return 0.4f;
			while (!this.player.CollideCheck<Solid>(this.player.Position + new Vector2(-1f, 0f)))
			{
				yield return null;
			}
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("wallslide", false, false);
			while (this.player.CollideCheck<Solid>(this.player.Position + new Vector2(-1f, 32f)))
			{
				this.player.CreateWallSlideParticles(-1);
				this.player.Speed.Y = Math.Min(this.player.Speed.Y, 40f);
				yield return null;
			}
			this.PlayerJump(1);
			yield return 0.4f;
			while (!this.player.CollideCheck<Solid>(this.player.Position + new Vector2(1f, 0f)))
			{
				yield return null;
			}
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("wallslide", false, false);
			while (!this.player.CollideCheck<Solid>(this.player.Position + new Vector2(0f, 32f)))
			{
				this.player.CreateWallSlideParticles(1);
				this.player.Speed.Y = Math.Min(this.player.Speed.Y, 40f);
				yield return null;
			}
			this.PlayerJump(-1);
			yield return 0.4f;
			this.autoWalk = true;
			yield break;
		}

		// Token: 0x06000CC7 RID: 3271 RVA: 0x0002ADFE File Offset: 0x00028FFE
		private IEnumerator EventWait()
		{
			this.badeline.Sprite.Play("idle", false, false);
			this.badelineAutoWalk = false;
			this.autoWalk = false;
			this.player.DummyFriction = true;
			yield return 0.1f;
			this.player.DummyAutoAnimate = false;
			this.player.Speed = Vector2.Zero;
			yield return 0.5f;
			this.player.Sprite.Play("lookUp", false, false);
			yield return 2f;
			BirdNPC birdNPC = base.Scene.Entities.FindFirst<BirdNPC>();
			if (birdNPC != null)
			{
				birdNPC.AutoFly = true;
			}
			yield return 0.1f;
			this.player.Sprite.Play("idle", false, false);
			yield return 1f;
			this.autoWalk = true;
			this.player.DummyFriction = false;
			this.player.DummyAutoAnimate = true;
			this.badelineAutoWalk = true;
			this.badelineWalkApproach = 0f;
			this.badelineWalkApproachFrom = this.badeline.Position;
			this.badeline.Sprite.Play("walk", false, false);
			while (this.badelineWalkApproach < 1f)
			{
				this.badelineWalkApproach += Engine.DeltaTime * 4f;
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000CC8 RID: 3272 RVA: 0x0002AE0D File Offset: 0x0002900D
		private IEnumerator BadelineCombine()
		{
			Vector2 from = this.badeline.Position;
			this.badelineAutoFloat = false;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.25f)
			{
				this.badeline.Position = Vector2.Lerp(from, this.player.Position, Ease.CubeIn(p));
				yield return null;
			}
			this.badeline.Visible = false;
			this.Level.Displacement.AddBurst(this.player.Position, 0.4f, 8f, 32f, 0.5f, null, null);
			yield break;
		}

		// Token: 0x06000CC9 RID: 3273 RVA: 0x0002AE1C File Offset: 0x0002901C
		private void PlayerJump(int direction)
		{
			this.player.Facing = (Facings)direction;
			this.player.DummyFriction = false;
			this.player.DummyAutoAnimate = true;
			this.player.Speed.X = (float)(direction * 120);
			this.player.Jump(true, true);
			this.player.AutoJump = true;
			this.player.AutoJumpTimer = 2f;
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x0002AE8B File Offset: 0x0002908B
		private void SetBgFade(float alpha)
		{
			this.fillbg.Color = Color.Black * alpha;
		}

		// Token: 0x06000CCB RID: 3275 RVA: 0x0002AEA4 File Offset: 0x000290A4
		public override void Update()
		{
			MInput.Disabled = false;
			if (this.Level.CanPause && (Input.Pause.Pressed || Input.ESC.Pressed))
			{
				Input.Pause.ConsumeBuffer();
				Input.ESC.ConsumeBuffer();
				this.Level.Pause(0, true, false);
			}
			MInput.Disabled = true;
			if (this.player != null && this.player.Scene != null)
			{
				if (this.player.OverrideDashDirection != null)
				{
					Input.MoveX.Value = (int)this.player.OverrideDashDirection.Value.X;
					Input.MoveY.Value = (int)this.player.OverrideDashDirection.Value.Y;
				}
				if (this.autoWalk)
				{
					if (this.player.OnGround(1))
					{
						this.player.Speed.X = -44.8f;
						bool flag = this.player.CollideCheck<Solid>(this.player.Position + new Vector2(-20f, 0f));
						bool flag2 = !this.player.CollideCheck<Solid>(this.player.Position + new Vector2(-8f, 1f)) && !this.player.CollideCheck<Solid>(this.player.Position + new Vector2(-8f, 32f));
						if (flag || flag2)
						{
							this.player.Jump(true, true);
							this.player.AutoJump = true;
							this.player.AutoJumpTimer = (flag ? 0.6f : 2f);
						}
					}
					else
					{
						this.player.Speed.X = -64f;
					}
				}
				if (this.badeline != null && this.badelineAutoFloat)
				{
					Vector2 position = this.badeline.Position;
					Vector2 value = this.player.Position + new Vector2(16f, -16f);
					this.badeline.Position = position + (value - position) * (1f - (float)Math.Pow(0.009999999776482582, (double)Engine.DeltaTime));
					this.badeline.Sprite.Scale.X = -1f;
				}
				if (this.badeline != null && this.badelineAutoWalk)
				{
					Player.ChaserState chaserState;
					this.player.GetChasePosition(base.Scene.TimeActive, 0.35f + (float)Math.Sin((double)this.walkOffset) * 0.1f, out chaserState);
					if (chaserState.OnGround)
					{
						this.walkOffset += Engine.DeltaTime;
					}
					if (this.badelineWalkApproach >= 1f)
					{
						this.badeline.Position = chaserState.Position;
						if (this.badeline.Sprite.Has(chaserState.Animation))
						{
							this.badeline.Sprite.Play(chaserState.Animation, false, false);
						}
						this.badeline.Sprite.Scale.X = (float)chaserState.Facing;
					}
					else
					{
						this.badeline.Position = Vector2.Lerp(this.badelineWalkApproachFrom, chaserState.Position, this.badelineWalkApproach);
					}
				}
				if (Math.Abs(this.player.Speed.X) > 90f)
				{
					this.player.Speed.X = Calc.Approach(this.player.Speed.X, 90f * (float)Math.Sign(this.player.Speed.X), 1000f * Engine.DeltaTime);
				}
			}
			if (this.credits != null)
			{
				this.credits.Update();
			}
			base.Update();
		}

		// Token: 0x06000CCC RID: 3276 RVA: 0x0002B284 File Offset: 0x00029484
		public void PostUpdate()
		{
			if (this.player != null && this.player.Scene != null && this.autoUpdateCamera)
			{
				Vector2 position = this.Level.Camera.Position;
				Vector2 cameraTarget = this.player.CameraTarget;
				if (!this.player.OnGround(1))
				{
					cameraTarget.Y = (this.Level.Camera.Y * 2f + cameraTarget.Y) / 3f;
				}
				this.Level.Camera.Position = position + (cameraTarget - position) * (1f - (float)Math.Pow(0.009999999776482582, (double)Engine.DeltaTime));
				this.Level.Camera.X = (float)((int)cameraTarget.X);
			}
		}

		// Token: 0x06000CCD RID: 3277 RVA: 0x0002B364 File Offset: 0x00029564
		public override void Render()
		{
			bool flag = SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode;
			if (!this.Level.Paused)
			{
				if (flag)
				{
					this.gradient.Draw(new Vector2(1720f, -10f), Vector2.Zero, Color.White * 0.6f, new Vector2(-1f, 1100f));
				}
				else
				{
					this.gradient.Draw(new Vector2(200f, -10f), Vector2.Zero, Color.White * 0.6f, new Vector2(1f, 1100f));
				}
			}
			if (this.fade > 0f)
			{
				Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * Ease.CubeInOut(this.fade));
			}
			if (this.credits != null && !this.Level.Paused)
			{
				this.credits.Render(new Vector2((float)(flag ? 100 : 1820), 0f));
			}
			base.Render();
		}

		// Token: 0x06000CCE RID: 3278 RVA: 0x0002B494 File Offset: 0x00029694
		public override void OnEnd(Level level)
		{
			SaveData.Instance.Assists.DashAssist = this.wasDashAssistOn;
			Audio.BusMuted("bus:/gameplay_sfx", new bool?(false));
			CS07_Credits.Instance = null;
			MInput.Disabled = false;
			if (!this.gotoEpilogue)
			{
				Engine.Scene = new OverworldLoader(Overworld.StartMode.AreaComplete, this.snow);
				return;
			}
			LevelEnter.Go(new Session(new AreaKey(8, AreaMode.Normal), null, null), false);
		}

		// Token: 0x04000815 RID: 2069
		public const float CameraXOffset = 70f;

		// Token: 0x04000816 RID: 2070
		public const float CameraYOffset = -24f;

		// Token: 0x04000817 RID: 2071
		public static CS07_Credits Instance;

		// Token: 0x04000818 RID: 2072
		public string Event;

		// Token: 0x04000819 RID: 2073
		private MTexture gradient = GFX.Gui["creditsgradient"].GetSubtexture(0, 1, 1920, 1, null);

		// Token: 0x0400081A RID: 2074
		private Credits credits;

		// Token: 0x0400081B RID: 2075
		private Player player;

		// Token: 0x0400081C RID: 2076
		private bool autoWalk = true;

		// Token: 0x0400081D RID: 2077
		private bool autoUpdateCamera = true;

		// Token: 0x0400081E RID: 2078
		private BadelineDummy badeline;

		// Token: 0x0400081F RID: 2079
		private bool badelineAutoFloat = true;

		// Token: 0x04000820 RID: 2080
		private bool badelineAutoWalk;

		// Token: 0x04000821 RID: 2081
		private float badelineWalkApproach;

		// Token: 0x04000822 RID: 2082
		private Vector2 badelineWalkApproachFrom;

		// Token: 0x04000823 RID: 2083
		private float walkOffset;

		// Token: 0x04000824 RID: 2084
		private bool wasDashAssistOn;

		// Token: 0x04000825 RID: 2085
		private CS07_Credits.Fill fillbg;

		// Token: 0x04000826 RID: 2086
		private float fade = 1f;

		// Token: 0x04000827 RID: 2087
		private HiresSnow snow;

		// Token: 0x04000828 RID: 2088
		private bool gotoEpilogue;

		// Token: 0x02000401 RID: 1025
		private class Fill : Backdrop
		{
			// Token: 0x06002003 RID: 8195 RVA: 0x000DC438 File Offset: 0x000DA638
			public override void Render(Scene scene)
			{
				Draw.Rect(-10f, -10f, 340f, 200f, this.Color);
			}
		}
	}
}
