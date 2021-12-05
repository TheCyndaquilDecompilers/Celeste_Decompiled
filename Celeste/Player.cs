using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200037E RID: 894
	[Tracked(false)]
	public class Player : Actor
	{
		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06001C81 RID: 7297 RVA: 0x000C0E90 File Offset: 0x000BF090
		// (set) Token: 0x06001C82 RID: 7298 RVA: 0x000C0E98 File Offset: 0x000BF098
		public bool Dead { get; private set; }

		// Token: 0x06001C83 RID: 7299 RVA: 0x000C0EA4 File Offset: 0x000BF0A4
		public Player(Vector2 position, PlayerSpriteMode spriteMode) : base(new Vector2((float)((int)position.X), (float)((int)position.Y)))
		{
			Input.ResetGrab();
			this.DefaultSpriteMode = spriteMode;
			base.Depth = 0;
			base.Tag = Tags.Persistent;
			if (SaveData.Instance != null && SaveData.Instance.Assists.PlayAsBadeline)
			{
				spriteMode = PlayerSpriteMode.MadelineAsBadeline;
			}
			this.Sprite = new PlayerSprite(spriteMode);
			base.Add(this.Hair = new PlayerHair(this.Sprite));
			base.Add(this.Sprite);
			if (spriteMode == PlayerSpriteMode.MadelineAsBadeline)
			{
				this.Hair.Color = Player.NormalBadelineHairColor;
			}
			else
			{
				this.Hair.Color = Player.NormalHairColor;
			}
			this.startHairCount = this.Sprite.HairCount;
			this.sweatSprite = GFX.SpriteBank.Create("player_sweat");
			base.Add(this.sweatSprite);
			base.Collider = this.normalHitbox;
			this.hurtbox = this.normalHurtbox;
			this.onCollideH = new Collision(this.OnCollideH);
			this.onCollideV = new Collision(this.OnCollideV);
			this.StateMachine = new StateMachine(26);
			this.StateMachine.SetCallbacks(0, new Func<int>(this.NormalUpdate), null, new Action(this.NormalBegin), new Action(this.NormalEnd));
			this.StateMachine.SetCallbacks(1, new Func<int>(this.ClimbUpdate), null, new Action(this.ClimbBegin), new Action(this.ClimbEnd));
			this.StateMachine.SetCallbacks(2, new Func<int>(this.DashUpdate), new Func<IEnumerator>(this.DashCoroutine), new Action(this.DashBegin), new Action(this.DashEnd));
			this.StateMachine.SetCallbacks(3, new Func<int>(this.SwimUpdate), null, new Action(this.SwimBegin), null);
			this.StateMachine.SetCallbacks(4, new Func<int>(this.BoostUpdate), new Func<IEnumerator>(this.BoostCoroutine), new Action(this.BoostBegin), new Action(this.BoostEnd));
			this.StateMachine.SetCallbacks(5, new Func<int>(this.RedDashUpdate), new Func<IEnumerator>(this.RedDashCoroutine), new Action(this.RedDashBegin), new Action(this.RedDashEnd));
			this.StateMachine.SetCallbacks(6, new Func<int>(this.HitSquashUpdate), null, new Action(this.HitSquashBegin), null);
			this.StateMachine.SetCallbacks(7, new Func<int>(this.LaunchUpdate), null, new Action(this.LaunchBegin), null);
			this.StateMachine.SetCallbacks(8, null, new Func<IEnumerator>(this.PickupCoroutine), null, null);
			this.StateMachine.SetCallbacks(9, new Func<int>(this.DreamDashUpdate), null, new Action(this.DreamDashBegin), new Action(this.DreamDashEnd));
			this.StateMachine.SetCallbacks(10, new Func<int>(this.SummitLaunchUpdate), null, new Action(this.SummitLaunchBegin), null);
			this.StateMachine.SetCallbacks(11, new Func<int>(this.DummyUpdate), null, new Action(this.DummyBegin), null);
			this.StateMachine.SetCallbacks(12, null, new Func<IEnumerator>(this.IntroWalkCoroutine), null, null);
			this.StateMachine.SetCallbacks(13, null, new Func<IEnumerator>(this.IntroJumpCoroutine), null, null);
			this.StateMachine.SetCallbacks(14, null, null, new Action(this.IntroRespawnBegin), new Action(this.IntroRespawnEnd));
			this.StateMachine.SetCallbacks(15, null, new Func<IEnumerator>(this.IntroWakeUpCoroutine), null, null);
			this.StateMachine.SetCallbacks(20, new Func<int>(this.TempleFallUpdate), new Func<IEnumerator>(this.TempleFallCoroutine), null, null);
			this.StateMachine.SetCallbacks(18, new Func<int>(this.ReflectionFallUpdate), new Func<IEnumerator>(this.ReflectionFallCoroutine), new Action(this.ReflectionFallBegin), new Action(this.ReflectionFallEnd));
			this.StateMachine.SetCallbacks(16, new Func<int>(this.BirdDashTutorialUpdate), new Func<IEnumerator>(this.BirdDashTutorialCoroutine), new Action(this.BirdDashTutorialBegin), null);
			this.StateMachine.SetCallbacks(17, new Func<int>(this.FrozenUpdate), null, null, null);
			this.StateMachine.SetCallbacks(19, new Func<int>(this.StarFlyUpdate), new Func<IEnumerator>(this.StarFlyCoroutine), new Action(this.StarFlyBegin), new Action(this.StarFlyEnd));
			this.StateMachine.SetCallbacks(21, new Func<int>(this.CassetteFlyUpdate), new Func<IEnumerator>(this.CassetteFlyCoroutine), new Action(this.CassetteFlyBegin), new Action(this.CassetteFlyEnd));
			this.StateMachine.SetCallbacks(22, new Func<int>(this.AttractUpdate), null, new Action(this.AttractBegin), new Action(this.AttractEnd));
			this.StateMachine.SetCallbacks(23, null, new Func<IEnumerator>(this.IntroMoonJumpCoroutine), null, null);
			this.StateMachine.SetCallbacks(24, new Func<int>(this.FlingBirdUpdate), new Func<IEnumerator>(this.FlingBirdCoroutine), new Action(this.FlingBirdBegin), new Action(this.FlingBirdEnd));
			this.StateMachine.SetCallbacks(25, null, new Func<IEnumerator>(this.IntroThinkForABitCoroutine), null, null);
			base.Add(this.StateMachine);
			base.Add(this.Leader = new Leader(new Vector2(0f, -8f)));
			this.lastAim = Vector2.UnitX;
			this.Facing = Facings.Right;
			this.ChaserStates = new List<Player.ChaserState>();
			this.triggersInside = new HashSet<Trigger>();
			base.Add(this.Light = new VertexLight(this.normalLightOffset, Color.White, 1f, 32, 64));
			base.Add(new WaterInteraction(() => this.StateMachine.State == 2 || this.StateMachine.State == 18));
			base.Add(new WindMover(new Action<Vector2>(this.WindMove)));
			base.Add(this.wallSlideSfx = new SoundSource());
			base.Add(this.swimSurfaceLoopSfx = new SoundSource());
			this.Sprite.OnFrameChange = delegate(string anim)
			{
				if (base.Scene != null && !this.Dead && this.Sprite.Visible)
				{
					int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
					if ((anim.Equals("runSlow_carry") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim.Equals("runFast") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim.Equals("runSlow") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim.Equals("walk") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim.Equals("runStumble") && currentAnimationFrame == 6) || (anim.Equals("flip") && currentAnimationFrame == 4) || (anim.Equals("runWind") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim.Equals("idleC") && this.Sprite.Mode == PlayerSpriteMode.MadelineNoBackpack && (currentAnimationFrame == 3 || currentAnimationFrame == 6 || currentAnimationFrame == 8 || currentAnimationFrame == 11)) || (anim.Equals("carryTheoWalk") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim.Equals("push") && (currentAnimationFrame == 8 || currentAnimationFrame == 15)))
					{
						Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(base.CollideAll<Platform>(this.Position + Vector2.UnitY, this.temp));
						if (platformByPriority != null)
						{
							this.Play("event:/char/madeline/footstep", "surface_index", (float)platformByPriority.GetStepSoundIndex(this));
						}
					}
					else if ((anim.Equals("climbUp") && currentAnimationFrame == 5) || (anim.Equals("climbDown") && currentAnimationFrame == 5))
					{
						Platform platformByPriority2 = SurfaceIndex.GetPlatformByPriority(base.CollideAll<Solid>(base.Center + Vector2.UnitX * (float)this.Facing, this.temp));
						if (platformByPriority2 != null)
						{
							this.Play("event:/char/madeline/handhold", "surface_index", (float)platformByPriority2.GetWallSoundIndex(this, (int)this.Facing));
						}
					}
					else if (anim.Equals("wakeUp") && currentAnimationFrame == 19)
					{
						this.Play("event:/char/madeline/campfire_stand", null, 0f);
					}
					else if (anim.Equals("sitDown") && currentAnimationFrame == 12)
					{
						this.Play("event:/char/madeline/summit_sit", null, 0f);
					}
					if (anim.Equals("push") && (currentAnimationFrame == 8 || currentAnimationFrame == 15))
					{
						Dust.BurstFG(this.Position + new Vector2((float)(-this.Facing * (Facings)5), -1f), new Vector2((float)(-(float)this.Facing), -0.5f).Angle(), 1, 0f, null);
					}
				}
			};
			this.Sprite.OnLastFrame = delegate(string anim)
			{
				if (base.Scene != null && !this.Dead && this.Sprite.CurrentAnimationID == "idle" && !this.level.InCutscene && this.idleTimer > 3f && Calc.Random.Chance(0.2f))
				{
					string text;
					if (this.Sprite.Mode == PlayerSpriteMode.Madeline)
					{
						text = ((this.level.CoreMode == Session.CoreModes.Hot) ? Player.idleWarmOptions : Player.idleColdOptions).Choose();
					}
					else
					{
						text = Player.idleNoBackpackOptions.Choose();
					}
					if (!string.IsNullOrEmpty(text) && this.Sprite.Has(text))
					{
						this.Sprite.Play(text, false, false);
						if (this.Sprite.Mode == PlayerSpriteMode.Madeline)
						{
							if (text == "idleB")
							{
								this.idleSfx = this.Play("event:/char/madeline/idle_scratch", null, 0f);
								return;
							}
							if (text == "idleC")
							{
								this.idleSfx = this.Play("event:/char/madeline/idle_sneeze", null, 0f);
								return;
							}
						}
						else if (text == "idleA")
						{
							this.idleSfx = this.Play("event:/char/madeline/idle_crackknuckles", null, 0f);
						}
					}
				}
			};
			this.Sprite.OnChange = delegate(string last, string next)
			{
				if ((last == "idleB" || last == "idleC") && next != null && !next.StartsWith("idle") && this.idleSfx != null)
				{
					Audio.Stop(this.idleSfx, true);
				}
			};
			base.Add(this.reflection = new MirrorReflection());
		}

		// Token: 0x06001C84 RID: 7300 RVA: 0x000C16C1 File Offset: 0x000BF8C1
		public void ResetSpriteNextFrame(PlayerSpriteMode mode)
		{
			this.nextSpriteMode = new PlayerSpriteMode?(mode);
		}

		// Token: 0x06001C85 RID: 7301 RVA: 0x000C16D0 File Offset: 0x000BF8D0
		public void ResetSprite(PlayerSpriteMode mode)
		{
			string currentAnimationID = this.Sprite.CurrentAnimationID;
			int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
			this.Sprite.RemoveSelf();
			base.Add(this.Sprite = new PlayerSprite(mode));
			if (this.Sprite.Has(currentAnimationID))
			{
				this.Sprite.Play(currentAnimationID, false, false);
				if (currentAnimationFrame < this.Sprite.CurrentAnimationTotalFrames)
				{
					this.Sprite.SetAnimationFrame(currentAnimationFrame);
				}
			}
			this.Hair.Sprite = this.Sprite;
		}

		// Token: 0x06001C86 RID: 7302 RVA: 0x000C175C File Offset: 0x000BF95C
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
			this.lastDashes = (this.Dashes = this.MaxDashes);
			SpawnFacingTrigger spawnFacingTrigger = base.CollideFirst<SpawnFacingTrigger>();
			if (spawnFacingTrigger != null)
			{
				this.Facing = spawnFacingTrigger.Facing;
			}
			else if (base.X > (float)this.level.Bounds.Center.X && this.IntroType != Player.IntroTypes.None)
			{
				this.Facing = Facings.Left;
			}
			switch (this.IntroType)
			{
			case Player.IntroTypes.Respawn:
				this.StateMachine.State = 14;
				this.JustRespawned = true;
				break;
			case Player.IntroTypes.WalkInRight:
				this.IntroWalkDirection = Facings.Right;
				this.StateMachine.State = 12;
				break;
			case Player.IntroTypes.WalkInLeft:
				this.IntroWalkDirection = Facings.Left;
				this.StateMachine.State = 12;
				break;
			case Player.IntroTypes.Jump:
				this.StateMachine.State = 13;
				break;
			case Player.IntroTypes.WakeUp:
				this.Sprite.Play("asleep", false, false);
				this.Facing = Facings.Right;
				this.StateMachine.State = 15;
				break;
			case Player.IntroTypes.Fall:
				this.StateMachine.State = 18;
				break;
			case Player.IntroTypes.TempleMirrorVoid:
				this.StartTempleMirrorVoidSleep();
				break;
			case Player.IntroTypes.None:
				this.StateMachine.State = 0;
				break;
			case Player.IntroTypes.ThinkForABit:
				this.StateMachine.State = 25;
				break;
			}
			this.IntroType = Player.IntroTypes.Transition;
			this.StartHair();
			this.PreviousPosition = this.Position;
		}

		// Token: 0x06001C87 RID: 7303 RVA: 0x000C18DC File Offset: 0x000BFADC
		public void StartTempleMirrorVoidSleep()
		{
			this.Sprite.Play("asleep", false, false);
			this.Facing = Facings.Right;
			this.StateMachine.State = 11;
			this.StateMachine.Locked = true;
			this.DummyAutoAnimate = false;
			this.DummyGravity = false;
		}

		// Token: 0x06001C88 RID: 7304 RVA: 0x000C192C File Offset: 0x000BFB2C
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			this.level = null;
			Audio.Stop(this.conveyorLoopSfx, true);
			foreach (Trigger trigger in this.triggersInside)
			{
				trigger.Triggered = false;
				trigger.OnLeave(this);
			}
			this.triggersInside.Clear();
		}

		// Token: 0x06001C89 RID: 7305 RVA: 0x000C19AC File Offset: 0x000BFBAC
		public override void SceneEnd(Scene scene)
		{
			base.SceneEnd(scene);
			Audio.Stop(this.conveyorLoopSfx, true);
		}

		// Token: 0x06001C8A RID: 7306 RVA: 0x000C19C4 File Offset: 0x000BFBC4
		public override void Render()
		{
			if (SaveData.Instance.Assists.InvisibleMotion && this.InControl)
			{
				if (!this.onGround && this.StateMachine.State != 1 && this.StateMachine.State != 3)
				{
					return;
				}
				if (this.Speed.LengthSquared() > 800f)
				{
					return;
				}
			}
			Vector2 renderPosition = this.Sprite.RenderPosition;
			this.Sprite.RenderPosition = this.Sprite.RenderPosition.Floor();
			if (this.StateMachine.State == 14)
			{
				DeathEffect.Draw(base.Center + this.deadOffset, this.Hair.Color, this.introEase);
			}
			else
			{
				if (this.StateMachine.State != 19)
				{
					if (this.IsTired && this.flash)
					{
						this.Sprite.Color = Color.Red;
					}
					else
					{
						this.Sprite.Color = Color.White;
					}
				}
				if (this.reflection.IsRendering && this.FlipInReflection)
				{
					this.Facing = -this.Facing;
					this.Hair.Facing = this.Facing;
				}
				PlayerSprite sprite = this.Sprite;
				sprite.Scale.X = sprite.Scale.X * (float)this.Facing;
				if (this.sweatSprite.LastAnimationID == "idle")
				{
					this.sweatSprite.Scale = this.Sprite.Scale;
				}
				else
				{
					this.sweatSprite.Scale.Y = this.Sprite.Scale.Y;
					this.sweatSprite.Scale.X = Math.Abs(this.Sprite.Scale.X) * (float)Math.Sign(this.sweatSprite.Scale.X);
				}
				base.Render();
				if (this.Sprite.CurrentAnimationID == "startStarFly")
				{
					float scale = (float)this.Sprite.CurrentAnimationFrame / (float)this.Sprite.CurrentAnimationTotalFrames;
					GFX.Game.GetAtlasSubtexturesAt("characters/player/startStarFlyWhite", this.Sprite.CurrentAnimationFrame).Draw(this.Sprite.RenderPosition, this.Sprite.Origin, this.starFlyColor * scale, this.Sprite.Scale, this.Sprite.Rotation, SpriteEffects.None);
				}
				PlayerSprite sprite2 = this.Sprite;
				sprite2.Scale.X = sprite2.Scale.X * (float)this.Facing;
				if (this.reflection.IsRendering && this.FlipInReflection)
				{
					this.Facing = -this.Facing;
					this.Hair.Facing = this.Facing;
				}
			}
			this.Sprite.RenderPosition = renderPosition;
		}

		// Token: 0x06001C8B RID: 7307 RVA: 0x000C1C88 File Offset: 0x000BFE88
		public override void DebugRender(Camera camera)
		{
			base.DebugRender(camera);
			Collider collider = base.Collider;
			base.Collider = this.hurtbox;
			Draw.HollowRect(base.Collider, Color.Lime);
			base.Collider = collider;
		}

		// Token: 0x06001C8C RID: 7308 RVA: 0x000C1CC8 File Offset: 0x000BFEC8
		public override void Update()
		{
			if (SaveData.Instance.Assists.InfiniteStamina)
			{
				this.Stamina = 110f;
			}
			this.PreviousPosition = this.Position;
			if (this.nextSpriteMode != null)
			{
				this.ResetSprite(this.nextSpriteMode.Value);
				this.nextSpriteMode = null;
			}
			this.climbTriggerDir = 0;
			if (SaveData.Instance.Assists.Hiccups)
			{
				if (this.hiccupTimer <= 0f)
				{
					this.hiccupTimer = this.level.HiccupRandom.Range(1.2f, 1.8f);
				}
				if (this.Ducking)
				{
					this.hiccupTimer -= Engine.DeltaTime * 0.5f;
				}
				else
				{
					this.hiccupTimer -= Engine.DeltaTime;
				}
				if (this.hiccupTimer <= 0f)
				{
					this.HiccupJump();
				}
			}
			if (this.gliderBoostTimer > 0f)
			{
				this.gliderBoostTimer -= Engine.DeltaTime;
			}
			if (this.lowFrictionStopTimer > 0f)
			{
				this.lowFrictionStopTimer -= Engine.DeltaTime;
			}
			if (this.explodeLaunchBoostTimer > 0f)
			{
				if (Input.MoveX.Value == Math.Sign(this.explodeLaunchBoostSpeed))
				{
					this.Speed.X = this.explodeLaunchBoostSpeed;
					this.explodeLaunchBoostTimer = 0f;
				}
				else
				{
					this.explodeLaunchBoostTimer -= Engine.DeltaTime;
				}
			}
			this.StrawberryCollectResetTimer -= Engine.DeltaTime;
			if (this.StrawberryCollectResetTimer <= 0f)
			{
				this.StrawberryCollectIndex = 0;
			}
			this.idleTimer += Engine.DeltaTime;
			if (this.level != null && this.level.InCutscene)
			{
				this.idleTimer = -5f;
			}
			else if (this.Speed.X != 0f || this.Speed.Y != 0f)
			{
				this.idleTimer = 0f;
			}
			if (!this.Dead)
			{
				Audio.MusicUnderwater = this.UnderwaterMusicCheck();
			}
			if (this.JustRespawned && this.Speed != Vector2.Zero)
			{
				this.JustRespawned = false;
			}
			if (this.StateMachine.State == 9)
			{
				this.onGround = (this.OnSafeGround = false);
			}
			else if (this.Speed.Y >= 0f)
			{
				Platform platform = base.CollideFirst<Solid>(this.Position + Vector2.UnitY);
				if (platform == null)
				{
					platform = base.CollideFirstOutside<JumpThru>(this.Position + Vector2.UnitY);
				}
				if (platform != null)
				{
					this.onGround = true;
					this.OnSafeGround = platform.Safe;
				}
				else
				{
					this.onGround = (this.OnSafeGround = false);
				}
			}
			else
			{
				this.onGround = (this.OnSafeGround = false);
			}
			if (this.StateMachine.State == 3)
			{
				this.OnSafeGround = true;
			}
			if (this.OnSafeGround)
			{
				using (List<Component>.Enumerator enumerator = base.Scene.Tracker.GetComponents<SafeGroundBlocker>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (((SafeGroundBlocker)enumerator.Current).Check(this))
						{
							this.OnSafeGround = false;
							break;
						}
					}
				}
			}
			this.playFootstepOnLand -= Engine.DeltaTime;
			if (this.onGround)
			{
				this.highestAirY = base.Y;
			}
			else
			{
				this.highestAirY = Math.Min(base.Y, this.highestAirY);
			}
			if (base.Scene.OnInterval(0.05f))
			{
				this.flash = !this.flash;
			}
			if (this.wallSlideDir != 0)
			{
				this.wallSlideTimer = Math.Max(this.wallSlideTimer - Engine.DeltaTime, 0f);
				this.wallSlideDir = 0;
			}
			if (this.wallBoostTimer > 0f)
			{
				this.wallBoostTimer -= Engine.DeltaTime;
				if (this.moveX == this.wallBoostDir)
				{
					this.Speed.X = 130f * (float)this.moveX;
					this.Stamina += 27.5f;
					this.wallBoostTimer = 0f;
					this.sweatSprite.Play("idle", false, false);
				}
			}
			if (this.onGround && this.StateMachine.State != 1)
			{
				this.AutoJump = false;
				this.Stamina = 110f;
				this.wallSlideTimer = 1.2f;
			}
			if (this.dashAttackTimer > 0f)
			{
				this.dashAttackTimer -= Engine.DeltaTime;
			}
			if (this.onGround)
			{
				this.dreamJump = false;
				this.jumpGraceTimer = 0.1f;
			}
			else if (this.jumpGraceTimer > 0f)
			{
				this.jumpGraceTimer -= Engine.DeltaTime;
			}
			if (this.dashCooldownTimer > 0f)
			{
				this.dashCooldownTimer -= Engine.DeltaTime;
			}
			if (this.dashRefillCooldownTimer > 0f)
			{
				this.dashRefillCooldownTimer -= Engine.DeltaTime;
			}
			else if (SaveData.Instance.Assists.DashMode == Assists.DashModes.Infinite && !this.level.InCutscene)
			{
				this.RefillDash();
			}
			else if (!this.Inventory.NoRefills)
			{
				if (this.StateMachine.State == 3)
				{
					this.RefillDash();
				}
				else if (this.onGround && (base.CollideCheck<Solid, NegaBlock>(this.Position + Vector2.UnitY) || base.CollideCheckOutside<JumpThru>(this.Position + Vector2.UnitY)) && (!base.CollideCheck<Spikes>(this.Position) || SaveData.Instance.Assists.Invincible))
				{
					this.RefillDash();
				}
			}
			if (this.varJumpTimer > 0f)
			{
				this.varJumpTimer -= Engine.DeltaTime;
			}
			if (this.AutoJumpTimer > 0f)
			{
				if (this.AutoJump)
				{
					this.AutoJumpTimer -= Engine.DeltaTime;
					if (this.AutoJumpTimer <= 0f)
					{
						this.AutoJump = false;
					}
				}
				else
				{
					this.AutoJumpTimer = 0f;
				}
			}
			if (this.forceMoveXTimer > 0f)
			{
				this.forceMoveXTimer -= Engine.DeltaTime;
				this.moveX = this.forceMoveX;
			}
			else
			{
				this.moveX = Input.MoveX.Value;
				this.climbHopSolid = null;
			}
			if (this.climbHopSolid != null && !this.climbHopSolid.Collidable)
			{
				this.climbHopSolid = null;
			}
			else if (this.climbHopSolid != null && this.climbHopSolid.Position != this.climbHopSolidPosition)
			{
				Vector2 vector = this.climbHopSolid.Position - this.climbHopSolidPosition;
				this.climbHopSolidPosition = this.climbHopSolid.Position;
				base.MoveHExact((int)vector.X, null, null);
				base.MoveVExact((int)vector.Y, null, null);
			}
			if (this.noWindTimer > 0f)
			{
				this.noWindTimer -= Engine.DeltaTime;
			}
			if (this.moveX != 0 && this.InControl && this.StateMachine.State != 1 && this.StateMachine.State != 8 && this.StateMachine.State != 5 && this.StateMachine.State != 6)
			{
				Facings facings = (Facings)this.moveX;
				if (facings != this.Facing && this.Ducking)
				{
					this.Sprite.Scale = new Vector2(0.8f, 1.2f);
				}
				this.Facing = facings;
			}
			this.lastAim = Input.GetAimVector(this.Facing);
			if (this.wallSpeedRetentionTimer > 0f)
			{
				if (Math.Sign(this.Speed.X) == -Math.Sign(this.wallSpeedRetained))
				{
					this.wallSpeedRetentionTimer = 0f;
				}
				else if (!base.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float)Math.Sign(this.wallSpeedRetained)))
				{
					this.Speed.X = this.wallSpeedRetained;
					this.wallSpeedRetentionTimer = 0f;
				}
				else
				{
					this.wallSpeedRetentionTimer -= Engine.DeltaTime;
				}
			}
			if (this.hopWaitX != 0)
			{
				if (Math.Sign(this.Speed.X) == -this.hopWaitX || this.Speed.Y > 0f)
				{
					this.hopWaitX = 0;
				}
				else if (!base.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float)this.hopWaitX))
				{
					this.lowFrictionStopTimer = 0.15f;
					this.Speed.X = this.hopWaitXSpeed;
					this.hopWaitX = 0;
				}
			}
			if (this.windTimeout > 0f)
			{
				this.windTimeout -= Engine.DeltaTime;
			}
			Vector2 forceStrongWindHair = this.windDirection;
			if (this.ForceStrongWindHair.Length() > 0f)
			{
				forceStrongWindHair = this.ForceStrongWindHair;
			}
			if (this.windTimeout > 0f && forceStrongWindHair.X != 0f)
			{
				this.windHairTimer += Engine.DeltaTime * 8f;
				this.Hair.StepPerSegment = new Vector2(forceStrongWindHair.X * 5f, (float)Math.Sin((double)this.windHairTimer));
				this.Hair.StepInFacingPerSegment = 0f;
				this.Hair.StepApproach = 128f;
				this.Hair.StepYSinePerSegment = 0f;
			}
			else if (this.Dashes > 1)
			{
				this.Hair.StepPerSegment = new Vector2((float)Math.Sin((double)(base.Scene.TimeActive * 2f)) * 0.7f - (float)(this.Facing * (Facings)3), (float)Math.Sin((double)(base.Scene.TimeActive * 1f)));
				this.Hair.StepInFacingPerSegment = 0f;
				this.Hair.StepApproach = 90f;
				this.Hair.StepYSinePerSegment = 1f;
				PlayerHair hair = this.Hair;
				hair.StepPerSegment.Y = hair.StepPerSegment.Y + forceStrongWindHair.Y * 2f;
			}
			else
			{
				this.Hair.StepPerSegment = new Vector2(0f, 2f);
				this.Hair.StepInFacingPerSegment = 0.5f;
				this.Hair.StepApproach = 64f;
				this.Hair.StepYSinePerSegment = 0f;
				PlayerHair hair2 = this.Hair;
				hair2.StepPerSegment.Y = hair2.StepPerSegment.Y + forceStrongWindHair.Y * 0.5f;
			}
			if (this.StateMachine.State == 5)
			{
				this.Sprite.HairCount = 1;
			}
			else if (this.StateMachine.State != 19)
			{
				this.Sprite.HairCount = ((this.Dashes > 1) ? 5 : this.startHairCount);
			}
			if (this.minHoldTimer > 0f)
			{
				this.minHoldTimer -= Engine.DeltaTime;
			}
			if (this.launched)
			{
				if (this.Speed.LengthSquared() < 19600f)
				{
					this.launched = false;
				}
				else
				{
					float prevVal = this.launchedTimer;
					this.launchedTimer += Engine.DeltaTime;
					if (this.launchedTimer >= 0.5f)
					{
						this.launched = false;
						this.launchedTimer = 0f;
					}
					else if (Calc.OnInterval(this.launchedTimer, prevVal, 0.15f))
					{
						this.level.Add(Engine.Pooler.Create<SpeedRing>().Init(base.Center, this.Speed.Angle(), Color.White));
					}
				}
			}
			else
			{
				this.launchedTimer = 0f;
			}
			if (this.IsTired)
			{
				Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
				if (!this.wasTired)
				{
					this.wasTired = true;
				}
			}
			else
			{
				this.wasTired = false;
			}
			base.Update();
			if (this.Ducking)
			{
				this.Light.Position = this.duckingLightOffset;
			}
			else
			{
				this.Light.Position = this.normalLightOffset;
			}
			if (!this.onGround && this.Speed.Y <= 0f && (this.StateMachine.State != 1 || this.lastClimbMove == -1) && base.CollideCheck<JumpThru>() && !this.JumpThruBoostBlockedCheck())
			{
				base.MoveV(-40f * Engine.DeltaTime, null, null);
			}
			if (!this.onGround && this.DashAttacking && this.DashDir.Y == 0f && (base.CollideCheck<Solid>(this.Position + Vector2.UnitY * 3f) || base.CollideCheckOutside<JumpThru>(this.Position + Vector2.UnitY * 3f)) && !this.DashCorrectCheck(Vector2.UnitY * 3f))
			{
				base.MoveVExact(3, null, null);
			}
			if (this.Speed.Y > 0f && this.CanUnDuck && base.Collider != this.starFlyHitbox && !this.onGround && this.jumpGraceTimer <= 0f)
			{
				this.Ducking = false;
			}
			if (this.StateMachine.State != 9 && this.StateMachine.State != 22)
			{
				base.MoveH(this.Speed.X * Engine.DeltaTime, this.onCollideH, null);
			}
			if (this.StateMachine.State != 9 && this.StateMachine.State != 22)
			{
				base.MoveV(this.Speed.Y * Engine.DeltaTime, this.onCollideV, null);
			}
			if (this.StateMachine.State == 3)
			{
				if (this.Speed.Y < 0f && this.Speed.Y >= -60f)
				{
					while (!this.SwimCheck())
					{
						this.Speed.Y = 0f;
						if (base.MoveVExact(1, null, null))
						{
							break;
						}
					}
				}
			}
			else if (this.StateMachine.State == 0 && this.SwimCheck())
			{
				this.StateMachine.State = 3;
			}
			else if (this.StateMachine.State == 1 && this.SwimCheck())
			{
				Water water = base.CollideFirst<Water>(this.Position);
				if (water != null && base.Center.Y < water.Center.Y)
				{
					while (this.SwimCheck() && !base.MoveVExact(-1, null, null))
					{
					}
					if (this.SwimCheck())
					{
						this.StateMachine.State = 3;
					}
				}
				else
				{
					this.StateMachine.State = 3;
				}
			}
			if (this.Sprite.CurrentAnimationID != null && this.Sprite.CurrentAnimationID.Equals("wallslide") && this.Speed.Y > 0f)
			{
				if (!this.wallSlideSfx.Playing)
				{
					this.Loop(this.wallSlideSfx, "event:/char/madeline/wallslide");
				}
				Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(base.CollideAll<Solid>(base.Center + Vector2.UnitX * (float)this.Facing, this.temp));
				if (platformByPriority != null)
				{
					this.wallSlideSfx.Param("surface_index", (float)platformByPriority.GetWallSoundIndex(this, (int)this.Facing));
				}
			}
			else
			{
				this.Stop(this.wallSlideSfx);
			}
			this.UpdateSprite();
			this.UpdateCarry();
			if (this.StateMachine.State != 18)
			{
				foreach (Entity entity in base.Scene.Tracker.GetEntities<Trigger>())
				{
					Trigger trigger = (Trigger)entity;
					if (base.CollideCheck(trigger))
					{
						if (!trigger.Triggered)
						{
							trigger.Triggered = true;
							this.triggersInside.Add(trigger);
							trigger.OnEnter(this);
						}
						trigger.OnStay(this);
					}
					else if (trigger.Triggered)
					{
						this.triggersInside.Remove(trigger);
						trigger.Triggered = false;
						trigger.OnLeave(this);
					}
				}
			}
			this.StrawberriesBlocked = base.CollideCheck<BlockField>();
			if (this.InControl || this.ForceCameraUpdate)
			{
				if (this.StateMachine.State == 18)
				{
					this.level.Camera.Position = this.CameraTarget;
				}
				else
				{
					Vector2 position = this.level.Camera.Position;
					Vector2 cameraTarget = this.CameraTarget;
					float num = (this.StateMachine.State == 20) ? 8f : 1f;
					this.level.Camera.Position = position + (cameraTarget - position) * (1f - (float)Math.Pow((double)(0.01f / num), (double)Engine.DeltaTime));
				}
			}
			if (!this.Dead && this.StateMachine.State != 21)
			{
				Collider collider = base.Collider;
				base.Collider = this.hurtbox;
				using (List<Component>.Enumerator enumerator = base.Scene.Tracker.GetComponents<PlayerCollider>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (((PlayerCollider)enumerator.Current).Check(this) && this.Dead)
						{
							base.Collider = collider;
							return;
						}
					}
				}
				if (base.Collider == this.hurtbox)
				{
					base.Collider = collider;
				}
			}
			if (this.InControl && !this.Dead && this.StateMachine.State != 9 && this.EnforceLevelBounds)
			{
				this.level.EnforceBounds(this);
			}
			this.UpdateChaserStates();
			this.UpdateHair(true);
			if (this.wasDucking != this.Ducking)
			{
				this.wasDucking = this.Ducking;
				if (this.wasDucking)
				{
					this.Play("event:/char/madeline/duck", null, 0f);
				}
				else if (this.onGround)
				{
					this.Play("event:/char/madeline/stand", null, 0f);
				}
			}
			if (this.Speed.X != 0f && ((this.StateMachine.State == 3 && !this.SwimUnderwaterCheck()) || (this.StateMachine.State == 0 && base.CollideCheck<Water>(this.Position))))
			{
				if (!this.swimSurfaceLoopSfx.Playing)
				{
					this.swimSurfaceLoopSfx.Play("event:/char/madeline/water_move_shallow", null, 0f);
				}
			}
			else
			{
				this.swimSurfaceLoopSfx.Stop(true);
			}
			this.wasOnGround = this.onGround;
			this.windMovedUp = false;
		}

		// Token: 0x06001C8D RID: 7309 RVA: 0x000C2FA0 File Offset: 0x000C11A0
		private void CreateTrail()
		{
			Vector2 scale = new Vector2(Math.Abs(this.Sprite.Scale.X) * (float)this.Facing, this.Sprite.Scale.Y);
			if (this.Sprite.Mode == PlayerSpriteMode.MadelineAsBadeline)
			{
				TrailManager.Add(this, scale, this.wasDashB ? Player.NormalBadelineHairColor : Player.UsedBadelineHairColor, 1f);
				return;
			}
			TrailManager.Add(this, scale, this.wasDashB ? Player.NormalHairColor : Player.UsedHairColor, 1f);
		}

		// Token: 0x06001C8E RID: 7310 RVA: 0x000C3030 File Offset: 0x000C1230
		public void CleanUpTriggers()
		{
			if (this.triggersInside.Count > 0)
			{
				foreach (Trigger trigger in this.triggersInside)
				{
					trigger.OnLeave(this);
					trigger.Triggered = false;
				}
				this.triggersInside.Clear();
			}
		}

		// Token: 0x06001C8F RID: 7311 RVA: 0x000C30A4 File Offset: 0x000C12A4
		private void UpdateChaserStates()
		{
			while (this.ChaserStates.Count > 0 && base.Scene.TimeActive - this.ChaserStates[0].TimeStamp > 4f)
			{
				this.ChaserStates.RemoveAt(0);
			}
			this.ChaserStates.Add(new Player.ChaserState(this));
			this.activeSounds.Clear();
		}

		// Token: 0x06001C90 RID: 7312 RVA: 0x000C310D File Offset: 0x000C130D
		private void StartHair()
		{
			if (this.startHairCalled)
			{
				return;
			}
			this.startHairCalled = true;
			this.Hair.Facing = this.Facing;
			this.Hair.Start();
			this.UpdateHair(true);
		}

		// Token: 0x06001C91 RID: 7313 RVA: 0x000C3144 File Offset: 0x000C1344
		public void UpdateHair(bool applyGravity)
		{
			if (this.StateMachine.State == 19)
			{
				this.Hair.Color = this.Sprite.Color;
				applyGravity = false;
			}
			else if (this.Dashes == 0 && this.Dashes < this.MaxDashes)
			{
				if (this.Sprite.Mode == PlayerSpriteMode.MadelineAsBadeline)
				{
					this.Hair.Color = Color.Lerp(this.Hair.Color, Player.UsedBadelineHairColor, 6f * Engine.DeltaTime);
				}
				else
				{
					this.Hair.Color = Color.Lerp(this.Hair.Color, Player.UsedHairColor, 6f * Engine.DeltaTime);
				}
			}
			else
			{
				Color color;
				if (this.lastDashes != this.Dashes)
				{
					color = Player.FlashHairColor;
					this.hairFlashTimer = 0.12f;
				}
				else if (this.hairFlashTimer > 0f)
				{
					color = Player.FlashHairColor;
					this.hairFlashTimer -= Engine.DeltaTime;
				}
				else if (this.Sprite.Mode == PlayerSpriteMode.MadelineAsBadeline)
				{
					if (this.Dashes == 2)
					{
						color = Player.TwoDashesBadelineHairColor;
					}
					else
					{
						color = Player.NormalBadelineHairColor;
					}
				}
				else if (this.Dashes == 2)
				{
					color = Player.TwoDashesHairColor;
				}
				else
				{
					color = Player.NormalHairColor;
				}
				this.Hair.Color = color;
			}
			if (this.OverrideHairColor != null)
			{
				this.Hair.Color = this.OverrideHairColor.Value;
			}
			this.Hair.Facing = this.Facing;
			this.Hair.SimulateMotion = applyGravity;
			this.lastDashes = this.Dashes;
		}

		// Token: 0x06001C92 RID: 7314 RVA: 0x000C32E0 File Offset: 0x000C14E0
		private void UpdateSprite()
		{
			this.Sprite.Scale.X = Calc.Approach(this.Sprite.Scale.X, 1f, 1.75f * Engine.DeltaTime);
			this.Sprite.Scale.Y = Calc.Approach(this.Sprite.Scale.Y, 1f, 1.75f * Engine.DeltaTime);
			if (this.InControl && this.Sprite.CurrentAnimationID != "throw" && this.StateMachine.State != 20 && this.StateMachine.State != 18 && this.StateMachine.State != 19 && this.StateMachine.State != 21)
			{
				if (this.StateMachine.State == 22)
				{
					this.Sprite.Play("fallFast", false, false);
				}
				else if (this.StateMachine.State == 10)
				{
					this.Sprite.Play("launch", false, false);
				}
				else if (this.StateMachine.State == 8)
				{
					this.Sprite.Play("pickup", false, false);
				}
				else if (this.StateMachine.State == 3)
				{
					if (Input.MoveY.Value > 0)
					{
						this.Sprite.Play("swimDown", false, false);
					}
					else if (Input.MoveY.Value < 0)
					{
						this.Sprite.Play("swimUp", false, false);
					}
					else
					{
						this.Sprite.Play("swimIdle", false, false);
					}
				}
				else if (this.StateMachine.State == 9)
				{
					if (this.Sprite.CurrentAnimationID != "dreamDashIn" && this.Sprite.CurrentAnimationID != "dreamDashLoop")
					{
						this.Sprite.Play("dreamDashIn", false, false);
					}
				}
				else if (this.Sprite.DreamDashing && this.Sprite.LastAnimationID != "dreamDashOut")
				{
					this.Sprite.Play("dreamDashOut", false, false);
				}
				else if (this.Sprite.CurrentAnimationID != "dreamDashOut")
				{
					if (this.DashAttacking)
					{
						if (this.onGround && this.DashDir.Y == 0f && !this.Ducking && this.Speed.X != 0f && this.moveX == -Math.Sign(this.Speed.X))
						{
							if (base.Scene.OnInterval(0.02f))
							{
								Dust.Burst(this.Position, -1.5707964f, 1, null);
							}
							this.Sprite.Play("skid", false, false);
						}
						else if (this.Ducking)
						{
							this.Sprite.Play("duck", false, false);
						}
						else
						{
							this.Sprite.Play("dash", false, false);
						}
					}
					else if (this.StateMachine.State == 1)
					{
						if (this.lastClimbMove < 0)
						{
							this.Sprite.Play("climbUp", false, false);
						}
						else if (this.lastClimbMove > 0)
						{
							this.Sprite.Play("wallslide", false, false);
						}
						else if (!base.CollideCheck<Solid>(this.Position + new Vector2((float)this.Facing, 6f)))
						{
							this.Sprite.Play("dangling", false, false);
						}
						else if (Input.MoveX == (float)(-(float)this.Facing))
						{
							if (this.Sprite.CurrentAnimationID != "climbLookBack")
							{
								this.Sprite.Play("climbLookBackStart", false, false);
							}
						}
						else
						{
							this.Sprite.Play("wallslide", false, false);
						}
					}
					else if (this.Ducking && this.StateMachine.State == 0)
					{
						this.Sprite.Play("duck", false, false);
					}
					else if (this.onGround)
					{
						this.fastJump = false;
						if (this.Holding == null && this.moveX != 0 && base.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float)this.moveX) && !ClimbBlocker.EdgeCheck(this.level, this, this.moveX))
						{
							this.Sprite.Play("push", false, false);
						}
						else if (Math.Abs(this.Speed.X) <= 25f && this.moveX == 0)
						{
							if (this.Holding != null)
							{
								this.Sprite.Play("idle_carry", false, false);
							}
							else if (!base.Scene.CollideCheck<Solid>(this.Position + new Vector2((float)this.Facing, 2f)) && !base.Scene.CollideCheck<Solid>(this.Position + new Vector2((float)(this.Facing * (Facings)4), 2f)) && !base.CollideCheck<JumpThru>(this.Position + new Vector2((float)(this.Facing * (Facings)4), 2f)))
							{
								this.Sprite.Play("edge", false, false);
							}
							else if (!base.Scene.CollideCheck<Solid>(this.Position + new Vector2((float)(-(float)this.Facing), 2f)) && !base.Scene.CollideCheck<Solid>(this.Position + new Vector2((float)(-this.Facing * (Facings)4), 2f)) && !base.CollideCheck<JumpThru>(this.Position + new Vector2((float)(-this.Facing * (Facings)4), 2f)))
							{
								this.Sprite.Play("edgeBack", false, false);
							}
							else if (Input.MoveY.Value == -1)
							{
								if (this.Sprite.LastAnimationID != "lookUp")
								{
									this.Sprite.Play("lookUp", false, false);
								}
							}
							else if (this.Sprite.CurrentAnimationID != null && (!this.Sprite.CurrentAnimationID.Contains("idle") || (this.Sprite.CurrentAnimationID == "idle_carry" && this.Holding == null)))
							{
								this.Sprite.Play("idle", false, false);
							}
						}
						else if (this.Holding != null)
						{
							this.Sprite.Play("runSlow_carry", false, false);
						}
						else if (Math.Sign(this.Speed.X) == -this.moveX && this.moveX != 0)
						{
							if (Math.Abs(this.Speed.X) > 90f)
							{
								this.Sprite.Play("skid", false, false);
							}
							else if (this.Sprite.CurrentAnimationID != "skid")
							{
								this.Sprite.Play("flip", false, false);
							}
						}
						else if (this.windDirection.X != 0f && this.windTimeout > 0f && this.Facing == (Facings)(-(Facings)Math.Sign(this.windDirection.X)))
						{
							this.Sprite.Play("runWind", false, false);
						}
						else if (!this.Sprite.Running || this.Sprite.CurrentAnimationID == "runWind" || (this.Sprite.CurrentAnimationID == "runSlow_carry" && this.Holding == null))
						{
							if (Math.Abs(this.Speed.X) < 45f)
							{
								this.Sprite.Play("runSlow", false, false);
							}
							else
							{
								this.Sprite.Play("runFast", false, false);
							}
						}
					}
					else if (this.wallSlideDir != 0 && this.Holding == null)
					{
						this.Sprite.Play("wallslide", false, false);
					}
					else if (this.Speed.Y < 0f)
					{
						if (this.Holding != null)
						{
							this.Sprite.Play("jumpSlow_carry", false, false);
						}
						else if (this.fastJump || Math.Abs(this.Speed.X) > 90f)
						{
							this.fastJump = true;
							this.Sprite.Play("jumpFast", false, false);
						}
						else
						{
							this.Sprite.Play("jumpSlow", false, false);
						}
					}
					else if (this.Holding != null)
					{
						this.Sprite.Play("fallSlow_carry", false, false);
					}
					else if (this.fastJump || this.Speed.Y >= 160f || this.level.InSpace)
					{
						this.fastJump = true;
						if (this.Sprite.LastAnimationID != "fallFast")
						{
							this.Sprite.Play("fallFast", false, false);
						}
					}
					else
					{
						this.Sprite.Play("fallSlow", false, false);
					}
				}
			}
			if (this.StateMachine.State != 11)
			{
				if (this.level.InSpace)
				{
					this.Sprite.Rate = 0.5f;
					return;
				}
				this.Sprite.Rate = 1f;
			}
		}

		// Token: 0x06001C93 RID: 7315 RVA: 0x000C3C98 File Offset: 0x000C1E98
		public void CreateSplitParticles()
		{
			this.level.Particles.Emit(Player.P_Split, 16, base.Center, Vector2.One * 6f);
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06001C94 RID: 7316 RVA: 0x000C3CC8 File Offset: 0x000C1EC8
		public Vector2 CameraTarget
		{
			get
			{
				Vector2 vector = default(Vector2);
				Vector2 vector2 = new Vector2(base.X - 160f, base.Y - 90f);
				if (this.StateMachine.State != 18)
				{
					vector2 += new Vector2(this.level.CameraOffset.X, this.level.CameraOffset.Y);
				}
				if (this.StateMachine.State == 19)
				{
					vector2.X += 0.2f * this.Speed.X;
					vector2.Y += 0.2f * this.Speed.Y;
				}
				else if (this.StateMachine.State == 5)
				{
					vector2.X += (float)(48 * Math.Sign(this.Speed.X));
					vector2.Y += (float)(48 * Math.Sign(this.Speed.Y));
				}
				else if (this.StateMachine.State == 10)
				{
					vector2.Y -= 64f;
				}
				else if (this.StateMachine.State == 18)
				{
					vector2.Y += 32f;
				}
				if (this.CameraAnchorLerp.Length() > 0f)
				{
					if (this.CameraAnchorIgnoreX && !this.CameraAnchorIgnoreY)
					{
						vector2.Y = MathHelper.Lerp(vector2.Y, this.CameraAnchor.Y, this.CameraAnchorLerp.Y);
					}
					else if (!this.CameraAnchorIgnoreX && this.CameraAnchorIgnoreY)
					{
						vector2.X = MathHelper.Lerp(vector2.X, this.CameraAnchor.X, this.CameraAnchorLerp.X);
					}
					else if (this.CameraAnchorLerp.X == this.CameraAnchorLerp.Y)
					{
						vector2 = Vector2.Lerp(vector2, this.CameraAnchor, this.CameraAnchorLerp.X);
					}
					else
					{
						vector2.X = MathHelper.Lerp(vector2.X, this.CameraAnchor.X, this.CameraAnchorLerp.X);
						vector2.Y = MathHelper.Lerp(vector2.Y, this.CameraAnchor.Y, this.CameraAnchorLerp.Y);
					}
				}
				if (this.EnforceLevelBounds)
				{
					vector.X = MathHelper.Clamp(vector2.X, (float)this.level.Bounds.Left, (float)(this.level.Bounds.Right - 320));
					vector.Y = MathHelper.Clamp(vector2.Y, (float)this.level.Bounds.Top, (float)(this.level.Bounds.Bottom - 180));
				}
				else
				{
					vector = vector2;
				}
				if (this.level.CameraLockMode != Level.CameraLockModes.None)
				{
					CameraLocker component = base.Scene.Tracker.GetComponent<CameraLocker>();
					if (this.level.CameraLockMode != Level.CameraLockModes.BoostSequence)
					{
						vector.X = Math.Max(vector.X, this.level.Camera.X);
						if (component != null)
						{
							vector.X = Math.Min(vector.X, Math.Max((float)this.level.Bounds.Left, component.Entity.X - component.MaxXOffset));
						}
					}
					if (this.level.CameraLockMode == Level.CameraLockModes.FinalBoss)
					{
						vector.Y = Math.Max(vector.Y, this.level.Camera.Y);
						if (component != null)
						{
							vector.Y = Math.Min(vector.Y, Math.Max((float)this.level.Bounds.Top, component.Entity.Y - component.MaxYOffset));
						}
					}
					else if (this.level.CameraLockMode == Level.CameraLockModes.BoostSequence)
					{
						this.level.CameraUpwardMaxY = Math.Min(this.level.Camera.Y + 180f, this.level.CameraUpwardMaxY);
						vector.Y = Math.Min(vector.Y, this.level.CameraUpwardMaxY);
						if (component != null)
						{
							vector.Y = Math.Max(vector.Y, Math.Min((float)(this.level.Bounds.Bottom - 180), component.Entity.Y - component.MaxYOffset));
						}
					}
				}
				foreach (Entity entity in base.Scene.Tracker.GetEntities<Killbox>())
				{
					if (entity.Collidable && base.Top < entity.Bottom && base.Right > entity.Left && base.Left < entity.Right)
					{
						vector.Y = Math.Min(vector.Y, entity.Top - 180f);
					}
				}
				return vector;
			}
		}

		// Token: 0x06001C95 RID: 7317 RVA: 0x000C4200 File Offset: 0x000C2400
		public bool GetChasePosition(float sceneTime, float timeAgo, out Player.ChaserState chaseState)
		{
			if (!this.Dead)
			{
				bool flag = false;
				foreach (Player.ChaserState chaserState in this.ChaserStates)
				{
					float num = sceneTime - chaserState.TimeStamp;
					if (num <= timeAgo)
					{
						if (flag || timeAgo - num < 0.02f)
						{
							chaseState = chaserState;
							return true;
						}
						chaseState = default(Player.ChaserState);
						return false;
					}
					else
					{
						flag = true;
					}
				}
			}
			chaseState = default(Player.ChaserState);
			return false;
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06001C96 RID: 7318 RVA: 0x000C4298 File Offset: 0x000C2498
		public bool CanRetry
		{
			get
			{
				int state = this.StateMachine.State;
				return state - 12 > 3 && state != 18 && state != 25;
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06001C97 RID: 7319 RVA: 0x000C42C8 File Offset: 0x000C24C8
		public bool TimePaused
		{
			get
			{
				if (this.Dead)
				{
					return true;
				}
				int state = this.StateMachine.State;
				return state == 10 || state - 12 <= 3 || state == 25;
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06001C98 RID: 7320 RVA: 0x000C4300 File Offset: 0x000C2500
		public bool InControl
		{
			get
			{
				switch (this.StateMachine.State)
				{
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 23:
				case 25:
					return false;
				default:
					return true;
				}
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06001C99 RID: 7321 RVA: 0x000C4361 File Offset: 0x000C2561
		public PlayerInventory Inventory
		{
			get
			{
				if (this.level != null && this.level.Session != null)
				{
					return this.level.Session.Inventory;
				}
				return PlayerInventory.Default;
			}
		}

		// Token: 0x06001C9A RID: 7322 RVA: 0x000C4390 File Offset: 0x000C2590
		public void OnTransition()
		{
			this.wallSlideTimer = 1.2f;
			this.jumpGraceTimer = 0f;
			this.forceMoveXTimer = 0f;
			this.ChaserStates.Clear();
			this.RefillDash();
			this.RefillStamina();
			this.Leader.TransferFollowers();
		}

		// Token: 0x06001C9B RID: 7323 RVA: 0x000C43E4 File Offset: 0x000C25E4
		public bool TransitionTo(Vector2 target, Vector2 direction)
		{
			base.MoveTowardsX(target.X, 60f * Engine.DeltaTime, null);
			base.MoveTowardsY(target.Y, 60f * Engine.DeltaTime, null);
			this.UpdateHair(false);
			this.UpdateCarry();
			if (this.Position == target)
			{
				base.ZeroRemainderX();
				base.ZeroRemainderY();
				this.Speed.X = (float)((int)Math.Round((double)this.Speed.X));
				this.Speed.Y = (float)((int)Math.Round((double)this.Speed.Y));
				return true;
			}
			return false;
		}

		// Token: 0x06001C9C RID: 7324 RVA: 0x000091E2 File Offset: 0x000073E2
		public void BeforeSideTransition()
		{
		}

		// Token: 0x06001C9D RID: 7325 RVA: 0x000C4488 File Offset: 0x000C2688
		public void BeforeDownTransition()
		{
			if (this.StateMachine.State != 5 && this.StateMachine.State != 18 && this.StateMachine.State != 19)
			{
				this.StateMachine.State = 0;
				this.Speed.Y = Math.Max(0f, this.Speed.Y);
				this.AutoJump = false;
				this.varJumpTimer = 0f;
			}
			foreach (Entity entity in base.Scene.Tracker.GetEntities<Platform>())
			{
				if (!(entity is SolidTiles) && base.CollideCheckOutside(entity, this.Position + Vector2.UnitY * base.Height))
				{
					entity.Collidable = false;
				}
			}
		}

		// Token: 0x06001C9E RID: 7326 RVA: 0x000C457C File Offset: 0x000C277C
		public void BeforeUpTransition()
		{
			this.Speed.X = 0f;
			if (this.StateMachine.State != 5 && this.StateMachine.State != 18 && this.StateMachine.State != 19)
			{
				this.varJumpSpeed = (this.Speed.Y = -105f);
				if (this.StateMachine.State == 10)
				{
					this.StateMachine.State = 13;
				}
				else
				{
					this.StateMachine.State = 0;
				}
				this.AutoJump = true;
				this.AutoJumpTimer = 0f;
				this.varJumpTimer = 0.2f;
			}
			this.dashCooldownTimer = 0.2f;
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06001C9F RID: 7327 RVA: 0x000C4630 File Offset: 0x000C2830
		// (set) Token: 0x06001CA0 RID: 7328 RVA: 0x000C4638 File Offset: 0x000C2838
		public bool OnSafeGround { get; private set; }

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06001CA1 RID: 7329 RVA: 0x000C4641 File Offset: 0x000C2841
		public bool LoseShards
		{
			get
			{
				return this.onGround;
			}
		}

		// Token: 0x06001CA2 RID: 7330 RVA: 0x000C464C File Offset: 0x000C284C
		private bool LaunchedBoostCheck()
		{
			if (this.LiftBoost.LengthSquared() >= 10000f && this.Speed.LengthSquared() >= 48400f)
			{
				this.launched = true;
				return true;
			}
			this.launched = false;
			return false;
		}

		// Token: 0x06001CA3 RID: 7331 RVA: 0x000C4694 File Offset: 0x000C2894
		public void HiccupJump()
		{
			switch (this.StateMachine.State)
			{
			default:
				this.StateMachine.State = 0;
				this.Speed.X = Calc.Approach(this.Speed.X, 0f, 40f);
				if (this.Speed.Y > -60f)
				{
					this.varJumpSpeed = (this.Speed.Y = -60f);
					this.varJumpTimer = 0.15f;
					this.AutoJump = true;
					this.AutoJumpTimer = 0f;
					if (this.jumpGraceTimer > 0f)
					{
						this.jumpGraceTimer = 0.6f;
					}
				}
				this.sweatSprite.Play("jump", true, false);
				break;
			case 1:
				this.StateMachine.State = 0;
				this.varJumpSpeed = (this.Speed.Y = -60f);
				this.varJumpTimer = 0.15f;
				this.Speed.X = 130f * (float)(-(float)this.Facing);
				this.AutoJump = true;
				this.AutoJumpTimer = 0f;
				this.sweatSprite.Play("jump", true, false);
				break;
			case 4:
			case 7:
			case 22:
				this.sweatSprite.Play("jump", true, false);
				break;
			case 5:
			case 9:
				if (this.Speed.X < 0f || (this.Speed.X == 0f && this.Speed.Y < 0f))
				{
					this.Speed = this.Speed.Rotate(0.17453292f);
				}
				else
				{
					this.Speed = this.Speed.Rotate(-0.17453292f);
				}
				break;
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 18:
			case 21:
			case 24:
				return;
			case 19:
				if (this.Speed.X > 0f)
				{
					this.Speed = this.Speed.Rotate(0.6981317f);
				}
				else
				{
					this.Speed = this.Speed.Rotate(-0.6981317f);
				}
				break;
			}
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
			this.Play(this.Ducking ? "event:/new_content/char/madeline/hiccup_ducking" : "event:/new_content/char/madeline/hiccup_standing", null, 0f);
		}

		// Token: 0x06001CA4 RID: 7332 RVA: 0x000C491C File Offset: 0x000C2B1C
		public void Jump(bool particles = true, bool playSfx = true)
		{
			Input.Jump.ConsumeBuffer();
			this.jumpGraceTimer = 0f;
			this.varJumpTimer = 0.2f;
			this.AutoJump = false;
			this.dashAttackTimer = 0f;
			this.gliderBoostTimer = 0f;
			this.wallSlideTimer = 1.2f;
			this.wallBoostTimer = 0f;
			this.Speed.X = this.Speed.X + 40f * (float)this.moveX;
			this.Speed.Y = -105f;
			this.Speed += this.LiftBoost;
			this.varJumpSpeed = this.Speed.Y;
			this.LaunchedBoostCheck();
			if (playSfx)
			{
				if (this.launched)
				{
					this.Play("event:/char/madeline/jump_assisted", null, 0f);
				}
				if (this.dreamJump)
				{
					this.Play("event:/char/madeline/jump_dreamblock", null, 0f);
				}
				else
				{
					this.Play("event:/char/madeline/jump", null, 0f);
				}
			}
			this.Sprite.Scale = new Vector2(0.6f, 1.4f);
			if (particles)
			{
				int index = -1;
				Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(base.CollideAll<Platform>(this.Position + Vector2.UnitY, this.temp));
				if (platformByPriority != null)
				{
					index = platformByPriority.GetLandSoundIndex(this);
				}
				Dust.Burst(base.BottomCenter, -1.5707964f, 4, this.DustParticleFromSurfaceIndex(index));
			}
			SaveData.Instance.TotalJumps++;
		}

		// Token: 0x06001CA5 RID: 7333 RVA: 0x000C4A98 File Offset: 0x000C2C98
		private void SuperJump()
		{
			Input.Jump.ConsumeBuffer();
			this.jumpGraceTimer = 0f;
			this.varJumpTimer = 0.2f;
			this.AutoJump = false;
			this.dashAttackTimer = 0f;
			this.gliderBoostTimer = 0f;
			this.wallSlideTimer = 1.2f;
			this.wallBoostTimer = 0f;
			this.Speed.X = 260f * (float)this.Facing;
			this.Speed.Y = -105f;
			this.Speed += this.LiftBoost;
			this.gliderBoostTimer = 0.55f;
			this.Play("event:/char/madeline/jump", null, 0f);
			if (this.Ducking)
			{
				this.Ducking = false;
				this.Speed.X = this.Speed.X * 1.25f;
				this.Speed.Y = this.Speed.Y * 0.5f;
				this.Play("event:/char/madeline/jump_superslide", null, 0f);
				this.gliderBoostDir = Calc.AngleToVector(-0.5890486f, 1f);
			}
			else
			{
				this.gliderBoostDir = Calc.AngleToVector(-0.7853982f, 1f);
				this.Play("event:/char/madeline/jump_super", null, 0f);
			}
			this.varJumpSpeed = this.Speed.Y;
			this.launched = true;
			this.Sprite.Scale = new Vector2(0.6f, 1.4f);
			int index = -1;
			Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(base.CollideAll<Platform>(this.Position + Vector2.UnitY, this.temp));
			if (platformByPriority != null)
			{
				index = platformByPriority.GetLandSoundIndex(this);
			}
			Dust.Burst(base.BottomCenter, -1.5707964f, 4, this.DustParticleFromSurfaceIndex(index));
			SaveData.Instance.TotalJumps++;
		}

		// Token: 0x06001CA6 RID: 7334 RVA: 0x000C4C68 File Offset: 0x000C2E68
		private bool WallJumpCheck(int dir)
		{
			int num = 3;
			bool flag = this.DashAttacking && this.DashDir.X == 0f && this.DashDir.Y == -1f;
			if (flag)
			{
				Spikes.Directions directions;
				if (dir > 0)
				{
					directions = Spikes.Directions.Left;
				}
				else
				{
					directions = Spikes.Directions.Right;
				}
				foreach (Entity entity in this.level.Tracker.GetEntities<Spikes>())
				{
					Spikes spikes = (Spikes)entity;
					if (spikes.Direction == directions && base.CollideCheck(spikes, this.Position + Vector2.UnitX * (float)dir * 5f))
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				num = 5;
			}
			return this.ClimbBoundsCheck(dir) && !ClimbBlocker.EdgeCheck(this.level, this, dir * num) && base.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float)dir * (float)num);
		}

		// Token: 0x06001CA7 RID: 7335 RVA: 0x000C4D84 File Offset: 0x000C2F84
		private void WallJump(int dir)
		{
			this.Ducking = false;
			Input.Jump.ConsumeBuffer();
			this.jumpGraceTimer = 0f;
			this.varJumpTimer = 0.2f;
			this.AutoJump = false;
			this.dashAttackTimer = 0f;
			this.gliderBoostTimer = 0f;
			this.wallSlideTimer = 1.2f;
			this.wallBoostTimer = 0f;
			this.lowFrictionStopTimer = 0.15f;
			if (this.Holding != null && this.Holding.SlowFall)
			{
				this.forceMoveX = dir;
				this.forceMoveXTimer = 0.26f;
			}
			else if (this.moveX != 0)
			{
				this.forceMoveX = dir;
				this.forceMoveXTimer = 0.16f;
			}
			if (base.LiftSpeed == Vector2.Zero)
			{
				Solid solid = base.CollideFirst<Solid>(this.Position + Vector2.UnitX * 3f * (float)(-(float)dir));
				if (solid != null)
				{
					base.LiftSpeed = solid.LiftSpeed;
				}
			}
			this.Speed.X = 130f * (float)dir;
			this.Speed.Y = -105f;
			this.Speed += this.LiftBoost;
			this.varJumpSpeed = this.Speed.Y;
			this.LaunchedBoostCheck();
			int num = -1;
			Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(base.CollideAll<Platform>(this.Position - Vector2.UnitX * (float)dir * 4f, this.temp));
			if (platformByPriority != null)
			{
				num = platformByPriority.GetWallSoundIndex(this, -dir);
				this.Play("event:/char/madeline/landing", "surface_index", (float)num);
				if (platformByPriority is DreamBlock)
				{
					(platformByPriority as DreamBlock).FootstepRipple(this.Position + new Vector2((float)(dir * 3), -4f));
				}
			}
			this.Play((dir < 0) ? "event:/char/madeline/jump_wall_right" : "event:/char/madeline/jump_wall_left", null, 0f);
			this.Sprite.Scale = new Vector2(0.6f, 1.4f);
			if (dir == -1)
			{
				Dust.Burst(base.Center + Vector2.UnitX * 2f, -2.3561945f, 4, this.DustParticleFromSurfaceIndex(num));
			}
			else
			{
				Dust.Burst(base.Center + Vector2.UnitX * -2f, -0.7853982f, 4, this.DustParticleFromSurfaceIndex(num));
			}
			SaveData.Instance.TotalWallJumps++;
		}

		// Token: 0x06001CA8 RID: 7336 RVA: 0x000C5000 File Offset: 0x000C3200
		private void SuperWallJump(int dir)
		{
			this.Ducking = false;
			Input.Jump.ConsumeBuffer();
			this.jumpGraceTimer = 0f;
			this.varJumpTimer = 0.25f;
			this.AutoJump = false;
			this.dashAttackTimer = 0f;
			this.gliderBoostTimer = 0.55f;
			this.gliderBoostDir = -Vector2.UnitY;
			this.wallSlideTimer = 1.2f;
			this.wallBoostTimer = 0f;
			this.Speed.X = 170f * (float)dir;
			this.Speed.Y = -160f;
			this.Speed += this.LiftBoost;
			this.varJumpSpeed = this.Speed.Y;
			this.launched = true;
			this.Play((dir < 0) ? "event:/char/madeline/jump_wall_right" : "event:/char/madeline/jump_wall_left", null, 0f);
			this.Play("event:/char/madeline/jump_superwall", null, 0f);
			this.Sprite.Scale = new Vector2(0.6f, 1.4f);
			int index = -1;
			Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(base.CollideAll<Platform>(this.Position - Vector2.UnitX * (float)dir * 4f, this.temp));
			if (platformByPriority != null)
			{
				index = platformByPriority.GetWallSoundIndex(this, dir);
			}
			if (dir == -1)
			{
				Dust.Burst(base.Center + Vector2.UnitX * 2f, -2.3561945f, 4, this.DustParticleFromSurfaceIndex(index));
			}
			else
			{
				Dust.Burst(base.Center + Vector2.UnitX * -2f, -0.7853982f, 4, this.DustParticleFromSurfaceIndex(index));
			}
			SaveData.Instance.TotalWallJumps++;
		}

		// Token: 0x06001CA9 RID: 7337 RVA: 0x000C51C4 File Offset: 0x000C33C4
		private void ClimbJump()
		{
			if (!this.onGround)
			{
				this.Stamina -= 27.5f;
				this.sweatSprite.Play("jump", true, false);
				Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			}
			this.dreamJump = false;
			this.Jump(false, false);
			if (this.moveX == 0)
			{
				this.wallBoostDir = (int)(-(int)this.Facing);
				this.wallBoostTimer = 0.2f;
			}
			int index = -1;
			Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(base.CollideAll<Platform>(this.Position - Vector2.UnitX * (float)this.Facing * 4f, this.temp));
			if (platformByPriority != null)
			{
				index = platformByPriority.GetWallSoundIndex(this, (int)this.Facing);
			}
			if (this.Facing == Facings.Right)
			{
				this.Play("event:/char/madeline/jump_climb_right", null, 0f);
				Dust.Burst(base.Center + Vector2.UnitX * 2f, -2.3561945f, 4, this.DustParticleFromSurfaceIndex(index));
				return;
			}
			this.Play("event:/char/madeline/jump_climb_left", null, 0f);
			Dust.Burst(base.Center + Vector2.UnitX * -2f, -0.7853982f, 4, this.DustParticleFromSurfaceIndex(index));
		}

		// Token: 0x06001CAA RID: 7338 RVA: 0x000C5304 File Offset: 0x000C3504
		public void Bounce(float fromY)
		{
			if (this.StateMachine.State == 4 && this.CurrentBooster != null)
			{
				this.CurrentBooster.PlayerReleased();
				this.CurrentBooster = null;
			}
			Collider collider = base.Collider;
			base.Collider = this.normalHitbox;
			base.MoveVExact((int)(fromY - base.Bottom), null, null);
			if (!this.Inventory.NoRefills)
			{
				this.RefillDash();
			}
			this.RefillStamina();
			this.StateMachine.State = 0;
			this.jumpGraceTimer = 0f;
			this.varJumpTimer = 0.2f;
			this.AutoJump = true;
			this.AutoJumpTimer = 0.1f;
			this.dashAttackTimer = 0f;
			this.gliderBoostTimer = 0f;
			this.wallSlideTimer = 1.2f;
			this.wallBoostTimer = 0f;
			this.varJumpSpeed = (this.Speed.Y = -140f);
			this.launched = false;
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			this.Sprite.Scale = new Vector2(0.6f, 1.4f);
			base.Collider = collider;
		}

		// Token: 0x06001CAB RID: 7339 RVA: 0x000C5420 File Offset: 0x000C3620
		public void SuperBounce(float fromY)
		{
			if (this.StateMachine.State == 4 && this.CurrentBooster != null)
			{
				this.CurrentBooster.PlayerReleased();
				this.CurrentBooster = null;
			}
			Collider collider = base.Collider;
			base.Collider = this.normalHitbox;
			base.MoveV(fromY - base.Bottom, null, null);
			if (!this.Inventory.NoRefills)
			{
				this.RefillDash();
			}
			this.RefillStamina();
			this.StateMachine.State = 0;
			this.jumpGraceTimer = 0f;
			this.varJumpTimer = 0.2f;
			this.AutoJump = true;
			this.AutoJumpTimer = 0f;
			this.dashAttackTimer = 0f;
			this.gliderBoostTimer = 0f;
			this.wallSlideTimer = 1.2f;
			this.wallBoostTimer = 0f;
			this.Speed.X = 0f;
			this.varJumpSpeed = (this.Speed.Y = -185f);
			this.launched = false;
			this.level.DirectionalShake(-Vector2.UnitY, 0.1f);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			this.Sprite.Scale = new Vector2(0.5f, 1.5f);
			base.Collider = collider;
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x000C5568 File Offset: 0x000C3768
		public bool SideBounce(int dir, float fromX, float fromY)
		{
			if (Math.Abs(this.Speed.X) > 240f && Math.Sign(this.Speed.X) == dir)
			{
				return false;
			}
			Collider collider = base.Collider;
			base.Collider = this.normalHitbox;
			base.MoveV(Calc.Clamp(fromY - base.Bottom, -4f, 4f), null, null);
			if (dir > 0)
			{
				base.MoveH(fromX - base.Left, null, null);
			}
			else if (dir < 0)
			{
				base.MoveH(fromX - base.Right, null, null);
			}
			if (!this.Inventory.NoRefills)
			{
				this.RefillDash();
			}
			this.RefillStamina();
			this.StateMachine.State = 0;
			this.jumpGraceTimer = 0f;
			this.varJumpTimer = 0.2f;
			this.AutoJump = true;
			this.AutoJumpTimer = 0f;
			this.dashAttackTimer = 0f;
			this.gliderBoostTimer = 0f;
			this.wallSlideTimer = 1.2f;
			this.forceMoveX = dir;
			this.forceMoveXTimer = 0.3f;
			this.wallBoostTimer = 0f;
			this.launched = false;
			this.Speed.X = 240f * (float)dir;
			this.varJumpSpeed = (this.Speed.Y = -140f);
			this.level.DirectionalShake(Vector2.UnitX * (float)dir, 0.1f);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			this.Sprite.Scale = new Vector2(1.5f, 0.5f);
			base.Collider = collider;
			return true;
		}

		// Token: 0x06001CAD RID: 7341 RVA: 0x000C5704 File Offset: 0x000C3904
		public void Rebound(int direction = 0)
		{
			this.Speed.X = (float)direction * 120f;
			this.Speed.Y = -120f;
			this.varJumpSpeed = this.Speed.Y;
			this.varJumpTimer = 0.15f;
			this.AutoJump = true;
			this.AutoJumpTimer = 0f;
			this.dashAttackTimer = 0f;
			this.gliderBoostTimer = 0f;
			this.wallSlideTimer = 1.2f;
			this.wallBoostTimer = 0f;
			this.launched = false;
			this.lowFrictionStopTimer = 0.15f;
			this.forceMoveXTimer = 0f;
			this.StateMachine.State = 0;
		}

		// Token: 0x06001CAE RID: 7342 RVA: 0x000C57B8 File Offset: 0x000C39B8
		public void ReflectBounce(Vector2 direction)
		{
			if (direction.X != 0f)
			{
				this.Speed.X = direction.X * 220f;
			}
			if (direction.Y != 0f)
			{
				this.Speed.Y = direction.Y * 220f;
			}
			this.AutoJumpTimer = 0f;
			this.dashAttackTimer = 0f;
			this.gliderBoostTimer = 0f;
			this.wallSlideTimer = 1.2f;
			this.wallBoostTimer = 0f;
			this.launched = false;
			this.dashAttackTimer = 0f;
			this.gliderBoostTimer = 0f;
			this.forceMoveXTimer = 0f;
			this.StateMachine.State = 0;
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06001CAF RID: 7343 RVA: 0x000C5878 File Offset: 0x000C3A78
		public int MaxDashes
		{
			get
			{
				if (SaveData.Instance.Assists.DashMode != Assists.DashModes.Normal && !this.level.InCutscene)
				{
					return 2;
				}
				return this.Inventory.Dashes;
			}
		}

		// Token: 0x06001CB0 RID: 7344 RVA: 0x000C58A5 File Offset: 0x000C3AA5
		public bool RefillDash()
		{
			if (this.Dashes < this.MaxDashes)
			{
				this.Dashes = this.MaxDashes;
				return true;
			}
			return false;
		}

		// Token: 0x06001CB1 RID: 7345 RVA: 0x000C58C4 File Offset: 0x000C3AC4
		public bool UseRefill(bool twoDashes)
		{
			int num = this.MaxDashes;
			if (twoDashes)
			{
				num = 2;
			}
			if (this.Dashes < num || this.Stamina < 20f)
			{
				this.Dashes = num;
				this.RefillStamina();
				return true;
			}
			return false;
		}

		// Token: 0x06001CB2 RID: 7346 RVA: 0x000C5903 File Offset: 0x000C3B03
		public void RefillStamina()
		{
			this.Stamina = 110f;
		}

		// Token: 0x06001CB3 RID: 7347 RVA: 0x000C5910 File Offset: 0x000C3B10
		public PlayerDeadBody Die(Vector2 direction, bool evenIfInvincible = false, bool registerDeathInStats = true)
		{
			Session session = this.level.Session;
			bool flag = !evenIfInvincible && SaveData.Instance.Assists.Invincible;
			if (!this.Dead && !flag && this.StateMachine.State != 18)
			{
				this.Stop(this.wallSlideSfx);
				if (registerDeathInStats)
				{
					session.Deaths++;
					session.DeathsInCurrentLevel++;
					SaveData.Instance.AddDeath(session.Area);
				}
				Strawberry goldenStrawb = null;
				foreach (Follower follower in this.Leader.Followers)
				{
					if (follower.Entity is Strawberry && (follower.Entity as Strawberry).Golden && !(follower.Entity as Strawberry).Winged)
					{
						goldenStrawb = (follower.Entity as Strawberry);
					}
				}
				this.Dead = true;
				this.Leader.LoseFollowers();
				base.Depth = -1000000;
				this.Speed = Vector2.Zero;
				this.StateMachine.Locked = true;
				this.Collidable = false;
				this.Drop();
				if (this.LastBooster != null)
				{
					this.LastBooster.PlayerDied();
				}
				this.level.InCutscene = false;
				this.level.Shake(0.3f);
				Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
				PlayerDeadBody playerDeadBody = new PlayerDeadBody(this, direction);
				if (goldenStrawb != null)
				{
					playerDeadBody.HasGolden = true;
					playerDeadBody.DeathAction = delegate()
					{
						Engine.Scene = new LevelExit(LevelExit.Mode.GoldenBerryRestart, session, null)
						{
							GoldenStrawberryEntryLevel = goldenStrawb.ID.Level
						};
					};
				}
				base.Scene.Add(playerDeadBody);
				base.Scene.Remove(this);
				Lookout entity = base.Scene.Tracker.GetEntity<Lookout>();
				if (entity != null)
				{
					entity.StopInteracting();
				}
				return playerDeadBody;
			}
			return null;
		}

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06001CB4 RID: 7348 RVA: 0x000C5B40 File Offset: 0x000C3D40
		private Vector2 LiftBoost
		{
			get
			{
				Vector2 liftSpeed = base.LiftSpeed;
				if (Math.Abs(liftSpeed.X) > 250f)
				{
					liftSpeed.X = 250f * (float)Math.Sign(liftSpeed.X);
				}
				if (liftSpeed.Y > 0f)
				{
					liftSpeed.Y = 0f;
				}
				else if (liftSpeed.Y < -130f)
				{
					liftSpeed.Y = -130f;
				}
				return liftSpeed;
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06001CB5 RID: 7349 RVA: 0x000C5BB4 File Offset: 0x000C3DB4
		// (set) Token: 0x06001CB6 RID: 7350 RVA: 0x000C5BD4 File Offset: 0x000C3DD4
		public bool Ducking
		{
			get
			{
				return base.Collider == this.duckHitbox || base.Collider == this.duckHurtbox;
			}
			set
			{
				if (value)
				{
					base.Collider = this.duckHitbox;
					this.hurtbox = this.duckHurtbox;
					return;
				}
				base.Collider = this.normalHitbox;
				this.hurtbox = this.normalHurtbox;
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06001CB7 RID: 7351 RVA: 0x000C5C0C File Offset: 0x000C3E0C
		public bool CanUnDuck
		{
			get
			{
				if (!this.Ducking)
				{
					return true;
				}
				Collider collider = base.Collider;
				base.Collider = this.normalHitbox;
				bool result = !base.CollideCheck<Solid>();
				base.Collider = collider;
				return result;
			}
		}

		// Token: 0x06001CB8 RID: 7352 RVA: 0x000C5C48 File Offset: 0x000C3E48
		public bool CanUnDuckAt(Vector2 at)
		{
			Vector2 position = this.Position;
			this.Position = at;
			bool canUnDuck = this.CanUnDuck;
			this.Position = position;
			return canUnDuck;
		}

		// Token: 0x06001CB9 RID: 7353 RVA: 0x000C5C70 File Offset: 0x000C3E70
		public bool DuckFreeAt(Vector2 at)
		{
			Vector2 position = this.Position;
			Collider collider = base.Collider;
			this.Position = at;
			base.Collider = this.duckHitbox;
			bool result = !base.CollideCheck<Solid>();
			this.Position = position;
			base.Collider = collider;
			return result;
		}

		// Token: 0x06001CBA RID: 7354 RVA: 0x000C5CB5 File Offset: 0x000C3EB5
		private void Duck()
		{
			base.Collider = this.duckHitbox;
		}

		// Token: 0x06001CBB RID: 7355 RVA: 0x000C5CC3 File Offset: 0x000C3EC3
		private void UnDuck()
		{
			base.Collider = this.normalHitbox;
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06001CBC RID: 7356 RVA: 0x000C5CD1 File Offset: 0x000C3ED1
		// (set) Token: 0x06001CBD RID: 7357 RVA: 0x000C5CD9 File Offset: 0x000C3ED9
		public Holdable Holding { get; set; }

		// Token: 0x06001CBE RID: 7358 RVA: 0x000C5CE4 File Offset: 0x000C3EE4
		public void UpdateCarry()
		{
			if (this.Holding != null)
			{
				if (this.Holding.Scene == null)
				{
					this.Holding = null;
					return;
				}
				this.Holding.Carry(this.Position + this.carryOffset + Vector2.UnitY * this.Sprite.CarryYOffset);
			}
		}

		// Token: 0x06001CBF RID: 7359 RVA: 0x000C5D44 File Offset: 0x000C3F44
		public void Swat(int dir)
		{
			if (this.Holding != null)
			{
				this.Holding.Release(new Vector2(0.8f * (float)dir, -0.25f));
				this.Holding = null;
			}
		}

		// Token: 0x06001CC0 RID: 7360 RVA: 0x000C5D72 File Offset: 0x000C3F72
		private bool Pickup(Holdable pickup)
		{
			if (pickup.Pickup(this))
			{
				this.Ducking = false;
				this.Holding = pickup;
				this.minHoldTimer = 0.35f;
				return true;
			}
			return false;
		}

		// Token: 0x06001CC1 RID: 7361 RVA: 0x000C5D9C File Offset: 0x000C3F9C
		public void Throw()
		{
			if (this.Holding != null)
			{
				if (Input.MoveY.Value == 1)
				{
					this.Drop();
				}
				else
				{
					Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
					this.Holding.Release(Vector2.UnitX * (float)this.Facing);
					this.Speed.X = this.Speed.X + 80f * (float)(-(float)this.Facing);
					this.Play("event:/char/madeline/crystaltheo_throw", null, 0f);
					this.Sprite.Play("throw", false, false);
				}
				this.Holding = null;
			}
		}

		// Token: 0x06001CC2 RID: 7362 RVA: 0x000C5E34 File Offset: 0x000C4034
		public void Drop()
		{
			if (this.Holding != null)
			{
				Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
				this.Holding.Release(Vector2.Zero);
				this.Holding = null;
			}
		}

		// Token: 0x06001CC3 RID: 7363 RVA: 0x000C5E5C File Offset: 0x000C405C
		public void StartJumpGraceTime()
		{
			this.jumpGraceTimer = 0.1f;
		}

		// Token: 0x06001CC4 RID: 7364 RVA: 0x000C5E6C File Offset: 0x000C406C
		public override bool IsRiding(Solid solid)
		{
			if (this.StateMachine.State == 23)
			{
				return false;
			}
			if (this.StateMachine.State == 9)
			{
				return base.CollideCheck(solid);
			}
			if (this.StateMachine.State == 1 || this.StateMachine.State == 6)
			{
				return base.CollideCheck(solid, this.Position + Vector2.UnitX * (float)this.Facing);
			}
			if (this.climbTriggerDir != 0)
			{
				return base.CollideCheck(solid, this.Position + Vector2.UnitX * (float)this.climbTriggerDir);
			}
			return base.IsRiding(solid);
		}

		// Token: 0x06001CC5 RID: 7365 RVA: 0x000C5F14 File Offset: 0x000C4114
		public override bool IsRiding(JumpThru jumpThru)
		{
			return this.StateMachine.State != 9 && (this.StateMachine.State != 1 && this.Speed.Y >= 0f) && base.IsRiding(jumpThru);
		}

		// Token: 0x06001CC6 RID: 7366 RVA: 0x000C5F50 File Offset: 0x000C4150
		public bool BounceCheck(float y)
		{
			return base.Bottom <= y + 3f;
		}

		// Token: 0x06001CC7 RID: 7367 RVA: 0x000C5F64 File Offset: 0x000C4164
		public void PointBounce(Vector2 from)
		{
			if (this.StateMachine.State == 2)
			{
				this.StateMachine.State = 0;
			}
			if (this.StateMachine.State == 4 && this.CurrentBooster != null)
			{
				this.CurrentBooster.PlayerReleased();
			}
			this.RefillDash();
			this.RefillStamina();
			Vector2 vector = (base.Center - from).SafeNormalize();
			if (vector.Y > -0.2f && vector.Y <= 0.4f)
			{
				vector.Y = -0.2f;
			}
			this.Speed = vector * 220f;
			this.Speed.X = this.Speed.X * 1.5f;
			if (Math.Abs(this.Speed.X) < 100f)
			{
				if (this.Speed.X == 0f)
				{
					this.Speed.X = (float)(-(float)this.Facing) * 100f;
					return;
				}
				this.Speed.X = (float)Math.Sign(this.Speed.X) * 100f;
			}
		}

		// Token: 0x06001CC8 RID: 7368 RVA: 0x000C607C File Offset: 0x000C427C
		private void WindMove(Vector2 move)
		{
			if (!this.JustRespawned && this.noWindTimer <= 0f && this.InControl && this.StateMachine.State != 4 && this.StateMachine.State != 2 && this.StateMachine.State != 10)
			{
				if (move.X != 0f && this.StateMachine.State != 1)
				{
					this.windTimeout = 0.2f;
					this.windDirection.X = (float)Math.Sign(move.X);
					if (!base.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float)(-(float)Math.Sign(move.X)) * 3f))
					{
						if (this.Ducking && this.onGround)
						{
							move.X *= 0f;
						}
						if (move.X < 0f)
						{
							move.X = Math.Max(move.X, (float)this.level.Bounds.Left - (base.ExactPosition.X + base.Collider.Left));
						}
						else
						{
							move.X = Math.Min(move.X, (float)this.level.Bounds.Right - (base.ExactPosition.X + base.Collider.Right));
						}
						base.MoveH(move.X, null, null);
					}
				}
				if (move.Y != 0f)
				{
					this.windTimeout = 0.2f;
					this.windDirection.Y = (float)Math.Sign(move.Y);
					if (base.Bottom > (float)this.level.Bounds.Top && (this.Speed.Y < 0f || !base.OnGround(1)))
					{
						if (this.StateMachine.State == 1)
						{
							if (move.Y <= 0f || this.climbNoMoveTimer > 0f)
							{
								return;
							}
							move.Y *= 0.4f;
						}
						if (move.Y < 0f)
						{
							this.windMovedUp = true;
						}
						base.MoveV(move.Y, null, null);
					}
				}
			}
		}

		// Token: 0x06001CC9 RID: 7369 RVA: 0x000C62E0 File Offset: 0x000C44E0
		private void OnCollideH(CollisionData data)
		{
			this.canCurveDash = false;
			if (this.StateMachine.State == 19)
			{
				if (this.starFlyTimer < 0.2f)
				{
					this.Speed.X = 0f;
					return;
				}
				this.Play("event:/game/06_reflection/feather_state_bump", null, 0f);
				Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
				this.Speed.X = this.Speed.X * -0.5f;
				return;
			}
			else
			{
				if (this.StateMachine.State == 9)
				{
					return;
				}
				if (this.DashAttacking && data.Hit != null && data.Hit.OnDashCollide != null && data.Direction.X == (float)Math.Sign(this.DashDir.X))
				{
					DashCollisionResults dashCollisionResults = data.Hit.OnDashCollide(this, data.Direction);
					if (dashCollisionResults == DashCollisionResults.NormalOverride)
					{
						dashCollisionResults = DashCollisionResults.NormalCollision;
					}
					else if (this.StateMachine.State == 5)
					{
						dashCollisionResults = DashCollisionResults.Ignore;
					}
					if (dashCollisionResults == DashCollisionResults.Rebound)
					{
						this.Rebound(-Math.Sign(this.Speed.X));
						return;
					}
					if (dashCollisionResults == DashCollisionResults.Bounce)
					{
						this.ReflectBounce(new Vector2((float)(-(float)Math.Sign(this.Speed.X)), 0f));
						return;
					}
					if (dashCollisionResults == DashCollisionResults.Ignore)
					{
						return;
					}
				}
				if (this.StateMachine.State == 2 || this.StateMachine.State == 5)
				{
					if (this.onGround && this.DuckFreeAt(this.Position + Vector2.UnitX * (float)Math.Sign(this.Speed.X)))
					{
						this.Ducking = true;
						return;
					}
					if (this.Speed.Y == 0f && this.Speed.X != 0f)
					{
						for (int i = 1; i <= 4; i++)
						{
							for (int j = 1; j >= -1; j -= 2)
							{
								Vector2 vector = new Vector2((float)Math.Sign(this.Speed.X), (float)(i * j));
								Vector2 vector2 = this.Position + vector;
								if (!base.CollideCheck<Solid>(vector2) && base.CollideCheck<Solid>(vector2 - Vector2.UnitY * (float)j) && !this.DashCorrectCheck(vector))
								{
									base.MoveVExact(i * j, null, null);
									base.MoveHExact(Math.Sign(this.Speed.X), null, null);
									return;
								}
							}
						}
					}
				}
				if (this.DreamDashCheck(Vector2.UnitX * (float)Math.Sign(this.Speed.X)))
				{
					this.StateMachine.State = 9;
					this.dashAttackTimer = 0f;
					this.gliderBoostTimer = 0f;
					return;
				}
				if (this.wallSpeedRetentionTimer <= 0f)
				{
					this.wallSpeedRetained = this.Speed.X;
					this.wallSpeedRetentionTimer = 0.06f;
				}
				if (data.Hit != null && data.Hit.OnCollide != null)
				{
					data.Hit.OnCollide(data.Direction);
				}
				this.Speed.X = 0f;
				this.dashAttackTimer = 0f;
				this.gliderBoostTimer = 0f;
				if (this.StateMachine.State == 5)
				{
					Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
					this.level.Displacement.AddBurst(base.Center, 0.5f, 8f, 48f, 0.4f, Ease.QuadOut, Ease.QuadOut);
					this.StateMachine.State = 6;
				}
				return;
			}
		}

		// Token: 0x06001CCA RID: 7370 RVA: 0x000C665C File Offset: 0x000C485C
		private void OnCollideV(CollisionData data)
		{
			this.canCurveDash = false;
			if (this.StateMachine.State == 19)
			{
				if (this.starFlyTimer < 0.2f)
				{
					this.Speed.Y = 0f;
					return;
				}
				this.Play("event:/game/06_reflection/feather_state_bump", null, 0f);
				Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
				this.Speed.Y = this.Speed.Y * -0.5f;
				return;
			}
			else
			{
				if (this.StateMachine.State == 3)
				{
					this.Speed.Y = 0f;
					return;
				}
				if (this.StateMachine.State == 9)
				{
					return;
				}
				if (data.Hit != null && data.Hit.OnDashCollide != null)
				{
					if (this.DashAttacking && data.Direction.Y == (float)Math.Sign(this.DashDir.Y))
					{
						DashCollisionResults dashCollisionResults = data.Hit.OnDashCollide(this, data.Direction);
						if (this.StateMachine.State == 5)
						{
							dashCollisionResults = DashCollisionResults.Ignore;
						}
						if (dashCollisionResults == DashCollisionResults.Rebound)
						{
							this.Rebound(0);
							return;
						}
						if (dashCollisionResults == DashCollisionResults.Bounce)
						{
							this.ReflectBounce(new Vector2(0f, (float)(-(float)Math.Sign(this.Speed.Y))));
							return;
						}
						if (dashCollisionResults == DashCollisionResults.Ignore)
						{
							return;
						}
					}
					else if (this.StateMachine.State == 10)
					{
						data.Hit.OnDashCollide(this, data.Direction);
						return;
					}
				}
				if (this.Speed.Y > 0f)
				{
					if ((this.StateMachine.State == 2 || this.StateMachine.State == 5) && !this.dashStartedOnGround)
					{
						if (this.Speed.X <= 0.01f)
						{
							for (int i = -1; i >= -4; i--)
							{
								if (!base.OnGround(this.Position + new Vector2((float)i, 0f), 1))
								{
									base.MoveHExact(i, null, null);
									base.MoveVExact(1, null, null);
									return;
								}
							}
						}
						if (this.Speed.X >= -0.01f)
						{
							for (int j = 1; j <= 4; j++)
							{
								if (!base.OnGround(this.Position + new Vector2((float)j, 0f), 1))
								{
									base.MoveHExact(j, null, null);
									base.MoveVExact(1, null, null);
									return;
								}
							}
						}
					}
					if (this.DreamDashCheck(Vector2.UnitY * (float)Math.Sign(this.Speed.Y)))
					{
						this.StateMachine.State = 9;
						this.dashAttackTimer = 0f;
						this.gliderBoostTimer = 0f;
						return;
					}
					if (this.DashDir.X != 0f && this.DashDir.Y > 0f && this.Speed.Y > 0f)
					{
						this.DashDir.X = (float)Math.Sign(this.DashDir.X);
						this.DashDir.Y = 0f;
						this.Speed.Y = 0f;
						this.Speed.X = this.Speed.X * 1.2f;
						this.Ducking = true;
					}
					if (this.StateMachine.State != 1)
					{
						float amount = Math.Min(this.Speed.Y / 240f, 1f);
						this.Sprite.Scale.X = MathHelper.Lerp(1f, 1.6f, amount);
						this.Sprite.Scale.Y = MathHelper.Lerp(1f, 0.4f, amount);
						if (this.highestAirY < base.Y - 50f && this.Speed.Y >= 160f && Math.Abs(this.Speed.X) >= 90f)
						{
							this.Sprite.Play("runStumble", false, false);
						}
						Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
						Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(base.CollideAll<Platform>(this.Position + new Vector2(0f, 1f), this.temp));
						int num = -1;
						if (platformByPriority != null)
						{
							num = platformByPriority.GetLandSoundIndex(this);
							if (num >= 0 && !this.MuffleLanding)
							{
								this.Play((this.playFootstepOnLand > 0f) ? "event:/char/madeline/footstep" : "event:/char/madeline/landing", "surface_index", (float)num);
							}
							if (platformByPriority is DreamBlock)
							{
								(platformByPriority as DreamBlock).FootstepRipple(this.Position);
							}
							this.MuffleLanding = false;
						}
						if (this.Speed.Y >= 80f)
						{
							Dust.Burst(this.Position, new Vector2(0f, -1f).Angle(), 8, this.DustParticleFromSurfaceIndex(num));
						}
						this.playFootstepOnLand = 0f;
					}
				}
				else
				{
					if (this.Speed.Y < 0f)
					{
						int num2 = 4;
						if (this.DashAttacking && Math.Abs(this.Speed.X) < 0.01f)
						{
							num2 = 5;
						}
						if (this.Speed.X <= 0.01f)
						{
							for (int k = 1; k <= num2; k++)
							{
								if (!base.CollideCheck<Solid>(this.Position + new Vector2((float)(-(float)k), -1f)))
								{
									this.Position += new Vector2((float)(-(float)k), -1f);
									return;
								}
							}
						}
						if (this.Speed.X >= -0.01f)
						{
							for (int l = 1; l <= num2; l++)
							{
								if (!base.CollideCheck<Solid>(this.Position + new Vector2((float)l, -1f)))
								{
									this.Position += new Vector2((float)l, -1f);
									return;
								}
							}
						}
						if (this.varJumpTimer < 0.15f)
						{
							this.varJumpTimer = 0f;
						}
					}
					if (this.DreamDashCheck(Vector2.UnitY * (float)Math.Sign(this.Speed.Y)))
					{
						this.StateMachine.State = 9;
						this.dashAttackTimer = 0f;
						this.gliderBoostTimer = 0f;
						return;
					}
				}
				if (data.Hit != null && data.Hit.OnCollide != null)
				{
					data.Hit.OnCollide(data.Direction);
				}
				this.dashAttackTimer = 0f;
				this.gliderBoostTimer = 0f;
				this.Speed.Y = 0f;
				if (this.StateMachine.State == 5)
				{
					Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
					this.level.Displacement.AddBurst(base.Center, 0.5f, 8f, 48f, 0.4f, Ease.QuadOut, Ease.QuadOut);
					this.StateMachine.State = 6;
				}
				return;
			}
		}

		// Token: 0x06001CCB RID: 7371 RVA: 0x000C6D24 File Offset: 0x000C4F24
		private bool DreamDashCheck(Vector2 dir)
		{
			if (this.Inventory.DreamDash && this.DashAttacking && (dir.X == (float)Math.Sign(this.DashDir.X) || dir.Y == (float)Math.Sign(this.DashDir.Y)))
			{
				DreamBlock dreamBlock = base.CollideFirst<DreamBlock>(this.Position + dir);
				if (dreamBlock != null)
				{
					if (base.CollideCheck<Solid, DreamBlock>(this.Position + dir))
					{
						Vector2 value = new Vector2(Math.Abs(dir.Y), Math.Abs(dir.X));
						bool flag;
						bool flag2;
						if (dir.X != 0f)
						{
							flag = (this.Speed.Y <= 0f);
							flag2 = (this.Speed.Y >= 0f);
						}
						else
						{
							flag = (this.Speed.X <= 0f);
							flag2 = (this.Speed.X >= 0f);
						}
						if (flag)
						{
							for (int i = -1; i >= -4; i--)
							{
								Vector2 at = this.Position + dir + value * (float)i;
								if (!base.CollideCheck<Solid, DreamBlock>(at))
								{
									this.Position += value * (float)i;
									this.dreamBlock = dreamBlock;
									return true;
								}
							}
						}
						if (flag2)
						{
							for (int j = 1; j <= 4; j++)
							{
								Vector2 at2 = this.Position + dir + value * (float)j;
								if (!base.CollideCheck<Solid, DreamBlock>(at2))
								{
									this.Position += value * (float)j;
									this.dreamBlock = dreamBlock;
									return true;
								}
							}
						}
						return false;
					}
					this.dreamBlock = dreamBlock;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001CCC RID: 7372 RVA: 0x000C6EFA File Offset: 0x000C50FA
		public void OnBoundsH()
		{
			this.Speed.X = 0f;
			if (this.StateMachine.State == 5)
			{
				this.StateMachine.State = 0;
			}
		}

		// Token: 0x06001CCD RID: 7373 RVA: 0x000C6F26 File Offset: 0x000C5126
		public void OnBoundsV()
		{
			this.Speed.Y = 0f;
			if (this.StateMachine.State == 5)
			{
				this.StateMachine.State = 0;
			}
		}

		// Token: 0x06001CCE RID: 7374 RVA: 0x000C6F54 File Offset: 0x000C5154
		protected override void OnSquish(CollisionData data)
		{
			bool flag = false;
			if (!this.Ducking && this.StateMachine.State != 1)
			{
				flag = true;
				this.Ducking = true;
				data.Pusher.Collidable = true;
				if (!base.CollideCheck<Solid>())
				{
					data.Pusher.Collidable = false;
					return;
				}
				Vector2 position = this.Position;
				this.Position = data.TargetPosition;
				if (!base.CollideCheck<Solid>())
				{
					data.Pusher.Collidable = false;
					return;
				}
				this.Position = position;
				data.Pusher.Collidable = false;
			}
			if (!base.TrySquishWiggle(data, 3, 5))
			{
				bool evenIfInvincible = false;
				if (data.Pusher != null && data.Pusher.SquishEvenInAssistMode)
				{
					evenIfInvincible = true;
				}
				this.Die(Vector2.Zero, evenIfInvincible, true);
				return;
			}
			if (flag && this.CanUnDuck)
			{
				this.Ducking = false;
			}
		}

		// Token: 0x06001CCF RID: 7375 RVA: 0x000C7023 File Offset: 0x000C5223
		private void NormalBegin()
		{
			this.maxFall = 160f;
		}

		// Token: 0x06001CD0 RID: 7376 RVA: 0x000C7030 File Offset: 0x000C5230
		private void NormalEnd()
		{
			this.wallBoostTimer = 0f;
			this.wallSpeedRetentionTimer = 0f;
			this.hopWaitX = 0;
		}

		// Token: 0x06001CD1 RID: 7377 RVA: 0x000C7050 File Offset: 0x000C5250
		public bool ClimbBoundsCheck(int dir)
		{
			return base.Left + (float)(dir * 2) >= (float)this.level.Bounds.Left && base.Right + (float)(dir * 2) < (float)this.level.Bounds.Right;
		}

		// Token: 0x06001CD2 RID: 7378 RVA: 0x000C70A1 File Offset: 0x000C52A1
		public void ClimbTrigger(int dir)
		{
			this.climbTriggerDir = dir;
		}

		// Token: 0x06001CD3 RID: 7379 RVA: 0x000C70AC File Offset: 0x000C52AC
		public bool ClimbCheck(int dir, int yAdd = 0)
		{
			return this.ClimbBoundsCheck(dir) && !ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitY * (float)yAdd + Vector2.UnitX * 2f * (float)this.Facing) && base.CollideCheck<Solid>(this.Position + new Vector2((float)(dir * 2), (float)yAdd));
		}

		// Token: 0x06001CD4 RID: 7380 RVA: 0x000C7128 File Offset: 0x000C5328
		private int NormalUpdate()
		{
			if (this.LiftBoost.Y < 0f && this.wasOnGround && !this.onGround && this.Speed.Y >= 0f)
			{
				this.Speed.Y = this.LiftBoost.Y;
			}
			if (this.Holding == null)
			{
				if (Input.GrabCheck && !this.IsTired && !this.Ducking)
				{
					foreach (Component component in base.Scene.Tracker.GetComponents<Holdable>())
					{
						Holdable holdable = (Holdable)component;
						if (holdable.Check(this) && this.Pickup(holdable))
						{
							return 8;
						}
					}
					if (this.Speed.Y < 0f || Math.Sign(this.Speed.X) == (int)(-(int)this.Facing))
					{
						goto IL_1BD;
					}
					if (this.ClimbCheck((int)this.Facing, 0))
					{
						this.Ducking = false;
						if (!SaveData.Instance.Assists.NoGrabbing)
						{
							return 1;
						}
						this.ClimbTrigger((int)this.Facing);
					}
					if (!SaveData.Instance.Assists.NoGrabbing && Input.MoveY < 1f && this.level.Wind.Y <= 0f)
					{
						for (int i = 1; i <= 2; i++)
						{
							if (!base.CollideCheck<Solid>(this.Position + Vector2.UnitY * (float)(-(float)i)) && this.ClimbCheck((int)this.Facing, -i))
							{
								base.MoveVExact(-i, null, null);
								this.Ducking = false;
								return 1;
							}
						}
					}
				}
				IL_1BD:
				if (this.CanDash)
				{
					this.Speed += this.LiftBoost;
					return this.StartDash();
				}
				if (this.Ducking)
				{
					if (this.onGround && Input.MoveY != 1f)
					{
						if (this.CanUnDuck)
						{
							this.Ducking = false;
							this.Sprite.Scale = new Vector2(0.8f, 1.2f);
						}
						else if (this.Speed.X == 0f)
						{
							for (int j = 4; j > 0; j--)
							{
								if (this.CanUnDuckAt(this.Position + Vector2.UnitX * (float)j))
								{
									base.MoveH(50f * Engine.DeltaTime, null, null);
									break;
								}
								if (this.CanUnDuckAt(this.Position - Vector2.UnitX * (float)j))
								{
									base.MoveH(-50f * Engine.DeltaTime, null, null);
									break;
								}
							}
						}
					}
				}
				else if (this.onGround && Input.MoveY == 1f && this.Speed.Y >= 0f)
				{
					this.Ducking = true;
					this.Sprite.Scale = new Vector2(1.4f, 0.6f);
				}
			}
			else
			{
				if (!Input.GrabCheck && this.minHoldTimer <= 0f)
				{
					this.Throw();
				}
				if (!this.Ducking && this.onGround && Input.MoveY == 1f && this.Speed.Y >= 0f && !this.holdCannotDuck)
				{
					this.Drop();
					this.Ducking = true;
					this.Sprite.Scale = new Vector2(1.4f, 0.6f);
				}
				else if (this.onGround && this.Ducking && this.Speed.Y >= 0f)
				{
					if (this.CanUnDuck)
					{
						this.Ducking = false;
					}
					else
					{
						this.Drop();
					}
				}
				else if (this.onGround && Input.MoveY != 1f && this.holdCannotDuck)
				{
					this.holdCannotDuck = false;
				}
			}
			if (this.Ducking && this.onGround)
			{
				this.Speed.X = Calc.Approach(this.Speed.X, 0f, 500f * Engine.DeltaTime);
			}
			else
			{
				float num = this.onGround ? 1f : 0.65f;
				if (this.onGround && this.level.CoreMode == Session.CoreModes.Cold)
				{
					num *= 0.3f;
				}
				if (SaveData.Instance.Assists.LowFriction && this.lowFrictionStopTimer <= 0f)
				{
					num *= (this.onGround ? 0.35f : 0.5f);
				}
				float num2;
				if (this.Holding != null && this.Holding.SlowRun)
				{
					num2 = 70f;
				}
				else if (this.Holding != null && this.Holding.SlowFall && !this.onGround)
				{
					num2 = 108.00001f;
					num *= 0.5f;
				}
				else
				{
					num2 = 90f;
				}
				if (this.level.InSpace)
				{
					num2 *= 0.6f;
				}
				if (Math.Abs(this.Speed.X) > num2 && Math.Sign(this.Speed.X) == this.moveX)
				{
					this.Speed.X = Calc.Approach(this.Speed.X, num2 * (float)this.moveX, 400f * num * Engine.DeltaTime);
				}
				else
				{
					this.Speed.X = Calc.Approach(this.Speed.X, num2 * (float)this.moveX, 1000f * num * Engine.DeltaTime);
				}
			}
			float num3 = 160f;
			float num4 = 240f;
			if (this.level.InSpace)
			{
				num3 *= 0.6f;
				num4 *= 0.6f;
			}
			if (this.Holding != null && this.Holding.SlowFall && this.forceMoveXTimer <= 0f)
			{
				if (Input.GliderMoveY == 1f)
				{
					num3 = 120f;
				}
				else if (this.windMovedUp && Input.GliderMoveY == -1f)
				{
					num3 = -32f;
				}
				else if (Input.GliderMoveY == -1f)
				{
					num3 = 24f;
				}
				else if (this.windMovedUp)
				{
					num3 = 0f;
				}
				else
				{
					num3 = 40f;
				}
				this.maxFall = Calc.Approach(this.maxFall, num3, 300f * Engine.DeltaTime);
			}
			else if (Input.MoveY == 1f && this.Speed.Y >= num3)
			{
				this.maxFall = Calc.Approach(this.maxFall, num4, 300f * Engine.DeltaTime);
				float num5 = num3 + (num4 - num3) * 0.5f;
				if (this.Speed.Y >= num5)
				{
					float amount = Math.Min(1f, (this.Speed.Y - num5) / (num4 - num5));
					this.Sprite.Scale.X = MathHelper.Lerp(1f, 0.5f, amount);
					this.Sprite.Scale.Y = MathHelper.Lerp(1f, 1.5f, amount);
				}
			}
			else
			{
				this.maxFall = Calc.Approach(this.maxFall, num3, 300f * Engine.DeltaTime);
			}
			if (!this.onGround)
			{
				float target = this.maxFall;
				if (this.Holding != null && this.Holding.SlowFall)
				{
					this.holdCannotDuck = (Input.MoveY == 1f);
				}
				if ((this.moveX == (int)this.Facing || (this.moveX == 0 && Input.GrabCheck)) && Input.MoveY.Value != 1)
				{
					if (this.Speed.Y >= 0f && this.wallSlideTimer > 0f && this.Holding == null && this.ClimbBoundsCheck((int)this.Facing) && base.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float)this.Facing) && !ClimbBlocker.EdgeCheck(this.level, this, (int)this.Facing) && this.CanUnDuck)
					{
						this.Ducking = false;
						this.wallSlideDir = (int)this.Facing;
					}
					if (this.wallSlideDir != 0)
					{
						if (Input.GrabCheck)
						{
							this.ClimbTrigger(this.wallSlideDir);
						}
						if (this.wallSlideTimer > 0.6f && ClimbBlocker.Check(this.level, this, this.Position + Vector2.UnitX * (float)this.wallSlideDir))
						{
							this.wallSlideTimer = 0.6f;
						}
						target = MathHelper.Lerp(160f, 20f, this.wallSlideTimer / 1.2f);
						if (this.wallSlideTimer / 1.2f > 0.65f)
						{
							this.CreateWallSlideParticles(this.wallSlideDir);
						}
					}
				}
				float num6 = (Math.Abs(this.Speed.Y) < 40f && (Input.Jump.Check || this.AutoJump)) ? 0.5f : 1f;
				if (this.Holding != null && this.Holding.SlowFall && this.forceMoveXTimer <= 0f)
				{
					num6 *= 0.5f;
				}
				if (this.level.InSpace)
				{
					num6 *= 0.6f;
				}
				this.Speed.Y = Calc.Approach(this.Speed.Y, target, 900f * num6 * Engine.DeltaTime);
			}
			if (this.varJumpTimer > 0f)
			{
				if (this.AutoJump || Input.Jump.Check)
				{
					this.Speed.Y = Math.Min(this.Speed.Y, this.varJumpSpeed);
				}
				else
				{
					this.varJumpTimer = 0f;
				}
			}
			if (Input.Jump.Pressed && (TalkComponent.PlayerOver == null || !Input.Talk.Pressed))
			{
				if (this.jumpGraceTimer > 0f)
				{
					this.Jump(true, true);
				}
				else if (this.CanUnDuck)
				{
					bool canUnDuck = this.CanUnDuck;
					Water water;
					if (canUnDuck && this.WallJumpCheck(1))
					{
						if (this.Facing == Facings.Right && Input.GrabCheck && !SaveData.Instance.Assists.NoGrabbing && this.Stamina > 0f && this.Holding == null && !ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitX * 3f))
						{
							this.ClimbJump();
						}
						else if (this.DashAttacking && this.SuperWallJumpAngleCheck)
						{
							this.SuperWallJump(-1);
						}
						else
						{
							this.WallJump(-1);
						}
					}
					else if (canUnDuck && this.WallJumpCheck(-1))
					{
						if (this.Facing == Facings.Left && Input.GrabCheck && !SaveData.Instance.Assists.NoGrabbing && this.Stamina > 0f && this.Holding == null && !ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitX * -3f))
						{
							this.ClimbJump();
						}
						else if (this.DashAttacking && this.SuperWallJumpAngleCheck)
						{
							this.SuperWallJump(1);
						}
						else
						{
							this.WallJump(1);
						}
					}
					else if ((water = base.CollideFirst<Water>(this.Position + Vector2.UnitY * 2f)) != null)
					{
						this.Jump(true, true);
						water.TopSurface.DoRipple(this.Position, 1f);
					}
				}
			}
			return 0;
		}

		// Token: 0x06001CD5 RID: 7381 RVA: 0x000C7D40 File Offset: 0x000C5F40
		public void CreateWallSlideParticles(int dir)
		{
			if (base.Scene.OnInterval(0.01f))
			{
				int index = -1;
				Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(base.CollideAll<Platform>(this.Position + Vector2.UnitX * (float)dir * 4f, this.temp));
				if (platformByPriority != null)
				{
					index = platformByPriority.GetWallSoundIndex(this, dir);
				}
				ParticleType particleType = this.DustParticleFromSurfaceIndex(index);
				float num = (particleType == ParticleTypes.Dust) ? 5f : 2f;
				Vector2 vector = base.Center;
				if (dir == 1)
				{
					vector += new Vector2(num, 4f);
				}
				else
				{
					vector += new Vector2(-num, 4f);
				}
				Dust.Burst(vector, -1.5707964f, 1, particleType);
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06001CD6 RID: 7382 RVA: 0x000C7E05 File Offset: 0x000C6005
		private bool IsTired
		{
			get
			{
				return this.CheckStamina < 20f;
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06001CD7 RID: 7383 RVA: 0x000C7E14 File Offset: 0x000C6014
		private float CheckStamina
		{
			get
			{
				if (this.wallBoostTimer > 0f)
				{
					return this.Stamina + 27.5f;
				}
				return this.Stamina;
			}
		}

		// Token: 0x06001CD8 RID: 7384 RVA: 0x000C7E36 File Offset: 0x000C6036
		private void PlaySweatEffectDangerOverride(string state)
		{
			if (this.Stamina <= 20f)
			{
				this.sweatSprite.Play("danger", false, false);
				return;
			}
			this.sweatSprite.Play(state, false, false);
		}

		// Token: 0x06001CD9 RID: 7385 RVA: 0x000C7E68 File Offset: 0x000C6068
		private void ClimbBegin()
		{
			this.AutoJump = false;
			this.Speed.X = 0f;
			this.Speed.Y = this.Speed.Y * 0.2f;
			this.wallSlideTimer = 1.2f;
			this.climbNoMoveTimer = 0.1f;
			this.wallBoostTimer = 0f;
			this.lastClimbMove = 0;
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
			int num = 0;
			while (num < 2 && !base.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float)this.Facing))
			{
				this.Position += Vector2.UnitX * (float)this.Facing;
				num++;
			}
			Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(base.CollideAll<Solid>(this.Position + Vector2.UnitX * (float)this.Facing, this.temp));
			if (platformByPriority != null)
			{
				this.Play("event:/char/madeline/grab", "surface_index", (float)platformByPriority.GetWallSoundIndex(this, (int)this.Facing));
				if (platformByPriority is DreamBlock)
				{
					(platformByPriority as DreamBlock).FootstepRipple(this.Position + new Vector2((float)(this.Facing * (Facings)3), -4f));
				}
			}
		}

		// Token: 0x06001CDA RID: 7386 RVA: 0x000C7FA4 File Offset: 0x000C61A4
		private void ClimbEnd()
		{
			if (this.conveyorLoopSfx != null)
			{
				this.conveyorLoopSfx.setParameterValue("end", 1f);
				this.conveyorLoopSfx.release();
				this.conveyorLoopSfx = null;
			}
			this.wallSpeedRetentionTimer = 0f;
			if (this.sweatSprite != null && this.sweatSprite.CurrentAnimationID != "jump")
			{
				this.sweatSprite.Play("idle", false, false);
			}
		}

		// Token: 0x06001CDB RID: 7387 RVA: 0x000C8024 File Offset: 0x000C6224
		private int ClimbUpdate()
		{
			this.climbNoMoveTimer -= Engine.DeltaTime;
			if (this.onGround)
			{
				this.Stamina = 110f;
			}
			if (Input.Jump.Pressed && (!this.Ducking || this.CanUnDuck))
			{
				if (this.moveX == (int)(-(int)this.Facing))
				{
					this.WallJump((int)(-(int)this.Facing));
				}
				else
				{
					this.ClimbJump();
				}
				return 0;
			}
			if (this.CanDash)
			{
				this.Speed += this.LiftBoost;
				return this.StartDash();
			}
			if (!Input.GrabCheck)
			{
				this.Speed += this.LiftBoost;
				this.Play("event:/char/madeline/grab_letgo", null, 0f);
				return 0;
			}
			if (!base.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float)this.Facing))
			{
				if (this.Speed.Y < 0f)
				{
					if (this.wallBoosting)
					{
						this.Speed += this.LiftBoost;
						this.Play("event:/char/madeline/grab_letgo", null, 0f);
					}
					else
					{
						this.ClimbHop();
					}
				}
				return 0;
			}
			WallBooster wallBooster = this.WallBoosterCheck();
			if (this.climbNoMoveTimer <= 0f && wallBooster != null)
			{
				this.wallBoosting = true;
				if (this.conveyorLoopSfx == null)
				{
					this.conveyorLoopSfx = Audio.Play("event:/game/09_core/conveyor_activate", this.Position, "end", 0f);
				}
				Audio.Position(this.conveyorLoopSfx, this.Position);
				this.Speed.Y = Calc.Approach(this.Speed.Y, -160f, 600f * Engine.DeltaTime);
				base.LiftSpeed = Vector2.UnitY * Math.Max(this.Speed.Y, -80f);
				Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
			}
			else
			{
				this.wallBoosting = false;
				if (this.conveyorLoopSfx != null)
				{
					this.conveyorLoopSfx.setParameterValue("end", 1f);
					this.conveyorLoopSfx.release();
					this.conveyorLoopSfx = null;
				}
				float num = 0f;
				bool flag = false;
				if (this.climbNoMoveTimer <= 0f)
				{
					if (ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitX * (float)this.Facing))
					{
						flag = true;
					}
					else if (Input.MoveY.Value == -1)
					{
						num = -45f;
						if (base.CollideCheck<Solid>(this.Position - Vector2.UnitY) || (this.ClimbHopBlockedCheck() && this.SlipCheck(-1f)))
						{
							if (this.Speed.Y < 0f)
							{
								this.Speed.Y = 0f;
							}
							num = 0f;
							flag = true;
						}
						else if (this.SlipCheck(0f))
						{
							this.ClimbHop();
							return 0;
						}
					}
					else if (Input.MoveY.Value == 1)
					{
						num = 80f;
						if (this.onGround)
						{
							if (this.Speed.Y > 0f)
							{
								this.Speed.Y = 0f;
							}
							num = 0f;
						}
						else
						{
							this.CreateWallSlideParticles((int)this.Facing);
						}
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
				this.lastClimbMove = Math.Sign(num);
				if (flag && this.SlipCheck(0f))
				{
					num = 30f;
				}
				this.Speed.Y = Calc.Approach(this.Speed.Y, num, 900f * Engine.DeltaTime);
			}
			if (Input.MoveY.Value != 1 && this.Speed.Y > 0f && !base.CollideCheck<Solid>(this.Position + new Vector2((float)this.Facing, 1f)))
			{
				this.Speed.Y = 0f;
			}
			if (this.climbNoMoveTimer <= 0f)
			{
				if (this.lastClimbMove == -1)
				{
					this.Stamina -= 45.454544f * Engine.DeltaTime;
					if (this.Stamina <= 20f)
					{
						this.sweatSprite.Play("danger", false, false);
					}
					else if (this.sweatSprite.CurrentAnimationID != "climbLoop")
					{
						this.sweatSprite.Play("climb", false, false);
					}
					if (base.Scene.OnInterval(0.2f))
					{
						Input.Rumble(RumbleStrength.Climb, RumbleLength.Short);
					}
				}
				else
				{
					if (this.lastClimbMove == 0)
					{
						this.Stamina -= 10f * Engine.DeltaTime;
					}
					if (!this.onGround)
					{
						this.PlaySweatEffectDangerOverride("still");
						if (base.Scene.OnInterval(0.8f))
						{
							Input.Rumble(RumbleStrength.Climb, RumbleLength.Short);
						}
					}
					else
					{
						this.PlaySweatEffectDangerOverride("idle");
					}
				}
			}
			else
			{
				this.PlaySweatEffectDangerOverride("idle");
			}
			if (this.Stamina <= 0f)
			{
				this.Speed += this.LiftBoost;
				return 0;
			}
			return 1;
		}

		// Token: 0x06001CDC RID: 7388 RVA: 0x000C8540 File Offset: 0x000C6740
		private WallBooster WallBoosterCheck()
		{
			if (ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitX * (float)this.Facing))
			{
				return null;
			}
			foreach (Entity entity in base.Scene.Tracker.GetEntities<WallBooster>())
			{
				WallBooster wallBooster = (WallBooster)entity;
				if (wallBooster.Facing == this.Facing && base.CollideCheck(wallBooster))
				{
					return wallBooster;
				}
			}
			return null;
		}

		// Token: 0x06001CDD RID: 7389 RVA: 0x000C85E8 File Offset: 0x000C67E8
		private void ClimbHop()
		{
			this.climbHopSolid = base.CollideFirst<Solid>(this.Position + Vector2.UnitX * (float)this.Facing);
			this.playFootstepOnLand = 0.5f;
			if (this.climbHopSolid != null)
			{
				this.climbHopSolidPosition = this.climbHopSolid.Position;
				this.hopWaitX = (int)this.Facing;
				this.hopWaitXSpeed = (float)this.Facing * 100f;
			}
			else
			{
				this.hopWaitX = 0;
				this.Speed.X = (float)this.Facing * 100f;
			}
			this.lowFrictionStopTimer = 0.15f;
			this.Speed.Y = Math.Min(this.Speed.Y, -120f);
			this.forceMoveX = 0;
			this.forceMoveXTimer = 0.2f;
			this.fastJump = false;
			this.noWindTimer = 0.3f;
			this.Play("event:/char/madeline/climb_ledge", null, 0f);
		}

		// Token: 0x06001CDE RID: 7390 RVA: 0x000C86E4 File Offset: 0x000C68E4
		private bool SlipCheck(float addY = 0f)
		{
			Vector2 vector;
			if (this.Facing == Facings.Right)
			{
				vector = base.TopRight + Vector2.UnitY * (4f + addY);
			}
			else
			{
				vector = base.TopLeft - Vector2.UnitX + Vector2.UnitY * (4f + addY);
			}
			return !base.Scene.CollideCheck<Solid>(vector) && !base.Scene.CollideCheck<Solid>(vector + Vector2.UnitY * (-4f + addY));
		}

		// Token: 0x06001CDF RID: 7391 RVA: 0x000C8778 File Offset: 0x000C6978
		private bool ClimbHopBlockedCheck()
		{
			using (List<Follower>.Enumerator enumerator = this.Leader.Followers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Entity is StrawberrySeed)
					{
						return true;
					}
				}
			}
			using (List<Component>.Enumerator enumerator2 = base.Scene.Tracker.GetComponents<LedgeBlocker>().GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (((LedgeBlocker)enumerator2.Current).HopBlockCheck(this))
					{
						return true;
					}
				}
			}
			return base.CollideCheck<Solid>(this.Position - Vector2.UnitY * 6f);
		}

		// Token: 0x06001CE0 RID: 7392 RVA: 0x000C8858 File Offset: 0x000C6A58
		private bool JumpThruBoostBlockedCheck()
		{
			using (List<Component>.Enumerator enumerator = base.Scene.Tracker.GetComponents<LedgeBlocker>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((LedgeBlocker)enumerator.Current).JumpThruBoostCheck(this))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001CE1 RID: 7393 RVA: 0x000C88C4 File Offset: 0x000C6AC4
		private bool DashCorrectCheck(Vector2 add)
		{
			Vector2 position = this.Position;
			Collider collider = base.Collider;
			this.Position += add;
			base.Collider = this.hurtbox;
			using (List<Component>.Enumerator enumerator = base.Scene.Tracker.GetComponents<LedgeBlocker>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((LedgeBlocker)enumerator.Current).DashCorrectCheck(this))
					{
						this.Position = position;
						base.Collider = collider;
						return true;
					}
				}
			}
			this.Position = position;
			base.Collider = collider;
			return false;
		}

		// Token: 0x06001CE2 RID: 7394 RVA: 0x000C8978 File Offset: 0x000C6B78
		public int StartDash()
		{
			this.wasDashB = (this.Dashes == 2);
			this.Dashes = Math.Max(0, this.Dashes - 1);
			this.demoDashed = Input.CrouchDashPressed;
			Input.Dash.ConsumeBuffer();
			Input.CrouchDash.ConsumeBuffer();
			return 2;
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06001CE3 RID: 7395 RVA: 0x000C89C8 File Offset: 0x000C6BC8
		public bool DashAttacking
		{
			get
			{
				return this.dashAttackTimer > 0f || this.StateMachine.State == 5;
			}
		}

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06001CE4 RID: 7396 RVA: 0x000C89E8 File Offset: 0x000C6BE8
		public bool CanDash
		{
			get
			{
				return (Input.CrouchDashPressed || Input.DashPressed) && this.dashCooldownTimer <= 0f && this.Dashes > 0 && (TalkComponent.PlayerOver == null || !Input.Talk.Pressed) && (this.LastBooster == null || !this.LastBooster.Ch9HubTransition || !this.LastBooster.BoostingPlayer);
			}
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x06001CE5 RID: 7397 RVA: 0x000C8A53 File Offset: 0x000C6C53
		// (set) Token: 0x06001CE6 RID: 7398 RVA: 0x000C8A5B File Offset: 0x000C6C5B
		public bool StartedDashing { get; private set; }

		// Token: 0x06001CE7 RID: 7399 RVA: 0x000C8A64 File Offset: 0x000C6C64
		private void CallDashEvents()
		{
			if (!this.calledDashEvents)
			{
				this.calledDashEvents = true;
				if (this.CurrentBooster == null)
				{
					SaveData.Instance.TotalDashes++;
					this.level.Session.Dashes++;
					Stats.Increment(Stat.DASHES, 1);
					bool flag = this.DashDir.Y < 0f || (this.DashDir.Y == 0f && this.DashDir.X > 0f);
					if (this.DashDir == Vector2.Zero)
					{
						flag = (this.Facing == Facings.Right);
					}
					if (flag)
					{
						if (this.wasDashB)
						{
							this.Play("event:/char/madeline/dash_pink_right", null, 0f);
						}
						else
						{
							this.Play("event:/char/madeline/dash_red_right", null, 0f);
						}
					}
					else if (this.wasDashB)
					{
						this.Play("event:/char/madeline/dash_pink_left", null, 0f);
					}
					else
					{
						this.Play("event:/char/madeline/dash_red_left", null, 0f);
					}
					if (this.SwimCheck())
					{
						this.Play("event:/char/madeline/water_dash_gen", null, 0f);
					}
					using (List<Component>.Enumerator enumerator = base.Scene.Tracker.GetComponents<DashListener>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Component component = enumerator.Current;
							DashListener dashListener = (DashListener)component;
							if (dashListener.OnDash != null)
							{
								dashListener.OnDash(this.DashDir);
							}
						}
						return;
					}
				}
				this.CurrentBooster.PlayerBoosted(this, this.DashDir);
				this.CurrentBooster = null;
			}
		}

		// Token: 0x06001CE8 RID: 7400 RVA: 0x000C8C14 File Offset: 0x000C6E14
		private void DashBegin()
		{
			this.calledDashEvents = false;
			this.dashStartedOnGround = this.onGround;
			this.launched = false;
			this.canCurveDash = true;
			if (Engine.TimeRate > 0.25f)
			{
				Celeste.Freeze(0.05f);
			}
			this.dashCooldownTimer = 0.2f;
			this.dashRefillCooldownTimer = 0.1f;
			this.StartedDashing = true;
			this.wallSlideTimer = 1.2f;
			this.dashTrailTimer = 0f;
			this.dashTrailCounter = 0;
			if (!SaveData.Instance.Assists.DashAssist)
			{
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			}
			this.dashAttackTimer = 0.3f;
			this.gliderBoostTimer = 0.55f;
			if (SaveData.Instance.Assists.SuperDashing)
			{
				this.dashAttackTimer += 0.15f;
			}
			this.beforeDashSpeed = this.Speed;
			this.Speed = Vector2.Zero;
			this.DashDir = Vector2.Zero;
			if (!this.onGround && this.Ducking && this.CanUnDuck)
			{
				this.Ducking = false;
			}
			else if (!this.Ducking && (this.demoDashed || Input.MoveY.Value == 1))
			{
				this.Ducking = true;
			}
			this.DashAssistInit();
		}

		// Token: 0x06001CE9 RID: 7401 RVA: 0x000C8D50 File Offset: 0x000C6F50
		private void DashAssistInit()
		{
			if (SaveData.Instance.Assists.DashAssist && !this.demoDashed)
			{
				Input.LastAim = Vector2.UnitX * (float)this.Facing;
				Engine.DashAssistFreeze = true;
				Engine.DashAssistFreezePress = false;
				PlayerDashAssist playerDashAssist = base.Scene.Tracker.GetEntity<PlayerDashAssist>();
				if (playerDashAssist == null)
				{
					base.Scene.Add(playerDashAssist = new PlayerDashAssist());
				}
				playerDashAssist.Direction = Input.GetAimVector(this.Facing).Angle();
				playerDashAssist.Scale = 0f;
				playerDashAssist.Offset = ((this.CurrentBooster == null && this.StateMachine.PreviousState != 5) ? Vector2.Zero : new Vector2(0f, -4f));
			}
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x000C8E17 File Offset: 0x000C7017
		private void DashEnd()
		{
			this.CallDashEvents();
			this.demoDashed = false;
		}

		// Token: 0x06001CEB RID: 7403 RVA: 0x000C8E28 File Offset: 0x000C7028
		private int DashUpdate()
		{
			this.StartedDashing = false;
			if (this.dashTrailTimer > 0f)
			{
				this.dashTrailTimer -= Engine.DeltaTime;
				if (this.dashTrailTimer <= 0f)
				{
					this.CreateTrail();
					this.dashTrailCounter--;
					if (this.dashTrailCounter > 0)
					{
						this.dashTrailTimer = 0.1f;
					}
				}
			}
			if (SaveData.Instance.Assists.SuperDashing && this.canCurveDash && Input.Aim.Value != Vector2.Zero && this.Speed != Vector2.Zero)
			{
				Vector2 vector = Input.GetAimVector(Facings.Right);
				vector = this.CorrectDashPrecision(vector);
				float num = Vector2.Dot(vector, this.Speed.SafeNormalize());
				if (num >= -0.1f && num < 0.99f)
				{
					this.Speed = this.Speed.RotateTowards(vector.Angle(), 4.1887903f * Engine.DeltaTime);
					this.DashDir = this.Speed.SafeNormalize();
					this.DashDir = this.CorrectDashPrecision(this.DashDir);
				}
			}
			if (SaveData.Instance.Assists.SuperDashing && this.CanDash)
			{
				this.StartDash();
				this.StateMachine.ForceState(2);
				return 2;
			}
			if (this.Holding == null && this.DashDir != Vector2.Zero && Input.GrabCheck && !this.IsTired && this.CanUnDuck)
			{
				foreach (Component component in base.Scene.Tracker.GetComponents<Holdable>())
				{
					Holdable holdable = (Holdable)component;
					if (holdable.Check(this) && this.Pickup(holdable))
					{
						return 8;
					}
				}
			}
			if (Math.Abs(this.DashDir.Y) < 0.1f)
			{
				foreach (Entity entity in base.Scene.Tracker.GetEntities<JumpThru>())
				{
					JumpThru jumpThru = (JumpThru)entity;
					if (base.CollideCheck(jumpThru) && base.Bottom - jumpThru.Top <= 6f && !this.DashCorrectCheck(Vector2.UnitY * (jumpThru.Top - base.Bottom)))
					{
						base.MoveVExact((int)(jumpThru.Top - base.Bottom), null, null);
					}
				}
				if (this.CanUnDuck && Input.Jump.Pressed && this.jumpGraceTimer > 0f)
				{
					this.SuperJump();
					return 0;
				}
			}
			if (this.SuperWallJumpAngleCheck)
			{
				if (Input.Jump.Pressed && this.CanUnDuck)
				{
					if (this.WallJumpCheck(1))
					{
						this.SuperWallJump(-1);
						return 0;
					}
					if (this.WallJumpCheck(-1))
					{
						this.SuperWallJump(1);
						return 0;
					}
				}
			}
			else if (Input.Jump.Pressed && this.CanUnDuck)
			{
				if (this.WallJumpCheck(1))
				{
					if (this.Facing == Facings.Right && Input.GrabCheck && this.Stamina > 0f && this.Holding == null && !ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitX * 3f))
					{
						this.ClimbJump();
					}
					else
					{
						this.WallJump(-1);
					}
					return 0;
				}
				if (this.WallJumpCheck(-1))
				{
					if (this.Facing == Facings.Left && Input.GrabCheck && this.Stamina > 0f && this.Holding == null && !ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitX * -3f))
					{
						this.ClimbJump();
					}
					else
					{
						this.WallJump(1);
					}
					return 0;
				}
			}
			if (this.Speed != Vector2.Zero && this.level.OnInterval(0.02f))
			{
				ParticleType type;
				if (!this.wasDashB)
				{
					type = Player.P_DashA;
				}
				else if (this.Sprite.Mode == PlayerSpriteMode.MadelineAsBadeline)
				{
					type = Player.P_DashBadB;
				}
				else
				{
					type = Player.P_DashB;
				}
				this.level.ParticlesFG.Emit(type, base.Center + Calc.Random.Range(Vector2.One * -2f, Vector2.One * 2f), this.DashDir.Angle());
			}
			return 2;
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x06001CEC RID: 7404 RVA: 0x000C92DC File Offset: 0x000C74DC
		private bool SuperWallJumpAngleCheck
		{
			get
			{
				return Math.Abs(this.DashDir.X) <= 0.2f && this.DashDir.Y <= -0.75f;
			}
		}

		// Token: 0x06001CED RID: 7405 RVA: 0x000C930C File Offset: 0x000C750C
		private Vector2 CorrectDashPrecision(Vector2 dir)
		{
			if (dir.X != 0f && Math.Abs(dir.X) < 0.001f)
			{
				dir.X = 0f;
				dir.Y = (float)Math.Sign(dir.Y);
			}
			else if (dir.Y != 0f && Math.Abs(dir.Y) < 0.001f)
			{
				dir.Y = 0f;
				dir.X = (float)Math.Sign(dir.X);
			}
			return dir;
		}

		// Token: 0x06001CEE RID: 7406 RVA: 0x000C9398 File Offset: 0x000C7598
		private IEnumerator DashCoroutine()
		{
			yield return null;
			if (SaveData.Instance.Assists.DashAssist)
			{
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			}
			this.level.Displacement.AddBurst(base.Center, 0.4f, 8f, 64f, 0.5f, Ease.QuadOut, Ease.QuadOut);
			Vector2 vector = this.lastAim;
			if (this.OverrideDashDirection != null)
			{
				vector = this.OverrideDashDirection.Value;
			}
			vector = this.CorrectDashPrecision(vector);
			Vector2 vector2 = vector * 240f;
			if (Math.Sign(this.beforeDashSpeed.X) == Math.Sign(vector2.X) && Math.Abs(this.beforeDashSpeed.X) > Math.Abs(vector2.X))
			{
				vector2.X = this.beforeDashSpeed.X;
			}
			this.Speed = vector2;
			if (base.CollideCheck<Water>())
			{
				this.Speed *= 0.75f;
			}
			this.gliderBoostDir = (this.DashDir = vector);
			base.SceneAs<Level>().DirectionalShake(this.DashDir, 0.2f);
			if (this.DashDir.X != 0f)
			{
				this.Facing = (Facings)Math.Sign(this.DashDir.X);
			}
			this.CallDashEvents();
			if (this.StateMachine.PreviousState == 19)
			{
				this.level.Particles.Emit(FlyFeather.P_Boost, 12, base.Center, Vector2.One * 4f, (-vector).Angle());
			}
			if (this.onGround && this.DashDir.X != 0f && this.DashDir.Y > 0f && this.Speed.Y > 0f && (!this.Inventory.DreamDash || !base.CollideCheck<DreamBlock>(this.Position + Vector2.UnitY)))
			{
				this.DashDir.X = (float)Math.Sign(this.DashDir.X);
				this.DashDir.Y = 0f;
				this.Speed.Y = 0f;
				this.Speed.X = this.Speed.X * 1.2f;
				this.Ducking = true;
			}
			SlashFx.Burst(base.Center, this.DashDir.Angle());
			this.CreateTrail();
			if (SaveData.Instance.Assists.SuperDashing)
			{
				this.dashTrailTimer = 0.1f;
				this.dashTrailCounter = 2;
			}
			else
			{
				this.dashTrailTimer = 0.08f;
				this.dashTrailCounter = 1;
			}
			if (this.DashDir.X != 0f && Input.GrabCheck)
			{
				SwapBlock swapBlock = base.CollideFirst<SwapBlock>(this.Position + Vector2.UnitX * (float)Math.Sign(this.DashDir.X));
				if (swapBlock != null && swapBlock.Direction.X == (float)Math.Sign(this.DashDir.X))
				{
					this.StateMachine.State = 1;
					this.Speed = Vector2.Zero;
					yield break;
				}
			}
			Vector2 swapCancel = Vector2.One;
			foreach (Entity entity in base.Scene.Tracker.GetEntities<SwapBlock>())
			{
				SwapBlock swapBlock2 = (SwapBlock)entity;
				if (base.CollideCheck(swapBlock2, this.Position + Vector2.UnitY) && swapBlock2 != null && swapBlock2.Swapping)
				{
					if (this.DashDir.X != 0f && swapBlock2.Direction.X == (float)Math.Sign(this.DashDir.X))
					{
						this.Speed.X = (swapCancel.X = 0f);
					}
					if (this.DashDir.Y != 0f && swapBlock2.Direction.Y == (float)Math.Sign(this.DashDir.Y))
					{
						this.Speed.Y = (swapCancel.Y = 0f);
					}
				}
			}
			if (SaveData.Instance.Assists.SuperDashing)
			{
				yield return 0.3f;
			}
			else
			{
				yield return 0.15f;
			}
			this.CreateTrail();
			this.AutoJump = true;
			this.AutoJumpTimer = 0f;
			if (this.DashDir.Y <= 0f)
			{
				this.Speed = this.DashDir * 160f;
				this.Speed.X = this.Speed.X * swapCancel.X;
				this.Speed.Y = this.Speed.Y * swapCancel.Y;
			}
			if (this.Speed.Y < 0f)
			{
				this.Speed.Y = this.Speed.Y * 0.75f;
			}
			this.StateMachine.State = 0;
			yield break;
		}

		// Token: 0x06001CEF RID: 7407 RVA: 0x000C93A7 File Offset: 0x000C75A7
		private bool SwimCheck()
		{
			return base.CollideCheck<Water>(this.Position + Vector2.UnitY * -8f) && base.CollideCheck<Water>(this.Position);
		}

		// Token: 0x06001CF0 RID: 7408 RVA: 0x000C93D9 File Offset: 0x000C75D9
		private bool SwimUnderwaterCheck()
		{
			return base.CollideCheck<Water>(this.Position + Vector2.UnitY * -9f);
		}

		// Token: 0x06001CF1 RID: 7409 RVA: 0x000C93FB File Offset: 0x000C75FB
		private bool SwimJumpCheck()
		{
			return !base.CollideCheck<Water>(this.Position + Vector2.UnitY * -14f);
		}

		// Token: 0x06001CF2 RID: 7410 RVA: 0x000C9420 File Offset: 0x000C7620
		private bool SwimRiseCheck()
		{
			return !base.CollideCheck<Water>(this.Position + Vector2.UnitY * -18f);
		}

		// Token: 0x06001CF3 RID: 7411 RVA: 0x000C9445 File Offset: 0x000C7645
		private bool UnderwaterMusicCheck()
		{
			return base.CollideCheck<Water>(this.Position) && base.CollideCheck<Water>(this.Position + Vector2.UnitY * -12f);
		}

		// Token: 0x06001CF4 RID: 7412 RVA: 0x000C9477 File Offset: 0x000C7677
		private void SwimBegin()
		{
			if (this.Speed.Y > 0f)
			{
				this.Speed.Y = this.Speed.Y * 0.5f;
			}
			this.Stamina = 110f;
		}

		// Token: 0x06001CF5 RID: 7413 RVA: 0x000C94AC File Offset: 0x000C76AC
		private int SwimUpdate()
		{
			if (!this.SwimCheck())
			{
				return 0;
			}
			if (this.CanUnDuck)
			{
				this.Ducking = false;
			}
			if (this.CanDash)
			{
				this.demoDashed = Input.CrouchDashPressed;
				Input.Dash.ConsumeBuffer();
				Input.CrouchDash.ConsumeBuffer();
				return 2;
			}
			bool flag = this.SwimUnderwaterCheck();
			if (!flag && this.Speed.Y >= 0f && Input.GrabCheck && !this.IsTired && this.CanUnDuck && Math.Sign(this.Speed.X) != (int)(-(int)this.Facing) && this.ClimbCheck((int)this.Facing, 0))
			{
				if (SaveData.Instance.Assists.NoGrabbing)
				{
					this.ClimbTrigger((int)this.Facing);
				}
				else if (!base.MoveVExact(-1, null, null))
				{
					this.Ducking = false;
					return 1;
				}
			}
			Vector2 vector = Input.Feather.Value;
			vector = vector.SafeNormalize();
			float num = flag ? 60f : 80f;
			float num2 = 80f;
			if (Math.Abs(this.Speed.X) > 80f && Math.Sign(this.Speed.X) == Math.Sign(vector.X))
			{
				this.Speed.X = Calc.Approach(this.Speed.X, num * vector.X, 400f * Engine.DeltaTime);
			}
			else
			{
				this.Speed.X = Calc.Approach(this.Speed.X, num * vector.X, 600f * Engine.DeltaTime);
			}
			if (vector.Y == 0f && this.SwimRiseCheck())
			{
				this.Speed.Y = Calc.Approach(this.Speed.Y, -60f, 600f * Engine.DeltaTime);
			}
			else if (vector.Y >= 0f || this.SwimUnderwaterCheck())
			{
				if (Math.Abs(this.Speed.Y) > 80f && Math.Sign(this.Speed.Y) == Math.Sign(vector.Y))
				{
					this.Speed.Y = Calc.Approach(this.Speed.Y, num2 * vector.Y, 400f * Engine.DeltaTime);
				}
				else
				{
					this.Speed.Y = Calc.Approach(this.Speed.Y, num2 * vector.Y, 600f * Engine.DeltaTime);
				}
			}
			if (!flag && this.moveX != 0 && base.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float)this.moveX) && !base.CollideCheck<Solid>(this.Position + new Vector2((float)this.moveX, -3f)))
			{
				this.ClimbHop();
			}
			if (Input.Jump.Pressed && this.SwimJumpCheck())
			{
				this.Jump(true, true);
				return 0;
			}
			return 3;
		}

		// Token: 0x06001CF6 RID: 7414 RVA: 0x000C97B0 File Offset: 0x000C79B0
		public void Boost(Booster booster)
		{
			this.StateMachine.State = 4;
			this.Speed = Vector2.Zero;
			this.boostTarget = booster.Center;
			this.boostRed = false;
			this.CurrentBooster = booster;
			this.LastBooster = booster;
		}

		// Token: 0x06001CF7 RID: 7415 RVA: 0x000C97F8 File Offset: 0x000C79F8
		public void RedBoost(Booster booster)
		{
			this.StateMachine.State = 4;
			this.Speed = Vector2.Zero;
			this.boostTarget = booster.Center;
			this.boostRed = true;
			this.CurrentBooster = booster;
			this.LastBooster = booster;
		}

		// Token: 0x06001CF8 RID: 7416 RVA: 0x000C983F File Offset: 0x000C7A3F
		private void BoostBegin()
		{
			this.RefillDash();
			this.RefillStamina();
			if (this.Holding != null)
			{
				this.Drop();
			}
		}

		// Token: 0x06001CF9 RID: 7417 RVA: 0x000C985C File Offset: 0x000C7A5C
		private void BoostEnd()
		{
			Vector2 vector = (this.boostTarget - base.Collider.Center).Floor();
			base.MoveToX(vector.X, null);
			base.MoveToY(vector.Y, null);
		}

		// Token: 0x06001CFA RID: 7418 RVA: 0x000C98A0 File Offset: 0x000C7AA0
		private int BoostUpdate()
		{
			Vector2 value = Input.Aim.Value * 3f;
			Vector2 vector = Calc.Approach(base.ExactPosition, this.boostTarget - base.Collider.Center + value, 80f * Engine.DeltaTime);
			base.MoveToX(vector.X, null);
			base.MoveToY(vector.Y, null);
			if (!Input.DashPressed && !Input.CrouchDashPressed)
			{
				return 4;
			}
			this.demoDashed = Input.CrouchDashPressed;
			Input.Dash.ConsumePress();
			Input.CrouchDash.ConsumeBuffer();
			if (this.boostRed)
			{
				return 5;
			}
			return 2;
		}

		// Token: 0x06001CFB RID: 7419 RVA: 0x000C9949 File Offset: 0x000C7B49
		private IEnumerator BoostCoroutine()
		{
			yield return 0.25f;
			if (this.boostRed)
			{
				this.StateMachine.State = 5;
			}
			else
			{
				this.StateMachine.State = 2;
			}
			yield break;
		}

		// Token: 0x06001CFC RID: 7420 RVA: 0x000C9958 File Offset: 0x000C7B58
		private void RedDashBegin()
		{
			this.calledDashEvents = false;
			this.dashStartedOnGround = false;
			Celeste.Freeze(0.05f);
			Dust.Burst(this.Position, (-this.DashDir).Angle(), 8, null);
			this.dashCooldownTimer = 0.2f;
			this.dashRefillCooldownTimer = 0.1f;
			this.StartedDashing = true;
			this.level.Displacement.AddBurst(base.Center, 0.5f, 0f, 80f, 0.666f, Ease.QuadOut, Ease.QuadOut);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			this.dashAttackTimer = 0.3f;
			this.gliderBoostTimer = 0.55f;
			this.DashDir = (this.Speed = Vector2.Zero);
			if (!this.onGround && this.CanUnDuck)
			{
				this.Ducking = false;
			}
			this.DashAssistInit();
		}

		// Token: 0x06001CFD RID: 7421 RVA: 0x000C9A3A File Offset: 0x000C7C3A
		private void RedDashEnd()
		{
			this.CallDashEvents();
		}

		// Token: 0x06001CFE RID: 7422 RVA: 0x000C9A44 File Offset: 0x000C7C44
		private int RedDashUpdate()
		{
			this.StartedDashing = false;
			bool flag = this.LastBooster != null && this.LastBooster.Ch9HubTransition;
			this.gliderBoostTimer = 0.05f;
			if (this.CanDash)
			{
				return this.StartDash();
			}
			if (this.DashDir.Y == 0f)
			{
				foreach (Entity entity in base.Scene.Tracker.GetEntities<JumpThru>())
				{
					JumpThru jumpThru = (JumpThru)entity;
					if (base.CollideCheck(jumpThru) && base.Bottom - jumpThru.Top <= 6f)
					{
						base.MoveVExact((int)(jumpThru.Top - base.Bottom), null, null);
					}
				}
				if (this.CanUnDuck && Input.Jump.Pressed && this.jumpGraceTimer > 0f && !flag)
				{
					this.SuperJump();
					return 0;
				}
			}
			if (!flag)
			{
				if (this.SuperWallJumpAngleCheck)
				{
					if (Input.Jump.Pressed && this.CanUnDuck)
					{
						if (this.WallJumpCheck(1))
						{
							this.SuperWallJump(-1);
							return 0;
						}
						if (this.WallJumpCheck(-1))
						{
							this.SuperWallJump(1);
							return 0;
						}
					}
				}
				else if (Input.Jump.Pressed && this.CanUnDuck)
				{
					if (this.WallJumpCheck(1))
					{
						if (this.Facing == Facings.Right && Input.GrabCheck && this.Stamina > 0f && this.Holding == null && !ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitX * 3f))
						{
							this.ClimbJump();
						}
						else
						{
							this.WallJump(-1);
						}
						return 0;
					}
					if (this.WallJumpCheck(-1))
					{
						if (this.Facing == Facings.Left && Input.GrabCheck && this.Stamina > 0f && this.Holding == null && !ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitX * -3f))
						{
							this.ClimbJump();
						}
						else
						{
							this.WallJump(1);
						}
						return 0;
					}
				}
			}
			return 5;
		}

		// Token: 0x06001CFF RID: 7423 RVA: 0x000C9C84 File Offset: 0x000C7E84
		private IEnumerator RedDashCoroutine()
		{
			yield return null;
			this.Speed = this.CorrectDashPrecision(this.lastAim) * 240f;
			this.gliderBoostDir = (this.DashDir = this.lastAim);
			base.SceneAs<Level>().DirectionalShake(this.DashDir, 0.2f);
			if (this.DashDir.X != 0f)
			{
				this.Facing = (Facings)Math.Sign(this.DashDir.X);
			}
			this.CallDashEvents();
			yield break;
		}

		// Token: 0x06001D00 RID: 7424 RVA: 0x000C9C93 File Offset: 0x000C7E93
		private void HitSquashBegin()
		{
			this.hitSquashNoMoveTimer = 0.1f;
		}

		// Token: 0x06001D01 RID: 7425 RVA: 0x000C9CA0 File Offset: 0x000C7EA0
		private int HitSquashUpdate()
		{
			this.Speed.X = Calc.Approach(this.Speed.X, 0f, 800f * Engine.DeltaTime);
			this.Speed.Y = Calc.Approach(this.Speed.Y, 0f, 800f * Engine.DeltaTime);
			if (Input.Jump.Pressed)
			{
				if (this.onGround)
				{
					this.Jump(true, true);
				}
				else if (this.WallJumpCheck(1))
				{
					if (this.Facing == Facings.Right && Input.GrabCheck && this.Stamina > 0f && this.Holding == null && !ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitX * 3f))
					{
						this.ClimbJump();
					}
					else
					{
						this.WallJump(-1);
					}
				}
				else if (this.WallJumpCheck(-1))
				{
					if (this.Facing == Facings.Left && Input.GrabCheck && this.Stamina > 0f && this.Holding == null && !ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitX * -3f))
					{
						this.ClimbJump();
					}
					else
					{
						this.WallJump(1);
					}
				}
				else
				{
					Input.Jump.ConsumeBuffer();
				}
				return 0;
			}
			if (this.CanDash)
			{
				return this.StartDash();
			}
			if (Input.GrabCheck && this.ClimbCheck((int)this.Facing, 0))
			{
				return 1;
			}
			if (this.hitSquashNoMoveTimer > 0f)
			{
				this.hitSquashNoMoveTimer -= Engine.DeltaTime;
				return 6;
			}
			return 0;
		}

		// Token: 0x06001D02 RID: 7426 RVA: 0x000C9E4C File Offset: 0x000C804C
		public Vector2 ExplodeLaunch(Vector2 from, bool snapUp = true, bool sidesOnly = false)
		{
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			Celeste.Freeze(0.1f);
			this.launchApproachX = null;
			Vector2 vector = (base.Center - from).SafeNormalize(-Vector2.UnitY);
			float num = Vector2.Dot(vector, Vector2.UnitY);
			if (snapUp && num <= -0.7f)
			{
				vector.X = 0f;
				vector.Y = -1f;
			}
			else if (num <= 0.65f && num >= -0.55f)
			{
				vector.Y = 0f;
				vector.X = (float)Math.Sign(vector.X);
			}
			if (sidesOnly && vector.X != 0f)
			{
				vector.Y = 0f;
				vector.X = (float)Math.Sign(vector.X);
			}
			this.Speed = 280f * vector;
			if (this.Speed.Y <= 50f)
			{
				this.Speed.Y = Math.Min(-150f, this.Speed.Y);
				this.AutoJump = true;
			}
			if (this.Speed.X != 0f)
			{
				if (Input.MoveX.Value == Math.Sign(this.Speed.X))
				{
					this.explodeLaunchBoostTimer = 0f;
					this.Speed.X = this.Speed.X * 1.2f;
				}
				else
				{
					this.explodeLaunchBoostTimer = 0.01f;
					this.explodeLaunchBoostSpeed = this.Speed.X * 1.2f;
				}
			}
			SlashFx.Burst(base.Center, this.Speed.Angle());
			if (!this.Inventory.NoRefills)
			{
				this.RefillDash();
			}
			this.RefillStamina();
			this.dashCooldownTimer = 0.2f;
			this.StateMachine.State = 7;
			return vector;
		}

		// Token: 0x06001D03 RID: 7427 RVA: 0x000CA028 File Offset: 0x000C8228
		public void FinalBossPushLaunch(int dir)
		{
			this.launchApproachX = null;
			this.Speed.X = 0.9f * (float)dir * 280f;
			this.Speed.Y = -150f;
			this.AutoJump = true;
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			SlashFx.Burst(base.Center, this.Speed.Angle());
			this.RefillDash();
			this.RefillStamina();
			this.dashCooldownTimer = 0.28f;
			this.StateMachine.State = 7;
		}

		// Token: 0x06001D04 RID: 7428 RVA: 0x000CA0B4 File Offset: 0x000C82B4
		public void BadelineBoostLaunch(float atX)
		{
			this.launchApproachX = new float?(atX);
			this.Speed.X = 0f;
			this.Speed.Y = -330f;
			this.AutoJump = true;
			if (this.Holding != null)
			{
				this.Drop();
			}
			SlashFx.Burst(base.Center, this.Speed.Angle());
			this.RefillDash();
			this.RefillStamina();
			this.dashCooldownTimer = 0.2f;
			this.StateMachine.State = 7;
		}

		// Token: 0x06001D05 RID: 7429 RVA: 0x000CA13D File Offset: 0x000C833D
		private void LaunchBegin()
		{
			this.launched = true;
		}

		// Token: 0x06001D06 RID: 7430 RVA: 0x000CA148 File Offset: 0x000C8348
		private int LaunchUpdate()
		{
			if (this.launchApproachX != null)
			{
				base.MoveTowardsX(this.launchApproachX.Value, 60f * Engine.DeltaTime, null);
			}
			if (this.CanDash)
			{
				return this.StartDash();
			}
			if (Input.GrabCheck && !this.IsTired && !this.Ducking)
			{
				foreach (Component component in base.Scene.Tracker.GetComponents<Holdable>())
				{
					Holdable holdable = (Holdable)component;
					if (holdable.Check(this) && this.Pickup(holdable))
					{
						return 8;
					}
				}
			}
			if (this.Speed.Y < 0f)
			{
				this.Speed.Y = Calc.Approach(this.Speed.Y, 160f, 450f * Engine.DeltaTime);
			}
			else
			{
				this.Speed.Y = Calc.Approach(this.Speed.Y, 160f, 225f * Engine.DeltaTime);
			}
			this.Speed.X = Calc.Approach(this.Speed.X, 0f, 200f * Engine.DeltaTime);
			if (this.Speed.Length() < 220f)
			{
				return 0;
			}
			return 7;
		}

		// Token: 0x06001D07 RID: 7431 RVA: 0x000CA2B8 File Offset: 0x000C84B8
		public void SummitLaunch(float targetX)
		{
			this.summitLaunchTargetX = targetX;
			this.StateMachine.State = 10;
		}

		// Token: 0x06001D08 RID: 7432 RVA: 0x000CA2D0 File Offset: 0x000C84D0
		private void SummitLaunchBegin()
		{
			this.wallBoostTimer = 0f;
			this.Sprite.Play("launch", false, false);
			this.Speed = -Vector2.UnitY * 240f;
			this.summitLaunchParticleTimer = 0.4f;
		}

		// Token: 0x06001D09 RID: 7433 RVA: 0x000CA320 File Offset: 0x000C8520
		private int SummitLaunchUpdate()
		{
			this.summitLaunchParticleTimer -= Engine.DeltaTime;
			if (this.summitLaunchParticleTimer > 0f && base.Scene.OnInterval(0.03f))
			{
				this.level.ParticlesFG.Emit(BadelineBoost.P_Move, 1, base.Center, Vector2.One * 4f);
			}
			this.Facing = Facings.Right;
			base.MoveTowardsX(this.summitLaunchTargetX, 20f * Engine.DeltaTime, null);
			this.Speed = -Vector2.UnitY * 240f;
			if (this.level.OnInterval(0.2f))
			{
				this.level.Add(Engine.Pooler.Create<SpeedRing>().Init(base.Center, 1.5707964f, Color.White));
			}
			CrystalStaticSpinner crystalStaticSpinner = base.Scene.CollideFirst<CrystalStaticSpinner>(new Rectangle((int)(base.X - 4f), (int)(base.Y - 40f), 8, 12));
			if (crystalStaticSpinner != null)
			{
				crystalStaticSpinner.Destroy(false);
				this.level.Shake(0.3f);
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
				Celeste.Freeze(0.01f);
			}
			return 10;
		}

		// Token: 0x06001D0A RID: 7434 RVA: 0x000CA459 File Offset: 0x000C8659
		public void StopSummitLaunch()
		{
			this.StateMachine.State = 0;
			this.Speed.Y = -140f;
			this.AutoJump = true;
			this.varJumpSpeed = this.Speed.Y;
		}

		// Token: 0x06001D0B RID: 7435 RVA: 0x000CA48F File Offset: 0x000C868F
		private IEnumerator PickupCoroutine()
		{
			this.Play("event:/char/madeline/crystaltheo_lift", null, 0f);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
			if (this.Holding != null && this.Holding.SlowFall && ((this.gliderBoostTimer - 0.16f > 0f && this.gliderBoostDir.Y < 0f) || (this.Speed.Length() > 180f && this.Speed.Y <= 0f)))
			{
				Audio.Play("event:/new_content/game/10_farewell/glider_platform_dissipate", this.Position);
			}
			Vector2 oldSpeed = this.Speed;
			float varJump = this.varJumpTimer;
			this.Speed = Vector2.Zero;
			Vector2 vector = this.Holding.Entity.Position - this.Position;
			Vector2 carryOffsetTarget = Player.CarryOffsetTarget;
			Vector2 control = new Vector2(vector.X + (float)(Math.Sign(vector.X) * 2), Player.CarryOffsetTarget.Y - 2f);
			SimpleCurve curve = new SimpleCurve(vector, carryOffsetTarget, control);
			this.carryOffset = vector;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.16f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				this.carryOffset = curve.GetPoint(t.Eased);
			};
			base.Add(tween);
			yield return tween.Wait();
			this.Speed = oldSpeed;
			this.Speed.Y = Math.Min(this.Speed.Y, 0f);
			this.varJumpTimer = varJump;
			this.StateMachine.State = 0;
			if (this.Holding != null && this.Holding.SlowFall)
			{
				if (this.gliderBoostTimer > 0f && this.gliderBoostDir.Y < 0f)
				{
					Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
					this.gliderBoostTimer = 0f;
					this.Speed.Y = Math.Min(this.Speed.Y, -240f * Math.Abs(this.gliderBoostDir.Y));
				}
				else if (this.Speed.Y < 0f)
				{
					this.Speed.Y = Math.Min(this.Speed.Y, -105f);
				}
				if (this.onGround && Input.MoveY == 1f)
				{
					this.holdCannotDuck = true;
				}
			}
			yield break;
		}

		// Token: 0x06001D0C RID: 7436 RVA: 0x000CA4A0 File Offset: 0x000C86A0
		private void DreamDashBegin()
		{
			if (this.dreamSfxLoop == null)
			{
				base.Add(this.dreamSfxLoop = new SoundSource());
			}
			this.Speed = this.DashDir * 240f;
			this.TreatNaive = true;
			base.Depth = -12000;
			this.dreamDashCanEndTimer = 0.1f;
			this.Stamina = 110f;
			this.dreamJump = false;
			this.Play("event:/char/madeline/dreamblock_enter", null, 0f);
			this.Loop(this.dreamSfxLoop, "event:/char/madeline/dreamblock_travel");
		}

		// Token: 0x06001D0D RID: 7437 RVA: 0x000CA534 File Offset: 0x000C8734
		private void DreamDashEnd()
		{
			base.Depth = 0;
			if (!this.dreamJump)
			{
				this.AutoJump = true;
				this.AutoJumpTimer = 0f;
			}
			if (!this.Inventory.NoRefills)
			{
				this.RefillDash();
			}
			this.RefillStamina();
			this.TreatNaive = false;
			if (this.dreamBlock != null)
			{
				if (this.DashDir.X != 0f)
				{
					this.jumpGraceTimer = 0.1f;
					this.dreamJump = true;
				}
				else
				{
					this.jumpGraceTimer = 0f;
				}
				this.dreamBlock.OnPlayerExit(this);
				this.dreamBlock = null;
			}
			this.Stop(this.dreamSfxLoop);
			this.Play("event:/char/madeline/dreamblock_exit", null, 0f);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
		}

		// Token: 0x06001D0E RID: 7438 RVA: 0x000CA5F4 File Offset: 0x000C87F4
		private int DreamDashUpdate()
		{
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			Vector2 position = this.Position;
			base.NaiveMove(this.Speed * Engine.DeltaTime);
			if (this.dreamDashCanEndTimer > 0f)
			{
				this.dreamDashCanEndTimer -= Engine.DeltaTime;
			}
			DreamBlock dreamBlock = base.CollideFirst<DreamBlock>();
			if (dreamBlock == null)
			{
				if (this.DreamDashedIntoSolid())
				{
					if (SaveData.Instance.Assists.Invincible)
					{
						this.Position = position;
						this.Speed *= -1f;
						this.Play("event:/game/general/assist_dreamblockbounce", null, 0f);
					}
					else
					{
						this.Die(Vector2.Zero, false, true);
					}
				}
				else if (this.dreamDashCanEndTimer <= 0f)
				{
					Celeste.Freeze(0.05f);
					if (Input.Jump.Pressed && this.DashDir.X != 0f)
					{
						this.dreamJump = true;
						this.Jump(true, true);
					}
					else if (this.DashDir.Y >= 0f || this.DashDir.X != 0f)
					{
						if (this.DashDir.X > 0f && base.CollideCheck<Solid>(this.Position - Vector2.UnitX * 5f))
						{
							base.MoveHExact(-5, null, null);
						}
						else if (this.DashDir.X < 0f && base.CollideCheck<Solid>(this.Position + Vector2.UnitX * 5f))
						{
							base.MoveHExact(5, null, null);
						}
						bool flag = this.ClimbCheck(-1, 0);
						bool flag2 = this.ClimbCheck(1, 0);
						if (Input.GrabCheck && ((this.moveX == 1 && flag2) || (this.moveX == -1 && flag)))
						{
							this.Facing = (Facings)this.moveX;
							if (!SaveData.Instance.Assists.NoGrabbing)
							{
								return 1;
							}
							this.ClimbTrigger(this.moveX);
							this.Speed.X = 0f;
						}
					}
					return 0;
				}
			}
			else
			{
				this.dreamBlock = dreamBlock;
				if (base.Scene.OnInterval(0.1f))
				{
					this.CreateTrail();
				}
				if (this.level.OnInterval(0.04f))
				{
					DisplacementRenderer.Burst burst = this.level.Displacement.AddBurst(base.Center, 0.3f, 0f, 40f, 1f, null, null);
					burst.WorldClipCollider = this.dreamBlock.Collider;
					burst.WorldClipPadding = 2;
				}
			}
			return 9;
		}

		// Token: 0x06001D0F RID: 7439 RVA: 0x000CA88C File Offset: 0x000C8A8C
		private bool DreamDashedIntoSolid()
		{
			if (base.CollideCheck<Solid>())
			{
				for (int i = 1; i <= 5; i++)
				{
					for (int j = -1; j <= 1; j += 2)
					{
						for (int k = 1; k <= 5; k++)
						{
							for (int l = -1; l <= 1; l += 2)
							{
								Vector2 value = new Vector2((float)(i * j), (float)(k * l));
								if (!base.CollideCheck<Solid>(this.Position + value))
								{
									this.Position += value;
									return false;
								}
							}
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06001D10 RID: 7440 RVA: 0x000CA910 File Offset: 0x000C8B10
		public bool StartStarFly()
		{
			this.RefillStamina();
			if (this.StateMachine.State == 18)
			{
				return false;
			}
			if (this.StateMachine.State == 19)
			{
				this.starFlyTimer = 2f;
				this.Sprite.Color = this.starFlyColor;
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			}
			else
			{
				this.StateMachine.State = 19;
			}
			return true;
		}

		// Token: 0x06001D11 RID: 7441 RVA: 0x000CA978 File Offset: 0x000C8B78
		private void StarFlyBegin()
		{
			this.Sprite.Play("startStarFly", false, false);
			this.starFlyTransforming = true;
			this.starFlyTimer = 2f;
			this.starFlySpeedLerp = 0f;
			this.jumpGraceTimer = 0f;
			if (this.starFlyBloom == null)
			{
				base.Add(this.starFlyBloom = new BloomPoint(new Vector2(0f, -6f), 0f, 16f));
			}
			this.starFlyBloom.Visible = true;
			this.starFlyBloom.Alpha = 0f;
			base.Collider = this.starFlyHitbox;
			this.hurtbox = this.starFlyHurtbox;
			if (this.starFlyLoopSfx == null)
			{
				base.Add(this.starFlyLoopSfx = new SoundSource());
				this.starFlyLoopSfx.DisposeOnTransition = false;
				base.Add(this.starFlyWarningSfx = new SoundSource());
				this.starFlyWarningSfx.DisposeOnTransition = false;
			}
			this.starFlyLoopSfx.Play("event:/game/06_reflection/feather_state_loop", "feather_speed", 1f);
			this.starFlyWarningSfx.Stop(true);
		}

		// Token: 0x06001D12 RID: 7442 RVA: 0x000CAA98 File Offset: 0x000C8C98
		private void StarFlyEnd()
		{
			this.Play("event:/game/06_reflection/feather_state_end", null, 0f);
			this.starFlyWarningSfx.Stop(true);
			this.starFlyLoopSfx.Stop(true);
			this.Hair.DrawPlayerSpriteOutline = false;
			this.Sprite.Color = Color.White;
			this.level.Displacement.AddBurst(base.Center, 0.25f, 8f, 32f, 1f, null, null);
			this.starFlyBloom.Visible = false;
			this.Sprite.HairCount = this.startHairCount;
			this.StarFlyReturnToNormalHitbox();
			if (this.StateMachine.State != 2)
			{
				this.level.Particles.Emit(FlyFeather.P_Boost, 12, base.Center, Vector2.One * 4f, (-this.Speed).Angle());
			}
		}

		// Token: 0x06001D13 RID: 7443 RVA: 0x000CAB88 File Offset: 0x000C8D88
		private void StarFlyReturnToNormalHitbox()
		{
			base.Collider = this.normalHitbox;
			this.hurtbox = this.normalHurtbox;
			if (!base.CollideCheck<Solid>())
			{
				return;
			}
			Vector2 position = this.Position;
			base.Y -= this.normalHitbox.Bottom - this.starFlyHitbox.Bottom;
			if (!base.CollideCheck<Solid>())
			{
				return;
			}
			this.Position = position;
			this.Ducking = true;
			base.Y -= this.duckHitbox.Bottom - this.starFlyHitbox.Bottom;
			if (base.CollideCheck<Solid>())
			{
				this.Position = position;
				throw new Exception("Could not get out of solids when exiting Star Fly State!");
			}
		}

		// Token: 0x06001D14 RID: 7444 RVA: 0x000CAC3D File Offset: 0x000C8E3D
		private IEnumerator StarFlyCoroutine()
		{
			while (this.Sprite.CurrentAnimationID == "startStarFly")
			{
				yield return null;
			}
			while (this.Speed != Vector2.Zero)
			{
				yield return null;
			}
			yield return 0.1f;
			this.Sprite.Color = this.starFlyColor;
			this.Sprite.HairCount = 7;
			this.Hair.DrawPlayerSpriteOutline = true;
			this.level.Displacement.AddBurst(base.Center, 0.25f, 8f, 32f, 1f, null, null);
			this.starFlyTransforming = false;
			this.starFlyTimer = 2f;
			this.RefillDash();
			this.RefillStamina();
			Vector2 vector = Input.Feather.Value;
			if (vector == Vector2.Zero)
			{
				vector = Vector2.UnitX * (float)this.Facing;
			}
			this.Speed = vector * 250f;
			this.starFlyLastDir = vector;
			this.level.Particles.Emit(FlyFeather.P_Boost, 12, base.Center, Vector2.One * 4f, (-vector).Angle());
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			this.level.DirectionalShake(this.starFlyLastDir, 0.3f);
			while (this.starFlyTimer > 0.5f)
			{
				yield return null;
			}
			this.starFlyWarningSfx.Play("event:/game/06_reflection/feather_state_warning", null, 0f);
			yield break;
		}

		// Token: 0x06001D15 RID: 7445 RVA: 0x000CAC4C File Offset: 0x000C8E4C
		private int StarFlyUpdate()
		{
			this.starFlyBloom.Alpha = Calc.Approach(this.starFlyBloom.Alpha, 0.7f, Engine.DeltaTime * 2f);
			Input.Rumble(RumbleStrength.Climb, RumbleLength.Short);
			if (this.starFlyTransforming)
			{
				this.Speed = Calc.Approach(this.Speed, Vector2.Zero, 1000f * Engine.DeltaTime);
			}
			else
			{
				Vector2 value = Input.Feather.Value;
				bool flag = false;
				if (value == Vector2.Zero)
				{
					flag = true;
					value = this.starFlyLastDir;
				}
				Vector2 vector = this.Speed.SafeNormalize(Vector2.Zero);
				if (vector == Vector2.Zero)
				{
					vector = value;
				}
				else
				{
					vector = vector.RotateTowards(value.Angle(), 5.5850534f * Engine.DeltaTime);
				}
				this.starFlyLastDir = vector;
				float target;
				if (flag)
				{
					this.starFlySpeedLerp = 0f;
					target = 91f;
				}
				else if (vector != Vector2.Zero && Vector2.Dot(vector, value) >= 0.45f)
				{
					this.starFlySpeedLerp = Calc.Approach(this.starFlySpeedLerp, 1f, Engine.DeltaTime / 1f);
					target = MathHelper.Lerp(140f, 190f, this.starFlySpeedLerp);
				}
				else
				{
					this.starFlySpeedLerp = 0f;
					target = 140f;
				}
				this.starFlyLoopSfx.Param("feather_speed", (float)(flag ? 0 : 1));
				float num = this.Speed.Length();
				num = Calc.Approach(num, target, 1000f * Engine.DeltaTime);
				this.Speed = vector * num;
				if (this.level.OnInterval(0.02f))
				{
					this.level.Particles.Emit(FlyFeather.P_Flying, 1, base.Center, Vector2.One * 2f, (-this.Speed).Angle());
				}
				if (Input.Jump.Pressed)
				{
					if (base.OnGround(3))
					{
						this.Jump(true, true);
						return 0;
					}
					if (this.WallJumpCheck(-1))
					{
						this.WallJump(1);
						return 0;
					}
					if (this.WallJumpCheck(1))
					{
						this.WallJump(-1);
						return 0;
					}
				}
				if (Input.GrabCheck)
				{
					bool flag2 = false;
					int dir = 0;
					if (Input.MoveX.Value != -1 && this.ClimbCheck(1, 0))
					{
						this.Facing = Facings.Right;
						dir = 1;
						flag2 = true;
					}
					else if (Input.MoveX.Value != 1 && this.ClimbCheck(-1, 0))
					{
						this.Facing = Facings.Left;
						dir = -1;
						flag2 = true;
					}
					if (flag2)
					{
						if (SaveData.Instance.Assists.NoGrabbing)
						{
							this.Speed = Vector2.Zero;
							this.ClimbTrigger(dir);
							return 0;
						}
						return 1;
					}
				}
				if (this.CanDash)
				{
					return this.StartDash();
				}
				this.starFlyTimer -= Engine.DeltaTime;
				if (this.starFlyTimer <= 0f)
				{
					if (Input.MoveY.Value == -1)
					{
						this.Speed.Y = -100f;
					}
					if (Input.MoveY.Value < 1)
					{
						this.varJumpSpeed = this.Speed.Y;
						this.AutoJump = true;
						this.AutoJumpTimer = 0f;
						this.varJumpTimer = 0.2f;
					}
					if (this.Speed.Y > 0f)
					{
						this.Speed.Y = 0f;
					}
					if (Math.Abs(this.Speed.X) > 140f)
					{
						this.Speed.X = 140f * (float)Math.Sign(this.Speed.X);
					}
					Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
					return 0;
				}
				if (this.starFlyTimer < 0.5f && base.Scene.OnInterval(0.05f))
				{
					if (this.Sprite.Color == this.starFlyColor)
					{
						this.Sprite.Color = Player.NormalHairColor;
					}
					else
					{
						this.Sprite.Color = this.starFlyColor;
					}
				}
			}
			return 19;
		}

		// Token: 0x06001D16 RID: 7446 RVA: 0x000CB03D File Offset: 0x000C923D
		public bool DoFlingBird(FlingBird bird)
		{
			if (!this.Dead && this.StateMachine.State != 24)
			{
				this.flingBird = bird;
				this.StateMachine.State = 24;
				if (this.Holding != null)
				{
					this.Drop();
				}
				return true;
			}
			return false;
		}

		// Token: 0x06001D17 RID: 7447 RVA: 0x000CB07C File Offset: 0x000C927C
		public void FinishFlingBird()
		{
			this.StateMachine.State = 0;
			this.AutoJump = true;
			this.forceMoveX = 1;
			this.forceMoveXTimer = 0.2f;
			this.Speed = FlingBird.FlingSpeed;
			this.varJumpTimer = 0.2f;
			this.varJumpSpeed = this.Speed.Y;
			this.launched = true;
		}

		// Token: 0x06001D18 RID: 7448 RVA: 0x000CB0DC File Offset: 0x000C92DC
		private void FlingBirdBegin()
		{
			this.RefillDash();
			this.RefillStamina();
		}

		// Token: 0x06001D19 RID: 7449 RVA: 0x000091E2 File Offset: 0x000073E2
		private void FlingBirdEnd()
		{
		}

		// Token: 0x06001D1A RID: 7450 RVA: 0x000CB0EC File Offset: 0x000C92EC
		private int FlingBirdUpdate()
		{
			base.MoveTowardsX(this.flingBird.X, 250f * Engine.DeltaTime, null);
			base.MoveTowardsY(this.flingBird.Y + 8f + base.Collider.Height, 250f * Engine.DeltaTime, null);
			return 24;
		}

		// Token: 0x06001D1B RID: 7451 RVA: 0x000CB147 File Offset: 0x000C9347
		private IEnumerator FlingBirdCoroutine()
		{
			yield break;
		}

		// Token: 0x06001D1C RID: 7452 RVA: 0x000CB150 File Offset: 0x000C9350
		public void StartCassetteFly(Vector2 targetPosition, Vector2 control)
		{
			this.StateMachine.State = 21;
			this.cassetteFlyCurve = new SimpleCurve(this.Position, targetPosition, control);
			this.cassetteFlyLerp = 0f;
			this.Speed = Vector2.Zero;
			if (this.Holding != null)
			{
				this.Drop();
			}
		}

		// Token: 0x06001D1D RID: 7453 RVA: 0x000CB1A1 File Offset: 0x000C93A1
		private void CassetteFlyBegin()
		{
			this.Sprite.Play("bubble", false, false);
			this.Sprite.Y += 5f;
		}

		// Token: 0x06001D1E RID: 7454 RVA: 0x000091E2 File Offset: 0x000073E2
		private void CassetteFlyEnd()
		{
		}

		// Token: 0x06001D1F RID: 7455 RVA: 0x000CB1CC File Offset: 0x000C93CC
		private int CassetteFlyUpdate()
		{
			return 21;
		}

		// Token: 0x06001D20 RID: 7456 RVA: 0x000CB1D0 File Offset: 0x000C93D0
		private IEnumerator CassetteFlyCoroutine()
		{
			this.level.CanRetry = false;
			this.level.FormationBackdrop.Display = true;
			this.level.FormationBackdrop.Alpha = 0.5f;
			this.Sprite.Scale = Vector2.One * 1.25f;
			base.Depth = -2000000;
			yield return 0.4f;
			while (this.cassetteFlyLerp < 1f)
			{
				if (this.level.OnInterval(0.03f))
				{
					this.level.Particles.Emit(Player.P_CassetteFly, 2, base.Center, Vector2.One * 4f);
				}
				this.cassetteFlyLerp = Calc.Approach(this.cassetteFlyLerp, 1f, 1.6f * Engine.DeltaTime);
				this.Position = this.cassetteFlyCurve.GetPoint(Ease.SineInOut(this.cassetteFlyLerp));
				this.level.Camera.Position = this.CameraTarget;
				yield return null;
			}
			this.Position = this.cassetteFlyCurve.End;
			this.Sprite.Scale = Vector2.One * 1.25f;
			this.Sprite.Y -= 5f;
			this.Sprite.Play("fallFast", false, false);
			yield return 0.2f;
			this.level.CanRetry = true;
			this.level.FormationBackdrop.Display = false;
			this.level.FormationBackdrop.Alpha = 0.5f;
			this.StateMachine.State = 0;
			base.Depth = 0;
			yield break;
		}

		// Token: 0x06001D21 RID: 7457 RVA: 0x000CB1DF File Offset: 0x000C93DF
		public void StartAttract(Vector2 attractTo)
		{
			this.attractTo = attractTo.Round();
			this.StateMachine.State = 22;
		}

		// Token: 0x06001D22 RID: 7458 RVA: 0x000CB1FA File Offset: 0x000C93FA
		private void AttractBegin()
		{
			this.Speed = Vector2.Zero;
		}

		// Token: 0x06001D23 RID: 7459 RVA: 0x000091E2 File Offset: 0x000073E2
		private void AttractEnd()
		{
		}

		// Token: 0x06001D24 RID: 7460 RVA: 0x000CB208 File Offset: 0x000C9408
		private int AttractUpdate()
		{
			if (Vector2.Distance(this.attractTo, base.ExactPosition) <= 1.5f)
			{
				this.Position = this.attractTo;
				base.ZeroRemainderX();
				base.ZeroRemainderY();
			}
			else
			{
				Vector2 vector = Calc.Approach(base.ExactPosition, this.attractTo, 200f * Engine.DeltaTime);
				base.MoveToX(vector.X, null);
				base.MoveToY(vector.Y, null);
			}
			return 22;
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x06001D25 RID: 7461 RVA: 0x000CB280 File Offset: 0x000C9480
		public bool AtAttractTarget
		{
			get
			{
				return this.StateMachine.State == 22 && base.ExactPosition == this.attractTo;
			}
		}

		// Token: 0x06001D26 RID: 7462 RVA: 0x000CB2A4 File Offset: 0x000C94A4
		private void DummyBegin()
		{
			this.DummyMoving = false;
			this.DummyGravity = true;
			this.DummyAutoAnimate = true;
		}

		// Token: 0x06001D27 RID: 7463 RVA: 0x000CB2BC File Offset: 0x000C94BC
		private int DummyUpdate()
		{
			if (this.CanUnDuck)
			{
				this.Ducking = false;
			}
			if (!this.onGround && this.DummyGravity)
			{
				float num = (Math.Abs(this.Speed.Y) < 40f && (Input.Jump.Check || this.AutoJump)) ? 0.5f : 1f;
				if (this.level.InSpace)
				{
					num *= 0.6f;
				}
				this.Speed.Y = Calc.Approach(this.Speed.Y, 160f, 900f * num * Engine.DeltaTime);
			}
			if (this.varJumpTimer > 0f)
			{
				if (this.AutoJump || Input.Jump.Check)
				{
					this.Speed.Y = Math.Min(this.Speed.Y, this.varJumpSpeed);
				}
				else
				{
					this.varJumpTimer = 0f;
				}
			}
			if (!this.DummyMoving)
			{
				if (Math.Abs(this.Speed.X) > 90f && this.DummyMaxspeed)
				{
					this.Speed.X = Calc.Approach(this.Speed.X, 90f * (float)Math.Sign(this.Speed.X), 2500f * Engine.DeltaTime);
				}
				if (this.DummyFriction)
				{
					this.Speed.X = Calc.Approach(this.Speed.X, 0f, 1000f * Engine.DeltaTime);
				}
			}
			if (this.DummyAutoAnimate)
			{
				if (this.onGround)
				{
					if (this.Speed.X == 0f)
					{
						this.Sprite.Play("idle", false, false);
					}
					else
					{
						this.Sprite.Play("walk", false, false);
					}
				}
				else if (this.Speed.Y < 0f)
				{
					this.Sprite.Play("jumpSlow", false, false);
				}
				else
				{
					this.Sprite.Play("fallSlow", false, false);
				}
			}
			return 11;
		}

		// Token: 0x06001D28 RID: 7464 RVA: 0x000CB4D2 File Offset: 0x000C96D2
		public IEnumerator DummyWalkTo(float x, bool walkBackwards = false, float speedMultiplier = 1f, bool keepWalkingIntoWalls = false)
		{
			this.StateMachine.State = 11;
			if (Math.Abs(base.X - x) > 4f && !this.Dead)
			{
				this.DummyMoving = true;
				if (walkBackwards)
				{
					this.Sprite.Rate = -1f;
					this.Facing = (Facings)Math.Sign(base.X - x);
				}
				else
				{
					this.Facing = (Facings)Math.Sign(x - base.X);
				}
				while (Math.Abs(x - base.X) > 4f && base.Scene != null && (keepWalkingIntoWalls || !base.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float)Math.Sign(x - base.X))))
				{
					this.Speed.X = Calc.Approach(this.Speed.X, (float)Math.Sign(x - base.X) * 64f * speedMultiplier, 1000f * Engine.DeltaTime);
					yield return null;
				}
				this.Sprite.Rate = 1f;
				this.Sprite.Play("idle", false, false);
				this.DummyMoving = false;
			}
			yield break;
		}

		// Token: 0x06001D29 RID: 7465 RVA: 0x000CB4FE File Offset: 0x000C96FE
		public IEnumerator DummyWalkToExact(int x, bool walkBackwards = false, float speedMultiplier = 1f, bool cancelOnFall = false)
		{
			this.StateMachine.State = 11;
			if (base.X != (float)x)
			{
				this.DummyMoving = true;
				if (walkBackwards)
				{
					this.Sprite.Rate = -1f;
					this.Facing = (Facings)Math.Sign(base.X - (float)x);
				}
				else
				{
					this.Facing = (Facings)Math.Sign((float)x - base.X);
				}
				int last = Math.Sign(base.X - (float)x);
				while (!this.Dead && base.X != (float)x && !base.CollideCheck<Solid>(this.Position + new Vector2((float)this.Facing, 0f)) && (!cancelOnFall || base.OnGround(1)))
				{
					this.Speed.X = Calc.Approach(this.Speed.X, (float)Math.Sign((float)x - base.X) * 64f * speedMultiplier, 1000f * Engine.DeltaTime);
					int num = Math.Sign(base.X - (float)x);
					if (num != last)
					{
						base.X = (float)x;
						break;
					}
					last = num;
					yield return null;
				}
				this.Speed.X = 0f;
				this.Sprite.Rate = 1f;
				this.Sprite.Play("idle", false, false);
				this.DummyMoving = false;
			}
			yield break;
		}

		// Token: 0x06001D2A RID: 7466 RVA: 0x000CB52A File Offset: 0x000C972A
		public IEnumerator DummyRunTo(float x, bool fastAnim = false)
		{
			this.StateMachine.State = 11;
			if (Math.Abs(base.X - x) > 4f)
			{
				this.DummyMoving = true;
				if (fastAnim)
				{
					this.Sprite.Play("runFast", false, false);
				}
				else if (!this.Sprite.LastAnimationID.StartsWith("run"))
				{
					this.Sprite.Play("runSlow", false, false);
				}
				this.Facing = (Facings)Math.Sign(x - base.X);
				while (Math.Abs(base.X - x) > 4f)
				{
					this.Speed.X = Calc.Approach(this.Speed.X, (float)Math.Sign(x - base.X) * 90f, 1000f * Engine.DeltaTime);
					yield return null;
				}
				this.Sprite.Play("idle", false, false);
				this.DummyMoving = false;
			}
			yield break;
		}

		// Token: 0x06001D2B RID: 7467 RVA: 0x000CB547 File Offset: 0x000C9747
		private int FrozenUpdate()
		{
			return 17;
		}

		// Token: 0x06001D2C RID: 7468 RVA: 0x000CB54C File Offset: 0x000C974C
		private int TempleFallUpdate()
		{
			this.Facing = Facings.Right;
			if (!this.onGround)
			{
				int num = this.level.Bounds.Left + 160;
				int num2;
				if (Math.Abs((float)num - base.X) > 4f)
				{
					num2 = Math.Sign((float)num - base.X);
				}
				else
				{
					num2 = 0;
				}
				this.Speed.X = Calc.Approach(this.Speed.X, 54.000004f * (float)num2, 325f * Engine.DeltaTime);
			}
			if (!this.onGround && this.DummyGravity)
			{
				this.Speed.Y = Calc.Approach(this.Speed.Y, 320f, 225f * Engine.DeltaTime);
			}
			return 20;
		}

		// Token: 0x06001D2D RID: 7469 RVA: 0x000CB615 File Offset: 0x000C9815
		private IEnumerator TempleFallCoroutine()
		{
			this.Sprite.Play("fallFast", false, false);
			while (!this.onGround)
			{
				yield return null;
			}
			this.Play("event:/char/madeline/mirrortemple_big_landing", null, 0f);
			if (this.Dashes <= 1)
			{
				this.Sprite.Play("fallPose", false, false);
			}
			else
			{
				this.Sprite.Play("idle", false, false);
			}
			this.Sprite.Scale.Y = 0.7f;
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			this.level.DirectionalShake(new Vector2(0f, 1f), 0.5f);
			this.Speed.X = 0f;
			this.level.Particles.Emit(Player.P_SummitLandA, 12, base.BottomCenter, Vector2.UnitX * 3f, -1.5707964f);
			this.level.Particles.Emit(Player.P_SummitLandB, 8, base.BottomCenter - Vector2.UnitX * 2f, Vector2.UnitX * 2f, 3.403392f);
			this.level.Particles.Emit(Player.P_SummitLandB, 8, base.BottomCenter + Vector2.UnitX * 2f, Vector2.UnitX * 2f, -0.2617994f);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime)
			{
				yield return null;
			}
			this.StateMachine.State = 0;
			yield break;
		}

		// Token: 0x06001D2E RID: 7470 RVA: 0x000CB624 File Offset: 0x000C9824
		private void ReflectionFallBegin()
		{
			this.IgnoreJumpThrus = true;
		}

		// Token: 0x06001D2F RID: 7471 RVA: 0x000CB62D File Offset: 0x000C982D
		private void ReflectionFallEnd()
		{
			FallEffects.Show(false);
			this.IgnoreJumpThrus = false;
		}

		// Token: 0x06001D30 RID: 7472 RVA: 0x000CB63C File Offset: 0x000C983C
		private int ReflectionFallUpdate()
		{
			this.Facing = Facings.Right;
			if (base.Scene.OnInterval(0.05f))
			{
				this.wasDashB = true;
				this.CreateTrail();
			}
			if (base.CollideCheck<Water>())
			{
				this.Speed.Y = Calc.Approach(this.Speed.Y, -20f, 400f * Engine.DeltaTime);
			}
			else
			{
				this.Speed.Y = Calc.Approach(this.Speed.Y, 320f, 225f * Engine.DeltaTime);
			}
			foreach (Entity entity in base.Scene.Tracker.GetEntities<FlyFeather>())
			{
				entity.RemoveSelf();
			}
			CrystalStaticSpinner crystalStaticSpinner = base.Scene.CollideFirst<CrystalStaticSpinner>(new Rectangle((int)(base.X - 6f), (int)(base.Y - 6f), 12, 12));
			if (crystalStaticSpinner != null)
			{
				crystalStaticSpinner.Destroy(false);
				this.level.Shake(0.3f);
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
				Celeste.Freeze(0.01f);
			}
			return 18;
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x000CB778 File Offset: 0x000C9978
		private IEnumerator ReflectionFallCoroutine()
		{
			this.Sprite.Play("bigFall", false, false);
			this.level.StartCutscene(new Action<Level>(this.OnReflectionFallSkip), true, false, true);
			for (float t = 0f; t < 2f; t += Engine.DeltaTime)
			{
				this.Speed.Y = 0f;
				yield return null;
			}
			FallEffects.Show(true);
			this.Speed.Y = 320f;
			while (!base.CollideCheck<Water>())
			{
				yield return null;
			}
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			FallEffects.Show(false);
			this.Sprite.Play("bigFallRecover", false, false);
			this.level.Session.Audio.Music.Event = "event:/music/lvl6/main";
			this.level.Session.Audio.Apply(false);
			this.level.EndCutscene();
			yield return 1.2f;
			this.StateMachine.State = 0;
			yield break;
		}

		// Token: 0x06001D32 RID: 7474 RVA: 0x000CB788 File Offset: 0x000C9988
		private void OnReflectionFallSkip(Level level)
		{
			level.OnEndOfFrame += delegate()
			{
				level.Remove(this);
				level.UnloadLevel();
				level.Session.Level = "00";
				level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float)level.Bounds.Left, (float)level.Bounds.Bottom)));
				level.LoadLevel(Player.IntroTypes.None, false);
				FallEffects.Show(false);
				level.Session.Audio.Music.Event = "event:/music/lvl6/main";
				level.Session.Audio.Apply(false);
			};
		}

		// Token: 0x06001D33 RID: 7475 RVA: 0x000CB7C0 File Offset: 0x000C99C0
		public IEnumerator IntroWalkCoroutine()
		{
			Vector2 start = this.Position;
			if (this.IntroWalkDirection == Facings.Right)
			{
				base.X = (float)(this.level.Bounds.Left - 16);
				this.Facing = Facings.Right;
			}
			else
			{
				base.X = (float)(this.level.Bounds.Right + 16);
				this.Facing = Facings.Left;
			}
			yield return 0.3f;
			this.Sprite.Play("runSlow", false, false);
			while (Math.Abs(base.X - start.X) > 2f && !base.CollideCheck<Solid>(this.Position + new Vector2((float)this.Facing, 0f)))
			{
				base.MoveTowardsX(start.X, 64f * Engine.DeltaTime, null);
				yield return null;
			}
			this.Position = start;
			this.Sprite.Play("idle", false, false);
			yield return 0.2f;
			this.StateMachine.State = 0;
			yield break;
		}

		// Token: 0x06001D34 RID: 7476 RVA: 0x000CB7CF File Offset: 0x000C99CF
		private IEnumerator IntroJumpCoroutine()
		{
			Vector2 start = this.Position;
			bool wasSummitJump = this.StateMachine.PreviousState == 10;
			base.Depth = -1000000;
			this.Facing = Facings.Right;
			if (!wasSummitJump)
			{
				base.Y = (float)(this.level.Bounds.Bottom + 16);
				yield return 0.5f;
			}
			else
			{
				start.Y = (float)(this.level.Bounds.Bottom - 24);
				base.MoveToX((float)((int)Math.Round((double)(base.X / 8f)) * 8), null);
			}
			if (!wasSummitJump)
			{
				this.Sprite.Play("jumpSlow", false, false);
			}
			while (base.Y > start.Y - 8f)
			{
				base.Y += -120f * Engine.DeltaTime;
				yield return null;
			}
			base.Y = (float)Math.Round((double)base.Y);
			this.Speed.Y = -100f;
			while (this.Speed.Y < 0f)
			{
				this.Speed.Y = this.Speed.Y + Engine.DeltaTime * 800f;
				yield return null;
			}
			this.Speed.Y = 0f;
			if (wasSummitJump)
			{
				yield return 0.2f;
				this.Play("event:/char/madeline/summit_areastart", null, 0f);
				this.Sprite.Play("launchRecover", false, false);
				yield return 0.1f;
			}
			else
			{
				yield return 0.1f;
			}
			if (!wasSummitJump)
			{
				this.Sprite.Play("fallSlow", false, false);
			}
			while (!this.onGround)
			{
				this.Speed.Y = this.Speed.Y + Engine.DeltaTime * 800f;
				yield return null;
			}
			if (this.StateMachine.PreviousState != 10)
			{
				this.Position = start;
			}
			base.Depth = 0;
			this.level.DirectionalShake(Vector2.UnitY, 0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			if (wasSummitJump)
			{
				this.level.Particles.Emit(Player.P_SummitLandA, 12, base.BottomCenter, Vector2.UnitX * 3f, -1.5707964f);
				this.level.Particles.Emit(Player.P_SummitLandB, 8, base.BottomCenter - Vector2.UnitX * 2f, Vector2.UnitX * 2f, 3.403392f);
				this.level.Particles.Emit(Player.P_SummitLandB, 8, base.BottomCenter + Vector2.UnitX * 2f, Vector2.UnitX * 2f, -0.2617994f);
				this.level.ParticlesBG.Emit(Player.P_SummitLandC, 30, base.BottomCenter, Vector2.UnitX * 5f);
				yield return 0.35f;
				for (int i = 0; i < this.Hair.Nodes.Count; i++)
				{
					this.Hair.Nodes[i] = new Vector2(0f, (float)(2 + i));
				}
			}
			this.StateMachine.State = 0;
			yield break;
		}

		// Token: 0x06001D35 RID: 7477 RVA: 0x000CB7DE File Offset: 0x000C99DE
		private IEnumerator IntroMoonJumpCoroutine()
		{
			Vector2 start = this.Position;
			this.Facing = Facings.Right;
			this.Speed = Vector2.Zero;
			this.Visible = false;
			base.Y = (float)(this.level.Bounds.Bottom + 16);
			yield return 0.5f;
			yield return this.MoonLanding(start);
			this.StateMachine.State = 0;
			yield break;
		}

		// Token: 0x06001D36 RID: 7478 RVA: 0x000CB7ED File Offset: 0x000C99ED
		public IEnumerator MoonLanding(Vector2 groundPosition)
		{
			base.Depth = -1000000;
			this.Speed = Vector2.Zero;
			this.Visible = true;
			this.Sprite.Play("jumpSlow", false, false);
			while (base.Y > groundPosition.Y - 8f)
			{
				base.MoveV(-200f * Engine.DeltaTime, null, null);
				yield return null;
			}
			this.Speed.Y = -200f;
			while (this.Speed.Y < 0f)
			{
				this.Speed.Y = this.Speed.Y + Engine.DeltaTime * 400f;
				yield return null;
			}
			this.Speed.Y = 0f;
			yield return 0.2f;
			this.Sprite.Play("fallSlow", false, false);
			float s = 100f;
			while (!base.OnGround(1))
			{
				this.Speed.Y = this.Speed.Y + Engine.DeltaTime * s;
				s = Calc.Approach(s, 2f, Engine.DeltaTime * 50f);
				yield return null;
			}
			base.Depth = 0;
			yield break;
		}

		// Token: 0x06001D37 RID: 7479 RVA: 0x000CB803 File Offset: 0x000C9A03
		private IEnumerator IntroWakeUpCoroutine()
		{
			this.Sprite.Play("asleep", false, false);
			yield return 0.5f;
			yield return this.Sprite.PlayRoutine("wakeUp", false);
			yield return 0.2f;
			this.StateMachine.State = 0;
			yield break;
		}

		// Token: 0x06001D38 RID: 7480 RVA: 0x000CB814 File Offset: 0x000C9A14
		private void IntroRespawnBegin()
		{
			this.Play("event:/char/madeline/revive", null, 0f);
			base.Depth = -1000000;
			this.introEase = 1f;
			Vector2 from = this.Position;
			from.X = MathHelper.Clamp(from.X, (float)this.level.Bounds.Left + 40f, (float)this.level.Bounds.Right - 40f);
			from.Y = MathHelper.Clamp(from.Y, (float)this.level.Bounds.Top + 40f, (float)this.level.Bounds.Bottom - 40f);
			this.deadOffset = from;
			from -= this.Position;
			this.respawnTween = Tween.Create(Tween.TweenMode.Oneshot, null, 0.6f, true);
			this.respawnTween.OnUpdate = delegate(Tween t)
			{
				this.deadOffset = Vector2.Lerp(from, Vector2.Zero, t.Eased);
				this.introEase = 1f - t.Eased;
			};
			this.respawnTween.OnComplete = delegate(Tween t)
			{
				if (this.StateMachine.State == 14)
				{
					this.StateMachine.State = 0;
					this.Sprite.Scale = new Vector2(1.5f, 0.5f);
				}
			};
			base.Add(this.respawnTween);
		}

		// Token: 0x06001D39 RID: 7481 RVA: 0x000CB970 File Offset: 0x000C9B70
		private void IntroRespawnEnd()
		{
			base.Depth = 0;
			this.deadOffset = Vector2.Zero;
			base.Remove(this.respawnTween);
			this.respawnTween = null;
		}

		// Token: 0x06001D3A RID: 7482 RVA: 0x000CB997 File Offset: 0x000C9B97
		public IEnumerator IntroThinkForABitCoroutine()
		{
			(base.Scene as Level).Camera.X += 8f;
			yield return 0.1f;
			this.Sprite.Play("walk", false, false);
			float target = base.X + 8f;
			while (base.X < target)
			{
				base.MoveH(32f * Engine.DeltaTime, null, null);
				yield return null;
			}
			this.Sprite.Play("idle", false, false);
			yield return 0.3f;
			this.Facing = Facings.Left;
			yield return 0.8f;
			this.Facing = Facings.Right;
			yield return 0.1f;
			this.StateMachine.State = 0;
			yield break;
		}

		// Token: 0x06001D3B RID: 7483 RVA: 0x000CB9A6 File Offset: 0x000C9BA6
		private void BirdDashTutorialBegin()
		{
			this.DashBegin();
			this.Play("event:/char/madeline/dash_red_right", null, 0f);
			this.Sprite.Play("dash", false, false);
		}

		// Token: 0x06001D3C RID: 7484 RVA: 0x000CB9D2 File Offset: 0x000C9BD2
		private int BirdDashTutorialUpdate()
		{
			return 16;
		}

		// Token: 0x06001D3D RID: 7485 RVA: 0x000CB9D6 File Offset: 0x000C9BD6
		private IEnumerator BirdDashTutorialCoroutine()
		{
			yield return null;
			this.CreateTrail();
			base.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, new Action(this.CreateTrail), 0.08f, true));
			base.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, new Action(this.CreateTrail), 0.15f, true));
			Vector2 vector = new Vector2(1f, -1f).SafeNormalize();
			this.Facing = Facings.Right;
			this.Speed = vector * 240f;
			this.DashDir = vector;
			base.SceneAs<Level>().DirectionalShake(this.DashDir, 0.2f);
			SlashFx.Burst(base.Center, this.DashDir.Angle());
			for (float time = 0f; time < 0.15f; time += Engine.DeltaTime)
			{
				if (this.Speed != Vector2.Zero && this.level.OnInterval(0.02f))
				{
					this.level.ParticlesFG.Emit(Player.P_DashA, base.Center + Calc.Random.Range(Vector2.One * -2f, Vector2.One * 2f), this.DashDir.Angle());
				}
				yield return null;
			}
			this.AutoJump = true;
			this.AutoJumpTimer = 0f;
			if (this.DashDir.Y <= 0f)
			{
				this.Speed = this.DashDir * 160f;
			}
			if (this.Speed.Y < 0f)
			{
				this.Speed.Y = this.Speed.Y * 0.75f;
			}
			this.Sprite.Play("fallFast", false, false);
			bool climbing = false;
			while (!base.OnGround(1) && !climbing)
			{
				this.Speed.Y = Calc.Approach(this.Speed.Y, 160f, 900f * Engine.DeltaTime);
				if (base.CollideCheck<Solid>(this.Position + new Vector2(1f, 0f)))
				{
					climbing = true;
				}
				if (base.Top > (float)this.level.Bounds.Bottom)
				{
					this.level.CancelCutscene();
					this.Die(Vector2.Zero, false, true);
				}
				yield return null;
			}
			if (climbing)
			{
				this.Sprite.Play("wallslide", false, false);
				Dust.Burst(this.Position + new Vector2(4f, -6f), new Vector2(-4f, 0f).Angle(), 1, null);
				this.Speed.Y = 0f;
				yield return 0.2f;
				this.Sprite.Play("climbUp", false, false);
				while (base.CollideCheck<Solid>(this.Position + new Vector2(1f, 0f)))
				{
					base.Y += -45f * Engine.DeltaTime;
					yield return null;
				}
				base.Y = (float)Math.Round((double)base.Y);
				this.Play("event:/char/madeline/climb_ledge", null, 0f);
				this.Sprite.Play("jumpFast", false, false);
				this.Speed.Y = -105f;
				while (!base.OnGround(1))
				{
					this.Speed.Y = Calc.Approach(this.Speed.Y, 160f, 900f * Engine.DeltaTime);
					this.Speed.X = 20f;
					yield return null;
				}
				this.Speed.X = 0f;
				this.Speed.Y = 0f;
				this.Sprite.Play("walk", false, false);
				for (float time = 0f; time < 0.5f; time += Engine.DeltaTime)
				{
					base.X += 32f * Engine.DeltaTime;
					yield return null;
				}
				this.Sprite.Play("tired", false, false);
			}
			else
			{
				this.Sprite.Play("tired", false, false);
				this.Speed.Y = 0f;
				while (this.Speed.X != 0f)
				{
					this.Speed.X = Calc.Approach(this.Speed.X, 0f, 240f * Engine.DeltaTime);
					if (base.Scene.OnInterval(0.04f))
					{
						Dust.Burst(base.BottomCenter + new Vector2(0f, -2f), -2.3561945f, 1, null);
					}
					yield return null;
				}
			}
			yield break;
		}

		// Token: 0x06001D3E RID: 7486 RVA: 0x000CB9E8 File Offset: 0x000C9BE8
		public EventInstance Play(string sound, string param = null, float value = 0f)
		{
			float value2 = 0f;
			Level level = base.Scene as Level;
			if (level != null && level.Raining)
			{
				value2 = 1f;
			}
			this.AddChaserStateSound(sound, param, value, Player.ChaserStateSound.Actions.Oneshot);
			return Audio.Play(sound, base.Center, param, value, "raining", value2);
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x000CBA36 File Offset: 0x000C9C36
		public void Loop(SoundSource sfx, string sound)
		{
			this.AddChaserStateSound(sound, null, 0f, Player.ChaserStateSound.Actions.Loop);
			sfx.Play(sound, null, 0f);
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x000CBA54 File Offset: 0x000C9C54
		public void Stop(SoundSource sfx)
		{
			if (sfx.Playing)
			{
				this.AddChaserStateSound(sfx.EventName, null, 0f, Player.ChaserStateSound.Actions.Stop);
				sfx.Stop(true);
			}
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x000CBA79 File Offset: 0x000C9C79
		private void AddChaserStateSound(string sound, Player.ChaserStateSound.Actions action)
		{
			this.AddChaserStateSound(sound, null, 0f, action);
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x000CBA8C File Offset: 0x000C9C8C
		private void AddChaserStateSound(string sound, string param = null, float value = 0f, Player.ChaserStateSound.Actions action = Player.ChaserStateSound.Actions.Oneshot)
		{
			string text = null;
			SFX.MadelineToBadelineSound.TryGetValue(sound, out text);
			if (text != null)
			{
				this.activeSounds.Add(new Player.ChaserStateSound
				{
					Event = text,
					Parameter = param,
					ParameterValue = value,
					Action = action
				});
			}
		}

		// Token: 0x06001D43 RID: 7491 RVA: 0x000CBAE1 File Offset: 0x000C9CE1
		private ParticleType DustParticleFromSurfaceIndex(int index)
		{
			if (index == 40)
			{
				return ParticleTypes.SparkyDust;
			}
			return ParticleTypes.Dust;
		}

		// Token: 0x0400197B RID: 6523
		public static ParticleType P_DashA;

		// Token: 0x0400197C RID: 6524
		public static ParticleType P_DashB;

		// Token: 0x0400197D RID: 6525
		public static ParticleType P_DashBadB;

		// Token: 0x0400197E RID: 6526
		public static ParticleType P_CassetteFly;

		// Token: 0x0400197F RID: 6527
		public static ParticleType P_Split;

		// Token: 0x04001980 RID: 6528
		public static ParticleType P_SummitLandA;

		// Token: 0x04001981 RID: 6529
		public static ParticleType P_SummitLandB;

		// Token: 0x04001982 RID: 6530
		public static ParticleType P_SummitLandC;

		// Token: 0x04001983 RID: 6531
		public const float MaxFall = 160f;

		// Token: 0x04001984 RID: 6532
		private const float Gravity = 900f;

		// Token: 0x04001985 RID: 6533
		private const float HalfGravThreshold = 40f;

		// Token: 0x04001986 RID: 6534
		private const float FastMaxFall = 240f;

		// Token: 0x04001987 RID: 6535
		private const float FastMaxAccel = 300f;

		// Token: 0x04001988 RID: 6536
		public const float MaxRun = 90f;

		// Token: 0x04001989 RID: 6537
		public const float RunAccel = 1000f;

		// Token: 0x0400198A RID: 6538
		private const float RunReduce = 400f;

		// Token: 0x0400198B RID: 6539
		private const float AirMult = 0.65f;

		// Token: 0x0400198C RID: 6540
		private const float HoldingMaxRun = 70f;

		// Token: 0x0400198D RID: 6541
		private const float HoldMinTime = 0.35f;

		// Token: 0x0400198E RID: 6542
		private const float BounceAutoJumpTime = 0.1f;

		// Token: 0x0400198F RID: 6543
		private const float DuckFriction = 500f;

		// Token: 0x04001990 RID: 6544
		private const int DuckCorrectCheck = 4;

		// Token: 0x04001991 RID: 6545
		private const float DuckCorrectSlide = 50f;

		// Token: 0x04001992 RID: 6546
		private const float DodgeSlideSpeedMult = 1.2f;

		// Token: 0x04001993 RID: 6547
		private const float DuckSuperJumpXMult = 1.25f;

		// Token: 0x04001994 RID: 6548
		private const float DuckSuperJumpYMult = 0.5f;

		// Token: 0x04001995 RID: 6549
		private const float JumpGraceTime = 0.1f;

		// Token: 0x04001996 RID: 6550
		private const float JumpSpeed = -105f;

		// Token: 0x04001997 RID: 6551
		private const float JumpHBoost = 40f;

		// Token: 0x04001998 RID: 6552
		private const float VarJumpTime = 0.2f;

		// Token: 0x04001999 RID: 6553
		private const float CeilingVarJumpGrace = 0.05f;

		// Token: 0x0400199A RID: 6554
		private const int UpwardCornerCorrection = 4;

		// Token: 0x0400199B RID: 6555
		private const int DashingUpwardCornerCorrection = 5;

		// Token: 0x0400199C RID: 6556
		private const float WallSpeedRetentionTime = 0.06f;

		// Token: 0x0400199D RID: 6557
		private const int WallJumpCheckDist = 3;

		// Token: 0x0400199E RID: 6558
		private const int SuperWallJumpCheckDist = 5;

		// Token: 0x0400199F RID: 6559
		private const float WallJumpForceTime = 0.16f;

		// Token: 0x040019A0 RID: 6560
		private const float WallJumpHSpeed = 130f;

		// Token: 0x040019A1 RID: 6561
		public const float WallSlideStartMax = 20f;

		// Token: 0x040019A2 RID: 6562
		private const float WallSlideTime = 1.2f;

		// Token: 0x040019A3 RID: 6563
		private const float BounceVarJumpTime = 0.2f;

		// Token: 0x040019A4 RID: 6564
		private const float BounceSpeed = -140f;

		// Token: 0x040019A5 RID: 6565
		private const float SuperBounceVarJumpTime = 0.2f;

		// Token: 0x040019A6 RID: 6566
		private const float SuperBounceSpeed = -185f;

		// Token: 0x040019A7 RID: 6567
		private const float SuperJumpSpeed = -105f;

		// Token: 0x040019A8 RID: 6568
		private const float SuperJumpH = 260f;

		// Token: 0x040019A9 RID: 6569
		private const float SuperWallJumpSpeed = -160f;

		// Token: 0x040019AA RID: 6570
		private const float SuperWallJumpVarTime = 0.25f;

		// Token: 0x040019AB RID: 6571
		private const float SuperWallJumpForceTime = 0.2f;

		// Token: 0x040019AC RID: 6572
		private const float SuperWallJumpH = 170f;

		// Token: 0x040019AD RID: 6573
		private const float DashSpeed = 240f;

		// Token: 0x040019AE RID: 6574
		private const float EndDashSpeed = 160f;

		// Token: 0x040019AF RID: 6575
		private const float EndDashUpMult = 0.75f;

		// Token: 0x040019B0 RID: 6576
		private const float DashTime = 0.15f;

		// Token: 0x040019B1 RID: 6577
		private const float SuperDashTime = 0.3f;

		// Token: 0x040019B2 RID: 6578
		private const float DashCooldown = 0.2f;

		// Token: 0x040019B3 RID: 6579
		private const float DashRefillCooldown = 0.1f;

		// Token: 0x040019B4 RID: 6580
		private const int DashHJumpThruNudge = 6;

		// Token: 0x040019B5 RID: 6581
		private const int DashCornerCorrection = 4;

		// Token: 0x040019B6 RID: 6582
		private const int DashVFloorSnapDist = 3;

		// Token: 0x040019B7 RID: 6583
		private const float DashAttackTime = 0.3f;

		// Token: 0x040019B8 RID: 6584
		private const float BoostMoveSpeed = 80f;

		// Token: 0x040019B9 RID: 6585
		public const float BoostTime = 0.25f;

		// Token: 0x040019BA RID: 6586
		private const float DuckWindMult = 0f;

		// Token: 0x040019BB RID: 6587
		private const int WindWallDistance = 3;

		// Token: 0x040019BC RID: 6588
		private const float ReboundSpeedX = 120f;

		// Token: 0x040019BD RID: 6589
		private const float ReboundSpeedY = -120f;

		// Token: 0x040019BE RID: 6590
		private const float ReboundVarJumpTime = 0.15f;

		// Token: 0x040019BF RID: 6591
		private const float ReflectBoundSpeed = 220f;

		// Token: 0x040019C0 RID: 6592
		private const float DreamDashSpeed = 240f;

		// Token: 0x040019C1 RID: 6593
		private const int DreamDashEndWiggle = 5;

		// Token: 0x040019C2 RID: 6594
		private const float DreamDashMinTime = 0.1f;

		// Token: 0x040019C3 RID: 6595
		public const float ClimbMaxStamina = 110f;

		// Token: 0x040019C4 RID: 6596
		private const float ClimbUpCost = 45.454544f;

		// Token: 0x040019C5 RID: 6597
		private const float ClimbStillCost = 10f;

		// Token: 0x040019C6 RID: 6598
		private const float ClimbJumpCost = 27.5f;

		// Token: 0x040019C7 RID: 6599
		private const int ClimbCheckDist = 2;

		// Token: 0x040019C8 RID: 6600
		private const int ClimbUpCheckDist = 2;

		// Token: 0x040019C9 RID: 6601
		private const float ClimbNoMoveTime = 0.1f;

		// Token: 0x040019CA RID: 6602
		public const float ClimbTiredThreshold = 20f;

		// Token: 0x040019CB RID: 6603
		private const float ClimbUpSpeed = -45f;

		// Token: 0x040019CC RID: 6604
		private const float ClimbDownSpeed = 80f;

		// Token: 0x040019CD RID: 6605
		private const float ClimbSlipSpeed = 30f;

		// Token: 0x040019CE RID: 6606
		private const float ClimbAccel = 900f;

		// Token: 0x040019CF RID: 6607
		private const float ClimbGrabYMult = 0.2f;

		// Token: 0x040019D0 RID: 6608
		private const float ClimbHopY = -120f;

		// Token: 0x040019D1 RID: 6609
		private const float ClimbHopX = 100f;

		// Token: 0x040019D2 RID: 6610
		private const float ClimbHopForceTime = 0.2f;

		// Token: 0x040019D3 RID: 6611
		private const float ClimbJumpBoostTime = 0.2f;

		// Token: 0x040019D4 RID: 6612
		private const float ClimbHopNoWindTime = 0.3f;

		// Token: 0x040019D5 RID: 6613
		private const float LaunchSpeed = 280f;

		// Token: 0x040019D6 RID: 6614
		private const float LaunchCancelThreshold = 220f;

		// Token: 0x040019D7 RID: 6615
		private const float LiftYCap = -130f;

		// Token: 0x040019D8 RID: 6616
		private const float LiftXCap = 250f;

		// Token: 0x040019D9 RID: 6617
		private const float JumpThruAssistSpeed = -40f;

		// Token: 0x040019DA RID: 6618
		private const float FlyPowerFlashTime = 0.5f;

		// Token: 0x040019DB RID: 6619
		private const float ThrowRecoil = 80f;

		// Token: 0x040019DC RID: 6620
		private static readonly Vector2 CarryOffsetTarget = new Vector2(0f, -12f);

		// Token: 0x040019DD RID: 6621
		private const float ChaserStateMaxTime = 4f;

		// Token: 0x040019DE RID: 6622
		public const float WalkSpeed = 64f;

		// Token: 0x040019DF RID: 6623
		private const float LowFrictionMult = 0.35f;

		// Token: 0x040019E0 RID: 6624
		private const float LowFrictionAirMult = 0.5f;

		// Token: 0x040019E1 RID: 6625
		private const float LowFrictionStopTime = 0.15f;

		// Token: 0x040019E2 RID: 6626
		private const float HiccupTimeMin = 1.2f;

		// Token: 0x040019E3 RID: 6627
		private const float HiccupTimeMax = 1.8f;

		// Token: 0x040019E4 RID: 6628
		private const float HiccupDuckMult = 0.5f;

		// Token: 0x040019E5 RID: 6629
		private const float HiccupAirBoost = -60f;

		// Token: 0x040019E6 RID: 6630
		private const float HiccupAirVarTime = 0.15f;

		// Token: 0x040019E7 RID: 6631
		private const float GliderMaxFall = 40f;

		// Token: 0x040019E8 RID: 6632
		private const float GliderWindMaxFall = 0f;

		// Token: 0x040019E9 RID: 6633
		private const float GliderWindUpFall = -32f;

		// Token: 0x040019EA RID: 6634
		public const float GliderFastFall = 120f;

		// Token: 0x040019EB RID: 6635
		private const float GliderSlowFall = 24f;

		// Token: 0x040019EC RID: 6636
		private const float GliderGravMult = 0.5f;

		// Token: 0x040019ED RID: 6637
		private const float GliderMaxRun = 108.00001f;

		// Token: 0x040019EE RID: 6638
		private const float GliderRunMult = 0.5f;

		// Token: 0x040019EF RID: 6639
		private const float GliderUpMinPickupSpeed = -105f;

		// Token: 0x040019F0 RID: 6640
		private const float GliderDashMinPickupSpeed = -240f;

		// Token: 0x040019F1 RID: 6641
		private const float GliderWallJumpForceTime = 0.26f;

		// Token: 0x040019F2 RID: 6642
		private const float DashGliderBoostTime = 0.55f;

		// Token: 0x040019F3 RID: 6643
		public const int StNormal = 0;

		// Token: 0x040019F4 RID: 6644
		public const int StClimb = 1;

		// Token: 0x040019F5 RID: 6645
		public const int StDash = 2;

		// Token: 0x040019F6 RID: 6646
		public const int StSwim = 3;

		// Token: 0x040019F7 RID: 6647
		public const int StBoost = 4;

		// Token: 0x040019F8 RID: 6648
		public const int StRedDash = 5;

		// Token: 0x040019F9 RID: 6649
		public const int StHitSquash = 6;

		// Token: 0x040019FA RID: 6650
		public const int StLaunch = 7;

		// Token: 0x040019FB RID: 6651
		public const int StPickup = 8;

		// Token: 0x040019FC RID: 6652
		public const int StDreamDash = 9;

		// Token: 0x040019FD RID: 6653
		public const int StSummitLaunch = 10;

		// Token: 0x040019FE RID: 6654
		public const int StDummy = 11;

		// Token: 0x040019FF RID: 6655
		public const int StIntroWalk = 12;

		// Token: 0x04001A00 RID: 6656
		public const int StIntroJump = 13;

		// Token: 0x04001A01 RID: 6657
		public const int StIntroRespawn = 14;

		// Token: 0x04001A02 RID: 6658
		public const int StIntroWakeUp = 15;

		// Token: 0x04001A03 RID: 6659
		public const int StBirdDashTutorial = 16;

		// Token: 0x04001A04 RID: 6660
		public const int StFrozen = 17;

		// Token: 0x04001A05 RID: 6661
		public const int StReflectionFall = 18;

		// Token: 0x04001A06 RID: 6662
		public const int StStarFly = 19;

		// Token: 0x04001A07 RID: 6663
		public const int StTempleFall = 20;

		// Token: 0x04001A08 RID: 6664
		public const int StCassetteFly = 21;

		// Token: 0x04001A09 RID: 6665
		public const int StAttract = 22;

		// Token: 0x04001A0A RID: 6666
		public const int StIntroMoonJump = 23;

		// Token: 0x04001A0B RID: 6667
		public const int StFlingBird = 24;

		// Token: 0x04001A0C RID: 6668
		public const int StIntroThinkForABit = 25;

		// Token: 0x04001A0D RID: 6669
		public const string TalkSfx = "player_talk";

		// Token: 0x04001A0E RID: 6670
		public Vector2 Speed;

		// Token: 0x04001A0F RID: 6671
		public Facings Facing;

		// Token: 0x04001A10 RID: 6672
		public PlayerSprite Sprite;

		// Token: 0x04001A11 RID: 6673
		public PlayerHair Hair;

		// Token: 0x04001A12 RID: 6674
		public StateMachine StateMachine;

		// Token: 0x04001A13 RID: 6675
		public Vector2 CameraAnchor;

		// Token: 0x04001A14 RID: 6676
		public bool CameraAnchorIgnoreX;

		// Token: 0x04001A15 RID: 6677
		public bool CameraAnchorIgnoreY;

		// Token: 0x04001A16 RID: 6678
		public Vector2 CameraAnchorLerp;

		// Token: 0x04001A17 RID: 6679
		public bool ForceCameraUpdate;

		// Token: 0x04001A18 RID: 6680
		public Leader Leader;

		// Token: 0x04001A19 RID: 6681
		public VertexLight Light;

		// Token: 0x04001A1A RID: 6682
		public int Dashes;

		// Token: 0x04001A1B RID: 6683
		public float Stamina = 110f;

		// Token: 0x04001A1C RID: 6684
		public bool StrawberriesBlocked;

		// Token: 0x04001A1D RID: 6685
		public Vector2 PreviousPosition;

		// Token: 0x04001A1E RID: 6686
		public bool DummyAutoAnimate = true;

		// Token: 0x04001A1F RID: 6687
		public Vector2 ForceStrongWindHair;

		// Token: 0x04001A20 RID: 6688
		public Vector2? OverrideDashDirection;

		// Token: 0x04001A21 RID: 6689
		public bool FlipInReflection;

		// Token: 0x04001A22 RID: 6690
		public bool JustRespawned;

		// Token: 0x04001A24 RID: 6692
		public bool EnforceLevelBounds = true;

		// Token: 0x04001A25 RID: 6693
		private Level level;

		// Token: 0x04001A26 RID: 6694
		private Collision onCollideH;

		// Token: 0x04001A27 RID: 6695
		private Collision onCollideV;

		// Token: 0x04001A28 RID: 6696
		private bool onGround;

		// Token: 0x04001A29 RID: 6697
		private bool wasOnGround;

		// Token: 0x04001A2A RID: 6698
		private int moveX;

		// Token: 0x04001A2B RID: 6699
		private bool flash;

		// Token: 0x04001A2C RID: 6700
		private bool wasDucking;

		// Token: 0x04001A2D RID: 6701
		private int climbTriggerDir;

		// Token: 0x04001A2E RID: 6702
		private bool holdCannotDuck;

		// Token: 0x04001A2F RID: 6703
		private bool windMovedUp;

		// Token: 0x04001A30 RID: 6704
		private float idleTimer;

		// Token: 0x04001A31 RID: 6705
		private static Chooser<string> idleColdOptions = new Chooser<string>().Add("idleA", 5f).Add("idleB", 3f).Add("idleC", 1f);

		// Token: 0x04001A32 RID: 6706
		private static Chooser<string> idleNoBackpackOptions = new Chooser<string>().Add("idleA", 1f).Add("idleB", 3f).Add("idleC", 3f);

		// Token: 0x04001A33 RID: 6707
		private static Chooser<string> idleWarmOptions = new Chooser<string>().Add("idleA", 5f).Add("idleB", 3f);

		// Token: 0x04001A34 RID: 6708
		public int StrawberryCollectIndex;

		// Token: 0x04001A35 RID: 6709
		public float StrawberryCollectResetTimer;

		// Token: 0x04001A36 RID: 6710
		private Hitbox hurtbox;

		// Token: 0x04001A37 RID: 6711
		private float jumpGraceTimer;

		// Token: 0x04001A38 RID: 6712
		public bool AutoJump;

		// Token: 0x04001A39 RID: 6713
		public float AutoJumpTimer;

		// Token: 0x04001A3A RID: 6714
		private float varJumpSpeed;

		// Token: 0x04001A3B RID: 6715
		private float varJumpTimer;

		// Token: 0x04001A3C RID: 6716
		private int forceMoveX;

		// Token: 0x04001A3D RID: 6717
		private float forceMoveXTimer;

		// Token: 0x04001A3E RID: 6718
		private int hopWaitX;

		// Token: 0x04001A3F RID: 6719
		private float hopWaitXSpeed;

		// Token: 0x04001A40 RID: 6720
		private Vector2 lastAim;

		// Token: 0x04001A41 RID: 6721
		private float dashCooldownTimer;

		// Token: 0x04001A42 RID: 6722
		private float dashRefillCooldownTimer;

		// Token: 0x04001A43 RID: 6723
		public Vector2 DashDir;

		// Token: 0x04001A44 RID: 6724
		private float wallSlideTimer = 1.2f;

		// Token: 0x04001A45 RID: 6725
		private int wallSlideDir;

		// Token: 0x04001A46 RID: 6726
		private float climbNoMoveTimer;

		// Token: 0x04001A47 RID: 6727
		private Vector2 carryOffset;

		// Token: 0x04001A48 RID: 6728
		private Vector2 deadOffset;

		// Token: 0x04001A49 RID: 6729
		private float introEase;

		// Token: 0x04001A4A RID: 6730
		private float wallSpeedRetentionTimer;

		// Token: 0x04001A4B RID: 6731
		private float wallSpeedRetained;

		// Token: 0x04001A4C RID: 6732
		private int wallBoostDir;

		// Token: 0x04001A4D RID: 6733
		private float wallBoostTimer;

		// Token: 0x04001A4E RID: 6734
		private float maxFall;

		// Token: 0x04001A4F RID: 6735
		private float dashAttackTimer;

		// Token: 0x04001A50 RID: 6736
		private float gliderBoostTimer;

		// Token: 0x04001A51 RID: 6737
		public List<Player.ChaserState> ChaserStates;

		// Token: 0x04001A52 RID: 6738
		private bool wasTired;

		// Token: 0x04001A53 RID: 6739
		private HashSet<Trigger> triggersInside;

		// Token: 0x04001A54 RID: 6740
		private float highestAirY;

		// Token: 0x04001A55 RID: 6741
		private bool dashStartedOnGround;

		// Token: 0x04001A56 RID: 6742
		private bool fastJump;

		// Token: 0x04001A57 RID: 6743
		private int lastClimbMove;

		// Token: 0x04001A58 RID: 6744
		private float noWindTimer;

		// Token: 0x04001A59 RID: 6745
		private float dreamDashCanEndTimer;

		// Token: 0x04001A5A RID: 6746
		private Solid climbHopSolid;

		// Token: 0x04001A5B RID: 6747
		private Vector2 climbHopSolidPosition;

		// Token: 0x04001A5C RID: 6748
		private SoundSource wallSlideSfx;

		// Token: 0x04001A5D RID: 6749
		private SoundSource swimSurfaceLoopSfx;

		// Token: 0x04001A5E RID: 6750
		private float playFootstepOnLand;

		// Token: 0x04001A5F RID: 6751
		private float minHoldTimer;

		// Token: 0x04001A60 RID: 6752
		public Booster CurrentBooster;

		// Token: 0x04001A61 RID: 6753
		public Booster LastBooster;

		// Token: 0x04001A62 RID: 6754
		private bool calledDashEvents;

		// Token: 0x04001A63 RID: 6755
		private int lastDashes;

		// Token: 0x04001A64 RID: 6756
		private Sprite sweatSprite;

		// Token: 0x04001A65 RID: 6757
		private int startHairCount;

		// Token: 0x04001A66 RID: 6758
		private bool launched;

		// Token: 0x04001A67 RID: 6759
		private float launchedTimer;

		// Token: 0x04001A68 RID: 6760
		private float dashTrailTimer;

		// Token: 0x04001A69 RID: 6761
		private int dashTrailCounter;

		// Token: 0x04001A6A RID: 6762
		private bool canCurveDash;

		// Token: 0x04001A6B RID: 6763
		private float lowFrictionStopTimer;

		// Token: 0x04001A6C RID: 6764
		private float hiccupTimer;

		// Token: 0x04001A6D RID: 6765
		private List<Player.ChaserStateSound> activeSounds = new List<Player.ChaserStateSound>();

		// Token: 0x04001A6E RID: 6766
		private EventInstance idleSfx;

		// Token: 0x04001A6F RID: 6767
		public bool MuffleLanding;

		// Token: 0x04001A70 RID: 6768
		private Vector2 gliderBoostDir;

		// Token: 0x04001A71 RID: 6769
		private float explodeLaunchBoostTimer;

		// Token: 0x04001A72 RID: 6770
		private float explodeLaunchBoostSpeed;

		// Token: 0x04001A73 RID: 6771
		private bool demoDashed;

		// Token: 0x04001A74 RID: 6772
		private readonly Hitbox normalHitbox = new Hitbox(8f, 11f, -4f, -11f);

		// Token: 0x04001A75 RID: 6773
		private readonly Hitbox duckHitbox = new Hitbox(8f, 6f, -4f, -6f);

		// Token: 0x04001A76 RID: 6774
		private readonly Hitbox normalHurtbox = new Hitbox(8f, 9f, -4f, -11f);

		// Token: 0x04001A77 RID: 6775
		private readonly Hitbox duckHurtbox = new Hitbox(8f, 4f, -4f, -6f);

		// Token: 0x04001A78 RID: 6776
		private readonly Hitbox starFlyHitbox = new Hitbox(8f, 8f, -4f, -10f);

		// Token: 0x04001A79 RID: 6777
		private readonly Hitbox starFlyHurtbox = new Hitbox(6f, 6f, -3f, -9f);

		// Token: 0x04001A7A RID: 6778
		private Vector2 normalLightOffset = new Vector2(0f, -8f);

		// Token: 0x04001A7B RID: 6779
		private Vector2 duckingLightOffset = new Vector2(0f, -3f);

		// Token: 0x04001A7C RID: 6780
		private List<Entity> temp = new List<Entity>();

		// Token: 0x04001A7D RID: 6781
		public static readonly Color NormalHairColor = Calc.HexToColor("AC3232");

		// Token: 0x04001A7E RID: 6782
		public static readonly Color FlyPowerHairColor = Calc.HexToColor("F2EB6D");

		// Token: 0x04001A7F RID: 6783
		public static readonly Color UsedHairColor = Calc.HexToColor("44B7FF");

		// Token: 0x04001A80 RID: 6784
		public static readonly Color FlashHairColor = Color.White;

		// Token: 0x04001A81 RID: 6785
		public static readonly Color TwoDashesHairColor = Calc.HexToColor("ff6def");

		// Token: 0x04001A82 RID: 6786
		public static readonly Color NormalBadelineHairColor = BadelineOldsite.HairColor;

		// Token: 0x04001A83 RID: 6787
		public static readonly Color UsedBadelineHairColor = Player.UsedHairColor;

		// Token: 0x04001A84 RID: 6788
		public static readonly Color TwoDashesBadelineHairColor = Player.TwoDashesHairColor;

		// Token: 0x04001A85 RID: 6789
		private float hairFlashTimer;

		// Token: 0x04001A86 RID: 6790
		private bool startHairCalled;

		// Token: 0x04001A87 RID: 6791
		public Color? OverrideHairColor;

		// Token: 0x04001A88 RID: 6792
		private Vector2 windDirection;

		// Token: 0x04001A89 RID: 6793
		private float windTimeout;

		// Token: 0x04001A8A RID: 6794
		private float windHairTimer;

		// Token: 0x04001A8B RID: 6795
		public Player.IntroTypes IntroType;

		// Token: 0x04001A8C RID: 6796
		private MirrorReflection reflection;

		// Token: 0x04001A8D RID: 6797
		public PlayerSpriteMode DefaultSpriteMode;

		// Token: 0x04001A8E RID: 6798
		private PlayerSpriteMode? nextSpriteMode;

		// Token: 0x04001A90 RID: 6800
		private const float LaunchedBoostCheckSpeedSq = 10000f;

		// Token: 0x04001A91 RID: 6801
		private const float LaunchedJumpCheckSpeedSq = 48400f;

		// Token: 0x04001A92 RID: 6802
		private const float LaunchedMinSpeedSq = 19600f;

		// Token: 0x04001A93 RID: 6803
		private const float LaunchedDoubleSpeedSq = 22500f;

		// Token: 0x04001A94 RID: 6804
		private const float SideBounceSpeed = 240f;

		// Token: 0x04001A95 RID: 6805
		private const float SideBounceThreshold = 240f;

		// Token: 0x04001A96 RID: 6806
		private const float SideBounceForceMoveXTime = 0.3f;

		// Token: 0x04001A98 RID: 6808
		private const float SpacePhysicsMult = 0.6f;

		// Token: 0x04001A99 RID: 6809
		private EventInstance conveyorLoopSfx;

		// Token: 0x04001A9A RID: 6810
		private const float WallBoosterSpeed = -160f;

		// Token: 0x04001A9B RID: 6811
		private const float WallBoosterLiftSpeed = -80f;

		// Token: 0x04001A9C RID: 6812
		private const float WallBoosterAccel = 600f;

		// Token: 0x04001A9D RID: 6813
		private const float WallBoostingHopHSpeed = 100f;

		// Token: 0x04001A9E RID: 6814
		private const float WallBoosterOverTopSpeed = -180f;

		// Token: 0x04001A9F RID: 6815
		private const float IceBoosterSpeed = 40f;

		// Token: 0x04001AA0 RID: 6816
		private const float IceBoosterAccel = 300f;

		// Token: 0x04001AA1 RID: 6817
		private bool wallBoosting;

		// Token: 0x04001AA2 RID: 6818
		private Vector2 beforeDashSpeed;

		// Token: 0x04001AA3 RID: 6819
		private bool wasDashB;

		// Token: 0x04001AA5 RID: 6821
		private const float SwimYSpeedMult = 0.5f;

		// Token: 0x04001AA6 RID: 6822
		private const float SwimMaxRise = -60f;

		// Token: 0x04001AA7 RID: 6823
		private const float SwimVDeccel = 600f;

		// Token: 0x04001AA8 RID: 6824
		private const float SwimMax = 80f;

		// Token: 0x04001AA9 RID: 6825
		private const float SwimUnderwaterMax = 60f;

		// Token: 0x04001AAA RID: 6826
		private const float SwimAccel = 600f;

		// Token: 0x04001AAB RID: 6827
		private const float SwimReduce = 400f;

		// Token: 0x04001AAC RID: 6828
		private const float SwimDashSpeedMult = 0.75f;

		// Token: 0x04001AAD RID: 6829
		private Vector2 boostTarget;

		// Token: 0x04001AAE RID: 6830
		private bool boostRed;

		// Token: 0x04001AAF RID: 6831
		private const float HitSquashNoMoveTime = 0.1f;

		// Token: 0x04001AB0 RID: 6832
		private const float HitSquashFriction = 800f;

		// Token: 0x04001AB1 RID: 6833
		private float hitSquashNoMoveTimer;

		// Token: 0x04001AB2 RID: 6834
		private float? launchApproachX;

		// Token: 0x04001AB3 RID: 6835
		private float summitLaunchTargetX;

		// Token: 0x04001AB4 RID: 6836
		private float summitLaunchParticleTimer;

		// Token: 0x04001AB5 RID: 6837
		private DreamBlock dreamBlock;

		// Token: 0x04001AB6 RID: 6838
		private SoundSource dreamSfxLoop;

		// Token: 0x04001AB7 RID: 6839
		private bool dreamJump;

		// Token: 0x04001AB8 RID: 6840
		private const float StarFlyTransformDeccel = 1000f;

		// Token: 0x04001AB9 RID: 6841
		private const float StarFlyTime = 2f;

		// Token: 0x04001ABA RID: 6842
		private const float StarFlyStartSpeed = 250f;

		// Token: 0x04001ABB RID: 6843
		private const float StarFlyTargetSpeed = 140f;

		// Token: 0x04001ABC RID: 6844
		private const float StarFlyMaxSpeed = 190f;

		// Token: 0x04001ABD RID: 6845
		private const float StarFlyMaxLerpTime = 1f;

		// Token: 0x04001ABE RID: 6846
		private const float StarFlySlowSpeed = 91f;

		// Token: 0x04001ABF RID: 6847
		private const float StarFlyAccel = 1000f;

		// Token: 0x04001AC0 RID: 6848
		private const float StarFlyRotateSpeed = 5.5850534f;

		// Token: 0x04001AC1 RID: 6849
		private const float StarFlyEndX = 160f;

		// Token: 0x04001AC2 RID: 6850
		private const float StarFlyEndXVarJumpTime = 0.1f;

		// Token: 0x04001AC3 RID: 6851
		private const float StarFlyEndFlashDuration = 0.5f;

		// Token: 0x04001AC4 RID: 6852
		private const float StarFlyEndNoBounceTime = 0.2f;

		// Token: 0x04001AC5 RID: 6853
		private const float StarFlyWallBounce = -0.5f;

		// Token: 0x04001AC6 RID: 6854
		private const float StarFlyMaxExitY = 0f;

		// Token: 0x04001AC7 RID: 6855
		private const float StarFlyMaxExitX = 140f;

		// Token: 0x04001AC8 RID: 6856
		private const float StarFlyExitUp = -100f;

		// Token: 0x04001AC9 RID: 6857
		private Color starFlyColor = Calc.HexToColor("ffd65c");

		// Token: 0x04001ACA RID: 6858
		private BloomPoint starFlyBloom;

		// Token: 0x04001ACB RID: 6859
		private float starFlyTimer;

		// Token: 0x04001ACC RID: 6860
		private bool starFlyTransforming;

		// Token: 0x04001ACD RID: 6861
		private float starFlySpeedLerp;

		// Token: 0x04001ACE RID: 6862
		private Vector2 starFlyLastDir;

		// Token: 0x04001ACF RID: 6863
		private SoundSource starFlyLoopSfx;

		// Token: 0x04001AD0 RID: 6864
		private SoundSource starFlyWarningSfx;

		// Token: 0x04001AD1 RID: 6865
		private FlingBird flingBird;

		// Token: 0x04001AD2 RID: 6866
		private SimpleCurve cassetteFlyCurve;

		// Token: 0x04001AD3 RID: 6867
		private float cassetteFlyLerp;

		// Token: 0x04001AD4 RID: 6868
		private Vector2 attractTo;

		// Token: 0x04001AD5 RID: 6869
		public bool DummyMoving;

		// Token: 0x04001AD6 RID: 6870
		public bool DummyGravity = true;

		// Token: 0x04001AD7 RID: 6871
		public bool DummyFriction = true;

		// Token: 0x04001AD8 RID: 6872
		public bool DummyMaxspeed = true;

		// Token: 0x04001AD9 RID: 6873
		private Facings IntroWalkDirection;

		// Token: 0x04001ADA RID: 6874
		private Tween respawnTween;

		// Token: 0x02000739 RID: 1849
		public enum IntroTypes
		{
			// Token: 0x04002E61 RID: 11873
			Transition,
			// Token: 0x04002E62 RID: 11874
			Respawn,
			// Token: 0x04002E63 RID: 11875
			WalkInRight,
			// Token: 0x04002E64 RID: 11876
			WalkInLeft,
			// Token: 0x04002E65 RID: 11877
			Jump,
			// Token: 0x04002E66 RID: 11878
			WakeUp,
			// Token: 0x04002E67 RID: 11879
			Fall,
			// Token: 0x04002E68 RID: 11880
			TempleMirrorVoid,
			// Token: 0x04002E69 RID: 11881
			None,
			// Token: 0x04002E6A RID: 11882
			ThinkForABit
		}

		// Token: 0x0200073A RID: 1850
		public struct ChaserStateSound
		{
			// Token: 0x04002E6B RID: 11883
			public string Event;

			// Token: 0x04002E6C RID: 11884
			public string Parameter;

			// Token: 0x04002E6D RID: 11885
			public float ParameterValue;

			// Token: 0x04002E6E RID: 11886
			public Player.ChaserStateSound.Actions Action;

			// Token: 0x02000799 RID: 1945
			public enum Actions
			{
				// Token: 0x04002FC8 RID: 12232
				Oneshot,
				// Token: 0x04002FC9 RID: 12233
				Loop,
				// Token: 0x04002FCA RID: 12234
				Stop
			}
		}

		// Token: 0x0200073B RID: 1851
		public struct ChaserState
		{
			// Token: 0x06002EC3 RID: 11971 RVA: 0x00125EDC File Offset: 0x001240DC
			public ChaserState(Player player)
			{
				this.Position = player.Position;
				this.TimeStamp = player.Scene.TimeActive;
				this.Animation = player.Sprite.CurrentAnimationID;
				this.Facing = player.Facing;
				this.OnGround = player.onGround;
				this.HairColor = player.Hair.Color;
				this.Depth = player.Depth;
				this.Scale = new Vector2(Math.Abs(player.Sprite.Scale.X) * (float)player.Facing, player.Sprite.Scale.Y);
				this.DashDirection = player.DashDir;
				List<Player.ChaserStateSound> activeSounds = player.activeSounds;
				this.Sounds = Math.Min(5, activeSounds.Count);
				this.sound0 = ((this.Sounds > 0) ? activeSounds[0] : default(Player.ChaserStateSound));
				this.sound1 = ((this.Sounds > 1) ? activeSounds[1] : default(Player.ChaserStateSound));
				this.sound2 = ((this.Sounds > 2) ? activeSounds[2] : default(Player.ChaserStateSound));
				this.sound3 = ((this.Sounds > 3) ? activeSounds[3] : default(Player.ChaserStateSound));
				this.sound4 = ((this.Sounds > 4) ? activeSounds[4] : default(Player.ChaserStateSound));
			}

			// Token: 0x170006CA RID: 1738
			public Player.ChaserStateSound this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return this.sound0;
					case 1:
						return this.sound1;
					case 2:
						return this.sound2;
					case 3:
						return this.sound3;
					case 4:
						return this.sound4;
					default:
						return default(Player.ChaserStateSound);
					}
				}
			}

			// Token: 0x04002E6F RID: 11887
			public Vector2 Position;

			// Token: 0x04002E70 RID: 11888
			public float TimeStamp;

			// Token: 0x04002E71 RID: 11889
			public string Animation;

			// Token: 0x04002E72 RID: 11890
			public Facings Facing;

			// Token: 0x04002E73 RID: 11891
			public bool OnGround;

			// Token: 0x04002E74 RID: 11892
			public Color HairColor;

			// Token: 0x04002E75 RID: 11893
			public int Depth;

			// Token: 0x04002E76 RID: 11894
			public Vector2 Scale;

			// Token: 0x04002E77 RID: 11895
			public Vector2 DashDirection;

			// Token: 0x04002E78 RID: 11896
			private Player.ChaserStateSound sound0;

			// Token: 0x04002E79 RID: 11897
			private Player.ChaserStateSound sound1;

			// Token: 0x04002E7A RID: 11898
			private Player.ChaserStateSound sound2;

			// Token: 0x04002E7B RID: 11899
			private Player.ChaserStateSound sound3;

			// Token: 0x04002E7C RID: 11900
			private Player.ChaserStateSound sound4;

			// Token: 0x04002E7D RID: 11901
			public int Sounds;
		}
	}
}
