using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000230 RID: 560
	public class ObjModel : IDisposable
	{
		// Token: 0x060011D3 RID: 4563 RVA: 0x00059208 File Offset: 0x00057408
		private bool ResetVertexBuffer()
		{
			if (this.Vertices == null || this.Vertices.IsDisposed || this.Vertices.GraphicsDevice.IsDisposed)
			{
				this.Vertices = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexPositionTexture), this.verts.Length, BufferUsage.None);
				this.Vertices.SetData<VertexPositionTexture>(this.verts);
				return true;
			}
			return false;
		}

		// Token: 0x060011D4 RID: 4564 RVA: 0x00059278 File Offset: 0x00057478
		public void ReassignVertices()
		{
			if (!this.ResetVertexBuffer())
			{
				this.Vertices.SetData<VertexPositionTexture>(this.verts);
			}
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x00059294 File Offset: 0x00057494
		public void Draw(Effect effect)
		{
			this.ResetVertexBuffer();
			Engine.Graphics.GraphicsDevice.SetVertexBuffer(this.Vertices);
			foreach (EffectPass effectPass in effect.CurrentTechnique.Passes)
			{
				effectPass.Apply();
				Engine.Graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.Vertices.VertexCount / 3);
			}
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x00059324 File Offset: 0x00057524
		public void Dispose()
		{
			this.Vertices.Dispose();
			this.Meshes = null;
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x00059338 File Offset: 0x00057538
		public static ObjModel Create(string filename)
		{
			Path.GetDirectoryName(filename);
			ObjModel objModel = new ObjModel();
			List<VertexPositionTexture> list = new List<VertexPositionTexture>();
			List<Vector3> list2 = new List<Vector3>();
			List<Vector2> list3 = new List<Vector2>();
			ObjModel.Mesh mesh = null;
			if (File.Exists(filename + ".export"))
			{
				using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(filename + ".export")))
				{
					int num = binaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						if (mesh != null)
						{
							mesh.VertexCount = list.Count - mesh.VertexStart;
						}
						mesh = new ObjModel.Mesh();
						mesh.Name = binaryReader.ReadString();
						mesh.VertexStart = list.Count;
						objModel.Meshes.Add(mesh);
						int num2 = binaryReader.ReadInt32();
						for (int j = 0; j < num2; j++)
						{
							float x = binaryReader.ReadSingle();
							float y = binaryReader.ReadSingle();
							float z = binaryReader.ReadSingle();
							list2.Add(new Vector3(x, y, z));
						}
						int num3 = binaryReader.ReadInt32();
						for (int k = 0; k < num3; k++)
						{
							float x2 = binaryReader.ReadSingle();
							float y2 = binaryReader.ReadSingle();
							list3.Add(new Vector2(x2, y2));
						}
						int num4 = binaryReader.ReadInt32();
						for (int l = 0; l < num4; l++)
						{
							int index = binaryReader.ReadInt32() - 1;
							int index2 = binaryReader.ReadInt32() - 1;
							list.Add(new VertexPositionTexture
							{
								Position = list2[index],
								TextureCoordinate = list3[index2]
							});
						}
					}
					goto IL_357;
				}
			}
			using (StreamReader streamReader = new StreamReader(filename))
			{
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					string[] array = text.Split(new char[]
					{
						' '
					});
					if (array.Length != 0)
					{
						string a = array[0];
						if (a == "o")
						{
							if (mesh != null)
							{
								mesh.VertexCount = list.Count - mesh.VertexStart;
							}
							mesh = new ObjModel.Mesh();
							mesh.Name = array[1];
							mesh.VertexStart = list.Count;
							objModel.Meshes.Add(mesh);
						}
						else if (a == "v")
						{
							Vector3 item = new Vector3(ObjModel.Float(array[1]), ObjModel.Float(array[2]), ObjModel.Float(array[3]));
							list2.Add(item);
						}
						else if (a == "vt")
						{
							Vector2 item2 = new Vector2(ObjModel.Float(array[1]), ObjModel.Float(array[2]));
							list3.Add(item2);
						}
						else if (a == "f")
						{
							for (int m = 1; m < Math.Min(4, array.Length); m++)
							{
								VertexPositionTexture item3 = default(VertexPositionTexture);
								string[] array2 = array[m].Split(new char[]
								{
									'/'
								});
								if (array2[0].Length > 0)
								{
									item3.Position = list2[int.Parse(array2[0]) - 1];
								}
								if (array2[1].Length > 0)
								{
									item3.TextureCoordinate = list3[int.Parse(array2[1]) - 1];
								}
								list.Add(item3);
							}
						}
					}
				}
			}
			IL_357:
			if (mesh != null)
			{
				mesh.VertexCount = list.Count - mesh.VertexStart;
			}
			objModel.verts = list.ToArray();
			objModel.ResetVertexBuffer();
			return objModel;
		}

		// Token: 0x060011D8 RID: 4568 RVA: 0x00059700 File Offset: 0x00057900
		private static float Float(string data)
		{
			return float.Parse(data, CultureInfo.InvariantCulture);
		}

		// Token: 0x04000D72 RID: 3442
		public List<ObjModel.Mesh> Meshes = new List<ObjModel.Mesh>();

		// Token: 0x04000D73 RID: 3443
		public VertexBuffer Vertices;

		// Token: 0x04000D74 RID: 3444
		private VertexPositionTexture[] verts;

		// Token: 0x0200054E RID: 1358
		public class Mesh
		{
			// Token: 0x040025E9 RID: 9705
			public string Name = "";

			// Token: 0x040025EA RID: 9706
			public ObjModel Model;

			// Token: 0x040025EB RID: 9707
			public int VertexStart;

			// Token: 0x040025EC RID: 9708
			public int VertexCount;
		}
	}
}
