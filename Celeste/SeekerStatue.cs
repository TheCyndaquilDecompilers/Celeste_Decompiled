using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000199 RID: 409
	public class SeekerStatue : Entity
	{
		// Token: 0x06000E36 RID: 3638 RVA: 0x000330EC File Offset: 0x000312EC
		public SeekerStatue(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			SeekerStatue <>4__this = this;
			base.Depth = 8999;
			base.Add(this.sprite = GFX.SpriteBank.Create("seeker"));
			this.sprite.Play("statue", false, false);
			this.sprite.OnLastFrame = delegate(string f)
			{
				if (f == "hatch")
				{
					Seeker seeker = new Seeker(data, offset);
					seeker.Light.Alpha = 0f;
					<>4__this.Scene.Add(seeker);
					<>4__this.RemoveSelf();
				}
			};
			this.hatch = data.Enum<SeekerStatue.Hatch>("hatch", SeekerStatue.Hatch.Distance);
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x0003319C File Offset: 0x0003139C
		public override void Update()
		{
			base.Update();
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null && this.sprite.CurrentAnimationID == "statue")
			{
				bool flag = false;
				if (this.hatch == SeekerStatue.Hatch.Distance && (entity.Position - this.Position).Length() < 220f)
				{
					flag = true;
				}
				else if (this.hatch == SeekerStatue.Hatch.PlayerRightOfX && entity.X > base.X + 32f)
				{
					flag = true;
				}
				if (flag)
				{
					this.BreakOutParticles();
					this.sprite.Play("hatch", false, false);
					Audio.Play("event:/game/05_mirror_temple/seeker_statue_break", this.Position);
					Alarm.Set(this, 0.8f, new Action(this.BreakOutParticles), Alarm.AlarmMode.Oneshot);
				}
			}
		}

		// Token: 0x06000E38 RID: 3640 RVA: 0x00033274 File Offset: 0x00031474
		private void BreakOutParticles()
		{
			Level level = base.SceneAs<Level>();
			for (float num = 0f; num < 6.2831855f; num += 0.17453292f)
			{
				Vector2 position = base.Center + Calc.AngleToVector(num + Calc.Random.Range(-0.034906585f, 0.034906585f), (float)Calc.Random.Range(12, 20));
				level.Particles.Emit(Seeker.P_BreakOut, position, num);
			}
		}

		// Token: 0x0400096B RID: 2411
		private SeekerStatue.Hatch hatch;

		// Token: 0x0400096C RID: 2412
		private Sprite sprite;

		// Token: 0x02000497 RID: 1175
		private enum Hatch
		{
			// Token: 0x040022CD RID: 8909
			Distance,
			// Token: 0x040022CE RID: 8910
			PlayerRightOfX
		}
	}
}
