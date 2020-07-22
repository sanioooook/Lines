using UnityEngine;
using System.Collections;

public class circle : MonoBehaviour
{

    public int cellID;//поле для идентификации клика
    public Lines _lines;
    public bool touch;//если игровой квадрат нажимной, равно true

    void OnMouseDown()
    {
        if(touch)//если кадратик нажимной
            _lines.FindBall(cellID);//ищем шарик
    }
}