using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000209 RID: 521
	public class MiniTextboxTrigger : Trigger
	{
		// Token: 0x06001101 RID: 4353 RVA: 0x00051690 File Offset: 0x0004F890
		public MiniTextboxTrigger(EntityData data, Vector2 offset, EntityID id) : base(data, offset)
		{
			this.id = id;
			this.mode = data.Enum<MiniTextboxTrigger.Modes>("mode", MiniTextboxTrigger.Modes.OnPlayerEnter);
			this.dialogOptions = data.Attr("dialog_id", "").Split(new char[]
			{
				','
			});
			this.onlyOnce = data.Bool("only_once", false);
			this.deathCount = data.Int("death_count", -1);
			if (this.mode == MiniTextboxTrigger.Modes.OnTheoEnter)
			{
				base.Add(new HoldableCollider(delegate(Holdable c)
				{
					this.Trigger();
				}, null));
			}
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x00051729 File Offset: 0x0004F929
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (this.mode == MiniTextboxTrigger.Modes.OnLevelStart)
			{
				this.Trigger();
			}
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x00051741 File Offset: 0x0004F941
		public override void OnEnter(Player player)
		{
			if (this.mode == MiniTextboxTrigger.Modes.OnPlayerEnter)
			{
				this.Trigger();
			}
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x00051754 File Offset: 0x0004F954
		private void Trigger()
		{
			if (this.triggered)
			{
				return;
			}
			if (this.deathCount >= 0 && (base.Scene as Level).Session.DeathsInCurrentLevel != this.deathCount)
			{
				return;
			}
			this.triggered = true;
			base.Scene.Add(new MiniTextbox(Calc.Random.Choose(this.dialogOptions)));
			if (this.onlyOnce)
			{
				(base.Scene as Level).Session.DoNotLoad.Add(this.id);
			}
		}

		// Token: 0x04000CA0 RID: 3232
		private EntityID id;

		// Token: 0x04000CA1 RID: 3233
		private string[] dialogOptions;

		// Token: 0x04000CA2 RID: 3234
		private MiniTextboxTrigger.Modes mode;

		// Token: 0x04000CA3 RID: 3235
		private bool triggered;

		// Token: 0x04000CA4 RID: 3236
		private bool onlyOnce;

		// Token: 0x04000CA5 RID: 3237
		private int deathCount;

		// Token: 0x02000513 RID: 1299
		private enum Modes
		{
			// Token: 0x040024E4 RID: 9444
			OnPlayerEnter,
			// Token: 0x040024E5 RID: 9445
			OnLevelStart,
			// Token: 0x040024E6 RID: 9446
			OnTheoEnter
		}
	}
}
