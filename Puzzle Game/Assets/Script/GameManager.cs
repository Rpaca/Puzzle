using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum STATE
{
    EMPTY, MONSTER, PLAYER
}


public class Tile
{
    public STATE state;
    public Vector3 pos;
    public GameObject obj;

    public Tile()
    {
        state = STATE.EMPTY;
        pos = new Vector3(0, 0, 0);
        obj = null;
    }
}

public class GameManager : MonoBehaviour
{
    //GameManager 싱글톤 처리
    public static GameManager instance { get; set; }

    public Tile[ , ] tile = new Tile[3, 3];

    public bool playerTurn;
    public bool enemyTurn;
    public bool playerAniTime;
    public bool enemyAniTime;
    public GameObject player;
    public int stageLevel;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        playerTurn = true;
        enemyTurn = false;
        playerAniTime = false;
        enemyAniTime = false;

    }

    private void Start()
    {
        player = GameObject.Find("Player");
        int num = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                tile[i, j] = new Tile();
                tile[i, j].obj = GameObject.Find("Tile").transform.GetChild(num).gameObject;
                num++;
            }
        }
        tile[0, 1].state = STATE.MONSTER;
        tile[2, 1].state = STATE.PLAYER;
    }

    public Text[] counterText = new Text[6];

    public int attackPoint;
    public int skillPoint;
    public int moveFrontPoint;
    public int moveBackPoint;
    public int moveLeftPoint;
    public int moveRightPoint;

    public int attackChance;
    public int skillChance;
    public int upChance;
    public int downChance;
    public int leftChance;
    public int rightChance;

    public GameObject clearUI;
    public GameObject faitUI;
    public GameObject endigUI;

    private void Update()
    {
        if (GameObject.Find("Player").GetComponent<PlayerControl>().hp <= 0)
        {
            //실패
            faitUI.SetActive(true);
        }

        if (GameObject.Find("Monster").GetComponent<MonsterCotrol>().monsterHp <= 0)
        {
            //클리어
            if (stageLevel == 3)
            {
                endigUI.SetActive(true);
            }

            else
            {
                clearUI.SetActive(true);
            }
        }

        if (GameObject.Find("GameRoot").GetComponent<BlockRoot>().playerMoved)
        {
            // 블록포인트가 3이상이면 행동을 시키고 0으로 초기화
            if (attackPoint >= 3)
            {
                attackPoint = 0;
                attackChance++;
                GameObject.Find("GameRoot").GetComponent<AudioSource>().Play();
                counterText[0].text = attackChance.ToString();
            }
            if (skillPoint >= 3)
            {
                skillPoint = 0;
                skillChance++;
                GameObject.Find("GameRoot").GetComponent<AudioSource>().Play();
                counterText[1].text = skillChance.ToString();
            }
            if (moveFrontPoint >= 3)
            {
                moveFrontPoint = 0;
                upChance++;
                GameObject.Find("GameRoot").GetComponent<AudioSource>().Play();
                counterText[2].text = upChance.ToString();
            }
            if (moveBackPoint >= 3)
            {
                moveBackPoint = 0;
                downChance++;
                GameObject.Find("GameRoot").GetComponent<AudioSource>().Play();
                counterText[3].text = downChance.ToString();
            }
            if (moveLeftPoint >= 3)
            {
                moveLeftPoint = 0;
                leftChance++;
                GameObject.Find("GameRoot").GetComponent<AudioSource>().Play();
                counterText[4].text = leftChance.ToString();
            }
            if (moveRightPoint >= 3)
            {
                moveRightPoint = 0;
                rightChance++;
                GameObject.Find("GameRoot").GetComponent<AudioSource>().Play();
                counterText[5].text = rightChance.ToString();
            }
        }
    }
}
