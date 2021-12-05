using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000174 RID: 372
	public class CS07_Ascend : CutsceneEntity
	{
		// Token: 0x06000D35 RID: 3381 RVA: 0x0002CC34 File Offset: 0x0002AE34
		public CS07_Ascend(int index, string cutscene, bool dark) : base(true, false)
		{
			this.index = index;
			this.cutscene = cutscene;
			this.dark = dark;
		}

		// Token: 0x06000D36 RID: 3382 RVA: 0x0002CC53 File Offset: 0x0002AE53
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(), true));
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x0002CC67 File Offset: 0x0002AE67
		private IEnumerator Cutscene()
		{
			while ((this.player = base.Scene.Tracker.GetEntity<Player>()) == null)
			{
				yield return null;
			}
			this.origin = this.player.Position;
			Audio.Play("event:/char/badeline/maddy_split");
			this.player.CreateSplitParticles();
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			this.Level.Displacement.AddBurst(this.player.Position, 0.4f, 8f, 32f, 0.5f, null, null);
			this.player.Dashes = 1;
			this.player.Facing = Facings.Right;
			base.Scene.Add(this.badeline = new BadelineDummy(this.player.Position));
			this.badeline.AutoAnimator.Enabled = false;
			this.spinning = true;
			base.Add(new Coroutine(this.SpinCharacters(), true));
			yield return Textbox.Say(this.cutscene, new Func<IEnumerator>[0]);
			Audio.Play("event:/char/badeline/maddy_join");
			this.spinning = false;
			yield return 0.25f;
			this.badeline.RemoveSelf();
			this.player.Dashes = 2;
			this.player.CreateSplitParticles();
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			this.Level.Displacement.AddBurst(this.player.Position, 0.4f, 8f, 32f, 0.5f, null, null);
			base.EndCutscene(this.Level, true);
			yield break;
		}

		// Token: 0x06000D38 RID: 3384 RVA: 0x0002CC76 File Offset: 0x0002AE76
		private IEnumerator SpinCharacters()
		{
			float dist = 0f;
			Vector2 center = this.player.Position;
			float timer = 1.5707964f;
			this.player.Sprite.Play("spin", false, false);
			this.badeline.Sprite.Play("spin", false, false);
			this.badeline.Sprite.Scale.X = 1f;
			while (this.spinning || dist > 0f)
			{
				dist = Calc.Approach(dist, this.spinning ? 1f : 0f, Engine.DeltaTime * 4f);
				int num = (int)(timer / 6.2831855f * 14f + 10f);
				float num2 = (float)Math.Sin((double)timer);
				float num3 = (float)Math.Cos((double)timer);
				float num4 = Ease.CubeOut(dist) * 32f;
				this.player.Sprite.SetAnimationFrame(num);
				this.badeline.Sprite.SetAnimationFrame(num + 7);
				this.player.Position = center - new Vector2(num2 * num4, num3 * dist * 8f);
				this.badeline.Position = center + new Vector2(num2 * num4, num3 * dist * 8f);
				timer -= Engine.DeltaTime * 2f;
				if (timer <= 0f)
				{
					timer += 6.2831855f;
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000D39 RID: 3385 RVA: 0x0002CC88 File Offset: 0x0002AE88
		public override void OnEnd(Level level)
		{
			if (this.badeline != null)
			{
				this.badeline.RemoveSelf();
			}
			if (this.player != null)
			{
				this.player.Dashes = 2;
				this.player.Position = this.origin;
			}
			if (!this.dark)
			{
				level.Add(new HeightDisplay(this.index));
			}
		}

		// Token: 0x04000875 RID: 2165
		private int index;

		// Token: 0x04000876 RID: 2166
		private string cutscene;

		// Token: 0x04000877 RID: 2167
		private BadelineDummy badeline;

		// Token: 0x04000878 RID: 2168
		private Player player;

		// Token: 0x04000879 RID: 2169
		private Vector2 origin;

		// Token: 0x0400087A RID: 2170
		private bool spinning;

		// Token: 0x0400087B RID: 2171
		private bool dark;
	}
}
