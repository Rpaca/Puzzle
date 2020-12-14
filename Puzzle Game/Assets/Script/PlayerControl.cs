using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{

    public Animator animator;
    public int hp;
    public int shield;
    public int damage;
    private int hitDamage;
    private float speed = 2;
    private Vector3 targetPos;

    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip skillSound;
    public AudioClip walkSound;
    
    public Slider hpSilder;
    public Slider shiledSlier;

    private void Start()
    {
        shield = 0;
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
            this.transform.GetChild(0).transform.rotation = Quaternion.Euler(-90, 0, 0);
            animator.SetBool("WalkAni", false);
        }

        shiledSlier.value = shield;
        hpSilder.value = hp;
    }

    public void attack()
    {
        StartCoroutine( attackAni());
    }

    IEnumerator attackAni()
    {
        GameManager.instance.playerAniTime = true;
        bool hitCheck = false;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (GameManager.instance.tile[i, j].state == STATE.PLAYER)
                {
                    if (i > 0 && GameManager.instance.tile[i - 1, j].state == STATE.MONSTER) ///down
                    {
                        hitCheck = true;
                    }
                    else if (i < 2&& GameManager.instance.tile[i + 1, j].state == STATE.MONSTER) ///down
                    {
                        this.transform.GetChild(0).transform.rotation = Quaternion.Euler(90, 90, -90);
                        hitCheck = true;
                    }
                    else if ( j > 0 && GameManager.instance.tile[i, j-1].state == STATE.MONSTER) ///left
                    {
                        this.transform.GetChild(0).transform.rotation = Quaternion.Euler(90, 90, -90);
                        hitCheck = true;
                    }
                    else if ( j < 2 && GameManager.instance.tile[i, j+1].state == STATE.MONSTER) ///right
                    {
                        this.transform.GetChild(0).transform.rotation = Quaternion.Euler(0, 90, -90);
                        hitCheck = true;
                    }
                }
            }
        }


        if (GameManager.instance.attackChance > 0 )
        {
            if (hitCheck == true)
            {
                animator.SetTrigger("AttackAni");
                yield return new WaitForSeconds(0.5f);
                audioSource.clip = attackSound;
                audioSource.Play();
                yield return new WaitForSeconds(0.5f);
                GameObject.Find("Monster").GetComponent<MonsterCotrol>().OnHit(damage);
            }
            yield return new WaitForSeconds(1f);
            GameManager.instance.attackChance -= 1;
            GameManager.instance.counterText[0].text = GameManager.instance.attackChance.ToString();
        }

        if (GameManager.instance.attackChance > 0)
            StartCoroutine(attackAni());

        else if(GameManager.instance.skillChance > 0)
            StartCoroutine(guadrAni());
        else if (GameManager.instance.upChance > 0)
            StartCoroutine(upAni());
        else if (GameManager.instance.downChance > 0)
            StartCoroutine(downAni());
        else if (GameManager.instance.leftChance > 0)
            StartCoroutine(leftAni());
        else if (GameManager.instance.rightChance > 0)
            StartCoroutine(rightAni());
        else
            GameManager.instance.enemyTurn = true;

    }


    IEnumerator guadrAni()
    {
        shield += 10;
        animator.SetTrigger("GuardAni");
        yield return new WaitForSeconds(1.0f);
        audioSource.clip = skillSound;
        audioSource.Play();
        yield return new WaitForSeconds(1.0f);
        GameManager.instance.skillChance--;
        GameManager.instance.counterText[1].text = GameManager.instance.skillChance.ToString();
        if (GameManager.instance.skillChance > 0)
            StartCoroutine(guadrAni());
        else if (GameManager.instance.upChance > 0)
            StartCoroutine(upAni());
        else if (GameManager.instance.downChance > 0)
            StartCoroutine(downAni());
        else if (GameManager.instance.leftChance > 0)
            StartCoroutine(leftAni());
        else if (GameManager.instance.rightChance > 0)
            StartCoroutine(rightAni());
        else
            GameManager.instance.enemyTurn = true;

    }

    IEnumerator upAni()
    {
        animator.SetBool("WalkAni", true);
        bool exitOuterLoop = false;
        //tile 확인
        for (int i = 1; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (GameManager.instance.tile[i, j].state == STATE.PLAYER
                    && GameManager.instance.tile[i - 1, j].state != STATE.MONSTER)
                {
                    exitOuterLoop = true;
                    GameManager.instance.tile[i - 1, j].state = STATE.PLAYER;
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
            targetPos = this.transform.position +(new Vector3(0, 4, 0));
        }


        yield return new WaitForSeconds(2.0f);
        GameManager.instance.upChance--;
        GameManager.instance.counterText[2].text = GameManager.instance.upChance.ToString();
        if (GameManager.instance.upChance > 0)
            StartCoroutine(upAni());
        else if (GameManager.instance.downChance > 0)
            StartCoroutine(downAni());
        else if (GameManager.instance.leftChance > 0)
            StartCoroutine(leftAni());
        else if (GameManager.instance.rightChance > 0)
            StartCoroutine(rightAni());
        else
            GameManager.instance.enemyTurn = true;

    }

    IEnumerator downAni()
    {
        animator.SetBool("WalkAni", true);
        bool exitOuterLoop = false;
        //tile 확인
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (GameManager.instance.tile[i, j].state == STATE.PLAYER
                    && GameManager.instance.tile[i + 1, j].state != STATE.MONSTER)
                {
                    exitOuterLoop = true;
                    GameManager.instance.tile[i + 1, j].state = STATE.PLAYER;
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
            this.transform.GetChild(0).transform.rotation = Quaternion.Euler(90, 90, -90);
            targetPos = this.transform.position + (new Vector3(0, -4, -0));
        }

        yield return new WaitForSeconds(2.0f);
        GameManager.instance.downChance--;
        GameManager.instance.counterText[3].text = GameManager.instance.downChance.ToString();
        if (GameManager.instance.downChance > 0)
            StartCoroutine(downAni());
        else if (GameManager.instance.leftChance > 0)
            StartCoroutine(leftAni());
        else if (GameManager.instance.rightChance > 0)
            StartCoroutine(rightAni());
        else
            GameManager.instance.enemyTurn = true;

    }

    IEnumerator leftAni()
    {
        animator.SetBool("WalkAni", true);
        bool exitOuterLoop = false;
        //tile 확인
        for (int i = 0; i < 3; i++)
        {
            for (int j = 1; j < 3; j++)
            {
                if (GameManager.instance.tile[i, j].state == STATE.PLAYER
                    && GameManager.instance.tile[i , j -1].state != STATE.MONSTER)
                {
                    exitOuterLoop = true;
                    GameManager.instance.tile[i , j - 1].state = STATE.PLAYER;
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
            this.transform.GetChild(0).transform.rotation = Quaternion.Euler(-180, 90, -90);
            targetPos = this.transform.position + (new Vector3(-4, 0, 0));
        }

        yield return new WaitForSeconds(2.0f);
        GameManager.instance.leftChance--;
        GameManager.instance.counterText[4].text = GameManager.instance.leftChance.ToString();
        if (GameManager.instance.leftChance > 0)
            StartCoroutine(leftAni());
        else if (GameManager.instance.rightChance > 0)
            StartCoroutine(rightAni());
        else
            GameManager.instance.enemyTurn = true;

    }

    IEnumerator rightAni()
    {
        animator.SetBool("WalkAni", true);
        bool exitOuterLoop = false;
        //tile 확인
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (GameManager.instance.tile[i, j].state == STATE.PLAYER
                    && GameManager.instance.tile[i, j + 1].state != STATE.MONSTER)
                {
                    exitOuterLoop = true;
                    GameManager.instance.tile[i, j + 1].state = STATE.PLAYER;
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
            this.transform.GetChild(0).transform.rotation = Quaternion.Euler(0, 90, -90);
            targetPos = this.transform.position + (new Vector3(4, 0, 0));
        }

        yield return new WaitForSeconds(2.0f);
        GameManager.instance.rightChance--;
        GameManager.instance.counterText[5].text = GameManager.instance.rightChance.ToString();
        if (GameManager.instance.rightChance > 0)
            StartCoroutine(rightAni());
        else
            GameManager.instance.enemyTurn = true;

    }

    public void OnHit(int damage)
    {
        StartCoroutine(hitAni(damage));
    }

    IEnumerator hitAni(int damage)
    {
        audioSource.clip = hitSound;
        audioSource.Play();
        animator.SetTrigger("GetHitAni");
        yield return new WaitForSeconds(1.0f);
        damage -= shield;
        if(damage >0)
            hp -= damage;
    }

}
