using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200016E RID: 366
	public class CS07_Ending : CutsceneEntity
	{
		// Token: 0x06000D09 RID: 3337 RVA: 0x0002C46C File Offset: 0x0002A66C
		public CS07_Ending(Player player, Vector2 target) : base(false, true)
		{
			this.player = player;
			this.target = target;
		}

		// Token: 0x06000D0A RID: 3338 RVA: 0x0002C484 File Offset: 0x0002A684
		public override void OnBegin(Level level)
		{
			level.RegisterAreaComplete();
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x0002C49F File Offset: 0x0002A69F
		private IEnumerator Cutscene(Level level)
		{
			Audio.SetMusic(null, true, true);
			this.player.StateMachine.State = 11;
			yield return this.player.DummyWalkTo(this.target.X, false, 1f, false);
			yield return 0.25f;
			base.Add(new Coroutine(CutsceneEntity.CameraTo(this.target + new Vector2(-160f, -130f), 3f, Ease.CubeInOut, 0f), true));
			this.player.Facing = Facings.Right;
			yield return 1f;
			this.player.Sprite.Play("idle", false, false);
			this.player.DummyAutoAnimate = false;
			this.player.Dashes = 1;
			level.Session.Inventory.Dashes = 1;
			level.Add(this.badeline = new BadelineDummy(this.player.Center));
			this.player.CreateSplitParticles();
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			this.Level.Displacement.AddBurst(this.player.Center, 0.4f, 8f, 32f, 0.5f, null, null);
			this.badeline.Sprite.Scale.X = 1f;
			Audio.Play("event:/char/badeline/maddy_split", this.player.Position);
			yield return this.badeline.FloatTo(this.target + new Vector2(-10f, -30f), new int?(1), false, false, false);
			yield return 0.5f;
			yield return Textbox.Say("CH7_ENDING", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.WaitABit),
				new Func<IEnumerator>(this.SitDown),
				new Func<IEnumerator>(this.BadelineApproaches)
			});
			yield return 1f;
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000D0C RID: 3340 RVA: 0x0002C4B5 File Offset: 0x0002A6B5
		private IEnumerator WaitABit()
		{
			yield return 3f;
			yield break;
		}

		// Token: 0x06000D0D RID: 3341 RVA: 0x0002C4BD File Offset: 0x0002A6BD
		private IEnumerator SitDown()
		{
			yield return 0.5f;
			this.player.DummyAutoAnimate = true;
			yield return this.player.DummyWalkTo(this.player.X + 16f, false, 0.25f, false);
			yield return 0.1f;
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("sitDown", false, false);
			yield return 1f;
			yield break;
		}

		// Token: 0x06000D0E RID: 3342 RVA: 0x0002C4CC File Offset: 0x0002A6CC
		private IEnumerator BadelineApproaches()
		{
			yield return 0.5f;
			this.badeline.Sprite.Scale.X = -1f;
			yield return 1f;
			this.badeline.Sprite.Scale.X = 1f;
			yield return 1f;
			base.Add(new Coroutine(CutsceneEntity.CameraTo(this.Level.Camera.Position + new Vector2(88f, 0f), 6f, Ease.CubeInOut, 0f), true));
			this.badeline.FloatSpeed = 40f;
			yield return this.badeline.FloatTo(new Vector2(this.player.X - 10f, this.player.Y - 4f), null, true, false, false);
			yield return 0.5f;
			yield break;
		}

		// Token: 0x06000D0F RID: 3343 RVA: 0x0002C4DC File Offset: 0x0002A6DC
		public override void OnEnd(Level level)
		{
			Audio.SetMusic(null, true, true);
			ScreenWipe screenWipe = level.CompleteArea(false, false, false);
			if (screenWipe != null)
			{
				screenWipe.Duration = 2f;
				screenWipe.EndTimer = 1f;
			}
		}

		// Token: 0x04000850 RID: 2128
		private Player player;

		// Token: 0x04000851 RID: 2129
		private BadelineDummy badeline;

		// Token: 0x04000852 RID: 2130
		private Vector2 target;
	}
}
