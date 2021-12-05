using System;
using System.Collections;
using Celeste.Pico8;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002D0 RID: 720
	public class PicoConsole : Entity
	{
		// Token: 0x0600163C RID: 5692 RVA: 0x00082868 File Offset: 0x00080A68
		public PicoConsole(Vector2 position) : base(position)
		{
			base.Depth = 1000;
			base.AddTag(Tags.TransitionUpdate);
			base.AddTag(Tags.PauseUpdate);
			base.Add(this.sprite = new Image(GFX.Game["objects/pico8Console"]));
			this.sprite.JustifyOrigin(0.5f, 1f);
			base.Add(this.talk = new TalkComponent(new Rectangle(-12, -8, 24, 8), new Vector2(0f, -24f), new Action<Player>(this.OnInteract), null));
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x0008291C File Offset: 0x00080B1C
		public PicoConsole(EntityData data, Vector2 position) : this(data.Position + position)
		{
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x00082930 File Offset: 0x00080B30
		public override void Update()
		{
			base.Update();
			if (this.sfx == null)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.Y < base.Y + 16f)
				{
					base.Add(this.sfx = new SoundSource("event:/env/local/03_resort/pico8_machine"));
				}
			}
		}

		// Token: 0x0600163F RID: 5695 RVA: 0x0008298C File Offset: 0x00080B8C
		private void OnInteract(Player player)
		{
			if (!this.talking)
			{
				(base.Scene as Level).PauseLock = true;
				this.talking = true;
				base.Add(new Coroutine(this.InteractRoutine(player), true));
			}
		}

		// Token: 0x06001640 RID: 5696 RVA: 0x000829C1 File Offset: 0x00080BC1
		private IEnumerator InteractRoutine(Player player)
		{
			player.StateMachine.State = 11;
			yield return player.DummyWalkToExact((int)base.X - 6, false, 1f, false);
			player.Facing = Facings.Right;
			bool wasUnlocked = Settings.Instance.Pico8OnMainMenu;
			Settings.Instance.Pico8OnMainMenu = true;
			if (!wasUnlocked)
			{
				UserIO.SaveHandler(false, true);
				while (UserIO.Saving)
				{
					yield return null;
				}
			}
			else
			{
				yield return 0.5f;
			}
			bool done = false;
			SpotlightWipe.FocusPoint = player.Position - (base.Scene as Level).Camera.Position + new Vector2(0f, -8f);
			Action <>9__1;
			new SpotlightWipe(base.Scene, false, delegate()
			{
				if (!wasUnlocked)
				{
					Scene scene = this.Scene;
					Action callback;
					if ((callback = <>9__1) == null)
					{
						callback = (<>9__1 = delegate()
						{
							done = true;
						});
					}
					scene.Add(new UnlockedPico8Message(callback));
				}
				else
				{
					done = true;
				}
				Engine.Scene = new Emulator(this.Scene as Level, 0, 0);
			});
			while (!done)
			{
				yield return null;
			}
			yield return 0.25f;
			this.talking = false;
			(base.Scene as Level).PauseLock = false;
			player.StateMachine.State = 0;
			yield break;
		}

		// Token: 0x06001641 RID: 5697 RVA: 0x000829D7 File Offset: 0x00080BD7
		public override void SceneEnd(Scene scene)
		{
			if (this.sfx != null)
			{
				this.sfx.Stop(true);
				this.sfx.RemoveSelf();
				this.sfx = null;
			}
			base.SceneEnd(scene);
		}

		// Token: 0x04001299 RID: 4761
		private Image sprite;

		// Token: 0x0400129A RID: 4762
		private TalkComponent talk;

		// Token: 0x0400129B RID: 4763
		private bool talking;

		// Token: 0x0400129C RID: 4764
		private SoundSource sfx;
	}
}
