using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000332 RID: 818
	[Tracked(false)]
	public class StrawberrySeed : Entity
	{
		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x060019A3 RID: 6563 RVA: 0x000A5151 File Offset: 0x000A3351
		public bool Collected
		{
			get
			{
				return this.follower.HasLeader || this.finished;
			}
		}

		// Token: 0x060019A4 RID: 6564 RVA: 0x000A5168 File Offset: 0x000A3368
		public StrawberrySeed(Strawberry strawberry, Vector2 position, int index, bool ghost) : base(position)
		{
			this.Strawberry = strawberry;
			base.Depth = -100;
			this.start = this.Position;
			base.Collider = new Hitbox(12f, 12f, -6f, -6f);
			this.index = index;
			this.ghost = ghost;
			base.Add(this.follower = new Follower(new Action(this.OnGainLeader), new Action(this.OnLoseLeader)));
			this.follower.FollowDelay = 0.2f;
			this.follower.PersistentFollow = false;
			base.Add(new StaticMover
			{
				SolidChecker = ((Solid s) => s.CollideCheck(this)),
				OnAttach = delegate(Platform p)
				{
					base.Depth = -1000000;
					base.Collider = new Hitbox(24f, 24f, -12f, -12f);
					this.attached = p;
					this.start = this.Position - p.Position;
				}
			});
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(this.wiggler = Wiggler.Create(0.5f, 4f, delegate(float v)
			{
				this.sprite.Scale = Vector2.One * (1f + 0.2f * v);
			}, false, false));
			base.Add(this.sine = new SineWave(0.5f, 0f).Randomize());
			base.Add(this.shaker = new Shaker(false, null));
			base.Add(this.bloom = new BloomPoint(1f, 12f));
			base.Add(this.light = new VertexLight(Color.White, 1f, 16, 24));
			base.Add(this.lightTween = this.light.CreatePulseTween());
		}

		// Token: 0x060019A5 RID: 6565 RVA: 0x000A5320 File Offset: 0x000A3520
		public override void Awake(Scene scene)
		{
			this.level = (scene as Level);
			base.Awake(scene);
			this.sprite = GFX.SpriteBank.Create(this.ghost ? "ghostberrySeed" : ((this.level.Session.Area.Mode == AreaMode.CSide) ? "goldberrySeed" : "strawberrySeed"));
			this.sprite.Position = new Vector2(this.sine.Value * 2f, this.sine.ValueOverTwo * 1f);
			base.Add(this.sprite);
			if (this.ghost)
			{
				this.sprite.Color = Color.White * 0.8f;
			}
			int num = base.Scene.Tracker.CountEntities<StrawberrySeed>();
			float num2 = 1f - (float)this.index / ((float)num + 1f);
			num2 = 0.25f + num2 * 0.75f;
			this.sprite.PlayOffset("idle", num2, false);
			this.sprite.OnFrameChange = delegate(string s)
			{
				if (this.Visible && this.sprite.CurrentAnimationID == "idle" && this.sprite.CurrentAnimationFrame == 19)
				{
					Audio.Play("event:/game/general/seed_pulse", this.Position, "count", (float)this.index);
					this.lightTween.Start();
					this.level.Displacement.AddBurst(this.Position, 0.6f, 8f, 20f, 0.2f, null, null);
				}
			};
			StrawberrySeed.P_Burst.Color = this.sprite.Color;
		}

		// Token: 0x060019A6 RID: 6566 RVA: 0x000A5458 File Offset: 0x000A3658
		public override void Update()
		{
			base.Update();
			if (!this.finished)
			{
				if (this.canLoseTimer > 0f)
				{
					this.canLoseTimer -= Engine.DeltaTime;
				}
				else if (this.follower.HasLeader && this.player.LoseShards)
				{
					this.losing = true;
				}
				if (this.losing)
				{
					if (this.loseTimer <= 0f || this.player.Speed.Y < 0f)
					{
						this.player.Leader.LoseFollower(this.follower);
						this.losing = false;
					}
					else if (this.player.LoseShards)
					{
						this.loseTimer -= Engine.DeltaTime;
					}
					else
					{
						this.loseTimer = 0.15f;
						this.losing = false;
					}
				}
				this.sprite.Position = new Vector2(this.sine.Value * 2f, this.sine.ValueOverTwo * 1f) + this.shaker.Value;
				return;
			}
			this.light.Alpha = Calc.Approach(this.light.Alpha, 0f, Engine.DeltaTime * 4f);
		}

		// Token: 0x060019A7 RID: 6567 RVA: 0x000A55A4 File Offset: 0x000A37A4
		private void OnPlayer(Player player)
		{
			Audio.Play("event:/game/general/seed_touch", this.Position, "count", (float)this.index);
			this.player = player;
			player.Leader.GainFollower(this.follower);
			this.Collidable = false;
			base.Depth = -1000000;
			bool flag = true;
			using (List<StrawberrySeed>.Enumerator enumerator = this.Strawberry.Seeds.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.follower.HasLeader)
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				base.Scene.Add(new CSGEN_StrawberrySeeds(this.Strawberry));
			}
		}

		// Token: 0x060019A8 RID: 6568 RVA: 0x000A5664 File Offset: 0x000A3864
		private void OnGainLeader()
		{
			this.wiggler.Start();
			this.canLoseTimer = 0.25f;
			this.loseTimer = 0.15f;
		}

		// Token: 0x060019A9 RID: 6569 RVA: 0x000A5687 File Offset: 0x000A3887
		private void OnLoseLeader()
		{
			if (!this.finished)
			{
				base.Add(new Coroutine(this.ReturnRoutine(), true));
			}
		}

		// Token: 0x060019AA RID: 6570 RVA: 0x000A56A3 File Offset: 0x000A38A3
		private IEnumerator ReturnRoutine()
		{
			Audio.Play("event:/game/general/seed_poof", this.Position);
			this.Collidable = false;
			this.sprite.Scale = Vector2.One * 2f;
			yield return 0.05f;
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			for (int i = 0; i < 6; i++)
			{
				float num = Calc.Random.NextFloat(6.2831855f);
				this.level.ParticlesFG.Emit(StrawberrySeed.P_Burst, 1, this.Position + Calc.AngleToVector(num, 4f), Vector2.Zero, num);
			}
			this.Visible = false;
			yield return 0.3f + (float)this.index * 0.1f;
			Audio.Play("event:/game/general/seed_reappear", this.Position, "count", (float)this.index);
			this.Position = this.start;
			if (this.attached != null)
			{
				this.Position += this.attached.Position;
			}
			this.shaker.ShakeFor(0.4f, false);
			this.sprite.Scale = Vector2.One;
			this.Visible = true;
			this.Collidable = true;
			this.level.Displacement.AddBurst(this.Position, 0.2f, 8f, 28f, 0.2f, null, null);
			yield break;
		}

		// Token: 0x060019AB RID: 6571 RVA: 0x000A56B4 File Offset: 0x000A38B4
		public void OnAllCollected()
		{
			this.finished = true;
			this.follower.Leader.LoseFollower(this.follower);
			base.Depth = -2000002;
			base.Tag = Tags.FrozenUpdate;
			this.wiggler.Start();
		}

		// Token: 0x060019AC RID: 6572 RVA: 0x000A5704 File Offset: 0x000A3904
		public void StartSpinAnimation(Vector2 averagePos, Vector2 centerPos, float angleOffset, float time)
		{
			float spinLerp = 0f;
			Vector2 start = this.Position;
			this.sprite.Play("noFlash", false, false);
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, time / 2f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				spinLerp = t.Eased;
			};
			base.Add(tween);
			tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, time, true);
			tween.OnUpdate = delegate(Tween t)
			{
				float angleRadians = 1.5707964f + angleOffset - MathHelper.Lerp(0f, 32.201324f, t.Eased);
				Vector2 value = Vector2.Lerp(averagePos, centerPos, spinLerp) + Calc.AngleToVector(angleRadians, 25f);
				this.Position = Vector2.Lerp(start, value, spinLerp);
			};
			base.Add(tween);
		}

		// Token: 0x060019AD RID: 6573 RVA: 0x000A57B4 File Offset: 0x000A39B4
		public void StartCombineAnimation(Vector2 centerPos, float time, ParticleSystem particleSystem)
		{
			Vector2 position = this.Position;
			float startAngle = Calc.Angle(centerPos, position);
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.BigBackIn, time, true);
			tween.OnUpdate = delegate(Tween t)
			{
				float angleRadians = MathHelper.Lerp(startAngle, startAngle - 6.2831855f, Ease.CubeIn(t.Percent));
				float length = MathHelper.Lerp(25f, 0f, t.Eased);
				this.Position = centerPos + Calc.AngleToVector(angleRadians, length);
			};
			tween.OnComplete = delegate(Tween t)
			{
				this.Visible = false;
				for (int i = 0; i < 6; i++)
				{
					float num = Calc.Random.NextFloat(6.2831855f);
					particleSystem.Emit(StrawberrySeed.P_Burst, 1, this.Position + Calc.AngleToVector(num, 4f), Vector2.Zero, num);
				}
				this.RemoveSelf();
			};
			base.Add(tween);
		}

		// Token: 0x0400165D RID: 5725
		public static ParticleType P_Burst;

		// Token: 0x0400165E RID: 5726
		private const float LoseDelay = 0.25f;

		// Token: 0x0400165F RID: 5727
		private const float LoseGraceTime = 0.15f;

		// Token: 0x04001660 RID: 5728
		public Strawberry Strawberry;

		// Token: 0x04001661 RID: 5729
		private Sprite sprite;

		// Token: 0x04001662 RID: 5730
		private Follower follower;

		// Token: 0x04001663 RID: 5731
		private Wiggler wiggler;

		// Token: 0x04001664 RID: 5732
		private Platform attached;

		// Token: 0x04001665 RID: 5733
		private SineWave sine;

		// Token: 0x04001666 RID: 5734
		private Tween lightTween;

		// Token: 0x04001667 RID: 5735
		private VertexLight light;

		// Token: 0x04001668 RID: 5736
		private BloomPoint bloom;

		// Token: 0x04001669 RID: 5737
		private Shaker shaker;

		// Token: 0x0400166A RID: 5738
		private int index;

		// Token: 0x0400166B RID: 5739
		private Vector2 start;

		// Token: 0x0400166C RID: 5740
		private Player player;

		// Token: 0x0400166D RID: 5741
		private Level level;

		// Token: 0x0400166E RID: 5742
		private float canLoseTimer;

		// Token: 0x0400166F RID: 5743
		private float loseTimer;

		// Token: 0x04001670 RID: 5744
		private bool finished;

		// Token: 0x04001671 RID: 5745
		private bool losing;

		// Token: 0x04001672 RID: 5746
		private bool ghost;
	}
}
