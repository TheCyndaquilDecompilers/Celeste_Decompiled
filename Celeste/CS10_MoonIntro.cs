using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200016C RID: 364
	public class CS10_MoonIntro : CutsceneEntity
	{
		// Token: 0x06000CFC RID: 3324 RVA: 0x0002C118 File Offset: 0x0002A318
		public CS10_MoonIntro(Player player) : base(true, false)
		{
			base.Depth = -8500;
			this.player = player;
			this.targetX = player.CameraTarget.X + 8f;
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x0002C158 File Offset: 0x0002A358
		public override void OnBegin(Level level)
		{
			this.bird = base.Scene.Entities.FindFirst<BirdNPC>();
			this.player.StateMachine.State = 11;
			if (level.Wipe != null)
			{
				level.Wipe.Cancel();
			}
			level.Wipe = new FadeWipe(level, true, null);
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x0002C1C1 File Offset: 0x0002A3C1
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.Visible = false;
			this.player.Active = false;
			this.player.Dashes = 2;
			for (float t = 0f; t < 1f; t += Engine.DeltaTime / 0.9f)
			{
				level.Wipe.Percent = 0f;
				yield return null;
			}
			base.Add(new Coroutine(this.FadeIn(5f), true));
			level.Camera.Position = level.LevelOffset + new Vector2(-100f, 0f);
			yield return CutsceneEntity.CameraTo(new Vector2(this.targetX, level.Camera.Y), 6f, Ease.SineOut, 0f);
			level.Camera.Position = new Vector2(this.targetX, level.Camera.Y);
			if (this.bird != null)
			{
				yield return this.bird.StartleAndFlyAway();
				level.Session.DoNotLoad.Add(this.bird.EntityID);
				this.bird = null;
			}
			yield return 0.5f;
			this.player.Speed = Vector2.Zero;
			this.player.Position = level.GetSpawnPoint(this.player.Position);
			this.player.Active = true;
			this.player.StateMachine.State = 23;
			while (this.player.Top > (float)level.Bounds.Bottom)
			{
				yield return null;
			}
			yield return 0.2f;
			Audio.Play("event:/new_content/char/madeline/screenentry_lowgrav", this.player.Position);
			while (this.player.StateMachine.State == 23)
			{
				yield return null;
			}
			this.player.X = (float)((int)this.player.X);
			this.player.Y = (float)((int)this.player.Y);
			while (!this.player.OnGround(1) && this.player.Bottom < (float)level.Bounds.Bottom)
			{
				this.player.MoveVExact(16, null, null);
			}
			this.player.StateMachine.State = 11;
			yield return 0.5f;
			yield return this.BadelineAppears();
			yield return Textbox.Say("CH9_LANDING", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.BadelineTurns),
				new Func<IEnumerator>(this.BadelineVanishes)
			});
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x0002C1D7 File Offset: 0x0002A3D7
		private IEnumerator BadelineTurns()
		{
			yield return 0.1f;
			int num = Math.Sign(this.badeline.Sprite.Scale.X);
			int target = num * -1;
			Wiggler component = Wiggler.Create(0.5f, 3f, delegate(float v)
			{
				this.badeline.Sprite.Scale = new Vector2((float)target, 1f) * (1f + 0.2f * v);
			}, true, true);
			base.Add(component);
			Audio.Play((target < 0) ? "event:/char/badeline/jump_wall_left" : "event:/char/badeline/jump_wall_left", this.badeline.Position);
			yield return 0.6f;
			yield break;
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x0002C1E6 File Offset: 0x0002A3E6
		private IEnumerator BadelineAppears()
		{
			this.Level.Session.Inventory.Dashes = 1;
			this.player.Dashes = 1;
			this.Level.Add(this.badeline = new BadelineDummy(this.player.Position));
			this.Level.Displacement.AddBurst(this.player.Center, 0.5f, 8f, 32f, 0.5f, null, null);
			Audio.Play("event:/char/badeline/maddy_split", this.player.Position);
			this.badeline.Sprite.Scale.X = 1f;
			yield return this.badeline.FloatTo(this.player.Position + new Vector2(-16f, -16f), new int?(1), false, false, false);
			this.player.Facing = Facings.Left;
			yield return null;
			yield break;
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x0002C1F5 File Offset: 0x0002A3F5
		private IEnumerator BadelineVanishes()
		{
			yield return 0.5f;
			this.badeline.Vanish();
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			this.badeline = null;
			yield return 0.8f;
			this.player.Facing = Facings.Right;
			yield return 0.6f;
			yield break;
		}

		// Token: 0x06000D02 RID: 3330 RVA: 0x0002C204 File Offset: 0x0002A404
		private IEnumerator FadeIn(float duration)
		{
			while (this.fade > 0f)
			{
				this.fade = Calc.Approach(this.fade, 0f, Engine.DeltaTime / duration);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000D03 RID: 3331 RVA: 0x0002C21C File Offset: 0x0002A41C
		public override void OnEnd(Level level)
		{
			level.Session.Inventory.Dashes = 1;
			this.player.Dashes = 1;
			this.player.Depth = 0;
			this.player.Speed = Vector2.Zero;
			this.player.Position = level.GetSpawnPoint(this.player.Position) + new Vector2(0f, -32f);
			this.player.Active = true;
			this.player.Visible = true;
			this.player.StateMachine.State = 0;
			this.player.X = (float)((int)this.player.X);
			this.player.Y = (float)((int)this.player.Y);
			while (!this.player.OnGround(1) && this.player.Bottom < (float)level.Bounds.Bottom)
			{
				this.player.MoveVExact(16, null, null);
			}
			if (this.badeline != null)
			{
				this.badeline.RemoveSelf();
			}
			if (this.bird != null)
			{
				this.bird.RemoveSelf();
				level.Session.DoNotLoad.Add(this.bird.EntityID);
			}
			level.Camera.Position = new Vector2(this.targetX, level.Camera.Y);
			level.Session.SetFlag("moon_intro", true);
		}

		// Token: 0x06000D04 RID: 3332 RVA: 0x0002C39C File Offset: 0x0002A59C
		public override void Render()
		{
			Camera camera = (base.Scene as Level).Camera;
			Draw.Rect(camera.X - 10f, camera.Y - 10f, 340f, 200f, Color.Black * this.fade);
		}

		// Token: 0x04000848 RID: 2120
		public const string Flag = "moon_intro";

		// Token: 0x04000849 RID: 2121
		private Player player;

		// Token: 0x0400084A RID: 2122
		private BadelineDummy badeline;

		// Token: 0x0400084B RID: 2123
		private BirdNPC bird;

		// Token: 0x0400084C RID: 2124
		private float fade = 1f;

		// Token: 0x0400084D RID: 2125
		private float targetX;
	}
}
