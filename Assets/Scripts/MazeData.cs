using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    /// <summary>
    /// 3D迷路を構築するために必要な情報を保持するクラス
    /// </summary>
    public class MazeData
    {
        /// <summary>
        /// 壁(1bit目)
        /// </summary>
        public const int Wall = 1;
        /// <summary>
        /// 道
        /// </summary>
        public const int Way = 0;

        /// <summary>
        /// 登り階段(2bit目)
        /// </summary>
        public const int UpStair = 1 << 1;
        /// <summary>
        /// 下り階段(3bit目)
        /// </summary>
        public const int DownStair = 1 << 2;

        /// <summary>
        /// 北(4bit目)
        /// </summary>
        public const int North = 1 << 3;
        /// <summary>
        /// 南(5bit目)
        /// </summary>
        public const int South = 1 << 4;
        /// <summary>
        /// 東(6bit目)
        /// </summary>
        public const int East = 1 << 5;
        /// <summary>
        /// 西(7bit目)
        /// </summary>
        public const int West = 1 << 6;

        public const int UpStairNorth = UpStair + North;
        public const int UpStairSouth = UpStair + South;
        public const int UpStairEast = UpStair + East;
        public const int UpStairWest = UpStair + West;
        public const int DownStairNorth = DownStair + North;
        public const int DownStairSouth = DownStair + South;
        public const int DownStairEast = DownStair + East;
        public const int DownStairWest = DownStair + West;

        /// <summary>
        /// MazeMap上での横幅
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// MazeMap上での高さ
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// MazeMap上での奥行
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// 迷路のマップ情報
        /// </summary>
        public int[,,] Map { get; internal set; }

        /// <summary>
        /// MazeMap上でのスタート位置
        /// </summary>
        public (int, int, int) StartPosition { get; internal set; }
        /// <summary>
        /// MazeMap上でのゴール位置
        /// </summary>
        public (int, int, int) GorlPosition { get; internal set; }


        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="x">高さ</param>
        /// <param name="y">横幅</param>
        /// <param name="z">奥行</param>
        public MazeData(int x, int y, int z)
        {
            this.Height = x;
            this.Width = y * 2 + 1;
            this.Depth = z * 2 + 1;

            this.Map = new int[this.Height, this.Width, this.Depth];
        }
    }
}
