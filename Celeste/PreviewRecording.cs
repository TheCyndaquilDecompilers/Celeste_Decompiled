using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x02000243 RID: 579
	public class PreviewRecording : Scene
	{
		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06001256 RID: 4694 RVA: 0x0005FFFF File Offset: 0x0005E1FF
		private Matrix Matrix
		{
			get
			{
				return Matrix.CreateScale(1920f / this.ScreenWidth) * Engine.ScreenMatrix;
			}
		}

		// Token: 0x06001257 RID: 4695 RVA: 0x0006001C File Offset: 0x0005E21C
		public PreviewRecording(string filename)
		{
			this.Filename = filename;
			byte[] buffer = File.ReadAllBytes(filename);
			this.Timeline = PlaybackData.Import(buffer);
			float num = float.MaxValue;
			float num2 = float.MinValue;
			float num3 = float.MinValue;
			float num4 = float.MaxValue;
			foreach (Player.ChaserState chaserState in this.Timeline)
			{
				num = Math.Min(chaserState.Position.X, num);
				num2 = Math.Max(chaserState.Position.X, num2);
				num4 = Math.Min(chaserState.Position.Y, num4);
				num3 = Math.Max(chaserState.Position.Y, num3);
			}
			this.Width = (float)((int)(num2 - num));
			this.Height = (float)((int)(num3 - num4));
			base.Add(this.entity = new PlayerPlayback(new Vector2((this.ScreenWidth - this.Width) / 2f - num, (this.ScreenHeight - this.Height) / 2f - num4), PlayerSpriteMode.Madeline, this.Timeline));
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x00060164 File Offset: 0x0005E364
		public override void Update()
		{
			if (MInput.Keyboard.Check(Keys.A))
			{
				this.entity.TrimStart = Math.Max(0f, this.entity.TrimStart -= Engine.DeltaTime);
			}
			if (MInput.Keyboard.Check(Keys.D))
			{
				this.entity.TrimStart = Math.Min(this.entity.Duration, this.entity.TrimStart += Engine.DeltaTime);
			}
			if (MInput.Keyboard.Check(Keys.Left))
			{
				this.entity.TrimEnd = Math.Max(0f, this.entity.TrimEnd -= Engine.DeltaTime);
			}
			if (MInput.Keyboard.Check(Keys.Right))
			{
				this.entity.TrimEnd = Math.Min(this.entity.Duration, this.entity.TrimEnd += Engine.DeltaTime);
			}
			if (MInput.Keyboard.Check(Keys.LeftControl) && MInput.Keyboard.Pressed(Keys.S))
			{
				while (this.Timeline[0].TimeStamp < this.entity.TrimStart)
				{
					this.Timeline.RemoveAt(0);
				}
				while (this.Timeline[this.Timeline.Count - 1].TimeStamp > this.entity.TrimEnd)
				{
					this.Timeline.RemoveAt(this.Timeline.Count - 1);
				}
				PlaybackData.Export(this.Timeline, this.Filename);
				Engine.Scene = new PreviewRecording(this.Filename);
			}
			base.Update();
			this.entity.Hair.AfterUpdate();
		}

		// Token: 0x06001259 RID: 4697 RVA: 0x00060340 File Offset: 0x0005E540
		public override void Render()
		{
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, RasterizerState.CullNone, null, Engine.ScreenMatrix);
			ActiveFont.Draw("A/D:        Move Start Trim", new Vector2(8f, 8f), new Vector2(0f, 0f), Vector2.One * 0.5f, Color.White);
			ActiveFont.Draw("Left/Right: Move End Trim", new Vector2(8f, 32f), new Vector2(0f, 0f), Vector2.One * 0.5f, Color.White);
			ActiveFont.Draw("CTRL+S: Save New Trim", new Vector2(8f, 56f), new Vector2(0f, 0f), Vector2.One * 0.5f, Color.White);
			Draw.SpriteBatch.End();
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, RasterizerState.CullNone, null, this.Matrix);
			Draw.HollowRect((this.ScreenWidth - this.Width) / 2f - 16f, (this.ScreenHeight - this.Height) / 2f - 16f, this.Width + 32f, this.Height + 32f, Color.Red * 0.6f);
			Draw.HollowRect((this.ScreenWidth - 320f) / 2f, (this.ScreenHeight - 180f) / 2f, 320f, 180f, Color.White * 0.6f);
			if (this.entity.Visible)
			{
				this.entity.Render();
			}
			Draw.Rect(32f, this.ScreenHeight - 48f, this.ScreenWidth - 64f, 16f, Color.DarkGray);
			Draw.Rect(32f, this.ScreenHeight - 48f, (this.ScreenWidth - 64f) * (this.entity.Time / this.entity.Duration), 16f, Color.White);
			Draw.Rect(32f + (this.ScreenWidth - 64f) * (this.entity.Time / this.entity.Duration) - 2f, this.ScreenHeight - 48f, 4f, 16f, Color.LimeGreen);
			Draw.Rect(32f + (this.ScreenWidth - 64f) * (this.entity.TrimStart / this.entity.Duration) - 2f, this.ScreenHeight - 48f, 4f, 16f, Color.Red);
			Draw.Rect(32f + (this.ScreenWidth - 64f) * (this.entity.TrimEnd / this.entity.Duration) - 2f, this.ScreenHeight - 48f, 4f, 16f, Color.Red);
			Draw.SpriteBatch.End();
		}

		// Token: 0x04000E00 RID: 3584
		public string Filename;

		// Token: 0x04000E01 RID: 3585
		public List<Player.ChaserState> Timeline;

		// Token: 0x04000E02 RID: 3586
		public PlayerPlayback entity;

		// Token: 0x04000E03 RID: 3587
		public float ScreenWidth = 640f;

		// Token: 0x04000E04 RID: 3588
		public float ScreenHeight = 360f;

		// Token: 0x04000E05 RID: 3589
		public float Width;

		// Token: 0x04000E06 RID: 3590
		public float Height;
	}
}
