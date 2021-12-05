using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002D4 RID: 724
	[Tracked(false)]
	public class Bonfire : Entity
	{
		// Token: 0x0600165E RID: 5726 RVA: 0x000838E0 File Offset: 0x00081AE0
		public Bonfire(Vector2 position, Bonfire.Mode mode)
		{
			base.Tag = Tags.TransitionUpdate;
			base.Depth = -5;
			base.Add(this.loopSfx = new SoundSource());
			base.Add(this.sprite = GFX.SpriteBank.Create("campfire"));
			base.Add(this.light = new VertexLight(new Vector2(0f, -6f), Color.PaleVioletRed, 1f, 32, 64));
			base.Add(this.bloom = new BloomPoint(new Vector2(0f, -6f), 1f, 32f));
			base.Add(this.wiggle = Wiggler.Create(0.2f, 4f, delegate(float f)
			{
				this.light.Alpha = (this.bloom.Alpha = Math.Min(1f, this.brightness + f * 0.25f) * this.multiplier);
			}, false, false));
			this.Position = position;
			this.mode = mode;
		}

		// Token: 0x0600165F RID: 5727 RVA: 0x000839D7 File Offset: 0x00081BD7
		public Bonfire(EntityData data, Vector2 offset) : this(data.Position + offset, data.Enum<Bonfire.Mode>("mode", Bonfire.Mode.Unlit))
		{
		}

		// Token: 0x06001660 RID: 5728 RVA: 0x000839F7 File Offset: 0x00081BF7
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.SetMode(this.mode);
		}

		// Token: 0x06001661 RID: 5729 RVA: 0x00083A0C File Offset: 0x00081C0C
		public void SetMode(Bonfire.Mode mode)
		{
			this.mode = mode;
			switch (mode)
			{
			default:
				this.sprite.Play("idle", false, false);
				this.bloom.Alpha = (this.light.Alpha = (this.brightness = 0f));
				break;
			case Bonfire.Mode.Lit:
				if (this.Activated)
				{
					Audio.Play("event:/env/local/campfire_start", this.Position);
					this.loopSfx.Play("event:/env/local/campfire_loop", null, 0f);
					this.sprite.Play(base.SceneAs<Level>().Session.Dreaming ? "startDream" : "start", false, false);
				}
				else
				{
					this.loopSfx.Play("event:/env/local/campfire_loop", null, 0f);
					this.sprite.Play(base.SceneAs<Level>().Session.Dreaming ? "burnDream" : "burn", false, false);
				}
				break;
			case Bonfire.Mode.Smoking:
				this.sprite.Play("smoking", false, false);
				break;
			}
			this.Activated = true;
		}

		// Token: 0x06001662 RID: 5730 RVA: 0x00083B2C File Offset: 0x00081D2C
		public override void Update()
		{
			if (this.mode == Bonfire.Mode.Lit)
			{
				this.multiplier = Calc.Approach(this.multiplier, 1f, Engine.DeltaTime * 2f);
				if (base.Scene.OnInterval(0.25f))
				{
					this.brightness = 0.5f + Calc.Random.NextFloat(0.5f);
					this.wiggle.Start();
				}
			}
			base.Update();
		}

		// Token: 0x040012B4 RID: 4788
		private Bonfire.Mode mode;

		// Token: 0x040012B5 RID: 4789
		private Sprite sprite;

		// Token: 0x040012B6 RID: 4790
		private VertexLight light;

		// Token: 0x040012B7 RID: 4791
		private BloomPoint bloom;

		// Token: 0x040012B8 RID: 4792
		private Wiggler wiggle;

		// Token: 0x040012B9 RID: 4793
		private float brightness;

		// Token: 0x040012BA RID: 4794
		private float multiplier;

		// Token: 0x040012BB RID: 4795
		public bool Activated;

		// Token: 0x040012BC RID: 4796
		private SoundSource loopSfx;

		// Token: 0x0200066A RID: 1642
		public enum Mode
		{
			// Token: 0x04002A88 RID: 10888
			Unlit,
			// Token: 0x04002A89 RID: 10889
			Lit,
			// Token: 0x04002A8A RID: 10890
			Smoking
		}
	}
}
