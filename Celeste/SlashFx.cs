using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200038F RID: 911
	[Tracked(false)]
	[Pooled]
	public class SlashFx : Entity
	{
		// Token: 0x06001D8E RID: 7566 RVA: 0x000CD444 File Offset: 0x000CB644
		public SlashFx()
		{
			base.Add(this.Sprite = new Sprite(GFX.Game, "effects/slash/"));
			this.Sprite.Add("play", "", 0.1f, new int[]
			{
				0,
				1,
				2,
				3
			});
			this.Sprite.CenterOrigin();
			this.Sprite.OnFinish = delegate(string anim)
			{
				base.RemoveSelf();
			};
			base.Depth = -100;
		}

		// Token: 0x06001D8F RID: 7567 RVA: 0x000CD4CB File Offset: 0x000CB6CB
		public override void Update()
		{
			this.Position += this.Direction * 8f * Engine.DeltaTime;
			base.Update();
		}

		// Token: 0x06001D90 RID: 7568 RVA: 0x000CD500 File Offset: 0x000CB700
		public static SlashFx Burst(Vector2 position, float direction)
		{
			Scene scene = Engine.Scene;
			SlashFx slashFx = Engine.Pooler.Create<SlashFx>();
			scene.Add(slashFx);
			slashFx.Position = position;
			slashFx.Direction = Calc.AngleToVector(direction, 1f);
			slashFx.Sprite.Play("play", true, false);
			slashFx.Sprite.Scale = Vector2.One;
			slashFx.Sprite.Rotation = 0f;
			if (Math.Abs(direction - 3.1415927f) > 0.01f)
			{
				slashFx.Sprite.Rotation = direction;
			}
			slashFx.Visible = (slashFx.Active = true);
			return slashFx;
		}

		// Token: 0x04001E5B RID: 7771
		public Sprite Sprite;

		// Token: 0x04001E5C RID: 7772
		public Vector2 Direction;
	}
}
