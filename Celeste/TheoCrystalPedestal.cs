using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000208 RID: 520
	public class TheoCrystalPedestal : Solid
	{
		// Token: 0x060010FD RID: 4349 RVA: 0x000514A4 File Offset: 0x0004F6A4
		public TheoCrystalPedestal(EntityData data, Vector2 offset) : base(data.Position + offset, 32f, 32f, false)
		{
			base.Add(this.sprite = new Image(GFX.Game["characters/theoCrystal/pedestal"]));
			this.EnableAssistModeChecks = false;
			this.sprite.JustifyOrigin(0.5f, 1f);
			base.Depth = 8998;
			base.Collider.Position = new Vector2(-16f, -64f);
			this.Collidable = false;
			this.OnDashCollide = delegate(Player player, Vector2 direction)
			{
				TheoCrystal entity = base.Scene.Tracker.GetEntity<TheoCrystal>();
				entity.OnPedestal = false;
				entity.Speed = new Vector2(0f, -300f);
				this.DroppedTheo = true;
				this.Collidable = false;
				(base.Scene as Level).Flash(Color.White, false);
				Celeste.Freeze(0.1f);
				Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
				Audio.Play("event:/game/05_mirror_temple/crystaltheo_break_free", entity.Position);
				return DashCollisionResults.Rebound;
			};
			base.Tag = Tags.TransitionUpdate;
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x0005155C File Offset: 0x0004F75C
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if ((scene as Level).Session.GetFlag("foundTheoInCrystal"))
			{
				this.DroppedTheo = true;
				return;
			}
			TheoCrystal theoCrystal = base.Scene.Entities.FindFirst<TheoCrystal>();
			if (theoCrystal != null)
			{
				theoCrystal.Depth = base.Depth + 1;
			}
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x000515B4 File Offset: 0x0004F7B4
		public override void Update()
		{
			TheoCrystal entity = base.Scene.Tracker.GetEntity<TheoCrystal>();
			if (entity != null && !this.DroppedTheo)
			{
				entity.Position = this.Position + new Vector2(0f, -32f);
				entity.OnPedestal = true;
			}
			base.Update();
		}

		// Token: 0x04000C9E RID: 3230
		public Image sprite;

		// Token: 0x04000C9F RID: 3231
		public bool DroppedTheo;
	}
}
