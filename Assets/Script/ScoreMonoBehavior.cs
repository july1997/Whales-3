using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

public class ScoreMonoBehavior : MonoBehaviour
{
    private Text _countText;

    private void Awake()
    {
        _countText = this.GetComponent<Text>();
    }

    public void SetCount(int count)
    {
        _countText.text = "Score : " + count.ToString();
    }
}
