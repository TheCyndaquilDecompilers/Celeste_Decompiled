using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000348 RID: 840
	public class MemorialText : Entity
	{
		// Token: 0x06001A64 RID: 6756 RVA: 0x000A9AD0 File Offset: 0x000A7CD0
		public MemorialText(Memorial memorial, bool dreamy)
		{
			base.AddTag(Tags.HUD);
			base.AddTag(Tags.PauseUpdate);
			base.Add(this.textSfx = new SoundSource());
			this.Dreamy = dreamy;
			this.Memorial = memorial;
			this.message = Dialog.Clean("memorial", null);
			this.firstLineLength = this.CountToNewline(0);
			for (int i = 0; i < this.message.Length; i++)
			{
				float x = ActiveFont.Measure(this.message[i]).X;
				if (x > this.widestCharacter)
				{
					this.widestCharacter = x;
				}
			}
			this.widestCharacter *= 0.9f;
		}

		// Token: 0x06001A65 RID: 6757 RVA: 0x000A9B94 File Offset: 0x000A7D94
		public override void Update()
		{
			base.Update();
			if ((base.Scene as Level).Paused)
			{
				this.textSfx.Pause();
				return;
			}
			this.timer += Engine.DeltaTime;
			if (!this.Show)
			{
				this.alpha = Calc.Approach(this.alpha, 0f, Engine.DeltaTime);
				if (this.alpha <= 0f)
				{
					this.index = (float)this.firstLineLength;
				}
			}
			else
			{
				this.alpha = Calc.Approach(this.alpha, 1f, Engine.DeltaTime * 2f);
				if (this.alpha >= 1f)
				{
					this.index = Calc.Approach(this.index, (float)this.message.Length, 32f * Engine.DeltaTime);
				}
			}
			if (this.Show && this.alpha >= 1f && this.index < (float)this.message.Length)
			{
				if (!this.textSfxPlaying)
				{
					this.textSfxPlaying = true;
					this.textSfx.Play(this.Dreamy ? "event:/ui/game/memorial_dream_text_loop" : "event:/ui/game/memorial_text_loop", null, 0f);
					this.textSfx.Param("end", 0f);
				}
			}
			else if (this.textSfxPlaying)
			{
				this.textSfxPlaying = false;
				this.textSfx.Stop(true);
				this.textSfx.Param("end", 1f);
			}
			this.textSfx.Resume();
		}

		// Token: 0x06001A66 RID: 6758 RVA: 0x000A9D24 File Offset: 0x000A7F24
		private int CountToNewline(int start)
		{
			int num = start;
			while (num < this.message.Length && this.message[num] != '\n')
			{
				num++;
			}
			return num - start;
		}

		// Token: 0x06001A67 RID: 6759 RVA: 0x000A9D5C File Offset: 0x000A7F5C
		public override void Render()
		{
			if ((base.Scene as Level).FrozenOrPaused || (base.Scene as Level).Completed)
			{
				return;
			}
			if (this.index > 0f && this.alpha > 0f)
			{
				Camera camera = base.SceneAs<Level>().Camera;
				Vector2 vector = new Vector2((this.Memorial.X - camera.X) * 6f, (this.Memorial.Y - camera.Y) * 6f - 350f - ActiveFont.LineHeight * 3.3f);
				if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
				{
					vector.X = 1920f - vector.X;
				}
				float num = Ease.CubeInOut(this.alpha);
				int num2 = (int)Math.Min((float)this.message.Length, this.index);
				int num3 = 0;
				float num4 = 64f * (1f - num);
				int num5 = this.CountToNewline(0);
				for (int i = 0; i < num2; i++)
				{
					char c = this.message[i];
					if (c == '\n')
					{
						num3 = 0;
						num5 = this.CountToNewline(i + 1);
						num4 += ActiveFont.LineHeight * 1.1f;
					}
					else
					{
						float x = 1f;
						float x2 = (float)(-(float)num5) * this.widestCharacter / 2f + ((float)num3 + 0.5f) * this.widestCharacter;
						float num6 = 0f;
						if (this.Dreamy && c != ' ' && c != '-' && c != '\n')
						{
							c = this.message[(i + (int)(Math.Sin((double)(this.timer * 2f + (float)i / 8f)) * 4.0) + this.message.Length) % this.message.Length];
							num6 = (float)Math.Sin((double)(this.timer * 2f + (float)i / 8f)) * 8f;
							x = (float)((Math.Sin((double)(this.timer * 4f + (float)i / 16f)) < 0.0) ? -1 : 1);
						}
						ActiveFont.Draw(c, vector + new Vector2(x2, num4 + num6), new Vector2(0.5f, 1f), new Vector2(x, 1f), Color.White * num);
						num3++;
					}
				}
			}
		}

		// Token: 0x040016FA RID: 5882
		public bool Show;

		// Token: 0x040016FB RID: 5883
		public bool Dreamy;

		// Token: 0x040016FC RID: 5884
		public Memorial Memorial;

		// Token: 0x040016FD RID: 5885
		private float index;

		// Token: 0x040016FE RID: 5886
		private string message;

		// Token: 0x040016FF RID: 5887
		private float alpha;

		// Token: 0x04001700 RID: 5888
		private float timer;

		// Token: 0x04001701 RID: 5889
		private float widestCharacter;

		// Token: 0x04001702 RID: 5890
		private int firstLineLength;

		// Token: 0x04001703 RID: 5891
		private SoundSource textSfx;

		// Token: 0x04001704 RID: 5892
		private bool textSfxPlaying;
	}
}
