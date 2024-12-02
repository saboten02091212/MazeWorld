using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    public class MazeUtility
    {
        /// <summary>
        /// mapから指定されたTuple型の位置の値を返却します。
        /// </summary>
        /// <param name="map">対象マップ</param>
        /// <param name="position">取得位置</param>
        /// <returns>mapの値</returns>
        public static int GetMapValue(int[,,] map, (int, int, int) position)
        {
            return map[position.Item1, position.Item2, position.Item3];
        }

        /// <summary>
        /// mapから指定されたTuple型の位置の値を設定します。
        /// </summary>
        /// <param name="map">対象マップ</param>
        /// <param name="position">設定位置</param>
        /// <param name="setValue">設定値</param>
        public static void SetMapValue(int[,,] map, (int, int, int) position, int setValue)
        {
            map[position.Item1, position.Item2, position.Item3] = setValue;
        }
    }
}
