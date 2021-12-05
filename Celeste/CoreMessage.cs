using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001C1 RID: 449
	public class CoreMessage : Entity
	{
		// Token: 0x06000F5E RID: 3934 RVA: 0x0003E778 File Offset: 0x0003C978
		public CoreMessage(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Tag = Tags.HUD;
			string[] array = Dialog.Clean("app_ending", null).Split(new char[]
			{
				'\n',
				'\r'
			}, StringSplitOptions.RemoveEmptyEntries);
			this.text = array[data.Int("line", 0)];
		}

		// Token: 0x06000F5F RID: 3935 RVA: 0x0003E7E0 File Offset: 0x0003C9E0
		public override void Update()
		{
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				this.alpha = Ease.CubeInOut(Calc.ClampedMap(Math.Abs(base.X - entity.X), 0f, 128f, 1f, 0f));
			}
			base.Update();
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x0003E844 File Offset: 0x0003CA44
		public override void Render()
		{
			Vector2 position = (base.Scene as Level).Camera.Position;
			Vector2 value = position + new Vector2(160f, 90f);
			Vector2 vector = (this.Position - position + (this.Position - value) * 0.2f) * 6f;
			if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
			{
				vector.X = 1920f - vector.X;
			}
			ActiveFont.Draw(this.text, vector, new Vector2(0.5f, 0.5f), Vector2.One * 1.25f, Color.White * this.alpha);
		}

		// Token: 0x04000AC3 RID: 2755
		private string text;

		// Token: 0x04000AC4 RID: 2756
		private float alpha;
	}
}
