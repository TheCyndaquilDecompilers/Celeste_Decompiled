using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200019F RID: 415
	[Tracked(false)]
	public class FinalBossMovingBlock : Solid
	{
		// Token: 0x06000E8A RID: 3722 RVA: 0x00035FEC File Offset: 0x000341EC
		public FinalBossMovingBlock(Vector2[] nodes, float width, float height, int bossNodeIndex) : base(nodes[0], width, height, false)
		{
			this.BossNodeIndex = bossNodeIndex;
			this.nodes = nodes;
			int newSeed = Calc.Random.Next();
			Calc.PushRandom(newSeed);
			this.sprite = GFX.FGAutotiler.GenerateBox('g', (int)base.Width / 8, (int)base.Height / 8).TileGrid;
			base.Add(this.sprite);
			Calc.PopRandom();
			Calc.PushRandom(newSeed);
			this.highlight = GFX.FGAutotiler.GenerateBox('G', (int)(base.Width / 8f), (int)base.Height / 8).TileGrid;
			this.highlight.Alpha = 0f;
			base.Add(this.highlight);
			Calc.PopRandom();
			base.Add(new TileInterceptor(this.sprite, false));
			base.Add(new LightOcclude(1f));
		}

		// Token: 0x06000E8B RID: 3723 RVA: 0x000360D7 File Offset: 0x000342D7
		public FinalBossMovingBlock(EntityData data, Vector2 offset) : this(data.NodesWithPosition(offset), (float)data.Width, (float)data.Height, data.Int("nodeIndex", 0))
		{
		}

		// Token: 0x06000E8C RID: 3724 RVA: 0x00036100 File Offset: 0x00034300
		public override void OnShake(Vector2 amount)
		{
			base.OnShake(amount);
			this.sprite.Position = amount;
		}

		// Token: 0x06000E8D RID: 3725 RVA: 0x00036118 File Offset: 0x00034318
		public void StartMoving(float delay)
		{
			this.startDelay = delay;
			base.Add(this.moveCoroutine = new Coroutine(this.MoveSequence(), true));
		}

		// Token: 0x06000E8E RID: 3726 RVA: 0x00036147 File Offset: 0x00034347
		private IEnumerator MoveSequence()
		{
			for (;;)
			{
				FinalBossMovingBlock.<>c__DisplayClass14_0 CS$<>8__locals1 = new FinalBossMovingBlock.<>c__DisplayClass14_0();
				CS$<>8__locals1.<>4__this = this;
				base.StartShaking(0.2f + this.startDelay);
				if (!this.isHighlighted)
				{
					for (float p = 0f; p < 1f; p += Engine.DeltaTime / (0.2f + this.startDelay + 0.2f))
					{
						this.highlight.Alpha = Ease.CubeIn(p);
						this.sprite.Alpha = 1f - this.highlight.Alpha;
						yield return null;
					}
					this.highlight.Alpha = 1f;
					this.sprite.Alpha = 0f;
					this.isHighlighted = true;
				}
				else
				{
					yield return 0.2f + this.startDelay + 0.2f;
				}
				this.startDelay = 0f;
				this.nodeIndex++;
				this.nodeIndex %= this.nodes.Length;
				CS$<>8__locals1.from = this.Position;
				CS$<>8__locals1.to = this.nodes[this.nodeIndex];
				Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, 0.8f, true);
				tween.OnUpdate = delegate(Tween t)
				{
					CS$<>8__locals1.<>4__this.MoveTo(Vector2.Lerp(CS$<>8__locals1.from, CS$<>8__locals1.to, t.Eased));
				};
				tween.OnComplete = delegate(Tween t)
				{
					if (CS$<>8__locals1.<>4__this.CollideCheck<SolidTiles>(CS$<>8__locals1.<>4__this.Position + (CS$<>8__locals1.to - CS$<>8__locals1.from).SafeNormalize() * 2f))
					{
						Audio.Play("event:/game/06_reflection/fallblock_boss_impact", CS$<>8__locals1.<>4__this.Center);
						CS$<>8__locals1.<>4__this.ImpactParticles(CS$<>8__locals1.to - CS$<>8__locals1.from);
						return;
					}
					CS$<>8__locals1.<>4__this.StopParticles(CS$<>8__locals1.to - CS$<>8__locals1.from);
				};
				base.Add(tween);
				yield return 0.8f;
				CS$<>8__locals1 = null;
			}
			yield break;
		}

		// Token: 0x06000E8F RID: 3727 RVA: 0x00036158 File Offset: 0x00034358
		private void StopParticles(Vector2 moved)
		{
			Level level = base.SceneAs<Level>();
			float direction = moved.Angle();
			if (moved.X > 0f)
			{
				Vector2 value = new Vector2(base.Right - 1f, base.Top);
				int num = 0;
				while ((float)num < base.Height)
				{
					level.Particles.Emit(FinalBossMovingBlock.P_Stop, value + Vector2.UnitY * (float)(2 + num + Calc.Random.Range(-1, 1)), direction);
					num += 4;
				}
			}
			else if (moved.X < 0f)
			{
				Vector2 value2 = new Vector2(base.Left, base.Top);
				int num2 = 0;
				while ((float)num2 < base.Height)
				{
					level.Particles.Emit(FinalBossMovingBlock.P_Stop, value2 + Vector2.UnitY * (float)(2 + num2 + Calc.Random.Range(-1, 1)), direction);
					num2 += 4;
				}
			}
			if (moved.Y > 0f)
			{
				Vector2 value3 = new Vector2(base.Left, base.Bottom - 1f);
				int num3 = 0;
				while ((float)num3 < base.Width)
				{
					level.Particles.Emit(FinalBossMovingBlock.P_Stop, value3 + Vector2.UnitX * (float)(2 + num3 + Calc.Random.Range(-1, 1)), direction);
					num3 += 4;
				}
				return;
			}
			if (moved.Y < 0f)
			{
				Vector2 value4 = new Vector2(base.Left, base.Top);
				int num4 = 0;
				while ((float)num4 < base.Width)
				{
					level.Particles.Emit(FinalBossMovingBlock.P_Stop, value4 + Vector2.UnitX * (float)(2 + num4 + Calc.Random.Range(-1, 1)), direction);
					num4 += 4;
				}
			}
		}

		// Token: 0x06000E90 RID: 3728 RVA: 0x00036324 File Offset: 0x00034524
		private void BreakParticles()
		{
			Vector2 center = base.Center;
			int num = 0;
			while ((float)num < base.Width)
			{
				int num2 = 0;
				while ((float)num2 < base.Height)
				{
					Vector2 vector = this.Position + new Vector2((float)(2 + num), (float)(2 + num2));
					base.SceneAs<Level>().Particles.Emit(FinalBossMovingBlock.P_Break, 1, vector, Vector2.One * 2f, (vector - center).Angle());
					num2 += 4;
				}
				num += 4;
			}
		}

		// Token: 0x06000E91 RID: 3729 RVA: 0x000363A8 File Offset: 0x000345A8
		private void ImpactParticles(Vector2 moved)
		{
			if (moved.X < 0f)
			{
				Vector2 value = new Vector2(0f, 2f);
				int num = 0;
				while ((float)num < base.Height / 8f)
				{
					Vector2 vector = new Vector2(base.Left - 1f, base.Top + 4f + (float)(num * 8));
					if (!base.Scene.CollideCheck<Water>(vector) && base.Scene.CollideCheck<Solid>(vector))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector + value, 0f);
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector - value, 0f);
					}
					num++;
				}
			}
			else if (moved.X > 0f)
			{
				Vector2 value2 = new Vector2(0f, 2f);
				int num2 = 0;
				while ((float)num2 < base.Height / 8f)
				{
					Vector2 vector2 = new Vector2(base.Right + 1f, base.Top + 4f + (float)(num2 * 8));
					if (!base.Scene.CollideCheck<Water>(vector2) && base.Scene.CollideCheck<Solid>(vector2))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector2 + value2, 3.1415927f);
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector2 - value2, 3.1415927f);
					}
					num2++;
				}
			}
			if (moved.Y < 0f)
			{
				Vector2 value3 = new Vector2(2f, 0f);
				int num3 = 0;
				while ((float)num3 < base.Width / 8f)
				{
					Vector2 vector3 = new Vector2(base.Left + 4f + (float)(num3 * 8), base.Top - 1f);
					if (!base.Scene.CollideCheck<Water>(vector3) && base.Scene.CollideCheck<Solid>(vector3))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector3 + value3, 1.5707964f);
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector3 - value3, 1.5707964f);
					}
					num3++;
				}
				return;
			}
			if (moved.Y > 0f)
			{
				Vector2 value4 = new Vector2(2f, 0f);
				int num4 = 0;
				while ((float)num4 < base.Width / 8f)
				{
					Vector2 vector4 = new Vector2(base.Left + 4f + (float)(num4 * 8), base.Bottom + 1f);
					if (!base.Scene.CollideCheck<Water>(vector4) && base.Scene.CollideCheck<Solid>(vector4))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector4 + value4, -1.5707964f);
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector4 - value4, -1.5707964f);
					}
					num4++;
				}
			}
		}

		// Token: 0x06000E92 RID: 3730 RVA: 0x000366E0 File Offset: 0x000348E0
		public override void Render()
		{
			Vector2 position = this.Position;
			this.Position += base.Shake;
			base.Render();
			if (this.highlight.Alpha > 0f && this.highlight.Alpha < 1f)
			{
				int num = (int)((1f - this.highlight.Alpha) * 16f);
				Rectangle rect = new Rectangle((int)base.X, (int)base.Y, (int)base.Width, (int)base.Height);
				rect.Inflate(num, num);
				Draw.HollowRect(rect, Color.Lerp(Color.Purple, Color.Pink, 0.7f));
			}
			this.Position = position;
		}

		// Token: 0x06000E93 RID: 3731 RVA: 0x0003679C File Offset: 0x0003499C
		private void Finish()
		{
			Vector2 from = base.CenterRight + Vector2.UnitX * 10f;
			int num = 0;
			while ((float)num < base.Width / 8f)
			{
				int num2 = 0;
				while ((float)num2 < base.Height / 8f)
				{
					base.Scene.Add(Engine.Pooler.Create<Debris>().Init(this.Position + new Vector2((float)(4 + num * 8), (float)(4 + num2 * 8)), 'f', true).BlastFrom(from));
					num2++;
				}
				num++;
			}
			this.BreakParticles();
			base.DestroyStaticMovers();
			base.RemoveSelf();
		}

		// Token: 0x06000E94 RID: 3732 RVA: 0x00036848 File Offset: 0x00034A48
		public void Destroy(float delay)
		{
			if (base.Scene != null)
			{
				if (this.moveCoroutine != null)
				{
					base.Remove(this.moveCoroutine);
				}
				if (delay <= 0f)
				{
					this.Finish();
					return;
				}
				base.StartShaking(delay);
				Alarm.Set(this, delay, new Action(this.Finish), Alarm.AlarmMode.Oneshot);
			}
		}

		// Token: 0x040009BB RID: 2491
		public static ParticleType P_Stop;

		// Token: 0x040009BC RID: 2492
		public static ParticleType P_Break;

		// Token: 0x040009BD RID: 2493
		public int BossNodeIndex;

		// Token: 0x040009BE RID: 2494
		private float startDelay;

		// Token: 0x040009BF RID: 2495
		private int nodeIndex;

		// Token: 0x040009C0 RID: 2496
		private Vector2[] nodes;

		// Token: 0x040009C1 RID: 2497
		private TileGrid sprite;

		// Token: 0x040009C2 RID: 2498
		private TileGrid highlight;

		// Token: 0x040009C3 RID: 2499
		private Coroutine moveCoroutine;

		// Token: 0x040009C4 RID: 2500
		private bool isHighlighted;
	}
}
