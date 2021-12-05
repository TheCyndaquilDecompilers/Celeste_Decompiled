using System;
using System.Collections.Generic;

namespace Celeste
{
	// Token: 0x020002AA RID: 682
	public class CheckpointData
	{
		// Token: 0x06001521 RID: 5409 RVA: 0x00078ED8 File Offset: 0x000770D8
		public CheckpointData(string level, string name, PlayerInventory? inventory = null, bool dreaming = false, AudioState audioState = null)
		{
			this.Level = level;
			this.Name = name;
			this.Inventory = inventory;
			this.Dreaming = dreaming;
			this.AudioState = audioState;
			this.CoreMode = null;
			this.ColorGrade = null;
		}

		// Token: 0x04001114 RID: 4372
		public string Level;

		// Token: 0x04001115 RID: 4373
		public string Name;

		// Token: 0x04001116 RID: 4374
		public bool Dreaming;

		// Token: 0x04001117 RID: 4375
		public int Strawberries;

		// Token: 0x04001118 RID: 4376
		public string ColorGrade;

		// Token: 0x04001119 RID: 4377
		public PlayerInventory? Inventory;

		// Token: 0x0400111A RID: 4378
		public AudioState AudioState;

		// Token: 0x0400111B RID: 4379
		public HashSet<string> Flags;

		// Token: 0x0400111C RID: 4380
		public Session.CoreModes? CoreMode;
	}
}
