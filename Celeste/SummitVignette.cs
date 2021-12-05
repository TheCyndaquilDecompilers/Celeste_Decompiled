using System;
using System.IO;
using System.Xml;
using Monocle;

namespace Celeste
{
	// Token: 0x02000304 RID: 772
	public class SummitVignette : Scene
	{
		// Token: 0x06001819 RID: 6169 RVA: 0x0009621E File Offset: 0x0009441E
		public SummitVignette(Session session)
		{
			this.session = session;
			session.Audio.Apply(false);
			RunThread.Start(new Action(this.LoadCompleteThread), "SUMMIT_VIGNETTE", false);
		}

		// Token: 0x0600181A RID: 6170 RVA: 0x00096250 File Offset: 0x00094450
		private void LoadCompleteThread()
		{
			Atlas atlas = null;
			XmlElement xmlElement = GFX.CompleteScreensXml["Screens"]["SummitIntro"];
			if (xmlElement != null)
			{
				atlas = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", xmlElement.Attr("atlas")), Atlas.AtlasDataFormat.PackerNoAtlas);
			}
			this.complete = new CompleteRenderer(xmlElement, atlas, 0f, delegate()
			{
				this.slideFinished = true;
			});
			this.complete.SlideDuration = 7.5f;
			this.ready = true;
		}

		// Token: 0x0600181B RID: 6171 RVA: 0x000962D4 File Offset: 0x000944D4
		public override void Update()
		{
			if (this.ready && !this.addedRenderer)
			{
				base.Add(this.complete);
				this.addedRenderer = true;
			}
			base.Update();
			if ((Input.MenuConfirm.Pressed || this.slideFinished) && !this.ending && this.ready)
			{
				this.ending = true;
				new MountainWipe(this, false, delegate()
				{
					Engine.Scene = new LevelLoader(this.session, null);
				});
			}
		}

		// Token: 0x0600181C RID: 6172 RVA: 0x00096349 File Offset: 0x00094549
		public override void End()
		{
			base.End();
			if (this.complete != null)
			{
				this.complete.Dispose();
			}
		}

		// Token: 0x040014D9 RID: 5337
		private CompleteRenderer complete;

		// Token: 0x040014DA RID: 5338
		private bool slideFinished;

		// Token: 0x040014DB RID: 5339
		private Session session;

		// Token: 0x040014DC RID: 5340
		private bool ending;

		// Token: 0x040014DD RID: 5341
		private bool ready;

		// Token: 0x040014DE RID: 5342
		private bool addedRenderer;
	}
}
