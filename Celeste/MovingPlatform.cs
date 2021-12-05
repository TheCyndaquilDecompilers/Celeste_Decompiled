using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000324 RID: 804
	public class MovingPlatform : JumpThru
	{
		// Token: 0x06001958 RID: 6488 RVA: 0x000A2AF8 File Offset: 0x000A0CF8
		public MovingPlatform(Vector2 position, int width, Vector2 node) : base(position, width, false)
		{
			this.start = this.Position;
			this.end = node;
			base.Add(this.sfx = new SoundSource());
			this.SurfaceSoundIndex = 5;
			this.lastSfx = ((Math.Sign(this.start.X - this.end.X) > 0 || Math.Sign(this.start.Y - this.end.Y) > 0) ? "event:/game/03_resort/platform_horiz_left" : "event:/game/03_resort/platform_horiz_right");
			Tween tween = Tween.Create(Tween.TweenMode.YoyoLooping, Ease.SineInOut, 2f, false);
			tween.OnUpdate = delegate(Tween t)
			{
				base.MoveTo(Vector2.Lerp(this.start, this.end, t.Eased) + Vector2.UnitY * this.addY);
			};
			tween.OnStart = delegate(Tween t)
			{
				if (this.lastSfx == "event:/game/03_resort/platform_horiz_left")
				{
					this.sfx.Play(this.lastSfx = "event:/game/03_resort/platform_horiz_right", null, 0f);
					return;
				}
				this.sfx.Play(this.lastSfx = "event:/game/03_resort/platform_horiz_left", null, 0f);
			};
			base.Add(tween);
			tween.Start(false);
			base.Add(new LightOcclude(0.2f));
		}

		// Token: 0x06001959 RID: 6489 RVA: 0x000A2BE0 File Offset: 0x000A0DE0
		public MovingPlatform(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Nodes[0] + offset)
		{
		}

		// Token: 0x0600195A RID: 6490 RVA: 0x000A2C0C File Offset: 0x000A0E0C
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Session session = base.SceneAs<Level>().Session;
			MTexture mtexture;
			if (session.Area.ID == 7 && session.Level.StartsWith("e-"))
			{
				mtexture = GFX.Game["objects/woodPlatform/" + AreaData.Get(4).WoodPlatform];
			}
			else
			{
				mtexture = GFX.Game["objects/woodPlatform/" + AreaData.Get(scene).WoodPlatform];
			}
			this.textures = new MTexture[mtexture.Width / 8];
			for (int i = 0; i < this.textures.Length; i++)
			{
				this.textures[i] = mtexture.GetSubtexture(i * 8, 0, 8, 8, null);
			}
			Vector2 value = new Vector2(base.Width, base.Height + 4f) / 2f;
			scene.Add(new MovingPlatformLine(this.start + value, this.end + value));
		}

		// Token: 0x0600195B RID: 6491 RVA: 0x000A2D10 File Offset: 0x000A0F10
		public override void Render()
		{
			this.textures[0].Draw(this.Position);
			int num = 8;
			while ((float)num < base.Width - 8f)
			{
				this.textures[1].Draw(this.Position + new Vector2((float)num, 0f));
				num += 8;
			}
			this.textures[3].Draw(this.Position + new Vector2(base.Width - 8f, 0f));
			this.textures[2].Draw(this.Position + new Vector2(base.Width / 2f - 4f, 0f));
		}

		// Token: 0x0600195C RID: 6492 RVA: 0x000A2DCE File Offset: 0x000A0FCE
		public override void OnStaticMoverTrigger(StaticMover sm)
		{
			this.sinkTimer = 0.4f;
		}

		// Token: 0x0600195D RID: 6493 RVA: 0x000A2DDC File Offset: 0x000A0FDC
		public override void Update()
		{
			base.Update();
			if (base.HasPlayerRider())
			{
				this.sinkTimer = 0.2f;
				this.addY = Calc.Approach(this.addY, 3f, 50f * Engine.DeltaTime);
				return;
			}
			if (this.sinkTimer > 0f)
			{
				this.sinkTimer -= Engine.DeltaTime;
				this.addY = Calc.Approach(this.addY, 3f, 50f * Engine.DeltaTime);
				return;
			}
			this.addY = Calc.Approach(this.addY, 0f, 20f * Engine.DeltaTime);
		}

		// Token: 0x04001612 RID: 5650
		private Vector2 start;

		// Token: 0x04001613 RID: 5651
		private Vector2 end;

		// Token: 0x04001614 RID: 5652
		private float addY;

		// Token: 0x04001615 RID: 5653
		private float sinkTimer;

		// Token: 0x04001616 RID: 5654
		private MTexture[] textures;

		// Token: 0x04001617 RID: 5655
		private string lastSfx;

		// Token: 0x04001618 RID: 5656
		private SoundSource sfx;
	}
}
