using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000300 RID: 768
	public class OuiTitleScreen : Oui
	{
		// Token: 0x060017FC RID: 6140 RVA: 0x00095458 File Offset: 0x00093658
		public OuiTitleScreen()
		{
			this.logo = new Image(GFX.Gui["logo"]);
			this.logo.CenterOrigin();
			this.logo.Position = new Vector2(1920f, 1080f) / 2f;
			this.title = GFX.Gui["title"];
			this.reflections = new List<MTexture>();
			for (int i = this.title.Height - 4; i > 0; i -= 4)
			{
				this.reflections.Add(this.title.GetSubtexture(0, i, this.title.Width, 4, null));
			}
			if (Celeste.PlayMode != Celeste.PlayModes.Normal)
			{
				if ("".Length > 0)
				{
					this.version += "\n";
				}
				this.version = this.version + "\n" + Celeste.PlayMode.ToString() + " Build";
			}
			if (Settings.Instance.LaunchWithFMODLiveUpdate)
			{
				this.version += "\nFMOD Live Update Enabled";
			}
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x000955A4 File Offset: 0x000937A4
		public override bool IsStart(Overworld overworld, Overworld.StartMode start)
		{
			if (start == Overworld.StartMode.Titlescreen)
			{
				overworld.ShowInputUI = false;
				overworld.Mountain.SnapCamera(-1, OuiTitleScreen.MountainTarget, false);
				this.textY = 1000f;
				this.alpha = 1f;
				this.fade = 1f;
				return true;
			}
			this.textY = 1200f;
			return false;
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x000955FC File Offset: 0x000937FC
		public override IEnumerator Enter(Oui from)
		{
			yield return null;
			base.Overworld.ShowInputUI = false;
			MountainCamera camera = base.Overworld.Mountain.Camera;
			Vector3 rotateLookAt = MountainRenderer.RotateLookAt;
			Vector3 value = (camera.Position - new Vector3(rotateLookAt.X, camera.Position.Y - 2f, rotateLookAt.Z)).SafeNormalize();
			MountainCamera transform = new MountainCamera(MountainRenderer.RotateLookAt + value * 20f, camera.Target);
			base.Add(new Coroutine(this.FadeBgTo(1f), true));
			this.hideConfirmButton = false;
			this.Visible = true;
			base.Overworld.Mountain.EaseCamera(-1, transform, new float?(2f), false, false);
			float start = this.textY;
			yield return 0.4f;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.6f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				this.alpha = t.Percent;
				this.textY = MathHelper.Lerp(start, 1000f, t.Eased);
			};
			base.Add(tween);
			yield return tween.Wait();
			base.Overworld.Mountain.SnapCamera(-1, OuiTitleScreen.MountainTarget, false);
			yield break;
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x0009560B File Offset: 0x0009380B
		public override IEnumerator Leave(Oui next)
		{
			base.Overworld.ShowInputUI = true;
			base.Overworld.Mountain.GotoRotationMode();
			float start = this.textY;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.6f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				this.alpha = 1f - t.Percent;
				this.textY = MathHelper.Lerp(start, 1200f, t.Eased);
			};
			base.Add(tween);
			yield return tween.Wait();
			yield return this.FadeBgTo(0f);
			this.Visible = false;
			yield break;
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x0009561A File Offset: 0x0009381A
		private IEnumerator FadeBgTo(float to)
		{
			while (this.fade != to)
			{
				yield return null;
				this.fade = Calc.Approach(this.fade, to, Engine.DeltaTime * 2f);
			}
			yield break;
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x00095630 File Offset: 0x00093830
		public override void Update()
		{
			int gamepad = -1;
			if (base.Selected && Input.AnyGamepadConfirmPressed(out gamepad) && !this.hideConfirmButton)
			{
				Input.Gamepad = gamepad;
				Audio.Play("event:/ui/main/title_firstinput");
				base.Overworld.Goto<OuiMainMenu>();
			}
			base.Update();
		}

		// Token: 0x06001802 RID: 6146 RVA: 0x0009567C File Offset: 0x0009387C
		public override void Render()
		{
			Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.fade);
			if (!this.hideConfirmButton)
			{
				Input.GuiButton(Input.MenuConfirm, Input.PrefixMode.Latest, "controls/keyboard/oemquestion").DrawJustified(new Vector2(1840f, this.textY), new Vector2(1f, 1f), Color.White * this.alpha, 1f);
			}
			ActiveFont.Draw(this.version, new Vector2(80f, this.textY), new Vector2(0f, 1f), Vector2.One * 0.5f, Color.DarkSlateBlue);
			if (this.alpha > 0f)
			{
				float num = MathHelper.Lerp(0.5f, 1f, Ease.SineOut(this.alpha));
				this.logo.Color = Color.White * this.alpha;
				this.logo.Scale = Vector2.One * num;
				this.logo.Render();
				float num2 = base.Scene.TimeActive * 3f;
				float num3 = 1f / (float)this.reflections.Count * 6.2831855f * 2f;
				float scaleFactor = (float)this.title.Width / this.logo.Width * num;
				for (int i = 0; i < this.reflections.Count; i++)
				{
					float num4 = (float)i / (float)this.reflections.Count;
					float x = (float)Math.Sin((double)num2) * 32f * num4;
					Vector2 position = new Vector2(1920f, 1080f) / 2f + new Vector2(x, this.logo.Height * 0.5f + (float)(i * 4)) * scaleFactor;
					float scale = Ease.CubeIn(1f - num4) * this.alpha * 0.9f;
					this.reflections[i].DrawJustified(position, new Vector2(0.5f, 0.5f), Color.White * scale, new Vector2(1f, -1f) * scaleFactor);
					num2 += num3 * ((float)Math.Sin((double)(base.Scene.TimeActive + (float)i * 6.2831855f * 0.04f)) + 1f);
				}
			}
		}

		// Token: 0x040014BA RID: 5306
		public static readonly MountainCamera MountainTarget = new MountainCamera(new Vector3(0f, 12f, 24f), MountainRenderer.RotateLookAt);

		// Token: 0x040014BB RID: 5307
		private const float TextY = 1000f;

		// Token: 0x040014BC RID: 5308
		private const float TextOutY = 1200f;

		// Token: 0x040014BD RID: 5309
		private const int ReflectionSliceSize = 4;

		// Token: 0x040014BE RID: 5310
		private float alpha;

		// Token: 0x040014BF RID: 5311
		private float fade;

		// Token: 0x040014C0 RID: 5312
		private string version = "v." + Celeste.Instance.Version;

		// Token: 0x040014C1 RID: 5313
		private bool hideConfirmButton;

		// Token: 0x040014C2 RID: 5314
		private Image logo;

		// Token: 0x040014C3 RID: 5315
		private MTexture title;

		// Token: 0x040014C4 RID: 5316
		private List<MTexture> reflections;

		// Token: 0x040014C5 RID: 5317
		private float textY;
	}
}
