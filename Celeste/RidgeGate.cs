using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001C7 RID: 455
	public class RidgeGate : Solid
	{
		// Token: 0x06000F8B RID: 3979 RVA: 0x00040C85 File Offset: 0x0003EE85
		public RidgeGate(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height, data.FirstNodeNullable(new Vector2?(offset)), data.Bool("ridge", true))
		{
		}

		// Token: 0x06000F8C RID: 3980 RVA: 0x00040CBF File Offset: 0x0003EEBF
		public RidgeGate(Vector2 position, float width, float height, Vector2? node, bool ridgeImage = true) : base(position, width, height, true)
		{
			this.node = node;
			base.Add(new Image(GFX.Game[ridgeImage ? "objects/ridgeGate" : "objects/farewellGate"]));
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x00040CF8 File Offset: 0x0003EEF8
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (this.node != null && base.CollideCheck<Player>())
			{
				this.Visible = (this.Collidable = false);
				Vector2 position = this.Position;
				this.Position = this.node.Value;
				base.Add(new Coroutine(this.EnterSequence(position), true));
			}
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x00040D5C File Offset: 0x0003EF5C
		private IEnumerator EnterSequence(Vector2 moveTo)
		{
			this.Visible = (this.Collidable = true);
			yield return 0.25f;
			Audio.Play("event:/game/04_cliffside/stone_blockade", this.Position);
			yield return 0.25f;
			Vector2 start = this.Position;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 1f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				this.MoveTo(Vector2.Lerp(start, moveTo, t.Eased));
			};
			base.Add(tween);
			yield break;
		}

		// Token: 0x04000AF3 RID: 2803
		private Vector2? node;
	}
}
