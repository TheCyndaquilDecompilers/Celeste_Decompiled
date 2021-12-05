using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000219 RID: 537
	[Tracked(false)]
	public class TrailManager : Entity
	{
		// Token: 0x06001162 RID: 4450 RVA: 0x00055778 File Offset: 0x00053978
		public TrailManager()
		{
			base.Tag = Tags.Global;
			base.Depth = 10;
			base.Add(new BeforeRenderHook(new Action(this.BeforeRender)));
			base.Add(new MirrorReflection());
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x000557D2 File Offset: 0x000539D2
		public override void Removed(Scene scene)
		{
			this.Dispose();
			base.Removed(scene);
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x000557E1 File Offset: 0x000539E1
		public override void SceneEnd(Scene scene)
		{
			this.Dispose();
			base.SceneEnd(scene);
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x000557F0 File Offset: 0x000539F0
		private void Dispose()
		{
			if (this.buffer != null)
			{
				this.buffer.Dispose();
			}
			this.buffer = null;
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x0005580C File Offset: 0x00053A0C
		private void BeforeRender()
		{
			if (!this.dirty)
			{
				return;
			}
			if (this.buffer == null)
			{
				this.buffer = VirtualContent.CreateRenderTarget("trail-manager", 512, 512, false, true, 0);
			}
			Engine.Graphics.GraphicsDevice.SetRenderTarget(this.buffer);
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, LightingRenderer.OccludeBlendState);
			for (int i = 0; i < this.snapshots.Length; i++)
			{
				if (this.snapshots[i] != null && !this.snapshots[i].Drawn)
				{
					Draw.Rect((float)(i % 8 * 64), (float)(i / 8 * 64), 64f, 64f, Color.Transparent);
				}
			}
			Draw.SpriteBatch.End();
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, RasterizerState.CullNone);
			for (int j = 0; j < this.snapshots.Length; j++)
			{
				if (this.snapshots[j] != null && !this.snapshots[j].Drawn)
				{
					TrailManager.Snapshot snapshot = this.snapshots[j];
					Vector2 value = new Vector2(((float)(j % 8) + 0.5f) * 64f, ((float)(j / 8) + 0.5f) * 64f) - snapshot.Position;
					if (snapshot.Hair != null)
					{
						for (int k = 0; k < snapshot.Hair.Nodes.Count; k++)
						{
							List<Vector2> nodes = snapshot.Hair.Nodes;
							int index = k;
							nodes[index] += value;
						}
						snapshot.Hair.Render();
						for (int l = 0; l < snapshot.Hair.Nodes.Count; l++)
						{
							List<Vector2> nodes = snapshot.Hair.Nodes;
							int index = l;
							nodes[index] -= value;
						}
					}
					Vector2 scale = snapshot.Sprite.Scale;
					snapshot.Sprite.Scale = snapshot.SpriteScale;
					snapshot.Sprite.Position += value;
					snapshot.Sprite.Render();
					snapshot.Sprite.Scale = scale;
					snapshot.Sprite.Position -= value;
					snapshot.Drawn = true;
				}
			}
			Draw.SpriteBatch.End();
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, TrailManager.MaxBlendState);
			Draw.Rect(0f, 0f, (float)this.buffer.Width, (float)this.buffer.Height, new Color(1f, 1f, 1f, 1f));
			Draw.SpriteBatch.End();
			this.dirty = false;
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x00055AD4 File Offset: 0x00053CD4
		public static void Add(Entity entity, Color color, float duration = 1f, bool frozenUpdate = false, bool useRawDeltaTime = false)
		{
			Image image = entity.Get<PlayerSprite>();
			if (image == null)
			{
				image = entity.Get<Sprite>();
			}
			PlayerHair hair = entity.Get<PlayerHair>();
			TrailManager.Add(entity.Position, image, hair, image.Scale, color, entity.Depth + 1, duration, frozenUpdate, useRawDeltaTime);
		}

		// Token: 0x06001168 RID: 4456 RVA: 0x00055B1C File Offset: 0x00053D1C
		public static void Add(Entity entity, Vector2 scale, Color color, float duration = 1f)
		{
			Image image = entity.Get<PlayerSprite>();
			if (image == null)
			{
				image = entity.Get<Sprite>();
			}
			PlayerHair hair = entity.Get<PlayerHair>();
			TrailManager.Add(entity.Position, image, hair, scale, color, entity.Depth + 1, duration, false, false);
		}

		// Token: 0x06001169 RID: 4457 RVA: 0x00055B5C File Offset: 0x00053D5C
		public static void Add(Vector2 position, Image image, Color color, int depth, float duration = 1f)
		{
			TrailManager.Add(position, image, null, image.Scale, color, depth, duration, false, false);
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x00055B80 File Offset: 0x00053D80
		public static TrailManager.Snapshot Add(Vector2 position, Image sprite, PlayerHair hair, Vector2 scale, Color color, int depth, float duration = 1f, bool frozenUpdate = false, bool useRawDeltaTime = false)
		{
			TrailManager trailManager = Engine.Scene.Tracker.GetEntity<TrailManager>();
			if (trailManager == null)
			{
				trailManager = new TrailManager();
				Engine.Scene.Add(trailManager);
			}
			for (int i = 0; i < trailManager.snapshots.Length; i++)
			{
				if (trailManager.snapshots[i] == null)
				{
					TrailManager.Snapshot snapshot = Engine.Pooler.Create<TrailManager.Snapshot>();
					snapshot.Init(trailManager, i, position, sprite, hair, scale, color, duration, depth, frozenUpdate, useRawDeltaTime);
					trailManager.snapshots[i] = snapshot;
					trailManager.dirty = true;
					Engine.Scene.Add(snapshot);
					return snapshot;
				}
			}
			return null;
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x00055C10 File Offset: 0x00053E10
		public static void Clear()
		{
			TrailManager entity = Engine.Scene.Tracker.GetEntity<TrailManager>();
			if (entity != null)
			{
				for (int i = 0; i < entity.snapshots.Length; i++)
				{
					if (entity.snapshots[i] != null)
					{
						entity.snapshots[i].RemoveSelf();
					}
				}
			}
		}

		// Token: 0x04000CFA RID: 3322
		private static BlendState MaxBlendState = new BlendState
		{
			ColorSourceBlend = Blend.DestinationAlpha,
			AlphaSourceBlend = Blend.DestinationAlpha
		};

		// Token: 0x04000CFB RID: 3323
		private const int size = 64;

		// Token: 0x04000CFC RID: 3324
		private const int columns = 8;

		// Token: 0x04000CFD RID: 3325
		private const int rows = 8;

		// Token: 0x04000CFE RID: 3326
		private TrailManager.Snapshot[] snapshots = new TrailManager.Snapshot[64];

		// Token: 0x04000CFF RID: 3327
		private VirtualRenderTarget buffer;

		// Token: 0x04000D00 RID: 3328
		private bool dirty;

		// Token: 0x02000529 RID: 1321
		[Pooled]
		[Tracked(false)]
		public class Snapshot : Entity
		{
			// Token: 0x0600254D RID: 9549 RVA: 0x000F804E File Offset: 0x000F624E
			public Snapshot()
			{
				base.Add(new MirrorReflection());
			}

			// Token: 0x0600254E RID: 9550 RVA: 0x000F8064 File Offset: 0x000F6264
			public void Init(TrailManager manager, int index, Vector2 position, Image sprite, PlayerHair hair, Vector2 scale, Color color, float duration, int depth, bool frozenUpdate, bool useRawDeltaTime)
			{
				base.Tag = Tags.Global;
				if (frozenUpdate)
				{
					base.Tag |= Tags.FrozenUpdate;
				}
				this.Manager = manager;
				this.Index = index;
				this.Position = position;
				this.Sprite = sprite;
				this.SpriteScale = scale;
				this.Hair = hair;
				this.Color = color;
				this.Percent = 0f;
				this.Duration = duration;
				base.Depth = depth;
				this.Drawn = false;
				this.UseRawDeltaTime = useRawDeltaTime;
			}

			// Token: 0x0600254F RID: 9551 RVA: 0x000F80FC File Offset: 0x000F62FC
			public override void Update()
			{
				if (this.Duration <= 0f)
				{
					if (this.Drawn)
					{
						base.RemoveSelf();
						return;
					}
				}
				else
				{
					if (this.Percent >= 1f)
					{
						base.RemoveSelf();
					}
					this.Percent += (this.UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime) / this.Duration;
				}
			}

			// Token: 0x06002550 RID: 9552 RVA: 0x000F8160 File Offset: 0x000F6360
			public override void Render()
			{
				VirtualRenderTarget buffer = this.Manager.buffer;
				Rectangle value = new Rectangle(this.Index % 8 * 64, this.Index / 8 * 64, 64, 64);
				float scale = (this.Duration > 0f) ? (0.75f * (1f - Ease.CubeOut(this.Percent))) : 1f;
				if (buffer != null)
				{
					Draw.SpriteBatch.Draw(buffer, this.Position, new Rectangle?(value), this.Color * scale, 0f, new Vector2(64f, 64f) * 0.5f, Vector2.One, SpriteEffects.None, 0f);
				}
			}

			// Token: 0x06002551 RID: 9553 RVA: 0x000F821F File Offset: 0x000F641F
			public override void Removed(Scene scene)
			{
				if (this.Manager != null)
				{
					this.Manager.snapshots[this.Index] = null;
				}
				base.Removed(scene);
			}

			// Token: 0x0400254C RID: 9548
			public TrailManager Manager;

			// Token: 0x0400254D RID: 9549
			public Image Sprite;

			// Token: 0x0400254E RID: 9550
			public Vector2 SpriteScale;

			// Token: 0x0400254F RID: 9551
			public PlayerHair Hair;

			// Token: 0x04002550 RID: 9552
			public int Index;

			// Token: 0x04002551 RID: 9553
			public Color Color;

			// Token: 0x04002552 RID: 9554
			public float Percent;

			// Token: 0x04002553 RID: 9555
			public float Duration;

			// Token: 0x04002554 RID: 9556
			public bool Drawn;

			// Token: 0x04002555 RID: 9557
			public bool UseRawDeltaTime;
		}
	}
}
