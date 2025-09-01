using System;
using System.Windows.Forms;
using AlibreAddOn;
using AlibreX;
public class cmd11 : IAlibreAddOnCommand
{
    private IADAddOnCommandSite commandSite;
    private bool isOutOfDate = true;
    private long wireframeSegmentId = -1;
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
        if (wireframeSegmentId != -1)
            canvas.DeleteSegment(wireframeSegmentId);
        double[,] nodes = new double[,]
        {
            { 0, 0, 0 },    // Node 0
            { 10, 0, 0 },   // Node 1
            { 5, 8, 0 },    // Node 2
            { 15, 8, 0 }    // Node 3
        };
        int[,] bars = new int[,]
        {
            { 0, 1 },
            { 0, 2 },
            { 1, 2 },
            { 1, 3 },
            { 2, 3 }
        };
        wireframeSegmentId = canvas.AddSubSegment(0, "TrussWireframeSegment");
        double[] transform = {
            1, 0, 0,
            0, 1, 0,
            0, 0, 1,
            0, 0, 0
        };
        Array t = transform;
        canvas.SetSegmentTransform(wireframeSegmentId, true, ref t);
        canvas.SetSegmentColor(wireframeSegmentId, 0, 0, 0, 255); // Black lines
        for (int i = 0; i < bars.GetLength(0); i++)
        {
            int n1 = bars[i, 0];
            int n2 = bars[i, 1];
            float[] polylinePoints = {
                (float)nodes[n1, 0], (float)nodes[n1, 1], (float)nodes[n1, 2],
                (float)nodes[n2, 0], (float)nodes[n2, 1], (float)nodes[n2, 2]
            };
            Array pts = polylinePoints;
            canvas.DrawPolyline(wireframeSegmentId, ref pts);
        }
        commandSite.End3DDisplay();
        isOutOfDate = false;
    }
    public void OnDoubleClick(int screenX, int screenY)
    {
        MessageBox.Show("2D Truss wireframe generated.", "Info");
    }
    public void OnRender(int _, int __, int ___, int ____, int _____) { }
    public void OnClick(int x, int y, ADDONMouseButtons b) { }
    public void OnMouseDown(int x, int y, ADDONMouseButtons b) { }
    public void OnMouseMove(int x, int y, ADDONMouseButtons b) { }
    public void OnMouseUp(int x, int y, ADDONMouseButtons b) { }
    public void OnMouseWheel(double d) { }
    public void OnKeyUp(int c) { }
    public void OnSelectionChange() { }
    public void OnTerminate() { }
    public bool IsTwoWayToggle() => false;
    bool IAlibreAddOnCommand.OnClick(int screenX, int screenY, ADDONMouseButtons buttons) { return false; }
    bool IAlibreAddOnCommand.OnDoubleClick(int screenX, int screenY) { return false; }
    bool IAlibreAddOnCommand.OnMouseDown(int screenX, int screenY, ADDONMouseButtons buttons) { return false; }
    bool IAlibreAddOnCommand.OnMouseMove(int screenX, int screenY, ADDONMouseButtons buttons) { return false; }
    bool IAlibreAddOnCommand.OnMouseUp(int screenX, int screenY, ADDONMouseButtons buttons) { return false; }
    bool IAlibreAddOnCommand.OnKeyDown(int keycode) { return false; }
    bool IAlibreAddOnCommand.OnKeyUp(int keycode) { return false; }
    bool IAlibreAddOnCommand.OnEscape() { return false; }
    bool IAlibreAddOnCommand.OnMouseWheel(double delta) { return false; }
}
