using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;
    public GameObject slimePrefab, ratPrefab, batPrefab, zombieDogPrefab, zombiePrefab, elite1Prefab, elite2Prefab, oldRobotPrefab, smallSpiderRobotPrefab; // 각 몬스터의 프리펩
    public GameObject player; // 플레이어
    public float spawnDistance = 10.0f; //스폰 간격
    [SerializeField] public LayerMask obstacleLayer;
    private Camera mainCamera; // 카메라
    private float startTime; // 게임 시작 이후 경과시간

    public Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>(); // <몬스터, 큐>의 형태로 오브젝트 풀 구성
    public Queue<GameObject> activeObjects = new Queue<GameObject>(); // 리스트 대신 큐로 변경                      // 추가
    private Queue<Vector3> spawnPointsQueue = new Queue<Vector3>(); // 동시 스폰을 위한, 해당 스폰 타이밍마다의 가능한 스폰 포인트 좌표 큐
    [SerializeField] public Tilemap tilemap; // 유효 스폰 위치 검사 위한 타일맵

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindWithTag("Player");
        tilemap = GameObject.Find("Background").GetComponent<Tilemap>();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로드될 때마다 호출
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 메모리 누수 방지
    }

    void Start()
    {
        mainCamera = Camera.main; // 메인 카메라 변수에 카메라 할당
        InitializeObjectPools(); // 풀 초기화
        startTime = Time.time; // map1씬 로드된 시점의 시간
        StartCoroutine(SpawnMonsters()); // 코루틴으로 몬스터 스폰 시작
    }

    void InitializeObjectPools() // 풀 초기화 함수
    {
        CreatePool("Slime", slimePrefab, 100);
        CreatePool("Rat", ratPrefab, 100);
        CreatePool("Bat", batPrefab, 100);
        CreatePool("ZombieDog", zombieDogPrefab, 100);
        CreatePool("Zombie", zombiePrefab, 100);
        CreatePool("Elite1", elite1Prefab, 10);
        CreatePool("Elite2", elite2Prefab, 10);
        CreatePool("OldRobot", oldRobotPrefab, 100);
        CreatePool("SmallSpiderRobot", smallSpiderRobotPrefab, 100);
    }

    void CreatePool(string key, GameObject prefab, int count) // 풀 제작 함수
    {
        objectPools[key] = new Queue<GameObject>(); //<이름이 key인> 몬스터의 풀(큐) 선언 
        for (int i = 0; i < count; i++) // 최초 선언시 원하는 만큼 생성 후 큐에 담는다.
        {
            GameObject obj = Instantiate(prefab); // 객체 생성
            obj.SetActive(false); // 비활성화 상태로
            DontDestroyOnLoad(obj);
            objectPools[key].Enqueue(obj); // 풀에 담는다.
        }
    }

    GameObject GetFromPool(string key) // 풀에서 꺼내오는 함수
    {
        if (objectPools.ContainsKey(key)) // 인자로 받은 key가 dictionary에 있으면
        {
            GameObject obj = objectPools[key].Dequeue(); // <이름이 key인> 풀에서 front pop
            obj.SetActive(true); // pop된 몬스터 활성화

            activeObjects.Enqueue(obj); // 활성화된 오브젝트를 activeObjects 큐에 추가 (FIFO)

            //objectPools[key].Enqueue(obj); // 활성화 된 상태로 rear에 push
            return obj; //pop된 몬스터 반환
        }
        return null; // 아니면 null 반환
    }

    IEnumerator SpawnMonsters() // 몬스터 스폰하는 코루틴 함수
    {
        while (true) // 무한 루프
        {
            float gameTime = Time.time - startTime; // 총 경과시간 - map1씬 로드 시간 = 게임 시작 후 경과 시간

            if (gameTime < 60) // 0~1분: 슬라임만 소환
            {
                SpawnMonster("Slime");
            }
            else if (gameTime < 180) // 1~3분: 거대 쥐만 소환
            {
                SpawnMonster("Rat");
            }
            else if (gameTime < 240) // 3~4분: 거대쥐와 거대 박쥐 소환
            {
                SpawnMonster("Rat");
                SpawnMonster("Bat", isDiagonal: true);
            }
            else if (gameTime < 300) // 4~5분: 좀비화된 개만 소환
            {
                SpawnMonster("ZombieDog");
            }
            else if (gameTime == 360) // 5분: 정예 몹1 소환, 주변에 좀비 원형 소환
            {
                SpawnMonster("Elite1");
                SpawnCircleFormation("Zombie", 3, 8); // 원형 소환
            }
            else if (gameTime < 420) // 5~7분: 좀비화된 개와 좀비 소환
            {
                SpawnMonster("ZombieDog", isDiagonal: true); // 대각선
                SpawnMonster("Zombie");
            }
            else if (gameTime < 540) // 7~9분: 좀비와 약간 노후화된 로봇 소환
            {
                SpawnMonster("Zombie");
                SpawnMonster("OldRobot");
            }
            else if (gameTime < 600) // 9~10분: 좀비화된 개, 좀비, 노후화된 로봇 모두 소환
            {
                SpawnMonster("ZombieDog", isDiagonal: true);
                SpawnMonster("Zombie");
                SpawnMonster("OldRobot");
            }
            else if (gameTime == 600) // 10분: 정예 몹2 소환, 주변으로 노후화된 로봇 원형 소환
            {
                SpawnMonster("Elite2");
                SpawnCircleFormation("OldRobot", 4, 12); // 원형 소환
            }
            else // 10분 이후: 노후화된 로봇만 소환
            {
                SpawnMonster("OldRobot");
                if ((int)gameTime % 30 == 0)
                {
                    SpawnLineFormation("SmallSpiderRobot", 5, 10);
                }
            }

            yield return new WaitForSeconds(3.0f); // 스폰 간격
        }
    }

    void SpawnMonster(string poolKey, bool isDiagonal = false) // 몬스터 스폰시키기
    {
        GetSpawnPosition(isDiagonal);
        while (spawnPointsQueue.Count > 0) // 큐에 스폰 포인트가 남아 있는지 확인s
        {
            Vector3 spawnPosition = spawnPointsQueue.Dequeue(); // 큐에서 위치를 꺼낸다.

            GameObject monster = GetFromPool(poolKey); // 풀에서 몬스터를 가져온다.
            if (monster != null) // 몬스터가 null이 아니면
            {
                monster.transform.position = spawnPosition; // 큐에서 꺼낸 위치에 몬스터를 스폰시킨다.
                                                            // SimplePathfinding 컴포넌트를 가져온다.
                SimplePathfinding pathfinding = monster.GetComponent<SimplePathfinding>();
                if (pathfinding != null)
                {
                    pathfinding.player = GameObject.FindGameObjectWithTag("Player").transform;
                    pathfinding.backgroundTilemap = GameObject.Find("Background").GetComponent<Tilemap>();
                    pathfinding.obstacleTilemaps = new Tilemap[7] {
                    GameObject.Find("Left").GetComponent<Tilemap>(),
                    GameObject.Find("Right").GetComponent<Tilemap>(),
                    GameObject.Find("Top").GetComponent<Tilemap>(),
                    GameObject.Find("Bottom").GetComponent<Tilemap>(),
                    GameObject.Find("Wall").GetComponent<Tilemap>(),
                    GameObject.Find("Wreck").GetComponent<Tilemap>(),
                    GameObject.Find("Building").GetComponent<Tilemap>()
                };
                    pathfinding.monster = monster.transform;
                }
                else
                {
                    Debug.LogWarning("SimplePathfinding 컴포넌트를 찾을 수 없습니다.");
                }
            }
        }

    }

    void GetSpawnPosition(bool isDiagonal)
    {
        Vector3 playerPosition = player.transform.position; // 플레이어 좌표

        //카메라 좌표 -> 월드 좌표로 변환
        float screenLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)).x;
        float screenRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane)).x;
        float screenTop = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane)).y;
        float screenBottom = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)).y;

        Vector3[] spawnPositions = new Vector3[8]; // 일단 기본 스폰 위치 8개 포인트 설정

        // Define the 8 spawn points based on the diagram
        spawnPositions[0] = new Vector3(screenLeft - spawnDistance, screenTop + spawnDistance, 0); // 대각선
        spawnPositions[1] = new Vector3((screenLeft + screenRight) / 2, screenTop + spawnDistance, 0);
        spawnPositions[2] = new Vector3(screenRight + spawnDistance, screenTop + spawnDistance, 0); // 대각선
        spawnPositions[3] = new Vector3(screenRight + spawnDistance, (screenTop + screenBottom) / 2, 0);
        spawnPositions[4] = new Vector3(screenRight + spawnDistance, screenBottom - spawnDistance, 0); // 대각선
        spawnPositions[5] = new Vector3((screenLeft + screenRight) / 2, screenBottom - spawnDistance, 0);
        spawnPositions[6] = new Vector3(screenLeft - spawnDistance, screenBottom - spawnDistance, 0); // 대각선
        spawnPositions[7] = new Vector3(screenLeft - spawnDistance, (screenTop + screenBottom) / 2, 0);

        for (int i = 0; i < 8; i++)
        {
            if (isDiagonal && (i % 2 != 0)) // 대각선 스폰일 경우, 대각선 위치만 고려
            {
                continue;
            }
            Vector3Int tilePosition = tilemap.WorldToCell(spawnPositions[i]); // 월드 좌표를 타일맵 셀 좌표로 변환
            TileBase tile = tilemap.GetTile(tilePosition); // 해당 위치의 타일을 가져옴

            if (!Physics2D.OverlapCircle(spawnPositions[i], 0.5f, obstacleLayer) && (tile != null) && tilemap.HasTile(tilePosition) && tilemap.gameObject.CompareTag("map")) //해당 스폰 포지션이 맵 밖이 아니거나 장애물 위가 아니면
                spawnPointsQueue.Enqueue(spawnPositions[i]);
        }
    }


    void SpawnCircleFormation(string poolKey, int radius, int count) // 원형 스폰
    {
        Vector3 center = player.transform.position; // 원의 중심 : 플레이어 좌표

        for (int i = 0; i < count; i++) // 스폰 개수만큼 반복
        {
            float angle = i * Mathf.PI * 2 / count; // 등각도 스폰
            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius + center; // 스폰 좌표 = 원위 극좌표 + 플레이어 중심 좌표

            Vector3Int tilePosition = tilemap.WorldToCell(position); // 월드 좌표를 타일맵 셀 좌표로 변환
            TileBase tile = tilemap.GetTile(tilePosition); // 해당 위치의 타일을 가져옴

            // 타일이 존재하고, 해당 타일맵의 타일이 "map" 태그를 가지고 있으며, 해당 위치가 장애물이 아닌 경우
            if (tile != null && tilemap.HasTile(tilePosition) && tilemap.gameObject.CompareTag("map") &&
                !Physics2D.OverlapCircle(position, 0.5f, obstacleLayer))
            {
                GameObject monster = GetFromPool(poolKey); // 몬스터를 풀에서 가져옴
                if (monster != null)
                {
                    monster.transform.position = position; // 몬스터를 해당 위치에 스폰
                }
            }
        }
    }


    void SpawnLineFormation(string poolKey, int rows, int countPerRow) // 플레이어 위치 기준 몬스터를 여러 줄 스폰
    {
        Vector3 playerPosition = player.transform.position; // 플레이어 포지션
        float offset = 2.0f; // 떨어진 거리

        for (int i = 0; i < rows; i++) // 스폰할 줄만큼 반복
        {
            Vector3 rowStart = new Vector3(playerPosition.x - (countPerRow / 2) * offset, playerPosition.y + (i * offset), 0);

            for (int j = 0; j < countPerRow; j++)
            {
                Vector3 position = rowStart + new Vector3(j * offset, 0, 0);

                Vector3Int tilePosition = tilemap.WorldToCell(position); // 월드 좌표를 타일맵 셀 좌표로 변환
                TileBase tile = tilemap.GetTile(tilePosition); // 해당 위치의 타일을 가져옴

                // 타일이 존재하고, 해당 타일맵의 타일이 "map" 태그를 가지고 있으며, 해당 위치가 장애물이 아닌 경우
                if (tile != null && tilemap.HasTile(tilePosition) && tilemap.gameObject.CompareTag("map") &&
                    !Physics2D.OverlapCircle(position, 0.5f, obstacleLayer))
                {
                    GameObject monster = GetFromPool(poolKey); // 몬스터를 풀에서 가져옴
                    if (monster != null)
                    {
                        monster.transform.position = position; // 몬스터를 해당 위치에 스폰
                    }
                }
            }
        }
    }

    public void DestroyAllPools()
    {
        foreach (var pool in objectPools)
        {
            // 각각의 몬스터 풀(queue)에 있는 모든 객체를 가져와서 파괴
            while (pool.Value.Count > 0)
            {
                GameObject obj = pool.Value.Dequeue();

                // 게임 오브젝트가 파괴되지 않았으면 파괴
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }

        DestroyActiveObjects();

        // 딕셔너리 초기화 (objectPools를 비운다)
        objectPools.Clear();
        activeObjects.Clear();
    }

    // 활성화된 오브젝트들을 다시 풀에 담는 함수
    public void DestroyActiveObjects()
    {
        // 1. activeObjects 큐에 있는 모든 오브젝트를 Destroy
        while (activeObjects.Count > 0) // activeObjects 큐에 오브젝트가 남아 있을 때까지
        {
            GameObject obj = activeObjects.Dequeue(); // activeObjects에서 오브젝트를 꺼냄
            if (obj != null && obj.activeInHierarchy)
            {
                Destroy(obj); // 오브젝트를 파괴
            }
        }
        activeObjects.Clear();
    }



}
