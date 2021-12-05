using System;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002BB RID: 699
	[Tracked(false)]
	public class SoundSource : Component
	{
		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06001588 RID: 5512 RVA: 0x0007C4D0 File Offset: 0x0007A6D0
		// (set) Token: 0x06001589 RID: 5513 RVA: 0x0007C4D8 File Offset: 0x0007A6D8
		public bool Playing { get; private set; }

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x0600158A RID: 5514 RVA: 0x0007C4E1 File Offset: 0x0007A6E1
		public bool Is3D
		{
			get
			{
				return this.is3D;
			}
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x0600158B RID: 5515 RVA: 0x0007C4E9 File Offset: 0x0007A6E9
		public bool IsOneshot
		{
			get
			{
				return this.isOneshot;
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x0600158C RID: 5516 RVA: 0x0007C4F4 File Offset: 0x0007A6F4
		public bool InstancePlaying
		{
			get
			{
				if (this.instance != null)
				{
					PLAYBACK_STATE playback_STATE;
					this.instance.getPlaybackState(out playback_STATE);
					if (playback_STATE == PLAYBACK_STATE.PLAYING || playback_STATE == PLAYBACK_STATE.STARTING || playback_STATE == PLAYBACK_STATE.SUSTAINING)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x0007C52B File Offset: 0x0007A72B
		public SoundSource() : base(true, false)
		{
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x0007C547 File Offset: 0x0007A747
		public SoundSource(string path) : this()
		{
			this.Play(path, null, 0f);
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x0007C55D File Offset: 0x0007A75D
		public SoundSource(Vector2 offset, string path) : this()
		{
			this.Position = offset;
			this.Play(path, null, 0f);
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x0007C57A File Offset: 0x0007A77A
		public override void Added(Entity entity)
		{
			base.Added(entity);
			this.UpdateSfxPosition();
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x0007C58C File Offset: 0x0007A78C
		public SoundSource Play(string path, string param = null, float value = 0f)
		{
			this.Stop(true);
			this.EventName = path;
			EventDescription eventDescription = Audio.GetEventDescription(path);
			if (eventDescription != null)
			{
				eventDescription.createInstance(out this.instance);
				eventDescription.is3D(out this.is3D);
				eventDescription.isOneshot(out this.isOneshot);
			}
			if (this.instance != null)
			{
				if (this.is3D)
				{
					Vector2 vector = this.Position;
					if (base.Entity != null)
					{
						vector += base.Entity.Position;
					}
					Audio.Position(this.instance, vector);
				}
				if (param != null)
				{
					this.instance.setParameterValue(param, value);
				}
				this.instance.start();
				this.Playing = true;
			}
			return this;
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x0007C647 File Offset: 0x0007A847
		public SoundSource Param(string param, float value)
		{
			if (this.instance != null)
			{
				this.instance.setParameterValue(param, value);
			}
			return this;
		}

		// Token: 0x06001593 RID: 5523 RVA: 0x0007C666 File Offset: 0x0007A866
		public SoundSource Pause()
		{
			if (this.instance != null)
			{
				this.instance.setPaused(true);
			}
			this.Playing = false;
			return this;
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x0007C68C File Offset: 0x0007A88C
		public SoundSource Resume()
		{
			if (this.instance != null)
			{
				bool flag;
				this.instance.getPaused(out flag);
				if (flag)
				{
					this.instance.setPaused(false);
					this.Playing = true;
				}
			}
			return this;
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x0007C6CD File Offset: 0x0007A8CD
		public SoundSource Stop(bool allowFadeout = true)
		{
			Audio.Stop(this.instance, allowFadeout);
			this.instance = null;
			this.Playing = false;
			return this;
		}

		// Token: 0x06001596 RID: 5526 RVA: 0x0007C6EC File Offset: 0x0007A8EC
		public void UpdateSfxPosition()
		{
			if (this.is3D && this.instance != null)
			{
				Vector2 vector = this.Position;
				if (base.Entity != null)
				{
					vector += base.Entity.Position;
				}
				Audio.Position(this.instance, vector);
			}
		}

		// Token: 0x06001597 RID: 5527 RVA: 0x0007C73C File Offset: 0x0007A93C
		public override void Update()
		{
			this.UpdateSfxPosition();
			if (this.isOneshot && this.instance != null)
			{
				PLAYBACK_STATE playback_STATE;
				this.instance.getPlaybackState(out playback_STATE);
				if (playback_STATE == PLAYBACK_STATE.STOPPED)
				{
					this.instance.release();
					this.instance = null;
					this.Playing = false;
					if (this.RemoveOnOneshotEnd)
					{
						base.RemoveSelf();
					}
				}
			}
		}

		// Token: 0x06001598 RID: 5528 RVA: 0x0007C79F File Offset: 0x0007A99F
		public override void EntityRemoved(Scene scene)
		{
			base.EntityRemoved(scene);
			this.Stop(true);
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x0007C7B0 File Offset: 0x0007A9B0
		public override void Removed(Entity entity)
		{
			base.Removed(entity);
			this.Stop(true);
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x0007C7C1 File Offset: 0x0007A9C1
		public override void SceneEnd(Scene scene)
		{
			base.SceneEnd(scene);
			this.Stop(false);
		}

		// Token: 0x0600159B RID: 5531 RVA: 0x0007C7D4 File Offset: 0x0007A9D4
		public override void DebugRender(Camera camera)
		{
			Vector2 vector = this.Position;
			if (base.Entity != null)
			{
				vector += base.Entity.Position;
			}
			if (this.instance != null && this.Playing)
			{
				Draw.Circle(vector, 4f + base.Scene.RawTimeActive * 2f % 1f * 16f, Color.BlueViolet, 16);
			}
			Draw.HollowRect(vector.X - 2f, vector.Y - 2f, 4f, 4f, Color.BlueViolet);
		}

		// Token: 0x040011AE RID: 4526
		public string EventName;

		// Token: 0x040011AF RID: 4527
		public Vector2 Position = Vector2.Zero;

		// Token: 0x040011B1 RID: 4529
		public bool DisposeOnTransition = true;

		// Token: 0x040011B2 RID: 4530
		public bool RemoveOnOneshotEnd;

		// Token: 0x040011B3 RID: 4531
		private EventInstance instance;

		// Token: 0x040011B4 RID: 4532
		private bool is3D;

		// Token: 0x040011B5 RID: 4533
		private bool isOneshot;
	}
}
