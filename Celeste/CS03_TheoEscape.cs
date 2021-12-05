using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000289 RID: 649
	public class CS03_TheoEscape : CutsceneEntity
	{
		// Token: 0x0600140D RID: 5133 RVA: 0x0006C540 File Offset: 0x0006A740
		public CS03_TheoEscape(NPC03_Theo_Escaping theo, Player player) : base(true, false)
		{
			this.theo = theo;
			this.theoStart = theo.Position;
			this.player = player;
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x0006C564 File Offset: 0x0006A764
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x0006C579 File Offset: 0x0006A779
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			yield return this.player.DummyWalkTo(this.theo.X - 64f, false, 1f, false);
			this.player.Facing = Facings.Right;
			yield return this.Level.ZoomTo(new Vector2(240f, 135f), 2f, 0.5f);
			Func<IEnumerator>[] events = new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.StopRemovingVent),
				new Func<IEnumerator>(this.StartRemoveVent),
				new Func<IEnumerator>(this.RemoveVent),
				new Func<IEnumerator>(this.GivePhone)
			};
			string dialog = "CH3_THEO_INTRO";
			if (!SaveData.Instance.HasFlag("MetTheo"))
			{
				dialog = "CH3_THEO_NEVER_MET";
			}
			else if (!SaveData.Instance.HasFlag("TheoKnowsName"))
			{
				dialog = "CH3_THEO_NEVER_INTRODUCED";
			}
			yield return Textbox.Say(dialog, events);
			this.theo.Sprite.Scale.X = 1f;
			yield return 0.2f;
			this.theo.Sprite.Play("walk", false, false);
			while (!this.theo.CollideCheck<Solid>(this.theo.Position + new Vector2(2f, 0f)))
			{
				yield return null;
				this.theo.X += 48f * Engine.DeltaTime;
			}
			this.theo.Sprite.Play("idle", false, false);
			yield return 0.2f;
			Audio.Play("event:/char/theo/resort_standtocrawl", this.theo.Position);
			this.theo.Sprite.Play("duck", false, false);
			yield return 0.5f;
			if (this.theo.Talker != null)
			{
				this.theo.Talker.Active = false;
			}
			level.Session.SetFlag("resort_theo", true);
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			this.theo.CrawlUntilOut();
			yield return level.ZoomBack(0.5f);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x0006C58F File Offset: 0x0006A78F
		private IEnumerator StartRemoveVent()
		{
			this.theo.Sprite.Scale.X = 1f;
			yield return 0.1f;
			Audio.Play("event:/char/theo/resort_vent_grab", this.theo.Position);
			this.theo.Sprite.Play("goToVent", false, false);
			yield return 0.25f;
			yield break;
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x0006C59E File Offset: 0x0006A79E
		private IEnumerator StopRemovingVent()
		{
			this.theo.Sprite.Play("idle", false, false);
			yield return 0.1f;
			this.theo.Sprite.Scale.X = -1f;
			yield break;
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x0006C5AD File Offset: 0x0006A7AD
		private IEnumerator RemoveVent()
		{
			yield return 0.8f;
			Audio.Play("event:/char/theo/resort_vent_rip", this.theo.Position);
			this.theo.Sprite.Play("fallVent", false, false);
			yield return 0.8f;
			this.theo.grate.Fall();
			yield return 0.8f;
			this.theo.Sprite.Scale.X = -1f;
			yield return 0.25f;
			yield break;
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x0006C5BC File Offset: 0x0006A7BC
		private IEnumerator GivePhone()
		{
			Player player = base.Scene.Tracker.GetEntity<Player>();
			if (player != null)
			{
				this.theo.Sprite.Play("walk", false, false);
				this.theo.Sprite.Scale.X = -1f;
				while (this.theo.X > player.X + 24f)
				{
					this.theo.X -= 48f * Engine.DeltaTime;
					yield return null;
				}
			}
			this.theo.Sprite.Play("idle", false, false);
			yield return 1f;
			yield break;
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x0006C5CC File Offset: 0x0006A7CC
		public override void OnEnd(Level level)
		{
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			level.Session.SetFlag("resort_theo", true);
			SaveData.Instance.SetFlag("MetTheo");
			SaveData.Instance.SetFlag("TheoKnowsName");
			if (this.theo != null && this.WasSkipped)
			{
				this.theo.Position = this.theoStart;
				this.theo.CrawlUntilOut();
				if (this.theo.grate != null)
				{
					this.theo.grate.RemoveSelf();
				}
			}
		}

		// Token: 0x04000FC1 RID: 4033
		public const string Flag = "resort_theo";

		// Token: 0x04000FC2 RID: 4034
		private NPC03_Theo_Escaping theo;

		// Token: 0x04000FC3 RID: 4035
		private Player player;

		// Token: 0x04000FC4 RID: 4036
		private Vector2 theoStart;
	}
}
