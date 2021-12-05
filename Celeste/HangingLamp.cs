using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000327 RID: 807
	public class HangingLamp : Entity
	{
		// Token: 0x06001966 RID: 6502 RVA: 0x000A31B0 File Offset: 0x000A13B0
		public HangingLamp(Vector2 position, int length)
		{
			this.Position = position + Vector2.UnitX * 4f;
			this.Length = Math.Max(16, length);
			base.Depth = 2000;
			MTexture mtexture = GFX.Game["objects/hanginglamp"];
			Image image;
			for (int i = 0; i < this.Length - 8; i += 8)
			{
				base.Add(image = new Image(mtexture.GetSubtexture(0, 8, 8, 8, null)));
				image.Origin.X = 4f;
				image.Origin.Y = (float)(-(float)i);
				this.images.Add(image);
			}
			base.Add(image = new Image(mtexture.GetSubtexture(0, 0, 8, 8, null)));
			image.Origin.X = 4f;
			base.Add(image = new Image(mtexture.GetSubtexture(0, 16, 8, 8, null)));
			image.Origin.X = 4f;
			image.Origin.Y = (float)(-(float)(this.Length - 8));
			this.images.Add(image);
			base.Add(this.bloom = new BloomPoint(Vector2.UnitY * (float)(this.Length - 4), 1f, 48f));
			base.Add(this.light = new VertexLight(Vector2.UnitY * (float)(this.Length - 4), Color.White, 1f, 24, 48));
			base.Add(this.sfx = new SoundSource());
			base.Collider = new Hitbox(8f, (float)this.Length, -4f, 0f);
		}

		// Token: 0x06001967 RID: 6503 RVA: 0x000A337A File Offset: 0x000A157A
		public HangingLamp(EntityData e, Vector2 position) : this(position, Math.Max(16, e.Height))
		{
		}

		// Token: 0x06001968 RID: 6504 RVA: 0x000A3390 File Offset: 0x000A1590
		public override void Update()
		{
			base.Update();
			this.soundDelay -= Engine.DeltaTime;
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null && base.Collider.Collide(entity))
			{
				this.speed = -entity.Speed.X * 0.005f * ((entity.Y - base.Y) / (float)this.Length);
				if (Math.Abs(this.speed) < 0.1f)
				{
					this.speed = 0f;
				}
				else if (this.soundDelay <= 0f)
				{
					this.sfx.Play("event:/game/02_old_site/lantern_hit", null, 0f);
					this.soundDelay = 0.25f;
				}
			}
			float num = (Math.Sign(this.rotation) == Math.Sign(this.speed)) ? 8f : 6f;
			if (Math.Abs(this.rotation) < 0.5f)
			{
				num *= 0.5f;
			}
			if (Math.Abs(this.rotation) < 0.25f)
			{
				num *= 0.5f;
			}
			float value = this.rotation;
			this.speed += (float)(-(float)Math.Sign(this.rotation)) * num * Engine.DeltaTime;
			this.rotation += this.speed * Engine.DeltaTime;
			this.rotation = Calc.Clamp(this.rotation, -0.4f, 0.4f);
			if (Math.Abs(this.rotation) < 0.02f && Math.Abs(this.speed) < 0.2f)
			{
				this.rotation = (this.speed = 0f);
			}
			else if (Math.Sign(this.rotation) != Math.Sign(value) && this.soundDelay <= 0f && Math.Abs(this.speed) > 0.5f)
			{
				this.sfx.Play("event:/game/02_old_site/lantern_hit", null, 0f);
				this.soundDelay = 0.25f;
			}
			foreach (Image image in this.images)
			{
				image.Rotation = this.rotation;
			}
			Vector2 position = Calc.AngleToVector(this.rotation + 1.5707964f, (float)this.Length - 4f);
			this.bloom.Position = (this.light.Position = position);
			this.sfx.Position = position;
		}

		// Token: 0x06001969 RID: 6505 RVA: 0x000A3630 File Offset: 0x000A1830
		public override void Render()
		{
			foreach (Component component in base.Components)
			{
				Image image = component as Image;
				if (image != null)
				{
					image.DrawOutline(1);
				}
			}
			base.Render();
		}

		// Token: 0x04001621 RID: 5665
		public readonly int Length;

		// Token: 0x04001622 RID: 5666
		private List<Image> images = new List<Image>();

		// Token: 0x04001623 RID: 5667
		private BloomPoint bloom;

		// Token: 0x04001624 RID: 5668
		private VertexLight light;

		// Token: 0x04001625 RID: 5669
		private float speed;

		// Token: 0x04001626 RID: 5670
		private float rotation;

		// Token: 0x04001627 RID: 5671
		private float soundDelay;

		// Token: 0x04001628 RID: 5672
		private SoundSource sfx;
	}
}
