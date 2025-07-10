using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BroadcastKeyword;
using Devcat;
using UnityEngine;

public class DataManager : GenericSingleton<DataManager>
{
    public static string ResourcePath = Path.Combine(Application.dataPath, "Resources");

    private new void Awake()
    {
        base.Awake();

        HashInit();
    }

    public void SetReviewComment(Dictionary<int, Dictionary<int, string[]>> data)
    {
        List<ReviewCommentData> reviewList = JsonManager.Instance.GetReviewCommentData();


        //랭크간 멘트 통합 -> 구간별 멘트 변경
        //각 등급 멘트들의 수가 동일하기 때문에 숫자로 나눠서 판단
        //딕셔너리에는 전체 수 / 카페 등급 수로 반복

        int cafeRankCount = ValueCastTo<int>.From(BroadcastReviewManager.CafeRank.Count);       //리뷰값 미리 캐스팅
        int reviewPointSection = reviewList.Count / cafeRankCount;                              //전체리뷰 수 / 카페 랭크 수 = 랭크당 리뷰 수

        for (int i = 0; i < cafeRankCount; i++)
        {
            data.Add(i, new Dictionary<int, string[]>());

            for (int j = 0; j < reviewPointSection; j++)
            {
                int index = i * reviewPointSection + j;

                if (data[i].ContainsKey(reviewList[index].Point))
                {
                    data[i][reviewList[index].Point][1] = reviewList[index].Comment;
                }
                else
                {
                    data[i].Add(reviewList[index].Point, new string[2] { reviewList[index].Comment, null });
                }
            }
        }
    }

    public void SetMoney(ref long money)
    {
        int value = JsonManager.Instance.GetPlayerData().Money;

        if (value == 0)
        {
            money = 5000;
        }

        money = value;
    }

    public void SetDate(ref int date)
    {
        date = JsonManager.Instance.GetPlayerData().TimeElapsed;
    }

    public void SetProductorInfo(List<ProductorInfo> infoList)
    {
        List<ProductorData> datas = JsonManager.Instance.GetProductorData();

        foreach (var data in datas)
        {
            ProductorInfo newInfo = new ProductorInfo();

            newInfo.SetProductorData(data);

            infoList.Add(newInfo);
        }
    }

    public void SetMatchingValue(ref int[,] list)
    {
        List<MatchingData> matchingData = JsonManager.Instance.GetMatchingData();

        int index = 0;

        list = new int[ValueCastTo<int>.From(Content.Count), ValueCastTo<int>.From(Kategorie.Count)];

        foreach (var data in matchingData)
        {
            int contentIndex = ValueCastTo<int>.From(Content.LOL) + index;

            list[contentIndex, ValueCastTo<int>.From(Kategorie.Game)] = data.Game;
            list[contentIndex, ValueCastTo<int>.From(Kategorie.Sports)] = data.Sports;
            list[contentIndex, ValueCastTo<int>.From(Kategorie.Music)] = data.Music;
            list[contentIndex, ValueCastTo<int>.From(Kategorie.Event)] = data.Event;
            list[contentIndex, ValueCastTo<int>.From(Kategorie.VRChat)] = data.VRChat;
            list[contentIndex, ValueCastTo<int>.From(Kategorie.Life)] = data.Life;
            list[contentIndex, ValueCastTo<int>.From(Kategorie.Creative)] = data.Creative;
            list[contentIndex, ValueCastTo<int>.From(Kategorie.Game)] = data.Game;

            index++;
        }
    }

    public void SetDirectionComment()
    {

    }

    public List<Keyword> GetKeyword(bool isKategorie)
    {
        List<Keyword> list = new List<Keyword>();

        List<Keyword> keywords = JsonManager.Instance.GetKeyword();

        int index = isKategorie ? 0 : ValueCastTo<int>.From(Kategorie.Creative) + 1;
        int limit = isKategorie ? ValueCastTo<int>.From(Kategorie.Creative) + 1 : keywords.Count;

        while (index < limit)
        {
            if (keywords[index].Unlocked == 1)
            {
                list.Add(keywords[index]);
            }

            index++;
        }

        return list;
    }

    /// <summary>
    /// 여기부터 테스트용임.
    /// 컨텐츠와 방식의 조합 방법들을 테스트하기 위해서 구간 나눠둠
    /// 1. 비트연산         비트마다 기준을 잡고 계산하면 됨 -> 코사인보다는 쉬운데 기준을 잘 잡아줘야 함
    /// 2. 코사인 유사도    코사인 유사도 계산 특징상 벡터를 생성해야 하는데 방송과 키워드의 연관성을 지어서 문장을 벡터화 해야함 
    /// 
    /// 사람마다 기준이 달라서 대부분의 사람이 만족할만한 기준을 정해야 하는게 문제.
    /// 비트 + 피로도                        
    ///                  0 피로도 수치 표현할 예정이라 비트가 아니고 0부터 9까지 표현하는 걸로 따로 할 수도 있음  
    ///                  1 장시간 가능한지 5시간 기준으로 아니면 0 
    ///                  2 시참이 가능한 컨텐츠인가 아니면 0 (예로 노래는 참여 불가능, 토크는 같이 얘기하는 거니 가능)
    ///                  3 합방이 가능한 컨텐츠인가 아니면 0 (대부분 가능할 거)
    ///                  4 대결 구도가 성립이 되는가 아니면 0 (토크는 대결이 불가함)
    ///                  5 혼자 진행이 가능한 방식인가 아니면 0 (대결은 안되는 걸로)
    ///                  6 
    ///                  7 
    ///                  8 
    ///                  장시간하면 힘든지, 시참이 괜찮은지 
    ///                  
    /// 기준이 이상하다고 생각들고 모든 키워드에 해당할 만한 그런 기준을 잡아야 함.
    /// 1번의 경우 평소 하는 방송이고 어울리는 키워드인데 3점이 나옴
    /// 2번의 경우 어울리는건 2점 안 어울리는건 1점 나왔음 개인으로 노래를 많이 부르는데 이것도 기준이 너무 부실함
    /// 3번의 경우 어울리지 않는 키워드인데 노래+개인 키워드와 동점임.
    /// 
    /// 지금은 기준에 특정 방송방식의 기준이 들어있어서 안 맞음
    /// 
    /// 1.
    /// 게임             11111000
    /// 개인             11001000
    /// => 3점(어울리는 키워드)
    /// 
    /// 2.
    /// 노래             01001000
    /// 개인             11001000
    /// 대결             01110000
    /// => 어울리는건 2점, 안 어울리는 키워드는 1점 나옴
    /// 
    /// 3.
    /// 토크             11101000
    /// 대결             01110000  
    /// => 2점(어울리지 않는 키워드)
    /// </summary>

    // private float CosineSimilarity()
    // {
    //     int[] test1 = { 1, 2, 4 };
    //     int[] test2 = { 1, 2, 3 };

    //     //float result = Vector2.Dot(test1, test2, 0)


    // }

    // void Start()
    // {
    //     //TEST(test1, test2);
    // }

    static Dictionary<string, int> TextToVector(string[] words)
    {
        Dictionary<string, int> vector = new Dictionary<string, int>();

        foreach (string word in words)
        {
            if (vector.ContainsKey(word))
            {
                vector[word]++;
            }
            else
            {
                vector[word] = 1;
            }
        }
        return vector;
    }

    static float CosineSimilarity(Dictionary<string, int> vector1, Dictionary<string, int> vector2)
    {
        var intersection = vector1.Keys.Intersect(vector2.Keys);
        float numerator = intersection.Sum(key => vector1[key] * vector2[key]);

        float sum1 = vector1.Values.Sum(value => Mathf.Pow(value, 2));
        float sum2 = vector2.Values.Sum(value => Mathf.Pow(value, 2));

        float denominator = Mathf.Sqrt(sum1) * Mathf.Sqrt(sum2);

        if (denominator == 0)
        {
            return 0.0f;
        }
        else
        {
            return numerator / denominator;
        }
    }

    static void TEST(string[] word1, string[] word2)
    {
        // 단어를 벡터로 변환
        Dictionary<string, int> vector1 = TextToVector(word1);
        Dictionary<string, int> vector2 = TextToVector(word2);

        // 코사인 유사도 계산
        float similarity = CosineSimilarity(vector1, vector2);

        Debug.Log(similarity);
    }

    // private int[] ConvertStringToVector(string[] data)
    // {

    // }

    private static double GetCosineSimilarity(int[] a, int[] b)
    {
        double dataA = 0;
        double dataB = 0;
        double product = 0;


        for (int k = 0; k < 3; k++)
        {
            dataA += Mathf.Pow(a[k], 2);
            dataB += Mathf.Pow(b[k], 2);
            product += (a[k] * 1.0 * b[k]);
        }

        double dataAB = Mathf.Sqrt((float)(dataA * dataB));
        return product / dataAB;
    }




    ////////////////////////////////Resource Hash Table////////////////////////////

    [SerializeField] private Hashtable resourceHashTable;

    private Dictionary<int, Character> characterDataEachSeat;

    public Sprite GetSpriteFromID(ResourceID resourceID, ResourceType resourceType)
    {
        long key = ValueCastTo<long>.From(resourceID) + ValueCastTo<long>.From(resourceType);
        return GetSpriteFromIDNum(key);
    }

    public Sprite GetSpriteFromIDNum(long key)
    {
        Sprite temp = resourceHashTable[key] as Sprite;

        return temp;
    }

    public ISDKeywordLevel GetISDKeyworldLevel(int index)
    {
        List<ISDKeywordLevel> iSDKeywordLevels = JsonManager.Instance.GetISDKeywordLevel();

        return iSDKeywordLevels[index];
    }

    public AnimationClip GetAnimationClipFromID(ResourceID resourceID, ResourceType resourceType)
    {
        long key = ValueCastTo<long>.From(resourceID) + ValueCastTo<long>.From(resourceType);

        return resourceHashTable[key] as AnimationClip;
    }

    public string GetPathFromID(ResourceID resourceID, ResourceType resourceType)
    {
        string folderName = resourceType.ToString();
        string fileName = string.Format("{0}_{1}", folderName, resourceID.ToString());

        return Path.Combine(folderName, fileName);
    }

    //Resource파일 내부에 리소스들을 미리 캐싱함.
    private void HashInit()
    {
        //해시테이블 내부에 키, 밸류 형식으로 리소스 가져옴
        //Key -> ResourceID + ResourceType
        //현재는 공통 이미지이지만 추후 캐릭터 별 Sprite, sprite sheet가 있을 경우 해당 부분들 가져와야 함

        long key = ValueCastTo<long>.From(ResourceID.Character_ISD_Ine);


        List<ResourcesTable> resourcesTables = JsonManager.Instance.GetResourcesTable();
        List<CharacterData> characterDatas = JsonManager.Instance.GetCharacterData();

        resourceHashTable = new Hashtable();


        //Texture 2D로 들어오는데 Sprite 변환으로 받으면 될 듯.
        //주석처리된 부분들은 아직 애니메이션 파일들 추가를 안해서 못 읽어와서 주석 처리해둠.

        foreach (var data in resourcesTables)
        {
            resourceHashTable.Add(data.ID, data.Key);

            //item default sprite 
            if (data.Key.Contains(ResourceType.Item.ToString())
                || data.Key.Contains(ResourceType.Stat.ToString()))
            {
                if (!File.Exists(Path.Combine(ResourcePath, ResourceType.DefaultSprite.ToString(), data.Key)))
                {
                    resourceHashTable.Add(data.SpriteID, Resources.Load<Sprite>(Path.Combine(ResourceType.DefaultSprite.ToString(), data.Key)));
                }

                continue;
            }

            //Character
            //sprite
            if (!File.Exists(Path.Combine(ResourcePath, ResourceFileName.DefaultSprite.ToString(), ResourceFileName.Standing.ToString(), string.Format("{0}_{1}", ResourceType.StandingSprite.ToString(), data.Key))))
            {
                resourceHashTable.Add(data.SpriteID, Resources.Load<Sprite>(Path.Combine(ResourceFileName.DefaultSprite.ToString(),
                                                                                            ResourceFileName.Standing.ToString(),
                                                                                            string.Format("{0}_{1}", ResourceType.StandingSprite.ToString(), data.Key))));

                resourceHashTable.Add(data.SitBackID, Resources.Load<Sprite>(Path.Combine(ResourceFileName.DefaultSprite.ToString(),
                                                                                            ResourceFileName.SitBack.ToString(),
                                                                                            string.Format("{0}_{1}", ResourceType.SitBackSprite.ToString(), data.Key))));

                resourceHashTable.Add(data.SitFrontID, Resources.Load<Sprite>(Path.Combine(ResourceFileName.DefaultSprite.ToString(),
                                                                                            ResourceFileName.SitFront.ToString(),
                                                                                            string.Format("{0}_{1}", ResourceType.SitFrontSprite.ToString(), data.Key))));


                resourceHashTable.Add(data.ID + ValueCastTo<long>.From(SitInteractiveResourceType.SitFrontInteractiveLeft),
                                                                                            Resources.Load<Sprite>(Path.Combine(ResourceFileName.DefaultSprite.ToString(),
                                                                                            ResourceFileName.SitInteractive.ToString(),
                                                                                            SitInteractiveResourceType.SitFrontInteractiveLeft.ToString(),
                                                                                            string.Format("{0}_{1}", SitInteractiveResourceType.SitFrontInteractiveLeft.ToString(), data.Key))));
                resourceHashTable.Add(data.ID + ValueCastTo<long>.From(SitInteractiveResourceType.SitFrontInteractiveRight),
                                                                                            Resources.Load<Sprite>(Path.Combine(ResourceFileName.DefaultSprite.ToString(),
                                                                                            ResourceFileName.SitInteractive.ToString(),
                                                                                            SitInteractiveResourceType.SitFrontInteractiveRight.ToString(),
                                                                                            string.Format("{0}_{1}", SitInteractiveResourceType.SitFrontInteractiveRight.ToString(), data.Key))));
                resourceHashTable.Add(data.ID + ValueCastTo<long>.From(SitInteractiveResourceType.SitBackInteractiveLeft),
                                                                                            Resources.Load<Sprite>(Path.Combine(ResourceFileName.DefaultSprite.ToString(),
                                                                                            ResourceFileName.SitInteractive.ToString(),
                                                                                            SitInteractiveResourceType.SitBackInteractiveLeft.ToString(),
                                                                                            string.Format("{0}_{1}", SitInteractiveResourceType.SitBackInteractiveLeft.ToString(), data.Key))));
                resourceHashTable.Add(data.ID + ValueCastTo<long>.From(SitInteractiveResourceType.SitBackInteractiveRight),
                                                                                            Resources.Load<Sprite>(Path.Combine(ResourceFileName.DefaultSprite.ToString(),
                                                                                            ResourceFileName.SitInteractive.ToString(),
                                                                                            SitInteractiveResourceType.SitBackInteractiveRight.ToString(),
                                                                                            string.Format("{0}_{1}", SitInteractiveResourceType.SitBackInteractiveRight.ToString(), data.Key))));
            }

            //idle standing
            if (!File.Exists(Path.Combine(ResourcePath, ResourceFileName.StandingIdleAni.ToString(), string.Format("{0}_{1}", ResourceFileName.StandingIdleAni.ToString(), data.Key))))
            {
                resourceHashTable.Add(data.StandingIdleAniID, Resources.Load<AnimationClip>(Path.Combine(ResourceFileName.StandingIdleAni.ToString(), string.Format("{0}_{1}", ResourceFileName.StandingIdleAni.ToString(), data.Key))));
            }

            //walkani
            if (!File.Exists(Path.Combine(ResourcePath, ResourceFileName.WalkAni.ToString(), string.Format("{0}_{1}", ResourceFileName.WalkAni.ToString(), data.Key))))
            {
                resourceHashTable.Add(data.WalkAniID, Resources.Load<AnimationClip>(Path.Combine(ResourceFileName.WalkAni.ToString(), string.Format("{0}_{1}", ResourceFileName.WalkAni.ToString(), data.Key))));
            }

            //workani
            if (!File.Exists(Path.Combine(ResourcePath, ResourceFileName.WorkAni.ToString(), ResourceFileName.FrontWorkAni.ToString(), string.Format("{0}_{1}", ResourceType.FrontWorkAni.ToString(), data.Key))))
            {
                //back
                //lookaround
                resourceHashTable.Add(data.SitBackIdleLookAroundAniID, Resources.Load<AnimationClip>(Path.Combine(ResourceFileName.WorkAni.ToString(), ResourceFileName.BackIdleLookAroundAni.ToString(), string.Format("{0}_{1}", ResourceType.BackIdleLookAroundAni.ToString(), data.Key))));
                //stretching
                resourceHashTable.Add(data.SitBackIdleStretchingAniID, Resources.Load<AnimationClip>(Path.Combine(ResourceFileName.WorkAni.ToString(), ResourceFileName.BackIdleStretchingAni.ToString(), string.Format("{0}_{1}", ResourceType.BackIdleStretchingAni.ToString(), data.Key))));
                //work
                resourceHashTable.Add(data.BackWorkAniID, Resources.Load<AnimationClip>(Path.Combine(ResourceFileName.WorkAni.ToString(), ResourceFileName.BackWorkAni.ToString(), string.Format("{0}_{1}", ResourceType.BackWorkAni.ToString(), data.Key))));


                //front
                //lookaround
                resourceHashTable.Add(data.SitFrontIdleLookAroundAniID, Resources.Load<AnimationClip>(Path.Combine(ResourceFileName.WorkAni.ToString(), ResourceFileName.FrontIdleLookAroundAni.ToString(), string.Format("{0}_{1}", ResourceType.FrontIdleLookAroundAni.ToString(), data.Key))));
                //stretching
                resourceHashTable.Add(data.SitFrontIdleStretchingAniID, Resources.Load<AnimationClip>(Path.Combine(ResourceFileName.WorkAni.ToString(), ResourceFileName.FrontIdleStretchingAni.ToString(), string.Format("{0}_{1}", ResourceType.FrontIdleStretchingAni.ToString(), data.Key))));
                //work
                resourceHashTable.Add(data.FrontWorkAniID, Resources.Load<AnimationClip>(Path.Combine(ResourceFileName.WorkAni.ToString(), ResourceFileName.FrontWorkAni.ToString(), string.Format("{0}_{1}", ResourceType.FrontWorkAni.ToString(), data.Key))));
            }

            if (!File.Exists(Path.Combine(ResourcePath, ResourceFileName.InteractiveAni.ToString(), string.Format("{0}_{1}", ResourceType.InteractiveAni.ToString(), data.Key))))
            {
                resourceHashTable.Add(data.InteractiveAniID, Resources.Load<AnimationClip>(Path.Combine(ResourceFileName.InteractiveAni.ToString(), string.Format("{0}_{1}", ResourceType.InteractiveAni.ToString(), data.Key))));
            }

            // if (!File.Exists(Path.Combine(ResourcePath, ResourceFileName.SitAni.ToString(), string.Format("{0}_{1}", ResourceType.SitAni.ToString(), data.Key))))
            // {
            //     resourceHashTable.Add(data.SittingAniID, Resources.Load<AnimationClip>(Path.Combine(ResourceFileName.SitAni.ToString(), string.Format("{0}_{1}", ResourceType.SitAni.ToString(), data.Key))));
            // }
        }
    }

    private int CharacterID(int id)
    {
        //resourcetype 뒤에서부터 크기 확인하고 이상이면 빼서 리턴 다 확인해도 안빼면 그게 코드임

        return 1;
    }

    private string ConvertIDToResourceName(int key)
    {
        ResourceID resourceID = ValueCastTo<ResourceID>.From(key);

        return resourceID.ToString();
    }
}
