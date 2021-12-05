using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000342 RID: 834
	public class IntroCar : JumpThru
	{
		// Token: 0x06001A50 RID: 6736 RVA: 0x000A9218 File Offset: 0x000A7418
		public IntroCar(Vector2 position) : base(position, 25, true)
		{
			this.startY = position.Y;
			base.Depth = 1;
			base.Add(this.bodySprite = new Image(GFX.Game["scenery/car/body"]));
			this.bodySprite.Origin = new Vector2(this.bodySprite.Width / 2f, this.bodySprite.Height);
			Hitbox hitbox = new Hitbox(25f, 4f, -15f, -17f);
			Hitbox hitbox2 = new Hitbox(19f, 4f, 8f, -11f);
			base.Collider = new ColliderList(new Collider[]
			{
				hitbox,
				hitbox2
			});
			this.SurfaceSoundIndex = 2;
		}

		// Token: 0x06001A51 RID: 6737 RVA: 0x000A92E5 File Offset: 0x000A74E5
		public IntroCar(EntityData data, Vector2 offset) : this(data.Position + offset)
		{
		}

		// Token: 0x06001A52 RID: 6738 RVA: 0x000A92FC File Offset: 0x000A74FC
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Image image = new Image(GFX.Game["scenery/car/wheels"]);
			image.Origin = new Vector2(image.Width / 2f, image.Height);
			this.wheels = new Entity(this.Position);
			this.wheels.Add(image);
			this.wheels.Depth = 3;
			scene.Add(this.wheels);
			Level level = scene as Level;
			if (level.Session.Area.ID == 0)
			{
				level.Add(new IntroPavement(new Vector2((float)level.Bounds.Left, base.Y), (int)(base.X - (float)level.Bounds.Left - 48f))
				{
					Depth = -10001
				});
				level.Add(new IntroCarBarrier(this.Position + new Vector2(32f, 0f), -10, Color.White));
				level.Add(new IntroCarBarrier(this.Position + new Vector2(41f, 0f), 5, Color.DarkGray));
			}
		}

		// Token: 0x06001A53 RID: 6739 RVA: 0x000A943C File Offset: 0x000A763C
		public override void Update()
		{
			bool flag = base.HasRider();
			if (base.Y > this.startY && (!flag || base.Y > this.startY + 1f))
			{
				float moveV = -10f * Engine.DeltaTime;
				base.MoveV(moveV);
			}
			if (base.Y <= this.startY && !this.didHaveRider && flag)
			{
				base.MoveV(2f);
			}
			if (this.didHaveRider && !flag)
			{
				Audio.Play("event:/game/00_prologue/car_up", this.Position);
			}
			this.didHaveRider = flag;
			base.Update();
		}

		// Token: 0x06001A54 RID: 6740 RVA: 0x000A94DA File Offset: 0x000A76DA
		public override int GetLandSoundIndex(Entity entity)
		{
			Audio.Play("event:/game/00_prologue/car_down", this.Position);
			return -1;
		}

		// Token: 0x040016EA RID: 5866
		private Image bodySprite;

		// Token: 0x040016EB RID: 5867
		private Entity wheels;

		// Token: 0x040016EC RID: 5868
		private float startY;

		// Token: 0x040016ED RID: 5869
		private bool didHaveRider;
	}
}
