using System;
using System.Windows.Forms;
using AlibreAddOn;
using AlibreX;
public class cmd6 : IAlibreAddOnCommand
{
    private IADAddOnCommandSite commandSite;
    private bool isOutOfDate = true;
    private double triangleOffsetX = 0;
    private Timer _timer;
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
    public void OnShowUI(long hWnd)
    {
        _timer = new Timer();
        _timer.Interval = 50;
        _timer.Tick += (_, __) =>
        {
            triangleOffsetX += 1;
            commandSite.InvalidateCanvas();
        };
        _timer.Start();
    }
    public void On3DRender()
    {
        if (!isOutOfDate) return;
        var canvas = (IADAddOnCanvasDisplay)commandSite.Begin3DDisplay(false);
        float[] verticesRaw = { -10, 0, 0, 10, 0, 0, 0, 10, 0 };
        float[] normalsRaw = { 0, 0, 1, 0, 0, 1, 0, 0, 1 };
        int[] indicesRaw = { 3, 0, 1, 2 };
        double[] transformRaw = {
            1, 0, 0,
            0, 1, 0,
            0, 0, 1,
            triangleOffsetX, 0, 0
        };
        Array vertices = verticesRaw;
        Array normals = normalsRaw;
        Array indices = indicesRaw;
        Array transform = transformRaw;
        long segment = canvas.AddSubSegment(0, "TriangleSegment");
        canvas.SetSegmentTransform(segment, true, ref transform);
        canvas.SetSegmentColor(segment, 200, 100, 0, 255);
        canvas.DrawMesh(segment, ref vertices, ref normals, ref indices);
        commandSite.End3DDisplay();
        isOutOfDate = false;
    }
    public void OnKeyDown(int key)
    {
        if (key == 39)
        {
            triangleOffsetX += 5;
            commandSite.InvalidateCanvas();
        }
    }
    public void OnComplete()
    {
        _timer?.Stop();
        isOutOfDate = true;
        commandSite.InvalidateCanvas();
        commandSite.Terminate(); 
    }
    public void OnEscape()
    {
        _timer?.Stop();
        commandSite.Terminate();
    }
    public bool AddTab() => false;
    public string TabName => null;
    public Array Extents => null;
    public void OnRender(int _, int __, int ___, int ____, int _____) { }
    public void OnClick(int x, int y, ADDONMouseButtons b) { }
    public void OnDoubleClick(int x, int y) { }
    public void OnMouseDown(int x, int y, ADDONMouseButtons b) { }
    public void OnMouseMove(int x, int y, ADDONMouseButtons b) { }
    public void OnMouseUp(int x, int y, ADDONMouseButtons b) { }
    public void OnMouseWheel(double d) { }
    public void OnKeyUp(int c) { }
    public void OnSelectionChange() { }
    public void OnTerminate() { }
    public bool IsTwoWayToggle() => false;
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