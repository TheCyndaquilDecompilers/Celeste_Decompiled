using System;
using System.Collections.Generic;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200017D RID: 381
	[Tracked(false)]
	public class PlayerDashAssist : Entity
	{
		// Token: 0x06000D7E RID: 3454 RVA: 0x0002F004 File Offset: 0x0002D204
		public PlayerDashAssist()
		{
			base.Tag = Tags.Global;
			base.Depth = -1000000;
			this.Visible = false;
			this.images = GFX.Game.GetAtlasSubtextures("util/dasharrow/dasharrow");
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x0002F044 File Offset: 0x0002D244
		public override void Update()
		{
			if (!Engine.DashAssistFreeze)
			{
				if (this.paused)
				{
					if (!base.Scene.Paused)
					{
						Audio.PauseGameplaySfx = false;
					}
					this.DisableSnapshot();
					this.timer = 0f;
					this.paused = false;
				}
				return;
			}
			this.paused = true;
			Audio.PauseGameplaySfx = true;
			this.timer += Engine.RawDeltaTime;
			if (this.timer > 0.2f && this.snapshot == null)
			{
				this.EnableSnapshot();
			}
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity == null)
			{
				return;
			}
			float num = Input.GetAimVector(entity.Facing).Angle();
			if (Calc.AbsAngleDiff(num, this.Direction) >= 1.5807964f)
			{
				this.Direction = num;
				this.Scale = 0f;
			}
			else
			{
				this.Direction = Calc.AngleApproach(this.Direction, num, 18.849556f * Engine.RawDeltaTime);
			}
			this.Scale = Calc.Approach(this.Scale, 1f, Engine.DeltaTime * 4f);
			int num2 = 1 + (8 + (int)Math.Round((double)(num / 0.7853982f))) % 8;
			if (this.lastIndex != 0 && this.lastIndex != num2)
			{
				Audio.Play("event:/game/general/assist_dash_aim", entity.Center, "dash_direction", (float)num2);
			}
			this.lastIndex = num2;
		}

		// Token: 0x06000D80 RID: 3456 RVA: 0x0002F19C File Offset: 0x0002D39C
		public override void Render()
		{
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity == null)
			{
				return;
			}
			if (!Engine.DashAssistFreeze)
			{
				return;
			}
			MTexture mtexture = null;
			float num = float.MaxValue;
			for (int i = 0; i < 8; i++)
			{
				float num2 = Calc.AngleDiff(6.2831855f * ((float)i / 8f), this.Direction);
				if (Math.Abs(num2) < Math.Abs(num))
				{
					num = num2;
					mtexture = this.images[i];
				}
			}
			if (mtexture != null)
			{
				if (Math.Abs(num) < 0.05f)
				{
					num = 0f;
				}
				mtexture.DrawOutlineCentered((entity.Center + this.Offset + Calc.AngleToVector(this.Direction, 20f)).Round(), Color.White, Ease.BounceOut(this.Scale), num);
			}
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x000091E2 File Offset: 0x000073E2
		private void EnableSnapshot()
		{
		}

		// Token: 0x06000D82 RID: 3458 RVA: 0x0002F271 File Offset: 0x0002D471
		private void DisableSnapshot()
		{
			if (this.snapshot != null)
			{
				Audio.ReleaseSnapshot(this.snapshot);
				this.snapshot = null;
			}
		}

		// Token: 0x06000D83 RID: 3459 RVA: 0x0002F293 File Offset: 0x0002D493
		public override void Removed(Scene scene)
		{
			this.DisableSnapshot();
			base.Removed(scene);
		}

		// Token: 0x06000D84 RID: 3460 RVA: 0x0002F2A2 File Offset: 0x0002D4A2
		public override void SceneEnd(Scene scene)
		{
			this.DisableSnapshot();
			base.SceneEnd(scene);
		}

		// Token: 0x040008BC RID: 2236
		public float Direction;

		// Token: 0x040008BD RID: 2237
		public float Scale;

		// Token: 0x040008BE RID: 2238
		public Vector2 Offset;

		// Token: 0x040008BF RID: 2239
		private List<MTexture> images;

		// Token: 0x040008C0 RID: 2240
		private EventInstance snapshot;

		// Token: 0x040008C1 RID: 2241
		private float timer;

		// Token: 0x040008C2 RID: 2242
		private bool paused;

		// Token: 0x040008C3 RID: 2243
		private int lastIndex;
	}
}
