using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCotrol : MonoBehaviour
{
    public Image nextActionUI;
    public int monsterHp;
    public int damage;
    public Slider monsterHpSilder;
    public Animator animator;
    private bool actionCheker;
    private Vector3 targetPos;
    private float speed = 1.0f;

    public AudioSource audioSource;
    public AudioClip walkSound;
    public AudioClip hitSound;
    public AudioClip attackSound;


    public enum ENEMYSTATE
    {
        ATTACK1, ATTACK2, UP, DOWN, LEFT, RIGHT, NONE
    }

    ENEMYSTATE nowState;
    ENEMYSTATE preState;

    private void Start()
    {
        readyToAction();
        actionCheker = false;
        targetPos = this.transform.position;
    }

    private void Update()
    {
        float dis = Vector3.Distance(transform.position, targetPos);
        if (dis > 0)
        {
            transform.localPosition = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        else
        {
            this.transform.GetChild(0).transform.rotation = Quaternion.Euler(90, 0, 180);
            animator.SetBool("walkAni", false);
        }
        monsterHpSilder.value = monsterHp;
    }

    public void action()
    {
        nowState = preState;
        actionCheker = true;
        nowAction();

    }

    IEnumerator attack1()
    {
        animator.SetTrigger("AttackAni");
        //주변에 플레이어가 있는지 체크 그리고 있다면 데미지와 피해받는 모션까지
        //GameObject.Find("Player").GetComponent<MonsterCotrol>().OnHit(10);
        yield return new WaitForSeconds(2.0f);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (GameManager.instance.tile[i, j].state == STATE.MONSTER)
                {
                    if (i > 0 && GameManager.instance.tile[i - 1, j].state == STATE.PLAYER) ///down
                    {
                        GameObject.Find("Player").GetComponent<PlayerControl>().OnHit(damage);
                    }
                    else if (i < 2 && GameManager.instance.tile[i + 1, j].state == STATE.PLAYER) ///down
                    {
                        GameObject.Find("Player").GetComponent<PlayerControl>().OnHit(damage);
                    }
                    else if (j > 0 && GameManager.instance.tile[i, j -1 ].state == STATE.PLAYER) ///down
                    {
                        GameObject.Find("Player").GetComponent<PlayerControl>().OnHit(damage);
                    }
                    else if (j < 2 && GameManager.instance.tile[i , j + 1].state == STATE.PLAYER) ///down
                    {
                        GameObject.Find("Player").GetComponent<PlayerControl>().OnHit(damage);
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        audioSource.clip = attackSound;
        audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.enemyTurn = false;
        GameManager.instance.playerAniTime = false;
        readyToAction();
        GameObject.Find("Player").GetComponent<PlayerControl>().shield = 0;
    }

    IEnumerator attack2()
    {
        animator.SetTrigger("AttackAni");
        //주변에 플레이어가 있는지 체크 그리고 있다면 데미지와 피해받는 모션까지
        //GameObject.Find("Player").GetComponent<MonsterCotrol>().OnHit(10);
        yield return new WaitForSeconds(2.0f);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (GameManager.instance.tile[i, j].state == STATE.MONSTER)
                {
                    if (i > 0 && GameManager.instance.tile[i - 1, j].state == STATE.PLAYER)
                            GameObject.Find("Player").GetComponent<PlayerControl>().OnHit(damage);

                    if (i > 0 && j > 0 && GameManager.instance.tile[i - 1, j - 1].state == STATE.PLAYER)
                        GameObject.Find("Player").GetComponent<PlayerControl>().OnHit(damage);

                    if (i > 0 && j < 2 && GameManager.instance.tile[i - 1, j + 1].state == STATE.PLAYER)
                        GameObject.Find("Player").GetComponent<PlayerControl>().OnHit(damage);

                    if (i < 2 && GameManager.instance.tile[i + 1, j].state == STATE.PLAYER)
                        GameObject.Find("Player").GetComponent<PlayerControl>().OnHit(damage);

                    if (i < 2 && j > 0 &&  GameManager.instance.tile[i + 1, j - 1].state == STATE.PLAYER)
                        GameObject.Find("Player").GetComponent<PlayerControl>().OnHit(damage);
                    if (i < 2 && j < 2 && GameManager.instance.tile[i + 1, j + 1].state == STATE.PLAYER)
                        GameObject.Find("Player").GetComponent<PlayerControl>().OnHit(damage);


                    //좌, 우
                    if (j > 0)
                        GameManager.instance.tile[i, j - 1].obj.GetComponent<MeshRenderer>().material.color = Color.red;
                    if (j < 2)
                        GameManager.instance.tile[i, j + 1].obj.GetComponent<MeshRenderer>().material.color = Color.red;

                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        audioSource.clip = attackSound;
        audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.enemyTurn = false;
        GameManager.instance.playerAniTime = false;
        readyToAction();
        GameObject.Find("Player").GetComponent<PlayerControl>().shield = 0;
    }

    IEnumerator upAni()
    {
        animator.SetBool("walkAni", true);
        bool exitOuterLoop = false;
        for (int i = 1; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (GameManager.instance.tile[i, j].state == STATE.MONSTER
                    && GameManager.instance.tile[i - 1, j].state != STATE.PLAYER)
                {
                    exitOuterLoop = true;
                    GameManager.instance.tile[i - 1, j].state = STATE.MONSTER;
                    GameManager.instance.tile[i, j].state = STATE.EMPTY;
                    break;
                }
            }
            if (exitOuterLoop == true)
                break;
        }

        if (exitOuterLoop == true)
        {
            audioSource.clip = walkSound;
            audioSource.Play();
            this.transform.GetChild(0).transform.rotation = Quaternion.Euler(-90, 90, -90);
            targetPos = this.transform.position + (new Vector3(0, 4, 0));
        }

        yield return new WaitForSeconds(2.0f);
        GameManager.instance.enemyTurn = false;
        GameManager.instance.playerAniTime = false;
        readyToAction();
        GameObject.Find("Player").GetComponent<PlayerControl>().shield = 0;
    }


    IEnumerator downAni()
    {
        animator.SetBool("walkAni", true);
        bool exitOuterLoop = false;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (GameManager.instance.tile[i, j].state == STATE.MONSTER
                    && GameManager.instance.tile[i + 1, j].state != STATE.PLAYER)
                {
                    exitOuterLoop = true;
                    GameManager.instance.tile[i + 1, j].state = STATE.MONSTER;
                    GameManager.instance.tile[i, j].state = STATE.EMPTY;
                    break;
                }
            }
            if (exitOuterLoop == true)
                break;
        }

        if (exitOuterLoop == true)
        {
            audioSource.clip = walkSound;
            audioSource.Play();
            this.transform.GetChild(0).transform.rotation = Quaternion.Euler(90, -90, 90);
            targetPos = this.transform.position + (new Vector3(0, -4, -0));
        }

        yield return new WaitForSeconds(2.0f);
        GameManager.instance.enemyTurn = false;
        GameManager.instance.playerAniTime = false;
        readyToAction();
        GameObject.Find("Player").GetComponent<PlayerControl>().shield = 0;
    }


    IEnumerator leftAni()
    {
        animator.SetBool("walkAni", true);
        bool exitOuterLoop = false;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 1; j < 3; j++)
            {
                if (GameManager.instance.tile[i, j].state == STATE.MONSTER
                    && GameManager.instance.tile[i, j - 1].state != STATE.PLAYER)
                {
                    exitOuterLoop = true;
                    GameManager.instance.tile[i, j - 1].state = STATE.MONSTER;
                    GameManager.instance.tile[i, j].state = STATE.EMPTY;
                    break;
                }
            }
            if (exitOuterLoop == true)
                break;
        }

        if (exitOuterLoop == true)
        {
            audioSource.clip = walkSound;
            audioSource.Play();
            this.transform.GetChild(0).transform.rotation = Quaternion.Euler(0, -90, 90);
            targetPos = this.transform.position + (new Vector3(-4, 0, -0));
        }
        yield return new WaitForSeconds(2.0f);
        GameManager.instance.enemyTurn = false;
        GameManager.instance.playerAniTime = false;
        readyToAction();
        GameObject.Find("Player").GetComponent<PlayerControl>().shield = 0;
    }


    IEnumerator rightAni()
    {
        animator.SetBool("walkAni", true);
        bool exitOuterLoop = false;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (GameManager.instance.tile[i, j].state == STATE.MONSTER
                    && GameManager.instance.tile[i, j + 1].state != STATE.PLAYER)
                {
                    exitOuterLoop = true;
                    GameManager.instance.tile[i, j + 1].state = STATE.MONSTER;
                    GameManager.instance.tile[i, j].state = STATE.EMPTY;
                    break;
                }
            }
            if (exitOuterLoop == true)
                break;
        }

        if (exitOuterLoop == true)
        {
            audioSource.clip = walkSound;
            audioSource.Play();
            this.transform.GetChild(0).transform.rotation = Quaternion.Euler(180, -90, 90);
            targetPos = this.transform.position + (new Vector3(4, 0, 0));
        }
        yield return new WaitForSeconds(2.0f);
        GameManager.instance.enemyTurn = false;
        GameManager.instance.playerAniTime = false;
        readyToAction();
        GameObject.Find("Player").GetComponent<PlayerControl>().shield = 0;
    }

    private void readyToAction()
    {

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameManager.instance.tile[i, j].obj.GetComponent<MeshRenderer>().material.color = Color.green;
            }

        }


        int maxRand = 14;
        int attackSize = 11;
        int skillSize  = 0; //skill이 없음
        int noneSize = 11;

        if (GameManager.instance.stageLevel == 2)
        {
            maxRand = 12;
            attackSize = 10;
            skillSize = 15;
            noneSize = 20;
        }


        if (GameManager.instance.stageLevel == 3)
        {
            maxRand = 13;
            attackSize = 9;
            skillSize = 15;
            noneSize = 20;
        }


        int rand = Random.Range(0, maxRand);
        Debug.Log(rand);
        if (rand > 3 && rand < attackSize)
        {
            preState = ENEMYSTATE.ATTACK1;
            nextActionUI.sprite = Resources.Load<Sprite>("sword");
            bool exitOuterLoop = false;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (GameManager.instance.tile[i, j].state == STATE.MONSTER)
                    {
                        if(i>0)
                            GameManager.instance.tile[i - 1, j].obj.GetComponent<MeshRenderer>().material.color = Color.red;
                        if(i < 2)
                            GameManager.instance.tile[i + 1, j].obj.GetComponent<MeshRenderer>().material.color = Color.red;
                        if(j > 0)
                            GameManager.instance.tile[i , j - 1].obj.GetComponent<MeshRenderer>().material.color = Color.red;
                        if (j < 2)
                            GameManager.instance.tile[i , j + 1].obj.GetComponent<MeshRenderer>().material.color = Color.red;

                        Debug.Log(i);
                        Debug.Log(j);
                        exitOuterLoop = true;
                        break;
                    }
                }
                if (exitOuterLoop == true)
                    break;
            }



        }
        else if (rand < skillSize && rand >= attackSize)
        {
            preState = ENEMYSTATE.ATTACK2;
            nextActionUI.sprite = Resources.Load<Sprite>("bow");
            bool exitOuterLoop = false;
            //for (int i = 0; i < 3; i++)
            //{
            //    for (int j = 0; j < 3; j++)
            //    {
            //        if (GameManager.instance.tile[i, j].state == STATE.MONSTER)
            //        {
            //            if (i > 0)
            //                GameManager.instance.tile[i - 1, j].obj.GetComponent<MeshRenderer>().material.color = Color.red;
            //            if (i < 2)
            //                GameManager.instance.tile[i + 1, j].obj.GetComponent<MeshRenderer>().material.color = Color.red;
            //            if (j > 0)
            //                GameManager.instance.tile[i, j - 1].obj.GetComponent<MeshRenderer>().material.color = Color.red;
            //            if (j < 2)
            //                GameManager.instance.tile[i, j + 1].obj.GetComponent<MeshRenderer>().material.color = Color.red;

            //            Debug.Log(i);
            //            Debug.Log(j);
            //            exitOuterLoop = true;
            //            break;
            //        }
            //    }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (GameManager.instance.tile[i, j].state == STATE.MONSTER)
                    {
                        if (i > 0)
                            GameManager.instance.tile[i - 1, j].obj.GetComponent<MeshRenderer>().material.color = Color.red;
                        if (i > 0 && j > 0)
                            GameManager.instance.tile[i - 1, j-1].obj.GetComponent<MeshRenderer>().material.color = Color.red;
                        if (i > 0 && j < 2)
                            GameManager.instance.tile[i - 1, j +1].obj.GetComponent<MeshRenderer>().material.color = Color.red;


                        if (i < 2)
                            GameManager.instance.tile[i + 1, j].obj.GetComponent<MeshRenderer>().material.color = Color.red;
                        if (i < 2 && j > 0)
                            GameManager.instance.tile[i + 1, j-1].obj.GetComponent<MeshRenderer>().material.color = Color.red;
                        if (i < 2 && j < 2)
                            GameManager.instance.tile[i + 1, j+1].obj.GetComponent<MeshRenderer>().material.color = Color.red;


                        //좌, 우
                        if (j > 0)
                            GameManager.instance.tile[i, j - 1].obj.GetComponent<MeshRenderer>().material.color = Color.red;
                        if (j < 2)
                            GameManager.instance.tile[i, j + 1].obj.GetComponent<MeshRenderer>().material.color = Color.red;

                        Debug.Log(i);
                        Debug.Log(j);
                        exitOuterLoop = true;
                        break;
                    }
                }
                if (exitOuterLoop == true)
                    break;
            }
        }
        else if (rand > noneSize)
        {
            preState = ENEMYSTATE.NONE;
            nextActionUI.sprite = Resources.Load<Sprite>("none");
        }

        else if (rand == 3)
        {
            preState = ENEMYSTATE.UP;
            nextActionUI.sprite = Resources.Load<Sprite>("up");

            bool exitOuterLoop = false;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (GameManager.instance.tile[i, j].state == STATE.MONSTER)
                    {
                        if (i > 0)
                            GameManager.instance.tile[i - 1, j].obj.GetComponent<MeshRenderer>().material.color = Color.blue;
                        Debug.Log(i);
                        Debug.Log(j);
                        exitOuterLoop = true;
                        break;
                    }
                }
                if (exitOuterLoop == true)
                    break;
            }
        }
        else if (rand == 2)
        {
            preState = ENEMYSTATE.DOWN;
            nextActionUI.sprite = Resources.Load<Sprite>("down");

            bool exitOuterLoop = false;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (GameManager.instance.tile[i, j].state == STATE.MONSTER)
                    {
                        if (i < 2)
                            GameManager.instance.tile[i + 1, j].obj.GetComponent<MeshRenderer>().material.color = Color.blue;
                        Debug.Log(i);
                        Debug.Log(j);
                        exitOuterLoop = true;
                        break;
                    }
                }
                if (exitOuterLoop == true)
                    break;
            }
        }
        else if (rand == 1)
        {
            preState = ENEMYSTATE.LEFT;
            nextActionUI.sprite = Resources.Load<Sprite>("left");
            bool exitOuterLoop = false;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (GameManager.instance.tile[i, j].state == STATE.MONSTER)
                    {
                        if (j > 0)
                            GameManager.instance.tile[i, j - 1].obj.GetComponent<MeshRenderer>().material.color = Color.blue;

                        Debug.Log(i);
                        Debug.Log(j);
                        exitOuterLoop = true;
                        break;
                    }
                }
                if (exitOuterLoop == true)
                    break;
            }
        }
        else if (rand == 0)
        {
            preState = ENEMYSTATE.RIGHT;
            nextActionUI.sprite = Resources.Load<Sprite>("right");

            bool exitOuterLoop = false;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (GameManager.instance.tile[i, j].state == STATE.MONSTER)
                    {
                        if (j < 2)
                            GameManager.instance.tile[i, j + 1].obj.GetComponent<MeshRenderer>().material.color = Color.blue;

                        Debug.Log(i);
                        Debug.Log(j);
                        exitOuterLoop = true;
                        break;
                    }
                }
                if (exitOuterLoop == true)
                    break;
            }
        }

        nowState = ENEMYSTATE.NONE;
    }

    private void nowAction()
    {
        if (nowState == ENEMYSTATE.ATTACK1)
            StartCoroutine(attack1());
        else if (nowState == ENEMYSTATE.ATTACK2)
            StartCoroutine(attack2());
        else if (nowState == ENEMYSTATE.UP)
            StartCoroutine(upAni());
        else if (nowState == ENEMYSTATE.DOWN)
            StartCoroutine(downAni());
        else if (nowState == ENEMYSTATE.LEFT)
            StartCoroutine(leftAni());
        else if (nowState == ENEMYSTATE.RIGHT)
            StartCoroutine(rightAni());
        else if (nowState == ENEMYSTATE.NONE)
            StartCoroutine(noneAni());
    }


    public void OnHit(int damage)
    {
        StartCoroutine(hitAni(damage));
    }

    IEnumerator noneAni()
    {
        yield return new WaitForSeconds(2.0f);
        GameManager.instance.enemyTurn = false;
        GameManager.instance.playerAniTime = false;
        readyToAction();
        GameObject.Find("Player").GetComponent<PlayerControl>().shield = 0;
    }

    IEnumerator hitAni(int damage)
    {
        animator.SetTrigger("HitAni");
        yield return new WaitForSeconds(0.5f);
        audioSource.clip = hitSound;
        audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        monsterHp -= damage;
    }
}
