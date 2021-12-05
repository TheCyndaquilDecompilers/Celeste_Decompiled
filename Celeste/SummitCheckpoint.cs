using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001CB RID: 459
	public class SummitCheckpoint : Entity
	{
		// Token: 0x06000FAB RID: 4011 RVA: 0x000422CC File Offset: 0x000404CC
		public SummitCheckpoint(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			this.Number = data.Int("number", 0);
			this.numberString = this.Number.ToString("D2");
			this.baseEmpty = GFX.Game["scenery/summitcheckpoints/base00"];
			this.baseToggle = GFX.Game["scenery/summitcheckpoints/base01"];
			this.baseActive = GFX.Game["scenery/summitcheckpoints/base02"];
			this.numbersEmpty = GFX.Game.GetAtlasSubtextures("scenery/summitcheckpoints/numberbg");
			this.numbersActive = GFX.Game.GetAtlasSubtextures("scenery/summitcheckpoints/number");
			base.Collider = new Hitbox(32f, 32f, -16f, -8f);
			base.Depth = 8999;
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x000423A8 File Offset: 0x000405A8
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if ((scene as Level).Session.GetFlag("summit_checkpoint_" + this.Number))
			{
				this.Activated = true;
			}
			this.respawn = base.SceneAs<Level>().GetSpawnPoint(this.Position);
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x00042404 File Offset: 0x00040604
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (!this.Activated && base.CollideCheck<Player>())
			{
				this.Activated = true;
				Level level = base.Scene as Level;
				level.Session.SetFlag("summit_checkpoint_" + this.Number, true);
				level.Session.RespawnPoint = new Vector2?(this.respawn);
			}
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x00042470 File Offset: 0x00040670
		public override void Update()
		{
			if (!this.Activated)
			{
				Player player = base.CollideFirst<Player>();
				if (player != null && player.OnGround(1) && player.Speed.Y >= 0f)
				{
					Level level = base.Scene as Level;
					this.Activated = true;
					level.Session.SetFlag("summit_checkpoint_" + this.Number, true);
					level.Session.RespawnPoint = new Vector2?(this.respawn);
					level.Session.UpdateLevelStartDashes();
					level.Session.HitCheckpoint = true;
					level.Displacement.AddBurst(this.Position, 0.5f, 4f, 24f, 0.5f, null, null);
					level.Add(new SummitCheckpoint.ConfettiRenderer(this.Position));
					Audio.Play("event:/game/07_summit/checkpoint_confetti", this.Position);
				}
			}
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x00042560 File Offset: 0x00040760
		public override void Render()
		{
			object obj = this.Activated ? this.numbersActive : this.numbersEmpty;
			MTexture mtexture = this.baseActive;
			if (!this.Activated)
			{
				mtexture = (base.Scene.BetweenInterval(0.25f) ? this.baseEmpty : this.baseToggle);
			}
			mtexture.Draw(this.Position - new Vector2((float)(mtexture.Width / 2 + 1), (float)(mtexture.Height / 2)));
			object obj2 = obj;
			obj2[(int)(this.numberString[0] - '0')].DrawJustified(this.Position + new Vector2(-1f, 1f), new Vector2(1f, 0f));
			obj2[(int)(this.numberString[1] - '0')].DrawJustified(this.Position + new Vector2(0f, 1f), new Vector2(0f, 0f));
		}

		// Token: 0x04000B14 RID: 2836
		private const string Flag = "summit_checkpoint_";

		// Token: 0x04000B15 RID: 2837
		public bool Activated;

		// Token: 0x04000B16 RID: 2838
		public readonly int Number;

		// Token: 0x04000B17 RID: 2839
		private string numberString;

		// Token: 0x04000B18 RID: 2840
		private Vector2 respawn;

		// Token: 0x04000B19 RID: 2841
		private MTexture baseEmpty;

		// Token: 0x04000B1A RID: 2842
		private MTexture baseToggle;

		// Token: 0x04000B1B RID: 2843
		private MTexture baseActive;

		// Token: 0x04000B1C RID: 2844
		private List<MTexture> numbersEmpty;

		// Token: 0x04000B1D RID: 2845
		private List<MTexture> numbersActive;

		// Token: 0x020004CF RID: 1231
		public class ConfettiRenderer : Entity
		{
			// Token: 0x06002414 RID: 9236 RVA: 0x000F1C94 File Offset: 0x000EFE94
			public ConfettiRenderer(Vector2 position) : base(position)
			{
				base.Depth = -10010;
				for (int i = 0; i < this.particles.Length; i++)
				{
					this.particles[i].Position = this.Position + new Vector2((float)Calc.Random.Range(-3, 3), (float)Calc.Random.Range(-3, 3));
					this.particles[i].Color = Calc.Random.Choose(SummitCheckpoint.ConfettiRenderer.confettiColors);
					this.particles[i].Timer = Calc.Random.NextFloat();
					this.particles[i].Duration = (float)Calc.Random.Range(2, 4);
					this.particles[i].Alpha = 1f;
					float angleRadians = -1.5707964f + Calc.Random.Range(-0.5f, 0.5f);
					int num = Calc.Random.Range(140, 220);
					this.particles[i].Speed = Calc.AngleToVector(angleRadians, (float)num);
				}
			}

			// Token: 0x06002415 RID: 9237 RVA: 0x000F1DD0 File Offset: 0x000EFFD0
			public override void Update()
			{
				for (int i = 0; i < this.particles.Length; i++)
				{
					SummitCheckpoint.ConfettiRenderer.Particle[] array = this.particles;
					int num = i;
					array[num].Position = array[num].Position + this.particles[i].Speed * Engine.DeltaTime;
					this.particles[i].Speed.X = Calc.Approach(this.particles[i].Speed.X, 0f, 80f * Engine.DeltaTime);
					this.particles[i].Speed.Y = Calc.Approach(this.particles[i].Speed.Y, 20f, 500f * Engine.DeltaTime);
					SummitCheckpoint.ConfettiRenderer.Particle[] array2 = this.particles;
					int num2 = i;
					array2[num2].Timer = array2[num2].Timer + Engine.DeltaTime;
					SummitCheckpoint.ConfettiRenderer.Particle[] array3 = this.particles;
					int num3 = i;
					array3[num3].Percent = array3[num3].Percent + Engine.DeltaTime / this.particles[i].Duration;
					this.particles[i].Alpha = Calc.ClampedMap(this.particles[i].Percent, 0.9f, 1f, 1f, 0f);
					if (this.particles[i].Speed.Y > 0f)
					{
						this.particles[i].Approach = Calc.Approach(this.particles[i].Approach, 5f, Engine.DeltaTime * 16f);
					}
				}
			}

			// Token: 0x06002416 RID: 9238 RVA: 0x000F1F8C File Offset: 0x000F018C
			public override void Render()
			{
				for (int i = 0; i < this.particles.Length; i++)
				{
					Vector2 vector = this.particles[i].Position;
					float num;
					if (this.particles[i].Speed.Y < 0f)
					{
						num = this.particles[i].Speed.Angle();
					}
					else
					{
						num = (float)Math.Sin((double)(this.particles[i].Timer * 4f)) * 1f;
						vector += Calc.AngleToVector(1.5707964f + num, this.particles[i].Approach);
					}
					GFX.Game["particles/confetti"].DrawCentered(vector + Vector2.UnitY, Color.Black * (this.particles[i].Alpha * 0.5f), 1f, num);
					GFX.Game["particles/confetti"].DrawCentered(vector, this.particles[i].Color * this.particles[i].Alpha, 1f, num);
				}
			}

			// Token: 0x040023B6 RID: 9142
			private static readonly Color[] confettiColors = new Color[]
			{
				Calc.HexToColor("fe2074"),
				Calc.HexToColor("205efe"),
				Calc.HexToColor("cefe20")
			};

			// Token: 0x040023B7 RID: 9143
			private SummitCheckpoint.ConfettiRenderer.Particle[] particles = new SummitCheckpoint.ConfettiRenderer.Particle[30];

			// Token: 0x0200078B RID: 1931
			private struct Particle
			{
				// Token: 0x04002F88 RID: 12168
				public Vector2 Position;

				// Token: 0x04002F89 RID: 12169
				public Color Color;

				// Token: 0x04002F8A RID: 12170
				public Vector2 Speed;

				// Token: 0x04002F8B RID: 12171
				public float Timer;

				// Token: 0x04002F8C RID: 12172
				public float Percent;

				// Token: 0x04002F8D RID: 12173
				public float Duration;

				// Token: 0x04002F8E RID: 12174
				public float Alpha;

				// Token: 0x04002F8F RID: 12175
				public float Approach;
			}
		}
	}
}
