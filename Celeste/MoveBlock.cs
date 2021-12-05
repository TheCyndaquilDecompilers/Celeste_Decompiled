using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002C9 RID: 713
	public class MoveBlock : Solid
	{
		// Token: 0x06001612 RID: 5650 RVA: 0x00080938 File Offset: 0x0007EB38
		public MoveBlock(Vector2 position, int width, int height, MoveBlock.Directions direction, bool canSteer, bool fast) : base(position, (float)width, (float)height, false)
		{
			base.Depth = -1;
			this.startPosition = position;
			this.canSteer = canSteer;
			this.direction = direction;
			this.fast = fast;
			switch (direction)
			{
			case MoveBlock.Directions.Left:
				this.homeAngle = (this.targetAngle = (this.angle = 3.1415927f));
				this.angleSteerSign = -1;
				break;
			default:
				this.homeAngle = (this.targetAngle = (this.angle = 0f));
				this.angleSteerSign = 1;
				break;
			case MoveBlock.Directions.Up:
				this.homeAngle = (this.targetAngle = (this.angle = -1.5707964f));
				this.angleSteerSign = 1;
				break;
			case MoveBlock.Directions.Down:
				this.homeAngle = (this.targetAngle = (this.angle = 1.5707964f));
				this.angleSteerSign = -1;
				break;
			}
			int num = width / 8;
			int num2 = height / 8;
			MTexture mtexture = GFX.Game["objects/moveBlock/base"];
			MTexture mtexture2 = GFX.Game["objects/moveBlock/button"];
			if (canSteer && (direction == MoveBlock.Directions.Left || direction == MoveBlock.Directions.Right))
			{
				for (int i = 0; i < num; i++)
				{
					int num3 = (i == 0) ? 0 : ((i < num - 1) ? 1 : 2);
					this.AddImage(mtexture2.GetSubtexture(num3 * 8, 0, 8, 8, null), new Vector2((float)(i * 8), -4f), 0f, new Vector2(1f, 1f), this.topButton);
				}
				mtexture = GFX.Game["objects/moveBlock/base_h"];
			}
			else if (canSteer && (direction == MoveBlock.Directions.Up || direction == MoveBlock.Directions.Down))
			{
				for (int j = 0; j < num2; j++)
				{
					int num4 = (j == 0) ? 0 : ((j < num2 - 1) ? 1 : 2);
					this.AddImage(mtexture2.GetSubtexture(num4 * 8, 0, 8, 8, null), new Vector2(-4f, (float)(j * 8)), 1.5707964f, new Vector2(1f, -1f), this.leftButton);
					this.AddImage(mtexture2.GetSubtexture(num4 * 8, 0, 8, 8, null), new Vector2((float)((num - 1) * 8 + 4), (float)(j * 8)), 1.5707964f, new Vector2(1f, 1f), this.rightButton);
				}
				mtexture = GFX.Game["objects/moveBlock/base_v"];
			}
			for (int k = 0; k < num; k++)
			{
				for (int l = 0; l < num2; l++)
				{
					int num5 = (k == 0) ? 0 : ((k < num - 1) ? 1 : 2);
					int num6 = (l == 0) ? 0 : ((l < num2 - 1) ? 1 : 2);
					this.AddImage(mtexture.GetSubtexture(num5 * 8, num6 * 8, 8, 8, null), new Vector2((float)k, (float)l) * 8f, 0f, new Vector2(1f, 1f), this.body);
				}
			}
			this.arrows = GFX.Game.GetAtlasSubtextures("objects/moveBlock/arrow");
			base.Add(this.moveSfx = new SoundSource());
			base.Add(new Coroutine(this.Controller(), true));
			this.UpdateColors();
			base.Add(new LightOcclude(0.5f));
		}

		// Token: 0x06001613 RID: 5651 RVA: 0x00080CCC File Offset: 0x0007EECC
		public MoveBlock(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height, data.Enum<MoveBlock.Directions>("direction", MoveBlock.Directions.Left), data.Bool("canSteer", true), data.Bool("fast", false))
		{
		}

		// Token: 0x06001614 RID: 5652 RVA: 0x00080D1C File Offset: 0x0007EF1C
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			scene.Add(this.border = new MoveBlock.Border(this));
		}

		// Token: 0x06001615 RID: 5653 RVA: 0x00080D45 File Offset: 0x0007EF45
		private IEnumerator Controller()
		{
			for (;;)
			{
				this.triggered = false;
				this.state = MoveBlock.MovementState.Idling;
				while (!this.triggered && !base.HasPlayerRider())
				{
					yield return null;
				}
				Audio.Play("event:/game/04_cliffside/arrowblock_activate", this.Position);
				this.state = MoveBlock.MovementState.Moving;
				base.StartShaking(0.2f);
				this.ActivateParticles();
				yield return 0.2f;
				this.targetSpeed = (this.fast ? 75f : 60f);
				this.moveSfx.Play("event:/game/04_cliffside/arrowblock_move", null, 0f);
				this.moveSfx.Param("arrow_stop", 0f);
				this.StopPlayerRunIntoAnimation = false;
				float crashTimer = 0.15f;
				float crashResetTimer = 0.1f;
				float noSteerTimer = 0.2f;
				for (;;)
				{
					if (this.canSteer)
					{
						this.targetAngle = this.homeAngle;
						bool flag;
						if (this.direction == MoveBlock.Directions.Right || this.direction == MoveBlock.Directions.Left)
						{
							flag = base.HasPlayerOnTop();
						}
						else
						{
							flag = base.HasPlayerClimbing();
						}
						if (flag && noSteerTimer > 0f)
						{
							noSteerTimer -= Engine.DeltaTime;
						}
						if (flag)
						{
							if (noSteerTimer <= 0f)
							{
								if (this.direction == MoveBlock.Directions.Right || this.direction == MoveBlock.Directions.Left)
								{
									this.targetAngle = this.homeAngle + 0.7853982f * (float)this.angleSteerSign * (float)Input.MoveY.Value;
								}
								else
								{
									this.targetAngle = this.homeAngle + 0.7853982f * (float)this.angleSteerSign * (float)Input.MoveX.Value;
								}
							}
						}
						else
						{
							noSteerTimer = 0.2f;
						}
					}
					if (base.Scene.OnInterval(0.02f))
					{
						this.MoveParticles();
					}
					this.speed = Calc.Approach(this.speed, this.targetSpeed, 300f * Engine.DeltaTime);
					this.angle = Calc.Approach(this.angle, this.targetAngle, 50.265484f * Engine.DeltaTime);
					Vector2 vector = Calc.AngleToVector(this.angle, this.speed);
					Vector2 vector2 = vector * Engine.DeltaTime;
					bool flag2;
					if (this.direction == MoveBlock.Directions.Right || this.direction == MoveBlock.Directions.Left)
					{
						flag2 = this.MoveCheck(vector2.XComp());
						this.noSquish = base.Scene.Tracker.GetEntity<Player>();
						base.MoveVCollideSolids(vector2.Y, false, null);
						this.noSquish = null;
						this.LiftSpeed = vector;
						if (base.Scene.OnInterval(0.03f))
						{
							if (vector2.Y > 0f)
							{
								this.ScrapeParticles(Vector2.UnitY);
							}
							else if (vector2.Y < 0f)
							{
								this.ScrapeParticles(-Vector2.UnitY);
							}
						}
					}
					else
					{
						flag2 = this.MoveCheck(vector2.YComp());
						this.noSquish = base.Scene.Tracker.GetEntity<Player>();
						base.MoveHCollideSolids(vector2.X, false, null);
						this.noSquish = null;
						this.LiftSpeed = vector;
						if (base.Scene.OnInterval(0.03f))
						{
							if (vector2.X > 0f)
							{
								this.ScrapeParticles(Vector2.UnitX);
							}
							else if (vector2.X < 0f)
							{
								this.ScrapeParticles(-Vector2.UnitX);
							}
						}
						if (this.direction == MoveBlock.Directions.Down && base.Top > (float)(base.SceneAs<Level>().Bounds.Bottom + 32))
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						this.moveSfx.Param("arrow_stop", 1f);
						crashResetTimer = 0.1f;
						if (crashTimer <= 0f)
						{
							break;
						}
						crashTimer -= Engine.DeltaTime;
					}
					else
					{
						this.moveSfx.Param("arrow_stop", 0f);
						if (crashResetTimer > 0f)
						{
							crashResetTimer -= Engine.DeltaTime;
						}
						else
						{
							crashTimer = 0.15f;
						}
					}
					Level level = base.Scene as Level;
					if (base.Left < (float)level.Bounds.Left || base.Top < (float)level.Bounds.Top || base.Right > (float)level.Bounds.Right)
					{
						break;
					}
					yield return null;
				}
				Audio.Play("event:/game/04_cliffside/arrowblock_break", this.Position);
				this.moveSfx.Stop(true);
				this.state = MoveBlock.MovementState.Breaking;
				this.speed = (this.targetSpeed = 0f);
				this.angle = (this.targetAngle = this.homeAngle);
				base.StartShaking(0.2f);
				this.StopPlayerRunIntoAnimation = true;
				yield return 0.2f;
				this.BreakParticles();
				List<MoveBlock.Debris> debris = new List<MoveBlock.Debris>();
				int num = 0;
				while ((float)num < base.Width)
				{
					int num2 = 0;
					while ((float)num2 < base.Height)
					{
						Vector2 value = new Vector2((float)num + 4f, (float)num2 + 4f);
						MoveBlock.Debris debris2 = Engine.Pooler.Create<MoveBlock.Debris>().Init(this.Position + value, base.Center, this.startPosition + value);
						debris.Add(debris2);
						base.Scene.Add(debris2);
						num2 += 8;
					}
					num += 8;
				}
				base.MoveStaticMovers(this.startPosition - this.Position);
				base.DisableStaticMovers();
				this.Position = this.startPosition;
				this.Visible = (this.Collidable = false);
				yield return 2.2f;
				using (List<MoveBlock.Debris>.Enumerator enumerator = debris.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MoveBlock.Debris debris3 = enumerator.Current;
						debris3.StopMoving();
					}
					goto IL_6BE;
				}
				goto IL_6A7;
				IL_6BE:
				if (!base.CollideCheck<Actor>() && !base.CollideCheck<Solid>())
				{
					this.Collidable = true;
					EventInstance instance = Audio.Play("event:/game/04_cliffside/arrowblock_reform_begin", debris[0].Position);
					Coroutine routine;
					base.Add(routine = new Coroutine(this.SoundFollowsDebrisCenter(instance, debris), true));
					foreach (MoveBlock.Debris debris4 in debris)
					{
						debris4.StartShaking();
					}
					yield return 0.2f;
					foreach (MoveBlock.Debris debris5 in debris)
					{
						debris5.ReturnHome(0.65f);
					}
					yield return 0.6f;
					routine.RemoveSelf();
					foreach (MoveBlock.Debris debris6 in debris)
					{
						debris6.RemoveSelf();
					}
					routine = null;
					Audio.Play("event:/game/04_cliffside/arrowblock_reappear", this.Position);
					this.Visible = true;
					base.EnableStaticMovers();
					this.speed = (this.targetSpeed = 0f);
					this.angle = (this.targetAngle = this.homeAngle);
					this.noSquish = null;
					this.fillColor = MoveBlock.idleBgFill;
					this.UpdateColors();
					this.flash = 1f;
					debris = null;
					continue;
				}
				IL_6A7:
				yield return null;
				goto IL_6BE;
			}
			yield break;
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x00080D54 File Offset: 0x0007EF54
		private IEnumerator SoundFollowsDebrisCenter(EventInstance instance, List<MoveBlock.Debris> debris)
		{
			for (;;)
			{
				PLAYBACK_STATE playback_STATE;
				instance.getPlaybackState(out playback_STATE);
				if (playback_STATE == PLAYBACK_STATE.STOPPED)
				{
					break;
				}
				Vector2 vector = Vector2.Zero;
				foreach (MoveBlock.Debris debris2 in debris)
				{
					vector += debris2.Position;
				}
				vector /= (float)debris.Count;
				Audio.Position(instance, vector);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001617 RID: 5655 RVA: 0x00080D6C File Offset: 0x0007EF6C
		public override void Update()
		{
			base.Update();
			if (this.canSteer)
			{
				bool flag = (this.direction == MoveBlock.Directions.Up || this.direction == MoveBlock.Directions.Down) && base.CollideCheck<Player>(this.Position + new Vector2(-1f, 0f));
				bool flag2 = (this.direction == MoveBlock.Directions.Up || this.direction == MoveBlock.Directions.Down) && base.CollideCheck<Player>(this.Position + new Vector2(1f, 0f));
				bool flag3 = (this.direction == MoveBlock.Directions.Left || this.direction == MoveBlock.Directions.Right) && base.CollideCheck<Player>(this.Position + new Vector2(0f, -1f));
				foreach (Image image in this.topButton)
				{
					image.Y = (float)(flag3 ? 2 : 0);
				}
				foreach (Image image2 in this.leftButton)
				{
					image2.X = (float)(flag ? 2 : 0);
				}
				foreach (Image image3 in this.rightButton)
				{
					image3.X = base.Width + (float)(flag2 ? -2 : 0);
				}
				if ((flag && !this.leftPressed) || (flag3 && !this.topPressed) || (flag2 && !this.rightPressed))
				{
					Audio.Play("event:/game/04_cliffside/arrowblock_side_depress", this.Position);
				}
				if ((!flag && this.leftPressed) || (!flag3 && this.topPressed) || (!flag2 && this.rightPressed))
				{
					Audio.Play("event:/game/04_cliffside/arrowblock_side_release", this.Position);
				}
				this.leftPressed = flag;
				this.rightPressed = flag2;
				this.topPressed = flag3;
			}
			if (this.moveSfx != null && this.moveSfx.Playing)
			{
				int num = (int)Math.Floor((double)((-(double)(Calc.AngleToVector(this.angle, 1f) * new Vector2(-1f, 1f)).Angle() + 6.2831855f) % 6.2831855f / 6.2831855f * 8f + 0.5f));
				this.moveSfx.Param("arrow_influence", (float)(num + 1));
			}
			this.border.Visible = this.Visible;
			this.flash = Calc.Approach(this.flash, 0f, Engine.DeltaTime * 5f);
			this.UpdateColors();
		}

		// Token: 0x06001618 RID: 5656 RVA: 0x0008103C File Offset: 0x0007F23C
		public override void OnStaticMoverTrigger(StaticMover sm)
		{
			this.triggered = true;
		}

		// Token: 0x06001619 RID: 5657 RVA: 0x00081048 File Offset: 0x0007F248
		public override void MoveHExact(int move)
		{
			if (this.noSquish != null)
			{
				if (move >= 0 || this.noSquish.X >= base.X)
				{
					if (move <= 0 || this.noSquish.X <= base.X)
					{
						goto IL_6E;
					}
				}
				while (move != 0 && this.noSquish.CollideCheck<Solid>(this.noSquish.Position + Vector2.UnitX * (float)move))
				{
					move -= Math.Sign(move);
				}
			}
			IL_6E:
			base.MoveHExact(move);
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x000810CC File Offset: 0x0007F2CC
		public override void MoveVExact(int move)
		{
			if (this.noSquish != null && move < 0 && this.noSquish.Y <= base.Y)
			{
				while (move != 0 && this.noSquish.CollideCheck<Solid>(this.noSquish.Position + Vector2.UnitY * (float)move))
				{
					move -= Math.Sign(move);
				}
			}
			base.MoveVExact(move);
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x00081138 File Offset: 0x0007F338
		private bool MoveCheck(Vector2 speed)
		{
			if (speed.X != 0f)
			{
				if (base.MoveHCollideSolids(speed.X, false, null))
				{
					for (int i = 1; i <= 3; i++)
					{
						for (int j = 1; j >= -1; j -= 2)
						{
							Vector2 value = new Vector2((float)Math.Sign(speed.X), (float)(i * j));
							if (!base.CollideCheck<Solid>(this.Position + value))
							{
								this.MoveVExact(i * j);
								this.MoveHExact(Math.Sign(speed.X));
								return false;
							}
						}
					}
					return true;
				}
				return false;
			}
			else
			{
				if (speed.Y == 0f)
				{
					return false;
				}
				if (base.MoveVCollideSolids(speed.Y, false, null))
				{
					for (int k = 1; k <= 3; k++)
					{
						for (int l = 1; l >= -1; l -= 2)
						{
							Vector2 value2 = new Vector2((float)(k * l), (float)Math.Sign(speed.Y));
							if (!base.CollideCheck<Solid>(this.Position + value2))
							{
								this.MoveHExact(k * l);
								this.MoveVExact(Math.Sign(speed.Y));
								return false;
							}
						}
					}
					return true;
				}
				return false;
			}
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x00081250 File Offset: 0x0007F450
		private void UpdateColors()
		{
			Color value = MoveBlock.idleBgFill;
			if (this.state == MoveBlock.MovementState.Moving)
			{
				value = MoveBlock.pressedBgFill;
			}
			else if (this.state == MoveBlock.MovementState.Breaking)
			{
				value = MoveBlock.breakingBgFill;
			}
			this.fillColor = Color.Lerp(this.fillColor, value, 10f * Engine.DeltaTime);
			foreach (Image image in this.topButton)
			{
				image.Color = this.fillColor;
			}
			foreach (Image image2 in this.leftButton)
			{
				image2.Color = this.fillColor;
			}
			foreach (Image image3 in this.rightButton)
			{
				image3.Color = this.fillColor;
			}
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x00081374 File Offset: 0x0007F574
		private void AddImage(MTexture tex, Vector2 position, float rotation, Vector2 scale, List<Image> addTo)
		{
			Image image = new Image(tex);
			image.Position = position + new Vector2(4f, 4f);
			image.CenterOrigin();
			image.Rotation = rotation;
			image.Scale = scale;
			base.Add(image);
			if (addTo != null)
			{
				addTo.Add(image);
			}
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x000813CC File Offset: 0x0007F5CC
		private void SetVisible(List<Image> images, bool visible)
		{
			foreach (Image image in images)
			{
				image.Visible = visible;
			}
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x00081418 File Offset: 0x0007F618
		public override void Render()
		{
			Vector2 position = this.Position;
			this.Position += base.Shake;
			foreach (Image image in this.leftButton)
			{
				image.Render();
			}
			foreach (Image image2 in this.rightButton)
			{
				image2.Render();
			}
			foreach (Image image3 in this.topButton)
			{
				image3.Render();
			}
			Draw.Rect(base.X + 3f, base.Y + 3f, base.Width - 6f, base.Height - 6f, this.fillColor);
			foreach (Image image4 in this.body)
			{
				image4.Render();
			}
			Draw.Rect(base.Center.X - 4f, base.Center.Y - 4f, 8f, 8f, this.fillColor);
			if (this.state != MoveBlock.MovementState.Breaking)
			{
				int value = (int)Math.Floor((double)((-(double)this.angle + 6.2831855f) % 6.2831855f / 6.2831855f * 8f + 0.5f));
				this.arrows[Calc.Clamp(value, 0, 7)].DrawCentered(base.Center);
			}
			else
			{
				GFX.Game["objects/moveBlock/x"].DrawCentered(base.Center);
			}
			float num = this.flash * 4f;
			Draw.Rect(base.X - num, base.Y - num, base.Width + num * 2f, base.Height + num * 2f, Color.White * this.flash);
			this.Position = position;
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x0008167C File Offset: 0x0007F87C
		private void ActivateParticles()
		{
			bool flag = this.direction == MoveBlock.Directions.Down || this.direction == MoveBlock.Directions.Up;
			bool flag2 = (!this.canSteer || !flag) && !base.CollideCheck<Player>(this.Position - Vector2.UnitX);
			bool flag3 = (!this.canSteer || !flag) && !base.CollideCheck<Player>(this.Position + Vector2.UnitX);
			bool flag4 = (!this.canSteer || flag) && !base.CollideCheck<Player>(this.Position - Vector2.UnitY);
			if (flag2)
			{
				base.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int)(base.Height / 2f), base.CenterLeft, Vector2.UnitY * (base.Height - 4f) * 0.5f, 3.1415927f);
			}
			if (flag3)
			{
				base.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int)(base.Height / 2f), base.CenterRight, Vector2.UnitY * (base.Height - 4f) * 0.5f, 0f);
			}
			if (flag4)
			{
				base.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int)(base.Width / 2f), base.TopCenter, Vector2.UnitX * (base.Width - 4f) * 0.5f, -1.5707964f);
			}
			base.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int)(base.Width / 2f), base.BottomCenter, Vector2.UnitX * (base.Width - 4f) * 0.5f, 1.5707964f);
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x00081854 File Offset: 0x0007FA54
		private void BreakParticles()
		{
			Vector2 center = base.Center;
			int num = 0;
			while ((float)num < base.Width)
			{
				int num2 = 0;
				while ((float)num2 < base.Height)
				{
					Vector2 vector = this.Position + new Vector2((float)(2 + num), (float)(2 + num2));
					base.SceneAs<Level>().Particles.Emit(MoveBlock.P_Break, 1, vector, Vector2.One * 2f, (vector - center).Angle());
					num2 += 4;
				}
				num += 4;
			}
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x000818D8 File Offset: 0x0007FAD8
		private void MoveParticles()
		{
			Vector2 position;
			Vector2 vector;
			float num;
			float num2;
			if (this.direction == MoveBlock.Directions.Right)
			{
				position = base.CenterLeft + Vector2.UnitX;
				vector = Vector2.UnitY * (base.Height - 4f);
				num = 3.1415927f;
				num2 = base.Height / 32f;
			}
			else if (this.direction == MoveBlock.Directions.Left)
			{
				position = base.CenterRight;
				vector = Vector2.UnitY * (base.Height - 4f);
				num = 0f;
				num2 = base.Height / 32f;
			}
			else if (this.direction == MoveBlock.Directions.Down)
			{
				position = base.TopCenter + Vector2.UnitY;
				vector = Vector2.UnitX * (base.Width - 4f);
				num = -1.5707964f;
				num2 = base.Width / 32f;
			}
			else
			{
				position = base.BottomCenter;
				vector = Vector2.UnitX * (base.Width - 4f);
				num = 1.5707964f;
				num2 = base.Width / 32f;
			}
			this.particleRemainder += num2;
			int num3 = (int)this.particleRemainder;
			this.particleRemainder -= (float)num3;
			vector *= 0.5f;
			if (num3 > 0)
			{
				base.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Move, num3, position, vector, num);
			}
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x00081A34 File Offset: 0x0007FC34
		private void ScrapeParticles(Vector2 dir)
		{
			bool collidable = this.Collidable;
			this.Collidable = false;
			if (dir.X != 0f)
			{
				float x;
				if (dir.X > 0f)
				{
					x = base.Right;
				}
				else
				{
					x = base.Left - 1f;
				}
				int num = 0;
				while ((float)num < base.Height)
				{
					Vector2 vector = new Vector2(x, base.Top + 4f + (float)num);
					if (base.Scene.CollideCheck<Solid>(vector))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, vector);
					}
					num += 8;
				}
			}
			else
			{
				float y;
				if (dir.Y > 0f)
				{
					y = base.Bottom;
				}
				else
				{
					y = base.Top - 1f;
				}
				int num2 = 0;
				while ((float)num2 < base.Width)
				{
					Vector2 vector2 = new Vector2(base.Left + 4f + (float)num2, y);
					if (base.Scene.CollideCheck<Solid>(vector2))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, vector2);
					}
					num2 += 8;
				}
			}
			this.Collidable = true;
		}

		// Token: 0x04001258 RID: 4696
		public static ParticleType P_Activate;

		// Token: 0x04001259 RID: 4697
		public static ParticleType P_Break;

		// Token: 0x0400125A RID: 4698
		public static ParticleType P_Move;

		// Token: 0x0400125B RID: 4699
		private const float Accel = 300f;

		// Token: 0x0400125C RID: 4700
		private const float MoveSpeed = 60f;

		// Token: 0x0400125D RID: 4701
		private const float FastMoveSpeed = 75f;

		// Token: 0x0400125E RID: 4702
		private const float SteerSpeed = 50.265484f;

		// Token: 0x0400125F RID: 4703
		private const float MaxAngle = 0.7853982f;

		// Token: 0x04001260 RID: 4704
		private const float NoSteerTime = 0.2f;

		// Token: 0x04001261 RID: 4705
		private const float CrashTime = 0.15f;

		// Token: 0x04001262 RID: 4706
		private const float CrashResetTime = 0.1f;

		// Token: 0x04001263 RID: 4707
		private const float RegenTime = 3f;

		// Token: 0x04001264 RID: 4708
		private bool canSteer;

		// Token: 0x04001265 RID: 4709
		private bool fast;

		// Token: 0x04001266 RID: 4710
		private MoveBlock.Directions direction;

		// Token: 0x04001267 RID: 4711
		private float homeAngle;

		// Token: 0x04001268 RID: 4712
		private int angleSteerSign;

		// Token: 0x04001269 RID: 4713
		private Vector2 startPosition;

		// Token: 0x0400126A RID: 4714
		private MoveBlock.MovementState state;

		// Token: 0x0400126B RID: 4715
		private bool leftPressed;

		// Token: 0x0400126C RID: 4716
		private bool rightPressed;

		// Token: 0x0400126D RID: 4717
		private bool topPressed;

		// Token: 0x0400126E RID: 4718
		private float speed;

		// Token: 0x0400126F RID: 4719
		private float targetSpeed;

		// Token: 0x04001270 RID: 4720
		private float angle;

		// Token: 0x04001271 RID: 4721
		private float targetAngle;

		// Token: 0x04001272 RID: 4722
		private Player noSquish;

		// Token: 0x04001273 RID: 4723
		private List<Image> body = new List<Image>();

		// Token: 0x04001274 RID: 4724
		private List<Image> topButton = new List<Image>();

		// Token: 0x04001275 RID: 4725
		private List<Image> leftButton = new List<Image>();

		// Token: 0x04001276 RID: 4726
		private List<Image> rightButton = new List<Image>();

		// Token: 0x04001277 RID: 4727
		private List<MTexture> arrows = new List<MTexture>();

		// Token: 0x04001278 RID: 4728
		private MoveBlock.Border border;

		// Token: 0x04001279 RID: 4729
		private Color fillColor = MoveBlock.idleBgFill;

		// Token: 0x0400127A RID: 4730
		private float flash;

		// Token: 0x0400127B RID: 4731
		private SoundSource moveSfx;

		// Token: 0x0400127C RID: 4732
		private bool triggered;

		// Token: 0x0400127D RID: 4733
		private static readonly Color idleBgFill = Calc.HexToColor("474070");

		// Token: 0x0400127E RID: 4734
		private static readonly Color pressedBgFill = Calc.HexToColor("30b335");

		// Token: 0x0400127F RID: 4735
		private static readonly Color breakingBgFill = Calc.HexToColor("cc2541");

		// Token: 0x04001280 RID: 4736
		private float particleRemainder;

		// Token: 0x02000656 RID: 1622
		public enum Directions
		{
			// Token: 0x04002A21 RID: 10785
			Left,
			// Token: 0x04002A22 RID: 10786
			Right,
			// Token: 0x04002A23 RID: 10787
			Up,
			// Token: 0x04002A24 RID: 10788
			Down
		}

		// Token: 0x02000657 RID: 1623
		private enum MovementState
		{
			// Token: 0x04002A26 RID: 10790
			Idling,
			// Token: 0x04002A27 RID: 10791
			Moving,
			// Token: 0x04002A28 RID: 10792
			Breaking
		}

		// Token: 0x02000658 RID: 1624
		private class Border : Entity
		{
			// Token: 0x06002B23 RID: 11043 RVA: 0x001130F8 File Offset: 0x001112F8
			public Border(MoveBlock parent)
			{
				this.Parent = parent;
				base.Depth = 1;
			}

			// Token: 0x06002B24 RID: 11044 RVA: 0x0011310E File Offset: 0x0011130E
			public override void Update()
			{
				if (this.Parent.Scene != base.Scene)
				{
					base.RemoveSelf();
				}
				base.Update();
			}

			// Token: 0x06002B25 RID: 11045 RVA: 0x00113130 File Offset: 0x00111330
			public override void Render()
			{
				Draw.Rect(this.Parent.X + this.Parent.Shake.X - 1f, this.Parent.Y + this.Parent.Shake.Y - 1f, this.Parent.Width + 2f, this.Parent.Height + 2f, Color.Black);
			}

			// Token: 0x04002A29 RID: 10793
			public MoveBlock Parent;
		}

		// Token: 0x02000659 RID: 1625
		[Pooled]
		private class Debris : Actor
		{
			// Token: 0x06002B26 RID: 11046 RVA: 0x001131B0 File Offset: 0x001113B0
			public Debris() : base(Vector2.Zero)
			{
				base.Tag = Tags.TransitionUpdate;
				base.Collider = new Hitbox(4f, 4f, -2f, -2f);
				base.Add(this.sprite = new Image(Calc.Random.Choose(GFX.Game.GetAtlasSubtextures("objects/moveblock/debris"))));
				this.sprite.CenterOrigin();
				this.sprite.FlipX = Calc.Random.Chance(0.5f);
				this.onCollideH = delegate(CollisionData c)
				{
					this.speed.X = -this.speed.X * 0.5f;
				};
				this.onCollideV = delegate(CollisionData c)
				{
					if (this.firstHit || this.speed.Y > 50f)
					{
						Audio.Play("event:/game/general/debris_stone", this.Position, "debris_velocity", Calc.ClampedMap(this.speed.Y, 0f, 600f, 0f, 1f));
					}
					if (this.speed.Y > 0f && this.speed.Y < 40f)
					{
						this.speed.Y = 0f;
					}
					else
					{
						this.speed.Y = -this.speed.Y * 0.25f;
					}
					this.firstHit = false;
				};
			}

			// Token: 0x06002B27 RID: 11047 RVA: 0x000091E2 File Offset: 0x000073E2
			protected override void OnSquish(CollisionData data)
			{
			}

			// Token: 0x06002B28 RID: 11048 RVA: 0x00113270 File Offset: 0x00111470
			public MoveBlock.Debris Init(Vector2 position, Vector2 center, Vector2 returnTo)
			{
				this.Collidable = true;
				this.Position = position;
				this.speed = (position - center).SafeNormalize(60f + Calc.Random.NextFloat(60f));
				this.home = returnTo;
				this.sprite.Position = Vector2.Zero;
				this.sprite.Rotation = Calc.Random.NextAngle();
				this.returning = false;
				this.shaking = false;
				this.sprite.Scale.X = 1f;
				this.sprite.Scale.Y = 1f;
				this.sprite.Color = Color.White;
				this.alpha = 1f;
				this.firstHit = false;
				this.spin = Calc.Random.Range(3.4906585f, 10.471975f) * (float)Calc.Random.Choose(1, -1);
				return this;
			}

			// Token: 0x06002B29 RID: 11049 RVA: 0x00113364 File Offset: 0x00111564
			public override void Update()
			{
				base.Update();
				if (!this.returning)
				{
					if (this.Collidable)
					{
						this.speed.X = Calc.Approach(this.speed.X, 0f, Engine.DeltaTime * 100f);
						if (!base.OnGround(1))
						{
							this.speed.Y = this.speed.Y + 400f * Engine.DeltaTime;
						}
						base.MoveH(this.speed.X * Engine.DeltaTime, this.onCollideH, null);
						base.MoveV(this.speed.Y * Engine.DeltaTime, this.onCollideV, null);
					}
					if (this.shaking && base.Scene.OnInterval(0.05f))
					{
						this.sprite.X = (float)(-1 + Calc.Random.Next(3));
						this.sprite.Y = (float)(-1 + Calc.Random.Next(3));
					}
				}
				else
				{
					this.Position = this.returnCurve.GetPoint(Ease.CubeOut(this.returnEase));
					this.returnEase = Calc.Approach(this.returnEase, 1f, Engine.DeltaTime / this.returnDuration);
					this.sprite.Scale = Vector2.One * (1f + this.returnEase * 0.5f);
				}
				if ((base.Scene as Level).Transitioning)
				{
					this.alpha = Calc.Approach(this.alpha, 0f, Engine.DeltaTime * 4f);
					this.sprite.Color = Color.White * this.alpha;
				}
				this.sprite.Rotation += this.spin * Calc.ClampedMap(Math.Abs(this.speed.Y), 50f, 150f, 0f, 1f) * Engine.DeltaTime;
			}

			// Token: 0x06002B2A RID: 11050 RVA: 0x0011356D File Offset: 0x0011176D
			public void StopMoving()
			{
				this.Collidable = false;
			}

			// Token: 0x06002B2B RID: 11051 RVA: 0x00113576 File Offset: 0x00111776
			public void StartShaking()
			{
				this.shaking = true;
			}

			// Token: 0x06002B2C RID: 11052 RVA: 0x00113580 File Offset: 0x00111780
			public void ReturnHome(float duration)
			{
				if (base.Scene != null)
				{
					Camera camera = (base.Scene as Level).Camera;
					if (base.X < camera.X)
					{
						base.X = camera.X - 8f;
					}
					if (base.Y < camera.Y)
					{
						base.Y = camera.Y - 8f;
					}
					if (base.X > camera.X + 320f)
					{
						base.X = camera.X + 320f + 8f;
					}
					if (base.Y > camera.Y + 180f)
					{
						base.Y = camera.Y + 180f + 8f;
					}
				}
				this.returning = true;
				this.returnEase = 0f;
				this.returnDuration = duration;
				Vector2 vector = (this.home - this.Position).SafeNormalize();
				Vector2 control = (this.Position + this.home) / 2f + new Vector2(vector.Y, -vector.X) * (Calc.Random.NextFloat(16f) + 16f) * (float)Calc.Random.Facing();
				this.returnCurve = new SimpleCurve(this.Position, this.home, control);
			}

			// Token: 0x04002A2A RID: 10794
			private Image sprite;

			// Token: 0x04002A2B RID: 10795
			private Vector2 home;

			// Token: 0x04002A2C RID: 10796
			private Vector2 speed;

			// Token: 0x04002A2D RID: 10797
			private bool shaking;

			// Token: 0x04002A2E RID: 10798
			private bool returning;

			// Token: 0x04002A2F RID: 10799
			private float returnEase;

			// Token: 0x04002A30 RID: 10800
			private float returnDuration;

			// Token: 0x04002A31 RID: 10801
			private SimpleCurve returnCurve;

			// Token: 0x04002A32 RID: 10802
			private bool firstHit;

			// Token: 0x04002A33 RID: 10803
			private float alpha;

			// Token: 0x04002A34 RID: 10804
			private Collision onCollideH;

			// Token: 0x04002A35 RID: 10805
			private Collision onCollideV;

			// Token: 0x04002A36 RID: 10806
			private float spin;
		}
	}
}
