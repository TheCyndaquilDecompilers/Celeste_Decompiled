using System;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020002F4 RID: 756
	public class SpotlightWipe : ScreenWipe
	{
		// Token: 0x0600175E RID: 5982 RVA: 0x0008E570 File Offset: 0x0008C770
		public SpotlightWipe(Scene scene, bool wipeIn, Action onComplete = null) : base(scene, wipeIn, onComplete)
		{
			this.Duration = 1.8f;
			SpotlightWipe.Modifier = 0f;
			if (wipeIn)
			{
				this.sfx = Audio.Play("event:/game/general/spotlight_intro");
				return;
			}
			this.sfx = Audio.Play("event:/game/general/spotlight_outro");
		}

		// Token: 0x0600175F RID: 5983 RVA: 0x0008E5BF File Offset: 0x0008C7BF
		public override void Cancel()
		{
			if (this.sfx != null)
			{
				this.sfx.stop(STOP_MODE.IMMEDIATE);
				this.sfx.release();
				this.sfx = null;
			}
			base.Cancel();
		}

		// Token: 0x06001760 RID: 5984 RVA: 0x0008E5F8 File Offset: 0x0008C7F8
		public override void Render(Scene scene)
		{
			float num = this.WipeIn ? this.Percent : (1f - this.Percent);
			Vector2 focusPoint = SpotlightWipe.FocusPoint;
			if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
			{
				focusPoint.X = 320f - focusPoint.X;
			}
			focusPoint.X *= 6f;
			focusPoint.Y *= 6f;
			float num2 = 288f + SpotlightWipe.Modifier;
			float radius;
			if (!this.Linear)
			{
				if (num < 0.2f)
				{
					radius = Ease.CubeInOut(num / 0.2f) * num2;
				}
				else if (num < 0.8f)
				{
					radius = num2;
				}
				else
				{
					radius = num2 + (num - 0.8f) / 0.2f * (1920f - num2);
				}
			}
			else
			{
				radius = Ease.CubeInOut(num) * 1920f;
			}
			SpotlightWipe.DrawSpotlight(focusPoint, radius, ScreenWipe.WipeColor);
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x0008E6F0 File Offset: 0x0008C8F0
		public static void DrawSpotlight(Vector2 position, float radius, Color color)
		{
			Vector2 value = new Vector2(1f, 0f);
			for (int i = 0; i < SpotlightWipe.vertexBuffer.Length; i += 12)
			{
				Vector2 vector = Calc.AngleToVector(((float)i + 12f) / (float)SpotlightWipe.vertexBuffer.Length * 6.2831855f, 1f);
				SpotlightWipe.vertexBuffer[i].Position = new Vector3(position + value * 5000f, 0f);
				SpotlightWipe.vertexBuffer[i].Color = color;
				SpotlightWipe.vertexBuffer[i + 1].Position = new Vector3(position + value * radius, 0f);
				SpotlightWipe.vertexBuffer[i + 1].Color = color;
				SpotlightWipe.vertexBuffer[i + 2].Position = new Vector3(position + vector * radius, 0f);
				SpotlightWipe.vertexBuffer[i + 2].Color = color;
				SpotlightWipe.vertexBuffer[i + 3].Position = new Vector3(position + value * 5000f, 0f);
				SpotlightWipe.vertexBuffer[i + 3].Color = color;
				SpotlightWipe.vertexBuffer[i + 4].Position = new Vector3(position + vector * 5000f, 0f);
				SpotlightWipe.vertexBuffer[i + 4].Color = color;
				SpotlightWipe.vertexBuffer[i + 5].Position = new Vector3(position + vector * radius, 0f);
				SpotlightWipe.vertexBuffer[i + 5].Color = color;
				SpotlightWipe.vertexBuffer[i + 6].Position = new Vector3(position + value * radius, 0f);
				SpotlightWipe.vertexBuffer[i + 6].Color = color;
				SpotlightWipe.vertexBuffer[i + 7].Position = new Vector3(position + value * (radius - 2f), 0f);
				SpotlightWipe.vertexBuffer[i + 7].Color = Color.Transparent;
				SpotlightWipe.vertexBuffer[i + 8].Position = new Vector3(position + vector * (radius - 2f), 0f);
				SpotlightWipe.vertexBuffer[i + 8].Color = Color.Transparent;
				SpotlightWipe.vertexBuffer[i + 9].Position = new Vector3(position + value * radius, 0f);
				SpotlightWipe.vertexBuffer[i + 9].Color = color;
				SpotlightWipe.vertexBuffer[i + 10].Position = new Vector3(position + vector * radius, 0f);
				SpotlightWipe.vertexBuffer[i + 10].Color = color;
				SpotlightWipe.vertexBuffer[i + 11].Position = new Vector3(position + vector * (radius - 2f), 0f);
				SpotlightWipe.vertexBuffer[i + 11].Color = Color.Transparent;
				value = vector;
			}
			ScreenWipe.DrawPrimitives(SpotlightWipe.vertexBuffer);
		}

		// Token: 0x04001406 RID: 5126
		public static Vector2 FocusPoint;

		// Token: 0x04001407 RID: 5127
		public static float Modifier = 0f;

		// Token: 0x04001408 RID: 5128
		public bool Linear;

		// Token: 0x04001409 RID: 5129
		private const float SmallCircleRadius = 288f;

		// Token: 0x0400140A RID: 5130
		private const float EaseDuration = 1.8f;

		// Token: 0x0400140B RID: 5131
		private const float EaseOpenPercent = 0.2f;

		// Token: 0x0400140C RID: 5132
		private const float EaseClosePercent = 0.2f;

		// Token: 0x0400140D RID: 5133
		private static VertexPositionColor[] vertexBuffer = new VertexPositionColor[768];

		// Token: 0x0400140E RID: 5134
		private EventInstance sfx;
	}
}
