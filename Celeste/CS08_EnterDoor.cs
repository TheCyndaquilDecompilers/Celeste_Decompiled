using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200016D RID: 365
	public class CS08_EnterDoor : CutsceneEntity
	{
		// Token: 0x06000D05 RID: 3333 RVA: 0x0002C3F1 File Offset: 0x0002A5F1
		public CS08_EnterDoor(Player player, float targetX) : base(true, false)
		{
			this.player = player;
			this.targetX = targetX;
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x0002C409 File Offset: 0x0002A609
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x0002C41E File Offset: 0x0002A61E
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			base.Add(new Coroutine(this.player.DummyWalkToExact((int)this.targetX, false, 0.7f, false), true));
			base.Add(new Coroutine(level.ZoomTo(new Vector2(this.targetX - level.Camera.X, 90f), 2f, 2f), true));
			yield return new FadeWipe(level, false, null)
			{
				Duration = 2f
			}.Wait();
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x0002C434 File Offset: 0x0002A634
		public override void OnEnd(Level level)
		{
			level.OnEndOfFrame += delegate()
			{
				level.Remove(this.player);
				level.UnloadLevel();
				level.Session.Level = "inside";
				level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float)level.Bounds.Left, (float)level.Bounds.Top)));
				level.LoadLevel(Player.IntroTypes.None, false);
				level.Add(new CS08_Ending());
			};
		}

		// Token: 0x0400084E RID: 2126
		private Player player;

		// Token: 0x0400084F RID: 2127
		private float targetX;
	}
}
