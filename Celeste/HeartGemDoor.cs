using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001C2 RID: 450
	public class HeartGemDoor : Entity
	{
		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000F61 RID: 3937 RVA: 0x0003E914 File Offset: 0x0003CB14
		public int HeartGems
		{
			get
			{
				if (SaveData.Instance.CheatMode)
				{
					return this.Requires;
				}
				return SaveData.Instance.TotalHeartGems;
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000F62 RID: 3938 RVA: 0x0003E933 File Offset: 0x0003CB33
		// (set) Token: 0x06000F63 RID: 3939 RVA: 0x0003E93B File Offset: 0x0003CB3B
		public float Counter { get; private set; }

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000F64 RID: 3940 RVA: 0x0003E944 File Offset: 0x0003CB44
		// (set) Token: 0x06000F65 RID: 3941 RVA: 0x0003E94C File Offset: 0x0003CB4C
		public bool Opened { get; private set; }

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06000F66 RID: 3942 RVA: 0x0003E955 File Offset: 0x0003CB55
		private float openAmount
		{
			get
			{
				return this.openPercent * this.openDistance;
			}
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x0003E964 File Offset: 0x0003CB64
		public HeartGemDoor(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			this.Requires = data.Int("requires", 0);
			base.Add(new CustomBloom(new Action(this.RenderBloom)));
			this.Size = data.Width;
			this.openDistance = 32f;
			Vector2? vector = data.FirstNodeNullable(new Vector2?(offset));
			if (vector != null)
			{
				this.openDistance = Math.Abs(vector.Value.Y - base.Y);
			}
			this.icon = GFX.Game.GetAtlasSubtextures("objects/heartdoor/icon");
			this.startHidden = data.Bool("startHidden", false);
		}

		// Token: 0x06000F68 RID: 3944 RVA: 0x0003EA44 File Offset: 0x0003CC44
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Level level = scene as Level;
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.particles[i].Position = new Vector2(Calc.Random.NextFloat((float)this.Size), Calc.Random.NextFloat((float)level.Bounds.Height));
				this.particles[i].Speed = (float)Calc.Random.Range(4, 12);
				this.particles[i].Color = Color.White * Calc.Random.Range(0.2f, 0.6f);
			}
			level.Add(this.TopSolid = new Solid(new Vector2(base.X, (float)(level.Bounds.Top - 32)), (float)this.Size, base.Y - (float)level.Bounds.Top + 32f, true));
			this.TopSolid.SurfaceSoundIndex = 32;
			this.TopSolid.SquishEvenInAssistMode = true;
			this.TopSolid.EnableAssistModeChecks = false;
			level.Add(this.BotSolid = new Solid(new Vector2(base.X, base.Y), (float)this.Size, (float)level.Bounds.Bottom - base.Y + 32f, true));
			this.BotSolid.SurfaceSoundIndex = 32;
			this.BotSolid.SquishEvenInAssistMode = true;
			this.BotSolid.EnableAssistModeChecks = false;
			if ((base.Scene as Level).Session.GetFlag("opened_heartgem_door_" + this.Requires))
			{
				this.Opened = true;
				this.Visible = true;
				this.openPercent = 1f;
				this.Counter = (float)this.Requires;
				this.TopSolid.Y -= this.openDistance;
				this.BotSolid.Y += this.openDistance;
				return;
			}
			base.Add(new Coroutine(this.Routine(), true));
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x0003EC80 File Offset: 0x0003CE80
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (this.Opened)
			{
				DashBlock dashBlock = base.Scene.CollideFirst<DashBlock>(this.BotSolid.Collider.Bounds);
				if (dashBlock != null)
				{
					dashBlock.RemoveSelf();
					return;
				}
			}
			else if (this.startHidden)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.X > base.X)
				{
					this.startHidden = false;
					DashBlock dashBlock2 = base.Scene.CollideFirst<DashBlock>(this.BotSolid.Collider.Bounds);
					if (dashBlock2 != null)
					{
						dashBlock2.RemoveSelf();
						return;
					}
				}
				else
				{
					this.Visible = false;
				}
			}
		}

		// Token: 0x06000F6A RID: 3946 RVA: 0x0003ED20 File Offset: 0x0003CF20
		private IEnumerator Routine()
		{
			Level level = base.Scene as Level;
			float topTo;
			float botTo;
			float topFrom;
			float botFrom;
			if (this.startHidden)
			{
				Player entity;
				do
				{
					yield return null;
					entity = base.Scene.Tracker.GetEntity<Player>();
				}
				while (entity == null || Math.Abs(entity.X - base.Center.X) >= 100f);
				Audio.Play("event:/new_content/game/10_farewell/heart_door", this.Position);
				this.Visible = true;
				this.heartAlpha = 0f;
				topTo = this.TopSolid.Y;
				botTo = this.BotSolid.Y;
				topFrom = (this.TopSolid.Y -= 240f);
				botFrom = (this.BotSolid.Y -= 240f);
				for (float p = 0f; p < 1f; p += Engine.DeltaTime * 1.2f)
				{
					float num = Ease.CubeIn(p);
					this.TopSolid.MoveToY(topFrom + (topTo - topFrom) * num);
					this.BotSolid.MoveToY(botFrom + (botTo - botFrom) * num);
					DashBlock dashBlock = base.Scene.CollideFirst<DashBlock>(this.BotSolid.Collider.Bounds);
					if (dashBlock != null)
					{
						level.Shake(0.5f);
						Celeste.Freeze(0.1f);
						Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
						dashBlock.Break(this.BotSolid.BottomCenter, new Vector2(0f, 1f), true, false);
						Player entity2 = base.Scene.Tracker.GetEntity<Player>();
						if (entity2 != null && Math.Abs(entity2.X - base.Center.X) < 40f)
						{
							entity2.PointBounce(entity2.Position + Vector2.UnitX * 8f);
						}
					}
					yield return null;
				}
				level.Shake(0.5f);
				Celeste.Freeze(0.1f);
				this.TopSolid.Y = topTo;
				this.BotSolid.Y = botTo;
				while (this.heartAlpha < 1f)
				{
					this.heartAlpha = Calc.Approach(this.heartAlpha, 1f, Engine.DeltaTime * 2f);
					yield return null;
				}
				yield return 0.6f;
			}
			while (!this.Opened && this.Counter < (float)this.Requires)
			{
				Player entity3 = base.Scene.Tracker.GetEntity<Player>();
				if (entity3 != null && Math.Abs(entity3.X - base.Center.X) < 80f && entity3.X < base.X)
				{
					if (this.Counter == 0f && this.HeartGems > 0)
					{
						Audio.Play("event:/game/09_core/frontdoor_heartfill", this.Position);
					}
					if (this.HeartGems < this.Requires)
					{
						level.Session.SetFlag("granny_door", true);
					}
					int num2 = (int)this.Counter;
					int target = Math.Min(this.HeartGems, this.Requires);
					this.Counter = Calc.Approach(this.Counter, (float)target, Engine.DeltaTime * (float)this.Requires * 0.8f);
					if (num2 != (int)this.Counter)
					{
						yield return 0.1f;
						if (this.Counter < (float)target)
						{
							Audio.Play("event:/game/09_core/frontdoor_heartfill", this.Position);
						}
					}
				}
				else
				{
					this.Counter = Calc.Approach(this.Counter, 0f, Engine.DeltaTime * (float)this.Requires * 4f);
				}
				yield return null;
			}
			yield return 0.5f;
			base.Scene.Add(new HeartGemDoor.WhiteLine(this.Position, this.Size));
			level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
			level.Flash(Color.White * 0.5f, false);
			Audio.Play("event:/game/09_core/frontdoor_unlock", this.Position);
			this.Opened = true;
			level.Session.SetFlag("opened_heartgem_door_" + this.Requires, true);
			this.offset = 0f;
			yield return 0.6f;
			botFrom = this.TopSolid.Y;
			topFrom = this.TopSolid.Y - this.openDistance;
			botTo = this.BotSolid.Y;
			topTo = this.BotSolid.Y + this.openDistance;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime)
			{
				level.Shake(0.3f);
				this.openPercent = Ease.CubeIn(p);
				this.TopSolid.MoveToY(MathHelper.Lerp(botFrom, topFrom, this.openPercent));
				this.BotSolid.MoveToY(MathHelper.Lerp(botTo, topTo, this.openPercent));
				if (p >= 0.4f && level.OnInterval(0.1f))
				{
					for (int i = 4; i < this.Size; i += 4)
					{
						level.ParticlesBG.Emit(HeartGemDoor.P_Shimmer, 1, new Vector2(this.TopSolid.Left + (float)i + 1f, this.TopSolid.Bottom - 2f), new Vector2(2f, 2f), -1.5707964f);
						level.ParticlesBG.Emit(HeartGemDoor.P_Shimmer, 1, new Vector2(this.BotSolid.Left + (float)i + 1f, this.BotSolid.Top + 2f), new Vector2(2f, 2f), 1.5707964f);
					}
				}
				yield return null;
			}
			this.TopSolid.MoveToY(topFrom);
			this.BotSolid.MoveToY(topTo);
			this.openPercent = 1f;
			yield break;
		}

		// Token: 0x06000F6B RID: 3947 RVA: 0x0003ED30 File Offset: 0x0003CF30
		public override void Update()
		{
			base.Update();
			if (!this.Opened)
			{
				this.offset += 12f * Engine.DeltaTime;
				this.mist.X = this.mist.X - 4f * Engine.DeltaTime;
				this.mist.Y = this.mist.Y - 24f * Engine.DeltaTime;
				for (int i = 0; i < this.particles.Length; i++)
				{
					HeartGemDoor.Particle[] array = this.particles;
					int num = i;
					array[num].Position.Y = array[num].Position.Y + this.particles[i].Speed * Engine.DeltaTime;
				}
			}
		}

		// Token: 0x06000F6C RID: 3948 RVA: 0x0003EDE0 File Offset: 0x0003CFE0
		public void RenderBloom()
		{
			if (!this.Opened && this.Visible)
			{
				this.DrawBloom(new Rectangle((int)this.TopSolid.X, (int)this.TopSolid.Y, this.Size, (int)(this.TopSolid.Height + this.BotSolid.Height)));
			}
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x0003EE40 File Offset: 0x0003D040
		private void DrawBloom(Rectangle bounds)
		{
			Draw.Rect((float)(bounds.Left - 4), (float)bounds.Top, 2f, (float)bounds.Height, Color.White * 0.25f);
			Draw.Rect((float)(bounds.Left - 2), (float)bounds.Top, 2f, (float)bounds.Height, Color.White * 0.5f);
			Draw.Rect(bounds, Color.White * 0.75f);
			Draw.Rect((float)bounds.Right, (float)bounds.Top, 2f, (float)bounds.Height, Color.White * 0.5f);
			Draw.Rect((float)(bounds.Right + 2), (float)bounds.Top, 2f, (float)bounds.Height, Color.White * 0.25f);
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x0003EF28 File Offset: 0x0003D128
		private void DrawMist(Rectangle bounds, Vector2 mist)
		{
			Color color = Color.White * 0.6f;
			MTexture mtexture = GFX.Game["objects/heartdoor/mist"];
			int num = mtexture.Width / 2;
			int num2 = mtexture.Height / 2;
			for (int i = 0; i < bounds.Width; i += num)
			{
				for (int j = 0; j < bounds.Height; j += num2)
				{
					mtexture.GetSubtexture((int)this.Mod(mist.X, (float)num), (int)this.Mod(mist.Y, (float)num2), Math.Min(num, bounds.Width - i), Math.Min(num2, bounds.Height - j), this.temp);
					this.temp.Draw(new Vector2((float)(bounds.X + i), (float)(bounds.Y + j)), Vector2.Zero, color);
				}
			}
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x0003F010 File Offset: 0x0003D210
		private void DrawInterior(Rectangle bounds)
		{
			Draw.Rect(bounds, Calc.HexToColor("18668f"));
			this.DrawMist(bounds, this.mist);
			this.DrawMist(bounds, new Vector2(this.mist.Y, this.mist.X) * 1.5f);
			Vector2 value = (base.Scene as Level).Camera.Position;
			if (this.Opened)
			{
				value = Vector2.Zero;
			}
			for (int i = 0; i < this.particles.Length; i++)
			{
				Vector2 vector = this.particles[i].Position + value * 0.2f;
				vector.X = this.Mod(vector.X, (float)bounds.Width);
				vector.Y = this.Mod(vector.Y, (float)bounds.Height);
				Draw.Pixel.Draw(new Vector2((float)bounds.X, (float)bounds.Y) + vector, Vector2.Zero, this.particles[i].Color);
			}
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x0003F134 File Offset: 0x0003D334
		private void DrawEdges(Rectangle bounds, Color color)
		{
			MTexture mtexture = GFX.Game["objects/heartdoor/edge"];
			MTexture mtexture2 = GFX.Game["objects/heartdoor/top"];
			int num = (int)(this.offset % 8f);
			if (num > 0)
			{
				mtexture.GetSubtexture(0, 8 - num, 7, num, this.temp);
				this.temp.DrawJustified(new Vector2((float)(bounds.Left + 4), (float)bounds.Top), new Vector2(0.5f, 0f), color, new Vector2(-1f, 1f));
				this.temp.DrawJustified(new Vector2((float)(bounds.Right - 4), (float)bounds.Top), new Vector2(0.5f, 0f), color, new Vector2(1f, 1f));
			}
			for (int i = num; i < bounds.Height; i += 8)
			{
				mtexture.GetSubtexture(0, 0, 8, Math.Min(8, bounds.Height - i), this.temp);
				this.temp.DrawJustified(new Vector2((float)(bounds.Left + 4), (float)(bounds.Top + i)), new Vector2(0.5f, 0f), color, new Vector2(-1f, 1f));
				this.temp.DrawJustified(new Vector2((float)(bounds.Right - 4), (float)(bounds.Top + i)), new Vector2(0.5f, 0f), color, new Vector2(1f, 1f));
			}
			for (int j = 0; j < bounds.Width; j += 8)
			{
				mtexture2.DrawCentered(new Vector2((float)(bounds.Left + 4 + j), (float)(bounds.Top + 4)), color);
				mtexture2.DrawCentered(new Vector2((float)(bounds.Left + 4 + j), (float)(bounds.Bottom - 4)), color, new Vector2(1f, -1f));
			}
		}

		// Token: 0x06000F71 RID: 3953 RVA: 0x0003F330 File Offset: 0x0003D530
		public override void Render()
		{
			Color color = this.Opened ? (Color.White * 0.25f) : Color.White;
			if (!this.Opened && this.TopSolid.Visible && this.BotSolid.Visible)
			{
				Rectangle bounds = new Rectangle((int)this.TopSolid.X, (int)this.TopSolid.Y, this.Size, (int)(this.TopSolid.Height + this.BotSolid.Height));
				this.DrawInterior(bounds);
				this.DrawEdges(bounds, color);
			}
			else
			{
				if (this.TopSolid.Visible)
				{
					Rectangle bounds2 = new Rectangle((int)this.TopSolid.X, (int)this.TopSolid.Y, this.Size, (int)this.TopSolid.Height);
					this.DrawInterior(bounds2);
					this.DrawEdges(bounds2, color);
				}
				if (this.BotSolid.Visible)
				{
					Rectangle bounds3 = new Rectangle((int)this.BotSolid.X, (int)this.BotSolid.Y, this.Size, (int)this.BotSolid.Height);
					this.DrawInterior(bounds3);
					this.DrawEdges(bounds3, color);
				}
			}
			if (this.heartAlpha > 0f)
			{
				float num = 12f;
				int num2 = (int)((float)(this.Size - 8) / num);
				int num3 = (int)Math.Ceiling((double)((float)this.Requires / (float)num2));
				Color color2 = color * this.heartAlpha;
				for (int i = 0; i < num3; i++)
				{
					int num4 = ((i + 1) * num2 < this.Requires) ? num2 : (this.Requires - i * num2);
					Vector2 value = new Vector2(base.X + (float)this.Size * 0.5f, base.Y) + new Vector2((float)(-(float)num4) / 2f + 0.5f, (float)(-(float)num3) / 2f + (float)i + 0.5f) * num;
					if (this.Opened)
					{
						if (i < num3 / 2)
						{
							value.Y -= this.openAmount + 8f;
						}
						else
						{
							value.Y += this.openAmount + 8f;
						}
					}
					for (int j = 0; j < num4; j++)
					{
						int num5 = i * num2 + j;
						float num6 = Ease.CubeIn(Calc.ClampedMap(this.Counter, (float)num5, (float)num5 + 1f, 0f, 1f));
						this.icon[(int)(num6 * (float)(this.icon.Count - 1))].DrawCentered(value + new Vector2((float)j * num, 0f), color2);
					}
				}
			}
		}

		// Token: 0x06000F72 RID: 3954 RVA: 0x00026894 File Offset: 0x00024A94
		private float Mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000AC5 RID: 2757
		private const string OpenedFlag = "opened_heartgem_door_";

		// Token: 0x04000AC6 RID: 2758
		public static ParticleType P_Shimmer;

		// Token: 0x04000AC7 RID: 2759
		public static ParticleType P_Slice;

		// Token: 0x04000AC8 RID: 2760
		public readonly int Requires;

		// Token: 0x04000ACB RID: 2763
		public int Size;

		// Token: 0x04000ACC RID: 2764
		private readonly float openDistance;

		// Token: 0x04000ACD RID: 2765
		private float openPercent;

		// Token: 0x04000ACE RID: 2766
		private Solid TopSolid;

		// Token: 0x04000ACF RID: 2767
		private Solid BotSolid;

		// Token: 0x04000AD0 RID: 2768
		private float offset;

		// Token: 0x04000AD1 RID: 2769
		private Vector2 mist;

		// Token: 0x04000AD2 RID: 2770
		private MTexture temp = new MTexture();

		// Token: 0x04000AD3 RID: 2771
		private List<MTexture> icon;

		// Token: 0x04000AD4 RID: 2772
		private HeartGemDoor.Particle[] particles = new HeartGemDoor.Particle[50];

		// Token: 0x04000AD5 RID: 2773
		private bool startHidden;

		// Token: 0x04000AD6 RID: 2774
		private float heartAlpha = 1f;

		// Token: 0x020004C3 RID: 1219
		private struct Particle
		{
			// Token: 0x04002373 RID: 9075
			public Vector2 Position;

			// Token: 0x04002374 RID: 9076
			public float Speed;

			// Token: 0x04002375 RID: 9077
			public Color Color;
		}

		// Token: 0x020004C4 RID: 1220
		private class WhiteLine : Entity
		{
			// Token: 0x060023EE RID: 9198 RVA: 0x000F03A5 File Offset: 0x000EE5A5
			public WhiteLine(Vector2 origin, int blockSize) : base(origin)
			{
				base.Depth = -1000000;
				this.blockSize = blockSize;
			}

			// Token: 0x060023EF RID: 9199 RVA: 0x000F03CC File Offset: 0x000EE5CC
			public override void Update()
			{
				base.Update();
				this.fade = Calc.Approach(this.fade, 0f, Engine.DeltaTime);
				if (this.fade <= 0f)
				{
					base.RemoveSelf();
					Level level = base.SceneAs<Level>();
					for (float num = (float)((int)level.Camera.Left); num < level.Camera.Right; num += 1f)
					{
						if (num < base.X || num >= base.X + (float)this.blockSize)
						{
							level.Particles.Emit(HeartGemDoor.P_Slice, new Vector2(num, base.Y));
						}
					}
				}
			}

			// Token: 0x060023F0 RID: 9200 RVA: 0x000F0470 File Offset: 0x000EE670
			public override void Render()
			{
				ref Vector2 position = (base.Scene as Level).Camera.Position;
				float num = Math.Max(1f, 4f * this.fade);
				Draw.Rect(position.X - 10f, base.Y - num / 2f, 340f, num, Color.White);
			}

			// Token: 0x04002376 RID: 9078
			private float fade = 1f;

			// Token: 0x04002377 RID: 9079
			private int blockSize;
		}
	}
}
