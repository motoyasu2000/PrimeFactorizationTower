using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchBlock : MonoBehaviour
{
    //���͊Ǘ�
    bool isDragging = false; //�h���b�O���Ă��邩�ǂ���
    Vector3 touchPosition; //���݃^�b�`���Ă���ʒu
    Transform draggedObject = null; //���ݑI�����Ă���Q�[���I�u�W�F�N�g���i�[����ϐ��@Update����Raycast�𖈕b�s���Ă���̂ŁA
                                    //�I������Q�[���I�u�W�F�N�g���ύX����Ȃ��悤�Ƀh���b�O���̃I�u�W�F�N�g�݂̂��擾����悤�ɂ��Ă���B
    //�u���b�N�̏���
    BlockInfo blockInfo;
    Network network;
    GraphicRaycaster graphicRaycaster;
    SingleGenerateManager singleGenerateManager; //�Q�[���I�u�W�F�N�g���P��ł��邱�Ƃ�ۏ؂��邽�߂̃N���X
    GameObject primeNumberGeneratingPoint; //�Q�[���I�u�W�F�N�g�𐶐�����ꏊ�������Q�[���I�u�W�F�N�g  
    GameObject blockField;
    GameObject afterField;

    //UI����
    GameObject canvas;
    EventSystem eventSystem;

    private void Start()
    {
        //������
        blockInfo = GetComponent<BlockInfo>();
        primeNumberGeneratingPoint = GameObject.Find("PrimeNumberGeneratingPoint");
        singleGenerateManager = primeNumberGeneratingPoint.GetComponent<SingleGenerateManager>();
        blockField = GameObject.Find("BlockField");
        afterField = blockField.transform.Find("AfterField").gameObject;
        network = GameObject.Find("Network").GetComponent<Network>();
        canvas = GameObject.Find("Canvas");
        eventSystem = FindObjectOfType<EventSystem>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
    }

    void Update()
    {
        //Input.touches�̓t���[�����Ƃ̑S�Ẵ^�b�`���擾����(��ʂɓ����ɐG�ꂽ�S�Ă̎w�A�P�t���[���ł̒�����touch)
        foreach (Touch touch in Input.touches)
        {
            //����͕����^�b�`���󂯓��ꂸ�A��̎w�̓��݂͂̂��󂯓����
            if (touch.fingerId == 0)
            {
                touchPosition = GetTouchWorldPosition(touch);

                // �^�b�`�̏�Ԃɉ����ď���
                switch (touch.phase)
                {
                    //�^�b�`�����u�Ԃł����
                    case TouchPhase.Began:
                        if (!isDragging)
                        {
                            HandleTouchBegan(touch);
                        }
                        break;

                    //�^�b�`���Ă���Ԃł����
                    case TouchPhase.Moved:
                        if (isDragging)
                        {
                            HandleTouchMoved(touch);
                        }
                        break;

                    //�^�b�`���I��点���Ȃ�(�w�𗣂�)
                    case TouchPhase.Ended:
                        if (isDragging)
                        {
                            HandleTouchEnded(touch);
                        }
                        break;
                }
            }
        }
    }

    //�^�b�`�����X�N���[�����W�����[���h���W�ɕϊ����ĕԂ��֐�
    Vector3 GetTouchWorldPosition(Touch touch)
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
        touchPosition.z = Camera.main.nearClipPlane;
        return touchPosition;
    }

    void HandleTouchBegan(Touch touch)
    {
        Vector3 upCondition_view = new Vector3(0, 0.3f, touchPosition.z - Camera.main.transform.position.z);
        Vector3 upCondition = Camera.main.ViewportToWorldPoint(upCondition_view);
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = touch.position; //�X�N���[�����W�Ŏw�肷�邱�Ƃɒ���
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            GameObject hitGameObject = result.gameObject;
            Debug.Log(hitGameObject);
            if (hitGameObject != null && !hitGameObject.CompareTag("UnderClickable")) return;
        }

        if (touchPosition.y < upCondition.y) return;
        if (singleGenerateManager.GetSingleGameObject() == null) return;
        draggedObject = singleGenerateManager.GetSingleGameObject().transform;
        draggedObject.position = new Vector3(touchPosition.x, primeNumberGeneratingPoint.transform.position.y, touchPosition.z); //�u���b�Nx���W���^�b�`���Ă�����W��
        isDragging = true;
    }

    void HandleTouchMoved(Touch touch)
    {
        draggedObject.position = new Vector3(touchPosition.x, primeNumberGeneratingPoint.transform.position.y, touchPosition.z); //�u���b�Nx���W���^�b�`���Ă�����W��
    }

    void HandleTouchEnded(Touch touch)
    {
        isDragging = false;
        draggedObject = null;
        singleGenerateManager.SetSingleGameObject(null); //���̃u���b�N��singleGameObject�ɓ������܂܂ɂ��Ă���ƁA�{�^���������ꂽ�u�Ԃ�Destroy���Ă΂�Ă��܂��B
        this.enabled = false;
        this.tag = "PrimeNumberBlock";
        gameObject.layer = LayerMask.NameToLayer("PrimeNumberBlock"); //���C���[��ύX���邱�Ƃɂ��A���߂đ��̃u���b�N�ƏՓ˂���悤�ɂȂ�B
        blockInfo.ChangeDynamic(); //�d�͂̉e�����󂯂�悤�ɂ���B
        blockInfo.EnableCollider(); //�Q�[���I�u�W�F�N�g�̗����n�_�����o��������̕`��̍ۂɈꎞ�I�ɃR���C�_�[��񊈐�������̂ŁA�����ŃR���C�_�[�𕜊�������B
        gameObject.transform.parent = afterField.transform;
        network.AddNode(gameObject);
    }
}
