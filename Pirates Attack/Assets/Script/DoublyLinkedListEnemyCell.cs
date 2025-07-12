using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnemyCell;

public class DoublyLinkedListEnemyCell : IEnumerable
{
    private List<EnemyCell> _enemyCellList = new List<EnemyCell>();
    private EnemyCell _head;
    private EnemyCell _tail;
    public IEnumerator GetEnumerator() => _enemyCellList.GetEnumerator();
    public void Add(EnemyCell _enemyCell)
    {
        EnemyCell node = _enemyCell;
        if (_head == null)
        {
            _head = node;
            _enemyCellList.Add(_head);

        }
        else
        {
            _tail.SetLinkEnemies(LinksPosition.Right, node);
            node.SetLinkEnemies(LinksPosition.Left, _tail);
            _enemyCellList.Add(node);
        }
        _tail = node;
    }
    public EnemyCell GetAnEnemyByIndex(int index) //Метод возвращает врага по индексу
    {
        return _enemyCellList[index];
    }
    public List<EnemyCell> GetEnemyList()
    {
        return _enemyCellList;
    }
    public void Clear()
    {
        _enemyCellList.Clear();
        _enemyCellList = null;
        _head = null;
        _tail = null;
    }
    public EnemyCell FirstCell()
    {
        return _enemyCellList.First();
    }
    public void SetTopEnemyCellNull()  //Метод устанавливает верхним ячейкам врагов значение null, это значение устанавливается если линия пирата первая, или при удалении верхней пустой линии
                                       //Установка null значения в данных случаях предназначена для того чтобы останавливать цикл, который ищет и спавнит пиратов для падения
    {
        foreach(EnemyCell pc in _enemyCellList)
        {
            pc.SetLinkEnemies(EnemyCell.LinksPosition.Top, null);
        }
    }

}
