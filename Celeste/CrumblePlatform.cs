using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200033D RID: 829
	public class CrumblePlatform : Solid
	{
		// Token: 0x06001A02 RID: 6658 RVA: 0x000A6D8D File Offset: 0x000A4F8D
		public CrumblePlatform(Vector2 position, float width) : base(position, width, 8f, false)
		{
			this.EnableAssistModeChecks = false;
		}

		// Token: 0x06001A03 RID: 6659 RVA: 0x000A6DA4 File Offset: 0x000A4FA4
		public CrumblePlatform(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width)
		{
		}

		// Token: 0x06001A04 RID: 6660 RVA: 0x000A6DC0 File Offset: 0x000A4FC0
		public override void Added(Scene scene)
		{
			base.Added(scene);
			MTexture mtexture = GFX.Game["objects/crumbleBlock/outline"];
			this.outline = new List<Image>();
			if (base.Width <= 8f)
			{
				Image image = new Image(mtexture.GetSubtexture(24, 0, 8, 8, null));
				image.Color = Color.White * 0f;
				base.Add(image);
				this.outline.Add(image);
			}
			else
			{
				int num = 0;
				while ((float)num < base.Width)
				{
					int num2;
					if (num == 0)
					{
						num2 = 0;
					}
					else if (num > 0 && (float)num < base.Width - 8f)
					{
						num2 = 1;
					}
					else
					{
						num2 = 2;
					}
					Image image2 = new Image(mtexture.GetSubtexture(num2 * 8, 0, 8, 8, null));
					image2.Position = new Vector2((float)num, 0f);
					image2.Color = Color.White * 0f;
					base.Add(image2);
					this.outline.Add(image2);
					num += 8;
				}
			}
			base.Add(this.outlineFader = new Coroutine(true));
			this.outlineFader.RemoveOnComplete = false;
			this.images = new List<Image>();
			this.falls = new List<Coroutine>();
			this.fallOrder = new List<int>();
			MTexture mtexture2 = GFX.Game["objects/crumbleBlock/" + AreaData.Get(scene).CrumbleBlock];
			int num3 = 0;
			while ((float)num3 < base.Width)
			{
				int num4 = (int)((Math.Abs(base.X) + (float)num3) / 8f) % 4;
				Image image3 = new Image(mtexture2.GetSubtexture(num4 * 8, 0, 8, 8, null));
				image3.Position = new Vector2((float)(4 + num3), 4f);
				image3.CenterOrigin();
				base.Add(image3);
				this.images.Add(image3);
				Coroutine coroutine = new Coroutine(true);
				coroutine.RemoveOnComplete = false;
				this.falls.Add(coroutine);
				base.Add(coroutine);
				this.fallOrder.Add(num3 / 8);
				num3 += 8;
			}
			this.fallOrder.Shuffle<int>();
			base.Add(new Coroutine(this.Sequence(), true));
			base.Add(this.shaker = new ShakerList(this.images.Count, false, delegate(Vector2[] v)
			{
				for (int i = 0; i < this.images.Count; i++)
				{
					this.images[i].Position = new Vector2((float)(4 + i * 8), 4f) + v[i];
				}
			}));
			base.Add(this.occluder = new LightOcclude(0.2f));
		}

		// Token: 0x06001A05 RID: 6661 RVA: 0x000A7041 File Offset: 0x000A5241
		private IEnumerator Sequence()
		{
			for (;;)
			{
				IL_39:
				bool onTop = false;
				while (base.GetPlayerOnTop() == null)
				{
					if (base.GetPlayerClimbing() != null)
					{
						onTop = false;
						Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
						IL_89:
						Audio.Play("event:/game/general/platform_disintegrate", base.Center);
						this.shaker.ShakeFor(onTop ? 0.6f : 1f, false);
						foreach (Image image in this.images)
						{
							base.SceneAs<Level>().Particles.Emit(CrumblePlatform.P_Crumble, 2, this.Position + image.Position + new Vector2(0f, 2f), Vector2.One * 3f);
						}
						int num;
						for (int i = 0; i < (onTop ? 1 : 3); i = num + 1)
						{
							yield return 0.2f;
							foreach (Image image2 in this.images)
							{
								base.SceneAs<Level>().Particles.Emit(CrumblePlatform.P_Crumble, 2, this.Position + image2.Position + new Vector2(0f, 2f), Vector2.One * 3f);
							}
							num = i;
						}
						float timer = 0.4f;
						if (onTop)
						{
							while (timer > 0f)
							{
								if (base.GetPlayerOnTop() == null)
								{
									break;
								}
								yield return null;
								timer -= Engine.DeltaTime;
							}
						}
						else
						{
							while (timer > 0f)
							{
								yield return null;
								timer -= Engine.DeltaTime;
							}
						}
						this.outlineFader.Replace(this.OutlineFade(1f));
						this.occluder.Visible = false;
						this.Collidable = false;
						float num2 = 0.05f;
						for (int j = 0; j < 4; j++)
						{
							for (int k = 0; k < this.images.Count; k++)
							{
								if (k % 4 - j == 0)
								{
									this.falls[k].Replace(this.TileOut(this.images[this.fallOrder[k]], num2 * (float)j));
								}
							}
						}
						yield return 2f;
						while (base.CollideCheck<Actor>() || base.CollideCheck<Solid>())
						{
							yield return null;
						}
						this.outlineFader.Replace(this.OutlineFade(0f));
						this.occluder.Visible = true;
						this.Collidable = true;
						for (int l = 0; l < 4; l++)
						{
							for (int m = 0; m < this.images.Count; m++)
							{
								if (m % 4 - l == 0)
								{
									this.falls[m].Replace(this.TileIn(m, this.images[this.fallOrder[m]], 0.05f * (float)l));
								}
							}
						}
						goto IL_39;
					}
					yield return null;
				}
				onTop = true;
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
				goto IL_89;
			}
			yield break;
		}

		// Token: 0x06001A06 RID: 6662 RVA: 0x000A7050 File Offset: 0x000A5250
		private IEnumerator OutlineFade(float to)
		{
			float from = 1f - to;
			for (float t = 0f; t < 1f; t += Engine.DeltaTime * 2f)
			{
				Color color = Color.White * (from + (to - from) * Ease.CubeInOut(t));
				foreach (Image image in this.outline)
				{
					image.Color = color;
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001A07 RID: 6663 RVA: 0x000A7066 File Offset: 0x000A5266
		private IEnumerator TileOut(Image img, float delay)
		{
			img.Color = Color.Gray;
			yield return delay;
			float distance = (img.X * 7f % 3f + 1f) * 12f;
			Vector2 from = img.Position;
			for (float time = 0f; time < 1f; time += Engine.DeltaTime / 0.4f)
			{
				yield return null;
				img.Position = from + Vector2.UnitY * Ease.CubeIn(time) * distance;
				img.Color = Color.Gray * (1f - time);
				img.Scale = Vector2.One * (1f - time * 0.5f);
			}
			img.Visible = false;
			yield break;
		}

		// Token: 0x06001A08 RID: 6664 RVA: 0x000A707C File Offset: 0x000A527C
		private IEnumerator TileIn(int index, Image img, float delay)
		{
			yield return delay;
			Audio.Play("event:/game/general/platform_return", base.Center);
			img.Visible = true;
			img.Color = Color.White;
			img.Position = new Vector2((float)(index * 8 + 4), 4f);
			for (float time = 0f; time < 1f; time += Engine.DeltaTime / 0.25f)
			{
				yield return null;
				img.Scale = Vector2.One * (1f + Ease.BounceOut(1f - time) * 0.2f);
			}
			img.Scale = Vector2.One;
			yield break;
		}

		// Token: 0x040016A8 RID: 5800
		public static ParticleType P_Crumble;

		// Token: 0x040016A9 RID: 5801
		private List<Image> images;

		// Token: 0x040016AA RID: 5802
		private List<Image> outline;

		// Token: 0x040016AB RID: 5803
		private List<Coroutine> falls;

		// Token: 0x040016AC RID: 5804
		private List<int> fallOrder;

		// Token: 0x040016AD RID: 5805
		private ShakerList shaker;

		// Token: 0x040016AE RID: 5806
		private LightOcclude occluder;

		// Token: 0x040016AF RID: 5807
		private Coroutine outlineFader;
	}
}
