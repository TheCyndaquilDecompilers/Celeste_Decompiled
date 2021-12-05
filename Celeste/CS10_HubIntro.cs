using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200016A RID: 362
	public class CS10_HubIntro : CutsceneEntity
	{
		// Token: 0x06000CEE RID: 3310 RVA: 0x0002BC20 File Offset: 0x00029E20
		public CS10_HubIntro(Scene scene, Player player) : base(true, false)
		{
			this.player = player;
			this.spawn = (scene as Level).GetSpawnPoint(player.Position);
			this.locks = scene.Entities.FindAll<LockBlock>();
			this.locks.Sort((LockBlock a, LockBlock b) => (int)(a.Y - b.Y));
			foreach (LockBlock lockBlock in this.locks)
			{
				lockBlock.Visible = false;
			}
			this.booster = scene.Entities.FindFirst<Booster>();
			if (this.booster != null)
			{
				this.booster.Visible = false;
			}
		}

		// Token: 0x06000CEF RID: 3311 RVA: 0x0002BD04 File Offset: 0x00029F04
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x0002BD19 File Offset: 0x00029F19
		private IEnumerator Cutscene(Level level)
		{
			if (this.player.Holding != null)
			{
				this.player.Throw();
			}
			this.player.StateMachine.State = 11;
			this.player.ForceCameraUpdate = true;
			while (!this.player.OnGround(1))
			{
				yield return null;
			}
			this.player.ForceCameraUpdate = false;
			yield return 0.1f;
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("lookUp", false, false);
			yield return 0.25f;
			level.Add(this.bird = new CS10_HubIntro.Bird(new Vector2(this.spawn.X, (float)level.Bounds.Top + 190f)));
			Audio.Play("event:/new_content/game/10_farewell/bird_camera_pan_up");
			yield return CutsceneEntity.CameraTo(new Vector2(this.spawn.X - 160f, (float)level.Bounds.Top + 190f - 90f), 2f, Ease.CubeInOut, 0f);
			yield return this.bird.IdleRoutine();
			base.Add(new Coroutine(CutsceneEntity.CameraTo(new Vector2(level.Camera.X, (float)level.Bounds.Top), 0.8f, Ease.CubeInOut, 0.1f), true));
			Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
			yield return this.bird.FlyAwayRoutine();
			this.bird.RemoveSelf();
			this.bird = null;
			yield return 0.5f;
			float duration = 6f;
			string sfx = "event:/new_content/game/10_farewell/locked_door_appear_1".Substring(0, "event:/new_content/game/10_farewell/locked_door_appear_1".Length - 1);
			int doorIndex = 1;
			base.Add(new Coroutine(CutsceneEntity.CameraTo(new Vector2(level.Camera.X, (float)(level.Bounds.Bottom - 180)), duration, Ease.SineInOut, 0f), true));
			base.Add(new Coroutine(this.Level.ZoomTo(new Vector2(160f, 90f), 1.5f, duration), true));
			for (float t = 0f; t < duration; t += Engine.DeltaTime)
			{
				foreach (LockBlock lockBlock in this.locks)
				{
					if (!lockBlock.Visible && level.Camera.Y + 90f > lockBlock.Y - 20f)
					{
						this.sfxs.Add(Audio.Play(sfx + doorIndex, lockBlock.Center));
						lockBlock.Appear();
						Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
						int num = doorIndex;
						doorIndex = num + 1;
					}
				}
				yield return null;
			}
			sfx = null;
			yield return 0.5f;
			if (this.booster != null)
			{
				Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
				this.booster.Appear();
			}
			yield return 0.3f;
			yield return this.Level.ZoomBack(0.3f);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x0002BD30 File Offset: 0x00029F30
		public override void OnEnd(Level level)
		{
			if (this.WasSkipped)
			{
				foreach (EventInstance instance in this.sfxs)
				{
					Audio.Stop(instance, true);
				}
				if (this.bird != null)
				{
					Audio.Stop(this.bird.sfx, true);
				}
			}
			foreach (LockBlock lockBlock in this.locks)
			{
				lockBlock.Visible = true;
			}
			if (this.booster != null)
			{
				this.booster.Visible = true;
			}
			if (this.bird != null)
			{
				this.bird.RemoveSelf();
			}
			if (this.WasSkipped)
			{
				this.player.Position = this.spawn;
			}
			this.player.Speed = Vector2.Zero;
			this.player.DummyAutoAnimate = true;
			this.player.ForceCameraUpdate = false;
			this.player.StateMachine.State = 0;
			level.Camera.Y = (float)(level.Bounds.Bottom - 180);
			level.Session.SetFlag("hub_intro", true);
			level.ResetZoom();
		}

		// Token: 0x0400083C RID: 2108
		public const string Flag = "hub_intro";

		// Token: 0x0400083D RID: 2109
		public const float BirdOffset = 190f;

		// Token: 0x0400083E RID: 2110
		private Player player;

		// Token: 0x0400083F RID: 2111
		private List<LockBlock> locks;

		// Token: 0x04000840 RID: 2112
		private Booster booster;

		// Token: 0x04000841 RID: 2113
		private CS10_HubIntro.Bird bird;

		// Token: 0x04000842 RID: 2114
		private Vector2 spawn;

		// Token: 0x04000843 RID: 2115
		private List<EventInstance> sfxs = new List<EventInstance>();

		// Token: 0x02000425 RID: 1061
		private class Bird : Entity
		{
			// Token: 0x060020CD RID: 8397 RVA: 0x000E1C7C File Offset: 0x000DFE7C
			public Bird(Vector2 position)
			{
				this.Position = position;
				base.Depth = -8500;
				base.Add(this.sprite = GFX.SpriteBank.Create("bird"));
				this.sprite.Play("hover", false, false);
				this.sprite.OnFrameChange = delegate(string spr)
				{
					BirdNPC.FlapSfxCheck(this.sprite);
				};
			}

			// Token: 0x060020CE RID: 8398 RVA: 0x000E1CE8 File Offset: 0x000DFEE8
			public IEnumerator IdleRoutine()
			{
				yield return 0.5f;
				yield break;
			}

			// Token: 0x060020CF RID: 8399 RVA: 0x000E1CF0 File Offset: 0x000DFEF0
			public IEnumerator FlyAwayRoutine()
			{
				Level level = base.Scene as Level;
				this.sfx = Audio.Play("event:/new_content/game/10_farewell/bird_fly_uptonext", this.Position);
				this.sprite.Play("flyup", false, false);
				float spd = -32f;
				while (base.Y > (float)(level.Bounds.Top - 32))
				{
					spd -= 400f * Engine.DeltaTime;
					base.Y += spd * Engine.DeltaTime;
					yield return null;
				}
				yield break;
			}

			// Token: 0x0400210A RID: 8458
			private Sprite sprite;

			// Token: 0x0400210B RID: 8459
			public EventInstance sfx;
		}
	}
}
