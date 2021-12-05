using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002B4 RID: 692
	public class ClutterDoor : Solid
	{
		// Token: 0x0600155B RID: 5467 RVA: 0x0007ACCC File Offset: 0x00078ECC
		public ClutterDoor(EntityData data, Vector2 offset, Session session) : base(data.Position + offset, (float)data.Width, (float)data.Height, false)
		{
			this.Color = data.Enum<ClutterBlock.Colors>("type", ClutterBlock.Colors.Green);
			this.SurfaceSoundIndex = 20;
			base.Tag = Tags.TransitionUpdate;
			base.Add(this.sprite = GFX.SpriteBank.Create("ghost_door"));
			this.sprite.Position = new Vector2(base.Width, base.Height) / 2f;
			this.sprite.Play("idle", false, false);
			this.OnDashCollide = new DashCollision(this.OnDashed);
			base.Add(this.wiggler = Wiggler.Create(0.6f, 3f, delegate(float f)
			{
				this.sprite.Scale = Vector2.One * (1f - f * 0.2f);
			}, false, false));
			if (!this.IsLocked(session))
			{
				this.InstantUnlock();
			}
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x0007ADC8 File Offset: 0x00078FC8
		public override void Update()
		{
			Level level = base.Scene as Level;
			if (level.Transitioning && base.CollideCheck<Player>())
			{
				this.Visible = false;
				this.Collidable = false;
			}
			else if (!this.Collidable && this.IsLocked(level.Session) && !base.CollideCheck<Player>())
			{
				this.Visible = true;
				this.Collidable = true;
				this.wiggler.Start();
				Audio.Play("event:/game/03_resort/forcefield_bump", this.Position);
			}
			base.Update();
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x0007AE4F File Offset: 0x0007904F
		public bool IsLocked(Session session)
		{
			return !session.GetFlag("oshiro_clutter_door_open") || this.IsComplete(session);
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x0007AE67 File Offset: 0x00079067
		public bool IsComplete(Session session)
		{
			return session.GetFlag("oshiro_clutter_cleared_" + (int)this.Color);
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x0007AE84 File Offset: 0x00079084
		public IEnumerator UnlockRoutine()
		{
			Camera camera = base.SceneAs<Level>().Camera;
			Vector2 from = camera.Position;
			Vector2 to = this.CameraTarget();
			if ((from - to).Length() > 8f)
			{
				for (float p = 0f; p < 1f; p += Engine.DeltaTime)
				{
					camera.Position = from + (to - from) * Ease.CubeInOut(p);
					yield return null;
				}
			}
			else
			{
				yield return 0.2f;
			}
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			Audio.Play("event:/game/03_resort/forcefield_vanish", this.Position);
			this.sprite.Play("open", false, false);
			this.Collidable = false;
			for (float p = 0f; p < 0.4f; p += Engine.DeltaTime)
			{
				camera.Position = this.CameraTarget();
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001560 RID: 5472 RVA: 0x0007AE94 File Offset: 0x00079094
		public void InstantUnlock()
		{
			this.Visible = (this.Collidable = false);
		}

		// Token: 0x06001561 RID: 5473 RVA: 0x0007AEB4 File Offset: 0x000790B4
		private Vector2 CameraTarget()
		{
			Level level = base.SceneAs<Level>();
			Vector2 vector = this.Position - new Vector2(320f, 180f) / 2f;
			vector.X = MathHelper.Clamp(vector.X, (float)level.Bounds.Left, (float)(level.Bounds.Right - 320));
			vector.Y = MathHelper.Clamp(vector.Y, (float)level.Bounds.Top, (float)(level.Bounds.Bottom - 180));
			return vector;
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x0007AF5A File Offset: 0x0007915A
		private DashCollisionResults OnDashed(Player player, Vector2 direction)
		{
			this.wiggler.Start();
			Audio.Play("event:/game/03_resort/forcefield_bump", this.Position);
			return DashCollisionResults.Bounce;
		}

		// Token: 0x04001171 RID: 4465
		public ClutterBlock.Colors Color;

		// Token: 0x04001172 RID: 4466
		private Sprite sprite;

		// Token: 0x04001173 RID: 4467
		private Wiggler wiggler;
	}
}
