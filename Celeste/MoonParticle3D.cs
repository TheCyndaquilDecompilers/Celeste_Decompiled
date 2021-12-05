using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000247 RID: 583
	public class MoonParticle3D : Entity
	{
		// Token: 0x06001269 RID: 4713 RVA: 0x00060DC8 File Offset: 0x0005EFC8
		public MoonParticle3D(MountainModel model, Vector3 center)
		{
			this.model = model;
			this.Visible = false;
			Matrix matrix = Matrix.CreateRotationZ(0.4f);
			Color[] choices = new Color[]
			{
				Calc.HexToColor("53f3dd"),
				Calc.HexToColor("53c9f3")
			};
			for (int i = 0; i < 20; i++)
			{
				base.Add(new MoonParticle3D.Particle(OVR.Atlas["star"], Calc.Random.Choose(choices), center, 1f, matrix));
			}
			for (int j = 0; j < 30; j++)
			{
				base.Add(new MoonParticle3D.Particle(OVR.Atlas["snow"], Calc.Random.Choose(choices), center, 0.3f, matrix));
			}
			Matrix matrix2 = Matrix.CreateRotationZ(0.8f) * Matrix.CreateRotationX(0.4f);
			Color[] choices2 = new Color[]
			{
				Calc.HexToColor("ab6ffa"),
				Calc.HexToColor("fa70ea")
			};
			for (int k = 0; k < 20; k++)
			{
				base.Add(new MoonParticle3D.Particle(OVR.Atlas["star"], Calc.Random.Choose(choices2), center, 1f, matrix2));
			}
			for (int l = 0; l < 30; l++)
			{
				base.Add(new MoonParticle3D.Particle(OVR.Atlas["snow"], Calc.Random.Choose(choices2), center, 0.3f, matrix2));
			}
		}

		// Token: 0x0600126A RID: 4714 RVA: 0x00060F5D File Offset: 0x0005F15D
		public override void Update()
		{
			base.Update();
			this.Visible = (this.model.StarEase > 0f);
		}

		// Token: 0x04000E21 RID: 3617
		private MountainModel model;

		// Token: 0x04000E22 RID: 3618
		private List<MoonParticle3D.Particle> particles = new List<MoonParticle3D.Particle>();

		// Token: 0x0200056A RID: 1386
		public class Particle : Billboard
		{
			// Token: 0x06002685 RID: 9861 RVA: 0x000FD700 File Offset: 0x000FB900
			public Particle(MTexture texture, Color color, Vector3 center, float size, Matrix matrix) : base(texture, Vector3.Zero, null, new Color?(color), null)
			{
				this.Center = center;
				this.Matrix = matrix;
				this.Size = Vector2.One * Calc.Random.Range(0.05f, 0.15f) * size;
				this.Distance = Calc.Random.Range(1.8f, 1.9f);
				this.Rotation = Calc.Random.NextFloat(6.2831855f);
				this.YOff = Calc.Random.Range(-0.1f, 0.1f);
				this.Spd = Calc.Random.Range(0.8f, 1.2f);
			}

			// Token: 0x06002686 RID: 9862 RVA: 0x000FD7D0 File Offset: 0x000FB9D0
			public override void Update()
			{
				this.Rotation += Engine.DeltaTime * 0.4f * this.Spd;
				Vector3 position = new Vector3((float)Math.Cos((double)this.Rotation) * this.Distance, (float)Math.Sin((double)(this.Rotation * 3f)) * 0.25f + this.YOff, (float)Math.Sin((double)this.Rotation) * this.Distance);
				this.Position = this.Center + Vector3.Transform(position, this.Matrix);
			}

			// Token: 0x0400266A RID: 9834
			public Vector3 Center;

			// Token: 0x0400266B RID: 9835
			public Matrix Matrix;

			// Token: 0x0400266C RID: 9836
			public float Rotation;

			// Token: 0x0400266D RID: 9837
			public float Distance;

			// Token: 0x0400266E RID: 9838
			public float YOff;

			// Token: 0x0400266F RID: 9839
			public float Spd;
		}
	}
}
