using UnityEngine;

namespace KohaneEngine.Scripts.Utils
{
    public static class UIUtils
    {
        
        /// <summary>
        /// Map (-1~1, -1~1) to (CanvasWidth, CanvasHeight)
        /// </summary>
        /// <param name="scriptPosition"></param>
        /// <returns></returns>
        public static Vector2 ScriptPositionToCanvasPosition(Vector2 scriptPosition)
        {
            return new Vector2(Constants.CanvasWidth / 2f * scriptPosition.x,
                Constants.CanvasHeight / 2f * scriptPosition.y);
        }
    }
}