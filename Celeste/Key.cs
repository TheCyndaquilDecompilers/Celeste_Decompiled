using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200032F RID: 815
	public class Key : Entity
	{
		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x0600198C RID: 6540 RVA: 0x000A492C File Offset: 0x000A2B2C
		// (set) Token: 0x0600198D RID: 6541 RVA: 0x000A4934 File Offset: 0x000A2B34
		public bool Turning { get; private set; }

		// Token: 0x0600198E RID: 6542 RVA: 0x000A4940 File Offset: 0x000A2B40
		public Key(Vector2 position, EntityID id, Vector2[] nodes) : base(position)
		{
			this.ID = id;
			base.Collider = new Hitbox(12f, 12f, -6f, -6f);
			this.nodes = nodes;
			base.Add(this.follower = new Follower(id, null, null));
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(new MirrorReflection());
			base.Add(this.sprite = GFX.SpriteBank.Create("key"));
			this.sprite.CenterOrigin();
			this.sprite.Play("idle", false, false);
			base.Add(new TransitionListener
			{
				OnOut = delegate(float f)
				{
					this.StartedUsing = false;
					if (!this.IsUsed)
					{
						if (this.tween != null)
						{
							this.tween.RemoveSelf();
							this.tween = null;
						}
						if (this.alarm != null)
						{
							this.alarm.RemoveSelf();
							this.alarm = null;
						}
						this.Turning = false;
						this.Visible = true;
						this.sprite.Visible = true;
						this.sprite.Rate = 1f;
						this.sprite.Scale = Vector2.One;
						this.sprite.Play("idle", false, false);
						this.sprite.Rotation = 0f;
						this.wiggler.Stop();
						this.follower.MoveTowardsLeader = true;
					}
				}
			});
			base.Add(this.wiggler = Wiggler.Create(0.4f, 4f, delegate(float v)
			{
				this.sprite.Scale = Vector2.One * (1f + v * 0.35f);
			}, false, false));
			base.Add(this.light = new VertexLight(Color.White, 1f, 32, 48));
		}

		// Token: 0x0600198F RID: 6543 RVA: 0x000A4A67 File Offset: 0x000A2C67
		public Key(EntityData data, Vector2 offset, EntityID id) : this(data.Position + offset, id, data.NodesOffset(offset))
		{
		}

		// Token: 0x06001990 RID: 6544 RVA: 0x000A4A84 File Offset: 0x000A2C84
		public Key(Player player, EntityID id) : this(player.Position + new Vector2((float)((Facings)(-12) * player.Facing), -8f), id, null)
		{
			player.Leader.GainFollower(this.follower);
			this.Collidable = false;
			base.Depth = -1000000;
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x000A4ADC File Offset: 0x000A2CDC
		public override void Added(Scene scene)
		{
			base.Added(scene);
			ParticleSystem particlesFG = (scene as Level).ParticlesFG;
			base.Add(this.shimmerParticles = new ParticleEmitter(particlesFG, Key.P_Shimmer, Vector2.Zero, new Vector2(6f, 6f), 1, 0.1f));
			this.shimmerParticles.SimulateCycle();
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x000A4B3B File Offset: 0x000A2D3B
		public override void Update()
		{
			if (this.wobbleActive)
			{
				this.wobble += Engine.DeltaTime * 4f;
				this.sprite.Y = (float)Math.Sin((double)this.wobble);
			}
			base.Update();
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x000A4B7C File Offset: 0x000A2D7C
		private void OnPlayer(Player player)
		{
			base.SceneAs<Level>().Particles.Emit(Key.P_Collect, 10, this.Position, Vector2.One * 3f);
			Audio.Play("event:/game/general/key_get", this.Position);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			player.Leader.GainFollower(this.follower);
			this.Collidable = false;
			Session session = base.SceneAs<Level>().Session;
			session.DoNotLoad.Add(this.ID);
			session.Keys.Add(this.ID);
			session.UpdateLevelStartDashes();
			this.wiggler.Start();
			base.Depth = -1000000;
			if (this.nodes != null && this.nodes.Length >= 2)
			{
				base.Add(new Coroutine(this.NodeRoutine(player), true));
			}
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x000A4C55 File Offset: 0x000A2E55
		private IEnumerator NodeRoutine(Player player)
		{
			yield return 0.3f;
			if (!player.Dead)
			{
				Audio.Play("event:/game/general/cassette_bubblereturn", base.SceneAs<Level>().Camera.Position + new Vector2(160f, 90f));
				player.StartCassetteFly(this.nodes[1], this.nodes[0]);
			}
			yield break;
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x000A4C6C File Offset: 0x000A2E6C
		public void RegisterUsed()
		{
			this.IsUsed = true;
			if (this.follower.Leader != null)
			{
				this.follower.Leader.LoseFollower(this.follower);
			}
			base.SceneAs<Level>().Session.Keys.Remove(this.ID);
		}

		// Token: 0x06001996 RID: 6550 RVA: 0x000A4CBF File Offset: 0x000A2EBF
		public IEnumerator UseRoutine(Vector2 target)
		{
			this.Turning = true;
			this.follower.MoveTowardsLeader = false;
			this.wiggler.Start();
			this.wobbleActive = false;
			this.sprite.Y = 0f;
			Vector2 position = this.Position;
			SimpleCurve curve = new SimpleCurve(position, target, (target + position) / 2f + new Vector2(0f, -48f));
			this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 1f, true);
			this.tween.OnUpdate = delegate(Tween t)
			{
				this.Position = curve.GetPoint(t.Eased);
				this.sprite.Rate = 1f + t.Eased * 2f;
			};
			base.Add(this.tween);
			yield return this.tween.Wait();
			this.tween = null;
			while (this.sprite.CurrentAnimationFrame != 4)
			{
				yield return null;
			}
			this.shimmerParticles.Active = false;
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			for (int i = 0; i < 16; i++)
			{
				base.SceneAs<Level>().ParticlesFG.Emit(Key.P_Insert, base.Center, 0.3926991f * (float)i);
			}
			this.sprite.Play("enter", false, false);
			yield return 0.3f;
			this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, 0.3f, true);
			this.tween.OnUpdate = delegate(Tween t)
			{
				this.sprite.Rotation = t.Eased * 1.5707964f;
			};
			base.Add(this.tween);
			yield return this.tween.Wait();
			this.tween = null;
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			Action<Tween> <>9__3;
			Action<Tween> <>9__4;
			this.alarm = Alarm.Set(this, 1f, delegate
			{
				this.alarm = null;
				this.tween = Tween.Create(Tween.TweenMode.Oneshot, null, 1f, true);
				Tween tween = this.tween;
				Action<Tween> onUpdate;
				if ((onUpdate = <>9__3) == null)
				{
					onUpdate = (<>9__3 = delegate(Tween t)
					{
						this.light.Alpha = 1f - t.Eased;
					});
				}
				tween.OnUpdate = onUpdate;
				Tween tween2 = this.tween;
				Action<Tween> onComplete;
				if ((onComplete = <>9__4) == null)
				{
					onComplete = (<>9__4 = delegate(Tween t)
					{
						this.RemoveSelf();
					});
				}
				tween2.OnComplete = onComplete;
				this.Add(this.tween);
			}, Alarm.AlarmMode.Oneshot);
			yield return 0.2f;
			for (int j = 0; j < 8; j++)
			{
				base.SceneAs<Level>().ParticlesFG.Emit(Key.P_Insert, base.Center, 0.7853982f * (float)j);
			}
			this.sprite.Visible = false;
			this.Turning = false;
			yield break;
		}

		// Token: 0x04001645 RID: 5701
		public static ParticleType P_Shimmer;

		// Token: 0x04001646 RID: 5702
		public static ParticleType P_Insert;

		// Token: 0x04001647 RID: 5703
		public static ParticleType P_Collect;

		// Token: 0x04001648 RID: 5704
		public EntityID ID;

		// Token: 0x04001649 RID: 5705
		public bool IsUsed;

		// Token: 0x0400164A RID: 5706
		public bool StartedUsing;

		// Token: 0x0400164C RID: 5708
		private Follower follower;

		// Token: 0x0400164D RID: 5709
		private Sprite sprite;

		// Token: 0x0400164E RID: 5710
		private Wiggler wiggler;

		// Token: 0x0400164F RID: 5711
		private VertexLight light;

		// Token: 0x04001650 RID: 5712
		private ParticleEmitter shimmerParticles;

		// Token: 0x04001651 RID: 5713
		private float wobble;

		// Token: 0x04001652 RID: 5714
		private bool wobbleActive;

		// Token: 0x04001653 RID: 5715
		private Tween tween;

		// Token: 0x04001654 RID: 5716
		private Alarm alarm;

		// Token: 0x04001655 RID: 5717
		private Vector2[] nodes;
	}
}
