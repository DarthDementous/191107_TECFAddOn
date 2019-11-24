using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberScroller : MonoBehaviour
{
    public int CurrentNum = 0;
    public int MaxFrames  = 10;

    //[HideInInspector]
    //public float ScrollSpeed = 1;

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

    // Update is called once per frame
    void Update()
    {
        float targetOffsetY = GetOffsetForNum(CurrentNum);

        _currOffsetY = (CurrentNum != 9 && CurrentNum != 0) ? Mathf.MoveTowards(_currOffsetY, targetOffsetY, Time.deltaTime) : targetOffsetY;
        _mat.SetTextureOffset("_MainTex", new Vector2(0, _currOffsetY));
    }

    float GetOffsetForNum(int a_num)
    {
        return (1.0f - (a_num / (float)MaxFrames) - (1f / MaxFrames));
    }
}
