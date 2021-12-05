using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200015F RID: 351
	public class CS10_Ending : CutsceneEntity
	{
		// Token: 0x06000C7A RID: 3194 RVA: 0x00029E54 File Offset: 0x00028054
		public CS10_Ending(Player player) : base(false, true)
		{
			base.Tag = Tags.HUD;
			player.StateMachine.State = 11;
			player.DummyAutoAnimate = false;
			player.Sprite.Rate = 0f;
			this.RemoveOnSkipped = false;
			base.Add(new LevelEndingHook(delegate()
			{
				Audio.Stop(this.cinIntro, true);
			}));
		}

		// Token: 0x06000C7B RID: 3195 RVA: 0x00029F17 File Offset: 0x00028117
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Level level = scene as Level;
			level.TimerStopped = true;
			level.TimerHidden = true;
			level.SaveQuitDisabled = true;
			level.PauseLock = true;
			level.AllowHudHide = false;
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x00029F48 File Offset: 0x00028148
		public override void OnBegin(Level level)
		{
			Audio.SetAmbience(null, true);
			level.AutoSave();
			this.speedrunTimerChapterString = TimeSpan.FromTicks(level.Session.Time).ShortGameplayFormat();
			this.speedrunTimerFileString = Dialog.FileTime(SaveData.Instance.Time);
			SpeedrunTimerDisplay.CalculateBaseSizes();
			base.Add(this.cutscene = new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000C7D RID: 3197 RVA: 0x00029FB4 File Offset: 0x000281B4
		private IEnumerator Cutscene(Level level)
		{
			if (level.Wipe != null)
			{
				level.Wipe.Cancel();
			}
			while (level.IsAutoSaving())
			{
				yield return null;
			}
			yield return 1f;
			this.Atlas = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Farewell"), Atlas.AtlasDataFormat.PackerNoAtlas);
			this.Frames = this.Atlas.GetAtlasSubtextures("");
			base.Add(this.attachment = new Image(this.Atlas["21-window"]));
			base.Add(this.picture = new Image(this.Atlas["21-picture"]));
			base.Add(this.ok = new Image(this.Atlas["21-button"]));
			base.Add(this.cursor = new Image(this.Atlas["21-cursor"]));
			this.attachment.Visible = false;
			this.picture.Visible = false;
			this.ok.Visible = false;
			this.cursor.Visible = false;
			level.PauseLock = false;
			yield return 2f;
			this.cinIntro = Audio.Play("event:/new_content/music/lvl10/cinematic/end_intro");
			Audio.SetAmbience(null, true);
			this.counting = true;
			base.Add(new Coroutine(this.Fade(1f, 0f, 4f, 0f), true));
			base.Add(new Coroutine(this.Zoom(1.38f, 1.2f, 4f, null), true));
			yield return this.Loop("0", 2f);
			Input.Rumble(RumbleStrength.Climb, RumbleLength.TwoSeconds);
			yield return this.Loop("0,1,1,0,0,1,1,0*8", 2f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
			Audio.SetMusic("event:/new_content/music/lvl10/cinematic/end", true, false);
			this.endAmbience = Audio.Play("event:/new_content/env/10_endscene");
			base.Add(new Coroutine(this.Zoom(1.2f, 1.05f, 0.06f, Ease.CubeOut), true));
			yield return this.Play("2-7");
			yield return this.Loop("7", 1.5f);
			yield return this.Play("8-10,10*20");
			List<int> frameData = this.GetFrameData("10-13,13*16,14*28,14-17,14*24");
			float num = (float)(frameData.Count + 3) * 0.083333336f;
			this.fadeColor = Color.Black;
			base.Add(new Coroutine(this.Zoom(1.05f, 1f, num, Ease.Linear), true));
			base.Add(new Coroutine(this.Fade(0f, 1f, num * 0.1f, num * 0.85f), true));
			base.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate
			{
				Audio.Play("event:/new_content/game/10_farewell/endscene_dial_theo");
			}, num, true));
			yield return this.Play(frameData);
			this.frame = 18;
			this.fade = 1f;
			yield return 0.5f;
			yield return this.Fade(1f, 0f, 1.2f, 0f);
			base.Add(this.talkingLoop = new Coroutine(this.Loop("18*24,19,19,18*6,20,20", -1f), true));
			yield return 1f;
			yield return Textbox.Say("CH9_END_CINEMATIC", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.ShowPicture)
			});
			Audio.SetMusicParam("end", 1f);
			Audio.Play("event:/new_content/game/10_farewell/endscene_photo_zoom");
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 4f)
			{
				Audio.SetParameter(this.endAmbience, "fade", 1f - p);
				this.computerFade = p;
				this.picture.Scale = Vector2.One * (0.9f + 0.100000024f * p);
				yield return null;
			}
			base.EndCutscene(level, false);
			yield break;
		}

		// Token: 0x06000C7E RID: 3198 RVA: 0x00029FCA File Offset: 0x000281CA
		private IEnumerator ShowPicture()
		{
			this.center = new Vector2(1230f, 312f);
			base.Add(new Coroutine(this.Fade(0f, 1f, 0.25f, 0f), true));
			base.Add(new Coroutine(this.Zoom(1f, 1.1f, 0.25f, null), true));
			yield return 0.25f;
			if (this.talkingLoop != null)
			{
				this.talkingLoop.RemoveSelf();
			}
			this.talkingLoop = null;
			yield return null;
			this.frame = 21;
			this.cursor.Visible = true;
			this.center = Celeste.TargetCenter;
			base.Add(new Coroutine(this.Fade(1f, 0f, 0.25f, 0f), true));
			base.Add(new Coroutine(this.Zoom(1.1f, 1f, 0.25f, null), true));
			yield return 0.25f;
			Audio.Play("event:/new_content/game/10_farewell/endscene_attachment_notify");
			this.attachment.Origin = Celeste.TargetCenter;
			this.attachment.Position = Celeste.TargetCenter;
			this.attachment.Visible = true;
			this.attachment.Scale = Vector2.Zero;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.3f)
			{
				this.attachment.Scale.Y = 0.25f + 0.75f * Ease.BigBackOut(p);
				this.attachment.Scale.X = 1.5f - 0.5f * Ease.BigBackOut(p);
				yield return null;
			}
			yield return 0.25f;
			this.ok.Position = new Vector2(1208f, 620f);
			this.ok.Origin = this.ok.Position;
			this.ok.Visible = true;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.25f)
			{
				this.ok.Scale.Y = 0.25f + 0.75f * Ease.BigBackOut(p);
				this.ok.Scale.X = 1.5f - 0.5f * Ease.BigBackOut(p);
				yield return null;
			}
			yield return 0.8f;
			Vector2 from = this.cursor.Position;
			Vector2 to = from + new Vector2(-160f, -190f);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.5f)
			{
				this.cursor.Position = from + (to - from) * Ease.CubeInOut(p);
				yield return null;
			}
			yield return 0.2f;
			from = default(Vector2);
			to = default(Vector2);
			Audio.Play("event:/new_content/game/10_farewell/endscene_attachment_click");
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.25f)
			{
				this.ok.Scale.Y = 1f - Ease.BigBackIn(p);
				this.ok.Scale.X = 1f - Ease.BigBackIn(p);
				yield return null;
			}
			this.ok.Visible = false;
			yield return 0.1f;
			this.picture.Origin = Celeste.TargetCenter;
			this.picture.Position = Celeste.TargetCenter;
			this.picture.Visible = true;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.4f)
			{
				this.picture.Scale.Y = (0.9f + 0.1f * Ease.BigBackOut(p)) * 0.9f;
				this.picture.Scale.X = (1.1f - 0.1f * Ease.BigBackOut(p)) * 0.9f;
				this.picture.Position = Celeste.TargetCenter + Vector2.UnitY * 120f * (1f - Ease.CubeOut(p));
				this.picture.Color = Color.White * p;
				yield return null;
			}
			this.picture.Position = Celeste.TargetCenter;
			this.attachment.Visible = false;
			to = this.cursor.Position;
			from = new Vector2(120f, 30f);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.5f)
			{
				this.cursor.Position = to + (from - to) * Ease.CubeInOut(p);
				yield return null;
			}
			this.cursor.Visible = false;
			to = default(Vector2);
			from = default(Vector2);
			yield return 2f;
			yield break;
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x00029FDC File Offset: 0x000281DC
		public override void OnEnd(Level level)
		{
			ScreenWipe.WipeColor = Color.Black;
			if (Audio.CurrentMusicEventInstance == null)
			{
				Audio.SetMusic("event:/new_content/music/lvl10/cinematic/end", true, true);
			}
			Audio.SetMusicParam("end", 1f);
			this.frame = 21;
			this.zoom = 1f;
			this.fade = 0f;
			this.fadeColor = Color.Black;
			this.center = Celeste.TargetCenter;
			this.picture.Scale = Vector2.One;
			this.picture.Visible = true;
			this.picture.Position = Celeste.TargetCenter;
			this.picture.Origin = Celeste.TargetCenter;
			this.computerFade = 1f;
			this.attachment.Visible = false;
			this.cursor.Visible = false;
			this.ok.Visible = false;
			Audio.Stop(this.cinIntro, true);
			this.cinIntro = null;
			Audio.Stop(this.endAmbience, true);
			this.endAmbience = null;
			List<Coroutine> list = new List<Coroutine>();
			foreach (Coroutine item in base.Components.GetAll<Coroutine>())
			{
				list.Add(item);
			}
			foreach (Coroutine coroutine in list)
			{
				coroutine.RemoveSelf();
			}
			Textbox textbox = base.Scene.Entities.FindFirst<Textbox>();
			if (textbox != null)
			{
				textbox.RemoveSelf();
			}
			this.Level.InCutscene = true;
			this.Level.PauseLock = true;
			this.Level.TimerHidden = true;
			base.Add(new Coroutine(this.EndingRoutine(), true));
		}

		// Token: 0x06000C80 RID: 3200 RVA: 0x0002A1B8 File Offset: 0x000283B8
		private IEnumerator EndingRoutine()
		{
			this.Level.InCutscene = true;
			this.Level.PauseLock = true;
			this.Level.TimerHidden = true;
			yield return 0.5f;
			if (Settings.Instance.SpeedrunClock != SpeedrunType.Off)
			{
				this.showTimer = true;
			}
			while (!Input.MenuConfirm.Pressed)
			{
				yield return null;
			}
			Audio.Play("event:/new_content/game/10_farewell/endscene_final_input");
			this.showTimer = false;
			base.Add(new Coroutine(this.Zoom(1f, 0.75f, 5f, Ease.CubeIn), true));
			base.Add(new Coroutine(this.Fade(0f, 1f, 5f, 0f), true));
			yield return 4f;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 3f)
			{
				Audio.SetMusicParam("fade", 1f - p);
				yield return null;
			}
			Audio.SetMusic(null, true, true);
			yield return 1f;
			if (this.Atlas != null)
			{
				this.Atlas.Dispose();
			}
			this.Atlas = null;
			this.Level.CompleteArea(false, true, true);
			yield break;
		}

		// Token: 0x06000C81 RID: 3201 RVA: 0x0002A1C8 File Offset: 0x000283C8
		public override void Update()
		{
			if (this.counting)
			{
				this.timer += Engine.DeltaTime;
			}
			this.speedrunTimerEase = Calc.Approach(this.speedrunTimerEase, this.showTimer ? 1f : 0f, Engine.DeltaTime * 4f);
			base.Update();
		}

		// Token: 0x06000C82 RID: 3202 RVA: 0x0002A228 File Offset: 0x00028428
		public override void Render()
		{
			Draw.Rect(-100f, -100f, 2120f, 1280f, Color.Black);
			if (this.Atlas != null && this.Frames != null && this.frame < this.Frames.Count)
			{
				MTexture mtexture = this.Frames[this.frame];
				MTexture linkedTexture = this.Atlas.GetLinkedTexture(mtexture.AtlasPath);
				if (linkedTexture != null)
				{
					linkedTexture.DrawJustified(this.center, new Vector2(this.center.X / (float)linkedTexture.Width, this.center.Y / (float)linkedTexture.Height), Color.White, this.zoom);
				}
				mtexture.DrawJustified(this.center, new Vector2(this.center.X / (float)mtexture.Width, this.center.Y / (float)mtexture.Height), Color.White, this.zoom);
				if (this.computerFade > 0f)
				{
					Draw.Rect(0f, 0f, 1920f, 1080f, Color.Black * this.computerFade);
				}
				base.Render();
				AreaComplete.Info(this.speedrunTimerEase, this.speedrunTimerChapterString, this.speedrunTimerFileString, this.chapterSpeedrunText, this.version);
			}
			Draw.Rect(0f, 0f, 1920f, 1080f, this.fadeColor * this.fade);
			if ((base.Scene as Level).Paused)
			{
				Draw.Rect(0f, 0f, 1920f, 1080f, Color.Black * 0.5f);
			}
		}

		// Token: 0x06000C83 RID: 3203 RVA: 0x0002A3F0 File Offset: 0x000285F0
		private List<int> GetFrameData(string data)
		{
			List<int> list = new List<int>();
			string[] array = data.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Contains('*'))
				{
					string[] array2 = array[i].Split(new char[]
					{
						'*'
					});
					int item = int.Parse(array2[0]);
					int num = int.Parse(array2[1]);
					for (int j = 0; j < num; j++)
					{
						list.Add(item);
					}
				}
				else if (array[i].Contains('-'))
				{
					string[] array3 = array[i].Split(new char[]
					{
						'-'
					});
					int num2 = int.Parse(array3[0]);
					int num3 = int.Parse(array3[1]);
					for (int k = num2; k <= num3; k++)
					{
						list.Add(k);
					}
				}
				else
				{
					list.Add(int.Parse(array[i]));
				}
			}
			return list;
		}

		// Token: 0x06000C84 RID: 3204 RVA: 0x0002A4D1 File Offset: 0x000286D1
		private IEnumerator Zoom(float from, float to, float duration, Ease.Easer ease = null)
		{
			if (ease == null)
			{
				ease = Ease.Linear;
			}
			this.zoom = from;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
			{
				this.zoom = from + (to - from) * ease(p);
				if (this.picture != null)
				{
					this.picture.Scale = Vector2.One * this.zoom;
				}
				yield return null;
			}
			this.zoom = to;
			yield break;
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x0002A4FD File Offset: 0x000286FD
		private IEnumerator Play(string data)
		{
			return this.Play(this.GetFrameData(data));
		}

		// Token: 0x06000C86 RID: 3206 RVA: 0x0002A50C File Offset: 0x0002870C
		private IEnumerator Play(List<int> frames)
		{
			int num;
			for (int i = 0; i < frames.Count; i = num + 1)
			{
				this.frame = frames[i];
				yield return 0.083333336f;
				num = i;
			}
			yield break;
		}

		// Token: 0x06000C87 RID: 3207 RVA: 0x0002A522 File Offset: 0x00028722
		private IEnumerator Loop(string data, float duration = -1f)
		{
			List<int> frames = this.GetFrameData(data);
			float time = 0f;
			while (time < duration || duration < 0f)
			{
				this.frame = frames[(int)(time / 0.083333336f) % frames.Count];
				time += Engine.DeltaTime;
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000C88 RID: 3208 RVA: 0x0002A53F File Offset: 0x0002873F
		private IEnumerator Fade(float from, float to, float duration, float delay = 0f)
		{
			this.fade = from;
			yield return delay;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
			{
				this.fade = from + (to - from) * p;
				yield return null;
			}
			this.fade = to;
			yield break;
		}

		// Token: 0x040007E7 RID: 2023
		private const int FPS = 12;

		// Token: 0x040007E8 RID: 2024
		private const float DELAY = 0.083333336f;

		// Token: 0x040007E9 RID: 2025
		private Atlas Atlas;

		// Token: 0x040007EA RID: 2026
		private List<MTexture> Frames;

		// Token: 0x040007EB RID: 2027
		private int frame;

		// Token: 0x040007EC RID: 2028
		private float fade = 1f;

		// Token: 0x040007ED RID: 2029
		private float zoom = 1f;

		// Token: 0x040007EE RID: 2030
		private float computerFade;

		// Token: 0x040007EF RID: 2031
		private Coroutine talkingLoop;

		// Token: 0x040007F0 RID: 2032
		private Vector2 center = Celeste.TargetCenter;

		// Token: 0x040007F1 RID: 2033
		private Coroutine cutscene;

		// Token: 0x040007F2 RID: 2034
		private Color fadeColor = Color.White;

		// Token: 0x040007F3 RID: 2035
		private Image attachment;

		// Token: 0x040007F4 RID: 2036
		private Image cursor;

		// Token: 0x040007F5 RID: 2037
		private Image ok;

		// Token: 0x040007F6 RID: 2038
		private Image picture;

		// Token: 0x040007F7 RID: 2039
		private const float PictureIdleScale = 0.9f;

		// Token: 0x040007F8 RID: 2040
		private float speedrunTimerEase;

		// Token: 0x040007F9 RID: 2041
		private string speedrunTimerChapterString;

		// Token: 0x040007FA RID: 2042
		private string speedrunTimerFileString;

		// Token: 0x040007FB RID: 2043
		private string chapterSpeedrunText = Dialog.Get("OPTIONS_SPEEDRUN_CHAPTER", null) + ":";

		// Token: 0x040007FC RID: 2044
		private string version = Celeste.Instance.Version.ToString();

		// Token: 0x040007FD RID: 2045
		private bool showTimer;

		// Token: 0x040007FE RID: 2046
		private EventInstance endAmbience;

		// Token: 0x040007FF RID: 2047
		private EventInstance cinIntro;

		// Token: 0x04000800 RID: 2048
		private bool counting;

		// Token: 0x04000801 RID: 2049
		private float timer;
	}
}
