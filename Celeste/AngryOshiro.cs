using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200028B RID: 651
	[Tracked(false)]
	public class AngryOshiro : Entity
	{
		// Token: 0x06001420 RID: 5152 RVA: 0x0006C7B8 File Offset: 0x0006A9B8
		public AngryOshiro(Vector2 position, bool fromCutscene) : base(position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("oshiro_boss"));
			this.Sprite.Play("idle", false, false);
			base.Add(this.lightning = GFX.SpriteBank.Create("oshiro_boss_lightning"));
			this.lightning.Visible = false;
			this.lightning.OnFinish = delegate(string s)
			{
				this.lightningVisible = false;
			};
			base.Collider = new Circle(14f, 0f, 0f);
			base.Collider.Position = (this.colliderTargetPosition = new Vector2(3f, 4f));
			base.Add(this.sine = new SineWave(0.5f, 0f));
			base.Add(this.bounceCollider = new PlayerCollider(new Action<Player>(this.OnPlayerBounce), new Hitbox(28f, 6f, -11f, -11f), null));
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Depth = -12500;
			this.Visible = false;
			base.Add(this.light = new VertexLight(Color.White, 1f, 32, 64));
			base.Add(this.shaker = new Shaker(false, null));
			this.state = new StateMachine(10);
			this.state.SetCallbacks(0, new Func<int>(this.ChaseUpdate), new Func<IEnumerator>(this.ChaseCoroutine), new Action(this.ChaseBegin), null);
			this.state.SetCallbacks(1, new Func<int>(this.ChargeUpUpdate), new Func<IEnumerator>(this.ChargeUpCoroutine), null, new Action(this.ChargeUpEnd));
			this.state.SetCallbacks(2, new Func<int>(this.AttackUpdate), new Func<IEnumerator>(this.AttackCoroutine), new Action(this.AttackBegin), new Action(this.AttackEnd));
			this.state.SetCallbacks(3, null, null, null, null);
			this.state.SetCallbacks(4, new Func<int>(this.WaitingUpdate), null, null, null);
			this.state.SetCallbacks(5, new Func<int>(this.HurtUpdate), null, new Action(this.HurtBegin), null);
			base.Add(this.state);
			if (fromCutscene)
			{
				this.yApproachSpeed = 0f;
			}
			this.fromCutscene = fromCutscene;
			base.Add(new TransitionListener
			{
				OnOutBegin = delegate()
				{
					if (base.X > (float)this.level.Bounds.Left + this.Sprite.Width / 2f)
					{
						this.Visible = false;
						return;
					}
					this.easeBackFromRightEdge = true;
				},
				OnOut = delegate(float f)
				{
					this.lightning.Update();
					if (this.easeBackFromRightEdge)
					{
						base.X -= 128f * Engine.RawDeltaTime;
					}
				}
			});
			base.Add(this.prechargeSfx = new SoundSource());
			base.Add(this.chargeSfx = new SoundSource());
			Distort.AnxietyOrigin = new Vector2(1f, 0.5f);
		}

		// Token: 0x06001421 RID: 5153 RVA: 0x0006CADB File Offset: 0x0006ACDB
		public AngryOshiro(EntityData data, Vector2 offset) : this(data.Position + offset, false)
		{
		}

		// Token: 0x06001422 RID: 5154 RVA: 0x0006CAF0 File Offset: 0x0006ACF0
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
			if (this.level.Session.GetFlag("oshiroEnding") || (!this.level.Session.GetFlag("oshiro_resort_roof") && this.level.Session.Level.Equals("roof00")))
			{
				base.RemoveSelf();
			}
			if (this.state.State != 3 && !this.fromCutscene)
			{
				this.state.State = 4;
			}
			if (!this.fromCutscene)
			{
				base.Y = this.TargetY;
				this.cameraXOffset = -48f;
				return;
			}
			this.cameraXOffset = base.X - this.level.Camera.Left;
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06001423 RID: 5155 RVA: 0x0006CBC0 File Offset: 0x0006ADC0
		private float TargetY
		{
			get
			{
				Player entity = this.level.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					return MathHelper.Clamp(entity.CenterY, (float)(this.level.Bounds.Top + 8), (float)(this.level.Bounds.Bottom - 8));
				}
				return base.Y;
			}
		}

		// Token: 0x06001424 RID: 5156 RVA: 0x0006CC20 File Offset: 0x0006AE20
		private void OnPlayer(Player player)
		{
			if (this.state.State != 5 && (base.CenterX < player.CenterX + 4f || this.Sprite.CurrentAnimationID != "respawn"))
			{
				player.Die((player.Center - base.Center).SafeNormalize(Vector2.UnitX), false, true);
			}
		}

		// Token: 0x06001425 RID: 5157 RVA: 0x0006CC8C File Offset: 0x0006AE8C
		private void OnPlayerBounce(Player player)
		{
			if (this.state.State == 2 && player.Bottom <= base.Top + 6f)
			{
				Audio.Play("event:/game/general/thing_booped", this.Position);
				Celeste.Freeze(0.2f);
				player.Bounce(base.Top + 2f);
				this.state.State = 5;
				this.prechargeSfx.Stop(true);
				this.chargeSfx.Stop(true);
			}
		}

		// Token: 0x06001426 RID: 5158 RVA: 0x0006CD10 File Offset: 0x0006AF10
		public override void Update()
		{
			base.Update();
			this.Sprite.Scale.X = Calc.Approach(this.Sprite.Scale.X, 1f, 0.6f * Engine.DeltaTime);
			this.Sprite.Scale.Y = Calc.Approach(this.Sprite.Scale.Y, 1f, 0.6f * Engine.DeltaTime);
			if (!this.doRespawnAnim)
			{
				this.Visible = (base.X > (float)this.level.Bounds.Left - base.Width / 2f);
			}
			this.yApproachSpeed = Calc.Approach(this.yApproachSpeed, 100f, 300f * Engine.DeltaTime);
			if (this.state.State != 3 && this.canControlTimeRate)
			{
				if (this.state.State == 2 && this.attackSpeed > 200f)
				{
					Player entity = base.Scene.Tracker.GetEntity<Player>();
					if (entity != null && !entity.Dead && base.CenterX < entity.CenterX + 4f)
					{
						Engine.TimeRate = MathHelper.Lerp(Calc.ClampedMap(entity.CenterX - base.CenterX, 30f, 80f, 0.5f, 1f), 1f, Calc.ClampedMap(Math.Abs(entity.CenterY - base.CenterY), 32f, 48f, 0f, 1f));
					}
					else
					{
						Engine.TimeRate = 1f;
					}
				}
				else
				{
					Engine.TimeRate = 1f;
				}
				Distort.GameRate = Calc.Approach(Distort.GameRate, Calc.Map(Engine.TimeRate, 0.5f, 1f, 0f, 1f), Engine.DeltaTime * 8f);
				Distort.Anxiety = Calc.Approach(Distort.Anxiety, this.targetAnxiety, this.anxietySpeed * Engine.DeltaTime);
				return;
			}
			Distort.GameRate = 1f;
			Distort.Anxiety = 0f;
		}

		// Token: 0x06001427 RID: 5159 RVA: 0x0006CF38 File Offset: 0x0006B138
		public void StopControllingTime()
		{
			this.canControlTimeRate = false;
		}

		// Token: 0x06001428 RID: 5160 RVA: 0x0006CF44 File Offset: 0x0006B144
		public override void Render()
		{
			if (this.lightningVisible)
			{
				this.lightning.RenderPosition = new Vector2(this.level.Camera.Left - 2f, base.Top + 16f);
				this.lightning.Render();
			}
			this.Sprite.Position = this.shaker.Value * 2f;
			base.Render();
		}

		// Token: 0x06001429 RID: 5161 RVA: 0x0006CFBC File Offset: 0x0006B1BC
		public void Leave()
		{
			this.leaving = true;
		}

		// Token: 0x0600142A RID: 5162 RVA: 0x0006CFC5 File Offset: 0x0006B1C5
		public void Squish()
		{
			this.Sprite.Scale = new Vector2(1.3f, 0.5f);
			this.shaker.ShakeFor(0.5f, false);
		}

		// Token: 0x0600142B RID: 5163 RVA: 0x0006CFF3 File Offset: 0x0006B1F3
		private void ChaseBegin()
		{
			this.Sprite.Play("idle", false, false);
		}

		// Token: 0x0600142C RID: 5164 RVA: 0x0006D008 File Offset: 0x0006B208
		private int ChaseUpdate()
		{
			if (!this.hasEnteredSfx && this.cameraXOffset >= -16f && !this.doRespawnAnim)
			{
				Audio.Play("event:/char/oshiro/boss_enter_screen", this.Position);
				this.hasEnteredSfx = true;
			}
			if (this.doRespawnAnim && this.cameraXOffset >= 0f)
			{
				base.Collider.Position.X = -48f;
				this.Visible = true;
				this.Sprite.Play("respawn", false, false);
				this.doRespawnAnim = false;
				if (base.Scene.Tracker.GetEntity<Player>() != null)
				{
					Audio.Play("event:/char/oshiro/boss_reform", this.Position);
				}
			}
			this.cameraXOffset = Calc.Approach(this.cameraXOffset, 20f, 80f * Engine.DeltaTime);
			base.X = this.level.Camera.Left + this.cameraXOffset;
			base.Collider.Position.X = Calc.Approach(base.Collider.Position.X, this.colliderTargetPosition.X, Engine.DeltaTime * 128f);
			this.Collidable = this.Visible;
			if (this.level.Tracker.GetEntity<Player>() != null && this.Sprite.CurrentAnimationID != "respawn")
			{
				base.CenterY = Calc.Approach(base.CenterY, this.TargetY, this.yApproachSpeed * Engine.DeltaTime);
			}
			return 0;
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x0006D189 File Offset: 0x0006B389
		private IEnumerator ChaseCoroutine()
		{
			if (this.level.Session.Area.Mode != AreaMode.Normal)
			{
				yield return 1f;
			}
			else
			{
				yield return AngryOshiro.ChaseWaitTimes[this.attackIndex];
				this.attackIndex++;
				this.attackIndex %= AngryOshiro.ChaseWaitTimes.Length;
			}
			this.prechargeSfx.Play("event:/char/oshiro/boss_precharge", null, 0f);
			this.Sprite.Play("charge", false, false);
			yield return 0.7f;
			if (base.Scene.Tracker.GetEntity<Player>() != null)
			{
				Alarm.Set(this, 0.216f, delegate
				{
					this.chargeSfx.Play("event:/char/oshiro/boss_charge", null, 0f);
				}, Alarm.AlarmMode.Oneshot);
				this.state.State = 1;
			}
			else
			{
				this.Sprite.Play("idle", false, false);
			}
			yield break;
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x0006D198 File Offset: 0x0006B398
		private int ChargeUpUpdate()
		{
			if (this.level.OnInterval(0.05f))
			{
				this.Sprite.Position = Calc.Random.ShakeVector();
			}
			this.cameraXOffset = Calc.Approach(this.cameraXOffset, 0f, 40f * Engine.DeltaTime);
			base.X = this.level.Camera.Left + this.cameraXOffset;
			Player entity = this.level.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				base.CenterY = Calc.Approach(base.CenterY, MathHelper.Clamp(entity.CenterY, (float)(this.level.Bounds.Top + 8), (float)(this.level.Bounds.Bottom - 8)), 30f * Engine.DeltaTime);
			}
			return 1;
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x0006D272 File Offset: 0x0006B472
		private void ChargeUpEnd()
		{
			this.Sprite.Position = Vector2.Zero;
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x0006D284 File Offset: 0x0006B484
		private IEnumerator ChargeUpCoroutine()
		{
			Celeste.Freeze(0.05f);
			Distort.Anxiety = 0.3f;
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			this.lightningVisible = true;
			this.lightning.Play("once", true, false);
			yield return 0.3f;
			if (base.Scene.Tracker.GetEntity<Player>() != null)
			{
				this.state.State = 2;
			}
			else
			{
				this.state.State = 0;
			}
			yield break;
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x0006D293 File Offset: 0x0006B493
		private void AttackBegin()
		{
			this.attackSpeed = 0f;
			this.targetAnxiety = 0.3f;
			this.anxietySpeed = 4f;
			this.level.DirectionalShake(Vector2.UnitX, 0.3f);
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x0006D2CB File Offset: 0x0006B4CB
		private void AttackEnd()
		{
			this.targetAnxiety = 0f;
			this.anxietySpeed = 0.5f;
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x0006D2E4 File Offset: 0x0006B4E4
		private int AttackUpdate()
		{
			base.X += this.attackSpeed * Engine.DeltaTime;
			this.attackSpeed = Calc.Approach(this.attackSpeed, 500f, 2000f * Engine.DeltaTime);
			if (base.X < this.level.Camera.Right + 48f)
			{
				Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
				if (base.Scene.OnInterval(0.05f))
				{
					TrailManager.Add(this, Color.Red * 0.6f, 0.5f, false, false);
				}
				return 2;
			}
			if (this.leaving)
			{
				base.RemoveSelf();
				return 2;
			}
			base.X = this.level.Camera.Left + -48f;
			this.cameraXOffset = -48f;
			this.doRespawnAnim = true;
			this.Visible = false;
			return 0;
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x0006D3C6 File Offset: 0x0006B5C6
		private IEnumerator AttackCoroutine()
		{
			yield return 0.1f;
			this.targetAnxiety = 0f;
			this.anxietySpeed = 0.5f;
			yield break;
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06001435 RID: 5173 RVA: 0x0006D3D5 File Offset: 0x0006B5D5
		public bool DummyMode
		{
			get
			{
				return this.state.State == 3;
			}
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x0006D3E5 File Offset: 0x0006B5E5
		public void EnterDummyMode()
		{
			this.state.State = 3;
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x0006D3F3 File Offset: 0x0006B5F3
		public void LeaveDummyMode()
		{
			this.state.State = 0;
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x0006D404 File Offset: 0x0006B604
		private int WaitingUpdate()
		{
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null && entity.Speed != Vector2.Zero && entity.X > (float)(this.level.Bounds.Left + 48))
			{
				return 0;
			}
			return 4;
		}

		// Token: 0x06001439 RID: 5177 RVA: 0x0006D459 File Offset: 0x0006B659
		private void HurtBegin()
		{
			this.Sprite.Play("hurt", true, false);
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x0006D470 File Offset: 0x0006B670
		private int HurtUpdate()
		{
			base.X += 100f * Engine.DeltaTime;
			base.Y += 200f * Engine.DeltaTime;
			if (base.Top <= (float)(this.level.Bounds.Bottom + 20))
			{
				return 5;
			}
			if (this.leaving)
			{
				base.RemoveSelf();
				return 5;
			}
			base.X = this.level.Camera.Left + -48f;
			this.cameraXOffset = -48f;
			this.doRespawnAnim = true;
			this.Visible = false;
			return 0;
		}

		// Token: 0x04000FC8 RID: 4040
		private const int StChase = 0;

		// Token: 0x04000FC9 RID: 4041
		private const int StChargeUp = 1;

		// Token: 0x04000FCA RID: 4042
		private const int StAttack = 2;

		// Token: 0x04000FCB RID: 4043
		private const int StDummy = 3;

		// Token: 0x04000FCC RID: 4044
		private const int StWaiting = 4;

		// Token: 0x04000FCD RID: 4045
		private const int StHurt = 5;

		// Token: 0x04000FCE RID: 4046
		private const float HitboxBackRange = 4f;

		// Token: 0x04000FCF RID: 4047
		public Sprite Sprite;

		// Token: 0x04000FD0 RID: 4048
		private Sprite lightning;

		// Token: 0x04000FD1 RID: 4049
		private bool lightningVisible;

		// Token: 0x04000FD2 RID: 4050
		private VertexLight light;

		// Token: 0x04000FD3 RID: 4051
		private Level level;

		// Token: 0x04000FD4 RID: 4052
		private SineWave sine;

		// Token: 0x04000FD5 RID: 4053
		private float cameraXOffset;

		// Token: 0x04000FD6 RID: 4054
		private StateMachine state;

		// Token: 0x04000FD7 RID: 4055
		private int attackIndex;

		// Token: 0x04000FD8 RID: 4056
		private float targetAnxiety;

		// Token: 0x04000FD9 RID: 4057
		private float anxietySpeed;

		// Token: 0x04000FDA RID: 4058
		private bool easeBackFromRightEdge;

		// Token: 0x04000FDB RID: 4059
		private bool fromCutscene;

		// Token: 0x04000FDC RID: 4060
		private bool doRespawnAnim;

		// Token: 0x04000FDD RID: 4061
		private bool leaving;

		// Token: 0x04000FDE RID: 4062
		private Shaker shaker;

		// Token: 0x04000FDF RID: 4063
		private PlayerCollider bounceCollider;

		// Token: 0x04000FE0 RID: 4064
		private Vector2 colliderTargetPosition;

		// Token: 0x04000FE1 RID: 4065
		private bool canControlTimeRate = true;

		// Token: 0x04000FE2 RID: 4066
		private SoundSource prechargeSfx;

		// Token: 0x04000FE3 RID: 4067
		private SoundSource chargeSfx;

		// Token: 0x04000FE4 RID: 4068
		private bool hasEnteredSfx;

		// Token: 0x04000FE5 RID: 4069
		private const float minCameraOffsetX = -48f;

		// Token: 0x04000FE6 RID: 4070
		private const float yApproachTargetSpeed = 100f;

		// Token: 0x04000FE7 RID: 4071
		private float yApproachSpeed = 100f;

		// Token: 0x04000FE8 RID: 4072
		private static readonly float[] ChaseWaitTimes = new float[]
		{
			1f,
			2f,
			3f,
			2f,
			3f
		};

		// Token: 0x04000FE9 RID: 4073
		private float attackSpeed;

		// Token: 0x04000FEA RID: 4074
		private const float HurtXSpeed = 100f;

		// Token: 0x04000FEB RID: 4075
		private const float HurtYSpeed = 200f;
	}
}
