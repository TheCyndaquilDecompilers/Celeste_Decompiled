using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001AF RID: 431
	public class PlayerPlayback : Entity
	{
		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000F0E RID: 3854 RVA: 0x0003B56C File Offset: 0x0003976C
		// (set) Token: 0x06000F0F RID: 3855 RVA: 0x0003B574 File Offset: 0x00039774
		public Vector2 DashDirection { get; private set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000F10 RID: 3856 RVA: 0x0003B57D File Offset: 0x0003977D
		public float Time
		{
			get
			{
				return this.time;
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06000F11 RID: 3857 RVA: 0x0003B585 File Offset: 0x00039785
		public int FrameIndex
		{
			get
			{
				return this.index;
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000F12 RID: 3858 RVA: 0x0003B58D File Offset: 0x0003978D
		public int FrameCount
		{
			get
			{
				return this.Timeline.Count;
			}
		}

		// Token: 0x06000F13 RID: 3859 RVA: 0x0003B59C File Offset: 0x0003979C
		public PlayerPlayback(EntityData e, Vector2 offset) : this(e.Position + offset, PlayerSpriteMode.Playback, PlaybackData.Tutorials[e.Attr("tutorial", "")])
		{
			if (e.Nodes != null && e.Nodes.Length != 0)
			{
				this.rangeMinX = base.X;
				this.rangeMaxX = base.X;
				foreach (Vector2 vector in e.NodesOffset(offset))
				{
					this.rangeMinX = Math.Min(this.rangeMinX, vector.X);
					this.rangeMaxX = Math.Max(this.rangeMaxX, vector.X);
				}
			}
			this.startDelay = 1f;
		}

		// Token: 0x06000F14 RID: 3860 RVA: 0x0003B658 File Offset: 0x00039858
		public PlayerPlayback(Vector2 start, PlayerSpriteMode sprite, List<Player.ChaserState> timeline)
		{
			this.start = start;
			base.Collider = new Hitbox(8f, 11f, -4f, -11f);
			this.Timeline = timeline;
			this.Position = start;
			this.time = 0f;
			this.index = 0;
			this.Duration = timeline[timeline.Count - 1].TimeStamp;
			this.TrimStart = 0f;
			this.TrimEnd = this.Duration;
			this.Sprite = new PlayerSprite(sprite);
			base.Add(this.Hair = new PlayerHair(this.Sprite));
			base.Add(this.Sprite);
			base.Collider = new Hitbox(8f, 4f, -4f, -4f);
			if (sprite == PlayerSpriteMode.Playback)
			{
				this.ShowTrail = true;
			}
			base.Depth = 9008;
			this.SetFrame(0);
			for (int i = 0; i < 10; i++)
			{
				this.Hair.AfterUpdate();
			}
			this.Visible = false;
			this.index = this.Timeline.Count;
		}

		// Token: 0x06000F15 RID: 3861 RVA: 0x0003B798 File Offset: 0x00039998
		private void Restart()
		{
			Audio.Play("event:/new_content/char/tutorial_ghost/appear", this.Position);
			this.Visible = true;
			this.time = this.TrimStart;
			this.index = 0;
			this.loopDelay = 0.25f;
			while (this.time > this.Timeline[this.index].TimeStamp)
			{
				this.index++;
			}
			this.SetFrame(this.index);
		}

		// Token: 0x06000F16 RID: 3862 RVA: 0x0003B818 File Offset: 0x00039A18
		public void SetFrame(int index)
		{
			Player.ChaserState chaserState = this.Timeline[index];
			string currentAnimationID = this.Sprite.CurrentAnimationID;
			bool flag = base.Scene != null && base.CollideCheck<Solid>(this.Position + new Vector2(0f, 1f));
			Vector2 dashDirection = this.DashDirection;
			this.Position = this.start + chaserState.Position;
			if (chaserState.Animation != this.Sprite.CurrentAnimationID && chaserState.Animation != null && this.Sprite.Has(chaserState.Animation))
			{
				this.Sprite.Play(chaserState.Animation, true, false);
			}
			this.Sprite.Scale = chaserState.Scale;
			if (this.Sprite.Scale.X != 0f)
			{
				this.Hair.Facing = (Facings)Math.Sign(this.Sprite.Scale.X);
			}
			this.Hair.Color = chaserState.HairColor;
			if (this.Sprite.Mode == PlayerSpriteMode.Playback)
			{
				this.Sprite.Color = this.Hair.Color;
			}
			this.DashDirection = chaserState.DashDirection;
			if (base.Scene != null)
			{
				if (!flag && base.Scene != null && base.CollideCheck<Solid>(this.Position + new Vector2(0f, 1f)))
				{
					Audio.Play("event:/new_content/char/tutorial_ghost/land", this.Position);
				}
				if (currentAnimationID != this.Sprite.CurrentAnimationID)
				{
					string currentAnimationID2 = this.Sprite.CurrentAnimationID;
					int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
					if (currentAnimationID2 == "jumpFast" || currentAnimationID2 == "jumpSlow")
					{
						Audio.Play("event:/new_content/char/tutorial_ghost/jump", this.Position);
						return;
					}
					if (currentAnimationID2 == "dreamDashIn")
					{
						Audio.Play("event:/new_content/char/tutorial_ghost/dreamblock_sequence", this.Position);
						return;
					}
					if (currentAnimationID2 == "dash")
					{
						if (this.DashDirection.Y != 0f)
						{
							Audio.Play("event:/new_content/char/tutorial_ghost/jump_super", this.Position);
							return;
						}
						if (chaserState.Scale.X > 0f)
						{
							Audio.Play("event:/new_content/char/tutorial_ghost/dash_red_right", this.Position);
							return;
						}
						Audio.Play("event:/new_content/char/tutorial_ghost/dash_red_left", this.Position);
						return;
					}
					else
					{
						if (currentAnimationID2 == "climbUp" || currentAnimationID2 == "climbDown" || currentAnimationID2 == "wallslide")
						{
							Audio.Play("event:/new_content/char/tutorial_ghost/grab", this.Position);
							return;
						}
						if ((currentAnimationID2.Equals("runSlow_carry") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (currentAnimationID2.Equals("runFast") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (currentAnimationID2.Equals("runSlow") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (currentAnimationID2.Equals("walk") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (currentAnimationID2.Equals("runStumble") && currentAnimationFrame == 6) || (currentAnimationID2.Equals("flip") && currentAnimationFrame == 4) || (currentAnimationID2.Equals("runWind") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (currentAnimationID2.Equals("idleC") && this.Sprite.Mode == PlayerSpriteMode.MadelineNoBackpack && (currentAnimationFrame == 3 || currentAnimationFrame == 6 || currentAnimationFrame == 8 || currentAnimationFrame == 11)) || (currentAnimationID2.Equals("carryTheoWalk") && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (currentAnimationID2.Equals("push") && (currentAnimationFrame == 8 || currentAnimationFrame == 15)))
						{
							Audio.Play("event:/new_content/char/tutorial_ghost/footstep", this.Position);
						}
					}
				}
			}
		}

		// Token: 0x06000F17 RID: 3863 RVA: 0x0003BBD8 File Offset: 0x00039DD8
		public override void Update()
		{
			if (this.startDelay > 0f)
			{
				this.startDelay -= Engine.DeltaTime;
			}
			this.LastPosition = this.Position;
			base.Update();
			if (this.index >= this.Timeline.Count - 1 || this.Time >= this.TrimEnd)
			{
				if (this.Visible)
				{
					Audio.Play("event:/new_content/char/tutorial_ghost/disappear", this.Position);
				}
				this.Visible = false;
				this.Position = this.start;
				this.loopDelay -= Engine.DeltaTime;
				if (this.loopDelay <= 0f)
				{
					Player player = (base.Scene == null) ? null : base.Scene.Tracker.GetEntity<Player>();
					if (player == null || (player.X > this.rangeMinX && player.X < this.rangeMaxX))
					{
						this.Restart();
					}
				}
			}
			else if (this.startDelay <= 0f)
			{
				this.SetFrame(this.index);
				this.time += Engine.DeltaTime;
				while (this.index < this.Timeline.Count - 1 && this.time >= this.Timeline[this.index + 1].TimeStamp)
				{
					this.index++;
				}
			}
			if (this.Visible && this.ShowTrail && base.Scene != null && base.Scene.OnInterval(0.1f))
			{
				TrailManager.Add(this.Position, this.Sprite, this.Hair, this.Sprite.Scale, this.Hair.Color, base.Depth + 1, 1f, false, false);
			}
		}

		// Token: 0x04000A5E RID: 2654
		public Vector2 LastPosition;

		// Token: 0x04000A5F RID: 2655
		public List<Player.ChaserState> Timeline;

		// Token: 0x04000A60 RID: 2656
		public PlayerSprite Sprite;

		// Token: 0x04000A61 RID: 2657
		public PlayerHair Hair;

		// Token: 0x04000A62 RID: 2658
		private Vector2 start;

		// Token: 0x04000A63 RID: 2659
		private float time;

		// Token: 0x04000A64 RID: 2660
		private int index;

		// Token: 0x04000A65 RID: 2661
		private float loopDelay;

		// Token: 0x04000A66 RID: 2662
		private float startDelay;

		// Token: 0x04000A68 RID: 2664
		public float TrimStart;

		// Token: 0x04000A69 RID: 2665
		public float TrimEnd;

		// Token: 0x04000A6A RID: 2666
		public readonly float Duration;

		// Token: 0x04000A6B RID: 2667
		private float rangeMinX = float.MinValue;

		// Token: 0x04000A6C RID: 2668
		private float rangeMaxX = float.MaxValue;

		// Token: 0x04000A6D RID: 2669
		private bool ShowTrail;
	}
}
