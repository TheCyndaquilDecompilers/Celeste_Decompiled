using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x02000100 RID: 256
	public class ShakerList : Component
	{
		// Token: 0x060006E4 RID: 1764 RVA: 0x0000BD32 File Offset: 0x00009F32
		public ShakerList(int length, bool on = true, Action<Vector2[]> onShake = null) : base(true, false)
		{
			this.Values = new Vector2[length];
			this.on = on;
			this.OnShake = onShake;
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x0000BD61 File Offset: 0x00009F61
		public ShakerList(int length, float time, bool removeOnFinish, Action<Vector2[]> onShake = null) : this(length, true, onShake)
		{
			this.Timer = time;
			this.RemoveOnFinish = removeOnFinish;
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060006E6 RID: 1766 RVA: 0x0000BD7B File Offset: 0x00009F7B
		// (set) Token: 0x060006E7 RID: 1767 RVA: 0x0000BD84 File Offset: 0x00009F84
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
					if (this.Values[0] != Vector2.Zero)
					{
						for (int i = 0; i < this.Values.Length; i++)
						{
							this.Values[i] = Vector2.Zero;
						}
						if (this.OnShake != null)
						{
							this.OnShake(this.Values);
						}
					}
				}
			}
		}

		// Token: 0x060006E8 RID: 1768 RVA: 0x0000BE00 File Offset: 0x0000A000
		public ShakerList ShakeFor(float seconds, bool removeOnFinish)
		{
			this.on = true;
			this.Timer = seconds;
			this.RemoveOnFinish = removeOnFinish;
			return this;
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x0000BE18 File Offset: 0x0000A018
		public override void Update()
		{
			if (this.on && this.Timer > 0f)
			{
				this.Timer -= Engine.DeltaTime;
				if (this.Timer <= 0f)
				{
					this.on = false;
					for (int i = 0; i < this.Values.Length; i++)
					{
						this.Values[i] = Vector2.Zero;
					}
					if (this.OnShake != null)
					{
						this.OnShake(this.Values);
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
				for (int j = 0; j < this.Values.Length; j++)
				{
					this.Values[j] = Calc.Random.ShakeVector();
				}
				if (this.OnShake != null)
				{
					this.OnShake(this.Values);
				}
			}
		}

		// Token: 0x04000514 RID: 1300
		public Vector2[] Values;

		// Token: 0x04000515 RID: 1301
		public float Interval = 0.05f;

		// Token: 0x04000516 RID: 1302
		public float Timer;

		// Token: 0x04000517 RID: 1303
		public bool RemoveOnFinish;

		// Token: 0x04000518 RID: 1304
		public Action<Vector2[]> OnShake;

		// Token: 0x04000519 RID: 1305
		private bool on;
	}
}
