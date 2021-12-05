using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000175 RID: 373
	public class CSGEN_StrawberrySeeds : CutsceneEntity
	{
		// Token: 0x06000D3A RID: 3386 RVA: 0x0002CCE6 File Offset: 0x0002AEE6
		public CSGEN_StrawberrySeeds(Strawberry strawberry) : base(true, false)
		{
			this.strawberry = strawberry;
		}

		// Token: 0x06000D3B RID: 3387 RVA: 0x0002CCF7 File Offset: 0x0002AEF7
		public override void OnBegin(Level level)
		{
			this.cameraStart = level.Camera.Position;
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000D3C RID: 3388 RVA: 0x0002CD1D File Offset: 0x0002AF1D
		private IEnumerator Cutscene(Level level)
		{
			this.sfx = Audio.Play("event:/game/general/seed_complete_main", this.Position);
			this.snapshot = Audio.CreateSnapshot("snapshot:/music_mains_mute", true);
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				this.cameraStart = entity.CameraTarget;
			}
			foreach (StrawberrySeed strawberrySeed in this.strawberry.Seeds)
			{
				strawberrySeed.OnAllCollected();
			}
			this.strawberry.Depth = -2000002;
			this.strawberry.AddTag(Tags.FrozenUpdate);
			yield return 0.35f;
			base.Tag = (Tags.FrozenUpdate | Tags.HUD);
			level.Frozen = true;
			level.FormationBackdrop.Display = true;
			level.FormationBackdrop.Alpha = 0.5f;
			level.Displacement.Clear();
			level.Displacement.Enabled = false;
			Audio.BusPaused("bus:/gameplay_sfx/ambience", new bool?(true));
			Audio.BusPaused("bus:/gameplay_sfx/char", new bool?(true));
			Audio.BusPaused("bus:/gameplay_sfx/game/general/yes_pause", new bool?(true));
			Audio.BusPaused("bus:/gameplay_sfx/game/chapters", new bool?(true));
			yield return 0.1f;
			this.system = new ParticleSystem(-2000002, 50);
			this.system.Tag = Tags.FrozenUpdate;
			level.Add(this.system);
			float num = 6.2831855f / (float)this.strawberry.Seeds.Count;
			float num2 = 1.5707964f;
			Vector2 vector = Vector2.Zero;
			foreach (StrawberrySeed strawberrySeed2 in this.strawberry.Seeds)
			{
				vector += strawberrySeed2.Position;
			}
			vector /= (float)this.strawberry.Seeds.Count;
			foreach (StrawberrySeed strawberrySeed3 in this.strawberry.Seeds)
			{
				strawberrySeed3.StartSpinAnimation(vector, this.strawberry.Position, num2, 4f);
				num2 -= num;
			}
			Vector2 vector2 = this.strawberry.Position - new Vector2(160f, 90f);
			vector2 = vector2.Clamp((float)level.Bounds.Left, (float)level.Bounds.Top, (float)(level.Bounds.Right - 320), (float)(level.Bounds.Bottom - 180));
			base.Add(new Coroutine(CutsceneEntity.CameraTo(vector2, 3.5f, Ease.CubeInOut, 0f), true));
			yield return 4f;
			Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
			Audio.Play("event:/game/general/seed_complete_berry", this.strawberry.Position);
			foreach (StrawberrySeed strawberrySeed4 in this.strawberry.Seeds)
			{
				strawberrySeed4.StartCombineAnimation(this.strawberry.Position, 0.6f, this.system);
			}
			yield return 0.6f;
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			foreach (StrawberrySeed strawberrySeed5 in this.strawberry.Seeds)
			{
				strawberrySeed5.RemoveSelf();
			}
			this.strawberry.CollectedSeeds();
			yield return 0.5f;
			float dist = (level.Camera.Position - this.cameraStart).Length();
			yield return CutsceneEntity.CameraTo(this.cameraStart, dist / 180f, null, 0f);
			if (dist > 80f)
			{
				yield return 0.25f;
			}
			level.EndCutscene();
			this.OnEnd(level);
			yield break;
		}

		// Token: 0x06000D3D RID: 3389 RVA: 0x0002CD34 File Offset: 0x0002AF34
		public override void OnEnd(Level level)
		{
			if (this.WasSkipped)
			{
				Audio.Stop(this.sfx, true);
			}
			level.OnEndOfFrame += delegate()
			{
				if (this.WasSkipped)
				{
					foreach (StrawberrySeed strawberrySeed in this.strawberry.Seeds)
					{
						strawberrySeed.RemoveSelf();
					}
					this.strawberry.CollectedSeeds();
					level.Camera.Position = this.cameraStart;
				}
				this.strawberry.Depth = -100;
				this.strawberry.RemoveTag(Tags.FrozenUpdate);
				level.Frozen = false;
				level.FormationBackdrop.Display = false;
				level.Displacement.Enabled = true;
			};
			base.RemoveSelf();
		}

		// Token: 0x06000D3E RID: 3390 RVA: 0x0002CD88 File Offset: 0x0002AF88
		private void EndSfx()
		{
			Audio.BusPaused("bus:/gameplay_sfx/ambience", new bool?(false));
			Audio.BusPaused("bus:/gameplay_sfx/char", new bool?(false));
			Audio.BusPaused("bus:/gameplay_sfx/game/general/yes_pause", new bool?(false));
			Audio.BusPaused("bus:/gameplay_sfx/game/chapters", new bool?(false));
			Audio.ReleaseSnapshot(this.snapshot);
		}

		// Token: 0x06000D3F RID: 3391 RVA: 0x0002CDE4 File Offset: 0x0002AFE4
		public override void Removed(Scene scene)
		{
			this.EndSfx();
			base.Removed(scene);
		}

		// Token: 0x06000D40 RID: 3392 RVA: 0x0002CDF3 File Offset: 0x0002AFF3
		public override void SceneEnd(Scene scene)
		{
			this.EndSfx();
			base.SceneEnd(scene);
		}

		// Token: 0x0400087C RID: 2172
		private Strawberry strawberry;

		// Token: 0x0400087D RID: 2173
		private Vector2 cameraStart;

		// Token: 0x0400087E RID: 2174
		private ParticleSystem system;

		// Token: 0x0400087F RID: 2175
		private EventInstance snapshot;

		// Token: 0x04000880 RID: 2176
		private EventInstance sfx;
	}
}
