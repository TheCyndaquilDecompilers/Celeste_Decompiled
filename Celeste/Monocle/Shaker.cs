using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x020000FF RID: 255
	public class Shaker : Component
	{
		// Token: 0x060006DE RID: 1758 RVA: 0x0000BBBA File Offset: 0x00009DBA
		public Shaker(bool on = true, Action<Vector2> onShake = null) : base(true, false)
		{
			this.on = on;
			this.OnShake = onShake;
		}

		// Token: 0x060006DF RID: 1759 RVA: 0x0000BBDD File Offset: 0x00009DDD
		public Shaker(float time, bool removeOnFinish, Action<Vector2> onShake = null) : this(true, onShake)
		{
			this.Timer = time;
			this.RemoveOnFinish = removeOnFinish;
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x0000BBF5 File Offset: 0x00009DF5
		// (set) Token: 0x060006E1 RID: 1761 RVA: 0x0000BC00 File Offset: 0x00009E00
		public bool On
		{
			get
			{
				return this.on;
			}
			set
			{
				this.on = value;
				if (!this.on)
				{
					this.Timer = 0f;
					if (this.Value != Vector2.Zero)
					{
						this.Value = Vector2.Zero;
						if (this.OnShake != null)
						{
							this.OnShake(Vector2.Zero);
						}
					}
				}
			}
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x0000BC5C File Offset: 0x00009E5C
		public Shaker ShakeFor(float seconds, bool removeOnFinish)
		{
			this.on = true;
			this.Timer = seconds;
			this.RemoveOnFinish = removeOnFinish;
			return this;
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x0000BC74 File Offset: 0x00009E74
		public override void Update()
		{
			if (this.on && this.Timer > 0f)
			{
				this.Timer -= Engine.DeltaTime;
				if (this.Timer <= 0f)
				{
					this.on = false;
					this.Value = Vector2.Zero;
					if (this.OnShake != null)
					{
						this.OnShake(Vector2.Zero);
					}
					if (this.RemoveOnFinish)
					{
						base.RemoveSelf();
					}
					return;
				}
			}
			if (this.on && base.Scene.OnInterval(this.Interval))
			{
				this.Value = Calc.Random.ShakeVector();
				if (this.OnShake != null)
				{
					this.OnShake(this.Value);
				}
			}
		}

		// Token: 0x0400050E RID: 1294
		public Vector2 Value;

		// Token: 0x0400050F RID: 1295
		public float Interval = 0.05f;

		// Token: 0x04000510 RID: 1296
		public float Timer;

		// Token: 0x04000511 RID: 1297
		public bool RemoveOnFinish;

		// Token: 0x04000512 RID: 1298
		public Action<Vector2> OnShake;

		// Token: 0x04000513 RID: 1299
		private bool on;
	}
}
