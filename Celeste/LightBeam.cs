using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002E9 RID: 745
	public class LightBeam : Entity
	{
		// Token: 0x060016F5 RID: 5877 RVA: 0x0008AF6C File Offset: 0x0008916C
		public LightBeam(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Tag = Tags.TransitionUpdate;
			base.Depth = -9998;
			this.LightWidth = data.Width;
			this.LightLength = data.Height;
			this.Flag = data.Attr("flag", "");
			this.Rotation = data.Float("rotation", 0f) * 0.017453292f;
		}

		// Token: 0x060016F6 RID: 5878 RVA: 0x0008B034 File Offset: 0x00089234
		public override void Update()
		{
			this.timer += Engine.DeltaTime;
			Level level = base.Scene as Level;
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null && (string.IsNullOrEmpty(this.Flag) || level.Session.GetFlag(this.Flag)))
			{
				Vector2 value = Calc.AngleToVector(this.Rotation + 1.5707964f, 1f);
				Vector2 value2 = Calc.ClosestPointOnLine(this.Position, this.Position + value * 10000f, entity.Center);
				float target = Math.Min(1f, Math.Max(0f, (value2 - this.Position).Length() - 8f) / (float)this.LightLength);
				if ((value2 - entity.Center).Length() > (float)this.LightWidth / 2f)
				{
					target = 1f;
				}
				if (level.Transitioning)
				{
					target = 0f;
				}
				this.alpha = Calc.Approach(this.alpha, target, Engine.DeltaTime * 4f);
			}
			if (this.alpha >= 0.5f && level.OnInterval(0.8f))
			{
				Vector2 vector = Calc.AngleToVector(this.Rotation + 1.5707964f, 1f);
				Vector2 vector2 = this.Position - vector * 4f;
				float scaleFactor = (float)(Calc.Random.Next(this.LightWidth - 4) + 2 - this.LightWidth / 2);
				vector2 += scaleFactor * vector.Perpendicular();
				level.Particles.Emit(LightBeam.P_Glow, vector2, this.Rotation + 1.5707964f);
			}
			base.Update();
		}

		// Token: 0x060016F7 RID: 5879 RVA: 0x0008B218 File Offset: 0x00089418
		public override void Render()
		{
			if (this.alpha > 0f)
			{
				this.DrawTexture(0f, (float)this.LightWidth, (float)(this.LightLength - 4) + (float)Math.Sin((double)(this.timer * 2f)) * 4f, 0.4f);
				for (int i = 0; i < this.LightWidth; i += 4)
				{
					float num = this.timer + (float)i * 0.6f;
					float num2 = 4f + (float)Math.Sin((double)(num * 0.5f + 1.2f)) * 4f;
					float offset = (float)Math.Sin((double)((num + (float)(i * 32)) * 0.1f) + Math.Sin((double)(num * 0.05f + (float)i * 0.1f)) * 0.25) * ((float)this.LightWidth / 2f - num2 / 2f);
					float length = (float)this.LightLength + (float)Math.Sin((double)(num * 0.25f)) * 8f;
					float a = 0.6f + (float)Math.Sin((double)(num + 0.8f)) * 0.3f;
					this.DrawTexture(offset, num2, length, a);
				}
			}
		}

		// Token: 0x060016F8 RID: 5880 RVA: 0x0008B34C File Offset: 0x0008954C
		private void DrawTexture(float offset, float width, float length, float a)
		{
			float rotation = this.Rotation + 1.5707964f;
			if (width >= 1f)
			{
				this.texture.Draw(this.Position + Calc.AngleToVector(this.Rotation, 1f) * offset, new Vector2(0f, 0.5f), this.color * a * this.alpha, new Vector2(1f / (float)this.texture.Width * length, width), rotation);
			}
		}

		// Token: 0x04001382 RID: 4994
		public static ParticleType P_Glow;

		// Token: 0x04001383 RID: 4995
		private MTexture texture = GFX.Game["util/lightbeam"];

		// Token: 0x04001384 RID: 4996
		private Color color = new Color(0.8f, 1f, 1f);

		// Token: 0x04001385 RID: 4997
		private float alpha;

		// Token: 0x04001386 RID: 4998
		public int LightWidth;

		// Token: 0x04001387 RID: 4999
		public int LightLength;

		// Token: 0x04001388 RID: 5000
		public float Rotation;

		// Token: 0x04001389 RID: 5001
		public string Flag;

		// Token: 0x0400138A RID: 5002
		private float timer = Calc.Random.NextFloat(1000f);
	}
}
