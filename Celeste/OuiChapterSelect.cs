using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002FB RID: 763
	public class OuiChapterSelect : Oui
	{
		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060017B4 RID: 6068 RVA: 0x00092D17 File Offset: 0x00090F17
		// (set) Token: 0x060017B5 RID: 6069 RVA: 0x00092D28 File Offset: 0x00090F28
		private int area
		{
			get
			{
				return SaveData.Instance.LastArea.ID;
			}
			set
			{
				SaveData.Instance.LastArea.ID = value;
			}
		}

		// Token: 0x060017B6 RID: 6070 RVA: 0x00092D3A File Offset: 0x00090F3A
		public override bool IsStart(Overworld overworld, Overworld.StartMode start)
		{
			if (start == Overworld.StartMode.AreaComplete || start == Overworld.StartMode.AreaQuit)
			{
				this.indexToSnap = this.area;
			}
			return false;
		}

		// Token: 0x060017B7 RID: 6071 RVA: 0x00092D54 File Offset: 0x00090F54
		public override void Added(Scene scene)
		{
			base.Added(scene);
			int count = AreaData.Areas.Count;
			for (int i = 0; i < count; i++)
			{
				MTexture mtexture = GFX.Gui[AreaData.Areas[i].Icon];
				MTexture back = GFX.Gui.Has(AreaData.Areas[i].Icon + "_back") ? GFX.Gui[AreaData.Areas[i].Icon + "_back"] : mtexture;
				this.icons.Add(new OuiChapterSelectIcon(i, mtexture, back));
				base.Scene.Add(this.icons[i]);
			}
			this.scarfSegments = new MTexture[this.scarf.Height / 2];
			for (int j = 0; j < this.scarfSegments.Length; j++)
			{
				this.scarfSegments[j] = this.scarf.GetSubtexture(0, j * 2, this.scarf.Width, 2, null);
			}
			if (this.indexToSnap >= 0)
			{
				this.area = this.indexToSnap;
				this.icons[this.indexToSnap].SnapToSelected();
			}
			base.Depth = -20;
		}

		// Token: 0x060017B8 RID: 6072 RVA: 0x00092EA0 File Offset: 0x000910A0
		public override IEnumerator Enter(Oui from)
		{
			this.Visible = true;
			this.EaseCamera();
			this.display = true;
			this.journalEnabled = (Celeste.PlayMode == Celeste.PlayModes.Debug || SaveData.Instance.CheatMode);
			int num = 0;
			while (num <= SaveData.Instance.UnlockedAreas && !this.journalEnabled)
			{
				if (SaveData.Instance.Areas[num].Modes[0].TimePlayed > 0L && !AreaData.Get(num).Interlude)
				{
					this.journalEnabled = true;
				}
				num++;
			}
			OuiChapterSelectIcon unselected = null;
			if (from is OuiChapterPanel)
			{
				(unselected = this.icons[this.area]).Unselect();
			}
			foreach (OuiChapterSelectIcon ouiChapterSelectIcon in this.icons)
			{
				if (ouiChapterSelectIcon.Area <= SaveData.Instance.UnlockedAreas && ouiChapterSelectIcon != unselected)
				{
					ouiChapterSelectIcon.Position = ouiChapterSelectIcon.HiddenPosition;
					ouiChapterSelectIcon.Show();
					ouiChapterSelectIcon.AssistModeUnlockable = false;
				}
				else if (SaveData.Instance.AssistMode && ouiChapterSelectIcon.Area == SaveData.Instance.UnlockedAreas + 1 && ouiChapterSelectIcon.Area <= SaveData.Instance.MaxAssistArea)
				{
					ouiChapterSelectIcon.Position = ouiChapterSelectIcon.HiddenPosition;
					ouiChapterSelectIcon.Show();
					ouiChapterSelectIcon.AssistModeUnlockable = true;
				}
				yield return 0.01f;
			}
			List<OuiChapterSelectIcon>.Enumerator enumerator = default(List<OuiChapterSelectIcon>.Enumerator);
			if (!this.autoAdvancing && SaveData.Instance.UnlockedAreas == 10 && !SaveData.Instance.RevealedChapter9)
			{
				int ch = this.area;
				yield return this.SetupCh9Unlock();
				yield return this.PerformCh9Unlock(ch != 10);
			}
			if (from is OuiChapterPanel)
			{
				yield return 0.25f;
			}
			yield break;
			yield break;
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x00092EB6 File Offset: 0x000910B6
		public override IEnumerator Leave(Oui next)
		{
			this.display = false;
			if (next is OuiMainMenu)
			{
				while (this.area > SaveData.Instance.UnlockedAreas)
				{
					int area = this.area;
					this.area = area - 1;
				}
				UserIO.SaveHandler(true, false);
				yield return this.EaseOut(next);
				while (UserIO.Saving)
				{
					yield return null;
				}
			}
			else
			{
				yield return this.EaseOut(next);
			}
			yield break;
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x00092ECC File Offset: 0x000910CC
		private IEnumerator EaseOut(Oui next)
		{
			OuiChapterSelectIcon selected = null;
			if (next is OuiChapterPanel)
			{
				(selected = this.icons[this.area]).Select();
			}
			foreach (OuiChapterSelectIcon ouiChapterSelectIcon in this.icons)
			{
				if (selected != ouiChapterSelectIcon)
				{
					ouiChapterSelectIcon.Hide();
				}
				yield return 0.01f;
			}
			List<OuiChapterSelectIcon>.Enumerator enumerator = default(List<OuiChapterSelectIcon>.Enumerator);
			this.Visible = false;
			yield break;
			yield break;
		}

		// Token: 0x060017BB RID: 6075 RVA: 0x00092EE2 File Offset: 0x000910E2
		public void AdvanceToNext()
		{
			this.autoAdvancing = true;
			base.Overworld.ShowInputUI = false;
			this.Focused = false;
			this.disableInput = true;
			base.Add(new Coroutine(this.AutoAdvanceRoutine(), true));
		}

		// Token: 0x060017BC RID: 6076 RVA: 0x00092F17 File Offset: 0x00091117
		private IEnumerator AutoAdvanceRoutine()
		{
			if (this.area < SaveData.Instance.MaxArea)
			{
				int nextArea = this.area + 1;
				if (nextArea == 9 || nextArea == 10)
				{
					this.icons[nextArea].HideIcon = true;
				}
				while (!base.Selected)
				{
					yield return null;
				}
				yield return 1f;
				if (nextArea == 10)
				{
					yield return this.PerformCh9Unlock(true);
				}
				else if (nextArea == 9)
				{
					yield return this.PerformCh8Unlock();
				}
				else
				{
					Audio.Play("event:/ui/postgame/unlock_newchapter");
					Audio.Play("event:/ui/world_map/icon/roll_right");
					this.area = nextArea;
					this.EaseCamera();
					base.Overworld.Maddy.Hide(true);
				}
				yield return 0.25f;
			}
			this.autoAdvancing = false;
			this.disableInput = false;
			this.Focused = true;
			base.Overworld.ShowInputUI = true;
			yield break;
		}

		// Token: 0x060017BD RID: 6077 RVA: 0x00092F28 File Offset: 0x00091128
		public override void Update()
		{
			if (this.Focused && !this.disableInput)
			{
				this.inputDelay -= Engine.DeltaTime;
				if (this.area >= 0 && this.area < AreaData.Areas.Count)
				{
					Input.SetLightbarColor(AreaData.Get(this.area).TitleBaseColor);
				}
				if (Input.MenuCancel.Pressed)
				{
					Audio.Play("event:/ui/main/button_back");
					base.Overworld.Goto<OuiMainMenu>();
					base.Overworld.Maddy.Hide(true);
				}
				else if (Input.MenuJournal.Pressed && this.journalEnabled)
				{
					Audio.Play("event:/ui/world_map/journal/select");
					base.Overworld.Goto<OuiJournal>();
				}
				else if (this.inputDelay <= 0f)
				{
					if (this.area > 0 && Input.MenuLeft.Pressed)
					{
						Audio.Play("event:/ui/world_map/icon/roll_left");
						this.inputDelay = 0.15f;
						int area = this.area;
						this.area = area - 1;
						this.icons[this.area].Hovered(-1);
						this.EaseCamera();
						base.Overworld.Maddy.Hide(true);
					}
					else if (Input.MenuRight.Pressed)
					{
						bool flag = SaveData.Instance.AssistMode && this.area == SaveData.Instance.UnlockedAreas && this.area < SaveData.Instance.MaxAssistArea;
						if (this.area < SaveData.Instance.UnlockedAreas || flag)
						{
							Audio.Play("event:/ui/world_map/icon/roll_right");
							this.inputDelay = 0.15f;
							int area = this.area;
							this.area = area + 1;
							this.icons[this.area].Hovered(1);
							if (this.area <= SaveData.Instance.UnlockedAreas)
							{
								this.EaseCamera();
							}
							base.Overworld.Maddy.Hide(true);
						}
					}
					else if (Input.MenuConfirm.Pressed)
					{
						if (this.icons[this.area].AssistModeUnlockable)
						{
							Audio.Play("event:/ui/world_map/icon/assist_skip");
							this.Focused = false;
							base.Overworld.ShowInputUI = false;
							this.icons[this.area].AssistModeUnlock(delegate
							{
								this.Focused = true;
								base.Overworld.ShowInputUI = true;
								this.EaseCamera();
								if (this.area == 10)
								{
									SaveData.Instance.RevealedChapter9 = true;
								}
								if (this.area < SaveData.Instance.MaxAssistArea)
								{
									OuiChapterSelectIcon ouiChapterSelectIcon = this.icons[this.area + 1];
									ouiChapterSelectIcon.AssistModeUnlockable = true;
									ouiChapterSelectIcon.Position = ouiChapterSelectIcon.HiddenPosition;
									ouiChapterSelectIcon.Show();
								}
							});
						}
						else
						{
							Audio.Play("event:/ui/world_map/icon/select");
							SaveData.Instance.LastArea.Mode = AreaMode.Normal;
							base.Overworld.Goto<OuiChapterPanel>();
						}
					}
				}
			}
			this.ease = Calc.Approach(this.ease, this.display ? 1f : 0f, Engine.DeltaTime * 3f);
			this.journalEase = Calc.Approach(this.journalEase, (this.display && !this.disableInput && this.Focused && this.journalEnabled) ? 1f : 0f, Engine.DeltaTime * 4f);
			base.Update();
		}

		// Token: 0x060017BE RID: 6078 RVA: 0x0009324C File Offset: 0x0009144C
		public override void Render()
		{
			Vector2 value = new Vector2(960f, (float)(-(float)this.scarf.Height) * Ease.CubeInOut(1f - this.ease));
			for (int i = 0; i < this.scarfSegments.Length; i++)
			{
				float num = Ease.CubeIn((float)i / (float)this.scarfSegments.Length);
				float x = num * (float)Math.Sin((double)(base.Scene.RawTimeActive * 4f + (float)i * 0.05f)) * 4f - num * 16f;
				this.scarfSegments[i].DrawJustified(value + new Vector2(x, (float)(i * 2)), new Vector2(0.5f, 0f));
			}
			if (this.journalEase > 0f)
			{
				Vector2 position = new Vector2(128f * Ease.CubeOut(this.journalEase), 952f);
				GFX.Gui["menu/journal"].DrawCentered(position, Color.White * Ease.CubeOut(this.journalEase));
				Input.GuiButton(Input.MenuJournal, Input.PrefixMode.Latest, "controls/keyboard/oemquestion").Draw(position, Vector2.Zero, Color.White * Ease.CubeOut(this.journalEase));
			}
		}

		// Token: 0x060017BF RID: 6079 RVA: 0x000933AC File Offset: 0x000915AC
		private void EaseCamera()
		{
			AreaData areaData = AreaData.Areas[this.area];
			base.Overworld.Mountain.EaseCamera(this.area, areaData.MountainIdle, null, true, this.area == 10);
			base.Overworld.Mountain.Model.EaseState(areaData.MountainState);
		}

		// Token: 0x060017C0 RID: 6080 RVA: 0x00093416 File Offset: 0x00091616
		private IEnumerator PerformCh8Unlock()
		{
			Audio.Play("event:/ui/postgame/unlock_newchapter");
			Audio.Play("event:/ui/world_map/icon/roll_right");
			this.area = 9;
			this.EaseCamera();
			base.Overworld.Maddy.Hide(true);
			bool ready = false;
			this.icons[9].HighlightUnlock(delegate
			{
				ready = true;
			});
			while (!ready)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x060017C1 RID: 6081 RVA: 0x00093425 File Offset: 0x00091625
		private IEnumerator SetupCh9Unlock()
		{
			this.icons[10].HideIcon = true;
			yield return 0.25f;
			while (this.area < 9)
			{
				int area = this.area;
				this.area = area + 1;
				yield return 0.1f;
			}
			yield break;
		}

		// Token: 0x060017C2 RID: 6082 RVA: 0x00093434 File Offset: 0x00091634
		private IEnumerator PerformCh9Unlock(bool easeCamera = true)
		{
			Audio.Play("event:/ui/postgame/unlock_newchapter");
			Audio.Play("event:/ui/world_map/icon/roll_right");
			this.area = 10;
			yield return 0.25f;
			bool ready = false;
			this.icons[10].HighlightUnlock(delegate
			{
				ready = true;
			});
			while (!ready)
			{
				yield return null;
			}
			if (easeCamera)
			{
				this.EaseCamera();
			}
			base.Overworld.Maddy.Hide(true);
			SaveData.Instance.RevealedChapter9 = true;
			yield break;
		}

		// Token: 0x0400146F RID: 5231
		private List<OuiChapterSelectIcon> icons = new List<OuiChapterSelectIcon>();

		// Token: 0x04001470 RID: 5232
		private int indexToSnap = -1;

		// Token: 0x04001471 RID: 5233
		private const int scarfSegmentSize = 2;

		// Token: 0x04001472 RID: 5234
		private MTexture scarf = GFX.Gui["areas/hover"];

		// Token: 0x04001473 RID: 5235
		private MTexture[] scarfSegments;

		// Token: 0x04001474 RID: 5236
		private float ease;

		// Token: 0x04001475 RID: 5237
		private float journalEase;

		// Token: 0x04001476 RID: 5238
		private bool journalEnabled;

		// Token: 0x04001477 RID: 5239
		private bool disableInput;

		// Token: 0x04001478 RID: 5240
		private bool display;

		// Token: 0x04001479 RID: 5241
		private float inputDelay;

		// Token: 0x0400147A RID: 5242
		private bool autoAdvancing;
	}
}
