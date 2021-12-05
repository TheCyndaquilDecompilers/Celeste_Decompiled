using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200027B RID: 635
	public class CS00_Granny : CutsceneEntity
	{
		// Token: 0x060013A8 RID: 5032 RVA: 0x0006AF38 File Offset: 0x00069138
		public CS00_Granny(NPC00_Granny granny, Player player) : base(true, false)
		{
			this.granny = granny;
			this.player = player;
			this.endPlayerPosition = granny.Position + new Vector2(48f, 0f);
		}

		// Token: 0x060013A9 RID: 5033 RVA: 0x0006AF70 File Offset: 0x00069170
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(), true));
		}

		// Token: 0x060013AA RID: 5034 RVA: 0x0006AF84 File Offset: 0x00069184
		private IEnumerator Cutscene()
		{
			this.player.StateMachine.State = 11;
			if (Math.Abs(this.player.X - this.granny.X) < 20f)
			{
				yield return this.player.DummyWalkTo(this.granny.X - 48f, false, 1f, false);
			}
			this.player.Facing = Facings.Right;
			yield return 0.5f;
			yield return Textbox.Say("CH0_GRANNY", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.Meet),
				new Func<IEnumerator>(this.RunAlong),
				new Func<IEnumerator>(this.LaughAndAirQuotes),
				new Func<IEnumerator>(this.Laugh),
				new Func<IEnumerator>(this.StopLaughing),
				new Func<IEnumerator>(this.OminousZoom),
				new Func<IEnumerator>(this.PanToMaddy)
			});
			yield return this.Level.ZoomBack(0.5f);
			base.EndCutscene(this.Level, true);
			yield break;
		}

		// Token: 0x060013AB RID: 5035 RVA: 0x0006AF93 File Offset: 0x00069193
		private IEnumerator Meet()
		{
			yield return 0.25f;
			this.granny.Sprite.Scale.X = (float)Math.Sign(this.player.X - this.granny.X);
			yield return this.player.DummyWalkTo(this.granny.X - 20f, false, 1f, false);
			this.player.Facing = Facings.Right;
			yield return 0.8f;
			yield break;
		}

		// Token: 0x060013AC RID: 5036 RVA: 0x0006AFA2 File Offset: 0x000691A2
		private IEnumerator RunAlong()
		{
			yield return this.player.DummyWalkToExact((int)this.endPlayerPosition.X, false, 1f, false);
			yield return 0.8f;
			this.player.Facing = Facings.Left;
			yield return 0.4f;
			this.granny.Sprite.Scale.X = 1f;
			yield return this.Level.ZoomTo(new Vector2(210f, 90f), 2f, 0.5f);
			yield return 0.2f;
			yield break;
		}

		// Token: 0x060013AD RID: 5037 RVA: 0x0006AFB1 File Offset: 0x000691B1
		private IEnumerator LaughAndAirQuotes()
		{
			yield return 0.6f;
			this.granny.LaughSfx.FirstPlay = true;
			this.granny.Sprite.Play("laugh", false, false);
			yield return 2f;
			this.granny.Sprite.Play("airQuotes", false, false);
			yield break;
		}

		// Token: 0x060013AE RID: 5038 RVA: 0x0006AFC0 File Offset: 0x000691C0
		private IEnumerator Laugh()
		{
			this.granny.LaughSfx.FirstPlay = false;
			yield return null;
			this.granny.Sprite.Play("laugh", false, false);
			yield break;
		}

		// Token: 0x060013AF RID: 5039 RVA: 0x0006AFCF File Offset: 0x000691CF
		private IEnumerator StopLaughing()
		{
			this.granny.Sprite.Play("idle", false, false);
			yield break;
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x0006AFDE File Offset: 0x000691DE
		private IEnumerator OminousZoom()
		{
			Vector2 screenSpaceFocusPoint = new Vector2(210f, 100f);
			this.zoomCoroutine = new Coroutine(this.Level.ZoomAcross(screenSpaceFocusPoint, 4f, 3f), true);
			base.Add(this.zoomCoroutine);
			this.granny.Sprite.Play("idle", false, false);
			yield return 0.2f;
			yield break;
		}

		// Token: 0x060013B1 RID: 5041 RVA: 0x0006AFED File Offset: 0x000691ED
		private IEnumerator PanToMaddy()
		{
			while (this.zoomCoroutine != null && this.zoomCoroutine.Active)
			{
				yield return null;
			}
			yield return 0.2f;
			yield return this.Level.ZoomAcross(new Vector2(210f, 90f), 2f, 0.5f);
			yield return 0.2f;
			yield break;
		}

		// Token: 0x060013B2 RID: 5042 RVA: 0x0006AFFC File Offset: 0x000691FC
		public override void OnEnd(Level level)
		{
			this.granny.Hahaha.Enabled = true;
			this.granny.Sprite.Play("laugh", false, false);
			this.granny.Sprite.Scale.X = 1f;
			this.player.Position.X = this.endPlayerPosition.X;
			this.player.Facing = Facings.Left;
			this.player.StateMachine.State = 0;
			level.Session.SetFlag("granny", true);
			level.ResetZoom();
		}

		// Token: 0x04000F79 RID: 3961
		public const string Flag = "granny";

		// Token: 0x04000F7A RID: 3962
		private NPC00_Granny granny;

		// Token: 0x04000F7B RID: 3963
		private Player player;

		// Token: 0x04000F7C RID: 3964
		private Vector2 endPlayerPosition;

		// Token: 0x04000F7D RID: 3965
		private Coroutine zoomCoroutine;
	}
}
