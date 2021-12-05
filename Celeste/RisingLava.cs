using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200017A RID: 378
	public class RisingLava : Entity
	{
		// Token: 0x06000D65 RID: 3429 RVA: 0x0002DEB0 File Offset: 0x0002C0B0
		public RisingLava(bool intro)
		{
			this.intro = intro;
			base.Depth = -1000000;
			base.Collider = new Hitbox(340f, 120f, 0f, 0f);
			this.Visible = false;
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
			base.Add(this.loopSfx = new SoundSource());
			base.Add(this.bottomRect = new LavaRect(400f, 200f, 4));
			this.bottomRect.Position = new Vector2(-40f, 0f);
			this.bottomRect.OnlyMode = LavaRect.OnlyModes.OnlyTop;
			this.bottomRect.SmallWaveAmplitude = 2f;
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x0002DF94 File Offset: 0x0002C194
		public RisingLava(EntityData data, Vector2 offset) : this(data.Bool("intro", false))
		{
		}

		// Token: 0x06000D67 RID: 3431 RVA: 0x0002DFA8 File Offset: 0x0002C1A8
		public override void Added(Scene scene)
		{
			base.Added(scene);
			base.X = (float)(base.SceneAs<Level>().Bounds.Left - 10);
			base.Y = (float)(base.SceneAs<Level>().Bounds.Bottom + 16);
			this.iceMode = (base.SceneAs<Level>().Session.CoreMode == Session.CoreModes.Cold);
			this.loopSfx.Play("event:/game/09_core/rising_threat", "room_state", (float)(this.iceMode ? 1 : 0));
			this.loopSfx.Position = new Vector2(base.Width / 2f, 0f);
		}

		// Token: 0x06000D68 RID: 3432 RVA: 0x0002E054 File Offset: 0x0002C254
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (this.intro)
			{
				this.waiting = true;
			}
			else
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.JustRespawned)
				{
					this.waiting = true;
				}
			}
			if (this.intro)
			{
				this.Visible = true;
			}
		}

		// Token: 0x06000D69 RID: 3433 RVA: 0x0002E0AB File Offset: 0x0002C2AB
		private void OnChangeMode(Session.CoreModes mode)
		{
			this.iceMode = (mode == Session.CoreModes.Cold);
			this.loopSfx.Param("room_state", (float)(this.iceMode ? 1 : 0));
		}

		// Token: 0x06000D6A RID: 3434 RVA: 0x0002E0D8 File Offset: 0x0002C2D8
		private void OnPlayer(Player player)
		{
			if (SaveData.Instance.Assists.Invincible)
			{
				if (this.delay <= 0f)
				{
					float from = base.Y;
					float to = base.Y + 48f;
					player.Speed.Y = -200f;
					player.RefillDash();
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

		// Token: 0x06000D6B RID: 3435 RVA: 0x0002E1B0 File Offset: 0x0002C3B0
		public override void Update()
		{
			this.delay -= Engine.DeltaTime;
			base.X = base.SceneAs<Level>().Camera.X;
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			base.Update();
			this.Visible = true;
			if (this.waiting)
			{
				this.loopSfx.Param("rising", 0f);
				if (!this.intro && entity != null && entity.JustRespawned)
				{
					base.Y = Calc.Approach(base.Y, entity.Y + 32f, 32f * Engine.DeltaTime);
				}
				if ((!this.iceMode || !this.intro) && (entity == null || !entity.JustRespawned))
				{
					this.waiting = false;
				}
			}
			else
			{
				float num = base.SceneAs<Level>().Camera.Bottom - 12f;
				if (base.Top > num + 96f)
				{
					base.Top = num + 96f;
				}
				float num2;
				if (base.Top > num)
				{
					num2 = Calc.ClampedMap(base.Top - num, 0f, 96f, 1f, 2f);
				}
				else
				{
					num2 = Calc.ClampedMap(num - base.Top, 0f, 32f, 1f, 0.5f);
				}
				if (this.delay <= 0f)
				{
					this.loopSfx.Param("rising", 1f);
					base.Y += -30f * num2 * Engine.DeltaTime;
				}
			}
			this.lerp = Calc.Approach(this.lerp, (float)(this.iceMode ? 1 : 0), Engine.DeltaTime * 4f);
			this.bottomRect.SurfaceColor = Color.Lerp(RisingLava.Hot[0], RisingLava.Cold[0], this.lerp);
			this.bottomRect.EdgeColor = Color.Lerp(RisingLava.Hot[1], RisingLava.Cold[1], this.lerp);
			this.bottomRect.CenterColor = Color.Lerp(RisingLava.Hot[2], RisingLava.Cold[2], this.lerp);
			this.bottomRect.Spikey = this.lerp * 5f;
			this.bottomRect.UpdateMultiplier = (1f - this.lerp) * 2f;
			this.bottomRect.Fade = (float)(this.iceMode ? 128 : 32);
		}

		// Token: 0x040008A2 RID: 2210
		private const float Speed = -30f;

		// Token: 0x040008A3 RID: 2211
		private bool intro;

		// Token: 0x040008A4 RID: 2212
		private bool iceMode;

		// Token: 0x040008A5 RID: 2213
		private bool waiting;

		// Token: 0x040008A6 RID: 2214
		private float lerp;

		// Token: 0x040008A7 RID: 2215
		public static Color[] Hot = new Color[]
		{
			Calc.HexToColor("ff8933"),
			Calc.HexToColor("f25e29"),
			Calc.HexToColor("d01c01")
		};

		// Token: 0x040008A8 RID: 2216
		public static Color[] Cold = new Color[]
		{
			Calc.HexToColor("33ffe7"),
			Calc.HexToColor("4ca2eb"),
			Calc.HexToColor("0151d0")
		};

		// Token: 0x040008A9 RID: 2217
		private LavaRect bottomRect;

		// Token: 0x040008AA RID: 2218
		private float delay;

		// Token: 0x040008AB RID: 2219
		private SoundSource loopSfx;
	}
}
