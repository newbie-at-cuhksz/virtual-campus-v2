using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GercStudio.USK.Scripts
{
	[ExecuteInEditMode]
	public class UIPlaceholder : MonoBehaviour
	{
		private RectTransform _rectTransform, _parentRectTransform;
		private VerticalLayoutGroup verticalLayoutGroup;

		public PUNHelper.ContentType ContentType;
		
		public Text KD;
		public Text Rank;
		public Text Score;
		public RawImage Icon;
		public Color32 HighlightedColor;

		public Text KillerName;
		public Text VictimName;
		public RawImage WeaponIcon;
		
		public Text Mode;
		public Text Map;
		public Text Count;

		public Text Name;
		public RawImage ImagePlaceholder;
		public Button Button;
		public Image Background;
		public RawImage SelectionIndicator;

		void OnEnable()
		{
			UpdateWidth();
		}

		void Update()
		{
			UpdateWidth();
		}

		private void UpdateWidth()
		{
			if (ContentType != PUNHelper.ContentType.Player) return;
			if (verticalLayoutGroup == null || _rectTransform == null || _parentRectTransform == null)
			{
				verticalLayoutGroup = GetComponentInParent<VerticalLayoutGroup>();
				
				if (verticalLayoutGroup != null)
				{
					_parentRectTransform = verticalLayoutGroup.GetComponent<RectTransform>();
					_rectTransform = GetComponent<RectTransform>();
					_rectTransform.pivot = new Vector2(0, 1);
					_rectTransform.sizeDelta = new Vector2(_parentRectTransform.rect.size.x - (verticalLayoutGroup.padding.left + verticalLayoutGroup.padding.right), _rectTransform.sizeDelta.y);
				}
			}
			else
			{
				_rectTransform.sizeDelta = new Vector2(_parentRectTransform.rect.size.x - (verticalLayoutGroup.padding.left + verticalLayoutGroup.padding.right), _rectTransform.sizeDelta.y);
			}
		}
	}
}

