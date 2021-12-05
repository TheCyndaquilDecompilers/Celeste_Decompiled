using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001ED RID: 493
	public class HeightDisplay : Entity
	{
		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06001044 RID: 4164 RVA: 0x00046C38 File Offset: 0x00044E38
		private bool drawText
		{
			get
			{
				return this.index >= 0 && this.ease > 0f && !string.IsNullOrEmpty(this.text);
			}
		}

		// Token: 0x06001045 RID: 4165 RVA: 0x00046C60 File Offset: 0x00044E60
		public HeightDisplay(int index)
		{
			base.Tag = (Tags.HUD | Tags.Persistent);
			this.index = index;
			string name = "CH7_HEIGHT_" + ((index < 0) ? "START" : index.ToString());
			if (index >= 0 && Dialog.Has(name, null))
			{
				this.text = Dialog.Get(name, null);
				this.text = this.text.ToUpper();
				this.height = (index + 1) * 500;
				this.approach = (float)(index * 500);
				int num = this.text.IndexOf("{X}");
				this.leftText = this.text.Substring(0, num);
				this.leftSize = ActiveFont.Measure(this.leftText).X;
				this.rightText = this.text.Substring(num + 3);
				this.numberSize = ActiveFont.Measure(this.height.ToString()).X;
				this.rightSize = ActiveFont.Measure(this.rightText).X;
				this.size = ActiveFont.Measure(this.leftText + this.height + this.rightText);
			}
			base.Add(new Coroutine(this.Routine(), true));
		}

		// Token: 0x06001046 RID: 4166 RVA: 0x00046DE0 File Offset: 0x00044FE0
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.spawnedLevel = (scene as Level).Session.Level;
		}

		// Token: 0x06001047 RID: 4167 RVA: 0x00046DFF File Offset: 0x00044FFF
		private IEnumerator Routine()
		{
			Player player;
			for (;;)
			{
				player = base.Scene.Tracker.GetEntity<Player>();
				if (player != null && (base.Scene as Level).Session.Level != this.spawnedLevel)
				{
					break;
				}
				yield return null;
			}
			this.StepAudioProgression();
			this.easingCamera = false;
			yield return 0.1f;
			base.Add(new Coroutine(this.CameraUp(), true));
			if (!string.IsNullOrEmpty(this.text) && this.index >= 0)
			{
				Audio.Play("event:/game/07_summit/altitude_count");
			}
			while ((this.ease += Engine.DeltaTime / 0.15f) < 1f)
			{
				yield return null;
			}
			while (this.approach < (float)this.height && !player.OnGround(1))
			{
				yield return null;
			}
			this.approach = (float)this.height;
			this.pulse = 1f;
			while ((this.pulse -= Engine.DeltaTime * 4f) > 0f)
			{
				yield return null;
			}
			this.pulse = 0f;
			yield return 1f;
			while ((this.ease -= Engine.DeltaTime / 0.15f) > 0f)
			{
				yield return null;
			}
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x06001048 RID: 4168 RVA: 0x00046E0E File Offset: 0x0004500E
		private IEnumerator CameraUp()
		{
			this.easingCamera = true;
			Level level = base.Scene as Level;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 1.5f)
			{
				level.Camera.Y = (float)(level.Bounds.Bottom - 180) + 64f * (1f - Ease.CubeOut(p));
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001049 RID: 4169 RVA: 0x00046E20 File Offset: 0x00045020
		private void StepAudioProgression()
		{
			Session session = (base.Scene as Level).Session;
			if (!this.setAudioProgression && this.index >= 0 && session.Area.Mode == AreaMode.Normal)
			{
				this.setAudioProgression = true;
				int num = this.index + 1;
				if (num <= 5)
				{
					session.Audio.Music.Progress = num;
				}
				else
				{
					session.Audio.Music.Event = "event:/music/lvl7/final_ascent";
				}
				session.Audio.Apply(false);
			}
		}

		// Token: 0x0600104A RID: 4170 RVA: 0x00046EA4 File Offset: 0x000450A4
		public override void Update()
		{
			if (this.index >= 0 && this.ease > 0f)
			{
				if ((float)this.height - this.approach > 100f)
				{
					this.approach += 1000f * Engine.DeltaTime;
				}
				else if ((float)this.height - this.approach > 25f)
				{
					this.approach += 200f * Engine.DeltaTime;
				}
				else if ((float)this.height - this.approach > 5f)
				{
					this.approach += 50f * Engine.DeltaTime;
				}
				else if ((float)this.height - this.approach > 0f)
				{
					this.approach += 10f * Engine.DeltaTime;
				}
				else
				{
					this.approach = (float)this.height;
				}
			}
			Level level = base.Scene as Level;
			if (!this.easingCamera)
			{
				level.Camera.Y = (float)(level.Bounds.Bottom - 180 + 64);
			}
			base.Update();
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x00046FD8 File Offset: 0x000451D8
		public override void Render()
		{
			if (base.Scene.Paused)
			{
				return;
			}
			if (this.drawText)
			{
				Vector2 vector = new Vector2(1920f, 1080f) / 2f;
				float scaleFactor = 1.2f + this.pulse * 0.2f;
				Vector2 vector2 = this.size * scaleFactor;
				float num = Ease.SineInOut(this.ease);
				Vector2 vector3 = new Vector2(1f, num);
				Draw.Rect(vector.X - (vector2.X + 64f) * 0.5f * vector3.X, vector.Y - (vector2.Y + 32f) * 0.5f * vector3.Y, (vector2.X + 64f) * vector3.X, (vector2.Y + 32f) * vector3.Y, Color.Black);
				Vector2 vector4 = vector + new Vector2(-vector2.X * 0.5f, 0f);
				Vector2 scale = vector3 * scaleFactor;
				Color color = Color.White * num;
				ActiveFont.Draw(this.leftText, vector4, new Vector2(0f, 0.5f), scale, color);
				ActiveFont.Draw(this.rightText, vector4 + Vector2.UnitX * (this.leftSize + this.numberSize) * scaleFactor, new Vector2(0f, 0.5f), scale, color);
				ActiveFont.Draw(((int)this.approach).ToString(), vector4 + Vector2.UnitX * (this.leftSize + this.numberSize * 0.5f) * scaleFactor, new Vector2(0.5f, 0.5f), scale, color);
			}
		}

		// Token: 0x0600104C RID: 4172 RVA: 0x000471B4 File Offset: 0x000453B4
		public override void Removed(Scene scene)
		{
			this.StepAudioProgression();
			base.Removed(scene);
		}

		// Token: 0x04000B9D RID: 2973
		private int index;

		// Token: 0x04000B9E RID: 2974
		private string text = "";

		// Token: 0x04000B9F RID: 2975
		private string leftText = "";

		// Token: 0x04000BA0 RID: 2976
		private string rightText = "";

		// Token: 0x04000BA1 RID: 2977
		private float leftSize;

		// Token: 0x04000BA2 RID: 2978
		private float rightSize;

		// Token: 0x04000BA3 RID: 2979
		private float numberSize;

		// Token: 0x04000BA4 RID: 2980
		private Vector2 size;

		// Token: 0x04000BA5 RID: 2981
		private int height;

		// Token: 0x04000BA6 RID: 2982
		private float approach;

		// Token: 0x04000BA7 RID: 2983
		private float ease;

		// Token: 0x04000BA8 RID: 2984
		private float pulse;

		// Token: 0x04000BA9 RID: 2985
		private string spawnedLevel;

		// Token: 0x04000BAA RID: 2986
		private bool setAudioProgression;

		// Token: 0x04000BAB RID: 2987
		private bool easingCamera = true;
	}
}
