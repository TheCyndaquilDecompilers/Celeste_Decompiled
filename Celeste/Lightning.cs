using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000179 RID: 377
	public class Lightning : Entity
	{
		// Token: 0x06000D56 RID: 3414 RVA: 0x0002DABC File Offset: 0x0002BCBC
		public Lightning(Vector2 position, int width, int height, Vector2? node, float moveTime) : base(position)
		{
			this.VisualWidth = width;
			this.VisualHeight = height;
			base.Collider = new Hitbox((float)(width - 2), (float)(height - 2), 1f, 1f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			if (node != null)
			{
				base.Add(new Coroutine(this.MoveRoutine(position, node.Value, moveTime), true));
			}
			this.toggleOffset = Calc.Random.NextFloat();
		}

		// Token: 0x06000D57 RID: 3415 RVA: 0x0002DB49 File Offset: 0x0002BD49
		public Lightning(EntityData data, Vector2 levelOffset) : this(data.Position + levelOffset, data.Width, data.Height, data.FirstNodeNullable(new Vector2?(levelOffset)), data.Float("moveTime", 0f))
		{
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x0002DB85 File Offset: 0x0002BD85
		public override void Added(Scene scene)
		{
			base.Added(scene);
			scene.Tracker.GetEntity<LightningRenderer>().Track(this);
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x0002DB9F File Offset: 0x0002BD9F
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			scene.Tracker.GetEntity<LightningRenderer>().Untrack(this);
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x0002DBBC File Offset: 0x0002BDBC
		public override void Update()
		{
			if (this.Collidable && base.Scene.OnInterval(0.25f, this.toggleOffset))
			{
				this.ToggleCheck();
			}
			if (!this.Collidable && base.Scene.OnInterval(0.05f, this.toggleOffset))
			{
				this.ToggleCheck();
			}
			base.Update();
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x0002DC1C File Offset: 0x0002BE1C
		public void ToggleCheck()
		{
			this.Collidable = (this.Visible = this.InView());
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x0002DC40 File Offset: 0x0002BE40
		private bool InView()
		{
			Camera camera = (base.Scene as Level).Camera;
			return base.X + base.Width > camera.X - 16f && base.Y + base.Height > camera.Y - 16f && base.X < camera.X + 320f + 16f && base.Y < camera.Y + 180f + 16f;
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x0002DCCC File Offset: 0x0002BECC
		private void OnPlayer(Player player)
		{
			if (!this.disappearing && !SaveData.Instance.Assists.Invincible)
			{
				int num = Math.Sign(player.X - base.X);
				if (num == 0)
				{
					num = -1;
				}
				player.Die(Vector2.UnitX * (float)num, false, true);
			}
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x0002DD1F File Offset: 0x0002BF1F
		private IEnumerator MoveRoutine(Vector2 start, Vector2 end, float moveTime)
		{
			for (;;)
			{
				yield return this.Move(start, end, moveTime);
				yield return this.Move(end, start, moveTime);
			}
			yield break;
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x0002DD43 File Offset: 0x0002BF43
		private IEnumerator Move(Vector2 start, Vector2 end, float moveTime)
		{
			float at = 0f;
			for (;;)
			{
				this.Position = Vector2.Lerp(start, end, Ease.SineInOut(at));
				if (at >= 1f)
				{
					break;
				}
				yield return null;
				at = MathHelper.Clamp(at + Engine.DeltaTime / moveTime, 0f, 1f);
			}
			yield break;
			yield break;
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x0002DD68 File Offset: 0x0002BF68
		private void Shatter()
		{
			if (base.Scene != null)
			{
				int num = 4;
				while ((float)num < base.Width)
				{
					int num2 = 4;
					while ((float)num2 < base.Height)
					{
						base.SceneAs<Level>().ParticlesFG.Emit(Lightning.P_Shatter, 1, base.TopLeft + new Vector2((float)num, (float)num2), Vector2.One * 3f);
						num2 += 8;
					}
					num += 8;
				}
			}
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x0002DDDA File Offset: 0x0002BFDA
		public static IEnumerator PulseRoutine(Level level)
		{
			for (float t = 0f; t < 1f; t += Engine.DeltaTime * 8f)
			{
				Lightning.SetPulseValue(level, t);
				yield return null;
			}
			for (float t = 1f; t > 0f; t -= Engine.DeltaTime * 8f)
			{
				Lightning.SetPulseValue(level, t);
				yield return null;
			}
			Lightning.SetPulseValue(level, 0f);
			yield break;
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x0002DDEC File Offset: 0x0002BFEC
		private static void SetPulseValue(Level level, float t)
		{
			BloomRenderer bloom = level.Bloom;
			LightningRenderer entity = level.Tracker.GetEntity<LightningRenderer>();
			Glitch.Value = MathHelper.Lerp(0f, 0.075f, t);
			bloom.Strength = MathHelper.Lerp(1f, 1.2f, t);
			entity.Fade = t * 0.2f;
		}

		// Token: 0x06000D63 RID: 3427 RVA: 0x0002DE44 File Offset: 0x0002C044
		private static void SetBreakValue(Level level, float t)
		{
			BloomRenderer bloom = level.Bloom;
			LightningRenderer entity = level.Tracker.GetEntity<LightningRenderer>();
			Glitch.Value = MathHelper.Lerp(0f, 0.15f, t);
			bloom.Strength = MathHelper.Lerp(1f, 1.5f, t);
			entity.Fade = t * 0.6f;
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x0002DE9A File Offset: 0x0002C09A
		public static IEnumerator RemoveRoutine(Level level, Action onComplete = null)
		{
			List<Lightning> blocks = level.Entities.FindAll<Lightning>();
			foreach (Lightning lightning in new List<Lightning>(blocks))
			{
				lightning.disappearing = true;
				if (lightning.Right < level.Camera.Left || lightning.Bottom < level.Camera.Top || lightning.Left > level.Camera.Right || lightning.Top > level.Camera.Bottom)
				{
					blocks.Remove(lightning);
					lightning.RemoveSelf();
				}
			}
			LightningRenderer entity = level.Tracker.GetEntity<LightningRenderer>();
			entity.StopAmbience();
			entity.UpdateSeeds = false;
			for (float t = 0f; t < 1f; t += Engine.DeltaTime * 4f)
			{
				Lightning.SetBreakValue(level, t);
				yield return null;
			}
			Lightning.SetBreakValue(level, 1f);
			level.Shake(0.3f);
			for (int i = blocks.Count - 1; i >= 0; i--)
			{
				blocks[i].Shatter();
			}
			for (float t = 0f; t < 1f; t += Engine.DeltaTime * 8f)
			{
				Lightning.SetBreakValue(level, 1f - t);
				yield return null;
			}
			Lightning.SetBreakValue(level, 0f);
			foreach (Lightning lightning2 in blocks)
			{
				lightning2.RemoveSelf();
			}
			FlingBird flingBird = level.Entities.FindFirst<FlingBird>();
			if (flingBird != null)
			{
				flingBird.LightningRemoved = true;
			}
			if (onComplete != null)
			{
				onComplete();
			}
			yield break;
		}

		// Token: 0x0400089B RID: 2203
		public static ParticleType P_Shatter;

		// Token: 0x0400089C RID: 2204
		public const string Flag = "disable_lightning";

		// Token: 0x0400089D RID: 2205
		public float Fade;

		// Token: 0x0400089E RID: 2206
		private bool disappearing;

		// Token: 0x0400089F RID: 2207
		private float toggleOffset;

		// Token: 0x040008A0 RID: 2208
		public int VisualWidth;

		// Token: 0x040008A1 RID: 2209
		public int VisualHeight;
	}
}
