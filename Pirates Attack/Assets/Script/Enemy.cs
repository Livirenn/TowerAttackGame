using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Enemy : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float _speedEnemy; //ѕеременна€ хранит скорость при падении враг
    public System.Action _callEnemyCellDelegate; //ƒелегат который будет ссылатьс€ на метод в €чейке, она в свою очередь вернет ссылку на себ€ в GameField
    private bool _isActive;//Ѕулева€ переменна€ здесь мы храним статус врага активен он на данный момент или нет
    public bool IsActive //√еттер сеттер врага возвращающий статус врага активен он на данный момент или нет, при изменении параметра мен€етс€ булево значение isActive и отключаетс€/включаетс€ иконка
    {
        get { return _isActive; }
        set { _isActive = value; _enemyIcon.enabled = value; }
    }
    [SerializeField] private Image _enemyIcon; // ѕеременна€ иконки врага
    public Sprite EnemyIcon //√еттер сеттер врага возвращающего иконку пирата
    {
        get { return _enemyIcon.sprite; }
        set { _enemyIcon.sprite = value; }
    } 
    private int _colorIdEnemy; //ѕеременна€ хран€ща€ код цвета, по которому мы будем сравнивать врагов
    public int ColorIdEnemy //√еттер сеттер дл€ кода цвета врага
    {
        get { return _colorIdEnemy; }
        set { _colorIdEnemy = value;}
    }
    //public void SetIcon(Sprite icon) //ћетод назначает иконку врагу
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
    public void setZeroRectTransform() //ћетод дает команду врагу переместитьс€ в нулевое положение его рект трансформа (к этому моменту родителем врага будет друга€ €чейка относительно нее и выровн€етс€ данный враг)
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
