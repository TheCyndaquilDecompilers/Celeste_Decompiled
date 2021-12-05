using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000353 RID: 851
	[Tracked(false)]
	public class Spikes : Entity
	{
		// Token: 0x06001AA8 RID: 6824 RVA: 0x000AC7EC File Offset: 0x000AA9EC
		public Spikes(Vector2 position, int size, Spikes.Directions direction, string type) : base(position)
		{
			base.Depth = -1;
			this.Direction = direction;
			this.size = size;
			this.overrideType = type;
			switch (direction)
			{
			case Spikes.Directions.Up:
				base.Collider = new Hitbox((float)size, 3f, 0f, -3f);
				base.Add(new LedgeBlocker(null));
				break;
			case Spikes.Directions.Down:
				base.Collider = new Hitbox((float)size, 3f, 0f, 0f);
				break;
			case Spikes.Directions.Left:
				base.Collider = new Hitbox(3f, (float)size, -3f, 0f);
				base.Add(new LedgeBlocker(null));
				break;
			case Spikes.Directions.Right:
				base.Collider = new Hitbox(3f, (float)size, 0f, 0f);
				base.Add(new LedgeBlocker(null));
				break;
			}
			base.Add(this.pc = new PlayerCollider(new Action<Player>(this.OnCollide), null, null));
			base.Add(new StaticMover
			{
				OnShake = new Action<Vector2>(this.OnShake),
				SolidChecker = new Func<Solid, bool>(this.IsRiding),
				JumpThruChecker = new Func<JumpThru, bool>(this.IsRiding),
				OnEnable = new Action(this.OnEnable),
				OnDisable = new Action(this.OnDisable)
			});
		}

		// Token: 0x06001AA9 RID: 6825 RVA: 0x000AC971 File Offset: 0x000AAB71
		public Spikes(EntityData data, Vector2 offset, Spikes.Directions dir) : this(data.Position + offset, Spikes.GetSize(data, dir), dir, data.Attr("type", "default"))
		{
		}

		// Token: 0x06001AAA RID: 6826 RVA: 0x000AC9A0 File Offset: 0x000AABA0
		public void SetSpikeColor(Color color)
		{
			foreach (Component component in base.Components)
			{
				Image image = component as Image;
				if (image != null)
				{
					image.Color = color;
				}
			}
		}

		// Token: 0x06001AAB RID: 6827 RVA: 0x000AC9F8 File Offset: 0x000AABF8
		public override void Added(Scene scene)
		{
			base.Added(scene);
			AreaData areaData = AreaData.Get(scene);
			this.spikeType = areaData.Spike;
			if (!string.IsNullOrEmpty(this.overrideType) && !this.overrideType.Equals("default"))
			{
				this.spikeType = this.overrideType;
			}
			string str = this.Direction.ToString().ToLower();
			if (this.spikeType == "tentacles")
			{
				for (int i = 0; i < this.size / 16; i++)
				{
					this.AddTentacle((float)i);
				}
				if (this.size / 8 % 2 == 1)
				{
					this.AddTentacle((float)(this.size / 16) - 0.5f);
					return;
				}
			}
			else
			{
				List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("danger/spikes/" + this.spikeType + "_" + str);
				for (int j = 0; j < this.size / 8; j++)
				{
					Image image = new Image(Calc.Random.Choose(atlasSubtextures));
					switch (this.Direction)
					{
					case Spikes.Directions.Up:
						image.JustifyOrigin(0.5f, 1f);
						image.Position = Vector2.UnitX * ((float)j + 0.5f) * 8f + Vector2.UnitY;
						break;
					case Spikes.Directions.Down:
						image.JustifyOrigin(0.5f, 0f);
						image.Position = Vector2.UnitX * ((float)j + 0.5f) * 8f - Vector2.UnitY;
						break;
					case Spikes.Directions.Left:
						image.JustifyOrigin(1f, 0.5f);
						image.Position = Vector2.UnitY * ((float)j + 0.5f) * 8f + Vector2.UnitX;
						break;
					case Spikes.Directions.Right:
						image.JustifyOrigin(0f, 0.5f);
						image.Position = Vector2.UnitY * ((float)j + 0.5f) * 8f - Vector2.UnitX;
						break;
					}
					base.Add(image);
				}
			}
		}

		// Token: 0x06001AAC RID: 6828 RVA: 0x000ACC40 File Offset: 0x000AAE40
		private void AddTentacle(float i)
		{
			Sprite sprite = GFX.SpriteBank.Create("tentacles");
			sprite.Play(Calc.Random.Next(3).ToString(), true, true);
			sprite.Position = ((this.Direction == Spikes.Directions.Up || this.Direction == Spikes.Directions.Down) ? Vector2.UnitX : Vector2.UnitY) * (i + 0.5f) * 16f;
			sprite.Scale.X = (float)Calc.Random.Choose(-1, 1);
			sprite.SetAnimationFrame(Calc.Random.Next(sprite.CurrentAnimationTotalFrames));
			if (this.Direction == Spikes.Directions.Up)
			{
				sprite.Rotation = -1.5707964f;
				Sprite sprite2 = sprite;
				float num = sprite2.Y;
				sprite2.Y = num + 1f;
			}
			else if (this.Direction == Spikes.Directions.Right)
			{
				sprite.Rotation = 0f;
				Sprite sprite3 = sprite;
				float num = sprite3.X;
				sprite3.X = num - 1f;
			}
			else if (this.Direction == Spikes.Directions.Left)
			{
				sprite.Rotation = 3.1415927f;
				Sprite sprite4 = sprite;
				float num = sprite4.X;
				sprite4.X = num + 1f;
			}
			else if (this.Direction == Spikes.Directions.Down)
			{
				sprite.Rotation = 1.5707964f;
				Sprite sprite5 = sprite;
				float num = sprite5.Y;
				sprite5.Y = num - 1f;
			}
			sprite.Rotation += 1.5707964f;
			base.Add(sprite);
		}

		// Token: 0x06001AAD RID: 6829 RVA: 0x000ACDA0 File Offset: 0x000AAFA0
		private void OnEnable()
		{
			this.Active = (this.Visible = (this.Collidable = true));
			this.SetSpikeColor(this.EnabledColor);
		}

		// Token: 0x06001AAE RID: 6830 RVA: 0x000ACDD4 File Offset: 0x000AAFD4
		private void OnDisable()
		{
			this.Active = (this.Collidable = false);
			if (this.VisibleWhenDisabled)
			{
				using (IEnumerator<Component> enumerator = base.Components.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Component component = enumerator.Current;
						Image image = component as Image;
						if (image != null)
						{
							image.Color = this.DisabledColor;
						}
					}
					return;
				}
			}
			this.Visible = false;
		}

		// Token: 0x06001AAF RID: 6831 RVA: 0x000ACE50 File Offset: 0x000AB050
		private void OnShake(Vector2 amount)
		{
			this.imageOffset += amount;
		}

		// Token: 0x06001AB0 RID: 6832 RVA: 0x000ACE64 File Offset: 0x000AB064
		public override void Render()
		{
			Vector2 position = this.Position;
			this.Position += this.imageOffset;
			base.Render();
			this.Position = position;
		}

		// Token: 0x06001AB1 RID: 6833 RVA: 0x000ACE9C File Offset: 0x000AB09C
		public void SetOrigins(Vector2 origin)
		{
			foreach (Component component in base.Components)
			{
				Image image = component as Image;
				if (image != null)
				{
					Vector2 vector = origin - this.Position;
					image.Origin = image.Origin + vector - image.Position;
					image.Position = vector;
				}
			}
		}

		// Token: 0x06001AB2 RID: 6834 RVA: 0x000ACF1C File Offset: 0x000AB11C
		private void OnCollide(Player player)
		{
			switch (this.Direction)
			{
			case Spikes.Directions.Up:
				if (player.Speed.Y >= 0f && player.Bottom <= base.Bottom)
				{
					player.Die(new Vector2(0f, -1f), false, true);
					return;
				}
				break;
			case Spikes.Directions.Down:
				if (player.Speed.Y <= 0f)
				{
					player.Die(new Vector2(0f, 1f), false, true);
					return;
				}
				break;
			case Spikes.Directions.Left:
				if (player.Speed.X >= 0f)
				{
					player.Die(new Vector2(-1f, 0f), false, true);
					return;
				}
				break;
			case Spikes.Directions.Right:
				if (player.Speed.X <= 0f)
				{
					player.Die(new Vector2(1f, 0f), false, true);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06001AB3 RID: 6835 RVA: 0x000A41C1 File Offset: 0x000A23C1
		private static int GetSize(EntityData data, Spikes.Directions dir)
		{
			if (dir > Spikes.Directions.Down)
			{
				int num = dir - Spikes.Directions.Left;
				return data.Height;
			}
			return data.Width;
		}

		// Token: 0x06001AB4 RID: 6836 RVA: 0x000AD008 File Offset: 0x000AB208
		private bool IsRiding(Solid solid)
		{
			switch (this.Direction)
			{
			case Spikes.Directions.Up:
				return base.CollideCheckOutside(solid, this.Position + Vector2.UnitY);
			case Spikes.Directions.Down:
				return base.CollideCheckOutside(solid, this.Position - Vector2.UnitY);
			case Spikes.Directions.Left:
				return base.CollideCheckOutside(solid, this.Position + Vector2.UnitX);
			case Spikes.Directions.Right:
				return base.CollideCheckOutside(solid, this.Position - Vector2.UnitX);
			default:
				return false;
			}
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x000AD094 File Offset: 0x000AB294
		private bool IsRiding(JumpThru jumpThru)
		{
			Spikes.Directions direction = this.Direction;
			return direction == Spikes.Directions.Up && base.CollideCheck(jumpThru, this.Position + Vector2.UnitY);
		}

		// Token: 0x04001757 RID: 5975
		public const string TentacleType = "tentacles";

		// Token: 0x04001758 RID: 5976
		public Spikes.Directions Direction;

		// Token: 0x04001759 RID: 5977
		private PlayerCollider pc;

		// Token: 0x0400175A RID: 5978
		private Vector2 imageOffset;

		// Token: 0x0400175B RID: 5979
		private int size;

		// Token: 0x0400175C RID: 5980
		private string overrideType;

		// Token: 0x0400175D RID: 5981
		private string spikeType;

		// Token: 0x0400175E RID: 5982
		public Color EnabledColor = Color.White;

		// Token: 0x0400175F RID: 5983
		public Color DisabledColor = Color.White;

		// Token: 0x04001760 RID: 5984
		public bool VisibleWhenDisabled;

		// Token: 0x0200070C RID: 1804
		public enum Directions
		{
			// Token: 0x04002D7C RID: 11644
			Up,
			// Token: 0x04002D7D RID: 11645
			Down,
			// Token: 0x04002D7E RID: 11646
			Left,
			// Token: 0x04002D7F RID: 11647
			Right
		}
	}
}
