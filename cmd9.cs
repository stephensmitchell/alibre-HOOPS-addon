using System;
using System.Windows.Forms;
using AlibreAddOn;
using AlibreX;

public class cmd9 : IAlibreAddOnCommand
{
    private IADAddOnCommandSite commandSite;
    private bool isOutOfDate = true;
    private double triangleOffsetX = 0;
    private Timer _timer;
    private long staticSegmentId = -1;
    private long dynamicSegmentId = -1;

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
        _timer.Interval = 100;
        _timer.Tick += (_, __) =>
        {
            triangleOffsetX += 0.5;
            isOutOfDate = true;
            commandSite.InvalidateCanvas();
        };
        _timer.Start();
    }

    public void On3DRender()
    {
        if (!isOutOfDate) return;

        var canvas = (IADAddOnCanvasDisplay)commandSite.Begin3DDisplay(false);

        float[] vertices = { -10, 0, 0, 10, 0, 0, 0, 10, 0 };
        float[] normals = { 0, 0, 1, 0, 0, 1, 0, 0, 1 };
        int[] indices = { 3, 0, 1, 2 };

        Array v = vertices, n = normals, i = indices;

        if (staticSegmentId != -1)
            canvas.DeleteSegment(staticSegmentId);
        if (dynamicSegmentId != -1)
            canvas.DeleteSegment(dynamicSegmentId);
        double[] staticTransform = {
            1, 0, 0,
            0, 1, 0,
            0, 0, 1,
            -30, 0, 0
        };
        Array t1 = staticTransform;
        staticSegmentId = canvas.AddSubSegment(0, "StaticTriangleSegment");
        canvas.SetSegmentTransform(staticSegmentId, true, ref t1);
        canvas.SetSegmentColor(staticSegmentId, 100, 100, 255, 255);
        canvas.DrawMesh(staticSegmentId, ref v, ref n, ref i);
        double[] movingTransform = {
            1, 0, 0,
            0, 1, 0,
            0, 0, 1,
            triangleOffsetX, 0, 0
        };
        Array t2 = movingTransform;
        dynamicSegmentId = canvas.AddSubSegment(0, "DynamicTriangleSegment");
        canvas.SetSegmentTransform(dynamicSegmentId, true, ref t2);
        canvas.SetSegmentColor(dynamicSegmentId, 255, 100, 100, 255);
        canvas.DrawMesh(dynamicSegmentId, ref v, ref n, ref i);

        commandSite.End3DDisplay();
        isOutOfDate = false;
    }

    public void OnKeyDown(int key)
    {
        if (key == 39) // VK_RIGHT
        {
            triangleOffsetX += 5;
            isOutOfDate = true;
            commandSite.InvalidateCanvas();
        }
    }

    public void OnEscape()
    {
        _timer?.Stop();
        commandSite.Terminate();
    }

    public void OnComplete()
    {
        _timer?.Stop();
        isOutOfDate = true;
        commandSite.InvalidateCanvas();
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