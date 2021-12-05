using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000184 RID: 388
	public class NPC09_Granny_Inside : NPC
	{
		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000DB1 RID: 3505 RVA: 0x0003059C File Offset: 0x0002E79C
		private bool HasDoorConversation
		{
			get
			{
				return this.Level.Session.GetFlag("granny_door") && !this.Level.Session.GetFlag("granny_door_done");
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000DB2 RID: 3506 RVA: 0x000305CF File Offset: 0x0002E7CF
		private bool talkerEnabled
		{
			get
			{
				return (this.conversation > 0 && this.conversation < 4) || this.HasDoorConversation;
			}
		}

		// Token: 0x06000DB3 RID: 3507 RVA: 0x000305EC File Offset: 0x0002E7EC
		public NPC09_Granny_Inside(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("granny"));
			this.Sprite.Play("idle", false, false);
			base.Add(this.LaughSfx = new GrannyLaughSfx(this.Sprite));
			this.MoveAnim = "walk";
			this.Maxspeed = 40f;
			base.Add(this.talker = new TalkComponent(new Rectangle(-20, -8, 40, 8), new Vector2(0f, -24f), new Action<Player>(this.OnTalk), null));
			this.talker.Enabled = false;
		}

		// Token: 0x06000DB4 RID: 3508 RVA: 0x000306B4 File Offset: 0x0002E8B4
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.conversation = this.Level.Session.GetCounter("granny");
			scene.Add(this.Hahaha = new Hahaha(this.Position + new Vector2(8f, -4f), "", false, null));
			this.Hahaha.Enabled = false;
		}

		// Token: 0x06000DB5 RID: 3509 RVA: 0x0003072C File Offset: 0x0002E92C
		public override void Update()
		{
			if (!this.talking && this.conversation == 0)
			{
				this.player = this.Level.Tracker.GetEntity<Player>();
				if (this.player != null && Math.Abs(this.player.X - base.X) < 48f)
				{
					this.OnTalk(this.player);
				}
			}
			this.talker.Enabled = this.talkerEnabled;
			this.Hahaha.Enabled = (this.Sprite.CurrentAnimationID == "laugh");
			base.Update();
		}

		// Token: 0x06000DB6 RID: 3510 RVA: 0x000307C8 File Offset: 0x0002E9C8
		private void OnTalk(Player player)
		{
			this.player = player;
			(base.Scene as Level).StartCutscene(new Action<Level>(this.EndTalking), true, false, true);
			base.Add(this.talkRoutine = new Coroutine(this.TalkRoutine(player), true));
			this.talking = true;
		}

		// Token: 0x06000DB7 RID: 3511 RVA: 0x0003081E File Offset: 0x0002EA1E
		private IEnumerator TalkRoutine(Player player)
		{
			player.StateMachine.State = 11;
			player.Dashes = 1;
			player.ForceCameraUpdate = true;
			while (!player.OnGround(1))
			{
				yield return null;
			}
			yield return player.DummyWalkToExact((int)base.X - 16, false, 1f, false);
			player.Facing = Facings.Right;
			player.ForceCameraUpdate = false;
			Vector2 zoomPoint = new Vector2(base.X - 8f - this.Level.Camera.X, 110f);
			if (this.HasDoorConversation)
			{
				this.Sprite.Scale.X = -1f;
				yield return this.Level.ZoomTo(zoomPoint, 2f, 0.5f);
				yield return Textbox.Say("APP_OLDLADY_LOCKED", new Func<IEnumerator>[0]);
			}
			else if (this.conversation == 0)
			{
				yield return 0.5f;
				this.Sprite.Scale.X = -1f;
				yield return 0.25f;
				yield return this.Level.ZoomTo(zoomPoint, 2f, 0.5f);
				yield return Textbox.Say("APP_OLDLADY_B", new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(this.StartLaughing),
					new Func<IEnumerator>(this.StopLaughing)
				});
			}
			else if (this.conversation == 1)
			{
				this.Sprite.Scale.X = -1f;
				yield return this.Level.ZoomTo(zoomPoint, 2f, 0.5f);
				yield return Textbox.Say("APP_OLDLADY_C", new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(this.StartLaughing),
					new Func<IEnumerator>(this.StopLaughing)
				});
			}
			else if (this.conversation == 2)
			{
				this.Sprite.Scale.X = -1f;
				yield return this.Level.ZoomTo(zoomPoint, 2f, 0.5f);
				yield return Textbox.Say("APP_OLDLADY_D", new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(this.StartLaughing),
					new Func<IEnumerator>(this.StopLaughing)
				});
			}
			else if (this.conversation == 3)
			{
				this.Sprite.Scale.X = -1f;
				yield return this.Level.ZoomTo(zoomPoint, 2f, 0.5f);
				yield return Textbox.Say("APP_OLDLADY_E", new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(this.StartLaughing),
					new Func<IEnumerator>(this.StopLaughing)
				});
			}
			this.talker.Enabled = this.talkerEnabled;
			yield return this.Level.ZoomBack(0.5f);
			this.Level.EndCutscene();
			this.EndTalking(this.Level);
			yield break;
		}

		// Token: 0x06000DB8 RID: 3512 RVA: 0x00030834 File Offset: 0x0002EA34
		private IEnumerator StartLaughing()
		{
			this.Sprite.Play("laugh", false, false);
			yield return null;
			yield break;
		}

		// Token: 0x06000DB9 RID: 3513 RVA: 0x00030843 File Offset: 0x0002EA43
		private IEnumerator StopLaughing()
		{
			this.Sprite.Play("idle", false, false);
			yield return null;
			yield break;
		}

		// Token: 0x06000DBA RID: 3514 RVA: 0x00030854 File Offset: 0x0002EA54
		private void EndTalking(Level level)
		{
			if (this.player != null)
			{
				this.player.StateMachine.State = 0;
				this.player.ForceCameraUpdate = false;
			}
			if (this.HasDoorConversation)
			{
				this.Level.Session.SetFlag("granny_door_done", true);
			}
			else
			{
				this.Level.Session.IncrementCounter("granny");
				this.conversation++;
			}
			if (this.talkRoutine != null)
			{
				this.talkRoutine.RemoveSelf();
				this.talkRoutine = null;
			}
			this.Sprite.Play("idle", false, false);
			this.talking = false;
		}

		// Token: 0x040008EC RID: 2284
		public const string DoorConversationAvailable = "granny_door";

		// Token: 0x040008ED RID: 2285
		private const string DoorConversationDone = "granny_door_done";

		// Token: 0x040008EE RID: 2286
		private const string CounterFlag = "granny";

		// Token: 0x040008EF RID: 2287
		private int conversation;

		// Token: 0x040008F0 RID: 2288
		private const int MaxConversation = 4;

		// Token: 0x040008F1 RID: 2289
		public Hahaha Hahaha;

		// Token: 0x040008F2 RID: 2290
		public GrannyLaughSfx LaughSfx;

		// Token: 0x040008F3 RID: 2291
		private Player player;

		// Token: 0x040008F4 RID: 2292
		private TalkComponent talker;

		// Token: 0x040008F5 RID: 2293
		private bool talking;

		// Token: 0x040008F6 RID: 2294
		private Coroutine talkRoutine;
	}
}
