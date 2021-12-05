using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Monocle
{
	// Token: 0x02000127 RID: 295
	public class Commands
	{
		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000A88 RID: 2696 RVA: 0x0001A6BF File Offset: 0x000188BF
		// (set) Token: 0x06000A89 RID: 2697 RVA: 0x0001A6C7 File Offset: 0x000188C7
		public Action[] FunctionKeyActions { get; private set; }

		// Token: 0x06000A8A RID: 2698 RVA: 0x0001A6D0 File Offset: 0x000188D0
		public Commands()
		{
			this.commandHistory = new List<string>();
			this.drawCommands = new List<Commands.Line>();
			this.commands = new Dictionary<string, Commands.CommandInfo>();
			this.sorted = new List<string>();
			this.FunctionKeyActions = new Action[12];
			this.BuildCommandsList();
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x0001A744 File Offset: 0x00018944
		public void Log(object obj, Color color)
		{
			string text = obj.ToString();
			if (text.Contains("\n"))
			{
				foreach (string obj2 in text.Split(new char[]
				{
					'\n'
				}))
				{
					this.Log(obj2, color);
				}
				return;
			}
			int num = Engine.Instance.Window.ClientBounds.Width - 40;
			while (Draw.DefaultFont.MeasureString(text).X > (float)num)
			{
				int num2 = -1;
				for (int j = 0; j < text.Length; j++)
				{
					if (text[j] == ' ')
					{
						if (Draw.DefaultFont.MeasureString(text.Substring(0, j)).X > (float)num)
						{
							break;
						}
						num2 = j;
					}
				}
				if (num2 == -1)
				{
					break;
				}
				this.drawCommands.Insert(0, new Commands.Line(text.Substring(0, num2), color));
				text = text.Substring(num2 + 1);
			}
			this.drawCommands.Insert(0, new Commands.Line(text, color));
			int num3 = (Engine.Instance.Window.ClientBounds.Height - 100) / 30;
			while (this.drawCommands.Count > num3)
			{
				this.drawCommands.RemoveAt(this.drawCommands.Count - 1);
			}
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x0001A88F File Offset: 0x00018A8F
		public void Log(object obj)
		{
			this.Log(obj, Color.White);
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x0001A8A0 File Offset: 0x00018AA0
		internal void UpdateClosed()
		{
			if (!this.canOpen)
			{
				this.canOpen = true;
			}
			else if (MInput.Keyboard.Pressed(Keys.OemTilde, Keys.Oem8))
			{
				this.Open = true;
				this.currentState = Keyboard.GetState();
			}
			for (int i = 0; i < this.FunctionKeyActions.Length; i++)
			{
				if (MInput.Keyboard.Pressed(Keys.F1 + i))
				{
					this.ExecuteFunctionKeyAction(i);
				}
			}
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x0001A910 File Offset: 0x00018B10
		internal void UpdateOpen()
		{
			this.oldState = this.currentState;
			this.currentState = Keyboard.GetState();
			this.underscoreCounter += Engine.DeltaTime;
			while (this.underscoreCounter >= 0.5f)
			{
				this.underscoreCounter -= 0.5f;
				this.underscore = !this.underscore;
			}
			if (this.repeatKey != null)
			{
				if (this.currentState[this.repeatKey.Value] == KeyState.Down)
				{
					this.repeatCounter += Engine.DeltaTime;
					while (this.repeatCounter >= 0.5f)
					{
						this.HandleKey(this.repeatKey.Value);
						this.repeatCounter -= 0.033333335f;
					}
				}
				else
				{
					this.repeatKey = null;
				}
			}
			foreach (Keys key in this.currentState.GetPressedKeys())
			{
				if (this.oldState[key] == KeyState.Up)
				{
					this.HandleKey(key);
					return;
				}
			}
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x0001AA24 File Offset: 0x00018C24
		private void HandleKey(Keys key)
		{
			if (key != Keys.Tab && key != Keys.LeftShift && key != Keys.RightShift && key != Keys.RightAlt && key != Keys.LeftAlt && key != Keys.RightControl && key != Keys.LeftControl)
			{
				this.tabIndex = -1;
			}
			if (key != Keys.OemTilde && key != Keys.Oem8 && key != Keys.Enter)
			{
				Keys? keys = this.repeatKey;
				Keys keys2 = key;
				if (!(keys.GetValueOrDefault() == keys2 & keys != null))
				{
					this.repeatKey = new Keys?(key);
					this.repeatCounter = 0f;
				}
			}
			if (key <= Keys.Enter)
			{
				if (key != Keys.Back)
				{
					if (key != Keys.Tab)
					{
						if (key == Keys.Enter)
						{
							if (this.currentText.Length > 0)
							{
								this.EnterCommand();
								return;
							}
							return;
						}
					}
					else
					{
						if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
						{
							if (this.tabIndex == -1)
							{
								this.tabSearch = this.currentText;
								this.FindLastTab();
							}
							else
							{
								this.tabIndex--;
								if (this.tabIndex < 0 || (this.tabSearch != "" && this.sorted[this.tabIndex].IndexOf(this.tabSearch) != 0))
								{
									this.FindLastTab();
								}
							}
						}
						else if (this.tabIndex == -1)
						{
							this.tabSearch = this.currentText;
							this.FindFirstTab();
						}
						else
						{
							this.tabIndex++;
							if (this.tabIndex >= this.sorted.Count || (this.tabSearch != "" && this.sorted[this.tabIndex].IndexOf(this.tabSearch) != 0))
							{
								this.FindFirstTab();
							}
						}
						if (this.tabIndex != -1)
						{
							this.currentText = this.sorted[this.tabIndex];
							return;
						}
						return;
					}
				}
				else
				{
					if (this.currentText.Length > 0)
					{
						this.currentText = this.currentText.Substring(0, this.currentText.Length - 1);
						return;
					}
					return;
				}
			}
			else
			{
				if (key > Keys.F12)
				{
					switch (key)
					{
					case Keys.OemSemicolon:
						if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
						{
							this.currentText += ":";
							return;
						}
						this.currentText += ";";
						return;
					case Keys.OemPlus:
						if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
						{
							this.currentText += "+";
							return;
						}
						this.currentText += "=";
						return;
					case Keys.OemComma:
						if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
						{
							this.currentText += "<";
							return;
						}
						this.currentText += ",";
						return;
					case Keys.OemMinus:
						if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
						{
							this.currentText += "_";
							return;
						}
						this.currentText += "-";
						return;
					case Keys.OemPeriod:
						if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
						{
							this.currentText += ">";
							return;
						}
						this.currentText += ".";
						return;
					case Keys.OemQuestion:
						if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
						{
							this.currentText += "?";
							return;
						}
						this.currentText += "/";
						return;
					case Keys.OemTilde:
						break;
					default:
						switch (key)
						{
						case Keys.OemOpenBrackets:
							if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
							{
								this.currentText += "{";
								return;
							}
							this.currentText += "[";
							return;
						default:
							goto IL_17D;
						case Keys.OemCloseBrackets:
							if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
							{
								this.currentText += "}";
								return;
							}
							this.currentText += "]";
							return;
						case Keys.OemQuotes:
							if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
							{
								this.currentText += "\"";
								return;
							}
							this.currentText += "'";
							return;
						case Keys.Oem8:
							break;
						case Keys.OemBackslash:
							if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
							{
								this.currentText += "|";
								return;
							}
							this.currentText += "\\";
							return;
						}
						break;
					}
					this.Open = (this.canOpen = false);
					return;
				}
				switch (key)
				{
				case Keys.Space:
					this.currentText += " ";
					return;
				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.End:
				case Keys.Home:
				case Keys.Left:
				case Keys.Right:
				case Keys.Select:
				case Keys.Print:
				case Keys.Execute:
				case Keys.PrintScreen:
				case Keys.Insert:
				case Keys.Help:
					break;
				case Keys.Up:
					if (this.seekIndex < this.commandHistory.Count - 1)
					{
						this.seekIndex++;
						this.currentText = string.Join(" ", new string[]
						{
							this.commandHistory[this.seekIndex]
						});
						return;
					}
					return;
				case Keys.Down:
					if (this.seekIndex <= -1)
					{
						return;
					}
					this.seekIndex--;
					if (this.seekIndex == -1)
					{
						this.currentText = "";
						return;
					}
					this.currentText = string.Join(" ", new string[]
					{
						this.commandHistory[this.seekIndex]
					});
					return;
				case Keys.Delete:
					this.currentText = "";
					return;
				case Keys.D0:
					if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
					{
						this.currentText += ")";
						return;
					}
					this.currentText += "0";
					return;
				case Keys.D1:
					if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
					{
						this.currentText += "!";
						return;
					}
					this.currentText += "1";
					return;
				case Keys.D2:
					if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
					{
						this.currentText += "@";
						return;
					}
					this.currentText += "2";
					return;
				case Keys.D3:
					if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
					{
						this.currentText += "#";
						return;
					}
					this.currentText += "3";
					return;
				case Keys.D4:
					if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
					{
						this.currentText += "$";
						return;
					}
					this.currentText += "4";
					return;
				case Keys.D5:
					if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
					{
						this.currentText += "%";
						return;
					}
					this.currentText += "5";
					return;
				case Keys.D6:
					if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
					{
						this.currentText += "^";
						return;
					}
					this.currentText += "6";
					return;
				case Keys.D7:
					if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
					{
						this.currentText += "&";
						return;
					}
					this.currentText += "7";
					return;
				case Keys.D8:
					if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
					{
						this.currentText += "*";
						return;
					}
					this.currentText += "8";
					return;
				case Keys.D9:
					if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
					{
						this.currentText += "(";
						return;
					}
					this.currentText += "9";
					return;
				default:
					if (key - Keys.F1 <= 11)
					{
						this.ExecuteFunctionKeyAction(key - Keys.F1);
						return;
					}
					break;
				}
			}
			IL_17D:
			if (key.ToString().Length == 1)
			{
				if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
				{
					this.currentText += key.ToString();
					return;
				}
				this.currentText += key.ToString().ToLower();
				return;
			}
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x0001B51C File Offset: 0x0001971C
		private void EnterCommand()
		{
			string[] array = this.currentText.Split(new char[]
			{
				' ',
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (this.commandHistory.Count == 0 || this.commandHistory[0] != this.currentText)
			{
				this.commandHistory.Insert(0, this.currentText);
			}
			this.drawCommands.Insert(0, new Commands.Line(this.currentText, Color.Aqua));
			this.currentText = "";
			this.seekIndex = -1;
			string[] array2 = new string[array.Length - 1];
			for (int i = 1; i < array.Length; i++)
			{
				array2[i - 1] = array[i];
			}
			this.ExecuteCommand(array[0].ToLower(), array2);
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x0001B5DC File Offset: 0x000197DC
		private void FindFirstTab()
		{
			for (int i = 0; i < this.sorted.Count; i++)
			{
				if (this.tabSearch == "" || this.sorted[i].IndexOf(this.tabSearch) == 0)
				{
					this.tabIndex = i;
					return;
				}
			}
		}

		// Token: 0x06000A92 RID: 2706 RVA: 0x0001B634 File Offset: 0x00019834
		private void FindLastTab()
		{
			for (int i = 0; i < this.sorted.Count; i++)
			{
				if (this.tabSearch == "" || this.sorted[i].IndexOf(this.tabSearch) == 0)
				{
					this.tabIndex = i;
				}
			}
		}

		// Token: 0x06000A93 RID: 2707 RVA: 0x0001B68C File Offset: 0x0001988C
		internal void Render()
		{
			int viewWidth = Engine.ViewWidth;
			int viewHeight = Engine.ViewHeight;
			Draw.SpriteBatch.Begin();
			Draw.Rect(10f, (float)(viewHeight - 50), (float)(viewWidth - 20), 40f, Color.Black * 0.8f);
			if (this.underscore)
			{
				Draw.SpriteBatch.DrawString(Draw.DefaultFont, ">" + this.currentText + "_", new Vector2(20f, (float)(viewHeight - 42)), Color.White);
			}
			else
			{
				Draw.SpriteBatch.DrawString(Draw.DefaultFont, ">" + this.currentText, new Vector2(20f, (float)(viewHeight - 42)), Color.White);
			}
			if (this.drawCommands.Count > 0)
			{
				int num = 10 + 30 * this.drawCommands.Count;
				Draw.Rect(10f, (float)(viewHeight - num - 60), (float)(viewWidth - 20), (float)num, Color.Black * 0.8f);
				for (int i = 0; i < this.drawCommands.Count; i++)
				{
					Draw.SpriteBatch.DrawString(Draw.DefaultFont, this.drawCommands[i].Text, new Vector2(20f, (float)(viewHeight - 92 - 30 * i)), this.drawCommands[i].Color);
				}
			}
			Draw.SpriteBatch.End();
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x0001B7FC File Offset: 0x000199FC
		public void ExecuteCommand(string command, string[] args)
		{
			if (this.commands.ContainsKey(command))
			{
				this.commands[command].Action(args);
				return;
			}
			this.Log("Command '" + command + "' not found! Type 'help' for list of commands", Color.Yellow);
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x0001B84A File Offset: 0x00019A4A
		public void ExecuteFunctionKeyAction(int num)
		{
			if (this.FunctionKeyActions[num] != null)
			{
				this.FunctionKeyActions[num]();
			}
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x0001B864 File Offset: 0x00019A64
		private void BuildCommandsList()
		{
			Type[] types = Assembly.GetCallingAssembly().GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				foreach (MethodInfo method in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					this.ProcessMethod(method);
				}
			}
			types = Assembly.GetEntryAssembly().GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				foreach (MethodInfo method2 in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					this.ProcessMethod(method2);
				}
			}
			foreach (KeyValuePair<string, Commands.CommandInfo> keyValuePair in this.commands)
			{
				this.sorted.Add(keyValuePair.Key);
			}
			this.sorted.Sort();
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x0001B950 File Offset: 0x00019B50
		private void ProcessMethod(MethodInfo method)
		{
			Command command = null;
			object[] customAttributes = method.GetCustomAttributes(typeof(Command), false);
			if (customAttributes.Length != 0)
			{
				command = (customAttributes[0] as Command);
			}
			if (command != null)
			{
				if (!method.IsStatic)
				{
					throw new Exception(method.DeclaringType.Name + "." + method.Name + " is marked as a command, but is not static");
				}
				Commands.CommandInfo value = default(Commands.CommandInfo);
				value.Help = command.Help;
				ParameterInfo[] parameters = method.GetParameters();
				object[] defaults = new object[parameters.Length];
				string[] array = new string[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					ParameterInfo parameterInfo = parameters[i];
					array[i] = parameterInfo.Name + ":";
					if (parameterInfo.ParameterType == typeof(string))
					{
						string[] array2 = array;
						int num = i;
						array2[num] += "string";
					}
					else if (parameterInfo.ParameterType == typeof(int))
					{
						string[] array3 = array;
						int num2 = i;
						array3[num2] += "int";
					}
					else if (parameterInfo.ParameterType == typeof(float))
					{
						string[] array4 = array;
						int num3 = i;
						array4[num3] += "float";
					}
					else
					{
						if (!(parameterInfo.ParameterType == typeof(bool)))
						{
							throw new Exception(method.DeclaringType.Name + "." + method.Name + " is marked as a command, but has an invalid parameter type. Allowed types are: string, int, float, and bool");
						}
						string[] array5 = array;
						int num4 = i;
						array5[num4] += "bool";
					}
					if (parameterInfo.DefaultValue == DBNull.Value)
					{
						defaults[i] = null;
					}
					else if (parameterInfo.DefaultValue != null)
					{
						defaults[i] = parameterInfo.DefaultValue;
						if (parameterInfo.ParameterType == typeof(string))
						{
							ref string ptr = ref array[i];
							ptr = string.Concat(new object[]
							{
								ptr,
								"=\"",
								parameterInfo.DefaultValue,
								"\""
							});
						}
						else
						{
							string[] array6 = array;
							int num5 = i;
							array6[num5] = array6[num5] + "=" + parameterInfo.DefaultValue;
						}
					}
					else
					{
						defaults[i] = null;
					}
				}
				if (array.Length == 0)
				{
					value.Usage = "";
				}
				else
				{
					value.Usage = "[" + string.Join(" ", array) + "]";
				}
				value.Action = delegate(string[] args)
				{
					if (parameters.Length == 0)
					{
						this.InvokeMethod(method, null);
						return;
					}
					object[] array7 = (object[])defaults.Clone();
					int num6 = 0;
					while (num6 < array7.Length && num6 < args.Length)
					{
						if (parameters[num6].ParameterType == typeof(string))
						{
							array7[num6] = Commands.ArgString(args[num6]);
						}
						else if (parameters[num6].ParameterType == typeof(int))
						{
							array7[num6] = Commands.ArgInt(args[num6]);
						}
						else if (parameters[num6].ParameterType == typeof(float))
						{
							array7[num6] = Commands.ArgFloat(args[num6]);
						}
						else if (parameters[num6].ParameterType == typeof(bool))
						{
							array7[num6] = Commands.ArgBool(args[num6]);
						}
						num6++;
					}
					this.InvokeMethod(method, array7);
				};
				this.commands[command.Name] = value;
			}
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x0001BC84 File Offset: 0x00019E84
		private void InvokeMethod(MethodInfo method, object[] param = null)
		{
			try
			{
				method.Invoke(null, param);
			}
			catch (Exception ex)
			{
				Engine.Commands.Log(ex.InnerException.Message, Color.Yellow);
				this.LogStackTrace(ex.InnerException.StackTrace);
			}
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x0001BCDC File Offset: 0x00019EDC
		private void LogStackTrace(string stackTrace)
		{
			foreach (string text in stackTrace.Split(new char[]
			{
				'\n'
			}))
			{
				int num = text.LastIndexOf(" in ") + 4;
				int num2 = text.LastIndexOf('\\') + 1;
				if (num != -1 && num2 != -1)
				{
					text = text.Substring(0, num) + text.Substring(num2);
				}
				int num3 = text.IndexOf('(') + 1;
				int num4 = text.IndexOf(')');
				if (num3 != -1 && num4 != -1)
				{
					text = text.Substring(0, num3) + text.Substring(num4);
				}
				int num5 = text.LastIndexOf(':');
				if (num5 != -1)
				{
					text = text.Insert(num5 + 1, " ").Insert(num5, " ");
				}
				text = text.TrimStart(new char[0]);
				text = "-> " + text;
				Engine.Commands.Log(text, Color.White);
			}
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x0001BDD6 File Offset: 0x00019FD6
		private static string ArgString(string arg)
		{
			if (arg == null)
			{
				return "";
			}
			return arg;
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x0001BDE2 File Offset: 0x00019FE2
		private static bool ArgBool(string arg)
		{
			return arg != null && (!(arg == "0") && !(arg.ToLower() == "false")) && !(arg.ToLower() == "f");
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x0001BE20 File Offset: 0x0001A020
		private static int ArgInt(string arg)
		{
			int result;
			try
			{
				result = Convert.ToInt32(arg);
			}
			catch
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x0001BE4C File Offset: 0x0001A04C
		private static float ArgFloat(string arg)
		{
			float result;
			try
			{
				result = Convert.ToSingle(arg, CultureInfo.InvariantCulture);
			}
			catch
			{
				result = 0f;
			}
			return result;
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x0001BE84 File Offset: 0x0001A084
		[Command("clear", "Clears the terminal")]
		public static void Clear()
		{
			Engine.Commands.drawCommands.Clear();
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x0001BE95 File Offset: 0x0001A095
		[Command("exit", "Exits the game")]
		private static void Exit()
		{
			Engine.Instance.Exit();
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x0001BEA1 File Offset: 0x0001A0A1
		[Command("vsync", "Enables or disables vertical sync")]
		private static void Vsync(bool enabled = true)
		{
			Engine.Graphics.SynchronizeWithVerticalRetrace = enabled;
			Engine.Graphics.ApplyChanges();
			Engine.Commands.Log("Vertical Sync " + (enabled ? "Enabled" : "Disabled"));
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x0001BEDC File Offset: 0x0001A0DC
		[Command("count", "Logs amount of Entities in the Scene. Pass a tagIndex to count only Entities with that tag")]
		private static void Count(int tagIndex = -1)
		{
			if (Engine.Scene == null)
			{
				Engine.Commands.Log("Current Scene is null!");
				return;
			}
			if (tagIndex < 0)
			{
				Engine.Commands.Log(Engine.Scene.Entities.Count.ToString());
				return;
			}
			Engine.Commands.Log(Engine.Scene.TagLists[tagIndex].Count.ToString());
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x0001BF50 File Offset: 0x0001A150
		[Command("tracker", "Logs all tracked objects in the scene. Set mode to 'e' for just entities, or 'c' for just components")]
		private static void Tracker(string mode)
		{
			if (Engine.Scene == null)
			{
				Engine.Commands.Log("Current Scene is null!");
				return;
			}
			if (mode == "e")
			{
				Engine.Scene.Tracker.LogEntities();
				return;
			}
			if (!(mode == "c"))
			{
				Engine.Commands.Log("-- Entities --");
				Engine.Scene.Tracker.LogEntities();
				Engine.Commands.Log("-- Components --");
				Engine.Scene.Tracker.LogComponents();
				return;
			}
			Engine.Scene.Tracker.LogComponents();
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x0001BFEA File Offset: 0x0001A1EA
		[Command("pooler", "Logs the pooled Entity counts")]
		private static void Pooler()
		{
			Engine.Pooler.Log();
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x0001BFF6 File Offset: 0x0001A1F6
		[Command("fullscreen", "Switches to fullscreen mode")]
		private static void Fullscreen()
		{
			Engine.SetFullscreen();
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x0001BFFD File Offset: 0x0001A1FD
		[Command("window", "Switches to window mode")]
		private static void Window(int scale = 1)
		{
			Engine.SetWindowed(320 * scale, 180 * scale);
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x0001C014 File Offset: 0x0001A214
		[Command("help", "Shows usage help for a given command")]
		private static void Help(string command)
		{
			if (!Engine.Commands.sorted.Contains(command))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Commands list: ");
				stringBuilder.Append(string.Join(", ", Engine.Commands.sorted));
				Engine.Commands.Log(stringBuilder.ToString());
				Engine.Commands.Log("Type 'help command' for more info on that command!");
				return;
			}
			Commands.CommandInfo commandInfo = Engine.Commands.commands[command];
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append(":: ");
			stringBuilder2.Append(command);
			if (!string.IsNullOrEmpty(commandInfo.Usage))
			{
				stringBuilder2.Append(" ");
				stringBuilder2.Append(commandInfo.Usage);
			}
			Engine.Commands.Log(stringBuilder2.ToString());
			if (string.IsNullOrEmpty(commandInfo.Help))
			{
				Engine.Commands.Log("No help info set");
				return;
			}
			Engine.Commands.Log(commandInfo.Help);
		}

		// Token: 0x04000644 RID: 1604
		private const float UNDERSCORE_TIME = 0.5f;

		// Token: 0x04000645 RID: 1605
		private const float REPEAT_DELAY = 0.5f;

		// Token: 0x04000646 RID: 1606
		private const float REPEAT_EVERY = 0.033333335f;

		// Token: 0x04000647 RID: 1607
		private const float OPACITY = 0.8f;

		// Token: 0x04000648 RID: 1608
		public bool Enabled = true;

		// Token: 0x04000649 RID: 1609
		public bool Open;

		// Token: 0x0400064B RID: 1611
		private Dictionary<string, Commands.CommandInfo> commands;

		// Token: 0x0400064C RID: 1612
		private List<string> sorted;

		// Token: 0x0400064D RID: 1613
		private KeyboardState oldState;

		// Token: 0x0400064E RID: 1614
		private KeyboardState currentState;

		// Token: 0x0400064F RID: 1615
		private string currentText = "";

		// Token: 0x04000650 RID: 1616
		private List<Commands.Line> drawCommands;

		// Token: 0x04000651 RID: 1617
		private bool underscore;

		// Token: 0x04000652 RID: 1618
		private float underscoreCounter;

		// Token: 0x04000653 RID: 1619
		private List<string> commandHistory;

		// Token: 0x04000654 RID: 1620
		private int seekIndex = -1;

		// Token: 0x04000655 RID: 1621
		private int tabIndex = -1;

		// Token: 0x04000656 RID: 1622
		private string tabSearch;

		// Token: 0x04000657 RID: 1623
		private float repeatCounter;

		// Token: 0x04000658 RID: 1624
		private Keys? repeatKey;

		// Token: 0x04000659 RID: 1625
		private bool canOpen;

		// Token: 0x020003B6 RID: 950
		private struct CommandInfo
		{
			// Token: 0x04001F51 RID: 8017
			public Action<string[]> Action;

			// Token: 0x04001F52 RID: 8018
			public string Help;

			// Token: 0x04001F53 RID: 8019
			public string Usage;
		}

		// Token: 0x020003B7 RID: 951
		private struct Line
		{
			// Token: 0x06001EA8 RID: 7848 RVA: 0x000D58CA File Offset: 0x000D3ACA
			public Line(string text)
			{
				this.Text = text;
				this.Color = Color.White;
			}

			// Token: 0x06001EA9 RID: 7849 RVA: 0x000D58DE File Offset: 0x000D3ADE
			public Line(string text, Color color)
			{
				this.Text = text;
				this.Color = color;
			}

			// Token: 0x04001F54 RID: 8020
			public string Text;

			// Token: 0x04001F55 RID: 8021
			public Color Color;
		}
	}
}
