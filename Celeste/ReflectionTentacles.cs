using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020001C6 RID: 454
	[Tracked(false)]
	public class ReflectionTentacles : Entity
	{
		// Token: 0x06000F7E RID: 3966 RVA: 0x0003F937 File Offset: 0x0003DB37
		public ReflectionTentacles()
		{
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x0003F96C File Offset: 0x0003DB6C
		public ReflectionTentacles(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			this.Nodes.Add(this.Position);
			foreach (Vector2 value in data.Nodes)
			{
				this.Nodes.Add(offset + value);
			}
			string a = data.Attr("fear_distance", "");
			if (a == "close")
			{
				this.fearDistance = 16f;
			}
			else if (a == "medium")
			{
				this.fearDistance = 40f;
			}
			else if (a == "far")
			{
				this.fearDistance = 80f;
			}
			int num = data.Int("slide_until", 0);
			this.Create(this.fearDistance, num, 0, this.Nodes);
			this.createdFromLevel = true;
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x0003FA80 File Offset: 0x0003DC80
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (this.createdFromLevel)
			{
				for (int i = 1; i < 4; i++)
				{
					ReflectionTentacles reflectionTentacles = new ReflectionTentacles();
					reflectionTentacles.Create(this.fearDistance, this.slideUntilIndex, i, this.Nodes);
					scene.Add(reflectionTentacles);
				}
			}
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x0003FAD0 File Offset: 0x0003DCD0
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			bool flag = false;
			while (entity != null && this.Index < this.Nodes.Count - 1)
			{
				Vector2 value = this.p = Calc.ClosestPointOnLine(this.Nodes[this.Index], this.Nodes[this.Index] + this.outwards * 10000f, entity.Center);
				if ((this.Nodes[this.Index] - value).Length() >= this.fearDistance)
				{
					break;
				}
				flag = true;
				this.Retreat();
			}
			if (flag)
			{
				this.ease = 1f;
				this.SnapTentacles();
			}
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x0003FBA4 File Offset: 0x0003DDA4
		public void Create(float fearDistance, int slideUntilIndex, int layer, List<Vector2> startNodes)
		{
			this.Nodes = new List<Vector2>();
			foreach (Vector2 value in startNodes)
			{
				this.Nodes.Add(value + new Vector2((float)Calc.Random.Range(-8, 8), (float)Calc.Random.Range(-8, 8)));
			}
			base.Tag = Tags.TransitionUpdate;
			this.Position = this.Nodes[0];
			this.outwards = (this.Nodes[0] - this.Nodes[1]).SafeNormalize();
			this.fearDistance = fearDistance;
			this.slideUntilIndex = slideUntilIndex;
			this.layer = layer;
			switch (layer)
			{
			case 0:
				base.Depth = -1000000;
				this.color = Calc.HexToColor("3f2a4f");
				this.offset = 110f;
				break;
			case 1:
				base.Depth = 8990;
				this.color = Calc.HexToColor("7b3555");
				this.offset = 80f;
				break;
			case 2:
				base.Depth = 10010;
				this.color = Calc.HexToColor("662847");
				this.offset = 50f;
				break;
			case 3:
				base.Depth = 10011;
				this.color = Calc.HexToColor("492632");
				this.offset = 20f;
				break;
			}
			foreach (MTexture mtexture in GFX.Game.GetAtlasSubtextures("scenery/tentacles/arms"))
			{
				MTexture[] array = new MTexture[10];
				int num = mtexture.Width / 10;
				for (int i = 0; i < 10; i++)
				{
					array[i] = mtexture.GetSubtexture(num * (10 - i - 1), 0, num, mtexture.Height, null);
				}
				this.arms.Add(array);
			}
			this.fillers = GFX.Game.GetAtlasSubtextures("scenery/tentacles/filler");
			this.tentacles = new ReflectionTentacles.Tentacle[100];
			float num2 = 0f;
			int num3 = 0;
			while (num3 < this.tentacles.Length && num2 < 440f)
			{
				this.tentacles[num3].Approach = 0.25f + Calc.Random.NextFloat() * 0.75f;
				this.tentacles[num3].Length = 32f + Calc.Random.NextFloat(64f);
				this.tentacles[num3].Width = 4f + Calc.Random.NextFloat(16f);
				this.tentacles[num3].Position = this.TargetTentaclePosition(this.tentacles[num3], this.Nodes[0], num2);
				this.tentacles[num3].WaveOffset = Calc.Random.NextFloat();
				this.tentacles[num3].TexIndex = Calc.Random.Next(this.arms.Count);
				this.tentacles[num3].FillerTexIndex = Calc.Random.Next(this.fillers.Count);
				this.tentacles[num3].LerpDuration = 0.5f + Calc.Random.NextFloat(0.25f);
				num2 += this.tentacles[num3].Width;
				num3++;
				this.tentacleCount++;
			}
			this.vertices = new VertexPositionColorTexture[this.tentacleCount * 12 * 6];
			for (int j = 0; j < this.vertices.Length; j++)
			{
				this.vertices[j].Color = this.color;
			}
		}

		// Token: 0x06000F83 RID: 3971 RVA: 0x0003FFC8 File Offset: 0x0003E1C8
		private Vector2 TargetTentaclePosition(ReflectionTentacles.Tentacle tentacle, Vector2 position, float along)
		{
			Vector2 value2;
			Vector2 value = value2 = position - this.outwards * this.offset;
			if (this.player != null)
			{
				Vector2 value3 = this.outwards.Perpendicular();
				value2 = Calc.ClosestPointOnLine(value2 - value3 * 200f, value2 + value3 * 200f, this.player.Position);
			}
			Vector2 vector = value + this.outwards.Perpendicular() * (-220f + along + tentacle.Width * 0.5f);
			float scaleFactor = (value2 - vector).Length();
			return vector + this.outwards * scaleFactor * 0.6f;
		}

		// Token: 0x06000F84 RID: 3972 RVA: 0x0004008C File Offset: 0x0003E28C
		public void Retreat()
		{
			if (this.Index < this.Nodes.Count - 1)
			{
				this.lastOutwards = this.outwards;
				this.ease = 0f;
				this.Index++;
				if (this.layer == 0 && this.soundDelay <= 0f)
				{
					Audio.Play(((this.Nodes[this.Index - 1] - this.Nodes[this.Index]).Length() > 180f) ? "event:/game/06_reflection/scaryhair_whoosh" : "event:/game/06_reflection/scaryhair_move");
				}
				for (int i = 0; i < this.tentacleCount; i++)
				{
					this.tentacles[i].LerpPercent = 0f;
					this.tentacles[i].LerpPositionFrom = this.tentacles[i].Position;
				}
			}
		}

		// Token: 0x06000F85 RID: 3973 RVA: 0x00040180 File Offset: 0x0003E380
		public override void Update()
		{
			this.soundDelay -= Engine.DeltaTime;
			if (this.slideUntilIndex > this.Index)
			{
				this.player = base.Scene.Tracker.GetEntity<Player>();
				if (this.player != null)
				{
					Vector2 value = this.p = Calc.ClosestPointOnLine(this.Nodes[this.Index] - this.outwards * 10000f, this.Nodes[this.Index] + this.outwards * 10000f, this.player.Center);
					if ((value - this.Nodes[this.Index]).Length() < 32f)
					{
						this.Retreat();
						this.outwards = (this.Nodes[this.Index - 1] - this.Nodes[this.Index]).SafeNormalize();
					}
					else
					{
						this.MoveTentacles(value - this.outwards * 190f);
					}
				}
			}
			else
			{
				bool entity = base.Scene.Tracker.GetEntity<FinalBoss>() != null;
				this.player = base.Scene.Tracker.GetEntity<Player>();
				if (!entity && this.player != null && this.Index < this.Nodes.Count - 1)
				{
					Vector2 value2 = this.p = Calc.ClosestPointOnLine(this.Nodes[this.Index], this.Nodes[this.Index] + this.outwards * 10000f, this.player.Center);
					if ((this.Nodes[this.Index] - value2).Length() < this.fearDistance)
					{
						this.Retreat();
					}
				}
				if (this.Index > 0)
				{
					this.ease = Calc.Approach(this.ease, 1f, (float)((this.Index == this.Nodes.Count - 1) ? 2 : 1) * Engine.DeltaTime);
					this.outwards = Calc.AngleToVector(Calc.AngleLerp(this.lastOutwards.Angle(), (this.Nodes[this.Index - 1] - this.Nodes[this.Index]).Angle(), Ease.QuadOut(this.ease)), 1f);
					float num = 0f;
					for (int i = 0; i < this.tentacleCount; i++)
					{
						Vector2 vector = this.TargetTentaclePosition(this.tentacles[i], this.Nodes[this.Index], num);
						if (this.tentacles[i].LerpPercent < 1f)
						{
							ReflectionTentacles.Tentacle[] array = this.tentacles;
							int num2 = i;
							array[num2].LerpPercent = array[num2].LerpPercent + Engine.DeltaTime / this.tentacles[i].LerpDuration;
							this.tentacles[i].Position = Vector2.Lerp(this.tentacles[i].LerpPositionFrom, vector, Ease.CubeInOut(this.tentacles[i].LerpPercent));
						}
						else
						{
							ReflectionTentacles.Tentacle[] array2 = this.tentacles;
							int num3 = i;
							array2[num3].Position = array2[num3].Position + (vector - this.tentacles[i].Position) * (1f - (float)Math.Pow((double)(0.1f * this.tentacles[i].Approach), (double)Engine.DeltaTime));
						}
						num += this.tentacles[i].Width;
					}
				}
				else
				{
					this.MoveTentacles(this.Nodes[this.Index]);
				}
			}
			if (this.Index == this.Nodes.Count - 1)
			{
				Color color = this.color * (1f - this.ease);
				for (int j = 0; j < this.vertices.Length; j++)
				{
					this.vertices[j].Color = color;
				}
			}
			this.UpdateVertices();
		}

		// Token: 0x06000F86 RID: 3974 RVA: 0x000405F8 File Offset: 0x0003E7F8
		private void MoveTentacles(Vector2 pos)
		{
			float num = 0f;
			for (int i = 0; i < this.tentacleCount; i++)
			{
				Vector2 value = this.TargetTentaclePosition(this.tentacles[i], pos, num);
				ReflectionTentacles.Tentacle[] array = this.tentacles;
				int num2 = i;
				array[num2].Position = array[num2].Position + (value - this.tentacles[i].Position) * (1f - (float)Math.Pow((double)(0.1f * this.tentacles[i].Approach), (double)Engine.DeltaTime));
				num += this.tentacles[i].Width;
			}
		}

		// Token: 0x06000F87 RID: 3975 RVA: 0x000406B4 File Offset: 0x0003E8B4
		public void SnapTentacles()
		{
			float num = 0f;
			for (int i = 0; i < this.tentacleCount; i++)
			{
				this.tentacles[i].LerpPercent = 1f;
				this.tentacles[i].Position = this.TargetTentaclePosition(this.tentacles[i], this.Nodes[this.Index], num);
				num += this.tentacles[i].Width;
			}
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x00040738 File Offset: 0x0003E938
		private void UpdateVertices()
		{
			Vector2 value = -this.outwards.Perpendicular();
			int num = 0;
			for (int i = 0; i < this.tentacleCount; i++)
			{
				Vector2 position = this.tentacles[i].Position;
				Vector2 vector = value * (this.tentacles[i].Width * 0.5f + 2f);
				MTexture[] array = this.arms[this.tentacles[i].TexIndex];
				this.Quad(ref num, position + vector, position + vector * 1.5f - this.outwards * 240f, position - vector * 1.5f - this.outwards * 240f, position - vector, this.fillers[this.tentacles[i].FillerTexIndex]);
				Vector2 value2 = position;
				Vector2 value3 = vector;
				float num2 = this.tentacles[i].Length / 10f;
				num2 += Calc.YoYo(this.tentacles[i].LerpPercent) * 6f;
				for (int j = 1; j <= 10; j++)
				{
					float num3 = (float)j / 10f;
					float num4 = base.Scene.TimeActive * this.tentacles[i].WaveOffset * (float)Math.Pow(1.100000023841858, (double)j) * 2f;
					float num5 = this.tentacles[i].WaveOffset * 3f + (float)j * 0.05f;
					float scaleFactor = 2f + 4f * num3;
					Vector2 value4 = value * (float)Math.Sin((double)(num4 + num5)) * scaleFactor;
					Vector2 vector2 = value2 + this.outwards * num2 + value4;
					Vector2 vector3 = vector * (1f - num3);
					this.Quad(ref num, vector2 - vector3, value2 - value3, value2 + value3, vector2 + vector3, array[j - 1]);
					value2 = vector2;
					value3 = vector3;
				}
			}
			this.vertexCount = num;
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x000409A0 File Offset: 0x0003EBA0
		private void Quad(ref int n, Vector2 a, Vector2 b, Vector2 c, Vector2 d, MTexture subtexture = null)
		{
			if (subtexture == null)
			{
				subtexture = GFX.Game["util/pixel"];
			}
			float num = 1f / (float)subtexture.Texture.Texture.Width;
			float num2 = 1f / (float)subtexture.Texture.Texture.Height;
			Vector2 textureCoordinate = new Vector2((float)subtexture.ClipRect.Left * num, (float)subtexture.ClipRect.Top * num2);
			Vector2 textureCoordinate2 = new Vector2((float)subtexture.ClipRect.Right * num, (float)subtexture.ClipRect.Top * num2);
			Vector2 textureCoordinate3 = new Vector2((float)subtexture.ClipRect.Left * num, (float)subtexture.ClipRect.Bottom * num2);
			Vector2 textureCoordinate4 = new Vector2((float)subtexture.ClipRect.Right * num, (float)subtexture.ClipRect.Bottom * num2);
			this.vertices[n].Position = new Vector3(a, 0f);
			VertexPositionColorTexture[] array = this.vertices;
			int num3 = n;
			n = num3 + 1;
			array[num3].TextureCoordinate = textureCoordinate;
			this.vertices[n].Position = new Vector3(b, 0f);
			VertexPositionColorTexture[] array2 = this.vertices;
			num3 = n;
			n = num3 + 1;
			array2[num3].TextureCoordinate = textureCoordinate2;
			this.vertices[n].Position = new Vector3(d, 0f);
			VertexPositionColorTexture[] array3 = this.vertices;
			num3 = n;
			n = num3 + 1;
			array3[num3].TextureCoordinate = textureCoordinate3;
			this.vertices[n].Position = new Vector3(d, 0f);
			VertexPositionColorTexture[] array4 = this.vertices;
			num3 = n;
			n = num3 + 1;
			array4[num3].TextureCoordinate = textureCoordinate3;
			this.vertices[n].Position = new Vector3(b, 0f);
			VertexPositionColorTexture[] array5 = this.vertices;
			num3 = n;
			n = num3 + 1;
			array5[num3].TextureCoordinate = textureCoordinate2;
			this.vertices[n].Position = new Vector3(c, 0f);
			VertexPositionColorTexture[] array6 = this.vertices;
			num3 = n;
			n = num3 + 1;
			array6[num3].TextureCoordinate = textureCoordinate4;
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x00040C0C File Offset: 0x0003EE0C
		public override void Render()
		{
			if (this.vertexCount > 0)
			{
				GameplayRenderer.End();
				Engine.Graphics.GraphicsDevice.Textures[0] = this.arms[0][0].Texture.Texture;
				GFX.DrawVertices<VertexPositionColorTexture>((base.Scene as Level).Camera.Matrix, this.vertices, this.vertexCount, GFX.FxTexture, null);
				GameplayRenderer.Begin();
			}
		}

		// Token: 0x04000ADE RID: 2782
		public int Index;

		// Token: 0x04000ADF RID: 2783
		public List<Vector2> Nodes = new List<Vector2>();

		// Token: 0x04000AE0 RID: 2784
		private Vector2 outwards;

		// Token: 0x04000AE1 RID: 2785
		private Vector2 lastOutwards;

		// Token: 0x04000AE2 RID: 2786
		private float ease;

		// Token: 0x04000AE3 RID: 2787
		private Vector2 p;

		// Token: 0x04000AE4 RID: 2788
		private Player player;

		// Token: 0x04000AE5 RID: 2789
		private float fearDistance;

		// Token: 0x04000AE6 RID: 2790
		private float offset;

		// Token: 0x04000AE7 RID: 2791
		private bool createdFromLevel;

		// Token: 0x04000AE8 RID: 2792
		private int slideUntilIndex;

		// Token: 0x04000AE9 RID: 2793
		private int layer;

		// Token: 0x04000AEA RID: 2794
		private const int NodesPerTentacle = 10;

		// Token: 0x04000AEB RID: 2795
		private ReflectionTentacles.Tentacle[] tentacles;

		// Token: 0x04000AEC RID: 2796
		private int tentacleCount;

		// Token: 0x04000AED RID: 2797
		private VertexPositionColorTexture[] vertices;

		// Token: 0x04000AEE RID: 2798
		private int vertexCount;

		// Token: 0x04000AEF RID: 2799
		private Color color = Color.Purple;

		// Token: 0x04000AF0 RID: 2800
		private float soundDelay = 0.25f;

		// Token: 0x04000AF1 RID: 2801
		private List<MTexture[]> arms = new List<MTexture[]>();

		// Token: 0x04000AF2 RID: 2802
		private List<MTexture> fillers;

		// Token: 0x020004C6 RID: 1222
		private struct Tentacle
		{
			// Token: 0x04002382 RID: 9090
			public Vector2 Position;

			// Token: 0x04002383 RID: 9091
			public float Width;

			// Token: 0x04002384 RID: 9092
			public float Length;

			// Token: 0x04002385 RID: 9093
			public float Approach;

			// Token: 0x04002386 RID: 9094
			public float WaveOffset;

			// Token: 0x04002387 RID: 9095
			public int TexIndex;

			// Token: 0x04002388 RID: 9096
			public int FillerTexIndex;

			// Token: 0x04002389 RID: 9097
			public Vector2 LerpPositionFrom;

			// Token: 0x0400238A RID: 9098
			public float LerpPercent;

			// Token: 0x0400238B RID: 9099
			public float LerpDuration;
		}
	}
}
