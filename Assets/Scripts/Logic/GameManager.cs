using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Rendering;
using static Unity.Collections.AllocatorManager;

public class GameManager : MonoBehaviour
{
    //UI
    TextMeshProUGUI nowUpCompositeNumberText;
    TextMeshProUGUI nextUpCompositeNumberText;
    TextMeshProUGUI nowScoreText;
    GameObject gameOverMenu;
    GameObject explainPileUp; //�`���[�g���A�����̃e�L�X�g�̕\��

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
    int nowUpNumber = 1;
    int nowPrimeNumberProduct = 1;
    bool completeCompositeNumberFlag = false;
    Queue<int> upNumberqueue = new Queue<int>();
    SoundManager soundManager;

    //�Q�[���I�[�o�[����
    int compositeNumber_GO; //�Q�[���I�[�o�[���̍�����
    int primeNumber_GO; //�Q�[���I�[�o�[���̑f��
    float gameOverTimer = 0;
    bool isGameOver = false;
    BloomManager bloomManager; //�Q�[���I�[�o�[���̉��o�p
    public int CompositeNumber_GO => compositeNumber_GO;
    public int PrimeNumber_GO => primeNumber_GO;
    public bool IsGameOver => isGameOver;

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
        upNumberqueue.Enqueue(GenerateUpNumber());
        gameOverMenu = GameObject.Find("Canvas").transform.Find("GameOverMenu").gameObject;
        explainPileUp = GameObject.Find("Canvas").transform.Find("ExplainPileUp").gameObject;
    }

    private void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "/PileUp.json")) explainPileUp.gameObject.SetActive(true); //�Z�[�u�f�[�^���Ȃ���ΐ������s���B
    }

    // Update is called once per frame
    void Update()
    {
        UpCompositeNumberSetting();
        CheckAllBlocksOnGround(); 
        CheckPrimeNumberProduct();
        CalculateScore();
        CountGameOverTime();
    }

    public bool GetCompleteNumberFlag()
    {
        return completeCompositeNumberFlag;
    }

    int GenerateUpNumber()
    {
        int randomIndex;
        int randomPrimeNumber;
        int returnUpNumber = 1;

        switch (GameModeManager.GameModemanagerInstance.NowDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                for (int i = 0; i <UnityEngine.Random.Range(2,5); i++)
                {
                    randomIndex = UnityEngine.Random.Range(0, gameModeManager.NormalPool.Count);
                    randomPrimeNumber = gameModeManager.NormalPool[randomIndex];
                    if (returnUpNumber * randomPrimeNumber < 10000) returnUpNumber *= randomPrimeNumber;
                }
                break;
                    
            case GameModeManager.DifficultyLevel.Difficult:
                for (int i = 0; i < UnityEngine.Random.Range(3, 6); i++)
                {
                    randomIndex = UnityEngine.Random.Range(0, gameModeManager.DifficultPool.Count);
                    randomPrimeNumber = gameModeManager.DifficultPool[randomIndex];
                    if (returnUpNumber * randomPrimeNumber < 10000) returnUpNumber *= randomPrimeNumber;
                }
                break;

            case GameModeManager.DifficultyLevel.Insane:
                for (int i = 0; i < UnityEngine.Random.Range(3, 7); i++)
                {
                    randomIndex = UnityEngine.Random.Range(0, gameModeManager.InsanePool.Count);
                    randomPrimeNumber = gameModeManager.InsanePool[randomIndex];
                    if(returnUpNumber * randomPrimeNumber < 100000) returnUpNumber *= randomPrimeNumber;
                }
                break;
        }

        nowPhase++;
        return returnUpNumber;
        return 16 * 27 * 125 * 343;
    }

    //��ʏ㕔�ɕ\������鍇������A�l�N�X�g�̍������̐ݒ���s��
    void UpCompositeNumberSetting()
    {
        //��ʏ㕔�̍�����������ł���΁A�܂�f�������������������Ȃ��
        if (string.IsNullOrWhiteSpace(nowUpCompositeNumberText.text))
        {
            upNumberqueue.Enqueue(GenerateUpNumber());
            nowUpNumber = upNumberqueue.Dequeue();
            nowUpCompositeNumberText.text = nowUpNumber.ToString();
            nextUpCompositeNumberText.text = upNumberqueue.Peek().ToString();
        }
    }

    //�S�ẴQ�[���I�u�W�F�N�g���n�ʂɐݒu���Ă��邩�̃`�F�b�N
    void CheckAllBlocksOnGround()
    {
        isGroundAll_past = isGroundAll;
        isGroundAll = true; //������true�ɂ��Ă���
        foreach (Transform block in afterField.transform)
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();
            if (!blockInfo.CheckIsGround()) //��ł��n�ʂɐڒn���ĂȂ����
            {
                isGroundAll = false; //isGroundAll��false
            }
        }
        foreach (Transform block in completedField.transform)
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
        nowPrimeNumberProduct = 1; //������1�ɂ��Ă���
        foreach (Transform block in afterField.transform) //afterField���̑S�ẴQ�[���I�u�W�F�N�g�̃`�F�b�N
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();

            nowPrimeNumberProduct *= blockInfo.GetPrimeNumber();//����block�̑f������̍������̑f��������Ȃ�������
            nowUpCompositeNumberText.text = (nowUpNumber / nowPrimeNumberProduct).ToString(); //�c��̐������v�Z���ĕ`��B������afterField����ɂȂ�Ƃ��̒��̏������s���Ȃ��Ȃ�̂�
                                                                                           //UpNumber�̍X�V�̂��тɁA���̒l���X�V���Ă�����K�v������B

            if (nowUpNumber % nowPrimeNumberProduct != 0)
            {
                if (isGameOver) break;
                //�Ō�̃Q�[���I�[�o�[���R�̏o�͂̍ۂɁA���̍������Ƃ��̎��I�����Ă��܂����f���̏�񂪕K�v�Ȃ̂ŁA�ϐ��ɓ���Ă����B
                compositeNumber_GO = nowUpNumber * afterField.transform.GetChild(afterField.transform.childCount - 1).GetComponent<BlockInfo>().GetPrimeNumber() / nowPrimeNumberProduct;
                primeNumber_GO = afterField.transform.GetChild(afterField.transform.childCount - 1).GetComponent<BlockInfo>().GetPrimeNumber();
                Debug.Log(compositeNumber_GO);
                Debug.Log(primeNumber_GO);
                GameOver();
            }
        }
        //�����u���b�N�̐��l�̐ς��A�㕔�̍������ƈ�v���Ă����Ȃ�
        if (nowPrimeNumberProduct == nowUpNumber) 
        {
            completeCompositeNumberFlag = true;
        }
        //��ʏ㕔�̍������B�����̏���
        if (completeCompositeNumberFlag)
        {
            RemoveUpNumber(); //��̐����̏���
            soundManager.PlayAudio(soundManager.SE_DONE); //done�̍Đ�
        }
    }

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

    void RemoveUpNumber()
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
        nowUpCompositeNumberText.text = ""; //�e�L�X�g�̏�����
        nowPrimeNumberProduct = 1; //�f���̐ς̏�����
        completeCompositeNumberFlag = false; //���ꂪtrue�̊Ԃ�block����������Ȃ��悤�ɂȂ��Ă���̂ŁAremove�̏u�Ԃɒ����Ă�����Ђ悤������B
    }

    public void GameOver()
    {
        if (isGameOver) return;
        //�\�[�g�O�ɉߋ��̍ō��X�R�A�̏����擾���Ă���(�̂��ɂ��̃Q�[���ōō��X�R�A���X�V���������m�F���邽��)
        oldMaxScore = scoreManager.PileUpScores[gameModeManager.NowDifficultyLevel][0];

        scoreManager.InsertPileUpScoreAndSort(newScore);
        scoreManager.SaveScoreData();
        isGameOver = true;
        bloomManager.isLightUpStart = true;
        soundManager.FadeOutVolume();
    }

    void CountGameOverTime()
    {
        if (isGameOver)
        {
            gameOverTimer += Time.deltaTime;
            if (gameOverTimer > 1.2f)
            {
                PostGameOver();
                gameOverTimer = float.MinValue;
            }
        }
    }

    //�Q�[���I�[�o�[�������s������̌㏈��
    void PostGameOver()
    {
        gameOverMenu.SetActive(true);
        soundManager.StopAudio(soundManager.BGM_PLAY);
        SoundManager.LoadSoundData();
    }


}