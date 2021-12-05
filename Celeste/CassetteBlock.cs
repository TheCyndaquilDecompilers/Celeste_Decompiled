using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000142 RID: 322
	[Tracked(false)]
	public class CassetteBlock : Solid
	{
		// Token: 0x06000BB5 RID: 2997 RVA: 0x00022C18 File Offset: 0x00020E18
		public CassetteBlock(Vector2 position, EntityID id, float width, float height, int index, float tempo) : base(position, width, height, false)
		{
			this.SurfaceSoundIndex = 35;
			this.Index = index;
			this.Tempo = tempo;
			this.Collidable = false;
			this.ID = id;
			switch (this.Index)
			{
			default:
				this.color = Calc.HexToColor("49aaf0");
				break;
			case 1:
				this.color = Calc.HexToColor("f049be");
				break;
			case 2:
				this.color = Calc.HexToColor("fcdc3a");
				break;
			case 3:
				this.color = Calc.HexToColor("38e04e");
				break;
			}
			base.Add(this.occluder = new LightOcclude(1f));
		}

		// Token: 0x06000BB6 RID: 2998 RVA: 0x00022CFC File Offset: 0x00020EFC
		public CassetteBlock(EntityData data, Vector2 offset, EntityID id) : this(data.Position + offset, id, (float)data.Width, (float)data.Height, data.Int("index", 0), data.Float("tempo", 1f))
		{
		}

		// Token: 0x06000BB7 RID: 2999 RVA: 0x00022D48 File Offset: 0x00020F48
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Color color = Calc.HexToColor("667da5");
			Color disabledColor = new Color((float)color.R / 255f * ((float)this.color.R / 255f), (float)color.G / 255f * ((float)this.color.G / 255f), (float)color.B / 255f * ((float)this.color.B / 255f), 1f);
			scene.Add(this.side = new CassetteBlock.BoxSide(this, disabledColor));
			foreach (StaticMover staticMover in this.staticMovers)
			{
				Spikes spikes = staticMover.Entity as Spikes;
				if (spikes != null)
				{
					spikes.EnabledColor = this.color;
					spikes.DisabledColor = disabledColor;
					spikes.VisibleWhenDisabled = true;
					spikes.SetSpikeColor(this.color);
				}
				Spring spring = staticMover.Entity as Spring;
				if (spring != null)
				{
					spring.DisabledColor = disabledColor;
					spring.VisibleWhenDisabled = true;
				}
			}
			if (this.group == null)
			{
				this.groupLeader = true;
				this.group = new List<CassetteBlock>();
				this.group.Add(this);
				this.FindInGroup(this);
				float num = float.MaxValue;
				float num2 = float.MinValue;
				float num3 = float.MaxValue;
				float num4 = float.MinValue;
				foreach (CassetteBlock cassetteBlock in this.group)
				{
					if (cassetteBlock.Left < num)
					{
						num = cassetteBlock.Left;
					}
					if (cassetteBlock.Right > num2)
					{
						num2 = cassetteBlock.Right;
					}
					if (cassetteBlock.Bottom > num4)
					{
						num4 = cassetteBlock.Bottom;
					}
					if (cassetteBlock.Top < num3)
					{
						num3 = cassetteBlock.Top;
					}
				}
				this.groupOrigin = new Vector2((float)((int)(num + (num2 - num) / 2f)), (float)((int)num4));
				this.wigglerScaler = new Vector2(Calc.ClampedMap(num2 - num, 32f, 96f, 1f, 0.2f), Calc.ClampedMap(num4 - num3, 32f, 96f, 1f, 0.2f));
				base.Add(this.wiggler = Wiggler.Create(0.3f, 3f, null, false, false));
				foreach (CassetteBlock cassetteBlock2 in this.group)
				{
					cassetteBlock2.wiggler = this.wiggler;
					cassetteBlock2.wigglerScaler = this.wigglerScaler;
					cassetteBlock2.groupOrigin = this.groupOrigin;
				}
			}
			foreach (StaticMover staticMover2 in this.staticMovers)
			{
				Spikes spikes2 = staticMover2.Entity as Spikes;
				if (spikes2 != null)
				{
					spikes2.SetOrigins(this.groupOrigin);
				}
			}
			for (float num5 = base.Left; num5 < base.Right; num5 += 8f)
			{
				for (float num6 = base.Top; num6 < base.Bottom; num6 += 8f)
				{
					bool flag = this.CheckForSame(num5 - 8f, num6);
					bool flag2 = this.CheckForSame(num5 + 8f, num6);
					bool flag3 = this.CheckForSame(num5, num6 - 8f);
					bool flag4 = this.CheckForSame(num5, num6 + 8f);
					if (flag && flag2 && flag3 && flag4)
					{
						if (!this.CheckForSame(num5 + 8f, num6 - 8f))
						{
							this.SetImage(num5, num6, 3, 0);
						}
						else if (!this.CheckForSame(num5 - 8f, num6 - 8f))
						{
							this.SetImage(num5, num6, 3, 1);
						}
						else if (!this.CheckForSame(num5 + 8f, num6 + 8f))
						{
							this.SetImage(num5, num6, 3, 2);
						}
						else if (!this.CheckForSame(num5 - 8f, num6 + 8f))
						{
							this.SetImage(num5, num6, 3, 3);
						}
						else
						{
							this.SetImage(num5, num6, 1, 1);
						}
					}
					else if (flag && flag2 && !flag3 && flag4)
					{
						this.SetImage(num5, num6, 1, 0);
					}
					else if (flag && flag2 && flag3 && !flag4)
					{
						this.SetImage(num5, num6, 1, 2);
					}
					else if (flag && !flag2 && flag3 && flag4)
					{
						this.SetImage(num5, num6, 2, 1);
					}
					else if (!flag && flag2 && flag3 && flag4)
					{
						this.SetImage(num5, num6, 0, 1);
					}
					else if (flag && !flag2 && !flag3 && flag4)
					{
						this.SetImage(num5, num6, 2, 0);
					}
					else if (!flag && flag2 && !flag3 && flag4)
					{
						this.SetImage(num5, num6, 0, 0);
					}
					else if (flag && !flag2 && flag3 && !flag4)
					{
						this.SetImage(num5, num6, 2, 2);
					}
					else if (!flag && flag2 && flag3 && !flag4)
					{
						this.SetImage(num5, num6, 0, 2);
					}
				}
			}
			this.UpdateVisualState();
		}

		// Token: 0x06000BB8 RID: 3000 RVA: 0x0002330C File Offset: 0x0002150C
		private void FindInGroup(CassetteBlock block)
		{
			foreach (Entity entity in base.Scene.Tracker.GetEntities<CassetteBlock>())
			{
				CassetteBlock cassetteBlock = (CassetteBlock)entity;
				if (cassetteBlock != this && cassetteBlock != block && cassetteBlock.Index == this.Index && (cassetteBlock.CollideRect(new Rectangle((int)block.X - 1, (int)block.Y, (int)block.Width + 2, (int)block.Height)) || cassetteBlock.CollideRect(new Rectangle((int)block.X, (int)block.Y - 1, (int)block.Width, (int)block.Height + 2))) && !this.group.Contains(cassetteBlock))
				{
					this.group.Add(cassetteBlock);
					this.FindInGroup(cassetteBlock);
					cassetteBlock.group = this.group;
				}
			}
		}

		// Token: 0x06000BB9 RID: 3001 RVA: 0x00023418 File Offset: 0x00021618
		private bool CheckForSame(float x, float y)
		{
			foreach (Entity entity in base.Scene.Tracker.GetEntities<CassetteBlock>())
			{
				CassetteBlock cassetteBlock = (CassetteBlock)entity;
				if (cassetteBlock.Index == this.Index && cassetteBlock.Collider.Collide(new Rectangle((int)x, (int)y, 8, 8)))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000BBA RID: 3002 RVA: 0x000234A0 File Offset: 0x000216A0
		private void SetImage(float x, float y, int tx, int ty)
		{
			List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/cassetteblock/pressed");
			this.pressed.Add(this.CreateImage(x, y, tx, ty, atlasSubtextures[this.Index % atlasSubtextures.Count]));
			this.solid.Add(this.CreateImage(x, y, tx, ty, GFX.Game["objects/cassetteblock/solid"]));
		}

		// Token: 0x06000BBB RID: 3003 RVA: 0x0002350C File Offset: 0x0002170C
		private Image CreateImage(float x, float y, int tx, int ty, MTexture tex)
		{
			Vector2 value = new Vector2(x - base.X, y - base.Y);
			Image image = new Image(tex.GetSubtexture(tx * 8, ty * 8, 8, 8, null));
			Vector2 vector = this.groupOrigin - this.Position;
			image.Origin = vector - value;
			image.Position = vector;
			image.Color = this.color;
			base.Add(image);
			this.all.Add(image);
			return image;
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x00023590 File Offset: 0x00021790
		public override void Update()
		{
			base.Update();
			if (this.groupLeader && this.Activated && !this.Collidable)
			{
				bool flag = false;
				using (List<CassetteBlock>.Enumerator enumerator = this.group.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.BlockedCheck())
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					foreach (CassetteBlock cassetteBlock in this.group)
					{
						cassetteBlock.Collidable = true;
						cassetteBlock.EnableStaticMovers();
						cassetteBlock.ShiftSize(-1);
					}
					this.wiggler.Start();
				}
			}
			else if (!this.Activated && this.Collidable)
			{
				this.ShiftSize(1);
				this.Collidable = false;
				base.DisableStaticMovers();
			}
			this.UpdateVisualState();
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x00023698 File Offset: 0x00021898
		public bool BlockedCheck()
		{
			TheoCrystal theoCrystal = base.CollideFirst<TheoCrystal>();
			if (theoCrystal != null && !this.TryActorWiggleUp(theoCrystal))
			{
				return true;
			}
			Player player = base.CollideFirst<Player>();
			return player != null && !this.TryActorWiggleUp(player);
		}

		// Token: 0x06000BBE RID: 3006 RVA: 0x000236D0 File Offset: 0x000218D0
		private void UpdateVisualState()
		{
			if (!this.Collidable)
			{
				base.Depth = 8990;
			}
			else
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.Top >= base.Bottom - 1f)
				{
					base.Depth = 10;
				}
				else
				{
					base.Depth = -10;
				}
			}
			foreach (StaticMover staticMover in this.staticMovers)
			{
				staticMover.Entity.Depth = base.Depth + 1;
			}
			this.side.Depth = base.Depth + 5;
			this.side.Visible = (this.blockHeight > 0);
			this.occluder.Visible = this.Collidable;
			foreach (Image image in this.solid)
			{
				image.Visible = this.Collidable;
			}
			foreach (Image image2 in this.pressed)
			{
				image2.Visible = !this.Collidable;
			}
			if (this.groupLeader)
			{
				Vector2 scale = new Vector2(1f + this.wiggler.Value * 0.05f * this.wigglerScaler.X, 1f + this.wiggler.Value * 0.15f * this.wigglerScaler.Y);
				foreach (CassetteBlock cassetteBlock in this.group)
				{
					foreach (Image image3 in cassetteBlock.all)
					{
						image3.Scale = scale;
					}
					foreach (StaticMover staticMover2 in cassetteBlock.staticMovers)
					{
						Spikes spikes = staticMover2.Entity as Spikes;
						if (spikes != null)
						{
							foreach (Component component in spikes.Components)
							{
								Image image4 = component as Image;
								if (image4 != null)
								{
									image4.Scale = scale;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000BBF RID: 3007 RVA: 0x000239B8 File Offset: 0x00021BB8
		public void SetActivatedSilently(bool activated)
		{
			this.Collidable = activated;
			this.Activated = activated;
			this.UpdateVisualState();
			if (activated)
			{
				base.EnableStaticMovers();
				return;
			}
			this.ShiftSize(2);
			base.DisableStaticMovers();
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x000239F2 File Offset: 0x00021BF2
		public void Finish()
		{
			this.Activated = false;
		}

		// Token: 0x06000BC1 RID: 3009 RVA: 0x000239FB File Offset: 0x00021BFB
		public void WillToggle()
		{
			this.ShiftSize(this.Collidable ? 1 : -1);
			this.UpdateVisualState();
		}

		// Token: 0x06000BC2 RID: 3010 RVA: 0x00023A15 File Offset: 0x00021C15
		private void ShiftSize(int amount)
		{
			base.MoveV((float)amount);
			this.blockHeight -= amount;
		}

		// Token: 0x06000BC3 RID: 3011 RVA: 0x00023A30 File Offset: 0x00021C30
		private bool TryActorWiggleUp(Entity actor)
		{
			foreach (CassetteBlock cassetteBlock in this.group)
			{
				if (cassetteBlock != this && cassetteBlock.CollideCheck(actor, cassetteBlock.Position + Vector2.UnitY * 4f))
				{
					return false;
				}
			}
			bool collidable = this.Collidable;
			this.Collidable = true;
			for (int i = 1; i <= 4; i++)
			{
				if (!actor.CollideCheck<Solid>(actor.Position - Vector2.UnitY * (float)i))
				{
					actor.Position -= Vector2.UnitY * (float)i;
					this.Collidable = collidable;
					return true;
				}
			}
			this.Collidable = collidable;
			return false;
		}

		// Token: 0x0400070F RID: 1807
		public int Index;

		// Token: 0x04000710 RID: 1808
		public float Tempo;

		// Token: 0x04000711 RID: 1809
		public bool Activated;

		// Token: 0x04000712 RID: 1810
		public CassetteBlock.Modes Mode;

		// Token: 0x04000713 RID: 1811
		public EntityID ID;

		// Token: 0x04000714 RID: 1812
		private int blockHeight = 2;

		// Token: 0x04000715 RID: 1813
		private List<CassetteBlock> group;

		// Token: 0x04000716 RID: 1814
		private bool groupLeader;

		// Token: 0x04000717 RID: 1815
		private Vector2 groupOrigin;

		// Token: 0x04000718 RID: 1816
		private Color color;

		// Token: 0x04000719 RID: 1817
		private List<Image> pressed = new List<Image>();

		// Token: 0x0400071A RID: 1818
		private List<Image> solid = new List<Image>();

		// Token: 0x0400071B RID: 1819
		private List<Image> all = new List<Image>();

		// Token: 0x0400071C RID: 1820
		private LightOcclude occluder;

		// Token: 0x0400071D RID: 1821
		private Wiggler wiggler;

		// Token: 0x0400071E RID: 1822
		private Vector2 wigglerScaler;

		// Token: 0x0400071F RID: 1823
		private CassetteBlock.BoxSide side;

		// Token: 0x020003C6 RID: 966
		public enum Modes
		{
			// Token: 0x04001F80 RID: 8064
			Solid,
			// Token: 0x04001F81 RID: 8065
			Leaving,
			// Token: 0x04001F82 RID: 8066
			Disabled,
			// Token: 0x04001F83 RID: 8067
			Returning
		}

		// Token: 0x020003C7 RID: 967
		private class BoxSide : Entity
		{
			// Token: 0x06001EDF RID: 7903 RVA: 0x000D63F8 File Offset: 0x000D45F8
			public BoxSide(CassetteBlock block, Color color)
			{
				this.block = block;
				this.color = color;
			}

			// Token: 0x06001EE0 RID: 7904 RVA: 0x000D6410 File Offset: 0x000D4610
			public override void Render()
			{
				Draw.Rect(this.block.X, this.block.Y + this.block.Height - 8f, this.block.Width, (float)(8 + this.block.blockHeight), this.color);
			}

			// Token: 0x04001F84 RID: 8068
			private CassetteBlock block;

			// Token: 0x04001F85 RID: 8069
			private Color color;
		}
	}
}
