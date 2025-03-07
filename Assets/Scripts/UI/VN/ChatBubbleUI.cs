using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gamelogic.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityRandom = UnityEngine.Random;

public class ChatBubbleUI : MonoBehaviour
{
	[SerializeField] private TMP_Text speakerName;
	[SerializeField] private TMP_Text text;
	[SerializeField] private Image background;
	[SerializeField] LayoutGroup layoutGroup;
		
	private Sequence sequence;

	public void Setup(Dialogue dialogue, Sprite bubbleSprite)
	{
		speakerName.text = dialogue.Name.ToString();
		layoutGroup.childAlignment = dialogue.CharacterPosition == CharacterPosition.Left ? TextAnchor.UpperLeft : TextAnchor.UpperRight;
		speakerName.alignment = dialogue.CharacterPosition == CharacterPosition.Left ? TextAlignmentOptions.MidlineLeft : TextAlignmentOptions.MidlineRight;
		speakerName.horizontalAlignment = dialogue.CharacterPosition == CharacterPosition.Left ? HorizontalAlignmentOptions.Left : HorizontalAlignmentOptions.Right;
		background.sprite = bubbleSprite;
		var rotation = dialogue.CharacterPosition == CharacterPosition.Left ? 0 : 180;
		speakerName.transform.SetRotationY(rotation);
		text.transform.SetRotationY(rotation);
		background.transform.SetRotationY(rotation);
		text.text = dialogue.Msg;
	}
		
	public void SetAlpha(float alpha)
	{
		if (sequence.IsActive())
			sequence.Kill();
		sequence = DOTween.Sequence();
		sequence.Append(background.DOFade(alpha, 0.2f));
		sequence.Join(speakerName.DOFade(alpha, 0.2f));
		sequence.Join(text.DOFade(alpha, 0.2f));
	}
}