using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002B0 RID: 688
	public class Checkpoint : Entity
	{
		// Token: 0x06001546 RID: 5446 RVA: 0x00079D6C File Offset: 0x00077F6C
		public Checkpoint(Vector2 position, string bg = "", Vector2? spawnTarget = null) : base(position)
		{
			base.Depth = 9990;
			this.SpawnOffset = ((spawnTarget != null) ? (spawnTarget.Value - this.Position) : Vector2.Zero);
			this.bg = bg;
		}

		// Token: 0x06001547 RID: 5447 RVA: 0x00079DD0 File Offset: 0x00077FD0
		public Checkpoint(EntityData data, Vector2 offset) : this(data.Position + offset, data.Attr("bg", ""), data.FirstNodeNullable(new Vector2?(offset)))
		{
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x00079E00 File Offset: 0x00078000
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Level level = base.Scene as Level;
			int id = level.Session.Area.ID;
			string id2;
			if (string.IsNullOrWhiteSpace(this.bg))
			{
				id2 = "objects/checkpoint/bg/" + id;
			}
			else
			{
				id2 = "objects/checkpoint/bg/" + this.bg;
			}
			if (GFX.Game.Has(id2))
			{
				base.Add(this.image = new Image(GFX.Game[id2]));
				this.image.JustifyOrigin(0.5f, 1f);
			}
			base.Add(this.sprite = GFX.SpriteBank.Create("checkpoint_highlight"));
			this.sprite.Play("off", false, false);
			base.Add(this.flash = GFX.SpriteBank.Create("checkpoint_flash"));
			this.flash.Visible = false;
			this.flash.Color = Color.White * 0.6f;
			if (SaveData.Instance.HasCheckpoint(level.Session.Area, level.Session.Level))
			{
				this.TurnOn(false);
			}
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x00079F4C File Offset: 0x0007814C
		public override void Update()
		{
			if (!this.triggered)
			{
				Level level = base.Scene as Level;
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && !level.Transitioning)
				{
					if (!entity.CollideCheck<CheckpointBlockerTrigger>() && SaveData.Instance.SetCheckpoint(level.Session.Area, level.Session.Level))
					{
						level.AutoSave();
						this.TurnOn(true);
					}
					this.triggered = true;
				}
			}
			if (this.triggered && this.sprite.CurrentAnimationID == "on")
			{
				this.sine += Engine.DeltaTime * 2f;
				this.fade = Calc.Approach(this.fade, 0.5f, Engine.DeltaTime);
				float num = (float)(1.0 + Math.Sin((double)this.sine)) / 2f;
				this.sprite.Color = Color.White * (0.5f + num * 0.5f) * this.fade;
			}
			base.Update();
		}

		// Token: 0x0600154A RID: 5450 RVA: 0x0007A070 File Offset: 0x00078270
		private void TurnOn(bool animate)
		{
			this.triggered = true;
			base.Add(this.light = new VertexLight(Color.White, 0f, 16, 32));
			base.Add(this.bloom = new BloomPoint(0f, 16f));
			this.light.Position = new Vector2(0f, -8f);
			this.bloom.Position = new Vector2(0f, -8f);
			this.flash.Visible = true;
			this.flash.Play("flash", true, false);
			if (animate)
			{
				this.sprite.Play("turnOn", false, false);
				base.Add(new Coroutine(this.EaseLightsOn(), true));
				this.fade = 1f;
				return;
			}
			this.fade = 0.5f;
			this.sprite.Play("on", false, false);
			this.sprite.Color = Color.White * 0.5f;
			this.light.Alpha = 0.8f;
			this.bloom.Alpha = 0.5f;
		}

		// Token: 0x0600154B RID: 5451 RVA: 0x0007A1A0 File Offset: 0x000783A0
		private IEnumerator EaseLightsOn()
		{
			float lightStartRadius = this.light.StartRadius;
			float lightEndRadius = this.light.EndRadius;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 0.5f)
			{
				float num = Ease.BigBackOut(p);
				this.light.Alpha = 0.8f * num;
				this.light.StartRadius = (float)((int)(lightStartRadius + Calc.YoYo(p) * 8f));
				this.light.EndRadius = (float)((int)(lightEndRadius + Calc.YoYo(p) * 16f));
				this.bloom.Alpha = 0.5f * num;
				yield return null;
			}
			yield break;
		}

		// Token: 0x04001143 RID: 4419
		private const float LightAlpha = 0.8f;

		// Token: 0x04001144 RID: 4420
		private const float BloomAlpha = 0.5f;

		// Token: 0x04001145 RID: 4421
		private const float TargetFade = 0.5f;

		// Token: 0x04001146 RID: 4422
		private Image image;

		// Token: 0x04001147 RID: 4423
		private Sprite sprite;

		// Token: 0x04001148 RID: 4424
		private Sprite flash;

		// Token: 0x04001149 RID: 4425
		private VertexLight light;

		// Token: 0x0400114A RID: 4426
		private BloomPoint bloom;

		// Token: 0x0400114B RID: 4427
		private bool triggered;

		// Token: 0x0400114C RID: 4428
		private float sine = 1.5707964f;

		// Token: 0x0400114D RID: 4429
		private float fade = 1f;

		// Token: 0x0400114E RID: 4430
		private string bg;

		// Token: 0x0400114F RID: 4431
		public Vector2 SpawnOffset;
	}
}
