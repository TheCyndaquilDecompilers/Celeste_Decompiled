using System;
using System.Collections;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000165 RID: 357
	public class CS03_Memo : CutsceneEntity
	{
		// Token: 0x06000CAD RID: 3245 RVA: 0x0002AACB File Offset: 0x00028CCB
		public CS03_Memo(Player player) : base(true, false)
		{
			this.player = player;
		}

		// Token: 0x06000CAE RID: 3246 RVA: 0x0002AADC File Offset: 0x00028CDC
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Routine(), true));
		}

		// Token: 0x06000CAF RID: 3247 RVA: 0x0002AAF0 File Offset: 0x00028CF0
		private IEnumerator Routine()
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			if (!this.Level.Session.GetFlag("memo_read"))
			{
				yield return Textbox.Say("ch3_memo_opening", new Func<IEnumerator>[0]);
				yield return 0.1f;
			}
			this.memo = new CS03_Memo.MemoPage();
			base.Scene.Add(this.memo);
			yield return this.memo.EaseIn();
			yield return this.memo.Wait();
			yield return this.memo.EaseOut();
			this.memo = null;
			base.EndCutscene(this.Level, true);
			yield break;
		}

		// Token: 0x06000CB0 RID: 3248 RVA: 0x0002AB00 File Offset: 0x00028D00
		public override void OnEnd(Level level)
		{
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			level.Session.SetFlag("memo_read", true);
			if (this.memo != null)
			{
				this.memo.RemoveSelf();
			}
		}

		// Token: 0x04000810 RID: 2064
		private const string ReadOnceFlag = "memo_read";

		// Token: 0x04000811 RID: 2065
		private Player player;

		// Token: 0x04000812 RID: 2066
		private CS03_Memo.MemoPage memo;

		// Token: 0x020003FC RID: 1020
		private class MemoPage : Entity
		{
			// Token: 0x06001FE2 RID: 8162 RVA: 0x000DBB4C File Offset: 0x000D9D4C
			public MemoPage()
			{
				base.Tag = Tags.HUD;
				this.atlas = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Memo"), Atlas.AtlasDataFormat.Packer);
				this.paper = this.atlas["memo"];
				if (this.atlas.Has("title_" + Settings.Instance.Language))
				{
					this.title = this.atlas["title_" + Settings.Instance.Language];
				}
				else
				{
					this.title = this.atlas["title_english"];
				}
				float num = (float)this.paper.Width * 1.5f - 120f;
				this.text = FancyText.Parse(Dialog.Get("CH3_MEMO", null), (int)(num / 0.75f), -1, 1f, new Color?(Color.Black * 0.6f), null);
				float num2 = this.text.WidestLine() * 0.75f;
				if (num2 > num)
				{
					this.textDownscale = num / num2;
				}
				base.Add(new BeforeRenderHook(new Action(this.BeforeRender)));
			}

			// Token: 0x06001FE3 RID: 8163 RVA: 0x000DBCAC File Offset: 0x000D9EAC
			public IEnumerator EaseIn()
			{
				Audio.Play("event:/game/03_resort/memo_in");
				Vector2 from = new Vector2((float)(Engine.Width / 2), (float)(Engine.Height + 100));
				Vector2 to = new Vector2((float)(Engine.Width / 2), (float)(Engine.Height / 2 - 150));
				float rFrom = -0.1f;
				float rTo = 0.05f;
				for (float p = 0f; p < 1f; p += Engine.DeltaTime)
				{
					this.Position = from + (to - from) * Ease.CubeOut(p);
					this.alpha = Ease.CubeOut(p);
					this.rotation = rFrom + (rTo - rFrom) * Ease.CubeOut(p);
					yield return null;
				}
				yield break;
			}

			// Token: 0x06001FE4 RID: 8164 RVA: 0x000DBCBB File Offset: 0x000D9EBB
			public IEnumerator Wait()
			{
				float start = this.Position.Y;
				int index = 0;
				while (!Input.MenuCancel.Pressed)
				{
					float num = start - (float)(index * 400);
					this.Position.Y = this.Position.Y + (num - this.Position.Y) * (1f - (float)Math.Pow(0.009999999776482582, (double)Engine.DeltaTime));
					if (Input.MenuUp.Pressed && index > 0)
					{
						int num2 = index;
						index = num2 - 1;
					}
					else if (index < 2)
					{
						if ((Input.MenuDown.Pressed && !Input.MenuDown.Repeating) || Input.MenuConfirm.Pressed)
						{
							int num2 = index;
							index = num2 + 1;
						}
					}
					else if (Input.MenuConfirm.Pressed)
					{
						break;
					}
					yield return null;
				}
				Audio.Play("event:/ui/main/button_lowkey");
				yield break;
			}

			// Token: 0x06001FE5 RID: 8165 RVA: 0x000DBCCA File Offset: 0x000D9ECA
			public IEnumerator EaseOut()
			{
				Audio.Play("event:/game/03_resort/memo_out");
				this.easingOut = true;
				Vector2 from = this.Position;
				Vector2 to = new Vector2((float)(Engine.Width / 2), (float)(-(float)this.target.Height));
				float rFrom = this.rotation;
				float rTo = this.rotation + 0.1f;
				for (float p = 0f; p < 1f; p += Engine.DeltaTime * 1.5f)
				{
					this.Position = from + (to - from) * Ease.CubeIn(p);
					this.alpha = 1f - Ease.CubeIn(p);
					this.rotation = rFrom + (rTo - rFrom) * Ease.CubeIn(p);
					yield return null;
				}
				base.RemoveSelf();
				yield break;
			}

			// Token: 0x06001FE6 RID: 8166 RVA: 0x000DBCDC File Offset: 0x000D9EDC
			public void BeforeRender()
			{
				if (this.target == null)
				{
					this.target = VirtualContent.CreateRenderTarget("oshiro-memo", (int)((float)this.paper.Width * 1.5f), (int)((float)this.paper.Height * 1.5f), false, true, 0);
				}
				Engine.Graphics.GraphicsDevice.SetRenderTarget(this.target);
				Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				this.paper.Draw(Vector2.Zero, Vector2.Zero, Color.White, 1.5f);
				this.title.Draw(Vector2.Zero, Vector2.Zero, Color.White, 1.5f);
				this.text.Draw(new Vector2((float)this.paper.Width * 1.5f / 2f, 210f), new Vector2(0.5f, 0f), Vector2.One * 0.75f * this.textDownscale, 1f, 0, int.MaxValue);
				Draw.SpriteBatch.End();
			}

			// Token: 0x06001FE7 RID: 8167 RVA: 0x000DBE11 File Offset: 0x000DA011
			public override void Removed(Scene scene)
			{
				if (this.target != null)
				{
					this.target.Dispose();
				}
				this.target = null;
				this.atlas.Dispose();
				base.Removed(scene);
			}

			// Token: 0x06001FE8 RID: 8168 RVA: 0x000DBE3F File Offset: 0x000DA03F
			public override void SceneEnd(Scene scene)
			{
				if (this.target != null)
				{
					this.target.Dispose();
				}
				this.target = null;
				this.atlas.Dispose();
				base.SceneEnd(scene);
			}

			// Token: 0x06001FE9 RID: 8169 RVA: 0x000DBE6D File Offset: 0x000DA06D
			public override void Update()
			{
				this.timer += Engine.DeltaTime;
				base.Update();
			}

			// Token: 0x06001FEA RID: 8170 RVA: 0x000DBE88 File Offset: 0x000DA088
			public override void Render()
			{
				Level level = base.Scene as Level;
				if (level != null && (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene))
				{
					return;
				}
				if (this.target != null)
				{
					Draw.SpriteBatch.Draw(this.target, this.Position, new Rectangle?(this.target.Bounds), Color.White * this.alpha, this.rotation, new Vector2((float)this.target.Width, 0f) / 2f, this.scale, SpriteEffects.None, 0f);
					if (!this.easingOut)
					{
						GFX.Gui["textboxbutton"].DrawCentered(this.Position + new Vector2((float)(this.target.Width / 2 + 40), (float)(this.target.Height + ((this.timer % 1f < 0.25f) ? 6 : 0))));
					}
				}
			}

			// Token: 0x04002058 RID: 8280
			private const float TextScale = 0.75f;

			// Token: 0x04002059 RID: 8281
			private const float PaperScale = 1.5f;

			// Token: 0x0400205A RID: 8282
			private Atlas atlas;

			// Token: 0x0400205B RID: 8283
			private MTexture paper;

			// Token: 0x0400205C RID: 8284
			private MTexture title;

			// Token: 0x0400205D RID: 8285
			private VirtualRenderTarget target;

			// Token: 0x0400205E RID: 8286
			private FancyText.Text text;

			// Token: 0x0400205F RID: 8287
			private float textDownscale = 1f;

			// Token: 0x04002060 RID: 8288
			private float alpha = 1f;

			// Token: 0x04002061 RID: 8289
			private float scale = 1f;

			// Token: 0x04002062 RID: 8290
			private float rotation;

			// Token: 0x04002063 RID: 8291
			private float timer;

			// Token: 0x04002064 RID: 8292
			private bool easingOut;
		}
	}
}
