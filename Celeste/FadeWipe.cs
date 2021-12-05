using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020002F1 RID: 753
	public class FadeWipe : ScreenWipe
	{
		// Token: 0x06001757 RID: 5975 RVA: 0x0008DBF2 File Offset: 0x0008BDF2
		public FadeWipe(Scene scene, bool wipeIn, Action onComplete = null) : base(scene, wipeIn, onComplete)
		{
		}

		// Token: 0x06001758 RID: 5976 RVA: 0x0008DC09 File Offset: 0x0008BE09
		public override void Update(Scene scene)
		{
			base.Update(scene);
			if (this.OnUpdate != null)
			{
				this.OnUpdate(this.Percent);
			}
		}

		// Token: 0x06001759 RID: 5977 RVA: 0x0008DC2C File Offset: 0x0008BE2C
		public override void Render(Scene scene)
		{
			Color color = ScreenWipe.WipeColor * (this.WipeIn ? (1f - Ease.CubeIn(this.Percent)) : Ease.CubeOut(this.Percent));
			this.vertexBuffer[0].Color = color;
			this.vertexBuffer[0].Position = new Vector3(-10f, -10f, 0f);
			this.vertexBuffer[1].Color = color;
			this.vertexBuffer[1].Position = new Vector3((float)base.Right, -10f, 0f);
			this.vertexBuffer[2].Color = color;
			this.vertexBuffer[2].Position = new Vector3(-10f, (float)base.Bottom, 0f);
			this.vertexBuffer[3].Color = color;
			this.vertexBuffer[3].Position = new Vector3((float)base.Right, -10f, 0f);
			this.vertexBuffer[4].Color = color;
			this.vertexBuffer[4].Position = new Vector3((float)base.Right, (float)base.Bottom, 0f);
			this.vertexBuffer[5].Color = color;
			this.vertexBuffer[5].Position = new Vector3(-10f, (float)base.Bottom, 0f);
			ScreenWipe.DrawPrimitives(this.vertexBuffer);
		}

		// Token: 0x040013FE RID: 5118
		private VertexPositionColor[] vertexBuffer = new VertexPositionColor[6];

		// Token: 0x040013FF RID: 5119
		public Action<float> OnUpdate;
	}
}
