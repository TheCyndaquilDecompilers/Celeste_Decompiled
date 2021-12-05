using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000197 RID: 407
	public class BladeRotateSpinner : RotateSpinner
	{
		// Token: 0x06000E28 RID: 3624 RVA: 0x00032D44 File Offset: 0x00030F44
		public BladeRotateSpinner(EntityData data, Vector2 offset) : base(data, offset)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("templeBlade"));
			this.Sprite.Play("idle", false, false);
			base.Depth = -50;
			base.Add(new MirrorReflection());
		}

		// Token: 0x06000E29 RID: 3625 RVA: 0x00032D9C File Offset: 0x00030F9C
		public override void Update()
		{
			base.Update();
			if (base.Scene.OnInterval(0.04f))
			{
				base.SceneAs<Level>().ParticlesBG.Emit(BladeTrackSpinner.P_Trail, 2, this.Position, Vector2.One * 3f);
			}
			if (base.Scene.OnInterval(1f))
			{
				this.Sprite.Play("spin", false, false);
			}
		}

		// Token: 0x04000963 RID: 2403
		public Sprite Sprite;
	}
}
