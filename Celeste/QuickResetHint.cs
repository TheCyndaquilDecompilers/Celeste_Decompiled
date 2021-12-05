using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x020001EF RID: 495
	public class QuickResetHint : Entity
	{
		// Token: 0x06001052 RID: 4178 RVA: 0x00047580 File Offset: 0x00045780
		public QuickResetHint()
		{
			base.Tag = Tags.HUD;
			Buttons buttons = Buttons.LeftShoulder;
			Buttons buttons2 = Buttons.RightShoulder;
			this.textStart = Dialog.Clean("UI_QUICK_RESTART_TITLE", null) + " ";
			this.textHold = Dialog.Clean("UI_QUICK_RESTART_HOLD", null);
			this.textPress = Dialog.Clean("UI_QUICK_RESTART_PRESS", null);
			if (Settings.Instance.Language == "japanese")
			{
				this.controllerList = new List<object>
				{
					this.textStart,
					buttons,
					buttons2,
					this.textHold,
					"、",
					Input.FirstButton(Input.Pause),
					this.textPress
				};
				this.keyboardList = new List<object>
				{
					this.textStart,
					Input.FirstKey(Input.QuickRestart),
					this.textPress
				};
				return;
			}
			this.controllerList = new List<object>
			{
				this.textStart,
				this.textHold,
				buttons,
				buttons2,
				",  ",
				this.textPress,
				Input.FirstButton(Input.Pause)
			};
			this.keyboardList = new List<object>
			{
				this.textStart,
				this.textPress,
				Input.FirstKey(Input.QuickRestart)
			};
		}

		// Token: 0x06001053 RID: 4179 RVA: 0x00047744 File Offset: 0x00045944
		public override void Render()
		{
			List<object> list = Input.GuiInputController(Input.PrefixMode.Latest) ? this.controllerList : this.keyboardList;
			float num = 0f;
			foreach (object obj in list)
			{
				if (obj is string)
				{
					num += ActiveFont.Measure(obj as string).X;
				}
				else if (obj is Buttons)
				{
					num += (float)Input.GuiSingleButton((Buttons)obj, Input.PrefixMode.Latest, "controls/keyboard/oemquestion").Width + 16f;
				}
				else if (obj is Keys)
				{
					num += (float)Input.GuiKey((Keys)obj, "controls/keyboard/oemquestion").Width + 16f;
				}
			}
			num *= 0.75f;
			Vector2 vector = new Vector2((1920f - num) / 2f, 980f);
			foreach (object obj2 in list)
			{
				if (obj2 is string)
				{
					ActiveFont.DrawOutline(obj2 as string, vector, new Vector2(0f, 0.5f), Vector2.One * 0.75f, Color.LightGray, 2f, Color.Black);
					vector.X += ActiveFont.Measure(obj2 as string).X * 0.75f;
				}
				else if (obj2 is Buttons)
				{
					MTexture mtexture = Input.GuiSingleButton((Buttons)obj2, Input.PrefixMode.Latest, "controls/keyboard/oemquestion");
					mtexture.DrawJustified(vector + new Vector2(((float)mtexture.Width + 16f) * 0.75f * 0.5f, 0f), new Vector2(0.5f, 0.5f), Color.White, 0.75f);
					vector.X += ((float)mtexture.Width + 16f) * 0.75f;
				}
				else if (obj2 is Keys)
				{
					MTexture mtexture2 = Input.GuiKey((Keys)obj2, "controls/keyboard/oemquestion");
					mtexture2.DrawJustified(vector + new Vector2(((float)mtexture2.Width + 16f) * 0.75f * 0.5f, 0f), new Vector2(0.5f, 0.5f), Color.White, 0.75f);
					vector.X += ((float)mtexture2.Width + 16f) * 0.75f;
				}
			}
		}

		// Token: 0x04000BAD RID: 2989
		private string textStart;

		// Token: 0x04000BAE RID: 2990
		private string textHold;

		// Token: 0x04000BAF RID: 2991
		private string textPress;

		// Token: 0x04000BB0 RID: 2992
		private List<object> controllerList;

		// Token: 0x04000BB1 RID: 2993
		private List<object> keyboardList;
	}
}
