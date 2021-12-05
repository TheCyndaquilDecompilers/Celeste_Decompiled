using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200014B RID: 331
	public class CrushBlock : Solid
	{
		// Token: 0x06000C0B RID: 3083 RVA: 0x00025810 File Offset: 0x00023A10
		public CrushBlock(Vector2 position, float width, float height, CrushBlock.Axes axes, bool chillOut = false) : base(position, width, height, false)
		{
			this.OnDashCollide = new DashCollision(this.OnDashed);
			this.returnStack = new List<CrushBlock.MoveState>();
			this.chillOut = chillOut;
			this.giant = (base.Width >= 48f && base.Height >= 48f && chillOut);
			this.canActivate = true;
			this.attackCoroutine = new Coroutine(true);
			this.attackCoroutine.RemoveOnComplete = false;
			base.Add(this.attackCoroutine);
			List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/crushblock/block");
			MTexture idle;
			switch (axes)
			{
			default:
				idle = atlasSubtextures[3];
				this.canMoveHorizontally = (this.canMoveVertically = true);
				break;
			case CrushBlock.Axes.Horizontal:
				idle = atlasSubtextures[1];
				this.canMoveHorizontally = true;
				this.canMoveVertically = false;
				break;
			case CrushBlock.Axes.Vertical:
				idle = atlasSubtextures[2];
				this.canMoveHorizontally = false;
				this.canMoveVertically = true;
				break;
			}
			base.Add(this.face = GFX.SpriteBank.Create(this.giant ? "giant_crushblock_face" : "crushblock_face"));
			this.face.Position = new Vector2(base.Width, base.Height) / 2f;
			this.face.Play("idle", false, false);
			this.face.OnLastFrame = delegate(string f)
			{
				if (f == "hit")
				{
					this.face.Play(this.nextFaceDirection, false, false);
				}
			};
			int num = (int)(base.Width / 8f) - 1;
			int num2 = (int)(base.Height / 8f) - 1;
			this.AddImage(idle, 0, 0, 0, 0, -1, -1);
			this.AddImage(idle, num, 0, 3, 0, 1, -1);
			this.AddImage(idle, 0, num2, 0, 3, -1, 1);
			this.AddImage(idle, num, num2, 3, 3, 1, 1);
			for (int i = 1; i < num; i++)
			{
				this.AddImage(idle, i, 0, Calc.Random.Choose(1, 2), 0, 0, -1);
				this.AddImage(idle, i, num2, Calc.Random.Choose(1, 2), 3, 0, 1);
			}
			for (int j = 1; j < num2; j++)
			{
				this.AddImage(idle, 0, j, 0, Calc.Random.Choose(1, 2), -1, 0);
				this.AddImage(idle, num, j, 3, Calc.Random.Choose(1, 2), 1, 0);
			}
			base.Add(new LightOcclude(0.2f));
			base.Add(this.returnLoopSfx = new SoundSource());
			base.Add(new WaterInteraction(() => this.crushDir != Vector2.Zero));
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x00025AED File Offset: 0x00023CED
		public CrushBlock(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height, data.Enum<CrushBlock.Axes>("axes", CrushBlock.Axes.Both), data.Bool("chillout", false))
		{
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x00025B27 File Offset: 0x00023D27
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x00025B3C File Offset: 0x00023D3C
		public override void Update()
		{
			base.Update();
			if (this.crushDir == Vector2.Zero)
			{
				this.face.Position = new Vector2(base.Width, base.Height) / 2f;
				if (base.CollideCheck<Player>(this.Position + new Vector2(-1f, 0f)))
				{
					this.face.X -= 1f;
				}
				else if (base.CollideCheck<Player>(this.Position + new Vector2(1f, 0f)))
				{
					this.face.X += 1f;
				}
				else if (base.CollideCheck<Player>(this.Position + new Vector2(0f, -1f)))
				{
					this.face.Y -= 1f;
				}
			}
			if (this.currentMoveLoopSfx != null)
			{
				this.currentMoveLoopSfx.Param("submerged", (float)(this.Submerged ? 1 : 0));
			}
			if (this.returnLoopSfx != null)
			{
				this.returnLoopSfx.Param("submerged", (float)(this.Submerged ? 1 : 0));
			}
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x00025C88 File Offset: 0x00023E88
		public override void Render()
		{
			Vector2 position = this.Position;
			this.Position += base.Shake;
			Draw.Rect(base.X + 2f, base.Y + 2f, base.Width - 4f, base.Height - 4f, this.fill);
			base.Render();
			this.Position = position;
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000C10 RID: 3088 RVA: 0x00025CFB File Offset: 0x00023EFB
		private bool Submerged
		{
			get
			{
				return base.Scene.CollideCheck<Water>(new Rectangle((int)(base.Center.X - 4f), (int)base.Center.Y, 8, 4));
			}
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x00025D30 File Offset: 0x00023F30
		private void AddImage(MTexture idle, int x, int y, int tx, int ty, int borderX = 0, int borderY = 0)
		{
			MTexture subtexture = idle.GetSubtexture(tx * 8, ty * 8, 8, 8, null);
			Vector2 vector = new Vector2((float)(x * 8), (float)(y * 8));
			if (borderX != 0)
			{
				base.Add(new Image(subtexture)
				{
					Color = Color.Black,
					Position = vector + new Vector2((float)borderX, 0f)
				});
			}
			if (borderY != 0)
			{
				base.Add(new Image(subtexture)
				{
					Color = Color.Black,
					Position = vector + new Vector2(0f, (float)borderY)
				});
			}
			Image image = new Image(subtexture);
			image.Position = vector;
			base.Add(image);
			this.idleImages.Add(image);
			if (borderX != 0 || borderY != 0)
			{
				if (borderX < 0)
				{
					Image image2 = new Image(GFX.Game["objects/crushblock/lit_left"].GetSubtexture(0, ty * 8, 8, 8, null));
					this.activeLeftImages.Add(image2);
					image2.Position = vector;
					image2.Visible = false;
					base.Add(image2);
				}
				else if (borderX > 0)
				{
					Image image3 = new Image(GFX.Game["objects/crushblock/lit_right"].GetSubtexture(0, ty * 8, 8, 8, null));
					this.activeRightImages.Add(image3);
					image3.Position = vector;
					image3.Visible = false;
					base.Add(image3);
				}
				if (borderY < 0)
				{
					Image image4 = new Image(GFX.Game["objects/crushblock/lit_top"].GetSubtexture(tx * 8, 0, 8, 8, null));
					this.activeTopImages.Add(image4);
					image4.Position = vector;
					image4.Visible = false;
					base.Add(image4);
					return;
				}
				if (borderY > 0)
				{
					Image image5 = new Image(GFX.Game["objects/crushblock/lit_bottom"].GetSubtexture(tx * 8, 0, 8, 8, null));
					this.activeBottomImages.Add(image5);
					image5.Position = vector;
					image5.Visible = false;
					base.Add(image5);
				}
			}
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x00025F34 File Offset: 0x00024134
		private void TurnOffImages()
		{
			foreach (Image image in this.activeLeftImages)
			{
				image.Visible = false;
			}
			foreach (Image image2 in this.activeRightImages)
			{
				image2.Visible = false;
			}
			foreach (Image image3 in this.activeTopImages)
			{
				image3.Visible = false;
			}
			foreach (Image image4 in this.activeBottomImages)
			{
				image4.Visible = false;
			}
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x00026048 File Offset: 0x00024248
		private DashCollisionResults OnDashed(Player player, Vector2 direction)
		{
			if (this.CanActivate(-direction))
			{
				this.Attack(-direction);
				return DashCollisionResults.Rebound;
			}
			return DashCollisionResults.NormalCollision;
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x00026068 File Offset: 0x00024268
		private bool CanActivate(Vector2 direction)
		{
			return (!this.giant || direction.X > 0f) && (this.canActivate && this.crushDir != direction) && (direction.X == 0f || this.canMoveHorizontally) && (direction.Y == 0f || this.canMoveVertically);
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x000260D4 File Offset: 0x000242D4
		private void Attack(Vector2 direction)
		{
			Audio.Play("event:/game/06_reflection/crushblock_activate", base.Center);
			if (this.currentMoveLoopSfx != null)
			{
				this.currentMoveLoopSfx.Param("end", 1f);
				SoundSource sfx = this.currentMoveLoopSfx;
				Alarm.Set(this, 0.5f, delegate
				{
					sfx.RemoveSelf();
				}, Alarm.AlarmMode.Oneshot);
			}
			base.Add(this.currentMoveLoopSfx = new SoundSource());
			this.currentMoveLoopSfx.Position = new Vector2(base.Width, base.Height) / 2f;
			if (SaveData.Instance != null && SaveData.Instance.Name != null && SaveData.Instance.Name.StartsWith("FWAHAHA", StringComparison.InvariantCultureIgnoreCase))
			{
				this.currentMoveLoopSfx.Play("event:/game/06_reflection/crushblock_move_loop_covert", null, 0f);
			}
			else
			{
				this.currentMoveLoopSfx.Play("event:/game/06_reflection/crushblock_move_loop", null, 0f);
			}
			this.face.Play("hit", false, false);
			this.crushDir = direction;
			this.canActivate = false;
			this.attackCoroutine.Replace(this.AttackSequence());
			base.ClearRemainder();
			this.TurnOffImages();
			this.ActivateParticles(this.crushDir);
			if (this.crushDir.X < 0f)
			{
				foreach (Image image in this.activeLeftImages)
				{
					image.Visible = true;
				}
				this.nextFaceDirection = "left";
			}
			else if (this.crushDir.X > 0f)
			{
				foreach (Image image2 in this.activeRightImages)
				{
					image2.Visible = true;
				}
				this.nextFaceDirection = "right";
			}
			else if (this.crushDir.Y < 0f)
			{
				foreach (Image image3 in this.activeTopImages)
				{
					image3.Visible = true;
				}
				this.nextFaceDirection = "up";
			}
			else if (this.crushDir.Y > 0f)
			{
				foreach (Image image4 in this.activeBottomImages)
				{
					image4.Visible = true;
				}
				this.nextFaceDirection = "down";
			}
			bool flag = true;
			if (this.returnStack.Count > 0)
			{
				CrushBlock.MoveState moveState = this.returnStack[this.returnStack.Count - 1];
				if (moveState.Direction == direction || moveState.Direction == -direction)
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.returnStack.Add(new CrushBlock.MoveState(this.Position, this.crushDir));
			}
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x0002640C File Offset: 0x0002460C
		private void ActivateParticles(Vector2 dir)
		{
			float direction;
			Vector2 position;
			Vector2 positionRange;
			int num;
			if (dir == Vector2.UnitX)
			{
				direction = 0f;
				position = base.CenterRight - Vector2.UnitX;
				positionRange = Vector2.UnitY * (base.Height - 2f) * 0.5f;
				num = (int)(base.Height / 8f) * 4;
			}
			else if (dir == -Vector2.UnitX)
			{
				direction = 3.1415927f;
				position = base.CenterLeft + Vector2.UnitX;
				positionRange = Vector2.UnitY * (base.Height - 2f) * 0.5f;
				num = (int)(base.Height / 8f) * 4;
			}
			else if (dir == Vector2.UnitY)
			{
				direction = 1.5707964f;
				position = base.BottomCenter - Vector2.UnitY;
				positionRange = Vector2.UnitX * (base.Width - 2f) * 0.5f;
				num = (int)(base.Width / 8f) * 4;
			}
			else
			{
				direction = -1.5707964f;
				position = base.TopCenter + Vector2.UnitY;
				positionRange = Vector2.UnitX * (base.Width - 2f) * 0.5f;
				num = (int)(base.Width / 8f) * 4;
			}
			num += 2;
			this.level.Particles.Emit(CrushBlock.P_Activate, num, position, positionRange, direction);
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x0002658E File Offset: 0x0002478E
		private IEnumerator AttackSequence()
		{
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			base.StartShaking(0.4f);
			yield return 0.4f;
			if (!this.chillOut)
			{
				this.canActivate = true;
			}
			this.StopPlayerRunIntoAnimation = false;
			bool slowing = false;
			float speed = 0f;
			Action <>9__1;
			for (;;)
			{
				if (!this.chillOut)
				{
					speed = Calc.Approach(speed, 240f, 500f * Engine.DeltaTime);
				}
				else if (slowing || base.CollideCheck<SolidTiles>(this.Position + this.crushDir * 256f))
				{
					speed = Calc.Approach(speed, 24f, 500f * Engine.DeltaTime * 0.25f);
					if (!slowing)
					{
						slowing = true;
						float duration = 0.5f;
						Action onComplete;
						if ((onComplete = <>9__1) == null)
						{
							onComplete = (<>9__1 = delegate()
							{
								this.face.Play("hurt", false, false);
								this.currentMoveLoopSfx.Stop(true);
								this.TurnOffImages();
							});
						}
						Alarm.Set(this, duration, onComplete, Alarm.AlarmMode.Oneshot);
					}
				}
				else
				{
					speed = Calc.Approach(speed, 240f, 500f * Engine.DeltaTime);
				}
				bool flag;
				if (this.crushDir.X != 0f)
				{
					flag = this.MoveHCheck(speed * this.crushDir.X * Engine.DeltaTime);
				}
				else
				{
					flag = this.MoveVCheck(speed * this.crushDir.Y * Engine.DeltaTime);
				}
				if (base.Top >= (float)(this.level.Bounds.Bottom + 32))
				{
					break;
				}
				if (flag)
				{
					goto Block_8;
				}
				if (base.Scene.OnInterval(0.02f))
				{
					Vector2 position;
					float direction;
					if (this.crushDir == Vector2.UnitX)
					{
						position = new Vector2(base.Left + 1f, Calc.Random.Range(base.Top + 3f, base.Bottom - 3f));
						direction = 3.1415927f;
					}
					else if (this.crushDir == -Vector2.UnitX)
					{
						position = new Vector2(base.Right - 1f, Calc.Random.Range(base.Top + 3f, base.Bottom - 3f));
						direction = 0f;
					}
					else if (this.crushDir == Vector2.UnitY)
					{
						position = new Vector2(Calc.Random.Range(base.Left + 3f, base.Right - 3f), base.Top + 1f);
						direction = -1.5707964f;
					}
					else
					{
						position = new Vector2(Calc.Random.Range(base.Left + 3f, base.Right - 3f), base.Bottom - 1f);
						direction = 1.5707964f;
					}
					this.level.Particles.Emit(CrushBlock.P_Crushing, position, direction);
				}
				yield return null;
			}
			base.RemoveSelf();
			yield break;
			Block_8:
			FallingBlock fallingBlock = base.CollideFirst<FallingBlock>(this.Position + this.crushDir);
			if (fallingBlock != null)
			{
				fallingBlock.Triggered = true;
			}
			if (this.crushDir == -Vector2.UnitX)
			{
				Vector2 value = new Vector2(0f, 2f);
				int num = 0;
				while ((float)num < base.Height / 8f)
				{
					Vector2 vector = new Vector2(base.Left - 1f, base.Top + 4f + (float)(num * 8));
					if (!base.Scene.CollideCheck<Water>(vector) && base.Scene.CollideCheck<Solid>(vector))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector + value, 0f);
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector - value, 0f);
					}
					num++;
				}
			}
			else if (this.crushDir == Vector2.UnitX)
			{
				Vector2 value2 = new Vector2(0f, 2f);
				int num2 = 0;
				while ((float)num2 < base.Height / 8f)
				{
					Vector2 vector2 = new Vector2(base.Right + 1f, base.Top + 4f + (float)(num2 * 8));
					if (!base.Scene.CollideCheck<Water>(vector2) && base.Scene.CollideCheck<Solid>(vector2))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector2 + value2, 3.1415927f);
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector2 - value2, 3.1415927f);
					}
					num2++;
				}
			}
			else if (this.crushDir == -Vector2.UnitY)
			{
				Vector2 value3 = new Vector2(2f, 0f);
				int num3 = 0;
				while ((float)num3 < base.Width / 8f)
				{
					Vector2 vector3 = new Vector2(base.Left + 4f + (float)(num3 * 8), base.Top - 1f);
					if (!base.Scene.CollideCheck<Water>(vector3) && base.Scene.CollideCheck<Solid>(vector3))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector3 + value3, 1.5707964f);
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector3 - value3, 1.5707964f);
					}
					num3++;
				}
			}
			else if (this.crushDir == Vector2.UnitY)
			{
				Vector2 value4 = new Vector2(2f, 0f);
				int num4 = 0;
				while ((float)num4 < base.Width / 8f)
				{
					Vector2 vector4 = new Vector2(base.Left + 4f + (float)(num4 * 8), base.Bottom + 1f);
					if (!base.Scene.CollideCheck<Water>(vector4) && base.Scene.CollideCheck<Solid>(vector4))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector4 + value4, -1.5707964f);
						base.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector4 - value4, -1.5707964f);
					}
					num4++;
				}
			}
			Audio.Play("event:/game/06_reflection/crushblock_impact", base.Center);
			this.level.DirectionalShake(this.crushDir, 0.3f);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			base.StartShaking(0.4f);
			this.StopPlayerRunIntoAnimation = true;
			SoundSource sfx = this.currentMoveLoopSfx;
			this.currentMoveLoopSfx.Param("end", 1f);
			this.currentMoveLoopSfx = null;
			Alarm.Set(this, 0.5f, delegate
			{
				sfx.RemoveSelf();
			}, Alarm.AlarmMode.Oneshot);
			this.crushDir = Vector2.Zero;
			this.TurnOffImages();
			if (!this.chillOut)
			{
				this.face.Play("hurt", false, false);
				this.returnLoopSfx.Play("event:/game/06_reflection/crushblock_return_loop", null, 0f);
				yield return 0.4f;
				speed = 0f;
				float waypointSfxDelay = 0f;
				while (this.returnStack.Count > 0)
				{
					yield return null;
					this.StopPlayerRunIntoAnimation = false;
					CrushBlock.MoveState moveState = this.returnStack[this.returnStack.Count - 1];
					speed = Calc.Approach(speed, 60f, 160f * Engine.DeltaTime);
					waypointSfxDelay -= Engine.DeltaTime;
					if (moveState.Direction.X != 0f)
					{
						base.MoveTowardsX(moveState.From.X, speed * Engine.DeltaTime);
					}
					if (moveState.Direction.Y != 0f)
					{
						base.MoveTowardsY(moveState.From.Y, speed * Engine.DeltaTime);
					}
					if ((moveState.Direction.X == 0f || base.ExactPosition.X == moveState.From.X) && (moveState.Direction.Y == 0f || base.ExactPosition.Y == moveState.From.Y))
					{
						speed = 0f;
						this.returnStack.RemoveAt(this.returnStack.Count - 1);
						this.StopPlayerRunIntoAnimation = true;
						if (this.returnStack.Count <= 0)
						{
							this.face.Play("idle", false, false);
							this.returnLoopSfx.Stop(true);
							if (waypointSfxDelay <= 0f)
							{
								Audio.Play("event:/game/06_reflection/crushblock_rest", base.Center);
							}
						}
						else if (waypointSfxDelay <= 0f)
						{
							Audio.Play("event:/game/06_reflection/crushblock_rest_waypoint", base.Center);
						}
						waypointSfxDelay = 0.1f;
						base.StartShaking(0.2f);
						yield return 0.2f;
					}
				}
			}
			yield break;
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x000265A0 File Offset: 0x000247A0
		private bool MoveHCheck(float amount)
		{
			if (!base.MoveHCollideSolidsAndBounds(this.level, amount, true, null))
			{
				return false;
			}
			if (amount < 0f && base.Left <= (float)this.level.Bounds.Left)
			{
				return true;
			}
			if (amount > 0f && base.Right >= (float)this.level.Bounds.Right)
			{
				return true;
			}
			for (int i = 1; i <= 4; i++)
			{
				for (int j = 1; j >= -1; j -= 2)
				{
					Vector2 value = new Vector2((float)Math.Sign(amount), (float)(i * j));
					if (!base.CollideCheck<Solid>(this.Position + value))
					{
						this.MoveVExact(i * j);
						this.MoveHExact(Math.Sign(amount));
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x00026668 File Offset: 0x00024868
		private bool MoveVCheck(float amount)
		{
			if (!base.MoveVCollideSolidsAndBounds(this.level, amount, true, null, false))
			{
				return false;
			}
			if (amount < 0f && base.Top <= (float)this.level.Bounds.Top)
			{
				return true;
			}
			for (int i = 1; i <= 4; i++)
			{
				for (int j = 1; j >= -1; j -= 2)
				{
					Vector2 value = new Vector2((float)(i * j), (float)Math.Sign(amount));
					if (!base.CollideCheck<Solid>(this.Position + value))
					{
						this.MoveHExact(i * j);
						this.MoveVExact(Math.Sign(amount));
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0400075A RID: 1882
		public static ParticleType P_Impact;

		// Token: 0x0400075B RID: 1883
		public static ParticleType P_Crushing;

		// Token: 0x0400075C RID: 1884
		public static ParticleType P_Activate;

		// Token: 0x0400075D RID: 1885
		private const float CrushSpeed = 240f;

		// Token: 0x0400075E RID: 1886
		private const float CrushAccel = 500f;

		// Token: 0x0400075F RID: 1887
		private const float ReturnSpeed = 60f;

		// Token: 0x04000760 RID: 1888
		private const float ReturnAccel = 160f;

		// Token: 0x04000761 RID: 1889
		private Color fill = Calc.HexToColor("62222b");

		// Token: 0x04000762 RID: 1890
		private Level level;

		// Token: 0x04000763 RID: 1891
		private bool canActivate;

		// Token: 0x04000764 RID: 1892
		private Vector2 crushDir;

		// Token: 0x04000765 RID: 1893
		private List<CrushBlock.MoveState> returnStack;

		// Token: 0x04000766 RID: 1894
		private Coroutine attackCoroutine;

		// Token: 0x04000767 RID: 1895
		private bool canMoveVertically;

		// Token: 0x04000768 RID: 1896
		private bool canMoveHorizontally;

		// Token: 0x04000769 RID: 1897
		private bool chillOut;

		// Token: 0x0400076A RID: 1898
		private bool giant;

		// Token: 0x0400076B RID: 1899
		private Sprite face;

		// Token: 0x0400076C RID: 1900
		private string nextFaceDirection;

		// Token: 0x0400076D RID: 1901
		private List<Image> idleImages = new List<Image>();

		// Token: 0x0400076E RID: 1902
		private List<Image> activeTopImages = new List<Image>();

		// Token: 0x0400076F RID: 1903
		private List<Image> activeRightImages = new List<Image>();

		// Token: 0x04000770 RID: 1904
		private List<Image> activeLeftImages = new List<Image>();

		// Token: 0x04000771 RID: 1905
		private List<Image> activeBottomImages = new List<Image>();

		// Token: 0x04000772 RID: 1906
		private SoundSource currentMoveLoopSfx;

		// Token: 0x04000773 RID: 1907
		private SoundSource returnLoopSfx;

		// Token: 0x020003D7 RID: 983
		public enum Axes
		{
			// Token: 0x04001FC2 RID: 8130
			Both,
			// Token: 0x04001FC3 RID: 8131
			Horizontal,
			// Token: 0x04001FC4 RID: 8132
			Vertical
		}

		// Token: 0x020003D8 RID: 984
		private struct MoveState
		{
			// Token: 0x06001F37 RID: 7991 RVA: 0x000D7ABC File Offset: 0x000D5CBC
			public MoveState(Vector2 from, Vector2 direction)
			{
				this.From = from;
				this.Direction = direction;
			}

			// Token: 0x04001FC5 RID: 8133
			public Vector2 From;

			// Token: 0x04001FC6 RID: 8134
			public Vector2 Direction;
		}
	}
}
