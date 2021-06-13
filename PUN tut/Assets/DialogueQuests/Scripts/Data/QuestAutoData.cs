using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests
{
    /// <summary>
    /// Automated quests, means they will be completed automatically when conditions are true
    /// </summary>

    public class QuestAutoData : QuestData
    {
        [Header("Automated Quest")]
        public string region_start;
        public string region_end;

        public virtual void OnLoad() { }
        public virtual void Update() { } //This will only run while the quest is active

        public virtual void OnStart() { }
        public virtual void OnComplete() { }
        public virtual void OnFail() { }
        public virtual void OnCancel() { }

        public virtual bool AreStartConditionsMet(Actor player)
        {
            if (!string.IsNullOrWhiteSpace(region_start))
            {
                Region region = Region.Get(region_start);
                if (region != null && player != null)
                {
                    return region.IsInsideXZ(player.transform.position);
                }
                return false;
            }

            return false; //If region is empty, need to start manually
        }

        public virtual bool AreConditionsMet(Actor player)
        {
            if (!string.IsNullOrWhiteSpace(region_end))
            {
                Region region = Region.Get(region_end);
                if (region != null && player != null)
                {
                    return region.IsInsideXZ(player.transform.position);
                }
                return false;
            }

            return true; //If region is empty, quest will complete when other conditions are true
        }
    }

}
