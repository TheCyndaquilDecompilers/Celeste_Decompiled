using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000144 RID: 324
	[Tracked(false)]
	public class FloatySpaceBlock : Solid
	{
		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000BD2 RID: 3026 RVA: 0x000240E8 File Offset: 0x000222E8
		// (set) Token: 0x06000BD3 RID: 3027 RVA: 0x000240F0 File Offset: 0x000222F0
		public bool HasGroup { get; private set; }

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000BD4 RID: 3028 RVA: 0x000240F9 File Offset: 0x000222F9
		// (set) Token: 0x06000BD5 RID: 3029 RVA: 0x00024101 File Offset: 0x00022301
		public bool MasterOfGroup { get; private set; }

		// Token: 0x06000BD6 RID: 3030 RVA: 0x0002410C File Offset: 0x0002230C
		public FloatySpaceBlock(Vector2 position, float width, float height, char tileType, bool disableSpawnOffset) : base(position, width, height, true)
		{
			this.tileType = tileType;
			base.Depth = -9000;
			base.Add(new LightOcclude(1f));
			this.SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
			if (!disableSpawnOffset)
			{
				this.sineWave = Calc.Random.NextFloat(6.2831855f);
				return;
			}
			this.sineWave = 0f;
		}

		// Token: 0x06000BD7 RID: 3031 RVA: 0x0002417D File Offset: 0x0002237D
		public FloatySpaceBlock(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height, data.Char("tiletype", '3'), data.Bool("disableSpawnOffset", false))
		{
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x000241B8 File Offset: 0x000223B8
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			this.awake = true;
			if (!this.HasGroup)
			{
				this.MasterOfGroup = true;
				this.Moves = new Dictionary<Platform, Vector2>();
				this.Group = new List<FloatySpaceBlock>();
				this.Jumpthrus = new List<JumpThru>();
				this.GroupBoundsMin = new Point((int)base.X, (int)base.Y);
				this.GroupBoundsMax = new Point((int)base.Right, (int)base.Bottom);
				this.AddToGroupAndFindChildren(this);
				Scene scene2 = base.Scene;
				Rectangle rectangle = new Rectangle(this.GroupBoundsMin.X / 8, this.GroupBoundsMin.Y / 8, (this.GroupBoundsMax.X - this.GroupBoundsMin.X) / 8 + 1, (this.GroupBoundsMax.Y - this.GroupBoundsMin.Y) / 8 + 1);
				VirtualMap<char> virtualMap = new VirtualMap<char>(rectangle.Width, rectangle.Height, '0');
				foreach (FloatySpaceBlock floatySpaceBlock in this.Group)
				{
					int num = (int)(floatySpaceBlock.X / 8f) - rectangle.X;
					int num2 = (int)(floatySpaceBlock.Y / 8f) - rectangle.Y;
					int num3 = (int)(floatySpaceBlock.Width / 8f);
					int num4 = (int)(floatySpaceBlock.Height / 8f);
					for (int i = num; i < num + num3; i++)
					{
						for (int j = num2; j < num2 + num4; j++)
						{
							virtualMap[i, j] = this.tileType;
						}
					}
				}
				this.tiles = GFX.FGAutotiler.GenerateMap(virtualMap, new Autotiler.Behaviour
				{
					EdgesExtend = false,
					EdgesIgnoreOutOfLevel = false,
					PaddingIgnoreOutOfLevel = false
				}).TileGrid;
				this.tiles.Position = new Vector2((float)this.GroupBoundsMin.X - base.X, (float)this.GroupBoundsMin.Y - base.Y);
				base.Add(this.tiles);
			}
			this.TryToInitPosition();
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x000243F8 File Offset: 0x000225F8
		public override void OnStaticMoverTrigger(StaticMover sm)
		{
			if (sm.Entity is Spring)
			{
				switch ((sm.Entity as Spring).Orientation)
				{
				case Spring.Orientations.Floor:
					this.sinkTimer = 0.5f;
					return;
				case Spring.Orientations.WallLeft:
					this.dashEase = 1f;
					this.dashDirection = -Vector2.UnitX;
					return;
				case Spring.Orientations.WallRight:
					this.dashEase = 1f;
					this.dashDirection = Vector2.UnitX;
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x00024474 File Offset: 0x00022674
		private void TryToInitPosition()
		{
			if (this.MasterOfGroup)
			{
				using (List<FloatySpaceBlock>.Enumerator enumerator = this.Group.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.awake)
						{
							return;
						}
					}
				}
				this.MoveToTarget();
				return;
			}
			this.master.TryToInitPosition();
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x000244E4 File Offset: 0x000226E4
		private void AddToGroupAndFindChildren(FloatySpaceBlock from)
		{
			if (from.X < (float)this.GroupBoundsMin.X)
			{
				this.GroupBoundsMin.X = (int)from.X;
			}
			if (from.Y < (float)this.GroupBoundsMin.Y)
			{
				this.GroupBoundsMin.Y = (int)from.Y;
			}
			if (from.Right > (float)this.GroupBoundsMax.X)
			{
				this.GroupBoundsMax.X = (int)from.Right;
			}
			if (from.Bottom > (float)this.GroupBoundsMax.Y)
			{
				this.GroupBoundsMax.Y = (int)from.Bottom;
			}
			from.HasGroup = true;
			from.OnDashCollide = new DashCollision(this.OnDash);
			this.Group.Add(from);
			this.Moves.Add(from, from.Position);
			if (from != this)
			{
				from.master = this;
			}
			foreach (JumpThru jumpThru in base.Scene.CollideAll<JumpThru>(new Rectangle((int)from.X - 1, (int)from.Y, (int)from.Width + 2, (int)from.Height)))
			{
				if (!this.Jumpthrus.Contains(jumpThru))
				{
					this.AddJumpThru(jumpThru);
				}
			}
			foreach (JumpThru jumpThru2 in base.Scene.CollideAll<JumpThru>(new Rectangle((int)from.X, (int)from.Y - 1, (int)from.Width, (int)from.Height + 2)))
			{
				if (!this.Jumpthrus.Contains(jumpThru2))
				{
					this.AddJumpThru(jumpThru2);
				}
			}
			foreach (Entity entity in base.Scene.Tracker.GetEntities<FloatySpaceBlock>())
			{
				FloatySpaceBlock floatySpaceBlock = (FloatySpaceBlock)entity;
				if (!floatySpaceBlock.HasGroup && floatySpaceBlock.tileType == this.tileType && (base.Scene.CollideCheck(new Rectangle((int)from.X - 1, (int)from.Y, (int)from.Width + 2, (int)from.Height), floatySpaceBlock) || base.Scene.CollideCheck(new Rectangle((int)from.X, (int)from.Y - 1, (int)from.Width, (int)from.Height + 2), floatySpaceBlock)))
				{
					this.AddToGroupAndFindChildren(floatySpaceBlock);
				}
			}
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x0002479C File Offset: 0x0002299C
		private void AddJumpThru(JumpThru jp)
		{
			jp.OnDashCollide = new DashCollision(this.OnDash);
			this.Jumpthrus.Add(jp);
			this.Moves.Add(jp, jp.Position);
			foreach (Entity entity in base.Scene.Tracker.GetEntities<FloatySpaceBlock>())
			{
				FloatySpaceBlock floatySpaceBlock = (FloatySpaceBlock)entity;
				if (!floatySpaceBlock.HasGroup && floatySpaceBlock.tileType == this.tileType && base.Scene.CollideCheck(new Rectangle((int)jp.X - 1, (int)jp.Y, (int)jp.Width + 2, (int)jp.Height), floatySpaceBlock))
				{
					this.AddToGroupAndFindChildren(floatySpaceBlock);
				}
			}
		}

		// Token: 0x06000BDD RID: 3037 RVA: 0x00024878 File Offset: 0x00022A78
		private DashCollisionResults OnDash(Player player, Vector2 direction)
		{
			if (this.MasterOfGroup && this.dashEase <= 0.2f)
			{
				this.dashEase = 1f;
				this.dashDirection = direction;
			}
			return DashCollisionResults.NormalOverride;
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x000248A4 File Offset: 0x00022AA4
		public override void Update()
		{
			base.Update();
			if (this.MasterOfGroup)
			{
				bool flag = false;
				using (List<FloatySpaceBlock>.Enumerator enumerator = this.Group.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.HasPlayerRider())
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					using (List<JumpThru>.Enumerator enumerator2 = this.Jumpthrus.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.HasPlayerRider())
							{
								flag = true;
								break;
							}
						}
					}
				}
				if (flag)
				{
					this.sinkTimer = 0.3f;
				}
				else if (this.sinkTimer > 0f)
				{
					this.sinkTimer -= Engine.DeltaTime;
				}
				if (this.sinkTimer > 0f)
				{
					this.yLerp = Calc.Approach(this.yLerp, 1f, 1f * Engine.DeltaTime);
				}
				else
				{
					this.yLerp = Calc.Approach(this.yLerp, 0f, 1f * Engine.DeltaTime);
				}
				this.sineWave += Engine.DeltaTime;
				this.dashEase = Calc.Approach(this.dashEase, 0f, Engine.DeltaTime * 1.5f);
				this.MoveToTarget();
			}
			this.LiftSpeed = Vector2.Zero;
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x00024A1C File Offset: 0x00022C1C
		private void MoveToTarget()
		{
			float num = (float)Math.Sin((double)this.sineWave) * 4f;
			Vector2 vector = Calc.YoYo(Ease.QuadIn(this.dashEase)) * this.dashDirection * 8f;
			for (int i = 0; i < 2; i++)
			{
				foreach (KeyValuePair<Platform, Vector2> keyValuePair in this.Moves)
				{
					Platform key = keyValuePair.Key;
					bool flag = false;
					JumpThru jumpThru = key as JumpThru;
					Solid solid = key as Solid;
					if ((jumpThru != null && jumpThru.HasRider()) || (solid != null && solid.HasRider()))
					{
						flag = true;
					}
					if ((flag || i != 0) && (!flag || i != 1))
					{
						Vector2 value = keyValuePair.Value;
						float num2 = MathHelper.Lerp(value.Y, value.Y + 12f, Ease.SineInOut(this.yLerp)) + num;
						key.MoveToY(num2 + vector.Y);
						key.MoveToX(value.X + vector.X);
					}
				}
			}
		}

		// Token: 0x06000BE0 RID: 3040 RVA: 0x00024B68 File Offset: 0x00022D68
		public override void OnShake(Vector2 amount)
		{
			if (this.MasterOfGroup)
			{
				base.OnShake(amount);
				this.tiles.Position += amount;
				foreach (JumpThru jumpThru in this.Jumpthrus)
				{
					foreach (Component component in jumpThru.Components)
					{
						Image image = component as Image;
						if (image != null)
						{
							image.Position += amount;
						}
					}
				}
			}
		}

		// Token: 0x0400072A RID: 1834
		private TileGrid tiles;

		// Token: 0x0400072B RID: 1835
		private char tileType;

		// Token: 0x0400072C RID: 1836
		private float yLerp;

		// Token: 0x0400072D RID: 1837
		private float sinkTimer;

		// Token: 0x0400072E RID: 1838
		private float sineWave;

		// Token: 0x0400072F RID: 1839
		private float dashEase;

		// Token: 0x04000730 RID: 1840
		private Vector2 dashDirection;

		// Token: 0x04000731 RID: 1841
		private FloatySpaceBlock master;

		// Token: 0x04000732 RID: 1842
		private bool awake;

		// Token: 0x04000735 RID: 1845
		public List<FloatySpaceBlock> Group;

		// Token: 0x04000736 RID: 1846
		public List<JumpThru> Jumpthrus;

		// Token: 0x04000737 RID: 1847
		public Dictionary<Platform, Vector2> Moves;

		// Token: 0x04000738 RID: 1848
		public Point GroupBoundsMin;

		// Token: 0x04000739 RID: 1849
		public Point GroupBoundsMax;
	}
}
