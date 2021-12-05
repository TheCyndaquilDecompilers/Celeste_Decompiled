using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200017B RID: 379
	public class SandwichLava : Entity
	{
		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000D6D RID: 3437 RVA: 0x0002E4D8 File Offset: 0x0002C6D8
		private float centerY
		{
			get
			{
				return (float)base.SceneAs<Level>().Bounds.Bottom - 10f;
			}
		}

		// Token: 0x06000D6E RID: 3438 RVA: 0x0002E500 File Offset: 0x0002C700
		public SandwichLava(float startX)
		{
			this.startX = startX;
			base.Depth = -1000000;
			base.Collider = new ColliderList(new Collider[]
			{
				new Hitbox(340f, 120f, 0f, 0f),
				new Hitbox(340f, 120f, 0f, -280f)
			});
			this.Visible = false;
			base.Add(this.loopSfx = new SoundSource());
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
			base.Add(this.bottomRect = new LavaRect(400f, 200f, 4));
			this.bottomRect.Position = new Vector2(-40f, 0f);
			this.bottomRect.OnlyMode = LavaRect.OnlyModes.OnlyTop;
			this.bottomRect.SmallWaveAmplitude = 2f;
			base.Add(this.topRect = new LavaRect(400f, 200f, 4));
			this.topRect.Position = new Vector2(-40f, -360f);
			this.topRect.OnlyMode = LavaRect.OnlyModes.OnlyBottom;
			this.topRect.SmallWaveAmplitude = 2f;
			this.topRect.BigWaveAmplitude = (this.bottomRect.BigWaveAmplitude = 2f);
			this.topRect.CurveAmplitude = (this.bottomRect.CurveAmplitude = 4f);
			base.Add(new TransitionListener
			{
				OnOutBegin = delegate()
				{
					this.transitionStartY = base.Y;
					if (this.persistent && base.Scene != null && base.Scene.Entities.FindAll<SandwichLava>().Count <= 1)
					{
						this.Leave();
					}
				},
				OnOut = delegate(float f)
				{
					if (base.Scene != null)
					{
						base.X = (base.Scene as Level).Camera.X;
						if (!this.leaving)
						{
							base.Y = MathHelper.Lerp(this.transitionStartY, this.centerY, f);
						}
					}
					if (f > 0.95f & this.leaving)
					{
						base.RemoveSelf();
					}
				}
			});
		}

		// Token: 0x06000D6F RID: 3439 RVA: 0x0002E6D0 File Offset: 0x0002C8D0
		public SandwichLava(EntityData data, Vector2 offset) : this(data.Position.X + offset.X)
		{
		}

		// Token: 0x06000D70 RID: 3440 RVA: 0x0002E6EC File Offset: 0x0002C8EC
		public override void Added(Scene scene)
		{
			base.Added(scene);
			base.X = (float)(base.SceneAs<Level>().Bounds.Left - 10);
			base.Y = this.centerY;
			this.iceMode = (base.SceneAs<Level>().Session.CoreMode == Session.CoreModes.Cold);
		}

		// Token: 0x06000D71 RID: 3441 RVA: 0x0002E744 File Offset: 0x0002C944
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null && (entity.JustRespawned || entity.X < this.startX))
			{
				this.Waiting = true;
			}
			List<SandwichLava> list = base.Scene.Entities.FindAll<SandwichLava>();
			bool flag = false;
			if (!this.persistent && list.Count >= 2)
			{
				SandwichLava sandwichLava = (list[0] == this) ? list[1] : list[0];
				if (!sandwichLava.leaving)
				{
					sandwichLava.startX = this.startX;
					sandwichLava.Waiting = true;
					base.RemoveSelf();
					flag = true;
				}
			}
			if (!flag)
			{
				this.persistent = true;
				base.Tag = Tags.Persistent;
				if ((scene as Level).LastIntroType != Player.IntroTypes.Respawn)
				{
					LavaRect lavaRect = this.topRect;
					lavaRect.Position.Y = lavaRect.Position.Y - 60f;
					LavaRect lavaRect2 = this.bottomRect;
					lavaRect2.Position.Y = lavaRect2.Position.Y + 60f;
				}
				else
				{
					this.Visible = true;
				}
				this.loopSfx.Play("event:/game/09_core/rising_threat", "room_state", (float)(this.iceMode ? 1 : 0));
				this.loopSfx.Position = new Vector2(base.Width / 2f, 0f);
			}
		}

		// Token: 0x06000D72 RID: 3442 RVA: 0x0002E895 File Offset: 0x0002CA95
		private void OnChangeMode(Session.CoreModes mode)
		{
			this.iceMode = (mode == Session.CoreModes.Cold);
			this.loopSfx.Param("room_state", (float)(this.iceMode ? 1 : 0));
		}

		// Token: 0x06000D73 RID: 3443 RVA: 0x0002E8C0 File Offset: 0x0002CAC0
		private void OnPlayer(Player player)
		{
			if (!this.Waiting)
			{
				if (SaveData.Instance.Assists.Invincible)
				{
					if (this.delay <= 0f)
					{
						int num = (player.Y > base.Y + this.bottomRect.Position.Y - 32f) ? 1 : -1;
						float from = base.Y;
						float to = base.Y + (float)(num * 48);
						player.Speed.Y = (float)(-(float)num * 200);
						if (num > 0)
						{
							player.RefillDash();
						}
						Tween.Set(this, Tween.TweenMode.Oneshot, 0.4f, Ease.CubeOut, delegate(Tween t)
						{
							this.Y = MathHelper.Lerp(from, to, t.Eased);
						}, null);
						this.delay = 0.5f;
						this.loopSfx.Param("rising", 0f);
						Audio.Play("event:/game/general/assist_screenbottom", player.Position);
						return;
					}
				}
				else
				{
					player.Die(-Vector2.UnitY, false, true);
				}
			}
		}

		// Token: 0x06000D74 RID: 3444 RVA: 0x0002E9D5 File Offset: 0x0002CBD5
		public void Leave()
		{
			base.AddTag(Tags.TransitionUpdate);
			this.leaving = true;
			this.Collidable = false;
			Alarm.Set(this, 2f, delegate
			{
				base.RemoveSelf();
			}, Alarm.AlarmMode.Oneshot);
		}

		// Token: 0x06000D75 RID: 3445 RVA: 0x0002EA10 File Offset: 0x0002CC10
		public override void Update()
		{
			Level level = base.Scene as Level;
			base.X = level.Camera.X;
			this.delay -= Engine.DeltaTime;
			base.Update();
			this.Visible = true;
			if (this.Waiting)
			{
				base.Y = Calc.Approach(base.Y, this.centerY, 128f * Engine.DeltaTime);
				this.loopSfx.Param("rising", 0f);
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.X >= this.startX && !entity.JustRespawned && entity.StateMachine.State != 11)
				{
					this.Waiting = false;
				}
			}
			else if (!this.leaving && this.delay <= 0f)
			{
				this.loopSfx.Param("rising", 1f);
				if (this.iceMode)
				{
					base.Y += 20f * Engine.DeltaTime;
				}
				else
				{
					base.Y -= 20f * Engine.DeltaTime;
				}
			}
			this.topRect.Position.Y = Calc.Approach(this.topRect.Position.Y, -160f - this.topRect.Height + (float)(this.leaving ? -512 : 0), (float)(this.leaving ? 256 : 64) * Engine.DeltaTime);
			this.bottomRect.Position.Y = Calc.Approach(this.bottomRect.Position.Y, (float)(this.leaving ? 512 : 0), (float)(this.leaving ? 256 : 64) * Engine.DeltaTime);
			this.lerp = Calc.Approach(this.lerp, (float)(this.iceMode ? 1 : 0), Engine.DeltaTime * 4f);
			this.bottomRect.SurfaceColor = Color.Lerp(RisingLava.Hot[0], RisingLava.Cold[0], this.lerp);
			this.bottomRect.EdgeColor = Color.Lerp(RisingLava.Hot[1], RisingLava.Cold[1], this.lerp);
			this.bottomRect.CenterColor = Color.Lerp(RisingLava.Hot[2], RisingLava.Cold[2], this.lerp);
			this.bottomRect.Spikey = this.lerp * 5f;
			this.bottomRect.UpdateMultiplier = (1f - this.lerp) * 2f;
			this.bottomRect.Fade = (float)(this.iceMode ? 128 : 32);
			this.topRect.SurfaceColor = this.bottomRect.SurfaceColor;
			this.topRect.EdgeColor = this.bottomRect.EdgeColor;
			this.topRect.CenterColor = this.bottomRect.CenterColor;
			this.topRect.Spikey = this.bottomRect.Spikey;
			this.topRect.UpdateMultiplier = this.bottomRect.UpdateMultiplier;
			this.topRect.Fade = this.bottomRect.Fade;
		}

		// Token: 0x040008AC RID: 2220
		private const float TopOffset = -160f;

		// Token: 0x040008AD RID: 2221
		private const float Speed = 20f;

		// Token: 0x040008AE RID: 2222
		public bool Waiting;

		// Token: 0x040008AF RID: 2223
		private bool iceMode;

		// Token: 0x040008B0 RID: 2224
		private float startX;

		// Token: 0x040008B1 RID: 2225
		private float lerp;

		// Token: 0x040008B2 RID: 2226
		private float transitionStartY;

		// Token: 0x040008B3 RID: 2227
		private bool leaving;

		// Token: 0x040008B4 RID: 2228
		private float delay;

		// Token: 0x040008B5 RID: 2229
		private LavaRect bottomRect;

		// Token: 0x040008B6 RID: 2230
		private LavaRect topRect;

		// Token: 0x040008B7 RID: 2231
		private bool persistent;

		// Token: 0x040008B8 RID: 2232
		private SoundSource loopSfx;
	}
}
