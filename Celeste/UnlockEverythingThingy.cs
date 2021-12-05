using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001CE RID: 462
	public class UnlockEverythingThingy : CheatListener
	{
		// Token: 0x06000FB5 RID: 4021 RVA: 0x000427E8 File Offset: 0x000409E8
		public UnlockEverythingThingy()
		{
			base.AddInput('u', () => Input.MenuUp.Pressed && !Input.MenuUp.Repeating);
			base.AddInput('d', () => Input.MenuDown.Pressed && !Input.MenuDown.Repeating);
			base.AddInput('r', () => Input.MenuRight.Pressed && !Input.MenuRight.Repeating);
			base.AddInput('l', () => Input.MenuLeft.Pressed && !Input.MenuLeft.Repeating);
			base.AddInput('A', () => Input.MenuConfirm.Pressed);
			base.AddInput('L', () => Input.MenuJournal.Pressed);
			base.AddInput('R', () => Input.Grab.Pressed && !Input.MenuJournal.Pressed);
			base.AddCheat("lrLRuudlRA", new Action(this.EnteredCheat));
			this.Logging = true;
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x0004292C File Offset: 0x00040B2C
		public void EnteredCheat()
		{
			Level level = base.SceneAs<Level>();
			level.PauseLock = true;
			level.Frozen = true;
			level.Flash(Color.White, false);
			Audio.Play("event:/game/06_reflection/feather_bubble_get", (base.Scene as Level).Camera.Position + new Vector2(160f, 90f));
			new FadeWipe(base.Scene, false, delegate()
			{
				this.UnlockEverything(level);
			}).Duration = 2f;
			base.RemoveSelf();
		}

		// Token: 0x06000FB7 RID: 4023 RVA: 0x000429D8 File Offset: 0x00040BD8
		public void UnlockEverything(Level level)
		{
			SaveData.Instance.RevealedChapter9 = true;
			SaveData.Instance.UnlockedAreas = SaveData.Instance.MaxArea;
			SaveData.Instance.CheatMode = true;
			Settings.Instance.Pico8OnMainMenu = true;
			Settings.Instance.VariantsUnlocked = true;
			level.Session.InArea = false;
			Engine.Scene = new LevelExit(LevelExit.Mode.GiveUp, level.Session, null);
		}
	}
}
