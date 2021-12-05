using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002DB RID: 731
	public class Torch : Entity
	{
		// Token: 0x060016A1 RID: 5793 RVA: 0x00086320 File Offset: 0x00084520
		public Torch(EntityID id, Vector2 position, bool startLit) : base(position)
		{
			this.id = id;
			this.startLit = startLit;
			base.Collider = new Hitbox(32f, 32f, -16f, -16f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
			base.Add(this.light = new VertexLight(Torch.Color, 1f, 48, 64));
			base.Add(this.bloom = new BloomPoint(0.5f, 8f));
			this.bloom.Visible = false;
			this.light.Visible = false;
			base.Depth = 2000;
			if (startLit)
			{
				this.light.Color = Torch.StartLitColor;
				base.Add(this.sprite = GFX.SpriteBank.Create("litTorch"));
				return;
			}
			base.Add(this.sprite = GFX.SpriteBank.Create("torch"));
		}

		// Token: 0x060016A2 RID: 5794 RVA: 0x0008642D File Offset: 0x0008462D
		public Torch(EntityData data, Vector2 offset, EntityID id) : this(id, data.Position + offset, data.Bool("startLit", false))
		{
		}

		// Token: 0x060016A3 RID: 5795 RVA: 0x00086450 File Offset: 0x00084650
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (this.startLit || base.SceneAs<Level>().Session.GetFlag(this.FlagName))
			{
				this.bloom.Visible = (this.light.Visible = true);
				this.lit = true;
				this.Collidable = false;
				this.sprite.Play("on", false, false);
			}
		}

		// Token: 0x060016A4 RID: 5796 RVA: 0x000864C0 File Offset: 0x000846C0
		private void OnPlayer(Player player)
		{
			if (!this.lit)
			{
				Audio.Play("event:/game/05_mirror_temple/torch_activate", this.Position);
				this.lit = true;
				this.bloom.Visible = true;
				this.light.Visible = true;
				this.Collidable = false;
				this.sprite.Play("turnOn", false, false);
				Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.BackOut, 1f, true);
				tween.OnUpdate = delegate(Tween t)
				{
					this.light.Color = Color.Lerp(Color.White, Torch.Color, t.Eased);
					this.light.StartRadius = 48f + (1f - t.Eased) * 32f;
					this.light.EndRadius = 64f + (1f - t.Eased) * 32f;
					this.bloom.Alpha = 0.5f + 0.5f * (1f - t.Eased);
				};
				base.Add(tween);
				base.SceneAs<Level>().Session.SetFlag(this.FlagName, true);
				base.SceneAs<Level>().ParticlesFG.Emit(Torch.P_OnLight, 12, this.Position, new Vector2(3f, 3f));
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060016A5 RID: 5797 RVA: 0x0008658F File Offset: 0x0008478F
		private string FlagName
		{
			get
			{
				return "torch_" + this.id.Key;
			}
		}

		// Token: 0x04001321 RID: 4897
		public static ParticleType P_OnLight;

		// Token: 0x04001322 RID: 4898
		public const float BloomAlpha = 0.5f;

		// Token: 0x04001323 RID: 4899
		public const int StartRadius = 48;

		// Token: 0x04001324 RID: 4900
		public const int EndRadius = 64;

		// Token: 0x04001325 RID: 4901
		public static readonly Color Color = Color.Lerp(Color.White, Color.Cyan, 0.5f);

		// Token: 0x04001326 RID: 4902
		public static readonly Color StartLitColor = Color.Lerp(Color.White, Color.Orange, 0.5f);

		// Token: 0x04001327 RID: 4903
		private EntityID id;

		// Token: 0x04001328 RID: 4904
		private bool lit;

		// Token: 0x04001329 RID: 4905
		private VertexLight light;

		// Token: 0x0400132A RID: 4906
		private BloomPoint bloom;

		// Token: 0x0400132B RID: 4907
		private bool startLit;

		// Token: 0x0400132C RID: 4908
		private Sprite sprite;
	}
}
