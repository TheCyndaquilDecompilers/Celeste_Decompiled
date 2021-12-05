using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002DA RID: 730
	[Tracked(false)]
	public class TempleGate : Solid
	{
		// Token: 0x06001691 RID: 5777 RVA: 0x00085C64 File Offset: 0x00083E64
		public TempleGate(Vector2 position, int height, TempleGate.Types type, string spriteName, string levelID) : base(position, 8f, (float)height, true)
		{
			this.Type = type;
			this.closedHeight = height;
			this.LevelID = levelID;
			base.Add(this.sprite = GFX.SpriteBank.Create("templegate_" + spriteName));
			this.sprite.X = base.Collider.Width / 2f;
			this.sprite.Play("idle", false, false);
			base.Add(this.shaker = new Shaker(false, null));
			base.Depth = -9000;
			this.theoGate = spriteName.Equals("theo", StringComparison.InvariantCultureIgnoreCase);
			this.holdingCheckFrom = this.Position + new Vector2(base.Width / 2f, (float)(height / 2));
		}

		// Token: 0x06001692 RID: 5778 RVA: 0x00085D4E File Offset: 0x00083F4E
		public TempleGate(EntityData data, Vector2 offset, string levelID) : this(data.Position + offset, data.Height, data.Enum<TempleGate.Types>("type", TempleGate.Types.NearestSwitch), data.Attr("sprite", "default"), levelID)
		{
		}

		// Token: 0x06001693 RID: 5779 RVA: 0x00085D88 File Offset: 0x00083F88
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (this.Type == TempleGate.Types.CloseBehindPlayer)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.Left < base.Right && entity.Bottom >= base.Top && entity.Top <= base.Bottom)
				{
					this.StartOpen();
					base.Add(new Coroutine(this.CloseBehindPlayer(), true));
				}
			}
			else if (this.Type == TempleGate.Types.CloseBehindPlayerAlways)
			{
				this.StartOpen();
				base.Add(new Coroutine(this.CloseBehindPlayer(), true));
			}
			else if (this.Type == TempleGate.Types.CloseBehindPlayerAndTheo)
			{
				this.StartOpen();
				base.Add(new Coroutine(this.CloseBehindPlayerAndTheo(), true));
			}
			else if (this.Type == TempleGate.Types.HoldingTheo)
			{
				if (this.TheoIsNearby())
				{
					this.StartOpen();
				}
				base.Hitbox.Width = 16f;
			}
			else if (this.Type == TempleGate.Types.TouchSwitches)
			{
				base.Add(new Coroutine(this.CheckTouchSwitches(), true));
			}
			this.drawHeight = Math.Max(4f, base.Height);
		}

		// Token: 0x06001694 RID: 5780 RVA: 0x00085EAC File Offset: 0x000840AC
		public bool CloseBehindPlayerCheck()
		{
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			return entity != null && entity.X < base.X;
		}

		// Token: 0x06001695 RID: 5781 RVA: 0x00085EDD File Offset: 0x000840DD
		public void SwitchOpen()
		{
			this.sprite.Play("open", false, false);
			Alarm.Set(this, 0.2f, delegate
			{
				this.shaker.ShakeFor(0.2f, false);
				Alarm.Set(this, 0.2f, new Action(this.Open), Alarm.AlarmMode.Oneshot);
			}, Alarm.AlarmMode.Oneshot);
		}

		// Token: 0x06001696 RID: 5782 RVA: 0x00085F0C File Offset: 0x0008410C
		public void Open()
		{
			Audio.Play(this.theoGate ? "event:/game/05_mirror_temple/gate_theo_open" : "event:/game/05_mirror_temple/gate_main_open", this.Position);
			this.holdingWaitTimer = 0.2f;
			this.drawHeightMoveSpeed = 200f;
			this.drawHeight = base.Height;
			this.shaker.ShakeFor(0.2f, false);
			this.SetHeight(0);
			this.sprite.Play("open", false, false);
			this.open = true;
		}

		// Token: 0x06001697 RID: 5783 RVA: 0x00085F8D File Offset: 0x0008418D
		public void StartOpen()
		{
			this.SetHeight(0);
			this.drawHeight = 4f;
			this.open = true;
		}

		// Token: 0x06001698 RID: 5784 RVA: 0x00085FA8 File Offset: 0x000841A8
		public void Close()
		{
			Audio.Play(this.theoGate ? "event:/game/05_mirror_temple/gate_theo_close" : "event:/game/05_mirror_temple/gate_main_close", this.Position);
			this.holdingWaitTimer = 0.2f;
			this.drawHeightMoveSpeed = 300f;
			this.drawHeight = Math.Max(4f, base.Height);
			this.shaker.ShakeFor(0.2f, false);
			this.SetHeight(this.closedHeight);
			this.sprite.Play("hit", false, false);
			this.open = false;
		}

		// Token: 0x06001699 RID: 5785 RVA: 0x00086038 File Offset: 0x00084238
		private IEnumerator CloseBehindPlayer()
		{
			for (;;)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (!this.lockState && entity != null && entity.Left > base.Right + 4f)
				{
					break;
				}
				yield return null;
			}
			this.Close();
			yield break;
		}

		// Token: 0x0600169A RID: 5786 RVA: 0x00086047 File Offset: 0x00084247
		private IEnumerator CloseBehindPlayerAndTheo()
		{
			for (;;)
			{
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null && entity.Left > base.Right + 4f)
				{
					TheoCrystal entity2 = base.Scene.Tracker.GetEntity<TheoCrystal>();
					if (!this.lockState && entity2 != null && entity2.Left > base.Right + 4f)
					{
						break;
					}
				}
				yield return null;
			}
			this.Close();
			yield break;
		}

		// Token: 0x0600169B RID: 5787 RVA: 0x00086056 File Offset: 0x00084256
		private IEnumerator CheckTouchSwitches()
		{
			while (!Switch.Check(base.Scene))
			{
				yield return null;
			}
			this.sprite.Play("open", false, false);
			yield return 0.5f;
			this.shaker.ShakeFor(0.2f, false);
			yield return 0.2f;
			while (this.lockState)
			{
				yield return null;
			}
			this.Open();
			yield break;
		}

		// Token: 0x0600169C RID: 5788 RVA: 0x00086068 File Offset: 0x00084268
		public bool TheoIsNearby()
		{
			TheoCrystal entity = base.Scene.Tracker.GetEntity<TheoCrystal>();
			return entity == null || entity.X > base.X + 10f || Vector2.DistanceSquared(this.holdingCheckFrom, entity.Center) < (this.open ? 6400f : 4096f);
		}

		// Token: 0x0600169D RID: 5789 RVA: 0x000860C8 File Offset: 0x000842C8
		private void SetHeight(int height)
		{
			if ((float)height < base.Collider.Height)
			{
				base.Collider.Height = (float)height;
				return;
			}
			float y = base.Y;
			int num = (int)base.Collider.Height;
			if (base.Collider.Height < 64f)
			{
				base.Y -= 64f - base.Collider.Height;
				base.Collider.Height = 64f;
			}
			this.MoveVExact(height - num);
			base.Y = y;
			base.Collider.Height = (float)height;
		}

		// Token: 0x0600169E RID: 5790 RVA: 0x00086164 File Offset: 0x00084364
		public override void Update()
		{
			base.Update();
			if (this.Type == TempleGate.Types.HoldingTheo)
			{
				if (this.holdingWaitTimer > 0f)
				{
					this.holdingWaitTimer -= Engine.DeltaTime;
				}
				else if (!this.lockState)
				{
					if (this.open && !this.TheoIsNearby())
					{
						this.Close();
						Player player = base.CollideFirst<Player>(this.Position + new Vector2(8f, 0f));
						if (player != null)
						{
							player.Die(Vector2.Zero, false, true);
						}
					}
					else if (!this.open && this.TheoIsNearby())
					{
						this.Open();
					}
				}
			}
			float num = Math.Max(4f, base.Height);
			if (this.drawHeight != num)
			{
				this.lockState = true;
				this.drawHeight = Calc.Approach(this.drawHeight, num, this.drawHeightMoveSpeed * Engine.DeltaTime);
				return;
			}
			this.lockState = false;
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x00086254 File Offset: 0x00084454
		public override void Render()
		{
			Vector2 value = new Vector2((float)Math.Sign(this.shaker.Value.X), 0f);
			Draw.Rect(base.X - 2f, base.Y - 8f, 14f, 10f, Color.Black);
			this.sprite.DrawSubrect(Vector2.Zero + value, new Rectangle(0, (int)(this.sprite.Height - this.drawHeight), (int)this.sprite.Width, (int)this.drawHeight));
		}

		// Token: 0x0400130F RID: 4879
		private const int OpenHeight = 0;

		// Token: 0x04001310 RID: 4880
		private const float HoldingWaitTime = 0.2f;

		// Token: 0x04001311 RID: 4881
		private const float HoldingOpenDistSq = 4096f;

		// Token: 0x04001312 RID: 4882
		private const float HoldingCloseDistSq = 6400f;

		// Token: 0x04001313 RID: 4883
		private const int MinDrawHeight = 4;

		// Token: 0x04001314 RID: 4884
		public string LevelID;

		// Token: 0x04001315 RID: 4885
		public TempleGate.Types Type;

		// Token: 0x04001316 RID: 4886
		public bool ClaimedByASwitch;

		// Token: 0x04001317 RID: 4887
		private bool theoGate;

		// Token: 0x04001318 RID: 4888
		private int closedHeight;

		// Token: 0x04001319 RID: 4889
		private Sprite sprite;

		// Token: 0x0400131A RID: 4890
		private Shaker shaker;

		// Token: 0x0400131B RID: 4891
		private float drawHeight;

		// Token: 0x0400131C RID: 4892
		private float drawHeightMoveSpeed;

		// Token: 0x0400131D RID: 4893
		private bool open;

		// Token: 0x0400131E RID: 4894
		private float holdingWaitTimer = 0.2f;

		// Token: 0x0400131F RID: 4895
		private Vector2 holdingCheckFrom;

		// Token: 0x04001320 RID: 4896
		private bool lockState;

		// Token: 0x02000678 RID: 1656
		public enum Types
		{
			// Token: 0x04002AD6 RID: 10966
			NearestSwitch,
			// Token: 0x04002AD7 RID: 10967
			CloseBehindPlayer,
			// Token: 0x04002AD8 RID: 10968
			CloseBehindPlayerAlways,
			// Token: 0x04002AD9 RID: 10969
			HoldingTheo,
			// Token: 0x04002ADA RID: 10970
			TouchSwitches,
			// Token: 0x04002ADB RID: 10971
			CloseBehindPlayerAndTheo
		}
	}
}
