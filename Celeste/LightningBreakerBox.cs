using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200014F RID: 335
	public class LightningBreakerBox : Solid
	{
		// Token: 0x06000C2F RID: 3119 RVA: 0x0002725C File Offset: 0x0002545C
		public LightningBreakerBox(Vector2 position, bool flipX) : base(position, 32f, 32f, true)
		{
			this.SurfaceSoundIndex = 9;
			this.start = this.Position;
			this.sprite = GFX.SpriteBank.Create("breakerBox");
			Sprite sprite = this.sprite;
			sprite.OnLastFrame = (Action<string>)Delegate.Combine(sprite.OnLastFrame, new Action<string>(delegate(string anim)
			{
				if (anim == "break")
				{
					this.Visible = false;
					return;
				}
				if (anim == "open")
				{
					this.makeSparks = true;
				}
			}));
			this.sprite.Position = new Vector2(base.Width, base.Height) / 2f;
			this.sprite.FlipX = flipX;
			base.Add(this.sprite);
			this.sine = new SineWave(0.5f, 0f);
			base.Add(this.sine);
			this.bounce = Wiggler.Create(1f, 0.5f, null, false, false);
			this.bounce.StartZero = false;
			base.Add(this.bounce);
			base.Add(this.shaker = new Shaker(false, null));
			this.OnDashCollide = new DashCollision(this.Dashed);
		}

		// Token: 0x06000C30 RID: 3120 RVA: 0x00027390 File Offset: 0x00025590
		public LightningBreakerBox(EntityData e, Vector2 levelOffset) : this(e.Position + levelOffset, e.Bool("flipX", false))
		{
			this.flag = e.Bool("flag", false);
			this.music = e.Attr("music", null);
			this.musicProgress = e.Int("music_progress", -1);
			this.musicStoreInSession = e.Bool("music_session", false);
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x00027404 File Offset: 0x00025604
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			this.spikesUp = base.CollideCheck<Spikes>(this.Position - Vector2.UnitY);
			this.spikesDown = base.CollideCheck<Spikes>(this.Position + Vector2.UnitY);
			this.spikesLeft = base.CollideCheck<Spikes>(this.Position - Vector2.UnitX);
			this.spikesRight = base.CollideCheck<Spikes>(this.Position + Vector2.UnitX);
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x00027488 File Offset: 0x00025688
		public DashCollisionResults Dashed(Player player, Vector2 dir)
		{
			if (!SaveData.Instance.Assists.Invincible)
			{
				if (dir == Vector2.UnitX && this.spikesLeft)
				{
					return DashCollisionResults.NormalCollision;
				}
				if (dir == -Vector2.UnitX && this.spikesRight)
				{
					return DashCollisionResults.NormalCollision;
				}
				if (dir == Vector2.UnitY && this.spikesUp)
				{
					return DashCollisionResults.NormalCollision;
				}
				if (dir == -Vector2.UnitY && this.spikesDown)
				{
					return DashCollisionResults.NormalCollision;
				}
			}
			(base.Scene as Level).DirectionalShake(dir, 0.3f);
			this.sprite.Scale = new Vector2(1f + Math.Abs(dir.Y) * 0.4f - Math.Abs(dir.X) * 0.4f, 1f + Math.Abs(dir.X) * 0.4f - Math.Abs(dir.Y) * 0.4f);
			this.health--;
			if (this.health > 0)
			{
				base.Add(this.firstHitSfx = new SoundSource("event:/new_content/game/10_farewell/fusebox_hit_1"));
				Celeste.Freeze(0.1f);
				this.shakeCounter = 0.2f;
				this.shaker.On = true;
				this.bounceDir = dir;
				this.bounce.Start();
				this.smashParticles = true;
				this.Pulse();
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			}
			else
			{
				if (this.firstHitSfx != null)
				{
					this.firstHitSfx.Stop(true);
				}
				Audio.Play("event:/new_content/game/10_farewell/fusebox_hit_2", this.Position);
				Celeste.Freeze(0.2f);
				player.RefillDash();
				this.Break();
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
				this.SmashParticles(dir.Perpendicular());
				this.SmashParticles(-dir.Perpendicular());
			}
			return DashCollisionResults.Rebound;
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x00027660 File Offset: 0x00025860
		private void SmashParticles(Vector2 dir)
		{
			float direction;
			Vector2 position;
			Vector2 positionRange;
			int num;
			if (dir == Vector2.UnitX)
			{
				direction = 0f;
				position = base.CenterRight - Vector2.UnitX * 12f;
				positionRange = Vector2.UnitY * (base.Height - 6f) * 0.5f;
				num = (int)(base.Height / 8f) * 4;
			}
			else if (dir == -Vector2.UnitX)
			{
				direction = 3.1415927f;
				position = base.CenterLeft + Vector2.UnitX * 12f;
				positionRange = Vector2.UnitY * (base.Height - 6f) * 0.5f;
				num = (int)(base.Height / 8f) * 4;
			}
			else if (dir == Vector2.UnitY)
			{
				direction = 1.5707964f;
				position = base.BottomCenter - Vector2.UnitY * 12f;
				positionRange = Vector2.UnitX * (base.Width - 6f) * 0.5f;
				num = (int)(base.Width / 8f) * 4;
			}
			else
			{
				direction = -1.5707964f;
				position = base.TopCenter + Vector2.UnitY * 12f;
				positionRange = Vector2.UnitX * (base.Width - 6f) * 0.5f;
				num = (int)(base.Width / 8f) * 4;
			}
			num += 2;
			base.SceneAs<Level>().Particles.Emit(LightningBreakerBox.P_Smash, num, position, positionRange, direction);
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x0002780A File Offset: 0x00025A0A
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (this.flag && (base.Scene as Level).Session.GetFlag("disable_lightning"))
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x00027840 File Offset: 0x00025A40
		public override void Update()
		{
			base.Update();
			if (this.makeSparks && base.Scene.OnInterval(0.03f))
			{
				base.SceneAs<Level>().ParticlesFG.Emit(LightningBreakerBox.P_Sparks, 1, base.Center, Vector2.One * 12f);
			}
			if (this.shakeCounter > 0f)
			{
				this.shakeCounter -= Engine.DeltaTime;
				if (this.shakeCounter <= 0f)
				{
					this.shaker.On = false;
					this.sprite.Scale = Vector2.One * 1.2f;
					this.sprite.Play("open", false, false);
				}
			}
			if (this.Collidable)
			{
				bool flag = base.HasPlayerRider();
				this.sink = Calc.Approach(this.sink, (float)(flag ? 1 : 0), 2f * Engine.DeltaTime);
				this.sine.Rate = MathHelper.Lerp(1f, 0.5f, this.sink);
				Vector2 vector = this.start;
				vector.Y += this.sink * 6f + this.sine.Value * MathHelper.Lerp(4f, 2f, this.sink);
				vector += this.bounce.Value * this.bounceDir * 12f;
				base.MoveToX(vector.X);
				base.MoveToY(vector.Y);
				if (this.smashParticles)
				{
					this.smashParticles = false;
					this.SmashParticles(this.bounceDir.Perpendicular());
					this.SmashParticles(-this.bounceDir.Perpendicular());
				}
			}
			this.sprite.Scale.X = Calc.Approach(this.sprite.Scale.X, 1f, Engine.DeltaTime * 4f);
			this.sprite.Scale.Y = Calc.Approach(this.sprite.Scale.Y, 1f, Engine.DeltaTime * 4f);
			this.LiftSpeed = Vector2.Zero;
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x00027A7C File Offset: 0x00025C7C
		public override void Render()
		{
			Vector2 position = this.sprite.Position;
			this.sprite.Position += this.shaker.Value;
			base.Render();
			this.sprite.Position = position;
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x00027AC8 File Offset: 0x00025CC8
		private void Pulse()
		{
			this.pulseRoutine = new Coroutine(Lightning.PulseRoutine(base.SceneAs<Level>()), true);
			base.Add(this.pulseRoutine);
		}

		// Token: 0x06000C38 RID: 3128 RVA: 0x00027AF0 File Offset: 0x00025CF0
		private void Break()
		{
			Session session = (base.Scene as Level).Session;
			RumbleTrigger.ManuallyTrigger(base.Center.X, 1.2f);
			base.Tag = Tags.Persistent;
			this.shakeCounter = 0f;
			this.shaker.On = false;
			this.sprite.Play("break", false, false);
			this.Collidable = false;
			base.DestroyStaticMovers();
			if (this.flag)
			{
				session.SetFlag("disable_lightning", true);
			}
			if (this.musicStoreInSession)
			{
				if (!string.IsNullOrEmpty(this.music))
				{
					session.Audio.Music.Event = SFX.EventnameByHandle(this.music);
				}
				if (this.musicProgress >= 0)
				{
					session.Audio.Music.SetProgress(this.musicProgress);
				}
				session.Audio.Apply(false);
			}
			else
			{
				if (!string.IsNullOrEmpty(this.music))
				{
					Audio.SetMusic(SFX.EventnameByHandle(this.music), false, true);
				}
				if (this.musicProgress >= 0)
				{
					Audio.SetMusicParam("progress", (float)this.musicProgress);
				}
				if (!string.IsNullOrEmpty(this.music) && Audio.CurrentMusicEventInstance != null)
				{
					Audio.CurrentMusicEventInstance.start();
				}
			}
			if (this.pulseRoutine != null)
			{
				this.pulseRoutine.Active = false;
			}
			base.Add(new Coroutine(Lightning.RemoveRoutine(base.SceneAs<Level>(), new Action(base.RemoveSelf)), true));
		}

		// Token: 0x04000781 RID: 1921
		public static ParticleType P_Smash;

		// Token: 0x04000782 RID: 1922
		public static ParticleType P_Sparks;

		// Token: 0x04000783 RID: 1923
		private Sprite sprite;

		// Token: 0x04000784 RID: 1924
		private SineWave sine;

		// Token: 0x04000785 RID: 1925
		private Vector2 start;

		// Token: 0x04000786 RID: 1926
		private float sink;

		// Token: 0x04000787 RID: 1927
		private int health = 2;

		// Token: 0x04000788 RID: 1928
		private bool flag;

		// Token: 0x04000789 RID: 1929
		private float shakeCounter;

		// Token: 0x0400078A RID: 1930
		private string music;

		// Token: 0x0400078B RID: 1931
		private int musicProgress = -1;

		// Token: 0x0400078C RID: 1932
		private bool musicStoreInSession;

		// Token: 0x0400078D RID: 1933
		private Vector2 bounceDir;

		// Token: 0x0400078E RID: 1934
		private Wiggler bounce;

		// Token: 0x0400078F RID: 1935
		private Shaker shaker;

		// Token: 0x04000790 RID: 1936
		private bool makeSparks;

		// Token: 0x04000791 RID: 1937
		private bool smashParticles;

		// Token: 0x04000792 RID: 1938
		private Coroutine pulseRoutine;

		// Token: 0x04000793 RID: 1939
		private SoundSource firstHitSfx;

		// Token: 0x04000794 RID: 1940
		private bool spikesLeft;

		// Token: 0x04000795 RID: 1941
		private bool spikesRight;

		// Token: 0x04000796 RID: 1942
		private bool spikesUp;

		// Token: 0x04000797 RID: 1943
		private bool spikesDown;
	}
}
