using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200020B RID: 523
	public class AnimatedTilesBank
	{
		// Token: 0x0600110F RID: 4367 RVA: 0x00051C78 File Offset: 0x0004FE78
		public void Add(string name, float delay, Vector2 offset, Vector2 origin, List<MTexture> textures)
		{
			AnimatedTilesBank.Animation animation = new AnimatedTilesBank.Animation
			{
				Name = name,
				Delay = delay,
				Offset = offset,
				Origin = origin,
				Frames = textures.ToArray()
			};
			animation.ID = this.Animations.Count;
			this.Animations.Add(animation);
			this.AnimationsByName.Add(name, animation);
		}

		// Token: 0x04000CB5 RID: 3253
		public Dictionary<string, AnimatedTilesBank.Animation> AnimationsByName = new Dictionary<string, AnimatedTilesBank.Animation>();

		// Token: 0x04000CB6 RID: 3254
		public List<AnimatedTilesBank.Animation> Animations = new List<AnimatedTilesBank.Animation>();

		// Token: 0x02000516 RID: 1302
		public struct Animation
		{
			// Token: 0x040024EE RID: 9454
			public int ID;

			// Token: 0x040024EF RID: 9455
			public string Name;

			// Token: 0x040024F0 RID: 9456
			public float Delay;

			// Token: 0x040024F1 RID: 9457
			public Vector2 Offset;

			// Token: 0x040024F2 RID: 9458
			public Vector2 Origin;

			// Token: 0x040024F3 RID: 9459
			public MTexture[] Frames;
		}
	}
}
