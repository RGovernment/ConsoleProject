using ConsoleGameFramework.Core;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using static ConsoleGameFramework.Common.Constants;
using static ConsoleGameFramework.Common.Enums;
namespace ConsoleGameFramework.Common;

public static class Utility
{
    /// <summary>
    /// 소수 판별 함수
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    public static bool Prime(int limit)
    {
        if (limit <= 1) return false;
        if (limit == 2 || limit == 3) return true;


        double sqrt = Math.Sqrt(limit);

        for (int i = 2; i <= (int)sqrt; i++)
        {

            if (limit % i == 0) return false; 
        }
        return true;
    }

    /// <summary>
    /// 특정 숫자 배열로 만들 수 있는 모든 숫자 순열 생성 함수(매개 데이터 내 숫자간 중복 허용)
    /// </summary>
    /// <param name="list">특정 숫자들이 든 배열</param>
    /// <param name="result">결과 배열</param>
    /// <param name="visited">백트래킹중 이미 순회한 리스트 체크</param>
    /// <param name="current">백트래킹중 사용되는 리스트</param>
    /// <param name="count"></param>
    static void GetPermutations(List<string> list, List<string> result,
    bool[] visited, List<string> current, int count)
    {
        if (current.Count == count)
        {
            result.Add(string.Concat(current));
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (visited[i]) continue;

            visited[i] = true;
            current.Add(list[i]);

            GetPermutations(list, result, visited, current, count);

            current.RemoveAt(current.Count - 1);
            visited[i] = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="startR"></param>
    /// <param name="startC"></param>
    /// <param name="endR"></param>
    /// <param name="endC"></param>
    /// <returns></returns>
    public static int BFSPath(int[,] map, int startR, int startC, int endR, int endC)
    {


        // 맵의 크기
        int mapRows = map.GetLength(0);
        int mapCols = map.GetLength(1);

        int[,] dist = new int[mapRows, mapCols];

        // 거리 배열 초기화(아직 미방문시 -1이기 때문에)
        for (int i = 0; i < mapRows; i++)
        {
            for (int j = 0; j < mapCols; j++)
            {
                dist[i, j] = -1;
            }
        }
        // 가까운 곳부터 탐색을 해서 넣어두면 가까운 곳부터 나오게 됨.
        Queue<Node> bfsQueue = new();
        // 큐와 distance 배열에 시작점을 넣어준다. (초기화)
        bfsQueue.Enqueue(new Node(startR, startC));
        dist[startR, startC] = 0;

        // 큐가 빌때까지.
        // 큐에서 하나를 빼서 그 위치부터 주변을 순회하고
        // 마지막에는 갱신한 내용을 넣어줄것이기때문에.
        // 큐가 비었다 == 더이상 갱신할게 없다 == 전부 순회했다.
        while (bfsQueue.Count > 0)
        {
            // 큐에서 현재 좌표를 하나 꺼낸다. 가장 먼저 들어간 노드.
            Node currentNode = bfsQueue.Dequeue();
            // 현재 위치
            int currentRow = currentNode.Row;
            int currentCol = currentNode.Col;

            // 만약 내가 지정한 도착점에 도착했다면
            if (currentRow == endR && currentCol == endC)
            {
                // 현재 위치의 거리를 반환
                return dist[currentRow, currentCol];
            }

            // 현재 위치에서 상,하,좌,우를 전부 확인한다.
            for (int i = 0; i < 4; i++)
            {
                // X(행)의 위치와 Y(열) 의위치를 상하좌우 움직이면서 판단할 수 있도록해준다.
                int moveRow = currentRow + X_DIRECTION[i];
                int moveCol = currentCol + Y_DIRECTION[i];

                // 맵의 범위를 넘었거나
                if (moveRow < 0 || moveRow >= mapRows || moveCol < 0 || moveCol >= mapCols)
                    continue;

                // 벽이거나
                if (map[moveRow, moveCol] == 1)
                    continue;

                // 이미방문했으면 continue
                if (dist[moveRow, moveCol] != -1)
                    continue;

                // 위의 내용을 전부 만족하지 않는다면, 거리를 갱신하고 큐에 추가
                dist[moveRow, moveCol] = dist[currentRow, currentCol] + 1;
                bfsQueue.Enqueue(new Node(moveRow, moveCol));
            }
        }

        // 도착점에 도달하지 못한 경우
        return -1;
    }

    public static List<Node> BFSFindPath(int[,] map, int startRow, int startCol, int endRow, int endCol)
    {
        // 목표지점이 잘못되었을 경우
        if (map[endCol, endRow] == 1) return new();

        List<Node> result = new();
        int mapRow = map.GetLength(0);
        int mapCol = map.GetLength(1);
        // map의 최단거리 저장, 확인하지 않았다면 -1
        int[,] dist = new int[mapRow, mapCol];
        Node[,] prevNodes = new Node[mapRow, mapCol];
        dist.MatrixFill(-1);
        prevNodes.MatrixFill(new(-1, -1));

        Queue<Node> visited = new();

        // 시작점 초기화 
        visited.Enqueue(new Node(startRow, startCol));
        dist[startRow, startCol] = 0;

        while (visited.Count > 0)
        {
            // 큐에서 하나 빼고 그 위치 주변 순회
            // 순회 마지막에 갱신한 위치를 넣을 것이기에, 큐가 비었다 = 전부 순회함
            Node current = visited.Dequeue();
            int currentRow = current.Row;
            int currentCol = current.Col;
            int colNum = dist[currentRow, currentCol];
            int minDist = mapRow * mapCol + 1;

            if (currentRow == endRow && currentCol == endCol) break;
            //상하좌우 서치
            for (int i = 0; i < X_DIRECTION.Length; i++)
            {
                int moveRow = -1;
                int moveCol = -1;
                if (currentRow + X_DIRECTION[i] >= 0) moveRow = currentRow + X_DIRECTION[i];
                if (currentCol + Y_DIRECTION[i] >= 0) moveCol = currentCol + Y_DIRECTION[i];

                // 서칭 규칙 확립 
                if (// 맵을 벗어난 경우
                    moveRow < 0 || moveCol < 0 ||
                    moveRow >= mapRow || moveCol >= mapCol) continue;

                // 벽인 경우 
                if (map[moveRow, moveCol] == 1) continue;


                // 이미 방문한 경우
                if (dist[moveRow, moveCol] > -1) continue;
                // 방문 확인
                dist[moveRow, moveCol] = colNum + 1;

                // 이전 경로까지 저장
                // 현재 row/col이 같은 경우 current에서 상/하/좌/우를 움직인 상태
                // 이동하려는 위치에 현재 위치를 저장하면 경로를 저장해둔 것과 같다.
                prevNodes[moveRow, moveCol] = new(currentRow, currentCol);

                // Enqueue
                visited.Enqueue(new Node(moveRow, moveCol));
            }
        }

        Node currentNode = new(endRow, endCol);

        while (currentNode.Row != -1)
        {
            // 역추적은 도착점 -> 시작점 순서로 진행
            // 사람들이 보기엔 시작점 -> 도착점이 자연스러우므로
            // 앞쪽부터 넣어서 마지막이 끝이 되도록
            result.Add(currentNode);

            // 현재 좌표에서 이전좌표로 이동시킨다.
            currentNode = prevNodes[currentNode.Row, currentNode.Col];
        }
        result.Reverse();
        return result;
    }

    public static bool DFSPath<T>(T[,] map, bool[,] isVisited, int startX, int startY, int endX, int endY)
    {
        int mapRow = map.GetLength(0);
        int mapCol = map.GetLength(1);

        if (startX == endX && startY == endY) return true;

        //초기값 세팅(초기 위치는 이미 방문함)
        // 초기화하지 않으면 무한루프 가능성 있음
        isVisited[startX, startY] = true;

        for (int i = 0; i < X_DIRECTION.Length; i++)
        {
            int moveRow = startX + X_DIRECTION[i];
            int moveCol = startY + Y_DIRECTION[i];

            // 서칭 규칙 확립 
            if (// 맵을 벗어난 경우
                moveRow < 0 || moveCol < 0 ||
                moveRow >= mapRow || moveCol >= mapCol) continue;

            /*// 벽인 경우 
            if (map[moveRow, moveCol] == ) continue;*/

            // 이미 방문한 경우
            if (isVisited[moveRow, moveCol]) continue;

            if (DFSPath(map, isVisited, moveRow, moveCol, endX, endY)) return true;
        }

        return false;
    }

    public static int[,] Dijkstra(int[,] map, int startX, int startY)
    {
        int mapRow = map.GetLength(0);
        int mapCol = map.GetLength(1);
        int[,] dist = new int[mapRow, mapCol];

        dist.MatrixFill(int.MaxValue);

        //도달 불가능함의 차이(실제 도달 불가 or 아직 검사하지 않음)를 표시해야함 
        bool[,] isVisited = new bool[mapRow, mapCol];

        dist[startX, startY] = 0;

        //반복은 모든 칸 수 - 1 만큼 반복
        int total = mapRow * mapCol;

        for (int i = 0; i < total - 1; i++)
        {
            // 다.익.알
            // 방문을 하지 않은 노드중에서 가장 비용이 적은 칸 확인
            Node current = new(-1, -1);
            int minDist = int.MaxValue;
            for (int r = 0; r < mapRow; r++)
            {
                for (int c = 0; c < mapCol; c++)
                {
                    if (map[r, c] == 0 || isVisited[r, c]) continue;

                    if (minDist > dist[r, c])
                    {
                        minDist = dist[r, c];
                        current = new Node(r, c);
                    }
                }
            }
            // 후보가 없을 경우,
            // 더이상 길을 찾을 수 없거나 이미 모든 노드의 방문을 완료했을 경우
            if (current.Row == -1 || dist[current.Row, current.Col] == int.MaxValue)
                break;

            isVisited[current.Row, current.Col] = true;

            for (int j = 0; j < X_DIRECTION.Length; j++)
            {
                int moveX = current.Row + X_DIRECTION[j];
                int moveY = current.Col + Y_DIRECTION[j];

                if (moveX < 0 || moveX >= mapRow || moveY < 0 || moveY >= mapCol)
                    continue;

                if (map[moveX, moveY] == 0)
                    continue;

                if (isVisited[moveX, moveY])
                    continue;

                // current를 거쳐 지금 가려고 하는 위치(moveX, moveY)로 가는 비용을 계산.
                int newDist = dist[current.Row, current.Col] + map[moveX, moveY];

                if (newDist < dist[moveX, moveY]) dist[moveX, moveY] = newDist;
            }
        }

        return dist;
    }

    public static string StrLengExtend(StringBuilder sb, int range, string data,  bool isLeft = false)
    {
        int byteCount = GameManager.Instance.Euckr.GetByteCount(data);
        
        if (byteCount < range)
        {
            if(isLeft)
                sb.Insert(0, ' ');
            else
                sb.Append(' ');
            isLeft = !isLeft;
            return StrLengExtend(sb, range, sb.ToString(), isLeft);
        }
        else if (byteCount > range)
        {
            for (int i = range; i >= range / 2; i--)
            {
                if (data.Length < i) continue;

                string ck = data[..i];
                if (GameManager.Instance.Euckr.GetByteCount(ck) <= range)
                {
                    sb.Clear();
                    sb.Append(ck);
                    return StrLengExtend(sb, range, ck, isLeft);
                }
            }
            // 이거 일어나면 안됨 인코딩상 2바이트 초과하는 글자가 없어서 불가능함;
            return "버그남;;";
        }
        else return sb.ToString();
    }

    public static bool CoinToss(int sanity)
    {
        int value = Random.Shared.Next(0, 100);

        return value < (sanity + 50);
    }
}

