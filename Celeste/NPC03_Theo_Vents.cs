using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000271 RID: 625
	public class NPC03_Theo_Vents : NPC
	{
		// Token: 0x06001368 RID: 4968 RVA: 0x00069A5C File Offset: 0x00067C5C
		public NPC03_Theo_Vents(Vector2 position) : base(position)
		{
			base.Tag = Tags.TransitionUpdate;
			base.Add(this.Sprite = GFX.SpriteBank.Create("theo"));
			this.Sprite.Scale.Y = -1f;
			this.Sprite.Scale.X = -1f;
			this.Visible = false;
			this.Maxspeed = 48f;
		}

		// Token: 0x06001369 RID: 4969 RVA: 0x00069ADA File Offset: 0x00067CDA
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (base.Session.GetFlag("theoVentsTalked"))
			{
				base.RemoveSelf();
				return;
			}
			base.Add(new Coroutine(this.Appear(), true));
		}

		// Token: 0x0600136A RID: 4970 RVA: 0x00069B10 File Offset: 0x00067D10
		public override void Update()
		{
			base.Update();
			if (!this.appeared)
			{
				this.particleDelay -= Engine.DeltaTime;
				if (this.particleDelay <= 0f)
				{
					this.Level.ParticlesFG.Emit(ParticleTypes.VentDust, 8, this.Position, new Vector2(6f, 0f));
					if (this.grate != null)
					{
						this.grate.Shake();
					}
					this.particleDelay = Calc.Random.Choose(1f, 2f, 3f);
				}
			}
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x00069BA7 File Offset: 0x00067DA7
		private IEnumerator Appear()
		{
			if (!base.Session.GetFlag("theoVentsAppeared"))
			{
				this.grate = new NPC03_Theo_Vents.Grate(this.Position);
				base.Scene.Add(this.grate);
				Player entity;
				do
				{
					yield return null;
					entity = base.Scene.Tracker.GetEntity<Player>();
				}
				while (entity == null || entity.X <= base.X - 32f);
				Audio.Play("event:/char/theo/resort_ceilingvent_hey", this.Position);
				this.Level.ParticlesFG.Emit(ParticleTypes.VentDust, 24, this.Position, new Vector2(6f, 0f));
				this.grate.Fall();
				int from = -24;
				for (float p = 0f; p < 1f; p += Engine.DeltaTime * 2f)
				{
					yield return null;
					this.Visible = true;
					this.Sprite.Y = (float)from + (float)(-8 - from) * Ease.CubeOut(p);
				}
				base.Session.SetFlag("theoVentsAppeared", true);
			}
			this.appeared = true;
			this.Sprite.Y = -8f;
			this.Visible = true;
			base.Add(this.Talker = new TalkComponent(new Rectangle(-16, 0, 32, 100), new Vector2(0f, -8f), new Action<Player>(this.OnTalk), null));
			yield break;
		}

		// Token: 0x0600136C RID: 4972 RVA: 0x00069BB6 File Offset: 0x00067DB6
		private void OnTalk(Player player)
		{
			this.Level.StartCutscene(new Action<Level>(this.OnTalkEnd), true, false, true);
			base.Add(new Coroutine(this.Talk(player), true));
		}

		// Token: 0x0600136D RID: 4973 RVA: 0x00069BE5 File Offset: 0x00067DE5
		private IEnumerator Talk(Player player)
		{
			yield return base.PlayerApproach(player, true, new float?((float)10), new int?(-1));
			player.DummyAutoAnimate = false;
			player.Sprite.Play("lookUp", false, false);
			yield return CutsceneEntity.CameraTo(new Vector2((float)(this.Level.Bounds.Right - 320), (float)this.Level.Bounds.Top), 0.5f, null, 0f);
			yield return this.Level.ZoomTo(new Vector2(240f, 70f), 2f, 0.5f);
			yield return Textbox.Say("CH3_THEO_VENTS", new Func<IEnumerator>[0]);
			yield return this.Disappear();
			yield return 0.25f;
			yield return this.Level.ZoomBack(0.5f);
			this.Level.EndCutscene();
			this.OnTalkEnd(this.Level);
			yield break;
		}

		// Token: 0x0600136E RID: 4974 RVA: 0x00069BFC File Offset: 0x00067DFC
		private void OnTalkEnd(Level level)
		{
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				entity.DummyAutoAnimate = true;
				entity.StateMachine.Locked = false;
				entity.StateMachine.State = 0;
			}
			base.Session.SetFlag("theoVentsTalked", true);
			base.RemoveSelf();
		}

		// Token: 0x0600136F RID: 4975 RVA: 0x00069C53 File Offset: 0x00067E53
		private IEnumerator Disappear()
		{
			Audio.Play("event:/char/theo/resort_ceilingvent_seeya", this.Position);
			int to = -24;
			float from = this.Sprite.Y;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 2f)
			{
				yield return null;
				this.Level.ParticlesFG.Emit(ParticleTypes.VentDust, 1, this.Position, new Vector2(6f, 0f));
				this.Sprite.Y = from + ((float)to - from) * Ease.BackIn(p);
			}
			yield break;
		}

		// Token: 0x04000F47 RID: 3911
		private const string AppeardFlag = "theoVentsAppeared";

		// Token: 0x04000F48 RID: 3912
		private const string TalkedFlag = "theoVentsTalked";

		// Token: 0x04000F49 RID: 3913
		private const int SpriteAppearY = -8;

		// Token: 0x04000F4A RID: 3914
		private float particleDelay;

		// Token: 0x04000F4B RID: 3915
		private bool appeared;

		// Token: 0x04000F4C RID: 3916
		private NPC03_Theo_Vents.Grate grate;

		// Token: 0x020005AD RID: 1453
		private class Grate : Entity
		{
			// Token: 0x060027F1 RID: 10225 RVA: 0x00104994 File Offset: 0x00102B94
			public Grate(Vector2 position) : base(position)
			{
				base.Add(this.sprite = new Image(GFX.Game["scenery/grate"]));
				this.sprite.JustifyOrigin(0.5f, 0f);
			}

			// Token: 0x060027F2 RID: 10226 RVA: 0x001049EC File Offset: 0x00102BEC
			public void Shake()
			{
				if (!this.falling)
				{
					Audio.Play("event:/char/theo/resort_ceilingvent_shake", this.Position);
					this.shake = 0.5f;
				}
			}

			// Token: 0x060027F3 RID: 10227 RVA: 0x00104A14 File Offset: 0x00102C14
			public void Fall()
			{
				Audio.Play("event:/char/theo/resort_ceilingvent_popoff", this.Position);
				this.falling = true;
				this.speed = new Vector2(40f, 200f);
				base.Collider = new Hitbox(2f, 2f, -1f, 0f);
			}

			// Token: 0x060027F4 RID: 10228 RVA: 0x00104A70 File Offset: 0x00102C70
			public override void Update()
			{
				if (this.shake > 0f)
				{
					this.shake -= Engine.DeltaTime;
					if (base.Scene.OnInterval(0.05f))
					{
						this.sprite.X = 1f - this.sprite.X;
					}
				}
				if (this.falling)
				{
					this.speed.X = Calc.Approach(this.speed.X, 0f, Engine.DeltaTime * 80f);
					this.speed.Y = this.speed.Y + 200f * Engine.DeltaTime;
					this.Position += this.speed * Engine.DeltaTime;
					if (base.CollideCheck<Solid>(this.Position + new Vector2(0f, 2f)) && this.speed.Y > 0f)
					{
						this.speed.Y = -this.speed.Y * 0.25f;
					}
					this.alpha -= Engine.DeltaTime;
					this.sprite.Rotation += Engine.DeltaTime;
					this.sprite.Color = Color.White * this.alpha;
					if (this.alpha <= 0f)
					{
						base.RemoveSelf();
					}
				}
				base.Update();
			}

			// Token: 0x04002790 RID: 10128
			private Image sprite;

			// Token: 0x04002791 RID: 10129
			private float shake;

			// Token: 0x04002792 RID: 10130
			private Vector2 speed;

			// Token: 0x04002793 RID: 10131
			private bool falling;

			// Token: 0x04002794 RID: 10132
			private float alpha = 1f;
		}
	}
}
