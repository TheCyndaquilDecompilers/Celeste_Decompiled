using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000361 RID: 865
	public class Hahaha : Entity
	{
		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06001B3F RID: 6975 RVA: 0x000B1D6A File Offset: 0x000AFF6A
		// (set) Token: 0x06001B40 RID: 6976 RVA: 0x000B1D72 File Offset: 0x000AFF72
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				if (!this.enabled && value)
				{
					this.timer = 0f;
					this.counter = 0;
				}
				this.enabled = value;
			}
		}

		// Token: 0x06001B41 RID: 6977 RVA: 0x000B1D9C File Offset: 0x000AFF9C
		public Hahaha(Vector2 position, string ifSet = "", bool triggerLaughSfx = false, Vector2? triggerLaughSfxOrigin = null)
		{
			base.Depth = -10001;
			this.Position = position;
			this.ifSet = ifSet;
			if (triggerLaughSfx)
			{
				this.autoTriggerLaughSfx = triggerLaughSfx;
				this.autoTriggerLaughOrigin = triggerLaughSfxOrigin.Value;
			}
		}

		// Token: 0x06001B42 RID: 6978 RVA: 0x000B1DEC File Offset: 0x000AFFEC
		public Hahaha(EntityData data, Vector2 offset) : this(data.Position + offset, data.Attr("ifset", ""), data.Bool("triggerLaughSfx", false), new Vector2?((data.Nodes.Length != 0) ? (offset + data.Nodes[0]) : Vector2.Zero))
		{
		}

		// Token: 0x06001B43 RID: 6979 RVA: 0x000B1E4E File Offset: 0x000B004E
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (!string.IsNullOrEmpty(this.ifSet) && !(base.Scene as Level).Session.GetFlag(this.ifSet))
			{
				this.Enabled = false;
			}
		}

		// Token: 0x06001B44 RID: 6980 RVA: 0x000B1E88 File Offset: 0x000B0088
		public override void Update()
		{
			if (this.Enabled)
			{
				this.timer -= Engine.DeltaTime;
				if (this.timer <= 0f)
				{
					this.has.Add(new Hahaha.Ha());
					this.counter++;
					if (this.counter >= 3)
					{
						this.counter = 0;
						this.timer = 1.5f;
					}
					else
					{
						this.timer = 0.6f;
					}
				}
				if (this.autoTriggerLaughSfx && base.Scene.OnInterval(0.4f))
				{
					Audio.Play("event:/char/granny/laugh_oneha", this.autoTriggerLaughOrigin);
				}
			}
			for (int i = this.has.Count - 1; i >= 0; i--)
			{
				if (this.has[i].Percent > 1f)
				{
					this.has.RemoveAt(i);
				}
				else
				{
					this.has[i].Sprite.Update();
					this.has[i].Percent += Engine.DeltaTime / this.has[i].Duration;
				}
			}
			if (!this.Enabled && !string.IsNullOrEmpty(this.ifSet) && (base.Scene as Level).Session.GetFlag(this.ifSet))
			{
				this.Enabled = true;
			}
			base.Update();
		}

		// Token: 0x06001B45 RID: 6981 RVA: 0x000B1FF4 File Offset: 0x000B01F4
		public override void Render()
		{
			foreach (Hahaha.Ha ha in this.has)
			{
				ha.Sprite.Position = this.Position + new Vector2(ha.Percent * 60f, -10f + (float)(-(float)Math.Sin((double)(ha.Percent * 13f))) * 4f + ha.Percent * -16f);
				ha.Sprite.Render();
			}
		}

		// Token: 0x04001816 RID: 6166
		private bool enabled;

		// Token: 0x04001817 RID: 6167
		private string ifSet;

		// Token: 0x04001818 RID: 6168
		private float timer;

		// Token: 0x04001819 RID: 6169
		private int counter;

		// Token: 0x0400181A RID: 6170
		private List<Hahaha.Ha> has = new List<Hahaha.Ha>();

		// Token: 0x0400181B RID: 6171
		private bool autoTriggerLaughSfx;

		// Token: 0x0400181C RID: 6172
		private Vector2 autoTriggerLaughOrigin;

		// Token: 0x0200071A RID: 1818
		private class Ha
		{
			// Token: 0x06002E35 RID: 11829 RVA: 0x00123C38 File Offset: 0x00121E38
			public Ha()
			{
				this.Sprite = new Sprite(GFX.Game, "characters/oldlady/");
				this.Sprite.Add("normal", "ha", 0.15f, new int[]
				{
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9
				});
				this.Sprite.Play("normal", false, false);
				this.Sprite.JustifyOrigin(0.5f, 0.5f);
				this.Duration = (float)this.Sprite.CurrentAnimationTotalFrames * 0.15f;
			}

			// Token: 0x04002DB2 RID: 11698
			public Sprite Sprite;

			// Token: 0x04002DB3 RID: 11699
			public float Percent;

			// Token: 0x04002DB4 RID: 11700
			public float Duration;
		}
	}
}
