using System;
using System.Windows;
using AlibreAddOn;
using AlibreX;
public class cmd7 : IAlibreAddOnCommand
{
    private IADAddOnCommandSite commandSite;
    private bool isOutOfDate = true;
    public IADAddOnCommandSite CommandSite
    {
        get => commandSite;
        set => commandSite = value;
    }
    public void putref_CommandSite(IADAddOnCommandSite pSite)
    {
        commandSite = pSite;
        bool isLegacy = commandSite.LegacyRenderingEngine();
        if (isLegacy)
            throw new InvalidOperationException("");
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
        object canvasObj = commandSite.Begin3DDisplay(false);
        var canvas = (IADAddOnCanvasDisplay)canvasObj;
        DrawTriangle(canvas);
        isOutOfDate = false;
    }
    private void DrawTriangle(IADAddOnCanvasDisplay canvas)
    {
        Array vertices = new float[] { -5, 0, 0, 5, 0, 0, 0, 5, 0 };
        Array normals = new float[] { 0, 0, 1, 0, 0, 1, 0, 0, 1 };
        Array indices = new int[] { 3, 0, 1, 2 };
        Array transform = new double[]
        {
            1, 0, 0,
            0, 1, 0,
            0, 0, 1,
            0, 0, 0
        };
        long segment = canvas.AddSubSegment(0, "HOOPS_Triangle");
        canvas.SetSegmentTransform(segment, true, ref transform);
        canvas.SetSegmentColor(segment, 255, 128, 0, 255); // Orange
        canvas.DrawMesh(segment, ref vertices, ref normals, ref indices);
        canvas.DrawPolyline(segment, ref vertices);
        commandSite.End3DDisplay();
    }
    public void OnClick(int screenX, int screenY, ADDONMouseButtons buttons) { }
    public void OnDoubleClick(int screenX, int screenY)
    {
        OnEscape(); // Same behavior as Escape
    }
    public void OnMouseDown(int screenX, int screenY, ADDONMouseButtons buttons) { }
    public void OnMouseMove(int screenX, int screenY, ADDONMouseButtons buttons) { }
    public void OnMouseUp(int screenX, int screenY, ADDONMouseButtons buttons) { }
    public void OnMouseWheel(double delta) { }
    public void OnKeyDown(int keyCode) { }
    public void OnKeyUp(int keyCode) { }
    public void OnEscape()
    {
        commandSite.Override3DRender(false);
        commandSite.InvalidateCanvas();
        commandSite.Terminate();
    }
    public void OnSelectionChange() { }
    public void OnTerminate() { }
    public bool IsTwoWayToggle() => false;
    public void OnRender(int hDC, int clipRectX, int clipRectY, int clipRectWidth, int clipRectHeight)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnClick(int screenX, int screenY, ADDONMouseButtons buttons)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnDoubleClick(int screenX, int screenY)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnMouseDown(int screenX, int screenY, ADDONMouseButtons buttons)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnMouseMove(int screenX, int screenY, ADDONMouseButtons buttons)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnMouseUp(int screenX, int screenY, ADDONMouseButtons buttons)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnKeyDown(int keycode)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnKeyUp(int keycode)
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnEscape()
    {
        throw new NotImplementedException();
    }
    bool IAlibreAddOnCommand.OnMouseWheel(double delta)
    {
        throw new NotImplementedException();
    }
}
