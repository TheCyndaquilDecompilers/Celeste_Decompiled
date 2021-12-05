using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000256 RID: 598
	public class OverworldReflectionsFall : Scene
	{
		// Token: 0x060012B0 RID: 4784 RVA: 0x00064D00 File Offset: 0x00062F00
		public OverworldReflectionsFall(Level returnTo, Action returnCallback)
		{
			this.returnTo = returnTo;
			this.returnCallback = returnCallback;
			base.Add(this.mountain = new MountainRenderer());
			this.mountain.SnapCamera(-1, new MountainCamera(this.startCamera.Position + (this.startCamera.Target - this.startCamera.Position).SafeNormalize() * 2f, this.startCamera.Target), false);
			base.Add(new HiresSnow(0.45f)
			{
				ParticleAlpha = 0f
			});
			base.Add(new Snow3D(this.mountain.Model));
			base.Add(this.maddy = new Maddy3D(this.mountain));
			this.maddy.Falling();
			base.Add(new Entity
			{
				new Coroutine(this.Routine(), true)
			});
		}

		// Token: 0x060012B1 RID: 4785 RVA: 0x00064E69 File Offset: 0x00063069
		private IEnumerator Routine()
		{
			this.mountain.EaseCamera(-1, this.startCamera, new float?(0.4f), true, false);
			float duration = 4f;
			this.maddy.Position = this.startCamera.Target;
			int num;
			for (int i = 0; i < 30; i = num + 1)
			{
				this.maddy.Position = this.startCamera.Target + new Vector3(Calc.Random.Range(-0.05f, 0.05f), Calc.Random.Range(-0.05f, 0.05f), Calc.Random.Range(-0.05f, 0.05f));
				yield return 0.01f;
				num = i;
			}
			yield return 0.1f;
			this.maddy.Add(new Coroutine(this.MaddyFall(duration + 0.1f), true));
			yield return 0.1f;
			this.mountain.EaseCamera(-1, this.fallCamera, new float?(duration), true, false);
			this.mountain.ForceNearFog = true;
			yield return duration;
			yield return 0.25f;
			MountainCamera transform = new MountainCamera(this.fallCamera.Position + this.mountain.Model.Forward * 3f, this.fallCamera.Target);
			this.mountain.EaseCamera(-1, transform, new float?(0.5f), true, false);
			this.Return();
			yield break;
		}

		// Token: 0x060012B2 RID: 4786 RVA: 0x00064E78 File Offset: 0x00063078
		private IEnumerator MaddyFall(float duration)
		{
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
			{
				this.maddy.Position = Vector3.Lerp(this.startCamera.Target, this.fallCamera.Target, p);
				yield return null;
			}
			yield break;
		}

		// Token: 0x060012B3 RID: 4787 RVA: 0x00064E8E File Offset: 0x0006308E
		private void Return()
		{
			new FadeWipe(this, false, delegate()
			{
				this.mountain.Dispose();
				if (this.returnTo != null)
				{
					Engine.Scene = this.returnTo;
				}
				this.returnCallback();
			});
		}

		// Token: 0x04000EAC RID: 3756
		private Level returnTo;

		// Token: 0x04000EAD RID: 3757
		private Action returnCallback;

		// Token: 0x04000EAE RID: 3758
		private Maddy3D maddy;

		// Token: 0x04000EAF RID: 3759
		private MountainRenderer mountain;

		// Token: 0x04000EB0 RID: 3760
		private MountainCamera startCamera = new MountainCamera(new Vector3(-8f, 12f, -0.4f), new Vector3(-2f, 9f, -0.5f));

		// Token: 0x04000EB1 RID: 3761
		private MountainCamera fallCamera = new MountainCamera(new Vector3(-10f, 6f, -0.4f), new Vector3(-4.25f, 1.5f, -1.25f));
	}
}
