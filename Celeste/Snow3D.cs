using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000248 RID: 584
	public class Snow3D : Entity
	{
		// Token: 0x0600126B RID: 4715 RVA: 0x00060F80 File Offset: 0x0005F180
		public Snow3D(MountainModel model)
		{
			this.Model = model;
			for (int i = 0; i < Snow3D.alphas.Length; i++)
			{
				Snow3D.alphas[i] = Color.White * ((float)i / (float)Snow3D.alphas.Length);
			}
			for (int j = 0; j < 400; j++)
			{
				Snow3D.Particle particle = new Snow3D.Particle(this, 1f);
				this.particles.Add(particle);
				base.Add(particle);
			}
		}

		// Token: 0x0600126C RID: 4716 RVA: 0x00061034 File Offset: 0x0005F234
		public override void Update()
		{
			Overworld overworld = base.Scene as Overworld;
			this.Range = 20f;
			if (SaveData.Instance != null && overworld != null && (overworld.IsCurrent<OuiChapterPanel>() || overworld.IsCurrent<OuiChapterSelect>()))
			{
				int id = SaveData.Instance.LastArea.ID;
				if (id == 0 || id == 2 || id == 8)
				{
					this.Range = 3f;
				}
				else if (id == 1)
				{
					this.Range = 12f;
				}
			}
			Matrix matrix = Matrix.CreatePerspectiveFieldOfView(0.98174775f, (float)Engine.Width / (float)Engine.Height, 0.1f, this.Range);
			Matrix matrix2 = Matrix.CreateTranslation(-this.Model.Camera.Position) * Matrix.CreateFromQuaternion(this.Model.Camera.Rotation) * matrix;
			if (base.Scene.OnInterval(0.05f))
			{
				this.LastFrustum.Matrix = matrix2;
			}
			this.Frustum.Matrix = matrix2;
			base.Update();
		}

		// Token: 0x04000E23 RID: 3619
		private static Color[] alphas = new Color[32];

		// Token: 0x04000E24 RID: 3620
		private List<Snow3D.Particle> particles = new List<Snow3D.Particle>();

		// Token: 0x04000E25 RID: 3621
		private BoundingFrustum Frustum = new BoundingFrustum(Matrix.Identity);

		// Token: 0x04000E26 RID: 3622
		private BoundingFrustum LastFrustum = new BoundingFrustum(Matrix.Identity);

		// Token: 0x04000E27 RID: 3623
		private MountainModel Model;

		// Token: 0x04000E28 RID: 3624
		private float Range = 30f;

		// Token: 0x0200056B RID: 1387
		[Tracked(false)]
		public class Particle : Billboard
		{
			// Token: 0x06002687 RID: 9863 RVA: 0x000FD868 File Offset: 0x000FBA68
			public Particle(Snow3D manager, float size) : base(OVR.Atlas["snow"], Vector3.Zero, null, null, null)
			{
				this.Manager = manager;
				this.size = size;
				this.Size = Vector2.One * size;
				this.Reset(Calc.Random.NextFloat());
				this.ResetPosition();
			}

			// Token: 0x06002688 RID: 9864 RVA: 0x000FD8E0 File Offset: 0x000FBAE0
			public void ResetPosition()
			{
				float range = this.Manager.Range;
				this.Position = this.Manager.Model.Camera.Position + this.Manager.Model.Forward * range * 0.5f + new Vector3(Calc.Random.Range(-range, range), Calc.Random.Range(-range, range), Calc.Random.Range(-range, range));
			}

			// Token: 0x06002689 RID: 9865 RVA: 0x000FD96C File Offset: 0x000FBB6C
			public void Reset(float percent = 0f)
			{
				float num = this.Manager.Range / 30f;
				this.Speed = Calc.Random.Range(1f, 6f) * num;
				this.Percent = percent;
				this.Duration = Calc.Random.Range(1f, 5f);
				this.Float = new Vector2((float)Calc.Random.Range(-1, 1), (float)Calc.Random.Range(-1, 1)).SafeNormalize() * 0.25f;
				this.Scale = Vector2.One * 0.05f * num;
			}

			// Token: 0x0600268A RID: 9866 RVA: 0x000FDA18 File Offset: 0x000FBC18
			public override void Update()
			{
				if (this.Percent > 1f || !this.InView())
				{
					this.ResetPosition();
					int num = 0;
					while (!this.InView() && num++ < 10)
					{
						this.ResetPosition();
					}
					if (num > 10)
					{
						this.Color = Color.Transparent;
						return;
					}
					this.Reset((!this.InLastView()) ? Calc.Random.NextFloat() : 0f);
				}
				this.Percent += Engine.DeltaTime / this.Duration;
				float num2 = Calc.YoYo(this.Percent);
				if (this.Manager.Model.SnowForceFloat > 0f)
				{
					num2 *= this.Manager.Model.SnowForceFloat;
				}
				else if (this.Manager.Model.StarEase > 0f)
				{
					num2 *= Calc.Map(this.Manager.Model.StarEase, 0f, 1f, 1f, 0f);
				}
				this.Color = Color.White * num2;
				this.Size.Y = this.size + this.Manager.Model.SnowStretch * (1f - this.Manager.Model.SnowForceFloat);
				this.Position.Y = this.Position.Y - (this.Speed + this.Manager.Model.SnowSpeedAddition) * (1f - this.Manager.Model.SnowForceFloat) * Engine.DeltaTime;
				this.Position.X = this.Position.X + this.Float.X * Engine.DeltaTime;
				this.Position.Z = this.Position.Z + this.Float.Y * Engine.DeltaTime;
			}

			// Token: 0x0600268B RID: 9867 RVA: 0x000FDBEE File Offset: 0x000FBDEE
			private bool InView()
			{
				return this.Manager.Frustum.Contains(this.Position) == ContainmentType.Contains && this.Position.Y > 0f;
			}

			// Token: 0x0600268C RID: 9868 RVA: 0x000FDC1D File Offset: 0x000FBE1D
			private bool InLastView()
			{
				return this.Manager.LastFrustum != null && this.Manager.LastFrustum.Contains(this.Position) == ContainmentType.Contains;
			}

			// Token: 0x04002670 RID: 9840
			public Snow3D Manager;

			// Token: 0x04002671 RID: 9841
			public Vector2 Float;

			// Token: 0x04002672 RID: 9842
			public float Percent;

			// Token: 0x04002673 RID: 9843
			public float Duration;

			// Token: 0x04002674 RID: 9844
			public float Speed;

			// Token: 0x04002675 RID: 9845
			private float size;
		}
	}
}
