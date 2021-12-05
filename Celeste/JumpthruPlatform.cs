using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000356 RID: 854
	public class JumpthruPlatform : JumpThru
	{
		// Token: 0x06001AE1 RID: 6881 RVA: 0x000AE468 File Offset: 0x000AC668
		public JumpthruPlatform(Vector2 position, int width, string overrideTexture, int overrideSoundIndex = -1) : base(position, width, true)
		{
			this.columns = width / 8;
			base.Depth = -60;
			this.overrideTexture = overrideTexture;
			this.overrideSoundIndex = overrideSoundIndex;
		}

		// Token: 0x06001AE2 RID: 6882 RVA: 0x000AE49A File Offset: 0x000AC69A
		public JumpthruPlatform(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Attr("texture", "default"), data.Int("surfaceIndex", -1))
		{
		}

		// Token: 0x06001AE3 RID: 6883 RVA: 0x000AE4D0 File Offset: 0x000AC6D0
		public override void Awake(Scene scene)
		{
			string jumpthru = AreaData.Get(scene).Jumpthru;
			if (!string.IsNullOrEmpty(this.overrideTexture) && !this.overrideTexture.Equals("default"))
			{
				jumpthru = this.overrideTexture;
			}
			if (this.overrideSoundIndex > 0)
			{
				this.SurfaceSoundIndex = this.overrideSoundIndex;
			}
			else
			{
				string a = jumpthru.ToLower();
				if (!(a == "dream"))
				{
					if (!(a == "temple") && !(a == "templeb"))
					{
						if (!(a == "core"))
						{
							if (!(a == "wood") && !(a == "cliffside"))
							{
							}
							this.SurfaceSoundIndex = 5;
						}
						else
						{
							this.SurfaceSoundIndex = 3;
						}
					}
					else
					{
						this.SurfaceSoundIndex = 8;
					}
				}
				else
				{
					this.SurfaceSoundIndex = 32;
				}
			}
			MTexture mtexture = GFX.Game["objects/jumpthru/" + jumpthru];
			int num = mtexture.Width / 8;
			for (int i = 0; i < this.columns; i++)
			{
				int num2;
				int num3;
				if (i == 0)
				{
					num2 = 0;
					num3 = (base.CollideCheck<Solid, SwapBlock, ExitBlock>(this.Position + new Vector2(-1f, 0f)) ? 0 : 1);
				}
				else if (i == this.columns - 1)
				{
					num2 = num - 1;
					num3 = (base.CollideCheck<Solid, SwapBlock, ExitBlock>(this.Position + new Vector2(1f, 0f)) ? 0 : 1);
				}
				else
				{
					num2 = 1 + Calc.Random.Next(num - 2);
					num3 = Calc.Random.Choose(0, 1);
				}
				base.Add(new Image(mtexture.GetSubtexture(num2 * 8, num3 * 8, 8, 8, null))
				{
					X = (float)(i * 8)
				});
			}
		}

		// Token: 0x04001782 RID: 6018
		private int columns;

		// Token: 0x04001783 RID: 6019
		private string overrideTexture;

		// Token: 0x04001784 RID: 6020
		private int overrideSoundIndex = -1;
	}
}
