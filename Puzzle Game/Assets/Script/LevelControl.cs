using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{ // 각레벨의레벨데이터를저장하는List 값
    public float[] probability; // 블록의출현빈도를저장하는배열.
    public float heat_time; // 연소시간.
    public LevelData()
    { // 생성자.
        this.probability = new float[(int)Block.COLOR.NORMAL_COLOR_NUM];// 블록의 종류수와 같은크기로 데이터 영역을 확보.
        for (int i = 0; i < (int)Block.COLOR.NORMAL_COLOR_NUM; i++)
        {// 모든종류의출현확률을우선균등하게해둔다.
            this.probability[i] = 1.0f / (float)Block.COLOR.NORMAL_COLOR_NUM;
        }
    }
    public void clear()
    {// 모든종류의출현확률을0으로리셋하는메소드.
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] = 0.0f;
        }
    }
    public void normalize()
    {// 모든종류의출현확률의합계를100%(=1.0)로하는메소드.
        float sum = 0.0f;
        for (int i = 0; i < this.probability.Length; i++)
        {// 출현확률의'임시합계값'을계산한다.
            sum += this.probability[i];
        }
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] /= sum;// 각각의출현확률을'임시합계값'으로나누면, 합계가100%(=1.0) 딱떨어진다.
            if (float.IsInfinity(this.probability[i]))
            {// 만약그값이무한대라면.
                this.clear(); // 모든확률을0으로리셋하고.
                this.probability[0] = 1.0f; // 최초의요소만1.0으로해둔다.
                break; // 그리고루프를빠져나간다.
            }
        }
    }
}

public class LevelControl : MonoBehaviour
{
    private List<LevelData> level_datas = null; // 각레벨의레벨데이터.
    private int select_level = 0; // 선택된레벨.
    public void initialize() { this.level_datas = new List<LevelData>(); }// List를초기화.

    public void loadLevelData(TextAsset level_data_text)
    { // 텍스트데이터를읽어와서그내용을해석하고데이터를보관
        string level_texts = level_data_text.text;// 텍스트데이터를문자열로서받아들인다.
        string[] lines = level_texts.Split('\n');// 개행코드'\'마다나누어, 문자열배열에집어넣는다.
         foreach (var line in lines)
        {// lines 안의각행에대하여차례로처리해가는루프.
            if (line == "")
            {// 행이비었으면.
                continue;
            } // 아래처리는하지않고루프의처음으로점프.
            string[] words = line.Split(); // 행내의워드를배열에저장.
            int n = 0;
            LevelData level_data = new LevelData();// LevelData형변수를작성, 여기에현재처리하는행의데이터를넣는다.
            foreach (var word in words)
            {// words내의각워드에대해서, 순서대로처리해가는루프.
                if (word.StartsWith("#")) { break; }// 워드의시작문자가#이면, 루프탈출
                if (word == "") { continue; }// 워드가비었으면, 루프시작으로점프.
                switch (n)
                {// 'n'의값을0,1,2,...6으로변화시켜감으로써일곱개항목을처리. 각워드를float값으로변환하고level_data에저장.
                    case 0: level_data.probability[(int)Block.COLOR.ATTACK] = float.Parse(word); break;
                    case 1: level_data.probability[(int)Block.COLOR.UP] = float.Parse(word); break;
                    case 2: level_data.probability[(int)Block.COLOR.DOWN] = float.Parse(word); break;
                    case 3: level_data.probability[(int)Block.COLOR.LEFT] = float.Parse(word); break;
                    case 4: level_data.probability[(int)Block.COLOR.RIGHT] = float.Parse(word); break;
                    case 5: level_data.probability[(int)Block.COLOR.SKILL] = float.Parse(word); break;
                    case 6: level_data.heat_time = float.Parse(word); break;
                }
                n++;
            }
            if (n >= 7)
            { // 8항목(이상)이제대로처리되었다면.
                level_data.normalize();// 출현확률의합계가정확히100%가되도록하고나서.
                this.level_datas.Add(level_data);// List 구조의level_datas에level_data를추가한다.
            }
            else
            { // 그렇지않으면(오류가능성이있다).
                if (n == 0)
                { // 1워드도처리하지않은경우는주석이므로, 문제없음. 아무것도하지않는다.
                }
                else
                { // 그이외라면오류.
                    Debug.LogError("[LevelData] Out of parameter.\n");// 데이터의개수가맞지않는다는오류메시지를표시.
                }
            }
        }
        // level_datas에데이터가하나도없으면.
        if (this.level_datas.Count == 0)
        {
            // 오류메시지를표시.
            Debug.LogError("[LevelData] Has no data.\n");
            // level_datas에LevelData를하나추가해둔다.
            this.level_datas.Add(new LevelData());
        }
    }

    public void selectLevel()
    { // 몇개의레벨패턴에서지금사용할패턴을선택
      // 0~패턴사이의값을임의로선택.
        this.select_level = Random.Range(0, this.level_datas.Count);
        Debug.Log("select level = " + this.select_level.ToString());
    }

    public LevelData getCurrentLevelData()
    { // 선택되어있는레벨패턴의레벨데이터를반환
      // 선택된패턴의레벨데이터를반환한다.
        return (this.level_datas[this.select_level]);
    }

    public float getVanishTime()
    { // 선택되어있는레벨패턴의연소시간을반환
      // 선택된패턴의연소시간을반환한다.
        return (this.level_datas[this.select_level].heat_time);
    }
}
