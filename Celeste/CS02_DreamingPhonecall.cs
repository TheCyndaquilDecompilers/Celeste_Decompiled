using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000281 RID: 641
	public class CS02_DreamingPhonecall : CutsceneEntity
	{
		// Token: 0x060013CF RID: 5071 RVA: 0x0006B75C File Offset: 0x0006995C
		public CS02_DreamingPhonecall(Player player) : base(false, false)
		{
			this.player = player;
		}

		// Token: 0x060013D0 RID: 5072 RVA: 0x0006B770 File Offset: 0x00069970
		public override void OnBegin(Level level)
		{
			this.payphone = base.Scene.Tracker.GetEntity<Payphone>();
			base.Add(new Coroutine(this.Cutscene(level), true));
			base.Add(this.ringtone = new SoundSource());
			this.ringtone.Position = this.payphone.Position;
		}

		// Token: 0x060013D1 RID: 5073 RVA: 0x0006B7D0 File Offset: 0x000699D0
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.Dashes = 1;
			yield return 0.3f;
			this.ringtone.Play("event:/game/02_old_site/sequence_phone_ring_loop", null, 0f);
			while (this.player.Light.Alpha > 0f)
			{
				this.player.Light.Alpha -= Engine.DeltaTime * 2f;
				yield return null;
			}
			yield return 3.2f;
			yield return this.player.DummyWalkTo(this.payphone.X - 24f, false, 1f, false);
			yield return 1.5f;
			this.player.Facing = Facings.Left;
			yield return 1.5f;
			this.player.Facing = Facings.Right;
			yield return 0.25f;
			yield return this.player.DummyWalkTo(this.payphone.X - 4f, false, 1f, false);
			yield return 1.5f;
			base.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate
			{
				this.ringtone.Param("end", 1f);
			}, 0.43f, true));
			this.player.Visible = false;
			Audio.Play("event:/game/02_old_site/sequence_phone_pickup", this.player.Position);
			yield return this.payphone.Sprite.PlayRoutine("pickUp", false);
			yield return 1f;
			if (level.Session.Area.Mode == AreaMode.Normal)
			{
				Audio.SetMusic("event:/music/lvl2/phone_loop", true, true);
			}
			this.payphone.Sprite.Play("talkPhone", false, false);
			yield return Textbox.Say("CH2_DREAM_PHONECALL", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.ShowShadowMadeline)
			});
			if (this.evil != null)
			{
				if (level.Session.Area.Mode == AreaMode.Normal)
				{
					Audio.SetMusic("event:/music/lvl2/phone_end", true, true);
				}
				this.evil.Vanish();
				this.evil = null;
				yield return 1f;
			}
			base.Add(new Coroutine(this.WireFalls(), true));
			this.payphone.Broken = true;
			level.Shake(0.2f);
			VertexLight light = new VertexLight(new Vector2(16f, -28f), Color.White, 0f, 32, 48);
			this.payphone.Add(light);
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, null, 2f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				light.Alpha = t.Eased;
			};
			base.Add(tween);
			Audio.Play("event:/game/02_old_site/sequence_phone_transform", this.payphone.Position);
			yield return this.payphone.Sprite.PlayRoutine("transform", false);
			yield return 0.4f;
			yield return this.payphone.Sprite.PlayRoutine("eat", false);
			this.payphone.Sprite.Play("monsterIdle", false, false);
			yield return 1.2f;
			level.EndCutscene();
			new FadeWipe(level, false, delegate()
			{
				this.EndCutscene(level, true);
			});
			yield break;
		}

		// Token: 0x060013D2 RID: 5074 RVA: 0x0006B7E6 File Offset: 0x000699E6
		private IEnumerator ShowShadowMadeline()
		{
			Payphone payphone = base.Scene.Tracker.GetEntity<Payphone>();
			Level level = base.Scene as Level;
			yield return level.ZoomTo(new Vector2(240f, 116f), 2f, 0.5f);
			this.evil = new BadelineDummy(payphone.Position + new Vector2(32f, -24f));
			this.evil.Appear(level, false);
			base.Scene.Add(this.evil);
			yield return 0.2f;
			payphone.Blink.X += 1f;
			yield return payphone.Sprite.PlayRoutine("jumpBack", false);
			yield return payphone.Sprite.PlayRoutine("scare", false);
			yield return 1.2f;
			yield break;
		}

		// Token: 0x060013D3 RID: 5075 RVA: 0x0006B7F5 File Offset: 0x000699F5
		private IEnumerator WireFalls()
		{
			yield return 0.5f;
			Wire wire = base.Scene.Entities.FindFirst<Wire>();
			Vector2 speed = Vector2.Zero;
			Level level = base.SceneAs<Level>();
			while (wire != null && wire.Curve.Begin.X < (float)level.Bounds.Right)
			{
				speed += new Vector2(0.7f, 1f) * 200f * Engine.DeltaTime;
				Wire wire2 = wire;
				wire2.Curve.Begin = wire2.Curve.Begin + speed * Engine.DeltaTime;
				yield return null;
			}
			yield break;
		}

		// Token: 0x060013D4 RID: 5076 RVA: 0x0006B804 File Offset: 0x00069A04
		public override void OnEnd(Level level)
		{
			Leader.StoreStrawberries(this.player.Leader);
			level.ResetZoom();
			level.Bloom.Base = 0f;
			level.Remove(this.player);
			level.UnloadLevel();
			level.Session.Dreaming = false;
			level.Session.Level = "end_0";
			level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float)level.Bounds.Left, (float)level.Bounds.Bottom)));
			level.Session.Audio.Music.Event = "event:/music/lvl2/awake";
			level.Session.Audio.Ambience.Event = "event:/env/amb/02_awake";
			level.LoadLevel(Player.IntroTypes.WakeUp, false);
			level.EndCutscene();
			Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
		}

		// Token: 0x04000F95 RID: 3989
		private BadelineDummy evil;

		// Token: 0x04000F96 RID: 3990
		private Player player;

		// Token: 0x04000F97 RID: 3991
		private Payphone payphone;

		// Token: 0x04000F98 RID: 3992
		private SoundSource ringtone;
	}
}
