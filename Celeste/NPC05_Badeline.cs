using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000224 RID: 548
	public class NPC05_Badeline : NPC
	{
		// Token: 0x0600119B RID: 4507 RVA: 0x00057394 File Offset: 0x00055594
		public NPC05_Badeline(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			this.nodes = data.NodesOffset(offset);
			base.Add(this.moveSfx = new SoundSource());
			base.Add(new TransitionListener
			{
				OnOut = delegate(float f)
				{
					if (this.shadow != null)
					{
						this.shadow.Hair.Alpha = 1f - Math.Min(1f, f * 2f);
						this.shadow.Sprite.Color = Color.White * this.shadow.Hair.Alpha;
						this.shadow.Light.Alpha = this.shadow.Hair.Alpha;
					}
				}
			});
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x000573F4 File Offset: 0x000555F4
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (base.Session.Level.Equals("c-00"))
			{
				if (!base.Session.GetLevelFlag("c-01"))
				{
					scene.Add(this.shadow = new BadelineDummy(this.Position));
					this.shadow.Depth = -1000000;
					base.Add(new Coroutine(this.FirstScene(), true));
				}
				else
				{
					base.RemoveSelf();
				}
			}
			else if (base.Session.Level.Equals("c-01"))
			{
				if (!base.Session.GetLevelFlag("c-01b"))
				{
					int num = 0;
					while (num < 4 && base.Session.GetFlag(CS05_Badeline.GetFlag(num)))
					{
						num++;
					}
					if (num >= 4)
					{
						base.RemoveSelf();
					}
					else
					{
						Vector2 position = this.Position;
						if (num > 0)
						{
							position = this.nodes[num - 1];
						}
						scene.Add(this.shadow = new BadelineDummy(position));
						this.shadow.Depth = -1000000;
						base.Add(new Coroutine(this.SecondScene(num), true));
					}
				}
				else
				{
					base.RemoveSelf();
				}
			}
			this.levelBounds = (scene as Level).Bounds;
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x0005753F File Offset: 0x0005573F
		private IEnumerator FirstScene()
		{
			this.shadow.Sprite.Scale.X = -1f;
			this.shadow.FloatSpeed = 150f;
			bool playerHasFallen = false;
			bool startedMusic = false;
			Player player = null;
			for (;;)
			{
				player = base.Scene.Tracker.GetEntity<Player>();
				if (player != null && player.Y > (float)(this.Level.Bounds.Top + 180) && !player.OnGround(1) && !playerHasFallen)
				{
					player.StateMachine.State = 20;
					playerHasFallen = true;
				}
				if (player != null && playerHasFallen && !startedMusic && player.OnGround(1))
				{
					this.Level.Session.Audio.Music.Event = "event:/music/lvl5/middle_temple";
					this.Level.Session.Audio.Apply(false);
					startedMusic = true;
				}
				if (player != null && player.X > base.X - 64f && player.Y > base.Y - 32f)
				{
					break;
				}
				yield return null;
			}
			this.MoveToNode(0, false);
			while (this.shadow.X < (float)(this.Level.Bounds.Right + 8))
			{
				yield return null;
				if (player.X > this.shadow.X - 24f)
				{
					this.shadow.X = player.X + 24f;
				}
			}
			base.Scene.Remove(this.shadow);
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x0005754E File Offset: 0x0005574E
		private IEnumerator SecondScene(int startIndex)
		{
			this.shadow.Sprite.Scale.X = -1f;
			this.shadow.FloatSpeed = 300f;
			this.shadow.FloatAccel = 400f;
			yield return 0.1f;
			int index = startIndex;
			while (index < this.nodes.Length)
			{
				Player player = base.Scene.Tracker.GetEntity<Player>();
				while (player == null || (player.Position - this.shadow.Position).Length() > 70f)
				{
					yield return null;
				}
				if (index < 4 && !base.Session.GetFlag(CS05_Badeline.GetFlag(index)))
				{
					CS05_Badeline cutscene = new CS05_Badeline(player, this, this.shadow, index);
					base.Scene.Add(cutscene);
					yield return null;
					while (cutscene.Scene != null)
					{
						yield return null;
					}
					int num = index;
					index = num + 1;
					cutscene = null;
				}
				player = null;
			}
			base.Tag |= Tags.TransitionUpdate;
			this.shadow.Tag |= Tags.TransitionUpdate;
			base.Scene.Remove(this.shadow);
			base.RemoveSelf();
			yield break;
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x00057564 File Offset: 0x00055764
		public void MoveToNode(int index, bool chatMove = true)
		{
			if (chatMove)
			{
				this.moveSfx.Play("event:/char/badeline/temple_move_chats", null, 0f);
			}
			else
			{
				SoundEmitter.Play("event:/char/badeline/temple_move_first", this, null);
			}
			Vector2 start = this.shadow.Position;
			Vector2 end = this.nodes[index];
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.5f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				this.shadow.Position = Vector2.Lerp(start, end, t.Eased);
				if (this.Scene.OnInterval(0.03f))
				{
					this.SceneAs<Level>().ParticlesFG.Emit(BadelineOldsite.P_Vanish, 2, this.shadow.Position + new Vector2(0f, -6f), Vector2.One * 2f);
				}
				if (t.Eased >= 0.1f && t.Eased <= 0.9f && this.Scene.OnInterval(0.05f))
				{
					TrailManager.Add(this.shadow, Color.Red, 0.5f, false, false);
				}
			};
			base.Add(tween);
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x000575FD File Offset: 0x000557FD
		public void SnapToNode(int index)
		{
			this.shadow.Position = this.nodes[index];
		}

		// Token: 0x04000D36 RID: 3382
		public const string FirstLevel = "c-00";

		// Token: 0x04000D37 RID: 3383
		public const string SecondLevel = "c-01";

		// Token: 0x04000D38 RID: 3384
		public const string ThirdLevel = "c-01b";

		// Token: 0x04000D39 RID: 3385
		private BadelineDummy shadow;

		// Token: 0x04000D3A RID: 3386
		private Vector2[] nodes;

		// Token: 0x04000D3B RID: 3387
		private Rectangle levelBounds;

		// Token: 0x04000D3C RID: 3388
		private SoundSource moveSfx;
	}
}
