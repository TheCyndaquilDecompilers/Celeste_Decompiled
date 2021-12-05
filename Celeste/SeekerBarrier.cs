using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200025E RID: 606
	[Tracked(false)]
	public class SeekerBarrier : Solid
	{
		// Token: 0x060012DB RID: 4827 RVA: 0x00065FCC File Offset: 0x000641CC
		public SeekerBarrier(Vector2 position, float width, float height) : base(position, width, height, false)
		{
			this.Collidable = false;
			int num = 0;
			while ((float)num < base.Width * base.Height / 16f)
			{
				this.particles.Add(new Vector2(Calc.Random.NextFloat(base.Width - 1f), Calc.Random.NextFloat(base.Height - 1f)));
				num++;
			}
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x00066072 File Offset: 0x00064272
		public SeekerBarrier(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height)
		{
		}

		// Token: 0x060012DD RID: 4829 RVA: 0x00066094 File Offset: 0x00064294
		public override void Added(Scene scene)
		{
			base.Added(scene);
			scene.Tracker.GetEntity<SeekerBarrierRenderer>().Track(this);
		}

		// Token: 0x060012DE RID: 4830 RVA: 0x000660AE File Offset: 0x000642AE
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			scene.Tracker.GetEntity<SeekerBarrierRenderer>().Untrack(this);
		}

		// Token: 0x060012DF RID: 4831 RVA: 0x000660C8 File Offset: 0x000642C8
		public override void Update()
		{
			if (this.Flashing)
			{
				this.Flash = Calc.Approach(this.Flash, 0f, Engine.DeltaTime * 4f);
				if (this.Flash <= 0f)
				{
					this.Flashing = false;
				}
			}
			else if (this.solidifyDelay > 0f)
			{
				this.solidifyDelay -= Engine.DeltaTime;
			}
			else if (this.Solidify > 0f)
			{
				this.Solidify = Calc.Approach(this.Solidify, 0f, Engine.DeltaTime);
			}
			int num = this.speeds.Length;
			float height = base.Height;
			int i = 0;
			int count = this.particles.Count;
			while (i < count)
			{
				Vector2 value = this.particles[i] + Vector2.UnitY * this.speeds[i % num] * Engine.DeltaTime;
				value.Y %= height - 1f;
				this.particles[i] = value;
				i++;
			}
			base.Update();
		}

		// Token: 0x060012E0 RID: 4832 RVA: 0x000661DC File Offset: 0x000643DC
		public void OnReflectSeeker()
		{
			this.Flash = 1f;
			this.Solidify = 1f;
			this.solidifyDelay = 1f;
			this.Flashing = true;
			base.Scene.CollideInto<SeekerBarrier>(new Rectangle((int)base.X, (int)base.Y - 2, (int)base.Width, (int)base.Height + 4), this.adjacent);
			base.Scene.CollideInto<SeekerBarrier>(new Rectangle((int)base.X - 2, (int)base.Y, (int)base.Width + 4, (int)base.Height), this.adjacent);
			foreach (SeekerBarrier seekerBarrier in this.adjacent)
			{
				if (!seekerBarrier.Flashing)
				{
					seekerBarrier.OnReflectSeeker();
				}
			}
			this.adjacent.Clear();
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x000662D8 File Offset: 0x000644D8
		public override void Render()
		{
			Color color = Color.White * 0.5f;
			foreach (Vector2 value in this.particles)
			{
				Draw.Pixel.Draw(this.Position + value, Vector2.Zero, color);
			}
			if (this.Flashing)
			{
				Draw.Rect(base.Collider, Color.White * this.Flash * 0.5f);
			}
		}

		// Token: 0x04000ECA RID: 3786
		public float Flash;

		// Token: 0x04000ECB RID: 3787
		public float Solidify;

		// Token: 0x04000ECC RID: 3788
		public bool Flashing;

		// Token: 0x04000ECD RID: 3789
		private float solidifyDelay;

		// Token: 0x04000ECE RID: 3790
		private List<Vector2> particles = new List<Vector2>();

		// Token: 0x04000ECF RID: 3791
		private List<SeekerBarrier> adjacent = new List<SeekerBarrier>();

		// Token: 0x04000ED0 RID: 3792
		private float[] speeds = new float[]
		{
			12f,
			20f,
			40f
		};
	}
}
