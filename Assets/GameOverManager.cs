using AWS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    const float delayTime = 1.2f;
    int oldMaxScore = -1;
    int newScore = -1;
    int compositeNumberAtGameOver; //�Q�[���I�[�o�[���̍�����
    int blockNumberAtGameOver; //�Q�[���I�[�o�[�̈������ƂȂ����u���b�N�̑f��

    bool isGameOver = false; //�Q�[���I�[�o�[�ɂȂ����炱�̃t���O��true�ɂ��Afalse�̎��̂݃Q�[���I�[�o�[�̏��������s����悤�ɂ��邱�ƂŁA�Q�[���I�[�o�[�̏�����1�x�����Ă΂�Ȃ��悤�ɂ���B
    GameObject primeNumberCheckField; //�u���b�N�𗎉��������u�ԁA���̃u���b�N�́A���̃Q�[���I�u�W�F�N�g�̎q�v�f�ƂȂ�
    GameObject gameOverMenu;
    GameObject gameOverBlock; //�Q�[���I�[�o�[�̈������ƂȂ����u���b�N
    BloomManager bloomManager; //�Q�[���I�[�o�[���̉��o�p
    OriginManager originManager;
    DynamoDBManager ddbManager;

    public int CompositeNumberAtGameOver => compositeNumberAtGameOver;
    public int BlockNumberAtGameOver => blockNumberAtGameOver;
    public int OldMaxScore => oldMaxScore;
    public int NewScore => newScore;

    public bool IsBreakScore => (oldMaxScore < newScore); //�X�R�A���X�V�������𔻒肷��t���O

    private void Awake()
    {
        originManager = GameObject.Find("OriginManager").GetComponent<OriginManager>();
        ddbManager = GameObject.Find("DynamoDBManager").GetComponent<DynamoDBManager>();
        primeNumberCheckField = GameObject.Find("PrimeNumberCheckField");
        gameOverMenu = GameObject.Find("Canvas").transform.Find("GameOverMenu").gameObject;
        bloomManager = GameObject.Find("GlobalVolume").GetComponent<BloomManager>();
    }
    public async void GameOver(bool isFactorizationIncorrect)
    {
        //���̃��\�b�h��1�x�����Ă΂�Ȃ��悤��
        if (isGameOver) return;
        else isGameOver = true;

        Debug.Log("GameOver");

        //�f�����������ԈႦ�Ă��܂����ꍇ�A�Ō�̃Q�[���I�[�o�[���R�̏o�͂̍ۂɁA���̍������Ƃ��̎��I�����Ă��܂����f���̏�񂪕K�v�Ȃ̂ŁA�ϐ��ɓ���Ă����B
        if (isFactorizationIncorrect)
        {
            gameOverBlock = primeNumberCheckField.transform.GetChild(primeNumberCheckField.transform.childCount - 1).gameObject;
            blockNumberAtGameOver = gameOverBlock.GetComponent<BlockInfo>().GetPrimeNumber();
            compositeNumberAtGameOver = originManager.OriginNumber * blockNumberAtGameOver / CalculateBlocksCompositNumberAtGameOver(); //CalculateBlocksCompositNumberAtGameOver()�ɂ�blockNumber_GO���܂܂�Ă��邽��blockNumber_GO��������
        }

        //�X�R�A�̍X�V�ƃQ�[���I�[�o�[���̉��o�A�㏈���̌Ăяo���B
        oldMaxScore = ScoreManager.Ins.PileUpScores[GameModeManager.Ins.NowDifficultyLevel][0]; //�\�[�g�O�ɉߋ��̍ō��X�R�A�̏����擾���Ă���(�̂��ɂ��̃Q�[���ōō��X�R�A���X�V���������m�F���邽��)
        bloomManager.LightUpStart();
        ScoreManager.Ins.InsertPileUpScoreAndSort(newScore);
        ScoreManager.Ins.SaveScoreData();
        SoundManager.Ins.FadeOutVolume();
        //�X�R�A���X�V���Ă���΁A�f�[�^�x�[�X�̍X�V
        if (IsBreakScore) await ddbManager.SaveScoreAsyncHandler(GameModeManager.Ins.ModeAndLevel, newScore);

        StartCoroutine(PostGameOver(delayTime));
    }


    //�Q�[���I�[�o�[��A��莞�Ԍ�ɃQ�[���I�[�o�[���j���[��\�����Abgm�̃X�g�b�v�B�Q�[���I�[�o�[�̌㏈��
    IEnumerator PostGameOver(float time)
    {
        yield return new WaitForSeconds(time);
        gameOverMenu.SetActive(true);
        SoundManager.Ins.StopAudio(SoundManager.Ins.BGM_PLAY);
        SoundManager.LoadSoundSettingData();
    }

    int CalculateBlocksCompositNumberAtGameOver()
    {
        int blocksCompositNumberAtGameOver = 1;
        foreach(Transform block in primeNumberCheckField.transform)
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();
            blocksCompositNumberAtGameOver *= blockInfo.GetPrimeNumber();
        }
        return blocksCompositNumberAtGameOver;
    }
}
