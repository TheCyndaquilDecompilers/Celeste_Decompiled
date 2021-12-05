using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000204 RID: 516
	public class TempleBigEyeball : Entity
	{
		// Token: 0x060010DC RID: 4316 RVA: 0x0004F7A0 File Offset: 0x0004D9A0
		public TempleBigEyeball(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Add(this.sprite = GFX.SpriteBank.Create("temple_eyeball"));
			base.Add(this.pupil = new Image(GFX.Game["danger/templeeye/pupil"]));
			this.pupil.CenterOrigin();
			base.Collider = new Hitbox(48f, 64f, -24f, -32f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(new HoldableCollider(new Action<Holdable>(this.OnHoldable), null));
			base.Add(this.bounceWiggler = Wiggler.Create(0.5f, 3f, null, false, false));
			base.Add(this.pupilWiggler = Wiggler.Create(0.5f, 3f, null, false, false));
			this.shockwaveTimer = 2f;
		}

		// Token: 0x060010DD RID: 4317 RVA: 0x0004F8B4 File Offset: 0x0004DAB4
		private void OnPlayer(Player player)
		{
			if (!this.triggered)
			{
				Audio.Play("event:/game/05_mirror_temple/eyewall_bounce", player.Position);
				player.ExplodeLaunch(player.Center + Vector2.UnitX * 20f, true, false);
				player.Swat(-1);
				this.bounceWiggler.Start();
			}
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x0004F910 File Offset: 0x0004DB10
		private void OnHoldable(Holdable h)
		{
			if (h.Entity is TheoCrystal)
			{
				TheoCrystal theoCrystal = h.Entity as TheoCrystal;
				if (!this.triggered && theoCrystal.Speed.X > 32f && !theoCrystal.Hold.IsHeld)
				{
					theoCrystal.Speed.X = -50f;
					theoCrystal.Speed.Y = -10f;
					this.triggered = true;
					this.bounceWiggler.Start();
					this.Collidable = false;
					Audio.SetAmbience(null, true);
					Audio.Play("event:/game/05_mirror_temple/eyewall_destroy", this.Position);
					Alarm.Set(this, 1.3f, delegate
					{
						Audio.SetMusic(null, true, true);
					}, Alarm.AlarmMode.Oneshot);
					base.Add(new Coroutine(this.Burst(), true));
				}
			}
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x0004F9F9 File Offset: 0x0004DBF9
		private IEnumerator Burst()
		{
			this.bursting = true;
			Level level = base.Scene as Level;
			level.StartCutscene(new Action<Level>(this.OnSkip), false, true, true);
			level.RegisterAreaComplete();
			Celeste.Freeze(0.1f);
			yield return null;
			float start = Glitch.Value;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, null, 0.5f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				Glitch.Value = MathHelper.Lerp(start, 0f, t.Eased);
			};
			base.Add(tween);
			Player player = base.Scene.Tracker.GetEntity<Player>();
			TheoCrystal entity = base.Scene.Tracker.GetEntity<TheoCrystal>();
			if (player != null)
			{
				player.StateMachine.State = 11;
				player.StateMachine.Locked = true;
				if (player.OnGround(1))
				{
					player.DummyAutoAnimate = false;
					player.Sprite.Play("shaking", false, false);
				}
			}
			base.Add(new Coroutine(level.ZoomTo(entity.TopCenter - level.Camera.Position, 2f, 0.5f), true));
			base.Add(new Coroutine(entity.Shatter(), true));
			foreach (TempleEye templeEye in base.Scene.Entities.FindAll<TempleEye>())
			{
				templeEye.Burst();
			}
			this.sprite.Play("burst", false, false);
			this.pupil.Visible = false;
			level.Shake(0.4f);
			yield return 2f;
			if (player != null && player.OnGround(1))
			{
				player.DummyAutoAnimate = false;
				player.Sprite.Play("shaking", false, false);
			}
			this.Visible = false;
			TempleBigEyeball.Fader fade = new TempleBigEyeball.Fader();
			level.Add(fade);
			while ((fade.Fade += Engine.DeltaTime) < 1f)
			{
				yield return null;
			}
			yield return 1f;
			fade = null;
			level.EndCutscene();
			level.CompleteArea(false, false, false);
			yield break;
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x0004FA08 File Offset: 0x0004DC08
		private void OnSkip(Level level)
		{
			level.CompleteArea(false, false, false);
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x0004FA14 File Offset: 0x0004DC14
		public override void Update()
		{
			base.Update();
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				Audio.SetMusicParam("eye_distance", Calc.ClampedMap(entity.X, (float)(base.Scene as Level).Bounds.Left, base.X, 0f, 1f));
			}
			if (entity != null && !this.bursting)
			{
				Glitch.Value = Calc.ClampedMap(Math.Abs(base.X - entity.X), 100f, 900f, 0.2f, 0f);
			}
			if (!this.triggered && this.shockwaveTimer > 0f)
			{
				this.shockwaveTimer -= Engine.DeltaTime;
				if (this.shockwaveTimer <= 0f)
				{
					if (entity != null)
					{
						this.shockwaveTimer = Calc.ClampedMap(Math.Abs(base.X - entity.X), 100f, 500f, 2f, 3f);
						this.shockwaveFlag = !this.shockwaveFlag;
						if (this.shockwaveFlag)
						{
							this.shockwaveTimer -= 1f;
						}
					}
					base.Scene.Add(Engine.Pooler.Create<TempleBigEyeballShockwave>().Init(base.Center + new Vector2(50f, 0f)));
					this.pupilWiggler.Start();
					this.pupilTarget = new Vector2(-1f, 0f);
					this.pupilSpeed = 120f;
					this.pupilDelay = Math.Max(0.5f, this.pupilDelay);
				}
			}
			this.pupil.Position = Calc.Approach(this.pupil.Position, this.pupilTarget * 12f, Engine.DeltaTime * this.pupilSpeed);
			this.pupilSpeed = Calc.Approach(this.pupilSpeed, 40f, Engine.DeltaTime * 400f);
			TheoCrystal entity2 = base.Scene.Tracker.GetEntity<TheoCrystal>();
			if (entity2 != null && Math.Abs(base.X - entity2.X) < 64f && Math.Abs(base.Y - entity2.Y) < 64f)
			{
				this.pupilTarget = (entity2.Center - this.Position).SafeNormalize();
			}
			else if (this.pupilDelay < 0f)
			{
				this.pupilTarget = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f);
				this.pupilDelay = Calc.Random.Choose(0.2f, 1f, 2f);
			}
			else
			{
				this.pupilDelay -= Engine.DeltaTime;
			}
			if (entity != null)
			{
				Level level = base.Scene as Level;
				Audio.SetMusicParam("eye_distance", Calc.ClampedMap(entity.X, (float)(level.Bounds.Left + 32), base.X - 32f, 1f, 0f));
			}
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x0004FD2C File Offset: 0x0004DF2C
		public override void Render()
		{
			this.sprite.Scale.X = 1f + 0.15f * this.bounceWiggler.Value;
			this.pupil.Scale = Vector2.One * (1f + this.pupilWiggler.Value * 0.15f);
			base.Render();
		}

		// Token: 0x04000C7C RID: 3196
		private Sprite sprite;

		// Token: 0x04000C7D RID: 3197
		private Image pupil;

		// Token: 0x04000C7E RID: 3198
		private bool triggered;

		// Token: 0x04000C7F RID: 3199
		private Vector2 pupilTarget;

		// Token: 0x04000C80 RID: 3200
		private float pupilDelay;

		// Token: 0x04000C81 RID: 3201
		private Wiggler bounceWiggler;

		// Token: 0x04000C82 RID: 3202
		private Wiggler pupilWiggler;

		// Token: 0x04000C83 RID: 3203
		private float shockwaveTimer;

		// Token: 0x04000C84 RID: 3204
		private bool shockwaveFlag;

		// Token: 0x04000C85 RID: 3205
		private float pupilSpeed = 40f;

		// Token: 0x04000C86 RID: 3206
		private bool bursting;

		// Token: 0x0200050A RID: 1290
		private class Fader : Entity
		{
			// Token: 0x06002501 RID: 9473 RVA: 0x000471C3 File Offset: 0x000453C3
			public Fader()
			{
				base.Tag = Tags.HUD;
			}

			// Token: 0x06002502 RID: 9474 RVA: 0x000F6A9B File Offset: 0x000F4C9B
			public override void Render()
			{
				Draw.Rect(-10f, -10f, (float)(Engine.Width + 20), (float)(Engine.Height + 20), Color.White * this.Fade);
			}

			// Token: 0x040024C3 RID: 9411
			public float Fade;
		}
	}
}
