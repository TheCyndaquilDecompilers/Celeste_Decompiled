using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001A2 RID: 418
	public class NPC06_Badeline_Crying : NPC
	{
		// Token: 0x06000EA3 RID: 3747 RVA: 0x00036EF4 File Offset: 0x000350F4
		public NPC06_Badeline_Crying(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("badeline_boss"));
			this.Sprite.Play("scaredIdle", false, false);
			base.Add(this.white = new Image(GFX.Game["characters/badelineBoss/calm_white"]));
			this.white.Color = Color.White * 0f;
			this.white.Origin = this.Sprite.Origin;
			this.white.Position = this.Sprite.Position;
			base.Add(this.bloom = new BloomPoint(new Vector2(0f, -6f), 0f, 16f));
			base.Add(this.light = new VertexLight(new Vector2(0f, -6f), Color.White, 1f, 24, 64));
			base.Add(this.LoopingSfx = new SoundSource("event:/char/badeline/boss_idle_ground"));
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x00037030 File Offset: 0x00035230
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (base.Session.GetFlag("badeline_connection"))
			{
				FinalBossStarfield finalBossStarfield = (scene as Level).Background.Get<FinalBossStarfield>();
				if (finalBossStarfield != null)
				{
					finalBossStarfield.Alpha = 0f;
				}
				foreach (Entity entity in base.Scene.Tracker.GetEntities<ReflectionTentacles>())
				{
					entity.RemoveSelf();
				}
				base.RemoveSelf();
			}
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x000370C8 File Offset: 0x000352C8
		public override void Update()
		{
			base.Update();
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (!this.started && entity != null && entity.X > base.X - 32f)
			{
				base.Scene.Add(new CS06_BossEnd(entity, this));
				this.started = true;
			}
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x00037124 File Offset: 0x00035324
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			foreach (NPC06_Badeline_Crying.Orb orb in this.orbs)
			{
				orb.RemoveSelf();
			}
		}

		// Token: 0x06000EA7 RID: 3751 RVA: 0x0003717C File Offset: 0x0003537C
		public IEnumerator TurnWhite(float duration)
		{
			float alpha = 0f;
			while (alpha < 1f)
			{
				alpha += Engine.DeltaTime / duration;
				this.white.Color = Color.White * alpha;
				this.bloom.Alpha = alpha;
				yield return null;
			}
			this.Sprite.Visible = false;
			yield break;
		}

		// Token: 0x06000EA8 RID: 3752 RVA: 0x00037192 File Offset: 0x00035392
		public IEnumerator Disperse()
		{
			Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
			float size = 1f;
			while (this.orbs.Count < 8)
			{
				float to = size - 0.125f;
				while (size > to)
				{
					this.white.Scale = Vector2.One * size;
					this.light.Alpha = size;
					this.bloom.Alpha = size;
					size -= Engine.DeltaTime;
					yield return null;
				}
				NPC06_Badeline_Crying.Orb orb = new NPC06_Badeline_Crying.Orb(this.Position);
				orb.Target = this.Position + new Vector2(-16f, -40f);
				base.Scene.Add(orb);
				this.orbs.Add(orb);
			}
			yield return 3.25f;
			int i = 0;
			foreach (NPC06_Badeline_Crying.Orb orb2 in this.orbs)
			{
				orb2.Routine.Replace(orb2.CircleRoutine((float)i / 8f * 6.2831855f));
				int num = i;
				i = num + 1;
				yield return 0.2f;
			}
			List<NPC06_Badeline_Crying.Orb>.Enumerator enumerator = default(List<NPC06_Badeline_Crying.Orb>.Enumerator);
			yield return 2f;
			foreach (NPC06_Badeline_Crying.Orb orb3 in this.orbs)
			{
				orb3.Routine.Replace(orb3.AbsorbRoutine());
			}
			yield return 1f;
			yield break;
			yield break;
		}

		// Token: 0x040009DD RID: 2525
		private bool started;

		// Token: 0x040009DE RID: 2526
		private Image white;

		// Token: 0x040009DF RID: 2527
		private BloomPoint bloom;

		// Token: 0x040009E0 RID: 2528
		private VertexLight light;

		// Token: 0x040009E1 RID: 2529
		public SoundSource LoopingSfx;

		// Token: 0x040009E2 RID: 2530
		private List<NPC06_Badeline_Crying.Orb> orbs = new List<NPC06_Badeline_Crying.Orb>();

		// Token: 0x020004B3 RID: 1203
		private class Orb : Entity
		{
			// Token: 0x170003EB RID: 1003
			// (get) Token: 0x060023B3 RID: 9139 RVA: 0x000EF55A File Offset: 0x000ED75A
			// (set) Token: 0x060023B4 RID: 9140 RVA: 0x000EF562 File Offset: 0x000ED762
			public float Ease
			{
				get
				{
					return this.ease;
				}
				set
				{
					this.ease = value;
					this.Sprite.Scale = Vector2.One * this.ease;
					this.Bloom.Alpha = this.ease;
				}
			}

			// Token: 0x060023B5 RID: 9141 RVA: 0x000EF598 File Offset: 0x000ED798
			public Orb(Vector2 position) : base(position)
			{
				base.Add(this.Sprite = new Image(GFX.Game["characters/badeline/orb"]));
				base.Add(this.Bloom = new BloomPoint(0f, 32f));
				base.Add(this.Routine = new Coroutine(this.FloatRoutine(), true));
				this.Sprite.CenterOrigin();
				base.Depth = -10001;
			}

			// Token: 0x060023B6 RID: 9142 RVA: 0x000EF61F File Offset: 0x000ED81F
			public IEnumerator FloatRoutine()
			{
				Vector2 speed = Vector2.Zero;
				this.Ease = 0.2f;
				for (;;)
				{
					Vector2 target = this.Target + Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 16f + Calc.Random.NextFloat(40f));
					float reset = 0f;
					while (reset < 1f && (target - this.Position).Length() > 8f)
					{
						Vector2 value = (target - this.Position).SafeNormalize();
						speed += value * 420f * Engine.DeltaTime;
						if (speed.Length() > 90f)
						{
							speed = speed.SafeNormalize(90f);
						}
						this.Position += speed * Engine.DeltaTime;
						reset += Engine.DeltaTime;
						this.Ease = Calc.Approach(this.Ease, 1f, Engine.DeltaTime * 4f);
						yield return null;
					}
					target = default(Vector2);
				}
				yield break;
			}

			// Token: 0x060023B7 RID: 9143 RVA: 0x000EF62E File Offset: 0x000ED82E
			public IEnumerator CircleRoutine(float offset)
			{
				Vector2 from = this.Position;
				float ease = 0f;
				Player player = base.Scene.Tracker.GetEntity<Player>();
				while (player != null)
				{
					float angleRadians = base.Scene.TimeActive * 2f + offset;
					Vector2 value = player.Center + Calc.AngleToVector(angleRadians, 24f);
					ease = Calc.Approach(ease, 1f, Engine.DeltaTime * 2f);
					this.Position = from + (value - from) * Monocle.Ease.CubeInOut(ease);
					yield return null;
				}
				yield break;
			}

			// Token: 0x060023B8 RID: 9144 RVA: 0x000EF644 File Offset: 0x000ED844
			public IEnumerator AbsorbRoutine()
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity == null)
				{
					yield break;
				}
				Vector2 from = this.Position;
				Vector2 to = entity.Center;
				for (float p = 0f; p < 1f; p += Engine.DeltaTime)
				{
					float num = Monocle.Ease.BigBackIn(p);
					this.Position = from + (to - from) * num;
					this.Ease = 0.2f + (1f - num) * 0.8f;
					yield return null;
				}
				yield break;
			}

			// Token: 0x0400232D RID: 9005
			public Image Sprite;

			// Token: 0x0400232E RID: 9006
			public BloomPoint Bloom;

			// Token: 0x0400232F RID: 9007
			private float ease;

			// Token: 0x04002330 RID: 9008
			public Vector2 Target;

			// Token: 0x04002331 RID: 9009
			public Coroutine Routine;
		}
	}
}
