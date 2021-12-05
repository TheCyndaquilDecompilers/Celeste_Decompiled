using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000176 RID: 374
	public class FireBall : Entity
	{
		// Token: 0x06000D41 RID: 3393 RVA: 0x0002CE04 File Offset: 0x0002B004
		public FireBall(Vector2[] nodes, int amount, int index, float offset, float speedMult, bool notCoreMode)
		{
			base.Tag = Tags.TransitionUpdate;
			base.Collider = new Circle(6f, 0f, 0f);
			this.nodes = nodes;
			this.amount = amount;
			this.index = index;
			this.offset = offset;
			this.mult = speedMult;
			this.notCoreMode = notCoreMode;
			this.lengths = new float[nodes.Length];
			for (int i = 1; i < this.lengths.Length; i++)
			{
				this.lengths[i] = this.lengths[i - 1] + Vector2.Distance(nodes[i - 1], nodes[i]);
			}
			this.speed = 60f / this.lengths[this.lengths.Length - 1] * this.mult;
			if (index == 0)
			{
				this.percent = 0f;
			}
			else
			{
				this.percent = (float)index / (float)amount;
			}
			this.percent += 1f / (float)amount * offset;
			this.percent %= 1f;
			this.Position = this.GetPercentPosition(this.percent);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(new PlayerCollider(new Action<Player>(this.OnBounce), new Hitbox(16f, 6f, -8f, -3f), null));
			base.Add(new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
			base.Add(this.sprite = GFX.SpriteBank.Create("fireball"));
			base.Add(this.hitWiggler = Wiggler.Create(1.2f, 2f, null, false, false));
			this.hitWiggler.StartZero = true;
			if (index == 0)
			{
				base.Add(this.trackSfx = new SoundSource());
			}
		}

		// Token: 0x06000D42 RID: 3394 RVA: 0x0002CFF8 File Offset: 0x0002B1F8
		public FireBall(EntityData data, Vector2 offset) : this(data.NodesWithPosition(offset), data.Int("amount", 1), 0, data.Float("offset", 0f), data.Float("speed", 1f), data.Bool("notCoreMode", false))
		{
		}

		// Token: 0x06000D43 RID: 3395 RVA: 0x0002D04C File Offset: 0x0002B24C
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.iceMode = (base.SceneAs<Level>().CoreMode == Session.CoreModes.Cold || this.notCoreMode);
			this.speedMult = (float)(this.iceMode ? 0 : 1);
			this.sprite.Play(this.iceMode ? "ice" : "hot", false, true);
			if (this.index == 0)
			{
				for (int i = 1; i < this.amount; i++)
				{
					base.Scene.Add(new FireBall(this.nodes, this.amount, i, this.offset, this.mult, this.notCoreMode));
				}
			}
			if (this.trackSfx != null && !this.iceMode)
			{
				this.PositionTrackSfx();
				this.trackSfx.Play("event:/env/local/09_core/fireballs_idle", null, 0f);
			}
		}

		// Token: 0x06000D44 RID: 3396 RVA: 0x0002D128 File Offset: 0x0002B328
		public override void Update()
		{
			if ((base.Scene as Level).Transitioning)
			{
				this.PositionTrackSfx();
				return;
			}
			base.Update();
			this.speedMult = Calc.Approach(this.speedMult, this.iceMode ? 0.5f : 1f, 2f * Engine.DeltaTime);
			this.percent += this.speed * this.speedMult * Engine.DeltaTime;
			if (this.percent >= 1f)
			{
				this.percent %= 1f;
				if (this.broken && this.nodes[this.nodes.Length - 1] != this.nodes[0])
				{
					this.broken = false;
					this.Collidable = true;
					this.sprite.Play(this.iceMode ? "ice" : "hot", false, true);
				}
			}
			this.Position = this.GetPercentPosition(this.percent);
			this.PositionTrackSfx();
			if (!this.broken && base.Scene.OnInterval(this.iceMode ? 0.08f : 0.05f))
			{
				base.SceneAs<Level>().ParticlesBG.Emit(this.iceMode ? FireBall.P_IceTrail : FireBall.P_FireTrail, 1, base.Center, Vector2.One * 4f);
			}
		}

		// Token: 0x06000D45 RID: 3397 RVA: 0x0002D2A0 File Offset: 0x0002B4A0
		public void PositionTrackSfx()
		{
			if (this.trackSfx != null)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					Vector2? vector = null;
					for (int i = 1; i < this.nodes.Length; i++)
					{
						Vector2 vector2 = Calc.ClosestPointOnLine(this.nodes[i - 1], this.nodes[i], entity.Center);
						if (vector == null || (vector2 - entity.Center).Length() < (vector.Value - entity.Center).Length())
						{
							vector = new Vector2?(vector2);
						}
					}
					if (vector != null)
					{
						this.trackSfx.Position = vector.Value - this.Position;
						this.trackSfx.UpdateSfxPosition();
					}
				}
			}
		}

		// Token: 0x06000D46 RID: 3398 RVA: 0x0002D384 File Offset: 0x0002B584
		public override void Render()
		{
			this.sprite.Position = this.hitDir * this.hitWiggler.Value * 8f;
			if (!this.broken)
			{
				this.sprite.DrawOutline(Color.Black, 1);
			}
			base.Render();
		}

		// Token: 0x06000D47 RID: 3399 RVA: 0x0002D3DC File Offset: 0x0002B5DC
		private void OnPlayer(Player player)
		{
			if (!this.iceMode && !this.broken)
			{
				this.KillPlayer(player);
				return;
			}
			if (this.iceMode && !this.broken && player.Bottom > base.Y + 4f)
			{
				this.KillPlayer(player);
			}
		}

		// Token: 0x06000D48 RID: 3400 RVA: 0x0002D42C File Offset: 0x0002B62C
		private void KillPlayer(Player player)
		{
			Vector2 direction = (player.Center - base.Center).SafeNormalize();
			if (player.Die(direction, false, true) != null)
			{
				this.hitDir = direction;
				this.hitWiggler.Start();
			}
		}

		// Token: 0x06000D49 RID: 3401 RVA: 0x0002D470 File Offset: 0x0002B670
		private void OnBounce(Player player)
		{
			if (this.iceMode && !this.broken && player.Bottom <= base.Y + 4f && player.Speed.Y >= 0f)
			{
				Audio.Play("event:/game/09_core/iceball_break", this.Position);
				this.sprite.Play("shatter", false, false);
				this.broken = true;
				this.Collidable = false;
				player.Bounce((float)((int)(base.Y - 2f)));
				base.SceneAs<Level>().Particles.Emit(FireBall.P_IceBreak, 18, base.Center, Vector2.One * 6f);
			}
		}

		// Token: 0x06000D4A RID: 3402 RVA: 0x0002D530 File Offset: 0x0002B730
		private void OnChangeMode(Session.CoreModes mode)
		{
			this.iceMode = (mode == Session.CoreModes.Cold);
			if (!this.broken)
			{
				this.sprite.Play(this.iceMode ? "ice" : "hot", false, true);
			}
			if (this.index == 0 && this.trackSfx != null)
			{
				if (this.iceMode)
				{
					this.trackSfx.Stop(true);
					return;
				}
				this.PositionTrackSfx();
				this.trackSfx.Play("event:/env/local/09_core/fireballs_idle", null, 0f);
			}
		}

		// Token: 0x06000D4B RID: 3403 RVA: 0x0002D5B4 File Offset: 0x0002B7B4
		private Vector2 GetPercentPosition(float percent)
		{
			if (percent <= 0f)
			{
				return this.nodes[0];
			}
			if (percent >= 1f)
			{
				return this.nodes[this.nodes.Length - 1];
			}
			float num = this.lengths[this.lengths.Length - 1];
			float num2 = num * percent;
			int num3 = 0;
			while (num3 < this.lengths.Length - 1 && this.lengths[num3 + 1] <= num2)
			{
				num3++;
			}
			float min = this.lengths[num3] / num;
			float max = this.lengths[num3 + 1] / num;
			float num4 = Calc.ClampedMap(percent, min, max, 0f, 1f);
			return Vector2.Lerp(this.nodes[num3], this.nodes[num3 + 1], num4);
		}

		// Token: 0x04000881 RID: 2177
		public static ParticleType P_FireTrail;

		// Token: 0x04000882 RID: 2178
		public static ParticleType P_IceTrail;

		// Token: 0x04000883 RID: 2179
		public static ParticleType P_IceBreak;

		// Token: 0x04000884 RID: 2180
		private const float FireSpeed = 60f;

		// Token: 0x04000885 RID: 2181
		private const float IceSpeed = 30f;

		// Token: 0x04000886 RID: 2182
		private const float IceSpeedMult = 0.5f;

		// Token: 0x04000887 RID: 2183
		private Vector2[] nodes;

		// Token: 0x04000888 RID: 2184
		private int amount;

		// Token: 0x04000889 RID: 2185
		private int index;

		// Token: 0x0400088A RID: 2186
		private float offset;

		// Token: 0x0400088B RID: 2187
		private float[] lengths;

		// Token: 0x0400088C RID: 2188
		private float speed;

		// Token: 0x0400088D RID: 2189
		private float speedMult;

		// Token: 0x0400088E RID: 2190
		private float percent;

		// Token: 0x0400088F RID: 2191
		private bool iceMode;

		// Token: 0x04000890 RID: 2192
		private bool broken;

		// Token: 0x04000891 RID: 2193
		private float mult;

		// Token: 0x04000892 RID: 2194
		private bool notCoreMode;

		// Token: 0x04000893 RID: 2195
		private SoundSource trackSfx;

		// Token: 0x04000894 RID: 2196
		private Sprite sprite;

		// Token: 0x04000895 RID: 2197
		private Wiggler hitWiggler;

		// Token: 0x04000896 RID: 2198
		private Vector2 hitDir;
	}
}
