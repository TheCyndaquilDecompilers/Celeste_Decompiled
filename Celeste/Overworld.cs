using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002F9 RID: 761
	public class Overworld : Scene, IOverlayHandler
	{
		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06001786 RID: 6022 RVA: 0x00090DF5 File Offset: 0x0008EFF5
		// (set) Token: 0x06001787 RID: 6023 RVA: 0x00090DFD File Offset: 0x0008EFFD
		public Overlay Overlay { get; set; }

		// Token: 0x06001788 RID: 6024 RVA: 0x00090E08 File Offset: 0x0008F008
		public Overworld(OverworldLoader loader)
		{
			base.Add(this.Mountain = new MountainRenderer());
			base.Add(new HudRenderer());
			base.Add(this.routineEntity = new Entity());
			base.Add(new Overworld.InputEntity(this));
			this.Snow = loader.Snow;
			if (this.Snow == null)
			{
				this.Snow = new HiresSnow(0.45f);
			}
			base.Add(this.Snow);
			base.RendererList.UpdateLists();
			base.Add(this.Snow3D = new Snow3D(this.Mountain.Model));
			base.Add(new MoonParticle3D(this.Mountain.Model, new Vector3(0f, 31f, 0f)));
			base.Add(this.Maddy = new Maddy3D(this.Mountain));
			this.ReloadMenus(loader.StartMode);
			this.Mountain.OnEaseEnd = delegate()
			{
				if (this.Mountain.Area >= 0 && (!this.Maddy.Show || this.lastArea != this.Mountain.Area))
				{
					this.Maddy.Running(this.Mountain.Area < 7);
					this.Maddy.Wiggler.Start();
				}
				this.lastArea = this.Mountain.Area;
			};
			this.lastArea = this.Mountain.Area;
			if (this.Mountain.Area < 0)
			{
				this.Maddy.Hide(true);
			}
			else
			{
				this.Maddy.Position = AreaData.Areas[this.Mountain.Area].MountainCursor;
			}
			Settings.Instance.ApplyVolumes();
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x00090F98 File Offset: 0x0008F198
		public override void Begin()
		{
			base.Begin();
			this.SetNormalMusic();
			ScreenWipe.WipeColor = Color.Black;
			new FadeWipe(this, true, null);
			base.RendererList.UpdateLists();
			if (!this.EnteringPico8)
			{
				base.RendererList.MoveToFront(this.Snow);
				base.RendererList.UpdateLists();
			}
			this.EnteringPico8 = false;
			this.ReloadMountainStuff();
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x00091000 File Offset: 0x0008F200
		public override void End()
		{
			if (!this.EnteringPico8)
			{
				this.Mountain.Dispose();
			}
			base.End();
		}

		// Token: 0x0600178B RID: 6027 RVA: 0x0009101C File Offset: 0x0008F21C
		public void ReloadMenus(Overworld.StartMode startMode = Overworld.StartMode.Titlescreen)
		{
			foreach (Oui entity in this.UIs)
			{
				base.Remove(entity);
			}
			this.UIs.Clear();
			foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
			{
				if (typeof(Oui).IsAssignableFrom(type) && !type.IsAbstract)
				{
					Oui oui = (Oui)Activator.CreateInstance(type);
					oui.Visible = false;
					base.Add(oui);
					this.UIs.Add(oui);
					if (oui.IsStart(this, startMode))
					{
						oui.Visible = true;
						this.Last = (this.Current = oui);
					}
				}
			}
		}

		// Token: 0x0600178C RID: 6028 RVA: 0x00091108 File Offset: 0x0008F308
		public void SetNormalMusic()
		{
			Audio.SetMusic("event:/music/menu/level_select", true, true);
			Audio.SetAmbience("event:/env/amb/worldmap", true);
		}

		// Token: 0x0600178D RID: 6029 RVA: 0x00091124 File Offset: 0x0008F324
		public void ReloadMountainStuff()
		{
			MTN.MountainBird.ReassignVertices();
			MTN.MountainMoon.ReassignVertices();
			MTN.MountainTerrain.ReassignVertices();
			MTN.MountainBuildings.ReassignVertices();
			MTN.MountainCoreWall.ReassignVertices();
			this.Mountain.Model.DisposeBillboardBuffers();
			this.Mountain.Model.ResetBillboardBuffers();
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x00091183 File Offset: 0x0008F383
		public override void HandleGraphicsReset()
		{
			this.ReloadMountainStuff();
			base.HandleGraphicsReset();
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x00091194 File Offset: 0x0008F394
		public override void Update()
		{
			if (this.Mountain.Area >= 0 && !this.Mountain.Animating)
			{
				Vector3 mountainCursor = AreaData.Areas[this.Mountain.Area].MountainCursor;
				if (mountainCursor != Vector3.Zero)
				{
					this.Maddy.Position = mountainCursor + new Vector3(0f, (float)Math.Sin((double)(this.TimeActive * 2f)) * 0.02f, 0f);
				}
			}
			if (this.Overlay != null)
			{
				if (this.Overlay.XboxOverlay)
				{
					this.Mountain.Update(this);
					this.Snow3D.Update();
				}
				this.Overlay.Update();
				base.Entities.UpdateLists();
				if (this.Snow != null)
				{
					this.Snow.Update(this);
				}
			}
			else
			{
				if (!this.transitioning || !this.ShowInputUI)
				{
					this.inputEase = Calc.Approach(this.inputEase, (float)((this.ShowInputUI && !Input.GuiInputController(Input.PrefixMode.Latest)) ? 1 : 0), Engine.DeltaTime * 4f);
				}
				base.Update();
			}
			if (SaveData.Instance != null && SaveData.Instance.LastArea.ID == 10 && 10 <= SaveData.Instance.UnlockedAreas && !this.IsCurrent<OuiMainMenu>())
			{
				Audio.SetMusicParam("moon", 1f);
			}
			else
			{
				Audio.SetMusicParam("moon", 0f);
			}
			float num = 1f;
			bool flag = false;
			foreach (Renderer renderer in base.RendererList.Renderers)
			{
				if (renderer is ScreenWipe)
				{
					flag = true;
					num = (renderer as ScreenWipe).Duration;
				}
			}
			bool flag2 = (this.Current is OuiTitleScreen && this.Next == null) || this.Next is OuiTitleScreen;
			if (this.Snow != null)
			{
				this.Snow.ParticleAlpha = Calc.Approach(this.Snow.ParticleAlpha, (float)((flag2 || flag || (this.Overlay != null && !this.Overlay.XboxOverlay)) ? 1 : 0), Engine.DeltaTime / num);
			}
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x000913E4 File Offset: 0x0008F5E4
		public T Goto<T>() where T : Oui
		{
			T ui = this.GetUI<T>();
			if (ui != null)
			{
				this.routineEntity.Add(new Coroutine(this.GotoRoutine(ui), true));
			}
			return ui;
		}

		// Token: 0x06001791 RID: 6033 RVA: 0x0009141E File Offset: 0x0008F61E
		public bool IsCurrent<T>() where T : Oui
		{
			if (this.Current != null)
			{
				return this.Current is T;
			}
			return this.Last is T;
		}

		// Token: 0x06001792 RID: 6034 RVA: 0x00091448 File Offset: 0x0008F648
		public T GetUI<T>() where T : Oui
		{
			Oui oui = null;
			foreach (Oui oui2 in this.UIs)
			{
				if (oui2 is T)
				{
					oui = oui2;
				}
			}
			return oui as T;
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x000914AC File Offset: 0x0008F6AC
		private IEnumerator GotoRoutine(Oui next)
		{
			while (this.Current == null)
			{
				yield return null;
			}
			this.transitioning = true;
			this.Next = next;
			this.Last = this.Current;
			this.Current = null;
			this.Last.Focused = false;
			yield return this.Last.Leave(next);
			if (next.Scene != null)
			{
				yield return next.Enter(this.Last);
				next.Focused = true;
				this.Current = next;
				this.transitioning = false;
			}
			this.Next = null;
			yield break;
		}

		// Token: 0x0400143E RID: 5182
		public List<Oui> UIs = new List<Oui>();

		// Token: 0x0400143F RID: 5183
		public Oui Current;

		// Token: 0x04001440 RID: 5184
		public Oui Last;

		// Token: 0x04001441 RID: 5185
		public Oui Next;

		// Token: 0x04001442 RID: 5186
		public bool EnteringPico8;

		// Token: 0x04001444 RID: 5188
		public bool ShowInputUI = true;

		// Token: 0x04001445 RID: 5189
		public bool ShowConfirmUI = true;

		// Token: 0x04001446 RID: 5190
		private float inputEase;

		// Token: 0x04001447 RID: 5191
		public MountainRenderer Mountain;

		// Token: 0x04001448 RID: 5192
		public HiresSnow Snow;

		// Token: 0x04001449 RID: 5193
		private Snow3D Snow3D;

		// Token: 0x0400144A RID: 5194
		public Maddy3D Maddy;

		// Token: 0x0400144B RID: 5195
		private Entity routineEntity;

		// Token: 0x0400144C RID: 5196
		private bool transitioning;

		// Token: 0x0400144D RID: 5197
		private int lastArea = -1;

		// Token: 0x020006A4 RID: 1700
		public enum StartMode
		{
			// Token: 0x04002B7B RID: 11131
			Titlescreen,
			// Token: 0x04002B7C RID: 11132
			ReturnFromOptions,
			// Token: 0x04002B7D RID: 11133
			AreaComplete,
			// Token: 0x04002B7E RID: 11134
			AreaQuit,
			// Token: 0x04002B7F RID: 11135
			ReturnFromPico8,
			// Token: 0x04002B80 RID: 11136
			MainMenu
		}

		// Token: 0x020006A5 RID: 1701
		private class InputEntity : Entity
		{
			// Token: 0x06002C41 RID: 11329 RVA: 0x00119FE8 File Offset: 0x001181E8
			public InputEntity(Overworld overworld)
			{
				this.Overworld = overworld;
				base.Tag = Tags.HUD;
				base.Depth = -100000;
				base.Add(this.confirmWiggle = Wiggler.Create(0.4f, 4f, null, false, false));
				base.Add(this.cancelWiggle = Wiggler.Create(0.4f, 4f, null, false, false));
			}

			// Token: 0x06002C42 RID: 11330 RVA: 0x0011A060 File Offset: 0x00118260
			public override void Update()
			{
				if (Input.MenuConfirm.Pressed && this.confirmWiggleDelay <= 0f)
				{
					this.confirmWiggle.Start();
					this.confirmWiggleDelay = 0.5f;
				}
				if (Input.MenuCancel.Pressed && this.cancelWiggleDelay <= 0f)
				{
					this.cancelWiggle.Start();
					this.cancelWiggleDelay = 0.5f;
				}
				this.confirmWiggleDelay -= Engine.DeltaTime;
				this.cancelWiggleDelay -= Engine.DeltaTime;
				base.Update();
			}

			// Token: 0x06002C43 RID: 11331 RVA: 0x0011A0F8 File Offset: 0x001182F8
			public override void Render()
			{
				float inputEase = this.Overworld.inputEase;
				if (inputEase > 0f)
				{
					float num = 0.5f;
					int num2 = 32;
					string label = Dialog.Clean("ui_cancel", null);
					string label2 = Dialog.Clean("ui_confirm", null);
					float num3 = ButtonUI.Width(label, Input.MenuCancel);
					float num4 = ButtonUI.Width(label2, Input.MenuConfirm);
					Vector2 position = new Vector2(1880f, 1024f);
					position.X += (40f + (num4 + num3) * num + (float)num2) * (1f - Ease.CubeOut(inputEase));
					ButtonUI.Render(position, label, Input.MenuCancel, num, 1f, this.cancelWiggle.Value * 0.05f, 1f);
					if (this.Overworld.ShowConfirmUI)
					{
						position.X -= num * num3 + (float)num2;
						ButtonUI.Render(position, label2, Input.MenuConfirm, num, 1f, this.confirmWiggle.Value * 0.05f, 1f);
					}
				}
			}

			// Token: 0x04002B81 RID: 11137
			public Overworld Overworld;

			// Token: 0x04002B82 RID: 11138
			private Wiggler confirmWiggle;

			// Token: 0x04002B83 RID: 11139
			private Wiggler cancelWiggle;

			// Token: 0x04002B84 RID: 11140
			private float confirmWiggleDelay;

			// Token: 0x04002B85 RID: 11141
			private float cancelWiggleDelay;
		}
	}
}
