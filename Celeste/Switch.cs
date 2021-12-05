using System;
using System.Collections.Generic;
using Monocle;

namespace Celeste
{
	// Token: 0x0200033C RID: 828
	[Tracked(false)]
	public class Switch : Component
	{
		// Token: 0x060019F3 RID: 6643 RVA: 0x000A6B21 File Offset: 0x000A4D21
		public Switch(bool groundReset) : base(true, false)
		{
			this.GroundReset = groundReset;
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x060019F4 RID: 6644 RVA: 0x000A6B32 File Offset: 0x000A4D32
		// (set) Token: 0x060019F5 RID: 6645 RVA: 0x000A6B3A File Offset: 0x000A4D3A
		public bool Activated { get; private set; }

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x060019F6 RID: 6646 RVA: 0x000A6B43 File Offset: 0x000A4D43
		// (set) Token: 0x060019F7 RID: 6647 RVA: 0x000A6B4B File Offset: 0x000A4D4B
		public bool Finished { get; private set; }

		// Token: 0x060019F8 RID: 6648 RVA: 0x000A6B54 File Offset: 0x000A4D54
		public override void EntityAdded(Scene scene)
		{
			base.EntityAdded(scene);
			if (Switch.CheckLevelFlag(base.SceneAs<Level>()))
			{
				this.StartFinished();
			}
		}

		// Token: 0x060019F9 RID: 6649 RVA: 0x000A6B70 File Offset: 0x000A4D70
		public override void Update()
		{
			base.Update();
			if (this.GroundReset && this.Activated && !this.Finished)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.OnGround(1))
				{
					this.Deactivate();
				}
			}
		}

		// Token: 0x060019FA RID: 6650 RVA: 0x000A6BBE File Offset: 0x000A4DBE
		public bool Activate()
		{
			if (!this.Finished && !this.Activated)
			{
				this.Activated = true;
				if (this.OnActivate != null)
				{
					this.OnActivate();
				}
				return Switch.FinishedCheck(base.SceneAs<Level>());
			}
			return false;
		}

		// Token: 0x060019FB RID: 6651 RVA: 0x000A6BF7 File Offset: 0x000A4DF7
		public void Deactivate()
		{
			if (!this.Finished && this.Activated)
			{
				this.Activated = false;
				if (this.OnDeactivate != null)
				{
					this.OnDeactivate();
				}
			}
		}

		// Token: 0x060019FC RID: 6652 RVA: 0x000A6C23 File Offset: 0x000A4E23
		public void Finish()
		{
			this.Finished = true;
			if (this.OnFinish != null)
			{
				this.OnFinish();
			}
		}

		// Token: 0x060019FD RID: 6653 RVA: 0x000A6C40 File Offset: 0x000A4E40
		public void StartFinished()
		{
			if (!this.Finished)
			{
				this.Finished = (this.Activated = true);
				if (this.OnStartFinished != null)
				{
					this.OnStartFinished();
				}
			}
		}

		// Token: 0x060019FE RID: 6654 RVA: 0x000A6C78 File Offset: 0x000A4E78
		public static bool Check(Scene scene)
		{
			Switch component = scene.Tracker.GetComponent<Switch>();
			return component != null && component.Finished;
		}

		// Token: 0x060019FF RID: 6655 RVA: 0x000A6C9C File Offset: 0x000A4E9C
		private static bool FinishedCheck(Level level)
		{
			using (List<Component>.Enumerator enumerator = level.Tracker.GetComponents<Switch>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!((Switch)enumerator.Current).Activated)
					{
						return false;
					}
				}
			}
			foreach (Component component in level.Tracker.GetComponents<Switch>())
			{
				((Switch)component).Finish();
			}
			return true;
		}

		// Token: 0x06001A00 RID: 6656 RVA: 0x000A6D48 File Offset: 0x000A4F48
		public static bool CheckLevelFlag(Level level)
		{
			return level.Session.GetFlag("switches_" + level.Session.Level);
		}

		// Token: 0x06001A01 RID: 6657 RVA: 0x000A6D6A File Offset: 0x000A4F6A
		public static void SetLevelFlag(Level level)
		{
			level.Session.SetFlag("switches_" + level.Session.Level, true);
		}

		// Token: 0x040016A1 RID: 5793
		public bool GroundReset;

		// Token: 0x040016A2 RID: 5794
		public Action OnActivate;

		// Token: 0x040016A3 RID: 5795
		public Action OnDeactivate;

		// Token: 0x040016A4 RID: 5796
		public Action OnFinish;

		// Token: 0x040016A5 RID: 5797
		public Action OnStartFinished;
	}
}
