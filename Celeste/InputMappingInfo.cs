using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x020001FB RID: 507
	public class InputMappingInfo : TextMenu.Item
	{
		// Token: 0x060010A3 RID: 4259 RVA: 0x0004D048 File Offset: 0x0004B248
		public InputMappingInfo(bool controllerMode)
		{
			string[] array = Dialog.Clean("BTN_CONFIG_INFO", null).Split(new char[]
			{
				'|'
			});
			if (array.Length == 3)
			{
				this.info.Add(array[0]);
				this.info.Add(Input.MenuConfirm);
				this.info.Add(array[1]);
				this.info.Add(Input.MenuJournal);
				this.info.Add(array[2]);
			}
			this.controllerMode = controllerMode;
			this.AboveAll = true;
		}

		// Token: 0x060010A4 RID: 4260 RVA: 0x0004D0E0 File Offset: 0x0004B2E0
		public override float LeftWidth()
		{
			return 100f;
		}

		// Token: 0x060010A5 RID: 4261 RVA: 0x0004D0E7 File Offset: 0x0004B2E7
		public override float Height()
		{
			return ActiveFont.LineHeight * 2f;
		}

		// Token: 0x060010A6 RID: 4262 RVA: 0x0004D0F4 File Offset: 0x0004B2F4
		public override void Update()
		{
			this.borderEase = Calc.Approach(this.borderEase, this.fixedPosition ? 1f : 0f, Engine.DeltaTime * 4f);
			base.Update();
		}

		// Token: 0x060010A7 RID: 4263 RVA: 0x0004D12C File Offset: 0x0004B32C
		public override void Render(Vector2 position, bool highlighted)
		{
			this.fixedPosition = false;
			if (position.Y < 100f)
			{
				this.fixedPosition = true;
				position.Y = 100f;
			}
			Color color = Color.Gray * Ease.CubeOut(this.Container.Alpha);
			Color strokeColor = Color.Black * Ease.CubeOut(this.Container.Alpha);
			Color color2 = Color.White * Ease.CubeOut(this.Container.Alpha);
			float num = 0f;
			for (int i = 0; i < this.info.Count; i++)
			{
				if (this.info[i] is string)
				{
					string text = this.info[i] as string;
					num += ActiveFont.Measure(text).X * 0.6f;
				}
				else if (this.info[i] is VirtualButton)
				{
					VirtualButton virtualButton = this.info[i] as VirtualButton;
					if (this.controllerMode)
					{
						MTexture mtexture = Input.GuiButton(virtualButton, Input.PrefixMode.Attached, "controls/keyboard/oemquestion");
						num += (float)mtexture.Width * 0.6f;
					}
					else if (virtualButton.Binding.Keyboard.Count > 0)
					{
						MTexture mtexture2 = Input.GuiKey(virtualButton.Binding.Keyboard[0], "controls/keyboard/oemquestion");
						num += (float)mtexture2.Width * 0.6f;
					}
					else
					{
						MTexture mtexture3 = Input.GuiKey(Keys.None, "controls/keyboard/oemquestion");
						num += (float)mtexture3.Width * 0.6f;
					}
				}
			}
			Vector2 vector = position + new Vector2(this.Container.Width - num, 0f) / 2f;
			if (this.borderEase > 0f)
			{
				Draw.HollowRect(vector.X - 22f, vector.Y - 42f, num + 44f, 84f, Color.White * Ease.CubeOut(this.Container.Alpha) * this.borderEase);
				Draw.HollowRect(vector.X - 21f, vector.Y - 41f, num + 42f, 82f, Color.White * Ease.CubeOut(this.Container.Alpha) * this.borderEase);
				Draw.Rect(vector.X - 20f, vector.Y - 40f, num + 40f, 80f, Color.Black * Ease.CubeOut(this.Container.Alpha));
			}
			for (int j = 0; j < this.info.Count; j++)
			{
				if (this.info[j] is string)
				{
					string text2 = this.info[j] as string;
					ActiveFont.DrawOutline(text2, vector, new Vector2(0f, 0.5f), Vector2.One * 0.6f, color, 2f, strokeColor);
					vector.X += ActiveFont.Measure(text2).X * 0.6f;
				}
				else if (this.info[j] is VirtualButton)
				{
					VirtualButton virtualButton2 = this.info[j] as VirtualButton;
					if (this.controllerMode)
					{
						MTexture mtexture4 = Input.GuiButton(virtualButton2, Input.PrefixMode.Attached, "controls/keyboard/oemquestion");
						mtexture4.DrawJustified(vector, new Vector2(0f, 0.5f), color2, 0.6f);
						vector.X += (float)mtexture4.Width * 0.6f;
					}
					else if (virtualButton2.Binding.Keyboard.Count > 0)
					{
						MTexture mtexture5 = Input.GuiKey(virtualButton2.Binding.Keyboard[0], "controls/keyboard/oemquestion");
						mtexture5.DrawJustified(vector, new Vector2(0f, 0.5f), color2, 0.6f);
						vector.X += (float)mtexture5.Width * 0.6f;
					}
					else
					{
						MTexture mtexture6 = Input.GuiKey(Keys.None, "controls/keyboard/oemquestion");
						mtexture6.DrawJustified(vector, new Vector2(0f, 0.5f), color2, 0.6f);
						vector.X += (float)mtexture6.Width * 0.6f;
					}
				}
			}
		}

		// Token: 0x04000C1E RID: 3102
		private List<object> info = new List<object>();

		// Token: 0x04000C1F RID: 3103
		private bool controllerMode;

		// Token: 0x04000C20 RID: 3104
		private float borderEase;

		// Token: 0x04000C21 RID: 3105
		private bool fixedPosition;
	}
}
