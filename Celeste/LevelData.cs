using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x02000371 RID: 881
	public class LevelData
	{
		// Token: 0x06001BA3 RID: 7075 RVA: 0x000B5E68 File Offset: 0x000B4068
		public LevelData(BinaryPacker.Element data)
		{
			this.Bounds = default(Rectangle);
			foreach (KeyValuePair<string, object> keyValuePair in data.Attributes)
			{
				string key = keyValuePair.Key;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
				if (num <= 2369371622U)
				{
					if (num <= 894689925U)
					{
						if (num <= 450916041U)
						{
							if (num != 400583184U)
							{
								if (num != 434138422U)
								{
									if (num == 450916041U)
									{
										if (key == "musicLayer2")
										{
											this.MusicLayers[1] = (((bool)keyValuePair.Value) ? 1f : 0f);
										}
									}
								}
								else if (key == "musicLayer3")
								{
									this.MusicLayers[2] = (((bool)keyValuePair.Value) ? 1f : 0f);
								}
							}
							else if (key == "musicLayer1")
							{
								this.MusicLayers[0] = (((bool)keyValuePair.Value) ? 1f : 0f);
							}
						}
						else if (num != 484471279U)
						{
							if (num != 785355184U)
							{
								if (num == 894689925U)
								{
									if (key == "space")
									{
										this.Space = (bool)keyValuePair.Value;
									}
								}
							}
							else if (key == "disableDownTransition")
							{
								this.DisableDownTransition = (bool)keyValuePair.Value;
							}
						}
						else if (key == "musicLayer4")
						{
							this.MusicLayers[3] = (((bool)keyValuePair.Value) ? 1f : 0f);
						}
					}
					else if (num <= 1832650111U)
					{
						if (num != 1670160576U)
						{
							if (num != 1686938195U)
							{
								if (num == 1832650111U)
								{
									if (key == "musicProgress")
									{
										string text = keyValuePair.Value.ToString();
										if (string.IsNullOrEmpty(text) || !int.TryParse(text, out this.MusicProgress))
										{
											this.MusicProgress = -1;
										}
									}
								}
							}
							else if (key == "cameraOffsetX")
							{
								this.CameraOffset.X = Convert.ToSingle(keyValuePair.Value, CultureInfo.InvariantCulture);
							}
						}
						else if (key == "cameraOffsetY")
						{
							this.CameraOffset.Y = Convert.ToSingle(keyValuePair.Value, CultureInfo.InvariantCulture);
						}
					}
					else if (num != 2202603688U)
					{
						if (num != 2336083335U)
						{
							if (num == 2369371622U)
							{
								if (key == "name")
								{
									this.Name = keyValuePair.Value.ToString().Substring(4);
								}
							}
						}
						else if (key == "windPattern")
						{
							this.WindPattern = (WindController.Patterns)Enum.Parse(typeof(WindController.Patterns), (string)keyValuePair.Value);
						}
					}
					else if (key == "underwater")
					{
						this.Underwater = (bool)keyValuePair.Value;
					}
				}
				else if (num <= 2931535181U)
				{
					if (num <= 2508680735U)
					{
						if (num != 2415059504U)
						{
							if (num != 2415649054U)
							{
								if (num == 2508680735U)
								{
									if (key == "width")
									{
										this.Bounds.Width = (int)keyValuePair.Value;
									}
								}
							}
							else if (key == "ambienceProgress")
							{
								string text2 = keyValuePair.Value.ToString();
								if (string.IsNullOrEmpty(text2) || !int.TryParse(text2, out this.AmbienceProgress))
								{
									this.AmbienceProgress = -1;
								}
							}
						}
						else if (key == "alt_music")
						{
							this.AltMusic = (string)keyValuePair.Value;
						}
					}
					else if (num != 2677821396U)
					{
						if (num != 2834889848U)
						{
							if (num == 2931535181U)
							{
								if (key == "ambience")
								{
									this.Ambience = (string)keyValuePair.Value;
								}
							}
						}
						else if (key == "delayAltMusicFade")
						{
							this.DelayAltMusic = (bool)keyValuePair.Value;
						}
					}
					else if (key == "music")
					{
						this.Music = (string)keyValuePair.Value;
					}
				}
				else if (num <= 3859557458U)
				{
					if (num != 3585981250U)
					{
						if (num != 3732367685U)
						{
							if (num == 3859557458U)
							{
								if (key == "c")
								{
									this.EditorColorIndex = (int)keyValuePair.Value;
								}
							}
						}
						else if (key == "dark")
						{
							this.Dark = (bool)keyValuePair.Value;
						}
					}
					else if (key == "height")
					{
						this.Bounds.Height = (int)keyValuePair.Value;
						if (this.Bounds.Height == 184)
						{
							this.Bounds.Height = 180;
						}
					}
				}
				else if (num <= 4138884116U)
				{
					if (num != 4002569197U)
					{
						if (num == 4138884116U)
						{
							if (key == "enforceDashNumber")
							{
								this.EnforceDashNumber = (int)keyValuePair.Value;
							}
						}
					}
					else if (key == "whisper")
					{
						this.MusicWhispers = (bool)keyValuePair.Value;
					}
				}
				else if (num != 4228665076U)
				{
					if (num == 4245442695U)
					{
						if (key == "x")
						{
							this.Bounds.X = (int)keyValuePair.Value;
						}
					}
				}
				else if (key == "y")
				{
					this.Bounds.Y = (int)keyValuePair.Value;
				}
			}
			this.Spawns = new List<Vector2>();
			this.Entities = new List<EntityData>();
			this.Triggers = new List<EntityData>();
			this.BgDecals = new List<DecalData>();
			this.FgDecals = new List<DecalData>();
			foreach (BinaryPacker.Element element in data.Children)
			{
				if (element.Name == "entities")
				{
					if (element.Children == null)
					{
						continue;
					}
					using (List<BinaryPacker.Element>.Enumerator enumerator3 = element.Children.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							BinaryPacker.Element element2 = enumerator3.Current;
							if (element2.Name == "player")
							{
								this.Spawns.Add(new Vector2((float)this.Bounds.X + Convert.ToSingle(element2.Attributes["x"], CultureInfo.InvariantCulture), (float)this.Bounds.Y + Convert.ToSingle(element2.Attributes["y"], CultureInfo.InvariantCulture)));
							}
							else if (element2.Name == "strawberry" || element2.Name == "snowberry")
							{
								this.Strawberries++;
							}
							else if (element2.Name == "shard")
							{
								this.HasGem = true;
							}
							else if (element2.Name == "blackGem")
							{
								this.HasHeartGem = true;
							}
							else if (element2.Name == "checkpoint")
							{
								this.HasCheckpoint = true;
							}
							if (!element2.Name.Equals("player"))
							{
								this.Entities.Add(this.CreateEntityData(element2));
							}
						}
						continue;
					}
				}
				if (element.Name == "triggers")
				{
					if (element.Children == null)
					{
						continue;
					}
					using (List<BinaryPacker.Element>.Enumerator enumerator3 = element.Children.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							BinaryPacker.Element entity = enumerator3.Current;
							this.Triggers.Add(this.CreateEntityData(entity));
						}
						continue;
					}
				}
				if (element.Name == "bgdecals")
				{
					if (element.Children == null)
					{
						continue;
					}
					using (List<BinaryPacker.Element>.Enumerator enumerator3 = element.Children.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							BinaryPacker.Element element3 = enumerator3.Current;
							this.BgDecals.Add(new DecalData
							{
								Position = new Vector2(Convert.ToSingle(element3.Attributes["x"], CultureInfo.InvariantCulture), Convert.ToSingle(element3.Attributes["y"], CultureInfo.InvariantCulture)),
								Scale = new Vector2(Convert.ToSingle(element3.Attributes["scaleX"], CultureInfo.InvariantCulture), Convert.ToSingle(element3.Attributes["scaleY"], CultureInfo.InvariantCulture)),
								Texture = (string)element3.Attributes["texture"]
							});
						}
						continue;
					}
				}
				if (element.Name == "fgdecals")
				{
					if (element.Children == null)
					{
						continue;
					}
					using (List<BinaryPacker.Element>.Enumerator enumerator3 = element.Children.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							BinaryPacker.Element element4 = enumerator3.Current;
							this.FgDecals.Add(new DecalData
							{
								Position = new Vector2(Convert.ToSingle(element4.Attributes["x"], CultureInfo.InvariantCulture), Convert.ToSingle(element4.Attributes["y"], CultureInfo.InvariantCulture)),
								Scale = new Vector2(Convert.ToSingle(element4.Attributes["scaleX"], CultureInfo.InvariantCulture), Convert.ToSingle(element4.Attributes["scaleY"], CultureInfo.InvariantCulture)),
								Texture = (string)element4.Attributes["texture"]
							});
						}
						continue;
					}
				}
				if (element.Name == "solids")
				{
					this.Solids = element.Attr("innerText", "");
				}
				else if (element.Name == "bg")
				{
					this.Bg = element.Attr("innerText", "");
				}
				else if (element.Name == "fgtiles")
				{
					this.FgTiles = element.Attr("innerText", "");
				}
				else if (element.Name == "bgtiles")
				{
					this.BgTiles = element.Attr("innerText", "");
				}
				else if (element.Name == "objtiles")
				{
					this.ObjTiles = element.Attr("innerText", "");
				}
			}
			this.Dummy = (this.Spawns.Count <= 0);
		}

		// Token: 0x06001BA4 RID: 7076 RVA: 0x000B6BCC File Offset: 0x000B4DCC
		private EntityData CreateEntityData(BinaryPacker.Element entity)
		{
			EntityData entityData = new EntityData();
			entityData.Name = entity.Name;
			entityData.Level = this;
			if (entity.Attributes != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in entity.Attributes)
				{
					if (keyValuePair.Key == "id")
					{
						entityData.ID = (int)keyValuePair.Value;
					}
					else if (keyValuePair.Key == "x")
					{
						entityData.Position.X = Convert.ToSingle(keyValuePair.Value, CultureInfo.InvariantCulture);
					}
					else if (keyValuePair.Key == "y")
					{
						entityData.Position.Y = Convert.ToSingle(keyValuePair.Value, CultureInfo.InvariantCulture);
					}
					else if (keyValuePair.Key == "width")
					{
						entityData.Width = (int)keyValuePair.Value;
					}
					else if (keyValuePair.Key == "height")
					{
						entityData.Height = (int)keyValuePair.Value;
					}
					else if (keyValuePair.Key == "originX")
					{
						entityData.Origin.X = Convert.ToSingle(keyValuePair.Value, CultureInfo.InvariantCulture);
					}
					else if (keyValuePair.Key == "originY")
					{
						entityData.Origin.Y = Convert.ToSingle(keyValuePair.Value, CultureInfo.InvariantCulture);
					}
					else
					{
						if (entityData.Values == null)
						{
							entityData.Values = new Dictionary<string, object>();
						}
						entityData.Values.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
			entityData.Nodes = new Vector2[(entity.Children == null) ? 0 : entity.Children.Count];
			for (int i = 0; i < entityData.Nodes.Length; i++)
			{
				foreach (KeyValuePair<string, object> keyValuePair2 in entity.Children[i].Attributes)
				{
					if (keyValuePair2.Key == "x")
					{
						entityData.Nodes[i].X = Convert.ToSingle(keyValuePair2.Value, CultureInfo.InvariantCulture);
					}
					else if (keyValuePair2.Key == "y")
					{
						entityData.Nodes[i].Y = Convert.ToSingle(keyValuePair2.Value, CultureInfo.InvariantCulture);
					}
				}
			}
			return entityData;
		}

		// Token: 0x06001BA5 RID: 7077 RVA: 0x000B6EBC File Offset: 0x000B50BC
		public bool Check(Vector2 at)
		{
			return at.X >= (float)this.Bounds.Left && at.Y >= (float)this.Bounds.Top && at.X < (float)this.Bounds.Right && at.Y < (float)this.Bounds.Bottom;
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06001BA6 RID: 7078 RVA: 0x000B6F1C File Offset: 0x000B511C
		public Rectangle TileBounds
		{
			get
			{
				return new Rectangle(this.Bounds.X / 8, this.Bounds.Y / 8, (int)Math.Ceiling((double)((float)this.Bounds.Width / 8f)), (int)Math.Ceiling((double)((float)this.Bounds.Height / 8f)));
			}
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06001BA7 RID: 7079 RVA: 0x000B6F7A File Offset: 0x000B517A
		// (set) Token: 0x06001BA8 RID: 7080 RVA: 0x000B6F9C File Offset: 0x000B519C
		public Vector2 Position
		{
			get
			{
				return new Vector2((float)this.Bounds.X, (float)this.Bounds.Y);
			}
			set
			{
				for (int i = 0; i < this.Spawns.Count; i++)
				{
					List<Vector2> spawns = this.Spawns;
					int index = i;
					spawns[index] -= this.Position;
				}
				this.Bounds.X = (int)value.X;
				this.Bounds.Y = (int)value.Y;
				for (int j = 0; j < this.Spawns.Count; j++)
				{
					List<Vector2> spawns = this.Spawns;
					int index = j;
					spawns[index] += this.Position;
				}
			}
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x06001BA9 RID: 7081 RVA: 0x000B7040 File Offset: 0x000B5240
		public int LoadSeed
		{
			get
			{
				int num = 0;
				foreach (char c in this.Name)
				{
					num += (int)c;
				}
				return num;
			}
		}

		// Token: 0x040018AE RID: 6318
		public string Name;

		// Token: 0x040018AF RID: 6319
		public bool Dummy;

		// Token: 0x040018B0 RID: 6320
		public int Strawberries;

		// Token: 0x040018B1 RID: 6321
		public bool HasGem;

		// Token: 0x040018B2 RID: 6322
		public bool HasHeartGem;

		// Token: 0x040018B3 RID: 6323
		public bool HasCheckpoint;

		// Token: 0x040018B4 RID: 6324
		public bool DisableDownTransition;

		// Token: 0x040018B5 RID: 6325
		public Rectangle Bounds;

		// Token: 0x040018B6 RID: 6326
		public List<EntityData> Entities;

		// Token: 0x040018B7 RID: 6327
		public List<EntityData> Triggers;

		// Token: 0x040018B8 RID: 6328
		public List<Vector2> Spawns;

		// Token: 0x040018B9 RID: 6329
		public List<DecalData> FgDecals;

		// Token: 0x040018BA RID: 6330
		public List<DecalData> BgDecals;

		// Token: 0x040018BB RID: 6331
		public string Solids = "";

		// Token: 0x040018BC RID: 6332
		public string Bg = "";

		// Token: 0x040018BD RID: 6333
		public string FgTiles = "";

		// Token: 0x040018BE RID: 6334
		public string BgTiles = "";

		// Token: 0x040018BF RID: 6335
		public string ObjTiles = "";

		// Token: 0x040018C0 RID: 6336
		public WindController.Patterns WindPattern;

		// Token: 0x040018C1 RID: 6337
		public Vector2 CameraOffset;

		// Token: 0x040018C2 RID: 6338
		public bool Dark;

		// Token: 0x040018C3 RID: 6339
		public bool Underwater;

		// Token: 0x040018C4 RID: 6340
		public bool Space;

		// Token: 0x040018C5 RID: 6341
		public string Music = "";

		// Token: 0x040018C6 RID: 6342
		public string AltMusic = "";

		// Token: 0x040018C7 RID: 6343
		public string Ambience = "";

		// Token: 0x040018C8 RID: 6344
		public float[] MusicLayers = new float[4];

		// Token: 0x040018C9 RID: 6345
		public int MusicProgress = -1;

		// Token: 0x040018CA RID: 6346
		public int AmbienceProgress = -1;

		// Token: 0x040018CB RID: 6347
		public bool MusicWhispers;

		// Token: 0x040018CC RID: 6348
		public bool DelayAltMusic;

		// Token: 0x040018CD RID: 6349
		public int EnforceDashNumber;

		// Token: 0x040018CE RID: 6350
		public int EditorColorIndex;
	}
}
