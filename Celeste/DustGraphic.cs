using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002B9 RID: 697
	[Tracked(false)]
	public class DustGraphic : Component
	{
		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06001578 RID: 5496 RVA: 0x0007B73F File Offset: 0x0007993F
		// (set) Token: 0x06001579 RID: 5497 RVA: 0x0007B747 File Offset: 0x00079947
		public bool Estableshed { get; private set; }

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x0600157A RID: 5498 RVA: 0x0007B750 File Offset: 0x00079950
		public Vector2 RenderPosition
		{
			get
			{
				return base.Entity.Position + this.Position + this.shakeValue;
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x0600157B RID: 5499 RVA: 0x0007B774 File Offset: 0x00079974
		private bool InView
		{
			get
			{
				Camera camera = (base.Scene as Level).Camera;
				Vector2 position = base.Entity.Position;
				return position.X + 16f >= camera.Left && position.Y + 16f >= camera.Top && position.X - 16f <= camera.Right && position.Y - 16f <= camera.Bottom;
			}
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x0007B7F4 File Offset: 0x000799F4
		public DustGraphic(bool ignoreSolids, bool autoControlEyes = false, bool autoExpandDust = false) : base(true, true)
		{
			this.ignoreSolids = ignoreSolids;
			this.autoControlEyes = autoControlEyes;
			this.autoExpandDust = autoExpandDust;
			this.center = Calc.Random.Choose(GFX.Game.GetAtlasSubtextures("danger/dustcreature/center"));
			this.offset = Calc.Random.NextFloat() * 4f;
			this.timer = Calc.Random.NextFloat();
			this.EyeTargetDirection = (this.EyeDirection = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f));
			this.eyeTextureIndex = Calc.Random.Next(128);
			this.eyesExist = true;
			if (autoControlEyes)
			{
				this.eyesExist = Calc.Random.Chance(0.5f);
				this.eyesFollowPlayer = Calc.Random.Chance(0.3f);
			}
			this.randomSeed = Calc.Random.Next();
		}

		// Token: 0x0600157D RID: 5501 RVA: 0x0007B93C File Offset: 0x00079B3C
		public override void Added(Entity entity)
		{
			base.Added(entity);
			entity.Add(new TransitionListener
			{
				OnIn = delegate(float f)
				{
					this.AddDustNodesIfInCamera();
				}
			});
			entity.Add(new DustEdge(new Action(this.Render)));
		}

		// Token: 0x0600157E RID: 5502 RVA: 0x0007B988 File Offset: 0x00079B88
		public override void Update()
		{
			this.timer += Engine.DeltaTime * 0.6f;
			bool inView = this.InView;
			if (this.shakeTimer > 0f)
			{
				this.shakeTimer -= Engine.DeltaTime;
				if (this.shakeTimer <= 0f)
				{
					this.shakeValue = Vector2.Zero;
				}
				else if (base.Scene.OnInterval(0.05f))
				{
					this.shakeValue = Calc.Random.ShakeVector();
				}
			}
			if (this.eyesExist)
			{
				if (this.EyeDirection != this.EyeTargetDirection && inView)
				{
					if (!this.eyesMoveByRotation)
					{
						this.EyeDirection = Calc.Approach(this.EyeDirection, this.EyeTargetDirection, 12f * Engine.DeltaTime);
					}
					else
					{
						float num = this.EyeDirection.Angle();
						float num2 = this.EyeTargetDirection.Angle();
						num = Calc.AngleApproach(num, num2, 8f * Engine.DeltaTime);
						if (num == num2)
						{
							this.EyeDirection = this.EyeTargetDirection;
						}
						else
						{
							this.EyeDirection = Calc.AngleToVector(num, 1f);
						}
					}
				}
				if (this.eyesFollowPlayer && inView)
				{
					Player entity = base.Entity.Scene.Tracker.GetEntity<Player>();
					if (entity != null)
					{
						Vector2 vector = (entity.Position - base.Entity.Position).SafeNormalize();
						if (this.eyesMoveByRotation)
						{
							float target = vector.Angle();
							float val = this.eyeLookRange.Angle();
							this.EyeTargetDirection = Calc.AngleToVector(Calc.AngleApproach(val, target, 0.7853982f), 1f);
						}
						else
						{
							this.EyeTargetDirection = vector;
						}
					}
				}
				if (this.blink != null)
				{
					this.blink.Update();
				}
			}
			if (this.nodes.Count <= 0 && base.Entity.Scene != null && !this.Estableshed)
			{
				this.AddDustNodesIfInCamera();
				return;
			}
			foreach (DustGraphic.Node node in this.nodes)
			{
				node.Rotation += Engine.DeltaTime * 0.5f;
			}
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x0007BBC4 File Offset: 0x00079DC4
		public void OnHitPlayer()
		{
			if (SaveData.Instance.Assists.Invincible)
			{
				return;
			}
			this.shakeTimer = 0.6f;
			if (this.eyesExist)
			{
				this.blink = null;
				this.leftEyeVisible = true;
				this.rightEyeVisible = true;
				this.eyeTexture = GFX.Game["danger/dustcreature/deadEyes"];
			}
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x0007BC20 File Offset: 0x00079E20
		public void AddDustNodesIfInCamera()
		{
			if (this.nodes.Count > 0 || !this.InView || DustEdges.DustGraphicEstabledCounter > 25 || this.Estableshed)
			{
				return;
			}
			Calc.PushRandom(this.randomSeed);
			int num = (int)base.Entity.X;
			int num2 = (int)base.Entity.Y;
			Vector2 vector = new Vector2(1f, 1f).SafeNormalize();
			this.AddNode(new Vector2(-vector.X, -vector.Y), this.ignoreSolids || !base.Entity.Scene.CollideCheck<Solid>(new Rectangle(num - 8, num2 - 8, 8, 8)));
			this.AddNode(new Vector2(vector.X, -vector.Y), this.ignoreSolids || !base.Entity.Scene.CollideCheck<Solid>(new Rectangle(num, num2 - 8, 8, 8)));
			this.AddNode(new Vector2(-vector.X, vector.Y), this.ignoreSolids || !base.Entity.Scene.CollideCheck<Solid>(new Rectangle(num - 8, num2, 8, 8)));
			this.AddNode(new Vector2(vector.X, vector.Y), this.ignoreSolids || !base.Entity.Scene.CollideCheck<Solid>(new Rectangle(num, num2, 8, 8)));
			if (this.nodes[0].Enabled || this.nodes[2].Enabled)
			{
				this.Position.X = this.Position.X - 1f;
			}
			if (this.nodes[1].Enabled || this.nodes[3].Enabled)
			{
				this.Position.X = this.Position.X + 1f;
			}
			if (this.nodes[0].Enabled || this.nodes[1].Enabled)
			{
				this.Position.Y = this.Position.Y - 1f;
			}
			if (this.nodes[2].Enabled || this.nodes[3].Enabled)
			{
				this.Position.Y = this.Position.Y + 1f;
			}
			int num3 = 0;
			using (List<DustGraphic.Node>.Enumerator enumerator = this.nodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Enabled)
					{
						num3++;
					}
				}
			}
			this.eyesMoveByRotation = (num3 < 4);
			if (this.autoControlEyes && this.eyesExist && this.eyesMoveByRotation)
			{
				this.eyeLookRange = Vector2.Zero;
				if (this.nodes[0].Enabled)
				{
					this.eyeLookRange += new Vector2(-1f, -1f).SafeNormalize();
				}
				if (this.nodes[1].Enabled)
				{
					this.eyeLookRange += new Vector2(1f, -1f).SafeNormalize();
				}
				if (this.nodes[2].Enabled)
				{
					this.eyeLookRange += new Vector2(-1f, 1f).SafeNormalize();
				}
				if (this.nodes[3].Enabled)
				{
					this.eyeLookRange += new Vector2(1f, 1f).SafeNormalize();
				}
				if (num3 > 0 && this.eyeLookRange.Length() > 0f)
				{
					this.eyeLookRange /= (float)num3;
					this.eyeLookRange = this.eyeLookRange.SafeNormalize();
				}
				this.EyeTargetDirection = (this.EyeDirection = this.eyeLookRange);
			}
			if (this.eyesExist)
			{
				this.blink = new Coroutine(this.BlinkRoutine(), true);
				List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(DustStyles.Get(base.Scene).EyeTextures);
				this.eyeTexture = atlasSubtextures[this.eyeTextureIndex % atlasSubtextures.Count];
				base.Entity.Scene.Add(this.eyes = new DustGraphic.Eyeballs(this));
			}
			DustEdges.DustGraphicEstabledCounter++;
			this.Estableshed = true;
			if (this.OnEstablish != null)
			{
				this.OnEstablish();
			}
			Calc.PopRandom();
		}

		// Token: 0x06001581 RID: 5505 RVA: 0x0007C0CC File Offset: 0x0007A2CC
		private void AddNode(Vector2 angle, bool enabled)
		{
			Vector2 value = new Vector2(1f, 1f);
			if (this.autoExpandDust)
			{
				int num = Math.Sign(angle.X);
				int num2 = Math.Sign(angle.Y);
				base.Entity.Collidable = false;
				if (base.Scene.CollideCheck<Solid>(new Rectangle((int)(base.Entity.X - 4f + (float)(num * 16)), (int)(base.Entity.Y - 4f + (float)(num2 * 4)), 8, 8)) || base.Scene.CollideCheck<DustStaticSpinner>(new Rectangle((int)(base.Entity.X - 4f + (float)(num * 16)), (int)(base.Entity.Y - 4f + (float)(num2 * 4)), 8, 8)))
				{
					value.X = 5f;
				}
				if (base.Scene.CollideCheck<Solid>(new Rectangle((int)(base.Entity.X - 4f + (float)(num * 4)), (int)(base.Entity.Y - 4f + (float)(num2 * 16)), 8, 8)) || base.Scene.CollideCheck<DustStaticSpinner>(new Rectangle((int)(base.Entity.X - 4f + (float)(num * 4)), (int)(base.Entity.Y - 4f + (float)(num2 * 16)), 8, 8)))
				{
					value.Y = 5f;
				}
				base.Entity.Collidable = true;
			}
			DustGraphic.Node node = new DustGraphic.Node();
			node.Base = Calc.Random.Choose(GFX.Game.GetAtlasSubtextures("danger/dustcreature/base"));
			node.Overlay = Calc.Random.Choose(GFX.Game.GetAtlasSubtextures("danger/dustcreature/overlay"));
			node.Rotation = Calc.Random.NextFloat(6.2831855f);
			node.Angle = angle * value;
			node.Enabled = enabled;
			this.nodes.Add(node);
			if (angle.X < 0f)
			{
				this.LeftNodes.Add(node);
			}
			else
			{
				this.RightNodes.Add(node);
			}
			if (angle.Y < 0f)
			{
				this.TopNodes.Add(node);
				return;
			}
			this.BottomNodes.Add(node);
		}

		// Token: 0x06001582 RID: 5506 RVA: 0x0007C30F File Offset: 0x0007A50F
		private IEnumerator BlinkRoutine()
		{
			for (;;)
			{
				yield return 2f + Calc.Random.NextFloat(1.5f);
				this.leftEyeVisible = false;
				yield return 0.02f + Calc.Random.NextFloat(0.05f);
				this.rightEyeVisible = false;
				yield return 0.25f;
				this.leftEyeVisible = (this.rightEyeVisible = true);
			}
			yield break;
		}

		// Token: 0x06001583 RID: 5507 RVA: 0x0007C320 File Offset: 0x0007A520
		public override void Render()
		{
			if (!this.InView)
			{
				return;
			}
			Vector2 renderPosition = this.RenderPosition;
			foreach (DustGraphic.Node node in this.nodes)
			{
				if (node.Enabled)
				{
					node.Base.DrawCentered(renderPosition + node.Angle * this.Scale, Color.White, this.Scale, node.Rotation);
					node.Overlay.DrawCentered(renderPosition + node.Angle * this.Scale, Color.White, this.Scale, -node.Rotation);
				}
			}
			this.center.DrawCentered(renderPosition, Color.White, this.Scale, this.timer);
		}

		// Token: 0x0400118D RID: 4493
		public Vector2 Position;

		// Token: 0x0400118E RID: 4494
		public float Scale = 1f;

		// Token: 0x04001190 RID: 4496
		private MTexture center;

		// Token: 0x04001191 RID: 4497
		public Action OnEstablish;

		// Token: 0x04001192 RID: 4498
		private List<DustGraphic.Node> nodes = new List<DustGraphic.Node>();

		// Token: 0x04001193 RID: 4499
		public List<DustGraphic.Node> LeftNodes = new List<DustGraphic.Node>();

		// Token: 0x04001194 RID: 4500
		public List<DustGraphic.Node> RightNodes = new List<DustGraphic.Node>();

		// Token: 0x04001195 RID: 4501
		public List<DustGraphic.Node> TopNodes = new List<DustGraphic.Node>();

		// Token: 0x04001196 RID: 4502
		public List<DustGraphic.Node> BottomNodes = new List<DustGraphic.Node>();

		// Token: 0x04001197 RID: 4503
		public Vector2 EyeTargetDirection;

		// Token: 0x04001198 RID: 4504
		public Vector2 EyeDirection;

		// Token: 0x04001199 RID: 4505
		public int EyeFlip = 1;

		// Token: 0x0400119A RID: 4506
		private bool eyesExist;

		// Token: 0x0400119B RID: 4507
		private int eyeTextureIndex;

		// Token: 0x0400119C RID: 4508
		private MTexture eyeTexture;

		// Token: 0x0400119D RID: 4509
		private Vector2 eyeLookRange;

		// Token: 0x0400119E RID: 4510
		private bool eyesMoveByRotation;

		// Token: 0x0400119F RID: 4511
		private bool autoControlEyes;

		// Token: 0x040011A0 RID: 4512
		private bool eyesFollowPlayer;

		// Token: 0x040011A1 RID: 4513
		private Coroutine blink;

		// Token: 0x040011A2 RID: 4514
		private bool leftEyeVisible = true;

		// Token: 0x040011A3 RID: 4515
		private bool rightEyeVisible = true;

		// Token: 0x040011A4 RID: 4516
		private DustGraphic.Eyeballs eyes;

		// Token: 0x040011A5 RID: 4517
		private float timer;

		// Token: 0x040011A6 RID: 4518
		private float offset;

		// Token: 0x040011A7 RID: 4519
		private bool ignoreSolids;

		// Token: 0x040011A8 RID: 4520
		private bool autoExpandDust;

		// Token: 0x040011A9 RID: 4521
		private float shakeTimer;

		// Token: 0x040011AA RID: 4522
		private Vector2 shakeValue;

		// Token: 0x040011AB RID: 4523
		private int randomSeed;

		// Token: 0x02000645 RID: 1605
		public class Node
		{
			// Token: 0x040029EC RID: 10732
			public MTexture Base;

			// Token: 0x040029ED RID: 10733
			public MTexture Overlay;

			// Token: 0x040029EE RID: 10734
			public float Rotation;

			// Token: 0x040029EF RID: 10735
			public Vector2 Angle;

			// Token: 0x040029F0 RID: 10736
			public bool Enabled;
		}

		// Token: 0x02000646 RID: 1606
		private class Eyeballs : Entity
		{
			// Token: 0x06002ADF RID: 10975 RVA: 0x001121AE File Offset: 0x001103AE
			public Eyeballs(DustGraphic dust)
			{
				this.Dust = dust;
				base.Depth = this.Dust.Entity.Depth - 1;
			}

			// Token: 0x06002AE0 RID: 10976 RVA: 0x001121D5 File Offset: 0x001103D5
			public override void Added(Scene scene)
			{
				base.Added(scene);
				this.Color = DustStyles.Get(scene).EyeColor;
			}

			// Token: 0x06002AE1 RID: 10977 RVA: 0x001121EF File Offset: 0x001103EF
			public override void Update()
			{
				base.Update();
				if (this.Dust.Entity == null || this.Dust.Scene == null)
				{
					base.RemoveSelf();
				}
			}

			// Token: 0x06002AE2 RID: 10978 RVA: 0x00112218 File Offset: 0x00110418
			public override void Render()
			{
				if (this.Dust.Visible && this.Dust.Entity.Visible)
				{
					Vector2 value = new Vector2(-this.Dust.EyeDirection.Y, this.Dust.EyeDirection.X).SafeNormalize();
					if (this.Dust.leftEyeVisible)
					{
						this.Dust.eyeTexture.DrawCentered(this.Dust.RenderPosition + (this.Dust.EyeDirection * 5f + value * 3f) * this.Dust.Scale, this.Color, this.Dust.Scale);
					}
					if (this.Dust.rightEyeVisible)
					{
						this.Dust.eyeTexture.DrawCentered(this.Dust.RenderPosition + (this.Dust.EyeDirection * 5f - value * 3f) * this.Dust.Scale, this.Color, this.Dust.Scale);
					}
				}
			}

			// Token: 0x040029F1 RID: 10737
			public DustGraphic Dust;

			// Token: 0x040029F2 RID: 10738
			public Color Color;
		}
	}
}
