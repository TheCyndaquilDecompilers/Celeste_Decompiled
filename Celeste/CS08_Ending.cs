using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000168 RID: 360
	public class CS08_Ending : CutsceneEntity
	{
		// Token: 0x06000CD1 RID: 3281 RVA: 0x0002B510 File Offset: 0x00029710
		public CS08_Ending() : base(false, true)
		{
			base.Tag = (Tags.HUD | Tags.PauseUpdate);
			this.RemoveOnSkipped = false;
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x0002B55C File Offset: 0x0002975C
		public override void OnBegin(Level level)
		{
			level.SaveQuitDisabled = true;
			int totalStrawberries = SaveData.Instance.TotalStrawberries;
			string id;
			if (totalStrawberries < 20)
			{
				id = "final1";
				this.endingDialog = "EP_PIE_DISAPPOINTED";
			}
			else if (totalStrawberries < 50)
			{
				id = "final2";
				this.endingDialog = "EP_PIE_GROSSED_OUT";
			}
			else if (totalStrawberries < 90)
			{
				id = "final3";
				this.endingDialog = "EP_PIE_OKAY";
			}
			else if (totalStrawberries < 150)
			{
				id = "final4";
				this.endingDialog = "EP_PIE_REALLY_GOOD";
			}
			else
			{
				id = "final5";
				this.endingDialog = "EP_PIE_AMAZING";
			}
			base.Add(this.vignettebg = new Image(GFX.Portraits["finalbg"]));
			this.vignettebg.Visible = false;
			base.Add(this.vignette = new Image(GFX.Portraits[id]));
			this.vignette.Visible = false;
			this.vignette.CenterOrigin();
			this.vignette.Position = Celeste.TargetCenter;
			base.Add(this.cutscene = new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x0002B687 File Offset: 0x00029887
		private IEnumerator Cutscene(Level level)
		{
			level.ZoomSnap(new Vector2(164f, 120f), 2f);
			level.Wipe.Cancel();
			new FadeWipe(level, true, null);
			while (this.player == null)
			{
				this.granny = level.Entities.FindFirst<NPC08_Granny>();
				this.theo = level.Entities.FindFirst<NPC08_Theo>();
				this.player = level.Tracker.GetEntity<Player>();
				yield return null;
			}
			this.player.StateMachine.State = 11;
			yield return 1f;
			yield return this.player.DummyWalkToExact((int)this.player.X + 16, false, 1f, false);
			yield return 0.25f;
			yield return Textbox.Say("EP_CABIN", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.BadelineEmerges),
				new Func<IEnumerator>(this.OshiroEnters),
				new Func<IEnumerator>(this.OshiroSettles),
				new Func<IEnumerator>(this.MaddyTurns)
			});
			yield return new FadeWipe(this.Level, false, null)
			{
				Duration = 1.5f
			}.Wait();
			this.fade = 1f;
			yield return Textbox.Say("EP_PIE_START", new Func<IEnumerator>[0]);
			yield return 0.5f;
			this.vignettebg.Visible = true;
			this.vignette.Visible = true;
			this.vignettebg.Color = Color.Black;
			this.vignette.Color = Color.White * 0f;
			base.Add(this.vignette);
			float p;
			for (p = 0f; p < 1f; p += Engine.DeltaTime)
			{
				this.vignette.Color = Color.White * Ease.CubeIn(p);
				this.vignette.Scale = Vector2.One * (1f + 0.25f * (1f - p));
				this.vignette.Rotation = 0.05f * (1f - p);
				yield return null;
			}
			this.vignette.Color = Color.White;
			this.vignettebg.Color = Color.White;
			yield return 2f;
			p = 1f;
			float p2;
			for (p2 = 0f; p2 < 1f; p2 += Engine.DeltaTime / p)
			{
				float num = Ease.CubeOut(p2);
				this.vignette.Position = Vector2.Lerp(Celeste.TargetCenter, Celeste.TargetCenter + new Vector2(0f, 140f), num);
				this.vignette.Scale = Vector2.One * (0.65f + 0.35f * (1f - num));
				this.vignette.Rotation = -0.025f * num;
				yield return null;
			}
			yield return Textbox.Say(this.endingDialog, new Func<IEnumerator>[0]);
			yield return 0.25f;
			p = 2f;
			Vector2 posFrom = this.vignette.Position;
			p2 = this.vignette.Rotation;
			float scaleFrom = this.vignette.Scale.X;
			for (float p3 = 0f; p3 < 1f; p3 += Engine.DeltaTime / p)
			{
				float amount = Ease.CubeOut(p3);
				this.vignette.Position = Vector2.Lerp(posFrom, Celeste.TargetCenter, amount);
				this.vignette.Scale = Vector2.One * MathHelper.Lerp(scaleFrom, 1f, amount);
				this.vignette.Rotation = MathHelper.Lerp(p2, 0f, amount);
				yield return null;
			}
			posFrom = default(Vector2);
			base.EndCutscene(level, false);
			yield break;
		}

		// Token: 0x06000CD4 RID: 3284 RVA: 0x0002B6A0 File Offset: 0x000298A0
		public override void OnEnd(Level level)
		{
			this.vignette.Visible = true;
			this.vignette.Color = Color.White;
			this.vignette.Position = Celeste.TargetCenter;
			this.vignette.Scale = Vector2.One;
			this.vignette.Rotation = 0f;
			if (this.player != null)
			{
				this.player.Speed = Vector2.Zero;
			}
			Textbox textbox = base.Scene.Entities.FindFirst<Textbox>();
			if (textbox != null)
			{
				textbox.RemoveSelf();
			}
			this.cutscene.RemoveSelf();
			base.Add(new Coroutine(this.EndingRoutine(), true));
		}

		// Token: 0x06000CD5 RID: 3285 RVA: 0x0002B748 File Offset: 0x00029948
		private IEnumerator EndingRoutine()
		{
			this.Level.InCutscene = true;
			this.Level.PauseLock = true;
			yield return 0.5f;
			TimeSpan timeSpan = TimeSpan.FromTicks(SaveData.Instance.Time);
			string text = ((int)timeSpan.TotalHours).ToString() + timeSpan.ToString("\\:mm\\:ss\\.fff");
			StrawberriesCounter strawbs = new StrawberriesCounter(true, SaveData.Instance.TotalStrawberries, 175, true);
			DeathsCounter deaths = new DeathsCounter(AreaMode.Normal, true, SaveData.Instance.TotalDeaths, 0);
			CS08_Ending.TimeDisplay time = new CS08_Ending.TimeDisplay(text);
			float timeWidth = SpeedrunTimerDisplay.GetTimeWidth(text, 1f);
			base.Add(strawbs);
			base.Add(deaths);
			base.Add(time);
			Vector2 from = new Vector2(960f, 1180f);
			Vector2 to = new Vector2(960f, 940f);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.5f)
			{
				Vector2 value = Vector2.Lerp(from, to, Ease.CubeOut(p));
				strawbs.Position = value + new Vector2(-170f, 0f);
				deaths.Position = value + new Vector2(170f, 0f);
				time.Position = value + new Vector2(-timeWidth / 2f, 100f);
				yield return null;
			}
			strawbs = null;
			deaths = null;
			time = null;
			from = default(Vector2);
			to = default(Vector2);
			this.showVersion = true;
			yield return 0.25f;
			while (!Input.MenuConfirm.Pressed)
			{
				yield return null;
			}
			this.showVersion = false;
			yield return 0.25f;
			this.Level.CompleteArea(false, false, false);
			yield break;
		}

		// Token: 0x06000CD6 RID: 3286 RVA: 0x0002B757 File Offset: 0x00029957
		private IEnumerator MaddyTurns()
		{
			yield return 0.1f;
			this.player.Facing = -this.player.Facing;
			yield return 0.1f;
			yield break;
		}

		// Token: 0x06000CD7 RID: 3287 RVA: 0x0002B766 File Offset: 0x00029966
		private IEnumerator BadelineEmerges()
		{
			this.Level.Displacement.AddBurst(this.player.Center, 0.5f, 8f, 32f, 0.5f, null, null);
			this.Level.Session.Inventory.Dashes = 1;
			this.player.Dashes = 1;
			this.Level.Add(this.badeline = new BadelineDummy(this.player.Position));
			Audio.Play("event:/char/badeline/maddy_split", this.player.Position);
			this.badeline.Sprite.Scale.X = 1f;
			yield return this.badeline.FloatTo(this.player.Position + new Vector2(-12f, -16f), new int?(1), false, false, false);
			yield break;
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x0002B775 File Offset: 0x00029975
		private IEnumerator OshiroEnters()
		{
			yield return new FadeWipe(this.Level, false, null)
			{
				Duration = 1.5f
			}.Wait();
			this.fade = 1f;
			yield return 0.25f;
			float x = this.player.X;
			this.player.X = this.granny.X + 8f;
			this.badeline.X = this.player.X + 12f;
			this.player.Facing = Facings.Left;
			this.badeline.Sprite.Scale.X = -1f;
			this.granny.X = x + 8f;
			this.theo.X += 16f;
			this.Level.Add(this.oshiro = new Entity(new Vector2(this.granny.X - 24f, this.granny.Y + 4f)));
			OshiroSprite component = new OshiroSprite(1);
			this.oshiro.Add(component);
			this.fade = 0f;
			FadeWipe fadeWipe = new FadeWipe(this.Level, true, null);
			fadeWipe.Duration = 1f;
			yield return 0.25f;
			while (this.oshiro.Y > this.granny.Y - 4f)
			{
				this.oshiro.Y -= Engine.DeltaTime * 32f;
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x0002B784 File Offset: 0x00029984
		private IEnumerator OshiroSettles()
		{
			Vector2 from = this.oshiro.Position;
			Vector2 to = this.oshiro.Position + new Vector2(40f, 8f);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime)
			{
				this.oshiro.Position = Vector2.Lerp(from, to, p);
				yield return null;
			}
			this.granny.Sprite.Scale.X = 1f;
			yield return null;
			yield break;
		}

		// Token: 0x06000CDA RID: 3290 RVA: 0x0002B793 File Offset: 0x00029993
		public override void Update()
		{
			this.versionAlpha = Calc.Approach(this.versionAlpha, (float)(this.showVersion ? 1 : 0), Engine.DeltaTime * 5f);
			base.Update();
		}

		// Token: 0x06000CDB RID: 3291 RVA: 0x0002B7C4 File Offset: 0x000299C4
		public override void Render()
		{
			if (this.fade > 0f)
			{
				Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.fade);
			}
			base.Render();
			if (Settings.Instance.SpeedrunClock != SpeedrunType.Off && this.versionAlpha > 0f)
			{
				AreaComplete.VersionNumberAndVariants(this.version, this.versionAlpha, 1f);
			}
		}

		// Token: 0x04000829 RID: 2089
		private Player player;

		// Token: 0x0400082A RID: 2090
		private NPC08_Granny granny;

		// Token: 0x0400082B RID: 2091
		private NPC08_Theo theo;

		// Token: 0x0400082C RID: 2092
		private BadelineDummy badeline;

		// Token: 0x0400082D RID: 2093
		private Entity oshiro;

		// Token: 0x0400082E RID: 2094
		private Image vignette;

		// Token: 0x0400082F RID: 2095
		private Image vignettebg;

		// Token: 0x04000830 RID: 2096
		private string endingDialog;

		// Token: 0x04000831 RID: 2097
		private float fade;

		// Token: 0x04000832 RID: 2098
		private bool showVersion;

		// Token: 0x04000833 RID: 2099
		private float versionAlpha;

		// Token: 0x04000834 RID: 2100
		private Coroutine cutscene;

		// Token: 0x04000835 RID: 2101
		private string version = Celeste.Instance.Version.ToString();

		// Token: 0x02000412 RID: 1042
		public class TimeDisplay : Component
		{
			// Token: 0x0600205E RID: 8286 RVA: 0x000DFA6C File Offset: 0x000DDC6C
			public TimeDisplay(string time) : base(true, true)
			{
				this.Time = time;
			}

			// Token: 0x0600205F RID: 8287 RVA: 0x000DFA7D File Offset: 0x000DDC7D
			public override void Render()
			{
				SpeedrunTimerDisplay.DrawTime(this.RenderPosition, this.Time, 1f, true, false, false, 1f);
			}

			// Token: 0x170002EA RID: 746
			// (get) Token: 0x06002060 RID: 8288 RVA: 0x000DFA9D File Offset: 0x000DDC9D
			public Vector2 RenderPosition
			{
				get
				{
					return (((base.Entity != null) ? base.Entity.Position : Vector2.Zero) + this.Position).Round();
				}
			}

			// Token: 0x040020BD RID: 8381
			public Vector2 Position;

			// Token: 0x040020BE RID: 8382
			public string Time;
		}
	}
}
