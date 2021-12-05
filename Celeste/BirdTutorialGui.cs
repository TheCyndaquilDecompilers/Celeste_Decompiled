using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002E7 RID: 743
	public class BirdTutorialGui : Entity
	{
		// Token: 0x060016E0 RID: 5856 RVA: 0x00088768 File Offset: 0x00086968
		public BirdTutorialGui(Entity entity, Vector2 position, object info, params object[] controls)
		{
			base.AddTag(Tags.HUD);
			this.Entity = entity;
			this.Position = position;
			this.info = info;
			this.controls = new List<object>(controls);
			if (info is string)
			{
				this.infoWidth = ActiveFont.Measure((string)info).X;
				this.infoHeight = ActiveFont.LineHeight;
			}
			else if (info is MTexture)
			{
				this.infoWidth = (float)((MTexture)info).Width;
				this.infoHeight = (float)((MTexture)info).Height;
			}
			this.UpdateControlsSize();
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x00088850 File Offset: 0x00086A50
		public void UpdateControlsSize()
		{
			this.controlsWidth = 0f;
			foreach (object obj in this.controls)
			{
				if (obj is BirdTutorialGui.ButtonPrompt)
				{
					this.controlsWidth += (float)Input.GuiButton(BirdTutorialGui.ButtonPromptToVirtualButton((BirdTutorialGui.ButtonPrompt)obj), Input.PrefixMode.Latest, "controls/keyboard/oemquestion").Width + this.buttonPadding * 2f;
				}
				else if (obj is Vector2)
				{
					this.controlsWidth += (float)Input.GuiDirection((Vector2)obj).Width + this.buttonPadding * 2f;
				}
				else if (obj is string)
				{
					this.controlsWidth += ActiveFont.Measure(obj.ToString()).X;
				}
				else if (obj is MTexture)
				{
					this.controlsWidth += (float)((MTexture)obj).Width;
				}
			}
		}

		// Token: 0x060016E2 RID: 5858 RVA: 0x0008896C File Offset: 0x00086B6C
		public override void Update()
		{
			this.UpdateControlsSize();
			this.Scale = Calc.Approach(this.Scale, (float)(this.Open ? 1 : 0), Engine.RawDeltaTime * 8f);
			base.Update();
		}

		// Token: 0x060016E3 RID: 5859 RVA: 0x000889A4 File Offset: 0x00086BA4
		public override void Render()
		{
			Level level = base.Scene as Level;
			if (level.FrozenOrPaused || level.RetryPlayerCorpse != null || this.Scale <= 0f)
			{
				return;
			}
			Camera camera = base.SceneAs<Level>().Camera;
			Vector2 vector = this.Entity.Position + this.Position - camera.Position.Floor();
			if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
			{
				vector.X = 320f - vector.X;
			}
			vector.X *= 6f;
			vector.Y *= 6f;
			float lineHeight = ActiveFont.LineHeight;
			float num = (Math.Max(this.controlsWidth, this.infoWidth) + 64f) * this.Scale;
			float num2 = this.infoHeight + lineHeight + 32f;
			float num3 = vector.X - num / 2f;
			float num4 = vector.Y - num2 - 32f;
			Draw.Rect(num3 - 6f, num4 - 6f, num + 12f, num2 + 12f, this.lineColor);
			Draw.Rect(num3, num4, num, num2, this.bgColor);
			for (int i = 0; i <= 36; i++)
			{
				float num5 = (float)(73 - i * 2) * this.Scale;
				Draw.Rect(vector.X - num5 / 2f, num4 + num2 + (float)i, num5, 1f, this.lineColor);
				if (num5 > 12f)
				{
					Draw.Rect(vector.X - num5 / 2f + 6f, num4 + num2 + (float)i, num5 - 12f, 1f, this.bgColor);
				}
			}
			if (num > 3f)
			{
				Vector2 vector2 = new Vector2(vector.X, num4 + 16f);
				if (this.info is string)
				{
					ActiveFont.Draw((string)this.info, vector2, new Vector2(0.5f, 0f), new Vector2(this.Scale, 1f), this.textColor);
				}
				else if (this.info is MTexture)
				{
					((MTexture)this.info).DrawJustified(vector2, new Vector2(0.5f, 0f), Color.White, new Vector2(this.Scale, 1f));
				}
				vector2.Y += this.infoHeight + lineHeight * 0.5f;
				Vector2 vector3 = new Vector2(-this.controlsWidth / 2f, 0f);
				foreach (object obj in this.controls)
				{
					if (obj is BirdTutorialGui.ButtonPrompt)
					{
						MTexture mtexture = Input.GuiButton(BirdTutorialGui.ButtonPromptToVirtualButton((BirdTutorialGui.ButtonPrompt)obj), Input.PrefixMode.Latest, "controls/keyboard/oemquestion");
						vector3.X += this.buttonPadding;
						mtexture.Draw(vector2, new Vector2(-vector3.X, (float)(mtexture.Height / 2)), Color.White, new Vector2(this.Scale, 1f));
						vector3.X += (float)mtexture.Width + this.buttonPadding;
					}
					else if (obj is Vector2)
					{
						Vector2 vector4 = (Vector2)obj;
						if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
						{
							vector4.X = -vector4.X;
						}
						MTexture mtexture2 = Input.GuiDirection(vector4);
						vector3.X += this.buttonPadding;
						mtexture2.Draw(vector2, new Vector2(-vector3.X, (float)(mtexture2.Height / 2)), Color.White, new Vector2(this.Scale, 1f));
						vector3.X += (float)mtexture2.Width + this.buttonPadding;
					}
					else if (obj is string)
					{
						string text = obj.ToString();
						float x = ActiveFont.Measure(text).X;
						ActiveFont.Draw(text, vector2 + new Vector2(1f, 2f), new Vector2(-vector3.X / x, 0.5f), new Vector2(this.Scale, 1f), this.textColor);
						ActiveFont.Draw(text, vector2 + new Vector2(1f, -2f), new Vector2(-vector3.X / x, 0.5f), new Vector2(this.Scale, 1f), Color.White);
						vector3.X += x + 1f;
					}
					else if (obj is MTexture)
					{
						MTexture mtexture3 = (MTexture)obj;
						mtexture3.Draw(vector2, new Vector2(-vector3.X, (float)(mtexture3.Height / 2)), Color.White, new Vector2(this.Scale, 1f));
						vector3.X += (float)mtexture3.Width;
					}
				}
			}
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x00088EF8 File Offset: 0x000870F8
		public static VirtualButton ButtonPromptToVirtualButton(BirdTutorialGui.ButtonPrompt prompt)
		{
			switch (prompt)
			{
			case BirdTutorialGui.ButtonPrompt.Dash:
				return Input.Dash;
			case BirdTutorialGui.ButtonPrompt.Jump:
				return Input.Jump;
			case BirdTutorialGui.ButtonPrompt.Grab:
				return Input.Grab;
			case BirdTutorialGui.ButtonPrompt.Talk:
				return Input.Talk;
			default:
				return Input.Jump;
			}
		}

		// Token: 0x04001368 RID: 4968
		public Entity Entity;

		// Token: 0x04001369 RID: 4969
		public bool Open;

		// Token: 0x0400136A RID: 4970
		public float Scale;

		// Token: 0x0400136B RID: 4971
		private object info;

		// Token: 0x0400136C RID: 4972
		private List<object> controls;

		// Token: 0x0400136D RID: 4973
		private float controlsWidth;

		// Token: 0x0400136E RID: 4974
		private float infoWidth;

		// Token: 0x0400136F RID: 4975
		private float infoHeight;

		// Token: 0x04001370 RID: 4976
		private float buttonPadding = 8f;

		// Token: 0x04001371 RID: 4977
		private Color bgColor = Calc.HexToColor("061526");

		// Token: 0x04001372 RID: 4978
		private Color lineColor = new Color(1f, 1f, 1f);

		// Token: 0x04001373 RID: 4979
		private Color textColor = Calc.HexToColor("6179e2");

		// Token: 0x02000690 RID: 1680
		public enum ButtonPrompt
		{
			// Token: 0x04002B34 RID: 11060
			Dash,
			// Token: 0x04002B35 RID: 11061
			Jump,
			// Token: 0x04002B36 RID: 11062
			Grab,
			// Token: 0x04002B37 RID: 11063
			Talk
		}
	}
}
