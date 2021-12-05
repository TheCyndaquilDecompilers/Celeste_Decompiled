using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000273 RID: 627
	public class NPC02_Theo : NPC
	{
		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06001376 RID: 4982 RVA: 0x00069E63 File Offset: 0x00068063
		// (set) Token: 0x06001377 RID: 4983 RVA: 0x00069E75 File Offset: 0x00068075
		private int CurrentConversation
		{
			get
			{
				return base.Session.GetCounter("theo");
			}
			set
			{
				base.Session.SetCounter("theo", value);
			}
		}

		// Token: 0x06001378 RID: 4984 RVA: 0x00069E88 File Offset: 0x00068088
		public NPC02_Theo(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("theo"));
			this.Sprite.Play("idle", false, false);
		}

		// Token: 0x06001379 RID: 4985 RVA: 0x00069ECC File Offset: 0x000680CC
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (!base.Session.GetFlag("theoDoneTalking"))
			{
				base.Add(this.Talker = new TalkComponent(new Rectangle(-20, -8, 100, 8), new Vector2(0f, -24f), new Action<Player>(this.OnTalk), null));
			}
		}

		// Token: 0x0600137A RID: 4986 RVA: 0x00069F30 File Offset: 0x00068130
		private void OnTalk(Player player)
		{
			if (!SaveData.Instance.HasFlag("MetTheo") || !SaveData.Instance.HasFlag("TheoKnowsName"))
			{
				this.CurrentConversation = -1;
			}
			this.Level.StartCutscene(new Action<Level>(this.OnTalkEnd), true, false, true);
			base.Add(this.talkRoutine = new Coroutine(this.Talk(player), true));
		}

		// Token: 0x0600137B RID: 4987 RVA: 0x00069F9C File Offset: 0x0006819C
		private IEnumerator Talk(Player player)
		{
			if (!SaveData.Instance.HasFlag("MetTheo"))
			{
				base.Session.SetFlag("hadntMetTheoAtStart", true);
				SaveData.Instance.SetFlag("MetTheo");
				yield return base.PlayerApproachRightSide(player, true, new float?((float)48));
				yield return Textbox.Say("CH2_THEO_INTRO_NEVER_MET", new Func<IEnumerator>[0]);
			}
			else if (!SaveData.Instance.HasFlag("TheoKnowsName"))
			{
				base.Session.SetFlag("hadntMetTheoAtStart", true);
				SaveData.Instance.SetFlag("TheoKnowsName");
				yield return base.PlayerApproachRightSide(player, true, new float?((float)48));
				yield return Textbox.Say("CH2_THEO_INTRO_NEVER_INTRODUCED", new Func<IEnumerator>[0]);
			}
			else if (this.CurrentConversation <= 0)
			{
				yield return base.PlayerApproachRightSide(player, true, null);
				yield return 0.2f;
				if (base.Session.GetFlag("hadntMetTheoAtStart"))
				{
					yield return base.PlayerApproach48px();
					yield return Textbox.Say("CH2_THEO_A", new Func<IEnumerator>[]
					{
						new Func<IEnumerator>(this.ShowPhotos),
						new Func<IEnumerator>(this.HidePhotos),
						new Func<IEnumerator>(this.Selfie)
					});
				}
				else
				{
					yield return Textbox.Say("CH2_THEO_A_EXT", new Func<IEnumerator>[]
					{
						new Func<IEnumerator>(this.ShowPhotos),
						new Func<IEnumerator>(this.HidePhotos),
						new Func<IEnumerator>(this.Selfie),
						new Func<IEnumerator>(base.PlayerApproach48px)
					});
				}
			}
			else if (this.CurrentConversation == 1)
			{
				yield return base.PlayerApproachRightSide(player, true, new float?((float)48));
				yield return Textbox.Say("CH2_THEO_B", new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(this.SelfieFiltered)
				});
			}
			else if (this.CurrentConversation == 2)
			{
				yield return base.PlayerApproachRightSide(player, true, new float?((float)48));
				yield return Textbox.Say("CH2_THEO_C", new Func<IEnumerator>[0]);
			}
			else if (this.CurrentConversation == 3)
			{
				yield return base.PlayerApproachRightSide(player, true, new float?((float)48));
				yield return Textbox.Say("CH2_THEO_D", new Func<IEnumerator>[0]);
			}
			else if (this.CurrentConversation == 4)
			{
				yield return base.PlayerApproachRightSide(player, true, new float?((float)48));
				yield return Textbox.Say("CH2_THEO_E", new Func<IEnumerator>[0]);
			}
			this.Level.EndCutscene();
			this.OnTalkEnd(this.Level);
			yield break;
		}

		// Token: 0x0600137C RID: 4988 RVA: 0x00069FB4 File Offset: 0x000681B4
		private void OnTalkEnd(Level level)
		{
			if (this.CurrentConversation == 4)
			{
				base.Session.SetFlag("theoDoneTalking", true);
				base.Remove(this.Talker);
			}
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				entity.StateMachine.Locked = false;
				entity.StateMachine.State = 0;
				if (level.SkippingCutscene)
				{
					entity.X = (float)((int)(base.X + 48f));
					entity.Facing = Facings.Left;
				}
			}
			this.Sprite.Scale.X = 1f;
			if (this.selfie != null)
			{
				this.selfie.RemoveSelf();
			}
			int currentConversation = this.CurrentConversation;
			this.CurrentConversation = currentConversation + 1;
			this.talkRoutine.Cancel();
			this.talkRoutine.RemoveSelf();
		}

		// Token: 0x0600137D RID: 4989 RVA: 0x0006A084 File Offset: 0x00068284
		private IEnumerator ShowPhotos()
		{
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			yield return base.PlayerApproach(entity, true, new float?((float)10), null);
			this.Sprite.Play("getPhone", false, false);
			yield return 2f;
			yield break;
		}

		// Token: 0x0600137E RID: 4990 RVA: 0x0006A093 File Offset: 0x00068293
		private IEnumerator HidePhotos()
		{
			this.Sprite.Play("idle", false, false);
			yield return 0.5f;
			yield break;
		}

		// Token: 0x0600137F RID: 4991 RVA: 0x0006A0A2 File Offset: 0x000682A2
		private IEnumerator Selfie()
		{
			yield return 0.5f;
			Audio.Play("event:/game/02_old_site/theoselfie_foley", this.Position);
			this.Sprite.Scale.X = -this.Sprite.Scale.X;
			this.Sprite.Play("takeSelfie", false, false);
			yield return 1f;
			base.Scene.Add(this.selfie = new Selfie(base.SceneAs<Level>()));
			yield return this.selfie.PictureRoutine("selfie");
			this.selfie = null;
			this.Sprite.Scale.X = -this.Sprite.Scale.X;
			yield break;
		}

		// Token: 0x06001380 RID: 4992 RVA: 0x0006A0B1 File Offset: 0x000682B1
		private IEnumerator SelfieFiltered()
		{
			base.Scene.Add(this.selfie = new Selfie(base.SceneAs<Level>()));
			yield return this.selfie.FilterRoutine();
			this.selfie = null;
			yield break;
		}

		// Token: 0x04000F51 RID: 3921
		private const string DoneTalking = "theoDoneTalking";

		// Token: 0x04000F52 RID: 3922
		private const string HadntMetAtStart = "hadntMetTheoAtStart";

		// Token: 0x04000F53 RID: 3923
		private Coroutine talkRoutine;

		// Token: 0x04000F54 RID: 3924
		private Selfie selfie;
	}
}
