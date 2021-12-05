using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000194 RID: 404
	public class CS06_BossIntro : CutsceneEntity
	{
		// Token: 0x06000E0B RID: 3595 RVA: 0x000325DD File Offset: 0x000307DD
		public CS06_BossIntro(float playerTargetX, Player player, FinalBoss boss) : base(true, false)
		{
			this.player = player;
			this.boss = boss;
			this.playerTargetX = playerTargetX;
			this.bossEndPosition = boss.Position + new Vector2(0f, -16f);
		}

		// Token: 0x06000E0C RID: 3596 RVA: 0x0003261C File Offset: 0x0003081C
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000E0D RID: 3597 RVA: 0x00032631 File Offset: 0x00030831
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			while (!this.player.Dead)
			{
				if (this.player.OnGround(1))
				{
					break;
				}
				yield return null;
			}
			while (this.player.Dead)
			{
				yield return null;
			}
			this.player.Facing = Facings.Right;
			base.Add(new Coroutine(CutsceneEntity.CameraTo(new Vector2((this.player.X + this.boss.X) / 2f - 160f, (float)(level.Bounds.Bottom - 180)), 1f, null, 0f), true));
			yield return 0.5f;
			if (!this.player.Dead)
			{
				yield return this.player.DummyWalkToExact((int)(this.playerTargetX - 8f), false, 1f, false);
			}
			this.player.Facing = Facings.Right;
			yield return Textbox.Say("ch6_boss_start", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.BadelineFloat),
				new Func<IEnumerator>(this.PlayerStepForward)
			});
			yield return level.ZoomBack(0.5f);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000E0E RID: 3598 RVA: 0x00032647 File Offset: 0x00030847
		private IEnumerator BadelineFloat()
		{
			base.Add(new Coroutine(this.Level.ZoomTo(new Vector2(170f, 110f), 2f, 1f), true));
			Audio.Play("event:/char/badeline/boss_prefight_getup", this.boss.Position);
			this.boss.Sitting = false;
			this.boss.NormalSprite.Play("fallSlow", false, false);
			this.boss.NormalSprite.Scale.X = -1f;
			this.boss.Add(this.animator = new BadelineAutoAnimator());
			float fromY = this.boss.Y;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
			{
				this.boss.Position.Y = MathHelper.Lerp(fromY, this.bossEndPosition.Y, Ease.CubeInOut(p));
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000E0F RID: 3599 RVA: 0x00032656 File Offset: 0x00030856
		private IEnumerator PlayerStepForward()
		{
			yield return this.player.DummyWalkToExact((int)this.player.X + 8, false, 1f, false);
			yield break;
		}

		// Token: 0x06000E10 RID: 3600 RVA: 0x00032668 File Offset: 0x00030868
		public override void OnEnd(Level level)
		{
			if (this.WasSkipped && this.player != null)
			{
				this.player.X = this.playerTargetX;
				while (!this.player.OnGround(1) && this.player.Y < (float)level.Bounds.Bottom)
				{
					Player player = this.player;
					float y = player.Y;
					player.Y = y + 1f;
				}
			}
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			this.boss.Position = this.bossEndPosition;
			if (this.boss.NormalSprite != null)
			{
				this.boss.NormalSprite.Scale.X = -1f;
				this.boss.NormalSprite.Play("laugh", false, false);
			}
			this.boss.Sitting = false;
			if (this.animator != null)
			{
				this.boss.Remove(this.animator);
			}
			level.Session.SetFlag("boss_intro", true);
		}

		// Token: 0x0400094D RID: 2381
		public const string Flag = "boss_intro";

		// Token: 0x0400094E RID: 2382
		private Player player;

		// Token: 0x0400094F RID: 2383
		private FinalBoss boss;

		// Token: 0x04000950 RID: 2384
		private Vector2 bossEndPosition;

		// Token: 0x04000951 RID: 2385
		private BadelineAutoAnimator animator;

		// Token: 0x04000952 RID: 2386
		private float playerTargetX;
	}
}
