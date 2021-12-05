using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Celeste.Pico8
{
	// Token: 0x02000395 RID: 917
	public class Classic
	{
		// Token: 0x06001DCE RID: 7630 RVA: 0x000D0D28 File Offset: 0x000CEF28
		public void Init(Emulator emulator)
		{
			this.E = emulator;
			this.room = new Point(0, 0);
			this.objects = new List<Classic.ClassicObject>();
			this.freeze = 0;
			this.will_restart = false;
			this.delay_restart = 0;
			this.got_fruit = new HashSet<int>();
			this.has_dashed = false;
			this.sfx_timer = 0;
			this.has_key = false;
			this.pause_player = false;
			this.flash_bg = false;
			this.music_timer = 0;
			this.new_bg = false;
			this.room_just_loaded = false;
			this.frames = 0;
			this.seconds = 0;
			this.minutes = 0;
			this.deaths = 0;
			this.max_djump = 1;
			this.start_game = false;
			this.start_game_flash = 0;
			this.clouds = new List<Classic.Cloud>();
			for (int i = 0; i <= 16; i++)
			{
				this.clouds.Add(new Classic.Cloud
				{
					x = this.E.rnd(128f),
					y = this.E.rnd(128f),
					spd = 1f + this.E.rnd(4f),
					w = 32f + this.E.rnd(32f)
				});
			}
			this.particles = new List<Classic.Particle>();
			for (int j = 0; j <= 32; j++)
			{
				this.particles.Add(new Classic.Particle
				{
					x = this.E.rnd(128f),
					y = this.E.rnd(128f),
					s = this.E.flr(this.E.rnd(5f) / 4f),
					spd = 0.25f + this.E.rnd(5f),
					off = this.E.rnd(1f),
					c = 6 + this.E.flr(0.5f + this.E.rnd(1f))
				});
			}
			this.dead_particles = new List<Classic.DeadParticle>();
			this.title_screen();
		}

		// Token: 0x06001DCF RID: 7631 RVA: 0x000D0F58 File Offset: 0x000CF158
		private void title_screen()
		{
			this.got_fruit = new HashSet<int>();
			this.frames = 0;
			this.deaths = 0;
			this.max_djump = 1;
			this.start_game = false;
			this.start_game_flash = 0;
			this.E.music(40, 0, 7);
			this.load_room(7, 3);
		}

		// Token: 0x06001DD0 RID: 7632 RVA: 0x000D0FAA File Offset: 0x000CF1AA
		private void begin_game()
		{
			this.frames = 0;
			this.seconds = 0;
			this.minutes = 0;
			this.music_timer = 0;
			this.start_game = false;
			this.E.music(0, 0, 7);
			this.load_room(0, 0);
		}

		// Token: 0x06001DD1 RID: 7633 RVA: 0x000D0FE5 File Offset: 0x000CF1E5
		private int level_index()
		{
			return this.room.X % 8 + this.room.Y * 8;
		}

		// Token: 0x06001DD2 RID: 7634 RVA: 0x000D1002 File Offset: 0x000CF202
		private bool is_title()
		{
			return this.level_index() == 31;
		}

		// Token: 0x06001DD3 RID: 7635 RVA: 0x000D100E File Offset: 0x000CF20E
		private void psfx(int num)
		{
			if (this.sfx_timer <= 0)
			{
				this.E.sfx(num);
			}
		}

		// Token: 0x06001DD4 RID: 7636 RVA: 0x000D1028 File Offset: 0x000CF228
		private void draw_player(Classic.ClassicObject obj, int djump)
		{
			int num = 0;
			if (djump == 2)
			{
				if (this.E.flr((float)(this.frames / 3 % 2)) == 0)
				{
					num = 160;
				}
				else
				{
					num = 144;
				}
			}
			else if (djump == 0)
			{
				num = 128;
			}
			this.E.spr(obj.spr + (float)num, obj.x, obj.y, 1, 1, obj.flipX, obj.flipY);
		}

		// Token: 0x06001DD5 RID: 7637 RVA: 0x000D109A File Offset: 0x000CF29A
		private void break_spring(Classic.spring obj)
		{
			obj.hide_in = 15;
		}

		// Token: 0x06001DD6 RID: 7638 RVA: 0x000D10A4 File Offset: 0x000CF2A4
		private void break_fall_floor(Classic.fall_floor obj)
		{
			if (obj.state == 0)
			{
				this.psfx(15);
				obj.state = 1;
				obj.delay = 15;
				this.init_object<Classic.smoke>(new Classic.smoke(), obj.x, obj.y, null);
				Classic.spring spring = obj.collide<Classic.spring>(0, -1);
				if (spring != null)
				{
					this.break_spring(spring);
				}
			}
		}

		// Token: 0x06001DD7 RID: 7639 RVA: 0x000D1104 File Offset: 0x000CF304
		private T init_object<T>(T obj, float x, float y, int? tile = null) where T : Classic.ClassicObject
		{
			this.objects.Add(obj);
			if (tile != null)
			{
				obj.spr = (float)tile.Value;
			}
			obj.x = (float)((int)x);
			obj.y = (float)((int)y);
			obj.init(this, this.E);
			return obj;
		}

		// Token: 0x06001DD8 RID: 7640 RVA: 0x000D1170 File Offset: 0x000CF370
		private void destroy_object(Classic.ClassicObject obj)
		{
			int num = this.objects.IndexOf(obj);
			if (num >= 0)
			{
				this.objects[num] = null;
			}
		}

		// Token: 0x06001DD9 RID: 7641 RVA: 0x000D119C File Offset: 0x000CF39C
		private void kill_player(Classic.player obj)
		{
			this.sfx_timer = 12;
			this.E.sfx(0);
			this.deaths++;
			this.shake = 10;
			this.destroy_object(obj);
			Stats.Increment(Stat.PICO_DEATHS, 1);
			this.dead_particles.Clear();
			for (int i = 0; i <= 7; i++)
			{
				float num = (float)i / 8f;
				this.dead_particles.Add(new Classic.DeadParticle
				{
					x = obj.x + 4f,
					y = obj.y + 4f,
					t = 10,
					spd = new Vector2(this.E.cos(num) * 3f, this.E.sin(num + 0.5f) * 3f)
				});
			}
			this.restart_room();
		}

		// Token: 0x06001DDA RID: 7642 RVA: 0x000D127B File Offset: 0x000CF47B
		private void restart_room()
		{
			this.will_restart = true;
			this.delay_restart = 15;
		}

		// Token: 0x06001DDB RID: 7643 RVA: 0x000D128C File Offset: 0x000CF48C
		private void next_room()
		{
			if (this.room.X == 2 && this.room.Y == 1)
			{
				this.E.music(30, 500, 7);
			}
			else if (this.room.X == 3 && this.room.Y == 1)
			{
				this.E.music(20, 500, 7);
			}
			else if (this.room.X == 4 && this.room.Y == 2)
			{
				this.E.music(30, 500, 7);
			}
			else if (this.room.X == 5 && this.room.Y == 3)
			{
				this.E.music(30, 500, 7);
			}
			if (this.room.X == 7)
			{
				this.load_room(0, this.room.Y + 1);
				return;
			}
			this.load_room(this.room.X + 1, this.room.Y);
		}

		// Token: 0x06001DDC RID: 7644 RVA: 0x000D13A0 File Offset: 0x000CF5A0
		public void load_room(int x, int y)
		{
			this.room_just_loaded = true;
			this.has_dashed = false;
			this.has_key = false;
			for (int i = 0; i < this.objects.Count; i++)
			{
				this.objects[i] = null;
			}
			this.room.X = x;
			this.room.Y = y;
			for (int j = 0; j <= 15; j++)
			{
				for (int k = 0; k <= 15; k++)
				{
					int num = this.E.mget(this.room.X * 16 + j, this.room.Y * 16 + k);
					if (num == 11)
					{
						this.init_object<Classic.platform>(new Classic.platform(), (float)(j * 8), (float)(k * 8), null).dir = -1f;
					}
					else if (num == 12)
					{
						this.init_object<Classic.platform>(new Classic.platform(), (float)(j * 8), (float)(k * 8), null).dir = 1f;
					}
					else
					{
						Classic.ClassicObject classicObject = null;
						if (num == 1)
						{
							classicObject = new Classic.player_spawn();
						}
						else if (num == 18)
						{
							classicObject = new Classic.spring();
						}
						else if (num == 22)
						{
							classicObject = new Classic.balloon();
						}
						else if (num == 23)
						{
							classicObject = new Classic.fall_floor();
						}
						else if (num == 86)
						{
							classicObject = new Classic.message();
						}
						else if (num == 96)
						{
							classicObject = new Classic.big_chest();
						}
						else if (num == 118)
						{
							classicObject = new Classic.flag();
						}
						else if (!this.got_fruit.Contains(1 + this.level_index()))
						{
							if (num == 26)
							{
								classicObject = new Classic.fruit();
							}
							else if (num == 28)
							{
								classicObject = new Classic.fly_fruit();
							}
							else if (num == 64)
							{
								classicObject = new Classic.fake_wall();
							}
							else if (num == 8)
							{
								classicObject = new Classic.key();
							}
							else if (num == 20)
							{
								classicObject = new Classic.chest();
							}
						}
						if (classicObject != null)
						{
							this.init_object<Classic.ClassicObject>(classicObject, (float)(j * 8), (float)(k * 8), new int?(num));
						}
					}
				}
			}
			if (!this.is_title())
			{
				this.init_object<Classic.room_title>(new Classic.room_title(), 0f, 0f, null);
			}
		}

		// Token: 0x06001DDD RID: 7645 RVA: 0x000D15BC File Offset: 0x000CF7BC
		public void Update()
		{
			this.frames = (this.frames + 1) % 30;
			if (this.frames == 0 && this.level_index() < 30)
			{
				this.seconds = (this.seconds + 1) % 60;
				if (this.seconds == 0)
				{
					this.minutes++;
				}
			}
			if (this.music_timer > 0)
			{
				this.music_timer--;
				if (this.music_timer <= 0)
				{
					this.E.music(10, 0, 7);
				}
			}
			if (this.sfx_timer > 0)
			{
				this.sfx_timer--;
			}
			if (this.freeze > 0)
			{
				this.freeze--;
				return;
			}
			if (this.shake > 0 && Settings.Instance.ScreenShake != ScreenshakeAmount.Off)
			{
				this.shake--;
				this.E.camera();
				if (this.shake > 0)
				{
					if (Settings.Instance.ScreenShake == ScreenshakeAmount.On)
					{
						this.E.camera(-2f + this.E.rnd(5f), -2f + this.E.rnd(5f));
					}
					else
					{
						this.E.camera(-1f + this.E.rnd(3f), -1f + this.E.rnd(3f));
					}
				}
			}
			if (this.will_restart && this.delay_restart > 0)
			{
				this.delay_restart--;
				if (this.delay_restart <= 0)
				{
					this.will_restart = true;
					this.load_room(this.room.X, this.room.Y);
				}
			}
			this.room_just_loaded = false;
			int num = 0;
			IL_233:
			while (num != -1)
			{
				int i = num;
				num = -1;
				while (i < this.objects.Count)
				{
					Classic.ClassicObject classicObject = this.objects[i];
					if (classicObject != null)
					{
						classicObject.move(classicObject.spd.X, classicObject.spd.Y);
						classicObject.update();
						if (this.room_just_loaded)
						{
							this.room_just_loaded = false;
							num = i;
							IL_224:
							while (this.objects.IndexOf(null) >= 0)
							{
								this.objects.Remove(null);
							}
							goto IL_233;
						}
					}
					i++;
				}
				goto IL_224;
			}
			if (this.is_title())
			{
				if (!this.start_game && (this.E.btn(this.k_jump) || this.E.btn(this.k_dash)))
				{
					this.E.music(-1, 0, 0);
					this.start_game_flash = 50;
					this.start_game = true;
					this.E.sfx(38);
				}
				if (this.start_game)
				{
					this.start_game_flash--;
					if (this.start_game_flash <= -30)
					{
						this.begin_game();
					}
				}
			}
		}

		// Token: 0x06001DDE RID: 7646 RVA: 0x000D1888 File Offset: 0x000CFA88
		public void Draw()
		{
			this.E.pal();
			if (this.start_game)
			{
				int num = 10;
				if (this.start_game_flash > 10)
				{
					if (this.frames % 10 < 5)
					{
						num = 7;
					}
				}
				else if (this.start_game_flash > 5)
				{
					num = 2;
				}
				else if (this.start_game_flash > 0)
				{
					num = 1;
				}
				else
				{
					num = 0;
				}
				if (num < 10)
				{
					this.E.pal(6, num);
					this.E.pal(12, num);
					this.E.pal(13, num);
					this.E.pal(5, num);
					this.E.pal(1, num);
					this.E.pal(7, num);
				}
			}
			int num2 = 0;
			if (this.flash_bg)
			{
				num2 = this.frames / 5;
			}
			else if (this.new_bg)
			{
				num2 = 2;
			}
			this.E.rectfill(0f, 0f, 128f, 128f, (float)num2);
			if (!this.is_title())
			{
				foreach (Classic.Cloud cloud in this.clouds)
				{
					cloud.x += cloud.spd;
					this.E.rectfill(cloud.x, cloud.y, cloud.x + cloud.w, cloud.y + 4f + (1f - cloud.w / 64f) * 12f, (float)(this.new_bg ? 14 : 1));
					if (cloud.x > 128f)
					{
						cloud.x = -cloud.w;
						cloud.y = this.E.rnd(120f);
					}
				}
			}
			this.E.map(this.room.X * 16, this.room.Y * 16, 0, 0, 16, 16, 2);
			for (int i = 0; i < this.objects.Count; i++)
			{
				Classic.ClassicObject classicObject = this.objects[i];
				if (classicObject != null && (classicObject is Classic.platform || classicObject is Classic.big_chest))
				{
					this.draw_object(classicObject);
				}
			}
			int tx = this.is_title() ? -4 : 0;
			this.E.map(this.room.X * 16, this.room.Y * 16, tx, 0, 16, 16, 1);
			for (int j = 0; j < this.objects.Count; j++)
			{
				Classic.ClassicObject classicObject2 = this.objects[j];
				if (classicObject2 != null && !(classicObject2 is Classic.platform) && !(classicObject2 is Classic.big_chest))
				{
					this.draw_object(classicObject2);
				}
			}
			this.E.map(this.room.X * 16, this.room.Y * 16, 0, 0, 16, 16, 3);
			foreach (Classic.Particle particle in this.particles)
			{
				particle.x += particle.spd;
				particle.y += this.E.sin(particle.off);
				particle.off += this.E.min(0.05f, particle.spd / 32f);
				this.E.rectfill(particle.x, particle.y, particle.x + (float)particle.s, particle.y + (float)particle.s, (float)particle.c);
				if (particle.x > 132f)
				{
					particle.x = -4f;
					particle.y = this.E.rnd(128f);
				}
			}
			for (int k = this.dead_particles.Count - 1; k >= 0; k--)
			{
				Classic.DeadParticle deadParticle = this.dead_particles[k];
				deadParticle.x += deadParticle.spd.X;
				deadParticle.y += deadParticle.spd.Y;
				deadParticle.t--;
				if (deadParticle.t <= 0)
				{
					this.dead_particles.RemoveAt(k);
				}
				this.E.rectfill(deadParticle.x - (float)(deadParticle.t / 5), deadParticle.y - (float)(deadParticle.t / 5), deadParticle.x + (float)(deadParticle.t / 5), deadParticle.y + (float)(deadParticle.t / 5), (float)(14 + deadParticle.t % 2));
			}
			this.E.rectfill(-5f, -5f, -1f, 133f, 0f);
			this.E.rectfill(-5f, -5f, 133f, -1f, 0f);
			this.E.rectfill(-5f, 128f, 133f, 133f, 0f);
			this.E.rectfill(128f, -5f, 133f, 133f, 0f);
			if (this.is_title())
			{
				this.E.print("press button", 42f, 96f, 5f);
			}
			if (this.level_index() == 30)
			{
				Classic.ClassicObject classicObject3 = null;
				foreach (Classic.ClassicObject classicObject4 in this.objects)
				{
					if (classicObject4 is Classic.player)
					{
						classicObject3 = classicObject4;
						break;
					}
				}
				if (classicObject3 != null)
				{
					float num3 = this.E.min(24f, 40f - this.E.abs(classicObject3.x + 4f - 64f));
					this.E.rectfill(0f, 0f, num3, 128f, 0f);
					this.E.rectfill(128f - num3, 0f, 128f, 128f, 0f);
				}
			}
		}

		// Token: 0x06001DDF RID: 7647 RVA: 0x000D1F2C File Offset: 0x000D012C
		private void draw_object(Classic.ClassicObject obj)
		{
			obj.draw();
		}

		// Token: 0x06001DE0 RID: 7648 RVA: 0x000D1F34 File Offset: 0x000D0134
		private void draw_time(int x, int y)
		{
			int num = this.seconds;
			int num2 = this.minutes % 60;
			int num3 = this.E.flr((float)(this.minutes / 60));
			this.E.rectfill((float)x, (float)y, (float)(x + 32), (float)(y + 6), 0f);
			this.E.print(string.Concat(new object[]
			{
				(num3 < 10) ? "0" : "",
				num3,
				":",
				(num2 < 10) ? "0" : "",
				num2,
				":",
				(num < 10) ? "0" : "",
				num
			}), (float)(x + 1), (float)(y + 1), 7f);
		}

		// Token: 0x06001DE1 RID: 7649 RVA: 0x000D200F File Offset: 0x000D020F
		private float clamp(float val, float a, float b)
		{
			return this.E.max(a, this.E.min(b, val));
		}

		// Token: 0x06001DE2 RID: 7650 RVA: 0x000D202A File Offset: 0x000D022A
		private float appr(float val, float target, float amount)
		{
			if (val <= target)
			{
				return this.E.min(val + amount, target);
			}
			return this.E.max(val - amount, target);
		}

		// Token: 0x06001DE3 RID: 7651 RVA: 0x000D204F File Offset: 0x000D024F
		private int sign(float v)
		{
			if (v > 0f)
			{
				return 1;
			}
			if (v >= 0f)
			{
				return 0;
			}
			return -1;
		}

		// Token: 0x06001DE4 RID: 7652 RVA: 0x000D2066 File Offset: 0x000D0266
		private bool maybe()
		{
			return this.E.rnd(1f) < 0.5f;
		}

		// Token: 0x06001DE5 RID: 7653 RVA: 0x000D207F File Offset: 0x000D027F
		private bool solid_at(float x, float y, float w, float h)
		{
			return this.tile_flag_at(x, y, w, h, 0);
		}

		// Token: 0x06001DE6 RID: 7654 RVA: 0x000D208D File Offset: 0x000D028D
		private bool ice_at(float x, float y, float w, float h)
		{
			return this.tile_flag_at(x, y, w, h, 4);
		}

		// Token: 0x06001DE7 RID: 7655 RVA: 0x000D209C File Offset: 0x000D029C
		private bool tile_flag_at(float x, float y, float w, float h, int flag)
		{
			int num = (int)this.E.max(0f, (float)this.E.flr(x / 8f));
			while ((float)num <= this.E.min(15f, (x + w - 1f) / 8f))
			{
				int num2 = (int)this.E.max(0f, (float)this.E.flr(y / 8f));
				while ((float)num2 <= this.E.min(15f, (y + h - 1f) / 8f))
				{
					if (this.E.fget(this.tile_at(num, num2), flag))
					{
						return true;
					}
					num2++;
				}
				num++;
			}
			return false;
		}

		// Token: 0x06001DE8 RID: 7656 RVA: 0x000D2163 File Offset: 0x000D0363
		private int tile_at(int x, int y)
		{
			return this.E.mget(this.room.X * 16 + x, this.room.Y * 16 + y);
		}

		// Token: 0x06001DE9 RID: 7657 RVA: 0x000D2190 File Offset: 0x000D0390
		private bool spikes_at(float x, float y, int w, int h, float xspd, float yspd)
		{
			int num = (int)this.E.max(0f, (float)this.E.flr(x / 8f));
			while ((float)num <= this.E.min(15f, (x + (float)w - 1f) / 8f))
			{
				int num2 = (int)this.E.max(0f, (float)this.E.flr(y / 8f));
				while ((float)num2 <= this.E.min(15f, (y + (float)h - 1f) / 8f))
				{
					int num3 = this.tile_at(num, num2);
					if (num3 == 17 && (this.E.mod(y + (float)h - 1f, 8f) >= 6f || y + (float)h == (float)(num2 * 8 + 8)) && yspd >= 0f)
					{
						return true;
					}
					if (num3 == 27 && this.E.mod(y, 8f) <= 2f && yspd <= 0f)
					{
						return true;
					}
					if (num3 == 43 && this.E.mod(x, 8f) <= 2f && xspd <= 0f)
					{
						return true;
					}
					if (num3 == 59 && ((x + (float)w - 1f) % 8f >= 6f || x + (float)w == (float)(num * 8 + 8)) && xspd >= 0f)
					{
						return true;
					}
					num2++;
				}
				num++;
			}
			return false;
		}

		// Token: 0x04001E97 RID: 7831
		public Emulator E;

		// Token: 0x04001E98 RID: 7832
		private Point room;

		// Token: 0x04001E99 RID: 7833
		private List<Classic.ClassicObject> objects;

		// Token: 0x04001E9A RID: 7834
		public int freeze;

		// Token: 0x04001E9B RID: 7835
		private int shake;

		// Token: 0x04001E9C RID: 7836
		private bool will_restart;

		// Token: 0x04001E9D RID: 7837
		private int delay_restart;

		// Token: 0x04001E9E RID: 7838
		private HashSet<int> got_fruit;

		// Token: 0x04001E9F RID: 7839
		private bool has_dashed;

		// Token: 0x04001EA0 RID: 7840
		private int sfx_timer;

		// Token: 0x04001EA1 RID: 7841
		private bool has_key;

		// Token: 0x04001EA2 RID: 7842
		private bool pause_player;

		// Token: 0x04001EA3 RID: 7843
		private bool flash_bg;

		// Token: 0x04001EA4 RID: 7844
		private int music_timer;

		// Token: 0x04001EA5 RID: 7845
		private bool new_bg;

		// Token: 0x04001EA6 RID: 7846
		private int k_left;

		// Token: 0x04001EA7 RID: 7847
		private int k_right = 1;

		// Token: 0x04001EA8 RID: 7848
		private int k_up = 2;

		// Token: 0x04001EA9 RID: 7849
		private int k_down = 3;

		// Token: 0x04001EAA RID: 7850
		private int k_jump = 4;

		// Token: 0x04001EAB RID: 7851
		private int k_dash = 5;

		// Token: 0x04001EAC RID: 7852
		private int frames;

		// Token: 0x04001EAD RID: 7853
		private int seconds;

		// Token: 0x04001EAE RID: 7854
		private int minutes;

		// Token: 0x04001EAF RID: 7855
		private int deaths;

		// Token: 0x04001EB0 RID: 7856
		private int max_djump;

		// Token: 0x04001EB1 RID: 7857
		private bool start_game;

		// Token: 0x04001EB2 RID: 7858
		private int start_game_flash;

		// Token: 0x04001EB3 RID: 7859
		private bool room_just_loaded;

		// Token: 0x04001EB4 RID: 7860
		private List<Classic.Cloud> clouds;

		// Token: 0x04001EB5 RID: 7861
		private List<Classic.Particle> particles;

		// Token: 0x04001EB6 RID: 7862
		private List<Classic.DeadParticle> dead_particles;

		// Token: 0x0200075A RID: 1882
		private class Cloud
		{
			// Token: 0x04002EED RID: 12013
			public float x;

			// Token: 0x04002EEE RID: 12014
			public float y;

			// Token: 0x04002EEF RID: 12015
			public float spd;

			// Token: 0x04002EF0 RID: 12016
			public float w;
		}

		// Token: 0x0200075B RID: 1883
		private class Particle
		{
			// Token: 0x04002EF1 RID: 12017
			public float x;

			// Token: 0x04002EF2 RID: 12018
			public float y;

			// Token: 0x04002EF3 RID: 12019
			public int s;

			// Token: 0x04002EF4 RID: 12020
			public float spd;

			// Token: 0x04002EF5 RID: 12021
			public float off;

			// Token: 0x04002EF6 RID: 12022
			public int c;
		}

		// Token: 0x0200075C RID: 1884
		private class DeadParticle
		{
			// Token: 0x04002EF7 RID: 12023
			public float x;

			// Token: 0x04002EF8 RID: 12024
			public float y;

			// Token: 0x04002EF9 RID: 12025
			public int t;

			// Token: 0x04002EFA RID: 12026
			public Vector2 spd;
		}

		// Token: 0x0200075D RID: 1885
		public class player : Classic.ClassicObject
		{
			// Token: 0x06002F4E RID: 12110 RVA: 0x00128C22 File Offset: 0x00126E22
			public override void init(Classic g, Emulator e)
			{
				base.init(g, e);
				this.spr = 1f;
				this.djump = g.max_djump;
				this.hitbox = new Rectangle(1, 3, 6, 5);
			}

			// Token: 0x06002F4F RID: 12111 RVA: 0x00128C54 File Offset: 0x00126E54
			public override void update()
			{
				if (this.G.pause_player)
				{
					return;
				}
				int num = this.E.btn(this.G.k_right) ? 1 : (this.E.btn(this.G.k_left) ? -1 : 0);
				if (this.G.spikes_at(this.x + (float)this.hitbox.X, this.y + (float)this.hitbox.Y, this.hitbox.Width, this.hitbox.Height, this.spd.X, this.spd.Y))
				{
					this.G.kill_player(this);
				}
				if (this.y > 128f)
				{
					this.G.kill_player(this);
				}
				bool flag = base.is_solid(0, 1);
				bool flag2 = base.is_ice(0, 1);
				if (flag && !this.was_on_ground)
				{
					this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y + 4f, null);
				}
				bool flag3 = this.E.btn(this.G.k_jump) && !this.p_jump;
				this.p_jump = this.E.btn(this.G.k_jump);
				if (flag3)
				{
					this.jbuffer = 4;
				}
				else if (this.jbuffer > 0)
				{
					this.jbuffer--;
				}
				bool flag4 = this.E.btn(this.G.k_dash) && !this.p_dash;
				this.p_dash = this.E.btn(this.G.k_dash);
				if (flag)
				{
					this.grace = 6;
					if (this.djump < this.G.max_djump)
					{
						this.G.psfx(54);
						this.djump = this.G.max_djump;
					}
				}
				else if (this.grace > 0)
				{
					this.grace--;
				}
				this.dash_effect_time--;
				if (this.dash_time > 0)
				{
					this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, null);
					this.dash_time--;
					this.spd.X = this.G.appr(this.spd.X, this.dash_target.X, this.dash_accel.X);
					this.spd.Y = this.G.appr(this.spd.Y, this.dash_target.Y, this.dash_accel.Y);
				}
				else
				{
					int num2 = 1;
					float amount = 0.6f;
					float amount2 = 0.15f;
					if (!flag)
					{
						amount = 0.4f;
					}
					else if (flag2)
					{
						amount = 0.05f;
						if (num == (this.flipX ? -1 : 1))
						{
							amount = 0.05f;
						}
					}
					if (this.E.abs(this.spd.X) > (float)num2)
					{
						this.spd.X = this.G.appr(this.spd.X, (float)(this.E.sign(this.spd.X) * num2), amount2);
					}
					else
					{
						this.spd.X = this.G.appr(this.spd.X, (float)(num * num2), amount);
					}
					if (this.spd.X != 0f)
					{
						this.flipX = (this.spd.X < 0f);
					}
					float target = 2f;
					float num3 = 0.21f;
					if (this.E.abs(this.spd.Y) <= 0.15f)
					{
						num3 *= 0.5f;
					}
					if (num != 0 && base.is_solid(num, 0) && !base.is_ice(num, 0))
					{
						target = 0.4f;
						if (this.E.rnd(10f) < 2f)
						{
							this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x + (float)(num * 6), this.y, null);
						}
					}
					if (!flag)
					{
						this.spd.Y = this.G.appr(this.spd.Y, target, num3);
					}
					if (this.jbuffer > 0)
					{
						if (this.grace > 0)
						{
							this.G.psfx(1);
							this.jbuffer = 0;
							this.grace = 0;
							this.spd.Y = -2f;
							this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y + 4f, null);
						}
						else
						{
							int num4 = base.is_solid(-3, 0) ? -1 : (base.is_solid(3, 0) ? 1 : 0);
							if (num4 != 0)
							{
								this.G.psfx(2);
								this.jbuffer = 0;
								this.spd.Y = -2f;
								this.spd.X = (float)(-(float)num4 * (num2 + 1));
								if (!base.is_ice(num4 * 3, 0))
								{
									this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x + (float)(num4 * 6), this.y, null);
								}
							}
						}
					}
					int num5 = 5;
					float num6 = (float)num5 * 0.70710677f;
					if (this.djump > 0 && flag4)
					{
						this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, null);
						this.djump--;
						this.dash_time = 4;
						this.G.has_dashed = true;
						this.dash_effect_time = 10;
						int num7 = this.E.dashDirectionX(this.flipX ? -1 : 1);
						int num8 = this.E.dashDirectionY(this.flipX ? -1 : 1);
						if (num7 != 0 && num8 != 0)
						{
							this.spd.X = (float)num7 * num6;
							this.spd.Y = (float)num8 * num6;
						}
						else if (num7 != 0)
						{
							this.spd.X = (float)(num7 * num5);
							this.spd.Y = 0f;
						}
						else if (num8 != 0)
						{
							this.spd.X = 0f;
							this.spd.Y = (float)(num8 * num5);
						}
						else
						{
							this.spd.X = (float)(this.flipX ? -1 : 1);
							this.spd.Y = 0f;
						}
						this.G.psfx(3);
						this.G.freeze = 2;
						this.G.shake = 6;
						this.dash_target.X = (float)(2 * this.E.sign(this.spd.X));
						this.dash_target.Y = (float)(2 * this.E.sign(this.spd.Y));
						this.dash_accel.X = 1.5f;
						this.dash_accel.Y = 1.5f;
						if (this.spd.Y < 0f)
						{
							this.dash_target.Y = this.dash_target.Y * 0.75f;
						}
						if (this.spd.Y != 0f)
						{
							this.dash_accel.X = this.dash_accel.X * 0.70710677f;
						}
						if (this.spd.X != 0f)
						{
							this.dash_accel.Y = this.dash_accel.Y * 0.70710677f;
						}
					}
					else if (flag4 && this.djump <= 0)
					{
						this.G.psfx(9);
						this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, null);
					}
				}
				this.spr_off += 0.25f;
				if (!flag)
				{
					if (base.is_solid(num, 0))
					{
						this.spr = 5f;
					}
					else
					{
						this.spr = 3f;
					}
				}
				else if (this.E.btn(this.G.k_down))
				{
					this.spr = 6f;
				}
				else if (this.E.btn(this.G.k_up))
				{
					this.spr = 7f;
				}
				else if (this.spd.X == 0f || (!this.E.btn(this.G.k_left) && !this.E.btn(this.G.k_right)))
				{
					this.spr = 1f;
				}
				else
				{
					this.spr = 1f + this.spr_off % 4f;
				}
				if (this.y < -4f && this.G.level_index() < 30)
				{
					this.G.next_room();
				}
				this.was_on_ground = flag;
			}

			// Token: 0x06002F50 RID: 12112 RVA: 0x0012957C File Offset: 0x0012777C
			public override void draw()
			{
				if (this.x < -1f || this.x > 121f)
				{
					this.x = this.G.clamp(this.x, -1f, 121f);
					this.spd.X = 0f;
				}
				this.hair.draw_hair(this, this.flipX ? -1 : 1, this.djump);
				this.G.draw_player(this, this.djump);
			}

			// Token: 0x04002EFB RID: 12027
			public bool p_jump;

			// Token: 0x04002EFC RID: 12028
			public bool p_dash;

			// Token: 0x04002EFD RID: 12029
			public int grace;

			// Token: 0x04002EFE RID: 12030
			public int jbuffer;

			// Token: 0x04002EFF RID: 12031
			public int djump;

			// Token: 0x04002F00 RID: 12032
			public int dash_time;

			// Token: 0x04002F01 RID: 12033
			public int dash_effect_time;

			// Token: 0x04002F02 RID: 12034
			public Vector2 dash_target = new Vector2(0f, 0f);

			// Token: 0x04002F03 RID: 12035
			public Vector2 dash_accel = new Vector2(0f, 0f);

			// Token: 0x04002F04 RID: 12036
			public float spr_off;

			// Token: 0x04002F05 RID: 12037
			public bool was_on_ground;

			// Token: 0x04002F06 RID: 12038
			public Classic.player_hair hair;
		}

		// Token: 0x0200075E RID: 1886
		public class player_hair
		{
			// Token: 0x06002F52 RID: 12114 RVA: 0x00129638 File Offset: 0x00127838
			public player_hair(Classic.ClassicObject obj)
			{
				this.E = obj.E;
				this.G = obj.G;
				for (int i = 0; i <= 4; i++)
				{
					this.hair[i] = new Classic.player_hair.node
					{
						x = obj.x,
						y = obj.y,
						size = this.E.max(1f, this.E.min(2f, (float)(3 - i)))
					};
				}
			}

			// Token: 0x06002F53 RID: 12115 RVA: 0x001296CC File Offset: 0x001278CC
			public void draw_hair(Classic.ClassicObject obj, int facing, int djump)
			{
				int num = (djump == 1) ? 8 : ((djump == 2) ? (7 + this.E.flr((float)(this.G.frames / 3 % 2)) * 4) : 12);
				Vector2 vector = new Vector2(obj.x + 4f - (float)(facing * 2), obj.y + (float)(this.E.btn(this.G.k_down) ? 4 : 3));
				foreach (Classic.player_hair.node node in this.hair)
				{
					node.x += (vector.X - node.x) / 1.5f;
					node.y += (vector.Y + 0.5f - node.y) / 1.5f;
					this.E.circfill(node.x, node.y, node.size, (float)num);
					vector = new Vector2(node.x, node.y);
				}
			}

			// Token: 0x04002F07 RID: 12039
			private Classic.player_hair.node[] hair = new Classic.player_hair.node[5];

			// Token: 0x04002F08 RID: 12040
			private Emulator E;

			// Token: 0x04002F09 RID: 12041
			private Classic G;

			// Token: 0x0200079A RID: 1946
			private class node
			{
				// Token: 0x04002FCB RID: 12235
				public float x;

				// Token: 0x04002FCC RID: 12236
				public float y;

				// Token: 0x04002FCD RID: 12237
				public float size;
			}
		}

		// Token: 0x0200075F RID: 1887
		public class player_spawn : Classic.ClassicObject
		{
			// Token: 0x06002F54 RID: 12116 RVA: 0x001297E8 File Offset: 0x001279E8
			public override void init(Classic g, Emulator e)
			{
				base.init(g, e);
				this.spr = 3f;
				this.target = new Vector2(this.x, this.y);
				this.y = 128f;
				this.spd.Y = -4f;
				this.state = 0;
				this.delay = 0;
				this.solids = false;
				this.hair = new Classic.player_hair(this);
				this.E.sfx(4);
			}

			// Token: 0x06002F55 RID: 12117 RVA: 0x00129868 File Offset: 0x00127A68
			public override void update()
			{
				if (this.state == 0)
				{
					if (this.y < this.target.Y + 16f)
					{
						this.state = 1;
						this.delay = 3;
						return;
					}
				}
				else if (this.state == 1)
				{
					this.spd.Y = this.spd.Y + 0.5f;
					if (this.spd.Y > 0f && this.delay > 0)
					{
						this.spd.Y = 0f;
						this.delay--;
					}
					if (this.spd.Y > 0f && this.y > this.target.Y)
					{
						this.y = this.target.Y;
						this.spd = new Vector2(0f, 0f);
						this.state = 2;
						this.delay = 5;
						this.G.shake = 5;
						this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y + 4f, null);
						this.E.sfx(5);
						return;
					}
				}
				else if (this.state == 2)
				{
					this.delay--;
					this.spr = 6f;
					if (this.delay < 0)
					{
						this.G.destroy_object(this);
						this.G.init_object<Classic.player>(new Classic.player(), this.x, this.y, null).hair = this.hair;
					}
				}
			}

			// Token: 0x06002F56 RID: 12118 RVA: 0x00129A0C File Offset: 0x00127C0C
			public override void draw()
			{
				this.hair.draw_hair(this, 1, this.G.max_djump);
				this.G.draw_player(this, this.G.max_djump);
			}

			// Token: 0x04002F0A RID: 12042
			private Vector2 target;

			// Token: 0x04002F0B RID: 12043
			private int state;

			// Token: 0x04002F0C RID: 12044
			private int delay;

			// Token: 0x04002F0D RID: 12045
			private Classic.player_hair hair;
		}

		// Token: 0x02000760 RID: 1888
		public class spring : Classic.ClassicObject
		{
			// Token: 0x06002F58 RID: 12120 RVA: 0x00129A48 File Offset: 0x00127C48
			public override void update()
			{
				if (this.hide_for > 0)
				{
					this.hide_for--;
					if (this.hide_for <= 0)
					{
						this.spr = 18f;
						this.delay = 0;
					}
				}
				else if (this.spr == 18f)
				{
					Classic.player player = base.collide<Classic.player>(0, 0);
					if (player != null && player.spd.Y >= 0f)
					{
						this.spr = 19f;
						player.y = this.y - 4f;
						Classic.player player2 = player;
						player2.spd.X = player2.spd.X * 0.2f;
						player.spd.Y = -3f;
						player.djump = this.G.max_djump;
						this.delay = 10;
						this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, null);
						Classic.fall_floor fall_floor = base.collide<Classic.fall_floor>(0, 1);
						if (fall_floor != null)
						{
							this.G.break_fall_floor(fall_floor);
						}
						this.G.psfx(8);
					}
				}
				else if (this.delay > 0)
				{
					this.delay--;
					if (this.delay <= 0)
					{
						this.spr = 18f;
					}
				}
				if (this.hide_in > 0)
				{
					this.hide_in--;
					if (this.hide_in <= 0)
					{
						this.hide_for = 60;
						this.spr = 0f;
					}
				}
			}

			// Token: 0x04002F0E RID: 12046
			public int hide_in;

			// Token: 0x04002F0F RID: 12047
			private int hide_for;

			// Token: 0x04002F10 RID: 12048
			private int delay;
		}

		// Token: 0x02000761 RID: 1889
		public class balloon : Classic.ClassicObject
		{
			// Token: 0x06002F5A RID: 12122 RVA: 0x00129BC7 File Offset: 0x00127DC7
			public override void init(Classic g, Emulator e)
			{
				base.init(g, e);
				this.offset = this.E.rnd(1f);
				this.start = this.y;
				this.hitbox = new Rectangle(-1, -1, 10, 10);
			}

			// Token: 0x06002F5B RID: 12123 RVA: 0x00129C04 File Offset: 0x00127E04
			public override void update()
			{
				if (this.spr == 22f)
				{
					this.offset += 0.01f;
					this.y = this.start + this.E.sin(this.offset) * 2f;
					Classic.player player = base.collide<Classic.player>(0, 0);
					if (player != null && player.djump < this.G.max_djump)
					{
						this.G.psfx(6);
						this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, null);
						player.djump = this.G.max_djump;
						this.spr = 0f;
						this.timer = 60f;
						return;
					}
				}
				else
				{
					if (this.timer > 0f)
					{
						this.timer -= 1f;
						return;
					}
					this.G.psfx(7);
					this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, null);
					this.spr = 22f;
				}
			}

			// Token: 0x06002F5C RID: 12124 RVA: 0x00129D34 File Offset: 0x00127F34
			public override void draw()
			{
				if (this.spr == 22f)
				{
					this.E.spr(13f + this.offset * 8f % 3f, this.x, this.y + 6f, 1, 1, false, false);
					this.E.spr(this.spr, this.x, this.y, 1, 1, false, false);
				}
			}

			// Token: 0x04002F11 RID: 12049
			private float offset;

			// Token: 0x04002F12 RID: 12050
			private float start;

			// Token: 0x04002F13 RID: 12051
			private float timer;
		}

		// Token: 0x02000762 RID: 1890
		public class fall_floor : Classic.ClassicObject
		{
			// Token: 0x06002F5E RID: 12126 RVA: 0x00129DA8 File Offset: 0x00127FA8
			public override void update()
			{
				if (this.state == 0)
				{
					if (base.check<Classic.player>(0, -1) || base.check<Classic.player>(-1, 0) || base.check<Classic.player>(1, 0))
					{
						this.G.break_fall_floor(this);
						return;
					}
				}
				else if (this.state == 1)
				{
					this.delay--;
					if (this.delay <= 0)
					{
						this.state = 2;
						this.delay = 60;
						this.collideable = false;
						return;
					}
				}
				else if (this.state == 2)
				{
					this.delay--;
					if (this.delay <= 0 && !base.check<Classic.player>(0, 0))
					{
						this.G.psfx(7);
						this.state = 0;
						this.collideable = true;
						this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, null);
					}
				}
			}

			// Token: 0x06002F5F RID: 12127 RVA: 0x00129E90 File Offset: 0x00128090
			public override void draw()
			{
				if (this.state != 2)
				{
					if (this.state != 1)
					{
						this.E.spr(23f, this.x, this.y, 1, 1, false, false);
						return;
					}
					this.E.spr((float)(23 + (15 - this.delay) / 5), this.x, this.y, 1, 1, false, false);
				}
			}

			// Token: 0x04002F14 RID: 12052
			public int state;

			// Token: 0x04002F15 RID: 12053
			public bool solid = true;

			// Token: 0x04002F16 RID: 12054
			public int delay;
		}

		// Token: 0x02000763 RID: 1891
		public class smoke : Classic.ClassicObject
		{
			// Token: 0x06002F61 RID: 12129 RVA: 0x00129F0C File Offset: 0x0012810C
			public override void init(Classic g, Emulator e)
			{
				base.init(g, e);
				this.spr = 29f;
				this.spd.Y = -0.1f;
				this.spd.X = 0.3f + this.E.rnd(0.2f);
				this.x += -1f + this.E.rnd(2f);
				this.y += -1f + this.E.rnd(2f);
				this.flipX = this.G.maybe();
				this.flipY = this.G.maybe();
				this.solids = false;
			}

			// Token: 0x06002F62 RID: 12130 RVA: 0x00129FCC File Offset: 0x001281CC
			public override void update()
			{
				this.spr += 0.2f;
				if (this.spr >= 32f)
				{
					this.G.destroy_object(this);
				}
			}
		}

		// Token: 0x02000764 RID: 1892
		public class fruit : Classic.ClassicObject
		{
			// Token: 0x06002F64 RID: 12132 RVA: 0x00129FF9 File Offset: 0x001281F9
			public override void init(Classic g, Emulator e)
			{
				base.init(g, e);
				this.spr = 26f;
				this.start = this.y;
				this.off = 0f;
			}

			// Token: 0x06002F65 RID: 12133 RVA: 0x0012A028 File Offset: 0x00128228
			public override void update()
			{
				Classic.player player = base.collide<Classic.player>(0, 0);
				if (player != null)
				{
					player.djump = this.G.max_djump;
					this.G.sfx_timer = 20;
					this.E.sfx(13);
					this.G.got_fruit.Add(1 + this.G.level_index());
					this.G.init_object<Classic.lifeup>(new Classic.lifeup(), this.x, this.y, null);
					this.G.destroy_object(this);
					Stats.Increment(Stat.PICO_BERRIES, 1);
				}
				this.off += 1f;
				this.y = this.start + this.E.sin(this.off / 40f) * 2.5f;
			}

			// Token: 0x04002F17 RID: 12055
			private float start;

			// Token: 0x04002F18 RID: 12056
			private float off;
		}

		// Token: 0x02000765 RID: 1893
		public class fly_fruit : Classic.ClassicObject
		{
			// Token: 0x06002F67 RID: 12135 RVA: 0x0012A102 File Offset: 0x00128302
			public override void init(Classic g, Emulator e)
			{
				base.init(g, e);
				this.start = this.y;
				this.solids = false;
			}

			// Token: 0x06002F68 RID: 12136 RVA: 0x0012A120 File Offset: 0x00128320
			public override void update()
			{
				if (this.fly)
				{
					if (this.sfx_delay > 0f)
					{
						this.sfx_delay -= 1f;
						if (this.sfx_delay <= 0f)
						{
							this.G.sfx_timer = 20;
							this.E.sfx(14);
						}
					}
					this.spd.Y = this.G.appr(this.spd.Y, -3.5f, 0.25f);
					if (this.y < -16f)
					{
						this.G.destroy_object(this);
					}
				}
				else
				{
					if (this.G.has_dashed)
					{
						this.fly = true;
					}
					this.step += 0.05f;
					this.spd.Y = this.E.sin(this.step) * 0.5f;
				}
				Classic.player player = base.collide<Classic.player>(0, 0);
				if (player != null)
				{
					player.djump = this.G.max_djump;
					this.G.sfx_timer = 20;
					this.E.sfx(13);
					this.G.got_fruit.Add(1 + this.G.level_index());
					this.G.init_object<Classic.lifeup>(new Classic.lifeup(), this.x, this.y, null);
					this.G.destroy_object(this);
					Stats.Increment(Stat.PICO_BERRIES, 1);
				}
			}

			// Token: 0x06002F69 RID: 12137 RVA: 0x0012A2A0 File Offset: 0x001284A0
			public override void draw()
			{
				float num = 0f;
				if (!this.fly)
				{
					if (this.E.sin(this.step) < 0f)
					{
						num = 1f + this.E.max(0f, (float)this.G.sign(this.y - this.start));
					}
				}
				else
				{
					num = (num + 0.25f) % 3f;
				}
				this.E.spr(45f + num, this.x - 6f, this.y - 2f, 1, 1, true, false);
				this.E.spr(this.spr, this.x, this.y, 1, 1, false, false);
				this.E.spr(45f + num, this.x + 6f, this.y - 2f, 1, 1, false, false);
			}

			// Token: 0x04002F19 RID: 12057
			private float start;

			// Token: 0x04002F1A RID: 12058
			private bool fly;

			// Token: 0x04002F1B RID: 12059
			private float step = 0.5f;

			// Token: 0x04002F1C RID: 12060
			private float sfx_delay = 8f;
		}

		// Token: 0x02000766 RID: 1894
		public class lifeup : Classic.ClassicObject
		{
			// Token: 0x06002F6B RID: 12139 RVA: 0x0012A3B0 File Offset: 0x001285B0
			public override void init(Classic g, Emulator e)
			{
				base.init(g, e);
				this.spd.Y = -0.25f;
				this.duration = 30;
				this.x -= 2f;
				this.y -= 4f;
				this.flash = 0f;
				this.solids = false;
			}

			// Token: 0x06002F6C RID: 12140 RVA: 0x0012A413 File Offset: 0x00128613
			public override void update()
			{
				this.duration--;
				if (this.duration <= 0)
				{
					this.G.destroy_object(this);
				}
			}

			// Token: 0x06002F6D RID: 12141 RVA: 0x0012A438 File Offset: 0x00128638
			public override void draw()
			{
				this.flash += 0.5f;
				this.E.print("1000", this.x - 2f, this.y, 7f + this.flash % 2f);
			}

			// Token: 0x04002F1D RID: 12061
			private int duration;

			// Token: 0x04002F1E RID: 12062
			private float flash;
		}

		// Token: 0x02000767 RID: 1895
		public class fake_wall : Classic.ClassicObject
		{
			// Token: 0x06002F6F RID: 12143 RVA: 0x0012A48C File Offset: 0x0012868C
			public override void update()
			{
				this.hitbox = new Rectangle(-1, -1, 18, 18);
				Classic.player player = base.collide<Classic.player>(0, 0);
				if (player != null && player.dash_effect_time > 0)
				{
					player.spd.X = (float)(-(float)this.G.sign(player.spd.X)) * 1.5f;
					player.spd.Y = -1.5f;
					player.dash_time = -1;
					this.G.sfx_timer = 20;
					this.E.sfx(16);
					this.G.destroy_object(this);
					this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, null);
					this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x + 8f, this.y, null);
					this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y + 8f, null);
					this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x + 8f, this.y + 8f, null);
					this.G.init_object<Classic.fruit>(new Classic.fruit(), this.x + 4f, this.y + 4f, null);
				}
				this.hitbox = new Rectangle(0, 0, 16, 16);
			}

			// Token: 0x06002F70 RID: 12144 RVA: 0x0012A620 File Offset: 0x00128820
			public override void draw()
			{
				this.E.spr(64f, this.x, this.y, 1, 1, false, false);
				this.E.spr(65f, this.x + 8f, this.y, 1, 1, false, false);
				this.E.spr(80f, this.x, this.y + 8f, 1, 1, false, false);
				this.E.spr(81f, this.x + 8f, this.y + 8f, 1, 1, false, false);
			}
		}

		// Token: 0x02000768 RID: 1896
		public class key : Classic.ClassicObject
		{
			// Token: 0x06002F72 RID: 12146 RVA: 0x0012A6C8 File Offset: 0x001288C8
			public override void update()
			{
				int num = this.E.flr(this.spr);
				this.spr = 9f + (this.E.sin((float)this.G.frames / 30f) + 0.5f) * 1f;
				int num2 = this.E.flr(this.spr);
				if (num2 == 10 && num2 != num)
				{
					this.flipX = !this.flipX;
				}
				if (base.check<Classic.player>(0, 0))
				{
					this.E.sfx(23);
					this.G.sfx_timer = 20;
					this.G.destroy_object(this);
					this.G.has_key = true;
				}
			}
		}

		// Token: 0x02000769 RID: 1897
		public class chest : Classic.ClassicObject
		{
			// Token: 0x06002F74 RID: 12148 RVA: 0x0012A782 File Offset: 0x00128982
			public override void init(Classic g, Emulator e)
			{
				base.init(g, e);
				this.x -= 4f;
				this.start = this.x;
				this.timer = 20f;
			}

			// Token: 0x06002F75 RID: 12149 RVA: 0x0012A7B8 File Offset: 0x001289B8
			public override void update()
			{
				if (this.G.has_key)
				{
					this.timer -= 1f;
					this.x = this.start - 1f + this.E.rnd(3f);
					if (this.timer <= 0f)
					{
						this.G.sfx_timer = 20;
						this.E.sfx(16);
						this.G.init_object<Classic.fruit>(new Classic.fruit(), this.x, this.y - 4f, null);
						this.G.destroy_object(this);
					}
				}
			}

			// Token: 0x04002F1F RID: 12063
			private float start;

			// Token: 0x04002F20 RID: 12064
			private float timer;
		}

		// Token: 0x0200076A RID: 1898
		public class platform : Classic.ClassicObject
		{
			// Token: 0x06002F77 RID: 12151 RVA: 0x0012A869 File Offset: 0x00128A69
			public override void init(Classic g, Emulator e)
			{
				base.init(g, e);
				this.x -= 4f;
				this.solids = false;
				this.hitbox.Width = 16;
				this.last = this.x;
			}

			// Token: 0x06002F78 RID: 12152 RVA: 0x0012A8A8 File Offset: 0x00128AA8
			public override void update()
			{
				this.spd.X = this.dir * 0.65f;
				if (this.x < -16f)
				{
					this.x = 128f;
				}
				if (this.x > 128f)
				{
					this.x = -16f;
				}
				if (!base.check<Classic.player>(0, 0))
				{
					Classic.player player = base.collide<Classic.player>(0, -1);
					if (player != null)
					{
						player.move_x((int)(this.x - this.last), 1);
					}
				}
				this.last = this.x;
			}

			// Token: 0x06002F79 RID: 12153 RVA: 0x0012A934 File Offset: 0x00128B34
			public override void draw()
			{
				this.E.spr(11f, this.x, this.y - 1f, 1, 1, false, false);
				this.E.spr(12f, this.x + 8f, this.y - 1f, 1, 1, false, false);
			}

			// Token: 0x04002F21 RID: 12065
			public float dir;

			// Token: 0x04002F22 RID: 12066
			private float last;
		}

		// Token: 0x0200076B RID: 1899
		public class message : Classic.ClassicObject
		{
			// Token: 0x06002F7B RID: 12155 RVA: 0x0012A994 File Offset: 0x00128B94
			public override void draw()
			{
				string text = "-- celeste mountain --#this memorial to those# perished on the climb";
				if (base.check<Classic.player>(4, 0))
				{
					if (this.index < (float)text.Length)
					{
						this.index += 0.5f;
						if (this.index >= this.last + 1f)
						{
							this.last += 1f;
							this.E.sfx(35);
						}
					}
					Vector2 vector = new Vector2(8f, 96f);
					int num = 0;
					while ((float)num < this.index)
					{
						if (text[num] != '#')
						{
							this.E.rectfill(vector.X - 2f, vector.Y - 2f, vector.X + 7f, vector.Y + 6f, 7f);
							this.E.print(text[num].ToString() ?? "", vector.X, vector.Y, 0f);
							vector.X += 5f;
						}
						else
						{
							vector.X = 8f;
							vector.Y += 7f;
						}
						num++;
					}
					return;
				}
				this.index = 0f;
				this.last = 0f;
			}

			// Token: 0x04002F23 RID: 12067
			private float last;

			// Token: 0x04002F24 RID: 12068
			private float index;
		}

		// Token: 0x0200076C RID: 1900
		public class big_chest : Classic.ClassicObject
		{
			// Token: 0x06002F7D RID: 12157 RVA: 0x0012AAF8 File Offset: 0x00128CF8
			public override void init(Classic g, Emulator e)
			{
				base.init(g, e);
				this.hitbox.Width = 16;
			}

			// Token: 0x06002F7E RID: 12158 RVA: 0x0012AB10 File Offset: 0x00128D10
			public override void draw()
			{
				if (this.state == 0)
				{
					Classic.player player = base.collide<Classic.player>(0, 8);
					if (player != null && player.is_solid(0, 1))
					{
						this.E.music(-1, 500, 7);
						this.E.sfx(37);
						this.G.pause_player = true;
						player.spd.X = 0f;
						player.spd.Y = 0f;
						this.state = 1;
						this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, null);
						this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x + 8f, this.y, null);
						this.timer = 60f;
						this.particles = new List<Classic.big_chest.particle>();
					}
					this.E.spr(96f, this.x, this.y, 1, 1, false, false);
					this.E.spr(97f, this.x + 8f, this.y, 1, 1, false, false);
				}
				else if (this.state == 1)
				{
					this.timer -= 1f;
					this.G.shake = 5;
					this.G.flash_bg = true;
					if (this.timer <= 45f && this.particles.Count < 50)
					{
						this.particles.Add(new Classic.big_chest.particle
						{
							x = 1f + this.E.rnd(14f),
							y = 0f,
							h = 32f + this.E.rnd(32f),
							spd = 8f + this.E.rnd(8f)
						});
					}
					if (this.timer < 0f)
					{
						this.state = 2;
						this.particles.Clear();
						this.G.flash_bg = false;
						this.G.new_bg = true;
						this.G.init_object<Classic.orb>(new Classic.orb(), this.x + 4f, this.y + 4f, null);
						this.G.pause_player = false;
					}
					foreach (Classic.big_chest.particle particle in this.particles)
					{
						particle.y += particle.spd;
						this.E.rectfill(this.x + particle.x, this.y + 8f - particle.y, this.x + particle.x, this.E.min(this.y + 8f - particle.y + particle.h, this.y + 8f), 7f);
					}
				}
				this.E.spr(112f, this.x, this.y + 8f, 1, 1, false, false);
				this.E.spr(113f, this.x + 8f, this.y + 8f, 1, 1, false, false);
			}

			// Token: 0x04002F25 RID: 12069
			private int state;

			// Token: 0x04002F26 RID: 12070
			private float timer;

			// Token: 0x04002F27 RID: 12071
			private List<Classic.big_chest.particle> particles;

			// Token: 0x0200079B RID: 1947
			private class particle
			{
				// Token: 0x04002FCE RID: 12238
				public float x;

				// Token: 0x04002FCF RID: 12239
				public float y;

				// Token: 0x04002FD0 RID: 12240
				public float h;

				// Token: 0x04002FD1 RID: 12241
				public float spd;
			}
		}

		// Token: 0x0200076D RID: 1901
		public class orb : Classic.ClassicObject
		{
			// Token: 0x06002F80 RID: 12160 RVA: 0x0012AE98 File Offset: 0x00129098
			public override void init(Classic g, Emulator e)
			{
				base.init(g, e);
				this.spd.Y = -4f;
				this.solids = false;
			}

			// Token: 0x06002F81 RID: 12161 RVA: 0x0012AEBC File Offset: 0x001290BC
			public override void draw()
			{
				this.spd.Y = this.G.appr(this.spd.Y, 0f, 0.5f);
				Classic.player player = base.collide<Classic.player>(0, 0);
				if (this.spd.Y == 0f && player != null)
				{
					this.G.music_timer = 45;
					this.E.sfx(51);
					this.G.freeze = 10;
					this.G.shake = 10;
					this.G.destroy_object(this);
					this.G.max_djump = 2;
					player.djump = 2;
				}
				this.E.spr(102f, this.x, this.y, 1, 1, false, false);
				float num = (float)this.G.frames / 30f;
				for (int i = 0; i <= 7; i++)
				{
					this.E.circfill(this.x + 4f + this.E.cos(num + (float)i / 8f) * 8f, this.y + 4f + this.E.sin(num + (float)i / 8f) * 8f, 1f, 7f);
				}
			}
		}

		// Token: 0x0200076E RID: 1902
		public class flag : Classic.ClassicObject
		{
			// Token: 0x06002F83 RID: 12163 RVA: 0x0012B00C File Offset: 0x0012920C
			public override void init(Classic g, Emulator e)
			{
				base.init(g, e);
				this.x += 5f;
				this.score = (float)this.G.got_fruit.Count;
				Stats.Increment(Stat.PICO_COMPLETES, 1);
				Achievements.Register(Achievement.PICO8);
			}

			// Token: 0x06002F84 RID: 12164 RVA: 0x0012B058 File Offset: 0x00129258
			public override void draw()
			{
				this.spr = 118f + (float)this.G.frames / 5f % 3f;
				this.E.spr(this.spr, this.x, this.y, 1, 1, false, false);
				if (this.show)
				{
					this.E.rectfill(32f, 2f, 96f, 31f, 0f);
					this.E.spr(26f, 55f, 6f, 1, 1, false, false);
					this.E.print("x" + this.score, 64f, 9f, 7f);
					this.G.draw_time(49, 16);
					this.E.print("deaths:" + this.G.deaths, 48f, 24f, 7f);
					return;
				}
				if (base.check<Classic.player>(0, 0))
				{
					this.E.sfx(55);
					this.G.sfx_timer = 30;
					this.show = true;
				}
			}

			// Token: 0x04002F28 RID: 12072
			private float score;

			// Token: 0x04002F29 RID: 12073
			private bool show;
		}

		// Token: 0x0200076F RID: 1903
		public class room_title : Classic.ClassicObject
		{
			// Token: 0x06002F86 RID: 12166 RVA: 0x0012B198 File Offset: 0x00129398
			public override void draw()
			{
				this.delay -= 1f;
				if (this.delay < -30f)
				{
					this.G.destroy_object(this);
					return;
				}
				if (this.delay < 0f)
				{
					this.E.rectfill(24f, 58f, 104f, 70f, 0f);
					if (this.G.room.X == 3 && this.G.room.Y == 1)
					{
						this.E.print("old site", 48f, 62f, 7f);
					}
					else if (this.G.level_index() == 30)
					{
						this.E.print("summit", 52f, 62f, 7f);
					}
					else
					{
						int num = (1 + this.G.level_index()) * 100;
						this.E.print(num + "m", (float)(52 + ((num < 1000) ? 2 : 0)), 62f, 7f);
					}
					this.G.draw_time(4, 4);
				}
			}

			// Token: 0x04002F2A RID: 12074
			private float delay = 5f;
		}

		// Token: 0x02000770 RID: 1904
		public class ClassicObject
		{
			// Token: 0x06002F88 RID: 12168 RVA: 0x0012B2E2 File Offset: 0x001294E2
			public virtual void init(Classic g, Emulator e)
			{
				this.G = g;
				this.E = e;
			}

			// Token: 0x06002F89 RID: 12169 RVA: 0x000091E2 File Offset: 0x000073E2
			public virtual void update()
			{
			}

			// Token: 0x06002F8A RID: 12170 RVA: 0x0012B2F2 File Offset: 0x001294F2
			public virtual void draw()
			{
				if (this.spr > 0f)
				{
					this.E.spr(this.spr, this.x, this.y, 1, 1, this.flipX, this.flipY);
				}
			}

			// Token: 0x06002F8B RID: 12171 RVA: 0x0012B32C File Offset: 0x0012952C
			public bool is_solid(int ox, int oy)
			{
				return (oy > 0 && !this.check<Classic.platform>(ox, 0) && this.check<Classic.platform>(ox, oy)) || this.G.solid_at(this.x + (float)this.hitbox.X + (float)ox, this.y + (float)this.hitbox.Y + (float)oy, (float)this.hitbox.Width, (float)this.hitbox.Height) || this.check<Classic.fall_floor>(ox, oy) || this.check<Classic.fake_wall>(ox, oy);
			}

			// Token: 0x06002F8C RID: 12172 RVA: 0x0012B3B8 File Offset: 0x001295B8
			public bool is_ice(int ox, int oy)
			{
				return this.G.ice_at(this.x + (float)this.hitbox.X + (float)ox, this.y + (float)this.hitbox.Y + (float)oy, (float)this.hitbox.Width, (float)this.hitbox.Height);
			}

			// Token: 0x06002F8D RID: 12173 RVA: 0x0012B414 File Offset: 0x00129614
			public T collide<T>(int ox, int oy) where T : Classic.ClassicObject
			{
				Type typeFromHandle = typeof(T);
				foreach (Classic.ClassicObject classicObject in this.G.objects)
				{
					if (classicObject != null && classicObject.GetType() == typeFromHandle && classicObject != this && classicObject.collideable && classicObject.x + (float)classicObject.hitbox.X + (float)classicObject.hitbox.Width > this.x + (float)this.hitbox.X + (float)ox && classicObject.y + (float)classicObject.hitbox.Y + (float)classicObject.hitbox.Height > this.y + (float)this.hitbox.Y + (float)oy && classicObject.x + (float)classicObject.hitbox.X < this.x + (float)this.hitbox.X + (float)this.hitbox.Width + (float)ox && classicObject.y + (float)classicObject.hitbox.Y < this.y + (float)this.hitbox.Y + (float)this.hitbox.Height + (float)oy)
					{
						return classicObject as T;
					}
				}
				return default(T);
			}

			// Token: 0x06002F8E RID: 12174 RVA: 0x0012B5A8 File Offset: 0x001297A8
			public bool check<T>(int ox, int oy) where T : Classic.ClassicObject
			{
				return this.collide<T>(ox, oy) != null;
			}

			// Token: 0x06002F8F RID: 12175 RVA: 0x0012B5BC File Offset: 0x001297BC
			public void move(float ox, float oy)
			{
				this.rem.X = this.rem.X + ox;
				int num = this.E.flr(this.rem.X + 0.5f);
				this.rem.X = this.rem.X - (float)num;
				this.move_x(num, 0);
				this.rem.Y = this.rem.Y + oy;
				num = this.E.flr(this.rem.Y + 0.5f);
				this.rem.Y = this.rem.Y - (float)num;
				this.move_y(num);
			}

			// Token: 0x06002F90 RID: 12176 RVA: 0x0012B658 File Offset: 0x00129858
			public void move_x(int amount, int start)
			{
				if (this.solids)
				{
					int num = this.G.sign((float)amount);
					int num2 = start;
					while ((float)num2 <= this.E.abs((float)amount))
					{
						if (this.is_solid(num, 0))
						{
							this.spd.X = 0f;
							this.rem.X = 0f;
							return;
						}
						this.x += (float)num;
						num2++;
					}
					return;
				}
				this.x += (float)amount;
			}

			// Token: 0x06002F91 RID: 12177 RVA: 0x0012B6E0 File Offset: 0x001298E0
			public void move_y(int amount)
			{
				if (this.solids)
				{
					int num = this.G.sign((float)amount);
					int num2 = 0;
					while ((float)num2 <= this.E.abs((float)amount))
					{
						if (this.is_solid(0, num))
						{
							this.spd.Y = 0f;
							this.rem.Y = 0f;
							return;
						}
						this.y += (float)num;
						num2++;
					}
					return;
				}
				this.y += (float)amount;
			}

			// Token: 0x04002F2B RID: 12075
			public Classic G;

			// Token: 0x04002F2C RID: 12076
			public Emulator E;

			// Token: 0x04002F2D RID: 12077
			public int type;

			// Token: 0x04002F2E RID: 12078
			public bool collideable = true;

			// Token: 0x04002F2F RID: 12079
			public bool solids = true;

			// Token: 0x04002F30 RID: 12080
			public float spr;

			// Token: 0x04002F31 RID: 12081
			public bool flipX;

			// Token: 0x04002F32 RID: 12082
			public bool flipY;

			// Token: 0x04002F33 RID: 12083
			public float x;

			// Token: 0x04002F34 RID: 12084
			public float y;

			// Token: 0x04002F35 RID: 12085
			public Rectangle hitbox = new Rectangle(0, 0, 8, 8);

			// Token: 0x04002F36 RID: 12086
			public Vector2 spd = new Vector2(0f, 0f);

			// Token: 0x04002F37 RID: 12087
			public Vector2 rem = new Vector2(0f, 0f);
		}
	}
}
