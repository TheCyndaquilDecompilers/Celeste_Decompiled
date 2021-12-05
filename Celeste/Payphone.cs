using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002D6 RID: 726
	[Tracked(false)]
	public class Payphone : Entity
	{
		// Token: 0x06001670 RID: 5744 RVA: 0x000843B8 File Offset: 0x000825B8
		public Payphone(Vector2 pos) : base(pos)
		{
			base.Depth = 1;
			base.Add(this.Sprite = GFX.SpriteBank.Create("payphone"));
			this.Sprite.Play("idle", false, false);
			base.Add(this.Blink = new Image(GFX.Game["cutscenes/payphone/blink"]));
			this.Blink.Origin = this.Sprite.Origin;
			this.Blink.Visible = false;
			base.Add(this.light = new VertexLight(new Vector2(-6f, -45f), Color.White, 1f, 8, 96));
			this.light.Spotlight = true;
			this.light.SpotlightDirection = new Vector2(0f, 1f).Angle();
			base.Add(this.bloom = new BloomPoint(new Vector2(-6f, -45f), 0.8f, 8f));
			base.Add(this.buzzSfx = new SoundSource());
			this.buzzSfx.Play("event:/env/local/02_old_site/phone_lamp", null, 0f);
			this.buzzSfx.Param("on", 1f);
		}

		// Token: 0x06001671 RID: 5745 RVA: 0x00084520 File Offset: 0x00082720
		public override void Update()
		{
			base.Update();
			if (!this.Broken)
			{
				this.lightFlickerTimer -= Engine.DeltaTime;
				if (this.lightFlickerTimer <= 0f)
				{
					if (base.Scene.OnInterval(0.025f))
					{
						bool flag = Calc.Random.NextFloat() > 0.5f;
						this.light.Visible = flag;
						this.bloom.Visible = flag;
						this.Blink.Visible = !flag;
						this.buzzSfx.Param("on", (float)(flag ? 1 : 0));
					}
					if (this.lightFlickerTimer < -this.lightFlickerFor)
					{
						this.lightFlickerTimer = Calc.Random.Choose(0.4f, 0.6f, 0.8f, 1f);
						this.lightFlickerFor = Calc.Random.Choose(0.1f, 0.2f, 0.05f);
						this.light.Visible = true;
						this.bloom.Visible = true;
						this.Blink.Visible = false;
						this.buzzSfx.Param("on", 1f);
					}
				}
			}
			else
			{
				this.Blink.Visible = (this.bloom.Visible = (this.light.Visible = false));
				this.buzzSfx.Param("on", 0f);
			}
			if (this.Sprite.CurrentAnimationID == "eat" && this.Sprite.CurrentAnimationFrame == 5 && this.lastFrame != this.Sprite.CurrentAnimationFrame)
			{
				Level level = base.SceneAs<Level>();
				level.ParticlesFG.Emit(Payphone.P_Snow, 10, level.Camera.Position + new Vector2(236f, 152f), new Vector2(10f, 0f));
				level.ParticlesFG.Emit(Payphone.P_SnowB, 8, level.Camera.Position + new Vector2(236f, 152f), new Vector2(6f, 0f));
				level.DirectionalShake(Vector2.UnitY, 0.3f);
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			}
			if (this.Sprite.CurrentAnimationID == "eat" && this.Sprite.CurrentAnimationFrame == this.Sprite.CurrentAnimationTotalFrames - 5 && this.lastFrame != this.Sprite.CurrentAnimationFrame)
			{
				Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			}
			this.lastFrame = this.Sprite.CurrentAnimationFrame;
		}

		// Token: 0x040012D2 RID: 4818
		public static ParticleType P_Snow;

		// Token: 0x040012D3 RID: 4819
		public static ParticleType P_SnowB;

		// Token: 0x040012D4 RID: 4820
		public bool Broken;

		// Token: 0x040012D5 RID: 4821
		public Sprite Sprite;

		// Token: 0x040012D6 RID: 4822
		public Image Blink;

		// Token: 0x040012D7 RID: 4823
		private VertexLight light;

		// Token: 0x040012D8 RID: 4824
		private BloomPoint bloom;

		// Token: 0x040012D9 RID: 4825
		private float lightFlickerTimer;

		// Token: 0x040012DA RID: 4826
		private float lightFlickerFor = 0.1f;

		// Token: 0x040012DB RID: 4827
		private int lastFrame;

		// Token: 0x040012DC RID: 4828
		private SoundSource buzzSfx;
	}
}
