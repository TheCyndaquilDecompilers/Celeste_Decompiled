using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000330 RID: 816
	public class LockBlock : Solid
	{
		// Token: 0x06001999 RID: 6553 RVA: 0x000A4DC4 File Offset: 0x000A2FC4
		public LockBlock(Vector2 position, EntityID id, bool stepMusicProgress, string spriteName, string unlock_sfx) : base(position, 32f, 32f, false)
		{
			this.ID = id;
			this.DisableLightsInside = false;
			this.stepMusicProgress = stepMusicProgress;
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), new Circle(60f, 16f, 16f), null));
			base.Add(this.sprite = GFX.SpriteBank.Create("lockdoor_" + spriteName));
			this.sprite.Play("idle", false, false);
			this.sprite.Position = new Vector2(base.Width / 2f, base.Height / 2f);
			if (string.IsNullOrWhiteSpace(unlock_sfx))
			{
				this.unlockSfxName = "event:/game/03_resort/key_unlock";
				if (spriteName == "temple_a")
				{
					this.unlockSfxName = "event:/game/05_mirror_temple/key_unlock_light";
					return;
				}
				if (spriteName == "temple_b")
				{
					this.unlockSfxName = "event:/game/05_mirror_temple/key_unlock_dark";
					return;
				}
			}
			else
			{
				this.unlockSfxName = SFX.EventnameByHandle(unlock_sfx);
			}
		}

		// Token: 0x0600199A RID: 6554 RVA: 0x000A4ED8 File Offset: 0x000A30D8
		public LockBlock(EntityData data, Vector2 offset, EntityID id) : this(data.Position + offset, id, data.Bool("stepMusicProgress", false), data.Attr("sprite", "wood"), data.Attr("unlock_sfx", null))
		{
		}

		// Token: 0x0600199B RID: 6555 RVA: 0x000A4F15 File Offset: 0x000A3115
		public void Appear()
		{
			this.Visible = true;
			this.sprite.Play("appear", false, false);
			base.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate
			{
				Level level = base.Scene as Level;
				if (!base.CollideCheck<Solid>(this.Position - Vector2.UnitX))
				{
					level.Particles.Emit(LockBlock.P_Appear, 16, this.Position + new Vector2(3f, 16f), new Vector2(2f, 10f), 3.1415927f);
					level.Particles.Emit(LockBlock.P_Appear, 16, this.Position + new Vector2(29f, 16f), new Vector2(2f, 10f), 0f);
				}
				level.Shake(0.3f);
			}, 0.25f, true));
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x000A4F50 File Offset: 0x000A3150
		private void OnPlayer(Player player)
		{
			if (!this.opening)
			{
				foreach (Follower follower in player.Leader.Followers)
				{
					if (follower.Entity is Key && !(follower.Entity as Key).StartedUsing)
					{
						this.TryOpen(player, follower);
						break;
					}
				}
			}
		}

		// Token: 0x0600199D RID: 6557 RVA: 0x000A4FD4 File Offset: 0x000A31D4
		private void TryOpen(Player player, Follower fol)
		{
			this.Collidable = false;
			if (!base.Scene.CollideCheck<Solid>(player.Center, base.Center))
			{
				this.opening = true;
				(fol.Entity as Key).StartedUsing = true;
				base.Add(new Coroutine(this.UnlockRoutine(fol), true));
			}
			this.Collidable = true;
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x000A5033 File Offset: 0x000A3233
		private IEnumerator UnlockRoutine(Follower fol)
		{
			SoundEmitter emitter = SoundEmitter.Play(this.unlockSfxName, this, null);
			emitter.Source.DisposeOnTransition = true;
			Level level = base.SceneAs<Level>();
			Key key = fol.Entity as Key;
			base.Add(new Coroutine(key.UseRoutine(base.Center + new Vector2(0f, 2f)), true));
			yield return 1.2f;
			this.UnlockingRegistered = true;
			if (this.stepMusicProgress)
			{
				AudioTrackState music = level.Session.Audio.Music;
				int progress = music.Progress;
				music.Progress = progress + 1;
				level.Session.Audio.Apply(false);
			}
			level.Session.DoNotLoad.Add(this.ID);
			key.RegisterUsed();
			while (key.Turning)
			{
				yield return null;
			}
			base.Tag |= Tags.TransitionUpdate;
			this.Collidable = false;
			emitter.Source.DisposeOnTransition = false;
			yield return this.sprite.PlayRoutine("open", false);
			level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			yield return this.sprite.PlayRoutine("burst", false);
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x04001656 RID: 5718
		public static ParticleType P_Appear;

		// Token: 0x04001657 RID: 5719
		public EntityID ID;

		// Token: 0x04001658 RID: 5720
		public bool UnlockingRegistered;

		// Token: 0x04001659 RID: 5721
		private Sprite sprite;

		// Token: 0x0400165A RID: 5722
		private bool opening;

		// Token: 0x0400165B RID: 5723
		private bool stepMusicProgress;

		// Token: 0x0400165C RID: 5724
		private string unlockSfxName;
	}
}
