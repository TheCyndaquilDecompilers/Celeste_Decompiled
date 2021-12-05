using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200027F RID: 639
	public class CS02_BadelineIntro : CutsceneEntity
	{
		// Token: 0x060013C1 RID: 5057 RVA: 0x0006B380 File Offset: 0x00069580
		public CS02_BadelineIntro(BadelineOldsite badeline) : base(true, false)
		{
			this.badeline = badeline;
			this.badelineEndPosition = badeline.Position + new Vector2(8f, -24f);
			base.Add(this.anxietySine = new SineWave(0.3f, 0f));
			Distort.AnxietyOrigin = new Vector2(0.5f, 0.75f);
		}

		// Token: 0x060013C2 RID: 5058 RVA: 0x0006B3EE File Offset: 0x000695EE
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x060013C3 RID: 5059 RVA: 0x0006B404 File Offset: 0x00069604
		public override void Update()
		{
			base.Update();
			this.anxietyFade = Calc.Approach(this.anxietyFade, this.anxietyFadeTarget, 2.5f * Engine.DeltaTime);
			if (base.Scene.OnInterval(0.1f))
			{
				this.anxietyJitter = Calc.Random.Range(-0.1f, 0.1f);
			}
			Distort.Anxiety = this.anxietyFade * Math.Max(0f, 0f + this.anxietyJitter + this.anxietySine.Value * 0.3f);
		}

		// Token: 0x060013C4 RID: 5060 RVA: 0x0006B499 File Offset: 0x00069699
		private IEnumerator Cutscene(Level level)
		{
			this.anxietyFadeTarget = 1f;
			for (;;)
			{
				this.player = level.Tracker.GetEntity<Player>();
				if (this.player != null)
				{
					break;
				}
				yield return null;
			}
			while (!this.player.OnGround(1))
			{
				yield return null;
			}
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			yield return 1f;
			if (level.Session.Area.Mode == AreaMode.Normal)
			{
				Audio.SetMusic("event:/music/lvl2/evil_madeline", true, true);
			}
			yield return Textbox.Say("CH2_BADELINE_INTRO", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.TurnAround),
				new Func<IEnumerator>(this.RevealBadeline),
				new Func<IEnumerator>(this.StartLaughing),
				new Func<IEnumerator>(this.StopLaughing)
			});
			this.anxietyFadeTarget = 0f;
			yield return this.Level.ZoomBack(0.5f);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x060013C5 RID: 5061 RVA: 0x0006B4AF File Offset: 0x000696AF
		private IEnumerator TurnAround()
		{
			this.player.Facing = Facings.Left;
			yield return 0.2f;
			base.Add(new Coroutine(CutsceneEntity.CameraTo(new Vector2((float)this.Level.Bounds.X, this.Level.Camera.Y), 0.5f, null, 0f), true));
			yield return this.Level.ZoomTo(new Vector2(84f, 135f), 2f, 0.5f);
			yield return 0.2f;
			yield break;
		}

		// Token: 0x060013C6 RID: 5062 RVA: 0x0006B4BE File Offset: 0x000696BE
		private IEnumerator RevealBadeline()
		{
			Audio.Play("event:/game/02_old_site/sequence_badeline_intro", this.badeline.Position);
			yield return 0.1f;
			this.Level.Displacement.AddBurst(this.badeline.Position + new Vector2(0f, -4f), 0.8f, 8f, 48f, 0.5f, null, null);
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			yield return 0.1f;
			this.badeline.Hovering = true;
			this.badeline.Hair.Visible = true;
			this.badeline.Sprite.Play("fallSlow", false, false);
			Vector2 from = this.badeline.Position;
			Vector2 to = this.badelineEndPosition;
			for (float t = 0f; t < 1f; t += Engine.DeltaTime)
			{
				this.badeline.Position = from + (to - from) * Ease.CubeInOut(t);
				yield return null;
			}
			this.player.Facing = (Facings)Math.Sign(this.badeline.X - this.player.X);
			yield return 1f;
			yield break;
		}

		// Token: 0x060013C7 RID: 5063 RVA: 0x0006B4CD File Offset: 0x000696CD
		private IEnumerator StartLaughing()
		{
			yield return 0.2f;
			this.badeline.Sprite.Play("laugh", true, false);
			yield return null;
			yield break;
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x0006B4DC File Offset: 0x000696DC
		private IEnumerator StopLaughing()
		{
			this.badeline.Sprite.Play("fallSlow", true, false);
			yield return null;
			yield break;
		}

		// Token: 0x060013C9 RID: 5065 RVA: 0x0006B4EC File Offset: 0x000696EC
		public override void OnEnd(Level level)
		{
			Audio.SetMusic(null, true, true);
			Distort.Anxiety = 0f;
			if (this.player != null)
			{
				this.player.StateMachine.Locked = false;
				this.player.Facing = Facings.Left;
				this.player.StateMachine.State = 0;
				this.player.JustRespawned = true;
			}
			this.badeline.Position = this.badelineEndPosition;
			this.badeline.Visible = true;
			this.badeline.Hair.Visible = true;
			this.badeline.Sprite.Play("fallSlow", false, false);
			this.badeline.Hovering = false;
			this.badeline.Add(new Coroutine(this.badeline.StartChasingRoutine(level), true));
			level.Session.SetFlag("evil_maddy_intro", true);
		}

		// Token: 0x04000F88 RID: 3976
		public const string Flag = "evil_maddy_intro";

		// Token: 0x04000F89 RID: 3977
		private Player player;

		// Token: 0x04000F8A RID: 3978
		private BadelineOldsite badeline;

		// Token: 0x04000F8B RID: 3979
		private Vector2 badelineEndPosition;

		// Token: 0x04000F8C RID: 3980
		private float anxietyFade;

		// Token: 0x04000F8D RID: 3981
		private float anxietyFadeTarget;

		// Token: 0x04000F8E RID: 3982
		private SineWave anxietySine;

		// Token: 0x04000F8F RID: 3983
		private float anxietyJitter;
	}
}
