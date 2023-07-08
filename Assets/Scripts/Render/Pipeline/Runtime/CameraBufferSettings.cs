using UnityEngine;

[System.Serializable]
public struct CameraBufferSettings
{
	public bool CopyColor, CopyColorReflection, CopyDepth, CopyDepthReflection;
    public Vector2Int BufferSize;
}