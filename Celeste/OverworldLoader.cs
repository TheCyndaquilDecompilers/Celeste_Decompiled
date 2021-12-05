using System;
using System.Collections;
using System.Threading;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000255 RID: 597
	public class OverworldLoader : Scene
	{
		// Token: 0x060012AA RID: 4778 RVA: 0x00064AFD File Offset: 0x00062CFD
		public OverworldLoader(Overworld.StartMode startMode, HiresSnow snow = null)
		{
			this.StartMode = startMode;
			this.Snow = ((snow == null) ? new HiresSnow(0.45f) : snow);
			this.fadeIn = (snow == null);
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x00064B2C File Offset: 0x00062D2C
		public override void Begin()
		{
			base.Add(new HudRenderer());
			base.Add(this.Snow);
			if (this.fadeIn)
			{
				ScreenWipe.WipeColor = Color.Black;
				new FadeWipe(this, true, null);
			}
			base.RendererList.UpdateLists();
			Session session = null;
			if (SaveData.Instance != null)
			{
				session = SaveData.Instance.CurrentSession;
			}
			base.Add(new Entity
			{
				new Coroutine(this.Routine(session), true)
			});
			this.activeThread = Thread.CurrentThread;
			this.activeThread.Priority = ThreadPriority.Lowest;
			RunThread.Start(new Action(this.LoadThread), "OVERWORLD_LOADER", true);
		}

		// Token: 0x060012AC RID: 4780 RVA: 0x00064BD8 File Offset: 0x00062DD8
		private void LoadThread()
		{
			if (!MTN.Loaded)
			{
				MTN.Load();
			}
			if (!MTN.DataLoaded)
			{
				MTN.LoadData();
			}
			this.CheckVariantsPostcardAtLaunch();
			this.overworld = new Overworld(this);
			this.overworld.Entities.UpdateLists();
			this.loaded = true;
			this.activeThread.Priority = ThreadPriority.Normal;
		}

		// Token: 0x060012AD RID: 4781 RVA: 0x00064C32 File Offset: 0x00062E32
		private IEnumerator Routine(Session session)
		{
			if ((this.StartMode == Overworld.StartMode.AreaComplete || this.StartMode == Overworld.StartMode.AreaQuit) && session != null)
			{
				if (session.UnlockedCSide)
				{
					this.showUnlockCSidePostcard = true;
				}
				if (!Settings.Instance.VariantsUnlocked && SaveData.Instance != null && SaveData.Instance.TotalHeartGems >= 24)
				{
					this.showVariantPostcard = true;
				}
			}
			if (this.showUnlockCSidePostcard)
			{
				yield return 3f;
				base.Add(this.postcard = new Postcard(Dialog.Get("POSTCARD_CSIDES", null), "event:/ui/main/postcard_csides_in", "event:/ui/main/postcard_csides_out"));
				yield return this.postcard.DisplayRoutine();
			}
			while (!this.loaded)
			{
				yield return null;
			}
			if (this.showVariantPostcard)
			{
				yield return 3f;
				Settings.Instance.VariantsUnlocked = true;
				base.Add(this.postcard = new Postcard(Dialog.Get("POSTCARD_VARIANTS", null), "event:/new_content/ui/postcard_variants_in", "event:/new_content/ui/postcard_variants_out"));
				yield return this.postcard.DisplayRoutine();
				UserIO.SaveHandler(false, true);
				while (UserIO.Saving)
				{
					yield return null;
				}
				while (SaveLoadIcon.Instance != null)
				{
					yield return null;
				}
			}
			Engine.Scene = this.overworld;
			yield break;
		}

		// Token: 0x060012AE RID: 4782 RVA: 0x00064C48 File Offset: 0x00062E48
		public override void BeforeRender()
		{
			base.BeforeRender();
			if (this.postcard != null)
			{
				this.postcard.BeforeRender();
			}
		}

		// Token: 0x060012AF RID: 4783 RVA: 0x00064C64 File Offset: 0x00062E64
		private void CheckVariantsPostcardAtLaunch()
		{
			if (this.StartMode == Overworld.StartMode.Titlescreen && !Settings.Instance.VariantsUnlocked && (Settings.LastVersion == null || new Version(Settings.LastVersion) <= new Version(1, 2, 4, 2)) && UserIO.Open(UserIO.Mode.Read))
			{
				for (int i = 0; i < 3; i++)
				{
					if (UserIO.Exists(SaveData.GetFilename(i)))
					{
						SaveData saveData = UserIO.Load<SaveData>(SaveData.GetFilename(i), false);
						if (saveData != null)
						{
							saveData.AfterInitialize();
							if (saveData.TotalHeartGems >= 24)
							{
								this.showVariantPostcard = true;
								break;
							}
						}
					}
				}
				UserIO.Close();
				SaveData.Instance = null;
			}
		}

		// Token: 0x04000EA3 RID: 3747
		public Overworld.StartMode StartMode;

		// Token: 0x04000EA4 RID: 3748
		public HiresSnow Snow;

		// Token: 0x04000EA5 RID: 3749
		private bool loaded;

		// Token: 0x04000EA6 RID: 3750
		private bool fadeIn;

		// Token: 0x04000EA7 RID: 3751
		private Overworld overworld;

		// Token: 0x04000EA8 RID: 3752
		private Postcard postcard;

		// Token: 0x04000EA9 RID: 3753
		private bool showVariantPostcard;

		// Token: 0x04000EAA RID: 3754
		private bool showUnlockCSidePostcard;

		// Token: 0x04000EAB RID: 3755
		private Thread activeThread;
	}
}
