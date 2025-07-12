using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Script;
public class EnemyCell : MonoBehaviour /*IPointerClickHandler*/
{
    public System.Action<EnemyCell> SendClickedEnemyCellDelegate; //������� ������� �������� ������ ����� �� ������� ������ 
    public System.Action<EnemyCell> SendTheSameNeighborsDelegate; //������� ������� ���������� � ������� ���� �������� ������ ���������� ���������� ������
    private Dictionary<LinksPosition, EnemyCell> _neighboringEnemies; // ������� �������� �������� ������
    [SerializeField] public Image border;
    private int _numberOfPositionInLine; //������� ������ ������ �����
    private DoublyLinkedListEnemyCell _linkLine; //���������� ������� ������ ������ �� ����� � ������� ��������� ������ �����, ��������� ����� �������� ���������� � ����� ����� ��������� ������ �� ���������� Y
    public DoublyLinkedListEnemyCell LinkLine // �������� � ��� ���������� ������ �� ����� � ������� ������� ������ �����, �� ��� �� ����� ��������� ����� ����� � ������� ��������� ��������� ������ (��� y)
    {
        get { return _linkLine; }
        set { _linkLine = value; }
    }
    public int NumberOfPositionInLine
    {
        get { return _numberOfPositionInLine; }
        set { _numberOfPositionInLine = value; }
    }
    public bool IsEnemyInside { get { return _enemyInCell != null; } } //������ ������ ������ ������������ ��������, ������� ����������� ���� �� ������ ������ ���� ��� ���
    private Enemy _enemyInCell; //���� ������� �������� � ������
    public Enemy EnemyInCell //������ ������ ������ ������������ �����, ��� ��������� � ������ ����� �� ��� ������� ������������� ����� �� ������ EnemyCell, ���� ����� � ���� ������� �������� ������ � ������ � ������� ���� ��� ���������� ��������������
    { 
        get { return _enemyInCell; }
        set { _enemyInCell = value; _enemyInCell._callEnemyCellDelegate += GetEventClickFromEnemy; }
    }
    public GameObject _borderObject;
    
    public void ClearCell() // ���������� ������� � ������� ������
    {
        _enemyInCell._callEnemyCellDelegate -= GetEventClickFromEnemy;
        _enemyInCell.DestroyEnemy();
    }
    public EnemyCell()
    {
        _neighboringEnemies = new Dictionary<LinksPosition, EnemyCell>();
    }
    public enum LinksPosition //������������ � ������� �������� �� ����� ��������� ����� �������� ������ ��� ����� ����� ����� �� ������� ��� �������� � ����
    {
        Left,
        Right,
        Top,
        Bottom

    }

    public void SetLinkEnemies(LinksPosition position, EnemyCell _enemyCell) //�������� ����� � ������� �������� ������� � ������ �� ����
    {
        _neighboringEnemies.Add(position, _enemyCell);
    }
 
    private void GetEventClickFromEnemy()//����� �� ������� ����� �������� ������� �����, ��� ������� �� ����� �� ������� ������ ������� ������� �������� ������ � ������� ��������� ��������� ����
    {
        SendClickedEnemyCellDelegate?.Invoke(this);
    }
    public void FindTheSameNeighbors() //����� �������� �� ������ _neighboringEnemies � ���� ���������� ������ �� id �����
                                       //����� ������������� ����������� ����� �� �������� � �������, ������� ������� ���� ������� � ������ �� �����, � ����� �� ����� ��� � ����� �������� ��������� ������ �����
    {
        foreach (var item in _neighboringEnemies)
        {
            if (item.Value != null)
            {
                if (item.Value.EnemyInCell.ColorIdEnemy == this.EnemyInCell.ColorIdEnemy)  //������ ������ ������ 
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
