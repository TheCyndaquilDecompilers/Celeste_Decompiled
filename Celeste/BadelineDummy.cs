using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002D3 RID: 723
	public class BadelineDummy : Entity
	{
		// Token: 0x06001653 RID: 5715 RVA: 0x000834E4 File Offset: 0x000816E4
		public BadelineDummy(Vector2 position) : base(position)
		{
			base.Collider = new Hitbox(6f, 6f, -3f, -7f);
			this.Sprite = new PlayerSprite(PlayerSpriteMode.Badeline);
			this.Sprite.Play("fallSlow", false, false);
			this.Sprite.Scale.X = -1f;
			this.Hair = new PlayerHair(this.Sprite);
			this.Hair.Color = BadelineOldsite.HairColor;
			this.Hair.Border = Color.Black;
			this.Hair.Facing = Facings.Left;
			base.Add(this.Hair);
			base.Add(this.Sprite);
			base.Add(this.AutoAnimator = new BadelineAutoAnimator());
			this.Sprite.OnFrameChange = delegate(string anim)
			{
				int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
				if ((anim == "walk" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim == "runSlow" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim == "runFast" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)))
				{
					Audio.Play("event:/char/badeline/footstep", this.Position);
				}
			};
			base.Add(this.Wave = new SineWave(0.25f, 0f));
			this.Wave.OnUpdate = delegate(float f)
			{
				this.Sprite.Position = this.floatNormal * f * this.Floatness;
			};
			base.Add(this.Light = new VertexLight(new Vector2(0f, -8f), Color.PaleVioletRed, 1f, 20, 60));
		}

		// Token: 0x06001654 RID: 5716 RVA: 0x00083668 File Offset: 0x00081868
		public void Appear(Level level, bool silent = false)
		{
			if (!silent)
			{
				Audio.Play("event:/char/badeline/appear", this.Position);
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			}
			level.Displacement.AddBurst(base.Center, 0.5f, 24f, 96f, 0.4f, null, null);
			level.Particles.Emit(BadelineOldsite.P_Vanish, 12, base.Center, Vector2.One * 6f);
		}

		// Token: 0x06001655 RID: 5717 RVA: 0x000836E0 File Offset: 0x000818E0
		public void Vanish()
		{
			Audio.Play("event:/char/badeline/disappear", this.Position);
			this.Shockwave();
			base.SceneAs<Level>().Particles.Emit(BadelineOldsite.P_Vanish, 12, base.Center, Vector2.One * 6f);
			base.RemoveSelf();
		}

		// Token: 0x06001656 RID: 5718 RVA: 0x00083736 File Offset: 0x00081936
		private void Shockwave()
		{
			base.SceneAs<Level>().Displacement.AddBurst(base.Center, 0.5f, 24f, 96f, 0.4f, null, null);
		}

		// Token: 0x06001657 RID: 5719 RVA: 0x00083765 File Offset: 0x00081965
		public IEnumerator FloatTo(Vector2 target, int? turnAtEndTo = null, bool faceDirection = true, bool fadeLight = false, bool quickEnd = false)
		{
			this.Sprite.Play("fallSlow", false, false);
			if (faceDirection && Math.Sign(target.X - base.X) != 0)
			{
				this.Sprite.Scale.X = (float)Math.Sign(target.X - base.X);
			}
			Vector2 vector = (target - this.Position).SafeNormalize();
			Vector2 perp = new Vector2(-vector.Y, vector.X);
			float speed = 0f;
			while (this.Position != target)
			{
				speed = Calc.Approach(speed, this.FloatSpeed, this.FloatAccel * Engine.DeltaTime);
				this.Position = Calc.Approach(this.Position, target, speed * Engine.DeltaTime);
				this.Floatness = Calc.Approach(this.Floatness, 4f, 8f * Engine.DeltaTime);
				this.floatNormal = Calc.Approach(this.floatNormal, perp, Engine.DeltaTime * 12f);
				if (fadeLight)
				{
					this.Light.Alpha = Calc.Approach(this.Light.Alpha, 0f, Engine.DeltaTime * 2f);
				}
				yield return null;
			}
			if (quickEnd)
			{
				this.Floatness = 2f;
			}
			else
			{
				while (this.Floatness != 2f)
				{
					this.Floatness = Calc.Approach(this.Floatness, 2f, 8f * Engine.DeltaTime);
					yield return null;
				}
			}
			if (turnAtEndTo != null)
			{
				this.Sprite.Scale.X = (float)turnAtEndTo.Value;
			}
			yield break;
		}

		// Token: 0x06001658 RID: 5720 RVA: 0x00083799 File Offset: 0x00081999
		public IEnumerator WalkTo(float x, float speed = 64f)
		{
			this.Floatness = 0f;
			this.Sprite.Play("walk", false, false);
			if (Math.Sign(x - base.X) != 0)
			{
				this.Sprite.Scale.X = (float)Math.Sign(x - base.X);
			}
			while (base.X != x)
			{
				base.X = Calc.Approach(base.X, x, Engine.DeltaTime * speed);
				yield return null;
			}
			this.Sprite.Play("idle", false, false);
			yield break;
		}

		// Token: 0x06001659 RID: 5721 RVA: 0x000837B6 File Offset: 0x000819B6
		public IEnumerator SmashBlock(Vector2 target)
		{
			base.SceneAs<Level>().Displacement.AddBurst(this.Position, 0.5f, 24f, 96f, 1f, null, null);
			this.Sprite.Play("dreamDashLoop", false, false);
			Vector2 from = this.Position;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 6f)
			{
				this.Position = from + (target - from) * Ease.CubeOut(p);
				yield return null;
			}
			base.Scene.Entities.FindFirst<DashBlock>().Break(this.Position, new Vector2(0f, -1f), false, true);
			this.Sprite.Play("idle", false, false);
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
			{
				this.Position = target + (from - target) * Ease.CubeOut(p);
				yield return null;
			}
			this.Sprite.Play("fallSlow", false, false);
			yield break;
		}

		// Token: 0x0600165A RID: 5722 RVA: 0x000837CC File Offset: 0x000819CC
		public override void Update()
		{
			if (this.Sprite.Scale.X != 0f)
			{
				this.Hair.Facing = (Facings)Math.Sign(this.Sprite.Scale.X);
			}
			base.Update();
		}

		// Token: 0x0600165B RID: 5723 RVA: 0x0008380C File Offset: 0x00081A0C
		public override void Render()
		{
			Vector2 renderPosition = this.Sprite.RenderPosition;
			this.Sprite.RenderPosition = this.Sprite.RenderPosition.Floor();
			base.Render();
			this.Sprite.RenderPosition = renderPosition;
		}

		// Token: 0x040012AB RID: 4779
		public PlayerSprite Sprite;

		// Token: 0x040012AC RID: 4780
		public PlayerHair Hair;

		// Token: 0x040012AD RID: 4781
		public BadelineAutoAnimator AutoAnimator;

		// Token: 0x040012AE RID: 4782
		public SineWave Wave;

		// Token: 0x040012AF RID: 4783
		public VertexLight Light;

		// Token: 0x040012B0 RID: 4784
		public float FloatSpeed = 120f;

		// Token: 0x040012B1 RID: 4785
		public float FloatAccel = 240f;

		// Token: 0x040012B2 RID: 4786
		public float Floatness = 2f;

		// Token: 0x040012B3 RID: 4787
		public Vector2 floatNormal = new Vector2(0f, 1f);
	}
}
