using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002B6 RID: 694
	public class ClutterAbsorbEffect : Entity
	{
		// Token: 0x0600156D RID: 5485 RVA: 0x0007B4E4 File Offset: 0x000796E4
		public ClutterAbsorbEffect()
		{
			this.Position = Vector2.Zero;
			base.Tag = Tags.TransitionUpdate;
			base.Depth = -10001;
		}

		// Token: 0x0600156E RID: 5486 RVA: 0x0007B520 File Offset: 0x00079720
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
			foreach (Entity entity in this.level.Tracker.GetEntities<ClutterCabinet>())
			{
				this.cabinets.Add(entity as ClutterCabinet);
			}
		}

		// Token: 0x0600156F RID: 5487 RVA: 0x0007B59C File Offset: 0x0007979C
		public void FlyClutter(Vector2 position, MTexture texture, bool shake, float delay)
		{
			Image image = new Image(texture);
			image.Position = position - this.Position;
			image.CenterOrigin();
			base.Add(image);
			base.Add(new Coroutine(this.FlyClutterRoutine(image, shake, delay), true)
			{
				RemoveOnComplete = true
			});
		}

		// Token: 0x06001570 RID: 5488 RVA: 0x0007B5EF File Offset: 0x000797EF
		private IEnumerator FlyClutterRoutine(Image img, bool shake, float delay)
		{
			yield return delay;
			ClutterCabinet cabinet = Calc.Random.Choose(this.cabinets);
			Vector2 value = cabinet.Position + new Vector2(8f);
			Vector2 from = img.Position;
			Vector2 vector = value + new Vector2((float)(Calc.Random.Next(16) - 8), (float)(Calc.Random.Next(4) - 2));
			Vector2 vector2 = (vector - from).SafeNormalize();
			float num = (vector - from).Length();
			Vector2 value2 = new Vector2(-vector2.Y, vector2.X) * (num / 4f + Calc.Random.NextFloat(40f)) * (float)(Calc.Random.Chance(0.5f) ? -1 : 1);
			SimpleCurve curve = new SimpleCurve(from, vector, (vector + from) / 2f + value2);
			if (shake)
			{
				for (float time = 0.25f; time > 0f; time -= Engine.DeltaTime)
				{
					img.X = from.X + (float)Calc.Random.Next(3) - 1f;
					img.Y = from.Y + (float)Calc.Random.Next(3) - 1f;
					yield return null;
				}
			}
			for (float time = 0f; time < 1f; time += Engine.DeltaTime)
			{
				img.Position = curve.GetPoint(Ease.CubeInOut(time));
				img.Scale = Vector2.One * Ease.CubeInOut(1f - time * 0.5f);
				if (time > 0.5f && !cabinet.Opened)
				{
					cabinet.Open();
				}
				if (this.level.OnInterval(0.25f))
				{
					this.level.ParticlesFG.Emit(ClutterSwitch.P_ClutterFly, img.Position);
				}
				yield return null;
			}
			base.Remove(img);
			yield break;
		}

		// Token: 0x06001571 RID: 5489 RVA: 0x0007B613 File Offset: 0x00079813
		public void CloseCabinets()
		{
			base.Add(new Coroutine(this.CloseCabinetsRoutine(), true));
		}

		// Token: 0x06001572 RID: 5490 RVA: 0x0007B627 File Offset: 0x00079827
		private IEnumerator CloseCabinetsRoutine()
		{
			this.cabinets.Sort(delegate(ClutterCabinet a, ClutterCabinet b)
			{
				if (Math.Abs(a.Y - b.Y) < 24f)
				{
					return Math.Sign(a.X - b.X);
				}
				return Math.Sign(a.Y - b.Y);
			});
			int i = 0;
			foreach (ClutterCabinet clutterCabinet in this.cabinets)
			{
				clutterCabinet.Close();
				int num = i;
				i = num + 1;
				if (num % 3 == 0)
				{
					yield return 0.1f;
				}
			}
			List<ClutterCabinet>.Enumerator enumerator = default(List<ClutterCabinet>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x04001187 RID: 4487
		private Level level;

		// Token: 0x04001188 RID: 4488
		private List<ClutterCabinet> cabinets = new List<ClutterCabinet>();
	}
}
