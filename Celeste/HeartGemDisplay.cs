using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002EE RID: 750
	public class HeartGemDisplay : Component
	{
		// Token: 0x06001738 RID: 5944 RVA: 0x0008CC2C File Offset: 0x0008AE2C
		public HeartGemDisplay(int heartgem, bool hasGem) : base(true, true)
		{
			this.Sprites = new Sprite[3];
			for (int i = 0; i < this.Sprites.Length; i++)
			{
				this.Sprites[i] = GFX.GuiSpriteBank.Create("heartgem" + i);
				this.Sprites[i].Visible = (heartgem == i && hasGem);
				this.Sprites[i].Play("spin", false, false);
			}
			this.bg = new Image(GFX.Gui["collectables/heartgem/0/spin00"]);
			this.bg.Color = Color.Black;
			this.bg.CenterOrigin();
			this.rotateWiggler = Wiggler.Create(0.4f, 6f, null, false, false);
			this.rotateWiggler.UseRawDeltaTime = true;
			SimpleCurve curve = new SimpleCurve(Vector2.UnitY * 80f, Vector2.Zero, Vector2.UnitY * -160f);
			this.tween = Tween.Create(Tween.TweenMode.Oneshot, null, 0.4f, false);
			this.tween.OnStart = delegate(Tween t)
			{
				this.SpriteColor = Color.Transparent;
			};
			this.tween.OnUpdate = delegate(Tween t)
			{
				this.bounce = curve.GetPoint(t.Eased);
				this.SpriteColor = Color.White * Calc.LerpClamp(0f, 1f, t.Percent * 1.5f);
			};
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06001739 RID: 5945 RVA: 0x0008CD7F File Offset: 0x0008AF7F
		// (set) Token: 0x0600173A RID: 5946 RVA: 0x0008CD90 File Offset: 0x0008AF90
		private Color SpriteColor
		{
			get
			{
				return this.Sprites[0].Color;
			}
			set
			{
				for (int i = 0; i < this.Sprites.Length; i++)
				{
					this.Sprites[i].Color = value;
				}
			}
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x0008CDC0 File Offset: 0x0008AFC0
		public void Wiggle()
		{
			this.rotateWiggler.Start();
			for (int i = 0; i < this.Sprites.Length; i++)
			{
				if (this.Sprites[i].Visible)
				{
					this.Sprites[i].Play("spin", true, false);
					this.Sprites[i].SetAnimationFrame(19);
				}
			}
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x0008CE1D File Offset: 0x0008B01D
		public void Appear(AreaMode mode)
		{
			this.tween.Start();
			this.routine = new Coroutine(this.AppearSequence(this.Sprites[(int)mode]), true);
			this.routine.UseRawDeltaTime = true;
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x0008CE50 File Offset: 0x0008B050
		public void SetCurrentMode(AreaMode mode, bool has)
		{
			for (int i = 0; i < this.Sprites.Length; i++)
			{
				this.Sprites[i].Visible = (i == (int)mode && has);
			}
			if (!has)
			{
				this.routine = null;
			}
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x0008CE90 File Offset: 0x0008B090
		public override void Update()
		{
			base.Update();
			if (this.routine != null && this.routine.Active)
			{
				this.routine.Update();
			}
			if (this.rotateWiggler.Active)
			{
				this.rotateWiggler.Update();
			}
			for (int i = 0; i < this.Sprites.Length; i++)
			{
				if (this.Sprites[i].Active)
				{
					this.Sprites[i].Update();
				}
			}
			if (this.tween != null && this.tween.Active)
			{
				this.tween.Update();
			}
			this.Position = Calc.Approach(this.Position, this.TargetPosition, 200f * Engine.DeltaTime);
			for (int j = 0; j < this.Sprites.Length; j++)
			{
				this.Sprites[j].Scale.X = Calc.Approach(this.Sprites[j].Scale.X, 1f, 2f * Engine.DeltaTime);
				this.Sprites[j].Scale.Y = Calc.Approach(this.Sprites[j].Scale.Y, 1f, 2f * Engine.DeltaTime);
			}
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x0008CFD4 File Offset: 0x0008B1D4
		public override void Render()
		{
			base.Render();
			this.bg.Position = base.Entity.Position + this.Position;
			for (int i = 0; i < this.Sprites.Length; i++)
			{
				if (this.Sprites[i].Visible)
				{
					this.Sprites[i].Rotation = this.rotateWiggler.Value * 30f * 0.017453292f;
					this.Sprites[i].Position = base.Entity.Position + this.Position + this.bounce;
					this.Sprites[i].Render();
				}
			}
		}

		// Token: 0x06001740 RID: 5952 RVA: 0x0008D089 File Offset: 0x0008B289
		private IEnumerator AppearSequence(Sprite sprite)
		{
			sprite.Play("idle", false, false);
			sprite.Visible = true;
			sprite.Scale = new Vector2(0.8f, 1.4f);
			yield return this.tween.Wait();
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			sprite.Scale = new Vector2(1.4f, 0.8f);
			yield return 0.4f;
			sprite.CenterOrigin();
			this.rotateWiggler.Start();
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			sprite.Play("spin", false, false);
			this.routine = null;
			yield break;
		}

		// Token: 0x040013DA RID: 5082
		public Vector2 Position;

		// Token: 0x040013DB RID: 5083
		public Sprite[] Sprites;

		// Token: 0x040013DC RID: 5084
		public Vector2 TargetPosition;

		// Token: 0x040013DD RID: 5085
		private Image bg;

		// Token: 0x040013DE RID: 5086
		private Wiggler rotateWiggler;

		// Token: 0x040013DF RID: 5087
		private Coroutine routine;

		// Token: 0x040013E0 RID: 5088
		private Vector2 bounce;

		// Token: 0x040013E1 RID: 5089
		private Tween tween;
	}
}
