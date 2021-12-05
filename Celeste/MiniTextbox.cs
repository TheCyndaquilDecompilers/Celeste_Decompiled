using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200020A RID: 522
	[Tracked(false)]
	public class MiniTextbox : Entity
	{
		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06001106 RID: 4358 RVA: 0x000517EC File Offset: 0x0004F9EC
		public static bool Displayed
		{
			get
			{
				foreach (Entity entity in Engine.Scene.Tracker.GetEntities<MiniTextbox>())
				{
					MiniTextbox miniTextbox = (MiniTextbox)entity;
					if (!miniTextbox.closing && miniTextbox.ease > 0.25f)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x00051864 File Offset: 0x0004FA64
		public MiniTextbox(string dialogId)
		{
			base.Tag = (Tags.HUD | Tags.TransitionUpdate);
			this.portraitSize = 112f;
			this.box = GFX.Portraits["textbox/default_mini"];
			this.text = FancyText.Parse(Dialog.Get(dialogId.Trim(), null), (int)(1688f - this.portraitSize - 32f), 2, 1f, null, null);
			foreach (FancyText.Node node in this.text.Nodes)
			{
				if (node is FancyText.Portrait)
				{
					FancyText.Portrait portrait = node as FancyText.Portrait;
					this.portraitData = portrait;
					this.portrait = GFX.PortraitsSpriteBank.Create("portrait_" + portrait.Sprite);
					XmlElement xml = GFX.PortraitsSpriteBank.SpriteData["portrait_" + portrait.Sprite].Sources[0].XML;
					this.portraitScale = this.portraitSize / xml.AttrFloat("size", 160f);
					string id = "textbox/" + xml.Attr("textbox", "default") + "_mini";
					if (GFX.Portraits.Has(id))
					{
						this.box = GFX.Portraits[id];
					}
					base.Add(this.portrait);
				}
			}
			base.Add(this.routine = new Coroutine(this.Routine(), true));
			this.routine.UseRawDeltaTime = true;
			base.Add(new TransitionListener
			{
				OnOutBegin = delegate()
				{
					if (!this.closing)
					{
						this.routine.Replace(this.Close());
					}
				}
			});
			if (Level.DialogSnapshot == null)
			{
				Level.DialogSnapshot = Audio.CreateSnapshot("snapshot:/dialogue_in_progress", false);
			}
			Audio.ResumeSnapshot(Level.DialogSnapshot);
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x00051A80 File Offset: 0x0004FC80
		private IEnumerator Routine()
		{
			List<Entity> entities = base.Scene.Tracker.GetEntities<MiniTextbox>();
			foreach (Entity entity in entities)
			{
				MiniTextbox miniTextbox = (MiniTextbox)entity;
				if (miniTextbox != this)
				{
					miniTextbox.Add(new Coroutine(miniTextbox.Close(), true));
				}
			}
			if (entities.Count > 0)
			{
				yield return 0.3f;
			}
			while ((this.ease += Engine.DeltaTime * 4f) < 1f)
			{
				yield return null;
			}
			this.ease = 1f;
			if (this.portrait != null)
			{
				string beginAnim = "begin_" + this.portraitData.Animation;
				if (this.portrait.Has(beginAnim))
				{
					this.portrait.Play(beginAnim, false, false);
					while (this.portrait.CurrentAnimationID == beginAnim && this.portrait.Animating)
					{
						yield return null;
					}
				}
				this.portrait.Play("talk_" + this.portraitData.Animation, false, false);
				this.talkerSfx = new SoundSource().Play(this.portraitData.SfxEvent, null, 0f);
				this.talkerSfx.Param("dialogue_portrait", (float)this.portraitData.SfxExpression);
				this.talkerSfx.Param("dialogue_end", 0f);
				base.Add(this.talkerSfx);
				beginAnim = null;
			}
			float num = 0f;
			while (this.index < this.text.Nodes.Count)
			{
				if (this.text.Nodes[this.index] is FancyText.Char)
				{
					num += (this.text.Nodes[this.index] as FancyText.Char).Delay;
				}
				this.index++;
				if (num > 0.016f)
				{
					yield return num;
					num = 0f;
				}
			}
			if (this.portrait != null)
			{
				this.portrait.Play("idle_" + this.portraitData.Animation, false, false);
			}
			if (this.talkerSfx != null)
			{
				this.talkerSfx.Param("dialogue_portrait", 0f);
				this.talkerSfx.Param("dialogue_end", 1f);
			}
			Audio.EndSnapshot(Level.DialogSnapshot);
			yield return 3f;
			yield return this.Close();
			yield break;
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x00051A8F File Offset: 0x0004FC8F
		private IEnumerator Close()
		{
			if (!this.closing)
			{
				this.closing = true;
				while ((this.ease -= Engine.DeltaTime * 4f) > 0f)
				{
					yield return null;
				}
				this.ease = 0f;
				base.RemoveSelf();
			}
			yield break;
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x00051A9E File Offset: 0x0004FC9E
		public override void Update()
		{
			if ((base.Scene as Level).RetryPlayerCorpse != null && !this.closing)
			{
				this.routine.Replace(this.Close());
			}
			base.Update();
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x00051AD4 File Offset: 0x0004FCD4
		public override void Render()
		{
			if (this.ease <= 0f)
			{
				return;
			}
			Level level = base.Scene as Level;
			if (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene)
			{
				return;
			}
			Vector2 vector = new Vector2((float)(Engine.Width / 2), 72f + ((float)Engine.Width - 1688f) / 4f);
			Vector2 vector2 = vector + new Vector2(-828f, -56f);
			this.box.DrawCentered(vector, Color.White, new Vector2(1f, this.ease));
			if (this.portrait != null)
			{
				this.portrait.Scale = new Vector2(1f, this.ease) * this.portraitScale;
				this.portrait.RenderPosition = vector2 + new Vector2(this.portraitSize / 2f, this.portraitSize / 2f);
				this.portrait.Render();
			}
			this.text.Draw(new Vector2(vector2.X + this.portraitSize + 32f, vector.Y), new Vector2(0f, 0.5f), new Vector2(1f, this.ease) * 0.75f, 1f, 0, this.index);
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x00051C37 File Offset: 0x0004FE37
		public override void Removed(Scene scene)
		{
			Audio.EndSnapshot(Level.DialogSnapshot);
			base.Removed(scene);
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x00051C4A File Offset: 0x0004FE4A
		public override void SceneEnd(Scene scene)
		{
			Audio.EndSnapshot(Level.DialogSnapshot);
			base.SceneEnd(scene);
		}

		// Token: 0x04000CA6 RID: 3238
		public const float TextScale = 0.75f;

		// Token: 0x04000CA7 RID: 3239
		public const float BoxWidth = 1688f;

		// Token: 0x04000CA8 RID: 3240
		public const float BoxHeight = 144f;

		// Token: 0x04000CA9 RID: 3241
		public const float HudElementHeight = 180f;

		// Token: 0x04000CAA RID: 3242
		private int index;

		// Token: 0x04000CAB RID: 3243
		private FancyText.Text text;

		// Token: 0x04000CAC RID: 3244
		private MTexture box;

		// Token: 0x04000CAD RID: 3245
		private float ease;

		// Token: 0x04000CAE RID: 3246
		private bool closing;

		// Token: 0x04000CAF RID: 3247
		private Coroutine routine;

		// Token: 0x04000CB0 RID: 3248
		private Sprite portrait;

		// Token: 0x04000CB1 RID: 3249
		private FancyText.Portrait portraitData;

		// Token: 0x04000CB2 RID: 3250
		private float portraitSize;

		// Token: 0x04000CB3 RID: 3251
		private float portraitScale;

		// Token: 0x04000CB4 RID: 3252
		private SoundSource talkerSfx;
	}
}
