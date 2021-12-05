using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001A8 RID: 424
	public class FlingBird : Entity
	{
		// Token: 0x06000EBC RID: 3772 RVA: 0x00037A2C File Offset: 0x00035C2C
		public FlingBird(Vector2[] nodes, bool skippable) : base(nodes[0])
		{
			base.Depth = -1;
			base.Add(this.sprite = GFX.SpriteBank.Create("bird"));
			this.sprite.Play("hover", false, false);
			this.sprite.Scale.X = -1f;
			this.sprite.Position = this.spriteOffset;
			this.sprite.OnFrameChange = delegate(string spr)
			{
				BirdNPC.FlapSfxCheck(this.sprite);
			};
			base.Collider = new Circle(16f, 0f, 0f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(this.moveSfx = new SoundSource());
			this.NodeSegments = new List<Vector2[]>();
			this.NodeSegments.Add(nodes);
			this.SegmentsWaiting = new List<bool>();
			this.SegmentsWaiting.Add(skippable);
			base.Add(new TransitionListener
			{
				OnOut = delegate(float t)
				{
					this.sprite.Color = Color.White * (1f - Calc.Map(t, 0f, 0.4f, 0f, 1f));
				}
			});
		}

		// Token: 0x06000EBD RID: 3773 RVA: 0x00037B73 File Offset: 0x00035D73
		public FlingBird(EntityData data, Vector2 levelOffset) : this(data.NodesWithPosition(levelOffset), data.Bool("waiting", false))
		{
			this.entityData = data;
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x00037B98 File Offset: 0x00035D98
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			List<FlingBird> list = base.Scene.Entities.FindAll<FlingBird>();
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i].entityData.Level.Name != this.entityData.Level.Name)
				{
					list.RemoveAt(i);
				}
			}
			list.Sort((FlingBird a, FlingBird b) => Math.Sign(a.X - b.X));
			if (list[0] == this)
			{
				for (int j = 1; j < list.Count; j++)
				{
					this.NodeSegments.Add(list[j].NodeSegments[0]);
					this.SegmentsWaiting.Add(list[j].SegmentsWaiting[0]);
					list[j].RemoveSelf();
				}
			}
			if (this.SegmentsWaiting[0])
			{
				this.sprite.Play("hoverStressed", false, false);
				this.sprite.Scale.X = 1f;
			}
			Player entity = scene.Tracker.GetEntity<Player>();
			if (entity != null && entity.X > base.X)
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06000EBF RID: 3775 RVA: 0x00037CE2 File Offset: 0x00035EE2
		private void Skip()
		{
			this.state = FlingBird.States.Move;
			base.Add(new Coroutine(this.MoveRoutine(), true));
		}

		// Token: 0x06000EC0 RID: 3776 RVA: 0x00037D00 File Offset: 0x00035F00
		private void OnPlayer(Player player)
		{
			if (this.state == FlingBird.States.Wait && player.DoFlingBird(this))
			{
				this.flingSpeed = player.Speed * 0.4f;
				this.flingSpeed.Y = 120f;
				this.flingTargetSpeed = Vector2.Zero;
				this.flingAccel = 1000f;
				player.Speed = Vector2.Zero;
				this.state = FlingBird.States.Fling;
				base.Add(new Coroutine(this.DoFlingRoutine(player), true));
				Audio.Play("event:/new_content/game/10_farewell/bird_throw", base.Center);
			}
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x00037D90 File Offset: 0x00035F90
		public override void Update()
		{
			base.Update();
			if (this.state != FlingBird.States.Wait)
			{
				this.sprite.Position = Calc.Approach(this.sprite.Position, this.spriteOffset, 32f * Engine.DeltaTime);
			}
			switch (this.state)
			{
			case FlingBird.States.Wait:
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.X - base.X >= 100f)
				{
					this.Skip();
					return;
				}
				if (this.SegmentsWaiting[this.segmentIndex] && this.LightningRemoved)
				{
					this.Skip();
					return;
				}
				if (entity != null)
				{
					float scaleFactor = Calc.ClampedMap((entity.Center - this.Position).Length(), 16f, 64f, 12f, 0f);
					Vector2 value = (entity.Center - this.Position).SafeNormalize();
					this.sprite.Position = Calc.Approach(this.sprite.Position, this.spriteOffset + value * scaleFactor, 32f * Engine.DeltaTime);
					return;
				}
				break;
			}
			case FlingBird.States.Fling:
				if (this.flingAccel > 0f)
				{
					this.flingSpeed = Calc.Approach(this.flingSpeed, this.flingTargetSpeed, this.flingAccel * Engine.DeltaTime);
				}
				this.Position += this.flingSpeed * Engine.DeltaTime;
				return;
			case FlingBird.States.Move:
				break;
			case FlingBird.States.WaitForLightningClear:
				if (base.Scene.Entities.FindFirst<Lightning>() == null || base.X > (float)(base.Scene as Level).Bounds.Right)
				{
					this.sprite.Scale.X = 1f;
					this.state = FlingBird.States.Leaving;
					base.Add(new Coroutine(this.LeaveRoutine(), true));
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x00037F84 File Offset: 0x00036184
		private IEnumerator DoFlingRoutine(Player player)
		{
			Level level = base.Scene as Level;
			Vector2 position = level.Camera.Position;
			Vector2 vector = player.Position - position;
			vector.X = Calc.Clamp(vector.X, 145f, 215f);
			vector.Y = Calc.Clamp(vector.Y, 85f, 95f);
			base.Add(new Coroutine(level.ZoomTo(vector, 1.1f, 0.2f), true));
			Engine.TimeRate = 0.8f;
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			while (this.flingSpeed != Vector2.Zero)
			{
				yield return null;
			}
			this.sprite.Play("throw", false, false);
			this.sprite.Scale.X = 1f;
			this.flingSpeed = new Vector2(-140f, 140f);
			this.flingTargetSpeed = Vector2.Zero;
			this.flingAccel = 1400f;
			yield return 0.1f;
			Celeste.Freeze(0.05f);
			this.flingTargetSpeed = FlingBird.FlingSpeed;
			this.flingAccel = 6000f;
			yield return 0.1f;
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			Engine.TimeRate = 1f;
			level.Shake(0.3f);
			base.Add(new Coroutine(level.ZoomBack(0.1f), true));
			player.FinishFlingBird();
			this.flingTargetSpeed = Vector2.Zero;
			this.flingAccel = 4000f;
			yield return 0.3f;
			base.Add(new Coroutine(this.MoveRoutine(), true));
			yield break;
		}

		// Token: 0x06000EC3 RID: 3779 RVA: 0x00037F9A File Offset: 0x0003619A
		private IEnumerator MoveRoutine()
		{
			this.state = FlingBird.States.Move;
			this.sprite.Play("fly", false, false);
			this.sprite.Scale.X = 1f;
			this.moveSfx.Play("event:/new_content/game/10_farewell/bird_relocate", null, 0f);
			for (int nodeIndex = 1; nodeIndex < this.NodeSegments[this.segmentIndex].Length - 1; nodeIndex += 2)
			{
				Vector2 position = this.Position;
				Vector2 anchor = this.NodeSegments[this.segmentIndex][nodeIndex];
				Vector2 to = this.NodeSegments[this.segmentIndex][nodeIndex + 1];
				yield return this.MoveOnCurve(position, anchor, to);
			}
			this.segmentIndex++;
			bool atEnding = this.segmentIndex >= this.NodeSegments.Count;
			if (!atEnding)
			{
				Vector2 position2 = this.Position;
				Vector2 anchor2 = this.NodeSegments[this.segmentIndex - 1][this.NodeSegments[this.segmentIndex - 1].Length - 1];
				Vector2 to2 = this.NodeSegments[this.segmentIndex][0];
				yield return this.MoveOnCurve(position2, anchor2, to2);
			}
			this.sprite.Rotation = 0f;
			this.sprite.Scale = Vector2.One;
			if (atEnding)
			{
				this.sprite.Play("hoverStressed", false, false);
				this.sprite.Scale.X = 1f;
				this.state = FlingBird.States.WaitForLightningClear;
			}
			else
			{
				if (this.SegmentsWaiting[this.segmentIndex])
				{
					this.sprite.Play("hoverStressed", false, false);
				}
				else
				{
					this.sprite.Play("hover", false, false);
				}
				this.sprite.Scale.X = -1f;
				this.state = FlingBird.States.Wait;
			}
			yield break;
		}

		// Token: 0x06000EC4 RID: 3780 RVA: 0x00037FA9 File Offset: 0x000361A9
		private IEnumerator LeaveRoutine()
		{
			this.sprite.Scale.X = 1f;
			this.sprite.Play("fly", false, false);
			Vector2 vector = new Vector2((float)((base.Scene as Level).Bounds.Right + 32), base.Y);
			yield return this.MoveOnCurve(this.Position, (this.Position + vector) * 0.5f - Vector2.UnitY * 12f, vector);
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x06000EC5 RID: 3781 RVA: 0x00037FB8 File Offset: 0x000361B8
		private IEnumerator MoveOnCurve(Vector2 from, Vector2 anchor, Vector2 to)
		{
			SimpleCurve curve = new SimpleCurve(from, to, anchor);
			float duration = curve.GetLengthParametric(32) / 500f;
			Vector2 was = from;
			for (float t = 0.016f; t <= 1f; t += Engine.DeltaTime / duration)
			{
				this.Position = curve.GetPoint(t).Floor();
				this.sprite.Rotation = Calc.Angle(curve.GetPoint(Math.Max(0f, t - 0.05f)), curve.GetPoint(Math.Min(1f, t + 0.05f)));
				this.sprite.Scale.X = 1.25f;
				this.sprite.Scale.Y = 0.7f;
				if ((was - this.Position).Length() > 32f)
				{
					TrailManager.Add(this, this.trailColor, 1f, false, false);
					was = this.Position;
				}
				yield return null;
			}
			this.Position = to;
			yield break;
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x0002FD8C File Offset: 0x0002DF8C
		public override void Render()
		{
			base.Render();
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x00037FDC File Offset: 0x000361DC
		private void DrawLine(Vector2 a, Vector2 anchor, Vector2 b)
		{
			SimpleCurve simpleCurve = new SimpleCurve(a, b, anchor);
			simpleCurve.Render(Color.Red, 32);
		}

		// Token: 0x040009F0 RID: 2544
		public static ParticleType P_Feather;

		// Token: 0x040009F1 RID: 2545
		public const float SkipDist = 100f;

		// Token: 0x040009F2 RID: 2546
		public static readonly Vector2 FlingSpeed = new Vector2(380f, -100f);

		// Token: 0x040009F3 RID: 2547
		private Vector2 spriteOffset = new Vector2(0f, 8f);

		// Token: 0x040009F4 RID: 2548
		private Sprite sprite;

		// Token: 0x040009F5 RID: 2549
		private FlingBird.States state;

		// Token: 0x040009F6 RID: 2550
		private Vector2 flingSpeed;

		// Token: 0x040009F7 RID: 2551
		private Vector2 flingTargetSpeed;

		// Token: 0x040009F8 RID: 2552
		private float flingAccel;

		// Token: 0x040009F9 RID: 2553
		private Color trailColor = Calc.HexToColor("639bff");

		// Token: 0x040009FA RID: 2554
		private EntityData entityData;

		// Token: 0x040009FB RID: 2555
		private SoundSource moveSfx;

		// Token: 0x040009FC RID: 2556
		private int segmentIndex;

		// Token: 0x040009FD RID: 2557
		public List<Vector2[]> NodeSegments;

		// Token: 0x040009FE RID: 2558
		public List<bool> SegmentsWaiting;

		// Token: 0x040009FF RID: 2559
		public bool LightningRemoved;

		// Token: 0x020004B6 RID: 1206
		private enum States
		{
			// Token: 0x0400233F RID: 9023
			Wait,
			// Token: 0x04002340 RID: 9024
			Fling,
			// Token: 0x04002341 RID: 9025
			Move,
			// Token: 0x04002342 RID: 9026
			WaitForLightningClear,
			// Token: 0x04002343 RID: 9027
			Leaving
		}
	}
}
