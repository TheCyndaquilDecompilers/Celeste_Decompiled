using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000358 RID: 856
	public abstract class Backdrop
	{
		// Token: 0x06001AEE RID: 6894 RVA: 0x000AEFD8 File Offset: 0x000AD1D8
		public Backdrop()
		{
			this.Visible = true;
		}

		// Token: 0x06001AEF RID: 6895 RVA: 0x000AF03C File Offset: 0x000AD23C
		public bool IsVisible(Level level)
		{
			return this.ForceVisible || ((string.IsNullOrEmpty(this.OnlyIfNotFlag) || !level.Session.GetFlag(this.OnlyIfNotFlag)) && ((!string.IsNullOrEmpty(this.AlsoIfFlag) && level.Session.GetFlag(this.AlsoIfFlag)) || ((this.Dreaming == null || this.Dreaming.Value == level.Session.Dreaming) && (string.IsNullOrEmpty(this.OnlyIfFlag) || level.Session.GetFlag(this.OnlyIfFlag)) && (this.ExcludeFrom == null || !this.ExcludeFrom.Contains(level.Session.Level)) && (this.OnlyIn == null || this.OnlyIn.Contains(level.Session.Level)))));
		}

		// Token: 0x06001AF0 RID: 6896 RVA: 0x000AF128 File Offset: 0x000AD328
		public virtual void Update(Scene scene)
		{
			Level level = scene as Level;
			if (level.Transitioning)
			{
				if (this.InstantIn && this.IsVisible(level))
				{
					this.Visible = true;
				}
				if (this.InstantOut && !this.IsVisible(level))
				{
					this.Visible = false;
					return;
				}
			}
			else
			{
				this.Visible = this.IsVisible(level);
			}
		}

		// Token: 0x06001AF1 RID: 6897 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void BeforeRender(Scene scene)
		{
		}

		// Token: 0x06001AF2 RID: 6898 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void Render(Scene scene)
		{
		}

		// Token: 0x06001AF3 RID: 6899 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void Ended(Scene scene)
		{
		}

		// Token: 0x04001795 RID: 6037
		public bool UseSpritebatch = true;

		// Token: 0x04001796 RID: 6038
		public string Name;

		// Token: 0x04001797 RID: 6039
		public HashSet<string> Tags = new HashSet<string>();

		// Token: 0x04001798 RID: 6040
		public Vector2 Position;

		// Token: 0x04001799 RID: 6041
		public Vector2 Scroll = Vector2.One;

		// Token: 0x0400179A RID: 6042
		public Vector2 Speed;

		// Token: 0x0400179B RID: 6043
		public Color Color = Color.White;

		// Token: 0x0400179C RID: 6044
		public bool LoopX = true;

		// Token: 0x0400179D RID: 6045
		public bool LoopY = true;

		// Token: 0x0400179E RID: 6046
		public bool FlipX;

		// Token: 0x0400179F RID: 6047
		public bool FlipY;

		// Token: 0x040017A0 RID: 6048
		public Backdrop.Fader FadeX;

		// Token: 0x040017A1 RID: 6049
		public Backdrop.Fader FadeY;

		// Token: 0x040017A2 RID: 6050
		public float FadeAlphaMultiplier = 1f;

		// Token: 0x040017A3 RID: 6051
		public float WindMultiplier;

		// Token: 0x040017A4 RID: 6052
		public HashSet<string> ExcludeFrom;

		// Token: 0x040017A5 RID: 6053
		public HashSet<string> OnlyIn;

		// Token: 0x040017A6 RID: 6054
		public string OnlyIfFlag;

		// Token: 0x040017A7 RID: 6055
		public string OnlyIfNotFlag;

		// Token: 0x040017A8 RID: 6056
		public string AlsoIfFlag;

		// Token: 0x040017A9 RID: 6057
		public bool? Dreaming;

		// Token: 0x040017AA RID: 6058
		public bool Visible;

		// Token: 0x040017AB RID: 6059
		public bool InstantIn = true;

		// Token: 0x040017AC RID: 6060
		public bool InstantOut;

		// Token: 0x040017AD RID: 6061
		public bool ForceVisible;

		// Token: 0x040017AE RID: 6062
		public BackdropRenderer Renderer;

		// Token: 0x02000715 RID: 1813
		public class Fader
		{
			// Token: 0x06002E2C RID: 11820 RVA: 0x00123994 File Offset: 0x00121B94
			public Backdrop.Fader Add(float posFrom, float posTo, float fadeFrom, float fadeTo)
			{
				this.Segments.Add(new Backdrop.Fader.Segment
				{
					PositionFrom = posFrom,
					PositionTo = posTo,
					From = fadeFrom,
					To = fadeTo
				});
				return this;
			}

			// Token: 0x06002E2D RID: 11821 RVA: 0x001239D8 File Offset: 0x00121BD8
			public float Value(float position)
			{
				float num = 1f;
				foreach (Backdrop.Fader.Segment segment in this.Segments)
				{
					num *= Calc.ClampedMap(position, segment.PositionFrom, segment.PositionTo, segment.From, segment.To);
				}
				return num;
			}

			// Token: 0x04002DA0 RID: 11680
			private List<Backdrop.Fader.Segment> Segments = new List<Backdrop.Fader.Segment>();

			// Token: 0x02000798 RID: 1944
			private struct Segment
			{
				// Token: 0x04002FC3 RID: 12227
				public float PositionFrom;

				// Token: 0x04002FC4 RID: 12228
				public float PositionTo;

				// Token: 0x04002FC5 RID: 12229
				public float From;

				// Token: 0x04002FC6 RID: 12230
				public float To;
			}
		}
	}
}
