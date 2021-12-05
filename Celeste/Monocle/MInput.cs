using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Monocle
{
	// Token: 0x0200010D RID: 269
	public static class MInput
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000851 RID: 2129 RVA: 0x00012507 File Offset: 0x00010707
		// (set) Token: 0x06000852 RID: 2130 RVA: 0x0001250E File Offset: 0x0001070E
		public static MInput.KeyboardData Keyboard { get; private set; }

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000853 RID: 2131 RVA: 0x00012516 File Offset: 0x00010716
		// (set) Token: 0x06000854 RID: 2132 RVA: 0x0001251D File Offset: 0x0001071D
		public static MInput.MouseData Mouse { get; private set; }

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000855 RID: 2133 RVA: 0x00012525 File Offset: 0x00010725
		// (set) Token: 0x06000856 RID: 2134 RVA: 0x0001252C File Offset: 0x0001072C
		public static MInput.GamePadData[] GamePads { get; private set; }

		// Token: 0x06000857 RID: 2135 RVA: 0x00012534 File Offset: 0x00010734
		internal static void Initialize()
		{
			MInput.Keyboard = new MInput.KeyboardData();
			MInput.Mouse = new MInput.MouseData();
			MInput.GamePads = new MInput.GamePadData[4];
			for (int i = 0; i < 4; i++)
			{
				MInput.GamePads[i] = new MInput.GamePadData(i);
			}
			MInput.VirtualInputs = new List<VirtualInput>();
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x00012584 File Offset: 0x00010784
		internal static void Shutdown()
		{
			MInput.GamePadData[] gamePads = MInput.GamePads;
			for (int i = 0; i < gamePads.Length; i++)
			{
				gamePads[i].StopRumble();
			}
		}

		// Token: 0x06000859 RID: 2137 RVA: 0x000125B0 File Offset: 0x000107B0
		internal static void Update()
		{
			if (Engine.Instance.IsActive && MInput.Active)
			{
				if (Engine.Commands.Open)
				{
					MInput.Keyboard.UpdateNull();
					MInput.Mouse.UpdateNull();
				}
				else
				{
					MInput.Keyboard.Update();
					MInput.Mouse.Update();
				}
				bool flag = false;
				bool flag2 = false;
				for (int i = 0; i < 4; i++)
				{
					MInput.GamePads[i].Update();
					if (MInput.GamePads[i].HasAnyInput())
					{
						MInput.ControllerHasFocus = true;
						flag = true;
					}
					if (MInput.GamePads[i].Attached)
					{
						flag2 = true;
					}
				}
				if (!flag2 || (!flag && MInput.Keyboard.HasAnyInput()))
				{
					MInput.ControllerHasFocus = false;
				}
			}
			else
			{
				MInput.Keyboard.UpdateNull();
				MInput.Mouse.UpdateNull();
				for (int j = 0; j < 4; j++)
				{
					MInput.GamePads[j].UpdateNull();
				}
			}
			MInput.UpdateVirtualInputs();
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x0001269C File Offset: 0x0001089C
		public static void UpdateNull()
		{
			MInput.Keyboard.UpdateNull();
			MInput.Mouse.UpdateNull();
			for (int i = 0; i < 4; i++)
			{
				MInput.GamePads[i].UpdateNull();
			}
			MInput.UpdateVirtualInputs();
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x000126DC File Offset: 0x000108DC
		private static void UpdateVirtualInputs()
		{
			foreach (VirtualInput virtualInput in MInput.VirtualInputs)
			{
				virtualInput.Update();
			}
		}

		// Token: 0x0600085C RID: 2140 RVA: 0x0001272C File Offset: 0x0001092C
		public static void RumbleFirst(float strength, float time)
		{
			MInput.GamePads[0].Rumble(strength, time);
		}

		// Token: 0x0600085D RID: 2141 RVA: 0x0001273C File Offset: 0x0001093C
		public static int Axis(bool negative, bool positive, int bothValue)
		{
			if (negative)
			{
				if (positive)
				{
					return bothValue;
				}
				return -1;
			}
			else
			{
				if (positive)
				{
					return 1;
				}
				return 0;
			}
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x0001274E File Offset: 0x0001094E
		public static int Axis(float axisValue, float deadzone)
		{
			if (Math.Abs(axisValue) >= deadzone)
			{
				return Math.Sign(axisValue);
			}
			return 0;
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x00012764 File Offset: 0x00010964
		public static int Axis(bool negative, bool positive, int bothValue, float axisValue, float deadzone)
		{
			int num = MInput.Axis(axisValue, deadzone);
			if (num == 0)
			{
				num = MInput.Axis(negative, positive, bothValue);
			}
			return num;
		}

		// Token: 0x0400058C RID: 1420
		internal static List<VirtualInput> VirtualInputs;

		// Token: 0x0400058D RID: 1421
		public static bool Active = true;

		// Token: 0x0400058E RID: 1422
		public static bool Disabled = false;

		// Token: 0x0400058F RID: 1423
		public static bool ControllerHasFocus = false;

		// Token: 0x04000590 RID: 1424
		public static bool IsControllerFocused = false;

		// Token: 0x020003A7 RID: 935
		public class KeyboardData
		{
			// Token: 0x06001E25 RID: 7717 RVA: 0x000026FC File Offset: 0x000008FC
			internal KeyboardData()
			{
			}

			// Token: 0x06001E26 RID: 7718 RVA: 0x000D3945 File Offset: 0x000D1B45
			internal void Update()
			{
				this.PreviousState = this.CurrentState;
				this.CurrentState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
			}

			// Token: 0x06001E27 RID: 7719 RVA: 0x000D395E File Offset: 0x000D1B5E
			internal void UpdateNull()
			{
				this.PreviousState = this.CurrentState;
				this.CurrentState = default(KeyboardState);
				this.CurrentState.GetPressedKeys();
			}

			// Token: 0x06001E28 RID: 7720 RVA: 0x000D3984 File Offset: 0x000D1B84
			public bool HasAnyInput()
			{
				return this.CurrentState.GetPressedKeys().Length != 0;
			}

			// Token: 0x06001E29 RID: 7721 RVA: 0x000D3995 File Offset: 0x000D1B95
			public bool Check(Keys key)
			{
				return !MInput.Disabled && key != Keys.None && this.CurrentState.IsKeyDown(key);
			}

			// Token: 0x06001E2A RID: 7722 RVA: 0x000D39B1 File Offset: 0x000D1BB1
			public bool Pressed(Keys key)
			{
				return !MInput.Disabled && (key != Keys.None && this.CurrentState.IsKeyDown(key)) && !this.PreviousState.IsKeyDown(key);
			}

			// Token: 0x06001E2B RID: 7723 RVA: 0x000D39DE File Offset: 0x000D1BDE
			public bool Released(Keys key)
			{
				return !MInput.Disabled && (key != Keys.None && !this.CurrentState.IsKeyDown(key)) && this.PreviousState.IsKeyDown(key);
			}

			// Token: 0x06001E2C RID: 7724 RVA: 0x000D3A08 File Offset: 0x000D1C08
			public bool Check(Keys keyA, Keys keyB)
			{
				return this.Check(keyA) || this.Check(keyB);
			}

			// Token: 0x06001E2D RID: 7725 RVA: 0x000D3A1C File Offset: 0x000D1C1C
			public bool Pressed(Keys keyA, Keys keyB)
			{
				return this.Pressed(keyA) || this.Pressed(keyB);
			}

			// Token: 0x06001E2E RID: 7726 RVA: 0x000D3A30 File Offset: 0x000D1C30
			public bool Released(Keys keyA, Keys keyB)
			{
				return this.Released(keyA) || this.Released(keyB);
			}

			// Token: 0x06001E2F RID: 7727 RVA: 0x000D3A44 File Offset: 0x000D1C44
			public bool Check(Keys keyA, Keys keyB, Keys keyC)
			{
				return this.Check(keyA) || this.Check(keyB) || this.Check(keyC);
			}

			// Token: 0x06001E30 RID: 7728 RVA: 0x000D3A61 File Offset: 0x000D1C61
			public bool Pressed(Keys keyA, Keys keyB, Keys keyC)
			{
				return this.Pressed(keyA) || this.Pressed(keyB) || this.Pressed(keyC);
			}

			// Token: 0x06001E31 RID: 7729 RVA: 0x000D3A7E File Offset: 0x000D1C7E
			public bool Released(Keys keyA, Keys keyB, Keys keyC)
			{
				return this.Released(keyA) || this.Released(keyB) || this.Released(keyC);
			}

			// Token: 0x06001E32 RID: 7730 RVA: 0x000D3A9B File Offset: 0x000D1C9B
			public int AxisCheck(Keys negative, Keys positive)
			{
				if (this.Check(negative))
				{
					if (this.Check(positive))
					{
						return 0;
					}
					return -1;
				}
				else
				{
					if (this.Check(positive))
					{
						return 1;
					}
					return 0;
				}
			}

			// Token: 0x06001E33 RID: 7731 RVA: 0x000D3ABF File Offset: 0x000D1CBF
			public int AxisCheck(Keys negative, Keys positive, int both)
			{
				if (this.Check(negative))
				{
					if (this.Check(positive))
					{
						return both;
					}
					return -1;
				}
				else
				{
					if (this.Check(positive))
					{
						return 1;
					}
					return 0;
				}
			}

			// Token: 0x04001F1C RID: 7964
			public KeyboardState PreviousState;

			// Token: 0x04001F1D RID: 7965
			public KeyboardState CurrentState;
		}

		// Token: 0x020003A8 RID: 936
		public class MouseData
		{
			// Token: 0x06001E34 RID: 7732 RVA: 0x000D3AE3 File Offset: 0x000D1CE3
			internal MouseData()
			{
				this.PreviousState = default(MouseState);
				this.CurrentState = default(MouseState);
			}

			// Token: 0x06001E35 RID: 7733 RVA: 0x000D3B03 File Offset: 0x000D1D03
			internal void Update()
			{
				this.PreviousState = this.CurrentState;
				this.CurrentState = Microsoft.Xna.Framework.Input.Mouse.GetState();
			}

			// Token: 0x06001E36 RID: 7734 RVA: 0x000D3B1C File Offset: 0x000D1D1C
			internal void UpdateNull()
			{
				this.PreviousState = this.CurrentState;
				this.CurrentState = default(MouseState);
			}

			// Token: 0x17000252 RID: 594
			// (get) Token: 0x06001E37 RID: 7735 RVA: 0x000D3B36 File Offset: 0x000D1D36
			public bool CheckLeftButton
			{
				get
				{
					return this.CurrentState.LeftButton == ButtonState.Pressed;
				}
			}

			// Token: 0x17000253 RID: 595
			// (get) Token: 0x06001E38 RID: 7736 RVA: 0x000D3B46 File Offset: 0x000D1D46
			public bool CheckRightButton
			{
				get
				{
					return this.CurrentState.RightButton == ButtonState.Pressed;
				}
			}

			// Token: 0x17000254 RID: 596
			// (get) Token: 0x06001E39 RID: 7737 RVA: 0x000D3B56 File Offset: 0x000D1D56
			public bool CheckMiddleButton
			{
				get
				{
					return this.CurrentState.MiddleButton == ButtonState.Pressed;
				}
			}

			// Token: 0x17000255 RID: 597
			// (get) Token: 0x06001E3A RID: 7738 RVA: 0x000D3B66 File Offset: 0x000D1D66
			public bool PressedLeftButton
			{
				get
				{
					return this.CurrentState.LeftButton == ButtonState.Pressed && this.PreviousState.LeftButton == ButtonState.Released;
				}
			}

			// Token: 0x17000256 RID: 598
			// (get) Token: 0x06001E3B RID: 7739 RVA: 0x000D3B86 File Offset: 0x000D1D86
			public bool PressedRightButton
			{
				get
				{
					return this.CurrentState.RightButton == ButtonState.Pressed && this.PreviousState.RightButton == ButtonState.Released;
				}
			}

			// Token: 0x17000257 RID: 599
			// (get) Token: 0x06001E3C RID: 7740 RVA: 0x000D3BA6 File Offset: 0x000D1DA6
			public bool PressedMiddleButton
			{
				get
				{
					return this.CurrentState.MiddleButton == ButtonState.Pressed && this.PreviousState.MiddleButton == ButtonState.Released;
				}
			}

			// Token: 0x17000258 RID: 600
			// (get) Token: 0x06001E3D RID: 7741 RVA: 0x000D3BC6 File Offset: 0x000D1DC6
			public bool ReleasedLeftButton
			{
				get
				{
					return this.CurrentState.LeftButton == ButtonState.Released && this.PreviousState.LeftButton == ButtonState.Pressed;
				}
			}

			// Token: 0x17000259 RID: 601
			// (get) Token: 0x06001E3E RID: 7742 RVA: 0x000D3BE5 File Offset: 0x000D1DE5
			public bool ReleasedRightButton
			{
				get
				{
					return this.CurrentState.RightButton == ButtonState.Released && this.PreviousState.RightButton == ButtonState.Pressed;
				}
			}

			// Token: 0x1700025A RID: 602
			// (get) Token: 0x06001E3F RID: 7743 RVA: 0x000D3C04 File Offset: 0x000D1E04
			public bool ReleasedMiddleButton
			{
				get
				{
					return this.CurrentState.MiddleButton == ButtonState.Released && this.PreviousState.MiddleButton == ButtonState.Pressed;
				}
			}

			// Token: 0x1700025B RID: 603
			// (get) Token: 0x06001E40 RID: 7744 RVA: 0x000D3C23 File Offset: 0x000D1E23
			public int Wheel
			{
				get
				{
					return this.CurrentState.ScrollWheelValue;
				}
			}

			// Token: 0x1700025C RID: 604
			// (get) Token: 0x06001E41 RID: 7745 RVA: 0x000D3C30 File Offset: 0x000D1E30
			public int WheelDelta
			{
				get
				{
					return this.CurrentState.ScrollWheelValue - this.PreviousState.ScrollWheelValue;
				}
			}

			// Token: 0x1700025D RID: 605
			// (get) Token: 0x06001E42 RID: 7746 RVA: 0x000D3C49 File Offset: 0x000D1E49
			public bool WasMoved
			{
				get
				{
					return this.CurrentState.X != this.PreviousState.X || this.CurrentState.Y != this.PreviousState.Y;
				}
			}

			// Token: 0x1700025E RID: 606
			// (get) Token: 0x06001E43 RID: 7747 RVA: 0x000D3C80 File Offset: 0x000D1E80
			// (set) Token: 0x06001E44 RID: 7748 RVA: 0x000D3C8D File Offset: 0x000D1E8D
			public float X
			{
				get
				{
					return this.Position.X;
				}
				set
				{
					this.Position = new Vector2(value, this.Position.Y);
				}
			}

			// Token: 0x1700025F RID: 607
			// (get) Token: 0x06001E45 RID: 7749 RVA: 0x000D3CA6 File Offset: 0x000D1EA6
			// (set) Token: 0x06001E46 RID: 7750 RVA: 0x000D3CB3 File Offset: 0x000D1EB3
			public float Y
			{
				get
				{
					return this.Position.Y;
				}
				set
				{
					this.Position = new Vector2(this.Position.X, value);
				}
			}

			// Token: 0x17000260 RID: 608
			// (get) Token: 0x06001E47 RID: 7751 RVA: 0x000D3CCC File Offset: 0x000D1ECC
			// (set) Token: 0x06001E48 RID: 7752 RVA: 0x000D3CFC File Offset: 0x000D1EFC
			public Vector2 Position
			{
				get
				{
					return Vector2.Transform(new Vector2((float)this.CurrentState.X, (float)this.CurrentState.Y), Matrix.Invert(Engine.ScreenMatrix));
				}
				set
				{
					Vector2 vector = Vector2.Transform(value, Engine.ScreenMatrix);
					Microsoft.Xna.Framework.Input.Mouse.SetPosition((int)Math.Round((double)vector.X), (int)Math.Round((double)vector.Y));
				}
			}

			// Token: 0x04001F1E RID: 7966
			public MouseState PreviousState;

			// Token: 0x04001F1F RID: 7967
			public MouseState CurrentState;
		}

		// Token: 0x020003A9 RID: 937
		public class GamePadData
		{
			// Token: 0x06001E49 RID: 7753 RVA: 0x000D3D34 File Offset: 0x000D1F34
			internal GamePadData(int playerIndex)
			{
				this.PlayerIndex = (PlayerIndex)Calc.Clamp(playerIndex, 0, 3);
			}

			// Token: 0x06001E4A RID: 7754 RVA: 0x000D3D4C File Offset: 0x000D1F4C
			public bool HasAnyInput()
			{
				return (!this.PreviousState.IsConnected && this.CurrentState.IsConnected) || this.PreviousState.Buttons != this.CurrentState.Buttons || this.PreviousState.DPad != this.CurrentState.DPad || (this.CurrentState.Triggers.Left > 0.01f || this.CurrentState.Triggers.Right > 0.01f) || (this.CurrentState.ThumbSticks.Left.Length() > 0.01f || this.CurrentState.ThumbSticks.Right.Length() > 0.01f);
			}

			// Token: 0x06001E4B RID: 7755 RVA: 0x000D3E30 File Offset: 0x000D2030
			public void Update()
			{
				this.PreviousState = this.CurrentState;
				this.CurrentState = GamePad.GetState(this.PlayerIndex);
				if (!this.Attached && this.CurrentState.IsConnected)
				{
					MInput.IsControllerFocused = true;
				}
				this.Attached = this.CurrentState.IsConnected;
				if (this.rumbleTime > 0f)
				{
					this.rumbleTime -= Engine.DeltaTime;
					if (this.rumbleTime <= 0f)
					{
						GamePad.SetVibration(this.PlayerIndex, 0f, 0f);
					}
				}
			}

			// Token: 0x06001E4C RID: 7756 RVA: 0x000D3EC8 File Offset: 0x000D20C8
			public void UpdateNull()
			{
				this.PreviousState = this.CurrentState;
				this.CurrentState = default(GamePadState);
				this.Attached = GamePad.GetState(this.PlayerIndex).IsConnected;
				if (this.rumbleTime > 0f)
				{
					this.rumbleTime -= Engine.DeltaTime;
				}
				GamePad.SetVibration(this.PlayerIndex, 0f, 0f);
			}

			// Token: 0x06001E4D RID: 7757 RVA: 0x000D3F3C File Offset: 0x000D213C
			public void Rumble(float strength, float time)
			{
				if (this.rumbleTime <= 0f || strength > this.rumbleStrength || (strength == this.rumbleStrength && time > this.rumbleTime))
				{
					GamePad.SetVibration(this.PlayerIndex, strength, strength);
					this.rumbleStrength = strength;
					this.rumbleTime = time;
				}
			}

			// Token: 0x06001E4E RID: 7758 RVA: 0x000D3F8D File Offset: 0x000D218D
			public void StopRumble()
			{
				GamePad.SetVibration(this.PlayerIndex, 0f, 0f);
				this.rumbleTime = 0f;
			}

			// Token: 0x06001E4F RID: 7759 RVA: 0x000D3FB0 File Offset: 0x000D21B0
			public bool Check(Buttons button)
			{
				return !MInput.Disabled && this.CurrentState.IsButtonDown(button);
			}

			// Token: 0x06001E50 RID: 7760 RVA: 0x000D3FC7 File Offset: 0x000D21C7
			public bool Pressed(Buttons button)
			{
				return !MInput.Disabled && this.CurrentState.IsButtonDown(button) && this.PreviousState.IsButtonUp(button);
			}

			// Token: 0x06001E51 RID: 7761 RVA: 0x000D3FEE File Offset: 0x000D21EE
			public bool Released(Buttons button)
			{
				return !MInput.Disabled && this.CurrentState.IsButtonUp(button) && this.PreviousState.IsButtonDown(button);
			}

			// Token: 0x06001E52 RID: 7762 RVA: 0x000D4015 File Offset: 0x000D2215
			public bool Check(Buttons buttonA, Buttons buttonB)
			{
				return this.Check(buttonA) || this.Check(buttonB);
			}

			// Token: 0x06001E53 RID: 7763 RVA: 0x000D4029 File Offset: 0x000D2229
			public bool Pressed(Buttons buttonA, Buttons buttonB)
			{
				return this.Pressed(buttonA) || this.Pressed(buttonB);
			}

			// Token: 0x06001E54 RID: 7764 RVA: 0x000D403D File Offset: 0x000D223D
			public bool Released(Buttons buttonA, Buttons buttonB)
			{
				return this.Released(buttonA) || this.Released(buttonB);
			}

			// Token: 0x06001E55 RID: 7765 RVA: 0x000D4051 File Offset: 0x000D2251
			public bool Check(Buttons buttonA, Buttons buttonB, Buttons buttonC)
			{
				return this.Check(buttonA) || this.Check(buttonB) || this.Check(buttonC);
			}

			// Token: 0x06001E56 RID: 7766 RVA: 0x000D406E File Offset: 0x000D226E
			public bool Pressed(Buttons buttonA, Buttons buttonB, Buttons buttonC)
			{
				return this.Pressed(buttonA) || this.Pressed(buttonB) || this.Check(buttonC);
			}

			// Token: 0x06001E57 RID: 7767 RVA: 0x000D408B File Offset: 0x000D228B
			public bool Released(Buttons buttonA, Buttons buttonB, Buttons buttonC)
			{
				return this.Released(buttonA) || this.Released(buttonB) || this.Check(buttonC);
			}

			// Token: 0x06001E58 RID: 7768 RVA: 0x000D40A8 File Offset: 0x000D22A8
			public Vector2 GetLeftStick()
			{
				Vector2 left = this.CurrentState.ThumbSticks.Left;
				left.Y = -left.Y;
				return left;
			}

			// Token: 0x06001E59 RID: 7769 RVA: 0x000D40D8 File Offset: 0x000D22D8
			public Vector2 GetLeftStick(float deadzone)
			{
				Vector2 vector = this.CurrentState.ThumbSticks.Left;
				if (vector.LengthSquared() < deadzone * deadzone)
				{
					vector = Vector2.Zero;
				}
				else
				{
					vector.Y = -vector.Y;
				}
				return vector;
			}

			// Token: 0x06001E5A RID: 7770 RVA: 0x000D411C File Offset: 0x000D231C
			public Vector2 GetRightStick()
			{
				Vector2 right = this.CurrentState.ThumbSticks.Right;
				right.Y = -right.Y;
				return right;
			}

			// Token: 0x06001E5B RID: 7771 RVA: 0x000D414C File Offset: 0x000D234C
			public Vector2 GetRightStick(float deadzone)
			{
				Vector2 vector = this.CurrentState.ThumbSticks.Right;
				if (vector.LengthSquared() < deadzone * deadzone)
				{
					vector = Vector2.Zero;
				}
				else
				{
					vector.Y = -vector.Y;
				}
				return vector;
			}

			// Token: 0x06001E5C RID: 7772 RVA: 0x000D4190 File Offset: 0x000D2390
			public bool LeftStickLeftCheck(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Left.X <= -deadzone;
			}

			// Token: 0x06001E5D RID: 7773 RVA: 0x000D41BC File Offset: 0x000D23BC
			public bool LeftStickLeftPressed(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Left.X <= -deadzone && this.PreviousState.ThumbSticks.Left.X > -deadzone;
			}

			// Token: 0x06001E5E RID: 7774 RVA: 0x000D4204 File Offset: 0x000D2404
			public bool LeftStickLeftReleased(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Left.X > -deadzone && this.PreviousState.ThumbSticks.Left.X <= -deadzone;
			}

			// Token: 0x06001E5F RID: 7775 RVA: 0x000D4250 File Offset: 0x000D2450
			public bool LeftStickRightCheck(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Left.X >= deadzone;
			}

			// Token: 0x06001E60 RID: 7776 RVA: 0x000D427C File Offset: 0x000D247C
			public bool LeftStickRightPressed(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Left.X >= deadzone && this.PreviousState.ThumbSticks.Left.X < deadzone;
			}

			// Token: 0x06001E61 RID: 7777 RVA: 0x000D42C4 File Offset: 0x000D24C4
			public bool LeftStickRightReleased(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Left.X < deadzone && this.PreviousState.ThumbSticks.Left.X >= deadzone;
			}

			// Token: 0x06001E62 RID: 7778 RVA: 0x000D430C File Offset: 0x000D250C
			public bool LeftStickDownCheck(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Left.Y <= -deadzone;
			}

			// Token: 0x06001E63 RID: 7779 RVA: 0x000D4338 File Offset: 0x000D2538
			public bool LeftStickDownPressed(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Left.Y <= -deadzone && this.PreviousState.ThumbSticks.Left.Y > -deadzone;
			}

			// Token: 0x06001E64 RID: 7780 RVA: 0x000D4380 File Offset: 0x000D2580
			public bool LeftStickDownReleased(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Left.Y > -deadzone && this.PreviousState.ThumbSticks.Left.Y <= -deadzone;
			}

			// Token: 0x06001E65 RID: 7781 RVA: 0x000D43CC File Offset: 0x000D25CC
			public bool LeftStickUpCheck(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Left.Y >= deadzone;
			}

			// Token: 0x06001E66 RID: 7782 RVA: 0x000D43F8 File Offset: 0x000D25F8
			public bool LeftStickUpPressed(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Left.Y >= deadzone && this.PreviousState.ThumbSticks.Left.Y < deadzone;
			}

			// Token: 0x06001E67 RID: 7783 RVA: 0x000D4440 File Offset: 0x000D2640
			public bool LeftStickUpReleased(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Left.Y < deadzone && this.PreviousState.ThumbSticks.Left.Y >= deadzone;
			}

			// Token: 0x06001E68 RID: 7784 RVA: 0x000D4488 File Offset: 0x000D2688
			public float LeftStickHorizontal(float deadzone)
			{
				float x = this.CurrentState.ThumbSticks.Left.X;
				if (Math.Abs(x) < deadzone)
				{
					return 0f;
				}
				return x;
			}

			// Token: 0x06001E69 RID: 7785 RVA: 0x000D44C0 File Offset: 0x000D26C0
			public float LeftStickVertical(float deadzone)
			{
				float y = this.CurrentState.ThumbSticks.Left.Y;
				if (Math.Abs(y) < deadzone)
				{
					return 0f;
				}
				return -y;
			}

			// Token: 0x06001E6A RID: 7786 RVA: 0x000D44F8 File Offset: 0x000D26F8
			public bool RightStickLeftCheck(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Right.X <= -deadzone;
			}

			// Token: 0x06001E6B RID: 7787 RVA: 0x000D4524 File Offset: 0x000D2724
			public bool RightStickLeftPressed(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Right.X <= -deadzone && this.PreviousState.ThumbSticks.Right.X > -deadzone;
			}

			// Token: 0x06001E6C RID: 7788 RVA: 0x000D456C File Offset: 0x000D276C
			public bool RightStickLeftReleased(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Right.X > -deadzone && this.PreviousState.ThumbSticks.Right.X <= -deadzone;
			}

			// Token: 0x06001E6D RID: 7789 RVA: 0x000D45B8 File Offset: 0x000D27B8
			public bool RightStickRightCheck(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Right.X >= deadzone;
			}

			// Token: 0x06001E6E RID: 7790 RVA: 0x000D45E4 File Offset: 0x000D27E4
			public bool RightStickRightPressed(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Right.X >= deadzone && this.PreviousState.ThumbSticks.Right.X < deadzone;
			}

			// Token: 0x06001E6F RID: 7791 RVA: 0x000D462C File Offset: 0x000D282C
			public bool RightStickRightReleased(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Right.X < deadzone && this.PreviousState.ThumbSticks.Right.X >= deadzone;
			}

			// Token: 0x06001E70 RID: 7792 RVA: 0x000D4674 File Offset: 0x000D2874
			public bool RightStickDownCheck(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Right.Y <= -deadzone;
			}

			// Token: 0x06001E71 RID: 7793 RVA: 0x000D46A0 File Offset: 0x000D28A0
			public bool RightStickDownPressed(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Right.Y <= -deadzone && this.PreviousState.ThumbSticks.Right.Y > -deadzone;
			}

			// Token: 0x06001E72 RID: 7794 RVA: 0x000D46E8 File Offset: 0x000D28E8
			public bool RightStickDownReleased(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Right.Y > -deadzone && this.PreviousState.ThumbSticks.Right.Y <= -deadzone;
			}

			// Token: 0x06001E73 RID: 7795 RVA: 0x000D4734 File Offset: 0x000D2934
			public bool RightStickUpCheck(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Right.Y >= deadzone;
			}

			// Token: 0x06001E74 RID: 7796 RVA: 0x000D4760 File Offset: 0x000D2960
			public bool RightStickUpPressed(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Right.Y >= deadzone && this.PreviousState.ThumbSticks.Right.Y < deadzone;
			}

			// Token: 0x06001E75 RID: 7797 RVA: 0x000D47A8 File Offset: 0x000D29A8
			public bool RightStickUpReleased(float deadzone)
			{
				return this.CurrentState.ThumbSticks.Right.Y < deadzone && this.PreviousState.ThumbSticks.Right.Y >= deadzone;
			}

			// Token: 0x06001E76 RID: 7798 RVA: 0x000D47F0 File Offset: 0x000D29F0
			public float RightStickHorizontal(float deadzone)
			{
				float x = this.CurrentState.ThumbSticks.Right.X;
				if (Math.Abs(x) < deadzone)
				{
					return 0f;
				}
				return x;
			}

			// Token: 0x06001E77 RID: 7799 RVA: 0x000D4828 File Offset: 0x000D2A28
			public float RightStickVertical(float deadzone)
			{
				float y = this.CurrentState.ThumbSticks.Right.Y;
				if (Math.Abs(y) < deadzone)
				{
					return 0f;
				}
				return -y;
			}

			// Token: 0x17000261 RID: 609
			// (get) Token: 0x06001E78 RID: 7800 RVA: 0x000D4860 File Offset: 0x000D2A60
			public int DPadHorizontal
			{
				get
				{
					if (this.CurrentState.DPad.Right == ButtonState.Pressed)
					{
						return 1;
					}
					if (this.CurrentState.DPad.Left != ButtonState.Pressed)
					{
						return 0;
					}
					return -1;
				}
			}

			// Token: 0x17000262 RID: 610
			// (get) Token: 0x06001E79 RID: 7801 RVA: 0x000D48A0 File Offset: 0x000D2AA0
			public int DPadVertical
			{
				get
				{
					if (this.CurrentState.DPad.Down == ButtonState.Pressed)
					{
						return 1;
					}
					if (this.CurrentState.DPad.Up != ButtonState.Pressed)
					{
						return 0;
					}
					return -1;
				}
			}

			// Token: 0x17000263 RID: 611
			// (get) Token: 0x06001E7A RID: 7802 RVA: 0x000D48DE File Offset: 0x000D2ADE
			public Vector2 DPad
			{
				get
				{
					return new Vector2((float)this.DPadHorizontal, (float)this.DPadVertical);
				}
			}

			// Token: 0x17000264 RID: 612
			// (get) Token: 0x06001E7B RID: 7803 RVA: 0x000D48F4 File Offset: 0x000D2AF4
			public bool DPadLeftCheck
			{
				get
				{
					return this.CurrentState.DPad.Left == ButtonState.Pressed;
				}
			}

			// Token: 0x17000265 RID: 613
			// (get) Token: 0x06001E7C RID: 7804 RVA: 0x000D4918 File Offset: 0x000D2B18
			public bool DPadLeftPressed
			{
				get
				{
					return this.CurrentState.DPad.Left == ButtonState.Pressed && this.PreviousState.DPad.Left == ButtonState.Released;
				}
			}

			// Token: 0x17000266 RID: 614
			// (get) Token: 0x06001E7D RID: 7805 RVA: 0x000D4954 File Offset: 0x000D2B54
			public bool DPadLeftReleased
			{
				get
				{
					return this.CurrentState.DPad.Left == ButtonState.Released && this.PreviousState.DPad.Left == ButtonState.Pressed;
				}
			}

			// Token: 0x17000267 RID: 615
			// (get) Token: 0x06001E7E RID: 7806 RVA: 0x000D4990 File Offset: 0x000D2B90
			public bool DPadRightCheck
			{
				get
				{
					return this.CurrentState.DPad.Right == ButtonState.Pressed;
				}
			}

			// Token: 0x17000268 RID: 616
			// (get) Token: 0x06001E7F RID: 7807 RVA: 0x000D49B4 File Offset: 0x000D2BB4
			public bool DPadRightPressed
			{
				get
				{
					return this.CurrentState.DPad.Right == ButtonState.Pressed && this.PreviousState.DPad.Right == ButtonState.Released;
				}
			}

			// Token: 0x17000269 RID: 617
			// (get) Token: 0x06001E80 RID: 7808 RVA: 0x000D49F0 File Offset: 0x000D2BF0
			public bool DPadRightReleased
			{
				get
				{
					return this.CurrentState.DPad.Right == ButtonState.Released && this.PreviousState.DPad.Right == ButtonState.Pressed;
				}
			}

			// Token: 0x1700026A RID: 618
			// (get) Token: 0x06001E81 RID: 7809 RVA: 0x000D4A2C File Offset: 0x000D2C2C
			public bool DPadUpCheck
			{
				get
				{
					return this.CurrentState.DPad.Up == ButtonState.Pressed;
				}
			}

			// Token: 0x1700026B RID: 619
			// (get) Token: 0x06001E82 RID: 7810 RVA: 0x000D4A50 File Offset: 0x000D2C50
			public bool DPadUpPressed
			{
				get
				{
					return this.CurrentState.DPad.Up == ButtonState.Pressed && this.PreviousState.DPad.Up == ButtonState.Released;
				}
			}

			// Token: 0x1700026C RID: 620
			// (get) Token: 0x06001E83 RID: 7811 RVA: 0x000D4A8C File Offset: 0x000D2C8C
			public bool DPadUpReleased
			{
				get
				{
					return this.CurrentState.DPad.Up == ButtonState.Released && this.PreviousState.DPad.Up == ButtonState.Pressed;
				}
			}

			// Token: 0x1700026D RID: 621
			// (get) Token: 0x06001E84 RID: 7812 RVA: 0x000D4AC8 File Offset: 0x000D2CC8
			public bool DPadDownCheck
			{
				get
				{
					return this.CurrentState.DPad.Down == ButtonState.Pressed;
				}
			}

			// Token: 0x1700026E RID: 622
			// (get) Token: 0x06001E85 RID: 7813 RVA: 0x000D4AEC File Offset: 0x000D2CEC
			public bool DPadDownPressed
			{
				get
				{
					return this.CurrentState.DPad.Down == ButtonState.Pressed && this.PreviousState.DPad.Down == ButtonState.Released;
				}
			}

			// Token: 0x1700026F RID: 623
			// (get) Token: 0x06001E86 RID: 7814 RVA: 0x000D4B28 File Offset: 0x000D2D28
			public bool DPadDownReleased
			{
				get
				{
					return this.CurrentState.DPad.Down == ButtonState.Released && this.PreviousState.DPad.Down == ButtonState.Pressed;
				}
			}

			// Token: 0x06001E87 RID: 7815 RVA: 0x000D4B64 File Offset: 0x000D2D64
			public bool LeftTriggerCheck(float threshold)
			{
				return !MInput.Disabled && this.CurrentState.Triggers.Left >= threshold;
			}

			// Token: 0x06001E88 RID: 7816 RVA: 0x000D4B94 File Offset: 0x000D2D94
			public bool LeftTriggerPressed(float threshold)
			{
				return !MInput.Disabled && this.CurrentState.Triggers.Left >= threshold && this.PreviousState.Triggers.Left < threshold;
			}

			// Token: 0x06001E89 RID: 7817 RVA: 0x000D4BD8 File Offset: 0x000D2DD8
			public bool LeftTriggerReleased(float threshold)
			{
				return !MInput.Disabled && this.CurrentState.Triggers.Left < threshold && this.PreviousState.Triggers.Left >= threshold;
			}

			// Token: 0x06001E8A RID: 7818 RVA: 0x000D4C20 File Offset: 0x000D2E20
			public bool RightTriggerCheck(float threshold)
			{
				return !MInput.Disabled && this.CurrentState.Triggers.Right >= threshold;
			}

			// Token: 0x06001E8B RID: 7819 RVA: 0x000D4C50 File Offset: 0x000D2E50
			public bool RightTriggerPressed(float threshold)
			{
				return !MInput.Disabled && this.CurrentState.Triggers.Right >= threshold && this.PreviousState.Triggers.Right < threshold;
			}

			// Token: 0x06001E8C RID: 7820 RVA: 0x000D4C94 File Offset: 0x000D2E94
			public bool RightTriggerReleased(float threshold)
			{
				return !MInput.Disabled && this.CurrentState.Triggers.Right < threshold && this.PreviousState.Triggers.Right >= threshold;
			}

			// Token: 0x06001E8D RID: 7821 RVA: 0x000D4CDC File Offset: 0x000D2EDC
			public float Axis(Buttons button, float threshold)
			{
				if (MInput.Disabled)
				{
					return 0f;
				}
				if (button <= Buttons.B)
				{
					if (button <= Buttons.Back)
					{
						if (button <= Buttons.DPadLeft)
						{
							if (button - Buttons.DPadUp > 1 && button != Buttons.DPadLeft)
							{
								goto IL_379;
							}
						}
						else if (button != Buttons.DPadRight && button != Buttons.Start && button != Buttons.Back)
						{
							goto IL_379;
						}
					}
					else if (button <= Buttons.LeftShoulder)
					{
						if (button != Buttons.LeftStick && button != Buttons.RightStick && button != Buttons.LeftShoulder)
						{
							goto IL_379;
						}
					}
					else if (button != Buttons.RightShoulder && button != Buttons.A && button != Buttons.B)
					{
						goto IL_379;
					}
				}
				else if (button <= Buttons.RightThumbstickUp)
				{
					if (button <= Buttons.LeftThumbstickLeft)
					{
						if (button != Buttons.X && button != Buttons.Y)
						{
							if (button != Buttons.LeftThumbstickLeft)
							{
								goto IL_379;
							}
							if (this.CurrentState.ThumbSticks.Left.X <= -threshold)
							{
								return -this.CurrentState.ThumbSticks.Left.X;
							}
							goto IL_379;
						}
					}
					else if (button != Buttons.RightTrigger)
					{
						if (button != Buttons.LeftTrigger)
						{
							if (button != Buttons.RightThumbstickUp)
							{
								goto IL_379;
							}
							if (this.CurrentState.ThumbSticks.Right.Y >= threshold)
							{
								return this.CurrentState.ThumbSticks.Right.Y;
							}
							goto IL_379;
						}
						else
						{
							if (this.CurrentState.Triggers.Left >= threshold)
							{
								return this.CurrentState.Triggers.Left;
							}
							goto IL_379;
						}
					}
					else
					{
						if (this.CurrentState.Triggers.Right >= threshold)
						{
							return this.CurrentState.Triggers.Right;
						}
						goto IL_379;
					}
				}
				else if (button <= Buttons.RightThumbstickLeft)
				{
					if (button != Buttons.RightThumbstickDown)
					{
						if (button != Buttons.RightThumbstickRight)
						{
							if (button != Buttons.RightThumbstickLeft)
							{
								goto IL_379;
							}
							if (this.CurrentState.ThumbSticks.Right.X <= -threshold)
							{
								return -this.CurrentState.ThumbSticks.Right.X;
							}
							goto IL_379;
						}
						else
						{
							if (this.CurrentState.ThumbSticks.Right.X >= threshold)
							{
								return this.CurrentState.ThumbSticks.Right.X;
							}
							goto IL_379;
						}
					}
					else
					{
						if (this.CurrentState.ThumbSticks.Right.Y <= -threshold)
						{
							return -this.CurrentState.ThumbSticks.Right.Y;
						}
						goto IL_379;
					}
				}
				else if (button != Buttons.LeftThumbstickUp)
				{
					if (button != Buttons.LeftThumbstickDown)
					{
						if (button != Buttons.LeftThumbstickRight)
						{
							goto IL_379;
						}
						if (this.CurrentState.ThumbSticks.Left.X >= threshold)
						{
							return this.CurrentState.ThumbSticks.Left.X;
						}
						goto IL_379;
					}
					else
					{
						if (this.CurrentState.ThumbSticks.Left.Y <= -threshold)
						{
							return -this.CurrentState.ThumbSticks.Left.Y;
						}
						goto IL_379;
					}
				}
				else
				{
					if (this.CurrentState.ThumbSticks.Left.Y >= threshold)
					{
						return this.CurrentState.ThumbSticks.Left.Y;
					}
					goto IL_379;
				}
				if (this.Check(button))
				{
					return 1f;
				}
				IL_379:
				return 0f;
			}

			// Token: 0x06001E8E RID: 7822 RVA: 0x000D5068 File Offset: 0x000D3268
			public bool Check(Buttons button, float threshold)
			{
				if (MInput.Disabled)
				{
					return false;
				}
				if (button <= Buttons.B)
				{
					if (button <= Buttons.Back)
					{
						if (button <= Buttons.DPadLeft)
						{
							if (button - Buttons.DPadUp > 1 && button != Buttons.DPadLeft)
							{
								return false;
							}
						}
						else if (button != Buttons.DPadRight && button != Buttons.Start && button != Buttons.Back)
						{
							return false;
						}
					}
					else if (button <= Buttons.LeftShoulder)
					{
						if (button != Buttons.LeftStick && button != Buttons.RightStick && button != Buttons.LeftShoulder)
						{
							return false;
						}
					}
					else if (button != Buttons.RightShoulder && button != Buttons.A && button != Buttons.B)
					{
						return false;
					}
				}
				else if (button <= Buttons.RightThumbstickUp)
				{
					if (button <= Buttons.LeftThumbstickLeft)
					{
						if (button != Buttons.X && button != Buttons.Y)
						{
							if (button != Buttons.LeftThumbstickLeft)
							{
								return false;
							}
							if (this.LeftStickLeftCheck(threshold))
							{
								return true;
							}
							return false;
						}
					}
					else if (button != Buttons.RightTrigger)
					{
						if (button != Buttons.LeftTrigger)
						{
							if (button != Buttons.RightThumbstickUp)
							{
								return false;
							}
							if (this.RightStickUpCheck(threshold))
							{
								return true;
							}
							return false;
						}
						else
						{
							if (this.LeftTriggerCheck(threshold))
							{
								return true;
							}
							return false;
						}
					}
					else
					{
						if (this.RightTriggerCheck(threshold))
						{
							return true;
						}
						return false;
					}
				}
				else if (button <= Buttons.RightThumbstickLeft)
				{
					if (button != Buttons.RightThumbstickDown)
					{
						if (button != Buttons.RightThumbstickRight)
						{
							if (button != Buttons.RightThumbstickLeft)
							{
								return false;
							}
							if (this.RightStickLeftCheck(threshold))
							{
								return true;
							}
							return false;
						}
						else
						{
							if (this.RightStickRightCheck(threshold))
							{
								return true;
							}
							return false;
						}
					}
					else
					{
						if (this.RightStickDownCheck(threshold))
						{
							return true;
						}
						return false;
					}
				}
				else if (button != Buttons.LeftThumbstickUp)
				{
					if (button != Buttons.LeftThumbstickDown)
					{
						if (button != Buttons.LeftThumbstickRight)
						{
							return false;
						}
						if (this.LeftStickRightCheck(threshold))
						{
							return true;
						}
						return false;
					}
					else
					{
						if (this.LeftStickDownCheck(threshold))
						{
							return true;
						}
						return false;
					}
				}
				else
				{
					if (this.LeftStickUpCheck(threshold))
					{
						return true;
					}
					return false;
				}
				if (this.Check(button))
				{
					return true;
				}
				return false;
			}

			// Token: 0x06001E8F RID: 7823 RVA: 0x000D522C File Offset: 0x000D342C
			public bool Pressed(Buttons button, float threshold)
			{
				if (MInput.Disabled)
				{
					return false;
				}
				if (button <= Buttons.B)
				{
					if (button <= Buttons.Back)
					{
						if (button <= Buttons.DPadLeft)
						{
							if (button - Buttons.DPadUp > 1 && button != Buttons.DPadLeft)
							{
								return false;
							}
						}
						else if (button != Buttons.DPadRight && button != Buttons.Start && button != Buttons.Back)
						{
							return false;
						}
					}
					else if (button <= Buttons.LeftShoulder)
					{
						if (button != Buttons.LeftStick && button != Buttons.RightStick && button != Buttons.LeftShoulder)
						{
							return false;
						}
					}
					else if (button != Buttons.RightShoulder && button != Buttons.A && button != Buttons.B)
					{
						return false;
					}
				}
				else if (button <= Buttons.RightThumbstickUp)
				{
					if (button <= Buttons.LeftThumbstickLeft)
					{
						if (button != Buttons.X && button != Buttons.Y)
						{
							if (button != Buttons.LeftThumbstickLeft)
							{
								return false;
							}
							if (this.LeftStickLeftPressed(threshold))
							{
								return true;
							}
							return false;
						}
					}
					else if (button != Buttons.RightTrigger)
					{
						if (button != Buttons.LeftTrigger)
						{
							if (button != Buttons.RightThumbstickUp)
							{
								return false;
							}
							if (this.RightStickUpPressed(threshold))
							{
								return true;
							}
							return false;
						}
						else
						{
							if (this.LeftTriggerPressed(threshold))
							{
								return true;
							}
							return false;
						}
					}
					else
					{
						if (this.RightTriggerPressed(threshold))
						{
							return true;
						}
						return false;
					}
				}
				else if (button <= Buttons.RightThumbstickLeft)
				{
					if (button != Buttons.RightThumbstickDown)
					{
						if (button != Buttons.RightThumbstickRight)
						{
							if (button != Buttons.RightThumbstickLeft)
							{
								return false;
							}
							if (this.RightStickLeftPressed(threshold))
							{
								return true;
							}
							return false;
						}
						else
						{
							if (this.RightStickRightPressed(threshold))
							{
								return true;
							}
							return false;
						}
					}
					else
					{
						if (this.RightStickDownPressed(threshold))
						{
							return true;
						}
						return false;
					}
				}
				else if (button != Buttons.LeftThumbstickUp)
				{
					if (button != Buttons.LeftThumbstickDown)
					{
						if (button != Buttons.LeftThumbstickRight)
						{
							return false;
						}
						if (this.LeftStickRightPressed(threshold))
						{
							return true;
						}
						return false;
					}
					else
					{
						if (this.LeftStickDownPressed(threshold))
						{
							return true;
						}
						return false;
					}
				}
				else
				{
					if (this.LeftStickUpPressed(threshold))
					{
						return true;
					}
					return false;
				}
				if (this.Pressed(button))
				{
					return true;
				}
				return false;
			}

			// Token: 0x06001E90 RID: 7824 RVA: 0x000D53F0 File Offset: 0x000D35F0
			public bool Released(Buttons button, float threshold)
			{
				if (MInput.Disabled)
				{
					return false;
				}
				if (button <= Buttons.B)
				{
					if (button <= Buttons.Back)
					{
						if (button <= Buttons.DPadLeft)
						{
							if (button - Buttons.DPadUp > 1 && button != Buttons.DPadLeft)
							{
								return false;
							}
						}
						else if (button != Buttons.DPadRight && button != Buttons.Start && button != Buttons.Back)
						{
							return false;
						}
					}
					else if (button <= Buttons.LeftShoulder)
					{
						if (button != Buttons.LeftStick && button != Buttons.RightStick && button != Buttons.LeftShoulder)
						{
							return false;
						}
					}
					else if (button != Buttons.RightShoulder && button != Buttons.A && button != Buttons.B)
					{
						return false;
					}
				}
				else if (button <= Buttons.RightThumbstickUp)
				{
					if (button <= Buttons.LeftThumbstickLeft)
					{
						if (button != Buttons.X && button != Buttons.Y)
						{
							if (button != Buttons.LeftThumbstickLeft)
							{
								return false;
							}
							if (this.LeftStickLeftReleased(threshold))
							{
								return true;
							}
							return false;
						}
					}
					else if (button != Buttons.RightTrigger)
					{
						if (button != Buttons.LeftTrigger)
						{
							if (button != Buttons.RightThumbstickUp)
							{
								return false;
							}
							if (this.RightStickUpReleased(threshold))
							{
								return true;
							}
							return false;
						}
						else
						{
							if (this.LeftTriggerReleased(threshold))
							{
								return true;
							}
							return false;
						}
					}
					else
					{
						if (this.RightTriggerReleased(threshold))
						{
							return true;
						}
						return false;
					}
				}
				else if (button <= Buttons.RightThumbstickLeft)
				{
					if (button != Buttons.RightThumbstickDown)
					{
						if (button != Buttons.RightThumbstickRight)
						{
							if (button != Buttons.RightThumbstickLeft)
							{
								return false;
							}
							if (this.RightStickLeftReleased(threshold))
							{
								return true;
							}
							return false;
						}
						else
						{
							if (this.RightStickRightReleased(threshold))
							{
								return true;
							}
							return false;
						}
					}
					else
					{
						if (this.RightStickDownReleased(threshold))
						{
							return true;
						}
						return false;
					}
				}
				else if (button != Buttons.LeftThumbstickUp)
				{
					if (button != Buttons.LeftThumbstickDown)
					{
						if (button != Buttons.LeftThumbstickRight)
						{
							return false;
						}
						if (this.LeftStickRightReleased(threshold))
						{
							return true;
						}
						return false;
					}
					else
					{
						if (this.LeftStickDownReleased(threshold))
						{
							return true;
						}
						return false;
					}
				}
				else
				{
					if (this.LeftStickUpReleased(threshold))
					{
						return true;
					}
					return false;
				}
				if (this.Released(button))
				{
					return true;
				}
				return false;
			}

			// Token: 0x04001F20 RID: 7968
			public readonly PlayerIndex PlayerIndex;

			// Token: 0x04001F21 RID: 7969
			public GamePadState PreviousState;

			// Token: 0x04001F22 RID: 7970
			public GamePadState CurrentState;

			// Token: 0x04001F23 RID: 7971
			public bool Attached;

			// Token: 0x04001F24 RID: 7972
			public bool HadInputThisFrame;

			// Token: 0x04001F25 RID: 7973
			private float rumbleStrength;

			// Token: 0x04001F26 RID: 7974
			private float rumbleTime;
		}
	}
}
