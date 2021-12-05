using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000177 RID: 375
	public class FireBarrier : Entity
	{
		// Token: 0x06000D4C RID: 3404 RVA: 0x0002D67C File Offset: 0x0002B87C
		public FireBarrier(Vector2 position, float width, float height) : base(position)
		{
			base.Tag = Tags.TransitionUpdate;
			base.Collider = new Hitbox(width, height, 0f, 0f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
			base.Add(this.Lava = new LavaRect(width, height, 4));
			this.Lava.SurfaceColor = RisingLava.Hot[0];
			this.Lava.EdgeColor = RisingLava.Hot[1];
			this.Lava.CenterColor = RisingLava.Hot[2];
			this.Lava.SmallWaveAmplitude = 2f;
			this.Lava.BigWaveAmplitude = 1f;
			this.Lava.CurveAmplitude = 1f;
			base.Depth = -8500;
			base.Add(this.idleSfx = new SoundSource());
			this.idleSfx.Position = new Vector2(base.Width, base.Height) / 2f;
		}

		// Token: 0x06000D4D RID: 3405 RVA: 0x0002D7B5 File Offset: 0x0002B9B5
		public FireBarrier(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height)
		{
		}

		// Token: 0x06000D4E RID: 3406 RVA: 0x0002D7D8 File Offset: 0x0002B9D8
		public override void Added(Scene scene)
		{
			base.Added(scene);
			scene.Add(this.solid = new Solid(this.Position + new Vector2(2f, 3f), base.Width - 4f, base.Height - 5f, false));
			this.Collidable = (this.solid.Collidable = (base.SceneAs<Level>().CoreMode == Session.CoreModes.Hot));
			if (this.Collidable)
			{
				this.idleSfx.Play("event:/env/local/09_core/lavagate_idle", null, 0f);
			}
		}

		// Token: 0x06000D4F RID: 3407 RVA: 0x0002D874 File Offset: 0x0002BA74
		private void OnChangeMode(Session.CoreModes mode)
		{
			this.Collidable = (this.solid.Collidable = (mode == Session.CoreModes.Hot));
			if (!this.Collidable)
			{
				Level level = base.SceneAs<Level>();
				Vector2 center = base.Center;
				int num = 0;
				while ((float)num < base.Width)
				{
					int num2 = 0;
					while ((float)num2 < base.Height)
					{
						Vector2 vector = this.Position + new Vector2((float)(num + 2), (float)(num2 + 2)) + Calc.Random.Range(-Vector2.One * 2f, Vector2.One * 2f);
						level.Particles.Emit(FireBarrier.P_Deactivate, vector, (vector - center).Angle());
						num2 += 4;
					}
					num += 4;
				}
				this.idleSfx.Stop(true);
				return;
			}
			this.idleSfx.Play("event:/env/local/09_core/lavagate_idle", null, 0f);
		}

		// Token: 0x06000D50 RID: 3408 RVA: 0x00027228 File Offset: 0x00025428
		private void OnPlayer(Player player)
		{
			player.Die((player.Center - base.Center).SafeNormalize(), false, true);
		}

		// Token: 0x06000D51 RID: 3409 RVA: 0x0002D971 File Offset: 0x0002BB71
		public override void Update()
		{
			if ((base.Scene as Level).Transitioning)
			{
				if (this.idleSfx != null)
				{
					this.idleSfx.UpdateSfxPosition();
				}
				return;
			}
			base.Update();
		}

		// Token: 0x06000D52 RID: 3410 RVA: 0x00027249 File Offset: 0x00025449
		public override void Render()
		{
			if (this.Collidable)
			{
				base.Render();
			}
		}

		// Token: 0x04000897 RID: 2199
		public static ParticleType P_Deactivate;

		// Token: 0x04000898 RID: 2200
		private LavaRect Lava;

		// Token: 0x04000899 RID: 2201
		private Solid solid;

		// Token: 0x0400089A RID: 2202
		private SoundSource idleSfx;
	}
}
