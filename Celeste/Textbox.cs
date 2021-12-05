using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002EB RID: 747
	[Tracked(false)]
	public class Textbox : Entity
	{
		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06001704 RID: 5892 RVA: 0x0008B956 File Offset: 0x00089B56
		// (set) Token: 0x06001705 RID: 5893 RVA: 0x0008B95E File Offset: 0x00089B5E
		public bool Opened { get; private set; }

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06001706 RID: 5894 RVA: 0x0008B967 File Offset: 0x00089B67
		// (set) Token: 0x06001707 RID: 5895 RVA: 0x0008B96F File Offset: 0x00089B6F
		public int Page { get; private set; }

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06001708 RID: 5896 RVA: 0x0008B978 File Offset: 0x00089B78
		public List<FancyText.Node> Nodes
		{
			get
			{
				return this.text.Nodes;
			}
		}

		// Token: 0x1700018D RID: 397
		// (set) Token: 0x06001709 RID: 5897 RVA: 0x0008B985 File Offset: 0x00089B85
		public bool UseRawDeltaTime
		{
			set
			{
				this.runRoutine.UseRawDeltaTime = value;
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x0600170A RID: 5898 RVA: 0x0008B993 File Offset: 0x00089B93
		// (set) Token: 0x0600170B RID: 5899 RVA: 0x0008B99B File Offset: 0x00089B9B
		public int Start { get; private set; }

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x0600170C RID: 5900 RVA: 0x0008B9A4 File Offset: 0x00089BA4
		public string PortraitName
		{
			get
			{
				if (this.portrait == null || this.portrait.Sprite == null)
				{
					return "";
				}
				return this.portrait.Sprite;
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x0600170D RID: 5901 RVA: 0x0008B9CC File Offset: 0x00089BCC
		public string PortraitAnimation
		{
			get
			{
				if (this.portrait == null || this.portrait.Sprite == null)
				{
					return "";
				}
				return this.portrait.Animation;
			}
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x0008B9F4 File Offset: 0x00089BF4
		public Textbox(string dialog, params Func<IEnumerator>[] events) : this(dialog, null, events)
		{
		}

		// Token: 0x0600170F RID: 5903 RVA: 0x0008BA00 File Offset: 0x00089C00
		public Textbox(string dialog, Language language, params Func<IEnumerator>[] events)
		{
			base.Tag = (Tags.PauseUpdate | Tags.HUD);
			this.Opened = true;
			this.font = Dialog.Language.Font;
			this.lineHeight = (float)(Dialog.Language.FontSize.LineHeight - 1);
			this.portraitSprite.UseRawDeltaTime = true;
			base.Add(this.portraitWiggle = Wiggler.Create(0.4f, 4f, null, false, false));
			this.events = events;
			this.linesPerPage = (int)(240f / this.lineHeight);
			this.innerTextPadding = (272f - this.lineHeight * (float)this.linesPerPage) / 2f;
			this.maxLineWidthNoPortrait = 1688f - this.innerTextPadding * 2f;
			this.maxLineWidth = this.maxLineWidthNoPortrait - 240f - 32f;
			this.text = FancyText.Parse(Dialog.Get(dialog, language), (int)this.maxLineWidth, this.linesPerPage, 0f, null, language);
			this.index = 0;
			this.Start = 0;
			this.skipRoutine = new Coroutine(this.SkipDialog(), true);
			this.runRoutine = new Coroutine(this.RunRoutine(), true);
			this.runRoutine.UseRawDeltaTime = true;
			if (Level.DialogSnapshot == null)
			{
				Level.DialogSnapshot = Audio.CreateSnapshot("snapshot:/dialogue_in_progress", false);
			}
			Audio.ResumeSnapshot(Level.DialogSnapshot);
			base.Add(this.phonestatic = new SoundSource());
		}

		// Token: 0x06001710 RID: 5904 RVA: 0x0008BBDC File Offset: 0x00089DDC
		public void SetStart(int value)
		{
			this.Start = value;
			this.index = value;
		}

		// Token: 0x06001711 RID: 5905 RVA: 0x0008BBF9 File Offset: 0x00089DF9
		private IEnumerator RunRoutine()
		{
			FancyText.Node last = null;
			float delayBuildup = 0f;
			while (this.index < this.Nodes.Count)
			{
				FancyText.Node current = this.Nodes[this.index];
				float delay = 0f;
				if (current is FancyText.Anchor)
				{
					if (this.RenderOffset == Vector2.Zero)
					{
						FancyText.Anchors next = (current as FancyText.Anchor).Position;
						if (this.ease >= 1f && next != this.anchor)
						{
							yield return this.EaseClose(false);
						}
						this.anchor = next;
					}
				}
				else if (current is FancyText.Portrait)
				{
					FancyText.Portrait next2 = current as FancyText.Portrait;
					this.phonestatic.Stop(true);
					if (this.ease >= 1f && (this.portrait == null || next2.Sprite != this.portrait.Sprite || next2.Side != this.portrait.Side))
					{
						yield return this.EaseClose(false);
					}
					this.textbox = GFX.Portraits["textbox/default"];
					this.textboxOverlay = null;
					this.portraitExists = false;
					this.activeTalker = null;
					this.isPortraitGlitchy = false;
					XmlElement xmlElement = null;
					if (!string.IsNullOrEmpty(next2.Sprite))
					{
						if (GFX.PortraitsSpriteBank.Has(next2.SpriteId))
						{
							xmlElement = GFX.PortraitsSpriteBank.SpriteData[next2.SpriteId].Sources[0].XML;
						}
						this.portraitExists = (xmlElement != null);
						this.isPortraitGlitchy = next2.Glitchy;
						if (this.isPortraitGlitchy && this.portraitGlitchy == null)
						{
							this.portraitGlitchy = new Sprite(GFX.Portraits, "noise/");
							this.portraitGlitchy.AddLoop("noise", "", 0.1f);
							this.portraitGlitchy.Play("noise", false, false);
						}
					}
					if (this.portraitExists)
					{
						if (this.portrait == null || next2.Sprite != this.portrait.Sprite)
						{
							GFX.PortraitsSpriteBank.CreateOn(this.portraitSprite, next2.SpriteId);
							this.portraitScale = 240f / (float)xmlElement.AttrInt("size", 160);
							if (!this.talkers.ContainsKey(next2.SfxEvent))
							{
								SoundSource soundSource = new SoundSource().Play(next2.SfxEvent, null, 0f);
								this.talkers.Add(next2.SfxEvent, soundSource);
								base.Add(soundSource);
							}
						}
						if (this.talkers.ContainsKey(next2.SfxEvent))
						{
							this.activeTalker = this.talkers[next2.SfxEvent];
						}
						string text = "textbox/" + xmlElement.Attr("textbox", "default");
						this.textbox = GFX.Portraits[text];
						if (GFX.Portraits.Has(text + "_overlay"))
						{
							this.textboxOverlay = GFX.Portraits[text + "_overlay"];
						}
						string text2 = xmlElement.Attr("phonestatic", "");
						if (!string.IsNullOrEmpty(text2))
						{
							if (text2 == "ex")
							{
								this.phonestatic.Play("event:/char/dialogue/sfx_support/phone_static_ex", null, 0f);
							}
							else if (text2 == "mom")
							{
								this.phonestatic.Play("event:/char/dialogue/sfx_support/phone_static_mom", null, 0f);
							}
						}
						this.canSkip = false;
						FancyText.Portrait portrait = this.portrait;
						this.portrait = next2;
						if (next2.Pop)
						{
							this.portraitWiggle.Start();
						}
						if (portrait == null || portrait.Sprite != next2.Sprite || portrait.Animation != next2.Animation)
						{
							if (this.portraitSprite.Has(next2.BeginAnimation))
							{
								this.portraitSprite.Play(next2.BeginAnimation, true, false);
								yield return this.EaseOpen();
								while (this.portraitSprite.CurrentAnimationID == next2.BeginAnimation && this.portraitSprite.Animating)
								{
									yield return null;
								}
							}
							if (this.portraitSprite.Has(next2.IdleAnimation))
							{
								this.portraitIdling = true;
								this.portraitSprite.Play(next2.IdleAnimation, true, false);
							}
						}
						yield return this.EaseOpen();
						this.canSkip = true;
					}
					else
					{
						this.portrait = null;
						yield return this.EaseOpen();
					}
					next2 = null;
				}
				else if (current is FancyText.NewPage)
				{
					this.PlayIdleAnimation();
					if (this.ease >= 1f)
					{
						this.waitingForInput = true;
						yield return 0.1f;
						while (!this.ContinuePressed())
						{
							yield return null;
						}
						this.waitingForInput = false;
					}
					this.Start = this.index + 1;
					int page = this.Page;
					this.Page = page + 1;
				}
				else if (current is FancyText.Wait)
				{
					this.PlayIdleAnimation();
					delay = (current as FancyText.Wait).Duration;
				}
				else if (current is FancyText.Trigger)
				{
					this.isInTrigger = true;
					this.PlayIdleAnimation();
					FancyText.Trigger trigger = current as FancyText.Trigger;
					if (!trigger.Silent)
					{
						yield return this.EaseClose(false);
					}
					int num = trigger.Index;
					if (this.events != null && num >= 0 && num < this.events.Length)
					{
						yield return this.events[num]();
					}
					this.isInTrigger = false;
					trigger = null;
				}
				else if (current is FancyText.Char)
				{
					FancyText.Char ch = current as FancyText.Char;
					this.lastChar = (char)ch.Character;
					if (this.ease < 1f)
					{
						yield return this.EaseOpen();
					}
					bool flag = false;
					if (this.index - 5 > this.Start)
					{
						for (int i = this.index; i < Math.Min(this.index + 4, this.Nodes.Count); i++)
						{
							if (this.Nodes[i] is FancyText.NewPage)
							{
								flag = true;
								this.PlayIdleAnimation();
							}
						}
					}
					if (!flag && !ch.IsPunctuation)
					{
						this.PlayTalkAnimation();
					}
					if (last != null && last is FancyText.NewPage)
					{
						this.index--;
						yield return 0.2f;
						this.index++;
					}
					delay = ch.Delay + delayBuildup;
					ch = null;
				}
				last = current;
				this.index++;
				if (delay < 0.016f)
				{
					delayBuildup += delay;
				}
				else
				{
					delayBuildup = 0f;
					if (delay > 0.5f)
					{
						this.PlayIdleAnimation();
					}
					yield return delay;
				}
				current = null;
			}
			this.PlayIdleAnimation();
			if (this.ease > 0f)
			{
				this.waitingForInput = true;
				while (!this.ContinuePressed())
				{
					yield return null;
				}
				this.waitingForInput = false;
				this.Start = this.Nodes.Count;
				yield return this.EaseClose(true);
			}
			this.Close();
			yield break;
		}

		// Token: 0x06001712 RID: 5906 RVA: 0x0008BC08 File Offset: 0x00089E08
		private void PlayIdleAnimation()
		{
			this.StopTalker();
			if (!this.portraitIdling && this.portraitSprite != null && this.portrait != null && this.portraitSprite.Has(this.portrait.IdleAnimation))
			{
				this.portraitSprite.Play(this.portrait.IdleAnimation, false, false);
				this.portraitIdling = true;
			}
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x0008BC6A File Offset: 0x00089E6A
		private void StopTalker()
		{
			if (this.activeTalker != null)
			{
				this.activeTalker.Param("dialogue_portrait", 0f);
				this.activeTalker.Param("dialogue_end", 1f);
			}
		}

		// Token: 0x06001714 RID: 5908 RVA: 0x0008BCA0 File Offset: 0x00089EA0
		private void PlayTalkAnimation()
		{
			this.StartTalker();
			if (this.portraitIdling && this.portraitSprite != null && this.portrait != null && this.portraitSprite.Has(this.portrait.TalkAnimation))
			{
				this.portraitSprite.Play(this.portrait.TalkAnimation, false, false);
				this.portraitIdling = false;
			}
		}

		// Token: 0x06001715 RID: 5909 RVA: 0x0008BD04 File Offset: 0x00089F04
		private void StartTalker()
		{
			if (this.activeTalker != null)
			{
				this.activeTalker.Param("dialogue_portrait", (float)((this.portrait != null) ? this.portrait.SfxExpression : 1));
				this.activeTalker.Param("dialogue_end", 0f);
			}
		}

		// Token: 0x06001716 RID: 5910 RVA: 0x0008BD57 File Offset: 0x00089F57
		private IEnumerator EaseOpen()
		{
			if (this.ease < 1f)
			{
				this.easingOpen = true;
				if (this.portrait != null && this.portrait.Sprite.IndexOf("madeline", StringComparison.InvariantCultureIgnoreCase) >= 0)
				{
					Audio.Play("event:/ui/game/textbox_madeline_in");
				}
				else
				{
					Audio.Play("event:/ui/game/textbox_other_in");
				}
				while ((this.ease += (this.runRoutine.UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime) / 0.4f) < 1f)
				{
					this.gradientFade = Math.Max(this.gradientFade, this.ease);
					yield return null;
				}
				this.ease = (this.gradientFade = 1f);
				this.easingOpen = false;
			}
			yield break;
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x0008BD66 File Offset: 0x00089F66
		private IEnumerator EaseClose(bool final)
		{
			this.easingClose = true;
			if (this.portrait != null && this.portrait.Sprite.IndexOf("madeline", StringComparison.InvariantCultureIgnoreCase) >= 0)
			{
				Audio.Play("event:/ui/game/textbox_madeline_out");
			}
			else
			{
				Audio.Play("event:/ui/game/textbox_other_out");
			}
			while ((this.ease -= (this.runRoutine.UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime) / 0.4f) > 0f)
			{
				if (final)
				{
					this.gradientFade = this.ease;
				}
				yield return null;
			}
			this.ease = 0f;
			this.easingClose = false;
			yield break;
		}

		// Token: 0x06001718 RID: 5912 RVA: 0x0008BD7C File Offset: 0x00089F7C
		private IEnumerator SkipDialog()
		{
			for (;;)
			{
				if (!this.waitingForInput && this.canSkip && !this.easingOpen && !this.easingClose && this.ContinuePressed())
				{
					this.StopTalker();
					this.disableInput = true;
					while (!this.waitingForInput && this.canSkip && !this.easingOpen && !this.easingClose && !this.isInTrigger && !this.runRoutine.Finished)
					{
						this.runRoutine.Update();
					}
				}
				yield return null;
				this.disableInput = false;
			}
			yield break;
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x0008BD8C File Offset: 0x00089F8C
		public bool SkipToPage(int page)
		{
			this.autoPressContinue = true;
			while (this.Page != page && !this.runRoutine.Finished)
			{
				this.Update();
			}
			this.autoPressContinue = false;
			this.Update();
			while (this.Opened && this.ease < 1f)
			{
				this.Update();
			}
			return this.Page == page && this.Opened;
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x0008BDF9 File Offset: 0x00089FF9
		public void Close()
		{
			this.Opened = false;
			if (base.Scene != null)
			{
				base.Scene.Remove(this);
			}
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x0008BE16 File Offset: 0x0008A016
		private bool ContinuePressed()
		{
			return this.autoPressContinue || ((Input.MenuConfirm.Pressed || Input.MenuCancel.Pressed) && !this.disableInput);
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x0008BE48 File Offset: 0x0008A048
		public override void Update()
		{
			Level level = base.Scene as Level;
			if (level != null && (level.FrozenOrPaused || level.RetryPlayerCorpse != null))
			{
				return;
			}
			if (!this.autoPressContinue)
			{
				this.skipRoutine.Update();
			}
			this.runRoutine.Update();
			if (base.Scene != null && base.Scene.OnInterval(0.05f))
			{
				this.shakeSeed = Calc.Random.Next();
			}
			if (this.portraitSprite != null && this.ease >= 1f)
			{
				this.portraitSprite.Update();
			}
			if (this.portraitGlitchy != null && this.ease >= 1f)
			{
				this.portraitGlitchy.Update();
			}
			this.timer += Engine.DeltaTime;
			this.portraitWiggle.Update();
			int num = Math.Min(this.index, this.Nodes.Count);
			for (int i = this.Start; i < num; i++)
			{
				if (this.Nodes[i] is FancyText.Char)
				{
					FancyText.Char @char = this.Nodes[i] as FancyText.Char;
					if (@char.Fade < 1f)
					{
						@char.Fade = Calc.Clamp(@char.Fade + 8f * Engine.DeltaTime, 0f, 1f);
					}
				}
			}
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x0008BF9C File Offset: 0x0008A19C
		public override void Render()
		{
			Level level = base.Scene as Level;
			if (level != null && (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene))
			{
				return;
			}
			float num = Ease.CubeInOut(this.ease);
			if (num < 0.05f)
			{
				return;
			}
			float num2 = 116f;
			Vector2 vector = new Vector2(num2, num2 / 2f) + this.RenderOffset;
			if (this.RenderOffset == Vector2.Zero)
			{
				if (this.anchor == FancyText.Anchors.Bottom)
				{
					vector = new Vector2(num2, 1080f - num2 / 2f - 272f);
				}
				else if (this.anchor == FancyText.Anchors.Middle)
				{
					vector = new Vector2(num2, 404f);
				}
				vector.Y += (float)((int)(136f * (1f - num)));
			}
			this.textbox.DrawCentered(vector + new Vector2(1688f, 272f * num) / 2f, Color.White, new Vector2(1f, num));
			if (this.waitingForInput)
			{
				float num3 = (this.portrait == null || this.PortraitSide(this.portrait) < 0) ? 1688f : 1432f;
				Vector2 position = new Vector2(vector.X + num3, vector.Y + 272f) + new Vector2(-48f, (float)(-40 + ((this.timer % 1f < 0.25f) ? 6 : 0)));
				GFX.Gui["textboxbutton"].DrawCentered(position);
			}
			if (this.portraitExists)
			{
				if (this.PortraitSide(this.portrait) > 0)
				{
					this.portraitSprite.Position = new Vector2(vector.X + 1688f - 240f - 16f, vector.Y);
					this.portraitSprite.Scale.X = -this.portraitScale;
				}
				else
				{
					this.portraitSprite.Position = new Vector2(vector.X + 16f, vector.Y);
					this.portraitSprite.Scale.X = this.portraitScale;
				}
				Sprite sprite = this.portraitSprite;
				sprite.Scale.X = sprite.Scale.X * (float)(this.portrait.Flipped ? -1 : 1);
				this.portraitSprite.Scale.Y = this.portraitScale * ((272f * num - 32f) / 240f) * (float)(this.portrait.UpsideDown ? -1 : 1);
				this.portraitSprite.Scale *= 0.9f + this.portraitWiggle.Value * 0.1f;
				this.portraitSprite.Position += new Vector2(120f, 272f * num * 0.5f);
				this.portraitSprite.Color = Color.White * num;
				if (Math.Abs(this.portraitSprite.Scale.Y) > 0.05f)
				{
					this.portraitSprite.Render();
					if (this.isPortraitGlitchy && this.portraitGlitchy != null)
					{
						this.portraitGlitchy.Position = this.portraitSprite.Position;
						this.portraitGlitchy.Origin = this.portraitSprite.Origin;
						this.portraitGlitchy.Scale = this.portraitSprite.Scale;
						this.portraitGlitchy.Color = Color.White * 0.2f * num;
						this.portraitGlitchy.Render();
					}
				}
			}
			if (this.textboxOverlay != null)
			{
				int num4 = 1;
				if (this.portrait != null && this.PortraitSide(this.portrait) > 0)
				{
					num4 = -1;
				}
				this.textboxOverlay.DrawCentered(vector + new Vector2(1688f, 272f * num) / 2f, Color.White, new Vector2((float)num4, num));
			}
			Calc.PushRandom(this.shakeSeed);
			int num5 = 1;
			for (int i = this.Start; i < this.text.Nodes.Count; i++)
			{
				if (this.text.Nodes[i] is FancyText.NewLine)
				{
					num5++;
				}
				else if (this.text.Nodes[i] is FancyText.NewPage)
				{
					break;
				}
			}
			Vector2 value = new Vector2(this.innerTextPadding + ((this.portrait != null && this.PortraitSide(this.portrait) < 0) ? 256f : 0f), this.innerTextPadding);
			Vector2 value2 = new Vector2((this.portrait == null) ? this.maxLineWidthNoPortrait : this.maxLineWidth, (float)this.linesPerPage * this.lineHeight * num) / 2f;
			float scaleFactor = (num5 >= 4) ? 0.75f : 1f;
			this.text.Draw(vector + value + value2, new Vector2(0.5f, 0.5f), new Vector2(1f, num) * scaleFactor, num, this.Start, int.MaxValue);
			Calc.PopRandom();
		}

		// Token: 0x0600171E RID: 5918 RVA: 0x0008C4E5 File Offset: 0x0008A6E5
		public int PortraitSide(FancyText.Portrait portrait)
		{
			if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
			{
				return -portrait.Side;
			}
			return portrait.Side;
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x00051C37 File Offset: 0x0004FE37
		public override void Removed(Scene scene)
		{
			Audio.EndSnapshot(Level.DialogSnapshot);
			base.Removed(scene);
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x00051C4A File Offset: 0x0004FE4A
		public override void SceneEnd(Scene scene)
		{
			Audio.EndSnapshot(Level.DialogSnapshot);
			base.SceneEnd(scene);
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x0008C50D File Offset: 0x0008A70D
		public static IEnumerator Say(string dialog, params Func<IEnumerator>[] events)
		{
			Textbox textbox = new Textbox(dialog, events);
			Engine.Scene.Add(textbox);
			while (textbox.Opened)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x0008C523 File Offset: 0x0008A723
		public static IEnumerator SayWhileFrozen(string dialog, params Func<IEnumerator>[] events)
		{
			Textbox textbox = new Textbox(dialog, events);
			textbox.Tag |= Tags.FrozenUpdate;
			Engine.Scene.Add(textbox);
			while (textbox.Opened)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x04001392 RID: 5010
		private MTexture textbox = GFX.Portraits["textbox/default"];

		// Token: 0x04001393 RID: 5011
		private MTexture textboxOverlay;

		// Token: 0x04001394 RID: 5012
		private const int textboxInnerWidth = 1688;

		// Token: 0x04001395 RID: 5013
		private const int textboxInnerHeight = 272;

		// Token: 0x04001396 RID: 5014
		private const float portraitPadding = 16f;

		// Token: 0x04001397 RID: 5015
		private const float tweenDuration = 0.4f;

		// Token: 0x04001398 RID: 5016
		private const float switchToIdleAnimationDelay = 0.5f;

		// Token: 0x04001399 RID: 5017
		private readonly float innerTextPadding;

		// Token: 0x0400139A RID: 5018
		private readonly float maxLineWidthNoPortrait;

		// Token: 0x0400139B RID: 5019
		private readonly float maxLineWidth;

		// Token: 0x0400139C RID: 5020
		private readonly int linesPerPage;

		// Token: 0x0400139D RID: 5021
		private const int stopVoiceCharactersEarly = 4;

		// Token: 0x040013A0 RID: 5024
		private float ease;

		// Token: 0x040013A1 RID: 5025
		private FancyText.Text text;

		// Token: 0x040013A2 RID: 5026
		private Func<IEnumerator>[] events;

		// Token: 0x040013A3 RID: 5027
		private Coroutine runRoutine;

		// Token: 0x040013A4 RID: 5028
		private Coroutine skipRoutine;

		// Token: 0x040013A5 RID: 5029
		private PixelFont font;

		// Token: 0x040013A6 RID: 5030
		private float lineHeight;

		// Token: 0x040013A7 RID: 5031
		private FancyText.Anchors anchor;

		// Token: 0x040013A8 RID: 5032
		private FancyText.Portrait portrait;

		// Token: 0x040013A9 RID: 5033
		private int index;

		// Token: 0x040013AB RID: 5035
		private bool waitingForInput;

		// Token: 0x040013AC RID: 5036
		private bool disableInput;

		// Token: 0x040013AD RID: 5037
		private int shakeSeed;

		// Token: 0x040013AE RID: 5038
		private float timer;

		// Token: 0x040013AF RID: 5039
		private float gradientFade;

		// Token: 0x040013B0 RID: 5040
		private bool isInTrigger;

		// Token: 0x040013B1 RID: 5041
		private bool canSkip = true;

		// Token: 0x040013B2 RID: 5042
		private bool easingClose;

		// Token: 0x040013B3 RID: 5043
		private bool easingOpen;

		// Token: 0x040013B4 RID: 5044
		public Vector2 RenderOffset;

		// Token: 0x040013B5 RID: 5045
		private bool autoPressContinue;

		// Token: 0x040013B6 RID: 5046
		private char lastChar;

		// Token: 0x040013B7 RID: 5047
		private Sprite portraitSprite = new Sprite(null, null);

		// Token: 0x040013B8 RID: 5048
		private bool portraitExists;

		// Token: 0x040013B9 RID: 5049
		private bool portraitIdling;

		// Token: 0x040013BA RID: 5050
		private float portraitScale = 1.5f;

		// Token: 0x040013BB RID: 5051
		private Wiggler portraitWiggle;

		// Token: 0x040013BC RID: 5052
		private Sprite portraitGlitchy;

		// Token: 0x040013BD RID: 5053
		private bool isPortraitGlitchy;

		// Token: 0x040013BE RID: 5054
		private Dictionary<string, SoundSource> talkers = new Dictionary<string, SoundSource>();

		// Token: 0x040013BF RID: 5055
		private SoundSource activeTalker;

		// Token: 0x040013C0 RID: 5056
		private SoundSource phonestatic;
	}
}
