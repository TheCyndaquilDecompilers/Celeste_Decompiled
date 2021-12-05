using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001A1 RID: 417
	public class BladeTrackSpinner : TrackSpinner
	{
		// Token: 0x06000E9F RID: 3743 RVA: 0x00036E00 File Offset: 0x00035000
		public BladeTrackSpinner(EntityData data, Vector2 offset) : base(data, offset)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("templeBlade"));
			this.Sprite.Play("idle", false, false);
			base.Depth = -50;
			base.Add(new MirrorReflection());
		}

		// Token: 0x06000EA0 RID: 3744 RVA: 0x00036E58 File Offset: 0x00035058
		public override void Update()
		{
			base.Update();
			if (this.trail && base.Scene.OnInterval(0.04f))
			{
				base.SceneAs<Level>().ParticlesBG.Emit(BladeTrackSpinner.P_Trail, 2, this.Position, Vector2.One * 3f);
			}
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x00036EB0 File Offset: 0x000350B0
		public override void OnTrackStart()
		{
			this.Sprite.Play("spin", false, false);
			if (this.hasStarted)
			{
				Audio.Play("event:/game/05_mirror_temple/bladespinner_spin", this.Position);
			}
			this.hasStarted = true;
			this.trail = true;
		}

		// Token: 0x06000EA2 RID: 3746 RVA: 0x00036EEB File Offset: 0x000350EB
		public override void OnTrackEnd()
		{
			this.trail = false;
		}

		// Token: 0x040009D9 RID: 2521
		public static ParticleType P_Trail;

		// Token: 0x040009DA RID: 2522
		public Sprite Sprite;

		// Token: 0x040009DB RID: 2523
		private bool hasStarted;

		// Token: 0x040009DC RID: 2524
		private bool trail;
	}
}
