using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001EC RID: 492
	public class GameplayStats : Entity
	{
		// Token: 0x06001041 RID: 4161 RVA: 0x00046818 File Offset: 0x00044A18
		public GameplayStats()
		{
			base.Depth = -101;
			base.Tag = (Tags.HUD | Tags.Global | Tags.PauseUpdate | Tags.TransitionUpdate);
		}

		// Token: 0x06001042 RID: 4162 RVA: 0x00046864 File Offset: 0x00044A64
		public override void Update()
		{
			base.Update();
			Level level = base.Scene as Level;
			this.DrawLerp = Calc.Approach(this.DrawLerp, (float)((level.Paused && level.PauseMainMenuOpen && level.Wipe == null) ? 1 : 0), Engine.DeltaTime * 8f);
		}

		// Token: 0x06001043 RID: 4163 RVA: 0x000468BC File Offset: 0x00044ABC
		public override void Render()
		{
			if (this.DrawLerp <= 0f)
			{
				return;
			}
			float num = Ease.CubeOut(this.DrawLerp);
			Level level = base.Scene as Level;
			AreaKey area = level.Session.Area;
			AreaModeStats areaModeStats = SaveData.Instance.Areas[area.ID].Modes[(int)area.Mode];
			if (areaModeStats.Completed || SaveData.Instance.CheatMode || SaveData.Instance.DebugMode)
			{
				ModeProperties modeProperties = AreaData.Get(area).Mode[(int)area.Mode];
				int totalStrawberries = modeProperties.TotalStrawberries;
				int num2 = 32;
				int num3 = (totalStrawberries - 1) * num2;
				int num4 = (totalStrawberries > 0 && modeProperties.Checkpoints != null) ? (modeProperties.Checkpoints.Length * num2) : 0;
				Vector2 vector = new Vector2((float)((1920 - num3 - num4) / 2), 1016f + (1f - num) * 80f);
				if (totalStrawberries > 0)
				{
					int num5 = (modeProperties.Checkpoints == null) ? 1 : (modeProperties.Checkpoints.Length + 1);
					for (int i = 0; i < num5; i++)
					{
						int num6 = (i == 0) ? modeProperties.StartStrawberries : modeProperties.Checkpoints[i - 1].Strawberries;
						for (int j = 0; j < num6; j++)
						{
							EntityData entityData = modeProperties.StrawberriesByCheckpoint[i, j];
							if (entityData != null)
							{
								bool flag = false;
								foreach (EntityID entityID in level.Session.Strawberries)
								{
									if (entityData.ID == entityID.ID && entityData.Level.Name == entityID.Level)
									{
										flag = true;
									}
								}
								MTexture mtexture = GFX.Gui["dot"];
								if (flag)
								{
									if (area.Mode == AreaMode.CSide)
									{
										mtexture.DrawOutlineCentered(vector, Calc.HexToColor("f2ff30"), 1.5f);
									}
									else
									{
										mtexture.DrawOutlineCentered(vector, Calc.HexToColor("ff3040"), 1.5f);
									}
								}
								else
								{
									bool flag2 = false;
									foreach (EntityID entityID2 in areaModeStats.Strawberries)
									{
										if (entityData.ID == entityID2.ID && entityData.Level.Name == entityID2.Level)
										{
											flag2 = true;
										}
									}
									if (flag2)
									{
										mtexture.DrawOutlineCentered(vector, Calc.HexToColor("4193ff"), 1f);
									}
									else
									{
										Draw.Rect(vector.X - (float)mtexture.ClipRect.Width * 0.5f, vector.Y - 4f, (float)mtexture.ClipRect.Width, 8f, Color.DarkGray);
									}
								}
								vector.X += (float)num2;
							}
						}
						if (modeProperties.Checkpoints != null && i < modeProperties.Checkpoints.Length)
						{
							Draw.Rect(vector.X - 3f, vector.Y - 16f, 6f, 32f, Color.DarkGray);
							vector.X += (float)num2;
						}
					}
				}
			}
		}

		// Token: 0x04000B9C RID: 2972
		public float DrawLerp;
	}
}
