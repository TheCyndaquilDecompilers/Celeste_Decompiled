using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200028C RID: 652
	[Tracked(false)]
	public class BadelineOldsite : Entity
	{
		// Token: 0x06001440 RID: 5184 RVA: 0x0006D5C8 File Offset: 0x0006B7C8
		public BadelineOldsite(Vector2 position, int index) : base(position)
		{
			this.index = index;
			base.Depth = -1;
			base.Collider = new Hitbox(6f, 6f, -3f, -7f);
			this.Collidable = false;
			this.Sprite = new PlayerSprite(PlayerSpriteMode.Badeline);
			this.Sprite.Play("fallSlow", true, false);
			this.Hair = new PlayerHair(this.Sprite);
			this.Hair.Color = Color.Lerp(BadelineOldsite.HairColor, Color.White, (float)index / 6f);
			this.Hair.Border = Color.Black;
			base.Add(this.Hair);
			base.Add(this.Sprite);
			this.Visible = false;
			this.followBehindTime = 1.55f;
			this.followBehindIndexDelay = 0.4f * (float)index;
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
		}

		// Token: 0x06001441 RID: 5185 RVA: 0x0006D6D8 File Offset: 0x0006B8D8
		public BadelineOldsite(EntityData data, Vector2 offset, int index) : this(data.Position + offset, index)
		{
		}

		// Token: 0x06001442 RID: 5186 RVA: 0x0006D6F0 File Offset: 0x0006B8F0
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Session session = base.SceneAs<Level>().Session;
			if (session.GetLevelFlag("11") && session.Area.Mode == AreaMode.Normal)
			{
				base.RemoveSelf();
				return;
			}
			if (!session.GetLevelFlag("3") && session.Area.Mode == AreaMode.Normal)
			{
				base.RemoveSelf();
				return;
			}
			if (!session.GetFlag("evil_maddy_intro") && session.Level == "3" && session.Area.Mode == AreaMode.Normal)
			{
				this.Hovering = false;
				this.Visible = true;
				this.Hair.Visible = false;
				this.Sprite.Play("pretendDead", false, false);
				if (session.Area.Mode == AreaMode.Normal)
				{
					session.Audio.Music.Event = null;
					session.Audio.Apply(false);
				}
				base.Scene.Add(new CS02_BadelineIntro(this));
				return;
			}
			base.Add(new Coroutine(this.StartChasingRoutine(base.Scene as Level), true));
		}

		// Token: 0x06001443 RID: 5187 RVA: 0x0006D806 File Offset: 0x0006BA06
		public IEnumerator StartChasingRoutine(Level level)
		{
			this.Hovering = true;
			while ((this.player = base.Scene.Tracker.GetEntity<Player>()) == null || this.player.JustRespawned)
			{
				yield return null;
			}
			Vector2 to = this.player.Position;
			yield return this.followBehindIndexDelay;
			if (!this.Visible)
			{
				this.PopIntoExistance(0.5f);
			}
			this.Sprite.Play("fallSlow", false, false);
			this.Hair.Visible = true;
			this.Hovering = false;
			if (level.Session.Area.Mode == AreaMode.Normal)
			{
				level.Session.Audio.Music.Event = "event:/music/lvl2/chase";
				level.Session.Audio.Apply(false);
			}
			yield return this.TweenToPlayer(to);
			this.Collidable = true;
			this.following = true;
			base.Add(this.occlude = new LightOcclude(1f));
			if (level.Session.Level == "2")
			{
				base.Add(new Coroutine(this.StopChasing(), true));
			}
			yield break;
		}

		// Token: 0x06001444 RID: 5188 RVA: 0x0006D81C File Offset: 0x0006BA1C
		private IEnumerator TweenToPlayer(Vector2 to)
		{
			Audio.Play("event:/char/badeline/level_entry", this.Position, "chaser_count", (float)this.index);
			Vector2 from = this.Position;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, this.followBehindTime - 0.1f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				this.Position = Vector2.Lerp(from, to, t.Eased);
				if (to.X != from.X)
				{
					this.Sprite.Scale.X = Math.Abs(this.Sprite.Scale.X) * (float)Math.Sign(to.X - from.X);
				}
				this.Trail();
			};
			base.Add(tween);
			yield return tween.Duration;
			yield break;
		}

		// Token: 0x06001445 RID: 5189 RVA: 0x0006D832 File Offset: 0x0006BA32
		private IEnumerator StopChasing()
		{
			Level level = base.SceneAs<Level>();
			int boundsRight = level.Bounds.X + 148;
			int boundsBottom = level.Bounds.Y + 168 + 184;
			while (base.X != (float)boundsRight || base.Y != (float)boundsBottom)
			{
				yield return null;
				if (base.X > (float)boundsRight)
				{
					base.X = (float)boundsRight;
				}
				if (base.Y > (float)boundsBottom)
				{
					base.Y = (float)boundsBottom;
				}
			}
			this.following = false;
			this.ignorePlayerAnim = true;
			this.Sprite.Play("laugh", false, false);
			this.Sprite.Scale.X = 1f;
			yield return 1f;
			Audio.Play("event:/char/badeline/disappear", this.Position);
			level.Displacement.AddBurst(base.Center, 0.5f, 24f, 96f, 0.4f, null, null);
			level.Particles.Emit(BadelineOldsite.P_Vanish, 12, base.Center, Vector2.One * 6f);
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x06001446 RID: 5190 RVA: 0x0006D844 File Offset: 0x0006BA44
		public override void Update()
		{
			Player.ChaserState chaserState;
			if (this.player != null && this.player.Dead)
			{
				this.Sprite.Play("laugh", false, false);
				this.Sprite.X = (float)(Math.Sin((double)this.hoveringTimer) * 4.0);
				this.Hovering = true;
				this.hoveringTimer += Engine.DeltaTime * 2f;
				base.Depth = -12500;
				foreach (KeyValuePair<string, SoundSource> keyValuePair in this.loopingSounds)
				{
					keyValuePair.Value.Stop(true);
				}
				this.Trail();
			}
			else if (this.following && this.player.GetChasePosition(base.Scene.TimeActive, this.followBehindTime + this.followBehindIndexDelay, out chaserState))
			{
				this.Position = Calc.Approach(this.Position, chaserState.Position, 500f * Engine.DeltaTime);
				if (!this.ignorePlayerAnim && chaserState.Animation != this.Sprite.CurrentAnimationID && chaserState.Animation != null && this.Sprite.Has(chaserState.Animation))
				{
					this.Sprite.Play(chaserState.Animation, true, false);
				}
				if (!this.ignorePlayerAnim)
				{
					this.Sprite.Scale.X = Math.Abs(this.Sprite.Scale.X) * (float)chaserState.Facing;
				}
				for (int i = 0; i < chaserState.Sounds; i++)
				{
					if (chaserState[i].Action == Player.ChaserStateSound.Actions.Oneshot)
					{
						Audio.Play(chaserState[i].Event, this.Position, chaserState[i].Parameter, chaserState[i].ParameterValue, "chaser_count", (float)this.index);
					}
					else if (chaserState[i].Action == Player.ChaserStateSound.Actions.Loop && !this.loopingSounds.ContainsKey(chaserState[i].Event))
					{
						SoundSource soundSource;
						if (this.inactiveLoopingSounds.Count > 0)
						{
							soundSource = this.inactiveLoopingSounds[0];
							this.inactiveLoopingSounds.RemoveAt(0);
						}
						else
						{
							base.Add(soundSource = new SoundSource());
						}
						soundSource.Play(chaserState[i].Event, "chaser_count", (float)this.index);
						this.loopingSounds.Add(chaserState[i].Event, soundSource);
					}
					else if (chaserState[i].Action == Player.ChaserStateSound.Actions.Stop)
					{
						SoundSource soundSource2 = null;
						if (this.loopingSounds.TryGetValue(chaserState[i].Event, out soundSource2))
						{
							soundSource2.Stop(true);
							this.loopingSounds.Remove(chaserState[i].Event);
							this.inactiveLoopingSounds.Add(soundSource2);
						}
					}
				}
				base.Depth = chaserState.Depth;
				this.Trail();
			}
			if (this.Sprite.Scale.X != 0f)
			{
				this.Hair.Facing = (Facings)Math.Sign(this.Sprite.Scale.X);
			}
			if (this.Hovering)
			{
				this.hoveringTimer += Engine.DeltaTime;
				this.Sprite.Y = (float)(Math.Sin((double)(this.hoveringTimer * 2f)) * 4.0);
			}
			else
			{
				this.Sprite.Y = Calc.Approach(this.Sprite.Y, 0f, Engine.DeltaTime * 4f);
			}
			if (this.occlude != null)
			{
				this.occlude.Visible = !base.CollideCheck<Solid>();
			}
			base.Update();
		}

		// Token: 0x06001447 RID: 5191 RVA: 0x0006DC40 File Offset: 0x0006BE40
		private void Trail()
		{
			if (base.Scene.OnInterval(0.1f))
			{
				TrailManager.Add(this, Player.NormalHairColor, 1f, false, false);
			}
		}

		// Token: 0x06001448 RID: 5192 RVA: 0x00033BF1 File Offset: 0x00031DF1
		private void OnPlayer(Player player)
		{
			player.Die((player.Position - this.Position).SafeNormalize(), false, true);
		}

		// Token: 0x06001449 RID: 5193 RVA: 0x0002836E File Offset: 0x0002656E
		private void Die()
		{
			base.RemoveSelf();
		}

		// Token: 0x0600144A RID: 5194 RVA: 0x0006DC68 File Offset: 0x0006BE68
		private void PopIntoExistance(float duration)
		{
			this.Visible = true;
			this.Sprite.Scale = Vector2.Zero;
			this.Sprite.Color = Color.Transparent;
			this.Hair.Visible = true;
			this.Hair.Alpha = 0f;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, duration, true);
			tween.OnUpdate = delegate(Tween t)
			{
				this.Sprite.Scale = Vector2.One * t.Eased;
				this.Sprite.Color = Color.White * t.Eased;
				this.Hair.Alpha = t.Eased;
			};
			base.Add(tween);
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x0006DCE0 File Offset: 0x0006BEE0
		private bool OnGround(int dist = 1)
		{
			for (int i = 1; i <= dist; i++)
			{
				if (base.CollideCheck<Solid>(this.Position + new Vector2(0f, (float)i)))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000FEC RID: 4076
		public static ParticleType P_Vanish;

		// Token: 0x04000FED RID: 4077
		public static readonly Color HairColor = Calc.HexToColor("9B3FB5");

		// Token: 0x04000FEE RID: 4078
		public PlayerSprite Sprite;

		// Token: 0x04000FEF RID: 4079
		public PlayerHair Hair;

		// Token: 0x04000FF0 RID: 4080
		private LightOcclude occlude;

		// Token: 0x04000FF1 RID: 4081
		private bool ignorePlayerAnim;

		// Token: 0x04000FF2 RID: 4082
		private int index;

		// Token: 0x04000FF3 RID: 4083
		private Player player;

		// Token: 0x04000FF4 RID: 4084
		private bool following;

		// Token: 0x04000FF5 RID: 4085
		private float followBehindTime;

		// Token: 0x04000FF6 RID: 4086
		private float followBehindIndexDelay;

		// Token: 0x04000FF7 RID: 4087
		public bool Hovering;

		// Token: 0x04000FF8 RID: 4088
		private float hoveringTimer;

		// Token: 0x04000FF9 RID: 4089
		private Dictionary<string, SoundSource> loopingSounds = new Dictionary<string, SoundSource>();

		// Token: 0x04000FFA RID: 4090
		private List<SoundSource> inactiveLoopingSounds = new List<SoundSource>();
	}
}
