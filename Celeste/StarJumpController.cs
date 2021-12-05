using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020001C8 RID: 456
	[Tracked(false)]
	public class StarJumpController : Entity
	{
		// Token: 0x06000F8F RID: 3983 RVA: 0x00040D74 File Offset: 0x0003EF74
		public StarJumpController()
		{
			this.InitBlockFill();
		}

		// Token: 0x06000F90 RID: 3984 RVA: 0x00040DC4 File Offset: 0x0003EFC4
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
			this.minY = (float)(this.level.Bounds.Top + 80);
			this.maxY = (float)(this.level.Bounds.Top + 1800);
			this.minX = (float)(this.level.Bounds.Left + 80);
			this.maxX = (float)(this.level.Bounds.Right - 80);
			this.level.Session.Audio.Music.Event = "event:/music/lvl6/starjump";
			this.level.Session.Audio.Music.Layer(1, 1f);
			this.level.Session.Audio.Music.Layer(2, 0f);
			this.level.Session.Audio.Music.Layer(3, 0f);
			this.level.Session.Audio.Music.Layer(4, 0f);
			this.level.Session.Audio.Apply(false);
			this.random = new Random(666);
			base.Add(new BeforeRenderHook(new Action(this.BeforeRender)));
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x00040F3C File Offset: 0x0003F13C
		public override void Update()
		{
			base.Update();
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				float centerY = entity.CenterY;
				this.level.Session.Audio.Music.Layer(1, Calc.ClampedMap(centerY, this.maxY, this.minY, 1f, 0f));
				this.level.Session.Audio.Music.Layer(2, Calc.ClampedMap(centerY, this.maxY, this.minY, 0f, 1f));
				this.level.Session.Audio.Apply(false);
				if (this.level.CameraOffset.Y == -38.4f)
				{
					if (entity.StateMachine.State != 19)
					{
						this.cameraOffsetTimer += Engine.DeltaTime;
						if (this.cameraOffsetTimer >= 0.5f)
						{
							this.cameraOffsetTimer = 0f;
							this.level.CameraOffset.Y = -12.8f;
						}
					}
					else
					{
						this.cameraOffsetTimer = 0f;
					}
				}
				else if (entity.StateMachine.State == 19)
				{
					this.cameraOffsetTimer += Engine.DeltaTime;
					if (this.cameraOffsetTimer >= 0.1f)
					{
						this.cameraOffsetTimer = 0f;
						this.level.CameraOffset.Y = -38.4f;
					}
				}
				else
				{
					this.cameraOffsetTimer = 0f;
				}
				this.cameraOffsetMarker = this.level.Camera.Y;
			}
			else
			{
				this.level.Session.Audio.Music.Layer(1, 1f);
				this.level.Session.Audio.Music.Layer(2, 0f);
				this.level.Session.Audio.Apply(false);
			}
			this.UpdateBlockFill();
		}

		// Token: 0x06000F92 RID: 3986 RVA: 0x0004113C File Offset: 0x0003F33C
		private void InitBlockFill()
		{
			for (int i = 0; i < this.rays.Length; i++)
			{
				this.rays[i].Reset();
				this.rays[i].Percent = Calc.Random.NextFloat();
			}
		}

		// Token: 0x06000F93 RID: 3987 RVA: 0x00041188 File Offset: 0x0003F388
		private void UpdateBlockFill()
		{
			Level level = base.Scene as Level;
			Vector2 vector = Calc.AngleToVector(-1.6707964f, 1f);
			Vector2 value = new Vector2(-vector.Y, vector.X);
			int num = 0;
			for (int i = 0; i < this.rays.Length; i++)
			{
				if (this.rays[i].Percent >= 1f)
				{
					this.rays[i].Reset();
				}
				StarJumpController.Ray[] array = this.rays;
				int num2 = i;
				array[num2].Percent = array[num2].Percent + Engine.DeltaTime / this.rays[i].Duration;
				StarJumpController.Ray[] array2 = this.rays;
				int num3 = i;
				array2[num3].Y = array2[num3].Y + 8f * Engine.DeltaTime;
				float percent = this.rays[i].Percent;
				float num4 = this.mod(this.rays[i].X - level.Camera.X * 0.9f, 320f);
				float num5 = -200f + this.mod(this.rays[i].Y - level.Camera.Y * 0.7f, 580f);
				float width = this.rays[i].Width;
				float length = this.rays[i].Length;
				Vector2 value2 = new Vector2((float)((int)num4), (float)((int)num5));
				Color color = this.rayColor * Ease.CubeInOut(Calc.YoYo(percent));
				VertexPositionColor vertexPositionColor = new VertexPositionColor(new Vector3(value2 + value * width + vector * length, 0f), color);
				VertexPositionColor vertexPositionColor2 = new VertexPositionColor(new Vector3(value2 - value * width, 0f), color);
				VertexPositionColor vertexPositionColor3 = new VertexPositionColor(new Vector3(value2 + value * width, 0f), color);
				VertexPositionColor vertexPositionColor4 = new VertexPositionColor(new Vector3(value2 - value * width - vector * length, 0f), color);
				this.vertices[num++] = vertexPositionColor;
				this.vertices[num++] = vertexPositionColor2;
				this.vertices[num++] = vertexPositionColor3;
				this.vertices[num++] = vertexPositionColor2;
				this.vertices[num++] = vertexPositionColor3;
				this.vertices[num++] = vertexPositionColor4;
			}
			this.vertexCount = num;
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x00041444 File Offset: 0x0003F644
		private void BeforeRender()
		{
			if (this.BlockFill == null)
			{
				this.BlockFill = VirtualContent.CreateRenderTarget("block-fill", 320, 180, false, true, 0);
			}
			if (this.vertexCount > 0)
			{
				Engine.Graphics.GraphicsDevice.SetRenderTarget(this.BlockFill);
				Engine.Graphics.GraphicsDevice.Clear(Color.Lerp(Color.Black, Color.LightSkyBlue, 0.3f));
				GFX.DrawVertices<VertexPositionColor>(Matrix.Identity, this.vertices, this.vertexCount, null, null);
			}
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x000414D4 File Offset: 0x0003F6D4
		public override void Removed(Scene scene)
		{
			this.Dispose();
			base.Removed(scene);
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x000414E3 File Offset: 0x0003F6E3
		public override void SceneEnd(Scene scene)
		{
			this.Dispose();
			base.SceneEnd(scene);
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x000414F2 File Offset: 0x0003F6F2
		private void Dispose()
		{
			if (this.BlockFill != null)
			{
				this.BlockFill.Dispose();
			}
			this.BlockFill = null;
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x00026894 File Offset: 0x00024A94
		private float mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000AF4 RID: 2804
		private Level level;

		// Token: 0x04000AF5 RID: 2805
		private Random random;

		// Token: 0x04000AF6 RID: 2806
		private float minY;

		// Token: 0x04000AF7 RID: 2807
		private float maxY;

		// Token: 0x04000AF8 RID: 2808
		private float minX;

		// Token: 0x04000AF9 RID: 2809
		private float maxX;

		// Token: 0x04000AFA RID: 2810
		private float cameraOffsetMarker;

		// Token: 0x04000AFB RID: 2811
		private float cameraOffsetTimer;

		// Token: 0x04000AFC RID: 2812
		public VirtualRenderTarget BlockFill;

		// Token: 0x04000AFD RID: 2813
		private const int RayCount = 100;

		// Token: 0x04000AFE RID: 2814
		private VertexPositionColor[] vertices = new VertexPositionColor[600];

		// Token: 0x04000AFF RID: 2815
		private int vertexCount;

		// Token: 0x04000B00 RID: 2816
		private Color rayColor = Calc.HexToColor("a3ffff") * 0.25f;

		// Token: 0x04000B01 RID: 2817
		private StarJumpController.Ray[] rays = new StarJumpController.Ray[100];

		// Token: 0x020004C9 RID: 1225
		private struct Ray
		{
			// Token: 0x060023FF RID: 9215 RVA: 0x000F0DF4 File Offset: 0x000EEFF4
			public void Reset()
			{
				this.Percent = 0f;
				this.X = Calc.Random.NextFloat(320f);
				this.Y = Calc.Random.NextFloat(580f);
				this.Duration = 4f + Calc.Random.NextFloat() * 8f;
				this.Width = (float)Calc.Random.Next(8, 80);
				this.Length = (float)Calc.Random.Next(20, 200);
			}

			// Token: 0x04002394 RID: 9108
			public float X;

			// Token: 0x04002395 RID: 9109
			public float Y;

			// Token: 0x04002396 RID: 9110
			public float Percent;

			// Token: 0x04002397 RID: 9111
			public float Duration;

			// Token: 0x04002398 RID: 9112
			public float Width;

			// Token: 0x04002399 RID: 9113
			public float Length;
		}
	}
}
