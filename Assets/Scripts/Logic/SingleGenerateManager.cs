﻿using Common;
using UnityEngine;

//ブロックの生成が単一であることを保証するクラス
//複数のブロックが同時に生成されないよう、異なるブロックを生成仕様としたとき、初めにあったブロックの方が削除され、上書きされるようにする。
//既にあるブロックと同じボタンを押した場合には、初めにあったブロックが時計回りに回転する。(ここは後に他のクラスで管理できるようにする。)
public class SingleGenerateManager : MonoBehaviour
{
    ScoreManager scoreManager;
    GameObject singleBlock;
    Vector3 defaultPoint; //初期位置
    public GameObject SingleBlock => singleBlock;
    private void Awake()
    {
        defaultPoint = transform.position;
        scoreManager = ScoreManager.ScoreManagerInstance;
    }
    private void Update()
    {
        MoveSingleGameObjectPoint();
    }

    //単一のブロックのみが格納されることを保証する。
    public void SetSingleGameObject(GameObject newBlock)
    {
        //引数がnullならsingleGameObjectをnullにして処理を終了
        if(newBlock == null)
        {
            singleBlock = null;
            return;
        }

        //singleGameObjectがもともとnullなら普通に代入
        if(singleBlock == null)
        {
            singleBlock = newBlock;
        }
        //singleGameObjectに何かが入っている状態で呼ばれた場合には
        else
        {
            //ブロックの数値が一致していなければ、元ブロックを消去して更新
            if(singleBlock.GetComponent<BlockInfo>().GetPrimeNumber() != newBlock.GetComponent<BlockInfo>().GetPrimeNumber())
            {
                //Debug.Log($"oldnum: {singleGameObject.GetComponent<BlockInfo>().GetNumber()}  newnum: {setObject.GetComponent<BlockInfo>().GetNumber()}");
                Destroy(singleBlock);
                singleBlock = newBlock;
            }
            //一致していたら、新しい方を消して更新を行わない。
            else
            {
                Destroy(newBlock);
            }
        }
    }

    public GameObject GetSingleGameObject()
    {
        return singleBlock;
    }

    //ブロックの生成地点をゲームの実行中に変更するメソッド
    void MoveSingleGameObjectPoint()
    {
        if(scoreManager.NowHeight < Info.CameraTrackingStartHeight) return;
        transform.position = new Vector3(defaultPoint.x,scoreManager.NowHeight + 3, defaultPoint.z); //最も高いぶろっぐより3つ上にブロックを生成
    }
}
