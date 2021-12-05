using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000193 RID: 403
	public class CS06_Campfire : CutsceneEntity
	{
		// Token: 0x06000DFE RID: 3582 RVA: 0x00031D08 File Offset: 0x0002FF08
		public CS06_Campfire(NPC theo, Player player) : base(true, false)
		{
			base.Tag = Tags.HUD;
			this.theo = theo;
			this.player = player;
			CS06_Campfire.Question question = new CS06_Campfire.Question("outfor");
			CS06_Campfire.Question question2 = new CS06_Campfire.Question("temple");
			CS06_Campfire.Question question3 = new CS06_Campfire.Question("explain");
			CS06_Campfire.Question question4 = new CS06_Campfire.Question("thankyou");
			CS06_Campfire.Question question5 = new CS06_Campfire.Question("why");
			CS06_Campfire.Question question6 = new CS06_Campfire.Question("depression");
			CS06_Campfire.Question question7 = new CS06_Campfire.Question("defense");
			CS06_Campfire.Question question8 = new CS06_Campfire.Question("vacation");
			CS06_Campfire.Question question9 = new CS06_Campfire.Question("trust");
			CS06_Campfire.Question question10 = new CS06_Campfire.Question("family");
			CS06_Campfire.Question question11 = new CS06_Campfire.Question("grandpa");
			CS06_Campfire.Question question12 = new CS06_Campfire.Question("tips");
			CS06_Campfire.Question question13 = new CS06_Campfire.Question("selfie");
			CS06_Campfire.Question question14 = new CS06_Campfire.Question("sleep");
			CS06_Campfire.Question question15 = new CS06_Campfire.Question("sleep_confirm");
			CS06_Campfire.Question question16 = new CS06_Campfire.Question("sleep_cancel");
			this.nodes.Add("start", new CS06_Campfire.Option[]
			{
				new CS06_Campfire.Option(question, "start").ExcludedBy(new CS06_Campfire.Question[]
				{
					question5
				}),
				new CS06_Campfire.Option(question2, "start").Require(new CS06_Campfire.Question[]
				{
					question9
				}),
				new CS06_Campfire.Option(question9, "start").Require(new CS06_Campfire.Question[]
				{
					question3
				}),
				new CS06_Campfire.Option(question10, "start").Require(new CS06_Campfire.Question[]
				{
					question9,
					question5
				}),
				new CS06_Campfire.Option(question11, "start").Require(new CS06_Campfire.Question[]
				{
					question10,
					question7
				}),
				new CS06_Campfire.Option(question12, "start").Require(new CS06_Campfire.Question[]
				{
					question11
				}),
				new CS06_Campfire.Option(question3, "start"),
				new CS06_Campfire.Option(question4, "start").Require(new CS06_Campfire.Question[]
				{
					question3
				}),
				new CS06_Campfire.Option(question5, "start").Require(new CS06_Campfire.Question[]
				{
					question3,
					question9
				}),
				new CS06_Campfire.Option(question6, "start").Require(new CS06_Campfire.Question[]
				{
					question5
				}),
				new CS06_Campfire.Option(question7, "start").Require(new CS06_Campfire.Question[]
				{
					question6
				}),
				new CS06_Campfire.Option(question8, "start").Require(new CS06_Campfire.Question[]
				{
					question6
				}),
				new CS06_Campfire.Option(question13, "").Require(new CS06_Campfire.Question[]
				{
					question7,
					question11
				}),
				new CS06_Campfire.Option(question14, "sleep").Require(new CS06_Campfire.Question[]
				{
					question5
				}).ExcludedBy(new CS06_Campfire.Question[]
				{
					question7,
					question11
				}).Repeatable()
			});
			this.nodes.Add("sleep", new CS06_Campfire.Option[]
			{
				new CS06_Campfire.Option(question16, "start").Repeatable(),
				new CS06_Campfire.Option(question15, "")
			});
		}

		// Token: 0x06000DFF RID: 3583 RVA: 0x00032038 File Offset: 0x00030238
		public override void OnBegin(Level level)
		{
			Audio.SetMusic(null, false, false);
			level.SnapColorGrade(null);
			level.Bloom.Base = 0f;
			level.Session.SetFlag("duskbg", true);
			this.plateau = base.Scene.Entities.FindFirst<Plateau>();
			this.bonfire = base.Scene.Tracker.GetEntity<Bonfire>();
			level.Camera.Position = new Vector2((float)level.Bounds.Left, this.bonfire.Y - 144f);
			level.ZoomSnap(new Vector2(80f, 120f), 2f);
			this.cameraStart = level.Camera.Position;
			this.theo.X = level.Camera.X - 48f;
			this.theoCampfirePosition = new Vector2(this.bonfire.X - 16f, this.bonfire.Y);
			this.player.Light.Alpha = 0f;
			this.player.X = (float)(level.Bounds.Left - 40);
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			this.playerCampfirePosition = new Vector2(this.bonfire.X + 20f, this.bonfire.Y);
			if (level.Session.GetFlag("campfire_chat"))
			{
				this.WasSkipped = true;
				level.ResetZoom();
				level.EndCutscene();
				base.EndCutscene(level, true);
				return;
			}
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000E00 RID: 3584 RVA: 0x000321FD File Offset: 0x000303FD
		private IEnumerator PlayerLightApproach()
		{
			while (this.player.Light.Alpha < 1f)
			{
				this.player.Light.Alpha = Calc.Approach(this.player.Light.Alpha, 1f, Engine.DeltaTime * 2f);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000E01 RID: 3585 RVA: 0x0003220C File Offset: 0x0003040C
		private IEnumerator Cutscene(Level level)
		{
			yield return 0.1f;
			base.Add(new Coroutine(this.PlayerLightApproach(), true));
			Coroutine camTo;
			base.Add(camTo = new Coroutine(CutsceneEntity.CameraTo(new Vector2(level.Camera.X + 90f, level.Camera.Y), 6f, Ease.CubeIn, 0f), true));
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("carryTheoWalk", false, false);
			for (float p = 0f; p < 3.5f; p += Engine.DeltaTime)
			{
				SpotlightWipe.FocusPoint = new Vector2(40f, 120f);
				this.player.NaiveMove(new Vector2(32f * Engine.DeltaTime, 0f));
				yield return null;
			}
			this.player.Sprite.Play("carryTheoCollapse", false, false);
			Audio.Play("event:/char/madeline/theo_collapse", this.player.Position);
			yield return 0.3f;
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			Vector2 position = this.player.Position + new Vector2(16f, 1f);
			this.Level.ParticlesFG.Emit(Payphone.P_Snow, 2, position, Vector2.UnitX * 4f);
			this.Level.ParticlesFG.Emit(Payphone.P_SnowB, 12, position, Vector2.UnitX * 10f);
			yield return 0.7f;
			FadeWipe fade = new FadeWipe(level, false, null);
			fade.Duration = 1.5f;
			fade.EndTimer = 2.5f;
			yield return fade.Wait();
			this.bonfire.SetMode(Bonfire.Mode.Lit);
			yield return 2.45f;
			camTo.Cancel();
			this.theo.Position = this.theoCampfirePosition;
			this.theo.Sprite.Play("sleep", false, false);
			this.theo.Sprite.SetAnimationFrame(this.theo.Sprite.CurrentAnimationTotalFrames - 1);
			this.player.Position = this.playerCampfirePosition;
			this.player.Facing = Facings.Left;
			this.player.Sprite.Play("asleep", false, false);
			level.Session.SetFlag("starsbg", true);
			level.Session.SetFlag("duskbg", false);
			fade.EndTimer = 0f;
			new FadeWipe(level, true, null);
			yield return null;
			level.ResetZoom();
			level.Camera.Position = new Vector2(this.bonfire.X - 160f, this.bonfire.Y - 140f);
			yield return 3f;
			Audio.SetMusic("event:/music/lvl6/madeline_and_theo", true, true);
			yield return 1.5f;
			base.Add(Wiggler.Create(0.6f, 3f, delegate(float v)
			{
				this.theo.Sprite.Scale = Vector2.One * (1f + 0.1f * v);
			}, true, true));
			this.Level.Particles.Emit(NPC01_Theo.P_YOLO, 4, this.theo.Position + new Vector2(-4f, -14f), Vector2.One * 3f);
			yield return 0.5f;
			this.theo.Sprite.Play("wakeup", false, false);
			yield return 1f;
			this.player.Sprite.Play("halfWakeUp", false, false);
			yield return 0.25f;
			yield return Textbox.Say("ch6_theo_intro", new Func<IEnumerator>[0]);
			string text = "start";
			while (!string.IsNullOrEmpty(text) && this.nodes.ContainsKey(text))
			{
				this.currentOptionIndex = 0;
				this.currentOptions = new List<CS06_Campfire.Option>();
				foreach (CS06_Campfire.Option option in this.nodes[text])
				{
					if (option.CanAsk(this.asked))
					{
						this.currentOptions.Add(option);
					}
				}
				if (this.currentOptions.Count <= 0)
				{
					break;
				}
				Audio.Play("event:/ui/game/chatoptions_appear");
				while ((this.optionEase += Engine.DeltaTime * 4f) < 1f)
				{
					yield return null;
				}
				this.optionEase = 1f;
				yield return 0.25f;
				while (!Input.MenuConfirm.Pressed)
				{
					if (Input.MenuUp.Pressed && this.currentOptionIndex > 0)
					{
						Audio.Play("event:/ui/game/chatoptions_roll_up");
						this.currentOptionIndex--;
					}
					else if (Input.MenuDown.Pressed && this.currentOptionIndex < this.currentOptions.Count - 1)
					{
						Audio.Play("event:/ui/game/chatoptions_roll_down");
						this.currentOptionIndex++;
					}
					yield return null;
				}
				Audio.Play("event:/ui/game/chatoptions_select");
				while ((this.optionEase -= Engine.DeltaTime * 4f) > 0f)
				{
					yield return null;
				}
				CS06_Campfire.Option selected = this.currentOptions[this.currentOptionIndex];
				this.asked.Add(selected.Question);
				this.currentOptions = null;
				yield return Textbox.Say(selected.Question.Answer, new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(this.WaitABit),
					new Func<IEnumerator>(this.SelfieSequence),
					new Func<IEnumerator>(this.BeerSequence)
				});
				text = selected.Goto;
				if (string.IsNullOrEmpty(text))
				{
					break;
				}
				selected = null;
			}
			yield return new FadeWipe(level, false, null)
			{
				Duration = 3f
			}.Wait();
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000E02 RID: 3586 RVA: 0x00032222 File Offset: 0x00030422
		private IEnumerator WaitABit()
		{
			yield return 0.8f;
			yield break;
		}

		// Token: 0x06000E03 RID: 3587 RVA: 0x0003222A File Offset: 0x0003042A
		private IEnumerator SelfieSequence()
		{
			base.Add(new Coroutine(this.Level.ZoomTo(new Vector2(160f, 105f), 2f, 0.5f), true));
			yield return 0.1f;
			this.theo.Sprite.Play("idle", false, false);
			base.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate
			{
				this.theo.Sprite.Scale.X = -1f;
			}, 0.25f, true));
			this.player.DummyAutoAnimate = true;
			yield return this.player.DummyWalkToExact((int)(this.theo.X + 5f), false, 0.7f, false);
			yield return 0.2f;
			Audio.Play("event:/game/02_old_site/theoselfie_foley", this.theo.Position);
			this.theo.Sprite.Play("takeSelfie", false, false);
			yield return 1f;
			this.selfie = new Selfie(base.SceneAs<Level>());
			base.Scene.Add(this.selfie);
			yield return this.selfie.PictureRoutine("selfieCampfire");
			this.selfie = null;
			yield return 0.5f;
			yield return this.Level.ZoomBack(0.5f);
			yield return 0.2f;
			this.theo.Sprite.Scale.X = 1f;
			yield return this.player.DummyWalkToExact((int)this.playerCampfirePosition.X, false, 0.7f, false);
			this.theo.Sprite.Play("wakeup", false, false);
			yield return 0.1;
			this.player.DummyAutoAnimate = false;
			this.player.Facing = Facings.Left;
			this.player.Sprite.Play("sleep", false, false);
			yield return 2f;
			this.player.Sprite.Play("halfWakeUp", false, false);
			yield break;
		}

		// Token: 0x06000E04 RID: 3588 RVA: 0x00032239 File Offset: 0x00030439
		private IEnumerator BeerSequence()
		{
			yield return 0.5f;
			yield break;
		}

		// Token: 0x06000E05 RID: 3589 RVA: 0x00032244 File Offset: 0x00030444
		public override void OnEnd(Level level)
		{
			if (!this.WasSkipped)
			{
				level.ZoomSnap(new Vector2(160f, 120f), 2f);
				FadeWipe fadeWipe = new FadeWipe(level, true, null);
				fadeWipe.Duration = 3f;
				Coroutine zoom = new Coroutine(level.ZoomBack(fadeWipe.Duration), true);
				fadeWipe.OnUpdate = delegate(float f)
				{
					zoom.Update();
				};
			}
			if (this.selfie != null)
			{
				this.selfie.RemoveSelf();
			}
			level.Session.SetFlag("campfire_chat", true);
			level.Session.SetFlag("starsbg", false);
			level.Session.SetFlag("duskbg", false);
			level.Session.Dreaming = true;
			level.Add(new StarJumpController());
			level.Add(new CS06_StarJumpEnd(this.theo, this.player, this.playerCampfirePosition, this.cameraStart));
			level.Add(new FlyFeather(level.LevelOffset + new Vector2(272f, 2616f), false, false));
			this.SetBloom(1f);
			this.bonfire.Activated = false;
			this.bonfire.SetMode(Bonfire.Mode.Lit);
			this.theo.Sprite.Play("sleep", false, false);
			this.theo.Sprite.SetAnimationFrame(this.theo.Sprite.CurrentAnimationTotalFrames - 1);
			this.theo.Sprite.Scale.X = 1f;
			this.theo.Position = this.theoCampfirePosition;
			this.player.Sprite.Play("asleep", false, false);
			this.player.Position = this.playerCampfirePosition;
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 15;
			this.player.Speed = Vector2.Zero;
			this.player.Facing = Facings.Left;
			level.Camera.Position = this.player.CameraTarget;
			if (this.WasSkipped)
			{
				this.player.StateMachine.State = 0;
			}
			base.RemoveSelf();
		}

		// Token: 0x06000E06 RID: 3590 RVA: 0x0002CB61 File Offset: 0x0002AD61
		private void SetBloom(float add)
		{
			this.Level.Session.BloomBaseAdd = add;
			this.Level.Bloom.Base = AreaData.Get(this.Level).BloomBase + add;
		}

		// Token: 0x06000E07 RID: 3591 RVA: 0x00032484 File Offset: 0x00030684
		public override void Update()
		{
			if (this.currentOptions != null)
			{
				for (int i = 0; i < this.currentOptions.Count; i++)
				{
					this.currentOptions[i].Update();
					this.currentOptions[i].Highlight = Calc.Approach(this.currentOptions[i].Highlight, (float)((this.currentOptionIndex == i) ? 1 : 0), Engine.DeltaTime * 4f);
				}
			}
			base.Update();
		}

		// Token: 0x06000E08 RID: 3592 RVA: 0x00032508 File Offset: 0x00030708
		public override void Render()
		{
			if (this.Level.Paused)
			{
				return;
			}
			if (this.currentOptions != null)
			{
				int num = 0;
				foreach (CS06_Campfire.Option option in this.currentOptions)
				{
					Vector2 position = new Vector2(260f, 120f + 160f * (float)num);
					option.Render(position, this.optionEase);
					num++;
				}
			}
		}

		// Token: 0x0400093D RID: 2365
		public const string Flag = "campfire_chat";

		// Token: 0x0400093E RID: 2366
		public const string DuskBackgroundFlag = "duskbg";

		// Token: 0x0400093F RID: 2367
		public const string StarsBackgrundFlag = "starsbg";

		// Token: 0x04000940 RID: 2368
		private NPC theo;

		// Token: 0x04000941 RID: 2369
		private Player player;

		// Token: 0x04000942 RID: 2370
		private Bonfire bonfire;

		// Token: 0x04000943 RID: 2371
		private Plateau plateau;

		// Token: 0x04000944 RID: 2372
		private Vector2 cameraStart;

		// Token: 0x04000945 RID: 2373
		private Vector2 playerCampfirePosition;

		// Token: 0x04000946 RID: 2374
		private Vector2 theoCampfirePosition;

		// Token: 0x04000947 RID: 2375
		private Selfie selfie;

		// Token: 0x04000948 RID: 2376
		private float optionEase;

		// Token: 0x04000949 RID: 2377
		private Dictionary<string, CS06_Campfire.Option[]> nodes = new Dictionary<string, CS06_Campfire.Option[]>();

		// Token: 0x0400094A RID: 2378
		private HashSet<CS06_Campfire.Question> asked = new HashSet<CS06_Campfire.Question>();

		// Token: 0x0400094B RID: 2379
		private List<CS06_Campfire.Option> currentOptions = new List<CS06_Campfire.Option>();

		// Token: 0x0400094C RID: 2380
		private int currentOptionIndex;

		// Token: 0x0200047D RID: 1149
		private class Option
		{
			// Token: 0x060022A3 RID: 8867 RVA: 0x000EB1AA File Offset: 0x000E93AA
			public Option(CS06_Campfire.Question question, string go)
			{
				this.Question = question;
				this.Goto = go;
			}

			// Token: 0x060022A4 RID: 8868 RVA: 0x000EB1C0 File Offset: 0x000E93C0
			public CS06_Campfire.Option Require(params CS06_Campfire.Question[] onlyAppearIfAsked)
			{
				this.OnlyAppearIfAsked = new List<CS06_Campfire.Question>(onlyAppearIfAsked);
				return this;
			}

			// Token: 0x060022A5 RID: 8869 RVA: 0x000EB1CF File Offset: 0x000E93CF
			public CS06_Campfire.Option ExcludedBy(params CS06_Campfire.Question[] doNotAppearIfAsked)
			{
				this.DoNotAppearIfAsked = new List<CS06_Campfire.Question>(doNotAppearIfAsked);
				return this;
			}

			// Token: 0x060022A6 RID: 8870 RVA: 0x000EB1DE File Offset: 0x000E93DE
			public CS06_Campfire.Option Repeatable()
			{
				this.CanRepeat = true;
				return this;
			}

			// Token: 0x060022A7 RID: 8871 RVA: 0x000EB1E8 File Offset: 0x000E93E8
			public bool CanAsk(HashSet<CS06_Campfire.Question> asked)
			{
				if (!this.CanRepeat && asked.Contains(this.Question))
				{
					return false;
				}
				if (this.OnlyAppearIfAsked != null)
				{
					foreach (CS06_Campfire.Question item in this.OnlyAppearIfAsked)
					{
						if (!asked.Contains(item))
						{
							return false;
						}
					}
				}
				if (this.DoNotAppearIfAsked != null)
				{
					bool flag = true;
					foreach (CS06_Campfire.Question item2 in this.DoNotAppearIfAsked)
					{
						if (!asked.Contains(item2))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x060022A8 RID: 8872 RVA: 0x000EB2C0 File Offset: 0x000E94C0
			public void Update()
			{
				this.Question.Portrait.Update();
			}

			// Token: 0x060022A9 RID: 8873 RVA: 0x000EB2D4 File Offset: 0x000E94D4
			public void Render(Vector2 position, float ease)
			{
				float num = Ease.CubeOut(ease);
				float num2 = Ease.CubeInOut(this.Highlight);
				position.Y += -32f * (1f - num);
				position.X += num2 * 32f;
				Color color = Color.Lerp(Color.Gray, Color.White, num2) * num;
				float alpha = MathHelper.Lerp(0.6f, 1f, num2) * num;
				Color value = Color.White * (0.5f + num2 * 0.5f);
				GFX.Portraits[this.Question.Textbox].Draw(position, Vector2.Zero, color);
				Facings facings = this.Question.PortraitSide;
				if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
				{
					facings = -facings;
				}
				float num3 = 100f;
				this.Question.Portrait.Scale = Vector2.One * (num3 / this.Question.PortraitSize);
				if (facings == Facings.Right)
				{
					this.Question.Portrait.Position = position + new Vector2(1380f - num3 * 0.5f, 70f);
					Sprite portrait = this.Question.Portrait;
					portrait.Scale.X = portrait.Scale.X * -1f;
				}
				else
				{
					this.Question.Portrait.Position = position + new Vector2(20f + num3 * 0.5f, 70f);
				}
				this.Question.Portrait.Color = value * num;
				this.Question.Portrait.Render();
				float num4 = (140f - ActiveFont.LineHeight * 0.7f) / 2f;
				Vector2 position2 = new Vector2(0f, position.Y + 70f);
				Vector2 justify = new Vector2(0f, 0.5f);
				if (facings == Facings.Right)
				{
					justify.X = 1f;
					position2.X = position.X + 1400f - 20f - num4 - num3;
				}
				else
				{
					position2.X = position.X + 20f + num4 + num3;
				}
				this.Question.AskText.Draw(position2, justify, Vector2.One * 0.7f, alpha, 0, int.MaxValue);
			}

			// Token: 0x04002268 RID: 8808
			public CS06_Campfire.Question Question;

			// Token: 0x04002269 RID: 8809
			public string Goto;

			// Token: 0x0400226A RID: 8810
			public List<CS06_Campfire.Question> OnlyAppearIfAsked;

			// Token: 0x0400226B RID: 8811
			public List<CS06_Campfire.Question> DoNotAppearIfAsked;

			// Token: 0x0400226C RID: 8812
			public bool CanRepeat;

			// Token: 0x0400226D RID: 8813
			public float Highlight;

			// Token: 0x0400226E RID: 8814
			public const float Width = 1400f;

			// Token: 0x0400226F RID: 8815
			public const float Height = 140f;

			// Token: 0x04002270 RID: 8816
			public const float Padding = 20f;

			// Token: 0x04002271 RID: 8817
			public const float TextScale = 0.7f;
		}

		// Token: 0x0200047E RID: 1150
		private class Question
		{
			// Token: 0x060022AA RID: 8874 RVA: 0x000EB548 File Offset: 0x000E9748
			public Question(string id)
			{
				int maxLineWidth = 1828;
				this.Ask = "ch6_theo_ask_" + id;
				this.Answer = "ch6_theo_say_" + id;
				this.AskText = FancyText.Parse(Dialog.Get(this.Ask, null), maxLineWidth, -1, 1f, null, null);
				foreach (FancyText.Node node in this.AskText.Nodes)
				{
					if (node is FancyText.Portrait)
					{
						FancyText.Portrait portrait = node as FancyText.Portrait;
						this.Portrait = GFX.PortraitsSpriteBank.Create(portrait.SpriteId);
						this.Portrait.Play(portrait.IdleAnimation, false, false);
						this.PortraitSide = (Facings)portrait.Side;
						this.Textbox = "textbox/" + portrait.Sprite + "_ask";
						XmlElement xml = GFX.PortraitsSpriteBank.SpriteData[portrait.SpriteId].Sources[0].XML;
						if (xml != null)
						{
							this.PortraitSize = (float)xml.AttrInt("size", 160);
							break;
						}
						break;
					}
				}
			}

			// Token: 0x04002272 RID: 8818
			public string Ask;

			// Token: 0x04002273 RID: 8819
			public string Answer;

			// Token: 0x04002274 RID: 8820
			public string Textbox;

			// Token: 0x04002275 RID: 8821
			public FancyText.Text AskText;

			// Token: 0x04002276 RID: 8822
			public Sprite Portrait;

			// Token: 0x04002277 RID: 8823
			public Facings PortraitSide;

			// Token: 0x04002278 RID: 8824
			public float PortraitSize;
		}
	}
}
