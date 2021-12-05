using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000152 RID: 338
	public class SummitGem : Entity
	{
		// Token: 0x06000C48 RID: 3144 RVA: 0x00028378 File Offset: 0x00026578
		public SummitGem(EntityData data, Vector2 position, EntityID gid) : base(data.Position + position)
		{
			this.GID = gid;
			this.GemID = data.Int("gem", 0);
			base.Collider = new Hitbox(12f, 12f, -6f, -6f);
			base.Add(this.sprite = new Sprite(GFX.Game, "collectables/summitgems/" + this.GemID + "/gem"));
			this.sprite.AddLoop("idle", "", 0.08f);
			this.sprite.Play("idle", false, false);
			this.sprite.CenterOrigin();
			if (SaveData.Instance.SummitGems != null && SaveData.Instance.SummitGems[this.GemID])
			{
				this.sprite.Color = Color.White * 0.5f;
			}
			base.Add(this.scaleWiggler = Wiggler.Create(0.5f, 4f, delegate(float f)
			{
				this.sprite.Scale = Vector2.One * (1f + f * 0.3f);
			}, false, false));
			this.moveWiggler = Wiggler.Create(0.8f, 2f, null, false, false);
			this.moveWiggler.StartZero = true;
			base.Add(this.moveWiggler);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
		}

		// Token: 0x06000C49 RID: 3145 RVA: 0x000284E8 File Offset: 0x000266E8
		private void OnPlayer(Player player)
		{
			Level level = base.Scene as Level;
			if (player.DashAttacking)
			{
				base.Add(new Coroutine(this.SmashRoutine(player, level), true));
				return;
			}
			player.PointBounce(base.Center);
			this.moveWiggler.Start();
			this.scaleWiggler.Start();
			this.moveWiggleDir = (base.Center - player.Center).SafeNormalize(Vector2.UnitY);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			if (this.bounceSfxDelay <= 0f)
			{
				Audio.Play("event:/game/general/crystalheart_bounce", this.Position);
				this.bounceSfxDelay = 0.1f;
			}
		}

		// Token: 0x06000C4A RID: 3146 RVA: 0x00028591 File Offset: 0x00026791
		private IEnumerator SmashRoutine(Player player, Level level)
		{
			this.Visible = false;
			this.Collidable = false;
			player.Stamina = 110f;
			SoundEmitter.Play("event:/game/07_summit/gem_get", this, null);
			Session session = (base.Scene as Level).Session;
			session.DoNotLoad.Add(this.GID);
			session.SummitGems[this.GemID] = true;
			SaveData.Instance.RegisterSummitGem(this.GemID);
			level.Shake(0.3f);
			Celeste.Freeze(0.1f);
			SummitGem.P_Shatter.Color = SummitGem.GemColors[this.GemID];
			float num = player.Speed.Angle();
			level.ParticlesFG.Emit(SummitGem.P_Shatter, 5, this.Position, Vector2.One * 4f, num - 1.5707964f);
			level.ParticlesFG.Emit(SummitGem.P_Shatter, 5, this.Position, Vector2.One * 4f, num + 1.5707964f);
			SlashFx.Burst(this.Position, num);
			for (int i = 0; i < 10; i++)
			{
				base.Scene.Add(new AbsorbOrb(this.Position, player, null));
			}
			level.Flash(Color.White, true);
			base.Scene.Add(new SummitGem.BgFlash());
			Engine.TimeRate = 0.5f;
			while (Engine.TimeRate < 1f)
			{
				Engine.TimeRate += Engine.RawDeltaTime * 0.5f;
				yield return null;
			}
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x06000C4B RID: 3147 RVA: 0x000285B0 File Offset: 0x000267B0
		public override void Update()
		{
			base.Update();
			this.bounceSfxDelay -= Engine.DeltaTime;
			this.sprite.Position = this.moveWiggleDir * this.moveWiggler.Value * -8f;
		}

		// Token: 0x040007AB RID: 1963
		public static ParticleType P_Shatter;

		// Token: 0x040007AC RID: 1964
		public static readonly Color[] GemColors = new Color[]
		{
			Calc.HexToColor("9ee9ff"),
			Calc.HexToColor("54baff"),
			Calc.HexToColor("90ff2d"),
			Calc.HexToColor("ffd300"),
			Calc.HexToColor("ff609d"),
			Calc.HexToColor("c5e1ba")
		};

		// Token: 0x040007AD RID: 1965
		public int GemID;

		// Token: 0x040007AE RID: 1966
		public EntityID GID;

		// Token: 0x040007AF RID: 1967
		private Sprite sprite;

		// Token: 0x040007B0 RID: 1968
		private Wiggler scaleWiggler;

		// Token: 0x040007B1 RID: 1969
		private Vector2 moveWiggleDir;

		// Token: 0x040007B2 RID: 1970
		private Wiggler moveWiggler;

		// Token: 0x040007B3 RID: 1971
		private float bounceSfxDelay;

		// Token: 0x020003DF RID: 991
		private class BgFlash : Entity
		{
			// Token: 0x06001F50 RID: 8016 RVA: 0x000D8E48 File Offset: 0x000D7048
			public BgFlash()
			{
				base.Depth = 10100;
				base.Tag = Tags.Persistent;
			}

			// Token: 0x06001F51 RID: 8017 RVA: 0x000D8E76 File Offset: 0x000D7076
			public override void Update()
			{
				base.Update();
				this.alpha = Calc.Approach(this.alpha, 0f, Engine.DeltaTime * 0.5f);
				if (this.alpha <= 0f)
				{
					base.RemoveSelf();
				}
			}

			// Token: 0x06001F52 RID: 8018 RVA: 0x000D8EB4 File Offset: 0x000D70B4
			public override void Render()
			{
				Vector2 position = (base.Scene as Level).Camera.Position;
				Draw.Rect(position.X - 10f, position.Y - 10f, 340f, 200f, Color.Black * this.alpha);
			}

			// Token: 0x04001FE5 RID: 8165
			private float alpha = 1f;
		}
	}
}
