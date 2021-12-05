using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000268 RID: 616
	public class NPC : Entity
	{
		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06001334 RID: 4916 RVA: 0x00068586 File Offset: 0x00066786
		public Session Session
		{
			get
			{
				return this.Level.Session;
			}
		}

		// Token: 0x06001335 RID: 4917 RVA: 0x00068594 File Offset: 0x00066794
		public NPC(Vector2 position)
		{
			this.Position = position;
			base.Depth = 1000;
			base.Collider = new Hitbox(8f, 8f, -4f, -8f);
			base.Add(new MirrorReflection());
		}

		// Token: 0x06001336 RID: 4918 RVA: 0x0006861D File Offset: 0x0006681D
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.Level = (scene as Level);
		}

		// Token: 0x06001337 RID: 4919 RVA: 0x00068634 File Offset: 0x00066834
		public override void Update()
		{
			base.Update();
			if (this.UpdateLight && this.Light != null)
			{
				Rectangle bounds = this.Level.Bounds;
				bool flag = base.X > (float)(bounds.Left - 16) && base.Y > (float)(bounds.Top - 16) && base.X < (float)(bounds.Right + 16) && base.Y < (float)(bounds.Bottom + 16);
				this.Light.Alpha = Calc.Approach(this.Light.Alpha, (float)((flag && !this.Level.Transitioning) ? 1 : 0), Engine.DeltaTime * 2f);
			}
			if (this.Sprite != null && this.Sprite.CurrentAnimationID == "usePhone")
			{
				if (this.PhoneTapSfx == null)
				{
					base.Add(this.PhoneTapSfx = new SoundSource());
				}
				if (!this.PhoneTapSfx.Playing)
				{
					this.PhoneTapSfx.Play("event:/char/theo/phone_taps_loop", null, 0f);
					return;
				}
			}
			else if (this.PhoneTapSfx != null && this.PhoneTapSfx.Playing)
			{
				this.PhoneTapSfx.Stop(true);
			}
		}

		// Token: 0x06001338 RID: 4920 RVA: 0x00068776 File Offset: 0x00066976
		public void SetupTheoSpriteSounds()
		{
			this.Sprite.OnFrameChange = delegate(string anim)
			{
				int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
				if ((anim == "walk" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim == "run" && (currentAnimationFrame == 0 || currentAnimationFrame == 4)))
				{
					Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(base.CollideAll<Platform>(this.Position + Vector2.UnitY, this.temp));
					if (platformByPriority != null)
					{
						Audio.Play("event:/char/madeline/footstep", base.Center, "surface_index", (float)platformByPriority.GetStepSoundIndex(this));
						return;
					}
				}
				else if (anim == "crawl" && currentAnimationFrame == 0)
				{
					if (!this.Level.Transitioning)
					{
						Audio.Play("event:/char/theo/resort_crawl", this.Position);
						return;
					}
				}
				else if (anim == "pullVent" && currentAnimationFrame == 0)
				{
					Audio.Play("event:/char/theo/resort_vent_tug", this.Position);
				}
			};
		}

		// Token: 0x06001339 RID: 4921 RVA: 0x0006878F File Offset: 0x0006698F
		public void SetupGrannySpriteSounds()
		{
			this.Sprite.OnFrameChange = delegate(string anim)
			{
				int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
				if (anim == "walk" && (currentAnimationFrame == 0 || currentAnimationFrame == 4))
				{
					Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(base.CollideAll<Platform>(this.Position + Vector2.UnitY, this.temp));
					if (platformByPriority != null)
					{
						Audio.Play("event:/char/madeline/footstep", base.Center, "surface_index", (float)platformByPriority.GetStepSoundIndex(this));
						return;
					}
				}
				else if (anim == "walk" && currentAnimationFrame == 2)
				{
					Audio.Play("event:/char/granny/cane_tap", this.Position);
				}
			};
		}

		// Token: 0x0600133A RID: 4922 RVA: 0x000687A8 File Offset: 0x000669A8
		public IEnumerator PlayerApproachRightSide(Player player, bool turnToFace = true, float? spacing = null)
		{
			yield return this.PlayerApproach(player, turnToFace, spacing, new int?(1));
			yield break;
		}

		// Token: 0x0600133B RID: 4923 RVA: 0x000687CC File Offset: 0x000669CC
		public IEnumerator PlayerApproachLeftSide(Player player, bool turnToFace = true, float? spacing = null)
		{
			yield return this.PlayerApproach(player, turnToFace, spacing, new int?(-1));
			yield break;
		}

		// Token: 0x0600133C RID: 4924 RVA: 0x000687F0 File Offset: 0x000669F0
		public IEnumerator PlayerApproach(Player player, bool turnToFace = true, float? spacing = null, int? side = null)
		{
			if (side == null)
			{
				side = new int?(Math.Sign(player.X - base.X));
			}
			int? num = side;
			int num2 = 0;
			if (num.GetValueOrDefault() == num2 & num != null)
			{
				side = new int?(1);
			}
			player.StateMachine.State = 11;
			player.StateMachine.Locked = true;
			if (spacing != null)
			{
				float x = base.X;
				num = side;
				yield return player.DummyWalkToExact((int)(x + ((num != null) ? new float?((float)num.GetValueOrDefault()) : null) * spacing).Value, false, 1f, false);
			}
			else if (Math.Abs(base.X - player.X) < 12f || Math.Sign(player.X - base.X) != side.Value)
			{
				float x = base.X;
				num = side;
				yield return player.DummyWalkToExact((int)(x + ((num != null) ? new float?((float)(num.GetValueOrDefault() * 12)) : null)).Value, false, 1f, false);
			}
			player.Facing = (Facings)(-(Facings)side.Value);
			if (turnToFace && this.Sprite != null)
			{
				this.Sprite.Scale.X = (float)side.Value;
			}
			yield return null;
			yield break;
		}

		// Token: 0x0600133D RID: 4925 RVA: 0x0006881C File Offset: 0x00066A1C
		public IEnumerator PlayerApproach48px()
		{
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			yield return this.PlayerApproach(entity, true, new float?((float)48), null);
			yield break;
		}

		// Token: 0x0600133E RID: 4926 RVA: 0x0006882B File Offset: 0x00066A2B
		public IEnumerator PlayerLeave(Player player, float? to = null)
		{
			if (to != null)
			{
				yield return player.DummyWalkToExact((int)to.Value, false, 1f, false);
			}
			player.StateMachine.Locked = false;
			player.StateMachine.State = 0;
			yield return null;
			yield break;
		}

		// Token: 0x0600133F RID: 4927 RVA: 0x00068841 File Offset: 0x00066A41
		public IEnumerator MoveTo(Vector2 target, bool fadeIn = false, int? turnAtEndTo = null, bool removeAtEnd = false)
		{
			if (removeAtEnd)
			{
				base.Tag |= Tags.TransitionUpdate;
			}
			if (Math.Sign(target.X - base.X) != 0 && this.Sprite != null)
			{
				this.Sprite.Scale.X = (float)Math.Sign(target.X - base.X);
			}
			(target - this.Position).SafeNormalize();
			float alpha = fadeIn ? 0f : 1f;
			if (this.Sprite != null && this.Sprite.Has(this.MoveAnim))
			{
				this.Sprite.Play(this.MoveAnim, false, false);
			}
			float speed = 0f;
			while ((this.MoveY && this.Position != target) || (!this.MoveY && base.X != target.X))
			{
				speed = Calc.Approach(speed, this.Maxspeed, 160f * Engine.DeltaTime);
				if (this.MoveY)
				{
					this.Position = Calc.Approach(this.Position, target, speed * Engine.DeltaTime);
				}
				else
				{
					base.X = Calc.Approach(base.X, target.X, speed * Engine.DeltaTime);
				}
				if (this.Sprite != null)
				{
					this.Sprite.Color = Color.White * alpha;
				}
				alpha = Calc.Approach(alpha, 1f, Engine.DeltaTime);
				yield return null;
			}
			if (this.Sprite != null && this.Sprite.Has(this.IdleAnim))
			{
				this.Sprite.Play(this.IdleAnim, false, false);
			}
			while (alpha < 1f)
			{
				if (this.Sprite != null)
				{
					this.Sprite.Color = Color.White * alpha;
				}
				alpha = Calc.Approach(alpha, 1f, Engine.DeltaTime);
				yield return null;
			}
			if (turnAtEndTo != null && this.Sprite != null)
			{
				this.Sprite.Scale.X = (float)turnAtEndTo.Value;
			}
			if (removeAtEnd)
			{
				base.Scene.Remove(this);
			}
			yield return null;
			yield break;
		}

		// Token: 0x06001340 RID: 4928 RVA: 0x00068870 File Offset: 0x00066A70
		public void MoveToAndRemove(Vector2 target)
		{
			base.Add(new Coroutine(this.MoveTo(target, false, null, true), true));
		}

		// Token: 0x04000F21 RID: 3873
		public const string MetTheo = "MetTheo";

		// Token: 0x04000F22 RID: 3874
		public const string TheoKnowsName = "TheoKnowsName";

		// Token: 0x04000F23 RID: 3875
		public const float TheoMaxSpeed = 48f;

		// Token: 0x04000F24 RID: 3876
		public Sprite Sprite;

		// Token: 0x04000F25 RID: 3877
		public TalkComponent Talker;

		// Token: 0x04000F26 RID: 3878
		public VertexLight Light;

		// Token: 0x04000F27 RID: 3879
		public Level Level;

		// Token: 0x04000F28 RID: 3880
		public SoundSource PhoneTapSfx;

		// Token: 0x04000F29 RID: 3881
		public float Maxspeed = 80f;

		// Token: 0x04000F2A RID: 3882
		public string MoveAnim = "";

		// Token: 0x04000F2B RID: 3883
		public string IdleAnim = "";

		// Token: 0x04000F2C RID: 3884
		public bool MoveY = true;

		// Token: 0x04000F2D RID: 3885
		public bool UpdateLight = true;

		// Token: 0x04000F2E RID: 3886
		private List<Entity> temp = new List<Entity>();
	}
}
