using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if SURVIVAL_ENGINE
using SurvivalEngine;
#endif

#if FARMING_ENGINE
using FarmingEngine;
#endif

namespace DialogueQuests
{
    public enum NarrativeConditionType
    {
        None = 0,
        
        IsVisible = 10,
        InsideRegion = 12,
        CountSceneObjects = 15,

        EventTriggered = 20,

        ActorRelation=25,

        QuestStarted =30, //Either active or completed
        QuestActive = 32, //Started but not completed
        QuestCompleted =34, //Quest is completed
        QuestFailed =35, //Quest is failed
        QuestProgress = 37,

#if SURVIVAL_ENGINE || FARMING_ENGINE
        Day=50,
        WeekDay=51,
        DayTime=52,
        PlayerAttribute=65,
        HasItem=70,
        HasCrafted=74,
        HasKilled=76,
#endif

#if FARMING_ENGINE
        HasGold=80,
#endif

        RandomValue = 90,

        CustomInt = 91,
        CustomFloat = 92,
        CustomString = 93,

        CustomCondition = 99,
    }

    public enum NarrativeConditionOperator
    {
        Equal = 0,
        NotEqual = 1,
        GreaterEqual = 2,
        LessEqual = 3,
        Greater = 4,
        Less = 5,
    }

    public enum NarrativeConditionOperator2
    {
        IsTrue = 0,
        IsFalse = 1,
    }

    public enum NarrativeConditionTargetType
    {
        Value = 0,
        Target = 1,
    }
    
    public class NarrativeCondition : MonoBehaviour
    {
        public NarrativeConditionType type;
        public NarrativeConditionOperator oper;
        public NarrativeConditionOperator2 oper2;
        public NarrativeConditionTargetType target_type;
        public string target_id = "";
        public string other_target_id;
        public GameObject value_object;
        public ScriptableObject value_data;
        public CustomCondition value_custom;
        public int value_int = 0;
        public float value_float = 0f;
        public string value_string = "";

#if SURVIVAL_ENGINE || FARMING_ENGINE
        public AttributeType value_attribute;
#endif

        private void Start()
        {
            if (value_data != null)
            {
                if (value_data is ActorData)
                    ActorData.Load((ActorData)value_data);
                if (value_data is QuestData)
                    QuestData.Load((QuestData)value_data);
            }

            if (value_custom != null && !value_custom.inited)
            {
                value_custom.inited = true;
                value_custom.Start();
            }
        }

        public bool IsMet(Actor player)
        {
            bool condition_met = false;

            if (type == NarrativeConditionType.None)
            {
                condition_met = true;
                if (oper2 == NarrativeConditionOperator2.IsFalse)
                    condition_met = !condition_met;
            }

            if (type == NarrativeConditionType.CustomInt)
            {
                int i1 = NarrativeData.Get().GetCustomInt(target_id);
                int i2 = target_type == NarrativeConditionTargetType.Target ? NarrativeData.Get().GetCustomInt(other_target_id) : value_int;
                condition_met = CompareInt(oper, i1, i2);
            }

            if (type == NarrativeConditionType.CustomFloat)
            {
                float f1 = NarrativeData.Get().GetCustomFloat(target_id);
                float f2 = target_type == NarrativeConditionTargetType.Target ? NarrativeData.Get().GetCustomFloat(other_target_id) : value_float;
                condition_met = CompareFloat(oper, f1, f2);
            }

            if (type == NarrativeConditionType.CustomString)
            {
                string s1 = NarrativeData.Get().GetCustomString(target_id);
                string s2 = target_type == NarrativeConditionTargetType.Target ? NarrativeData.Get().GetCustomString(other_target_id) : value_string;
                condition_met = CompareString(oper, s1, s2);
            }

            if (type == NarrativeConditionType.IsVisible)
            {
                condition_met = (value_object != null && value_object.activeSelf);
                if (oper2 == NarrativeConditionOperator2.IsFalse)
                    condition_met = !condition_met;
            }
            
            if (type == NarrativeConditionType.InsideRegion)
            {
                ActorData adata = (ActorData)value_data;
                if (adata)
                {
                    Actor actor = Actor.Get(adata);
                    Region region = Region.Get(target_id);
                    if (actor && region)
                        condition_met = region.IsInsideXZ(actor.transform.position);
                }
                if (oper2 == NarrativeConditionOperator2.IsFalse)
                    condition_met = !condition_met;
            }

            if (type == NarrativeConditionType.CountSceneObjects)
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag(target_id);
                int i1 = objs.Length;
                int i2 = value_int;
                condition_met = CompareInt(oper, i1, i2);
            }

            if (type == NarrativeConditionType.QuestStarted)
            {
                QuestData quest = (QuestData)value_data;
                if (quest != null){
                    condition_met = NarrativeData.Get().IsQuestStarted(quest.quest_id);
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.QuestActive)
            {
                QuestData quest = (QuestData)value_data;
                if (quest != null)
                {
                    condition_met = NarrativeData.Get().IsQuestActive(quest.quest_id);
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.QuestCompleted)
            {
                QuestData quest = (QuestData)value_data;
                if (quest != null)
                {
                    condition_met = NarrativeData.Get().IsQuestCompleted(quest.quest_id);
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.QuestFailed)
            {
                QuestData quest = (QuestData)value_data;
                if (quest != null)
                {
                    condition_met = NarrativeData.Get().IsQuestFailed(quest.quest_id);
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.QuestProgress)
            {
                QuestData quest = (QuestData)value_data;
                if (quest != null)
                {
                    int avalue = NarrativeData.Get().GetQuestProgress(quest.quest_id, target_id);
                    condition_met = CompareInt(oper, avalue, value_int);
                }
            }

            if (type == NarrativeConditionType.EventTriggered)
            {
                GameObject targ = value_object;
                if (targ && targ.GetComponent<NarrativeEvent>())
                {
                    NarrativeEvent evt = targ.GetComponent<NarrativeEvent>();
                    condition_met = evt.GetTriggerCount() >= 1;
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.ActorRelation)
            {
                ActorData actor = (ActorData)value_data;
                if (actor != null)
                {
                    int avalue = NarrativeData.Get().GetActorValue(actor);
                    condition_met = CompareInt(oper, avalue, value_int);
                }
            }

#if SURVIVAL_ENGINE || FARMING_ENGINE
            if(type == NarrativeConditionType.Day){
                int i1 = PlayerData.Get().day;
                condition_met = CompareFloat(oper, i1, value_int);
            }

            if(type == NarrativeConditionType.WeekDay){
                int i1 = PlayerData.Get().day;
                i1 = (i1 + 6) % 7 + 1; //Return value between 1 and 7
                condition_met = CompareFloat(oper, i1, value_int);
            }

            if(type == NarrativeConditionType.DayTime){
                float f1 = PlayerData.Get().day_time;
                condition_met = CompareFloat(oper, f1, value_float);
            }

            if(type == NarrativeConditionType.PlayerAttribute){
                PlayerCharacter character = player.GetComponent<PlayerCharacter>();
                float f1 = character != null ? character.Attributes.GetAttributeValue(value_attribute) : 0f;
                condition_met = CompareFloat(oper, f1, value_float);
            }

            if(type == NarrativeConditionType.HasItem){
                PlayerCharacter character = player.GetComponent<PlayerCharacter>();
                ItemData item = (ItemData) value_data;
                int i1 = character != null && item != null ? character.Inventory.CountItem(item) : 0;
                condition_met = CompareInt(oper, i1, value_int);
            }

            if(type == NarrativeConditionType.HasCrafted){
                PlayerCharacter character = player.GetComponent<PlayerCharacter>();
                CraftData item = (CraftData) value_data;
                int i1 = character != null && item != null ? character.Crafting.CountTotalCrafted(item) : 0;
                condition_met = CompareInt(oper, i1, value_int);
            }

            if(type == NarrativeConditionType.HasKilled){
                PlayerCharacter character = player.GetComponent<PlayerCharacter>();
                CraftData item = (CraftData) value_data;
                int i1 = character != null && item != null ? character.Combat.CountTotalKilled(item) : 0;
                condition_met = CompareInt(oper, i1, value_int);
            }
#endif

#if FARMING_ENGINE
            if(type == NarrativeConditionType.HasGold){
                PlayerCharacter character = player.GetComponent<PlayerCharacter>();
                int i1 = character != null ? character.Data.gold : 0;
                condition_met = CompareInt(oper, i1, value_int);
            }
#endif

            if (type == NarrativeConditionType.RandomValue)
            {
                int value = NarrativeManager.Get().GetRandomValue(target_id);
                condition_met = CompareInt(oper, value, value_int);
            }

            if (type == NarrativeConditionType.CustomCondition)
            {
                if (value_custom != null)
                {
                    condition_met = value_custom.IsMet(player);
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            return condition_met;
        }

        public static bool CompareInt(NarrativeConditionOperator oper, int ival1, int ival2)
        {
            bool condition_met = true;
            if (oper == NarrativeConditionOperator.Equal && ival1 != ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.NotEqual && ival1 == ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.GreaterEqual && ival1 < ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.LessEqual && ival1 > ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Greater && ival1 <= ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Less && ival1 >= ival2)
            {
                condition_met = false;
            }
            return condition_met;
        }

        public static bool CompareFloat(NarrativeConditionOperator oper, float fval1, float fval2)
        {
            bool condition_met = true;
            if (oper == NarrativeConditionOperator.Equal && fval1 != fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.NotEqual && fval1 == fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.GreaterEqual && fval1 < fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.LessEqual && fval1 > fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Greater && fval1 <= fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Less && fval1 >= fval2)
            {
                condition_met = false;
            }
            return condition_met;
        }

        public static bool CompareString(NarrativeConditionOperator oper, string sval1, string sval2)
        {
            bool condition_met = true;
            if (oper == NarrativeConditionOperator.Equal && sval1 != sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.NotEqual && sval1 == sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.GreaterEqual && sval1 != sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.LessEqual && sval1 != sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Greater && sval1 == sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Less && sval1 == sval2)
            {
                condition_met = false;
            }
            return condition_met;
        }
    }

}
