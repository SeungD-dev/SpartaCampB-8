using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Card firstCard;
    public Card secondCard;
    public Text timeTxt;
    public GameObject endTxt;
    public Text matchTxt;
    public int cardCount;
    int matchCount = 0;

    private float _time = 10.00f;

    private AudioSource _audioSource;
    public AudioClip clip;

    private Alert _alert;
    private GameObject _obj;

    bool isPause;

    // public GameObject hudTimeDecreaseTxt;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (!(_obj = GameObject.Find("TimeTxt"))) return;
        if (!_obj.TryGetComponent(out _alert))
        {
            Debug.Log("GameManager.cs - Awake() - alert 참조실패");
        }
    }


    private void Start()
    {
        Time.timeScale = 1.0f;
        _audioSource = GetComponent<AudioSource>();
        isPause = false;
    }

    private void Update()
    {
        _time -= Time.deltaTime;
        timeTxt.text = _time.ToString("N2");

        switch (_time)
        {
            case > 0.0f and <= 5.0f:
                _alert.AlertTime();
                break;
            case <= 0.0f:
                GameOver();
                break;
        }
    }

    public void Matched()
    {
        if (firstCard.idx == secondCard.idx)
        {
            _audioSource.PlayOneShot(clip);

            firstCard.DestroyCard();
            secondCard.DestroyCard();

            cardCount -= 2;
            if (cardCount == 0)
            {
                GameOver();
            }
        }
        // 카드 매칭 실패 시
        else
        {
            // Instantiate(hudTimeDecreaseTxt, GameObject.FindGameObjectWithTag("Canvas").transform);  // 시간 감소 효과 텍스트 생성(미완성)
            _time -= 2.0f; // 제한시간 2초 감소
            firstCard.CloseCard();
            secondCard.CloseCard();
        }

        matchCount++;

        firstCard = null;
        secondCard = null;
    }

    private void GameOver()
    {
        Time.timeScale = 0.0f;
        matchTxt.text = "매칭 시도 : " + matchCount.ToString();
        endTxt.SetActive(true);
    }

   
    public void Pause()
    {
        if (!isPause)
        {
            Time.timeScale = 0.0f;
            isPause = true;
        }
        else
        {
            Time.timeScale = 1.0f;
            isPause = false;
        }
    }
}