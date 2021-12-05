using System;
using System.Collections;
using System.Collections.Generic;
using Celeste.Pico8;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002FE RID: 766
	public class OuiMainMenu : Oui
	{
		// Token: 0x060017E1 RID: 6113 RVA: 0x00094B79 File Offset: 0x00092D79
		public OuiMainMenu()
		{
			this.buttons = new List<MenuButton>();
		}

		// Token: 0x060017E2 RID: 6114 RVA: 0x00094B8C File Offset: 0x00092D8C
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.Position = OuiMainMenu.TweenFrom;
			this.CreateButtons();
		}

		// Token: 0x060017E3 RID: 6115 RVA: 0x00094BA8 File Offset: 0x00092DA8
		public void CreateButtons()
		{
			foreach (MenuButton menuButton in this.buttons)
			{
				menuButton.RemoveSelf();
			}
			this.buttons.Clear();
			Vector2 vector = new Vector2(320f, 160f);
			Vector2 value = new Vector2(-640f, 0f);
			this.climbButton = new MainMenuClimb(this, vector, vector + value, new Action(this.OnBegin));
			if (!this.startOnOptions)
			{
				this.climbButton.StartSelected();
			}
			this.buttons.Add(this.climbButton);
			vector += Vector2.UnitY * this.climbButton.ButtonHeight;
			vector.X -= 140f;
			if (Celeste.PlayMode == Celeste.PlayModes.Debug)
			{
				MainMenuSmallButton mainMenuSmallButton = new MainMenuSmallButton("menu_debug", "menu/options", this, vector, vector + value, new Action(this.OnDebug));
				this.buttons.Add(mainMenuSmallButton);
				vector += Vector2.UnitY * mainMenuSmallButton.ButtonHeight;
			}
			if (Settings.Instance.Pico8OnMainMenu || Celeste.PlayMode == Celeste.PlayModes.Debug || Celeste.PlayMode == Celeste.PlayModes.Event)
			{
				MainMenuSmallButton mainMenuSmallButton2 = new MainMenuSmallButton("menu_pico8", "menu/pico8", this, vector, vector + value, new Action(this.OnPico8));
				this.buttons.Add(mainMenuSmallButton2);
				vector += Vector2.UnitY * mainMenuSmallButton2.ButtonHeight;
			}
			MainMenuSmallButton mainMenuSmallButton3 = new MainMenuSmallButton("menu_options", "menu/options", this, vector, vector + value, new Action(this.OnOptions));
			if (this.startOnOptions)
			{
				mainMenuSmallButton3.StartSelected();
			}
			this.buttons.Add(mainMenuSmallButton3);
			vector += Vector2.UnitY * mainMenuSmallButton3.ButtonHeight;
			MainMenuSmallButton mainMenuSmallButton4 = new MainMenuSmallButton("menu_credits", "menu/credits", this, vector, vector + value, new Action(this.OnCredits));
			this.buttons.Add(mainMenuSmallButton4);
			vector += Vector2.UnitY * mainMenuSmallButton4.ButtonHeight;
			MainMenuSmallButton mainMenuSmallButton5 = new MainMenuSmallButton("menu_exit", "menu/exit", this, vector, vector + value, new Action(this.OnExit));
			this.buttons.Add(mainMenuSmallButton5);
			vector += Vector2.UnitY * mainMenuSmallButton5.ButtonHeight;
			for (int i = 0; i < this.buttons.Count; i++)
			{
				if (i > 0)
				{
					this.buttons[i].UpButton = this.buttons[i - 1];
				}
				if (i < this.buttons.Count - 1)
				{
					this.buttons[i].DownButton = this.buttons[i + 1];
				}
				base.Scene.Add(this.buttons[i]);
			}
			if (this.Visible && this.Focused)
			{
				foreach (MenuButton menuButton2 in this.buttons)
				{
					menuButton2.Position = menuButton2.TargetPosition;
				}
			}
		}

		// Token: 0x060017E4 RID: 6116 RVA: 0x00094F20 File Offset: 0x00093120
		public override void Removed(Scene scene)
		{
			foreach (MenuButton entity in this.buttons)
			{
				scene.Remove(entity);
			}
			base.Removed(scene);
		}

		// Token: 0x060017E5 RID: 6117 RVA: 0x00094F7C File Offset: 0x0009317C
		public override bool IsStart(Overworld overworld, Overworld.StartMode start)
		{
			if (start == Overworld.StartMode.ReturnFromOptions)
			{
				this.startOnOptions = true;
				base.Add(new Coroutine(this.Enter(null), true));
				return true;
			}
			if (start == Overworld.StartMode.MainMenu)
			{
				this.mountainStartFront = true;
				base.Add(new Coroutine(this.Enter(null), true));
				return true;
			}
			return start == Overworld.StartMode.ReturnFromOptions || start == Overworld.StartMode.ReturnFromPico8;
		}

		// Token: 0x060017E6 RID: 6118 RVA: 0x00094FD3 File Offset: 0x000931D3
		public override IEnumerator Enter(Oui from)
		{
			if (from is OuiTitleScreen || from is OuiFileSelect)
			{
				Audio.Play("event:/ui/main/whoosh_list_in");
				yield return 0.1f;
			}
			if (from is OuiTitleScreen)
			{
				MenuButton.ClearSelection(base.Scene);
				this.climbButton.StartSelected();
			}
			this.Visible = true;
			if (this.mountainStartFront)
			{
				base.Overworld.Mountain.SnapCamera(-1, new MountainCamera(new Vector3(0f, 6f, 12f), MountainRenderer.RotateLookAt), false);
			}
			base.Overworld.Mountain.GotoRotationMode();
			base.Overworld.Maddy.Hide(true);
			foreach (MenuButton menuButton in this.buttons)
			{
				menuButton.TweenIn(0.2f);
			}
			yield return 0.2f;
			this.Focused = true;
			this.mountainStartFront = false;
			yield return null;
			yield break;
		}

		// Token: 0x060017E7 RID: 6119 RVA: 0x00094FE9 File Offset: 0x000931E9
		public override IEnumerator Leave(Oui next)
		{
			this.Focused = false;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.2f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				this.ease = 1f - t.Eased;
				this.Position = Vector2.Lerp(OuiMainMenu.TargetPosition, OuiMainMenu.TweenFrom, t.Eased);
			};
			base.Add(tween);
			bool keepClimb = this.climbButton.Selected && !(next is OuiTitleScreen);
			foreach (MenuButton menuButton in this.buttons)
			{
				if (menuButton != this.climbButton || !keepClimb)
				{
					menuButton.TweenOut(0.2f);
				}
			}
			yield return 0.2f;
			if (keepClimb)
			{
				base.Add(new Coroutine(this.SlideClimbOutLate(), true));
			}
			else
			{
				this.Visible = false;
			}
			yield break;
		}

		// Token: 0x060017E8 RID: 6120 RVA: 0x00094FFF File Offset: 0x000931FF
		private IEnumerator SlideClimbOutLate()
		{
			yield return 0.2f;
			this.climbButton.TweenOut(0.2f);
			yield return 0.2f;
			this.Visible = false;
			yield break;
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x060017E9 RID: 6121 RVA: 0x0009500E File Offset: 0x0009320E
		public Color SelectionColor
		{
			get
			{
				if (!Settings.Instance.DisableFlashes && !base.Scene.BetweenInterval(0.1f))
				{
					return OuiMainMenu.SelectedColorB;
				}
				return OuiMainMenu.SelectedColorA;
			}
		}

		// Token: 0x060017EA RID: 6122 RVA: 0x0009503C File Offset: 0x0009323C
		public override void Update()
		{
			if (base.Selected && this.Focused && Input.MenuCancel.Pressed)
			{
				this.Focused = false;
				Audio.Play("event:/ui/main/whoosh_list_out");
				Audio.Play("event:/ui/main/button_back");
				base.Overworld.Goto<OuiTitleScreen>();
			}
			base.Update();
		}

		// Token: 0x060017EB RID: 6123 RVA: 0x00095094 File Offset: 0x00093294
		public override void Render()
		{
			foreach (MenuButton menuButton in this.buttons)
			{
				if (menuButton.Scene == base.Scene)
				{
					menuButton.Render();
				}
			}
		}

		// Token: 0x060017EC RID: 6124 RVA: 0x000950F4 File Offset: 0x000932F4
		private void OnDebug()
		{
			Audio.Play("event:/ui/main/whoosh_list_out");
			Audio.Play("event:/ui/main/button_select");
			SaveData.InitializeDebugMode(true);
			base.Overworld.Goto<OuiChapterSelect>();
		}

		// Token: 0x060017ED RID: 6125 RVA: 0x0009511E File Offset: 0x0009331E
		private void OnBegin()
		{
			Audio.Play("event:/ui/main/whoosh_list_out");
			Audio.Play("event:/ui/main/button_climb");
			if (Celeste.PlayMode == Celeste.PlayModes.Event)
			{
				SaveData.InitializeDebugMode(false);
				base.Overworld.Goto<OuiChapterSelect>();
				return;
			}
			base.Overworld.Goto<OuiFileSelect>();
		}

		// Token: 0x060017EE RID: 6126 RVA: 0x0009515D File Offset: 0x0009335D
		private void OnPico8()
		{
			Audio.Play("event:/ui/main/button_select");
			this.Focused = false;
			new FadeWipe(base.Scene, false, delegate()
			{
				this.Focused = true;
				base.Overworld.EnteringPico8 = true;
				SaveData.Instance = null;
				SaveData.NoFileAssistChecks();
				Engine.Scene = new Emulator(base.Overworld, 0, 0);
			});
		}

		// Token: 0x060017EF RID: 6127 RVA: 0x0009518A File Offset: 0x0009338A
		private void OnOptions()
		{
			Audio.Play("event:/ui/main/button_select");
			Audio.Play("event:/ui/main/whoosh_large_in");
			base.Overworld.Goto<OuiOptions>();
		}

		// Token: 0x060017F0 RID: 6128 RVA: 0x000951AE File Offset: 0x000933AE
		private void OnCredits()
		{
			Audio.Play("event:/ui/main/button_select");
			Audio.Play("event:/ui/main/whoosh_large_in");
			base.Overworld.Goto<OuiCredits>();
		}

		// Token: 0x060017F1 RID: 6129 RVA: 0x000951D2 File Offset: 0x000933D2
		private void OnExit()
		{
			Audio.Play("event:/ui/main/button_select");
			this.Focused = false;
			new FadeWipe(base.Scene, false, delegate()
			{
				Engine.Scene = new Scene();
				Engine.Instance.Exit();
			});
		}

		// Token: 0x040014A8 RID: 5288
		private static readonly Vector2 TargetPosition = new Vector2(160f, 160f);

		// Token: 0x040014A9 RID: 5289
		private static readonly Vector2 TweenFrom = new Vector2(-500f, 160f);

		// Token: 0x040014AA RID: 5290
		private static readonly Color UnselectedColor = Color.White;

		// Token: 0x040014AB RID: 5291
		private static readonly Color SelectedColorA = TextMenu.HighlightColorA;

		// Token: 0x040014AC RID: 5292
		private static readonly Color SelectedColorB = TextMenu.HighlightColorB;

		// Token: 0x040014AD RID: 5293
		private const float IconWidth = 64f;

		// Token: 0x040014AE RID: 5294
		private const float IconSpacing = 20f;

		// Token: 0x040014AF RID: 5295
		private float ease;

		// Token: 0x040014B0 RID: 5296
		private MainMenuClimb climbButton;

		// Token: 0x040014B1 RID: 5297
		private List<MenuButton> buttons;

		// Token: 0x040014B2 RID: 5298
		private bool startOnOptions;

		// Token: 0x040014B3 RID: 5299
		private bool mountainStartFront;
	}
}
