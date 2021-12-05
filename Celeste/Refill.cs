using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000350 RID: 848
	public class Refill : Entity
	{
		// Token: 0x06001A93 RID: 6803 RVA: 0x000AB8EC File Offset: 0x000A9AEC
		public Refill(Vector2 position, bool twoDashes, bool oneUse) : base(position)
		{
			base.Collider = new Hitbox(16f, 16f, -8f, -8f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			this.twoDashes = twoDashes;
			this.oneUse = oneUse;
			string str;
			if (twoDashes)
			{
				str = "objects/refillTwo/";
				this.p_shatter = Refill.P_ShatterTwo;
				this.p_regen = Refill.P_RegenTwo;
				this.p_glow = Refill.P_GlowTwo;
			}
			else
			{
				str = "objects/refill/";
				this.p_shatter = Refill.P_Shatter;
				this.p_regen = Refill.P_Regen;
				this.p_glow = Refill.P_Glow;
			}
			base.Add(this.outline = new Image(GFX.Game[str + "outline"]));
			this.outline.CenterOrigin();
			this.outline.Visible = false;
			base.Add(this.sprite = new Sprite(GFX.Game, str + "idle"));
			this.sprite.AddLoop("idle", "", 0.1f);
			this.sprite.Play("idle", false, false);
			this.sprite.CenterOrigin();
			base.Add(this.flash = new Sprite(GFX.Game, str + "flash"));
			this.flash.Add("flash", "", 0.05f);
			this.flash.OnFinish = delegate(string anim)
			{
				this.flash.Visible = false;
			};
			this.flash.CenterOrigin();
			base.Add(this.wiggler = Wiggler.Create(1f, 4f, delegate(float v)
			{
				this.sprite.Scale = (this.flash.Scale = Vector2.One * (1f + v * 0.2f));
			}, false, false));
			base.Add(new MirrorReflection());
			base.Add(this.bloom = new BloomPoint(0.8f, 16f));
			base.Add(this.light = new VertexLight(Color.White, 1f, 16, 48));
			base.Add(this.sine = new SineWave(0.6f, 0f));
			this.sine.Randomize();
			this.UpdateY();
			base.Depth = -100;
		}

		// Token: 0x06001A94 RID: 6804 RVA: 0x000ABB4C File Offset: 0x000A9D4C
		public Refill(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("twoDash", false), data.Bool("oneUse", false))
		{
		}

		// Token: 0x06001A95 RID: 6805 RVA: 0x000ABB78 File Offset: 0x000A9D78
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
		}

		// Token: 0x06001A96 RID: 6806 RVA: 0x000ABB90 File Offset: 0x000A9D90
		public override void Update()
		{
			base.Update();
			if (this.respawnTimer > 0f)
			{
				this.respawnTimer -= Engine.DeltaTime;
				if (this.respawnTimer <= 0f)
				{
					this.Respawn();
				}
			}
			else if (base.Scene.OnInterval(0.1f))
			{
				this.level.ParticlesFG.Emit(this.p_glow, 1, this.Position, Vector2.One * 5f);
			}
			this.UpdateY();
			this.light.Alpha = Calc.Approach(this.light.Alpha, this.sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
			this.bloom.Alpha = this.light.Alpha * 0.8f;
			if (base.Scene.OnInterval(2f) && this.sprite.Visible)
			{
				this.flash.Play("flash", true, false);
				this.flash.Visible = true;
			}
		}

		// Token: 0x06001A97 RID: 6807 RVA: 0x000ABCB4 File Offset: 0x000A9EB4
		private void Respawn()
		{
			if (!this.Collidable)
			{
				this.Collidable = true;
				this.sprite.Visible = true;
				this.outline.Visible = false;
				base.Depth = -100;
				this.wiggler.Start();
				Audio.Play(this.twoDashes ? "event:/new_content/game/10_farewell/pinkdiamond_return" : "event:/game/general/diamond_return", this.Position);
				this.level.ParticlesFG.Emit(this.p_regen, 16, this.Position, Vector2.One * 2f);
			}
		}

		// Token: 0x06001A98 RID: 6808 RVA: 0x000ABD48 File Offset: 0x000A9F48
		private void UpdateY()
		{
			this.flash.Y = (this.sprite.Y = (this.bloom.Y = this.sine.Value * 2f));
		}

		// Token: 0x06001A99 RID: 6809 RVA: 0x000ABD8D File Offset: 0x000A9F8D
		public override void Render()
		{
			if (this.sprite.Visible)
			{
				this.sprite.DrawOutline(1);
			}
			base.Render();
		}

		// Token: 0x06001A9A RID: 6810 RVA: 0x000ABDB0 File Offset: 0x000A9FB0
		private void OnPlayer(Player player)
		{
			if (player.UseRefill(this.twoDashes))
			{
				Audio.Play(this.twoDashes ? "event:/new_content/game/10_farewell/pinkdiamond_touch" : "event:/game/general/diamond_touch", this.Position);
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
				this.Collidable = false;
				base.Add(new Coroutine(this.RefillRoutine(player), true));
				this.respawnTimer = 2.5f;
			}
		}

		// Token: 0x06001A9B RID: 6811 RVA: 0x000ABE17 File Offset: 0x000AA017
		private IEnumerator RefillRoutine(Player player)
		{
			Celeste.Freeze(0.05f);
			yield return null;
			this.level.Shake(0.3f);
			this.sprite.Visible = (this.flash.Visible = false);
			if (!this.oneUse)
			{
				this.outline.Visible = true;
			}
			base.Depth = 8999;
			yield return 0.05f;
			float num = player.Speed.Angle();
			this.level.ParticlesFG.Emit(this.p_shatter, 5, this.Position, Vector2.One * 4f, num - 1.5707964f);
			this.level.ParticlesFG.Emit(this.p_shatter, 5, this.Position, Vector2.One * 4f, num + 1.5707964f);
			SlashFx.Burst(this.Position, num);
			if (this.oneUse)
			{
				base.RemoveSelf();
			}
			yield break;
		}

		// Token: 0x04001731 RID: 5937
		public static ParticleType P_Shatter;

		// Token: 0x04001732 RID: 5938
		public static ParticleType P_Regen;

		// Token: 0x04001733 RID: 5939
		public static ParticleType P_Glow;

		// Token: 0x04001734 RID: 5940
		public static ParticleType P_ShatterTwo;

		// Token: 0x04001735 RID: 5941
		public static ParticleType P_RegenTwo;

		// Token: 0x04001736 RID: 5942
		public static ParticleType P_GlowTwo;

		// Token: 0x04001737 RID: 5943
		private Sprite sprite;

		// Token: 0x04001738 RID: 5944
		private Sprite flash;

		// Token: 0x04001739 RID: 5945
		private Image outline;

		// Token: 0x0400173A RID: 5946
		private Wiggler wiggler;

		// Token: 0x0400173B RID: 5947
		private BloomPoint bloom;

		// Token: 0x0400173C RID: 5948
		private VertexLight light;

		// Token: 0x0400173D RID: 5949
		private Level level;

		// Token: 0x0400173E RID: 5950
		private SineWave sine;

		// Token: 0x0400173F RID: 5951
		private bool twoDashes;

		// Token: 0x04001740 RID: 5952
		private bool oneUse;

		// Token: 0x04001741 RID: 5953
		private ParticleType p_shatter;

		// Token: 0x04001742 RID: 5954
		private ParticleType p_regen;

		// Token: 0x04001743 RID: 5955
		private ParticleType p_glow;

		// Token: 0x04001744 RID: 5956
		private float respawnTimer;
	}
}
