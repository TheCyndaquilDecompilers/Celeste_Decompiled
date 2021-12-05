using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002FD RID: 765
	public class OuiFileSelect : Oui
	{
		// Token: 0x060017D9 RID: 6105 RVA: 0x00094850 File Offset: 0x00092A50
		public OuiFileSelect()
		{
			OuiFileSelect.Loaded = false;
		}

		// Token: 0x060017DA RID: 6106 RVA: 0x0009486A File Offset: 0x00092A6A
		public override IEnumerator Enter(Oui from)
		{
			this.SlotSelected = false;
			if (!OuiFileSelect.Loaded)
			{
				for (int j = 0; j < this.Slots.Length; j++)
				{
					if (this.Slots[j] != null)
					{
						base.Scene.Remove(this.Slots[j]);
					}
				}
				RunThread.Start(new Action(this.LoadThread), "FILE_LOADING", false);
				float elapsed = 0f;
				while (!OuiFileSelect.Loaded || elapsed < 0.5f)
				{
					elapsed += Engine.DeltaTime;
					yield return null;
				}
				for (int k = 0; k < this.Slots.Length; k++)
				{
					if (this.Slots[k] != null)
					{
						base.Scene.Add(this.Slots[k]);
					}
				}
				if (!this.loadedSuccess)
				{
					FileErrorOverlay error = new FileErrorOverlay(FileErrorOverlay.Error.Load);
					while (error.Open)
					{
						yield return null;
					}
					if (!error.Ignore)
					{
						base.Overworld.Goto<OuiMainMenu>();
						yield break;
					}
					error = null;
				}
			}
			else if (!(from is OuiFileNaming) && !(from is OuiAssistMode))
			{
				yield return 0.2f;
			}
			this.HasSlots = false;
			for (int l = 0; l < this.Slots.Length; l++)
			{
				if (this.Slots[l].Exists)
				{
					this.HasSlots = true;
				}
			}
			Audio.Play("event:/ui/main/whoosh_savefile_in");
			if (from is OuiFileNaming || from is OuiAssistMode)
			{
				if (!this.SlotSelected)
				{
					this.SelectSlot(false);
				}
			}
			else if (!this.HasSlots)
			{
				this.SlotIndex = 0;
				this.Slots[this.SlotIndex].Position = new Vector2(this.Slots[this.SlotIndex].HiddenPosition(1, 0).X, this.Slots[this.SlotIndex].SelectedPosition.Y);
				this.SelectSlot(true);
			}
			else if (!this.SlotSelected)
			{
				Alarm.Set(this, 0.4f, delegate
				{
					Audio.Play("event:/ui/main/savefile_rollover_first");
				}, Alarm.AlarmMode.Oneshot);
				int num;
				for (int i = 0; i < this.Slots.Length; i = num + 1)
				{
					this.Slots[i].Position = new Vector2(this.Slots[i].HiddenPosition(1, 0).X, this.Slots[i].IdlePosition.Y);
					this.Slots[i].Show();
					yield return 0.02f;
					num = i;
				}
			}
			yield break;
		}

		// Token: 0x060017DB RID: 6107 RVA: 0x00094880 File Offset: 0x00092A80
		private void LoadThread()
		{
			if (UserIO.Open(UserIO.Mode.Read))
			{
				for (int i = 0; i < this.Slots.Length; i++)
				{
					OuiFileSelectSlot ouiFileSelectSlot;
					if (!UserIO.Exists(SaveData.GetFilename(i)))
					{
						ouiFileSelectSlot = new OuiFileSelectSlot(i, this, false);
					}
					else
					{
						SaveData saveData = UserIO.Load<SaveData>(SaveData.GetFilename(i), false);
						if (saveData != null)
						{
							saveData.AfterInitialize();
							ouiFileSelectSlot = new OuiFileSelectSlot(i, this, saveData);
						}
						else
						{
							ouiFileSelectSlot = new OuiFileSelectSlot(i, this, true);
						}
					}
					this.Slots[i] = ouiFileSelectSlot;
				}
				UserIO.Close();
				this.loadedSuccess = true;
			}
			OuiFileSelect.Loaded = true;
		}

		// Token: 0x060017DC RID: 6108 RVA: 0x00094905 File Offset: 0x00092B05
		public override IEnumerator Leave(Oui next)
		{
			Audio.Play("event:/ui/main/whoosh_savefile_out");
			int slideTo = 1;
			if (next == null || next is OuiChapterSelect || next is OuiFileNaming || next is OuiAssistMode)
			{
				slideTo = -1;
			}
			int num;
			for (int i = 0; i < this.Slots.Length; i = num + 1)
			{
				if (next is OuiFileNaming && this.SlotIndex == i)
				{
					this.Slots[i].MoveTo(this.Slots[i].IdlePosition.X, this.Slots[0].IdlePosition.Y);
				}
				else if (next is OuiAssistMode && this.SlotIndex == i)
				{
					this.Slots[i].MoveTo(this.Slots[i].IdlePosition.X, -400f);
				}
				else
				{
					this.Slots[i].Hide(slideTo, 0);
				}
				yield return 0.02f;
				num = i;
			}
			yield break;
		}

		// Token: 0x060017DD RID: 6109 RVA: 0x0009491C File Offset: 0x00092B1C
		public void UnselectHighlighted()
		{
			this.SlotSelected = false;
			this.Slots[this.SlotIndex].Unselect();
			for (int i = 0; i < this.Slots.Length; i++)
			{
				if (this.SlotIndex != i)
				{
					this.Slots[i].Show();
				}
			}
		}

		// Token: 0x060017DE RID: 6110 RVA: 0x0009496C File Offset: 0x00092B6C
		public void SelectSlot(bool reset)
		{
			if (!this.Slots[this.SlotIndex].Exists && reset)
			{
				if (Settings.Instance != null && !string.IsNullOrWhiteSpace(Settings.Instance.DefaultFileName))
				{
					this.Slots[this.SlotIndex].Name = Settings.Instance.DefaultFileName;
				}
				else
				{
					this.Slots[this.SlotIndex].Name = Dialog.Clean("FILE_DEFAULT", null);
				}
				this.Slots[this.SlotIndex].AssistModeEnabled = false;
				this.Slots[this.SlotIndex].VariantModeEnabled = false;
			}
			this.SlotSelected = true;
			this.Slots[this.SlotIndex].Select(reset);
			for (int i = 0; i < this.Slots.Length; i++)
			{
				if (this.SlotIndex != i)
				{
					this.Slots[i].Hide(0, (i < this.SlotIndex) ? -1 : 1);
				}
			}
		}

		// Token: 0x060017DF RID: 6111 RVA: 0x00094A5C File Offset: 0x00092C5C
		public override void Update()
		{
			base.Update();
			if (this.Focused)
			{
				if (!this.SlotSelected)
				{
					if (Input.MenuUp.Pressed && this.SlotIndex > 0)
					{
						Audio.Play("event:/ui/main/savefile_rollover_up");
						this.SlotIndex--;
						return;
					}
					if (Input.MenuDown.Pressed && this.SlotIndex < this.Slots.Length - 1)
					{
						Audio.Play("event:/ui/main/savefile_rollover_down");
						this.SlotIndex++;
						return;
					}
					if (Input.MenuConfirm.Pressed)
					{
						Audio.Play("event:/ui/main/button_select");
						Audio.Play("event:/ui/main/whoosh_savefile_out");
						this.SelectSlot(true);
						return;
					}
					if (Input.MenuCancel.Pressed)
					{
						Audio.Play("event:/ui/main/button_back");
						base.Overworld.Goto<OuiMainMenu>();
						return;
					}
				}
				else if (Input.MenuCancel.Pressed && !this.HasSlots && !this.Slots[this.SlotIndex].StartingGame)
				{
					Audio.Play("event:/ui/main/button_back");
					base.Overworld.Goto<OuiMainMenu>();
				}
			}
		}

		// Token: 0x040014A2 RID: 5282
		public OuiFileSelectSlot[] Slots = new OuiFileSelectSlot[3];

		// Token: 0x040014A3 RID: 5283
		public int SlotIndex;

		// Token: 0x040014A4 RID: 5284
		public bool SlotSelected;

		// Token: 0x040014A5 RID: 5285
		public static bool Loaded;

		// Token: 0x040014A6 RID: 5286
		private bool loadedSuccess;

		// Token: 0x040014A7 RID: 5287
		public bool HasSlots;
	}
}
