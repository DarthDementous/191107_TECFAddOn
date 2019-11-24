using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberScroller : MonoBehaviour
{
    public int CurrentNum  = 0;
    public int MaxFrames   = 10;
    public float ScrollSpeed = 1;

    float _currOffsetY;
    float _startOffsetY;
    Material _mat;

    private void Awake()
    {
        _mat = GetComponent<Image>().material;
    }

    // Start is called before the first frame update
    void Start()
    {
        _currOffsetY = _mat.GetTextureOffset("_MainTex").y;
    }

    // Update is called once per frame
    void Update()
    {
        float targetOffsetY = 1.0f - (CurrentNum / (float)MaxFrames) - (1f / MaxFrames);

        _currOffsetY = Mathf.Lerp(_currOffsetY, targetOffsetY, ScrollSpeed / 10);
        _mat.SetTextureOffset("_MainTex", new Vector2(0, _currOffsetY));
    }
}
