using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    RectTransform thrusterfuelfill;

    private PlayerController controller;

    public void setController(PlayerController _controller) {
        controller = _controller;
    }

    void Update() {

        setFuelAmount(controller.getThrusterFuelAmount());
    }

    void setFuelAmount(float _amount) {

        thrusterfuelfill.localScale = new Vector3(1f, _amount, 1f);
    } 
}
