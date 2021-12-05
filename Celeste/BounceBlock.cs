using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000141 RID: 321
	public class BounceBlock : Solid
	{
		// Token: 0x06000BA7 RID: 2983 RVA: 0x00021D84 File Offset: 0x0001FF84
		public BounceBlock(Vector2 position, float width, float height) : base(position, width, height, false)
		{
			this.state = BounceBlock.States.Waiting;
			this.startPos = this.Position;
			this.hotImages = this.BuildSprite(GFX.Game["objects/bumpblocknew/fire00"]);
			this.hotCenterSprite = GFX.SpriteBank.Create("bumpBlockCenterFire");
			this.hotCenterSprite.Position = new Vector2(base.Width, base.Height) / 2f;
			this.hotCenterSprite.Visible = false;
			base.Add(this.hotCenterSprite);
			this.coldImages = this.BuildSprite(GFX.Game["objects/bumpblocknew/ice00"]);
			this.coldCenterSprite = GFX.SpriteBank.Create("bumpBlockCenterIce");
			this.coldCenterSprite.Position = new Vector2(base.Width, base.Height) / 2f;
			this.coldCenterSprite.Visible = false;
			base.Add(this.coldCenterSprite);
			base.Add(new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
		}

		// Token: 0x06000BA8 RID: 2984 RVA: 0x00021EA8 File Offset: 0x000200A8
		public BounceBlock(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height)
		{
		}

		// Token: 0x06000BA9 RID: 2985 RVA: 0x00021ECC File Offset: 0x000200CC
		private List<Image> BuildSprite(MTexture source)
		{
			List<Image> list = new List<Image>();
			int num = source.Width / 8;
			int num2 = source.Height / 8;
			int num3 = 0;
			while ((float)num3 < base.Width)
			{
				int num4 = 0;
				while ((float)num4 < base.Height)
				{
					int num5;
					if (num3 == 0)
					{
						num5 = 0;
					}
					else if ((float)num3 >= base.Width - 8f)
					{
						num5 = num - 1;
					}
					else
					{
						num5 = Calc.Random.Next(1, num - 1);
					}
					int num6;
					if (num4 == 0)
					{
						num6 = 0;
					}
					else if ((float)num4 >= base.Height - 8f)
					{
						num6 = num2 - 1;
					}
					else
					{
						num6 = Calc.Random.Next(1, num2 - 1);
					}
					Image image = new Image(source.GetSubtexture(num5 * 8, num6 * 8, 8, 8, null));
					image.Position = new Vector2((float)num3, (float)num4);
					list.Add(image);
					base.Add(image);
					num4 += 8;
				}
				num3 += 8;
			}
			return list;
		}

		// Token: 0x06000BAA RID: 2986 RVA: 0x00021FC0 File Offset: 0x000201C0
		private void ToggleSprite()
		{
			this.hotCenterSprite.Visible = !this.iceMode;
			this.coldCenterSprite.Visible = this.iceMode;
			foreach (Image image in this.hotImages)
			{
				image.Visible = !this.iceMode;
			}
			foreach (Image image2 in this.coldImages)
			{
				image2.Visible = this.iceMode;
			}
		}

		// Token: 0x06000BAB RID: 2987 RVA: 0x00022084 File Offset: 0x00020284
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.iceModeNext = (this.iceMode = (base.SceneAs<Level>().CoreMode == Session.CoreModes.Cold));
			this.ToggleSprite();
		}

		// Token: 0x06000BAC RID: 2988 RVA: 0x000220BB File Offset: 0x000202BB
		private void OnChangeMode(Session.CoreModes coreMode)
		{
			this.iceModeNext = (coreMode == Session.CoreModes.Cold);
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x000220C7 File Offset: 0x000202C7
		private void CheckModeChange()
		{
			if (this.iceModeNext != this.iceMode)
			{
				this.iceMode = this.iceModeNext;
				this.ToggleSprite();
			}
		}

		// Token: 0x06000BAE RID: 2990 RVA: 0x000220EC File Offset: 0x000202EC
		public override void Render()
		{
			Vector2 position = this.Position;
			this.Position += base.Shake;
			if (this.state != BounceBlock.States.Broken && this.reformed)
			{
				base.Render();
			}
			if (this.reappearFlash > 0f)
			{
				float num = Ease.CubeOut(this.reappearFlash);
				float num2 = num * 2f;
				Draw.Rect(base.X - num2, base.Y - num2, base.Width + num2 * 2f, base.Height + num2 * 2f, Color.White * num);
			}
			this.Position = position;
		}

		// Token: 0x06000BAF RID: 2991 RVA: 0x00022198 File Offset: 0x00020398
		public override void Update()
		{
			base.Update();
			this.reappearFlash = Calc.Approach(this.reappearFlash, 0f, Engine.DeltaTime * 8f);
			if (this.state == BounceBlock.States.Waiting)
			{
				this.CheckModeChange();
				this.moveSpeed = Calc.Approach(this.moveSpeed, 100f, 400f * Engine.DeltaTime);
				Vector2 vector = Calc.Approach(base.ExactPosition, this.startPos, this.moveSpeed * Engine.DeltaTime);
				Vector2 liftSpeed = (vector - base.ExactPosition).SafeNormalize(this.moveSpeed);
				liftSpeed.X *= 0.75f;
				base.MoveTo(vector, liftSpeed);
				this.windUpProgress = Calc.Approach(this.windUpProgress, 0f, 1f * Engine.DeltaTime);
				Player player = this.WindUpPlayerCheck();
				if (player != null)
				{
					this.moveSpeed = 80f;
					this.windUpStartTimer = 0f;
					if (this.iceMode)
					{
						this.bounceDir = -Vector2.UnitY;
					}
					else
					{
						this.bounceDir = (player.Center - base.Center).SafeNormalize();
					}
					this.state = BounceBlock.States.WindingUp;
					Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
					if (this.iceMode)
					{
						base.StartShaking(0.2f);
						Audio.Play("event:/game/09_core/iceblock_touch", base.Center);
						return;
					}
					Audio.Play("event:/game/09_core/bounceblock_touch", base.Center);
					return;
				}
			}
			else if (this.state == BounceBlock.States.WindingUp)
			{
				Player player2 = this.WindUpPlayerCheck();
				if (player2 != null)
				{
					if (this.iceMode)
					{
						this.bounceDir = -Vector2.UnitY;
					}
					else
					{
						this.bounceDir = (player2.Center - base.Center).SafeNormalize();
					}
				}
				if (this.windUpStartTimer > 0f)
				{
					this.windUpStartTimer -= Engine.DeltaTime;
					this.windUpProgress = Calc.Approach(this.windUpProgress, 0f, 1f * Engine.DeltaTime);
					return;
				}
				this.moveSpeed = Calc.Approach(this.moveSpeed, this.iceMode ? 35f : 40f, 600f * Engine.DeltaTime);
				float num = this.iceMode ? 0.333f : 1f;
				Vector2 vector2 = this.startPos - this.bounceDir * (this.iceMode ? 16f : 10f);
				Vector2 vector3 = Calc.Approach(base.ExactPosition, vector2, this.moveSpeed * num * Engine.DeltaTime);
				Vector2 liftSpeed2 = (vector3 - base.ExactPosition).SafeNormalize(this.moveSpeed * num);
				liftSpeed2.X *= 0.75f;
				base.MoveTo(vector3, liftSpeed2);
				this.windUpProgress = Calc.ClampedMap(Vector2.Distance(base.ExactPosition, vector2), 16f, 2f, 0f, 1f);
				if (this.iceMode && Vector2.DistanceSquared(base.ExactPosition, vector2) <= 12f)
				{
					base.StartShaking(0.1f);
				}
				else if (!this.iceMode && this.windUpProgress >= 0.5f)
				{
					base.StartShaking(0.1f);
				}
				if (Vector2.DistanceSquared(base.ExactPosition, vector2) <= 2f)
				{
					if (this.iceMode)
					{
						this.Break();
					}
					else
					{
						this.state = BounceBlock.States.Bouncing;
					}
					this.moveSpeed = 0f;
					return;
				}
			}
			else if (this.state == BounceBlock.States.Bouncing)
			{
				this.moveSpeed = Calc.Approach(this.moveSpeed, 140f, 800f * Engine.DeltaTime);
				Vector2 vector4 = this.startPos + this.bounceDir * 24f;
				Vector2 vector5 = Calc.Approach(base.ExactPosition, vector4, this.moveSpeed * Engine.DeltaTime);
				this.bounceLift = (vector5 - base.ExactPosition).SafeNormalize(Math.Min(this.moveSpeed * 3f, 200f));
				this.bounceLift.X = this.bounceLift.X * 0.75f;
				base.MoveTo(vector5, this.bounceLift);
				this.windUpProgress = 1f;
				if (base.ExactPosition == vector4 || (!this.iceMode && this.WindUpPlayerCheck() == null))
				{
					this.debrisDirection = (vector4 - this.startPos).SafeNormalize();
					this.state = BounceBlock.States.BounceEnd;
					Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
					this.moveSpeed = 0f;
					this.bounceEndTimer = 0.05f;
					this.ShakeOffPlayer(this.bounceLift);
					return;
				}
			}
			else if (this.state == BounceBlock.States.BounceEnd)
			{
				this.bounceEndTimer -= Engine.DeltaTime;
				if (this.bounceEndTimer <= 0f)
				{
					this.Break();
					return;
				}
			}
			else if (this.state == BounceBlock.States.Broken)
			{
				base.Depth = 8990;
				this.reformed = false;
				if (this.respawnTimer > 0f)
				{
					this.respawnTimer -= Engine.DeltaTime;
					return;
				}
				Vector2 position = this.Position;
				this.Position = this.startPos;
				if (!base.CollideCheck<Actor>() && !base.CollideCheck<Solid>())
				{
					this.CheckModeChange();
					Audio.Play(this.iceMode ? "event:/game/09_core/iceblock_reappear" : "event:/game/09_core/bounceblock_reappear", base.Center);
					float duration = 0.35f;
					int num2 = 0;
					while ((float)num2 < base.Width)
					{
						int num3 = 0;
						while ((float)num3 < base.Height)
						{
							Vector2 vector6 = new Vector2(base.X + (float)num2 + 4f, base.Y + (float)num3 + 4f);
							base.Scene.Add(Engine.Pooler.Create<BounceBlock.RespawnDebris>().Init(vector6 + (vector6 - base.Center).SafeNormalize() * 12f, vector6, this.iceMode, duration));
							num3 += 8;
						}
						num2 += 8;
					}
					Alarm.Set(this, duration, delegate
					{
						this.reformed = true;
						this.reappearFlash = 0.6f;
						base.EnableStaticMovers();
						this.ReformParticles();
					}, Alarm.AlarmMode.Oneshot);
					base.Depth = -9000;
					base.MoveStaticMovers(this.Position - position);
					this.Collidable = true;
					this.state = BounceBlock.States.Waiting;
					return;
				}
				this.Position = position;
			}
		}

		// Token: 0x06000BB0 RID: 2992 RVA: 0x000227F4 File Offset: 0x000209F4
		private void ReformParticles()
		{
			Level level = base.SceneAs<Level>();
			int num = 0;
			while ((float)num < base.Width)
			{
				level.Particles.Emit(BounceBlock.P_Reform, new Vector2(base.X + 2f + (float)num + (float)Calc.Random.Range(-1, 1), base.Y), -1.5707964f);
				level.Particles.Emit(BounceBlock.P_Reform, new Vector2(base.X + 2f + (float)num + (float)Calc.Random.Range(-1, 1), base.Bottom - 1f), 1.5707964f);
				num += 4;
			}
			int num2 = 0;
			while ((float)num2 < base.Height)
			{
				level.Particles.Emit(BounceBlock.P_Reform, new Vector2(base.X, base.Y + 2f + (float)num2 + (float)Calc.Random.Range(-1, 1)), 3.1415927f);
				level.Particles.Emit(BounceBlock.P_Reform, new Vector2(base.Right - 1f, base.Y + 2f + (float)num2 + (float)Calc.Random.Range(-1, 1)), 0f);
				num2 += 4;
			}
		}

		// Token: 0x06000BB1 RID: 2993 RVA: 0x00022938 File Offset: 0x00020B38
		private Player WindUpPlayerCheck()
		{
			Player player = base.CollideFirst<Player>(this.Position - Vector2.UnitY);
			if (player != null && player.Speed.Y < 0f)
			{
				player = null;
			}
			if (player == null)
			{
				player = base.CollideFirst<Player>(this.Position + Vector2.UnitX);
				if (player == null || player.StateMachine.State != 1 || player.Facing != Facings.Left)
				{
					player = base.CollideFirst<Player>(this.Position - Vector2.UnitX);
					if (player == null || player.StateMachine.State != 1 || player.Facing != Facings.Right)
					{
						player = null;
					}
				}
			}
			return player;
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x000229DC File Offset: 0x00020BDC
		private void ShakeOffPlayer(Vector2 liftSpeed)
		{
			Player player = this.WindUpPlayerCheck();
			if (player != null)
			{
				player.StateMachine.State = 0;
				player.Speed = liftSpeed;
				player.StartJumpGraceTime();
			}
		}

		// Token: 0x06000BB3 RID: 2995 RVA: 0x00022A0C File Offset: 0x00020C0C
		private void Break()
		{
			if (!this.iceMode)
			{
				Audio.Play("event:/game/09_core/bounceblock_break", base.Center);
			}
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			this.state = BounceBlock.States.Broken;
			this.Collidable = false;
			base.DisableStaticMovers();
			this.respawnTimer = 1.6f;
			Vector2 direction = new Vector2(0f, 1f);
			if (!this.iceMode)
			{
				direction = this.debrisDirection;
			}
			Vector2 center = base.Center;
			int num = 0;
			while ((float)num < base.Width)
			{
				int num2 = 0;
				while ((float)num2 < base.Height)
				{
					if (this.iceMode)
					{
						direction = (new Vector2(base.X + (float)num + 4f, base.Y + (float)num2 + 4f) - center).SafeNormalize();
					}
					base.Scene.Add(Engine.Pooler.Create<BounceBlock.BreakDebris>().Init(new Vector2(base.X + (float)num + 4f, base.Y + (float)num2 + 4f), direction, this.iceMode));
					num2 += 8;
				}
				num += 8;
			}
			float num3 = this.debrisDirection.Angle();
			Level level = base.SceneAs<Level>();
			int num4 = 0;
			while ((float)num4 < base.Width)
			{
				int num5 = 0;
				while ((float)num5 < base.Height)
				{
					Vector2 vector = this.Position + new Vector2((float)(2 + num4), (float)(2 + num5)) + Calc.Random.Range(-Vector2.One, Vector2.One);
					float direction2 = this.iceMode ? (vector - center).Angle() : num3;
					level.Particles.Emit(this.iceMode ? BounceBlock.P_IceBreak : BounceBlock.P_FireBreak, vector, direction2);
					num5 += 4;
				}
				num4 += 4;
			}
		}

		// Token: 0x040006F2 RID: 1778
		public static ParticleType P_Reform;

		// Token: 0x040006F3 RID: 1779
		public static ParticleType P_FireBreak;

		// Token: 0x040006F4 RID: 1780
		public static ParticleType P_IceBreak;

		// Token: 0x040006F5 RID: 1781
		private const float WindUpDelay = 0f;

		// Token: 0x040006F6 RID: 1782
		private const float WindUpDist = 10f;

		// Token: 0x040006F7 RID: 1783
		private const float IceWindUpDist = 16f;

		// Token: 0x040006F8 RID: 1784
		private const float BounceDist = 24f;

		// Token: 0x040006F9 RID: 1785
		private const float LiftSpeedXMult = 0.75f;

		// Token: 0x040006FA RID: 1786
		private const float RespawnTime = 1.6f;

		// Token: 0x040006FB RID: 1787
		private const float WallPushTime = 0.1f;

		// Token: 0x040006FC RID: 1788
		private const float BounceEndTime = 0.05f;

		// Token: 0x040006FD RID: 1789
		private Vector2 bounceDir;

		// Token: 0x040006FE RID: 1790
		private BounceBlock.States state;

		// Token: 0x040006FF RID: 1791
		private Vector2 startPos;

		// Token: 0x04000700 RID: 1792
		private float moveSpeed;

		// Token: 0x04000701 RID: 1793
		private float windUpStartTimer;

		// Token: 0x04000702 RID: 1794
		private float windUpProgress;

		// Token: 0x04000703 RID: 1795
		private bool iceMode;

		// Token: 0x04000704 RID: 1796
		private bool iceModeNext;

		// Token: 0x04000705 RID: 1797
		private float respawnTimer;

		// Token: 0x04000706 RID: 1798
		private float bounceEndTimer;

		// Token: 0x04000707 RID: 1799
		private Vector2 bounceLift;

		// Token: 0x04000708 RID: 1800
		private float reappearFlash;

		// Token: 0x04000709 RID: 1801
		private bool reformed = true;

		// Token: 0x0400070A RID: 1802
		private Vector2 debrisDirection;

		// Token: 0x0400070B RID: 1803
		private List<Image> hotImages;

		// Token: 0x0400070C RID: 1804
		private List<Image> coldImages;

		// Token: 0x0400070D RID: 1805
		private Sprite hotCenterSprite;

		// Token: 0x0400070E RID: 1806
		private Sprite coldCenterSprite;

		// Token: 0x020003C3 RID: 963
		private enum States
		{
			// Token: 0x04001F71 RID: 8049
			Waiting,
			// Token: 0x04001F72 RID: 8050
			WindingUp,
			// Token: 0x04001F73 RID: 8051
			Bouncing,
			// Token: 0x04001F74 RID: 8052
			BounceEnd,
			// Token: 0x04001F75 RID: 8053
			Broken
		}

		// Token: 0x020003C4 RID: 964
		[Pooled]
		private class RespawnDebris : Entity
		{
			// Token: 0x06001ED7 RID: 7895 RVA: 0x000D6100 File Offset: 0x000D4300
			public BounceBlock.RespawnDebris Init(Vector2 from, Vector2 to, bool ice, float duration)
			{
				List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(ice ? "objects/bumpblocknew/ice_rubble" : "objects/bumpblocknew/fire_rubble");
				MTexture texture = Calc.Random.Choose(atlasSubtextures);
				if (this.sprite == null)
				{
					base.Add(this.sprite = new Image(texture));
					this.sprite.CenterOrigin();
				}
				else
				{
					this.sprite.Texture = texture;
				}
				this.from = from;
				this.Position = from;
				this.percent = 0f;
				this.to = to;
				this.duration = duration;
				return this;
			}

			// Token: 0x06001ED8 RID: 7896 RVA: 0x000D6198 File Offset: 0x000D4398
			public override void Update()
			{
				if (this.percent > 1f)
				{
					base.RemoveSelf();
					return;
				}
				this.percent += Engine.DeltaTime / this.duration;
				this.Position = Vector2.Lerp(this.from, this.to, Ease.CubeIn(this.percent));
				this.sprite.Color = Color.White * this.percent;
			}

			// Token: 0x06001ED9 RID: 7897 RVA: 0x000D6214 File Offset: 0x000D4414
			public override void Render()
			{
				this.sprite.DrawOutline(Color.Black, 1);
				base.Render();
			}

			// Token: 0x04001F76 RID: 8054
			private Image sprite;

			// Token: 0x04001F77 RID: 8055
			private Vector2 from;

			// Token: 0x04001F78 RID: 8056
			private Vector2 to;

			// Token: 0x04001F79 RID: 8057
			private float percent;

			// Token: 0x04001F7A RID: 8058
			private float duration;
		}

		// Token: 0x020003C5 RID: 965
		[Pooled]
		private class BreakDebris : Entity
		{
			// Token: 0x06001EDB RID: 7899 RVA: 0x000D6230 File Offset: 0x000D4430
			public BounceBlock.BreakDebris Init(Vector2 position, Vector2 direction, bool ice)
			{
				List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(ice ? "objects/bumpblocknew/ice_rubble" : "objects/bumpblocknew/fire_rubble");
				MTexture texture = Calc.Random.Choose(atlasSubtextures);
				if (this.sprite == null)
				{
					base.Add(this.sprite = new Image(texture));
					this.sprite.CenterOrigin();
				}
				else
				{
					this.sprite.Texture = texture;
				}
				this.Position = position;
				direction = Calc.AngleToVector(direction.Angle() + Calc.Random.Range(-0.1f, 0.1f), 1f);
				this.speed = direction * (float)(ice ? Calc.Random.Range(20, 40) : Calc.Random.Range(120, 200));
				this.percent = 0f;
				this.duration = (float)Calc.Random.Range(2, 3);
				return this;
			}

			// Token: 0x06001EDC RID: 7900 RVA: 0x000D6318 File Offset: 0x000D4518
			public override void Update()
			{
				base.Update();
				if (this.percent >= 1f)
				{
					base.RemoveSelf();
					return;
				}
				this.Position += this.speed * Engine.DeltaTime;
				this.speed.X = Calc.Approach(this.speed.X, 0f, 180f * Engine.DeltaTime);
				this.speed.Y = this.speed.Y + 200f * Engine.DeltaTime;
				this.percent += Engine.DeltaTime / this.duration;
				this.sprite.Color = Color.White * (1f - this.percent);
			}

			// Token: 0x06001EDD RID: 7901 RVA: 0x000D63DF File Offset: 0x000D45DF
			public override void Render()
			{
				this.sprite.DrawOutline(Color.Black, 1);
				base.Render();
			}

			// Token: 0x04001F7B RID: 8059
			private Image sprite;

			// Token: 0x04001F7C RID: 8060
			private Vector2 speed;

			// Token: 0x04001F7D RID: 8061
			private float percent;

			// Token: 0x04001F7E RID: 8062
			private float duration;
		}
	}
}
