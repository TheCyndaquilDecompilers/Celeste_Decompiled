using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020001DD RID: 477
	public class WaveDashPlaybackTutorial
	{
		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000FF9 RID: 4089 RVA: 0x000440A7 File Offset: 0x000422A7
		// (set) Token: 0x06000FFA RID: 4090 RVA: 0x000440AF File Offset: 0x000422AF
		public PlayerPlayback Playback { get; private set; }

		// Token: 0x06000FFB RID: 4091 RVA: 0x000440B8 File Offset: 0x000422B8
		public WaveDashPlaybackTutorial(string name, Vector2 offset, Vector2 dashDirection0, Vector2 dashDirection1)
		{
			List<Player.ChaserState> timeline = PlaybackData.Tutorials[name];
			this.Playback = new PlayerPlayback(offset, PlayerSpriteMode.MadelineNoBackpack, timeline);
			this.tag = Calc.Random.Next();
			this.dashDirection0 = dashDirection0;
			this.dashDirection1 = dashDirection1;
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x0004410C File Offset: 0x0004230C
		public void Update()
		{
			this.Playback.Update();
			this.Playback.Hair.AfterUpdate();
			if (this.Playback.Sprite.CurrentAnimationID == "dash" && this.Playback.Sprite.CurrentAnimationFrame == 0)
			{
				if (!this.dashing)
				{
					this.dashing = true;
					Celeste.Freeze(0.05f);
					SlashFx.Burst(this.Playback.Center, (this.firstDash ? this.dashDirection0 : this.dashDirection1).Angle()).Tag = this.tag;
					this.dashTrailTimer = 0.1f;
					this.dashTrailCounter = 2;
					this.CreateTrail();
					if (this.firstDash)
					{
						this.launchedDelay = 0.15f;
					}
					this.firstDash = !this.firstDash;
				}
			}
			else
			{
				this.dashing = false;
			}
			if (this.dashTrailTimer > 0f)
			{
				this.dashTrailTimer -= Engine.DeltaTime;
				if (this.dashTrailTimer <= 0f)
				{
					this.CreateTrail();
					this.dashTrailCounter--;
					if (this.dashTrailCounter > 0)
					{
						this.dashTrailTimer = 0.1f;
					}
				}
			}
			if (this.launchedDelay > 0f)
			{
				this.launchedDelay -= Engine.DeltaTime;
				if (this.launchedDelay <= 0f)
				{
					this.launched = true;
					this.launchedTimer = 0f;
				}
			}
			if (this.launched)
			{
				float prevVal = this.launchedTimer;
				this.launchedTimer += Engine.DeltaTime;
				if (this.launchedTimer >= 0.5f)
				{
					this.launched = false;
					this.launchedTimer = 0f;
				}
				else if (Calc.OnInterval(this.launchedTimer, prevVal, 0.15f))
				{
					SpeedRing speedRing = Engine.Pooler.Create<SpeedRing>().Init(this.Playback.Center, (this.Playback.Position - this.Playback.LastPosition).Angle(), Color.White);
					speedRing.Tag = this.tag;
					Engine.Scene.Add(speedRing);
				}
			}
			this.hasUpdated = true;
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x00044348 File Offset: 0x00042548
		public void Render(Vector2 position, float scale)
		{
			Matrix transformMatrix = Matrix.CreateScale(4f) * Matrix.CreateTranslation(position.X, position.Y, 0f);
			Draw.SpriteBatch.End();
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix);
			foreach (Entity entity in Engine.Scene.Tracker.GetEntities<TrailManager.Snapshot>())
			{
				if (entity.Tag == this.tag)
				{
					entity.Render();
				}
			}
			foreach (Entity entity2 in Engine.Scene.Tracker.GetEntities<SlashFx>())
			{
				if (entity2.Tag == this.tag && entity2.Visible)
				{
					entity2.Render();
				}
			}
			foreach (Entity entity3 in Engine.Scene.Tracker.GetEntities<SpeedRing>())
			{
				if (entity3.Tag == this.tag)
				{
					entity3.Render();
				}
			}
			if (this.Playback.Visible && this.hasUpdated)
			{
				this.Playback.Render();
			}
			if (this.OnRender != null)
			{
				this.OnRender();
			}
			Draw.SpriteBatch.End();
			Draw.SpriteBatch.Begin();
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x000444FC File Offset: 0x000426FC
		private void CreateTrail()
		{
			TrailManager.Add(this.Playback.Position, this.Playback.Sprite, this.Playback.Hair, this.Playback.Sprite.Scale, Player.UsedHairColor, 0, 1f, false, false).Tag = this.tag;
		}

		// Token: 0x04000B4D RID: 2893
		public Action OnRender;

		// Token: 0x04000B4F RID: 2895
		private bool hasUpdated;

		// Token: 0x04000B50 RID: 2896
		private float dashTrailTimer;

		// Token: 0x04000B51 RID: 2897
		private int dashTrailCounter;

		// Token: 0x04000B52 RID: 2898
		private bool dashing;

		// Token: 0x04000B53 RID: 2899
		private bool firstDash = true;

		// Token: 0x04000B54 RID: 2900
		private bool launched;

		// Token: 0x04000B55 RID: 2901
		private float launchedDelay;

		// Token: 0x04000B56 RID: 2902
		private float launchedTimer;

		// Token: 0x04000B57 RID: 2903
		private int tag;

		// Token: 0x04000B58 RID: 2904
		private Vector2 dashDirection0;

		// Token: 0x04000B59 RID: 2905
		private Vector2 dashDirection1;
	}
}
