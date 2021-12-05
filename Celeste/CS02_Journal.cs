using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200016F RID: 367
	public class CS02_Journal : CutsceneEntity
	{
		// Token: 0x06000D10 RID: 3344 RVA: 0x0002C515 File Offset: 0x0002A715
		public CS02_Journal(Player player) : base(true, false)
		{
			this.player = player;
		}

		// Token: 0x06000D11 RID: 3345 RVA: 0x0002C526 File Offset: 0x0002A726
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Routine(), true));
		}

		// Token: 0x06000D12 RID: 3346 RVA: 0x0002C53A File Offset: 0x0002A73A
		private IEnumerator Routine()
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			if (!this.Level.Session.GetFlag("poem_read"))
			{
				yield return Textbox.Say("ch2_journal", new Func<IEnumerator>[0]);
				yield return 0.1f;
			}
			this.poem = new CS02_Journal.PoemPage();
			base.Scene.Add(this.poem);
			yield return this.poem.EaseIn();
			while (!Input.MenuConfirm.Pressed)
			{
				yield return null;
			}
			Audio.Play("event:/ui/main/button_lowkey");
			yield return this.poem.EaseOut();
			this.poem = null;
			base.EndCutscene(this.Level, true);
			yield break;
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x0002C54C File Offset: 0x0002A74C
		public override void OnEnd(Level level)
		{
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			level.Session.SetFlag("poem_read", true);
			if (this.poem != null)
			{
				this.poem.RemoveSelf();
			}
		}

		// Token: 0x04000853 RID: 2131
		private const string ReadOnceFlag = "poem_read";

		// Token: 0x04000854 RID: 2132
		private Player player;

		// Token: 0x04000855 RID: 2133
		private CS02_Journal.PoemPage poem;

		// Token: 0x0200043A RID: 1082
		private class PoemPage : Entity
		{
			// Token: 0x0600213E RID: 8510 RVA: 0x000E38C0 File Offset: 0x000E1AC0
			public PoemPage()
			{
				base.Tag = Tags.HUD;
				this.paper = GFX.Gui["poempage"];
				this.text = FancyText.Parse(Dialog.Get("CH2_POEM", null), (int)((float)(this.paper.Width - 120) / 0.7f), -1, 1f, new Color?(Color.Black * 0.6f), null);
				base.Add(new BeforeRenderHook(new Action(this.BeforeRender)));
			}

			// Token: 0x0600213F RID: 8511 RVA: 0x000E396C File Offset: 0x000E1B6C
			public IEnumerator EaseIn()
			{
				Audio.Play("event:/game/03_resort/memo_in");
				Vector2 vector = new Vector2((float)Engine.Width, (float)Engine.Height) / 2f;
				Vector2 from = vector + new Vector2(0f, 200f);
				Vector2 to = vector;
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

			// Token: 0x06002140 RID: 8512 RVA: 0x000E397B File Offset: 0x000E1B7B
			public IEnumerator EaseOut()
			{
				Audio.Play("event:/game/03_resort/memo_out");
				this.easingOut = true;
				Vector2 from = this.Position;
				Vector2 to = new Vector2((float)Engine.Width, (float)Engine.Height) / 2f + new Vector2(0f, -200f);
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

			// Token: 0x06002141 RID: 8513 RVA: 0x000E398C File Offset: 0x000E1B8C
			public void BeforeRender()
			{
				if (this.target == null)
				{
					this.target = VirtualContent.CreateRenderTarget("journal-poem", this.paper.Width, this.paper.Height, false, true, 0);
				}
				Engine.Graphics.GraphicsDevice.SetRenderTarget(this.target);
				Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				this.paper.Draw(Vector2.Zero);
				this.text.DrawJustifyPerLine(new Vector2((float)this.paper.Width, (float)this.paper.Height) / 2f, new Vector2(0.5f, 0.5f), Vector2.One * 0.7f, 1f, 0, int.MaxValue);
				Draw.SpriteBatch.End();
			}

			// Token: 0x06002142 RID: 8514 RVA: 0x000E3A7D File Offset: 0x000E1C7D
			public override void Removed(Scene scene)
			{
				if (this.target != null)
				{
					this.target.Dispose();
				}
				this.target = null;
				base.Removed(scene);
			}

			// Token: 0x06002143 RID: 8515 RVA: 0x000E3AA0 File Offset: 0x000E1CA0
			public override void SceneEnd(Scene scene)
			{
				if (this.target != null)
				{
					this.target.Dispose();
				}
				this.target = null;
				base.SceneEnd(scene);
			}

			// Token: 0x06002144 RID: 8516 RVA: 0x000E3AC3 File Offset: 0x000E1CC3
			public override void Update()
			{
				this.timer += Engine.DeltaTime;
				base.Update();
			}

			// Token: 0x06002145 RID: 8517 RVA: 0x000E3AE0 File Offset: 0x000E1CE0
			public override void Render()
			{
				Level level = base.Scene as Level;
				if (level != null && (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene))
				{
					return;
				}
				if (this.target != null)
				{
					Draw.SpriteBatch.Draw(this.target, this.Position, new Rectangle?(this.target.Bounds), Color.White * this.alpha, this.rotation, new Vector2((float)this.target.Width, (float)this.target.Height) / 2f, this.scale, SpriteEffects.None, 0f);
					if (!this.easingOut)
					{
						GFX.Gui["textboxbutton"].DrawCentered(this.Position + new Vector2((float)(this.target.Width / 2 + 40), (float)(this.target.Height / 2 + ((this.timer % 1f < 0.25f) ? 6 : 0))));
					}
				}
			}

			// Token: 0x04002151 RID: 8529
			private const float TextScale = 0.7f;

			// Token: 0x04002152 RID: 8530
			private MTexture paper;

			// Token: 0x04002153 RID: 8531
			private VirtualRenderTarget target;

			// Token: 0x04002154 RID: 8532
			private FancyText.Text text;

			// Token: 0x04002155 RID: 8533
			private float alpha = 1f;

			// Token: 0x04002156 RID: 8534
			private float scale = 1f;

			// Token: 0x04002157 RID: 8535
			private float rotation;

			// Token: 0x04002158 RID: 8536
			private float timer;

			// Token: 0x04002159 RID: 8537
			private bool easingOut;
		}
	}
}
