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

    int[] primeNumberPool = new int[9]
    {
        2,3,5,7,11,13,17,19,23
    };

    List<int> normalPool = new List<int>();
    List<int> difficultPool = new List<int>();
    List<int> insanePool = new List<int>();

    [SerializeField] TextMeshProUGUI upNumberText; //��ʏ㕔�̍������̃e�L�X�g
    [SerializeField] TextMeshProUGUI remainingNumberText;

    int nowPhase = 1; //���݂�phase
    int compositeNumber = 1;

    int allBlockNumber = 1;

    [SerializeField] GameObject blockField;
    [SerializeField] GameObject beforeField;
    GameObject afterField;
    [SerializeField] GameObject tmpField;
    [SerializeField] GameObject completedField;

    bool existTmpBlocks = false;
    bool isGroundAll = false;
    bool completeNumberFlag = false;


    void Start()
    {
        for(int i=0; i<primeNumberPool.Length; i++)
        {
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 7) normalPool.Add(primeNumberPool[i]);
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 13) difficultPool.Add(primeNumberPool[i]);
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 23) insanePool.Add(primeNumberPool[i]);
        }
        afterField = blockField.transform.Find("AfterField").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (string.IsNullOrWhiteSpace(upNumberText.text))//�����񂪋�ł����
        {
            upNumberText.text = GenerateUpNumber().ToString();
        }

        isGroundAll = true; //������true�ɂ��Ă���(����g���Ă��Ȃ�)
        allBlockNumber = 1; //������1�ɂ��Ă���(����g���Ă��Ȃ�)
        foreach (Transform block in afterField.transform) //afterField�̃`�F�b�N
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();
            if (!blockInfo.CheckIsGround()) //��ł��n�ʂɐڒn���ĂȂ����
            {
                isGroundAll = false; //isGroundAll��false
            }
            
            //�f���̐ς��X�V�B
            allBlockNumber *= blockInfo.GetNumber();//����block�̑f������̍������̑f��������Ȃ�������
            remainingNumberText.text = (compositeNumber / allBlockNumber).ToString(); //�c��̐������v�Z���ĕ`��B������afterField����ɂȂ�Ƃ��̒��̏������s���Ȃ��Ȃ�̂�
                                                                                      //UpNumber�̍X�V�̂��тɁA���̒l���X�V���Ă�����K�v������B
        }
        if (beforeField.transform.childCount != 0) //beforeField���`�F�b�N
        {
            if (!beforeField.transform.GetChild(0).GetComponent<BlockInfo>().CheckIsGround())
            {
                isGroundAll = false;
            }
            if (beforeField.transform.childCount >= 2)
            {
                Debug.LogError("beforeField�ɓ�ȏ�̃u���b�N�����݂��܂��B");
            }
        }
        foreach (Transform block in tmpField.transform) //tmpField�̃`�F�b�N afterField�̂Ԃ��������������u�Ԃ�tmpField�ɓ]������邽��
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();
            if (!blockInfo.CheckIsGround()) //��ł��n�ʂɐڒn���ĂȂ����
            {
                isGroundAll = false; //isGroundAll��false
            }
        }


        if (allBlockNumber == compositeNumber) //�����u���b�N�̐��l�̐ς��A�㕔�̍������ƈ�v���Ă����Ȃ�
        {
            completeNumberFlag = true;
        }

        if(completeNumberFlag)
        {
            SendTempBlocks();
            RemoveUpNumber();
        }

        if(isGroundAll && existTmpBlocks)
        {
            ConnectCompleteBlocks();
        }

        if (compositeNumber % allBlockNumber != 0 && isGroundAll) //���l�����Z�b�g�����̂��f���u���b�N�����ׂĂ�������^�C�~���O�ŁAAfterField����CompletedFiled�ɑ�����̂��n�ʂɐݒu�����^�C�~���O�B���̍��𖄂߂郍�W�b�N��g�ޕK�v������B���ԍ�����������n�_��tmpblocks�ɑ��M���Ă�����B���������afterblocks���ł̒T�����s���Ȃ��B
        {
            GameOver();
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
        compositeNumber = 1;

        if (myDifficultyLevel == DifficultyLevel.Normal)
        {
            for (int i=0; i<2+(int)(Random.value*nowPhase/2); i++)
            {
                randomIndex = Random.Range(0, normalPool.Count);
                randomPrimeNumber = normalPool[randomIndex];
                compositeNumber *= randomPrimeNumber;
            }
        }
        remainingNumberText.text = compositeNumber.ToString(); //�c��̐��l���X�V����^�C�~���O�Ŏc��i���o�[���X�V����K�v������B
        nowPhase++;
        return compositeNumber;
    }

    //�㕔�̐����������֐�
    void RemoveUpNumber()
    {
        upNumberText.text = ""; //�e�L�X�g�̏�����
        allBlockNumber = 1; //�f���̐ς̏�����
        completeNumberFlag = false; //���ꂪtrue�̊Ԃ�block����������Ȃ��悤�ɂȂ��Ă���̂ŁAremove�̏u�Ԃɒ����Ă�����Ђ悤������B
    }

    //���������u���b�N����������tempblock�ɓ]������֐�
    void SendTempBlocks()
    {
        //�܂��́AblockField����ړ�����B
        List<Transform> TmpBlocks = new List<Transform>();
        //���ׂĂ̎q�I�u�W�F�N�g���ꎞ�I�ȃ��X�g�ɒǉ��BTransform���C�e���[�g���Ȃ���transform��ύX���Ȃ��悤�ɁA��U���X�g�ɒǉ��B
        foreach (Transform block in afterField.transform)
        {
            TmpBlocks.Add(block);
        }
        foreach (Transform block in TmpBlocks)
        {
            block.SetParent(tmpField.transform);
        }
        existTmpBlocks = true;
    }

    //���������u���b�N���������A�e�I�u�W�F�N�g��completedField�ɓ]������֐�
    void ConnectCompleteBlocks()
    {
        //�܂��́AblockField����ړ�����B
        List<Transform> JointTmpBlocks = new List<Transform>();
        //���ׂĂ̎q�I�u�W�F�N�g���ꎞ�I�ȃ��X�g�ɒǉ��BTransform���C�e���[�g���Ȃ���transform��ύX���Ȃ��悤�ɁA��U���X�g�ɒǉ��B
        foreach (Transform block in tmpField.transform)
        {
            JointTmpBlocks.Add(block);
        }

        for (int i = 0; i < JointTmpBlocks.Count; i++)
        {
            //�Ō�̗v�f��0�Ԗڂ̗v�f�ƌ���
            if (i >= JointTmpBlocks.Count - 1)
            {
                FixedJoint2D fixedJoint = JointTmpBlocks[i].gameObject.AddComponent<FixedJoint2D>();
                fixedJoint.connectedBody = JointTmpBlocks[0].GetComponent<Rigidbody2D>();
            }
            //���̔ԍ������I�u�W�F�N�g�ƌ���
            else
            {
                FixedJoint2D fixedJoint = JointTmpBlocks[i].gameObject.AddComponent<FixedJoint2D>();
                fixedJoint.connectedBody = JointTmpBlocks[i + 1].GetComponent<Rigidbody2D>();
            }
            JointTmpBlocks[i].SetParent(tmpField.transform);
        }

        //�ꎞ�I�ȃ��X�g���g�p���Ďq�I�u�W�F�N�g�̐e��ύX
        foreach (Transform block in JointTmpBlocks)
        {
            block.SetParent(completedField.transform);
        }
        existTmpBlocks = false;
    }
    public static void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
