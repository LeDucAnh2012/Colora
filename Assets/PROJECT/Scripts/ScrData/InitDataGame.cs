using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GoogleMobileAds.Ump.Api;
using System.Diagnostics.Eventing.Reader;
using I2.Loc;
using UnityEngine.Events;
using System.IO;

public class InitDataGame : MonoBehaviour
{
    public static InitDataGame instance;
    [ShowInInspector] private Dictionary<TypeTopic, List<ShapeInfo>> dicListShapeInfo = new Dictionary<TypeTopic, List<ShapeInfo>>();
    [ShowInInspector] private Dictionary<TypeTopic, List<ShapeInfo>> dicListShapeInfoNotDoneTopic = new Dictionary<TypeTopic, List<ShapeInfo>>();
    public List<DataDailyQuest> listDataDailyQuest = new List<DataDailyQuest>();
    [ShowInInspector] public TextureMetadataList metadataList = new TextureMetadataList();
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        StartCoroutine(ActionHelper.StartAction(() =>
        {
            LoadData();
            LoadDataNotDone();
        }, 1f));
        CheckDailyQuest();
        DataDIY.LoadMetadataList();

    }
    public void LoadData()
    {
        ClearDicShape();
        string level = "1,2,3,4,5,6,7,";// RemoteConfig.instance.allConfigData.SortTopic;
        var listSortTopic = ActionHelper.ConfigListTypeTopicFromString(level);

        foreach (var shape in DataAllShape.GetListTopicInfo())
        {
            //if (shape.typeUnlock == TypeUnlock.DailyQuest && !shape.IsUnlock)
            //    continue;

            dicListShapeInfo.Add(shape.typeTopic, shape.listShapeInfo);
        }

        var dicTMP = new Dictionary<TypeTopic, List<ShapeInfo>>();
        foreach (var topic in listSortTopic)
        {
            var list = CheckDoneAllShape(dicListShapeInfo[topic]);
            if (list.Count > 0)
            {
                dicListShapeInfo[topic].Reverse();
                dicTMP.Add(topic, dicListShapeInfo[topic]);
            }
        }

        //  dicTMP = dicTMP.OrderByDescending(kvp => kvp.Value.Count).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        dicListShapeInfo.Clear();
        dicListShapeInfo = dicTMP;

        //foreach (var list in dicListShapeInfo)
        //    list.Value.Reverse();
    }
    private void LoadDataNotDone()
    {
        foreach (var topic in dicListShapeInfo)
            dicListShapeInfoNotDoneTopic.Add(topic.Key, CheckDoneAllShape(dicListShapeInfo[topic.Key]));
    }
    private List<ShapeInfo> CheckDoneAllShape(List<ShapeInfo> list)
    {
        var l = new List<ShapeInfo>();

        foreach (var shape in list)
        {
            if (shape.StateDone != StateDone.Done)
                if (shape.typeUnlock == TypeUnlock.DailyQuest)
                {
                    if (shape.IsUnlock)
                        l.Add(shape);
                }
                else
                    l.Add(shape);
        }
        return l;
    }
    private void CheckDailyQuest()
    {
        if (VariableSystem.DayOfYear <= TimeManager.instance.GetDayOfYear())
        {
            VariableSystem.DayOfYear = TimeManager.instance.GetDayOfYear() + 1;
            // reset new day

            VariableSystem.IsResetRewardDailyQuest = true;

            VariableSystem.IsCollect = false;
            VariableSystem.IsCollectX2 = false;

            if (VariableSystem.CountReceivedDaily >= 6)
                VariableSystem.CountReceivedDaily = 0;
            else
                VariableSystem.CountReceivedDaily++;

        }
    }
    public void ClearDicShape()
    {
        dicListShapeInfo.Clear();
        dicListShapeInfo = new Dictionary<TypeTopic, List<ShapeInfo>>();
    }
    public Dictionary<TypeTopic, List<ShapeInfo>> GetDicListShapeInfo()
    {
        return dicListShapeInfo;
    }

    public void ClearDicNotDoneShape()
    {
        dicListShapeInfoNotDoneTopic.Clear();
        dicListShapeInfoNotDoneTopic = new Dictionary<TypeTopic, List<ShapeInfo>>();
    }

    public Dictionary<TypeTopic, List<ShapeInfo>> GetDicListShapeInfoNotDoneTopic()
    {
        return dicListShapeInfoNotDoneTopic;
    }
}

[System.Serializable]
public class DataDailyQuest
{
    private const string AMOUNT_QUEST = "AmountQuest";
    private const string COUNT_QUEST = "CountQuest";
    private const string QUEST_REWARD = "QuestReward";
    private const string IS_GET = "IsGetQuest";

    public int id;
    [TermsPopup] public string nameQuest;
    public int amountReward;
    public List<TypeBooster> listTypeBooster = new List<TypeBooster>();
    public List<TypeTopic> listTypeTopic = new List<TypeTopic>();
    public Sprite sprGiftReward;

    // [ShowInInspector]
    public int AmountQuest
    {
        get => PlayerPrefs.GetInt(AMOUNT_QUEST + "_" + id);
        set
        {
            PlayerPrefs.SetInt(AMOUNT_QUEST + "_" + id, value);
            PlayerPrefs.Save();
        }
    }
    //[ShowInInspector]
    public int AmountQuest_2
    {
        get => PlayerPrefs.GetInt(AMOUNT_QUEST + "_2_" + id);
        set
        {
            PlayerPrefs.SetInt(AMOUNT_QUEST + "_2_" + id, value);
            PlayerPrefs.Save();
        }
    }
    //  [ShowInInspector]
    public int CountFinishQuest
    {
        get => PlayerPrefs.GetInt(COUNT_QUEST + "_" + id, 0);
        set
        {
            PlayerPrefs.SetInt(COUNT_QUEST + "_" + id, value);
            PlayerPrefs.Save();
        }
    }
    //  [ShowInInspector]
    public bool IsGetQuest
    {
        get => PlayerPrefs.GetInt(IS_GET + "_" + id) == 1;
        set
        {
            PlayerPrefs.SetInt(IS_GET + "_" + id, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    // funct
    public string GetRewardQuest()
    {
        return PlayerPrefs.GetString(QUEST_REWARD + "_" + id);
    }
    public string GetTypeTopicQuest_6()
    {
        return PlayerPrefs.GetString(QUEST_REWARD + "_2_" + id);
    }
    public void SetTypeTopic(TypeTopic typeTopic)
    {
        for (int i = 0; i < AmountQuest; i++)
        {
            string str = PlayerPrefs.GetString(QUEST_REWARD + "_" + i + "_" + id, "null");
            if (str.Equals("null"))
            {
                listTypeTopic.Add(typeTopic);
                return;
            }
        }
    }
    public void SetDoneQuest()
    {
        Debug.Log(" id quest = " + id);
        CountFinishQuest = AmountQuest;
    }

    public void InitRewardQuest(UnityAction callback = null) // init type reward quest
    {

        //Random: Lv 24, 39, 45, 65, 75, 78
        if (id == 6)
        {
            var list = DataAllShape.GetListShapeInfo(TypeUnlock.DailyQuest, isUnlock: false);
            var isHasShape = list.Count > 0;
            if (!isHasShape)
            {
                amountReward = 10;

                var type = (TypeBooster)Random.Range(1, 4);
                TypeBooster _typeBooster;
                string _type = PlayerPrefs.GetString(QUEST_REWARD + "_" + id, type.ToString());
                System.Enum.TryParse(_type, out _typeBooster);
                listTypeBooster.Add(_typeBooster);
                PlayerPrefs.SetString(QUEST_REWARD + "_" + id, _typeBooster.ToString());
                sprGiftReward = DataAllShape.GetDataBooster(_typeBooster).sprBooster;

            }
            else
            {
                amountReward = 1;
                var shapeInfo = list[Random.Range(0, list.Count)];
                var idShape = shapeInfo.nameShape;

                if (!PlayerPrefs.HasKey(QUEST_REWARD + "_" + id))
                {
                    PlayerPrefs.SetString(QUEST_REWARD + "_" + id, idShape.ToString());
                    PlayerPrefs.SetString(QUEST_REWARD + "_2_" + id, shapeInfo.typeTopic.ToString());
                }
                else
                {
                    shapeInfo = DataAllShape.GetShapeInfo(int.Parse(PlayerPrefs.GetString(QUEST_REWARD + "_" + id)));
                }
                sprGiftReward = ActionHelper.Texture2DToSprite(shapeInfo.textureGray);// DataAllShape.GetDataTopic(shapeInfo.typeTopic).sprTopic;
            }
        }
        else
        {
            var type = (TypeBooster)Random.Range(1, 4);
            TypeBooster _typeBooster;
            string _type = PlayerPrefs.GetString(QUEST_REWARD + "_" + id, type.ToString());
            System.Enum.TryParse(_type, out _typeBooster);
            listTypeBooster.Add(_typeBooster);
            PlayerPrefs.SetString(QUEST_REWARD + "_" + id, _typeBooster.ToString());
            sprGiftReward = DataAllShape.GetDataBooster(_typeBooster).sprBooster;
        }
        if (id == 3)
        {
            for (int i = 0; i < AmountQuest; i++)
            {
                TypeTopic typeTopic = TypeTopic.Animal;
                string str = PlayerPrefs.GetString(QUEST_REWARD + "_" + i + "_" + id, "null");
                System.Enum.TryParse(str, out typeTopic);

                if (!str.Equals("null"))
                    listTypeTopic.Add(typeTopic);
            }
        }
        callback?.Invoke();
    }
    public void RandomAmountReward() // random amount quest
    {
        var typeMax = TypeTopic.Monster;
        var idTopic = (TypeTopic)Random.Range(2, (int)typeMax + 1);
        var dic = InitDataGame.instance.GetDicListShapeInfoNotDoneTopic();

        switch (id)
        {
            case 0:
                AmountQuest = Random.Range(1, 4);
                break;
            case 1:
                AmountQuest = Random.Range(100, 501);
                break;
            case 2:
                AmountQuest = Random.Range(5, 11);
                break;
            case 3:
                AmountQuest = Random.Range(2, 5);
                break;
            case 4:

                idTopic = TypeTopic.Animal;// (TypeTopic)Random.Range(2, (int)typeMax + 1);

                while (!dic.ContainsKey(idTopic))
                    idTopic = (TypeTopic)Random.Range(2, (int)typeMax + 1);

                if (dic[idTopic].Count < 5)
                    AmountQuest = dic[idTopic].Count;
                else
                    AmountQuest = Random.Range(1, 6);

                AmountQuest_2 = (int)idTopic;

                InitDataGame.instance.ClearDicNotDoneShape();
                break;
            case 5:
                AmountQuest = Random.Range(1, 4);
                break;
            case 6:// quest done all task ( 3 task )

                AmountQuest = 4; // 3 task random 1 ngay
                break;
        }
    }

    public void ResetReward()
    {
        for (int i = 0; i < 10; i++)
            PlayerPrefs.DeleteKey(QUEST_REWARD + "_" + i + "_" + id);

        PlayerPrefs.DeleteKey(QUEST_REWARD + "_" + id);
        PlayerPrefs.DeleteKey(COUNT_QUEST + "_" + id);
        PlayerPrefs.DeleteKey(AMOUNT_QUEST + "_" + id);
        PlayerPrefs.DeleteKey(AMOUNT_QUEST + "_2_" + id);
        PlayerPrefs.DeleteKey(IS_GET + "_" + id);

        RandomAmountReward();
        InitRewardQuest();
    }
}