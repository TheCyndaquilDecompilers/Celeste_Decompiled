using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste.Editor
{
	// Token: 0x02000394 RID: 916
	public class MapEditor : Scene
	{
		// Token: 0x06001DB7 RID: 7607 RVA: 0x000CF2D0 File Offset: 0x000CD4D0
		public MapEditor(AreaKey area, bool reloadMapData = true)
		{
			area.ID = Calc.Clamp(area.ID, 0, AreaData.Areas.Count - 1);
			this.mapData = AreaData.Areas[area.ID].Mode[(int)area.Mode].MapData;
			if (reloadMapData)
			{
				this.mapData.Reload();
			}
			foreach (LevelData data in this.mapData.Levels)
			{
				this.levels.Add(new LevelTemplate(data));
			}
			foreach (Rectangle rectangle in this.mapData.Filler)
			{
				this.levels.Add(new LevelTemplate(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height));
			}
			if (area != MapEditor.area)
			{
				MapEditor.area = area;
				MapEditor.Camera = new Camera();
				MapEditor.Camera.Zoom = 6f;
				MapEditor.Camera.CenterOrigin();
			}
			if (SaveData.Instance == null)
			{
				SaveData.InitializeDebugMode(true);
			}
		}

		// Token: 0x06001DB8 RID: 7608 RVA: 0x000CF470 File Offset: 0x000CD670
		public override void GainFocus()
		{
			base.GainFocus();
			this.SaveAndReload();
		}

		// Token: 0x06001DB9 RID: 7609 RVA: 0x000CF480 File Offset: 0x000CD680
		private void SelectAll()
		{
			this.selection.Clear();
			foreach (LevelTemplate item in this.levels)
			{
				this.selection.Add(item);
			}
		}

		// Token: 0x06001DBA RID: 7610 RVA: 0x000CF4E4 File Offset: 0x000CD6E4
		public void Rename(string oldName, string newName)
		{
			LevelTemplate levelTemplate = null;
			LevelTemplate levelTemplate2 = null;
			foreach (LevelTemplate levelTemplate3 in this.levels)
			{
				if (levelTemplate == null && levelTemplate3.Name == oldName)
				{
					levelTemplate = levelTemplate3;
					if (levelTemplate2 != null)
					{
						break;
					}
				}
				else if (levelTemplate2 == null && levelTemplate3.Name == newName)
				{
					levelTemplate2 = levelTemplate3;
					if (levelTemplate != null)
					{
						break;
					}
				}
			}
			string path = Path.Combine(new string[]
			{
				"..",
				"..",
				"..",
				"Content",
				"Levels",
				this.mapData.Filename
			});
			if (levelTemplate2 == null)
			{
				File.Move(Path.Combine(path, levelTemplate.Name + ".xml"), Path.Combine(path, newName + ".xml"));
				levelTemplate.Name = newName;
			}
			else
			{
				string text = Path.Combine(path, "TEMP.xml");
				File.Move(Path.Combine(path, levelTemplate.Name + ".xml"), text);
				File.Move(Path.Combine(path, levelTemplate2.Name + ".xml"), Path.Combine(path, oldName + ".xml"));
				File.Move(text, Path.Combine(path, newName + ".xml"));
				levelTemplate.Name = newName;
				levelTemplate2.Name = oldName;
			}
			this.Save();
		}

		// Token: 0x06001DBB RID: 7611 RVA: 0x000091E2 File Offset: 0x000073E2
		private void Save()
		{
		}

		// Token: 0x06001DBC RID: 7612 RVA: 0x000091E2 File Offset: 0x000073E2
		private void SaveAndReload()
		{
		}

		// Token: 0x06001DBD RID: 7613 RVA: 0x000CF668 File Offset: 0x000CD868
		private void UpdateMouse()
		{
			this.mousePosition = Vector2.Transform(MInput.Mouse.Position, Matrix.Invert(MapEditor.Camera.Matrix));
		}

		// Token: 0x06001DBE RID: 7614 RVA: 0x000CF690 File Offset: 0x000CD890
		public override void Update()
		{
			Vector2 value;
			value.X = (this.lastMouseScreenPosition.X - MInput.Mouse.Position.X) / MapEditor.Camera.Zoom;
			value.Y = (this.lastMouseScreenPosition.Y - MInput.Mouse.Position.Y) / MapEditor.Camera.Zoom;
			if (MInput.Keyboard.Pressed(Keys.Space) && MInput.Keyboard.Check(Keys.LeftControl))
			{
				MapEditor.Camera.Zoom = 6f;
				MapEditor.Camera.Position = Vector2.Zero;
			}
			int num = Math.Sign(MInput.Mouse.WheelDelta);
			if ((num > 0 && MapEditor.Camera.Zoom >= 1f) || MapEditor.Camera.Zoom > 1f)
			{
				MapEditor.Camera.Zoom += (float)num;
			}
			else
			{
				MapEditor.Camera.Zoom += (float)num * 0.25f;
			}
			MapEditor.Camera.Zoom = Math.Max(0.25f, Math.Min(24f, MapEditor.Camera.Zoom));
			MapEditor.Camera.Position += new Vector2((float)Input.MoveX.Value, (float)Input.MoveY.Value) * 300f * Engine.DeltaTime;
			this.UpdateMouse();
			this.hovered.Clear();
			if (this.mouseMode == MapEditor.MouseModes.Hover)
			{
				this.mouseDragStart = this.mousePosition;
				if (MInput.Mouse.PressedLeftButton)
				{
					bool flag = this.LevelCheck(this.mousePosition);
					if (MInput.Keyboard.Check(Keys.Space))
					{
						this.mouseMode = MapEditor.MouseModes.Pan;
					}
					else if (MInput.Keyboard.Check(Keys.LeftControl))
					{
						if (flag)
						{
							this.ToggleSelection(this.mousePosition);
						}
						else
						{
							this.mouseMode = MapEditor.MouseModes.Select;
						}
					}
					else if (MInput.Keyboard.Check(Keys.F))
					{
						this.levels.Add(new LevelTemplate((int)this.mousePosition.X, (int)this.mousePosition.Y, 32, 32));
					}
					else if (flag)
					{
						if (!this.SelectionCheck(this.mousePosition))
						{
							this.SetSelection(this.mousePosition);
						}
						bool flag2 = false;
						if (this.selection.Count == 1)
						{
							foreach (LevelTemplate levelTemplate in this.selection)
							{
								if (levelTemplate.ResizePosition(this.mousePosition) && levelTemplate.Type == LevelTemplateType.Filler)
								{
									flag2 = true;
								}
							}
						}
						if (flag2)
						{
							foreach (LevelTemplate levelTemplate2 in this.selection)
							{
								levelTemplate2.StartResizing();
							}
							this.mouseMode = MapEditor.MouseModes.Resize;
						}
						else
						{
							this.StoreUndo();
							foreach (LevelTemplate levelTemplate3 in this.selection)
							{
								levelTemplate3.StartMoving();
							}
							this.mouseMode = MapEditor.MouseModes.Move;
						}
					}
					else
					{
						this.mouseMode = MapEditor.MouseModes.Select;
					}
				}
				else if (MInput.Mouse.PressedRightButton)
				{
					LevelTemplate levelTemplate4 = this.TestCheck(this.mousePosition);
					if (levelTemplate4 != null)
					{
						if (levelTemplate4.Type == LevelTemplateType.Filler)
						{
							if (MInput.Keyboard.Check(Keys.F))
							{
								this.levels.Remove(levelTemplate4);
								return;
							}
						}
						else
						{
							this.LoadLevel(levelTemplate4, this.mousePosition * 8f);
						}
						return;
					}
				}
				else if (MInput.Mouse.PressedMiddleButton)
				{
					this.mouseMode = MapEditor.MouseModes.Pan;
				}
				else if (!MInput.Keyboard.Check(Keys.Space))
				{
					foreach (LevelTemplate levelTemplate5 in this.levels)
					{
						if (levelTemplate5.Check(this.mousePosition))
						{
							this.hovered.Add(levelTemplate5);
						}
					}
					if (MInput.Keyboard.Check(Keys.LeftControl))
					{
						if (MInput.Keyboard.Pressed(Keys.Z))
						{
							this.Undo();
						}
						else if (MInput.Keyboard.Pressed(Keys.Y))
						{
							this.Redo();
						}
						else if (MInput.Keyboard.Pressed(Keys.A))
						{
							this.SelectAll();
						}
					}
				}
			}
			else if (this.mouseMode == MapEditor.MouseModes.Pan)
			{
				MapEditor.Camera.Position += value;
				if (!MInput.Mouse.CheckLeftButton && !MInput.Mouse.CheckMiddleButton)
				{
					this.mouseMode = MapEditor.MouseModes.Hover;
				}
			}
			else if (this.mouseMode == MapEditor.MouseModes.Select)
			{
				Rectangle mouseRect = this.GetMouseRect(this.mouseDragStart, this.mousePosition);
				foreach (LevelTemplate levelTemplate6 in this.levels)
				{
					if (levelTemplate6.Check(mouseRect))
					{
						this.hovered.Add(levelTemplate6);
					}
				}
				if (!MInput.Mouse.CheckLeftButton)
				{
					if (MInput.Keyboard.Check(Keys.LeftControl))
					{
						this.ToggleSelection(mouseRect);
					}
					else
					{
						this.SetSelection(mouseRect);
					}
					this.mouseMode = MapEditor.MouseModes.Hover;
				}
			}
			else if (this.mouseMode == MapEditor.MouseModes.Move)
			{
				Vector2 relativeMove = (this.mousePosition - this.mouseDragStart).Round();
				bool snap = this.selection.Count == 1 && !MInput.Keyboard.Check(Keys.LeftAlt);
				foreach (LevelTemplate levelTemplate7 in this.selection)
				{
					levelTemplate7.Move(relativeMove, this.levels, snap);
				}
				if (!MInput.Mouse.CheckLeftButton)
				{
					this.mouseMode = MapEditor.MouseModes.Hover;
				}
			}
			else if (this.mouseMode == MapEditor.MouseModes.Resize)
			{
				Vector2 relativeMove2 = (this.mousePosition - this.mouseDragStart).Round();
				foreach (LevelTemplate levelTemplate8 in this.selection)
				{
					levelTemplate8.Resize(relativeMove2);
				}
				if (!MInput.Mouse.CheckLeftButton)
				{
					this.mouseMode = MapEditor.MouseModes.Hover;
				}
			}
			if (MInput.Keyboard.Pressed(Keys.D1))
			{
				this.SetEditorColor(0);
			}
			else if (MInput.Keyboard.Pressed(Keys.D2))
			{
				this.SetEditorColor(1);
			}
			else if (MInput.Keyboard.Pressed(Keys.D3))
			{
				this.SetEditorColor(2);
			}
			else if (MInput.Keyboard.Pressed(Keys.D4))
			{
				this.SetEditorColor(3);
			}
			else if (MInput.Keyboard.Pressed(Keys.D5))
			{
				this.SetEditorColor(4);
			}
			else if (MInput.Keyboard.Pressed(Keys.D6))
			{
				this.SetEditorColor(5);
			}
			else if (MInput.Keyboard.Pressed(Keys.D7))
			{
				this.SetEditorColor(6);
			}
			if (MInput.Keyboard.Pressed(Keys.F1) || (MInput.Keyboard.Check(Keys.LeftControl) && MInput.Keyboard.Pressed(Keys.S)))
			{
				this.SaveAndReload();
				return;
			}
			if (MapEditor.saveFlash > 0f)
			{
				MapEditor.saveFlash -= Engine.DeltaTime * 4f;
			}
			this.lastMouseScreenPosition = MInput.Mouse.Position;
			base.Update();
		}

		// Token: 0x06001DBF RID: 7615 RVA: 0x000CFE7C File Offset: 0x000CE07C
		private void SetEditorColor(int index)
		{
			foreach (LevelTemplate levelTemplate in this.selection)
			{
				levelTemplate.EditorColorIndex = index;
			}
		}

		// Token: 0x06001DC0 RID: 7616 RVA: 0x000CFED0 File Offset: 0x000CE0D0
		public override void Render()
		{
			this.UpdateMouse();
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, MapEditor.Camera.Matrix * Engine.ScreenMatrix);
			float num = 1920f / MapEditor.Camera.Zoom;
			float num2 = 1080f / MapEditor.Camera.Zoom;
			int num3 = 5;
			float num4 = (float)Math.Floor((double)(MapEditor.Camera.Left / (float)num3 - 1f)) * (float)num3;
			float num5 = (float)Math.Floor((double)(MapEditor.Camera.Top / (float)num3 - 1f)) * (float)num3;
			for (float num6 = num4; num6 <= num4 + num + 10f; num6 += 5f)
			{
				Draw.Line(num6, MapEditor.Camera.Top, num6, MapEditor.Camera.Top + num2, MapEditor.gridColor);
			}
			for (float num7 = num5; num7 <= num5 + num2 + 10f; num7 += 5f)
			{
				Draw.Line(MapEditor.Camera.Left, num7, MapEditor.Camera.Left + num, num7, MapEditor.gridColor);
			}
			Draw.Line(0f, MapEditor.Camera.Top, 0f, MapEditor.Camera.Top + num2, Color.DarkSlateBlue, 1f / MapEditor.Camera.Zoom);
			Draw.Line(MapEditor.Camera.Left, 0f, MapEditor.Camera.Left + num, 0f, Color.DarkSlateBlue, 1f / MapEditor.Camera.Zoom);
			foreach (LevelTemplate levelTemplate in this.levels)
			{
				levelTemplate.RenderContents(MapEditor.Camera, this.levels);
			}
			foreach (LevelTemplate levelTemplate2 in this.levels)
			{
				levelTemplate2.RenderOutline(MapEditor.Camera);
			}
			foreach (LevelTemplate levelTemplate3 in this.levels)
			{
				levelTemplate3.RenderHighlight(MapEditor.Camera, this.selection.Contains(levelTemplate3), this.hovered.Contains(levelTemplate3));
			}
			if (this.mouseMode == MapEditor.MouseModes.Hover)
			{
				Draw.Line(this.mousePosition.X - 12f / MapEditor.Camera.Zoom, this.mousePosition.Y, this.mousePosition.X + 12f / MapEditor.Camera.Zoom, this.mousePosition.Y, Color.Yellow, 3f / MapEditor.Camera.Zoom);
				Draw.Line(this.mousePosition.X, this.mousePosition.Y - 12f / MapEditor.Camera.Zoom, this.mousePosition.X, this.mousePosition.Y + 12f / MapEditor.Camera.Zoom, Color.Yellow, 3f / MapEditor.Camera.Zoom);
			}
			else if (this.mouseMode == MapEditor.MouseModes.Select)
			{
				Draw.Rect(this.GetMouseRect(this.mouseDragStart, this.mousePosition), Color.Lime * 0.25f);
			}
			if (MapEditor.saveFlash > 0f)
			{
				Draw.Rect(MapEditor.Camera.Left, MapEditor.Camera.Top, num, num2, Color.White * Ease.CubeInOut(MapEditor.saveFlash));
			}
			if (this.fade > 0f)
			{
				Draw.Rect(0f, 0f, 320f, 180f, Color.Black * this.fade);
			}
			Draw.SpriteBatch.End();
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.ScreenMatrix);
			Draw.Rect(0f, 0f, 1920f, 72f, Color.Black);
			Vector2 position = new Vector2(16f, 4f);
			Vector2 position2 = new Vector2(1904f, 4f);
			if (MInput.Keyboard.Check(Keys.Q))
			{
				Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.25f);
				foreach (LevelTemplate levelTemplate4 in this.levels)
				{
					int num8 = 0;
					while (levelTemplate4.Strawberries != null && num8 < levelTemplate4.Strawberries.Count)
					{
						Vector2 vector = levelTemplate4.Strawberries[num8];
						ActiveFont.DrawOutline(levelTemplate4.StrawberryMetadata[num8], (new Vector2((float)levelTemplate4.X + vector.X, (float)levelTemplate4.Y + vector.Y) - MapEditor.Camera.Position) * MapEditor.Camera.Zoom + new Vector2(960f, 532f), new Vector2(0.5f, 1f), Vector2.One * 1f, Color.Red, 2f, Color.Black);
						num8++;
					}
				}
			}
			if (this.hovered.Count == 0)
			{
				if (this.selection.Count > 0)
				{
					ActiveFont.Draw(this.selection.Count + " levels selected", position, Color.Red);
				}
				else
				{
					ActiveFont.Draw(Dialog.Clean(this.mapData.Data.Name, null), position, Color.Aqua);
					ActiveFont.Draw(this.mapData.Area.Mode + " MODE", position2, Vector2.UnitX, Vector2.One, Color.Red);
				}
			}
			else if (this.hovered.Count == 1)
			{
				LevelTemplate levelTemplate5 = null;
				using (HashSet<LevelTemplate>.Enumerator enumerator2 = this.hovered.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						levelTemplate5 = enumerator2.Current;
					}
				}
				string text = string.Concat(new object[]
				{
					levelTemplate5.ActualWidth.ToString(),
					"x",
					levelTemplate5.ActualHeight.ToString(),
					"   ",
					levelTemplate5.X,
					",",
					levelTemplate5.Y,
					"   ",
					levelTemplate5.X * 8,
					",",
					levelTemplate5.Y * 8
				});
				ActiveFont.Draw(levelTemplate5.Name, position, Color.Yellow);
				ActiveFont.Draw(text, position2, Vector2.UnitX, Vector2.One, Color.Green);
			}
			else
			{
				ActiveFont.Draw(this.hovered.Count + " levels", position, Color.Yellow);
			}
			Draw.SpriteBatch.End();
		}

		// Token: 0x06001DC1 RID: 7617 RVA: 0x000D068C File Offset: 0x000CE88C
		private void LoadLevel(LevelTemplate level, Vector2 at)
		{
			this.Save();
			Engine.Scene = new LevelLoader(new Session(MapEditor.area, null, null)
			{
				FirstLevel = false,
				Level = level.Name,
				StartedFromBeginning = false
			}, new Vector2?(at));
		}

		// Token: 0x06001DC2 RID: 7618 RVA: 0x000D06CC File Offset: 0x000CE8CC
		private void StoreUndo()
		{
			Vector2[] array = new Vector2[this.levels.Count];
			for (int i = 0; i < this.levels.Count; i++)
			{
				array[i] = new Vector2((float)this.levels[i].X, (float)this.levels[i].Y);
			}
			this.undoStack.Add(array);
			while (this.undoStack.Count > 30)
			{
				this.undoStack.RemoveAt(0);
			}
			this.redoStack.Clear();
		}

		// Token: 0x06001DC3 RID: 7619 RVA: 0x000D0764 File Offset: 0x000CE964
		private void Undo()
		{
			if (this.undoStack.Count > 0)
			{
				Vector2[] array = new Vector2[this.levels.Count];
				for (int i = 0; i < this.levels.Count; i++)
				{
					array[i] = new Vector2((float)this.levels[i].X, (float)this.levels[i].Y);
				}
				this.redoStack.Add(array);
				Vector2[] array2 = this.undoStack[this.undoStack.Count - 1];
				this.undoStack.RemoveAt(this.undoStack.Count - 1);
				for (int j = 0; j < array2.Length; j++)
				{
					this.levels[j].X = (int)array2[j].X;
					this.levels[j].Y = (int)array2[j].Y;
				}
			}
		}

		// Token: 0x06001DC4 RID: 7620 RVA: 0x000D0860 File Offset: 0x000CEA60
		private void Redo()
		{
			if (this.redoStack.Count > 0)
			{
				Vector2[] array = new Vector2[this.levels.Count];
				for (int i = 0; i < this.levels.Count; i++)
				{
					array[i] = new Vector2((float)this.levels[i].X, (float)this.levels[i].Y);
				}
				this.undoStack.Add(array);
				Vector2[] array2 = this.redoStack[this.undoStack.Count - 1];
				this.redoStack.RemoveAt(this.undoStack.Count - 1);
				for (int j = 0; j < array2.Length; j++)
				{
					this.levels[j].X = (int)array2[j].X;
					this.levels[j].Y = (int)array2[j].Y;
				}
			}
		}

		// Token: 0x06001DC5 RID: 7621 RVA: 0x000D095C File Offset: 0x000CEB5C
		private Rectangle GetMouseRect(Vector2 a, Vector2 b)
		{
			Vector2 vector = new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
			Vector2 vector2 = new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
			return new Rectangle((int)vector.X, (int)vector.Y, (int)(vector2.X - vector.X), (int)(vector2.Y - vector.Y));
		}

		// Token: 0x06001DC6 RID: 7622 RVA: 0x000D09EC File Offset: 0x000CEBEC
		private LevelTemplate TestCheck(Vector2 point)
		{
			foreach (LevelTemplate levelTemplate in this.levels)
			{
				if (!levelTemplate.Dummy && levelTemplate.Check(point))
				{
					return levelTemplate;
				}
			}
			return null;
		}

		// Token: 0x06001DC7 RID: 7623 RVA: 0x000D0A50 File Offset: 0x000CEC50
		private bool LevelCheck(Vector2 point)
		{
			using (List<LevelTemplate>.Enumerator enumerator = this.levels.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Check(point))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001DC8 RID: 7624 RVA: 0x000D0AAC File Offset: 0x000CECAC
		private bool SelectionCheck(Vector2 point)
		{
			using (HashSet<LevelTemplate>.Enumerator enumerator = this.selection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Check(point))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001DC9 RID: 7625 RVA: 0x000D0B08 File Offset: 0x000CED08
		private bool SetSelection(Vector2 point)
		{
			this.selection.Clear();
			foreach (LevelTemplate levelTemplate in this.levels)
			{
				if (levelTemplate.Check(point))
				{
					this.selection.Add(levelTemplate);
				}
			}
			return this.selection.Count > 0;
		}

		// Token: 0x06001DCA RID: 7626 RVA: 0x000D0B84 File Offset: 0x000CED84
		private bool ToggleSelection(Vector2 point)
		{
			bool result = false;
			foreach (LevelTemplate levelTemplate in this.levels)
			{
				if (levelTemplate.Check(point))
				{
					result = true;
					if (this.selection.Contains(levelTemplate))
					{
						this.selection.Remove(levelTemplate);
					}
					else
					{
						this.selection.Add(levelTemplate);
					}
				}
			}
			return result;
		}

		// Token: 0x06001DCB RID: 7627 RVA: 0x000D0C08 File Offset: 0x000CEE08
		private void SetSelection(Rectangle rect)
		{
			this.selection.Clear();
			foreach (LevelTemplate levelTemplate in this.levels)
			{
				if (levelTemplate.Check(rect))
				{
					this.selection.Add(levelTemplate);
				}
			}
		}

		// Token: 0x06001DCC RID: 7628 RVA: 0x000D0C78 File Offset: 0x000CEE78
		private void ToggleSelection(Rectangle rect)
		{
			foreach (LevelTemplate levelTemplate in this.levels)
			{
				if (levelTemplate.Check(rect))
				{
					if (this.selection.Contains(levelTemplate))
					{
						this.selection.Remove(levelTemplate);
					}
					else
					{
						this.selection.Add(levelTemplate);
					}
				}
			}
		}

		// Token: 0x04001E88 RID: 7816
		private static readonly Color gridColor = new Color(0.1f, 0.1f, 0.1f);

		// Token: 0x04001E89 RID: 7817
		private static Camera Camera;

		// Token: 0x04001E8A RID: 7818
		private static AreaKey area = AreaKey.None;

		// Token: 0x04001E8B RID: 7819
		private static float saveFlash = 0f;

		// Token: 0x04001E8C RID: 7820
		private MapData mapData;

		// Token: 0x04001E8D RID: 7821
		private List<LevelTemplate> levels = new List<LevelTemplate>();

		// Token: 0x04001E8E RID: 7822
		private Vector2 mousePosition;

		// Token: 0x04001E8F RID: 7823
		private MapEditor.MouseModes mouseMode;

		// Token: 0x04001E90 RID: 7824
		private Vector2 lastMouseScreenPosition;

		// Token: 0x04001E91 RID: 7825
		private Vector2 mouseDragStart;

		// Token: 0x04001E92 RID: 7826
		private HashSet<LevelTemplate> selection = new HashSet<LevelTemplate>();

		// Token: 0x04001E93 RID: 7827
		private HashSet<LevelTemplate> hovered = new HashSet<LevelTemplate>();

		// Token: 0x04001E94 RID: 7828
		private float fade;

		// Token: 0x04001E95 RID: 7829
		private List<Vector2[]> undoStack = new List<Vector2[]>();

		// Token: 0x04001E96 RID: 7830
		private List<Vector2[]> redoStack = new List<Vector2[]>();

		// Token: 0x02000759 RID: 1881
		private enum MouseModes
		{
			// Token: 0x04002EE8 RID: 12008
			Hover,
			// Token: 0x04002EE9 RID: 12009
			Pan,
			// Token: 0x04002EEA RID: 12010
			Select,
			// Token: 0x04002EEB RID: 12011
			Move,
			// Token: 0x04002EEC RID: 12012
			Resize
		}
	}
}
