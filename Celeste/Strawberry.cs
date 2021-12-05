using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000355 RID: 853
	public class Strawberry : Entity
	{
		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06001AC8 RID: 6856 RVA: 0x000AD6AA File Offset: 0x000AB8AA
		// (set) Token: 0x06001AC9 RID: 6857 RVA: 0x000AD6B2 File Offset: 0x000AB8B2
		public bool Winged { get; private set; }

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06001ACA RID: 6858 RVA: 0x000AD6BB File Offset: 0x000AB8BB
		// (set) Token: 0x06001ACB RID: 6859 RVA: 0x000AD6C3 File Offset: 0x000AB8C3
		public bool Golden { get; private set; }

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06001ACC RID: 6860 RVA: 0x000AD6CC File Offset: 0x000AB8CC
		// (set) Token: 0x06001ACD RID: 6861 RVA: 0x000AD6D4 File Offset: 0x000AB8D4
		public bool Moon { get; private set; }

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06001ACE RID: 6862 RVA: 0x000AD6DD File Offset: 0x000AB8DD
		private string gotSeedFlag
		{
			get
			{
				return "collected_seeds_of_" + this.ID.ToString();
			}
		}

		// Token: 0x06001ACF RID: 6863 RVA: 0x000AD6FC File Offset: 0x000AB8FC
		public Strawberry(EntityData data, Vector2 offset, EntityID gid)
		{
			this.ID = gid;
			this.Position = (this.start = data.Position + offset);
			this.Winged = (data.Bool("winged", false) || data.Name == "memorialTextController");
			this.Golden = (data.Name == "memorialTextController" || data.Name == "goldenBerry");
			this.Moon = data.Bool("moon", false);
			this.isGhostBerry = SaveData.Instance.CheckStrawberry(this.ID);
			base.Depth = -100;
			base.Collider = new Hitbox(14f, 14f, -7f, -7f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(new MirrorReflection());
			base.Add(this.Follower = new Follower(this.ID, null, new Action(this.OnLoseLeader)));
			this.Follower.FollowDelay = 0.3f;
			if (this.Winged)
			{
				base.Add(new DashListener
				{
					OnDash = new Action<Vector2>(this.OnDash)
				});
			}
			if (data.Nodes != null && data.Nodes.Length != 0)
			{
				this.Seeds = new List<StrawberrySeed>();
				for (int i = 0; i < data.Nodes.Length; i++)
				{
					this.Seeds.Add(new StrawberrySeed(this, offset + data.Nodes[i], i, this.isGhostBerry));
				}
			}
		}

		// Token: 0x06001AD0 RID: 6864 RVA: 0x000AD8B4 File Offset: 0x000ABAB4
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (SaveData.Instance.CheckStrawberry(this.ID))
			{
				if (this.Moon)
				{
					this.sprite = GFX.SpriteBank.Create("moonghostberry");
				}
				else if (this.Golden)
				{
					this.sprite = GFX.SpriteBank.Create("goldghostberry");
				}
				else
				{
					this.sprite = GFX.SpriteBank.Create("ghostberry");
				}
				this.sprite.Color = Color.White * 0.8f;
			}
			else if (this.Moon)
			{
				this.sprite = GFX.SpriteBank.Create("moonberry");
			}
			else if (this.Golden)
			{
				this.sprite = GFX.SpriteBank.Create("goldberry");
			}
			else
			{
				this.sprite = GFX.SpriteBank.Create("strawberry");
			}
			base.Add(this.sprite);
			if (this.Winged)
			{
				this.sprite.Play("flap", false, false);
			}
			this.sprite.OnFrameChange = new Action<string>(this.OnAnimate);
			base.Add(this.wiggler = Wiggler.Create(0.4f, 4f, delegate(float v)
			{
				this.sprite.Scale = Vector2.One * (1f + v * 0.35f);
			}, false, false));
			base.Add(this.rotateWiggler = Wiggler.Create(0.5f, 4f, delegate(float v)
			{
				this.sprite.Rotation = v * 30f * 0.017453292f;
			}, false, false));
			base.Add(this.bloom = new BloomPoint((this.Golden || this.Moon || this.isGhostBerry) ? 0.5f : 1f, 12f));
			base.Add(this.light = new VertexLight(Color.White, 1f, 16, 24));
			base.Add(this.lightTween = this.light.CreatePulseTween());
			if (this.Seeds != null && this.Seeds.Count > 0 && !(scene as Level).Session.GetFlag(this.gotSeedFlag))
			{
				foreach (StrawberrySeed entity in this.Seeds)
				{
					scene.Add(entity);
				}
				this.Visible = false;
				this.Collidable = false;
				this.WaitingOnSeeds = true;
				this.bloom.Visible = (this.light.Visible = false);
			}
			if ((scene as Level).Session.BloomBaseAdd > 0.1f)
			{
				this.bloom.Alpha *= 0.5f;
			}
		}

		// Token: 0x06001AD1 RID: 6865 RVA: 0x000ADB88 File Offset: 0x000ABD88
		public override void Update()
		{
			if (this.WaitingOnSeeds)
			{
				return;
			}
			if (!this.collected)
			{
				if (!this.Winged)
				{
					this.wobble += Engine.DeltaTime * 4f;
					this.sprite.Y = (this.bloom.Y = (this.light.Y = (float)Math.Sin((double)this.wobble) * 2f));
				}
				int followIndex = this.Follower.FollowIndex;
				if (this.Follower.Leader != null && this.Follower.DelayTimer <= 0f && this.IsFirstStrawberry)
				{
					Player player = this.Follower.Leader.Entity as Player;
					bool flag = false;
					if (player != null && player.Scene != null && !player.StrawberriesBlocked)
					{
						if (this.Golden)
						{
							if (player.CollideCheck<GoldBerryCollectTrigger>() || (base.Scene as Level).Completed)
							{
								flag = true;
							}
						}
						else if (player.OnSafeGround && (!this.Moon || player.StateMachine.State != 13))
						{
							flag = true;
						}
					}
					if (flag)
					{
						this.collectTimer += Engine.DeltaTime;
						if (this.collectTimer > 0.15f)
						{
							this.OnCollect();
						}
					}
					else
					{
						this.collectTimer = Math.Min(this.collectTimer, 0f);
					}
				}
				else
				{
					if (followIndex > 0)
					{
						this.collectTimer = -0.15f;
					}
					if (this.Winged)
					{
						base.Y += this.flapSpeed * Engine.DeltaTime;
						if (this.flyingAway)
						{
							if (base.Y < (float)(base.SceneAs<Level>().Bounds.Top - 16))
							{
								base.RemoveSelf();
							}
						}
						else
						{
							this.flapSpeed = Calc.Approach(this.flapSpeed, 20f, 170f * Engine.DeltaTime);
							if (base.Y < this.start.Y - 5f)
							{
								base.Y = this.start.Y - 5f;
							}
							else if (base.Y > this.start.Y + 5f)
							{
								base.Y = this.start.Y + 5f;
							}
						}
					}
				}
			}
			base.Update();
			if (this.Follower.Leader != null && base.Scene.OnInterval(0.08f))
			{
				ParticleType type;
				if (this.isGhostBerry)
				{
					type = Strawberry.P_GhostGlow;
				}
				else if (this.Golden)
				{
					type = Strawberry.P_GoldGlow;
				}
				else if (this.Moon)
				{
					type = Strawberry.P_MoonGlow;
				}
				else
				{
					type = Strawberry.P_Glow;
				}
				base.SceneAs<Level>().ParticlesFG.Emit(type, this.Position + Calc.Random.Range(-Vector2.One * 6f, Vector2.One * 6f));
			}
		}

		// Token: 0x06001AD2 RID: 6866 RVA: 0x000ADE9B File Offset: 0x000AC09B
		private void OnDash(Vector2 dir)
		{
			if (!this.flyingAway && this.Winged && !this.WaitingOnSeeds)
			{
				base.Depth = -1000000;
				base.Add(new Coroutine(this.FlyAwayRoutine(), true));
				this.flyingAway = true;
			}
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06001AD3 RID: 6867 RVA: 0x000ADEDC File Offset: 0x000AC0DC
		private bool IsFirstStrawberry
		{
			get
			{
				for (int i = this.Follower.FollowIndex - 1; i >= 0; i--)
				{
					Strawberry strawberry = this.Follower.Leader.Followers[i].Entity as Strawberry;
					if (strawberry != null && !strawberry.Golden)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x06001AD4 RID: 6868 RVA: 0x000ADF30 File Offset: 0x000AC130
		private void OnAnimate(string id)
		{
			if (!this.flyingAway && id == "flap" && this.sprite.CurrentAnimationFrame % 9 == 4)
			{
				Audio.Play("event:/game/general/strawberry_wingflap", this.Position);
				this.flapSpeed = -50f;
			}
			int num;
			if (id == "flap")
			{
				num = 25;
			}
			else if (this.Golden)
			{
				num = 30;
			}
			else if (this.Moon)
			{
				num = 30;
			}
			else
			{
				num = 35;
			}
			if (this.sprite.CurrentAnimationFrame == num)
			{
				this.lightTween.Start();
				if (!this.collected && (base.CollideCheck<FakeWall>() || base.CollideCheck<Solid>()))
				{
					Audio.Play("event:/game/general/strawberry_pulse", this.Position);
					base.SceneAs<Level>().Displacement.AddBurst(this.Position, 0.6f, 4f, 28f, 0.1f, null, null);
					return;
				}
				Audio.Play("event:/game/general/strawberry_pulse", this.Position);
				base.SceneAs<Level>().Displacement.AddBurst(this.Position, 0.6f, 4f, 28f, 0.2f, null, null);
			}
		}

		// Token: 0x06001AD5 RID: 6869 RVA: 0x000AE060 File Offset: 0x000AC260
		public void OnPlayer(Player player)
		{
			if (this.Follower.Leader == null && !this.collected && !this.WaitingOnSeeds)
			{
				this.ReturnHomeWhenLost = true;
				if (this.Winged)
				{
					Level level = base.SceneAs<Level>();
					this.Winged = false;
					this.sprite.Rate = 0f;
					Alarm.Set(this, this.Follower.FollowDelay, delegate
					{
						this.sprite.Rate = 1f;
						this.sprite.Play("idle", false, false);
						level.Particles.Emit(Strawberry.P_WingsBurst, 8, this.Position + new Vector2(8f, 0f), new Vector2(4f, 2f));
						level.Particles.Emit(Strawberry.P_WingsBurst, 8, this.Position - new Vector2(8f, 0f), new Vector2(4f, 2f));
					}, Alarm.AlarmMode.Oneshot);
				}
				if (this.Golden)
				{
					(base.Scene as Level).Session.GrabbedGolden = true;
				}
				Audio.Play(this.isGhostBerry ? "event:/game/general/strawberry_blue_touch" : "event:/game/general/strawberry_touch", this.Position);
				player.Leader.GainFollower(this.Follower);
				this.wiggler.Start();
				base.Depth = -1000000;
			}
		}

		// Token: 0x06001AD6 RID: 6870 RVA: 0x000AE158 File Offset: 0x000AC358
		public void OnCollect()
		{
			if (this.collected)
			{
				return;
			}
			int collectIndex = 0;
			this.collected = true;
			if (this.Follower.Leader != null)
			{
				Player player = this.Follower.Leader.Entity as Player;
				collectIndex = player.StrawberryCollectIndex;
				player.StrawberryCollectIndex++;
				player.StrawberryCollectResetTimer = 2.5f;
				this.Follower.Leader.LoseFollower(this.Follower);
			}
			if (this.Moon)
			{
				Achievements.Register(Achievement.WOW);
			}
			SaveData.Instance.AddStrawberry(this.ID, this.Golden);
			Session session = (base.Scene as Level).Session;
			session.DoNotLoad.Add(this.ID);
			session.Strawberries.Add(this.ID);
			session.UpdateLevelStartDashes();
			base.Add(new Coroutine(this.CollectRoutine(collectIndex), true));
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x000AE23F File Offset: 0x000AC43F
		private IEnumerator FlyAwayRoutine()
		{
			this.rotateWiggler.Start();
			this.flapSpeed = -200f;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.25f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				this.flapSpeed = MathHelper.Lerp(-200f, 0f, t.Eased);
			};
			base.Add(tween);
			yield return 0.1f;
			Audio.Play("event:/game/general/strawberry_laugh", this.Position);
			yield return 0.2f;
			if (!this.Follower.HasLeader)
			{
				Audio.Play("event:/game/general/strawberry_flyaway", this.Position);
			}
			tween = Tween.Create(Tween.TweenMode.Oneshot, null, 0.5f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				this.flapSpeed = MathHelper.Lerp(0f, -200f, t.Eased);
			};
			base.Add(tween);
			yield break;
		}

		// Token: 0x06001AD8 RID: 6872 RVA: 0x000AE24E File Offset: 0x000AC44E
		private IEnumerator CollectRoutine(int collectIndex)
		{
			Scene scene = base.Scene;
			base.Tag = Tags.TransitionUpdate;
			base.Depth = -2000010;
			int num = 0;
			if (this.Moon)
			{
				num = 3;
			}
			else if (this.isGhostBerry)
			{
				num = 1;
			}
			else if (this.Golden)
			{
				num = 2;
			}
			Audio.Play("event:/game/general/strawberry_get", this.Position, "colour", (float)num, "count", (float)collectIndex);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			this.sprite.Play("collect", false, false);
			while (this.sprite.Animating)
			{
				yield return null;
			}
			base.Scene.Add(new StrawberryPoints(this.Position, this.isGhostBerry, collectIndex, this.Moon));
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x000AE264 File Offset: 0x000AC464
		private void OnLoseLeader()
		{
			if (!this.collected && this.ReturnHomeWhenLost)
			{
				Alarm.Set(this, 0.15f, delegate
				{
					Vector2 vector = (this.start - this.Position).SafeNormalize();
					float num = Vector2.Distance(this.Position, this.start);
					float scaleFactor = Calc.ClampedMap(num, 16f, 120f, 16f, 96f);
					Vector2 control = this.start + vector * 16f + vector.Perpendicular() * scaleFactor * (float)Calc.Random.Choose(1, -1);
					SimpleCurve curve = new SimpleCurve(this.Position, this.start, control);
					Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineOut, MathHelper.Max(num / 100f, 0.4f), true);
					tween.OnUpdate = delegate(Tween f)
					{
						this.Position = curve.GetPoint(f.Eased);
					};
					tween.OnComplete = delegate(Tween f)
					{
						base.Depth = 0;
					};
					base.Add(tween);
				}, Alarm.AlarmMode.Oneshot);
			}
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x000AE290 File Offset: 0x000AC490
		public void CollectedSeeds()
		{
			this.WaitingOnSeeds = false;
			this.Visible = true;
			this.Collidable = true;
			this.bloom.Visible = (this.light.Visible = true);
			(base.Scene as Level).Session.SetFlag(this.gotSeedFlag, true);
		}

		// Token: 0x04001768 RID: 5992
		public static ParticleType P_Glow;

		// Token: 0x04001769 RID: 5993
		public static ParticleType P_GhostGlow;

		// Token: 0x0400176A RID: 5994
		public static ParticleType P_GoldGlow;

		// Token: 0x0400176B RID: 5995
		public static ParticleType P_MoonGlow;

		// Token: 0x0400176C RID: 5996
		public static ParticleType P_WingsBurst;

		// Token: 0x0400176D RID: 5997
		public EntityID ID;

		// Token: 0x0400176E RID: 5998
		private Sprite sprite;

		// Token: 0x0400176F RID: 5999
		public Follower Follower;

		// Token: 0x04001770 RID: 6000
		private Wiggler wiggler;

		// Token: 0x04001771 RID: 6001
		private Wiggler rotateWiggler;

		// Token: 0x04001772 RID: 6002
		private BloomPoint bloom;

		// Token: 0x04001773 RID: 6003
		private VertexLight light;

		// Token: 0x04001774 RID: 6004
		private Tween lightTween;

		// Token: 0x04001775 RID: 6005
		private float wobble;

		// Token: 0x04001776 RID: 6006
		private Vector2 start;

		// Token: 0x04001777 RID: 6007
		private float collectTimer;

		// Token: 0x04001778 RID: 6008
		private bool collected;

		// Token: 0x04001779 RID: 6009
		private bool isGhostBerry;

		// Token: 0x0400177A RID: 6010
		private bool flyingAway;

		// Token: 0x0400177B RID: 6011
		private float flapSpeed;

		// Token: 0x0400177F RID: 6015
		public bool ReturnHomeWhenLost = true;

		// Token: 0x04001780 RID: 6016
		public bool WaitingOnSeeds;

		// Token: 0x04001781 RID: 6017
		public List<StrawberrySeed> Seeds;
	}
}
