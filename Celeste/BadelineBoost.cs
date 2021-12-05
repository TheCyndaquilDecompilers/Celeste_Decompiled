using System;
using System.Collections;
using System.Diagnostics;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200018A RID: 394
	public class BadelineBoost : Entity
	{
		// Token: 0x06000DD0 RID: 3536 RVA: 0x00031364 File Offset: 0x0002F564
		public BadelineBoost(Vector2[] nodes, bool lockCamera, bool canSkip = false, bool finalCh9Boost = false, bool finalCh9GoldenBoost = false, bool finalCh9Dialog = false) : base(nodes[0])
		{
			base.Depth = -1000000;
			this.nodes = nodes;
			this.canSkip = canSkip;
			this.finalCh9Boost = finalCh9Boost;
			this.finalCh9GoldenBoost = finalCh9GoldenBoost;
			this.finalCh9Dialog = finalCh9Dialog;
			base.Collider = new Circle(16f, 0f, 0f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(this.sprite = GFX.SpriteBank.Create("badelineBoost"));
			base.Add(this.stretch = new Image(GFX.Game["objects/badelineboost/stretch"]));
			this.stretch.Visible = false;
			this.stretch.CenterOrigin();
			base.Add(this.light = new VertexLight(Color.White, 0.7f, 12, 20));
			base.Add(this.bloom = new BloomPoint(0.5f, 12f));
			base.Add(this.wiggler = Wiggler.Create(0.4f, 3f, delegate(float f)
			{
				this.sprite.Scale = Vector2.One * (1f + this.wiggler.Value * 0.4f);
			}, false, false));
			if (lockCamera)
			{
				base.Add(new CameraLocker(Level.CameraLockModes.BoostSequence, 0f, 160f));
			}
			base.Add(this.relocateSfx = new SoundSource());
		}

		// Token: 0x06000DD1 RID: 3537 RVA: 0x000314D8 File Offset: 0x0002F6D8
		public BadelineBoost(EntityData data, Vector2 offset) : this(data.NodesWithPosition(offset), data.Bool("lockCamera", true), data.Bool("canSkip", false), data.Bool("finalCh9Boost", false), data.Bool("finalCh9GoldenBoost", false), data.Bool("finalCh9Dialog", false))
		{
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x0003152E File Offset: 0x0002F72E
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (base.CollideCheck<FakeWall>())
			{
				base.Depth = -12500;
			}
		}

		// Token: 0x06000DD3 RID: 3539 RVA: 0x0003154A File Offset: 0x0002F74A
		private void OnPlayer(Player player)
		{
			base.Add(new Coroutine(this.BoostRoutine(player), true));
		}

		// Token: 0x06000DD4 RID: 3540 RVA: 0x0003155F File Offset: 0x0002F75F
		private IEnumerator BoostRoutine(Player player)
		{
			this.holding = player;
			this.travelling = true;
			this.nodeIndex++;
			this.sprite.Visible = false;
			this.sprite.Position = Vector2.Zero;
			this.Collidable = false;
			bool finalBoost = this.nodeIndex >= this.nodes.Length;
			Level level = base.Scene as Level;
			bool endLevel = false;
			if (finalBoost && this.finalCh9GoldenBoost)
			{
				endLevel = true;
			}
			else
			{
				bool flag = false;
				foreach (Follower follower in player.Leader.Followers)
				{
					Strawberry strawberry = follower.Entity as Strawberry;
					if (strawberry != null && strawberry.Golden)
					{
						flag = true;
						break;
					}
				}
				endLevel = (finalBoost && this.finalCh9Boost && !flag);
			}
			Stopwatch sw = new Stopwatch();
			sw.Start();
			if (this.finalCh9Boost)
			{
				Audio.Play("event:/new_content/char/badeline/booster_finalfinal_part1", this.Position);
			}
			else if (!finalBoost)
			{
				Audio.Play("event:/char/badeline/booster_begin", this.Position);
			}
			else
			{
				Audio.Play("event:/char/badeline/booster_final", this.Position);
			}
			if (player.Holding != null)
			{
				player.Drop();
			}
			player.StateMachine.State = 11;
			player.DummyAutoAnimate = false;
			player.DummyGravity = false;
			if (player.Inventory.Dashes > 1)
			{
				player.Dashes = 1;
			}
			else
			{
				player.RefillDash();
			}
			player.RefillStamina();
			player.Speed = Vector2.Zero;
			int num = Math.Sign(player.X - base.X);
			if (num == 0)
			{
				num = -1;
			}
			BadelineDummy badeline = new BadelineDummy(this.Position);
			base.Scene.Add(badeline);
			player.Facing = (Facings)(-(Facings)num);
			badeline.Sprite.Scale.X = (float)num;
			Vector2 playerFrom = player.Position;
			Vector2 playerTo = this.Position + new Vector2((float)(num * 4), -3f);
			Vector2 badelineFrom = badeline.Position;
			Vector2 badelineTo = this.Position + new Vector2((float)(-(float)num * 4), 3f);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.2f)
			{
				Vector2 vector = Vector2.Lerp(playerFrom, playerTo, p);
				if (player.Scene != null)
				{
					player.MoveToX(vector.X, null);
				}
				if (player.Scene != null)
				{
					player.MoveToY(vector.Y, null);
				}
				badeline.Position = Vector2.Lerp(badelineFrom, badelineTo, p);
				yield return null;
			}
			playerFrom = default(Vector2);
			playerTo = default(Vector2);
			badelineFrom = default(Vector2);
			badelineTo = default(Vector2);
			if (finalBoost)
			{
				Vector2 screenSpaceFocusPoint = new Vector2(Calc.Clamp(player.X - level.Camera.X, 120f, 200f), Calc.Clamp(player.Y - level.Camera.Y, 60f, 120f));
				base.Add(new Coroutine(level.ZoomTo(screenSpaceFocusPoint, 1.5f, 0.18f), true));
				Engine.TimeRate = 0.5f;
			}
			else
			{
				Audio.Play("event:/char/badeline/booster_throw", this.Position);
			}
			badeline.Sprite.Play("boost", false, false);
			yield return 0.1f;
			if (!player.Dead)
			{
				player.MoveV(5f, null, null);
			}
			yield return 0.1f;
			if (endLevel)
			{
				level.TimerStopped = true;
				level.RegisterAreaComplete();
			}
			if (finalBoost && this.finalCh9Boost)
			{
				base.Scene.Add(new CS10_FinalLaunch(player, this, this.finalCh9Dialog));
				player.Active = false;
				badeline.Active = false;
				this.Active = false;
				yield return null;
				player.Active = true;
				badeline.Active = true;
			}
			base.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate
			{
				if (player.Dashes < player.Inventory.Dashes)
				{
					player.Dashes++;
				}
				this.Scene.Remove(badeline);
				(this.Scene as Level).Displacement.AddBurst(badeline.Position, 0.25f, 8f, 32f, 0.5f, null, null);
			}, 0.15f, true));
			(base.Scene as Level).Shake(0.3f);
			this.holding = null;
			if (!finalBoost)
			{
				player.BadelineBoostLaunch(base.CenterX);
				Vector2 from = this.Position;
				Vector2 to = this.nodes[this.nodeIndex];
				float num2 = Vector2.Distance(from, to) / 320f;
				num2 = Math.Min(3f, num2);
				this.stretch.Visible = true;
				this.stretch.Rotation = (to - from).Angle();
				Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, num2, true);
				tween.OnUpdate = delegate(Tween t)
				{
					this.Position = Vector2.Lerp(from, to, t.Eased);
					this.stretch.Scale.X = 1f + Calc.YoYo(t.Eased) * 2f;
					this.stretch.Scale.Y = 1f - Calc.YoYo(t.Eased) * 0.75f;
					if (t.Eased < 0.9f && this.Scene.OnInterval(0.03f))
					{
						TrailManager.Add(this, Player.TwoDashesHairColor, 0.5f, false, false);
						level.ParticlesFG.Emit(BadelineBoost.P_Move, 1, this.Center, Vector2.One * 4f);
					}
				};
				tween.OnComplete = delegate(Tween t)
				{
					if (this.X >= (float)level.Bounds.Right)
					{
						this.RemoveSelf();
						return;
					}
					this.travelling = false;
					this.stretch.Visible = false;
					this.sprite.Visible = true;
					this.Collidable = true;
					Audio.Play("event:/char/badeline/booster_reappear", this.Position);
				};
				base.Add(tween);
				this.relocateSfx.Play("event:/char/badeline/booster_relocate", null, 0f);
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
				level.DirectionalShake(-Vector2.UnitY, 0.3f);
				level.Displacement.AddBurst(base.Center, 0.4f, 8f, 32f, 0.5f, null, null);
			}
			else
			{
				if (this.finalCh9Boost)
				{
					this.Ch9FinalBoostSfx = Audio.Play("event:/new_content/char/badeline/booster_finalfinal_part2", this.Position);
				}
				Console.WriteLine("TIME: " + sw.ElapsedMilliseconds);
				Engine.FreezeTimer = 0.1f;
				yield return null;
				if (endLevel)
				{
					level.TimerHidden = true;
				}
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
				level.Flash(Color.White * 0.5f, true);
				level.DirectionalShake(-Vector2.UnitY, 0.6f);
				level.Displacement.AddBurst(base.Center, 0.6f, 8f, 64f, 0.5f, null, null);
				level.ResetZoom();
				player.SummitLaunch(base.X);
				Engine.TimeRate = 1f;
				this.Finish();
			}
			yield break;
		}

		// Token: 0x06000DD5 RID: 3541 RVA: 0x00031578 File Offset: 0x0002F778
		private void Skip()
		{
			this.travelling = true;
			this.nodeIndex++;
			this.Collidable = false;
			Level level = base.SceneAs<Level>();
			Vector2 from = this.Position;
			Vector2 to = this.nodes[this.nodeIndex];
			float num = Vector2.Distance(from, to) / 320f;
			num = Math.Min(3f, num);
			this.stretch.Visible = true;
			this.stretch.Rotation = (to - from).Angle();
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, num, true);
			tween.OnUpdate = delegate(Tween t)
			{
				this.Position = Vector2.Lerp(from, to, t.Eased);
				this.stretch.Scale.X = 1f + Calc.YoYo(t.Eased) * 2f;
				this.stretch.Scale.Y = 1f - Calc.YoYo(t.Eased) * 0.75f;
				if (t.Eased < 0.9f && this.Scene.OnInterval(0.03f))
				{
					TrailManager.Add(this, Player.TwoDashesHairColor, 0.5f, false, false);
					level.ParticlesFG.Emit(BadelineBoost.P_Move, 1, this.Center, Vector2.One * 4f);
				}
			};
			tween.OnComplete = delegate(Tween t)
			{
				if (this.X >= (float)level.Bounds.Right)
				{
					this.RemoveSelf();
					return;
				}
				this.travelling = false;
				this.stretch.Visible = false;
				this.sprite.Visible = true;
				this.Collidable = true;
				Audio.Play("event:/char/badeline/booster_reappear", this.Position);
			};
			base.Add(tween);
			this.relocateSfx.Play("event:/char/badeline/booster_relocate", null, 0f);
			level.Displacement.AddBurst(base.Center, 0.4f, 8f, 32f, 0.5f, null, null);
		}

		// Token: 0x06000DD6 RID: 3542 RVA: 0x000316AC File Offset: 0x0002F8AC
		public void Wiggle()
		{
			this.wiggler.Start();
			(base.Scene as Level).Displacement.AddBurst(this.Position, 0.3f, 4f, 16f, 0.25f, null, null);
			Audio.Play("event:/game/general/crystalheart_pulse", this.Position);
		}

		// Token: 0x06000DD7 RID: 3543 RVA: 0x00031708 File Offset: 0x0002F908
		public override void Update()
		{
			if (this.sprite.Visible && base.Scene.OnInterval(0.05f))
			{
				base.SceneAs<Level>().ParticlesBG.Emit(BadelineBoost.P_Ambience, 1, base.Center, Vector2.One * 3f);
			}
			if (this.holding != null)
			{
				this.holding.Speed = Vector2.Zero;
			}
			if (!this.travelling)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					float scaleFactor = Calc.ClampedMap((entity.Center - this.Position).Length(), 16f, 64f, 12f, 0f);
					Vector2 value = (entity.Center - this.Position).SafeNormalize();
					this.sprite.Position = Calc.Approach(this.sprite.Position, value * scaleFactor, 32f * Engine.DeltaTime);
					if (this.canSkip && entity.Position.X - base.X >= 100f && this.nodeIndex + 1 < this.nodes.Length)
					{
						this.Skip();
					}
				}
			}
			this.light.Visible = (this.bloom.Visible = (this.sprite.Visible || this.stretch.Visible));
			base.Update();
		}

		// Token: 0x06000DD8 RID: 3544 RVA: 0x00031888 File Offset: 0x0002FA88
		private void Finish()
		{
			base.SceneAs<Level>().Displacement.AddBurst(base.Center, 0.5f, 24f, 96f, 0.4f, null, null);
			base.SceneAs<Level>().Particles.Emit(BadelineOldsite.P_Vanish, 12, base.Center, Vector2.One * 6f);
			base.SceneAs<Level>().CameraLockMode = Level.CameraLockModes.None;
			base.SceneAs<Level>().CameraOffset = new Vector2(0f, -16f);
			base.RemoveSelf();
		}

		// Token: 0x04000914 RID: 2324
		public static ParticleType P_Ambience;

		// Token: 0x04000915 RID: 2325
		public static ParticleType P_Move;

		// Token: 0x04000916 RID: 2326
		private const float MoveSpeed = 320f;

		// Token: 0x04000917 RID: 2327
		private Sprite sprite;

		// Token: 0x04000918 RID: 2328
		private Image stretch;

		// Token: 0x04000919 RID: 2329
		private Wiggler wiggler;

		// Token: 0x0400091A RID: 2330
		private VertexLight light;

		// Token: 0x0400091B RID: 2331
		private BloomPoint bloom;

		// Token: 0x0400091C RID: 2332
		private bool canSkip;

		// Token: 0x0400091D RID: 2333
		private bool finalCh9Boost;

		// Token: 0x0400091E RID: 2334
		private bool finalCh9GoldenBoost;

		// Token: 0x0400091F RID: 2335
		private bool finalCh9Dialog;

		// Token: 0x04000920 RID: 2336
		private Vector2[] nodes;

		// Token: 0x04000921 RID: 2337
		private int nodeIndex;

		// Token: 0x04000922 RID: 2338
		private bool travelling;

		// Token: 0x04000923 RID: 2339
		private Player holding;

		// Token: 0x04000924 RID: 2340
		private SoundSource relocateSfx;

		// Token: 0x04000925 RID: 2341
		public FMOD.Studio.EventInstance Ch9FinalBoostSfx;
	}
}
