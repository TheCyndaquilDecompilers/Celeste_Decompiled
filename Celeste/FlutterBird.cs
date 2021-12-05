using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000291 RID: 657
	[Tracked(false)]
	public class FlutterBird : Entity
	{
		// Token: 0x0600145E RID: 5214 RVA: 0x0006EB24 File Offset: 0x0006CD24
		public FlutterBird(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Depth = -9999;
			this.start = this.Position;
			base.Add(this.sprite = GFX.SpriteBank.Create("flutterbird"));
			this.sprite.Color = Calc.Random.Choose(FlutterBird.colors);
			base.Add(this.routine = new Coroutine(this.IdleRoutine(), true));
			base.Add(this.flyawaySfx = new SoundSource());
			base.Add(this.tweetingSfx = new SoundSource());
			this.tweetingSfx.Play("event:/game/general/birdbaby_tweet_loop", null, 0f);
		}

		// Token: 0x0600145F RID: 5215 RVA: 0x0006EBEC File Offset: 0x0006CDEC
		public override void Update()
		{
			this.sprite.Scale.X = Calc.Approach(this.sprite.Scale.X, (float)Math.Sign(this.sprite.Scale.X), 4f * Engine.DeltaTime);
			this.sprite.Scale.Y = Calc.Approach(this.sprite.Scale.Y, 1f, 4f * Engine.DeltaTime);
			base.Update();
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x0006EC7A File Offset: 0x0006CE7A
		private IEnumerator IdleRoutine()
		{
			for (;;)
			{
				Player player = base.Scene.Tracker.GetEntity<Player>();
				float delay = 0.25f + Calc.Random.NextFloat(1f);
				for (float p = 0f; p < delay; p += Engine.DeltaTime)
				{
					if (player != null && Math.Abs(player.X - base.X) < 48f && player.Y > base.Y - 40f && player.Y < base.Y + 8f)
					{
						this.FlyAway(Math.Sign(base.X - player.X), Calc.Random.NextFloat(0.2f));
					}
					yield return null;
				}
				Audio.Play("event:/game/general/birdbaby_hop", this.Position);
				Vector2 target = this.start + new Vector2(-4f + Calc.Random.NextFloat(8f), 0f);
				this.sprite.Scale.X = (float)Math.Sign(target.X - this.Position.X);
				SimpleCurve bezier = new SimpleCurve(this.Position, target, (this.Position + target) / 2f - Vector2.UnitY * 14f);
				for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
				{
					this.Position = bezier.GetPoint(p);
					yield return null;
				}
				this.sprite.Scale.X = (float)Math.Sign(this.sprite.Scale.X) * 1.4f;
				this.sprite.Scale.Y = 0.6f;
				this.Position = target;
				player = null;
				target = default(Vector2);
				bezier = default(SimpleCurve);
			}
			yield break;
		}

		// Token: 0x06001461 RID: 5217 RVA: 0x0006EC89 File Offset: 0x0006CE89
		private IEnumerator FlyAwayRoutine(int direction, float delay)
		{
			Level level = base.Scene as Level;
			yield return delay;
			this.sprite.Play("fly", false, false);
			this.sprite.Scale.X = (float)(-(float)direction) * 1.25f;
			this.sprite.Scale.Y = 1.25f;
			level.ParticlesFG.Emit(Calc.Random.Choose(new ParticleType[]
			{
				ParticleTypes.Dust
			}), this.Position, -1.5707964f);
			Vector2 from = this.Position;
			Vector2 to = this.Position + new Vector2((float)(direction * 4), -8f);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 3f)
			{
				this.Position = from + (to - from) * Ease.CubeOut(p);
				yield return null;
			}
			from = default(Vector2);
			to = default(Vector2);
			base.Depth = -10001;
			this.sprite.Scale.X = -this.sprite.Scale.X;
			to = new Vector2((float)direction, -4f) * 8f;
			while (base.Y + 8f > (float)level.Bounds.Top)
			{
				to += new Vector2((float)(direction * 64), -128f) * Engine.DeltaTime;
				this.Position += to * Engine.DeltaTime;
				if (base.Scene.OnInterval(0.1f) && base.Y > level.Camera.Top + 32f)
				{
					foreach (Entity entity in base.Scene.Tracker.GetEntities<FlutterBird>())
					{
						if (Math.Abs(base.X - entity.X) < 48f && Math.Abs(base.Y - entity.Y) < 48f && !(entity as FlutterBird).flyingAway)
						{
							(entity as FlutterBird).FlyAway(direction, Calc.Random.NextFloat(0.25f));
						}
					}
				}
				yield return null;
			}
			to = default(Vector2);
			base.Scene.Remove(this);
			yield break;
		}

		// Token: 0x06001462 RID: 5218 RVA: 0x0006ECA8 File Offset: 0x0006CEA8
		public void FlyAway(int direction, float delay)
		{
			if (!this.flyingAway)
			{
				this.tweetingSfx.Stop(true);
				this.flyingAway = true;
				this.flyawaySfx.Play("event:/game/general/birdbaby_flyaway", null, 0f);
				base.Remove(this.routine);
				base.Add(this.routine = new Coroutine(this.FlyAwayRoutine(direction, delay), true));
			}
		}

		// Token: 0x0400100D RID: 4109
		private static readonly Color[] colors = new Color[]
		{
			Calc.HexToColor("89fbff"),
			Calc.HexToColor("f0fc6c"),
			Calc.HexToColor("f493ff"),
			Calc.HexToColor("93baff")
		};

		// Token: 0x0400100E RID: 4110
		private Sprite sprite;

		// Token: 0x0400100F RID: 4111
		private Vector2 start;

		// Token: 0x04001010 RID: 4112
		private Coroutine routine;

		// Token: 0x04001011 RID: 4113
		private bool flyingAway;

		// Token: 0x04001012 RID: 4114
		private SoundSource tweetingSfx;

		// Token: 0x04001013 RID: 4115
		private SoundSource flyawaySfx;
	}
}
