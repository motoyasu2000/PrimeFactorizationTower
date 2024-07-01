using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common
{
    /// <summary>
    /// 様々な機能を提供するヘルパークラス 
    /// </summary>
    public static class Helper
    {
        //シーンを推移すると同時に、BGMをシーンに合わせて適切に変更する
        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            SoundManager.LoadSoundSettingData();
            SoundManager soundManager = SoundManager.Ins;
            if (sceneName == "PlayScene") soundManager.PlayAudio(soundManager.BGM_PLAY);
            else if (sceneName == "TitleScene") soundManager.PlayAudio(soundManager.BGM_TITLE);
            else if (sceneName == "MaterialScene") soundManager.PlayAudio(soundManager.BGM_MATERIAL);
        }


        /// <summary>
        /// 引数で分割数を指定し、その分割数にあったアンカーポイントのfloat配列をビューポート座標で返す。
        /// </summary>
        /// <param name="numOfDiv">分割数</param>
        /// <returns>分割されたアンカーポイントのfloat配列</returns>
        public static float[] CalculateSplitAnchorPoints(int numOfDiv)
        {
            float[] splitAnchorPoints = new float[numOfDiv+1];
            float anchorPointsInterval = 1.0f / (float)numOfDiv;
            float nowAnchorPoint = 0.0f;
            for(int i=0; i<numOfDiv; i++)
            {
                splitAnchorPoints[i] = nowAnchorPoint;
                nowAnchorPoint += anchorPointsInterval;
            }
            //浮動小数点誤差を無くすため、最後は直接1を入れる
            splitAnchorPoints[numOfDiv] = 1.0f;
            return splitAnchorPoints;
        }

        //引数で受け取った素数辞書の合成数を計算するメソッド
        public static int CalculateCompsiteNumberForDict(Dictionary<int,int> primeNumberDict)
        {
            int compositNumber = 1;
            foreach (KeyValuePair<int,int> pair in primeNumberDict)
            {
                int primeNumber = pair.Key;
                int primeCount = pair.Value;
                compositNumber *= (int)MathF.Pow(primeNumber, primeCount);
            }
            return compositNumber;
        }

        //第一引数で受け取った各素数の数をカウントする辞書に、第二引数の素数を追加する処理
        static void AddPrimeNumberDict(ref Dictionary<int,int> primeNumberDict, int primeNumber)
        {
            //追加したいキーが存在しなければ初期化。
            if (!primeNumberDict.ContainsKey(primeNumber))
            {
                primeNumberDict[primeNumber] = 0;
            }

            primeNumberDict[primeNumber]++;
        }

        /// <summary>
        /// 合成数を辞書型として生成する。
        /// 引数で様々なカスタムが可能。
        /// </summary>
        /// <param name="primeNumberPool">合成数生成のもととなる素数リスト</param>
        /// <param name="maxCompositeNumber">合成数の上限値</param>
        /// <param name="minRand">合成数を構成する素数の最小個数</param>
        /// <param name="maxRand">合成数を構成する素数の最大個数</param>
        /// <returns>生成した素数辞書</returns>
        public static Dictionary<int, int> GenerateCompositeNumberDictCustom(List<int> primeNumberPool, int maxCompositeNumber, int minRand, int maxRand)
        {
            int randomIndex; //素数のインデックス(乱数によって生成)
            int randomPrimeNumber; //インデックスに対応する素数
            int nowCompositeNumber = 1;
            int numberOfPrimeNumber = UnityEngine.Random.Range(minRand, maxRand+1);

            Dictionary<int, int> compositeNumbersDict = new Dictionary<int, int>();
            for (int i = 0; i < numberOfPrimeNumber; i++)
            {
                randomIndex = UnityEngine.Random.Range(0, primeNumberPool.Count);
                randomPrimeNumber = primeNumberPool[randomIndex]; //乱数インデックスから素数の生成
                if (nowCompositeNumber * randomPrimeNumber > maxCompositeNumber) continue; //もし生成した素数を追加すると上限値を超えてしまうならfor文を戻る
                nowCompositeNumber *= randomPrimeNumber; //現在の素数の積を更新
                AddPrimeNumberDict(ref compositeNumbersDict, randomPrimeNumber); //各素数の数を数える辞書に素数の追加
            }

            //Debug.Log(String.Join(",", compositeNumbersDict.Keys));
            //Debug.Log(String.Join(",", compositeNumbersDict.Values));
            return compositeNumbersDict;
        }

        //引数で与えられたゲームオブジェクトたちの重心を計算して返すメソッド
        public static Vector3 CaluculateCenter(List<GameObject> gameObjects)
        {
            Vector3 center = Vector3.zero;
            foreach (var gameObject in gameObjects)
            {
                center += gameObject.transform.position;
            }
            center /= gameObjects.Count;
            return center;
        }
    }
}