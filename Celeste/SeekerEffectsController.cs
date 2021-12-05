using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000228 RID: 552
	public class SeekerEffectsController : Entity
	{
		// Token: 0x060011AB RID: 4523 RVA: 0x00057A57 File Offset: 0x00055C57
		public SeekerEffectsController()
		{
			base.Tag = Tags.Global;
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x00057A76 File Offset: 0x00055C76
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Level level = scene as Level;
			level.Session.Audio.Music.Layer(3, 0f);
			level.Session.Audio.Apply(false);
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x00057AB4 File Offset: 0x00055CB4
		public override void Update()
		{
			base.Update();
			if (this.enabled)
			{
				if (base.Scene.OnInterval(0.05f))
				{
					this.randomAnxietyOffset = Calc.Random.Range(-0.2f, 0.2f);
				}
				Vector2 position = (base.Scene as Level).Camera.Position;
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				float target;
				float num4;
				if (entity != null && !entity.Dead)
				{
					float num = -1f;
					float num2 = -1f;
					foreach (Entity entity2 in base.Scene.Tracker.GetEntities<Seeker>())
					{
						Seeker seeker = (Seeker)entity2;
						float num3 = Vector2.DistanceSquared(entity.Center, seeker.Center);
						if (!seeker.Regenerating)
						{
							if (num < 0f)
							{
								num = num3;
							}
							else
							{
								num = Math.Min(num, num3);
							}
						}
						if (seeker.Attacking)
						{
							if (num2 < 0f)
							{
								num2 = num3;
							}
							else
							{
								num2 = Math.Min(num2, num3);
							}
						}
					}
					if (num2 >= 0f)
					{
						target = Calc.ClampedMap(num2, 256f, 4096f, 0.5f, 1f);
					}
					else
					{
						target = 1f;
					}
					Distort.AnxietyOrigin = new Vector2((entity.Center.X - position.X) / 320f, (entity.Center.Y - position.Y) / 180f);
					if (num >= 0f)
					{
						num4 = Calc.ClampedMap(num, 256f, 16384f, 1f, 0f);
					}
					else
					{
						num4 = 0f;
					}
				}
				else
				{
					target = 1f;
					num4 = 0f;
				}
				Engine.TimeRate = Calc.Approach(Engine.TimeRate, target, 4f * Engine.DeltaTime);
				Distort.GameRate = Calc.Approach(Distort.GameRate, Calc.Map(Engine.TimeRate, 0.5f, 1f, 0f, 1f), Engine.DeltaTime * 2f);
				Distort.Anxiety = Calc.Approach(Distort.Anxiety, (0.5f + this.randomAnxietyOffset) * num4, 8f * Engine.DeltaTime);
				if (Engine.TimeRate == 1f && Distort.GameRate == 1f && Distort.Anxiety == 0f && base.Scene.Tracker.CountEntities<Seeker>() == 0)
				{
					this.enabled = false;
					return;
				}
			}
			else if (base.Scene.Tracker.CountEntities<Seeker>() > 0)
			{
				this.enabled = true;
			}
		}

		// Token: 0x04000D42 RID: 3394
		private float randomAnxietyOffset;

		// Token: 0x04000D43 RID: 3395
		public bool enabled = true;
	}
}
