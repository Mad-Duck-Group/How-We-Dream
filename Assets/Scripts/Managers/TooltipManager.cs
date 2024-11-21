using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = System.Object;
using UnityRandom = UnityEngine.Random;

public class TooltipManager : PersistentMonoSingleton<TooltipManager>
	{
		[SerializeField] private GameObject tooltipPrefab;
		[SerializeField] private SkillTooptipUI skillTooptipUIPrefab;
		[SerializeField] private Vector3 defaultOffset;
		
		public delegate void TooltipDestroyed();
		public static event TooltipDestroyed OnTooltipDestroyed;
		
		private RectTransform currentTooltip;
		private Vector3 CurrentMousePosition => Input.mousePosition;

		public void ShowTooltip(Object tooltipObject)
		{
			if (tooltipObject == null) return;
			switch (tooltipObject)
			{
				case Skill skill:
					ShowTooltip(skill);
					break;
			}
		}

		private void ShowTooltip(Skill skill)
		{
			DestroyTooltip();
			currentTooltip = Instantiate(tooltipPrefab, new Vector3(10000, 10000, 0), Quaternion.identity, transform)
				.GetComponent<RectTransform>();
			var skillTooltipUI = Instantiate(skillTooptipUIPrefab, currentTooltip.transform);
			skillTooltipUI.SetTooltip(skill);
			StartCoroutine(AdjustTooltipPosition());
		}

		private IEnumerator AdjustTooltipPosition()
		{
			if (!currentTooltip) yield break;
			yield return new WaitForEndOfFrame();
			LayoutRebuilder.ForceRebuildLayoutImmediate(currentTooltip);

			Canvas canvas = GetComponent<Canvas>();
			RectTransform canvasRect = canvas.GetComponent<RectTransform>();
			currentTooltip.anchorMin = new Vector2(0.5f, 0.5f);
			currentTooltip.anchorMax = new Vector2(0.5f, 0.5f);
			currentTooltip.pivot = new Vector2(0.5f, 0.5f);
			
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, canvas.worldCamera, out Vector2 localMousePosition);
			
			currentTooltip.anchoredPosition = localMousePosition + (Vector2)defaultOffset;
			
			Vector3[] corners = new Vector3[4];
			currentTooltip.GetWorldCorners(corners);

			for (int i = 0; i < corners.Length; i++)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, corners[i], canvas.worldCamera, out Vector2 localPoint);
				corners[i] = localPoint;
			}
			
			float tooltipLeft = corners[0].x;
			float tooltipBottom = corners[0].y;
			float tooltipRight = corners[2].x;
			float tooltipTop = corners[2].y;
			
			float canvasLeft = canvasRect.rect.xMin;
			float canvasRight = canvasRect.rect.xMax;
			float canvasTop = canvasRect.rect.yMax;
			float canvasBottom = canvasRect.rect.yMin;
			
			Vector2 finalPosition = currentTooltip.anchoredPosition;

			if (tooltipBottom < canvasBottom)
				finalPosition.y += (canvasBottom - tooltipBottom);
			if (tooltipTop > canvasTop)
				finalPosition.y -= (tooltipTop - canvasTop);
			if (tooltipLeft < canvasLeft)
				finalPosition.x += (canvasLeft - tooltipLeft);
			if (tooltipRight > canvasRight)
				finalPosition.x -= (tooltipRight - canvasRight);
			
			currentTooltip.anchoredPosition = finalPosition;
		}

		public void DestroyTooltip()
		{
			if (!currentTooltip) return;
			Destroy(currentTooltip.gameObject);
			OnTooltipDestroyed?.Invoke();
		}
	}