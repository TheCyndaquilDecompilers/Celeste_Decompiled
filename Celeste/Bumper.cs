using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000189 RID: 393
	public class Bumper : Entity
	{
		// Token: 0x06000DC7 RID: 3527 RVA: 0x00030D6C File Offset: 0x0002EF6C
		public Bumper(Vector2 position, Vector2? node) : base(position)
		{
			base.Collider = new Circle(12f, 0f, 0f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(this.sine = new SineWave(0.44f, 0f).Randomize());
			base.Add(this.sprite = GFX.SpriteBank.Create("bumper"));
			base.Add(this.spriteEvil = GFX.SpriteBank.Create("bumper_evil"));
			this.spriteEvil.Visible = false;
			base.Add(this.light = new VertexLight(Color.Teal, 1f, 16, 32));
			base.Add(this.bloom = new BloomPoint(0.5f, 16f));
			this.node = node;
			this.anchor = this.Position;
			if (node != null)
			{
				Vector2 start = this.Position;
				Vector2 end = node.Value;
				Tween tween = Tween.Create(Tween.TweenMode.Looping, Ease.CubeInOut, 1.8181819f, true);
				tween.OnUpdate = delegate(Tween t)
				{
					if (this.goBack)
					{
						this.anchor = Vector2.Lerp(end, start, t.Eased);
						return;
					}
					this.anchor = Vector2.Lerp(start, end, t.Eased);
				};
				tween.OnComplete = delegate(Tween t)
				{
					this.goBack = !this.goBack;
				};
				base.Add(tween);
			}
			this.UpdatePosition();
			base.Add(this.hitWiggler = Wiggler.Create(1.2f, 2f, delegate(float v)
			{
				this.spriteEvil.Position = this.hitDir * this.hitWiggler.Value * 8f;
			}, false, false));
			base.Add(new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
		}

		// Token: 0x06000DC8 RID: 3528 RVA: 0x00030F31 File Offset: 0x0002F131
		public Bumper(EntityData data, Vector2 offset) : this(data.Position + offset, data.FirstNodeNullable(new Vector2?(offset)))
		{
		}

		// Token: 0x06000DC9 RID: 3529 RVA: 0x00030F54 File Offset: 0x0002F154
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.fireMode = (base.SceneAs<Level>().CoreMode == Session.CoreModes.Hot);
			this.spriteEvil.Visible = this.fireMode;
			this.sprite.Visible = !this.fireMode;
		}

		// Token: 0x06000DCA RID: 3530 RVA: 0x00030FA1 File Offset: 0x0002F1A1
		private void OnChangeMode(Session.CoreModes coreMode)
		{
			this.fireMode = (coreMode == Session.CoreModes.Hot);
			this.spriteEvil.Visible = this.fireMode;
			this.sprite.Visible = !this.fireMode;
		}

		// Token: 0x06000DCB RID: 3531 RVA: 0x00030FD2 File Offset: 0x0002F1D2
		private void UpdatePosition()
		{
			this.Position = this.anchor + new Vector2(this.sine.Value * 3f, this.sine.ValueOverTwo * 2f);
		}

		// Token: 0x06000DCC RID: 3532 RVA: 0x0003100C File Offset: 0x0002F20C
		public override void Update()
		{
			base.Update();
			if (this.respawnTimer > 0f)
			{
				this.respawnTimer -= Engine.DeltaTime;
				if (this.respawnTimer <= 0f)
				{
					this.light.Visible = true;
					this.bloom.Visible = true;
					this.sprite.Play("on", false, false);
					this.spriteEvil.Play("on", false, false);
					if (!this.fireMode)
					{
						Audio.Play("event:/game/06_reflection/pinballbumper_reset", this.Position);
					}
				}
			}
			else if (base.Scene.OnInterval(0.05f))
			{
				float num = Calc.Random.NextAngle();
				ParticleType type = this.fireMode ? Bumper.P_FireAmbience : Bumper.P_Ambience;
				float direction = this.fireMode ? -1.5707964f : num;
				float length = (float)(this.fireMode ? 12 : 8);
				base.SceneAs<Level>().Particles.Emit(type, 1, base.Center + Calc.AngleToVector(num, length), Vector2.One * 2f, direction);
			}
			this.UpdatePosition();
		}

		// Token: 0x06000DCD RID: 3533 RVA: 0x00031138 File Offset: 0x0002F338
		private void OnPlayer(Player player)
		{
			if (this.fireMode)
			{
				if (!SaveData.Instance.Assists.Invincible)
				{
					Vector2 vector = (player.Center - base.Center).SafeNormalize();
					this.hitDir = -vector;
					this.hitWiggler.Start();
					Audio.Play("event:/game/09_core/hotpinball_activate", this.Position);
					this.respawnTimer = 0.6f;
					player.Die(vector, false, true);
					base.SceneAs<Level>().Particles.Emit(Bumper.P_FireHit, 12, base.Center + vector * 12f, Vector2.One * 3f, vector.Angle());
					return;
				}
			}
			else if (this.respawnTimer <= 0f)
			{
				if ((base.Scene as Level).Session.Area.ID == 9)
				{
					Audio.Play("event:/game/09_core/pinballbumper_hit", this.Position);
				}
				else
				{
					Audio.Play("event:/game/06_reflection/pinballbumper_hit", this.Position);
				}
				this.respawnTimer = 0.6f;
				Vector2 vector2 = player.ExplodeLaunch(this.Position, false, false);
				this.sprite.Play("hit", true, false);
				this.spriteEvil.Play("hit", true, false);
				this.light.Visible = false;
				this.bloom.Visible = false;
				base.SceneAs<Level>().DirectionalShake(vector2, 0.15f);
				base.SceneAs<Level>().Displacement.AddBurst(base.Center, 0.3f, 8f, 32f, 0.8f, null, null);
				base.SceneAs<Level>().Particles.Emit(Bumper.P_Launch, 12, base.Center + vector2 * 12f, Vector2.One * 3f, vector2.Angle());
			}
		}

		// Token: 0x04000901 RID: 2305
		public static ParticleType P_Ambience;

		// Token: 0x04000902 RID: 2306
		public static ParticleType P_Launch;

		// Token: 0x04000903 RID: 2307
		public static ParticleType P_FireAmbience;

		// Token: 0x04000904 RID: 2308
		public static ParticleType P_FireHit;

		// Token: 0x04000905 RID: 2309
		private const float RespawnTime = 0.6f;

		// Token: 0x04000906 RID: 2310
		private const float MoveCycleTime = 1.8181819f;

		// Token: 0x04000907 RID: 2311
		private const float SineCycleFreq = 0.44f;

		// Token: 0x04000908 RID: 2312
		private Sprite sprite;

		// Token: 0x04000909 RID: 2313
		private Sprite spriteEvil;

		// Token: 0x0400090A RID: 2314
		private VertexLight light;

		// Token: 0x0400090B RID: 2315
		private BloomPoint bloom;

		// Token: 0x0400090C RID: 2316
		private Vector2? node;

		// Token: 0x0400090D RID: 2317
		private bool goBack;

		// Token: 0x0400090E RID: 2318
		private Vector2 anchor;

		// Token: 0x0400090F RID: 2319
		private SineWave sine;

		// Token: 0x04000910 RID: 2320
		private float respawnTimer;

		// Token: 0x04000911 RID: 2321
		private bool fireMode;

		// Token: 0x04000912 RID: 2322
		private Wiggler hitWiggler;

		// Token: 0x04000913 RID: 2323
		private Vector2 hitDir;
	}
}
