using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000285 RID: 645
	public class CS03_OshiroMasterSuite : CutsceneEntity
	{
		// Token: 0x060013F2 RID: 5106 RVA: 0x0006BF68 File Offset: 0x0006A168
		public CS03_OshiroMasterSuite(NPC oshiro) : base(true, false)
		{
			this.oshiro = oshiro;
		}

		// Token: 0x060013F3 RID: 5107 RVA: 0x0006BF79 File Offset: 0x0006A179
		public override void OnBegin(Level level)
		{
			this.mirror = base.Scene.Entities.FindFirst<ResortMirror>();
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x060013F4 RID: 5108 RVA: 0x0006BFA4 File Offset: 0x0006A1A4
		private IEnumerator Cutscene(Level level)
		{
			for (;;)
			{
				this.player = base.Scene.Tracker.GetEntity<Player>();
				if (this.player != null)
				{
					break;
				}
				yield return null;
			}
			Audio.SetMusic(null, true, true);
			yield return 0.4f;
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			base.Add(new Coroutine(this.player.DummyWalkTo(this.oshiro.X + 32f, false, 1f, false), true));
			yield return 1f;
			Audio.SetMusic("event:/music/lvl3/oshiro_theme", true, true);
			yield return Textbox.Say("CH3_OSHIRO_SUITE", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.SuiteShadowAppear),
				new Func<IEnumerator>(this.SuiteShadowDisrupt),
				new Func<IEnumerator>(this.SuiteShadowCeiling),
				new Func<IEnumerator>(this.Wander),
				new Func<IEnumerator>(this.Console),
				new Func<IEnumerator>(this.JumpBack),
				new Func<IEnumerator>(this.Collapse),
				new Func<IEnumerator>(this.AwkwardPause)
			});
			this.evil.Add(new SoundSource(Vector2.Zero, "event:/game/03_resort/suite_bad_exittop"));
			yield return this.evil.FloatTo(new Vector2(this.evil.X, (float)(level.Bounds.Top - 32)), null, true, false, false);
			base.Scene.Remove(this.evil);
			while (level.Lighting.Alpha != level.BaseLightingAlpha)
			{
				level.Lighting.Alpha = Calc.Approach(level.Lighting.Alpha, level.BaseLightingAlpha, Engine.DeltaTime * 0.5f);
				yield return null;
			}
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x060013F5 RID: 5109 RVA: 0x0006BFBA File Offset: 0x0006A1BA
		private IEnumerator Wander()
		{
			yield return 0.5f;
			this.player.Facing = Facings.Right;
			yield return 0.1f;
			yield return this.player.DummyWalkToExact((int)this.oshiro.X + 48, false, 1f, false);
			yield return 1f;
			this.player.Facing = Facings.Left;
			yield return 0.2f;
			yield return this.player.DummyWalkToExact((int)this.oshiro.X - 32, false, 1f, false);
			yield return 0.1f;
			this.oshiro.Sprite.Scale.X = -1f;
			yield return 0.2f;
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("lookUp", false, false);
			yield return 1f;
			this.player.DummyAutoAnimate = true;
			yield return 0.4f;
			this.player.Facing = Facings.Right;
			yield return 0.2f;
			yield return this.player.DummyWalkToExact((int)this.oshiro.X - 24, false, 1f, false);
			yield return 0.5f;
			Level level = base.SceneAs<Level>();
			yield return level.ZoomTo(new Vector2(190f, 110f), 2f, 0.5f);
			yield break;
		}

		// Token: 0x060013F6 RID: 5110 RVA: 0x0006BFC9 File Offset: 0x0006A1C9
		private IEnumerator AwkwardPause()
		{
			yield return 2f;
			yield break;
		}

		// Token: 0x060013F7 RID: 5111 RVA: 0x0006BFD1 File Offset: 0x0006A1D1
		private IEnumerator SuiteShadowAppear()
		{
			if (this.mirror == null)
			{
				yield break;
			}
			this.mirror.EvilAppear();
			this.SetMusic();
			Audio.Play("event:/game/03_resort/suite_bad_intro", this.mirror.Position);
			Vector2 from = this.Level.ZoomFocusPoint;
			Vector2 to = new Vector2(216f, 110f);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 2f)
			{
				this.Level.ZoomFocusPoint = from + (to - from) * Ease.SineInOut(p);
				yield return null;
			}
			yield return null;
			yield break;
		}

		// Token: 0x060013F8 RID: 5112 RVA: 0x0006BFE0 File Offset: 0x0006A1E0
		private IEnumerator SuiteShadowDisrupt()
		{
			if (this.mirror == null)
			{
				yield break;
			}
			Audio.Play("event:/game/03_resort/suite_bad_mirrorbreak", this.mirror.Position);
			yield return this.mirror.SmashRoutine();
			this.evil = new BadelineDummy(this.mirror.Position + new Vector2(0f, -8f));
			base.Scene.Add(this.evil);
			yield return 1.2f;
			this.oshiro.Sprite.Scale.X = 1f;
			yield return this.evil.FloatTo(this.oshiro.Position + new Vector2(32f, -24f), null, true, false, false);
			yield break;
		}

		// Token: 0x060013F9 RID: 5113 RVA: 0x0006BFEF File Offset: 0x0006A1EF
		private IEnumerator Collapse()
		{
			this.oshiro.Sprite.Play("fall", false, false);
			Audio.Play("event:/char/oshiro/chat_collapse", this.oshiro.Position);
			yield return null;
			yield break;
		}

		// Token: 0x060013FA RID: 5114 RVA: 0x0006BFFE File Offset: 0x0006A1FE
		private IEnumerator Console()
		{
			yield return this.player.DummyWalkToExact((int)this.oshiro.X - 16, false, 1f, false);
			yield break;
		}

		// Token: 0x060013FB RID: 5115 RVA: 0x0006C00D File Offset: 0x0006A20D
		private IEnumerator JumpBack()
		{
			yield return this.player.DummyWalkToExact((int)this.oshiro.X - 24, true, 1f, false);
			yield return 0.8f;
			yield break;
		}

		// Token: 0x060013FC RID: 5116 RVA: 0x0006C01C File Offset: 0x0006A21C
		private IEnumerator SuiteShadowCeiling()
		{
			yield return base.SceneAs<Level>().ZoomBack(0.5f);
			this.evil.Add(new SoundSource(Vector2.Zero, "event:/game/03_resort/suite_bad_movestageleft"));
			yield return this.evil.FloatTo(new Vector2((float)(this.Level.Bounds.Left + 96), this.evil.Y - 16f), new int?(1), true, false, false);
			this.player.Facing = Facings.Left;
			yield return 0.25f;
			this.evil.Add(new SoundSource(Vector2.Zero, "event:/game/03_resort/suite_bad_ceilingbreak"));
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
			this.Level.DirectionalShake(-Vector2.UnitY, 0.3f);
			yield return this.evil.SmashBlock(this.evil.Position + new Vector2(0f, -32f));
			yield return 0.8f;
			yield break;
		}

		// Token: 0x060013FD RID: 5117 RVA: 0x0006C02C File Offset: 0x0006A22C
		private void SetMusic()
		{
			if (this.Level.Session.Area.Mode == AreaMode.Normal)
			{
				this.Level.Session.Audio.Music.Event = "event:/music/lvl2/evil_madeline";
				this.Level.Session.Audio.Apply(false);
			}
		}

		// Token: 0x060013FE RID: 5118 RVA: 0x0006C088 File Offset: 0x0006A288
		public override void OnEnd(Level level)
		{
			if (this.WasSkipped)
			{
				if (this.evil != null)
				{
					base.Scene.Remove(this.evil);
				}
				if (this.mirror != null)
				{
					this.mirror.Broken();
				}
				DashBlock dashBlock = base.Scene.Entities.FindFirst<DashBlock>();
				if (dashBlock != null)
				{
					dashBlock.RemoveAndFlagAsGone();
				}
				this.oshiro.Sprite.Play("idle_ground", false, false);
			}
			this.oshiro.Talker.Enabled = true;
			if (this.player != null)
			{
				this.player.StateMachine.Locked = false;
				this.player.StateMachine.State = 0;
			}
			level.Lighting.Alpha = level.BaseLightingAlpha;
			level.Session.SetFlag("oshiro_resort_suite", true);
			this.SetMusic();
		}

		// Token: 0x04000FB0 RID: 4016
		public const string Flag = "oshiro_resort_suite";

		// Token: 0x04000FB1 RID: 4017
		private Player player;

		// Token: 0x04000FB2 RID: 4018
		private NPC oshiro;

		// Token: 0x04000FB3 RID: 4019
		private BadelineDummy evil;

		// Token: 0x04000FB4 RID: 4020
		private ResortMirror mirror;
	}
}
