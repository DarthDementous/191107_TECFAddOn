using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberScroller : MonoBehaviour
{
    public int CurrentNum
    {
        get
        {
            return _currentNum;
        }
        set
        {
            _currentNum = value;
        }
    }
    public int MaxFrames  = 10;

    //[HideInInspector]
    //public float ScrollSpeed = 1;

    int _currentNum;
    float _currOffsetY;
    float _startOffsetY;
    Material _mat;

    private void Awake()
    {
        // Copy material so individual modifications can be made without affecting other objects
        _mat = Instantiate(GetComponent<Image>().material);
        GetComponent<Image>().material = _mat;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set to initial number position
        _currOffsetY = GetOffsetForNum(CurrentNum);
        _mat.SetTextureOffset("_MainTex", new Vector2(0, _currOffsetY));
    }

    /**
     * @brief Set the new target number for the scroller.
     * @param a_num is the target number
     * @param a_isInstant determines whether it instantly snaps towards the number or moves gradually towards it
     * */
    public void SetTargetNum(int a_num, bool a_isInstant = false)
    {
        CurrentNum = a_num;

        if (a_isInstant)
        {
            _currOffsetY = GetOffsetForNum(CurrentNum);
            _mat.SetTextureOffset("_MainTex", new Vector2(0, _currOffsetY));
        }
        else
        {
            StartCoroutine(MoveToNum(a_num));
        }
    }

    IEnumerator MoveToNum(int a_num)
    {
        int startNum            = GetNumFromOffset(_currOffsetY);
        float startOffsetY      = _currOffsetY;
        float targetNumOffset   = GetOffsetForNum(a_num);

        float targetOffsetY = targetNumOffset;

        /// Special cases
        // 0 to 9
        if (startNum == 0 && a_num == 9)
        {
            targetOffsetY = -0.1f;
        }

        // 9 to 0
        if (startNum == 9 && a_num == 0)
        {
            targetOffsetY = 1.0f;
        }

        float t = 0.0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 1.01f;

            _currOffsetY = Mathf.MoveTowards(startOffsetY, targetOffsetY, t);
            _mat.SetTextureOffset("_MainTex", new Vector2(0, _currOffsetY));

            yield return null;
        }

        /// Special cases
        // Reached 9 from 0
        if (startNum == 0 && a_num == 9)
        {
            _currOffsetY = 0.9f;
        }

        // Reached 0 from 9
        if (startNum == 9 && a_num == 0)
        {
            _currOffsetY = 0f;
        }

        _mat.SetTextureOffset("_MainTex", new Vector2(0, _currOffsetY));
    }

    float GetOffsetForNum(int a_num)
    {
        return a_num / (float)MaxFrames;
        //return (1.0f - (a_num / (float)MaxFrames) - (1f / MaxFrames));
    }

    int GetNumFromOffset(float a_offset)
    {
        return Mathf.RoundToInt(a_offset * MaxFrames);
        //return Mathf.RoundToInt(MaxFrames * -a_offset + MaxFrames - 1f);
    }
}
