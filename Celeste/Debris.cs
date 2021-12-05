using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002CB RID: 715
	[Pooled]
	public class Debris : Actor
	{
		// Token: 0x06001627 RID: 5671 RVA: 0x00081BE0 File Offset: 0x0007FDE0
		public Debris() : base(Vector2.Zero)
		{
			base.Collider = new Hitbox(4f, 4f, -2f, -2f);
			base.Tag = Tags.Persistent;
			base.Depth = 2000;
			base.Add(this.image = new Image(null));
			this.collideH = new Collision(this.OnCollideH);
			this.collideV = new Collision(this.OnCollideV);
			base.Add(this.dreamSine = new SineWave(0.6f, 0f));
			this.dreamSine.Randomize();
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x00081C9C File Offset: 0x0007FE9C
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.dreaming = base.SceneAs<Level>().Session.Dreaming;
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x00081CBC File Offset: 0x0007FEBC
		public Debris Init(Vector2 pos, char tileset, bool playSound = true)
		{
			this.Position = pos;
			this.tileset = tileset;
			this.playSound = playSound;
			this.lifeTimer = Calc.Random.Range(0.6f, 2.6f);
			this.alpha = 1f;
			this.hasHitGround = false;
			this.speed = Vector2.Zero;
			this.fadeLerp = 0f;
			this.rotateSign = Calc.Random.Choose(1, -1);
			if (GFX.Game.Has("debris/" + tileset.ToString()))
			{
				this.image.Texture = GFX.Game["debris/" + tileset.ToString()];
			}
			else
			{
				this.image.Texture = GFX.Game["debris/1"];
			}
			this.image.CenterOrigin();
			this.image.Color = Color.White * this.alpha;
			this.image.Rotation = Calc.Random.NextAngle();
			this.image.Scale.X = Calc.Random.Range(0.5f, 1f);
			this.image.Scale.Y = Calc.Random.Range(0.5f, 1f);
			this.image.FlipX = Calc.Random.Chance(0.5f);
			this.image.FlipY = Calc.Random.Chance(0.5f);
			return this;
		}

		// Token: 0x0600162A RID: 5674 RVA: 0x00081E4C File Offset: 0x0008004C
		public Debris BlastFrom(Vector2 from)
		{
			float length = (float)Calc.Random.Range(30, 40);
			this.speed = (this.Position - from).SafeNormalize(length);
			this.speed = this.speed.Rotate(Calc.Random.Range(-0.2617994f, 0.2617994f));
			return this;
		}

		// Token: 0x0600162B RID: 5675 RVA: 0x00081EA7 File Offset: 0x000800A7
		private void OnCollideH(CollisionData data)
		{
			this.speed.X = this.speed.X * -0.8f;
		}

		// Token: 0x0600162C RID: 5676 RVA: 0x00081EC0 File Offset: 0x000800C0
		private void OnCollideV(CollisionData data)
		{
			if (this.speed.Y > 0f)
			{
				this.hasHitGround = true;
			}
			this.speed.Y = this.speed.Y * -0.6f;
			if (this.speed.Y < 0f && this.speed.Y > -50f)
			{
				this.speed.Y = 0f;
			}
			if (this.speed.Y != 0f || !this.hasHitGround)
			{
				this.ImpactSfx(Math.Abs(this.speed.Y));
			}
		}

		// Token: 0x0600162D RID: 5677 RVA: 0x00081F60 File Offset: 0x00080160
		private void ImpactSfx(float spd)
		{
			if (this.playSound)
			{
				string path = "event:/game/general/debris_dirt";
				if (this.tileset == '4' || this.tileset == '5' || this.tileset == '6' || this.tileset == '7' || this.tileset == 'a' || this.tileset == 'c' || this.tileset == 'd' || this.tileset == 'e' || this.tileset == 'f' || this.tileset == 'd' || this.tileset == 'g')
				{
					path = "event:/game/general/debris_stone";
				}
				else if (this.tileset == '9')
				{
					path = "event:/game/general/debris_wood";
				}
				Audio.Play(path, this.Position, "debris_velocity", Calc.ClampedMap(spd, 0f, 150f, 0f, 1f));
			}
		}

		// Token: 0x0600162E RID: 5678 RVA: 0x00082030 File Offset: 0x00080230
		public override void Update()
		{
			base.Update();
			this.image.Rotation += Math.Abs(this.speed.X) * (float)this.rotateSign * Engine.DeltaTime;
			if (this.fadeLerp < 1f)
			{
				this.fadeLerp = Calc.Approach(this.fadeLerp, 1f, 2f * Engine.DeltaTime);
			}
			base.MoveH(this.speed.X * Engine.DeltaTime, this.collideH, null);
			base.MoveV(this.speed.Y * Engine.DeltaTime, this.collideV, null);
			if (this.dreaming)
			{
				this.speed.X = Calc.Approach(this.speed.X, 0f, 50f * Engine.DeltaTime);
				this.speed.Y = Calc.Approach(this.speed.Y, 6f * this.dreamSine.Value, 100f * Engine.DeltaTime);
			}
			else
			{
				bool flag = base.OnGround(1);
				this.speed.X = Calc.Approach(this.speed.X, 0f, (flag ? 50f : 20f) * Engine.DeltaTime);
				if (!flag)
				{
					this.speed.Y = Calc.Approach(this.speed.Y, 100f, 400f * Engine.DeltaTime);
				}
			}
			if (this.lifeTimer > 0f)
			{
				this.lifeTimer -= Engine.DeltaTime;
			}
			else if (this.alpha > 0f)
			{
				this.alpha -= 4f * Engine.DeltaTime;
				if (this.alpha <= 0f)
				{
					base.RemoveSelf();
				}
			}
			this.image.Color = Color.Lerp(Color.White, Color.Gray, this.fadeLerp) * this.alpha;
		}

		// Token: 0x04001281 RID: 4737
		private Image image;

		// Token: 0x04001282 RID: 4738
		private float lifeTimer;

		// Token: 0x04001283 RID: 4739
		private float alpha;

		// Token: 0x04001284 RID: 4740
		private Vector2 speed;

		// Token: 0x04001285 RID: 4741
		private Collision collideH;

		// Token: 0x04001286 RID: 4742
		private Collision collideV;

		// Token: 0x04001287 RID: 4743
		private int rotateSign;

		// Token: 0x04001288 RID: 4744
		private float fadeLerp;

		// Token: 0x04001289 RID: 4745
		private bool playSound = true;

		// Token: 0x0400128A RID: 4746
		private bool dreaming;

		// Token: 0x0400128B RID: 4747
		private SineWave dreamSine;

		// Token: 0x0400128C RID: 4748
		private bool hasHitGround;

		// Token: 0x0400128D RID: 4749
		private char tileset;
	}
}
