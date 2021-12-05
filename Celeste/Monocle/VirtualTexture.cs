using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x0200013B RID: 315
	public class VirtualTexture : VirtualAsset
	{
		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000B6D RID: 2925 RVA: 0x00020422 File Offset: 0x0001E622
		// (set) Token: 0x06000B6E RID: 2926 RVA: 0x0002042A File Offset: 0x0001E62A
		public string Path { get; private set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000B6F RID: 2927 RVA: 0x00020433 File Offset: 0x0001E633
		public bool IsDisposed
		{
			get
			{
				return this.Texture == null || this.Texture.IsDisposed || this.Texture.GraphicsDevice.IsDisposed;
			}
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x0002045C File Offset: 0x0001E65C
		internal VirtualTexture(string path)
		{
			this.Path = path;
			base.Name = path;
			this.Reload();
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x00020485 File Offset: 0x0001E685
		internal VirtualTexture(string name, int width, int height, Color color)
		{
			base.Name = name;
			base.Width = width;
			base.Height = height;
			this.color = color;
			this.Reload();
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x000204B0 File Offset: 0x0001E6B0
		internal override void Unload()
		{
			if (this.Texture != null && !this.Texture.IsDisposed)
			{
				this.Texture.Dispose();
			}
			this.Texture = null;
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x000204DC File Offset: 0x0001E6DC
		internal unsafe override void Reload()
		{
			this.Unload();
			if (string.IsNullOrEmpty(this.Path))
			{
				this.Texture = new Texture2D(Engine.Instance.GraphicsDevice, base.Width, base.Height);
				Color[] array = new Color[base.Width * base.Height];
				Color[] array2;
				Color* ptr;
				if ((array2 = array) == null || array2.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array2[0];
				}
				for (int i = 0; i < array.Length; i++)
				{
					ptr[i] = this.color;
				}
				array2 = null;
				this.Texture.SetData<Color>(array);
				return;
			}
			string extension = System.IO.Path.GetExtension(this.Path);
			if (extension == ".data")
			{
				using (FileStream fileStream = File.OpenRead(System.IO.Path.Combine(Engine.ContentDirectory, this.Path)))
				{
					fileStream.Read(VirtualTexture.bytes, 0, 524288);
					int num = 0;
					int num2 = BitConverter.ToInt32(VirtualTexture.bytes, num);
					int num3 = BitConverter.ToInt32(VirtualTexture.bytes, num + 4);
					bool flag = VirtualTexture.bytes[num + 8] == 1;
					num += 9;
					int num4 = num2 * num3 * 4;
					int j = 0;
					try
					{
						byte[] array3;
						byte* ptr2;
						if ((array3 = VirtualTexture.bytes) == null || array3.Length == 0)
						{
							ptr2 = null;
						}
						else
						{
							ptr2 = &array3[0];
						}
						byte[] array4;
						byte* ptr3;
						if ((array4 = VirtualTexture.buffer) == null || array4.Length == 0)
						{
							ptr3 = null;
						}
						else
						{
							ptr3 = &array4[0];
						}
						while (j < num4)
						{
							int num5 = (int)(ptr2[num] * 4);
							if (flag)
							{
								byte b = ptr2[num + 1];
								if (b > 0)
								{
									ptr3[j] = ptr2[num + 4];
									ptr3[j + 1] = ptr2[num + 3];
									ptr3[j + 2] = ptr2[num + 2];
									ptr3[j + 3] = b;
									num += 5;
								}
								else
								{
									ptr3[j] = 0;
									ptr3[j + 1] = 0;
									ptr3[j + 2] = 0;
									ptr3[j + 3] = 0;
									num += 2;
								}
							}
							else
							{
								ptr3[j] = ptr2[num + 3];
								ptr3[j + 1] = ptr2[num + 2];
								ptr3[j + 2] = ptr2[num + 1];
								ptr3[j + 3] = byte.MaxValue;
								num += 4;
							}
							if (num5 > 4)
							{
								int k = j + 4;
								int num6 = j + num5;
								while (k < num6)
								{
									ptr3[k] = ptr3[j];
									ptr3[k + 1] = ptr3[j + 1];
									ptr3[k + 2] = ptr3[j + 2];
									ptr3[k + 3] = ptr3[j + 3];
									k += 4;
								}
							}
							j += num5;
							if (num > 524256)
							{
								int num7 = 524288 - num;
								for (int l = 0; l < num7; l++)
								{
									ptr2[l] = ptr2[num + l];
								}
								fileStream.Read(VirtualTexture.bytes, num7, 524288 - num7);
								num = 0;
							}
						}
					}
					finally
					{
						byte[] array3 = null;
						byte[] array4 = null;
					}
					this.Texture = new Texture2D(Engine.Graphics.GraphicsDevice, num2, num3);
					this.Texture.SetData<byte>(VirtualTexture.buffer, 0, num4);
					goto IL_52D;
				}
			}
			if (extension == ".png")
			{
				using (FileStream fileStream2 = File.OpenRead(System.IO.Path.Combine(Engine.ContentDirectory, this.Path)))
				{
					this.Texture = Texture2D.FromStream(Engine.Graphics.GraphicsDevice, fileStream2);
				}
				int num8 = this.Texture.Width * this.Texture.Height;
				Color[] array5 = new Color[num8];
				this.Texture.GetData<Color>(array5, 0, num8);
				Color[] array2;
				Color* ptr4;
				if ((array2 = array5) == null || array2.Length == 0)
				{
					ptr4 = null;
				}
				else
				{
					ptr4 = &array2[0];
				}
				for (int m = 0; m < num8; m++)
				{
					ptr4[m].R = (byte)((float)ptr4[m].R * ((float)ptr4[m].A / 255f));
					ptr4[m].G = (byte)((float)ptr4[m].G * ((float)ptr4[m].A / 255f));
					ptr4[m].B = (byte)((float)ptr4[m].B * ((float)ptr4[m].A / 255f));
				}
				array2 = null;
				this.Texture.SetData<Color>(array5, 0, num8);
			}
			else if (extension == ".xnb")
			{
				string assetName = this.Path.Replace(".xnb", "");
				this.Texture = Engine.Instance.Content.Load<Texture2D>(assetName);
			}
			else
			{
				using (FileStream fileStream3 = File.OpenRead(System.IO.Path.Combine(Engine.ContentDirectory, this.Path)))
				{
					this.Texture = Texture2D.FromStream(Engine.Graphics.GraphicsDevice, fileStream3);
				}
			}
			IL_52D:
			base.Width = this.Texture.Width;
			base.Height = this.Texture.Height;
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x00020A9C File Offset: 0x0001EC9C
		public override void Dispose()
		{
			this.Unload();
			this.Texture = null;
			VirtualContent.Remove(this);
		}

		// Token: 0x040006C9 RID: 1737
		private const int ByteArraySize = 524288;

		// Token: 0x040006CA RID: 1738
		private const int ByteArrayCheckSize = 524256;

		// Token: 0x040006CB RID: 1739
		internal static readonly byte[] buffer = new byte[67108864];

		// Token: 0x040006CC RID: 1740
		internal static readonly byte[] bytes = new byte[524288];

		// Token: 0x040006CD RID: 1741
		public Texture2D Texture;

		// Token: 0x040006CF RID: 1743
		private Color color;
	}
}
