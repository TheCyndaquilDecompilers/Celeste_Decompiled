using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000307 RID: 775
	public static class PlaybackData
	{
		// Token: 0x0600182F RID: 6191 RVA: 0x00097644 File Offset: 0x00095844
		public static void Load()
		{
			foreach (string path in Directory.GetFiles(Path.Combine(Engine.ContentDirectory, "Tutorials")))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
				List<Player.ChaserState> value = PlaybackData.Import(File.ReadAllBytes(path));
				PlaybackData.Tutorials[fileNameWithoutExtension] = value;
			}
		}

		// Token: 0x06001830 RID: 6192 RVA: 0x00097698 File Offset: 0x00095898
		public static void Export(List<Player.ChaserState> list, string path)
		{
			float timeStamp = list[0].TimeStamp;
			Vector2 position = list[0].Position;
			using (BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(path)))
			{
				binaryWriter.Write("TIMELINE");
				binaryWriter.Write(2);
				binaryWriter.Write(list.Count);
				foreach (Player.ChaserState chaserState in list)
				{
					binaryWriter.Write(chaserState.Position.X - position.X);
					binaryWriter.Write(chaserState.Position.Y - position.Y);
					binaryWriter.Write(chaserState.TimeStamp - timeStamp);
					binaryWriter.Write(chaserState.Animation);
					binaryWriter.Write((int)chaserState.Facing);
					binaryWriter.Write(chaserState.OnGround);
					BinaryWriter binaryWriter2 = binaryWriter;
					Color hairColor = chaserState.HairColor;
					binaryWriter2.Write(hairColor.R);
					BinaryWriter binaryWriter3 = binaryWriter;
					hairColor = chaserState.HairColor;
					binaryWriter3.Write(hairColor.G);
					BinaryWriter binaryWriter4 = binaryWriter;
					hairColor = chaserState.HairColor;
					binaryWriter4.Write(hairColor.B);
					binaryWriter.Write(chaserState.Depth);
					binaryWriter.Write(chaserState.Scale.X);
					binaryWriter.Write(chaserState.Scale.Y);
					binaryWriter.Write(chaserState.DashDirection.X);
					binaryWriter.Write(chaserState.DashDirection.Y);
				}
			}
		}

		// Token: 0x06001831 RID: 6193 RVA: 0x00097858 File Offset: 0x00095A58
		public static List<Player.ChaserState> Import(byte[] buffer)
		{
			List<Player.ChaserState> list = new List<Player.ChaserState>();
			using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer)))
			{
				int num = 1;
				if (binaryReader.ReadString() == "TIMELINE")
				{
					num = binaryReader.ReadInt32();
				}
				else
				{
					binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
				}
				int num2 = binaryReader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					Player.ChaserState chaserState = default(Player.ChaserState);
					chaserState.Position.X = binaryReader.ReadSingle();
					chaserState.Position.Y = binaryReader.ReadSingle();
					chaserState.TimeStamp = binaryReader.ReadSingle();
					chaserState.Animation = binaryReader.ReadString();
					chaserState.Facing = (Facings)binaryReader.ReadInt32();
					chaserState.OnGround = binaryReader.ReadBoolean();
					chaserState.HairColor = new Color((int)binaryReader.ReadByte(), (int)binaryReader.ReadByte(), (int)binaryReader.ReadByte(), 255);
					chaserState.Depth = binaryReader.ReadInt32();
					chaserState.Sounds = 0;
					if (num == 1)
					{
						chaserState.Scale = new Vector2((float)chaserState.Facing, 1f);
						chaserState.DashDirection = Vector2.Zero;
					}
					else
					{
						chaserState.Scale.X = binaryReader.ReadSingle();
						chaserState.Scale.Y = binaryReader.ReadSingle();
						chaserState.DashDirection.X = binaryReader.ReadSingle();
						chaserState.DashDirection.Y = binaryReader.ReadSingle();
					}
					list.Add(chaserState);
				}
			}
			return list;
		}

		// Token: 0x0400150B RID: 5387
		public static Dictionary<string, List<Player.ChaserState>> Tutorials = new Dictionary<string, List<Player.ChaserState>>();
	}
}
