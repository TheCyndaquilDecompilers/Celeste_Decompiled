using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002D7 RID: 727
	[Tracked(false)]
	public class Lookout : Entity
	{
		// Token: 0x06001672 RID: 5746 RVA: 0x000847D0 File Offset: 0x000829D0
		public Lookout(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Depth = -8500;
			base.Add(this.talk = new TalkComponent(new Rectangle(-24, -8, 48, 8), new Vector2(-0.5f, -20f), new Action<Player>(this.Interact), null));
			this.talk.PlayerMustBeFacing = false;
			this.summit = data.Bool("summit", false);
			this.onlyY = data.Bool("onlyY", false);
			base.Collider = new Hitbox(4f, 4f, -2f, -4f);
			VertexLight vertexLight = new VertexLight(new Vector2(-1f, -11f), Color.White, 0.8f, 16, 24);
			base.Add(vertexLight);
			this.lightTween = vertexLight.CreatePulseTween();
			base.Add(this.lightTween);
			base.Add(this.sprite = GFX.SpriteBank.Create("lookout"));
			this.sprite.OnFrameChange = delegate(string s)
			{
				if ((s == "idle" || s == "badeline_idle" || s == "nobackpack_idle") && this.sprite.CurrentAnimationFrame == this.sprite.CurrentAnimationTotalFrames - 1)
				{
					this.lightTween.Start();
				}
			};
			Vector2[] array = data.NodesOffset(offset);
			if (array != null && array.Length != 0)
			{
				this.nodes = new List<Vector2>(array);
			}
		}

		// Token: 0x06001673 RID: 5747 RVA: 0x00084924 File Offset: 0x00082B24
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			if (this.interacting)
			{
				Player entity = scene.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					entity.StateMachine.State = 0;
				}
			}
		}

		// Token: 0x06001674 RID: 5748 RVA: 0x0008495C File Offset: 0x00082B5C
		private void Interact(Player player)
		{
			if (player.DefaultSpriteMode == PlayerSpriteMode.MadelineAsBadeline || SaveData.Instance.Assists.PlayAsBadeline)
			{
				this.animPrefix = "badeline_";
			}
			else if (player.DefaultSpriteMode == PlayerSpriteMode.MadelineNoBackpack)
			{
				this.animPrefix = "nobackpack_";
			}
			else
			{
				this.animPrefix = "";
			}
			base.Add(new Coroutine(this.LookRoutine(player), true)
			{
				RemoveOnComplete = true
			});
			this.interacting = true;
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x000849D4 File Offset: 0x00082BD4
		public void StopInteracting()
		{
			this.interacting = false;
			this.sprite.Play(this.animPrefix + "idle", false, false);
		}

		// Token: 0x06001676 RID: 5750 RVA: 0x000849FC File Offset: 0x00082BFC
		public override void Update()
		{
			base.Update();
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				this.sprite.Active = (this.interacting || entity.StateMachine.State != 11);
				if (!this.sprite.Active)
				{
					this.sprite.SetAnimationFrame(0);
				}
			}
			if (this.talk != null && base.CollideCheck<Solid>())
			{
				base.Remove(this.talk);
				this.talk = null;
			}
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x00084A87 File Offset: 0x00082C87
		private IEnumerator LookRoutine(Player player)
		{
			Level level = base.SceneAs<Level>();
			SandwichLava sandwichLava = base.Scene.Entities.FindFirst<SandwichLava>();
			if (sandwichLava != null)
			{
				sandwichLava.Waiting = true;
			}
			if (player.Holding != null)
			{
				player.Drop();
			}
			player.StateMachine.State = 11;
			yield return player.DummyWalkToExact((int)base.X, false, 1f, true);
			if (Math.Abs(base.X - player.X) > 4f || player.Dead || !player.OnGround(1))
			{
				if (!player.Dead)
				{
					player.StateMachine.State = 0;
				}
				yield break;
			}
			Audio.Play("event:/game/general/lookout_use", this.Position);
			if (player.Facing == Facings.Right)
			{
				this.sprite.Play(this.animPrefix + "lookRight", false, false);
			}
			else
			{
				this.sprite.Play(this.animPrefix + "lookLeft", false, false);
			}
			player.Sprite.Visible = (player.Hair.Visible = false);
			yield return 0.2f;
			base.Scene.Add(this.hud = new Lookout.Hud());
			this.hud.TrackMode = (this.nodes != null);
			this.hud.OnlyY = this.onlyY;
			this.nodePercent = 0f;
			this.node = 0;
			Audio.Play("event:/ui/game/lookout_on");
			while ((this.hud.Easer = Calc.Approach(this.hud.Easer, 1f, Engine.DeltaTime * 3f)) < 1f)
			{
				level.ScreenPadding = (float)((int)(Ease.CubeInOut(this.hud.Easer) * 16f));
				yield return null;
			}
			float accel = 800f;
			float maxspd = 240f;
			Vector2 cam = level.Camera.Position;
			Vector2 speed = Vector2.Zero;
			Vector2 lastDir = Vector2.Zero;
			Vector2 camStart = level.Camera.Position;
			Vector2 camStartCenter = camStart + new Vector2(160f, 90f);
			while (!Input.MenuCancel.Pressed && !Input.MenuConfirm.Pressed && !Input.Dash.Pressed && !Input.Jump.Pressed && this.interacting)
			{
				Vector2 value = Input.Aim.Value;
				if (this.onlyY)
				{
					value.X = 0f;
				}
				if (Math.Sign(value.X) != Math.Sign(lastDir.X) || Math.Sign(value.Y) != Math.Sign(lastDir.Y))
				{
					Audio.Play("event:/game/general/lookout_move", this.Position);
				}
				lastDir = value;
				if (this.sprite.CurrentAnimationID != "lookLeft" && this.sprite.CurrentAnimationID != "lookRight")
				{
					if (value.X == 0f)
					{
						if (value.Y == 0f)
						{
							this.sprite.Play(this.animPrefix + "looking", false, false);
						}
						else if (value.Y > 0f)
						{
							this.sprite.Play(this.animPrefix + "lookingDown", false, false);
						}
						else
						{
							this.sprite.Play(this.animPrefix + "lookingUp", false, false);
						}
					}
					else if (value.X > 0f)
					{
						if (value.Y == 0f)
						{
							this.sprite.Play(this.animPrefix + "lookingRight", false, false);
						}
						else if (value.Y > 0f)
						{
							this.sprite.Play(this.animPrefix + "lookingDownRight", false, false);
						}
						else
						{
							this.sprite.Play(this.animPrefix + "lookingUpRight", false, false);
						}
					}
					else if (value.X < 0f)
					{
						if (value.Y == 0f)
						{
							this.sprite.Play(this.animPrefix + "lookingLeft", false, false);
						}
						else if (value.Y > 0f)
						{
							this.sprite.Play(this.animPrefix + "lookingDownLeft", false, false);
						}
						else
						{
							this.sprite.Play(this.animPrefix + "lookingUpLeft", false, false);
						}
					}
				}
				if (this.nodes == null)
				{
					speed += accel * value * Engine.DeltaTime;
					if (value.X == 0f)
					{
						speed.X = Calc.Approach(speed.X, 0f, accel * 2f * Engine.DeltaTime);
					}
					if (value.Y == 0f)
					{
						speed.Y = Calc.Approach(speed.Y, 0f, accel * 2f * Engine.DeltaTime);
					}
					if (speed.Length() > maxspd)
					{
						speed = speed.SafeNormalize(maxspd);
					}
					Vector2 vector = cam;
					List<Entity> entities = base.Scene.Tracker.GetEntities<LookoutBlocker>();
					cam.X += speed.X * Engine.DeltaTime;
					if (cam.X < (float)level.Bounds.Left || cam.X + 320f > (float)level.Bounds.Right)
					{
						speed.X = 0f;
					}
					cam.X = Calc.Clamp(cam.X, (float)level.Bounds.Left, (float)(level.Bounds.Right - 320));
					foreach (Entity entity in entities)
					{
						if (cam.X + 320f > entity.Left && cam.Y + 180f > entity.Top && cam.X < entity.Right && cam.Y < entity.Bottom)
						{
							cam.X = vector.X;
							speed.X = 0f;
						}
					}
					cam.Y += speed.Y * Engine.DeltaTime;
					if (cam.Y < (float)level.Bounds.Top || cam.Y + 180f > (float)level.Bounds.Bottom)
					{
						speed.Y = 0f;
					}
					cam.Y = Calc.Clamp(cam.Y, (float)level.Bounds.Top, (float)(level.Bounds.Bottom - 180));
					foreach (Entity entity2 in entities)
					{
						if (cam.X + 320f > entity2.Left && cam.Y + 180f > entity2.Top && cam.X < entity2.Right && cam.Y < entity2.Bottom)
						{
							cam.Y = vector.Y;
							speed.Y = 0f;
						}
					}
					level.Camera.Position = cam;
				}
				else
				{
					Vector2 vector2 = (this.node <= 0) ? camStartCenter : this.nodes[this.node - 1];
					Vector2 vector3 = this.nodes[this.node];
					float num = (vector2 - vector3).Length();
					(vector3 - vector2).SafeNormalize();
					if (this.nodePercent < 0.25f && this.node > 0)
					{
						Vector2 begin = Vector2.Lerp((this.node <= 1) ? camStartCenter : this.nodes[this.node - 2], vector2, 0.75f);
						Vector2 end = Vector2.Lerp(vector2, vector3, 0.25f);
						SimpleCurve simpleCurve = new SimpleCurve(begin, end, vector2);
						level.Camera.Position = simpleCurve.GetPoint(0.5f + this.nodePercent / 0.25f * 0.5f);
					}
					else if (this.nodePercent > 0.75f && this.node < this.nodes.Count - 1)
					{
						Vector2 value2 = this.nodes[this.node + 1];
						Vector2 begin2 = Vector2.Lerp(vector2, vector3, 0.75f);
						Vector2 end2 = Vector2.Lerp(vector3, value2, 0.25f);
						SimpleCurve simpleCurve2 = new SimpleCurve(begin2, end2, vector3);
						level.Camera.Position = simpleCurve2.GetPoint((this.nodePercent - 0.75f) / 0.25f * 0.5f);
					}
					else
					{
						level.Camera.Position = Vector2.Lerp(vector2, vector3, this.nodePercent);
					}
					level.Camera.Position += new Vector2(-160f, -90f);
					this.nodePercent -= value.Y * (maxspd / num) * Engine.DeltaTime;
					if (this.nodePercent < 0f)
					{
						if (this.node > 0)
						{
							this.node--;
							this.nodePercent = 1f;
						}
						else
						{
							this.nodePercent = 0f;
						}
					}
					else if (this.nodePercent > 1f)
					{
						if (this.node < this.nodes.Count - 1)
						{
							this.node++;
							this.nodePercent = 0f;
						}
						else
						{
							this.nodePercent = 1f;
							if (this.summit)
							{
								break;
							}
						}
					}
					float num2 = 0f;
					float num3 = 0f;
					for (int i = 0; i < this.nodes.Count; i++)
					{
						float num4 = (((i == 0) ? camStartCenter : this.nodes[i - 1]) - this.nodes[i]).Length();
						num3 += num4;
						if (i < this.node)
						{
							num2 += num4;
						}
						else if (i == this.node)
						{
							num2 += num4 * this.nodePercent;
						}
					}
					this.hud.TrackPercent = num2 / num3;
				}
				yield return null;
			}
			player.Sprite.Visible = (player.Hair.Visible = true);
			this.sprite.Play(this.animPrefix + "idle", false, false);
			Audio.Play("event:/ui/game/lookout_off");
			while ((this.hud.Easer = Calc.Approach(this.hud.Easer, 0f, Engine.DeltaTime * 3f)) > 0f)
			{
				level.ScreenPadding = (float)((int)(Ease.CubeInOut(this.hud.Easer) * 16f));
				yield return null;
			}
			bool atSummitTop = this.summit && this.node >= this.nodes.Count - 1 && this.nodePercent >= 0.95f;
			if (atSummitTop)
			{
				yield return 0.5f;
				float duration = 3f;
				float approach = 0f;
				Coroutine component = new Coroutine(level.ZoomTo(new Vector2(160f, 90f), 2f, duration), true);
				base.Add(component);
				while (!Input.MenuCancel.Pressed && !Input.MenuConfirm.Pressed && !Input.Dash.Pressed && !Input.Jump.Pressed && this.interacting)
				{
					approach = Calc.Approach(approach, 1f, Engine.DeltaTime / duration);
					Audio.SetMusicParam("escape", approach);
					yield return null;
				}
			}
			if ((camStart - level.Camera.Position).Length() > 600f)
			{
				Vector2 was = level.Camera.Position;
				Vector2 direction = (was - camStart).SafeNormalize();
				float approach = atSummitTop ? 1f : 0.5f;
				new FadeWipe(base.Scene, false, null).Duration = approach;
				for (float duration = 0f; duration < 1f; duration += Engine.DeltaTime / approach)
				{
					level.Camera.Position = was - direction * MathHelper.Lerp(0f, 64f, Ease.CubeIn(duration));
					yield return null;
				}
				level.Camera.Position = camStart + direction * 32f;
				new FadeWipe(base.Scene, true, null);
				was = default(Vector2);
				direction = default(Vector2);
			}
			Audio.SetMusicParam("escape", 0f);
			level.ScreenPadding = 0f;
			level.ZoomSnap(Vector2.Zero, 1f);
			base.Scene.Remove(this.hud);
			this.interacting = false;
			player.StateMachine.State = 0;
			yield return null;
			yield break;
		}

		// Token: 0x040012DD RID: 4829
		private TalkComponent talk;

		// Token: 0x040012DE RID: 4830
		private Lookout.Hud hud;

		// Token: 0x040012DF RID: 4831
		private Sprite sprite;

		// Token: 0x040012E0 RID: 4832
		private Tween lightTween;

		// Token: 0x040012E1 RID: 4833
		private bool interacting;

		// Token: 0x040012E2 RID: 4834
		private bool onlyY;

		// Token: 0x040012E3 RID: 4835
		private List<Vector2> nodes;

		// Token: 0x040012E4 RID: 4836
		private int node;

		// Token: 0x040012E5 RID: 4837
		private float nodePercent;

		// Token: 0x040012E6 RID: 4838
		private bool summit;

		// Token: 0x040012E7 RID: 4839
		private string animPrefix = "";

		// Token: 0x0200066F RID: 1647
		private class Hud : Entity
		{
			// Token: 0x06002B89 RID: 11145 RVA: 0x00115B03 File Offset: 0x00113D03
			public Hud()
			{
				base.AddTag(Tags.HUD);
			}

			// Token: 0x06002B8A RID: 11146 RVA: 0x00115B3C File Offset: 0x00113D3C
			public override void Update()
			{
				Level level = base.SceneAs<Level>();
				Vector2 position = level.Camera.Position;
				Rectangle bounds = level.Bounds;
				int num = 320;
				int num2 = 180;
				bool flag = base.Scene.CollideCheck<LookoutBlocker>(new Rectangle((int)(position.X - 8f), (int)position.Y, num, num2));
				bool flag2 = base.Scene.CollideCheck<LookoutBlocker>(new Rectangle((int)(position.X + 8f), (int)position.Y, num, num2));
				bool flag3 = (this.TrackMode && this.TrackPercent >= 1f) || base.Scene.CollideCheck<LookoutBlocker>(new Rectangle((int)position.X, (int)(position.Y - 8f), num, num2));
				bool flag4 = (this.TrackMode && this.TrackPercent <= 0f) || base.Scene.CollideCheck<LookoutBlocker>(new Rectangle((int)position.X, (int)(position.Y + 8f), num, num2));
				this.left = Calc.Approach(this.left, (float)((!flag && position.X > (float)(bounds.Left + 2)) ? 1 : 0), Engine.DeltaTime * 8f);
				this.right = Calc.Approach(this.right, (float)((!flag2 && position.X + (float)num < (float)(bounds.Right - 2)) ? 1 : 0), Engine.DeltaTime * 8f);
				this.up = Calc.Approach(this.up, (float)((!flag3 && position.Y > (float)(bounds.Top + 2)) ? 1 : 0), Engine.DeltaTime * 8f);
				this.down = Calc.Approach(this.down, (float)((!flag4 && position.Y + (float)num2 < (float)(bounds.Bottom - 2)) ? 1 : 0), Engine.DeltaTime * 8f);
				this.aim = Input.Aim.Value;
				if (this.aim.X < 0f)
				{
					this.multLeft = Calc.Approach(this.multLeft, 0f, Engine.DeltaTime * 2f);
					this.timerLeft += Engine.DeltaTime * 12f;
				}
				else
				{
					this.multLeft = Calc.Approach(this.multLeft, 1f, Engine.DeltaTime * 2f);
					this.timerLeft += Engine.DeltaTime * 6f;
				}
				if (this.aim.X > 0f)
				{
					this.multRight = Calc.Approach(this.multRight, 0f, Engine.DeltaTime * 2f);
					this.timerRight += Engine.DeltaTime * 12f;
				}
				else
				{
					this.multRight = Calc.Approach(this.multRight, 1f, Engine.DeltaTime * 2f);
					this.timerRight += Engine.DeltaTime * 6f;
				}
				if (this.aim.Y < 0f)
				{
					this.multUp = Calc.Approach(this.multUp, 0f, Engine.DeltaTime * 2f);
					this.timerUp += Engine.DeltaTime * 12f;
				}
				else
				{
					this.multUp = Calc.Approach(this.multUp, 1f, Engine.DeltaTime * 2f);
					this.timerUp += Engine.DeltaTime * 6f;
				}
				if (this.aim.Y > 0f)
				{
					this.multDown = Calc.Approach(this.multDown, 0f, Engine.DeltaTime * 2f);
					this.timerDown += Engine.DeltaTime * 12f;
				}
				else
				{
					this.multDown = Calc.Approach(this.multDown, 1f, Engine.DeltaTime * 2f);
					this.timerDown += Engine.DeltaTime * 6f;
				}
				base.Update();
			}

			// Token: 0x06002B8B RID: 11147 RVA: 0x00115F50 File Offset: 0x00114150
			public override void Render()
			{
				Level level = base.Scene as Level;
				float num = Ease.CubeInOut(this.Easer);
				Color color = Color.White * num;
				int num2 = (int)(80f * num);
				int num3 = (int)(80f * num * 0.5625f);
				int num4 = 8;
				if (level.FrozenOrPaused || level.RetryPlayerCorpse != null)
				{
					color *= 0.25f;
				}
				Draw.Rect((float)num2, (float)num3, (float)(1920 - num2 * 2 - num4), (float)num4, color);
				Draw.Rect((float)num2, (float)(num3 + num4), (float)(num4 + 2), (float)(1080 - num3 * 2 - num4), color);
				Draw.Rect((float)(1920 - num2 - num4 - 2), (float)num3, (float)(num4 + 2), (float)(1080 - num3 * 2 - num4), color);
				Draw.Rect((float)(num2 + num4), (float)(1080 - num3 - num4), (float)(1920 - num2 * 2 - num4), (float)num4, color);
				if (level.FrozenOrPaused || level.RetryPlayerCorpse != null)
				{
					return;
				}
				MTexture mtexture = GFX.Gui["towerarrow"];
				float y = (float)num3 * this.up - (float)(Math.Sin((double)this.timerUp) * 18.0 * (double)MathHelper.Lerp(0.5f, 1f, this.multUp)) - (1f - this.multUp) * 12f;
				mtexture.DrawCentered(new Vector2(960f, y), color * this.up, 1f, 1.5707964f);
				float y2 = 1080f - (float)num3 * this.down + (float)(Math.Sin((double)this.timerDown) * 18.0 * (double)MathHelper.Lerp(0.5f, 1f, this.multDown)) + (1f - this.multDown) * 12f;
				mtexture.DrawCentered(new Vector2(960f, y2), color * this.down, 1f, 4.712389f);
				if (!this.TrackMode && !this.OnlyY)
				{
					float num5 = this.left;
					float num6 = this.multLeft;
					float num7 = this.timerLeft;
					float num8 = this.right;
					float num9 = this.multRight;
					float num10 = this.timerRight;
					if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
					{
						num5 = this.right;
						num6 = this.multRight;
						num7 = this.timerRight;
						num8 = this.left;
						num9 = this.multLeft;
						num10 = this.timerLeft;
					}
					float x = (float)num2 * num5 - (float)(Math.Sin((double)num7) * 18.0 * (double)MathHelper.Lerp(0.5f, 1f, num6)) - (1f - num6) * 12f;
					mtexture.DrawCentered(new Vector2(x, 540f), color * num5);
					float x2 = 1920f - (float)num2 * num8 + (float)(Math.Sin((double)num10) * 18.0 * (double)MathHelper.Lerp(0.5f, 1f, num9)) + (1f - num9) * 12f;
					mtexture.DrawCentered(new Vector2(x2, 540f), color * num8, 1f, 3.1415927f);
					return;
				}
				if (this.TrackMode)
				{
					int num11 = 1080 - num3 * 2 - 128 - 64;
					int num12 = 1920 - num2 - 64;
					float num13 = (float)(1080 - num11) / 2f + 32f;
					Draw.Rect((float)(num12 - 7), num13 + 7f, 14f, (float)(num11 - 14), Color.Black * num);
					this.halfDot.DrawJustified(new Vector2((float)num12, num13 + 7f), new Vector2(0.5f, 1f), Color.Black * num);
					this.halfDot.DrawJustified(new Vector2((float)num12, num13 + (float)num11 - 7f), new Vector2(0.5f, 1f), Color.Black * num, new Vector2(1f, -1f));
					GFX.Gui["lookout/cursor"].DrawCentered(new Vector2((float)num12, num13 + (1f - this.TrackPercent) * (float)num11), Color.White * num, 1f);
					GFX.Gui["lookout/summit"].DrawCentered(new Vector2((float)num12, num13 - 64f), Color.White * num, 0.65f);
				}
			}

			// Token: 0x04002A98 RID: 10904
			public bool TrackMode;

			// Token: 0x04002A99 RID: 10905
			public float TrackPercent;

			// Token: 0x04002A9A RID: 10906
			public bool OnlyY;

			// Token: 0x04002A9B RID: 10907
			public float Easer;

			// Token: 0x04002A9C RID: 10908
			private float timerUp;

			// Token: 0x04002A9D RID: 10909
			private float timerDown;

			// Token: 0x04002A9E RID: 10910
			private float timerLeft;

			// Token: 0x04002A9F RID: 10911
			private float timerRight;

			// Token: 0x04002AA0 RID: 10912
			private float multUp;

			// Token: 0x04002AA1 RID: 10913
			private float multDown;

			// Token: 0x04002AA2 RID: 10914
			private float multLeft;

			// Token: 0x04002AA3 RID: 10915
			private float multRight;

			// Token: 0x04002AA4 RID: 10916
			private float left;

			// Token: 0x04002AA5 RID: 10917
			private float right;

			// Token: 0x04002AA6 RID: 10918
			private float up;

			// Token: 0x04002AA7 RID: 10919
			private float down;

			// Token: 0x04002AA8 RID: 10920
			private Vector2 aim;

			// Token: 0x04002AA9 RID: 10921
			private MTexture halfDot = GFX.Gui["dot"].GetSubtexture(0, 0, 64, 32, null);
		}
	}
}
