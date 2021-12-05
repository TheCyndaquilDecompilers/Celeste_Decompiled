using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001CF RID: 463
	public class BirdPathTrigger : Trigger
	{
		// Token: 0x06000FB8 RID: 4024 RVA: 0x00042A43 File Offset: 0x00040C43
		public BirdPathTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
		}

		// Token: 0x06000FB9 RID: 4025 RVA: 0x00042A50 File Offset: 0x00040C50
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			BirdPath birdPath = base.Scene.Entities.FindFirst<BirdPath>();
			if (birdPath != null)
			{
				this.bird = birdPath;
				this.bird.WaitForTrigger();
				return;
			}
			base.RemoveSelf();
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x00042A91 File Offset: 0x00040C91
		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			if (!this.triggered)
			{
				this.bird.Trigger();
				this.triggered = true;
			}
		}

		// Token: 0x04000B20 RID: 2848
		private BirdPath bird;

		// Token: 0x04000B21 RID: 2849
		private bool triggered;
	}
}
