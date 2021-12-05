using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200019C RID: 412
	public class TrackSpinner : Entity
	{
		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000E4C RID: 3660 RVA: 0x00033DC9 File Offset: 0x00031FC9
		// (set) Token: 0x06000E4D RID: 3661 RVA: 0x00033DD1 File Offset: 0x00031FD1
		public Vector2 Start { get; private set; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000E4E RID: 3662 RVA: 0x00033DDA File Offset: 0x00031FDA
		// (set) Token: 0x06000E4F RID: 3663 RVA: 0x00033DE2 File Offset: 0x00031FE2
		public Vector2 End { get; private set; }

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000E50 RID: 3664 RVA: 0x00033DEB File Offset: 0x00031FEB
		// (set) Token: 0x06000E51 RID: 3665 RVA: 0x00033DF3 File Offset: 0x00031FF3
		public float Percent { get; private set; }

		// Token: 0x06000E52 RID: 3666 RVA: 0x00033DFC File Offset: 0x00031FFC
		public TrackSpinner(EntityData data, Vector2 offset)
		{
			base.Collider = new ColliderList(new Collider[]
			{
				new Circle(6f, 0f, 0f),
				new Hitbox(16f, 4f, -8f, -3f)
			});
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			this.Start = data.Position + offset;
			this.End = data.Nodes[0] + offset;
			this.Speed = data.Enum<TrackSpinner.Speeds>("speed", TrackSpinner.Speeds.Normal);
			this.Angle = (this.Start - this.End).Angle();
			this.Percent = (data.Bool("startCenter", false) ? 0.5f : 0f);
			if (this.Percent == 1f)
			{
				this.Up = false;
			}
			this.UpdatePosition();
		}

		// Token: 0x06000E53 RID: 3667 RVA: 0x00033F0D File Offset: 0x0003210D
		public void UpdatePosition()
		{
			this.Position = Vector2.Lerp(this.Start, this.End, Ease.SineInOut(this.Percent));
		}

		// Token: 0x06000E54 RID: 3668 RVA: 0x00033F36 File Offset: 0x00032136
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			this.OnTrackStart();
		}

		// Token: 0x06000E55 RID: 3669 RVA: 0x00033F48 File Offset: 0x00032148
		public override void Update()
		{
			base.Update();
			if (this.Moving)
			{
				if (this.PauseTimer > 0f)
				{
					this.PauseTimer -= Engine.DeltaTime;
					if (this.PauseTimer <= 0f)
					{
						this.OnTrackStart();
						return;
					}
				}
				else
				{
					this.Percent = Calc.Approach(this.Percent, (float)(this.Up ? 1 : 0), Engine.DeltaTime / TrackSpinner.MoveTimes[(int)this.Speed]);
					this.UpdatePosition();
					if ((this.Up && this.Percent == 1f) || (!this.Up && this.Percent == 0f))
					{
						this.Up = !this.Up;
						this.PauseTimer = TrackSpinner.PauseTimes[(int)this.Speed];
						this.OnTrackEnd();
					}
				}
			}
		}

		// Token: 0x06000E56 RID: 3670 RVA: 0x00034023 File Offset: 0x00032223
		public virtual void OnPlayer(Player player)
		{
			if (player.Die((player.Position - this.Position).SafeNormalize(), false, true) != null)
			{
				this.Moving = false;
			}
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void OnTrackStart()
		{
		}

		// Token: 0x06000E58 RID: 3672 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void OnTrackEnd()
		{
		}

		// Token: 0x0400097D RID: 2429
		public static readonly float[] PauseTimes = new float[]
		{
			0.3f,
			0.2f,
			0.6f
		};

		// Token: 0x0400097E RID: 2430
		public static readonly float[] MoveTimes = new float[]
		{
			0.9f,
			0.4f,
			0.3f
		};

		// Token: 0x04000982 RID: 2434
		public bool Up = true;

		// Token: 0x04000983 RID: 2435
		public float PauseTimer;

		// Token: 0x04000984 RID: 2436
		public TrackSpinner.Speeds Speed;

		// Token: 0x04000985 RID: 2437
		public bool Moving = true;

		// Token: 0x04000986 RID: 2438
		public float Angle;

		// Token: 0x0200049B RID: 1179
		public enum Speeds
		{
			// Token: 0x040022D5 RID: 8917
			Slow,
			// Token: 0x040022D6 RID: 8918
			Normal,
			// Token: 0x040022D7 RID: 8919
			Fast
		}
	}
}
