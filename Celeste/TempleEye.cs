using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001B8 RID: 440
	public class TempleEye : Entity
	{
		// Token: 0x06000F45 RID: 3909 RVA: 0x0003DEF1 File Offset: 0x0003C0F1
		public TempleEye(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
		}

		// Token: 0x06000F46 RID: 3910 RVA: 0x0003DF08 File Offset: 0x0003C108
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.isBG = !scene.CollideCheck<Solid>(this.Position);
			if (this.isBG)
			{
				this.eyeTexture = GFX.Game["scenery/temple/eye/bg_eye"];
				this.pupilTexture = GFX.Game["scenery/temple/eye/bg_pupil"];
				base.Add(this.eyelid = new Sprite(GFX.Game, "scenery/temple/eye/bg_lid"));
				base.Depth = 8990;
			}
			else
			{
				this.eyeTexture = GFX.Game["scenery/temple/eye/fg_eye"];
				this.pupilTexture = GFX.Game["scenery/temple/eye/fg_pupil"];
				base.Add(this.eyelid = new Sprite(GFX.Game, "scenery/temple/eye/fg_lid"));
				base.Depth = -10001;
			}
			this.eyelid.AddLoop("open", "", 0f, new int[1]);
			this.eyelid.Add("blink", "", 0.08f, "open", new int[]
			{
				0,
				1,
				1,
				2,
				3,
				0
			});
			this.eyelid.Play("open", false, false);
			this.eyelid.CenterOrigin();
			this.SetBlinkTimer();
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x0003E055 File Offset: 0x0003C255
		private void SetBlinkTimer()
		{
			this.blinkTimer = Calc.Random.Range(1f, 15f);
		}

		// Token: 0x06000F48 RID: 3912 RVA: 0x0003E074 File Offset: 0x0003C274
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			TheoCrystal entity = base.Scene.Tracker.GetEntity<TheoCrystal>();
			if (entity != null)
			{
				this.pupilTarget = (entity.Center - this.Position).SafeNormalize();
				this.pupilPosition = this.pupilTarget * 3f;
			}
		}

		// Token: 0x06000F49 RID: 3913 RVA: 0x0003E0D0 File Offset: 0x0003C2D0
		public override void Update()
		{
			if (!this.bursting)
			{
				this.pupilPosition = Calc.Approach(this.pupilPosition, this.pupilTarget * 3f, Engine.DeltaTime * 16f);
				TheoCrystal entity = base.Scene.Tracker.GetEntity<TheoCrystal>();
				if (entity != null)
				{
					this.pupilTarget = (entity.Center - this.Position).SafeNormalize();
					if (base.Scene.OnInterval(0.25f) && Calc.Random.Chance(0.01f))
					{
						this.eyelid.Play("blink", false, false);
					}
				}
				this.blinkTimer -= Engine.DeltaTime;
				if (this.blinkTimer <= 0f)
				{
					this.SetBlinkTimer();
					this.eyelid.Play("blink", false, false);
				}
			}
			base.Update();
		}

		// Token: 0x06000F4A RID: 3914 RVA: 0x0003E1B8 File Offset: 0x0003C3B8
		public void Burst()
		{
			this.bursting = true;
			Sprite sprite = new Sprite(GFX.Game, this.isBG ? "scenery/temple/eye/bg_burst" : "scenery/temple/eye/fg_burst");
			sprite.Add("burst", "", 0.08f);
			sprite.Play("burst", false, false);
			sprite.OnLastFrame = delegate(string f)
			{
				base.RemoveSelf();
			};
			sprite.CenterOrigin();
			base.Add(sprite);
			base.Remove(this.eyelid);
		}

		// Token: 0x06000F4B RID: 3915 RVA: 0x0003E239 File Offset: 0x0003C439
		public override void Render()
		{
			if (!this.bursting)
			{
				this.eyeTexture.DrawCentered(this.Position);
				this.pupilTexture.DrawCentered(this.Position + this.pupilPosition);
			}
			base.Render();
		}

		// Token: 0x04000AAE RID: 2734
		private MTexture eyeTexture;

		// Token: 0x04000AAF RID: 2735
		private MTexture pupilTexture;

		// Token: 0x04000AB0 RID: 2736
		private Sprite eyelid;

		// Token: 0x04000AB1 RID: 2737
		private Vector2 pupilPosition;

		// Token: 0x04000AB2 RID: 2738
		private Vector2 pupilTarget;

		// Token: 0x04000AB3 RID: 2739
		private float blinkTimer;

		// Token: 0x04000AB4 RID: 2740
		private bool bursting;

		// Token: 0x04000AB5 RID: 2741
		private bool isBG;
	}
}
