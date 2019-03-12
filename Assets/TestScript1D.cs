using UnityEngine;
using System.Collections;

public class TestScript1D : MonoBehaviour
{
    public GameObject uPrefab;

    private int mMapWidth = 128;

    private GameObject[] mMap;
    private float[] mSpeedX;

    private float mTimeDif;

    float imp = 1;

    // Use this for initialization
    void Start()
    {
        mMap = new GameObject[mMapWidth];
        float y = 0;
        mSpeedX = new float[mMapWidth];
        for (int x = 0; x < mMapWidth; x++)
        {
            mMap[x] = (GameObject)Instantiate(uPrefab, new Vector3(x, 0, y), Quaternion.identity);
        }

        mTimeDif = 0.5f;// Time.deltaTime;
        //SetSpeedX(5, 0, -2.0f);
        SetHeight(64, 2);
        SetHeight(64, 2);

    }
    private bool IsAbsorber(int x)
    {
        if( x == 8)
        {
            return true;
        }
        return false;
    }
    private float GetHeight(int x)
    {
        return mMap[x].transform.position.y;
    }
    private void SetHeight(int x, float val)
    {
        Vector3 pos = mMap[x].transform.position;
        pos.y = val;
        mMap[x].transform.position = pos;
    }
    private float GetSpeedX(int x)
    {
        return mSpeedX[x];
    }
    private void SetSpeedX(int x, float value)
    {
        mSpeedX[x] = value;
    }
    private void CalculateSpeedChange()
    {
        for (int x = 0; x < mMapWidth - 1; x++)
        {
            float height = GetHeight(x);
            float heightx = GetHeight(x + 1);





            float speedx = GetSpeedX(x);
            speedx = speedx + (height - heightx) * mTimeDif;
            SetSpeedX(x, speedx);
        }
    }

    private void CalculateHeightChange()
    {
        for (int x = 1; x < mMapWidth; x++)
        {
            float height = GetHeight(x);
            float speedxp = GetSpeedX(x);;
            float speedxn = GetSpeedX(x - 1);




            height = height - (speedxp - speedxn) * mTimeDif;
            SetHeight(x, height);
        }
        
    }

    void Update()
    {
        SetSpeedX(7, 0);
        SetHeight(7, 0);
        CalculateSpeedChange();
        CalculateHeightChange();
    }
}