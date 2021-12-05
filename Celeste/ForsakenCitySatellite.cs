using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000229 RID: 553
	public class ForsakenCitySatellite : Entity
	{
		// Token: 0x060011AE RID: 4526 RVA: 0x00057D64 File Offset: 0x00055F64
		public ForsakenCitySatellite(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Add(this.sprite = new Image(GFX.Game["objects/citysatellite/dish"]));
			base.Add(this.pulse = new Image(GFX.Game["objects/citysatellite/light"]));
			base.Add(this.computer = new Image(GFX.Game["objects/citysatellite/computer"]));
			base.Add(this.computerScreen = new Image(GFX.Game["objects/citysatellite/computerscreen"]));
			base.Add(this.computerScreenNoise = new Sprite(GFX.Game, "objects/citysatellite/computerScreenNoise"));
			base.Add(this.computerScreenShine = new Image(GFX.Game["objects/citysatellite/computerscreenShine"]));
			this.sprite.JustifyOrigin(0.5f, 1f);
			this.pulse.JustifyOrigin(0.5f, 1f);
			base.Add(new Coroutine(this.PulseRoutine(), true));
			base.Add(this.pulseBloom = new BloomPoint(new Vector2(-12f, -44f), 1f, 8f));
			base.Add(this.screenBloom = new BloomPoint(new Vector2(32f, 20f), 1f, 8f));
			this.computerScreenNoise.AddLoop("static", "", 0.05f);
			this.computerScreenNoise.Play("static", false, false);
			this.computer.Position = (this.computerScreen.Position = (this.computerScreenShine.Position = (this.computerScreenNoise.Position = new Vector2(8f, 8f))));
			this.birdFlyPosition = offset + data.Nodes[0];
			this.gemSpawnPosition = offset + data.Nodes[1];
			base.Add(this.dashListener = new DashListener());
			this.dashListener.OnDash = delegate(Vector2 dir)
			{
				string text = "";
				if (dir.Y < 0f)
				{
					text = "U";
				}
				else if (dir.Y > 0f)
				{
					text = "D";
				}
				if (dir.X < 0f)
				{
					text += "L";
				}
				else if (dir.X > 0f)
				{
					text += "R";
				}
				this.currentInputs.Add(text);
				if (this.currentInputs.Count > ForsakenCitySatellite.Code.Length)
				{
					this.currentInputs.RemoveAt(0);
				}
				if (this.currentInputs.Count == ForsakenCitySatellite.Code.Length)
				{
					bool flag = true;
					for (int j = 0; j < ForsakenCitySatellite.Code.Length; j++)
					{
						if (!this.currentInputs[j].Equals(ForsakenCitySatellite.Code[j]))
						{
							flag = false;
						}
					}
					if (flag && this.level.Camera.Left + 32f < this.gemSpawnPosition.X && this.enabled)
					{
						base.Add(new Coroutine(this.UnlockGem(), true));
					}
				}
			};
			foreach (string item in ForsakenCitySatellite.Code)
			{
				if (!ForsakenCitySatellite.uniqueCodes.Contains(item))
				{
					ForsakenCitySatellite.uniqueCodes.Add(item);
				}
			}
			base.Depth = 8999;
			base.Add(this.staticLoopSfx = new SoundSource());
			this.staticLoopSfx.Position = this.computer.Position;
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x00058040 File Offset: 0x00056240
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = (scene as Level);
			this.enabled = (!this.level.Session.HeartGem && !this.level.Session.GetFlag("unlocked_satellite"));
			if (this.enabled)
			{
				foreach (string code in ForsakenCitySatellite.uniqueCodes)
				{
					ForsakenCitySatellite.CodeBird codeBird = new ForsakenCitySatellite.CodeBird(this.birdFlyPosition, code);
					this.birds.Add(codeBird);
					this.level.Add(codeBird);
				}
				base.Add(this.birdFlyingSfx = new SoundSource());
				base.Add(this.birdFinishSfx = new SoundSource());
				base.Add(this.birdThrustSfx = new SoundSource());
				this.birdFlyingSfx.Position = this.birdFlyPosition - this.Position;
				this.birdFlyingSfx.Play("event:/game/01_forsaken_city/birdbros_fly_loop", null, 0f);
			}
			else
			{
				this.staticLoopSfx.Play("event:/game/01_forsaken_city/console_static_loop", null, 0f);
			}
			if (!this.level.Session.HeartGem && this.level.Session.GetFlag("unlocked_satellite"))
			{
				HeartGem entity = new HeartGem(this.gemSpawnPosition);
				this.level.Add(entity);
			}
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x000581CC File Offset: 0x000563CC
		public override void Update()
		{
			base.Update();
			this.computerScreenNoise.Visible = !this.pulse.Visible;
			this.computerScreen.Visible = this.pulse.Visible;
			this.screenBloom.Visible = this.pulseBloom.Visible;
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00058224 File Offset: 0x00056424
		private IEnumerator PulseRoutine()
		{
			this.pulseBloom.Visible = (this.pulse.Visible = false);
			while (this.enabled)
			{
				yield return 2f;
				int i = 0;
				while (i < ForsakenCitySatellite.Code.Length && this.enabled)
				{
					this.pulse.Color = (this.computerScreen.Color = ForsakenCitySatellite.Colors[ForsakenCitySatellite.Code[i]]);
					this.pulseBloom.Visible = (this.pulse.Visible = true);
					Audio.Play(ForsakenCitySatellite.Sounds[ForsakenCitySatellite.Code[i]], this.Position + this.computer.Position);
					yield return 0.5f;
					this.pulseBloom.Visible = (this.pulse.Visible = false);
					Audio.Play((i < ForsakenCitySatellite.Code.Length - 1) ? "event:/game/01_forsaken_city/console_static_short" : "event:/game/01_forsaken_city/console_static_long", this.Position + this.computer.Position);
					yield return 0.2f;
					int num = i;
					i = num + 1;
				}
				base.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate
				{
					if (this.enabled)
					{
						this.birdThrustSfx.Position = this.birdFlyPosition - this.Position;
						this.birdThrustSfx.Play("event:/game/01_forsaken_city/birdbros_thrust", null, 0f);
					}
				}, 1.1f, true));
				this.birds.Shuffle<ForsakenCitySatellite.CodeBird>();
				foreach (ForsakenCitySatellite.CodeBird codeBird in this.birds)
				{
					if (this.enabled)
					{
						codeBird.Dash();
						yield return 0.02f;
					}
				}
				List<ForsakenCitySatellite.CodeBird>.Enumerator enumerator = default(List<ForsakenCitySatellite.CodeBird>.Enumerator);
			}
			this.pulseBloom.Visible = (this.pulse.Visible = false);
			yield break;
			yield break;
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x00058233 File Offset: 0x00056433
		private IEnumerator UnlockGem()
		{
			this.level.Session.SetFlag("unlocked_satellite", true);
			this.birdFinishSfx.Position = this.birdFlyPosition - this.Position;
			this.birdFinishSfx.Play("event:/game/01_forsaken_city/birdbros_finish", null, 0f);
			this.staticLoopSfx.Play("event:/game/01_forsaken_city/console_static_loop", null, 0f);
			this.enabled = false;
			yield return 0.25f;
			this.level.Displacement.Clear();
			yield return null;
			this.birdFlyingSfx.Stop(true);
			this.level.Frozen = true;
			base.Tag = Tags.FrozenUpdate;
			BloomPoint bloom = new BloomPoint(this.birdFlyPosition - this.Position, 0f, 32f);
			base.Add(bloom);
			using (List<ForsakenCitySatellite.CodeBird>.Enumerator enumerator = this.birds.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ForsakenCitySatellite.CodeBird codeBird = enumerator.Current;
					codeBird.Transform(3f);
				}
				goto IL_1B6;
			}
			IL_182:
			bloom.Alpha += Engine.DeltaTime / 3f;
			yield return null;
			IL_1B6:
			if (bloom.Alpha >= 1f)
			{
				yield return 0.25f;
				foreach (ForsakenCitySatellite.CodeBird codeBird2 in this.birds)
				{
					codeBird2.RemoveSelf();
				}
				ParticleSystem particles = new ParticleSystem(-10000, 100);
				particles.Tag = Tags.FrozenUpdate;
				particles.Emit(BirdNPC.P_Feather, 24, this.birdFlyPosition, new Vector2(4f, 4f));
				this.level.Add(particles);
				HeartGem gem = new HeartGem(this.birdFlyPosition);
				gem.Tag = Tags.FrozenUpdate;
				this.level.Add(gem);
				yield return null;
				gem.ScaleWiggler.Start();
				yield return 0.85f;
				SimpleCurve curve = new SimpleCurve(gem.Position, this.gemSpawnPosition, (gem.Position + this.gemSpawnPosition) / 2f + new Vector2(0f, -64f));
				for (float t = 0f; t < 1f; t += Engine.DeltaTime)
				{
					yield return null;
					gem.Position = curve.GetPoint(Ease.CubeInOut(t));
				}
				yield return 0.5f;
				particles.RemoveSelf();
				base.Remove(bloom);
				this.level.Frozen = false;
				yield break;
			}
			goto IL_182;
		}

		// Token: 0x04000D44 RID: 3396
		private const string UnlockedFlag = "unlocked_satellite";

		// Token: 0x04000D45 RID: 3397
		public static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>
		{
			{
				"U",
				Calc.HexToColor("f0f0f0")
			},
			{
				"L",
				Calc.HexToColor("9171f2")
			},
			{
				"DR",
				Calc.HexToColor("0a44e0")
			},
			{
				"UR",
				Calc.HexToColor("b32d00")
			},
			{
				"UL",
				Calc.HexToColor("ffcd37")
			}
		};

		// Token: 0x04000D46 RID: 3398
		public static readonly Dictionary<string, string> Sounds = new Dictionary<string, string>
		{
			{
				"U",
				"event:/game/01_forsaken_city/console_white"
			},
			{
				"L",
				"event:/game/01_forsaken_city/console_purple"
			},
			{
				"DR",
				"event:/game/01_forsaken_city/console_blue"
			},
			{
				"UR",
				"event:/game/01_forsaken_city/console_red"
			},
			{
				"UL",
				"event:/game/01_forsaken_city/console_yellow"
			}
		};

		// Token: 0x04000D47 RID: 3399
		public static readonly Dictionary<string, ParticleType> Particles = new Dictionary<string, ParticleType>();

		// Token: 0x04000D48 RID: 3400
		private static readonly string[] Code = new string[]
		{
			"U",
			"L",
			"DR",
			"UR",
			"L",
			"UL"
		};

		// Token: 0x04000D49 RID: 3401
		private static List<string> uniqueCodes = new List<string>();

		// Token: 0x04000D4A RID: 3402
		private bool enabled;

		// Token: 0x04000D4B RID: 3403
		private List<string> currentInputs = new List<string>();

		// Token: 0x04000D4C RID: 3404
		private List<ForsakenCitySatellite.CodeBird> birds = new List<ForsakenCitySatellite.CodeBird>();

		// Token: 0x04000D4D RID: 3405
		private Vector2 gemSpawnPosition;

		// Token: 0x04000D4E RID: 3406
		private Vector2 birdFlyPosition;

		// Token: 0x04000D4F RID: 3407
		private Image sprite;

		// Token: 0x04000D50 RID: 3408
		private Image pulse;

		// Token: 0x04000D51 RID: 3409
		private Image computer;

		// Token: 0x04000D52 RID: 3410
		private Image computerScreen;

		// Token: 0x04000D53 RID: 3411
		private Sprite computerScreenNoise;

		// Token: 0x04000D54 RID: 3412
		private Image computerScreenShine;

		// Token: 0x04000D55 RID: 3413
		private BloomPoint pulseBloom;

		// Token: 0x04000D56 RID: 3414
		private BloomPoint screenBloom;

		// Token: 0x04000D57 RID: 3415
		private Level level;

		// Token: 0x04000D58 RID: 3416
		private DashListener dashListener;

		// Token: 0x04000D59 RID: 3417
		private SoundSource birdFlyingSfx;

		// Token: 0x04000D5A RID: 3418
		private SoundSource birdThrustSfx;

		// Token: 0x04000D5B RID: 3419
		private SoundSource birdFinishSfx;

		// Token: 0x04000D5C RID: 3420
		private SoundSource staticLoopSfx;

		// Token: 0x02000545 RID: 1349
		private class CodeBird : Entity
		{
			// Token: 0x060025D0 RID: 9680 RVA: 0x000FA4F0 File Offset: 0x000F86F0
			public CodeBird(Vector2 origin, string code) : base(origin)
			{
				this.code = code;
				this.origin = origin;
				base.Add(this.sprite = new Sprite(GFX.Game, "scenery/flutterbird/"));
				this.sprite.AddLoop("fly", "flap", 0.08f);
				this.sprite.Play("fly", false, false);
				this.sprite.CenterOrigin();
				this.sprite.Color = ForsakenCitySatellite.Colors[code];
				Vector2 zero = Vector2.Zero;
				zero.X = (float)(code.Contains('L') ? -1 : (code.Contains('R') ? 1 : 0));
				zero.Y = (float)(code.Contains('U') ? -1 : (code.Contains('D') ? 1 : 0));
				this.dash = zero.SafeNormalize();
				base.Add(this.routine = new Coroutine(this.AimlessFlightRoutine(), true));
			}

			// Token: 0x060025D1 RID: 9681 RVA: 0x000FA601 File Offset: 0x000F8801
			public override void Update()
			{
				this.timer += Engine.DeltaTime;
				this.sprite.Y = (float)Math.Sin((double)(this.timer * 2f));
				base.Update();
			}

			// Token: 0x060025D2 RID: 9682 RVA: 0x000FA639 File Offset: 0x000F8839
			public void Dash()
			{
				this.routine.Replace(this.DashRoutine());
			}

			// Token: 0x060025D3 RID: 9683 RVA: 0x000FA64C File Offset: 0x000F884C
			public void Transform(float duration)
			{
				base.Tag = Tags.FrozenUpdate;
				this.routine.Replace(this.TransformRoutine(duration));
			}

			// Token: 0x060025D4 RID: 9684 RVA: 0x000FA670 File Offset: 0x000F8870
			private IEnumerator AimlessFlightRoutine()
			{
				this.speed = Vector2.Zero;
				for (;;)
				{
					Vector2 target = this.origin + Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 16f + Calc.Random.NextFloat(40f));
					float reset = 0f;
					while (reset < 1f && (target - this.Position).Length() > 8f)
					{
						Vector2 vector = (target - this.Position).SafeNormalize();
						this.speed += vector * 420f * Engine.DeltaTime;
						if (this.speed.Length() > 90f)
						{
							this.speed = this.speed.SafeNormalize(90f);
						}
						this.Position += this.speed * Engine.DeltaTime;
						reset += Engine.DeltaTime;
						if (Math.Sign(vector.X) != 0)
						{
							this.sprite.Scale.X = (float)Math.Sign(vector.X);
						}
						yield return null;
					}
					target = default(Vector2);
				}
				yield break;
			}

			// Token: 0x060025D5 RID: 9685 RVA: 0x000FA67F File Offset: 0x000F887F
			private IEnumerator DashRoutine()
			{
				for (float t = 0.25f; t > 0f; t -= Engine.DeltaTime)
				{
					this.speed = Calc.Approach(this.speed, Vector2.Zero, 200f * Engine.DeltaTime);
					this.Position += this.speed * Engine.DeltaTime;
					yield return null;
				}
				Vector2 from = this.Position;
				Vector2 to = this.origin + this.dash * 8f;
				if (Math.Sign(to.X - from.X) != 0)
				{
					this.sprite.Scale.X = (float)Math.Sign(to.X - from.X);
				}
				for (float t = 0f; t < 1f; t += Engine.DeltaTime * 1.5f)
				{
					this.Position = from + (to - from) * Ease.CubeInOut(t);
					yield return null;
				}
				this.Position = to;
				yield return 0.2f;
				from = default(Vector2);
				to = default(Vector2);
				if (this.dash.X != 0f)
				{
					this.sprite.Scale.X = (float)Math.Sign(this.dash.X);
				}
				(base.Scene as Level).Displacement.AddBurst(this.Position, 0.25f, 4f, 24f, 0.4f, null, null);
				this.speed = this.dash * 300f;
				for (float t = 0.4f; t > 0f; t -= Engine.DeltaTime)
				{
					if (t > 0.1f && base.Scene.OnInterval(0.02f))
					{
						base.SceneAs<Level>().ParticlesBG.Emit(ForsakenCitySatellite.Particles[this.code], 1, this.Position, Vector2.One * 2f, this.dash.Angle());
					}
					this.speed = Calc.Approach(this.speed, Vector2.Zero, 800f * Engine.DeltaTime);
					this.Position += this.speed * Engine.DeltaTime;
					yield return null;
				}
				yield return 0.4f;
				this.routine.Replace(this.AimlessFlightRoutine());
				yield break;
			}

			// Token: 0x060025D6 RID: 9686 RVA: 0x000FA68E File Offset: 0x000F888E
			private IEnumerator TransformRoutine(float duration)
			{
				Color colorFrom = this.sprite.Color;
				Color colorTo = Color.White;
				Vector2 target = this.origin;
				base.Add(this.heartImage = new Image(GFX.Game["collectables/heartGem/shape"]));
				this.heartImage.CenterOrigin();
				this.heartImage.Scale = Vector2.Zero;
				for (float t = 0f; t < 1f; t += Engine.DeltaTime / duration)
				{
					Vector2 vector = (target - this.Position).SafeNormalize();
					this.speed += 400f * vector * Engine.DeltaTime;
					float num = Math.Max(20f, (1f - t) * 200f);
					if (this.speed.Length() > num)
					{
						this.speed = this.speed.SafeNormalize(num);
					}
					this.Position += this.speed * Engine.DeltaTime;
					this.sprite.Color = Color.Lerp(colorFrom, colorTo, t);
					this.heartImage.Scale = Vector2.One * Math.Max(0f, (t - 0.75f) * 4f);
					if (vector.X != 0f)
					{
						this.sprite.Scale.X = Math.Abs(this.sprite.Scale.X) * (float)Math.Sign(vector.X);
					}
					this.sprite.Scale.X = (float)Math.Sign(this.sprite.Scale.X) * (1f - this.heartImage.Scale.X);
					this.sprite.Scale.Y = 1f - this.heartImage.Scale.X;
					yield return null;
				}
				yield break;
			}

			// Token: 0x040025C2 RID: 9666
			private Sprite sprite;

			// Token: 0x040025C3 RID: 9667
			private Coroutine routine;

			// Token: 0x040025C4 RID: 9668
			private float timer = Calc.Random.NextFloat();

			// Token: 0x040025C5 RID: 9669
			private Vector2 speed;

			// Token: 0x040025C6 RID: 9670
			private Image heartImage;

			// Token: 0x040025C7 RID: 9671
			private readonly string code;

			// Token: 0x040025C8 RID: 9672
			private readonly Vector2 origin;

			// Token: 0x040025C9 RID: 9673
			private readonly Vector2 dash;
		}
	}
}
