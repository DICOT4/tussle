
using UnityEngine;

public class Util 
{

    public static void setLayerRecursively(GameObject _obj, int _newLayer) {
        if (_obj == null)
            return;
        _obj.layer = _newLayer;

        foreach (Transform _child in _obj.transform) {
            if (_child == null)
                return;
            setLayerRecursively(_child.gameObject, _newLayer);
        }
    }
}
