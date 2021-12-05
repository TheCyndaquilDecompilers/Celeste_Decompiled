using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000288 RID: 648
	public class CS03_OshiroLobby : CutsceneEntity
	{
		// Token: 0x06001407 RID: 5127 RVA: 0x0006C30D File Offset: 0x0006A50D
		public CS03_OshiroLobby(Player player, NPC oshiro) : base(true, false)
		{
			this.player = player;
			this.oshiro = oshiro;
			base.Add(this.sfx);
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x0006C33C File Offset: 0x0006A53C
		public override void Update()
		{
			base.Update();
			if (this.createSparks && this.Level.OnInterval(0.025f))
			{
				Vector2 vector = this.oshiro.Position + new Vector2(0f, -12f) + new Vector2((float)(Calc.Random.Range(4, 12) * Calc.Random.Choose(1, -1)), (float)(Calc.Random.Range(4, 12) * Calc.Random.Choose(1, -1)));
				this.Level.Particles.Emit(NPC03_Oshiro_Lobby.P_AppearSpark, vector, (vector - this.oshiro.Position).Angle());
			}
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x0006C3FB File Offset: 0x0006A5FB
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x0006C410 File Offset: 0x0006A610
		private IEnumerator Cutscene(Level level)
		{
			CS03_OshiroLobby.<>c__DisplayClass9_0 CS$<>8__locals1 = new CS03_OshiroLobby.<>c__DisplayClass9_0();
			CS$<>8__locals1.level = level;
			CS$<>8__locals1.<>4__this = this;
			this.startLightAlpha = CS$<>8__locals1.level.Lighting.Alpha;
			CS$<>8__locals1.endLightAlpha = 1f;
			float from = this.oshiro.Y;
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			yield return 0.5f;
			yield return this.player.DummyWalkTo(this.oshiro.X - 16f, false, 1f, false);
			this.player.Facing = Facings.Right;
			this.sfx.Play("event:/game/03_resort/sequence_oshiro_intro", null, 0f);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			yield return 1.4f;
			CS$<>8__locals1.level.Shake(0.3f);
			CS$<>8__locals1.level.Lighting.Alpha += 0.5f;
			while (CS$<>8__locals1.level.Lighting.Alpha > this.startLightAlpha)
			{
				CS$<>8__locals1.level.Lighting.Alpha -= Engine.DeltaTime * 4f;
				yield return null;
			}
			CS$<>8__locals1.light = new VertexLight(new Vector2(0f, -8f), Color.White, 1f, 32, 64);
			CS$<>8__locals1.bloom = new BloomPoint(new Vector2(0f, -8f), 1f, 16f);
			CS$<>8__locals1.level.Lighting.SetSpotlight(CS$<>8__locals1.light);
			this.oshiro.Add(CS$<>8__locals1.light);
			this.oshiro.Add(CS$<>8__locals1.bloom);
			this.oshiro.Y -= 16f;
			Vector2 target = CS$<>8__locals1.light.Position;
			Tween tween2 = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.5f, true);
			tween2.OnUpdate = delegate(Tween t)
			{
				CS$<>8__locals1.light.Alpha = (CS$<>8__locals1.bloom.Alpha = t.Percent);
				CS$<>8__locals1.light.Position = Vector2.Lerp(target - Vector2.UnitY * 48f, target, t.Percent);
				CS$<>8__locals1.level.Lighting.Alpha = MathHelper.Lerp(CS$<>8__locals1.<>4__this.startLightAlpha, CS$<>8__locals1.endLightAlpha, t.Eased);
			};
			base.Add(tween2);
			yield return tween2.Wait();
			yield return 0.2f;
			yield return CS$<>8__locals1.level.ZoomTo(new Vector2(170f, 126f), 2f, 0.5f);
			yield return 0.6f;
			CS$<>8__locals1.level.Shake(0.3f);
			this.oshiro.Sprite.Visible = true;
			this.oshiro.Sprite.Play("appear", false, false);
			yield return this.player.DummyWalkToExact((int)(this.player.X - 12f), true, 1f, false);
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("shaking", false, false);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.FullSecond);
			yield return 0.6f;
			this.createSparks = true;
			yield return 0.4f;
			this.createSparks = false;
			yield return 0.2f;
			CS$<>8__locals1.level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			yield return 1.4f;
			CS$<>8__locals1.level.Lighting.UnsetSpotlight();
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, 0.5f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				CS$<>8__locals1.level.Lighting.Alpha = MathHelper.Lerp(CS$<>8__locals1.endLightAlpha, CS$<>8__locals1.<>4__this.startLightAlpha, t.Percent);
				CS$<>8__locals1.bloom.Alpha = 1f - t.Percent;
			};
			base.Add(tween);
			while (this.oshiro.Y != from)
			{
				this.oshiro.Y = Calc.Approach(this.oshiro.Y, from, Engine.DeltaTime * 40f);
				yield return null;
			}
			yield return tween.Wait();
			tween = null;
			Audio.SetMusic("event:/music/lvl3/oshiro_theme", true, true);
			this.player.DummyAutoAnimate = true;
			yield return Textbox.Say("CH3_OSHIRO_FRONT_DESK", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.ZoomOut)
			});
			foreach (MrOshiroDoor mrOshiroDoor in base.Scene.Entities.FindAll<MrOshiroDoor>())
			{
				mrOshiroDoor.Open();
			}
			this.oshiro.MoveToAndRemove(new Vector2((float)(CS$<>8__locals1.level.Bounds.Right + 64), this.oshiro.Y));
			this.oshiro.Add(new SoundSource("event:/char/oshiro/move_01_0xa_exit"));
			yield return 1.5f;
			base.EndCutscene(CS$<>8__locals1.level, true);
			yield break;
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x0006C426 File Offset: 0x0006A626
		private IEnumerator ZoomOut()
		{
			yield return 0.2f;
			yield return this.Level.ZoomBack(0.5f);
			yield return 0.2f;
			yield break;
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x0006C438 File Offset: 0x0006A638
		public override void OnEnd(Level level)
		{
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			if (this.WasSkipped)
			{
				foreach (MrOshiroDoor mrOshiroDoor in base.Scene.Entities.FindAll<MrOshiroDoor>())
				{
					mrOshiroDoor.InstantOpen();
				}
			}
			level.Lighting.Alpha = this.startLightAlpha;
			level.Lighting.UnsetSpotlight();
			level.Session.SetFlag("oshiro_resort_talked_1", true);
			level.Session.Audio.Music.Event = "event:/music/lvl3/explore";
			level.Session.Audio.Music.Progress = 1;
			level.Session.Audio.Apply(false);
			if (this.WasSkipped)
			{
				level.Remove(this.oshiro);
			}
		}

		// Token: 0x04000FBB RID: 4027
		public const string Flag = "oshiro_resort_talked_1";

		// Token: 0x04000FBC RID: 4028
		private Player player;

		// Token: 0x04000FBD RID: 4029
		private NPC oshiro;

		// Token: 0x04000FBE RID: 4030
		private float startLightAlpha;

		// Token: 0x04000FBF RID: 4031
		private bool createSparks;

		// Token: 0x04000FC0 RID: 4032
		private SoundSource sfx = new SoundSource();
	}
}
