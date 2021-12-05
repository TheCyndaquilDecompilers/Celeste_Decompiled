using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000191 RID: 401
	public class CS04_MirrorPortal : CutsceneEntity
	{
		// Token: 0x06000DEA RID: 3562 RVA: 0x00031B2E File Offset: 0x0002FD2E
		public CS04_MirrorPortal(Player player, TempleMirrorPortal portal) : base(true, false)
		{
			this.player = player;
			this.portal = portal;
		}

		// Token: 0x06000DEB RID: 3563 RVA: 0x00031B48 File Offset: 0x0002FD48
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
			level.Add(this.fader = new CS04_MirrorPortal.Fader());
		}

		// Token: 0x06000DEC RID: 3564 RVA: 0x00031B7C File Offset: 0x0002FD7C
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			this.player.Dashes = 1;
			if (level.Session.Area.Mode == AreaMode.Normal)
			{
				Audio.SetMusic(null, true, true);
			}
			else
			{
				base.Add(new Coroutine(this.MusicFadeOutBSide(), true));
			}
			base.Add(this.sfx = new SoundSource());
			this.sfx.Position = this.portal.Center;
			this.sfx.Play("event:/music/lvl5/mirror_cutscene", null, 0f);
			base.Add(new Coroutine(this.CenterCamera(), true));
			yield return this.player.DummyWalkToExact((int)this.portal.X, false, 1f, false);
			yield return 0.25f;
			yield return this.player.DummyWalkToExact((int)this.portal.X - 16, false, 1f, false);
			yield return 0.5f;
			yield return this.player.DummyWalkToExact((int)this.portal.X + 16, false, 1f, false);
			yield return 0.25f;
			this.player.Facing = Facings.Left;
			yield return 0.25f;
			yield return this.player.DummyWalkToExact((int)this.portal.X, false, 1f, false);
			yield return 0.1f;
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("lookUp", false, false);
			yield return 1f;
			this.player.DummyAutoAnimate = true;
			this.portal.Activate();
			base.Add(new Coroutine(level.ZoomTo(new Vector2(160f, 90f), 3f, 12f), true));
			yield return 0.25f;
			this.player.ForceStrongWindHair.X = -1f;
			yield return this.player.DummyWalkToExact((int)this.player.X + 12, true, 1f, false);
			yield return 0.5f;
			this.player.Facing = Facings.Right;
			this.player.DummyAutoAnimate = false;
			this.player.DummyGravity = false;
			this.player.Sprite.Play("runWind", false, false);
			while (this.player.Sprite.Rate > 0f)
			{
				this.player.MoveH(this.player.Sprite.Rate * 10f * Engine.DeltaTime, null, null);
				this.player.MoveV(-(1f - this.player.Sprite.Rate) * 6f * Engine.DeltaTime, null, null);
				this.player.Sprite.Rate -= Engine.DeltaTime * 0.15f;
				yield return null;
			}
			yield return 0.5f;
			this.player.Sprite.Play("fallFast", false, false);
			this.player.Sprite.Rate = 1f;
			Vector2 target = this.portal.Center + new Vector2(0f, 8f);
			Vector2 from = this.player.Position;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 2f)
			{
				this.player.Position = from + (target - from) * Ease.SineInOut(p);
				yield return null;
			}
			this.player.ForceStrongWindHair.X = 0f;
			target = default(Vector2);
			from = default(Vector2);
			this.fader.Target = 1f;
			yield return 2f;
			this.player.Sprite.Play("sleep", false, false);
			yield return 1f;
			yield return level.ZoomBack(1f);
			if (level.Session.Area.Mode == AreaMode.Normal)
			{
				level.Session.ColorGrade = "templevoid";
				for (float p = 0f; p < 1f; p += Engine.DeltaTime)
				{
					Glitch.Value = p * 0.05f;
					level.ScreenPadding = 32f * p;
					yield return null;
				}
			}
			while ((this.portal.DistortionFade -= Engine.DeltaTime * 2f) > 0f)
			{
				yield return null;
			}
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000DED RID: 3565 RVA: 0x00031B92 File Offset: 0x0002FD92
		private IEnumerator CenterCamera()
		{
			Camera camera = this.Level.Camera;
			Vector2 target = this.portal.Center - new Vector2(160f, 90f);
			while ((camera.Position - target).Length() > 1f)
			{
				camera.Position += (target - camera.Position) * (1f - (float)Math.Pow(0.009999999776482582, (double)Engine.DeltaTime));
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000DEE RID: 3566 RVA: 0x00031BA1 File Offset: 0x0002FDA1
		private IEnumerator MusicFadeOutBSide()
		{
			for (float p = 1f; p > 0f; p -= Engine.DeltaTime)
			{
				Audio.SetMusicParam("fade", p);
				yield return null;
			}
			Audio.SetMusicParam("fade", 0f);
			yield break;
		}

		// Token: 0x06000DEF RID: 3567 RVA: 0x00031BAC File Offset: 0x0002FDAC
		public override void OnEnd(Level level)
		{
			level.OnEndOfFrame += delegate()
			{
				if (this.fader != null && !this.WasSkipped)
				{
					this.fader.Tag = Tags.Global;
					this.fader.Target = 0f;
					this.fader.Ended = true;
				}
				Leader.StoreStrawberries(this.player.Leader);
				level.Remove(this.player);
				level.UnloadLevel();
				level.Session.Dreaming = true;
				level.Session.Keys.Clear();
				if (level.Session.Area.Mode == AreaMode.Normal)
				{
					level.Session.Level = "void";
					level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float)level.Bounds.Left, (float)level.Bounds.Top)));
					level.LoadLevel(Player.IntroTypes.TempleMirrorVoid, false);
				}
				else
				{
					level.Session.Level = "c-00";
					level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float)level.Bounds.Left, (float)level.Bounds.Top)));
					level.LoadLevel(Player.IntroTypes.WakeUp, false);
					Audio.SetMusicParam("fade", 1f);
				}
				Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
				level.Camera.Y -= 8f;
				if (!this.WasSkipped && level.Wipe != null)
				{
					level.Wipe.Cancel();
				}
				if (this.fader != null)
				{
					this.fader.RemoveTag(Tags.Global);
				}
			};
		}

		// Token: 0x04000935 RID: 2357
		private Player player;

		// Token: 0x04000936 RID: 2358
		private TempleMirrorPortal portal;

		// Token: 0x04000937 RID: 2359
		private CS04_MirrorPortal.Fader fader;

		// Token: 0x04000938 RID: 2360
		private SoundSource sfx;

		// Token: 0x0200046E RID: 1134
		private class Fader : Entity
		{
			// Token: 0x06002250 RID: 8784 RVA: 0x000E9AC2 File Offset: 0x000E7CC2
			public Fader()
			{
				base.Depth = -1000000;
			}

			// Token: 0x06002251 RID: 8785 RVA: 0x000E9AD8 File Offset: 0x000E7CD8
			public override void Update()
			{
				this.fade = Calc.Approach(this.fade, this.Target, Engine.DeltaTime * 0.5f);
				if (this.Target <= 0f && this.fade <= 0f && this.Ended)
				{
					base.RemoveSelf();
				}
				base.Update();
			}

			// Token: 0x06002252 RID: 8786 RVA: 0x000E9B38 File Offset: 0x000E7D38
			public override void Render()
			{
				Camera camera = (base.Scene as Level).Camera;
				if (this.fade > 0f)
				{
					Draw.Rect(camera.X - 10f, camera.Y - 10f, 340f, 200f, Color.Black * this.fade);
				}
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && !entity.OnGround(2))
				{
					entity.Render();
				}
			}

			// Token: 0x04002236 RID: 8758
			public float Target;

			// Token: 0x04002237 RID: 8759
			public bool Ended;

			// Token: 0x04002238 RID: 8760
			private float fade;
		}
	}
}
