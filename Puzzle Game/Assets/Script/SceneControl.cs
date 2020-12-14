using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// SceneControl.cs
public class SceneControl : MonoBehaviour
{
    private ScoreCounter score_counter= null;
    public enum STEP
    {
        NONE = -1, PLAY = 0, CLEAR, NUM,
    };// 상태정보없음, 플레이중, 클리어, 상태의종류(= 2).
    public STEP step = STEP.NONE; // 현재상태.
    public STEP next_step = STEP.NONE; // 다음상태.
    public float step_timer = 0.0f; // 경과시간.
    private float clear_time = 0.0f; // 클리어시간.
    public GUIStyle guistyle; // 폰트스타일.

    private BlockRoot block_root = null;

    void Start()
    {
        // BlockRoot 스크립트를 가져온다.
        this.block_root = this.gameObject.GetComponent<BlockRoot>();
        // BlockRoot 스크립트의 initialSetUp()을 호출한다.
        this.block_root.create();
        this.block_root.initialSetUp();
        this.score_counter = this.gameObject.GetComponent<ScoreCounter>(); // ScoreCounter가져오기
        this.next_step = STEP.PLAY; // 다음상태를'플레이중'으로.
        this.guistyle.fontSize = 24; //
    }

    void Update()
    {
        this.step_timer += Time.deltaTime;
        switch (this.step)
        {
            case STEP.CLEAR:
                if (Input.GetMouseButtonDown(0))
                {
                    SceneManager.LoadScene("TitleScene");
                }
                break;
        }

        if (this.next_step == STEP.NONE)
        {// 상태변화대기-----.
            switch (this.step)
            {
                case STEP.PLAY:
                    if (this.score_counter.isGameClear()) { this.next_step = STEP.CLEAR; } // 클리어조건을만족하면, 클리어상태로이행.
                    break;
            }
        }

        while (this.next_step != STEP.NONE)
        {// 상태가변화했다면------.
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.CLEAR:
                    this.block_root.enabled = false;// block_root를정지.
                    this.clear_time = this.step_timer;// 경과시간을클리어시간으로설정.
                    break;
            }
            this.step_timer = 0.0f;
        }
    }

    void OnGUI()
    {
        switch (this.step)
        {
            case STEP.PLAY:
                GUI.color = Color.black;
                // 경과시간을표시.
                GUI.Label(new Rect(40.0f, 10.0f, 200.0f, 20.0f), "시간" + Mathf.CeilToInt(this.step_timer).ToString() + "초", guistyle);
                GUI.color = Color.white;
                break;
            case STEP.CLEAR:
                GUI.color = Color.black;
                // 「☆클리어-！☆」라는문자열을표시.
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 20.0f, 200.0f, 20.0f), "☆클리어-!☆", guistyle);
                // 클리어시간을표시.
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 40.0f, 200.0f, 20.0f), "클리어시간" + Mathf.CeilToInt(this.clear_time).ToString()
                + "초", guistyle);
                GUI.color = Color.white;
                break;
        }
    }
}