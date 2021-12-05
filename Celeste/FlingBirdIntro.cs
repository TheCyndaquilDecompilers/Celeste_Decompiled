using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000182 RID: 386
	public class FlingBirdIntro : Entity
	{
		// Token: 0x06000DA0 RID: 3488 RVA: 0x0002FF0C File Offset: 0x0002E10C
		public FlingBirdIntro(Vector2 position, Vector2[] nodes, bool crashes) : base(position)
		{
			this.crashes = crashes;
			base.Add(this.Sprite = GFX.SpriteBank.Create("bird"));
			this.Sprite.Play(crashes ? "hoverStressed" : "hover", false, false);
			this.Sprite.Scale.X = (float)(crashes ? -1 : 1);
			this.Sprite.OnFrameChange = delegate(string anim)
			{
				if (!this.inCutscene)
				{
					BirdNPC.FlapSfxCheck(this.Sprite);
				}
			};
			base.Collider = new Circle(16f, 0f, -8f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			this.nodes = nodes;
			this.start = position;
			this.BirdEndPosition = nodes[nodes.Length - 1];
		}

		// Token: 0x06000DA1 RID: 3489 RVA: 0x0002FFE2 File Offset: 0x0002E1E2
		public FlingBirdIntro(EntityData data, Vector2 levelOffset) : this(data.Position + levelOffset, data.NodesOffset(levelOffset), data.Bool("crashes", false))
		{
		}

		// Token: 0x06000DA2 RID: 3490 RVA: 0x0003000C File Offset: 0x0002E20C
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (!this.crashes && (scene as Level).Session.GetFlag("MissTheBird"))
			{
				base.RemoveSelf();
				return;
			}
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null && entity.X > base.X)
			{
				if (this.crashes)
				{
					CS10_CatchTheBird.HandlePostCutsceneSpawn(this, scene as Level);
				}
				CassetteBlockManager entity2 = base.Scene.Tracker.GetEntity<CassetteBlockManager>();
				if (entity2 != null)
				{
					entity2.StopBlocks();
					entity2.Finish();
				}
				base.RemoveSelf();
			}
			else
			{
				scene.Add(this.fakeRightWall = new InvisibleBarrier(new Vector2(base.X + 160f, base.Y - 200f), 8f, 400f));
			}
			if (!this.crashes)
			{
				Vector2 position = this.Position;
				this.Position = new Vector2(base.X - 150f, (float)((scene as Level).Bounds.Top - 8));
				base.Add(this.flyToRoutine = new Coroutine(this.FlyTo(position), true));
			}
		}

		// Token: 0x06000DA3 RID: 3491 RVA: 0x0003013A File Offset: 0x0002E33A
		private IEnumerator FlyTo(Vector2 to)
		{
			base.Add(new SoundSource().Play("event:/new_content/game/10_farewell/bird_flappyscene_entry", null, 0f));
			this.Sprite.Play("fly", false, false);
			Vector2 from = this.Position;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 0.3f)
			{
				this.Position = from + (to - from) * Ease.SineOut(p);
				yield return null;
			}
			this.Sprite.Play("hover", false, false);
			float sine = 0f;
			for (;;)
			{
				this.Position = to + Vector2.UnitY * (float)Math.Sin((double)sine) * 8f;
				sine += Engine.DeltaTime * 2f;
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000DA4 RID: 3492 RVA: 0x00030150 File Offset: 0x0002E350
		public override void Removed(Scene scene)
		{
			if (this.fakeRightWall != null)
			{
				this.fakeRightWall.RemoveSelf();
			}
			this.fakeRightWall = null;
			base.Removed(scene);
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x00030174 File Offset: 0x0002E374
		private void OnPlayer(Player player)
		{
			if (!player.Dead && !this.startedRoutine)
			{
				if (this.flyToRoutine != null)
				{
					this.flyToRoutine.RemoveSelf();
				}
				this.startedRoutine = true;
				player.Speed = Vector2.Zero;
				base.Depth = player.Depth - 5;
				this.Sprite.Play("hoverStressed", false, false);
				this.Sprite.Scale.X = 1f;
				this.fakeRightWall.RemoveSelf();
				this.fakeRightWall = null;
				if (!this.crashes)
				{
					base.Scene.Add(new CS10_MissTheBird(player, this));
					return;
				}
				CassetteBlockManager entity = base.Scene.Tracker.GetEntity<CassetteBlockManager>();
				if (entity != null)
				{
					entity.StopBlocks();
					entity.Finish();
				}
				base.Scene.Add(new CS10_CatchTheBird(player, this));
			}
		}

		// Token: 0x06000DA6 RID: 3494 RVA: 0x00030250 File Offset: 0x0002E450
		public override void Update()
		{
			if (!this.startedRoutine && this.fakeRightWall != null)
			{
				Level level = base.Scene as Level;
				if (level.Camera.X > this.fakeRightWall.X - 320f - 16f)
				{
					level.Camera.X = this.fakeRightWall.X - 320f - 16f;
				}
			}
			if (this.emitParticles && base.Scene.OnInterval(0.1f))
			{
				base.SceneAs<Level>().ParticlesBG.Emit(FlingBird.P_Feather, 1, this.Position + new Vector2(0f, -8f), new Vector2(6f, 4f));
			}
			base.Update();
		}

		// Token: 0x06000DA7 RID: 3495 RVA: 0x0003031E File Offset: 0x0002E51E
		public IEnumerator DoGrabbingRoutine(Player player)
		{
			Level level = base.Scene as Level;
			this.inCutscene = true;
			if (!this.crashes)
			{
				this.CrashSfxEmitter = SoundEmitter.Play("event:/new_content/game/10_farewell/bird_flappyscene", this, null);
			}
			else
			{
				this.CrashSfxEmitter = SoundEmitter.Play("event:/new_content/game/10_farewell/bird_crashscene_start", this, null);
			}
			player.StateMachine.State = 11;
			player.DummyGravity = false;
			player.DummyAutoAnimate = false;
			player.ForceCameraUpdate = true;
			player.Sprite.Play("jumpSlow_carry", false, false);
			player.Speed = Vector2.Zero;
			player.Facing = Facings.Right;
			Celeste.Freeze(0.1f);
			level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
			this.emitParticles = true;
			base.Add(new Coroutine(level.ZoomTo(new Vector2(140f, 120f), 1.5f, 4f), true));
			float sin = 0f;
			int index = 0;
			while (index < this.nodes.Length - 1)
			{
				Vector2 position = this.Position;
				Vector2 vector = this.nodes[index];
				SimpleCurve curve = new SimpleCurve(position, vector, position + (vector - position) * 0.5f + new Vector2(0f, -24f));
				float lengthParametric = curve.GetLengthParametric(32);
				float duration = lengthParametric / 100f;
				if (vector.Y < position.Y)
				{
					duration *= 1.1f;
					this.Sprite.Rate = 2f;
				}
				else
				{
					duration *= 0.8f;
					this.Sprite.Rate = 1f;
				}
				if (!this.crashes)
				{
					if (index == 0)
					{
						duration = 0.7f;
					}
					if (index == 1)
					{
						duration += 0.191f;
					}
					if (index == 2)
					{
						duration += 0.191f;
					}
				}
				for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
				{
					sin += Engine.DeltaTime * 10f;
					this.Position = (curve.GetPoint(p) + Vector2.UnitY * (float)Math.Sin((double)sin) * 8f).Floor();
					player.Position = this.Position + new Vector2(2f, 10f);
					int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
					if (currentAnimationFrame == 1)
					{
						player.Position += new Vector2(1f, -1f);
					}
					else if (currentAnimationFrame == 2)
					{
						player.Position += new Vector2(-1f, 0f);
					}
					else if (currentAnimationFrame == 3)
					{
						player.Position += new Vector2(-1f, 1f);
					}
					else if (currentAnimationFrame == 4)
					{
						player.Position += new Vector2(1f, 3f);
					}
					else if (currentAnimationFrame == 5)
					{
						player.Position += new Vector2(2f, 5f);
					}
					yield return null;
				}
				level.Shake(0.3f);
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
				int num = index;
				index = num + 1;
				curve = default(SimpleCurve);
			}
			this.Sprite.Rate = 1f;
			Celeste.Freeze(0.05f);
			yield return null;
			level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
			level.Flash(Color.White, false);
			this.emitParticles = false;
			this.inCutscene = false;
			yield break;
		}

		// Token: 0x040008DA RID: 2266
		public Vector2 BirdEndPosition;

		// Token: 0x040008DB RID: 2267
		public Sprite Sprite;

		// Token: 0x040008DC RID: 2268
		public SoundEmitter CrashSfxEmitter;

		// Token: 0x040008DD RID: 2269
		private Vector2[] nodes;

		// Token: 0x040008DE RID: 2270
		private bool startedRoutine;

		// Token: 0x040008DF RID: 2271
		private Vector2 start;

		// Token: 0x040008E0 RID: 2272
		private InvisibleBarrier fakeRightWall;

		// Token: 0x040008E1 RID: 2273
		private bool crashes;

		// Token: 0x040008E2 RID: 2274
		private Coroutine flyToRoutine;

		// Token: 0x040008E3 RID: 2275
		private bool emitParticles;

		// Token: 0x040008E4 RID: 2276
		private bool inCutscene;
	}
}
