using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001B5 RID: 437
	public class MoonCreature : Entity
	{
		// Token: 0x06000F37 RID: 3895 RVA: 0x0003D5B0 File Offset: 0x0003B7B0
		public MoonCreature(Vector2 position)
		{
			base.Tag = Tags.TransitionUpdate;
			base.Depth = -13010;
			base.Collider = new Hitbox(20f, 20f, -10f, -10f);
			this.start = position;
			this.targetTimer = 0f;
			this.GetRandomTarget();
			this.Position = this.target;
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			this.OrbColor = Calc.HexToColor("b0e6ff");
			this.CenterColor = Calc.Random.Choose(Calc.HexToColor("c34fc7"), Calc.HexToColor("4f95c7"), Calc.HexToColor("53c74f"));
			Color value = Color.Lerp(this.CenterColor, Calc.HexToColor("bde4ee"), 0.5f);
			Color value2 = Color.Lerp(this.CenterColor, Calc.HexToColor("2f2941"), 0.5f);
			this.trail = new MoonCreature.TrailNode[10];
			for (int i = 0; i < 10; i++)
			{
				this.trail[i] = new MoonCreature.TrailNode
				{
					Position = this.Position,
					Color = Color.Lerp(value, value2, (float)i / 9f)
				};
			}
			base.Add(this.Sprite = GFX.SpriteBank.Create("moonCreatureTiny"));
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x0003D720 File Offset: 0x0003B920
		public MoonCreature(EntityData data, Vector2 offset) : this(data.Position + offset)
		{
			this.spawn = data.Int("number", 1) - 1;
		}

		// Token: 0x06000F39 RID: 3897 RVA: 0x0003D748 File Offset: 0x0003B948
		public override void Added(Scene scene)
		{
			base.Added(scene);
			for (int i = 0; i < this.spawn; i++)
			{
				scene.Add(new MoonCreature(this.Position + new Vector2((float)Calc.Random.Range(-4, 4), (float)Calc.Random.Range(-4, 4))));
			}
			this.originLevelBounds = (scene as Level).Bounds;
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x0003D7B8 File Offset: 0x0003B9B8
		private void OnPlayer(Player player)
		{
			Vector2 vector = (this.Position - player.Center).SafeNormalize(player.Speed.Length() * 0.3f);
			if (vector.LengthSquared() > this.bump.LengthSquared())
			{
				this.bump = vector;
				if ((player.Center - this.start).Length() < 200f)
				{
					this.following = player;
					this.followingTime = Calc.Random.Range(6f, 12f);
					this.GetFollowOffset();
				}
			}
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x0003D84F File Offset: 0x0003BA4F
		private void GetFollowOffset()
		{
			this.followingOffset = new Vector2((float)(Calc.Random.Choose(-1, 1) * Calc.Random.Range(8, 16)), Calc.Random.Range(-20f, 0f));
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x0003D88C File Offset: 0x0003BA8C
		private void GetRandomTarget()
		{
			Vector2 value = this.target;
			do
			{
				float length = Calc.Random.NextFloat(32f);
				float angleRadians = Calc.Random.NextFloat(6.2831855f);
				this.target = this.start + Calc.AngleToVector(angleRadians, length);
			}
			while ((value - this.target).Length() < 8f);
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x0003D8F4 File Offset: 0x0003BAF4
		public override void Update()
		{
			base.Update();
			if (this.following == null)
			{
				this.targetTimer -= Engine.DeltaTime;
				if (this.targetTimer <= 0f)
				{
					this.targetTimer = Calc.Random.Range(0.8f, 4f);
					this.GetRandomTarget();
				}
			}
			else
			{
				this.followingTime -= Engine.DeltaTime;
				this.targetTimer -= Engine.DeltaTime;
				if (this.targetTimer <= 0f)
				{
					this.targetTimer = Calc.Random.Range(0.8f, 2f);
					this.GetFollowOffset();
				}
				this.target = this.following.Center + this.followingOffset;
				if ((this.Position - this.start).Length() > 200f || this.followingTime <= 0f)
				{
					this.following = null;
					this.targetTimer = 0f;
				}
			}
			Vector2 value = (this.target - this.Position).SafeNormalize();
			this.speed += value * ((this.following == null) ? 90f : 120f) * Engine.DeltaTime;
			this.speed = this.speed.SafeNormalize() * Math.Min(this.speed.Length(), (this.following == null) ? 40f : 70f);
			this.bump = this.bump.SafeNormalize() * Calc.Approach(this.bump.Length(), 0f, Engine.DeltaTime * 80f);
			this.Position += (this.speed + this.bump) * Engine.DeltaTime;
			Vector2 position = this.Position;
			for (int i = 0; i < this.trail.Length; i++)
			{
				Vector2 vector = (this.trail[i].Position - position).SafeNormalize();
				if (vector == Vector2.Zero)
				{
					vector = new Vector2(0f, 1f);
				}
				vector.Y += 0.05f;
				Vector2 vector2 = position + vector * 2f;
				this.trail[i].Position = Calc.Approach(this.trail[i].Position, vector2, 128f * Engine.DeltaTime);
				position = this.trail[i].Position;
			}
			base.X = Calc.Clamp(base.X, (float)(this.originLevelBounds.Left + 4), (float)(this.originLevelBounds.Right - 4));
			base.Y = Calc.Clamp(base.Y, (float)(this.originLevelBounds.Top + 4), (float)(this.originLevelBounds.Bottom - 4));
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x0003DC0C File Offset: 0x0003BE0C
		public override void Render()
		{
			Vector2 position = this.Position;
			this.Position = this.Position.Floor();
			for (int i = this.trail.Length - 1; i >= 0; i--)
			{
				Vector2 position2 = this.trail[i].Position;
				float num = Calc.ClampedMap((float)i, 0f, (float)(this.trail.Length - 1), 3f, 1f);
				Draw.Rect(position2.X - num / 2f, position2.Y - num / 2f, num, num, this.trail[i].Color);
			}
			base.Render();
			this.Position = position;
		}

		// Token: 0x04000A97 RID: 2711
		private MoonCreature.TrailNode[] trail;

		// Token: 0x04000A98 RID: 2712
		private Vector2 start;

		// Token: 0x04000A99 RID: 2713
		private Vector2 target;

		// Token: 0x04000A9A RID: 2714
		private float targetTimer;

		// Token: 0x04000A9B RID: 2715
		private Vector2 speed;

		// Token: 0x04000A9C RID: 2716
		private Vector2 bump;

		// Token: 0x04000A9D RID: 2717
		private Player following;

		// Token: 0x04000A9E RID: 2718
		private Vector2 followingOffset;

		// Token: 0x04000A9F RID: 2719
		private float followingTime;

		// Token: 0x04000AA0 RID: 2720
		private Color OrbColor;

		// Token: 0x04000AA1 RID: 2721
		private Color CenterColor;

		// Token: 0x04000AA2 RID: 2722
		private Sprite Sprite;

		// Token: 0x04000AA3 RID: 2723
		private const float Acceleration = 90f;

		// Token: 0x04000AA4 RID: 2724
		private const float FollowAcceleration = 120f;

		// Token: 0x04000AA5 RID: 2725
		private const float MaxSpeed = 40f;

		// Token: 0x04000AA6 RID: 2726
		private const float MaxFollowSpeed = 70f;

		// Token: 0x04000AA7 RID: 2727
		private const float MaxFollowDistance = 200f;

		// Token: 0x04000AA8 RID: 2728
		private readonly int spawn;

		// Token: 0x04000AA9 RID: 2729
		private Rectangle originLevelBounds;

		// Token: 0x020004C2 RID: 1218
		private struct TrailNode
		{
			// Token: 0x04002371 RID: 9073
			public Vector2 Position;

			// Token: 0x04002372 RID: 9074
			public Color Color;
		}
	}
}
