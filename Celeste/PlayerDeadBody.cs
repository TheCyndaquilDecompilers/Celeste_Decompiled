using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002A5 RID: 677
	public class PlayerDeadBody : Entity
	{
		// Token: 0x060014FB RID: 5371 RVA: 0x000767D8 File Offset: 0x000749D8
		public PlayerDeadBody(Player player, Vector2 direction)
		{
			base.Depth = -1000000;
			this.player = player;
			this.facing = player.Facing;
			this.Position = player.Position;
			base.Add(this.hair = player.Hair);
			base.Add(this.sprite = player.Sprite);
			base.Add(this.light = player.Light);
			this.sprite.Color = Color.White;
			this.initialHairColor = this.hair.Color;
			this.bounce = direction;
			base.Add(new Coroutine(this.DeathRoutine(), true));
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x000768A4 File Offset: 0x00074AA4
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (this.bounce != Vector2.Zero)
			{
				if (Math.Abs(this.bounce.X) > Math.Abs(this.bounce.Y))
				{
					this.sprite.Play("deadside", false, false);
					this.facing = (Facings)(-(Facings)Math.Sign(this.bounce.X));
					return;
				}
				this.bounce = Calc.AngleToVector(Calc.AngleApproach(this.bounce.Angle(), new Vector2((float)(-(float)this.player.Facing), 0f).Angle(), 0.5f), 1f);
				if (this.bounce.Y < 0f)
				{
					this.sprite.Play("deadup", false, false);
					return;
				}
				this.sprite.Play("deaddown", false, false);
			}
		}

		// Token: 0x060014FD RID: 5373 RVA: 0x00076991 File Offset: 0x00074B91
		private IEnumerator DeathRoutine()
		{
			Level level = base.SceneAs<Level>();
			if (this.bounce != Vector2.Zero)
			{
				PlayerDeadBody.<>c__DisplayClass15_0 CS$<>8__locals1 = new PlayerDeadBody.<>c__DisplayClass15_0();
				CS$<>8__locals1.<>4__this = this;
				Audio.Play("event:/char/madeline/predeath", this.Position);
				this.scale = 1.5f;
				Celeste.Freeze(0.05f);
				yield return null;
				CS$<>8__locals1.from = this.Position;
				CS$<>8__locals1.to = CS$<>8__locals1.from + this.bounce * 24f;
				Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.5f, true);
				base.Add(tween);
				tween.OnUpdate = delegate(Tween t)
				{
					CS$<>8__locals1.<>4__this.Position = CS$<>8__locals1.from + (CS$<>8__locals1.to - CS$<>8__locals1.from) * t.Eased;
					CS$<>8__locals1.<>4__this.scale = 1.5f - t.Eased * 0.5f;
					CS$<>8__locals1.<>4__this.sprite.Rotation = (float)(Math.Floor((double)(t.Eased * 4f)) * 6.2831854820251465);
				};
				yield return tween.Duration * 0.75f;
				tween.Stop();
				CS$<>8__locals1 = null;
				tween = null;
			}
			this.Position += Vector2.UnitY * -5f;
			level.Displacement.AddBurst(this.Position, 0.3f, 0f, 80f, 1f, null, null);
			level.Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
			Audio.Play(this.HasGolden ? "event:/new_content/char/madeline/death_golden" : "event:/char/madeline/death", this.Position);
			this.deathEffect = new DeathEffect(this.initialHairColor, new Vector2?(base.Center - this.Position));
			this.deathEffect.OnUpdate = delegate(float f)
			{
				this.light.Alpha = 1f - f;
			};
			base.Add(this.deathEffect);
			yield return this.deathEffect.Duration * 0.65f;
			if (this.ActionDelay > 0f)
			{
				yield return this.ActionDelay;
			}
			this.End();
			yield break;
		}

		// Token: 0x060014FE RID: 5374 RVA: 0x000769A0 File Offset: 0x00074BA0
		private void End()
		{
			if (!this.finished)
			{
				this.finished = true;
				Level level = base.SceneAs<Level>();
				if (this.DeathAction == null)
				{
					this.DeathAction = new Action(level.Reload);
				}
				level.DoScreenWipe(false, this.DeathAction, false);
			}
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x000769EC File Offset: 0x00074BEC
		public override void Update()
		{
			base.Update();
			if (Input.MenuConfirm.Pressed && !this.finished)
			{
				this.End();
			}
			this.hair.Color = ((this.sprite.CurrentAnimationFrame == 0) ? Color.White : this.initialHairColor);
		}

		// Token: 0x06001500 RID: 5376 RVA: 0x00076A40 File Offset: 0x00074C40
		public override void Render()
		{
			if (this.deathEffect == null)
			{
				this.sprite.Scale.X = (float)this.facing * this.scale;
				this.sprite.Scale.Y = this.scale;
				this.hair.Facing = this.facing;
				base.Render();
				return;
			}
			this.deathEffect.Render();
		}

		// Token: 0x040010D2 RID: 4306
		public Action DeathAction;

		// Token: 0x040010D3 RID: 4307
		public float ActionDelay;

		// Token: 0x040010D4 RID: 4308
		public bool HasGolden;

		// Token: 0x040010D5 RID: 4309
		private Color initialHairColor;

		// Token: 0x040010D6 RID: 4310
		private Vector2 bounce = Vector2.Zero;

		// Token: 0x040010D7 RID: 4311
		private Player player;

		// Token: 0x040010D8 RID: 4312
		private PlayerHair hair;

		// Token: 0x040010D9 RID: 4313
		private PlayerSprite sprite;

		// Token: 0x040010DA RID: 4314
		private VertexLight light;

		// Token: 0x040010DB RID: 4315
		private DeathEffect deathEffect;

		// Token: 0x040010DC RID: 4316
		private Facings facing;

		// Token: 0x040010DD RID: 4317
		private float scale = 1f;

		// Token: 0x040010DE RID: 4318
		private bool finished;
	}
}
