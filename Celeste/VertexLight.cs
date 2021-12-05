using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000372 RID: 882
	[Tracked(false)]
	public class VertexLight : Component
	{
		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06001BAA RID: 7082 RVA: 0x000B7074 File Offset: 0x000B5274
		public Vector2 Center
		{
			get
			{
				return base.Entity.Position + this.position;
			}
		}

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x06001BAB RID: 7083 RVA: 0x000B708C File Offset: 0x000B528C
		// (set) Token: 0x06001BAC RID: 7084 RVA: 0x000B7099 File Offset: 0x000B5299
		public float X
		{
			get
			{
				return this.position.X;
			}
			set
			{
				this.Position = new Vector2(value, this.position.Y);
			}
		}

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06001BAD RID: 7085 RVA: 0x000B70B2 File Offset: 0x000B52B2
		// (set) Token: 0x06001BAE RID: 7086 RVA: 0x000B70BF File Offset: 0x000B52BF
		public float Y
		{
			get
			{
				return this.position.Y;
			}
			set
			{
				this.Position = new Vector2(this.position.X, value);
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x06001BAF RID: 7087 RVA: 0x000B70D8 File Offset: 0x000B52D8
		// (set) Token: 0x06001BB0 RID: 7088 RVA: 0x000B70E0 File Offset: 0x000B52E0
		public Vector2 Position
		{
			get
			{
				return this.position;
			}
			set
			{
				if (this.position != value)
				{
					this.Dirty = true;
					this.position = value;
				}
			}
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06001BB1 RID: 7089 RVA: 0x000B70FE File Offset: 0x000B52FE
		// (set) Token: 0x06001BB2 RID: 7090 RVA: 0x000B7106 File Offset: 0x000B5306
		public float StartRadius
		{
			get
			{
				return this.startRadius;
			}
			set
			{
				if (this.startRadius != value)
				{
					this.Dirty = true;
					this.startRadius = value;
				}
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06001BB3 RID: 7091 RVA: 0x000B711F File Offset: 0x000B531F
		// (set) Token: 0x06001BB4 RID: 7092 RVA: 0x000B7127 File Offset: 0x000B5327
		public float EndRadius
		{
			get
			{
				return this.endRadius;
			}
			set
			{
				if (this.endRadius != value)
				{
					this.Dirty = true;
					this.endRadius = value;
				}
			}
		}

		// Token: 0x06001BB5 RID: 7093 RVA: 0x000B7140 File Offset: 0x000B5340
		public VertexLight() : base(true, true)
		{
		}

		// Token: 0x06001BB6 RID: 7094 RVA: 0x000B719A File Offset: 0x000B539A
		public VertexLight(Color color, float alpha, int startFade, int endFade) : this(Vector2.Zero, color, alpha, startFade, endFade)
		{
		}

		// Token: 0x06001BB7 RID: 7095 RVA: 0x000B71AC File Offset: 0x000B53AC
		public VertexLight(Vector2 position, Color color, float alpha, int startFade, int endFade) : base(true, true)
		{
			this.Position = position;
			this.Color = color;
			this.Alpha = alpha;
			this.StartRadius = (float)startFade;
			this.EndRadius = (float)endFade;
		}

		// Token: 0x06001BB8 RID: 7096 RVA: 0x000B722D File Offset: 0x000B542D
		public override void Added(Entity entity)
		{
			base.Added(entity);
			this.LastNonSolidPosition = this.Center;
			this.LastEntityPosition = base.Entity.Position;
			this.LastPosition = this.Position;
		}

		// Token: 0x06001BB9 RID: 7097 RVA: 0x000B725F File Offset: 0x000B545F
		public override void Update()
		{
			this.InSolidAlphaMultiplier = Calc.Approach(this.InSolidAlphaMultiplier, (float)(this.InSolid ? 0 : 1), Engine.DeltaTime * 4f);
			base.Update();
		}

		// Token: 0x06001BBA RID: 7098 RVA: 0x000B7290 File Offset: 0x000B5490
		public override void HandleGraphicsReset()
		{
			this.Dirty = true;
			base.HandleGraphicsReset();
		}

		// Token: 0x06001BBB RID: 7099 RVA: 0x000B72A0 File Offset: 0x000B54A0
		public Tween CreatePulseTween()
		{
			float startA = this.StartRadius;
			float startB = startA + 6f;
			float endA = this.EndRadius;
			float endB = endA + 12f;
			Tween tween = Tween.Create(Tween.TweenMode.Persist, null, 0.5f, false);
			tween.OnUpdate = delegate(Tween t)
			{
				this.StartRadius = (float)((int)MathHelper.Lerp(startB, startA, t.Eased));
				this.EndRadius = (float)((int)MathHelper.Lerp(endB, endA, t.Eased));
			};
			return tween;
		}

		// Token: 0x06001BBC RID: 7100 RVA: 0x000B7318 File Offset: 0x000B5518
		public Tween CreateFadeInTween(float time)
		{
			float from = 0f;
			float to = this.Alpha;
			this.Alpha = 0f;
			Tween tween = Tween.Create(Tween.TweenMode.Persist, Ease.CubeOut, time, false);
			tween.OnUpdate = delegate(Tween t)
			{
				this.Alpha = MathHelper.Lerp(from, to, t.Eased);
			};
			return tween;
		}

		// Token: 0x06001BBD RID: 7101 RVA: 0x000B7374 File Offset: 0x000B5574
		public Tween CreateBurstTween(float time)
		{
			time += 0.8f;
			float delay = (time - 0.8f) / time;
			float startA = this.StartRadius;
			float startB = startA + 6f;
			float endA = this.EndRadius;
			float endB = endA + 12f;
			Tween tween = Tween.Create(Tween.TweenMode.Persist, null, time, false);
			tween.OnUpdate = delegate(Tween t)
			{
				float num;
				if (t.Percent >= delay)
				{
					num = (t.Percent - delay) / (1f - delay);
					num = MathHelper.Clamp(num, 0f, 1f);
					num = Ease.CubeIn(num);
				}
				else
				{
					num = 0f;
				}
				this.StartRadius = (float)((int)MathHelper.Lerp(startB, startA, num));
				this.EndRadius = (float)((int)MathHelper.Lerp(endB, endA, num));
			};
			return tween;
		}

		// Token: 0x040018CF RID: 6351
		public int Index = -1;

		// Token: 0x040018D0 RID: 6352
		public bool Dirty = true;

		// Token: 0x040018D1 RID: 6353
		public bool InSolid;

		// Token: 0x040018D2 RID: 6354
		public Vector2 LastNonSolidPosition;

		// Token: 0x040018D3 RID: 6355
		public Vector2 LastEntityPosition;

		// Token: 0x040018D4 RID: 6356
		public Vector2 LastPosition;

		// Token: 0x040018D5 RID: 6357
		public float InSolidAlphaMultiplier = 1f;

		// Token: 0x040018D6 RID: 6358
		public bool Started;

		// Token: 0x040018D7 RID: 6359
		public bool Spotlight;

		// Token: 0x040018D8 RID: 6360
		public float SpotlightDirection;

		// Token: 0x040018D9 RID: 6361
		public float SpotlightPush;

		// Token: 0x040018DA RID: 6362
		public Color Color = Color.White;

		// Token: 0x040018DB RID: 6363
		public float Alpha = 1f;

		// Token: 0x040018DC RID: 6364
		private Vector2 position;

		// Token: 0x040018DD RID: 6365
		private float startRadius = 16f;

		// Token: 0x040018DE RID: 6366
		private float endRadius = 32f;
	}
}
