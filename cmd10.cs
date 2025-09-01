using System;
using System.Windows.Forms;
using AlibreAddOn;
using AlibreX;
public class cmd10 : IAlibreAddOnCommand
{
    private IADAddOnCommandSite commandSite;
    private bool isOutOfDate = true;
    private long tempSegmentId = -1;
    public IADAddOnCommandSite CommandSite
    {
        get => commandSite;
        set => commandSite = value;
    }
    public void putref_CommandSite(IADAddOnCommandSite pSite)
    {
        commandSite = pSite;
        if (commandSite.LegacyRenderingEngine())
            throw new InvalidOperationException("HOOPS");
    }
    public bool AddTab() => false;
    public string TabName => null;
    public Array Extents => null;
    public void OnShowUI(long hWnd) { }
    public void OnComplete()
    {
        isOutOfDate = true;
        commandSite.InvalidateCanvas();
    }
    public void On3DRender()
    {
        if (!isOutOfDate) return;
        var canvas = (IADAddOnCanvasDisplay)commandSite.Begin3DDisplay(false);
        if (tempSegmentId != -1)
            canvas.DeleteSegment(tempSegmentId);
        float[] vertices = {
            -10, -10, 0,
            10, -10, 0,
            10, 10, 0,
            -10, 10, 0
        };
        float[] normals = {
            0, 0, 1,
            0, 0, 1,
            0, 0, 1,
            0, 0, 1
        };
        int[] indices = { 4, 0, 1, 2, 3 };
        Array v = vertices, n = normals, i = indices;
        double[] transform = {
            1, 0, 0,
            0, 1, 0,
            0, 0, 1,
            0, 0, 0
        };
        Array t = transform;
        tempSegmentId = canvas.AddSubSegment(0, "TempSquareSegment");
        canvas.SetSegmentTransform(tempSegmentId, true, ref t);
        canvas.SetSegmentColor(tempSegmentId, 0, 255, 0, 128); // Semi-transparent green
        canvas.DrawMesh(tempSegmentId, ref v, ref n, ref i);
        commandSite.End3DDisplay();
        isOutOfDate = false;
    }
    public void OnDoubleClick(int screenX, int screenY)
    {
        MessageBox.Show("Temporary geometry drawn with HOOPS.", "Info");
    }
    public bool IsTwoWayToggle() => false;
    public void OnRender(int hDC, int clipRectX, int clipRectY, int clipRectWidth, int clipRectHeight)
    {
        throw new NotImplementedException();
    }
    public bool OnClick(int screenX, int screenY, ADDONMouseButtons buttons)
    {
        return false;
    }
    bool IAlibreAddOnCommand.OnDoubleClick(int screenX, int screenY)
    {
        return false;
    }
    public bool OnMouseDown(int screenX, int screenY, ADDONMouseButtons buttons)
    {
        return false;
    }
    public bool OnMouseMove(int screenX, int screenY, ADDONMouseButtons buttons)
    {
        return false;
    }
    public bool OnMouseUp(int screenX, int screenY, ADDONMouseButtons buttons)
    {
        return false;
    }
    public void OnSelectionChange()
    {
    }
    public void OnTerminate()
    {
    }
    public bool OnKeyDown(int keycode)
    {
        return false;
    }
    public bool OnKeyUp(int keycode)
    {
        return false;
    }
    public bool OnEscape()
    {
        return false;
    }
    public bool OnMouseWheel(double delta)
    {
        return false;
    }
}
