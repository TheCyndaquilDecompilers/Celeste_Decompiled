using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000180 RID: 384
	public class BirdPath : Entity
	{
		// Token: 0x06000D94 RID: 3476 RVA: 0x0002F9D8 File Offset: 0x0002DBD8
		public BirdPath(EntityID id, EntityData data, Vector2 offset) : this(id, data.Position + offset, data.NodesOffset(offset), data.Bool("only_once", false), data.Bool("onlyIfLeft", false), data.Float("speedMult", 1f))
		{
		}

		// Token: 0x06000D95 RID: 3477 RVA: 0x0002FA28 File Offset: 0x0002DC28
		public BirdPath(EntityID id, Vector2 position, Vector2[] nodes, bool onlyOnce, bool onlyIfLeft, float speedMult)
		{
			base.Tag = Tags.TransitionUpdate;
			this.ID = id;
			this.Position = position;
			this.start = position;
			this.nodes = nodes;
			this.onlyOnce = onlyOnce;
			this.onlyIfLeft = onlyIfLeft;
			this.speedMult = speedMult;
			this.maxspeed = 150f * speedMult;
			base.Add(this.sprite = GFX.SpriteBank.Create("bird"));
			this.sprite.Play("flyupRoll", false, false);
			this.sprite.JustifyOrigin(0.5f, 0.75f);
			base.Add(new SoundSource("event:/new_content/game/10_farewell/bird_flyuproll")
			{
				RemoveOnOneshotEnd = true
			});
			base.Add(new Coroutine(this.Routine(), true));
		}

		// Token: 0x06000D96 RID: 3478 RVA: 0x0002FB10 File Offset: 0x0002DD10
		public void WaitForTrigger()
		{
			this.Visible = (this.Active = false);
		}

		// Token: 0x06000D97 RID: 3479 RVA: 0x0002FB30 File Offset: 0x0002DD30
		public void Trigger()
		{
			this.Visible = (this.Active = true);
		}

		// Token: 0x06000D98 RID: 3480 RVA: 0x0002FB4D File Offset: 0x0002DD4D
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (this.onlyOnce)
			{
				(base.Scene as Level).Session.DoNotLoad.Add(this.ID);
			}
		}

		// Token: 0x06000D99 RID: 3481 RVA: 0x0002FB80 File Offset: 0x0002DD80
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (this.onlyIfLeft)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.X > base.X)
				{
					base.RemoveSelf();
				}
			}
		}

		// Token: 0x06000D9A RID: 3482 RVA: 0x0002FBC4 File Offset: 0x0002DDC4
		private IEnumerator Routine()
		{
			Vector2 begin = this.start;
			for (int i = 0; i <= this.nodes.Length - 1; i += 2)
			{
				Vector2 control = this.nodes[i];
				Vector2 next = this.nodes[i + 1];
				SimpleCurve curve = new SimpleCurve(begin, next, control);
				float lengthParametric = curve.GetLengthParametric(32);
				float duration = lengthParametric / this.maxspeed;
				bool playedSfx = false;
				Vector2 position = this.Position;
				for (float p = 0f; p < 1f; p += Engine.DeltaTime * this.speedMult / duration)
				{
					this.target = curve.GetPoint(p);
					if (p > 0.9f)
					{
						if (!playedSfx && this.sprite.CurrentAnimationID != "flyupRoll")
						{
							base.Add(new SoundSource("event:/new_content/game/10_farewell/bird_flyuproll")
							{
								RemoveOnOneshotEnd = true
							});
							playedSfx = true;
						}
						this.sprite.Play("flyupRoll", false, false);
					}
					yield return null;
				}
				begin = next;
				next = default(Vector2);
				curve = default(SimpleCurve);
			}
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x06000D9B RID: 3483 RVA: 0x0002FBD4 File Offset: 0x0002DDD4
		public override void Update()
		{
			if ((base.Scene as Level).Transitioning)
			{
				foreach (Component component in this)
				{
					SoundSource soundSource = component as SoundSource;
					if (soundSource != null)
					{
						soundSource.UpdateSfxPosition();
					}
				}
				return;
			}
			base.Update();
			int num = Math.Sign(base.X - this.target.X);
			Vector2 value = (this.target - this.Position).SafeNormalize();
			float scaleFactor = 800f;
			this.speed += value * scaleFactor * Engine.DeltaTime;
			if (this.speed.Length() > this.maxspeed)
			{
				this.speed = this.speed.SafeNormalize(this.maxspeed);
			}
			this.Position += this.speed * Engine.DeltaTime;
			if (num != Math.Sign(base.X - this.target.X))
			{
				this.speed.X = this.speed.X * 0.75f;
			}
			float startAngle = this.speed.Angle();
			float endAngle = Calc.Angle(this.Position, this.target);
			float num2 = Calc.AngleLerp(startAngle, endAngle, 0.5f);
			this.sprite.Rotation = 1.5707964f + num2;
			if ((this.lastTrail - this.Position).Length() > 32f)
			{
				TrailManager.Add(this, this.trailColor, 1f, false, false);
				this.lastTrail = this.Position;
			}
		}

		// Token: 0x06000D9C RID: 3484 RVA: 0x0002FD8C File Offset: 0x0002DF8C
		public override void Render()
		{
			base.Render();
		}

		// Token: 0x06000D9D RID: 3485 RVA: 0x0002FD94 File Offset: 0x0002DF94
		public override void DebugRender(Camera camera)
		{
			Vector2 begin = this.start;
			for (int i = 0; i < this.nodes.Length - 1; i += 2)
			{
				Vector2 vector = this.nodes[i + 1];
				SimpleCurve simpleCurve = new SimpleCurve(begin, vector, this.nodes[i]);
				simpleCurve.Render(Color.Red * 0.25f, 32);
				begin = vector;
			}
			Draw.Line(this.Position, this.Position + (this.target - this.Position).SafeNormalize() * ((this.target - this.Position).Length() - 3f), Color.Yellow);
			Draw.Circle(this.target, 3f, Color.Yellow, 16);
		}

		// Token: 0x040008CD RID: 2253
		private Vector2 start;

		// Token: 0x040008CE RID: 2254
		private Sprite sprite;

		// Token: 0x040008CF RID: 2255
		private Vector2[] nodes;

		// Token: 0x040008D0 RID: 2256
		private Color trailColor = Calc.HexToColor("639bff");

		// Token: 0x040008D1 RID: 2257
		private Vector2 target;

		// Token: 0x040008D2 RID: 2258
		private Vector2 speed;

		// Token: 0x040008D3 RID: 2259
		private float maxspeed;

		// Token: 0x040008D4 RID: 2260
		private Vector2 lastTrail;

		// Token: 0x040008D5 RID: 2261
		private float speedMult;

		// Token: 0x040008D6 RID: 2262
		private EntityID ID;

		// Token: 0x040008D7 RID: 2263
		private bool onlyOnce;

		// Token: 0x040008D8 RID: 2264
		private bool onlyIfLeft;
	}
}
