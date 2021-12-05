using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x020002F5 RID: 757
	public class AreaComplete : Scene
	{
		// Token: 0x06001763 RID: 5987 RVA: 0x0008EA6C File Offset: 0x0008CC6C
		public AreaComplete(Session session, XmlElement xml, Atlas atlas, HiresSnow snow)
		{
			this.Session = session;
			this.version = Celeste.Instance.Version.ToString();
			if (session.Area.ID != 7)
			{
				string text = Dialog.Clean("areacomplete_" + session.Area.Mode + (session.FullClear ? "_fullclear" : ""), null);
				Vector2 origin = new Vector2(960f, 200f);
				float scale = Math.Min(1600f / ActiveFont.Measure(text).X, 3f);
				this.title = new AreaCompleteTitle(origin, text, scale, false);
			}
			base.Add(this.complete = new CompleteRenderer(xml, atlas, 1f, delegate()
			{
				this.finishedSlide = true;
			}));
			if (this.title != null)
			{
				this.complete.RenderUI = delegate(Vector2 v)
				{
					this.title.DrawLineUI();
				};
			}
			this.complete.RenderPostUI = new Action(this.RenderUI);
			this.speedrunTimerChapterString = TimeSpan.FromTicks(this.Session.Time).ShortGameplayFormat();
			this.speedrunTimerFileString = Dialog.FileTime(SaveData.Instance.Time);
			SpeedrunTimerDisplay.CalculateBaseSizes();
			this.snow = snow;
			base.Add(snow);
			base.RendererList.UpdateLists();
			AreaKey area = session.Area;
			if (area.Mode == AreaMode.Normal)
			{
				if (area.ID == 1)
				{
					Achievements.Register(Achievement.CH1);
					return;
				}
				if (area.ID == 2)
				{
					Achievements.Register(Achievement.CH2);
					return;
				}
				if (area.ID == 3)
				{
					Achievements.Register(Achievement.CH3);
					return;
				}
				if (area.ID == 4)
				{
					Achievements.Register(Achievement.CH4);
					return;
				}
				if (area.ID == 5)
				{
					Achievements.Register(Achievement.CH5);
					return;
				}
				if (area.ID == 6)
				{
					Achievements.Register(Achievement.CH6);
					return;
				}
				if (area.ID == 7)
				{
					Achievements.Register(Achievement.CH7);
				}
			}
		}

		// Token: 0x06001764 RID: 5988 RVA: 0x0008EC77 File Offset: 0x0008CE77
		public override void End()
		{
			base.End();
			this.complete.Dispose();
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x0008EC8C File Offset: 0x0008CE8C
		public override void Update()
		{
			base.Update();
			if (Input.MenuConfirm.Pressed && this.finishedSlide && this.canConfirm)
			{
				this.canConfirm = false;
				if (this.Session.Area.ID == 7 && this.Session.Area.Mode == AreaMode.Normal)
				{
					new FadeWipe(this, false, delegate()
					{
						this.Session.RespawnPoint = null;
						this.Session.FirstLevel = false;
						this.Session.Level = "credits-summit";
						this.Session.Audio.Music.Event = "event:/music/lvl8/main";
						this.Session.Audio.Apply(false);
						Engine.Scene = new LevelLoader(this.Session, null)
						{
							PlayerIntroTypeOverride = new Player.IntroTypes?(Player.IntroTypes.None),
							Level = 
							{
								new CS07_Credits()
							}
						};
					});
				}
				else
				{
					HiresSnow outSnow = new HiresSnow(0.45f);
					outSnow.Alpha = 0f;
					outSnow.AttachAlphaTo = new FadeWipe(this, false, delegate()
					{
						Engine.Scene = new OverworldLoader(Overworld.StartMode.AreaComplete, outSnow);
					});
					base.Add(outSnow);
				}
			}
			this.snow.Alpha = Calc.Approach(this.snow.Alpha, 0f, Engine.DeltaTime * 0.5f);
			this.snow.Direction.Y = Calc.Approach(this.snow.Direction.Y, 1f, Engine.DeltaTime * 24f);
			this.speedrunTimerDelay -= Engine.DeltaTime;
			if (this.speedrunTimerDelay <= 0f)
			{
				this.speedrunTimerEase = Calc.Approach(this.speedrunTimerEase, 1f, Engine.DeltaTime * 2f);
			}
			if (this.title != null)
			{
				this.title.Update();
			}
			if (Celeste.PlayMode == Celeste.PlayModes.Debug)
			{
				if (MInput.Keyboard.Pressed(Keys.F2))
				{
					Celeste.ReloadAssets(false, true, false, null);
					Engine.Scene = new LevelExit(LevelExit.Mode.Completed, this.Session, null);
					return;
				}
				if (MInput.Keyboard.Pressed(Keys.F3))
				{
					Celeste.ReloadAssets(false, true, true, null);
					Engine.Scene = new LevelExit(LevelExit.Mode.Completed, this.Session, null);
				}
			}
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x0008EE74 File Offset: 0x0008D074
		private void RenderUI()
		{
			base.Entities.Render();
			AreaComplete.Info(this.speedrunTimerEase, this.speedrunTimerChapterString, this.speedrunTimerFileString, this.chapterSpeedrunText, this.version);
			if (this.complete.HasUI && this.title != null)
			{
				this.title.Render();
			}
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x0008EED0 File Offset: 0x0008D0D0
		public static void Info(float ease, string speedrunTimerChapterString, string speedrunTimerFileString, string chapterSpeedrunText, string versionText)
		{
			if (ease > 0f && Settings.Instance.SpeedrunClock != SpeedrunType.Off)
			{
				Vector2 vector = new Vector2(80f - 300f * (1f - Ease.CubeOut(ease)), 1000f);
				if (Settings.Instance.SpeedrunClock == SpeedrunType.Chapter)
				{
					SpeedrunTimerDisplay.DrawTime(vector, speedrunTimerChapterString, 1f, true, false, false, 1f);
				}
				else
				{
					vector.Y -= 16f;
					SpeedrunTimerDisplay.DrawTime(vector, speedrunTimerFileString, 1f, true, false, false, 1f);
					ActiveFont.DrawOutline(chapterSpeedrunText, vector + new Vector2(0f, 40f), new Vector2(0f, 1f), Vector2.One * 0.6f, Color.White, 2f, Color.Black);
					SpeedrunTimerDisplay.DrawTime(vector + new Vector2(ActiveFont.Measure(chapterSpeedrunText).X * 0.6f + 8f, 40f), speedrunTimerChapterString, 0.6f, true, false, false, 1f);
				}
				AreaComplete.VersionNumberAndVariants(versionText, ease, 1f);
			}
		}

		// Token: 0x06001768 RID: 5992 RVA: 0x0008EFFC File Offset: 0x0008D1FC
		public static void VersionNumberAndVariants(string version, float ease, float alpha)
		{
			Vector2 vector = new Vector2(1820f + 300f * (1f - Ease.CubeOut(ease)), 1020f);
			if (SaveData.Instance.AssistMode || SaveData.Instance.VariantMode)
			{
				MTexture mtexture = GFX.Gui[SaveData.Instance.AssistMode ? "cs_assistmode" : "cs_variantmode"];
				vector.Y -= 32f;
				mtexture.DrawJustified(vector + new Vector2(0f, -8f), new Vector2(0.5f, 1f), Color.White, 0.6f);
				ActiveFont.DrawOutline(version, vector, new Vector2(0.5f, 0f), Vector2.One * 0.5f, Color.White, 2f, Color.Black);
				return;
			}
			ActiveFont.DrawOutline(version, vector, new Vector2(0.5f, 0.5f), Vector2.One * 0.5f, Color.White, 2f, Color.Black);
		}

		// Token: 0x0400140F RID: 5135
		public Session Session;

		// Token: 0x04001410 RID: 5136
		private bool finishedSlide;

		// Token: 0x04001411 RID: 5137
		private bool canConfirm = true;

		// Token: 0x04001412 RID: 5138
		private HiresSnow snow;

		// Token: 0x04001413 RID: 5139
		private float speedrunTimerDelay = 1.1f;

		// Token: 0x04001414 RID: 5140
		private float speedrunTimerEase;

		// Token: 0x04001415 RID: 5141
		private string speedrunTimerChapterString;

		// Token: 0x04001416 RID: 5142
		private string speedrunTimerFileString;

		// Token: 0x04001417 RID: 5143
		private string chapterSpeedrunText = Dialog.Get("OPTIONS_SPEEDRUN_CHAPTER", null) + ":";

		// Token: 0x04001418 RID: 5144
		private AreaCompleteTitle title;

		// Token: 0x04001419 RID: 5145
		private CompleteRenderer complete;

		// Token: 0x0400141A RID: 5146
		private string version;
	}
}
