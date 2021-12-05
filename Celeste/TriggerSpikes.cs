using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200032C RID: 812
	public class TriggerSpikes : Entity
	{
		// Token: 0x06001973 RID: 6515 RVA: 0x000A3A5C File Offset: 0x000A1C5C
		public TriggerSpikes(Vector2 position, int size, TriggerSpikes.Directions direction) : base(position)
		{
			this.size = size;
			this.direction = direction;
			switch (direction)
			{
			case TriggerSpikes.Directions.Up:
				this.tentacleTextures = GFX.Game.GetAtlasSubtextures("danger/triggertentacle/wiggle_v");
				this.outwards = new Vector2(0f, -1f);
				this.offset = new Vector2(0f, -1f);
				base.Collider = new Hitbox((float)size, 4f, 0f, -4f);
				base.Add(new SafeGroundBlocker(null));
				base.Add(new LedgeBlocker(new Func<Player, bool>(this.UpSafeBlockCheck)));
				break;
			case TriggerSpikes.Directions.Down:
				this.tentacleTextures = GFX.Game.GetAtlasSubtextures("danger/triggertentacle/wiggle_v");
				this.outwards = new Vector2(0f, 1f);
				base.Collider = new Hitbox((float)size, 4f, 0f, 0f);
				break;
			case TriggerSpikes.Directions.Left:
				this.tentacleTextures = GFX.Game.GetAtlasSubtextures("danger/triggertentacle/wiggle_h");
				this.outwards = new Vector2(-1f, 0f);
				base.Collider = new Hitbox(4f, (float)size, -4f, 0f);
				base.Add(new SafeGroundBlocker(null));
				base.Add(new LedgeBlocker(new Func<Player, bool>(this.SideSafeBlockCheck)));
				break;
			case TriggerSpikes.Directions.Right:
				this.tentacleTextures = GFX.Game.GetAtlasSubtextures("danger/triggertentacle/wiggle_h");
				this.outwards = new Vector2(1f, 0f);
				this.offset = new Vector2(1f, 0f);
				base.Collider = new Hitbox(4f, (float)size, 0f, 0f);
				base.Add(new SafeGroundBlocker(null));
				base.Add(new LedgeBlocker(new Func<Player, bool>(this.SideSafeBlockCheck)));
				break;
			}
			base.Add(this.pc = new PlayerCollider(new Action<Player>(this.OnCollide), null, null));
			base.Add(new StaticMover
			{
				OnShake = new Action<Vector2>(this.OnShake),
				SolidChecker = new Func<Solid, bool>(this.IsRiding),
				JumpThruChecker = new Func<JumpThru, bool>(this.IsRiding)
			});
			base.Add(new DustEdge(new Action(this.RenderSpikes)));
			base.Depth = -50;
		}

		// Token: 0x06001974 RID: 6516 RVA: 0x000A3CD4 File Offset: 0x000A1ED4
		public TriggerSpikes(EntityData data, Vector2 offset, TriggerSpikes.Directions dir) : this(data.Position + offset, TriggerSpikes.GetSize(data, dir), dir)
		{
		}

		// Token: 0x06001975 RID: 6517 RVA: 0x000A3CF0 File Offset: 0x000A1EF0
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Vector3[] edgeColors = DustStyles.Get(scene).EdgeColors;
			this.dustTextures = GFX.Game.GetAtlasSubtextures("danger/dustcreature/base");
			this.tentacleColors = new Color[edgeColors.Length];
			for (int i = 0; i < this.tentacleColors.Length; i++)
			{
				this.tentacleColors[i] = Color.Lerp(new Color(edgeColors[i]), Color.DarkSlateBlue, 0.4f);
			}
			Vector2 value = new Vector2(Math.Abs(this.outwards.Y), Math.Abs(this.outwards.X));
			this.spikes = new TriggerSpikes.SpikeInfo[this.size / 4];
			for (int j = 0; j < this.spikes.Length; j++)
			{
				this.spikes[j].Parent = this;
				this.spikes[j].Index = j;
				this.spikes[j].WorldPosition = this.Position + value * (float)(2 + j * 4);
				this.spikes[j].ParticleTimerOffset = Calc.Random.NextFloat(0.25f);
				this.spikes[j].TextureIndex = Calc.Random.Next(this.dustTextures.Count);
				this.spikes[j].DustOutDistance = Calc.Random.Choose(3, 4, 6);
				this.spikes[j].TentacleColor = Calc.Random.Next(this.tentacleColors.Length);
				this.spikes[j].TentacleFrame = Calc.Random.NextFloat((float)this.tentacleTextures.Count);
			}
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x000A3EBA File Offset: 0x000A20BA
		private void OnShake(Vector2 amount)
		{
			this.shakeOffset += amount;
		}

		// Token: 0x06001977 RID: 6519 RVA: 0x000A3ED0 File Offset: 0x000A20D0
		private bool UpSafeBlockCheck(Player player)
		{
			int num = (int)((Facings)8 * player.Facing);
			int num2 = (int)((player.Left + (float)num - base.Left) / 4f);
			int num3 = (int)((player.Right + (float)num - base.Left) / 4f);
			if (num3 < 0 || num2 >= this.spikes.Length)
			{
				return false;
			}
			num2 = Math.Max(num2, 0);
			num3 = Math.Min(num3, this.spikes.Length - 1);
			for (int i = num2; i <= num3; i++)
			{
				if (this.spikes[i].Lerp >= 1f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001978 RID: 6520 RVA: 0x000A3F68 File Offset: 0x000A2168
		private bool SideSafeBlockCheck(Player player)
		{
			int num = (int)((player.Top - base.Top) / 4f);
			int num2 = (int)((player.Bottom - base.Top) / 4f);
			if (num2 < 0 || num >= this.spikes.Length)
			{
				return false;
			}
			num = Math.Max(num, 0);
			num2 = Math.Min(num2, this.spikes.Length - 1);
			for (int i = num; i <= num2; i++)
			{
				if (this.spikes[i].Lerp >= 1f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001979 RID: 6521 RVA: 0x000A3FF0 File Offset: 0x000A21F0
		private void OnCollide(Player player)
		{
			int num;
			int num2;
			this.GetPlayerCollideIndex(player, out num, out num2);
			if (num2 < 0 || num >= this.spikes.Length)
			{
				return;
			}
			num = Math.Max(num, 0);
			num2 = Math.Min(num2, this.spikes.Length - 1);
			int num3 = num;
			while (num3 <= num2 && !this.spikes[num3].OnPlayer(player, this.outwards))
			{
				num3++;
			}
		}

		// Token: 0x0600197A RID: 6522 RVA: 0x000A4058 File Offset: 0x000A2258
		private void GetPlayerCollideIndex(Player player, out int minIndex, out int maxIndex)
		{
			minIndex = (maxIndex = -1);
			switch (this.direction)
			{
			case TriggerSpikes.Directions.Up:
				if (player.Speed.Y >= 0f)
				{
					minIndex = (int)((player.Left - base.Left) / 4f);
					maxIndex = (int)((player.Right - base.Left) / 4f);
					return;
				}
				break;
			case TriggerSpikes.Directions.Down:
				if (player.Speed.Y <= 0f)
				{
					minIndex = (int)((player.Left - base.Left) / 4f);
					maxIndex = (int)((player.Right - base.Left) / 4f);
					return;
				}
				break;
			case TriggerSpikes.Directions.Left:
				if (player.Speed.X >= 0f)
				{
					minIndex = (int)((player.Top - base.Top) / 4f);
					maxIndex = (int)((player.Bottom - base.Top) / 4f);
					return;
				}
				break;
			case TriggerSpikes.Directions.Right:
				if (player.Speed.X <= 0f)
				{
					minIndex = (int)((player.Top - base.Top) / 4f);
					maxIndex = (int)((player.Bottom - base.Top) / 4f);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x0600197B RID: 6523 RVA: 0x000A418C File Offset: 0x000A238C
		private bool PlayerCheck(int spikeIndex)
		{
			Player player = base.CollideFirst<Player>();
			if (player != null)
			{
				int num;
				int num2;
				this.GetPlayerCollideIndex(player, out num, out num2);
				return num <= spikeIndex + 1 && num2 >= spikeIndex - 1;
			}
			return false;
		}

		// Token: 0x0600197C RID: 6524 RVA: 0x000A41C1 File Offset: 0x000A23C1
		private static int GetSize(EntityData data, TriggerSpikes.Directions dir)
		{
			if (dir > TriggerSpikes.Directions.Down)
			{
				int num = dir - TriggerSpikes.Directions.Left;
				return data.Height;
			}
			return data.Width;
		}

		// Token: 0x0600197D RID: 6525 RVA: 0x000A41DC File Offset: 0x000A23DC
		public override void Update()
		{
			base.Update();
			for (int i = 0; i < this.spikes.Length; i++)
			{
				this.spikes[i].Update();
			}
		}

		// Token: 0x0600197E RID: 6526 RVA: 0x000A4214 File Offset: 0x000A2414
		public override void Render()
		{
			base.Render();
			Vector2 vector = new Vector2(Math.Abs(this.outwards.Y), Math.Abs(this.outwards.X));
			int count = this.tentacleTextures.Count;
			Vector2 one = Vector2.One;
			Vector2 justify = new Vector2(0f, 0.5f);
			if (this.direction == TriggerSpikes.Directions.Left)
			{
				one.X = -1f;
			}
			else if (this.direction == TriggerSpikes.Directions.Up)
			{
				one.Y = -1f;
			}
			if (this.direction == TriggerSpikes.Directions.Up || this.direction == TriggerSpikes.Directions.Down)
			{
				justify = new Vector2(0.5f, 0f);
			}
			for (int i = 0; i < this.spikes.Length; i++)
			{
				if (!this.spikes[i].Triggered)
				{
					MTexture mtexture = this.tentacleTextures[(int)(this.spikes[i].TentacleFrame % (float)count)];
					Vector2 vector2 = this.Position + vector * (float)(2 + i * 4);
					mtexture.DrawJustified(vector2 + vector, justify, Color.Black, one, 0f);
					mtexture.DrawJustified(vector2, justify, this.tentacleColors[this.spikes[i].TentacleColor], one, 0f);
				}
			}
			this.RenderSpikes();
		}

		// Token: 0x0600197F RID: 6527 RVA: 0x000A4378 File Offset: 0x000A2578
		private void RenderSpikes()
		{
			Vector2 value = new Vector2(Math.Abs(this.outwards.Y), Math.Abs(this.outwards.X));
			for (int i = 0; i < this.spikes.Length; i++)
			{
				if (this.spikes[i].Triggered)
				{
					MTexture mtexture = this.dustTextures[this.spikes[i].TextureIndex];
					Vector2 position = this.Position + this.outwards * (-4f + this.spikes[i].Lerp * (float)this.spikes[i].DustOutDistance) + value * (float)(2 + i * 4);
					mtexture.DrawCentered(position, Color.White, 0.5f * this.spikes[i].Lerp, this.spikes[i].TextureRotation);
				}
			}
		}

		// Token: 0x06001980 RID: 6528 RVA: 0x000A447C File Offset: 0x000A267C
		private bool IsRiding(Solid solid)
		{
			switch (this.direction)
			{
			case TriggerSpikes.Directions.Up:
				return base.CollideCheckOutside(solid, this.Position + Vector2.UnitY);
			case TriggerSpikes.Directions.Down:
				return base.CollideCheckOutside(solid, this.Position - Vector2.UnitY);
			case TriggerSpikes.Directions.Left:
				return base.CollideCheckOutside(solid, this.Position + Vector2.UnitX);
			case TriggerSpikes.Directions.Right:
				return base.CollideCheckOutside(solid, this.Position - Vector2.UnitX);
			default:
				return false;
			}
		}

		// Token: 0x06001981 RID: 6529 RVA: 0x000A4508 File Offset: 0x000A2708
		private bool IsRiding(JumpThru jumpThru)
		{
			TriggerSpikes.Directions directions = this.direction;
			return directions == TriggerSpikes.Directions.Up && base.CollideCheck(jumpThru, this.Position + Vector2.UnitY);
		}

		// Token: 0x04001632 RID: 5682
		private const float RetractTime = 6f;

		// Token: 0x04001633 RID: 5683
		private const float DelayTime = 0.4f;

		// Token: 0x04001634 RID: 5684
		private TriggerSpikes.Directions direction;

		// Token: 0x04001635 RID: 5685
		private Vector2 outwards;

		// Token: 0x04001636 RID: 5686
		private Vector2 offset;

		// Token: 0x04001637 RID: 5687
		private PlayerCollider pc;

		// Token: 0x04001638 RID: 5688
		private Vector2 shakeOffset;

		// Token: 0x04001639 RID: 5689
		private TriggerSpikes.SpikeInfo[] spikes;

		// Token: 0x0400163A RID: 5690
		private List<MTexture> dustTextures;

		// Token: 0x0400163B RID: 5691
		private List<MTexture> tentacleTextures;

		// Token: 0x0400163C RID: 5692
		private Color[] tentacleColors;

		// Token: 0x0400163D RID: 5693
		private int size;

		// Token: 0x020006DE RID: 1758
		public enum Directions
		{
			// Token: 0x04002C8F RID: 11407
			Up,
			// Token: 0x04002C90 RID: 11408
			Down,
			// Token: 0x04002C91 RID: 11409
			Left,
			// Token: 0x04002C92 RID: 11410
			Right
		}

		// Token: 0x020006DF RID: 1759
		private struct SpikeInfo
		{
			// Token: 0x06002D3E RID: 11582 RVA: 0x0011E5F4 File Offset: 0x0011C7F4
			public void Update()
			{
				if (this.Triggered)
				{
					if (this.DelayTimer > 0f)
					{
						this.DelayTimer -= Engine.DeltaTime;
						if (this.DelayTimer <= 0f)
						{
							if (this.PlayerCheck())
							{
								this.DelayTimer = 0.05f;
							}
							else
							{
								Audio.Play("event:/game/03_resort/fluff_tendril_emerge", this.WorldPosition);
							}
						}
					}
					else
					{
						this.Lerp = Calc.Approach(this.Lerp, 1f, 8f * Engine.DeltaTime);
					}
					this.TextureRotation += Engine.DeltaTime * 1.2f;
					return;
				}
				this.Lerp = Calc.Approach(this.Lerp, 0f, 4f * Engine.DeltaTime);
				this.TentacleFrame += Engine.DeltaTime * 12f;
				if (this.Lerp <= 0f)
				{
					this.Triggered = false;
				}
			}

			// Token: 0x06002D3F RID: 11583 RVA: 0x0011E6E7 File Offset: 0x0011C8E7
			public bool PlayerCheck()
			{
				return this.Parent.PlayerCheck(this.Index);
			}

			// Token: 0x06002D40 RID: 11584 RVA: 0x0011E6FC File Offset: 0x0011C8FC
			public bool OnPlayer(Player player, Vector2 outwards)
			{
				if (!this.Triggered)
				{
					Audio.Play("event:/game/03_resort/fluff_tendril_touch", this.WorldPosition);
					this.Triggered = true;
					this.DelayTimer = 0.4f;
					this.RetractTimer = 6f;
				}
				else if (this.Lerp >= 1f)
				{
					player.Die(outwards, false, true);
					return true;
				}
				return false;
			}

			// Token: 0x04002C93 RID: 11411
			public TriggerSpikes Parent;

			// Token: 0x04002C94 RID: 11412
			public int Index;

			// Token: 0x04002C95 RID: 11413
			public Vector2 WorldPosition;

			// Token: 0x04002C96 RID: 11414
			public bool Triggered;

			// Token: 0x04002C97 RID: 11415
			public float RetractTimer;

			// Token: 0x04002C98 RID: 11416
			public float DelayTimer;

			// Token: 0x04002C99 RID: 11417
			public float Lerp;

			// Token: 0x04002C9A RID: 11418
			public float ParticleTimerOffset;

			// Token: 0x04002C9B RID: 11419
			public int TextureIndex;

			// Token: 0x04002C9C RID: 11420
			public float TextureRotation;

			// Token: 0x04002C9D RID: 11421
			public int DustOutDistance;

			// Token: 0x04002C9E RID: 11422
			public int TentacleColor;

			// Token: 0x04002C9F RID: 11423
			public float TentacleFrame;
		}
	}
}
