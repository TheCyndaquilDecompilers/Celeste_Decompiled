using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x02000119 RID: 281
	public class ParticleType
	{
		// Token: 0x060008DA RID: 2266 RVA: 0x00014DEC File Offset: 0x00012FEC
		public ParticleType()
		{
			this.Color = (this.Color2 = Color.White);
			this.ColorMode = ParticleType.ColorModes.Static;
			this.FadeMode = ParticleType.FadeModes.None;
			this.SpeedMin = (this.SpeedMax = 0f);
			this.SpeedMultiplier = 1f;
			this.Acceleration = Vector2.Zero;
			this.Friction = 0f;
			this.Direction = (this.DirectionRange = 0f);
			this.LifeMin = (this.LifeMax = 0f);
			this.Size = 2f;
			this.SizeRange = 0f;
			this.SpinMin = (this.SpinMax = 0f);
			this.SpinFlippedChance = false;
			this.RotationMode = ParticleType.RotationModes.None;
			ParticleType.AllTypes.Add(this);
		}

		// Token: 0x060008DB RID: 2267 RVA: 0x00014EC4 File Offset: 0x000130C4
		public ParticleType(ParticleType copyFrom)
		{
			this.Source = copyFrom.Source;
			this.SourceChooser = copyFrom.SourceChooser;
			this.Color = copyFrom.Color;
			this.Color2 = copyFrom.Color2;
			this.ColorMode = copyFrom.ColorMode;
			this.FadeMode = copyFrom.FadeMode;
			this.SpeedMin = copyFrom.SpeedMin;
			this.SpeedMax = copyFrom.SpeedMax;
			this.SpeedMultiplier = copyFrom.SpeedMultiplier;
			this.Acceleration = copyFrom.Acceleration;
			this.Friction = copyFrom.Friction;
			this.Direction = copyFrom.Direction;
			this.DirectionRange = copyFrom.DirectionRange;
			this.LifeMin = copyFrom.LifeMin;
			this.LifeMax = copyFrom.LifeMax;
			this.Size = copyFrom.Size;
			this.SizeRange = copyFrom.SizeRange;
			this.RotationMode = copyFrom.RotationMode;
			this.SpinMin = copyFrom.SpinMin;
			this.SpinMax = copyFrom.SpinMax;
			this.SpinFlippedChance = copyFrom.SpinFlippedChance;
			this.ScaleOut = copyFrom.ScaleOut;
			this.UseActualDeltaTime = copyFrom.UseActualDeltaTime;
			ParticleType.AllTypes.Add(this);
		}

		// Token: 0x060008DC RID: 2268 RVA: 0x00014FF6 File Offset: 0x000131F6
		public Particle Create(ref Particle particle, Vector2 position)
		{
			return this.Create(ref particle, position, this.Direction);
		}

		// Token: 0x060008DD RID: 2269 RVA: 0x00015006 File Offset: 0x00013206
		public Particle Create(ref Particle particle, Vector2 position, Color color)
		{
			return this.Create(ref particle, null, position, this.Direction, color);
		}

		// Token: 0x060008DE RID: 2270 RVA: 0x00015018 File Offset: 0x00013218
		public Particle Create(ref Particle particle, Vector2 position, float direction)
		{
			return this.Create(ref particle, null, position, direction, this.Color);
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x0001502A File Offset: 0x0001322A
		public Particle Create(ref Particle particle, Vector2 position, Color color, float direction)
		{
			return this.Create(ref particle, null, position, direction, color);
		}

		// Token: 0x060008E0 RID: 2272 RVA: 0x00015038 File Offset: 0x00013238
		public Particle Create(ref Particle particle, Entity entity, Vector2 position, float direction, Color color)
		{
			particle.Track = entity;
			particle.Type = this;
			particle.Active = true;
			particle.Position = position;
			if (this.SourceChooser != null)
			{
				particle.Source = this.SourceChooser.Choose();
			}
			else if (this.Source != null)
			{
				particle.Source = this.Source;
			}
			else
			{
				particle.Source = Draw.Particle;
			}
			if (this.SizeRange != 0f)
			{
				particle.StartSize = (particle.Size = this.Size - this.SizeRange * 0.5f + Calc.Random.NextFloat(this.SizeRange));
			}
			else
			{
				particle.StartSize = (particle.Size = this.Size);
			}
			if (this.ColorMode == ParticleType.ColorModes.Choose)
			{
				particle.StartColor = (particle.Color = Calc.Random.Choose(color, this.Color2));
			}
			else
			{
				particle.Color = color;
				particle.StartColor = color;
			}
			float num = direction - this.DirectionRange / 2f + Calc.Random.NextFloat() * this.DirectionRange;
			particle.Speed = Calc.AngleToVector(num, Calc.Random.Range(this.SpeedMin, this.SpeedMax));
			particle.StartLife = (particle.Life = Calc.Random.Range(this.LifeMin, this.LifeMax));
			if (this.RotationMode == ParticleType.RotationModes.Random)
			{
				particle.Rotation = Calc.Random.NextAngle();
			}
			else if (this.RotationMode == ParticleType.RotationModes.SameAsDirection)
			{
				particle.Rotation = num;
			}
			else
			{
				particle.Rotation = 0f;
			}
			particle.Spin = Calc.Random.Range(this.SpinMin, this.SpinMax);
			if (this.SpinFlippedChance)
			{
				particle.Spin *= (float)Calc.Random.Choose(1, -1);
			}
			return particle;
		}

		// Token: 0x040005E5 RID: 1509
		private static List<ParticleType> AllTypes = new List<ParticleType>();

		// Token: 0x040005E6 RID: 1510
		public MTexture Source;

		// Token: 0x040005E7 RID: 1511
		public Chooser<MTexture> SourceChooser;

		// Token: 0x040005E8 RID: 1512
		public Color Color;

		// Token: 0x040005E9 RID: 1513
		public Color Color2;

		// Token: 0x040005EA RID: 1514
		public ParticleType.ColorModes ColorMode;

		// Token: 0x040005EB RID: 1515
		public ParticleType.FadeModes FadeMode;

		// Token: 0x040005EC RID: 1516
		public float SpeedMin;

		// Token: 0x040005ED RID: 1517
		public float SpeedMax;

		// Token: 0x040005EE RID: 1518
		public float SpeedMultiplier;

		// Token: 0x040005EF RID: 1519
		public Vector2 Acceleration;

		// Token: 0x040005F0 RID: 1520
		public float Friction;

		// Token: 0x040005F1 RID: 1521
		public float Direction;

		// Token: 0x040005F2 RID: 1522
		public float DirectionRange;

		// Token: 0x040005F3 RID: 1523
		public float LifeMin;

		// Token: 0x040005F4 RID: 1524
		public float LifeMax;

		// Token: 0x040005F5 RID: 1525
		public float Size;

		// Token: 0x040005F6 RID: 1526
		public float SizeRange;

		// Token: 0x040005F7 RID: 1527
		public float SpinMin;

		// Token: 0x040005F8 RID: 1528
		public float SpinMax;

		// Token: 0x040005F9 RID: 1529
		public bool SpinFlippedChance;

		// Token: 0x040005FA RID: 1530
		public ParticleType.RotationModes RotationMode;

		// Token: 0x040005FB RID: 1531
		public bool ScaleOut;

		// Token: 0x040005FC RID: 1532
		public bool UseActualDeltaTime;

		// Token: 0x020003AF RID: 943
		public enum ColorModes
		{
			// Token: 0x04001F3A RID: 7994
			Static,
			// Token: 0x04001F3B RID: 7995
			Choose,
			// Token: 0x04001F3C RID: 7996
			Blink,
			// Token: 0x04001F3D RID: 7997
			Fade
		}

		// Token: 0x020003B0 RID: 944
		public enum FadeModes
		{
			// Token: 0x04001F3F RID: 7999
			None,
			// Token: 0x04001F40 RID: 8000
			Linear,
			// Token: 0x04001F41 RID: 8001
			Late,
			// Token: 0x04001F42 RID: 8002
			InAndOut
		}

		// Token: 0x020003B1 RID: 945
		public enum RotationModes
		{
			// Token: 0x04001F44 RID: 8004
			None,
			// Token: 0x04001F45 RID: 8005
			Random,
			// Token: 0x04001F46 RID: 8006
			SameAsDirection
		}
	}
}
