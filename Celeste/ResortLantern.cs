using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002C6 RID: 710
	public class ResortLantern : Entity
	{
		// Token: 0x06001600 RID: 5632 RVA: 0x000801BC File Offset: 0x0007E3BC
		public ResortLantern(Vector2 position) : base(position)
		{
			base.Collider = new Hitbox(8f, 8f, -4f, -4f);
			base.Depth = 2000;
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			this.holder = new Image(GFX.Game["objects/resortLantern/holder"]);
			this.holder.CenterOrigin();
			base.Add(this.holder);
			this.lantern = new Sprite(GFX.Game, "objects/resortLantern/");
			this.lantern.AddLoop("light", "lantern", 0.3f, new int[]
			{
				0,
				0,
				1,
				2,
				1
			});
			this.lantern.Play("light", false, false);
			this.lantern.Origin = new Vector2(7f, 7f);
			this.lantern.Position = new Vector2(-1f, -5f);
			base.Add(this.lantern);
			this.wiggler = Wiggler.Create(2.5f, 1.2f, delegate(float v)
			{
				this.lantern.Rotation = v * (float)this.mult * 0.017453292f * 30f;
			}, false, false);
			this.wiggler.StartZero = true;
			base.Add(this.wiggler);
			base.Add(this.light = new VertexLight(Color.White, 0.95f, 32, 64));
			base.Add(this.bloom = new BloomPoint(0.8f, 8f));
			base.Add(this.sfx = new SoundSource());
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x00080366 File Offset: 0x0007E566
		public ResortLantern(EntityData data, Vector2 offset) : this(data.Position + offset)
		{
		}

		// Token: 0x06001602 RID: 5634 RVA: 0x0008037C File Offset: 0x0007E57C
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (base.CollideCheck<Solid>(this.Position + Vector2.UnitX * 8f))
			{
				this.holder.Scale.X = -1f;
				this.lantern.Scale.X = -1f;
				this.lantern.X += 2f;
			}
		}

		// Token: 0x06001603 RID: 5635 RVA: 0x000803F4 File Offset: 0x0007E5F4
		public override void Update()
		{
			base.Update();
			if (this.collideTimer > 0f)
			{
				this.collideTimer -= Engine.DeltaTime;
			}
			this.alphaTimer += Engine.DeltaTime;
			this.bloom.Alpha = (this.light.Alpha = 0.95f + (float)Math.Sin((double)(this.alphaTimer * 1f)) * 0.05f);
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x00080470 File Offset: 0x0007E670
		private void OnPlayer(Player player)
		{
			if (this.collideTimer <= 0f)
			{
				if (player.Speed != Vector2.Zero)
				{
					this.sfx.Play("event:/game/03_resort/lantern_bump", null, 0f);
					this.collideTimer = 0.5f;
					this.mult = Calc.Random.Choose(1, -1);
					this.wiggler.Start();
					return;
				}
			}
			else
			{
				this.collideTimer = 0.5f;
			}
		}

		// Token: 0x04001248 RID: 4680
		private Image holder;

		// Token: 0x04001249 RID: 4681
		private Sprite lantern;

		// Token: 0x0400124A RID: 4682
		private float collideTimer;

		// Token: 0x0400124B RID: 4683
		private int mult;

		// Token: 0x0400124C RID: 4684
		private Wiggler wiggler;

		// Token: 0x0400124D RID: 4685
		private VertexLight light;

		// Token: 0x0400124E RID: 4686
		private BloomPoint bloom;

		// Token: 0x0400124F RID: 4687
		private float alphaTimer;

		// Token: 0x04001250 RID: 4688
		private SoundSource sfx;
	}
}
