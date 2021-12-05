using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000186 RID: 390
	public class NPC09_Granny_Outside : NPC
	{
		// Token: 0x06000DBE RID: 3518 RVA: 0x00030A2C File Offset: 0x0002EC2C
		public NPC09_Granny_Outside(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("granny"));
			this.Sprite.Play("idle", false, false);
			base.Add(this.LaughSfx = new GrannyLaughSfx(this.Sprite));
			this.MoveAnim = "walk";
			this.IdleAnim = "idle";
			this.Maxspeed = 40f;
			base.SetupGrannySpriteSounds();
		}

		// Token: 0x06000DBF RID: 3519 RVA: 0x00030ABC File Offset: 0x0002ECBC
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if ((scene as Level).Session.GetFlag("granny_outside"))
			{
				base.RemoveSelf();
			}
			scene.Add(this.Hahaha = new Hahaha(this.Position + new Vector2(8f, -4f), "", false, null));
			this.Hahaha.Enabled = false;
		}

		// Token: 0x06000DC0 RID: 3520 RVA: 0x00030B38 File Offset: 0x0002ED38
		public override void Update()
		{
			if (!this.talking)
			{
				this.player = this.Level.Tracker.GetEntity<Player>();
				if (this.player != null && this.player.X > base.X - 48f)
				{
					(base.Scene as Level).StartCutscene(new Action<Level>(this.EndTalking), true, false, true);
					base.Add(new Coroutine(this.TalkRoutine(this.player), true));
					this.talking = true;
				}
			}
			this.Hahaha.Enabled = (this.Sprite.CurrentAnimationID == "laugh");
			base.Update();
		}

		// Token: 0x06000DC1 RID: 3521 RVA: 0x00030BE8 File Offset: 0x0002EDE8
		private IEnumerator TalkRoutine(Player player)
		{
			player.StateMachine.State = 11;
			while (!player.OnGround(1))
			{
				yield return null;
			}
			this.Sprite.Scale.X = -1f;
			yield return player.DummyWalkToExact((int)base.X - 16, false, 1f, false);
			yield return 0.5f;
			yield return this.Level.ZoomTo(new Vector2(200f, 110f), 2f, 0.5f);
			yield return Textbox.Say("APP_OLDLADY_A", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.MoveRight),
				new Func<IEnumerator>(this.ExitRight)
			});
			yield return this.Level.ZoomBack(0.5f);
			this.Sprite.Scale.X = 1f;
			if (!this.leaving)
			{
				yield return this.ExitRight();
			}
			while (base.X < (float)(this.Level.Bounds.Right + 8))
			{
				yield return null;
			}
			this.Level.EndCutscene();
			this.EndTalking(this.Level);
			yield break;
		}

		// Token: 0x06000DC2 RID: 3522 RVA: 0x00030BFE File Offset: 0x0002EDFE
		private IEnumerator MoveRight()
		{
			yield return base.MoveTo(new Vector2(base.X + 8f, base.Y), false, null, false);
			yield break;
		}

		// Token: 0x06000DC3 RID: 3523 RVA: 0x00030C0D File Offset: 0x0002EE0D
		private IEnumerator ExitRight()
		{
			this.leaving = true;
			base.Add(new Coroutine(base.MoveTo(new Vector2((float)(this.Level.Bounds.Right + 16), base.Y), false, null, false), true));
			yield return null;
			yield break;
		}

		// Token: 0x06000DC4 RID: 3524 RVA: 0x00030C1C File Offset: 0x0002EE1C
		private void EndTalking(Level level)
		{
			if (this.player != null)
			{
				this.player.StateMachine.State = 0;
			}
			this.Level.Session.SetFlag("granny_outside", true);
			base.RemoveSelf();
		}

		// Token: 0x040008FB RID: 2299
		public const string Flag = "granny_outside";

		// Token: 0x040008FC RID: 2300
		public Hahaha Hahaha;

		// Token: 0x040008FD RID: 2301
		public GrannyLaughSfx LaughSfx;

		// Token: 0x040008FE RID: 2302
		private bool talking;

		// Token: 0x040008FF RID: 2303
		private Player player;

		// Token: 0x04000900 RID: 2304
		private bool leaving;
	}
}
