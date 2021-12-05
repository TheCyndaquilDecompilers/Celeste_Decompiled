using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020002A3 RID: 675
	public class IntroVignette : Scene
	{
		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060014E6 RID: 5350 RVA: 0x00074E5D File Offset: 0x0007305D
		public bool CanPause
		{
			get
			{
				return this.menu == null;
			}
		}

		// Token: 0x060014E7 RID: 5351 RVA: 0x00074E68 File Offset: 0x00073068
		public IntroVignette(Session session, HiresSnow snow = null)
		{
			this.session = session;
			this.areaMusic = session.Audio.Music.Event;
			session.Audio.Music.Event = null;
			session.Audio.Apply(false);
			this.sfx = Audio.Play("event:/game/00_prologue/intro_vignette");
			if (snow == null)
			{
				this.fade = 1f;
				snow = new HiresSnow(0.45f);
			}
			base.Add(this.hud = new HudRenderer());
			base.Add(this.snow = snow);
			base.RendererList.UpdateLists();
			this.text = FancyText.Parse(Dialog.Get("CH0_INTRO", null), 960, 8, 0f, null, null);
			this.textCoroutine = new Coroutine(this.TextSequence(), true);
		}

		// Token: 0x060014E8 RID: 5352 RVA: 0x00074F4B File Offset: 0x0007314B
		private IEnumerator TextSequence()
		{
			yield return 3f;
			while (this.textStart < this.text.Count)
			{
				this.textAlpha = 1f;
				int charactersOnPage = this.text.GetCharactersOnPage(this.textStart);
				float fadeTimePerCharacter = 1f / (float)charactersOnPage;
				int i = this.textStart;
				while (i < this.text.Count && !(this.text[i] is FancyText.NewPage))
				{
					FancyText.Char c = this.text[i] as FancyText.Char;
					if (c != null)
					{
						while ((c.Fade += Engine.DeltaTime / fadeTimePerCharacter) < 1f)
						{
							yield return null;
						}
						c.Fade = 1f;
						c = null;
					}
					int num = i;
					i = num + 1;
				}
				yield return 2.5f;
				while (this.textAlpha > 0f)
				{
					this.textAlpha -= 1f * Engine.DeltaTime;
					yield return null;
				}
				this.textAlpha = 0f;
				this.textStart = this.text.GetNextPageStart(this.textStart);
				yield return 0.5f;
			}
			this.textStart = int.MaxValue;
			yield break;
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x00074F5C File Offset: 0x0007315C
		public override void Update()
		{
			if (this.menu == null)
			{
				base.Update();
				if (!this.exiting)
				{
					if (this.textCoroutine != null && this.textCoroutine.Active)
					{
						this.textCoroutine.Update();
					}
					this.timer += Engine.DeltaTime;
					if (this.timer >= 18.683f && !this.started)
					{
						this.StartGame();
					}
					if (this.timer < 16.683f && this.menu == null && (Input.Pause.Pressed || Input.ESC.Pressed))
					{
						Input.Pause.ConsumeBuffer();
						Input.ESC.ConsumeBuffer();
						this.OpenMenu();
					}
				}
			}
			else if (!this.exiting)
			{
				this.menu.Update();
			}
			this.pauseFade = Calc.Approach(this.pauseFade, (float)((this.menu != null) ? 1 : 0), Engine.DeltaTime * 8f);
			this.hud.BackgroundFade = Calc.Approach(this.hud.BackgroundFade, (this.menu != null) ? 0.6f : 0f, Engine.DeltaTime * 3f);
			this.fade = Calc.Approach(this.fade, 0f, Engine.DeltaTime);
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x000750AC File Offset: 0x000732AC
		public void OpenMenu()
		{
			Audio.Play("event:/ui/game/pause");
			Audio.Pause(this.sfx);
			base.Add(this.menu = new TextMenu());
			this.menu.Add(new TextMenu.Button(Dialog.Clean("intro_vignette_resume", null)).Pressed(new Action(this.CloseMenu)));
			this.menu.Add(new TextMenu.Button(Dialog.Clean("intro_vignette_skip", null)).Pressed(new Action(this.StartGame)));
			this.menu.Add(new TextMenu.Button(Dialog.Clean("intro_vignette_quit", null)).Pressed(new Action(this.ReturnToMap)));
			this.menu.OnCancel = (this.menu.OnESC = (this.menu.OnPause = new Action(this.CloseMenu)));
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x0007519D File Offset: 0x0007339D
		private void CloseMenu()
		{
			Audio.Play("event:/ui/game/unpause");
			Audio.Resume(this.sfx);
			if (this.menu != null)
			{
				this.menu.RemoveSelf();
			}
			this.menu = null;
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x000751D0 File Offset: 0x000733D0
		private void StartGame()
		{
			this.textCoroutine = null;
			this.StopSfx();
			this.session.Audio.Music.Event = this.areaMusic;
			if (this.menu != null)
			{
				this.menu.RemoveSelf();
				this.menu = null;
			}
			new FadeWipe(this, false, delegate()
			{
				Engine.Scene = new LevelLoader(this.session, null);
			}).OnUpdate = delegate(float f)
			{
				this.textAlpha = Math.Min(this.textAlpha, 1f - f);
			};
			this.started = true;
			this.exiting = true;
		}

		// Token: 0x060014ED RID: 5357 RVA: 0x00075254 File Offset: 0x00073454
		private void ReturnToMap()
		{
			this.StopSfx();
			this.menu.RemoveSelf();
			this.menu = null;
			this.exiting = true;
			bool toAreaQuit = SaveData.Instance.Areas[0].Modes[0].Completed && Celeste.PlayMode != Celeste.PlayModes.Event;
			new FadeWipe(this, false, delegate()
			{
				if (toAreaQuit)
				{
					Engine.Scene = new OverworldLoader(Overworld.StartMode.AreaQuit, this.snow);
					return;
				}
				Engine.Scene = new OverworldLoader(Overworld.StartMode.Titlescreen, this.snow);
			}).OnUpdate = delegate(float f)
			{
				this.textAlpha = Math.Min(this.textAlpha, 1f - f);
			};
			base.RendererList.UpdateLists();
			base.RendererList.MoveToFront(this.snow);
		}

		// Token: 0x060014EE RID: 5358 RVA: 0x000752FF File Offset: 0x000734FF
		private void StopSfx()
		{
			Audio.Stop(this.sfx, false);
		}

		// Token: 0x060014EF RID: 5359 RVA: 0x0007530D File Offset: 0x0007350D
		public override void End()
		{
			this.StopSfx();
			base.End();
		}

		// Token: 0x060014F0 RID: 5360 RVA: 0x0007531C File Offset: 0x0007351C
		public override void Render()
		{
			base.Render();
			if (this.fade > 0f || this.textAlpha > 0f)
			{
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, RasterizerState.CullNone, null, Engine.ScreenMatrix);
				if (this.fade > 0f)
				{
					Draw.Rect(-1f, -1f, 1922f, 1082f, Color.Black * this.fade);
				}
				if (this.textStart < this.text.Nodes.Count && this.textAlpha > 0f)
				{
					this.text.Draw(new Vector2(1920f, 1080f) * 0.5f, new Vector2(0.5f, 0.5f), Vector2.One, this.textAlpha * (1f - this.pauseFade), this.textStart, int.MaxValue);
				}
				Draw.SpriteBatch.End();
			}
		}

		// Token: 0x040010BC RID: 4284
		private const float SFXDuration = 18.683f;

		// Token: 0x040010BD RID: 4285
		private Session session;

		// Token: 0x040010BE RID: 4286
		private bool started;

		// Token: 0x040010BF RID: 4287
		private float timer;

		// Token: 0x040010C0 RID: 4288
		private string areaMusic;

		// Token: 0x040010C1 RID: 4289
		private float fade;

		// Token: 0x040010C2 RID: 4290
		private EventInstance sfx;

		// Token: 0x040010C3 RID: 4291
		private TextMenu menu;

		// Token: 0x040010C4 RID: 4292
		private float pauseFade;

		// Token: 0x040010C5 RID: 4293
		private HudRenderer hud;

		// Token: 0x040010C6 RID: 4294
		private bool exiting;

		// Token: 0x040010C7 RID: 4295
		private Coroutine textCoroutine;

		// Token: 0x040010C8 RID: 4296
		private FancyText.Text text;

		// Token: 0x040010C9 RID: 4297
		private int textStart;

		// Token: 0x040010CA RID: 4298
		private float textAlpha;

		// Token: 0x040010CB RID: 4299
		private HiresSnow snow;
	}
}
