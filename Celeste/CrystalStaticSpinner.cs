using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200019B RID: 411
	[Tracked(false)]
	public class CrystalStaticSpinner : Entity
	{
		// Token: 0x06000E39 RID: 3641 RVA: 0x000332E8 File Offset: 0x000314E8
		public CrystalStaticSpinner(Vector2 position, bool attachToSolid, CrystalColor color) : base(position)
		{
			this.color = color;
			base.Tag = Tags.TransitionUpdate;
			base.Collider = new ColliderList(new Collider[]
			{
				new Circle(6f, 0f, 0f),
				new Hitbox(16f, 4f, -8f, -3f)
			});
			this.Visible = false;
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(new HoldableCollider(new Action<Holdable>(this.OnHoldable), null));
			base.Add(new LedgeBlocker(null));
			base.Depth = -8500;
			this.AttachToSolid = attachToSolid;
			if (attachToSolid)
			{
				base.Add(new StaticMover
				{
					OnShake = new Action<Vector2>(this.OnShake),
					SolidChecker = new Func<Solid, bool>(this.IsRiding),
					OnDestroy = new Action(base.RemoveSelf)
				});
			}
			this.randomSeed = Calc.Random.Next();
		}

		// Token: 0x06000E3A RID: 3642 RVA: 0x00033413 File Offset: 0x00031613
		public CrystalStaticSpinner(EntityData data, Vector2 offset, CrystalColor color) : this(data.Position + offset, data.Bool("attachToSolid", false), color)
		{
		}

		// Token: 0x06000E3B RID: 3643 RVA: 0x00033434 File Offset: 0x00031634
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if ((scene as Level).Session.Area.ID == 9)
			{
				base.Add(new CrystalStaticSpinner.CoreModeListener(this));
				if ((scene as Level).CoreMode == Session.CoreModes.Cold)
				{
					this.color = CrystalColor.Blue;
				}
				else
				{
					this.color = CrystalColor.Red;
				}
			}
			if (this.InView())
			{
				this.CreateSprites();
			}
		}

		// Token: 0x06000E3C RID: 3644 RVA: 0x00033499 File Offset: 0x00031699
		public void ForceInstantiate()
		{
			this.CreateSprites();
			this.Visible = true;
		}

		// Token: 0x06000E3D RID: 3645 RVA: 0x000334A8 File Offset: 0x000316A8
		public override void Update()
		{
			if (!this.Visible)
			{
				this.Collidable = false;
				if (this.InView())
				{
					this.Visible = true;
					if (!this.expanded)
					{
						this.CreateSprites();
					}
					if (this.color == CrystalColor.Rainbow)
					{
						this.UpdateHue();
					}
				}
			}
			else
			{
				base.Update();
				if (this.color == CrystalColor.Rainbow && base.Scene.OnInterval(0.08f, this.offset))
				{
					this.UpdateHue();
				}
				if (base.Scene.OnInterval(0.25f, this.offset) && !this.InView())
				{
					this.Visible = false;
				}
				if (base.Scene.OnInterval(0.05f, this.offset))
				{
					Player entity = base.Scene.Tracker.GetEntity<Player>();
					if (entity != null)
					{
						this.Collidable = (Math.Abs(entity.X - base.X) < 128f && Math.Abs(entity.Y - base.Y) < 128f);
					}
				}
			}
			if (this.filler != null)
			{
				this.filler.Position = this.Position;
			}
		}

		// Token: 0x06000E3E RID: 3646 RVA: 0x000335D0 File Offset: 0x000317D0
		private void UpdateHue()
		{
			foreach (Component component in base.Components)
			{
				Image image = component as Image;
				if (image != null)
				{
					image.Color = this.GetHue(this.Position + image.Position);
				}
			}
			if (this.filler != null)
			{
				foreach (Component component2 in this.filler.Components)
				{
					Image image2 = component2 as Image;
					if (image2 != null)
					{
						image2.Color = this.GetHue(this.Position + image2.Position);
					}
				}
			}
		}

		// Token: 0x06000E3F RID: 3647 RVA: 0x000336A4 File Offset: 0x000318A4
		private bool InView()
		{
			Camera camera = (base.Scene as Level).Camera;
			return base.X > camera.X - 16f && base.Y > camera.Y - 16f && base.X < camera.X + 320f + 16f && base.Y < camera.Y + 180f + 16f;
		}

		// Token: 0x06000E40 RID: 3648 RVA: 0x00033720 File Offset: 0x00031920
		private void CreateSprites()
		{
			if (!this.expanded)
			{
				Calc.PushRandom(this.randomSeed);
				List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(CrystalStaticSpinner.fgTextureLookup[this.color]);
				MTexture mtexture = Calc.Random.Choose(atlasSubtextures);
				Color color = Color.White;
				if (this.color == CrystalColor.Rainbow)
				{
					color = this.GetHue(this.Position);
				}
				if (!this.SolidCheck(new Vector2(base.X - 4f, base.Y - 4f)))
				{
					base.Add(new Image(mtexture.GetSubtexture(0, 0, 14, 14, null)).SetOrigin(12f, 12f).SetColor(color));
				}
				if (!this.SolidCheck(new Vector2(base.X + 4f, base.Y - 4f)))
				{
					base.Add(new Image(mtexture.GetSubtexture(10, 0, 14, 14, null)).SetOrigin(2f, 12f).SetColor(color));
				}
				if (!this.SolidCheck(new Vector2(base.X + 4f, base.Y + 4f)))
				{
					base.Add(new Image(mtexture.GetSubtexture(10, 10, 14, 14, null)).SetOrigin(2f, 2f).SetColor(color));
				}
				if (!this.SolidCheck(new Vector2(base.X - 4f, base.Y + 4f)))
				{
					base.Add(new Image(mtexture.GetSubtexture(0, 10, 14, 14, null)).SetOrigin(12f, 2f).SetColor(color));
				}
				foreach (Entity entity in base.Scene.Tracker.GetEntities<CrystalStaticSpinner>())
				{
					CrystalStaticSpinner crystalStaticSpinner = (CrystalStaticSpinner)entity;
					if (crystalStaticSpinner != this && crystalStaticSpinner.AttachToSolid == this.AttachToSolid && crystalStaticSpinner.X >= base.X && (crystalStaticSpinner.Position - this.Position).Length() < 24f)
					{
						this.AddSprite((this.Position + crystalStaticSpinner.Position) / 2f - this.Position);
					}
				}
				base.Scene.Add(this.border = new CrystalStaticSpinner.Border(this, this.filler));
				this.expanded = true;
				Calc.PopRandom();
			}
		}

		// Token: 0x06000E41 RID: 3649 RVA: 0x000339C8 File Offset: 0x00031BC8
		private void AddSprite(Vector2 offset)
		{
			if (this.filler == null)
			{
				base.Scene.Add(this.filler = new Entity(this.Position));
				this.filler.Depth = base.Depth + 1;
			}
			List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(CrystalStaticSpinner.bgTextureLookup[this.color]);
			Image image = new Image(Calc.Random.Choose(atlasSubtextures));
			image.Position = offset;
			image.Rotation = (float)Calc.Random.Choose(0, 1, 2, 3) * 1.5707964f;
			image.CenterOrigin();
			if (this.color == CrystalColor.Rainbow)
			{
				image.Color = this.GetHue(this.Position + offset);
			}
			this.filler.Add(image);
		}

		// Token: 0x06000E42 RID: 3650 RVA: 0x00033A94 File Offset: 0x00031C94
		private bool SolidCheck(Vector2 position)
		{
			if (this.AttachToSolid)
			{
				return false;
			}
			using (List<Solid>.Enumerator enumerator = base.Scene.CollideAll<Solid>(position).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is SolidTiles)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000E43 RID: 3651 RVA: 0x00033B00 File Offset: 0x00031D00
		private void ClearSprites()
		{
			if (this.filler != null)
			{
				this.filler.RemoveSelf();
			}
			this.filler = null;
			if (this.border != null)
			{
				this.border.RemoveSelf();
			}
			this.border = null;
			foreach (Image image in base.Components.GetAll<Image>())
			{
				image.RemoveSelf();
			}
			this.expanded = false;
		}

		// Token: 0x06000E44 RID: 3652 RVA: 0x00033B8C File Offset: 0x00031D8C
		private void OnShake(Vector2 pos)
		{
			foreach (Component component in base.Components)
			{
				if (component is Image)
				{
					(component as Image).Position = pos;
				}
			}
		}

		// Token: 0x06000E45 RID: 3653 RVA: 0x00033BE8 File Offset: 0x00031DE8
		private bool IsRiding(Solid solid)
		{
			return base.CollideCheck(solid);
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x00033BF1 File Offset: 0x00031DF1
		private void OnPlayer(Player player)
		{
			player.Die((player.Position - this.Position).SafeNormalize(), false, true);
		}

		// Token: 0x06000E47 RID: 3655 RVA: 0x00033C12 File Offset: 0x00031E12
		private void OnHoldable(Holdable h)
		{
			h.HitSpinner(this);
		}

		// Token: 0x06000E48 RID: 3656 RVA: 0x00033C1C File Offset: 0x00031E1C
		public override void Removed(Scene scene)
		{
			if (this.filler != null && this.filler.Scene == scene)
			{
				this.filler.RemoveSelf();
			}
			if (this.border != null && this.border.Scene == scene)
			{
				this.border.RemoveSelf();
			}
			base.Removed(scene);
		}

		// Token: 0x06000E49 RID: 3657 RVA: 0x00033C74 File Offset: 0x00031E74
		public void Destroy(bool boss = false)
		{
			if (this.InView())
			{
				Audio.Play("event:/game/06_reflection/fall_spike_smash", this.Position);
				Color color = Color.White;
				if (this.color == CrystalColor.Red)
				{
					color = Calc.HexToColor("ff4f4f");
				}
				else if (this.color == CrystalColor.Blue)
				{
					color = Calc.HexToColor("639bff");
				}
				else if (this.color == CrystalColor.Purple)
				{
					color = Calc.HexToColor("ff4fef");
				}
				CrystalDebris.Burst(this.Position, color, boss, 8);
			}
			base.RemoveSelf();
		}

		// Token: 0x06000E4A RID: 3658 RVA: 0x00033CF4 File Offset: 0x00031EF4
		private Color GetHue(Vector2 position)
		{
			float num = 280f;
			float value = (position.Length() + base.Scene.TimeActive * 50f) % num / num;
			return Calc.HsvToColor(0.4f + Calc.YoYo(value) * 0.4f, 0.4f, 0.9f);
		}

		// Token: 0x04000972 RID: 2418
		public static ParticleType P_Move;

		// Token: 0x04000973 RID: 2419
		public const float ParticleInterval = 0.02f;

		// Token: 0x04000974 RID: 2420
		private static Dictionary<CrystalColor, string> fgTextureLookup = new Dictionary<CrystalColor, string>
		{
			{
				CrystalColor.Blue,
				"danger/crystal/fg_blue"
			},
			{
				CrystalColor.Red,
				"danger/crystal/fg_red"
			},
			{
				CrystalColor.Purple,
				"danger/crystal/fg_purple"
			},
			{
				CrystalColor.Rainbow,
				"danger/crystal/fg_white"
			}
		};

		// Token: 0x04000975 RID: 2421
		private static Dictionary<CrystalColor, string> bgTextureLookup = new Dictionary<CrystalColor, string>
		{
			{
				CrystalColor.Blue,
				"danger/crystal/bg_blue"
			},
			{
				CrystalColor.Red,
				"danger/crystal/bg_red"
			},
			{
				CrystalColor.Purple,
				"danger/crystal/bg_purple"
			},
			{
				CrystalColor.Rainbow,
				"danger/crystal/bg_white"
			}
		};

		// Token: 0x04000976 RID: 2422
		public bool AttachToSolid;

		// Token: 0x04000977 RID: 2423
		private Entity filler;

		// Token: 0x04000978 RID: 2424
		private CrystalStaticSpinner.Border border;

		// Token: 0x04000979 RID: 2425
		private float offset = Calc.Random.NextFloat();

		// Token: 0x0400097A RID: 2426
		private bool expanded;

		// Token: 0x0400097B RID: 2427
		private int randomSeed;

		// Token: 0x0400097C RID: 2428
		private CrystalColor color;

		// Token: 0x02000499 RID: 1177
		private class CoreModeListener : Component
		{
			// Token: 0x06002339 RID: 9017 RVA: 0x000EDB2C File Offset: 0x000EBD2C
			public CoreModeListener(CrystalStaticSpinner parent) : base(true, false)
			{
				this.Parent = parent;
			}

			// Token: 0x0600233A RID: 9018 RVA: 0x000EDB40 File Offset: 0x000EBD40
			public override void Update()
			{
				Level level = base.Scene as Level;
				if ((this.Parent.color == CrystalColor.Blue && level.CoreMode == Session.CoreModes.Hot) || (this.Parent.color == CrystalColor.Red && level.CoreMode == Session.CoreModes.Cold))
				{
					if (this.Parent.color == CrystalColor.Blue)
					{
						this.Parent.color = CrystalColor.Red;
					}
					else
					{
						this.Parent.color = CrystalColor.Blue;
					}
					this.Parent.ClearSprites();
					this.Parent.CreateSprites();
				}
			}

			// Token: 0x040022D2 RID: 8914
			public CrystalStaticSpinner Parent;
		}

		// Token: 0x0200049A RID: 1178
		private class Border : Entity
		{
			// Token: 0x0600233B RID: 9019 RVA: 0x000EDBC3 File Offset: 0x000EBDC3
			public Border(Entity parent, Entity filler)
			{
				this.drawing[0] = parent;
				this.drawing[1] = filler;
				base.Depth = parent.Depth + 2;
			}

			// Token: 0x0600233C RID: 9020 RVA: 0x000EDBF7 File Offset: 0x000EBDF7
			public override void Render()
			{
				if (!this.drawing[0].Visible)
				{
					return;
				}
				this.DrawBorder(this.drawing[0]);
				this.DrawBorder(this.drawing[1]);
			}

			// Token: 0x0600233D RID: 9021 RVA: 0x000EDC28 File Offset: 0x000EBE28
			private void DrawBorder(Entity entity)
			{
				if (entity == null)
				{
					return;
				}
				foreach (Component component in entity.Components)
				{
					Image image = component as Image;
					if (image != null)
					{
						Color color = image.Color;
						Vector2 position = image.Position;
						image.Color = Color.Black;
						image.Position = position + new Vector2(0f, -1f);
						image.Render();
						image.Position = position + new Vector2(0f, 1f);
						image.Render();
						image.Position = position + new Vector2(-1f, 0f);
						image.Render();
						image.Position = position + new Vector2(1f, 0f);
						image.Render();
						image.Color = color;
						image.Position = position;
					}
				}
			}

			// Token: 0x040022D3 RID: 8915
			private Entity[] drawing = new Entity[2];
		}
	}
}
