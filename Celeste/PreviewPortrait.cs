using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x020002F6 RID: 758
	public class PreviewPortrait : Scene
	{
		// Token: 0x0600176C RID: 5996 RVA: 0x0008F1D0 File Offset: 0x0008D3D0
		public PreviewPortrait(float scroll = 64f)
		{
			foreach (KeyValuePair<string, SpriteData> keyValuePair in GFX.PortraitsSpriteBank.SpriteData)
			{
				if (keyValuePair.Key.StartsWith("portrait"))
				{
					this.options.Add(keyValuePair.Key);
				}
			}
			this.topleft.Y = scroll;
		}

		// Token: 0x0600176D RID: 5997 RVA: 0x0008F284 File Offset: 0x0008D484
		public override void Update()
		{
			if (this.animation != null)
			{
				this.animation.Update();
				if (MInput.Mouse.PressedLeftButton)
				{
					int i = 0;
					while (i < this.animations.Count)
					{
						if (this.MouseOverOption(i))
						{
							if (i == 0)
							{
								this.animation = null;
								break;
							}
							this.animation.Play(this.animations[i], false, false);
							break;
						}
						else
						{
							i++;
						}
					}
				}
			}
			else if (MInput.Mouse.PressedLeftButton)
			{
				for (int j = 0; j < this.options.Count; j++)
				{
					if (this.MouseOverOption(j))
					{
						this.currentPortrait = this.options[j].Split(new char[]
						{
							'_'
						})[1];
						this.animation = GFX.PortraitsSpriteBank.Create(this.options[j]);
						this.animations.Clear();
						this.animations.Add("<-BACK");
						XmlElement xml = GFX.PortraitsSpriteBank.SpriteData[this.options[j]].Sources[0].XML;
						foreach (object obj in xml.GetElementsByTagName("Anim"))
						{
							XmlElement xml2 = (XmlElement)obj;
							this.animations.Add(xml2.Attr("id"));
						}
						using (IEnumerator enumerator = xml.GetElementsByTagName("Loop").GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								object obj2 = enumerator.Current;
								XmlElement xml3 = (XmlElement)obj2;
								this.animations.Add(xml3.Attr("id"));
							}
							break;
						}
					}
				}
			}
			this.topleft.Y = this.topleft.Y + (float)MInput.Mouse.WheelDelta * Engine.DeltaTime * ActiveFont.LineHeight;
			if (MInput.Keyboard.Pressed(Keys.F1))
			{
				Celeste.ReloadPortraits();
				Engine.Scene = new PreviewPortrait(this.topleft.Y);
			}
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x0600176E RID: 5998 RVA: 0x0008F4D8 File Offset: 0x0008D6D8
		public Vector2 Mouse
		{
			get
			{
				return Vector2.Transform(new Vector2((float)MInput.Mouse.CurrentState.X, (float)MInput.Mouse.CurrentState.Y), Matrix.Invert(Engine.ScreenMatrix));
			}
		}

		// Token: 0x0600176F RID: 5999 RVA: 0x0008F510 File Offset: 0x0008D710
		public override void Render()
		{
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, RasterizerState.CullNone, null, Engine.ScreenMatrix);
			Draw.Rect(0f, 0f, 960f, 1080f, Color.DarkSlateGray * 0.25f);
			if (this.animation != null)
			{
				this.animation.Scale = Vector2.One;
				this.animation.Position = new Vector2(1440f, 540f);
				this.animation.Render();
				int num = 0;
				foreach (string text in this.animations)
				{
					Color color = Color.Gray;
					if (this.MouseOverOption(num))
					{
						color = Color.White;
					}
					else if (this.animation.CurrentAnimationID == text)
					{
						color = Color.Yellow;
					}
					ActiveFont.Draw(text, this.topleft + new Vector2(0f, (float)num * ActiveFont.LineHeight), color);
					num++;
				}
				if (!string.IsNullOrEmpty(this.animation.CurrentAnimationID))
				{
					string[] array = this.animation.CurrentAnimationID.Split(new char[]
					{
						'_'
					});
					if (array.Length > 1)
					{
						ActiveFont.Draw(this.currentPortrait + " " + array[1], new Vector2(1440f, 1016f), new Vector2(0.5f, 1f), Vector2.One, Color.White);
					}
				}
			}
			else
			{
				int num2 = 0;
				foreach (string text2 in this.options)
				{
					ActiveFont.Draw(text2, this.topleft + new Vector2(0f, (float)num2 * ActiveFont.LineHeight), this.MouseOverOption(num2) ? Color.White : Color.Gray);
					num2++;
				}
			}
			Draw.Rect(this.Mouse.X - 12f, this.Mouse.Y - 4f, 24f, 8f, Color.Red);
			Draw.Rect(this.Mouse.X - 4f, this.Mouse.Y - 12f, 8f, 24f, Color.Red);
			Draw.SpriteBatch.End();
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x0008F7B0 File Offset: 0x0008D9B0
		private bool MouseOverOption(int i)
		{
			return this.Mouse.X > this.topleft.X && this.Mouse.Y > this.topleft.Y + (float)i * ActiveFont.LineHeight && MInput.Mouse.X < 960f && this.Mouse.Y < this.topleft.Y + (float)(i + 1) * ActiveFont.LineHeight;
		}

		// Token: 0x0400141B RID: 5147
		private Sprite animation;

		// Token: 0x0400141C RID: 5148
		private List<string> options = new List<string>();

		// Token: 0x0400141D RID: 5149
		private List<string> animations = new List<string>();

		// Token: 0x0400141E RID: 5150
		private Vector2 topleft = new Vector2(64f, 64f);

		// Token: 0x0400141F RID: 5151
		private string currentPortrait;
	}
}
