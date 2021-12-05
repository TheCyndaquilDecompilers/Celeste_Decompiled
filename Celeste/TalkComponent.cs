using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002BC RID: 700
	public class TalkComponent : Component
	{
		// Token: 0x0600159C RID: 5532 RVA: 0x0007C874 File Offset: 0x0007AA74
		public TalkComponent(Rectangle bounds, Vector2 drawAt, Action<Player> onTalk, TalkComponent.HoverDisplay hoverDisplay = null) : base(true, true)
		{
			this.Bounds = bounds;
			this.DrawAt = drawAt;
			this.OnTalk = onTalk;
			if (hoverDisplay == null)
			{
				this.HoverUI = new TalkComponent.HoverDisplay
				{
					Texture = GFX.Gui["hover/highlight"],
					InputPosition = new Vector2(0f, -75f)
				};
				return;
			}
			this.HoverUI = hoverDisplay;
		}

		// Token: 0x0600159D RID: 5533 RVA: 0x0007C8F0 File Offset: 0x0007AAF0
		public override void Update()
		{
			if (this.UI == null)
			{
				base.Entity.Scene.Add(this.UI = new TalkComponent.TalkComponentUI(this));
			}
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			bool flag = this.disableDelay < 0.05f && entity != null && entity.CollideRect(new Rectangle((int)(base.Entity.X + (float)this.Bounds.X), (int)(base.Entity.Y + (float)this.Bounds.Y), this.Bounds.Width, this.Bounds.Height)) && entity.OnGround(1) && entity.Bottom < base.Entity.Y + (float)this.Bounds.Bottom + 4f && entity.StateMachine.State == 0 && (!this.PlayerMustBeFacing || Math.Abs(entity.X - base.Entity.X) <= 16f || entity.Facing == (Facings)Math.Sign(base.Entity.X - entity.X)) && (TalkComponent.PlayerOver == null || TalkComponent.PlayerOver == this);
			if (flag)
			{
				this.hoverTimer += Engine.DeltaTime;
			}
			else if (this.UI.Display)
			{
				this.hoverTimer = 0f;
			}
			if (TalkComponent.PlayerOver == this && !flag)
			{
				TalkComponent.PlayerOver = null;
			}
			else if (flag)
			{
				TalkComponent.PlayerOver = this;
			}
			if (flag && this.cooldown <= 0f && entity != null && entity.StateMachine == 0 && Input.Talk.Pressed && this.Enabled && !base.Scene.Paused)
			{
				this.cooldown = 0.1f;
				if (this.OnTalk != null)
				{
					this.OnTalk(entity);
				}
			}
			if (flag && entity.StateMachine == 0)
			{
				this.cooldown -= Engine.DeltaTime;
			}
			if (!this.Enabled)
			{
				this.disableDelay += Engine.DeltaTime;
			}
			else
			{
				this.disableDelay = 0f;
			}
			this.UI.Highlighted = (flag && this.hoverTimer > 0.1f);
			base.Update();
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x0007CB55 File Offset: 0x0007AD55
		public override void Removed(Entity entity)
		{
			this.Dispose();
			base.Removed(entity);
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x0007CB64 File Offset: 0x0007AD64
		public override void EntityRemoved(Scene scene)
		{
			this.Dispose();
			base.EntityRemoved(scene);
		}

		// Token: 0x060015A0 RID: 5536 RVA: 0x0007CB73 File Offset: 0x0007AD73
		public override void SceneEnd(Scene scene)
		{
			this.Dispose();
			base.SceneEnd(scene);
		}

		// Token: 0x060015A1 RID: 5537 RVA: 0x0007CB82 File Offset: 0x0007AD82
		private void Dispose()
		{
			if (TalkComponent.PlayerOver == this)
			{
				TalkComponent.PlayerOver = null;
			}
			base.Scene.Remove(this.UI);
			this.UI = null;
		}

		// Token: 0x060015A2 RID: 5538 RVA: 0x0007CBAC File Offset: 0x0007ADAC
		public override void DebugRender(Camera camera)
		{
			base.DebugRender(camera);
			Draw.HollowRect(base.Entity.X + (float)this.Bounds.X, base.Entity.Y + (float)this.Bounds.Y, (float)this.Bounds.Width, (float)this.Bounds.Height, Color.Green);
		}

		// Token: 0x040011B6 RID: 4534
		public static TalkComponent PlayerOver;

		// Token: 0x040011B7 RID: 4535
		public bool Enabled = true;

		// Token: 0x040011B8 RID: 4536
		public Rectangle Bounds;

		// Token: 0x040011B9 RID: 4537
		public Vector2 DrawAt;

		// Token: 0x040011BA RID: 4538
		public Action<Player> OnTalk;

		// Token: 0x040011BB RID: 4539
		public bool PlayerMustBeFacing = true;

		// Token: 0x040011BC RID: 4540
		public TalkComponent.TalkComponentUI UI;

		// Token: 0x040011BD RID: 4541
		public TalkComponent.HoverDisplay HoverUI;

		// Token: 0x040011BE RID: 4542
		private float cooldown;

		// Token: 0x040011BF RID: 4543
		private float hoverTimer;

		// Token: 0x040011C0 RID: 4544
		private float disableDelay;

		// Token: 0x02000648 RID: 1608
		public class HoverDisplay
		{
			// Token: 0x040029F6 RID: 10742
			public MTexture Texture;

			// Token: 0x040029F7 RID: 10743
			public Vector2 InputPosition;

			// Token: 0x040029F8 RID: 10744
			public string SfxIn = "event:/ui/game/hotspot_main_in";

			// Token: 0x040029F9 RID: 10745
			public string SfxOut = "event:/ui/game/hotspot_main_out";
		}

		// Token: 0x02000649 RID: 1609
		public class TalkComponentUI : Entity
		{
			// Token: 0x170005D3 RID: 1491
			// (get) Token: 0x06002AEA RID: 10986 RVA: 0x0011246E File Offset: 0x0011066E
			// (set) Token: 0x06002AEB RID: 10987 RVA: 0x00112478 File Offset: 0x00110678
			public bool Highlighted
			{
				get
				{
					return this.highlighted;
				}
				set
				{
					if (this.highlighted != value & this.Display)
					{
						this.highlighted = value;
						if (this.highlighted)
						{
							Audio.Play(this.Handler.HoverUI.SfxIn);
						}
						else
						{
							Audio.Play(this.Handler.HoverUI.SfxOut);
						}
						this.wiggler.Start();
					}
				}
			}

			// Token: 0x170005D4 RID: 1492
			// (get) Token: 0x06002AEC RID: 10988 RVA: 0x001124E4 File Offset: 0x001106E4
			public bool Display
			{
				get
				{
					if (!this.Handler.Enabled)
					{
						return false;
					}
					if (base.Scene == null)
					{
						return false;
					}
					if (base.Scene.Tracker.GetEntity<Textbox>() != null)
					{
						return false;
					}
					Player entity = base.Scene.Tracker.GetEntity<Player>();
					if (entity == null || entity.StateMachine.State == 11)
					{
						return false;
					}
					Level level = base.Scene as Level;
					return !level.FrozenOrPaused && level.RetryPlayerCorpse == null;
				}
			}

			// Token: 0x06002AED RID: 10989 RVA: 0x00112564 File Offset: 0x00110764
			public TalkComponentUI(TalkComponent handler)
			{
				this.Handler = handler;
				base.AddTag(Tags.HUD | Tags.Persistent);
				base.Add(this.wiggler = Wiggler.Create(0.25f, 4f, null, false, false));
			}

			// Token: 0x06002AEE RID: 10990 RVA: 0x001125DF File Offset: 0x001107DF
			public override void Awake(Scene scene)
			{
				base.Awake(scene);
				if (this.Handler.Entity == null || base.Scene.CollideCheck<FakeWall>(this.Handler.Entity.Position))
				{
					this.alpha = 0f;
				}
			}

			// Token: 0x06002AEF RID: 10991 RVA: 0x00112620 File Offset: 0x00110820
			public override void Update()
			{
				this.timer += Engine.DeltaTime;
				this.slide = Calc.Approach(this.slide, (float)(this.Display ? 1 : 0), Engine.DeltaTime * 4f);
				if (this.alpha < 1f && this.Handler.Entity != null && !base.Scene.CollideCheck<FakeWall>(this.Handler.Entity.Position))
				{
					this.alpha = Calc.Approach(this.alpha, 1f, 2f * Engine.DeltaTime);
				}
				base.Update();
			}

			// Token: 0x06002AF0 RID: 10992 RVA: 0x001126C8 File Offset: 0x001108C8
			public override void Render()
			{
				Level level = base.Scene as Level;
				if (level.FrozenOrPaused)
				{
					return;
				}
				if (this.slide > 0f && this.Handler.Entity != null)
				{
					Vector2 value = level.Camera.Position.Floor();
					Vector2 vector = this.Handler.Entity.Position + this.Handler.DrawAt - value;
					if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
					{
						vector.X = 320f - vector.X;
					}
					vector.X *= 6f;
					vector.Y *= 6f;
					vector.Y += (float)Math.Sin((double)(this.timer * 4f)) * 12f + 64f * (1f - Ease.CubeOut(this.slide));
					float num;
					if (this.Highlighted)
					{
						num = 1f - this.wiggler.Value * 0.5f;
					}
					else
					{
						num = 1f + this.wiggler.Value * 0.5f;
					}
					float scale = Ease.CubeInOut(this.slide) * this.alpha;
					Color value2 = this.lineColor * scale;
					if (this.Highlighted)
					{
						this.Handler.HoverUI.Texture.DrawJustified(vector, new Vector2(0.5f, 1f), value2 * this.alpha, num);
					}
					else
					{
						GFX.Gui["hover/idle"].DrawJustified(vector, new Vector2(0.5f, 1f), value2 * this.alpha, num);
					}
					if (this.Highlighted)
					{
						Vector2 position = vector + this.Handler.HoverUI.InputPosition * num;
						if (Input.GuiInputController(Input.PrefixMode.Latest))
						{
							Input.GuiButton(Input.Talk, Input.PrefixMode.Latest, "controls/keyboard/oemquestion").DrawJustified(position, new Vector2(0.5f), Color.White * scale, num);
							return;
						}
						ActiveFont.DrawOutline(Input.FirstKey(Input.Talk).ToString().ToUpper(), position, new Vector2(0.5f), new Vector2(num), Color.White * scale, 2f, Color.Black);
					}
				}
			}

			// Token: 0x040029FA RID: 10746
			public TalkComponent Handler;

			// Token: 0x040029FB RID: 10747
			private bool highlighted;

			// Token: 0x040029FC RID: 10748
			private float slide;

			// Token: 0x040029FD RID: 10749
			private float timer;

			// Token: 0x040029FE RID: 10750
			private Wiggler wiggler;

			// Token: 0x040029FF RID: 10751
			private float alpha = 1f;

			// Token: 0x04002A00 RID: 10752
			private Color lineColor = new Color(1f, 1f, 1f);
		}
	}
}
