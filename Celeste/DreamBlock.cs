using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200033E RID: 830
	[Tracked(false)]
	public class DreamBlock : Solid
	{
		// Token: 0x06001A0A RID: 6666 RVA: 0x000A70F0 File Offset: 0x000A52F0
		public DreamBlock(Vector2 position, float width, float height, Vector2? node, bool fastMoving, bool oneUse, bool below) : base(position, width, height, true)
		{
			base.Depth = -11000;
			this.node = node;
			this.fastMoving = fastMoving;
			this.oneUse = oneUse;
			if (below)
			{
				base.Depth = 5000;
			}
			this.SurfaceSoundIndex = 11;
			this.particleTextures = new MTexture[]
			{
				GFX.Game["objects/dreamblock/particles"].GetSubtexture(14, 0, 7, 7, null),
				GFX.Game["objects/dreamblock/particles"].GetSubtexture(7, 0, 7, 7, null),
				GFX.Game["objects/dreamblock/particles"].GetSubtexture(0, 0, 7, 7, null),
				GFX.Game["objects/dreamblock/particles"].GetSubtexture(7, 0, 7, 7, null)
			};
		}

		// Token: 0x06001A0B RID: 6667 RVA: 0x000A71F4 File Offset: 0x000A53F4
		public DreamBlock(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height, data.FirstNodeNullable(new Vector2?(offset)), data.Bool("fastMoving", false), data.Bool("oneUse", false), data.Bool("below", false))
		{
		}

		// Token: 0x06001A0C RID: 6668 RVA: 0x000A7254 File Offset: 0x000A5454
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.playerHasDreamDash = base.SceneAs<Level>().Session.Inventory.DreamDash;
			if (this.playerHasDreamDash && this.node != null)
			{
				Vector2 start = this.Position;
				Vector2 end = this.node.Value;
				float num = Vector2.Distance(start, end) / 12f;
				if (this.fastMoving)
				{
					num /= 3f;
				}
				Tween tween = Tween.Create(Tween.TweenMode.YoyoLooping, Ease.SineInOut, num, true);
				tween.OnUpdate = delegate(Tween t)
				{
					if (this.Collidable)
					{
						this.MoveTo(Vector2.Lerp(start, end, t.Eased));
						return;
					}
					this.MoveToNaive(Vector2.Lerp(start, end, t.Eased));
				};
				base.Add(tween);
			}
			if (!this.playerHasDreamDash)
			{
				base.Add(this.occlude = new LightOcclude(1f));
			}
			this.Setup();
		}

		// Token: 0x06001A0D RID: 6669 RVA: 0x000A733C File Offset: 0x000A553C
		public void Setup()
		{
			this.particles = new DreamBlock.DreamParticle[(int)(base.Width / 8f * (base.Height / 8f) * 0.7f)];
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.particles[i].Position = new Vector2(Calc.Random.NextFloat(base.Width), Calc.Random.NextFloat(base.Height));
				this.particles[i].Layer = Calc.Random.Choose(0, 1, 1, 2, 2, 2);
				this.particles[i].TimeOffset = Calc.Random.NextFloat();
				this.particles[i].Color = Color.LightGray * (0.5f + (float)this.particles[i].Layer / 2f * 0.5f);
				if (this.playerHasDreamDash)
				{
					switch (this.particles[i].Layer)
					{
					case 0:
						this.particles[i].Color = Calc.Random.Choose(Calc.HexToColor("FFEF11"), Calc.HexToColor("FF00D0"), Calc.HexToColor("08a310"));
						break;
					case 1:
						this.particles[i].Color = Calc.Random.Choose(Calc.HexToColor("5fcde4"), Calc.HexToColor("7fb25e"), Calc.HexToColor("E0564C"));
						break;
					case 2:
						this.particles[i].Color = Calc.Random.Choose(Calc.HexToColor("5b6ee1"), Calc.HexToColor("CC3B3B"), Calc.HexToColor("7daa64"));
						break;
					}
				}
			}
		}

		// Token: 0x06001A0E RID: 6670 RVA: 0x000A7524 File Offset: 0x000A5724
		public void OnPlayerExit(Player player)
		{
			Dust.Burst(player.Position, player.Speed.Angle(), 16, null);
			Vector2 value = Vector2.Zero;
			if (base.CollideCheck(player, this.Position + Vector2.UnitX * 4f))
			{
				value = Vector2.UnitX;
			}
			else if (base.CollideCheck(player, this.Position - Vector2.UnitX * 4f))
			{
				value = -Vector2.UnitX;
			}
			else if (base.CollideCheck(player, this.Position + Vector2.UnitY * 4f))
			{
				value = Vector2.UnitY;
			}
			else if (base.CollideCheck(player, this.Position - Vector2.UnitY * 4f))
			{
				value = -Vector2.UnitY;
			}
			value != Vector2.Zero;
			if (this.oneUse)
			{
				this.OneUseDestroy();
			}
		}

		// Token: 0x06001A0F RID: 6671 RVA: 0x000A7624 File Offset: 0x000A5824
		private void OneUseDestroy()
		{
			this.Collidable = (this.Visible = false);
			base.DisableStaticMovers();
			base.RemoveSelf();
		}

		// Token: 0x06001A10 RID: 6672 RVA: 0x000A7650 File Offset: 0x000A5850
		public override void Update()
		{
			base.Update();
			if (this.playerHasDreamDash)
			{
				this.animTimer += 6f * Engine.DeltaTime;
				this.wobbleEase += Engine.DeltaTime * 2f;
				if (this.wobbleEase > 1f)
				{
					this.wobbleEase = 0f;
					this.wobbleFrom = this.wobbleTo;
					this.wobbleTo = Calc.Random.NextFloat(6.2831855f);
				}
				this.SurfaceSoundIndex = 12;
			}
		}

		// Token: 0x06001A11 RID: 6673 RVA: 0x000A76DC File Offset: 0x000A58DC
		public bool BlockedCheck()
		{
			TheoCrystal theoCrystal = base.CollideFirst<TheoCrystal>();
			if (theoCrystal != null && !this.TryActorWiggleUp(theoCrystal))
			{
				return true;
			}
			Player player = base.CollideFirst<Player>();
			return player != null && !this.TryActorWiggleUp(player);
		}

		// Token: 0x06001A12 RID: 6674 RVA: 0x000A7714 File Offset: 0x000A5914
		private bool TryActorWiggleUp(Entity actor)
		{
			bool collidable = this.Collidable;
			this.Collidable = true;
			for (int i = 1; i <= 4; i++)
			{
				if (!actor.CollideCheck<Solid>(actor.Position - Vector2.UnitY * (float)i))
				{
					actor.Position -= Vector2.UnitY * (float)i;
					this.Collidable = collidable;
					return true;
				}
			}
			this.Collidable = collidable;
			return false;
		}

		// Token: 0x06001A13 RID: 6675 RVA: 0x000A7788 File Offset: 0x000A5988
		public override void Render()
		{
			Camera camera = base.SceneAs<Level>().Camera;
			if (base.Right < camera.Left || base.Left > camera.Right || base.Bottom < camera.Top || base.Top > camera.Bottom)
			{
				return;
			}
			Draw.Rect(this.shake.X + base.X, this.shake.Y + base.Y, base.Width, base.Height, this.playerHasDreamDash ? DreamBlock.activeBackColor : DreamBlock.disabledBackColor);
			Vector2 position = base.SceneAs<Level>().Camera.Position;
			for (int i = 0; i < this.particles.Length; i++)
			{
				int layer = this.particles[i].Layer;
				Vector2 vector = this.particles[i].Position;
				vector += position * (0.3f + 0.25f * (float)layer);
				vector = this.PutInside(vector);
				Color color = this.particles[i].Color;
				MTexture mtexture;
				if (layer == 0)
				{
					int num = (int)((this.particles[i].TimeOffset * 4f + this.animTimer) % 4f);
					mtexture = this.particleTextures[3 - num];
				}
				else if (layer == 1)
				{
					int num2 = (int)((this.particles[i].TimeOffset * 2f + this.animTimer) % 2f);
					mtexture = this.particleTextures[1 + num2];
				}
				else
				{
					mtexture = this.particleTextures[2];
				}
				if (vector.X >= base.X + 2f && vector.Y >= base.Y + 2f && vector.X < base.Right - 2f && vector.Y < base.Bottom - 2f)
				{
					mtexture.DrawCentered(vector + this.shake, color);
				}
			}
			if (this.whiteFill > 0f)
			{
				Draw.Rect(base.X + this.shake.X, base.Y + this.shake.Y, base.Width, base.Height * this.whiteHeight, Color.White * this.whiteFill);
			}
			this.WobbleLine(this.shake + new Vector2(base.X, base.Y), this.shake + new Vector2(base.X + base.Width, base.Y), 0f);
			this.WobbleLine(this.shake + new Vector2(base.X + base.Width, base.Y), this.shake + new Vector2(base.X + base.Width, base.Y + base.Height), 0.7f);
			this.WobbleLine(this.shake + new Vector2(base.X + base.Width, base.Y + base.Height), this.shake + new Vector2(base.X, base.Y + base.Height), 1.5f);
			this.WobbleLine(this.shake + new Vector2(base.X, base.Y + base.Height), this.shake + new Vector2(base.X, base.Y), 2.5f);
			Draw.Rect(this.shake + new Vector2(base.X, base.Y), 2f, 2f, this.playerHasDreamDash ? DreamBlock.activeLineColor : DreamBlock.disabledLineColor);
			Draw.Rect(this.shake + new Vector2(base.X + base.Width - 2f, base.Y), 2f, 2f, this.playerHasDreamDash ? DreamBlock.activeLineColor : DreamBlock.disabledLineColor);
			Draw.Rect(this.shake + new Vector2(base.X, base.Y + base.Height - 2f), 2f, 2f, this.playerHasDreamDash ? DreamBlock.activeLineColor : DreamBlock.disabledLineColor);
			Draw.Rect(this.shake + new Vector2(base.X + base.Width - 2f, base.Y + base.Height - 2f), 2f, 2f, this.playerHasDreamDash ? DreamBlock.activeLineColor : DreamBlock.disabledLineColor);
		}

		// Token: 0x06001A14 RID: 6676 RVA: 0x000A7C60 File Offset: 0x000A5E60
		private Vector2 PutInside(Vector2 pos)
		{
			while (pos.X < base.X)
			{
				pos.X += base.Width;
			}
			while (pos.X > base.X + base.Width)
			{
				pos.X -= base.Width;
			}
			while (pos.Y < base.Y)
			{
				pos.Y += base.Height;
			}
			while (pos.Y > base.Y + base.Height)
			{
				pos.Y -= base.Height;
			}
			return pos;
		}

		// Token: 0x06001A15 RID: 6677 RVA: 0x000A7D00 File Offset: 0x000A5F00
		private void WobbleLine(Vector2 from, Vector2 to, float offset)
		{
			float num = (to - from).Length();
			Vector2 vector = Vector2.Normalize(to - from);
			Vector2 vector2 = new Vector2(vector.Y, -vector.X);
			Color color = this.playerHasDreamDash ? DreamBlock.activeLineColor : DreamBlock.disabledLineColor;
			Color color2 = this.playerHasDreamDash ? DreamBlock.activeBackColor : DreamBlock.disabledBackColor;
			if (this.whiteFill > 0f)
			{
				color = Color.Lerp(color, Color.White, this.whiteFill);
				color2 = Color.Lerp(color2, Color.White, this.whiteFill);
			}
			float scaleFactor = 0f;
			int num2 = 16;
			int num3 = 2;
			while ((float)num3 < num - 2f)
			{
				float num4 = this.Lerp(this.LineAmplitude(this.wobbleFrom + offset, (float)num3), this.LineAmplitude(this.wobbleTo + offset, (float)num3), this.wobbleEase);
				if ((float)(num3 + num2) >= num)
				{
					num4 = 0f;
				}
				float num5 = Math.Min((float)num2, num - 2f - (float)num3);
				Vector2 vector3 = from + vector * (float)num3 + vector2 * scaleFactor;
				Vector2 vector4 = from + vector * ((float)num3 + num5) + vector2 * num4;
				Draw.Line(vector3 - vector2, vector4 - vector2, color2);
				Draw.Line(vector3 - vector2 * 2f, vector4 - vector2 * 2f, color2);
				Draw.Line(vector3, vector4, color);
				scaleFactor = num4;
				num3 += num2;
			}
		}

		// Token: 0x06001A16 RID: 6678 RVA: 0x000A7EA4 File Offset: 0x000A60A4
		private float LineAmplitude(float seed, float index)
		{
			return (float)(Math.Sin((double)(seed + index / 16f) + Math.Sin((double)(seed * 2f + index / 32f)) * 6.2831854820251465) + 1.0) * 1.5f;
		}

		// Token: 0x06001A17 RID: 6679 RVA: 0x000A7EF1 File Offset: 0x000A60F1
		private float Lerp(float a, float b, float percent)
		{
			return a + (b - a) * percent;
		}

		// Token: 0x06001A18 RID: 6680 RVA: 0x000A7EFA File Offset: 0x000A60FA
		public IEnumerator Activate()
		{
			Level level = base.SceneAs<Level>();
			yield return 1f;
			Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
			base.Add(this.shaker = new Shaker(true, delegate(Vector2 t)
			{
				this.shake = t;
			}));
			this.shaker.Interval = 0.02f;
			this.shaker.On = true;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime)
			{
				this.whiteFill = Ease.CubeIn(p);
				yield return null;
			}
			this.shaker.On = false;
			yield return 0.5f;
			this.ActivateNoRoutine();
			this.whiteHeight = 1f;
			this.whiteFill = 1f;
			for (float p = 1f; p > 0f; p -= Engine.DeltaTime * 0.5f)
			{
				this.whiteHeight = p;
				if (level.OnInterval(0.1f))
				{
					int num = 0;
					while ((float)num < base.Width)
					{
						level.ParticlesFG.Emit(Strawberry.P_WingsBurst, new Vector2(base.X + (float)num, base.Y + base.Height * this.whiteHeight + 1f));
						num += 4;
					}
				}
				if (level.OnInterval(0.1f))
				{
					level.Shake(0.3f);
				}
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
				yield return null;
			}
			while (this.whiteFill > 0f)
			{
				this.whiteFill -= Engine.DeltaTime * 3f;
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001A19 RID: 6681 RVA: 0x000A7F0C File Offset: 0x000A610C
		public void ActivateNoRoutine()
		{
			if (!this.playerHasDreamDash)
			{
				this.playerHasDreamDash = true;
				this.Setup();
				base.Remove(this.occlude);
			}
			this.whiteHeight = 0f;
			this.whiteFill = 0f;
			if (this.shaker != null)
			{
				this.shaker.On = false;
			}
		}

		// Token: 0x06001A1A RID: 6682 RVA: 0x000A7F64 File Offset: 0x000A6164
		public void FootstepRipple(Vector2 position)
		{
			if (this.playerHasDreamDash)
			{
				DisplacementRenderer.Burst burst = (base.Scene as Level).Displacement.AddBurst(position, 0.5f, 0f, 40f, 1f, null, null);
				burst.WorldClipCollider = base.Collider;
				burst.WorldClipPadding = 1;
			}
		}

		// Token: 0x040016B0 RID: 5808
		private static readonly Color activeBackColor = Color.Black;

		// Token: 0x040016B1 RID: 5809
		private static readonly Color disabledBackColor = Calc.HexToColor("1f2e2d");

		// Token: 0x040016B2 RID: 5810
		private static readonly Color activeLineColor = Color.White;

		// Token: 0x040016B3 RID: 5811
		private static readonly Color disabledLineColor = Calc.HexToColor("6a8480");

		// Token: 0x040016B4 RID: 5812
		private bool playerHasDreamDash;

		// Token: 0x040016B5 RID: 5813
		private Vector2? node;

		// Token: 0x040016B6 RID: 5814
		private LightOcclude occlude;

		// Token: 0x040016B7 RID: 5815
		private MTexture[] particleTextures;

		// Token: 0x040016B8 RID: 5816
		private DreamBlock.DreamParticle[] particles;

		// Token: 0x040016B9 RID: 5817
		private float whiteFill;

		// Token: 0x040016BA RID: 5818
		private float whiteHeight = 1f;

		// Token: 0x040016BB RID: 5819
		private Vector2 shake;

		// Token: 0x040016BC RID: 5820
		private float animTimer;

		// Token: 0x040016BD RID: 5821
		private Shaker shaker;

		// Token: 0x040016BE RID: 5822
		private bool fastMoving;

		// Token: 0x040016BF RID: 5823
		private bool oneUse;

		// Token: 0x040016C0 RID: 5824
		private float wobbleFrom = Calc.Random.NextFloat(6.2831855f);

		// Token: 0x040016C1 RID: 5825
		private float wobbleTo = Calc.Random.NextFloat(6.2831855f);

		// Token: 0x040016C2 RID: 5826
		private float wobbleEase;

		// Token: 0x020006EC RID: 1772
		private struct DreamParticle
		{
			// Token: 0x04002CDF RID: 11487
			public Vector2 Position;

			// Token: 0x04002CE0 RID: 11488
			public int Layer;

			// Token: 0x04002CE1 RID: 11489
			public Color Color;

			// Token: 0x04002CE2 RID: 11490
			public float TimeOffset;
		}
	}
}
