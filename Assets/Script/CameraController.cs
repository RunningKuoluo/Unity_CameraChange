using UnityEngine;
using System;
using System.Collections;

public class CameraController : MonoBehaviour
{
    /// <summary>
    /// �Ƿ�����תX��
    /// </summary>
    public bool canRotation_X = true;
    /// <summary>
    /// �Ƿ�����תY��
    /// </summary>
    public bool canRotation_Y = true;
    /// <summary>
    /// �Ƿ�������
    /// </summary>
    public bool canScale = true;

    #region ����
    /// <summary>
    /// ��������Ŀ��
    /// </summary>
    public Transform target;

    /// <summary>
    /// ��갴ť��ָ��͹��ֵ�����
    /// </summary>
    public MouseSettings mouseSettings = new MouseSettings(0, 10, 10);

    /// <summary>
    /// �Ƕȷ�Χ����
    /// </summary>
    public Range angleRange = new Range(-90, 90);

    /// <summary>
    /// ���뼫�޷�Χ
    /// </summary>
    public Range distanceRange = new Range(0, 10);

    /// <summary>
    /// ƽ���ϵľ�������
    /// </summary>
    public PlaneArea PlaneArea = new PlaneArea();

    /// <summary>
    /// �������׼Ŀ��
    /// </summary>
    public AlignTarget AlignTarget = new AlignTarget();

    /// <summary>
    /// ���������ƶ�����ת�ķ�Χ.
    /// </summary>
    [Range(0, 10)]
    public float damper = 5;

    /// <summary>
    /// �����ǰ�ĽǶ�.
    /// </summary>
    public Vector2 CurrentAngles { protected set; get; }

    /// <summary>
    /// �����Ŀ��ĵ�ǰ����.
    /// </summary>
    public float CurrentDistance { protected set; get; }

    /// <summary>
    /// ���Ŀ��Ƕ�.
    /// </summary>
    protected Vector2 targetAngles;

    /// <summary>
    /// �����Ŀ��ľ���
    /// </summary>
    protected float targetDistance;
    #endregion

    #region Protected Method
    protected virtual void Start()
    {
        CurrentAngles = targetAngles = transform.eulerAngles;
        CurrentDistance = targetDistance = Vector3.Distance(transform.position, target.position);
    }

    protected virtual void LateUpdate()
    {
#if UNITY_EDITOR
        AroundByMouseInput();
#elif UNITY_STANDALONE_WIN
                AroundByMobileInput();
#elif UNITY_ANDROID || UNITY_IPHONE
                AroundByMobileInput();
#endif
    }

    //��¼��һ���ֻ�����λ���ж��û�������Ŵ�����С����  
    private Vector2 oldPosition1;
    private Vector2 oldPosition2;

    //�Ƿ�ָ����
    private bool m_IsSingleFinger;

    /// <summary>
    /// �ƶ��ˣ�Winƽ�壩
    /// </summary>
    protected void AroundByMobileInput()
    {
        if (Input.touchCount == 1)
        {
            if (Input.touches[0].phase == TouchPhase.Moved)
            {
                //�ֻ��˿ɵ��ô˴��루window����ֻ�ܻ�ȡ��겻�ܻ�ȡ�����壩
                targetAngles.y += Input.GetAxis("Mouse X") * mouseSettings.pointerSensitivity;
                targetAngles.x -= Input.GetAxis("Mouse Y") * mouseSettings.pointerSensitivity;

                //window���Կɻ�ȡ���ƶ���Ҳ����ʹ�ã�
                targetAngles.y += Input.touches[0].deltaPosition.x * Time.deltaTime * 5;
                targetAngles.x -= Input.touches[0].deltaPosition.y * Time.deltaTime * 5;

                //��Χ
                targetAngles.x = Mathf.Clamp(targetAngles.x, angleRange.min, angleRange.max);
            }
            //���ָ��
            m_IsSingleFinger = true;
        }

        //������
        if (canScale)
        {
            if (Input.touchCount > 1)
            {

                //�������ǰ���㴥�����λ��  
                if (m_IsSingleFinger)
                {
                    oldPosition1 = Input.GetTouch(0).position;
                    oldPosition2 = Input.GetTouch(1).position;
                }
                if (Input.touches[0].phase == TouchPhase.Moved && Input.touches[1].phase == TouchPhase.Moved)
                {
                    var tempPosition1 = Input.GetTouch(0).position;
                    var tempPosition2 = Input.GetTouch(1).position;

                    float currentTouchDistance = Vector3.Distance(tempPosition1, tempPosition2);
                    float lastTouchDistance = Vector3.Distance(oldPosition1, oldPosition2);

                    //�����ϴκ����˫ָ����֮��ľ�����  
                    //Ȼ��ȥ����������ľ���  
                    targetDistance -= (currentTouchDistance - lastTouchDistance) * Time.deltaTime * mouseSettings.wheelSensitivity;

                    //������һ�δ������λ�ã����ڶԱ�  
                    oldPosition1 = tempPosition1;
                    oldPosition2 = tempPosition2;
                    m_IsSingleFinger = false;
                }
            }

        }

        targetDistance = Mathf.Clamp(targetDistance, distanceRange.min, distanceRange.max);

        //����
        CurrentAngles = Vector2.Lerp(CurrentAngles, targetAngles, damper * Time.deltaTime);
        CurrentDistance = Mathf.Lerp(CurrentDistance, targetDistance, damper * Time.deltaTime);


        if (!canRotation_X) targetAngles.y = 0;
        if (!canRotation_Y) targetAngles.x = 0;

        //ʵʱλ����ת
        transform.rotation = Quaternion.Euler(CurrentAngles);
        //ʵʱλ���ƶ�
        transform.position = target.position - transform.forward * CurrentDistance;

    }

    /// <summary>
    /// ���ͨ���������Χ��Ŀ��
    /// </summary>
    protected void AroundByMouseInput()
    {
        if (Input.GetMouseButton(mouseSettings.mouseButtonID))
        {
            //���ָ��
            targetAngles.y += Input.GetAxis("Mouse X") * mouseSettings.pointerSensitivity;
            targetAngles.x -= Input.GetAxis("Mouse Y") * mouseSettings.pointerSensitivity;

            //��Χ
            targetAngles.x = Mathf.Clamp(targetAngles.x, angleRange.min, angleRange.max);
        }

        //������
        if (canScale)
        {
            targetDistance -= Input.GetAxis("Mouse ScrollWheel") * mouseSettings.wheelSensitivity;

        }
        targetDistance = Mathf.Clamp(targetDistance, distanceRange.min, distanceRange.max);

        //����
        CurrentAngles = Vector2.Lerp(CurrentAngles, targetAngles, damper * Time.deltaTime);
        CurrentDistance = Mathf.Lerp(CurrentDistance, targetDistance, damper * Time.deltaTime);


        if (!canRotation_X) targetAngles.y = 0;
        if (!canRotation_Y) targetAngles.x = 0;


        //ʵʱλ����ת
        transform.rotation = Quaternion.Euler(CurrentAngles);
        //ʵʱλ���ƶ�
        transform.position = target.position - transform.forward * CurrentDistance;
    }
    #endregion
}

[Serializable]
public struct MouseSettings
{
    /// <summary>
    /// ��갴����ID
    /// </summary>
    public int mouseButtonID;

    /// <summary>
    /// ���ָ���������.
    /// </summary>
    public float pointerSensitivity;

    /// <summary>
    /// �����ֵ�������
    /// </summary>
    public float wheelSensitivity;

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="mouseButtonID">��갴ť��ID</param>
    /// <param name="pointerSensitivity">���ָ���������</param>
    /// <param name="wheelSensitivity">�����ֵ�������</param>
    public MouseSettings(int mouseButtonID, float pointerSensitivity, float wheelSensitivity)
    {
        this.mouseButtonID = mouseButtonID;
        this.pointerSensitivity = pointerSensitivity;
        this.wheelSensitivity = wheelSensitivity;
    }
}

/// <summary>
/// ��Χ����С�����
/// </summary>
[Serializable]
public struct Range
{
    /// <summary>
    /// ��Χ����Сֵ
    /// </summary>
    public float min;

    /// <summary>
    /// ��Χ�����ֵ
    /// </summary>
    public float max;

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="min">��Χ����Сֵ</param>
    /// <param name="max">��Χ�����ֵ</param>
    public Range(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}

/// <summary>
/// ƽ���ϵľ�������
/// </summary>
[Serializable]
public struct PlaneArea
{
    /// <summary>
    /// ��������
    /// </summary>
    public Transform center;

    /// <summary>
    /// ������
    /// </summary>
    public float width;

    /// <summary>
    /// ���򳤶�
    /// </summary>
    public float length;

    /// <summary>
    /// ƽ������_���캯��
    /// </summary>
    /// <param name="center">��������</param>
    /// <param name="width">������</param>
    /// <param name="length">���򳤶�</param>
    public PlaneArea(Transform center, float width, float length)
    {
        this.center = center;
        this.width = width;
        this.length = length;
    }
}

/// <summary>
/// �������׼Ŀ��
/// </summary>
[Serializable]
public struct AlignTarget
{
    /// <summary>
    /// ��׼Ŀ������
    /// </summary>
    public Transform center;

    /// <summary>
    /// ����Ƕ�
    /// </summary>
    public Vector2 angles;

    /// <summary>
    /// �����Ŀ�����ĵľ���
    /// </summary>
    public float distance;

    /// <summary>
    /// �Ƕȷ�Χ����
    /// </summary>
    public Range angleRange;

    /// <summary>
    /// ���뷶Χ����
    /// </summary>
    public Range distanceRange;

    /// <summary>
    /// ��׼Ŀ��_���캯��
    /// </summary>
    /// <param name="center">��׼Ŀ������</param>
    /// <param name="angles">����Ƕ�</param>
    /// <param name="distance">�����Ŀ�����ĵľ���</param>
    /// <param name="angleRange">�Ƕȷ�Χ����</param>
    /// <param name="distanceRange">���뷶Χ����</param>
    public AlignTarget(Transform center, Vector2 angles, float distance, Range angleRange, Range distanceRange)
    {
        this.center = center;
        this.angles = angles;
        this.distance = distance;
        this.angleRange = angleRange;
        this.distanceRange = distanceRange;
    }
}

