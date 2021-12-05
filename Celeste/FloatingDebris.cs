using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002CC RID: 716
	[Tracked(false)]
	public class FloatingDebris : Entity
	{
		// Token: 0x0600162F RID: 5679 RVA: 0x0008223C File Offset: 0x0008043C
		public FloatingDebris(Vector2 position) : base(position)
		{
			this.start = this.Position;
			base.Collider = new Hitbox(12f, 12f, -6f, -6f);
			base.Depth = -5;
			MTexture mtexture = GFX.Game["scenery/debris"];
			MTexture texture = new MTexture(mtexture, Calc.Random.Next(mtexture.Width / 8) * 8, 0, 8, 8);
			this.image = new Image(texture);
			this.image.CenterOrigin();
			base.Add(this.image);
			this.rotateSpeed = (float)(Calc.Random.Choose(new int[]
			{
				-2,
				-1,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				2
			}) * 40) * 0.017453292f;
			base.Add(this.sine = new SineWave(0.4f, 0f));
			this.sine.Randomize();
			this.image.Y = this.sine.Value * 2f;
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
		}

		// Token: 0x06001630 RID: 5680 RVA: 0x0008236B File Offset: 0x0008056B
		public FloatingDebris(EntityData data, Vector2 offset) : this(data.Position + offset)
		{
		}

		// Token: 0x06001631 RID: 5681 RVA: 0x00082380 File Offset: 0x00080580
		public override void Update()
		{
			base.Update();
			if (this.pushOut != Vector2.Zero)
			{
				this.Position += this.pushOut * Engine.DeltaTime;
				this.pushOut = Calc.Approach(this.pushOut, Vector2.Zero, 64f * this.accelMult * Engine.DeltaTime);
			}
			else
			{
				this.accelMult = 1f;
				this.Position = Calc.Approach(this.Position, this.start, 6f * Engine.DeltaTime);
			}
			this.image.Rotation += this.rotateSpeed * Engine.DeltaTime;
			this.image.Y = this.sine.Value * 2f;
		}

		// Token: 0x06001632 RID: 5682 RVA: 0x00082458 File Offset: 0x00080658
		private void OnPlayer(Player player)
		{
			Vector2 vector = (this.Position - player.Center).SafeNormalize(player.Speed.Length() * 0.2f);
			if (vector.LengthSquared() > this.pushOut.LengthSquared())
			{
				this.pushOut = vector;
			}
			this.accelMult = 1f;
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x000824B3 File Offset: 0x000806B3
		public void OnExplode(Vector2 from)
		{
			this.pushOut = (this.Position - from).SafeNormalize(160f);
			this.accelMult = 4f;
		}

		// Token: 0x0400128E RID: 4750
		private Vector2 start;

		// Token: 0x0400128F RID: 4751
		private Image image;

		// Token: 0x04001290 RID: 4752
		private SineWave sine;

		// Token: 0x04001291 RID: 4753
		private float rotateSpeed;

		// Token: 0x04001292 RID: 4754
		private Vector2 pushOut;

		// Token: 0x04001293 RID: 4755
		private float accelMult = 1f;
	}
}
