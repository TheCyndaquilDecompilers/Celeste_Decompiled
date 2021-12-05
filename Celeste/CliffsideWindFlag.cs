using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001B2 RID: 434
	public class CliffsideWindFlag : Entity
	{
		// Token: 0x06000F23 RID: 3875 RVA: 0x0003C95C File Offset: 0x0003AB5C
		public CliffsideWindFlag(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			MTexture atlasSubtexturesAt = GFX.Game.GetAtlasSubtexturesAt("scenery/cliffside/flag", data.Int("index", 0));
			this.segments = new CliffsideWindFlag.Segment[atlasSubtexturesAt.Width];
			for (int i = 0; i < this.segments.Length; i++)
			{
				CliffsideWindFlag.Segment segment = new CliffsideWindFlag.Segment();
				segment.Texture = atlasSubtexturesAt.GetSubtexture(i, 0, 1, atlasSubtexturesAt.Height, null);
				segment.Offset = new Vector2((float)i, 0f);
				this.segments[i] = segment;
			}
			this.sine = Calc.Random.NextFloat(6.2831855f);
			this.random = Calc.Random.NextFloat();
			base.Depth = 8999;
			base.Tag = Tags.TransitionUpdate;
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000F24 RID: 3876 RVA: 0x0003CA32 File Offset: 0x0003AC32
		private float wind
		{
			get
			{
				return Calc.ClampedMap(Math.Abs((base.Scene as Level).Wind.X), 0f, 800f, 0f, 1f);
			}
		}

		// Token: 0x06000F25 RID: 3877 RVA: 0x0003CA68 File Offset: 0x0003AC68
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.sign = 1;
			if (this.wind != 0f)
			{
				this.sign = Math.Sign((base.Scene as Level).Wind.X);
			}
			for (int i = 0; i < this.segments.Length; i++)
			{
				this.SetFlagSegmentPosition(i, true);
			}
		}

		// Token: 0x06000F26 RID: 3878 RVA: 0x0003CACC File Offset: 0x0003ACCC
		public override void Update()
		{
			base.Update();
			if (this.wind != 0f)
			{
				this.sign = Math.Sign((base.Scene as Level).Wind.X);
			}
			this.sine += Engine.DeltaTime * (4f + this.wind * 4f) * (0.8f + this.random * 0.2f);
			for (int i = 0; i < this.segments.Length; i++)
			{
				this.SetFlagSegmentPosition(i, false);
			}
		}

		// Token: 0x06000F27 RID: 3879 RVA: 0x0003CB5F File Offset: 0x0003AD5F
		private float Sin(float timer)
		{
			return (float)Math.Sin((double)(-(double)timer));
		}

		// Token: 0x06000F28 RID: 3880 RVA: 0x0003CB6C File Offset: 0x0003AD6C
		private void SetFlagSegmentPosition(int i, bool snap)
		{
			CliffsideWindFlag.Segment segment = this.segments[i];
			float value = (float)(i * this.sign) * (0.2f + this.wind * 0.8f * (0.8f + this.random * 0.2f)) * (0.9f + this.Sin(this.sine) * 0.1f);
			float num = Calc.LerpClamp(this.Sin(this.sine * 0.5f - (float)i * 0.1f) * ((float)i / (float)this.segments.Length) * (float)i * 0.2f, value, (float)Math.Ceiling((double)this.wind));
			float num2 = (float)i / (float)this.segments.Length * Math.Max(0.1f, 1f - this.wind) * 16f;
			if (!snap)
			{
				segment.Offset.X = Calc.Approach(segment.Offset.X, num, Engine.DeltaTime * 40f);
				segment.Offset.Y = Calc.Approach(segment.Offset.Y, num2, Engine.DeltaTime * 40f);
				return;
			}
			segment.Offset.X = num;
			segment.Offset.Y = num2;
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x0003CCA4 File Offset: 0x0003AEA4
		public override void Render()
		{
			base.Render();
			for (int i = 0; i < this.segments.Length; i++)
			{
				CliffsideWindFlag.Segment segment = this.segments[i];
				float scaleFactor = (float)i / (float)this.segments.Length * this.Sin((float)(-(float)i) * 0.1f + this.sine) * 2f;
				segment.Texture.Draw(this.Position + segment.Offset + Vector2.UnitY * scaleFactor);
			}
		}

		// Token: 0x04000A84 RID: 2692
		private CliffsideWindFlag.Segment[] segments;

		// Token: 0x04000A85 RID: 2693
		private float sine;

		// Token: 0x04000A86 RID: 2694
		private float random;

		// Token: 0x04000A87 RID: 2695
		private int sign;

		// Token: 0x020004C0 RID: 1216
		private class Segment
		{
			// Token: 0x0400236C RID: 9068
			public MTexture Texture;

			// Token: 0x0400236D RID: 9069
			public Vector2 Offset;
		}
	}
}
