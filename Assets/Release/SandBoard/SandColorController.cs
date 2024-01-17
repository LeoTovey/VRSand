using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

enum SandColorType
{
    Static,
    Dynamic
}

public class SandColorController : MonoBehaviour
{

    [SerializeField] private SandColorType _type = SandColorType.Static;
    [SerializeField] private RawImage _colorHandleImage;
    [SerializeField] private Color _color;

    public Color SandColor => _sandColor;

    private Color _sandColor;

    private void Update()
    {
        if (_type == SandColorType.Dynamic)
        {
            _sandColor = _colorHandleImage.color;
        }
        else
        {
            _sandColor = _color;
        }
    }

}