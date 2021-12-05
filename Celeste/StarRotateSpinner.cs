using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000148 RID: 328
	public class StarRotateSpinner : RotateSpinner
	{
		// Token: 0x06000C01 RID: 3073 RVA: 0x000252CC File Offset: 0x000234CC
		public StarRotateSpinner(EntityData data, Vector2 offset) : base(data, offset)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("moonBlade"));
			this.colorID = Calc.Random.Choose(0, 1, 2);
			this.Sprite.Play("idle" + this.colorID, false, false);
			base.Depth = -50;
			base.Add(new MirrorReflection());
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x00025348 File Offset: 0x00023548
		public override void Update()
		{
			base.Update();
			if (this.Moving && base.Scene.OnInterval(0.03f))
			{
				base.SceneAs<Level>().ParticlesBG.Emit(StarTrackSpinner.P_Trail[this.colorID], 1, this.Position, Vector2.One * 3f);
			}
			if (base.Scene.OnInterval(0.8f))
			{
				this.colorID++;
				this.colorID %= 3;
				this.Sprite.Play("spin" + this.colorID, false, false);
			}
		}

		// Token: 0x0400074F RID: 1871
		public Sprite Sprite;

		// Token: 0x04000750 RID: 1872
		private int colorID;
	}
}
