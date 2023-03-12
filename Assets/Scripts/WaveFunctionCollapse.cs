using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaveFunctionCollapse : MonoBehaviour
{
    //public float Speed = 0.1f;

    private Tilemap baseMap, input, output;
    private List<AdjacentRule> adjacentRules;
    private List<TileBase> allTiles = new List<TileBase>() { null };
    private List<TileBase>[,] board;

    public void Generate(Tilemap baseMap, Tilemap input, Tilemap output)
    {
        this.baseMap = baseMap;
        this.input = input;
        this.output = output;

        InitializeWFC();
        StartWFC();
    }

    private void InitializeWFC()
    {
        var bounds = output.cellBounds;
        adjacentRules = new List<AdjacentRule>()
        {
            new AdjacentRule(null, null, Direction.Left),
            new AdjacentRule(null, null, Direction.Up),
            new AdjacentRule(null, null, Direction.Right),
            new AdjacentRule(null, null, Direction.Down)
        };
        board = new List<TileBase>[bounds.size.y, bounds.size.x];

        foreach (var pos in baseMap.cellBounds.allPositionsWithin)
        {
            var tile = baseMap.GetTile(pos);

            if (tile != null)
            {
                if (!allTiles.Contains(tile))
                    allTiles.Add(tile);

                //GetTile()�� Ÿ�ϸ� �� ��ġ�� ������ null�� ������ ������ ���� ��Ģ�� ������
                //���� Ÿ�ϸ� ���ʿ� �ִ� Ÿ�ϵ���� ��Ģ �����ϵ��� ����
                if (pos.x > baseMap.cellBounds.x)
                {
                    //�پ��� ������ ���� �ݴ� ��Ģ�� �߰�
                    var left = baseMap.GetTile(new Vector3Int(pos.x - 1, pos.y, pos.z));
                    adjacentRules.Add(new AdjacentRule(tile, left, Direction.Left));
                    adjacentRules.Add(new AdjacentRule(left, tile, Direction.Right));
                }

                if (pos.x < baseMap.cellBounds.xMax)
                {
                    var right = baseMap.GetTile(new Vector3Int(pos.x + 1, pos.y, pos.z));
                    adjacentRules.Add(new AdjacentRule(tile, right, Direction.Right));
                    adjacentRules.Add(new AdjacentRule(right, tile, Direction.Left));
                }

                if (pos.y < baseMap.cellBounds.yMax)
                {
                    var up = baseMap.GetTile(new Vector3Int(pos.x, pos.y + 1, pos.z));
                    adjacentRules.Add(new AdjacentRule(tile, up, Direction.Up));
                    adjacentRules.Add(new AdjacentRule(up, tile, Direction.Down));
                }

                if (pos.y > baseMap.cellBounds.y)
                {
                    var down = baseMap.GetTile(new Vector3Int(pos.x, pos.y - 1, pos.z));
                    adjacentRules.Add(new AdjacentRule(tile, down, Direction.Down));
                    adjacentRules.Add(new AdjacentRule(down, tile, Direction.Up));
                }
            }
        }

        adjacentRules = adjacentRules.Distinct().ToList();
    }

    private void StartWFC()
    {
        var bounds = output.cellBounds;
        int attempts = 0;

        while (true)
        {
            for (int y = 0; y < board.GetLength(0); y++)
            {
                for (int x = 0; x < board.GetLength(1); x++)
                {
                    //2���� �迭�� ����Ƽ ��ǥ���� y�� �����ϴ� ������ �ٸ��� ������ ���� �ʿ�
                    output.SetTile(new Vector3Int(bounds.x + x, bounds.y + board.GetLength(0) - y - 1), null);
                    board[y, x] = new List<TileBase>(allTiles);
                }
            }

            for (int y = 0; y < board.GetLength(0); y++)
            {
                for (int x = 0; x < board.GetLength(1); x++)
                {
                    var position = new Vector3Int(bounds.x + x, bounds.y + board.GetLength(0) - y - 1);

                    //�Է��� ������ �� �� Ÿ�ϸʿ� ����
                    if (input == null)
                    {
                        //�����ڸ� Ÿ������ üũ�� null�� ������ �� �ִ� Ÿ�ϸ� ����
                        //���� �� �� �ִ� Ÿ�ϸ� �ɷ����� ��
                        //Ÿ�ϸ� �׵θ��� null�� �� ���� �ѷ��׿��ٰ� �����ص� ��
                        if (y == 0)
                        {
                            var rules = adjacentRules.Where(r => r.To == null && r.Direction == Direction.Up).Select(x => x.From).ToList();
                            board[y, x].RemoveAll(x => !rules.Contains(x));
                        }

                        if (y == board.GetLength(0) - 1)
                        {
                            var rules = adjacentRules.Where(r => r.To == null && r.Direction == Direction.Down).Select(x => x.From).ToList();
                            board[y, x].RemoveAll(x => !rules.Contains(x));
                        }

                        if (x == 0)
                        {
                            var rules = adjacentRules.Where(r => r.To == null && r.Direction == Direction.Left).Select(x => x.From).ToList();
                            board[y, x].RemoveAll(x => !rules.Contains(x));
                        }

                        if (x == board.GetLength(1) - 1)
                        {
                            var rules = adjacentRules.Where(r => r.To == null && r.Direction == Direction.Right).Select(x => x.From).ToList();
                            board[y, x].RemoveAll(x => !rules.Contains(x));
                        }
                    }
                    else if (input != null && input.GetTile(position) != null) //�Է��� ������ �Է¿� ���� Ÿ�� �ر���Ű�� ����
                    {
                        var inputTile = input.GetTile(position);
                        board[y, x].RemoveAll(t => t != inputTile);

                        if (x > 0)
                            Propagate(x - 1, y, Direction.Left);

                        if (x < board.GetLength(1) - 1)
                            Propagate(x + 1, y, Direction.Right);

                        if (y > 0)
                            Propagate(x, y - 1, Direction.Up);

                        if (y < board.GetLength(0) - 1)
                            Propagate(x, y + 1, Direction.Down);

                        output.SetTile(position, inputTile);
                    }
                }
            }

            //��Ʈ��ŷ�� ���� �� ���� ������ ����
            var cache = new Stack<Observation>();

            while (board.Cast<List<TileBase>>().Any(x => x.Count > 1))
            {
                int minEntropy = board.Cast<List<TileBase>>().Where(x => x.Count > 1).Select(x => x.Count).Min();
                int randomIndex = Random.Range(0, board.Cast<List<TileBase>>().Count(x => x.Count == minEntropy));
                int index = 0;

                for (int y = 0; y < board.GetLength(0); y++)
                {
                    for (int x = 0; x < board.GetLength(1); x++)
                    {
                        //�ּ� ��Ʈ���� ���� ���� ������ Ÿ�� ����
                        if (board[y, x].Count == minEntropy && index++ == randomIndex)
                        {
                            var targetTile = board[y, x][Random.Range(0, board[y, x].Count)];
                            var cloned = new List<TileBase>[board.GetLength(0), board.GetLength(1)];

                            //board�� 2���� �迭�� ���� ���� �����̶� ���� for������ ���� ����
                            for (int i = 0; i < board.GetLength(0); i++)
                                for (int j = 0; j < board.GetLength(1); j++)
                                    cloned[i, j] = new List<TileBase>(board[i, j]);

                            cache.Push(new Observation(cloned, targetTile, x, y));
                            board[y, x].RemoveAll(t => t != targetTile);

                            bool isSuccessful = true;
                            //���� �� 4�������� ����
                            if (x > 0)
                                isSuccessful &= Propagate(x - 1, y, Direction.Left);

                            if (x < board.GetLength(1) - 1)
                                isSuccessful &= Propagate(x + 1, y, Direction.Right);

                            if (y > 0)
                                isSuccessful &= Propagate(x, y - 1, Direction.Up);

                            if (y < board.GetLength(0) - 1)
                                isSuccessful &= Propagate(x, y + 1, Direction.Down);

                            if (!isSuccessful)
                            {
                                Observation observation = null;

                                //���Ŀ��� ������ �߻��ϸ� ��Ʈ��ŷ
                                //���� ���� ���·� �ǵ����� ������ �����ߴ� Ÿ���� ���ɼ����� ����
                                //���� �ش� ó���� Empty Domain�� �Ǹ� �ٽ� ��Ʈ��ŷ
                                do
                                {
                                    Debug.Log($"Popping cache ({cache.Count})");
                                    observation = cache.Pop();

                                    for (int i = 0; i < board.GetLength(0); i++)
                                        for (int j = 0; j < board.GetLength(1); j++)
                                            board[i, j] = new List<TileBase>(observation.Board[i, j]);

                                    board[observation.Y, observation.X].Remove(observation.Tile);
                                } while (cache.Count != 0 && board[observation.Y, observation.X].Count == 0);
                            }
                            else
                                output.SetTile(new Vector3Int(bounds.x + x, bounds.y + board.GetLength(0) - y - 1), targetTile);

                            break;
                        }
                    }
                }
            }

            //��Ʈ���ǰ� 0�� Ÿ���� ������ �ٽ� ����
            if (board.Cast<List<TileBase>>().Any(x => x.Count == 0))
            {
                Debug.LogError($"Empty domain found! (Attempts: {++attempts})");
                continue;
            }

            //���� ��Ģ�� �����ϴ� Ÿ���� �ִٸ� ������ϱ� ���� �˻�
            bool notContradicted = true;
            for (int y = 0; y < board.GetLength(0); y++)
            {
                for (int x = 0; x < board.GetLength(1); x++)
                {
                    if (x > 0)
                        notContradicted &= adjacentRules
                            .Any(r => r.Direction == Direction.Left && r.From == board[y, x][0] && r.To == board[y, x - 1][0]);

                    if (x < board.GetLength(1) - 1)
                        notContradicted &= adjacentRules
                            .Any(r => r.Direction == Direction.Right && r.From == board[y, x][0] && r.To == board[y, x + 1][0]);

                    if (y > 0)
                        notContradicted &= adjacentRules
                            .Any(r => r.Direction == Direction.Up && r.From == board[y, x][0] && r.To == board[y - 1, x][0]);

                    if (y < board.GetLength(0) - 1)
                        notContradicted &= adjacentRules
                            .Any(r => r.Direction == Direction.Down && r.From == board[y, x][0] && r.To == board[y + 1, x][0]);
                }
            }

            if (!notContradicted)
            {
                Debug.LogError($"Contradiction found! (Attempts: {++attempts})");
                continue;
            }
            else
            {
                //��Ʈ��ŷ �� ������Ʈ���� ���� Ÿ���� ���� �� �־ �ٽ� �׷��ֱ�
                for (int y = 0; y < board.GetLength(0); y++)
                    for (int x = 0; x < board.GetLength(1); x++)
                        output.SetTile(new Vector3Int(bounds.x + x, bounds.y + board.GetLength(0) - y - 1), board[y, x][0]);

                break;
            }
        }
    }

    private bool Propagate(int x, int y, Direction previousDirection)
    {
        //��Ʈ���ǰ� 1�̸� �̹� �ر��� Ÿ���̹Ƿ� ������ �ʿ䰡 ����
        if (board[y, x].Count == 1)
            return true;

        var previous = board[y, x];
        var bounds = output.cellBounds;

        switch (previousDirection)
        {
            case Direction.Left:
                previous = board[y, x + 1];
                break;

            case Direction.Right:
                previous = board[y, x - 1];
                break;

            case Direction.Up:
                previous = board[y + 1, x];
                break;

            case Direction.Down:
                previous = board[y - 1, x];
                break;
        }

        //���� Ÿ�ϰ� ������ �� �ִ� Ÿ�ϸ� ����
        var count = board[y, x].Count;
        var tos = adjacentRules.Where(x => x.Direction == previousDirection && previous.Contains(x.From)).Select(x => x.To).ToList();
        board[y, x].RemoveAll(t => !tos.Contains(t));

        if (board[y, x].Count == 0) //������ ������ 0�� �Ǹ� �� ��, ��Ʈ��ŷ�ϱ� ���� false ��ȯ
            return false;
        else if (board[y, x].Count == count)
            return true;

        //���� �ܰ迡�� Empty Domain �߻��ϸ� ��Ʈ��ŷ�ϱ� ���� ���� ���
        bool isSuccessful = true;

        //������ �����ؿ� ������ ������ 3�������� �ٽ� ����
        //���� �� �����ϸ� ���� �ݺ��Ǿ StackOverflowException �߻�
        if (x > 0 && previousDirection != Direction.Right)
            isSuccessful &= Propagate(x - 1, y, Direction.Left);

        if (x < board.GetLength(1) - 1 && previousDirection != Direction.Left)
            isSuccessful &= Propagate(x + 1, y, Direction.Right);

        if (y > 0 && previousDirection != Direction.Down)
            isSuccessful &= Propagate(x, y - 1, Direction.Up);

        if (y < board.GetLength(0) - 1 && previousDirection != Direction.Up)
            isSuccessful &= Propagate(x, y + 1, Direction.Down);

        //���� �ر��ƴٸ� ���� ��Ģ�� ���������� �ʴ��� �˻�
        if (board[y, x].Count == 1)
        {
            bool notContradicted = true;

            if (x > 0 && board[y, x - 1].Count != 0)
                notContradicted &= adjacentRules
                    .Any(r => r.Direction == Direction.Left && r.From == board[y, x][0] && r.To == board[y, x - 1][0]);

            if (x < board.GetLength(1) - 1 && board[y, x + 1].Count != 0)
                notContradicted &= adjacentRules
                    .Any(r => r.Direction == Direction.Right && r.From == board[y, x][0] && r.To == board[y, x + 1][0]);

            if (y > 0  && board[y - 1, x].Count != 0)
                notContradicted &= adjacentRules
                    .Any(r => r.Direction == Direction.Up && r.From == board[y, x][0] && r.To == board[y - 1, x][0]);

            if (y < board.GetLength(0) - 1  && board[y + 1, x].Count != 0)
                notContradicted &= adjacentRules
                    .Any(r => r.Direction == Direction.Down && r.From == board[y, x][0] && r.To == board[y + 1, x][0]);

            if (!notContradicted)
            {
                Debug.Log("Found contradiction during propagation");
                isSuccessful = false;
            }
        }

        if (isSuccessful)
            output.SetTile(new Vector3Int(bounds.x + x, bounds.y + board.GetLength(0) - y - 1), board[y, x][0]);

        return isSuccessful;
    }

    //private void OnDrawGizmos()
    //{
    //    var bounds = output.cellBounds;

    //    if (board != null)
    //    {
    //        for (int y = 0; y < board.GetLength(0); y++)
    //        {
    //            for (int x = 0; x < board.GetLength(1); x++)
    //            {
    //                if (board[y, x] != null)
    //                {
    //                    var pos = output.CellToWorld(new Vector3Int(bounds.x + x, bounds.y + board.GetLength(0) - y - 1));
    //                    pos += new Vector3(0, 0.5f);

    //                    var style = new GUIStyle();
    //                    int count = board[y, x].Count;

    //                    if (count > 10)
    //                        style.normal.textColor = new Color32(252, 40, 3, 255);
    //                    else if (count > 6)
    //                        style.normal.textColor = new Color32(252, 186, 3, 255);
    //                    else if (count > 3)
    //                        style.normal.textColor = new Color32(188, 235, 35, 255);
    //                    else
    //                        style.normal.textColor = new Color32(65, 240, 71, 255);

    //                    Handles.Label(pos, board[y, x].Count.ToString(), style);
    //                }
    //            }
    //        }
    //    }
    //}
}