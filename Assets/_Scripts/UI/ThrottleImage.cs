using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrottleImage : MonoBehaviour
{
    [SerializeField] private Image[] images = new Image[0];

    [SerializeField] private Vector3 offset_ = new Vector3();

    public Vector3 offset { get => offset_; set => UpdateOffset(value); }
    public Sprite sprite { get => images[0].sprite; set => SetImage(value); }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateOffset(offset_);
    }
#endif

    private void SetImage(Sprite image)
    {
        foreach(Image img in images) {
            img.sprite = image;
        }
    }

    private void UpdateOffset(Vector3 offset)
    {
        this.offset_ = offset;
        for (int i = 0; i < images.Length; i++) {
            float factor = i - (images.Length - 1) * 0.5f;
            Vector3 newPos = new Vector3(factor * offset.x, factor * offset.y, images[i].transform.localPosition.z);

            images[i].transform.localPosition = newPos;

            images[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, offset.z * (images.Length - 1 - i)));
        }
    }
}
