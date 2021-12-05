using System;
using System.Collections.Generic;

namespace Monocle
{
	// Token: 0x02000116 RID: 278
	public class TagLists
	{
		// Token: 0x060008BF RID: 2239 RVA: 0x000143C0 File Offset: 0x000125C0
		internal TagLists()
		{
			this.lists = new List<Entity>[BitTag.TotalTags];
			this.unsorted = new bool[BitTag.TotalTags];
			for (int i = 0; i < this.lists.Length; i++)
			{
				this.lists[i] = new List<Entity>();
			}
		}

		// Token: 0x170000CF RID: 207
		public List<Entity> this[int index]
		{
			get
			{
				return this.lists[index];
			}
		}

		// Token: 0x060008C1 RID: 2241 RVA: 0x0001441D File Offset: 0x0001261D
		internal void MarkUnsorted(int index)
		{
			this.areAnyUnsorted = true;
			this.unsorted[index] = true;
		}

		// Token: 0x060008C2 RID: 2242 RVA: 0x00014430 File Offset: 0x00012630
		internal void UpdateLists()
		{
			if (this.areAnyUnsorted)
			{
				for (int i = 0; i < this.lists.Length; i++)
				{
					if (this.unsorted[i])
					{
						this.lists[i].Sort(EntityList.CompareDepth);
						this.unsorted[i] = false;
					}
				}
				this.areAnyUnsorted = false;
			}
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x00014484 File Offset: 0x00012684
		internal void EntityAdded(Entity entity)
		{
			for (int i = 0; i < BitTag.TotalTags; i++)
			{
				if (entity.TagCheck(1 << i))
				{
					this[i].Add(entity);
					this.areAnyUnsorted = true;
					this.unsorted[i] = true;
				}
			}
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x000144CC File Offset: 0x000126CC
		internal void EntityRemoved(Entity entity)
		{
			for (int i = 0; i < BitTag.TotalTags; i++)
			{
				if (entity.TagCheck(1 << i))
				{
					this.lists[i].Remove(entity);
				}
			}
		}

		// Token: 0x040005D1 RID: 1489
		private List<Entity>[] lists;

		// Token: 0x040005D2 RID: 1490
		private bool[] unsorted;

		// Token: 0x040005D3 RID: 1491
		private bool areAnyUnsorted;
	}
}
