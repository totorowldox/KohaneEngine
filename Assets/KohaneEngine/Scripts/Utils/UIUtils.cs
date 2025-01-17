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
        
        /// <summary>
        /// Map (CanvasWidth, CanvasHeight) back to (-1~1, -1~1)
        /// </summary>
        /// <param name="canvasPosition"></param>
        /// <returns></returns>
        public static Vector2 CanvasPositionToScriptPosition(Vector2 canvasPosition)
        {
            return new Vector2(canvasPosition.x / Constants.CanvasWidth * 2f - 1f,
                canvasPosition.y / Constants.CanvasHeight * 2f - 1f);
        }
    }
}