using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001A0 RID: 416
	[Pooled]
	[Tracked(false)]
	public class FinalBossShot : Entity
	{
		// Token: 0x06000E95 RID: 3733 RVA: 0x0003689C File Offset: 0x00034A9C
		public FinalBossShot() : base(Vector2.Zero)
		{
			base.Add(this.sprite = GFX.SpriteBank.Create("badeline_projectile"));
			base.Collider = new Hitbox(4f, 4f, -2f, -2f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Depth = -1000000;
			base.Add(this.sine = new SineWave(1.4f, 0f));
		}

		// Token: 0x06000E96 RID: 3734 RVA: 0x00036934 File Offset: 0x00034B34
		public FinalBossShot Init(FinalBoss boss, Player target, float angleOffset = 0f)
		{
			this.boss = boss;
			this.anchor = (this.Position = boss.Center);
			this.target = target;
			this.angleOffset = angleOffset;
			this.dead = (this.hasBeenInCamera = false);
			this.cantKillTimer = 0.15f;
			this.appearTimer = 0.1f;
			this.sine.Reset();
			this.sineMult = 0f;
			this.sprite.Play("charge", true, false);
			this.InitSpeed();
			return this;
		}

		// Token: 0x06000E97 RID: 3735 RVA: 0x000369C0 File Offset: 0x00034BC0
		public FinalBossShot Init(FinalBoss boss, Vector2 target)
		{
			this.boss = boss;
			this.anchor = (this.Position = boss.Center);
			this.target = null;
			this.angleOffset = 0f;
			this.targetPt = target;
			this.dead = (this.hasBeenInCamera = false);
			this.cantKillTimer = 0.15f;
			this.appearTimer = 0.1f;
			this.sine.Reset();
			this.sineMult = 0f;
			this.sprite.Play("charge", true, false);
			this.InitSpeed();
			return this;
		}

		// Token: 0x06000E98 RID: 3736 RVA: 0x00036A58 File Offset: 0x00034C58
		private void InitSpeed()
		{
			if (this.target != null)
			{
				this.speed = (this.target.Center - base.Center).SafeNormalize(100f);
			}
			else
			{
				this.speed = (this.targetPt - base.Center).SafeNormalize(100f);
			}
			if (this.angleOffset != 0f)
			{
				this.speed = this.speed.Rotate(this.angleOffset);
			}
			this.perp = this.speed.Perpendicular().SafeNormalize();
			this.particleDir = (-this.speed).Angle();
		}

		// Token: 0x06000E99 RID: 3737 RVA: 0x00036B06 File Offset: 0x00034D06
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
			if (this.boss.Moving)
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06000E9A RID: 3738 RVA: 0x00036B2E File Offset: 0x00034D2E
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			this.level = null;
		}

		// Token: 0x06000E9B RID: 3739 RVA: 0x00036B40 File Offset: 0x00034D40
		public override void Update()
		{
			base.Update();
			if (this.appearTimer > 0f)
			{
				this.Position = (this.anchor = this.boss.ShotOrigin);
				this.appearTimer -= Engine.DeltaTime;
				return;
			}
			if (this.cantKillTimer > 0f)
			{
				this.cantKillTimer -= Engine.DeltaTime;
			}
			this.anchor += this.speed * Engine.DeltaTime;
			this.Position = this.anchor + this.perp * this.sineMult * this.sine.Value * 3f;
			this.sineMult = Calc.Approach(this.sineMult, 1f, 2f * Engine.DeltaTime);
			if (!this.dead)
			{
				bool flag = this.level.IsInCamera(this.Position, 8f);
				if (flag && !this.hasBeenInCamera)
				{
					this.hasBeenInCamera = true;
				}
				else if (!flag && this.hasBeenInCamera)
				{
					this.Destroy();
				}
				if (base.Scene.OnInterval(0.04f))
				{
					this.level.ParticlesFG.Emit(FinalBossShot.P_Trail, 1, base.Center, Vector2.One * 2f, this.particleDir);
				}
			}
		}

		// Token: 0x06000E9C RID: 3740 RVA: 0x00036CB4 File Offset: 0x00034EB4
		public override void Render()
		{
			Color color = this.sprite.Color;
			Vector2 position = this.sprite.Position;
			this.sprite.Color = Color.Black;
			this.sprite.Position = position + new Vector2(-1f, 0f);
			this.sprite.Render();
			this.sprite.Position = position + new Vector2(1f, 0f);
			this.sprite.Render();
			this.sprite.Position = position + new Vector2(0f, -1f);
			this.sprite.Render();
			this.sprite.Position = position + new Vector2(0f, 1f);
			this.sprite.Render();
			this.sprite.Color = color;
			this.sprite.Position = position;
			base.Render();
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x00036DB3 File Offset: 0x00034FB3
		public void Destroy()
		{
			this.dead = true;
			base.RemoveSelf();
		}

		// Token: 0x06000E9E RID: 3742 RVA: 0x00036DC2 File Offset: 0x00034FC2
		private void OnPlayer(Player player)
		{
			if (!this.dead)
			{
				if (this.cantKillTimer > 0f)
				{
					this.Destroy();
					return;
				}
				player.Die((player.Center - this.Position).SafeNormalize(), false, true);
			}
		}

		// Token: 0x040009C5 RID: 2501
		public static ParticleType P_Trail;

		// Token: 0x040009C6 RID: 2502
		private const float MoveSpeed = 100f;

		// Token: 0x040009C7 RID: 2503
		private const float CantKillTime = 0.15f;

		// Token: 0x040009C8 RID: 2504
		private const float AppearTime = 0.1f;

		// Token: 0x040009C9 RID: 2505
		private FinalBoss boss;

		// Token: 0x040009CA RID: 2506
		private Level level;

		// Token: 0x040009CB RID: 2507
		private Vector2 speed;

		// Token: 0x040009CC RID: 2508
		private float particleDir;

		// Token: 0x040009CD RID: 2509
		private Vector2 anchor;

		// Token: 0x040009CE RID: 2510
		private Vector2 perp;

		// Token: 0x040009CF RID: 2511
		private Player target;

		// Token: 0x040009D0 RID: 2512
		private Vector2 targetPt;

		// Token: 0x040009D1 RID: 2513
		private float angleOffset;

		// Token: 0x040009D2 RID: 2514
		private bool dead;

		// Token: 0x040009D3 RID: 2515
		private float cantKillTimer;

		// Token: 0x040009D4 RID: 2516
		private float appearTimer;

		// Token: 0x040009D5 RID: 2517
		private bool hasBeenInCamera;

		// Token: 0x040009D6 RID: 2518
		private SineWave sine;

		// Token: 0x040009D7 RID: 2519
		private float sineMult;

		// Token: 0x040009D8 RID: 2520
		private Sprite sprite;

		// Token: 0x020004B2 RID: 1202
		public enum ShotPatterns
		{
			// Token: 0x0400232A RID: 9002
			Single,
			// Token: 0x0400232B RID: 9003
			Double,
			// Token: 0x0400232C RID: 9004
			Triple
		}
	}
}
