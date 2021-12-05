using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000344 RID: 836
	public class IntroCrusher : Solid
	{
		// Token: 0x06001A56 RID: 6742 RVA: 0x000A954C File Offset: 0x000A774C
		public IntroCrusher(Vector2 position, int width, int height, Vector2 node) : base(position, (float)width, (float)height, true)
		{
			this.start = position;
			this.end = node;
			base.Depth = -10501;
			this.SurfaceSoundIndex = 4;
			base.Add(this.tilegrid = GFX.FGAutotiler.GenerateBox('3', width / 8, height / 8).TileGrid);
			base.Add(this.shakingSfx = new SoundSource());
		}

		// Token: 0x06001A57 RID: 6743 RVA: 0x000A95C0 File Offset: 0x000A77C0
		public IntroCrusher(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height, data.Nodes[0] + offset)
		{
		}

		// Token: 0x06001A58 RID: 6744 RVA: 0x000A95F4 File Offset: 0x000A77F4
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (base.SceneAs<Level>().Session.GetLevelFlag("1") || base.SceneAs<Level>().Session.GetLevelFlag("0b"))
			{
				this.Position = this.end;
				return;
			}
			base.Add(new Coroutine(this.Sequence(), true));
		}

		// Token: 0x06001A59 RID: 6745 RVA: 0x000A9655 File Offset: 0x000A7855
		public override void Update()
		{
			this.tilegrid.Position = this.shake;
			base.Update();
		}

		// Token: 0x06001A5A RID: 6746 RVA: 0x000A966E File Offset: 0x000A786E
		private IEnumerator Sequence()
		{
			Player entity;
			do
			{
				yield return null;
				entity = base.Scene.Tracker.GetEntity<Player>();
			}
			while (entity == null || entity.X < base.X + 30f || entity.X > base.Right + 8f);
			this.shakingSfx.Play("event:/game/00_prologue/fallblock_first_shake", null, 0f);
			float time = 1.2f;
			Shaker shaker = new Shaker(time, true, delegate(Vector2 v)
			{
				this.shake = v;
			});
			base.Add(shaker);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			while (time > 0f)
			{
				Player entity2 = base.Scene.Tracker.GetEntity<Player>();
				if (entity2 != null && (entity2.X >= base.X + base.Width - 8f || entity2.X < base.X + 28f))
				{
					shaker.RemoveSelf();
					break;
				}
				yield return null;
				time -= Engine.DeltaTime;
			}
			shaker = null;
			int num = 2;
			while ((float)num < base.Width)
			{
				base.SceneAs<Level>().Particles.Emit(FallingBlock.P_FallDustA, 2, new Vector2(base.X + (float)num, base.Y), Vector2.One * 4f, 1.5707964f);
				base.SceneAs<Level>().Particles.Emit(FallingBlock.P_FallDustB, 2, new Vector2(base.X + (float)num, base.Y), Vector2.One * 4f);
				num += 4;
			}
			this.shakingSfx.Param("release", 1f);
			time = 0f;
			do
			{
				yield return null;
				time = Calc.Approach(time, 1f, 2f * Engine.DeltaTime);
				base.MoveTo(Vector2.Lerp(this.start, this.end, Ease.CubeIn(time)));
			}
			while (time < 1f);
			int num2 = 0;
			while ((float)num2 <= base.Width)
			{
				base.SceneAs<Level>().ParticlesFG.Emit(FallingBlock.P_FallDustA, 1, new Vector2(base.X + (float)num2, base.Bottom), Vector2.One * 4f, -1.5707964f);
				float direction;
				if ((float)num2 < base.Width / 2f)
				{
					direction = 3.1415927f;
				}
				else
				{
					direction = 0f;
				}
				base.SceneAs<Level>().ParticlesFG.Emit(FallingBlock.P_LandDust, 1, new Vector2(base.X + (float)num2, base.Bottom), Vector2.One * 4f, direction);
				num2 += 4;
			}
			this.shakingSfx.Stop(true);
			Audio.Play("event:/game/00_prologue/fallblock_first_impact", this.Position);
			base.SceneAs<Level>().Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			base.Add(new Shaker(0.25f, true, delegate(Vector2 v)
			{
				this.shake = v;
			}));
			yield break;
		}

		// Token: 0x040016EE RID: 5870
		private Vector2 shake;

		// Token: 0x040016EF RID: 5871
		private Vector2 start;

		// Token: 0x040016F0 RID: 5872
		private Vector2 end;

		// Token: 0x040016F1 RID: 5873
		private TileGrid tilegrid;

		// Token: 0x040016F2 RID: 5874
		private SoundSource shakingSfx;
	}
}
