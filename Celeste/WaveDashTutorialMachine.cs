using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001DF RID: 479
	public class WaveDashTutorialMachine : JumpThru
	{
		// Token: 0x06001013 RID: 4115 RVA: 0x00045180 File Offset: 0x00043380
		public WaveDashTutorialMachine(Vector2 position) : base(position, 88, true)
		{
			base.Tag = Tags.TransitionUpdate;
			base.Depth = 1000;
			base.Hitbox.Position = new Vector2(-41f, -59f);
			base.Add(this.backSprite = new Image(GFX.Game["objects/wavedashtutorial/building_back"]));
			this.backSprite.JustifyOrigin(0.5f, 1f);
			base.Add(this.noise = new Sprite(GFX.Game, "objects/wavedashtutorial/noise"));
			this.noise.AddLoop("static", "", 0.05f);
			this.noise.Play("static", false, false);
			this.noise.CenterOrigin();
			this.noise.Position = new Vector2(0f, -30f);
			this.noise.Color = Color.White * 0.5f;
			base.Add(this.frontLeftSprite = new Image(GFX.Game["objects/wavedashtutorial/building_front_left"]));
			this.frontLeftSprite.JustifyOrigin(0.5f, 1f);
			base.Add(this.talk = new TalkComponent(new Rectangle(-12, -8, 24, 8), new Vector2(0f, -50f), new Action<Player>(this.OnInteract), null));
			this.talk.Enabled = false;
			this.SurfaceSoundIndex = 42;
		}

		// Token: 0x06001014 RID: 4116 RVA: 0x00045318 File Offset: 0x00043518
		public WaveDashTutorialMachine(EntityData data, Vector2 position) : this(data.Position + position)
		{
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x0004532C File Offset: 0x0004352C
		public override void Added(Scene scene)
		{
			base.Added(scene);
			scene.Add(this.frontEntity = new Entity(this.Position));
			this.frontEntity.Tag = Tags.TransitionUpdate;
			this.frontEntity.Depth = -10500;
			this.frontEntity.Add(this.frontRightSprite = new Image(GFX.Game["objects/wavedashtutorial/building_front_right"]));
			this.frontRightSprite.JustifyOrigin(0.5f, 1f);
			this.frontEntity.Add(this.neon = new Sprite(GFX.Game, "objects/wavedashtutorial/neon_"));
			this.neon.AddLoop("loop", "", 0.07f);
			this.neon.Play("loop", false, false);
			this.neon.JustifyOrigin(0.5f, 1f);
			scene.Add(this.frontWall = new Solid(this.Position + new Vector2(-41f, -59f), 88f, 38f, true));
			this.frontWall.SurfaceSoundIndex = 42;
		}

		// Token: 0x06001016 RID: 4118 RVA: 0x00045468 File Offset: 0x00043668
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			base.Add(this.signSfx = new SoundSource(new Vector2(8f, -16f), "event:/new_content/env/local/cafe_sign"));
		}

		// Token: 0x06001017 RID: 4119 RVA: 0x000454A4 File Offset: 0x000436A4
		public override void Update()
		{
			base.Update();
			if (!this.inCutscene)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					this.frontWall.Collidable = true;
					bool flag = (entity.X > base.X - 37f && entity.X < base.X + 46f && entity.Y > base.Y - 58f) || this.frontWall.CollideCheck(entity);
					if (flag != this.playerInside)
					{
						this.playerInside = flag;
						if (this.playerInside)
						{
							this.signSfx.Stop(true);
							this.snapshot = Audio.CreateSnapshot("snapshot:/game_10_inside_cafe", true);
						}
						else
						{
							this.signSfx.Play("event:/new_content/env/local/cafe_sign", null, 0f);
							Audio.ReleaseSnapshot(this.snapshot);
							this.snapshot = null;
						}
					}
				}
				base.SceneAs<Level>().ZoomSnap(new Vector2(160f, 90f), 1f + Ease.QuadInOut(this.cameraEase) * 0.75f);
			}
			this.talk.Enabled = this.playerInside;
			this.frontWall.Collidable = !this.playerInside;
			this.insideEase = Calc.Approach(this.insideEase, this.playerInside ? 1f : 0f, Engine.DeltaTime * 4f);
			this.cameraEase = Calc.Approach(this.cameraEase, this.playerInside ? 1f : 0f, Engine.DeltaTime * 2f);
			this.frontRightSprite.Color = Color.White * (1f - this.insideEase);
			this.frontLeftSprite.Color = this.frontRightSprite.Color;
			this.neon.Color = this.frontRightSprite.Color;
			this.frontRightSprite.Visible = (this.insideEase < 1f);
			this.frontLeftSprite.Visible = (this.insideEase < 1f);
			this.neon.Visible = (this.insideEase < 1f);
			if (base.Scene.OnInterval(0.05f))
			{
				this.noise.Scale = Calc.Random.Choose(new Vector2(1f, 1f), new Vector2(-1f, 1f), new Vector2(1f, -1f), new Vector2(-1f, -1f));
			}
		}

		// Token: 0x06001018 RID: 4120 RVA: 0x00045748 File Offset: 0x00043948
		private void OnInteract(Player player)
		{
			if (!this.inCutscene)
			{
				Level level = base.Scene as Level;
				if (this.usingSfx != null)
				{
					Audio.SetParameter(this.usingSfx, "end", 1f);
					Audio.Stop(this.usingSfx, true);
				}
				this.inCutscene = true;
				this.interactStartZoom = level.ZoomTarget;
				level.StartCutscene(new Action<Level>(this.SkipInteraction), true, false, false);
				base.Add(this.routine = new Coroutine(this.InteractRoutine(player), true));
			}
		}

		// Token: 0x06001019 RID: 4121 RVA: 0x000457DC File Offset: 0x000439DC
		private IEnumerator InteractRoutine(Player player)
		{
			Level level = base.Scene as Level;
			player.StateMachine.State = 11;
			player.StateMachine.Locked = true;
			yield return CutsceneEntity.CameraTo(new Vector2(base.X, base.Y - 30f) - new Vector2(160f, 90f), 0.25f, Ease.CubeOut, 0f);
			yield return level.ZoomTo(new Vector2(160f, 90f), 10f, 1f);
			this.usingSfx = Audio.Play("event:/state/cafe_computer_active", player.Position);
			Audio.Play("event:/new_content/game/10_farewell/cafe_computer_on", player.Position);
			Audio.Play("event:/new_content/game/10_farewell/cafe_computer_startupsfx", player.Position);
			this.presentation = new WaveDashPresentation(this.usingSfx);
			base.Scene.Add(this.presentation);
			while (this.presentation.Viewing)
			{
				yield return null;
			}
			yield return level.ZoomTo(new Vector2(160f, 90f), this.interactStartZoom, 1f);
			player.StateMachine.Locked = false;
			player.StateMachine.State = 0;
			this.inCutscene = false;
			level.EndCutscene();
			Audio.SetAltMusic(null);
			yield break;
		}

		// Token: 0x0600101A RID: 4122 RVA: 0x000457F4 File Offset: 0x000439F4
		private void SkipInteraction(Level level)
		{
			Audio.SetAltMusic(null);
			this.inCutscene = false;
			level.ZoomSnap(new Vector2(160f, 90f), this.interactStartZoom);
			if (this.usingSfx != null)
			{
				Audio.SetParameter(this.usingSfx, "end", 1f);
				this.usingSfx.release();
			}
			if (this.presentation != null)
			{
				this.presentation.RemoveSelf();
			}
			this.presentation = null;
			if (this.routine != null)
			{
				this.routine.RemoveSelf();
			}
			this.routine = null;
			Player entity = level.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				entity.StateMachine.Locked = false;
				entity.StateMachine.State = 0;
			}
		}

		// Token: 0x0600101B RID: 4123 RVA: 0x000458B3 File Offset: 0x00043AB3
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			this.Dispose();
		}

		// Token: 0x0600101C RID: 4124 RVA: 0x000458C2 File Offset: 0x00043AC2
		public override void SceneEnd(Scene scene)
		{
			base.SceneEnd(scene);
			this.Dispose();
		}

		// Token: 0x0600101D RID: 4125 RVA: 0x000458D4 File Offset: 0x00043AD4
		private void Dispose()
		{
			if (this.usingSfx != null)
			{
				Audio.SetParameter(this.usingSfx, "quit", 1f);
				this.usingSfx.release();
				this.usingSfx = null;
			}
			Audio.ReleaseSnapshot(this.snapshot);
			this.snapshot = null;
		}

		// Token: 0x04000B6D RID: 2925
		private Entity frontEntity;

		// Token: 0x04000B6E RID: 2926
		private Image backSprite;

		// Token: 0x04000B6F RID: 2927
		private Image frontRightSprite;

		// Token: 0x04000B70 RID: 2928
		private Image frontLeftSprite;

		// Token: 0x04000B71 RID: 2929
		private Sprite noise;

		// Token: 0x04000B72 RID: 2930
		private Sprite neon;

		// Token: 0x04000B73 RID: 2931
		private Solid frontWall;

		// Token: 0x04000B74 RID: 2932
		private float insideEase;

		// Token: 0x04000B75 RID: 2933
		private float cameraEase;

		// Token: 0x04000B76 RID: 2934
		private bool playerInside;

		// Token: 0x04000B77 RID: 2935
		private bool inCutscene;

		// Token: 0x04000B78 RID: 2936
		private Coroutine routine;

		// Token: 0x04000B79 RID: 2937
		private WaveDashPresentation presentation;

		// Token: 0x04000B7A RID: 2938
		private float interactStartZoom;

		// Token: 0x04000B7B RID: 2939
		private EventInstance snapshot;

		// Token: 0x04000B7C RID: 2940
		private EventInstance usingSfx;

		// Token: 0x04000B7D RID: 2941
		private SoundSource signSfx;

		// Token: 0x04000B7E RID: 2942
		private TalkComponent talk;
	}
}
