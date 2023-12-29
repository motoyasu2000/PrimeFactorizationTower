using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI upNumberText; //��ʏ㕔�̍������̃e�L�X�g
    [SerializeField] TextMeshProUGUI nextUpNumberText;
    [SerializeField] TextMeshProUGUI remainingNumberText;
    [SerializeField] TextMeshProUGUI MainText;
    [SerializeField] TextMeshProUGUI scoreText;
    GameObject gameOverMenu;

    int nowPhase = 1; //���݂�phase
    int nowUpNumber = 1;
    int allBlockNumber = 1;
    int compositeNumber_GO; //�Q�[���I�[�o�[���̍�����
    int primeNumber_GO; //�Q�[���I�[�o�[���̑f��
    public int CompositeNumber_GO => compositeNumber_GO;
    public int PrimeNumber_GO => primeNumber_GO;

    [SerializeField] GameObject blockField;
    GameObject afterField;
    [SerializeField] GameObject completedField;

    bool isGroundAll = false;
    bool isGroundAll_past = false;
    public bool IsGroundAll => isGroundAll;
    public bool IsGroundAll_past => isGroundAll_past;
    bool completeNumberFlag = false;

    [SerializeField] Queue<int> upNumberqueue = new Queue<int>();

    SoundManager soundManager;
    ScoreManager scoreManager;
    GameModeManager gameModeManager;
    BloomManager bloomManager;

    bool isGameOver = false;
    float gameOverTimer = 0;

    

    private void Awake()
    {
        afterField = blockField.transform.Find("AfterField").gameObject;
        soundManager = SoundManager.SoundManagerInstance;
        scoreManager = ScoreManager.ScoreManagerInstance;
        gameModeManager = GameModeManager.GameModemanagerInstance;
        bloomManager = GameObject.Find("GlobalVolume").GetComponent<BloomManager>();
        upNumberqueue.Enqueue(GenerateUpNumber());
        gameOverMenu = GameObject.Find("Canvas").transform.Find("GameOverMenu").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (string.IsNullOrWhiteSpace(upNumberText.text))//�����񂪋�ł����
        {
            upNumberqueue.Enqueue(GenerateUpNumber());
            nowUpNumber = upNumberqueue.Dequeue();
            upNumberText.text = nowUpNumber.ToString();
            nextUpNumberText.text = upNumberqueue.Peek().ToString();
            remainingNumberText.text = nowUpNumber.ToString(); //�c��̐��l���X�V����^�C�~���O�Ŏc��i���o�[���X�V����K�v������B
        }

        isGroundAll_past = isGroundAll;

        isGroundAll = true; //������true�ɂ��Ă���(����g���Ă��Ȃ�)
        allBlockNumber = 1; //������1�ɂ��Ă���(����g���Ă��Ȃ�)
        foreach (Transform block in afterField.transform) //���ׂẴQ�[���I�u�W�F�N�g�̃`�F�b�N
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();
            if (!blockInfo.CheckIsGround()) //��ł��n�ʂɐڒn���ĂȂ����
            {
                isGroundAll = false; //isGroundAll��false
            }

            allBlockNumber *= blockInfo.GetNumber();//����block�̑f������̍������̑f��������Ȃ�������
            remainingNumberText.text = (nowUpNumber / allBlockNumber).ToString(); //�c��̐������v�Z���ĕ`��B������afterField����ɂȂ�Ƃ��̒��̏������s���Ȃ��Ȃ�̂�
                                                                                  //UpNumber�̍X�V�̂��тɁA���̒l���X�V���Ă�����K�v������B

            if (nowUpNumber % allBlockNumber != 0)
            {
                if (isGameOver) break;
                compositeNumber_GO = nowUpNumber * afterField.transform.GetChild(afterField.transform.childCount - 1).GetComponent<BlockInfo>().GetNumber() / allBlockNumber;
                primeNumber_GO = afterField.transform.GetChild(afterField.transform.childCount - 1).GetComponent<BlockInfo>().GetNumber();
                Debug.Log(compositeNumber_GO);
                Debug.Log(primeNumber_GO);
                GameOver();
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

        //�����ςݏグ���[�h�ŁA�n�ʂɐݒu���Ă���Ȃ獂�����v�Z����B
        if (gameModeManager.NowGameMode == GameModeManager.GameMode.PileUp)
        {
            if (isGroundAll)
            {
                scoreText.text = ((int)(scoreManager.CalculateAllVerticesHeight()*1000)).ToString();
            }
        }

        if (allBlockNumber == nowUpNumber) //�����u���b�N�̐��l�̐ς��A�㕔�̍������ƈ�v���Ă����Ȃ�
        {
            completeNumberFlag = true;
        }

        //�������B�����̏���
        if (completeNumberFlag)
        {
            RemoveUpNumber(); //��̐����̏���
            soundManager.PlayAudio(soundManager.SE_DONE); //done�̍Đ�
        }

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

    public bool GetCompleteNumberFlag()
    {
        return completeNumberFlag;
    }

    int GenerateUpNumber()
    {
        int randomIndex;
        int randomPrimeNumber;
        int returnUpNumber = 1;

        switch (GameModeManager.GameModemanagerInstance.NowDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                for (int i = 0; i < 2 + (int)(UnityEngine.Random.value * nowPhase / 2) && i <= 5; i++)
                {
                    randomIndex = UnityEngine.Random.Range(0, gameModeManager.NormalPool.Count);
                    randomPrimeNumber = gameModeManager.NormalPool[randomIndex];
                    returnUpNumber *= randomPrimeNumber;
                }
                break;
                    
            case GameModeManager.DifficultyLevel.Difficult:
                for (int i = 0; i < 2 + (int)(UnityEngine.Random.value * nowPhase / 2) && i <= 5; i++)
                {
                    randomIndex = UnityEngine.Random.Range(0, gameModeManager.DifficultPool.Count);
                    randomPrimeNumber = gameModeManager.DifficultPool[randomIndex];
                    returnUpNumber *= randomPrimeNumber;
                }
                break;

            case GameModeManager.DifficultyLevel.Insane:
                for (int i = 0; i < 2 + (int)(UnityEngine.Random.value * nowPhase / 2) && i <= 5; i++)
                {
                    randomIndex = UnityEngine.Random.Range(0, gameModeManager.InsanePool.Count);
                    randomPrimeNumber = gameModeManager.InsanePool[randomIndex];
                    returnUpNumber *= randomPrimeNumber;
                }
                break;
        }

        nowPhase++;
        return returnUpNumber;
        return 16 * 27 * 125 * 343;
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
        upNumberText.text = ""; //�e�L�X�g�̏�����
        allBlockNumber = 1; //�f���̐ς̏�����
        completeNumberFlag = false; //���ꂪtrue�̊Ԃ�block����������Ȃ��悤�ɂȂ��Ă���̂ŁAremove�̏u�Ԃɒ����Ă�����Ђ悤������B
    }

    public void GameOver()
    {
        ScoreManager.ScoreManagerInstance.InsertPileUpScoreAndSort((int)(ScoreManager.ScoreManagerInstance.CalculateAllVerticesHeight() * 1000));
        ScoreManager.ScoreManagerInstance.SaveScoreData();
        isGameOver = true;
        bloomManager.isLightUpStart = true;
        soundManager.FadeOutVolume();
    }

    public void PostGameOver()
    {
        gameOverMenu.SetActive(true);
        soundManager.StopAudio(soundManager.BGM_PLAY);
        SoundManager.LoadSoundData();
    }


}