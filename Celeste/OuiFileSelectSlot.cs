using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000297 RID: 663
	public class OuiFileSelectSlot : Entity
	{
		// Token: 0x1700015F RID: 351
		// (get) Token: 0x0600147A RID: 5242 RVA: 0x0006FE5A File Offset: 0x0006E05A
		public Vector2 IdlePosition
		{
			get
			{
				return new Vector2(960f, (float)(540 + 310 * (this.FileSlot - 1)));
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x0600147B RID: 5243 RVA: 0x0006FE7B File Offset: 0x0006E07B
		public Vector2 SelectedPosition
		{
			get
			{
				return new Vector2(960f, 440f);
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x0600147C RID: 5244 RVA: 0x0006FE8C File Offset: 0x0006E08C
		private bool highlighted
		{
			get
			{
				return this.fileSelect.SlotIndex == this.FileSlot;
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600147D RID: 5245 RVA: 0x0006FEA1 File Offset: 0x0006E0A1
		private bool selected
		{
			get
			{
				return this.fileSelect.SlotSelected && this.highlighted;
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600147E RID: 5246 RVA: 0x0006FEB8 File Offset: 0x0006E0B8
		private bool Golden
		{
			get
			{
				return !this.Corrupted && this.Exists && this.SaveData.TotalStrawberries >= 202;
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x0600147F RID: 5247 RVA: 0x0006FEE1 File Offset: 0x0006E0E1
		private Sprite Card
		{
			get
			{
				if (!this.Golden)
				{
					return this.normalCard;
				}
				return this.goldCard;
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06001480 RID: 5248 RVA: 0x0006FEF8 File Offset: 0x0006E0F8
		private Sprite Ticket
		{
			get
			{
				if (!this.Golden)
				{
					return this.normalTicket;
				}
				return this.goldTicket;
			}
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x0006FF10 File Offset: 0x0006E110
		private OuiFileSelectSlot(int index, OuiFileSelect fileSelect)
		{
			this.FileSlot = index;
			this.fileSelect = fileSelect;
			base.Tag |= (Tags.HUD | Tags.PauseUpdate);
			this.Visible = false;
			base.Add(this.wiggler = Wiggler.Create(0.4f, 4f, null, false, false));
			this.normalTicket = new Sprite(MTN.FileSelect, "ticket");
			this.normalTicket.AddLoop("idle", "", 0.1f);
			this.normalTicket.Add("shine", "", 0.1f, "idle");
			this.normalTicket.CenterOrigin();
			this.normalTicket.Play("idle", false, false);
			this.normalCard = new Sprite(MTN.FileSelect, "card");
			this.normalCard.AddLoop("idle", "", 0.1f);
			this.normalCard.Add("shine", "", 0.1f, "idle");
			this.normalCard.CenterOrigin();
			this.normalCard.Play("idle", false, false);
			this.goldTicket = new Sprite(MTN.FileSelect, "ticketShine");
			this.goldTicket.AddLoop("idle", "", 0.1f, new int[1]);
			this.goldTicket.Add("shine", "", 0.05f, "idle", new int[]
			{
				0,
				0,
				0,
				0,
				0,
				1,
				2,
				3,
				4,
				5
			});
			this.goldTicket.CenterOrigin();
			this.goldTicket.Play("idle", false, false);
			this.goldCard = new Sprite(MTN.FileSelect, "cardShine");
			this.goldCard.AddLoop("idle", "", 0.1f, new int[]
			{
				5
			});
			this.goldCard.Add("shine", "", 0.05f, "idle");
			this.goldCard.CenterOrigin();
			this.goldCard.Play("idle", false, false);
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x00070179 File Offset: 0x0006E379
		public OuiFileSelectSlot(int index, OuiFileSelect fileSelect, bool corrupted) : this(index, fileSelect)
		{
			this.Corrupted = corrupted;
			this.Exists = corrupted;
			this.Setup();
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x00070198 File Offset: 0x0006E398
		public OuiFileSelectSlot(int index, OuiFileSelect fileSelect, SaveData data) : this(index, fileSelect)
		{
			this.Exists = true;
			this.SaveData = data;
			this.Name = data.Name;
			if (!Dialog.Language.CanDisplay(this.Name))
			{
				this.Name = Dialog.Clean("FILE_DEFAULT", null);
			}
			if (!Settings.Instance.VariantsUnlocked && data.TotalHeartGems >= 24)
			{
				Settings.Instance.VariantsUnlocked = true;
			}
			this.AssistModeEnabled = data.AssistMode;
			this.VariantModeEnabled = data.VariantMode;
			base.Add(this.Deaths = new DeathsCounter(AreaMode.Normal, false, data.TotalDeaths, 0));
			base.Add(this.Strawberries = new StrawberriesCounter(true, data.TotalStrawberries, 0, false));
			this.Time = Dialog.FileTime(data.Time);
			if (TimeSpan.FromTicks(data.Time).TotalHours > 0.0)
			{
				this.timeScale = 0.725f;
			}
			this.FurthestArea = data.UnlockedAreas;
			foreach (AreaStats areaStats in data.Areas)
			{
				if (areaStats.ID > data.UnlockedAreas)
				{
					break;
				}
				if (!AreaData.Areas[areaStats.ID].Interlude && AreaData.Areas[areaStats.ID].CanFullClear)
				{
					bool[] array = new bool[3];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = areaStats.Modes[i].HeartGem;
					}
					this.Cassettes.Add(areaStats.Cassette);
					this.HeartGems.Add(array);
				}
			}
			this.Setup();
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x00070384 File Offset: 0x0006E584
		private void Setup()
		{
			string text = "portrait_madeline";
			string id = "idle_normal";
			this.Portrait = GFX.PortraitsSpriteBank.Create(text);
			this.Portrait.Play(id, false, false);
			this.Portrait.Scale = Vector2.One * (200f / (float)GFX.PortraitsSpriteBank.SpriteData[text].Sources[0].XML.AttrInt("size", 160));
			base.Add(this.Portrait);
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x00070414 File Offset: 0x0006E614
		public void CreateButtons()
		{
			this.buttons.Clear();
			OuiFileSelectSlot.Button item;
			if (this.Exists)
			{
				if (!this.Corrupted)
				{
					this.buttons.Add(new OuiFileSelectSlot.Button
					{
						Label = Dialog.Clean("file_continue", null),
						Action = new Action(this.OnContinueSelected)
					});
					if (this.SaveData != null)
					{
						List<OuiFileSelectSlot.Button> list = this.buttons;
						OuiFileSelectSlot.Button button = new OuiFileSelectSlot.Button();
						button.Label = Dialog.Clean("FILE_ASSIST_" + (this.AssistModeEnabled ? "ON" : "OFF"), null);
						button.Action = new Action(this.OnAssistSelected);
						button.Scale = 0.7f;
						item = button;
						this.assistButton = button;
						list.Add(item);
						if (Settings.Instance.VariantsUnlocked || this.SaveData.CheatMode)
						{
							List<OuiFileSelectSlot.Button> list2 = this.buttons;
							OuiFileSelectSlot.Button button2 = new OuiFileSelectSlot.Button();
							button2.Label = Dialog.Clean("FILE_VARIANT_" + (this.VariantModeEnabled ? "ON" : "OFF"), null);
							button2.Action = new Action(this.OnVariantSelected);
							button2.Scale = 0.7f;
							item = button2;
							this.variantButton = button2;
							list2.Add(item);
						}
					}
				}
				this.buttons.Add(new OuiFileSelectSlot.Button
				{
					Label = Dialog.Clean("file_delete", null),
					Action = new Action(this.OnDeleteSelected),
					Scale = 0.7f
				});
				return;
			}
			this.buttons.Add(new OuiFileSelectSlot.Button
			{
				Label = Dialog.Clean("file_begin", null),
				Action = new Action(this.OnNewGameSelected)
			});
			this.buttons.Add(new OuiFileSelectSlot.Button
			{
				Label = Dialog.Clean("file_rename", null),
				Action = new Action(this.OnRenameSelected),
				Scale = 0.7f
			});
			List<OuiFileSelectSlot.Button> list3 = this.buttons;
			OuiFileSelectSlot.Button button3 = new OuiFileSelectSlot.Button();
			button3.Label = Dialog.Clean("FILE_ASSIST_" + (this.AssistModeEnabled ? "ON" : "OFF"), null);
			button3.Action = new Action(this.OnAssistSelected);
			button3.Scale = 0.7f;
			item = button3;
			this.assistButton = button3;
			list3.Add(item);
			if (Settings.Instance.VariantsUnlocked)
			{
				List<OuiFileSelectSlot.Button> list4 = this.buttons;
				OuiFileSelectSlot.Button button4 = new OuiFileSelectSlot.Button();
				button4.Label = Dialog.Clean("FILE_VARIANT_" + (this.VariantModeEnabled ? "ON" : "OFF"), null);
				button4.Action = new Action(this.OnVariantSelected);
				button4.Scale = 0.7f;
				item = button4;
				this.variantButton = button4;
				list4.Add(item);
			}
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x000706D8 File Offset: 0x0006E8D8
		private void OnContinueSelected()
		{
			this.StartingGame = true;
			Audio.Play("event:/ui/main/savefile_begin");
			SaveData.Start(this.SaveData, this.FileSlot);
			SaveData.Instance.AssistMode = this.AssistModeEnabled;
			SaveData.Instance.VariantMode = this.VariantModeEnabled;
			SaveData.Instance.AssistModeChecks();
			if (SaveData.Instance.CurrentSession != null && SaveData.Instance.CurrentSession.InArea)
			{
				Audio.SetMusic(null, true, true);
				Audio.SetAmbience(null, true);
				this.fileSelect.Overworld.ShowInputUI = false;
				new FadeWipe(base.Scene, false, delegate()
				{
					LevelEnter.Go(SaveData.Instance.CurrentSession, true);
				});
				return;
			}
			if (SaveData.Instance.Areas[0].Modes[0].Completed || SaveData.Instance.CheatMode)
			{
				if (SaveData.Instance.CurrentSession != null && SaveData.Instance.CurrentSession.ShouldAdvance)
				{
					SaveData.Instance.LastArea.ID = SaveData.Instance.UnlockedAreas;
				}
				SaveData.Instance.CurrentSession = null;
				(base.Scene as Overworld).Goto<OuiChapterSelect>();
				return;
			}
			Audio.SetMusic(null, true, true);
			Audio.SetAmbience(null, true);
			this.EnterFirstArea();
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x00070832 File Offset: 0x0006EA32
		private void OnDeleteSelected()
		{
			this.deleting = true;
			this.wiggler.Start();
			Audio.Play("event:/ui/main/message_confirm");
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x00070854 File Offset: 0x0006EA54
		private void OnNewGameSelected()
		{
			Audio.SetMusic(null, true, true);
			Audio.SetAmbience(null, true);
			Audio.Play("event:/ui/main/savefile_begin");
			SaveData.Start(new SaveData
			{
				Name = this.Name,
				AssistMode = this.AssistModeEnabled,
				VariantMode = this.VariantModeEnabled
			}, this.FileSlot);
			this.StartingGame = true;
			this.EnterFirstArea();
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x000708BE File Offset: 0x0006EABE
		private void EnterFirstArea()
		{
			this.fileSelect.Overworld.Maddy.Disabled = true;
			this.fileSelect.Overworld.ShowInputUI = false;
			base.Add(new Coroutine(this.EnterFirstAreaRoutine(), true));
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x000708F9 File Offset: 0x0006EAF9
		private IEnumerator EnterFirstAreaRoutine()
		{
			Overworld overworld = this.fileSelect.Overworld;
			yield return this.fileSelect.Leave(null);
			yield return overworld.Mountain.EaseCamera(0, AreaData.Areas[0].MountainIdle, null, true, false);
			yield return 0.3f;
			overworld.Mountain.EaseCamera(0, AreaData.Areas[0].MountainZoom, new float?(1f), true, false);
			yield return 0.4f;
			AreaData.Areas[0].Wipe(overworld, false, null);
			overworld.RendererList.UpdateLists();
			overworld.RendererList.MoveToFront(overworld.Snow);
			yield return 0.5f;
			LevelEnter.Go(new Session(new AreaKey(0, AreaMode.Normal), null, null), false);
			yield break;
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x00070908 File Offset: 0x0006EB08
		private void OnRenameSelected()
		{
			this.Renaming = true;
			OuiFileNaming ouiFileNaming = this.fileSelect.Overworld.Goto<OuiFileNaming>();
			ouiFileNaming.FileSlot = this;
			ouiFileNaming.StartingName = this.Name;
			Audio.Play("event:/ui/main/savefile_rename_start");
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x0007093E File Offset: 0x0006EB3E
		private void OnAssistSelected()
		{
			this.Assisting = true;
			this.fileSelect.Overworld.Goto<OuiAssistMode>().FileSlot = this;
			Audio.Play("event:/ui/main/assist_button_info");
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x00070968 File Offset: 0x0006EB68
		private void OnVariantSelected()
		{
			if (Settings.Instance.VariantsUnlocked || (this.SaveData != null && this.SaveData.CheatMode))
			{
				this.VariantModeEnabled = !this.VariantModeEnabled;
				if (this.VariantModeEnabled)
				{
					this.AssistModeEnabled = false;
					Audio.Play("event:/ui/main/button_toggle_on");
				}
				else
				{
					Audio.Play("event:/ui/main/button_toggle_off");
				}
				this.assistButton.Label = Dialog.Clean("FILE_ASSIST_" + (this.AssistModeEnabled ? "ON" : "OFF"), null);
				this.variantButton.Label = Dialog.Clean("FILE_VARIANT_" + (this.VariantModeEnabled ? "ON" : "OFF"), null);
			}
		}

		// Token: 0x0600148E RID: 5262 RVA: 0x00070A30 File Offset: 0x0006EC30
		public Vector2 HiddenPosition(int x, int y)
		{
			if (!this.selected)
			{
				return new Vector2(960f, base.Y) + new Vector2((float)x, (float)y) * new Vector2(1920f, 1080f);
			}
			return new Vector2(1920f, 1080f) / 2f + new Vector2((float)x, (float)y) * new Vector2(1920f, 1080f);
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x00070AB4 File Offset: 0x0006ECB4
		public void Show()
		{
			this.Visible = true;
			this.deleting = false;
			this.StartingGame = false;
			this.Renaming = false;
			this.Assisting = false;
			this.selectedEase = 0f;
			this.highlightEase = 0f;
			this.highlightEaseDelay = 0.35f;
			Vector2 from = this.Position;
			this.StartTween(0.25f, delegate(Tween f)
			{
				this.Position = Vector2.Lerp(from, this.IdlePosition, f.Eased);
			}, false);
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x00070B38 File Offset: 0x0006ED38
		public void Select(bool resetButtonIndex)
		{
			this.Visible = true;
			this.deleting = false;
			this.StartingGame = false;
			this.Renaming = false;
			this.Assisting = false;
			this.CreateButtons();
			this.Card.Play("shine", false, false);
			this.Ticket.Play("shine", false, false);
			Vector2 from = this.Position;
			this.wiggler.Start();
			if (resetButtonIndex)
			{
				this.buttonIndex = 0;
			}
			this.deleteIndex = 1;
			this.inputDelay = 0.1f;
			this.StartTween(0.25f, delegate(Tween f)
			{
				this.Position = Vector2.Lerp(from, this.SelectedPosition, this.selectedEase = f.Eased);
				this.newgameFade = Math.Max(this.newgameFade, f.Eased);
			}, false);
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x00070BEC File Offset: 0x0006EDEC
		public void Unselect()
		{
			Vector2 from = this.Position;
			this.buttonIndex = 0;
			this.StartTween(0.25f, delegate(Tween f)
			{
				this.selectedEase = 1f - f.Eased;
				this.newgameFade = 1f - f.Eased;
				this.Position = Vector2.Lerp(from, this.IdlePosition, f.Eased);
			}, false);
		}

		// Token: 0x06001492 RID: 5266 RVA: 0x00070C34 File Offset: 0x0006EE34
		public void MoveTo(float x, float y)
		{
			Vector2 from = this.Position;
			Vector2 to = new Vector2(x, y);
			this.StartTween(0.25f, delegate(Tween f)
			{
				this.Position = Vector2.Lerp(from, to, f.Eased);
			}, false);
		}

		// Token: 0x06001493 RID: 5267 RVA: 0x00070C80 File Offset: 0x0006EE80
		public void Hide(int x, int y)
		{
			Vector2 from = this.Position;
			Vector2 to = this.HiddenPosition(x, y);
			this.StartTween(0.25f, delegate(Tween f)
			{
				this.Position = Vector2.Lerp(from, to, f.Eased);
			}, true);
		}

		// Token: 0x06001494 RID: 5268 RVA: 0x00070CCC File Offset: 0x0006EECC
		private void StartTween(float duration, Action<Tween> callback, bool hide = false)
		{
			if (this.tween != null && this.tween.Entity == this)
			{
				this.tween.RemoveSelf();
			}
			base.Add(this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, duration, false));
			this.tween.OnUpdate = callback;
			this.tween.OnComplete = delegate(Tween t)
			{
				if (hide)
				{
					this.Visible = false;
				}
				this.tween = null;
			};
			this.tween.Start();
		}

		// Token: 0x06001495 RID: 5269 RVA: 0x00070D58 File Offset: 0x0006EF58
		public override void Update()
		{
			this.inputDelay -= Engine.DeltaTime;
			this.Ticket.Update();
			this.Card.Update();
			if (this.selected && this.fileSelect.Selected && this.fileSelect.Focused && !this.StartingGame && this.tween == null && this.inputDelay <= 0f && !this.StartingGame)
			{
				if (this.deleting)
				{
					if (Input.MenuCancel.Pressed)
					{
						this.deleting = false;
						this.wiggler.Start();
						Audio.Play("event:/ui/main/button_back");
					}
					else if (Input.MenuUp.Pressed && this.deleteIndex > 0)
					{
						this.deleteIndex = 0;
						this.wiggler.Start();
						Audio.Play("event:/ui/main/rollover_up");
					}
					else if (Input.MenuDown.Pressed && this.deleteIndex < 1)
					{
						this.deleteIndex = 1;
						this.wiggler.Start();
						Audio.Play("event:/ui/main/rollover_down");
					}
					else if (Input.MenuConfirm.Pressed)
					{
						if (this.deleteIndex == 1)
						{
							this.deleting = false;
							this.wiggler.Start();
							Audio.Play("event:/ui/main/button_back");
						}
						else if (SaveData.TryDelete(this.FileSlot))
						{
							this.Exists = false;
							this.Corrupted = false;
							this.deleting = false;
							this.deletingEase = 0f;
							this.fileSelect.UnselectHighlighted();
							Audio.Play("event:/ui/main/savefile_delete");
							if (!Settings.Instance.DisableFlashes)
							{
								this.screenFlash = 1f;
							}
							this.CreateButtons();
						}
						else
						{
							this.failedToDeleteEase = 0f;
							this.failedToDeleteTimer = 3f;
							Audio.Play("event:/ui/main/button_invalid");
						}
					}
				}
				else if (Input.MenuCancel.Pressed)
				{
					if (this.fileSelect.HasSlots)
					{
						this.fileSelect.UnselectHighlighted();
						Audio.Play("event:/ui/main/whoosh_savefile_in");
						Audio.Play("event:/ui/main/button_back");
					}
				}
				else if (Input.MenuUp.Pressed && this.buttonIndex > 0)
				{
					this.buttonIndex--;
					this.wiggler.Start();
					Audio.Play("event:/ui/main/rollover_up");
				}
				else if (Input.MenuDown.Pressed && this.buttonIndex < this.buttons.Count - 1)
				{
					this.buttonIndex++;
					this.wiggler.Start();
					Audio.Play("event:/ui/main/rollover_down");
				}
				else if (Input.MenuConfirm.Pressed)
				{
					this.buttons[this.buttonIndex].Action();
				}
			}
			if (this.highlightEaseDelay <= 0f)
			{
				this.highlightEase = Calc.Approach(this.highlightEase, (this.highlighted && (this.Exists || !this.selected)) ? 1f : 0f, Engine.DeltaTime * 4f);
			}
			else
			{
				this.highlightEaseDelay -= Engine.DeltaTime;
			}
			base.Depth = (this.highlighted ? -10 : 0);
			if (this.Renaming || this.Assisting)
			{
				this.selectedEase = Calc.Approach(this.selectedEase, 0f, Engine.DeltaTime * 4f);
			}
			this.deletingEase = Calc.Approach(this.deletingEase, this.deleting ? 1f : 0f, Engine.DeltaTime * 4f);
			this.failedToDeleteEase = Calc.Approach(this.failedToDeleteEase, (this.failedToDeleteTimer > 0f) ? 1f : 0f, Engine.DeltaTime * 4f);
			this.failedToDeleteTimer -= Engine.DeltaTime;
			this.screenFlash = Calc.Approach(this.screenFlash, 0f, Engine.DeltaTime * 4f);
			base.Update();
		}

		// Token: 0x06001496 RID: 5270 RVA: 0x00071184 File Offset: 0x0006F384
		public override void Render()
		{
			float scaleFactor = Ease.CubeInOut(this.highlightEase);
			float num = this.wiggler.Value * 8f;
			if (this.selectedEase > 0f)
			{
				Vector2 value = this.Position + new Vector2(0f, -150f + 350f * this.selectedEase);
				float lineHeight = ActiveFont.LineHeight;
				for (int i = 0; i < this.buttons.Count; i++)
				{
					OuiFileSelectSlot.Button button = this.buttons[i];
					Vector2 value2 = Vector2.UnitX * ((this.buttonIndex == i && !this.deleting) ? num : 0f);
					Color color = this.SelectionColor(this.buttonIndex == i && !this.deleting);
					ActiveFont.DrawOutline(button.Label, value + value2, new Vector2(0.5f, 0f), Vector2.One * button.Scale, color, 2f, Color.Black);
					value.Y += lineHeight * button.Scale + 15f;
				}
			}
			Vector2 vector = this.Position + Vector2.UnitX * scaleFactor * 360f;
			this.Ticket.RenderPosition = vector;
			this.Ticket.Render();
			if (this.highlightEase > 0f && this.Exists && !this.Corrupted)
			{
				int num2 = -280;
				int num3 = 600;
				for (int j = 0; j < this.Cassettes.Count; j++)
				{
					MTN.FileSelect[this.Cassettes[j] ? "cassette" : "dot"].DrawCentered(vector + new Vector2((float)num2 + ((float)j + 0.5f) * 75f, -75f));
					bool[] array = this.HeartGems[j];
					int num4 = 0;
					for (int k = 0; k < array.Length; k++)
					{
						if (array[k])
						{
							num4++;
						}
					}
					Vector2 vector2 = vector + new Vector2((float)num2 + ((float)j + 0.5f) * 75f, -12f);
					if (num4 == 0)
					{
						MTN.FileSelect["dot"].DrawCentered(vector2);
					}
					else
					{
						vector2.Y -= (float)(num4 - 1) * 0.5f * 14f;
						int l = 0;
						int num5 = 0;
						while (l < array.Length)
						{
							if (array[l])
							{
								MTN.FileSelect["heartgem" + l].DrawCentered(vector2 + new Vector2(0f, (float)(num5 * 14)));
								num5++;
							}
							l++;
						}
					}
				}
				this.Deaths.Position = vector + new Vector2((float)num2, 68f) - this.Position;
				this.Deaths.Render();
				ActiveFont.Draw(this.Time, vector + new Vector2((float)(num2 + num3), 68f), new Vector2(1f, 0.5f), Vector2.One * this.timeScale, Color.Black * 0.6f);
			}
			else if (this.Corrupted)
			{
				ActiveFont.Draw(Dialog.Clean("file_corrupted", null), vector, new Vector2(0.5f, 0.5f), Vector2.One, Color.Black * 0.8f);
			}
			else if (!this.Exists)
			{
				ActiveFont.Draw(Dialog.Clean("file_newgame", null), vector, new Vector2(0.5f, 0.5f), Vector2.One, Color.Black * 0.8f);
			}
			Vector2 vector3 = this.Position - Vector2.UnitX * scaleFactor * 360f;
			int num6 = 64;
			int num7 = 16;
			float num8 = this.Card.Width - (float)(num6 * 2) - 200f - (float)num7;
			float x = -this.Card.Width / 2f + (float)num6 + 200f + (float)num7 + num8 / 2f;
			float scale = this.Exists ? 1f : this.newgameFade;
			if (!this.Corrupted)
			{
				if (this.newgameFade > 0f || this.Exists)
				{
					if (this.AssistModeEnabled)
					{
						MTN.FileSelect["assist"].DrawCentered(vector3, Color.White * scale);
					}
					else if (this.VariantModeEnabled)
					{
						MTN.FileSelect["variants"].DrawCentered(vector3, Color.White * scale);
					}
				}
				if (this.Exists && this.SaveData.CheatMode)
				{
					MTN.FileSelect["cheatmode"].DrawCentered(vector3, Color.White * scale);
				}
			}
			this.Card.RenderPosition = vector3;
			this.Card.Render();
			if (!this.Corrupted)
			{
				if (this.Exists)
				{
					if (this.SaveData.TotalStrawberries >= 175)
					{
						MTN.FileSelect["strawberry"].DrawCentered(vector3, Color.White * scale);
					}
					if (this.SaveData.Areas.Count > 7 && this.SaveData.Areas[7].Modes[0].Completed)
					{
						MTN.FileSelect["flag"].DrawCentered(vector3, Color.White * scale);
					}
					if (this.SaveData.TotalCassettes >= 8)
					{
						MTN.FileSelect["cassettes"].DrawCentered(vector3, Color.White * scale);
					}
					if (this.SaveData.TotalHeartGems >= 16)
					{
						MTN.FileSelect["heart"].DrawCentered(vector3, Color.White * scale);
					}
					if (this.SaveData.TotalGoldenStrawberries >= 25)
					{
						MTN.FileSelect["goldberry"].DrawCentered(vector3, Color.White * scale);
					}
					if (this.SaveData.TotalHeartGems >= 24)
					{
						MTN.FileSelect["goldheart"].DrawCentered(vector3, Color.White * scale);
					}
					if (this.SaveData.Areas.Count > 10 && this.SaveData.Areas[10].Modes[0].Completed)
					{
						MTN.FileSelect["farewell"].DrawCentered(vector3, Color.White * scale);
					}
				}
				if (this.Exists || this.Renaming || this.newgameFade > 0f)
				{
					this.Portrait.RenderPosition = vector3 + new Vector2(-this.Card.Width / 2f + (float)num6 + 100f, 0f);
					this.Portrait.Color = Color.White * scale;
					this.Portrait.Render();
					MTN.FileSelect[(!this.Golden) ? "portraitOverlay" : "portraitOverlayGold"].DrawCentered(this.Portrait.RenderPosition, Color.White * scale);
					string name = this.Name;
					Vector2 vector4 = vector3 + new Vector2(x, (float)(-32 + (this.Exists ? 0 : 64)));
					float num9 = Math.Min(1f, 440f / ActiveFont.Measure(name).X);
					ActiveFont.Draw(name, vector4, new Vector2(0.5f, 1f), Vector2.One * num9, Color.Black * 0.8f * scale);
					if (this.Renaming && base.Scene.BetweenInterval(0.3f))
					{
						ActiveFont.Draw("|", new Vector2(vector4.X + ActiveFont.Measure(name).X * num9 * 0.5f, vector4.Y), new Vector2(0f, 1f), Vector2.One * num9, Color.Black * 0.8f * scale);
					}
				}
				if (this.Exists)
				{
					if (this.FurthestArea < AreaData.Areas.Count)
					{
						ActiveFont.Draw(Dialog.Clean(AreaData.Areas[this.FurthestArea].Name, null), vector3 + new Vector2(x, -10f), new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, Color.Black * 0.6f);
					}
					this.Strawberries.Position = vector3 + new Vector2(x, 55f) - this.Position;
					this.Strawberries.Render();
				}
			}
			else
			{
				ActiveFont.Draw(Dialog.Clean("file_failedtoload", null), vector3, new Vector2(0.5f, 0.5f), Vector2.One, Color.Black * 0.8f);
			}
			if (this.deletingEase > 0f)
			{
				float num10 = Ease.CubeOut(this.deletingEase);
				Vector2 value3 = new Vector2(960f, 540f);
				float lineHeight2 = ActiveFont.LineHeight;
				Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * num10 * 0.9f);
				ActiveFont.Draw(Dialog.Clean("file_delete_really", null), value3 + new Vector2(0f, -16f - 64f * (1f - num10)), new Vector2(0.5f, 1f), Vector2.One, Color.White * num10);
				ActiveFont.DrawOutline(Dialog.Clean("file_delete_yes", null), value3 + new Vector2(((this.deleting && this.deleteIndex == 0) ? num : 0f) * 1.2f * num10, 16f + 64f * (1f - num10)), new Vector2(0.5f, 0f), Vector2.One * 0.8f, this.deleting ? this.SelectionColor(this.deleteIndex == 0) : Color.Gray, 2f, Color.Black * num10);
				ActiveFont.DrawOutline(Dialog.Clean("file_delete_no", null), value3 + new Vector2(((this.deleting && this.deleteIndex == 1) ? num : 0f) * 1.2f * num10, 16f + lineHeight2 + 64f * (1f - num10)), new Vector2(0.5f, 0f), Vector2.One * 0.8f, this.deleting ? this.SelectionColor(this.deleteIndex == 1) : Color.Gray, 2f, Color.Black * num10);
				if (this.failedToDeleteEase > 0f)
				{
					Vector2 vector5 = new Vector2(960f, 980f - 100f * this.deletingEase);
					Vector2 scale2 = Vector2.One * 0.8f;
					if (this.failedToDeleteEase < 1f && this.failedToDeleteTimer > 0f)
					{
						vector5 += new Vector2((float)(-5 + Calc.Random.Next(10)), (float)(-5 + Calc.Random.Next(10)));
						scale2 = Vector2.One * (0.8f + 0.2f * (1f - this.failedToDeleteEase));
					}
					ActiveFont.DrawOutline(Dialog.Clean("file_delete_failed", null), vector5, new Vector2(0.5f, 0f), scale2, Color.PaleVioletRed * this.deletingEase, 2f, Color.Black * this.deletingEase);
				}
			}
			if (this.screenFlash > 0f)
			{
				Draw.Rect(-10f, -10f, 1940f, 1100f, Color.White * Ease.CubeOut(this.screenFlash));
			}
		}

		// Token: 0x06001497 RID: 5271 RVA: 0x00071E59 File Offset: 0x00070059
		public Color SelectionColor(bool selected)
		{
			if (!selected)
			{
				return Color.White;
			}
			if (!Settings.Instance.DisableFlashes && !base.Scene.BetweenInterval(0.1f))
			{
				return TextMenu.HighlightColorB;
			}
			return TextMenu.HighlightColorA;
		}

		// Token: 0x0400102C RID: 4140
		public SaveData SaveData;

		// Token: 0x0400102D RID: 4141
		public int FileSlot;

		// Token: 0x0400102E RID: 4142
		public string Name;

		// Token: 0x0400102F RID: 4143
		public bool AssistModeEnabled;

		// Token: 0x04001030 RID: 4144
		public bool VariantModeEnabled;

		// Token: 0x04001031 RID: 4145
		public bool Exists;

		// Token: 0x04001032 RID: 4146
		public bool Corrupted;

		// Token: 0x04001033 RID: 4147
		public string Time;

		// Token: 0x04001034 RID: 4148
		public int FurthestArea;

		// Token: 0x04001035 RID: 4149
		public Sprite Portrait;

		// Token: 0x04001036 RID: 4150
		public bool HasBlackgems;

		// Token: 0x04001037 RID: 4151
		public StrawberriesCounter Strawberries;

		// Token: 0x04001038 RID: 4152
		public DeathsCounter Deaths;

		// Token: 0x04001039 RID: 4153
		public List<bool> Cassettes = new List<bool>();

		// Token: 0x0400103A RID: 4154
		public List<bool[]> HeartGems = new List<bool[]>();

		// Token: 0x0400103B RID: 4155
		private const int height = 300;

		// Token: 0x0400103C RID: 4156
		private const int spacing = 10;

		// Token: 0x0400103D RID: 4157
		private const float portraitSize = 200f;

		// Token: 0x0400103E RID: 4158
		public bool StartingGame;

		// Token: 0x0400103F RID: 4159
		public bool Renaming;

		// Token: 0x04001040 RID: 4160
		public bool Assisting;

		// Token: 0x04001041 RID: 4161
		private OuiFileSelect fileSelect;

		// Token: 0x04001042 RID: 4162
		private bool deleting;

		// Token: 0x04001043 RID: 4163
		private float highlightEase;

		// Token: 0x04001044 RID: 4164
		private float highlightEaseDelay;

		// Token: 0x04001045 RID: 4165
		private float selectedEase;

		// Token: 0x04001046 RID: 4166
		private float deletingEase;

		// Token: 0x04001047 RID: 4167
		private Tween tween;

		// Token: 0x04001048 RID: 4168
		private int buttonIndex;

		// Token: 0x04001049 RID: 4169
		private int deleteIndex;

		// Token: 0x0400104A RID: 4170
		private Wiggler wiggler;

		// Token: 0x0400104B RID: 4171
		private float failedToDeleteEase;

		// Token: 0x0400104C RID: 4172
		private float failedToDeleteTimer;

		// Token: 0x0400104D RID: 4173
		private float screenFlash;

		// Token: 0x0400104E RID: 4174
		private float inputDelay;

		// Token: 0x0400104F RID: 4175
		private float newgameFade;

		// Token: 0x04001050 RID: 4176
		private float timeScale = 1f;

		// Token: 0x04001051 RID: 4177
		private OuiFileSelectSlot.Button assistButton;

		// Token: 0x04001052 RID: 4178
		private OuiFileSelectSlot.Button variantButton;

		// Token: 0x04001053 RID: 4179
		private Sprite normalCard;

		// Token: 0x04001054 RID: 4180
		private Sprite goldCard;

		// Token: 0x04001055 RID: 4181
		private Sprite normalTicket;

		// Token: 0x04001056 RID: 4182
		private Sprite goldTicket;

		// Token: 0x04001057 RID: 4183
		private List<OuiFileSelectSlot.Button> buttons = new List<OuiFileSelectSlot.Button>();

		// Token: 0x02000613 RID: 1555
		private class Button
		{
			// Token: 0x04002929 RID: 10537
			public string Label;

			// Token: 0x0400292A RID: 10538
			public Action Action;

			// Token: 0x0400292B RID: 10539
			public float Scale = 1f;
		}
	}
}
