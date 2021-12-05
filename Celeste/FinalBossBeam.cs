using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200019E RID: 414
	[Pooled]
	[Tracked(false)]
	public class FinalBossBeam : Entity
	{
		// Token: 0x06000E80 RID: 3712 RVA: 0x000353A4 File Offset: 0x000335A4
		public FinalBossBeam()
		{
			base.Add(this.beamSprite = GFX.SpriteBank.Create("badeline_beam"));
			this.beamSprite.OnLastFrame = delegate(string anim)
			{
				if (anim == "shoot")
				{
					this.Destroy();
				}
			};
			base.Add(this.beamStartSprite = GFX.SpriteBank.Create("badeline_beam_start"));
			this.beamSprite.Visible = false;
			base.Depth = -1000000;
		}

		// Token: 0x06000E81 RID: 3713 RVA: 0x00035430 File Offset: 0x00033630
		public FinalBossBeam Init(FinalBoss boss, Player target)
		{
			this.boss = boss;
			this.chargeTimer = 1.4f;
			this.followTimer = 0.9f;
			this.activeTimer = 0.12f;
			this.beamSprite.Play("charge", false, false);
			this.sideFadeAlpha = 0f;
			this.beamAlpha = 0f;
			int num;
			if (target.Y <= boss.Y + 16f)
			{
				num = 1;
			}
			else
			{
				num = -1;
			}
			if (target.X >= boss.X)
			{
				num *= -1;
			}
			this.angle = Calc.Angle(boss.BeamOrigin, target.Center);
			Vector2 vector = Calc.ClosestPointOnLine(boss.BeamOrigin, boss.BeamOrigin + Calc.AngleToVector(this.angle, 2000f), target.Center);
			vector += (target.Center - boss.BeamOrigin).Perpendicular().SafeNormalize(100f) * (float)num;
			this.angle = Calc.Angle(boss.BeamOrigin, vector);
			return this;
		}

		// Token: 0x06000E82 RID: 3714 RVA: 0x0003553E File Offset: 0x0003373E
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (this.boss.Moving)
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06000E83 RID: 3715 RVA: 0x0003555C File Offset: 0x0003375C
		public override void Update()
		{
			base.Update();
			this.player = base.Scene.Tracker.GetEntity<Player>();
			this.beamAlpha = Calc.Approach(this.beamAlpha, 1f, 2f * Engine.DeltaTime);
			if (this.chargeTimer > 0f)
			{
				this.sideFadeAlpha = Calc.Approach(this.sideFadeAlpha, 1f, Engine.DeltaTime);
				if (this.player != null && !this.player.Dead)
				{
					this.followTimer -= Engine.DeltaTime;
					this.chargeTimer -= Engine.DeltaTime;
					if (this.followTimer > 0f && this.player.Center != this.boss.BeamOrigin)
					{
						Vector2 vector = Calc.ClosestPointOnLine(this.boss.BeamOrigin, this.boss.BeamOrigin + Calc.AngleToVector(this.angle, 2000f), this.player.Center);
						Vector2 center = this.player.Center;
						vector = Calc.Approach(vector, center, 200f * Engine.DeltaTime);
						this.angle = Calc.Angle(this.boss.BeamOrigin, vector);
					}
					else if (this.beamSprite.CurrentAnimationID == "charge")
					{
						this.beamSprite.Play("lock", false, false);
					}
					if (this.chargeTimer <= 0f)
					{
						base.SceneAs<Level>().DirectionalShake(Calc.AngleToVector(this.angle, 1f), 0.15f);
						Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
						this.DissipateParticles();
						return;
					}
				}
			}
			else if (this.activeTimer > 0f)
			{
				this.sideFadeAlpha = Calc.Approach(this.sideFadeAlpha, 0f, Engine.DeltaTime * 8f);
				if (this.beamSprite.CurrentAnimationID != "shoot")
				{
					this.beamSprite.Play("shoot", false, false);
					this.beamStartSprite.Play("shoot", true, false);
				}
				this.activeTimer -= Engine.DeltaTime;
				if (this.activeTimer > 0f)
				{
					this.PlayerCollideCheck();
				}
			}
		}

		// Token: 0x06000E84 RID: 3716 RVA: 0x000357AC File Offset: 0x000339AC
		private void DissipateParticles()
		{
			Level level = base.SceneAs<Level>();
			Vector2 vector = level.Camera.Position + new Vector2(160f, 90f);
			Vector2 vector2 = this.boss.BeamOrigin + Calc.AngleToVector(this.angle, 12f);
			Vector2 vector3 = this.boss.BeamOrigin + Calc.AngleToVector(this.angle, 2000f);
			Vector2 vector4 = (vector3 - vector2).Perpendicular().SafeNormalize();
			Vector2 value = (vector3 - vector2).SafeNormalize();
			Vector2 min = -vector4 * 1f;
			Vector2 max = vector4 * 1f;
			float direction = vector4.Angle();
			float direction2 = (-vector4).Angle();
			float num = Vector2.Distance(vector, vector2) - 12f;
			vector = Calc.ClosestPointOnLine(vector2, vector3, vector);
			for (int i = 0; i < 200; i += 12)
			{
				for (int j = -1; j <= 1; j += 2)
				{
					level.ParticlesFG.Emit(FinalBossBeam.P_Dissipate, vector + value * (float)i + vector4 * 2f * (float)j + Calc.Random.Range(min, max), direction);
					level.ParticlesFG.Emit(FinalBossBeam.P_Dissipate, vector + value * (float)i - vector4 * 2f * (float)j + Calc.Random.Range(min, max), direction2);
					if (i != 0 && (float)i < num)
					{
						level.ParticlesFG.Emit(FinalBossBeam.P_Dissipate, vector - value * (float)i + vector4 * 2f * (float)j + Calc.Random.Range(min, max), direction);
						level.ParticlesFG.Emit(FinalBossBeam.P_Dissipate, vector - value * (float)i - vector4 * 2f * (float)j + Calc.Random.Range(min, max), direction2);
					}
				}
			}
		}

		// Token: 0x06000E85 RID: 3717 RVA: 0x00035A10 File Offset: 0x00033C10
		private void PlayerCollideCheck()
		{
			Vector2 vector = this.boss.BeamOrigin + Calc.AngleToVector(this.angle, 12f);
			Vector2 vector2 = this.boss.BeamOrigin + Calc.AngleToVector(this.angle, 2000f);
			Vector2 value = (vector2 - vector).Perpendicular().SafeNormalize(2f);
			Player player = base.Scene.CollideFirst<Player>(vector + value, vector2 + value);
			if (player == null)
			{
				player = base.Scene.CollideFirst<Player>(vector - value, vector2 - value);
			}
			if (player == null)
			{
				player = base.Scene.CollideFirst<Player>(vector, vector2);
			}
			if (player != null)
			{
				player.Die((player.Center - this.boss.BeamOrigin).SafeNormalize(), false, true);
			}
		}

		// Token: 0x06000E86 RID: 3718 RVA: 0x00035AE8 File Offset: 0x00033CE8
		public override void Render()
		{
			Vector2 vector = this.boss.BeamOrigin;
			Vector2 vector2 = Calc.AngleToVector(this.angle, this.beamSprite.Width);
			this.beamSprite.Rotation = this.angle;
			this.beamSprite.Color = Color.White * this.beamAlpha;
			this.beamStartSprite.Rotation = this.angle;
			this.beamStartSprite.Color = Color.White * this.beamAlpha;
			if (this.beamSprite.CurrentAnimationID == "shoot")
			{
				vector += Calc.AngleToVector(this.angle, 8f);
			}
			for (int i = 0; i < 15; i++)
			{
				this.beamSprite.RenderPosition = vector;
				this.beamSprite.Render();
				vector += vector2;
			}
			if (this.beamSprite.CurrentAnimationID == "shoot")
			{
				this.beamStartSprite.RenderPosition = this.boss.BeamOrigin;
				this.beamStartSprite.Render();
			}
			GameplayRenderer.End();
			Vector2 vector3 = vector2.SafeNormalize();
			Vector2 vector4 = vector3.Perpendicular();
			Color color = Color.Black * this.sideFadeAlpha * 0.35f;
			Color transparent = Color.Transparent;
			vector3 *= 4000f;
			vector4 *= 120f;
			int num = 0;
			this.Quad(ref num, vector, -vector3 + vector4 * 2f, vector3 + vector4 * 2f, vector3 + vector4, -vector3 + vector4, color, color);
			this.Quad(ref num, vector, -vector3 + vector4, vector3 + vector4, vector3, -vector3, color, transparent);
			this.Quad(ref num, vector, -vector3, vector3, vector3 - vector4, -vector3 - vector4, transparent, color);
			this.Quad(ref num, vector, -vector3 - vector4, vector3 - vector4, vector3 - vector4 * 2f, -vector3 - vector4 * 2f, color, color);
			GFX.DrawVertices<VertexPositionColor>((base.Scene as Level).Camera.Matrix, this.fade, this.fade.Length, null, null);
			GameplayRenderer.Begin();
		}

		// Token: 0x06000E87 RID: 3719 RVA: 0x00035D70 File Offset: 0x00033F70
		private void Quad(ref int v, Vector2 offset, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color ab, Color cd)
		{
			this.fade[v].Position.X = offset.X + a.X;
			this.fade[v].Position.Y = offset.Y + a.Y;
			VertexPositionColor[] array = this.fade;
			int num = v;
			v = num + 1;
			array[num].Color = ab;
			this.fade[v].Position.X = offset.X + b.X;
			this.fade[v].Position.Y = offset.Y + b.Y;
			VertexPositionColor[] array2 = this.fade;
			num = v;
			v = num + 1;
			array2[num].Color = ab;
			this.fade[v].Position.X = offset.X + c.X;
			this.fade[v].Position.Y = offset.Y + c.Y;
			VertexPositionColor[] array3 = this.fade;
			num = v;
			v = num + 1;
			array3[num].Color = cd;
			this.fade[v].Position.X = offset.X + a.X;
			this.fade[v].Position.Y = offset.Y + a.Y;
			VertexPositionColor[] array4 = this.fade;
			num = v;
			v = num + 1;
			array4[num].Color = ab;
			this.fade[v].Position.X = offset.X + c.X;
			this.fade[v].Position.Y = offset.Y + c.Y;
			VertexPositionColor[] array5 = this.fade;
			num = v;
			v = num + 1;
			array5[num].Color = cd;
			this.fade[v].Position.X = offset.X + d.X;
			this.fade[v].Position.Y = offset.Y + d.Y;
			VertexPositionColor[] array6 = this.fade;
			num = v;
			v = num + 1;
			array6[num].Color = cd;
		}

		// Token: 0x06000E88 RID: 3720 RVA: 0x0002836E File Offset: 0x0002656E
		public void Destroy()
		{
			base.RemoveSelf();
		}

		// Token: 0x040009A5 RID: 2469
		public static ParticleType P_Dissipate;

		// Token: 0x040009A6 RID: 2470
		public const float ChargeTime = 1.4f;

		// Token: 0x040009A7 RID: 2471
		public const float FollowTime = 0.9f;

		// Token: 0x040009A8 RID: 2472
		public const float ActiveTime = 0.12f;

		// Token: 0x040009A9 RID: 2473
		private const float AngleStartOffset = 100f;

		// Token: 0x040009AA RID: 2474
		private const float RotationSpeed = 200f;

		// Token: 0x040009AB RID: 2475
		private const float CollideCheckSep = 2f;

		// Token: 0x040009AC RID: 2476
		private const float BeamLength = 2000f;

		// Token: 0x040009AD RID: 2477
		private const float BeamStartDist = 12f;

		// Token: 0x040009AE RID: 2478
		private const int BeamsDrawn = 15;

		// Token: 0x040009AF RID: 2479
		private const float SideDarknessAlpha = 0.35f;

		// Token: 0x040009B0 RID: 2480
		private FinalBoss boss;

		// Token: 0x040009B1 RID: 2481
		private Player player;

		// Token: 0x040009B2 RID: 2482
		private Sprite beamSprite;

		// Token: 0x040009B3 RID: 2483
		private Sprite beamStartSprite;

		// Token: 0x040009B4 RID: 2484
		private float chargeTimer;

		// Token: 0x040009B5 RID: 2485
		private float followTimer;

		// Token: 0x040009B6 RID: 2486
		private float activeTimer;

		// Token: 0x040009B7 RID: 2487
		private float angle;

		// Token: 0x040009B8 RID: 2488
		private float beamAlpha;

		// Token: 0x040009B9 RID: 2489
		private float sideFadeAlpha;

		// Token: 0x040009BA RID: 2490
		private VertexPositionColor[] fade = new VertexPositionColor[24];
	}
}
