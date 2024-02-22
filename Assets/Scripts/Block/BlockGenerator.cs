using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockGenerator : MonoBehaviour
{
    int primeNumber;
    [SerializeField] GameObject primeNumberGeneratingPoint;
    GameObject blockField;
    GameObject beforeField;
    SingleGenerateManager singleGenerateManager;
    TextMeshProUGUI text;
    protected GameManager gameManager;
    static int IDCounter = 0;


    private void Start()
    {
        singleGenerateManager = primeNumberGeneratingPoint.GetComponent<SingleGenerateManager>();
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        text.text = primeNumber.ToString();
        blockField = GameObject.Find("BlockField");
        beforeField = blockField.transform.Find("BeforeField").gameObject;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    //�u���b�N�𐶐�����֐��̈����𐧌䂷��֐��B
    public void GenerateBlock()
    {
        if (gameManager.GetCompleteNumberFlag()) return; //�f�����������Ă����Ԃł���΃��^�[��
        HundleGenerateBlock(primeNumber);
    }

    //�����ŗ^����ꂽ���l�ɍ��킹�ău���b�N�𐶐�����֐��B���̃u���b�N�̏��(ID��f���Ȃ�)���ǉ�����B
    void HundleGenerateBlock(int primeNumber)
    {
        //�Q�[���I�u�W�F�N�g�̐����Ƃ��̏������C���X�^���X�̎擾
        GameObject generateObject = Instantiate(GetPrimeNumberBlock(primeNumber), primeNumberGeneratingPoint.transform.position, GetPrimeNumberBlock(primeNumber).transform.rotation, beforeField.transform);
        BlockInfo blockInfo = generateObject.GetComponent<BlockInfo>();

        //�u���b�N�̎��f���̐ݒ�ƃe�L�X�g�̐؂�ւ�
        blockInfo.SetMyNumber(primeNumber);
        blockInfo.SetText();

        //ID�̐ݒ�
        blockInfo.SetID(IDCounter);
        generateObject.name = $"Block{primeNumber}_{IDCounter}";
        IDCounter++;

        singleGenerateManager.SetSingleGameObject(generateObject);//���������Q�[���I�u�W�F�N�g�̏��𐶐��ł���Q�[���I�u�W�F�N�g�͏�ɒP��ł���悤�ɊǗ����郁�\�b�h�ɓ����B
    }
    GameObject GetPrimeNumberBlock(int primeNumber)
    {
        //Debug.Log("Block" + primeNumber.ToString());
        return (GameObject)Resources.Load("Block" + primeNumber.ToString());
    }

    public void SetPrimeNumber(int newPrimeNumber)
    {
        primeNumber = newPrimeNumber;
    }
}
