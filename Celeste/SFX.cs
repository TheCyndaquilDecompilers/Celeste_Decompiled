using System;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste
{
	// Token: 0x02000386 RID: 902
	public static class SFX
	{
		// Token: 0x06001D68 RID: 7528 RVA: 0x000CCBE0 File Offset: 0x000CADE0
		public static void Initialize()
		{
			foreach (FieldInfo fieldInfo in typeof(SFX).GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				if (fieldInfo.FieldType == typeof(string))
				{
					string text = fieldInfo.GetValue(null).ToString();
					SFX.byHandle.Add(fieldInfo.Name, text);
					if (text.StartsWith("event:/char/madeline/"))
					{
						SFX.MadelineToBadelineSound.Add(text, text.Replace("madeline", "badeline"));
					}
				}
			}
			SFX.MadelineToBadelineSound.Add("event:/game/general/assist_screenbottom", "event:/game/general/assist_screenbottom");
		}

		// Token: 0x06001D69 RID: 7529 RVA: 0x000CCC84 File Offset: 0x000CAE84
		public static string EventnameByHandle(string handle)
		{
			string result = "";
			SFX.byHandle.TryGetValue(handle, out result);
			return result;
		}

		// Token: 0x04001B07 RID: 6919
		public const string NONE = "null";

		// Token: 0x04001B08 RID: 6920
		public const string music_levelselect = "event:/music/menu/level_select";

		// Token: 0x04001B09 RID: 6921
		public const string music_credits = "event:/music/menu/credits";

		// Token: 0x04001B0A RID: 6922
		public const string music_complete_area = "event:/music/menu/complete_area";

		// Token: 0x04001B0B RID: 6923
		public const string music_complete_summit = "event:/music/menu/complete_summit";

		// Token: 0x04001B0C RID: 6924
		public const string music_complete_bside = "event:/music/menu/complete_bside";

		// Token: 0x04001B0D RID: 6925
		public const string music_prologue_intro_vignette = "event:/game/00_prologue/intro_vignette";

		// Token: 0x04001B0E RID: 6926
		public const string music_prologue_beginning = "event:/music/lvl0/intro";

		// Token: 0x04001B0F RID: 6927
		public const string music_prologue_collapse = "event:/music/lvl0/bridge";

		// Token: 0x04001B10 RID: 6928
		public const string music_prologue_title_ping = "event:/music/lvl0/title_ping";

		// Token: 0x04001B11 RID: 6929
		public const string music_city = "event:/music/lvl1/main";

		// Token: 0x04001B12 RID: 6930
		public const string music_city_theo = "event:/music/lvl1/theo";

		// Token: 0x04001B13 RID: 6931
		public const string music_oldsite_beginning = "event:/music/lvl2/beginning";

		// Token: 0x04001B14 RID: 6932
		public const string music_oldsite_mirror = "event:/music/lvl2/mirror";

		// Token: 0x04001B15 RID: 6933
		public const string music_oldsite_dreamblock_sting_pt1 = "event:/music/lvl2/dreamblock_sting_pt1";

		// Token: 0x04001B16 RID: 6934
		public const string music_oldsite_dreamblock_sting_pt2 = "event:/music/lvl2/dreamblock_sting_pt2";

		// Token: 0x04001B17 RID: 6935
		public const string music_oldsite_evil_maddy = "event:/music/lvl2/evil_madeline";

		// Token: 0x04001B18 RID: 6936
		public const string music_oldsite_chase = "event:/music/lvl2/chase";

		// Token: 0x04001B19 RID: 6937
		public const string music_oldsite_payphone_loop = "event:/music/lvl2/phone_loop";

		// Token: 0x04001B1A RID: 6938
		public const string music_oldsite_payphone_end = "event:/music/lvl2/phone_end";

		// Token: 0x04001B1B RID: 6939
		public const string music_oldsite_awake = "event:/music/lvl2/awake";

		// Token: 0x04001B1C RID: 6940
		public const string music_resort_intro = "event:/music/lvl3/intro";

		// Token: 0x04001B1D RID: 6941
		public const string music_resort_explore = "event:/music/lvl3/explore";

		// Token: 0x04001B1E RID: 6942
		public const string music_resort_clean = "event:/music/lvl3/clean";

		// Token: 0x04001B1F RID: 6943
		public const string music_resort_clean_extended = "event:/music/lvl3/clean_extended";

		// Token: 0x04001B20 RID: 6944
		public const string music_resort_oshiro_theme = "event:/music/lvl3/oshiro_theme";

		// Token: 0x04001B21 RID: 6945
		public const string music_resort_oshiro_chase = "event:/music/lvl3/oshiro_chase";

		// Token: 0x04001B22 RID: 6946
		public const string music_cliffside_main = "event:/music/lvl4/main";

		// Token: 0x04001B23 RID: 6947
		public const string music_cliffside_heavywinds = "event:/music/lvl4/heavy_winds";

		// Token: 0x04001B24 RID: 6948
		public const string music_cliffside_panicattack = "event:/music/lvl4/minigame";

		// Token: 0x04001B25 RID: 6949
		public const string music_temple_normal = "event:/music/lvl5/normal";

		// Token: 0x04001B26 RID: 6950
		public const string music_temple_middle = "event:/music/lvl5/middle_temple";

		// Token: 0x04001B27 RID: 6951
		public const string music_temple_mirror = "event:/music/lvl5/mirror";

		// Token: 0x04001B28 RID: 6952
		public const string music_temple_mirrorcutscene = "event:/music/lvl5/mirror_cutscene";

		// Token: 0x04001B29 RID: 6953
		public const string music_reflection_maddietheo = "event:/music/lvl6/madeline_and_theo";

		// Token: 0x04001B2A RID: 6954
		public const string music_reflection_starjump = "event:/music/lvl6/starjump";

		// Token: 0x04001B2B RID: 6955
		public const string music_reflection_fall = "event:/music/lvl6/the_fall";

		// Token: 0x04001B2C RID: 6956
		public const string music_reflection_fight = "event:/music/lvl6/badeline_fight";

		// Token: 0x04001B2D RID: 6957
		public const string music_reflection_fight_glitch = "event:/music/lvl6/badeline_glitch";

		// Token: 0x04001B2E RID: 6958
		public const string music_reflection_fight_finish = "event:/music/lvl6/badeline_acoustic";

		// Token: 0x04001B2F RID: 6959
		public const string music_reflection_main = "event:/music/lvl6/main";

		// Token: 0x04001B30 RID: 6960
		public const string music_reflection_secretroom = "event:/music/lvl6/secret_room";

		// Token: 0x04001B31 RID: 6961
		public const string music_summit_main = "event:/music/lvl7/main";

		// Token: 0x04001B32 RID: 6962
		public const string music_summit_finalascent = "event:/music/lvl7/final_ascent";

		// Token: 0x04001B33 RID: 6963
		public const string music_epilogue_main = "event:/music/lvl8/main";

		// Token: 0x04001B34 RID: 6964
		public const string music_core_main = "event:/music/lvl9/main";

		// Token: 0x04001B35 RID: 6965
		public const string music_farewell_part01 = "event:/new_content/music/lvl10/part01";

		// Token: 0x04001B36 RID: 6966
		public const string music_farewell_part02 = "event:/new_content/music/lvl10/part02";

		// Token: 0x04001B37 RID: 6967
		public const string music_farewell_part03 = "event:/new_content/music/lvl10/part03";

		// Token: 0x04001B38 RID: 6968
		public const string music_farewell_intermission_heartgroove = "event:/new_content/music/lvl10/intermission_heartgroove";

		// Token: 0x04001B39 RID: 6969
		public const string music_farewell_intermission_powerpoint = "event:/new_content/music/lvl10/intermission_powerpoint";

		// Token: 0x04001B3A RID: 6970
		public const string music_farewell_reconciliation = "event:/new_content/music/lvl10/reconciliation";

		// Token: 0x04001B3B RID: 6971
		public const string music_farewell_cassette = "event:/new_content/music/lvl10/cassette_rooms";

		// Token: 0x04001B3C RID: 6972
		public const string music_farewell_final_run = "event:/new_content/music/lvl10/final_run";

		// Token: 0x04001B3D RID: 6973
		public const string music_farewell_end_cinematic = "event:/new_content/music/lvl10/cinematic/end";

		// Token: 0x04001B3E RID: 6974
		public const string music_farewell_end_cinematic_intro = "event:/new_content/music/lvl10/cinematic/end_intro";

		// Token: 0x04001B3F RID: 6975
		public const string music_farewell_firstbirdcrash_cinematic = "event:/new_content/music/lvl10/cinematic/bird_crash_first";

		// Token: 0x04001B40 RID: 6976
		public const string music_farewell_secondbirdcrash_cinematic = "event:/new_content/music/lvl10/cinematic/bird_crash_second";

		// Token: 0x04001B41 RID: 6977
		public const string music_farewell_granny = "event:/new_content/music/lvl10/granny_farewell";

		// Token: 0x04001B42 RID: 6978
		public const string music_farewell_golden_room = "event:/new_content/music/lvl10/golden_room";

		// Token: 0x04001B43 RID: 6979
		public const string music_pico8_title = "event:/classic/pico8_mus_00";

		// Token: 0x04001B44 RID: 6980
		public const string music_pico8_area1 = "event:/classic/pico8_mus_01";

		// Token: 0x04001B45 RID: 6981
		public const string music_pico8_area2 = "event:/classic/pico8_mus_02";

		// Token: 0x04001B46 RID: 6982
		public const string music_pico8_area3 = "event:/classic/pico8_mus_03";

		// Token: 0x04001B47 RID: 6983
		public const string music_pico8_wind = "event:/classic/sfx61";

		// Token: 0x04001B48 RID: 6984
		public const string music_pico8_end = "event:/classic/sfx62";

		// Token: 0x04001B49 RID: 6985
		public const string music_pico8_boot = "event:/classic/pico8_boot";

		// Token: 0x04001B4A RID: 6986
		public const string music_rmx_01_forsaken_city = "event:/music/remix/01_forsaken_city";

		// Token: 0x04001B4B RID: 6987
		public const string music_rmx_02_old_site = "event:/music/remix/02_old_site";

		// Token: 0x04001B4C RID: 6988
		public const string music_rmx_03_resort = "event:/music/remix/03_resort";

		// Token: 0x04001B4D RID: 6989
		public const string music_rmx_04_cliffside = "event:/music/remix/04_cliffside";

		// Token: 0x04001B4E RID: 6990
		public const string music_rmx_05_mirror_temple = "event:/music/remix/05_mirror_temple";

		// Token: 0x04001B4F RID: 6991
		public const string music_rmx_06_reflection = "event:/music/remix/06_reflection";

		// Token: 0x04001B50 RID: 6992
		public const string music_rmx_07_summit = "event:/music/remix/07_summit";

		// Token: 0x04001B51 RID: 6993
		public const string music_rmx_09_core = "event:/music/remix/09_core";

		// Token: 0x04001B52 RID: 6994
		public const string cas_01_forsaken_city = "event:/music/cassette/01_forsaken_city";

		// Token: 0x04001B53 RID: 6995
		public const string cas_02_old_site = "event:/music/cassette/02_old_site";

		// Token: 0x04001B54 RID: 6996
		public const string cas_03_resort = "event:/music/cassette/03_resort";

		// Token: 0x04001B55 RID: 6997
		public const string cas_04_cliffside = "event:/music/cassette/04_cliffside";

		// Token: 0x04001B56 RID: 6998
		public const string cas_05_mirror_temple = "event:/music/cassette/05_mirror_temple";

		// Token: 0x04001B57 RID: 6999
		public const string cas_06_reflection = "event:/music/cassette/06_reflection";

		// Token: 0x04001B58 RID: 7000
		public const string cas_07_summit = "event:/music/cassette/07_summit";

		// Token: 0x04001B59 RID: 7001
		public const string cas_08_core = "event:/music/cassette/09_core";

		// Token: 0x04001B5A RID: 7002
		public const string dialog_prefix = "event:/char/dialogue/";

		// Token: 0x04001B5B RID: 7003
		public const string dialog_phone_static_ex = "event:/char/dialogue/sfx_support/phone_static_ex";

		// Token: 0x04001B5C RID: 7004
		public const string dialog_phone_static_mom = "event:/char/dialogue/sfx_support/phone_static_mom";

		// Token: 0x04001B5D RID: 7005
		public const string char_mad_footstep = "event:/char/madeline/footstep";

		// Token: 0x04001B5E RID: 7006
		public const string char_mad_land = "event:/char/madeline/landing";

		// Token: 0x04001B5F RID: 7007
		public const string char_mad_jump = "event:/char/madeline/jump";

		// Token: 0x04001B60 RID: 7008
		public const string char_mad_jump_assisted = "event:/char/madeline/jump_assisted";

		// Token: 0x04001B61 RID: 7009
		public const string char_mad_jump_wall_right = "event:/char/madeline/jump_wall_right";

		// Token: 0x04001B62 RID: 7010
		public const string char_mad_jump_wall_left = "event:/char/madeline/jump_wall_left";

		// Token: 0x04001B63 RID: 7011
		public const string char_mad_jump_climb_left = "event:/char/madeline/jump_climb_left";

		// Token: 0x04001B64 RID: 7012
		public const string char_mad_jump_climb_right = "event:/char/madeline/jump_climb_right";

		// Token: 0x04001B65 RID: 7013
		public const string char_mad_jump_super = "event:/char/madeline/jump_super";

		// Token: 0x04001B66 RID: 7014
		public const string char_mad_jump_superwall = "event:/char/madeline/jump_superwall";

		// Token: 0x04001B67 RID: 7015
		public const string char_mad_jump_superslide = "event:/char/madeline/jump_superslide";

		// Token: 0x04001B68 RID: 7016
		public const string char_mad_jump_special = "event:/char/madeline/jump_special";

		// Token: 0x04001B69 RID: 7017
		public const string char_mad_jump_dreamblock = "event:/char/madeline/jump_dreamblock";

		// Token: 0x04001B6A RID: 7018
		public const string char_mad_grab = "event:/char/madeline/grab";

		// Token: 0x04001B6B RID: 7019
		public const string char_mad_grab_letgo = "event:/char/madeline/grab_letgo";

		// Token: 0x04001B6C RID: 7020
		public const string char_mad_handhold = "event:/char/madeline/handhold";

		// Token: 0x04001B6D RID: 7021
		public const string char_mad_wallslide = "event:/char/madeline/wallslide";

		// Token: 0x04001B6E RID: 7022
		public const string char_mad_duck = "event:/char/madeline/duck";

		// Token: 0x04001B6F RID: 7023
		public const string char_mad_stand = "event:/char/madeline/stand";

		// Token: 0x04001B70 RID: 7024
		public const string char_mad_climb_loop = "event:/char/madeline/climb_loop";

		// Token: 0x04001B71 RID: 7025
		public const string char_mad_climb_ledge = "event:/char/madeline/climb_ledge";

		// Token: 0x04001B72 RID: 7026
		public const string char_mad_dash_red_left = "event:/char/madeline/dash_red_left";

		// Token: 0x04001B73 RID: 7027
		public const string char_mad_dash_red_right = "event:/char/madeline/dash_red_right";

		// Token: 0x04001B74 RID: 7028
		public const string char_mad_dash_pink_left = "event:/char/madeline/dash_pink_left";

		// Token: 0x04001B75 RID: 7029
		public const string char_mad_dash_pink_right = "event:/char/madeline/dash_pink_right";

		// Token: 0x04001B76 RID: 7030
		public const string char_mad_core_haircharged = "event:/char/madeline/core_hair_charged";

		// Token: 0x04001B77 RID: 7031
		public const string char_mad_predeath = "event:/char/madeline/predeath";

		// Token: 0x04001B78 RID: 7032
		public const string char_mad_death = "event:/char/madeline/death";

		// Token: 0x04001B79 RID: 7033
		public const string char_mad_death_golden = "event:/new_content/char/madeline/death_golden";

		// Token: 0x04001B7A RID: 7034
		public const string char_mad_revive = "event:/char/madeline/revive";

		// Token: 0x04001B7B RID: 7035
		public const string char_mad_campfire_sit = "event:/char/madeline/campfire_sit";

		// Token: 0x04001B7C RID: 7036
		public const string char_mad_campfire_stand = "event:/char/madeline/campfire_stand";

		// Token: 0x04001B7D RID: 7037
		public const string char_mad_dreamblock_enter = "event:/char/madeline/dreamblock_enter";

		// Token: 0x04001B7E RID: 7038
		public const string char_mad_dreamblock_travel = "event:/char/madeline/dreamblock_travel";

		// Token: 0x04001B7F RID: 7039
		public const string char_mad_dreamblock_exit = "event:/char/madeline/dreamblock_exit";

		// Token: 0x04001B80 RID: 7040
		public const string char_mad_mirrortemple_landing = "event:/char/madeline/mirrortemple_big_landing";

		// Token: 0x04001B81 RID: 7041
		public const string char_mad_crystaltheo_lift = "event:/char/madeline/crystaltheo_lift";

		// Token: 0x04001B82 RID: 7042
		public const string char_mad_crystaltheo_throw = "event:/char/madeline/crystaltheo_throw";

		// Token: 0x04001B83 RID: 7043
		public const string char_mad_theo_collapse = "event:/char/madeline/theo_collapse";

		// Token: 0x04001B84 RID: 7044
		public const string char_mad_summit_flytonext = "event:/char/madeline/summit_flytonext";

		// Token: 0x04001B85 RID: 7045
		public const string char_mad_summit_areastart = "event:/char/madeline/summit_areastart";

		// Token: 0x04001B86 RID: 7046
		public const string char_mad_summit_sit = "event:/char/madeline/summit_sit";

		// Token: 0x04001B87 RID: 7047
		public const string char_mad_idle_scratch = "event:/char/madeline/idle_scratch";

		// Token: 0x04001B88 RID: 7048
		public const string char_mad_idle_sneeze = "event:/char/madeline/idle_sneeze";

		// Token: 0x04001B89 RID: 7049
		public const string char_mad_idle_crackknuckles = "event:/char/madeline/idle_crackknuckles";

		// Token: 0x04001B8A RID: 7050
		public const string char_mad_backpack_drop = "event:/char/madeline/backpack_drop";

		// Token: 0x04001B8B RID: 7051
		public const string char_mad_screenentry_lowgrav = "event:/new_content/char/madeline/screenentry_lowgrav";

		// Token: 0x04001B8C RID: 7052
		public const string char_mad_screenentry_stubborn = "event:/new_content/char/madeline/screenentry_stubborn";

		// Token: 0x04001B8D RID: 7053
		public const string char_mad_screenentry_golden = "event:/new_content/char/madeline/screenentry_golden";

		// Token: 0x04001B8E RID: 7054
		public const string char_mad_screenentry_gran = "event:/new_content/char/madeline/screenentry_gran";

		// Token: 0x04001B8F RID: 7055
		public const string char_mad_screenentry_gran_landing = "event:/new_content/char/madeline/screenentry_gran_landing";

		// Token: 0x04001B90 RID: 7056
		public const string char_mad_glider_drop = "event:/new_content/char/madeline/glider_drop";

		// Token: 0x04001B91 RID: 7057
		public const string char_mad_bounce_boost = "event:/new_content/char/madeline/bounce_boost";

		// Token: 0x04001B92 RID: 7058
		public const string char_mad_water_in = "event:/char/madeline/water_in";

		// Token: 0x04001B93 RID: 7059
		public const string char_mad_water_out = "event:/char/madeline/water_out";

		// Token: 0x04001B94 RID: 7060
		public const string char_mad_water_dash_in = "event:/char/madeline/water_dash_in";

		// Token: 0x04001B95 RID: 7061
		public const string char_mad_water_dash_out = "event:/char/madeline/water_dash_out";

		// Token: 0x04001B96 RID: 7062
		public const string char_mad_water_dash_gen = "event:/char/madeline/water_dash_gen";

		// Token: 0x04001B97 RID: 7063
		public const string char_mad_water_move_shallow = "event:/char/madeline/water_move_shallow";

		// Token: 0x04001B98 RID: 7064
		public const string char_mad_water_move_general = "event:/char/madeline/water_move_general";

		// Token: 0x04001B99 RID: 7065
		public const string char_mad_energy_out_loop = "event:/char/madeline/energy_out_loop";

		// Token: 0x04001B9A RID: 7066
		public const string char_mad_energy_recharged = "event:/char/madeline/energy_recharged";

		// Token: 0x04001B9B RID: 7067
		public const string char_mad_hiccup_standing = "event:/new_content/char/madeline/hiccup_standing";

		// Token: 0x04001B9C RID: 7068
		public const string char_mad_hiccup_ducking = "event:/new_content/char/madeline/hiccup_ducking";

		// Token: 0x04001B9D RID: 7069
		public const string char_theo_yolofist = "event:/char/theo/yolo_fist";

		// Token: 0x04001B9E RID: 7070
		public const string char_theo_phonetaps_loop = "event:/char/theo/phone_taps_loop";

		// Token: 0x04001B9F RID: 7071
		public const string char_theo_resort_vent_tug = "event:/char/theo/resort_vent_tug";

		// Token: 0x04001BA0 RID: 7072
		public const string char_theo_resort_vent_grab = "event:/char/theo/resort_vent_grab";

		// Token: 0x04001BA1 RID: 7073
		public const string char_theo_resort_vent_rip = "event:/char/theo/resort_vent_rip";

		// Token: 0x04001BA2 RID: 7074
		public const string char_theo_resort_vent_tumble = "event:/char/theo/resort_vent_tumble";

		// Token: 0x04001BA3 RID: 7075
		public const string char_theo_resort_standtocrawl = "event:/char/theo/resort_standtocrawl";

		// Token: 0x04001BA4 RID: 7076
		public const string char_theo_resort_crawl = "event:/char/theo/resort_crawl";

		// Token: 0x04001BA5 RID: 7077
		public const string char_theo_resort_ceilingvent_shake = "event:/char/theo/resort_ceilingvent_shake";

		// Token: 0x04001BA6 RID: 7078
		public const string char_theo_resort_ceilingvent_popoff = "event:/char/theo/resort_ceilingvent_popoff";

		// Token: 0x04001BA7 RID: 7079
		public const string char_theo_resort_ceilingvent_hey = "event:/char/theo/resort_ceilingvent_hey";

		// Token: 0x04001BA8 RID: 7080
		public const string char_theo_resort_ceilingvent_seeya = "event:/char/theo/resort_ceilingvent_seeya";

		// Token: 0x04001BA9 RID: 7081
		public const string char_bad_appear = "event:/char/badeline/appear";

		// Token: 0x04001BAA RID: 7082
		public const string char_bad_disappear = "event:/char/badeline/disappear";

		// Token: 0x04001BAB RID: 7083
		public const string char_bad_level_entry = "event:/char/badeline/level_entry";

		// Token: 0x04001BAC RID: 7084
		public const string char_bad_footstep = "event:/char/badeline/footstep";

		// Token: 0x04001BAD RID: 7085
		public const string char_bad_land = "event:/char/badeline/landing";

		// Token: 0x04001BAE RID: 7086
		public const string char_bad_grab = "event:/char/badeline/grab";

		// Token: 0x04001BAF RID: 7087
		public const string char_bad_handhold = "event:/char/badeline/handhold";

		// Token: 0x04001BB0 RID: 7088
		public const string char_bad_wallslide = "event:/char/badeline/wallslide";

		// Token: 0x04001BB1 RID: 7089
		public const string char_bad_grab_letgo = "event:/char/badeline/grab_letgo";

		// Token: 0x04001BB2 RID: 7090
		public const string char_bad_jump = "event:/char/badeline/jump";

		// Token: 0x04001BB3 RID: 7091
		public const string char_bad_jump_assisted = "event:/char/badeline/jump_assisted";

		// Token: 0x04001BB4 RID: 7092
		public const string char_bad_jump_wall_left = "event:/char/badeline/jump_wall_left";

		// Token: 0x04001BB5 RID: 7093
		public const string char_bad_jump_wall_right = "event:/char/badeline/jump_wall_right";

		// Token: 0x04001BB6 RID: 7094
		public const string char_bad_jump_climb_left = "event:/char/badeline/jump_climb_left";

		// Token: 0x04001BB7 RID: 7095
		public const string char_bad_jump_climb_right = "event:/char/badeline/jump_climb_right";

		// Token: 0x04001BB8 RID: 7096
		public const string char_bad_duck = "event:/char/badeline/duck";

		// Token: 0x04001BB9 RID: 7097
		public const string char_bad_stand = "event:/char/badeline/stand";

		// Token: 0x04001BBA RID: 7098
		public const string char_bad_climb_ledge = "event:/char/badeline/climb_ledge";

		// Token: 0x04001BBB RID: 7099
		public const string char_bad_dash_red_left = "event:/char/badeline/dash_red_left";

		// Token: 0x04001BBC RID: 7100
		public const string char_bad_dash_red_right = "event:/char/badeline/dash_red_right";

		// Token: 0x04001BBD RID: 7101
		public const string char_bad_jump_dreamblock = "event:/char/badeline/jump_dreamblock";

		// Token: 0x04001BBE RID: 7102
		public const string char_bad_dreamblock_enter = "event:/char/badeline/dreamblock_enter";

		// Token: 0x04001BBF RID: 7103
		public const string char_bad_dreamblock_travel = "event:/char/badeline/dreamblock_travel";

		// Token: 0x04001BC0 RID: 7104
		public const string char_bad_dreamblock_exit = "event:/char/badeline/dreamblock_exit";

		// Token: 0x04001BC1 RID: 7105
		public const string char_bad_jump_super = "event:/char/badeline/jump_super";

		// Token: 0x04001BC2 RID: 7106
		public const string char_bad_jump_superwall = "event:/char/badeline/jump_superwall";

		// Token: 0x04001BC3 RID: 7107
		public const string char_bad_jump_superslide = "event:/char/badeline/jump_superslide";

		// Token: 0x04001BC4 RID: 7108
		public const string char_bad_jump_special = "event:/char/badeline/jump_special";

		// Token: 0x04001BC5 RID: 7109
		public const string char_bad_temple_move_first = "event:/char/badeline/temple_move_first";

		// Token: 0x04001BC6 RID: 7110
		public const string char_bad_temple_move_chats = "event:/char/badeline/temple_move_chats";

		// Token: 0x04001BC7 RID: 7111
		public const string char_bad_boss_prefightgetup = "event:/char/badeline/boss_prefight_getup";

		// Token: 0x04001BC8 RID: 7112
		public const string char_bad_boss_hug = "event:/char/badeline/boss_hug";

		// Token: 0x04001BC9 RID: 7113
		public const string char_bad_boss_idle_air = "event:/char/badeline/boss_idle_air";

		// Token: 0x04001BCA RID: 7114
		public const string char_bad_boss_idle_ground_loop = "event:/char/badeline/boss_idle_ground";

		// Token: 0x04001BCB RID: 7115
		public const string char_bad_boss_bullet = "event:/char/badeline/boss_bullet";

		// Token: 0x04001BCC RID: 7116
		public const string char_bad_boss_laser_charge = "event:/char/badeline/boss_laser_charge";

		// Token: 0x04001BCD RID: 7117
		public const string char_bad_boss_laser_fire = "event:/char/badeline/boss_laser_fire";

		// Token: 0x04001BCE RID: 7118
		public const string char_bad_booster_begin = "event:/char/badeline/booster_begin";

		// Token: 0x04001BCF RID: 7119
		public const string char_bad_booster_throw = "event:/char/badeline/booster_throw";

		// Token: 0x04001BD0 RID: 7120
		public const string char_bad_booster_relocate = "event:/char/badeline/booster_relocate";

		// Token: 0x04001BD1 RID: 7121
		public const string char_bad_booster_reappear = "event:/char/badeline/booster_reappear";

		// Token: 0x04001BD2 RID: 7122
		public const string char_bad_booster_final = "event:/char/badeline/booster_final";

		// Token: 0x04001BD3 RID: 7123
		public const string char_bad_maddy_join = "event:/char/badeline/maddy_join";

		// Token: 0x04001BD4 RID: 7124
		public const string char_bad_maddy_split = "event:/char/badeline/maddy_split";

		// Token: 0x04001BD5 RID: 7125
		public const string char_bad_maddy_join_quick = "event:/new_content/char/badeline/maddy_join_quick";

		// Token: 0x04001BD6 RID: 7126
		public const string char_bad_booster_first_appear = "event:/new_content/char/badeline/booster_first_appear";

		// Token: 0x04001BD7 RID: 7127
		public const string char_bad_booster_relocate_slow = "event:/new_content/char/badeline/booster_relocate_slow";

		// Token: 0x04001BD8 RID: 7128
		public const string char_bad_booster_finalfinal_part1 = "event:/new_content/char/badeline/booster_finalfinal_part1";

		// Token: 0x04001BD9 RID: 7129
		public const string char_bad_booster_finalfinal_part2 = "event:/new_content/char/badeline/booster_finalfinal_part2";

		// Token: 0x04001BDA RID: 7130
		public const string char_bad_birdscene_float = "event:/new_content/char/badeline/birdcrash_scene_float";

		// Token: 0x04001BDB RID: 7131
		public const string char_gran_laugh_firstphrase = "event:/char/granny/laugh_firstphrase";

		// Token: 0x04001BDC RID: 7132
		public const string char_gran_laugh_oneha = "event:/char/granny/laugh_oneha";

		// Token: 0x04001BDD RID: 7133
		public const string char_gran_cane_tap = "event:/char/granny/cane_tap";

		// Token: 0x04001BDE RID: 7134
		public const string char_gran_dissipate = "event:/new_content/char/granny/dissipate";

		// Token: 0x04001BDF RID: 7135
		public const string char_gran_cane_tap_ending = "event:/new_content/char/granny/cane_tap_ending";

		// Token: 0x04001BE0 RID: 7136
		public const string char_oshiro_chat_turn_right = "event:/char/oshiro/chat_turn_right";

		// Token: 0x04001BE1 RID: 7137
		public const string char_oshiro_chat_turn_left = "event:/char/oshiro/chat_turn_left";

		// Token: 0x04001BE2 RID: 7138
		public const string char_oshiro_chat_collapse = "event:/char/oshiro/chat_collapse";

		// Token: 0x04001BE3 RID: 7139
		public const string char_oshiro_chat_getup = "event:/char/oshiro/chat_get_up";

		// Token: 0x04001BE4 RID: 7140
		public const string char_oshiro_boss_transform_begin = "event:/char/oshiro/boss_transform_begin";

		// Token: 0x04001BE5 RID: 7141
		public const string char_oshiro_boss_transform_burst = "event:/char/oshiro/boss_transform_burst";

		// Token: 0x04001BE6 RID: 7142
		public const string char_oshiro_boss_enterscreen = "event:/char/oshiro/boss_enter_screen";

		// Token: 0x04001BE7 RID: 7143
		public const string char_oshiro_boss_reform = "event:/char/oshiro/boss_reform";

		// Token: 0x04001BE8 RID: 7144
		public const string char_oshiro_boss_precharge = "event:/char/oshiro/boss_precharge";

		// Token: 0x04001BE9 RID: 7145
		public const string char_oshiro_boss_charge = "event:/char/oshiro/boss_charge";

		// Token: 0x04001BEA RID: 7146
		public const string char_oshiro_boss_slam_first = "event:/char/oshiro/boss_slam_first";

		// Token: 0x04001BEB RID: 7147
		public const string char_oshiro_boss_slam_final = "event:/char/oshiro/boss_slam_final";

		// Token: 0x04001BEC RID: 7148
		public const string char_oshiro_move_01_0xa_exit = "event:/char/oshiro/move_01_0xa_exit";

		// Token: 0x04001BED RID: 7149
		public const string char_oshiro_move_02_03a_exit = "event:/char/oshiro/move_02_03a_exit";

		// Token: 0x04001BEE RID: 7150
		public const string char_oshiro_move_03_08a_exit = "event:/char/oshiro/move_03_08a_exit";

		// Token: 0x04001BEF RID: 7151
		public const string char_oshiro_move_04_pace_right = "event:/char/oshiro/move_04_pace_right";

		// Token: 0x04001BF0 RID: 7152
		public const string char_oshiro_move_04_pace_left = "event:/char/oshiro/move_04_pace_left";

		// Token: 0x04001BF1 RID: 7153
		public const string char_oshiro_move_05_09b_exit = "event:/char/oshiro/move_05_09b_exit";

		// Token: 0x04001BF2 RID: 7154
		public const string char_oshiro_move_06_04d_exit = "event:/char/oshiro/move_06_04d_exit";

		// Token: 0x04001BF3 RID: 7155
		public const string char_oshiro_move_07_roof00_enter = "event:/char/oshiro/move_07_roof00_enter";

		// Token: 0x04001BF4 RID: 7156
		public const string char_oshiro_move_08_roof07_exit = "event:/char/oshiro/move_08_roof07_exit";

		// Token: 0x04001BF5 RID: 7157
		public const string char_tutghost_appear = "event:/new_content/char/tutorial_ghost/appear";

		// Token: 0x04001BF6 RID: 7158
		public const string char_tutghost_disappear = "event:/new_content/char/tutorial_ghost/disappear";

		// Token: 0x04001BF7 RID: 7159
		public const string char_tutghost_jump = "event:/new_content/char/tutorial_ghost/jump";

		// Token: 0x04001BF8 RID: 7160
		public const string char_tutghost_jump_super = "event:/new_content/char/tutorial_ghost/jump_super";

		// Token: 0x04001BF9 RID: 7161
		public const string char_tutghost_dash_left = "event:/new_content/char/tutorial_ghost/dash_red_left";

		// Token: 0x04001BFA RID: 7162
		public const string char_tutghost_dash_right = "event:/new_content/char/tutorial_ghost/dash_red_right";

		// Token: 0x04001BFB RID: 7163
		public const string char_tutghost_dreamblock_sequence = "event:/new_content/char/tutorial_ghost/dreamblock_sequence";

		// Token: 0x04001BFC RID: 7164
		public const string char_tutghost_land = "event:/new_content/char/tutorial_ghost/land";

		// Token: 0x04001BFD RID: 7165
		public const string char_tutghost_grab = "event:/new_content/char/tutorial_ghost/grab";

		// Token: 0x04001BFE RID: 7166
		public const string char_tutghost_footstep = "event:/new_content/char/tutorial_ghost/footstep";

		// Token: 0x04001BFF RID: 7167
		public const string char_tutghost_handhold = "event:/new_content/char/tutorial_ghost/handhold";

		// Token: 0x04001C00 RID: 7168
		public const string game_gen_diamond_touch = "event:/game/general/diamond_touch";

		// Token: 0x04001C01 RID: 7169
		public const string game_gen_diamond_return = "event:/game/general/diamond_return";

		// Token: 0x04001C02 RID: 7170
		public const string game_gen_strawberry_touch = "event:/game/general/strawberry_touch";

		// Token: 0x04001C03 RID: 7171
		public const string game_gen_strawberry_blue_touch = "event:/game/general/strawberry_blue_touch";

		// Token: 0x04001C04 RID: 7172
		public const string game_gen_strawberry_pulse = "event:/game/general/strawberry_pulse";

		// Token: 0x04001C05 RID: 7173
		public const string game_gen_strawberry_blue_pulse = "event:/game/general/strawberry_blue_pulse";

		// Token: 0x04001C06 RID: 7174
		public const string game_gen_strawberry_get = "event:/game/general/strawberry_get";

		// Token: 0x04001C07 RID: 7175
		public const string game_gen_strawberry_flyaway = "event:/game/general/strawberry_flyaway";

		// Token: 0x04001C08 RID: 7176
		public const string game_gen_strawberry_laugh = "event:/game/general/strawberry_laugh";

		// Token: 0x04001C09 RID: 7177
		public const string game_gen_strawberry_wingflap = "event:/game/general/strawberry_wingflap";

		// Token: 0x04001C0A RID: 7178
		public const string game_gen_seed_pulse = "event:/game/general/seed_pulse";

		// Token: 0x04001C0B RID: 7179
		public const string game_gen_seed_touch = "event:/game/general/seed_touch";

		// Token: 0x04001C0C RID: 7180
		public const string game_gen_seed_poof = "event:/game/general/seed_poof";

		// Token: 0x04001C0D RID: 7181
		public const string game_gen_seed_reappear = "event:/game/general/seed_reappear";

		// Token: 0x04001C0E RID: 7182
		public const string game_gen_seed_complete_main = "event:/game/general/seed_complete_main";

		// Token: 0x04001C0F RID: 7183
		public const string game_gen_seed_complete_berry = "event:/game/general/seed_complete_berry";

		// Token: 0x04001C10 RID: 7184
		public const string game_gen_key_get = "event:/game/general/key_get";

		// Token: 0x04001C11 RID: 7185
		public const string game_gen_lookout_use = "event:/game/general/lookout_use";

		// Token: 0x04001C12 RID: 7186
		public const string game_gen_lookout_move = "event:/game/general/lookout_move";

		// Token: 0x04001C13 RID: 7187
		public const string game_gen_touchswitch_any = "event:/game/general/touchswitch_any";

		// Token: 0x04001C14 RID: 7188
		public const string game_gen_touchswitch_last_oneshot = "event:/game/general/touchswitch_last_oneshot";

		// Token: 0x04001C15 RID: 7189
		public const string game_gen_touchswitch_last_cutoff = "event:/game/general/touchswitch_last_cutoff";

		// Token: 0x04001C16 RID: 7190
		public const string game_gen_touchswitch_gate_open = "event:/game/general/touchswitch_gate_open";

		// Token: 0x04001C17 RID: 7191
		public const string game_gen_touchswitch_gate_finish = "event:/game/general/touchswitch_gate_finish";

		// Token: 0x04001C18 RID: 7192
		public const string game_gen_crystalheart_bounce = "event:/game/general/crystalheart_bounce";

		// Token: 0x04001C19 RID: 7193
		public const string game_gen_crystalheart_pulse = "event:/game/general/crystalheart_pulse";

		// Token: 0x04001C1A RID: 7194
		public const string game_gen_crystalheart_red_get = "event:/game/general/crystalheart_red_get";

		// Token: 0x04001C1B RID: 7195
		public const string game_gen_crystalheart_blue_get = "event:/game/general/crystalheart_blue_get";

		// Token: 0x04001C1C RID: 7196
		public const string game_gen_crystalheart_gold_get = "event:/game/general/crystalheart_gold_get";

		// Token: 0x04001C1D RID: 7197
		public const string game_gen_thing_booped = "event:/game/general/thing_booped";

		// Token: 0x04001C1E RID: 7198
		public const string game_gen_cassette_bubblereturn = "event:/game/general/cassette_bubblereturn";

		// Token: 0x04001C1F RID: 7199
		public const string game_gen_cassette_get = "event:/game/general/cassette_get";

		// Token: 0x04001C20 RID: 7200
		public const string game_gen_cassette_preview = "event:/game/general/cassette_preview";

		// Token: 0x04001C21 RID: 7201
		public const string game_gen_spring = "event:/game/general/spring";

		// Token: 0x04001C22 RID: 7202
		public const string game_gen_platform_disintegrate = "event:/game/general/platform_disintegrate";

		// Token: 0x04001C23 RID: 7203
		public const string game_gen_platform_unit_return = "event:/game/general/platform_return";

		// Token: 0x04001C24 RID: 7204
		public const string game_gen_secret_revealed = "event:/game/general/secret_revealed";

		// Token: 0x04001C25 RID: 7205
		public const string game_gen_passageclosedbehind = "event:/game/general/passage_closed_behind";

		// Token: 0x04001C26 RID: 7206
		public const string game_gen_bird_squawk = "event:/game/general/bird_squawk";

		// Token: 0x04001C27 RID: 7207
		public const string game_gen_bird_startle = "event:/game/general/bird_startle";

		// Token: 0x04001C28 RID: 7208
		public const string game_gen_bird_in = "event:/game/general/bird_in";

		// Token: 0x04001C29 RID: 7209
		public const string game_gen_bird_land_dirt = "event:/game/general/bird_land_dirt";

		// Token: 0x04001C2A RID: 7210
		public const string game_gen_bird_peck = "event:/game/general/bird_peck";

		// Token: 0x04001C2B RID: 7211
		public const string game_gen_birdbaby_hop = "event:/game/general/birdbaby_hop";

		// Token: 0x04001C2C RID: 7212
		public const string game_gen_birdbaby_flyaway = "event:/game/general/birdbaby_flyaway";

		// Token: 0x04001C2D RID: 7213
		public const string game_gen_birdbaby_tweet_loop = "event:/game/general/birdbaby_tweet_loop";

		// Token: 0x04001C2E RID: 7214
		public const string game_gen_wallbreak_dirt = "event:/game/general/wall_break_dirt";

		// Token: 0x04001C2F RID: 7215
		public const string game_gen_wallbreak_stone = "event:/game/general/wall_break_stone";

		// Token: 0x04001C30 RID: 7216
		public const string game_gen_wallbreak_ice = "event:/game/general/wall_break_ice";

		// Token: 0x04001C31 RID: 7217
		public const string game_gen_wallbreak_wood = "event:/game/general/wall_break_wood";

		// Token: 0x04001C32 RID: 7218
		public const string game_gen_fallblock_shake = "event:/game/general/fallblock_shake";

		// Token: 0x04001C33 RID: 7219
		public const string game_gen_fallblock_impact = "event:/game/general/fallblock_impact";

		// Token: 0x04001C34 RID: 7220
		public const string game_gen_debris_stone = "event:/game/general/debris_stone";

		// Token: 0x04001C35 RID: 7221
		public const string game_gen_debris_wood = "event:/game/general/debris_wood";

		// Token: 0x04001C36 RID: 7222
		public const string game_gen_debris_dirt = "event:/game/general/debris_dirt";

		// Token: 0x04001C37 RID: 7223
		public const string game_gen_spotlight_intro = "event:/game/general/spotlight_intro";

		// Token: 0x04001C38 RID: 7224
		public const string game_gen_spotlight_outro = "event:/game/general/spotlight_outro";

		// Token: 0x04001C39 RID: 7225
		public const string game_gen_cassetteblock_switch_1 = "event:/game/general/cassette_block_switch_1";

		// Token: 0x04001C3A RID: 7226
		public const string game_gen_cassetteblock_switch_2 = "event:/game/general/cassette_block_switch_2";

		// Token: 0x04001C3B RID: 7227
		public const string game_assist_screenbottom = "event:/game/general/assist_screenbottom";

		// Token: 0x04001C3C RID: 7228
		public const string game_assist_dreamblockbounce = "event:/game/general/assist_dreamblockbounce";

		// Token: 0x04001C3D RID: 7229
		public const string game_assist_nonsolidblock_in = "event:/game/general/assist_nonsolid_in";

		// Token: 0x04001C3E RID: 7230
		public const string game_assist_nonsolidblock_out = "event:/game/general/assist_nonsolid_out";

		// Token: 0x04001C3F RID: 7231
		public const string game_assist_dash_aim = "event:/game/general/assist_dash_aim";

		// Token: 0x04001C40 RID: 7232
		public const string game_00_car_down = "event:/game/00_prologue/car_down";

		// Token: 0x04001C41 RID: 7233
		public const string game_00_car_up = "event:/game/00_prologue/car_up";

		// Token: 0x04001C42 RID: 7234
		public const string game_00_fallingblock_prologue_shake = "event:/game/00_prologue/fallblock_first_shake";

		// Token: 0x04001C43 RID: 7235
		public const string game_00_fallingblock_prologue_impact = "event:/game/00_prologue/fallblock_first_impact";

		// Token: 0x04001C44 RID: 7236
		public const string game_00_bridge_rumble_loop = "event:/game/00_prologue/bridge_rumble_loop";

		// Token: 0x04001C45 RID: 7237
		public const string game_00_bridge_supportbreak = "event:/game/00_prologue/bridge_support_break";

		// Token: 0x04001C46 RID: 7238
		public const string game_01_zipmover = "event:/game/01_forsaken_city/zip_mover";

		// Token: 0x04001C47 RID: 7239
		public const string game_01_fallingblock_ice_shake = "event:/game/01_forsaken_city/fallblock_ice_shake";

		// Token: 0x04001C48 RID: 7240
		public const string game_01_fallingblock_ice_impact = "event:/game/01_forsaken_city/fallblock_ice_impact";

		// Token: 0x04001C49 RID: 7241
		public const string game_01_birdbros_fly_loop = "event:/game/01_forsaken_city/birdbros_fly_loop";

		// Token: 0x04001C4A RID: 7242
		public const string game_01_birdbros_thrust = "event:/game/01_forsaken_city/birdbros_thrust";

		// Token: 0x04001C4B RID: 7243
		public const string game_01_birdbros_finish = "event:/game/01_forsaken_city/birdbros_finish";

		// Token: 0x04001C4C RID: 7244
		public const string game_01_console_static_short = "event:/game/01_forsaken_city/console_static_short";

		// Token: 0x04001C4D RID: 7245
		public const string game_01_console_static_long = "event:/game/01_forsaken_city/console_static_long";

		// Token: 0x04001C4E RID: 7246
		public const string game_01_console_static_loop = "event:/game/01_forsaken_city/console_static_loop";

		// Token: 0x04001C4F RID: 7247
		public const string game_01_console_white = "event:/game/01_forsaken_city/console_white";

		// Token: 0x04001C50 RID: 7248
		public const string game_01_console_purple = "event:/game/01_forsaken_city/console_purple";

		// Token: 0x04001C51 RID: 7249
		public const string game_01_console_yellow = "event:/game/01_forsaken_city/console_yellow";

		// Token: 0x04001C52 RID: 7250
		public const string game_01_console_red = "event:/game/01_forsaken_city/console_red";

		// Token: 0x04001C53 RID: 7251
		public const string game_01_console_blue = "event:/game/01_forsaken_city/console_blue";

		// Token: 0x04001C54 RID: 7252
		public const string game_02_sequence_mirror = "event:/game/02_old_site/sequence_mirror";

		// Token: 0x04001C55 RID: 7253
		public const string game_02_sequence_badeline_intro = "event:/game/02_old_site/sequence_badeline_intro";

		// Token: 0x04001C56 RID: 7254
		public const string game_02_sequence_phone_pickup = "event:/game/02_old_site/sequence_phone_pickup";

		// Token: 0x04001C57 RID: 7255
		public const string game_02_sequence_phone_transform = "event:/game/02_old_site/sequence_phone_transform";

		// Token: 0x04001C58 RID: 7256
		public const string game_02_sequence_phone_ring_loop = "event:/game/02_old_site/sequence_phone_ring_loop";

		// Token: 0x04001C59 RID: 7257
		public const string game_02_sequence_phone_ringtone_loop = "event:/game/02_old_site/sequence_phone_ringtone_loop";

		// Token: 0x04001C5A RID: 7258
		public const string game_02_theoselfie_foley = "event:/game/02_old_site/theoselfie_foley";

		// Token: 0x04001C5B RID: 7259
		public const string game_02_theoselfie_photo_in = "event:/game/02_old_site/theoselfie_photo_in";

		// Token: 0x04001C5C RID: 7260
		public const string game_02_theoselfie_photo_out = "event:/game/02_old_site/theoselfie_photo_out";

		// Token: 0x04001C5D RID: 7261
		public const string game_02_theoselfie_photo_filter = "event:/game/02_old_site/theoselfie_photo_filter";

		// Token: 0x04001C5E RID: 7262
		public const string game_02_lantern_hit = "event:/game/02_old_site/lantern_hit";

		// Token: 0x04001C5F RID: 7263
		public const string game_03_door_wood_open = "event:/game/03_resort/door_wood_open";

		// Token: 0x04001C60 RID: 7264
		public const string game_03_door_wood_close = "event:/game/03_resort/door_wood_close";

		// Token: 0x04001C61 RID: 7265
		public const string game_03_fallingblock_wood_shake = "event:/game/03_resort/fallblock_wood_shake";

		// Token: 0x04001C62 RID: 7266
		public const string game_03_fallingblock_wood_impact = "event:/game/03_resort/fallblock_wood_impact";

		// Token: 0x04001C63 RID: 7267
		public const string game_03_fallingblock_wood_distantimpact = "event:/game/03_resort/fallblock_wooddistant_impact";

		// Token: 0x04001C64 RID: 7268
		public const string game_03_door_metal_open = "event:/game/03_resort/door_metal_open";

		// Token: 0x04001C65 RID: 7269
		public const string game_03_door_metal_close = "event:/game/03_resort/door_metal_close";

		// Token: 0x04001C66 RID: 7270
		public const string game_03_door_trapdoor_fromtop = "event:/game/03_resort/trapdoor_fromtop";

		// Token: 0x04001C67 RID: 7271
		public const string game_03_door_trapdoor_frombottom = "event:/game/03_resort/trapdoor_frombottom";

		// Token: 0x04001C68 RID: 7272
		public const string game_03_key_unlock = "event:/game/03_resort/key_unlock";

		// Token: 0x04001C69 RID: 7273
		public const string game_03_forcefield_vanish = "event:/game/03_resort/forcefield_vanish";

		// Token: 0x04001C6A RID: 7274
		public const string game_03_forcefield_bump = "event:/game/03_resort/forcefield_bump";

		// Token: 0x04001C6B RID: 7275
		public const string game_03_platform_horiz_left = "event:/game/03_resort/platform_horiz_left";

		// Token: 0x04001C6C RID: 7276
		public const string game_03_platform_horiz_right = "event:/game/03_resort/platform_horiz_right";

		// Token: 0x04001C6D RID: 7277
		public const string game_03_lantern_bump = "event:/game/03_resort/lantern_bump";

		// Token: 0x04001C6E RID: 7278
		public const string game_03_deskbell_again = "event:/game/03_resort/deskbell_again";

		// Token: 0x04001C6F RID: 7279
		public const string game_03_clutterswitch_return = "event:/game/03_resort/clutterswitch_return";

		// Token: 0x04001C70 RID: 7280
		public const string game_03_clutterswitch_squish = "event:/game/03_resort/clutterswitch_squish";

		// Token: 0x04001C71 RID: 7281
		public const string game_03_clutterswitch_press_books = "event:/game/03_resort/clutterswitch_books";

		// Token: 0x04001C72 RID: 7282
		public const string game_03_clutterswitch_press_boxes = "event:/game/03_resort/clutterswitch_boxes";

		// Token: 0x04001C73 RID: 7283
		public const string game_03_clutterswitch_press_linens = "event:/game/03_resort/clutterswitch_linens";

		// Token: 0x04001C74 RID: 7284
		public const string game_03_clutterswitch_press_finish = "event:/game/03_resort/clutterswitch_finish";

		// Token: 0x04001C75 RID: 7285
		public const string game_03_fluff_tendril_emerge = "event:/game/03_resort/fluff_tendril_emerge";

		// Token: 0x04001C76 RID: 7286
		public const string game_03_fluff_tendril_recede = "event:/game/03_resort/fluff_tendril_recede";

		// Token: 0x04001C77 RID: 7287
		public const string game_03_fluff_tendril_touch = "event:/game/03_resort/fluff_tendril_touch";

		// Token: 0x04001C78 RID: 7288
		public const string game_03_platform_vert_start = "event:/game/03_resort/platform_vert_start";

		// Token: 0x04001C79 RID: 7289
		public const string game_03_platform_vert_down_loop = "event:/game/03_resort/platform_vert_down_loop";

		// Token: 0x04001C7A RID: 7290
		public const string game_03_platform_vert_up_loop = "event:/game/03_resort/platform_vert_up_loop";

		// Token: 0x04001C7B RID: 7291
		public const string game_03_platform_vert_end = "event:/game/03_resort/platform_vert_end";

		// Token: 0x04001C7C RID: 7292
		public const string game_03_memo_in = "event:/game/03_resort/memo_in";

		// Token: 0x04001C7D RID: 7293
		public const string game_03_memo_out = "event:/game/03_resort/memo_out";

		// Token: 0x04001C7E RID: 7294
		public const string game_03_suite_badintro = "event:/game/03_resort/suite_bad_intro";

		// Token: 0x04001C7F RID: 7295
		public const string game_03_suite_badmirrorbreak = "event:/game/03_resort/suite_bad_mirrorbreak";

		// Token: 0x04001C80 RID: 7296
		public const string game_03_suite_badmovestageleft = "event:/game/03_resort/suite_bad_movestageleft";

		// Token: 0x04001C81 RID: 7297
		public const string game_03_suite_badceilingbreak = "event:/game/03_resort/suite_bad_ceilingbreak";

		// Token: 0x04001C82 RID: 7298
		public const string game_03_suite_badexittop = "event:/game/03_resort/suite_bad_exittop";

		// Token: 0x04001C83 RID: 7299
		public const string game_03_suite_badmoveroof = "event:/game/03_resort/suite_bad_moveroof";

		// Token: 0x04001C84 RID: 7300
		public const string game_03_sequence_oshiro_intro = "event:/game/03_resort/sequence_oshiro_intro";

		// Token: 0x04001C85 RID: 7301
		public const string game_03_sequence_oshirofluff_pt1 = "event:/game/03_resort/sequence_oshirofluff_pt1";

		// Token: 0x04001C86 RID: 7302
		public const string game_03_sequence_oshirofluff_pt2 = "event:/game/03_resort/sequence_oshirofluff_pt2";

		// Token: 0x04001C87 RID: 7303
		public const string game_04_arrowblock_activate = "event:/game/04_cliffside/arrowblock_activate";

		// Token: 0x04001C88 RID: 7304
		public const string game_04_arrowblock_move_loop = "event:/game/04_cliffside/arrowblock_move";

		// Token: 0x04001C89 RID: 7305
		public const string game_04_arrowblock_side_depress = "event:/game/04_cliffside/arrowblock_side_depress";

		// Token: 0x04001C8A RID: 7306
		public const string game_04_arrowblock_side_release = "event:/game/04_cliffside/arrowblock_side_release";

		// Token: 0x04001C8B RID: 7307
		public const string game_04_arrowblock_break = "event:/game/04_cliffside/arrowblock_break";

		// Token: 0x04001C8C RID: 7308
		public const string game_04_arrowblock_debris = "event:/game/04_cliffside/arrowblock_debris";

		// Token: 0x04001C8D RID: 7309
		public const string game_04_arrowblock_reform_begin = "event:/game/04_cliffside/arrowblock_reform_begin";

		// Token: 0x04001C8E RID: 7310
		public const string game_04_arrowblock_reappear = "event:/game/04_cliffside/arrowblock_reappear";

		// Token: 0x04001C8F RID: 7311
		public const string game_04_greenbooster_enter = "event:/game/04_cliffside/greenbooster_enter";

		// Token: 0x04001C90 RID: 7312
		public const string game_04_greenbooster_dash = "event:/game/04_cliffside/greenbooster_dash";

		// Token: 0x04001C91 RID: 7313
		public const string game_04_greenbooster_end = "event:/game/04_cliffside/greenbooster_end";

		// Token: 0x04001C92 RID: 7314
		public const string game_04_greenbooster_reappear = "event:/game/04_cliffside/greenbooster_reappear";

		// Token: 0x04001C93 RID: 7315
		public const string game_04_cloud_blue_boost = "event:/game/04_cliffside/cloud_blue_boost";

		// Token: 0x04001C94 RID: 7316
		public const string game_04_cloud_pink_boost = "event:/game/04_cliffside/cloud_pink_boost";

		// Token: 0x04001C95 RID: 7317
		public const string game_04_cloud_pink_reappear = "event:/game/04_cliffside/cloud_pink_reappear";

		// Token: 0x04001C96 RID: 7318
		public const string game_04_snowball_spawn = "event:/game/04_cliffside/snowball_spawn";

		// Token: 0x04001C97 RID: 7319
		public const string game_04_snowball_impact = "event:/game/04_cliffside/snowball_impact";

		// Token: 0x04001C98 RID: 7320
		public const string game_04_stone_blockade = "event:/game/04_cliffside/stone_blockade";

		// Token: 0x04001C99 RID: 7321
		public const string game_04_whiteblock_fallthru = "event:/game/04_cliffside/whiteblock_fallthru";

		// Token: 0x04001C9A RID: 7322
		public const string game_04_gondola_theo_fall = "event:/game/04_cliffside/gondola_theo_fall";

		// Token: 0x04001C9B RID: 7323
		public const string game_04_gondola_theo_recover = "event:/game/04_cliffside/gondola_theo_recover";

		// Token: 0x04001C9C RID: 7324
		public const string game_04_gondola_theo_lever_start = "event:/game/04_cliffside/gondola_theo_lever_start";

		// Token: 0x04001C9D RID: 7325
		public const string game_04_gondola_cliffmechanism_start = "event:/game/04_cliffside/gondola_cliffmechanism_start";

		// Token: 0x04001C9E RID: 7326
		public const string game_04_gondola_movement_loop = "event:/game/04_cliffside/gondola_movement_loop";

		// Token: 0x04001C9F RID: 7327
		public const string game_04_gondola_theoselfie_halt = "event:/game/04_cliffside/gondola_theoselfie_halt";

		// Token: 0x04001CA0 RID: 7328
		public const string game_04_gondola_halted_loop = "event:/game/04_cliffside/gondola_halted_loop";

		// Token: 0x04001CA1 RID: 7329
		public const string game_04_gondola_theo_lever_fail = "event:/game/04_cliffside/gondola_theo_lever_fail";

		// Token: 0x04001CA2 RID: 7330
		public const string game_04_gondola_scaryhair_01 = "event:/game/04_cliffside/gondola_scaryhair_01";

		// Token: 0x04001CA3 RID: 7331
		public const string game_04_gondola_scaryhair_02 = "event:/game/04_cliffside/gondola_scaryhair_02";

		// Token: 0x04001CA4 RID: 7332
		public const string game_04_gondola_scaryhair_03 = "event:/game/04_cliffside/gondola_scaryhair_03";

		// Token: 0x04001CA5 RID: 7333
		public const string game_04_gondola_restart = "event:/game/04_cliffside/gondola_restart";

		// Token: 0x04001CA6 RID: 7334
		public const string game_04_gondola_finish = "event:/game/04_cliffside/gondola_finish";

		// Token: 0x04001CA7 RID: 7335
		public const string game_05_key_unlock_light = "event:/game/05_mirror_temple/key_unlock_light";

		// Token: 0x04001CA8 RID: 7336
		public const string game_05_key_unlock_dark = "event:/game/05_mirror_temple/key_unlock_dark";

		// Token: 0x04001CA9 RID: 7337
		public const string game_05_redbooster_enter = "event:/game/05_mirror_temple/redbooster_enter";

		// Token: 0x04001CAA RID: 7338
		public const string game_05_redbooster_dash = "event:/game/05_mirror_temple/redbooster_dash";

		// Token: 0x04001CAB RID: 7339
		public const string game_05_redbooster_move_loop = "event:/game/05_mirror_temple/redbooster_move";

		// Token: 0x04001CAC RID: 7340
		public const string game_05_redbooster_end = "event:/game/05_mirror_temple/redbooster_end";

		// Token: 0x04001CAD RID: 7341
		public const string game_05_redbooster_reappear = "event:/game/05_mirror_temple/redbooster_reappear";

		// Token: 0x04001CAE RID: 7342
		public const string game_05_torch_activate = "event:/game/05_mirror_temple/torch_activate";

		// Token: 0x04001CAF RID: 7343
		public const string game_05_bladespinner_spin = "event:/game/05_mirror_temple/bladespinner_spin";

		// Token: 0x04001CB0 RID: 7344
		public const string game_05_room_lightlevel_down = "event:/game/05_mirror_temple/room_lightlevel_down";

		// Token: 0x04001CB1 RID: 7345
		public const string game_05_room_lightlevel_up = "event:/game/05_mirror_temple/room_lightlevel_up";

		// Token: 0x04001CB2 RID: 7346
		public const string game_05_swapblock_move = "event:/game/05_mirror_temple/swapblock_move";

		// Token: 0x04001CB3 RID: 7347
		public const string game_05_swapblock_move_end = "event:/game/05_mirror_temple/swapblock_move_end";

		// Token: 0x04001CB4 RID: 7348
		public const string game_05_swapblock_return = "event:/game/05_mirror_temple/swapblock_return";

		// Token: 0x04001CB5 RID: 7349
		public const string game_05_swapblock_return_end = "event:/game/05_mirror_temple/swapblock_return_end";

		// Token: 0x04001CB6 RID: 7350
		public const string game_05_gatebutton_depress = "event:/game/05_mirror_temple/button_depress";

		// Token: 0x04001CB7 RID: 7351
		public const string game_05_gatebutton_return = "event:/game/05_mirror_temple/button_return";

		// Token: 0x04001CB8 RID: 7352
		public const string game_05_gatebutton_activate = "event:/game/05_mirror_temple/button_activate";

		// Token: 0x04001CB9 RID: 7353
		public const string game_05_gate_main_open = "event:/game/05_mirror_temple/gate_main_open";

		// Token: 0x04001CBA RID: 7354
		public const string game_05_gate_main_close = "event:/game/05_mirror_temple/gate_main_close";

		// Token: 0x04001CBB RID: 7355
		public const string game_05_gate_theo_open = "event:/game/05_mirror_temple/gate_theo_open";

		// Token: 0x04001CBC RID: 7356
		public const string game_05_gate_theo_close = "event:/game/05_mirror_temple/gate_theo_close";

		// Token: 0x04001CBD RID: 7357
		public const string game_05_sequence_mainmirror_torch_1 = "event:/game/05_mirror_temple/mainmirror_torch_lit_1";

		// Token: 0x04001CBE RID: 7358
		public const string game_05_sequence_mainmirror_torch_2 = "event:/game/05_mirror_temple/mainmirror_torch_lit_2";

		// Token: 0x04001CBF RID: 7359
		public const string game_05_sequence_mainmirror_torch_loop = "event:/game/05_mirror_temple/mainmirror_torch_loop";

		// Token: 0x04001CC0 RID: 7360
		public const string game_05_sequence_mainmirror_reveal = "event:/game/05_mirror_temple/mainmirror_reveal";

		// Token: 0x04001CC1 RID: 7361
		public const string game_05_seeker_playercontrolstart = "event:/game/05_mirror_temple/seeker_playercontrolstart";

		// Token: 0x04001CC2 RID: 7362
		public const string game_05_seeker_statuebreak = "event:/game/05_mirror_temple/seeker_statue_break";

		// Token: 0x04001CC3 RID: 7363
		public const string game_05_seeker_aggro = "event:/game/05_mirror_temple/seeker_aggro";

		// Token: 0x04001CC4 RID: 7364
		public const string game_05_seeker_dash = "event:/game/05_mirror_temple/seeker_dash";

		// Token: 0x04001CC5 RID: 7365
		public const string game_05_seeker_dash_turn = "event:/game/05_mirror_temple/seeker_dash_turn";

		// Token: 0x04001CC6 RID: 7366
		public const string game_05_seeker_impact_normal = "event:/game/05_mirror_temple/seeker_hit_normal";

		// Token: 0x04001CC7 RID: 7367
		public const string game_05_seeker_impact_lightwall = "event:/game/05_mirror_temple/seeker_hit_lightwall";

		// Token: 0x04001CC8 RID: 7368
		public const string game_05_seeker_booped = "event:/game/05_mirror_temple/seeker_booped";

		// Token: 0x04001CC9 RID: 7369
		public const string game_05_seeker_death = "event:/game/05_mirror_temple/seeker_death";

		// Token: 0x04001CCA RID: 7370
		public const string game_05_seeker_revive = "event:/game/05_mirror_temple/seeker_revive";

		// Token: 0x04001CCB RID: 7371
		public const string game_05_crackedwall_vanish = "event:/game/05_mirror_temple/crackedwall_vanish";

		// Token: 0x04001CCC RID: 7372
		public const string game_05_crystaltheo_breakfree = "event:/game/05_mirror_temple/crystaltheo_break_free";

		// Token: 0x04001CCD RID: 7373
		public const string game_05_crystaltheo_impact_ground = "event:/game/05_mirror_temple/crystaltheo_hit_ground";

		// Token: 0x04001CCE RID: 7374
		public const string game_05_crystaltheo_impact_side = "event:/game/05_mirror_temple/crystaltheo_hit_side";

		// Token: 0x04001CCF RID: 7375
		public const string game_05_eyewall_idle_eyemove = "event:/game/05_mirror_temple/eyebro_eyemove";

		// Token: 0x04001CD0 RID: 7376
		public const string game_05_eyewall_pulse = "event:/game/05_mirror_temple/eye_pulse";

		// Token: 0x04001CD1 RID: 7377
		public const string game_05_eyewall_destroy = "event:/game/05_mirror_temple/eyewall_destroy";

		// Token: 0x04001CD2 RID: 7378
		public const string game_05_eyewall_bounce = "event:/game/05_mirror_temple/eyewall_bounce";

		// Token: 0x04001CD3 RID: 7379
		public const string game_06_fallingblock_boss_shake = "event:/game/06_reflection/fallblock_boss_shake";

		// Token: 0x04001CD4 RID: 7380
		public const string game_06_fallingblock_boss_impact = "event:/game/06_reflection/fallblock_boss_impact";

		// Token: 0x04001CD5 RID: 7381
		public const string game_06_crushblock_activate = "event:/game/06_reflection/crushblock_activate";

		// Token: 0x04001CD6 RID: 7382
		public const string game_06_crushblock_move_loop = "event:/game/06_reflection/crushblock_move_loop";

		// Token: 0x04001CD7 RID: 7383
		public const string game_06_crushblock_move_loop_covert = "event:/game/06_reflection/crushblock_move_loop_covert";

		// Token: 0x04001CD8 RID: 7384
		public const string game_06_crushblock_impact = "event:/game/06_reflection/crushblock_impact";

		// Token: 0x04001CD9 RID: 7385
		public const string game_06_crushblock_return_loop = "event:/game/06_reflection/crushblock_return_loop";

		// Token: 0x04001CDA RID: 7386
		public const string game_06_crushblock_rest = "event:/game/06_reflection/crushblock_rest";

		// Token: 0x04001CDB RID: 7387
		public const string game_06_crushblock_rest_waypoint = "event:/game/06_reflection/crushblock_rest_waypoint";

		// Token: 0x04001CDC RID: 7388
		public const string game_06_badeline_freakout_1 = "event:/game/06_reflection/badeline_freakout_1";

		// Token: 0x04001CDD RID: 7389
		public const string game_06_badeline_freakout_2 = "event:/game/06_reflection/badeline_freakout_2";

		// Token: 0x04001CDE RID: 7390
		public const string game_06_badeline_freakout_3 = "event:/game/06_reflection/badeline_freakout_3";

		// Token: 0x04001CDF RID: 7391
		public const string game_06_badeline_freakout_4 = "event:/game/06_reflection/badeline_freakout_4";

		// Token: 0x04001CE0 RID: 7392
		public const string game_06_badeline_freakout_5 = "event:/game/06_reflection/badeline_freakout_5";

		// Token: 0x04001CE1 RID: 7393
		public const string game_06_badeline_featherslice = "event:/game/06_reflection/badeline_feather_slice";

		// Token: 0x04001CE2 RID: 7394
		public const string game_06_badelinepull_whooshdown = "event:/game/06_reflection/badeline_pull_whooshdown";

		// Token: 0x04001CE3 RID: 7395
		public const string game_06_badelinepull_impact = "event:/game/06_reflection/badeline_pull_impact";

		// Token: 0x04001CE4 RID: 7396
		public const string game_06_badelinepull_rumble_loop = "event:/game/06_reflection/badeline_pull_rumble_loop";

		// Token: 0x04001CE5 RID: 7397
		public const string game_06_badelinepull_cliffbreak = "event:/game/06_reflection/badeline_pull_cliffbreak";

		// Token: 0x04001CE6 RID: 7398
		public const string game_06_fall_spike_smash = "event:/game/06_reflection/fall_spike_smash";

		// Token: 0x04001CE7 RID: 7399
		public const string game_06_supersecret_torch_1 = "event:/game/06_reflection/supersecret_torch_1";

		// Token: 0x04001CE8 RID: 7400
		public const string game_06_supersecret_torch_2 = "event:/game/06_reflection/supersecret_torch_2";

		// Token: 0x04001CE9 RID: 7401
		public const string game_06_supersecret_torch_3 = "event:/game/06_reflection/supersecret_torch_3";

		// Token: 0x04001CEA RID: 7402
		public const string game_06_supersecret_torch_4 = "event:/game/06_reflection/supersecret_torch_4";

		// Token: 0x04001CEB RID: 7403
		public const string game_06_supersecret_dashflavour = "event:/game/06_reflection/supersecret_dashflavour";

		// Token: 0x04001CEC RID: 7404
		public const string game_06_supersecret_heartappear = "event:/game/06_reflection/supersecret_heartappear";

		// Token: 0x04001CED RID: 7405
		public const string game_06_pinballbumper_hit = "event:/game/06_reflection/pinballbumper_hit";

		// Token: 0x04001CEE RID: 7406
		public const string game_06_pinballbumper_reset = "event:/game/06_reflection/pinballbumper_reset";

		// Token: 0x04001CEF RID: 7407
		public const string game_06_feather_get = "event:/game/06_reflection/feather_get";

		// Token: 0x04001CF0 RID: 7408
		public const string game_06_feather_renew = "event:/game/06_reflection/feather_renew";

		// Token: 0x04001CF1 RID: 7409
		public const string game_06_feather_reappear = "event:/game/06_reflection/feather_reappear";

		// Token: 0x04001CF2 RID: 7410
		public const string game_06_feather_state_loop = "event:/game/06_reflection/feather_state_loop";

		// Token: 0x04001CF3 RID: 7411
		public const string game_06_feather_state_warning = "event:/game/06_reflection/feather_state_warning";

		// Token: 0x04001CF4 RID: 7412
		public const string game_06_feather_state_end = "event:/game/06_reflection/feather_state_end";

		// Token: 0x04001CF5 RID: 7413
		public const string game_06_feather_state_bump = "event:/game/06_reflection/feather_state_bump";

		// Token: 0x04001CF6 RID: 7414
		public const string game_06_feather_bubble_get = "event:/game/06_reflection/feather_bubble_get";

		// Token: 0x04001CF7 RID: 7415
		public const string game_06_feather_bubble_renew = "event:/game/06_reflection/feather_bubble_renew";

		// Token: 0x04001CF8 RID: 7416
		public const string game_06_feather_bubble_bounce = "event:/game/06_reflection/feather_bubble_bounce";

		// Token: 0x04001CF9 RID: 7417
		public const string game_06_scaryhair_move = "event:/game/06_reflection/scaryhair_move";

		// Token: 0x04001CFA RID: 7418
		public const string game_06_scaryhair_whoosh = "event:/game/06_reflection/scaryhair_whoosh";

		// Token: 0x04001CFB RID: 7419
		public const string game_06_boss_spikes_burst = "event:/game/06_reflection/boss_spikes_burst";

		// Token: 0x04001CFC RID: 7420
		public const string game_06_hug_image_1 = "event:/game/06_reflection/hug_image_1";

		// Token: 0x04001CFD RID: 7421
		public const string game_06_hug_image_2 = "event:/game/06_reflection/hug_image_2";

		// Token: 0x04001CFE RID: 7422
		public const string game_06_hug_image_3 = "event:/game/06_reflection/hug_image_3";

		// Token: 0x04001CFF RID: 7423
		public const string game_06_hug_badelineglow = "event:/game/06_reflection/hug_badeline_glow";

		// Token: 0x04001D00 RID: 7424
		public const string game_06_hug_levelup_text_in = "event:/game/06_reflection/hug_levelup_text_in";

		// Token: 0x04001D01 RID: 7425
		public const string game_06_hug_levelup_text_out = "event:/game/06_reflection/hug_levelup_text_out";

		// Token: 0x04001D02 RID: 7426
		public const string game_07_altitudecount = "event:/game/07_summit/altitude_count";

		// Token: 0x04001D03 RID: 7427
		public const string game_07_checkpointconfetti = "event:/game/07_summit/checkpoint_confetti";

		// Token: 0x04001D04 RID: 7428
		public const string game_07_gem_get = "event:/game/07_summit/gem_get";

		// Token: 0x04001D05 RID: 7429
		public const string game_07_gem_unlock_1 = "event:/game/07_summit/gem_unlock_1";

		// Token: 0x04001D06 RID: 7430
		public const string game_07_gem_unlock_2 = "event:/game/07_summit/gem_unlock_2";

		// Token: 0x04001D07 RID: 7431
		public const string game_07_gem_unlock_3 = "event:/game/07_summit/gem_unlock_3";

		// Token: 0x04001D08 RID: 7432
		public const string game_07_gem_unlock_4 = "event:/game/07_summit/gem_unlock_4";

		// Token: 0x04001D09 RID: 7433
		public const string game_07_gem_unlock_5 = "event:/game/07_summit/gem_unlock_5";

		// Token: 0x04001D0A RID: 7434
		public const string game_07_gem_unlock_6 = "event:/game/07_summit/gem_unlock_6";

		// Token: 0x04001D0B RID: 7435
		public const string game_07_gem_unlock_complete = "event:/game/07_summit/gem_unlock_complete";

		// Token: 0x04001D0C RID: 7436
		public const string game_09_frontdoor_heartfill = "event:/game/09_core/frontdoor_heartfill";

		// Token: 0x04001D0D RID: 7437
		public const string game_09_frontdoor_unlock = "event:/game/09_core/frontdoor_unlock";

		// Token: 0x04001D0E RID: 7438
		public const string game_09_conveyor_activate = "event:/game/09_core/conveyor_activate";

		// Token: 0x04001D0F RID: 7439
		public const string game_09_hotpinball_activate = "event:/game/09_core/hotpinball_activate";

		// Token: 0x04001D10 RID: 7440
		public const string game_09_bounceblock_touch = "event:/game/09_core/bounceblock_touch";

		// Token: 0x04001D11 RID: 7441
		public const string game_09_bounceblock_break = "event:/game/09_core/bounceblock_break";

		// Token: 0x04001D12 RID: 7442
		public const string game_09_bounceblock_reappear = "event:/game/09_core/bounceblock_reappear";

		// Token: 0x04001D13 RID: 7443
		public const string game_09_iceblock_touch = "event:/game/09_core/iceblock_touch";

		// Token: 0x04001D14 RID: 7444
		public const string game_09_iceblock_reappear = "event:/game/09_core/iceblock_reappear";

		// Token: 0x04001D15 RID: 7445
		public const string game_09_switch_to_cold = "event:/game/09_core/switch_to_cold";

		// Token: 0x04001D16 RID: 7446
		public const string game_09_switch_to_hot = "event:/game/09_core/switch_to_hot";

		// Token: 0x04001D17 RID: 7447
		public const string game_09_switch_shutdown = "event:/game/09_core/switch_dies";

		// Token: 0x04001D18 RID: 7448
		public const string game_09_iceball_break = "event:/game/09_core/iceball_break";

		// Token: 0x04001D19 RID: 7449
		public const string game_09_pinballbumper_hit = "event:/game/09_core/pinballbumper_hit";

		// Token: 0x04001D1A RID: 7450
		public const string game_09_risingthreat_loop = "event:/game/09_core/rising_threat";

		// Token: 0x04001D1B RID: 7451
		public const string game_10_lightning_strike = "event:/new_content/game/10_farewell/lightning_strike";

		// Token: 0x04001D1C RID: 7452
		public const string game_10_heart_door = "event:/new_content/game/10_farewell/heart_door";

		// Token: 0x04001D1D RID: 7453
		public const string game_10_bird_camera_pan_up = "event:/new_content/game/10_farewell/bird_camera_pan_up";

		// Token: 0x04001D1E RID: 7454
		public const string game_10_bird_fly_uptonext = "event:/new_content/game/10_farewell/bird_fly_uptonext";

		// Token: 0x04001D1F RID: 7455
		public const string game_10_bird_startle = "event:/new_content/game/10_farewell/bird_startle";

		// Token: 0x04001D20 RID: 7456
		public const string game_10_bird_relocate = "event:/new_content/game/10_farewell/bird_relocate";

		// Token: 0x04001D21 RID: 7457
		public const string game_10_bird_wingflap = "event:/new_content/game/10_farewell/bird_wingflap";

		// Token: 0x04001D22 RID: 7458
		public const string game_10_bird_flyuproll = "event:/new_content/game/10_farewell/bird_flyuproll";

		// Token: 0x04001D23 RID: 7459
		public const string game_10_bird_throw = "event:/new_content/game/10_farewell/bird_throw";

		// Token: 0x04001D24 RID: 7460
		public const string game_10_bird_flappyscene_entry = "event:/new_content/game/10_farewell/bird_flappyscene_entry";

		// Token: 0x04001D25 RID: 7461
		public const string game_10_bird_flappyscene = "event:/new_content/game/10_farewell/bird_flappyscene";

		// Token: 0x04001D26 RID: 7462
		public const string game_10_bird_crashscene_start = "event:/new_content/game/10_farewell/bird_crashscene_start";

		// Token: 0x04001D27 RID: 7463
		public const string game_10_bird_crashscene_twitch_1 = "event:/new_content/game/10_farewell/bird_crashscene_twitch_1";

		// Token: 0x04001D28 RID: 7464
		public const string game_10_bird_crashscene_twitch_2 = "event:/new_content/game/10_farewell/bird_crashscene_twitch_2";

		// Token: 0x04001D29 RID: 7465
		public const string game_10_bird_crashscene_twitch_3 = "event:/new_content/game/10_farewell/bird_crashscene_twitch_3";

		// Token: 0x04001D2A RID: 7466
		public const string game_10_bird_crashscene_recover = "event:/new_content/game/10_farewell/bird_crashscene_recover";

		// Token: 0x04001D2B RID: 7467
		public const string game_10_bird_crashscene_relocate = "event:/new_content/game/10_farewell/bird_crashscene_relocate";

		// Token: 0x04001D2C RID: 7468
		public const string game_10_bird_crashscene_leave = "event:/new_content/game/10_farewell/bird_crashscene_leave";

		// Token: 0x04001D2D RID: 7469
		public const string game_10_pinkdiamond_touch = "event:/new_content/game/10_farewell/pinkdiamond_touch";

		// Token: 0x04001D2E RID: 7470
		public const string game_10_pinkdiamond_return = "event:/new_content/game/10_farewell/pinkdiamond_return";

		// Token: 0x04001D2F RID: 7471
		public const string game_10_puffer_shrink = "event:/new_content/game/10_farewell/puffer_shrink";

		// Token: 0x04001D30 RID: 7472
		public const string game_10_puffer_expand = "event:/new_content/game/10_farewell/puffer_expand";

		// Token: 0x04001D31 RID: 7473
		public const string game_10_puffer_boop = "event:/new_content/game/10_farewell/puffer_boop";

		// Token: 0x04001D32 RID: 7474
		public const string game_10_puffer_splode = "event:/new_content/game/10_farewell/puffer_splode";

		// Token: 0x04001D33 RID: 7475
		public const string game_10_puffer_reform = "event:/new_content/game/10_farewell/puffer_reform";

		// Token: 0x04001D34 RID: 7476
		public const string game_10_puffer_return = "event:/new_content/game/10_farewell/puffer_return";

		// Token: 0x04001D35 RID: 7477
		public const string game_10_quake_onset = "event:/new_content/game/10_farewell/quake_onset";

		// Token: 0x04001D36 RID: 7478
		public const string game_10_quake_rockbreak = "event:/new_content/game/10_farewell/quake_rockbreak";

		// Token: 0x04001D37 RID: 7479
		public const string game_10_locked_door_appear_1 = "event:/new_content/game/10_farewell/locked_door_appear_1";

		// Token: 0x04001D38 RID: 7480
		public const string game_10_locked_door_appear_2 = "event:/new_content/game/10_farewell/locked_door_appear_2";

		// Token: 0x04001D39 RID: 7481
		public const string game_10_locked_door_appear_3 = "event:/new_content/game/10_farewell/locked_door_appear_3";

		// Token: 0x04001D3A RID: 7482
		public const string game_10_locked_door_appear_4 = "event:/new_content/game/10_farewell/locked_door_appear_4";

		// Token: 0x04001D3B RID: 7483
		public const string game_10_locked_door_appear_5 = "event:/new_content/game/10_farewell/locked_door_appear_5";

		// Token: 0x04001D3C RID: 7484
		public const string game_10_key_unlock_1 = "event:/new_content/game/10_farewell/key_unlock_1";

		// Token: 0x04001D3D RID: 7485
		public const string game_10_key_unlock_2 = "event:/new_content/game/10_farewell/key_unlock_2";

		// Token: 0x04001D3E RID: 7486
		public const string game_10_key_unlock_3 = "event:/new_content/game/10_farewell/key_unlock_3";

		// Token: 0x04001D3F RID: 7487
		public const string game_10_key_unlock_4 = "event:/new_content/game/10_farewell/key_unlock_4";

		// Token: 0x04001D40 RID: 7488
		public const string game_10_key_unlock_5 = "event:/new_content/game/10_farewell/key_unlock_5";

		// Token: 0x04001D41 RID: 7489
		public const string game_10_glider_platform_dissipate = "event:/new_content/game/10_farewell/glider_platform_dissipate";

		// Token: 0x04001D42 RID: 7490
		public const string game_10_glider_wallbounce_left = "event:/new_content/game/10_farewell/glider_wallbounce_left";

		// Token: 0x04001D43 RID: 7491
		public const string game_10_glider_wallbounce_right = "event:/new_content/game/10_farewell/glider_wallbounce_right";

		// Token: 0x04001D44 RID: 7492
		public const string game_10_glider_land = "event:/new_content/game/10_farewell/glider_land";

		// Token: 0x04001D45 RID: 7493
		public const string game_10_glider_engage = "event:/new_content/game/10_farewell/glider_engage";

		// Token: 0x04001D46 RID: 7494
		public const string game_10_glider_movement = "event:/new_content/game/10_farewell/glider_movement";

		// Token: 0x04001D47 RID: 7495
		public const string game_10_glider_emancipate = "event:/new_content/game/10_farewell/glider_emancipate";

		// Token: 0x04001D48 RID: 7496
		public const string game_10_fusebox_hit_1 = "event:/new_content/game/10_farewell/fusebox_hit_1";

		// Token: 0x04001D49 RID: 7497
		public const string game_10_fusebox_hit_2 = "event:/new_content/game/10_farewell/fusebox_hit_2";

		// Token: 0x04001D4A RID: 7498
		public const string game_10_fakeheart_get = "event:/new_content/game/10_farewell/fakeheart_get";

		// Token: 0x04001D4B RID: 7499
		public const string game_10_fakeheart_pulse = "event:/new_content/game/10_farewell/fakeheart_pulse";

		// Token: 0x04001D4C RID: 7500
		public const string game_10_fakeheart_bounce = "event:/new_content/game/10_farewell/fakeheart_bounce";

		// Token: 0x04001D4D RID: 7501
		public const string game_10_glitch_short = "event:/new_content/game/10_farewell/glitch_short";

		// Token: 0x04001D4E RID: 7502
		public const string game_10_glitch_medium = "event:/new_content/game/10_farewell/glitch_medium";

		// Token: 0x04001D4F RID: 7503
		public const string game_10_glitch_long = "event:/new_content/game/10_farewell/glitch_long";

		// Token: 0x04001D50 RID: 7504
		public const string game_10_zip_mover = "event:/new_content/game/10_farewell/zip_mover";

		// Token: 0x04001D51 RID: 7505
		public const string game_10_pico8_flag = "event:/new_content/game/10_farewell/pico8_flag";

		// Token: 0x04001D52 RID: 7506
		public const string game_10_strawberry_gold_detach = "event:/new_content/game/10_farewell/strawberry_gold_detach";

		// Token: 0x04001D53 RID: 7507
		public const string game_10_cafe_computer_on = "event:/new_content/game/10_farewell/cafe_computer_on";

		// Token: 0x04001D54 RID: 7508
		public const string game_10_cafe_computer_startupsfx = "event:/new_content/game/10_farewell/cafe_computer_startupsfx";

		// Token: 0x04001D55 RID: 7509
		public const string game_10_cafe_computer_off = "event:/new_content/game/10_farewell/cafe_computer_off";

		// Token: 0x04001D56 RID: 7510
		public const string game_10_ppt_mouseclick = "event:/new_content/game/10_farewell/ppt_mouseclick";

		// Token: 0x04001D57 RID: 7511
		public const string game_10_ppt_doubleclick = "event:/new_content/game/10_farewell/ppt_doubleclick";

		// Token: 0x04001D58 RID: 7512
		public const string game_10_ppt_cube_transition = "event:/new_content/game/10_farewell/ppt_cube_transition";

		// Token: 0x04001D59 RID: 7513
		public const string game_10_ppt_dissolve_transition = "event:/new_content/game/10_farewell/ppt_dissolve_transition";

		// Token: 0x04001D5A RID: 7514
		public const string game_10_ppt_happy_wavedashing = "event:/new_content/game/10_farewell/ppt_happy_wavedashing";

		// Token: 0x04001D5B RID: 7515
		public const string game_10_ppt_impossible = "event:/new_content/game/10_farewell/ppt_impossible";

		// Token: 0x04001D5C RID: 7516
		public const string game_10_ppt_its_easy = "event:/new_content/game/10_farewell/ppt_its_easy";

		// Token: 0x04001D5D RID: 7517
		public const string game_10_ppt_spinning_transition = "event:/new_content/game/10_farewell/ppt_spinning_transition";

		// Token: 0x04001D5E RID: 7518
		public const string game_10_ppt_wavedash_whoosh = "event:/new_content/game/10_farewell/ppt_wavedash_whoosh";

		// Token: 0x04001D5F RID: 7519
		public const string game_10_endscene_dial_theo = "event:/new_content/game/10_farewell/endscene_dial_theo";

		// Token: 0x04001D60 RID: 7520
		public const string game_10_endscene_attachment_notify = "event:/new_content/game/10_farewell/endscene_attachment_notify";

		// Token: 0x04001D61 RID: 7521
		public const string game_10_endscene_attachment_click = "event:/new_content/game/10_farewell/endscene_attachment_click";

		// Token: 0x04001D62 RID: 7522
		public const string game_10_endscene_photozoom = "event:/new_content/game/10_farewell/endscene_photo_zoom";

		// Token: 0x04001D63 RID: 7523
		public const string game_10_endscene_final_input = "event:/new_content/game/10_farewell/endscene_final_input";

		// Token: 0x04001D64 RID: 7524
		public const string game_10_timeline_bubble_to_remembered = "event:/new_content/timeline_bubble_to_remembered";

		// Token: 0x04001D65 RID: 7525
		public const string env_amb_worldmap = "event:/env/amb/worldmap";

		// Token: 0x04001D66 RID: 7526
		public const string env_amb_00_main = "event:/env/amb/00_prologue";

		// Token: 0x04001D67 RID: 7527
		public const string env_amb_01_main = "event:/env/amb/01_main";

		// Token: 0x04001D68 RID: 7528
		public const string env_amb_02_dream = "event:/env/amb/02_dream";

		// Token: 0x04001D69 RID: 7529
		public const string env_amb_02_awake = "event:/env/amb/02_awake";

		// Token: 0x04001D6A RID: 7530
		public const string env_amb_03_exterior = "event:/env/amb/03_exterior";

		// Token: 0x04001D6B RID: 7531
		public const string env_amb_03_interior = "event:/env/amb/03_interior";

		// Token: 0x04001D6C RID: 7532
		public const string env_amb_03_pico8_closeup = "event:/env/amb/03_pico8_closeup";

		// Token: 0x04001D6D RID: 7533
		public const string env_amb_04_main = "event:/env/amb/04_main";

		// Token: 0x04001D6E RID: 7534
		public const string env_amb_05_interior_main = "event:/env/amb/05_interior_main";

		// Token: 0x04001D6F RID: 7535
		public const string env_amb_05_interior_dark = "event:/env/amb/05_interior_dark";

		// Token: 0x04001D70 RID: 7536
		public const string env_amb_05_mirror_sequence = "event:/env/amb/05_mirror_sequence";

		// Token: 0x04001D71 RID: 7537
		public const string env_amb_06_lake = "event:/env/amb/06_lake";

		// Token: 0x04001D72 RID: 7538
		public const string env_amb_06_main = "event:/env/amb/06_main";

		// Token: 0x04001D73 RID: 7539
		public const string env_amb_06_prehug = "event:/env/amb/06_prehug";

		// Token: 0x04001D74 RID: 7540
		public const string env_amb_09_main = "event:/env/amb/09_main";

		// Token: 0x04001D75 RID: 7541
		public const string env_amb_10_rain = "event:/new_content/env/10_rain";

		// Token: 0x04001D76 RID: 7542
		public const string env_amb_10_electricity = "event:/new_content/env/10_electricity";

		// Token: 0x04001D77 RID: 7543
		public const string env_amb_10_endscene = "event:/new_content/env/10_endscene";

		// Token: 0x04001D78 RID: 7544
		public const string env_amb_10_rushingvoid = "event:/new_content/env/10_rushingvoid";

		// Token: 0x04001D79 RID: 7545
		public const string env_amb_10_space_underwater = "event:/new_content/env/10_space_underwater";

		// Token: 0x04001D7A RID: 7546
		public const string env_amb_10_voidspiral = "event:/new_content/env/10_voidspiral";

		// Token: 0x04001D7B RID: 7547
		public const string env_amb_10_grannyclouds = "event:/new_content/env/10_grannyclouds";

		// Token: 0x04001D7C RID: 7548
		public const string env_state_underwater = "event:/env/state/underwater";

		// Token: 0x04001D7D RID: 7549
		public const string env_loc_campfire_start = "event:/env/local/campfire_start";

		// Token: 0x04001D7E RID: 7550
		public const string env_loc_campfire_loop = "event:/env/local/campfire_loop";

		// Token: 0x04001D7F RID: 7551
		public const string env_loc_waterfall_big_main = "event:/env/local/waterfall_big_main";

		// Token: 0x04001D80 RID: 7552
		public const string env_loc_waterfall_big_in = "event:/env/local/waterfall_big_in";

		// Token: 0x04001D81 RID: 7553
		public const string env_loc_waterfall_small_main = "event:/env/local/waterfall_small_main";

		// Token: 0x04001D82 RID: 7554
		public const string env_loc_waterfall_small_in_deep = "event:/env/local/waterfall_small_in_deep";

		// Token: 0x04001D83 RID: 7555
		public const string env_loc_waterfall_small_in_shallow = "event:/env/local/waterfall_small_in_shallow";

		// Token: 0x04001D84 RID: 7556
		public const string env_loc_02_lamp = "event:/env/local/02_old_site/phone_lamp";

		// Token: 0x04001D85 RID: 7557
		public const string env_loc_03_pico8machine_loop = "event:/env/local/03_resort/pico8_machine";

		// Token: 0x04001D86 RID: 7558
		public const string env_loc_03_brokenwindow_large_loop = "event:/env/local/03_resort/broken_window_large";

		// Token: 0x04001D87 RID: 7559
		public const string env_loc_03_brokenwindow_small_loop = "event:/env/local/03_resort/broken_window_small";

		// Token: 0x04001D88 RID: 7560
		public const string env_loc_07_flag_flap = "event:/env/local/07_summit/flag_flap";

		// Token: 0x04001D89 RID: 7561
		public const string env_loc_09_conveyer_idle = "event:/env/local/09_core/conveyor_idle";

		// Token: 0x04001D8A RID: 7562
		public const string env_loc_09_lavagate_idle = "event:/env/local/09_core/lavagate_idle";

		// Token: 0x04001D8B RID: 7563
		public const string env_loc_09_fireball_idle = "event:/env/local/09_core/fireballs_idle";

		// Token: 0x04001D8C RID: 7564
		public const string env_loc_10_cafe_computer = "event:/new_content/env/local/cafe_computer";

		// Token: 0x04001D8D RID: 7565
		public const string env_loc_10_cafe_sign = "event:/new_content/env/local/cafe_sign";

		// Token: 0x04001D8E RID: 7566
		public const string env_loc_10_tutorial_static_left = "event:/new_content/env/local/tutorial_static_left";

		// Token: 0x04001D8F RID: 7567
		public const string env_loc_10_tutorial_static_right = "event:/new_content/env/local/tutorial_static_right";

		// Token: 0x04001D90 RID: 7568
		public const string env_loc_10_kevinpc = "event:/new_content/env/local/kevinpc";

		// Token: 0x04001D91 RID: 7569
		public const string state_cafe_computer_active = "event:/state/cafe_computer_active";

		// Token: 0x04001D92 RID: 7570
		public const string ui_main_postcard_ch1_in = "event:/ui/main/postcard_ch1_in";

		// Token: 0x04001D93 RID: 7571
		public const string ui_main_postcard_ch1_out = "event:/ui/main/postcard_ch1_out";

		// Token: 0x04001D94 RID: 7572
		public const string ui_main_postcard_ch2_in = "event:/ui/main/postcard_ch2_in";

		// Token: 0x04001D95 RID: 7573
		public const string ui_main_postcard_ch2_out = "event:/ui/main/postcard_ch2_out";

		// Token: 0x04001D96 RID: 7574
		public const string ui_main_postcard_ch3_in = "event:/ui/main/postcard_ch3_in";

		// Token: 0x04001D97 RID: 7575
		public const string ui_main_postcard_ch3_out = "event:/ui/main/postcard_ch3_out";

		// Token: 0x04001D98 RID: 7576
		public const string ui_main_postcard_ch4_in = "event:/ui/main/postcard_ch4_in";

		// Token: 0x04001D99 RID: 7577
		public const string ui_main_postcard_ch4_out = "event:/ui/main/postcard_ch4_out";

		// Token: 0x04001D9A RID: 7578
		public const string ui_main_postcard_ch5_in = "event:/ui/main/postcard_ch5_in";

		// Token: 0x04001D9B RID: 7579
		public const string ui_main_postcard_ch5_out = "event:/ui/main/postcard_ch5_out";

		// Token: 0x04001D9C RID: 7580
		public const string ui_main_postcard_ch6_in = "event:/ui/main/postcard_ch6_in";

		// Token: 0x04001D9D RID: 7581
		public const string ui_main_postcard_ch6_out = "event:/ui/main/postcard_ch6_out";

		// Token: 0x04001D9E RID: 7582
		public const string ui_main_postcard_csides_in = "event:/ui/main/postcard_csides_in";

		// Token: 0x04001D9F RID: 7583
		public const string ui_main_postcard_csides_out = "event:/ui/main/postcard_csides_out";

		// Token: 0x04001DA0 RID: 7584
		public const string ui_main_postcard_variants_in = "event:/new_content/ui/postcard_variants_in";

		// Token: 0x04001DA1 RID: 7585
		public const string ui_main_postcard_variants_out = "event:/new_content/ui/postcard_variants_out";

		// Token: 0x04001DA2 RID: 7586
		public const string ui_main_title_firstinput = "event:/ui/main/title_firstinput";

		// Token: 0x04001DA3 RID: 7587
		public const string ui_main_roll_down = "event:/ui/main/rollover_down";

		// Token: 0x04001DA4 RID: 7588
		public const string ui_main_roll_up = "event:/ui/main/rollover_up";

		// Token: 0x04001DA5 RID: 7589
		public const string ui_main_button_select = "event:/ui/main/button_select";

		// Token: 0x04001DA6 RID: 7590
		public const string ui_main_button_back = "event:/ui/main/button_back";

		// Token: 0x04001DA7 RID: 7591
		public const string ui_main_button_climb = "event:/ui/main/button_climb";

		// Token: 0x04001DA8 RID: 7592
		public const string ui_main_button_toggle_on = "event:/ui/main/button_toggle_on";

		// Token: 0x04001DA9 RID: 7593
		public const string ui_main_button_toggle_off = "event:/ui/main/button_toggle_off";

		// Token: 0x04001DAA RID: 7594
		public const string ui_main_button_invalid = "event:/ui/main/button_invalid";

		// Token: 0x04001DAB RID: 7595
		public const string ui_main_button_lowkey = "event:/ui/main/button_lowkey";

		// Token: 0x04001DAC RID: 7596
		public const string ui_main_whoosh_large_in = "event:/ui/main/whoosh_large_in";

		// Token: 0x04001DAD RID: 7597
		public const string ui_main_whoosh_large_out = "event:/ui/main/whoosh_large_out";

		// Token: 0x04001DAE RID: 7598
		public const string ui_main_whoosh_list_in = "event:/ui/main/whoosh_list_in";

		// Token: 0x04001DAF RID: 7599
		public const string ui_main_whoosh_list_out = "event:/ui/main/whoosh_list_out";

		// Token: 0x04001DB0 RID: 7600
		public const string ui_main_whoosh_savefile_in = "event:/ui/main/whoosh_savefile_in";

		// Token: 0x04001DB1 RID: 7601
		public const string ui_main_whoosh_savefile_out = "event:/ui/main/whoosh_savefile_out";

		// Token: 0x04001DB2 RID: 7602
		public const string ui_main_savefile_roll_down = "event:/ui/main/savefile_rollover_down";

		// Token: 0x04001DB3 RID: 7603
		public const string ui_main_savefile_roll_up = "event:/ui/main/savefile_rollover_up";

		// Token: 0x04001DB4 RID: 7604
		public const string ui_main_savefile_roll_first = "event:/ui/main/savefile_rollover_first";

		// Token: 0x04001DB5 RID: 7605
		public const string ui_main_savefile_rename_start = "event:/ui/main/savefile_rename_start";

		// Token: 0x04001DB6 RID: 7606
		public const string ui_main_savefile_delete = "event:/ui/main/savefile_delete";

		// Token: 0x04001DB7 RID: 7607
		public const string ui_main_savefile_begin = "event:/ui/main/savefile_begin";

		// Token: 0x04001DB8 RID: 7608
		public const string ui_main_message_confirm = "event:/ui/main/message_confirm";

		// Token: 0x04001DB9 RID: 7609
		public const string ui_main_rename_entry_roll = "event:/ui/main/rename_entry_rollover";

		// Token: 0x04001DBA RID: 7610
		public const string ui_main_rename_entry_char = "event:/ui/main/rename_entry_char";

		// Token: 0x04001DBB RID: 7611
		public const string ui_main_rename_entry_space = "event:/ui/main/rename_entry_space";

		// Token: 0x04001DBC RID: 7612
		public const string ui_main_rename_entry_backspace = "event:/ui/main/rename_entry_backspace";

		// Token: 0x04001DBD RID: 7613
		public const string ui_main_rename_entry_accept = "event:/ui/main/rename_entry_accept";

		// Token: 0x04001DBE RID: 7614
		public const string ui_main_rename_entry_accept_locked = "event:/new_content/ui/rename_entry_accept_locked";

		// Token: 0x04001DBF RID: 7615
		public const string ui_main_assist_button_info = "event:/ui/main/assist_button_info";

		// Token: 0x04001DC0 RID: 7616
		public const string ui_main_assist_info_whistle = "event:/ui/main/assist_info_whistle";

		// Token: 0x04001DC1 RID: 7617
		public const string ui_main_assist_button_yes = "event:/ui/main/assist_button_yes";

		// Token: 0x04001DC2 RID: 7618
		public const string ui_main_assist_button_no = "event:/ui/main/assist_button_no";

		// Token: 0x04001DC3 RID: 7619
		public const string ui_main_bside_intro_text = "event:/ui/main/bside_intro_text";

		// Token: 0x04001DC4 RID: 7620
		public const string ui_game_pause = "event:/ui/game/pause";

		// Token: 0x04001DC5 RID: 7621
		public const string ui_game_unpause = "event:/ui/game/unpause";

		// Token: 0x04001DC6 RID: 7622
		public const string ui_game_tutorialnote_flip_back = "event:/ui/game/tutorial_note_flip_back";

		// Token: 0x04001DC7 RID: 7623
		public const string ui_game_tutorialnote_flip_front = "event:/ui/game/tutorial_note_flip_front";

		// Token: 0x04001DC8 RID: 7624
		public const string ui_game_hotspot_main_in = "event:/ui/game/hotspot_main_in";

		// Token: 0x04001DC9 RID: 7625
		public const string ui_game_hotspot_main_out = "event:/ui/game/hotspot_main_out";

		// Token: 0x04001DCA RID: 7626
		public const string ui_game_hotspot_note_in = "event:/ui/game/hotspot_note_in";

		// Token: 0x04001DCB RID: 7627
		public const string ui_game_hotspot_note_out = "event:/ui/game/hotspot_note_out";

		// Token: 0x04001DCC RID: 7628
		public const string ui_game_general_text_loop = "event:/ui/game/general_text_loop";

		// Token: 0x04001DCD RID: 7629
		public const string ui_game_memorial_text_in = "event:/ui/game/memorial_text_in";

		// Token: 0x04001DCE RID: 7630
		public const string ui_game_memorial_text_loop = "event:/ui/game/memorial_text_loop";

		// Token: 0x04001DCF RID: 7631
		public const string ui_game_memorial_text_out = "event:/ui/game/memorial_text_out";

		// Token: 0x04001DD0 RID: 7632
		public const string ui_game_memorialdream_text_in = "event:/ui/game/memorial_dream_text_in";

		// Token: 0x04001DD1 RID: 7633
		public const string ui_game_memorialdream_text_loop = "event:/ui/game/memorial_dream_text_loop";

		// Token: 0x04001DD2 RID: 7634
		public const string ui_game_memorialdream_text_out = "event:/ui/game/memorial_dream_text_out";

		// Token: 0x04001DD3 RID: 7635
		public const string ui_game_memorialdream_loop = "event:/ui/game/memorial_dream_loop";

		// Token: 0x04001DD4 RID: 7636
		public const string ui_game_lookout_on = "event:/ui/game/lookout_on";

		// Token: 0x04001DD5 RID: 7637
		public const string ui_game_lookout_off = "event:/ui/game/lookout_off";

		// Token: 0x04001DD6 RID: 7638
		public const string ui_game_chatoptions_roll_up = "event:/ui/game/chatoptions_roll_up";

		// Token: 0x04001DD7 RID: 7639
		public const string ui_game_chatoptions_roll_down = "event:/ui/game/chatoptions_roll_down";

		// Token: 0x04001DD8 RID: 7640
		public const string ui_game_chatoptions_appear = "event:/ui/game/chatoptions_appear";

		// Token: 0x04001DD9 RID: 7641
		public const string ui_game_chatoptions_select = "event:/ui/game/chatoptions_select";

		// Token: 0x04001DDA RID: 7642
		public const string ui_game_textbox_madeline_in = "event:/ui/game/textbox_madeline_in";

		// Token: 0x04001DDB RID: 7643
		public const string ui_game_textbox_madeline_out = "event:/ui/game/textbox_madeline_out";

		// Token: 0x04001DDC RID: 7644
		public const string ui_game_textbox_other_in = "event:/ui/game/textbox_other_in";

		// Token: 0x04001DDD RID: 7645
		public const string ui_game_textbox_other_out = "event:/ui/game/textbox_other_out";

		// Token: 0x04001DDE RID: 7646
		public const string ui_game_textadvance_madeline = "event:/ui/game/textadvance_madeline";

		// Token: 0x04001DDF RID: 7647
		public const string ui_game_textadvance_other = "event:/ui/game/textadvance_other";

		// Token: 0x04001DE0 RID: 7648
		public const string ui_game_increment_strawberry = "event:/ui/game/increment_strawberry";

		// Token: 0x04001DE1 RID: 7649
		public const string ui_game_increment_dashcount = "event:/ui/game/increment_dashcount";

		// Token: 0x04001DE2 RID: 7650
		public const string ui_world_icon_roll_right = "event:/ui/world_map/icon/roll_right";

		// Token: 0x04001DE3 RID: 7651
		public const string ui_world_icon_roll_left = "event:/ui/world_map/icon/roll_left";

		// Token: 0x04001DE4 RID: 7652
		public const string ui_world_icon_select = "event:/ui/world_map/icon/select";

		// Token: 0x04001DE5 RID: 7653
		public const string ui_world_icon_flip_right = "event:/ui/world_map/icon/flip_right";

		// Token: 0x04001DE6 RID: 7654
		public const string ui_world_icon_flip_left = "event:/ui/world_map/icon/flip_left";

		// Token: 0x04001DE7 RID: 7655
		public const string ui_world_icon_assistskip = "event:/ui/world_map/icon/assist_skip";

		// Token: 0x04001DE8 RID: 7656
		public const string ui_world_whoosh_400ms_forward = "event:/ui/world_map/whoosh/400ms_forward";

		// Token: 0x04001DE9 RID: 7657
		public const string ui_world_whoosh_400ms_back = "event:/ui/world_map/whoosh/400ms_back";

		// Token: 0x04001DEA RID: 7658
		public const string ui_world_whoosh_600ms_forward = "event:/ui/world_map/whoosh/600ms_forward";

		// Token: 0x04001DEB RID: 7659
		public const string ui_world_whoosh_600ms_back = "event:/ui/world_map/whoosh/600ms_back";

		// Token: 0x04001DEC RID: 7660
		public const string ui_world_whoosh_700ms_forward = "event:/ui/world_map/whoosh/700ms_forward";

		// Token: 0x04001DED RID: 7661
		public const string ui_world_whoosh_700ms_back = "event:/ui/world_map/whoosh/700ms_back";

		// Token: 0x04001DEE RID: 7662
		public const string ui_world_whoosh_1000ms_forward = "event:/ui/world_map/whoosh/1000ms_forward";

		// Token: 0x04001DEF RID: 7663
		public const string ui_world_whoosh_1000ms_back = "event:/ui/world_map/whoosh/1000ms_back";

		// Token: 0x04001DF0 RID: 7664
		public const string ui_world_whoosh_900ms_forward = "event:/ui/world_map/whoosh/900ms_forward";

		// Token: 0x04001DF1 RID: 7665
		public const string ui_world_whoosh_900ms_back = "event:/ui/world_map/whoosh/900ms_back";

		// Token: 0x04001DF2 RID: 7666
		public const string ui_world_chapter_back = "event:/ui/world_map/chapter/back";

		// Token: 0x04001DF3 RID: 7667
		public const string ui_world_chapter_pane_expand = "event:/ui/world_map/chapter/pane_expand";

		// Token: 0x04001DF4 RID: 7668
		public const string ui_world_chapter_pane_contract = "event:/ui/world_map/chapter/pane_contract";

		// Token: 0x04001DF5 RID: 7669
		public const string ui_world_chapter_tab_roll_right = "event:/ui/world_map/chapter/tab_roll_right";

		// Token: 0x04001DF6 RID: 7670
		public const string ui_world_chapter_tab_roll_left = "event:/ui/world_map/chapter/tab_roll_left";

		// Token: 0x04001DF7 RID: 7671
		public const string ui_world_chapter_level_select = "event:/ui/world_map/chapter/level_select";

		// Token: 0x04001DF8 RID: 7672
		public const string ui_world_chapter_checkpoint_back = "event:/ui/world_map/chapter/checkpoint_back";

		// Token: 0x04001DF9 RID: 7673
		public const string ui_world_chapter_checkpoint_photo_add = "event:/ui/world_map/chapter/checkpoint_photo_add";

		// Token: 0x04001DFA RID: 7674
		public const string ui_world_chapter_checkpoint_photo_remove = "event:/ui/world_map/chapter/checkpoint_photo_remove";

		// Token: 0x04001DFB RID: 7675
		public const string ui_world_chapter_checkpoint_start = "event:/ui/world_map/chapter/checkpoint_start";

		// Token: 0x04001DFC RID: 7676
		public const string ui_world_journal_select = "event:/ui/world_map/journal/select";

		// Token: 0x04001DFD RID: 7677
		public const string ui_world_journal_back = "event:/ui/world_map/journal/back";

		// Token: 0x04001DFE RID: 7678
		public const string ui_world_journal_page_cover_forward = "event:/ui/world_map/journal/page_cover_forward";

		// Token: 0x04001DFF RID: 7679
		public const string ui_world_journal_page_cover_back = "event:/ui/world_map/journal/page_cover_back";

		// Token: 0x04001E00 RID: 7680
		public const string ui_world_journal_page_main_forward = "event:/ui/world_map/journal/page_main_forward";

		// Token: 0x04001E01 RID: 7681
		public const string ui_world_journal_page_main_back = "event:/ui/world_map/journal/page_main_back";

		// Token: 0x04001E02 RID: 7682
		public const string ui_world_journal_heart_roll = "event:/ui/world_map/journal/heart_roll";

		// Token: 0x04001E03 RID: 7683
		public const string ui_world_journal_heart_grab = "event:/ui/world_map/journal/heart_grab";

		// Token: 0x04001E04 RID: 7684
		public const string ui_world_journal_heart_release = "event:/ui/world_map/journal/heart_release";

		// Token: 0x04001E05 RID: 7685
		public const string ui_world_journal_heart_shift_up = "event:/ui/world_map/journal/heart_shift_up";

		// Token: 0x04001E06 RID: 7686
		public const string ui_world_journal_heart_shift_down = "event:/ui/world_map/journal/heart_shift_down";

		// Token: 0x04001E07 RID: 7687
		public const string ui_postgame_crystalheart = "event:/ui/postgame/crystal_heart";

		// Token: 0x04001E08 RID: 7688
		public const string ui_postgame_strawberry_count = "event:/ui/postgame/strawberry_count";

		// Token: 0x04001E09 RID: 7689
		public const string ui_postgame_goldberry_count = "event:/ui/postgame/goldberry_count";

		// Token: 0x04001E0A RID: 7690
		public const string ui_postgame_strawberry_total = "event:/ui/postgame/strawberry_total";

		// Token: 0x04001E0B RID: 7691
		public const string ui_postgame_strawberry_total_all = "event:/ui/postgame/strawberry_total_all";

		// Token: 0x04001E0C RID: 7692
		public const string ui_postgame_death_appear = "event:/ui/postgame/death_appear";

		// Token: 0x04001E0D RID: 7693
		public const string ui_postgame_death_count = "event:/ui/postgame/death_count";

		// Token: 0x04001E0E RID: 7694
		public const string ui_postgame_death_final = "event:/ui/postgame/death_final";

		// Token: 0x04001E0F RID: 7695
		public const string ui_postgame_unlock_newchapter = "event:/ui/postgame/unlock_newchapter";

		// Token: 0x04001E10 RID: 7696
		public const string ui_postgame_unlock_newchapter_icon = "event:/ui/postgame/unlock_newchapter_icon";

		// Token: 0x04001E11 RID: 7697
		public const string ui_postgame_unlock_bside = "event:/ui/postgame/unlock_bside";

		// Token: 0x04001E12 RID: 7698
		public const string ui_postgame_skip_all = "event:/new_content/ui/skip_all";

		// Token: 0x04001E13 RID: 7699
		private static Dictionary<string, string> byHandle = new Dictionary<string, string>();

		// Token: 0x04001E14 RID: 7700
		public static Dictionary<string, string> MadelineToBadelineSound = new Dictionary<string, string>();
	}
}
