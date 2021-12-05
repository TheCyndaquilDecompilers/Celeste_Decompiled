using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002E2 RID: 738
	public class WindController : Entity
	{
		// Token: 0x060016BE RID: 5822 RVA: 0x00086D87 File Offset: 0x00084F87
		public WindController(WindController.Patterns pattern)
		{
			base.Tag = Tags.TransitionUpdate;
			this.startPattern = pattern;
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x00086DA6 File Offset: 0x00084FA6
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
		}

		// Token: 0x060016C0 RID: 5824 RVA: 0x00086DBB File Offset: 0x00084FBB
		public void SetStartPattern()
		{
			if (!this.everSetPattern)
			{
				this.SetPattern(this.startPattern);
			}
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x00086DD4 File Offset: 0x00084FD4
		public void SetPattern(WindController.Patterns pattern)
		{
			if (this.pattern == pattern && this.everSetPattern)
			{
				return;
			}
			this.everSetPattern = true;
			this.pattern = pattern;
			if (this.coroutine != null)
			{
				base.Remove(this.coroutine);
				this.coroutine = null;
			}
			switch (pattern)
			{
			case WindController.Patterns.None:
				this.targetSpeed = Vector2.Zero;
				this.SetAmbienceStrength(false);
				return;
			case WindController.Patterns.Left:
				this.targetSpeed.X = -400f;
				this.SetAmbienceStrength(false);
				return;
			case WindController.Patterns.Right:
				this.targetSpeed.X = 400f;
				this.SetAmbienceStrength(false);
				return;
			case WindController.Patterns.LeftStrong:
				this.targetSpeed.X = -800f;
				this.SetAmbienceStrength(true);
				return;
			case WindController.Patterns.RightStrong:
				this.targetSpeed.X = 800f;
				this.SetAmbienceStrength(true);
				return;
			case WindController.Patterns.LeftOnOff:
				base.Add(this.coroutine = new Coroutine(this.LeftOnOffSequence(), true));
				return;
			case WindController.Patterns.RightOnOff:
				base.Add(this.coroutine = new Coroutine(this.RightOnOffSequence(), true));
				return;
			case WindController.Patterns.LeftOnOffFast:
				base.Add(this.coroutine = new Coroutine(this.LeftOnOffFastSequence(), true));
				return;
			case WindController.Patterns.RightOnOffFast:
				base.Add(this.coroutine = new Coroutine(this.RightOnOffFastSequence(), true));
				return;
			case WindController.Patterns.Alternating:
				base.Add(this.coroutine = new Coroutine(this.AlternatingSequence(), true));
				return;
			case WindController.Patterns.LeftGemsOnly:
				break;
			case WindController.Patterns.RightCrazy:
				this.targetSpeed.X = 1200f;
				this.SetAmbienceStrength(true);
				return;
			case WindController.Patterns.Down:
				this.targetSpeed.Y = 300f;
				this.SetAmbienceStrength(false);
				return;
			case WindController.Patterns.Up:
				this.targetSpeed.Y = -400f;
				this.SetAmbienceStrength(false);
				return;
			case WindController.Patterns.Space:
				this.targetSpeed.Y = -600f;
				this.SetAmbienceStrength(false);
				break;
			default:
				return;
			}
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x00086FC0 File Offset: 0x000851C0
		private void SetAmbienceStrength(bool strong)
		{
			int num = 0;
			if (this.targetSpeed.X != 0f)
			{
				num = Math.Sign(this.targetSpeed.X);
			}
			else if (this.targetSpeed.Y != 0f)
			{
				num = Math.Sign(this.targetSpeed.Y);
			}
			Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "wind_direction", (float)num);
			Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "strong_wind", (float)(strong ? 1 : 0));
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x0008703F File Offset: 0x0008523F
		public void SnapWind()
		{
			if (this.coroutine != null && this.coroutine.Active)
			{
				this.coroutine.Update();
			}
			this.level.Wind = this.targetSpeed;
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x00087074 File Offset: 0x00085274
		public override void Update()
		{
			base.Update();
			if (this.pattern == WindController.Patterns.LeftGemsOnly)
			{
				bool flag = false;
				using (List<Entity>.Enumerator enumerator = base.Scene.Tracker.GetEntities<StrawberrySeed>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (((StrawberrySeed)enumerator.Current).Collected)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					this.targetSpeed.X = -400f;
					this.SetAmbienceStrength(false);
				}
				else
				{
					this.targetSpeed.X = 0f;
					this.SetAmbienceStrength(false);
				}
			}
			this.level.Wind = Calc.Approach(this.level.Wind, this.targetSpeed, 1000f * Engine.DeltaTime);
			if (this.level.Wind != Vector2.Zero && !this.level.Transitioning)
			{
				foreach (Component component in base.Scene.Tracker.GetComponents<WindMover>())
				{
					((WindMover)component).Move(this.level.Wind * 0.1f * Engine.DeltaTime);
				}
			}
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x000871E0 File Offset: 0x000853E0
		private IEnumerator AlternatingSequence()
		{
			for (;;)
			{
				this.targetSpeed.X = -400f;
				this.SetAmbienceStrength(false);
				yield return 3f;
				this.targetSpeed.X = 0f;
				this.SetAmbienceStrength(false);
				yield return 2f;
				this.targetSpeed.X = 400f;
				this.SetAmbienceStrength(false);
				yield return 3f;
				this.targetSpeed.X = 0f;
				this.SetAmbienceStrength(false);
				yield return 2f;
			}
			yield break;
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x000871EF File Offset: 0x000853EF
		private IEnumerator RightOnOffSequence()
		{
			for (;;)
			{
				this.targetSpeed.X = 800f;
				this.SetAmbienceStrength(true);
				yield return 3f;
				this.targetSpeed.X = 0f;
				this.SetAmbienceStrength(false);
				yield return 3f;
			}
			yield break;
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x000871FE File Offset: 0x000853FE
		private IEnumerator LeftOnOffSequence()
		{
			for (;;)
			{
				this.targetSpeed.X = -800f;
				this.SetAmbienceStrength(true);
				yield return 3f;
				this.targetSpeed.X = 0f;
				this.SetAmbienceStrength(false);
				yield return 3f;
			}
			yield break;
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x0008720D File Offset: 0x0008540D
		private IEnumerator RightOnOffFastSequence()
		{
			for (;;)
			{
				this.targetSpeed.X = 800f;
				this.SetAmbienceStrength(true);
				yield return 2f;
				this.targetSpeed.X = 0f;
				this.SetAmbienceStrength(false);
				yield return 2f;
			}
			yield break;
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x0008721C File Offset: 0x0008541C
		private IEnumerator LeftOnOffFastSequence()
		{
			for (;;)
			{
				this.targetSpeed.X = -800f;
				this.SetAmbienceStrength(true);
				yield return 2f;
				this.targetSpeed.X = 0f;
				this.SetAmbienceStrength(false);
				yield return 2f;
			}
			yield break;
		}

		// Token: 0x04001346 RID: 4934
		private const float Weak = 400f;

		// Token: 0x04001347 RID: 4935
		private const float Strong = 800f;

		// Token: 0x04001348 RID: 4936
		private const float Crazy = 1200f;

		// Token: 0x04001349 RID: 4937
		private const float Accel = 1000f;

		// Token: 0x0400134A RID: 4938
		private const float Down = 300f;

		// Token: 0x0400134B RID: 4939
		private const float Up = -400f;

		// Token: 0x0400134C RID: 4940
		private const float Space = -600f;

		// Token: 0x0400134D RID: 4941
		private Level level;

		// Token: 0x0400134E RID: 4942
		private WindController.Patterns pattern;

		// Token: 0x0400134F RID: 4943
		private Vector2 targetSpeed;

		// Token: 0x04001350 RID: 4944
		private Coroutine coroutine;

		// Token: 0x04001351 RID: 4945
		private WindController.Patterns startPattern;

		// Token: 0x04001352 RID: 4946
		private bool everSetPattern;

		// Token: 0x02000682 RID: 1666
		public enum Patterns
		{
			// Token: 0x04002AFD RID: 11005
			None,
			// Token: 0x04002AFE RID: 11006
			Left,
			// Token: 0x04002AFF RID: 11007
			Right,
			// Token: 0x04002B00 RID: 11008
			LeftStrong,
			// Token: 0x04002B01 RID: 11009
			RightStrong,
			// Token: 0x04002B02 RID: 11010
			LeftOnOff,
			// Token: 0x04002B03 RID: 11011
			RightOnOff,
			// Token: 0x04002B04 RID: 11012
			LeftOnOffFast,
			// Token: 0x04002B05 RID: 11013
			RightOnOffFast,
			// Token: 0x04002B06 RID: 11014
			Alternating,
			// Token: 0x04002B07 RID: 11015
			LeftGemsOnly,
			// Token: 0x04002B08 RID: 11016
			RightCrazy,
			// Token: 0x04002B09 RID: 11017
			Down,
			// Token: 0x04002B0A RID: 11018
			Up,
			// Token: 0x04002B0B RID: 11019
			Space
		}
	}
}
