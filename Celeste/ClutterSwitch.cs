using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002B5 RID: 693
	public class ClutterSwitch : Solid
	{
		// Token: 0x06001564 RID: 5476 RVA: 0x0007AFA0 File Offset: 0x000791A0
		public ClutterSwitch(Vector2 position, ClutterBlock.Colors color) : base(position, 32f, 16f, true)
		{
			this.color = color;
			this.startY = (this.atY = base.Y);
			this.OnDashCollide = new DashCollision(this.OnDashed);
			this.SurfaceSoundIndex = 21;
			base.Add(this.sprite = GFX.SpriteBank.Create("clutterSwitch"));
			this.sprite.Position = new Vector2(16f, 16f);
			this.sprite.Play("idle", false, false);
			base.Add(this.icon = new Image(GFX.Game["objects/resortclutter/icon_" + color.ToString()]));
			this.icon.CenterOrigin();
			this.icon.Position = new Vector2(16f, 8f);
			base.Add(this.vertexLight = new VertexLight(new Vector2(base.CenterX - base.X, -1f), Color.Aqua, 1f, 32, 64));
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x0007B0DE File Offset: 0x000792DE
		public ClutterSwitch(EntityData data, Vector2 offset) : this(data.Position + offset, data.Enum<ClutterBlock.Colors>("type", ClutterBlock.Colors.Green))
		{
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x0007B100 File Offset: 0x00079300
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (this.color == ClutterBlock.Colors.Lightning && base.SceneAs<Level>().Session.GetFlag("disable_lightning"))
			{
				this.BePressed();
				return;
			}
			if (base.SceneAs<Level>().Session.GetFlag("oshiro_clutter_cleared_" + (int)this.color))
			{
				this.BePressed();
			}
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x0007B168 File Offset: 0x00079368
		private void BePressed()
		{
			this.pressed = true;
			this.atY += 10f;
			base.Y += 10f;
			this.sprite.Y += 2f;
			this.sprite.Play("active", false, false);
			base.Remove(this.icon);
			this.vertexLight.StartRadius = 24f;
			this.vertexLight.EndRadius = 48f;
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x0007B1F8 File Offset: 0x000793F8
		public override void Update()
		{
			base.Update();
			if (base.HasPlayerOnTop())
			{
				if (this.speedY < 0f)
				{
					this.speedY = 0f;
				}
				this.speedY = Calc.Approach(this.speedY, 70f, 200f * Engine.DeltaTime);
				base.MoveTowardsY(this.atY + (float)(this.pressed ? 2 : 4), this.speedY * Engine.DeltaTime);
				this.targetXScale = 1.2f;
				if (!this.playerWasOnTop)
				{
					Audio.Play("event:/game/03_resort/clutterswitch_squish", this.Position);
				}
				this.playerWasOnTop = true;
			}
			else
			{
				if (this.speedY > 0f)
				{
					this.speedY = 0f;
				}
				this.speedY = Calc.Approach(this.speedY, -150f, 200f * Engine.DeltaTime);
				base.MoveTowardsY(this.atY, -this.speedY * Engine.DeltaTime);
				this.targetXScale = 1f;
				if (this.playerWasOnTop)
				{
					Audio.Play("event:/game/03_resort/clutterswitch_return", this.Position);
				}
				this.playerWasOnTop = false;
			}
			this.sprite.Scale.X = Calc.Approach(this.sprite.Scale.X, this.targetXScale, 0.8f * Engine.DeltaTime);
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x0007B358 File Offset: 0x00079558
		private DashCollisionResults OnDashed(Player player, Vector2 direction)
		{
			if (!this.pressed && direction == Vector2.UnitY)
			{
				Celeste.Freeze(0.2f);
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
				Level level = base.Scene as Level;
				level.Session.SetFlag("oshiro_clutter_cleared_" + (int)this.color, true);
				level.Session.SetFlag("oshiro_clutter_door_open", false);
				this.vertexLight.StartRadius = 64f;
				this.vertexLight.EndRadius = 128f;
				level.DirectionalShake(Vector2.UnitY, 0.6f);
				level.Particles.Emit(ClutterSwitch.P_Pressed, 20, base.TopCenter - Vector2.UnitY * 10f, new Vector2(16f, 8f));
				this.BePressed();
				this.sprite.Scale.X = 1.5f;
				if (this.color == ClutterBlock.Colors.Lightning)
				{
					base.Add(new Coroutine(this.LightningRoutine(player), true));
				}
				else
				{
					base.Add(new Coroutine(this.AbsorbRoutine(player), true));
				}
			}
			return DashCollisionResults.NormalCollision;
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x0007B485 File Offset: 0x00079685
		private IEnumerator LightningRoutine(Player player)
		{
			Level level = base.SceneAs<Level>();
			level.Session.SetFlag("disable_lightning", true);
			AudioTrackState music = level.Session.Audio.Music;
			int progress = music.Progress;
			music.Progress = progress + 1;
			level.Session.Audio.Apply(false);
			yield return Lightning.RemoveRoutine(level, null);
			yield break;
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x0007B494 File Offset: 0x00079694
		private IEnumerator AbsorbRoutine(Player player)
		{
			ClutterSwitch.<>c__DisplayClass26_0 CS$<>8__locals1 = new ClutterSwitch.<>c__DisplayClass26_0();
			CS$<>8__locals1.<>4__this = this;
			base.Add(this.cutsceneSfx = new SoundSource());
			float duration = 0f;
			if (this.color == ClutterBlock.Colors.Green)
			{
				this.cutsceneSfx.Play("event:/game/03_resort/clutterswitch_books", null, 0f);
				duration = 6.366f;
			}
			else if (this.color == ClutterBlock.Colors.Red)
			{
				this.cutsceneSfx.Play("event:/game/03_resort/clutterswitch_linens", null, 0f);
				duration = 6.15f;
			}
			else if (this.color == ClutterBlock.Colors.Yellow)
			{
				this.cutsceneSfx.Play("event:/game/03_resort/clutterswitch_boxes", null, 0f);
				duration = 6.066f;
			}
			base.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate
			{
				Audio.Play("event:/game/03_resort/clutterswitch_finish", CS$<>8__locals1.<>4__this.Position);
			}, duration, true));
			player.StateMachine.State = 11;
			Vector2 target = this.Position + new Vector2(base.Width / 2f, 0f);
			ClutterAbsorbEffect effect = new ClutterAbsorbEffect();
			base.Scene.Add(effect);
			this.sprite.Play("break", false, false);
			CS$<>8__locals1.level = base.SceneAs<Level>();
			AudioTrackState music = CS$<>8__locals1.level.Session.Audio.Music;
			int num = music.Progress;
			music.Progress = num + 1;
			CS$<>8__locals1.level.Session.Audio.Apply(false);
			CS$<>8__locals1.level.Session.LightingAlphaAdd -= 0.05f;
			float start = CS$<>8__locals1.level.Lighting.Alpha;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 2f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				CS$<>8__locals1.level.Lighting.Alpha = MathHelper.Lerp(start, 0.05f, t.Eased);
			};
			base.Add(tween);
			Alarm.Set(this, 3f, delegate
			{
				float start = CS$<>8__locals1.<>4__this.vertexLight.StartRadius;
				float end = CS$<>8__locals1.<>4__this.vertexLight.EndRadius;
				Tween tween2 = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 2f, true);
				tween2.OnUpdate = delegate(Tween t)
				{
					CS$<>8__locals1.level.Lighting.Alpha = MathHelper.Lerp(0.05f, CS$<>8__locals1.level.BaseLightingAlpha + CS$<>8__locals1.level.Session.LightingAlphaAdd, t.Eased);
					CS$<>8__locals1.<>4__this.vertexLight.StartRadius = (float)((int)Math.Round((double)MathHelper.Lerp(start, 24f, t.Eased)));
					CS$<>8__locals1.<>4__this.vertexLight.EndRadius = (float)((int)Math.Round((double)MathHelper.Lerp(end, 48f, t.Eased)));
				};
				CS$<>8__locals1.<>4__this.Add(tween2);
			}, Alarm.AlarmMode.Oneshot);
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			foreach (ClutterBlock clutterBlock in base.Scene.Entities.FindAll<ClutterBlock>())
			{
				if (clutterBlock.BlockColor == this.color)
				{
					clutterBlock.Absorb(effect);
				}
			}
			foreach (ClutterBlockBase clutterBlockBase in base.Scene.Entities.FindAll<ClutterBlockBase>())
			{
				if (clutterBlockBase.BlockColor == this.color)
				{
					clutterBlockBase.Deactivate();
				}
			}
			yield return 1.5f;
			player.StateMachine.State = 0;
			List<MTexture> images = GFX.Game.GetAtlasSubtextures("objects/resortclutter/" + this.color.ToString() + "_");
			for (int i = 0; i < 25; i = num + 1)
			{
				for (int j = 0; j < 5; j++)
				{
					Vector2 position = target + Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 320f);
					effect.FlyClutter(position, Calc.Random.Choose(images), false, 0f);
				}
				CS$<>8__locals1.level.Shake(0.3f);
				Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
				yield return 0.05f;
				num = i;
			}
			yield return 1.5f;
			effect.CloseCabinets();
			yield return 0.2f;
			Input.Rumble(RumbleStrength.Medium, RumbleLength.FullSecond);
			yield return 0.3f;
			yield break;
		}

		// Token: 0x0600156C RID: 5484 RVA: 0x0007B4AC File Offset: 0x000796AC
		public override void Removed(Scene scene)
		{
			Level level = scene as Level;
			level.Lighting.Alpha = level.BaseLightingAlpha + level.Session.LightingAlphaAdd;
			base.Removed(scene);
		}

		// Token: 0x04001174 RID: 4468
		public const float LightingAlphaAdd = 0.05f;

		// Token: 0x04001175 RID: 4469
		public static ParticleType P_Pressed;

		// Token: 0x04001176 RID: 4470
		public static ParticleType P_ClutterFly;

		// Token: 0x04001177 RID: 4471
		private const int PressedAdd = 10;

		// Token: 0x04001178 RID: 4472
		private const int PressedSpriteAdd = 2;

		// Token: 0x04001179 RID: 4473
		private const int UnpressedLightRadius = 32;

		// Token: 0x0400117A RID: 4474
		private const int PressedLightRadius = 24;

		// Token: 0x0400117B RID: 4475
		private const int BrightLightRadius = 64;

		// Token: 0x0400117C RID: 4476
		private ClutterBlock.Colors color;

		// Token: 0x0400117D RID: 4477
		private float startY;

		// Token: 0x0400117E RID: 4478
		private float atY;

		// Token: 0x0400117F RID: 4479
		private float speedY;

		// Token: 0x04001180 RID: 4480
		private bool pressed;

		// Token: 0x04001181 RID: 4481
		private Sprite sprite;

		// Token: 0x04001182 RID: 4482
		private Image icon;

		// Token: 0x04001183 RID: 4483
		private float targetXScale = 1f;

		// Token: 0x04001184 RID: 4484
		private VertexLight vertexLight;

		// Token: 0x04001185 RID: 4485
		private bool playerWasOnTop;

		// Token: 0x04001186 RID: 4486
		private SoundSource cutsceneSfx;
	}
}
