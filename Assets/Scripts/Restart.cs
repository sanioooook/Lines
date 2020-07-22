using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour {

    public Lines _lines;

    public Restart(Lines lines)//констуктор класса рестарт
    {
        _lines = lines;
    }

    void OnMouseDown()//событие нажатия мыши по объекту
    {
        _lines.EndGame();//вызов метода завершения игры
    }
}
