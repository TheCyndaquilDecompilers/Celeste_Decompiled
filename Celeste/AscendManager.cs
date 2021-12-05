using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001CA RID: 458
	public class AscendManager : Entity
	{
		// Token: 0x06000FA0 RID: 4000 RVA: 0x00041E20 File Offset: 0x00040020
		public AscendManager(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Tag = Tags.TransitionUpdate;
			base.Depth = 8900;
			this.index = data.Int("index", 0);
			this.cutscene = data.Attr("cutscene", "");
			this.introLaunch = data.Bool("intro_launch", false);
			this.Dark = data.Bool("dark", false);
			this.Ch9Ending = this.cutscene.Equals("CH9_FREE_BIRD", StringComparison.InvariantCultureIgnoreCase);
			this.ambience = data.Attr("ambience", "");
			this.background = (this.Dark ? Color.Black : Calc.HexToColor("75a0ab"));
		}

		// Token: 0x06000FA1 RID: 4001 RVA: 0x00041EF2 File Offset: 0x000400F2
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = (base.Scene as Level);
			base.Add(new Coroutine(this.Routine(), true));
		}

		// Token: 0x06000FA2 RID: 4002 RVA: 0x00041F1E File Offset: 0x0004011E
		private IEnumerator Routine()
		{
			Player player = base.Scene.Tracker.GetEntity<Player>();
			while (player == null || player.Y > base.Y)
			{
				player = base.Scene.Tracker.GetEntity<Player>();
				yield return null;
			}
			if (this.index == 9)
			{
				yield return 1.6f;
			}
			AscendManager.Streaks entity = new AscendManager.Streaks(this);
			base.Scene.Add(entity);
			if (!this.Dark)
			{
				AscendManager.Clouds entity2 = new AscendManager.Clouds(this);
				base.Scene.Add(entity2);
			}
			this.level.Session.SetFlag("beginswap_" + this.index, true);
			player.Sprite.Play("launch", false, false);
			player.Speed = Vector2.Zero;
			player.StateMachine.State = 11;
			player.DummyGravity = false;
			player.DummyAutoAnimate = false;
			if (!string.IsNullOrWhiteSpace(this.ambience))
			{
				if (this.ambience.Equals("null", StringComparison.InvariantCultureIgnoreCase))
				{
					Audio.SetAmbience(null, true);
				}
				else
				{
					Audio.SetAmbience(SFX.EventnameByHandle(this.ambience), true);
				}
			}
			if (this.introLaunch)
			{
				this.FadeSnapTo(1f);
				this.level.Camera.Position = player.Center + new Vector2(-160f, -90f);
				yield return 2.3f;
			}
			else
			{
				yield return this.FadeTo(1f, this.Dark ? 2f : 0.8f);
				if (this.Ch9Ending)
				{
					this.level.Add(new CS10_FreeBird());
					for (;;)
					{
						yield return null;
					}
				}
				else if (!string.IsNullOrEmpty(this.cutscene))
				{
					yield return 0.25f;
					CS07_Ascend cs = new CS07_Ascend(this.index, this.cutscene, this.Dark);
					this.level.Add(cs);
					yield return null;
					while (cs.Running)
					{
						yield return null;
					}
					cs = null;
				}
				else
				{
					yield return 0.5f;
				}
			}
			this.level.CanRetry = false;
			player.Sprite.Play("launch", false, false);
			Audio.Play("event:/char/madeline/summit_flytonext", player.Position);
			yield return 0.25f;
			Vector2 from = player.Position;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 1f)
			{
				player.Position = Vector2.Lerp(from, from + new Vector2(0f, 60f), Ease.CubeInOut(p)) + Calc.Random.ShakeVector();
				Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
				yield return null;
			}
			AscendManager.Fader fader = new AscendManager.Fader(this);
			base.Scene.Add(fader);
			from = player.Position;
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.5f)
			{
				float y = player.Y;
				player.Position = Vector2.Lerp(from, from + new Vector2(0f, -160f), Ease.SineIn(p));
				if (p == 0f || Calc.OnInterval(player.Y, y, 16f))
				{
					this.level.Add(Engine.Pooler.Create<SpeedRing>().Init(player.Center, new Vector2(0f, -1f).Angle(), Color.White));
				}
				if (p >= 0.5f)
				{
					fader.Fade = (p - 0.5f) * 2f;
				}
				else
				{
					fader.Fade = 0f;
				}
				yield return null;
			}
			from = default(Vector2);
			fader = null;
			this.level.CanRetry = true;
			this.outTheTop = true;
			player.Y = (float)this.level.Bounds.Top;
			player.SummitLaunch(player.X);
			player.DummyGravity = true;
			player.DummyAutoAnimate = true;
			this.level.Session.SetFlag("bgswap_" + this.index, true);
			this.level.NextTransitionDuration = 0.05f;
			if (this.introLaunch)
			{
				this.level.Add(new HeightDisplay(-1));
			}
			yield break;
		}

		// Token: 0x06000FA3 RID: 4003 RVA: 0x00041F2D File Offset: 0x0004012D
		public override void Update()
		{
			this.scroll += Engine.DeltaTime * 240f;
			base.Update();
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x00041F50 File Offset: 0x00040150
		public override void Render()
		{
			Draw.Rect(this.level.Camera.X - 10f, this.level.Camera.Y - 10f, 340f, 200f, this.background * this.fade);
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x00041FAC File Offset: 0x000401AC
		public override void Removed(Scene scene)
		{
			this.FadeSnapTo(0f);
			this.level.Session.SetFlag("bgswap_" + this.index, false);
			this.level.Session.SetFlag("beginswap_" + this.index, false);
			if (this.outTheTop)
			{
				ScreenWipe.WipeColor = (this.Dark ? Color.Black : Color.White);
				if (this.introLaunch)
				{
					new MountainWipe(base.Scene, true, null);
				}
				else if (this.index == 0)
				{
					AreaData.Get(1).DoScreenWipe(base.Scene, true, null);
				}
				else if (this.index == 1)
				{
					AreaData.Get(2).DoScreenWipe(base.Scene, true, null);
				}
				else if (this.index == 2)
				{
					AreaData.Get(3).DoScreenWipe(base.Scene, true, null);
				}
				else if (this.index == 3)
				{
					AreaData.Get(4).DoScreenWipe(base.Scene, true, null);
				}
				else if (this.index == 4)
				{
					AreaData.Get(5).DoScreenWipe(base.Scene, true, null);
				}
				else if (this.index == 5)
				{
					AreaData.Get(7).DoScreenWipe(base.Scene, true, null);
				}
				else if (this.index >= 9)
				{
					AreaData.Get(10).DoScreenWipe(base.Scene, true, null);
				}
				ScreenWipe.WipeColor = Color.Black;
			}
			base.Removed(scene);
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x00042137 File Offset: 0x00040337
		private IEnumerator FadeTo(float target, float duration = 0.8f)
		{
			while ((this.fade = Calc.Approach(this.fade, target, Engine.DeltaTime / duration)) != target)
			{
				this.FadeSnapTo(this.fade);
				yield return null;
			}
			this.FadeSnapTo(target);
			yield break;
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x00042154 File Offset: 0x00040354
		private void FadeSnapTo(float target)
		{
			this.fade = target;
			this.SetSnowAlpha(1f - this.fade);
			this.SetBloom(this.fade * 0.1f);
			if (this.Dark)
			{
				foreach (Parallax parallax in this.level.Background.GetEach<Parallax>())
				{
					parallax.CameraOffset.Y = parallax.CameraOffset.Y - 25f * target;
				}
				foreach (Parallax parallax2 in this.level.Foreground.GetEach<Parallax>())
				{
					parallax2.Alpha = 1f - this.fade;
				}
			}
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x00042240 File Offset: 0x00040440
		private void SetBloom(float add)
		{
			this.level.Bloom.Base = AreaData.Get(this.level).BloomBase + add;
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x00042264 File Offset: 0x00040464
		private void SetSnowAlpha(float value)
		{
			Snow snow = this.level.Foreground.Get<Snow>();
			if (snow != null)
			{
				snow.Alpha = value;
			}
			RainFG rainFG = this.level.Foreground.Get<RainFG>();
			if (rainFG != null)
			{
				rainFG.Alpha = value;
			}
			WindSnowFG windSnowFG = this.level.Foreground.Get<WindSnowFG>();
			if (windSnowFG != null)
			{
				windSnowFG.Alpha = value;
			}
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x000422C2 File Offset: 0x000404C2
		private static float Mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000B07 RID: 2823
		private const string BeginSwapFlag = "beginswap_";

		// Token: 0x04000B08 RID: 2824
		private const string BgSwapFlag = "bgswap_";

		// Token: 0x04000B09 RID: 2825
		public readonly bool Dark;

		// Token: 0x04000B0A RID: 2826
		public readonly bool Ch9Ending;

		// Token: 0x04000B0B RID: 2827
		private bool introLaunch;

		// Token: 0x04000B0C RID: 2828
		private int index;

		// Token: 0x04000B0D RID: 2829
		private string cutscene;

		// Token: 0x04000B0E RID: 2830
		private Level level;

		// Token: 0x04000B0F RID: 2831
		private float fade;

		// Token: 0x04000B10 RID: 2832
		private float scroll;

		// Token: 0x04000B11 RID: 2833
		private bool outTheTop;

		// Token: 0x04000B12 RID: 2834
		private Color background;

		// Token: 0x04000B13 RID: 2835
		private string ambience;

		// Token: 0x020004CA RID: 1226
		public class Streaks : Entity
		{
			// Token: 0x06002400 RID: 9216 RVA: 0x000F0E80 File Offset: 0x000EF080
			public Streaks(AscendManager manager)
			{
				this.manager = manager;
				if (manager == null || !manager.Dark)
				{
					this.colors = new Color[]
					{
						Color.White,
						Calc.HexToColor("e69ecb")
					};
				}
				else
				{
					this.colors = new Color[]
					{
						Calc.HexToColor("041b44"),
						Calc.HexToColor("011230")
					};
				}
				base.Depth = 20;
				this.textures = GFX.Game.GetAtlasSubtextures("scenery/launch/slice");
				this.alphaColors = new Color[this.colors.Length];
				for (int i = 0; i < this.particles.Length; i++)
				{
					float num = 160f + Calc.Random.Range(24f, 144f) * (float)Calc.Random.Choose(-1, 1);
					float y = Calc.Random.NextFloat(436f);
					float speed = Calc.ClampedMap(Math.Abs(num - 160f), 0f, 160f, 0.25f, 1f) * Calc.Random.Range(600f, 2000f);
					this.particles[i] = new AscendManager.Streaks.Particle
					{
						Position = new Vector2(num, y),
						Speed = speed,
						Index = Calc.Random.Next(this.textures.Count),
						Color = Calc.Random.Next(this.colors.Length)
					};
				}
			}

			// Token: 0x06002401 RID: 9217 RVA: 0x000F1028 File Offset: 0x000EF228
			public override void Update()
			{
				base.Update();
				for (int i = 0; i < this.particles.Length; i++)
				{
					AscendManager.Streaks.Particle particle = this.particles[i];
					particle.Position.Y = particle.Position.Y + this.particles[i].Speed * Engine.DeltaTime;
				}
			}

			// Token: 0x06002402 RID: 9218 RVA: 0x000F1078 File Offset: 0x000EF278
			public override void Render()
			{
				float scale = Ease.SineInOut(((this.manager != null) ? this.manager.fade : 1f) * this.Alpha);
				Vector2 position = (base.Scene as Level).Camera.Position;
				for (int i = 0; i < this.colors.Length; i++)
				{
					this.alphaColors[i] = this.colors[i] * scale;
				}
				for (int j = 0; j < this.particles.Length; j++)
				{
					Vector2 vector = this.particles[j].Position;
					vector.X = AscendManager.Mod(vector.X, 320f);
					vector.Y = -128f + AscendManager.Mod(vector.Y, 436f);
					vector += position;
					Vector2 scale2 = new Vector2
					{
						X = Calc.ClampedMap(this.particles[j].Speed, 600f, 2000f, 1f, 0.25f),
						Y = Calc.ClampedMap(this.particles[j].Speed, 600f, 2000f, 1f, 2f)
					} * Calc.ClampedMap(this.particles[j].Speed, 600f, 2000f, 1f, 4f);
					MTexture mtexture = this.textures[this.particles[j].Index];
					Color color = this.alphaColors[this.particles[j].Color];
					mtexture.DrawCentered(vector, color, scale2);
				}
				Draw.Rect(position.X - 10f, position.Y - 10f, 26f, 200f, this.alphaColors[0]);
				Draw.Rect(position.X + 320f - 16f, position.Y - 10f, 26f, 200f, this.alphaColors[0]);
			}

			// Token: 0x0400239A RID: 9114
			private const float MinSpeed = 600f;

			// Token: 0x0400239B RID: 9115
			private const float MaxSpeed = 2000f;

			// Token: 0x0400239C RID: 9116
			public float Alpha = 1f;

			// Token: 0x0400239D RID: 9117
			private AscendManager.Streaks.Particle[] particles = new AscendManager.Streaks.Particle[80];

			// Token: 0x0400239E RID: 9118
			private List<MTexture> textures;

			// Token: 0x0400239F RID: 9119
			private Color[] colors;

			// Token: 0x040023A0 RID: 9120
			private Color[] alphaColors;

			// Token: 0x040023A1 RID: 9121
			private AscendManager manager;

			// Token: 0x02000789 RID: 1929
			private class Particle
			{
				// Token: 0x04002F81 RID: 12161
				public Vector2 Position;

				// Token: 0x04002F82 RID: 12162
				public float Speed;

				// Token: 0x04002F83 RID: 12163
				public int Index;

				// Token: 0x04002F84 RID: 12164
				public int Color;
			}
		}

		// Token: 0x020004CB RID: 1227
		public class Clouds : Entity
		{
			// Token: 0x06002403 RID: 9219 RVA: 0x000F129C File Offset: 0x000EF49C
			public Clouds(AscendManager manager)
			{
				this.manager = manager;
				if (manager == null || !manager.Dark)
				{
					this.color = Calc.HexToColor("b64a86");
				}
				else
				{
					this.color = Calc.HexToColor("082644");
				}
				base.Depth = -1000000;
				this.textures = GFX.Game.GetAtlasSubtextures("scenery/launch/cloud");
				for (int i = 0; i < this.particles.Length; i++)
				{
					this.particles[i] = new AscendManager.Clouds.Particle
					{
						Position = new Vector2(Calc.Random.NextFloat(320f), Calc.Random.NextFloat(900f)),
						Speed = (float)Calc.Random.Range(400, 800),
						Index = Calc.Random.Next(this.textures.Count)
					};
				}
			}

			// Token: 0x06002404 RID: 9220 RVA: 0x000F1390 File Offset: 0x000EF590
			public override void Update()
			{
				base.Update();
				for (int i = 0; i < this.particles.Length; i++)
				{
					AscendManager.Clouds.Particle particle = this.particles[i];
					particle.Position.Y = particle.Position.Y + this.particles[i].Speed * Engine.DeltaTime;
				}
			}

			// Token: 0x06002405 RID: 9221 RVA: 0x000F13E0 File Offset: 0x000EF5E0
			public override void Render()
			{
				float scale = ((this.manager != null) ? this.manager.fade : 1f) * this.Alpha;
				Color color = this.color * scale;
				Vector2 position = (base.Scene as Level).Camera.Position;
				for (int i = 0; i < this.particles.Length; i++)
				{
					Vector2 vector = this.particles[i].Position;
					vector.Y = -360f + AscendManager.Mod(vector.Y, 900f);
					vector += position;
					this.textures[this.particles[i].Index].DrawCentered(vector, color);
				}
			}

			// Token: 0x040023A2 RID: 9122
			public float Alpha;

			// Token: 0x040023A3 RID: 9123
			private AscendManager manager;

			// Token: 0x040023A4 RID: 9124
			private List<MTexture> textures;

			// Token: 0x040023A5 RID: 9125
			private AscendManager.Clouds.Particle[] particles = new AscendManager.Clouds.Particle[10];

			// Token: 0x040023A6 RID: 9126
			private Color color;

			// Token: 0x0200078A RID: 1930
			private class Particle
			{
				// Token: 0x04002F85 RID: 12165
				public Vector2 Position;

				// Token: 0x04002F86 RID: 12166
				public float Speed;

				// Token: 0x04002F87 RID: 12167
				public int Index;
			}
		}

		// Token: 0x020004CC RID: 1228
		private class Fader : Entity
		{
			// Token: 0x06002406 RID: 9222 RVA: 0x000F149C File Offset: 0x000EF69C
			public Fader(AscendManager manager)
			{
				this.manager = manager;
				base.Depth = -1000010;
			}

			// Token: 0x06002407 RID: 9223 RVA: 0x000F14B8 File Offset: 0x000EF6B8
			public override void Render()
			{
				if (this.Fade > 0f)
				{
					Vector2 position = (base.Scene as Level).Camera.Position;
					Draw.Rect(position.X - 10f, position.Y - 10f, 340f, 200f, (this.manager.Dark ? Color.Black : Color.White) * this.Fade);
				}
			}

			// Token: 0x040023A7 RID: 9127
			public float Fade;

			// Token: 0x040023A8 RID: 9128
			private AscendManager manager;
		}
	}
}
