using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002D2 RID: 722
	public class ResortRoofEnding : Solid
	{
		// Token: 0x0600164E RID: 5710 RVA: 0x000831CC File Offset: 0x000813CC
		public ResortRoofEnding(EntityData data, Vector2 offset) : base(data.Position + offset, (float)data.Width, 2f, true)
		{
			this.EnableAssistModeChecks = false;
			Image image = new Image(GFX.Game["decals/3-resort/roofEdge_d"]);
			image.CenterOrigin();
			image.X = 8f;
			image.Y = 4f;
			base.Add(image);
			int num = 0;
			while ((float)num < base.Width)
			{
				Image image2 = new Image(Calc.Random.Choose(this.roofCenters));
				image2.CenterOrigin();
				image2.X = (float)(num + 8);
				image2.Y = 4f;
				base.Add(image2);
				this.images.Add(image2);
				num += 16;
			}
			Image image3 = new Image(GFX.Game["decals/3-resort/roofEdge"]);
			image3.CenterOrigin();
			image3.X = (float)(num + 8);
			image3.Y = 4f;
			base.Add(image3);
			this.images.Add(image3);
		}

		// Token: 0x0600164F RID: 5711 RVA: 0x00083340 File Offset: 0x00081540
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (!(base.Scene as Level).Session.GetFlag("oshiroEnding"))
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					base.Scene.Add(new CS03_Ending(this, entity));
				}
			}
		}

		// Token: 0x06001650 RID: 5712 RVA: 0x00083396 File Offset: 0x00081596
		public override void Render()
		{
			this.Position += base.Shake;
			base.Render();
			this.Position -= base.Shake;
		}

		// Token: 0x06001651 RID: 5713 RVA: 0x000833CC File Offset: 0x000815CC
		public void Wobble(AngryOshiro ghost, bool fall = false)
		{
			foreach (Coroutine coroutine in this.wobbleRoutines)
			{
				coroutine.RemoveSelf();
			}
			this.wobbleRoutines.Clear();
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			foreach (Image image in this.images)
			{
				Coroutine coroutine2 = new Coroutine(this.WobbleImage(image, Math.Abs(base.X + image.X - ghost.X) * 0.001f, entity, fall), true);
				base.Add(coroutine2);
				this.wobbleRoutines.Add(coroutine2);
			}
		}

		// Token: 0x06001652 RID: 5714 RVA: 0x000834B8 File Offset: 0x000816B8
		private IEnumerator WobbleImage(Image img, float delay, Player player, bool fall)
		{
			float orig = img.Y;
			yield return delay;
			for (int i = 0; i < 2; i++)
			{
				base.Scene.Add(Engine.Pooler.Create<Debris>().Init(this.Position + img.Position + new Vector2((float)(-4 + i * 8), (float)Calc.Random.Range(0, 8)), '9', true));
			}
			float amount;
			float p;
			if (fall)
			{
				if (fall)
				{
					while (!this.BeginFalling)
					{
						int num = Calc.Random.Range(0, 2);
						img.Y = orig + (float)num;
						if (player != null && Math.Abs(base.X + img.X - player.X) < 16f)
						{
							player.Sprite.Y = (float)num;
						}
						yield return 0.01f;
					}
					img.Texture = GFX.Game["decals/3-resort/roofCenter_snapped_" + Calc.Random.Choose("a", "b", "c")];
					this.Collidable = false;
					amount = Calc.Random.NextFloat();
					p = -24f + Calc.Random.NextFloat(48f);
					float speedY = -(80f + Calc.Random.NextFloat(80f));
					float up = new Vector2(0f, -1f).Angle();
					float off = Calc.Random.NextFloat();
					for (float p2 = 0f; p2 < 4f; p2 += Engine.DeltaTime)
					{
						img.Position += new Vector2(p, speedY) * Engine.DeltaTime;
						img.Rotation += amount * Ease.CubeIn(p2);
						p = Calc.Approach(p, 0f, Engine.DeltaTime * 200f);
						speedY += 600f * Engine.DeltaTime;
						if (base.Scene.OnInterval(0.1f, off))
						{
							Dust.Burst(this.Position + img.Position, up, 1, null);
						}
						yield return null;
					}
				}
				player.Sprite.Y = 0f;
				yield break;
			}
			p = 0f;
			amount = 5f;
			for (;;)
			{
				p += Engine.DeltaTime * 16f;
				amount = Calc.Approach(amount, 1f, Engine.DeltaTime * 5f);
				float num2 = (float)Math.Sin((double)p) * amount;
				img.Y = orig + num2;
				if (player != null && Math.Abs(base.X + img.X - player.X) < 16f)
				{
					player.Sprite.Y = num2;
				}
				yield return null;
			}
		}

		// Token: 0x040012A7 RID: 4775
		private MTexture[] roofCenters = new MTexture[]
		{
			GFX.Game["decals/3-resort/roofCenter"],
			GFX.Game["decals/3-resort/roofCenter_b"],
			GFX.Game["decals/3-resort/roofCenter_c"],
			GFX.Game["decals/3-resort/roofCenter_d"]
		};

		// Token: 0x040012A8 RID: 4776
		private List<Image> images = new List<Image>();

		// Token: 0x040012A9 RID: 4777
		private List<Coroutine> wobbleRoutines = new List<Coroutine>();

		// Token: 0x040012AA RID: 4778
		public bool BeginFalling;
	}
}
