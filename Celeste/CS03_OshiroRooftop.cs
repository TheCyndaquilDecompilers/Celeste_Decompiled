using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000284 RID: 644
	public class CS03_OshiroRooftop : CutsceneEntity
	{
		// Token: 0x060013E6 RID: 5094 RVA: 0x0006BC74 File Offset: 0x00069E74
		public CS03_OshiroRooftop(NPC oshiro) : base(true, false)
		{
			this.oshiro = oshiro;
		}

		// Token: 0x060013E7 RID: 5095 RVA: 0x0006BC9C File Offset: 0x00069E9C
		public override void OnBegin(Level level)
		{
			this.bossSpawnPosition = new Vector2(this.oshiro.X, (float)(level.Bounds.Bottom - 40));
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x060013E8 RID: 5096 RVA: 0x0006BCE4 File Offset: 0x00069EE4
		private IEnumerator Cutscene(Level level)
		{
			while (this.player == null)
			{
				this.player = base.Scene.Tracker.GetEntity<Player>();
				if (this.player != null)
				{
					break;
				}
				yield return null;
			}
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			while (!this.player.OnGround(1) || this.player.Speed.Y < 0f)
			{
				yield return null;
			}
			yield return 0.6f;
			this.evil = new BadelineDummy(new Vector2(this.oshiro.X - 40f, (float)(level.Bounds.Bottom - 60)));
			this.evil.Sprite.Scale.X = 1f;
			this.evil.Appear(level, false);
			level.Add(this.evil);
			yield return 0.1f;
			this.player.Facing = Facings.Left;
			yield return Textbox.Say("CH3_OSHIRO_START_CHASE", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.MaddyWalkAway),
				new Func<IEnumerator>(this.MaddyTurnAround),
				new Func<IEnumerator>(this.EnterOshiro),
				new Func<IEnumerator>(this.OshiroGetsAngry)
			});
			yield return this.OshiroTransform();
			base.Add(new Coroutine(this.AnxietyAndCameraOut(), true));
			yield return level.ZoomBack(0.5f);
			yield return 0.25f;
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x060013E9 RID: 5097 RVA: 0x0006BCFA File Offset: 0x00069EFA
		private IEnumerator MaddyWalkAway()
		{
			Level level = base.Scene as Level;
			base.Add(new Coroutine(this.player.DummyWalkTo((float)level.Bounds.Left + 170f, false, 1f, false), true));
			yield return 0.2f;
			Audio.Play("event:/game/03_resort/suite_bad_moveroof", this.evil.Position);
			base.Add(new Coroutine(this.evil.FloatTo(this.evil.Position + new Vector2(80f, 30f), null, true, false, false), true));
			yield return null;
			yield break;
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x0006BD09 File Offset: 0x00069F09
		private IEnumerator MaddyTurnAround()
		{
			yield return 0.25f;
			this.player.Facing = Facings.Left;
			yield return 0.1f;
			Level level = base.SceneAs<Level>();
			yield return level.ZoomTo(new Vector2(150f, this.bossSpawnPosition.Y - (float)level.Bounds.Y - 8f), 2f, 0.5f);
			yield break;
		}

		// Token: 0x060013EB RID: 5099 RVA: 0x0006BD18 File Offset: 0x00069F18
		private IEnumerator EnterOshiro()
		{
			yield return 0.3f;
			this.bossSpriteOffset = (this.bossSprite.Justify.Value.Y - this.oshiro.Sprite.Justify.Value.Y) * this.bossSprite.Height;
			this.oshiro.Visible = true;
			this.oshiro.Sprite.Scale.X = 1f;
			base.Add(new Coroutine(this.oshiro.MoveTo(this.bossSpawnPosition - new Vector2(0f, this.bossSpriteOffset), false, null, false), true));
			this.oshiro.Add(new SoundSource("event:/char/oshiro/move_07_roof00_enter"));
			float from = this.Level.ZoomFocusPoint.X;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.7f)
			{
				this.Level.ZoomFocusPoint.X = from + (126f - from) * Ease.CubeInOut(p);
				yield return null;
			}
			yield return 0.3f;
			this.player.Facing = Facings.Left;
			yield return 0.1f;
			this.evil.Sprite.Scale.X = -1f;
			yield break;
		}

		// Token: 0x060013EC RID: 5100 RVA: 0x0006BD27 File Offset: 0x00069F27
		private IEnumerator OshiroGetsAngry()
		{
			yield return 0.1f;
			this.evil.Vanish();
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			this.evil = null;
			yield return 0.8f;
			Audio.Play("event:/char/oshiro/boss_transform_begin", this.oshiro.Position);
			this.oshiro.Remove(this.oshiro.Sprite);
			this.oshiro.Sprite = this.bossSprite;
			this.oshiro.Sprite.Play("transformStart", false, false);
			this.oshiro.Y += this.bossSpriteOffset;
			this.oshiro.Add(this.oshiro.Sprite);
			this.oshiro.Depth = -12500;
			this.oshiroRumble = true;
			yield return 1f;
			yield break;
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x0006BD36 File Offset: 0x00069F36
		private IEnumerator OshiroTransform()
		{
			yield return 0.2f;
			Audio.Play("event:/char/oshiro/boss_transform_burst", this.oshiro.Position);
			this.oshiro.Sprite.Play("transformFinish", false, false);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
			base.SceneAs<Level>().Shake(0.5f);
			this.SetChaseMusic();
			while (this.anxiety < 0.5f)
			{
				this.anxiety = Calc.Approach(this.anxiety, 0.5f, Engine.DeltaTime * 0.5f);
				yield return null;
			}
			yield return 0.25f;
			yield break;
		}

		// Token: 0x060013EE RID: 5102 RVA: 0x0006BD45 File Offset: 0x00069F45
		private IEnumerator AnxietyAndCameraOut()
		{
			Level level = base.Scene as Level;
			Vector2 from = level.Camera.Position;
			Vector2 to = this.player.CameraTarget;
			for (float t = 0f; t < 1f; t += Engine.DeltaTime * 2f)
			{
				this.anxiety = Calc.Approach(this.anxiety, 0f, Engine.DeltaTime * 4f);
				level.Camera.Position = from + (to - from) * Ease.CubeInOut(t);
				yield return null;
			}
			yield break;
		}

		// Token: 0x060013EF RID: 5103 RVA: 0x0006BD54 File Offset: 0x00069F54
		private void SetChaseMusic()
		{
			Level level = base.Scene as Level;
			level.Session.Audio.Music.Event = "event:/music/lvl3/oshiro_chase";
			level.Session.Audio.Apply(false);
		}

		// Token: 0x060013F0 RID: 5104 RVA: 0x0006BD8C File Offset: 0x00069F8C
		public override void OnEnd(Level level)
		{
			Distort.Anxiety = (this.anxiety = (this.anxietyFlicker = 0f));
			if (this.evil != null)
			{
				level.Remove(this.evil);
			}
			this.player = base.Scene.Tracker.GetEntity<Player>();
			if (this.player != null)
			{
				this.player.StateMachine.Locked = false;
				this.player.StateMachine.State = 0;
				this.player.X = (float)level.Bounds.Left + 170f;
				this.player.Speed.Y = 0f;
				while (this.player.CollideCheck<Solid>())
				{
					Player player = this.player;
					float y = player.Y;
					player.Y = y - 1f;
				}
				level.Camera.Position = this.player.CameraTarget;
			}
			if (this.WasSkipped)
			{
				this.SetChaseMusic();
			}
			this.oshiro.RemoveSelf();
			base.Scene.Add(new AngryOshiro(this.bossSpawnPosition, true));
			level.Session.RespawnPoint = new Vector2?(new Vector2((float)level.Bounds.Left + 170f, (float)(level.Bounds.Top + 160)));
			level.Session.SetFlag("oshiro_resort_roof", true);
		}

		// Token: 0x060013F1 RID: 5105 RVA: 0x0006BF00 File Offset: 0x0006A100
		public override void Update()
		{
			Distort.Anxiety = this.anxiety + this.anxiety * this.anxietyFlicker;
			if (base.Scene.OnInterval(0.05f))
			{
				this.anxietyFlicker = -0.2f + Calc.Random.NextFloat(0.4f);
			}
			base.Update();
			if (this.oshiroRumble)
			{
				Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
			}
		}

		// Token: 0x04000FA5 RID: 4005
		public const string Flag = "oshiro_resort_roof";

		// Token: 0x04000FA6 RID: 4006
		private const float playerEndPosition = 170f;

		// Token: 0x04000FA7 RID: 4007
		private Player player;

		// Token: 0x04000FA8 RID: 4008
		private NPC oshiro;

		// Token: 0x04000FA9 RID: 4009
		private BadelineDummy evil;

		// Token: 0x04000FAA RID: 4010
		private Vector2 bossSpawnPosition;

		// Token: 0x04000FAB RID: 4011
		private float anxiety;

		// Token: 0x04000FAC RID: 4012
		private float anxietyFlicker;

		// Token: 0x04000FAD RID: 4013
		private Sprite bossSprite = GFX.SpriteBank.Create("oshiro_boss");

		// Token: 0x04000FAE RID: 4014
		private float bossSpriteOffset;

		// Token: 0x04000FAF RID: 4015
		private bool oshiroRumble;
	}
}
