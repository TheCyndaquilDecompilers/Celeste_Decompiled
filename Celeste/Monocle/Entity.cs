using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x02000106 RID: 262
	public class Entity : IEnumerable<Component>, IEnumerable
	{
		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000761 RID: 1889 RVA: 0x0000D738 File Offset: 0x0000B938
		// (set) Token: 0x06000762 RID: 1890 RVA: 0x0000D740 File Offset: 0x0000B940
		public Scene Scene { get; private set; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000763 RID: 1891 RVA: 0x0000D749 File Offset: 0x0000B949
		// (set) Token: 0x06000764 RID: 1892 RVA: 0x0000D751 File Offset: 0x0000B951
		public ComponentList Components { get; private set; }

		// Token: 0x06000765 RID: 1893 RVA: 0x0000D75A File Offset: 0x0000B95A
		public Entity(Vector2 position)
		{
			this.Position = position;
			this.Components = new ComponentList(this);
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x0000D78A File Offset: 0x0000B98A
		public Entity() : this(Vector2.Zero)
		{
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void SceneBegin(Scene scene)
		{
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x0000D798 File Offset: 0x0000B998
		public virtual void SceneEnd(Scene scene)
		{
			if (this.Components != null)
			{
				foreach (Component component in this.Components)
				{
					component.SceneEnd(scene);
				}
			}
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x0000D7EC File Offset: 0x0000B9EC
		public virtual void Awake(Scene scene)
		{
			if (this.Components != null)
			{
				foreach (Component component in this.Components)
				{
					component.EntityAwake();
				}
			}
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x0000D840 File Offset: 0x0000BA40
		public virtual void Added(Scene scene)
		{
			this.Scene = scene;
			if (this.Components != null)
			{
				foreach (Component component in this.Components)
				{
					component.EntityAdded(scene);
				}
			}
			this.Scene.SetActualDepth(this);
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x0000D8A8 File Offset: 0x0000BAA8
		public virtual void Removed(Scene scene)
		{
			if (this.Components != null)
			{
				foreach (Component component in this.Components)
				{
					component.EntityRemoved(scene);
				}
			}
			this.Scene = null;
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x0000D904 File Offset: 0x0000BB04
		public virtual void Update()
		{
			this.Components.Update();
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x0000D911 File Offset: 0x0000BB11
		public virtual void Render()
		{
			this.Components.Render();
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x0000D91E File Offset: 0x0000BB1E
		public virtual void DebugRender(Camera camera)
		{
			if (this.Collider != null)
			{
				this.Collider.Render(camera, this.Collidable ? Color.Red : Color.DarkRed);
			}
			this.Components.DebugRender(camera);
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x0000D954 File Offset: 0x0000BB54
		public virtual void HandleGraphicsReset()
		{
			this.Components.HandleGraphicsReset();
		}

		// Token: 0x06000770 RID: 1904 RVA: 0x0000D961 File Offset: 0x0000BB61
		public virtual void HandleGraphicsCreate()
		{
			this.Components.HandleGraphicsCreate();
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x0000D96E File Offset: 0x0000BB6E
		public void RemoveSelf()
		{
			if (this.Scene != null)
			{
				this.Scene.Entities.Remove(this);
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000772 RID: 1906 RVA: 0x0000D989 File Offset: 0x0000BB89
		// (set) Token: 0x06000773 RID: 1907 RVA: 0x0000D991 File Offset: 0x0000BB91
		public int Depth
		{
			get
			{
				return this.depth;
			}
			set
			{
				if (this.depth != value)
				{
					this.depth = value;
					if (this.Scene != null)
					{
						this.Scene.SetActualDepth(this);
					}
				}
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000774 RID: 1908 RVA: 0x0000D9B7 File Offset: 0x0000BBB7
		// (set) Token: 0x06000775 RID: 1909 RVA: 0x0000D9C4 File Offset: 0x0000BBC4
		public float X
		{
			get
			{
				return this.Position.X;
			}
			set
			{
				this.Position.X = value;
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000776 RID: 1910 RVA: 0x0000D9D2 File Offset: 0x0000BBD2
		// (set) Token: 0x06000777 RID: 1911 RVA: 0x0000D9DF File Offset: 0x0000BBDF
		public float Y
		{
			get
			{
				return this.Position.Y;
			}
			set
			{
				this.Position.Y = value;
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000778 RID: 1912 RVA: 0x0000D9ED File Offset: 0x0000BBED
		// (set) Token: 0x06000779 RID: 1913 RVA: 0x0000D9F5 File Offset: 0x0000BBF5
		public Collider Collider
		{
			get
			{
				return this.collider;
			}
			set
			{
				if (value == this.collider)
				{
					return;
				}
				if (this.collider != null)
				{
					this.collider.Removed();
				}
				this.collider = value;
				if (this.collider != null)
				{
					this.collider.Added(this);
				}
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x0600077A RID: 1914 RVA: 0x0000DA2F File Offset: 0x0000BC2F
		public float Width
		{
			get
			{
				if (this.Collider == null)
				{
					return 0f;
				}
				return this.Collider.Width;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600077B RID: 1915 RVA: 0x0000DA4A File Offset: 0x0000BC4A
		public float Height
		{
			get
			{
				if (this.Collider == null)
				{
					return 0f;
				}
				return this.Collider.Height;
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x0600077C RID: 1916 RVA: 0x0000DA65 File Offset: 0x0000BC65
		// (set) Token: 0x0600077D RID: 1917 RVA: 0x0000DA8D File Offset: 0x0000BC8D
		public float Left
		{
			get
			{
				if (this.Collider == null)
				{
					return this.X;
				}
				return this.Position.X + this.Collider.Left;
			}
			set
			{
				if (this.Collider == null)
				{
					this.Position.X = value;
					return;
				}
				this.Position.X = value - this.Collider.Left;
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x0600077E RID: 1918 RVA: 0x0000DABC File Offset: 0x0000BCBC
		// (set) Token: 0x0600077F RID: 1919 RVA: 0x0000DAE9 File Offset: 0x0000BCE9
		public float Right
		{
			get
			{
				if (this.Collider == null)
				{
					return this.Position.X;
				}
				return this.Position.X + this.Collider.Right;
			}
			set
			{
				if (this.Collider == null)
				{
					this.Position.X = value;
					return;
				}
				this.Position.X = value - this.Collider.Right;
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000780 RID: 1920 RVA: 0x0000DB18 File Offset: 0x0000BD18
		// (set) Token: 0x06000781 RID: 1921 RVA: 0x0000DB45 File Offset: 0x0000BD45
		public float Top
		{
			get
			{
				if (this.Collider == null)
				{
					return this.Position.Y;
				}
				return this.Position.Y + this.Collider.Top;
			}
			set
			{
				if (this.Collider == null)
				{
					this.Position.Y = value;
					return;
				}
				this.Position.Y = value - this.Collider.Top;
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000782 RID: 1922 RVA: 0x0000DB74 File Offset: 0x0000BD74
		// (set) Token: 0x06000783 RID: 1923 RVA: 0x0000DBA1 File Offset: 0x0000BDA1
		public float Bottom
		{
			get
			{
				if (this.Collider == null)
				{
					return this.Position.Y;
				}
				return this.Position.Y + this.Collider.Bottom;
			}
			set
			{
				if (this.Collider == null)
				{
					this.Position.Y = value;
					return;
				}
				this.Position.Y = value - this.Collider.Bottom;
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000784 RID: 1924 RVA: 0x0000DBD0 File Offset: 0x0000BDD0
		// (set) Token: 0x06000785 RID: 1925 RVA: 0x0000DBFD File Offset: 0x0000BDFD
		public float CenterX
		{
			get
			{
				if (this.Collider == null)
				{
					return this.Position.X;
				}
				return this.Position.X + this.Collider.CenterX;
			}
			set
			{
				if (this.Collider == null)
				{
					this.Position.X = value;
					return;
				}
				this.Position.X = value - this.Collider.CenterX;
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000786 RID: 1926 RVA: 0x0000DC2C File Offset: 0x0000BE2C
		// (set) Token: 0x06000787 RID: 1927 RVA: 0x0000DC59 File Offset: 0x0000BE59
		public float CenterY
		{
			get
			{
				if (this.Collider == null)
				{
					return this.Position.Y;
				}
				return this.Position.Y + this.Collider.CenterY;
			}
			set
			{
				if (this.Collider == null)
				{
					this.Position.Y = value;
					return;
				}
				this.Position.Y = value - this.Collider.CenterY;
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000788 RID: 1928 RVA: 0x0000DC88 File Offset: 0x0000BE88
		// (set) Token: 0x06000789 RID: 1929 RVA: 0x0000DC9B File Offset: 0x0000BE9B
		public Vector2 TopLeft
		{
			get
			{
				return new Vector2(this.Left, this.Top);
			}
			set
			{
				this.Left = value.X;
				this.Top = value.Y;
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x0600078A RID: 1930 RVA: 0x0000DCB5 File Offset: 0x0000BEB5
		// (set) Token: 0x0600078B RID: 1931 RVA: 0x0000DCC8 File Offset: 0x0000BEC8
		public Vector2 TopRight
		{
			get
			{
				return new Vector2(this.Right, this.Top);
			}
			set
			{
				this.Right = value.X;
				this.Top = value.Y;
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x0600078C RID: 1932 RVA: 0x0000DCE2 File Offset: 0x0000BEE2
		// (set) Token: 0x0600078D RID: 1933 RVA: 0x0000DCF5 File Offset: 0x0000BEF5
		public Vector2 BottomLeft
		{
			get
			{
				return new Vector2(this.Left, this.Bottom);
			}
			set
			{
				this.Left = value.X;
				this.Bottom = value.Y;
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x0600078E RID: 1934 RVA: 0x0000DD0F File Offset: 0x0000BF0F
		// (set) Token: 0x0600078F RID: 1935 RVA: 0x0000DD22 File Offset: 0x0000BF22
		public Vector2 BottomRight
		{
			get
			{
				return new Vector2(this.Right, this.Bottom);
			}
			set
			{
				this.Right = value.X;
				this.Bottom = value.Y;
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000790 RID: 1936 RVA: 0x0000DD3C File Offset: 0x0000BF3C
		// (set) Token: 0x06000791 RID: 1937 RVA: 0x0000DD4F File Offset: 0x0000BF4F
		public Vector2 Center
		{
			get
			{
				return new Vector2(this.CenterX, this.CenterY);
			}
			set
			{
				this.CenterX = value.X;
				this.CenterY = value.Y;
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000792 RID: 1938 RVA: 0x0000DD69 File Offset: 0x0000BF69
		// (set) Token: 0x06000793 RID: 1939 RVA: 0x0000DD7C File Offset: 0x0000BF7C
		public Vector2 CenterLeft
		{
			get
			{
				return new Vector2(this.Left, this.CenterY);
			}
			set
			{
				this.Left = value.X;
				this.CenterY = value.Y;
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000794 RID: 1940 RVA: 0x0000DD96 File Offset: 0x0000BF96
		// (set) Token: 0x06000795 RID: 1941 RVA: 0x0000DDA9 File Offset: 0x0000BFA9
		public Vector2 CenterRight
		{
			get
			{
				return new Vector2(this.Right, this.CenterY);
			}
			set
			{
				this.Right = value.X;
				this.CenterY = value.Y;
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000796 RID: 1942 RVA: 0x0000DDC3 File Offset: 0x0000BFC3
		// (set) Token: 0x06000797 RID: 1943 RVA: 0x0000DDD6 File Offset: 0x0000BFD6
		public Vector2 TopCenter
		{
			get
			{
				return new Vector2(this.CenterX, this.Top);
			}
			set
			{
				this.CenterX = value.X;
				this.Top = value.Y;
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000798 RID: 1944 RVA: 0x0000DDF0 File Offset: 0x0000BFF0
		// (set) Token: 0x06000799 RID: 1945 RVA: 0x0000DE03 File Offset: 0x0000C003
		public Vector2 BottomCenter
		{
			get
			{
				return new Vector2(this.CenterX, this.Bottom);
			}
			set
			{
				this.CenterX = value.X;
				this.Bottom = value.Y;
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x0600079A RID: 1946 RVA: 0x0000DE1D File Offset: 0x0000C01D
		// (set) Token: 0x0600079B RID: 1947 RVA: 0x0000DE28 File Offset: 0x0000C028
		public int Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				if (this.tag != value)
				{
					if (this.Scene != null)
					{
						for (int i = 0; i < BitTag.TotalTags; i++)
						{
							int num = 1 << i;
							bool flag = (value & num) != 0;
							if ((this.Tag & num) != 0 != flag)
							{
								if (flag)
								{
									this.Scene.TagLists[i].Add(this);
								}
								else
								{
									this.Scene.TagLists[i].Remove(this);
								}
							}
						}
					}
					this.tag = value;
				}
			}
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x0000DEAD File Offset: 0x0000C0AD
		public bool TagFullCheck(int tag)
		{
			return (this.tag & tag) == tag;
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x0000DEBA File Offset: 0x0000C0BA
		public bool TagCheck(int tag)
		{
			return (this.tag & tag) != 0;
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x0000DEC7 File Offset: 0x0000C0C7
		public void AddTag(int tag)
		{
			this.Tag |= tag;
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x0000DED7 File Offset: 0x0000C0D7
		public void RemoveTag(int tag)
		{
			this.Tag &= ~tag;
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x0000DEE8 File Offset: 0x0000C0E8
		public bool CollideCheck(Entity other)
		{
			return Collide.Check(this, other);
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x0000DEF1 File Offset: 0x0000C0F1
		public bool CollideCheck(Entity other, Vector2 at)
		{
			return Collide.Check(this, other, at);
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x0000DEFB File Offset: 0x0000C0FB
		public bool CollideCheck(BitTag tag)
		{
			return Collide.Check(this, this.Scene[tag]);
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x0000DF0F File Offset: 0x0000C10F
		public bool CollideCheck(BitTag tag, Vector2 at)
		{
			return Collide.Check(this, this.Scene[tag], at);
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x0000DF24 File Offset: 0x0000C124
		public bool CollideCheck<T>() where T : Entity
		{
			return Collide.Check(this, this.Scene.Tracker.Entities[typeof(T)]);
		}

		// Token: 0x060007A5 RID: 1957 RVA: 0x0000DF4B File Offset: 0x0000C14B
		public bool CollideCheck<T>(Vector2 at) where T : Entity
		{
			return Collide.Check(this, this.Scene.Tracker.Entities[typeof(T)], at);
		}

		// Token: 0x060007A6 RID: 1958 RVA: 0x0000DF74 File Offset: 0x0000C174
		public bool CollideCheck<T, Exclude>() where T : Entity where Exclude : Entity
		{
			List<Entity> list = this.Scene.Tracker.Entities[typeof(Exclude)];
			foreach (Entity entity in this.Scene.Tracker.Entities[typeof(T)])
			{
				if (!list.Contains(entity) && Collide.Check(this, entity))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x0000E014 File Offset: 0x0000C214
		public bool CollideCheck<T, Exclude>(Vector2 at) where T : Entity where Exclude : Entity
		{
			Vector2 position = this.Position;
			this.Position = at;
			bool result = this.CollideCheck<T, Exclude>();
			this.Position = position;
			return result;
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x0000E03C File Offset: 0x0000C23C
		public bool CollideCheck<T, Exclude1, Exclude2>() where T : Entity where Exclude1 : Entity where Exclude2 : Entity
		{
			List<Entity> list = this.Scene.Tracker.Entities[typeof(Exclude1)];
			List<Entity> list2 = this.Scene.Tracker.Entities[typeof(Exclude2)];
			foreach (Entity entity in this.Scene.Tracker.Entities[typeof(T)])
			{
				if (!list.Contains(entity) && !list2.Contains(entity) && Collide.Check(this, entity))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x0000E108 File Offset: 0x0000C308
		public bool CollideCheck<T, Exclude1, Exclude2>(Vector2 at) where T : Entity where Exclude1 : Entity where Exclude2 : Entity
		{
			Vector2 position = this.Position;
			this.Position = at;
			bool result = this.CollideCheck<T, Exclude1, Exclude2>();
			this.Position = position;
			return result;
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x0000E130 File Offset: 0x0000C330
		public bool CollideCheckByComponent<T>() where T : Component
		{
			foreach (Component component in this.Scene.Tracker.Components[typeof(T)])
			{
				if (Collide.Check(this, component.Entity))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x0000E1AC File Offset: 0x0000C3AC
		public bool CollideCheckByComponent<T>(Vector2 at) where T : Component
		{
			Vector2 position = this.Position;
			this.Position = at;
			bool result = this.CollideCheckByComponent<T>();
			this.Position = position;
			return result;
		}

		// Token: 0x060007AC RID: 1964 RVA: 0x0000E1D4 File Offset: 0x0000C3D4
		public bool CollideCheckOutside(Entity other, Vector2 at)
		{
			return !Collide.Check(this, other) && Collide.Check(this, other, at);
		}

		// Token: 0x060007AD RID: 1965 RVA: 0x0000E1EC File Offset: 0x0000C3EC
		public bool CollideCheckOutside(BitTag tag, Vector2 at)
		{
			foreach (Entity b in this.Scene[tag])
			{
				if (!Collide.Check(this, b) && Collide.Check(this, b, at))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060007AE RID: 1966 RVA: 0x0000E258 File Offset: 0x0000C458
		public bool CollideCheckOutside<T>(Vector2 at) where T : Entity
		{
			foreach (Entity b in this.Scene.Tracker.Entities[typeof(T)])
			{
				if (!Collide.Check(this, b) && Collide.Check(this, b, at))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060007AF RID: 1967 RVA: 0x0000E2D8 File Offset: 0x0000C4D8
		public bool CollideCheckOutsideByComponent<T>(Vector2 at) where T : Component
		{
			foreach (Component component in this.Scene.Tracker.Components[typeof(T)])
			{
				if (!Collide.Check(this, component.Entity) && Collide.Check(this, component.Entity, at))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060007B0 RID: 1968 RVA: 0x0000E364 File Offset: 0x0000C564
		public Entity CollideFirst(BitTag tag)
		{
			return Collide.First(this, this.Scene[tag]);
		}

		// Token: 0x060007B1 RID: 1969 RVA: 0x0000E378 File Offset: 0x0000C578
		public Entity CollideFirst(BitTag tag, Vector2 at)
		{
			return Collide.First(this, this.Scene[tag], at);
		}

		// Token: 0x060007B2 RID: 1970 RVA: 0x0000E38D File Offset: 0x0000C58D
		public T CollideFirst<T>() where T : Entity
		{
			return Collide.First(this, this.Scene.Tracker.Entities[typeof(T)]) as T;
		}

		// Token: 0x060007B3 RID: 1971 RVA: 0x0000E3BE File Offset: 0x0000C5BE
		public T CollideFirst<T>(Vector2 at) where T : Entity
		{
			return Collide.First(this, this.Scene.Tracker.Entities[typeof(T)], at) as T;
		}

		// Token: 0x060007B4 RID: 1972 RVA: 0x0000E3F0 File Offset: 0x0000C5F0
		public T CollideFirstByComponent<T>() where T : Component
		{
			foreach (Component component in this.Scene.Tracker.Components[typeof(T)])
			{
				if (Collide.Check(this, component.Entity))
				{
					return component as T;
				}
			}
			return default(T);
		}

		// Token: 0x060007B5 RID: 1973 RVA: 0x0000E47C File Offset: 0x0000C67C
		public T CollideFirstByComponent<T>(Vector2 at) where T : Component
		{
			foreach (Component component in this.Scene.Tracker.Components[typeof(T)])
			{
				if (Collide.Check(this, component.Entity, at))
				{
					return component as T;
				}
			}
			return default(T);
		}

		// Token: 0x060007B6 RID: 1974 RVA: 0x0000E50C File Offset: 0x0000C70C
		public Entity CollideFirstOutside(BitTag tag, Vector2 at)
		{
			foreach (Entity entity in this.Scene[tag])
			{
				if (!Collide.Check(this, entity) && Collide.Check(this, entity, at))
				{
					return entity;
				}
			}
			return null;
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x0000E578 File Offset: 0x0000C778
		public T CollideFirstOutside<T>(Vector2 at) where T : Entity
		{
			foreach (Entity entity in this.Scene.Tracker.Entities[typeof(T)])
			{
				if (!Collide.Check(this, entity) && Collide.Check(this, entity, at))
				{
					return entity as T;
				}
			}
			return default(T);
		}

		// Token: 0x060007B8 RID: 1976 RVA: 0x0000E60C File Offset: 0x0000C80C
		public T CollideFirstOutsideByComponent<T>(Vector2 at) where T : Component
		{
			foreach (Component component in this.Scene.Tracker.Components[typeof(T)])
			{
				if (!Collide.Check(this, component.Entity) && Collide.Check(this, component.Entity, at))
				{
					return component as T;
				}
			}
			return default(T);
		}

		// Token: 0x060007B9 RID: 1977 RVA: 0x0000E6A8 File Offset: 0x0000C8A8
		public List<Entity> CollideAll(BitTag tag)
		{
			return Collide.All(this, this.Scene[tag]);
		}

		// Token: 0x060007BA RID: 1978 RVA: 0x0000E6BC File Offset: 0x0000C8BC
		public List<Entity> CollideAll(BitTag tag, Vector2 at)
		{
			return Collide.All(this, this.Scene[tag], at);
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x0000E6D1 File Offset: 0x0000C8D1
		public List<Entity> CollideAll<T>() where T : Entity
		{
			return Collide.All(this, this.Scene.Tracker.Entities[typeof(T)]);
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x0000E6F8 File Offset: 0x0000C8F8
		public List<Entity> CollideAll<T>(Vector2 at) where T : Entity
		{
			return Collide.All(this, this.Scene.Tracker.Entities[typeof(T)], at);
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x0000E720 File Offset: 0x0000C920
		public List<Entity> CollideAll<T>(Vector2 at, List<Entity> into) where T : Entity
		{
			into.Clear();
			return Collide.All(this, this.Scene.Tracker.Entities[typeof(T)], into, at);
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x0000E750 File Offset: 0x0000C950
		public List<T> CollideAllByComponent<T>() where T : Component
		{
			List<T> list = new List<T>();
			foreach (Component component in this.Scene.Tracker.Components[typeof(T)])
			{
				if (Collide.Check(this, component.Entity))
				{
					list.Add(component as T);
				}
			}
			return list;
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x0000E7DC File Offset: 0x0000C9DC
		public List<T> CollideAllByComponent<T>(Vector2 at) where T : Component
		{
			Vector2 position = this.Position;
			this.Position = at;
			List<T> result = this.CollideAllByComponent<T>();
			this.Position = position;
			return result;
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x0000E804 File Offset: 0x0000CA04
		public bool CollideDo(BitTag tag, Action<Entity> action)
		{
			bool result = false;
			foreach (Entity entity in this.Scene[tag])
			{
				if (this.CollideCheck(entity))
				{
					action(entity);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x0000E86C File Offset: 0x0000CA6C
		public bool CollideDo(BitTag tag, Action<Entity> action, Vector2 at)
		{
			bool result = false;
			Vector2 position = this.Position;
			this.Position = at;
			foreach (Entity entity in this.Scene[tag])
			{
				if (this.CollideCheck(entity))
				{
					action(entity);
					result = true;
				}
			}
			this.Position = position;
			return result;
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x0000E8E8 File Offset: 0x0000CAE8
		public bool CollideDo<T>(Action<T> action) where T : Entity
		{
			bool result = false;
			foreach (Entity entity in this.Scene.Tracker.Entities[typeof(T)])
			{
				if (this.CollideCheck(entity))
				{
					action(entity as T);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x0000E96C File Offset: 0x0000CB6C
		public bool CollideDo<T>(Action<T> action, Vector2 at) where T : Entity
		{
			bool result = false;
			Vector2 position = this.Position;
			this.Position = at;
			foreach (Entity entity in this.Scene.Tracker.Entities[typeof(T)])
			{
				if (this.CollideCheck(entity))
				{
					action(entity as T);
					result = true;
				}
			}
			this.Position = position;
			return result;
		}

		// Token: 0x060007C4 RID: 1988 RVA: 0x0000EA08 File Offset: 0x0000CC08
		public bool CollideDoByComponent<T>(Action<T> action) where T : Component
		{
			bool result = false;
			foreach (Component component in this.Scene.Tracker.Components[typeof(T)])
			{
				if (this.CollideCheck(component.Entity))
				{
					action(component as T);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x0000EA94 File Offset: 0x0000CC94
		public bool CollideDoByComponent<T>(Action<T> action, Vector2 at) where T : Component
		{
			bool result = false;
			Vector2 position = this.Position;
			this.Position = at;
			foreach (Component component in this.Scene.Tracker.Components[typeof(T)])
			{
				if (this.CollideCheck(component.Entity))
				{
					action(component as T);
					result = true;
				}
			}
			this.Position = position;
			return result;
		}

		// Token: 0x060007C6 RID: 1990 RVA: 0x0000EB34 File Offset: 0x0000CD34
		public bool CollidePoint(Vector2 point)
		{
			return Collide.CheckPoint(this, point);
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x0000EB3D File Offset: 0x0000CD3D
		public bool CollidePoint(Vector2 point, Vector2 at)
		{
			return Collide.CheckPoint(this, point, at);
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x0000EB47 File Offset: 0x0000CD47
		public bool CollideLine(Vector2 from, Vector2 to)
		{
			return Collide.CheckLine(this, from, to);
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x0000EB51 File Offset: 0x0000CD51
		public bool CollideLine(Vector2 from, Vector2 to, Vector2 at)
		{
			return Collide.CheckLine(this, from, to, at);
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x0000EB5C File Offset: 0x0000CD5C
		public bool CollideRect(Rectangle rect)
		{
			return Collide.CheckRect(this, rect);
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x0000EB65 File Offset: 0x0000CD65
		public bool CollideRect(Rectangle rect, Vector2 at)
		{
			return Collide.CheckRect(this, rect, at);
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x0000EB6F File Offset: 0x0000CD6F
		public void Add(Component component)
		{
			this.Components.Add(component);
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x0000EB7D File Offset: 0x0000CD7D
		public void Remove(Component component)
		{
			this.Components.Remove(component);
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x0000EB8B File Offset: 0x0000CD8B
		public void Add(params Component[] components)
		{
			this.Components.Add(components);
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x0000EB99 File Offset: 0x0000CD99
		public void Remove(params Component[] components)
		{
			this.Components.Remove(components);
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x0000EBA7 File Offset: 0x0000CDA7
		public T Get<T>() where T : Component
		{
			return this.Components.Get<T>();
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x0000EBB4 File Offset: 0x0000CDB4
		public IEnumerator<Component> GetEnumerator()
		{
			return this.Components.GetEnumerator();
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x0000EBC1 File Offset: 0x0000CDC1
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x0000EBCC File Offset: 0x0000CDCC
		public Entity Closest(params Entity[] entities)
		{
			Entity entity = entities[0];
			float num = Vector2.DistanceSquared(this.Position, entity.Position);
			for (int i = 1; i < entities.Length; i++)
			{
				float num2 = Vector2.DistanceSquared(this.Position, entities[i].Position);
				if (num2 < num)
				{
					entity = entities[i];
					num = num2;
				}
			}
			return entity;
		}

		// Token: 0x060007D4 RID: 2004 RVA: 0x0000EC1C File Offset: 0x0000CE1C
		public Entity Closest(BitTag tag)
		{
			List<Entity> list = this.Scene[tag];
			Entity entity = null;
			if (list.Count >= 1)
			{
				entity = list[0];
				float num = Vector2.DistanceSquared(this.Position, entity.Position);
				for (int i = 1; i < list.Count; i++)
				{
					float num2 = Vector2.DistanceSquared(this.Position, list[i].Position);
					if (num2 < num)
					{
						entity = list[i];
						num = num2;
					}
				}
			}
			return entity;
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x0000EC96 File Offset: 0x0000CE96
		public T SceneAs<T>() where T : Scene
		{
			return this.Scene as T;
		}

		// Token: 0x04000563 RID: 1379
		public bool Active = true;

		// Token: 0x04000564 RID: 1380
		public bool Visible = true;

		// Token: 0x04000565 RID: 1381
		public bool Collidable = true;

		// Token: 0x04000566 RID: 1382
		public Vector2 Position;

		// Token: 0x04000569 RID: 1385
		private int tag;

		// Token: 0x0400056A RID: 1386
		private Collider collider;

		// Token: 0x0400056B RID: 1387
		internal int depth;

		// Token: 0x0400056C RID: 1388
		internal double actualDepth;
	}
}
