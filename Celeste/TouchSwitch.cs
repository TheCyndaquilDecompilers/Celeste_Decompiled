using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200034B RID: 843
	[Tracked(false)]
	public class TouchSwitch : Entity
	{
		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06001A71 RID: 6769 RVA: 0x000AA53E File Offset: 0x000A873E
		private Level level
		{
			get
			{
				return (Level)base.Scene;
			}
		}

		// Token: 0x06001A72 RID: 6770 RVA: 0x000AA54C File Offset: 0x000A874C
		public TouchSwitch(Vector2 position) : base(position)
		{
			base.Depth = 2000;
			base.Add(this.Switch = new Switch(false));
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, new Hitbox(30f, 30f, -15f, -15f)));
			base.Add(this.icon);
			base.Add(this.bloom = new BloomPoint(0f, 16f));
			this.bloom.Alpha = 0f;
			this.icon.Add("idle", "", 0f, new int[1]);
			this.icon.Add("spin", "", 0.1f, new Chooser<string>("spin", 1f), new int[]
			{
				0,
				1,
				2,
				3,
				4,
				5
			});
			this.icon.Play("spin", false, false);
			this.icon.Color = this.inactiveColor;
			this.icon.CenterOrigin();
			base.Collider = new Hitbox(16f, 16f, -8f, -8f);
			base.Add(new HoldableCollider(new Action<Holdable>(this.OnHoldable), new Hitbox(20f, 20f, -10f, -10f)));
			base.Add(new SeekerCollider(new Action<Seeker>(this.OnSeeker), new Hitbox(24f, 24f, -12f, -12f)));
			this.Switch.OnActivate = delegate()
			{
				this.wiggler.Start();
				for (int i = 0; i < 32; i++)
				{
					float num = Calc.Random.NextFloat(6.2831855f);
					this.level.Particles.Emit(TouchSwitch.P_FireWhite, this.Position + Calc.AngleToVector(num, 6f), num);
				}
				this.icon.Rate = 4f;
			};
			this.Switch.OnFinish = delegate()
			{
				this.ease = 0f;
			};
			this.Switch.OnStartFinished = delegate()
			{
				this.icon.Rate = 0.1f;
				this.icon.Play("idle", false, false);
				this.icon.Color = this.finishColor;
				this.ease = 1f;
			};
			base.Add(this.wiggler = Wiggler.Create(0.5f, 4f, delegate(float v)
			{
				this.pulse = Vector2.One * (1f + v * 0.25f);
			}, false, false));
			base.Add(new VertexLight(Color.White, 0.8f, 16, 32));
			base.Add(this.touchSfx = new SoundSource());
		}

		// Token: 0x06001A73 RID: 6771 RVA: 0x000AA7F1 File Offset: 0x000A89F1
		public TouchSwitch(EntityData data, Vector2 offset) : this(data.Position + offset)
		{
		}

		// Token: 0x06001A74 RID: 6772 RVA: 0x000AA808 File Offset: 0x000A8A08
		public void TurnOn()
		{
			if (!this.Switch.Activated)
			{
				this.touchSfx.Play("event:/game/general/touchswitch_any", null, 0f);
				if (this.Switch.Activate())
				{
					SoundEmitter.Play("event:/game/general/touchswitch_last_oneshot");
					base.Add(new SoundSource("event:/game/general/touchswitch_last_cutoff"));
				}
			}
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x000AA861 File Offset: 0x000A8A61
		private void OnPlayer(Player player)
		{
			this.TurnOn();
		}

		// Token: 0x06001A76 RID: 6774 RVA: 0x000AA861 File Offset: 0x000A8A61
		private void OnHoldable(Holdable h)
		{
			this.TurnOn();
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x000AA869 File Offset: 0x000A8A69
		private void OnSeeker(Seeker seeker)
		{
			if (base.SceneAs<Level>().InsideCamera(this.Position, 10f))
			{
				this.TurnOn();
			}
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x000AA88C File Offset: 0x000A8A8C
		public override void Update()
		{
			this.timer += Engine.DeltaTime * 8f;
			this.ease = Calc.Approach(this.ease, (this.Switch.Finished || this.Switch.Activated) ? 1f : 0f, Engine.DeltaTime * 2f);
			this.icon.Color = Color.Lerp(this.inactiveColor, this.Switch.Finished ? this.finishColor : this.activeColor, this.ease);
			this.icon.Color *= 0.5f + ((float)Math.Sin((double)this.timer) + 1f) / 2f * (1f - this.ease) * 0.5f + 0.5f * this.ease;
			this.bloom.Alpha = this.ease;
			if (this.Switch.Finished)
			{
				if (this.icon.Rate > 0.1f)
				{
					this.icon.Rate -= 2f * Engine.DeltaTime;
					if (this.icon.Rate <= 0.1f)
					{
						this.icon.Rate = 0.1f;
						this.wiggler.Start();
						this.icon.Play("idle", false, false);
						this.level.Displacement.AddBurst(this.Position, 0.6f, 4f, 28f, 0.2f, null, null);
					}
				}
				else if (base.Scene.OnInterval(0.03f))
				{
					Vector2 position = this.Position + new Vector2(0f, 1f) + Calc.AngleToVector(Calc.Random.NextAngle(), 5f);
					this.level.ParticlesBG.Emit(TouchSwitch.P_Fire, position);
				}
			}
			base.Update();
		}

		// Token: 0x06001A79 RID: 6777 RVA: 0x000AAAA8 File Offset: 0x000A8CA8
		public override void Render()
		{
			this.border.DrawCentered(this.Position + new Vector2(0f, -1f), Color.Black);
			this.border.DrawCentered(this.Position, this.icon.Color, this.pulse);
			base.Render();
		}

		// Token: 0x04001715 RID: 5909
		public static ParticleType P_Fire;

		// Token: 0x04001716 RID: 5910
		public static ParticleType P_FireWhite;

		// Token: 0x04001717 RID: 5911
		public Switch Switch;

		// Token: 0x04001718 RID: 5912
		private SoundSource touchSfx;

		// Token: 0x04001719 RID: 5913
		private MTexture border = GFX.Game["objects/touchswitch/container"];

		// Token: 0x0400171A RID: 5914
		private Sprite icon = new Sprite(GFX.Game, "objects/touchswitch/icon");

		// Token: 0x0400171B RID: 5915
		private Color inactiveColor = Calc.HexToColor("5fcde4");

		// Token: 0x0400171C RID: 5916
		private Color activeColor = Color.White;

		// Token: 0x0400171D RID: 5917
		private Color finishColor = Calc.HexToColor("f141df");

		// Token: 0x0400171E RID: 5918
		private float ease;

		// Token: 0x0400171F RID: 5919
		private Wiggler wiggler;

		// Token: 0x04001720 RID: 5920
		private Vector2 pulse = Vector2.One;

		// Token: 0x04001721 RID: 5921
		private float timer;

		// Token: 0x04001722 RID: 5922
		private BloomPoint bloom;
	}
}
