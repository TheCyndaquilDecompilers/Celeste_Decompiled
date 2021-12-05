using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200025A RID: 602
	public class AreaCompleteTitle : Entity
	{
		// Token: 0x060012BF RID: 4799 RVA: 0x00065850 File Offset: 0x00063A50
		public AreaCompleteTitle(Vector2 origin, string text, float scale, bool rainbow = false)
		{
			this.origin = origin;
			this.scale = scale;
			Vector2 vector = ActiveFont.Measure(text) * scale;
			Vector2 value = origin + Vector2.UnitY * vector.Y * 0.5f + Vector2.UnitX * vector.X * -0.5f;
			for (int i = 0; i < text.Length; i++)
			{
				Vector2 vector2 = ActiveFont.Measure(text[i].ToString()) * scale;
				if (text[i] != ' ')
				{
					AreaCompleteTitle.Letter letter = new AreaCompleteTitle.Letter(i, text[i].ToString(), value + Vector2.UnitX * vector2.X * 0.5f);
					if (rainbow)
					{
						float hue = (float)i / (float)text.Length;
						letter.Color = Calc.HsvToColor(hue, 0.8f, 0.9f);
						letter.Shadow = Color.Lerp(letter.Color, Color.Black, 0.7f);
					}
					this.letters.Add(letter);
				}
				value += Vector2.UnitX * vector2.X;
			}
			Alarm.Set(this, 2.6f, delegate
			{
				Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineOut, 0.5f, true);
				tween.OnUpdate = delegate(Tween t)
				{
					this.rectangleEase = t.Eased;
				};
				base.Add(tween);
			}, Alarm.AlarmMode.Oneshot);
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x000659D0 File Offset: 0x00063BD0
		public override void Update()
		{
			base.Update();
			foreach (AreaCompleteTitle.Letter letter in this.letters)
			{
				letter.Update();
			}
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x00065A28 File Offset: 0x00063C28
		public void DrawLineUI()
		{
			Draw.Rect(base.X, base.Y + this.origin.Y - 40f, 1920f * this.rectangleEase, 80f, Color.Black * 0.65f);
		}

		// Token: 0x060012C2 RID: 4802 RVA: 0x00065A78 File Offset: 0x00063C78
		public override void Render()
		{
			base.Render();
			foreach (AreaCompleteTitle.Letter letter in this.letters)
			{
				letter.Render(this.Position, this.scale, this.Alpha);
			}
		}

		// Token: 0x04000EBE RID: 3774
		public float Alpha = 1f;

		// Token: 0x04000EBF RID: 3775
		private Vector2 origin;

		// Token: 0x04000EC0 RID: 3776
		private List<AreaCompleteTitle.Letter> letters = new List<AreaCompleteTitle.Letter>();

		// Token: 0x04000EC1 RID: 3777
		private float rectangleEase;

		// Token: 0x04000EC2 RID: 3778
		private float scale;

		// Token: 0x0200057A RID: 1402
		public class Letter
		{
			// Token: 0x060026D2 RID: 9938 RVA: 0x000FF3DC File Offset: 0x000FD5DC
			public Letter(int index, string value, Vector2 position)
			{
				this.Value = value;
				this.Position = position;
				this.delay = 0.2f + (float)index * 0.02f;
				this.curve = new SimpleCurve(position + Vector2.UnitY * 60f, position, position - Vector2.UnitY * 100f);
				this.scale = new Vector2(0.75f, 1.5f);
			}

			// Token: 0x060026D3 RID: 9939 RVA: 0x000FF474 File Offset: 0x000FD674
			public void Update()
			{
				this.scale.X = Calc.Approach(this.scale.X, 1f, 3f * Engine.DeltaTime);
				this.scale.Y = Calc.Approach(this.scale.Y, 1f, 3f * Engine.DeltaTime);
				if (this.delay > 0f)
				{
					this.delay -= Engine.DeltaTime;
					return;
				}
				if (this.ease < 1f)
				{
					this.ease += 4f * Engine.DeltaTime;
					if (this.ease >= 1f)
					{
						this.ease = 1f;
						this.scale = new Vector2(1.5f, 0.75f);
					}
				}
			}

			// Token: 0x060026D4 RID: 9940 RVA: 0x000FF54C File Offset: 0x000FD74C
			public void Render(Vector2 offset, float scale, float alphaMultiplier)
			{
				if (this.ease > 0f)
				{
					Vector2 vector = offset + this.curve.GetPoint(this.ease);
					float num = Calc.LerpClamp(0f, 1f, this.ease * 3f) * alphaMultiplier;
					Vector2 vector2 = this.scale * scale;
					if (num < 1f)
					{
						ActiveFont.Draw(this.Value, vector, new Vector2(0.5f, 1f), vector2, this.Color * num);
						return;
					}
					ActiveFont.Draw(this.Value, vector + Vector2.UnitY * 3.5f * scale, new Vector2(0.5f, 1f), vector2, this.Shadow);
					ActiveFont.DrawOutline(this.Value, vector, new Vector2(0.5f, 1f), vector2, this.Color, 2f, this.Shadow);
				}
			}

			// Token: 0x040026B5 RID: 9909
			public string Value;

			// Token: 0x040026B6 RID: 9910
			public Vector2 Position;

			// Token: 0x040026B7 RID: 9911
			public Color Color = Color.White;

			// Token: 0x040026B8 RID: 9912
			public Color Shadow = Color.Black;

			// Token: 0x040026B9 RID: 9913
			private float delay;

			// Token: 0x040026BA RID: 9914
			private float ease;

			// Token: 0x040026BB RID: 9915
			private Vector2 scale;

			// Token: 0x040026BC RID: 9916
			private SimpleCurve curve;
		}
	}
}
