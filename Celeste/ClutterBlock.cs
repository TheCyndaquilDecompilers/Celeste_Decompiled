using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002B2 RID: 690
	public class ClutterBlock : Entity
	{
		// Token: 0x06001552 RID: 5458 RVA: 0x0007A838 File Offset: 0x00078A38
		public ClutterBlock(Vector2 position, MTexture texture, ClutterBlock.Colors color) : base(position)
		{
			this.BlockColor = color;
			base.Add(this.Image = new Image(texture));
			base.Collider = new Hitbox((float)texture.Width, (float)texture.Height, 0f, 0f);
			base.Depth = -9998;
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x0007A8B8 File Offset: 0x00078AB8
		public void WeightDown()
		{
			foreach (ClutterBlock clutterBlock in this.Below)
			{
				clutterBlock.WeightDown();
			}
			this.floatTarget = 0f;
			this.floatDelay = 0.1f;
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x0007A920 File Offset: 0x00078B20
		public override void Update()
		{
			base.Update();
			if (!this.OnTheGround)
			{
				if (this.floatDelay <= 0f)
				{
					Player entity = base.Scene.Tracker.GetEntity<Player>();
					if (entity != null && ((this.TopSideOpen && (entity.Right > base.Left && entity.Left < base.Right && entity.Bottom >= base.Top - 1f) && entity.Bottom <= base.Top + 4f) | (entity.StateMachine.State == 1 && this.LeftSideOpen && entity.Right >= base.Left - 1f && entity.Right < base.Left + 4f && entity.Bottom > base.Top && entity.Top < base.Bottom) | (entity.StateMachine.State == 1 && this.RightSideOpen && entity.Left <= base.Right + 1f && entity.Left > base.Right - 4f && entity.Bottom > base.Top && entity.Top < base.Bottom)))
					{
						this.WeightDown();
					}
				}
				this.floatTimer += Engine.DeltaTime;
				this.floatDelay -= Engine.DeltaTime;
				if (this.floatDelay <= 0f)
				{
					this.floatTarget = Calc.Approach(this.floatTarget, this.WaveTarget, Engine.DeltaTime * 4f);
				}
				this.Image.Y = this.floatTarget;
			}
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06001555 RID: 5461 RVA: 0x0007AAE2 File Offset: 0x00078CE2
		private float WaveTarget
		{
			get
			{
				return -(((float)Math.Sin((double)((float)((int)this.Position.X / 16) * 0.25f + this.floatTimer * 2f)) + 1f) / 2f) - 1f;
			}
		}

		// Token: 0x06001556 RID: 5462 RVA: 0x0007AB24 File Offset: 0x00078D24
		public void Absorb(ClutterAbsorbEffect effect)
		{
			effect.FlyClutter(this.Position + new Vector2(this.Image.Width * 0.5f, this.Image.Height * 0.5f + this.floatTarget), this.Image.Texture, true, Calc.Random.NextFloat(0.5f));
			base.Scene.Remove(this);
		}

		// Token: 0x0400115F RID: 4447
		public ClutterBlock.Colors BlockColor;

		// Token: 0x04001160 RID: 4448
		public Image Image;

		// Token: 0x04001161 RID: 4449
		public HashSet<ClutterBlock> HasBelow = new HashSet<ClutterBlock>();

		// Token: 0x04001162 RID: 4450
		public List<ClutterBlock> Below = new List<ClutterBlock>();

		// Token: 0x04001163 RID: 4451
		public List<ClutterBlock> Above = new List<ClutterBlock>();

		// Token: 0x04001164 RID: 4452
		public bool OnTheGround;

		// Token: 0x04001165 RID: 4453
		public bool TopSideOpen;

		// Token: 0x04001166 RID: 4454
		public bool LeftSideOpen;

		// Token: 0x04001167 RID: 4455
		public bool RightSideOpen;

		// Token: 0x04001168 RID: 4456
		private float floatTarget;

		// Token: 0x04001169 RID: 4457
		private float floatDelay;

		// Token: 0x0400116A RID: 4458
		private float floatTimer;

		// Token: 0x0200063B RID: 1595
		public enum Colors
		{
			// Token: 0x040029BD RID: 10685
			Red,
			// Token: 0x040029BE RID: 10686
			Green,
			// Token: 0x040029BF RID: 10687
			Yellow,
			// Token: 0x040029C0 RID: 10688
			Lightning
		}
	}
}
