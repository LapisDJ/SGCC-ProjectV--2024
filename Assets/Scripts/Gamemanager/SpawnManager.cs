using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject slimePrefab, ratPrefab, batPrefab, zombieDogPrefab, zombiePrefab, elite1Prefab, elite2Prefab, oldRobotPrefab, smallSpiderRobotPrefab; // 각 몬스터의 프리펩
    public GameObject player; // 플레이어
    public float spawnDistance = 10.0f; //스폰 간격
    public LayerMask obstacleLayer; 
    private Camera mainCamera; // 카메라
    private float startTime; // 게임 시작 이후 경과시간

    private Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>(); // <몬스터, 큐>의 형태로 오브젝트 풀 구성

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬 넘어가도 파괴x
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
            objectPools[key].Enqueue(obj); // 풀에 담는다.
        }
    }

    GameObject GetFromPool(string key) // 풀에서 꺼내오는 함수
    {
        if (objectPools.ContainsKey(key)) // 인자로 받은 key가 dictionary에 있으면
        {
            GameObject obj = objectPools[key].Dequeue(); // <이름이 key인> 풀에서 front pop
            obj.SetActive(true); // pop된 몬스터 활성화
            objectPools[key].Enqueue(obj); // 활성화 된 상태로 rear에 push
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
        Vector3 spawnPosition = GetSpawnPosition(isDiagonal); // 스폰 위치 구하기
        if (spawnPosition != Vector3.zero) // 영벡터가 아니면
        {
            GameObject monster = GetFromPool(poolKey); // 풀에서 꺼내온다.
            if (monster != null) // 몬스터가 null이 아니면
            {
                monster.transform.position = spawnPosition; // 구한 스폰 위치에서 스폰시킨다.
            }
        }
    }

    Vector3 GetSpawnPosition(bool isDiagonal) // 스폰 위치 구하기
    {
        Vector3 playerPosition = player.transform.position; // 플레이어 위치

        float screenLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)).x; // 뷰포트 좌표계 -> 월드 좌표계 (뷰포트: (0, 0)이 좌하단 (1,1)이 우상단)
        float screenRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane)).x;
        float screenTop = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane)).y;
        float screenBottom = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)).y;

        Vector3 spawnPosition = Vector3.zero; // 스폰 포지션 : 일단 영벡터로 초기화
        int attemptCount = 0; // 시도 횟수(스폰 포지션이 벽일 경우 스폰 위치 이동)

        while (attemptCount < 10) // 최대 10번 탐색
        {
            attemptCount++;
            if (isDiagonal) // 대각 스폰이면
            {
                int diagonalDirection = Random.Range(0, 4); // 4가지 대각 위치 중 랜덤
                switch (diagonalDirection)
                {
                    case 0: spawnPosition = new Vector3(screenLeft - spawnDistance, screenTop + spawnDistance, 0); break;
                    case 1: spawnPosition = new Vector3(screenRight + spawnDistance, screenTop + spawnDistance, 0); break;
                    case 2: spawnPosition = new Vector3(screenLeft - spawnDistance, screenBottom - spawnDistance, 0); break;
                    case 3: spawnPosition = new Vector3(screenRight + spawnDistance, screenBottom - spawnDistance, 0); break;
                }
            }
            else // 대각 스폰이 아니면
            {
                int direction = Random.Range(0, 4); // 위치 랜덤 스폰
                switch (direction)
                {
                    case 0: spawnPosition = new Vector3(Random.Range(screenLeft, screenRight), screenTop + spawnDistance, 0); break;
                    case 1: spawnPosition = new Vector3(Random.Range(screenLeft, screenRight), screenBottom - spawnDistance, 0); break;
                    case 2: spawnPosition = new Vector3(screenLeft - spawnDistance, Random.Range(screenBottom, screenTop), 0); break;
                    case 3: spawnPosition = new Vector3(screenRight + spawnDistance, Random.Range(screenBottom, screenTop), 0); break;
                }
            }

            if (!Physics2D.OverlapCircle(spawnPosition, 0.5f, obstacleLayer)) // 구한 스폰 위치가 장애물이 아니면
            {
                return spawnPosition; // 해당 스폰 위치 반환
            }
        }

        return Vector3.zero; // 장애물 때문에 적합한 위치를 찾지 못한 경우
    }

    void SpawnCircleFormation(string poolKey, int radius, int count) // 원형 스폰
    {
        Vector3 center = player.transform.position; // 원의 중심 : 플레이어 좌표
        for (int i = 0; i < count; i++) // 스폰 개수만큼 반복
        {
            float angle = i * Mathf.PI * 2 / count; //등각도 스폰
            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius + center; // 스폰 좌표 = 원위 극좌표 + 플레이어 중심 좌표
            if (!Physics2D.OverlapCircle(position, 0.5f, obstacleLayer)) // 장애물이 아니면
            {
                GameObject monster = GetFromPool(poolKey); // 몬스터를 풀에서 가져온다.
                if (monster != null)
                {
                    monster.transform.position = position;
                }
            }
        }
    }

    void SpawnLineFormation(string poolKey, int rows, int countPerRow) // 플레이어 위치 기준 몬스터를 여러줄 스폰
    {
        Vector3 playerPosition = player.transform.position; // 플레이어 포지션
        float offset = 2.0f; // 떨어진 거리
        for (int i = 0; i < rows; i++) // 스폰할 줄만큼 반복
        {
            Vector3 rowStart = new Vector3(playerPosition.x - (countPerRow / 2) * offset, playerPosition.y + (i * offset), 0); 
            for (int j = 0; j < countPerRow; j++)
            {
                Vector3 position = rowStart + new Vector3(j * offset, 0, 0);
                if (!Physics2D.OverlapCircle(position, 0.5f, obstacleLayer))
                {
                    GameObject monster = GetFromPool(poolKey);
                    if (monster != null)
                    {
                        monster.transform.position = position;
                    }
                }
            }
        }
    }
}
