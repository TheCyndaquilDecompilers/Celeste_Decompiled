using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001FF RID: 511
	public class UnlockedPico8Message : Entity
	{
		// Token: 0x060010BA RID: 4282 RVA: 0x0004E53D File Offset: 0x0004C73D
		public UnlockedPico8Message(Action callback = null)
		{
			this.callback = callback;
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x0004E54C File Offset: 0x0004C74C
		public override void Added(Scene scene)
		{
			base.Added(scene);
			base.Tag = (Tags.HUD | Tags.PauseUpdate);
			this.text = ActiveFont.FontSize.AutoNewline(Dialog.Clean("PICO8_UNLOCKED", null), 900);
			base.Depth = -10000;
			base.Add(new Coroutine(this.Routine(), true));
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x0004E5B8 File Offset: 0x0004C7B8
		private IEnumerator Routine()
		{
			Level level = base.Scene as Level;
			level.PauseLock = true;
			level.Paused = true;
			while ((this.alpha += Engine.DeltaTime / 0.5f) < 1f)
			{
				yield return null;
			}
			this.alpha = 1f;
			this.waitForKeyPress = true;
			while (!Input.MenuConfirm.Pressed)
			{
				yield return null;
			}
			this.waitForKeyPress = false;
			while ((this.alpha -= Engine.DeltaTime / 0.5f) > 0f)
			{
				yield return null;
			}
			this.alpha = 0f;
			level.PauseLock = false;
			level.Paused = false;
			base.RemoveSelf();
			if (this.callback != null)
			{
				this.callback();
			}
			yield break;
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x0004E5C7 File Offset: 0x0004C7C7
		public override void Update()
		{
			this.timer += Engine.DeltaTime;
			base.Update();
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x0004E5E4 File Offset: 0x0004C7E4
		public override void Render()
		{
			float num = Ease.CubeOut(this.alpha);
			Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * num * 0.8f);
			GFX.Gui["pico8"].DrawJustified(Celeste.TargetCenter + new Vector2(0f, -64f * (1f - num) - 16f), new Vector2(0.5f, 1f), Color.White * num);
			Vector2 position = Celeste.TargetCenter + new Vector2(0f, 64f * (1f - num) + 16f);
			Vector2 vector = ActiveFont.Measure(this.text);
			ActiveFont.Draw(this.text, position, new Vector2(0.5f, 0f), Vector2.One, Color.White * num);
			if (this.waitForKeyPress)
			{
				GFX.Gui["textboxbutton"].DrawCentered(Celeste.TargetCenter + new Vector2(vector.X / 2f + 32f, vector.Y + 48f + (float)((this.timer % 1f < 0.25f) ? 6 : 0)));
			}
		}

		// Token: 0x04000C32 RID: 3122
		private float alpha;

		// Token: 0x04000C33 RID: 3123
		private string text;

		// Token: 0x04000C34 RID: 3124
		private bool waitForKeyPress;

		// Token: 0x04000C35 RID: 3125
		private float timer;

		// Token: 0x04000C36 RID: 3126
		private Action callback;
	}
}
