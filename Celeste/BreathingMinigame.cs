using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000202 RID: 514
	public class BreathingMinigame : Entity
	{
		// Token: 0x17000138 RID: 312
		// (get) Token: 0x060010CD RID: 4301 RVA: 0x0004EC30 File Offset: 0x0004CE30
		private Vector2 screenCenter
		{
			get
			{
				return new Vector2(1920f, 1080f) / 2f;
			}
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x0004EC4C File Offset: 0x0004CE4C
		public BreathingMinigame(bool winnable = true, BreathingRumbler rumbler = null)
		{
			this.rumbler = rumbler;
			this.winnable = winnable;
			base.Tag = Tags.HUD;
			base.Depth = 100;
			base.Add(this.featherSprite = GFX.GuiSpriteBank.Create("feather"));
			this.featherSprite.Position = this.screenCenter + Vector2.UnitY * (this.feather + -128f);
			base.Add(new Coroutine(this.Routine(), true));
			base.Add(this.featherWave = new SineWave(0.25f, 0f));
			base.Add(new BeforeRenderHook(new Action(this.BeforeRender)));
			this.particles = new BreathingMinigame.Particle[50];
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.particles[i].Reset();
			}
			this.particleSpeed = 120f;
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x0004ED8F File Offset: 0x0004CF8F
		public IEnumerator Routine()
		{
			this.insideTargetTimer = 1f;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime)
			{
				yield return null;
				if (p > 1f)
				{
					p = 1f;
				}
				this.bgAlpha = p * 0.65f;
			}
			if (this.winnable)
			{
				yield return this.ShowText(1);
				yield return this.FadeGameIn();
				yield return this.ShowText(2);
				yield return this.ShowText(3);
				yield return this.ShowText(4);
				yield return this.ShowText(5);
			}
			else
			{
				yield return this.FadeGameIn();
			}
			base.Add(new Coroutine(this.FadeBoxIn(), true));
			float activeBounds = 450f;
			while (this.stablizedTimer < 30f)
			{
				float num = this.stablizedTimer / 30f;
				bool flag = Input.Jump.Check || Input.Dash.Check || Input.Aim.Value.Y < 0f;
				if (this.winnable)
				{
					Audio.SetMusicParam("calm", num);
					Audio.SetMusicParam("gondola_idle", num);
				}
				else
				{
					Level level = base.Scene as Level;
					if (!this.losing)
					{
						float num2 = num / 0.4f;
						level.Session.Audio.Music.Layer(1, num2);
						level.Session.Audio.Music.Layer(3, 1f - num2);
						level.Session.Audio.Apply(false);
					}
					else
					{
						level.Session.Audio.Music.Layer(1, 1f - this.losingTimer);
						level.Session.Audio.Music.Layer(3, this.losingTimer);
						level.Session.Audio.Apply(false);
					}
				}
				if (!this.winnable && this.losing)
				{
					if (Calc.BetweenInterval(this.losingTimer * 10f, 0.5f))
					{
						flag = !flag;
					}
					activeBounds = 450f - Ease.CubeIn(this.losingTimer) * 200f;
				}
				if (flag)
				{
					if (this.feather > -activeBounds)
					{
						this.speed -= 280f * Engine.DeltaTime;
					}
					this.particleSpeed -= 2800f * Engine.DeltaTime;
				}
				else
				{
					if (this.feather < activeBounds)
					{
						this.speed += 280f * Engine.DeltaTime;
					}
					this.particleSpeed += 2800f * Engine.DeltaTime;
				}
				this.speed = Calc.Clamp(this.speed, -200f, 200f);
				if (this.feather > activeBounds && this.speedMultiplier == 0f && this.speed > 0f)
				{
					this.speed = 0f;
				}
				if (this.feather < activeBounds && this.speedMultiplier == 0f && this.speed < 0f)
				{
					this.speed = 0f;
				}
				this.particleSpeed = Calc.Clamp(this.particleSpeed, -1600f, 120f);
				this.speedMultiplier = Calc.Approach(this.speedMultiplier, (float)(((this.feather < -activeBounds && this.speed < 0f) || (this.feather > activeBounds && this.speed > 0f)) ? 0 : 1), Engine.DeltaTime * 4f);
				this.currentTargetBounds = Calc.Approach(this.currentTargetBounds, 160f + -60f * num, Engine.DeltaTime * 16f);
				this.feather += this.speed * this.speedMultiplier * Engine.DeltaTime;
				if (this.boxEnabled)
				{
					this.currentTargetCenter = -this.sine.Value * 300f * MathHelper.Lerp(1f, 0f, Ease.CubeIn(num));
					float num3 = this.currentTargetCenter - this.currentTargetBounds;
					float num4 = this.currentTargetCenter + this.currentTargetBounds;
					if (this.feather > num3 && this.feather < num4)
					{
						this.insideTargetTimer += Engine.DeltaTime;
						if (this.insideTargetTimer > 0.2f)
						{
							this.stablizedTimer += Engine.DeltaTime;
						}
						if (this.rumbler != null)
						{
							this.rumbler.Strength = 0.3f * (1f - num);
						}
					}
					else
					{
						if (this.insideTargetTimer > 0.2f)
						{
							this.stablizedTimer = Math.Max(0f, this.stablizedTimer - 0.5f);
						}
						if (this.stablizedTimer > 0f)
						{
							this.stablizedTimer -= 0.5f * Engine.DeltaTime;
						}
						this.insideTargetTimer = 0f;
						if (this.rumbler != null)
						{
							this.rumbler.Strength = 0.5f * (1f - num);
						}
					}
				}
				else if (this.rumbler != null)
				{
					this.rumbler.Strength = 0.2f;
				}
				float target = 0.65f + Math.Min(1f, num / 0.8f) * 0.35000002f;
				this.bgAlpha = Calc.Approach(this.bgAlpha, target, Engine.DeltaTime);
				this.featherSprite.Position = this.screenCenter + Vector2.UnitY * (this.feather + -128f);
				this.featherSprite.Play((this.insideTargetTimer > 0f || !this.boxEnabled) ? "hover" : "flutter", false, false);
				this.particleAlpha = Calc.Approach(this.particleAlpha, 1f, Engine.DeltaTime);
				if (!this.winnable && this.stablizedTimer > 12f)
				{
					this.losing = true;
				}
				if (this.losing)
				{
					this.losingTimer += Engine.DeltaTime / 5f;
					if (this.losingTimer > 1f)
					{
						break;
					}
				}
				yield return null;
			}
			if (!this.winnable)
			{
				this.Pausing = true;
				while (this.Pausing)
				{
					if (this.rumbler != null)
					{
						this.rumbler.Strength = Calc.Approach(this.rumbler.Strength, 1f, 2f * Engine.DeltaTime);
					}
					this.featherSprite.Position += (this.screenCenter - this.featherSprite.Position) * (1f - (float)Math.Pow(0.009999999776482582, (double)Engine.DeltaTime));
					this.boxAlpha -= Engine.DeltaTime * 10f;
					this.particleAlpha = this.boxAlpha;
					yield return null;
				}
				this.losing = false;
				this.losingTimer = 0f;
				yield return this.PopFeather();
			}
			else
			{
				this.bgAlpha = 1f;
				if (this.rumbler != null)
				{
					this.rumbler.RemoveSelf();
					this.rumbler = null;
				}
				while (this.boxAlpha > 0f)
				{
					yield return null;
					this.boxAlpha -= Engine.DeltaTime;
					this.particleAlpha = this.boxAlpha;
				}
				this.particleAlpha = 0f;
				yield return 2f;
				while (this.featherAlpha > 0f)
				{
					yield return null;
					this.featherAlpha -= Engine.DeltaTime;
				}
				yield return 1f;
			}
			this.Completed = true;
			while (this.bgAlpha > 0f)
			{
				yield return null;
				this.bgAlpha -= Engine.DeltaTime * (this.winnable ? 1f : 10f);
			}
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x0004ED9E File Offset: 0x0004CF9E
		private IEnumerator ShowText(int num)
		{
			yield return this.FadeTextTo(0f);
			this.text = Dialog.Clean("CH4_GONDOLA_FEATHER_" + num, null);
			yield return 0.1f;
			yield return this.FadeTextTo(1f);
			while (!Input.MenuConfirm.Pressed)
			{
				yield return null;
			}
			yield return this.FadeTextTo(0f);
			yield break;
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x0004EDB4 File Offset: 0x0004CFB4
		private IEnumerator FadeGameIn()
		{
			while (this.featherAlpha < 1f)
			{
				this.featherAlpha += Engine.DeltaTime;
				yield return null;
			}
			this.featherAlpha = 1f;
			yield break;
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x0004EDC3 File Offset: 0x0004CFC3
		private IEnumerator FadeBoxIn()
		{
			yield return this.winnable ? 5f : 2f;
			while (Math.Abs(this.feather) > 300f)
			{
				yield return null;
			}
			this.boxEnabled = true;
			base.Add(this.sine = new SineWave(0.12f, 0f));
			while (this.boxAlpha < 1f)
			{
				this.boxAlpha += Engine.DeltaTime;
				yield return null;
			}
			this.boxAlpha = 1f;
			yield break;
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x0004EDD2 File Offset: 0x0004CFD2
		private IEnumerator FadeTextTo(float v)
		{
			if (this.textAlpha == v)
			{
				yield break;
			}
			float from = this.textAlpha;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
			{
				yield return null;
				this.textAlpha = from + (v - from) * p;
			}
			this.textAlpha = v;
			yield break;
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x0004EDE8 File Offset: 0x0004CFE8
		private IEnumerator PopFeather()
		{
			Audio.Play("event:/game/06_reflection/badeline_feather_slice");
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			if (this.rumbler != null)
			{
				this.rumbler.RemoveSelf();
				this.rumbler = null;
			}
			this.featherSprite.Rotation = 0f;
			this.featherSprite.Play("hover", false, false);
			this.featherSprite.CenterOrigin();
			this.featherSprite.Y += this.featherSprite.Height / 2f;
			yield return 0.25f;
			this.featherSlice = new Image(GFX.Gui["feather/slice"]);
			this.featherSlice.CenterOrigin();
			this.featherSlice.Position = this.featherSprite.Position;
			this.featherSlice.Rotation = Calc.Angle(new Vector2(96f, 165f), new Vector2(140f, 112f));
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 8f)
			{
				this.featherSlice.Scale.X = (0.25f + Calc.YoYo(p) * 0.75f) * 8f;
				this.featherSlice.Scale.Y = (0.5f + (1f - Calc.YoYo(p)) * 0.5f) * 8f;
				this.featherSlice.Position = this.featherSprite.Position + Vector2.Lerp(new Vector2(128f, -128f), new Vector2(-128f, 128f), p);
				yield return null;
			}
			this.featherSlice.Visible = false;
			(base.Scene as Level).Shake(0.3f);
			(base.Scene as Level).Flash(Color.White, false);
			this.featherSprite.Visible = false;
			this.featherHalfLeft = new Image(GFX.Gui["feather/feather_half0"]);
			this.featherHalfLeft.CenterOrigin();
			this.featherHalfRight = new Image(GFX.Gui["feather/feather_half1"]);
			this.featherHalfRight.CenterOrigin();
			for (float p = 0f; p < 1f; p += Engine.DeltaTime)
			{
				this.featherHalfLeft.Position = this.featherSprite.Position + Vector2.Lerp(Vector2.Zero, new Vector2(-128f, -32f), p);
				this.featherHalfRight.Position = this.featherSprite.Position + Vector2.Lerp(Vector2.Zero, new Vector2(128f, 32f), p);
				this.featherAlpha = 1f - p;
				yield return null;
			}
			yield break;
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x0004EDF8 File Offset: 0x0004CFF8
		public override void Update()
		{
			this.timer += Engine.DeltaTime;
			this.trailSpeed = Calc.Approach(this.trailSpeed, this.speed, Engine.DeltaTime * 200f * 8f);
			if (this.featherWave != null)
			{
				this.featherSprite.Rotation = this.featherWave.Value * 0.25f + 0.1f;
			}
			for (int i = 0; i < this.particles.Length; i++)
			{
				BreathingMinigame.Particle[] array = this.particles;
				int num = i;
				array[num].Position.Y = array[num].Position.Y + this.particles[i].Speed * this.particleSpeed * Engine.DeltaTime;
				if (this.particleSpeed > -400f)
				{
					BreathingMinigame.Particle[] array2 = this.particles;
					int num2 = i;
					array2[num2].Position.X = array2[num2].Position.X + (this.particleSpeed + 400f) * (float)Math.Sin((double)this.particles[i].Sin) * 0.1f * Engine.DeltaTime;
				}
				BreathingMinigame.Particle[] array3 = this.particles;
				int num3 = i;
				array3[num3].Sin = array3[num3].Sin + Engine.DeltaTime;
				if (this.particles[i].Position.Y < -128f || this.particles[i].Position.Y > 1208f)
				{
					this.particles[i].Reset();
					if (this.particleSpeed < 0f)
					{
						this.particles[i].Position.Y = 1208f;
					}
					else
					{
						this.particles[i].Position.Y = -128f;
					}
				}
			}
			base.Update();
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x0004EFC4 File Offset: 0x0004D1C4
		public void BeforeRender()
		{
			if (this.featherBuffer == null)
			{
				int num = Math.Min(1920, Engine.ViewWidth);
				int num2 = Math.Min(1080, Engine.ViewHeight);
				this.featherBuffer = VirtualContent.CreateRenderTarget("breathing-minigame-a", num, num2, false, true, 0);
				this.smokeBuffer = VirtualContent.CreateRenderTarget("breathing-minigame-b", num / 2, num2 / 2, false, true, 0);
				this.tempBuffer = VirtualContent.CreateRenderTarget("breathing-minigame-c", num / 2, num2 / 2, false, true, 0);
			}
			Engine.Graphics.GraphicsDevice.SetRenderTarget(this.featherBuffer.Target);
			Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
			Matrix transformMatrix = Matrix.CreateScale((float)this.featherBuffer.Width / 1920f);
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, transformMatrix);
			if (this.losing)
			{
				this.featherSprite.Position += new Vector2((float)Calc.Random.Range(-1, 1), (float)Calc.Random.Range(-1, 1)).SafeNormalize() * this.losingTimer * 10f;
				this.featherSprite.Rotation += (float)Calc.Random.Range(-1, 1) * this.losingTimer * 0.1f;
			}
			this.featherSprite.Color = Color.White * this.featherAlpha;
			if (this.featherSprite.Visible)
			{
				this.featherSprite.Render();
			}
			if (this.featherSlice != null && this.featherSlice.Visible)
			{
				this.featherSlice.Render();
			}
			if (this.featherHalfLeft != null && this.featherHalfLeft.Visible)
			{
				this.featherHalfLeft.Color = Color.White * this.featherAlpha;
				this.featherHalfRight.Color = Color.White * this.featherAlpha;
				this.featherHalfLeft.Render();
				this.featherHalfRight.Render();
			}
			Draw.SpriteBatch.End();
			Engine.Graphics.GraphicsDevice.SetRenderTarget(this.smokeBuffer.Target);
			Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
			MagicGlow.Render(this.featherBuffer.Target, this.timer, -this.trailSpeed / 200f * 2f, Matrix.CreateScale(0.5f));
			GaussianBlur.Blur(this.smokeBuffer.Target, this.tempBuffer, this.smokeBuffer, 0f, true, GaussianBlur.Samples.Nine, 1f, GaussianBlur.Direction.Both, 1f);
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x0004F270 File Offset: 0x0004D470
		public override void Render()
		{
			Color value = (this.insideTargetTimer > 0.2f) ? Color.White : (Color.White * 0.6f);
			Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.bgAlpha);
			Level level = base.Scene as Level;
			if (level != null && (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene))
			{
				return;
			}
			MTexture mtexture = GFX.Gui["feather/border"];
			MTexture mtexture2 = GFX.Gui["feather/box"];
			Vector2 scale = new Vector2(((float)mtexture.Width * 2f - 32f) / (float)mtexture2.Width, (this.currentTargetBounds * 2f - 32f) / (float)mtexture2.Height);
			mtexture2.DrawCentered(this.screenCenter + new Vector2(0f, this.currentTargetCenter), Color.White * this.boxAlpha * 0.25f, scale);
			mtexture.Draw(this.screenCenter + new Vector2((float)(-(float)mtexture.Width), this.currentTargetCenter - this.currentTargetBounds), Vector2.Zero, value * this.boxAlpha, Vector2.One);
			mtexture.Draw(this.screenCenter + new Vector2((float)mtexture.Width, this.currentTargetCenter + this.currentTargetBounds), Vector2.Zero, value * this.boxAlpha, new Vector2(-1f, -1f));
			if (this.featherBuffer != null && !this.featherBuffer.IsDisposed)
			{
				float num = 1920f / (float)this.featherBuffer.Width;
				Draw.SpriteBatch.Draw(this.smokeBuffer.Target, Vector2.Zero, new Rectangle?(this.smokeBuffer.Bounds), Color.White * 0.3f, 0f, Vector2.Zero, num * 2f, SpriteEffects.None, 0f);
				Draw.SpriteBatch.Draw(this.featherBuffer.Target, Vector2.Zero, new Rectangle?(this.featherBuffer.Bounds), Color.White, 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
			}
			Vector2 value2 = new Vector2(1f, 1f);
			if (this.particleSpeed < 0f)
			{
				value2 = new Vector2(Math.Min(1f, 1f / (-this.particleSpeed * 0.004f)), Math.Max(1f, 1f * -this.particleSpeed * 0.004f));
			}
			for (int i = 0; i < this.particles.Length; i++)
			{
				Vector2 position = this.particles[i].Position;
				Vector2 scale2 = this.particles[i].Scale * value2;
				this.particleTexture.DrawCentered(position, Color.White * (0.5f * this.particleAlpha), scale2);
			}
			if (!string.IsNullOrEmpty(this.text) && this.textAlpha > 0f)
			{
				ActiveFont.Draw(this.text, new Vector2(960f, 920f), new Vector2(0.5f, 0.5f), Vector2.One, Color.White * this.textAlpha);
			}
			if (!string.IsNullOrEmpty(this.text) && this.textAlpha >= 1f)
			{
				Vector2 vector = ActiveFont.Measure(this.text);
				Vector2 position2 = new Vector2((1920f + vector.X) / 2f + 40f, 920f + vector.Y / 2f - 16f) + new Vector2(0f, (float)((this.timer % 1f < 0.25f) ? 6 : 0));
				GFX.Gui["textboxbutton"].DrawCentered(position2);
			}
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x0004F69C File Offset: 0x0004D89C
		public override void Removed(Scene scene)
		{
			this.Dispose();
			base.Removed(scene);
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x0004F6AB File Offset: 0x0004D8AB
		public override void SceneEnd(Scene scene)
		{
			this.Dispose();
			base.SceneEnd(scene);
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x0004F6BC File Offset: 0x0004D8BC
		private void Dispose()
		{
			if (this.featherBuffer != null && !this.featherBuffer.IsDisposed)
			{
				this.featherBuffer.Dispose();
				this.featherBuffer = null;
				this.smokeBuffer.Dispose();
				this.smokeBuffer = null;
				this.tempBuffer.Dispose();
				this.tempBuffer = null;
			}
		}

		// Token: 0x04000C46 RID: 3142
		private const float StablizeDuration = 30f;

		// Token: 0x04000C47 RID: 3143
		private const float StablizeLossRate = 0.5f;

		// Token: 0x04000C48 RID: 3144
		private const float StablizeIncreaseDelay = 0.2f;

		// Token: 0x04000C49 RID: 3145
		private const float StablizeLossPenalty = 0.5f;

		// Token: 0x04000C4A RID: 3146
		private const float Acceleration = 280f;

		// Token: 0x04000C4B RID: 3147
		private const float Gravity = 280f;

		// Token: 0x04000C4C RID: 3148
		private const float Maxspeed = 200f;

		// Token: 0x04000C4D RID: 3149
		private const float Bounds = 450f;

		// Token: 0x04000C4E RID: 3150
		private const float BGFadeStart = 0.65f;

		// Token: 0x04000C4F RID: 3151
		private const float featherSpriteOffset = -128f;

		// Token: 0x04000C50 RID: 3152
		private const float FadeBoxInMargin = 300f;

		// Token: 0x04000C51 RID: 3153
		private const float TargetSineAmplitude = 300f;

		// Token: 0x04000C52 RID: 3154
		private const float TargetSineFreq = 0.25f;

		// Token: 0x04000C53 RID: 3155
		private const float TargetBoundsAtStart = 160f;

		// Token: 0x04000C54 RID: 3156
		private const float TargetBoundsAtEnd = 100f;

		// Token: 0x04000C55 RID: 3157
		public const float MaxRumble = 0.5f;

		// Token: 0x04000C56 RID: 3158
		private const float PercentBeforeStartLosing = 0.4f;

		// Token: 0x04000C57 RID: 3159
		private const float LoseDuration = 5f;

		// Token: 0x04000C58 RID: 3160
		public bool Completed;

		// Token: 0x04000C59 RID: 3161
		public bool Pausing;

		// Token: 0x04000C5A RID: 3162
		private bool winnable;

		// Token: 0x04000C5B RID: 3163
		private float boxAlpha;

		// Token: 0x04000C5C RID: 3164
		private float featherAlpha;

		// Token: 0x04000C5D RID: 3165
		private float bgAlpha;

		// Token: 0x04000C5E RID: 3166
		private float feather;

		// Token: 0x04000C5F RID: 3167
		private float speed;

		// Token: 0x04000C60 RID: 3168
		private float stablizedTimer;

		// Token: 0x04000C61 RID: 3169
		private float currentTargetBounds = 160f;

		// Token: 0x04000C62 RID: 3170
		private float currentTargetCenter;

		// Token: 0x04000C63 RID: 3171
		private float speedMultiplier = 1f;

		// Token: 0x04000C64 RID: 3172
		private float insideTargetTimer;

		// Token: 0x04000C65 RID: 3173
		private bool boxEnabled;

		// Token: 0x04000C66 RID: 3174
		private float trailSpeed;

		// Token: 0x04000C67 RID: 3175
		private bool losing;

		// Token: 0x04000C68 RID: 3176
		private float losingTimer;

		// Token: 0x04000C69 RID: 3177
		private Sprite featherSprite;

		// Token: 0x04000C6A RID: 3178
		private Image featherSlice;

		// Token: 0x04000C6B RID: 3179
		private Image featherHalfLeft;

		// Token: 0x04000C6C RID: 3180
		private Image featherHalfRight;

		// Token: 0x04000C6D RID: 3181
		private SineWave sine;

		// Token: 0x04000C6E RID: 3182
		private SineWave featherWave;

		// Token: 0x04000C6F RID: 3183
		private BreathingRumbler rumbler;

		// Token: 0x04000C70 RID: 3184
		private string text;

		// Token: 0x04000C71 RID: 3185
		private float textAlpha;

		// Token: 0x04000C72 RID: 3186
		private VirtualRenderTarget featherBuffer;

		// Token: 0x04000C73 RID: 3187
		private VirtualRenderTarget smokeBuffer;

		// Token: 0x04000C74 RID: 3188
		private VirtualRenderTarget tempBuffer;

		// Token: 0x04000C75 RID: 3189
		private float timer;

		// Token: 0x04000C76 RID: 3190
		private BreathingMinigame.Particle[] particles;

		// Token: 0x04000C77 RID: 3191
		private MTexture particleTexture = OVR.Atlas["snow"].GetSubtexture(1, 1, 254, 254, null);

		// Token: 0x04000C78 RID: 3192
		private float particleSpeed;

		// Token: 0x04000C79 RID: 3193
		private float particleAlpha;

		// Token: 0x02000503 RID: 1283
		private struct Particle
		{
			// Token: 0x060024DC RID: 9436 RVA: 0x000F58BC File Offset: 0x000F3ABC
			public void Reset()
			{
				float num = Calc.Random.NextFloat();
				num *= num * num * num;
				this.Position = new Vector2(Calc.Random.NextFloat() * 1920f, Calc.Random.NextFloat() * 1080f);
				this.Scale = Calc.Map(num, 0f, 1f, 0.05f, 0.8f);
				this.Speed = this.Scale * Calc.Random.Range(2f, 8f);
				this.Sin = Calc.Random.NextFloat(6.2831855f);
			}

			// Token: 0x040024A6 RID: 9382
			public Vector2 Position;

			// Token: 0x040024A7 RID: 9383
			public float Speed;

			// Token: 0x040024A8 RID: 9384
			public float Scale;

			// Token: 0x040024A9 RID: 9385
			public float Sin;
		}
	}
}
