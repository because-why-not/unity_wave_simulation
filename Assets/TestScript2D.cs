using UnityEngine;
using System.Collections;

public class TestScript2D : MonoBehaviour
{
    public GameObject uPrefab;

    private int mMapWidth = 65;
    private int mMapHeight = 65;

    private GameObject[][] mMap;
    private float[][] mHeight;
    private float[][] mSpeedX;
    private float[][] mSpeedY;
    private float[][] mHeight2;
    private float[][] mSpeedX2;
    private float[][] mSpeedY2;

    private int[][] mType;

    private float mHeightDifToSpeedDif = 1.0f;
    private float mSpeedDifToHeightDif = 0.25f;

    private float mTimeDif;


    float waveDuration = -1;

    float imp = 1;

	// Use this for initialization
	void Start ()
    {
        Debug.Log("Press space to create a wave (keep button pressed)");

        mMap = new GameObject[mMapHeight][];
        InitToZero(ref mHeight);
        InitToZero(ref mSpeedX);
        InitToZero(ref mSpeedY);
        InitToZero(ref mHeight2);
        InitToZero(ref mSpeedX2);
        InitToZero(ref mSpeedY2);

        mType = new int[mMapHeight][];
	    for(int y = 0; y < mMapHeight; y++)
        {
            mMap[y] = new GameObject[mMapWidth];
            mType[y] = new int[mMapWidth];
            for (int x = 0; x < mMapWidth; x++)
            {
                var v = (GameObject)Instantiate(uPrefab, new Vector3(x, 0, y), Quaternion.identity);
                v.name = x + "/" + y;
                mMap[y][x] = v;
            }
        }



        for (int x = 0; x < mMapWidth; x++)
        {
            mType[0][x] = 1;
        }
        for (int x = 0; x < mMapWidth; x++)
        {
            mType[mMapHeight - 10][x] = 1;
        }

        for (int y = 0; y < mMapHeight; y++)
        {
            mType[y][0] = 1;

        }
        for (int y = 0; y < mMapHeight; y++)
        {
            mType[y][mMapWidth - 1] = 1;
        }
        mTimeDif = 1.00f;// Time.deltaTime;

        SetHeight(1, 1, 60);

	}

    private void InitToZero(ref float [][] arr)
    {
        arr = new float[mMapHeight][];
        for (int y = 0; y < mMapHeight; y++)
        {
            arr[y] = new float[mMapWidth];
        }
    }

    private float GetHeight(int x, int y)
    {
        Border(ref x, ref y);
        return mHeight[y][x];
    }
    private float GetSpeedX(int x, int y)
    {
        Border(ref x, ref y);
        return mSpeedX[y][x];
    }
    private float GetSpeedY(int x, int y)
    {
        Border(ref x, ref y);
        return mSpeedY[y][x];
    }
    private void SetHeight(int x, int y, float val)
    {
        Border(ref x, ref y);
        mHeight2[y][x] = val;
    }

    private void SyncOutputHeight()
    {

        for (int y = 0; y < mMapHeight; y++)
        {
            for (int x = 0; x < mMapWidth; x++)
            {
                Vector3 pos = mMap[y][x].transform.position;
                pos.y = mHeight[y][x];
                mMap[y][x].transform.position = pos;
            }
        }
    }
    private void Border(ref int x, ref int y)
    {
        Clamp(ref x, ref y);

    }
    private void Circle(ref int x, ref int y)
    {
        x = (x + mMapWidth) % mMapWidth;
        y = (y + mMapHeight) % mMapHeight;


    }
    private void Clamp(ref int x, ref int y)
    {

        if (x < 0)
            x = 0;
        if (x > mMapWidth - 1)
            x = mMapWidth - 1;
        if (y < 0)
            y = 0;
        if (y > mMapHeight - 1)
            y = mMapHeight - 1;

    }

    private void SetSpeedX(int x, int y, float value)
    {
        Border(ref x, ref y);
        mSpeedX2[y][x] = value;
    }


    private void SetSpeedY(int x, int y, float value)
    {
        Border(ref x, ref y);
        mSpeedY2[y][x] = value;
    }


    private bool IsInBounds(int x, int y)
    {
        if(x >= 0 && x < mMapWidth && y >= 0 && y < mMapHeight)
            return true;
        return false;
    }

    private bool IsAbsorber(int x, int y)
    {
        if (IsInBounds(x, y) && mType[y][x] == 1)
        {
            return true;
        }
        return false;
    }
    
    private bool IsSpeedXAbsorber(int x, int y)
    {
        return IsAbsorber(x, y) || IsAbsorber(x + 1, y);
    }
    private bool IsSpeedYAbsorber(int x, int y)
    {
        return IsAbsorber(x, y) || IsAbsorber(x, y + 1);
    }

    private void CalculateSpeedChange()
    {
	    for(int y = 0; y < mMapHeight; y++)
        {
            for (int x = 0; x < mMapWidth; x++)
            {
                {
                    float height = GetHeight(x, y);
                    float heightx = GetHeight(x + 1, y);

                    float speedx = GetSpeedX(x, y);
                    if (IsSpeedXAbsorber(x, y))
                    {
                        speedx = 0;
                    }
                    speedx = speedx + (height - heightx) * mHeightDifToSpeedDif * mTimeDif;
                    SetSpeedX(x, y, speedx);


                    float heighty = GetHeight(x, y + 1);
                    float speedy = GetSpeedY(x, y);
                    if (IsSpeedYAbsorber(x, y))
                    {
                        speedy = 0;
                    }
                    speedy = speedy + (height - heighty) * mHeightDifToSpeedDif * mTimeDif;
                    SetSpeedY(x, y, speedy);
                }
            }
        }
    }

    private void CalculateHeightChange()
    {
        for (int y = 0; y < mMapHeight; y++)
        {
            for (int x = 0; x < mMapWidth; x++)
            {
                if (IsAbsorber(x, y))
                {
                    SetHeight(x, y, 0);
                    continue;
                }
                float height = GetHeight(x, y);


                float speedxp = GetSpeedX(x, y);
                float speedyp = GetSpeedY(x, y);
                float speedxn = GetSpeedX(x - 1, y);
                float speedyn = GetSpeedY(x, y - 1);
                //height = height * 0.98f;
                height = height - (speedxp - speedxn) * mSpeedDifToHeightDif * mTimeDif;
                height = height - (speedyp - speedyn) * mSpeedDifToHeightDif * mTimeDif;
                SetHeight(x, y, height);
            }
        }
    }

	// Update is called once per frame
	void Update ()
    {
        //for (int y = 0; y < mMapHeight; y++)
        //{
        //    for (int x = 0; x < mMapWidth; x++)
        //    {
        //        if (IsAbsorber(x, y))
        //        {
        //            mHeight[y][x] = 0;
        //        }
        //        if (IsSpeedXAbsorber(x, y))
        //        {
        //            mSpeedX[y][x] = 0;
        //        }
        //        if (IsSpeedYAbsorber(x, y))
        //        {
        //            mSpeedY[y][x] = 0;
        //        }
        //    }
        //}


        CalculateSpeedChange();
        Swap(ref mSpeedX2, ref mSpeedX);
        Swap(ref mSpeedY2, ref mSpeedY);
        CalculateHeightChange();

        if(waveDuration >= 0)
        {

            int x = 8;
            int y = mMapHeight / 2;//8;
            //SetHeight(x, y, GetHeight(x, y) + Mathf.Sin((time) * 8) * 8);
            x = 6;//mMapWidth - 1 - 8;
            y = 8;//mMapHeight / 2;// -1 - 8;
            SetHeight(x, y, mHeight2[y][x] + Mathf.Sin((waveDuration) * 8) * 8);
        }
        Swap(ref mHeight2, ref mHeight);
        SyncOutputHeight();

        if(Input.GetKey(KeyCode.Space))
        {
            if (waveDuration == -1)
            {
                waveDuration = 0;
            }

            waveDuration += Time.deltaTime;

        }else
        {
            if (waveDuration <= 0)
                waveDuration = -1;
            else
            {
                waveDuration -= Time.deltaTime;
            }
        }
            
	}
    private void Swap(ref float[][] a, ref float[][] b)
    {
        float[][] c = b;
        b = a;
        a = c;
    }
}