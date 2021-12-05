using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000369 RID: 873
	public abstract class ScreenWipe : Renderer
	{
		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06001B71 RID: 7025 RVA: 0x000B379C File Offset: 0x000B199C
		public int Right
		{
			get
			{
				return 1930;
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06001B72 RID: 7026 RVA: 0x000B37A3 File Offset: 0x000B19A3
		public int Bottom
		{
			get
			{
				return 1090;
			}
		}

		// Token: 0x06001B73 RID: 7027 RVA: 0x000B37AC File Offset: 0x000B19AC
		public ScreenWipe(Scene scene, bool wipeIn, Action onComplete = null)
		{
			this.Scene = scene;
			this.WipeIn = wipeIn;
			if (this.Scene is Level)
			{
				(this.Scene as Level).Wipe = this;
			}
			this.Scene.Add(this);
			this.OnComplete = onComplete;
		}

		// Token: 0x06001B74 RID: 7028 RVA: 0x000B3809 File Offset: 0x000B1A09
		public IEnumerator Wait()
		{
			while (this.Percent < 1f)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001B75 RID: 7029 RVA: 0x000B3818 File Offset: 0x000B1A18
		public override void Update(Scene scene)
		{
			if (this.Completed)
			{
				if (!this.ending)
				{
					this.ending = true;
					scene.Remove(this);
					if (scene is Level && (scene as Level).Wipe == this)
					{
						(scene as Level).Wipe = null;
					}
					if (this.OnComplete != null)
					{
						this.OnComplete();
					}
				}
				return;
			}
			if (this.Percent < 1f)
			{
				this.Percent = Calc.Approach(this.Percent, 1f, Engine.RawDeltaTime / this.Duration);
				return;
			}
			if (this.EndTimer > 0f)
			{
				this.EndTimer -= Engine.RawDeltaTime;
				return;
			}
			this.Completed = true;
		}

		// Token: 0x06001B76 RID: 7030 RVA: 0x000B38D0 File Offset: 0x000B1AD0
		public virtual void Cancel()
		{
			this.Scene.Remove(this);
			if (this.Scene is Level)
			{
				(this.Scene as Level).Wipe = null;
			}
		}

		// Token: 0x06001B77 RID: 7031 RVA: 0x000B38FC File Offset: 0x000B1AFC
		public static void DrawPrimitives(VertexPositionColor[] vertices)
		{
			GFX.DrawVertices<VertexPositionColor>(Matrix.CreateScale((float)Engine.Graphics.GraphicsDevice.Viewport.Width / 1920f), vertices, vertices.Length, null, null);
		}

		// Token: 0x0400187B RID: 6267
		public static Color WipeColor = Color.Black;

		// Token: 0x0400187C RID: 6268
		public Scene Scene;

		// Token: 0x0400187D RID: 6269
		public bool WipeIn;

		// Token: 0x0400187E RID: 6270
		public float Percent;

		// Token: 0x0400187F RID: 6271
		public Action OnComplete;

		// Token: 0x04001880 RID: 6272
		public bool Completed;

		// Token: 0x04001881 RID: 6273
		public float Duration = 0.5f;

		// Token: 0x04001882 RID: 6274
		public float EndTimer;

		// Token: 0x04001883 RID: 6275
		private bool ending;

		// Token: 0x04001884 RID: 6276
		public const int Left = -10;

		// Token: 0x04001885 RID: 6277
		public const int Top = -10;
	}
}
