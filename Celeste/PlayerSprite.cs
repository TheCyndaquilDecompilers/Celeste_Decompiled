using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000364 RID: 868
	public class PlayerSprite : Sprite
	{
		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06001B4D RID: 6989 RVA: 0x000B2760 File Offset: 0x000B0960
		// (set) Token: 0x06001B4E RID: 6990 RVA: 0x000B2768 File Offset: 0x000B0968
		public PlayerSpriteMode Mode { get; private set; }

		// Token: 0x06001B4F RID: 6991 RVA: 0x000B2774 File Offset: 0x000B0974
		public PlayerSprite(PlayerSpriteMode mode) : base(null, null)
		{
			this.Mode = mode;
			string id = "";
			if (mode == PlayerSpriteMode.Madeline)
			{
				id = "player";
			}
			else if (mode == PlayerSpriteMode.MadelineNoBackpack)
			{
				id = "player_no_backpack";
			}
			else if (mode == PlayerSpriteMode.Badeline)
			{
				id = "badeline";
			}
			else if (mode == PlayerSpriteMode.MadelineAsBadeline)
			{
				id = "player_badeline";
			}
			else if (mode == PlayerSpriteMode.Playback)
			{
				id = "player_playback";
			}
			this.spriteName = id;
			GFX.SpriteBank.CreateOn(this, id);
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06001B50 RID: 6992 RVA: 0x000B27EC File Offset: 0x000B09EC
		public Vector2 HairOffset
		{
			get
			{
				PlayerAnimMetadata playerAnimMetadata;
				if (this.Texture != null && PlayerSprite.FrameMetadata.TryGetValue(this.Texture.AtlasPath, out playerAnimMetadata))
				{
					return playerAnimMetadata.HairOffset;
				}
				return Vector2.Zero;
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06001B51 RID: 6993 RVA: 0x000B2828 File Offset: 0x000B0A28
		public float CarryYOffset
		{
			get
			{
				PlayerAnimMetadata playerAnimMetadata;
				if (this.Texture != null && PlayerSprite.FrameMetadata.TryGetValue(this.Texture.AtlasPath, out playerAnimMetadata))
				{
					return (float)playerAnimMetadata.CarryYOffset * this.Scale.Y;
				}
				return 0f;
			}
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06001B52 RID: 6994 RVA: 0x000B2870 File Offset: 0x000B0A70
		public int HairFrame
		{
			get
			{
				PlayerAnimMetadata playerAnimMetadata;
				if (this.Texture != null && PlayerSprite.FrameMetadata.TryGetValue(this.Texture.AtlasPath, out playerAnimMetadata))
				{
					return playerAnimMetadata.Frame;
				}
				return 0;
			}
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06001B53 RID: 6995 RVA: 0x000B28A8 File Offset: 0x000B0AA8
		public bool HasHair
		{
			get
			{
				PlayerAnimMetadata playerAnimMetadata;
				return this.Texture != null && PlayerSprite.FrameMetadata.TryGetValue(this.Texture.AtlasPath, out playerAnimMetadata) && playerAnimMetadata.HasHair;
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06001B54 RID: 6996 RVA: 0x000B28DE File Offset: 0x000B0ADE
		public bool Running
		{
			get
			{
				return base.LastAnimationID != null && (base.LastAnimationID == "flip" || base.LastAnimationID.StartsWith("run"));
			}
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06001B55 RID: 6997 RVA: 0x000B290E File Offset: 0x000B0B0E
		public bool DreamDashing
		{
			get
			{
				return base.LastAnimationID != null && base.LastAnimationID.StartsWith("dreamDash");
			}
		}

		// Token: 0x06001B56 RID: 6998 RVA: 0x000B292C File Offset: 0x000B0B2C
		public override void Render()
		{
			Vector2 renderPosition = base.RenderPosition;
			base.RenderPosition = base.RenderPosition.Floor();
			base.Render();
			base.RenderPosition = renderPosition;
		}

		// Token: 0x06001B57 RID: 6999 RVA: 0x000B2960 File Offset: 0x000B0B60
		public static void CreateFramesMetadata(string sprite)
		{
			foreach (SpriteDataSource spriteDataSource in GFX.SpriteBank.SpriteData[sprite].Sources)
			{
				XmlElement xmlElement = spriteDataSource.XML["Metadata"];
				string str = spriteDataSource.Path;
				if (xmlElement != null)
				{
					if (!string.IsNullOrEmpty(spriteDataSource.OverridePath))
					{
						str = spriteDataSource.OverridePath;
					}
					foreach (object obj in xmlElement.GetElementsByTagName("Frames"))
					{
						XmlElement xml = (XmlElement)obj;
						string text = str + xml.Attr("path", "");
						string[] array = xml.Attr("hair").Split(new char[]
						{
							'|'
						});
						string[] array2 = xml.Attr("carry", "").Split(new char[]
						{
							','
						});
						for (int i = 0; i < Math.Max(array.Length, array2.Length); i++)
						{
							PlayerAnimMetadata playerAnimMetadata = new PlayerAnimMetadata();
							string text2 = text + ((i < 10) ? "0" : "") + i;
							if (i == 0 && !GFX.Game.Has(text2))
							{
								text2 = text;
							}
							PlayerSprite.FrameMetadata[text2] = playerAnimMetadata;
							if (i < array.Length)
							{
								if (array[i].Equals("x", StringComparison.OrdinalIgnoreCase) || array[i].Length <= 0)
								{
									playerAnimMetadata.HasHair = false;
								}
								else
								{
									string[] array3 = array[i].Split(new char[]
									{
										':'
									});
									string[] array4 = array3[0].Split(new char[]
									{
										','
									});
									playerAnimMetadata.HasHair = true;
									playerAnimMetadata.HairOffset = new Vector2((float)Convert.ToInt32(array4[0]), (float)Convert.ToInt32(array4[1]));
									playerAnimMetadata.Frame = ((array3.Length >= 2) ? Convert.ToInt32(array3[1]) : 0);
								}
							}
							if (i < array2.Length && array2[i].Length > 0)
							{
								playerAnimMetadata.CarryYOffset = int.Parse(array2[i]);
							}
						}
					}
				}
			}
		}

		// Token: 0x06001B58 RID: 7000 RVA: 0x000B2BF4 File Offset: 0x000B0DF4
		public static void ClearFramesMetadata()
		{
			PlayerSprite.FrameMetadata.Clear();
		}

		// Token: 0x04001832 RID: 6194
		public const string Idle = "idle";

		// Token: 0x04001833 RID: 6195
		public const string Shaking = "shaking";

		// Token: 0x04001834 RID: 6196
		public const string FrontEdge = "edge";

		// Token: 0x04001835 RID: 6197
		public const string LookUp = "lookUp";

		// Token: 0x04001836 RID: 6198
		public const string Walk = "walk";

		// Token: 0x04001837 RID: 6199
		public const string RunSlow = "runSlow";

		// Token: 0x04001838 RID: 6200
		public const string RunFast = "runFast";

		// Token: 0x04001839 RID: 6201
		public const string RunWind = "runWind";

		// Token: 0x0400183A RID: 6202
		public const string RunStumble = "runStumble";

		// Token: 0x0400183B RID: 6203
		public const string JumpSlow = "jumpSlow";

		// Token: 0x0400183C RID: 6204
		public const string FallSlow = "fallSlow";

		// Token: 0x0400183D RID: 6205
		public const string Fall = "fall";

		// Token: 0x0400183E RID: 6206
		public const string JumpFast = "jumpFast";

		// Token: 0x0400183F RID: 6207
		public const string FallFast = "fallFast";

		// Token: 0x04001840 RID: 6208
		public const string FallBig = "bigFall";

		// Token: 0x04001841 RID: 6209
		public const string LandInPose = "fallPose";

		// Token: 0x04001842 RID: 6210
		public const string Tired = "tired";

		// Token: 0x04001843 RID: 6211
		public const string TiredStill = "tiredStill";

		// Token: 0x04001844 RID: 6212
		public const string WallSlide = "wallslide";

		// Token: 0x04001845 RID: 6213
		public const string ClimbUp = "climbUp";

		// Token: 0x04001846 RID: 6214
		public const string ClimbDown = "climbDown";

		// Token: 0x04001847 RID: 6215
		public const string ClimbLookBackStart = "climbLookBackStart";

		// Token: 0x04001848 RID: 6216
		public const string ClimbLookBack = "climbLookBack";

		// Token: 0x04001849 RID: 6217
		public const string Dangling = "dangling";

		// Token: 0x0400184A RID: 6218
		public const string Duck = "duck";

		// Token: 0x0400184B RID: 6219
		public const string Dash = "dash";

		// Token: 0x0400184C RID: 6220
		public const string Sleep = "sleep";

		// Token: 0x0400184D RID: 6221
		public const string Sleeping = "asleep";

		// Token: 0x0400184E RID: 6222
		public const string Flip = "flip";

		// Token: 0x0400184F RID: 6223
		public const string Skid = "skid";

		// Token: 0x04001850 RID: 6224
		public const string DreamDashIn = "dreamDashIn";

		// Token: 0x04001851 RID: 6225
		public const string DreamDashLoop = "dreamDashLoop";

		// Token: 0x04001852 RID: 6226
		public const string DreamDashOut = "dreamDashOut";

		// Token: 0x04001853 RID: 6227
		public const string SwimIdle = "swimIdle";

		// Token: 0x04001854 RID: 6228
		public const string SwimUp = "swimUp";

		// Token: 0x04001855 RID: 6229
		public const string SwimDown = "swimDown";

		// Token: 0x04001856 RID: 6230
		public const string StartStarFly = "startStarFly";

		// Token: 0x04001857 RID: 6231
		public const string StarFly = "starFly";

		// Token: 0x04001858 RID: 6232
		public const string StarMorph = "starMorph";

		// Token: 0x04001859 RID: 6233
		public const string IdleCarry = "idle_carry";

		// Token: 0x0400185A RID: 6234
		public const string RunCarry = "runSlow_carry";

		// Token: 0x0400185B RID: 6235
		public const string JumpCarry = "jumpSlow_carry";

		// Token: 0x0400185C RID: 6236
		public const string FallCarry = "fallSlow_carry";

		// Token: 0x0400185D RID: 6237
		public const string PickUp = "pickup";

		// Token: 0x0400185E RID: 6238
		public const string Throw = "throw";

		// Token: 0x0400185F RID: 6239
		public const string Launch = "launch";

		// Token: 0x04001860 RID: 6240
		public const string TentacleGrab = "tentacle_grab";

		// Token: 0x04001861 RID: 6241
		public const string TentacleGrabbed = "tentacle_grabbed";

		// Token: 0x04001862 RID: 6242
		public const string TentaclePull = "tentacle_pull";

		// Token: 0x04001863 RID: 6243
		public const string TentacleDangling = "tentacle_dangling";

		// Token: 0x04001864 RID: 6244
		public const string SitDown = "sitDown";

		// Token: 0x04001866 RID: 6246
		private string spriteName;

		// Token: 0x04001867 RID: 6247
		public int HairCount = 4;

		// Token: 0x04001868 RID: 6248
		private static Dictionary<string, PlayerAnimMetadata> FrameMetadata = new Dictionary<string, PlayerAnimMetadata>(StringComparer.OrdinalIgnoreCase);
	}
}
