using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000236 RID: 566
	public class SaveLoadIcon : Entity
	{
		// Token: 0x17000141 RID: 321
		// (get) Token: 0x060011FE RID: 4606 RVA: 0x0005AEC5 File Offset: 0x000590C5
		public static bool OnScreen
		{
			get
			{
				return SaveLoadIcon.Instance != null;
			}
		}

		// Token: 0x060011FF RID: 4607 RVA: 0x0005AECF File Offset: 0x000590CF
		public static void Show(Scene scene)
		{
			if (SaveLoadIcon.Instance != null)
			{
				SaveLoadIcon.Instance.RemoveSelf();
			}
			scene.Add(SaveLoadIcon.Instance = new SaveLoadIcon());
		}

		// Token: 0x06001200 RID: 4608 RVA: 0x0005AEF3 File Offset: 0x000590F3
		public static void Hide()
		{
			if (SaveLoadIcon.Instance != null)
			{
				SaveLoadIcon.Instance.display = false;
			}
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x0005AF08 File Offset: 0x00059108
		public SaveLoadIcon()
		{
			base.Tag = (Tags.HUD | Tags.FrozenUpdate | Tags.PauseUpdate | Tags.Global);
			base.Depth = -1000000;
			base.Add(this.icon = GFX.GuiSpriteBank.Create("save"));
			this.icon.UseRawDeltaTime = true;
			base.Add(this.wiggler = Wiggler.Create(0.4f, 4f, delegate(float f)
			{
				this.icon.Rotation = f * 0.1f;
			}, false, false));
			this.wiggler.UseRawDeltaTime = true;
			base.Add(new Coroutine(this.Routine(), true)
			{
				UseRawDeltaTime = true
			});
			this.icon.Visible = false;
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x0005AFE7 File Offset: 0x000591E7
		private IEnumerator Routine()
		{
			this.icon.Play("start", true, false);
			this.icon.Visible = true;
			yield return 0.25f;
			float timer = 1f;
			while (this.display)
			{
				timer -= Engine.DeltaTime;
				if (timer <= 0f)
				{
					this.wiggler.Start();
					timer = 1f;
				}
				yield return null;
			}
			this.icon.Play("end", false, false);
			yield return 0.5f;
			this.icon.Visible = false;
			yield return null;
			SaveLoadIcon.Instance = null;
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x0005AFF6 File Offset: 0x000591F6
		public override void Render()
		{
			this.Position = new Vector2(1760f, 920f);
			base.Render();
		}

		// Token: 0x04000DB2 RID: 3506
		public static SaveLoadIcon Instance;

		// Token: 0x04000DB3 RID: 3507
		private bool display = true;

		// Token: 0x04000DB4 RID: 3508
		private Sprite icon;

		// Token: 0x04000DB5 RID: 3509
		private Wiggler wiggler;
	}
}
