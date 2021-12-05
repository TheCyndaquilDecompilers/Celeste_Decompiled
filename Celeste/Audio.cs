using System;
using System.Collections.Generic;
using System.IO;
using FMOD;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200030E RID: 782
	public static class Audio
	{
		// Token: 0x06001894 RID: 6292 RVA: 0x0009ACC8 File Offset: 0x00098EC8
		public static void Init()
		{
			FMOD.Studio.INITFLAGS studioFlags = FMOD.Studio.INITFLAGS.NORMAL;
			if (Settings.Instance.LaunchWithFMODLiveUpdate)
			{
				studioFlags = FMOD.Studio.INITFLAGS.LIVEUPDATE;
			}
			Audio.CheckFmod(FMOD.Studio.System.create(out Audio.system));
			Audio.CheckFmod(Audio.system.initialize(1024, studioFlags, FMOD.INITFLAGS.NORMAL, IntPtr.Zero));
			Audio.attributes3d.forward = new VECTOR
			{
				x = 0f,
				y = 0f,
				z = 1f
			};
			Audio.attributes3d.up = new VECTOR
			{
				x = 0f,
				y = 1f,
				z = 0f
			};
			Audio.SetListenerPosition(new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, -345f));
			Audio.ready = true;
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x0009ADC4 File Offset: 0x00098FC4
		public static void Update()
		{
			if (Audio.system != null && Audio.ready)
			{
				Audio.CheckFmod(Audio.system.update());
			}
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x0009ADE9 File Offset: 0x00098FE9
		public static void Unload()
		{
			if (Audio.system != null)
			{
				Audio.CheckFmod(Audio.system.unloadAll());
				Audio.CheckFmod(Audio.system.release());
				Audio.system = null;
			}
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x0009AE1C File Offset: 0x0009901C
		public static void SetListenerPosition(Vector3 forward, Vector3 up, Vector3 position)
		{
			FMOD.Studio._3D_ATTRIBUTES attributes = default(FMOD.Studio._3D_ATTRIBUTES);
			attributes.forward.x = forward.X;
			attributes.forward.z = forward.Y;
			attributes.forward.z = forward.Z;
			attributes.up.x = up.X;
			attributes.up.y = up.Y;
			attributes.up.z = up.Z;
			attributes.position.x = position.X;
			attributes.position.y = position.Y;
			attributes.position.z = position.Z;
			Audio.system.setListenerAttributes(0, attributes);
		}

		// Token: 0x06001898 RID: 6296 RVA: 0x0009AEE0 File Offset: 0x000990E0
		public static void SetCamera(Camera camera)
		{
			Audio.currentCamera = camera;
		}

		// Token: 0x06001899 RID: 6297 RVA: 0x0009AEE8 File Offset: 0x000990E8
		internal static void CheckFmod(RESULT result)
		{
			if (result != RESULT.OK)
			{
				throw new Exception("FMOD Failed: " + result);
			}
		}

		// Token: 0x0600189A RID: 6298 RVA: 0x0009AF04 File Offset: 0x00099104
		public static EventInstance Play(string path)
		{
			EventInstance eventInstance = Audio.CreateInstance(path, null);
			if (eventInstance != null)
			{
				eventInstance.start();
				eventInstance.release();
			}
			return eventInstance;
		}

		// Token: 0x0600189B RID: 6299 RVA: 0x0009AF3C File Offset: 0x0009913C
		public static EventInstance Play(string path, string param, float value)
		{
			EventInstance eventInstance = Audio.CreateInstance(path, null);
			if (eventInstance != null)
			{
				Audio.SetParameter(eventInstance, param, value);
				eventInstance.start();
				eventInstance.release();
			}
			return eventInstance;
		}

		// Token: 0x0600189C RID: 6300 RVA: 0x0009AF7C File Offset: 0x0009917C
		public static EventInstance Play(string path, Vector2 position)
		{
			EventInstance eventInstance = Audio.CreateInstance(path, new Vector2?(position));
			if (eventInstance != null)
			{
				eventInstance.start();
				eventInstance.release();
			}
			return eventInstance;
		}

		// Token: 0x0600189D RID: 6301 RVA: 0x0009AFB0 File Offset: 0x000991B0
		public static EventInstance Play(string path, Vector2 position, string param, float value)
		{
			EventInstance eventInstance = Audio.CreateInstance(path, new Vector2?(position));
			if (eventInstance != null)
			{
				if (param != null)
				{
					eventInstance.setParameterValue(param, value);
				}
				eventInstance.start();
				eventInstance.release();
			}
			return eventInstance;
		}

		// Token: 0x0600189E RID: 6302 RVA: 0x0009AFF0 File Offset: 0x000991F0
		public static EventInstance Play(string path, Vector2 position, string param, float value, string param2, float value2)
		{
			EventInstance eventInstance = Audio.CreateInstance(path, new Vector2?(position));
			if (eventInstance != null)
			{
				if (param != null)
				{
					eventInstance.setParameterValue(param, value);
				}
				if (param2 != null)
				{
					eventInstance.setParameterValue(param2, value2);
				}
				eventInstance.start();
				eventInstance.release();
			}
			return eventInstance;
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x0009B040 File Offset: 0x00099240
		public static EventInstance Loop(string path)
		{
			EventInstance eventInstance = Audio.CreateInstance(path, null);
			if (eventInstance != null)
			{
				eventInstance.start();
			}
			return eventInstance;
		}

		// Token: 0x060018A0 RID: 6304 RVA: 0x0009B070 File Offset: 0x00099270
		public static EventInstance Loop(string path, string param, float value)
		{
			EventInstance eventInstance = Audio.CreateInstance(path, null);
			if (eventInstance != null)
			{
				eventInstance.setParameterValue(param, value);
				eventInstance.start();
			}
			return eventInstance;
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x0009B0A8 File Offset: 0x000992A8
		public static EventInstance Loop(string path, Vector2 position)
		{
			EventInstance eventInstance = Audio.CreateInstance(path, new Vector2?(position));
			if (eventInstance != null)
			{
				eventInstance.start();
			}
			return eventInstance;
		}

		// Token: 0x060018A2 RID: 6306 RVA: 0x0009B0D4 File Offset: 0x000992D4
		public static EventInstance Loop(string path, Vector2 position, string param, float value)
		{
			EventInstance eventInstance = Audio.CreateInstance(path, new Vector2?(position));
			if (eventInstance != null)
			{
				eventInstance.setParameterValue(param, value);
				eventInstance.start();
			}
			return eventInstance;
		}

		// Token: 0x060018A3 RID: 6307 RVA: 0x0009B108 File Offset: 0x00099308
		public static void Pause(EventInstance instance)
		{
			if (instance != null)
			{
				instance.setPaused(true);
			}
		}

		// Token: 0x060018A4 RID: 6308 RVA: 0x0009B11B File Offset: 0x0009931B
		public static void Resume(EventInstance instance)
		{
			if (instance != null)
			{
				instance.setPaused(false);
			}
		}

		// Token: 0x060018A5 RID: 6309 RVA: 0x0009B130 File Offset: 0x00099330
		public static void Position(EventInstance instance, Vector2 position)
		{
			if (instance != null)
			{
				Vector2 vector = Vector2.Zero;
				if (Audio.currentCamera != null)
				{
					vector = Audio.currentCamera.Position + new Vector2(320f, 180f) / 2f;
				}
				float num = position.X - vector.X;
				if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
				{
					num = -num;
				}
				Audio.attributes3d.position.x = num;
				Audio.attributes3d.position.y = position.Y - vector.Y;
				Audio.attributes3d.position.z = 0f;
				instance.set3DAttributes(Audio.attributes3d);
			}
		}

		// Token: 0x060018A6 RID: 6310 RVA: 0x0009B1F4 File Offset: 0x000993F4
		public static void SetParameter(EventInstance instance, string param, float value)
		{
			if (instance != null)
			{
				instance.setParameterValue(param, value);
			}
		}

		// Token: 0x060018A7 RID: 6311 RVA: 0x0009B208 File Offset: 0x00099408
		public static void Stop(EventInstance instance, bool allowFadeOut = true)
		{
			if (instance != null)
			{
				instance.stop(allowFadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
				instance.release();
			}
		}

		// Token: 0x060018A8 RID: 6312 RVA: 0x0009B228 File Offset: 0x00099428
		public static EventInstance CreateInstance(string path, Vector2? position = null)
		{
			EventDescription eventDescription = Audio.GetEventDescription(path);
			if (eventDescription != null)
			{
				EventInstance eventInstance;
				eventDescription.createInstance(out eventInstance);
				bool flag;
				eventDescription.is3D(out flag);
				if (flag && position != null)
				{
					Audio.Position(eventInstance, position.Value);
				}
				return eventInstance;
			}
			return null;
		}

		// Token: 0x060018A9 RID: 6313 RVA: 0x0009B274 File Offset: 0x00099474
		public static EventDescription GetEventDescription(string path)
		{
			EventDescription eventDescription = null;
			if (path != null && !Audio.cachedEventDescriptions.TryGetValue(path, out eventDescription))
			{
				RESULT @event = Audio.system.getEvent(path, out eventDescription);
				if (@event == RESULT.OK)
				{
					eventDescription.loadSampleData();
					Audio.cachedEventDescriptions.Add(path, eventDescription);
				}
				else if (@event != RESULT.ERR_EVENT_NOTFOUND)
				{
					throw new Exception("FMOD getEvent failed: " + @event);
				}
			}
			return eventDescription;
		}

		// Token: 0x060018AA RID: 6314 RVA: 0x0009B2D8 File Offset: 0x000994D8
		public static void ReleaseUnusedDescriptions()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, EventDescription> keyValuePair in Audio.cachedEventDescriptions)
			{
				int num;
				keyValuePair.Value.getInstanceCount(out num);
				if (num <= 0)
				{
					keyValuePair.Value.unloadSampleData();
					list.Add(keyValuePair.Key);
				}
			}
			foreach (string key in list)
			{
				Audio.cachedEventDescriptions.Remove(key);
			}
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x0009B39C File Offset: 0x0009959C
		public static string GetEventName(EventInstance instance)
		{
			if (instance != null)
			{
				EventDescription eventDescription;
				instance.getDescription(out eventDescription);
				if (eventDescription != null)
				{
					string result = "";
					eventDescription.getPath(out result);
					return result;
				}
			}
			return "";
		}

		// Token: 0x060018AC RID: 6316 RVA: 0x0009B3DC File Offset: 0x000995DC
		public static bool IsPlaying(EventInstance instance)
		{
			if (instance != null)
			{
				PLAYBACK_STATE playback_STATE;
				instance.getPlaybackState(out playback_STATE);
				if (playback_STATE == PLAYBACK_STATE.PLAYING || playback_STATE == PLAYBACK_STATE.STARTING)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060018AD RID: 6317 RVA: 0x0009B408 File Offset: 0x00099608
		public static bool BusPaused(string path, bool? pause = null)
		{
			bool result = false;
			Bus bus;
			if (Audio.system != null && Audio.system.getBus(path, out bus) == RESULT.OK)
			{
				if (pause != null)
				{
					bus.setPaused(pause.Value);
				}
				bus.getPaused(out result);
			}
			return result;
		}

		// Token: 0x060018AE RID: 6318 RVA: 0x0009B454 File Offset: 0x00099654
		public static bool BusMuted(string path, bool? mute)
		{
			bool result = false;
			Bus bus;
			if (Audio.system.getBus(path, out bus) == RESULT.OK)
			{
				if (mute != null)
				{
					bus.setMute(mute.Value);
				}
				bus.getPaused(out result);
			}
			return result;
		}

		// Token: 0x060018AF RID: 6319 RVA: 0x0009B494 File Offset: 0x00099694
		public static void BusStopAll(string path, bool immediate = false)
		{
			Bus bus;
			if (Audio.system != null && Audio.system.getBus(path, out bus) == RESULT.OK)
			{
				bus.stopAllEvents(immediate ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
			}
		}

		// Token: 0x060018B0 RID: 6320 RVA: 0x0009B4CC File Offset: 0x000996CC
		public static float VCAVolume(string path, float? volume = null)
		{
			VCA vca2;
			bool vca = Audio.system.getVCA(path, out vca2) != RESULT.OK;
			float result = 1f;
			float num = 1f;
			if (!vca)
			{
				if (volume != null)
				{
					vca2.setVolume(volume.Value);
				}
				vca2.getVolume(out result, out num);
			}
			return result;
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x0009B518 File Offset: 0x00099718
		public static EventInstance CreateSnapshot(string name, bool start = true)
		{
			EventDescription eventDescription;
			Audio.system.getEvent(name, out eventDescription);
			if (eventDescription == null)
			{
				throw new Exception("Snapshot " + name + " doesn't exist");
			}
			EventInstance eventInstance;
			eventDescription.createInstance(out eventInstance);
			if (start)
			{
				eventInstance.start();
			}
			return eventInstance;
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x0009B566 File Offset: 0x00099766
		public static void ResumeSnapshot(EventInstance snapshot)
		{
			if (snapshot != null)
			{
				snapshot.start();
			}
		}

		// Token: 0x060018B3 RID: 6323 RVA: 0x0009B578 File Offset: 0x00099778
		public static bool IsSnapshotRunning(EventInstance snapshot)
		{
			if (snapshot != null)
			{
				PLAYBACK_STATE playback_STATE;
				snapshot.getPlaybackState(out playback_STATE);
				return playback_STATE == PLAYBACK_STATE.PLAYING || playback_STATE == PLAYBACK_STATE.STARTING || playback_STATE == PLAYBACK_STATE.SUSTAINING;
			}
			return false;
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x0009B5A6 File Offset: 0x000997A6
		public static void EndSnapshot(EventInstance snapshot)
		{
			if (snapshot != null)
			{
				snapshot.stop(STOP_MODE.ALLOWFADEOUT);
			}
		}

		// Token: 0x060018B5 RID: 6325 RVA: 0x0009B5B9 File Offset: 0x000997B9
		public static void ReleaseSnapshot(EventInstance snapshot)
		{
			if (snapshot != null)
			{
				snapshot.stop(STOP_MODE.ALLOWFADEOUT);
				snapshot.release();
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x060018B6 RID: 6326 RVA: 0x0009B5D3 File Offset: 0x000997D3
		public static EventInstance CurrentMusicEventInstance
		{
			get
			{
				return Audio.currentMusicEvent;
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x060018B7 RID: 6327 RVA: 0x0009B5DA File Offset: 0x000997DA
		public static EventInstance CurrentAmbienceEventInstance
		{
			get
			{
				return Audio.currentAmbientEvent;
			}
		}

		// Token: 0x060018B8 RID: 6328 RVA: 0x0009B5E4 File Offset: 0x000997E4
		public static bool SetMusic(string path, bool startPlaying = true, bool allowFadeOut = true)
		{
			if (string.IsNullOrEmpty(path) || path == "null")
			{
				Audio.Stop(Audio.currentMusicEvent, allowFadeOut);
				Audio.currentMusicEvent = null;
				Audio.CurrentMusic = "";
			}
			else if (!Audio.CurrentMusic.Equals(path, StringComparison.OrdinalIgnoreCase))
			{
				Audio.Stop(Audio.currentMusicEvent, allowFadeOut);
				EventInstance eventInstance = Audio.CreateInstance(path, null);
				if (eventInstance != null && startPlaying)
				{
					eventInstance.start();
				}
				Audio.currentMusicEvent = eventInstance;
				Audio.CurrentMusic = Audio.GetEventName(eventInstance);
				return true;
			}
			return false;
		}

		// Token: 0x060018B9 RID: 6329 RVA: 0x0009B674 File Offset: 0x00099874
		public static bool SetAmbience(string path, bool startPlaying = true)
		{
			if (string.IsNullOrEmpty(path) || path == "null")
			{
				Audio.Stop(Audio.currentAmbientEvent, true);
				Audio.currentAmbientEvent = null;
			}
			else if (!Audio.GetEventName(Audio.currentAmbientEvent).Equals(path, StringComparison.OrdinalIgnoreCase))
			{
				Audio.Stop(Audio.currentAmbientEvent, true);
				EventInstance eventInstance = Audio.CreateInstance(path, null);
				if (eventInstance != null && startPlaying)
				{
					eventInstance.start();
				}
				Audio.currentAmbientEvent = eventInstance;
				return true;
			}
			return false;
		}

		// Token: 0x060018BA RID: 6330 RVA: 0x0009B6F2 File Offset: 0x000998F2
		public static void SetMusicParam(string path, float value)
		{
			if (Audio.currentMusicEvent != null)
			{
				Audio.currentMusicEvent.setParameterValue(path, value);
			}
		}

		// Token: 0x060018BB RID: 6331 RVA: 0x0009B710 File Offset: 0x00099910
		public static void SetAltMusic(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				Audio.EndSnapshot(Audio.mainDownSnapshot);
				Audio.Stop(Audio.currentAltMusicEvent, true);
				Audio.currentAltMusicEvent = null;
				return;
			}
			if (!Audio.GetEventName(Audio.currentAltMusicEvent).Equals(path, StringComparison.OrdinalIgnoreCase))
			{
				Audio.StartMainDownSnapshot();
				Audio.Stop(Audio.currentAltMusicEvent, true);
				Audio.currentAltMusicEvent = Audio.Loop(path);
			}
		}

		// Token: 0x060018BC RID: 6332 RVA: 0x0009B76F File Offset: 0x0009996F
		private static void StartMainDownSnapshot()
		{
			if (Audio.mainDownSnapshot == null)
			{
				Audio.mainDownSnapshot = Audio.CreateSnapshot("snapshot:/music_mains_mute", true);
				return;
			}
			Audio.ResumeSnapshot(Audio.mainDownSnapshot);
		}

		// Token: 0x060018BD RID: 6333 RVA: 0x0009B799 File Offset: 0x00099999
		private static void EndMainDownSnapshot()
		{
			Audio.EndSnapshot(Audio.mainDownSnapshot);
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x060018BE RID: 6334 RVA: 0x0009B7A8 File Offset: 0x000999A8
		// (set) Token: 0x060018BF RID: 6335 RVA: 0x0009B7C8 File Offset: 0x000999C8
		public static float MusicVolume
		{
			get
			{
				return Audio.VCAVolume("vca:/music", null);
			}
			set
			{
				Audio.VCAVolume("vca:/music", new float?(value));
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x060018C0 RID: 6336 RVA: 0x0009B7DC File Offset: 0x000999DC
		// (set) Token: 0x060018C1 RID: 6337 RVA: 0x0009B7FC File Offset: 0x000999FC
		public static float SfxVolume
		{
			get
			{
				return Audio.VCAVolume("vca:/gameplay_sfx", null);
			}
			set
			{
				Audio.VCAVolume("vca:/gameplay_sfx", new float?(value));
				Audio.VCAVolume("vca:/ui_sfx", new float?(value));
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x060018C2 RID: 6338 RVA: 0x0009B820 File Offset: 0x00099A20
		// (set) Token: 0x060018C3 RID: 6339 RVA: 0x0009B840 File Offset: 0x00099A40
		public static bool PauseMusic
		{
			get
			{
				return Audio.BusPaused("bus:/music", null);
			}
			set
			{
				Audio.BusPaused("bus:/music", new bool?(value));
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x060018C4 RID: 6340 RVA: 0x0009B854 File Offset: 0x00099A54
		// (set) Token: 0x060018C5 RID: 6341 RVA: 0x0009B874 File Offset: 0x00099A74
		public static bool PauseGameplaySfx
		{
			get
			{
				return Audio.BusPaused("bus:/gameplay_sfx", null);
			}
			set
			{
				Audio.BusPaused("bus:/gameplay_sfx", new bool?(value));
				Audio.BusPaused("bus:/music/stings", new bool?(value));
			}
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x060018C6 RID: 6342 RVA: 0x0009B898 File Offset: 0x00099A98
		// (set) Token: 0x060018C7 RID: 6343 RVA: 0x0009B8B8 File Offset: 0x00099AB8
		public static bool PauseUISfx
		{
			get
			{
				return Audio.BusPaused("bus:/ui_sfx", null);
			}
			set
			{
				Audio.BusPaused("bus:/ui_sfx", new bool?(value));
			}
		}

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x060018C8 RID: 6344 RVA: 0x0009B8CB File Offset: 0x00099ACB
		// (set) Token: 0x060018C9 RID: 6345 RVA: 0x0009B8D4 File Offset: 0x00099AD4
		public static bool MusicUnderwater
		{
			get
			{
				return Audio.musicUnderwater;
			}
			set
			{
				if (Audio.musicUnderwater != value)
				{
					Audio.musicUnderwater = value;
					if (Audio.musicUnderwater)
					{
						if (Audio.musicUnderwaterSnapshot == null)
						{
							Audio.musicUnderwaterSnapshot = Audio.CreateSnapshot("snapshot:/underwater", true);
							return;
						}
						Audio.ResumeSnapshot(Audio.musicUnderwaterSnapshot);
						return;
					}
					else
					{
						Audio.EndSnapshot(Audio.musicUnderwaterSnapshot);
					}
				}
			}
		}

		// Token: 0x04001543 RID: 5443
		private static FMOD.Studio.System system;

		// Token: 0x04001544 RID: 5444
		private static FMOD.Studio._3D_ATTRIBUTES attributes3d = default(FMOD.Studio._3D_ATTRIBUTES);

		// Token: 0x04001545 RID: 5445
		public static Dictionary<string, EventDescription> cachedEventDescriptions = new Dictionary<string, EventDescription>();

		// Token: 0x04001546 RID: 5446
		private static Camera currentCamera;

		// Token: 0x04001547 RID: 5447
		private static bool ready;

		// Token: 0x04001548 RID: 5448
		private static EventInstance currentMusicEvent = null;

		// Token: 0x04001549 RID: 5449
		private static EventInstance currentAltMusicEvent = null;

		// Token: 0x0400154A RID: 5450
		private static EventInstance currentAmbientEvent = null;

		// Token: 0x0400154B RID: 5451
		private static EventInstance mainDownSnapshot = null;

		// Token: 0x0400154C RID: 5452
		public static string CurrentMusic = "";

		// Token: 0x0400154D RID: 5453
		private static bool musicUnderwater;

		// Token: 0x0400154E RID: 5454
		private static EventInstance musicUnderwaterSnapshot;

		// Token: 0x020006D9 RID: 1753
		public static class Banks
		{
			// Token: 0x06002D24 RID: 11556 RVA: 0x0011E3F4 File Offset: 0x0011C5F4
			public static Bank Load(string name, bool loadStrings)
			{
				string str = Path.Combine(Engine.ContentDirectory, "FMOD", "Desktop", name);
				Bank bank;
				Audio.CheckFmod(Audio.system.loadBankFile(str + ".bank", LOAD_BANK_FLAGS.NORMAL, out bank));
				bank.loadSampleData();
				if (loadStrings)
				{
					Bank bank2;
					Audio.CheckFmod(Audio.system.loadBankFile(str + ".strings.bank", LOAD_BANK_FLAGS.NORMAL, out bank2));
				}
				return bank;
			}

			// Token: 0x04002C67 RID: 11367
			public static Bank Master;

			// Token: 0x04002C68 RID: 11368
			public static Bank Music;

			// Token: 0x04002C69 RID: 11369
			public static Bank Sfxs;

			// Token: 0x04002C6A RID: 11370
			public static Bank UI;

			// Token: 0x04002C6B RID: 11371
			public static Bank DlcMusic;

			// Token: 0x04002C6C RID: 11372
			public static Bank DlcSfxs;
		}
	}
}
