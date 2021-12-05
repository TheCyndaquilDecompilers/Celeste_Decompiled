using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000298 RID: 664
	public class OuiChapterSelectIcon : Entity
	{
		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06001498 RID: 5272 RVA: 0x00071E90 File Offset: 0x00070090
		public Vector2 IdlePosition
		{
			get
			{
				float num = 960f + (float)(this.Area - SaveData.Instance.LastArea.ID) * 132f;
				if (this.Area < SaveData.Instance.LastArea.ID)
				{
					num -= 80f;
				}
				else if (this.Area > SaveData.Instance.LastArea.ID)
				{
					num += 80f;
				}
				float y = 130f;
				if (this.Area == SaveData.Instance.LastArea.ID)
				{
					y = 140f;
				}
				return new Vector2(num, y);
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06001499 RID: 5273 RVA: 0x00071F2B File Offset: 0x0007012B
		public Vector2 HiddenPosition
		{
			get
			{
				return new Vector2(this.IdlePosition.X, -100f);
			}
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x00071F44 File Offset: 0x00070144
		public OuiChapterSelectIcon(int area, MTexture front, MTexture back)
		{
			base.Tag = (Tags.HUD | Tags.PauseUpdate);
			this.Position = new Vector2(0f, -100f);
			this.Area = area;
			this.front = front;
			this.back = back;
			base.Add(this.wiggler = Wiggler.Create(0.35f, 2f, delegate(float f)
			{
				this.Rotation = (this.wiggleLeft ? (-f) : f) * 0.4f;
				this.Scale = Vector2.One * (1f + f * 0.5f);
			}, false, false));
			base.Add(this.newWiggle = Wiggler.Create(0.8f, 2f, null, false, false));
			this.newWiggle.StartZero = true;
		}

		// Token: 0x0600149B RID: 5275 RVA: 0x00072019 File Offset: 0x00070219
		public void Hovered(int dir)
		{
			this.wiggleLeft = (dir < 0);
			this.wiggler.Start();
		}

		// Token: 0x0600149C RID: 5276 RVA: 0x00072030 File Offset: 0x00070230
		public void Select()
		{
			Audio.Play("event:/ui/world_map/icon/flip_right");
			this.selected = true;
			this.hidden = false;
			Vector2 from = this.Position;
			this.StartTween(0.6f, delegate(Tween t)
			{
				this.SetSelectedPercent(from, t.Percent);
			});
		}

		// Token: 0x0600149D RID: 5277 RVA: 0x00072086 File Offset: 0x00070286
		public void SnapToSelected()
		{
			this.selected = true;
			this.hidden = false;
			this.StopTween();
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x0007209C File Offset: 0x0007029C
		public void Unselect()
		{
			Audio.Play("event:/ui/world_map/icon/flip_left");
			this.hidden = false;
			this.selected = false;
			Vector2 to = this.IdlePosition;
			this.StartTween(0.6f, delegate(Tween t)
			{
				this.SetSelectedPercent(to, 1f - t.Percent);
			});
		}

		// Token: 0x0600149F RID: 5279 RVA: 0x000720F4 File Offset: 0x000702F4
		public void Hide()
		{
			this.Scale = Vector2.One;
			this.hidden = true;
			this.selected = false;
			Vector2 from = this.Position;
			this.StartTween(0.25f, delegate(Tween t)
			{
				this.Position = Vector2.Lerp(from, this.HiddenPosition, this.tween.Eased);
			});
		}

		// Token: 0x060014A0 RID: 5280 RVA: 0x0007214C File Offset: 0x0007034C
		public void Show()
		{
			if (SaveData.Instance != null)
			{
				this.New = (SaveData.Instance.Areas[this.Area].Modes[0].TimePlayed <= 0L);
			}
			this.Scale = Vector2.One;
			this.hidden = false;
			this.selected = false;
			Vector2 from = this.Position;
			this.StartTween(0.25f, delegate(Tween t)
			{
				this.Position = Vector2.Lerp(from, this.IdlePosition, this.tween.Eased);
			});
		}

		// Token: 0x060014A1 RID: 5281 RVA: 0x000721D7 File Offset: 0x000703D7
		public void AssistModeUnlock(Action onComplete)
		{
			base.Add(new Coroutine(this.AssistModeUnlockRoutine(onComplete), true));
		}

		// Token: 0x060014A2 RID: 5282 RVA: 0x000721EC File Offset: 0x000703EC
		private IEnumerator AssistModeUnlockRoutine(Action onComplete)
		{
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
			{
				this.spotlightRadius = Ease.CubeOut(p) * 128f;
				this.spotlightAlpha = Ease.CubeOut(p) * 0.8f;
				yield return null;
			}
			this.shake.X = 6f;
			int num;
			for (int i = 0; i < 10; i = num + 1)
			{
				this.shake.X = -this.shake.X;
				yield return 0.01f;
				num = i;
			}
			this.shake = Vector2.Zero;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
			{
				float num2 = Ease.CubeIn(p);
				this.shake = new Vector2(0f, -160f * num2);
				this.Scale = new Vector2(1f - p, 1f + p * 0.25f);
				yield return null;
			}
			this.shake = Vector2.Zero;
			this.Scale = Vector2.One;
			this.AssistModeUnlockable = false;
			SaveData.Instance.UnlockedAreas++;
			this.wiggler.Start();
			yield return 1f;
			for (float p = 1f; p > 0f; p -= Engine.DeltaTime * 4f)
			{
				this.spotlightRadius = 128f + (1f - Ease.CubeOut(p)) * 128f;
				this.spotlightAlpha = Ease.CubeOut(p) * 0.8f;
				yield return null;
			}
			this.spotlightAlpha = 0f;
			if (onComplete != null)
			{
				onComplete();
			}
			yield break;
		}

		// Token: 0x060014A3 RID: 5283 RVA: 0x00072202 File Offset: 0x00070402
		public void HighlightUnlock(Action onComplete)
		{
			this.HideIcon = true;
			base.Add(new Coroutine(this.HighlightUnlockRoutine(onComplete), true));
		}

		// Token: 0x060014A4 RID: 5284 RVA: 0x0007221E File Offset: 0x0007041E
		private IEnumerator HighlightUnlockRoutine(Action onComplete)
		{
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 2f)
			{
				this.spotlightRadius = 128f + (1f - Ease.CubeOut(p)) * 128f;
				this.spotlightAlpha = Ease.CubeOut(p) * 0.8f;
				yield return null;
			}
			Audio.Play("event:/ui/postgame/unlock_newchapter_icon");
			this.HideIcon = false;
			this.wiggler.Start();
			yield return 2f;
			for (float p = 1f; p > 0f; p -= Engine.DeltaTime * 2f)
			{
				this.spotlightRadius = 128f + (1f - Ease.CubeOut(p)) * 128f;
				this.spotlightAlpha = Ease.CubeOut(p) * 0.8f;
				yield return null;
			}
			this.spotlightAlpha = 0f;
			if (onComplete != null)
			{
				onComplete();
			}
			yield break;
		}

		// Token: 0x060014A5 RID: 5285 RVA: 0x00072234 File Offset: 0x00070434
		private void StartTween(float duration, Action<Tween> callback)
		{
			this.StopTween();
			base.Add(this.tween = Tween.Create(Tween.TweenMode.Oneshot, null, duration, true));
			this.tween.OnUpdate = callback;
			this.tween.OnComplete = delegate(Tween t)
			{
				this.tween = null;
			};
		}

		// Token: 0x060014A6 RID: 5286 RVA: 0x00072282 File Offset: 0x00070482
		private void StopTween()
		{
			if (this.tween != null)
			{
				base.Remove(this.tween);
			}
			this.tween = null;
		}

		// Token: 0x060014A7 RID: 5287 RVA: 0x000722A0 File Offset: 0x000704A0
		private void SetSelectedPercent(Vector2 from, float p)
		{
			OuiChapterPanel ui = (base.Scene as Overworld).GetUI<OuiChapterPanel>();
			Vector2 vector = ui.OpenPosition + ui.IconOffset;
			SimpleCurve simpleCurve = new SimpleCurve(from, vector, (from + vector) / 2f + new Vector2(0f, 30f));
			float num = 1f + ((p < 0.5f) ? (p * 2f) : ((1f - p) * 2f));
			this.Scale.X = (float)Math.Cos((double)(Ease.SineInOut(p) * 6.2831855f)) * num;
			this.Scale.Y = num;
			this.Position = simpleCurve.GetPoint(Ease.Invert(Ease.CubeInOut)(p));
			this.Rotation = Ease.UpDown(Ease.SineInOut(p)) * 0.017453292f * 15f * (float)this.rotateDir;
			if (p <= 0f)
			{
				this.rotateDir = -1;
				return;
			}
			if (p >= 1f)
			{
				this.rotateDir = 1;
			}
		}

		// Token: 0x060014A8 RID: 5288 RVA: 0x000723BC File Offset: 0x000705BC
		public override void Update()
		{
			if (SaveData.Instance != null)
			{
				this.sizeEase = Calc.Approach(this.sizeEase, (SaveData.Instance.LastArea.ID == this.Area) ? 1f : 0f, Engine.DeltaTime * 4f);
				if (SaveData.Instance.LastArea.ID == this.Area)
				{
					base.Depth = -50;
				}
				else
				{
					base.Depth = -45;
				}
				if (this.tween == null)
				{
					if (this.selected)
					{
						OuiChapterPanel ui = (base.Scene as Overworld).GetUI<OuiChapterPanel>();
						this.Position = ((!ui.EnteringChapter) ? ui.OpenPosition : ui.Position) + ui.IconOffset;
					}
					else if (!this.hidden)
					{
						this.Position = Calc.Approach(this.Position, this.IdlePosition, 2400f * Engine.DeltaTime);
					}
				}
				if (this.New && base.Scene.OnInterval(1.5f))
				{
					this.newWiggle.Start();
				}
				base.Update();
			}
		}

		// Token: 0x060014A9 RID: 5289 RVA: 0x000724DC File Offset: 0x000706DC
		public override void Render()
		{
			MTexture mtexture = this.front;
			Vector2 vector = this.Scale;
			int num = mtexture.Width;
			if (vector.X < 0f)
			{
				mtexture = this.back;
			}
			if (this.AssistModeUnlockable)
			{
				mtexture = GFX.Gui["areas/lock"];
				num -= 32;
			}
			if (!this.HideIcon)
			{
				vector *= (100f + 44f * Ease.CubeInOut(this.sizeEase)) / (float)num;
				if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
				{
					vector.X = -vector.X;
				}
				mtexture.DrawCentered(this.Position + this.shake, Color.White, vector, this.Rotation);
				if (this.New && SaveData.Instance != null && !SaveData.Instance.CheatMode && this.Area == SaveData.Instance.UnlockedAreas && !this.selected && this.tween == null && !this.AssistModeUnlockable && Celeste.PlayMode != Celeste.PlayModes.Event)
				{
					Vector2 vector2 = this.Position + new Vector2((float)num * 0.25f, (float)(-(float)mtexture.Height) * 0.25f);
					vector2 += Vector2.UnitY * -Math.Abs(this.newWiggle.Value * 30f);
					GFX.Gui["areas/new"].DrawCentered(vector2);
				}
			}
			if (this.spotlightAlpha > 0f)
			{
				HiresRenderer.EndRender();
				SpotlightWipe.DrawSpotlight(new Vector2(this.Position.X, this.IdlePosition.Y), this.spotlightRadius, Color.Black * this.spotlightAlpha);
				HiresRenderer.BeginRender(null, null);
				return;
			}
			if (this.AssistModeUnlockable && SaveData.Instance.LastArea.ID == this.Area && !this.hidden)
			{
				ActiveFont.DrawOutline(Dialog.Clean("ASSIST_SKIP", null), this.Position + new Vector2(0f, 100f), new Vector2(0.5f, 0f), Vector2.One * 0.7f, Color.White, 2f, Color.Black);
			}
		}

		// Token: 0x04001058 RID: 4184
		public const float IdleSize = 100f;

		// Token: 0x04001059 RID: 4185
		public const float HoverSize = 144f;

		// Token: 0x0400105A RID: 4186
		public const float HoverSpacing = 80f;

		// Token: 0x0400105B RID: 4187
		public const float IdleY = 130f;

		// Token: 0x0400105C RID: 4188
		public const float HoverY = 140f;

		// Token: 0x0400105D RID: 4189
		public const float Spacing = 32f;

		// Token: 0x0400105E RID: 4190
		public int Area;

		// Token: 0x0400105F RID: 4191
		public bool New;

		// Token: 0x04001060 RID: 4192
		public Vector2 Scale = Vector2.One;

		// Token: 0x04001061 RID: 4193
		public float Rotation;

		// Token: 0x04001062 RID: 4194
		public float sizeEase = 1f;

		// Token: 0x04001063 RID: 4195
		public bool AssistModeUnlockable;

		// Token: 0x04001064 RID: 4196
		public bool HideIcon;

		// Token: 0x04001065 RID: 4197
		private Wiggler newWiggle;

		// Token: 0x04001066 RID: 4198
		private bool hidden = true;

		// Token: 0x04001067 RID: 4199
		private bool selected;

		// Token: 0x04001068 RID: 4200
		private Tween tween;

		// Token: 0x04001069 RID: 4201
		private Wiggler wiggler;

		// Token: 0x0400106A RID: 4202
		private bool wiggleLeft;

		// Token: 0x0400106B RID: 4203
		private int rotateDir = -1;

		// Token: 0x0400106C RID: 4204
		private Vector2 shake;

		// Token: 0x0400106D RID: 4205
		private float spotlightAlpha;

		// Token: 0x0400106E RID: 4206
		private float spotlightRadius;

		// Token: 0x0400106F RID: 4207
		private MTexture front;

		// Token: 0x04001070 RID: 4208
		private MTexture back;
	}
}
