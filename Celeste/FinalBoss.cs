using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200019D RID: 413
	[Tracked(false)]
	public class FinalBoss : Entity
	{
		// Token: 0x06000E5A RID: 3674 RVA: 0x0003407C File Offset: 0x0003227C
		public FinalBoss(Vector2 position, Vector2[] nodes, int patternIndex, float cameraYPastMax, bool dialog, bool startHit, bool cameraLockY) : base(position)
		{
			this.patternIndex = patternIndex;
			this.CameraYPastMax = cameraYPastMax;
			this.dialog = dialog;
			this.startHit = startHit;
			base.Add(this.light = new VertexLight(Color.White, 1f, 32, 64));
			base.Collider = (this.circle = new Circle(14f, 0f, -6f));
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			this.nodes = new Vector2[nodes.Length + 1];
			this.nodes[0] = this.Position;
			for (int i = 0; i < nodes.Length; i++)
			{
				this.nodes[i + 1] = nodes[i];
			}
			this.attackCoroutine = new Coroutine(false);
			base.Add(this.attackCoroutine);
			this.triggerBlocksCoroutine = new Coroutine(false);
			base.Add(this.triggerBlocksCoroutine);
			base.Add(new CameraLocker(cameraLockY ? Level.CameraLockModes.FinalBoss : Level.CameraLockModes.FinalBossNoY, 140f, cameraYPastMax));
			base.Add(this.floatSine = new SineWave(0.6f, 0f));
			base.Add(this.scaleWiggler = Wiggler.Create(0.6f, 3f, null, false, false));
			base.Add(this.chargeSfx = new SoundSource());
			base.Add(this.laserSfx = new SoundSource());
		}

		// Token: 0x06000E5B RID: 3675 RVA: 0x0003420C File Offset: 0x0003240C
		public FinalBoss(EntityData e, Vector2 offset) : this(e.Position + offset, e.NodesOffset(offset), e.Int("patternIndex", 0), e.Float("cameraPastY", 120f), e.Bool("dialog", false), e.Bool("startHit", false), e.Bool("cameraLockY", true))
		{
		}

		// Token: 0x06000E5C RID: 3676 RVA: 0x00034274 File Offset: 0x00032474
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
			if (this.patternIndex == 0)
			{
				this.NormalSprite = new PlayerSprite(PlayerSpriteMode.Badeline);
				this.NormalSprite.Scale.X = -1f;
				this.NormalSprite.Play("laugh", false, false);
				this.normalHair = new PlayerHair(this.NormalSprite);
				this.normalHair.Color = BadelineOldsite.HairColor;
				this.normalHair.Border = Color.Black;
				this.normalHair.Facing = Facings.Left;
				base.Add(this.normalHair);
				base.Add(this.NormalSprite);
			}
			else
			{
				this.CreateBossSprite();
			}
			this.bossBg = this.level.Background.Get<FinalBossStarfield>();
			if (this.patternIndex == 0 && !this.level.Session.GetFlag("boss_intro") && this.level.Session.Level.Equals("boss-00"))
			{
				this.level.Session.Audio.Music.Event = "event:/music/lvl2/phone_loop";
				this.level.Session.Audio.Apply(false);
				if (this.bossBg != null)
				{
					this.bossBg.Alpha = 0f;
				}
				this.Sitting = true;
				this.Position.Y = this.Position.Y + 16f;
				this.NormalSprite.Play("pretendDead", false, false);
				this.NormalSprite.Scale.X = 1f;
			}
			else if (this.patternIndex == 0 && !this.level.Session.GetFlag("boss_mid") && this.level.Session.Level.Equals("boss-14"))
			{
				this.level.Add(new CS06_BossMid());
			}
			else if (this.startHit)
			{
				Alarm.Set(this, 0.5f, delegate
				{
					this.OnPlayer(null);
				}, Alarm.AlarmMode.Oneshot);
			}
			this.light.Position = ((this.Sprite != null) ? this.Sprite : this.NormalSprite).Position + new Vector2(0f, -10f);
		}

		// Token: 0x06000E5D RID: 3677 RVA: 0x000344C4 File Offset: 0x000326C4
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			this.fallingBlocks = base.Scene.Tracker.GetEntitiesCopy<FallingBlock>();
			this.fallingBlocks.Sort((Entity a, Entity b) => (int)(a.X - b.X));
			this.movingBlocks = base.Scene.Tracker.GetEntitiesCopy<FinalBossMovingBlock>();
			this.movingBlocks.Sort((Entity a, Entity b) => (int)(a.X - b.X));
		}

		// Token: 0x06000E5E RID: 3678 RVA: 0x00034558 File Offset: 0x00032758
		private void CreateBossSprite()
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("badeline_boss"));
			this.Sprite.OnFrameChange = delegate(string anim)
			{
				if (anim == "idle" && this.Sprite.CurrentAnimationFrame == 18)
				{
					Audio.Play("event:/char/badeline/boss_idle_air", this.Position);
				}
			};
			this.facing = -1;
			if (this.NormalSprite != null)
			{
				this.Sprite.Position = this.NormalSprite.Position;
				base.Remove(this.NormalSprite);
			}
			if (this.normalHair != null)
			{
				base.Remove(this.normalHair);
			}
			this.NormalSprite = null;
			this.normalHair = null;
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000E5F RID: 3679 RVA: 0x000345ED File Offset: 0x000327ED
		public Vector2 BeamOrigin
		{
			get
			{
				return base.Center + this.Sprite.Position + new Vector2(0f, -14f);
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000E60 RID: 3680 RVA: 0x00034619 File Offset: 0x00032819
		public Vector2 ShotOrigin
		{
			get
			{
				return base.Center + this.Sprite.Position + new Vector2(6f * this.Sprite.Scale.X, 2f);
			}
		}

		// Token: 0x06000E61 RID: 3681 RVA: 0x00034658 File Offset: 0x00032858
		public override void Update()
		{
			base.Update();
			Sprite sprite = (this.Sprite != null) ? this.Sprite : this.NormalSprite;
			if (!this.Sitting)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (!this.Moving && entity != null)
				{
					if (this.facing == -1 && entity.X > base.X + 20f)
					{
						this.facing = 1;
						this.scaleWiggler.Start();
					}
					else if (this.facing == 1 && entity.X < base.X - 20f)
					{
						this.facing = -1;
						this.scaleWiggler.Start();
					}
				}
				if (!this.playerHasMoved && entity != null && entity.Speed != Vector2.Zero)
				{
					this.playerHasMoved = true;
					if (this.patternIndex != 0)
					{
						this.StartAttacking();
					}
					this.TriggerMovingBlocks(0);
				}
				if (!this.Moving)
				{
					sprite.Position = this.avoidPos + new Vector2(this.floatSine.Value * 3f, this.floatSine.ValueOverTwo * 4f);
				}
				else
				{
					sprite.Position = Calc.Approach(sprite.Position, Vector2.Zero, 12f * Engine.DeltaTime);
				}
				float radius = this.circle.Radius;
				this.circle.Radius = 6f;
				DashBlock dashBlock = base.CollideFirst<DashBlock>();
				if (dashBlock != null)
				{
					dashBlock.Break(base.Center, -Vector2.UnitY, true, true);
				}
				this.circle.Radius = radius;
				if (!this.level.IsInBounds(this.Position, 24f))
				{
					this.Active = (this.Visible = (this.Collidable = false));
					return;
				}
				Vector2 target;
				if (!this.Moving && entity != null)
				{
					float num = (base.Center - entity.Center).Length();
					num = Calc.ClampedMap(num, 32f, 88f, 12f, 0f);
					if (num <= 0f)
					{
						target = Vector2.Zero;
					}
					else
					{
						target = (base.Center - entity.Center).SafeNormalize(num);
					}
				}
				else
				{
					target = Vector2.Zero;
				}
				this.avoidPos = Calc.Approach(this.avoidPos, target, 40f * Engine.DeltaTime);
			}
			this.light.Position = sprite.Position + new Vector2(0f, -10f);
		}

		// Token: 0x06000E62 RID: 3682 RVA: 0x000348E4 File Offset: 0x00032AE4
		public override void Render()
		{
			if (this.Sprite != null)
			{
				this.Sprite.Scale.X = (float)this.facing;
				this.Sprite.Scale.Y = 1f;
				this.Sprite.Scale *= 1f + this.scaleWiggler.Value * 0.2f;
			}
			if (this.NormalSprite != null)
			{
				Vector2 position = this.NormalSprite.Position;
				this.NormalSprite.Position = this.NormalSprite.Position.Floor();
				base.Render();
				this.NormalSprite.Position = position;
				return;
			}
			base.Render();
		}

		// Token: 0x06000E63 RID: 3683 RVA: 0x0003499C File Offset: 0x00032B9C
		public void OnPlayer(Player player)
		{
			if (this.Sprite == null)
			{
				this.CreateBossSprite();
			}
			this.Sprite.Play("getHit", false, false);
			Audio.Play("event:/char/badeline/boss_hug", this.Position);
			this.chargeSfx.Stop(true);
			if (this.laserSfx.EventName == "event:/char/badeline/boss_laser_charge" && this.laserSfx.Playing)
			{
				this.laserSfx.Stop(true);
			}
			this.Collidable = false;
			this.avoidPos = Vector2.Zero;
			this.nodeIndex++;
			if (this.dialog)
			{
				if (this.nodeIndex == 1)
				{
					base.Scene.Add(new MiniTextbox("ch6_boss_tired_a"));
				}
				else if (this.nodeIndex == 2)
				{
					base.Scene.Add(new MiniTextbox("ch6_boss_tired_b"));
				}
				else if (this.nodeIndex == 3)
				{
					base.Scene.Add(new MiniTextbox("ch6_boss_tired_c"));
				}
			}
			foreach (Entity entity in this.level.Tracker.GetEntities<FinalBossShot>())
			{
				((FinalBossShot)entity).Destroy();
			}
			foreach (Entity entity2 in this.level.Tracker.GetEntities<FinalBossBeam>())
			{
				((FinalBossBeam)entity2).Destroy();
			}
			this.TriggerFallingBlocks(base.X);
			this.TriggerMovingBlocks(this.nodeIndex);
			this.attackCoroutine.Active = false;
			this.Moving = true;
			bool flag = this.nodeIndex == this.nodes.Length - 1;
			if (this.level.Session.Area.Mode == AreaMode.Normal)
			{
				if (flag && this.level.Session.Level.Equals("boss-19"))
				{
					Alarm.Set(this, 0.25f, delegate
					{
						Audio.Play("event:/game/06_reflection/boss_spikes_burst");
						foreach (Entity entity3 in base.Scene.Tracker.GetEntities<CrystalStaticSpinner>())
						{
							((CrystalStaticSpinner)entity3).Destroy(true);
						}
					}, Alarm.AlarmMode.Oneshot);
					Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "postboss", 1f);
					Audio.SetMusic(null, true, true);
				}
				else if (this.startHit && this.level.Session.Audio.Music.Event != "event:/music/lvl6/badeline_glitch")
				{
					this.level.Session.Audio.Music.Event = "event:/music/lvl6/badeline_glitch";
					this.level.Session.Audio.Apply(false);
				}
				else if (this.level.Session.Audio.Music.Event != "event:/music/lvl6/badeline_fight" && this.level.Session.Audio.Music.Event != "event:/music/lvl6/badeline_glitch")
				{
					this.level.Session.Audio.Music.Event = "event:/music/lvl6/badeline_fight";
					this.level.Session.Audio.Apply(false);
				}
			}
			base.Add(new Coroutine(this.MoveSequence(player, flag), true));
		}

		// Token: 0x06000E64 RID: 3684 RVA: 0x00034CF0 File Offset: 0x00032EF0
		private IEnumerator MoveSequence(Player player, bool lastHit)
		{
			FinalBoss.<>c__DisplayClass42_0 CS$<>8__locals1 = new FinalBoss.<>c__DisplayClass42_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.lastHit = lastHit;
			if (CS$<>8__locals1.lastHit)
			{
				Audio.SetMusicParam("boss_pitch", 1f);
				Tween tween = Tween.Create(Tween.TweenMode.Oneshot, null, 0.3f, true);
				tween.OnUpdate = delegate(Tween t)
				{
					Glitch.Value = 0.6f * t.Eased;
				};
				base.Add(tween);
			}
			else
			{
				Tween tween2 = Tween.Create(Tween.TweenMode.Oneshot, null, 0.3f, true);
				tween2.OnUpdate = delegate(Tween t)
				{
					Glitch.Value = 0.5f * (1f - t.Eased);
				};
				base.Add(tween2);
			}
			if (player != null && !player.Dead)
			{
				player.StartAttract(base.Center + Vector2.UnitY * 4f);
			}
			float timer = 0.15f;
			while (player != null && !player.Dead && !player.AtAttractTarget)
			{
				yield return null;
				timer -= Engine.DeltaTime;
			}
			if (timer > 0f)
			{
				yield return timer;
			}
			foreach (Entity entity in base.Scene.Tracker.GetEntities<ReflectionTentacles>())
			{
				((ReflectionTentacles)entity).Retreat();
			}
			if (player != null)
			{
				Celeste.Freeze(0.1f);
				if (CS$<>8__locals1.lastHit)
				{
					Engine.TimeRate = 0.5f;
				}
				else
				{
					Engine.TimeRate = 0.75f;
				}
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			}
			this.PushPlayer(player);
			this.level.Shake(0.3f);
			yield return 0.05f;
			for (float num = 0f; num < 6.2831855f; num += 0.17453292f)
			{
				Vector2 position = base.Center + this.Sprite.Position + Calc.AngleToVector(num + Calc.Random.Range(-0.034906585f, 0.034906585f), (float)Calc.Random.Range(16, 20));
				this.level.Particles.Emit(FinalBoss.P_Burst, position, num);
			}
			yield return 0.05f;
			Audio.SetMusicParam("boss_pitch", 0f);
			float from2 = Engine.TimeRate;
			Tween tween3 = Tween.Create(Tween.TweenMode.Oneshot, null, 0.35f / Engine.TimeRateB, true);
			tween3.UseRawDeltaTime = true;
			tween3.OnUpdate = delegate(Tween t)
			{
				if (CS$<>8__locals1.<>4__this.bossBg != null && CS$<>8__locals1.<>4__this.bossBg.Alpha < t.Eased)
				{
					CS$<>8__locals1.<>4__this.bossBg.Alpha = t.Eased;
				}
				Engine.TimeRate = MathHelper.Lerp(from2, 1f, t.Eased);
				if (CS$<>8__locals1.lastHit)
				{
					Glitch.Value = 0.6f * (1f - t.Eased);
				}
			};
			base.Add(tween3);
			yield return 0.2f;
			Vector2 from = this.Position;
			Vector2 to = this.nodes[this.nodeIndex];
			float duration = Vector2.Distance(from, to) / 600f;
			float dir = (to - from).Angle();
			Tween tween4 = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, duration, true);
			tween4.OnUpdate = delegate(Tween t)
			{
				CS$<>8__locals1.<>4__this.Position = Vector2.Lerp(from, to, t.Eased);
				if (t.Eased >= 0.1f && t.Eased <= 0.9f && CS$<>8__locals1.<>4__this.Scene.OnInterval(0.02f))
				{
					TrailManager.Add(CS$<>8__locals1.<>4__this, Player.NormalHairColor, 0.5f, false, false);
					CS$<>8__locals1.<>4__this.level.Particles.Emit(Player.P_DashB, 2, CS$<>8__locals1.<>4__this.Center, Vector2.One * 3f, dir);
				}
			};
			tween4.OnComplete = delegate(Tween t)
			{
				CS$<>8__locals1.<>4__this.Sprite.Play("recoverHit", false, false);
				CS$<>8__locals1.<>4__this.Moving = false;
				CS$<>8__locals1.<>4__this.Collidable = true;
				Player entity2 = CS$<>8__locals1.<>4__this.Scene.Tracker.GetEntity<Player>();
				if (entity2 != null)
				{
					CS$<>8__locals1.<>4__this.facing = Math.Sign(entity2.X - CS$<>8__locals1.<>4__this.X);
					if (CS$<>8__locals1.<>4__this.facing == 0)
					{
						CS$<>8__locals1.<>4__this.facing = -1;
					}
				}
				CS$<>8__locals1.<>4__this.StartAttacking();
				CS$<>8__locals1.<>4__this.floatSine.Reset();
			};
			base.Add(tween4);
			yield break;
		}

		// Token: 0x06000E65 RID: 3685 RVA: 0x00034D10 File Offset: 0x00032F10
		private void PushPlayer(Player player)
		{
			if (player != null && !player.Dead)
			{
				int num = Math.Sign(base.X - this.nodes[this.nodeIndex].X);
				if (num == 0)
				{
					num = -1;
				}
				player.FinalBossPushLaunch(num);
			}
			base.SceneAs<Level>().Displacement.AddBurst(this.Position, 0.4f, 12f, 36f, 0.5f, null, null);
			base.SceneAs<Level>().Displacement.AddBurst(this.Position, 0.4f, 24f, 48f, 0.5f, null, null);
			base.SceneAs<Level>().Displacement.AddBurst(this.Position, 0.4f, 36f, 60f, 0.5f, null, null);
		}

		// Token: 0x06000E66 RID: 3686 RVA: 0x00034DE0 File Offset: 0x00032FE0
		private void TriggerFallingBlocks(float leftOfX)
		{
			while (this.fallingBlocks.Count > 0 && this.fallingBlocks[0].Scene == null)
			{
				this.fallingBlocks.RemoveAt(0);
			}
			int num = 0;
			while (this.fallingBlocks.Count > 0 && this.fallingBlocks[0].X < leftOfX)
			{
				FallingBlock fallingBlock = this.fallingBlocks[0] as FallingBlock;
				fallingBlock.StartShaking(0f);
				fallingBlock.Triggered = true;
				fallingBlock.FallDelay = 0.4f * (float)num;
				num++;
				this.fallingBlocks.RemoveAt(0);
			}
		}

		// Token: 0x06000E67 RID: 3687 RVA: 0x00034E84 File Offset: 0x00033084
		private void TriggerMovingBlocks(int nodeIndex)
		{
			if (nodeIndex > 0)
			{
				this.DestroyMovingBlocks(nodeIndex - 1);
			}
			float num = 0f;
			foreach (Entity entity in this.movingBlocks)
			{
				FinalBossMovingBlock finalBossMovingBlock = (FinalBossMovingBlock)entity;
				if (finalBossMovingBlock.BossNodeIndex == nodeIndex)
				{
					finalBossMovingBlock.StartMoving(num);
					num += 0.15f;
				}
			}
		}

		// Token: 0x06000E68 RID: 3688 RVA: 0x00034F00 File Offset: 0x00033100
		private void DestroyMovingBlocks(int nodeIndex)
		{
			float num = 0f;
			foreach (Entity entity in this.movingBlocks)
			{
				FinalBossMovingBlock finalBossMovingBlock = (FinalBossMovingBlock)entity;
				if (finalBossMovingBlock.BossNodeIndex == nodeIndex)
				{
					finalBossMovingBlock.Destroy(num);
					num += 0.05f;
				}
			}
		}

		// Token: 0x06000E69 RID: 3689 RVA: 0x00034F70 File Offset: 0x00033170
		private void StartAttacking()
		{
			switch (this.patternIndex)
			{
			case 0:
			case 1:
				this.attackCoroutine.Replace(this.Attack01Sequence());
				return;
			case 2:
				this.attackCoroutine.Replace(this.Attack02Sequence());
				return;
			case 3:
				this.attackCoroutine.Replace(this.Attack03Sequence());
				return;
			case 4:
				this.attackCoroutine.Replace(this.Attack04Sequence());
				return;
			case 5:
				this.attackCoroutine.Replace(this.Attack05Sequence());
				return;
			case 6:
				this.attackCoroutine.Replace(this.Attack06Sequence());
				return;
			case 7:
				this.attackCoroutine.Replace(this.Attack07Sequence());
				return;
			case 8:
				this.attackCoroutine.Replace(this.Attack08Sequence());
				return;
			case 9:
				this.attackCoroutine.Replace(this.Attack09Sequence());
				return;
			case 10:
				this.attackCoroutine.Replace(this.Attack10Sequence());
				return;
			case 11:
				this.attackCoroutine.Replace(this.Attack11Sequence());
				return;
			case 12:
				break;
			case 13:
				this.attackCoroutine.Replace(this.Attack13Sequence());
				return;
			case 14:
				this.attackCoroutine.Replace(this.Attack14Sequence());
				return;
			case 15:
				this.attackCoroutine.Replace(this.Attack15Sequence());
				break;
			default:
				return;
			}
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x000350C6 File Offset: 0x000332C6
		private void StartShootCharge()
		{
			this.Sprite.Play("attack1Begin", false, false);
			this.chargeSfx.Play("event:/char/badeline/boss_bullet", null, 0f);
		}

		// Token: 0x06000E6B RID: 3691 RVA: 0x000350F1 File Offset: 0x000332F1
		private IEnumerator Attack01Sequence()
		{
			this.StartShootCharge();
			for (;;)
			{
				yield return 0.5f;
				this.Shoot(0f);
				yield return 1f;
				this.StartShootCharge();
				yield return 0.15f;
				yield return 0.3f;
			}
			yield break;
		}

		// Token: 0x06000E6C RID: 3692 RVA: 0x00035100 File Offset: 0x00033300
		private IEnumerator Attack02Sequence()
		{
			for (;;)
			{
				yield return 0.5f;
				yield return this.Beam();
				yield return 0.4f;
				this.StartShootCharge();
				yield return 0.3f;
				this.Shoot(0f);
				yield return 0.5f;
				yield return 0.3f;
			}
			yield break;
		}

		// Token: 0x06000E6D RID: 3693 RVA: 0x0003510F File Offset: 0x0003330F
		private IEnumerator Attack03Sequence()
		{
			this.StartShootCharge();
			yield return 0.1f;
			for (;;)
			{
				int num;
				for (int i = 0; i < 5; i = num + 1)
				{
					Player entity = this.level.Tracker.GetEntity<Player>();
					if (entity != null)
					{
						Vector2 at = entity.Center;
						for (int j = 0; j < 2; j = num + 1)
						{
							this.ShootAt(at);
							yield return 0.15f;
							num = j;
						}
						at = default(Vector2);
					}
					if (i < 4)
					{
						this.StartShootCharge();
						yield return 0.5f;
					}
					num = i;
				}
				yield return 2f;
				this.StartShootCharge();
				yield return 0.7f;
			}
			yield break;
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x0003511E File Offset: 0x0003331E
		private IEnumerator Attack04Sequence()
		{
			this.StartShootCharge();
			yield return 0.1f;
			for (;;)
			{
				int num;
				for (int i = 0; i < 5; i = num + 1)
				{
					Player entity = this.level.Tracker.GetEntity<Player>();
					if (entity != null)
					{
						Vector2 at = entity.Center;
						for (int j = 0; j < 2; j = num + 1)
						{
							this.ShootAt(at);
							yield return 0.15f;
							num = j;
						}
						at = default(Vector2);
					}
					if (i < 4)
					{
						this.StartShootCharge();
						yield return 0.5f;
					}
					num = i;
				}
				yield return 1.5f;
				yield return this.Beam();
				yield return 1.5f;
				this.StartShootCharge();
			}
			yield break;
		}

		// Token: 0x06000E6F RID: 3695 RVA: 0x0003512D File Offset: 0x0003332D
		private IEnumerator Attack05Sequence()
		{
			yield return 0.2f;
			for (;;)
			{
				yield return this.Beam();
				yield return 0.6f;
				this.StartShootCharge();
				yield return 0.3f;
				int num;
				for (int i = 0; i < 3; i = num + 1)
				{
					Player entity = this.level.Tracker.GetEntity<Player>();
					if (entity != null)
					{
						Vector2 at = entity.Center;
						for (int j = 0; j < 2; j = num + 1)
						{
							this.ShootAt(at);
							yield return 0.15f;
							num = j;
						}
						at = default(Vector2);
					}
					if (i < 2)
					{
						this.StartShootCharge();
						yield return 0.5f;
					}
					num = i;
				}
				yield return 0.8f;
			}
			yield break;
		}

		// Token: 0x06000E70 RID: 3696 RVA: 0x0003513C File Offset: 0x0003333C
		private IEnumerator Attack06Sequence()
		{
			for (;;)
			{
				yield return this.Beam();
				yield return 0.7f;
			}
			yield break;
		}

		// Token: 0x06000E71 RID: 3697 RVA: 0x0003514B File Offset: 0x0003334B
		private IEnumerator Attack07Sequence()
		{
			for (;;)
			{
				this.Shoot(0f);
				yield return 0.8f;
				this.StartShootCharge();
				yield return 0.8f;
			}
			yield break;
		}

		// Token: 0x06000E72 RID: 3698 RVA: 0x0003515A File Offset: 0x0003335A
		private IEnumerator Attack08Sequence()
		{
			for (;;)
			{
				yield return 0.1f;
				yield return this.Beam();
				yield return 0.8f;
			}
			yield break;
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x00035169 File Offset: 0x00033369
		private IEnumerator Attack09Sequence()
		{
			this.StartShootCharge();
			for (;;)
			{
				yield return 0.5f;
				this.Shoot(0f);
				yield return 0.15f;
				this.StartShootCharge();
				this.Shoot(0f);
				yield return 0.4f;
				this.StartShootCharge();
				yield return 0.1f;
			}
			yield break;
		}

		// Token: 0x06000E74 RID: 3700 RVA: 0x00035178 File Offset: 0x00033378
		private IEnumerator Attack10Sequence()
		{
			yield break;
		}

		// Token: 0x06000E75 RID: 3701 RVA: 0x00035180 File Offset: 0x00033380
		private IEnumerator Attack11Sequence()
		{
			if (this.nodeIndex == 0)
			{
				this.StartShootCharge();
				yield return 0.6f;
			}
			for (;;)
			{
				this.Shoot(0f);
				yield return 1.9f;
				this.StartShootCharge();
				yield return 0.6f;
			}
			yield break;
		}

		// Token: 0x06000E76 RID: 3702 RVA: 0x0003518F File Offset: 0x0003338F
		private IEnumerator Attack13Sequence()
		{
			if (this.nodeIndex == 0)
			{
				yield break;
			}
			yield return this.Attack01Sequence();
			yield break;
		}

		// Token: 0x06000E77 RID: 3703 RVA: 0x0003519E File Offset: 0x0003339E
		private IEnumerator Attack14Sequence()
		{
			for (;;)
			{
				yield return 0.2f;
				yield return this.Beam();
				yield return 0.3f;
			}
			yield break;
		}

		// Token: 0x06000E78 RID: 3704 RVA: 0x000351AD File Offset: 0x000333AD
		private IEnumerator Attack15Sequence()
		{
			for (;;)
			{
				yield return 0.2f;
				yield return this.Beam();
				yield return 1.2f;
			}
			yield break;
		}

		// Token: 0x06000E79 RID: 3705 RVA: 0x000351BC File Offset: 0x000333BC
		private void Shoot(float angleOffset = 0f)
		{
			if (!this.chargeSfx.Playing)
			{
				this.chargeSfx.Play("event:/char/badeline/boss_bullet", "end", 1f);
			}
			else
			{
				this.chargeSfx.Param("end", 1f);
			}
			this.Sprite.Play("attack1Recoil", true, false);
			Player entity = this.level.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				this.level.Add(Engine.Pooler.Create<FinalBossShot>().Init(this, entity, angleOffset));
			}
		}

		// Token: 0x06000E7A RID: 3706 RVA: 0x0003524C File Offset: 0x0003344C
		private void ShootAt(Vector2 at)
		{
			if (!this.chargeSfx.Playing)
			{
				this.chargeSfx.Play("event:/char/badeline/boss_bullet", "end", 1f);
			}
			else
			{
				this.chargeSfx.Param("end", 1f);
			}
			this.Sprite.Play("attack1Recoil", true, false);
			this.level.Add(Engine.Pooler.Create<FinalBossShot>().Init(this, at));
		}

		// Token: 0x06000E7B RID: 3707 RVA: 0x000352C7 File Offset: 0x000334C7
		private IEnumerator Beam()
		{
			this.laserSfx.Play("event:/char/badeline/boss_laser_charge", null, 0f);
			this.Sprite.Play("attack2Begin", true, false);
			yield return 0.1f;
			Player entity = this.level.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				this.level.Add(Engine.Pooler.Create<FinalBossBeam>().Init(this, entity));
			}
			yield return 0.9f;
			this.Sprite.Play("attack2Lock", true, false);
			yield return 0.5f;
			this.laserSfx.Stop(true);
			Audio.Play("event:/char/badeline/boss_laser_fire", this.Position);
			this.Sprite.Play("attack2Recoil", false, false);
			yield break;
		}

		// Token: 0x06000E7C RID: 3708 RVA: 0x000352D6 File Offset: 0x000334D6
		public override void Removed(Scene scene)
		{
			if (this.bossBg != null && this.patternIndex == 0)
			{
				this.bossBg.Alpha = 1f;
			}
			base.Removed(scene);
		}

		// Token: 0x04000987 RID: 2439
		public static ParticleType P_Burst;

		// Token: 0x04000988 RID: 2440
		public const float CameraXPastMax = 140f;

		// Token: 0x04000989 RID: 2441
		private const float MoveSpeed = 600f;

		// Token: 0x0400098A RID: 2442
		private const float AvoidRadius = 12f;

		// Token: 0x0400098B RID: 2443
		public Sprite Sprite;

		// Token: 0x0400098C RID: 2444
		public PlayerSprite NormalSprite;

		// Token: 0x0400098D RID: 2445
		private PlayerHair normalHair;

		// Token: 0x0400098E RID: 2446
		private Vector2 avoidPos;

		// Token: 0x0400098F RID: 2447
		public float CameraYPastMax;

		// Token: 0x04000990 RID: 2448
		public bool Moving;

		// Token: 0x04000991 RID: 2449
		public bool Sitting;

		// Token: 0x04000992 RID: 2450
		private int facing;

		// Token: 0x04000993 RID: 2451
		private Level level;

		// Token: 0x04000994 RID: 2452
		private Circle circle;

		// Token: 0x04000995 RID: 2453
		private Vector2[] nodes;

		// Token: 0x04000996 RID: 2454
		private int nodeIndex;

		// Token: 0x04000997 RID: 2455
		private int patternIndex;

		// Token: 0x04000998 RID: 2456
		private Coroutine attackCoroutine;

		// Token: 0x04000999 RID: 2457
		private Coroutine triggerBlocksCoroutine;

		// Token: 0x0400099A RID: 2458
		private List<Entity> fallingBlocks;

		// Token: 0x0400099B RID: 2459
		private List<Entity> movingBlocks;

		// Token: 0x0400099C RID: 2460
		private bool playerHasMoved;

		// Token: 0x0400099D RID: 2461
		private SineWave floatSine;

		// Token: 0x0400099E RID: 2462
		private bool dialog;

		// Token: 0x0400099F RID: 2463
		private bool startHit;

		// Token: 0x040009A0 RID: 2464
		private VertexLight light;

		// Token: 0x040009A1 RID: 2465
		private Wiggler scaleWiggler;

		// Token: 0x040009A2 RID: 2466
		private FinalBossStarfield bossBg;

		// Token: 0x040009A3 RID: 2467
		private SoundSource chargeSfx;

		// Token: 0x040009A4 RID: 2468
		private SoundSource laserSfx;
	}
}
