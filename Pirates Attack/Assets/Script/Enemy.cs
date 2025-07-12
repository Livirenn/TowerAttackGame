using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Enemy : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float _speedEnemy; //���������� ������ �������� ��� ������� ����
    public System.Action _callEnemyCellDelegate; //������� ������� ����� ��������� �� ����� � ������, ��� � ���� ������� ������ ������ �� ���� � GameField
    private bool _isActive;//������� ���������� ����� �� ������ ������ ����� ������� �� �� ������ ������ ��� ���
    public bool IsActive //������ ������ ����� ������������ ������ ����� ������� �� �� ������ ������ ��� ���, ��� ��������� ��������� �������� ������ �������� isActive � �����������/���������� ������
    {
        get { return _isActive; }
        set { _isActive = value; _enemyIcon.enabled = value; }
    }
    [SerializeField] private Image _enemyIcon; // ���������� ������ �����
    public Sprite EnemyIcon //������ ������ ����� ������������� ������ ������
    {
        get { return _enemyIcon.sprite; }
        set { _enemyIcon.sprite = value; }
    } 
    private int _colorIdEnemy; //���������� �������� ��� �����, �� �������� �� ����� ���������� ������
    public int ColorIdEnemy //������ ������ ��� ���� ����� �����
    {
        get { return _colorIdEnemy; }
        set { _colorIdEnemy = value;}
    }
    //public void SetIcon(Sprite icon) //����� ��������� ������ �����
    //{
    //    _enemyIcon.sprite=icon;
    //}
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData) 
    {
        _callEnemyCellDelegate?.Invoke();
    }
    public RectTransform EnemyRtransform
    {
        get { return (RectTransform)this.transform; }
        set { RectTransform rtr = (RectTransform)this.transform; rtr = value; }
    }
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    public void setZeroRectTransform() //����� ���� ������� ����� ������������� � ������� ��������� ��� ���� ���������� (� ����� ������� ��������� ����� ����� ������ ������ ������������ ��� � ����������� ������ ����)
    {
        EnemyRtransform.anchoredPosition = Vector2.zero;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
