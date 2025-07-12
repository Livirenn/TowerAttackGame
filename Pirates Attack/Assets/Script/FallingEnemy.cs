using System.Collections;
using UnityEngine;


namespace Assets.Script
{
    public class FallingEnemy : MonoBehaviour
    {
        [SerializeField] private float _speedEnemy;
        private EnemyCell _targetEnemyCell; //Переменная хранит целевую ячейку куда должен упасть враг
        private EnemyCell _startEnemyCell; //Переменная которая хранит стартовую ячейку откуда будет падать враг
        private RectTransform _rectTransform; //Рект трансформ который хранит координату падающего врага
        private RectTransform _targetRecttransform; //Рект трансформ который хранит координату куда должен упасть враг
        private bool _isFaling = false; // Флаг который оповещает враг падет или нет 
        private Enemy _pirateInside; //Переменная которая будет хранить пирата из стартовой ячейки, затем эта переменная будет назначена целевой ячейке 

        private void Start()
        {
            _rectTransform = (RectTransform)transform; //Берем рект трансформ падающего объекта
        }
        public void StartFall() //Метод который запустит падение объекта
        {
            _isFaling = true; 
        }
        public void SetTarget(EnemyCell targetCell, Enemy p) 
        {
            _targetEnemyCell = targetCell; 
            _targetRecttransform = (RectTransform)_targetEnemyCell.gameObject.transform;

            _pirateInside = p;
        }
        public EnemyCell TargetEnemyCell
        {
            get{ return _targetEnemyCell; }
            set{ _targetEnemyCell = value; }
        }
        public EnemyCell StartEnemyCell
        {
            get { return _startEnemyCell; }
            set { _startEnemyCell = value; }
        }

        // Update is called once per frame
        void Update()
        {
            if (_isFaling)
            {
                float speed = _speedEnemy * Time.deltaTime;
                _rectTransform.anchoredPosition = Vector2.MoveTowards(_rectTransform.anchoredPosition, Vector2.zero, speed);
                Debug.Log(_rectTransform.anchoredPosition +" "+ _targetRecttransform.anchoredPosition);
                if(_rectTransform.anchoredPosition.y<=0)
                {
                    _isFaling = false;
                    _targetEnemyCell.EnemyInCell = _pirateInside;
                    transform.parent = _targetEnemyCell.transform;
                }
            }
        }
    }
}