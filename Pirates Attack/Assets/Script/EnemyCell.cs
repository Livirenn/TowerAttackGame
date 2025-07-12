using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Script;
public class EnemyCell : MonoBehaviour /*IPointerClickHandler*/
{
    public System.Action<EnemyCell> SendClickedEnemyCellDelegate; //Делегат который передает ячейку врага на которую нажали 
    public System.Action<EnemyCell> SendTheSameNeighborsDelegate; //Делегат который отправляет в игровое поле соседние ячейки содержащие одинаковых врагов
    private Dictionary<LinksPosition, EnemyCell> _neighboringEnemies; // Словарь хранящий соседних врагов
    [SerializeField] public Image border;
    private int _numberOfPositionInLine; //Позиция ячейки внутри линии
    private DoublyLinkedListEnemyCell _linkLine; //Переменная которая хранит ссылку на линию в которой находится ячейка врага, небходима чтобы получить информацию в каком месте находится ячейка по координате Y
    public DoublyLinkedListEnemyCell LinkLine // Передаем в эту переменную ссылку на линию в которой создана ячейка врага, по ней бы будем вычислять номер линии в которой находится вражеская ячейка (ось y)
    {
        get { return _linkLine; }
        set { _linkLine = value; }
    }
    public int NumberOfPositionInLine
    {
        get { return _numberOfPositionInLine; }
        set { _numberOfPositionInLine = value; }
    }
    public bool IsEnemyInside { get { return _enemyInCell != null; } } //Геттер сеттер ячейки возвращающий значение, которое информирует есть ли внутри ячейки враг или нет
    private Enemy _enemyInCell; //Враг который хранится в ячейке
    public Enemy EnemyInCell //Геттер сеттер ячейки возвращающий врага, при помещении в ячейку врага на его делегат подписывается метод из класса EnemyCell, этот метод в свою очередь отправит ячейку с врагом в игровое поле для дальнейших преобразований
    { 
        get { return _enemyInCell; }
        set { _enemyInCell = value; _enemyInCell._callEnemyCellDelegate += GetEventClickFromEnemy; }
    }
    public GameObject _borderObject;
    
    public void ClearCell() // отписываем делегат и очищаем ячейку
    {
        _enemyInCell._callEnemyCellDelegate -= GetEventClickFromEnemy;
        _enemyInCell.DestroyEnemy();
    }
    public EnemyCell()
    {
        _neighboringEnemies = new Dictionary<LinksPosition, EnemyCell>();
    }
    public enum LinksPosition //Перечисление с помощью которого мы будем указывать каких соседних врагов нам нужно будет взять из словаря или добавить в него
    {
        Left,
        Right,
        Top,
        Bottom

    }

    public void SetLinkEnemies(LinksPosition position, EnemyCell _enemyCell) //Добавить врага в словарь указывая позицию и ссылку на него
    {
        _neighboringEnemies.Add(position, _enemyCell);
    }
 
    private void GetEventClickFromEnemy()//Метод на который будет подписан делегат врага, при нажатии на врага он вызовет другой делегат который отправит ячейку в которой находится выбранный враг
    {
        SendClickedEnemyCellDelegate?.Invoke(this);
    }
    public void FindTheSameNeighbors() //Метод проходит по списку _neighboringEnemies и ищет одинаковых врагов по id цвета
                                       //затем обнаруженного одинакового врага он передает в делегат, который добавит этот элемент в список на поиск, и затем по циклу уже у этого элемента вызовется данный метод
    {
        foreach (var item in _neighboringEnemies)
        {
            if (item.Value != null)
            {
                if (item.Value.EnemyInCell.ColorIdEnemy == this.EnemyInCell.ColorIdEnemy)  //пустые ячейки учесть 
                {
                    SendTheSameNeighborsDelegate?.Invoke(item.Value);
                }
            }
        }
    }
    public RectTransform GetRectTransformCell()
    {
        RectTransform rtr = (RectTransform)this.transform;
        return rtr;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
