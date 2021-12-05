using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200036F RID: 879
	public class EntityData
	{
		// Token: 0x06001B96 RID: 7062 RVA: 0x000B5B6C File Offset: 0x000B3D6C
		public Vector2[] NodesOffset(Vector2 offset)
		{
			if (this.Nodes == null)
			{
				return null;
			}
			Vector2[] array = new Vector2[this.Nodes.Length];
			for (int i = 0; i < this.Nodes.Length; i++)
			{
				array[i] = this.Nodes[i] + offset;
			}
			return array;
		}

		// Token: 0x06001B97 RID: 7063 RVA: 0x000B5BC0 File Offset: 0x000B3DC0
		public Vector2[] NodesWithPosition(Vector2 offset)
		{
			if (this.Nodes == null)
			{
				return new Vector2[]
				{
					this.Position + offset
				};
			}
			Vector2[] array = new Vector2[this.Nodes.Length + 1];
			array[0] = this.Position + offset;
			for (int i = 0; i < this.Nodes.Length; i++)
			{
				array[i + 1] = this.Nodes[i] + offset;
			}
			return array;
		}

		// Token: 0x06001B98 RID: 7064 RVA: 0x000B5C41 File Offset: 0x000B3E41
		public bool Has(string key)
		{
			return this.Values.ContainsKey(key);
		}

		// Token: 0x06001B99 RID: 7065 RVA: 0x000B5C50 File Offset: 0x000B3E50
		public string Attr(string key, string defaultValue = "")
		{
			object obj;
			if (this.Values != null && this.Values.TryGetValue(key, out obj))
			{
				return obj.ToString();
			}
			return defaultValue;
		}

		// Token: 0x06001B9A RID: 7066 RVA: 0x000B5C80 File Offset: 0x000B3E80
		public float Float(string key, float defaultValue = 0f)
		{
			object obj;
			if (this.Values != null && this.Values.TryGetValue(key, out obj))
			{
				if (obj is float)
				{
					return (float)obj;
				}
				float result;
				if (float.TryParse(obj.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
			}
			return defaultValue;
		}

		// Token: 0x06001B9B RID: 7067 RVA: 0x000B5CD0 File Offset: 0x000B3ED0
		public bool Bool(string key, bool defaultValue = false)
		{
			object obj;
			if (this.Values != null && this.Values.TryGetValue(key, out obj))
			{
				if (obj is bool)
				{
					return (bool)obj;
				}
				bool result;
				if (bool.TryParse(obj.ToString(), out result))
				{
					return result;
				}
			}
			return defaultValue;
		}

		// Token: 0x06001B9C RID: 7068 RVA: 0x000B5D18 File Offset: 0x000B3F18
		public int Int(string key, int defaultValue = 0)
		{
			object obj;
			if (this.Values != null && this.Values.TryGetValue(key, out obj))
			{
				if (obj is int)
				{
					return (int)obj;
				}
				int result;
				if (int.TryParse(obj.ToString(), out result))
				{
					return result;
				}
			}
			return defaultValue;
		}

		// Token: 0x06001B9D RID: 7069 RVA: 0x000B5D60 File Offset: 0x000B3F60
		public char Char(string key, char defaultValue = '\0')
		{
			object obj;
			char result;
			if (this.Values != null && this.Values.TryGetValue(key, out obj) && char.TryParse(obj.ToString(), out result))
			{
				return result;
			}
			return defaultValue;
		}

		// Token: 0x06001B9E RID: 7070 RVA: 0x000B5D98 File Offset: 0x000B3F98
		public Vector2? FirstNodeNullable(Vector2? offset = null)
		{
			if (this.Nodes == null || this.Nodes.Length == 0)
			{
				return null;
			}
			if (offset != null)
			{
				return new Vector2?(this.Nodes[0] + offset.Value);
			}
			return new Vector2?(this.Nodes[0]);
		}

		// Token: 0x06001B9F RID: 7071 RVA: 0x000B5DF8 File Offset: 0x000B3FF8
		public T Enum<T>(string key, T defaultValue = default(T)) where T : struct
		{
			object obj;
			T result;
			if (this.Values != null && this.Values.TryGetValue(key, out obj) && System.Enum.TryParse<T>(obj.ToString(), true, out result))
			{
				return result;
			}
			return defaultValue;
		}

		// Token: 0x06001BA0 RID: 7072 RVA: 0x000B5E30 File Offset: 0x000B4030
		public Color HexColor(string key, Color defaultValue = default(Color))
		{
			object obj;
			if (this.Values.TryGetValue(key, out obj))
			{
				string text = obj.ToString();
				if (text.Length == 6)
				{
					return Calc.HexToColor(text);
				}
			}
			return defaultValue;
		}

		// Token: 0x040018A2 RID: 6306
		public int ID;

		// Token: 0x040018A3 RID: 6307
		public string Name;

		// Token: 0x040018A4 RID: 6308
		public LevelData Level;

		// Token: 0x040018A5 RID: 6309
		public Vector2 Position;

		// Token: 0x040018A6 RID: 6310
		public Vector2 Origin;

		// Token: 0x040018A7 RID: 6311
		public int Width;

		// Token: 0x040018A8 RID: 6312
		public int Height;

		// Token: 0x040018A9 RID: 6313
		public Vector2[] Nodes;

		// Token: 0x040018AA RID: 6314
		public Dictionary<string, object> Values;
	}
}
