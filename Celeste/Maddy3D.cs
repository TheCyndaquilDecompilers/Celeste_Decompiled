using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000246 RID: 582
	public class Maddy3D : Entity
	{
		// Token: 0x06001262 RID: 4706 RVA: 0x000609A4 File Offset: 0x0005EBA4
		public Maddy3D(MountainRenderer renderer)
		{
			this.Renderer = renderer;
			base.Add(this.Image = new Billboard(null, Vector3.Zero, null, null, null));
			this.Image.BeforeRender = delegate()
			{
				if (this.Disabled)
				{
					this.Image.Color = Color.Transparent;
					return;
				}
				this.Image.Position = this.Position + (float)this.hideDown * Vector3.Up * (1f - Ease.CubeOut(this.alpha)) * 0.25f;
				this.Image.Scale = this.Scale + Vector2.One * this.Wiggler.Value * this.Scale * 0.2f;
				this.Image.Scale *= (this.Renderer.Model.Camera.Position - this.Position).Length() / 20f;
				this.Image.Color = Color.White * this.alpha;
			};
			base.Add(this.Wiggler = Wiggler.Create(0.5f, 3f, null, false, false));
			this.Running(renderer.Area < 7);
		}

		// Token: 0x06001263 RID: 4707 RVA: 0x00060A58 File Offset: 0x0005EC58
		public void Running(bool backpack = true)
		{
			this.running = true;
			this.Show = true;
			this.hideDown = -1;
			this.SetRunAnim();
			this.frameSpeed = 8f;
			this.frame = 0f;
			this.Image.Size = new Vector2((float)this.frames[0].ClipRect.Width, (float)this.frames[0].ClipRect.Height) / (float)this.frames[0].ClipRect.Width;
		}

		// Token: 0x06001264 RID: 4708 RVA: 0x00060AF0 File Offset: 0x0005ECF0
		public void Falling()
		{
			this.running = false;
			this.Show = true;
			this.hideDown = -1;
			this.frames = MTN.Mountain.GetAtlasSubtextures("marker/Fall");
			this.frameSpeed = 2f;
			this.frame = 0f;
			this.Image.Size = new Vector2((float)this.frames[0].ClipRect.Width, (float)this.frames[0].ClipRect.Height) / (float)this.frames[0].ClipRect.Width;
		}

		// Token: 0x06001265 RID: 4709 RVA: 0x00060B97 File Offset: 0x0005ED97
		public void Hide(bool down = true)
		{
			this.running = false;
			this.Show = false;
			this.hideDown = (down ? -1 : 1);
		}

		// Token: 0x06001266 RID: 4710 RVA: 0x00060BB4 File Offset: 0x0005EDB4
		private void SetRunAnim()
		{
			if (this.Renderer.Area < 7)
			{
				this.frames = MTN.Mountain.GetAtlasSubtextures("marker/runBackpack");
				return;
			}
			this.frames = MTN.Mountain.GetAtlasSubtextures("marker/runNoBackpack");
		}

		// Token: 0x06001267 RID: 4711 RVA: 0x00060BF0 File Offset: 0x0005EDF0
		public override void Update()
		{
			base.Update();
			if (this.running)
			{
				this.SetRunAnim();
			}
			if (this.frames != null && this.frames.Count > 0)
			{
				this.frame += Engine.DeltaTime * this.frameSpeed;
				if (this.frame >= (float)this.frames.Count)
				{
					this.frame -= (float)this.frames.Count;
				}
				this.Image.Texture = this.frames[(int)this.frame];
			}
			this.alpha = Calc.Approach(this.alpha, (float)(this.Show ? 1 : 0), Engine.DeltaTime * 4f);
		}

		// Token: 0x04000E14 RID: 3604
		public MountainRenderer Renderer;

		// Token: 0x04000E15 RID: 3605
		public Billboard Image;

		// Token: 0x04000E16 RID: 3606
		public Wiggler Wiggler;

		// Token: 0x04000E17 RID: 3607
		public Vector2 Scale = Vector2.One;

		// Token: 0x04000E18 RID: 3608
		public new Vector3 Position;

		// Token: 0x04000E19 RID: 3609
		public bool Show = true;

		// Token: 0x04000E1A RID: 3610
		public bool Disabled;

		// Token: 0x04000E1B RID: 3611
		private List<MTexture> frames;

		// Token: 0x04000E1C RID: 3612
		private float frame;

		// Token: 0x04000E1D RID: 3613
		private float frameSpeed;

		// Token: 0x04000E1E RID: 3614
		private float alpha = 1f;

		// Token: 0x04000E1F RID: 3615
		private int hideDown;

		// Token: 0x04000E20 RID: 3616
		private bool running;
	}
}
