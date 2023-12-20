using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //��Փx��\���񋓌^�̒�`
    public enum DifficultyLevel
    {
        Normal,
        difficult,
        Insane
    }

    DifficultyLevel myDifficultyLevel = DifficultyLevel.Normal; //��Փx�^�̕ϐ����`�A�Ƃ肠����Normal�ŏ����� �K�؂ȃ^�C�~���O�œ�Փx�������ł���悤�ɐ؂�ւ���K�v������B
    public DifficultyLevel MyDifficultyLevel => myDifficultyLevel;

    int[] primeNumberPool = new int[9]
    {
        2,3,5,7,11,13,17,19,23
    };

    List<int> normalPool = new List<int>();
    List<int> difficultPool = new List<int>();
    List<int> insanePool = new List<int>();
    public List<int> NormalPool => normalPool;

    [SerializeField] TextMeshProUGUI upNumberText; //��ʏ㕔�̍������̃e�L�X�g
    [SerializeField] TextMeshProUGUI nextUpNumberText;
    [SerializeField] TextMeshProUGUI remainingNumberText;
    [SerializeField] TextMeshProUGUI MainText;
    [SerializeField] TextMeshProUGUI scoreText;

    int nowPhase = 1; //���݂�phase
    int nowUpNumber = 1;

    int allBlockNumber = 1;

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

    bool isFirstAwake = true;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < primeNumberPool.Length; i++)
        {
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 7) normalPool.Add(primeNumberPool[i]);
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 13) difficultPool.Add(primeNumberPool[i]);
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 23) insanePool.Add(primeNumberPool[i]);
        }
        

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "PlayScene") return;
        if (upNumberText == null)
        {
            upNumberText = GameObject.Find("NowUpNumber").GetComponent<TextMeshProUGUI>();
            nextUpNumberText = GameObject.Find("NextUpNumber").GetComponent<TextMeshProUGUI>();
            remainingNumberText = GameObject.Find("RemainingNumberText").GetComponent<TextMeshProUGUI>();
            MainText = GameObject.Find("MainText").GetComponent<TextMeshProUGUI>();
            scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
            blockField = GameObject.Find("BlockField");
            completedField = GameObject.Find("CompletedField");
            afterField = blockField.transform.Find("AfterField").gameObject;
            upNumberqueue.Enqueue(GenerateUpNumber());
            soundManager = transform.Find("SoundManager").GetComponent<SoundManager>();
            scoreManager = transform.Find("ScoreManager").GetComponent<ScoreManager>();
            gameModeManager = transform.Find("GameModeManager").GetComponent<GameModeManager>();
        }
        if (string.IsNullOrWhiteSpace(upNumberText.text))//�����񂪋�ł����
        {
            Debug.Log($"upnumber == null");
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
                scoreText.text = ((int)(scoreManager.CalculateAllVerticesHeight() * 1000)).ToString();
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
    }

    public bool GetCompleteNumberFlag()
    {
        return completeNumberFlag;
    }

    void ChangeDifficultyLevel(DifficultyLevel newDifficultyLevel)
    {
        myDifficultyLevel = newDifficultyLevel;
    }

    int GenerateUpNumber()
    {
        int randomIndex;
        int randomPrimeNumber;
        int returnUpNumber = 1;

        if (myDifficultyLevel == DifficultyLevel.Normal)
        {
            for (int i = 0; i < 2 + (int)(Random.value * nowPhase / 2); i++)
            {
                randomIndex = Random.Range(0, normalPool.Count);
                randomPrimeNumber = normalPool[randomIndex];
                returnUpNumber *= randomPrimeNumber;
            }
        }

        nowPhase++;
        return returnUpNumber;
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

    public static void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    

}
