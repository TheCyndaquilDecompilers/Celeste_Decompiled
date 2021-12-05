using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020001E0 RID: 480
	[Pooled]
	[Tracked(false)]
	public class SpeedRing : Entity
	{
		// Token: 0x0600101E RID: 4126 RVA: 0x00045929 File Offset: 0x00043B29
		public SpeedRing Init(Vector2 position, float angle, Color color)
		{
			this.Position = position;
			this.angle = angle;
			this.color = color;
			this.lerp = 0f;
			this.normal = Calc.AngleToVector(angle, 1f);
			return this;
		}

		// Token: 0x0600101F RID: 4127 RVA: 0x00045960 File Offset: 0x00043B60
		public override void Update()
		{
			this.lerp += 3f * Engine.DeltaTime;
			this.Position += this.normal * 10f * Engine.DeltaTime;
			if (this.lerp >= 1f)
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06001020 RID: 4128 RVA: 0x000459C4 File Offset: 0x00043BC4
		public override void Render()
		{
			Color color = this.color * MathHelper.Lerp(0.6f, 0f, this.lerp);
			if (color.A > 0)
			{
				Draw.SpriteBatch.Draw(GameplayBuffers.SpeedRings, this.Position + new Vector2(-32f, -32f), new Rectangle?(new Rectangle(this.index % 4 * 64, this.index / 4 * 64, 64, 64)), color);
			}
		}

		// Token: 0x06001021 RID: 4129 RVA: 0x00045A50 File Offset: 0x00043C50
		private void DrawRing(Vector2 position)
		{
			float maxRadius = MathHelper.Lerp(4f, 14f, this.lerp);
			Vector2 value = this.GetVectorAtAngle(0f, maxRadius);
			for (int i = 1; i <= 8; i++)
			{
				float radians = (float)i * 0.3926991f;
				Vector2 vectorAtAngle = this.GetVectorAtAngle(radians, maxRadius);
				Draw.Line(position + value, position + vectorAtAngle, Color.White);
				Draw.Line(position - value, position - vectorAtAngle, Color.White);
				value = vectorAtAngle;
			}
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x00045AD4 File Offset: 0x00043CD4
		private Vector2 GetVectorAtAngle(float radians, float maxRadius)
		{
			Vector2 vector = Calc.AngleToVector(radians, 1f);
			float scaleFactor = MathHelper.Lerp(maxRadius, maxRadius * 0.5f, Math.Abs(Vector2.Dot(vector, this.normal)));
			return vector * scaleFactor;
		}

		// Token: 0x06001023 RID: 4131 RVA: 0x00045B14 File Offset: 0x00043D14
		public static void DrawToBuffer(Level level)
		{
			List<Entity> entities = level.Tracker.GetEntities<SpeedRing>();
			int num = 0;
			if (entities.Count > 0)
			{
				Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.SpeedRings);
				Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
				foreach (Entity entity in entities)
				{
					SpeedRing speedRing = (SpeedRing)entity;
					speedRing.index = num;
					speedRing.DrawRing(new Vector2((float)(num % 4 * 64 + 32), (float)(num / 4 * 64 + 32)));
					num++;
				}
				Draw.SpriteBatch.End();
			}
		}

		// Token: 0x04000B7F RID: 2943
		private int index;

		// Token: 0x04000B80 RID: 2944
		private float angle;

		// Token: 0x04000B81 RID: 2945
		private float lerp;

		// Token: 0x04000B82 RID: 2946
		private Color color;

		// Token: 0x04000B83 RID: 2947
		private Vector2 normal;
	}
}
