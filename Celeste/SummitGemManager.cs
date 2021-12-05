using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001CC RID: 460
	public class SummitGemManager : Entity
	{
		// Token: 0x06000FB0 RID: 4016 RVA: 0x00042664 File Offset: 0x00040864
		public SummitGemManager(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Depth = -10010;
			int num = 0;
			foreach (Vector2 position in data.NodesOffset(offset))
			{
				SummitGemManager.Gem item = new SummitGemManager.Gem(num, position);
				this.gems.Add(item);
				num++;
			}
			base.Add(new Coroutine(this.Routine(), true));
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x000426E8 File Offset: 0x000408E8
		public override void Awake(Scene scene)
		{
			foreach (SummitGemManager.Gem entity in this.gems)
			{
				scene.Add(entity);
			}
			base.Awake(scene);
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x00042744 File Offset: 0x00040944
		private IEnumerator Routine()
		{
			Level level = base.Scene as Level;
			if (level.Session.HeartGem)
			{
				foreach (SummitGemManager.Gem gem2 in this.gems)
				{
					gem2.Sprite.RemoveSelf();
				}
				this.gems.Clear();
				yield break;
			}
			for (;;)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && (entity.Position - this.Position).Length() < 64f)
				{
					break;
				}
				yield return null;
			}
			yield return 0.5f;
			bool alreadyHasHeart = level.Session.OldStats.Modes[0].HeartGem;
			int broken = 0;
			int index = 0;
			foreach (SummitGemManager.Gem gem in this.gems)
			{
				bool flag = level.Session.SummitGems[index];
				if (!alreadyHasHeart)
				{
					flag |= (SaveData.Instance.SummitGems != null && SaveData.Instance.SummitGems[index]);
				}
				int num;
				if (flag)
				{
					if (index == 0)
					{
						Audio.Play("event:/game/07_summit/gem_unlock_1", gem.Position);
					}
					else if (index == 1)
					{
						Audio.Play("event:/game/07_summit/gem_unlock_2", gem.Position);
					}
					else if (index == 2)
					{
						Audio.Play("event:/game/07_summit/gem_unlock_3", gem.Position);
					}
					else if (index == 3)
					{
						Audio.Play("event:/game/07_summit/gem_unlock_4", gem.Position);
					}
					else if (index == 4)
					{
						Audio.Play("event:/game/07_summit/gem_unlock_5", gem.Position);
					}
					else if (index == 5)
					{
						Audio.Play("event:/game/07_summit/gem_unlock_6", gem.Position);
					}
					gem.Sprite.Play("spin", false, false);
					while (gem.Sprite.CurrentAnimationID == "spin")
					{
						gem.Bloom.Alpha = Calc.Approach(gem.Bloom.Alpha, 1f, Engine.DeltaTime * 3f);
						if (gem.Bloom.Alpha > 0.5f)
						{
							gem.Shake = Calc.Random.ShakeVector();
						}
						gem.Sprite.Y -= Engine.DeltaTime * 8f;
						gem.Sprite.Scale = Vector2.One * (1f + gem.Bloom.Alpha * 0.1f);
						yield return null;
					}
					yield return 0.2f;
					level.Shake(0.3f);
					Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
					for (int i = 0; i < 20; i++)
					{
						level.ParticlesFG.Emit(SummitGem.P_Shatter, gem.Position + new Vector2((float)Calc.Random.Range(-8, 8), (float)Calc.Random.Range(-8, 8)), SummitGem.GemColors[index], Calc.Random.NextFloat(6.2831855f));
					}
					num = broken;
					broken = num + 1;
					gem.Bloom.RemoveSelf();
					gem.Sprite.RemoveSelf();
					yield return 0.25f;
				}
				num = index;
				index = num + 1;
				gem = null;
			}
			List<SummitGemManager.Gem>.Enumerator enumerator2 = default(List<SummitGemManager.Gem>.Enumerator);
			if (broken >= 6)
			{
				HeartGem heart = base.Scene.Entities.FindFirst<HeartGem>();
				if (heart != null)
				{
					Audio.Play("event:/game/07_summit/gem_unlock_complete", heart.Position);
					yield return 0.1f;
					Vector2 from = heart.Position;
					float p = 0f;
					while (p < 1f && heart.Scene != null)
					{
						heart.Position = Vector2.Lerp(from, this.Position + new Vector2(0f, -16f), Ease.CubeOut(p));
						yield return null;
						p += Engine.DeltaTime;
					}
					from = default(Vector2);
				}
				heart = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x04000B1E RID: 2846
		private List<SummitGemManager.Gem> gems = new List<SummitGemManager.Gem>();

		// Token: 0x020004D0 RID: 1232
		private class Gem : Entity
		{
			// Token: 0x06002418 RID: 9240 RVA: 0x000F2114 File Offset: 0x000F0314
			public Gem(int index, Vector2 position) : base(position)
			{
				base.Depth = -10010;
				base.Add(this.Bg = new Image(GFX.Game["collectables/summitgems/" + index + "/bg"]));
				base.Add(this.Sprite = new Sprite(GFX.Game, "collectables/summitgems/" + index + "/gem"));
				base.Add(this.Bloom = new BloomPoint(0f, 20f));
				this.Sprite.AddLoop("idle", "", 0.05f, new int[1]);
				this.Sprite.Add("spin", "", 0.05f, "idle");
				this.Sprite.Play("idle", false, false);
				this.Sprite.CenterOrigin();
				this.Bg.CenterOrigin();
			}

			// Token: 0x06002419 RID: 9241 RVA: 0x000F221B File Offset: 0x000F041B
			public override void Update()
			{
				this.Bloom.Position = this.Sprite.Position;
				base.Update();
			}

			// Token: 0x0600241A RID: 9242 RVA: 0x000F223C File Offset: 0x000F043C
			public override void Render()
			{
				Vector2 position = this.Sprite.Position;
				this.Sprite.Position += this.Shake;
				base.Render();
				this.Sprite.Position = position;
			}

			// Token: 0x040023B8 RID: 9144
			public Vector2 Shake;

			// Token: 0x040023B9 RID: 9145
			public Sprite Sprite;

			// Token: 0x040023BA RID: 9146
			public Image Bg;

			// Token: 0x040023BB RID: 9147
			public BloomPoint Bloom;
		}
	}
}
