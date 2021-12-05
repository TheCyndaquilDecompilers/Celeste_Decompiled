using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000282 RID: 642
	public class CS03_Ending : CutsceneEntity
	{
		// Token: 0x060013D5 RID: 5077 RVA: 0x0006B8F5 File Offset: 0x00069AF5
		public CS03_Ending(ResortRoofEnding roof, Player player) : base(false, true)
		{
			this.roof = roof;
			this.player = player;
			base.Depth = -1000000;
		}

		// Token: 0x060013D6 RID: 5078 RVA: 0x0006B918 File Offset: 0x00069B18
		public override void OnBegin(Level level)
		{
			level.RegisterAreaComplete();
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x060013D7 RID: 5079 RVA: 0x0006B933 File Offset: 0x00069B33
		public override void Update()
		{
			base.Update();
			if (this.smashRumble)
			{
				Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
			}
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x0006B94A File Offset: 0x00069B4A
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			this.player.ForceCameraUpdate = false;
			base.Add(new Coroutine(this.player.DummyRunTo(this.roof.X + this.roof.Width - 32f, true), true));
			yield return null;
			this.player.DummyAutoAnimate = false;
			yield return 0.5f;
			this.angryOshiro = base.Scene.Entities.FindFirst<AngryOshiro>();
			base.Add(new Coroutine(this.MoveGhostTo(new Vector2(this.roof.X + 40f, this.roof.Y - 12f)), true));
			yield return 1f;
			this.player.DummyAutoAnimate = true;
			yield return level.ZoomTo(new Vector2(130f, 60f), 2f, 0.5f);
			this.player.Facing = Facings.Left;
			yield return 0.5f;
			yield return Textbox.Say("CH3_OSHIRO_CHASE_END", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.GhostSmash)
			});
			yield return this.GhostSmash(0.5f, true);
			Audio.SetMusic(null, true, true);
			this.oshiroSprite = null;
			CS03_Ending.BgFlash bgFlash = new CS03_Ending.BgFlash();
			bgFlash.Alpha = 1f;
			level.Add(bgFlash);
			Distort.GameRate = 0f;
			Sprite sprite = GFX.SpriteBank.Create("oshiro_boss_lightning");
			sprite.Position = this.angryOshiro.Position + new Vector2(140f, -100f);
			sprite.Rotation = Calc.Angle(sprite.Position, this.angryOshiro.Position + new Vector2(0f, 10f));
			sprite.Play("once", false, false);
			base.Add(sprite);
			yield return null;
			Celeste.Freeze(0.3f);
			yield return null;
			level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
			this.smashRumble = false;
			yield return 0.2f;
			Distort.GameRate = 1f;
			level.Flash(Color.White, false);
			this.player.DummyGravity = false;
			this.angryOshiro.Sprite.Play("transformBack", false, false);
			this.player.Sprite.Play("fall", false, false);
			this.roof.BeginFalling = true;
			yield return null;
			Engine.TimeRate = 0.01f;
			this.player.Sprite.Play("fallFast", false, false);
			this.player.DummyGravity = true;
			this.player.Speed.Y = -200f;
			this.player.Speed.X = 300f;
			Vector2 oshiroFallSpeed = new Vector2(-100f, -250f);
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 1.5f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				this.angryOshiro.Sprite.Rotation = t.Eased * -100f * 0.017453292f;
			};
			base.Add(tween);
			float t2;
			for (t2 = 0f; t2 < 2f; t2 += Engine.DeltaTime)
			{
				oshiroFallSpeed.X = Calc.Approach(oshiroFallSpeed.X, 0f, Engine.DeltaTime * 400f);
				oshiroFallSpeed.Y += Engine.DeltaTime * 800f;
				this.angryOshiro.Position += oshiroFallSpeed * Engine.DeltaTime;
				bgFlash.Alpha = Calc.Approach(bgFlash.Alpha, 0f, Engine.RawDeltaTime);
				Engine.TimeRate = Calc.Approach(Engine.TimeRate, 1f, Engine.RawDeltaTime * 0.6f);
				yield return null;
			}
			level.DirectionalShake(new Vector2(0f, -1f), 0.5f);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Long);
			yield return 1f;
			bgFlash = null;
			oshiroFallSpeed = default(Vector2);
			while (!this.player.OnGround(1))
			{
				this.player.MoveV(1f, null, null);
			}
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("tired", false, false);
			this.angryOshiro.RemoveSelf();
			base.Scene.Add(this.oshiro = new Entity(new Vector2((float)(level.Bounds.Left + 110), this.player.Y)));
			this.oshiro.Add(this.oshiroSprite = GFX.SpriteBank.Create("oshiro"));
			this.oshiroSprite.Play("fall", false, false);
			this.oshiroSprite.Scale.X = 1f;
			this.oshiro.Collider = new Hitbox(8f, 8f, -4f, -8f);
			this.oshiro.Add(new VertexLight(new Vector2(0f, -8f), Color.White, 1f, 16, 32));
			yield return CutsceneEntity.CameraTo(this.player.CameraTarget + new Vector2(0f, 40f), 1f, Ease.CubeOut, 0f);
			yield return 1.5f;
			Audio.SetMusic("event:/music/lvl3/intro", true, true);
			yield return 3f;
			Audio.Play("event:/char/oshiro/chat_get_up", this.oshiro.Position);
			this.oshiroSprite.Play("recover", false, false);
			float target = this.oshiro.Y + 4f;
			while (this.oshiro.Y != target)
			{
				this.oshiro.Y = Calc.Approach(this.oshiro.Y, target, 6f * Engine.DeltaTime);
				yield return null;
			}
			yield return 0.6f;
			yield return Textbox.Say("CH3_ENDING", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.OshiroTurns)
			});
			base.Add(new Coroutine(CutsceneEntity.CameraTo(level.Camera.Position + new Vector2(-80f, 0f), 3f, null, 0f), true));
			yield return 0.5f;
			this.oshiroSprite.Scale.X = -1f;
			yield return 0.2f;
			t2 = 0f;
			this.oshiro.Add(new SoundSource("event:/char/oshiro/move_08_roof07_exit"));
			while (this.oshiro.X > (float)(level.Bounds.Left - 16))
			{
				this.oshiro.X -= 40f * Engine.DeltaTime;
				this.oshiroSprite.Y = (float)Math.Sin((double)(t2 += Engine.DeltaTime * 2f)) * 2f;
				Door door = this.oshiro.CollideFirst<Door>();
				if (door != null)
				{
					door.Open(this.oshiro.X);
				}
				yield return null;
			}
			base.Add(new Coroutine(CutsceneEntity.CameraTo(level.Camera.Position + new Vector2(80f, 0f), 2f, null, 0f), true));
			yield return 1.2f;
			this.player.DummyAutoAnimate = true;
			yield return this.player.DummyWalkTo(this.player.X - 16f, false, 1f, false);
			yield return 2f;
			this.player.Facing = Facings.Right;
			yield return 1f;
			this.player.ForceCameraUpdate = false;
			this.player.Add(new Coroutine(this.RunPlayerRight(), true));
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x0006B960 File Offset: 0x00069B60
		private IEnumerator OshiroTurns()
		{
			yield return 1f;
			this.oshiroSprite.Scale.X = -1f;
			yield return 0.2f;
			yield break;
		}

		// Token: 0x060013DA RID: 5082 RVA: 0x0006B96F File Offset: 0x00069B6F
		private IEnumerator MoveGhostTo(Vector2 target)
		{
			if (this.angryOshiro == null)
			{
				yield break;
			}
			target.Y -= this.angryOshiro.Height / 2f;
			this.angryOshiro.EnterDummyMode();
			this.angryOshiro.Collidable = false;
			while (this.angryOshiro.Position != target)
			{
				this.angryOshiro.Position = Calc.Approach(this.angryOshiro.Position, target, 64f * Engine.DeltaTime);
				yield return null;
			}
			yield break;
		}

		// Token: 0x060013DB RID: 5083 RVA: 0x0006B985 File Offset: 0x00069B85
		private IEnumerator GhostSmash()
		{
			yield return this.GhostSmash(0f, false);
			yield break;
		}

		// Token: 0x060013DC RID: 5084 RVA: 0x0006B994 File Offset: 0x00069B94
		private IEnumerator GhostSmash(float topDelay, bool final)
		{
			if (this.angryOshiro == null)
			{
				yield break;
			}
			if (final)
			{
				this.smashSfx = Audio.Play("event:/char/oshiro/boss_slam_final", this.angryOshiro.Position);
			}
			else
			{
				this.smashSfx = Audio.Play("event:/char/oshiro/boss_slam_first", this.angryOshiro.Position);
			}
			float from = this.angryOshiro.Y;
			float to = this.angryOshiro.Y - 32f;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 2f)
			{
				this.angryOshiro.Y = MathHelper.Lerp(from, to, Ease.CubeOut(p));
				yield return null;
			}
			yield return topDelay;
			float ground = from + 20f;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 8f)
			{
				this.angryOshiro.Y = MathHelper.Lerp(to, ground, Ease.CubeOut(p));
				yield return null;
			}
			this.angryOshiro.Squish();
			this.Level.Shake(0.5f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			this.smashRumble = true;
			this.roof.StartShaking(0.5f);
			if (!final)
			{
				for (float p = 0f; p < 1f; p += Engine.DeltaTime * 16f)
				{
					this.angryOshiro.Y = MathHelper.Lerp(ground, from, Ease.CubeOut(p));
					yield return null;
				}
			}
			else
			{
				this.angryOshiro.Y = (ground + from) / 2f;
			}
			if (this.angryOshiro == null)
			{
				yield break;
			}
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("shaking", false, false);
			this.roof.Wobble(this.angryOshiro, final);
			if (!final)
			{
				yield return 0.5f;
			}
			yield break;
		}

		// Token: 0x060013DD RID: 5085 RVA: 0x0006B9B1 File Offset: 0x00069BB1
		private IEnumerator RunPlayerRight()
		{
			yield return 0.75f;
			yield return this.player.DummyRunTo(this.player.X + 128f, false);
			yield break;
		}

		// Token: 0x060013DE RID: 5086 RVA: 0x0006B9C0 File Offset: 0x00069BC0
		public override void OnEnd(Level level)
		{
			Audio.SetMusic("event:/music/lvl3/intro", true, true);
			Audio.Stop(this.smashSfx, true);
			level.CompleteArea(true, false, false);
			SpotlightWipe.FocusPoint = new Vector2(192f, 120f);
		}

		// Token: 0x04000F99 RID: 3993
		public const string Flag = "oshiroEnding";

		// Token: 0x04000F9A RID: 3994
		private ResortRoofEnding roof;

		// Token: 0x04000F9B RID: 3995
		private AngryOshiro angryOshiro;

		// Token: 0x04000F9C RID: 3996
		private Player player;

		// Token: 0x04000F9D RID: 3997
		private Entity oshiro;

		// Token: 0x04000F9E RID: 3998
		private Sprite oshiroSprite;

		// Token: 0x04000F9F RID: 3999
		private EventInstance smashSfx;

		// Token: 0x04000FA0 RID: 4000
		private bool smashRumble;

		// Token: 0x020005D6 RID: 1494
		private class BgFlash : Entity
		{
			// Token: 0x060028D1 RID: 10449 RVA: 0x00108D7F File Offset: 0x00106F7F
			public BgFlash()
			{
				base.Depth = 10100;
			}

			// Token: 0x060028D2 RID: 10450 RVA: 0x00108D94 File Offset: 0x00106F94
			public override void Render()
			{
				Camera camera = (base.Scene as Level).Camera;
				Draw.Rect(camera.X - 10f, camera.Y - 10f, 340f, 200f, Color.Black * this.Alpha);
			}

			// Token: 0x04002835 RID: 10293
			public float Alpha;
		}
	}
}
