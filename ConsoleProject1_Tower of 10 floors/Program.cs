namespace ConsoleProject1_Tower_of_10_floors
{
    internal class Program
    {
        enum Scene
        {
            Select, Confirm, Lobby, floor1, floor2, floor3,
            floor4, floor5, floor6, floor7, floor8, floor9, floor10, inn, shop, upgrade
        }

        static bool[] flooarCleared = new bool[11];

        static void ClearFloor(Scene floor)
        {
            if (floor >= Scene.floor1 && floor <= Scene.floor10)
            {
                int floorIndex = (int)floor - (int)Scene.floor1 + 1;
                flooarCleared[floorIndex] = true;
            }
        }

        struct Inventory
        {
            public Dictionary<string, int> items;

            public Inventory()
            {
                items = new Dictionary<string, int>();
            }
        }


        struct GameData
        {
            public bool running;

            public Scene scene;

            public char[,] map;

            public string name;
            public int curHP;
            public int maxHP;
            public int attack;
            public int armor;
            public int gold;

            public Point playerPos;
            public Point goalPos;
            public Point previousPos;
            public ConsoleKey inputKey;

            public Inventory inventory;
        }

        static void AddItemToInventory(string item)
        {
            data.inventory.items.Add(item, 1);
            Console.WriteLine($"{item}가 인벤토리에 추가되었습니다.");
        }

        public struct Point
        {
            public int x;
            public int y;
        }


        static GameData data;

        static void UseKey()
        {
            if (data.inventory.items.ContainsKey("열쇠") && data.inventory.items["열쇠"] > 0)//인벤토리 아이템에 열쇠라는 종목이 포함되어있고 열쇠가 1개 이상인지 확인
            {
                data.inventory.items["열쇠"]--;
                Console.WriteLine("열쇠를 사용했습니다.");
            }
            else
            {
                Console.WriteLine("열쇠가 없습니다.");
                data.inventory.items.Remove("열쇠");
            }
        }

        struct Monster
        {
            public string name;
            public int hp;
            public int attack;
            public int armor;
            public Point position;
        }

        static List<Monster> monsters = new List<Monster>();

        static void CheckMonsterEncounter()
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                Monster monster = monsters[i];
                if (data.playerPos.x == monster.position.x && data.playerPos.y == monster.position.y)
                {
                    data.playerPos = data.previousPos;
                    Console.Clear();
                    Console.WriteLine($"몬스터 {monster.name}와 싸웁니다!");

                    while (true)
                    {
                        Render();
                        DisplayHP(data.name, data.curHP, data.maxHP);
                        DisplayHP(monster.name, monster.hp, monster.hp);

                        Console.WriteLine("공격하려면 아무 키나 누르세요...");
                        Console.ReadKey(true);

                        int userDamage = Math.Max(0, data.attack - monster.armor);//Max: 0과 유저공격력, 몬스터 방어력의 차이중 큰값
                        monster.hp -= userDamage;
                        Console.WriteLine($"{data.name}이(가) {monster.name}에게 {userDamage}의 피해를 입혔습니다.");
                        Wait(1);

                        if (monster.hp <= 0)
                        {
                            Console.WriteLine($"몬스터 {monster.name}을 처치하였습니다.");
                            monsters.RemoveAt(i);
                            Console.WriteLine("10골드와 열쇠를 받았습니다.(수정필요)");
                            Wait(2);
                            break;
                        }

                        int monsterDamage = Math.Max(0, monster.attack - data.armor);
                        data.curHP -= monsterDamage;
                        Console.WriteLine($"{monster.name}에게 {data.name}가 {monsterDamage}의 피해를 입었습니다.");
                        Wait(1);


                        if (data.curHP <= 0)
                        {
                            Console.WriteLine($"{data.name}이(가) 쓰러졌습니다.");
                            Console.WriteLine($"가지고 있는 돈의 10분의 1을 잃어버렸습니다.{data.gold * 0.1}g");
                            int lostGold = (int)(data.gold * 0.1);
                            data.gold -= lostGold;
                            data.curHP = data.maxHP;
                            Wait(2);
                            data.running = false;
                            break;
                        }

                        Console.Clear();
                    }
                    break;
                }
            }
        }



        static void DisplayHP(string name, int currentHP, int maxHP)
        {
            Console.Write($"{name}: ");
            for (int i = 0; i < maxHP; i += 5)
            {
                if (i < currentHP)
                {
                    Console.Write("|");
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine($" ({currentHP}/{maxHP})");
        }


        static char[,] Map1 = new char[,]
        {
            { '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', ' ', ' ', ' ', ' ', '#'},
            { '#', '#', ' ', '#', 'H', ' ', '#'},
            { '#', ' ', ' ', ' ', ' ', 'T', '#'},
            { '#', ' ', ' ', '#', ' ', ' ', '#'},
            { '#', ' ', ' ', '#', ' ', 'K', '#'},
            { '#', '#', '#', '#', '#', '#', '#'}
        };

        static char[,] Map2 = new char[,]
        {
            { '#', '#', '#', '#', '#', '#', '#'},
            { '#', 'K', 'T', ' ', ' ', ' ', '#'},
            { '#', ' ', '#', ' ', '#', '#', '#'},
            { '#', ' ', ' ', ' ', ' ', ' ', '#'},
            { '#', '#', '#', '#', ' ', ' ', '#'},
            { '#', ' ', ' ', ' ', ' ', 'H', '#'},
            { '#', '#', '#', '#', '#', '#', '#'}
        };

        static char[,] Map3 = new char[,]
        {
            { '#', '#', '#', '#', '#', '#', '#'},
            { '#', 'H', ' ', 'T', ' ', ' ', '#'},
            { '#', '#', '#', ' ', '#', ' ', '#'},
            { '#', ' ', ' ', ' ', ' ', ' ', '#'},
            { '#', ' ', '#', '#', '#', ' ', '#'},
            { '#', ' ', '#', 'K', ' ', ' ', '#'},
            { '#', '#', '#', '#', '#', '#', '#'}
        };

        static void LoadMap(int floor)
        {

            switch (floor)
            {
                case 1:
                    data.map = Map1;
                    data.playerPos = new Point { x = 1, y = 5 };
                    data.goalPos = new Point { x = 5, y = 1 };
                    monsters.Clear();
                    monsters.Add(new Monster { name = "슬라임", hp = 10, attack = 5, armor = 1, position = new Point { x = 3, y = 3 } });
                    monsters.Add(new Monster { name = "킹슬라임", hp = 50, attack = 10, armor = 5, position = new Point { x = 5, y = 1 } });
                    break;
                case 2:
                    data.map = Map2;
                    data.playerPos = new Point { x = 1, y = 5 };
                    data.goalPos = new Point { x = 5, y = 1 };
                    monsters.Add(new Monster { name = "고블린", hp = 30, attack = 7, armor = 3, position = new Point { x = 4, y = 4 } });
                    monsters.Add(new Monster { name = "홉고블린", hp = 100, attack = 15, armor = 10, position = new Point { x = 5, y = 1 } });
                    break;
                case 3:
                    data.map = Map3;
                    data.playerPos = new Point { x = 1, y = 5 };
                    data.goalPos = new Point { x = 5, y = 1 };
                    monsters.Add(new Monster { name = "코볼트", hp = 50, attack = 10, armor = 5, position = new Point { x = 1, y = 4 } });
                    monsters.Add(new Monster { name = "코볼트퀸", hp = 300, attack = 15, armor = 15, position = new Point { x = 5, y = 1 } });
                    break;
                case 4:
                    data.map = Map2;
                    data.playerPos = new Point { x = 1, y = 5 };
                    data.goalPos = new Point { x = 5, y = 1 };
                    monsters.Add(new Monster { name = "늑대", hp = 120, attack = 30, armor = 10, position = new Point { x = 3, y = 2 } });
                    monsters.Add(new Monster { name = "은빛 갈기 늑대", hp = 450, attack = 35, armor = 15, position = new Point { x = 5, y = 1 } });

                    break;
                case 5:
                    data.map = Map1;
                    data.playerPos = new Point { x = 1, y = 5 };
                    data.goalPos = new Point { x = 5, y = 1 };
                    monsters.Add(new Monster { name = "오크", hp = 200, attack = 20, armor = 20, position = new Point { x = 2, y = 2 } });
                    monsters.Add(new Monster { name = "로얄오크", hp = 700, attack = 30, armor = 30, position = new Point { x = 5, y = 1 } });
                    break;
                case 6:
                    data.map = Map3;
                    data.playerPos = new Point { x = 1, y = 5 };
                    data.goalPos = new Point { x = 5, y = 1 };
                    monsters.Add(new Monster { name = "트롤", hp = 800, attack = 25, armor = 25, position = new Point { x = 3, y = 3 } });
                    monsters.Add(new Monster { name = "핏빛달 트롤", hp = 2000, attack = 40, armor = 40, position = new Point { x = 5, y = 1 } });
                    break;
                case 7:
                    data.map = Map1;
                    data.playerPos = new Point { x = 1, y = 5 };
                    data.goalPos = new Point { x = 5, y = 1 };
                    monsters.Add(new Monster { name = "오우거", hp = 1500, attack = 40, armor = 30, position = new Point { x = 4, y = 4 } });
                    monsters.Add(new Monster { name = "트윈헤드 오우거", hp = 3000, attack = 65, armor = 50, position = new Point { x = 5, y = 1 } });

                    break;
                case 8:
                    data.map = Map2;
                    data.playerPos = new Point { x = 1, y = 5 };
                    data.goalPos = new Point { x = 5, y = 1 };
                    monsters.Add(new Monster { name = "골렘", hp = 4500, attack = 20, armor = 50, position = new Point { x = 4, y = 5 } });
                    monsters.Add(new Monster { name = "키메라", hp = 7000, attack = 40, armor = 70, position = new Point { x = 5, y = 1 } });

                    break;
                case 9:
                    data.map = Map3;
                    data.playerPos = new Point { x = 1, y = 5 };
                    data.goalPos = new Point { x = 5, y = 1 };
                    monsters.Add(new Monster { name = "거인", hp = 5000, attack = 35, armor = 70, position = new Point { x = 5, y = 4 } });
                    monsters.Add(new Monster { name = "거신", hp = 10000, attack = 50, armor = 100, position = new Point { x = 5, y = 1 } });
                    break;
                case 10:
                    data.map = Map2;
                    data.playerPos = new Point { x = 1, y = 5 };
                    data.goalPos = new Point { x = 5, y = 1 };
                    monsters.Add(new Monster { name = "와이번", hp = 7500, attack = 70, armor = 100, position = new Point { x = 1, y = 2 } });
                    monsters.Add(new Monster { name = "드래곤", hp = 25000, attack = 150, armor = 200, position = new Point { x = 5, y = 1 } });
                    break;

            }
        }

        static void PrintMap()
        {
            for (int y = 0; y < data.map.GetLength(0); y++)
            {
                for (int x = 0; x < data.map.GetLength(1); x++)
                {
                    char cell = data.map[y, x];
                    switch (cell)
                    {
                        case '#':
                            Console.Write("#");
                            break;
                        case ' ':
                            Console.Write(" ");
                            break;
                        case 'K':
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("K");
                            Console.ResetColor();
                            break;
                        case 'T':
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("T");
                            Console.ResetColor();
                            break;
                        case 'H':
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("H");
                            Console.ResetColor();
                            break;
                    }
                }
                Console.WriteLine();
            }

            for (int i = 0; i < monsters.Count; i++)
            {
                Monster monster = monsters[i];
                Console.SetCursorPosition(monster.position.x, monster.position.y);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("M");
                Console.ResetColor();
            }
        }


        static void Main(string[] args)
        {
            Start();

            while (data.running)
            {

                Run();
            }

            End();
        }

        static void Start()
        {
            Console.CursorVisible = false;

            data = new GameData();

            data.running = true;

            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("=                                  =");
            Console.WriteLine("=             마녀의 탑            =");
            Console.WriteLine("=                                  =");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("    계속하려면 아무키나 누르세요    ");
            Console.ReadKey();
        }

        static void End()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("=                                  =");
            Console.WriteLine("=           게임 클리어!           =");
            Console.WriteLine("=                                  =");
            Console.WriteLine("====================================");
            Console.WriteLine();
        }

        static void GameLoop()
        {
            while (data.running)
            {
                Render();
                Input();
                Update();
            }
            data.scene = Scene.Lobby;
            data.running = true;
        }

        static void Render()
        {
            Console.Clear();

            PrintMap();
            PrintPlayer();
            PrintGoal();

            int printprofileY = 15;
            PrintProfile(printprofileY);
            //로그박스 구현

        }

        static void Input()
        {
            data.inputKey = Console.ReadKey(true).Key;
        }

        static void Update()
        {
            Move();
            CheckMonsterEncounter();
            CheckFloorClear();
        }

        static void PrintPlayer()
        {
            Console.SetCursorPosition(data.playerPos.x, data.playerPos.y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("P");
            Console.ResetColor();
        }
        static void PrintGoal()
        {
            Console.SetCursorPosition(data.goalPos.x, data.goalPos.y);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("G");
            Console.ResetColor();
        }

        static void Move()
        {
            data.previousPos = data.playerPos;
            Point next = data.playerPos;

            switch (data.inputKey)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    next.y -= 1;
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    next.y += 1;
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    next.x -= 1;
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    next.x += 1;
                    break;
            }

            char nextCell = data.map[next.y, next.x];
            if (nextCell != '#')
            {
                data.playerPos = next;

                if (nextCell == 'K')
                {

                    //열쇠가 있을시에만 이동 가능 처리 로직
                    data.map[next.y, next.x] = ' '; // 열쇠를 통해 방을 열었을 시 빈 방으로 변경
                }
                else if (nextCell == 'T')
                {
                    Console.WriteLine("함정에 빠졌습니다!");
                    if (data.curHP <= 30)
                    {
                        Console.WriteLine("함정에 추락해 사망했습니다. 부상으로 체력 20을 잃은 채 로비에서 정신을 차랍니다.");
                        data.curHP = data.maxHP - 20;
                        data.running = false;
                    }
                    else
                    {
                        data.curHP -= 30;
                        Console.WriteLine("체력이 30 감소합니다.");
                    }
                    Wait(1);
                }
                else if (nextCell == 'H')
                {
                    Console.WriteLine("회복의 우물이 있습니다.");
                    if (data.curHP + 100 < data.maxHP)
                    {
                        data.curHP += 100;
                    }
                    else
                    {
                        data.curHP = data.maxHP;
                    }
                    Console.WriteLine("체력이 100 회복되었습니다.");
                    Wait(1);
                }
            }
        }

        static void CheckFloorClear()
        {
            if (data.playerPos.x == data.goalPos.x && data.playerPos.y == data.goalPos.y)
            {
                bool bossDefeated = true;
                for (int i = 0; i < monsters.Count; i++)
                {
                    Monster monster = monsters[i];
                    if (monster.position.x == data.goalPos.x && monster.position.y == data.goalPos.y)
                    {
                        bossDefeated = false;
                        break;
                    }
                }

                if (bossDefeated)
                {
                    Console.WriteLine($"{currentFloor}층을 클리어하셨습니다");
                    Wait(2);
                    ClearFloor((Scene)(currentFloor + (int)Scene.floor1 - 1));
                    data.scene = Scene.Lobby;
                    data.running = false;

                    if (currentFloor == 10)
                    {
                        data.running = false;
                        Console.Clear();
                    }
                }
            }
        }


        static void Run()
        {
            Console.Clear();
            switch (data.scene)
            {
                case Scene.Select:
                    SelectScene();
                    break;
                case Scene.Confirm:
                    ConfirmScene();
                    break;
                case Scene.Lobby:
                    LobbyScene();
                    break;
                case Scene.floor1:
                    floor1Scene();
                    break;
                case Scene.floor2:
                    floor2Scene();
                    break;
                case Scene.floor3:
                    floor3Scene();
                    break;
                case Scene.floor4:
                    floor4Scene();
                    break;
                case Scene.floor5:
                    floor5Scene();
                    break;
                case Scene.floor6:
                    floor6Scene();
                    break;
                case Scene.floor7:
                    floor7Scene();
                    break;
                case Scene.floor8:
                    floor8Scene();
                    break;
                case Scene.floor9:
                    floor9Scene();
                    break;
                case Scene.floor10:
                    floor10Scene();
                    break;

            }
        }

        static void PrintProfile(int startY)
        {
            Console.SetCursorPosition(0, startY);
            Console.WriteLine("=====================================");
            Console.WriteLine($"이름 : {data.name}");
            Console.WriteLine($"체력 : {data.curHP} / {data.maxHP}");
            Console.WriteLine($"공격력 : {data.attack} 방어력 : {data.armor} ");
            Console.WriteLine($"소지금 : {data.gold} G");
            Console.WriteLine("=====================================");
            Console.WriteLine();
        }

        static void PrintLobby()
        {
            Console.Clear();
            Console.WriteLine("=====================================");
            if (flooarCleared[10])
            {
                Console.WriteLine($"=          10층 (클리어됨)          =");

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"=               10층                =");
            }
            for (int i = 9; i >= 1; i--)
            {
                if (flooarCleared[i])
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"=          {i}층 (클리어됨)           =");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"=                {i}층                =");
                }
            }
            Console.ResetColor();
            Console.WriteLine($"=              강화소               =");
            Console.WriteLine($"=               상점                =");
            Console.WriteLine($"=               여관                =");
            Console.WriteLine("=====================================");
            Console.WriteLine();
        }

        static void Wait(float seconds)
        {
            Thread.Sleep((int)(seconds * 1000));
        }

        static void SelectScene()
        {
            Console.Write("캐릭터의 이름을 입력하세요 : ");
            data.name = Console.ReadLine();
            if (data.name == "")
            {
                Console.WriteLine("잘못된 입력입니다.");
                return;
            }
            data.maxHP = 200;
            data.curHP = data.maxHP;
            data.attack = 10;
            data.armor = 3;
            data.gold = 100;

            data.scene = Scene.Confirm;
        }

        static void ConfirmScene()
        {
            Console.WriteLine("===================");
            Console.WriteLine($"이름 : {data.name}");
            Console.WriteLine($"체력 : {data.maxHP}");
            Console.WriteLine($"공격력 : {data.attack}");
            Console.WriteLine($"민첩 : {data.armor}");
            Console.WriteLine($"소지금 : {data.gold}");
            Console.WriteLine("===================");
            Console.WriteLine();
            Console.Write("이 이름으로 결정하시겠습니까?(y/n)");

            string input = Console.ReadLine();

            // Update
            switch (input)
            {
                case "Y":
                case "y":
                    Console.Clear();
                    Console.WriteLine("탑의 로비로 이동합니다...");
                    Wait(2);
                    data.scene = Scene.Lobby;
                    break;
                case "N":
                case "n":
                    data.scene = Scene.Select;
                    break;
                default:
                    data.scene = Scene.Confirm;
                    break;
            }
        }

        static int currentFloor;
        static void LobbyScene()
        {
            PrintLobby();
            PrintProfile(15);

            Console.WriteLine("정체모를 탑에 떨어졌다.");
            Console.WriteLine("어디로 이동하겠습니까?");

            string input = Console.ReadLine();

            switch (input)
            {
                case "10층":
                case "10cmd":
                case "10":
                    if (flooarCleared[9])
                    {
                        data.scene = Scene.floor10;

                    }
                    else
                    {
                        Console.WriteLine("이전 층을 먼저 클리어 해야 합니다.");
                        Wait(2);
                    }
                    break;

                case "9층":
                case "9cmd":
                case "9":
                    if (flooarCleared[8])
                    {
                        data.scene = Scene.floor9;

                    }
                    else
                    {
                        Console.WriteLine("이전 층을 먼저 클리어 해야 합니다.");
                        Wait(2);
                    }
                    break;

                case "8층":
                case "8cmd":
                case "8":
                    if (flooarCleared[7])
                    {
                        data.scene = Scene.floor8;

                    }
                    else
                    {
                        Console.WriteLine("이전 층을 먼저 클리어 해야 합니다.");
                        Wait(2);
                    }
                    break;

                case "7층":
                case "7cmd":
                case "7":
                    if (flooarCleared[6])
                    {
                        data.scene = Scene.floor7;

                    }
                    else
                    {
                        Console.WriteLine("이전 층을 먼저 클리어 해야 합니다.");
                        Wait(2);
                    }
                    break;

                case "6층":
                case "6cmd":
                case "6":
                    if (flooarCleared[5])
                    {
                        data.scene = Scene.floor6;

                    }
                    else
                    {
                        Console.WriteLine("이전 층을 먼저 클리어 해야 합니다.");
                        Wait(2);
                    }
                    break;

                case "5층":
                case "5cmd":
                case "5":
                    if (flooarCleared[4])
                    {
                        data.scene = Scene.floor5;

                    }
                    else
                    {
                        Console.WriteLine("이전 층을 먼저 클리어 해야 합니다.");
                        Wait(2);
                    }
                    break;

                case "4층":
                case "4cmd":
                case "4":
                    if (flooarCleared[3])
                    {
                        data.scene = Scene.floor4;

                    }
                    else
                    {
                        Console.WriteLine("이전 층을 먼저 클리어 해야 합니다.");
                        Wait(2);
                    }
                    break;

                case "3층":
                case "3cmd":
                case "3":
                    if (flooarCleared[2])
                    {
                        data.scene = Scene.floor3;

                    }
                    else
                    {
                        Console.WriteLine("이전 층을 먼저 클리어 해야 합니다.");
                        Wait(2);
                    }
                    break;

                case "2층":
                case "2cmd":
                case "2":
                    if (flooarCleared[1])
                    {
                        data.scene = Scene.floor2;

                    }
                    else
                    {
                        Console.WriteLine("이전 층을 먼저 클리어 해야 합니다.");
                        Wait(2);
                    }
                    break;

                case "1층":
                case "1cmd":
                case "1":
                    data.scene = Scene.floor1;

                    //ClearFloor(Scene.floor1);

                    break;

                case "강화소":
                case "rkdghkth":
                case "강화":
                case "rkdghk":
                    Console.Clear();
                    Console.WriteLine("강화소로 이동합니다...");
                    Console.WriteLine("추후 개발 예정...");
                    Wait(2);

                    data.scene = Scene.Lobby;
                    break;

                case "상점":
                case "tkdwja":
                    Console.Clear();
                    Console.WriteLine("상점으로 이동합니다...");
                    Console.WriteLine("추후 개발 예정...");
                    Wait(2);

                    data.scene = Scene.Lobby;
                    break;

                case "여관":
                case "durhks":
                    Console.Clear();
                    Console.WriteLine("여관으로 이동합니다...");
                    Console.WriteLine("추후 개발 예정...");
                    Wait(2);

                    data.scene = Scene.Lobby;
                    break;
            }
        }
        static void floor1Scene()
        {
            Console.Clear();
            Console.WriteLine("1층으로 이동합니다...");
            currentFloor = 1;
            LoadMap(1);
            GameLoop();
        }

        static void floor2Scene()
        {
            Console.Clear();
            Console.WriteLine("2층으로 이동합니다...");
            currentFloor = 2;
            LoadMap(2);
            GameLoop();
        }

        static void floor3Scene()
        {
            Console.Clear();
            Console.WriteLine("3층으로 이동합니다...");
            currentFloor = 3;
            LoadMap(3);
            GameLoop();
        }

        static void floor4Scene()
        {
            Console.Clear();
            Console.WriteLine("4층으로 이동합니다...");
            currentFloor = 4;
            LoadMap(4);
            GameLoop();
        }

        static void floor5Scene()
        {
            Console.Clear();
            Console.WriteLine("5층으로 이동합니다...");
            currentFloor = 5;
            LoadMap(5);
            GameLoop();
        }

        static void floor6Scene()
        {
            Console.Clear();
            Console.WriteLine("6층으로 이동합니다...");
            currentFloor = 6;
            LoadMap(6);
            GameLoop();
        }

        static void floor7Scene()
        {
            Console.Clear();
            Console.WriteLine("7층으로 이동합니다...");
            currentFloor = 7;
            LoadMap(7);
            GameLoop();
        }

        static void floor8Scene()
        {
            Console.Clear();
            Console.WriteLine("8층으로 이동합니다...");
            currentFloor = 8;
            LoadMap(8);
            GameLoop();
        }

        static void floor9Scene()
        {
            Console.Clear();
            Console.WriteLine("9층으로 이동합니다...");
            currentFloor = 9;
            LoadMap(9);
            GameLoop();
        }

        static void floor10Scene()
        {
            Console.Clear();
            Console.WriteLine("10층으로 이동합니다...");
            currentFloor = 10;
            LoadMap(10);
            GameLoop();
        }
    }
}
