using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000156 RID: 342
	public class DeathEffect : Component
	{
		// Token: 0x06000C59 RID: 3161 RVA: 0x00028A48 File Offset: 0x00026C48
		public DeathEffect(Color color, Vector2? offset = null) : base(true, true)
		{
			this.Color = color;
			this.Position = ((offset != null) ? offset.Value : Vector2.Zero);
			this.Percent = 0f;
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x00028A98 File Offset: 0x00026C98
		public override void Update()
		{
			base.Update();
			if (this.Percent > 1f)
			{
				base.RemoveSelf();
				if (this.OnEnd != null)
				{
					this.OnEnd();
				}
			}
			this.Percent = Calc.Approach(this.Percent, 1f, Engine.DeltaTime / this.Duration);
			if (this.OnUpdate != null)
			{
				this.OnUpdate(this.Percent);
			}
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x00028B0C File Offset: 0x00026D0C
		public override void Render()
		{
			DeathEffect.Draw(base.Entity.Position + this.Position, this.Color, this.Percent);
		}

		// Token: 0x06000C5C RID: 3164 RVA: 0x00028B38 File Offset: 0x00026D38
		public static void Draw(Vector2 position, Color color, float ease)
		{
			Color color2 = (Math.Floor((double)(ease * 10f)) % 2.0 == 0.0) ? color : Color.White;
			MTexture mtexture = GFX.Game["characters/player/hair00"];
			float num = (ease < 0.5f) ? (0.5f + ease) : Ease.CubeOut(1f - (ease - 0.5f) * 2f);
			for (int i = 0; i < 8; i++)
			{
				Vector2 value = Calc.AngleToVector(((float)i / 8f + ease * 0.25f) * 6.2831855f, Ease.CubeOut(ease) * 24f);
				mtexture.DrawCentered(position + value + new Vector2(-1f, 0f), Color.Black, new Vector2(num, num));
				mtexture.DrawCentered(position + value + new Vector2(1f, 0f), Color.Black, new Vector2(num, num));
				mtexture.DrawCentered(position + value + new Vector2(0f, -1f), Color.Black, new Vector2(num, num));
				mtexture.DrawCentered(position + value + new Vector2(0f, 1f), Color.Black, new Vector2(num, num));
			}
			for (int j = 0; j < 8; j++)
			{
				Vector2 value2 = Calc.AngleToVector(((float)j / 8f + ease * 0.25f) * 6.2831855f, Ease.CubeOut(ease) * 24f);
				mtexture.DrawCentered(position + value2, color2, new Vector2(num, num));
			}
		}

		// Token: 0x040007BB RID: 1979
		public Vector2 Position;

		// Token: 0x040007BC RID: 1980
		public Color Color;

		// Token: 0x040007BD RID: 1981
		public float Percent;

		// Token: 0x040007BE RID: 1982
		public float Duration = 0.834f;

		// Token: 0x040007BF RID: 1983
		public Action<float> OnUpdate;

		// Token: 0x040007C0 RID: 1984
		public Action OnEnd;
	}
}
