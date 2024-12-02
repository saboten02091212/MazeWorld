using System;
using System.Collections;
using System.Collections.Generic;
using Maze;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CreateMaze : MonoBehaviour
{
    private const int ScaleCoef = 3;

    public int height;
    public int width;
    public int depth;

    [SerializeField] private Transform root;

    // Start is called before the first frame update
    void Start()
    {
        Create();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Create()
    {
        Clear();

        var creater = new MazeCreater(height, width, depth, 0, height - 2);
        var maze = creater.Create();

        var wall = (GameObject)Resources.Load("Wall");
        var floor = (GameObject)Resources.Load("Floor");
        var upStair = (GameObject)Resources.Load("UpStair");
        var downStair = (GameObject)Resources.Load("DownStair");

        for (int x = 0; x < maze.Height; x++)
        {
            // �e�}�X���Ƀ��[�v���ĒʘH�E�V��E�K�i���쐬
            for (int y = 1; y < maze.Width - 1; y++)
            {
                for (int z = 1; z < maze.Depth - 1; z++)
                {
                    // �ʘH���ǂ�������
                    if (MazeData.Way == maze.Map[x, y, z])
                    {
                        // �X�^�[�g�E�S�[�������̈ȊO�̏ꍇ�A�V����쐬����
                        if ((false == IsRoom(maze.StartPosition, x, y, z)) && (false == IsRoom(maze.GorlPosition, x, y, z)))
                        {
                            // �V������
                            Instantiate(
                                floor,
                                new Vector3(
                                    (y - 0.5f) * ScaleCoef,
                                    (x + 1) * ScaleCoef,
                                    (z - 0.5f) * ScaleCoef),
                                Quaternion.identity,
                                root);
                        }

                        // ���̊K���ʘH�łȂ���΁A�ʘH�����
                        if ((x == 0) || (MazeData.Way != maze.Map[x - 1, y, z]))
                        {
                            Instantiate(
                                floor,
                                new Vector3(
                                    (y - 0.5f) * ScaleCoef,
                                    x * ScaleCoef,
                                    (z - 0.5f) * ScaleCoef),
                                Quaternion.identity,
                                root);
                        }
                    }

                    // ���K�i���ǂ�������
                    if (MazeData.UpStair == (MazeData.UpStair & maze.Map[x, y, z]))
                    {
                        if (MazeData.North == (MazeData.North & maze.Map[x, y, z]))
                        {
                            Instantiate(
                                upStair,
                                new Vector3(
                                    (y - 0.5f) * ScaleCoef,
                                    (x + 0.5f) * ScaleCoef,
                                    (z - 0.5f) * ScaleCoef),
                                Quaternion.Euler(180, 0, 0),
                                root);
                        }
                        else if (MazeData.South == (MazeData.South & maze.Map[x, y, z]))
                        {
                            Instantiate(
                                upStair,
                                new Vector3(
                                    (y - 0.5f) * ScaleCoef,
                                    (x + 0.5f) * ScaleCoef,
                                    (z - 0.5f) * ScaleCoef),
                                Quaternion.Euler(180, 180, 0),
                                root);
                        }
                        else if (MazeData.East == (MazeData.East & maze.Map[x, y, z]))
                        {
                            Instantiate(
                                upStair,
                                new Vector3(
                                    (y - 0.5f) * ScaleCoef,
                                    (x + 0.5f) * ScaleCoef,
                                    (z - 0.5f) * ScaleCoef),
                                Quaternion.Euler(180, 90, 0),
                                root);
                        }
                        else if (MazeData.West == (MazeData.West & maze.Map[x, y, z]))
                        {
                            Instantiate(
                                upStair,
                                new Vector3(
                                    (y - 0.5f) * ScaleCoef,
                                    (x + 0.5f) * ScaleCoef,
                                    (z - 0.5f) * ScaleCoef),
                                Quaternion.Euler(180, 270, 0),
                                root);
                        }
                    }

                    // ����K�i���ǂ�������
                    if (MazeData.DownStair == (MazeData.DownStair & maze.Map[x, y, z]))
                    {
                        if (MazeData.North == (MazeData.North & maze.Map[x, y, z]))
                        {
                            Instantiate(
                                downStair,
                                new Vector3(
                                    (y - 0.5f) * ScaleCoef,
                                    (x + 0.5f) * ScaleCoef,
                                    (z - 0.5f) * ScaleCoef),
                                Quaternion.Euler(0, 180, 0),
                                root);
                        }
                        else if (MazeData.South == (MazeData.South & maze.Map[x, y, z]))
                        {
                            Instantiate(
                                downStair,
                                new Vector3(
                                    (y - 0.5f) * ScaleCoef,
                                    (x + 0.5f) * ScaleCoef,
                                    (z - 0.5f) * ScaleCoef),
                                Quaternion.Euler(0, 0, 0),
                                root);
                        }
                        else if (MazeData.East == (MazeData.East & maze.Map[x, y, z]))
                        {
                            Instantiate(
                                downStair,
                                new Vector3(
                                    (y - 0.5f) * ScaleCoef,
                                    (x + 0.5f) * ScaleCoef,
                                    (z - 0.5f) * ScaleCoef),
                                Quaternion.Euler(0, 270, 0),
                                root);
                        }
                        else if (MazeData.West == (MazeData.West & maze.Map[x, y, z]))
                        {
                            Instantiate(
                                downStair,
                                new Vector3(
                                    (y - 0.5f) * ScaleCoef,
                                    (x + 0.5f) * ScaleCoef,
                                    (z - 0.5f) * ScaleCoef),
                                Quaternion.Euler(0, 90, 0),
                                root);
                        }
                    }
                }

            }

            // ���s��1�}�X�����[�v���č��E�̕ǂ𐶐�
            for (int y = 1; y < maze.Width - 1; y += 2)
            {
                for (int z = 1; z < maze.Depth - 1; z++)
                {
                    // �ǂ��ǂ�������
                    if (MazeData.Wall == maze.Map[x, y, z])
                    {
                        continue;
                    }

                    // �����̕ǂ����邩����
                    if (MazeData.Wall == maze.Map[x, y - 1, z])
                    {
                        Instantiate(
                            wall,
                            new Vector3(
                                (y - 1) * ScaleCoef,
                                (x + 0.5f) * ScaleCoef,
                                (z - 0.5f) * ScaleCoef),
                            Quaternion.Euler(0, 90, 0),
                            root);
                    }

                    // �E���̕ǂ����邩����
                    if (MazeData.Wall == maze.Map[x, y + 1, z])
                    {
                        Instantiate(
                            wall,
                            new Vector3(
                                y * ScaleCoef,
                                (x + 0.5f) * ScaleCoef,
                                (z - 0.5f) * ScaleCoef),
                            Quaternion.Euler(0, 90, 0),
                            root);
                    }
                }
            }

            // ���ʒu��1�}�X�����[�v���č��E�̕ǂ𐶐�
            for (int z = 1; z < maze.Depth - 1; z += 2)
            {
                for (int y = 1; y < maze.Width - 1; y++)
                {
                    // �ǂ��ǂ�������
                    if (MazeData.Wall == maze.Map[x, y, z])
                    {
                        continue;
                    }

                    // �����̕ǂ����邩����
                    if (MazeData.Wall == maze.Map[x, y, z - 1])
                    {
                        Instantiate(
                            wall,
                            new Vector3(
                                (y - 0.5f) * ScaleCoef,
                                (x + 0.5f) * ScaleCoef,
                                (z - 1) * ScaleCoef),
                            Quaternion.identity,
                            root);
                    }

                    // ��O���̕ǂ����邩����
                    if (MazeData.Wall == maze.Map[x, y, z + 1])
                    {
                        Instantiate(
                            wall,
                            new Vector3(
                                (y - 0.5f) * ScaleCoef,
                                (x + 0.5f) * ScaleCoef,
                                z * ScaleCoef),
                            Quaternion.identity,
                            root);
                    }
                }
            }
        }

        var vrcWorld = GameObject.Find("VRCWorld");
        vrcWorld.transform.position = new Vector3(
            (maze.StartPosition.Item2 - 0.5f) * ScaleCoef,
            maze.StartPosition.Item1 * ScaleCoef + 0.1f,
            (maze.StartPosition.Item3 - 0.5f) * ScaleCoef);

        return;
    }

    public void Clear()
    {
        List<GameObject> tempList = new List<GameObject>();
        foreach (Transform child in root)
        {
            tempList.Add(child.gameObject);
        }
        for (int i = 0; i < tempList.Count; i++)
        {
            DestroyImmediate(tempList[i]);
        }

        return;
    }

    private bool IsRoom((int, int, int) position, int x, int y, int z)
    {
        if ((x == position.Item1) &&
            (y >= position.Item2 - 2) &&
            (y <= position.Item2 + 2) &&
            (z >= position.Item3 - 2) &&
            (z <= position.Item3 + 2))
        {
            return true;
        }

        return false;
    }
}
