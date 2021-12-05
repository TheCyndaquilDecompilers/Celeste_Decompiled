using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001EE RID: 494
	public class ReturnMapHint : Entity
	{
		// Token: 0x0600104D RID: 4173 RVA: 0x000471C3 File Offset: 0x000453C3
		public ReturnMapHint()
		{
			base.Tag = Tags.HUD;
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x000471DC File Offset: 0x000453DC
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Session session = (base.Scene as Level).Session;
			AreaKey area = session.Area;
			HashSet<string> checkpoints = SaveData.Instance.GetCheckpoints(area);
			CheckpointData checkpointData = null;
			ModeProperties modeProperties = AreaData.Areas[area.ID].Mode[(int)area.Mode];
			if (modeProperties.Checkpoints != null)
			{
				foreach (CheckpointData checkpointData2 in modeProperties.Checkpoints)
				{
					if (session.LevelFlags.Contains(checkpointData2.Level) && checkpoints.Contains(checkpointData2.Level))
					{
						checkpointData = checkpointData2;
					}
				}
			}
			string text = area.ToString();
			if (checkpointData != null)
			{
				text = text + "_" + checkpointData.Level;
			}
			if (MTN.Checkpoints.Has(text))
			{
				this.checkpoint = MTN.Checkpoints[text];
			}
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x000472CC File Offset: 0x000454CC
		public static string GetCheckpointPreviewName(AreaKey area, string level)
		{
			if (level == null)
			{
				return area.ToString();
			}
			return area.ToString() + "_" + level;
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x000472F8 File Offset: 0x000454F8
		private MTexture GetCheckpointPreview(AreaKey area, string level)
		{
			string checkpointPreviewName = ReturnMapHint.GetCheckpointPreviewName(area, level);
			if (MTN.Checkpoints.Has(checkpointPreviewName))
			{
				return MTN.Checkpoints[checkpointPreviewName];
			}
			return null;
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x00047328 File Offset: 0x00045528
		public override void Render()
		{
			MTexture mtexture = GFX.Gui["checkpoint"];
			string text = Dialog.Clean("MENU_RETURN_INFO", null);
			MTexture mtexture2 = MTN.Checkpoints["polaroid"];
			float num = ActiveFont.Measure(text).X * 0.75f;
			if (this.checkpoint != null)
			{
				float num2 = (float)mtexture2.Width * 0.25f;
				Vector2 value = new Vector2((1920f - num - num2 - 64f) / 2f, 730f);
				float num3 = 720f / (float)this.checkpoint.ClipRect.Width;
				ActiveFont.DrawOutline(text, value + new Vector2(num / 2f, 0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.75f, Color.LightGray, 2f, Color.Black);
				value.X += num + 64f;
				mtexture2.DrawCentered(value + new Vector2(num2 / 2f, 0f), Color.White, 0.25f, 0.1f);
				this.checkpoint.DrawCentered(value + new Vector2(num2 / 2f, 0f), Color.White, 0.25f * num3, 0.1f);
				mtexture.DrawCentered(value + new Vector2(num2 * 0.8f, (float)mtexture2.Height * 0.25f * 0.5f * 0.8f), Color.White, 0.75f);
				return;
			}
			float num4 = (float)mtexture.Width * 0.75f;
			Vector2 value2 = new Vector2((1920f - num - num4 - 64f) / 2f, 730f);
			ActiveFont.DrawOutline(text, value2 + new Vector2(num / 2f, 0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.75f, Color.LightGray, 2f, Color.Black);
			value2.X += num + 64f;
			mtexture.DrawCentered(value2 + new Vector2(num4 * 0.5f, 0f), Color.White, 0.75f);
		}

		// Token: 0x04000BAC RID: 2988
		private MTexture checkpoint;
	}
}
