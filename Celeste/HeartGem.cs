using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000261 RID: 609
	[Tracked(false)]
	public class HeartGem : Entity
	{
		// Token: 0x060012EA RID: 4842 RVA: 0x000667EC File Offset: 0x000649EC
		public HeartGem(Vector2 position) : base(position)
		{
			base.Add(this.holdableCollider = new HoldableCollider(new Action<Holdable>(this.OnHoldable), null));
			base.Add(new MirrorReflection());
		}

		// Token: 0x060012EB RID: 4843 RVA: 0x00066840 File Offset: 0x00064A40
		public HeartGem(EntityData data, Vector2 offset) : this(data.Position + offset)
		{
			this.removeCameraTriggers = data.Bool("removeCameraTriggers", false);
			this.IsFake = data.Bool("fake", false);
			this.entityID = new EntityID(data.Level.Name, data.ID);
		}

		// Token: 0x060012EC RID: 4844 RVA: 0x000668A0 File Offset: 0x00064AA0
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			AreaKey area = (base.Scene as Level).Session.Area;
			this.IsGhost = (!this.IsFake && SaveData.Instance.Areas[area.ID].Modes[(int)area.Mode].HeartGem);
			string id;
			if (this.IsFake)
			{
				id = "heartgem3";
			}
			else if (this.IsGhost)
			{
				id = "heartGemGhost";
			}
			else
			{
				id = "heartgem" + (int)area.Mode;
			}
			base.Add(this.sprite = GFX.SpriteBank.Create(id));
			this.sprite.Play("spin", false, false);
			this.sprite.OnLoop = delegate(string anim)
			{
				if (this.Visible && anim == "spin" && this.autoPulse)
				{
					if (this.IsFake)
					{
						Audio.Play("event:/new_content/game/10_farewell/fakeheart_pulse", this.Position);
					}
					else
					{
						Audio.Play("event:/game/general/crystalheart_pulse", this.Position);
					}
					this.ScaleWiggler.Start();
					(base.Scene as Level).Displacement.AddBurst(this.Position, 0.35f, 8f, 48f, 0.25f, null, null);
				}
			};
			if (this.IsGhost)
			{
				this.sprite.Color = Color.White * 0.8f;
			}
			base.Collider = new Hitbox(16f, 16f, -8f, -8f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(this.ScaleWiggler = Wiggler.Create(0.5f, 4f, delegate(float f)
			{
				this.sprite.Scale = Vector2.One * (1f + f * 0.25f);
			}, false, false));
			base.Add(this.bloom = new BloomPoint(0.75f, 16f));
			Color color;
			if (this.IsFake)
			{
				color = Calc.HexToColor("dad8cc");
				this.shineParticle = HeartGem.P_FakeShine;
			}
			else if (area.Mode == AreaMode.Normal)
			{
				color = Color.Aqua;
				this.shineParticle = HeartGem.P_BlueShine;
			}
			else if (area.Mode == AreaMode.BSide)
			{
				color = Color.Red;
				this.shineParticle = HeartGem.P_RedShine;
			}
			else
			{
				color = Color.Gold;
				this.shineParticle = HeartGem.P_GoldShine;
			}
			color = Color.Lerp(color, Color.White, 0.5f);
			base.Add(this.light = new VertexLight(color, 1f, 32, 64));
			if (this.IsFake)
			{
				this.bloom.Alpha = 0f;
				this.light.Alpha = 0f;
			}
			this.moveWiggler = Wiggler.Create(0.8f, 2f, null, false, false);
			this.moveWiggler.StartZero = true;
			base.Add(this.moveWiggler);
			if (this.IsFake)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if ((entity != null && entity.X > base.X) || (scene as Level).Session.GetFlag("fake_heart"))
				{
					this.Visible = false;
					Alarm.Set(this, 0.0001f, delegate
					{
						this.FakeRemoveCameraTrigger();
						base.RemoveSelf();
					}, Alarm.AlarmMode.Oneshot);
					return;
				}
				scene.Add(this.fakeRightWall = new InvisibleBarrier(new Vector2(base.X + 160f, base.Y - 200f), 8f, 400f));
			}
		}

		// Token: 0x060012ED RID: 4845 RVA: 0x00066BC0 File Offset: 0x00064DC0
		public override void Update()
		{
			this.bounceSfxDelay -= Engine.DeltaTime;
			this.timer += Engine.DeltaTime;
			this.sprite.Position = Vector2.UnitY * (float)Math.Sin((double)(this.timer * 2f)) * 2f + this.moveWiggleDir * this.moveWiggler.Value * -8f;
			if (this.white != null)
			{
				this.white.Position = this.sprite.Position;
				this.white.Scale = this.sprite.Scale;
				if (this.white.CurrentAnimationID != this.sprite.CurrentAnimationID)
				{
					this.white.Play(this.sprite.CurrentAnimationID, false, false);
				}
				this.white.SetAnimationFrame(this.sprite.CurrentAnimationFrame);
			}
			if (this.collected)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity == null || entity.Dead)
				{
					this.EndCutscene();
				}
			}
			base.Update();
			if (!this.collected && base.Scene.OnInterval(0.1f))
			{
				base.SceneAs<Level>().Particles.Emit(this.shineParticle, 1, base.Center, Vector2.One * 8f);
			}
		}

		// Token: 0x060012EE RID: 4846 RVA: 0x00066D40 File Offset: 0x00064F40
		public void OnHoldable(Holdable h)
		{
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (!this.collected && entity != null && h.Dangerous(this.holdableCollider))
			{
				this.Collect(entity);
			}
		}

		// Token: 0x060012EF RID: 4847 RVA: 0x00066D80 File Offset: 0x00064F80
		public void OnPlayer(Player player)
		{
			if (!this.collected && !(base.Scene as Level).Frozen)
			{
				if (player.DashAttacking)
				{
					this.Collect(player);
					return;
				}
				if (this.bounceSfxDelay <= 0f)
				{
					if (this.IsFake)
					{
						Audio.Play("event:/new_content/game/10_farewell/fakeheart_bounce", this.Position);
					}
					else
					{
						Audio.Play("event:/game/general/crystalheart_bounce", this.Position);
					}
					this.bounceSfxDelay = 0.1f;
				}
				player.PointBounce(base.Center);
				this.moveWiggler.Start();
				this.ScaleWiggler.Start();
				this.moveWiggleDir = (base.Center - player.Center).SafeNormalize(Vector2.UnitY);
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			}
		}

		// Token: 0x060012F0 RID: 4848 RVA: 0x00066E4C File Offset: 0x0006504C
		private void Collect(Player player)
		{
			AngryOshiro entity = base.Scene.Tracker.GetEntity<AngryOshiro>();
			if (entity != null)
			{
				entity.StopControllingTime();
			}
			base.Add(new Coroutine(this.CollectRoutine(player), true)
			{
				UseRawDeltaTime = true
			});
			this.collected = true;
			if (this.removeCameraTriggers)
			{
				foreach (CameraOffsetTrigger cameraOffsetTrigger in base.Scene.Entities.FindAll<CameraOffsetTrigger>())
				{
					cameraOffsetTrigger.RemoveSelf();
				}
			}
		}

		// Token: 0x060012F1 RID: 4849 RVA: 0x00066EEC File Offset: 0x000650EC
		private IEnumerator CollectRoutine(Player player)
		{
			Level level = base.Scene as Level;
			AreaKey area = level.Session.Area;
			string poemID = AreaData.Get(level).Mode[(int)area.Mode].PoemID;
			bool completeArea = !this.IsFake && (area.Mode != AreaMode.Normal || area.ID == 9);
			if (this.IsFake)
			{
				level.StartCutscene(new Action<Level>(this.SkipFakeHeartCutscene), true, false, true);
			}
			else
			{
				level.CanRetry = false;
			}
			if (completeArea || this.IsFake)
			{
				Audio.SetMusic(null, true, true);
				Audio.SetAmbience(null, true);
			}
			if (completeArea)
			{
				List<Strawberry> list = new List<Strawberry>();
				foreach (Follower follower in player.Leader.Followers)
				{
					if (follower.Entity is Strawberry)
					{
						list.Add(follower.Entity as Strawberry);
					}
				}
				foreach (Strawberry strawberry in list)
				{
					strawberry.OnCollect();
				}
			}
			string text = "event:/game/general/crystalheart_blue_get";
			if (this.IsFake)
			{
				text = "event:/new_content/game/10_farewell/fakeheart_get";
			}
			else if (area.Mode == AreaMode.BSide)
			{
				text = "event:/game/general/crystalheart_red_get";
			}
			else if (area.Mode == AreaMode.CSide)
			{
				text = "event:/game/general/crystalheart_gold_get";
			}
			this.sfx = SoundEmitter.Play(text, this, null);
			base.Add(new LevelEndingHook(delegate()
			{
				this.sfx.Source.Stop(true);
			}));
			this.walls.Add(new InvisibleBarrier(new Vector2((float)level.Bounds.Right, (float)level.Bounds.Top), 8f, (float)level.Bounds.Height));
			this.walls.Add(new InvisibleBarrier(new Vector2((float)(level.Bounds.Left - 8), (float)level.Bounds.Top), 8f, (float)level.Bounds.Height));
			this.walls.Add(new InvisibleBarrier(new Vector2((float)level.Bounds.Left, (float)(level.Bounds.Top - 8)), (float)level.Bounds.Width, 8f));
			foreach (InvisibleBarrier entity in this.walls)
			{
				base.Scene.Add(entity);
			}
			base.Add(this.white = GFX.SpriteBank.Create("heartGemWhite"));
			base.Depth = -2000000;
			yield return null;
			Celeste.Freeze(0.2f);
			yield return null;
			Engine.TimeRate = 0.5f;
			player.Depth = -2000000;
			for (int i = 0; i < 10; i++)
			{
				base.Scene.Add(new AbsorbOrb(this.Position, null, null));
			}
			level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			level.Flash(Color.White, false);
			level.FormationBackdrop.Display = true;
			level.FormationBackdrop.Alpha = 1f;
			this.light.Alpha = (this.bloom.Alpha = 0f);
			this.Visible = false;
			for (float t = 0f; t < 2f; t += Engine.RawDeltaTime)
			{
				Engine.TimeRate = Calc.Approach(Engine.TimeRate, 0f, Engine.RawDeltaTime * 0.25f);
				yield return null;
			}
			yield return null;
			if (player.Dead)
			{
				yield return 100f;
			}
			Engine.TimeRate = 1f;
			base.Tag = Tags.FrozenUpdate;
			level.Frozen = true;
			if (!this.IsFake)
			{
				this.RegisterAsCollected(level, poemID);
				if (completeArea)
				{
					level.TimerStopped = true;
					level.RegisterAreaComplete();
				}
			}
			string text2 = null;
			if (!string.IsNullOrEmpty(poemID))
			{
				text2 = Dialog.Clean("poem_" + poemID, null);
			}
			this.poem = new Poem(text2, (int)(this.IsFake ? ((AreaMode)3) : area.Mode), (area.Mode == AreaMode.CSide || this.IsFake) ? 1f : 0.6f);
			this.poem.Alpha = 0f;
			base.Scene.Add(this.poem);
			for (float t = 0f; t < 1f; t += Engine.RawDeltaTime)
			{
				this.poem.Alpha = Ease.CubeOut(t);
				yield return null;
			}
			if (this.IsFake)
			{
				yield return this.DoFakeRoutineWithBird(player);
			}
			else
			{
				while (!Input.MenuConfirm.Pressed && !Input.MenuCancel.Pressed)
				{
					yield return null;
				}
				this.sfx.Source.Param("end", 1f);
				if (!completeArea)
				{
					level.FormationBackdrop.Display = false;
					for (float t = 0f; t < 1f; t += Engine.RawDeltaTime * 2f)
					{
						this.poem.Alpha = Ease.CubeIn(1f - t);
						yield return null;
					}
					player.Depth = 0;
					this.EndCutscene();
				}
				else
				{
					yield return new FadeWipe(level, false, null)
					{
						Duration = 3.25f
					}.Duration;
					level.CompleteArea(false, true, false);
				}
			}
			yield break;
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x00066F04 File Offset: 0x00065104
		private void EndCutscene()
		{
			Level level = base.Scene as Level;
			level.Frozen = false;
			level.CanRetry = true;
			level.FormationBackdrop.Display = false;
			Engine.TimeRate = 1f;
			if (this.poem != null)
			{
				this.poem.RemoveSelf();
			}
			foreach (InvisibleBarrier invisibleBarrier in this.walls)
			{
				invisibleBarrier.RemoveSelf();
			}
			base.RemoveSelf();
		}

		// Token: 0x060012F3 RID: 4851 RVA: 0x00066F9C File Offset: 0x0006519C
		private void RegisterAsCollected(Level level, string poemID)
		{
			level.Session.HeartGem = true;
			level.Session.UpdateLevelStartDashes();
			int unlockedModes = SaveData.Instance.UnlockedModes;
			SaveData.Instance.RegisterHeartGem(level.Session.Area);
			if (!string.IsNullOrEmpty(poemID))
			{
				SaveData.Instance.RegisterPoemEntry(poemID);
			}
			if (unlockedModes < 3 && SaveData.Instance.UnlockedModes >= 3)
			{
				level.Session.UnlockedCSide = true;
			}
			if (SaveData.Instance.TotalHeartGems >= 24)
			{
				Achievements.Register(Achievement.CSIDES);
			}
		}

		// Token: 0x060012F4 RID: 4852 RVA: 0x00067024 File Offset: 0x00065224
		private IEnumerator DoFakeRoutineWithBird(Player player)
		{
			Level level = base.Scene as Level;
			int panAmount = 64;
			Vector2 panFrom = level.Camera.Position;
			Vector2 panTo = level.Camera.Position + new Vector2((float)(-(float)panAmount), 0f);
			Vector2 birdFrom = new Vector2(panTo.X - 16f, player.Y - 20f);
			Vector2 birdTo = new Vector2(panFrom.X + 320f + 16f, player.Y - 20f);
			yield return 2f;
			Glitch.Value = 0.75f;
			while (Glitch.Value > 0f)
			{
				Glitch.Value = Calc.Approach(Glitch.Value, 0f, Engine.RawDeltaTime * 4f);
				level.Shake(0.3f);
				yield return null;
			}
			yield return 1.1f;
			Glitch.Value = 0.75f;
			while (Glitch.Value > 0f)
			{
				Glitch.Value = Calc.Approach(Glitch.Value, 0f, Engine.RawDeltaTime * 4f);
				level.Shake(0.3f);
				yield return null;
			}
			yield return 0.4f;
			for (float p = 0f; p < 1f; p += Engine.RawDeltaTime / 2f)
			{
				level.Camera.Position = panFrom + (panTo - panFrom) * Ease.CubeInOut(p);
				this.poem.Offset = new Vector2((float)(panAmount * 8), 0f) * Ease.CubeInOut(p);
				yield return null;
			}
			this.bird = new BirdNPC(birdFrom, BirdNPC.Modes.None);
			this.bird.Sprite.Play("fly", false, false);
			this.bird.Sprite.UseRawDeltaTime = true;
			this.bird.Facing = Facings.Right;
			this.bird.Depth = -2000100;
			this.bird.Tag = Tags.FrozenUpdate;
			this.bird.Add(new VertexLight(Color.White, 0.5f, 8, 32));
			this.bird.Add(new BloomPoint(0.5f, 12f));
			level.Add(this.bird);
			for (float p = 0f; p < 1f; p += Engine.RawDeltaTime / 2.6f)
			{
				level.Camera.Position = panTo + (panFrom - panTo) * Ease.CubeInOut(p);
				this.poem.Offset = new Vector2((float)(panAmount * 8), 0f) * Ease.CubeInOut(1f - p);
				float num = 0.1f;
				float num2 = 0.9f;
				if (p > num && p <= num2)
				{
					float num3 = (p - num) / (num2 - num);
					this.bird.Position = birdFrom + (birdTo - birdFrom) * num3 + Vector2.UnitY * (float)Math.Sin((double)(num3 * 8f)) * 8f;
				}
				if (level.OnRawInterval(0.2f))
				{
					TrailManager.Add(this.bird, Calc.HexToColor("639bff"), 1f, true, true);
				}
				yield return null;
			}
			this.bird.RemoveSelf();
			this.bird = null;
			Engine.TimeRate = 0f;
			level.Frozen = false;
			player.Active = false;
			player.StateMachine.State = 11;
			while (Engine.TimeRate != 1f)
			{
				Engine.TimeRate = Calc.Approach(Engine.TimeRate, 1f, 0.5f * Engine.RawDeltaTime);
				yield return null;
			}
			Engine.TimeRate = 1f;
			yield return Textbox.Say("CH9_FAKE_HEART", new Func<IEnumerator>[0]);
			this.sfx.Source.Param("end", 1f);
			yield return 0.283f;
			level.FormationBackdrop.Display = false;
			for (float p = 0f; p < 1f; p += Engine.RawDeltaTime / 0.2f)
			{
				this.poem.TextAlpha = Ease.CubeIn(1f - p);
				this.poem.ParticleSpeed = this.poem.TextAlpha;
				yield return null;
			}
			this.poem.Heart.Play("break", false, false);
			while (this.poem.Heart.Animating)
			{
				this.poem.Shake += Engine.DeltaTime;
				yield return null;
			}
			this.poem.RemoveSelf();
			this.poem = null;
			for (int i = 0; i < 10; i++)
			{
				Vector2 position = level.Camera.Position + new Vector2(320f, 180f) * 0.5f;
				Vector2 value = level.Camera.Position + new Vector2(160f, -64f);
				base.Scene.Add(new AbsorbOrb(position, null, new Vector2?(value)));
			}
			level.Shake(0.3f);
			Glitch.Value = 0.8f;
			while (Glitch.Value > 0f)
			{
				Glitch.Value -= Engine.DeltaTime * 4f;
				yield return null;
			}
			yield return 0.25f;
			level.Session.Audio.Music.Event = "event:/new_content/music/lvl10/intermission_heartgroove";
			level.Session.Audio.Apply(false);
			player.Active = true;
			player.Depth = 0;
			player.StateMachine.State = 11;
			while (!player.OnGround(1) && player.Bottom < (float)level.Bounds.Bottom)
			{
				yield return null;
			}
			player.Facing = Facings.Right;
			yield return 0.5f;
			yield return Textbox.Say("CH9_KEEP_GOING", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.PlayerStepForward)
			});
			this.SkipFakeHeartCutscene(level);
			level.EndCutscene();
			yield break;
		}

		// Token: 0x060012F5 RID: 4853 RVA: 0x0006703A File Offset: 0x0006523A
		private IEnumerator PlayerStepForward()
		{
			yield return 0.1f;
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null && entity.CollideCheck<Solid>(entity.Position + new Vector2(12f, 1f)))
			{
				yield return entity.DummyWalkToExact((int)entity.X + 10, false, 1f, false);
			}
			yield return 0.2f;
			yield break;
		}

		// Token: 0x060012F6 RID: 4854 RVA: 0x0006704C File Offset: 0x0006524C
		private void SkipFakeHeartCutscene(Level level)
		{
			Engine.TimeRate = 1f;
			Glitch.Value = 0f;
			if (this.sfx != null)
			{
				this.sfx.Source.Stop(true);
			}
			level.Session.SetFlag("fake_heart", true);
			level.Frozen = false;
			level.FormationBackdrop.Display = false;
			level.Session.Audio.Music.Event = "event:/new_content/music/lvl10/intermission_heartgroove";
			level.Session.Audio.Apply(false);
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				entity.Sprite.Play("idle", false, false);
				entity.Active = true;
				entity.StateMachine.State = 0;
				entity.Dashes = 1;
				entity.Speed = Vector2.Zero;
				entity.MoveV(200f, null, null);
				entity.Depth = 0;
				for (int i = 0; i < 10; i++)
				{
					entity.UpdateHair(true);
				}
			}
			foreach (AbsorbOrb absorbOrb in base.Scene.Entities.FindAll<AbsorbOrb>())
			{
				absorbOrb.RemoveSelf();
			}
			if (this.poem != null)
			{
				this.poem.RemoveSelf();
			}
			if (this.bird != null)
			{
				this.bird.RemoveSelf();
			}
			if (this.fakeRightWall != null)
			{
				this.fakeRightWall.RemoveSelf();
			}
			this.FakeRemoveCameraTrigger();
			foreach (InvisibleBarrier invisibleBarrier in this.walls)
			{
				invisibleBarrier.RemoveSelf();
			}
			base.RemoveSelf();
		}

		// Token: 0x060012F7 RID: 4855 RVA: 0x0006721C File Offset: 0x0006541C
		private void FakeRemoveCameraTrigger()
		{
			CameraTargetTrigger cameraTargetTrigger = base.CollideFirst<CameraTargetTrigger>();
			if (cameraTargetTrigger != null)
			{
				cameraTargetTrigger.LerpStrength = 0f;
			}
		}

		// Token: 0x04000ED8 RID: 3800
		private const string FAKE_HEART_FLAG = "fake_heart";

		// Token: 0x04000ED9 RID: 3801
		public static ParticleType P_BlueShine;

		// Token: 0x04000EDA RID: 3802
		public static ParticleType P_RedShine;

		// Token: 0x04000EDB RID: 3803
		public static ParticleType P_GoldShine;

		// Token: 0x04000EDC RID: 3804
		public static ParticleType P_FakeShine;

		// Token: 0x04000EDD RID: 3805
		public bool IsGhost;

		// Token: 0x04000EDE RID: 3806
		public const float GhostAlpha = 0.8f;

		// Token: 0x04000EDF RID: 3807
		public bool IsFake;

		// Token: 0x04000EE0 RID: 3808
		private Sprite sprite;

		// Token: 0x04000EE1 RID: 3809
		private Sprite white;

		// Token: 0x04000EE2 RID: 3810
		private ParticleType shineParticle;

		// Token: 0x04000EE3 RID: 3811
		public Wiggler ScaleWiggler;

		// Token: 0x04000EE4 RID: 3812
		private Wiggler moveWiggler;

		// Token: 0x04000EE5 RID: 3813
		private Vector2 moveWiggleDir;

		// Token: 0x04000EE6 RID: 3814
		private BloomPoint bloom;

		// Token: 0x04000EE7 RID: 3815
		private VertexLight light;

		// Token: 0x04000EE8 RID: 3816
		private Poem poem;

		// Token: 0x04000EE9 RID: 3817
		private BirdNPC bird;

		// Token: 0x04000EEA RID: 3818
		private float timer;

		// Token: 0x04000EEB RID: 3819
		private bool collected;

		// Token: 0x04000EEC RID: 3820
		private bool autoPulse = true;

		// Token: 0x04000EED RID: 3821
		private float bounceSfxDelay;

		// Token: 0x04000EEE RID: 3822
		private bool removeCameraTriggers;

		// Token: 0x04000EEF RID: 3823
		private SoundEmitter sfx;

		// Token: 0x04000EF0 RID: 3824
		private List<InvisibleBarrier> walls = new List<InvisibleBarrier>();

		// Token: 0x04000EF1 RID: 3825
		private HoldableCollider holdableCollider;

		// Token: 0x04000EF2 RID: 3826
		private EntityID entityID;

		// Token: 0x04000EF3 RID: 3827
		private InvisibleBarrier fakeRightWall;
	}
}
