using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000269 RID: 617
	public class NPC03_Oshiro_Cluttter : NPC
	{
		// Token: 0x06001343 RID: 4931 RVA: 0x00068A08 File Offset: 0x00066C08
		public NPC03_Oshiro_Cluttter(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Add(this.Sprite = new OshiroSprite(-1));
			base.Add(this.Talker = new TalkComponent(new Rectangle(-24, -8, 48, 8), new Vector2(0f, -24f), new Action<Player>(this.OnTalk), null));
			base.Add(this.Light = new VertexLight(-Vector2.UnitY * 16f, Color.White, 1f, 32, 64));
			this.MoveAnim = "move";
			this.IdleAnim = "idle";
			foreach (Vector2 value in data.Nodes)
			{
				this.nodes.Add(value + offset);
			}
			base.Add(this.paceSfx = new SoundSource());
		}

		// Token: 0x06001344 RID: 4932 RVA: 0x00068B1C File Offset: 0x00066D1C
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (base.Session.GetFlag("oshiro_clutter_finished"))
			{
				base.RemoveSelf();
			}
			else
			{
				if (base.Session.GetFlag("oshiro_clutter_cleared_0"))
				{
					this.sectionsComplete++;
				}
				if (base.Session.GetFlag("oshiro_clutter_cleared_1"))
				{
					this.sectionsComplete++;
				}
				if (base.Session.GetFlag("oshiro_clutter_cleared_2"))
				{
					this.sectionsComplete++;
				}
				if (this.sectionsComplete == 0 || this.sectionsComplete == 3)
				{
					this.Sprite.Scale.X = 1f;
				}
				if (this.sectionsComplete > 0)
				{
					this.Position = this.nodes[this.sectionsComplete - 1];
				}
				else if (!base.Session.GetFlag("oshiro_clutter_0"))
				{
					base.Add(this.paceRoutine = new Coroutine(this.Pace(), true));
				}
				if (this.sectionsComplete == 0 && base.Session.GetFlag("oshiro_clutter_0") && !base.Session.GetFlag("oshiro_clutter_optional_0"))
				{
					this.Sprite.Play("idle_ground", false, false);
				}
				if (this.sectionsComplete == 3 || base.Session.GetFlag("oshiro_clutter_optional_" + this.sectionsComplete))
				{
					base.Remove(this.Talker);
				}
			}
			this.HomePosition = this.Position;
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06001345 RID: 4933 RVA: 0x00068CA8 File Offset: 0x00066EA8
		public Vector2 ZoomPoint
		{
			get
			{
				if (this.sectionsComplete < 2)
				{
					return this.Position + new Vector2(0f, -30f) - this.Level.Camera.Position;
				}
				return this.Position + new Vector2(0f, -15f) - this.Level.Camera.Position;
			}
		}

		// Token: 0x06001346 RID: 4934 RVA: 0x00068D20 File Offset: 0x00066F20
		private void OnTalk(Player player)
		{
			this.talked = true;
			if (this.paceRoutine != null)
			{
				this.paceRoutine.RemoveSelf();
			}
			this.paceRoutine = null;
			if (!base.Session.GetFlag("oshiro_clutter_" + this.sectionsComplete))
			{
				base.Scene.Add(new CS03_OshiroClutter(player, this, this.sectionsComplete));
				return;
			}
			this.Level.StartCutscene(new Action<Level>(this.EndTalkRoutine), true, false, true);
			base.Session.SetFlag("oshiro_clutter_optional_" + this.sectionsComplete, true);
			base.Add(this.talkRoutine = new Coroutine(this.TalkRoutine(player), true));
			if (this.Talker != null)
			{
				this.Talker.Enabled = false;
			}
		}

		// Token: 0x06001347 RID: 4935 RVA: 0x00068DF4 File Offset: 0x00066FF4
		private IEnumerator TalkRoutine(Player player)
		{
			yield return base.PlayerApproach(player, true, new float?((float)24), new int?((this.sectionsComplete == 1 || this.sectionsComplete == 2) ? -1 : 1));
			yield return this.Level.ZoomTo(this.ZoomPoint, 2f, 0.5f);
			yield return Textbox.Say("CH3_OSHIRO_CLUTTER" + this.sectionsComplete + "_B", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.StandUp)
			});
			yield return this.Level.ZoomBack(0.5f);
			this.Level.EndCutscene();
			this.EndTalkRoutine(this.Level);
			yield break;
		}

		// Token: 0x06001348 RID: 4936 RVA: 0x00068E0C File Offset: 0x0006700C
		private void EndTalkRoutine(Level level)
		{
			if (this.talkRoutine != null)
			{
				this.talkRoutine.RemoveSelf();
			}
			this.talkRoutine = null;
			(this.Sprite as OshiroSprite).Pop("idle", false);
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				entity.StateMachine.Locked = false;
				entity.StateMachine.State = 0;
			}
		}

		// Token: 0x06001349 RID: 4937 RVA: 0x00068E75 File Offset: 0x00067075
		private IEnumerator StandUp()
		{
			Audio.Play("event:/char/oshiro/chat_get_up", this.Position);
			(this.Sprite as OshiroSprite).Pop("idle", false);
			yield return 0.25f;
			yield break;
		}

		// Token: 0x0600134A RID: 4938 RVA: 0x00068E84 File Offset: 0x00067084
		private IEnumerator Pace()
		{
			for (;;)
			{
				(this.Sprite as OshiroSprite).Wiggle();
				yield return this.PaceLeft();
				while (this.paceTimer < 2.266f)
				{
					yield return null;
				}
				this.paceTimer = 0f;
				(this.Sprite as OshiroSprite).Wiggle();
				yield return this.PaceRight();
				while (this.paceTimer < 2.266f)
				{
					yield return null;
				}
				this.paceTimer = 0f;
			}
			yield break;
		}

		// Token: 0x0600134B RID: 4939 RVA: 0x00068E93 File Offset: 0x00067093
		public IEnumerator PaceRight()
		{
			Vector2 homePosition = this.HomePosition;
			if ((this.Position - homePosition).Length() > 8f)
			{
				this.paceSfx.Play("event:/char/oshiro/move_04_pace_right", null, 0f);
			}
			yield return base.MoveTo(homePosition, false, null, false);
			yield break;
		}

		// Token: 0x0600134C RID: 4940 RVA: 0x00068EA2 File Offset: 0x000670A2
		public IEnumerator PaceLeft()
		{
			Vector2 vector = this.HomePosition + new Vector2(-20f, 0f);
			if ((this.Position - vector).Length() > 8f)
			{
				this.paceSfx.Play("event:/char/oshiro/move_04_pace_left", null, 0f);
			}
			yield return base.MoveTo(vector, false, null, false);
			yield break;
		}

		// Token: 0x0600134D RID: 4941 RVA: 0x00068EB4 File Offset: 0x000670B4
		public override void Update()
		{
			base.Update();
			this.paceTimer += Engine.DeltaTime;
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (this.sectionsComplete == 3 && !this.inRoutine && entity != null && entity.X < base.X + 32f && entity.Y <= base.Y)
			{
				this.OnTalk(entity);
				this.inRoutine = true;
			}
			if (this.sectionsComplete == 0 && !this.talked)
			{
				Level level = base.Scene as Level;
				if (entity != null && !entity.Dead)
				{
					float num = Calc.ClampedMap(Vector2.Distance(base.Center, entity.Center), 40f, 128f, 0f, 1f);
					level.Session.Audio.Music.Layer(1, num);
					level.Session.Audio.Music.Layer(2, 1f - num);
					level.Session.Audio.Apply(false);
					return;
				}
				level.Session.Audio.Music.Layer(1, true);
				level.Session.Audio.Music.Layer(2, false);
				level.Session.Audio.Apply(false);
			}
		}

		// Token: 0x04000F2F RID: 3887
		public const string TalkFlagsA = "oshiro_clutter_";

		// Token: 0x04000F30 RID: 3888
		public const string TalkFlagsB = "oshiro_clutter_optional_";

		// Token: 0x04000F31 RID: 3889
		public const string ClearedFlags = "oshiro_clutter_cleared_";

		// Token: 0x04000F32 RID: 3890
		public const string FinishedFlag = "oshiro_clutter_finished";

		// Token: 0x04000F33 RID: 3891
		public const string DoorOpenFlag = "oshiro_clutter_door_open";

		// Token: 0x04000F34 RID: 3892
		public Vector2 HomePosition;

		// Token: 0x04000F35 RID: 3893
		private int sectionsComplete;

		// Token: 0x04000F36 RID: 3894
		private bool talked;

		// Token: 0x04000F37 RID: 3895
		private bool inRoutine;

		// Token: 0x04000F38 RID: 3896
		private List<Vector2> nodes = new List<Vector2>();

		// Token: 0x04000F39 RID: 3897
		private Coroutine paceRoutine;

		// Token: 0x04000F3A RID: 3898
		private Coroutine talkRoutine;

		// Token: 0x04000F3B RID: 3899
		private SoundSource paceSfx;

		// Token: 0x04000F3C RID: 3900
		private float paceTimer;
	}
}
