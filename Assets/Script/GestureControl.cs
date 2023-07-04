/***
*		���⣺���ƿ����������ת����
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Control
{
    public class GestureControl : MonoBehaviour
    {
        #region   ��������
        //���ڿ��Ʋ�����ת������
        public Transform target;
        //����ϵ��
        public float distance = 12.0F;
        //���һ����ƶ��ٶ�
        public float xSpeed = 200.0F;
        public float ySpeed = 200.0F;
        public float mSpeed = 10;
        //����˥��
        public bool needDamping = true;
        float damping = 5.0f;
        //��������ϵ��
        public float yMinLimit = 3F;
        public float yMaxLimit = 60F;
        //����ͷ��λ��
        public float x = 0.0F;
        public float y = 0.0F;
        //��¼��һ���ֻ�����λ���ж��û�������Ŵ�����С����
        private Vector2 oldPosition1;
        private Vector2 oldPosition2;
        #endregion

        //public GameObject moveObj;          //����ң���ƶ�������

        public void SetTarget(GameObject go)
        {
            target = go.transform;
        }

        //��ʼ����Ϸ��Ϣ����
        void Start()
        {
            //this.transform.position = new Vector3(-52.05F,4.54F,-3.61F);

            Vector3 angles = transform.eulerAngles;
            x = angles.y;//����-7F�ǵ����������ʼλ�õ�ƫ����
            y = angles.x;//����-8F�ǵ����������ʼλ�õ�ƫ����

            //// Make the rigid body not change rotation
            //if (this.GetComponent<Rigidbody>())
            //    this.GetComponent<Rigidbody>().freezeRotation = true;


        }

        void Update()
        {
            if (target)
            {
                //�жϴ�������Ϊ���㴥��
                if (Input.touchCount == 1)
                {

                    //��������Ϊ�ƶ�����
                    if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        //���ݴ��������X��Yλ��
                        x += Input.GetAxis("Mouse X") * xSpeed * 0.02F;
                        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02F;

                        y = ClampAngle(y, yMinLimit, yMaxLimit);

                        print(x);
                        print(y);

                        //moveObj.transform.position = Camera.main.transform.position;
                        //moveObj.transform.rotation = Camera.main.transform.rotation;
                    }
                }

                //�жϴ�������Ϊ��㴥��
                if (Input.touchCount > 1)
                {
                    //ǰ��ֻ��ָ�������Ͷ�Ϊ�ƶ�����
                    if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                    {
                        //�������ǰ���㴥�����λ��
                        Vector3 tempPosition1 = Input.GetTouch(0).position;
                        Vector3 tempPosition2 = Input.GetTouch(1).position;
                        //����������Ϊ�Ŵ󣬷��ؼ�Ϊ��С
                        if (IsEnlarge(oldPosition1, oldPosition2, tempPosition1, tempPosition2))
                        {
                            //�Ŵ�ϵ������10�Ժ���������Ŵ�
                            //����������Ǹ�������Ŀ�е�ģ�Ͷ����ڵģ���ҿ����Լ������޸�
                            if (distance > 10F)
                            {
                                distance -= 1F;
                            }
                        }
                        else
                        {
                            //��Сϵ������100�����������С
                            //����������Ǹ�������Ŀ�е�ģ�Ͷ����ڵģ���ҿ����Լ������޸�
                            if (distance < 100F)
                            {
                                distance += 1F;
                            }
                        }
                        //������һ�δ������λ�ã����ڶԱ�
                        oldPosition1 = tempPosition1;
                        oldPosition2 = tempPosition2;
                    }
                }

                Quaternion rotation = Quaternion.Euler(y, x, 0.0f);

                Vector3 disVector = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * disVector + target.position;

                //adjust the camera
                if (needDamping)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * damping);
                    transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * damping);
                }
                else
                {
                    transform.position = position;
                    transform.rotation = rotation;
                }
            }
        }

        //����������Ϊ�Ŵ󣬷��ؼ�Ϊ��С
        bool IsEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
        {
            //����������һ�δ��������λ���뱾�δ��������λ�ü�����û�������
            float leng1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
            float leng2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));
            if (leng1 < leng2)
            {
                //�Ŵ�����
                return true;
            }
            else
            {
                //��С����
                return false;
            }
        }

        //���巭���Ŀ���
        static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;

            return Mathf.Clamp(angle, min, max);
        }
    }//Class_end
}
