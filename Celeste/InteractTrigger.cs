using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001E6 RID: 486
	public class InteractTrigger : Entity
	{
		// Token: 0x0600102E RID: 4142 RVA: 0x00045F4C File Offset: 0x0004414C
		public InteractTrigger(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			this.Events = new List<string>();
			this.Events.Add(data.Attr("event", ""));
			base.Collider = new Hitbox((float)data.Width, (float)data.Height, 0f, 0f);
			int num = 2;
			while (num < 100 && data.Has("event_" + num) && !string.IsNullOrEmpty(data.Attr("event_" + num, "")))
			{
				this.Events.Add(data.Attr("event_" + num, ""));
				num++;
			}
			Vector2 drawAt = new Vector2((float)(data.Width / 2), 0f);
			if (data.Nodes.Length != 0)
			{
				drawAt = data.Nodes[0] - data.Position;
			}
			base.Add(this.Talker = new TalkComponent(new Rectangle(0, 0, data.Width, data.Height), drawAt, new Action<Player>(this.OnTalk), null));
			this.Talker.PlayerMustBeFacing = false;
		}

		// Token: 0x0600102F RID: 4143 RVA: 0x00046098 File Offset: 0x00044298
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Session session = (scene as Level).Session;
			for (int i = 0; i < this.Events.Count; i++)
			{
				if (session.GetFlag("it_" + this.Events[i]))
				{
					this.eventIndex++;
				}
			}
			if (this.eventIndex >= this.Events.Count)
			{
				base.RemoveSelf();
				return;
			}
			if (this.Events[this.eventIndex] == "ch5_theo_phone")
			{
				scene.Add(new TheoPhone(this.Position + new Vector2(base.Width / 2f - 8f, base.Height - 1f)));
			}
		}

		// Token: 0x06001030 RID: 4144 RVA: 0x0004616C File Offset: 0x0004436C
		public void OnTalk(Player player)
		{
			if (!this.used)
			{
				bool flag = true;
				string text = this.Events[this.eventIndex];
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 1800778982U)
				{
					if (num <= 1100792133U)
					{
						if (num != 665077538U)
						{
							if (num == 1100792133U)
							{
								if (text == "ch5_theo_phone")
								{
									base.Scene.Add(new CS05_TheoPhone(player, base.Center.X));
								}
							}
						}
						else if (text == "ch3_memo")
						{
							base.Scene.Add(new CS03_Memo(player));
							flag = false;
						}
					}
					else if (num != 1560651562U)
					{
						if (num == 1800778982U)
						{
							if (text == "ch5_see_theo")
							{
								base.Scene.Add(new CS05_SeeTheo(player, 0));
							}
						}
					}
					else if (text == "ch2_poem")
					{
						base.Scene.Add(new CS02_Journal(player));
						flag = false;
					}
				}
				else if (num <= 2687997453U)
				{
					if (num != 1810268171U)
					{
						if (num == 2687997453U)
						{
							if (text == "ch3_guestbook")
							{
								base.Scene.Add(new CS03_Guestbook(player));
								flag = false;
							}
						}
					}
					else if (text == "ch3_diary")
					{
						base.Scene.Add(new CS03_Diary(player));
						flag = false;
					}
				}
				else if (num != 3686365467U)
				{
					if (num == 3823623439U)
					{
						if (text == "ch5_mirror_reflection")
						{
							base.Scene.Add(new CS05_Reflection1(player));
						}
					}
				}
				else if (text == "ch5_see_theo_b")
				{
					base.Scene.Add(new CS05_SeeTheo(player, 1));
				}
				if (flag)
				{
					(base.Scene as Level).Session.SetFlag("it_" + this.Events[this.eventIndex], true);
					this.eventIndex++;
					if (this.eventIndex >= this.Events.Count)
					{
						this.used = true;
						this.timeout = 0.25f;
					}
				}
			}
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x000463D8 File Offset: 0x000445D8
		public override void Update()
		{
			if (this.used)
			{
				this.timeout -= Engine.DeltaTime;
				if (this.timeout <= 0f)
				{
					base.RemoveSelf();
				}
			}
			else
			{
				while (this.eventIndex < this.Events.Count && (base.Scene as Level).Session.GetFlag("it_" + this.Events[this.eventIndex]))
				{
					this.eventIndex++;
				}
				if (this.eventIndex >= this.Events.Count)
				{
					base.RemoveSelf();
				}
			}
			base.Update();
		}

		// Token: 0x04000B8F RID: 2959
		public const string FlagPrefix = "it_";

		// Token: 0x04000B90 RID: 2960
		public TalkComponent Talker;

		// Token: 0x04000B91 RID: 2961
		public List<string> Events;

		// Token: 0x04000B92 RID: 2962
		private int eventIndex;

		// Token: 0x04000B93 RID: 2963
		private float timeout;

		// Token: 0x04000B94 RID: 2964
		private bool used;
	}
}
