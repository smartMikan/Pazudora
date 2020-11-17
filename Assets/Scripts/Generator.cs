using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ドロップ生成クラス
/// </summary>
public class Generator : MonoBehaviour
{
    //D=>ドロッププリハブ(dropPrefab),L=>行のプリハブ(HorizontalZonePrefab)
    public GameObject D, L;

    //最初に生成する
    void Start()
    {
        //5行6列のドロップを生成する
        for (int i = 0; i < 5; i++)
        {
            //Lineオブジェクトを生成する
            GameObject l = Instantiate(L) as GameObject;
            //盤面オブジェクトにアタッチ
            l.transform.SetParent(transform);
            //ローカルスケールを1とする
            l.transform.localScale = Vector3.one;

            //6個のドロップを生成する
            for (int j = 0; j < 6; j++)
            {
                //ドロップオブジェクトを生成する
                GameObject d = Instantiate(D) as GameObject;
                //Lineオブジェクトにアタッチする
                d.transform.SetParent(l.transform);
                //ランダムのドロップタイプを生成する
                int type = Random.Range(0, 6);
                //ドロップタイプを指定
                d.GetComponent<DropCnt>().Set(type);
                //ドロップ行座標(i行)を指定
                d.GetComponent<DropCnt>().ID1 = i;
                //ドロップ列座標(j列)を指定
                d.GetComponent<DropCnt>().ID2 = j;
                //盤面データにも座標とタイプを保存する
                GameObject.Find("D").GetComponent<Director>().Obj[i, j] = d;
                GameObject.Find("D").GetComponent<Director>().Field[i, j] = type;

            }
        }
    }

    
}
