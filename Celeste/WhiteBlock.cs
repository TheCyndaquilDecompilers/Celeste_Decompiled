using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200022B RID: 555
	public class WhiteBlock : JumpThru
	{
		// Token: 0x060011BD RID: 4541 RVA: 0x00058684 File Offset: 0x00056884
		public WhiteBlock(EntityData data, Vector2 offset) : base(data.Position + offset, 48, true)
		{
			base.Add(this.sprite = new Image(GFX.Game["objects/whiteblock"]));
			base.Depth = 8990;
			this.SurfaceSoundIndex = 27;
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x000586E3 File Offset: 0x000568E3
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if ((scene as Level).Session.HeartGem)
			{
				this.Disable();
			}
		}

		// Token: 0x060011BF RID: 4543 RVA: 0x00058704 File Offset: 0x00056904
		private void Disable()
		{
			this.enabled = false;
			this.sprite.Color = Color.White * 0.25f;
			this.Collidable = false;
		}

		// Token: 0x060011C0 RID: 4544 RVA: 0x00058730 File Offset: 0x00056930
		private void Activate(Player player)
		{
			Audio.Play("event:/game/04_cliffside/whiteblock_fallthru", base.Center);
			this.activated = true;
			this.Collidable = false;
			player.Depth = 10001;
			base.Depth = -9000;
			Level level = base.Scene as Level;
			Rectangle rectangle = new Rectangle(level.Bounds.Left / 8, level.Bounds.Y / 8, level.Bounds.Width / 8, level.Bounds.Height / 8);
			Rectangle tileBounds = level.Session.MapData.TileBounds;
			bool[,] array = new bool[rectangle.Width, rectangle.Height];
			for (int i = 0; i < rectangle.Width; i++)
			{
				for (int j = 0; j < rectangle.Height; j++)
				{
					array[i, j] = (level.BgData[i + rectangle.Left - tileBounds.Left, j + rectangle.Top - tileBounds.Top] != '0');
				}
			}
			this.bgSolidTiles = new Solid(new Vector2((float)level.Bounds.Left, (float)level.Bounds.Top), 1f, 1f, true);
			this.bgSolidTiles.Collider = new Grid(8f, 8f, array);
			base.Scene.Add(this.bgSolidTiles);
		}

		// Token: 0x060011C1 RID: 4545 RVA: 0x000588B4 File Offset: 0x00056AB4
		public override void Update()
		{
			base.Update();
			if (this.enabled)
			{
				if (!this.activated)
				{
					Player entity = base.Scene.Tracker.GetEntity<Player>();
					if (base.HasPlayerRider() && entity != null && entity.Ducking)
					{
						this.playerDuckTimer += Engine.DeltaTime;
						if (this.playerDuckTimer >= 3f)
						{
							this.Activate(entity);
						}
					}
					else
					{
						this.playerDuckTimer = 0f;
					}
					if ((base.Scene as Level).Session.HeartGem)
					{
						this.Disable();
						return;
					}
				}
				else if (base.Scene.Tracker.GetEntity<HeartGem>() == null)
				{
					Player entity2 = base.Scene.Tracker.GetEntity<Player>();
					if (entity2 != null)
					{
						this.Disable();
						entity2.Depth = 0;
						base.Scene.Remove(this.bgSolidTiles);
					}
				}
			}
		}

		// Token: 0x04000D63 RID: 3427
		private const float duckDuration = 3f;

		// Token: 0x04000D64 RID: 3428
		private float playerDuckTimer;

		// Token: 0x04000D65 RID: 3429
		private bool enabled = true;

		// Token: 0x04000D66 RID: 3430
		private bool activated;

		// Token: 0x04000D67 RID: 3431
		private Image sprite;

		// Token: 0x04000D68 RID: 3432
		private Entity bgSolidTiles;
	}
}
