using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001E8 RID: 488
	public class MoonGlitchBackgroundTrigger : Trigger
	{
		// Token: 0x06001033 RID: 4147 RVA: 0x000464BD File Offset: 0x000446BD
		public MoonGlitchBackgroundTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.duration = data.Enum<MoonGlitchBackgroundTrigger.Duration>("duration", MoonGlitchBackgroundTrigger.Duration.Short);
			this.stayOn = data.Bool("stay", false);
			this.doGlitch = data.Bool("glitch", true);
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x000464FD File Offset: 0x000446FD
		public override void OnEnter(Player player)
		{
			this.Invoke();
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x00046505 File Offset: 0x00044705
		public void Invoke()
		{
			if (this.triggered)
			{
				return;
			}
			this.triggered = true;
			if (this.doGlitch)
			{
				base.Add(new Coroutine(this.InternalGlitchRoutine(), true));
				return;
			}
			if (!this.stayOn)
			{
				MoonGlitchBackgroundTrigger.Toggle(false);
			}
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x00046540 File Offset: 0x00044740
		private IEnumerator InternalGlitchRoutine()
		{
			this.running = true;
			base.Tag = Tags.Persistent;
			float num;
			if (this.duration == MoonGlitchBackgroundTrigger.Duration.Short)
			{
				num = 0.2f;
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
				Audio.Play("event:/new_content/game/10_farewell/glitch_short");
			}
			else if (this.duration == MoonGlitchBackgroundTrigger.Duration.Medium)
			{
				num = 0.5f;
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
				Audio.Play("event:/new_content/game/10_farewell/glitch_medium");
			}
			else
			{
				num = 1.25f;
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
				Audio.Play("event:/new_content/game/10_farewell/glitch_long");
			}
			yield return MoonGlitchBackgroundTrigger.GlitchRoutine(num, this.stayOn);
			base.Tag = 0;
			this.running = false;
			yield break;
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x00046550 File Offset: 0x00044750
		private static void Toggle(bool on)
		{
			Level level = Engine.Scene as Level;
			foreach (Backdrop backdrop in level.Background.GetEach<Backdrop>("blackhole"))
			{
				backdrop.ForceVisible = on;
			}
			foreach (Backdrop backdrop2 in level.Foreground.GetEach<Backdrop>("blackhole"))
			{
				backdrop2.ForceVisible = on;
			}
		}

		// Token: 0x06001038 RID: 4152 RVA: 0x000465F4 File Offset: 0x000447F4
		private static void Fade(float alpha, bool max = false)
		{
			Level level = Engine.Scene as Level;
			foreach (Backdrop backdrop in level.Background.GetEach<Backdrop>("blackhole"))
			{
				backdrop.FadeAlphaMultiplier = (max ? Math.Max(backdrop.FadeAlphaMultiplier, alpha) : alpha);
			}
			foreach (Backdrop backdrop2 in level.Foreground.GetEach<Backdrop>("blackhole"))
			{
				backdrop2.FadeAlphaMultiplier = (max ? Math.Max(backdrop2.FadeAlphaMultiplier, alpha) : alpha);
			}
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x000466C0 File Offset: 0x000448C0
		public static IEnumerator GlitchRoutine(float duration, bool stayOn)
		{
			MoonGlitchBackgroundTrigger.Toggle(true);
			if (Settings.Instance.DisableFlashes)
			{
				for (float a = 0f; a < 1f; a += Engine.DeltaTime / 0.1f)
				{
					MoonGlitchBackgroundTrigger.Fade(a, true);
					yield return null;
				}
				MoonGlitchBackgroundTrigger.Fade(1f, false);
				yield return duration;
				if (!stayOn)
				{
					for (float a = 0f; a < 1f; a += Engine.DeltaTime / 0.1f)
					{
						MoonGlitchBackgroundTrigger.Fade(1f - a, false);
						yield return null;
					}
					MoonGlitchBackgroundTrigger.Fade(1f, false);
				}
			}
			else if (duration > 0.4f)
			{
				Glitch.Value = 0.3f;
				yield return 0.2f;
				Glitch.Value = 0f;
				yield return duration - 0.4f;
				if (!stayOn)
				{
					Glitch.Value = 0.3f;
				}
				yield return 0.2f;
				Glitch.Value = 0f;
			}
			else
			{
				Glitch.Value = 0.3f;
				yield return duration;
				Glitch.Value = 0f;
			}
			if (!stayOn)
			{
				MoonGlitchBackgroundTrigger.Toggle(false);
			}
			yield break;
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x000466D6 File Offset: 0x000448D6
		public override void Removed(Scene scene)
		{
			if (this.running)
			{
				Glitch.Value = 0f;
				MoonGlitchBackgroundTrigger.Fade(1f, false);
				if (!this.stayOn)
				{
					MoonGlitchBackgroundTrigger.Toggle(false);
				}
			}
			base.Removed(scene);
		}

		// Token: 0x04000B95 RID: 2965
		private MoonGlitchBackgroundTrigger.Duration duration;

		// Token: 0x04000B96 RID: 2966
		private bool triggered;

		// Token: 0x04000B97 RID: 2967
		private bool stayOn;

		// Token: 0x04000B98 RID: 2968
		private bool running;

		// Token: 0x04000B99 RID: 2969
		private bool doGlitch;

		// Token: 0x020004E8 RID: 1256
		private enum Duration
		{
			// Token: 0x04002426 RID: 9254
			Short,
			// Token: 0x04002427 RID: 9255
			Medium,
			// Token: 0x04002428 RID: 9256
			Long
		}
	}
}
