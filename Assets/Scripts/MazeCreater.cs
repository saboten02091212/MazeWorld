using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    /// <summary>
    /// Mazeを自動生成するためのクラス
    /// </summary>
    public class MazeCreater
    {
        /// <summary>
        /// 方角
        /// </summary>
        readonly int[] Ways = { MazeData.North, MazeData.South, MazeData.East, MazeData.West };

        /// <summary>
        /// 横幅
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// 高さ
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// 奥行
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// スタート階数
        /// </summary>
        public int? StartHeight { get; private set; }
        /// <summary>
        /// ゴール階数
        /// </summary>
        public int? GorlHeight { get; private set; }

        /// <summary>
        /// 階段の生成確率(256分率)
        /// </summary>
        public int StairP { get; private set; }

        /// <summary>
        /// 乱数
        /// </summary>
        private Random Rnd { get; set; }


        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="x">高さ</param>
        /// <param name="y">横幅</param>
        /// <param name="z">奥行</param>
        /// <param name="startHeight">スタート階数</param>
        /// <param name="gorlHeight">ゴール階数</param>
        /// <param name="stairP">階段の生成確率(256分率)</param>
        public MazeCreater(int x, int y, int z, int? startHeight = null, int? gorlHeight = null, int stairP = 16)
        {
            this.Height = x;
            this.Width = y;
            this.Depth = z;
            this.StartHeight = startHeight;
            this.GorlHeight = gorlHeight;
            this.Rnd = new Random();
        }

        /// <summary>
        /// Mazeを自動生成します。
        /// </summary>
        /// <returns>自動生成されたMaze</returns>
        public MazeData Create()
        {
            // Mazeインスタンス生成
            var maze = new MazeData(this.Height, this.Width, this.Depth);

            // スタート・ゴール位置の決定
            var startPosition = this.InitEdgePosition(x: this.StartHeight);
            var gorlPosition = this.InitEdgePosition(x: this.Height - 2);
            maze.StartPosition = (startPosition.Item1, startPosition.Item2 * 2 + 1, startPosition.Item3 * 2 + 1);
            maze.GorlPosition = (gorlPosition.Item1, gorlPosition.Item2 * 2 + 1, gorlPosition.Item3 * 2 + 1);

            // スタート・ゴールの部屋作成
            var map = this.InitMap();
            this.SetEdgeRoom(map, startPosition);
            this.SetEdgeRoom(map, gorlPosition);

            // スタート・ゴールの出入口設定
            var startEntrance = this.SetEdgeEntrance(map, startPosition);
            var gorlEntrance = this.SetEdgeEntrance(map, gorlPosition);

            // ゴール地点に到達できるよう、壁を設定
            MazeUtility.SetMapValue(map, gorlEntrance, MazeData.Wall);

            // 迷路の自動生成
            this.MazeDig(map, startEntrance, gorlEntrance);

            // 迷路情報をコピー
            for (int x = 1; x < map.GetLength(0) - 1; x++)
            {
                for (int y = 1; y < map.GetLength(1) - 1; y++)
                {
                    for (int z = 1; z < map.GetLength(2) - 1; z++)
                    {
                        maze.Map[x - 1, y - 1, z -1] = map[x, y, z];
                    }
                }
            }

            return maze;
        }

        /// <summary>
        /// スタートまたはゴールとなりえる位置をランダムに生成します。
        /// </summary>
        /// <param name="x">指定した値で高さを決定します</param>
        /// <param name="y">指定した値で横位置を決定します</param>
        /// <param name="z">指定した値で奥行位置を決定します</param>
        /// <returns>スタートまたはゴールとなりえる位置</returns>
        private (int, int, int) InitEdgePosition(int? x = null, int? y = null, int? z = null)
        {
            int retX = x ?? this.Rnd.Next(0, this.Height - 2);
            int retY = y ?? this.Rnd.Next(1, this.Width - 2);
            int retZ = z ?? this.Rnd.Next(1, this.Depth - 2);

            return (retX, retY, retZ);
        }

        /// <summary>
        /// mapを初期化します
        /// </summary>
        /// <returns>初期化されたmap</returns>
        private int[,,] InitMap()
        {
            var map = new int[this.Height + 2, this.Width * 2 + 3, this.Depth * 2 + 3];

            // 高さでループ
            for (int x = 0; x < map.GetLength(0); x++)
            {
                // 横位置ループ
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    // 奥行ループ
                    for (int z = 0; z < map.GetLength(2); z++)
                    {
                        // 外周は通路(番兵)に設定、それ以外は壁で埋める
                        map[x, y, z] =
                            ((x == 0) || (y == 0) || (z == 0) || (x == map.GetLength(0) - 1) || (y == map.GetLength(1) - 1) || (z == map.GetLength(2) - 1)) ?
                            MazeData.Way :
                            MazeData.Wall;
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// スタートまたはゴールの部屋(2×5×5)をmap上に作成
        /// </summary>
        /// <param name="map">設定対象のmap</param>
        /// <param name="edgePosition">スタートまたはゴールの位置</param>
        private void SetEdgeRoom(int[,,] map, (int, int, int) edgePosition)
        {
            // 位置をmap上の位置に置き換え
            var mapPosition = this.ConvertPosition(edgePosition);

            // 高さループ (0 ～ 1)
            for (int x = mapPosition.Item1; x <= mapPosition.Item1 + 1; x++)
            {
                // 横位置ループ (-2 ～ 2)
                for (int y = mapPosition.Item2 - 2; y <= mapPosition.Item2 + 2; y++)
                {
                    // 奥行ループ (-2 ～ 2)
                    for (int z = mapPosition.Item3 - 2; z <= mapPosition.Item3 + 2; z++)
                    {
                        map[x, y, z] = MazeData.Way;
                    }
                }
            }

            return;
        }

        /// <summary>
        /// スタートまたはゴールの部屋の出入口をmap上に作成
        /// </summary>
        /// <param name="map">設定対象のmap</param>
        /// <param name="edgePosition">スタートまたはゴールの位置</param>
        /// <returns>出入口位置</returns>
        private (int, int, int) SetEdgeEntrance(int[,,] map, (int, int, int) edgePosition)
        {
            // 位置をmap上の位置に置き換え
            var mapPosition = this.ConvertPosition(edgePosition);

            // 出入口の位置を設定
            int way = 0;
            var entrancePosition = edgePosition;

            // ランダムな方角に4歩移動
            int rnd = this.Rnd.Next(this.Ways.Length - 1);
            for (int i = 0; i < 4; i++)
            {
                way = this.Ways[(rnd + i) % this.Ways.Length];
                entrancePosition = this.Move(mapPosition, way, step: 4);

                // 壁の場合、出入口を決定（通路の場合はやり直し）
                if (MazeData.Wall == MazeUtility.GetMapValue(map, entrancePosition))
                {
                    break;
                }
            }

            // 出入口に向かって穴掘り
            var movePosition = this.Move(mapPosition, way, step: 2);
            this.Dig(map, movePosition, way);

            return entrancePosition;
        }

        /// <summary>
        /// 迷路を穴掘り法で自動生成します。
        /// </summary>
        /// <param name="map">設定対象map</param>
        /// <param name="startEntrance">スタート位置</param>
        /// <param name="gorlEntrance">ゴール位置</param>
        private void MazeDig(int[,,] map, (int, int, int) startEntrance, (int, int, int) gorlEntrance)
        {
            // 階段に優先的に生成するか判定
            bool priorStair = this.PriorStair();
            int[] stairs;
            if (true == priorStair)
            {
                stairs = (0 == this.Rnd.Next(1)) ?
                    new int[] { 1, -1, 0 } :
                    new int[] { -1, 1, 0 };
            }
            else
            {
                stairs = (0 == this.Rnd.Next(1)) ?
                    new int[] { 0, 1, -1 } :
                    new int[] { 0, -1, 1 };
            }

            // 穴掘り
            foreach (var stair in stairs)
            {
                int rnd = this.Rnd.Next(this.Ways.Length - 1);
                for (int i = 0; i < 4; i++)
                {
                    // 掘れるか判定
                    int way = this.Ways[(rnd + i) % this.Ways.Length];
                    if (false == this.CanDig(map, startEntrance, way, stair))
                    {
                        continue;
                    }

                    // 穴掘り処理
                    switch (stair)
                    {
                        case 0:
                            this.Dig(map, startEntrance, way);
                            break;

                        case 1:
                            this.UpDig(map, startEntrance, way);
                            break;

                        case -1:
                            this.DownDig(map, startEntrance, way);
                            break;
                    }

                    // ゴール位置か判定
                    var movePosition = this.Move(startEntrance, way, stair, 2);
                    if (false == movePosition.Equals(gorlEntrance))
                    {
                        // ゴールでなければ、穴掘りを進める
                        this.MazeDig(map, movePosition, gorlEntrance);
                    }
                }
            }

            return;
        }

        /// <summary>
        /// 指定された方角に掘れるかどうか判定します。
        /// </summary>
        /// <param name="map">判定対象map</param>
        /// <param name="position">位置</param>
        /// <param name="way">方角</param>
        /// <param name="stair">階層</param>
        /// <returns>掘れる場合、true。掘れない場合、falseを返します</returns>
        private bool CanDig(int[,,] map, (int, int, int) position, int way, int stair)
        {
            // 移動先が壁か判定
            (int, int, int) movePostion = this.Move(position, way, stair, 2);
            if (MazeData.Wall != MazeUtility.GetMapValue(map, movePostion))
            {
                return false;
            }

            // 同一改装の場合は、移動途中も壁か(階段がないか)判定
            if (0 == stair)
            {
                movePostion = this.Move(position, way, stair, 1);
                if (MazeData.Wall != MazeUtility.GetMapValue(map, movePostion))
                {
                    return false;
                }
            }
            // 階段の場合は、階段が作れるか判定
            else
            {
                // 元階層の階段位置が壁か判定
                movePostion = this.Move(position, way, 0);
                if (MazeData.Wall != MazeUtility.GetMapValue(map, movePostion))
                {
                    return false;
                }

                // 先階層の階段位置が壁か判定
                movePostion = this.Move(position, way, stair);
                if (MazeData.Wall != MazeUtility.GetMapValue(map, movePostion))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 指定された方角に穴掘りでmapを変更します。
        /// </summary>
        /// <param name="map">設定対象map</param>
        /// <param name="mapPosition">元位置</param>
        /// <param name="way">方角</param>
        private void Dig(int[,,] map, (int, int, int) position, int way)
        {
            // 2歩通路に変更する
            (int, int, int) movePostion = this.Move(position, way);
            MazeUtility.SetMapValue(map, movePostion, MazeData.Way);

            movePostion = this.Move(movePostion, way);
            MazeUtility.SetMapValue(map, movePostion, MazeData.Way);

            return;
        }

        /// <summary>
        /// 指定された方角に上り階段でmapを変更します。
        /// </summary>
        /// <param name="map">設定対象map</param>
        /// <param name="mapPosition">元位置</param>
        /// <param name="way">方角</param>
        /// <exception cref="NotImplementedException"></exception>
        private void UpDig(int[,,] map, (int, int, int) position, int way)
        {
            // 1歩先は、上り階段
            (int, int, int) movePostion = this.Move(position, way);
            MazeUtility.SetMapValue(map, movePostion, MazeData.UpStair + way);

            // 上り階段の上は下り階段
            movePostion = this.Move(position, way, 1);
            MazeUtility.SetMapValue(map, movePostion, MazeData.DownStair + this.Turn(way));

            // 下り階段の先は通路
            movePostion = this.Move(movePostion, way);
            MazeUtility.SetMapValue(map, movePostion, MazeData.Way);

            return;
        }

        /// <summary>
        /// 指定された方角に下り階段でmapを変更します。
        /// </summary>
        /// <param name="map">設定対象map</param>
        /// <param name="mapPosition">元位置</param>
        /// <param name="way">方角</param>
        /// <exception cref="NotImplementedException"></exception>
        private void DownDig(int[,,] map, (int, int, int) position, int way)
        {
            // 1歩先は、下り階段
            (int, int, int) movePostion = this.Move(position, way);
            MazeUtility.SetMapValue(map, movePostion, MazeData.DownStair + way);

            // 下り階段の下は上り階段
            movePostion = this.Move(position, way, -1);
            MazeUtility.SetMapValue(map, movePostion, MazeData.UpStair + this.Turn(way));

            // 上り階段の先は通路
            movePostion = this.Move(movePostion, way);
            MazeUtility.SetMapValue(map, movePostion, MazeData.Way);

            return;
        }

        /// <summary>
        /// 指定された方角に位置を移動した位置を返却します。
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="way">方角</param>
        /// <param name="stair">0の場合は、同一階層。1は1階層上。-1は1階層下</param>
        /// <param name="step">移動距離</param>
        /// <returns>移動した位置</returns>
        private (int, int, int) Move((int, int, int) position, int way, int stair = 0, int step = 1)
        {
            (int, int, int) movePostion = position;
            switch (way)
            {
                case MazeData.North:
                    movePostion = (position.Item1 + stair, position.Item2, position.Item3 + step);
                    break;

                case MazeData.South:
                    movePostion = (position.Item1 + stair, position.Item2, position.Item3 - step);
                    break;

                case MazeData.East:
                    movePostion = (position.Item1 + stair, position.Item2 + step, position.Item3);
                    break;

                case MazeData.West:
                    movePostion = (position.Item1 + stair, position.Item2 - step, position.Item3);
                    break;
            }

            return movePostion;
        }

        /// <summary>
        /// 階段にするかどうかをランダムに決定します。
        /// </summary>
        /// <returns>階段にする場合、true。しない場合、false</returns>
        private bool PriorStair()
        {
            int p = this.Rnd.Next(0, 255);
            return p < this.StairP;
        }

        /// <summary>
        /// 指定された方角の反対を返します。
        /// </summary>
        /// <param name="way">方角</param>
        /// <returns>反対の方角</returns>
        private int Turn(int way)
        {
            int turnWay = way;
            switch(way)
            {
                case MazeData.North:
                    turnWay = MazeData.South;
                    break;
                case MazeData.South:
                    turnWay = MazeData.North;
                    break;
                case MazeData.East:
                    turnWay = MazeData.West;
                    break;
                case MazeData.West:
                    turnWay = MazeData.East;
                    break;
            }

            return turnWay;
        }

        /// <summary>
        /// 論理的な位置をmap上での位置に変換します。
        /// </summary>
        /// <param name="edgePosition">論理的な位置</param>
        /// <returns>map上での位置</returns>
        private (int, int, int) ConvertPosition((int, int, int) edgePosition)
        {
            return (edgePosition.Item1 + 1, edgePosition.Item2 * 2 + 2, edgePosition.Item3 * 2 + 2);
        }
    }
}
