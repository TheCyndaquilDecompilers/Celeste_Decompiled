using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000201 RID: 513
	public class SoundEmitter : Entity
	{
		// Token: 0x060010C5 RID: 4293 RVA: 0x0004EA98 File Offset: 0x0004CC98
		public static SoundEmitter Play(string sfx)
		{
			SoundEmitter soundEmitter = new SoundEmitter(sfx);
			Engine.Scene.Add(soundEmitter);
			return soundEmitter;
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x0004EAB8 File Offset: 0x0004CCB8
		public static SoundEmitter Play(string sfx, Entity follow, Vector2? offset = null)
		{
			SoundEmitter soundEmitter = new SoundEmitter(sfx, follow, (offset != null) ? offset.Value : Vector2.Zero);
			Engine.Scene.Add(soundEmitter);
			return soundEmitter;
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x060010C7 RID: 4295 RVA: 0x0004EAF0 File Offset: 0x0004CCF0
		// (set) Token: 0x060010C8 RID: 4296 RVA: 0x0004EAF8 File Offset: 0x0004CCF8
		public SoundSource Source { get; private set; }

		// Token: 0x060010C9 RID: 4297 RVA: 0x0004EB04 File Offset: 0x0004CD04
		private SoundEmitter(string sfx)
		{
			base.Add(this.Source = new SoundSource());
			this.Source.Play(sfx, null, 0f);
			this.Source.DisposeOnTransition = false;
			base.Tag = (Tags.Persistent | Tags.TransitionUpdate);
			base.Add(new LevelEndingHook(new Action(this.OnLevelEnding)));
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x0004EB7C File Offset: 0x0004CD7C
		private SoundEmitter(string sfx, Entity follow, Vector2 offset)
		{
			base.Add(this.Source = new SoundSource());
			this.Position = follow.Position + offset;
			this.Source.Play(sfx, null, 0f);
			this.Source.DisposeOnTransition = false;
			base.Tag = (Tags.Persistent | Tags.TransitionUpdate);
			base.Add(new LevelEndingHook(new Action(this.OnLevelEnding)));
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x0004EC06 File Offset: 0x0004CE06
		public override void Update()
		{
			base.Update();
			if (!this.Source.Playing)
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x0004EC21 File Offset: 0x0004CE21
		private void OnLevelEnding()
		{
			this.Source.Stop(true);
		}
	}
}
