using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Rendering;
using static Unity.Collections.AllocatorManager;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    //UI
    TextMeshProUGUI nowUpCompositeNumberText;
    TextMeshProUGUI nextUpCompositeNumberText;
    TextMeshProUGUI nowScoreText;
    GameObject gameOverMenu;
    GameObject explainPileUp; //�`���[�g���A�����̃e�L�X�g

    //�X�R�A�̊Ǘ�
    int oldMaxScore = -1;
    int newScore = -1;
    bool isGroundAll = false; //�S�Ẵu���b�N���n�ʂɐݒu���Ă��邩���`�F�b�N����ϐ��Bfalse�ł���΁A�������v�Z���Ȃ��B
    bool isGroundAll_past = false; //1�t���[���O��isGroundAll
    ScoreManager scoreManager;
    public bool IsGroundAll => isGroundAll;
    public bool IsGroundAll_past => isGroundAll_past;
    public int OldMaxScore => oldMaxScore;
    public int NewScore => newScore;

    //��ʒ����̍������̊Ǘ���A�f�����������ł��Ă��邩�̃`�F�b�N
    int nowUpCompositeNumber = 1;
    int nowPrimeNumberProduct = 1;
    bool completeCompositeNumberFlag = false;
    Queue<int> upCompositeNumberqueue = new Queue<int>();
    SoundManager soundManager;

    //�Q�[���I�[�o�[����
    int compositeNumber_GO; //�Q�[���I�[�o�[���̍�����
    int primeNumber_GO; //�Q�[���I�[�o�[���̑f��
    BloomManager bloomManager; //�Q�[���I�[�o�[���̉��o�p
    public int CompositeNumber_GO => compositeNumber_GO;
    public int PrimeNumber_GO => primeNumber_GO;

    //�u���b�N�̐e�I�u�W�F�N�g���
    GameObject blockField; //����̗l�ȃu���b�N�̐e�I�u�W�F�N�g���܂Ƃ߂�e�I�u�W�F�N�g
    GameObject afterField; //�u���b�N�𗎉��������u�ԁA���̃u���b�N�́A���̃Q�[���I�u�W�F�N�g�̎q�v�f�ƂȂ�
    GameObject completedField; //afterField���̃u���b�N�̐ς���ʏ㕔�̍������ƈ�v������A�����̃u���b�N�͂��̃Q�[���I�u�W�F�N�g�̎q�v�f�ɂȂ�

    //���̑�
    GameModeManager gameModeManager; //��Փx���Ƃɐ������鍇�������قȂ�̂ŁA���݂̓�Փx�̏�������Gamemodemanager�̏�񂪕K�v
                                     //�܂��A�X�R�A��ۑ�����ہA�ǂ̓�Փx�̃X�R�A���X�V���邩�̏����K�v�Ȃ̂ŁA�����ł��g���B
    int nowPhase = 0; //���݂����̍�������f�����������I�������@���ꂪ������Ə�ɕ\������鍇�����̒l���傫���Ȃ�Ȃǂ���B

    //����������
    private void Awake()
    {
        nowUpCompositeNumberText = GameObject.Find("NowUpCompositeNumberText").GetComponent<TextMeshProUGUI>();
        nextUpCompositeNumberText = GameObject.Find("NextUpCompositeNumberText").GetComponent<TextMeshProUGUI>();
        nowScoreText = GameObject.Find("NowScoreText").GetComponent<TextMeshProUGUI>();
        blockField = GameObject.Find("BlockField");
        afterField = blockField.transform.Find("AfterField").gameObject;
        completedField = blockField.transform.Find("CompletedField").gameObject;
        soundManager = SoundManager.SoundManagerInstance;
        scoreManager = ScoreManager.ScoreManagerInstance;
        gameModeManager = GameModeManager.GameModemanagerInstance;
        bloomManager = GameObject.Find("GlobalVolume").GetComponent<BloomManager>();
        upCompositeNumberqueue.Enqueue(GenerateCompositeNumber());
        gameOverMenu = GameObject.Find("Canvas").transform.Find("GameOverMenu").gameObject;
        explainPileUp = GameObject.Find("Canvas").transform.Find("ExplainPileUp").gameObject;
    }

    private void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "/PileUp.json")) explainPileUp.gameObject.SetActive(true); //�Z�[�u�f�[�^���Ȃ���ΐ������s���B
    }

    void Update()
    {
        UpCompositeNumberSetting();
        CheckAllBlocksOnGround(); 
        CheckPrimeNumberProduct();
        CalculateScore();
    }


    //��ʏ㕔�ɕ\������鍇������A�l�N�X�g�̍������̐ݒ���s��
    void UpCompositeNumberSetting()
    {
        //��ʏ㕔�̍�����������ł���΁A�܂�f�������������������Ȃ��
        if (string.IsNullOrWhiteSpace(nowUpCompositeNumberText.text))
        {
            completeCompositeNumberFlag = false; //���ꂪtrue�̊Ԃ�block����������Ȃ��悤�ɂȂ��Ă���̂ŁA��ʏ㕔�̍��������X�V���ꂽ�u�Ԃ�false�ɂ��Ă�����B
            upCompositeNumberqueue.Enqueue(GenerateCompositeNumber());
            nowUpCompositeNumber = upCompositeNumberqueue.Dequeue();
            nowUpCompositeNumberText.text = nowUpCompositeNumber.ToString();
            nextUpCompositeNumberText.text = upCompositeNumberqueue.Peek().ToString();
        }
    }

    //���݂̓�Փx���ǂ̗l�ɂȂ��Ă����Ƃ��Ă��A���̓�Փx�ɍ������������𐶐�����
    int GenerateCompositeNumber()
    {
        int upCompositeNumber = -1;

        switch (GameModeManager.GameModemanagerInstance.NowDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                upCompositeNumber = GenerateCompositeNumberForDifficultyLevel(gameModeManager.NormalPool, 3000, 2, 5);
                break;

            case GameModeManager.DifficultyLevel.Difficult:
                upCompositeNumber = GenerateCompositeNumberForDifficultyLevel(gameModeManager.DifficultPool, 10000, 3, 6);
                break;

            case GameModeManager.DifficultyLevel.Insane:
                upCompositeNumber = GenerateCompositeNumberForDifficultyLevel(gameModeManager.DifficultPool, 100000, 3, 7);
                break;
        }

        nowPhase++;
        return upCompositeNumber;
        return 16 * 27 * 125 * 343;
    }

    //�w�肵���f���v�[�����獇�����𐶐�����B�������̏���l��A�f���̐��������̏���l���������ƂŎw�肷�邱�Ƃ��ł���B
    int GenerateCompositeNumberForDifficultyLevel(List<int> primeNumberPool, int maxCompositeNumber ,int minRand, int maxRand)
    {
        int randomIndex;
        int randomPrimeNumber;
        int upCompositeNumber = 1;
        int numberOfPrimeNumber = Random.Range(minRand, maxRand);

        for(int i=0; i<numberOfPrimeNumber; i++)
        {
            randomIndex = Random.Range(0, primeNumberPool.Count);
            randomPrimeNumber = primeNumberPool[randomIndex];
            if(upCompositeNumber * randomPrimeNumber < maxCompositeNumber) upCompositeNumber *= randomPrimeNumber;
        }

        return upCompositeNumber;
    }

    //�S�ẴQ�[���I�u�W�F�N�g���n�ʂɐݒu���Ă��邩�̃`�F�b�N
    void CheckAllBlocksOnGround()
    {
        isGroundAll_past = isGroundAll; //1�t���[���O��isGroundAll�̕ۑ�
        isGroundAll = true; //������true�ɂ��Ă���

        //afterField�AcompletedField���̃u���b�N���S�Ēn�ʂɐݒu���Ă��邩�@�ݒu���Ă��Ȃ����isGroundAll��false�ƂȂ�
        CheckSingleFieldBlocksOnGround(afterField.transform);
        CheckSingleFieldBlocksOnGround(completedField.transform);
    }

    void CheckSingleFieldBlocksOnGround(Transform fieldTransform)
    {
        foreach (Transform block in fieldTransform)
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();
            if (!blockInfo.CheckIsGround()) //��ł��n�ʂɐڒn���ĂȂ����
            {
                isGroundAll = false; //isGroundAll��false
            }
        }
    }

    //�f���u���b�N�̐ς��A��ʏ㕔�̍������̈����ɂȂ��Ă��邩�̃`�F�b�N
    void CheckPrimeNumberProduct()
    {
        //AfterField���̍��������v�Z���A�f�����������Ԉ���Ă��Ȃ����̃`�F�b�N�@�Ԉ���Ă���΃Q�[���I�[�o�[
        CalculateNowPrimeNumberProduct();
        if (nowUpCompositeNumber % nowPrimeNumberProduct != 0)
        {
            GameOver();
        }

        //�����u���b�N�̐��l�̐ς��A�㕔�̍������ƈ�v���Ă����Ȃ�
        if (nowPrimeNumberProduct == nowUpCompositeNumber)
        {
            completeCompositeNumberFlag = true;
            RemoveUpCompositeNumber(); //��̐����̏���
            soundManager.PlayAudio(soundManager.SE_DONE); //done�̍Đ�
        }
    }

    //afterField���̃u���b�N�̐ς��v�Z�AnowPrimeNumberProduct���X�V�A�e�L�X�g�̕`��
    void CalculateNowPrimeNumberProduct()
    {
        nowPrimeNumberProduct = 1; //������1�ɂ��Ă���
        foreach (Transform block in afterField.transform) //afterField���̑S�ẴQ�[���I�u�W�F�N�g�̃`�F�b�N
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();

            nowPrimeNumberProduct *= blockInfo.GetPrimeNumber();
            nowUpCompositeNumberText.text = (nowUpCompositeNumber / nowPrimeNumberProduct).ToString(); //�c��̐������v�Z���ĕ`��B������afterField����ɂȂ�Ƃ��̒��̏������s���Ȃ��Ȃ�̂�
                                                                                                       //UpCompositeNumber�̍X�V�̂��тɁA���̒l���X�V���Ă�����K�v������B
        }
    }

    //�e�Q�[�����[�h�ł̃X�R�A�v�Z
    void CalculateScore()
    {
        //�����ςݏグ���[�h�ŁA�n�ʂɐݒu���Ă���Ȃ獂�����v�Z����B
        switch (gameModeManager.NowGameMode)
        {
            case GameModeManager.GameMode.PileUp:
                if (isGroundAll)
                {
                    newScore = scoreManager.CalculatePileUpScore();
                    nowScoreText.text = newScore.ToString();
                }
                break;
        }
    }

    //��ʏ㕔�����̍��������������AafterField���̃u���b�N��S��completedField�Ɉړ�������B
    void RemoveUpCompositeNumber()
    {
        //�܂��́AblockField����ړ�����B
        List<Transform> blocksToMove = new List<Transform>();
        //���ׂĂ̎q�I�u�W�F�N�g���ꎞ�I�ȃ��X�g�ɒǉ��BTransform���C�e���[�g���Ȃ���transform��ύX���Ȃ��悤�ɁA��U���X�g�ɒǉ��B
        foreach (Transform block in afterField.transform)
        {
            blocksToMove.Add(block);
        }
        //�ꎞ�I�ȃ��X�g���g�p���Ďq�I�u�W�F�N�g�̐e��ύX
        foreach (Transform block in blocksToMove)
        {
            block.SetParent(completedField.transform);
        }
        nowUpCompositeNumberText.text = "";
        nowPrimeNumberProduct = 1; 
    }

    public void GameOver()
    {
        //�Ō�̃Q�[���I�[�o�[���R�̏o�͂̍ۂɁA���̍������Ƃ��̎��I�����Ă��܂����f���̏�񂪕K�v�Ȃ̂ŁA�ϐ��ɓ���Ă����B
        compositeNumber_GO = nowUpCompositeNumber * afterField.transform.GetChild(afterField.transform.childCount - 1).GetComponent<BlockInfo>().GetPrimeNumber() / nowPrimeNumberProduct;
        primeNumber_GO = afterField.transform.GetChild(afterField.transform.childCount - 1).GetComponent<BlockInfo>().GetPrimeNumber();

        //�X�R�A�̍X�V�ƃQ�[���I�[�o�[���̉��o�A�㏈���̌Ăяo���BS
        oldMaxScore = scoreManager.PileUpScores[gameModeManager.NowDifficultyLevel][0]; //�\�[�g�O�ɉߋ��̍ō��X�R�A�̏����擾���Ă���(�̂��ɂ��̃Q�[���ōō��X�R�A���X�V���������m�F���邽��)
        scoreManager.InsertPileUpScoreAndSort(newScore);
        scoreManager.SaveScoreData();
        bloomManager.isLightUpStart = true;
        soundManager.FadeOutVolume();
        StartCoroutine(PostGameOver(1.2f));
    }


    //�Q�[���I�[�o�[��A��莞�Ԍ�ɃQ�[���I�[�o�[���j���[��\�����Abgm�̃X�g�b�v
    IEnumerator PostGameOver(float time)
    {
        yield return new WaitForSeconds(time);
        gameOverMenu.SetActive(true);
        soundManager.StopAudio(soundManager.BGM_PLAY);
        SoundManager.LoadSoundData();
    }

    public bool GetCompleteNumberFlag()
    {
        return completeCompositeNumberFlag;
    }
}