using Assets.Script;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameField : MonoBehaviour
{

    [SerializeField] private RectTransform _gameField; // RectTransform �������� ����
    [SerializeField] private EnemyCell _enemyCellPrefab; // ������ ������ �����
    [SerializeField] private Enemy _enemyPrefab; // ������ �����
    [SerializeField] private List<Sprite> _enemiesSprite;// ������ ������ ������� ������
    private RectTransform _lastCreatedEnemyLineCoord; // ���������� ��� �������� ��������� ��������� �����, �������� ��� �������� �����, ������� ������ ������� � ������������ ��� ���� ���������
    private DoublyLinkedListEnemyCell _enemiesCellList;
    private DoulbyLinkedListEnemiesLine _enemiesLineList;
    private List<EnemyCell> _queueForVerification; //������ ������ ������ �� ������� ����� ������� ������ 
    private List<EnemyCell> _queueForDeletion; //������ ������ ������ ������� ����� ��������� ����� �������
    private List<EnemyCell> _mostBottomEnemy; //������ ����� ������ ������ �� ������� ����� ���������� ������� ����� ��������
    //private List<FallingEnemy> _listOfFallingEnemies; //������ ���� �������� ��������, ������� ����� ������� �� ������ � �������� ��������� ������ ����
    private const int _COUNT_ENEMY_IN_LINE = 8; //���������� ������ � �����
    private float _spacesSize; //������ �������
    private float _cellSize; //������ ������, ������������� �����������
    private float _speed; // �������� ����������� �����
    private float _targetPosition; // ����� �� ������� ����� ��������� ����� (��� ��������� ���������� �� ���� ��� ���� �� ���� ������ ������ ����� ��� ����� ����� ����������)
    private float _spawnNewLinePosition; // ��� ����������� ���� ����� ��������� ��������� ������, ����� ���������� ����� 
    private Vector2 _spawnPosition; // ��������� ������� ������ �����
   /* private int _countLine; */// ���������� ���������� ��������� ����� ����������, ������������� ��� �������� ������� � ������ �����
    private int _indexSpriteAndIdColor; // ���������� � �������� ����� �������������� ��������� ����� ��� �������� �����, � ���� ���������� ����� �������� �����, �� �������� ����� ������� ������, ��� �� ����� ����� ColorId ��� ���������
    private Dictionary<int, Dictionary<int, EnemyCell>> _deletetedCellDictionary; // ������� ������� ������ ������ �������� �� ��������, int �������� ������ ��������� ������� �� ���������� x, ������ �������� � ���� ������� ������ ������� � ������� �������� ��� ���� ��� �������� ������ �� ��� ��������� y, � �������� ������ �� ������
    //����� �������, ���� ��� ����� ����� ����������� ��� ���������� �������������, ������� � �������� �� x � ��� ����� ������� �������� �� y ����� �����������, � � ��� �� ��������� ������� ����� ��������. 
    private void Awake()
    {
        //_countLine = 1;
        _mostBottomEnemy = new List<EnemyCell>();
        //_listOfFallingEnemies = new List<FallingEnemy>();
        _enemiesLineList = new DoulbyLinkedListEnemiesLine();
        _queueForVerification = new List<EnemyCell>();
        _queueForDeletion = new List<EnemyCell>();
        _deletetedCellDictionary = new Dictionary<int, Dictionary<int, EnemyCell>>();
        _spawnNewLinePosition = 66f;
        _speed = 100f;
        _spacesSize = 400f;
        _cellSize = ((float)Screen.width - _spacesSize * 2.0f) / (float)_COUNT_ENEMY_IN_LINE;
        _targetPosition = (float)Screen.height;
        _spawnPosition = new Vector2(_spacesSize, -70f);

    }
    void Start()
    {
        CreatePirateLine();
    }

    void Update()
    {
        MoveLine();
        CheckSpawnNewLine();

    }
    private void CreatePirateLine()
    {
        DoublyLinkedListEnemyCell _enemiesCellList = new DoublyLinkedListEnemyCell();
        for (int i = 0; i < _COUNT_ENEMY_IN_LINE; i++)
        {
            EnemyCell _currentEnemyCell = CreateEnemyCell();
            RectTransform _currentEnemyCellRectTransform = (RectTransform)_currentEnemyCell.transform;
            CreateEnemy(_currentEnemyCellRectTransform, _currentEnemyCell);
            _currentEnemyCell.LinkLine = _enemiesCellList; //����������� ���� ������� ������ �� ����� � ������� ��� ���������
            _currentEnemyCell.NumberOfPositionInLine = i; //����������� ������ ����� ����� ������� ������ �����
            _enemiesCellList.Add(_currentEnemyCell);
            _currentEnemyCellRectTransform.anchoredPosition = new Vector2(_spawnPosition.x + _cellSize / 2.0f + i * _cellSize, _spawnPosition.y);
            /* ������� �������������� ��������� �������, ������� ��������� ������� ����� ������, � ��� ������������ �������� ������ (_cellSize/2.0f) 
             ��������� �������� ������� ��� �������� ���� �� ������ � ����� ����� ������� �� ��� ������� ������, ����� ������ ����������� �� ������ ������, ����� �������� ������������ �� �����*/
        }
        //_countLine++; //����� �������� � ���������� ������ ����� ������ ����������� ������� �� ����, ��� ��������� ������ ������ ���� ������� ���������� ����� ����� ������� �����
        _lastCreatedEnemyLineCoord = (RectTransform)_enemiesCellList.FirstCell().transform; //����� ������ ������ �� ��������� ��������� �����, ��� ���������� ��� ���������� ������ ����� �����
        _enemiesLineList.Add(_enemiesCellList);
    }
    private EnemyCell CreateEnemyCell() //����� �������� ������ � ������� �������� ����
    {
        EnemyCell enemyCell = Instantiate(_enemyCellPrefab, _gameField); //������� ������ �� �������, �������� ��� � ���� ��������� �������� ����
        enemyCell.SendTheSameNeighborsDelegate += AddEnemyToQueueVerification; //�� �������� ��������� ������ ����������� �����, ������� ����� ���������� ������ ���, �� ����� ������ �������� ���������� ������
        //���� � ������ �������� ������ �������������� ��������� � ���������� ������, ����� ��������� ����� AddPirateToQueueVerification ������� �������� � gamefield ������������ ������, ��� � ���� ������� ����� ��������� � ������
        //������������ �������, � ��� ����� �� ����� ������ �� ����� ������ ���������� ������� ��� � ������������ ����� �������.
        enemyCell.SendClickedEnemyCellDelegate += GetEnemyCellFromDelegate; //�� ������� ��������� ������ ����������� �����, ������� ����� ���������� ������� ������ ����� � ������ gamefield, ��� ���������� �����������. 
        return enemyCell;
    }
    private void CreateEnemy(RectTransform _enemyRectTransformCell, EnemyCell _enemyCell)
    {
        Enemy enemy = Instantiate(_enemyPrefab, _enemyRectTransformCell);
        _indexSpriteAndIdColor = Random.Range(0, _enemiesSprite.Count);
        enemy.EnemyIcon = _enemiesSprite[_indexSpriteAndIdColor];
        //enemy.SetIcon(_enemiesSprite[_indexSpriteAndIdColor]);
        enemy.ColorIdEnemy = _indexSpriteAndIdColor;
        _enemyCell.EnemyInCell = enemy;
    }
    private void AddEnemyToQueueVerification(EnemyCell en) //����� ��������, ������� ��������� ������ ����� � ��������� �� � ������ ������ ���������� ������
                                                             //���� ������ ��� ��������� � ������ �� �������� �� �� ��������� �� �� ��������
    {
        if (!_queueForDeletion.Contains(en))
        {
            _queueForVerification.Add(en);
        }
    }
    private void GetEnemyCellFromDelegate(EnemyCell enc) //����� ������� �������� �� ������� ��������� ������ SendClickedPirateCellDelegate, ��������� � Gamefield ��� �� ����� ������ ��� ������� (��� �������� ���� ���� ����� this)
    {
        _queueForDeletion.Add(enc); // ���������� ������ ����������� � ������ ������ ������� ������� ������������� ���������� ������ �� �������� (����� ���� ������� � ��������
        enc.FindTheSameNeighbors(); //� ���� ������ ����� �� ����� ������ ���������� ������, ���� ��� ��������� �� ���������� ����� ������� SendTheSameNeighborsDelegate, ������� � ������� ������ AddPirateToQueueVerification ��������� ��� ������ � ������ ��� ���������� �������� ��� �� ������� �� ���������
        FindAllTheNeighbors();     //����� ���� �������� ������ � ��� ������������ ������ � ������ ��������
        FillingDeleteCellDictionary(_queueForDeletion); //��������� ������� ������������� ��������� ������ �����
        �learTheQueueForDeletion();
        makeFall(_deletetedCellDictionary);
        //ClearDeletedCellDictionary();


    }

    private void FindAllTheNeighbors() //����� ����������� ������ ������� ������ ������������ �������� ������, ��� ���� �� ������ �������� ������� ������ ��������� ������, � ��� ���������� ����� ������� ���� ���������� �������� ������ ������,
                                       //���� ������� ����� ������� SendTheSameNeighborsDelegate �������� � �����  AddEnemyToQueueVerification ������� ������� ������������� ������ � ����� ������ ������� �� �����������, 
                                       //����� ������ ������ ������ �������,�� ������ _queueForVerification ��������� ���� ������ ������� � ����������� � ������ �� ��������, ����� ������� �� ����������� ���� ������� ������� ������
                                       //� ��� ��������� ���� ������� � ��� �� ��� ��� ���� ���������� �������� �� ���������� � ������ AddEnemyToQueueVerification �� �������� ����
    {
        while (_queueForVerification.Count > 0) 
        {
            _queueForVerification.First().FindTheSameNeighbors();
            _queueForDeletion.Add(_queueForVerification.First());
            _queueForVerification.RemoveAt(0);
        }
    }
    private void �learTheQueueForDeletion()
    {
        foreach (EnemyCell en in _queueForDeletion)
        {
            en.ClearCell(); 

        }
        _queueForDeletion.Clear();
    }
    private void ClearDeletedCellDictionary()//������� ������ ������� ������ �����
    {
        foreach (int key in _deletetedCellDictionary.Keys)
        {
            Dictionary<int, EnemyCell> _currentColumnCell = _deletetedCellDictionary[key];
            _currentColumnCell.Clear();
        }

    }
    private void MoveLine()
    {
        foreach (DoublyLinkedListEnemyCell pl in _enemiesLineList)
        {
            foreach (EnemyCell enc in pl) 
            {
                RectTransform EnemyCellRectTransform = enc.GetRectTransformCell();
                EnemyCellRectTransform.anchoredPosition += new Vector2(0, _speed * Time.deltaTime);
            }
        }
    }
    private void CheckSpawnNewLine()
    {
        if (_lastCreatedEnemyLineCoord.anchoredPosition.y > _spawnNewLinePosition)
        {
            CreatePirateLine();
        }
    }
    private void makeFall (Dictionary<int, Dictionary<int, EnemyCell>> _fallingEnemy) //������� ������� ������ �������� � ��������� ����������� ������� ������ �� ���������� �����
    {
        Dictionary<int, int> _targetCell_XYDictionary = new Dictionary<int, int>(); // ������� ������ x,y ���������� ������ �� ������� ����� ���������� ���������� � �������, ������� ����� ������������
        Dictionary<int, int> _startCell_XYDictionary = new Dictionary<int, int>(); // ������� ������ x,y ���������� ������ � ������� ��������� ���������� ���������� � �������, ������� ����� ������������
        Dictionary<int,int> _indentBottom = new Dictionary<int, int>(); // ������� ������� ������ ������� (���������� ���������� ������ ����� �����������), ������ �������� ������ x ����������, � ������ ���������� ������ ������ 
        if (_fallingEnemy.Count > 0)  //���� � ����� ������� �������� �������� ���� ������� ������� ����� ������
        {
            foreach (int key in _fallingEnemy.Keys) //����� ���������� �� �������� �������
            {
                Dictionary<int, EnemyCell> _currentColumnEnemy = _fallingEnemy[key]; // �� ����� ������� (������� ���������� ���������� ������� �� x), ���������� � ��������� ���������� ������� �������� �������
                _targetCell_XYDictionary.Add(key,_currentColumnEnemy.Keys.First()); //�� ������������ ������ �������� ������ ������ �����
                _startCell_XYDictionary.Add(key,_currentColumnEnemy.Keys.Last()-1); //�������� ������� ������ ������
                _indentBottom.Add(key, _currentColumnEnemy.Count); //�������� ���������� ����� ������� ����� ������� 
            }
        }
        //������ � ��������� ����������� ��� ���������� ������ ���������, ����� �� ��������� ������ ��������� ����������� ������
        foreach (int _XIndexEnemyCell in _startCell_XYDictionary.Keys) // �������� �� ���� �������� ������� ������ ������ ���������� ������� ������� ��������
        {
            int _startYCoordEnemyCell = _startCell_XYDictionary[_XIndexEnemyCell]; //� ������� ������� ����������� ��� ��������, ��� ����� Y ����������
            int _ident = _indentBottom[_XIndexEnemyCell]; //�������� ���������� ��������� �������� �� ���������������� �������
            for (int _YIndexOfStartCell = _startYCoordEnemyCell; _YIndexOfStartCell>=0; _YIndexOfStartCell--) //_startCell ��������� ������ �� ������� �� ����� ��������� �����, ���������� ������ ������
                                                                                //���� ��� �� ����� �� ���� ������ ���� ������ ���������� ����, ������� ����� ������������ ������
            {
                EnemyCell _currentEnemyCell = _enemiesLineList.FoundEnemyCellByIndex(_XIndexEnemyCell, _YIndexOfStartCell); //�� ����������� �������� ������ ����� �� ������ ������ ��������� ����� �������
                if(_currentEnemyCell.IsEnemyInside) //��������� ���� ������ �� ������
                {
                    Enemy _tmpCurrentEnemy = _currentEnemyCell.EnemyInCell; //��������� ����� �� ���� ������
                    EnemyCell _targetEnemyCell = _enemiesLineList.FoundEnemyCellByIndex(_XIndexEnemyCell, _YIndexOfStartCell + _ident); //����� ��������� ������ ����� ���� ����� ������������� �����, �������� ������� X ����������

                  //  _targetEnemyCell.border.color = Color.red;
                  
                    _tmpCurrentEnemy.EnemyRtransform.SetParent(_targetEnemyCell.GetRectTransformCell());//���������� ����� ����������� ������ ������������ ���� ���������
                   _tmpCurrentEnemy.setZeroRectTransform();

                    
                }


            }
        }
    }
    private void FillingDeleteCellDictionary(List<EnemyCell> deletedCells) //����� ��������� � ��������� ������� ������ �����, � ����� ��������� ������� �������� ������ �����, � �������� ��������� ���������� ������ ���� ����� ������� ������ ���� �������
    {
        foreach (EnemyCell cell in deletedCells) //����������� ������ ����� ������� ������ ���� �������, �� ����� ������ ����������� ������������ ������� (�������) ����� �� ��������, � ����������� ������� �������� ��� ����� �������
        {
            int Ypos = _enemiesLineList.FindLineAndGetNumber(cell.LinkLine); //�������� ���������� ������ �� y, ��������� ������ �� ����� � ������� ��� ��������, ���� ����� ���� �������� -1 (�������� IndexOf)
            if (Ypos != -1) //��������� ���������� �� Y ����������, ���� ����� � ������� �� ����� �������� -1
            {
                if (!_deletetedCellDictionary.ContainsKey(cell.NumberOfPositionInLine)) //�� ������ ����� �� ��������� �� �����������, ��� ����� x ����������, ���� � ����� ������� (_deletedCellDictionary) �������� ������ ������� ������ ����� (������������ �������) ����������� ������� �� x ����������,
                                                                                        //(x �� ����� �� ������, ��� �������� ��� ������ ���� ����� � �����)�� �� ������� ����� ������ �������, ���������� ���������� �� ��� x � �����  ������� ������� ��� �������� ������� �� y ����������
                {
                    _deletetedCellDictionary[cell.NumberOfPositionInLine] = new Dictionary<int, EnemyCell>()
                {
                    {Ypos, cell}
                }; //��������� ��������� � ����� �������, ������������� ������ �� x ���������� � ��������� ������ ������� ������� ����� ������� ��� ������ �� y ���������� (����� � ���� ������).
                   //����� ����� ��������� ������� ��������, ��� � ������� ����������� ����� ������ �����(x ���������� ��� ������� �������), � � ������� �������� ������� (���������� �������) �����������
                   //����� ����� (y ����������) � ��������������� ���� ������.

                }
                else
                {
                    _deletetedCellDictionary[cell.NumberOfPositionInLine].Add(Ypos, cell);//���� ������� ������ ������� ��� ������ ����� ������ ��������� ������ � ������ �� y ����������
                }

            }
        }
    }
}
