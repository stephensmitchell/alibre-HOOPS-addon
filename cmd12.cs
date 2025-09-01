using System;
using AlibreAddOn;
using AlibreX;
public class cmd12 : IAlibreAddOnCommand
{
    private IADAddOnCommandSite commandSite;
    private bool isOutOfDate = true;
    private long wireframeSegmentId = -1;
    private int? selectedNode = null;
    private bool isDragging = false;
    private float[,] nodes;
    private int[,] bars;
    public cmd12()
    {
        nodes = new float[,]
        {
            { 0, 0, 0 },   
            { 10, 0, 0 },  
            { 5, 8, 0 },   
            { 15, 8, 0 }   
        };
        bars = new int[,]
        {
            { 0, 1 },
            { 0, 2 },
            { 1, 2 },
            { 1, 3 },
            { 2, 3 }
        };
    }
    public IADAddOnCommandSite CommandSite
    {
        get => commandSite;
        set => commandSite = value;
    }
    public void putref_CommandSite(IADAddOnCommandSite pSite)
    {
        commandSite = pSite;
        if (commandSite.LegacyRenderingEngine())
            throw new InvalidOperationException("This command requires the modern rendering engine (HOOPS).");
    }
    public bool AddTab() => false;
    public string TabName => null;
    public Array Extents => null;
    public void OnShowUI(long hWnd) { }
    public void OnComplete()
    {
    isOutOfDate = true;
    commandSite.InvalidateCanvas();
    commandSite.Terminate();
    }
    public void OnSelectionChange() { }
    public bool IsTwoWayToggle() => false;
    public void OnRender(int _, int __, int ___, int ____, int _____) { }
    public bool OnClick(int x, int y, ADDONMouseButtons b) { return false; }
    public bool OnDoubleClick(int x, int y) { return false; }
    public bool OnKeyDown(int keycode) { return false; }
    public bool OnKeyUp(int c) { return false; }
    public bool OnEscape()
    {
        commandSite.Terminate();
        return false;
    }
    public void OnTerminate()
    {
        commandSite.Terminate();
    }
    public bool OnMouseWheel(double d) { return false; }
    public void On3DRender()
    {
        if (!isOutOfDate) return;
        var canvas = (IADAddOnCanvasDisplay)commandSite.Begin3DDisplay(false);
        if (wireframeSegmentId != -1)
            canvas.DeleteSegment(wireframeSegmentId);
        wireframeSegmentId = canvas.AddSubSegment(0, "TrussWireframeSegment");
    double[] transform = { 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0 }; 
    Array t = transform;
    canvas.SetSegmentTransform(wireframeSegmentId, true, ref t);
        canvas.SetSegmentColor(wireframeSegmentId, 0, 0, 0, 255);         for (int i = 0; i < bars.GetLength(0); i++)
        {
            int n1 = bars[i, 0];
            int n2 = bars[i, 1];
            float[] polylinePoints = {
                nodes[n1, 0], nodes[n1, 1], nodes[n1, 2],
                nodes[n2, 0], nodes[n2, 1], nodes[n2, 2]
            };
            Array pts = polylinePoints;
            canvas.DrawPolyline(wireframeSegmentId, ref pts);
        }
        for (int i = 0; i < nodes.GetLength(0); i++)
        {
            float r = (selectedNode == i) ? 2.0f : 1.2f;
            canvas.DrawMarker(wireframeSegmentId, nodes[i, 0], nodes[i, 1], nodes[i, 2], 1, MarkerType.MARKER_CROSSHAIR);
        }
        commandSite.End3DDisplay();
        isOutOfDate = false;
    }
    public bool OnMouseDown(int x, int y, ADDONMouseButtons b)
    {
        Array screenXY = new int[] { x, y };
        Array modelXYZ = commandSite.ScreenToWorld(ref screenXY);
        double modelX = (double)modelXYZ.GetValue(0);
        double modelY = (double)modelXYZ.GetValue(1);
        int? nearest = null;
        float minDistSq = 25f;         for (int i = 0; i < nodes.GetLength(0); i++)
        {
            float dx = nodes[i, 0] - (float)modelX;
            float dy = nodes[i, 1] - (float)modelY;
            float distSq = dx * dx + dy * dy;
            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                nearest = i;
            }
        }
        if (nearest.HasValue)
        {
            selectedNode = nearest.Value;
            isDragging = true;
            isOutOfDate = true; 
            commandSite.InvalidateCanvas();
        }
        return true;
    }
    public bool OnMouseMove(int x, int y, ADDONMouseButtons b)
    {
        if (isDragging && selectedNode.HasValue)
        {
            Array screenXY = new int[] { x, y };
            Array modelXYZ = commandSite.ScreenToWorld(ref screenXY);
            double modelX = (double)modelXYZ.GetValue(0);
            double modelY = (double)modelXYZ.GetValue(1);
            nodes[selectedNode.Value, 0] = (float)modelX;
            nodes[selectedNode.Value, 1] = (float)modelY;
            isOutOfDate = true;
            commandSite.InvalidateCanvas(); 
        }
        return true;
    }
    public bool OnMouseUp(int x, int y, ADDONMouseButtons b)
    {
        if (isDragging)
        {
            isDragging = false;
            selectedNode = null;
            isOutOfDate = true; 
            commandSite.InvalidateCanvas();
        }
        return true;
    }
}