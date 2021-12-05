using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000162 RID: 354
	public class CS10_Gravestone : CutsceneEntity
	{
		// Token: 0x06000C9C RID: 3228 RVA: 0x0002A8D3 File Offset: 0x00028AD3
		public CS10_Gravestone(Player player, NPC10_Gravestone gravestone, Vector2 boostTarget) : base(true, false)
		{
			this.player = player;
			this.gravestone = gravestone;
			this.boostTarget = boostTarget;
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x0002A8F2 File Offset: 0x00028AF2
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(), true));
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x0002A906 File Offset: 0x00028B06
		private IEnumerator Cutscene()
		{
			this.player.StateMachine.State = 11;
			this.player.ForceCameraUpdate = true;
			this.player.DummyGravity = false;
			this.player.Speed.Y = 0f;
			yield return 0.1f;
			yield return this.player.DummyWalkToExact((int)this.gravestone.X - 30, false, 1f, false);
			yield return 0.1f;
			this.player.Facing = Facings.Right;
			yield return 0.2f;
			yield return this.Level.ZoomTo(new Vector2(160f, 90f), 2f, 3f);
			this.player.ForceCameraUpdate = false;
			yield return 0.5f;
			yield return Textbox.Say("CH9_GRAVESTONE", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.StepForward),
				new Func<IEnumerator>(this.BadelineAppears),
				new Func<IEnumerator>(this.SitDown)
			});
			yield return 1f;
			yield return this.BirdStuff();
			yield return this.BadelineRejoin();
			yield return 0.1f;
			yield return this.Level.ZoomBack(0.5f);
			yield return 0.3f;
			this.addedBooster = true;
			this.Level.Displacement.AddBurst(this.boostTarget, 0.5f, 8f, 32f, 0.5f, null, null);
			Audio.Play("event:/new_content/char/badeline/booster_first_appear", this.boostTarget);
			this.Level.Add(new BadelineBoost(new Vector2[]
			{
				this.boostTarget
			}, false, false, false, false, false));
			yield return 0.2f;
			base.EndCutscene(this.Level, true);
			yield break;
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x0002A915 File Offset: 0x00028B15
		private IEnumerator StepForward()
		{
			yield return this.player.DummyWalkTo(this.player.X + 8f, false, 1f, false);
			yield break;
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x0002A924 File Offset: 0x00028B24
		private IEnumerator BadelineAppears()
		{
			this.Level.Session.Inventory.Dashes = 1;
			this.player.Dashes = 1;
			Vector2 vector = this.player.Position + new Vector2(-12f, -10f);
			this.Level.Displacement.AddBurst(vector, 0.5f, 8f, 32f, 0.5f, null, null);
			this.Level.Add(this.badeline = new BadelineDummy(vector));
			Audio.Play("event:/char/badeline/maddy_split", vector);
			this.badeline.Sprite.Scale.X = 1f;
			yield return this.badeline.FloatTo(vector + new Vector2(0f, -6f), new int?(1), false, false, false);
			yield break;
		}

		// Token: 0x06000CA1 RID: 3233 RVA: 0x0002A933 File Offset: 0x00028B33
		private IEnumerator SitDown()
		{
			yield return 0.2f;
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("sitDown", false, false);
			yield return 0.3f;
			yield break;
		}

		// Token: 0x06000CA2 RID: 3234 RVA: 0x0002A942 File Offset: 0x00028B42
		private IEnumerator BirdStuff()
		{
			this.bird = new BirdNPC(this.player.Position + new Vector2(88f, -200f), BirdNPC.Modes.None);
			this.bird.DisableFlapSfx = true;
			base.Scene.Add(this.bird);
			EventInstance instance = Audio.Play("event:/game/general/bird_in", this.bird.Position);
			this.bird.Facing = Facings.Left;
			this.bird.Sprite.Play("fall", false, false);
			Vector2 from = this.bird.Position;
			Vector2 to = this.gravestone.Position + new Vector2(1f, -16f);
			float percent = 0f;
			while (percent < 1f)
			{
				this.bird.Position = from + (to - from) * Ease.QuadOut(percent);
				Audio.Position(instance, this.bird.Position);
				if (percent > 0.5f)
				{
					this.bird.Sprite.Play("fly", false, false);
				}
				percent += Engine.DeltaTime * 0.5f;
				yield return null;
			}
			this.bird.Position = to;
			this.bird.Sprite.Play("idle", false, false);
			yield return 0.5f;
			this.bird.Sprite.Play("croak", false, false);
			yield return 0.6f;
			Audio.Play("event:/game/general/bird_squawk", this.bird.Position);
			yield return 0.9f;
			Audio.Play("event:/char/madeline/stand", this.player.Position);
			this.player.Sprite.Play("idle", false, false);
			yield return 1f;
			yield return this.bird.StartleAndFlyAway();
			yield break;
		}

		// Token: 0x06000CA3 RID: 3235 RVA: 0x0002A951 File Offset: 0x00028B51
		private IEnumerator BadelineRejoin()
		{
			Audio.Play("event:/new_content/char/badeline/maddy_join_quick", this.badeline.Position);
			Vector2 from = this.badeline.Position;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.25f)
			{
				this.badeline.Position = Vector2.Lerp(from, this.player.Position, Ease.CubeIn(p));
				yield return null;
			}
			this.Level.Displacement.AddBurst(this.player.Center, 0.5f, 8f, 32f, 0.5f, null, null);
			this.Level.Session.Inventory.Dashes = 2;
			this.player.Dashes = 2;
			this.badeline.RemoveSelf();
			yield break;
		}

		// Token: 0x06000CA4 RID: 3236 RVA: 0x0002A960 File Offset: 0x00028B60
		public override void OnEnd(Level level)
		{
			this.player.Facing = Facings.Right;
			this.player.DummyAutoAnimate = true;
			this.player.DummyGravity = true;
			this.player.StateMachine.State = 0;
			this.Level.Session.Inventory.Dashes = 2;
			this.player.Dashes = 2;
			if (this.badeline != null)
			{
				this.badeline.RemoveSelf();
			}
			if (this.bird != null)
			{
				this.bird.RemoveSelf();
			}
			if (!this.addedBooster)
			{
				level.Add(new BadelineBoost(new Vector2[]
				{
					this.boostTarget
				}, false, false, false, false, false));
			}
			level.ResetZoom();
		}

		// Token: 0x04000808 RID: 2056
		private Player player;

		// Token: 0x04000809 RID: 2057
		private NPC10_Gravestone gravestone;

		// Token: 0x0400080A RID: 2058
		private BadelineDummy badeline;

		// Token: 0x0400080B RID: 2059
		private BirdNPC bird;

		// Token: 0x0400080C RID: 2060
		private Vector2 boostTarget;

		// Token: 0x0400080D RID: 2061
		private bool addedBooster;
	}
}
