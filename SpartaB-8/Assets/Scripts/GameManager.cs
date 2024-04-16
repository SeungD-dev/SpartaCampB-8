using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Card firstCard;
    public Card secondCard;
    public Text timeTxt;
    public Text matchTxt;
    public GameObject result;
    public Animator boardAnim;
    public int cardCount;
    private int _matchCount;

    public int best;
    public Text nowScore;
    public float getScore;    
    public Text bestScore;
    public string key = "bestScore";

    private const int MaxScore = 100;   // 획득 가능한 최대 점수
    private const int MaxTimes = 8; // 짝을 맞출 수 있는 최대 횟수

    public float time;

    private AudioSource _audioSource;
    public AudioClip clip;
    public AudioClip unMatch;

    private Alert _alert;
    private GameObject _obj;

    private bool _isPause;
    public bool animEnd = true;

    public GameObject hudTimeDecreaseTxt;

    public Text matchTimeTxt;
    float matchTime = 5.00f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (!(_obj = GameObject.Find("TimeTxt"))) return;
        if (!_obj.TryGetComponent(out _alert))
        {
            Debug.Log("GameManager.cs - Awake() - alert ÂüÁ¶ ½ÇÆÐ");
        }
        time = 60.0f;
    }


    private void Start()
    {
        Time.timeScale = 1.0f;
        _matchCount = 0;
        _audioSource = GetComponent<AudioSource>();
        _isPause = false;
        AudioManager.Instance.audioSource.Play();
        
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, 0);
        }
    }

    private void Update()
    {
        if(animEnd == false)
        {
            time -= Time.deltaTime;
            timeTxt.text = time.ToString("N2");

        }

        switch (time)
        {
            case > 0.0f and <= 5.0f:
                _alert.AlertTime();
                break;
            case <= 0.0f:
                GameOver();
                break;
        }

        MatchTime();
    }

    public void Matched()
    {
        if (firstCard.idx == secondCard.idx)
        {
            MatchTimeOver();
            _audioSource.PlayOneShot(clip);

            firstCard.DestroyCard();
            secondCard.DestroyCard();
            GetScore();
            cardCount -= 2;
            if (cardCount == 0)
            {
                GameOver();
            }
        }
        // 카드 매칭 실패 시
        else
        {
            MatchTimeOver();
            _audioSource.PlayOneShot(unMatch);

            Instantiate(hudTimeDecreaseTxt,
                GameObject.FindGameObjectWithTag("Canvas").transform); // 시간 감소 효과 텍스트 생성(미완성)
            // 제한시간 2초 감소
            if (time >= 2.0f)
            {
                time -= 2.0f;
            }
            else
            {
                time = 0.0f;
            }

            firstCard.CloseCard();
            secondCard.CloseCard();
        }

        _matchCount++;

        firstCard = null;
        secondCard = null;
    }

    private void GameOver()
    {
        Time.timeScale = 0.0f;
        matchTxt.text = "매칭시도: " + _matchCount;
        nowScore.text = "Score : " + getScore;

        if (PlayerPrefs.HasKey(key))
        {
            best = PlayerPrefs.GetInt(key);
            if (best < getScore)
            {
                PlayerPrefs.SetInt(key, (int)getScore);
                bestScore.text = "BEST : " + getScore;
            }
            else
            {
                bestScore.text = "BEST : " + getScore;
            }
        }
        else
        {
            PlayerPrefs.SetInt(key, (int)getScore);
            bestScore.text = "BEST : " + getScore;
        }

        PlayerPrefs.Save();
        result.SetActive(true);
        AudioManager.Instance.audioSource.Stop();
    } 

    public void Pause()
    {
        if (!_isPause)
        {
            Time.timeScale = 0.0f;
            _isPause = true;
        }
        else
        {
            Time.timeScale = 1.0f;
            _isPause = false;
        }
    }

    private void GetScore()
    {
        const float score = MaxScore / (float)MaxTimes; 
        switch (time)
        {
            case >= 30.0f:  // 60-30초
                getScore += score;
                break;
            case > 0.0f:    // 30-0초
                getScore += score - 5.0f;
                break;
            default:
                getScore = 0;
                break;
        }

        getScore -= _matchCount / (float)MaxTimes * 2.5f; // 매칭 횟수가 짝을 맞출 수 있는 최대 횟수 초과할 때마다 2.5점 감소
    }

    void MatchTime()
    {
        if (firstCard != null)
        {
            matchTimeTxt.gameObject.SetActive(true);

            matchTime -= Time.deltaTime;
            matchTimeTxt.text = matchTime.ToString("N2");

            if (matchTime <= 0.00f)
            {
                MatchTimeOver();

                firstCard.CloseCardInvoke();
                firstCard = null;
            }
        }
    }

    void MatchTimeOver()
    {        
        matchTimeTxt.gameObject.SetActive(false);
        matchTime = 5.00f;
    }
}