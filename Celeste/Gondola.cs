using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000275 RID: 629
	public class Gondola : Solid
	{
		// Token: 0x17000156 RID: 342
		// (get) Token: 0x06001383 RID: 4995 RVA: 0x0006A191 File Offset: 0x00068391
		// (set) Token: 0x06001384 RID: 4996 RVA: 0x0006A199 File Offset: 0x00068399
		public Vector2 Start { get; private set; }

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06001385 RID: 4997 RVA: 0x0006A1A2 File Offset: 0x000683A2
		// (set) Token: 0x06001386 RID: 4998 RVA: 0x0006A1AA File Offset: 0x000683AA
		public Vector2 Destination { get; private set; }

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06001387 RID: 4999 RVA: 0x0006A1B3 File Offset: 0x000683B3
		// (set) Token: 0x06001388 RID: 5000 RVA: 0x0006A1BB File Offset: 0x000683BB
		public Vector2 Halfway { get; private set; }

		// Token: 0x06001389 RID: 5001 RVA: 0x0006A1C4 File Offset: 0x000683C4
		public Gondola(EntityData data, Vector2 offset) : base(data.Position + offset, 64f, 8f, true)
		{
			this.EnableAssistModeChecks = false;
			base.Add(this.front = GFX.SpriteBank.Create("gondola"));
			this.front.Play("idle", false, false);
			this.front.Origin = new Vector2(this.front.Width / 2f, 12f);
			this.front.Y = -52f;
			base.Add(this.top = new Image(GFX.Game["objects/gondola/top"]));
			this.top.Origin = new Vector2(this.top.Width / 2f, 12f);
			this.top.Y = -52f;
			base.Add(this.Lever = new Sprite(GFX.Game, "objects/gondola/lever"));
			this.Lever.Add("idle", "", 0f, new int[1]);
			this.Lever.Add("pulled", "", 0.5f, "idle", new int[]
			{
				1,
				1
			});
			this.Lever.Origin = new Vector2(this.front.Width / 2f, 12f);
			this.Lever.Y = -52f;
			this.Lever.Play("idle", false, false);
			(base.Collider as Hitbox).Position.X = -base.Collider.Width / 2f;
			this.Start = this.Position;
			this.Destination = offset + data.Nodes[0];
			this.Halfway = (this.Position + this.Destination) / 2f;
			base.Depth = -10500;
			this.inCliffside = data.Bool("active", true);
			this.SurfaceSoundIndex = 28;
		}

		// Token: 0x0600138A RID: 5002 RVA: 0x0006A3F8 File Offset: 0x000685F8
		public override void Added(Scene scene)
		{
			base.Added(scene);
			scene.Add(this.back = new Entity(this.Position));
			this.back.Depth = 9000;
			this.backImg = new Image(GFX.Game["objects/gondola/back"]);
			this.backImg.Origin = new Vector2(this.backImg.Width / 2f, 12f);
			this.backImg.Y = -52f;
			this.back.Add(this.backImg);
			scene.Add(this.LeftCliffside = new Entity(this.Position + new Vector2(-124f, 0f)));
			Image image = new Image(GFX.Game["objects/gondola/cliffsideLeft"]);
			image.JustifyOrigin(0f, 1f);
			this.LeftCliffside.Add(image);
			this.LeftCliffside.Depth = 8998;
			scene.Add(this.RightCliffside = new Entity(this.Destination + new Vector2(144f, -104f)));
			Image image2 = new Image(GFX.Game["objects/gondola/cliffsideRight"]);
			image2.JustifyOrigin(0f, 0.5f);
			image2.Scale.X = -1f;
			this.RightCliffside.Add(image2);
			this.RightCliffside.Depth = 8998;
			scene.Add(new Gondola.Rope
			{
				Gondola = this
			});
			if (!this.inCliffside)
			{
				this.Position = this.Destination;
				this.Lever.Visible = false;
				this.UpdatePositions();
				JumpThru jumpThru = new JumpThru(this.Position + new Vector2(-base.Width / 2f, -36f), (int)base.Width, true);
				jumpThru.SurfaceSoundIndex = 28;
				base.Scene.Add(jumpThru);
			}
			this.top.Rotation = Calc.Angle(this.Start, this.Destination);
		}

		// Token: 0x0600138B RID: 5003 RVA: 0x0006A620 File Offset: 0x00068820
		public override void Update()
		{
			if (this.inCliffside)
			{
				float num = (Math.Sign(this.Rotation) == Math.Sign(this.RotationSpeed)) ? 8f : 6f;
				if (Math.Abs(this.Rotation) < 0.5f)
				{
					num *= 0.5f;
				}
				if (Math.Abs(this.Rotation) < 0.25f)
				{
					num *= 0.5f;
				}
				this.RotationSpeed += (float)(-(float)Math.Sign(this.Rotation)) * num * Engine.DeltaTime;
				this.Rotation += this.RotationSpeed * Engine.DeltaTime;
				this.Rotation = Calc.Clamp(this.Rotation, -0.4f, 0.4f);
				if (Math.Abs(this.Rotation) < 0.02f && Math.Abs(this.RotationSpeed) < 0.2f)
				{
					this.Rotation = (this.RotationSpeed = 0f);
				}
			}
			this.UpdatePositions();
			base.Update();
		}

		// Token: 0x0600138C RID: 5004 RVA: 0x0006A72C File Offset: 0x0006892C
		private void UpdatePositions()
		{
			this.back.Position = this.Position;
			this.backImg.Rotation = this.Rotation;
			this.front.Rotation = this.Rotation;
			if (!this.brokenLever)
			{
				this.Lever.Rotation = this.Rotation;
			}
			this.top.Rotation = Calc.Angle(this.Start, this.Destination);
		}

		// Token: 0x0600138D RID: 5005 RVA: 0x0006A7A4 File Offset: 0x000689A4
		public Vector2 GetRotatedFloorPositionAt(float x, float y = 52f)
		{
			Vector2 vector = Calc.AngleToVector(this.Rotation + 1.5707964f, 1f);
			Vector2 value = new Vector2(-vector.Y, vector.X);
			return this.Position + new Vector2(0f, -52f) + vector * y - value * x;
		}

		// Token: 0x0600138E RID: 5006 RVA: 0x0006A80E File Offset: 0x00068A0E
		public void BreakLever()
		{
			base.Add(new Coroutine(this.BreakLeverRoutine(), true));
		}

		// Token: 0x0600138F RID: 5007 RVA: 0x0006A822 File Offset: 0x00068A22
		private IEnumerator BreakLeverRoutine()
		{
			this.brokenLever = true;
			Vector2 speed = new Vector2(240f, -130f);
			for (;;)
			{
				this.Lever.Position += speed * Engine.DeltaTime;
				this.Lever.Rotation += 2f * Engine.DeltaTime;
				speed.Y += 400f * Engine.DeltaTime;
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001390 RID: 5008 RVA: 0x0006A831 File Offset: 0x00068A31
		public void PullSides()
		{
			this.front.Play("pull", false, false);
		}

		// Token: 0x06001391 RID: 5009 RVA: 0x0006A845 File Offset: 0x00068A45
		public void CancelPullSides()
		{
			this.front.Play("idle", false, false);
		}

		// Token: 0x04000F59 RID: 3929
		public float Rotation;

		// Token: 0x04000F5A RID: 3930
		public float RotationSpeed;

		// Token: 0x04000F5B RID: 3931
		public Entity LeftCliffside;

		// Token: 0x04000F5C RID: 3932
		public Entity RightCliffside;

		// Token: 0x04000F5D RID: 3933
		private Entity back;

		// Token: 0x04000F5E RID: 3934
		private Image backImg;

		// Token: 0x04000F5F RID: 3935
		private Sprite front;

		// Token: 0x04000F60 RID: 3936
		public Sprite Lever;

		// Token: 0x04000F61 RID: 3937
		private Image top;

		// Token: 0x04000F62 RID: 3938
		private bool brokenLever;

		// Token: 0x04000F63 RID: 3939
		private bool inCliffside;

		// Token: 0x020005B8 RID: 1464
		private class Rope : Entity
		{
			// Token: 0x06002831 RID: 10289 RVA: 0x00105D6F File Offset: 0x00103F6F
			public Rope()
			{
				base.Depth = 8999;
			}

			// Token: 0x06002832 RID: 10290 RVA: 0x00105D84 File Offset: 0x00103F84
			public override void Render()
			{
				Vector2 vector = (this.Gondola.LeftCliffside.Position + new Vector2(40f, -12f)).Floor();
				Vector2 value = (this.Gondola.RightCliffside.Position + new Vector2(-40f, -4f)).Floor();
				Vector2 value2 = (value - vector).SafeNormalize();
				Vector2 value3 = this.Gondola.Position + new Vector2(0f, -55f) - value2 * 6f;
				Vector2 value4 = this.Gondola.Position + new Vector2(0f, -55f) + value2 * 6f;
				for (int i = 0; i < 2; i++)
				{
					Vector2 value5 = Vector2.UnitY * (float)i;
					Draw.Line(vector + value5, value3 + value5, Color.Black);
					Draw.Line(value4 + value5, value + value5, Color.Black);
				}
			}

			// Token: 0x040027BB RID: 10171
			public Gondola Gondola;
		}
	}
}
