using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x020002FA RID: 762
	public class OuiChapterPanel : Oui
	{
		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06001795 RID: 6037 RVA: 0x00091539 File Offset: 0x0008F739
		public Vector2 OpenPosition
		{
			get
			{
				return new Vector2(1070f, 100f);
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06001796 RID: 6038 RVA: 0x0009154A File Offset: 0x0008F74A
		public Vector2 ClosePosition
		{
			get
			{
				return new Vector2(2220f, 100f);
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06001797 RID: 6039 RVA: 0x0009155B File Offset: 0x0008F75B
		public Vector2 IconOffset
		{
			get
			{
				return new Vector2(690f, 86f);
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06001798 RID: 6040 RVA: 0x0009156C File Offset: 0x0008F76C
		private Vector2 OptionsRenderPosition
		{
			get
			{
				return this.Position + new Vector2(this.contentOffset.X, 128f + this.height);
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06001799 RID: 6041 RVA: 0x00091595 File Offset: 0x0008F795
		// (set) Token: 0x0600179A RID: 6042 RVA: 0x000915B1 File Offset: 0x0008F7B1
		private int option
		{
			get
			{
				if (!this.selectingMode)
				{
					return this.checkpoint;
				}
				return (int)this.Area.Mode;
			}
			set
			{
				if (this.selectingMode)
				{
					this.Area.Mode = (AreaMode)value;
					return;
				}
				this.checkpoint = value;
			}
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x0600179B RID: 6043 RVA: 0x000915CF File Offset: 0x0008F7CF
		private List<OuiChapterPanel.Option> options
		{
			get
			{
				if (!this.selectingMode)
				{
					return this.checkpoints;
				}
				return this.modes;
			}
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x000915E8 File Offset: 0x0008F7E8
		public OuiChapterPanel()
		{
			base.Add(this.strawberries);
			base.Add(this.deaths);
			base.Add(this.heart);
			this.deaths.CanWiggle = false;
			this.strawberries.CanWiggle = false;
			this.strawberries.OverworldSfx = true;
			base.Add(this.wiggler = Wiggler.Create(0.4f, 4f, null, false, false));
			base.Add(this.modeAppearWiggler = Wiggler.Create(0.4f, 4f, null, false, false));
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x000916E4 File Offset: 0x0008F8E4
		public override bool IsStart(Overworld overworld, Overworld.StartMode start)
		{
			if (start == Overworld.StartMode.AreaComplete || start == Overworld.StartMode.AreaQuit)
			{
				bool shouldAdvance = start == Overworld.StartMode.AreaComplete && (Celeste.PlayMode == Celeste.PlayModes.Event || (SaveData.Instance.CurrentSession != null && SaveData.Instance.CurrentSession.ShouldAdvance));
				this.Position = this.OpenPosition;
				this.Reset();
				base.Add(new Coroutine(this.IncrementStats(shouldAdvance), true));
				overworld.ShowInputUI = false;
				overworld.Mountain.SnapState(this.Data.MountainState);
				overworld.Mountain.SnapCamera(this.Area.ID, this.Data.MountainZoom, false);
				overworld.Mountain.EaseCamera(this.Area.ID, this.Data.MountainSelect, new float?(1f), true, false);
				this.OverworldStartMode = start;
				return true;
			}
			this.Position = this.ClosePosition;
			return false;
		}

		// Token: 0x0600179E RID: 6046 RVA: 0x000917D5 File Offset: 0x0008F9D5
		public override IEnumerator Enter(Oui from)
		{
			this.Visible = true;
			this.Area.Mode = AreaMode.Normal;
			this.Reset();
			base.Overworld.Mountain.EaseCamera(this.Area.ID, this.Data.MountainSelect, null, true, false);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
			{
				yield return null;
				this.Position = this.ClosePosition + (this.OpenPosition - this.ClosePosition) * Ease.CubeOut(p);
			}
			this.Position = this.OpenPosition;
			yield break;
		}

		// Token: 0x0600179F RID: 6047 RVA: 0x000917E4 File Offset: 0x0008F9E4
		private void Reset()
		{
			this.Area = SaveData.Instance.LastArea;
			this.Data = AreaData.Areas[this.Area.ID];
			this.RealStats = SaveData.Instance.Areas[this.Area.ID];
			if (SaveData.Instance.CurrentSession != null && SaveData.Instance.CurrentSession.OldStats != null && SaveData.Instance.CurrentSession.Area.ID == this.Area.ID)
			{
				this.DisplayedStats = SaveData.Instance.CurrentSession.OldStats;
				SaveData.Instance.CurrentSession = null;
			}
			else
			{
				this.DisplayedStats = this.RealStats;
			}
			this.height = (float)this.GetModeHeight();
			this.modes.Clear();
			bool flag = false;
			if (!this.Data.Interlude && this.Data.HasMode(AreaMode.BSide) && (this.DisplayedStats.Cassette || ((SaveData.Instance.DebugMode || SaveData.Instance.CheatMode) && this.DisplayedStats.Cassette == this.RealStats.Cassette)))
			{
				flag = true;
			}
			bool flag2 = !this.Data.Interlude && this.Data.HasMode(AreaMode.CSide) && SaveData.Instance.UnlockedModes >= 3 && Celeste.PlayMode != Celeste.PlayModes.Event;
			this.modes.Add(new OuiChapterPanel.Option
			{
				Label = Dialog.Clean(this.Data.Interlude ? "FILE_BEGIN" : "overworld_normal", null).ToUpper(),
				Icon = GFX.Gui["menu/play"],
				ID = "A"
			});
			if (flag)
			{
				this.AddRemixButton();
			}
			if (flag2)
			{
				this.modes.Add(new OuiChapterPanel.Option
				{
					Label = Dialog.Clean("overworld_remix2", null),
					Icon = GFX.Gui["menu/rmx2"],
					ID = "C"
				});
			}
			this.selectingMode = true;
			this.UpdateStats(false, null, null, null);
			this.SetStatsPosition(false);
			for (int i = 0; i < this.options.Count; i++)
			{
				this.options[i].SlideTowards(i, this.options.Count, true);
			}
			this.chapter = Dialog.Get("area_chapter", null).Replace("{x}", this.Area.ChapterIndex.ToString().PadLeft(2));
			this.contentOffset = new Vector2(440f, 120f);
			this.initialized = true;
		}

		// Token: 0x060017A0 RID: 6048 RVA: 0x00091AB0 File Offset: 0x0008FCB0
		private int GetModeHeight()
		{
			AreaModeStats areaModeStats = this.RealStats.Modes[(int)this.Area.Mode];
			bool flag = areaModeStats.Strawberries.Count <= 0;
			if (!this.Data.Interlude && ((areaModeStats.Deaths > 0 && this.Area.Mode != AreaMode.Normal) || areaModeStats.Completed || areaModeStats.HeartGem))
			{
				flag = false;
			}
			if (!flag)
			{
				return 540;
			}
			return 300;
		}

		// Token: 0x060017A1 RID: 6049 RVA: 0x00091B2C File Offset: 0x0008FD2C
		private OuiChapterPanel.Option AddRemixButton()
		{
			OuiChapterPanel.Option option = new OuiChapterPanel.Option
			{
				Label = Dialog.Clean("overworld_remix", null),
				Icon = GFX.Gui["menu/remix"],
				ID = "B"
			};
			this.modes.Insert(1, option);
			return option;
		}

		// Token: 0x060017A2 RID: 6050 RVA: 0x00091B7E File Offset: 0x0008FD7E
		public override IEnumerator Leave(Oui next)
		{
			base.Overworld.Mountain.EaseCamera(this.Area.ID, this.Data.MountainIdle, null, true, false);
			base.Add(new Coroutine(this.EaseOut(true), true));
			yield break;
		}

		// Token: 0x060017A3 RID: 6051 RVA: 0x00091B8D File Offset: 0x0008FD8D
		public IEnumerator EaseOut(bool removeChildren = true)
		{
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
			{
				this.Position = this.OpenPosition + (this.ClosePosition - this.OpenPosition) * Ease.CubeIn(p);
				yield return null;
			}
			if (!base.Selected)
			{
				this.Visible = false;
			}
			yield break;
		}

		// Token: 0x060017A4 RID: 6052 RVA: 0x00091B9C File Offset: 0x0008FD9C
		public void Start(string checkpoint = null)
		{
			this.Focused = false;
			Audio.Play("event:/ui/world_map/chapter/checkpoint_start");
			base.Add(new Coroutine(this.StartRoutine(checkpoint), true));
		}

		// Token: 0x060017A5 RID: 6053 RVA: 0x00091BC3 File Offset: 0x0008FDC3
		private IEnumerator StartRoutine(string checkpoint = null)
		{
			this.EnteringChapter = true;
			base.Overworld.Maddy.Hide(false);
			base.Overworld.Mountain.EaseCamera(this.Area.ID, this.Data.MountainZoom, new float?(1f), true, false);
			base.Add(new Coroutine(this.EaseOut(false), true));
			yield return 0.2f;
			ScreenWipe.WipeColor = Color.Black;
			AreaData.Get(this.Area).Wipe(base.Overworld, false, null);
			Audio.SetMusic(null, true, true);
			Audio.SetAmbience(null, true);
			if ((this.Area.ID == 0 || this.Area.ID == 9) && checkpoint == null && this.Area.Mode == AreaMode.Normal)
			{
				base.Overworld.RendererList.UpdateLists();
				base.Overworld.RendererList.MoveToFront(base.Overworld.Snow);
			}
			yield return 0.5f;
			LevelEnter.Go(new Session(this.Area, checkpoint, null), false);
			yield break;
		}

		// Token: 0x060017A6 RID: 6054 RVA: 0x00091BD9 File Offset: 0x0008FDD9
		private void Swap()
		{
			this.Focused = false;
			base.Overworld.ShowInputUI = !this.selectingMode;
			base.Add(new Coroutine(this.SwapRoutine(), true));
		}

		// Token: 0x060017A7 RID: 6055 RVA: 0x00091C08 File Offset: 0x0008FE08
		private IEnumerator SwapRoutine()
		{
			float fromHeight = this.height;
			int toHeight = this.selectingMode ? 730 : this.GetModeHeight();
			this.resizing = true;
			this.PlayExpandSfx(fromHeight, (float)toHeight);
			float offset = 800f;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
			{
				yield return null;
				this.contentOffset.X = 440f + offset * Ease.CubeIn(p);
				this.height = MathHelper.Lerp(fromHeight, (float)toHeight, Ease.CubeOut(p * 0.5f));
			}
			this.selectingMode = !this.selectingMode;
			if (!this.selectingMode)
			{
				HashSet<string> hashSet = SaveData.Instance.GetCheckpoints(this.Area);
				int siblings = hashSet.Count + 1;
				this.checkpoints.Clear();
				this.checkpoints.Add(new OuiChapterPanel.Option
				{
					Label = Dialog.Clean("overworld_start", null),
					BgColor = Calc.HexToColor("eabe26"),
					Icon = GFX.Gui["areaselect/startpoint"],
					CheckpointLevelName = null,
					CheckpointRotation = (float)Calc.Random.Choose(-1, 1) * Calc.Random.Range(0.05f, 0.2f),
					CheckpointOffset = new Vector2((float)Calc.Random.Range(-16, 16), (float)Calc.Random.Range(-16, 16)),
					Large = false,
					Siblings = siblings
				});
				foreach (string text in hashSet)
				{
					this.checkpoints.Add(new OuiChapterPanel.Option
					{
						Label = AreaData.GetCheckpointName(this.Area, text),
						Icon = GFX.Gui["areaselect/checkpoint"],
						CheckpointLevelName = text,
						CheckpointRotation = (float)Calc.Random.Choose(-1, 1) * Calc.Random.Range(0.05f, 0.2f),
						CheckpointOffset = new Vector2((float)Calc.Random.Range(-16, 16), (float)Calc.Random.Range(-16, 16)),
						Large = false,
						Siblings = siblings
					});
				}
				if (!this.RealStats.Modes[(int)this.Area.Mode].Completed && !SaveData.Instance.DebugMode && !SaveData.Instance.CheatMode)
				{
					this.option = this.checkpoints.Count - 1;
					for (int i = 0; i < this.checkpoints.Count - 1; i++)
					{
						this.options[i].CheckpointSlideOut = 1f;
					}
				}
				else
				{
					this.option = 0;
				}
				for (int j = 0; j < this.options.Count; j++)
				{
					this.options[j].SlideTowards(j, this.options.Count, true);
				}
			}
			this.options[this.option].Pop = 1f;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
			{
				yield return null;
				this.height = MathHelper.Lerp(fromHeight, (float)toHeight, Ease.CubeOut(Math.Min(1f, 0.5f + p * 0.5f)));
				this.contentOffset.X = 440f + offset * (1f - Ease.CubeOut(p));
			}
			this.contentOffset.X = 440f;
			this.height = (float)toHeight;
			this.Focused = true;
			this.resizing = false;
			yield break;
		}

		// Token: 0x060017A8 RID: 6056 RVA: 0x00091C18 File Offset: 0x0008FE18
		public override void Update()
		{
			if (!this.initialized)
			{
				return;
			}
			base.Update();
			for (int i = 0; i < this.options.Count; i++)
			{
				OuiChapterPanel.Option option = this.options[i];
				option.Pop = Calc.Approach(option.Pop, (this.option == i) ? 1f : 0f, Engine.DeltaTime * 4f);
				option.Appear = Calc.Approach(option.Appear, 1f, Engine.DeltaTime * 3f);
				option.CheckpointSlideOut = Calc.Approach(option.CheckpointSlideOut, (float)((this.option > i) ? 1 : 0), Engine.DeltaTime * 4f);
				option.Faded = Calc.Approach(option.Faded, (float)((this.option == i || option.Appeared) ? 0 : 1), Engine.DeltaTime * 4f);
				option.SlideTowards(i, this.options.Count, false);
			}
			if (this.selectingMode && !this.resizing)
			{
				this.height = Calc.Approach(this.height, (float)this.GetModeHeight(), Engine.DeltaTime * 1600f);
			}
			if (base.Selected && this.Focused)
			{
				if (Input.MenuLeft.Pressed && this.option > 0)
				{
					Audio.Play("event:/ui/world_map/chapter/tab_roll_left");
					int option2 = this.option;
					this.option = option2 - 1;
					this.wiggler.Start();
					if (this.selectingMode)
					{
						this.UpdateStats(true, null, null, null);
						this.PlayExpandSfx(this.height, (float)this.GetModeHeight());
					}
					else
					{
						Audio.Play("event:/ui/world_map/chapter/checkpoint_photo_add");
					}
				}
				else if (Input.MenuRight.Pressed && this.option + 1 < this.options.Count)
				{
					Audio.Play("event:/ui/world_map/chapter/tab_roll_right");
					int option2 = this.option;
					this.option = option2 + 1;
					this.wiggler.Start();
					if (this.selectingMode)
					{
						this.UpdateStats(true, null, null, null);
						this.PlayExpandSfx(this.height, (float)this.GetModeHeight());
					}
					else
					{
						Audio.Play("event:/ui/world_map/chapter/checkpoint_photo_remove");
					}
				}
				else if (Input.MenuConfirm.Pressed)
				{
					if (this.selectingMode)
					{
						if (!SaveData.Instance.FoundAnyCheckpoints(this.Area))
						{
							this.Start(null);
						}
						else
						{
							Audio.Play("event:/ui/world_map/chapter/level_select");
							this.Swap();
						}
					}
					else
					{
						this.Start(this.options[this.option].CheckpointLevelName);
					}
				}
				else if (Input.MenuCancel.Pressed)
				{
					if (this.selectingMode)
					{
						Audio.Play("event:/ui/world_map/chapter/back");
						base.Overworld.Goto<OuiChapterSelect>();
					}
					else
					{
						Audio.Play("event:/ui/world_map/chapter/checkpoint_back");
						this.Swap();
					}
				}
			}
			this.SetStatsPosition(true);
		}

		// Token: 0x060017A9 RID: 6057 RVA: 0x00091F38 File Offset: 0x00090138
		public override void Render()
		{
			if (!this.initialized)
			{
				return;
			}
			Vector2 optionsRenderPosition = this.OptionsRenderPosition;
			for (int i = 0; i < this.options.Count; i++)
			{
				if (!this.options[i].OnTopOfUI)
				{
					this.options[i].Render(optionsRenderPosition, this.option == i, this.wiggler, this.modeAppearWiggler);
				}
			}
			bool flag = false;
			if (this.RealStats.Modes[(int)this.Area.Mode].Completed)
			{
				int mode = (int)this.Area.Mode;
				foreach (EntityData entityData in AreaData.Areas[this.Area.ID].Mode[mode].MapData.Goldenberries)
				{
					EntityID item = new EntityID(entityData.Level.Name, entityData.ID);
					if (this.RealStats.Modes[mode].Strawberries.Contains(item))
					{
						flag = true;
						break;
					}
				}
			}
			MTexture mtexture = GFX.Gui[(!flag) ? "areaselect/cardtop" : "areaselect/cardtop_golden"];
			mtexture.Draw(this.Position + new Vector2(0f, -32f));
			MTexture mtexture2 = GFX.Gui[(!flag) ? "areaselect/card" : "areaselect/card_golden"];
			this.card = mtexture2.GetSubtexture(0, mtexture2.Height - (int)this.height, mtexture2.Width, (int)this.height, this.card);
			this.card.Draw(this.Position + new Vector2(0f, (float)(-32 + mtexture.Height)));
			for (int j = 0; j < this.options.Count; j++)
			{
				if (this.options[j].OnTopOfUI)
				{
					this.options[j].Render(optionsRenderPosition, this.option == j, this.wiggler, this.modeAppearWiggler);
				}
			}
			ActiveFont.Draw(this.options[this.option].Label, optionsRenderPosition + new Vector2(0f, -140f), Vector2.One * 0.5f, Vector2.One * (1f + this.wiggler.Value * 0.1f), Color.Black * 0.8f);
			if (this.selectingMode)
			{
				this.strawberries.Position = this.contentOffset + new Vector2(0f, 170f) + this.strawberriesOffset;
				this.deaths.Position = this.contentOffset + new Vector2(0f, 170f) + this.deathsOffset;
				this.heart.Position = this.contentOffset + new Vector2(0f, 170f) + this.heartOffset;
				base.Render();
			}
			if (!this.selectingMode)
			{
				Vector2 center = this.Position + new Vector2(this.contentOffset.X, 340f);
				for (int k = this.options.Count - 1; k >= 0; k--)
				{
					this.DrawCheckpoint(center, this.options[k], k);
				}
			}
			GFX.Gui["areaselect/title"].Draw(this.Position + new Vector2(-60f, 0f), Vector2.Zero, this.Data.TitleBaseColor);
			GFX.Gui["areaselect/accent"].Draw(this.Position + new Vector2(-60f, 0f), Vector2.Zero, this.Data.TitleAccentColor);
			string text = Dialog.Clean(AreaData.Get(this.Area).Name, null);
			if (this.Data.Interlude)
			{
				ActiveFont.Draw(text, this.Position + this.IconOffset + new Vector2(-100f, 0f), new Vector2(1f, 0.5f), Vector2.One * 1f, this.Data.TitleTextColor * 0.8f);
			}
			else
			{
				ActiveFont.Draw(this.chapter, this.Position + this.IconOffset + new Vector2(-100f, -2f), new Vector2(1f, 1f), Vector2.One * 0.6f, this.Data.TitleAccentColor * 0.8f);
				ActiveFont.Draw(text, this.Position + this.IconOffset + new Vector2(-100f, -18f), new Vector2(1f, 0f), Vector2.One * 1f, this.Data.TitleTextColor * 0.8f);
			}
			if (this.spotlightAlpha > 0f)
			{
				HiresRenderer.EndRender();
				SpotlightWipe.DrawSpotlight(this.spotlightPosition, this.spotlightRadius, Color.Black * this.spotlightAlpha);
				HiresRenderer.BeginRender(null, null);
			}
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x000924EC File Offset: 0x000906EC
		private void DrawCheckpoint(Vector2 center, OuiChapterPanel.Option option, int checkpointIndex)
		{
			MTexture checkpointPreview = this.GetCheckpointPreview(this.Area, option.CheckpointLevelName);
			MTexture mtexture = MTN.Checkpoints["polaroid"];
			float checkpointRotation = option.CheckpointRotation;
			Vector2 vector = center + option.CheckpointOffset;
			vector += Vector2.UnitX * 800f * Ease.CubeIn(option.CheckpointSlideOut);
			mtexture.DrawCentered(vector, Color.White, 0.75f, checkpointRotation);
			MTexture mtexture2 = GFX.Gui["collectables/strawberry"];
			if (checkpointPreview != null)
			{
				Vector2 vector2 = Vector2.One * 0.75f;
				if (SaveData.Instance.Assists.MirrorMode)
				{
					vector2.X = -vector2.X;
				}
				vector2 *= 720f / (float)checkpointPreview.Width;
				HiresRenderer.EndRender();
				HiresRenderer.BeginRender(BlendState.AlphaBlend, SamplerState.PointClamp);
				checkpointPreview.DrawCentered(vector, Color.White, vector2, checkpointRotation);
				HiresRenderer.EndRender();
				HiresRenderer.BeginRender(null, null);
			}
			int mode = (int)this.Area.Mode;
			if (this.RealStats.Modes[mode].Completed || SaveData.Instance.CheatMode || SaveData.Instance.DebugMode)
			{
				Vector2 vector3 = new Vector2(300f, 220f);
				vector3 = vector + vector3.Rotate(checkpointRotation);
				int num = 0;
				if (checkpointIndex == 0)
				{
					num = this.Data.Mode[mode].StartStrawberries;
				}
				else
				{
					num = this.Data.Mode[mode].Checkpoints[checkpointIndex - 1].Strawberries;
				}
				bool[] array = new bool[num];
				foreach (EntityID entityID in this.RealStats.Modes[mode].Strawberries)
				{
					for (int i = 0; i < num; i++)
					{
						EntityData entityData = this.Data.Mode[mode].StrawberriesByCheckpoint[checkpointIndex, i];
						if (entityData != null && entityData.Level.Name == entityID.Level && entityData.ID == entityID.ID)
						{
							array[i] = true;
						}
					}
				}
				Vector2 value = Calc.AngleToVector(checkpointRotation, 1f);
				Vector2 vector4 = vector3 - value * (float)num * 44f;
				if (this.Area.Mode == AreaMode.Normal && this.Data.CassetteCheckpointIndex == checkpointIndex)
				{
					Vector2 position = vector4 - value * 60f;
					if (this.RealStats.Cassette)
					{
						MTN.Journal["cassette"].DrawCentered(position, Color.White, 1f, checkpointRotation);
					}
					else
					{
						MTN.Journal["cassette_outline"].DrawCentered(position, Color.DarkGray, 1f, checkpointRotation);
					}
				}
				for (int j = 0; j < num; j++)
				{
					mtexture2.DrawCentered(vector4, array[j] ? Color.White : (Color.Black * 0.3f), 0.5f, checkpointRotation);
					vector4 += value * 44f;
				}
			}
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x00092844 File Offset: 0x00090A44
		private void UpdateStats(bool wiggle = true, bool? overrideStrawberryWiggle = null, bool? overrideDeathWiggle = null, bool? overrideHeartWiggle = null)
		{
			AreaModeStats areaModeStats = this.DisplayedStats.Modes[(int)this.Area.Mode];
			AreaData areaData = AreaData.Get(this.Area);
			this.deaths.Visible = (areaModeStats.Deaths > 0 && (this.Area.Mode != AreaMode.Normal || this.RealStats.Modes[(int)this.Area.Mode].Completed) && !AreaData.Get(this.Area).Interlude);
			this.deaths.Amount = areaModeStats.Deaths;
			this.deaths.SetMode(areaData.IsFinal ? AreaMode.CSide : this.Area.Mode);
			this.heart.Visible = (areaModeStats.HeartGem && !areaData.Interlude && areaData.CanFullClear);
			this.heart.SetCurrentMode(this.Area.Mode, areaModeStats.HeartGem);
			this.strawberries.Visible = ((areaModeStats.TotalStrawberries > 0 || (areaModeStats.Completed && this.Area.Mode == AreaMode.Normal && AreaData.Get(this.Area).Mode[0].TotalStrawberries > 0)) && !AreaData.Get(this.Area).Interlude);
			this.strawberries.Amount = areaModeStats.TotalStrawberries;
			this.strawberries.OutOf = this.Data.Mode[0].TotalStrawberries;
			this.strawberries.ShowOutOf = (areaModeStats.Completed && this.Area.Mode == AreaMode.Normal);
			this.strawberries.Golden = (this.Area.Mode > AreaMode.Normal);
			if (wiggle)
			{
				if (this.strawberries.Visible && (overrideStrawberryWiggle == null || overrideStrawberryWiggle.Value))
				{
					this.strawberries.Wiggle();
				}
				if (this.heart.Visible && (overrideHeartWiggle == null || overrideHeartWiggle.Value))
				{
					this.heart.Wiggle();
				}
				if (this.deaths.Visible && (overrideDeathWiggle == null || overrideDeathWiggle.Value))
				{
					this.deaths.Wiggle();
				}
			}
		}

		// Token: 0x060017AC RID: 6060 RVA: 0x00092A84 File Offset: 0x00090C84
		private void SetStatsPosition(bool approach)
		{
			if (this.heart.Visible && (this.strawberries.Visible || this.deaths.Visible))
			{
				this.heartOffset = this.Approach(this.heartOffset, new Vector2(-120f, 0f), !approach);
				this.strawberriesOffset = this.Approach(this.strawberriesOffset, new Vector2(120f, (float)(this.deaths.Visible ? -40 : 0)), !approach);
				this.deathsOffset = this.Approach(this.deathsOffset, new Vector2(120f, (float)(this.strawberries.Visible ? 40 : 0)), !approach);
				return;
			}
			if (this.heart.Visible)
			{
				this.heartOffset = this.Approach(this.heartOffset, Vector2.Zero, !approach);
				return;
			}
			this.strawberriesOffset = this.Approach(this.strawberriesOffset, new Vector2(0f, (float)(this.deaths.Visible ? -40 : 0)), !approach);
			this.deathsOffset = this.Approach(this.deathsOffset, new Vector2(0f, (float)(this.strawberries.Visible ? 40 : 0)), !approach);
		}

		// Token: 0x060017AD RID: 6061 RVA: 0x00092BD9 File Offset: 0x00090DD9
		private Vector2 Approach(Vector2 from, Vector2 to, bool snap)
		{
			if (snap)
			{
				return to;
			}
			return from += (to - from) * (1f - (float)Math.Pow(0.0010000000474974513, (double)Engine.DeltaTime));
		}

		// Token: 0x060017AE RID: 6062 RVA: 0x00092C10 File Offset: 0x00090E10
		private IEnumerator IncrementStatsDisplay(AreaModeStats modeStats, AreaModeStats newModeStats, bool doHeartGem, bool doStrawberries, bool doDeaths, bool doRemixUnlock)
		{
			if (doHeartGem)
			{
				Audio.Play("event:/ui/postgame/crystal_heart");
				this.heart.Visible = true;
				this.heart.SetCurrentMode(this.Area.Mode, true);
				this.heart.Appear(this.Area.Mode);
				yield return 1.8f;
			}
			if (doStrawberries)
			{
				this.strawberries.CanWiggle = true;
				this.strawberries.Visible = true;
				while (newModeStats.TotalStrawberries > modeStats.TotalStrawberries)
				{
					int num = newModeStats.TotalStrawberries - modeStats.TotalStrawberries;
					if (num < 3)
					{
						yield return 0.3f;
					}
					else if (num < 8)
					{
						yield return 0.2f;
					}
					else
					{
						yield return 0.1f;
						modeStats.TotalStrawberries++;
					}
					modeStats.TotalStrawberries++;
					this.strawberries.Amount = modeStats.TotalStrawberries;
					Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
				}
				this.strawberries.CanWiggle = false;
				yield return 0.5f;
				if (newModeStats.Completed && !modeStats.Completed && this.Area.Mode == AreaMode.Normal)
				{
					yield return 0.25f;
					Audio.Play((this.strawberries.Amount >= this.Data.Mode[0].TotalStrawberries) ? "event:/ui/postgame/strawberry_total_all" : "event:/ui/postgame/strawberry_total");
					this.strawberries.OutOf = this.Data.Mode[0].TotalStrawberries;
					this.strawberries.ShowOutOf = true;
					this.strawberries.Wiggle();
					modeStats.Completed = true;
					Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
					yield return 0.6f;
				}
			}
			if (doDeaths)
			{
				Audio.Play("event:/ui/postgame/death_appear");
				this.deaths.CanWiggle = true;
				this.deaths.Visible = true;
				while (newModeStats.Deaths > modeStats.Deaths)
				{
					int add;
					yield return this.HandleDeathTick(modeStats.Deaths, newModeStats.Deaths, out add);
					modeStats.Deaths += add;
					this.deaths.Amount = modeStats.Deaths;
					if (modeStats.Deaths >= newModeStats.Deaths)
					{
						Audio.Play("event:/ui/postgame/death_final");
					}
					else
					{
						Audio.Play("event:/ui/postgame/death_count");
					}
					Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
				}
				yield return 0.8f;
				this.deaths.CanWiggle = false;
			}
			if (doRemixUnlock)
			{
				this.bSideUnlockSfx = Audio.Play("event:/ui/postgame/unlock_bside");
				OuiChapterPanel.Option o = this.AddRemixButton();
				o.Appear = 0f;
				o.IconEase = 0f;
				o.Appeared = true;
				yield return 0.5f;
				this.spotlightPosition = o.GetRenderPosition(this.OptionsRenderPosition);
				for (float t = 0f; t < 1f; t += Engine.DeltaTime / 0.5f)
				{
					this.spotlightAlpha = t * 0.9f;
					this.spotlightRadius = 128f * Ease.CubeOut(t);
					yield return null;
				}
				yield return 0.3f;
				while ((o.IconEase += Engine.DeltaTime * 2f) < 1f)
				{
					yield return null;
				}
				o.IconEase = 1f;
				this.modeAppearWiggler.Start();
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
				this.remixUnlockText = new AreaCompleteTitle(this.spotlightPosition + new Vector2(0f, 80f), Dialog.Clean("OVERWORLD_REMIX_UNLOCKED", null), 1f, false);
				this.remixUnlockText.Tag = Tags.HUD;
				base.Overworld.Add(this.remixUnlockText);
				yield return 1.5f;
				for (float t = 0f; t < 1f; t += Engine.DeltaTime / 0.5f)
				{
					this.spotlightAlpha = (1f - t) * 0.5f;
					this.spotlightRadius = 128f + 128f * Ease.CubeOut(t);
					this.remixUnlockText.Alpha = 1f - Ease.CubeOut(t);
					yield return null;
				}
				this.remixUnlockText.RemoveSelf();
				this.remixUnlockText = null;
				o.Appeared = false;
				o = null;
			}
			yield break;
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x00092C4C File Offset: 0x00090E4C
		public IEnumerator IncrementStats(bool shouldAdvance)
		{
			this.Focused = false;
			base.Overworld.ShowInputUI = false;
			if (this.Data.Interlude)
			{
				if (shouldAdvance && this.OverworldStartMode == Overworld.StartMode.AreaComplete)
				{
					yield return 1.2f;
					base.Overworld.Goto<OuiChapterSelect>().AdvanceToNext();
				}
				else
				{
					this.Focused = true;
				}
				yield return null;
				yield break;
			}
			AreaData data = this.Data;
			AreaStats displayedStats = this.DisplayedStats;
			AreaStats areaStats = SaveData.Instance.Areas[data.ID];
			AreaModeStats modeStats = displayedStats.Modes[(int)this.Area.Mode];
			AreaModeStats newModeStats = areaStats.Modes[(int)this.Area.Mode];
			bool doStrawberries = newModeStats.TotalStrawberries > modeStats.TotalStrawberries;
			bool doHeartGem = newModeStats.HeartGem && !modeStats.HeartGem;
			bool doDeaths = newModeStats.Deaths > modeStats.Deaths && (this.Area.Mode != AreaMode.Normal || newModeStats.Completed);
			bool doRemixUnlock = this.Area.Mode == AreaMode.Normal && data.HasMode(AreaMode.BSide) && areaStats.Cassette && !displayedStats.Cassette;
			if (doStrawberries || doHeartGem || doDeaths || doRemixUnlock)
			{
				yield return 0.8f;
			}
			bool skipped = false;
			Coroutine routine = new Coroutine(this.IncrementStatsDisplay(modeStats, newModeStats, doHeartGem, doStrawberries, doDeaths, doRemixUnlock), true);
			base.Add(routine);
			yield return null;
			while (!routine.Finished)
			{
				if (MInput.GamePads[0].Pressed(Buttons.Start) || MInput.Keyboard.Pressed(Keys.Enter))
				{
					routine.Active = false;
					routine.RemoveSelf();
					skipped = true;
					Audio.Stop(this.bSideUnlockSfx, true);
					Audio.Play("event:/new_content/ui/skip_all");
					break;
				}
				yield return null;
			}
			if (skipped && doRemixUnlock)
			{
				this.spotlightAlpha = 0f;
				this.spotlightRadius = 0f;
				if (this.remixUnlockText != null)
				{
					this.remixUnlockText.RemoveSelf();
					this.remixUnlockText = null;
				}
				if (this.modes.Count <= 1 || this.modes[1].ID != "B")
				{
					this.AddRemixButton();
				}
				else
				{
					OuiChapterPanel.Option option = this.modes[1];
					option.IconEase = 1f;
					option.Appear = 1f;
					option.Appeared = false;
				}
			}
			this.DisplayedStats = this.RealStats;
			if (skipped)
			{
				doStrawberries = (doStrawberries && modeStats.TotalStrawberries != newModeStats.TotalStrawberries);
				doDeaths &= (modeStats.Deaths != newModeStats.Deaths);
				doHeartGem = (doHeartGem && !this.heart.Visible);
				this.UpdateStats(true, new bool?(doStrawberries), new bool?(doDeaths), new bool?(doHeartGem));
			}
			yield return null;
			routine = null;
			if (shouldAdvance && this.OverworldStartMode == Overworld.StartMode.AreaComplete)
			{
				if ((!doDeaths && !doStrawberries && !doHeartGem) || Settings.Instance.SpeedrunClock != SpeedrunType.Off)
				{
					yield return 1.2f;
				}
				base.Overworld.Goto<OuiChapterSelect>().AdvanceToNext();
			}
			else
			{
				this.Focused = true;
				base.Overworld.ShowInputUI = true;
			}
			yield break;
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x00092C64 File Offset: 0x00090E64
		private float HandleDeathTick(int oldDeaths, int newDeaths, out int add)
		{
			int num = newDeaths - oldDeaths;
			if (num < 3)
			{
				add = 1;
				return 0.3f;
			}
			if (num < 8)
			{
				add = 2;
				return 0.2f;
			}
			if (num < 30)
			{
				add = 5;
				return 0.1f;
			}
			if (num < 100)
			{
				add = 10;
				return 0.1f;
			}
			if (num < 1000)
			{
				add = 25;
				return 0.1f;
			}
			add = 100;
			return 0.1f;
		}

		// Token: 0x060017B1 RID: 6065 RVA: 0x00092CC7 File Offset: 0x00090EC7
		private void PlayExpandSfx(float currentHeight, float nextHeight)
		{
			if (nextHeight > currentHeight)
			{
				Audio.Play("event:/ui/world_map/chapter/pane_expand");
				return;
			}
			if (nextHeight < currentHeight)
			{
				Audio.Play("event:/ui/world_map/chapter/pane_contract");
			}
		}

		// Token: 0x060017B2 RID: 6066 RVA: 0x000472CC File Offset: 0x000454CC
		public static string GetCheckpointPreviewName(AreaKey area, string level)
		{
			if (level == null)
			{
				return area.ToString();
			}
			return area.ToString() + "_" + level;
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x00092CE8 File Offset: 0x00090EE8
		private MTexture GetCheckpointPreview(AreaKey area, string level)
		{
			string checkpointPreviewName = OuiChapterPanel.GetCheckpointPreviewName(area, level);
			if (MTN.Checkpoints.Has(checkpointPreviewName))
			{
				return MTN.Checkpoints[checkpointPreviewName];
			}
			return null;
		}

		// Token: 0x0400144E RID: 5198
		public AreaKey Area;

		// Token: 0x0400144F RID: 5199
		public AreaStats RealStats;

		// Token: 0x04001450 RID: 5200
		public AreaStats DisplayedStats;

		// Token: 0x04001451 RID: 5201
		public AreaData Data;

		// Token: 0x04001452 RID: 5202
		public Overworld.StartMode OverworldStartMode;

		// Token: 0x04001453 RID: 5203
		public bool EnteringChapter;

		// Token: 0x04001454 RID: 5204
		public const int ContentOffsetX = 440;

		// Token: 0x04001455 RID: 5205
		public const int NoStatsHeight = 300;

		// Token: 0x04001456 RID: 5206
		public const int StatsHeight = 540;

		// Token: 0x04001457 RID: 5207
		public const int CheckpointsHeight = 730;

		// Token: 0x04001458 RID: 5208
		private bool initialized;

		// Token: 0x04001459 RID: 5209
		private string chapter = "";

		// Token: 0x0400145A RID: 5210
		private bool selectingMode = true;

		// Token: 0x0400145B RID: 5211
		private float height;

		// Token: 0x0400145C RID: 5212
		private bool resizing;

		// Token: 0x0400145D RID: 5213
		private Wiggler wiggler;

		// Token: 0x0400145E RID: 5214
		private Wiggler modeAppearWiggler;

		// Token: 0x0400145F RID: 5215
		private MTexture card = new MTexture();

		// Token: 0x04001460 RID: 5216
		private Vector2 contentOffset;

		// Token: 0x04001461 RID: 5217
		private float spotlightRadius;

		// Token: 0x04001462 RID: 5218
		private float spotlightAlpha;

		// Token: 0x04001463 RID: 5219
		private Vector2 spotlightPosition;

		// Token: 0x04001464 RID: 5220
		private AreaCompleteTitle remixUnlockText;

		// Token: 0x04001465 RID: 5221
		private StrawberriesCounter strawberries = new StrawberriesCounter(true, 0, 0, true);

		// Token: 0x04001466 RID: 5222
		private Vector2 strawberriesOffset;

		// Token: 0x04001467 RID: 5223
		private DeathsCounter deaths = new DeathsCounter(AreaMode.Normal, true, 0, 0);

		// Token: 0x04001468 RID: 5224
		private Vector2 deathsOffset;

		// Token: 0x04001469 RID: 5225
		private HeartGemDisplay heart = new HeartGemDisplay(0, false);

		// Token: 0x0400146A RID: 5226
		private Vector2 heartOffset;

		// Token: 0x0400146B RID: 5227
		private int checkpoint;

		// Token: 0x0400146C RID: 5228
		private List<OuiChapterPanel.Option> modes = new List<OuiChapterPanel.Option>();

		// Token: 0x0400146D RID: 5229
		private List<OuiChapterPanel.Option> checkpoints = new List<OuiChapterPanel.Option>();

		// Token: 0x0400146E RID: 5230
		private EventInstance bSideUnlockSfx;

		// Token: 0x020006A7 RID: 1703
		private class Option
		{
			// Token: 0x17000631 RID: 1585
			// (get) Token: 0x06002C4A RID: 11338 RVA: 0x0011A32F File Offset: 0x0011852F
			public float Scale
			{
				get
				{
					if (this.Siblings < 5)
					{
						return 1f;
					}
					return 0.8f;
				}
			}

			// Token: 0x17000632 RID: 1586
			// (get) Token: 0x06002C4B RID: 11339 RVA: 0x0011A345 File Offset: 0x00118545
			public bool OnTopOfUI
			{
				get
				{
					return this.Pop > 0.5f;
				}
			}

			// Token: 0x06002C4C RID: 11340 RVA: 0x0011A354 File Offset: 0x00118554
			public void SlideTowards(int i, int count, bool snap)
			{
				float num = (float)count / 2f - 0.5f;
				float num2 = (float)i - num;
				if (snap)
				{
					this.Slide = num2;
					return;
				}
				this.Slide = Calc.Approach(this.Slide, num2, Engine.DeltaTime * 4f);
			}

			// Token: 0x06002C4D RID: 11341 RVA: 0x0011A3A0 File Offset: 0x001185A0
			public Vector2 GetRenderPosition(Vector2 center)
			{
				float num = (float)(this.Large ? 170 : 130) * this.Scale;
				if (this.Siblings > 0 && num * (float)this.Siblings > 750f)
				{
					num = (float)(750 / this.Siblings);
				}
				Vector2 result = center + new Vector2(this.Slide * num, (float)Math.Sin((double)(this.Pop * 3.1415927f)) * 70f - this.Pop * 12f);
				result.Y += (1f - Ease.CubeOut(this.Appear)) * -200f;
				result.Y -= (1f - this.Scale) * 80f;
				return result;
			}

			// Token: 0x06002C4E RID: 11342 RVA: 0x0011A470 File Offset: 0x00118670
			public void Render(Vector2 center, bool selected, Wiggler wiggler, Wiggler appearWiggler)
			{
				float num = this.Scale + (selected ? (wiggler.Value * 0.25f) : 0f) + (this.Appeared ? (appearWiggler.Value * 0.25f) : 0f);
				Vector2 renderPosition = this.GetRenderPosition(center);
				Color color = Color.Lerp(this.BgColor, Color.Black, (1f - this.Pop) * 0.6f);
				this.Bg.DrawCentered(renderPosition + new Vector2(0f, 10f), color, (this.Appeared ? this.Scale : num) * new Vector2(this.Large ? 1f : 0.9f, 1f));
				if (this.IconEase > 0f)
				{
					float num2 = Ease.CubeIn(this.IconEase);
					Color color2 = Color.Lerp(Color.White, Color.Black, this.Faded * 0.6f) * num2;
					this.Icon.DrawCentered(renderPosition, color2, (float)(this.Bg.Width - 50) / (float)this.Icon.Width * num * (2.5f - num2 * 1.5f));
				}
			}

			// Token: 0x04002B8A RID: 11146
			public string Label;

			// Token: 0x04002B8B RID: 11147
			public string ID;

			// Token: 0x04002B8C RID: 11148
			public MTexture Icon;

			// Token: 0x04002B8D RID: 11149
			public MTexture Bg = GFX.Gui["areaselect/tab"];

			// Token: 0x04002B8E RID: 11150
			public Color BgColor = Calc.HexToColor("3c6180");

			// Token: 0x04002B8F RID: 11151
			public float Pop;

			// Token: 0x04002B90 RID: 11152
			public bool Large = true;

			// Token: 0x04002B91 RID: 11153
			public int Siblings;

			// Token: 0x04002B92 RID: 11154
			public float Slide;

			// Token: 0x04002B93 RID: 11155
			public float Appear = 1f;

			// Token: 0x04002B94 RID: 11156
			public float IconEase = 1f;

			// Token: 0x04002B95 RID: 11157
			public bool Appeared;

			// Token: 0x04002B96 RID: 11158
			public float Faded;

			// Token: 0x04002B97 RID: 11159
			public float CheckpointSlideOut;

			// Token: 0x04002B98 RID: 11160
			public string CheckpointLevelName;

			// Token: 0x04002B99 RID: 11161
			public float CheckpointRotation;

			// Token: 0x04002B9A RID: 11162
			public Vector2 CheckpointOffset;
		}
	}
}
