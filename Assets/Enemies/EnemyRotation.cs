using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;



    [SerializeField]
    public class EnemyRotation
    {


        public float speed;

        // Use this for initialization

       


        public static Quaternion Rotate(Transform Target, Transform Character)
        {
         //   Debug.Log("Rotating");
            Vector3 TargetPos = Target.transform.position;
            Vector3 CharPos = Character.transform.position;
            //Debug.Log("Enemy tranform " + EnemyPos);
            //  Debug.Log("Char transform " + CharPos);

            float yRot = CalculateDegree((CharPos.x - TargetPos.x), (CharPos.z - TargetPos.z), true);

            return (Quaternion.Euler(0, yRot, 0));
        }

        private static float CalculateDegree(float c, float a, bool Yrot)
        {
            int Quadrient = 0;
            // Debug.Log("c " + c);
            // Debug.Log("a " + a);
            if (-c >= 0 && -a >= 0)
            {
                Quadrient = 1;
            }
            else if (-c >= 0 && -a < 0)
            {
                Quadrient = 2;
            }
            else if (-c < 0 && -a < 0)
            {
                Quadrient = 3;
            }
            else if (-c < 0 && -a >= 0)
            {
                Quadrient = 4;
            }
            c = Mathf.Abs(c);
            a = Mathf.Abs(a);



            float b = Mathf.Sqrt(Mathf.Pow(c, 2) + Mathf.Pow(a, 2));
            float Numerator = Mathf.Pow(a, 2) + Mathf.Pow(b, 2) - Mathf.Pow(c, 2);
            float Denominator = 2 * b * a;

            float Degree = Mathf.Rad2Deg * (Mathf.Acos(Numerator / Denominator));

            //Debug.Log("Angle " + Degree);

            if (Yrot)
            {
                if (Quadrient == 1)
                {
                    return (Degree);
                }
                else if (Quadrient == 2)
                {
                    return (180 - Degree);
                }
                else if (Quadrient == 3)
                {
                    return (180 + Degree);
                }
                else if (Quadrient == 4)
                {
                    return (360 - Degree);
                }
                else
                {
                    Debug.LogError("Incorrect Quadrient found");
                    return (0);
                }
            }
            else
            {
                //  Debug.Log(Degree);
                if (Quadrient == 1)
                {
                    return (90 - Degree);
                }
                else if (Quadrient == 2)
                {
                    return (90 - Degree);
                }
                else if (Quadrient == 3)
                {
                    return (90 - Degree);
                }
                else if (Quadrient == 4)
                {
                    return (90 - Degree);
                }
                else
                {
                    Debug.LogError("Incorrect Quadrient found");
                    return (0);
                }
            }
        }
    }


