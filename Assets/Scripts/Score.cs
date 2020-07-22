using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Score : MonoBehaviour
{
    public int score = 0, old = 0, best = 0;//поля счета. текущий счет, предыдущий счет, лучший счет
    private string filename = "BestScore.dat";//имя файла для вывода лучшего счета
    public void FileInBestScore()//метод извлечения лучшего счета из файла
    {
        if (!File.Exists(filename))//если файл еще не создан
            File.WriteAllText(filename, "0");//создает файл, записывает в него 0 и закрывает
        string s = File.ReadAllText(filename);//считываение лучшего счета 
        if (s.Length == 0)//если файл оказался пуст
            best = 0;//присваеваем лучшему счету 0
        else//иначе
            best = int.Parse(s);//переводим считаное значение из файла в целочисленный тип
    }
    public void FileOutBestScore()//вывод лучшего счета в файл
    {
        File.WriteAllText(filename, best.ToString());
    }
}