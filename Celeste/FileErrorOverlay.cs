using System;
using System.Collections;
using Monocle;

namespace Celeste
{
	// Token: 0x02000216 RID: 534
	public class FileErrorOverlay : Overlay
	{
		// Token: 0x1700013B RID: 315
		// (get) Token: 0x0600114A RID: 4426 RVA: 0x0005548C File Offset: 0x0005368C
		// (set) Token: 0x0600114B RID: 4427 RVA: 0x00055494 File Offset: 0x00053694
		public bool Open { get; private set; }

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x0600114C RID: 4428 RVA: 0x0005549D File Offset: 0x0005369D
		// (set) Token: 0x0600114D RID: 4429 RVA: 0x000554A5 File Offset: 0x000536A5
		public bool TryAgain { get; private set; }

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x0600114E RID: 4430 RVA: 0x000554AE File Offset: 0x000536AE
		// (set) Token: 0x0600114F RID: 4431 RVA: 0x000554B6 File Offset: 0x000536B6
		public bool Ignore { get; private set; }

		// Token: 0x06001150 RID: 4432 RVA: 0x000554BF File Offset: 0x000536BF
		public FileErrorOverlay(FileErrorOverlay.Error mode)
		{
			this.Open = true;
			this.mode = mode;
			base.Add(new Coroutine(this.Routine(), true));
			Engine.Scene.Add(this);
		}

		// Token: 0x06001151 RID: 4433 RVA: 0x000554F2 File Offset: 0x000536F2
		private IEnumerator Routine()
		{
			yield return base.FadeIn();
			bool waiting = true;
			int option = 0;
			Audio.Play("event:/ui/main/message_confirm");
			this.menu = new TextMenu();
			this.menu.Add(new TextMenu.Header(Dialog.Clean("savefailed_title", null)));
			this.menu.Add(new TextMenu.Button(Dialog.Clean((this.mode == FileErrorOverlay.Error.Save) ? "savefailed_retry" : "loadfailed_goback", null)).Pressed(delegate
			{
				option = 0;
				waiting = false;
			}));
			this.menu.Add(new TextMenu.Button(Dialog.Clean("savefailed_ignore", null)).Pressed(delegate
			{
				option = 1;
				waiting = false;
			}));
			while (waiting)
			{
				yield return null;
			}
			this.menu = null;
			this.Ignore = (option == 1);
			this.TryAgain = (option == 0);
			yield return base.FadeOut();
			this.Open = false;
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x06001152 RID: 4434 RVA: 0x00055501 File Offset: 0x00053701
		public override void Update()
		{
			base.Update();
			if (this.menu != null)
			{
				this.menu.Update();
			}
			if (SaveLoadIcon.Instance != null && SaveLoadIcon.Instance.Scene == base.Scene)
			{
				SaveLoadIcon.Instance.Update();
			}
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x0005553F File Offset: 0x0005373F
		public override void Render()
		{
			base.RenderFade();
			if (this.menu != null)
			{
				this.menu.Render();
			}
			base.Render();
		}

		// Token: 0x04000CF6 RID: 3318
		private FileErrorOverlay.Error mode;

		// Token: 0x04000CF7 RID: 3319
		private TextMenu menu;

		// Token: 0x02000524 RID: 1316
		public enum Error
		{
			// Token: 0x0400253E RID: 9534
			Load,
			// Token: 0x0400253F RID: 9535
			Save
		}
	}
}
