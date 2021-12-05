using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000283 RID: 643
	public class CS03_OshiroClutter : CutsceneEntity
	{
		// Token: 0x060013E0 RID: 5088 RVA: 0x0006BA1D File Offset: 0x00069C1D
		public CS03_OshiroClutter(Player player, NPC03_Oshiro_Cluttter oshiro, int index) : base(true, false)
		{
			this.player = player;
			this.oshiro = oshiro;
			this.index = index;
		}

		// Token: 0x060013E1 RID: 5089 RVA: 0x0006BA3C File Offset: 0x00069C3C
		public override void OnBegin(Level level)
		{
			this.doors = base.Scene.Entities.FindAll<ClutterDoor>();
			this.doors.Sort((ClutterDoor a, ClutterDoor b) => (int)(a.Y - b.Y));
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x060013E2 RID: 5090 RVA: 0x0006BA9C File Offset: 0x00069C9C
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			int num;
			if (this.index == 1 || this.index == 2)
			{
				num = -1;
			}
			else
			{
				num = 1;
			}
			if (num == -1)
			{
				yield return this.player.DummyWalkToExact((int)this.oshiro.X - 24, false, 1f, false);
				this.player.Facing = Facings.Right;
				this.oshiro.Sprite.Scale.X = -1f;
			}
			else
			{
				base.Add(new Coroutine(this.oshiro.PaceRight(), true));
				yield return this.player.DummyWalkToExact((int)this.oshiro.HomePosition.X + 24, false, 1f, false);
				this.player.Facing = Facings.Left;
				this.oshiro.Sprite.Scale.X = 1f;
			}
			if (this.index < 3)
			{
				yield return this.Level.ZoomTo(this.oshiro.ZoomPoint, 2f, 0.5f);
				yield return Textbox.Say("CH3_OSHIRO_CLUTTER" + this.index, new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(this.Collapse),
					new Func<IEnumerator>(this.oshiro.PaceLeft),
					new Func<IEnumerator>(this.oshiro.PaceRight)
				});
				yield return this.Level.ZoomBack(0.5f);
				level.Session.SetFlag("oshiro_clutter_door_open", true);
				if (this.index == 0)
				{
					this.SetMusic();
				}
				foreach (ClutterDoor clutterDoor in this.doors)
				{
					if (!clutterDoor.IsLocked(level.Session))
					{
						yield return clutterDoor.UnlockRoutine();
					}
				}
				List<ClutterDoor>.Enumerator enumerator = default(List<ClutterDoor>.Enumerator);
			}
			else
			{
				yield return CutsceneEntity.CameraTo(new Vector2((float)this.Level.Bounds.X, (float)this.Level.Bounds.Y), 0.5f, null, 0f);
				yield return this.Level.ZoomTo(new Vector2(90f, 60f), 2f, 0.5f);
				yield return Textbox.Say("CH3_OSHIRO_CLUTTER_ENDING", new Func<IEnumerator>[0]);
				yield return this.oshiro.MoveTo(new Vector2(this.oshiro.X, (float)(level.Bounds.Top - 32)), false, null, false);
				this.oshiro.Add(new SoundSource("event:/char/oshiro/move_05_09b_exit"));
				yield return this.Level.ZoomBack(0.5f);
			}
			base.EndCutscene(level, true);
			yield break;
			yield break;
		}

		// Token: 0x060013E3 RID: 5091 RVA: 0x0006BAB2 File Offset: 0x00069CB2
		private IEnumerator Collapse()
		{
			Audio.Play("event:/char/oshiro/chat_collapse", this.oshiro.Position);
			this.oshiro.Sprite.Play("fall", false, false);
			yield return 0.5f;
			yield break;
		}

		// Token: 0x060013E4 RID: 5092 RVA: 0x0006BAC4 File Offset: 0x00069CC4
		private void SetMusic()
		{
			Level level = base.Scene as Level;
			level.Session.Audio.Music.Event = "event:/music/lvl3/clean";
			level.Session.Audio.Music.Progress = 1;
			level.Session.Audio.Apply(false);
		}

		// Token: 0x060013E5 RID: 5093 RVA: 0x0006BB1C File Offset: 0x00069D1C
		public override void OnEnd(Level level)
		{
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			if (this.oshiro.Sprite.CurrentAnimationID == "side")
			{
				(this.oshiro.Sprite as OshiroSprite).Pop("idle", true);
			}
			if (this.index < 3)
			{
				level.Session.SetFlag("oshiro_clutter_door_open", true);
				level.Session.SetFlag("oshiro_clutter_" + this.index, true);
				if (this.index == 0 && this.WasSkipped)
				{
					this.SetMusic();
				}
				foreach (ClutterDoor clutterDoor in this.doors)
				{
					if (!clutterDoor.IsLocked(level.Session))
					{
						clutterDoor.InstantUnlock();
					}
				}
				if (this.WasSkipped && this.index == 0)
				{
					this.oshiro.Sprite.Play("idle_ground", false, false);
					return;
				}
			}
			else
			{
				level.Session.SetFlag("oshiro_clutter_finished", true);
				base.Scene.Remove(this.oshiro);
			}
		}

		// Token: 0x04000FA1 RID: 4001
		private int index;

		// Token: 0x04000FA2 RID: 4002
		private Player player;

		// Token: 0x04000FA3 RID: 4003
		private NPC03_Oshiro_Cluttter oshiro;

		// Token: 0x04000FA4 RID: 4004
		private List<ClutterDoor> doors;
	}
}
