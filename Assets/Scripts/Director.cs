using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 盤面データを制御するクラス
/// </summary>
public class Director : MonoBehaviour
{

    //ドロップゲームオブジェクトを格納
    GameObject[,] o = new GameObject[5, 6];

    public GameObject[,] Obj { get => o; set => o = value; }

    //盤面上のドロップ種類を格納(6=空白)
    int[,] f = new int[5, 6];
    public int[,] Field { get => f; set => f = value; }


    /// <summary>
    /// 二つのドロップの距離が交換距離以内かどうかを測定
    /// </summary>
    /// <param name="pos1"> ドロップ1のスクリーン位置(screenPosition)</param>
    /// <param name="pos2"> ドロップ2のスクリーン位置(screenPosition)</param>
    /// <returns></returns>
    public bool CheckDistance(Vector2 pos1, Vector2 pos2)
    {
        return Vector2.Distance(pos1, pos2) < 93.75f;
    }
    
    //盤面上にいる二つのドロップを交換
    public void ChangePos(GameObject obj1, GameObject obj2)
    {
        //ドロップコントローラーを取得
        DropCnt d1 = obj1.GetComponent<DropCnt>();
        DropCnt d2 = obj2.GetComponent<DropCnt>();

        //交換データを格納
        GameObject tempObj;
        Vector2 tempPos;
        int temp;

        ////盤面情報を交換する:
        //盤面ドロップオブジェクト配列情報を交換する
        tempObj = Obj[d1.ID1, d1.ID2];
        Obj[d1.ID1, d1.ID2] = Obj[d2.ID1, d2.ID2];
        Obj[d2.ID1, d2.ID2] = tempObj;

        //盤面ドロップ種類配列情報を交換する
        temp = Field[d1.ID1, d1.ID2];
        Field[d1.ID1, d1.ID2] = Field[d2.ID1, d2.ID2];
        Field[d2.ID1, d2.ID2] = temp;
        ////


        ////ドロップコントローラーにいるドロップ自身の情報を交換する:
        //ローカル初期座標(ワールド空間)を交換する
        tempPos = d1.P1;
        d1.P1 = d2.P1;
        d2.P1 = tempPos;

        //ローカル初期スクリーン座標を交換する
        tempPos = d1.P2;
        d1.P2 = d2.P2;
        d2.P2 = tempPos;

        //盤面横目位置(Y行番)を交換する
        temp = d1.ID1;
        d1.ID1 = d2.ID1;
        d2.ID1 = temp;
        //盤面縦目位置(X列番)を交換する
        temp = d1.ID2;
        d1.ID2 = d2.ID2;
        d2.ID2 = temp;
    }

    /// <summary>
    /// 盤面上にいる3目並び以上のドロップを検出、消す(ドロップ種類を6に変える)
    /// </summary>
    public void DeleteDrop()
    {
        //c=>今検出した並び目数,t=>ドロップ種類ID(type)
        int c = 0, t = 0;

        //横向き並び目数盤面データ
        int[,] temp = new int[5, 6];
        //縦向き並び目数盤面データ
        int[,] temp2 = new int[5, 6];

        //横向きチェック
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                //行頭で並び目数をリセット
                if (j == 0)
                {
                    c = 1;
                    t = Field[i, j];
                    continue;
                }

                //次のドロップタイプは今検出したタイプの場合
                if (t == Field[i, j])
                {
                    //並び目数を1個増やす
                    c++;
                    //3並び目以上の場合
                    if (c >= 3)
                    {
                        //横向き並び目数情報を格納する(今i行j列にいるドロップは3並び目以上になったこと)
                        temp[i, j] = c;
                    }
                }
                //タイプが違う場合
                else
                {
                    //並び目数と検出タイプをリセット
                    c = 1;
                    t = Field[i, j];
                }
            }
        }
        //縦向きチェック(上から下へ)
        for (int j = 0; j < 6; j++)
        {
            for (int i = 0; i < 5; i++)
            {
                //一列目でリセット
                if (i == 0)
                {
                    c = 1;
                    t = Field[i, j];
                    continue;
                }
                //次のドロップタイプは今検出したタイプの場合
                if (t == Field[i, j])
                {
                    //並び目数を1個増やす
                    c++;
                    //3並び目以上の場合
                    if (c >= 3)
                    {
                        //縦向き並び目数情報を格納する(今i行j列にいるドロップは3並び目以上になったこと)
                        temp2[i, j] = c;
                    }
                }
                //タイプが違う場合
                else
                {
                    //並び目数と検出タイプをリセット
                    c = 1;
                    t = Field[i, j];
                }
            }
        }


        //ドロップを消す
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                //横向き消す
                //もしこのマスに保存した並び目数データが3以上の場合
                if (temp[i, j] >= 3)
                {
                    //今のマスから、左向きに、並び目数分のマスにいるドロップを、6番(空白タイプ)を変更する
                    for (int k = j; temp[i, j] > 0; k--, temp[i, j]--)
                    {
                        Field[i, k] = 6;
                        Obj[i, k].GetComponent<DropCnt>().Set(6);

                    }
                }
                //縦向き消す
                //もしこのマスに保存した並び目数データが3以上の場合
                if (temp2[i, j] >= 3)
                {
                    //今のマスから、上向きに、並び目数分のマスにいるドロップを、6番(空白タイプ)を変更する
                    for (int k = i; temp2[i, j] > 0; k--, temp2[i, j]--)
                    {
                        Field[k, j] = 6;
                        Obj[k, j].GetComponent<DropCnt>().Set(6);
                    }
                }
            }
        }
    }

    /// <summary>
    /// ドロップを落下させる
    /// </summary>
    public void DownDrop()
    {
        //縦向きチェック(上から下へ)
        for (int j = 0; j < 6; j++)
        {
            for (int i = 1; i < 5; i++)
            {
                //空白ドロップの場合
                if (Field[i, j] == 6)
                {
                    //空白ドロップのマスから、上へ
                    for (int k = i; k > 0; k--)
                    {
                        //このドロップのと上のマスにいるドロップのと交換する(最終的に、空白ドロップは一番上に行く)
                        ChangePos(Obj[k, j], Obj[k - 1, j]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 盤面にいる空白ドロップをランダムタイプのドロップに変換させる
    /// </summary>
    public void ResetDrop()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (Field[i,j]==6)
                {
                    int type = Random.Range(0, 6);
                    Field[i, j] = type;
                    Obj[i, j].GetComponent<DropCnt>().Set(type);
                }
                
            }
        }
    }

    /// <summary>
    /// 今の盤面上に空白ドロップが存在しているかどうかをチェックする
    /// </summary>
    /// <returns>空白ドロップが存在=>false,存在していない=>true(チェックOK)</returns>
    public bool Check()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                //空白ドロップが発見する場合、falseを返す
                if (Field[i, j] == 6)
                {
                    return false;
                }

            }
        }
        //発見していない場合、trueを返す
        return true;
    }
}
