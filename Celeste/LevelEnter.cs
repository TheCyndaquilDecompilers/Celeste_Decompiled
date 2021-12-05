using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200024F RID: 591
	public class LevelEnter : Scene
	{
		// Token: 0x0600128E RID: 4750 RVA: 0x0006279C File Offset: 0x0006099C
		public static void Go(Session session, bool fromSaveData)
		{
			HiresSnow snow = null;
			if (Engine.Scene is Overworld)
			{
				snow = (Engine.Scene as Overworld).Snow;
			}
			bool flag = !fromSaveData && session.StartedFromBeginning;
			if (flag && session.Area.ID == 0)
			{
				Engine.Scene = new IntroVignette(session, snow);
				return;
			}
			if (flag && session.Area.ID == 7 && session.Area.Mode == AreaMode.Normal)
			{
				Engine.Scene = new SummitVignette(session);
				return;
			}
			if (flag && session.Area.ID == 9 && session.Area.Mode == AreaMode.Normal)
			{
				Engine.Scene = new CoreVignette(session, snow);
				return;
			}
			Engine.Scene = new LevelEnter(session, fromSaveData);
		}

		// Token: 0x0600128F RID: 4751 RVA: 0x00062854 File Offset: 0x00060A54
		private LevelEnter(Session session, bool fromSaveData)
		{
			this.session = session;
			this.fromSaveData = fromSaveData;
			base.Add(new Entity
			{
				new Coroutine(this.Routine(), true)
			});
			base.Add(new HudRenderer());
		}

		// Token: 0x06001290 RID: 4752 RVA: 0x0006289F File Offset: 0x00060A9F
		private IEnumerator Routine()
		{
			int area = -1;
			if (this.session.StartedFromBeginning && !this.fromSaveData && this.session.Area.Mode == AreaMode.Normal && (!SaveData.Instance.Areas[this.session.Area.ID].Modes[0].Completed || SaveData.Instance.DebugMode) && this.session.Area.ID >= 1 && this.session.Area.ID <= 6)
			{
				area = this.session.Area.ID;
			}
			if (area >= 0)
			{
				yield return 1f;
				base.Add(this.postcard = new Postcard(Dialog.Get("postcard_area_" + area, null), area));
				yield return this.postcard.DisplayRoutine();
			}
			if (this.session.StartedFromBeginning && !this.fromSaveData && this.session.Area.Mode == AreaMode.BSide)
			{
				LevelEnter.BSideTitle title = new LevelEnter.BSideTitle(this.session);
				base.Add(title);
				Audio.Play("event:/ui/main/bside_intro_text");
				yield return title.EaseIn();
				yield return 0.25f;
				yield return title.EaseOut();
				yield return 0.25f;
				title = null;
			}
			Input.SetLightbarColor(AreaData.Get(this.session.Area).TitleBaseColor);
			Engine.Scene = new LevelLoader(this.session, null);
			yield break;
		}

		// Token: 0x06001291 RID: 4753 RVA: 0x000628AE File Offset: 0x00060AAE
		public override void BeforeRender()
		{
			base.BeforeRender();
			if (this.postcard != null)
			{
				this.postcard.BeforeRender();
			}
		}

		// Token: 0x04000E6B RID: 3691
		private Session session;

		// Token: 0x04000E6C RID: 3692
		private Postcard postcard;

		// Token: 0x04000E6D RID: 3693
		private bool fromSaveData;

		// Token: 0x02000573 RID: 1395
		private class BSideTitle : Entity
		{
			// Token: 0x060026AE RID: 9902 RVA: 0x000FE68C File Offset: 0x000FC88C
			public BSideTitle(Session session)
			{
				base.Tag = Tags.HUD;
				int id = session.Area.ID;
				if (id == 1)
				{
					this.artist = Credits.Remixers[0];
				}
				else if (id == 2)
				{
					this.artist = Credits.Remixers[1];
				}
				else if (id == 3)
				{
					this.artist = Credits.Remixers[2];
				}
				else if (id == 4)
				{
					this.artist = Credits.Remixers[3];
				}
				else if (id == 5)
				{
					this.artist = Credits.Remixers[4];
				}
				else if (id == 6)
				{
					this.artist = Credits.Remixers[5];
				}
				else if (id == 7)
				{
					this.artist = Credits.Remixers[6];
				}
				else if (id == 9)
				{
					this.artist = Credits.Remixers[7];
				}
				if (this.artist.StartsWith("image:"))
				{
					this.artistImage = GFX.Gui[this.artist.Substring(6)];
				}
				this.title = Dialog.Get(AreaData.Get(session).Name, null) + " " + Dialog.Get(AreaData.Get(session).Name + "_remix", null);
				this.musicBy = Dialog.Get("remix_by", null) + " ";
				this.musicByWidth = ActiveFont.Measure(this.musicBy).X;
				this.album = Dialog.Get("remix_album", null);
			}

			// Token: 0x060026AF RID: 9903 RVA: 0x000FE819 File Offset: 0x000FCA19
			public IEnumerator EaseIn()
			{
				base.Add(new Coroutine(this.FadeTo(0, 1f, 1f), true));
				yield return 0.2f;
				base.Add(new Coroutine(this.FadeTo(1, 1f, 1f), true));
				yield return 0.2f;
				base.Add(new Coroutine(this.FadeTo(2, 1f, 1f), true));
				yield return 1.8f;
				yield break;
			}

			// Token: 0x060026B0 RID: 9904 RVA: 0x000FE828 File Offset: 0x000FCA28
			public IEnumerator EaseOut()
			{
				base.Add(new Coroutine(this.FadeTo(0, 0f, 1f), true));
				yield return 0.2f;
				base.Add(new Coroutine(this.FadeTo(1, 0f, 1f), true));
				yield return 0.2f;
				base.Add(new Coroutine(this.FadeTo(2, 0f, 1f), true));
				yield return 1f;
				yield break;
			}

			// Token: 0x060026B1 RID: 9905 RVA: 0x000FE837 File Offset: 0x000FCA37
			private IEnumerator FadeTo(int index, float target, float duration)
			{
				while ((this.fade[index] = Calc.Approach(this.fade[index], target, Engine.DeltaTime / duration)) != target)
				{
					if (target == 0f)
					{
						this.offsets[index] = Ease.CubeIn(1f - this.fade[index]) * 32f;
					}
					else
					{
						this.offsets[index] = -Ease.CubeIn(1f - this.fade[index]) * 32f;
					}
					yield return null;
				}
				yield break;
			}

			// Token: 0x060026B2 RID: 9906 RVA: 0x000FE85B File Offset: 0x000FCA5B
			public override void Update()
			{
				base.Update();
				this.offset += Engine.DeltaTime * 32f;
			}

			// Token: 0x060026B3 RID: 9907 RVA: 0x000FE87C File Offset: 0x000FCA7C
			public override void Render()
			{
				Vector2 value = new Vector2(60f + this.offset, 800f);
				ActiveFont.Draw(this.title, value + new Vector2(this.offsets[0], 0f), Color.White * this.fade[0]);
				ActiveFont.Draw(this.musicBy, value + new Vector2(this.offsets[1], 60f), Color.White * this.fade[1]);
				if (this.artistImage != null)
				{
					this.artistImage.Draw(value + new Vector2(this.musicByWidth + this.offsets[1], 68f), Vector2.Zero, Color.White * this.fade[1]);
				}
				else
				{
					ActiveFont.Draw(this.artist, value + new Vector2(this.musicByWidth + this.offsets[1], 60f), Color.White * this.fade[1]);
				}
				ActiveFont.Draw(this.album, value + new Vector2(this.offsets[2], 120f), Color.White * this.fade[2]);
			}

			// Token: 0x0400268F RID: 9871
			private string title;

			// Token: 0x04002690 RID: 9872
			private string musicBy;

			// Token: 0x04002691 RID: 9873
			private string artist;

			// Token: 0x04002692 RID: 9874
			private MTexture artistImage;

			// Token: 0x04002693 RID: 9875
			private string album;

			// Token: 0x04002694 RID: 9876
			private float musicByWidth;

			// Token: 0x04002695 RID: 9877
			private float[] fade = new float[3];

			// Token: 0x04002696 RID: 9878
			private float[] offsets = new float[3];

			// Token: 0x04002697 RID: 9879
			private float offset;
		}
	}
}
