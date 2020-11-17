using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// ドロップコントローラークラス
/// </summary>
public class DropCnt : MonoBehaviour
{
    //ドロップスプライト
    [SerializeField] Sprite[] sp;
    //ドロップス座標
    public int ID1;//行
    public int ID2;//列

    //ドロップをタッチされているかどうかのフラグ
    bool touchFlag = false;
    //マウス座標(スクリーン空間)
    Vector2 m;
    //r=>ドロップ現在位置(ワールド空間)  r2=>ドロップの親オブジェクト位置(ワールド空間)
    RectTransform r, r2;

    //ドロップ初期位置(ワールド空間)
    public Vector2 P1 { get; set; }
    //ドロップ初期位置(スクリーン空間)
    public Vector2 P2 { get; set; }

    //盤面データ制御クラス
    Director d;

    //初期データ取得
    void Start()
    {
        r = GetComponent<RectTransform>();
        r2 = transform.parent.GetComponent<RectTransform>();
        d = GameObject.Find("D").GetComponent<Director>();
    }

    // Update is called once per frame
    void Update()
    {
        //タッチされている場合
        if (touchFlag)
        {
            //位置出力変数を初期化
            var pos = Vector2.zero;
            m = Input.mousePosition;
            //タッチしている位置を、スクリーン空間から、ローカル世界空間座標値に変換、出力
            RectTransformUtility.ScreenPointToLocalPointInRectangle
                (r2, m, Camera.main, out pos);
            //ドロップローカル位置として指定
            r.localPosition = pos;
        }

        //ドロップ初期位置を取得
        if (P1.x == 0)
        {
            P1 = GetComponent<RectTransform>().position;
            P2 = RectTransformUtility.WorldToScreenPoint(Camera.main, P1);
        }
        //ドロップが交換される或いはタッチされているまで初期位置に縛る
        else
        {
            if (!touchFlag)
            {
                GetComponent<RectTransform>().position = P1;
            }
        }
    }

    /// <summary>
    /// ドロップタイプを設定する
    /// </summary>
    /// <param name="n">タイプID</param>
    public void Set(int n)
    {
        GetComponent<SpriteRenderer>().sprite = sp[n];
    }

    /// <summary>
    /// ドロップをつかむ
    /// </summary>
    public void GetDrop()
    {
        touchFlag = true;
    }
    /// <summary>
    /// ドロップをセットする
    /// </summary>
    public void SetDrop()
    {
        touchFlag = false;
        //セットした後、ドロップ消すイベントを呼び出す
        Delete();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (touchFlag)
        {
            if (d.CheckDistance(m,collision.gameObject.GetComponent<DropCnt>().P2))
            {
                d.ChangePos(gameObject, collision.gameObject);
            }
        }
    }


    /// <summary>
    /// ドロップを消すイベントを呼び出す
    /// </summary>
    private async void Delete()
    {
        //まだ空白ドロップが存在していると無限ループさせる
        while (true)
        {
            //ドロップ消す
            d.DeleteDrop();
            //空白ドロップが全部消した場合、抜き出す
            if (d.Check())
            {
                break;
            }
            await Task.Delay(1000);
            //ドロップ落とす
            d.DownDrop();
            await Task.Delay(500);
            //空いているマスを埋め込む
            d.ResetDrop();
        }
    }
}
