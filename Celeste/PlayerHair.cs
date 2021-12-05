using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000362 RID: 866
	[Tracked(false)]
	public class PlayerHair : Component
	{
		// Token: 0x06001B46 RID: 6982 RVA: 0x000B20A0 File Offset: 0x000B02A0
		public PlayerHair(PlayerSprite sprite) : base(true, true)
		{
			this.Sprite = sprite;
			for (int i = 0; i < sprite.HairCount; i++)
			{
				this.Nodes.Add(Vector2.Zero);
			}
		}

		// Token: 0x06001B47 RID: 6983 RVA: 0x000B2150 File Offset: 0x000B0350
		public void Start()
		{
			Vector2 value = base.Entity.Position + new Vector2((float)(-this.Facing * (Facings)200), 200f);
			for (int i = 0; i < this.Nodes.Count; i++)
			{
				this.Nodes[i] = value;
			}
		}

		// Token: 0x06001B48 RID: 6984 RVA: 0x000B21AC File Offset: 0x000B03AC
		public void AfterUpdate()
		{
			Vector2 value = this.Sprite.HairOffset * new Vector2((float)this.Facing, 1f);
			this.Nodes[0] = this.Sprite.RenderPosition + new Vector2(0f, -9f * this.Sprite.Scale.Y) + value;
			Vector2 target = this.Nodes[0] + new Vector2((float)(-(float)this.Facing) * this.StepInFacingPerSegment * 2f, (float)Math.Sin((double)this.wave) * this.StepYSinePerSegment) + this.StepPerSegment;
			Vector2 vector = this.Nodes[0];
			float num = 3f;
			for (int i = 1; i < this.Sprite.HairCount; i++)
			{
				if (i >= this.Nodes.Count)
				{
					this.Nodes.Add(this.Nodes[i - 1]);
				}
				if (this.SimulateMotion)
				{
					float num2 = (1f - (float)i / (float)this.Sprite.HairCount * 0.5f) * this.StepApproach;
					this.Nodes[i] = Calc.Approach(this.Nodes[i], target, num2 * Engine.DeltaTime);
				}
				if ((this.Nodes[i] - vector).Length() > num)
				{
					this.Nodes[i] = vector + (this.Nodes[i] - vector).SafeNormalize() * num;
				}
				target = this.Nodes[i] + new Vector2((float)(-(float)this.Facing) * this.StepInFacingPerSegment, (float)Math.Sin((double)(this.wave + (float)i * 0.8f)) * this.StepYSinePerSegment) + this.StepPerSegment;
				vector = this.Nodes[i];
			}
		}

		// Token: 0x06001B49 RID: 6985 RVA: 0x000B23C7 File Offset: 0x000B05C7
		public override void Update()
		{
			this.wave += Engine.DeltaTime * 4f;
			base.Update();
		}

		// Token: 0x06001B4A RID: 6986 RVA: 0x000B23E8 File Offset: 0x000B05E8
		public void MoveHairBy(Vector2 amount)
		{
			for (int i = 0; i < this.Nodes.Count; i++)
			{
				List<Vector2> nodes = this.Nodes;
				int index = i;
				nodes[index] += amount;
			}
		}

		// Token: 0x06001B4B RID: 6987 RVA: 0x000B2428 File Offset: 0x000B0628
		public override void Render()
		{
			if (!this.Sprite.HasHair)
			{
				return;
			}
			Vector2 origin = new Vector2(5f, 5f);
			Color color = this.Border * this.Alpha;
			Color color2 = this.Color * this.Alpha;
			if (this.DrawPlayerSpriteOutline)
			{
				Color color3 = this.Sprite.Color;
				Vector2 position = this.Sprite.Position;
				this.Sprite.Color = color;
				this.Sprite.Position = position + new Vector2(0f, -1f);
				this.Sprite.Render();
				this.Sprite.Position = position + new Vector2(0f, 1f);
				this.Sprite.Render();
				this.Sprite.Position = position + new Vector2(-1f, 0f);
				this.Sprite.Render();
				this.Sprite.Position = position + new Vector2(1f, 0f);
				this.Sprite.Render();
				this.Sprite.Color = color3;
				this.Sprite.Position = position;
			}
			this.Nodes[0] = this.Nodes[0].Floor();
			if (color.A > 0)
			{
				for (int i = 0; i < this.Sprite.HairCount; i++)
				{
					int hairFrame = this.Sprite.HairFrame;
					object obj = (i == 0) ? this.bangs[hairFrame] : GFX.Game["characters/player/hair00"];
					Vector2 hairScale = this.GetHairScale(i);
					object obj2 = obj;
					obj2.Draw(this.Nodes[i] + new Vector2(-1f, 0f), origin, color, hairScale);
					obj2.Draw(this.Nodes[i] + new Vector2(1f, 0f), origin, color, hairScale);
					obj2.Draw(this.Nodes[i] + new Vector2(0f, -1f), origin, color, hairScale);
					obj2.Draw(this.Nodes[i] + new Vector2(0f, 1f), origin, color, hairScale);
				}
			}
			for (int j = this.Sprite.HairCount - 1; j >= 0; j--)
			{
				int hairFrame2 = this.Sprite.HairFrame;
				((j == 0) ? this.bangs[hairFrame2] : GFX.Game["characters/player/hair00"]).Draw(this.Nodes[j], origin, color2, this.GetHairScale(j));
			}
		}

		// Token: 0x06001B4C RID: 6988 RVA: 0x000B2708 File Offset: 0x000B0908
		private Vector2 GetHairScale(int index)
		{
			float num = 0.25f + (1f - (float)index / (float)this.Sprite.HairCount) * 0.75f;
			return new Vector2(((index == 0) ? ((float)this.Facing) : num) * Math.Abs(this.Sprite.Scale.X), num);
		}

		// Token: 0x0400181D RID: 6173
		public const string Hair = "characters/player/hair00";

		// Token: 0x0400181E RID: 6174
		public Color Color = Player.NormalHairColor;

		// Token: 0x0400181F RID: 6175
		public Color Border = Color.Black;

		// Token: 0x04001820 RID: 6176
		public float Alpha = 1f;

		// Token: 0x04001821 RID: 6177
		public Facings Facing;

		// Token: 0x04001822 RID: 6178
		public bool DrawPlayerSpriteOutline;

		// Token: 0x04001823 RID: 6179
		public bool SimulateMotion = true;

		// Token: 0x04001824 RID: 6180
		public Vector2 StepPerSegment = new Vector2(0f, 2f);

		// Token: 0x04001825 RID: 6181
		public float StepInFacingPerSegment = 0.5f;

		// Token: 0x04001826 RID: 6182
		public float StepApproach = 64f;

		// Token: 0x04001827 RID: 6183
		public float StepYSinePerSegment;

		// Token: 0x04001828 RID: 6184
		public PlayerSprite Sprite;

		// Token: 0x04001829 RID: 6185
		public List<Vector2> Nodes = new List<Vector2>();

		// Token: 0x0400182A RID: 6186
		private List<MTexture> bangs = GFX.Game.GetAtlasSubtextures("characters/player/bangs");

		// Token: 0x0400182B RID: 6187
		private float wave;
	}
}
