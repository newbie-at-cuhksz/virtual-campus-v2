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
    public enum NarrativeEffectType
    {
        None = 0,

        Move = 5,
        Show = 10,
        Hide = 11,
        Spawn = 15,
        Destroy = 16,

        StartEvent = 20,
        StartEventIfMet = 21,

        ActorRelation=25,

        StartQuest = 30,
        CancelQuest = 31,
        CompleteQuest = 32,
        FailQuest = 33,
        QuestProgress=37,

        PlaySFX=40,
        PlayMusic=42,
        StopMusic=44,

#if SURVIVAL_ENGINE || FARMING_ENGINE
        Create=60,
        PlayerAttribute=65,
        GainXP=68,
        GainItem=70,
        RemoveItem=71,
#endif
#if FARMING_ENGINE
        GainGold=80,
        OpenShop=84,
        NextDay=85,
#endif

        RollRandomValue = 90,

        CustomInt = 91,
        CustomFloat = 92,
        CustomString = 93,

        Wait = 95,
        CustomEffect = 97,
        CallFunction = 99,
    }

    public enum NarrativeEffectOperator
    {
        Add = 0,
        Set = 1,
    }

    [System.Serializable]
    public class NarrativeEffectCallback : UnityEvent<int> { }

    public class NarrativeEffect : MonoBehaviour
    {

        public NarrativeEffectType type;
        public string target_id = "";
        public NarrativeEffectOperator oper;
        public GameObject value_object;
        public ScriptableObject value_data;
        public AudioClip value_audio;
        public CustomEffect value_custom;
        public int value_int = 0;
        public float value_float = 1f;
        public string value_string = "";

#if SURVIVAL_ENGINE || FARMING_ENGINE
        public AttributeType value_attribute;
#endif

        [SerializeField]
        public UnityEvent callfunc_evt;

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

        public void Trigger(Actor player)
        {
            if (type == NarrativeEffectType.CustomInt)
            {
                if(oper == NarrativeEffectOperator.Set)
                    NarrativeData.Get().SetCustomInt(target_id, value_int);

                if (oper == NarrativeEffectOperator.Add)
                {
                    int value = NarrativeData.Get().GetCustomInt(target_id);
                    NarrativeData.Get().SetCustomInt(target_id, value + value_int);
                }
            }

            if (type == NarrativeEffectType.CustomFloat)
            {
                if (oper == NarrativeEffectOperator.Set)
                    NarrativeData.Get().SetCustomFloat(target_id, value_float);

                if (oper == NarrativeEffectOperator.Add)
                {
                    float value = NarrativeData.Get().GetCustomFloat(target_id);
                    NarrativeData.Get().SetCustomFloat(target_id, value + value_float);
                }
            }

            if (type == NarrativeEffectType.CustomString)
            {
                NarrativeData.Get().SetCustomString(target_id, value_string);
            }

            if (type == NarrativeEffectType.Move)
            {
                if (value_object != null && !string.IsNullOrWhiteSpace(target_id))
                {
                    Region region = Region.Get(target_id);
                    if (region != null)
                    {
                        Vector3 pos = region.PickRandomPosition();
                        value_object.transform.position = pos;
                    }
                }
            }

            if (type == NarrativeEffectType.Show)
            {
                GameObject targ = value_object;
#if SURVIVAL_ENGINE || FARMING_ENGINE
                if(targ != null){
                    UniqueID uid = targ.GetComponent<UniqueID>();
                    if(uid != null)
                        uid.Show();
                }
#endif
                if (targ)
                    targ.SetActive(true);
            }

            if (type == NarrativeEffectType.Hide)
            {
                GameObject targ = value_object;
#if SURVIVAL_ENGINE || FARMING_ENGINE
                if(targ != null){
                    UniqueID uid = targ.GetComponent<UniqueID>();
                    if(uid != null)
                        uid.Hide();
                }
#endif
                if (targ)
                    targ.SetActive(false);
            }

            if (type == NarrativeEffectType.Spawn)
            {
                if (value_object != null && !string.IsNullOrWhiteSpace(target_id))
                {
                    Region region = Region.Get(target_id);
                    if (region != null)
                    {
                        Vector3 pos = region.PickRandomPosition();
                        Instantiate(value_object, pos, Quaternion.identity);
                    }
                }
            }

            if (type == NarrativeEffectType.Destroy)
            {
                GameObject targ = value_object;
#if SURVIVAL_ENGINE || FARMING_ENGINE
                Craftable craftable = targ.GetComponent<Craftable>();
                Selectable select = targ.GetComponent<Selectable>();
                if (craftable != null)
                    craftable.Destroy();
                else if (select != null)
                    select.Destroy();
                else
                    Destroy(targ);
#else
                Destroy(targ);
#endif
            }

            if (type == NarrativeEffectType.StartEvent)
            {
                NarrativeEvent nevent = value_object.GetComponent<NarrativeEvent>();
                if (nevent != null)
                {
                    nevent.TriggerImmediately(player);
                }
            }

            if (type == NarrativeEffectType.StartEventIfMet)
            {
                NarrativeEvent nevent = value_object.GetComponent<NarrativeEvent>();
                if (nevent != null)
                {
                    nevent.TriggerIfConditionsMet(player);
                }
            }

            if (type == NarrativeEffectType.ActorRelation)
            {
                ActorData actor = (ActorData)value_data;
                if (actor != null)
                {
                    if (oper == NarrativeEffectOperator.Set)
                        NarrativeData.Get().SetActorValue(actor, value_int);

                    if (oper == NarrativeEffectOperator.Add)
                    {
                        int value = NarrativeData.Get().GetActorValue(actor);
                        NarrativeData.Get().SetActorValue(actor, value + value_int);
                    }
                }
            }

            if (type == NarrativeEffectType.StartQuest)
            {
                QuestData quest = (QuestData)value_data;
                NarrativeManager.Get().StartQuest(quest);
            }

            if (type == NarrativeEffectType.CancelQuest)
            {
                QuestData quest = (QuestData) value_data;
                NarrativeManager.Get().CancelQuest(quest);
            }

            if (type == NarrativeEffectType.CompleteQuest)
            {
                QuestData quest = (QuestData) value_data;
                NarrativeManager.Get().CompleteQuest(quest);
            }

            if (type == NarrativeEffectType.FailQuest)
            {
                QuestData quest = (QuestData)value_data;
                NarrativeManager.Get().FailQuest(quest);
            }

            if (type == NarrativeEffectType.QuestProgress)
            {
                QuestData quest = (QuestData)value_data;
                if(oper == NarrativeEffectOperator.Add)
                    NarrativeManager.Get().AddQuestProgress(quest, target_id, value_int);
                else if(oper == NarrativeEffectOperator.Set)
                    NarrativeManager.Get().SetQuestProgress(quest, target_id, value_int);
            }

            if (type == NarrativeEffectType.PlaySFX)
            {
                NarrativeManager.Get().PlaySFX(value_string, value_audio, value_float);
            }

            if (type == NarrativeEffectType.PlayMusic)
            {
                NarrativeManager.Get().PlayMusic(value_string, value_audio, value_float);
            }

            if (type == NarrativeEffectType.StopMusic)
            {
                NarrativeManager.Get().StopMusic(value_string);
            }

#if SURVIVAL_ENGINE || FARMING_ENGINE
            if(type == NarrativeEffectType.Create){
               Region region = Region.Get(target_id);
               if(region != null)
                    CraftData.Create((CraftData)value_data, region.transform.position);
            }

            if(type == NarrativeEffectType.PlayerAttribute){
                PlayerCharacter character = player.GetComponent<PlayerCharacter>();
                if (character != null && oper == NarrativeEffectOperator.Set)
                    character.Attributes.SetAttribute(value_attribute, value_float);
                if (character != null && oper == NarrativeEffectOperator.Add)
                    character.Attributes.AddAttribute(value_attribute, value_float);
            }

            if(type == NarrativeEffectType.GainXP){
                PlayerCharacter character = player.GetComponent<PlayerCharacter>();
                 if (character != null)
                    character.Attributes.GainXP(value_int);
            }

            if(type == NarrativeEffectType.GainItem){
                PlayerCharacter character = player.GetComponent<PlayerCharacter>();
                if(character != null)
                    character.Inventory.GainItem((ItemData) value_data, value_int, character.transform.position);
            }

            if(type == NarrativeEffectType.RemoveItem){
                PlayerCharacter character = player.GetComponent<PlayerCharacter>();
                if(character != null)
                    character.Inventory.RemoveItem((ItemData) value_data, value_int);
            }
#endif

#if FARMING_ENGINE
            if(type == NarrativeEffectType.GainGold){
                PlayerCharacter character = player.GetComponent<PlayerCharacter>();
                if(character != null){
                    character.Data.gold += value_int;
                    character.Data.gold = Mathf.Max(character.Data.gold, 0);
                }
            }

            if(type == NarrativeEffectType.OpenShop){
                PlayerCharacter character = player.GetComponent<PlayerCharacter>();
                ShopNPC shop = value_object.GetComponent<ShopNPC>();
                if(character != null && shop != null){
                    shop.OpenShop(character);
                }
            }

            if(type == NarrativeEffectType.NextDay){
                TheGame.Get().TransitionToNextDay();
            }
#endif

            if (type == NarrativeEffectType.RollRandomValue)
            {
                NarrativeManager.Get().RollRandomValue(target_id, 1, value_int);
            }

            if (type == NarrativeEffectType.CustomEffect)
            {
                if (value_custom != null)
                {
                    value_custom.DoEffect(player);
                }
            }

            if (type == NarrativeEffectType.CallFunction)
            {
                if (callfunc_evt != null)
                {
                    callfunc_evt.Invoke();
                }
            }

        }

        public float GetWaitTime()
        {
            if (type == NarrativeEffectType.Wait)
            {
                return value_float;
            }
            return 0f;
        }
    }

}