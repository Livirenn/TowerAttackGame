using Assets.Script;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameField : MonoBehaviour
{

    [SerializeField] private RectTransform _gameField; // RectTransform игрового поля
    [SerializeField] private EnemyCell _enemyCellPrefab; // Префаб ячейки врага
    [SerializeField] private Enemy _enemyPrefab; // Префаб врага
    [SerializeField] private List<Sprite> _enemiesSprite;// Список хранит спрайты врагов
    private RectTransform _lastCreatedEnemyLineCoord; // Переменная для хранения последней созданной линии, задается при создании линии, берется первый элемент и записывается его рект трансформ
    private DoublyLinkedListEnemyCell _enemiesCellList;
    private DoulbyLinkedListEnemiesLine _enemiesLineList;
    private List<EnemyCell> _queueForVerification; //Список хранит ячейки из которых будут удалены пираты 
    private List<EnemyCell> _queueForDeletion; //Список хранит ячейки которые будут проверять своих соседей
    private List<EnemyCell> _mostBottomEnemy; //Список самых нижних врагов до которых будут опускаться верхние после удаления
    //private List<FallingEnemy> _listOfFallingEnemies; //Список всех падающих объектов, объекты будут браться из списка и изменять положение каждый кадр
    private const int _COUNT_ENEMY_IN_LINE = 8; //Количество врагов в линии
    private float _spacesSize; //Размер отступа
    private float _cellSize; //Размер ячейки, расчитывается динамически
    private float _speed; // Скорость перемещения линии
    private float _targetPosition; // Точка до которой будет двигаться линия (она определит закончится ли игра или если до сюда дойдет пустая линия эта линия будет уничтожена)
    private float _spawnNewLinePosition; // При прохождении этой точки последней созданной линией, будет спавниться новая 
    private Vector2 _spawnPosition; // Стартовая позиция спавна линии
   /* private int _countLine; */// Записывает количество созданных линий переменная, предназначена для передачи позиции в ячейку врага
    private int _indexSpriteAndIdColor; // Переменная в ыкоторой будет генерироваться рандомное число при создании линии, в этой переменной будет хранится число, по которому будет браться спрайт, это же число будет ColorId для сравнения
    private Dictionary<int, Dictionary<int, EnemyCell>> _deletetedCellDictionary; // Словарь который хранит список столбцов на удаление, int значение хранит положение столбца по координате x, второе значение в виде словаря хранит столбец с пустыми ячейками где ключ это значение ячейки по оси координат y, а значение ссылка на ячейку
    //ключи словаря, если это целые числа сортируются при добавлении автоматически, поэтому и значения по x и что самое главное значение по y будут упорядочены, и у нас не возникнет никаких багов смещения. 
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
            _currentEnemyCell.LinkLine = _enemiesCellList; //Присваиваем всем ячейкам ссылку на линию в которой они находятся
            _currentEnemyCell.NumberOfPositionInLine = i; //Присваиваем ячейке врага номер позиции внутри линии
            _enemiesCellList.Add(_currentEnemyCell);
            _currentEnemyCellRectTransform.anchoredPosition = new Vector2(_spawnPosition.x + _cellSize / 2.0f + i * _cellSize, _spawnPosition.y);
            /* Позиция рассчитывается следующим образом, берется стартовая позиция спавн позишн, к ней прибавляется половина клетки (_cellSize/2.0f) 
             поскольку привязка объекта при создании идет по центру и линия будет смещена на пол корпуса ячейки, далее просто увеличиваем на размер ячейки, затем операция продолжается по циклу*/
        }
        //_countLine++; //После создания и присвоения номера линии ячейки увеличиваем счетчик на один, при следующем вызове метода всем ячейкам присвоится новый номер позиции линии
        _lastCreatedEnemyLineCoord = (RectTransform)_enemiesCellList.FirstCell().transform; //Берем первую ячейку из последней созданной линии, она необходима для вычисления спавна новой линии
        _enemiesLineList.Add(_enemiesCellList);
    }
    private EnemyCell CreateEnemyCell() //Метод создания ячейки в которой хранится враг
    {
        EnemyCell enemyCell = Instantiate(_enemyCellPrefab, _gameField); //Создаем объект из префаба, помещаем его в рект трансформ игрового поля
        enemyCell.SendTheSameNeighborsDelegate += AddEnemyToQueueVerification; //На делегаты созданной ячейки подписываем метод, который будет вызываться внутри нее, во время поиска соседних одинаковых врагов
        //если в списке соседних врагов обнаруживается противник с одинаковым цветом, тогда вызовется метод AddPirateToQueueVerification который передаст в gamefield обнаруженную ячейку, она в свою очередь будет добавлена в список
        //обнаруженных соседей, и уже потом по этому списку мы будем искать одинаковых соседей уже у обнаруженных ранее соседей.
        enemyCell.SendClickedEnemyCellDelegate += GetEnemyCellFromDelegate; //На делегат созданной ячейки подписываем метод, который будет передавать нажатую ячейку врага в объект gamefield, для дальнейших манипуляций. 
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
    private void AddEnemyToQueueVerification(EnemyCell en) //Метод делегата, который принимает ячейку врага и добавляет ее в список поиска одинаковых врагов
                                                             //если ячейка уже находится в списке на удаление то не добавляем ее на проверку
    {
        if (!_queueForDeletion.Contains(en))
        {
            _queueForVerification.Add(en);
        }
    }
    private void GetEnemyCellFromDelegate(EnemyCell enc) //Метод который подписан на делегат вражеской ячейки SendClickedPirateCellDelegate, принимает в Gamefield эту же самую ячейку при нажатии (она передает сама себя через this)
    {
        _queueForDeletion.Add(enc); // Переданная ячейка добавляется в другой список который соберет окончательное количество врагов на удаление (после всех поисков и проверок
        enc.FindTheSameNeighbors(); //У этой ячейки через ее метод ищутся одинаковые соседи, если они находятся то передаются через делегат SendTheSameNeighborsDelegate, который с помощью метода AddPirateToQueueVerification добавляет эти ячейки в список для дальнейшей проверки уже их соседей на равенство
        FindAllTheNeighbors();     //Метод ищет соседних врагов у уже обнаруженных врагов в первой итерации
        FillingDeleteCellDictionary(_queueForDeletion); //Заполняем словарь обнаруженными столбцами пустых ячеек
        СlearTheQueueForDeletion();
        makeFall(_deletetedCellDictionary);
        //ClearDeletedCellDictionary();


    }

    private void FindAllTheNeighbors() //Метод прочесывает список который хранит обнаруженных соседних врагов, при этом на каждой итерации берется первая вражеская ячейка, у нее вызывается метод который ищет одинаковых соседних врагов ячейки,
                                       //если находит через делегат SendTheSameNeighborsDelegate передает в метод  AddEnemyToQueueVerification который добавит обнаруженного пирата в конец списка который мы прочесываем, 
                                       //после вызова метода поиска соседей,из списка _queueForVerification удалеется этот первый элемент и добавляется в список на удаление, таким образом мы прочесываем всех соседей нажатой ячейки
                                       //у них проверяем тоже соседей и так до тех пор пока одинаковые элементы не закончатся и список AddEnemyToQueueVerification не окажется пуст
    {
        while (_queueForVerification.Count > 0) 
        {
            _queueForVerification.First().FindTheSameNeighbors();
            _queueForDeletion.Add(_queueForVerification.First());
            _queueForVerification.RemoveAt(0);
        }
    }
    private void СlearTheQueueForDeletion()
    {
        foreach (EnemyCell en in _queueForDeletion)
        {
            en.ClearCell(); 

        }
        _queueForDeletion.Clear();
    }
    private void ClearDeletedCellDictionary()//функция чистит словарь пустых ячеек
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
    private void makeFall (Dictionary<int, Dictionary<int, EnemyCell>> _fallingEnemy) //Обходим словарь пустых столбцов и выполняем перемещение верхних врагов на опустевшие места
    {
        Dictionary<int, int> _targetCell_XYDictionary = new Dictionary<int, int>(); // Словарь хранит x,y координату клетки до которой будут опускаться противники в столбце, занимая места уничтоженных
        Dictionary<int, int> _startCell_XYDictionary = new Dictionary<int, int>(); // Словарь хранит x,y координату клетки с которой начнуться опускаться противники в столбце, занимая места уничтоженных
        Dictionary<int,int> _indentBottom = new Dictionary<int, int>(); // Словарь который хранит отступы (количество опустевших клеток после уничтожения), первое значение хранит x координату, а второе количество пустых клеток 
        if (_fallingEnemy.Count > 0)  //Если в нашем словаре падающих объектов есть столбцы которые будут падать
        {
            foreach (int key in _fallingEnemy.Keys) //Тогда проходимся по индексам словаря
            {
                Dictionary<int, EnemyCell> _currentColumnEnemy = _fallingEnemy[key]; // По ключу словаря (который обозначает координату столбца по x), записываем в отдельную переменную словарь хранящий столбец
                _targetCell_XYDictionary.Add(key,_currentColumnEnemy.Keys.First()); //Из извлеченного столба получаем нижнюю пустую ячеку
                _startCell_XYDictionary.Add(key,_currentColumnEnemy.Keys.Last()-1); //Получаем верхнюю пустую ячейку
                _indentBottom.Add(key, _currentColumnEnemy.Count); //Получаем количество ячеек которые будут удалены 
            }
        }
        //Списки с ключевыми параметрами для дальнейшей работы заполнены, далее по собранным данным выполняем перемещение врагов
        foreach (int _XIndexEnemyCell in _startCell_XYDictionary.Keys) // Проходим по всем индексам словаря откуда должно начинаться падение верхних объектов
        {
            int _startYCoordEnemyCell = _startCell_XYDictionary[_XIndexEnemyCell]; //У каждого индекса запрашиваем его значение, это будет Y координата
            int _ident = _indentBottom[_XIndexEnemyCell]; //Получаем количество удаленных объектов из соответствующего словаря
            for (int _YIndexOfStartCell = _startYCoordEnemyCell; _YIndexOfStartCell>=0; _YIndexOfStartCell--) //_startCell начальная ячейка от которой мы будем двигаться вверх, прочесывая каждую ячейку
                                                                                //если она не пуста то враг внутри этой ячейки спускается вниз, занимая места уничтоженных врагов
            {
                EnemyCell _currentEnemyCell = _enemiesLineList.FoundEnemyCellByIndex(_XIndexEnemyCell, _YIndexOfStartCell); //По координатам получаем ячейку врага из общего списка созданных линий пиратов
                if(_currentEnemyCell.IsEnemyInside) //Проверяем если ячейка не пустая
                {
                    Enemy _tmpCurrentEnemy = _currentEnemyCell.EnemyInCell; //Извлекаем врага из этой ячейки
                    EnemyCell _targetEnemyCell = _enemiesLineList.FoundEnemyCellByIndex(_XIndexEnemyCell, _YIndexOfStartCell + _ident); //Затем извлекаем ячейку врага куда будет производиться спуск, передаем текущую X координату

                  //  _targetEnemyCell.border.color = Color.red;
                  
                    _tmpCurrentEnemy.EnemyRtransform.SetParent(_targetEnemyCell.GetRectTransformCell());//Найденному врагу присваиваем другой родительский рект трансформ
                   _tmpCurrentEnemy.setZeroRectTransform();

                    
                }


            }
        }
    }
    private void FillingDeleteCellDictionary(List<EnemyCell> deletedCells) //Метод заполняет и формирует словарь пустых ячеек, а также заполняет словарь словарей пустых ячеек, в качестве аргумента передается список всех ячеек которые должны быть удалены
    {
        foreach (EnemyCell cell in deletedCells) //Прочесываем список ячеек которые должны быть удалены, из этого списка формируются вертикальные столбцы (словари) ячеек на удаление, и формируется словарь хранящий эти самые столбцы
        {
            int Ypos = _enemiesLineList.FindLineAndGetNumber(cell.LinkLine); //Получаем координату ячейки по y, передавая ссылку на линию в которой она хранится, если линии нету вернется -1 (согласно IndexOf)
            if (Ypos != -1) //Проверяем существует ли Y координата, Если линии с ячейкой не будет вернется -1
            {
                if (!_deletetedCellDictionary.ContainsKey(cell.NumberOfPositionInLine)) //Из ячейки берем ее положение по горизонтали, это будет x координата, если в нашем словаре (_deletedCellDictionary) хранящим другие словари пустых ячеек (вертикальные столбцы) отсутствует столбец по x координате,
                                                                                        //(x мы берем из ячейки, при создании они хранят свой номер в линии)то мы создаем новую запись словаре, присваивая координату по оси x и затем  выделяя словарь для хранения столбца по y координате
                {
                    _deletetedCellDictionary[cell.NumberOfPositionInLine] = new Dictionary<int, EnemyCell>()
                {
                    {Ypos, cell}
                }; //Создается заготовка в нашем словаре, присваивается индекс по x координате и создается пустой словарь который будет хранить все ячейки по y координате (номер и саму ячейку).
                   //После этого заполняем словарь словарей, где в индексе прописываем номер внутри линии(x координата это внешний словарь), а в словаре хранящим столбцы (внутренний словарь) прописываем
                   //номер линии (y координату) и непосредственно саму ячейку.

                }
                else
                {
                    _deletetedCellDictionary[cell.NumberOfPositionInLine].Add(Ypos, cell);//если словарь внутри словаря уже создан тогда просто добавляем ячейку и индекс по y координате
                }

            }
        }
    }
}
