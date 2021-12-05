using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200024B RID: 587
	public class CoreVignette : Scene
	{
		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06001277 RID: 4727 RVA: 0x00061A06 File Offset: 0x0005FC06
		public bool CanPause
		{
			get
			{
				return this.menu == null;
			}
		}

		// Token: 0x06001278 RID: 4728 RVA: 0x00061A14 File Offset: 0x0005FC14
		public CoreVignette(Session session, HiresSnow snow = null)
		{
			this.session = session;
			if (snow == null)
			{
				snow = new HiresSnow(0.45f);
			}
			base.Add(this.hud = new HudRenderer());
			base.Add(this.snow = snow);
			base.RendererList.UpdateLists();
			this.text = FancyText.Parse(Dialog.Get("APP_INTRO", null), 960, 8, 0f, null, null);
			this.textCoroutine = new Coroutine(this.TextSequence(), true);
		}

		// Token: 0x06001279 RID: 4729 RVA: 0x00061AA9 File Offset: 0x0005FCA9
		private IEnumerator TextSequence()
		{
			yield return 1f;
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
			if (!this.started)
			{
				this.StartGame();
			}
			this.textStart = int.MaxValue;
			yield break;
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x00061AB8 File Offset: 0x0005FCB8
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
					if (this.menu == null && (Input.Pause.Pressed || Input.ESC.Pressed))
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

		// Token: 0x0600127B RID: 4731 RVA: 0x00061BC8 File Offset: 0x0005FDC8
		public void OpenMenu()
		{
			Audio.Play("event:/ui/game/pause");
			base.Add(this.menu = new TextMenu());
			this.menu.Add(new TextMenu.Button(Dialog.Clean("intro_vignette_resume", null)).Pressed(new Action(this.CloseMenu)));
			this.menu.Add(new TextMenu.Button(Dialog.Clean("intro_vignette_skip", null)).Pressed(new Action(this.StartGame)));
			this.menu.Add(new TextMenu.Button(Dialog.Clean("intro_vignette_quit", null)).Pressed(new Action(this.ReturnToMap)));
			this.menu.OnCancel = (this.menu.OnESC = (this.menu.OnPause = new Action(this.CloseMenu)));
		}

		// Token: 0x0600127C RID: 4732 RVA: 0x00061CAE File Offset: 0x0005FEAE
		private void CloseMenu()
		{
			Audio.Play("event:/ui/game/unpause");
			if (this.menu != null)
			{
				this.menu.RemoveSelf();
			}
			this.menu = null;
		}

		// Token: 0x0600127D RID: 4733 RVA: 0x00061CD8 File Offset: 0x0005FED8
		private void StartGame()
		{
			this.textCoroutine = null;
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

		// Token: 0x0600127E RID: 4734 RVA: 0x00061D38 File Offset: 0x0005FF38
		private void ReturnToMap()
		{
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

		// Token: 0x0600127F RID: 4735 RVA: 0x00061DE0 File Offset: 0x0005FFE0
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

		// Token: 0x04000E38 RID: 3640
		private Session session;

		// Token: 0x04000E39 RID: 3641
		private Coroutine textCoroutine;

		// Token: 0x04000E3A RID: 3642
		private FancyText.Text text;

		// Token: 0x04000E3B RID: 3643
		private int textStart;

		// Token: 0x04000E3C RID: 3644
		private float textAlpha;

		// Token: 0x04000E3D RID: 3645
		private HiresSnow snow;

		// Token: 0x04000E3E RID: 3646
		private HudRenderer hud;

		// Token: 0x04000E3F RID: 3647
		private TextMenu menu;

		// Token: 0x04000E40 RID: 3648
		private float fade;

		// Token: 0x04000E41 RID: 3649
		private float pauseFade;

		// Token: 0x04000E42 RID: 3650
		private bool started;

		// Token: 0x04000E43 RID: 3651
		private bool exiting;
	}
}
