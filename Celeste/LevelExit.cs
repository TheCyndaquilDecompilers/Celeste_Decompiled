using System;
using System.Collections;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000259 RID: 601
	public class LevelExit : Scene
	{
		// Token: 0x060012BA RID: 4794 RVA: 0x000655FB File Offset: 0x000637FB
		public LevelExit(LevelExit.Mode mode, Session session, HiresSnow snow = null)
		{
			base.Add(new HudRenderer());
			this.session = session;
			this.mode = mode;
			this.snow = snow;
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x00065624 File Offset: 0x00063824
		public override void Begin()
		{
			base.Begin();
			if (this.mode != LevelExit.Mode.GoldenBerryRestart)
			{
				SaveLoadIcon.Show(this);
			}
			bool flag = this.snow == null;
			if (flag)
			{
				this.snow = new HiresSnow(0.45f);
			}
			if (this.mode == LevelExit.Mode.Completed)
			{
				this.snow.Direction = new Vector2(0f, 16f);
				if (flag)
				{
					this.snow.Reset();
				}
				RunThread.Start(new Action(this.LoadCompleteThread), "COMPLETE_LEVEL", false);
				if (this.session.Area.Mode != AreaMode.Normal)
				{
					Audio.SetMusic("event:/music/menu/complete_bside", true, true);
				}
				else if (this.session.Area.ID == 7)
				{
					Audio.SetMusic("event:/music/menu/complete_summit", true, true);
				}
				else
				{
					Audio.SetMusic("event:/music/menu/complete_area", true, true);
				}
				Audio.SetAmbience(null, true);
			}
			if (this.mode == LevelExit.Mode.GiveUp)
			{
				this.overworldLoader = new OverworldLoader(Overworld.StartMode.AreaQuit, this.snow);
			}
			else if (this.mode == LevelExit.Mode.SaveAndQuit)
			{
				this.overworldLoader = new OverworldLoader(Overworld.StartMode.MainMenu, this.snow);
			}
			else if (this.mode == LevelExit.Mode.CompletedInterlude)
			{
				this.overworldLoader = new OverworldLoader(Overworld.StartMode.AreaComplete, this.snow);
			}
			Entity entity;
			base.Add(entity = new Entity());
			entity.Add(new Coroutine(this.Routine(), true));
			if (this.mode != LevelExit.Mode.Restart && this.mode != LevelExit.Mode.GoldenBerryRestart)
			{
				base.Add(this.snow);
				if (flag)
				{
					new FadeWipe(this, true, null);
				}
			}
			Stats.Store();
			base.RendererList.UpdateLists();
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x000657B4 File Offset: 0x000639B4
		private void LoadCompleteThread()
		{
			this.completeXml = AreaData.Get(this.session).CompleteScreenXml;
			if (this.completeXml != null && this.completeXml.HasAttr("atlas"))
			{
				string path = Path.Combine("Graphics", "Atlases", this.completeXml.Attr("atlas"));
				this.completeAtlas = Atlas.FromAtlas(path, Atlas.AtlasDataFormat.PackerNoAtlas);
			}
			this.completeLoaded = true;
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x00065825 File Offset: 0x00063A25
		private IEnumerator Routine()
		{
			if (this.mode != LevelExit.Mode.GoldenBerryRestart)
			{
				UserIO.SaveHandler(true, true);
				while (UserIO.Saving)
				{
					yield return null;
				}
				if (this.mode == LevelExit.Mode.Completed)
				{
					while (!this.completeLoaded)
					{
						yield return null;
					}
				}
				while (SaveLoadIcon.OnScreen)
				{
					yield return null;
				}
			}
			if (this.mode == LevelExit.Mode.Completed)
			{
				while (this.timer < 3.3f)
				{
					yield return null;
				}
				Audio.SetMusicParam("end", 1f);
				Engine.Scene = new AreaComplete(this.session, this.completeXml, this.completeAtlas, this.snow);
			}
			else if (this.mode == LevelExit.Mode.GiveUp || this.mode == LevelExit.Mode.SaveAndQuit || this.mode == LevelExit.Mode.CompletedInterlude)
			{
				Engine.Scene = this.overworldLoader;
			}
			else if (this.mode == LevelExit.Mode.Restart || this.mode == LevelExit.Mode.GoldenBerryRestart)
			{
				Session session;
				if (this.mode == LevelExit.Mode.GoldenBerryRestart)
				{
					if ((this.session.Audio.Music.Event == "event:/music/lvl7/main" || this.session.Audio.Music.Event == "event:/music/lvl7/final_ascent") && this.session.Audio.Music.Progress > 0)
					{
						Audio.SetMusic(null, true, true);
					}
					session = this.session.Restart(this.GoldenStrawberryEntryLevel);
				}
				else
				{
					session = this.session.Restart(null);
				}
				LevelLoader levelLoader = new LevelLoader(session, null);
				if (this.mode == LevelExit.Mode.GoldenBerryRestart)
				{
					levelLoader.PlayerIntroTypeOverride = new Player.IntroTypes?(Player.IntroTypes.Respawn);
				}
				Engine.Scene = levelLoader;
			}
			yield break;
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x00065834 File Offset: 0x00063A34
		public override void Update()
		{
			this.timer += Engine.DeltaTime;
			base.Update();
		}

		// Token: 0x04000EB4 RID: 3764
		private LevelExit.Mode mode;

		// Token: 0x04000EB5 RID: 3765
		private Session session;

		// Token: 0x04000EB6 RID: 3766
		private float timer;

		// Token: 0x04000EB7 RID: 3767
		private XmlElement completeXml;

		// Token: 0x04000EB8 RID: 3768
		private Atlas completeAtlas;

		// Token: 0x04000EB9 RID: 3769
		private bool completeLoaded;

		// Token: 0x04000EBA RID: 3770
		private HiresSnow snow;

		// Token: 0x04000EBB RID: 3771
		private OverworldLoader overworldLoader;

		// Token: 0x04000EBC RID: 3772
		public string GoldenStrawberryEntryLevel;

		// Token: 0x04000EBD RID: 3773
		private const float MinTimeForCompleteScreen = 3.3f;

		// Token: 0x02000578 RID: 1400
		public enum Mode
		{
			// Token: 0x040026AC RID: 9900
			SaveAndQuit,
			// Token: 0x040026AD RID: 9901
			GiveUp,
			// Token: 0x040026AE RID: 9902
			Restart,
			// Token: 0x040026AF RID: 9903
			GoldenBerryRestart,
			// Token: 0x040026B0 RID: 9904
			Completed,
			// Token: 0x040026B1 RID: 9905
			CompletedInterlude
		}
	}
}
