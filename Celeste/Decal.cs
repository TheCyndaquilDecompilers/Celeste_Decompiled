using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002E8 RID: 744
	public class Decal : Entity
	{
		// Token: 0x060016E5 RID: 5861 RVA: 0x00088F30 File Offset: 0x00087130
		public Decal(string texture, Vector2 position, Vector2 scale, int depth) : base(position)
		{
			base.Depth = depth;
			this.scale = scale;
			string extension = Path.GetExtension(texture);
			string input = Path.Combine("decals", texture.Replace(extension, "")).Replace('\\', '/');
			this.Name = Regex.Replace(input, "\\d+$", string.Empty);
			this.textures = GFX.Game.GetAtlasSubtextures(this.Name);
		}

		// Token: 0x060016E6 RID: 5862 RVA: 0x00088FB8 File Offset: 0x000871B8
		public override void Added(Scene scene)
		{
			base.Added(scene);
			string text = this.Name.ToLower().Replace("decals/", "");
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 2354802830U)
			{
				if (num > 784572981U)
				{
					if (num <= 1357832045U)
					{
						if (num <= 1167845130U)
						{
							if (num <= 918793933U)
							{
								if (num != 868461076U)
								{
									if (num != 885238695U)
									{
										if (num != 918793933U)
										{
											goto IL_16C5;
										}
										if (!(text == "10-farewell/clouds/cloud_j"))
										{
											goto IL_16C5;
										}
										goto IL_163C;
									}
									else
									{
										if (!(text == "10-farewell/clouds/cloud_h"))
										{
											goto IL_16C5;
										}
										goto IL_163C;
									}
								}
								else
								{
									if (!(text == "10-farewell/clouds/cloud_i"))
									{
										goto IL_16C5;
									}
									goto IL_163C;
								}
							}
							else if (num <= 1083957035U)
							{
								if (num != 1070258815U)
								{
									if (num != 1083957035U)
									{
										goto IL_16C5;
									}
									if (!(text == "9-core/heart_bevel_d"))
									{
										goto IL_16C5;
									}
								}
								else
								{
									if (!(text == "10-farewell/glitch_c"))
									{
										goto IL_16C5;
									}
									goto IL_166A;
								}
							}
							else if (num != 1134289892U)
							{
								if (num != 1167845130U)
								{
									goto IL_16C5;
								}
								if (!(text == "9-core/heart_bevel_c"))
								{
									goto IL_16C5;
								}
							}
							else if (!(text == "9-core/heart_bevel_a"))
							{
								goto IL_16C5;
							}
						}
						else if (num <= 1303447157U)
						{
							if (num <= 1184622749U)
							{
								if (num != 1180110792U)
								{
									if (num != 1184622749U)
									{
										goto IL_16C5;
									}
									if (!(text == "9-core/heart_bevel_b"))
									{
										goto IL_16C5;
									}
								}
								else
								{
									if (!(text == "6-reflection/crystal_reflection"))
									{
										goto IL_16C5;
									}
									this.MakeMirrorSpecialCase(text, new Vector2(-12f, 2f));
									goto IL_16C5;
								}
							}
							else if (num != 1273943950U)
							{
								if (num != 1303447157U)
								{
									goto IL_16C5;
								}
								if (!(text == "10-farewell/tower"))
								{
									goto IL_16C5;
								}
								goto IL_1688;
							}
							else
							{
								if (!(text == "10-farewell/coral_d"))
								{
									goto IL_16C5;
								}
								goto IL_1631;
							}
						}
						else if (num <= 1318071190U)
						{
							if (num != 1307499188U)
							{
								if (num != 1318071190U)
								{
									goto IL_16C5;
								}
								if (!(text == "3-resort/curtain_side_d"))
								{
									goto IL_16C5;
								}
								goto IL_130B;
							}
							else
							{
								if (!(text == "10-farewell/coral_b"))
								{
									goto IL_16C5;
								}
								goto IL_1631;
							}
						}
						else if (num != 1324276807U)
						{
							if (num != 1357832045U)
							{
								goto IL_16C5;
							}
							if (!(text == "10-farewell/coral_a"))
							{
								goto IL_16C5;
							}
							goto IL_1631;
						}
						else
						{
							if (!(text == "10-farewell/coral_c"))
							{
								goto IL_16C5;
							}
							goto IL_1631;
						}
						this.scale.Y = 1f;
						this.scale.X = 1f;
						goto IL_16C5;
					}
					if (num <= 1666259448U)
					{
						if (num <= 1588012304U)
						{
							if (num <= 1387907411U)
							{
								if (num != 1371129792U)
								{
									if (num != 1387907411U)
									{
										goto IL_16C5;
									}
									if (!(text == "5-temple/bg_mirror_shard_group_a_c"))
									{
										goto IL_16C5;
									}
									goto IL_1467;
								}
								else
								{
									if (!(text == "5-temple/bg_mirror_shard_group_a_b"))
									{
										goto IL_16C5;
									}
									goto IL_1467;
								}
							}
							else if (num != 1401959285U)
							{
								if (num != 1588012304U)
								{
									goto IL_16C5;
								}
								if (!(text == "5-temple/bg_mirror_c"))
								{
									goto IL_16C5;
								}
								goto IL_1484;
							}
							else
							{
								if (!(text == "3-resort/curtain_side_a"))
								{
									goto IL_16C5;
								}
								goto IL_130B;
							}
						}
						else if (num <= 1621567542U)
						{
							if (num != 1604789923U)
							{
								if (num != 1621567542U)
								{
									goto IL_16C5;
								}
								if (!(text == "5-temple/bg_mirror_a"))
								{
									goto IL_16C5;
								}
								goto IL_1467;
							}
							else
							{
								if (!(text == "5-temple/bg_mirror_b"))
								{
									goto IL_16C5;
								}
								goto IL_1467;
							}
						}
						else if (num != 1647656243U)
						{
							if (num != 1666259448U)
							{
								goto IL_16C5;
							}
							if (!(text == "9-core/ball_a"))
							{
								goto IL_16C5;
							}
							base.Add(this.image = new Decal.CoreSwapImage(this.textures[0], GFX.Game["decals/9-core/ball_a_ice"]));
							goto IL_16C5;
						}
						else
						{
							if (!(text == "5-temple-dark/mosaic_b"))
							{
								goto IL_16C5;
							}
							base.Add(new BloomPoint(new Vector2(0f, 5f), 0.75f, 16f));
							goto IL_16C5;
						}
					}
					else if (num <= 2177459677U)
					{
						if (num <= 2060016344U)
						{
							if (num != 1796380340U)
							{
								if (num != 2060016344U)
								{
									goto IL_16C5;
								}
								if (!(text == "3-resort/roofcenter_c"))
								{
									goto IL_16C5;
								}
								goto IL_132D;
							}
							else
							{
								if (!(text == "9-core/rock_e"))
								{
									goto IL_16C5;
								}
								base.Add(this.image = new Decal.CoreSwapImage(this.textures[0], GFX.Game["decals/9-core/rock_e_ice"]));
								goto IL_16C5;
							}
						}
						else if (num != 2076793963U)
						{
							if (num != 2177459677U)
							{
								goto IL_16C5;
							}
							if (!(text == "3-resort/roofcenter_d"))
							{
								goto IL_16C5;
							}
							goto IL_132D;
						}
						else
						{
							if (!(text == "3-resort/roofcenter_b"))
							{
								goto IL_16C5;
							}
							goto IL_132D;
						}
					}
					else if (num <= 2319792170U)
					{
						if (num != 2301192480U)
						{
							if (num != 2319792170U)
							{
								goto IL_16C5;
							}
							if (!(text == "10-farewell/floating house"))
							{
								goto IL_16C5;
							}
							goto IL_1688;
						}
						else
						{
							if (!(text == "4-cliffside/flower_d"))
							{
								goto IL_16C5;
							}
							goto IL_1445;
						}
					}
					else if (num != 2354258377U)
					{
						if (num != 2354802830U)
						{
							goto IL_16C5;
						}
						if (!(text == "10-farewell/glitch_a_"))
						{
							goto IL_16C5;
						}
					}
					else if (!(text == "10-farewell/glitch_b_"))
					{
						goto IL_16C5;
					}
					IL_166A:
					this.frame = Calc.Random.NextFloat((float)this.textures.Count);
					goto IL_16C5;
				}
				if (num <= 420141667U)
				{
					if (num <= 232572921U)
					{
						if (num <= 47469209U)
						{
							if (num != 1686930U)
							{
								if (num != 18464549U)
								{
									if (num != 47469209U)
									{
										goto IL_16C5;
									}
									if (!(text == "4-cliffside/bridge_a"))
									{
										goto IL_16C5;
									}
									this.MakeSolid(-24f, 0f, 48f, 8f, 8, base.Depth != 9000);
									goto IL_16C5;
								}
								else
								{
									if (!(text == "7-summit/cloud_bb"))
									{
										goto IL_16C5;
									}
									goto IL_14D5;
								}
							}
							else
							{
								if (!(text == "7-summit/cloud_bc"))
								{
									goto IL_16C5;
								}
								goto IL_14D5;
							}
						}
						else if (num <= 182240064U)
						{
							if (num != 105373372U)
							{
								if (num != 182240064U)
								{
									goto IL_16C5;
								}
								if (!(text == "10-farewell/creature_f"))
								{
									goto IL_16C5;
								}
							}
							else
							{
								if (!(text == "9-core/rock_e_ice"))
								{
									goto IL_16C5;
								}
								base.Add(this.image = new Decal.CoreSwapImage(GFX.Game["decals/9-core/rock_e"], this.textures[0]));
								goto IL_16C5;
							}
						}
						else if (num != 215795302U)
						{
							if (num != 232572921U)
							{
								goto IL_16C5;
							}
							if (!(text == "10-farewell/creature_e"))
							{
								goto IL_16C5;
							}
						}
						else if (!(text == "10-farewell/creature_d"))
						{
							goto IL_16C5;
						}
					}
					else if (num <= 387321904U)
					{
						if (num <= 266128159U)
						{
							if (num != 249350540U)
							{
								if (num != 266128159U)
								{
									goto IL_16C5;
								}
								if (!(text == "10-farewell/creature_c"))
								{
									goto IL_16C5;
								}
							}
							else if (!(text == "10-farewell/creature_b"))
							{
								goto IL_16C5;
							}
						}
						else if (num != 299683397U)
						{
							if (num != 387321904U)
							{
								goto IL_16C5;
							}
							if (!(text == "7-summit/cloud_cb"))
							{
								goto IL_16C5;
							}
							goto IL_14D5;
						}
						else if (!(text == "10-farewell/creature_a"))
						{
							goto IL_16C5;
						}
					}
					else if (num <= 403364048U)
					{
						if (num != 393960377U)
						{
							if (num != 403364048U)
							{
								goto IL_16C5;
							}
							if (!(text == "7-summit/cloud_dc"))
							{
								goto IL_16C5;
							}
							goto IL_14D5;
						}
						else
						{
							if (!(text == "10-farewell/temple"))
							{
								goto IL_16C5;
							}
							goto IL_1688;
						}
					}
					else if (num != 404099523U)
					{
						if (num != 420141667U)
						{
							goto IL_16C5;
						}
						if (!(text == "7-summit/cloud_db"))
						{
							goto IL_16C5;
						}
						goto IL_14D5;
					}
					else
					{
						if (!(text == "7-summit/cloud_cc"))
						{
							goto IL_16C5;
						}
						goto IL_14D5;
					}
					base.Depth = 10001;
					this.MakeParallax(-0.1f);
					this.MakeFloaty();
					goto IL_16C5;
				}
				if (num <= 683907267U)
				{
					if (num <= 487987618U)
					{
						if (num <= 430240726U)
						{
							if (num != 422430762U)
							{
								if (num != 430240726U)
								{
									goto IL_16C5;
								}
								if (!(text == "3-resort/roofcenter"))
								{
									goto IL_16C5;
								}
							}
							else
							{
								if (!(text == "3-resort/vent"))
								{
									goto IL_16C5;
								}
								this.CreateSmoke(Vector2.Zero, false);
								goto IL_16C5;
							}
						}
						else if (num != 431698817U)
						{
							if (num != 487987618U)
							{
								goto IL_16C5;
							}
							if (!(text == "7-summit/cloud_cd"))
							{
								goto IL_16C5;
							}
							goto IL_14D5;
						}
						else
						{
							if (!(text == "10-farewell/giantcassete"))
							{
								goto IL_16C5;
							}
							goto IL_1688;
						}
					}
					else if (num <= 520807381U)
					{
						if (num != 504765237U)
						{
							if (num != 520807381U)
							{
								goto IL_16C5;
							}
							if (!(text == "7-summit/cloud_dd"))
							{
								goto IL_16C5;
							}
							goto IL_14D5;
						}
						else
						{
							if (!(text == "7-summit/cloud_ce"))
							{
								goto IL_16C5;
							}
							goto IL_14D5;
						}
					}
					else if (num != 667129648U)
					{
						if (num != 683907267U)
						{
							goto IL_16C5;
						}
						if (!(text == "10-farewell/clouds/cloud_d"))
						{
							goto IL_16C5;
						}
						goto IL_163C;
					}
					else
					{
						if (!(text == "10-farewell/clouds/cloud_e"))
						{
							goto IL_16C5;
						}
						goto IL_163C;
					}
				}
				else if (num <= 728119966U)
				{
					if (num <= 700684886U)
					{
						if (num != 692652390U)
						{
							if (num != 700684886U)
							{
								goto IL_16C5;
							}
							if (!(text == "10-farewell/clouds/cloud_g"))
							{
								goto IL_16C5;
							}
							goto IL_163C;
						}
						else
						{
							if (!(text == "10-farewell/car"))
							{
								goto IL_16C5;
							}
							goto IL_1688;
						}
					}
					else if (num != 717462505U)
					{
						if (num != 728119966U)
						{
							goto IL_16C5;
						}
						if (!(text == "10-farewell/coral_"))
						{
							goto IL_16C5;
						}
						goto IL_1631;
					}
					else
					{
						if (!(text == "10-farewell/clouds/cloud_f"))
						{
							goto IL_16C5;
						}
						goto IL_163C;
					}
				}
				else if (num <= 743971057U)
				{
					if (num != 734240124U)
					{
						if (num != 743971057U)
						{
							goto IL_16C5;
						}
						if (!(text == "3-resort/bridgecolumntop"))
						{
							goto IL_16C5;
						}
						this.MakeSolid(-8f, -8f, 16f, 8f, 8, true);
						this.MakeSolid(-5f, 0f, 10f, 8f, 8, true);
						goto IL_16C5;
					}
					else
					{
						if (!(text == "10-farewell/clouds/cloud_a"))
						{
							goto IL_16C5;
						}
						goto IL_163C;
					}
				}
				else if (num != 767795362U)
				{
					if (num != 784572981U)
					{
						goto IL_16C5;
					}
					if (!(text == "10-farewell/clouds/cloud_b"))
					{
						goto IL_16C5;
					}
					goto IL_163C;
				}
				else
				{
					if (!(text == "10-farewell/clouds/cloud_c"))
					{
						goto IL_16C5;
					}
					goto IL_163C;
				}
				IL_132D:
				this.MakeSolid(-8f, -4f, 16f, 8f, 14, true);
				goto IL_16C5;
				IL_1631:
				this.MakeScaredAnimation();
				goto IL_16C5;
			}
			if (num <= 3437839731U)
			{
				if (num <= 2732820485U)
				{
					if (num <= 2622308830U)
					{
						if (num <= 2418635813U)
						{
							if (num != 2385080575U)
							{
								if (num != 2401858194U)
								{
									if (num != 2418635813U)
									{
										goto IL_16C5;
									}
									if (!(text == "4-cliffside/flower_c"))
									{
										goto IL_16C5;
									}
									goto IL_1445;
								}
								else
								{
									if (!(text == "4-cliffside/flower_b"))
									{
										goto IL_16C5;
									}
									goto IL_1445;
								}
							}
							else
							{
								if (!(text == "4-cliffside/flower_a"))
								{
									goto IL_16C5;
								}
								goto IL_1445;
							}
						}
						else if (num <= 2512741571U)
						{
							if (num != 2463347682U)
							{
								if (num != 2512741571U)
								{
									goto IL_16C5;
								}
								if (!(text == "1-forsakencity/ragsb"))
								{
									goto IL_16C5;
								}
								goto IL_130B;
							}
							else
							{
								if (!(text == "10-farewell/finalflag"))
								{
									goto IL_16C5;
								}
								this.AnimationSpeed = 6f;
								base.Add(this.image = new Decal.FinalFlagDecalImage());
								goto IL_16C5;
							}
						}
						else if (num != 2550685928U)
						{
							if (num != 2622308830U)
							{
								goto IL_16C5;
							}
							if (!(text == "7-summit/summitflag"))
							{
								goto IL_16C5;
							}
							base.Add(new SoundSource("event:/env/local/07_summit/flag_flap"));
							goto IL_16C5;
						}
						else
						{
							if (!(text == "5-temple/statue_d"))
							{
								goto IL_16C5;
							}
							goto IL_1484;
						}
					}
					else if (num <= 2682816294U)
					{
						if (num <= 2649261056U)
						{
							if (num != 2624345110U)
							{
								if (num != 2649261056U)
								{
									goto IL_16C5;
								}
								if (!(text == "7-summit/cloud_e"))
								{
									goto IL_16C5;
								}
								goto IL_14D5;
							}
							else
							{
								if (!(text == "3-resort/brokenelevator"))
								{
									goto IL_16C5;
								}
								this.MakeSolid(-16f, -20f, 32f, 48f, 22, true);
								goto IL_16C5;
							}
						}
						else if (num != 2666038675U)
						{
							if (num != 2682816294U)
							{
								goto IL_16C5;
							}
							if (!(text == "7-summit/cloud_g"))
							{
								goto IL_16C5;
							}
							goto IL_14D5;
						}
						else
						{
							if (!(text == "7-summit/cloud_d"))
							{
								goto IL_16C5;
							}
							goto IL_14D5;
						}
					}
					else if (num <= 2700670080U)
					{
						if (num != 2699593913U)
						{
							if (num != 2700670080U)
							{
								goto IL_16C5;
							}
							if (!(text == "10-farewell/heart_a"))
							{
								goto IL_16C5;
							}
							goto IL_1688;
						}
						else
						{
							if (!(text == "7-summit/cloud_f"))
							{
								goto IL_16C5;
							}
							goto IL_14D5;
						}
					}
					else if (num != 2716371532U)
					{
						if (num != 2732820485U)
						{
							goto IL_16C5;
						}
						if (!(text == "10-farewell/cliffside"))
						{
							goto IL_16C5;
						}
						goto IL_1688;
					}
					else
					{
						if (!(text == "7-summit/cloud_a"))
						{
							goto IL_16C5;
						}
						goto IL_14D5;
					}
				}
				else if (num <= 2969259615U)
				{
					if (num <= 2808517080U)
					{
						if (num <= 2751002937U)
						{
							if (num != 2749926770U)
							{
								if (num != 2751002937U)
								{
									goto IL_16C5;
								}
								if (!(text == "10-farewell/heart_b"))
								{
									goto IL_16C5;
								}
								goto IL_1688;
							}
							else
							{
								if (!(text == "7-summit/cloud_c"))
								{
									goto IL_16C5;
								}
								goto IL_14D5;
							}
						}
						else if (num != 2766704389U)
						{
							if (num != 2808517080U)
							{
								goto IL_16C5;
							}
							if (!(text == "0-prologue/house"))
							{
								goto IL_16C5;
							}
							this.CreateSmoke(new Vector2(36f, -28f), true);
							goto IL_16C5;
						}
						else
						{
							if (!(text == "7-summit/cloud_b"))
							{
								goto IL_16C5;
							}
							goto IL_14D5;
						}
					}
					else if (num <= 2867370103U)
					{
						if (num != 2850592484U)
						{
							if (num != 2867370103U)
							{
								goto IL_16C5;
							}
							if (!(text == "7-summit/cloud_h"))
							{
								goto IL_16C5;
							}
							goto IL_14D5;
						}
						else
						{
							if (!(text == "7-summit/cloud_i"))
							{
								goto IL_16C5;
							}
							goto IL_14D5;
						}
					}
					else if (num != 2900925341U)
					{
						if (num != 2969259615U)
						{
							goto IL_16C5;
						}
						if (!(text == "10-farewell/bed"))
						{
							goto IL_16C5;
						}
						goto IL_1688;
					}
					else
					{
						if (!(text == "7-summit/cloud_j"))
						{
							goto IL_16C5;
						}
						goto IL_14D5;
					}
				}
				else if (num <= 3258214997U)
				{
					if (num <= 3157549283U)
					{
						if (num != 3140771664U)
						{
							if (num != 3157549283U)
							{
								goto IL_16C5;
							}
							if (!(text == "3-resort/roofedge_b"))
							{
								goto IL_16C5;
							}
						}
						else if (!(text == "3-resort/roofedge_c"))
						{
							goto IL_16C5;
						}
					}
					else if (num != 3209907782U)
					{
						if (num != 3258214997U)
						{
							goto IL_16C5;
						}
						if (!(text == "3-resort/roofedge_d"))
						{
							goto IL_16C5;
						}
					}
					else
					{
						if (!(text == "3-resort/bridgecolumn"))
						{
							goto IL_16C5;
						}
						this.MakeSolid(-5f, -8f, 10f, 16f, 8, true);
						goto IL_16C5;
					}
				}
				else if (num <= 3421062112U)
				{
					if (num != 3405123136U)
					{
						if (num != 3421062112U)
						{
							goto IL_16C5;
						}
						if (!(text == "10-farewell/clouds/cloud_dc"))
						{
							goto IL_16C5;
						}
						goto IL_163C;
					}
					else
					{
						if (!(text == "10-farewell/clouds/cloud_cb"))
						{
							goto IL_16C5;
						}
						goto IL_163C;
					}
				}
				else if (num != 3421900755U)
				{
					if (num != 3437839731U)
					{
						goto IL_16C5;
					}
					if (!(text == "10-farewell/clouds/cloud_db"))
					{
						goto IL_16C5;
					}
					goto IL_163C;
				}
				else
				{
					if (!(text == "10-farewell/clouds/cloud_cc"))
					{
						goto IL_16C5;
					}
					goto IL_163C;
				}
			}
			else if (num <= 3788710541U)
			{
				if (num > 3588556745U)
				{
					if (num <= 3671267208U)
					{
						if (num <= 3655667221U)
						{
							if (num != 3638889602U)
							{
								if (num != 3655667221U)
								{
									goto IL_16C5;
								}
								if (!(text == "5-temple/bg_mirror_shard_group_e"))
								{
									goto IL_16C5;
								}
								goto IL_1467;
							}
							else
							{
								if (!(text == "5-temple/bg_mirror_shard_group_d"))
								{
									goto IL_16C5;
								}
								goto IL_1467;
							}
						}
						else if (num != 3659755230U)
						{
							if (num != 3671267208U)
							{
								goto IL_16C5;
							}
							if (!(text == "generic/grass_d"))
							{
								goto IL_16C5;
							}
						}
						else
						{
							if (!(text == "3-resort/roofedge"))
							{
								goto IL_16C5;
							}
							goto IL_134F;
						}
					}
					else if (num <= 3755155303U)
					{
						if (num != 3716561381U)
						{
							if (num != 3755155303U)
							{
								goto IL_16C5;
							}
							if (!(text == "generic/grass_a"))
							{
								goto IL_16C5;
							}
						}
						else
						{
							if (!(text == "10-farewell/reflection"))
							{
								goto IL_16C5;
							}
							goto IL_1688;
						}
					}
					else if (num != 3771932922U)
					{
						if (num != 3788710541U)
						{
							goto IL_16C5;
						}
						if (!(text == "generic/grass_c"))
						{
							goto IL_16C5;
						}
					}
					else if (!(text == "generic/grass_b"))
					{
						goto IL_16C5;
					}
					this.MakeBanner(2f, 2f, 1, 0.05f, false, -2f, false);
					goto IL_16C5;
				}
				if (num <= 3538223888U)
				{
					if (num != 3505788850U)
					{
						if (num != 3522566469U)
						{
							if (num != 3538223888U)
							{
								goto IL_16C5;
							}
							if (!(text == "5-temple/bg_mirror_shard_group_b"))
							{
								goto IL_16C5;
							}
							goto IL_1467;
						}
						else
						{
							if (!(text == "10-farewell/clouds/cloud_ce"))
							{
								goto IL_16C5;
							}
							goto IL_163C;
						}
					}
					else
					{
						if (!(text == "10-farewell/clouds/cloud_cd"))
						{
							goto IL_16C5;
						}
						goto IL_163C;
					}
				}
				else if (num <= 3555001507U)
				{
					if (num != 3538505445U)
					{
						if (num != 3555001507U)
						{
							goto IL_16C5;
						}
						if (!(text == "5-temple/bg_mirror_shard_group_c"))
						{
							goto IL_16C5;
						}
						goto IL_1467;
					}
					else
					{
						if (!(text == "10-farewell/clouds/cloud_dd"))
						{
							goto IL_16C5;
						}
						goto IL_163C;
					}
				}
				else if (num != 3561787168U)
				{
					if (num != 3588556745U)
					{
						goto IL_16C5;
					}
					if (!(text == "5-temple/bg_mirror_shard_group_a"))
					{
						goto IL_16C5;
					}
					goto IL_1467;
				}
				else
				{
					if (!(text == "9-core/ball_a_ice"))
					{
						goto IL_16C5;
					}
					base.Add(this.image = new Decal.CoreSwapImage(GFX.Game["decals/9-core/ball_a"], this.textures[0]));
					goto IL_16C5;
				}
			}
			else if (num <= 3909120453U)
			{
				if (num <= 3825232358U)
				{
					if (num <= 3808454739U)
					{
						if (num != 3791677120U)
						{
							if (num != 3808454739U)
							{
								goto IL_16C5;
							}
							if (!(text == "5-temple/bg_mirror_shard_g"))
							{
								goto IL_16C5;
							}
							goto IL_1467;
						}
						else
						{
							if (!(text == "5-temple/bg_mirror_shard_f"))
							{
								goto IL_16C5;
							}
							goto IL_1467;
						}
					}
					else if (num != 3810250355U)
					{
						if (num != 3825232358U)
						{
							goto IL_16C5;
						}
						if (!(text == "5-temple/bg_mirror_shard_d"))
						{
							goto IL_16C5;
						}
						goto IL_1467;
					}
					else
					{
						if (!(text == "1-forsakencity/rags"))
						{
							goto IL_16C5;
						}
						goto IL_130B;
					}
				}
				else if (num <= 3858787596U)
				{
					if (num != 3842009977U)
					{
						if (num != 3858787596U)
						{
							goto IL_16C5;
						}
						if (!(text == "5-temple/bg_mirror_shard_b"))
						{
							goto IL_16C5;
						}
						goto IL_1467;
					}
					else
					{
						if (!(text == "5-temple/bg_mirror_shard_e"))
						{
							goto IL_16C5;
						}
						goto IL_1467;
					}
				}
				else if (num != 3875565215U)
				{
					if (num != 3909120453U)
					{
						goto IL_16C5;
					}
					if (!(text == "5-temple/bg_mirror_shard_a"))
					{
						goto IL_16C5;
					}
					goto IL_1467;
				}
				else
				{
					if (!(text == "5-temple/bg_mirror_shard_c"))
					{
						goto IL_16C5;
					}
					goto IL_1467;
				}
			}
			else if (num <= 4026563786U)
			{
				if (num <= 4009264515U)
				{
					if (num != 3993008548U)
					{
						if (num != 4009264515U)
						{
							goto IL_16C5;
						}
						if (!(text == "10-farewell/clouds/cloud_bd"))
						{
							goto IL_16C5;
						}
						goto IL_163C;
					}
					else
					{
						if (!(text == "5-temple/bg_mirror_shard_j"))
						{
							goto IL_16C5;
						}
						goto IL_1467;
					}
				}
				else if (num != 4009786167U)
				{
					if (num != 4026563786U)
					{
						goto IL_16C5;
					}
					if (!(text == "5-temple/bg_mirror_shard_h"))
					{
						goto IL_16C5;
					}
					goto IL_1467;
				}
				else
				{
					if (!(text == "5-temple/bg_mirror_shard_k"))
					{
						goto IL_16C5;
					}
					goto IL_1467;
				}
			}
			else if (num <= 4093152610U)
			{
				if (num != 4043341405U)
				{
					if (num != 4093152610U)
					{
						goto IL_16C5;
					}
					if (!(text == "10-farewell/clouds/cloud_bc"))
					{
						goto IL_16C5;
					}
					goto IL_163C;
				}
				else
				{
					if (!(text == "5-temple/bg_mirror_shard_i"))
					{
						goto IL_16C5;
					}
					goto IL_1467;
				}
			}
			else if (num != 4109930229U)
			{
				if (num != 4212766131U)
				{
					goto IL_16C5;
				}
				if (!(text == "7-summit/cloud_bd"))
				{
					goto IL_16C5;
				}
				goto IL_14D5;
			}
			else
			{
				if (!(text == "10-farewell/clouds/cloud_bb"))
				{
					goto IL_16C5;
				}
				goto IL_163C;
			}
			IL_134F:
			this.MakeSolid((float)((this.scale.X < 0f) ? 0 : -8), -4f, 8f, 8f, 14, true);
			goto IL_16C5;
			IL_130B:
			this.MakeBanner(2f, 3.5f, 2, 0.05f, true, 0f, false);
			goto IL_16C5;
			IL_1445:
			this.MakeBanner(2f, 2f, 1, 0.05f, false, 2f, true);
			goto IL_16C5;
			IL_1467:
			this.scale.Y = 1f;
			this.MakeMirror(text, false);
			goto IL_16C5;
			IL_1484:
			this.MakeMirror(text, true);
			goto IL_16C5;
			IL_14D5:
			base.Depth = -13001;
			this.MakeParallax(0.1f);
			this.scale *= 1.15f;
			goto IL_16C5;
			IL_163C:
			base.Depth = -13001;
			this.MakeParallax(0.1f);
			this.scale *= 1.15f;
			goto IL_16C5;
			IL_1688:
			base.Depth = 10001;
			this.MakeParallax(-0.15f);
			this.MakeFloaty();
			IL_16C5:
			if (this.Name.Contains("crack"))
			{
				this.IsCrack = true;
			}
			if (this.image == null)
			{
				base.Add(this.image = new Decal.DecalImage());
			}
		}

		// Token: 0x060016E7 RID: 5863 RVA: 0x0008A6C0 File Offset: 0x000888C0
		private void MakeBanner(float speed, float amplitude, int sliceSize, float sliceSinIncrement, bool easeDown, float offset = 0f, bool onlyIfWindy = false)
		{
			Decal.Banner banner = new Decal.Banner
			{
				WaveSpeed = speed,
				WaveAmplitude = amplitude,
				SliceSize = sliceSize,
				SliceSinIncrement = sliceSinIncrement,
				Segments = new List<List<MTexture>>(),
				EaseDown = easeDown,
				Offset = offset,
				OnlyIfWindy = onlyIfWindy
			};
			foreach (MTexture mtexture in this.textures)
			{
				List<MTexture> list = new List<MTexture>();
				for (int i = 0; i < mtexture.Height; i += sliceSize)
				{
					list.Add(mtexture.GetSubtexture(0, i, mtexture.Width, sliceSize, null));
				}
				banner.Segments.Add(list);
			}
			base.Add(this.image = banner);
		}

		// Token: 0x060016E8 RID: 5864 RVA: 0x0008A7A4 File Offset: 0x000889A4
		private void MakeFloaty()
		{
			base.Add(this.wave = new SineWave(Calc.Random.Range(0.1f, 0.4f), Calc.Random.NextFloat() * 6.2831855f));
		}

		// Token: 0x060016E9 RID: 5865 RVA: 0x0008A7EC File Offset: 0x000889EC
		private void MakeSolid(float x, float y, float w, float h, int surfaceSoundIndex, bool blockWaterfalls = true)
		{
			Solid solid = new Solid(this.Position + new Vector2(x, y), w, h, true);
			solid.BlockWaterfalls = blockWaterfalls;
			solid.SurfaceSoundIndex = surfaceSoundIndex;
			base.Scene.Add(solid);
		}

		// Token: 0x060016EA RID: 5866 RVA: 0x0008A834 File Offset: 0x00088A34
		private void CreateSmoke(Vector2 offset, bool inbg)
		{
			Level level = base.Scene as Level;
			ParticleEmitter particleEmitter = new ParticleEmitter(inbg ? level.ParticlesBG : level.ParticlesFG, ParticleTypes.Chimney, offset, new Vector2(4f, 1f), -1.5707964f, 1, 0.2f);
			base.Add(particleEmitter);
			particleEmitter.SimulateCycle();
		}

		// Token: 0x060016EB RID: 5867 RVA: 0x0008A894 File Offset: 0x00088A94
		private void MakeMirror(string path, bool keepOffsetsClose)
		{
			base.Depth = 9500;
			if (keepOffsetsClose)
			{
				this.MakeMirror(path, this.GetMirrorOffset());
				return;
			}
			using (List<MTexture>.Enumerator enumerator = GFX.Game.GetAtlasSubtextures("mirrormasks/" + path).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Decal.<>c__DisplayClass24_0 CS$<>8__locals1 = new Decal.<>c__DisplayClass24_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.mask = enumerator.Current;
					MirrorSurface surface = new MirrorSurface(null);
					surface.ReflectionOffset = this.GetMirrorOffset();
					surface.OnRender = delegate()
					{
						CS$<>8__locals1.mask.DrawCentered(CS$<>8__locals1.<>4__this.Position, surface.ReflectionColor, CS$<>8__locals1.<>4__this.scale);
					};
					base.Add(surface);
				}
			}
		}

		// Token: 0x060016EC RID: 5868 RVA: 0x0008A96C File Offset: 0x00088B6C
		private void MakeMirror(string path, Vector2 offset)
		{
			base.Depth = 9500;
			using (List<MTexture>.Enumerator enumerator = GFX.Game.GetAtlasSubtextures("mirrormasks/" + path).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Decal.<>c__DisplayClass25_0 CS$<>8__locals1 = new Decal.<>c__DisplayClass25_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.mask = enumerator.Current;
					MirrorSurface surface = new MirrorSurface(null);
					surface.ReflectionOffset = offset + new Vector2(-2f + Calc.Random.NextFloat(4f), -2f + Calc.Random.NextFloat(4f));
					surface.OnRender = delegate()
					{
						CS$<>8__locals1.mask.DrawCentered(CS$<>8__locals1.<>4__this.Position, surface.ReflectionColor, CS$<>8__locals1.<>4__this.scale);
					};
					base.Add(surface);
				}
			}
		}

		// Token: 0x060016ED RID: 5869 RVA: 0x0008AA68 File Offset: 0x00088C68
		private void MakeMirrorSpecialCase(string path, Vector2 offset)
		{
			base.Depth = 9500;
			List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("mirrormasks/" + path);
			for (int i = 0; i < atlasSubtextures.Count; i++)
			{
				Vector2 value = new Vector2(-2f + Calc.Random.NextFloat(4f), -2f + Calc.Random.NextFloat(4f));
				if (i == 2)
				{
					value = new Vector2(4f, 2f);
				}
				else if (i == 6)
				{
					value = new Vector2(-2f, 0f);
				}
				MTexture mask = atlasSubtextures[i];
				MirrorSurface surface = new MirrorSurface(null);
				surface.ReflectionOffset = offset + value;
				surface.OnRender = delegate()
				{
					mask.DrawCentered(this.Position, surface.ReflectionColor, this.scale);
				};
				base.Add(surface);
			}
		}

		// Token: 0x060016EE RID: 5870 RVA: 0x0008AB65 File Offset: 0x00088D65
		private Vector2 GetMirrorOffset()
		{
			return new Vector2((float)(Calc.Random.Range(5, 14) * Calc.Random.Choose(1, -1)), (float)(Calc.Random.Range(2, 6) * Calc.Random.Choose(1, -1)));
		}

		// Token: 0x060016EF RID: 5871 RVA: 0x0008ABA1 File Offset: 0x00088DA1
		private void MakeParallax(float amount)
		{
			this.parallax = true;
			this.parallaxAmount = amount;
		}

		// Token: 0x060016F0 RID: 5872 RVA: 0x0008ABB4 File Offset: 0x00088DB4
		private void MakeScaredAnimation()
		{
			Sprite sprite = new Sprite(null, null);
			this.image = sprite;
			sprite.AddLoop("hidden", 0.1f, new MTexture[]
			{
				this.textures[0]
			});
			sprite.Add("return", 0.1f, "idle", new MTexture[]
			{
				this.textures[1]
			});
			sprite.AddLoop("idle", 0.1f, new MTexture[]
			{
				this.textures[2],
				this.textures[3],
				this.textures[4],
				this.textures[5],
				this.textures[6],
				this.textures[7]
			});
			sprite.Add("hide", 0.1f, "hidden", new MTexture[]
			{
				this.textures[8],
				this.textures[9],
				this.textures[10],
				this.textures[11],
				this.textures[12]
			});
			sprite.Play("idle", true, false);
			sprite.Scale = this.scale;
			sprite.CenterOrigin();
			base.Add(sprite);
			this.scaredAnimal = true;
		}

		// Token: 0x060016F1 RID: 5873 RVA: 0x0008AD28 File Offset: 0x00088F28
		public override void Update()
		{
			if (this.animated && this.textures.Count > 1)
			{
				this.frame += this.AnimationSpeed * Engine.DeltaTime;
				this.frame %= (float)this.textures.Count;
			}
			if (this.scaredAnimal)
			{
				Sprite sprite = this.image as Sprite;
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					if (sprite.CurrentAnimationID == "idle" && (entity.Position - this.Position).Length() < 32f)
					{
						sprite.Play("hide", false, false);
					}
					else if (sprite.CurrentAnimationID == "hidden" && (entity.Position - this.Position).Length() > 48f)
					{
						sprite.Play("return", false, false);
					}
				}
			}
			base.Update();
		}

		// Token: 0x060016F2 RID: 5874 RVA: 0x0008AE34 File Offset: 0x00089034
		public override void Render()
		{
			Vector2 position = this.Position;
			if (this.parallax)
			{
				Vector2 value = (base.Scene as Level).Camera.Position + new Vector2(160f, 90f);
				Vector2 value2 = (this.Position - value) * this.parallaxAmount;
				this.Position += value2;
			}
			if (this.wave != null)
			{
				this.Position.Y = this.Position.Y + this.wave.Value * 4f;
			}
			base.Render();
			this.Position = position;
		}

		// Token: 0x060016F3 RID: 5875 RVA: 0x0008AEDC File Offset: 0x000890DC
		public void FinalFlagTrigger()
		{
			Wiggler component = Wiggler.Create(1f, 4f, delegate(float v)
			{
				(this.image as Decal.FinalFlagDecalImage).Rotation = 0.20943952f * v;
			}, true, false);
			Vector2 position = this.Position;
			position.X = Calc.Snap(position.X, 8f) - 8f;
			position.Y += 6f;
			base.Scene.Add(new SummitCheckpoint.ConfettiRenderer(position));
			base.Add(component);
		}

		// Token: 0x04001374 RID: 4980
		public const string Root = "decals";

		// Token: 0x04001375 RID: 4981
		public const string MirrorMaskRoot = "mirrormasks";

		// Token: 0x04001376 RID: 4982
		public string Name;

		// Token: 0x04001377 RID: 4983
		public float AnimationSpeed = 12f;

		// Token: 0x04001378 RID: 4984
		private Component image;

		// Token: 0x04001379 RID: 4985
		public bool IsCrack;

		// Token: 0x0400137A RID: 4986
		private List<MTexture> textures;

		// Token: 0x0400137B RID: 4987
		private Vector2 scale;

		// Token: 0x0400137C RID: 4988
		private float frame;

		// Token: 0x0400137D RID: 4989
		private bool animated = true;

		// Token: 0x0400137E RID: 4990
		private bool parallax;

		// Token: 0x0400137F RID: 4991
		private float parallaxAmount;

		// Token: 0x04001380 RID: 4992
		private bool scaredAnimal;

		// Token: 0x04001381 RID: 4993
		private SineWave wave;

		// Token: 0x02000691 RID: 1681
		private class Banner : Component
		{
			// Token: 0x1700061D RID: 1565
			// (get) Token: 0x06002BFE RID: 11262 RVA: 0x00118CA7 File Offset: 0x00116EA7
			public Decal Decal
			{
				get
				{
					return (Decal)base.Entity;
				}
			}

			// Token: 0x06002BFF RID: 11263 RVA: 0x00118CB4 File Offset: 0x00116EB4
			public Banner() : base(true, true)
			{
			}

			// Token: 0x06002C00 RID: 11264 RVA: 0x00118CDC File Offset: 0x00116EDC
			public override void Update()
			{
				if (this.OnlyIfWindy)
				{
					float x = (base.Scene as Level).Wind.X;
					float target = Math.Min(3f, Math.Abs(x) * 0.004f);
					this.WindMultiplier = Calc.Approach(this.WindMultiplier, target, Engine.DeltaTime * 4f);
					if (x != 0f)
					{
						this.Offset = (float)Math.Sign(x) * Math.Abs(this.Offset);
					}
				}
				this.sineTimer += Engine.DeltaTime * this.WindMultiplier;
				base.Update();
			}

			// Token: 0x06002C01 RID: 11265 RVA: 0x00118D7C File Offset: 0x00116F7C
			public override void Render()
			{
				MTexture mtexture = this.Decal.textures[(int)this.Decal.frame];
				List<MTexture> list = this.Segments[(int)this.Decal.frame];
				for (int i = 0; i < list.Count; i++)
				{
					float num = (this.EaseDown ? ((float)i / (float)list.Count) : (1f - (float)i / (float)list.Count)) * this.WindMultiplier;
					float x = (float)(Math.Sin((double)(this.sineTimer * this.WaveSpeed + (float)i * this.SliceSinIncrement)) * (double)num * (double)this.WaveAmplitude + (double)(num * this.Offset));
					list[i].Draw(this.Decal.Position + new Vector2(x, 0f), new Vector2((float)(mtexture.Width / 2), (float)(mtexture.Height / 2 - i * this.SliceSize)), Color.White, this.Decal.scale);
				}
			}

			// Token: 0x04002B38 RID: 11064
			public float WaveSpeed;

			// Token: 0x04002B39 RID: 11065
			public float WaveAmplitude;

			// Token: 0x04002B3A RID: 11066
			public int SliceSize;

			// Token: 0x04002B3B RID: 11067
			public float SliceSinIncrement;

			// Token: 0x04002B3C RID: 11068
			public bool EaseDown;

			// Token: 0x04002B3D RID: 11069
			public float Offset;

			// Token: 0x04002B3E RID: 11070
			public bool OnlyIfWindy;

			// Token: 0x04002B3F RID: 11071
			public float WindMultiplier = 1f;

			// Token: 0x04002B40 RID: 11072
			private float sineTimer = Calc.Random.NextFloat();

			// Token: 0x04002B41 RID: 11073
			public List<List<MTexture>> Segments;
		}

		// Token: 0x02000692 RID: 1682
		private class DecalImage : Component
		{
			// Token: 0x1700061E RID: 1566
			// (get) Token: 0x06002C02 RID: 11266 RVA: 0x00118CA7 File Offset: 0x00116EA7
			public Decal Decal
			{
				get
				{
					return (Decal)base.Entity;
				}
			}

			// Token: 0x06002C03 RID: 11267 RVA: 0x00118E8E File Offset: 0x0011708E
			public DecalImage() : base(true, true)
			{
			}

			// Token: 0x06002C04 RID: 11268 RVA: 0x00118E98 File Offset: 0x00117098
			public override void Render()
			{
				this.Decal.textures[(int)this.Decal.frame].DrawCentered(this.Decal.Position, Color.White, this.Decal.scale);
			}
		}

		// Token: 0x02000693 RID: 1683
		private class FinalFlagDecalImage : Component
		{
			// Token: 0x1700061F RID: 1567
			// (get) Token: 0x06002C05 RID: 11269 RVA: 0x00118CA7 File Offset: 0x00116EA7
			public Decal Decal
			{
				get
				{
					return (Decal)base.Entity;
				}
			}

			// Token: 0x06002C06 RID: 11270 RVA: 0x00118E8E File Offset: 0x0011708E
			public FinalFlagDecalImage() : base(true, true)
			{
			}

			// Token: 0x06002C07 RID: 11271 RVA: 0x00118ED8 File Offset: 0x001170D8
			public override void Render()
			{
				MTexture mtexture = this.Decal.textures[(int)this.Decal.frame];
				mtexture.DrawJustified(this.Decal.Position + Vector2.UnitY * (float)(mtexture.Height / 2), new Vector2(0.5f, 1f), Color.White, this.Decal.scale, this.Rotation);
			}

			// Token: 0x04002B42 RID: 11074
			public float Rotation;
		}

		// Token: 0x02000694 RID: 1684
		private class CoreSwapImage : Component
		{
			// Token: 0x17000620 RID: 1568
			// (get) Token: 0x06002C08 RID: 11272 RVA: 0x00118CA7 File Offset: 0x00116EA7
			public Decal Decal
			{
				get
				{
					return (Decal)base.Entity;
				}
			}

			// Token: 0x06002C09 RID: 11273 RVA: 0x00118F50 File Offset: 0x00117150
			public CoreSwapImage(MTexture hot, MTexture cold) : base(false, true)
			{
				this.hot = hot;
				this.cold = cold;
			}

			// Token: 0x06002C0A RID: 11274 RVA: 0x00118F68 File Offset: 0x00117168
			public override void Render()
			{
				(((base.Scene as Level).CoreMode == Session.CoreModes.Cold) ? this.cold : this.hot).DrawCentered(this.Decal.Position, Color.White, this.Decal.scale);
			}

			// Token: 0x04002B43 RID: 11075
			private MTexture hot;

			// Token: 0x04002B44 RID: 11076
			private MTexture cold;
		}
	}
}
