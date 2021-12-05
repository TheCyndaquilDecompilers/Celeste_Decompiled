using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000299 RID: 665
	public class OuiJournal : Oui
	{
		// Token: 0x17000168 RID: 360
		// (get) Token: 0x060014AC RID: 5292 RVA: 0x00072775 File Offset: 0x00070975
		public OuiJournalPage Page
		{
			get
			{
				return this.Pages[this.PageIndex];
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x060014AD RID: 5293 RVA: 0x00072788 File Offset: 0x00070988
		public OuiJournalPage NextPage
		{
			get
			{
				return this.Pages[this.PageIndex + 1];
			}
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x060014AE RID: 5294 RVA: 0x0007279D File Offset: 0x0007099D
		public OuiJournalPage PrevPage
		{
			get
			{
				return this.Pages[this.PageIndex - 1];
			}
		}

		// Token: 0x060014AF RID: 5295 RVA: 0x000727B2 File Offset: 0x000709B2
		public override IEnumerator Enter(Oui from)
		{
			Stats.MakeRequest();
			base.Overworld.ShowConfirmUI = false;
			this.fromAreaInspect = (from is OuiChapterPanel);
			this.PageIndex = 0;
			this.Visible = true;
			base.X = -1920f;
			this.turningPage = false;
			this.turningScale = 1f;
			this.rotation = 0f;
			this.dot = 0f;
			this.dotTarget = 0f;
			this.dotEase = 0f;
			this.leftArrowEase = 0f;
			this.rightArrowEase = 0f;
			this.NextPageBuffer = VirtualContent.CreateRenderTarget("journal-a", 1610, 1000, false, true, 0);
			this.CurrentPageBuffer = VirtualContent.CreateRenderTarget("journal-b", 1610, 1000, false, true, 0);
			this.Pages.Add(new OuiJournalCover(this));
			this.Pages.Add(new OuiJournalProgress(this));
			this.Pages.Add(new OuiJournalSpeedrun(this));
			this.Pages.Add(new OuiJournalDeaths(this));
			this.Pages.Add(new OuiJournalPoem(this));
			if (Stats.Has())
			{
				this.Pages.Add(new OuiJournalGlobal(this));
			}
			int num = 0;
			foreach (OuiJournalPage ouiJournalPage in this.Pages)
			{
				ouiJournalPage.PageIndex = num++;
			}
			this.Pages[0].Redraw(this.CurrentPageBuffer);
			this.cameraStart = base.Overworld.Mountain.UntiltedCamera;
			this.cameraEnd = this.cameraStart;
			this.cameraEnd.Position = this.cameraEnd.Position + -this.cameraStart.Rotation.Forward() * 1f;
			base.Overworld.Mountain.EaseCamera(base.Overworld.Mountain.Area, this.cameraEnd, new float?(2f), true, false);
			base.Overworld.Mountain.AllowUserRotation = false;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.4f)
			{
				this.rotation = -0.025f * Ease.BackOut(p);
				base.X = -1920f + 1920f * Ease.CubeInOut(p);
				this.dotEase = p;
				yield return null;
			}
			this.dotEase = 1f;
			yield break;
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x000727C8 File Offset: 0x000709C8
		public override void HandleGraphicsReset()
		{
			base.HandleGraphicsReset();
			if (this.Pages.Count > 0)
			{
				this.Page.Redraw(this.CurrentPageBuffer);
			}
		}

		// Token: 0x060014B1 RID: 5297 RVA: 0x000727EF File Offset: 0x000709EF
		public IEnumerator TurnPage(int direction)
		{
			this.turningPage = true;
			if (direction < 0)
			{
				this.PageIndex--;
				this.turningScale = -1f;
				this.dotTarget -= 1f;
				this.Page.Redraw(this.CurrentPageBuffer);
				this.NextPage.Redraw(this.NextPageBuffer);
				while ((this.turningScale = Calc.Approach(this.turningScale, 1f, Engine.DeltaTime * 8f)) < 1f)
				{
					yield return null;
				}
			}
			else
			{
				this.NextPage.Redraw(this.NextPageBuffer);
				this.turningScale = 1f;
				this.dotTarget += 1f;
				while ((this.turningScale = Calc.Approach(this.turningScale, -1f, Engine.DeltaTime * 8f)) > -1f)
				{
					yield return null;
				}
				this.PageIndex++;
				this.Page.Redraw(this.CurrentPageBuffer);
			}
			this.turningScale = 1f;
			this.turningPage = false;
			yield break;
		}

		// Token: 0x060014B2 RID: 5298 RVA: 0x00072805 File Offset: 0x00070A05
		public override IEnumerator Leave(Oui next)
		{
			Audio.Play("event:/ui/world_map/journal/back");
			base.Overworld.Mountain.EaseCamera(base.Overworld.Mountain.Area, this.cameraStart, new float?(0.4f), true, false);
			UserIO.SaveHandler(false, true);
			yield return this.EaseOut(0.4f);
			while (UserIO.Saving)
			{
				yield return null;
			}
			this.CurrentPageBuffer.Dispose();
			this.NextPageBuffer.Dispose();
			base.Overworld.ShowConfirmUI = true;
			this.Pages.Clear();
			this.Visible = false;
			base.Overworld.Mountain.AllowUserRotation = true;
			yield break;
		}

		// Token: 0x060014B3 RID: 5299 RVA: 0x00072814 File Offset: 0x00070A14
		private IEnumerator EaseOut(float duration)
		{
			float rotFrom = this.rotation;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
			{
				this.rotation = rotFrom * (1f - Ease.BackOut(p));
				base.X = 0f + -1920f * Ease.CubeInOut(p);
				this.dotEase = 1f - p;
				yield return null;
			}
			this.dotEase = 0f;
			yield break;
		}

		// Token: 0x060014B4 RID: 5300 RVA: 0x0007282C File Offset: 0x00070A2C
		public override void Update()
		{
			base.Update();
			this.dot = Calc.Approach(this.dot, this.dotTarget, Engine.DeltaTime * 8f);
			this.leftArrowEase = Calc.Approach(this.leftArrowEase, (float)((this.dotTarget > 0f) ? 1 : 0), Engine.DeltaTime * 5f) * this.dotEase;
			this.rightArrowEase = Calc.Approach(this.rightArrowEase, (float)((this.dotTarget < (float)(this.Pages.Count - 1)) ? 1 : 0), Engine.DeltaTime * 5f) * this.dotEase;
			if (this.Focused && !this.turningPage)
			{
				this.Page.Update();
				if (!this.PageTurningLocked)
				{
					if (Input.MenuLeft.Pressed && this.PageIndex > 0)
					{
						if (this.PageIndex == 1)
						{
							Audio.Play("event:/ui/world_map/journal/page_cover_back");
						}
						else
						{
							Audio.Play("event:/ui/world_map/journal/page_main_back");
						}
						base.Add(new Coroutine(this.TurnPage(-1), true));
					}
					else if (Input.MenuRight.Pressed && this.PageIndex < this.Pages.Count - 1)
					{
						if (this.PageIndex == 0)
						{
							Audio.Play("event:/ui/world_map/journal/page_cover_forward");
						}
						else
						{
							Audio.Play("event:/ui/world_map/journal/page_main_forward");
						}
						base.Add(new Coroutine(this.TurnPage(1), true));
					}
				}
				if (!this.PageTurningLocked && (Input.MenuJournal.Pressed || Input.MenuCancel.Pressed))
				{
					this.Close();
				}
			}
		}

		// Token: 0x060014B5 RID: 5301 RVA: 0x000729C5 File Offset: 0x00070BC5
		private void Close()
		{
			if (this.fromAreaInspect)
			{
				base.Overworld.Goto<OuiChapterPanel>();
				return;
			}
			base.Overworld.Goto<OuiChapterSelect>();
		}

		// Token: 0x060014B6 RID: 5302 RVA: 0x000729E8 File Offset: 0x00070BE8
		public override void Render()
		{
			Vector2 vector = this.Position + new Vector2(128f, 120f);
			float num = Ease.CubeInOut(Math.Max(0f, this.turningScale));
			float num2 = Ease.CubeInOut(Math.Abs(Math.Min(0f, this.turningScale)));
			if (SaveData.Instance.CheatMode)
			{
				MTN.FileSelect["cheatmode"].DrawCentered(vector + new Vector2(80f, 360f), Color.White, 1f, 1.5707964f);
			}
			if (SaveData.Instance.AssistMode)
			{
				MTN.FileSelect["assist"].DrawCentered(vector + new Vector2(100f, 370f), Color.White, 1f, 1.5707964f);
			}
			MTexture mtexture = MTN.Journal["edge"];
			mtexture.Draw(vector + new Vector2((float)(-(float)mtexture.Width), 0f), Vector2.Zero, Color.White, 1f, this.rotation);
			if (this.PageIndex > 0)
			{
				MTN.Journal[this.PrevPage.PageTexture].Draw(vector, Vector2.Zero, this.backColor, new Vector2(-1f, 1f), this.rotation);
			}
			if (this.turningPage)
			{
				MTN.Journal[this.NextPage.PageTexture].Draw(vector, Vector2.Zero, Color.White, 1f, this.rotation);
				Draw.SpriteBatch.Draw(this.NextPageBuffer, vector, new Rectangle?(this.NextPageBuffer.Bounds), Color.White, this.rotation, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
			}
			if (this.turningPage && num2 > 0f)
			{
				MTN.Journal[this.Page.PageTexture].Draw(vector, Vector2.Zero, this.backColor, new Vector2(-1f * num2, 1f), this.rotation);
			}
			if (num > 0f)
			{
				MTN.Journal[this.Page.PageTexture].Draw(vector, Vector2.Zero, Color.White, new Vector2(num, 1f), this.rotation);
				Draw.SpriteBatch.Draw(this.CurrentPageBuffer, vector, new Rectangle?(this.CurrentPageBuffer.Bounds), Color.White, this.rotation, Vector2.Zero, new Vector2(num, 1f), SpriteEffects.None, 0f);
			}
			if (this.Pages.Count > 0)
			{
				int count = this.Pages.Count;
				MTexture mtexture2 = GFX.Gui["dot_outline"];
				int num3 = mtexture2.Width * count;
				Vector2 value = new Vector2(960f, 1040f - 40f * Ease.CubeOut(this.dotEase));
				for (int i = 0; i < count; i++)
				{
					mtexture2.DrawCentered(value + new Vector2((float)(-(float)num3 / 2) + (float)mtexture2.Width * ((float)i + 0.5f), 0f), Color.White * 0.25f);
				}
				float x = 1f + Calc.YoYo(this.dot % 1f) * 4f;
				mtexture2.DrawCentered(value + new Vector2((float)(-(float)num3 / 2) + (float)mtexture2.Width * (this.dot + 0.5f), 0f), Color.White, new Vector2(x, 1f));
				GFX.Gui["dotarrow_outline"].DrawCentered(value + new Vector2((float)(-(float)num3 / 2 - 50), 32f * (1f - Ease.CubeOut(this.leftArrowEase))), Color.White * this.leftArrowEase, new Vector2(-1f, 1f));
				GFX.Gui["dotarrow_outline"].DrawCentered(value + new Vector2((float)(num3 / 2 + 50), 32f * (1f - Ease.CubeOut(this.rightArrowEase))), Color.White * this.rightArrowEase);
			}
		}

		// Token: 0x04001071 RID: 4209
		private const float onScreenX = 0f;

		// Token: 0x04001072 RID: 4210
		private const float offScreenX = -1920f;

		// Token: 0x04001073 RID: 4211
		public bool PageTurningLocked;

		// Token: 0x04001074 RID: 4212
		public List<OuiJournalPage> Pages = new List<OuiJournalPage>();

		// Token: 0x04001075 RID: 4213
		public int PageIndex;

		// Token: 0x04001076 RID: 4214
		public VirtualRenderTarget CurrentPageBuffer;

		// Token: 0x04001077 RID: 4215
		public VirtualRenderTarget NextPageBuffer;

		// Token: 0x04001078 RID: 4216
		private bool turningPage;

		// Token: 0x04001079 RID: 4217
		private float turningScale;

		// Token: 0x0400107A RID: 4218
		private Color backColor = Color.Lerp(Color.White, Color.Black, 0.2f);

		// Token: 0x0400107B RID: 4219
		private bool fromAreaInspect;

		// Token: 0x0400107C RID: 4220
		private float rotation;

		// Token: 0x0400107D RID: 4221
		private MountainCamera cameraStart;

		// Token: 0x0400107E RID: 4222
		private MountainCamera cameraEnd;

		// Token: 0x0400107F RID: 4223
		private MTexture arrow = MTN.Journal["pageArrow"];

		// Token: 0x04001080 RID: 4224
		private float dot;

		// Token: 0x04001081 RID: 4225
		private float dotTarget;

		// Token: 0x04001082 RID: 4226
		private float dotEase;

		// Token: 0x04001083 RID: 4227
		private float leftArrowEase;

		// Token: 0x04001084 RID: 4228
		private float rightArrowEase;
	}
}
