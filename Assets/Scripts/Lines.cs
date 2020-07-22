using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Lines : MonoBehaviour
{
    public GameObject cellSprite;//спрайт квадратика
    public GameObject ballSprite;//спрайт шарика
    public Color[] colors;//массив цветов
    public float shift = 1.6f;//расстояние между квадратиками
    public int size = 9;//размер поля
    public Text scoreText;//переменная для вывода счета
    /*
     * переменная для идентификации шарика и квадратика
     * временная переменная для идентификации шарика и квадратика
     * координаты для сброса выбора шарика
     */
    private int id, ball_id_temp, X_RESET, Y_RESET;
    private GameObject[,] cells;//матрица квадратиков
    private GameObject[] cellstemp;//массив квадратиков для показа следующих шариков
    private GameObject[,] balls;//матрица шариков
    private GameObject[] ballstemp;//массив шариков следуюющего хода
    private GameObject ball;//объект шарик. для временного хранения выбора шарика
    private Restart restart;//кнопка рестарт
    private bool gameOver;//переменная для определения конца игры(нужно ли искать линию)
    private Score score;//объект класса счет

    //функция запуска игры
    void Start()
    {
        //создание объектов класса
        restart = new Restart(this);
        balls = new GameObject[size, size];
        score = new Score();
        //считывание с файла лучшего счета
        score.FileInBestScore();
        id = 0;
        //переменные для расположение игрового поля
        float posX = -shift * (size - 1) / 2 - shift;
        float posY = Mathf.Abs(posX);
        float Xreset = posX;
        cells = new GameObject[size, size];
        cellstemp = new GameObject[3];
        ballstemp = new GameObject[3];
        //инициализация игрового поля
        for (int y = 0; y < size; y++)
        {
            posY -= shift;
            for (int x = 0; x < size; x++)
            {
                posX += shift;
                cells[x, y] = Instantiate(cellSprite, new Vector3(posX, posY, 0), Quaternion.identity);
                cells[x, y].GetComponent<circle>()._lines = this;
                cells[x, y].GetComponent<circle>().cellID = id;
                cells[x, y].GetComponent<circle>().touch = true;
                cells[x, y].transform.parent = transform;
                id++;
            }
            posX = Xreset;
        }
        //инициализация квадратиков для показа следующих шариков
        for (int i = 0; i < 3; i++)
        {
            cellstemp[i] = Instantiate(cellSprite, new Vector3(Xreset + shift * (4 + i), Mathf.Abs(Xreset - shift), 0), Quaternion.identity);
            cellstemp[i].GetComponent<circle>()._lines = this;
            cellstemp[i].GetComponent<circle>().cellID = id;
            cellstemp[i].GetComponent<circle>().touch = false;
            cellstemp[i].transform.parent = transform;
            id++;
        }
        id -= 3;
        gameOver = false;
        //добавляем шарики
        AddBalls(true);
    }

    //функция поиска линии
    //isBall - нужно ли добавлять новые шарики на поле после удаления
    void FindLines(bool isBall)
    {
        //список объектов на удаление
        List<GameObject> arr = new List<GameObject>();
        bool getBall = true;
        int z = 0;//чтобы проверить каждый счетчик
        int index = 0;
        while (z < id)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {//цыкл поиска горизонтальной линии
                    z++;
                    if (x <= size - 2)
                    {//если шарик и шарик справа существуют и равны по цвету и их еще нужно искать
                        if (balls[x, y] && balls[x + 1, y] && balls[x, y].GetComponent<Balls>().color == balls[x + 1, y].GetComponent<Balls>().color && getBall)
                        {
                            if (index == 0)//если счечик шариков = 0
                            {
                                index = 2;//добавляем 2 шарика
                                arr.Add(balls[x, y]);
                                arr.Add(balls[x + 1, y]);//и помещаем их в массив на удаление
                            }
                            else//если счетчик не равен 0
                            {
                                index++;//увеличиваем счетчик на 1
                                arr.Add(balls[x + 1, y]);//добавляем в массив на удаление один шарик
                            }
                        }
                        else if (index < 5)//иначе если счетчик меньше 5ти - обнуляем счетчик и заново выделяем память подмассив на удаление
                        {
                            index = 0;
                            arr = new List<GameObject>();
                        }
                        else//иначе шарик искать не нужно
                        {
                            getBall = false;
                        }
                    }

                }
                if (index < 5)//иначе если счетчик меньше 5ти - обнуляем счетчик и заново выделяем память подмассив на удаление
                {
                    index = 0;
                    arr = new List<GameObject>();
                }
            }
        }
        if (index < 5)
        {
            index = 0;
            arr = new List<GameObject>();
            getBall = true;
            z = 0;
            while (z < id)
            {
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {//поиск вертикальной линий
                        z++;
                        if (y <= size - 2)
                        {
                            if (balls[x, y] && balls[x, y + 1] && balls[x, y].GetComponent<Balls>().color == balls[x, y + 1].GetComponent<Balls>().color && getBall)
                            {
                                if (index == 0)
                                {
                                    index = 2;
                                    arr.Add(balls[x, y]);
                                    arr.Add(balls[x, y + 1]);
                                }
                                else
                                {
                                    index++;
                                    arr.Add(balls[x, y + 1]);
                                }
                            }
                            else if (index < 5)
                            {
                                index = 0;
                                arr = new List<GameObject>();
                            }
                            else
                            {
                                getBall = false;
                            }
                        }
                    }
                    if (index < 5)
                    {
                        index = 0;
                        arr = new List<GameObject>();
                    }
                }
            }
        }
        if (index < 5)
        {
            index = 0;
            arr = new List<GameObject>();
            getBall = true;
            z = 0;
            while (z < id)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {//поиск диагональной линии от левого верхнего угла
                        z++;
                        if (x <= size - 2 && y <= size - 2)
                        {
                            if (balls[y, x] && balls[y + 1, x + 1] && balls[y, x].GetComponent<Balls>().color == balls[y + 1, x + 1].GetComponent<Balls>().color && getBall)
                            {
                                if (index == 0)
                                {
                                    index = 2;
                                    arr.Add(balls[y, x]);
                                    arr.Add(balls[y + 1, x + 1]);
                                    bool prov = true;//переменная для проверки
                                    int X = x + 1, Y = y + 1;//дополнительные перменные чтобы не потерять изначальные значения и не уйти дальше по матрице
                                    while (prov)//работает пока не равно false
                                    {
                                        if (X <= size - 2 && Y <= size - 2)//если не выходим за пределы матрицы
                                        {
                                            if (balls[Y, X] && balls[Y + 1, X + 1] && balls[Y, X].GetComponent<Balls>().color == balls[Y + 1, X + 1].GetComponent<Balls>().color)
                                            {//проверка шариков на совместимость
                                                arr.Add(balls[Y + 1, X + 1]);
                                                index++;
                                                X++;
                                                Y++;
                                            }
                                            else//иначе если шарики не равны
                                            {
                                                prov = false;//проверка равна false
                                                if (index < 5)//если счетчик меньше 5ти - обнуление счетчика и массива на удаление и выход из цикла while
                                                {
                                                    index = 0;
                                                    arr = new List<GameObject>();
                                                }
                                            }
                                        }
                                        else//если выходим за пределы - выход из цикла while
                                            prov = false;
                                    }
                                }
                            }
                            else if (index < 5)
                            {
                                index = 0;
                                arr = new List<GameObject>();
                            }
                            else
                            {
                                getBall = false;
                            }
                        }
                    }
                }
            }
        }
        if (index < 5)
        {
            index = 0;
            arr = new List<GameObject>();
            getBall = true;
            z = 0;
            while (z < id)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = size - 1; x >= 0; x--)
                    {//поиск диагонали от правого угла
                        z++;
                        if (x >= 1 && y <= size - 2)
                        {
                            if (balls[y, x] && balls[y + 1, x - 1] && balls[y, x].GetComponent<Balls>().color == balls[y + 1, x - 1].GetComponent<Balls>().color && getBall)
                            {
                                if (index == 0)
                                {
                                    index = 2;
                                    arr.Add(balls[y, x]);
                                    arr.Add(balls[y + 1, x - 1]);
                                    bool prov = true;
                                    int X = x - 1, Y = y + 1;
                                    while (prov)
                                    {
                                        if (X >= 1 && Y <= size - 2)
                                        {
                                            if (balls[Y, X] && balls[Y + 1, X - 1] && balls[Y, X].GetComponent<Balls>().color == balls[Y + 1, X - 1].GetComponent<Balls>().color)
                                            {
                                                arr.Add(balls[Y + 1, X - 1]);
                                                index++;
                                                X--;
                                                Y++;
                                            }
                                            else
                                            {
                                                prov = false;
                                                if (index < 5)
                                                {
                                                    index = 0;
                                                    arr = new List<GameObject>();
                                                }
                                            }
                                        }
                                        else
                                            prov = false;
                                    }
                                }
                            }
                            else if (index < 5)
                            {
                                index = 0;
                                arr = new List<GameObject>();
                            }
                            else
                            {
                                getBall = false;
                            }
                        }
                    }
                }
            }
        }
        if (isBall)
        {
            if (index < 5)
                AddBalls();
            else
                StartCoroutine(WaitDestroy(arr));
        }
        else
        {
            if (index >= 5)
                StartCoroutine(WaitDestroy(arr));
        }
    }

    //событие вывода счета на экран
    void OnGUI()
    {
        scoreText.text = "Текущий Счет:\n" + score.score + "\n\nПрошлый счет:\n" + score.old + "\n\nЛучший счет:\n"+ score.best;
    }

    //метод поиска шарика
    //параметр - ид квадтратика, на который нажали
    public void FindBall(int ball_id)
    {
        if (!gameOver)
        {//если игра еще не оконченна
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (ball == null)//если шарик еще не выбран
                    {
                        if (cells[x, y].GetComponent<circle>().cellID == ball_id && balls[x, y])//если по заданным x, y есть мячик и id квадратика равно найденому квадратику
                        {
                            balls[x, y].GetComponent<SpriteRenderer>().color = balls[x, y].GetComponent<Balls>().color;//запоминаем цвет(тем самым убирая тусклость)
                            ball = balls[x, y];//запоминаем
                            balls[x, y] = null;//удаляем шарик из матрицы шариков
                            X_RESET = x;
                            Y_RESET = y;//запоминаем его положение в матрице
                            ball_id_temp = ball_id;//запоминаем id шарика
                        }
                    }
                    else//если шарик уже выбран
                    {
                        if (x == X_RESET && y == Y_RESET && ball_id == ball_id_temp) //если найденный шарик - уже выбранный шарик
                        {
                            Color color = ball.GetComponent<Balls>().color;
                            color.a = 0.5f;
                            ball.GetComponent<SpriteRenderer>().color = color;
                            ball.transform.position = new Vector3(cells[X_RESET, Y_RESET].transform.position.x, cells[X_RESET, Y_RESET].transform.position.y, -1);
                            balls[X_RESET, Y_RESET] = ball;
                            ball = null;
                            X_RESET = -1;
                            Y_RESET = -1;
                            //возвращаем шарик на прежнее место в матрице и на поле
                        }
                        else if (cells[x, y].GetComponent<circle>().cellID == ball_id && balls[x, y] != null && ball_id != ball_id_temp)
                        {
                            //клик по другому шарику если шарик выбран но клик не по выбранному шарику
                            Color color = ball.GetComponent<Balls>().color;
                            color.a = 0.5f;
                            ball.GetComponent<SpriteRenderer>().color = color;
                            ball.transform.position = new Vector3(cells[X_RESET, Y_RESET].transform.position.x, cells[X_RESET, Y_RESET].transform.position.y, -1);
                            balls[X_RESET, Y_RESET] = ball;
                            //возвращаем шарик выбранный на то место где он и был до этого

                            balls[x, y].GetComponent<SpriteRenderer>().color = balls[x, y].GetComponent<Balls>().color;
                            ball = balls[x, y];
                            balls[x, y] = null;
                            X_RESET = x;
                            Y_RESET = y;
                            ball_id_temp = ball_id;
                            //и выбираем другой шарик
                        }
                        else if (cells[x, y].GetComponent<circle>().cellID == ball_id && balls[x, y] == null)//если клик по квадратику который без шарика - перемещаем шарик
                        {
                            bool t = false;
                            FindWay(X_RESET, Y_RESET, x, y, ref t);//вызов поиска пути. если t = true  - то путь проложить можно
                            if (t)
                            {
                                Color color = ball.GetComponent<Balls>().color;
                                color.a = 0.5f;
                                ball.GetComponent<SpriteRenderer>().color = color;
                                ball.transform.position = new Vector3(cells[x, y].transform.position.x, cells[x, y].transform.position.y, -1);
                                balls[x, y] = ball;
                                ball = null;
                                X_RESET = -1;
                                Y_RESET = -1;
                                //перемещение шарика и вызов метода поиска линии
                                FindLines(true);
                            }
                            else//если путь проложить нельзя - возвращаем шарик на исходную позицию
                            {
                                Color color = ball.GetComponent<Balls>().color;
                                color.a = 0.5f;
                                ball.GetComponent<SpriteRenderer>().color = color;
                                ball.transform.position = new Vector3(cells[X_RESET, Y_RESET].transform.position.x, cells[X_RESET, Y_RESET].transform.position.y, -1);
                                balls[X_RESET, Y_RESET] = ball;
                                ball = null;
                                X_RESET = -1;
                                Y_RESET = -1;
                            }
                        }
                    }
                }
            }
        }
    }
    /*удаление шариков с поля
     принимаемые параметры: список удаляемых объектов*/
    IEnumerator WaitDestroy(List<GameObject> item)
    {

        yield return new WaitForSeconds(0.2f);
        foreach (GameObject obj in item)
        {
            Destroy(obj.gameObject);//удаление объекта
            score.score++;//увеличение счета
        }
    }

    //метод отчистки игрового поля и начало новой игры
    IEnumerator ClearField()
    {
        yield return new WaitForSeconds(1);
        gameOver = false;
        int j = 0;
        while (j < id)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    j++;
                    if (balls[x, y] != null)//если шарик существует
                    {
                        Destroy(balls[x, y].gameObject);//удаляем его объект
                        balls[x, y] = null;//и обнуляем его в массиве шариков
                    }
                }
            }
        }
        Debug.Log("New Game");//записываем в лог "новая игра"
        AddBalls();//добавляем новые шарики на поле
    }

    //метод добавление шариков на игровое поле
    //параметр t - нужно ли добавлять шарики и на поле и во временные ячейки(шарики, которые будут следующие)
    //по умолчанию не нужно
    void AddBalls(bool t = false)
    {
        int ballCount = 0, e = 0;//счетчики количество свободных полей на игровом поле
        while (e < id)
        {//работает пока не пересмотрим все поля
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (balls[x, y] == null)//если шарика в ячейке нет
                        ballCount++;//увеличиваем счетчик шариков на 1
                    e++;
                }
            }
        }
        if (ballCount > 3)//если свободных полей больше чем 3
            ballCount = 3;//приравниваем 3м
        int i = 0;//счетчик поставленых шариков на поле
        while (i < ballCount)
        {
            int j = Random.Range(0, id);//выбираем случайное поле
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (cells[x, y].GetComponent<circle>().cellID == j && balls[x, y] == null)//проверяем занято ли выбранное поле
                    {
                        if (t == true)//если игра только началась нужно поставить шарики в верхние 3 поля и на игровое поле
                        {//ставим спрайт шарика на спрайт квадратика верхнего
                            ballstemp[i] = Instantiate(ballSprite, new Vector3(cellstemp[i].transform.position.x, cellstemp[i].transform.position.y, -1), Quaternion.identity);
                            Color _colortemp = colors[Random.Range(0, colors.Length)];//присваеваем случайный цвет
                            ballstemp[i].GetComponent<Balls>().color = _colortemp;//сохраняем цвет в объекте шарика
                            _colortemp.a = 0.5f;//делаем цвет тусклее
                            ballstemp[i].GetComponent<SpriteRenderer>().color = _colortemp;//присваеваем тусклый цвет спрайту шарика

                            //похожим образом ставим шарик на игровое поле
                            balls[x, y] = Instantiate(ballSprite, new Vector3(cells[x, y].transform.position.x, cells[x, y].transform.position.y, -1), Quaternion.identity);
                            Color _color = colors[Random.Range(0, colors.Length)];
                            balls[x, y].GetComponent<Balls>().color = _color;
                            _color.a = 0.5f;
                            balls[x, y].GetComponent<SpriteRenderer>().color = _color;
                            i++;//увеличиваем счетчик поставленых шариков на 1
                        }
                        else//если игра не первая и шарики не нужно ставить и на поле и во временные ячейки
                        {
                            balls[x, y] = Instantiate(ballSprite, new Vector3(cells[x, y].transform.position.x, cells[x, y].transform.position.y, -1), Quaternion.identity);
                            balls[x, y].GetComponent<Balls>().color = ballstemp[i].GetComponent<Balls>().color;
                            balls[x, y].GetComponent<SpriteRenderer>().color = ballstemp[i].GetComponent<SpriteRenderer>().color;
                            //ставим шарик на поле(берем атрибуты шарика из временых шариков, которые вверху стоят)

                            Color _colortemp = colors[Random.Range(0, colors.Length)];
                            ballstemp[i].GetComponent<Balls>().color = _colortemp;
                            _colortemp.a = 0.5f;
                            ballstemp[i].GetComponent<SpriteRenderer>().color = _colortemp;
                            //изменяем цвет шарика(на верху) на случайный

                            i++;
                        }
                    }
                }
            }
        }
        ballCount = 0;
        e = 0;
        //цикл поиска свободных полей
        while (e < id)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (balls[x, y] == null)
                        ballCount++;
                    e++;
                }
            }
        }
        if (ballCount == 0)//если свободных полей нет
            EndGame();//игра проиграна
        FindLines(false);//линии искать не нужно
    }
    /*Метод проверки пути
     x, y - координаты шарика, который перемещается
     _x, _y - координаты шарика куда перемещается
     t - значение для возврата, свободен ли путь между точками x, y и _x, _y*/
    private void FindWay(int x, int y, int _x, int _y, ref bool t)
    {
        int [,] matrix_temp = new int[size, size];//матрица для определения 
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++) 
            {
                if (balls[i, j] == null)//если шарик не существует в таком поле
                    matrix_temp[i, j] = 0;//заполняем матрицу 0
                else//иначе
                    matrix_temp[i, j] = -1;//заполняем -1(преграда для передвижения)
            }
        matrix_temp[_x, _y] = 100;//обозначим точку куда нужно переместится как 100
        BeWay(ref matrix_temp, x, y, _x, _y, ref t);
    }

    /*
     * рекурсивная функция поиска пути(можно ли проложить путь)
     * матрица преград и свободных полей(преграда = -1, свободное поле = 0)
     * x, y, _x, _y - координаты от куда прокладывать путь, куда прокладывать путь
     * t - если путь можно проложить - присваивается true, иначе выход из метода
     */
    private void BeWay(ref int[,] matrix, int x, int y, int _x, int _y, ref bool t)
    {
        if ((x + 1 == _x) && y == _y)//если нужная точка находится справа от переданных точек
            t = true;//путь проложить можно
        else if (x - 1 == _x && y == _y)//если нужная точка находится слева от переданных точек
            t = true;//путь проложить можно
        else if (x == _x && y + 1 == _y)//если нужная точка находится выше переданных точек
            t = true;//путь проложить можно
        else if (x == _x && y - 1 == _y)//если нужная точка находится ниже переданных точек
            t = true;//путь проложить можно
        else if (x == _x && y == _y)//если нужная точка уже переданна
            t = true;//путь проложить можно
        else//если нужная точка не находится в одном шаге от переданных точек
        {//проверяем, можно ли передать следующие точки в эту же функцию
            if (x + 1 < size)//шаг вправа
                if (matrix[x + 1, y] != -1 && matrix[x + 1, y] == 0)
                {
                    matrix[x + 1, y]++;//увеличиваем что бы не зайти в эту точку еще раз
                    BeWay(ref matrix, x + 1, y, _x, _y, ref t);
                }
            if (x - 1 >= 0)//шаг влево
                if (matrix[x - 1, y] != -1 && matrix[x - 1, y] == 0)
                {
                    matrix[x - 1, y]++;//увеличиваем что бы не зайти в эту точку еще раз
                    BeWay(ref matrix, x - 1, y, _x, _y, ref t);
                }
            if (y + 1 < size)//шаг вверх
                if (matrix[x, y + 1] != -1 && matrix[x, y + 1] == 0)
                {
                    matrix[x, y + 1]++;//увеличиваем что бы не зайти в эту точку еще раз
                    BeWay(ref matrix, x, y + 1, _x, _y, ref t);
                }
            if (y - 1 >= 0)//шаг вниз
                if (matrix[x, y - 1] != -1 && matrix[x, y - 1] == 0)
                {
                    matrix[x, y - 1]++;//увеличиваем что бы не зайти в эту точку еще раз
                    BeWay(ref matrix, x, y - 1, _x, _y, ref t);
                }
        }
    }

    //метод завершения игры и начало новой игры
    public void EndGame()
    {
        gameOver = true;
        Debug.Log("Game Over");//запись в лог "конец игры"
        score.old = score.score;//запоминаем счет набранный счет в поле старого счета
        if (score.score > score.best)//если текущий счет больше чем наибольший счет
        {
            score.best = score.score;//запоминаем текущий счет в лучший счет
            score.FileOutBestScore();//выводим лучший счет в файл
        }
        score.score = 0;//обнуляем текущий счет
        StartCoroutine(ClearField());//отчищаем поле от шариков, добавляем новые 3 шарика
        FindLines(false);//ищем линии
    }
}
