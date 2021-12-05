using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200014E RID: 334
	public class IceBlock : Entity
	{
		// Token: 0x06000C29 RID: 3113 RVA: 0x00026FA0 File Offset: 0x000251A0
		public IceBlock(Vector2 position, float width, float height) : base(position)
		{
			base.Collider = new Hitbox(width, height, 0f, 0f);
			base.Add(new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(this.lava = new LavaRect(width, height, 2));
			this.lava.UpdateMultiplier = 0f;
			this.lava.SurfaceColor = Calc.HexToColor("a6fff4");
			this.lava.EdgeColor = Calc.HexToColor("6cd6eb");
			this.lava.CenterColor = Calc.HexToColor("4ca8d6");
			this.lava.SmallWaveAmplitude = 1f;
			this.lava.BigWaveAmplitude = 1f;
			this.lava.CurveAmplitude = 1f;
			this.lava.Spikey = 3f;
			base.Depth = -8500;
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x000270AC File Offset: 0x000252AC
		public IceBlock(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height)
		{
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x000270D0 File Offset: 0x000252D0
		public override void Added(Scene scene)
		{
			base.Added(scene);
			scene.Add(this.solid = new Solid(this.Position + new Vector2(2f, 3f), base.Width - 4f, base.Height - 5f, false));
			this.Collidable = (this.solid.Collidable = (base.SceneAs<Level>().CoreMode == Session.CoreModes.Cold));
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x00027150 File Offset: 0x00025350
		private void OnChangeMode(Session.CoreModes mode)
		{
			this.Collidable = (this.solid.Collidable = (mode == Session.CoreModes.Cold));
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
						level.Particles.Emit(IceBlock.P_Deactivate, vector, (vector - center).Angle());
						num2 += 4;
					}
					num += 4;
				}
			}
		}

		// Token: 0x06000C2D RID: 3117 RVA: 0x00027228 File Offset: 0x00025428
		private void OnPlayer(Player player)
		{
			player.Die((player.Center - base.Center).SafeNormalize(), false, true);
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x00027249 File Offset: 0x00025449
		public override void Render()
		{
			if (this.Collidable)
			{
				base.Render();
			}
		}

		// Token: 0x0400077E RID: 1918
		public static ParticleType P_Deactivate;

		// Token: 0x0400077F RID: 1919
		private LavaRect lava;

		// Token: 0x04000780 RID: 1920
		private Solid solid;
	}
}
