using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200021F RID: 543
	public class CS05_SaveTheo : CutsceneEntity
	{
		// Token: 0x0600117A RID: 4474 RVA: 0x00056A7B File Offset: 0x00054C7B
		public CS05_SaveTheo(Player player) : base(true, false)
		{
			this.player = player;
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x00056A8C File Offset: 0x00054C8C
		public override void OnBegin(Level level)
		{
			this.theo = level.Tracker.GetEntity<TheoCrystal>();
			this.playerEndPosition = this.theo.Position + new Vector2(-24f, 0f);
			this.wasDashAssistOn = SaveData.Instance.Assists.DashAssist;
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x00056AF7 File Offset: 0x00054CF7
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			this.player.ForceCameraUpdate = true;
			level.Session.Audio.Music.Layer(6, 0f);
			level.Session.Audio.Apply(false);
			yield return this.player.DummyWalkTo(this.theo.X - 18f, false, 1f, false);
			this.player.Facing = Facings.Right;
			yield return Textbox.Say("ch5_found_theo", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.TryToBreakCrystal)
			});
			yield return 0.25f;
			yield return this.Level.ZoomBack(0.5f);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x00056B0D File Offset: 0x00054D0D
		private IEnumerator TryToBreakCrystal()
		{
			base.Scene.Entities.FindFirst<TheoCrystalPedestal>().Collidable = true;
			yield return this.player.DummyWalkTo(this.theo.X, false, 1f, false);
			yield return 0.1f;
			yield return this.Level.ZoomTo(new Vector2(160f, 90f), 2f, 0.5f);
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("lookUp", false, false);
			yield return 1f;
			this.wasDashAssistOn = SaveData.Instance.Assists.DashAssist;
			SaveData.Instance.Assists.DashAssist = false;
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			MInput.Disabled = true;
			this.player.OverrideDashDirection = new Vector2?(new Vector2(0f, -1f));
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = this.player.StartDash();
			this.player.Dashes = 0;
			yield return 0.1f;
			while (!this.player.OnGround(1) || this.player.Speed.Y < 0f)
			{
				this.player.Dashes = 0;
				Input.MoveY.Value = -1;
				Input.MoveX.Value = 0;
				yield return null;
			}
			this.player.OverrideDashDirection = null;
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			MInput.Disabled = false;
			this.player.DummyAutoAnimate = true;
			yield return this.player.DummyWalkToExact((int)this.playerEndPosition.X, true, 1f, false);
			yield return 1.5f;
			yield break;
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x00056B1C File Offset: 0x00054D1C
		public override void OnEnd(Level level)
		{
			SaveData.Instance.Assists.DashAssist = this.wasDashAssistOn;
			this.player.Position = this.playerEndPosition;
			while (!this.player.OnGround(1))
			{
				this.player.MoveV(1f, null, null);
			}
			level.Camera.Position = this.player.CameraTarget;
			level.Session.SetFlag("foundTheoInCrystal", true);
			level.ResetZoom();
			level.Session.Audio.Music.Layer(6, 1f);
			level.Session.Audio.Apply(false);
			List<Follower> list = new List<Follower>(this.player.Leader.Followers);
			this.player.RemoveSelf();
			level.Add(this.player = new Player(this.player.Position, this.player.DefaultSpriteMode));
			foreach (Follower follower in list)
			{
				this.player.Leader.Followers.Add(follower);
				follower.Leader = this.player.Leader;
			}
			this.player.Facing = Facings.Right;
			this.player.IntroType = Player.IntroTypes.None;
			TheoCrystalPedestal theoCrystalPedestal = base.Scene.Entities.FindFirst<TheoCrystalPedestal>();
			theoCrystalPedestal.Collidable = false;
			theoCrystalPedestal.DroppedTheo = true;
			this.theo.Depth = 100;
			this.theo.OnPedestal = false;
			this.theo.Speed = Vector2.Zero;
			while (!this.theo.OnGround(1))
			{
				this.theo.MoveV(1f, null, null);
			}
		}

		// Token: 0x04000D1C RID: 3356
		public const string Flag = "foundTheoInCrystal";

		// Token: 0x04000D1D RID: 3357
		private Player player;

		// Token: 0x04000D1E RID: 3358
		private TheoCrystal theo;

		// Token: 0x04000D1F RID: 3359
		private Vector2 playerEndPosition;

		// Token: 0x04000D20 RID: 3360
		private bool wasDashAssistOn;
	}
}
