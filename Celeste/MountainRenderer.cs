using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste
{
	// Token: 0x020002F8 RID: 760
	public class MountainRenderer : Renderer
	{
		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06001772 RID: 6002 RVA: 0x0008F8C7 File Offset: 0x0008DAC7
		public MountainCamera Camera
		{
			get
			{
				return this.Model.Camera;
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06001773 RID: 6003 RVA: 0x0008F8D4 File Offset: 0x0008DAD4
		// (set) Token: 0x06001774 RID: 6004 RVA: 0x0008F8DC File Offset: 0x0008DADC
		public bool Animating { get; private set; }

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06001775 RID: 6005 RVA: 0x0008F8E5 File Offset: 0x0008DAE5
		// (set) Token: 0x06001776 RID: 6006 RVA: 0x0008F8ED File Offset: 0x0008DAED
		public int Area { get; private set; }

		// Token: 0x06001777 RID: 6007 RVA: 0x0008F8F8 File Offset: 0x0008DAF8
		public MountainRenderer()
		{
			this.Model = new MountainModel();
			this.GotoRotationMode();
		}

		// Token: 0x06001778 RID: 6008 RVA: 0x0008F944 File Offset: 0x0008DB44
		public void Dispose()
		{
			this.Model.Dispose();
		}

		// Token: 0x06001779 RID: 6009 RVA: 0x0008F954 File Offset: 0x0008DB54
		public override void Update(Scene scene)
		{
			this.timer += Engine.DeltaTime;
			this.Model.Update();
			Vector2 value = this.AllowUserRotation ? (-Input.MountainAim.Value * 0.8f) : Vector2.Zero;
			this.userOffset += (value - this.userOffset) * (1f - (float)Math.Pow(0.009999999776482582, (double)Engine.DeltaTime));
			if (!this.rotateAroundCenter)
			{
				if (this.Area == 8)
				{
					this.userOffset.Y = Math.Max(0f, this.userOffset.Y);
				}
				if (this.Area == 7)
				{
					this.userOffset.X = Calc.Clamp(this.userOffset.X, -0.4f, 0.4f);
				}
			}
			if (!this.inFreeCameraDebugMode)
			{
				if (this.rotateAroundCenter)
				{
					this.rotateTimer -= Engine.DeltaTime * 0.1f;
					Vector3 value2 = new Vector3((float)Math.Cos((double)this.rotateTimer) * 15f, 3f, (float)Math.Sin((double)this.rotateTimer) * 15f);
					MountainModel model = this.Model;
					model.Camera.Position = model.Camera.Position + (value2 - this.Model.Camera.Position) * (1f - (float)Math.Pow(0.10000000149011612, (double)Engine.DeltaTime));
					this.Model.Camera.Target = MountainRenderer.RotateLookAt + Vector3.Up * this.userOffset.Y;
					Quaternion quaternion = default(Quaternion).LookAt(this.Model.Camera.Position, this.Model.Camera.Target, Vector3.Up);
					this.Model.Camera.Rotation = Quaternion.Slerp(this.Model.Camera.Rotation, quaternion, Engine.DeltaTime * 4f);
					this.UntiltedCamera = this.Camera;
				}
				else
				{
					if (this.Animating)
					{
						this.percent = Calc.Approach(this.percent, 1f, Engine.DeltaTime / this.duration);
						float num = Ease.SineOut(this.percent);
						this.Model.Camera.Position = this.GetBetween(this.easeCameraFrom.Position, this.easeCameraTo.Position, num);
						this.Model.Camera.Target = this.GetBetween(this.easeCameraFrom.Target, this.easeCameraTo.Target, num);
						Vector3 vector = this.easeCameraFrom.Rotation.Forward();
						Vector3 vector2 = this.easeCameraTo.Rotation.Forward();
						float length = Calc.LerpClamp(vector.XZ().Length(), vector2.XZ().Length(), num);
						Vector2 vector3 = Calc.AngleToVector(MathHelper.Lerp(vector.XZ().Angle(), this.easeCameraRotationAngleTo, num), length);
						float y = Calc.LerpClamp(vector.Y, vector2.Y, num);
						this.Model.Camera.Rotation = default(Quaternion).LookAt(new Vector3(vector3.X, y, vector3.Y), Vector3.Up);
						if (this.percent >= 1f)
						{
							this.rotateTimer = new Vector2(this.Model.Camera.Position.X, this.Model.Camera.Position.Z).Angle();
							this.Animating = false;
							if (this.OnEaseEnd != null)
							{
								this.OnEaseEnd();
							}
						}
					}
					else if (this.rotateAroundTarget)
					{
						this.rotateTimer -= Engine.DeltaTime * 0.1f;
						float num2 = (new Vector2(this.easeCameraTo.Target.X, this.easeCameraTo.Target.Z) - new Vector2(this.easeCameraTo.Position.X, this.easeCameraTo.Position.Z)).Length();
						Vector3 value3 = new Vector3(this.easeCameraTo.Target.X + (float)Math.Cos((double)this.rotateTimer) * num2, this.easeCameraTo.Position.Y, this.easeCameraTo.Target.Z + (float)Math.Sin((double)this.rotateTimer) * num2);
						MountainModel model2 = this.Model;
						model2.Camera.Position = model2.Camera.Position + (value3 - this.Model.Camera.Position) * (1f - (float)Math.Pow(0.10000000149011612, (double)Engine.DeltaTime));
						this.Model.Camera.Target = this.easeCameraTo.Target + Vector3.Up * this.userOffset.Y * 2f + Vector3.Left * this.userOffset.X * 2f;
						Quaternion quaternion2 = default(Quaternion).LookAt(this.Model.Camera.Position, this.Model.Camera.Target, Vector3.Up);
						this.Model.Camera.Rotation = Quaternion.Slerp(this.Model.Camera.Rotation, quaternion2, Engine.DeltaTime * 4f);
						this.UntiltedCamera = this.Camera;
					}
					else
					{
						this.Model.Camera.Rotation = this.easeCameraTo.Rotation;
						this.Model.Camera.Target = this.easeCameraTo.Target;
					}
					this.UntiltedCamera = this.Camera;
					if (this.userOffset != Vector2.Zero && !this.rotateAroundTarget)
					{
						Vector3 value4 = this.Model.Camera.Rotation.Left() * this.userOffset.X * 0.25f;
						Vector3 value5 = this.Model.Camera.Rotation.Up() * this.userOffset.Y * 0.25f;
						Vector3 pos = this.Model.Camera.Position + this.Model.Camera.Rotation.Forward() + value4 + value5;
						this.Model.Camera.LookAt(pos);
					}
				}
			}
			else
			{
				Vector3 vector4 = Vector3.Transform(Vector3.Forward, this.Model.Camera.Rotation.Conjugated());
				this.Model.Camera.Rotation = this.Model.Camera.Rotation.LookAt(Vector3.Zero, vector4, Vector3.Up);
				Vector3 vector5 = Vector3.Transform(Vector3.Left, this.Model.Camera.Rotation.Conjugated());
				Vector3 vector6 = new Vector3(0f, 0f, 0f);
				if (MInput.Keyboard.Check(Keys.W))
				{
					vector6 += vector4;
				}
				if (MInput.Keyboard.Check(Keys.S))
				{
					vector6 -= vector4;
				}
				if (MInput.Keyboard.Check(Keys.D))
				{
					vector6 -= vector5;
				}
				if (MInput.Keyboard.Check(Keys.A))
				{
					vector6 += vector5;
				}
				if (MInput.Keyboard.Check(Keys.Q))
				{
					vector6 += Vector3.Up;
				}
				if (MInput.Keyboard.Check(Keys.Z))
				{
					vector6 += Vector3.Down;
				}
				MountainModel model3 = this.Model;
				model3.Camera.Position = model3.Camera.Position + vector6 * (MInput.Keyboard.Check(Keys.LeftShift) ? 0.5f : 5f) * Engine.DeltaTime;
				if (MInput.Mouse.CheckLeftButton)
				{
					MouseState state = Mouse.GetState();
					int num3 = Engine.Graphics.GraphicsDevice.Viewport.Width / 2;
					int num4 = Engine.Graphics.GraphicsDevice.Viewport.Height / 2;
					int num5 = state.X - num3;
					int num6 = state.Y - num4;
					MountainModel model4 = this.Model;
					model4.Camera.Rotation = model4.Camera.Rotation * Quaternion.CreateFromAxisAngle(Vector3.Up, (float)num5 * 0.1f * Engine.DeltaTime);
					MountainModel model5 = this.Model;
					model5.Camera.Rotation = model5.Camera.Rotation * Quaternion.CreateFromAxisAngle(vector5, (float)(-(float)num6) * 0.1f * Engine.DeltaTime);
					Mouse.SetPosition(num3, num4);
				}
				if (this.Area >= 0)
				{
					Vector3 target = AreaData.Areas[this.Area].MountainIdle.Target;
					Vector3 vector7 = vector5 * 0.05f;
					Vector3 value6 = Vector3.Up * 0.05f;
					this.Model.DebugPoints.Clear();
					this.Model.DebugPoints.Add(new VertexPositionColor(target - vector7 + value6, Color.Red));
					this.Model.DebugPoints.Add(new VertexPositionColor(target + vector7 + value6, Color.Red));
					this.Model.DebugPoints.Add(new VertexPositionColor(target + vector7 - value6, Color.Red));
					this.Model.DebugPoints.Add(new VertexPositionColor(target - vector7 + value6, Color.Red));
					this.Model.DebugPoints.Add(new VertexPositionColor(target + vector7 - value6, Color.Red));
					this.Model.DebugPoints.Add(new VertexPositionColor(target - vector7 - value6, Color.Red));
					this.Model.DebugPoints.Add(new VertexPositionColor(target - vector7 * 0.25f - value6, Color.Red));
					this.Model.DebugPoints.Add(new VertexPositionColor(target + vector7 * 0.25f - value6, Color.Red));
					this.Model.DebugPoints.Add(new VertexPositionColor(target + vector7 * 0.25f + Vector3.Down * 100f, Color.Red));
					this.Model.DebugPoints.Add(new VertexPositionColor(target - vector7 * 0.25f - value6, Color.Red));
					this.Model.DebugPoints.Add(new VertexPositionColor(target + vector7 * 0.25f + Vector3.Down * 100f, Color.Red));
					this.Model.DebugPoints.Add(new VertexPositionColor(target - vector7 * 0.25f + Vector3.Down * 100f, Color.Red));
				}
			}
			this.door = Calc.Approach(this.door, (float)((this.Area == 9 && !this.rotateAroundCenter) ? 1 : 0), Engine.DeltaTime * 1f);
			this.Model.CoreWallPosition = Vector3.Lerp(Vector3.Zero, -new Vector3(-1.5f, 1.5f, 1f), Ease.CubeInOut(this.door));
			this.Model.NearFogAlpha = Calc.Approach(this.Model.NearFogAlpha, (float)((this.ForceNearFog || this.rotateAroundCenter) ? 1 : 0), (float)(this.rotateAroundCenter ? 1 : 4) * Engine.DeltaTime);
			if (Celeste.PlayMode == Celeste.PlayModes.Debug)
			{
				if (MInput.Keyboard.Pressed(Keys.P))
				{
					Console.WriteLine(this.GetCameraString());
				}
				if (MInput.Keyboard.Pressed(Keys.F2))
				{
					Engine.Scene = new OverworldLoader(Overworld.StartMode.ReturnFromOptions, null);
				}
				if (MInput.Keyboard.Pressed(Keys.Space))
				{
					this.inFreeCameraDebugMode = !this.inFreeCameraDebugMode;
				}
				this.Model.DrawDebugPoints = this.inFreeCameraDebugMode;
				if (MInput.Keyboard.Pressed(Keys.F1))
				{
					AreaData.ReloadMountainViews();
				}
			}
		}

		// Token: 0x0600177A RID: 6010 RVA: 0x000906AC File Offset: 0x0008E8AC
		private Vector3 GetBetween(Vector3 from, Vector3 to, float ease)
		{
			Vector2 from2 = new Vector2(from.X, from.Z);
			Vector2 from3 = new Vector2(to.X, to.Z);
			float startAngle = Calc.Angle(from2, Vector2.Zero);
			float endAngle = Calc.Angle(from3, Vector2.Zero);
			float angleRadians = Calc.AngleLerp(startAngle, endAngle, ease);
			float num = from2.Length();
			float num2 = from3.Length();
			float length = num + (num2 - num) * ease;
			float y = from.Y + (to.Y - from.Y) * ease;
			Vector2 vector = -Calc.AngleToVector(angleRadians, length);
			return new Vector3(vector.X, y, vector.Y);
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x00090753 File Offset: 0x0008E953
		public override void BeforeRender(Scene scene)
		{
			this.Model.BeforeRender(scene);
		}

		// Token: 0x0600177C RID: 6012 RVA: 0x00090764 File Offset: 0x0008E964
		public override void Render(Scene scene)
		{
			this.Model.Render();
			Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, null, null, null, Engine.ScreenMatrix);
			OVR.Atlas["vignette"].Draw(Vector2.Zero, Vector2.Zero, Color.White * 0.2f);
			Draw.SpriteBatch.End();
			if (this.inFreeCameraDebugMode)
			{
				Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, null, null, null, Engine.ScreenMatrix);
				ActiveFont.DrawOutline(this.GetCameraString(), new Vector2(8f, 8f), Vector2.Zero, Vector2.One * 0.75f, Color.White, 2f, Color.Black);
				Draw.SpriteBatch.End();
			}
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x0009083C File Offset: 0x0008EA3C
		public void SnapCamera(int area, MountainCamera transform, bool targetRotate = false)
		{
			this.Area = area;
			this.Animating = false;
			this.rotateAroundCenter = false;
			this.rotateAroundTarget = targetRotate;
			this.Model.Camera = transform;
			this.percent = 1f;
		}

		// Token: 0x0600177E RID: 6014 RVA: 0x00090871 File Offset: 0x0008EA71
		public void SnapState(int state)
		{
			this.Model.SnapState(state);
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x00090880 File Offset: 0x0008EA80
		public float EaseCamera(int area, MountainCamera transform, float? duration = null, bool nearTarget = true, bool targetRotate = false)
		{
			if (this.Area != area && area >= 0)
			{
				this.PlayWhoosh(this.Area, area);
			}
			this.Area = area;
			this.percent = 0f;
			this.Animating = true;
			this.rotateAroundCenter = false;
			this.rotateAroundTarget = targetRotate;
			this.userOffset = Vector2.Zero;
			this.easeCameraFrom = this.Model.Camera;
			if (nearTarget)
			{
				this.easeCameraFrom.Target = this.easeCameraFrom.Position + (this.easeCameraFrom.Target - this.easeCameraFrom.Position).SafeNormalize() * 0.5f;
			}
			this.easeCameraTo = transform;
			float num = this.easeCameraFrom.Rotation.Forward().XZ().Angle();
			float radiansB = this.easeCameraTo.Rotation.Forward().XZ().Angle();
			float num2 = Calc.AngleDiff(num, radiansB);
			float num3 = (float)(-(float)Math.Sign(num2)) * (6.2831855f - Math.Abs(num2));
			Vector3 between = this.GetBetween(this.easeCameraFrom.Position, this.easeCameraTo.Position, 0.5f);
			Vector2 vector = Calc.AngleToVector(MathHelper.Lerp(num, num + num2, 0.5f), 1f);
			Vector2 vector2 = Calc.AngleToVector(MathHelper.Lerp(num, num + num3, 0.5f), 1f);
			if ((between + new Vector3(vector.X, 0f, vector.Y)).Length() < (between + new Vector3(vector2.X, 0f, vector2.Y)).Length())
			{
				this.easeCameraRotationAngleTo = num + num2;
			}
			else
			{
				this.easeCameraRotationAngleTo = num + num3;
			}
			if (duration == null)
			{
				this.duration = this.GetDuration(this.easeCameraFrom, this.easeCameraTo);
			}
			else
			{
				this.duration = duration.Value;
			}
			return this.duration;
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x00090A82 File Offset: 0x0008EC82
		public void EaseState(int state)
		{
			this.Model.EaseState(state);
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x00090A90 File Offset: 0x0008EC90
		public void GotoRotationMode()
		{
			if (!this.rotateAroundCenter)
			{
				this.rotateAroundCenter = true;
				this.rotateTimer = new Vector2(this.Model.Camera.Position.X, this.Model.Camera.Position.Z).Angle();
				this.Model.EaseState(0);
			}
		}

		// Token: 0x06001782 RID: 6018 RVA: 0x00090AF4 File Offset: 0x0008ECF4
		private float GetDuration(MountainCamera from, MountainCamera to)
		{
			float value = Calc.AngleDiff(Calc.Angle(new Vector2(from.Position.X, from.Position.Z), new Vector2(from.Target.X, from.Target.Z)), Calc.Angle(new Vector2(to.Position.X, to.Position.Z), new Vector2(to.Target.X, to.Target.Z)));
			double val = Math.Sqrt((double)(from.Position - to.Position).Length()) / 3.0;
			return Calc.Clamp((float)(Math.Max((double)(Math.Abs(value) * 0.5f), val) * 0.699999988079071), 0.3f, 1.1f);
		}

		// Token: 0x06001783 RID: 6019 RVA: 0x00090BD4 File Offset: 0x0008EDD4
		private void PlayWhoosh(int from, int to)
		{
			string text = "";
			if (from == 0 && to == 1)
			{
				text = "event:/ui/world_map/whoosh/400ms_forward";
			}
			else if (from == 1 && to == 0)
			{
				text = "event:/ui/world_map/whoosh/400ms_back";
			}
			else if (from == 1 && to == 2)
			{
				text = "event:/ui/world_map/whoosh/600ms_forward";
			}
			else if (from == 2 && to == 1)
			{
				text = "event:/ui/world_map/whoosh/600ms_back";
			}
			else if (from < to && from > 1 && from < 7)
			{
				text = "event:/ui/world_map/whoosh/700ms_forward";
			}
			else if (from > to && from > 2 && from < 8)
			{
				text = "event:/ui/world_map/whoosh/700ms_back";
			}
			else if (from == 7 && to == 8)
			{
				text = "event:/ui/world_map/whoosh/1000ms_forward";
			}
			else if (from == 8 && to == 7)
			{
				text = "event:/ui/world_map/whoosh/1000ms_back";
			}
			else if (from == 8 && to == 9)
			{
				text = "event:/ui/world_map/whoosh/600ms_forward";
			}
			else if (from == 9 && to == 8)
			{
				text = "event:/ui/world_map/whoosh/600ms_back";
			}
			else if (from == 9 && to == 10)
			{
				text = "event:/ui/world_map/whoosh/1000ms_forward";
			}
			else if (from == 10 && to == 9)
			{
				text = "event:/ui/world_map/whoosh/1000ms_back";
			}
			if (!string.IsNullOrEmpty(text))
			{
				Audio.Play(text);
			}
		}

		// Token: 0x06001784 RID: 6020 RVA: 0x00090CCC File Offset: 0x0008EECC
		private string GetCameraString()
		{
			Vector3 position = this.Model.Camera.Position;
			Vector3 vector = position + Vector3.Transform(Vector3.Forward, this.Model.Camera.Rotation.Conjugated()) * 2f;
			return string.Concat(new string[]
			{
				"position=\"",
				position.X.ToString("0.000"),
				", ",
				position.Y.ToString("0.000"),
				", ",
				position.Z.ToString("0.000"),
				"\" \ntarget=\"",
				vector.X.ToString("0.000"),
				", ",
				vector.Y.ToString("0.000"),
				", ",
				vector.Z.ToString("0.000"),
				"\""
			});
		}

		// Token: 0x04001426 RID: 5158
		public bool ForceNearFog;

		// Token: 0x04001427 RID: 5159
		public Action OnEaseEnd;

		// Token: 0x04001428 RID: 5160
		public static readonly Vector3 RotateLookAt = new Vector3(0f, 7f, 0f);

		// Token: 0x04001429 RID: 5161
		private const float rotateDistance = 15f;

		// Token: 0x0400142A RID: 5162
		private const float rotateYPosition = 3f;

		// Token: 0x0400142B RID: 5163
		private bool rotateAroundCenter;

		// Token: 0x0400142C RID: 5164
		private bool rotateAroundTarget;

		// Token: 0x0400142D RID: 5165
		private float rotateAroundTargetDistance;

		// Token: 0x0400142E RID: 5166
		private float rotateTimer = 1.5707964f;

		// Token: 0x0400142F RID: 5167
		private const float DurationDivisor = 3f;

		// Token: 0x04001430 RID: 5168
		public MountainCamera UntiltedCamera;

		// Token: 0x04001431 RID: 5169
		public MountainModel Model;

		// Token: 0x04001432 RID: 5170
		public bool AllowUserRotation = true;

		// Token: 0x04001434 RID: 5172
		private Vector2 userOffset;

		// Token: 0x04001435 RID: 5173
		private bool inFreeCameraDebugMode;

		// Token: 0x04001436 RID: 5174
		private float percent = 1f;

		// Token: 0x04001437 RID: 5175
		private float duration = 1f;

		// Token: 0x04001438 RID: 5176
		private MountainCamera easeCameraFrom;

		// Token: 0x04001439 RID: 5177
		private MountainCamera easeCameraTo;

		// Token: 0x0400143A RID: 5178
		private float easeCameraRotationAngleTo;

		// Token: 0x0400143C RID: 5180
		private float timer;

		// Token: 0x0400143D RID: 5181
		private float door;
	}
}
