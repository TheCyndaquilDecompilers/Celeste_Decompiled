using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000357 RID: 855
	public class ZipMover : Solid
	{
		// Token: 0x06001AE4 RID: 6884 RVA: 0x000AE694 File Offset: 0x000AC894
		public ZipMover(Vector2 position, int width, int height, Vector2 target, ZipMover.Themes theme) : base(position, (float)width, (float)height, false)
		{
			base.Depth = -9999;
			this.start = this.Position;
			this.target = target;
			this.theme = theme;
			base.Add(new Coroutine(this.Sequence(), true));
			base.Add(new LightOcclude(1f));
			string path;
			string id;
			string key;
			if (theme == ZipMover.Themes.Moon)
			{
				path = "objects/zipmover/moon/light";
				id = "objects/zipmover/moon/block";
				key = "objects/zipmover/moon/innercog";
				this.drawBlackBorder = false;
			}
			else
			{
				path = "objects/zipmover/light";
				id = "objects/zipmover/block";
				key = "objects/zipmover/innercog";
				this.drawBlackBorder = true;
			}
			this.innerCogs = GFX.Game.GetAtlasSubtextures(key);
			base.Add(this.streetlight = new Sprite(GFX.Game, path));
			this.streetlight.Add("frames", "", 1f);
			this.streetlight.Play("frames", false, false);
			this.streetlight.Active = false;
			this.streetlight.SetAnimationFrame(1);
			this.streetlight.Position = new Vector2(base.Width / 2f - this.streetlight.Width / 2f, 0f);
			base.Add(this.bloom = new BloomPoint(1f, 6f));
			this.bloom.Position = new Vector2(base.Width / 2f, 4f);
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					this.edges[i, j] = GFX.Game[id].GetSubtexture(i * 8, j * 8, 8, 8, null);
				}
			}
			this.SurfaceSoundIndex = 7;
			this.sfx.Position = new Vector2(base.Width, base.Height) / 2f;
			base.Add(this.sfx);
		}

		// Token: 0x06001AE5 RID: 6885 RVA: 0x000AE8B8 File Offset: 0x000ACAB8
		public ZipMover(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height, data.Nodes[0] + offset, data.Enum<ZipMover.Themes>("theme", ZipMover.Themes.Normal))
		{
		}

		// Token: 0x06001AE6 RID: 6886 RVA: 0x000AE8F8 File Offset: 0x000ACAF8
		public override void Added(Scene scene)
		{
			base.Added(scene);
			scene.Add(this.pathRenderer = new ZipMover.ZipMoverPathRenderer(this));
		}

		// Token: 0x06001AE7 RID: 6887 RVA: 0x000AE921 File Offset: 0x000ACB21
		public override void Removed(Scene scene)
		{
			scene.Remove(this.pathRenderer);
			this.pathRenderer = null;
			base.Removed(scene);
		}

		// Token: 0x06001AE8 RID: 6888 RVA: 0x000AE93D File Offset: 0x000ACB3D
		public override void Update()
		{
			base.Update();
			this.bloom.Y = (float)(this.streetlight.CurrentAnimationFrame * 3);
		}

		// Token: 0x06001AE9 RID: 6889 RVA: 0x000AE960 File Offset: 0x000ACB60
		public override void Render()
		{
			Vector2 position = this.Position;
			this.Position += base.Shake;
			Draw.Rect(base.X + 1f, base.Y + 1f, base.Width - 2f, base.Height - 2f, Color.Black);
			int num = 1;
			float num2 = 0f;
			int count = this.innerCogs.Count;
			int num3 = 4;
			while ((float)num3 <= base.Height - 4f)
			{
				int num4 = num;
				int num5 = 4;
				while ((float)num5 <= base.Width - 4f)
				{
					int index = (int)(this.mod((num2 + (float)num * this.percent * 3.1415927f * 4f) / 1.5707964f, 1f) * (float)count);
					MTexture mtexture = this.innerCogs[index];
					Rectangle rectangle = new Rectangle(0, 0, mtexture.Width, mtexture.Height);
					Vector2 zero = Vector2.Zero;
					if (num5 <= 4)
					{
						zero.X = 2f;
						rectangle.X = 2;
						rectangle.Width -= 2;
					}
					else if ((float)num5 >= base.Width - 4f)
					{
						zero.X = -2f;
						rectangle.Width -= 2;
					}
					if (num3 <= 4)
					{
						zero.Y = 2f;
						rectangle.Y = 2;
						rectangle.Height -= 2;
					}
					else if ((float)num3 >= base.Height - 4f)
					{
						zero.Y = -2f;
						rectangle.Height -= 2;
					}
					mtexture = mtexture.GetSubtexture(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, this.temp);
					mtexture.DrawCentered(this.Position + new Vector2((float)num5, (float)num3) + zero, Color.White * ((num < 0) ? 0.5f : 1f));
					num = -num;
					num2 += 1.0471976f;
					num5 += 8;
				}
				if (num4 == num)
				{
					num = -num;
				}
				num3 += 8;
			}
			int num6 = 0;
			while ((float)num6 < base.Width / 8f)
			{
				int num7 = 0;
				while ((float)num7 < base.Height / 8f)
				{
					int num8 = (num6 == 0) ? 0 : (((float)num6 == base.Width / 8f - 1f) ? 2 : 1);
					int num9 = (num7 == 0) ? 0 : (((float)num7 == base.Height / 8f - 1f) ? 2 : 1);
					if (num8 != 1 || num9 != 1)
					{
						this.edges[num8, num9].Draw(new Vector2(base.X + (float)(num6 * 8), base.Y + (float)(num7 * 8)));
					}
					num7++;
				}
				num6++;
			}
			base.Render();
			this.Position = position;
		}

		// Token: 0x06001AEA RID: 6890 RVA: 0x000AEC6C File Offset: 0x000ACE6C
		private void ScrapeParticlesCheck(Vector2 to)
		{
			if (base.Scene.OnInterval(0.03f))
			{
				bool flag = to.Y != base.ExactPosition.Y;
				bool flag2 = to.X != base.ExactPosition.X;
				if (flag && !flag2)
				{
					int num = Math.Sign(to.Y - base.ExactPosition.Y);
					Vector2 value;
					if (num == 1)
					{
						value = base.BottomLeft;
					}
					else
					{
						value = base.TopLeft;
					}
					int num2 = 4;
					if (num == 1)
					{
						num2 = Math.Min((int)base.Height - 12, 20);
					}
					int num3 = (int)base.Height;
					if (num == -1)
					{
						num3 = Math.Max(16, (int)base.Height - 16);
					}
					if (base.Scene.CollideCheck<Solid>(value + new Vector2(-2f, (float)(num * -2))))
					{
						for (int i = num2; i < num3; i += 8)
						{
							base.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, base.TopLeft + new Vector2(0f, (float)i + (float)num * 2f), (num == 1) ? -0.7853982f : 0.7853982f);
						}
					}
					if (base.Scene.CollideCheck<Solid>(value + new Vector2(base.Width + 2f, (float)(num * -2))))
					{
						for (int j = num2; j < num3; j += 8)
						{
							base.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, base.TopRight + new Vector2(-1f, (float)j + (float)num * 2f), (num == 1) ? -2.3561945f : 2.3561945f);
						}
						return;
					}
				}
				else if (flag2 && !flag)
				{
					int num4 = Math.Sign(to.X - base.ExactPosition.X);
					Vector2 value2;
					if (num4 == 1)
					{
						value2 = base.TopRight;
					}
					else
					{
						value2 = base.TopLeft;
					}
					int num5 = 4;
					if (num4 == 1)
					{
						num5 = Math.Min((int)base.Width - 12, 20);
					}
					int num6 = (int)base.Width;
					if (num4 == -1)
					{
						num6 = Math.Max(16, (int)base.Width - 16);
					}
					if (base.Scene.CollideCheck<Solid>(value2 + new Vector2((float)(num4 * -2), -2f)))
					{
						for (int k = num5; k < num6; k += 8)
						{
							base.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, base.TopLeft + new Vector2((float)k + (float)num4 * 2f, -1f), (num4 == 1) ? 2.3561945f : 0.7853982f);
						}
					}
					if (base.Scene.CollideCheck<Solid>(value2 + new Vector2((float)(num4 * -2), base.Height + 2f)))
					{
						for (int l = num5; l < num6; l += 8)
						{
							base.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, base.BottomLeft + new Vector2((float)l + (float)num4 * 2f, 0f), (num4 == 1) ? -2.3561945f : -0.7853982f);
						}
					}
				}
			}
		}

		// Token: 0x06001AEB RID: 6891 RVA: 0x000AEFA8 File Offset: 0x000AD1A8
		private IEnumerator Sequence()
		{
			Vector2 start = this.Position;
			for (;;)
			{
				if (base.HasPlayerRider())
				{
					this.sfx.Play((this.theme == ZipMover.Themes.Normal) ? "event:/game/01_forsaken_city/zip_mover" : "event:/new_content/game/10_farewell/zip_mover", null, 0f);
					Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
					base.StartShaking(0.1f);
					yield return 0.1f;
					this.streetlight.SetAnimationFrame(3);
					this.StopPlayerRunIntoAnimation = false;
					float at = 0f;
					while (at < 1f)
					{
						yield return null;
						at = Calc.Approach(at, 1f, 2f * Engine.DeltaTime);
						this.percent = Ease.SineIn(at);
						Vector2 vector = Vector2.Lerp(start, this.target, this.percent);
						this.ScrapeParticlesCheck(vector);
						if (base.Scene.OnInterval(0.1f))
						{
							this.pathRenderer.CreateSparks();
						}
						base.MoveTo(vector);
					}
					base.StartShaking(0.2f);
					Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
					base.SceneAs<Level>().Shake(0.3f);
					this.StopPlayerRunIntoAnimation = true;
					yield return 0.5f;
					this.StopPlayerRunIntoAnimation = false;
					this.streetlight.SetAnimationFrame(2);
					at = 0f;
					while (at < 1f)
					{
						yield return null;
						at = Calc.Approach(at, 1f, 0.5f * Engine.DeltaTime);
						this.percent = 1f - Ease.SineIn(at);
						Vector2 position = Vector2.Lerp(this.target, start, Ease.SineIn(at));
						base.MoveTo(position);
					}
					this.StopPlayerRunIntoAnimation = true;
					base.StartShaking(0.2f);
					this.streetlight.SetAnimationFrame(1);
					yield return 0.5f;
				}
				else
				{
					yield return null;
				}
			}
			yield break;
		}

		// Token: 0x06001AEC RID: 6892 RVA: 0x00026894 File Offset: 0x00024A94
		private float mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04001785 RID: 6021
		public static ParticleType P_Scrape;

		// Token: 0x04001786 RID: 6022
		public static ParticleType P_Sparks;

		// Token: 0x04001787 RID: 6023
		private ZipMover.Themes theme;

		// Token: 0x04001788 RID: 6024
		private MTexture[,] edges = new MTexture[3, 3];

		// Token: 0x04001789 RID: 6025
		private Sprite streetlight;

		// Token: 0x0400178A RID: 6026
		private BloomPoint bloom;

		// Token: 0x0400178B RID: 6027
		private ZipMover.ZipMoverPathRenderer pathRenderer;

		// Token: 0x0400178C RID: 6028
		private List<MTexture> innerCogs;

		// Token: 0x0400178D RID: 6029
		private MTexture temp = new MTexture();

		// Token: 0x0400178E RID: 6030
		private bool drawBlackBorder;

		// Token: 0x0400178F RID: 6031
		private Vector2 start;

		// Token: 0x04001790 RID: 6032
		private Vector2 target;

		// Token: 0x04001791 RID: 6033
		private float percent;

		// Token: 0x04001792 RID: 6034
		private static readonly Color ropeColor = Calc.HexToColor("663931");

		// Token: 0x04001793 RID: 6035
		private static readonly Color ropeLightColor = Calc.HexToColor("9b6157");

		// Token: 0x04001794 RID: 6036
		private SoundSource sfx = new SoundSource();

		// Token: 0x02000712 RID: 1810
		public enum Themes
		{
			// Token: 0x04002D90 RID: 11664
			Normal,
			// Token: 0x04002D91 RID: 11665
			Moon
		}

		// Token: 0x02000713 RID: 1811
		private class ZipMoverPathRenderer : Entity
		{
			// Token: 0x06002E22 RID: 11810 RVA: 0x00123120 File Offset: 0x00121320
			public ZipMoverPathRenderer(ZipMover zipMover)
			{
				base.Depth = 5000;
				this.ZipMover = zipMover;
				this.from = this.ZipMover.start + new Vector2(this.ZipMover.Width / 2f, this.ZipMover.Height / 2f);
				this.to = this.ZipMover.target + new Vector2(this.ZipMover.Width / 2f, this.ZipMover.Height / 2f);
				this.sparkAdd = (this.from - this.to).SafeNormalize(5f).Perpendicular();
				float num = (this.from - this.to).Angle();
				this.sparkDirFromA = num + 0.3926991f;
				this.sparkDirFromB = num - 0.3926991f;
				this.sparkDirToA = num + 3.1415927f - 0.3926991f;
				this.sparkDirToB = num + 3.1415927f + 0.3926991f;
				if (zipMover.theme == ZipMover.Themes.Moon)
				{
					this.cog = GFX.Game["objects/zipmover/moon/cog"];
					return;
				}
				this.cog = GFX.Game["objects/zipmover/cog"];
			}

			// Token: 0x06002E23 RID: 11811 RVA: 0x00123270 File Offset: 0x00121470
			public void CreateSparks()
			{
				base.SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, this.from + this.sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), this.sparkDirFromA);
				base.SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, this.from - this.sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), this.sparkDirFromB);
				base.SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, this.to + this.sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), this.sparkDirToA);
				base.SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, this.to - this.sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), this.sparkDirToB);
			}

			// Token: 0x06002E24 RID: 11812 RVA: 0x001233A8 File Offset: 0x001215A8
			public override void Render()
			{
				this.DrawCogs(Vector2.UnitY, new Color?(Color.Black));
				this.DrawCogs(Vector2.Zero, null);
				if (this.ZipMover.drawBlackBorder)
				{
					Draw.Rect(new Rectangle((int)(this.ZipMover.X + this.ZipMover.Shake.X - 1f), (int)(this.ZipMover.Y + this.ZipMover.Shake.Y - 1f), (int)this.ZipMover.Width + 2, (int)this.ZipMover.Height + 2), Color.Black);
				}
			}

			// Token: 0x06002E25 RID: 11813 RVA: 0x0012345C File Offset: 0x0012165C
			private void DrawCogs(Vector2 offset, Color? colorOverride = null)
			{
				Vector2 vector = (this.to - this.from).SafeNormalize();
				Vector2 value = vector.Perpendicular() * 3f;
				Vector2 value2 = -vector.Perpendicular() * 4f;
				float rotation = this.ZipMover.percent * 3.1415927f * 2f;
				Draw.Line(this.from + value + offset, this.to + value + offset, (colorOverride != null) ? colorOverride.Value : ZipMover.ropeColor);
				Draw.Line(this.from + value2 + offset, this.to + value2 + offset, (colorOverride != null) ? colorOverride.Value : ZipMover.ropeColor);
				for (float num = 4f - this.ZipMover.percent * 3.1415927f * 8f % 4f; num < (this.to - this.from).Length(); num += 4f)
				{
					Vector2 value3 = this.from + value + vector.Perpendicular() + vector * num;
					Vector2 value4 = this.to + value2 - vector * num;
					Draw.Line(value3 + offset, value3 + vector * 2f + offset, (colorOverride != null) ? colorOverride.Value : ZipMover.ropeLightColor);
					Draw.Line(value4 + offset, value4 - vector * 2f + offset, (colorOverride != null) ? colorOverride.Value : ZipMover.ropeLightColor);
				}
				this.cog.DrawCentered(this.from + offset, (colorOverride != null) ? colorOverride.Value : Color.White, 1f, rotation);
				this.cog.DrawCentered(this.to + offset, (colorOverride != null) ? colorOverride.Value : Color.White, 1f, rotation);
			}

			// Token: 0x04002D92 RID: 11666
			public ZipMover ZipMover;

			// Token: 0x04002D93 RID: 11667
			private MTexture cog;

			// Token: 0x04002D94 RID: 11668
			private Vector2 from;

			// Token: 0x04002D95 RID: 11669
			private Vector2 to;

			// Token: 0x04002D96 RID: 11670
			private Vector2 sparkAdd;

			// Token: 0x04002D97 RID: 11671
			private float sparkDirFromA;

			// Token: 0x04002D98 RID: 11672
			private float sparkDirFromB;

			// Token: 0x04002D99 RID: 11673
			private float sparkDirToA;

			// Token: 0x04002D9A RID: 11674
			private float sparkDirToB;
		}
	}
}
