using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using static EnemyCell;
namespace Assets.Script
{
    /*Данный класс представляет из себя двухсвязный список, он хранит в себе список всех созданных линий, при этом всем ячейкам вновь созданной линии он прокидывает ссылки на ячейки предыдущей и наоборот*/
    public class DoulbyLinkedListEnemiesLine
    {
        private List<DoublyLinkedListEnemyCell> _enemyLineList; // Список хранит другие списки (линии) ячеек врагов
        private List<EnemyCell> _enemyListCurrent; // В эту переменную будет передаваться список из класса DoulbyLinkedListCell, в этом списке хранятся переменные enemyCell
        private List<EnemyCell> _enemyListPrev; // В эту переменную будет передаваться список из класса DoulbyLinkedListCell, в этом списке хранятся переменные enemyCell
        private DoublyLinkedListEnemyCell _head;
        private DoublyLinkedListEnemyCell _tail;
        private EnemyCell _tmpCurrentEnemyCell;
        private EnemyCell _tmpPrevEnemyCell;
        public DoulbyLinkedListEnemiesLine() { _enemyLineList = new List<DoublyLinkedListEnemyCell>(); }
        public IEnumerator GetEnumerator() => _enemyLineList.GetEnumerator();
        public DoublyLinkedListEnemyCell First()
        {
            return _enemyLineList.First();
        }
        public void Add(DoublyLinkedListEnemyCell _enemyCellList)
        {
            if (_head == null)
            {
                _head = _enemyCellList;
                _enemyLineList.Add(_head);
                _tail = _head;
                _tail.SetTopEnemyCellNull();
            }
            else
            {
                _head = _enemyCellList;
                _enemyListCurrent = _head.GetEnemyList();
                _enemyListPrev = _tail.GetEnemyList();
                for (int i = 0; i < _enemyListCurrent.Count; i++)
                {
                    _tmpCurrentEnemyCell = _enemyListCurrent[i];
                    _tmpPrevEnemyCell = _enemyListPrev[i];
                    _tmpCurrentEnemyCell.SetLinkEnemies(EnemyCell.LinksPosition.Top, _tmpPrevEnemyCell);
                    _tmpPrevEnemyCell.SetLinkEnemies(EnemyCell.LinksPosition.Bottom, _tmpCurrentEnemyCell);
                }
                _enemyLineList.Add(_head);
                _tail = _head;
            }
        }
        public int FindLineAndGetNumber(DoublyLinkedListEnemyCell enemyCellLine) //Метод принимает ссылку на линию и ищет ее в списке линий, в качестве аргумента возвращает ее номер в списке (ось Y)
        {
            return _enemyLineList.IndexOf(enemyCellLine);
        }
        public EnemyCell FoundEnemyCellByIndex(int xCoordEnemy, int YCoordEnemy) //Метод возвращает ячейку врага по введенным координатам x,y
        {
            List<EnemyCell>_tmpEnemy = _enemyLineList[YCoordEnemy].GetEnemyList(); //Берем список пиратов по индексу из списка линий пиратов
            return _tmpEnemy[xCoordEnemy]; //Из этого списка возвращаем нужного нам пирата по X индексу
        }

    }
}
