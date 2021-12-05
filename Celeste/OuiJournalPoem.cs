using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200029C RID: 668
	public class OuiJournalPoem : OuiJournalPage
	{
		// Token: 0x060014BD RID: 5309 RVA: 0x00073010 File Offset: 0x00071210
		public OuiJournalPoem(OuiJournal journal) : base(journal)
		{
			this.PageTexture = "page";
			this.swapRoutine.RemoveOnComplete = false;
			float num = 0f;
			foreach (string text in SaveData.Instance.Poem)
			{
				string text2 = Dialog.Clean("poem_" + text, null);
				text2 = text2.Replace("\n", " - ");
				this.lines.Add(new OuiJournalPoem.PoemLine
				{
					Text = text2,
					Index = num,
					Remix = AreaData.IsPoemRemix(text)
				});
				num += 1f;
			}
		}

		// Token: 0x060014BE RID: 5310 RVA: 0x00073108 File Offset: 0x00071308
		public static float GetY(float index)
		{
			return 120f + 44f * (index + 0.5f) + 4f * index + (float)((int)index / 4) * 16f;
		}

		// Token: 0x060014BF RID: 5311 RVA: 0x00073134 File Offset: 0x00071334
		public override void Redraw(VirtualRenderTarget buffer)
		{
			base.Redraw(buffer);
			Draw.SpriteBatch.Begin();
			ActiveFont.Draw(Dialog.Clean("journal_poem", null), new Vector2(60f, 60f), new Vector2(0f, 0.5f), Vector2.One, Color.Black * 0.6f);
			foreach (OuiJournalPoem.PoemLine poemLine in this.lines)
			{
				poemLine.Render();
			}
			if (this.lines.Count > 0)
			{
				MTexture mtexture = MTN.Journal[this.dragging ? "poemSlider" : "poemArrow"];
				Vector2 position = new Vector2(50f, OuiJournalPoem.GetY(this.slider));
				mtexture.DrawCentered(position, Color.White, 1f + 0.25f * this.wiggler.Value);
			}
			Draw.SpriteBatch.End();
		}

		// Token: 0x060014C0 RID: 5312 RVA: 0x00073248 File Offset: 0x00071448
		private IEnumerator Swap(int a, int b)
		{
			string value = SaveData.Instance.Poem[a];
			SaveData.Instance.Poem[a] = SaveData.Instance.Poem[b];
			SaveData.Instance.Poem[b] = value;
			OuiJournalPoem.PoemLine poemA = this.lines[a];
			OuiJournalPoem.PoemLine poemB = this.lines[b];
			OuiJournalPoem.PoemLine value2 = this.lines[a];
			this.lines[a] = this.lines[b];
			this.lines[b] = value2;
			this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.125f, true);
			this.tween.OnUpdate = delegate(Tween t)
			{
				poemA.Index = MathHelper.Lerp((float)a, (float)b, t.Eased);
				poemB.Index = MathHelper.Lerp((float)b, (float)a, t.Eased);
			};
			this.tween.OnComplete = delegate(Tween t)
			{
				this.tween = null;
			};
			yield return this.tween.Wait();
			this.swapping = false;
			yield break;
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x00073268 File Offset: 0x00071468
		public override void Update()
		{
			if (this.lines.Count <= 0)
			{
				return;
			}
			if (this.tween != null && this.tween.Active)
			{
				this.tween.Update();
			}
			if (Input.MenuDown.Pressed && this.index + 1 < this.lines.Count && !this.swapping)
			{
				if (this.dragging)
				{
					Audio.Play("event:/ui/world_map/journal/heart_shift_down");
					this.swapRoutine.Replace(this.Swap(this.index, this.index + 1));
					this.swapping = true;
				}
				else
				{
					Audio.Play("event:/ui/world_map/journal/heart_roll");
				}
				this.index++;
			}
			else if (Input.MenuUp.Pressed && this.index > 0 && !this.swapping)
			{
				if (this.dragging)
				{
					Audio.Play("event:/ui/world_map/journal/heart_shift_up");
					this.swapRoutine.Replace(this.Swap(this.index, this.index - 1));
					this.swapping = true;
				}
				else
				{
					Audio.Play("event:/ui/world_map/journal/heart_roll");
				}
				this.index--;
			}
			else if (Input.MenuConfirm.Pressed)
			{
				this.Journal.PageTurningLocked = true;
				Audio.Play("event:/ui/world_map/journal/heart_grab");
				this.dragging = true;
				this.wiggler.Start();
			}
			else if (!Input.MenuConfirm.Check && this.dragging)
			{
				this.Journal.PageTurningLocked = false;
				Audio.Play("event:/ui/world_map/journal/heart_release");
				this.dragging = false;
				this.wiggler.Start();
			}
			for (int i = 0; i < this.lines.Count; i++)
			{
				OuiJournalPoem.PoemLine poemLine = this.lines[i];
				poemLine.HoveringEase = Calc.Approach(poemLine.HoveringEase, (this.index == i) ? 1f : 0f, 8f * Engine.DeltaTime);
				poemLine.HoldingEase = Calc.Approach(poemLine.HoldingEase, (this.index == i && this.dragging) ? 1f : 0f, 8f * Engine.DeltaTime);
			}
			this.slider = Calc.Approach(this.slider, (float)this.index, 16f * Engine.DeltaTime);
			if (this.swapRoutine != null && this.swapRoutine.Active)
			{
				this.swapRoutine.Update();
			}
			this.wiggler.Update();
			this.Redraw(this.Journal.CurrentPageBuffer);
		}

		// Token: 0x0400108D RID: 4237
		private const float textScale = 0.5f;

		// Token: 0x0400108E RID: 4238
		private const float holdingScaleAdd = 0.1f;

		// Token: 0x0400108F RID: 4239
		private const float poemHeight = 44f;

		// Token: 0x04001090 RID: 4240
		private const float poemSpacing = 4f;

		// Token: 0x04001091 RID: 4241
		private const float poemStanzaSpacing = 16f;

		// Token: 0x04001092 RID: 4242
		private List<OuiJournalPoem.PoemLine> lines = new List<OuiJournalPoem.PoemLine>();

		// Token: 0x04001093 RID: 4243
		private int index;

		// Token: 0x04001094 RID: 4244
		private float slider;

		// Token: 0x04001095 RID: 4245
		private bool dragging;

		// Token: 0x04001096 RID: 4246
		private bool swapping;

		// Token: 0x04001097 RID: 4247
		private Coroutine swapRoutine = new Coroutine(true);

		// Token: 0x04001098 RID: 4248
		private Wiggler wiggler = Wiggler.Create(0.4f, 4f, null, false, false);

		// Token: 0x04001099 RID: 4249
		private Tween tween;

		// Token: 0x0200062D RID: 1581
		private class PoemLine
		{
			// Token: 0x06002A6E RID: 10862 RVA: 0x001104F8 File Offset: 0x0010E6F8
			public void Render()
			{
				float num = 100f + Ease.CubeInOut(this.HoveringEase) * 20f;
				float y = OuiJournalPoem.GetY(this.Index);
				Draw.Rect(num, y - 22f, 810f, 44f, Color.White * 0.25f);
				Vector2 scale = Vector2.One * (0.6f + this.HoldingEase * 0.4f);
				MTN.Journal[this.Remix ? "heartgem1" : "heartgem0"].DrawCentered(new Vector2(num + 20f, y), Color.White, scale);
				Color color = Color.Black * (0.7f + this.HoveringEase * 0.3f);
				Vector2 scale2 = Vector2.One * (0.5f + this.HoldingEase * 0.1f);
				ActiveFont.Draw(this.Text, new Vector2(num + 60f, y), new Vector2(0f, 0.5f), scale2, color);
			}

			// Token: 0x04002976 RID: 10614
			public float Index;

			// Token: 0x04002977 RID: 10615
			public string Text;

			// Token: 0x04002978 RID: 10616
			public float HoveringEase;

			// Token: 0x04002979 RID: 10617
			public float HoldingEase;

			// Token: 0x0400297A RID: 10618
			public bool Remix;
		}
	}
}
