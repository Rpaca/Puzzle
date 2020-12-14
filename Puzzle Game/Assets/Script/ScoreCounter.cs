using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public struct Count
    {// 점수관리용구조체
        public int ignite; // 연쇄수
        public int score; // 점수
        public int total_socre; // 합계점수
    };

    public Count last; // 마지막(이번) 점수
    public Count best; // 최고점수.
    public static int QUOTA_SCORE = 1000; // 클리어하는데필요한점수.
    public GUIStyle guistyle; // 폰트스타일.

    void Start()
    {
        this.last.ignite = 0;
        this.last.score = 0;
        this.last.total_socre = 0;
        this.guistyle.fontSize = 16;
    }

    void OnGUI()
    {// 화면에텍스트와이미지표시
        int x = 20;
        int y = 50;
        GUI.color = Color.black;
        this.print_value(x + 20, y, "연쇄카운트" , this.last.ignite);
        y += 30;
        this.print_value(x + 20, y, "가산스코어", this.last.score);
        y += 30;
        this.print_value(x + 20, y, "합계스코어", this.last.total_socre);
        y += 30;
    }
    // 지정된두개의데이터를두개의행에나눠표시.
    public void print_value(int x, int y, string label, int value)
    {
        GUI.Label(new Rect(x, y, 100, 20), label, guistyle);// label을표시.
        y += 15;
        GUI.Label(new Rect(x + 20, y, 100, 20), value.ToString(), guistyle);// 다음행에value를표시.
        y += 15;
    }
    // 연쇄횟수를가산
    public void addIgniteCount(int count)
    {
        this.last.ignite += count; // 연쇄수에count를합산.
        this.update_score(); // 점수계산.
    }
    // 연쇄횟수를리셋
    public void clearIgniteCount()
    {
        this.last.ignite = 0; // 연쇄횟수리셋.
    }
    // 더해야할점수를계산
    private void update_score()
    {
        this.last.score = this.last.ignite * 10; // 점수갱신.
    }
    // 합계점수를갱신
    public void updateTotalScore()
    {
        this.last.total_socre += this.last.score;
    }
    // 게임을클리어했는지판정(SceneControl에서사용)
    public bool isGameClear()
    {
        bool is_clear = false;
        // 현재합계점수가클리어기준보다크면.
        if (this.last.total_socre > QUOTA_SCORE)
        {
            is_clear = true;
        }
        return (is_clear);
    }
}
