using JetBrains.Annotations;
using System;

public enum ResourceID
{
    Item_Broadcast_Fever                        = 101,
    Item_Broadcast_FatigueReliever              = 102,
    Item_Broadcast_PlanPointBoost               = 103,
    Item_Broadcast_DesignPointBoost             = 104,
    Item_Broadcast_SoundPointBoost              = 105,
    Item_Broadcast_PromotionPointBoost          = 106,
    Stat_Broadcast_InterestPoint                = 201,
    Stat_Broadcast_QualityPoint                 = 202,
    Stat_Broadcast_SoundPoint                   = 203,
    Stat_Broadcast_EditingPoint                 = 204,

    Character_ISD_Ine                           = 1001,
    Character_ISD_Jingburger                    = 1002,
    Character_ISD_Lilpa                         = 1003,
    Character_ISD_Jururu                        = 1004,
    Character_ISD_Gosegu                        = 1005,
    Character_ISD_Viichan                       = 1006,

    Character_WAK_Wakgood                       = 2001,
    Character_WAK_Roentgenium                   = 2002,

    Character_Productor_Dulgi                   = 3001,
    Character_Productor_DdongGangAJi            = 3002,
    Character_Productor_PackJi                  = 3003,
    Character_Productor_JuPockDo                = 3004,
    Character_Productor_GyunNyangI              = 3005,
    Character_Productor_Rani                    = 3006,
    Character_Productor_Panchi                  = 3007,
    Character_Productor_MaeHyeong               = 3008,
    Character_Productor_Geomeori                = 3009,
    Character_Productor_Batman                  = 3010,
    Character_Productor_SakSakKimchi            = 3011,
    Character_Productor_Jentoo                  = 3012,
    Character_Productor_NinNin                  = 3013,
    Character_Productor_Bulgomo                 = 3014,
    Character_Productor_SuSaemi                 = 3015,
    Character_Productor_Dandap                  = 3016,
    Character_Productor_Tiffany                 = 3017,
    Character_Productor_KimchiMandoo            = 3018,
    Character_Productor_PungSin                 = 3019,
    Character_Productor_Gilbert                 = 3020,
    Character_Productor_Beiter                  = 3021,
    Character_Productor_DokkoHyeji              = 3022,
    Character_Productor_MechMenggisan           = 3023,
    Character_Productor_GwakChunSik             = 3024,
    Character_Productor_ButterusIII             = 3025,
    Character_Productor_Shallot                 = 3026,
    Character_Productor_JinHee                  = 3027,
    Character_Productor_CaptainSullivan         = 3028,
    Character_Productor_LeeDeokSoo              = 3029,
    Character_Productor_SirianRain              = 3030,
    Character_Productor_Gwonmin                 = 3031,
    Character_Productor_Dopamin                 = 3032,
    Character_Productor_Saeyong                 = 3033,
    Character_Productor_BujungMan               = 3034,
    Character_Productor_Wakpago                 = 3035,
    Character_Productor_CarnarJungtur           = 3036,
    Character_Productor_CallyCarly              = 3037,
    Character_Productor_AmadeusChoi             = 3038,
    Character_Productor_Secretto                = 3039,
    Character_Productor_Chouloky                = 3040,
    Character_Productor_BusinessKim             = 3041,
    Character_Productor_Sophia                  = 3042,
    Character_Productor_HIkiKing                = 3043,
    Character_Productor_Victory                 = 3044,
    Character_Productor_RUSUK                   = 3045,
    Character_Productor_HamIne                  = 3046,
    Character_Productor_BobBurger               = 3047,
    Character_Productor_Ddilpa                  = 3048,
    Character_Productor_GuiJokHee               = 3049,
    Character_Productor_BJPangE                 = 3050,
    Character_Productor_MangNyangNyang          = 3051,
} 

//위의 ResourceID + Type값 -> 리소스 데이터
//값이 너무 커져서 비트 플래그 연산으로 변경 필요
//bitarray 아니면 자체적으로 만들어서 쓰거나 할 듯
public enum ResourceType : long
{
    Item                                = 100,
    Stat                                = 200,
    ISD                                 = 1000,
    WAK                                 = 2000,
    Productor                           = 3000,
    TypeCount                           = 5,

    DefaultSprite                       = 100000,   
    StandingSprite                      = 100000,      
    SitFrontSprite                      = 200000,
    SitBackSprite                       = 300000,
    SitInteractiveSprite                = 400000,
    SpriteCount                         = 4,

    WalkAni                             = 1000000,
    FrontWorkAni                        = 2000000,
    BackWorkAni                         = 2100000,
    StandingIdleAni                     = 3000000,
    FrontIdleStretchingAni              = 4000000,
    FrontIdleLookAroundAni              = 4100000,
    BackIdleStretchingAni               = 4200000,
    BackIdleLookAroundAni               = 4300000,
    SitAni                              = 5000000,
    InteractiveAni                      = 6000000,         
    AniCount                            = 10,
}


public enum SitInteractiveResourceType : long
{
    SitFrontInteractiveLeft             = 400000,
    SitFrontInteractiveRight            = 410000,
    SitBackInteractiveLeft              = 420000,
    SitBackInteractiveRight             = 430000,
}

public enum ResourceFileName 
{   
    DefaultSprite, SitBack, SitFront, Standing, SitInteractive, //Sprite 
    BackIdleLookAroundAni, BackIdleStretchingAni, BackWorkAni, FrontIdleLookAroundAni, FrontIdleStretchingAni, FrontWorkAni, //Work Ani File
    WalkAni, StandingIdleAni, WorkAni, SitAni, InteractiveAni, //Ani Root File
};

public enum CommentType {ToIne, ToJingBurger, ToLilpa, ToJururu, ToGosegu, ToViichan, Count,};

// [Flags]
// public enum ResourceTypeBit
// {
//     Item                                = 1 << 2,
//     ISD                                 = 1 << 3,
//     WAK                                 = 1 << 4,
//     Productor                           = 1 << 5,
//     DefaultSprite                       = 1 << 6,         
//     SitSprite                           = 1 << 7,
//     WalkAni                             = 1 << 8,       
//     WorkAni                             = 1 << 9,
//     IdleAni                             = 1 << 10,
//     SitAni                              = 1 << 11,
// }


/// <summary>
/// Json파일의 데이터를 읽기위해 각 데이터테이블 구조를 작성.
/// </summary>
[System.Serializable]
public class DataTable
{
    public System.Collections.Generic.List<PlayerData> PlayerData;
    public System.Collections.Generic.List<ProductorData> ProductorData;
    public System.Collections.Generic.List<MatchingData> MatchingData;
    public System.Collections.Generic.List<ISDKeywordLevel> ISDKeywordLevel;
    public System.Collections.Generic.List<ReviewCommentData> ReviewComment;
    public System.Collections.Generic.List<EventData> Event;
    public System.Collections.Generic.List<Keyword> Keyword;
    public System.Collections.Generic.List<BroadcastRecord> BroadcastRecord;
    public System.Collections.Generic.List<CharacterData> CharacterData;
    public System.Collections.Generic.List<CharacterComment> CharacterComment;
    public System.Collections.Generic.List<GameHelpInfo> GameHelpInfo;
    public System.Collections.Generic.List<ResourcesTable> ResourcesTable;
}

//플레이어의 데이터
[System.Serializable]
public class PlayerData
{
    public string UserName;
    public string UserID;
    public int Money;
    public int TimeElapsed;
}

[System.Serializable]
public class ProductorData
{
    public long ID;
    public string Name;                         //작업자 이름
    public int IsFieldCharacter;                //필드 캐릭터인지 -> 이거는 나중에 없앨듯
    public string CharacterType;                //작업자 타입 (플래너, 그래픽, 사운드, 마케터)
    public int WorkPrice;                       //작업 비용
    public int OutsourcePrice;                  //외주 비용
    public int EmployPrice;                     //고용 비용
    public int PlannerStat;                     //플래너 스텟
    public int PlannerLevel;                    //플래너 레벨
    public int DesignStat;                      //디자이너 스텟
    public int DesignLevel;                     //디자이너 레벨
    public int SoundStat;                       //음향 스텟
    public int SoundLevel;                      //음향 레벨
    public int MarketerStat;                    //홍보 스텟
    public int MarketerLevel;                   //홍보 레벨
    public int MaxStemina;                      //최대 스테미나
    public string Info;                         //작업자 정보
    public string RecuritType;                  //고용 방법
    public string ProcessCompleteComment;       //작업 완료 대사

    public int SeatNum;                         //시트번호가 있을 경우 사내 멤버임. 0일경우 외주
}

[System.Serializable]
public class CharacterData
{
    public string Name;                 //이름
    public int IsIsegyeIdol;            //이세돌인지
    public int isFieldCharacter;        //필드에 나와있는 캐릭터인지
    public int SeatNumber;              //좌석 번호
    public int CharacterID;             //캐릭터 고유번호(저장 및 리소스 임포트에 사용)
    public int RemainFeverCount;        //저장 시 피버 상태였던 캐릭터의 남은 피버 수(남은 수 * 100 + 최대치로 저장)
                                        //(ex. 25번 피버 중 12번 피버를 했다 -> 1325)
}

[System.Serializable]
public class CharacterComment
{
    public long ID;
    public string GotoWork;
    public string LeaveWork;
    public string Signature;
    public string ToIne;
    public string ToJingBurger;
    public string ToLilpa;
    public string ToJururu;
    public string ToGosegu;
    public string ToViichan;
}

[System.Serializable]
public class GameHelpInfo
{
    public string Title;        //텝 이름
    public string Info;         //정보
}

[System.Serializable]
public class ResourcesTable
{
    public string Key;                          //리소스 키 값
    //public string Info;                       //정보
    public long ID;                              //리소스 ID 저장할 때 쓰는 값
    public long SpriteID;                       //Sprite ID
    public long SitFrontID;                     //정면 앉은 도트
    public long SitBackID;                      //뒷면 앉은 도트
    public long SitInteractiveID;               //앉을 때 interactive F
    public long WalkAniID;                      //걷는 ani
    public long FrontWorkAniID;                 //정면 일하는 ani
    public long BackWorkAniID;                  //뒷면 일하는 ani
    public long StandingIdleAniID;              //서있는 idle ani
    public long SitFrontIdleStretchingAniID;    //앉은 정면 기지개 ani
    public long SitFrontIdleLookAroundAniID;    //앉은 정면 두리번 ani
    public long SitBackIdleStretchingAniID;     //앉은 뒷면 기지개 ani
    public long SitBackIdleLookAroundAniID;     //앉은 뒷면 두리번 ani
    public long SittingAniID;                   //앉는 ani
    public long InteractiveAniID;               //상호작용 ani
}

//키워드의 매칭률
[System.Serializable]
public class MatchingData
{
    //컨텐츠이름과 각 카테고리와의 매칭률을 수치로 되어있음.
    //1~5

    public string Content;          //컨텐츠 이름
    //아래는 Kategorie와의 매칭률
    public int Game;
    public int Sports;
    public int Music;
    public int Event;
    public int VRChat;
    public int Life;
    public int Creative;
}

//이세돌분들 각 키워드들 숙련도
[System.Serializable]
public class ISDKeywordLevel
{
    public string Keyword;
    public int Ine;
    public int JingBurger;
    public int Lilpa;
    public int Jururu;
    public int Gosegu;
    public int Viichan;
}


[System.Serializable]
public class Keyword
{
    public string KeywordKey;       //enum타입으로 사용할 키워드 영어버전
    public string KoreanName;       //해당 영어 한국어 버전
    public string Type;             //컨텐츠인지 종류인지 
    public int Popularity;          //해당 키워드의 인기도
    public int Unlocked;         //해금된 조합인지(O, X)
    public int Price;               //가격 (장비 배율 적용 X)
}

//왁물원의 각 등급들의 리뷰 멘트들
[System.Serializable]
public class ReviewCommentData
{
    public string Comment;          //리뷰 멘트
    /// <summary>
    /// 1: 1~3점
    /// 2: 4~6점
    /// 3: 7~9점
    /// 4: 10점
    /// </summary>
    public int Point;               //리뷰 점수
    public string CafeRank;         //왁물원 카페랭크 (진드기, 닭둘기, 왁무새, 침팬치, 느그자)
}

//제작한 방송의 기록들 최대 20개
[System.Serializable]
public class BroadcastRecord
{
    public string BroadcastName;    //방송이름
    public int Viewer;              //조회 수
    public int Likes;               //좋아요 수
    public int PlannerStat;         //기획 수치
    public int MapStat;             //맵 제작 수치
    public int ComposStat;          //작곡 수치
    public int PromotionStat;       //홍보 수치
}

[System.Serializable]
public class EventData
{
    public string EventKey;         //이벤트 이름, 접근 시 사용
    public string EventComment;     //이벤트 멘트
    public string Achivement;       //도전과제
    public int Progressed;       //진행유무(O, X)
    public string Note;             //언락시기 ( ex) 튜토리얼 -> 초회한정)
    public int IsRepeat;         //반복유무(X -> 1회성 이벤트(튜토리얼))
}    

