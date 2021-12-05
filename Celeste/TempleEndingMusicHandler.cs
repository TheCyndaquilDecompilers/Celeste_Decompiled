using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Monocle;

namespace Celeste
{
	// Token: 0x02000207 RID: 519
	public class TempleEndingMusicHandler : Entity
	{
		// Token: 0x060010FA RID: 4346 RVA: 0x000512A8 File Offset: 0x0004F4A8
		public TempleEndingMusicHandler()
		{
			base.Tag = (Tags.TransitionUpdate | Tags.Global);
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x000512D8 File Offset: 0x0004F4D8
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Regex regex = new Regex(Regex.Escape("e-*").Replace("\\*", ".*") + "$");
			foreach (LevelData levelData in (scene as Level).Session.MapData.Levels)
			{
				if (levelData.Name.Equals("e-01"))
				{
					this.startX = (float)levelData.Bounds.Left;
				}
				else if (levelData.Name.Equals("e-09"))
				{
					this.endX = (float)levelData.Bounds.Right;
				}
				if (regex.IsMatch(levelData.Name))
				{
					this.levels.Add(levelData.Name);
				}
			}
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x000513D0 File Offset: 0x0004F5D0
		public override void Update()
		{
			base.Update();
			Level level = base.Scene as Level;
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null && this.levels.Contains(level.Session.Level) && Audio.CurrentMusic == "event:/music/lvl5/mirror")
			{
				float num = Calc.Clamp((entity.X - this.startX) / (this.endX - this.startX), 0f, 1f);
				level.Session.Audio.Music.Layer(1, 1f - num);
				level.Session.Audio.Music.Layer(5, num);
				level.Session.Audio.Apply(false);
			}
		}

		// Token: 0x04000C98 RID: 3224
		public const string StartLevel = "e-01";

		// Token: 0x04000C99 RID: 3225
		public const string EndLevel = "e-09";

		// Token: 0x04000C9A RID: 3226
		public const string ApplyIn = "e-*";

		// Token: 0x04000C9B RID: 3227
		private HashSet<string> levels = new HashSet<string>();

		// Token: 0x04000C9C RID: 3228
		private float startX;

		// Token: 0x04000C9D RID: 3229
		private float endX;
	}
}
