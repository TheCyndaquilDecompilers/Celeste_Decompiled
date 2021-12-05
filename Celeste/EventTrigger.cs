using System;
using System.Collections;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200034C RID: 844
	public class EventTrigger : Trigger
	{
		// Token: 0x06001A7E RID: 6782 RVA: 0x000AABE5 File Offset: 0x000A8DE5
		public EventTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.Event = data.Attr("event", "");
			this.OnSpawnHack = data.Bool("onSpawn", false);
		}

		// Token: 0x06001A7F RID: 6783 RVA: 0x000AAC18 File Offset: 0x000A8E18
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (this.OnSpawnHack)
			{
				Player player = base.CollideFirst<Player>();
				if (player != null)
				{
					this.OnEnter(player);
				}
			}
			if (this.Event == "ch9_badeline_helps")
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.Left > base.Right)
				{
					base.RemoveSelf();
				}
			}
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06001A80 RID: 6784 RVA: 0x000AAC7F File Offset: 0x000A8E7F
		// (set) Token: 0x06001A81 RID: 6785 RVA: 0x000AAC87 File Offset: 0x000A8E87
		public float Time { get; private set; }

		// Token: 0x06001A82 RID: 6786 RVA: 0x000AAC90 File Offset: 0x000A8E90
		public override void OnEnter(Player player)
		{
			if (this.triggered)
			{
				return;
			}
			this.triggered = true;
			Level level = base.Scene as Level;
			string @event = this.Event;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(@event);
			if (num <= 945633221U)
			{
				if (num <= 407496794U)
				{
					if (num <= 67053190U)
					{
						if (num != 34525938U)
						{
							if (num != 67053190U)
							{
								goto IL_81B;
							}
							if (!(@event == "ch9_farewell"))
							{
								goto IL_81B;
							}
							base.Scene.Add(new CS10_Farewell(player));
							return;
						}
						else
						{
							if (!(@event == "ch9_hub_transition_out"))
							{
								goto IL_81B;
							}
							base.Add(new Coroutine(this.Ch9HubTransitionBackgroundToBright(player), true));
							return;
						}
					}
					else if (num != 317372563U)
					{
						if (num != 354461440U)
						{
							if (num != 407496794U)
							{
								goto IL_81B;
							}
							if (!(@event == "ch6_reflect"))
							{
								goto IL_81B;
							}
							if (!level.Session.GetFlag("reflection"))
							{
								base.Scene.Add(new CS06_Reflection(player, base.Center.X - 5f));
								return;
							}
							return;
						}
						else
						{
							if (!(@event == "end_city"))
							{
								goto IL_81B;
							}
							base.Scene.Add(new CS01_Ending(player));
							return;
						}
					}
					else
					{
						if (!(@event == "ch5_found_theo"))
						{
							goto IL_81B;
						}
						if (!level.Session.GetFlag("foundTheoInCrystal"))
						{
							base.Scene.Add(new CS05_SaveTheo(player));
							return;
						}
						return;
					}
				}
				else if (num <= 447110648U)
				{
					if (num != 408446935U)
					{
						if (num != 444575575U)
						{
							if (num != 447110648U)
							{
								goto IL_81B;
							}
							if (!(@event == "ch9_hub_intro"))
							{
								goto IL_81B;
							}
							if (!level.Session.GetFlag("hub_intro"))
							{
								base.Scene.Add(new CS10_HubIntro(base.Scene, player));
								return;
							}
							return;
						}
						else
						{
							if (!(@event == "end_oldsite_dream"))
							{
								goto IL_81B;
							}
							base.Scene.Add(new CS02_DreamingPhonecall(player));
							return;
						}
					}
					else
					{
						if (!(@event == "end_oldsite_awake"))
						{
							goto IL_81B;
						}
						base.Scene.Add(new CS02_Ending(player));
						return;
					}
				}
				else if (num != 503736449U)
				{
					if (num != 567650949U)
					{
						if (num != 945633221U)
						{
							goto IL_81B;
						}
						if (!(@event == "ch9_goto_the_future"))
						{
							goto IL_81B;
						}
					}
					else
					{
						if (!(@event == "ch6_boss_intro"))
						{
							goto IL_81B;
						}
						if (!level.Session.GetFlag("boss_intro"))
						{
							level.Add(new CS06_BossIntro(base.Center.X, player, level.Entities.FindFirst<FinalBoss>()));
							return;
						}
						return;
					}
				}
				else
				{
					if (!(@event == "ch9_end_golden"))
					{
						goto IL_81B;
					}
					ScreenWipe.WipeColor = Color.White;
					Action <>9__2;
					new FadeWipe(level, false, delegate()
					{
						Scene level = level;
						Action value;
						if ((value = <>9__2) == null)
						{
							value = (<>9__2 = delegate()
							{
								level.TeleportTo(player, "end-granny", Player.IntroTypes.Transition, null);
								player.Speed = Vector2.Zero;
							});
						}
						level.OnEndOfFrame += value;
					}).Duration = 1f;
					return;
				}
			}
			else if (num <= 2712658756U)
			{
				if (num <= 2120337951U)
				{
					if (num != 1609386425U)
					{
						if (num != 1800778982U)
						{
							if (num != 2120337951U)
							{
								goto IL_81B;
							}
							if (!(@event == "ch8_door"))
							{
								goto IL_81B;
							}
							base.Scene.Add(new CS08_EnterDoor(player, base.Left));
							return;
						}
						else
						{
							if (!(@event == "ch5_see_theo"))
							{
								goto IL_81B;
							}
							if (!(base.Scene as Level).Session.GetFlag("seeTheoInCrystal"))
							{
								base.Scene.Add(new CS05_SeeTheo(player, 0));
								return;
							}
							return;
						}
					}
					else
					{
						if (!(@event == "ch9_badeline_helps"))
						{
							goto IL_81B;
						}
						if (!level.Session.GetFlag("badeline_helps"))
						{
							base.Scene.Add(new CS10_BadelineHelps(player));
							return;
						}
						return;
					}
				}
				else if (num != 2310820142U)
				{
					if (num != 2401869435U)
					{
						if (num != 2712658756U)
						{
							goto IL_81B;
						}
						if (!(@event == "ch9_golden_snapshot"))
						{
							goto IL_81B;
						}
						this.snapshot = Audio.CreateSnapshot("snapshot:/game_10_golden_room_flavour", true);
						(base.Scene as Level).SnapColorGrade("golden");
						return;
					}
					else
					{
						if (!(@event == "ch7_summit"))
						{
							goto IL_81B;
						}
						base.Scene.Add(new CS07_Ending(player, new Vector2(base.Center.X, base.Bottom)));
						return;
					}
				}
				else
				{
					if (!(@event == "ch9_moon_intro"))
					{
						goto IL_81B;
					}
					if (!level.Session.GetFlag("moon_intro") && player.StateMachine.State == 13)
					{
						base.Scene.Add(new CS10_MoonIntro(player));
						return;
					}
					BirdNPC birdNPC = level.Entities.FindFirst<BirdNPC>();
					if (birdNPC != null)
					{
						birdNPC.RemoveSelf();
					}
					level.Session.Inventory.Dashes = 1;
					player.Dashes = 1;
					return;
				}
			}
			else if (num <= 3890474523U)
			{
				if (num != 3048411069U)
				{
					if (num != 3823623439U)
					{
						if (num != 3890474523U)
						{
							goto IL_81B;
						}
						if (!(@event == "cancel_ch5_see_theo"))
						{
							goto IL_81B;
						}
						level.Session.SetFlag("it_ch5_see_theo", true);
						level.Session.SetFlag("it_ch5_see_theo_b", true);
						level.Session.SetFlag("ignore_darkness_" + level.Session.Level, true);
						base.Add(new Coroutine(this.Brighten(), true));
						return;
					}
					else
					{
						if (!(@event == "ch5_mirror_reflection"))
						{
							goto IL_81B;
						}
						if (!level.Session.GetFlag("reflection"))
						{
							base.Scene.Add(new CS05_Reflection1(player));
							return;
						}
						return;
					}
				}
				else
				{
					if (!(@event == "ch9_ending"))
					{
						goto IL_81B;
					}
					base.Scene.Add(new CS10_Ending(player));
					return;
				}
			}
			else if (num != 4058992408U)
			{
				if (num != 4074666886U)
				{
					if (num != 4100515604U)
					{
						goto IL_81B;
					}
					if (!(@event == "ch9_ding_ding_ding"))
					{
						goto IL_81B;
					}
					Audio.Play("event:/new_content/game/10_farewell/pico8_flag", base.Center);
					Decal decal = null;
					foreach (Decal decal2 in base.Scene.Entities.FindAll<Decal>())
					{
						if (decal2.Name.ToLower() == "decals/10-farewell/finalflag")
						{
							decal = decal2;
							break;
						}
					}
					if (decal != null)
					{
						decal.FinalFlagTrigger();
						return;
					}
					return;
				}
				else if (!(@event == "ch9_goto_the_past"))
				{
					goto IL_81B;
				}
			}
			else
			{
				if (!(@event == "ch9_final_room"))
				{
					goto IL_81B;
				}
				Session session = (base.Scene as Level).Session;
				int counter = session.GetCounter("final_room_deaths");
				if (counter == 0)
				{
					base.Scene.Add(new CS10_FinalRoom(player, true));
				}
				else if (counter == 50)
				{
					base.Scene.Add(new CS10_FinalRoom(player, false));
				}
				session.IncrementCounter("final_room_deaths");
				return;
			}
			level.OnEndOfFrame += delegate()
			{
				new Vector2(level.LevelOffset.X + (float)level.Bounds.Width - player.X, player.Y - level.LevelOffset.Y);
				Vector2 levelOffset = level.LevelOffset;
				Vector2 value = player.Position - level.LevelOffset;
				Vector2 value2 = level.Camera.Position - level.LevelOffset;
				Facings facing = player.Facing;
				level.Remove(player);
				level.UnloadLevel();
				level.Session.Dreaming = true;
				level.Session.Level = ((this.Event == "ch9_goto_the_future") ? "intro-01-future" : "intro-00-past");
				level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float)level.Bounds.Left, (float)level.Bounds.Top)));
				level.Session.FirstLevel = false;
				level.LoadLevel(Player.IntroTypes.Transition, false);
				level.Camera.Position = level.LevelOffset + value2;
				level.Session.Inventory.Dashes = 1;
				player.Dashes = Math.Min(player.Dashes, 1);
				level.Add(player);
				player.Position = level.LevelOffset + value;
				player.Facing = facing;
				player.Hair.MoveHairBy(level.LevelOffset - levelOffset);
				if (level.Wipe != null)
				{
					level.Wipe.Cancel();
				}
				level.Flash(Color.White, false);
				level.Shake(0.3f);
				level.Add(new LightningStrike(new Vector2(player.X + 60f, (float)(level.Bounds.Bottom - 180)), 10, 200f, 0f));
				level.Add(new LightningStrike(new Vector2(player.X + 220f, (float)(level.Bounds.Bottom - 180)), 40, 200f, 0.25f));
				Audio.Play("event:/new_content/game/10_farewell/lightning_strike");
			};
			return;
			IL_81B:
			throw new Exception("Event '" + this.Event + "' does not exist!");
		}

		// Token: 0x06001A83 RID: 6787 RVA: 0x000AB4E4 File Offset: 0x000A96E4
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			Audio.ReleaseSnapshot(this.snapshot);
		}

		// Token: 0x06001A84 RID: 6788 RVA: 0x000AB4F8 File Offset: 0x000A96F8
		public override void SceneEnd(Scene scene)
		{
			base.SceneEnd(scene);
			Audio.ReleaseSnapshot(this.snapshot);
		}

		// Token: 0x06001A85 RID: 6789 RVA: 0x000AB50C File Offset: 0x000A970C
		private IEnumerator Brighten()
		{
			Level level = base.Scene as Level;
			float darkness = AreaData.Get(level).DarknessAlpha;
			while (level.Lighting.Alpha != darkness)
			{
				level.Lighting.Alpha = Calc.Approach(level.Lighting.Alpha, darkness, Engine.DeltaTime * 4f);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001A86 RID: 6790 RVA: 0x000AB51B File Offset: 0x000A971B
		private IEnumerator Ch9HubTransitionBackgroundToBright(Player player)
		{
			Level level = base.Scene as Level;
			float start = base.Bottom;
			float end = base.Top;
			for (;;)
			{
				float fadeAlphaMultiplier = Calc.ClampedMap(player.Y, start, end, 0f, 1f);
				foreach (Backdrop backdrop in level.Background.GetEach<Backdrop>("bright"))
				{
					backdrop.ForceVisible = true;
					backdrop.FadeAlphaMultiplier = fadeAlphaMultiplier;
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x04001723 RID: 5923
		public string Event;

		// Token: 0x04001724 RID: 5924
		public bool OnSpawnHack;

		// Token: 0x04001725 RID: 5925
		private bool triggered;

		// Token: 0x04001726 RID: 5926
		private EventInstance snapshot;
	}
}
