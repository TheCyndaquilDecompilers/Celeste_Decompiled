using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002B1 RID: 689
	public class Cloud : JumpThru
	{
		// Token: 0x0600154C RID: 5452 RVA: 0x0007A1B0 File Offset: 0x000783B0
		public Cloud(Vector2 position, bool fragile) : base(position, 32, false)
		{
			this.fragile = fragile;
			this.startY = base.Y;
			base.Collider.Position.X = -16f;
			this.timer = Calc.Random.NextFloat() * 4f;
			base.Add(this.wiggler = Wiggler.Create(0.3f, 4f, null, false, false));
			this.particleType = (fragile ? Cloud.P_FragileCloud : Cloud.P_Cloud);
			this.SurfaceSoundIndex = 4;
			base.Add(new LightOcclude(0.2f));
			this.scale = Vector2.One;
			base.Add(this.sfx = new SoundSource());
		}

		// Token: 0x0600154D RID: 5453 RVA: 0x0007A278 File Offset: 0x00078478
		public Cloud(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("fragile", false))
		{
		}

		// Token: 0x0600154E RID: 5454 RVA: 0x0007A298 File Offset: 0x00078498
		public override void Added(Scene scene)
		{
			base.Added(scene);
			string text = this.fragile ? "cloudFragile" : "cloud";
			if (base.SceneAs<Level>().Session.Area.Mode != AreaMode.Normal)
			{
				Collider collider = base.Collider;
				collider.Position.X = collider.Position.X + 2f;
				base.Collider.Width -= 6f;
				text += "Remix";
			}
			base.Add(this.sprite = GFX.SpriteBank.Create(text));
			this.sprite.Origin = new Vector2(this.sprite.Width / 2f, 8f);
			this.sprite.OnFrameChange = delegate(string s)
			{
				if (s == "spawn" && this.sprite.CurrentAnimationFrame == 6)
				{
					this.wiggler.Start();
				}
			};
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x0007A36C File Offset: 0x0007856C
		public override void Update()
		{
			base.Update();
			this.scale.X = Calc.Approach(this.scale.X, 1f, 1f * Engine.DeltaTime);
			this.scale.Y = Calc.Approach(this.scale.Y, 1f, 1f * Engine.DeltaTime);
			this.timer += Engine.DeltaTime;
			if (base.GetPlayerRider() != null)
			{
				this.sprite.Position = Vector2.Zero;
			}
			else
			{
				this.sprite.Position = Calc.Approach(this.sprite.Position, new Vector2(0f, (float)Math.Sin((double)(this.timer * 2f))), Engine.DeltaTime * 4f);
			}
			if (this.respawnTimer > 0f)
			{
				this.respawnTimer -= Engine.DeltaTime;
				if (this.respawnTimer <= 0f)
				{
					this.waiting = true;
					base.Y = this.startY;
					this.speed = 0f;
					this.scale = Vector2.One;
					this.Collidable = true;
					this.sprite.Play("spawn", false, false);
					this.sfx.Play("event:/game/04_cliffside/cloud_pink_reappear", null, 0f);
					return;
				}
			}
			else if (this.waiting)
			{
				Player playerRider = base.GetPlayerRider();
				if (playerRider != null && playerRider.Speed.Y >= 0f)
				{
					this.canRumble = true;
					this.speed = 180f;
					this.scale = new Vector2(1.3f, 0.7f);
					this.waiting = false;
					if (this.fragile)
					{
						Audio.Play("event:/game/04_cliffside/cloud_pink_boost", this.Position);
						return;
					}
					Audio.Play("event:/game/04_cliffside/cloud_blue_boost", this.Position);
					return;
				}
			}
			else if (this.returning)
			{
				this.speed = Calc.Approach(this.speed, 180f, 600f * Engine.DeltaTime);
				base.MoveTowardsY(this.startY, this.speed * Engine.DeltaTime);
				if (base.ExactPosition.Y == this.startY)
				{
					this.returning = false;
					this.waiting = true;
					this.speed = 0f;
					return;
				}
			}
			else
			{
				if (this.fragile && this.Collidable && !base.HasPlayerRider())
				{
					this.Collidable = false;
					this.sprite.Play("fade", false, false);
				}
				if (this.speed < 0f && this.canRumble)
				{
					this.canRumble = false;
					if (base.HasPlayerRider())
					{
						Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
					}
				}
				if (this.speed < 0f && base.Scene.OnInterval(0.02f))
				{
					(base.Scene as Level).ParticlesBG.Emit(this.particleType, 1, this.Position + new Vector2(0f, 2f), new Vector2(base.Collider.Width / 2f, 1f), 1.5707964f);
				}
				if (this.fragile && this.speed < 0f)
				{
					this.sprite.Scale.Y = Calc.Approach(this.sprite.Scale.Y, 0f, Engine.DeltaTime * 4f);
				}
				if (base.Y >= this.startY)
				{
					this.speed -= 1200f * Engine.DeltaTime;
				}
				else
				{
					this.speed += 1200f * Engine.DeltaTime;
					if (this.speed >= -100f)
					{
						Player playerRider2 = base.GetPlayerRider();
						if (playerRider2 != null && playerRider2.Speed.Y >= 0f)
						{
							playerRider2.Speed.Y = -200f;
						}
						if (this.fragile)
						{
							this.Collidable = false;
							this.sprite.Play("fade", false, false);
							this.respawnTimer = 2.5f;
						}
						else
						{
							this.scale = new Vector2(0.7f, 1.3f);
							this.returning = true;
						}
					}
				}
				float num = this.speed;
				if (num < 0f)
				{
					num = -220f;
				}
				base.MoveV(this.speed * Engine.DeltaTime, num);
			}
		}

		// Token: 0x06001550 RID: 5456 RVA: 0x0007A7CC File Offset: 0x000789CC
		public override void Render()
		{
			Vector2 value = this.scale;
			value *= 1f + 0.1f * this.wiggler.Value;
			this.sprite.Scale = value;
			base.Render();
		}

		// Token: 0x04001150 RID: 4432
		public static ParticleType P_Cloud;

		// Token: 0x04001151 RID: 4433
		public static ParticleType P_FragileCloud;

		// Token: 0x04001152 RID: 4434
		private Sprite sprite;

		// Token: 0x04001153 RID: 4435
		private Wiggler wiggler;

		// Token: 0x04001154 RID: 4436
		private ParticleType particleType;

		// Token: 0x04001155 RID: 4437
		private SoundSource sfx;

		// Token: 0x04001156 RID: 4438
		private bool waiting = true;

		// Token: 0x04001157 RID: 4439
		private float speed;

		// Token: 0x04001158 RID: 4440
		private float startY;

		// Token: 0x04001159 RID: 4441
		private float respawnTimer;

		// Token: 0x0400115A RID: 4442
		private bool returning;

		// Token: 0x0400115B RID: 4443
		private bool fragile;

		// Token: 0x0400115C RID: 4444
		private float timer;

		// Token: 0x0400115D RID: 4445
		private Vector2 scale;

		// Token: 0x0400115E RID: 4446
		private bool canRumble;
	}
}
