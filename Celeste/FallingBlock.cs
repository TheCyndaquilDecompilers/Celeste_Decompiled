using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000340 RID: 832
	[Tracked(false)]
	public class FallingBlock : Solid
	{
		// Token: 0x06001A27 RID: 6695 RVA: 0x000A85C8 File Offset: 0x000A67C8
		public FallingBlock(Vector2 position, char tile, int width, int height, bool finalBoss, bool behind, bool climbFall) : base(position, (float)width, (float)height, false)
		{
			this.finalBoss = finalBoss;
			this.climbFall = climbFall;
			int newSeed = Calc.Random.Next();
			Calc.PushRandom(newSeed);
			base.Add(this.tiles = GFX.FGAutotiler.GenerateBox(tile, width / 8, height / 8).TileGrid);
			Calc.PopRandom();
			if (finalBoss)
			{
				Calc.PushRandom(newSeed);
				base.Add(this.highlight = GFX.FGAutotiler.GenerateBox('G', width / 8, height / 8).TileGrid);
				Calc.PopRandom();
				this.highlight.Alpha = 0f;
			}
			base.Add(new Coroutine(this.Sequence(), true));
			base.Add(new LightOcclude(1f));
			base.Add(new TileInterceptor(this.tiles, false));
			this.TileType = tile;
			this.SurfaceSoundIndex = SurfaceIndex.TileToIndex[tile];
			if (behind)
			{
				base.Depth = 5000;
			}
		}

		// Token: 0x06001A28 RID: 6696 RVA: 0x000A86D0 File Offset: 0x000A68D0
		public FallingBlock(EntityData data, Vector2 offset) : this(data.Position + offset, data.Char("tiletype", '3'), data.Width, data.Height, false, data.Bool("behind", false), data.Bool("climbFall", true))
		{
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x000A8721 File Offset: 0x000A6921
		public static FallingBlock CreateFinalBossBlock(EntityData data, Vector2 offset)
		{
			return new FallingBlock(data.Position + offset, 'g', data.Width, data.Height, true, false, false);
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x000A8745 File Offset: 0x000A6945
		public override void OnShake(Vector2 amount)
		{
			base.OnShake(amount);
			this.tiles.Position += amount;
			if (this.highlight != null)
			{
				this.highlight.Position += amount;
			}
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x000A8784 File Offset: 0x000A6984
		public override void OnStaticMoverTrigger(StaticMover sm)
		{
			if (!this.finalBoss)
			{
				this.Triggered = true;
			}
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06001A2C RID: 6700 RVA: 0x000A8795 File Offset: 0x000A6995
		// (set) Token: 0x06001A2D RID: 6701 RVA: 0x000A879D File Offset: 0x000A699D
		public bool HasStartedFalling { get; private set; }

		// Token: 0x06001A2E RID: 6702 RVA: 0x000A87A6 File Offset: 0x000A69A6
		private bool PlayerFallCheck()
		{
			if (this.climbFall)
			{
				return base.HasPlayerRider();
			}
			return base.HasPlayerOnTop();
		}

		// Token: 0x06001A2F RID: 6703 RVA: 0x000A87C0 File Offset: 0x000A69C0
		private bool PlayerWaitCheck()
		{
			return this.Triggered || this.PlayerFallCheck() || (this.climbFall && (base.CollideCheck<Player>(this.Position - Vector2.UnitX) || base.CollideCheck<Player>(this.Position + Vector2.UnitX)));
		}

		// Token: 0x06001A30 RID: 6704 RVA: 0x000A881B File Offset: 0x000A6A1B
		private IEnumerator Sequence()
		{
			while (!this.Triggered && (this.finalBoss || !this.PlayerFallCheck()))
			{
				yield return null;
			}
			while (this.FallDelay > 0f)
			{
				this.FallDelay -= Engine.DeltaTime;
				yield return null;
			}
			this.HasStartedFalling = true;
			Level level;
			for (;;)
			{
				this.ShakeSfx();
				base.StartShaking(0f);
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
				if (this.finalBoss)
				{
					base.Add(new Coroutine(this.HighlightFade(1f), true));
				}
				yield return 0.2f;
				float timer = 0.4f;
				if (this.finalBoss)
				{
					timer = 0.2f;
				}
				while (timer > 0f && this.PlayerWaitCheck())
				{
					yield return null;
					timer -= Engine.DeltaTime;
				}
				base.StopShaking();
				int num = 2;
				while ((float)num < base.Width)
				{
					if (base.Scene.CollideCheck<Solid>(base.TopLeft + new Vector2((float)num, -2f)))
					{
						base.SceneAs<Level>().Particles.Emit(FallingBlock.P_FallDustA, 2, new Vector2(base.X + (float)num, base.Y), Vector2.One * 4f, 1.5707964f);
					}
					base.SceneAs<Level>().Particles.Emit(FallingBlock.P_FallDustB, 2, new Vector2(base.X + (float)num, base.Y), Vector2.One * 4f);
					num += 4;
				}
				float speed = 0f;
				float maxSpeed = this.finalBoss ? 130f : 160f;
				for (;;)
				{
					level = base.SceneAs<Level>();
					speed = Calc.Approach(speed, maxSpeed, 500f * Engine.DeltaTime);
					if (base.MoveVCollideSolids(speed * Engine.DeltaTime, true, null))
					{
						break;
					}
					if (base.Top > (float)(level.Bounds.Bottom + 16) || (base.Top > (float)(level.Bounds.Bottom - 1) && base.CollideCheck<Solid>(this.Position + new Vector2(0f, 1f))))
					{
						goto IL_37E;
					}
					yield return null;
					level = null;
				}
				this.ImpactSfx();
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
				base.SceneAs<Level>().DirectionalShake(Vector2.UnitY, this.finalBoss ? 0.2f : 0.3f);
				if (this.finalBoss)
				{
					base.Add(new Coroutine(this.HighlightFade(0f), true));
				}
				base.StartShaking(0f);
				this.LandParticles();
				yield return 0.2f;
				base.StopShaking();
				if (base.CollideCheck<SolidTiles>(this.Position + new Vector2(0f, 1f)))
				{
					goto Block_15;
				}
				while (base.CollideCheck<Platform>(this.Position + new Vector2(0f, 1f)))
				{
					yield return 0.1f;
				}
			}
			IL_37E:
			this.Collidable = (this.Visible = false);
			yield return 0.2f;
			if (level.Session.MapData.CanTransitionTo(level, new Vector2(base.Center.X, base.Bottom + 12f)))
			{
				yield return 0.2f;
				base.SceneAs<Level>().Shake(0.3f);
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			}
			base.RemoveSelf();
			base.DestroyStaticMovers();
			yield break;
			Block_15:
			this.Safe = true;
			yield break;
			yield break;
		}

		// Token: 0x06001A31 RID: 6705 RVA: 0x000A882A File Offset: 0x000A6A2A
		private IEnumerator HighlightFade(float to)
		{
			float from = this.highlight.Alpha;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.5f)
			{
				this.highlight.Alpha = MathHelper.Lerp(from, to, Ease.CubeInOut(p));
				this.tiles.Alpha = 1f - this.highlight.Alpha;
				yield return null;
			}
			this.highlight.Alpha = to;
			this.tiles.Alpha = 1f - to;
			yield break;
		}

		// Token: 0x06001A32 RID: 6706 RVA: 0x000A8840 File Offset: 0x000A6A40
		private void LandParticles()
		{
			int num = 2;
			while ((float)num <= base.Width)
			{
				if (base.Scene.CollideCheck<Solid>(base.BottomLeft + new Vector2((float)num, 3f)))
				{
					base.SceneAs<Level>().ParticlesFG.Emit(FallingBlock.P_FallDustA, 1, new Vector2(base.X + (float)num, base.Bottom), Vector2.One * 4f, -1.5707964f);
					float direction;
					if ((float)num < base.Width / 2f)
					{
						direction = 3.1415927f;
					}
					else
					{
						direction = 0f;
					}
					base.SceneAs<Level>().ParticlesFG.Emit(FallingBlock.P_LandDust, 1, new Vector2(base.X + (float)num, base.Bottom), Vector2.One * 4f, direction);
				}
				num += 4;
			}
		}

		// Token: 0x06001A33 RID: 6707 RVA: 0x000A8924 File Offset: 0x000A6B24
		private void ShakeSfx()
		{
			if (this.TileType == '3')
			{
				Audio.Play("event:/game/01_forsaken_city/fallblock_ice_shake", base.Center);
				return;
			}
			if (this.TileType == '9')
			{
				Audio.Play("event:/game/03_resort/fallblock_wood_shake", base.Center);
				return;
			}
			if (this.TileType == 'g')
			{
				Audio.Play("event:/game/06_reflection/fallblock_boss_shake", base.Center);
				return;
			}
			Audio.Play("event:/game/general/fallblock_shake", base.Center);
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x000A8998 File Offset: 0x000A6B98
		private void ImpactSfx()
		{
			if (this.TileType == '3')
			{
				Audio.Play("event:/game/01_forsaken_city/fallblock_ice_impact", base.BottomCenter);
				return;
			}
			if (this.TileType == '9')
			{
				Audio.Play("event:/game/03_resort/fallblock_wood_impact", base.BottomCenter);
				return;
			}
			if (this.TileType == 'g')
			{
				Audio.Play("event:/game/06_reflection/fallblock_boss_impact", base.BottomCenter);
				return;
			}
			Audio.Play("event:/game/general/fallblock_impact", base.BottomCenter);
		}

		// Token: 0x040016CC RID: 5836
		public static ParticleType P_FallDustA;

		// Token: 0x040016CD RID: 5837
		public static ParticleType P_FallDustB;

		// Token: 0x040016CE RID: 5838
		public static ParticleType P_LandDust;

		// Token: 0x040016CF RID: 5839
		public bool Triggered;

		// Token: 0x040016D0 RID: 5840
		public float FallDelay;

		// Token: 0x040016D1 RID: 5841
		private char TileType;

		// Token: 0x040016D2 RID: 5842
		private TileGrid tiles;

		// Token: 0x040016D3 RID: 5843
		private TileGrid highlight;

		// Token: 0x040016D4 RID: 5844
		private bool finalBoss;

		// Token: 0x040016D5 RID: 5845
		private bool climbFall;
	}
}
