using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002E0 RID: 736
	public class Selfie : Entity
	{
		// Token: 0x060016B2 RID: 5810 RVA: 0x00086859 File Offset: 0x00084A59
		public Selfie(Level level)
		{
			base.Tag = Tags.HUD;
			this.level = level;
		}

		// Token: 0x060016B3 RID: 5811 RVA: 0x00086878 File Offset: 0x00084A78
		public IEnumerator PictureRoutine(string photo = "selfie")
		{
			this.level.Flash(Color.White, false);
			yield return 0.5f;
			yield return this.OpenRoutine(photo);
			yield return this.WaitForInput();
			yield return this.EndRoutine();
			yield break;
		}

		// Token: 0x060016B4 RID: 5812 RVA: 0x0008688E File Offset: 0x00084A8E
		public IEnumerator FilterRoutine()
		{
			yield return this.OpenRoutine("selfie");
			yield return 0.5f;
			MTexture tex = GFX.Portraits["selfieFilter"];
			this.overImage = new Image(tex);
			this.overImage.Visible = false;
			this.overImage.CenterOrigin();
			int atWidth = 0;
			this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 0.4f, true);
			this.tween.OnUpdate = delegate(Tween t)
			{
				int num = (int)Math.Round((double)MathHelper.Lerp(0f, (float)tex.Width, t.Eased));
				if (num != atWidth)
				{
					atWidth = num;
					this.overImage.Texture = tex.GetSubtexture(tex.Width - atWidth, 0, atWidth, tex.Height, null);
					this.overImage.Visible = true;
					this.overImage.Origin.X = (float)(atWidth - tex.Width / 2);
				}
			};
			Audio.Play("event:/game/02_old_site/theoselfie_photo_filter");
			yield return this.tween.Wait();
			yield return this.WaitForInput();
			yield return this.EndRoutine();
			yield break;
		}

		// Token: 0x060016B5 RID: 5813 RVA: 0x0008689D File Offset: 0x00084A9D
		public IEnumerator OpenRoutine(string selfie = "selfie")
		{
			Audio.Play("event:/game/02_old_site/theoselfie_photo_in");
			this.image = new Image(GFX.Portraits[selfie]);
			this.image.CenterOrigin();
			float percent = 0f;
			while (percent < 1f)
			{
				percent += Engine.DeltaTime;
				this.image.Position = Vector2.Lerp(new Vector2(992f, 1080f + this.image.Height / 2f), new Vector2(960f, 540f), Ease.CubeOut(percent));
				this.image.Rotation = MathHelper.Lerp(0.5f, 0f, Ease.BackOut(percent));
				yield return null;
			}
			yield break;
		}

		// Token: 0x060016B6 RID: 5814 RVA: 0x000868B3 File Offset: 0x00084AB3
		public IEnumerator WaitForInput()
		{
			this.waitForKeyPress = true;
			while (!Input.MenuCancel.Pressed && !Input.MenuConfirm.Pressed)
			{
				yield return null;
			}
			Audio.Play("event:/ui/main/button_lowkey");
			this.waitForKeyPress = false;
			yield break;
		}

		// Token: 0x060016B7 RID: 5815 RVA: 0x000868C2 File Offset: 0x00084AC2
		public IEnumerator EndRoutine()
		{
			Audio.Play("event:/game/02_old_site/theoselfie_photo_out");
			float percent = 0f;
			while (percent < 1f)
			{
				percent += Engine.DeltaTime * 2f;
				this.image.Position = Vector2.Lerp(new Vector2(960f, 540f), new Vector2(928f, -this.image.Height / 2f), Ease.BackIn(percent));
				this.image.Rotation = MathHelper.Lerp(0f, -0.15f, Ease.BackIn(percent));
				yield return null;
			}
			yield return null;
			this.level.Remove(this);
			yield break;
		}

		// Token: 0x060016B8 RID: 5816 RVA: 0x000868D1 File Offset: 0x00084AD1
		public override void Update()
		{
			if (this.tween != null && this.tween.Active)
			{
				this.tween.Update();
			}
			if (this.waitForKeyPress)
			{
				this.timer += Engine.DeltaTime;
			}
		}

		// Token: 0x060016B9 RID: 5817 RVA: 0x00086910 File Offset: 0x00084B10
		public override void Render()
		{
			Level level = base.Scene as Level;
			if (level != null && (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene))
			{
				return;
			}
			if (this.image != null && this.image.Visible)
			{
				this.image.Render();
				if (this.overImage != null && this.overImage.Visible)
				{
					this.overImage.Position = this.image.Position;
					this.overImage.Rotation = this.image.Rotation;
					this.overImage.Scale = this.image.Scale;
					this.overImage.Render();
				}
			}
			if (this.waitForKeyPress)
			{
				GFX.Gui["textboxbutton"].DrawCentered(this.image.Position + new Vector2(this.image.Width / 2f + 40f, this.image.Height / 2f + (float)((this.timer % 1f < 0.25f) ? 6 : 0)));
			}
		}

		// Token: 0x04001335 RID: 4917
		private Level level;

		// Token: 0x04001336 RID: 4918
		private Image image;

		// Token: 0x04001337 RID: 4919
		private Image overImage;

		// Token: 0x04001338 RID: 4920
		private bool waitForKeyPress;

		// Token: 0x04001339 RID: 4921
		private float timer;

		// Token: 0x0400133A RID: 4922
		private Tween tween;
	}
}
