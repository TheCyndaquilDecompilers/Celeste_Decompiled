using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000149 RID: 329
	public class StarTrackSpinner : TrackSpinner
	{
		// Token: 0x06000C03 RID: 3075 RVA: 0x000253F8 File Offset: 0x000235F8
		public StarTrackSpinner(EntityData data, Vector2 offset) : base(data, offset)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("moonBlade"));
			this.colorID = Calc.Random.Choose(0, 1, 2);
			this.Sprite.Play("idle" + this.colorID, false, false);
			base.Depth = -50;
			base.Add(new MirrorReflection());
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x00025474 File Offset: 0x00023674
		public override void Update()
		{
			base.Update();
			if (this.trail && base.Scene.OnInterval(0.03f))
			{
				base.SceneAs<Level>().ParticlesBG.Emit(StarTrackSpinner.P_Trail[this.colorID], 1, this.Position, Vector2.One * 3f);
			}
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x000254D4 File Offset: 0x000236D4
		public override void OnTrackStart()
		{
			this.colorID++;
			this.colorID %= 3;
			this.Sprite.Play("spin" + this.colorID, false, false);
			if (this.hasStarted)
			{
				Audio.Play("event:/game/05_mirror_temple/bladespinner_spin", this.Position);
			}
			this.hasStarted = true;
			this.trail = true;
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x00025546 File Offset: 0x00023746
		public override void OnTrackEnd()
		{
			this.trail = false;
		}

		// Token: 0x04000751 RID: 1873
		public static ParticleType[] P_Trail;

		// Token: 0x04000752 RID: 1874
		public Sprite Sprite;

		// Token: 0x04000753 RID: 1875
		private bool hasStarted;

		// Token: 0x04000754 RID: 1876
		private int colorID;

		// Token: 0x04000755 RID: 1877
		private bool trail;
	}
}
