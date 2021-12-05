using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200027A RID: 634
	public abstract class CutsceneEntity : Entity
	{
		// Token: 0x1700015A RID: 346
		// (get) Token: 0x0600139B RID: 5019 RVA: 0x0006AE2F File Offset: 0x0006902F
		// (set) Token: 0x0600139C RID: 5020 RVA: 0x0006AE37 File Offset: 0x00069037
		public bool Running { get; private set; }

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x0600139D RID: 5021 RVA: 0x0006AE40 File Offset: 0x00069040
		// (set) Token: 0x0600139E RID: 5022 RVA: 0x0006AE48 File Offset: 0x00069048
		public bool FadeInOnSkip { get; private set; }

		// Token: 0x0600139F RID: 5023 RVA: 0x0006AE51 File Offset: 0x00069051
		public CutsceneEntity(bool fadeInOnSkip = true, bool endingChapterAfter = false)
		{
			this.FadeInOnSkip = fadeInOnSkip;
			this.EndingChapterAfter = endingChapterAfter;
		}

		// Token: 0x060013A0 RID: 5024 RVA: 0x0006AE6E File Offset: 0x0006906E
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.Level = (scene as Level);
			this.Start();
		}

		// Token: 0x060013A1 RID: 5025 RVA: 0x0006AE89 File Offset: 0x00069089
		public void Start()
		{
			this.Running = true;
			this.Level.StartCutscene(new Action<Level>(this.SkipCutscene), this.FadeInOnSkip, this.EndingChapterAfter, true);
			this.OnBegin(this.Level);
		}

		// Token: 0x060013A2 RID: 5026 RVA: 0x0006AEC2 File Offset: 0x000690C2
		public override void Update()
		{
			if (this.Level.RetryPlayerCorpse != null)
			{
				this.Active = false;
				return;
			}
			base.Update();
		}

		// Token: 0x060013A3 RID: 5027 RVA: 0x0006AEDF File Offset: 0x000690DF
		private void SkipCutscene(Level level)
		{
			this.WasSkipped = true;
			this.EndCutscene(level, this.RemoveOnSkipped);
		}

		// Token: 0x060013A4 RID: 5028 RVA: 0x0006AEF5 File Offset: 0x000690F5
		public void EndCutscene(Level level, bool removeSelf = true)
		{
			this.Running = false;
			this.OnEnd(level);
			level.EndCutscene();
			if (removeSelf)
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x060013A5 RID: 5029
		public abstract void OnBegin(Level level);

		// Token: 0x060013A6 RID: 5030
		public abstract void OnEnd(Level level);

		// Token: 0x060013A7 RID: 5031 RVA: 0x0006AF14 File Offset: 0x00069114
		public static IEnumerator CameraTo(Vector2 target, float duration, Ease.Easer ease = null, float delay = 0f)
		{
			if (ease == null)
			{
				ease = Ease.CubeInOut;
			}
			if (delay > 0f)
			{
				yield return delay;
			}
			Level level = Engine.Scene as Level;
			Vector2 from = level.Camera.Position;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
			{
				level.Camera.Position = from + (target - from) * ease(p);
				yield return null;
			}
			level.Camera.Position = target;
			yield break;
		}

		// Token: 0x04000F75 RID: 3957
		public bool WasSkipped;

		// Token: 0x04000F76 RID: 3958
		public bool RemoveOnSkipped = true;

		// Token: 0x04000F77 RID: 3959
		public bool EndingChapterAfter;

		// Token: 0x04000F78 RID: 3960
		public Level Level;
	}
}
