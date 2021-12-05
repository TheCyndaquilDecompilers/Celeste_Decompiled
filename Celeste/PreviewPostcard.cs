using System;
using System.Collections;
using Monocle;

namespace Celeste
{
	// Token: 0x02000242 RID: 578
	public class PreviewPostcard : Scene
	{
		// Token: 0x06001253 RID: 4691 RVA: 0x0005FF78 File Offset: 0x0005E178
		public PreviewPostcard(Postcard postcard)
		{
			Audio.SetMusic(null, true, true);
			Audio.SetAmbience(null, true);
			this.postcard = postcard;
			base.Add(new Entity
			{
				new Coroutine(this.Routine(postcard), true)
			});
			base.Add(new HudRenderer());
		}

		// Token: 0x06001254 RID: 4692 RVA: 0x0005FFCE File Offset: 0x0005E1CE
		private IEnumerator Routine(Postcard postcard)
		{
			yield return 0.25f;
			base.Add(postcard);
			yield return postcard.DisplayRoutine();
			Engine.Scene = new OverworldLoader(Overworld.StartMode.MainMenu, null);
			yield break;
		}

		// Token: 0x06001255 RID: 4693 RVA: 0x0005FFE4 File Offset: 0x0005E1E4
		public override void BeforeRender()
		{
			base.BeforeRender();
			if (this.postcard != null)
			{
				this.postcard.BeforeRender();
			}
		}

		// Token: 0x04000DFF RID: 3583
		private Postcard postcard;
	}
}
