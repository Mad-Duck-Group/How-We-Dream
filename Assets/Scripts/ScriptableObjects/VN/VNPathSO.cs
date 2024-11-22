using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObjects/VNPathSO", fileName = "VNPathSO")]
public class VNPathSO : ScriptableObject
{
	[SerializeField] private TextAsset textAsset;
	[SerializeField][ShowAssetPreview] private Sprite background;
		
	public string TextAsset => textAsset.text;
	public Sprite Background => background;

	[Button("Test Show")]
	private void TestShow()
	{
		if (!Application.isPlaying) return;
		VNManager.Instance.ShowVN(this);
	}
}