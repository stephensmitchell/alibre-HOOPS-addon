using System;
using System.Collections.Generic;
using AlibreAddOn;
public class cmd2 : IAlibreAddOnCommand
{
    private IADAddOnCommandSite _site;
    private bool _dirty = true;
    private bool _meshBuilt = false;
    private long _meshSeg = -1;   
    private double _scale = 1.0;  
    public IADAddOnCommandSite CommandSite
    {
        get => _site;
        set => _site = value;
    }
    public void putref_CommandSite(IADAddOnCommandSite pSite)
    {
        _site = pSite ?? throw new ArgumentNullException(nameof(pSite));
        if (_site.LegacyRenderingEngine())
            throw new InvalidOperationException("HOOPS");
        _site.Override3DRender(true);
    }
    public void On3DRender()
    {
        var canvas = (IADAddOnCanvasDisplay)_site.Begin3DDisplay(false);
        try
        {
            EnsureMesh(canvas);
        }
        finally
        {
            _site.End3DDisplay();
        }
        _dirty = false;
    }
    private void EnsureMesh(IADAddOnCanvasDisplay canvas)
    {
        if (_meshSeg == -1)
            _meshSeg = canvas.AddSubSegment(0, "GeneratedMesh");
        if (!_meshBuilt)
        {
            BuildMesh(canvas, _meshSeg);
            _meshBuilt = true;
        }
    }
    private static void BuildMesh(IADAddOnCanvasDisplay canvas, long seg)
    {
        const int div = 40;
        const double size = 30.0;
        const double amp = 2.0;
        const double freq = 0.5;
        int gw = div + 1, gh = div + 1;
        var verts = new List<double>(gw * gh * 3);
        var norms = new List<double>(gw * gh * 3);
        var idx = new List<int>(div * div * 2 * 4);
        for (int j = 0; j < gh; ++j)
            for (int i = 0; i < gw; ++i)
            {
                double x = (i / (double)div - 0.5) * size;
                double y = (j / (double)div - 0.5) * size;
                double z = amp * (Math.Sin(x * freq) + Math.Cos(y * freq));
                verts.Add(x); verts.Add(y); verts.Add(z);
                norms.Add(0); norms.Add(0); norms.Add(1);
            }
        for (int j = 0; j < div; ++j)
            for (int i = 0; i < div; ++i)
            {
                int bl = j * gw + i;
                int br = bl + 1;
                int tl = (j + 1) * gw + i;
                int tr = tl + 1;
                idx.Add(3); idx.Add(bl); idx.Add(br); idx.Add(tr);
                idx.Add(3); idx.Add(bl); idx.Add(tr); idx.Add(tl);
            }
        Array vArr = verts.ToArray();
        Array nArr = norms.ToArray();
        Array fArr = idx.ToArray();
        Array identity = new double[]
        {
            1,0,0,0,
            0,1,0,0,
            0,0,1,0,
            0,0,0,1
        };
        canvas.SetSegmentTransform(seg, true, ref identity);
        canvas.SetSegmentColor(seg, 0, 128, 255, 255);
        canvas.DrawMesh(seg, ref vArr, ref nArr, ref fArr);
        canvas.DrawScreenText(seg, "Generated mesh",
                                  100, 100, TextAlignment.Center,
                                  "Arial", 100, 5, true);
    }
    public bool OnMouseWheel(double delta)
    {
        _scale *= delta > 0 ? 1.1 : 0.9;
        var canvas = (IADAddOnCanvasDisplay)_site.Begin3DDisplay(false);
        try
        {
            Array m = new double[]
            {
                _scale,0,     0,     0,
                0,     _scale,0,     0,
                0,     0,     _scale,0,
                0,     0,     0,     1
            };
            canvas.SetSegmentTransform(_meshSeg, true, ref m);
        }
        finally { _site.End3DDisplay(); }
        return true;
    }
    public bool OnMouseDown(int x, int y, ADDONMouseButtons b) => true;
    public bool OnMouseMove(int x, int y, ADDONMouseButtons b) => true;
    public bool OnMouseUp(int x, int y, ADDONMouseButtons b) => true;
    public bool OnClick(int x, int y, ADDONMouseButtons b) => false;
    public bool OnDoubleClick(int x, int y) => false;
    public bool OnKeyDown(int key)
    {
        if (key == 27) return OnEscape(); 
        if (key == 82)                     
        {
            _meshBuilt = false; _dirty = true;
        }
        return true;
    }
    public bool OnKeyUp(int key) => true;
    public bool OnEscape()
    {
        _site.Override3DRender(false);
        _site.InvalidateCanvas();
        _site.Terminate();
        return true;
    }
    public void OnComplete() { _dirty = true; }
    public void OnSelectionChange() { _dirty = true; }
    public void OnTerminate() { _site?.Override3DRender(false); }
    public bool AddTab() => false;
    public string TabName => null;
    public Array Extents => null;
    public void OnShowUI(long hWnd) { }
    public bool IsTwoWayToggle() => false;
    public void OnRender(int hDC, int x, int y, int w, int h) { }
}
