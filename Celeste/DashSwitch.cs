using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002C4 RID: 708
	public class DashSwitch : Solid
	{
		// Token: 0x060015F3 RID: 5619 RVA: 0x0007F648 File Offset: 0x0007D848
		public DashSwitch(Vector2 position, DashSwitch.Sides side, bool persistent, bool allGates, EntityID id, string spriteName) : base(position, 0f, 0f, true)
		{
			this.side = side;
			this.persistent = persistent;
			this.allGates = allGates;
			this.id = id;
			this.mirrorMode = (spriteName != "default");
			base.Add(this.sprite = GFX.SpriteBank.Create("dashSwitch_" + spriteName));
			this.sprite.Play("idle", false, false);
			if (side == DashSwitch.Sides.Up || side == DashSwitch.Sides.Down)
			{
				base.Collider.Width = 16f;
				base.Collider.Height = 8f;
			}
			else
			{
				base.Collider.Width = 8f;
				base.Collider.Height = 16f;
			}
			switch (side)
			{
			case DashSwitch.Sides.Up:
				this.sprite.Position = new Vector2(8f, 0f);
				this.sprite.Rotation = -1.5707964f;
				this.pressedTarget = this.Position + Vector2.UnitY * -8f;
				this.pressDirection = -Vector2.UnitY;
				break;
			case DashSwitch.Sides.Down:
				this.sprite.Position = new Vector2(8f, 8f);
				this.sprite.Rotation = 1.5707964f;
				this.pressedTarget = this.Position + Vector2.UnitY * 8f;
				this.pressDirection = Vector2.UnitY;
				this.startY = base.Y;
				break;
			case DashSwitch.Sides.Left:
				this.sprite.Position = new Vector2(0f, 8f);
				this.sprite.Rotation = 3.1415927f;
				this.pressedTarget = this.Position + Vector2.UnitX * -8f;
				this.pressDirection = -Vector2.UnitX;
				break;
			case DashSwitch.Sides.Right:
				this.sprite.Position = new Vector2(8f, 8f);
				this.sprite.Rotation = 0f;
				this.pressedTarget = this.Position + Vector2.UnitX * 8f;
				this.pressDirection = Vector2.UnitX;
				break;
			}
			this.OnDashCollide = new DashCollision(this.OnDashed);
		}

		// Token: 0x060015F4 RID: 5620 RVA: 0x0007F8BC File Offset: 0x0007DABC
		public static DashSwitch Create(EntityData data, Vector2 offset, EntityID id)
		{
			Vector2 position = data.Position + offset;
			bool flag = data.Bool("persistent", false);
			bool flag2 = data.Bool("allGates", false);
			string spriteName = data.Attr("sprite", "default");
			if (data.Name.Equals("dashSwitchH"))
			{
				if (data.Bool("leftSide", false))
				{
					return new DashSwitch(position, DashSwitch.Sides.Left, flag, flag2, id, spriteName);
				}
				return new DashSwitch(position, DashSwitch.Sides.Right, flag, flag2, id, spriteName);
			}
			else
			{
				if (data.Bool("ceiling", false))
				{
					return new DashSwitch(position, DashSwitch.Sides.Up, flag, flag2, id, spriteName);
				}
				return new DashSwitch(position, DashSwitch.Sides.Down, flag, flag2, id, spriteName);
			}
		}

		// Token: 0x060015F5 RID: 5621 RVA: 0x0007F960 File Offset: 0x0007DB60
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (this.persistent && base.SceneAs<Level>().Session.GetFlag(this.FlagName))
			{
				this.sprite.Play("pushed", false, false);
				this.Position = this.pressedTarget - this.pressDirection * 2f;
				this.pressed = true;
				this.Collidable = false;
				if (this.allGates)
				{
					using (List<Entity>.Enumerator enumerator = base.Scene.Tracker.GetEntities<TempleGate>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Entity entity = enumerator.Current;
							TempleGate templeGate = (TempleGate)entity;
							if (templeGate.Type == TempleGate.Types.NearestSwitch && templeGate.LevelID == this.id.Level)
							{
								templeGate.StartOpen();
							}
						}
						return;
					}
				}
				TempleGate gate = this.GetGate();
				if (gate != null)
				{
					gate.StartOpen();
				}
			}
		}

		// Token: 0x060015F6 RID: 5622 RVA: 0x0007FA68 File Offset: 0x0007DC68
		public override void Update()
		{
			base.Update();
			if (!this.pressed && this.side == DashSwitch.Sides.Down)
			{
				Player playerOnTop = base.GetPlayerOnTop();
				if (playerOnTop != null)
				{
					if (playerOnTop.Holding != null)
					{
						this.OnDashed(playerOnTop, Vector2.UnitY);
					}
					else
					{
						if (this.speedY < 0f)
						{
							this.speedY = 0f;
						}
						this.speedY = Calc.Approach(this.speedY, 70f, 200f * Engine.DeltaTime);
						base.MoveTowardsY(this.startY + 2f, this.speedY * Engine.DeltaTime);
						if (!this.playerWasOn)
						{
							Audio.Play("event:/game/05_mirror_temple/button_depress", this.Position);
						}
					}
				}
				else
				{
					if (this.speedY > 0f)
					{
						this.speedY = 0f;
					}
					this.speedY = Calc.Approach(this.speedY, -150f, 200f * Engine.DeltaTime);
					base.MoveTowardsY(this.startY, -this.speedY * Engine.DeltaTime);
					if (this.playerWasOn)
					{
						Audio.Play("event:/game/05_mirror_temple/button_return", this.Position);
					}
				}
				this.playerWasOn = (playerOnTop != null);
			}
		}

		// Token: 0x060015F7 RID: 5623 RVA: 0x0007FBA0 File Offset: 0x0007DDA0
		public DashCollisionResults OnDashed(Player player, Vector2 direction)
		{
			if (!this.pressed && direction == this.pressDirection)
			{
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
				Audio.Play("event:/game/05_mirror_temple/button_activate", this.Position);
				this.sprite.Play("push", false, false);
				this.pressed = true;
				base.MoveTo(this.pressedTarget);
				this.Collidable = false;
				this.Position -= this.pressDirection * 2f;
				base.SceneAs<Level>().ParticlesFG.Emit(this.mirrorMode ? DashSwitch.P_PressAMirror : DashSwitch.P_PressA, 10, this.Position + this.sprite.Position, direction.Perpendicular() * 6f, this.sprite.Rotation - 3.1415927f);
				base.SceneAs<Level>().ParticlesFG.Emit(this.mirrorMode ? DashSwitch.P_PressBMirror : DashSwitch.P_PressB, 4, this.Position + this.sprite.Position, direction.Perpendicular() * 6f, this.sprite.Rotation - 3.1415927f);
				if (this.allGates)
				{
					using (List<Entity>.Enumerator enumerator = base.Scene.Tracker.GetEntities<TempleGate>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Entity entity = enumerator.Current;
							TempleGate templeGate = (TempleGate)entity;
							if (templeGate.Type == TempleGate.Types.NearestSwitch && templeGate.LevelID == this.id.Level)
							{
								templeGate.SwitchOpen();
							}
						}
						goto IL_1B6;
					}
				}
				TempleGate gate = this.GetGate();
				if (gate != null)
				{
					gate.SwitchOpen();
				}
				IL_1B6:
				TempleMirrorPortal templeMirrorPortal = base.Scene.Entities.FindFirst<TempleMirrorPortal>();
				if (templeMirrorPortal != null)
				{
					templeMirrorPortal.OnSwitchHit(Math.Sign(base.X - (float)(base.Scene as Level).Bounds.Center.X));
				}
				if (this.persistent)
				{
					base.SceneAs<Level>().Session.SetFlag(this.FlagName, true);
				}
			}
			return DashCollisionResults.NormalCollision;
		}

		// Token: 0x060015F8 RID: 5624 RVA: 0x0007FDD8 File Offset: 0x0007DFD8
		private TempleGate GetGate()
		{
			List<Entity> entities = base.Scene.Tracker.GetEntities<TempleGate>();
			TempleGate templeGate = null;
			float num = 0f;
			foreach (Entity entity in entities)
			{
				TempleGate templeGate2 = (TempleGate)entity;
				if (templeGate2.Type == TempleGate.Types.NearestSwitch && !templeGate2.ClaimedByASwitch && templeGate2.LevelID == this.id.Level)
				{
					float num2 = Vector2.DistanceSquared(this.Position, templeGate2.Position);
					if (templeGate == null || num2 < num)
					{
						templeGate = templeGate2;
						num = num2;
					}
				}
			}
			if (templeGate != null)
			{
				templeGate.ClaimedByASwitch = true;
			}
			return templeGate;
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x060015F9 RID: 5625 RVA: 0x0007FE90 File Offset: 0x0007E090
		private string FlagName
		{
			get
			{
				return DashSwitch.GetFlagName(this.id);
			}
		}

		// Token: 0x060015FA RID: 5626 RVA: 0x0007FE9D File Offset: 0x0007E09D
		public static string GetFlagName(EntityID id)
		{
			return "dashSwitch_" + id.Key;
		}

		// Token: 0x04001231 RID: 4657
		public static ParticleType P_PressA;

		// Token: 0x04001232 RID: 4658
		public static ParticleType P_PressB;

		// Token: 0x04001233 RID: 4659
		public static ParticleType P_PressAMirror;

		// Token: 0x04001234 RID: 4660
		public static ParticleType P_PressBMirror;

		// Token: 0x04001235 RID: 4661
		private DashSwitch.Sides side;

		// Token: 0x04001236 RID: 4662
		private Vector2 pressedTarget;

		// Token: 0x04001237 RID: 4663
		private bool pressed;

		// Token: 0x04001238 RID: 4664
		private Vector2 pressDirection;

		// Token: 0x04001239 RID: 4665
		private float speedY;

		// Token: 0x0400123A RID: 4666
		private float startY;

		// Token: 0x0400123B RID: 4667
		private bool persistent;

		// Token: 0x0400123C RID: 4668
		private EntityID id;

		// Token: 0x0400123D RID: 4669
		private bool mirrorMode;

		// Token: 0x0400123E RID: 4670
		private bool playerWasOn;

		// Token: 0x0400123F RID: 4671
		private bool allGates;

		// Token: 0x04001240 RID: 4672
		private Sprite sprite;

		// Token: 0x02000655 RID: 1621
		public enum Sides
		{
			// Token: 0x04002A1C RID: 10780
			Up,
			// Token: 0x04002A1D RID: 10781
			Down,
			// Token: 0x04002A1E RID: 10782
			Left,
			// Token: 0x04002A1F RID: 10783
			Right
		}
	}
}
