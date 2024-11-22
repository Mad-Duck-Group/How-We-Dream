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
	[SerializeField] private bool repeatable;
	[SerializeField, HideIf(nameof(repeatable))] private bool played;

	public string TextAsset => textAsset.text;
	public Sprite Background => background;
	public bool Repeatable => repeatable;
	public bool Played {get => played; set => played = value;}

	[Button("Test Show")]
	private void TestShow()
	{
		if (!Application.isPlaying) return;
		VNManager.Instance.ShowVN(this);
	}

	public void Reset()
	{
		played = false;
	}
}