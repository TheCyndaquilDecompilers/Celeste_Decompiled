using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000150 RID: 336
	public class Cassette : Entity
	{
		// Token: 0x06000C3A RID: 3130 RVA: 0x00027C9C File Offset: 0x00025E9C
		public Cassette(Vector2 position, Vector2[] nodes) : base(position)
		{
			base.Collider = new Hitbox(16f, 16f, -8f, -8f);
			this.nodes = nodes;
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
		}

		// Token: 0x06000C3B RID: 3131 RVA: 0x00027CEF File Offset: 0x00025EEF
		public Cassette(EntityData data, Vector2 offset) : this(data.Position + offset, data.NodesOffset(offset))
		{
		}

		// Token: 0x06000C3C RID: 3132 RVA: 0x00027D0C File Offset: 0x00025F0C
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.IsGhost = SaveData.Instance.Areas[base.SceneAs<Level>().Session.Area.ID].Cassette;
			base.Add(this.sprite = GFX.SpriteBank.Create(this.IsGhost ? "cassetteGhost" : "cassette"));
			this.sprite.Play("idle", false, false);
			base.Add(this.scaleWiggler = Wiggler.Create(0.25f, 4f, delegate(float f)
			{
				this.sprite.Scale = Vector2.One * (1f + f * 0.25f);
			}, false, false));
			base.Add(this.bloom = new BloomPoint(0.25f, 16f));
			base.Add(this.light = new VertexLight(Color.White, 0.4f, 32, 64));
			base.Add(this.hover = new SineWave(0.5f, 0f));
			this.hover.OnUpdate = delegate(float f)
			{
				this.sprite.Y = (this.light.Y = (this.bloom.Y = f * 2f));
			};
			if (this.IsGhost)
			{
				this.sprite.Color = Color.White * 0.8f;
			}
		}

		// Token: 0x06000C3D RID: 3133 RVA: 0x00027E53 File Offset: 0x00026053
		public override void SceneEnd(Scene scene)
		{
			base.SceneEnd(scene);
			Audio.Stop(this.remixSfx, true);
		}

		// Token: 0x06000C3E RID: 3134 RVA: 0x00027E68 File Offset: 0x00026068
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			Audio.Stop(this.remixSfx, true);
		}

		// Token: 0x06000C3F RID: 3135 RVA: 0x00027E80 File Offset: 0x00026080
		public override void Update()
		{
			base.Update();
			if (!this.collecting && base.Scene.OnInterval(0.1f))
			{
				base.SceneAs<Level>().Particles.Emit(Cassette.P_Shine, 1, base.Center, new Vector2(12f, 10f));
			}
		}

		// Token: 0x06000C40 RID: 3136 RVA: 0x00027ED8 File Offset: 0x000260D8
		private void OnPlayer(Player player)
		{
			if (!this.collected)
			{
				if (player != null)
				{
					player.RefillStamina();
				}
				Audio.Play("event:/game/general/cassette_get", this.Position);
				this.collected = true;
				Celeste.Freeze(0.1f);
				base.Add(new Coroutine(this.CollectRoutine(player), true));
			}
		}

		// Token: 0x06000C41 RID: 3137 RVA: 0x00027F2B File Offset: 0x0002612B
		private IEnumerator CollectRoutine(Player player)
		{
			this.collecting = true;
			Level level = base.Scene as Level;
			CassetteBlockManager cbm = base.Scene.Tracker.GetEntity<CassetteBlockManager>();
			level.PauseLock = true;
			level.Frozen = true;
			base.Tag = Tags.FrozenUpdate;
			level.Session.Cassette = true;
			level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(this.nodes[1]));
			level.Session.UpdateLevelStartDashes();
			SaveData.Instance.RegisterCassette(level.Session.Area);
			if (cbm != null)
			{
				cbm.StopBlocks();
			}
			base.Depth = -1000000;
			level.Shake(0.3f);
			level.Flash(Color.White, false);
			level.Displacement.Clear();
			Vector2 camWas = level.Camera.Position;
			Vector2 camTo = (this.Position - new Vector2(160f, 90f)).Clamp((float)(level.Bounds.Left - 64), (float)(level.Bounds.Top - 32), (float)(level.Bounds.Right + 64 - 320), (float)(level.Bounds.Bottom + 32 - 180));
			level.Camera.Position = camTo;
			level.ZoomSnap((this.Position - level.Camera.Position).Clamp(60f, 60f, 260f, 120f), 2f);
			this.sprite.Play("spin", true, false);
			this.sprite.Rate = 2f;
			for (float p = 0f; p < 1.5f; p += Engine.DeltaTime)
			{
				this.sprite.Rate += Engine.DeltaTime * 4f;
				yield return null;
			}
			this.sprite.Rate = 0f;
			this.sprite.SetAnimationFrame(0);
			this.scaleWiggler.Start();
			yield return 0.25f;
			Vector2 from = this.Position;
			Vector2 to = new Vector2(base.X, level.Camera.Top - 16f);
			float duration = 0.4f;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
			{
				this.sprite.Scale.X = MathHelper.Lerp(1f, 0.1f, p);
				this.sprite.Scale.Y = MathHelper.Lerp(1f, 3f, p);
				this.Position = Vector2.Lerp(from, to, Ease.CubeIn(p));
				yield return null;
			}
			this.Visible = false;
			from = default(Vector2);
			to = default(Vector2);
			this.remixSfx = Audio.Play("event:/game/general/cassette_preview", "remix", (float)level.Session.Area.ID);
			Cassette.UnlockedBSide message = new Cassette.UnlockedBSide();
			base.Scene.Add(message);
			yield return message.EaseIn();
			while (!Input.MenuConfirm.Pressed)
			{
				yield return null;
			}
			Audio.SetParameter(this.remixSfx, "end", 1f);
			yield return message.EaseOut();
			message = null;
			duration = 0.25f;
			base.Add(new Coroutine(level.ZoomBack(duration - 0.05f), true));
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
			{
				level.Camera.Position = Vector2.Lerp(camTo, camWas, Ease.SineInOut(p));
				yield return null;
			}
			if (!player.Dead && this.nodes != null && this.nodes.Length >= 2)
			{
				Audio.Play("event:/game/general/cassette_bubblereturn", level.Camera.Position + new Vector2(160f, 90f));
				player.StartCassetteFly(this.nodes[1], this.nodes[0]);
			}
			foreach (SandwichLava sandwichLava in level.Entities.FindAll<SandwichLava>())
			{
				sandwichLava.Leave();
			}
			level.Frozen = false;
			yield return 0.25f;
			if (cbm != null)
			{
				cbm.Finish();
			}
			level.PauseLock = false;
			level.ResetZoom();
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x04000798 RID: 1944
		public static ParticleType P_Shine;

		// Token: 0x04000799 RID: 1945
		public static ParticleType P_Collect;

		// Token: 0x0400079A RID: 1946
		public bool IsGhost;

		// Token: 0x0400079B RID: 1947
		private Sprite sprite;

		// Token: 0x0400079C RID: 1948
		private SineWave hover;

		// Token: 0x0400079D RID: 1949
		private BloomPoint bloom;

		// Token: 0x0400079E RID: 1950
		private VertexLight light;

		// Token: 0x0400079F RID: 1951
		private Wiggler scaleWiggler;

		// Token: 0x040007A0 RID: 1952
		private bool collected;

		// Token: 0x040007A1 RID: 1953
		private Vector2[] nodes;

		// Token: 0x040007A2 RID: 1954
		private EventInstance remixSfx;

		// Token: 0x040007A3 RID: 1955
		private bool collecting;

		// Token: 0x020003DD RID: 989
		private class UnlockedBSide : Entity
		{
			// Token: 0x06001F44 RID: 8004 RVA: 0x000D858C File Offset: 0x000D678C
			public override void Added(Scene scene)
			{
				base.Added(scene);
				base.Tag = (Tags.HUD | Tags.PauseUpdate);
				this.text = ActiveFont.FontSize.AutoNewline(Dialog.Clean("UI_REMIX_UNLOCKED", null), 900);
				base.Depth = -10000;
			}

			// Token: 0x06001F45 RID: 8005 RVA: 0x000D85E6 File Offset: 0x000D67E6
			public IEnumerator EaseIn()
			{
				Scene scene = base.Scene;
				while ((this.alpha += Engine.DeltaTime / 0.5f) < 1f)
				{
					yield return null;
				}
				this.alpha = 1f;
				yield return 1.5f;
				this.waitForKeyPress = true;
				yield break;
			}

			// Token: 0x06001F46 RID: 8006 RVA: 0x000D85F5 File Offset: 0x000D67F5
			public IEnumerator EaseOut()
			{
				this.waitForKeyPress = false;
				while ((this.alpha -= Engine.DeltaTime / 0.5f) > 0f)
				{
					yield return null;
				}
				this.alpha = 0f;
				base.RemoveSelf();
				yield break;
			}

			// Token: 0x06001F47 RID: 8007 RVA: 0x000D8604 File Offset: 0x000D6804
			public override void Update()
			{
				this.timer += Engine.DeltaTime;
				base.Update();
			}

			// Token: 0x06001F48 RID: 8008 RVA: 0x000D8620 File Offset: 0x000D6820
			public override void Render()
			{
				float num = Ease.CubeOut(this.alpha);
				Vector2 value = Celeste.TargetCenter + new Vector2(0f, 64f);
				Vector2 value2 = Vector2.UnitY * 64f * (1f - num);
				Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * num * 0.8f);
				GFX.Gui["collectables/cassette"].DrawJustified(value - value2 + new Vector2(0f, 32f), new Vector2(0.5f, 1f), Color.White * num);
				ActiveFont.Draw(this.text, value + value2, new Vector2(0.5f, 0f), Vector2.One, Color.White * num);
				if (this.waitForKeyPress)
				{
					GFX.Gui["textboxbutton"].DrawCentered(new Vector2(1824f, (float)(984 + ((this.timer % 1f < 0.25f) ? 6 : 0))));
				}
			}

			// Token: 0x04001FD4 RID: 8148
			private float alpha;

			// Token: 0x04001FD5 RID: 8149
			private string text;

			// Token: 0x04001FD6 RID: 8150
			private bool waitForKeyPress;

			// Token: 0x04001FD7 RID: 8151
			private float timer;
		}
	}
}
