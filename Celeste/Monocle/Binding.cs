using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Input;

namespace Monocle
{
	// Token: 0x020000E7 RID: 231
	[Serializable]
	public class Binding
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600051F RID: 1311 RVA: 0x000066D4 File Offset: 0x000048D4
		public bool HasInput
		{
			get
			{
				return this.Keyboard.Count > 0 || this.Controller.Count > 0;
			}
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x000066F4 File Offset: 0x000048F4
		public bool Add(params Keys[] keys)
		{
			bool result = false;
			foreach (Keys keys2 in keys)
			{
				if (!this.Keyboard.Contains(keys2))
				{
					using (List<Binding>.Enumerator enumerator = this.ExclusiveFrom.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.Needs(keys2))
							{
								goto IL_61;
							}
						}
					}
					this.Keyboard.Add(keys2);
					result = true;
				}
				IL_61:;
			}
			return result;
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x00006780 File Offset: 0x00004980
		public bool Add(params Buttons[] buttons)
		{
			bool result = false;
			foreach (Buttons buttons2 in buttons)
			{
				if (!this.Controller.Contains(buttons2))
				{
					using (List<Binding>.Enumerator enumerator = this.ExclusiveFrom.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.Needs(buttons2))
							{
								goto IL_61;
							}
						}
					}
					this.Controller.Add(buttons2);
					result = true;
				}
				IL_61:;
			}
			return result;
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x0000680C File Offset: 0x00004A0C
		public bool Needs(Buttons button)
		{
			if (!this.Controller.Contains(button))
			{
				return false;
			}
			if (this.Controller.Count <= 1)
			{
				return true;
			}
			if (!this.IsExclusive(button))
			{
				return false;
			}
			foreach (Buttons buttons in this.Controller)
			{
				if (buttons != button && this.IsExclusive(buttons))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x00006898 File Offset: 0x00004A98
		public bool Needs(Keys key)
		{
			if (!this.Keyboard.Contains(key))
			{
				return false;
			}
			if (this.Keyboard.Count <= 1)
			{
				return true;
			}
			if (!this.IsExclusive(key))
			{
				return false;
			}
			foreach (Keys keys in this.Keyboard)
			{
				if (keys != key && this.IsExclusive(keys))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x00006924 File Offset: 0x00004B24
		public bool IsExclusive(Buttons button)
		{
			using (List<Binding>.Enumerator enumerator = this.ExclusiveFrom.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Controller.Contains(button))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x00006984 File Offset: 0x00004B84
		public bool IsExclusive(Keys key)
		{
			using (List<Binding>.Enumerator enumerator = this.ExclusiveFrom.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Keyboard.Contains(key))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x000069E4 File Offset: 0x00004BE4
		public bool ClearKeyboard()
		{
			if (this.ExclusiveFrom.Count > 0)
			{
				if (this.Keyboard.Count <= 1)
				{
					return false;
				}
				int index = 0;
				for (int i = 1; i < this.Keyboard.Count; i++)
				{
					if (this.IsExclusive(this.Keyboard[i]))
					{
						index = i;
					}
				}
				Keys item = this.Keyboard[index];
				this.Keyboard.Clear();
				this.Keyboard.Add(item);
			}
			else
			{
				this.Keyboard.Clear();
			}
			return true;
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x00006A70 File Offset: 0x00004C70
		public bool ClearGamepad()
		{
			if (this.ExclusiveFrom.Count > 0)
			{
				if (this.Controller.Count <= 1)
				{
					return false;
				}
				int index = 0;
				for (int i = 1; i < this.Controller.Count; i++)
				{
					if (this.IsExclusive(this.Controller[i]))
					{
						index = i;
					}
				}
				Buttons item = this.Controller[index];
				this.Controller.Clear();
				this.Controller.Add(item);
			}
			else
			{
				this.Controller.Clear();
			}
			return true;
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x00006AFC File Offset: 0x00004CFC
		public float Axis(int gamepadIndex, float threshold)
		{
			foreach (Keys key in this.Keyboard)
			{
				if (MInput.Keyboard.Check(key))
				{
					return 1f;
				}
			}
			foreach (Buttons button in this.Controller)
			{
				float num = MInput.GamePads[gamepadIndex].Axis(button, threshold);
				if (num != 0f)
				{
					return num;
				}
			}
			return 0f;
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x00006BC0 File Offset: 0x00004DC0
		public bool Check(int gamepadIndex, float threshold)
		{
			for (int i = 0; i < this.Keyboard.Count; i++)
			{
				if (MInput.Keyboard.Check(this.Keyboard[i]))
				{
					return true;
				}
			}
			for (int j = 0; j < this.Controller.Count; j++)
			{
				if (MInput.GamePads[gamepadIndex].Check(this.Controller[j], threshold))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x00006C34 File Offset: 0x00004E34
		public bool Pressed(int gamepadIndex, float threshold)
		{
			for (int i = 0; i < this.Keyboard.Count; i++)
			{
				if (MInput.Keyboard.Pressed(this.Keyboard[i]))
				{
					return true;
				}
			}
			for (int j = 0; j < this.Controller.Count; j++)
			{
				if (MInput.GamePads[gamepadIndex].Pressed(this.Controller[j], threshold))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x00006CA8 File Offset: 0x00004EA8
		public bool Released(int gamepadIndex, float threshold)
		{
			for (int i = 0; i < this.Keyboard.Count; i++)
			{
				if (MInput.Keyboard.Released(this.Keyboard[i]))
				{
					return true;
				}
			}
			for (int j = 0; j < this.Controller.Count; j++)
			{
				if (MInput.GamePads[gamepadIndex].Released(this.Controller[j], threshold))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x00006D1C File Offset: 0x00004F1C
		public static void SetExclusive(params Binding[] list)
		{
			for (int i = 0; i < list.Length; i++)
			{
				list[i].ExclusiveFrom.Clear();
			}
			foreach (Binding binding in list)
			{
				foreach (Binding binding2 in list)
				{
					if (binding != binding2)
					{
						binding.ExclusiveFrom.Add(binding2);
						binding2.ExclusiveFrom.Add(binding);
					}
				}
			}
		}

		// Token: 0x0400048B RID: 1163
		public List<Keys> Keyboard = new List<Keys>();

		// Token: 0x0400048C RID: 1164
		public List<Buttons> Controller = new List<Buttons>();

		// Token: 0x0400048D RID: 1165
		[XmlIgnore]
		public List<Binding> ExclusiveFrom = new List<Binding>();
	}
}
