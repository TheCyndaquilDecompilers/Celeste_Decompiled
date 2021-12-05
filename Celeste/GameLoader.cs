using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000244 RID: 580
	public class GameLoader : Scene
	{
		// Token: 0x0600125A RID: 4698 RVA: 0x00060670 File Offset: 0x0005E870
		public GameLoader()
		{
			Console.WriteLine("GAME DISPLAYED (in " + Celeste.LoadTimer.ElapsedMilliseconds + "ms)");
			this.Snow = new HiresSnow(0.45f);
			this.opening = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Opening"), Atlas.AtlasDataFormat.PackerNoAtlas);
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x000606D8 File Offset: 0x0005E8D8
		public override void Begin()
		{
			base.Add(new HudRenderer());
			base.Add(this.Snow);
			new FadeWipe(this, true, null);
			base.RendererList.UpdateLists();
			base.Add(this.handler = new Entity());
			this.handler.Tag = Tags.HUD;
			this.handler.Add(new Coroutine(this.IntroRoutine(), true));
			this.activeThread = Thread.CurrentThread;
			this.activeThread.Priority = ThreadPriority.Lowest;
			RunThread.Start(new Action(this.LoadThread), "GAME_LOADER", true);
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x00060780 File Offset: 0x0005E980
		private void LoadThread()
		{
			MInput.Disabled = true;
			Stopwatch stopwatch = Stopwatch.StartNew();
			Audio.Init();
			Audio.Banks.Master = Audio.Banks.Load("Master Bank", true);
			Audio.Banks.Music = Audio.Banks.Load("music", false);
			Audio.Banks.Sfxs = Audio.Banks.Load("sfx", false);
			Audio.Banks.UI = Audio.Banks.Load("ui", false);
			Audio.Banks.DlcMusic = Audio.Banks.Load("dlc_music", false);
			Audio.Banks.DlcSfxs = Audio.Banks.Load("dlc_sfx", false);
			Settings.Instance.ApplyVolumes();
			this.audioLoaded = true;
			Console.WriteLine(" - AUDIO LOAD: " + stopwatch.ElapsedMilliseconds + "ms");
			GFX.Load();
			MTN.Load();
			GFX.LoadData();
			MTN.LoadData();
			Stopwatch stopwatch2 = Stopwatch.StartNew();
			Fonts.Prepare();
			Dialog.Load();
			Fonts.Load(Dialog.Languages["english"].FontFace);
			Fonts.Load(Dialog.Languages[Settings.Instance.Language].FontFace);
			this.dialogLoaded = true;
			Console.WriteLine(" - DIA/FONT LOAD: " + stopwatch2.ElapsedMilliseconds + "ms");
			MInput.Disabled = false;
			Stopwatch stopwatch3 = Stopwatch.StartNew();
			AreaData.Load();
			Console.WriteLine(" - LEVELS LOAD: " + stopwatch3.ElapsedMilliseconds + "ms");
			Console.WriteLine("DONE LOADING (in " + Celeste.LoadTimer.ElapsedMilliseconds + "ms)");
			Celeste.LoadTimer.Stop();
			Celeste.LoadTimer = null;
			this.loaded = true;
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x0006091B File Offset: 0x0005EB1B
		public IEnumerator IntroRoutine()
		{
			if (Celeste.PlayMode != Celeste.PlayModes.Debug)
			{
				float p = 0f;
				while (p > 1f && !this.skipped)
				{
					yield return null;
					p += Engine.DeltaTime;
				}
				if (!this.skipped)
				{
					Image img2 = new Image(this.opening["presentedby"]);
					yield return this.FadeInOut(img2);
				}
				if (!this.skipped)
				{
					Image img3 = new Image(this.opening["gameby"]);
					yield return this.FadeInOut(img3);
				}
				bool flag = true;
				if (!this.skipped && flag)
				{
					while (!this.dialogLoaded)
					{
						yield return null;
					}
					AutoSavingNotice notice = new AutoSavingNotice();
					base.Add(notice);
					p = 0f;
					while (p < 1f && !this.skipped)
					{
						yield return null;
						p += Engine.DeltaTime;
					}
					notice.Display = false;
					while (notice.StillVisible)
					{
						notice.ForceClose = this.skipped;
						yield return null;
					}
					base.Remove(notice);
					notice = null;
				}
			}
			this.ready = true;
			if (!this.loaded)
			{
				this.loadingTextures = OVR.Atlas.GetAtlasSubtextures("loading/");
				Image img = new Image(this.loadingTextures[0]);
				img.CenterOrigin();
				img.Scale = Vector2.One * 0.5f;
				this.handler.Add(img);
				while (!this.loaded || this.loadingAlpha > 0f)
				{
					this.loadingFrame += Engine.DeltaTime * 10f;
					this.loadingAlpha = Calc.Approach(this.loadingAlpha, (float)(this.loaded ? 0 : 1), Engine.DeltaTime * 4f);
					img.Texture = this.loadingTextures[(int)(this.loadingFrame % (float)this.loadingTextures.Count)];
					img.Color = Color.White * Ease.CubeOut(this.loadingAlpha);
					img.Position = new Vector2(1792f, 1080f - 128f * Ease.CubeOut(this.loadingAlpha));
					yield return null;
				}
				img = null;
			}
			this.opening.Dispose();
			this.activeThread.Priority = ThreadPriority.Normal;
			MInput.Disabled = false;
			Engine.Scene = new OverworldLoader(Overworld.StartMode.Titlescreen, this.Snow);
			yield break;
		}

		// Token: 0x0600125E RID: 4702 RVA: 0x0006092A File Offset: 0x0005EB2A
		private IEnumerator FadeInOut(Image img)
		{
			float alpha = 0f;
			img.Color = Color.White * 0f;
			this.handler.Add(img);
			for (float i = 0f; i < 4.5f; i += Engine.DeltaTime)
			{
				if (this.skipped)
				{
					break;
				}
				alpha = Ease.CubeOut(Math.Min(i, 1f));
				img.Color = Color.White * alpha;
				yield return null;
			}
			while (alpha > 0f)
			{
				alpha -= Engine.DeltaTime * (float)(this.skipped ? 8 : 1);
				img.Color = Color.White * alpha;
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600125F RID: 4703 RVA: 0x00060940 File Offset: 0x0005EB40
		public override void Update()
		{
			if (this.audioLoaded && !this.audioStarted)
			{
				Audio.SetAmbience("event:/env/amb/worldmap", true);
				this.audioStarted = true;
			}
			if (!this.ready)
			{
				bool disabled = MInput.Disabled;
				MInput.Disabled = false;
				if (Input.MenuConfirm.Pressed)
				{
					this.skipped = true;
				}
				MInput.Disabled = disabled;
			}
			base.Update();
		}

		// Token: 0x04000E07 RID: 3591
		public HiresSnow Snow;

		// Token: 0x04000E08 RID: 3592
		private Atlas opening;

		// Token: 0x04000E09 RID: 3593
		private bool loaded;

		// Token: 0x04000E0A RID: 3594
		private bool audioLoaded;

		// Token: 0x04000E0B RID: 3595
		private bool audioStarted;

		// Token: 0x04000E0C RID: 3596
		private bool dialogLoaded;

		// Token: 0x04000E0D RID: 3597
		private Entity handler;

		// Token: 0x04000E0E RID: 3598
		private Thread activeThread;

		// Token: 0x04000E0F RID: 3599
		private bool skipped;

		// Token: 0x04000E10 RID: 3600
		private bool ready;

		// Token: 0x04000E11 RID: 3601
		private List<MTexture> loadingTextures;

		// Token: 0x04000E12 RID: 3602
		private float loadingFrame;

		// Token: 0x04000E13 RID: 3603
		private float loadingAlpha;
	}
}
