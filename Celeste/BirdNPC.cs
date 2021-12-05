using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000341 RID: 833
	public class BirdNPC : Actor
	{
		// Token: 0x06001A35 RID: 6709 RVA: 0x000A8A0C File Offset: 0x000A6C0C
		public BirdNPC(Vector2 position, BirdNPC.Modes mode) : base(position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("bird"));
			this.Sprite.Scale.X = (float)this.Facing;
			this.Sprite.UseRawDeltaTime = true;
			this.Sprite.OnFrameChange = delegate(string spr)
			{
				if (this.level != null && base.X > this.level.Camera.Left + 64f && base.X < this.level.Camera.Right - 64f && (spr.Equals("peck") || spr.Equals("peckRare")) && this.Sprite.CurrentAnimationFrame == 6)
				{
					Audio.Play("event:/game/general/bird_peck", this.Position);
				}
				if (this.level != null && this.level.Session.Area.ID == 10 && !this.DisableFlapSfx)
				{
					BirdNPC.FlapSfxCheck(this.Sprite);
				}
			};
			base.Add(this.Light = new VertexLight(new Vector2(0f, -8f), Color.White, 1f, 8, 32));
			this.StartPosition = this.Position;
			this.SetMode(mode);
		}

		// Token: 0x06001A36 RID: 6710 RVA: 0x000A8ACC File Offset: 0x000A6CCC
		public BirdNPC(EntityData data, Vector2 offset) : this(data.Position + offset, data.Enum<BirdNPC.Modes>("mode", BirdNPC.Modes.None))
		{
			this.EntityID = new EntityID(data.Level.Name, data.ID);
			this.nodes = data.NodesOffset(offset);
			this.onlyOnce = data.Bool("onlyOnce", false);
			this.onlyIfPlayerLeft = data.Bool("onlyIfPlayerLeft", false);
		}

		// Token: 0x06001A37 RID: 6711 RVA: 0x000A8B44 File Offset: 0x000A6D44
		public void SetMode(BirdNPC.Modes mode)
		{
			this.mode = mode;
			if (this.tutorialRoutine != null)
			{
				this.tutorialRoutine.RemoveSelf();
			}
			if (mode == BirdNPC.Modes.ClimbingTutorial)
			{
				base.Add(this.tutorialRoutine = new Coroutine(this.ClimbingTutorial(), true));
				return;
			}
			if (mode == BirdNPC.Modes.DashingTutorial)
			{
				base.Add(this.tutorialRoutine = new Coroutine(this.DashingTutorial(), true));
				return;
			}
			if (mode == BirdNPC.Modes.DreamJumpTutorial)
			{
				base.Add(this.tutorialRoutine = new Coroutine(this.DreamJumpTutorial(), true));
				return;
			}
			if (mode == BirdNPC.Modes.SuperWallJumpTutorial)
			{
				base.Add(this.tutorialRoutine = new Coroutine(this.SuperWallJumpTutorial(), true));
				return;
			}
			if (mode == BirdNPC.Modes.HyperJumpTutorial)
			{
				base.Add(this.tutorialRoutine = new Coroutine(this.HyperJumpTutorial(), true));
				return;
			}
			if (mode == BirdNPC.Modes.FlyAway)
			{
				base.Add(this.tutorialRoutine = new Coroutine(this.WaitRoutine(), true));
				return;
			}
			if (mode == BirdNPC.Modes.Sleeping)
			{
				this.Sprite.Play("sleep", false, false);
				this.Facing = Facings.Right;
				return;
			}
			if (mode == BirdNPC.Modes.MoveToNodes)
			{
				base.Add(this.tutorialRoutine = new Coroutine(this.MoveToNodesRoutine(), true));
				return;
			}
			if (mode == BirdNPC.Modes.WaitForLightningOff)
			{
				base.Add(this.tutorialRoutine = new Coroutine(this.WaitForLightningOffRoutine(), true));
			}
		}

		// Token: 0x06001A38 RID: 6712 RVA: 0x000A8C88 File Offset: 0x000A6E88
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = (scene as Level);
			if (this.mode == BirdNPC.Modes.ClimbingTutorial && this.level.Session.GetLevelFlag("2"))
			{
				base.RemoveSelf();
				return;
			}
			if (this.mode == BirdNPC.Modes.FlyAway && this.level.Session.GetFlag(BirdNPC.FlownFlag + this.level.Session.Level))
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06001A39 RID: 6713 RVA: 0x000A8D0C File Offset: 0x000A6F0C
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (this.mode == BirdNPC.Modes.SuperWallJumpTutorial)
			{
				Player entity = scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.Y < base.Y + 32f)
				{
					base.RemoveSelf();
				}
			}
			if (this.onlyIfPlayerLeft)
			{
				Player entity2 = this.level.Tracker.GetEntity<Player>();
				if (entity2 != null && entity2.X > base.X)
				{
					base.RemoveSelf();
				}
			}
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x0003F6E0 File Offset: 0x0003D8E0
		public override bool IsRiding(Solid solid)
		{
			return base.Scene.CollideCheck(new Rectangle((int)base.X - 4, (int)base.Y, 8, 2), solid);
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x000A8D82 File Offset: 0x000A6F82
		public override void Update()
		{
			this.Sprite.Scale.X = (float)this.Facing;
			base.Update();
		}

		// Token: 0x06001A3C RID: 6716 RVA: 0x000A8DA1 File Offset: 0x000A6FA1
		public IEnumerator Caw()
		{
			this.Sprite.Play("croak", false, false);
			while (this.Sprite.CurrentAnimationFrame < 9)
			{
				yield return null;
			}
			Audio.Play("event:/game/general/bird_squawk", this.Position);
			yield break;
		}

		// Token: 0x06001A3D RID: 6717 RVA: 0x000A8DB0 File Offset: 0x000A6FB0
		public IEnumerator ShowTutorial(BirdTutorialGui gui, bool caw = false)
		{
			if (caw)
			{
				yield return this.Caw();
			}
			this.gui = gui;
			gui.Open = true;
			base.Scene.Add(gui);
			while (gui.Scale < 1f)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001A3E RID: 6718 RVA: 0x000A8DCD File Offset: 0x000A6FCD
		public IEnumerator HideTutorial()
		{
			if (this.gui != null)
			{
				this.gui.Open = false;
				while (this.gui.Scale > 0f)
				{
					yield return null;
				}
				base.Scene.Remove(this.gui);
				this.gui = null;
			}
			yield break;
		}

		// Token: 0x06001A3F RID: 6719 RVA: 0x000A8DDC File Offset: 0x000A6FDC
		public IEnumerator StartleAndFlyAway()
		{
			base.Depth = -1000000;
			this.level.Session.SetFlag(BirdNPC.FlownFlag + this.level.Session.Level, true);
			yield return this.Startle("event:/game/general/bird_startle", 0.8f, null);
			yield return this.FlyAway(1f);
			yield break;
		}

		// Token: 0x06001A40 RID: 6720 RVA: 0x000A8DEB File Offset: 0x000A6FEB
		public IEnumerator FlyAway(float upwardsMultiplier = 1f)
		{
			if (this.staticMover != null)
			{
				this.staticMover.RemoveSelf();
				this.staticMover = null;
			}
			this.Sprite.Play("fly", false, false);
			this.Facing = -this.Facing;
			Vector2 speed = new Vector2((float)(this.Facing * (Facings)20), -40f * upwardsMultiplier);
			while (base.Y > (float)this.level.Bounds.Top)
			{
				speed += new Vector2((float)(this.Facing * (Facings)140), -120f * upwardsMultiplier) * Engine.DeltaTime;
				this.Position += speed * Engine.DeltaTime;
				yield return null;
			}
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x06001A41 RID: 6721 RVA: 0x000A8E01 File Offset: 0x000A7001
		private IEnumerator ClimbingTutorial()
		{
			yield return 0.25f;
			Player p = base.Scene.Tracker.GetEntity<Player>();
			while (Math.Abs(p.X - base.X) > 120f)
			{
				yield return null;
			}
			BirdTutorialGui tut = new BirdTutorialGui(this, new Vector2(0f, -16f), Dialog.Clean("tutorial_climb", null), new object[]
			{
				Dialog.Clean("tutorial_hold", null),
				BirdTutorialGui.ButtonPrompt.Grab
			});
			BirdTutorialGui tut2 = new BirdTutorialGui(this, new Vector2(0f, -16f), Dialog.Clean("tutorial_climb", null), new object[]
			{
				BirdTutorialGui.ButtonPrompt.Grab,
				"+",
				new Vector2(0f, -1f)
			});
			bool first = true;
			bool willEnd;
			do
			{
				yield return this.ShowTutorial(tut, first);
				first = false;
				while (p.StateMachine.State != 1 && p.Y > base.Y)
				{
					yield return null;
				}
				if (p.Y > base.Y)
				{
					Audio.Play("event:/ui/game/tutorial_note_flip_back");
					yield return this.HideTutorial();
					yield return this.ShowTutorial(tut2, false);
				}
				while (p.Scene != null && (!p.OnGround(1) || p.StateMachine.State == 1))
				{
					yield return null;
				}
				willEnd = (p.Y <= base.Y + 4f);
				if (!willEnd)
				{
					Audio.Play("event:/ui/game/tutorial_note_flip_front");
				}
				yield return this.HideTutorial();
			}
			while (!willEnd);
			yield return this.StartleAndFlyAway();
			yield break;
		}

		// Token: 0x06001A42 RID: 6722 RVA: 0x000A8E10 File Offset: 0x000A7010
		private IEnumerator DashingTutorial()
		{
			base.Y = (float)this.level.Bounds.Top;
			base.X += 32f;
			yield return 1f;
			Player player = base.Scene.Tracker.GetEntity<Player>();
			Bridge bridge = base.Scene.Entities.FindFirst<Bridge>();
			while ((player == null || player.X <= this.StartPosition.X - 92f || player.Y <= this.StartPosition.Y - 20f || player.Y >= this.StartPosition.Y - 10f) && (!SaveData.Instance.Assists.Invincible || player == null || player.X <= this.StartPosition.X - 60f || player.Y <= this.StartPosition.Y || player.Y >= this.StartPosition.Y + 34f))
			{
				yield return null;
			}
			base.Scene.Add(new CS00_Ending(player, this, bridge));
			yield break;
		}

		// Token: 0x06001A43 RID: 6723 RVA: 0x000A8E1F File Offset: 0x000A701F
		private IEnumerator DreamJumpTutorial()
		{
			yield return this.ShowTutorial(new BirdTutorialGui(this, new Vector2(0f, -16f), Dialog.Clean("tutorial_dreamjump", null), new object[]
			{
				new Vector2(1f, 0f),
				"+",
				BirdTutorialGui.ButtonPrompt.Jump
			}), true);
			for (;;)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && (entity.X > base.X || (this.Position - entity.Position).Length() < 32f))
				{
					break;
				}
				yield return null;
			}
			yield return this.HideTutorial();
			for (;;)
			{
				Player entity2 = base.Scene.Tracker.GetEntity<Player>();
				if (entity2 != null && (this.Position - entity2.Position).Length() < 24f)
				{
					break;
				}
				yield return null;
			}
			yield return this.StartleAndFlyAway();
			yield break;
		}

		// Token: 0x06001A44 RID: 6724 RVA: 0x000A8E2E File Offset: 0x000A702E
		private IEnumerator SuperWallJumpTutorial()
		{
			this.Facing = Facings.Right;
			yield return 0.25f;
			bool caw = true;
			BirdTutorialGui tut = new BirdTutorialGui(this, new Vector2(0f, -16f), GFX.Gui["hyperjump/tutorial00"], new object[]
			{
				Dialog.Clean("TUTORIAL_DASH", null),
				new Vector2(0f, -1f)
			});
			BirdTutorialGui tut2 = new BirdTutorialGui(this, new Vector2(0f, -16f), GFX.Gui["hyperjump/tutorial01"], new object[]
			{
				Dialog.Clean("TUTORIAL_DREAMJUMP", null)
			});
			Player entity;
			do
			{
				yield return this.ShowTutorial(tut, caw);
				this.Sprite.Play("idleRarePeck", false, false);
				yield return 2f;
				this.gui = tut2;
				this.gui.Open = true;
				this.gui.Scale = 1f;
				base.Scene.Add(this.gui);
				yield return null;
				tut.Open = false;
				tut.Scale = 0f;
				base.Scene.Remove(tut);
				yield return 2f;
				yield return this.HideTutorial();
				yield return 2f;
				caw = false;
				entity = base.Scene.Tracker.GetEntity<Player>();
			}
			while (entity == null || entity.Y > base.Y || entity.X <= base.X + 144f);
			yield return this.StartleAndFlyAway();
			yield break;
		}

		// Token: 0x06001A45 RID: 6725 RVA: 0x000A8E3D File Offset: 0x000A703D
		private IEnumerator HyperJumpTutorial()
		{
			this.Facing = Facings.Left;
			BirdTutorialGui tut = new BirdTutorialGui(this, new Vector2(0f, -16f), Dialog.Clean("TUTORIAL_DREAMJUMP", null), new object[]
			{
				new Vector2(1f, 1f),
				"+",
				BirdTutorialGui.ButtonPrompt.Dash,
				GFX.Gui["tinyarrow"],
				BirdTutorialGui.ButtonPrompt.Jump
			});
			yield return 0.3f;
			yield return this.ShowTutorial(tut, true);
			yield break;
		}

		// Token: 0x06001A46 RID: 6726 RVA: 0x000A8E4C File Offset: 0x000A704C
		private IEnumerator WaitRoutine()
		{
			while (!this.AutoFly)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && Math.Abs(entity.X - base.X) < 120f)
				{
					break;
				}
				yield return null;
			}
			yield return this.Caw();
			while (!this.AutoFly)
			{
				Player entity2 = base.Scene.Tracker.GetEntity<Player>();
				if (entity2 != null && (entity2.Center - this.Position).Length() < 32f)
				{
					break;
				}
				yield return null;
			}
			yield return this.StartleAndFlyAway();
			yield break;
		}

		// Token: 0x06001A47 RID: 6727 RVA: 0x000A8E5B File Offset: 0x000A705B
		public IEnumerator Startle(string startleSound, float duration = 0.8f, Vector2? multiplier = null)
		{
			if (multiplier == null)
			{
				multiplier = new Vector2?(new Vector2(1f, 1f));
			}
			if (!string.IsNullOrWhiteSpace(startleSound))
			{
				Audio.Play(startleSound, this.Position);
			}
			Dust.Burst(this.Position, -1.5707964f, 8, null);
			this.Sprite.Play("jump", false, false);
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, duration, true);
			tween.OnUpdate = delegate(Tween t)
			{
				if (t.Eased < 0.5f && this.Scene.OnInterval(0.05f))
				{
					this.level.Particles.Emit(BirdNPC.P_Feather, 2, this.Position + Vector2.UnitY * -6f, Vector2.One * 4f);
				}
				Vector2 value = Vector2.Lerp(new Vector2(100f, -100f) * multiplier.Value, new Vector2(20f, -20f) * multiplier.Value, t.Eased);
				value.X *= (float)(-(float)this.Facing);
				this.Position += value * Engine.DeltaTime;
			};
			base.Add(tween);
			while (tween.Active)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001A48 RID: 6728 RVA: 0x000A8E7F File Offset: 0x000A707F
		public IEnumerator FlyTo(Vector2 target, float durationMult = 1f, bool relocateSfx = true)
		{
			this.Sprite.Play("fly", false, false);
			if (relocateSfx)
			{
				base.Add(new SoundSource().Play("event:/new_content/game/10_farewell/bird_relocate", null, 0f));
			}
			int num = Math.Sign(target.X - base.X);
			if (num != 0)
			{
				this.Facing = (Facings)num;
			}
			Vector2 position = this.Position;
			Vector2 vector = target;
			SimpleCurve curve = new SimpleCurve(position, vector, position + (vector - position) * 0.75f - Vector2.UnitY * 30f);
			float duration = (vector - position).Length() / 100f * durationMult;
			for (float p = 0f; p < 0.95f; p += Engine.DeltaTime / duration)
			{
				this.Position = curve.GetPoint(Ease.SineInOut(p)).Floor();
				this.Sprite.Rate = 1f - p * 0.5f;
				yield return null;
			}
			Dust.Burst(this.Position, -1.5707964f, 8, null);
			this.Position = target;
			this.Facing = Facings.Left;
			this.Sprite.Rate = 1f;
			this.Sprite.Play("idle", false, false);
			yield break;
		}

		// Token: 0x06001A49 RID: 6729 RVA: 0x000A8EA3 File Offset: 0x000A70A3
		private IEnumerator MoveToNodesRoutine()
		{
			int index = 0;
			for (;;)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity == null || (entity.Center - this.Position).Length() >= 80f)
				{
					yield return null;
				}
				else
				{
					base.Depth = -1000000;
					yield return this.Startle("event:/new_content/game/10_farewell/bird_startle", 0.2f, null);
					if (index < this.nodes.Length)
					{
						yield return this.FlyTo(this.nodes[index], 0.6f, true);
						int num = index;
						index = num + 1;
					}
					else
					{
						base.Tag = Tags.Persistent;
						base.Add(new SoundSource().Play("event:/new_content/game/10_farewell/bird_relocate", null, 0f));
						if (this.onlyOnce)
						{
							this.level.Session.DoNotLoad.Add(this.EntityID);
						}
						this.Sprite.Play("fly", false, false);
						this.Facing = Facings.Right;
						Vector2 speed = new Vector2((float)(this.Facing * (Facings)20), -40f);
						while (base.Y > (float)(this.level.Bounds.Top - 200))
						{
							speed += new Vector2((float)(this.Facing * (Facings)140), -60f) * Engine.DeltaTime;
							this.Position += speed * Engine.DeltaTime;
							yield return null;
						}
						base.RemoveSelf();
						speed = default(Vector2);
					}
				}
			}
			yield break;
		}

		// Token: 0x06001A4A RID: 6730 RVA: 0x000A8EB2 File Offset: 0x000A70B2
		private IEnumerator WaitForLightningOffRoutine()
		{
			this.Sprite.Play("hoverStressed", false, false);
			while (base.Scene.Entities.FindFirst<Lightning>() != null)
			{
				yield return null;
			}
			if (this.WaitForLightningPostDelay > 0f)
			{
				yield return this.WaitForLightningPostDelay;
			}
			if (!this.FlyAwayUp)
			{
				this.Sprite.Play("fly", false, false);
				Vector2 speed = new Vector2((float)(this.Facing * (Facings)20), -10f);
				while (base.Y > (float)this.level.Bounds.Top)
				{
					speed += new Vector2((float)(this.Facing * (Facings)140), -10f) * Engine.DeltaTime;
					this.Position += speed * Engine.DeltaTime;
					yield return null;
				}
				speed = default(Vector2);
			}
			else
			{
				this.Sprite.Play("flyup", false, false);
				Vector2 speed = new Vector2(0f, -32f);
				while (base.Y > (float)this.level.Bounds.Top)
				{
					speed += new Vector2(0f, -100f) * Engine.DeltaTime;
					this.Position += speed * Engine.DeltaTime;
					yield return null;
				}
				speed = default(Vector2);
			}
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x06001A4B RID: 6731 RVA: 0x000A8EC1 File Offset: 0x000A70C1
		public override void SceneEnd(Scene scene)
		{
			Engine.TimeRate = 1f;
			base.SceneEnd(scene);
		}

		// Token: 0x06001A4C RID: 6732 RVA: 0x000A8ED4 File Offset: 0x000A70D4
		public override void DebugRender(Camera camera)
		{
			base.DebugRender(camera);
			if (this.mode == BirdNPC.Modes.DashingTutorial)
			{
				float x = this.StartPosition.X - 92f;
				float x2 = (float)this.level.Bounds.Right;
				float y = this.StartPosition.Y - 20f;
				float y2 = this.StartPosition.Y - 10f;
				Draw.Line(new Vector2(x, y), new Vector2(x, y2), Color.Aqua);
				Draw.Line(new Vector2(x, y), new Vector2(x2, y), Color.Aqua);
				Draw.Line(new Vector2(x2, y), new Vector2(x2, y2), Color.Aqua);
				Draw.Line(new Vector2(x, y2), new Vector2(x2, y2), Color.Aqua);
				float x3 = this.StartPosition.X - 60f;
				float x4 = (float)this.level.Bounds.Right;
				float y3 = this.StartPosition.Y;
				float y4 = this.StartPosition.Y + 34f;
				Draw.Line(new Vector2(x3, y3), new Vector2(x3, y4), Color.Aqua);
				Draw.Line(new Vector2(x3, y3), new Vector2(x4, y3), Color.Aqua);
				Draw.Line(new Vector2(x4, y3), new Vector2(x4, y4), Color.Aqua);
				Draw.Line(new Vector2(x3, y4), new Vector2(x4, y4), Color.Aqua);
			}
		}

		// Token: 0x06001A4D RID: 6733 RVA: 0x000A905C File Offset: 0x000A725C
		public static void FlapSfxCheck(Sprite sprite)
		{
			if (sprite.Entity != null && sprite.Entity.Scene != null)
			{
				Camera camera = (sprite.Entity.Scene as Level).Camera;
				Vector2 renderPosition = sprite.RenderPosition;
				if (renderPosition.X < camera.X - 32f || renderPosition.Y < camera.Y - 32f || renderPosition.X > camera.X + 320f + 32f || renderPosition.Y > camera.Y + 180f + 32f)
				{
					return;
				}
			}
			string currentAnimationID = sprite.CurrentAnimationID;
			int currentAnimationFrame = sprite.CurrentAnimationFrame;
			if ((currentAnimationID == "hover" && currentAnimationFrame == 0) || (currentAnimationID == "hoverStressed" && currentAnimationFrame == 0) || (currentAnimationID == "fly" && currentAnimationFrame == 0))
			{
				Audio.Play("event:/new_content/game/10_farewell/bird_wingflap", sprite.RenderPosition);
			}
		}

		// Token: 0x040016D7 RID: 5847
		public static ParticleType P_Feather;

		// Token: 0x040016D8 RID: 5848
		private static string FlownFlag = "bird_fly_away_";

		// Token: 0x040016D9 RID: 5849
		public Facings Facing = Facings.Left;

		// Token: 0x040016DA RID: 5850
		public Sprite Sprite;

		// Token: 0x040016DB RID: 5851
		public Vector2 StartPosition;

		// Token: 0x040016DC RID: 5852
		public VertexLight Light;

		// Token: 0x040016DD RID: 5853
		public bool AutoFly;

		// Token: 0x040016DE RID: 5854
		public EntityID EntityID;

		// Token: 0x040016DF RID: 5855
		public bool FlyAwayUp = true;

		// Token: 0x040016E0 RID: 5856
		public float WaitForLightningPostDelay;

		// Token: 0x040016E1 RID: 5857
		public bool DisableFlapSfx;

		// Token: 0x040016E2 RID: 5858
		private Coroutine tutorialRoutine;

		// Token: 0x040016E3 RID: 5859
		private BirdNPC.Modes mode;

		// Token: 0x040016E4 RID: 5860
		private BirdTutorialGui gui;

		// Token: 0x040016E5 RID: 5861
		private Level level;

		// Token: 0x040016E6 RID: 5862
		private Vector2[] nodes;

		// Token: 0x040016E7 RID: 5863
		private StaticMover staticMover;

		// Token: 0x040016E8 RID: 5864
		private bool onlyOnce;

		// Token: 0x040016E9 RID: 5865
		private bool onlyIfPlayerLeft;

		// Token: 0x020006F2 RID: 1778
		public enum Modes
		{
			// Token: 0x04002CFC RID: 11516
			ClimbingTutorial,
			// Token: 0x04002CFD RID: 11517
			DashingTutorial,
			// Token: 0x04002CFE RID: 11518
			DreamJumpTutorial,
			// Token: 0x04002CFF RID: 11519
			SuperWallJumpTutorial,
			// Token: 0x04002D00 RID: 11520
			HyperJumpTutorial,
			// Token: 0x04002D01 RID: 11521
			FlyAway,
			// Token: 0x04002D02 RID: 11522
			None,
			// Token: 0x04002D03 RID: 11523
			Sleeping,
			// Token: 0x04002D04 RID: 11524
			MoveToNodes,
			// Token: 0x04002D05 RID: 11525
			WaitForLightningOff
		}
	}
}
