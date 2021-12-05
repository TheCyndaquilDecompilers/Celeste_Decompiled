using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000280 RID: 640
	public class CS02_Mirror : CutsceneEntity
	{
		// Token: 0x060013CA RID: 5066 RVA: 0x0006B5CD File Offset: 0x000697CD
		public CS02_Mirror(Player player, DreamMirror mirror) : base(true, false)
		{
			this.player = player;
			this.mirror = mirror;
		}

		// Token: 0x060013CB RID: 5067 RVA: 0x0006B5EC File Offset: 0x000697EC
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x060013CC RID: 5068 RVA: 0x0006B601 File Offset: 0x00069801
		private IEnumerator Cutscene(Level level)
		{
			base.Add(this.sfx = new SoundSource());
			this.sfx.Position = this.mirror.Center;
			this.sfx.Play("event:/music/lvl2/dreamblock_sting_pt1", null, 0f);
			this.direction = Math.Sign(this.player.X - this.mirror.X);
			this.player.StateMachine.State = 11;
			this.playerEndX = (float)(8 * this.direction);
			yield return 1f;
			this.player.Facing = (Facings)(-(Facings)this.direction);
			yield return 0.4f;
			yield return this.player.DummyRunTo(this.mirror.X + this.playerEndX, false);
			yield return 0.5f;
			yield return level.ZoomTo(this.mirror.Position - level.Camera.Position - Vector2.UnitY * 24f, 2f, 1f);
			yield return 0.5f;
			yield return this.mirror.BreakRoutine(this.direction);
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("lookUp", false, false);
			Vector2 from = level.Camera.Position;
			Vector2 to = level.Camera.Position + new Vector2(0f, -80f);
			for (float ease = 0f; ease < 1f; ease += Engine.DeltaTime * 1.2f)
			{
				level.Camera.Position = from + (to - from) * Ease.CubeInOut(ease);
				yield return null;
			}
			base.Add(new Coroutine(this.ZoomBack(), true));
			using (List<Entity>.Enumerator enumerator = base.Scene.Tracker.GetEntities<DreamBlock>().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					DreamBlock dreamBlock = (DreamBlock)enumerator.Current;
					yield return dreamBlock.Activate();
				}
			}
			List<Entity>.Enumerator enumerator = default(List<Entity>.Enumerator);
			from = default(Vector2);
			to = default(Vector2);
			yield return 0.5f;
			base.EndCutscene(level, true);
			yield break;
			yield break;
		}

		// Token: 0x060013CD RID: 5069 RVA: 0x0006B617 File Offset: 0x00069817
		private IEnumerator ZoomBack()
		{
			yield return 1.2f;
			yield return this.Level.ZoomBack(3f);
			yield break;
		}

		// Token: 0x060013CE RID: 5070 RVA: 0x0006B628 File Offset: 0x00069828
		public override void OnEnd(Level level)
		{
			this.mirror.Broken(this.WasSkipped);
			if (this.WasSkipped)
			{
				base.SceneAs<Level>().ParticlesFG.Clear();
			}
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				entity.StateMachine.State = 0;
				entity.DummyAutoAnimate = true;
				entity.Speed = Vector2.Zero;
				entity.X = this.mirror.X + this.playerEndX;
				if (this.direction != 0)
				{
					entity.Facing = (Facings)(-(Facings)this.direction);
				}
				else
				{
					entity.Facing = Facings.Right;
				}
			}
			foreach (Entity entity2 in base.Scene.Tracker.GetEntities<DreamBlock>())
			{
				((DreamBlock)entity2).ActivateNoRoutine();
			}
			level.ResetZoom();
			level.Session.Inventory.DreamDash = true;
			level.Session.Audio.Music.Event = "event:/music/lvl2/mirror";
			level.Session.Audio.Apply(false);
		}

		// Token: 0x04000F90 RID: 3984
		private Player player;

		// Token: 0x04000F91 RID: 3985
		private DreamMirror mirror;

		// Token: 0x04000F92 RID: 3986
		private float playerEndX;

		// Token: 0x04000F93 RID: 3987
		private int direction = 1;

		// Token: 0x04000F94 RID: 3988
		private SoundSource sfx;
	}
}
